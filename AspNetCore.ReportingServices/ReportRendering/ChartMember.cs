using AspNetCore.ReportingServices.ReportProcessing;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class ChartMember : Group
	{
		internal enum SortOrders
		{
			None,
			Ascending,
			Descending
		}

		private ChartHeading m_headingDef;

		private ChartHeadingInstance m_headingInstance;

		private ChartHeadingInstanceInfo m_headingInstanceInfo;

		private ChartMemberCollection m_children;

		private ChartMember m_parent;

		private int m_index;

		private int m_cachedMemberDataPointIndex = -1;

		public override string ID
		{
			get
			{
				if (this.m_headingDef.Grouping == null && this.m_headingDef.IDs != null)
				{
					return this.m_headingDef.IDs[this.m_index].ToString(CultureInfo.InvariantCulture);
				}
				return this.m_headingDef.ID.ToString(CultureInfo.InvariantCulture);
			}
		}

		internal override TextBox ToggleParent
		{
			get
			{
				return null;
			}
		}

		public override SharedHiddenState SharedHidden
		{
			get
			{
				if (this.IsStatic)
				{
					return SharedHiddenState.Never;
				}
				return Visibility.GetSharedHidden(base.m_visibilityDef);
			}
		}

		public override bool IsToggleChild
		{
			get
			{
				return false;
			}
		}

		public override bool Hidden
		{
			get
			{
				if (this.m_headingInstance == null)
				{
					return RenderingContext.GetDefinitionHidden(this.m_headingDef.Visibility);
				}
				if (this.m_headingDef.Visibility == null)
				{
					return false;
				}
				if (this.m_headingDef.Visibility.Toggle != null)
				{
					return base.OwnerDataRegion.RenderingContext.IsItemHidden(this.m_headingInstance.UniqueName, false);
				}
				return false;
			}
		}

		public override CustomPropertyCollection CustomProperties
		{
			get
			{
				CustomPropertyCollection customPropertyCollection = base.m_customProperties;
				if (base.m_customProperties == null)
				{
					if (this.m_headingDef.Grouping == null || this.m_headingDef.Grouping.CustomProperties == null)
					{
						return null;
					}
					customPropertyCollection = ((this.m_headingInstance != null) ? new CustomPropertyCollection(this.m_headingDef.Grouping.CustomProperties, this.InstanceInfo.CustomPropertyInstances) : new CustomPropertyCollection(this.m_headingDef.Grouping.CustomProperties, null));
					if (base.OwnerDataRegion.RenderingContext.CacheState)
					{
						base.m_customProperties = customPropertyCollection;
					}
				}
				return customPropertyCollection;
			}
		}

		public override string Label
		{
			get
			{
				return null;
			}
		}

		public object MemberLabel
		{
			get
			{
				if (this.IsFakedStatic)
				{
					return null;
				}
				if (this.m_headingInstance == null)
				{
					if (this.m_headingDef.Labels != null && this.m_headingDef.Labels[this.m_index] != null && ExpressionInfo.Types.Constant == this.m_headingDef.Labels[this.m_index].Type)
					{
						return this.m_headingDef.Labels[this.m_index].Value;
					}
					return null;
				}
				if (this.m_headingDef.ChartGroupExpression)
				{
					return this.InstanceInfo.GroupExpressionValue;
				}
				return this.InstanceInfo.HeadingLabel;
			}
		}

		public ChartMember Parent
		{
			get
			{
				return this.m_parent;
			}
		}

		public bool IsInnerMostMember
		{
			get
			{
				return null == this.m_headingDef.SubHeading;
			}
		}

		public ChartMemberCollection Children
		{
			get
			{
				ChartHeading subHeading = this.m_headingDef.SubHeading;
				if (subHeading == null)
				{
					return null;
				}
				ChartMemberCollection chartMemberCollection = this.m_children;
				if (this.m_children == null)
				{
					ChartHeadingInstanceList headingInstances = null;
					if (this.m_headingInstance != null)
					{
						headingInstances = this.m_headingInstance.SubHeadingInstances;
					}
					chartMemberCollection = new ChartMemberCollection((Chart)base.OwnerDataRegion, this, subHeading, headingInstances);
					if (base.OwnerDataRegion.RenderingContext.CacheState)
					{
						this.m_children = chartMemberCollection;
					}
				}
				return chartMemberCollection;
			}
		}

		public int MemberDataPointIndex
		{
			get
			{
				if (this.m_headingInstance == null)
				{
					if (this.m_headingDef.Grouping == null)
					{
						return this.m_index;
					}
					return 0;
				}
				return this.InstanceInfo.HeadingCellIndex;
			}
		}

		internal int CachedMemberDataPointIndex
		{
			get
			{
				if (this.m_cachedMemberDataPointIndex < 0)
				{
					this.m_cachedMemberDataPointIndex = this.MemberDataPointIndex;
				}
				return this.m_cachedMemberDataPointIndex;
			}
		}

		public int MemberHeadingSpan
		{
			get
			{
				if (this.m_headingInstance == null)
				{
					return 1;
				}
				return this.InstanceInfo.HeadingSpan;
			}
		}

		private bool IsFakedStatic
		{
			get
			{
				if (this.m_headingDef.Grouping == null && this.m_headingDef.Labels == null)
				{
					return true;
				}
				return false;
			}
		}

		public bool IsStatic
		{
			get
			{
				if (this.m_headingDef.Grouping == null)
				{
					return true;
				}
				return false;
			}
		}

		public SortOrders SortOrder
		{
			get
			{
				SortOrders result = SortOrders.None;
				if (!this.IsStatic)
				{
					BoolList boolList = (this.m_headingDef.Sorting == null) ? this.m_headingDef.Grouping.SortDirections : this.m_headingDef.Sorting.SortDirections;
					if (boolList != null && 0 < boolList.Count)
					{
						result = (SortOrders)(boolList[0] ? 1 : 2);
					}
				}
				return result;
			}
		}

		public override string DataElementName
		{
			get
			{
				if (this.IsStatic)
				{
					if (this.m_headingInstance != null && this.InstanceInfo.HeadingLabel != null)
					{
						return DataTypeUtility.ConvertToInvariantString(this.InstanceInfo.HeadingLabel);
					}
					if (!this.m_headingDef.IsColumn)
					{
						return "Series" + this.m_index.ToString(CultureInfo.InvariantCulture);
					}
					return "Category" + this.m_index.ToString(CultureInfo.InvariantCulture);
				}
				return base.DataElementName;
			}
		}

		public override DataElementOutputTypes DataElementOutput
		{
			get
			{
				if (this.IsStatic)
				{
					return this.DataElementOutputForStatic(null);
				}
				return base.DataElementOutput;
			}
		}

		internal ExpressionInfo LabelDefinition
		{
			get
			{
				if (!this.IsFakedStatic && this.m_headingDef.Labels != null)
				{
					return this.m_headingDef.Labels[this.m_index];
				}
				return null;
			}
		}

		internal object LabelValue
		{
			get
			{
				if (!this.IsFakedStatic && this.m_headingDef.Labels != null)
				{
					if (this.m_headingInstance == null)
					{
						if (this.m_headingDef.Labels != null && this.m_headingDef.Labels[this.m_index] != null && ExpressionInfo.Types.Constant == this.m_headingDef.Labels[this.m_index].Type)
						{
							return this.m_headingDef.Labels[this.m_index].Value;
						}
						return null;
					}
					return this.InstanceInfo.HeadingLabel;
				}
				return null;
			}
		}

		internal ChartHeadingInstanceInfo InstanceInfo
		{
			get
			{
				if (this.m_headingInstance == null)
				{
					return null;
				}
				if (this.m_headingInstanceInfo == null)
				{
					this.m_headingInstanceInfo = this.m_headingInstance.GetInstanceInfo(base.OwnerDataRegion.RenderingContext.ChunkManager);
				}
				return this.m_headingInstanceInfo;
			}
		}

		internal ChartMember(Chart owner, ChartMember parent, ChartHeading headingDef, ChartHeadingInstance headingInstance, int index)
			: base(owner, headingDef.Grouping, headingDef.Visibility)
		{
			this.m_parent = parent;
			this.m_headingDef = headingDef;
			this.m_headingInstance = headingInstance;
			this.m_index = index;
			if (this.m_headingInstance != null)
			{
				base.m_uniqueName = this.m_headingInstance.UniqueName;
			}
		}

		public DataElementOutputTypes DataElementOutputForStatic(ChartMember staticHeading)
		{
			if (!this.IsStatic)
			{
				return this.DataElementOutput;
			}
			if (staticHeading != null && (!staticHeading.IsStatic || staticHeading.Parent == this.Parent))
			{
				staticHeading = null;
			}
			if (staticHeading != null)
			{
				int index;
				int index2;
				if (this.m_headingDef.IsColumn)
				{
					index = staticHeading.m_index;
					index2 = this.m_index;
				}
				else
				{
					index = this.m_index;
					index2 = staticHeading.m_index;
				}
				return this.GetDataElementOutputTypeFromDataPoint(index, index2);
			}
			AspNetCore.ReportingServices.ReportProcessing.Chart chart = (AspNetCore.ReportingServices.ReportProcessing.Chart)base.OwnerDataRegion.ReportItemDef;
			if (chart.PivotStaticColumns != null && chart.PivotStaticRows != null)
			{
				Global.Tracer.Assert(chart.PivotStaticColumns != null && chart.PivotStaticRows != null);
				return this.GetDataElementOutputTypeForSeriesCategory(this.m_index);
			}
			return this.GetDataElementOutputTypeFromDataPoint(0, this.m_index);
		}

		internal bool IsPlotTypeLine()
		{
			if (this.m_headingInstance == null)
			{
				return false;
			}
			if (0 <= this.InstanceInfo.StaticGroupingIndex)
			{
				Global.Tracer.Assert(null != this.m_headingDef);
				if (this.m_headingDef.PlotTypesLine != null)
				{
					return this.m_headingDef.PlotTypesLine[this.InstanceInfo.StaticGroupingIndex];
				}
			}
			return false;
		}

		private DataElementOutputTypes GetDataElementOutputTypeFromDataPoint(int seriesIndex, int categoryIndex)
		{
			AspNetCore.ReportingServices.ReportProcessing.Chart chart = (AspNetCore.ReportingServices.ReportProcessing.Chart)base.OwnerDataRegion.ReportItemDef;
			AspNetCore.ReportingServices.ReportProcessing.ChartDataPoint dataPoint = chart.GetDataPoint(seriesIndex, categoryIndex);
			return dataPoint.DataElementOutput;
		}

		private DataElementOutputTypes GetDataElementOutputTypeForSeriesCategory(int index)
		{
			AspNetCore.ReportingServices.ReportProcessing.Chart chart = (AspNetCore.ReportingServices.ReportProcessing.Chart)base.OwnerDataRegion.ReportItemDef;
			int num;
			int num2;
			int num3;
			if (this.m_headingDef.IsColumn)
			{
				num = 0;
				num2 = index;
				num3 = chart.StaticSeriesCount;
			}
			else
			{
				num = index;
				num2 = 0;
				num3 = chart.StaticCategoryCount;
			}
			while (true)
			{
				AspNetCore.ReportingServices.ReportProcessing.ChartDataPoint dataPoint = chart.GetDataPoint(num, num2);
				if (dataPoint.DataElementOutput != DataElementOutputTypes.NoOutput)
				{
					return DataElementOutputTypes.Output;
				}
				if (this.m_headingDef.IsColumn)
				{
					num++;
					if (num >= num3)
					{
						break;
					}
				}
				else
				{
					num2++;
					if (num2 >= num3)
					{
						break;
					}
				}
			}
			return DataElementOutputTypes.NoOutput;
		}
	}
}
