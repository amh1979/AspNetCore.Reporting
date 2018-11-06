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
	internal sealed class RadialPointer : GaugePointer, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = RadialPointer.GetDeclaration();

		private ExpressionInfo m_type;

		private PointerCap m_pointerCap;

		private ExpressionInfo m_needleStyle;

		internal ExpressionInfo Type
		{
			get
			{
				return this.m_type;
			}
			set
			{
				this.m_type = value;
			}
		}

		internal PointerCap PointerCap
		{
			get
			{
				return this.m_pointerCap;
			}
			set
			{
				this.m_pointerCap = value;
			}
		}

		internal ExpressionInfo NeedleStyle
		{
			get
			{
				return this.m_needleStyle;
			}
			set
			{
				this.m_needleStyle = value;
			}
		}

		internal RadialPointer()
		{
		}

		internal RadialPointer(GaugePanel gaugePanel, int id)
			: base(gaugePanel, id)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.RadialPointerStart(base.m_name);
			base.Initialize(context);
			if (this.m_type != null)
			{
				this.m_type.Initialize("Type", context);
				context.ExprHostBuilder.RadialPointerType(this.m_type);
			}
			if (this.m_pointerCap != null)
			{
				this.m_pointerCap.Initialize(context);
			}
			if (this.m_needleStyle != null)
			{
				this.m_needleStyle.Initialize("NeedleStyle", context);
				context.ExprHostBuilder.RadialPointerNeedleStyle(this.m_needleStyle);
			}
			base.m_exprHostID = context.ExprHostBuilder.RadialPointerEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			RadialPointer radialPointer = (RadialPointer)base.PublishClone(context);
			if (this.m_type != null)
			{
				radialPointer.m_type = (ExpressionInfo)this.m_type.PublishClone(context);
			}
			if (this.m_pointerCap != null)
			{
				radialPointer.m_pointerCap = (PointerCap)this.m_pointerCap.PublishClone(context);
			}
			if (this.m_needleStyle != null)
			{
				radialPointer.m_needleStyle = (ExpressionInfo)this.m_needleStyle.PublishClone(context);
			}
			return radialPointer;
		}

		internal void SetExprHost(RadialPointerExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			base.SetExprHost(exprHost, reportObjectModel);
			base.m_exprHost = exprHost;
			if (this.m_pointerCap != null && ((RadialPointerExprHost)base.m_exprHost).PointerCapHost != null)
			{
				this.m_pointerCap.SetExprHost(((RadialPointerExprHost)base.m_exprHost).PointerCapHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Type, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.PointerCap, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PointerCap));
			list.Add(new MemberInfo(MemberName.NeedleStyle, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RadialPointer, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePointer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(RadialPointer.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Type:
					writer.Write(this.m_type);
					break;
				case MemberName.PointerCap:
					writer.Write(this.m_pointerCap);
					break;
				case MemberName.NeedleStyle:
					writer.Write(this.m_needleStyle);
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
			reader.RegisterDeclaration(RadialPointer.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Type:
					this.m_type = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.PointerCap:
					this.m_pointerCap = (PointerCap)reader.ReadRIFObject();
					break;
				case MemberName.NeedleStyle:
					this.m_needleStyle = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RadialPointer;
		}

		internal RadialPointerTypes EvaluateType(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateRadialPointerTypes(context.ReportRuntime.EvaluateRadialPointerTypeExpression(this, base.m_gaugePanel.Name), context.ReportRuntime);
		}

		internal RadialPointerNeedleStyles EvaluateNeedleStyle(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateRadialPointerNeedleStyles(context.ReportRuntime.EvaluateRadialPointerNeedleStyleExpression(this, base.m_gaugePanel.Name), context.ReportRuntime);
		}
	}
}
