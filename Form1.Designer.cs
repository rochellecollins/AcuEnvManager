namespace AcuEnvManager
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            worktreeView = new DataGridView();
            statusStrip1 = new StatusStrip();
            statusLabel = new ToolStripStatusLabel();
            btnDeleteWT = new Button();
            actionQueueView = new DataGridView();
            btnStartQueue = new Button();
            txtLog = new TextBox();
            ((System.ComponentModel.ISupportInitialize)worktreeView).BeginInit();
            statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)actionQueueView).BeginInit();
            SuspendLayout();
            // 
            // worktreeView
            // 
            worktreeView.AllowUserToAddRows = false;
            worktreeView.AllowUserToDeleteRows = false;
            worktreeView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            worktreeView.Location = new Point(12, 11);
            worktreeView.Margin = new Padding(3, 2, 3, 2);
            worktreeView.Name = "worktreeView";
            worktreeView.RowHeadersWidth = 51;
            worktreeView.Size = new Size(1606, 428);
            worktreeView.TabIndex = 0;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { statusLabel });
            statusStrip1.Location = new Point(0, 709);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1630, 22);
            statusStrip1.TabIndex = 2;
            statusStrip1.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(39, 17);
            statusLabel.Text = "Ready";
            // 
            // btnDeleteWT
            // 
            btnDeleteWT.Location = new Point(12, 663);
            btnDeleteWT.Name = "btnDeleteWT";
            btnDeleteWT.Size = new Size(116, 23);
            btnDeleteWT.TabIndex = 3;
            btnDeleteWT.Text = "Delete Worktree";
            btnDeleteWT.UseVisualStyleBackColor = true;
            btnDeleteWT.Click += btnDeleteWT_Click;
            // 
            // actionQueueView
            // 
            actionQueueView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            actionQueueView.Location = new Point(265, 458);
            actionQueueView.Name = "actionQueueView";
            actionQueueView.Size = new Size(691, 228);
            actionQueueView.TabIndex = 4;
            // 
            // btnStartQueue
            // 
            btnStartQueue.Location = new Point(145, 459);
            btnStartQueue.Name = "btnStartQueue";
            btnStartQueue.Size = new Size(114, 23);
            btnStartQueue.TabIndex = 5;
            btnStartQueue.Text = "Start Queue";
            btnStartQueue.UseVisualStyleBackColor = true;
            btnStartQueue.Click += btnStartQueue_Click;
            // 
            // txtLog
            // 
            txtLog.Location = new Point(1002, 459);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.Size = new Size(612, 227);
            txtLog.TabIndex = 6;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1630, 731);
            Controls.Add(txtLog);
            Controls.Add(btnStartQueue);
            Controls.Add(actionQueueView);
            Controls.Add(btnDeleteWT);
            Controls.Add(statusStrip1);
            Controls.Add(worktreeView);
            Margin = new Padding(3, 2, 3, 2);
            Name = "Form1";
            Text = "Acumatica Envinronment Manager";
            ((System.ComponentModel.ISupportInitialize)worktreeView).EndInit();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)actionQueueView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView worktreeView;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel statusLabel;
        private Button btnDeleteWT;
        private DataGridView actionQueueView;
        private Button btnStartQueue;
        private TextBox txtLog;
    }
}
