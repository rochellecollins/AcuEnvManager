namespace AcuEnvManager
{
    partial class DatabaseUpdateDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panel1 = new Panel();
            lblTestConnection = new Label();
            btnCancel = new Button();
            btnOK = new Button();
            txtLocalBuildPath = new TextBox();
            btnTestConnection = new Button();
            cmbBuilds = new ComboBox();
            txtDBName = new TextBox();
            txtDBServer = new TextBox();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(lblTestConnection);
            panel1.Controls.Add(btnCancel);
            panel1.Controls.Add(btnOK);
            panel1.Controls.Add(txtLocalBuildPath);
            panel1.Controls.Add(btnTestConnection);
            panel1.Controls.Add(cmbBuilds);
            panel1.Controls.Add(txtDBName);
            panel1.Controls.Add(txtDBServer);
            panel1.Location = new Point(19, 20);
            panel1.Name = "panel1";
            panel1.Size = new Size(677, 279);
            panel1.TabIndex = 0;
            // 
            // lblTestConnection
            // 
            lblTestConnection.AutoSize = true;
            lblTestConnection.Location = new Point(515, 66);
            lblTestConnection.Name = "lblTestConnection";
            lblTestConnection.Size = new Size(0, 15);
            lblTestConnection.TabIndex = 7;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(401, 217);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 23);
            btnCancel.TabIndex = 6;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // btnOK
            // 
            btnOK.Enabled = false;
            btnOK.Location = new Point(295, 217);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(75, 23);
            btnOK.TabIndex = 5;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // txtLocalBuildPath
            // 
            txtLocalBuildPath.Location = new Point(118, 152);
            txtLocalBuildPath.Name = "txtLocalBuildPath";
            txtLocalBuildPath.ReadOnly = true;
            txtLocalBuildPath.Size = new Size(252, 23);
            txtLocalBuildPath.TabIndex = 4;
            // 
            // btnTestConnection
            // 
            btnTestConnection.Location = new Point(401, 63);
            btnTestConnection.Name = "btnTestConnection";
            btnTestConnection.Size = new Size(75, 23);
            btnTestConnection.TabIndex = 3;
            btnTestConnection.Text = "Connect";
            btnTestConnection.UseVisualStyleBackColor = true;
            btnTestConnection.Click += btnTestConnection_Click;
            // 
            // cmbBuilds
            // 
            cmbBuilds.Enabled = false;
            cmbBuilds.FormattingEnabled = true;
            cmbBuilds.Location = new Point(117, 104);
            cmbBuilds.Name = "cmbBuilds";
            cmbBuilds.Size = new Size(253, 23);
            cmbBuilds.TabIndex = 2;
            // 
            // txtDBName
            // 
            txtDBName.Location = new Point(115, 63);
            txtDBName.Name = "txtDBName";
            txtDBName.Size = new Size(255, 23);
            txtDBName.TabIndex = 1;
            // 
            // txtDBServer
            // 
            txtDBServer.Location = new Point(114, 26);
            txtDBServer.Name = "txtDBServer";
            txtDBServer.Size = new Size(256, 23);
            txtDBServer.TabIndex = 0;
            // 
            // DatabaseUpdateDialog
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(706, 312);
            Controls.Add(panel1);
            Name = "DatabaseUpdateDialog";
            Text = "Update Database";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private ComboBox cmbBuilds;
        private TextBox txtDBName;
        private TextBox txtDBServer;
        private Button btnCancel;
        private Button btnOK;
        private TextBox txtLocalBuildPath;
        private Button btnTestConnection;
        private Label lblTestConnection;
    }
}