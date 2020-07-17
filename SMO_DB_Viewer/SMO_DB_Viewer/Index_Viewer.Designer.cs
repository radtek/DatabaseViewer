using ScintillaNet;

namespace SMO_DB_Viewer
{
  partial class Index_Viewer
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    //protected override void Dispose(bool disposing)
    //{
    //  if (disposing && (components != null))
    //  {
    //    components.Dispose();
    //  }
    //  base.Dispose(disposing);
    //}

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
         this.tbDatabase = new System.Windows.Forms.Label();
         this.label2 = new System.Windows.Forms.Label();
         this.label3 = new System.Windows.Forms.Label();
         this.tbUser = new System.Windows.Forms.TextBox();
         this.tbPassword = new System.Windows.Forms.TextBox();
         this.tbTable = new System.Windows.Forms.Label();
         this.tbScripts = new ScintillaNet.Scintilla();
         this.cboTables = new System.Windows.Forms.ComboBox();
         this.cmdScript_Indexes = new System.Windows.Forms.Button();
         this.lblServer = new System.Windows.Forms.Label();
         this.groupBox1 = new System.Windows.Forms.GroupBox();
         this.cmdLogin = new System.Windows.Forms.Button();
         this.cboServer = new System.Windows.Forms.ComboBox();
         this.pnlScriptPanel = new System.Windows.Forms.Panel();
         this.cboDatabases = new System.Windows.Forms.ComboBox();
         this.cmdFKeys = new System.Windows.Forms.Button();
         ((System.ComponentModel.ISupportInitialize)(this.tbScripts)).BeginInit();
         this.groupBox1.SuspendLayout();
         this.pnlScriptPanel.SuspendLayout();
         this.SuspendLayout();
         // 
         // tbDatabase
         // 
         this.tbDatabase.AutoSize = true;
         this.tbDatabase.Location = new System.Drawing.Point(15, 126);
         this.tbDatabase.Name = "tbDatabase";
         this.tbDatabase.Size = new System.Drawing.Size(53, 13);
         this.tbDatabase.TabIndex = 0;
         this.tbDatabase.Text = "Database";
         // 
         // label2
         // 
         this.label2.AutoSize = true;
         this.label2.Location = new System.Drawing.Point(32, 26);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(29, 13);
         this.label2.TabIndex = 1;
         this.label2.Text = "User";
         // 
         // label3
         // 
         this.label3.AutoSize = true;
         this.label3.Location = new System.Drawing.Point(8, 48);
         this.label3.Name = "label3";
         this.label3.Size = new System.Drawing.Size(53, 13);
         this.label3.TabIndex = 2;
         this.label3.Text = "Password";
         // 
         // tbUser
         // 
         this.tbUser.Location = new System.Drawing.Point(67, 19);
         this.tbUser.Name = "tbUser";
         this.tbUser.Size = new System.Drawing.Size(211, 20);
         this.tbUser.TabIndex = 4;
         this.tbUser.Text = "vifezue";
         // 
         // tbPassword
         // 
         this.tbPassword.Location = new System.Drawing.Point(67, 44);
         this.tbPassword.Name = "tbPassword";
         this.tbPassword.PasswordChar = '*';
         this.tbPassword.Size = new System.Drawing.Size(211, 20);
         this.tbPassword.TabIndex = 5;
         this.tbPassword.Text = "";
         // 
         // tbTable
         // 
         this.tbTable.AutoSize = true;
         this.tbTable.Location = new System.Drawing.Point(31, 151);
         this.tbTable.Name = "tbTable";
         this.tbTable.Size = new System.Drawing.Size(34, 13);
         this.tbTable.TabIndex = 6;
         this.tbTable.Text = "Table";
         // 
         // tbScripts
         // 
         this.tbScripts.Dock = System.Windows.Forms.DockStyle.Fill;
         this.tbScripts.Location = new System.Drawing.Point(0, 0);
         this.tbScripts.Name = "tbScripts";
         this.tbScripts.Size = new System.Drawing.Size(487, 405);
         this.tbScripts.TabIndex = 8;
         // 
         // cboTables
         // 
         this.cboTables.FormattingEnabled = true;
         this.cboTables.Location = new System.Drawing.Point(79, 146);
         this.cboTables.Name = "cboTables";
         this.cboTables.Size = new System.Drawing.Size(211, 21);
         this.cboTables.TabIndex = 9;
         this.cboTables.SelectedIndexChanged += new System.EventHandler(this.cboTables_SelectedIndexChanged);
         this.cboTables.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboTables_KeyUp);
         // 
         // cmdScript_Indexes
         // 
         this.cmdScript_Indexes.Location = new System.Drawing.Point(296, 118);
         this.cmdScript_Indexes.Name = "cmdScript_Indexes";
         this.cmdScript_Indexes.Size = new System.Drawing.Size(84, 23);
         this.cmdScript_Indexes.TabIndex = 11;
         this.cmdScript_Indexes.Text = "Script Indexes";
         this.cmdScript_Indexes.UseVisualStyleBackColor = true;
         this.cmdScript_Indexes.Click += new System.EventHandler(this.cmdScript_Indexes_Click);
         // 
         // lblServer
         // 
         this.lblServer.AutoSize = true;
         this.lblServer.Location = new System.Drawing.Point(23, 73);
         this.lblServer.Name = "lblServer";
         this.lblServer.Size = new System.Drawing.Size(38, 13);
         this.lblServer.TabIndex = 12;
         this.lblServer.Text = "Server";
         // 
         // groupBox1
         // 
         this.groupBox1.Controls.Add(this.cmdLogin);
         this.groupBox1.Controls.Add(this.cboServer);
         this.groupBox1.Controls.Add(this.tbUser);
         this.groupBox1.Controls.Add(this.label2);
         this.groupBox1.Controls.Add(this.lblServer);
         this.groupBox1.Controls.Add(this.label3);
         this.groupBox1.Controls.Add(this.tbPassword);
         this.groupBox1.Location = new System.Drawing.Point(12, 12);
         this.groupBox1.Name = "groupBox1";
         this.groupBox1.Size = new System.Drawing.Size(374, 100);
         this.groupBox1.TabIndex = 14;
         this.groupBox1.TabStop = false;
         this.groupBox1.Text = "Credentials";
         // 
         // cmdLogin
         // 
         this.cmdLogin.Location = new System.Drawing.Point(284, 68);
         this.cmdLogin.Name = "cmdLogin";
         this.cmdLogin.Size = new System.Drawing.Size(75, 23);
         this.cmdLogin.TabIndex = 17;
         this.cmdLogin.Text = "Login";
         this.cmdLogin.UseVisualStyleBackColor = true;
         this.cmdLogin.Click += new System.EventHandler(this.cmdLogin_Click);
         // 
         // cboServer
         // 
         this.cboServer.FormattingEnabled = true;
         this.cboServer.Location = new System.Drawing.Point(67, 70);
         this.cboServer.Name = "cboServer";
         this.cboServer.Size = new System.Drawing.Size(211, 21);
         this.cboServer.TabIndex = 13;
         // 
         // pnlScriptPanel
         // 
         this.pnlScriptPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.pnlScriptPanel.Controls.Add(this.tbScripts);
         this.pnlScriptPanel.Location = new System.Drawing.Point(12, 173);
         this.pnlScriptPanel.Name = "pnlScriptPanel";
         this.pnlScriptPanel.Size = new System.Drawing.Size(487, 405);
         this.pnlScriptPanel.TabIndex = 15;
         // 
         // cboDatabases
         // 
         this.cboDatabases.FormattingEnabled = true;
         this.cboDatabases.Location = new System.Drawing.Point(79, 119);
         this.cboDatabases.Name = "cboDatabases";
         this.cboDatabases.Size = new System.Drawing.Size(211, 21);
         this.cboDatabases.TabIndex = 16;
         this.cboDatabases.SelectedIndexChanged += new System.EventHandler(this.cboDatabases_SelectedIndexChanged);
         // 
         // cmdFKeys
         // 
         this.cmdFKeys.Location = new System.Drawing.Point(385, 118);
         this.cmdFKeys.Name = "cmdFKeys";
         this.cmdFKeys.Size = new System.Drawing.Size(75, 23);
         this.cmdFKeys.TabIndex = 17;
         this.cmdFKeys.Text = "Script FKeys";
         this.cmdFKeys.UseVisualStyleBackColor = true;
         this.cmdFKeys.Click += new System.EventHandler(this.cmdFKeys_Click);
         // 
         // Index_Viewer
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(509, 590);
         this.Controls.Add(this.cmdFKeys);
         this.Controls.Add(this.cboDatabases);
         this.Controls.Add(this.pnlScriptPanel);
         this.Controls.Add(this.groupBox1);
         this.Controls.Add(this.cmdScript_Indexes);
         this.Controls.Add(this.cboTables);
         this.Controls.Add(this.tbTable);
         this.Controls.Add(this.tbDatabase);
         this.Name = "Index_Viewer";
         this.Text = "Index Viewer";
         this.Load += new System.EventHandler(this.Index_Viewer_Load);
         ((System.ComponentModel.ISupportInitialize)(this.tbScripts)).EndInit();
         this.groupBox1.ResumeLayout(false);
         this.groupBox1.PerformLayout();
         this.pnlScriptPanel.ResumeLayout(false);
         this.ResumeLayout(false);
         this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label tbDatabase;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox tbUser;
    private System.Windows.Forms.TextBox tbPassword;
    private System.Windows.Forms.Label tbTable;
    private ScintillaNet.Scintilla tbScripts;
    private System.Windows.Forms.ComboBox cboTables;
    private System.Windows.Forms.Button cmdScript_Indexes;
    private System.Windows.Forms.Label lblServer;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Panel pnlScriptPanel;
    private System.Windows.Forms.ComboBox cboDatabases;
    private System.Windows.Forms.ComboBox cboServer;
    private System.Windows.Forms.Button cmdLogin;
      private System.Windows.Forms.Button cmdFKeys;
   }
}