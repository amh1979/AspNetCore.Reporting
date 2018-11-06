using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal class ChartSeries : Row, IPersistable, IActionOwner, IStyleContainer, ICustomPropertiesHolder
	{
		private ChartDataPointList m_dataPoints;

		private int m_exprHostID;

		private string m_name;

		private Action m_action;

		private ExpressionInfo m_type;

		private ExpressionInfo m_subtype;

		private ChartEmptyPoints m_emptyPoints;

		private ExpressionInfo m_legendName;

		private ExpressionInfo m_legendText;

		private ExpressionInfo m_chartAreaName;

		private ExpressionInfo m_valueAxisName;

		private ExpressionInfo m_categoryAxisName;

		private Style m_styleClass;

		private ExpressionInfo m_hidden;

		private ExpressionInfo m_hideInLegend;

		private ChartSmartLabel m_chartSmartLabel;

		private DataValueList m_customProperties;

		private ChartDataLabel m_dataLabel;

		private ChartMarker m_marker;

		private ExpressionInfo m_toolTip;

		private ChartItemInLegend m_chartItemInLegend;

		[Reference]
		private Chart m_chart;

		[Reference]
		private ChartMember m_parentChartMember;

		[Reference]
		private ChartSeries m_sourceSeries;

		[NonSerialized]
		private ChartDerivedSeries m_parentDerivedSeries;

		[NonSerialized]
		private List<ChartDerivedSeries> m_childrenDerivedSeries;

		[NonSerialized]
		private List<string> m_fieldsUsedInValueExpression;

		[NonSerialized]
		private ChartSeriesExprHost m_exprHost;

		[NonSerialized]
		private Formatter m_formatter;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ChartSeries.GetDeclaration();

		internal override CellList Cells
		{
			get
			{
				return this.m_dataPoints;
			}
		}

		internal ChartDataPointList DataPoints
		{
			get
			{
				return this.m_dataPoints;
			}
			set
			{
				this.m_dataPoints = value;
			}
		}

		internal ChartSeriesExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal int ExpressionHostID
		{
			get
			{
				return this.m_exprHostID;
			}
		}

		internal Action Action
		{
			get
			{
				return this.m_action;
			}
			set
			{
				this.m_action = value;
			}
		}

		internal ExpressionInfo Hidden
		{
			get
			{
				return this.m_hidden;
			}
			set
			{
				this.m_hidden = value;
			}
		}

		internal ExpressionInfo HideInLegend
		{
			get
			{
				return this.m_hideInLegend;
			}
			set
			{
				this.m_hideInLegend = value;
			}
		}

		Action IActionOwner.Action
		{
			get
			{
				return this.m_action;
			}
		}

		List<string> IActionOwner.FieldsUsedInValueExpression
		{
			get
			{
				return this.m_fieldsUsedInValueExpression;
			}
			set
			{
				this.m_fieldsUsedInValueExpression = value;
			}
		}

		internal ExpressionInfo Type
		{
			get
			{
				return this.m_type;
			}
			set
			{
				this.m_type = value;
			}
		}

		internal ExpressionInfo Subtype
		{
			get
			{
				return this.m_subtype;
			}
			set
			{
				this.m_subtype = value;
			}
		}

		internal ChartEmptyPoints EmptyPoints
		{
			get
			{
				return this.m_emptyPoints;
			}
			set
			{
				this.m_emptyPoints = value;
			}
		}

		internal ExpressionInfo LegendName
		{
			get
			{
				return this.m_legendName;
			}
			set
			{
				this.m_legendName = value;
			}
		}

		internal ExpressionInfo LegendText
		{
			get
			{
				return this.m_legendText;
			}
			set
			{
				this.m_legendText = value;
			}
		}

		internal ExpressionInfo ChartAreaName
		{
			get
			{
				return this.m_chartAreaName;
			}
			set
			{
				this.m_chartAreaName = value;
			}
		}

		internal ExpressionInfo ValueAxisName
		{
			get
			{
				return this.m_valueAxisName;
			}
			set
			{
				this.m_valueAxisName = value;
			}
		}

		internal ExpressionInfo CategoryAxisName
		{
			get
			{
				return this.m_categoryAxisName;
			}
			set
			{
				this.m_categoryAxisName = value;
			}
		}

		internal ChartDataLabel DataLabel
		{
			get
			{
				return this.m_dataLabel;
			}
			set
			{
				this.m_dataLabel = value;
			}
		}

		internal ChartMarker Marker
		{
			get
			{
				return this.m_marker;
			}
			set
			{
				this.m_marker = value;
			}
		}

		public Style StyleClass
		{
			get
			{
				return this.m_styleClass;
			}
			set
			{
				this.m_styleClass = value;
			}
		}

		internal string Name
		{
			get
			{
				return this.m_name;
			}
			set
			{
				this.m_name = value;
			}
		}

		internal ExpressionInfo ToolTip
		{
			get
			{
				return this.m_toolTip;
			}
			set
			{
				this.m_toolTip = value;
			}
		}

		private ChartSeries SourceSeries
		{
			get
			{
				if (this.m_sourceSeries == null && this.m_parentDerivedSeries != null)
				{
					this.m_sourceSeries = this.m_parentDerivedSeries.SourceSeries;
				}
				return this.m_sourceSeries;
			}
		}

		internal ChartItemInLegend ChartItemInLegend
		{
			get
			{
				return this.m_chartItemInLegend;
			}
			set
			{
				this.m_chartItemInLegend = value;
			}
		}

		private ChartMember ParentChartMember
		{
			get
			{
				if (this.m_parentChartMember == null)
				{
					if (this.SourceSeries == null)
					{
						this.m_parentChartMember = this.m_chart.GetChartMember(this);
					}
					else
					{
						this.m_parentChartMember = this.SourceSeries.ParentChartMember;
					}
				}
				return this.m_parentChartMember;
			}
		}

		public override List<InstancePathItem> InstancePath
		{
			get
			{
				if (base.m_cachedInstancePath == null)
				{
					base.m_cachedInstancePath = new List<InstancePathItem>();
					if (this.ParentChartMember != null)
					{
						base.m_cachedInstancePath.AddRange(this.ParentChartMember.InstancePath);
					}
				}
				return base.m_cachedInstancePath;
			}
		}

		IInstancePath IStyleContainer.InstancePath
		{
			get
			{
				return this;
			}
		}

		IInstancePath ICustomPropertiesHolder.InstancePath
		{
			get
			{
				return this;
			}
		}

		AspNetCore.ReportingServices.ReportProcessing.ObjectType IStyleContainer.ObjectType
		{
			get
			{
				return AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart;
			}
		}

		string IStyleContainer.Name
		{
			get
			{
				return this.m_chart.Name;
			}
		}

		internal DataValueList CustomProperties
		{
			get
			{
				return this.m_customProperties;
			}
			set
			{
				this.m_customProperties = value;
			}
		}

		internal ChartSmartLabel ChartSmartLabel
		{
			get
			{
				return this.m_chartSmartLabel;
			}
			set
			{
				this.m_chartSmartLabel = value;
			}
		}

		DataValueList ICustomPropertiesHolder.CustomProperties
		{
			get
			{
				return this.m_customProperties;
			}
		}

		internal List<ChartDerivedSeries> ChildrenDerivedSeries
		{
			get
			{
				if (this.m_childrenDerivedSeries == null)
				{
					this.m_childrenDerivedSeries = this.m_chart.GetChildrenDerivedSeries(this.m_name);
				}
				return this.m_childrenDerivedSeries;
			}
		}

		internal ChartSeries()
		{
		}

		internal ChartSeries(Chart chart, int id)
			: base(id)
		{
			this.m_chart = chart;
		}

		internal ChartSeries(Chart chart, ChartDerivedSeries parentDerivedSeries, int id)
			: this(chart, id)
		{
			this.m_parentDerivedSeries = parentDerivedSeries;
		}

		internal void SetExprHost(ChartSeriesExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			this.m_exprHost = exprHost;
			this.m_exprHost.SetReportObjectModel(reportObjectModel);
			if (this.m_customProperties != null && this.m_exprHost.CustomPropertyHostsRemotable != null)
			{
				this.m_customProperties.SetExprHost(this.m_exprHost.CustomPropertyHostsRemotable, reportObjectModel);
			}
			if (this.m_action != null && this.m_exprHost.ActionInfoHost != null)
			{
				this.m_action.SetExprHost(this.m_exprHost.ActionInfoHost, reportObjectModel);
			}
			if (this.m_styleClass != null)
			{
				this.m_styleClass.SetStyleExprHost(this.m_exprHost);
			}
			if (this.m_chartSmartLabel != null && this.m_exprHost.SmartLabelHost != null)
			{
				this.m_chartSmartLabel.SetExprHost(this.m_exprHost.SmartLabelHost, reportObjectModel);
			}
			if (this.m_emptyPoints != null && this.m_exprHost.EmptyPointsHost != null)
			{
				this.m_emptyPoints.SetExprHost(this.m_exprHost.EmptyPointsHost, reportObjectModel);
			}
			if (this.m_action != null && this.m_exprHost.ActionInfoHost != null)
			{
				this.m_action.SetExprHost(this.m_exprHost.ActionInfoHost, reportObjectModel);
			}
			if (this.m_dataLabel != null && this.m_exprHost.DataLabelHost != null)
			{
				this.m_dataLabel.SetExprHost(this.m_exprHost.DataLabelHost, reportObjectModel);
			}
			if (this.m_marker != null && this.m_exprHost.ChartMarkerHost != null)
			{
				this.m_marker.SetExprHost(this.m_exprHost.ChartMarkerHost, reportObjectModel);
			}
			List<ChartDerivedSeries> childrenDerivedSeries = this.ChildrenDerivedSeries;
			IList<ChartDerivedSeriesExprHost> chartDerivedSeriesCollectionHostsRemotable = this.m_exprHost.ChartDerivedSeriesCollectionHostsRemotable;
			if (childrenDerivedSeries != null && chartDerivedSeriesCollectionHostsRemotable != null)
			{
				for (int i = 0; i < childrenDerivedSeries.Count; i++)
				{
					ChartDerivedSeries chartDerivedSeries = childrenDerivedSeries[i];
					if (chartDerivedSeries != null && chartDerivedSeries.ExpressionHostID > -1)
					{
						chartDerivedSeries.SetExprHost(chartDerivedSeriesCollectionHostsRemotable[chartDerivedSeries.ExpressionHostID], reportObjectModel);
					}
				}
			}
			if (this.m_chartItemInLegend != null && this.m_exprHost.DataPointInLegendHost != null)
			{
				this.m_chartItemInLegend.SetExprHost(this.m_exprHost.DataPointInLegendHost, reportObjectModel);
			}
		}

		internal void Initialize(InitializationContext context, string name)
		{
			context.ExprHostBuilder.ChartSeriesStart();
			if (this.m_customProperties != null)
			{
				this.m_customProperties.Initialize("ChartSeries" + name, context);
			}
			if (this.m_styleClass != null)
			{
				this.m_styleClass.Initialize(context);
			}
			if (this.m_action != null)
			{
				this.m_action.Initialize(context);
			}
			if (this.m_type == null)
			{
				this.m_type = ExpressionInfo.CreateConstExpression(ChartSeriesType.Column.ToString());
			}
			this.m_type.Initialize("Type", context);
			context.ExprHostBuilder.ChartSeriesType(this.m_type);
			if (this.m_subtype == null)
			{
				this.m_subtype = ExpressionInfo.CreateConstExpression(ChartSeriesSubtype.Plain.ToString());
			}
			this.m_subtype.Initialize("Subtype", context);
			context.ExprHostBuilder.ChartSeriesSubtype(this.m_subtype);
			if (this.m_chartSmartLabel != null)
			{
				this.m_chartSmartLabel.Initialize(context);
			}
			if (this.m_emptyPoints != null)
			{
				this.m_emptyPoints.Initialize(context);
			}
			if (this.m_legendName != null)
			{
				this.m_legendName.Initialize("LegendName", context);
				context.ExprHostBuilder.ChartSeriesLegendName(this.m_legendName);
			}
			if (this.m_legendText != null)
			{
				this.m_legendText.Initialize("LegendText", context);
				context.ExprHostBuilder.ChartSeriesLegendText(this.m_legendText);
			}
			if (this.m_chartAreaName != null)
			{
				this.m_chartAreaName.Initialize("ChartAreaName", context);
				context.ExprHostBuilder.ChartSeriesChartAreaName(this.m_chartAreaName);
			}
			if (this.m_valueAxisName != null)
			{
				this.m_valueAxisName.Initialize("ValueAxisName", context);
				context.ExprHostBuilder.ChartSeriesValueAxisName(this.m_valueAxisName);
			}
			if (this.m_categoryAxisName != null)
			{
				this.m_categoryAxisName.Initialize("CategoryAxisName", context);
				context.ExprHostBuilder.ChartSeriesCategoryAxisName(this.m_categoryAxisName);
			}
			if (this.m_hidden != null)
			{
				this.m_hidden.Initialize("Hidden", context);
				context.ExprHostBuilder.ChartSeriesHidden(this.m_hidden);
			}
			if (this.m_hideInLegend != null)
			{
				this.m_hideInLegend.Initialize("HideInLegend", context);
				context.ExprHostBuilder.ChartSeriesHideInLegend(this.m_hideInLegend);
			}
			if (this.m_dataLabel != null)
			{
				this.m_dataLabel.Initialize(context);
			}
			if (this.m_marker != null)
			{
				this.m_marker.Initialize(context);
			}
			List<ChartDerivedSeries> childrenDerivedSeries = this.ChildrenDerivedSeries;
			if (childrenDerivedSeries != null)
			{
				for (int i = 0; i < childrenDerivedSeries.Count; i++)
				{
					childrenDerivedSeries[i].Initialize(context, i);
				}
			}
			if (this.m_toolTip != null)
			{
				this.m_toolTip.Initialize("ToolTip", context);
				context.ExprHostBuilder.ChartSeriesToolTip(this.m_toolTip);
			}
			if (this.m_chartItemInLegend != null)
			{
				this.m_chartItemInLegend.Initialize(context);
			}
			context.ExprHostBuilder.ChartSeriesEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ChartSeries chartSeries = (ChartSeries)base.PublishClone(context);
			chartSeries.m_chart = (Chart)context.CurrentDataRegionClone;
			if (this.m_dataPoints != null)
			{
				chartSeries.m_dataPoints = new ChartDataPointList(this.m_dataPoints.Count);
				foreach (ChartDataPoint dataPoint in this.m_dataPoints)
				{
					chartSeries.m_dataPoints.Add((ChartDataPoint)dataPoint.PublishClone(context));
				}
			}
			if (this.m_customProperties != null)
			{
				chartSeries.m_customProperties = new DataValueList(this.m_customProperties.Count);
				foreach (DataValue customProperty in this.m_customProperties)
				{
					chartSeries.m_customProperties.Add((DataValue)customProperty.PublishClone(context));
				}
			}
			if (this.m_styleClass != null)
			{
				chartSeries.m_styleClass = (Style)this.m_styleClass.PublishClone(context);
			}
			if (this.m_action != null)
			{
				chartSeries.m_action = (Action)this.m_action.PublishClone(context);
			}
			if (this.m_type != null)
			{
				chartSeries.m_type = (ExpressionInfo)this.m_type.PublishClone(context);
			}
			if (this.m_subtype != null)
			{
				chartSeries.m_subtype = (ExpressionInfo)this.m_subtype.PublishClone(context);
			}
			if (this.m_emptyPoints != null)
			{
				chartSeries.m_emptyPoints = (ChartEmptyPoints)this.m_emptyPoints.PublishClone(context);
			}
			if (this.m_legendName != null)
			{
				chartSeries.m_legendName = (ExpressionInfo)this.m_legendName.PublishClone(context);
			}
			if (this.m_legendText != null)
			{
				chartSeries.m_legendText = (ExpressionInfo)this.m_legendText.PublishClone(context);
			}
			if (this.m_chartAreaName != null)
			{
				chartSeries.m_chartAreaName = (ExpressionInfo)this.m_chartAreaName.PublishClone(context);
			}
			if (this.m_valueAxisName != null)
			{
				chartSeries.m_valueAxisName = (ExpressionInfo)this.m_valueAxisName.PublishClone(context);
			}
			if (this.m_categoryAxisName != null)
			{
				chartSeries.m_categoryAxisName = (ExpressionInfo)this.m_categoryAxisName.PublishClone(context);
			}
			if (this.m_hidden != null)
			{
				chartSeries.m_hidden = (ExpressionInfo)this.m_hidden.PublishClone(context);
			}
			if (this.m_hideInLegend != null)
			{
				chartSeries.m_hideInLegend = (ExpressionInfo)this.m_hideInLegend.PublishClone(context);
			}
			if (this.m_chartSmartLabel != null)
			{
				chartSeries.m_chartSmartLabel = (ChartSmartLabel)this.m_chartSmartLabel.PublishClone(context);
			}
			if (this.m_dataLabel != null)
			{
				chartSeries.m_dataLabel = (ChartDataLabel)this.m_dataLabel.PublishClone(context);
			}
			if (this.m_marker != null)
			{
				chartSeries.m_marker = (ChartMarker)this.m_marker.PublishClone(context);
			}
			if (this.m_toolTip != null)
			{
				chartSeries.m_toolTip = (ExpressionInfo)this.m_toolTip.PublishClone(context);
			}
			if (this.m_chartItemInLegend != null)
			{
				chartSeries.m_chartItemInLegend = (ChartItemInLegend)this.m_chartItemInLegend.PublishClone(context);
			}
			return chartSeries;
		}

		internal ChartSeriesType EvaluateType(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			return EnumTranslator.TranslateChartSeriesType(context.ReportRuntime.EvaluateChartSeriesTypeExpression(this, this.m_chart.Name), context.ReportRuntime);
		}

		internal ChartSeriesSubtype EvaluateSubtype(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			return EnumTranslator.TranslateChartSeriesSubtype(context.ReportRuntime.EvaluateChartSeriesSubtypeExpression(this, this.m_chart.Name), context.ReportRuntime);
		}

		internal string EvaluateLegendName(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartSeriesLegendNameExpression(this, this.m_chart.Name);
		}

		internal string EvaluateLegendText(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			AspNetCore.ReportingServices.RdlExpressions.VariantResult variantResult = context.ReportRuntime.EvaluateChartSeriesLegendTextExpression(this, this.m_chart.Name);
			string result = null;
			if (variantResult.ErrorOccurred)
			{
				result = RPRes.rsExpressionErrorValue;
			}
			else if (variantResult.Value != null)
			{
				result = Formatter.Format(variantResult.Value, ref this.m_formatter, this.m_chart.StyleClass, this.m_styleClass, context, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, this.m_chart.Name);
			}
			return result;
		}

		internal string EvaluateChartAreaName(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartSeriesChartAreaNameExpression(this, this.m_chart.Name);
		}

		internal string EvaluateValueAxisName(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartSeriesValueAxisNameExpression(this, this.m_chart.Name);
		}

		internal string EvaluateCategoryAxisName(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartSeriesCategoryAxisNameExpression(this, this.m_chart.Name);
		}

		internal bool EvaluateHidden(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartSeriesHiddenExpression(this, this.m_chart.Name);
		}

		internal bool EvaluateHideInLegend(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartSeriesHideInLegendExpression(this, this.m_chart.Name);
		}

		internal string EvaluateToolTip(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			AspNetCore.ReportingServices.RdlExpressions.VariantResult variantResult = context.ReportRuntime.EvaluateChartSeriesToolTipExpression(this, this.m_chart.Name);
			string result = null;
			if (variantResult.ErrorOccurred)
			{
				result = RPRes.rsExpressionErrorValue;
			}
			else if (variantResult.Value != null)
			{
				result = Formatter.Format(variantResult.Value, ref this.m_formatter, this.m_chart.StyleClass, this.m_styleClass, context, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, this.m_chart.Name);
			}
			return result;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ChartDataPoints, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartDataPoint));
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.Action, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Action));
			list.Add(new MemberInfo(MemberName.Type, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Subtype, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.EmptyPoints, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartEmptyPoints));
			list.Add(new MemberInfo(MemberName.LegendName, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.LegendText, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ChartAreaName, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ValueAxisName, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.CategoryAxisName, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.StyleClass, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Style));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.Chart, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Chart, Token.Reference));
			list.Add(new MemberInfo(MemberName.Hidden, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.HideInLegend, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ChartSmartLabel, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartSmartLabel));
			list.Add(new MemberInfo(MemberName.CustomProperties, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataValue));
			list.Add(new MemberInfo(MemberName.DataLabel, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartDataLabel));
			list.Add(new MemberInfo(MemberName.Marker, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartMarker));
			list.Add(new MemberInfo(MemberName.ChartMember, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartMember, Token.Reference));
			list.Add(new MemberInfo(MemberName.SourceSeries, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartSeries, Token.Reference));
			list.Add(new MemberInfo(MemberName.ToolTip, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ChartItemInLegend, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartItemInLegend));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartSeries, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Row, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(ChartSeries.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Name:
					writer.Write(this.m_name);
					break;
				case MemberName.ChartDataPoints:
					writer.Write(this.m_dataPoints);
					break;
				case MemberName.ExprHostID:
					writer.Write(this.m_exprHostID);
					break;
				case MemberName.Chart:
					writer.WriteReference(this.m_chart);
					break;
				case MemberName.ChartMember:
					writer.WriteReference(this.ParentChartMember);
					break;
				case MemberName.SourceSeries:
					writer.WriteReference(this.SourceSeries);
					break;
				case MemberName.Action:
					writer.Write(this.m_action);
					break;
				case MemberName.Type:
					writer.Write(this.m_type);
					break;
				case MemberName.Subtype:
					writer.Write(this.m_subtype);
					break;
				case MemberName.EmptyPoints:
					writer.Write(this.m_emptyPoints);
					break;
				case MemberName.LegendName:
					writer.Write(this.m_legendName);
					break;
				case MemberName.LegendText:
					writer.Write(this.m_legendText);
					break;
				case MemberName.ChartAreaName:
					writer.Write(this.m_chartAreaName);
					break;
				case MemberName.ValueAxisName:
					writer.Write(this.m_valueAxisName);
					break;
				case MemberName.CategoryAxisName:
					writer.Write(this.m_categoryAxisName);
					break;
				case MemberName.StyleClass:
					writer.Write(this.m_styleClass);
					break;
				case MemberName.Hidden:
					writer.Write(this.m_hidden);
					break;
				case MemberName.HideInLegend:
					writer.Write(this.m_hideInLegend);
					break;
				case MemberName.ChartSmartLabel:
					writer.Write(this.m_chartSmartLabel);
					break;
				case MemberName.CustomProperties:
					writer.Write(this.m_customProperties);
					break;
				case MemberName.DataLabel:
					writer.Write(this.m_dataLabel);
					break;
				case MemberName.Marker:
					writer.Write(this.m_marker);
					break;
				case MemberName.ToolTip:
					writer.Write(this.m_toolTip);
					break;
				case MemberName.ChartItemInLegend:
					writer.Write(this.m_chartItemInLegend);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(ChartSeries.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Name:
					this.m_name = reader.ReadString();
					break;
				case MemberName.ChartDataPoints:
					this.m_dataPoints = reader.ReadListOfRIFObjects<ChartDataPointList>();
					break;
				case MemberName.ExprHostID:
					this.m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.Chart:
					this.m_chart = reader.ReadReference<Chart>(this);
					break;
				case MemberName.ChartMember:
					this.m_parentChartMember = reader.ReadReference<ChartMember>(this);
					break;
				case MemberName.SourceSeries:
					this.m_sourceSeries = reader.ReadReference<ChartSeries>(this);
					break;
				case MemberName.Action:
					this.m_action = (Action)reader.ReadRIFObject();
					break;
				case MemberName.Type:
					this.m_type = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Subtype:
					this.m_subtype = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.EmptyPoints:
					this.m_emptyPoints = (ChartEmptyPoints)reader.ReadRIFObject();
					break;
				case MemberName.LegendName:
					this.m_legendName = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.LegendText:
					this.m_legendText = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ChartAreaName:
					this.m_chartAreaName = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ValueAxisName:
					this.m_valueAxisName = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.CategoryAxisName:
					this.m_categoryAxisName = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.StyleClass:
					this.m_styleClass = (Style)reader.ReadRIFObject();
					break;
				case MemberName.Hidden:
					this.m_hidden = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.HideInLegend:
					this.m_hideInLegend = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ChartSmartLabel:
					this.m_chartSmartLabel = (ChartSmartLabel)reader.ReadRIFObject();
					break;
				case MemberName.CustomProperties:
					this.m_customProperties = reader.ReadListOfRIFObjects<DataValueList>();
					break;
				case MemberName.DataLabel:
					this.m_dataLabel = (ChartDataLabel)reader.ReadRIFObject();
					break;
				case MemberName.Marker:
					this.m_marker = (ChartMarker)reader.ReadRIFObject();
					break;
				case MemberName.ToolTip:
					this.m_toolTip = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ChartItemInLegend:
					this.m_chartItemInLegend = (ChartItemInLegend)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			List<MemberReference> list = default(List<MemberReference>);
			if (memberReferencesCollection.TryGetValue(ChartSeries.m_Declaration.ObjectType, out list))
			{
				foreach (MemberReference item in list)
				{
					switch (item.MemberName)
					{
					case MemberName.Chart:
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						this.m_chart = (Chart)referenceableItems[item.RefID];
						break;
					case MemberName.ChartMember:
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						this.m_parentChartMember = (ChartMember)referenceableItems[item.RefID];
						break;
					case MemberName.ChartSeries:
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						this.m_sourceSeries = (ChartSeries)referenceableItems[item.RefID];
						break;
					default:
						Global.Tracer.Assert(false);
						break;
					}
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartSeries;
		}
	}
}
