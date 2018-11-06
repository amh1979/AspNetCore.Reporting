using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer;
using Html=AspNetCore.ReportingServices.Rendering.HtmlRenderer;
using AspNetCore.ReportingServices.Rendering.ImageRenderer;
using AspNetCore.ReportingServices.Rendering.RPLRendering;
using AspNetCore.ReportingServices.Rendering.WordRenderer;
using AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer;
using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Policy;

namespace AspNetCore.Reporting
{
	[Serializable]
	internal sealed class ControlService : LocalService
	{
		[NonSerialized]
		private static List<LocalRenderingExtensionInfo> m_renderingExtensions;
        static ControlService()
        {
            if (m_renderingExtensions == null)
            {
                List<LocalRenderingExtensionInfo> list = new List<LocalRenderingExtensionInfo>();
                //Html.HTMLRenderer htmlRenderer = new Html.HTMLRenderer();
                //list.Add(new LocalRenderingExtensionInfo("HTML", htmlRenderer.LocalizedName, false, typeof(Html.HTMLRenderer), true));
                RPLRenderer rPLRenderer = new RPLRenderer();
                list.Add(new LocalRenderingExtensionInfo("RPL", rPLRenderer.LocalizedName, false, typeof(RPLRenderer), false));
                ExcelRenderer excelRenderer = new ExcelRenderer();
                list.Add(new LocalRenderingExtensionInfo("Excel", excelRenderer.LocalizedName, false, typeof(ExcelRenderer), true));
                ExcelOpenXmlRenderer excelOpenXmlRenderer = new ExcelOpenXmlRenderer();
                list.Add(new LocalRenderingExtensionInfo("EXCELOPENXML", excelOpenXmlRenderer.LocalizedName, true, typeof(ExcelOpenXmlRenderer), true));
                ImageRenderer imageRenderer = new ImageRenderer();
                list.Add(new LocalRenderingExtensionInfo("IMAGE", imageRenderer.LocalizedName, false, typeof(ImageRenderer), true));
                PDFRenderer pDFRenderer = new PDFRenderer();
                list.Add(new LocalRenderingExtensionInfo("PDF", pDFRenderer.LocalizedName, true, typeof(PDFRenderer), true));
                WordDocumentRenderer wordDocumentRenderer = new WordDocumentRenderer();
                list.Add(new LocalRenderingExtensionInfo("WORD", wordDocumentRenderer.LocalizedName, false, typeof(WordDocumentRenderer), true));
                WordOpenXmlDocumentRenderer wordOpenXmlDocumentRenderer = new WordOpenXmlDocumentRenderer();
                list.Add(new LocalRenderingExtensionInfo("WORDOPENXML", wordOpenXmlDocumentRenderer.LocalizedName, true, typeof(WordOpenXmlDocumentRenderer), true));
                m_renderingExtensions = list;
            }
        }

        private LocalProcessingHostMapTileServerConfiguration m_mapTileServerConfiguration;

		public override LocalProcessingHostMapTileServerConfiguration MapTileServerConfiguration
		{
			get
			{
				return this.m_mapTileServerConfiguration;
			}
		}

		private static Evidence InternetZoneEvidence
		{
			get
			{
				Evidence evidence = new Evidence();
				evidence.AddHostEvidence(new Zone(SecurityZone.Internet));
				return evidence;
			}
		}

		public ControlService(ILocalCatalog catalog)
			: base(catalog, ControlService.InternetZoneEvidence, new ControlPolicyManager())
		{
			this.m_mapTileServerConfiguration = new LocalProcessingHostMapTileServerConfiguration();
		}

        public override IEnumerable<LocalRenderingExtensionInfo> ListRenderingExtensions()
        {
            return m_renderingExtensions;
        }

		protected override IConfiguration CreateProcessingConfiguration()
		{
			ControlProcessingConfiguration controlProcessingConfiguration = new ControlProcessingConfiguration();
			controlProcessingConfiguration.ShowSubreportErrorDetails = base.ShowDetailedSubreportMessages;
			controlProcessingConfiguration.SetMapTileServerConfiguration(this.MapTileServerConfiguration);
			return controlProcessingConfiguration;
		}

		protected override void SetProcessingCulture()
		{
		}

		protected override SubreportCallbackHandler CreateSubreportCallbackHandler()
		{
			return new SubreportCallbackHandler(this);
		}

		protected override IRenderingExtension CreateRenderer(string format, bool allowInternal)
		{
			if (format == null)
			{
				return null;
			}
			foreach (LocalRenderingExtensionInfo item in this.ListRenderingExtensions())
			{
				if (string.Compare(item.Name, format, StringComparison.OrdinalIgnoreCase) == 0)
				{
					if (!allowInternal && !item.IsExposedExternally)
					{
						break;
					}
					return item.Instantiate();
				}
			}
			throw new ArgumentOutOfRangeException("format");
		}
	}
}
