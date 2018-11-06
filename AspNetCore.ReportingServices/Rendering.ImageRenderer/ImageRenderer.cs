using AspNetCore.ReportingServices.Interfaces;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.HPBProcessing;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ImageRenderer
{
	internal sealed class ImageRenderer : RendererBase
	{
		private const int DEFAULT_PRINT_DPI = 300;

		private int m_printDpiX = 300;

		private int m_printDpiY = 300;

		public override string LocalizedName
		{
			get
			{
				return ImageRendererRes.IMAGELocalizedName;
			}
		}

		protected override void Render(AspNetCore.ReportingServices.OnDemandReportRendering.Report report, NameValueCollection deviceInfo, Hashtable renderProperties, CreateAndRegisterStream createAndRegisterStream)
		{
			PaginationSettings paginationSettings = new PaginationSettings(report, deviceInfo);
			string text = paginationSettings.OutputFormat.ToString().ToUpperInvariant();
			bool flag = true;
			if (text == "TIFF")
			{
				text = "TIF";
				flag = false;
			}
			else if (text == "EMFPLUS")
			{
				text = "EMF";
			}
			if (text == "EMF")
			{
				paginationSettings.DynamicImageDpiX = 96;
				paginationSettings.DynamicImageDpiY = 96;
				paginationSettings.DpiX = this.m_printDpiX;
				paginationSettings.DpiY = this.m_printDpiY;
				int measureImageDpiX = default(int);
				int measureImageDpiY = default(int);
				ImageWriter.GetScreenDpi(out measureImageDpiX, out measureImageDpiY);
				paginationSettings.MeasureImageDpiX = measureImageDpiX;
				paginationSettings.MeasureImageDpiY = measureImageDpiY;
			}
			else
			{
				paginationSettings.MeasureImageDpiX = paginationSettings.DpiX;
				paginationSettings.MeasureImageDpiY = paginationSettings.DpiY;
			}
			paginationSettings.MeasureTextDpi = paginationSettings.DpiX;
			Stream stream = default(Stream);
			using (HPBProcessing.HPBProcessing hPBProcessing = new HPBProcessing.HPBProcessing(report, paginationSettings, createAndRegisterStream, ref renderProperties))
			{
				hPBProcessing.SetContext(hPBProcessing.PaginationSettings.StartPage, hPBProcessing.PaginationSettings.EndPage);
				using (Renderer renderer = new Renderer(true))
				{
					stream = createAndRegisterStream(report.Name + (flag ? ('_' + hPBProcessing.PaginationSettings.StartPage.ToString(CultureInfo.InvariantCulture)) : ""), text, null, "image/" + text, !flag, StreamOper.CreateAndRegister);
					using (ImageWriter imageWriter = new ImageWriter(renderer, stream, false, createAndRegisterStream, paginationSettings.MeasureImageDpiX, paginationSettings.MeasureImageDpiY))
					{
						imageWriter.OutputFormat = hPBProcessing.PaginationSettings.OutputFormat;
						hPBProcessing.PaginationSettings.UseGenericDefault = !imageWriter.IsEmf;
						imageWriter.BeginReport(hPBProcessing.PaginationSettings.DpiX, hPBProcessing.PaginationSettings.DpiY);
						int num = hPBProcessing.PaginationSettings.StartPage;
						while (true)
						{
							RPLReport rPLReport = default(RPLReport);
							hPBProcessing.GetNextPage(out rPLReport);
							if (rPLReport == null)
							{
								break;
							}
							if (flag && num > hPBProcessing.PaginationSettings.StartPage)
							{
								stream = (imageWriter.OutputStream = createAndRegisterStream(report.Name + '_' + num, text, null, "image/" + text, !flag, StreamOper.CreateForPersistedStreams));
							}
							renderer.ProcessPage(rPLReport, num, hPBProcessing.SharedFontCache, hPBProcessing.GlyphCache);
							rPLReport.Release();
							rPLReport = null;
							num++;
						}
						imageWriter.EndReport();
					}
				}
			}
			stream.Flush();
		}

		private void ValidatePrintDpiValue(ref int currValue, int defaultValue)
		{
			if (currValue <= 0)
			{
				currValue = defaultValue;
			}
		}

		protected override void ParseDeviceInfo(ref NameValueCollection deviceInfo)
		{
			if (deviceInfo != null)
			{
				for (int i = 0; i < deviceInfo.Count; i++)
				{
					string text = deviceInfo.Keys[i];
					string intValue = deviceInfo[i];
					switch (text.ToUpper(CultureInfo.InvariantCulture))
					{
					case "PRINTDPIX":
						this.m_printDpiX = RendererBase.ParseDeviceInfoInt32(intValue, 300);
						this.ValidatePrintDpiValue(ref this.m_printDpiX, 300);
						break;
					case "PRINTDPIY":
						this.m_printDpiY = RendererBase.ParseDeviceInfoInt32(intValue, 300);
						this.ValidatePrintDpiValue(ref this.m_printDpiY, 300);
						break;
					}
				}
			}
		}
	}
}
