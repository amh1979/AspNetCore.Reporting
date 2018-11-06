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
	internal sealed class PinLabel : GaugePanelStyleContainer, IPersistable
	{
		[NonSerialized]
		private PinLabelExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = PinLabel.GetDeclaration();

		private ExpressionInfo m_text;

		private ExpressionInfo m_allowUpsideDown;

		private ExpressionInfo m_distanceFromScale;

		private ExpressionInfo m_fontAngle;

		private ExpressionInfo m_placement;

		private ExpressionInfo m_rotateLabel;

		private ExpressionInfo m_useFontPercent;

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

		internal PinLabelExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal PinLabel()
		{
		}

		internal PinLabel(GaugePanel gaugePanel)
			: base(gaugePanel)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.PinLabelStart();
			base.Initialize(context);
			if (this.m_text != null)
			{
				this.m_text.Initialize("Text", context);
				context.ExprHostBuilder.PinLabelText(this.m_text);
			}
			if (this.m_allowUpsideDown != null)
			{
				this.m_allowUpsideDown.Initialize("AllowUpsideDown", context);
				context.ExprHostBuilder.PinLabelAllowUpsideDown(this.m_allowUpsideDown);
			}
			if (this.m_distanceFromScale != null)
			{
				this.m_distanceFromScale.Initialize("DistanceFromScale", context);
				context.ExprHostBuilder.PinLabelDistanceFromScale(this.m_distanceFromScale);
			}
			if (this.m_fontAngle != null)
			{
				this.m_fontAngle.Initialize("FontAngle", context);
				context.ExprHostBuilder.PinLabelFontAngle(this.m_fontAngle);
			}
			if (this.m_placement != null)
			{
				this.m_placement.Initialize("Placement", context);
				context.ExprHostBuilder.PinLabelPlacement(this.m_placement);
			}
			if (this.m_rotateLabel != null)
			{
				this.m_rotateLabel.Initialize("RotateLabel", context);
				context.ExprHostBuilder.PinLabelRotateLabel(this.m_rotateLabel);
			}
			if (this.m_useFontPercent != null)
			{
				this.m_useFontPercent.Initialize("UseFontPercent", context);
				context.ExprHostBuilder.PinLabelUseFontPercent(this.m_useFontPercent);
			}
			context.ExprHostBuilder.PinLabelEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			PinLabel pinLabel = (PinLabel)base.PublishClone(context);
			if (this.m_text != null)
			{
				pinLabel.m_text = (ExpressionInfo)this.m_text.PublishClone(context);
			}
			if (this.m_allowUpsideDown != null)
			{
				pinLabel.m_allowUpsideDown = (ExpressionInfo)this.m_allowUpsideDown.PublishClone(context);
			}
			if (this.m_distanceFromScale != null)
			{
				pinLabel.m_distanceFromScale = (ExpressionInfo)this.m_distanceFromScale.PublishClone(context);
			}
			if (this.m_fontAngle != null)
			{
				pinLabel.m_fontAngle = (ExpressionInfo)this.m_fontAngle.PublishClone(context);
			}
			if (this.m_placement != null)
			{
				pinLabel.m_placement = (ExpressionInfo)this.m_placement.PublishClone(context);
			}
			if (this.m_rotateLabel != null)
			{
				pinLabel.m_rotateLabel = (ExpressionInfo)this.m_rotateLabel.PublishClone(context);
			}
			if (this.m_useFontPercent != null)
			{
				pinLabel.m_useFontPercent = (ExpressionInfo)this.m_useFontPercent.PublishClone(context);
			}
			return pinLabel;
		}

		internal void SetExprHost(PinLabelExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			base.SetExprHost(exprHost, reportObjectModel);
			this.m_exprHost = exprHost;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Text, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.AllowUpsideDown, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DistanceFromScale, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.FontAngle, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Placement, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.RotateLabel, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.UseFontPercent, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PinLabel, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanelStyleContainer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(PinLabel.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
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
				case MemberName.UseFontPercent:
					writer.Write(this.m_useFontPercent);
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
			reader.RegisterDeclaration(PinLabel.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
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
				case MemberName.UseFontPercent:
					this.m_useFontPercent = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PinLabel;
		}

		internal string EvaluateText(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluatePinLabelTextExpression(this, base.m_gaugePanel.Name);
		}

		internal bool EvaluateAllowUpsideDown(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluatePinLabelAllowUpsideDownExpression(this, base.m_gaugePanel.Name);
		}

		internal double EvaluateDistanceFromScale(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluatePinLabelDistanceFromScaleExpression(this, base.m_gaugePanel.Name);
		}

		internal double EvaluateFontAngle(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluatePinLabelFontAngleExpression(this, base.m_gaugePanel.Name);
		}

		internal GaugeLabelPlacements EvaluatePlacement(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateGaugeLabelPlacements(context.ReportRuntime.EvaluatePinLabelPlacementExpression(this, base.m_gaugePanel.Name), context.ReportRuntime);
		}

		internal bool EvaluateRotateLabel(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluatePinLabelRotateLabelExpression(this, base.m_gaugePanel.Name);
		}

		internal bool EvaluateUseFontPercent(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluatePinLabelUseFontPercentExpression(this, base.m_gaugePanel.Name);
		}
	}
}
