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
	internal sealed class TopImage : BaseGaugeImage, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = TopImage.GetDeclaration();

		private ExpressionInfo m_hueColor;

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

		internal TopImage()
		{
		}

		internal TopImage(GaugePanel gaugePanel)
			: base(gaugePanel)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.TopImageStart();
			base.Initialize(context);
			if (this.m_hueColor != null)
			{
				this.m_hueColor.Initialize("HueColor", context);
				context.ExprHostBuilder.TopImageHueColor(this.m_hueColor);
			}
			context.ExprHostBuilder.TopImageEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			TopImage topImage = (TopImage)base.PublishClone(context);
			if (this.m_hueColor != null)
			{
				topImage.m_hueColor = (ExpressionInfo)this.m_hueColor.PublishClone(context);
			}
			return topImage;
		}

		internal void SetExprHost(TopImageExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			base.SetExprHost(exprHost, reportObjectModel);
			base.m_exprHost = exprHost;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.HueColor, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TopImage, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BaseGaugeImage, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(TopImage.m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.HueColor)
				{
					writer.Write(this.m_hueColor);
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(TopImage.m_Declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.HueColor)
				{
					this.m_hueColor = (ExpressionInfo)reader.ReadRIFObject();
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TopImage;
		}

		internal string EvaluateHueColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateTopImageHueColorExpression(this, base.m_gaugePanel.Name);
		}
	}
}
