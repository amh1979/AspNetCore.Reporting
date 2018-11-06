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
	internal sealed class RadialGauge : Gauge, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = RadialGauge.GetDeclaration();

		private List<RadialScale> m_gaugeScales;

		private ExpressionInfo m_pivotX;

		private ExpressionInfo m_pivotY;

		internal List<RadialScale> GaugeScales
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

		internal ExpressionInfo PivotX
		{
			get
			{
				return this.m_pivotX;
			}
			set
			{
				this.m_pivotX = value;
			}
		}

		internal ExpressionInfo PivotY
		{
			get
			{
				return this.m_pivotY;
			}
			set
			{
				this.m_pivotY = value;
			}
		}

		internal RadialGauge()
		{
		}

		internal RadialGauge(GaugePanel gaugePanel, int id)
			: base(gaugePanel, id)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.RadialGaugeStart(base.m_name);
			base.Initialize(context);
			if (this.m_gaugeScales != null)
			{
				for (int i = 0; i < this.m_gaugeScales.Count; i++)
				{
					this.m_gaugeScales[i].Initialize(context);
				}
			}
			if (this.m_pivotX != null)
			{
				this.m_pivotX.Initialize("PivotX", context);
				context.ExprHostBuilder.RadialGaugePivotX(this.m_pivotX);
			}
			if (this.m_pivotY != null)
			{
				this.m_pivotY.Initialize("PivotY", context);
				context.ExprHostBuilder.RadialGaugePivotY(this.m_pivotY);
			}
			base.m_exprHostID = context.ExprHostBuilder.RadialGaugeEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			RadialGauge radialGauge = (RadialGauge)base.PublishClone(context);
			if (this.m_gaugeScales != null)
			{
				radialGauge.m_gaugeScales = new List<RadialScale>(this.m_gaugeScales.Count);
				foreach (RadialScale gaugeScale in this.m_gaugeScales)
				{
					radialGauge.m_gaugeScales.Add((RadialScale)gaugeScale.PublishClone(context));
				}
			}
			if (this.m_pivotX != null)
			{
				radialGauge.m_pivotX = (ExpressionInfo)this.m_pivotX.PublishClone(context);
			}
			if (this.m_pivotY != null)
			{
				radialGauge.m_pivotY = (ExpressionInfo)this.m_pivotY.PublishClone(context);
			}
			return radialGauge;
		}

		internal void SetExprHost(RadialGaugeExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			base.SetExprHost(exprHost, reportObjectModel);
			base.m_exprHost = exprHost;
			IList<RadialScaleExprHost> radialScalesHostsRemotable = ((RadialGaugeExprHost)base.m_exprHost).RadialScalesHostsRemotable;
			if (this.m_gaugeScales != null && radialScalesHostsRemotable != null)
			{
				for (int i = 0; i < this.m_gaugeScales.Count; i++)
				{
					RadialScale radialScale = this.m_gaugeScales[i];
					if (radialScale != null && radialScale.ExpressionHostID > -1)
					{
						radialScale.SetExprHost(radialScalesHostsRemotable[radialScale.ExpressionHostID], reportObjectModel);
					}
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.GaugeScales, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RadialScale));
			list.Add(new MemberInfo(MemberName.PivotX, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.PivotY, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RadialGauge, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Gauge, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(RadialGauge.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.GaugeScales:
					writer.Write(this.m_gaugeScales);
					break;
				case MemberName.PivotX:
					writer.Write(this.m_pivotX);
					break;
				case MemberName.PivotY:
					writer.Write(this.m_pivotY);
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
			reader.RegisterDeclaration(RadialGauge.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.GaugeScales:
					this.m_gaugeScales = reader.ReadGenericListOfRIFObjects<RadialScale>();
					break;
				case MemberName.PivotX:
					this.m_pivotX = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.PivotY:
					this.m_pivotY = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RadialGauge;
		}

		internal double EvaluatePivotX(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateRadialGaugePivotXExpression(this, base.m_gaugePanel.Name);
		}

		internal double EvaluatePivotY(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateRadialGaugePivotYExpression(this, base.m_gaugePanel.Name);
		}
	}
}
