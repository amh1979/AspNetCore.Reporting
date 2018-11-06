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
	internal sealed class MapColorScale : MapDockableSubItem, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = MapColorScale.GetDeclaration();

		private MapColorScaleTitle m_mapColorScaleTitle;

		private ExpressionInfo m_tickMarkLength;

		private ExpressionInfo m_colorBarBorderColor;

		private ExpressionInfo m_labelInterval;

		private ExpressionInfo m_labelFormat;

		private ExpressionInfo m_labelPlacement;

		private ExpressionInfo m_labelBehavior;

		private ExpressionInfo m_hideEndLabels;

		private ExpressionInfo m_rangeGapColor;

		private ExpressionInfo m_noDataText;

		internal MapColorScaleTitle MapColorScaleTitle
		{
			get
			{
				return this.m_mapColorScaleTitle;
			}
			set
			{
				this.m_mapColorScaleTitle = value;
			}
		}

		internal ExpressionInfo TickMarkLength
		{
			get
			{
				return this.m_tickMarkLength;
			}
			set
			{
				this.m_tickMarkLength = value;
			}
		}

		internal ExpressionInfo ColorBarBorderColor
		{
			get
			{
				return this.m_colorBarBorderColor;
			}
			set
			{
				this.m_colorBarBorderColor = value;
			}
		}

		internal ExpressionInfo LabelInterval
		{
			get
			{
				return this.m_labelInterval;
			}
			set
			{
				this.m_labelInterval = value;
			}
		}

		internal ExpressionInfo LabelFormat
		{
			get
			{
				return this.m_labelFormat;
			}
			set
			{
				this.m_labelFormat = value;
			}
		}

		internal ExpressionInfo LabelPlacement
		{
			get
			{
				return this.m_labelPlacement;
			}
			set
			{
				this.m_labelPlacement = value;
			}
		}

		internal ExpressionInfo LabelBehavior
		{
			get
			{
				return this.m_labelBehavior;
			}
			set
			{
				this.m_labelBehavior = value;
			}
		}

		internal ExpressionInfo HideEndLabels
		{
			get
			{
				return this.m_hideEndLabels;
			}
			set
			{
				this.m_hideEndLabels = value;
			}
		}

		internal ExpressionInfo RangeGapColor
		{
			get
			{
				return this.m_rangeGapColor;
			}
			set
			{
				this.m_rangeGapColor = value;
			}
		}

		internal ExpressionInfo NoDataText
		{
			get
			{
				return this.m_noDataText;
			}
			set
			{
				this.m_noDataText = value;
			}
		}

		internal new MapColorScaleExprHost ExprHost
		{
			get
			{
				return (MapColorScaleExprHost)base.m_exprHost;
			}
		}

		internal MapColorScale()
		{
		}

		internal MapColorScale(Map map, int id)
			: base(map, id)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapColorScaleStart();
			base.Initialize(context);
			if (this.m_mapColorScaleTitle != null)
			{
				this.m_mapColorScaleTitle.Initialize(context);
			}
			if (this.m_tickMarkLength != null)
			{
				this.m_tickMarkLength.Initialize("TickMarkLength", context);
				context.ExprHostBuilder.MapColorScaleTickMarkLength(this.m_tickMarkLength);
			}
			if (this.m_colorBarBorderColor != null)
			{
				this.m_colorBarBorderColor.Initialize("ColorBarBorderColor", context);
				context.ExprHostBuilder.MapColorScaleColorBarBorderColor(this.m_colorBarBorderColor);
			}
			if (this.m_labelInterval != null)
			{
				this.m_labelInterval.Initialize("LabelInterval", context);
				context.ExprHostBuilder.MapColorScaleLabelInterval(this.m_labelInterval);
			}
			if (this.m_labelFormat != null)
			{
				this.m_labelFormat.Initialize("LabelFormat", context);
				context.ExprHostBuilder.MapColorScaleLabelFormat(this.m_labelFormat);
			}
			if (this.m_labelPlacement != null)
			{
				this.m_labelPlacement.Initialize("LabelPlacement", context);
				context.ExprHostBuilder.MapColorScaleLabelPlacement(this.m_labelPlacement);
			}
			if (this.m_labelBehavior != null)
			{
				this.m_labelBehavior.Initialize("LabelBehavior", context);
				context.ExprHostBuilder.MapColorScaleLabelBehavior(this.m_labelBehavior);
			}
			if (this.m_hideEndLabels != null)
			{
				this.m_hideEndLabels.Initialize("HideEndLabels", context);
				context.ExprHostBuilder.MapColorScaleHideEndLabels(this.m_hideEndLabels);
			}
			if (this.m_rangeGapColor != null)
			{
				this.m_rangeGapColor.Initialize("RangeGapColor", context);
				context.ExprHostBuilder.MapColorScaleRangeGapColor(this.m_rangeGapColor);
			}
			if (this.m_noDataText != null)
			{
				this.m_noDataText.Initialize("NoDataText", context);
				context.ExprHostBuilder.MapColorScaleNoDataText(this.m_noDataText);
			}
			context.ExprHostBuilder.MapColorScaleEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapColorScale mapColorScale = (MapColorScale)base.PublishClone(context);
			if (this.m_mapColorScaleTitle != null)
			{
				mapColorScale.m_mapColorScaleTitle = (MapColorScaleTitle)this.m_mapColorScaleTitle.PublishClone(context);
			}
			if (this.m_tickMarkLength != null)
			{
				mapColorScale.m_tickMarkLength = (ExpressionInfo)this.m_tickMarkLength.PublishClone(context);
			}
			if (this.m_colorBarBorderColor != null)
			{
				mapColorScale.m_colorBarBorderColor = (ExpressionInfo)this.m_colorBarBorderColor.PublishClone(context);
			}
			if (this.m_labelInterval != null)
			{
				mapColorScale.m_labelInterval = (ExpressionInfo)this.m_labelInterval.PublishClone(context);
			}
			if (this.m_labelFormat != null)
			{
				mapColorScale.m_labelFormat = (ExpressionInfo)this.m_labelFormat.PublishClone(context);
			}
			if (this.m_labelPlacement != null)
			{
				mapColorScale.m_labelPlacement = (ExpressionInfo)this.m_labelPlacement.PublishClone(context);
			}
			if (this.m_labelBehavior != null)
			{
				mapColorScale.m_labelBehavior = (ExpressionInfo)this.m_labelBehavior.PublishClone(context);
			}
			if (this.m_hideEndLabels != null)
			{
				mapColorScale.m_hideEndLabels = (ExpressionInfo)this.m_hideEndLabels.PublishClone(context);
			}
			if (this.m_rangeGapColor != null)
			{
				mapColorScale.m_rangeGapColor = (ExpressionInfo)this.m_rangeGapColor.PublishClone(context);
			}
			if (this.m_noDataText != null)
			{
				mapColorScale.m_noDataText = (ExpressionInfo)this.m_noDataText.PublishClone(context);
			}
			return mapColorScale;
		}

		internal void SetExprHost(MapColorScaleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			if (this.m_mapColorScaleTitle != null && this.ExprHost.MapColorScaleTitleHost != null)
			{
				this.m_mapColorScaleTitle.SetExprHost(this.ExprHost.MapColorScaleTitleHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.MapColorScaleTitle, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapColorScaleTitle));
			list.Add(new MemberInfo(MemberName.TickMarkLength, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ColorBarBorderColor, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.LabelInterval, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.LabelFormat, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.LabelPlacement, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.LabelBehavior, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.HideEndLabels, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.RangeGapColor, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.NoDataText, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapColorScale, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapDockableSubItem, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(MapColorScale.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.MapColorScaleTitle:
					writer.Write(this.m_mapColorScaleTitle);
					break;
				case MemberName.TickMarkLength:
					writer.Write(this.m_tickMarkLength);
					break;
				case MemberName.ColorBarBorderColor:
					writer.Write(this.m_colorBarBorderColor);
					break;
				case MemberName.LabelInterval:
					writer.Write(this.m_labelInterval);
					break;
				case MemberName.LabelFormat:
					writer.Write(this.m_labelFormat);
					break;
				case MemberName.LabelPlacement:
					writer.Write(this.m_labelPlacement);
					break;
				case MemberName.LabelBehavior:
					writer.Write(this.m_labelBehavior);
					break;
				case MemberName.HideEndLabels:
					writer.Write(this.m_hideEndLabels);
					break;
				case MemberName.RangeGapColor:
					writer.Write(this.m_rangeGapColor);
					break;
				case MemberName.NoDataText:
					writer.Write(this.m_noDataText);
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
			reader.RegisterDeclaration(MapColorScale.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.MapColorScaleTitle:
					this.m_mapColorScaleTitle = (MapColorScaleTitle)reader.ReadRIFObject();
					break;
				case MemberName.TickMarkLength:
					this.m_tickMarkLength = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ColorBarBorderColor:
					this.m_colorBarBorderColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.LabelInterval:
					this.m_labelInterval = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.LabelFormat:
					this.m_labelFormat = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.LabelPlacement:
					this.m_labelPlacement = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.LabelBehavior:
					this.m_labelBehavior = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.HideEndLabels:
					this.m_hideEndLabels = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.RangeGapColor:
					this.m_rangeGapColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.NoDataText:
					this.m_noDataText = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapColorScale;
		}

		internal string EvaluateTickMarkLength(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapColorScaleTickMarkLengthExpression(this, base.m_map.Name);
		}

		internal string EvaluateColorBarBorderColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapColorScaleColorBarBorderColorExpression(this, base.m_map.Name);
		}

		internal int EvaluateLabelInterval(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapColorScaleLabelIntervalExpression(this, base.m_map.Name);
		}

		internal string EvaluateLabelFormat(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapColorScaleLabelFormatExpression(this, base.m_map.Name);
		}

		internal MapLabelPlacement EvaluateLabelPlacement(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return EnumTranslator.TranslateLabelPlacement(context.ReportRuntime.EvaluateMapColorScaleLabelPlacementExpression(this, base.m_map.Name), context.ReportRuntime);
		}

		internal MapLabelBehavior EvaluateLabelBehavior(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return EnumTranslator.TranslateLabelBehavior(context.ReportRuntime.EvaluateMapColorScaleLabelBehaviorExpression(this, base.m_map.Name), context.ReportRuntime);
		}

		internal bool EvaluateHideEndLabels(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapColorScaleHideEndLabelsExpression(this, base.m_map.Name);
		}

		internal string EvaluateRangeGapColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapColorScaleRangeGapColorExpression(this, base.m_map.Name);
		}

		internal string EvaluateNoDataText(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapColorScaleNoDataTextExpression(this, base.m_map.Name);
		}
	}
}
