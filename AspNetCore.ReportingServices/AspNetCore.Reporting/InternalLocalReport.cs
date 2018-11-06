#define TRACE
using AspNetCore.Reporting;
using AspNetCore.ReportingServices;
using AspNetCore.ReportingServices.DataExtensions;
using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.Interfaces;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using System.Linq;

using Html = AspNetCore.ReportingServices.Rendering.HtmlRenderer;

namespace AspNetCore.Reporting
{
    [Serializable]
    internal sealed class InternalLocalReport : Report, ISerializable, IDisposable
    {
        private const string TopLevelDirectReportDefinitionPath = "";

        private string m_reportPath;

        private string m_reportEmbeddedResource;

        private Assembly m_embeddedResourceAssembly;

        private bool m_enableHyperlinks;

        private bool m_enableExternalImages;

        private NameValueCollection m_parentSuppliedParameters;

        private ReportDataSourceCollection m_dataSources;

        private ProcessingMessageList m_lastRenderingWarnings;

        internal readonly ILocalProcessingHost m_processingHost;

        private RenderingExtension[] m_externalRenderingExtensions;

        [NonSerialized]
        private MapTileServerConfiguration m_mapTileServerConfiguration;

        internal override string DisplayNameForUse
        {
            get
            {
                lock (base.m_syncObject)
                {
                    if (string.IsNullOrEmpty(base.DisplayName))
                    {
                        PreviewItemContext itemContext = this.m_processingHost.ItemContext;
                        if (itemContext != null)
                        {
                            string text = itemContext.ItemName;
                            if (string.IsNullOrEmpty(text))
                            {
                                text = CommonStrings.Report;
                            }
                            return text;
                        }
                        return string.Empty;
                    }
                    return base.DisplayName;
                }
            }
        }

        internal bool SupportsQueries
        {
            get
            {
                return this.m_processingHost.SupportsQueries;
            }
        }

        internal override bool CanSelfCancel
        {
            get
            {
                return this.m_processingHost.CanSelfCancel;
            }
        }

        private DefinitionSource DefinitionSource
        {
            get
            {
                if (!string.IsNullOrEmpty(this.ReportPath))
                {
                    return DefinitionSource.File;
                }
                if (!string.IsNullOrEmpty(this.ReportEmbeddedResource))
                {
                    return DefinitionSource.EmbeddedResource;
                }
                if (this.m_processingHost.Catalog.HasDirectReportDefinition(""))
                {
                    return DefinitionSource.Direct;
                }
                return DefinitionSource.Unknown;
            }
        }

        [DefaultValue(null)]
        //[SRDescription("LocalReportPathDesc")]
        [NotifyParentProperty(true)]
        public string ReportPath
        {
            get
            {
                return this.m_reportPath;
            }
            set
            {
                this.DemandFullTrustWithFriendlyMessage();
                lock (base.m_syncObject)
                {
                    if (string.Compare(value, this.ReportPath, StringComparison.Ordinal) != 0)
                    {
                        this.ChangeReportDefinition(DefinitionSource.File, delegate
                        {
                            this.m_reportPath = value;
                        });
                    }
                }
            }
        }

        [DefaultValue(null)]
        //[SRDescription("ReportEmbeddedResourceDesc")]
        [TypeConverter("AspNetCore.ReportingServices.ReportSelectionConverter, AspNetCore.Reporting.Design, Version=14.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91")]
        [NotifyParentProperty(true)]
        public string ReportEmbeddedResource
        {
            get
            {
                return this.m_reportEmbeddedResource;
            }
            set
            {
                this.DemandFullTrustWithFriendlyMessage();
                lock (base.m_syncObject)
                {
                    if (string.Compare(value, this.ReportEmbeddedResource, StringComparison.Ordinal) != 0)
                    {
                        this.SetEmbeddedResourceAsReportDefinition(value, Assembly.GetCallingAssembly());
                    }
                }
            }
        }

        //[SRDescription("EnableExternalImagesDesc")]
        [DefaultValue(false)]
        [Category("Security")]
        [NotifyParentProperty(true)]
        public bool EnableExternalImages
        {
            get
            {
                return this.m_enableExternalImages;
            }
            set
            {
                lock (base.m_syncObject)
                {
                    this.m_enableExternalImages = value;
                }
            }
        }

        [DefaultValue(true)]
        [NotifyParentProperty(true)]
        //[SRDescription("ShowDetailedSubreportMessagesDesc")]
        public bool ShowDetailedSubreportMessages
        {
            get
            {
                return this.m_processingHost.ShowDetailedSubreportMessages;
            }
            set
            {
                lock (base.m_syncObject)
                {
                    if (this.m_processingHost.ShowDetailedSubreportMessages != value)
                    {
                        this.m_processingHost.ShowDetailedSubreportMessages = value;
                        base.OnChange(false);
                    }
                }
            }
        }

        [Category("Security")]
        [NotifyParentProperty(true)]
        [DefaultValue(false)]
        //[SRDescription("EnableHyperlinksDesc")]
        public bool EnableHyperlinks
        {
            get
            {
                return this.m_enableHyperlinks;
            }
            set
            {
                lock (base.m_syncObject)
                {
                    this.m_enableHyperlinks = value;
                }
            }
        }

        [NotifyParentProperty(true)]
        //[PersistenceMode(PersistenceMode.InnerProperty)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        //[SRDescription("ReportDataSourcesDesc")]
        //[WebBrowsable(true)]
        public ReportDataSourceCollection DataSources
        {
            get
            {
                return this.m_dataSources;
            }
        }

        internal override bool IsReadyForConnection
        {
            get
            {
                return this.DefinitionSource != DefinitionSource.Unknown;
            }
        }

        internal override bool IsPreparedReportReadyForRendering
        {
            get
            {
                foreach (string dataSourceName in this.GetDataSourceNames())
                {
                    if (this.DataSources[dataSourceName] == null)
                    {
                        return false;
                    }
                }
                bool flag = default(bool);
                this.GetDataSources(out flag);
                if (!flag)
                {
                    return false;
                }
                foreach (ReportParameterInfo parameter in this.GetParameters())
                {
                    if (parameter.State != 0)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        internal bool HasExecutionSession
        {
            get
            {
                return this.m_processingHost.ExecutionInfo.IsCompiled;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public IList<ReportParameter> OriginalParametersToDrillthrough
        {
            get
            {
                ReportParameter[] list = ReportParameter.FromNameValueCollection(this.m_parentSuppliedParameters);
                return new ReadOnlyCollection<ReportParameter>(list);
            }
        }

        internal override bool HasDocMap
        {
            get
            {
                try
                {
                    lock (base.m_syncObject)
                    {
                        return this.m_processingHost.ExecutionInfo.HasDocMap;
                    }
                }
                catch (SecurityException processingException)
                {
                    throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException);
                }
            }
        }

        internal override int AutoRefreshInterval
        {
            get
            {
                lock (base.m_syncObject)
                {
                    return this.m_processingHost.ExecutionInfo.AutoRefreshInterval;
                }
            }
        }

        internal event InitializeDataSourcesEventHandler InitializeDataSources;

        //[SRDescription("SubreportProcessingEventDesc")]
        public event SubreportProcessingEventHandler SubreportProcessing;
        public InternalLocalReport() : this(new ControlService(new StandalonePreviewStore()))
        {
        }
        internal InternalLocalReport(ILocalProcessingHost processingHost)
        {
            this.m_processingHost = processingHost;
            this.m_dataSources = new ReportDataSourceCollection(base.m_syncObject);
            this.Construct();
        }

        [SecurityCritical]
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        internal InternalLocalReport(SerializationInfo info, StreamingContext context)
        {
            base.DisplayName = info.GetString("DisplayName");
            this.m_reportPath = info.GetString("ReportPath");
            this.m_reportEmbeddedResource = info.GetString("ReportEmbeddedResource");
            this.m_embeddedResourceAssembly = (Assembly)info.GetValue("EmbeddedResourceAssembly", typeof(Assembly));
            this.m_dataSources = (ReportDataSourceCollection)info.GetValue("DataSources", typeof(ReportDataSourceCollection));
            this.m_dataSources.SetSyncObject(base.m_syncObject);
            this.m_processingHost = (ILocalProcessingHost)info.GetValue("ControlService", typeof(ILocalProcessingHost));
            base.DrillthroughDepth = info.GetInt32("DrillthroughDepth");
            this.m_enableExternalImages = info.GetBoolean("EnableExternalImages");
            this.m_enableHyperlinks = info.GetBoolean("EnableHyperlinks");
            this.m_parentSuppliedParameters = (NameValueCollection)info.GetValue("ParentSuppliedParameters", typeof(NameValueCollection));
            this.Construct();
        }

        [SecurityCritical]
        [SecurityTreatAsSafe]
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("DisplayName", base.DisplayName);
            info.AddValue("ReportPath", this.m_reportPath);
            info.AddValue("ReportEmbeddedResource", this.m_reportEmbeddedResource);
            info.AddValue("EmbeddedResourceAssembly", this.m_embeddedResourceAssembly);
            info.AddValue("DataSources", this.m_dataSources);
            info.AddValue("ControlService", this.m_processingHost);
            info.AddValue("DrillthroughDepth", base.DrillthroughDepth);
            info.AddValue("EnableExternalImages", this.m_enableExternalImages);
            info.AddValue("EnableHyperlinks", this.m_enableHyperlinks);
            info.AddValue("ParentSuppliedParameters", this.m_parentSuppliedParameters);
        }

        private void Construct()
        {
            LocalService localService = this.m_processingHost as LocalService;
            if (localService != null)
            {
                localService.DataRetrieval = this.CreateDataRetrieval();
                localService.SecurityValidator = this.ValidateReportSecurity;
            }
            this.DataSources.Change += base.OnChange;
            base.Change += this.OnLocalReportChange;
            if (this.m_processingHost.MapTileServerConfiguration != null)
            {
                this.m_mapTileServerConfiguration = new MapTileServerConfiguration(this.m_processingHost.MapTileServerConfiguration);
            }
        }

        public void Dispose()
        {
            IDisposable disposable = this.m_processingHost as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        internal override void SetCancelState(bool shouldCancelRequests)
        {
            this.m_processingHost.SetCancelState(shouldCancelRequests);
        }

        private void DemandFullTrustWithFriendlyMessage()
        {
            try
            {
                SecurityPermission securityPermission = new SecurityPermission(PermissionState.Unrestricted);
                securityPermission.Demand();
            }
            catch (SecurityException)
            {
                throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors);
            }
        }

        private void SetEmbeddedResourceAsReportDefinition(string resourceName, Assembly assemblyWithResource)
        {
            this.ChangeReportDefinition(DefinitionSource.EmbeddedResource, delegate
            {
                if (string.IsNullOrEmpty(resourceName))
                {
                    assemblyWithResource = null;
                }
                this.m_reportEmbeddedResource = resourceName;
                this.m_embeddedResourceAssembly = assemblyWithResource;
            });
        }

        internal void SetDataSourceCredentials(IEnumerable credentials)
        {
            if (credentials == null)
            {
                throw new ArgumentNullException("credentials");
            }
            lock (base.m_syncObject)
            {
                this.EnsureExecutionSession();
                List<DatasourceCredentials> list = new List<DatasourceCredentials>();
                foreach (DataSourceCredentials credential in credentials)
                {
                    list.Add(new DatasourceCredentials(credential.Name, credential.UserId, credential.Password));
                }
                this.m_processingHost.SetReportDataSourceCredentials(list.ToArray());
                base.OnChange(false);
            }
        }

        internal ReportDataSourceInfoCollection GetDataSources(out bool allCredentialsSatisfied)
        {
            lock (base.m_syncObject)
            {
                this.EnsureExecutionSession();
                DataSourcePromptCollection reportDataSourcePrompts;
                try
                {
                    reportDataSourcePrompts = this.m_processingHost.GetReportDataSourcePrompts(out allCredentialsSatisfied);
                }
                catch (Exception processingException)
                {
                    throw this.WrapProcessingException(processingException);
                }
                List<ReportDataSourceInfo> list = new List<ReportDataSourceInfo>(reportDataSourcePrompts.Count);
                foreach (DataSourceInfo item in reportDataSourcePrompts)
                {
                    list.Add(new ReportDataSourceInfo(item.PromptIdentifier, item.Prompt));
                }
                return new ReportDataSourceInfoCollection(list);
            }
        }

        public IList<string> GetDataSourceNames()
        {
            lock (base.m_syncObject)
            {
                this.EnsureExecutionSession();
                string[] dataSetNames;
                try
                {
                    dataSetNames = this.m_processingHost.GetDataSetNames(null);
                }
                catch (Exception processingException)
                {
                    throw this.WrapProcessingException(processingException);
                }
                return new ReadOnlyCollection<string>(dataSetNames);
            }
        }

        public override int GetTotalPages(out PageCountMode pageCountMode)
        {
            lock (base.m_syncObject)
            {
                LocalExecutionInfo executionInfo = this.m_processingHost.ExecutionInfo;
                if (executionInfo.PaginationMode == PaginationMode.TotalPages)
                {
                    pageCountMode = PageCountMode.Actual;
                }
                else
                {
                    pageCountMode = PageCountMode.Estimate;
                }
                return executionInfo.TotalPages;
            }
        }

        public override void LoadReportDefinition(TextReader report)
        {
            lock (base.m_syncObject)
            {
                if (report == null)
                {
                    throw new ArgumentNullException("report");
                }
                this.SetDirectReportDefinition("", report);
            }
        }

        public void LoadSubreportDefinition(string reportName, TextReader report)
        {
            lock (base.m_syncObject)
            {
                if (reportName == null)
                {
                    throw new ArgumentNullException("reportName");
                }
                if (reportName.Length == 0)
                {
                    throw new ArgumentOutOfRangeException("reportName");
                }
                if (report == null)
                {
                    throw new ArgumentNullException("report");
                }
                this.SetDirectReportDefinition(reportName, report);
            }
        }

        public void LoadSubreportDefinition(string reportName, Stream report)
        {
            if (report == null)
            {
                throw new ArgumentNullException("report");
            }
            this.LoadSubreportDefinition(reportName, new StreamReader(report));
        }

        private void SetDirectReportDefinition(string reportName, TextReader report)
        {
            this.DemandFullTrustWithFriendlyMessage();
            string s = report.ReadToEnd();
            byte[] reportBytes = Encoding.UTF8.GetBytes(s);
            this.ChangeReportDefinition(DefinitionSource.Direct, delegate
            {
                this.m_processingHost.Catalog.SetReportDefinition(reportName, reportBytes);
            });
        }

        internal override int PerformSearch(string searchText, int startPage, int endPage)
        {
            try
            {
                lock (base.m_syncObject)
                {
                    if (!this.m_processingHost.ExecutionInfo.HasSnapshot)
                    {
                        throw new InvalidOperationException(CommonStrings.ReportNotReady);
                    }
                    if (string.IsNullOrEmpty(searchText))
                    {
                        return -1;
                    }
                    try
                    {
                        return this.m_processingHost.PerformSearch(startPage, endPage, searchText);
                    }
                    catch (Exception processingException)
                    {
                        throw this.WrapProcessingException(processingException);
                    }
                }
            }
            catch (SecurityException processingException2)
            {
                throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException2);
            }
        }

        internal override void PerformToggle(string toggleId)
        {
            try
            {
                lock (base.m_syncObject)
                {
                    if (!this.m_processingHost.ExecutionInfo.HasSnapshot)
                    {
                        throw new InvalidOperationException(CommonStrings.ReportNotReady);
                    }
                    try
                    {
                        this.m_processingHost.PerformToggle(toggleId);
                    }
                    catch (Exception processingException)
                    {
                        throw this.WrapProcessingException(processingException);
                    }
                }
            }
            catch (SecurityException processingException2)
            {
                throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException2);
            }
        }

        internal override int PerformBookmarkNavigation(string bookmarkId, out string uniqueName)
        {
            try
            {
                lock (base.m_syncObject)
                {
                    if (!this.m_processingHost.ExecutionInfo.HasSnapshot)
                    {
                        throw new InvalidOperationException(CommonStrings.ReportNotReady);
                    }
                    try
                    {
                        return this.m_processingHost.PerformBookmarkNavigation(bookmarkId, out uniqueName);
                    }
                    catch (Exception processingException)
                    {
                        throw this.WrapProcessingException(processingException);
                    }
                }
            }
            catch (SecurityException processingException2)
            {
                throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException2);
            }
        }

        internal override int PerformDocumentMapNavigation(string documentMapId)
        {
            try
            {
                lock (base.m_syncObject)
                {
                    if (!this.m_processingHost.ExecutionInfo.HasSnapshot)
                    {
                        throw new InvalidOperationException(CommonStrings.ReportNotReady);
                    }
                    try
                    {
                        return this.m_processingHost.PerformDocumentMapNavigation(documentMapId);
                    }
                    catch (Exception processingException)
                    {
                        throw this.WrapProcessingException(processingException);
                    }
                }
            }
            catch (SecurityException processingException2)
            {
                throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException2);
            }
        }

        internal override Report PerformDrillthrough(string drillthroughId, out string reportPath)
        {
            try
            {
                lock (base.m_syncObject)
                {
                    if (!this.m_processingHost.ExecutionInfo.HasSnapshot)
                    {
                        throw new InvalidOperationException(CommonStrings.ReportNotReady);
                    }
                    if (drillthroughId == null)
                    {
                        throw new ArgumentNullException("drillthroughId");
                    }
                    NameValueCollection drillParams = default(NameValueCollection);
                    try
                    {
                        reportPath = this.m_processingHost.PerformDrillthrough(drillthroughId, out drillParams);
                    }
                    catch (Exception processingException)
                    {
                        throw this.WrapProcessingException(processingException);
                    }
                    InternalLocalReport localReport = this.CreateNewLocalReport();
                    PreviewItemContext previewItemContext = this.CreateItemContext();
                    string reportPath2 = previewItemContext.MapUserProvidedPath(reportPath);
                    this.PopulateDrillthroughReport(reportPath2, drillParams, localReport);
                    return localReport;
                }
            }
            catch (SecurityException processingException2)
            {
                throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException2);
            }
        }

        public override ReportPageSettings GetDefaultPageSettings()
        {
            lock (base.m_syncObject)
            {
                this.EnsureExecutionSession();
                PageProperties pageProperties = this.m_processingHost.ExecutionInfo.PageProperties;
                return new ReportPageSettings(pageProperties.PageHeight, pageProperties.PageWidth, pageProperties.LeftMargin, pageProperties.RightMargin, pageProperties.TopMargin, pageProperties.BottomMargin);
            }
        }

        private void PopulateDrillthroughReport(string reportPath, NameValueCollection drillParams, InternalLocalReport drillReport)
        {
            drillReport.CopySecuritySettings(this);
            if (this.ReportPath != null)
            {
                drillReport.ReportPath = reportPath;
            }
            else if (this.ReportEmbeddedResource != null)
            {
                drillReport.SetEmbeddedResourceAsReportDefinition(reportPath, this.m_embeddedResourceAssembly);
            }
            drillReport.DrillthroughDepth = base.DrillthroughDepth + 1;
            drillReport.m_parentSuppliedParameters = drillParams;
        }

        internal override int PerformSort(string sortId, SortOrder sortDirection, bool clearSort, PageCountMode pageCountMode, out string uniqueName)
        {
            try
            {
                lock (base.m_syncObject)
                {
                    if (!this.m_processingHost.ExecutionInfo.HasSnapshot)
                    {
                        throw new InvalidOperationException(CommonStrings.ReportNotReady);
                    }
                    SortOptions sortOptions = (SortOptions)((sortDirection == SortOrder.Ascending) ? 1 : 2);
                    string paginationMode = InternalLocalReport.PageCountModeToProcessingPaginationMode(pageCountMode);
                    try
                    {
                        return this.m_processingHost.PerformSort(paginationMode, sortId, sortOptions, clearSort, out uniqueName);
                    }
                    catch (Exception processingException)
                    {
                        throw this.WrapProcessingException(processingException);
                    }
                }
            }
            catch (SecurityException processingException2)
            {
                throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException2);
            }
        }

        [Obsolete("This method requires Code Access Security policy, which is deprecated.  For more information please go to http://go.microsoft.com/fwlink/?LinkId=160787.")]
        public void ExecuteReportInCurrentAppDomain(Evidence reportEvidence)
        {
            try
            {
                lock (base.m_syncObject)
                {
                    this.m_processingHost.ExecuteReportInCurrentAppDomain(reportEvidence);
                }
            }
            catch (SecurityException processingException)
            {
                throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException);
            }
        }

        [Obsolete("This method requires Code Access Security policy, which is deprecated.  For more information please go to http://go.microsoft.com/fwlink/?LinkId=160787.")]
        public void AddTrustedCodeModuleInCurrentAppDomain(string assemblyName)
        {
            try
            {
                lock (base.m_syncObject)
                {
                    this.m_processingHost.AddTrustedCodeModuleInCurrentAppDomain(assemblyName);
                }
            }
            catch (SecurityException processingException)
            {
                throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException);
            }
        }

        [Obsolete("This method requires Code Access Security policy, which is deprecated.  For more information please go to http://go.microsoft.com/fwlink/?LinkId=160787.")]
        public void ExecuteReportInSandboxAppDomain()
        {
            try
            {
                lock (base.m_syncObject)
                {
                    this.m_processingHost.ExecuteReportInSandboxAppDomain();
                }
            }
            catch (SecurityException processingException)
            {
                throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException);
            }
        }

        public void AddFullTrustModuleInSandboxAppDomain(StrongName assemblyName)
        {
            try
            {
                lock (base.m_syncObject)
                {
                    this.m_processingHost.AddFullTrustModuleInSandboxAppDomain(assemblyName);
                }
            }
            catch (SecurityException processingException)
            {
                throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException);
            }
        }

        public void SetBasePermissionsForSandboxAppDomain(PermissionSet permissions)
        {
            try
            {
                lock (base.m_syncObject)
                {
                    this.m_processingHost.SetBasePermissionsForSandboxAppDomain(permissions);
                }
            }
            catch (SecurityException processingException)
            {
                throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException);
            }
        }

        public void ReleaseSandboxAppDomain()
        {
            lock (base.m_syncObject)
            {
                this.m_processingHost.ReleaseSandboxAppDomain();
            }
        }

        private void CopySecuritySettings(InternalLocalReport parentReport)
        {
            this.m_processingHost.CopySecuritySettingsFrom(parentReport.m_processingHost);
            this.m_enableExternalImages = parentReport.EnableExternalImages;
            this.m_enableHyperlinks = parentReport.EnableHyperlinks;
            this.ShowDetailedSubreportMessages = parentReport.ShowDetailedSubreportMessages;
        }

        internal override void EnsureExecutionSession()
        {
            if (this.DefinitionSource == DefinitionSource.Unknown)
            {
                throw new Exception("MissingReportSourceException");
            }
            try
            {
                if (!this.HasExecutionSession)
                {
                    this.m_processingHost.CompileReport();
                    this.m_processingHost.SetReportParameters(this.m_parentSuppliedParameters);
                }
            }
            catch (SecurityException processingException)
            {
                throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException);
            }
            catch (Exception processingException2)
            {
                throw this.WrapProcessingException(processingException2);
            }
        }

        private void ValidateReportSecurity(PreviewItemContext itemContext, PublishingResult publishingResult)
        {
            if (publishingResult.HasExternalImages && !this.EnableExternalImages)
            {
                throw new ReportSecurityException(CommonStrings.ExternalImagesError(itemContext.ItemName));
            }
            if (!publishingResult.HasHyperlinks)
            {
                return;
            }
            if (this.EnableHyperlinks)
            {
                return;
            }
            throw new ReportSecurityException(CommonStrings.HyperlinkSecurityError(itemContext.ItemName));
        }

        public override void Refresh()
        {
            try
            {
                lock (base.m_syncObject)
                {
                    if (this.m_processingHost.ExecutionInfo.HasSnapshot)
                    {
                        this.m_processingHost.ResetExecution();
                        base.OnChange(true);
                    }
                }
            }
            catch (SecurityException processingException)
            {
                throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException);
            }
        }

        private void ChangeReportDefinition(DefinitionSource updatingSourceType, System.Action changeAction)
        {
            DefinitionSource definitionSource = this.DefinitionSource;
            changeAction();
            DefinitionSource definitionSource2 = this.DefinitionSource;
            if (definitionSource2 != updatingSourceType && definitionSource2 == definitionSource)
            {
                return;
            }
            this.m_processingHost.ItemContext = this.CreateItemContext();
            base.OnChange(false);
        }

        public override ReportParameterInfoCollection GetParameters()
        {
            lock (base.m_syncObject)
            {
                this.EnsureExecutionSession();
                return InternalLocalReport.ParameterInfoCollectionToApi(this.m_processingHost.ExecutionInfo.ReportParameters, this.SupportsQueries);
            }
        }

        internal override ParametersPaneLayout GetParametersPaneLayout()
        {
            lock (base.m_syncObject)
            {
                this.EnsureExecutionSession();
                if (this.m_processingHost.ExecutionInfo.ReportParameters != null && this.m_processingHost.ExecutionInfo.ReportParameters.ParametersLayout != null)
                {
                    ReportParameterInfoCollection paramsInfo = InternalLocalReport.ParameterInfoCollectionToApi(this.m_processingHost.ExecutionInfo.ReportParameters, this.SupportsQueries);
                    return this.BuildParameterPaneLayout(this.m_processingHost.ExecutionInfo.ReportParameters.ParametersLayout, paramsInfo);
                }
                return null;
            }
        }

        private ParametersPaneLayout BuildParameterPaneLayout(ParametersGridLayout processingParameterLayout, ReportParameterInfoCollection paramsInfo)
        {
            List<GridLayoutCellDefinition> list = new List<GridLayoutCellDefinition>(processingParameterLayout.CellDefinitions.Count);
            foreach (object cellDefinition in processingParameterLayout.CellDefinitions)
            {
                ParameterGridLayoutCellDefinition parameterGridLayoutCellDefinition = (ParameterGridLayoutCellDefinition)cellDefinition;
                list.Add(new GridLayoutCellDefinition
                {
                    Column = parameterGridLayoutCellDefinition.ColumnIndex,
                    Row = parameterGridLayoutCellDefinition.RowIndex,
                    ParameterName = parameterGridLayoutCellDefinition.ParameterName
                });
            }
            ParametersPaneLayout parametersPaneLayout = new ParametersPaneLayout();
            parametersPaneLayout.GridLayoutDefinition = new GridLayoutDefinition(new GridLayoutCellDefinitionCollection(list), processingParameterLayout.NumberOfRows, processingParameterLayout.NumberOfColumns, paramsInfo);
            return parametersPaneLayout;
        }

        public override void SetParameters(IEnumerable<ReportParameter> parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }
            lock (base.m_syncObject)
            {
                this.EnsureExecutionSession();
                NameValueCollection reportParameters = ReportParameter.ToNameValueCollection(parameters);
                try
                {
                    this.m_processingHost.SetReportParameters(reportParameters);
                }
                catch (Exception processingException)
                {
                    throw this.WrapProcessingException(processingException);
                }
                base.OnChange(false);
            }
        }

        internal override byte[] InternalRenderStream(string format, string streamID, string deviceInfo, out string mimeType, out Encoding encoding)
        {
            try
            {
                encoding = null;
                lock (base.m_syncObject)
                {
                    return this.m_processingHost.RenderStream(format, deviceInfo, streamID, out mimeType);
                }
            }
            catch (SecurityException processingException)
            {
                throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException);
            }
            catch (Exception processingException2)
            {
                throw this.WrapProcessingException(processingException2);
            }
        }

        internal override void InternalDeliverReportItem(string format, string deviceInfo, ExtensionSettings settings, string description, string eventType, string matchData)
        {
            throw new NotImplementedException();
        }

        internal override DocumentMapNode GetDocumentMap(string rootLabel)
        {
            try
            {
                lock (base.m_syncObject)
                {
                    if (!this.m_processingHost.ExecutionInfo.HasSnapshot)
                    {
                        throw new InvalidOperationException(CommonStrings.ReportNotReady);
                    }
                    IDocumentMap documentMap;
                    try
                    {
                        documentMap = this.m_processingHost.GetDocumentMap();
                    }
                    catch (Exception processingException)
                    {
                        throw this.WrapProcessingException(processingException);
                    }
                    return DocumentMapNode.CreateTree(documentMap, rootLabel);
                }
            }
            catch (SecurityException processingException2)
            {
                throw new LocalProcessingException(CommonStrings.LocalModeMissingFullTrustErrors, processingException2);
            }
        }

        public override byte[] Render(string format, string deviceInfo, PageCountMode pageCountMode, out string mimeType, out Encoding encoding, out string fileNameExtension, out string[] streams, out Warning[] warnings)
        {
            return this.InternalRender(format, false, deviceInfo, pageCountMode, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
        }
        public int PageIndex{get;internal set;}
        public byte[] Styles { get;private set; }
        internal byte[] InternalRender(string format, bool allowInternalRenderers, string deviceInfo, PageCountMode pageCountMode, out string mimeType, out Encoding encoding, out string fileNameExtension, out string[] streams, out Warning[] warnings)
		{
			lock (base.m_syncObject)
			{
				using (AspNetCore.Reporting.StreamCache streamCache = new AspNetCore.Reporting.StreamCache())
				{
					this.InternalRender(format, allowInternalRenderers, deviceInfo, pageCountMode, (CreateAndRegisterStream)streamCache.StreamCallback, out warnings);
					streams = new string[0];
                    if (string.Equals("html", format, StringComparison.OrdinalIgnoreCase))
                    {
                        string styleStreamName = LocalHtmlRenderer.GetStyleStreamName(PageIndex);

                        //var bytes1 = Encoding.UTF8.GetBytes("<style type='text/css'>");
                        this.Styles= streamCache.GetSecondaryStream(false, styleStreamName, out encoding, out mimeType, out fileNameExtension);
                        //var bytes3 = Encoding.UTF8.GetBytes("</style>");
                        //var bytes4 = streamCache.GetMainStream(out encoding, out mimeType, out fileNameExtension);
                        //List<byte> bytes = new List<byte>();
                        //bytes.AddRange(bytes1);
                        //bytes.AddRange(bytes2);
                        //bytes.AddRange(bytes3);
                        //bytes.AddRange(bytes4);
                        //return bytes.ToArray();
                    }
                    else {
                        this.Styles = new byte[0];
                    }
                    return streamCache.GetMainStream(out encoding, out mimeType, out fileNameExtension);
				}
			}
		}

		internal void Render(string format, string deviceInfo, CreateStreamCallback createStream, out Warning[] warnings)
		{
			this.Render(format, deviceInfo, PageCountMode.Estimate, createStream, out warnings);
		}

		internal void Render(string format, string deviceInfo, PageCountMode pageCountMode, CreateStreamCallback createStream, out Warning[] warnings)
		{
			if (createStream == null)
			{
				throw new ArgumentNullException("createStream");
			}
			CreateAndRegisterStream createStreamCallback = (string name, string extension, Encoding encoding, string mimeType, bool willSeek,StreamOper operation) => createStream(name, extension, encoding, mimeType, willSeek);
			using (ProcessingStreamHandler @object = new ProcessingStreamHandler(createStreamCallback))
			{
				this.InternalRender(format, false, deviceInfo, pageCountMode, ((CreateAndRegisterStream)new CreateAndRegisterStream(@object.StreamCallback)).ToInnerType(), out warnings);
			}
		}

		internal void InternalRender(string format, bool allowInternalRenderers, string deviceInfo, PageCountMode pageCountMode, CreateAndRegisterStream createStreamCallback, out Warning[] warnings)
		{
			lock (base.m_syncObject)
			{
                warnings = null;
                if (createStreamCallback == null)
				{
					throw new ArgumentNullException("createStreamCallback");
				}
                if (string.Equals("html", format, StringComparison.OrdinalIgnoreCase))
                {
                    LocalModeSession session = new LocalModeSession(this);
                    session.RenderReportHTML(deviceInfo, pageCountMode, createStreamCallback, out string scrollScript, out string pageStyle);
                    return;
                }
                if (!this.ValidateRenderingFormat(format))
				{
					throw new ArgumentOutOfRangeException("format");
				}
                
                this.EnsureExecutionSession();
				try
				{
					this.m_lastRenderingWarnings = this.m_processingHost.Render(format, deviceInfo, InternalLocalReport.PageCountModeToProcessingPaginationMode(pageCountMode), allowInternalRenderers, this.m_dataSources, (CreateAndRegisterStream)createStreamCallback.ToOuterType());
				}
				catch (Exception processingException)
				{
					throw this.WrapProcessingException(processingException);
				}
				warnings = Warning.FromProcessingMessageList(this.m_lastRenderingWarnings);
				this.WriteDebugResults(warnings);
			}
		}

		private void WriteDebugResults(Warning[] warnings)
		{
			foreach (Warning warning in warnings)
			{
				string message = string.Format(CultureInfo.InvariantCulture, "{0}: {1} ({2})", warning.Severity, warning.Message, warning.Code);
				Trace.WriteLine(message);
			}
		}

		private bool ValidateRenderingFormat(string format)
		{
			try
			{
				foreach (LocalRenderingExtensionInfo item in this.m_processingHost.ListRenderingExtensions())
				{
					if (string.Compare(item.Name, format, StringComparison.OrdinalIgnoreCase) == 0)
					{
						return true;
					}
				}
				return false;
			}
			catch (Exception processingException)
			{
				throw this.WrapProcessingException(processingException);
			}
		}

		internal void TransferEvents(InternalLocalReport targetReport)
		{
			if (targetReport != null)
			{
				targetReport.SubreportProcessing = this.SubreportProcessing;
			}
			this.SubreportProcessing = null;
		}

		internal void CreateSnapshot()
		{
			this.m_processingHost.Render(null, null, null, false, this.m_dataSources, null);
		}

		private IEnumerable ControlSubReportInfoCallback(PreviewItemContext subReportContext, ParameterInfoCollection initialParameters)
		{
			if (this.SubreportProcessing == null)
			{
				return null;
			}
			string[] dataSetNames;
			try
			{
				dataSetNames = this.m_processingHost.GetDataSetNames(subReportContext);
			}
			catch (Exception processingException)
			{
				throw this.WrapProcessingException(processingException);
			}
			SubreportProcessingEventArgs subreportProcessingEventArgs = new SubreportProcessingEventArgs(subReportContext.OriginalItemPath, InternalLocalReport.ParameterInfoCollectionToApi(initialParameters, this.SupportsQueries), dataSetNames);
			this.SubreportProcessing(this, subreportProcessingEventArgs);
			if (this.InitializeDataSources != null)
			{
				InitializeDataSourcesEventArgs e = new InitializeDataSourcesEventArgs(subreportProcessingEventArgs.DataSources);
				this.InitializeDataSources(this, e);
			}
			return subreportProcessingEventArgs.DataSources;
		}

		public override RenderingExtension[] ListRenderingExtensions()
		{
			if (this.m_externalRenderingExtensions == null)
			{
				List<RenderingExtension> list = new List<RenderingExtension>();
				try
				{
					foreach (LocalRenderingExtensionInfo item in this.m_processingHost.ListRenderingExtensions())
					{
						if (item.IsExposedExternally)
						{
							list.Add(new RenderingExtension(item.Name, item.LocalizedName, item.IsVisible));
						}
					}
				}
				catch (Exception processingException)
				{
					throw this.WrapProcessingException(processingException);
				}
				this.m_externalRenderingExtensions = list.ToArray();
			}
			return this.m_externalRenderingExtensions;
		}

		private string GetFullyQualifiedReportPath()
		{
			switch (this.DefinitionSource)
			{
			case DefinitionSource.File:
				return InternalLocalReport.GetReportNameForFile(this.ReportPath);
			case DefinitionSource.EmbeddedResource:
				return this.ReportEmbeddedResource;
			case DefinitionSource.Direct:
				return "";
			default:
				return string.Empty;
			}
		}

		private static string GetReportNameForFile(string path)
		{
			if (Path.IsPathRooted(path))
			{
				return path;
			}
			string path2=string.Empty;
            //todo: HttpContext;
            /*
			if (HttpContext.Current != null && HttpContext.Current.Request != null)
			{
				HttpRequest request = HttpContext.Current.Request;
				path2 = request.MapPath(request.ApplicationPath);
			}
			else
			{
				path2 = Environment.CurrentDirectory;
			}
            */
			return Path.Combine(path2, path);
		}

		private PreviewItemContext CreateItemContext()
		{
			return InternalLocalReport.CreateItemContext(this.ReportPath, this.GetFullyQualifiedReportPath(), this.DefinitionSource, this.m_embeddedResourceAssembly);
		}

		internal static PreviewItemContext CreateItemContextForFilePath(string filePath)
		{
			return InternalLocalReport.CreateItemContext(filePath, InternalLocalReport.GetReportNameForFile(filePath), DefinitionSource.File, null);
		}

		private static PreviewItemContext CreateItemContext(string pathForFileDefinitionSource, string fullyQualifiedPath, DefinitionSource definitionSource, Assembly embeddedResourceAssembly)
		{
			if (definitionSource == DefinitionSource.Unknown)
			{
				return null;
			}
			PreviewItemContext previewItemContext = InternalLocalReport.InstantiatePreviewItemContext();
			previewItemContext.SetPath(pathForFileDefinitionSource, fullyQualifiedPath, definitionSource, embeddedResourceAssembly);
			return previewItemContext;
		}

		private LocalProcessingException WrapProcessingException(Exception processingException)
		{
			Exception ex = processingException;
			while (true)
			{
				if (ex == null)
				{
					break;
				}
				if (ex.InnerException == null)
				{
					break;
				}
				if (!(ex is ReportRenderingException) && !(ex is UnhandledReportRenderingException) && !(ex is HandledReportRenderingException))
				{
					break;
				}
				ex = ex.InnerException;
			}
			LocalProcessingException ex2 = ex as LocalProcessingException;
			if (ex2 != null)
			{
				return ex2;
			}
			return new LocalProcessingException(ex);
		}

		private static string PageCountModeToProcessingPaginationMode(PageCountMode pageCountMode)
		{
			if (pageCountMode == PageCountMode.Actual)
			{
				return "Actual";
			}
			return "Estimate";
		}

		private static ReportParameterInfoCollection ParameterInfoCollectionToApi(ParameterInfoCollection processingMetadata, bool supportsQueries)
		{
			if (processingMetadata == null)
			{
				return new ReportParameterInfoCollection();
			}
			ReportParameterInfo[] array = new ReportParameterInfo[processingMetadata.Count];
			for (int i = 0; i < processingMetadata.Count; i++)
			{
				array[i] = InternalLocalReport.ParameterInfoToApi(processingMetadata[i], supportsQueries);
			}
			return new ReportParameterInfoCollection(array);
		}

		private static ReportParameterInfo ParameterInfoToApi(AspNetCore.ReportingServices.ReportProcessing.ParameterInfo paramInfo, bool supportsQueries)
		{
			string[] array = null;
			if (paramInfo.DependencyList != null)
			{
				array = new string[paramInfo.DependencyList.Count];
				for (int i = 0; i < paramInfo.DependencyList.Count; i++)
				{
					array[i] = paramInfo.DependencyList[i].Name;
				}
			}
			string[] array2 = null;
			if (paramInfo.Values != null)
			{
				array2 = new string[paramInfo.Values.Length];
				for (int j = 0; j < paramInfo.Values.Length; j++)
				{
					array2[j] = paramInfo.CastToString(paramInfo.Values[j], CultureInfo.CurrentCulture);
				}
			}
			List<ValidValue> list = null;
			if (paramInfo.ValidValues != null)
			{
				list = new List<ValidValue>(paramInfo.ValidValues.Count);
				foreach (AspNetCore.ReportingServices.ReportProcessing.ValidValue validValue in paramInfo.ValidValues)
				{
					string value = paramInfo.CastToString(validValue.Value, CultureInfo.CurrentCulture);
					list.Add(new ValidValue(validValue.Label, value));
				}
			}
			ParameterState state;
			switch (paramInfo.State)
			{
			case ReportParameterState.HasValidValue:
				state = ParameterState.HasValidValue;
				break;
			case ReportParameterState.InvalidValueProvided:
			case ReportParameterState.DefaultValueInvalid:
			case ReportParameterState.MissingValidValue:
				state = ParameterState.MissingValidValue;
				break;
			case ReportParameterState.HasOutstandingDependencies:
				state = ParameterState.HasOutstandingDependencies;
				break;
			case ReportParameterState.DynamicValuesUnavailable:
				state = ParameterState.DynamicValuesUnavailable;
				break;
			default:
				state = ParameterState.MissingValidValue;
				break;
			}
			return new ReportParameterInfo(paramInfo.Name, (ParameterDataType)Enum.Parse(typeof(ParameterDataType), paramInfo.DataType.ToString()), paramInfo.Nullable, paramInfo.AllowBlank, paramInfo.MultiValue, supportsQueries && paramInfo.UsedInQuery, paramInfo.Prompt, paramInfo.PromptUser, supportsQueries && paramInfo.DynamicDefaultValue, supportsQueries && paramInfo.DynamicValidValues, null, array2, list, array, state);
		}

		private void OnLocalReportChange(object sender, EventArgs e)
		{
			this.m_processingHost.ResetExecution();
		}



		private InternalLocalReport CreateNewLocalReport()
		{
			return new InternalLocalReport();
		}

		private LocalDataRetrieval CreateDataRetrieval()
		{
			LocalDataRetrievalFromDataSet localDataRetrievalFromDataSet = new LocalDataRetrievalFromDataSet();
			localDataRetrievalFromDataSet.SubReportDataSetCallback = this.ControlSubReportInfoCallback;
			return localDataRetrievalFromDataSet;
		}

		private static PreviewItemContext InstantiatePreviewItemContext()
		{
			return new PreviewItemContext();
		}
	}
}
