using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Specialized;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class ChartDataPoint
	{
		private Chart m_owner;

		private int m_seriesIndex;

		private int m_categoryIndex;

		private ChartDataPointInstance m_chartDataPointInstance;

		private ChartDataPointInstanceInfo m_chartDataPointInstanceInfo;

		private CustomPropertyCollection m_customProperties;

		private ActionInfo m_actionInfo;

		public object[] DataValues
		{
			get
			{
				if (this.InstanceInfo == null)
				{
					return null;
				}
				return this.InstanceInfo.DataValues;
			}
		}

		public string DataElementName
		{
			get
			{
				AspNetCore.ReportingServices.ReportProcessing.Chart chart = (AspNetCore.ReportingServices.ReportProcessing.Chart)this.m_owner.ReportItemDef;
				int index = this.IndexDataPointDefinition(chart);
				return chart.ChartDataPoints[index].DataElementName;
			}
		}

		public DataElementOutputTypes DataElementOutput
		{
			get
			{
				AspNetCore.ReportingServices.ReportProcessing.Chart chart = (AspNetCore.ReportingServices.ReportProcessing.Chart)this.m_owner.ReportItemDef;
				int index = this.IndexDataPointDefinition(chart);
				return chart.ChartDataPoints[index].DataElementOutput;
			}
		}

		public CustomPropertyCollection CustomProperties
		{
			get
			{
				CustomPropertyCollection customPropertyCollection = this.m_customProperties;
				if (this.m_customProperties == null)
				{
					AspNetCore.ReportingServices.ReportProcessing.ChartDataPoint chartDataPointDefinition = this.ChartDataPointDefinition;
					Global.Tracer.Assert(null != chartDataPointDefinition);
					if (chartDataPointDefinition.CustomProperties == null)
					{
						return null;
					}
					if (this.m_owner.NoRows)
					{
						customPropertyCollection = new CustomPropertyCollection(chartDataPointDefinition.CustomProperties, null);
					}
					else
					{
						ChartDataPointInstanceInfo instanceInfo = this.InstanceInfo;
						Global.Tracer.Assert(null != instanceInfo);
						customPropertyCollection = new CustomPropertyCollection(chartDataPointDefinition.CustomProperties, instanceInfo.CustomPropertyInstances);
					}
					if (this.m_owner.RenderingContext.CacheState)
					{
						this.m_customProperties = customPropertyCollection;
					}
				}
				return customPropertyCollection;
			}
		}

		public ImageMapAreasCollection MapAreas
		{
			get
			{
				if (this.m_owner.DataPointMapAreas == null)
				{
					return null;
				}
				int num = this.m_seriesIndex * this.m_owner.DataPointCollection.CategoryCount + this.m_categoryIndex;
				Global.Tracer.Assert(num >= 0 && num < this.m_owner.DataPointMapAreas.Length);
				return this.m_owner.DataPointMapAreas[num];
			}
		}

		public ReportUrl HyperLinkURL
		{
			get
			{
				ActionInfo actionInfo = this.m_actionInfo;
				if (actionInfo == null)
				{
					actionInfo = this.ActionInfo;
				}
				if (actionInfo != null)
				{
					return actionInfo.Actions[0].HyperLinkURL;
				}
				return null;
			}
		}

		public ReportUrl DrillthroughReport
		{
			get
			{
				ActionInfo actionInfo = this.m_actionInfo;
				if (actionInfo == null)
				{
					actionInfo = this.ActionInfo;
				}
				if (actionInfo != null)
				{
					return actionInfo.Actions[0].DrillthroughReport;
				}
				return null;
			}
		}

		public NameValueCollection DrillthroughParameters
		{
			get
			{
				ActionInfo actionInfo = this.m_actionInfo;
				if (actionInfo == null)
				{
					actionInfo = this.ActionInfo;
				}
				if (actionInfo != null)
				{
					return actionInfo.Actions[0].DrillthroughParameters;
				}
				return null;
			}
		}

		public string BookmarkLink
		{
			get
			{
				ActionInfo actionInfo = this.m_actionInfo;
				if (actionInfo == null)
				{
					actionInfo = this.ActionInfo;
				}
				if (actionInfo != null)
				{
					return actionInfo.Actions[0].BookmarkLink;
				}
				return null;
			}
		}

		public ActionInfo ActionInfo
		{
			get
			{
				ActionInfo actionInfo = this.m_actionInfo;
				if (actionInfo == null)
				{
					AspNetCore.ReportingServices.ReportProcessing.ChartDataPoint chartDataPointDefinition = this.ChartDataPointDefinition;
					if (chartDataPointDefinition != null)
					{
						AspNetCore.ReportingServices.ReportProcessing.Action action = chartDataPointDefinition.Action;
						if (action != null)
						{
							ActionInstance actionInstance = null;
							string ownerUniqueName = null;
							if (this.m_chartDataPointInstance != null)
							{
								actionInstance = this.InstanceInfo.Action;
								ownerUniqueName = this.m_chartDataPointInstance.UniqueName.ToString(CultureInfo.InvariantCulture);
							}
							actionInfo = new ActionInfo(action, actionInstance, ownerUniqueName, this.m_owner.RenderingContext);
							if (this.m_owner.RenderingContext.CacheState)
							{
								this.m_actionInfo = actionInfo;
							}
						}
					}
				}
				return actionInfo;
			}
		}

		internal ChartDataPointInstanceInfo InstanceInfo
		{
			get
			{
				if (this.m_chartDataPointInstance == null)
				{
					return null;
				}
				if (this.m_chartDataPointInstanceInfo == null)
				{
					this.m_chartDataPointInstanceInfo = this.m_chartDataPointInstance.GetInstanceInfo(this.m_owner.RenderingContext.ChunkManager, ((AspNetCore.ReportingServices.ReportProcessing.Chart)this.m_owner.ReportItemDef).ChartDataPoints);
				}
				return this.m_chartDataPointInstanceInfo;
			}
		}

		private AspNetCore.ReportingServices.ReportProcessing.ChartDataPoint ChartDataPointDefinition
		{
			get
			{
				AspNetCore.ReportingServices.ReportProcessing.Chart chart = (AspNetCore.ReportingServices.ReportProcessing.Chart)this.m_owner.ReportItemDef;
				if (this.m_owner.NoRows)
				{
					return chart.ChartDataPoints[this.m_seriesIndex * chart.StaticCategoryCount + this.m_categoryIndex];
				}
				if (this.InstanceInfo != null)
				{
					return chart.ChartDataPoints[this.InstanceInfo.DataPointIndex];
				}
				return null;
			}
		}

		internal ChartDataPoint(Chart owner, int seriesIndex, int categoryIndex)
		{
			this.m_owner = owner;
			this.m_seriesIndex = seriesIndex;
			this.m_categoryIndex = categoryIndex;
			if (!owner.NoRows)
			{
				ChartDataPointInstancesList dataPoints = ((ChartInstance)owner.ReportItemInstance).DataPoints;
				this.m_chartDataPointInstance = dataPoints[seriesIndex][categoryIndex];
			}
		}

		private int IndexDataPointDefinition(AspNetCore.ReportingServices.ReportProcessing.Chart chartDef)
		{
			int num = 0;
			if (this.m_owner.NoRows)
			{
				return this.m_seriesIndex * chartDef.StaticCategoryCount + this.m_categoryIndex;
			}
			return this.InstanceInfo.DataPointIndex;
		}
	}
}
