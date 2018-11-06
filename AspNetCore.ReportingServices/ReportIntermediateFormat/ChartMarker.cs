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
	internal sealed class ChartMarker : ChartStyleContainer, IPersistable
	{
		private ExpressionInfo m_markerType;

		private ExpressionInfo m_markerSize;

		[Reference]
		private ChartDataPoint m_chartDataPoint;

		[Reference]
		private ChartSeries m_chartSeries;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ChartMarker.GetDeclaration();

		[NonSerialized]
		private ChartMarkerExprHost m_exprHost;

		internal ExpressionInfo Type
		{
			get
			{
				return this.m_markerType;
			}
			set
			{
				this.m_markerType = value;
			}
		}

		internal ExpressionInfo Size
		{
			get
			{
				return this.m_markerSize;
			}
			set
			{
				this.m_markerSize = value;
			}
		}

		internal ChartMarkerExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		public override IInstancePath InstancePath
		{
			get
			{
				if (this.m_chartDataPoint != null)
				{
					return this.m_chartDataPoint;
				}
				if (this.m_chartSeries != null)
				{
					return this.m_chartSeries;
				}
				return base.InstancePath;
			}
		}

		internal ChartMarker()
		{
		}

		internal ChartMarker(Chart chart, ChartDataPoint chartDataPoint)
			: base(chart)
		{
			this.m_chartDataPoint = chartDataPoint;
		}

		internal ChartMarker(Chart chart, ChartSeries chartSeries)
			: base(chart)
		{
			this.m_chartSeries = chartSeries;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Type, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Size, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ChartDataPoint, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartDataPoint, Token.Reference));
			list.Add(new MemberInfo(MemberName.ChartSeries, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartSeries, Token.Reference));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartMarker, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartStyleContainer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(ChartMarker.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Type:
					writer.Write(this.m_markerType);
					break;
				case MemberName.Size:
					writer.Write(this.m_markerSize);
					break;
				case MemberName.ChartDataPoint:
					writer.WriteReference(this.m_chartDataPoint);
					break;
				case MemberName.ChartSeries:
					writer.WriteReference(this.m_chartSeries);
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
			reader.RegisterDeclaration(ChartMarker.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Type:
					this.m_markerType = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Size:
					this.m_markerSize = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ChartDataPoint:
					this.m_chartDataPoint = reader.ReadReference<ChartDataPoint>(this);
					break;
				case MemberName.ChartSeries:
					this.m_chartSeries = reader.ReadReference<ChartSeries>(this);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
			List<MemberReference> list = default(List<MemberReference>);
			if (memberReferencesCollection.TryGetValue(ChartMarker.m_Declaration.ObjectType, out list))
			{
				foreach (MemberReference item in list)
				{
					switch (item.MemberName)
					{
					case MemberName.ChartDataPoint:
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						this.m_chartDataPoint = (ChartDataPoint)referenceableItems[item.RefID];
						break;
					case MemberName.ChartSeries:
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						this.m_chartSeries = (ChartSeries)referenceableItems[item.RefID];
						break;
					default:
						Global.Tracer.Assert(false);
						break;
					}
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartMarker;
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ChartMarker chartMarker = (ChartMarker)base.PublishClone(context);
			if (this.m_markerSize != null)
			{
				chartMarker.m_markerSize = (ExpressionInfo)this.m_markerSize.PublishClone(context);
			}
			if (this.m_markerType != null)
			{
				chartMarker.m_markerType = (ExpressionInfo)this.m_markerType.PublishClone(context);
			}
			return chartMarker;
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.DataPointMarkerStart();
			base.Initialize(context);
			if (this.m_markerSize != null)
			{
				this.m_markerSize.Initialize("Size", context);
				context.ExprHostBuilder.DataPointMarkerSize(this.m_markerSize);
			}
			if (this.m_markerType != null)
			{
				this.m_markerType.Initialize("Type", context);
				context.ExprHostBuilder.DataPointMarkerType(this.m_markerType);
			}
			context.ExprHostBuilder.DataPointMarkerEnd();
		}

		internal void SetExprHost(ChartMarkerExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			this.m_exprHost = exprHost;
		}

		internal string EvaluateChartMarkerSize(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartMarkerSize(this, base.m_chart.Name);
		}

		internal ChartMarkerTypes EvaluateChartMarkerType(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.InstancePath, reportScopeInstance);
			return EnumTranslator.TranslateChartMarkerType(context.ReportRuntime.EvaluateChartMarkerType(this, base.m_chart.Name), context.ReportRuntime);
		}
	}
}
