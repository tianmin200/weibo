using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Weibo.Common;
namespace Weibo
{
    public partial class FrmReadExcel : Form
    {
        public FrmReadExcel()
        {
            InitializeComponent();
        }

        private void btn_FileDialog_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.txt_FileAddr.Text = this.openFileDialog1.FileName;
            }

        }

        private void btn_ReadExcel_Click(object sender, EventArgs e)
        {
            string[] items = File.ReadAllLines(this.txt_FileAddr.Text, Encoding.UTF8);
            string[] contents = new string[0];
            if (this.txt_ContentFileAddr.Text !="")
                contents = File.ReadAllLines(this.txt_ContentFileAddr.Text, Encoding.UTF8);
            int xuanpinkuNum = Convert.ToInt32(this.numericUpDown1.Value);
            if (cbox_UseContentNum.Checked && this.txt_ContentFileAddr.Text != "")
                xuanpinkuNum = contents.Length;
            
            for (int i = 0; i < xuanpinkuNum; i++)
            {
                string content = "";
                if (contents.Length > 0 && i < contents.Length)
                    content = contents[i];
                int[] randomNums = FrmMain.GetRandomNum(9, 1, items.Length - 1);
                int j = 1;
                string dirname = "";
                string own_tbklinks = "";
                foreach (int itemnum in randomNums)
                {
                    string[] item = items[itemnum].Split('\t');
                    string picurl = item[2];
                    string tbklink = item[15];
                    string brand = item[4];
                    if (item.Length == 22) tbklink = item[10];
                    if (j == 1)
                    {
                        if (content != "")
                            dirname = content;
                        else
                        {
                            if (brand.Length >= 4)
                                dirname = brand.Substring(0, 4);
                            else
                                dirname = brand;
                        }
                        //创建选品文件夹
                        if (!Directory.Exists("temp/"))
                            Directory.CreateDirectory("temp");

                        if (!Directory.Exists("temp/" + dirname + "/"))
                            Directory.CreateDirectory("temp/" + dirname + "/");
                        else
                            continue;
                    }
                    //下载图片
                    HttpHelper1.HttpDownloadFile(picurl, "temp/" + dirname + "/" + j.ToString() + ".jpg", new System.Net.CookieContainer());


                    string weiboShortlink = WeiboHandler.GetWeiboShorturl(tbklink);
                    while (weiboShortlink == "")
                    {
                        System.Threading.Thread.Sleep(30 * 1000);
                        weiboShortlink = WeiboHandler.GetWeiboShorturl(tbklink);
                    }
                    own_tbklinks += "【P" + j.ToString() + "：" + weiboShortlink + "】 ";
                    j++;
                }
                if (j == 10)
                {
                    File.WriteAllText("temp/" + dirname + "/comment.txt", own_tbklinks);
                    //如果文件夹重名，先删除老的文件夹
                    if (Directory.Exists("待发布/" + dirname))
                        Directory.Delete("待发布/" + dirname, true);
                    Directory.Move("temp/" + dirname, "待发布/" + dirname);//下載完成放入待发布库
                }
                else
                    Directory.Delete("temp/" + dirname, true);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.txt_ContentFileAddr.Text = this.openFileDialog1.FileName;
            }
        }
    }
}
