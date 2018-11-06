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
	internal sealed class RadialScale : GaugeScale, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = RadialScale.GetDeclaration();

		private List<RadialPointer> m_gaugePointers;

		private ExpressionInfo m_radius;

		private ExpressionInfo m_startAngle;

		private ExpressionInfo m_sweepAngle;

		internal List<RadialPointer> GaugePointers
		{
			get
			{
				return this.m_gaugePointers;
			}
			set
			{
				this.m_gaugePointers = value;
			}
		}

		internal ExpressionInfo Radius
		{
			get
			{
				return this.m_radius;
			}
			set
			{
				this.m_radius = value;
			}
		}

		internal ExpressionInfo StartAngle
		{
			get
			{
				return this.m_startAngle;
			}
			set
			{
				this.m_startAngle = value;
			}
		}

		internal ExpressionInfo SweepAngle
		{
			get
			{
				return this.m_sweepAngle;
			}
			set
			{
				this.m_sweepAngle = value;
			}
		}

		internal RadialScale()
		{
		}

		internal RadialScale(GaugePanel gaugePanel, int id)
			: base(gaugePanel, id)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.RadialScaleStart(base.m_name);
			base.Initialize(context);
			if (this.m_gaugePointers != null)
			{
				for (int i = 0; i < this.m_gaugePointers.Count; i++)
				{
					this.m_gaugePointers[i].Initialize(context);
				}
			}
			if (this.m_radius != null)
			{
				this.m_radius.Initialize("Radius", context);
				context.ExprHostBuilder.RadialScaleRadius(this.m_radius);
			}
			if (this.m_startAngle != null)
			{
				this.m_startAngle.Initialize("StartAngle", context);
				context.ExprHostBuilder.RadialScaleStartAngle(this.m_startAngle);
			}
			if (this.m_sweepAngle != null)
			{
				this.m_sweepAngle.Initialize("SweepAngle", context);
				context.ExprHostBuilder.RadialScaleSweepAngle(this.m_sweepAngle);
			}
			base.m_exprHostID = context.ExprHostBuilder.RadialScaleEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			RadialScale radialScale = (RadialScale)base.PublishClone(context);
			if (this.m_gaugePointers != null)
			{
				radialScale.m_gaugePointers = new List<RadialPointer>(this.m_gaugePointers.Count);
				foreach (RadialPointer gaugePointer in this.m_gaugePointers)
				{
					radialScale.m_gaugePointers.Add((RadialPointer)gaugePointer.PublishClone(context));
				}
			}
			if (this.m_radius != null)
			{
				radialScale.m_radius = (ExpressionInfo)this.m_radius.PublishClone(context);
			}
			if (this.m_startAngle != null)
			{
				radialScale.m_startAngle = (ExpressionInfo)this.m_startAngle.PublishClone(context);
			}
			if (this.m_sweepAngle != null)
			{
				radialScale.m_sweepAngle = (ExpressionInfo)this.m_sweepAngle.PublishClone(context);
			}
			return radialScale;
		}

		internal void SetExprHost(RadialScaleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			base.SetExprHost(exprHost, reportObjectModel);
			base.m_exprHost = exprHost;
			IList<RadialPointerExprHost> radialPointersHostsRemotable = ((RadialScaleExprHost)base.m_exprHost).RadialPointersHostsRemotable;
			if (this.m_gaugePointers != null && radialPointersHostsRemotable != null)
			{
				for (int i = 0; i < this.m_gaugePointers.Count; i++)
				{
					RadialPointer radialPointer = this.m_gaugePointers[i];
					if (radialPointer != null && radialPointer.ExpressionHostID > -1)
					{
						radialPointer.SetExprHost(radialPointersHostsRemotable[radialPointer.ExpressionHostID], reportObjectModel);
					}
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.GaugePointers, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RadialPointer));
			list.Add(new MemberInfo(MemberName.Radius, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.StartAngle, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.SweepAngle, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RadialScale, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeScale, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(RadialScale.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.GaugePointers:
					writer.Write(this.m_gaugePointers);
					break;
				case MemberName.Radius:
					writer.Write(this.m_radius);
					break;
				case MemberName.StartAngle:
					writer.Write(this.m_startAngle);
					break;
				case MemberName.SweepAngle:
					writer.Write(this.m_sweepAngle);
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
			reader.RegisterDeclaration(RadialScale.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.GaugePointers:
					this.m_gaugePointers = reader.ReadGenericListOfRIFObjects<RadialPointer>();
					break;
				case MemberName.Radius:
					this.m_radius = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.StartAngle:
					this.m_startAngle = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.SweepAngle:
					this.m_sweepAngle = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RadialScale;
		}

		internal double EvaluateRadius(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateRadialScaleRadiusExpression(this, base.m_gaugePanel.Name);
		}

		internal double EvaluateStartAngle(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateRadialScaleStartAngleExpression(this, base.m_gaugePanel.Name);
		}

		internal double EvaluateSweepAngle(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateRadialScaleSweepAngleExpression(this, base.m_gaugePanel.Name);
		}
	}
}
