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
	internal sealed class MapMember : ReportHierarchyNode, IPersistable
	{
		[NonSerialized]
		private MapMemberList m_innerMembers;

		[NonSerialized]
		private static readonly Declaration m_Declaration = MapMember.GetDeclaration();

		internal override string RdlElementName
		{
			get
			{
				return "MapMember";
			}
		}

		internal override HierarchyNodeList InnerHierarchy
		{
			get
			{
				return this.m_innerMembers;
			}
		}

		internal MapMember ChildMapMember
		{
			get
			{
				if (this.m_innerMembers != null && this.m_innerMembers.Count == 1)
				{
					return this.m_innerMembers[0];
				}
				return null;
			}
			set
			{
				if (value != null)
				{
					if (this.m_innerMembers == null)
					{
						this.m_innerMembers = new MapMemberList();
					}
					else
					{
						this.m_innerMembers.Clear();
					}
					this.m_innerMembers.Add(value);
				}
			}
		}

		internal MapMember()
		{
		}

		internal MapMember(int id, MapDataRegion crItem)
			: base(id, crItem)
		{
		}

		internal void SetIsCategoryMember(bool value)
		{
			base.m_isColumn = value;
			if (this.ChildMapMember != null)
			{
				this.ChildMapMember.SetIsCategoryMember(value);
			}
		}

		protected override void DataGroupStart(AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder builder)
		{
			builder.DataGroupStart(AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.MapDataRegion, base.m_isColumn);
		}

		protected override int DataGroupEnd(AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder builder)
		{
			return builder.DataGroupEnd(AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.MapDataRegion, base.m_isColumn);
		}

		private List<MapVectorLayer> GetChildMapLayers()
		{
			return ((MapDataRegion)base.DataRegionDef).GetChildVectorLayers();
		}

		internal override bool InnerInitialize(InitializationContext context, bool restrictive)
		{
			List<MapVectorLayer> childMapLayers = this.GetChildMapLayers();
			foreach (MapVectorLayer item in childMapLayers)
			{
				item.InitializeMapMember(context);
			}
			return base.InnerInitialize(context, restrictive);
		}

		internal override bool Initialize(InitializationContext context, bool restrictive)
		{
			if (!base.m_isColumn)
			{
				if (base.m_grouping != null)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidRowMapMemberCannotBeDynamic, Severity.Error, AspNetCore.ReportingServices.ReportProcessing.ObjectType.MapDataRegion, context.TablixName, "MapMember", "Group", base.m_grouping.Name);
				}
				if (this.m_innerMembers != null)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidRowMapMemberCannotContainChildMember, Severity.Error, AspNetCore.ReportingServices.ReportProcessing.ObjectType.MapDataRegion, context.TablixName, "MapMember");
				}
			}
			else if (this.m_innerMembers != null && this.m_innerMembers.Count > 1)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidColumnMapMemberCannotContainMultipleChildMember, Severity.Error, AspNetCore.ReportingServices.ReportProcessing.ObjectType.MapDataRegion, context.TablixName, "MapMember");
			}
			return base.Initialize(context, restrictive);
		}

		internal override object PublishClone(AutomaticSubtotalContext context, DataRegion newContainingRegion)
		{
			MapMember mapMember = (MapMember)base.PublishClone(context, newContainingRegion);
			if (this.ChildMapMember != null)
			{
				mapMember.ChildMapMember = (MapMember)this.ChildMapMember.PublishClone(context, newContainingRegion);
			}
			return mapMember;
		}

		[SkipMemberStaticValidation(MemberName.MapMember)]
		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.MapMember, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapMember));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapMember, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportHierarchyNode, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(MapMember.m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.MapMember)
				{
					writer.Write(this.ChildMapMember);
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
			reader.RegisterDeclaration(MapMember.m_Declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.MapMember)
				{
					this.ChildMapMember = (MapMember)reader.ReadRIFObject();
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapMember;
		}

		internal override void SetExprHost(IMemberNode memberExprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(memberExprHost != null && reportObjectModel != null);
			base.MemberNodeSetExprHost(memberExprHost, reportObjectModel);
			List<MapVectorLayer> childMapLayers = this.GetChildMapLayers();
			MapMemberExprHost mapMemberExprHost = (MapMemberExprHost)memberExprHost;
			IList<MapPolygonLayerExprHost> mapPolygonLayersHostsRemotable = mapMemberExprHost.MapPolygonLayersHostsRemotable;
			IList<MapPointLayerExprHost> mapPointLayersHostsRemotable = mapMemberExprHost.MapPointLayersHostsRemotable;
			IList<MapLineLayerExprHost> mapLineLayersHostsRemotable = mapMemberExprHost.MapLineLayersHostsRemotable;
			if (childMapLayers != null)
			{
				for (int i = 0; i < childMapLayers.Count; i++)
				{
					MapVectorLayer mapVectorLayer = childMapLayers[i];
					if (mapVectorLayer != null && mapVectorLayer.ExpressionHostMapMemberID > -1)
					{
						if (mapVectorLayer is MapPolygonLayer)
						{
							if (mapPolygonLayersHostsRemotable != null)
							{
								mapVectorLayer.SetExprHostMapMember(mapPolygonLayersHostsRemotable[mapVectorLayer.ExpressionHostMapMemberID], reportObjectModel);
							}
						}
						else if (mapVectorLayer is MapPointLayer)
						{
							if (mapPointLayersHostsRemotable != null)
							{
								mapVectorLayer.SetExprHostMapMember(mapPointLayersHostsRemotable[mapVectorLayer.ExpressionHostMapMemberID], reportObjectModel);
							}
						}
						else if (mapVectorLayer is MapLineLayer && mapLineLayersHostsRemotable != null)
						{
							mapVectorLayer.SetExprHostMapMember(mapLineLayersHostsRemotable[mapVectorLayer.ExpressionHostMapMemberID], reportObjectModel);
						}
					}
				}
			}
		}

		internal override void MemberContentsSetExprHost(ObjectModelImpl reportObjectModel, bool traverseDataRegions)
		{
		}
	}
}
