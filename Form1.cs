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
        private List<ActionModel> tempQueue = new();


        private object? _oldValue;
        private int _oldRowIndex = -1;
        public const string repoPath = @"C:\Users\rochelle.collins\source\repos";

        public Form1()
        {
            InitializeComponent();
            wtcontroller = new WorktreeController(repoPath);
            worktreeView.DataSource = worktreeList;

            actionController = new ActionController(repoPath);
            actionQueueView.DataSource = actionQueueList;
            tempQueue = new List<ActionModel>();
            // Do NOT start async loading here!
            this.Load += Form1_Load;
        }

        private async void Form1_Load(object? sender, EventArgs e)
        {
            InitializeWorktreeView();
            InitializeActionQueueView();
            LoadWorkTrees();
            await UpdateWorktreeDetailsAsync();
            EnableFields();

        }

        private void EnableFields()
        {
            worktreeView.Columns["BranchName"].ReadOnly = false;
            worktreeView.Columns["DBServer"].ReadOnly = false;
            worktreeView.Columns["DBName"].ReadOnly = false;
        }

        private void InitializeActionQueueView()
        {
            // Hide the ActionName column
            actionQueueView.Columns["ActionName"].Visible = false;
            actionQueueView.Columns["Entity"].Visible = false;

            // Optionally, set the header text for the description column
            actionQueueView.Columns["ActionDescription"].HeaderText = "Action";
            actionQueueView.Columns["ActionDescription"].DisplayIndex = 0;
            actionQueueView.Columns["EntityDescription"].HeaderText = "Entity";
            actionQueueView.Columns["EntityDescription"].DisplayIndex = 1;

            actionQueueView.Columns["Value"].HeaderText = "Value";
            actionQueueView.Columns["Value"].Width = 200;
        }

        private void LoadWorkTrees()
        {
            try
            {
                this.Invoke(() => statusLabel.Text = "Loading worktree names...");

                // 1. Load and display all worktree names first
                // Simplified: Add all worktrees to the list in a single Invoke call
                worktreeView.Invoke(() =>
                {
                    foreach (var worktree in wtcontroller.GetWorktrees())
                    {
                        worktreeList.Add(worktree);
                    }
                });

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

                foreach (var worktree in worktreeList)
                {
                    await Task.Run(() => wtcontroller.UpdateWorktreeDetails(worktree));
                    worktreeView.Invoke(() =>
                    {
                        var currencyManager = (CurrencyManager)BindingContext[worktreeList];
                        currencyManager.Refresh();
                    });
                }
                this.Invoke(() => statusLabel.Text = $"Loaded {worktreeList.Count} worktrees.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Error updating worktree details:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Invoke(() => statusLabel.Text = "Error updating worktree details.");
            }
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
            worktreeView.Columns["WorkTreeName"].ReadOnly = true;
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
                HeaderText = "JIRA Ticket",
                DataPropertyName = "JIRALink",
                UseColumnTextForLinkValue = false,
                LinkBehavior = LinkBehavior.HoverUnderline,
                TrackVisitedState = true,
            };
            worktreeView.Columns.Add(linkCol);
            worktreeView.Columns["JIRALink"].Width = 200;

            worktreeView.CellBeginEdit += worktreeView_CellBeginEdit;
            worktreeView.CellValueChanged += worktreeView_CellValueChanged;
            worktreeView.CellEndEdit += worktreeView_CellEndEdit;
            worktreeView.CellFormatting += worktreeView_CellFormatting;

            worktreeView.ReadOnly = true;
        }

        private void worktreeView_CellBeginEdit(object? sender, DataGridViewCellCancelEventArgs e)
        {
            _oldValue = worktreeView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            _oldRowIndex = e.RowIndex;
        }

        private void worktreeView_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
        {
            var row = worktreeView.Rows[e.RowIndex];
            if(row == null) return;

            DialogResult result;
            switch (worktreeView.Columns[e.ColumnIndex].Name)
            {
                case "BranchName":

                    var newValue = row.Cells[e.ColumnIndex].Value?.ToString() ?? "";

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
                    tempQueue.Add(new ActionModel
                    {
                        ActionName = ActionType.Checkout,
                        Entity = EntityType.Worktree,
                        EntityName = row.Cells["WorkTreeName"].Value?.ToString() ?? "",
                        Value = newValue,
                        Status = "Ready"
                    });
                    if (result == DialogResult.Yes)
                    {
                        tempQueue.Add(new ActionModel
                        {
                            ActionName = ActionType.Acubuild,
                            Entity = EntityType.Worktree,
                            EntityName = row.Cells["WorkTreeName"].Value?.ToString() ?? "",
                            Status = "Ready"
                        });
                    }
                    break;
                case "DBServer":
                case "DBName":
                    var newServer = row.Cells["DBServer"].Value?.ToString() ?? "";
                    var newDatabase = row.Cells["DBName"].Value?.ToString() ?? "";
                    var dbHelper = new DatabaseHelper(newServer, newDatabase);

                    if (!dbHelper.IsValidConnection())
                    {                       
                        result = MessageBox.Show(
                            $"Connection failed to {newServer}, {newDatabase}. Do you wish to update anyway?",
                             "Connection Failed",
                             MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning);

                        if (result == DialogResult.No && e.RowIndex == _oldRowIndex)
                        {
                            row.Cells[e.ColumnIndex].Value = _oldValue;
                            return;
                        }
                    }
                    else
                        row.Cells["DBVers"].Value = dbHelper.GetDatabaseVersion();

                    tempQueue.Add(new ActionModel
                    {
                        ActionName = ActionType.Update,
                        Entity = EntityType.WebConfig,
                        EntityName = row.Cells["WorkTreeName"].Value?.ToString() ?? "",
                        Value = $"{newServer};{newDatabase}",
                        Status = "Ready"
                    });
                    break;
            }
        }

        private void worktreeView_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
        {
            foreach (var action in tempQueue)
            {
                actionQueueList.Add(action);
            }
            tempQueue.Clear();
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

        private void btnDeleteWT_Click(object sender, EventArgs e)
        {
            // Check if a row is selected
            if (worktreeView.CurrentRow != null && worktreeView.CurrentRow.DataBoundItem is WorktreeModel selectedWorktree)
            {
                string worktreeName = selectedWorktree.WorkTreeName;
                actionQueueList.Add(new ActionModel { ActionName = ActionType.Delete, Entity = EntityType.Worktree, EntityName = worktreeName, Status = "Ready" });
            }
            else
            {
                MessageBox.Show("Please select a worktree row first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void btnStartQueue_Click(object sender, EventArgs e)
        {
            txtLog.Text = string.Empty; // Clear the log text box
            await ProcessActionQueueAsync();
        }

        private async Task ProcessActionQueueAsync()
        {
            foreach (var action in actionQueueList)
            {
                action.Status = "In Progress";
                actionQueueView.Refresh();
                var progress = new Progress<string>(status =>
                {
                    txtLog.Text += status + Environment.NewLine;
                    txtLog.SelectionStart = txtLog.Text.Length; // Scroll to the end
                    txtLog.ScrollToCaret();
                });

                bool result = await ExecuteActionAsync(action, progress);

                action.Status = result ? "Completed" : "Failed";
                actionQueueView.Refresh();
            }
        }

        private async Task<bool> ExecuteActionAsync(ActionModel action, IProgress<string> progress)
        {
            return await actionController.ExecuteActionAsync(action, progress);
        }
    }
}
