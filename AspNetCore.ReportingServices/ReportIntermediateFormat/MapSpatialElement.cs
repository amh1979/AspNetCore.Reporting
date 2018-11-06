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
	internal class MapSpatialElement : IPersistable
	{
		protected int m_exprHostID = -1;

		[NonSerialized]
		protected MapSpatialElementExprHost m_exprHost;

		[Reference]
		protected Map m_map;

		[Reference]
		protected MapVectorLayer m_mapVectorLayer;

		[NonSerialized]
		private static readonly Declaration m_Declaration = MapSpatialElement.GetDeclaration();

		private string m_vectorData;

		private List<MapField> m_mapFields;

		internal string VectorData
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

		internal List<MapField> MapFields
		{
			get
			{
				return this.m_mapFields;
			}
			set
			{
				this.m_mapFields = value;
			}
		}

		internal string OwnerName
		{
			get
			{
				return this.m_map.Name;
			}
		}

		internal MapSpatialElementExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal int ExpressionHostID
		{
			get
			{
				return this.m_exprHostID;
			}
		}

		protected IInstancePath InstancePath
		{
			get
			{
				return this.m_mapVectorLayer.InstancePath;
			}
		}

		internal MapSpatialElement()
		{
		}

		internal MapSpatialElement(MapVectorLayer mapVectorLayer, Map map)
		{
			this.m_map = map;
			this.m_mapVectorLayer = mapVectorLayer;
		}

		internal virtual void Initialize(InitializationContext context, int index)
		{
		}

		internal virtual object PublishClone(AutomaticSubtotalContext context)
		{
			MapSpatialElement mapSpatialElement = (MapSpatialElement)base.MemberwiseClone();
			mapSpatialElement.m_map = context.CurrentMapClone;
			mapSpatialElement.m_mapVectorLayer = context.CurrentMapVectorLayerClone;
			if (this.m_mapFields != null)
			{
				mapSpatialElement.m_mapFields = new List<MapField>(this.m_mapFields.Count);
				{
					foreach (MapField mapField in this.m_mapFields)
					{
						mapSpatialElement.m_mapFields.Add((MapField)mapField.PublishClone(context));
					}
					return mapSpatialElement;
				}
			}
			return mapSpatialElement;
		}

		internal void SetExprHost(MapSpatialElementExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			this.m_exprHost = exprHost;
			this.m_exprHost.SetReportObjectModel(reportObjectModel);
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.VectorData, Token.String));
			list.Add(new MemberInfo(MemberName.MapFields, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapField));
			list.Add(new MemberInfo(MemberName.Map, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Map, Token.Reference));
			list.Add(new MemberInfo(MemberName.MapVectorLayer, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapVectorLayer, Token.Reference));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSpatialElement, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(MapSpatialElement.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Map:
					writer.WriteReference(this.m_map);
					break;
				case MemberName.MapVectorLayer:
					writer.WriteReference(this.m_mapVectorLayer);
					break;
				case MemberName.VectorData:
					writer.Write(this.m_vectorData);
					break;
				case MemberName.MapFields:
					writer.Write(this.m_mapFields);
					break;
				case MemberName.ExprHostID:
					writer.Write(this.m_exprHostID);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public virtual void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(MapSpatialElement.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Map:
					this.m_map = reader.ReadReference<Map>(this);
					break;
				case MemberName.MapVectorLayer:
					this.m_mapVectorLayer = reader.ReadReference<MapVectorLayer>(this);
					break;
				case MemberName.VectorData:
					this.m_vectorData = reader.ReadString();
					break;
				case MemberName.MapFields:
					this.m_mapFields = reader.ReadGenericListOfRIFObjects<MapField>();
					break;
				case MemberName.ExprHostID:
					this.m_exprHostID = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public virtual void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			List<MemberReference> list = default(List<MemberReference>);
			if (memberReferencesCollection.TryGetValue(MapSpatialElement.m_Declaration.ObjectType, out list))
			{
				foreach (MemberReference item in list)
				{
					switch (item.MemberName)
					{
					case MemberName.Map:
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						this.m_map = (Map)referenceableItems[item.RefID];
						break;
					case MemberName.MapVectorLayer:
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						this.m_mapVectorLayer = (MapVectorLayer)referenceableItems[item.RefID];
						break;
					default:
						Global.Tracer.Assert(false);
						break;
					}
				}
			}
		}

		public virtual AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSpatialElement;
		}
	}
}
