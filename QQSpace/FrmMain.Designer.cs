namespace QQSpace
{
    partial class FrmMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.txtColCmd = new System.Windows.Forms.RichTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btn_colStop = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.btn_colweibo = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.nud_coljiange = new System.Windows.Forms.NumericUpDown();
            this.nud_Pages = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btn_spaceStop = new System.Windows.Forms.Button();
            this.nud_spacejiange = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_test = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSpaceCmd = new System.Windows.Forms.RichTextBox();
            this.btn_colnh = new System.Windows.Forms.Button();
            this.btn_nhStop = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_coljiange)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_Pages)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_spacejiange)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(3, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(494, 466);
            this.tabControl1.TabIndex = 18;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.txtColCmd);
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(486, 440);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "微博抓取";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // txtColCmd
            // 
            this.txtColCmd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtColCmd.BackColor = System.Drawing.SystemColors.ControlText;
            this.txtColCmd.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtColCmd.ForeColor = System.Drawing.Color.SpringGreen;
            this.txtColCmd.Location = new System.Drawing.Point(0, 283);
            this.txtColCmd.Name = "txtColCmd";
            this.txtColCmd.ReadOnly = true;
            this.txtColCmd.Size = new System.Drawing.Size(486, 157);
            this.txtColCmd.TabIndex = 24;
            this.txtColCmd.Text = "";
            this.txtColCmd.TextChanged += new System.EventHandler(this.txtColCmd_TextChanged);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btn_colnh);
            this.panel1.Controls.Add(this.btn_nhStop);
            this.panel1.Controls.Add(this.btn_colStop);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.btn_colweibo);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.nud_coljiange);
            this.panel1.Controls.Add(this.nud_Pages);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Location = new System.Drawing.Point(6, 6);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(419, 80);
            this.panel1.TabIndex = 26;
            // 
            // btn_colStop
            // 
            this.btn_colStop.Location = new System.Drawing.Point(154, 43);
            this.btn_colStop.Name = "btn_colStop";
            this.btn_colStop.Size = new System.Drawing.Size(75, 23);
            this.btn_colStop.TabIndex = 26;
            this.btn_colStop.Text = "停止";
            this.btn_colStop.UseVisualStyleBackColor = true;
            this.btn_colStop.Click += new System.EventHandler(this.btn_colStop_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(101, 50);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 12);
            this.label6.TabIndex = 25;
            this.label6.Text = "分钟";
            // 
            // btn_colweibo
            // 
            this.btn_colweibo.Location = new System.Drawing.Point(154, 14);
            this.btn_colweibo.Name = "btn_colweibo";
            this.btn_colweibo.Size = new System.Drawing.Size(75, 23);
            this.btn_colweibo.TabIndex = 21;
            this.btn_colweibo.Text = "微博抓取";
            this.btn_colweibo.UseVisualStyleBackColor = true;
            this.btn_colweibo.Click += new System.EventHandler(this.btn_col_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 25;
            this.label3.Text = "抓取前";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(29, 50);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 12);
            this.label5.TabIndex = 25;
            this.label5.Text = "间隔";
            // 
            // nud_coljiange
            // 
            this.nud_coljiange.Location = new System.Drawing.Point(64, 46);
            this.nud_coljiange.Name = "nud_coljiange";
            this.nud_coljiange.Size = new System.Drawing.Size(31, 21);
            this.nud_coljiange.TabIndex = 22;
            this.nud_coljiange.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // nud_Pages
            // 
            this.nud_Pages.Location = new System.Drawing.Point(64, 14);
            this.nud_Pages.Name = "nud_Pages";
            this.nud_Pages.Size = new System.Drawing.Size(31, 21);
            this.nud_Pages.TabIndex = 23;
            this.nud_Pages.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(101, 17);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 12);
            this.label4.TabIndex = 25;
            this.label4.Text = "页";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.panel2);
            this.tabPage2.Controls.Add(this.txtSpaceCmd);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(486, 440);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "空间发布";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.btn_spaceStop);
            this.panel2.Controls.Add(this.nud_spacejiange);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.btn_test);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Location = new System.Drawing.Point(6, 6);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(258, 80);
            this.panel2.TabIndex = 27;
            // 
            // btn_spaceStop
            // 
            this.btn_spaceStop.Location = new System.Drawing.Point(154, 43);
            this.btn_spaceStop.Name = "btn_spaceStop";
            this.btn_spaceStop.Size = new System.Drawing.Size(75, 23);
            this.btn_spaceStop.TabIndex = 26;
            this.btn_spaceStop.Text = "停止";
            this.btn_spaceStop.UseVisualStyleBackColor = true;
            this.btn_spaceStop.Click += new System.EventHandler(this.btn_spaceStop_Click);
            // 
            // nud_spacejiange
            // 
            this.nud_spacejiange.Location = new System.Drawing.Point(50, 16);
            this.nud_spacejiange.Name = "nud_spacejiange";
            this.nud_spacejiange.Size = new System.Drawing.Size(31, 21);
            this.nud_spacejiange.TabIndex = 26;
            this.nud_spacejiange.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(87, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 23;
            this.label2.Text = "分钟";
            // 
            // btn_test
            // 
            this.btn_test.Location = new System.Drawing.Point(154, 14);
            this.btn_test.Name = "btn_test";
            this.btn_test.Size = new System.Drawing.Size(75, 23);
            this.btn_test.TabIndex = 21;
            this.btn_test.Text = "发布测试";
            this.btn_test.UseVisualStyleBackColor = true;
            this.btn_test.Click += new System.EventHandler(this.btn_test_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 22;
            this.label1.Text = "间隔";
            // 
            // txtSpaceCmd
            // 
            this.txtSpaceCmd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSpaceCmd.BackColor = System.Drawing.SystemColors.ControlText;
            this.txtSpaceCmd.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtSpaceCmd.ForeColor = System.Drawing.Color.SpringGreen;
            this.txtSpaceCmd.Location = new System.Drawing.Point(0, 276);
            this.txtSpaceCmd.Name = "txtSpaceCmd";
            this.txtSpaceCmd.ReadOnly = true;
            this.txtSpaceCmd.Size = new System.Drawing.Size(486, 164);
            this.txtSpaceCmd.TabIndex = 25;
            this.txtSpaceCmd.Text = "";
            this.txtSpaceCmd.TextChanged += new System.EventHandler(this.txtSpaceCmd_TextChanged);
            // 
            // btn_colnh
            // 
            this.btn_colnh.Location = new System.Drawing.Point(247, 14);
            this.btn_colnh.Name = "btn_colnh";
            this.btn_colnh.Size = new System.Drawing.Size(111, 23);
            this.btn_colnh.TabIndex = 27;
            this.btn_colnh.Text = "内涵段子抓取";
            this.btn_colnh.UseVisualStyleBackColor = true;
            this.btn_colnh.Click += new System.EventHandler(this.btn_colnh_Click);
            // 
            // btn_nhStop
            // 
            this.btn_nhStop.Location = new System.Drawing.Point(247, 43);
            this.btn_nhStop.Name = "btn_nhStop";
            this.btn_nhStop.Size = new System.Drawing.Size(75, 23);
            this.btn_nhStop.TabIndex = 26;
            this.btn_nhStop.Text = "停止";
            this.btn_nhStop.UseVisualStyleBackColor = true;
            this.btn_nhStop.Click += new System.EventHandler(this.btn_nhStop_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(496, 467);
            this.Controls.Add(this.tabControl1);
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "QQ空间自动发布";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_coljiange)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_Pages)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_spacejiange)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.RichTextBox txtColCmd;
        private System.Windows.Forms.NumericUpDown nud_coljiange;
        private System.Windows.Forms.NumericUpDown nud_Pages;
        private System.Windows.Forms.Button btn_colweibo;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.RichTextBox txtSpaceCmd;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_test;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nud_spacejiange;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btn_colStop;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btn_spaceStop;
        private System.Windows.Forms.Button btn_colnh;
        private System.Windows.Forms.Button btn_nhStop;
    }
}

