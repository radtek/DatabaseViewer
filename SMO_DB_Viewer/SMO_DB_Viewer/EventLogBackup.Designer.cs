namespace SMO_DB_Viewer
{
   partial class EventLogBackup
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
         this.cmdGO = new System.Windows.Forms.Button();
         this.label1 = new System.Windows.Forms.Label();
         this.lblPassword = new System.Windows.Forms.Label();
         this.label3 = new System.Windows.Forms.Label();
         this.tbUserName = new System.Windows.Forms.TextBox();
         this.tbPassword = new System.Windows.Forms.TextBox();
         this.tbComputerName = new System.Windows.Forms.TextBox();
         this.menuStrip1 = new System.Windows.Forms.MenuStrip();
         this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.menuStrip1.SuspendLayout();
         this.SuspendLayout();
         // 
         // cmdGO
         // 
         this.cmdGO.Location = new System.Drawing.Point(290, 29);
         this.cmdGO.Name = "cmdGO";
         this.cmdGO.Size = new System.Drawing.Size(75, 23);
         this.cmdGO.TabIndex = 0;
         this.cmdGO.Text = "GO!";
         this.cmdGO.UseVisualStyleBackColor = true;
         this.cmdGO.Click += new System.EventHandler(this.cmdGO_Click);
         // 
         // label1
         // 
         this.label1.AutoSize = true;
         this.label1.Location = new System.Drawing.Point(12, 32);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(57, 13);
         this.label1.TabIndex = 1;
         this.label1.Text = "UserName";
         // 
         // lblPassword
         // 
         this.lblPassword.AutoSize = true;
         this.lblPassword.Location = new System.Drawing.Point(13, 56);
         this.lblPassword.Name = "lblPassword";
         this.lblPassword.Size = new System.Drawing.Size(56, 13);
         this.lblPassword.TabIndex = 2;
         this.lblPassword.Text = "Password:";
         // 
         // label3
         // 
         this.label3.AutoSize = true;
         this.label3.Location = new System.Drawing.Point(13, 79);
         this.label3.Name = "label3";
         this.label3.Size = new System.Drawing.Size(86, 13);
         this.label3.TabIndex = 3;
         this.label3.Text = "Computer Name:";
         // 
         // tbUserName
         // 
         this.tbUserName.Location = new System.Drawing.Point(107, 29);
         this.tbUserName.Name = "tbUserName";
         this.tbUserName.Size = new System.Drawing.Size(177, 20);
         this.tbUserName.TabIndex = 4;
         // 
         // tbPassword
         // 
         this.tbPassword.Location = new System.Drawing.Point(107, 52);
         this.tbPassword.Name = "tbPassword";
         this.tbPassword.Size = new System.Drawing.Size(177, 20);
         this.tbPassword.TabIndex = 5;
         // 
         // tbComputerName
         // 
         this.tbComputerName.Location = new System.Drawing.Point(107, 75);
         this.tbComputerName.Name = "tbComputerName";
         this.tbComputerName.Size = new System.Drawing.Size(177, 20);
         this.tbComputerName.TabIndex = 6;
         // 
         // menuStrip1
         // 
         this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
         this.menuStrip1.Location = new System.Drawing.Point(0, 0);
         this.menuStrip1.Name = "menuStrip1";
         this.menuStrip1.Size = new System.Drawing.Size(378, 24);
         this.menuStrip1.TabIndex = 7;
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
         this.closeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
         this.closeToolStripMenuItem.Text = "C&lose";
         this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
         // 
         // EventLogBackup
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(378, 109);
         this.Controls.Add(this.tbComputerName);
         this.Controls.Add(this.tbPassword);
         this.Controls.Add(this.tbUserName);
         this.Controls.Add(this.label3);
         this.Controls.Add(this.lblPassword);
         this.Controls.Add(this.label1);
         this.Controls.Add(this.cmdGO);
         this.Controls.Add(this.menuStrip1);
         this.MainMenuStrip = this.menuStrip1;
         this.Name = "EventLogBackup";
         this.Text = "EventLogBackup";
         this.menuStrip1.ResumeLayout(false);
         this.menuStrip1.PerformLayout();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Button cmdGO;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Label lblPassword;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.TextBox tbUserName;
      private System.Windows.Forms.TextBox tbPassword;
      private System.Windows.Forms.TextBox tbComputerName;
      private System.Windows.Forms.MenuStrip menuStrip1;
      private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
   }
}