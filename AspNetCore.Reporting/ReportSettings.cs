using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AspNetCore.Reporting
{
    /// <summary>
    /// SSRS Server Settings
    /// </summary>
    public class ReportSettings
    {
        /// <summary>
        /// SSRS Server Url
        /// </summary>
        public string ReportServer
        {
            get { return _server; }
            set
            {
                _server = value;
                _isSSL = new Uri(this.ReportServer).Scheme.Equals("https");
            }
        }
        /// <summary>
        /// Credential 
        /// default is DefaultNetworkCredentials
        /// </summary>
        public ICredentials Credential
        {
            get;
            set;
        } = System.Net.CredentialCache.DefaultNetworkCredentials;
        //public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
        //public ReportRenderType RenderType { get; set; } = ReportRenderType.Html4_0;
        //public int PageIndex { get; set; } = 1;
        //public ReportExecuteType ExecuteType { get; set; } = ReportExecuteType.Display;
        /// <summary>
        /// Whether the toolbar is displayed
        /// </summary>
        public bool ShowToolBar { get; set; } = true;
        /// <summary>
        /// use UserAgent to render Report
        /// </summary>
        public string UserAgent { get; set; }
        //public string SessionId { get; set; }
        /// <summary>
        /// Whether the request is SSL
        /// </summary>
        public bool IsSSL
        {
            get
            {
                return _isSSL;
            }            
        }
        string _server = "http://localhost/ReportServer";
        bool _isSSL = false;
    }
}
