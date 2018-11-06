using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
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
	internal sealed class ChartSmartLabel : IPersistable
	{
		[Reference]
		private Chart m_chart;

		[Reference]
		private ChartSeries m_chartSeries;

		private ExpressionInfo m_allowOutSidePlotArea;

		private ExpressionInfo m_calloutBackColor;

		private ExpressionInfo m_calloutLineAnchor;

		private ExpressionInfo m_calloutLineColor;

		private ExpressionInfo m_calloutLineStyle;

		private ExpressionInfo m_calloutLineWidth;

		private ExpressionInfo m_calloutStyle;

		private ExpressionInfo m_showOverlapped;

		private ExpressionInfo m_markerOverlapping;

		private ExpressionInfo m_maxMovingDistance;

		private ExpressionInfo m_minMovingDistance;

		private ChartNoMoveDirections m_noMoveDirections;

		private ExpressionInfo m_disabled;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ChartSmartLabel.GetDeclaration();

		[NonSerialized]
		private ChartSmartLabelExprHost m_exprHost;

		internal ChartSmartLabelExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal ExpressionInfo AllowOutSidePlotArea
		{
			get
			{
				return this.m_allowOutSidePlotArea;
			}
			set
			{
				this.m_allowOutSidePlotArea = value;
			}
		}

		internal ExpressionInfo CalloutBackColor
		{
			get
			{
				return this.m_calloutBackColor;
			}
			set
			{
				this.m_calloutBackColor = value;
			}
		}

		internal ExpressionInfo CalloutLineAnchor
		{
			get
			{
				return this.m_calloutLineAnchor;
			}
			set
			{
				this.m_calloutLineAnchor = value;
			}
		}

		internal ExpressionInfo CalloutLineColor
		{
			get
			{
				return this.m_calloutLineColor;
			}
			set
			{
				this.m_calloutLineColor = value;
			}
		}

		internal ExpressionInfo CalloutLineStyle
		{
			get
			{
				return this.m_calloutLineStyle;
			}
			set
			{
				this.m_calloutLineStyle = value;
			}
		}

		internal ExpressionInfo CalloutLineWidth
		{
			get
			{
				return this.m_calloutLineWidth;
			}
			set
			{
				this.m_calloutLineWidth = value;
			}
		}

		internal ExpressionInfo CalloutStyle
		{
			get
			{
				return this.m_calloutStyle;
			}
			set
			{
				this.m_calloutStyle = value;
			}
		}

		internal ExpressionInfo ShowOverlapped
		{
			get
			{
				return this.m_showOverlapped;
			}
			set
			{
				this.m_showOverlapped = value;
			}
		}

		internal ExpressionInfo MarkerOverlapping
		{
			get
			{
				return this.m_markerOverlapping;
			}
			set
			{
				this.m_markerOverlapping = value;
			}
		}

		internal ExpressionInfo MaxMovingDistance
		{
			get
			{
				return this.m_maxMovingDistance;
			}
			set
			{
				this.m_maxMovingDistance = value;
			}
		}

		internal ExpressionInfo MinMovingDistance
		{
			get
			{
				return this.m_minMovingDistance;
			}
			set
			{
				this.m_minMovingDistance = value;
			}
		}

		internal ChartNoMoveDirections NoMoveDirections
		{
			get
			{
				return this.m_noMoveDirections;
			}
			set
			{
				this.m_noMoveDirections = value;
			}
		}

		internal ExpressionInfo Disabled
		{
			get
			{
				return this.m_disabled;
			}
			set
			{
				this.m_disabled = value;
			}
		}

		private IInstancePath InstancePath
		{
			get
			{
				if (this.m_chartSeries != null)
				{
					return this.m_chartSeries;
				}
				return this.m_chart;
			}
		}

		internal ChartSmartLabel()
		{
		}

		internal ChartSmartLabel(Chart chart, ChartSeries chartSeries)
		{
			this.m_chart = chart;
			this.m_chartSeries = chartSeries;
		}

		internal void SetExprHost(ChartSmartLabelExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			this.m_exprHost = exprHost;
			this.m_exprHost.SetReportObjectModel(reportObjectModel);
			if (this.m_noMoveDirections != null && this.m_exprHost.NoMoveDirectionsHost != null)
			{
				this.m_noMoveDirections.SetExprHost(this.m_exprHost.NoMoveDirectionsHost, reportObjectModel);
			}
		}

		internal void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.ChartSmartLabelStart();
			if (this.m_allowOutSidePlotArea != null)
			{
				this.m_allowOutSidePlotArea.Initialize("AllowOutSidePlotArea", context);
				context.ExprHostBuilder.ChartSmartLabelAllowOutSidePlotArea(this.m_allowOutSidePlotArea);
			}
			if (this.m_calloutBackColor != null)
			{
				this.m_calloutBackColor.Initialize("CalloutBackColor", context);
				context.ExprHostBuilder.ChartSmartLabelCalloutBackColor(this.m_calloutBackColor);
			}
			if (this.m_calloutLineAnchor != null)
			{
				this.m_calloutLineAnchor.Initialize("CalloutLineAnchor", context);
				context.ExprHostBuilder.ChartSmartLabelCalloutLineAnchor(this.m_calloutLineAnchor);
			}
			if (this.m_calloutLineColor != null)
			{
				this.m_calloutLineColor.Initialize("CalloutLineColor", context);
				context.ExprHostBuilder.ChartSmartLabelCalloutLineColor(this.m_calloutLineColor);
			}
			if (this.m_calloutLineStyle != null)
			{
				this.m_calloutLineStyle.Initialize("CalloutLineStyle", context);
				context.ExprHostBuilder.ChartSmartLabelCalloutLineStyle(this.m_calloutLineStyle);
			}
			if (this.m_calloutLineWidth != null)
			{
				this.m_calloutLineWidth.Initialize("CalloutLineWidth", context);
				context.ExprHostBuilder.ChartSmartLabelCalloutLineWidth(this.m_calloutLineWidth);
			}
			if (this.m_calloutStyle != null)
			{
				this.m_calloutStyle.Initialize("CalloutStyle", context);
				context.ExprHostBuilder.ChartSmartLabelCalloutStyle(this.m_calloutStyle);
			}
			if (this.m_showOverlapped != null)
			{
				this.m_showOverlapped.Initialize("ShowOverlapped", context);
				context.ExprHostBuilder.ChartSmartLabelShowOverlapped(this.m_showOverlapped);
			}
			if (this.m_markerOverlapping != null)
			{
				this.m_markerOverlapping.Initialize("MarkerOverlapping", context);
				context.ExprHostBuilder.ChartSmartLabelMarkerOverlapping(this.m_markerOverlapping);
			}
			if (this.m_maxMovingDistance != null)
			{
				this.m_maxMovingDistance.Initialize("MaxMovingDistance", context);
				context.ExprHostBuilder.ChartSmartLabelMaxMovingDistance(this.m_maxMovingDistance);
			}
			if (this.m_minMovingDistance != null)
			{
				this.m_minMovingDistance.Initialize("MinMovingDistance", context);
				context.ExprHostBuilder.ChartSmartLabelMinMovingDistance(this.m_minMovingDistance);
			}
			if (this.m_noMoveDirections != null)
			{
				this.m_noMoveDirections.Initialize(context);
			}
			if (this.m_disabled != null)
			{
				this.m_disabled.Initialize("Disabled", context);
				context.ExprHostBuilder.ChartSmartLabelDisabled(this.m_disabled);
			}
			context.ExprHostBuilder.ChartSmartLabelEnd();
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			ChartSmartLabel chartSmartLabel = (ChartSmartLabel)base.MemberwiseClone();
			chartSmartLabel.m_chart = (Chart)context.CurrentDataRegionClone;
			if (this.m_allowOutSidePlotArea != null)
			{
				chartSmartLabel.m_allowOutSidePlotArea = (ExpressionInfo)this.m_allowOutSidePlotArea.PublishClone(context);
			}
			if (this.m_calloutBackColor != null)
			{
				chartSmartLabel.m_calloutBackColor = (ExpressionInfo)this.m_calloutBackColor.PublishClone(context);
			}
			if (this.m_calloutLineAnchor != null)
			{
				chartSmartLabel.m_calloutLineAnchor = (ExpressionInfo)this.m_calloutLineAnchor.PublishClone(context);
			}
			if (this.m_calloutLineColor != null)
			{
				chartSmartLabel.m_calloutLineColor = (ExpressionInfo)this.m_calloutLineColor.PublishClone(context);
			}
			if (this.m_calloutLineStyle != null)
			{
				chartSmartLabel.m_calloutLineStyle = (ExpressionInfo)this.m_calloutLineStyle.PublishClone(context);
			}
			if (this.m_calloutLineWidth != null)
			{
				chartSmartLabel.m_calloutLineWidth = (ExpressionInfo)this.m_calloutLineWidth.PublishClone(context);
			}
			if (this.m_calloutStyle != null)
			{
				chartSmartLabel.m_calloutStyle = (ExpressionInfo)this.m_calloutStyle.PublishClone(context);
			}
			if (this.m_showOverlapped != null)
			{
				chartSmartLabel.m_showOverlapped = (ExpressionInfo)this.m_showOverlapped.PublishClone(context);
			}
			if (this.m_markerOverlapping != null)
			{
				chartSmartLabel.m_markerOverlapping = (ExpressionInfo)this.m_markerOverlapping.PublishClone(context);
			}
			if (this.m_maxMovingDistance != null)
			{
				chartSmartLabel.m_maxMovingDistance = (ExpressionInfo)this.m_maxMovingDistance.PublishClone(context);
			}
			if (this.m_minMovingDistance != null)
			{
				chartSmartLabel.m_minMovingDistance = (ExpressionInfo)this.m_minMovingDistance.PublishClone(context);
			}
			if (this.m_noMoveDirections != null)
			{
				chartSmartLabel.m_noMoveDirections = (ChartNoMoveDirections)this.m_noMoveDirections.PublishClone(context);
			}
			if (this.m_disabled != null)
			{
				chartSmartLabel.m_disabled = (ExpressionInfo)this.m_disabled.PublishClone(context);
			}
			return chartSmartLabel;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.AllowOutSidePlotArea, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.CalloutBackColor, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.CalloutLineAnchor, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.CalloutLineColor, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.CalloutLineStyle, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.CalloutLineWidth, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.CalloutStyle, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ShowOverlapped, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MarkerOverlapping, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MaxMovingDistance, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MinMovingDistance, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.NoMoveDirections, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartNoMoveDirections));
			list.Add(new MemberInfo(MemberName.Chart, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Chart, Token.Reference));
			list.Add(new MemberInfo(MemberName.ChartSeries, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartSeries, Token.Reference));
			list.Add(new MemberInfo(MemberName.Disabled, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartSmartLabel, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		internal ChartAllowOutsideChartArea EvaluateAllowOutSidePlotArea(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.InstancePath, reportScopeInstance);
			return EnumTranslator.TranslateChartAllowOutsideChartArea(context.ReportRuntime.EvaluateChartSmartLabelAllowOutSidePlotAreaExpression(this, this.m_chart.Name), context.ReportRuntime);
		}

		internal string EvaluateCalloutBackColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartSmartLabelCalloutBackColorExpression(this, this.m_chart.Name);
		}

		internal ChartCalloutLineAnchor EvaluateCalloutLineAnchor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.InstancePath, reportScopeInstance);
			return EnumTranslator.TranslateChartCalloutLineAnchor(context.ReportRuntime.EvaluateChartSmartLabelCalloutLineAnchorExpression(this, this.m_chart.Name), context.ReportRuntime);
		}

		internal string EvaluateCalloutLineColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartSmartLabelCalloutLineColorExpression(this, this.m_chart.Name);
		}

		internal ChartCalloutLineStyle EvaluateCalloutLineStyle(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.InstancePath, reportScopeInstance);
			return EnumTranslator.TranslateChartCalloutLineStyle(context.ReportRuntime.EvaluateChartSmartLabelCalloutLineStyleExpression(this, this.m_chart.Name), context.ReportRuntime);
		}

		internal string EvaluateCalloutLineWidth(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartSmartLabelCalloutLineWidthExpression(this, this.m_chart.Name);
		}

		internal ChartCalloutStyle EvaluateCalloutStyle(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.InstancePath, reportScopeInstance);
			return EnumTranslator.TranslateChartCalloutStyle(context.ReportRuntime.EvaluateChartSmartLabelCalloutStyleExpression(this, this.m_chart.Name), context.ReportRuntime);
		}

		internal bool EvaluateShowOverlapped(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartSmartLabelShowOverlappedExpression(this, this.m_chart.Name);
		}

		internal bool EvaluateMarkerOverlapping(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartSmartLabelMarkerOverlappingExpression(this, this.m_chart.Name);
		}

		internal string EvaluateMaxMovingDistance(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartSmartLabelMaxMovingDistanceExpression(this, this.m_chart.Name);
		}

		internal string EvaluateMinMovingDistance(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartSmartLabelMinMovingDistanceExpression(this, this.m_chart.Name);
		}

		internal bool EvaluateDisabled(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartSmartLabelDisabledExpression(this, this.m_chart.Name);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(ChartSmartLabel.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Chart:
					writer.WriteReference(this.m_chart);
					break;
				case MemberName.ChartSeries:
					writer.WriteReference(this.m_chartSeries);
					break;
				case MemberName.AllowOutSidePlotArea:
					writer.Write(this.m_allowOutSidePlotArea);
					break;
				case MemberName.CalloutBackColor:
					writer.Write(this.m_calloutBackColor);
					break;
				case MemberName.CalloutLineAnchor:
					writer.Write(this.m_calloutLineAnchor);
					break;
				case MemberName.CalloutLineColor:
					writer.Write(this.m_calloutLineColor);
					break;
				case MemberName.CalloutLineStyle:
					writer.Write(this.m_calloutLineStyle);
					break;
				case MemberName.CalloutLineWidth:
					writer.Write(this.m_calloutLineWidth);
					break;
				case MemberName.CalloutStyle:
					writer.Write(this.m_calloutStyle);
					break;
				case MemberName.ShowOverlapped:
					writer.Write(this.m_showOverlapped);
					break;
				case MemberName.MarkerOverlapping:
					writer.Write(this.m_markerOverlapping);
					break;
				case MemberName.MaxMovingDistance:
					writer.Write(this.m_maxMovingDistance);
					break;
				case MemberName.MinMovingDistance:
					writer.Write(this.m_minMovingDistance);
					break;
				case MemberName.NoMoveDirections:
					writer.Write(this.m_noMoveDirections);
					break;
				case MemberName.Disabled:
					writer.Write(this.m_disabled);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(ChartSmartLabel.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Chart:
					this.m_chart = reader.ReadReference<Chart>(this);
					break;
				case MemberName.ChartSeries:
					this.m_chartSeries = reader.ReadReference<ChartSeries>(this);
					break;
				case MemberName.AllowOutSidePlotArea:
					this.m_allowOutSidePlotArea = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.CalloutBackColor:
					this.m_calloutBackColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.CalloutLineAnchor:
					this.m_calloutLineAnchor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.CalloutLineColor:
					this.m_calloutLineColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.CalloutLineStyle:
					this.m_calloutLineStyle = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.CalloutLineWidth:
					this.m_calloutLineWidth = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.CalloutStyle:
					this.m_calloutStyle = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ShowOverlapped:
					this.m_showOverlapped = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MarkerOverlapping:
					this.m_markerOverlapping = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MaxMovingDistance:
					this.m_maxMovingDistance = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MinMovingDistance:
					this.m_minMovingDistance = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.NoMoveDirections:
					this.m_noMoveDirections = (ChartNoMoveDirections)reader.ReadRIFObject();
					break;
				case MemberName.Disabled:
					this.m_disabled = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			List<MemberReference> list = default(List<MemberReference>);
			if (memberReferencesCollection.TryGetValue(ChartSmartLabel.m_Declaration.ObjectType, out list))
			{
				foreach (MemberReference item in list)
				{
					switch (item.MemberName)
					{
					case MemberName.Chart:
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						this.m_chart = (Chart)referenceableItems[item.RefID];
						break;
					case MemberName.ChartSeries:
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						this.m_chartSeries = (ChartSeries)referenceableItems[item.RefID];
						break;
					default:
						Global.Tracer.Assert(false);
						break;
					}
				}
			}
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartSmartLabel;
		}
	}
}
