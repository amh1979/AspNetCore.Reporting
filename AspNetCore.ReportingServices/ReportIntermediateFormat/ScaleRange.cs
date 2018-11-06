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
	internal sealed class ScaleRange : GaugePanelStyleContainer, IPersistable, IActionOwner
	{
		private Action m_action;

		private int m_exprHostID;

		[NonSerialized]
		private List<string> m_fieldsUsedInValueExpression;

		[NonSerialized]
		private ScaleRangeExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ScaleRange.GetDeclaration();

		private string m_name;

		private ExpressionInfo m_distanceFromScale;

		private GaugeInputValue m_startValue;

		private GaugeInputValue m_endValue;

		private ExpressionInfo m_startWidth;

		private ExpressionInfo m_endWidth;

		private ExpressionInfo m_inRangeBarPointerColor;

		private ExpressionInfo m_inRangeLabelColor;

		private ExpressionInfo m_inRangeTickMarksColor;

		private ExpressionInfo m_backgroundGradientType;

		private ExpressionInfo m_placement;

		private ExpressionInfo m_toolTip;

		private ExpressionInfo m_hidden;

		private int m_id;

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

		internal int ID
		{
			get
			{
				return this.m_id;
			}
		}

		internal ExpressionInfo DistanceFromScale
		{
			get
			{
				return this.m_distanceFromScale;
			}
			set
			{
				this.m_distanceFromScale = value;
			}
		}

		internal GaugeInputValue StartValue
		{
			get
			{
				return this.m_startValue;
			}
			set
			{
				this.m_startValue = value;
			}
		}

		internal GaugeInputValue EndValue
		{
			get
			{
				return this.m_endValue;
			}
			set
			{
				this.m_endValue = value;
			}
		}

		internal ExpressionInfo StartWidth
		{
			get
			{
				return this.m_startWidth;
			}
			set
			{
				this.m_startWidth = value;
			}
		}

		internal ExpressionInfo EndWidth
		{
			get
			{
				return this.m_endWidth;
			}
			set
			{
				this.m_endWidth = value;
			}
		}

		internal ExpressionInfo InRangeBarPointerColor
		{
			get
			{
				return this.m_inRangeBarPointerColor;
			}
			set
			{
				this.m_inRangeBarPointerColor = value;
			}
		}

		internal ExpressionInfo InRangeLabelColor
		{
			get
			{
				return this.m_inRangeLabelColor;
			}
			set
			{
				this.m_inRangeLabelColor = value;
			}
		}

		internal ExpressionInfo InRangeTickMarksColor
		{
			get
			{
				return this.m_inRangeTickMarksColor;
			}
			set
			{
				this.m_inRangeTickMarksColor = value;
			}
		}

		internal ExpressionInfo BackgroundGradientType
		{
			get
			{
				return this.m_backgroundGradientType;
			}
			set
			{
				this.m_backgroundGradientType = value;
			}
		}

		internal ExpressionInfo Placement
		{
			get
			{
				return this.m_placement;
			}
			set
			{
				this.m_placement = value;
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

		internal string OwnerName
		{
			get
			{
				return base.m_gaugePanel.Name;
			}
		}

		internal ScaleRangeExprHost ExprHost
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

		internal ScaleRange()
		{
		}

		internal ScaleRange(GaugePanel gaugePanel, int id)
			: base(gaugePanel)
		{
			this.m_id = id;
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.ScaleRangeStart(this.m_name);
			base.Initialize(context);
			if (this.m_action != null)
			{
				this.m_action.Initialize(context);
			}
			if (this.m_distanceFromScale != null)
			{
				this.m_distanceFromScale.Initialize("DistanceFromScale", context);
				context.ExprHostBuilder.ScaleRangeDistanceFromScale(this.m_distanceFromScale);
			}
			if (this.m_startWidth != null)
			{
				this.m_startWidth.Initialize("StartWidth", context);
				context.ExprHostBuilder.ScaleRangeStartWidth(this.m_startWidth);
			}
			if (this.m_endWidth != null)
			{
				this.m_endWidth.Initialize("EndWidth", context);
				context.ExprHostBuilder.ScaleRangeEndWidth(this.m_endWidth);
			}
			if (this.m_inRangeBarPointerColor != null)
			{
				this.m_inRangeBarPointerColor.Initialize("InRangeBarPointerColor", context);
				context.ExprHostBuilder.ScaleRangeInRangeBarPointerColor(this.m_inRangeBarPointerColor);
			}
			if (this.m_inRangeLabelColor != null)
			{
				this.m_inRangeLabelColor.Initialize("InRangeLabelColor", context);
				context.ExprHostBuilder.ScaleRangeInRangeLabelColor(this.m_inRangeLabelColor);
			}
			if (this.m_inRangeTickMarksColor != null)
			{
				this.m_inRangeTickMarksColor.Initialize("InRangeTickMarksColor", context);
				context.ExprHostBuilder.ScaleRangeInRangeTickMarksColor(this.m_inRangeTickMarksColor);
			}
			if (this.m_backgroundGradientType != null)
			{
				this.m_backgroundGradientType.Initialize("BackgroundGradientType", context);
				context.ExprHostBuilder.ScaleRangeBackgroundGradientType(this.m_backgroundGradientType);
			}
			if (this.m_placement != null)
			{
				this.m_placement.Initialize("Placement", context);
				context.ExprHostBuilder.ScaleRangePlacement(this.m_placement);
			}
			if (this.m_toolTip != null)
			{
				this.m_toolTip.Initialize("ToolTip", context);
				context.ExprHostBuilder.ScaleRangeToolTip(this.m_toolTip);
			}
			if (this.m_hidden != null)
			{
				this.m_hidden.Initialize("Hidden", context);
				context.ExprHostBuilder.ScaleRangeHidden(this.m_hidden);
			}
			this.m_exprHostID = context.ExprHostBuilder.ScaleRangeEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ScaleRange scaleRange = (ScaleRange)base.PublishClone(context);
			if (this.m_action != null)
			{
				scaleRange.m_action = (Action)this.m_action.PublishClone(context);
			}
			if (this.m_distanceFromScale != null)
			{
				scaleRange.m_distanceFromScale = (ExpressionInfo)this.m_distanceFromScale.PublishClone(context);
			}
			if (this.m_startValue != null)
			{
				scaleRange.m_startValue = (GaugeInputValue)this.m_startValue.PublishClone(context);
			}
			if (this.m_endValue != null)
			{
				scaleRange.m_endValue = (GaugeInputValue)this.m_endValue.PublishClone(context);
			}
			if (this.m_startWidth != null)
			{
				scaleRange.m_startWidth = (ExpressionInfo)this.m_startWidth.PublishClone(context);
			}
			if (this.m_endWidth != null)
			{
				scaleRange.m_endWidth = (ExpressionInfo)this.m_endWidth.PublishClone(context);
			}
			if (this.m_inRangeBarPointerColor != null)
			{
				scaleRange.m_inRangeBarPointerColor = (ExpressionInfo)this.m_inRangeBarPointerColor.PublishClone(context);
			}
			if (this.m_inRangeLabelColor != null)
			{
				scaleRange.m_inRangeLabelColor = (ExpressionInfo)this.m_inRangeLabelColor.PublishClone(context);
			}
			if (this.m_inRangeTickMarksColor != null)
			{
				scaleRange.m_inRangeTickMarksColor = (ExpressionInfo)this.m_inRangeTickMarksColor.PublishClone(context);
			}
			if (this.m_backgroundGradientType != null)
			{
				scaleRange.m_backgroundGradientType = (ExpressionInfo)this.m_backgroundGradientType.PublishClone(context);
			}
			if (this.m_placement != null)
			{
				scaleRange.m_placement = (ExpressionInfo)this.m_placement.PublishClone(context);
			}
			if (this.m_toolTip != null)
			{
				scaleRange.m_toolTip = (ExpressionInfo)this.m_toolTip.PublishClone(context);
			}
			if (this.m_hidden != null)
			{
				scaleRange.m_hidden = (ExpressionInfo)this.m_hidden.PublishClone(context);
			}
			return scaleRange;
		}

		internal void SetExprHost(ScaleRangeExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			base.SetExprHost(exprHost, reportObjectModel);
			this.m_exprHost = exprHost;
			if (this.m_action != null && exprHost.ActionInfoHost != null)
			{
				this.m_action.SetExprHost(exprHost.ActionInfoHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.Action, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Action));
			list.Add(new MemberInfo(MemberName.DistanceFromScale, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.StartValue, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeInputValue));
			list.Add(new MemberInfo(MemberName.EndValue, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeInputValue));
			list.Add(new MemberInfo(MemberName.StartWidth, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.EndWidth, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.InRangeBarPointerColor, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.InRangeLabelColor, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.InRangeTickMarksColor, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.BackgroundGradientType, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Placement, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ToolTip, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Hidden, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.ID, Token.Int32));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScaleRange, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanelStyleContainer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(ScaleRange.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Action:
					writer.Write(this.m_action);
					break;
				case MemberName.Name:
					writer.Write(this.m_name);
					break;
				case MemberName.DistanceFromScale:
					writer.Write(this.m_distanceFromScale);
					break;
				case MemberName.StartValue:
					writer.Write(this.m_startValue);
					break;
				case MemberName.EndValue:
					writer.Write(this.m_endValue);
					break;
				case MemberName.StartWidth:
					writer.Write(this.m_startWidth);
					break;
				case MemberName.EndWidth:
					writer.Write(this.m_endWidth);
					break;
				case MemberName.InRangeBarPointerColor:
					writer.Write(this.m_inRangeBarPointerColor);
					break;
				case MemberName.InRangeLabelColor:
					writer.Write(this.m_inRangeLabelColor);
					break;
				case MemberName.InRangeTickMarksColor:
					writer.Write(this.m_inRangeTickMarksColor);
					break;
				case MemberName.BackgroundGradientType:
					writer.Write(this.m_backgroundGradientType);
					break;
				case MemberName.Placement:
					writer.Write(this.m_placement);
					break;
				case MemberName.ToolTip:
					writer.Write(this.m_toolTip);
					break;
				case MemberName.Hidden:
					writer.Write(this.m_hidden);
					break;
				case MemberName.ExprHostID:
					writer.Write(this.m_exprHostID);
					break;
				case MemberName.ID:
					writer.Write(this.m_id);
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
			reader.RegisterDeclaration(ScaleRange.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Action:
					this.m_action = (Action)reader.ReadRIFObject();
					break;
				case MemberName.Name:
					this.m_name = reader.ReadString();
					break;
				case MemberName.DistanceFromScale:
					this.m_distanceFromScale = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.StartValue:
					this.m_startValue = (GaugeInputValue)reader.ReadRIFObject();
					break;
				case MemberName.EndValue:
					this.m_endValue = (GaugeInputValue)reader.ReadRIFObject();
					break;
				case MemberName.StartWidth:
					this.m_startWidth = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.EndWidth:
					this.m_endWidth = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.InRangeBarPointerColor:
					this.m_inRangeBarPointerColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.InRangeLabelColor:
					this.m_inRangeLabelColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.InRangeTickMarksColor:
					this.m_inRangeTickMarksColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.BackgroundGradientType:
					this.m_backgroundGradientType = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Placement:
					this.m_placement = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ToolTip:
					this.m_toolTip = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Hidden:
					this.m_hidden = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ExprHostID:
					this.m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.ID:
					this.m_id = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
			if (this.m_id == 0)
			{
				this.m_id = base.m_gaugePanel.GenerateActionOwnerID();
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScaleRange;
		}

		internal double EvaluateDistanceFromScale(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateScaleRangeDistanceFromScaleExpression(this, base.m_gaugePanel.Name);
		}

		internal double EvaluateStartWidth(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateScaleRangeStartWidthExpression(this, base.m_gaugePanel.Name);
		}

		internal double EvaluateEndWidth(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateScaleRangeEndWidthExpression(this, base.m_gaugePanel.Name);
		}

		internal string EvaluateInRangeBarPointerColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateScaleRangeInRangeBarPointerColorExpression(this, base.m_gaugePanel.Name);
		}

		internal string EvaluateInRangeLabelColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateScaleRangeInRangeLabelColorExpression(this, base.m_gaugePanel.Name);
		}

		internal string EvaluateInRangeTickMarksColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateScaleRangeInRangeTickMarksColorExpression(this, base.m_gaugePanel.Name);
		}

		internal BackgroundGradientTypes EvaluateBackgroundGradientType(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateBackgroundGradientTypes(context.ReportRuntime.EvaluateScaleRangeBackgroundGradientTypeExpression(this, base.m_gaugePanel.Name), context.ReportRuntime);
		}

		internal ScaleRangePlacements EvaluatePlacement(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateScaleRangePlacements(context.ReportRuntime.EvaluateScaleRangePlacementExpression(this, base.m_gaugePanel.Name), context.ReportRuntime);
		}

		internal string EvaluateToolTip(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateScaleRangeToolTipExpression(this, base.m_gaugePanel.Name);
		}

		internal bool EvaluateHidden(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateScaleRangeHiddenExpression(this, base.m_gaugePanel.Name);
		}
	}
}
