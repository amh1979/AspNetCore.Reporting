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
	internal sealed class IndicatorImage : BaseGaugeImage, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = IndicatorImage.GetDeclaration();

		private ExpressionInfo m_hueColor;

		private ExpressionInfo m_transparency;

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

		internal new IndicatorImageExprHost ExprHost
		{
			get
			{
				return (IndicatorImageExprHost)base.m_exprHost;
			}
		}

		internal IndicatorImage()
		{
		}

		internal IndicatorImage(GaugePanel gaugePanel)
			: base(gaugePanel)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.IndicatorImageStart();
			base.Initialize(context);
			if (this.m_hueColor != null)
			{
				this.m_hueColor.Initialize("HueColor", context);
				context.ExprHostBuilder.IndicatorImageHueColor(this.m_hueColor);
			}
			if (this.m_transparency != null)
			{
				this.m_transparency.Initialize("Transparency", context);
				context.ExprHostBuilder.IndicatorImageTransparency(this.m_transparency);
			}
			context.ExprHostBuilder.IndicatorImageEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			IndicatorImage indicatorImage = (IndicatorImage)base.PublishClone(context);
			if (this.m_hueColor != null)
			{
				indicatorImage.m_hueColor = (ExpressionInfo)this.m_hueColor.PublishClone(context);
			}
			if (this.m_transparency != null)
			{
				indicatorImage.m_transparency = (ExpressionInfo)this.m_transparency.PublishClone(context);
			}
			return indicatorImage;
		}

		internal void SetExprHost(IndicatorImageExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.HueColor, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Transparency, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IndicatorImage, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BaseGaugeImage, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(IndicatorImage.m_Declaration);
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
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(IndicatorImage.m_Declaration);
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
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IndicatorImage;
		}

		internal string EvaluateHueColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateIndicatorImageHueColorExpression(this, base.m_gaugePanel.Name);
		}

		internal double EvaluateTransparency(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateIndicatorImageTransparencyExpression(this, base.m_gaugePanel.Name);
		}
	}
}
