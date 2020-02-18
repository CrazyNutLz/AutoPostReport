using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;
using HttpCodeLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;

namespace AutoPostReport
{
    public partial class Form1 : Form
    {
        public static Form1 MainForm;
        Form2 loginform = null;

        public Form1()
        {

            InitializeComponent();
            MainForm = this;
            Form1.MainForm.StartPosition = FormStartPosition.CenterScreen;
        }

        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            Global.ClearIECookie();

            if (loginform == null)
            {
                loginform = new Form2();
                loginform.StartPosition = FormStartPosition.CenterScreen;
                loginform.Show();
            }
            else if (loginform.IsDisposed)
            {
                loginform = new Form2();
                loginform.StartPosition = FormStartPosition.CenterScreen;
                loginform.Show();
            }
            else
            {
                loginform.Activate();
            }
        }

        /// <summary>
        /// cookie直接导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            Global.GetUserInfo(textBox3.Text);
        }

        /// <summary>
        /// 自动顶贴按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            if (RefreshTimer.Enabled == false)
            {
                RefreshTimer.Interval = 1;
                RefreshTimer.Enabled = true;
                button3.Text = "挂机检测中-点击关闭";
                NutDebug("开始挂机");
            }
            else
            {

                RefreshTimer.Enabled = false;

                button3.Text = "开始自动报平安";
                NutDebug("挂机已经停止");
            }
        }

        Thread Timerthread;
        /// <summary>
        /// 自动检测时钟
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            RefreshTimer.Interval = 60000;
            if (Timerthread == null || !Timerthread.IsAlive)
            {
                Timerthread = new Thread(TimerThreadInvoke);
                Timerthread.Start();
            }

        }

        /// <summary>
        /// 时钟方法
        /// </summary>
        public void TimerThreadInvoke()
        {
            TryRefreshTaskState();
            //判断是否在工作时间内
            TimeSpan nowDt = DateTime.Now.TimeOfDay;
            TimeSpan workStartDT = DateTime.Parse("14:00").TimeOfDay;
            TimeSpan workEndDT = DateTime.Parse("23:00").TimeOfDay;
            var count = Global.UserList.Count;
            if (count <= 0)
            {
                Invoke(new Action(() => {NutDebug("列表中没有账号");}));
                return;
            }

            if (nowDt < workStartDT || nowDt > workEndDT) //判断是否在工作时间内
            {
                Invoke(new Action(() => { NutDebug("未在14-20点内,一小时之后再次尝试"); }));
                RefreshTimer.Interval = 3600000;
                return;
            }

            bool flag = false;
            for (var i = 0; i < count; i++)
            {
                if (Global.UserList[i].TodayTaskState == "未填报")
                {
                    flag = PostReport(Global.UserList[i]);
                    if (flag)  //每次只完成一个号
                    {
                        break;
                    }
                }
            }


            if (!flag)
            {
                Invoke(new Action(() =>
                {
                    NutDebug("列表中所有账号已经报平安完毕，1小时之后再次尝试");
                    RefreshTimer.Interval = 3600000;
                }));
            }

        }

        /// <summary>
        /// 将账号添加到ListView并更新List和本地json
        /// </summary>
        /// <param name="AddUser"></param>
        public void AddToListView(Global.User AddUser)
        {
            var List = listView1.Items;

            var AccountNum = Form1.MainForm.listView1.Items.Count;
            if (AccountNum != 0)
            {
                for (int i = 0; i <= AccountNum - 1; i++)
                {
                    String name = "";
                    Invoke(new Action(() =>
                    {
                        name = Form1.MainForm.listView1.Items[i].SubItems[0].Text;
                    }));
                    if (name== AddUser.Name)
                    {
                        //账号已经在列表中
                        Invoke(new Action(() => 
                        {
                            Form1.MainForm.listView1.Items[i].SubItems[1].Text = AddUser.TodayTaskState;
                        }));
                        
                        
                    }
                }
            }
            //更新json
            Global.AddUserToJson(AddUser);
            //更新List
            bool inlist = false;
            foreach (var user in Global.UserList)
            {
                if (AddUser.Name == user.Name)
                {
                    int index = Global.UserList.IndexOf(user);
                    Global.UserList[index] = AddUser;
                    inlist = true;
                    break;
                }

            }

            if (!inlist)
            {
                Invoke(new Action(() =>
                {
                    var item = new ListViewItem(AddUser.Name);
                    item.SubItems.Add(AddUser.TodayTaskState);
                    Global.UserList.Add(AddUser);
                    listView1.Items.Add(item);
                }));
            }

            Global.AddUserToJson(AddUser);
        }

        /// <summary>
        /// 报平安Post
        /// </summary>
        /// <param name="user"></param>
        public bool PostReport(Global.User user)
        {
            try
            {
                user = Global.GetUserInfo(user.Cookie);//刷新一次账号信息
                if (user.TodayTaskState == "未填报")
                {
                    var rows = user.GetTodayJb;
                    //开始整理表单参数
                    Invoke(new Action(() =>
                    {
                        NutDebug("开始自动报平安！");
                    }));
                    String PostData = CreatePostdate(user);
                    var postUrl = "http://ehall.sicnu.edu.cn/qljfwapp/sys/lwReportEpidemicUndergraduate/modules/application/T_REPORT_UNDERGRADUATE_CHECKIN_SAVE.do";
                    var PostResult = NutWeb.Nut_Post(postUrl, PostData, user.Cookie, null);
                    user = Global.GetUserInfo(user.Cookie);//刷新一次账号信息
                    AddToListView(user);
                    return true;

                }
                AddToListView(user);
                return false;
            }
            catch
            {
                Invoke(new Action(() =>
                {
                    NutDebug("未知错误-PostReport");
                }));
                return false;
            }

        }

        /// <summary>
        /// 整理报平安参数
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public String CreatePostdate(Global.User user)
        {
            String ReturnStr = "";
            String dataA = "WID&USER_ID&USER_NAME&GENDER_CODE&GENDER&DEPT_CODE&DEPT_NAME&MAJOR_CODE&MAJOR&CLASS_CODE&CLASS_NAME&TO_LOCATION";
            var dataAs = dataA.Split('&');
            foreach (var key in dataAs)
            {
                ReturnStr += key + "=" + Global.UrlEncode( user.GetTodayJb[key].ToString()) + "&";
            }
            String dataB = "CAMPUS_DISPLAY&CAMPUS&PHONE_NUMBER&PARENT_NAME&PARENT_PHONE&LOCATION_PROVINCE_CODE_DISPLAY&LOCATION_PROVINCE_CODE&LOCATION_CITY_CODE_DISPLAY&LOCATION_CITY_CODE&LOCATION_COUNTY_CODE_DISPLAY&LOCATION_COUNTY_CODE&DETAIL_LOCATION&NOW_LOCATION&INFET_STATE_DISPLAY&INFET_STATE&ISOLATE_STATE_DISPLAY&ISOLATE_STATE&NOW_TEMPERATURE&AFTERNOON_TEMPERATURE&IS_TO_HUBEI_DISPLAY&IS_TO_HUBEI&IS_CONTACT_HUBEI_PEOPLE_DISPLAY&IS_CONTACT_HUBEI_PEOPLE&IS_SYMPTOM_DISPLAY&IS_SYMPTOM&IS_CONTACT_VIRUS_PEOPLE_DISPLAY&IS_CONTACT_VIRUS_PEOPLE&IS_TO_DISASTER_AREA_DISPLAY&IS_TO_DISASTER_AREA&IS_CONTACT_DISASTER_PEOPLE_DISPLAY&IS_CONTACT_DISASTER_PEOPLE&CONTACT_DATE&FAMILY_INFO";
            var dataBs = dataB.Split('&');
            foreach (var key in dataBs)
            {
                ReturnStr += key + "=" + Global.UrlEncode(user.LatestDailyJb[key].ToString()) + "&";
            }
            var time = Global.UrlEncode( DateTime.Now.ToString().Replace(@"/", "-"));
            var time2 = Global.UrlEncode(DateTime.Now.AddDays(-1).ToShortDateString().ToString() + " 23:50:48");

            ReturnStr += "CREATED_AT="+time+"&CZR=&CZZXM=&CZRQ="+time2+"&CHECKED_DISPLAY=%E6%9C%AA%E5%A1%AB%E6%8A%A5&CHECKED=NO";
            return ReturnStr;
        }

        /// <summary>
        /// 尝试更新账号状态
        /// </summary>
        public void TryRefreshTaskState()
        {
            var Today = DateTime.Parse(DateTime.Now.ToShortDateString());

            var path = Global.DataPath + "\\UserInfo.json";

            var LastRestDayInfo = Global.ReadKeyJson(path, "setting", "ressettime");
            if (LastRestDayInfo != null)
            {
                var LastRestDay = DateTime.Parse(LastRestDayInfo);
                if (LastRestDay >= Today)
                {
                    return; //如果上次 刷新的时间 大于等于 今天 就不用刷新
                }
            }
            //刷新操作
            for (int i = 0; i < Global.UserList.Count; i++)
            {
                var CacheAccount = Global.UserList[i];
                CacheAccount.TodayTaskState = "未填报";
                AddToListView(CacheAccount);
            }
            Global.WriteKeyJson(path, "setting", "ressettime", Today.ToString());

        }


        /// <summary>
        /// 检查更新
        /// </summary>
        public void CheckUpdate()
        {
            String Version = "1.0";
            Form1.MainForm.Text = "四川师范大学自动报平安 v" + Version + " by CrazyNut [L.C.G]";
            var serverVersion = NutWeb.Nut_Get("47.103.197.183/software/sicu/version", null);
            if (serverVersion != null)
            {
                if (Version != serverVersion.Html)
                {
                    NutDebug("\r\n\r\n当前软件版本：" + Version + " 服务器最新版本:" + serverVersion.Html + " \r\n\r\n请到下方链接下载更新\r\n\r\nhttp://47.103.197.183/software/sicu/川师自动报平安.zip");
                }

            }

        }


        public delegate void DebugWindowDelegate(String Text);
        public void NutDebug(String Text)
        {

            if (InvokeRequired)
            {
                DebugWindowDelegate _NutDebug = new DebugWindowDelegate(NutDebug);
                DebugWindow.Invoke(_NutDebug, new object[] { Text });
            }
            else
            {
                var time = DateTime.Now.ToString().Replace(@"/", "-");
                var text = "[" + time + "]--->" + Text;

                Debug.WriteLine(text);

                DebugWindow.Text += "\r\n" + text + "\r\n";

                if (DebugWindow.Text.Length > 10000)
                {
                    DebugWindow.Text = DebugWindow.Text.Remove(0, 3000);
                }

                DebugWindow.Focus();
                DebugWindow.Select(DebugWindow.Text.Length, 0);
                DebugWindow.ScrollToCaret();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Global.ReadUserJson();
            TryRefreshTaskState();
            CheckUpdate();
        }



        private void button4_Click(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count > 0)//判断listview有被选中项
            {

                var id = listView1.SelectedItems[0].SubItems[0].Text;
                foreach (var account in Global.UserList)
                {
                    if (id == account.Name)
                    {
                        Global.GetUserInfo(account.Cookie);
                        return;
                    }

                }
            }
            else
            {
                MessageBox.Show("未选中账号");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count > 0)//判断listview有被选中项
            {
                var id = listView1.SelectedItems[0].SubItems[0].Text.Replace("\n", "");
                //从list中移除
                foreach (var account in Global.UserList)
                {
                    if (account.Name == id)
                    {
                        Global.UserList.Remove(account);
                        break;
                    }
                }
                //从本地json中移除
                Global.RemoveAccountFromJson(id);
                var Index = this.listView1.SelectedItems[0].Index;//取当前选中项的index,SelectedItems[0]这必须为0       
                listView1.Items[Index].Remove();
            }
            else
            {
                MessageBox.Show("未选中账号");
            }
        }
    }
}
