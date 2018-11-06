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
	internal sealed class MapDataRegion : DataRegion, IPersistable
	{
		[NonSerialized]
		private MapMemberList m_columnMembers;

		[NonSerialized]
		private MapMemberList m_rowMembers;

		[NonSerialized]
		private MapMember m_innerMostMapMember;

		[NonSerialized]
		private MapRowList m_rows;

		[NonSerialized]
		private static readonly Declaration m_Declaration = MapDataRegion.GetDeclaration();

		[NonSerialized]
		private MapDataRegionExprHost m_exprHost;

		internal override AspNetCore.ReportingServices.ReportProcessing.ObjectType ObjectType
		{
			get
			{
				return AspNetCore.ReportingServices.ReportProcessing.ObjectType.MapDataRegion;
			}
		}

		internal override HierarchyNodeList ColumnMembers
		{
			get
			{
				return this.m_columnMembers;
			}
		}

		internal override HierarchyNodeList RowMembers
		{
			get
			{
				return this.m_rowMembers;
			}
		}

		internal override RowList Rows
		{
			get
			{
				return this.m_rows;
			}
		}

		internal MapMember MapMember
		{
			get
			{
				if (this.m_columnMembers != null && this.m_columnMembers.Count == 1)
				{
					return this.m_columnMembers[0];
				}
				return null;
			}
			set
			{
				if (this.m_columnMembers == null)
				{
					this.m_columnMembers = new MapMemberList();
				}
				else
				{
					this.m_columnMembers.Clear();
				}
				this.m_innerMostMapMember = null;
				this.m_columnMembers.Add(value);
			}
		}

		internal MapMember InnerMostMapMember
		{
			get
			{
				if (this.m_innerMostMapMember == null)
				{
					this.m_innerMostMapMember = this.MapMember;
					while (this.m_innerMostMapMember.ChildMapMember != null)
					{
						this.m_innerMostMapMember = this.m_innerMostMapMember.ChildMapMember;
					}
				}
				return this.m_innerMostMapMember;
			}
		}

		internal MapMember MapRowMember
		{
			get
			{
				if (this.m_rowMembers != null && this.m_rowMembers.Count == 1)
				{
					return this.m_rowMembers[0];
				}
				return null;
			}
			set
			{
				if (this.m_rowMembers == null)
				{
					this.m_rowMembers = new MapMemberList();
				}
				else
				{
					this.m_rowMembers.Clear();
				}
				this.m_rowMembers.Add(value);
			}
		}

		internal MapRow MapRow
		{
			get
			{
				if (this.m_rows != null && this.m_rows.Count == 1)
				{
					return this.m_rows[0];
				}
				return null;
			}
			set
			{
				if (this.m_rows == null)
				{
					this.m_rows = new MapRowList();
				}
				else
				{
					this.m_rows.Clear();
				}
				this.m_rows.Add(value);
			}
		}

		internal MapDataRegionExprHost MapDataRegionExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		protected override IndexedExprHost UserSortExpressionsHost
		{
			get
			{
				if (this.m_exprHost == null)
				{
					return null;
				}
				return this.m_exprHost.UserSortExpressionsHost;
			}
		}

		private Map Map
		{
			get
			{
				return (Map)base.m_parent;
			}
		}

		internal MapDataRegion(ReportItem parent)
			: base(parent)
		{
		}

		internal MapDataRegion(int id, ReportItem parent)
			: base(id, parent)
		{
			base.RowCount = 1;
			base.ColumnCount = 1;
		}

		internal override bool Initialize(InitializationContext context)
		{
			context.ObjectType = this.ObjectType;
			context.ObjectName = base.m_name;
			if ((context.Location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDetail) != 0 && (context.Location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InGrouping) == (AspNetCore.ReportingServices.ReportPublishing.LocationFlags)0)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsDataRegionInDetailList, Severity.Error, context.ObjectType, context.ObjectName, null);
			}
			else
			{
				if (!context.RegisterDataRegion(this))
				{
					return false;
				}
				context.Location |= (AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDataSet | AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDataRegion);
				context.ExprHostBuilder.DataRegionStart(AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.MapDataRegion, base.m_name);
				base.Initialize(context);
				base.ExprHostID = context.ExprHostBuilder.DataRegionEnd(AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.MapDataRegion);
				context.UnRegisterDataRegion(this);
			}
			return false;
		}

		protected override void InitializeCorner(InitializationContext context)
		{
			if (this.MapRow != null)
			{
				this.MapRow.Initialize(context);
			}
		}

		protected override bool ValidateInnerStructure(InitializationContext context)
		{
			return true;
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapDataRegion mapDataRegion = (MapDataRegion)(context.CurrentDataRegionClone = (MapDataRegion)base.PublishClone(context));
			mapDataRegion.m_parent = context.CurrentMapClone;
			mapDataRegion.m_rows = new MapRowList();
			mapDataRegion.m_rowMembers = new MapMemberList();
			mapDataRegion.m_columnMembers = new MapMemberList();
			if (this.MapMember != null)
			{
				mapDataRegion.MapMember = (MapMember)this.MapMember.PublishClone(context, mapDataRegion);
			}
			if (this.MapRowMember != null)
			{
				mapDataRegion.MapRowMember = (MapMember)this.MapRowMember.PublishClone(context);
			}
			if (this.MapRow != null)
			{
				mapDataRegion.MapRow = (MapRow)this.MapRow.PublishClone(context);
			}
			return mapDataRegion;
		}

		internal override object EvaluateNoRowsMessageExpression()
		{
			return null;
		}

		[SkipMemberStaticValidation(MemberName.MapMember)]
		[SkipMemberStaticValidation(MemberName.MapRow)]
		[SkipMemberStaticValidation(MemberName.MapRowMember)]
		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.MapMember, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapMember));
			list.Add(new MemberInfo(MemberName.MapRowMember, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapMember));
			list.Add(new MemberInfo(MemberName.MapRow, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapRow));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapDataRegion, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegion, list);
		}

		internal List<MapVectorLayer> GetChildVectorLayers()
		{
			List<MapVectorLayer> list = new List<MapVectorLayer>();
			if (this.Map.MapLayers != null)
			{
				{
					foreach (MapLayer mapLayer in this.Map.MapLayers)
					{
						if (mapLayer is MapVectorLayer)
						{
							MapVectorLayer mapVectorLayer = (MapVectorLayer)mapLayer;
							if (string.Equals(mapVectorLayer.MapDataRegionName, base.Name, StringComparison.Ordinal))
							{
								list.Add(mapVectorLayer);
							}
						}
					}
					return list;
				}
			}
			return list;
		}

		public override void CreateDomainScopeMember(ReportHierarchyNode parentNode, Grouping grouping, AutomaticSubtotalContext context)
		{
			Global.Tracer.Assert(false, "CreateDomainScopeMember should not be called for MapDataRegion");
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(MapDataRegion.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.MapMember:
					writer.Write(this.MapMember);
					break;
				case MemberName.MapRowMember:
					writer.Write(this.MapRowMember);
					break;
				case MemberName.MapRow:
					writer.Write(this.MapRow);
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
			reader.RegisterDeclaration(MapDataRegion.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.MapMember:
					this.MapMember = (MapMember)reader.ReadRIFObject();
					break;
				case MemberName.MapRowMember:
					this.MapRowMember = (MapMember)reader.ReadRIFObject();
					break;
				case MemberName.MapRow:
					this.MapRow = (MapRow)reader.ReadRIFObject();
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
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapDataRegion;
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID >= 0)
			{
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null);
				this.m_exprHost = reportExprHost.MapDataRegionHostsRemotable[base.ExprHostID];
				base.DataRegionSetExprHost(this.m_exprHost, this.m_exprHost.SortHost, this.m_exprHost.FilterHostsRemotable, this.m_exprHost.UserSortExpressionsHost, this.m_exprHost.PageBreakExprHost, this.m_exprHost.JoinConditionExprHostsRemotable, reportObjectModel);
			}
		}

		internal override void DataRegionContentsSetExprHost(ObjectModelImpl reportObjectModel, bool traverseDataRegions)
		{
		}
	}
}
