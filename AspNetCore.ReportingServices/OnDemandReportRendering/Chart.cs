using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class Chart : DataRegion
	{
		private int m_memberCellDefinitionIndex;

		private ChartHierarchy m_categories;

		private ChartHierarchy m_series;

		private ChartData m_chartData;

		private ReportSizeProperty m_dynamicHeight;

		private ReportSizeProperty m_dynamicWidth;

		private ChartAreaCollection m_chartAreas;

		private ChartTitleCollection m_titles;

		private ChartLegendCollection m_legends;

		private ChartBorderSkin m_borderSkin;

		private ChartCustomPaletteColorCollection m_customPaletteColors;

		private ReportEnumProperty<ChartPalette> m_palette;

		private ReportEnumProperty<PaletteHatchBehavior> m_paletteHatchBehavior;

		private ChartTitle m_noDataMessage;

		public bool DataValueSequenceRendering
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return true;
				}
				return this.ChartDef.DataValueSequenceRendering;
			}
		}

		public ChartHierarchy CategoryHierarchy
		{
			get
			{
				if (this.m_categories == null)
				{
					this.m_categories = new ChartHierarchy(this, true);
				}
				return this.m_categories;
			}
		}

		public ChartHierarchy SeriesHierarchy
		{
			get
			{
				if (this.m_series == null)
				{
					this.m_series = new ChartHierarchy(this, false);
				}
				return this.m_series;
			}
		}

		public ChartData ChartData
		{
			get
			{
				if (this.m_chartData == null)
				{
					this.m_chartData = new ChartData(this);
				}
				return this.m_chartData;
			}
		}

		public int Categories
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return this.RenderChart.CategoriesCount;
				}
				return this.ChartDef.CategoryCount;
			}
		}

		public int Series
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return this.RenderChart.SeriesCount;
				}
				return this.ChartDef.SeriesCount;
			}
		}

		public ChartTitleCollection Titles
		{
			get
			{
				if (this.m_titles == null)
				{
					if (base.m_isOldSnapshot)
					{
						if (this.RenderChartDef.Title != null)
						{
							this.m_titles = new ChartTitleCollection(this);
						}
					}
					else if (this.ChartDef.Titles != null)
					{
						this.m_titles = new ChartTitleCollection(this);
					}
				}
				return this.m_titles;
			}
		}

		public ChartCustomPaletteColorCollection CustomPaletteColors
		{
			get
			{
				if (this.m_customPaletteColors == null && !base.m_isOldSnapshot && this.ChartDef.CustomPaletteColors != null)
				{
					this.m_customPaletteColors = new ChartCustomPaletteColorCollection(this);
				}
				return this.m_customPaletteColors;
			}
		}

		public ChartBorderSkin BorderSkin
		{
			get
			{
				if (this.m_borderSkin == null && !base.m_isOldSnapshot && this.ChartDef.BorderSkin != null)
				{
					this.m_borderSkin = new ChartBorderSkin(this.ChartDef.BorderSkin, this);
				}
				return this.m_borderSkin;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.Chart ChartDef
		{
			get
			{
				return base.m_reportItemDef as AspNetCore.ReportingServices.ReportIntermediateFormat.Chart;
			}
		}

		internal override bool HasDataCells
		{
			get
			{
				if (this.m_chartData != null)
				{
					return this.m_chartData.HasSeriesCollection;
				}
				return false;
			}
		}

		internal override IDataRegionRowCollection RowCollection
		{
			get
			{
				if (this.m_chartData != null)
				{
					return this.m_chartData.SeriesCollection;
				}
				return null;
			}
		}

		public ChartAreaCollection ChartAreas
		{
			get
			{
				if (this.m_chartAreas == null)
				{
					if (base.m_isOldSnapshot)
					{
						this.m_chartAreas = new ChartAreaCollection(this);
					}
					else if (this.ChartDef.ChartAreas != null)
					{
						this.m_chartAreas = new ChartAreaCollection(this);
					}
				}
				return this.m_chartAreas;
			}
		}

		public ChartLegendCollection Legends
		{
			get
			{
				if (this.m_legends == null)
				{
					if (base.m_isOldSnapshot)
					{
						if (this.RenderChartDef.Legend != null)
						{
							this.m_legends = new ChartLegendCollection(this);
						}
					}
					else if (this.ChartDef.Legends != null)
					{
						this.m_legends = new ChartLegendCollection(this);
					}
				}
				return this.m_legends;
			}
		}

		public ReportEnumProperty<ChartPalette> Palette
		{
			get
			{
				if (this.m_palette == null)
				{
					if (base.m_isOldSnapshot)
					{
						this.m_palette = new ReportEnumProperty<ChartPalette>(false, null, (ChartPalette)this.RenderChartDef.Palette);
					}
					else if (this.ChartDef.Palette != null)
					{
						this.m_palette = new ReportEnumProperty<ChartPalette>(this.ChartDef.Palette.IsExpression, this.ChartDef.Palette.OriginalText, EnumTranslator.TranslateChartPalette(this.ChartDef.Palette.StringValue, null));
					}
				}
				return this.m_palette;
			}
		}

		public ReportEnumProperty<PaletteHatchBehavior> PaletteHatchBehavior
		{
			get
			{
				if (this.m_paletteHatchBehavior == null)
				{
					if (base.m_isOldSnapshot)
					{
						if (AspNetCore.ReportingServices.ReportProcessing.Chart.ChartPalette.GrayScale == this.RenderChartDef.Palette)
						{
							this.m_paletteHatchBehavior = new ReportEnumProperty<PaletteHatchBehavior>(AspNetCore.ReportingServices.OnDemandReportRendering.PaletteHatchBehavior.Always);
						}
					}
					else if (this.ChartDef.PaletteHatchBehavior != null)
					{
						this.m_paletteHatchBehavior = new ReportEnumProperty<PaletteHatchBehavior>(this.ChartDef.PaletteHatchBehavior.IsExpression, this.ChartDef.PaletteHatchBehavior.OriginalText, EnumTranslator.TranslatePaletteHatchBehavior(this.ChartDef.PaletteHatchBehavior.StringValue, null));
					}
				}
				return this.m_paletteHatchBehavior;
			}
		}

		public ReportSizeProperty DynamicHeight
		{
			get
			{
				if (this.m_dynamicHeight == null)
				{
					if (base.m_isOldSnapshot)
					{
						this.m_dynamicHeight = new ReportSizeProperty(false, base.m_renderReportItem.ReportItemDef.Height, new ReportSize(base.m_renderReportItem.ReportItemDef.Height));
					}
					else if (this.ChartDef.DynamicHeight != null)
					{
						this.m_dynamicHeight = new ReportSizeProperty(this.ChartDef.DynamicHeight);
					}
					else
					{
						this.m_dynamicHeight = new ReportSizeProperty(false, base.m_reportItemDef.Height, new ReportSize(base.m_reportItemDef.Height));
					}
				}
				return this.m_dynamicHeight;
			}
		}

		public ReportSizeProperty DynamicWidth
		{
			get
			{
				if (this.m_dynamicWidth == null)
				{
					if (base.m_isOldSnapshot)
					{
						this.m_dynamicWidth = new ReportSizeProperty(false, base.m_renderReportItem.ReportItemDef.Width, new ReportSize(base.m_renderReportItem.ReportItemDef.Width));
					}
					else if (this.ChartDef.DynamicWidth != null)
					{
						this.m_dynamicWidth = new ReportSizeProperty(this.ChartDef.DynamicWidth);
					}
					else
					{
						this.m_dynamicWidth = new ReportSizeProperty(false, base.m_reportItemDef.Width, new ReportSize(base.m_reportItemDef.Width));
					}
				}
				return this.m_dynamicWidth;
			}
		}

		public ChartTitle NoDataMessage
		{
			get
			{
				if (this.m_noDataMessage == null && !base.IsOldSnapshot && this.ChartDef.NoDataMessage != null)
				{
					this.m_noDataMessage = new ChartTitle(this.ChartDef.NoDataMessage, this);
				}
				return this.m_noDataMessage;
			}
		}

		public bool SpecialBorderHandling
		{
			get
			{
				ChartBorderSkin borderSkin = this.BorderSkin;
				if (borderSkin == null)
				{
					return false;
				}
				ReportEnumProperty<ChartBorderSkinType> borderSkinType = borderSkin.BorderSkinType;
				if (borderSkinType == null)
				{
					return false;
				}
				ChartBorderSkinType chartBorderSkinType = borderSkinType.IsExpression ? borderSkin.Instance.BorderSkinType : borderSkinType.Value;
				return chartBorderSkinType != ChartBorderSkinType.None;
			}
		}

		internal ChartInstanceInfo ChartInstanceInfo
		{
			get
			{
				return (ChartInstanceInfo)this.RenderReportItem.InstanceInfo;
			}
		}

		internal AspNetCore.ReportingServices.ReportProcessing.Chart RenderChartDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportProcessing.Chart)this.RenderReportItem.ReportItemDef;
			}
		}

		internal AspNetCore.ReportingServices.ReportRendering.Chart RenderChart
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return (AspNetCore.ReportingServices.ReportRendering.Chart)base.m_renderReportItem;
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
		}

		internal Chart(IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, AspNetCore.ReportingServices.ReportIntermediateFormat.Chart reportItemDef, RenderingContext renderingContext)
			: base(parentDefinitionPath, indexIntoParentCollectionDef, reportItemDef, renderingContext)
		{
		}

		internal Chart(IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, bool inSubtotal, AspNetCore.ReportingServices.ReportRendering.Chart renderChart, RenderingContext renderingContext)
			: base(parentDefinitionPath, indexIntoParentCollectionDef, inSubtotal, renderChart, renderingContext)
		{
			base.m_snapshotDataRegionType = Type.Chart;
		}

		internal override ReportItemInstance GetOrCreateInstance()
		{
			if (base.m_instance == null)
			{
				base.m_instance = new ChartInstance(this);
			}
			return base.m_instance;
		}

		internal override void SetNewContextChildren()
		{
			if (this.m_categories != null)
			{
				this.m_categories.ResetContext();
			}
			if (this.m_series != null)
			{
				this.m_series.ResetContext();
			}
			if (this.m_chartAreas != null)
			{
				this.m_chartAreas.SetNewContext();
			}
			if (this.m_titles != null)
			{
				this.m_titles.SetNewContext();
			}
			if (this.m_customPaletteColors != null)
			{
				this.m_customPaletteColors.SetNewContext();
			}
			if (this.m_legends != null)
			{
				this.m_legends.SetNewContext();
			}
			if (this.m_borderSkin != null)
			{
				this.m_borderSkin.SetNewContext();
			}
			if (this.m_noDataMessage != null)
			{
				this.m_noDataMessage.SetNewContext();
			}
		}

		internal override void UpdateRenderReportItem(AspNetCore.ReportingServices.ReportRendering.ReportItem renderReportItem)
		{
			base.UpdateRenderReportItem(renderReportItem);
			if (renderReportItem != null)
			{
				this.m_categories = null;
				this.m_series = null;
				this.m_memberCellDefinitionIndex = 0;
			}
			else
			{
				if (this.m_categories != null)
				{
					this.m_categories.ResetContext();
				}
				if (this.m_series != null)
				{
					this.m_series.ResetContext();
				}
			}
			this.m_chartAreas = null;
			this.m_titles = null;
			this.m_customPaletteColors = null;
			this.m_legends = null;
			this.m_borderSkin = null;
		}

		internal int GetCurrentMemberCellDefinitionIndex()
		{
			return this.m_memberCellDefinitionIndex;
		}

		internal int GetAndIncrementMemberCellDefinitionIndex()
		{
			return this.m_memberCellDefinitionIndex++;
		}

		internal void ResetMemberCellDefinitionIndex(int startIndex)
		{
			this.m_memberCellDefinitionIndex = startIndex;
		}

		internal ChartMember GetChartMember(ChartSeries chartSeries)
		{
			return this.GetChartMember(this.SeriesHierarchy.MemberCollection, this.GetSeriesIndex(chartSeries));
		}

		private int GetSeriesIndex(ChartSeries series)
		{
			ChartSeriesCollection seriesCollection = this.ChartData.SeriesCollection;
			for (int i = 0; i < seriesCollection.Count; i++)
			{
				if (object.ReferenceEquals(((ReportElementCollectionBase<ChartSeries>)seriesCollection)[i], series))
				{
					return i;
				}
			}
			return -1;
		}

		internal ChartMember GetChartMember(ChartMemberCollection chartMemberCollection, int memberCellIndex)
		{
			foreach (ChartMember item in chartMemberCollection)
			{
				if (item != null)
				{
					if (item.Children == null)
					{
						if (item.MemberCellIndex == memberCellIndex)
						{
							return item;
						}
					}
					else
					{
						ChartMember chartMember = this.GetChartMember(item.Children, memberCellIndex);
						if (chartMember != null)
						{
							return chartMember;
						}
					}
				}
			}
			return null;
		}

		internal List<ChartDerivedSeries> GetChildrenDerivedSeries(string chartSeriesName)
		{
			ChartDerivedSeriesCollection derivedSeriesCollection = this.ChartData.DerivedSeriesCollection;
			if (derivedSeriesCollection == null)
			{
				return null;
			}
			List<ChartDerivedSeries> list = null;
			foreach (ChartDerivedSeries item in derivedSeriesCollection)
			{
				if (item != null && AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.CompareWithInvariantCulture(item.SourceChartSeriesName, chartSeriesName, false) == 0)
				{
					if (list == null)
					{
						list = new List<ChartDerivedSeries>();
					}
					list.Add(item);
				}
			}
			return list;
		}
	}
}
