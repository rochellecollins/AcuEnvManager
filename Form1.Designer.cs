
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            worktreeView = new DataGridView();
            statusStrip1 = new StatusStrip();
            statusLabel = new ToolStripStatusLabel();
            actionQueueView = new DataGridView();
            txtLog = new TextBox();
            toolStrip1 = new ToolStrip();
            tsWorkTreeActions = new ToolStripDropDownButton();
            deleteToolStripMenuItem = new ToolStripMenuItem();
            tsDBActions = new ToolStripDropDownButton();
            updateToolStripMenuItem = new ToolStripMenuItem();
            panel1 = new Panel();
            toolStrip2 = new ToolStrip();
            btnStartQueue = new ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)worktreeView).BeginInit();
            statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)actionQueueView).BeginInit();
            toolStrip1.SuspendLayout();
            panel1.SuspendLayout();
            toolStrip2.SuspendLayout();
            SuspendLayout();
            // 
            // worktreeView
            // 
            worktreeView.AllowUserToAddRows = false;
            worktreeView.AllowUserToDeleteRows = false;
            worktreeView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            worktreeView.Location = new Point(0, 27);
            worktreeView.Margin = new Padding(3, 2, 3, 2);
            worktreeView.Name = "worktreeView";
            worktreeView.RowHeadersWidth = 51;
            worktreeView.Size = new Size(1630, 412);
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
            // actionQueueView
            // 
            actionQueueView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            actionQueueView.Location = new Point(0, 28);
            actionQueueView.Name = "actionQueueView";
            actionQueueView.Size = new Size(1009, 231);
            actionQueueView.TabIndex = 4;
            // 
            // txtLog
            // 
            txtLog.Location = new Point(1018, 450);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.Size = new Size(612, 256);
            txtLog.TabIndex = 6;
            // 
            // toolStrip1
            // 
            toolStrip1.Items.AddRange(new ToolStripItem[] { tsWorkTreeActions, tsDBActions });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(1630, 25);
            toolStrip1.TabIndex = 7;
            toolStrip1.Text = "toolStrip1";
            // 
            // tsWorkTreeActions
            // 
            tsWorkTreeActions.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tsWorkTreeActions.DropDownItems.AddRange(new ToolStripItem[] { deleteToolStripMenuItem });
            tsWorkTreeActions.Image = (Image)resources.GetObject("tsWorkTreeActions.Image");
            tsWorkTreeActions.ImageTransparentColor = Color.Magenta;
            tsWorkTreeActions.Name = "tsWorkTreeActions";
            tsWorkTreeActions.Size = new Size(111, 22);
            tsWorkTreeActions.Text = "Worktree Actions";
            tsWorkTreeActions.TextAlign = ContentAlignment.BottomLeft;
            tsWorkTreeActions.TextImageRelation = TextImageRelation.TextBeforeImage;
            // 
            // deleteToolStripMenuItem
            // 
            deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            deleteToolStripMenuItem.Size = new Size(180, 22);
            deleteToolStripMenuItem.Text = "Delete";
            deleteToolStripMenuItem.Click += deleteToolStripMenuItem_Click;
            // 
            // tsDBActions
            // 
            tsDBActions.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tsDBActions.DropDownItems.AddRange(new ToolStripItem[] { updateToolStripMenuItem });
            tsDBActions.Image = (Image)resources.GetObject("tsDBActions.Image");
            tsDBActions.ImageTransparentColor = Color.Magenta;
            tsDBActions.Name = "tsDBActions";
            tsDBActions.Size = new Size(111, 22);
            tsDBActions.Text = "Database Actions";
            // 
            // updateToolStripMenuItem
            // 
            updateToolStripMenuItem.Name = "updateToolStripMenuItem";
            updateToolStripMenuItem.Size = new Size(180, 22);
            updateToolStripMenuItem.Text = "Update";
            updateToolStripMenuItem.Click += updateToolStripMenuItem_Click;
            // 
            // panel1
            // 
            panel1.Controls.Add(toolStrip2);
            panel1.Controls.Add(actionQueueView);
            panel1.Location = new Point(0, 444);
            panel1.Name = "panel1";
            panel1.Size = new Size(1012, 262);
            panel1.TabIndex = 8;
            // 
            // toolStrip2
            // 
            toolStrip2.Items.AddRange(new ToolStripItem[] { btnStartQueue });
            toolStrip2.Location = new Point(0, 0);
            toolStrip2.Name = "toolStrip2";
            toolStrip2.Size = new Size(1012, 25);
            toolStrip2.TabIndex = 7;
            toolStrip2.Text = "toolStrip2";
            // 
            // btnStartQueue
            // 
            btnStartQueue.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnStartQueue.Image = (Image)resources.GetObject("btnStartQueue.Image");
            btnStartQueue.ImageTransparentColor = Color.Magenta;
            btnStartQueue.Name = "btnStartQueue";
            btnStartQueue.Size = new Size(73, 22);
            btnStartQueue.Text = "Start Queue";
            btnStartQueue.Click += btnStartQueue_Click_1;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1630, 731);
            Controls.Add(panel1);
            Controls.Add(toolStrip1);
            Controls.Add(txtLog);
            Controls.Add(statusStrip1);
            Controls.Add(worktreeView);
            Margin = new Padding(3, 2, 3, 2);
            Name = "Form1";
            Text = "Acumatica Envinronment Manager";
            ((System.ComponentModel.ISupportInitialize)worktreeView).EndInit();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)actionQueueView).EndInit();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            toolStrip2.ResumeLayout(false);
            toolStrip2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }



        #endregion

        private DataGridView worktreeView;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel statusLabel;
        private DataGridView actionQueueView;
        private TextBox txtLog;
        private ToolStrip toolStrip1;
        private ToolStripDropDownButton tsWorkTreeActions;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private Panel panel1;
        private ToolStrip toolStrip2;
        private ToolStripButton btnStartQueue;
        private ToolStripDropDownButton tsDBActions;
        private ToolStripMenuItem updateToolStripMenuItem;
    }
}
