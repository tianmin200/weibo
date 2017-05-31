namespace Weibo
{
    partial class FrmReadExcel
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
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btn_FileDialog = new System.Windows.Forms.Button();
            this.txt_FileAddr = new System.Windows.Forms.TextBox();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.btn_ReadExcel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_ContentFileAddr = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.cbox_UseContentNum = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // btn_FileDialog
            // 
            this.btn_FileDialog.Location = new System.Drawing.Point(345, 62);
            this.btn_FileDialog.Name = "btn_FileDialog";
            this.btn_FileDialog.Size = new System.Drawing.Size(75, 23);
            this.btn_FileDialog.TabIndex = 0;
            this.btn_FileDialog.Text = "浏览";
            this.btn_FileDialog.UseVisualStyleBackColor = true;
            this.btn_FileDialog.Click += new System.EventHandler(this.btn_FileDialog_Click);
            // 
            // txt_FileAddr
            // 
            this.txt_FileAddr.Location = new System.Drawing.Point(82, 35);
            this.txt_FileAddr.Name = "txt_FileAddr";
            this.txt_FileAddr.Size = new System.Drawing.Size(338, 21);
            this.txt_FileAddr.TabIndex = 1;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(190, 172);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(58, 21);
            this.numericUpDown1.TabIndex = 2;
            this.numericUpDown1.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // btn_ReadExcel
            // 
            this.btn_ReadExcel.Location = new System.Drawing.Point(131, 229);
            this.btn_ReadExcel.Name = "btn_ReadExcel";
            this.btn_ReadExcel.Size = new System.Drawing.Size(200, 30);
            this.btn_ReadExcel.TabIndex = 3;
            this.btn_ReadExcel.Text = "生成发布库";
            this.btn_ReadExcel.UseVisualStyleBackColor = true;
            this.btn_ReadExcel.Click += new System.EventHandler(this.btn_ReadExcel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(80, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "选品库文件（TXT）：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(80, 85);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(131, 12);
            this.label2.TabIndex = 7;
            this.label2.Text = "内容素材文件（TXT）：";
            // 
            // txt_ContentFileAddr
            // 
            this.txt_ContentFileAddr.Location = new System.Drawing.Point(82, 100);
            this.txt_ContentFileAddr.Name = "txt_ContentFileAddr";
            this.txt_ContentFileAddr.Size = new System.Drawing.Size(338, 21);
            this.txt_ContentFileAddr.TabIndex = 6;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(345, 127);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "浏览";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(83, 174);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "生成发布库数量：";
            // 
            // cbox_UseContentNum
            // 
            this.cbox_UseContentNum.AutoSize = true;
            this.cbox_UseContentNum.Location = new System.Drawing.Point(265, 173);
            this.cbox_UseContentNum.Name = "cbox_UseContentNum";
            this.cbox_UseContentNum.Size = new System.Drawing.Size(120, 16);
            this.cbox_UseContentNum.TabIndex = 9;
            this.cbox_UseContentNum.Text = "使用内容素材数量";
            this.cbox_UseContentNum.UseVisualStyleBackColor = true;
            // 
            // FrmReadExcel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 343);
            this.Controls.Add(this.cbox_UseContentNum);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txt_ContentFileAddr);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_ReadExcel);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.txt_FileAddr);
            this.Controls.Add(this.btn_FileDialog);
            this.Name = "FrmReadExcel";
            this.Text = "读取选品库Excel";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btn_FileDialog;
        private System.Windows.Forms.TextBox txt_FileAddr;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Button btn_ReadExcel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_ContentFileAddr;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox cbox_UseContentNum;
    }
}