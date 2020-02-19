using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoPostReport
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                HtmlDocument document = webBrowser1.Document;
                if (document != null)
                {
                    document.Window.ScrollTo(100, 150);
                }
                var cookie = Global.GetCookieString(webBrowser1.Document.Url.ToString());
                Debug.WriteLine(cookie);

                if (cookie.Contains("MOD_AUTH_CAS"))
                {
                    this.Text = this.Text + "       登陆中 --- 请稍等";
                }


                if (cookie.Contains("EMAP_LANG")&& cookie.Contains("zg_"))
                {
                    Form1.MainForm.NutDebug("登陆成功，开始获取账号信息");
                    var user =  Global.GetUserInfo(cookie);
                    if (user.Name != null)
                    {
                        Form1.MainForm.AddToListView(user);
                    }
                    Global.ClearIECookie();
                    this.Close();
                }
            }
            catch
            {

            }



        }



        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
       
        }
    }
}
