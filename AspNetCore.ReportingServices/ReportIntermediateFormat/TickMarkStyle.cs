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
	internal class TickMarkStyle : GaugePanelStyleContainer, IPersistable
	{
		[NonSerialized]
		protected TickMarkStyleExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = TickMarkStyle.GetDeclaration();

		private ExpressionInfo m_distanceFromScale;

		private ExpressionInfo m_placement;

		private ExpressionInfo m_enableGradient;

		private ExpressionInfo m_gradientDensity;

		private TopImage m_tickMarkImage;

		private ExpressionInfo m_length;

		private ExpressionInfo m_width;

		private ExpressionInfo m_shape;

		private ExpressionInfo m_hidden;

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

		internal ExpressionInfo EnableGradient
		{
			get
			{
				return this.m_enableGradient;
			}
			set
			{
				this.m_enableGradient = value;
			}
		}

		internal ExpressionInfo GradientDensity
		{
			get
			{
				return this.m_gradientDensity;
			}
			set
			{
				this.m_gradientDensity = value;
			}
		}

		internal TopImage TickMarkImage
		{
			get
			{
				return this.m_tickMarkImage;
			}
			set
			{
				this.m_tickMarkImage = value;
			}
		}

		internal ExpressionInfo Length
		{
			get
			{
				return this.m_length;
			}
			set
			{
				this.m_length = value;
			}
		}

		internal ExpressionInfo Width
		{
			get
			{
				return this.m_width;
			}
			set
			{
				this.m_width = value;
			}
		}

		internal ExpressionInfo Shape
		{
			get
			{
				return this.m_shape;
			}
			set
			{
				this.m_shape = value;
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

		internal string OwnerName
		{
			get
			{
				return base.m_gaugePanel.Name;
			}
		}

		internal TickMarkStyleExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal TickMarkStyle()
		{
		}

		internal TickMarkStyle(GaugePanel gaugePanel)
			: base(gaugePanel)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.TickMarkStyleStart();
			this.InitializeInternal(context);
			context.ExprHostBuilder.TickMarkStyleEnd();
		}

		internal void InitializeInternal(InitializationContext context)
		{
			base.Initialize(context);
			if (this.m_distanceFromScale != null)
			{
				this.m_distanceFromScale.Initialize("DistanceFromScale", context);
				context.ExprHostBuilder.TickMarkStyleDistanceFromScale(this.m_distanceFromScale);
			}
			if (this.m_placement != null)
			{
				this.m_placement.Initialize("Placement", context);
				context.ExprHostBuilder.TickMarkStylePlacement(this.m_placement);
			}
			if (this.m_enableGradient != null)
			{
				this.m_enableGradient.Initialize("EnableGradient", context);
				context.ExprHostBuilder.TickMarkStyleEnableGradient(this.m_enableGradient);
			}
			if (this.m_gradientDensity != null)
			{
				this.m_gradientDensity.Initialize("GradientDensity", context);
				context.ExprHostBuilder.TickMarkStyleGradientDensity(this.m_gradientDensity);
			}
			if (this.m_tickMarkImage != null)
			{
				this.m_tickMarkImage.Initialize(context);
			}
			if (this.m_length != null)
			{
				this.m_length.Initialize("Length", context);
				context.ExprHostBuilder.TickMarkStyleLength(this.m_length);
			}
			if (this.m_width != null)
			{
				this.m_width.Initialize("Width", context);
				context.ExprHostBuilder.TickMarkStyleWidth(this.m_width);
			}
			if (this.m_shape != null)
			{
				this.m_shape.Initialize("Shape", context);
				context.ExprHostBuilder.TickMarkStyleShape(this.m_shape);
			}
			if (this.m_hidden != null)
			{
				this.m_hidden.Initialize("Hidden", context);
				context.ExprHostBuilder.TickMarkStyleHidden(this.m_hidden);
			}
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			TickMarkStyle tickMarkStyle = (TickMarkStyle)base.PublishClone(context);
			if (this.m_distanceFromScale != null)
			{
				tickMarkStyle.m_distanceFromScale = (ExpressionInfo)this.m_distanceFromScale.PublishClone(context);
			}
			if (this.m_placement != null)
			{
				tickMarkStyle.m_placement = (ExpressionInfo)this.m_placement.PublishClone(context);
			}
			if (this.m_enableGradient != null)
			{
				tickMarkStyle.m_enableGradient = (ExpressionInfo)this.m_enableGradient.PublishClone(context);
			}
			if (this.m_gradientDensity != null)
			{
				tickMarkStyle.m_gradientDensity = (ExpressionInfo)this.m_gradientDensity.PublishClone(context);
			}
			if (this.m_tickMarkImage != null)
			{
				tickMarkStyle.m_tickMarkImage = (TopImage)this.m_tickMarkImage.PublishClone(context);
			}
			if (this.m_length != null)
			{
				tickMarkStyle.m_length = (ExpressionInfo)this.m_length.PublishClone(context);
			}
			if (this.m_width != null)
			{
				tickMarkStyle.m_width = (ExpressionInfo)this.m_width.PublishClone(context);
			}
			if (this.m_shape != null)
			{
				tickMarkStyle.m_shape = (ExpressionInfo)this.m_shape.PublishClone(context);
			}
			if (this.m_hidden != null)
			{
				tickMarkStyle.m_hidden = (ExpressionInfo)this.m_hidden.PublishClone(context);
			}
			return tickMarkStyle;
		}

		internal void SetExprHost(TickMarkStyleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			base.SetExprHost(exprHost, reportObjectModel);
			this.m_exprHost = exprHost;
			if (this.m_tickMarkImage != null && this.m_exprHost.TickMarkImageHost != null)
			{
				this.m_tickMarkImage.SetExprHost(this.m_exprHost.TickMarkImageHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.DistanceFromScale, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Placement, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.EnableGradient, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.GradientDensity, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.TickMarkImage, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TopImage));
			list.Add(new MemberInfo(MemberName.Length, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Width, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Shape, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Hidden, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TickMarkStyle, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanelStyleContainer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(TickMarkStyle.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.DistanceFromScale:
					writer.Write(this.m_distanceFromScale);
					break;
				case MemberName.Placement:
					writer.Write(this.m_placement);
					break;
				case MemberName.EnableGradient:
					writer.Write(this.m_enableGradient);
					break;
				case MemberName.GradientDensity:
					writer.Write(this.m_gradientDensity);
					break;
				case MemberName.TickMarkImage:
					writer.Write(this.m_tickMarkImage);
					break;
				case MemberName.Length:
					writer.Write(this.m_length);
					break;
				case MemberName.Width:
					writer.Write(this.m_width);
					break;
				case MemberName.Shape:
					writer.Write(this.m_shape);
					break;
				case MemberName.Hidden:
					writer.Write(this.m_hidden);
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
			reader.RegisterDeclaration(TickMarkStyle.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.DistanceFromScale:
					this.m_distanceFromScale = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Placement:
					this.m_placement = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.EnableGradient:
					this.m_enableGradient = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.GradientDensity:
					this.m_gradientDensity = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.TickMarkImage:
					this.m_tickMarkImage = (TopImage)reader.ReadRIFObject();
					break;
				case MemberName.Length:
					this.m_length = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Width:
					this.m_width = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Shape:
					this.m_shape = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Hidden:
					this.m_hidden = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TickMarkStyle;
		}

		internal double EvaluateDistanceFromScale(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateTickMarkStyleDistanceFromScaleExpression(this, base.m_gaugePanel.Name);
		}

		internal GaugeLabelPlacements EvaluatePlacement(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateGaugeLabelPlacements(context.ReportRuntime.EvaluateTickMarkStylePlacementExpression(this, base.m_gaugePanel.Name), context.ReportRuntime);
		}

		internal bool EvaluateEnableGradient(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateTickMarkStyleEnableGradientExpression(this, base.m_gaugePanel.Name);
		}

		internal double EvaluateGradientDensity(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateTickMarkStyleGradientDensityExpression(this, base.m_gaugePanel.Name);
		}

		internal double EvaluateLength(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateTickMarkStyleLengthExpression(this, base.m_gaugePanel.Name);
		}

		internal double EvaluateWidth(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateTickMarkStyleWidthExpression(this, base.m_gaugePanel.Name);
		}

		internal GaugeTickMarkShapes EvaluateShape(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateGaugeTickMarkShapes(context.ReportRuntime.EvaluateTickMarkStyleShapeExpression(this, base.m_gaugePanel.Name), context.ReportRuntime);
		}

		internal bool EvaluateHidden(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateTickMarkStyleHiddenExpression(this, base.m_gaugePanel.Name);
		}
	}
}
