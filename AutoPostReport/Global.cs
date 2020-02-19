using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AutoPostReport
{
    public static class Global
    {
        public struct User
        {
            public String Name;
            public String Cookie;
            public String TodayTaskState;
            public String TodayWid;
            public JToken GetTodayJb;
            public JToken LatestDailyJb;
        };
        public static List<User> UserList = new List<User>();

        public static String CurrtenLoginCookie;

        public static String DataPath = CheckDatapath();
        public static String CheckDatapath()
        {
            if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\Data"))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\Data");
            }
            return Directory.GetCurrentDirectory() + "\\Data";
        }




        /// <summary>
        /// 把账号信息添加到Json
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Cookie"></param>
        public static void AddUserToJson(Global.User AddUser)
        {
            var path = Global.DataPath + "\\UserInfo.json";
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
            }
            var Text = File.ReadAllText(path);

            JObject JsonObj = (JObject)JsonConvert.DeserializeObject(Text);
            JObject NewAccount = new JObject();

            NewAccount.Add("name", AddUser.Name);
            NewAccount.Add("cookie", AddUser.Cookie);
            NewAccount.Add("todaydone", AddUser.TodayTaskState);
            NewAccount.Add("wid", AddUser.TodayWid);
            if (JsonObj == null)
            {
                JsonObj = new JObject(new JProperty("user", new JArray(NewAccount)));
            }

            if (JsonObj["user"] == null)
            {
                JsonObj.Last.AddAfterSelf(new JProperty("user", new JArray()));
            }

            var count = ((JContainer)JsonObj["user"]).Count;


            if (count > 0)
            {
                var AccountList = JsonObj["user"];
                for (int i = 0; i < count; i++)
                {
                    var account = AccountList[i];
                    if (account["name"].ToString() == AddUser.Name)
                    {
                        //如果账号存在于json中
                        account["cookie"] = AddUser.Cookie;
                        account["name"] = AddUser.Name;
                        account["todaydone"] = AddUser.TodayTaskState;
                        account["wid"] = AddUser.TodayWid;

                        AccountList[i] = account;
                        JsonObj["user"] = AccountList;
                        string out_put = Newtonsoft.Json.JsonConvert.SerializeObject(JsonObj, Newtonsoft.Json.Formatting.Indented);
                        File.WriteAllText(path, out_put, UTF8Encoding.UTF8);
                        return;
                    }
                }
            }

            if (count == 0)
            {
                JsonObj["user"] = new JArray(NewAccount);
                string out_put = Newtonsoft.Json.JsonConvert.SerializeObject(JsonObj, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(path, out_put, UTF8Encoding.UTF8);
                return;
            }

            JsonObj["user"].Last.AddAfterSelf(NewAccount);
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(JsonObj, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(path, output, UTF8Encoding.UTF8);
            return;


        }

        /// <summary>
        /// 根据ID删除本地json中的账号数据
        /// </summary>
        /// <param name="ID">论坛id</param>
        public static void RemoveAccountFromJson(String ID)
        {
            try
            {
                var path = Global.DataPath + "\\UserInfo.json";
                if (!File.Exists(path))
                {
                    File.Create(path).Dispose();
                }
                var Text = File.ReadAllText(path);

                JObject JsonObj = (JObject)JsonConvert.DeserializeObject(Text);

                //获取账号数量
                var count = ((JContainer)JsonObj["user"]).Count;

                if (count > 0)
                {
                    var AccountList = JsonObj["user"];
                    var AccountList_out = AccountList;

                    for (int i = 0; i < count; i++)
                    {
                        var account = AccountList[i];

                        if (account["name"].ToString() == ID)
                        {
                            AccountList[i].Remove();
                            JsonObj["user"] = AccountList;
                            string output = Newtonsoft.Json.JsonConvert.SerializeObject(JsonObj, Newtonsoft.Json.Formatting.Indented);
                            File.WriteAllText(path, output, UTF8Encoding.UTF8);
                            break;
                        }
                    }
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// 从本地json读取账号
        /// </summary>
        public static void ReadUserJson()
        {
            var path = Global.DataPath + "\\UserInfo.json";
            if (!File.Exists(path))
            {
                Form1.MainForm.NutDebug("本地没有账号");
                return;
            }
            var Text = File.ReadAllText(path);

            JObject JsonObj = (JObject)JsonConvert.DeserializeObject(Text);

            if (JsonObj == null)
            {
                JsonObj = new JObject(new JProperty("user", new JArray()));
            }
            //获取账号数量

            if (JsonObj["user"] == null)
            {
                JsonObj.Last.AddAfterSelf(new JProperty("user", new JArray()));
            }
            var count = ((JContainer)JsonObj["user"]).Count;

            if (count > 0)
            {
                var users = JsonObj["user"];
                foreach (var user in users)
                {

                    var adduser = new Global.User();
                    adduser.Name = user["name"].ToString();
                    adduser.Cookie = user["cookie"].ToString();
                    adduser.TodayTaskState = user["todaydone"].ToString();
                    adduser.TodayWid = user["wid"].ToString();
                    Form1.MainForm.AddToListView(adduser);
                }
            }
        }


        /// <summary>
        /// 向指定路径中写入Json的键值对
        /// </summary>
        /// <param name="Path">文件路径</param>
        /// <param name="Meau">菜单名字</param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        public static void WriteKeyJson(String Path, String Meau, String Key, String Value)
        {
            if (!File.Exists(Path))
            {
                File.Create(Path).Dispose();
            }
            var Text = File.ReadAllText(Path);

            JObject JsonObj = (JObject)JsonConvert.DeserializeObject(Text);
            JObject MeauObj = new JObject();
            JObject ItemObj = new JObject();


            ItemObj.Add(Key, Value);

            //如果没有内容
            if (JsonObj == null)
            {
                JsonObj = new JObject(new JProperty(Meau, new JArray(ItemObj)));
            }

            //傻逼方法，判断菜单是否存在
            //因为JsonObj[Meau]为null会报错 所以用try
            try
            {
                ItemObj = JObject.Parse(JsonConvert.SerializeObject(JsonObj[Meau].First));
            }
            catch
            {
                //如果菜单不存在
                JsonObj.Last.AddAfterSelf(new JProperty(Meau, new JArray(ItemObj)));
                string output1 = Newtonsoft.Json.JsonConvert.SerializeObject(JsonObj, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(Path, output1, UTF8Encoding.UTF8);
                return;
            }

            bool flag = false;
            foreach (var item in ItemObj)
            {
                if (item.Key == Key)
                {
                    ItemObj[Key] = Value;
                    flag = true;
                }
            }

            if (!flag)
            {
                ItemObj.Add(new JProperty(Key, Value));
            }

            JsonObj[Meau] = new JArray(ItemObj);

            string output = Newtonsoft.Json.JsonConvert.SerializeObject(JsonObj, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(Path, output, UTF8Encoding.UTF8);
        }

        /// <summary>
        /// 读取指定路径中的键值对
        /// </summary>
        /// <param name="Path">路径</param>
        /// <param name="Meau">菜单</param>
        /// <param name="Key">KEY</param>
        /// <returns></returns>
        public static String ReadKeyJson(String Path, String Meau, String Key)
        {
            if (!File.Exists(Path))
            {
                return null;
            }
            var Text = File.ReadAllText(Path);

            JObject JsonObj = (JObject)JsonConvert.DeserializeObject(Text);
            JObject ItemObj = new JObject();

            //傻逼方法，判断菜单是否存在
            //因为JsonObj[Meau]为null会报错 所以用try
            try
            {
                ItemObj = JObject.Parse(JsonConvert.SerializeObject(JsonObj[Meau].First));
            }
            catch
            {
                return null;
            }

            foreach (var item in ItemObj)
            {
                if (item.Key == Key)
                {
                    return ItemObj[Key].ToString();
                }
            }
            return null;
        }


        /// <summary>
        /// 根据cookie获取最新的账号信息
        /// </summary>
        /// <param name="Cookie"></param>
        /// <returns>完成返回null 没完成返回表单wid参数</returns>
        public static Global.User GetUserInfo(String Cookie)
        {
            try
            {
                var GetUrl = "http://ehall.sicnu.edu.cn/qljfwapp/sys/lwReportEpidemicUndergraduate/modules/application/getMyTodayReportWid.do";
                var GetResult = NutWeb.Nut_Get(GetUrl, null, Cookie);

                var GetUrl2 = "http://ehall.sicnu.edu.cn/qljfwapp/sys/lwReportEpidemicUndergraduate/modules/application/getLatestDailyReportData.do";
                var GetResult2 = NutWeb.Nut_Get(GetUrl2, null, Cookie);

                var returnUser = new Global.User();
                if (GetResult != null&& GetResult2!=null)
                {
                    JObject JsonObj = (JObject)JsonConvert.DeserializeObject(GetResult.Html);
                    var rows = JsonObj["datas"]["getMyTodayReportWid"]["rows"].First;

                    JObject JsonObj2 = (JObject)JsonConvert.DeserializeObject(GetResult2.Html);
                    var rows2 = JsonObj2["datas"]["getLatestDailyReportData"]["rows"].First;

                    if (rows != null&& rows2!=null)
                    {
                        returnUser.Cookie = Cookie;
                        returnUser.TodayTaskState = rows["CHECKED_DISPLAY"].ToString();
                        returnUser.Name = rows["USER_NAME"].ToString();
                        returnUser.TodayWid = rows["WID"].ToString();
                        returnUser.GetTodayJb = rows;
                        returnUser.LatestDailyJb = rows2;
                        Form1.MainForm.NutDebug("获取成功！ 当前用户--->" + returnUser.Name);
                        Form1.MainForm.NutDebug("今日填报状态--->" + returnUser.TodayTaskState);
                        return returnUser;
                    }
                }
                Form1.MainForm.NutDebug("当前账号Cookie已经失效");
                returnUser.TodayTaskState = "账号失效";
                return returnUser;
            }
            catch
            {
                Form1.MainForm.NutDebug("当前账号Cookie已经失效");
                var returnUser = new Global.User();
                returnUser.TodayTaskState = "账号失效";
                return returnUser;
            }
        }

        /// <summary>
        /// 中文到Url编码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UrlEncode(string str)
        {
            StringBuilder sb = new StringBuilder();
            byte[] byStr = System.Text.Encoding.UTF8.GetBytes(str); //默认是System.Text.Encoding.Default.GetBytes(str)
            for (int i = 0; i < byStr.Length; i++)
            {
                sb.Append(@"%" + Convert.ToString(byStr[i], 16));
            }

            return (sb.ToString());
        }

        /// <summary>
        /// 取文本中间字符串
        /// </summary>
        /// <param name="left">左边的字符串</param>
        /// <param name="right">右边的字符串</param>
        /// <param name="text">字符串整体</param>
        /// <returns></returns>
        public static string TextGainCenter(string left, string right, string text)
        {
            //判断是否为null或者是empty
            if (string.IsNullOrEmpty(left))
                return "";
            if (string.IsNullOrEmpty(right))
                return "";
            if (string.IsNullOrEmpty(text))
                return "";
            //判断是否为null或者是empty

            int Lindex = text.IndexOf(left); //搜索left的位置

            if (Lindex == -1)
            { //判断是否找到left
                return "";
            }
            Lindex = Lindex + left.Length; //取出left右边文本起始位置

            int Rindex = text.IndexOf(right, Lindex);//从left的右边开始寻找right

            if (Rindex == -1)
            {//判断是否找到right
                return "";
            }
            return text.Substring(Lindex, Rindex - Lindex);//返回查找到的文本
        }

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]    //API设定Cookie
        static extern int InternetSetCookieEx(string lpszURL, string lpszCookieName, string lpszCookieData, int dwFlags, IntPtr dwReserved);
        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]    //API获取Cookie
        static extern bool InternetGetCookieEx(string pchURL, string pchCookieName, StringBuilder pchCookieData, ref System.UInt32 pcchCookieData, int dwFlags, IntPtr lpReserved);
        /// <summary>
        /// url通过API获取完整Cookie
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetCookieString(string url)    
        {
            uint datasize = 256;
            StringBuilder cookieData = new StringBuilder((int)datasize);
            if (!InternetGetCookieEx(url, null, cookieData, ref datasize, 0x2000, IntPtr.Zero))
            {
                if (datasize < 0)
                    return null;
                cookieData = new StringBuilder((int)datasize);
                if (!InternetGetCookieEx(url, null, cookieData, ref datasize, 0x00002000, IntPtr.Zero))
                    return null;
            }
            return cookieData.ToString();
        }


        [DllImport("shell32.dll")]
        static extern IntPtr ShellExecute(IntPtr hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int SHOW);
        /// <summary>
        /// 静默清除IEcookie
        /// </summary>
        public static void ClearIECookie()
        {
            //清除IE临时文件
            // ShellExecute(IntPtr.Zero, "open", "rundll32.exe", " InetCpl.cpl,ClearMyTracksByProcess 255", "", 0);

            Process process = new Process();
            process.StartInfo.FileName = "RunDll32.exe";
            process.StartInfo.Arguments = "InetCpl.cpl,ClearMyTracksByProcess 2";
            process.StartInfo.UseShellExecute = false;        //关闭Shell的使用
            process.StartInfo.RedirectStandardInput = true;   //重定向标准输入
            process.StartInfo.RedirectStandardOutput = true;  //重定向标准输出
            process.StartInfo.RedirectStandardError = true;   //重定向错误输出
            process.StartInfo.CreateNoWindow = true;
            process.Start();
        }








    }

}
