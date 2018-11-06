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
	internal sealed class MapSpatialDataRegion : MapSpatialData, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = MapSpatialDataRegion.GetDeclaration();

		private ExpressionInfo m_vectorData;

		internal ExpressionInfo VectorData
		{
			get
			{
				return this.m_vectorData;
			}
			set
			{
				this.m_vectorData = value;
			}
		}

		internal new MapSpatialDataRegionExprHost ExprHost
		{
			get
			{
				return (MapSpatialDataRegionExprHost)base.m_exprHost;
			}
		}

		private IInstancePath InstancePath
		{
			get
			{
				return base.m_mapVectorLayer.InstancePath;
			}
		}

		internal MapSpatialDataRegion()
		{
		}

		internal MapSpatialDataRegion(MapVectorLayer mapVectorLayer, Map map)
			: base(mapVectorLayer, map)
		{
		}

		internal override void InitializeMapMember(InitializationContext context)
		{
			context.ExprHostBuilder.MapSpatialDataRegionStart();
			base.InitializeMapMember(context);
			if (this.m_vectorData != null)
			{
				this.m_vectorData.Initialize("VectorData", context);
				context.ExprHostBuilder.MapSpatialDataRegionVectorData(this.m_vectorData);
			}
			context.ExprHostBuilder.MapSpatialDataRegionEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapSpatialDataRegion mapSpatialDataRegion = (MapSpatialDataRegion)base.PublishClone(context);
			if (this.m_vectorData != null)
			{
				mapSpatialDataRegion.m_vectorData = (ExpressionInfo)this.m_vectorData.PublishClone(context);
			}
			return mapSpatialDataRegion;
		}

		internal override void SetExprHostMapMember(MapSpatialDataExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			base.SetExprHostInternal(exprHost, reportObjectModel);
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.VectorData, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSpatialDataRegion, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSpatialData, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(MapSpatialDataRegion.m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.VectorData)
				{
					writer.Write(this.m_vectorData);
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(MapSpatialDataRegion.m_Declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.VectorData)
				{
					this.m_vectorData = (ExpressionInfo)reader.ReadRIFObject();
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSpatialDataRegion;
		}

		internal AspNetCore.ReportingServices.RdlExpressions.VariantResult EvaluateVectorData(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapSpatialDataRegionVectorDataExpression(this, base.m_map.Name);
		}
	}
}
