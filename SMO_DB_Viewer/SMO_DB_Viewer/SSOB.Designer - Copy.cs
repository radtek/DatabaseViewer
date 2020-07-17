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
         this.tvSQLServerObjs = new System.Windows.Forms.TreeView();
         this.menuStrip1 = new System.Windows.Forms.MenuStrip();
         this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.tbInstance = new System.Windows.Forms.TextBox();
         this.label1 = new System.Windows.Forms.Label();
         this.label2 = new System.Windows.Forms.Label();
         this.tbLogin = new System.Windows.Forms.TextBox();
         this.label3 = new System.Windows.Forms.Label();
         this.tbPassword = new System.Windows.Forms.TextBox();
         this.tbScripts = new ScintillaNet.Scintilla();
         this.cmdSSInstanceLogin = new System.Windows.Forms.Button();
         this.label4 = new System.Windows.Forms.Label();
         this.cboDatabases = new System.Windows.Forms.ComboBox();
         this.tbFindNode = new System.Windows.Forms.TextBox();
         this.cmdFindNode = new System.Windows.Forms.Button();
         this.label5 = new System.Windows.Forms.Label();
         this.menuStrip1.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.tbScripts)).BeginInit();
         this.SuspendLayout();
         // 
         // tvSQLServerObjs
         // 
         this.tvSQLServerObjs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
         this.tvSQLServerObjs.Location = new System.Drawing.Point(12, 86);
         this.tvSQLServerObjs.Name = "tvSQLServerObjs";
         this.tvSQLServerObjs.Size = new System.Drawing.Size(277, 579);
         this.tvSQLServerObjs.TabIndex = 0;
         this.tvSQLServerObjs.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.tvSQLServerObjs_AfterExpand);
         this.tvSQLServerObjs.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvSQLServerObjs_AfterSelect);
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
         // tbInstance
         // 
         this.tbInstance.Location = new System.Drawing.Point(114, 32);
         this.tbInstance.Name = "tbInstance";
         this.tbInstance.Size = new System.Drawing.Size(140, 20);
         this.tbInstance.TabIndex = 3;
         this.tbInstance.Text = "HTI-VISUAL";
         // 
         // label1
         // 
         this.label1.AutoSize = true;
         this.label1.Location = new System.Drawing.Point(25, 36);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(85, 13);
         this.label1.TabIndex = 4;
         this.label1.Text = "Server Instance:";
         // 
         // label2
         // 
         this.label2.AutoSize = true;
         this.label2.Location = new System.Drawing.Point(267, 36);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(36, 13);
         this.label2.TabIndex = 5;
         this.label2.Text = "Login:";
         // 
         // tbLogin
         // 
         this.tbLogin.Location = new System.Drawing.Point(310, 32);
         this.tbLogin.Name = "tbLogin";
         this.tbLogin.Size = new System.Drawing.Size(136, 20);
         this.tbLogin.TabIndex = 6;
         this.tbLogin.Text = "LONDECKD";
         // 
         // label3
         // 
         this.label3.AutoSize = true;
         this.label3.Location = new System.Drawing.Point(461, 36);
         this.label3.Name = "label3";
         this.label3.Size = new System.Drawing.Size(56, 13);
         this.label3.TabIndex = 7;
         this.label3.Text = "Password:";
         // 
         // tbPassword
         // 
         this.tbPassword.Location = new System.Drawing.Point(525, 32);
         this.tbPassword.Name = "tbPassword";
         this.tbPassword.PasswordChar = '*';
         this.tbPassword.Size = new System.Drawing.Size(130, 20);
         this.tbPassword.TabIndex = 8;
         this.tbPassword.Text = "CueTec@21";
         // 
         // tbScripts
         // 
         this.tbScripts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.tbScripts.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.tbScripts.Location = new System.Drawing.Point(295, 86);
         this.tbScripts.Name = "tbScripts";
         this.tbScripts.Size = new System.Drawing.Size(788, 579);
         this.tbScripts.Styles.BraceBad.FontName = "Verdana";
         this.tbScripts.Styles.BraceLight.FontName = "Verdana";
         this.tbScripts.Styles.ControlChar.FontName = "Verdana";
         this.tbScripts.Styles.Default.FontName = "Verdana";
         this.tbScripts.Styles.IndentGuide.FontName = "Verdana";
         this.tbScripts.Styles.LastPredefined.FontName = "Verdana";
         this.tbScripts.Styles.LineNumber.FontName = "Verdana";
         this.tbScripts.Styles.Max.FontName = "Verdana";
         this.tbScripts.TabIndex = 9;
         // 
         // cmdSSInstanceLogin
         // 
         this.cmdSSInstanceLogin.Location = new System.Drawing.Point(661, 31);
         this.cmdSSInstanceLogin.Name = "cmdSSInstanceLogin";
         this.cmdSSInstanceLogin.Size = new System.Drawing.Size(75, 23);
         this.cmdSSInstanceLogin.TabIndex = 10;
         this.cmdSSInstanceLogin.Text = "Login";
         this.cmdSSInstanceLogin.UseVisualStyleBackColor = true;
         this.cmdSSInstanceLogin.Click += new System.EventHandler(this.cmdSSInstanceLogin_Click);
         // 
         // label4
         // 
         this.label4.AutoSize = true;
         this.label4.Location = new System.Drawing.Point(12, 62);
         this.label4.Name = "label4";
         this.label4.Size = new System.Drawing.Size(95, 13);
         this.label4.TabIndex = 11;
         this.label4.Text = "Choose Database:";
         // 
         // cboDatabases
         // 
         this.cboDatabases.FormattingEnabled = true;
         this.cboDatabases.Location = new System.Drawing.Point(114, 59);
         this.cboDatabases.Name = "cboDatabases";
         this.cboDatabases.Size = new System.Drawing.Size(140, 21);
         this.cboDatabases.TabIndex = 12;
         this.cboDatabases.SelectedIndexChanged += new System.EventHandler(this.cboDatabases_SelectedIndexChanged);
         // 
         // tbFindNode
         // 
         this.tbFindNode.Location = new System.Drawing.Point(525, 58);
         this.tbFindNode.Name = "tbFindNode";
         this.tbFindNode.Size = new System.Drawing.Size(130, 20);
         this.tbFindNode.TabIndex = 13;
         // 
         // cmdFindNode
         // 
         this.cmdFindNode.Location = new System.Drawing.Point(661, 57);
         this.cmdFindNode.Name = "cmdFindNode";
         this.cmdFindNode.Size = new System.Drawing.Size(75, 23);
         this.cmdFindNode.TabIndex = 14;
         this.cmdFindNode.Text = "Find";
         this.cmdFindNode.UseVisualStyleBackColor = true;
         this.cmdFindNode.Click += new System.EventHandler(this.cmdFindNode_Click);
         // 
         // label5
         // 
         this.label5.AutoSize = true;
         this.label5.Location = new System.Drawing.Point(459, 62);
         this.label5.Name = "label5";
         this.label5.Size = new System.Drawing.Size(59, 13);
         this.label5.TabIndex = 15;
         this.label5.Text = "Find Node:";
         // 
         // SSOB
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(1102, 677);
         this.Controls.Add(this.label5);
         this.Controls.Add(this.cmdFindNode);
         this.Controls.Add(this.tbFindNode);
         this.Controls.Add(this.cboDatabases);
         this.Controls.Add(this.label4);
         this.Controls.Add(this.cmdSSInstanceLogin);
         this.Controls.Add(this.tbScripts);
         this.Controls.Add(this.tbPassword);
         this.Controls.Add(this.label3);
         this.Controls.Add(this.tbLogin);
         this.Controls.Add(this.label2);
         this.Controls.Add(this.label1);
         this.Controls.Add(this.tbInstance);
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
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.TreeView tvSQLServerObjs;
      private System.Windows.Forms.MenuStrip menuStrip1;
      private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
      private System.Windows.Forms.TextBox tbInstance;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.TextBox tbLogin;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.TextBox tbPassword;
      private ScintillaNet.Scintilla tbScripts;
      private System.Windows.Forms.Button cmdSSInstanceLogin;
      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.ComboBox cboDatabases;
      private System.Windows.Forms.TextBox tbFindNode;
      private System.Windows.Forms.Button cmdFindNode;
      private System.Windows.Forms.Label label5;
   }
}

