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
	internal sealed class GaugeLabel : GaugePanelItem, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GaugeLabel.GetDeclaration();

		[NonSerialized]
		private Formatter m_formatter;

		private ExpressionInfo m_text;

		private ExpressionInfo m_angle;

		private ExpressionInfo m_resizeMode;

		private ExpressionInfo m_textShadowOffset;

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

		internal ExpressionInfo Angle
		{
			get
			{
				return this.m_angle;
			}
			set
			{
				this.m_angle = value;
			}
		}

		internal ExpressionInfo ResizeMode
		{
			get
			{
				return this.m_resizeMode;
			}
			set
			{
				this.m_resizeMode = value;
			}
		}

		internal ExpressionInfo TextShadowOffset
		{
			get
			{
				return this.m_textShadowOffset;
			}
			set
			{
				this.m_textShadowOffset = value;
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

		internal GaugeLabel()
		{
		}

		internal GaugeLabel(GaugePanel gaugePanel, int id)
			: base(gaugePanel, id)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.GaugeLabelStart(base.m_name);
			base.Initialize(context);
			if (this.m_text != null)
			{
				this.m_text.Initialize("Text", context);
				context.ExprHostBuilder.GaugeLabelText(this.m_text);
			}
			if (this.m_angle != null)
			{
				this.m_angle.Initialize("Angle", context);
				context.ExprHostBuilder.GaugeLabelAngle(this.m_angle);
			}
			if (this.m_resizeMode != null)
			{
				this.m_resizeMode.Initialize("ResizeMode", context);
				context.ExprHostBuilder.GaugeLabelResizeMode(this.m_resizeMode);
			}
			if (this.m_textShadowOffset != null)
			{
				this.m_textShadowOffset.Initialize("TextShadowOffset", context);
				context.ExprHostBuilder.GaugeLabelTextShadowOffset(this.m_textShadowOffset);
			}
			if (this.m_useFontPercent != null)
			{
				this.m_useFontPercent.Initialize("UseFontPercent", context);
				context.ExprHostBuilder.GaugeLabelUseFontPercent(this.m_useFontPercent);
			}
			base.m_exprHostID = context.ExprHostBuilder.GaugeLabelEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			GaugeLabel gaugeLabel = (GaugeLabel)base.PublishClone(context);
			if (this.m_text != null)
			{
				gaugeLabel.m_text = (ExpressionInfo)this.m_text.PublishClone(context);
			}
			if (this.m_angle != null)
			{
				gaugeLabel.m_angle = (ExpressionInfo)this.m_angle.PublishClone(context);
			}
			if (this.m_resizeMode != null)
			{
				gaugeLabel.m_resizeMode = (ExpressionInfo)this.m_resizeMode.PublishClone(context);
			}
			if (this.m_textShadowOffset != null)
			{
				gaugeLabel.m_textShadowOffset = (ExpressionInfo)this.m_textShadowOffset.PublishClone(context);
			}
			if (this.m_useFontPercent != null)
			{
				gaugeLabel.m_useFontPercent = (ExpressionInfo)this.m_useFontPercent.PublishClone(context);
			}
			return gaugeLabel;
		}

		internal void SetExprHost(GaugeLabelExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			base.SetExprHost(exprHost, reportObjectModel);
			base.m_exprHost = exprHost;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Text, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Angle, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ResizeMode, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.TextShadowOffset, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.UseFontPercent, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeLabel, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanelItem, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(GaugeLabel.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Text:
					writer.Write(this.m_text);
					break;
				case MemberName.Angle:
					writer.Write(this.m_angle);
					break;
				case MemberName.ResizeMode:
					writer.Write(this.m_resizeMode);
					break;
				case MemberName.TextShadowOffset:
					writer.Write(this.m_textShadowOffset);
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
			reader.RegisterDeclaration(GaugeLabel.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Text:
					this.m_text = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Angle:
					this.m_angle = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ResizeMode:
					this.m_resizeMode = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.TextShadowOffset:
					this.m_textShadowOffset = (ExpressionInfo)reader.ReadRIFObject();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeLabel;
		}

		internal AspNetCore.ReportingServices.RdlExpressions.VariantResult EvaluateText(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeLabelTextExpression(this, base.m_gaugePanel.Name);
		}

		internal string FormatText(AspNetCore.ReportingServices.RdlExpressions.VariantResult result, OnDemandProcessingContext context)
		{
			string result2 = null;
			if (result.ErrorOccurred)
			{
				result2 = RPRes.rsExpressionErrorValue;
			}
			else if (result.Value != null)
			{
				result2 = Formatter.Format(result.Value, ref this.m_formatter, base.m_gaugePanel.StyleClass, base.m_styleClass, context, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, base.m_gaugePanel.Name);
			}
			return result2;
		}

		internal double EvaluateAngle(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeLabelAngleExpression(this, base.m_gaugePanel.Name);
		}

		internal GaugeResizeModes EvaluateResizeMode(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateGaugeResizeModes(context.ReportRuntime.EvaluateGaugeLabelResizeModeExpression(this, base.m_gaugePanel.Name), context.ReportRuntime);
		}

		internal string EvaluateTextShadowOffset(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeLabelTextShadowOffsetExpression(this, base.m_gaugePanel.Name);
		}

		internal bool EvaluateUseFontPercent(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeLabelUseFontPercentExpression(this, base.m_gaugePanel.Name);
		}
	}
}
