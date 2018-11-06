using System;
using System.Collections.Specialized;
using System.Web;

namespace AspNetCore.Reporting
{
    internal abstract class ViewerDataOperation : HandlerOperation
    {
        public ViewerDataOperation(IReportServerConnectionProvider connectionProvider)
        {
            if (connectionProvider == null)
            {
                throw new ArgumentNullException("connectionProvider");
            }
            this.m_connectionProvider = connectionProvider;
            this.m_processingMode = ProcessingMode.Local;
            /*
            this.m_instanceID = HandlerOperation.GetAndEnsureParam(HttpHandler.RequestParameters, "ControlID");
            if (HttpHandler.RequestParameters["Mode"] != null)
            {
               
            }
            */
            //ReportViewer reportViewer = this.CreateTempReportViewer();
            //this.m_isUsingSession = reportViewer.EnsureSessionOrConfig();
            if (this.m_isUsingSession)
            {
                //this.m_reportHierarchy = (ReportHierarchy)HttpContext.Current.Session[this.m_instanceID];
                if (this.m_reportHierarchy == null)
                {
                    throw new AspNetSessionExpiredException();
                }
            }
        }

        protected ProcessingMode ProcessingMode
        {
            get
            {
                return this.m_processingMode;
            }
        }

        protected string InstanceID
        {
            get
            {
                return this.m_instanceID;
            }
        }

        protected ReportHierarchy ReportHierarchy
        {
            get
            {
                return this.m_reportHierarchy;
            }
        }

        protected bool IsUsingSession
        {
            get
            {
                return this.m_isUsingSession;
            }
        }





        protected static string ViewerDataOperationQuery(Uri reportServerUri, string instanceID)
        {
            NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(string.Empty);
            nameValueCollection["ControlID"] = instanceID;
            if (reportServerUri != null)
            {
                nameValueCollection["RSProxy"] = reportServerUri.AbsoluteUri;
            }
            else
            {
                nameValueCollection["Mode"] = "true";
            }
            return "&" + nameValueCollection.ToString();
        }

        private const string ParamIsLocalMode = "Mode";

        private const string ParamControlID = "ControlID";

        private readonly IReportServerConnectionProvider m_connectionProvider;

        private ReportHierarchy m_reportHierarchy;

        private string m_instanceID;

        private bool m_isUsingSession;

        private ProcessingMode m_processingMode = ProcessingMode.Remote;
    }
}
