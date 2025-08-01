using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AcuEnvManager
{
    public partial class DatabaseUpdateDialog : Form
    {
        private Form1 _mainForm;
        public WorktreeModel Worktree { get; set; }
        public List<BuildModel> Builds { get; set; }
        public string LocalBuildPath { get; set; }
        public string SelectedBuild { get; set; }


        public DatabaseUpdateDialog(Form1 mainForm, WorktreeModel worktree, List<BuildModel> builds, SettingsModel settings)
        {
            _mainForm = mainForm;
            InitializeComponent();
            Worktree = worktree ?? throw new ArgumentNullException(nameof(worktree));
            txtDBServer.Text = Worktree.DBServer;
            txtDBName.Text =  Worktree.DBName;
            Builds = builds;
            txtLocalBuildPath.Text = LocalBuildPath = settings.LocalBuildPath;

            cmbBuilds.SelectedIndexChanged += cmbBuilds_SelectedIndexChanged;
        }

        private void btnTestConnection_Click(object sender, EventArgs e)
        {
            lblTestConnection.Text = "Testing...";
            var dbHelper = new DatabaseHelper(Worktree.DBServer, Worktree.DBName);
            if (dbHelper.IsValidConnection())
            {
                var currentVersion = dbHelper.GetDatabaseVersion();
                var versionPattern = @"^\d{2}\.\d{3}.*$";
                if (currentVersion == null || !Regex.IsMatch(currentVersion, versionPattern))
                {
                    lblTestConnection.Text = "Invalid version format.";
                    return;
                }
                lblTestConnection.Text = "Connected!";
                var majorVersion = currentVersion.Substring(0, 2);

                cmbBuilds.Items.Clear();
                foreach (var build in Builds)
                {
                    cmbBuilds.Items.AddRange(build.MinorVersions
                        .Where(minor => String.Compare(minor, currentVersion, StringComparison.Ordinal) > 0)
                        .ToArray());
                }
                cmbBuilds.Enabled = true;
            }
            else
            {
                lblTestConnection.Text = "Failed";
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void cmbBuilds_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Handle build selection change here
            SelectedBuild = cmbBuilds.SelectedItem as string ?? string.Empty;
            if (!string.IsNullOrEmpty(SelectedBuild))
            {
                btnOK.Enabled = true;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(SelectedBuild) && !string.IsNullOrEmpty(LocalBuildPath))
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Please select a build and ensure the local build path is set.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
