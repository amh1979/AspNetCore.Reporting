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
	internal class Gauge : GaugePanelItem, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = Gauge.GetDeclaration();

		private BackFrame m_backFrame;

		private ExpressionInfo m_clipContent;

		private TopImage m_topImage;

		private ExpressionInfo m_aspectRatio;

		internal BackFrame BackFrame
		{
			get
			{
				return this.m_backFrame;
			}
			set
			{
				this.m_backFrame = value;
			}
		}

		internal ExpressionInfo ClipContent
		{
			get
			{
				return this.m_clipContent;
			}
			set
			{
				this.m_clipContent = value;
			}
		}

		internal TopImage TopImage
		{
			get
			{
				return this.m_topImage;
			}
			set
			{
				this.m_topImage = value;
			}
		}

		internal ExpressionInfo AspectRatio
		{
			get
			{
				return this.m_aspectRatio;
			}
			set
			{
				this.m_aspectRatio = value;
			}
		}

		internal Gauge()
		{
		}

		internal Gauge(GaugePanel gaugePanel, int id)
			: base(gaugePanel, id)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			base.Initialize(context);
			if (this.m_backFrame != null)
			{
				this.m_backFrame.Initialize(context);
			}
			if (this.m_clipContent != null)
			{
				this.m_clipContent.Initialize("ClipContent", context);
				context.ExprHostBuilder.GaugeClipContent(this.m_clipContent);
			}
			if (this.m_topImage != null)
			{
				this.m_topImage.Initialize(context);
			}
			if (this.m_aspectRatio != null)
			{
				this.m_aspectRatio.Initialize("AspectRatio", context);
				context.ExprHostBuilder.GaugeAspectRatio(this.m_aspectRatio);
			}
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			Gauge gauge = (Gauge)base.PublishClone(context);
			if (this.m_backFrame != null)
			{
				gauge.m_backFrame = (BackFrame)this.m_backFrame.PublishClone(context);
			}
			if (this.m_clipContent != null)
			{
				gauge.m_clipContent = (ExpressionInfo)this.m_clipContent.PublishClone(context);
			}
			if (this.m_topImage != null)
			{
				gauge.m_topImage = (TopImage)this.m_topImage.PublishClone(context);
			}
			if (this.m_aspectRatio != null)
			{
				gauge.m_aspectRatio = (ExpressionInfo)this.m_aspectRatio.PublishClone(context);
			}
			return gauge;
		}

		internal void SetExprHost(GaugeExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			base.SetExprHost(exprHost, reportObjectModel);
			base.m_exprHost = exprHost;
			if (this.m_backFrame != null && ((GaugeExprHost)base.m_exprHost).BackFrameHost != null)
			{
				this.m_backFrame.SetExprHost(((GaugeExprHost)base.m_exprHost).BackFrameHost, reportObjectModel);
			}
			if (this.m_topImage != null && ((GaugeExprHost)base.m_exprHost).TopImageHost != null)
			{
				this.m_topImage.SetExprHost(((GaugeExprHost)base.m_exprHost).TopImageHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.BackFrame, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BackFrame));
			list.Add(new MemberInfo(MemberName.ClipContent, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.TopImage, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TopImage));
			list.Add(new MemberInfo(MemberName.AspectRatio, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Gauge, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanelItem, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(Gauge.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.BackFrame:
					writer.Write(this.m_backFrame);
					break;
				case MemberName.ClipContent:
					writer.Write(this.m_clipContent);
					break;
				case MemberName.TopImage:
					writer.Write(this.m_topImage);
					break;
				case MemberName.AspectRatio:
					writer.Write(this.m_aspectRatio);
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
			reader.RegisterDeclaration(Gauge.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.BackFrame:
					this.m_backFrame = (BackFrame)reader.ReadRIFObject();
					break;
				case MemberName.ClipContent:
					this.m_clipContent = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.TopImage:
					this.m_topImage = (TopImage)reader.ReadRIFObject();
					break;
				case MemberName.AspectRatio:
					this.m_aspectRatio = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Gauge;
		}

		internal bool EvaluateClipContent(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeClipContentExpression(this, base.m_gaugePanel.Name);
		}

		internal double EvaluateAspectRatio(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeAspectRatioExpression(this, base.m_gaugePanel.Name);
		}
	}
}
