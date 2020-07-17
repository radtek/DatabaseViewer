namespace SMO_DB_Viewer
{
   partial class SSOB
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
         this.components = new System.ComponentModel.Container();
         this.tvSQLServerObjs = new System.Windows.Forms.TreeView();
         this.menuStrip1 = new System.Windows.Forms.MenuStrip();
         this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.tbScripts = new ScintillaNet.Scintilla();
         this.ColumnSort = new System.Windows.Forms.ContextMenuStrip(this.components);
         this.sortResultsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.naturalSortToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.tbFindNode = new System.Windows.Forms.TextBox();
         this.cmdFindNode = new System.Windows.Forms.Button();
         this.label5 = new System.Windows.Forms.Label();
         this.cmdClipBoard = new System.Windows.Forms.Button();
         this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
         this.menuStrip1.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.tbScripts)).BeginInit();
         this.ColumnSort.SuspendLayout();
         this.SuspendLayout();
         // 
         // tvSQLServerObjs
         // 
         this.tvSQLServerObjs.AllowDrop = true;
         this.tvSQLServerObjs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
         this.tvSQLServerObjs.Location = new System.Drawing.Point(12, 63);
         this.tvSQLServerObjs.Name = "tvSQLServerObjs";
         this.tvSQLServerObjs.Size = new System.Drawing.Size(275, 600);
         this.tvSQLServerObjs.TabIndex = 3;
         this.tvSQLServerObjs.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.tvSQLServerObjs_AfterExpand);
         this.tvSQLServerObjs.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.tvSQLServerObjs_ItemDrag);
         this.tvSQLServerObjs.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvSQLServerObjs_AfterSelect);
         this.tvSQLServerObjs.MouseLeave += new System.EventHandler(this.tvSQLServerObjs_MouseLeave);
         // 
         // menuStrip1
         // 
         this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
         this.menuStrip1.Location = new System.Drawing.Point(0, 0);
         this.menuStrip1.Name = "menuStrip1";
         this.menuStrip1.Size = new System.Drawing.Size(1102, 24);
         this.menuStrip1.TabIndex = 2;
         this.menuStrip1.Text = "menuStrip1";
         // 
         // fileToolStripMenuItem
         // 
         this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeToolStripMenuItem});
         this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
         this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
         this.fileToolStripMenuItem.Text = "&File";
         // 
         // closeToolStripMenuItem
         // 
         this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
         this.closeToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
         this.closeToolStripMenuItem.Text = "C&lose";
         this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
         // 
         // tbScripts
         // 
         this.tbScripts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.tbScripts.ContextMenuStrip = this.ColumnSort;
         this.tbScripts.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.tbScripts.Location = new System.Drawing.Point(307, 63);
         this.tbScripts.Margin = new System.Windows.Forms.Padding(33, 3, 3, 3);
         this.tbScripts.Name = "tbScripts";
         this.tbScripts.Size = new System.Drawing.Size(780, 600);
         this.tbScripts.Styles.BraceBad.FontName = "Verdana";
         this.tbScripts.Styles.BraceLight.FontName = "Verdana";
         this.tbScripts.Styles.ControlChar.FontName = "Verdana";
         this.tbScripts.Styles.Default.FontName = "Verdana";
         this.tbScripts.Styles.IndentGuide.FontName = "Verdana";
         this.tbScripts.Styles.LastPredefined.FontName = "Verdana";
         this.tbScripts.Styles.LineNumber.FontName = "Verdana";
         this.tbScripts.Styles.Max.FontName = "Verdana";
         this.tbScripts.TabIndex = 4;
         // 
         // ColumnSort
         // 
         this.ColumnSort.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.ColumnSort.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sortResultsToolStripMenuItem,
            this.naturalSortToolStripMenuItem});
         this.ColumnSort.Name = "ColumnSort";
         this.ColumnSort.Size = new System.Drawing.Size(173, 48);
         this.ColumnSort.Text = "Sort Columns";
         // 
         // sortResultsToolStripMenuItem
         // 
         this.sortResultsToolStripMenuItem.Name = "sortResultsToolStripMenuItem";
         this.sortResultsToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
         this.sortResultsToolStripMenuItem.Text = "Sort Results";
         this.sortResultsToolStripMenuItem.Click += new System.EventHandler(this.sortResultsToolStripMenuItem_Click);
         // 
         // naturalSortToolStripMenuItem
         // 
         this.naturalSortToolStripMenuItem.Name = "naturalSortToolStripMenuItem";
         this.naturalSortToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
         this.naturalSortToolStripMenuItem.Text = "Natural Sort";
         this.naturalSortToolStripMenuItem.Click += new System.EventHandler(this.naturalSortToolStripMenuItem_Click);
         // 
         // tbFindNode
         // 
         this.tbFindNode.Location = new System.Drawing.Point(78, 37);
         this.tbFindNode.Name = "tbFindNode";
         this.tbFindNode.Size = new System.Drawing.Size(130, 20);
         this.tbFindNode.TabIndex = 1;
         // 
         // cmdFindNode
         // 
         this.cmdFindNode.Location = new System.Drawing.Point(214, 36);
         this.cmdFindNode.Name = "cmdFindNode";
         this.cmdFindNode.Size = new System.Drawing.Size(75, 23);
         this.cmdFindNode.TabIndex = 2;
         this.cmdFindNode.Text = "Find";
         this.cmdFindNode.UseVisualStyleBackColor = true;
         this.cmdFindNode.Click += new System.EventHandler(this.cmdFindNode_Click);
         // 
         // label5
         // 
         this.label5.AutoSize = true;
         this.label5.Location = new System.Drawing.Point(12, 41);
         this.label5.Name = "label5";
         this.label5.Size = new System.Drawing.Size(59, 13);
         this.label5.TabIndex = 0;
         this.label5.Text = "Find Node:";
         // 
         // cmdClipBoard
         // 
         this.cmdClipBoard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.cmdClipBoard.Location = new System.Drawing.Point(1014, 31);
         this.cmdClipBoard.Name = "cmdClipBoard";
         this.cmdClipBoard.Size = new System.Drawing.Size(75, 23);
         this.cmdClipBoard.TabIndex = 6;
         this.cmdClipBoard.Text = "ClipBoard";
         this.cmdClipBoard.UseVisualStyleBackColor = true;
         this.cmdClipBoard.Click += new System.EventHandler(this.cmdClipBoard_Click);
         // 
         // SSOB
         // 
         this.AcceptButton = this.cmdFindNode;
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(1102, 682);
         this.Controls.Add(this.cmdClipBoard);
         this.Controls.Add(this.label5);
         this.Controls.Add(this.cmdFindNode);
         this.Controls.Add(this.tbFindNode);
         this.Controls.Add(this.tbScripts);
         this.Controls.Add(this.tvSQLServerObjs);
         this.Controls.Add(this.menuStrip1);
         this.MainMenuStrip = this.menuStrip1;
         this.Name = "SSOB";
         this.Text = "SQL Server Object Browser";
         this.Load += new System.EventHandler(this.SSOB_Load);
         this.Shown += new System.EventHandler(this.SSOB_Shown);
         this.menuStrip1.ResumeLayout(false);
         this.menuStrip1.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this.tbScripts)).EndInit();
         this.ColumnSort.ResumeLayout(false);
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.TreeView tvSQLServerObjs;
      private System.Windows.Forms.MenuStrip menuStrip1;
      private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
      private ScintillaNet.Scintilla tbScripts;
      private System.Windows.Forms.TextBox tbFindNode;
      private System.Windows.Forms.Button cmdFindNode;
      private System.Windows.Forms.Label label5;
      private System.Windows.Forms.ContextMenuStrip ColumnSort;
      private System.Windows.Forms.ToolStripMenuItem sortResultsToolStripMenuItem;
      private System.Windows.Forms.Button cmdClipBoard;
      private System.Windows.Forms.ToolStripMenuItem naturalSortToolStripMenuItem;
      private System.Windows.Forms.ToolTip toolTip1;
   }
}

