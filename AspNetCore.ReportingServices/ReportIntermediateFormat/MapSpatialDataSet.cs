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
	internal sealed class MapSpatialDataSet : MapSpatialData, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = MapSpatialDataSet.GetDeclaration();

		private ExpressionInfo m_dataSetName;

		private ExpressionInfo m_spatialField;

		private List<MapFieldName> m_mapFieldNames;

		internal ExpressionInfo DataSetName
		{
			get
			{
				return this.m_dataSetName;
			}
			set
			{
				this.m_dataSetName = value;
			}
		}

		internal ExpressionInfo SpatialField
		{
			get
			{
				return this.m_spatialField;
			}
			set
			{
				this.m_spatialField = value;
			}
		}

		internal List<MapFieldName> MapFieldNames
		{
			get
			{
				return this.m_mapFieldNames;
			}
			set
			{
				this.m_mapFieldNames = value;
			}
		}

		internal new MapSpatialDataSetExprHost ExprHost
		{
			get
			{
				return (MapSpatialDataSetExprHost)base.m_exprHost;
			}
		}

		internal MapSpatialDataSet()
		{
		}

		internal MapSpatialDataSet(MapVectorLayer mapVectorLayer, Map map)
			: base(mapVectorLayer, map)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapSpatialDataSetStart();
			base.Initialize(context);
			if (this.m_dataSetName != null)
			{
				this.m_dataSetName.Initialize("DataSetName", context);
				context.ExprHostBuilder.MapSpatialDataSetDataSetName(this.m_dataSetName);
			}
			if (this.m_spatialField != null)
			{
				this.m_spatialField.Initialize("SpatialField", context);
				context.ExprHostBuilder.MapSpatialDataSetSpatialField(this.m_spatialField);
			}
			if (this.m_mapFieldNames != null)
			{
				for (int i = 0; i < this.m_mapFieldNames.Count; i++)
				{
					this.m_mapFieldNames[i].Initialize(context, i);
				}
			}
			context.ExprHostBuilder.MapSpatialDataSetEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapSpatialDataSet mapSpatialDataSet = (MapSpatialDataSet)base.PublishClone(context);
			if (this.m_dataSetName != null)
			{
				mapSpatialDataSet.m_dataSetName = (ExpressionInfo)this.m_dataSetName.PublishClone(context);
			}
			if (this.m_spatialField != null)
			{
				mapSpatialDataSet.m_spatialField = (ExpressionInfo)this.m_spatialField.PublishClone(context);
			}
			if (this.m_mapFieldNames != null)
			{
				mapSpatialDataSet.m_mapFieldNames = new List<MapFieldName>(this.m_mapFieldNames.Count);
				{
					foreach (MapFieldName mapFieldName in this.m_mapFieldNames)
					{
						mapSpatialDataSet.m_mapFieldNames.Add((MapFieldName)mapFieldName.PublishClone(context));
					}
					return mapSpatialDataSet;
				}
			}
			return mapSpatialDataSet;
		}

		internal override void SetExprHost(MapSpatialDataExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHostInternal(exprHost, reportObjectModel);
			IList<MapFieldNameExprHost> mapFieldNamesHostsRemotable = this.ExprHost.MapFieldNamesHostsRemotable;
			if (this.m_mapFieldNames != null && mapFieldNamesHostsRemotable != null)
			{
				for (int i = 0; i < this.m_mapFieldNames.Count; i++)
				{
					MapFieldName mapFieldName = this.m_mapFieldNames[i];
					if (mapFieldName != null && mapFieldName.ExpressionHostID > -1)
					{
						mapFieldName.SetExprHost(mapFieldNamesHostsRemotable[mapFieldName.ExpressionHostID], reportObjectModel);
					}
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.DataSetName, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.SpatialField, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MapFieldNames, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapFieldName));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSpatialDataSet, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSpatialData, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(MapSpatialDataSet.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.DataSetName:
					writer.Write(this.m_dataSetName);
					break;
				case MemberName.SpatialField:
					writer.Write(this.m_spatialField);
					break;
				case MemberName.MapFieldNames:
					writer.Write(this.m_mapFieldNames);
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
			reader.RegisterDeclaration(MapSpatialDataSet.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.DataSetName:
					this.m_dataSetName = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.SpatialField:
					this.m_spatialField = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MapFieldNames:
					this.m_mapFieldNames = reader.ReadGenericListOfRIFObjects<MapFieldName>();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSpatialDataSet;
		}

		internal string EvaluateDataSetName(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapSpatialDataSetDataSetNameExpression(this, base.m_map.Name);
		}

		internal string EvaluateSpatialField(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapSpatialDataSetSpatialFieldExpression(this, base.m_map.Name);
		}
	}
}
