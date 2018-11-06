using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class MapRow : Row, IPersistable
	{
		[NonSerialized]
		private CellList m_cells;

		[Reference]
		private MapDataRegion m_mapDataRegion;

		[NonSerialized]
		private static readonly Declaration m_Declaration = MapRow.GetDeclaration();

		internal override CellList Cells
		{
			get
			{
				return this.m_cells;
			}
		}

		internal MapCell Cell
		{
			get
			{
				if (this.m_cells != null && this.m_cells.Count == 1)
				{
					return (MapCell)this.m_cells[0];
				}
				return null;
			}
			set
			{
				if (this.m_cells == null)
				{
					this.m_cells = new CellList();
				}
				else
				{
					this.m_cells.Clear();
				}
				this.m_cells.Add(value);
			}
		}

		internal MapRow()
		{
		}

		internal MapRow(int id, MapDataRegion mapDataRegion)
			: base(id)
		{
			this.m_mapDataRegion = mapDataRegion;
		}

		[SkipMemberStaticValidation(MemberName.MapCell)]
		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.MapCell, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapCell));
			list.Add(new MemberInfo(MemberName.MapDataRegion, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapDataRegion, Token.Reference));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapRow, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Row, list);
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapRow mapRow = (MapRow)base.PublishClone(context);
			if (this.m_cells != null)
			{
				mapRow.m_cells = new CellList();
				mapRow.Cell = (MapCell)this.Cell.PublishClone(context);
			}
			return mapRow;
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(MapRow.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.MapCell:
					writer.Write(this.Cell);
					break;
				case MemberName.MapDataRegion:
					writer.WriteReference(this.m_mapDataRegion);
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
			reader.RegisterDeclaration(MapRow.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.MapCell:
					this.Cell = (MapCell)reader.ReadRIFObject();
					break;
				case MemberName.MapDataRegion:
					this.m_mapDataRegion = reader.ReadReference<MapDataRegion>(this);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			List<MemberReference> list = default(List<MemberReference>);
			if (memberReferencesCollection.TryGetValue(MapRow.m_Declaration.ObjectType, out list))
			{
				foreach (MemberReference item in list)
				{
					MemberName memberName = item.MemberName;
					if (memberName == MemberName.MapDataRegion)
					{
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						this.m_mapDataRegion = (MapDataRegion)referenceableItems[item.RefID];
					}
					else
					{
						Global.Tracer.Assert(false);
					}
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapRow;
		}
	}
}
