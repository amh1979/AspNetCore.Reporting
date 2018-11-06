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
	internal sealed class BackFrame : GaugePanelStyleContainer, IPersistable
	{
		[NonSerialized]
		private BackFrameExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = BackFrame.GetDeclaration();

		private ExpressionInfo m_frameStyle;

		private ExpressionInfo m_frameShape;

		private ExpressionInfo m_frameWidth;

		private ExpressionInfo m_glassEffect;

		private FrameBackground m_frameBackground;

		private FrameImage m_frameImage;

		internal ExpressionInfo FrameStyle
		{
			get
			{
				return this.m_frameStyle;
			}
			set
			{
				this.m_frameStyle = value;
			}
		}

		internal ExpressionInfo FrameShape
		{
			get
			{
				return this.m_frameShape;
			}
			set
			{
				this.m_frameShape = value;
			}
		}

		internal ExpressionInfo FrameWidth
		{
			get
			{
				return this.m_frameWidth;
			}
			set
			{
				this.m_frameWidth = value;
			}
		}

		internal ExpressionInfo GlassEffect
		{
			get
			{
				return this.m_glassEffect;
			}
			set
			{
				this.m_glassEffect = value;
			}
		}

		internal FrameBackground FrameBackground
		{
			get
			{
				return this.m_frameBackground;
			}
			set
			{
				this.m_frameBackground = value;
			}
		}

		internal FrameImage FrameImage
		{
			get
			{
				return this.m_frameImage;
			}
			set
			{
				this.m_frameImage = value;
			}
		}

		internal string OwnerName
		{
			get
			{
				return base.m_gaugePanel.Name;
			}
		}

		internal BackFrameExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal BackFrame()
		{
		}

		internal BackFrame(GaugePanel gaugePanel)
			: base(gaugePanel)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.BackFrameStart();
			base.Initialize(context);
			if (this.m_frameStyle != null)
			{
				this.m_frameStyle.Initialize("FrameStyle", context);
				context.ExprHostBuilder.BackFrameFrameStyle(this.m_frameStyle);
			}
			if (this.m_frameShape != null)
			{
				this.m_frameShape.Initialize("FrameShape", context);
				context.ExprHostBuilder.BackFrameFrameShape(this.m_frameShape);
			}
			if (this.m_frameWidth != null)
			{
				this.m_frameWidth.Initialize("FrameWidth", context);
				context.ExprHostBuilder.BackFrameFrameWidth(this.m_frameWidth);
			}
			if (this.m_glassEffect != null)
			{
				this.m_glassEffect.Initialize("GlassEffect", context);
				context.ExprHostBuilder.BackFrameGlassEffect(this.m_glassEffect);
			}
			if (this.m_frameBackground != null)
			{
				this.m_frameBackground.Initialize(context);
			}
			if (this.m_frameImage != null)
			{
				this.m_frameImage.Initialize(context);
			}
			context.ExprHostBuilder.BackFrameEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			BackFrame backFrame = (BackFrame)base.PublishClone(context);
			if (this.m_frameStyle != null)
			{
				backFrame.m_frameStyle = (ExpressionInfo)this.m_frameStyle.PublishClone(context);
			}
			if (this.m_frameShape != null)
			{
				backFrame.m_frameShape = (ExpressionInfo)this.m_frameShape.PublishClone(context);
			}
			if (this.m_frameWidth != null)
			{
				backFrame.m_frameWidth = (ExpressionInfo)this.m_frameWidth.PublishClone(context);
			}
			if (this.m_glassEffect != null)
			{
				backFrame.m_glassEffect = (ExpressionInfo)this.m_glassEffect.PublishClone(context);
			}
			if (this.m_frameBackground != null)
			{
				backFrame.m_frameBackground = (FrameBackground)this.m_frameBackground.PublishClone(context);
			}
			if (this.m_frameImage != null)
			{
				backFrame.m_frameImage = (FrameImage)this.m_frameImage.PublishClone(context);
			}
			return backFrame;
		}

		internal void SetExprHost(BackFrameExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			base.SetExprHost(exprHost, reportObjectModel);
			this.m_exprHost = exprHost;
			if (this.m_frameBackground != null && this.m_exprHost.FrameBackgroundHost != null)
			{
				this.m_frameBackground.SetExprHost(this.m_exprHost.FrameBackgroundHost, reportObjectModel);
			}
			if (this.m_frameImage != null && this.m_exprHost.FrameImageHost != null)
			{
				this.m_frameImage.SetExprHost(this.m_exprHost.FrameImageHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.FrameStyle, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.FrameShape, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.FrameWidth, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.GlassEffect, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.FrameBackground, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.FrameBackground));
			list.Add(new MemberInfo(MemberName.FrameImage, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.FrameImage));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BackFrame, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanelStyleContainer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(BackFrame.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.FrameStyle:
					writer.Write(this.m_frameStyle);
					break;
				case MemberName.FrameShape:
					writer.Write(this.m_frameShape);
					break;
				case MemberName.FrameWidth:
					writer.Write(this.m_frameWidth);
					break;
				case MemberName.GlassEffect:
					writer.Write(this.m_glassEffect);
					break;
				case MemberName.FrameBackground:
					writer.Write(this.m_frameBackground);
					break;
				case MemberName.FrameImage:
					writer.Write(this.m_frameImage);
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
			reader.RegisterDeclaration(BackFrame.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.FrameStyle:
					this.m_frameStyle = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.FrameShape:
					this.m_frameShape = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.FrameWidth:
					this.m_frameWidth = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.GlassEffect:
					this.m_glassEffect = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.FrameBackground:
					this.m_frameBackground = (FrameBackground)reader.ReadRIFObject();
					break;
				case MemberName.FrameImage:
					this.m_frameImage = (FrameImage)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BackFrame;
		}

		internal GaugeFrameStyles EvaluateFrameStyle(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateGaugeFrameStyles(context.ReportRuntime.EvaluateBackFrameFrameStyleExpression(this, base.m_gaugePanel.Name), context.ReportRuntime);
		}

		internal GaugeFrameShapes EvaluateFrameShape(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateGaugeFrameShapes(context.ReportRuntime.EvaluateBackFrameFrameShapeExpression(this, base.m_gaugePanel.Name), context.ReportRuntime);
		}

		internal double EvaluateFrameWidth(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateBackFrameFrameWidthExpression(this, base.m_gaugePanel.Name);
		}

		internal GaugeGlassEffects EvaluateGlassEffect(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateGaugeGlassEffects(context.ReportRuntime.EvaluateBackFrameGlassEffectExpression(this, base.m_gaugePanel.Name), context.ReportRuntime);
		}
	}
}
