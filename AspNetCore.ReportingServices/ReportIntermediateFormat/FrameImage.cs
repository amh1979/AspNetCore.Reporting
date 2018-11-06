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
	internal sealed class FrameImage : BaseGaugeImage, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = FrameImage.GetDeclaration();

		private ExpressionInfo m_hueColor;

		private ExpressionInfo m_transparency;

		private ExpressionInfo m_clipImage;

		internal ExpressionInfo HueColor
		{
			get
			{
				return this.m_hueColor;
			}
			set
			{
				this.m_hueColor = value;
			}
		}

		internal ExpressionInfo Transparency
		{
			get
			{
				return this.m_transparency;
			}
			set
			{
				this.m_transparency = value;
			}
		}

		internal ExpressionInfo ClipImage
		{
			get
			{
				return this.m_clipImage;
			}
			set
			{
				this.m_clipImage = value;
			}
		}

		internal FrameImage()
		{
		}

		internal FrameImage(GaugePanel gaugePanel)
			: base(gaugePanel)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.FrameImageStart();
			base.Initialize(context);
			if (this.m_hueColor != null)
			{
				this.m_hueColor.Initialize("HueColor", context);
				context.ExprHostBuilder.FrameImageHueColor(this.m_hueColor);
			}
			if (this.m_transparency != null)
			{
				this.m_transparency.Initialize("Transparency", context);
				context.ExprHostBuilder.FrameImageTransparency(this.m_transparency);
			}
			if (this.m_clipImage != null)
			{
				this.m_clipImage.Initialize("ClipImage", context);
				context.ExprHostBuilder.FrameImageClipImage(this.m_clipImage);
			}
			context.ExprHostBuilder.FrameImageEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			FrameImage frameImage = (FrameImage)base.PublishClone(context);
			if (this.m_hueColor != null)
			{
				frameImage.m_hueColor = (ExpressionInfo)this.m_hueColor.PublishClone(context);
			}
			if (this.m_transparency != null)
			{
				frameImage.m_transparency = (ExpressionInfo)this.m_transparency.PublishClone(context);
			}
			if (this.m_clipImage != null)
			{
				frameImage.m_clipImage = (ExpressionInfo)this.m_clipImage.PublishClone(context);
			}
			return frameImage;
		}

		internal void SetExprHost(FrameImageExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			base.SetExprHost(exprHost, reportObjectModel);
			base.m_exprHost = exprHost;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.HueColor, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Transparency, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ClipImage, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.FrameImage, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BaseGaugeImage, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(FrameImage.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.HueColor:
					writer.Write(this.m_hueColor);
					break;
				case MemberName.Transparency:
					writer.Write(this.m_transparency);
					break;
				case MemberName.ClipImage:
					writer.Write(this.m_clipImage);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(FrameImage.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.HueColor:
					this.m_hueColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Transparency:
					this.m_transparency = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ClipImage:
					this.m_clipImage = (ExpressionInfo)reader.ReadRIFObject();
					break;
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.FrameImage;
		}

		internal string EvaluateHueColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateFrameImageHueColorExpression(this, base.m_gaugePanel.Name);
		}

		internal double EvaluateTransparency(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateFrameImageTransparencyExpression(this, base.m_gaugePanel.Name);
		}

		internal bool EvaluateClipImage(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateFrameImageClipImageExpression(this, base.m_gaugePanel.Name);
		}
	}
}
