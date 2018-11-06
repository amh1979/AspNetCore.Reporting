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
	internal sealed class LinearPointer : GaugePointer, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = LinearPointer.GetDeclaration();

		private ExpressionInfo m_type;

		private Thermometer m_thermometer;

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

		internal Thermometer Thermometer
		{
			get
			{
				return this.m_thermometer;
			}
			set
			{
				this.m_thermometer = value;
			}
		}

		internal LinearPointer()
		{
		}

		internal LinearPointer(GaugePanel gaugePanel, int id)
			: base(gaugePanel, id)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.LinearPointerStart(base.m_name);
			base.Initialize(context);
			if (this.m_type != null)
			{
				this.m_type.Initialize("Type", context);
				context.ExprHostBuilder.LinearPointerType(this.m_type);
			}
			if (this.m_thermometer != null)
			{
				this.m_thermometer.Initialize(context);
			}
			base.m_exprHostID = context.ExprHostBuilder.LinearPointerEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			LinearPointer linearPointer = (LinearPointer)base.PublishClone(context);
			if (this.m_type != null)
			{
				linearPointer.m_type = (ExpressionInfo)this.m_type.PublishClone(context);
			}
			if (this.m_thermometer != null)
			{
				linearPointer.m_thermometer = (Thermometer)this.m_thermometer.PublishClone(context);
			}
			return linearPointer;
		}

		internal void SetExprHost(LinearPointerExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			base.SetExprHost(exprHost, reportObjectModel);
			base.m_exprHost = exprHost;
			if (this.m_thermometer != null && ((LinearPointerExprHost)base.m_exprHost).ThermometerHost != null)
			{
				this.m_thermometer.SetExprHost(((LinearPointerExprHost)base.m_exprHost).ThermometerHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Type, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Thermometer, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Thermometer));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LinearPointer, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePointer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(LinearPointer.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Type:
					writer.Write(this.m_type);
					break;
				case MemberName.Thermometer:
					writer.Write(this.m_thermometer);
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
			reader.RegisterDeclaration(LinearPointer.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Type:
					this.m_type = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Thermometer:
					this.m_thermometer = (Thermometer)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LinearPointer;
		}

		internal LinearPointerTypes EvaluateType(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateLinearPointerTypes(context.ReportRuntime.EvaluateLinearPointerTypeExpression(this, base.m_gaugePanel.Name), context.ReportRuntime);
		}
	}
}
