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
	internal sealed class PointerCap : GaugePanelStyleContainer, IPersistable
	{
		[NonSerialized]
		private PointerCapExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = PointerCap.GetDeclaration();

		private CapImage m_capImage;

		private ExpressionInfo m_onTop;

		private ExpressionInfo m_reflection;

		private ExpressionInfo m_capStyle;

		private ExpressionInfo m_hidden;

		private ExpressionInfo m_width;

		internal CapImage CapImage
		{
			get
			{
				return this.m_capImage;
			}
			set
			{
				this.m_capImage = value;
			}
		}

		internal ExpressionInfo OnTop
		{
			get
			{
				return this.m_onTop;
			}
			set
			{
				this.m_onTop = value;
			}
		}

		internal ExpressionInfo Reflection
		{
			get
			{
				return this.m_reflection;
			}
			set
			{
				this.m_reflection = value;
			}
		}

		internal ExpressionInfo CapStyle
		{
			get
			{
				return this.m_capStyle;
			}
			set
			{
				this.m_capStyle = value;
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

		internal string OwnerName
		{
			get
			{
				return base.m_gaugePanel.Name;
			}
		}

		internal PointerCapExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal PointerCap()
		{
		}

		internal PointerCap(GaugePanel gaugePanel)
			: base(gaugePanel)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.PointerCapStart();
			base.Initialize(context);
			if (this.m_capImage != null)
			{
				this.m_capImage.Initialize(context);
			}
			if (this.m_onTop != null)
			{
				this.m_onTop.Initialize("OnTop", context);
				context.ExprHostBuilder.PointerCapOnTop(this.m_onTop);
			}
			if (this.m_reflection != null)
			{
				this.m_reflection.Initialize("Reflection", context);
				context.ExprHostBuilder.PointerCapReflection(this.m_reflection);
			}
			if (this.m_capStyle != null)
			{
				this.m_capStyle.Initialize("CapStyle", context);
				context.ExprHostBuilder.PointerCapCapStyle(this.m_capStyle);
			}
			if (this.m_hidden != null)
			{
				this.m_hidden.Initialize("Hidden", context);
				context.ExprHostBuilder.PointerCapHidden(this.m_hidden);
			}
			if (this.m_width != null)
			{
				this.m_width.Initialize("Width", context);
				context.ExprHostBuilder.PointerCapWidth(this.m_width);
			}
			context.ExprHostBuilder.PointerCapEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			PointerCap pointerCap = (PointerCap)base.PublishClone(context);
			if (this.m_capImage != null)
			{
				pointerCap.m_capImage = (CapImage)this.m_capImage.PublishClone(context);
			}
			if (this.m_onTop != null)
			{
				pointerCap.m_onTop = (ExpressionInfo)this.m_onTop.PublishClone(context);
			}
			if (this.m_reflection != null)
			{
				pointerCap.m_reflection = (ExpressionInfo)this.m_reflection.PublishClone(context);
			}
			if (this.m_capStyle != null)
			{
				pointerCap.m_capStyle = (ExpressionInfo)this.m_capStyle.PublishClone(context);
			}
			if (this.m_hidden != null)
			{
				pointerCap.m_hidden = (ExpressionInfo)this.m_hidden.PublishClone(context);
			}
			if (this.m_width != null)
			{
				pointerCap.m_width = (ExpressionInfo)this.m_width.PublishClone(context);
			}
			return pointerCap;
		}

		internal void SetExprHost(PointerCapExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			base.SetExprHost(exprHost, reportObjectModel);
			this.m_exprHost = exprHost;
			if (this.m_capImage != null && this.m_exprHost.CapImageHost != null)
			{
				this.m_capImage.SetExprHost(this.m_exprHost.CapImageHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.CapImage, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.CapImage));
			list.Add(new MemberInfo(MemberName.OnTop, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Reflection, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.CapStyle, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Hidden, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Width, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PointerCap, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanelStyleContainer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(PointerCap.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.CapImage:
					writer.Write(this.m_capImage);
					break;
				case MemberName.OnTop:
					writer.Write(this.m_onTop);
					break;
				case MemberName.Reflection:
					writer.Write(this.m_reflection);
					break;
				case MemberName.CapStyle:
					writer.Write(this.m_capStyle);
					break;
				case MemberName.Hidden:
					writer.Write(this.m_hidden);
					break;
				case MemberName.Width:
					writer.Write(this.m_width);
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
			reader.RegisterDeclaration(PointerCap.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.CapImage:
					this.m_capImage = (CapImage)reader.ReadRIFObject();
					break;
				case MemberName.OnTop:
					this.m_onTop = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Reflection:
					this.m_reflection = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.CapStyle:
					this.m_capStyle = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Hidden:
					this.m_hidden = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Width:
					this.m_width = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PointerCap;
		}

		internal bool EvaluateOnTop(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluatePointerCapOnTopExpression(this, base.m_gaugePanel.Name);
		}

		internal bool EvaluateReflection(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluatePointerCapReflectionExpression(this, base.m_gaugePanel.Name);
		}

		internal GaugeCapStyles EvaluateCapStyle(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateGaugeCapStyles(context.ReportRuntime.EvaluatePointerCapCapStyleExpression(this, base.m_gaugePanel.Name), context.ReportRuntime);
		}

		internal bool EvaluateHidden(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluatePointerCapHiddenExpression(this, base.m_gaugePanel.Name);
		}

		internal double EvaluateWidth(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluatePointerCapWidthExpression(this, base.m_gaugePanel.Name);
		}
	}
}
