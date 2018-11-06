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
	internal sealed class PointerImage : BaseGaugeImage, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = PointerImage.GetDeclaration();

		private ExpressionInfo m_hueColor;

		private ExpressionInfo m_transparency;

		private ExpressionInfo m_offsetX;

		private ExpressionInfo m_offsetY;

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

		internal ExpressionInfo OffsetX
		{
			get
			{
				return this.m_offsetX;
			}
			set
			{
				this.m_offsetX = value;
			}
		}

		internal ExpressionInfo OffsetY
		{
			get
			{
				return this.m_offsetY;
			}
			set
			{
				this.m_offsetY = value;
			}
		}

		internal PointerImage()
		{
		}

		internal PointerImage(GaugePanel gaugePanel)
			: base(gaugePanel)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.PointerImageStart();
			base.Initialize(context);
			if (this.m_hueColor != null)
			{
				this.m_hueColor.Initialize("HueColor", context);
				context.ExprHostBuilder.PointerImageHueColor(this.m_hueColor);
			}
			if (this.m_transparency != null)
			{
				this.m_transparency.Initialize("Transparency", context);
				context.ExprHostBuilder.PointerImageTransparency(this.m_transparency);
			}
			if (this.m_offsetX != null)
			{
				this.m_offsetX.Initialize("OffsetX", context);
				context.ExprHostBuilder.PointerImageOffsetX(this.m_offsetX);
			}
			if (this.m_offsetY != null)
			{
				this.m_offsetY.Initialize("OffsetY", context);
				context.ExprHostBuilder.PointerImageOffsetY(this.m_offsetY);
			}
			context.ExprHostBuilder.PointerImageEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			PointerImage pointerImage = (PointerImage)base.PublishClone(context);
			if (this.m_hueColor != null)
			{
				pointerImage.m_hueColor = (ExpressionInfo)this.m_hueColor.PublishClone(context);
			}
			if (this.m_transparency != null)
			{
				pointerImage.m_transparency = (ExpressionInfo)this.m_transparency.PublishClone(context);
			}
			if (this.m_offsetX != null)
			{
				pointerImage.m_offsetX = (ExpressionInfo)this.m_offsetX.PublishClone(context);
			}
			if (this.m_offsetY != null)
			{
				pointerImage.m_offsetY = (ExpressionInfo)this.m_offsetY.PublishClone(context);
			}
			return pointerImage;
		}

		internal void SetExprHost(PointerImageExprHost exprHost, ObjectModelImpl reportObjectModel)
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
			list.Add(new MemberInfo(MemberName.OffsetX, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.OffsetY, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PointerImage, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BaseGaugeImage, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(PointerImage.m_Declaration);
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
				case MemberName.OffsetX:
					writer.Write(this.m_offsetX);
					break;
				case MemberName.OffsetY:
					writer.Write(this.m_offsetY);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(PointerImage.m_Declaration);
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
				case MemberName.OffsetX:
					this.m_offsetX = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.OffsetY:
					this.m_offsetY = (ExpressionInfo)reader.ReadRIFObject();
					break;
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PointerImage;
		}

		internal string EvaluateHueColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluatePointerImageHueColorExpression(this, base.m_gaugePanel.Name);
		}

		internal double EvaluateTransparency(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluatePointerImageTransparencyExpression(this, base.m_gaugePanel.Name);
		}

		internal string EvaluateOffsetX(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluatePointerImageOffsetXExpression(this, base.m_gaugePanel.Name);
		}

		internal string EvaluateOffsetY(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluatePointerImageOffsetYExpression(this, base.m_gaugePanel.Name);
		}
	}
}
