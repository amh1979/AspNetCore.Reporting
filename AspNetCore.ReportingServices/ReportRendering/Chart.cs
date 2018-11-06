using AspNetCore.ReportingServices.ReportProcessing;
using System.IO;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class Chart : DataRegion
	{
		internal enum ImageType
		{
			PNG,
			EMF
		}

		private ChartDataPointCollection m_datapoints;

		private ChartMemberCollection m_categories;

		private ChartMemberCollection m_series;

		private int m_categoryGroupingLevels;

		private ImageMapAreasCollection[] m_imageMapAreaCollection;

		private float m_scaleX = -1f;

		private float m_scaleY = -1f;

		public ChartDataPointCollection DataPointCollection
		{
			get
			{
				ChartDataPointCollection chartDataPointCollection = this.m_datapoints;
				if (this.m_datapoints == null)
				{
					int num = 0;
					int num2 = 0;
					if (base.ReportItemInstance != null && 0 < ((ChartInstance)base.ReportItemInstance).DataPoints.Count)
					{
						num = this.DataPointSeriesCount;
						num2 = this.DataPointCategoryCount;
					}
					else
					{
						num = ((AspNetCore.ReportingServices.ReportProcessing.Chart)base.ReportItemDef).StaticSeriesCount;
						num2 = ((AspNetCore.ReportingServices.ReportProcessing.Chart)base.ReportItemDef).StaticCategoryCount;
					}
					chartDataPointCollection = new ChartDataPointCollection(this, num, num2);
					if (base.RenderingContext.CacheState)
					{
						this.m_datapoints = chartDataPointCollection;
					}
				}
				return chartDataPointCollection;
			}
		}

		public ChartMemberCollection CategoryMemberCollection
		{
			get
			{
				ChartMemberCollection chartMemberCollection = this.m_categories;
				if (this.m_categories == null)
				{
					chartMemberCollection = new ChartMemberCollection(this, null, ((AspNetCore.ReportingServices.ReportProcessing.Chart)base.ReportItemDef).Columns, (base.ReportItemInstance == null) ? null : ((ChartInstance)base.ReportItemInstance).ColumnInstances);
					if (base.RenderingContext.CacheState)
					{
						this.m_categories = chartMemberCollection;
					}
				}
				return chartMemberCollection;
			}
		}

		public ChartMemberCollection SeriesMemberCollection
		{
			get
			{
				ChartMemberCollection chartMemberCollection = this.m_series;
				if (this.m_series == null)
				{
					chartMemberCollection = new ChartMemberCollection(this, null, ((AspNetCore.ReportingServices.ReportProcessing.Chart)base.ReportItemDef).Rows, (base.ReportItemInstance == null) ? null : ((ChartInstance)base.ReportItemInstance).RowInstances);
					if (base.RenderingContext.CacheState)
					{
						this.m_series = chartMemberCollection;
					}
				}
				return chartMemberCollection;
			}
		}

		public int DataPointCategoryCount
		{
			get
			{
				if (base.ReportItemInstance == null)
				{
					return 0;
				}
				return ((ChartInstance)base.ReportItemInstance).DataPointCategoryCount;
			}
		}

		public int DataPointSeriesCount
		{
			get
			{
				if (base.ReportItemInstance == null)
				{
					return 0;
				}
				return ((ChartInstance)base.ReportItemInstance).DataPointSeriesCount;
			}
		}

		public int CategoriesCount
		{
			get
			{
				return ((AspNetCore.ReportingServices.ReportProcessing.Chart)base.ReportItemDef).StaticCategoryCount;
			}
		}

		public int SeriesCount
		{
			get
			{
				return ((AspNetCore.ReportingServices.ReportProcessing.Chart)base.ReportItemDef).StaticSeriesCount;
			}
		}

		public override bool NoRows
		{
			get
			{
				if (base.ReportItemInstance != null && ((ChartInstance)base.ReportItemInstance).DataPointSeriesCount != 0 && ((ChartInstance)base.ReportItemInstance).DataPointCategoryCount != 0)
				{
					return false;
				}
				return true;
			}
		}

		internal ChartDataPointInstancesList DataPoints
		{
			get
			{
				return ((ChartInstance)base.ReportItemInstance).DataPoints;
			}
		}

		internal int CategoryGroupingLevels
		{
			get
			{
				if (this.m_categoryGroupingLevels == 0)
				{
					this.m_categoryGroupingLevels = 1;
					ChartHeading chartHeading = ((AspNetCore.ReportingServices.ReportProcessing.Chart)base.ReportItemDef).Columns;
					Global.Tracer.Assert(null != chartHeading);
					while (chartHeading.SubHeading != null)
					{
						chartHeading = chartHeading.SubHeading;
						this.m_categoryGroupingLevels++;
					}
				}
				return this.m_categoryGroupingLevels;
			}
		}

		internal override string InstanceInfoNoRowMessage
		{
			get
			{
				if (base.InstanceInfo != null)
				{
					return ((ChartInstanceInfo)base.InstanceInfo).NoRows;
				}
				return null;
			}
		}

		internal ImageMapAreasCollection[] DataPointMapAreas
		{
			get
			{
				if (this.m_imageMapAreaCollection == null)
				{
					this.RenderChartImageMap();
				}
				return this.m_imageMapAreaCollection;
			}
		}

		internal float ScaleX
		{
			get
			{
				return this.m_scaleX;
			}
			set
			{
				this.m_scaleX = value;
			}
		}

		internal float ScaleY
		{
			get
			{
				return this.m_scaleY;
			}
			set
			{
				this.m_scaleY = value;
			}
		}

		internal Chart(int intUniqueName, AspNetCore.ReportingServices.ReportProcessing.Chart reportItemDef, ChartInstance reportItemInstance, RenderingContext renderingContext)
			: base(intUniqueName, reportItemDef, reportItemInstance, renderingContext)
		{
		}

		public void SetDpi(int xDpi, int yDpi)
		{
			if (xDpi > 0 && xDpi != 96)
			{
				this.m_scaleX = (float)((float)xDpi / 96.0);
			}
			if (yDpi > 0 && yDpi != 96)
			{
				this.m_scaleY = (float)((float)yDpi / 96.0);
			}
		}

		public MemoryStream GetImage(out bool hasImageMap)
		{
			return this.GetImage(ImageType.PNG, out hasImageMap);
		}

		public MemoryStream GetImage()
		{
			bool flag = default(bool);
			return this.GetImage(ImageType.PNG, out flag);
		}

		public MemoryStream GetImage(ImageType type)
		{
			bool flag = default(bool);
			return this.GetImage(type, out flag);
		}

		public MemoryStream GetImage(ImageType type, out bool hasImageMap)
		{
			hasImageMap = false;
			return null;
		}

		private bool RenderChartImageMap()
		{
			return null != this.m_imageMapAreaCollection;
		}
	}
}
