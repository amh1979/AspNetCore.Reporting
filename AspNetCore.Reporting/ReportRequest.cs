using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Reporting
{
    /// <summary>
    /// how to Excute for report
    /// </summary>
    public class ReportRequest
    {
        string _fileName = string.Empty;
        /// <summary>
        /// The report id
        /// The SSRS useless, but can used by client to set something
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// report name, used for export the fileName
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// this is used by export
        /// </summary>
        public string FileName
        {
            get
            {
                if (string.IsNullOrEmpty(_fileName))
                {
                    SetFileName();
                }
                return _fileName;
            }
            protected set { _fileName = value; }
        }
        /// <summary>
        /// The report path on server
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// the report session id, can used by client to excute report fast
        /// </summary>
        public string SessionId { get; set; }
        /// <summary>
        /// set what character to find in report
        /// </summary>
        public string FindString { get; set; }
        /// <summary>
        /// the colse div for html which is the element id
        /// </summary>
        public string ToggleId { get; set; }
        /// <summary>
        /// the page index which to display for html
        /// </summary>
        public int PageIndex { get; set; }
        //public bool RefreshReport { get; set; }
        /// <summary>
        ///  Whether reload report data by ignore the session id
        /// </summary>

        public bool Reset { get; set; }
        /// <summary>
        /// how to render the report data
        /// </summary>
        public ReportRenderType RenderType { get; set; }
        /// <summary>
        /// how to execute the report
        /// </summary>
        public ReportExecuteType ExecuteType { get; set; }
        /// <summary>
        /// the report parameters of the conditions
        /// </summary>
        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
        /// <summary>
        /// set file name for export file
        /// </summary>
        /// <param name="append"></param>
        public void SetFileName(string append=null)
        {
            string name = string.Empty;
            if (string.IsNullOrEmpty(this.Name))
            {
                if (!string.IsNullOrEmpty(this.Path) &&this.Path.IndexOf("/") > -1)
                {
                    var m = this.Path.Split('/');
                    var s = m[m.Length - 1];
                    if (string.IsNullOrEmpty(s))
                    {
                        s = m[m.Length - 2];
                    }
                    name = s;
                }
                else
                {
                    name = this.Path;
                }
            }
            else {
                name = this.Name;
            }
            if (string.IsNullOrEmpty(name))
            {
                name = "Report";
            }
            name = name.Replace(" ", "_");
            if (!string.IsNullOrEmpty(append))
            {
                name = $"{name}_{append}";
            }
            _fileName = name;
        }
    }
}
