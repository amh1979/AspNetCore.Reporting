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
	internal abstract class MapVectorLayer : MapLayer, IPersistable, IReferenceable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = MapVectorLayer.GetDeclaration();

		private string m_mapDataRegionName;

		private int m_ID;

		private List<MapBindingFieldPair> m_mapBindingFieldPairs;

		private List<MapFieldDefinition> m_mapFieldDefinitions;

		private MapSpatialData m_mapSpatialData;

		private string m_dataElementName;

		private DataElementOutputTypes m_dataElementOutput;

		protected int m_exprHostMapMemberID = -1;

		[NonSerialized]
		protected MapVectorLayerExprHost m_exprHostMapMember;

		[NonSerialized]
		private IInstancePath m_instancePath;

		internal string DataElementName
		{
			get
			{
				if (string.IsNullOrEmpty(this.m_dataElementName))
				{
					return base.Name;
				}
				return this.m_dataElementName;
			}
			set
			{
				this.m_dataElementName = value;
			}
		}

		internal DataElementOutputTypes DataElementOutput
		{
			get
			{
				return this.m_dataElementOutput;
			}
			set
			{
				this.m_dataElementOutput = value;
			}
		}

		public int ID
		{
			get
			{
				return this.m_ID;
			}
		}

		internal string MapDataRegionName
		{
			get
			{
				return this.m_mapDataRegionName;
			}
			set
			{
				this.m_mapDataRegionName = value;
			}
		}

		internal List<MapBindingFieldPair> MapBindingFieldPairs
		{
			get
			{
				return this.m_mapBindingFieldPairs;
			}
			set
			{
				this.m_mapBindingFieldPairs = value;
			}
		}

		internal List<MapFieldDefinition> MapFieldDefinitions
		{
			get
			{
				return this.m_mapFieldDefinitions;
			}
			set
			{
				this.m_mapFieldDefinitions = value;
			}
		}

		internal MapSpatialData MapSpatialData
		{
			get
			{
				return this.m_mapSpatialData;
			}
			set
			{
				this.m_mapSpatialData = value;
			}
		}

		internal new MapVectorLayerExprHost ExprHost
		{
			get
			{
				return (MapVectorLayerExprHost)base.m_exprHost;
			}
		}

		internal MapVectorLayerExprHost ExprHostMapMember
		{
			get
			{
				return this.m_exprHostMapMember;
			}
		}

		internal int ExpressionHostMapMemberID
		{
			get
			{
				return this.m_exprHostMapMemberID;
			}
			set
			{
				this.m_exprHostMapMemberID = value;
			}
		}

		internal IInstancePath InstancePath
		{
			get
			{
				if (this.m_instancePath == null)
				{
					if (this.m_mapDataRegionName != null)
					{
						foreach (MapDataRegion mapDataRegion in base.m_map.MapDataRegions)
						{
							if (string.CompareOrdinal(this.m_mapDataRegionName, mapDataRegion.Name) == 0)
							{
								this.m_instancePath = mapDataRegion.InnerMostMapMember;
							}
						}
					}
					if (this.m_instancePath == null)
					{
						this.m_instancePath = base.m_map;
					}
				}
				return this.m_instancePath;
			}
		}

		protected abstract bool Embedded
		{
			get;
		}

		internal MapVectorLayer()
		{
		}

		internal MapVectorLayer(int ID, Map map)
			: base(map)
		{
			this.m_ID = ID;
		}

		internal void Validate(PublishingErrorContext errorContext)
		{
			if (this.MapSpatialData is MapSpatialDataRegion && this.MapDataRegionName == null)
			{
				errorContext.Register(ProcessingErrorCode.rsMapLayerMissingProperty, Severity.Error, base.m_map.ObjectType, base.m_map.Name, base.m_name, "MapDataRegionName");
			}
			if (!(this.MapSpatialData is MapSpatialDataRegion) && this.MapDataRegionName != null && this.MapBindingFieldPairs == null)
			{
				errorContext.Register(ProcessingErrorCode.rsMapLayerMissingProperty, Severity.Error, base.m_map.ObjectType, base.m_map.Name, base.m_name, "MapBindingFieldPairs");
			}
		}

		internal override void Initialize(InitializationContext context)
		{
			base.Initialize(context);
			if (this.m_mapSpatialData != null)
			{
				this.m_mapSpatialData.Initialize(context);
			}
			if (this.m_mapBindingFieldPairs != null)
			{
				for (int i = 0; i < this.m_mapBindingFieldPairs.Count; i++)
				{
					this.m_mapBindingFieldPairs[i].Initialize(context, i);
				}
			}
		}

		internal virtual void InitializeMapMember(InitializationContext context)
		{
			if (this.m_mapSpatialData != null)
			{
				this.m_mapSpatialData.InitializeMapMember(context);
			}
			if (this.m_mapBindingFieldPairs != null)
			{
				for (int i = 0; i < this.m_mapBindingFieldPairs.Count; i++)
				{
					this.m_mapBindingFieldPairs[i].InitializeMapMember(context, i);
				}
			}
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapVectorLayer mapVectorLayer2 = context.CurrentMapVectorLayerClone = (MapVectorLayer)base.PublishClone(context);
			mapVectorLayer2.m_ID = context.GenerateID();
			if (this.MapDataRegionName != null)
			{
				mapVectorLayer2.MapDataRegionName = context.GetNewScopeName(this.MapDataRegionName);
			}
			if (this.m_mapBindingFieldPairs != null)
			{
				mapVectorLayer2.m_mapBindingFieldPairs = new List<MapBindingFieldPair>(this.m_mapBindingFieldPairs.Count);
				foreach (MapBindingFieldPair mapBindingFieldPair in this.m_mapBindingFieldPairs)
				{
					mapVectorLayer2.m_mapBindingFieldPairs.Add((MapBindingFieldPair)mapBindingFieldPair.PublishClone(context));
				}
			}
			if (this.m_mapFieldDefinitions != null)
			{
				mapVectorLayer2.m_mapFieldDefinitions = new List<MapFieldDefinition>(this.m_mapFieldDefinitions.Count);
				foreach (MapFieldDefinition mapFieldDefinition in this.m_mapFieldDefinitions)
				{
					mapVectorLayer2.m_mapFieldDefinitions.Add((MapFieldDefinition)mapFieldDefinition.PublishClone(context));
				}
			}
			if (this.m_mapSpatialData != null)
			{
				mapVectorLayer2.m_mapSpatialData = (MapSpatialData)this.m_mapSpatialData.PublishClone(context);
			}
			return mapVectorLayer2;
		}

		internal override void SetExprHost(MapLayerExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			IList<MapBindingFieldPairExprHost> mapBindingFieldPairsHostsRemotable = this.ExprHost.MapBindingFieldPairsHostsRemotable;
			if (this.m_mapBindingFieldPairs != null && mapBindingFieldPairsHostsRemotable != null)
			{
				for (int i = 0; i < this.m_mapBindingFieldPairs.Count; i++)
				{
					MapBindingFieldPair mapBindingFieldPair = this.m_mapBindingFieldPairs[i];
					if (mapBindingFieldPair != null && mapBindingFieldPair.ExpressionHostID > -1)
					{
						mapBindingFieldPair.SetExprHost(mapBindingFieldPairsHostsRemotable[mapBindingFieldPair.ExpressionHostID], reportObjectModel);
					}
				}
			}
			if (this.m_mapSpatialData != null && this.ExprHost.MapSpatialDataHost != null)
			{
				this.m_mapSpatialData.SetExprHost(this.ExprHost.MapSpatialDataHost, reportObjectModel);
			}
		}

		internal virtual void SetExprHostMapMember(MapVectorLayerExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			this.m_exprHostMapMember = exprHost;
			this.m_exprHostMapMember.SetReportObjectModel(reportObjectModel);
			IList<MapBindingFieldPairExprHost> mapBindingFieldPairsHostsRemotable = this.ExprHostMapMember.MapBindingFieldPairsHostsRemotable;
			if (this.m_mapBindingFieldPairs != null && mapBindingFieldPairsHostsRemotable != null)
			{
				for (int i = 0; i < this.m_mapBindingFieldPairs.Count; i++)
				{
					MapBindingFieldPair mapBindingFieldPair = this.m_mapBindingFieldPairs[i];
					if (mapBindingFieldPair != null && mapBindingFieldPair.ExpressionHostMapMemberID > -1)
					{
						mapBindingFieldPair.SetExprHostMapMember(mapBindingFieldPairsHostsRemotable[mapBindingFieldPair.ExpressionHostMapMemberID], reportObjectModel);
					}
				}
			}
			if (this.m_mapSpatialData != null && this.ExprHostMapMember.MapSpatialDataHost != null)
			{
				this.m_mapSpatialData.SetExprHostMapMember(this.ExprHostMapMember.MapSpatialDataHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.MapDataRegionName, Token.String));
			list.Add(new MemberInfo(MemberName.MapBindingFieldPairs, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapBindingFieldPair));
			list.Add(new MemberInfo(MemberName.MapFieldDefinitions, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapFieldDefinition));
			list.Add(new MemberInfo(MemberName.MapSpatialData, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSpatialData));
			list.Add(new MemberInfo(MemberName.ExprHostMapMemberID, Token.Int32));
			list.Add(new MemberInfo(MemberName.ID, Token.Int32));
			list.Add(new MemberInfo(MemberName.DataElementName, Token.String));
			list.Add(new MemberInfo(MemberName.DataElementOutput, Token.Enum));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapVectorLayer, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLayer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(MapVectorLayer.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.MapDataRegionName:
					writer.Write(this.m_mapDataRegionName);
					break;
				case MemberName.MapBindingFieldPairs:
					writer.Write(this.m_mapBindingFieldPairs);
					break;
				case MemberName.MapFieldDefinitions:
					writer.Write(this.m_mapFieldDefinitions);
					break;
				case MemberName.MapSpatialData:
					writer.Write(this.m_mapSpatialData);
					break;
				case MemberName.ExprHostMapMemberID:
					writer.Write(this.m_exprHostMapMemberID);
					break;
				case MemberName.ID:
					writer.Write(this.m_ID);
					break;
				case MemberName.DataElementName:
					writer.Write(this.m_dataElementName);
					break;
				case MemberName.DataElementOutput:
					writer.WriteEnum((int)this.m_dataElementOutput);
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
			reader.RegisterDeclaration(MapVectorLayer.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.MapDataRegionName:
					this.m_mapDataRegionName = reader.ReadString();
					break;
				case MemberName.MapBindingFieldPairs:
					this.m_mapBindingFieldPairs = reader.ReadGenericListOfRIFObjects<MapBindingFieldPair>();
					break;
				case MemberName.MapFieldDefinitions:
					this.m_mapFieldDefinitions = reader.ReadGenericListOfRIFObjects<MapFieldDefinition>();
					break;
				case MemberName.MapSpatialData:
					this.m_mapSpatialData = (MapSpatialData)reader.ReadRIFObject();
					break;
				case MemberName.ExprHostMapMemberID:
					this.m_exprHostMapMemberID = reader.ReadInt32();
					break;
				case MemberName.ID:
					this.m_ID = reader.ReadInt32();
					break;
				case MemberName.DataElementName:
					this.m_dataElementName = reader.ReadString();
					break;
				case MemberName.DataElementOutput:
					this.m_dataElementOutput = (DataElementOutputTypes)reader.ReadEnum();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapVectorLayer;
		}
	}
}
