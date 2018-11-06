using AspNetCore.Reporting.DeviceInfos;
using AspNetCore.Reporting.ReportExecutionService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Security.Principal;
using System.Collections.Concurrent;

namespace AspNetCore.Reporting
{
    class StringCompare : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            return x.Equals(y, StringComparison.OrdinalIgnoreCase);
        }
        public int GetHashCode(string obj)
        {
            return obj.ToLower().GetHashCode();
        }
    }
    /// <summary>
    /// SSRS Report Viewer 
    /// </summary>
    public partial class ServerReport
    {
        /// <summary>
        /// MSSQL Server 2012
        /// </summary>
        const int SuportExportVersion = 11;
        static Regex VersionRegex = new Regex("([\\d\\.]+)$");
        static ConcurrentDictionary<string, Version> Versions = new ConcurrentDictionary<string, Version>(new StringCompare());
        /// <summary>
        /// 
        /// </summary>
        //protected static Version ReportServerVersion { get; private set; }
        #region 
        const string ExecutionPath = "/ReportExecution2005.asmx";
        static ReportExecutionServiceSoapClient client;
        //string _historyId;
        #endregion

        #region        

        /// <summary>
        /// init the report viewer, e.g. set report server,set certificate
        /// </summary>
        /// <param name="settings"></param>
        public ServerReport(ReportSettings settings)
        {
            this.ReportSettings = settings;
            ReportException ex;
            if (!CheckSettings(out ex))
            {
                throw ex;
            }
        }
        #endregion


        #region
        //public ICredentials Credential { get;protected set; } = System.Net.CredentialCache.DefaultNetworkCredentials;


        //public string ReportServer
        //{
        //    get { return _server; }
        //    protected set
        //    {
        //        client = null;
        //        _server = value;
        //    }
        //}
        //public bool IsSSL { get; private set; }

        internal ReportSettings ReportSettings { get; set; }
        #endregion

        #region Execute

        /// <summary>
        /// execute report by use <see cref="ReportRequest"/> settings
        /// </summary>
        /// <param name="request">the request settings for execute report</param>
        /// <returns></returns>
        public ReportResponse Execute(ReportRequest request)
        {
            switch (request.ExecuteType)
            {
                case ReportExecuteType.Display:
                    return Display(request);
                case ReportExecuteType.Export:
                    return Export(request);
                case ReportExecuteType.FindString:
                    return FindString(request);
                case ReportExecuteType.Toggle:
                    return Toggle(request);
                case ReportExecuteType.Print:
                    request.RenderType = ReportRenderType.WordOpenXml;
                    return Export(request);
                default:
                    return new ReportResponse { Status = 1, Message = "ExecuteType not provide or incorrect." };
            }
        }

        protected ReportResponse Display(ReportRequest request)
        {
            ReportResponse response = new ReportResponse();
            try
            {
                ReportExecuteResult result = new ReportExecuteResult();
                LoadReport(request, ref result);                
                Render(request, ref result);
                GetPageNumber(request, ref result);
                response.Data = SetData(result,false);                
            }
            catch (Exception ex)
            {
                response.Status = 1;
                response.Message = ex.Message;
            }
            return response;

        }
        protected ReportData SetData(ReportExecuteResult result,bool appendStream=true)
        {
            var data = new ReportData
            {
                SessionId = result.SessionId,
                Extension = result.Extension,
                MimeType = result.MimeType,
                PageCount = result.PageCount,
                PageIndex = result.PageIndex,
            };
            if (Versions.TryGetValue(this.ReportSettings.ReportServer, out Version version))
            {
                data.Version = version;
            }
            if (appendStream&&result.Stream != null)
            {
                data.Stream = result.Stream;
            }
            if (!string.IsNullOrEmpty(result.Contents))
            {
                data.Contents = GetContent(result.Contents, ReportSettings.ShowToolBar);
            }
            return data;
        }
        protected ReportResponse Export(ReportRequest request)
        {
            ReportResponse response = new ReportResponse();
            try
            {
                ReportExecuteResult result = new ReportExecuteResult();
                LoadReport(request, ref result);
                Render(request, ref result);
                response.Data = SetData(result);

                //response.Data.FileName = fileName;
            }
            catch (Exception ex)
            {
                response.Status = 1;
                response.Message = ex.Message;

            }
            return response;

        }
        protected ReportResponse FindString(ReportRequest request)
        {
            ReportResponse response = new ReportResponse();
            if (string.IsNullOrEmpty(request.FindString))
            {
                response.Status = 2;
                response.Message = "Search value can not be empty.";
                return response;
            }
            try
            {
                ReportExecuteResult result = new ReportExecuteResult();
                LoadReport(request, ref result);
                Render(request, ref result);
                GetPageNumber(request, ref result);

                var request0 = new FindStringRequest
                {
                    FindValue = request.FindString,
                    StartPage = request.PageIndex,
                    EndPage = result.PageCount
                };
                var response0 = ReportClient.FindStringAsync(request0).GetAwaiter().GetResult();
                if (response0.PageNumber > 0)
                {
                    result.PageIndex = request.PageIndex = response0.PageNumber;
                }
                else
                {
                    if (request.PageIndex > 1)
                    {
                        request0.StartPage = 1;
                    }
                    response0 = ReportClient.FindStringAsync(request0).GetAwaiter().GetResult();
                    if (response0.PageNumber < 1)
                    {
                        response.Status = 23;
                        response.Message = "The Find Text not found in the report";
                        return response;
                    }
                    else
                    {
                        result.PageIndex = request.PageIndex = response0.PageNumber;
                    }
                }
                Render(request, ref result);
                response.Data = SetData(result);
                response.Data.Contents = GetContent(result.Stream, ReportSettings.ShowToolBar);
            }
            catch (Exception ex)
            {
                response.Status = 1;
                response.Message = ex.Message;
            }
            return response;

        }
        protected ReportResponse Toggle(ReportRequest request)
        {
            ReportResponse response = new ReportResponse();
            if (string.IsNullOrEmpty(request.ToggleId))
            {
                response.Status = 2;
                response.Message = "The ToggleId can not be empty.";
                return response;
            }
            try
            {
                ReportExecuteResult result = new ReportExecuteResult();
                LoadReport(request, ref result);
                var s = ReportClient.ToggleItemAsync(new ToggleItemRequest(request.ToggleId)).GetAwaiter().GetResult();
                if (s.Found)
                {
                    Render(request, ref result);
                    GetPageNumber(request, ref result);
                    response.Data = SetData(result);
                    response.Data.Contents = GetContent(result.Stream, ReportSettings.ShowToolBar);
                }
                else
                {
                    response.Status = 5;
                    response.Message = "Toggle: not found item";
                }
            }
            catch (Exception ex)
            {
                response.Status = 1;
                response.Message = "Toggle:" + ex.Message;
            }
            return response;
        }

        protected string GetContent(byte[] bytes, bool showToolBar)
        {

            string content = System.Text.Encoding.UTF8.GetString(bytes);
            return GetContent(content, showToolBar);
        }
        protected string GetContent(string content, bool showToolBar)
        {
            
            StringBuilder sb = new StringBuilder();
            Regex reg1 = new Regex("(<style [^>]+>[^<]+?</style>)", RegexOptions.IgnoreCase);
            var m = reg1.Match(content);
            if (m.Success)
            {
                sb.AppendLine(m.Result("$1"));
            }
            Regex reg2 = new Regex("<body [^>]+>([\\w\\W]*?)</body>", RegexOptions.IgnoreCase);
            m = reg2.Match(content);
            if (m.Success)
            {
                if (ReportSettings.ShowToolBar)
                {
                    sb.AppendLine(TOOL_BAR_STRING);
                    content = string.Concat(TOOL_BAR_STRING, content);
                }
                sb.AppendLine(m.Result("$1"));
            }

            return sb.ToString();
        }
        /// <summary>
        /// step 1
        /// </summary>
        protected void LoadReport(ReportRequest request, ref ReportExecuteResult result)
        {
            if (string.IsNullOrEmpty(request.Path))
            {
                throw new ReportException("Please set ReportPath to execute.");
            }
            try
            {
                if (!string.IsNullOrEmpty(request.SessionId))
                {
                    try
                    {
                        if (request.Reset)
                        {
                            var r = ReportClient.ResetExecutionAsync(new ResetExecutionRequest()).GetAwaiter().GetResult();
                            result.SessionId = r.executionInfo.ExecutionID;
                        }
                        ReportClient.ExecutionHeader.ExecutionID = request.SessionId;
                        var rr = ReportClient.GetExecutionInfoAsync(new GetExecutionInfoRequest()).GetAwaiter().GetResult();
                    }
                    catch { request.SessionId = null; }
                }
                if (string.IsNullOrEmpty(request.SessionId))
                {
                    LoadReportRequest request0 = new LoadReportRequest(request.Path, null);
                    LoadReportResponse response = ReportClient.LoadReportAsync(request0).GetAwaiter().GetResult();

                    try
                    {
                        var match = VersionRegex.Match(response.ServerInfoHeader.ReportServerVersion);
                        if (match.Success)
                        {
                            Versions.TryAdd(this.ReportSettings.ReportServer, Version.Parse(match.Result("$1")));
                        }
                    }
                    catch { }


                    result.ParametersRequired = response.executionInfo.ParametersRequired;
                    var dict = new Dictionary<string, string>(response.executionInfo.Parameters.Length);
                    if (response.executionInfo.Parameters.Length > 0)
                    {
                        foreach (var p in response.executionInfo.Parameters)
                        {
                            if (p.DefaultValues != null && p.DefaultValues.Length > 0)
                            {
                                dict[p.Name] = p.DefaultValues.SingleOrDefault();
                            }
                            if (request.Parameters.ContainsKey(p.Name))
                            {
                                dict[p.Name] = request.Parameters[p.Name];                                
                            }
                        }
                    }
                    request.Parameters = dict;
                    result.SessionId = request.SessionId = response.executionInfo.ExecutionID;
                    //ReportClient.ToggleItemAsync()
                    SetParameters(request, result);
                }

            }
            catch (Exception ex)
            {
                throw new ReportException("LoadReport error: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// step 2.
        /// </summary>
        protected void SetParameters(ReportRequest rRequest, ReportExecuteResult result)
        {
            if (result.ParametersRequired && rRequest.Parameters.Count == 0)
            {
                throw new ReportException("The report parameters is required.");
            }
            try
            {
                var paramenters = rRequest.Parameters.Select(t => new ParameterValue { Name = t.Key, Value = t.Value }).ToArray();
                var request = new SetExecutionParametersRequest(paramenters, "en-us");
                var response = ReportClient.SetExecutionParametersAsync(request).GetAwaiter().GetResult();
                result.SessionId = rRequest.SessionId = response.executionInfo.ExecutionID;
            }
            catch (Exception ex)
            {
                throw new ReportException("SetParameters error: " + ex.Message, ex);
            }
        }
        /// <summary>
        /// step 3.
        /// </summary>
        /// <param name="result"></param>
        protected void Render(ReportRequest rRequest, ref ReportExecuteResult result, string findString = null)
        {
            try
            {
                var deviceInfo = GenerateDeviceInfo(rRequest);
                string format;
                Versions.TryGetValue(this.ReportSettings.ReportServer, out Version version);
                switch (rRequest.RenderType)
                {
                    case ReportRenderType.Html4_0:
                        format = "Html4.0";
                        break;
                    case ReportRenderType.ExcelOpenXml:
                        if (version.Major < SuportExportVersion)
                        {
                            format = ReportRenderType.Excel.ToString();
                        }
                        else
                        {
                            format = ReportRenderType.ExcelOpenXml.ToString();
                        }
                        break;
                    case ReportRenderType.WordOpenXml:
                        if (version.Major < SuportExportVersion)
                        {
                            format = ReportRenderType.Word.ToString();
                        }
                        else
                        {
                            format = ReportRenderType.WordOpenXml.ToString();
                        }
                        break;
                    default:
                        format = rRequest.RenderType.ToString();
                        break;
                }
                var strDeviceInfo = deviceInfo.ToString();

                //strDeviceInfo = @"<DeviceInfo><HTMLFragment>true</HTMLFragment><Section>0</Section></DeviceInfo>";

                var request = new Render2Request(format, strDeviceInfo,ReportExecutionService.PageCountMode.Actual);
                var response = ReportClient.Render2Async(request).GetAwaiter().GetResult();
                if (rRequest.RenderType == ReportRenderType.Html4_0 || rRequest.RenderType == ReportRenderType.Html5)
                {
                    StringBuilder sb = new StringBuilder(Encoding.UTF8.GetString(response.Result));
                    Regex reg = new Regex("(<img [^>]*? src=\"([^\"]+&rs%3AImageID=([^\"']+))\")", RegexOptions.IgnoreCase);
                    var matchs = reg.Match(sb.ToString());
                    while (matchs.Success)
                    {
                        var a = matchs.Result("$1");
                        var b = matchs.Result("$2");
                        var c = matchs.Result("$3");
                        var cc = ReportClient.RenderStreamAsync(new RenderStreamRequest(format, c, strDeviceInfo)).GetAwaiter().GetResult();
                        var img = $"data:{cc.MimeType};base64,{Convert.ToBase64String(cc.Result)}";
                        var aa = a.Replace(b, img);
                        sb.Replace(a, aa);
                        matchs = matchs.NextMatch();
                    }
                    result.Contents = sb.ToString();
                }
                result.Stream = response.Result;
                result.Encoding = response.Encoding;
                result.Extension = response.Extension;
                result.MimeType = response.MimeType;
            }
            catch (Exception ex)
            {
                throw new ReportException("Render error: " + ex.Message, ex);
            }
        }
        /// <summary>
        /// step 4
        /// </summary>
        /// <param name="result"></param>
        protected void GetPageNumber(ReportRequest rRequest, ref ReportExecuteResult result)
        {
            try
            {
                Versions.TryGetValue(this.ReportSettings.ReportServer, out Version version);
                if (version.Major <= SuportExportVersion)
                {
                    var request = new GetExecutionInfo2Request();

                    var response = ReportClient.GetExecutionInfo2Async(request).GetAwaiter().GetResult();
                    result.PageCount = response.executionInfo.NumPages;
                    result.PageIndex = rRequest.PageIndex;
                    result.SessionId
                        = rRequest.SessionId
                        = ReportClient.ExecutionHeader.ExecutionID
                        = response.executionInfo.ExecutionID;

                }
                else {
                    var request = new GetExecutionInfo3Request();

                    var response = ReportClient.GetExecutionInfo3Async(request).GetAwaiter().GetResult();
                    result.PageCount = response.executionInfo.NumPages;
                    result.PageIndex = rRequest.PageIndex;
                    result.SessionId
                        = rRequest.SessionId
                        = ReportClient.ExecutionHeader.ExecutionID
                        = response.executionInfo.ExecutionID;
                }
                   
            }
            catch (Exception ex)
            {
                throw new ReportException("GetPageNumber error: " + ex.Message, ex);
            }
        }
        #endregion

        #region DeviceInfo
        private DeviceInfoBase GenerateDeviceInfo(ReportRequest rRequest)
        {
            if (rRequest.ExecuteType == ReportExecuteType.Export)
            {
                var di = new DeviceInfoBase
                {
                    Toolbar = false
                };
                return di;
            }
            HtmlDeviceInfo deviceInfo = new HtmlDeviceInfo();
            deviceInfo.HTMLFragment = false;
            if (rRequest.PageIndex < 1)
            {
                rRequest.PageIndex = 1;
            }
            deviceInfo.Section = rRequest.PageIndex;
            deviceInfo.Toolbar = false;
            if (!string.IsNullOrEmpty(rRequest.FindString))
            {
                deviceInfo.FindString = rRequest.FindString;
                deviceInfo.Toolbar = null;
            }
            if (!string.IsNullOrEmpty(this.ReportSettings.UserAgent))
            {
                deviceInfo.UserAgent = this.ReportSettings.UserAgent;
            }
            //deviceInfo.ExpandContent = true;
            // deviceInfo.ToggleItems = true;
            //deviceInfo.MeasureItems = true;
            //deviceInfo.StylePrefixId
            return deviceInfo;
        }
        protected Binding CreateBinding()
        {
            CustomBinding result = new CustomBinding();
            
            TextMessageEncodingBindingElement textBindingElement = new TextMessageEncodingBindingElement();
            textBindingElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap11, AddressingVersion.None);
            result.Elements.Add(textBindingElement);
            if (ReportSettings.IsSSL)
            {
                HttpsTransportBindingElement httpBindingElement = new HttpsTransportBindingElement();
                httpBindingElement.AllowCookies = true;
                httpBindingElement.MaxBufferSize = int.MaxValue;
                httpBindingElement.MaxReceivedMessageSize = int.MaxValue;
                httpBindingElement.AuthenticationScheme = AuthenticationSchemes.Ntlm;                
                result.Elements.Add(httpBindingElement);
            }
            else
            {
                HttpTransportBindingElement httpBindingElement = new HttpTransportBindingElement();
                httpBindingElement.AllowCookies = true;
                httpBindingElement.MaxBufferSize = int.MaxValue;
                httpBindingElement.MaxReceivedMessageSize = int.MaxValue;
                httpBindingElement.AuthenticationScheme =AuthenticationSchemes.Ntlm;                
                result.Elements.Add(httpBindingElement);
            }
            return result;
        }
        private ReportExecutionServiceSoapClient ReportClient
        {
            get
            {
                if (client == null)
                {
                    client = new ReportExecutionServiceSoapClient(CreateBinding(), new EndpointAddress($"{ReportSettings.ReportServer}{ExecutionPath}"));
                    client.ClientCredentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Impersonation;
                    client.ClientCredentials.Windows.ClientCredential = (NetworkCredential)this.ReportSettings.Credential;                    
                    client.ExecutionHeader = new ExecutionHeader();
                    client.TrustedUserHeader = new TrustedUserHeader();

                    // added for .net core issue.
                    client.Endpoint.EndpointBehaviors.Add(new ReportEndpointBehavior(client));

                }
                return client;
            }
        }

        /*
        private ReportService2010.ReportingService2010SoapClient ServiceClient
        {
            get
            {
                return new AspNetCore.Report.ReportService2010.ReportingService2010SoapClient(CreateBinding(), new EndpointAddress("http://localhost/ReportServer/ReportService2010.asmx"));
            }
        }
        */
        bool CheckSettings(out ReportException ex)
        {
            ex = new ReportException();
            if (string.IsNullOrEmpty(ReportSettings.ReportServer))
            {
                ex = new ReportException("Please set ReportServer.");
                return false;
            }
            if (!Uri.IsWellFormedUriString(ReportSettings.ReportServer, UriKind.Absolute))
            {
                ex = new ReportException("Please check whether the ReportServer is correct.");
                return false;
            }

            return true;
        }
        #endregion

    }
}
