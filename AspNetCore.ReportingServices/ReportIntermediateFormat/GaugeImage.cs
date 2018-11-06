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
	internal sealed class GaugeImage : GaugePanelItem, IPersistable
	{
		private ExpressionInfo m_source;

		private ExpressionInfo m_value;

		private ExpressionInfo m_transparentColor;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GaugeImage.GetDeclaration();

		internal ExpressionInfo Source
		{
			get
			{
				return this.m_source;
			}
			set
			{
				this.m_source = value;
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

		internal ExpressionInfo TransparentColor
		{
			get
			{
				return this.m_transparentColor;
			}
			set
			{
				this.m_transparentColor = value;
			}
		}

		internal GaugeImage()
		{
		}

		internal GaugeImage(GaugePanel gaugePanel, int id)
			: base(gaugePanel, id)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.GaugeImageStart(base.m_name);
			base.Initialize(context);
			if (this.m_source != null)
			{
				this.m_source.Initialize("Source", context);
				context.ExprHostBuilder.BaseGaugeImageSource(this.m_source);
			}
			if (this.m_value != null)
			{
				this.m_value.Initialize("Value", context);
				context.ExprHostBuilder.BaseGaugeImageValue(this.m_value);
			}
			if (this.m_transparentColor != null)
			{
				this.m_transparentColor.Initialize("TransparentColor", context);
				context.ExprHostBuilder.BaseGaugeImageTransparentColor(this.m_transparentColor);
			}
			base.m_exprHostID = context.ExprHostBuilder.GaugeImageEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			GaugeImage gaugeImage = (GaugeImage)base.PublishClone(context);
			if (this.m_source != null)
			{
				gaugeImage.m_source = (ExpressionInfo)this.m_source.PublishClone(context);
			}
			if (this.m_value != null)
			{
				gaugeImage.m_value = (ExpressionInfo)this.m_value.PublishClone(context);
			}
			if (this.m_transparentColor != null)
			{
				gaugeImage.m_transparentColor = (ExpressionInfo)this.m_transparentColor.PublishClone(context);
			}
			return gaugeImage;
		}

		internal void SetExprHost(GaugeImageExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			base.SetExprHost(exprHost, reportObjectModel);
			base.m_exprHost = exprHost;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Source, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Value, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.TransparentColor, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeImage, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanelItem, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(GaugeImage.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Source:
					writer.Write(this.m_source);
					break;
				case MemberName.Value:
					writer.Write(this.m_value);
					break;
				case MemberName.TransparentColor:
					writer.Write(this.m_transparentColor);
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
			reader.RegisterDeclaration(GaugeImage.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Source:
					this.m_source = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Value:
					this.m_value = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.TransparentColor:
					this.m_transparentColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeImage;
		}

		internal AspNetCore.ReportingServices.OnDemandReportRendering.Image.SourceType EvaluateSource(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateImageSourceType(context.ReportRuntime.EvaluateGaugeImageSourceExpression(this, base.m_gaugePanel.Name), context.ReportRuntime);
		}

		internal AspNetCore.ReportingServices.RdlExpressions.VariantResult EvaluateValue(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeImageValueExpression(this, base.m_gaugePanel.Name);
		}

		internal string EvaluateTransparentColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeImageTransparentColorExpression(this, base.m_gaugePanel.Name);
		}
	}
}
