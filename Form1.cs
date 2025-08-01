using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AcuEnvManager
{
    public partial class Form1 : Form
    {
        public WorktreeController wtcontroller { get; set; }
        private BindingList<WorktreeModel> worktreeList = new();

        public ActionController actionController { get; set; }
        private BindingList<ActionModel> actionQueueList = new();
        private List<ActionModel> tempQueue { get; set; } = new List<ActionModel>();

        public List<BuildModel> buildList { get; set; } = new List<BuildModel>();
        public SettingsModel settings { get; set; }
        public const string settingsPath = @"C:\Temp\AcuEnvManager\settings.xml";


        private object? _oldValue;
        private int _oldRowIndex = -1;


        public Form1()
        {
            InitializeComponent();
            // Do NOT start async loading here!
            this.Load += Form1_Load;
        }

        private async void Form1_Load(object? sender, EventArgs e)
        {
            settings = XMLHelper.LoadSettings(settingsPath);
            settings.FastCopy = true; // Enable FastCopy by default
            wtcontroller = new WorktreeController(settings);
            worktreeView.DataSource = worktreeList;

            actionController = new ActionController(settings);
            actionQueueView.DataSource = actionQueueList;
            InitializeWorktreeView();
            InitializeActionQueueView();
            LoadSavedData();
            if (DateTime.Now - settings.LastSave > TimeSpan.FromHours(1))
            {
                await UpdateWorktreeDetailsAsync();
            }
            else
                txtLog.Text = $"Last update: {settings.LastSave.ToShortTimeString()}";

            EnableFields();
        }

        private void EnableFields()
        {
            worktreeView.Columns["WorkTreeName"].ReadOnly = true;
            worktreeView.Columns["CodeVers"].ReadOnly = true;
            worktreeView.Columns["DBVers"].ReadOnly = true;
        }

        private void InitializeWorktreeView()
        {
            worktreeList.Add(new WorktreeModel
            {
                WorkTreeName = "",
                CodeVers = "",
                BranchName = "",
                DBServer = "",
                DBName = "",
                DBVers = ""
            });

            worktreeView.Columns["WorkTreeName"].HeaderText = "Worktree Name";
            worktreeView.Columns["WorkTreeName"].Width = 200;

            worktreeView.Columns["CodeVers"].HeaderText = "Code Version";
            worktreeView.Columns["CodeVers"].Width = 150;

            worktreeView.Columns["BranchName"].HeaderText = "Branch Name";

            worktreeView.Columns["BranchName"].Width = 300;

            worktreeView.Columns["DBServer"].HeaderText = "DB Server";

            worktreeView.Columns["DBServer"].Width = 200;

            worktreeView.Columns["DBName"].HeaderText = "DB Name";
            worktreeView.Columns["DBName"].Width = 200;

            worktreeView.Columns["DBVers"].HeaderText = "DB Version";
            worktreeView.Columns["DBVers"].Width = 200;

            // Add a link column for JIRA
            worktreeView.Columns.Remove("JIRALink"); // Remove any existing JIRALink column if it exists
            var linkCol = new DataGridViewLinkColumn
            {
                Name = "JIRALink",
                HeaderText = "JIRA Link",
                DataPropertyName = "JIRALink",
                UseColumnTextForLinkValue = false,
                LinkBehavior = LinkBehavior.HoverUnderline,
                TrackVisitedState = true,
            };
            worktreeView.Columns.Add(linkCol);
            worktreeView.Columns["JIRALink"].Width = 200;

            worktreeView.CellBeginEdit += worktreeView_CellBeginEdit;
            worktreeView.CellValueChanged += worktreeView_CellValueChanged;
            //worktreeView.CellEndEdit += worktreeView_CellEndEdit;
            worktreeView.CellFormatting += worktreeView_CellFormatting;

        }

        private void InitializeActionQueueView()
        {
            // Hide the ActionName column
            actionQueueView.Columns["ActionName"].Visible = false;
            actionQueueView.Columns["Entity"].Visible = false;
            actionQueueView.Columns["EntityName"].Visible = false;

            // Optionally, set the header text for the description column
            actionQueueView.Columns["WorkTreeName"].HeaderText = "Worktree Name";
            actionQueueView.Columns["WorkTreeName"].DisplayIndex = 0;
            actionQueueView.Columns["ActionDescription"].HeaderText = "Action";
            actionQueueView.Columns["ActionDescription"].DisplayIndex = 1;
            actionQueueView.Columns["EntityDescription"].HeaderText = "Entity";
            actionQueueView.Columns["EntityDescription"].DisplayIndex = 2;

            actionQueueView.Columns["Value"].HeaderText = "Value";
            actionQueueView.Columns["Value"].Width = 200;

            var buttonColumn = new DataGridViewButtonColumn
            {
                Name = "StartActionButton",
                HeaderText = "Start",
                Text = "Start Now",
                UseColumnTextForButtonValue = true
            };
            actionQueueView.Columns.Add(buttonColumn);

            actionQueueView.CellContentClick += actionQueueView_CellContentClick;
        }

        private void LoadSavedData()
        {
            try
            {
                this.Invoke(() => statusLabel.Text = "Loading worktree names...");

                var savedWorktees = wtcontroller.GetWorktrees();
                worktreeView.Invoke(() =>
                {
                    worktreeView.ReadOnly = true; // Set to read-only initially
                    foreach (var worktree in savedWorktees)
                    {
                        worktreeList.Add(worktree);
                    }
                    worktreeView.ReadOnly = false; // Enable editing after loading
                });
                buildList = XMLHelper.LoadBuilds(settings.SettingsPath);                
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Error loading worktrees:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Invoke(() => statusLabel.Text = "Error loading worktrees.");
            }
            finally
            {
                worktreeList.RemoveAt(0); // Remove the initial empty entry
            }
        }

        private async Task UpdateWorktreeDetailsAsync()
        {
            try
            {
                this.Invoke(() => statusLabel.Text = "Populating worktree details...");

                // Run all updates in parallel (if thread-safe) or sequentially, but do not update the view yet
                List<Task> updateTasks = worktreeList
                    .Select(worktree => Task.Run(() => wtcontroller.UpdateWorktreeDetails(worktree, GetLogger)))
                    .ToList();

                if(buildList.Count == 0)
                {
                    updateTasks.Add(Task.Run(() => FileSystemHelper.GetBuildListAsync(settings, buildList, GetLogger)));
                }

                await Task.WhenAll(updateTasks);

                // Now update the view once
                RefreshWorktreeView();
                XMLHelper.SaveData(worktreeList, buildList, settings, GetLogger);


                this.Invoke(() => statusLabel.Text = $"Loaded {worktreeList.Count} worktrees.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Error updating worktree details:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Invoke(() => statusLabel.Text = "Error updating worktree details.");
            }
        }

        private void RefreshWorktreeView()
        {
            worktreeView.Invoke(() =>
            {
                worktreeView.ReadOnly = true;
                var currencyManager = (CurrencyManager)BindingContext[worktreeList];
                currencyManager.Refresh();
                worktreeView.ReadOnly = false;
            });
        }

        private void worktreeView_CellBeginEdit(object? sender, DataGridViewCellCancelEventArgs e)
        {
            _oldValue = worktreeView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            _oldRowIndex = e.RowIndex;
        }

        //private void worktreeView_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
        //{
        //    foreach (var action in tempQueue)
        //    {
        //        actionQueueList.Add(action);
        //    }
        //    tempQueue.Clear();
        //}

        private void worktreeView_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
        {
            var row = worktreeView.Rows[e.RowIndex];
            if (row == null) return;

            DialogResult result;
            var newValue = row.Cells[e.ColumnIndex].Value?.ToString() ?? string.Empty;
            switch (worktreeView.Columns[e.ColumnIndex].Name)
            {
                case "BranchName":
                    result = MessageBox.Show(
                        $"You changed the branch name to '{newValue}'. Do you want to build this branch after checkout?",
                        "Confirm Change",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Cancel && e.RowIndex == _oldRowIndex)
                    {
                        row.Cells[e.ColumnIndex].Value = _oldValue;
                        return;
                    }
                    AddActionToQueue(ActionType.Checkout, EntityType.Worktree, row.Cells["WorkTreeName"].Value?.ToString() ?? string.Empty, newValue);
                    if (result == DialogResult.Yes)
                    {
                        AddActionToQueue(ActionType.Acubuild, EntityType.Worktree, row.Cells["WorkTreeName"].Value?.ToString() ?? string.Empty);
                    }
                    break;
                case "DBServer":
                case "DBName":
                    var newServer = row.Cells["DBServer"].Value?.ToString() ?? string.Empty;
                    var newDatabase = row.Cells["DBName"].Value?.ToString() ?? string.Empty;
                    var dbHelper = new DatabaseHelper(newServer, newDatabase);
                    AddActionToQueue(ActionType.Update, EntityType.WebConfig, row.Cells["WorkTreeName"].Value?.ToString() ?? string.Empty, $"{newServer};{newDatabase}");
                    break;
            }
        }

        private void worktreeView_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (worktreeView.Columns[e.ColumnIndex].Name == "JIRALink")
            {
                var row = worktreeView.Rows[e.RowIndex].DataBoundItem as WorktreeModel;
                if (row != null && !string.IsNullOrEmpty(row.JIRALink))
                {
                    var match = System.Text.RegularExpressions.Regex.Match(row.BranchName, @"AC-\d+");
                    if (match.Success)
                    {
                        e.Value = match.Value;
                    }
                    e.FormattingApplied = true;
                }
            }
        }

        public void AddActionToQueue(string action, string entity, string worktreeName, string value = "")
        {
            actionQueueList.Add(new ActionModel
            {
                WorkTreeName = worktreeName,
                ActionName = action,
                Entity = entity,
                Value = value,
                Status = "Ready"
            });
        }

        private async Task ProcessActionQueueAsync()
        {
            int i = 0;
            while (i < actionQueueList.Count)
            {
                var action = actionQueueList[i];
                if (action.Status == "Completed")
                {
                    actionQueueList.RemoveAt(i); // Remove completed actions
                    i++;
                    continue; // Skip already completed actions
                }
                action.Status = "In Progress";
                actionQueueView.Refresh();
                bool result = await actionController.ExecuteActionAsync(action, GetLogger);
                action.Status = result ? "Completed" : "Failed";
                actionQueueView.Refresh();
                i++;
            }

            await UpdateWorktreeDetailsAsync(); // Refresh worktree details after processing the queue
        }

        public IProgress<string> GetLogger => new Progress<string>(status =>
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(new Action(() => AppendLog(status)));
            }
            else
            {
                AppendLog(status);
            }
        });

        private void AppendLog(string status)
        {
            txtLog.Text += status + Environment.NewLine;
            txtLog.SelectionStart = txtLog.Text.Length; // Scroll to the end
            txtLog.ScrollToCaret();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Check if a row is selected
            if (worktreeView.CurrentRow != null && worktreeView.CurrentRow.DataBoundItem is WorktreeModel selectedWorktree)
            {
                string worktreeName = selectedWorktree.WorkTreeName;
                AddActionToQueue(ActionType.Delete, EntityType.Worktree, worktreeName);
            }
            else
            {
                MessageBox.Show("Please select a worktree row first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void btnStartQueue_Click_1(object sender, EventArgs e)
        {
            txtLog.Text = string.Empty; // Clear the log text box
            await ProcessActionQueueAsync();
        }

        private void updateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (worktreeView.CurrentRow?.DataBoundItem is WorktreeModel selectedWorktree)
            {
                using var dialog = new DatabaseUpdateDialog(this, selectedWorktree, buildList, settings);
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    if (!FileSystemHelper.DirectoryExists(Path.Combine(dialog.LocalBuildPath, dialog.SelectedBuild)))
                        AddActionToQueue(ActionType.Download, EntityType.Build, selectedWorktree.WorkTreeName, dialog.SelectedBuild);
                    AddActionToQueue(ActionType.Update, EntityType.Database, selectedWorktree.WorkTreeName, dialog.SelectedBuild);
                }
            }
            else
            {
                MessageBox.Show("Please select a worktree row first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void actionQueueView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (actionQueueView.Columns[e.ColumnIndex].Name == "StartActionButton" && e.RowIndex >= 0)
            {
                var action = (ActionModel)actionQueueView.Rows[e.RowIndex].DataBoundItem;
                action.Status = "In Progress";
                actionQueueView.Refresh(); // Refresh the view to show the updated status
                var result = await actionController.ExecuteActionAsync(action, GetLogger);
                action.Status = result ? "Completed" : "Failed";
                actionQueueView.Refresh();
            }
        }
    }
}
