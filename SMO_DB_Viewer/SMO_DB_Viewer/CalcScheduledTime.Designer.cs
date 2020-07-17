namespace SMO_DB_Viewer
{
   partial class CalcScheduledTime
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
         this.tbTime1 = new System.Windows.Forms.TextBox();
         this.tbTime2 = new System.Windows.Forms.TextBox();
         this.label1 = new System.Windows.Forms.Label();
         this.label2 = new System.Windows.Forms.Label();
         this.label3 = new System.Windows.Forms.Label();
         this.tbTime3 = new System.Windows.Forms.TextBox();
         this.cmdCompute = new System.Windows.Forms.Button();
         this.SuspendLayout();
         // 
         // tbTime1
         // 
         this.tbTime1.Location = new System.Drawing.Point(92, 23);
         this.tbTime1.Name = "tbTime1";
         this.tbTime1.Size = new System.Drawing.Size(181, 20);
         this.tbTime1.TabIndex = 0;
         // 
         // tbTime2
         // 
         this.tbTime2.Location = new System.Drawing.Point(92, 49);
         this.tbTime2.Name = "tbTime2";
         this.tbTime2.Size = new System.Drawing.Size(181, 20);
         this.tbTime2.TabIndex = 1;
         // 
         // label1
         // 
         this.label1.AutoSize = true;
         this.label1.Location = new System.Drawing.Point(15, 26);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(35, 13);
         this.label1.TabIndex = 2;
         this.label1.Text = "label1";
         // 
         // label2
         // 
         this.label2.AutoSize = true;
         this.label2.Location = new System.Drawing.Point(15, 52);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(35, 13);
         this.label2.TabIndex = 3;
         this.label2.Text = "label2";
         // 
         // label3
         // 
         this.label3.AutoSize = true;
         this.label3.Location = new System.Drawing.Point(15, 81);
         this.label3.Name = "label3";
         this.label3.Size = new System.Drawing.Size(35, 13);
         this.label3.TabIndex = 4;
         this.label3.Text = "label3";
         // 
         // tbTime3
         // 
         this.tbTime3.Location = new System.Drawing.Point(92, 75);
         this.tbTime3.Name = "tbTime3";
         this.tbTime3.Size = new System.Drawing.Size(181, 20);
         this.tbTime3.TabIndex = 5;
         // 
         // cmdCompute
         // 
         this.cmdCompute.Location = new System.Drawing.Point(280, 21);
         this.cmdCompute.Name = "cmdCompute";
         this.cmdCompute.Size = new System.Drawing.Size(75, 23);
         this.cmdCompute.TabIndex = 6;
         this.cmdCompute.Text = "Complute";
         this.cmdCompute.UseVisualStyleBackColor = true;
         this.cmdCompute.Click += new System.EventHandler(this.cmdCompute_Click);
         // 
         // CalcScheduledTime
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(384, 124);
         this.Controls.Add(this.cmdCompute);
         this.Controls.Add(this.tbTime3);
         this.Controls.Add(this.label3);
         this.Controls.Add(this.label2);
         this.Controls.Add(this.label1);
         this.Controls.Add(this.tbTime2);
         this.Controls.Add(this.tbTime1);
         this.Name = "CalcScheduledTime";
         this.Text = "CalcScheduledTime";
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.TextBox tbTime1;
      private System.Windows.Forms.TextBox tbTime2;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.TextBox tbTime3;
      private System.Windows.Forms.Button cmdCompute;
   }
}