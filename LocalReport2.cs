/* ===============================================
* 功能描述：AspNetCore.Reporting.LocalReport
* 创 建 者：WeiGe
* 创建日期：8/17/2018 10:42:21 PM
* ===============================================*/

using AspNetCore.ReportingServices.Interfaces;
using AspNetCore.ReportingServices.Library;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer;
using AspNetCore.ReportingServices.Rendering.HtmlRenderer;
using ImageRender= AspNetCore.ReportingServices.Rendering.ImageRenderer.ImageRenderer;
using PDFRender = AspNetCore.ReportingServices.Rendering.ImageRenderer.PDFRenderer;
using AspNetCore.ReportingServices.Rendering.RPLRendering;
using AspNetCore.ReportingServices.Rendering.WordRenderer;
using AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AspNetCore.Reporting
{
   

    internal class DataSource
    {
        public DataSource(string name, object value)
        {
            this.Name = Check.NotEmpty(name, nameof(name));
            this.Value = Check.NotNull(value, nameof(value));
        }
        public string Name { get; }
        public object Value { get;}
    }
    internal class ExecutionInfo
    {
        public ExecutionInfo(string reportName)
        {
            if(string.IsNullOrWhiteSpace(reportName)) throw new ArgumentNullException(nameof(reportName));
            this.Name = reportName;
            this.Id = reportName.ToLower().GetHashCode();
        }
        public ExecutionInfo(string reportName,int id)
        {
            if (string.IsNullOrWhiteSpace(reportName)) throw new ArgumentNullException(nameof(reportName));
            this.Name = reportName;            
            this.Id = id;            
        }
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
        public RenderType RenderType { get; set; }
        /// <summary>
        /// the report parameters of the conditions
        /// </summary>
        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
        /// <summary>
        /// set file name for export file
        /// </summary>
        /// <param name="append"></param>
        public void SetFileName(string append = null)
        {
            string name = string.Empty;
            if (string.IsNullOrEmpty(this.Name))
            {
                if (!string.IsNullOrEmpty(this.Path) && this.Path.IndexOf("/") > -1)
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
            else
            {
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
    /// <summary>
    /// Report Execute Type
    /// </summary>
    internal enum ExecuteType
    {
        /// <summary>
        /// show html
        /// </summary>
        Display = 0,
        /// <summary>
        /// export file
        /// </summary>
        Export = 1,
        /// <summary>
        /// find strings in report
        /// </summary>
        FindString = 2,
        /// <summary>
        /// toggle the report for show or hide child content
        /// </summary>
        Toggle = 3,
        /// <summary>
        /// Print report
        /// </summary>
        Print = 4,
    }


    /// <summary>
    /// the report executed result
    /// </summary>
    internal class ReportResponse
    {
        /// <summary>
        /// string contents
        /// e.g. html
        /// </summary>
        public string Contents { get; internal set; }
        /// <summary>
        /// the report session id
        /// </summary>
        public string SessionId { get; internal set; }
        /// <summary>
        /// which page of report to excuting
        /// </summary>
        public int PageIndex { get; internal set; }
        /// <summary>
        /// the total page count of report
        /// </summary>
        public int PageCount { get; internal set; }
        /// <summary>
        /// 版本号
        /// </summary>
        public Version Version { get; internal set; }
        /// <summary>
        /// used for export the fileName
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// the content stream, for download file
        /// </summary>
        public byte[] Stream { get; internal set; }
        /// <summary>
        /// the file mime type
        /// </summary>
        public string MimeType { get; internal set; }
        /// <summary>
        /// the file extension
        /// </summary>
        public string Extension { get; internal set; }
    }

    /// <summary>
    /// Check
    /// </summary>
    internal sealed class Check
    {
        /// <summary>
        /// NotNull
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public static T NotNull<T>(T value, string parameterName)
        {
            if (value == null)
            {
                NotEmpty(parameterName, nameof(parameterName));

                throw new ArgumentNullException(parameterName);
            }

            return value;
        }

        /// <summary>
        /// NotNull
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static T NotNull<T>(
             T value,
            string parameterName,
             string propertyName)
        {
            if (value == null)
            {
                NotEmpty(parameterName, nameof(parameterName));
                NotEmpty(propertyName, nameof(propertyName));

                throw new ArgumentException($"The property '{propertyName}' of the argument '{parameterName}' cannot be null.");
            }

            return value;
        }

        /// <summary>
        /// NotEmpty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public static IReadOnlyList<T> NotEmpty<T>(IReadOnlyList<T> value, string parameterName)
        {
            NotNull(value, parameterName);

            if (value.Count == 0)
            {
                NotEmpty(parameterName, nameof(parameterName));

                throw new ArgumentException($"The collection argument '{parameterName}' must contain at least one element.");
            }

            return value;
        }

        /// <summary>
        /// NotEmpty
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public static string NotEmpty(string value, string parameterName)
        {
            Exception e = null;
            if (value == null)
            {
                e = new ArgumentNullException(parameterName);
            }
            else if (value.Trim().Length == 0)
            {
                e = new ArgumentException($"The string argument '{parameterName}' cannot be empty.");
            }

            if (e != null)
            {
                NotEmpty(parameterName, nameof(parameterName));

                throw e;
            }

            return value;
        }
        /// <summary>
        /// NullButNotEmpty
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public static string NullButNotEmpty(string value, string parameterName)
        {
            if (value != null
                && value.Length == 0)
            {
                NotEmpty(parameterName, nameof(parameterName));

                throw new ArgumentException($"The string argument '{parameterName}' cannot be empty.");
            }

            return value;
        }
        /// <summary>
        /// HasNoNulls
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public static IList<T> HasNoNulls<T>(IList<T> value, string parameterName)
            where T : class
        {
            NotNull(value, parameterName);

            if (value.Any(e => e == null))
            {
                NotEmpty(parameterName, nameof(parameterName));

                throw new ArgumentException(parameterName);
            }

            return value;
        }

        public static void Assert(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(message);
            }
        }
        public static void Assert(bool condition, string format, params object[] values)
        {
            if (!condition)
            {
                throw new InvalidOperationException(string.Format(format, values));
            }
        }
        public static void Assert(bool condition)
        {
            if (!condition)
            {
                throw new InvalidOperationException();
            }
        }
    }
}
