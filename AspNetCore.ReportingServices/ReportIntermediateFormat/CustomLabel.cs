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
	internal sealed class CustomLabel : GaugePanelStyleContainer, IPersistable
	{
		private int m_exprHostID;

		[NonSerialized]
		private CustomLabelExprHost m_exprHost;

		[NonSerialized]
		private Formatter m_formatter;

		[NonSerialized]
		private static readonly Declaration m_Declaration = CustomLabel.GetDeclaration();

		private string m_name;

		private ExpressionInfo m_text;

		private ExpressionInfo m_allowUpsideDown;

		private ExpressionInfo m_distanceFromScale;

		private ExpressionInfo m_fontAngle;

		private ExpressionInfo m_placement;

		private ExpressionInfo m_rotateLabel;

		private TickMarkStyle m_tickMarkStyle;

		private ExpressionInfo m_value;

		private ExpressionInfo m_hidden;

		private ExpressionInfo m_useFontPercent;

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

		internal ExpressionInfo Text
		{
			get
			{
				return this.m_text;
			}
			set
			{
				this.m_text = value;
			}
		}

		internal ExpressionInfo AllowUpsideDown
		{
			get
			{
				return this.m_allowUpsideDown;
			}
			set
			{
				this.m_allowUpsideDown = value;
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

		internal ExpressionInfo FontAngle
		{
			get
			{
				return this.m_fontAngle;
			}
			set
			{
				this.m_fontAngle = value;
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

		internal ExpressionInfo RotateLabel
		{
			get
			{
				return this.m_rotateLabel;
			}
			set
			{
				this.m_rotateLabel = value;
			}
		}

		internal TickMarkStyle TickMarkStyle
		{
			get
			{
				return this.m_tickMarkStyle;
			}
			set
			{
				this.m_tickMarkStyle = value;
			}
		}

		internal ExpressionInfo Value
		{
			get
			{
				return this.m_value;
			}
			set
			{
				this.m_value = value;
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

		internal ExpressionInfo UseFontPercent
		{
			get
			{
				return this.m_useFontPercent;
			}
			set
			{
				this.m_useFontPercent = value;
			}
		}

		internal string OwnerName
		{
			get
			{
				return base.m_gaugePanel.Name;
			}
		}

		internal CustomLabelExprHost ExprHost
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

		internal CustomLabel()
		{
		}

		internal CustomLabel(GaugePanel gaugePanel)
			: base(gaugePanel)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.CustomLabelStart(this.m_name);
			base.Initialize(context);
			if (this.m_text != null)
			{
				this.m_text.Initialize("Text", context);
				context.ExprHostBuilder.CustomLabelText(this.m_text);
			}
			if (this.m_allowUpsideDown != null)
			{
				this.m_allowUpsideDown.Initialize("AllowUpsideDown", context);
				context.ExprHostBuilder.CustomLabelAllowUpsideDown(this.m_allowUpsideDown);
			}
			if (this.m_distanceFromScale != null)
			{
				this.m_distanceFromScale.Initialize("DistanceFromScale", context);
				context.ExprHostBuilder.CustomLabelDistanceFromScale(this.m_distanceFromScale);
			}
			if (this.m_fontAngle != null)
			{
				this.m_fontAngle.Initialize("FontAngle", context);
				context.ExprHostBuilder.CustomLabelFontAngle(this.m_fontAngle);
			}
			if (this.m_placement != null)
			{
				this.m_placement.Initialize("Placement", context);
				context.ExprHostBuilder.CustomLabelPlacement(this.m_placement);
			}
			if (this.m_rotateLabel != null)
			{
				this.m_rotateLabel.Initialize("RotateLabel", context);
				context.ExprHostBuilder.CustomLabelRotateLabel(this.m_rotateLabel);
			}
			if (this.m_tickMarkStyle != null)
			{
				this.m_tickMarkStyle.Initialize(context);
			}
			if (this.m_value != null)
			{
				this.m_value.Initialize("Value", context);
				context.ExprHostBuilder.CustomLabelValue(this.m_value);
			}
			if (this.m_hidden != null)
			{
				this.m_hidden.Initialize("Hidden", context);
				context.ExprHostBuilder.CustomLabelHidden(this.m_hidden);
			}
			if (this.m_useFontPercent != null)
			{
				this.m_useFontPercent.Initialize("UseFontPercent", context);
				context.ExprHostBuilder.CustomLabelUseFontPercent(this.m_useFontPercent);
			}
			this.m_exprHostID = context.ExprHostBuilder.CustomLabelEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			CustomLabel customLabel = (CustomLabel)base.PublishClone(context);
			if (this.m_text != null)
			{
				customLabel.m_text = (ExpressionInfo)this.m_text.PublishClone(context);
			}
			if (this.m_allowUpsideDown != null)
			{
				customLabel.m_allowUpsideDown = (ExpressionInfo)this.m_allowUpsideDown.PublishClone(context);
			}
			if (this.m_distanceFromScale != null)
			{
				customLabel.m_distanceFromScale = (ExpressionInfo)this.m_distanceFromScale.PublishClone(context);
			}
			if (this.m_fontAngle != null)
			{
				customLabel.m_fontAngle = (ExpressionInfo)this.m_fontAngle.PublishClone(context);
			}
			if (this.m_placement != null)
			{
				customLabel.m_placement = (ExpressionInfo)this.m_placement.PublishClone(context);
			}
			if (this.m_rotateLabel != null)
			{
				customLabel.m_rotateLabel = (ExpressionInfo)this.m_rotateLabel.PublishClone(context);
			}
			if (this.m_tickMarkStyle != null)
			{
				customLabel.m_tickMarkStyle = (TickMarkStyle)this.m_tickMarkStyle.PublishClone(context);
			}
			if (this.m_value != null)
			{
				customLabel.m_value = (ExpressionInfo)this.m_value.PublishClone(context);
			}
			if (this.m_hidden != null)
			{
				customLabel.m_hidden = (ExpressionInfo)this.m_hidden.PublishClone(context);
			}
			if (this.m_useFontPercent != null)
			{
				customLabel.m_useFontPercent = (ExpressionInfo)this.m_useFontPercent.PublishClone(context);
			}
			return customLabel;
		}

		internal void SetExprHost(CustomLabelExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			base.SetExprHost(exprHost, reportObjectModel);
			this.m_exprHost = exprHost;
			if (this.m_tickMarkStyle != null && this.m_exprHost.TickMarkStyleHost != null)
			{
				this.m_tickMarkStyle.SetExprHost(this.m_exprHost.TickMarkStyleHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.Text, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.AllowUpsideDown, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DistanceFromScale, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.FontAngle, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Placement, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.RotateLabel, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.TickMarkStyle, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TickMarkStyle));
			list.Add(new MemberInfo(MemberName.Value, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Hidden, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.UseFontPercent, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.CustomLabel, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanelStyleContainer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(CustomLabel.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Name:
					writer.Write(this.m_name);
					break;
				case MemberName.Text:
					writer.Write(this.m_text);
					break;
				case MemberName.AllowUpsideDown:
					writer.Write(this.m_allowUpsideDown);
					break;
				case MemberName.DistanceFromScale:
					writer.Write(this.m_distanceFromScale);
					break;
				case MemberName.FontAngle:
					writer.Write(this.m_fontAngle);
					break;
				case MemberName.Placement:
					writer.Write(this.m_placement);
					break;
				case MemberName.RotateLabel:
					writer.Write(this.m_rotateLabel);
					break;
				case MemberName.TickMarkStyle:
					writer.Write(this.m_tickMarkStyle);
					break;
				case MemberName.Value:
					writer.Write(this.m_value);
					break;
				case MemberName.Hidden:
					writer.Write(this.m_hidden);
					break;
				case MemberName.UseFontPercent:
					writer.Write(this.m_useFontPercent);
					break;
				case MemberName.ExprHostID:
					writer.Write(this.m_exprHostID);
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
			reader.RegisterDeclaration(CustomLabel.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Name:
					this.m_name = reader.ReadString();
					break;
				case MemberName.Text:
					this.m_text = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.AllowUpsideDown:
					this.m_allowUpsideDown = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DistanceFromScale:
					this.m_distanceFromScale = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.FontAngle:
					this.m_fontAngle = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Placement:
					this.m_placement = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.RotateLabel:
					this.m_rotateLabel = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.TickMarkStyle:
					this.m_tickMarkStyle = (TickMarkStyle)reader.ReadRIFObject();
					break;
				case MemberName.Value:
					this.m_value = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Hidden:
					this.m_hidden = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.UseFontPercent:
					this.m_useFontPercent = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ExprHostID:
					this.m_exprHostID = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.CustomLabel;
		}

		internal string EvaluateText(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			AspNetCore.ReportingServices.RdlExpressions.VariantResult variantResult = context.ReportRuntime.EvaluateCustomLabelTextExpression(this, base.m_gaugePanel.Name);
			string result = null;
			if (variantResult.ErrorOccurred)
			{
				result = RPRes.rsExpressionErrorValue;
			}
			else if (variantResult.Value != null)
			{
				result = Formatter.Format(variantResult.Value, ref this.m_formatter, base.m_gaugePanel.StyleClass, base.m_styleClass, context, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, base.m_gaugePanel.Name);
			}
			return result;
		}

		internal bool EvaluateAllowUpsideDown(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateCustomLabelAllowUpsideDownExpression(this, base.m_gaugePanel.Name);
		}

		internal double EvaluateDistanceFromScale(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateCustomLabelDistanceFromScaleExpression(this, base.m_gaugePanel.Name);
		}

		internal double EvaluateFontAngle(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateCustomLabelFontAngleExpression(this, base.m_gaugePanel.Name);
		}

		internal GaugeLabelPlacements EvaluatePlacement(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateGaugeLabelPlacements(context.ReportRuntime.EvaluateCustomLabelPlacementExpression(this, base.m_gaugePanel.Name), context.ReportRuntime);
		}

		internal bool EvaluateRotateLabel(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateCustomLabelRotateLabelExpression(this, base.m_gaugePanel.Name);
		}

		internal double EvaluateValue(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateCustomLabelValueExpression(this, base.m_gaugePanel.Name);
		}

		internal bool EvaluateHidden(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateCustomLabelHiddenExpression(this, base.m_gaugePanel.Name);
		}

		internal bool EvaluateUseFontPercent(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateCustomLabelUseFontPercentExpression(this, base.m_gaugePanel.Name);
		}
	}
}
