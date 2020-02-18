using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using HttpCodeLib;
using System.Net;
using System.IO;
using System.Drawing;

namespace AutoPostReport
{
    class NutWeb
    {

        /// <summary>
        /// 随机抽取浏览器标识
        /// </summary>
        /// <returns></returns>
        public static String GetRandomUserAgent()
        {
            //不要有手机版的标识  否则会出问题
            String[] Lists =
            {
                "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.95 Safari/537.36 OPR/26.0.1656.60",
                "Mozilla/5.0 (Windows NT 5.1; U; en; rv:1.8.1) Gecko/20061208 Firefox/2.0.0 Opera 9.50",
                "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:34.0) Gecko/20100101 Firefox/34.0",
                "Mozilla/5.0 (X11; U; Linux x86_64; zh-CN; rv:1.9.2.10) Gecko/20100922 Ubuntu/10.10 (maverick) Firefox/3.6.10",
                "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.71 Safari/537.36",
                "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.16 (KHTML, like Gecko) Chrome/10.0.648.133 Safari/534.16",
                "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko",
                "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/30.0.1599.101 Safari/537.36",
                "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/536.11 (KHTML, like Gecko) Chrome/20.0.1132.11 TaoBrowser/2.0 Safari/536.11",
                "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.1 (KHTML, like Gecko) Chrome/21.0.1180.71 Safari/537.1 LBBROWSER",
                "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E; QQBrowser/7.0.3698.400)",
                "Mozilla/5.0 (Windows NT 5.1) AppleWebKit/535.11 (KHTML, like Gecko) Chrome/17.0.963.84 Safari/535.11 SE 2.X MetaSr 1.0",
                "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/38.0.2125.122 UBrowser/4.0.3214.0 Safari/537.36"
            };
            var Random = new Random();
            var RandomNum = Random.Next(0, Lists.Length - 1);

            return Lists[RandomNum];
        }

        /// <summary>
        /// Post
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="Postdata"></param>
        /// <param name="Cookie"></param>
        /// <param name="ProxyIp"></param>
        /// <param name="ContentType"></param>
        /// <param name="Referer"></param>
        /// <returns></returns>
        public static HttpResults Nut_Post(String Url, String Postdata, String Cookie, String ProxyIp, String Referer = null, String ContentType = "application/x-www-form-urlencoded")
        {

            HttpHelpers helper = new HttpHelpers();//请求执行对象
            HttpItems items;//请求参数对象
            HttpResults hr = new HttpResults();//请求结果对象

            string res = string.Empty;//请求结果,请求类型不是图片时有效
            string url = Url;//请求地址
            items = new HttpItems();//每次重新初始化请求对象
            items.URL = url;//设置请求地址

            items.ProxyIp = ProxyIp;//设置代理
            items.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";//设置UserAgent
            items.Referer = Referer;
            Cookie = new XJHTTP().UpdateCookie(Cookie, "");//合并自定义Cookie, 注意!!!!! 仅在有需要合并Cookie的情况下 第一次给 " " 信息,其他类库会自动维护,不需要每次调用更新
            items.Cookie = Cookie;//设置字符串方式提交cookie

            items.Allowautoredirect = true;//设置自动跳转(True为允许跳转) 如需获取跳转后URL 请使用 hr.RedirectUrl
            items.ContentType = ContentType;//内容类型
            items.Method = "POST";//设置请求数据方式为Post
            items.Postdata = Postdata;//Post提交的数据
            hr = helper.GetHtml(items, ref Cookie);//提交请求

            return hr;//返回具体结果
        }

        /// <summary>
        /// GET
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="Cookie"></param>
        /// <returns></returns>
        public static HttpResults Nut_Get(String Url, String ProxyIp, String Cookie = null)
        {
 
            HttpHelpers http = new HttpHelpers();
            HttpItems item = new HttpItems();
            item.ProxyIp = ProxyIp;
            item.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
            item.Cookie = Cookie;
            item.URL = Url;
            item.Timeout = 15000;
            var hr = http.GetHtml(item);

            return hr;
        }

        /// <summary>
        /// 提取账号所需的Cookie
        /// </summary>
        /// <param name="Cookies"></param>
        /// <returns></returns>
        public static String GetImportantCookie(String Cookies)
        {
            var result = "";
            foreach (var cookie in Cookies.Split(';'))
            {

                if (cookie.Contains("MOD_AUTH_CAS"))
                {
                    result = result + cookie + ";";
                }
            }

            return result;
        }

    }
}
