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
	internal sealed class LinearGauge : Gauge, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = LinearGauge.GetDeclaration();

		private List<LinearScale> m_gaugeScales;

		private ExpressionInfo m_orientation;

		internal List<LinearScale> GaugeScales
		{
			get
			{
				return this.m_gaugeScales;
			}
			set
			{
				this.m_gaugeScales = value;
			}
		}

		internal ExpressionInfo Orientation
		{
			get
			{
				return this.m_orientation;
			}
			set
			{
				this.m_orientation = value;
			}
		}

		internal LinearGauge()
		{
		}

		internal LinearGauge(GaugePanel gaugePanel, int id)
			: base(gaugePanel, id)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.LinearGaugeStart(base.m_name);
			base.Initialize(context);
			if (this.m_gaugeScales != null)
			{
				for (int i = 0; i < this.m_gaugeScales.Count; i++)
				{
					this.m_gaugeScales[i].Initialize(context);
				}
			}
			if (this.m_orientation != null)
			{
				this.m_orientation.Initialize("Orientation", context);
				context.ExprHostBuilder.LinearGaugeOrientation(this.m_orientation);
			}
			base.m_exprHostID = context.ExprHostBuilder.LinearGaugeEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			LinearGauge linearGauge = (LinearGauge)base.PublishClone(context);
			if (this.m_gaugeScales != null)
			{
				linearGauge.m_gaugeScales = new List<LinearScale>(this.m_gaugeScales.Count);
				foreach (LinearScale gaugeScale in this.m_gaugeScales)
				{
					linearGauge.m_gaugeScales.Add((LinearScale)gaugeScale.PublishClone(context));
				}
			}
			if (this.m_orientation != null)
			{
				linearGauge.m_orientation = (ExpressionInfo)this.m_orientation.PublishClone(context);
			}
			return linearGauge;
		}

		internal void SetExprHost(LinearGaugeExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			base.SetExprHost(exprHost, reportObjectModel);
			base.m_exprHost = exprHost;
			IList<LinearScaleExprHost> linearScalesHostsRemotable = ((LinearGaugeExprHost)base.m_exprHost).LinearScalesHostsRemotable;
			if (this.m_gaugeScales != null && linearScalesHostsRemotable != null)
			{
				for (int i = 0; i < this.m_gaugeScales.Count; i++)
				{
					LinearScale linearScale = this.m_gaugeScales[i];
					if (linearScale != null && linearScale.ExpressionHostID > -1)
					{
						linearScale.SetExprHost(linearScalesHostsRemotable[linearScale.ExpressionHostID], reportObjectModel);
					}
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.GaugeScales, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LinearScale));
			list.Add(new MemberInfo(MemberName.Orientation, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LinearGauge, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Gauge, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(LinearGauge.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.GaugeScales:
					writer.Write(this.m_gaugeScales);
					break;
				case MemberName.Orientation:
					writer.Write(this.m_orientation);
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
			reader.RegisterDeclaration(LinearGauge.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.GaugeScales:
					this.m_gaugeScales = reader.ReadGenericListOfRIFObjects<LinearScale>();
					break;
				case MemberName.Orientation:
					this.m_orientation = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LinearGauge;
		}

		internal GaugeOrientations EvaluateOrientation(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateGaugeOrientations(context.ReportRuntime.EvaluateLinearGaugeOrientationExpression(this, base.m_gaugePanel.Name), context.ReportRuntime);
		}
	}
}
