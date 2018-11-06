using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class CustomDataRow : Row
	{
		private DataCellList m_cells;

		[NonSerialized]
		private static readonly Declaration m_Declaration = CustomDataRow.GetDeclaration();

		internal override CellList Cells
		{
			get
			{
				return this.m_cells;
			}
		}

		internal DataCellList DataCells
		{
			get
			{
				return this.m_cells;
			}
			set
			{
				this.m_cells = value;
			}
		}

		internal CustomDataRow()
		{
		}

		internal CustomDataRow(int id)
			: base(id)
		{
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Cells, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataCell));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.CustomDataRow, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Row, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(CustomDataRow.m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.Cells)
				{
					writer.Write(this.m_cells);
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
			reader.RegisterDeclaration(CustomDataRow.m_Declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.Cells)
				{
					this.m_cells = reader.ReadListOfRIFObjects<DataCellList>();
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false);
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.CustomDataRow;
		}
	}
}
