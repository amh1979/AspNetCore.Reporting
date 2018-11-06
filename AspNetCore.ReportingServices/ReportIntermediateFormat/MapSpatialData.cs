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
	internal class MapSpatialData : IPersistable
	{
		[NonSerialized]
		protected MapSpatialDataExprHost m_exprHost;

		[Reference]
		protected Map m_map;

		[Reference]
		protected MapVectorLayer m_mapVectorLayer;

		[NonSerialized]
		private static readonly Declaration m_Declaration = MapSpatialData.GetDeclaration();

		internal string OwnerName
		{
			get
			{
				return this.m_map.Name;
			}
		}

		internal MapSpatialDataExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal MapSpatialData()
		{
		}

		internal MapSpatialData(MapVectorLayer mapVectorLayer, Map map)
		{
			this.m_map = map;
			this.m_mapVectorLayer = mapVectorLayer;
		}

		internal virtual void Initialize(InitializationContext context)
		{
		}

		internal virtual void InitializeMapMember(InitializationContext context)
		{
		}

		internal virtual object PublishClone(AutomaticSubtotalContext context)
		{
			MapSpatialData mapSpatialData = (MapSpatialData)base.MemberwiseClone();
			mapSpatialData.m_map = context.CurrentMapClone;
			mapSpatialData.m_mapVectorLayer = context.CurrentMapVectorLayerClone;
			return mapSpatialData;
		}

		internal virtual void SetExprHost(MapSpatialDataExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
		}

		internal virtual void SetExprHostMapMember(MapSpatialDataExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
		}

		protected void SetExprHostInternal(MapSpatialDataExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			this.m_exprHost = exprHost;
			this.m_exprHost.SetReportObjectModel(reportObjectModel);
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Map, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Map, Token.Reference));
			list.Add(new MemberInfo(MemberName.MapVectorLayer, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapVectorLayer, Token.Reference));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSpatialData, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(MapSpatialData.m_Declaration);
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
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public virtual void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(MapSpatialData.m_Declaration);
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
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public virtual void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			List<MemberReference> list = default(List<MemberReference>);
			if (memberReferencesCollection.TryGetValue(MapSpatialData.m_Declaration.ObjectType, out list))
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSpatialData;
		}
	}
}
