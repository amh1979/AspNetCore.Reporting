using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportRendering;
using System.IO;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartInstance : DynamicImageInstance, IDynamicImageInstance
	{
		private ReportSize m_dynamicHeight;

		private ReportSize m_dynamicWidth;

		private ChartPalette? m_palette;

		private PaletteHatchBehavior? m_paletteHatchBehavior;

		protected override int WidthInPixels
		{
			get
			{
				return MappingHelper.ToIntPixels(this.DynamicWidth, base.m_dpiX);
			}
		}

		protected override int HeightInPixels
		{
			get
			{
				return MappingHelper.ToIntPixels(this.DynamicHeight, base.m_dpiX);
			}
		}

		public ReportSize DynamicHeight
		{
			get
			{
				if (this.m_dynamicHeight == null)
				{
					if (base.m_reportElementDef.IsOldSnapshot)
					{
						this.m_dynamicHeight = new ReportSize(base.m_reportElementDef.RenderReportItem.Height);
					}
					else
					{
						string text = ((AspNetCore.ReportingServices.ReportIntermediateFormat.Chart)base.m_reportElementDef.ReportItemDef).EvaluateDynamicHeight(this, base.m_reportElementDef.RenderingContext.OdpContext);
						if (!string.IsNullOrEmpty(text))
						{
							this.m_dynamicHeight = new ReportSize(text);
						}
						else
						{
							this.m_dynamicHeight = ((ReportItem)base.m_reportElementDef).Height;
						}
					}
				}
				return this.m_dynamicHeight;
			}
		}

		public ReportSize DynamicWidth
		{
			get
			{
				if (this.m_dynamicWidth == null)
				{
					if (base.m_reportElementDef.IsOldSnapshot)
					{
						this.m_dynamicWidth = new ReportSize(base.m_reportElementDef.RenderReportItem.Width);
					}
					else
					{
						string text = ((AspNetCore.ReportingServices.ReportIntermediateFormat.Chart)base.m_reportElementDef.ReportItemDef).EvaluateDynamicWidth(this, base.m_reportElementDef.RenderingContext.OdpContext);
						if (!string.IsNullOrEmpty(text))
						{
							this.m_dynamicWidth = new ReportSize(text);
						}
						else
						{
							this.m_dynamicWidth = ((ReportItem)base.m_reportElementDef).Width;
						}
					}
				}
				return this.m_dynamicWidth;
			}
		}

		public ChartPalette Palette
		{
			get
			{
				if (!this.m_palette.HasValue)
				{
					if (base.m_reportElementDef.IsOldSnapshot)
					{
						this.m_palette = ((Chart)base.m_reportElementDef).Palette.Value;
					}
					else
					{
						this.m_palette = ((Chart)base.m_reportElementDef).ChartDef.EvaluatePalette(this.ReportScopeInstance, base.m_reportElementDef.RenderingContext.OdpContext);
					}
				}
				return this.m_palette.Value;
			}
		}

		public PaletteHatchBehavior PaletteHatchBehavior
		{
			get
			{
				if (!this.m_paletteHatchBehavior.HasValue)
				{
					this.m_paletteHatchBehavior = ((Chart)base.m_reportElementDef).ChartDef.EvaluatePaletteHatchBehavior(this.ReportScopeInstance, base.m_reportElementDef.RenderingContext.OdpContext);
				}
				return this.m_paletteHatchBehavior.Value;
			}
		}

		internal ChartInstance(Chart reportItemDef)
			: base(reportItemDef)
		{
		}

		public override void SetDpi(int xDpi, int yDpi)
		{
			if (base.m_reportElementDef.IsOldSnapshot)
			{
				((AspNetCore.ReportingServices.ReportRendering.Chart)base.m_reportElementDef.RenderReportItem).SetDpi(xDpi, yDpi);
			}
			else
			{
				base.SetDpi(xDpi, yDpi);
			}
		}

		protected override Stream GetImage(ImageType type, out bool hasImageMap)
		{
			if (base.m_reportElementDef.IsOldSnapshot)
			{
				return ((AspNetCore.ReportingServices.ReportRendering.Chart)base.m_reportElementDef.RenderReportItem).GetImage((AspNetCore.ReportingServices.ReportRendering.Chart.ImageType)type, out hasImageMap);
			}
			return base.GetImage(type, out hasImageMap);
		}

		public override Stream GetImage(ImageType type, out ActionInfoWithDynamicImageMapCollection actionImageMaps)
		{
			actionImageMaps = null;
			Stream stream = null;
			bool flag = false;
			if (base.m_reportElementDef.IsOldSnapshot)
			{
				AspNetCore.ReportingServices.ReportRendering.Chart chart = (AspNetCore.ReportingServices.ReportRendering.Chart)base.m_reportElementDef.RenderReportItem;
				stream = chart.GetImage((AspNetCore.ReportingServices.ReportRendering.Chart.ImageType)type, out flag);
				if (flag)
				{
					int dataPointSeriesCount = chart.DataPointSeriesCount;
					int dataPointCategoryCount = chart.DataPointCategoryCount;
					actionImageMaps = new ActionInfoWithDynamicImageMapCollection();
					for (int i = 0; i < dataPointSeriesCount; i++)
					{
						for (int j = 0; j < dataPointCategoryCount; j++)
						{
							AspNetCore.ReportingServices.ReportRendering.ChartDataPoint chartDataPoint = chart.DataPointCollection[i, j];
							AspNetCore.ReportingServices.ReportRendering.ActionInfo actionInfo = chartDataPoint.ActionInfo;
							if (actionInfo != null)
							{
								actionImageMaps.InternalList.Add(new ActionInfoWithDynamicImageMap(base.m_reportElementDef.RenderingContext, actionInfo, chartDataPoint.MapAreas));
							}
						}
					}
				}
			}
			else
			{
				stream = base.GetImage(type, out actionImageMaps);
			}
			return stream;
		}

		public Stream GetCoreXml()
		{
			if (base.m_reportElementDef.IsOldSnapshot)
			{
				return null;
			}
			using (IChartMapper chartMapper = ChartMapperFactory.CreateChartMapperInstance((Chart)base.m_reportElementDef, base.GetDefaultFontFamily()))
			{
				chartMapper.DpiX = base.m_dpiX;
				chartMapper.DpiY = base.m_dpiY;
				chartMapper.WidthOverride = base.m_widthOverride;
				chartMapper.HeightOverride = base.m_heightOverride;
				chartMapper.RenderChart();
				return chartMapper.GetCoreXml();
			}
		}

		protected override void GetImage(ImageType type, out ActionInfoWithDynamicImageMapCollection actionImageMaps, out Stream image)
		{
			using (IChartMapper chartMapper = ChartMapperFactory.CreateChartMapperInstance((Chart)base.m_reportElementDef, base.GetDefaultFontFamily()))
			{
				chartMapper.DpiX = base.m_dpiX;
				chartMapper.DpiY = base.m_dpiY;
				chartMapper.WidthOverride = base.m_widthOverride;
				chartMapper.HeightOverride = base.m_heightOverride;
				chartMapper.RenderChart();
				image = chartMapper.GetImage(type);
				actionImageMaps = chartMapper.GetImageMaps();
			}
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_dynamicHeight = null;
			this.m_dynamicWidth = null;
		}
	}
}
