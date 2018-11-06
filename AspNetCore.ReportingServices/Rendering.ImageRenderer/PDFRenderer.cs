using AspNetCore.ReportingServices.Interfaces;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.HPBProcessing;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ImageRenderer
{
	internal sealed class PDFRenderer : RendererBase
	{
		private const string FONT_EMBEDDING_NONE = "None";

		private const string FONT_EMBEDDING_SUBSET = "Subset";

		private bool m_humanReadablePDF;

		private bool m_printOnOpen;

		private bool m_test;

		private FontEmbedding m_embedFonts = FontEmbedding.Subset;

		public override string LocalizedName
		{
			get
			{
				return ImageRendererRes.PDFLocalizedName;
			}
		}

		protected override void Render(AspNetCore.ReportingServices.OnDemandReportRendering.Report report, NameValueCollection deviceInfo, Hashtable renderProperties, CreateAndRegisterStream createAndRegisterStream)
		{
			PaginationSettings paginationSettings = new PaginationSettings(report, deviceInfo);
			paginationSettings.UseEmSquare = true;
			paginationSettings.MeasureTextDpi = 1000;
			paginationSettings.UseGenericDefault = false;
			if (paginationSettings.DynamicImageDpiX == 96)
			{
				paginationSettings.DynamicImageDpiX = 300;
			}
			if (paginationSettings.DynamicImageDpiY == 96)
			{
				paginationSettings.DynamicImageDpiY = 300;
			}
			BufferedStream bufferedStream = default(BufferedStream);
			using (HPBProcessing.HPBProcessing hPBProcessing = new HPBProcessing.HPBProcessing(report, paginationSettings, createAndRegisterStream, ref renderProperties))
			{
				hPBProcessing.SetContext(hPBProcessing.PaginationSettings.StartPage, hPBProcessing.PaginationSettings.EndPage);
				using (Renderer renderer = new Renderer(true))
				{
					bufferedStream = new BufferedStream(createAndRegisterStream(report.Name, "pdf", null, "application/pdf", false, StreamOper.CreateAndRegister));
					using (PDFWriter pDFWriter = new PDFWriter(renderer, bufferedStream, false, createAndRegisterStream, hPBProcessing.PaginationSettings.DpiX, hPBProcessing.PaginationSettings.DpiY))
					{
						pDFWriter.HumanReadablePDF = this.m_humanReadablePDF;
						pDFWriter.PrintOnOpen = this.m_printOnOpen;
						pDFWriter.Test = this.m_test;
						pDFWriter.EmbedFonts = this.m_embedFonts;
						if (report.HasDocumentMap)
						{
							DocumentMap documentMap = null;
							try
							{
								documentMap = report.DocumentMap;
								if (documentMap != null)
								{
									if (documentMap.MoveNext())
									{
										DocumentMapNode current = documentMap.Current;
										PDFRenderer.HandleDocumentMap(documentMap, pDFWriter.DocumentMapRootLabel = new PDFLabel(current.Id, current.Label), 1);
									}
									if (pDFWriter.DocumentMapRootLabel.Children == null || pDFWriter.DocumentMapRootLabel.Children.Count == 0)
									{
										pDFWriter.DocumentMapRootLabel = null;
									}
									else
									{
										pDFWriter.DocumentMapLabelPoints = new Dictionary<string, PDFPagePoint>();
									}
								}
							}
							finally
							{
								if (documentMap != null)
								{
									documentMap.Dispose();
								}
							}
						}
						int num = hPBProcessing.PaginationSettings.StartPage;
						pDFWriter.BeginReport(hPBProcessing.PaginationSettings.MeasureTextDpi, hPBProcessing.PaginationSettings.MeasureTextDpi);
						while (true)
						{
							RPLReport rPLReport = default(RPLReport);
							hPBProcessing.GetNextPage(out rPLReport);
							if (rPLReport == null)
							{
								break;
							}
							renderer.ProcessPage(rPLReport, num, hPBProcessing.SharedFontCache, hPBProcessing.GlyphCache);
							rPLReport.Release();
							rPLReport = null;
							num++;
						}
						pDFWriter.EndReport();
					}
				}
			}
			bufferedStream.Flush();
		}

		private static void HandleDocumentMap(DocumentMap documentMap, PDFLabel currentLabel, int currentLevel)
		{
			while (documentMap.MoveNext())
			{
				DocumentMapNode current = documentMap.Current;
				PDFLabel pDFLabel = new PDFLabel(current.Id, current.Label);
				if (current.Level > currentLevel)
				{
					if (currentLabel.Children == null)
					{
						currentLabel.Children = new List<PDFLabel>();
					}
					currentLabel.Children.Add(pDFLabel);
					pDFLabel.Parent = currentLabel;
					currentLevel++;
				}
				else if (current.Level == currentLevel)
				{
					currentLabel.Parent.Children.Add(pDFLabel);
					pDFLabel.Parent = currentLabel.Parent;
				}
				else
				{
					for (int num = currentLevel - current.Level; num >= 0; num--)
					{
						currentLabel = currentLabel.Parent;
					}
					currentLabel.Children.Add(pDFLabel);
					pDFLabel.Parent = currentLabel;
					currentLevel = current.Level;
				}
				currentLabel = pDFLabel;
			}
		}

		private static FontEmbedding ParseDeviceInfoFontEmbedding(string fontEmbeddingValue, FontEmbedding defaultValue)
		{
			if (string.Compare(fontEmbeddingValue, "None", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return FontEmbedding.None;
			}
			if (string.Compare(fontEmbeddingValue, "Subset", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return FontEmbedding.Subset;
			}
			return defaultValue;
		}

		protected override void ParseDeviceInfo(ref NameValueCollection deviceInfo)
		{
			if (deviceInfo != null)
			{
				for (int i = 0; i < deviceInfo.Count; i++)
				{
					string text = deviceInfo.Keys[i];
					string text2 = deviceInfo[i];
					switch (text.ToUpper(CultureInfo.InvariantCulture))
					{
					case "HUMANREADABLEPDF":
						this.m_humanReadablePDF = RendererBase.ParseDeviceInfoBoolean(text2, false);
						break;
					case "PRINTONOPEN":
						this.m_printOnOpen = RendererBase.ParseDeviceInfoBoolean(text2, false);
						break;
					case "EMBEDFONTS":
						this.m_embedFonts = PDFRenderer.ParseDeviceInfoFontEmbedding(text2, FontEmbedding.Subset);
						break;
					case "TEST":
						this.m_test = RendererBase.ParseDeviceInfoBoolean(text2, false);
						break;
					}
				}
			}
		}
	}
}
