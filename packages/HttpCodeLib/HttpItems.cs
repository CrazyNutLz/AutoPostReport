using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace HttpCodeLib
{
    /// <summary>
    /// Http请求参考类 
    /// </summary>
   [System.Serializable]
    public class HttpItems
    {
        string _URL;
        /// <summary>
        /// 请求URL必须填写
        /// </summary>
        public string URL
        {
            get { return _URL; }
            set { _URL = value; }
        }
        string _Method = "GET";
        /// <summary>
        /// 请求方式默认为GET方式
        /// </summary>
        public string Method
        {
            get { return _Method; }
            set { _Method = value; }
        }
        int _Timeout = 15000;
        /// <summary>
        /// 默认请求超时时间
        /// </summary>
        public int Timeout
        {
            get { return _Timeout; }
            set { _Timeout = value; }
        }
        int _ReadWriteTimeout = 15000;
        /// <summary>
        /// 默认写入Post数据超时间
        /// </summary>
        public int ReadWriteTimeout
        {
            get { return _ReadWriteTimeout; }
            set { _ReadWriteTimeout = value; }
        }
        string _Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
        /// <summary>
        /// 请求标头值 默认为image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*
        /// </summary>
        public string Accept
        {
            get { return _Accept; }
            set { _Accept = value; }
        }
        string _ContentType = "application/x-www-form-urlencoded";
        /// <summary>
        /// 请求返回类型默认 application/x-www-form-urlencoded
        /// </summary>
        public string ContentType
        {
            get { return _ContentType; }
            set { _ContentType = value; }
        }
        string _UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.9 Safari/537.36";
        /// <summary>
        /// 客户端访问信息默认Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.9 Safari/537.36
        /// </summary>
        public string UserAgent
        {
            get { return _UserAgent; }
            set { _UserAgent = value; }
        }

        private Encoding _PostEncoding = Encoding.Default;

        /// <summary>
        /// Post数据时的编码 默认为 Default,如需必要 请勿修改
        /// </summary>
        public Encoding PostEncoding
        {
            get { return _PostEncoding; }
            set { _PostEncoding = value; }
        }
        /// <summary>
        ///  默认的编码类型,如果不初始化,则为null,每次自动识别 
        /// </summary>
        public Encoding Encoding = null;

        string _Encoding = string.Empty;
        /// <summary>
        /// 返回数据编码默认为NUll,可以自动识别
        /// </summary>
        public string EncodingStr
        {
            get { return _Encoding; }
            set { _Encoding = value; }
        }
        private PostDataType _PostDataType = PostDataType.String;
        /// <summary>
        /// Post的数据类型
        /// </summary>
        public PostDataType PostDataType
        {
            get { return _PostDataType; }
            set { _PostDataType = value; }
        }
        string _Postdata;
        /// <summary>
        /// Post请求时要发送的字符串Post数据
        /// </summary>
        public string Postdata
        {
            get { return _Postdata; }
            set { _Postdata = value; }
        }
        private byte[] _PostdataByte = null;
        /// <summary>
        /// Post请求时要发送的Byte类型的Post数据
        /// </summary>
        public byte[] PostdataByte
        {
            get { return _PostdataByte; }
            set { _PostdataByte = value; }
        }
        CookieCollection cookiecollection = null;
        /// <summary>
        /// Cookie对象集合
        /// </summary>
        public CookieCollection CookieCollection
        {
            get { return cookiecollection; }
            set { cookiecollection = value; }
        }
        private CookieContainer _Container = null;
        /// <summary>
        /// 自动处理cookie
        /// </summary>
        public CookieContainer Container
        {
            get { return _Container; }
            set { _Container = value; }
        }
        string _Cookie = string.Empty;
        /// <summary>
        /// 请求时的Cookie
        /// </summary>
        public string Cookie
        {
            get { return _Cookie; }
            set { _Cookie = value; }
        }
        string _Referer = string.Empty;
        /// <summary>
        /// 来源地址，上次访问地址
        /// </summary>
        public string Referer
        {
            get { return _Referer; }
            set { _Referer = value; }
        }
        string _CerPath = string.Empty;
        /// <summary>
        /// 证书绝对路径
        /// </summary>
        public string CerPath
        {
            get { return _CerPath; }
            set { _CerPath = value; }
        }
        string _CerPass;
       /// <summary>
       /// 证书密码
       /// </summary>
        public string CerPass
        {
            get { return _CerPass; }
            set { _CerPass = value; }
        }
        private Boolean isToLower = false;
        /// <summary>
        /// 是否设置为全文小写
        /// </summary>
        public Boolean IsToLower
        {
            get { return isToLower; }
            set { isToLower = value; }
        }
        private Boolean isAjax = false;
        /// <summary>
        /// 是否增加异步请求头
        /// 对应协议 : x-requested-with: XMLHttpRequest
        /// </summary>
        public bool IsAjax
        {
            get { return isAjax; }
            set { isAjax = value; }
        }

        private bool allowautoredirect = true;
        /// <summary>
        /// 支持跳转页面，查询结果将是跳转后的页面
        /// </summary>
        public Boolean Allowautoredirect
        {
            get { return allowautoredirect; }
            set { allowautoredirect = value; }
        }
        private bool _AutoRedirectMax;

        /// <summary>
        /// 如果返回内容为 : 尝试自动重定向的次数太多 请设置本属性为true
        /// 同时应注意设置超时时间(默认15s)
        /// </summary>
        public bool AutoRedirectMax
        {
            get { return _AutoRedirectMax; }
            set { _AutoRedirectMax = value; }
        }
        /// <summary>
        /// 当该属性设置为 true 时，使用 POST 方法的客户端请求应该从服务器收到 100-Continue 响应，以指示客户端应该发送要发送的数据。此机制使客户端能够在服务器根据请求报头打算拒绝请求时，避免在网络上发送大量的数据
        /// </summary>
        private bool expect100Continue = false;
        /// <summary>
        /// 当该属性设置为 true 时，使用 POST 方法的客户端请求应该从服务器收到 100-Continue 响应，以指示客户端应该发送要发送的数据。此机制使客户端能够在服务器根据请求报头打算拒绝请求时，避免在网络上发送大量的数据 默认False
        /// </summary>
        public bool Expect100Continue
        {
            get { return expect100Continue; }
            set { expect100Continue = value; }
        }
        private int connectionlimit = 1024;
        /// <summary>
        /// 最大连接数
        /// </summary>
        public int Connectionlimit
        {
            get { return connectionlimit; }
            set { connectionlimit = value; }
        }
        private string proxyusername = string.Empty;
        /// <summary>
        /// 代理Proxy 服务器用户名
        /// </summary>
        public string ProxyUserName
        {
            get { return proxyusername; }
            set { proxyusername = value; }
        }
        private string proxypwd = string.Empty;
        /// <summary>
        /// 代理 服务器密码
        /// </summary>
        public string ProxyPwd
        {
            get { return proxypwd; }
            set { proxypwd = value; }
        }
        private string proxyip = string.Empty;
        /// <summary>
        /// 代理 服务IP
        /// </summary>
        public string ProxyIp
        {
            get { return proxyip; }
            set { proxyip = value; }
        }
        private ResultType resulttype = ResultType.String;
        /// <summary>
        /// 设置返回类型String和Byte
        /// </summary>
        public ResultType ResultType
        {
            get { return resulttype; }
            set { resulttype = value; }
        }
        private WebHeaderCollection header = new WebHeaderCollection();
        /// <summary>
        /// 头数据
        /// </summary>
        public WebHeaderCollection Header
        {
            get { return header; }
            set { header = value; }
        }
       /// <summary>
       /// 字符串头
       /// </summary>
        public string HeaderStr { get; set; }
        /// <summary>
        /// 如果提示以下错误,请设置为true
        /// 服务器提交了协议冲突. Section=ResponseHeader Detail=CR 后面必须是 LF
        /// </summary>
        public bool UseUnsafe { get; set; }
        /// <summary>
        /// 如果提示以下错误,请设置为true
        /// 从传输流收到意外的 EOF 或 0 个字节 
        /// </summary>
        public bool IsEOFError { get; set; }
       /// <summary>
       /// 是否为So模式
       /// </summary>
        public bool IsSo { get; set; }

        /// <summary>
        /// 文件上传路径
        /// </summary>
        public string UpLoadPath { get; set; }

        /// <summary>
        /// 是否请求图片
        /// </summary>
        public bool IsGetImage { get; set; }
        bool _UseStringCookie = true;
       /// <summary>
        /// 是否使用字符串Cookie
       /// </summary>
        public bool UseStringCookie
        {
            get { return _UseStringCookie; }
            set { _UseStringCookie = value; }
        } 

        /// <summary>
        ///  Https时的传输协议.
        ///  替代原始SecurityProtocolType;
        ///  包含四个版本(Ssl3 Tls Tls1.1 Tls1.2)
        /// </summary>
        public SecurityProtocolTypeEx SecProtocolTypeEx { get; set; }

        /// <summary>
        /// 是否设置SecurityProtocolType 默认为false
        /// 如果需要设置具体属性,请使用SecProtocolTypeEx枚举
        /// </summary>
        public bool IsSetSecurityProtocolType { get; set; }
    }

    /// <summary>
    /// Post的数据格式默认为string
    /// </summary>
    public enum PostDataType
    {
        /// <summary>
        /// 字符串
        /// </summary>
        String,//字符串
        /// <summary>
        /// 字节流
        /// </summary>
        Byte,//字符串和字节流
        /// <summary>
        /// 文件路径
        /// </summary>
        FilePath//表示传入的是文件
    }

    /// <summary>
    /// 返回类型
    /// </summary>
    public enum ResultType
    {
        /// <summary>
        /// 表示只返回字符串
        /// </summary>
        String,
        /// <summary>
        /// 表示只返回字节流
        /// </summary>
        Byte,
        /// <summary>
        /// 急速请求,仅返回数据头 
        /// </summary>
        So
    }

    /// <summary>
    /// Https时的传输协议.
    /// </summary>
    public enum SecurityProtocolTypeEx
    {
        // 摘要: 
        //     指定安全套接字层 (SSL) 3.0 安全协议。
        Ssl3 = 48,
        //
        // 摘要: 
        //     指定传输层安全 (TLS) 1.0 安全协议。
        Tls = 192,
        /// <summary>
        /// 指定传输层安全 (TLS) 1.1 安全协议。
        /// </summary>
        Tls11 = 768,
        /// <summary>
        /// 指定传输层安全 (TLS) 1.2 安全协议。
        /// </summary>
        Tls12 = 3072
    }
}
