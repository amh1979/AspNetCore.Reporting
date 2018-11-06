using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class GaugeRow : Row, IPersistable
	{
		private GaugeCellList m_cells;

		[Reference]
		private GaugePanel m_gaugePanel;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GaugeRow.GetDeclaration();

		internal override CellList Cells
		{
			get
			{
				return this.m_cells;
			}
		}

		internal GaugeCell GaugeCell
		{
			get
			{
				if (this.m_cells != null && this.m_cells.Count > 0)
				{
					return this.m_cells[0];
				}
				return null;
			}
			set
			{
				if (this.m_cells == null)
				{
					this.m_cells = new GaugeCellList();
				}
				else
				{
					this.m_cells.Clear();
				}
				this.m_cells.Add(value);
			}
		}

		internal GaugeRow()
		{
		}

		internal GaugeRow(int id, GaugePanel gaugePanel)
			: base(id)
		{
			this.m_gaugePanel = gaugePanel;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new ReadOnlyMemberInfo(MemberName.GaugeCell, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeCell));
			list.Add(new MemberInfo(MemberName.GaugePanel, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanel, Token.Reference));
			list.Add(new MemberInfo(MemberName.Cells, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeCell));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeRow, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Row, list);
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			GaugeRow gaugeRow = (GaugeRow)base.PublishClone(context);
			if (this.m_cells != null)
			{
				gaugeRow.m_cells = new GaugeCellList();
				gaugeRow.GaugeCell = (GaugeCell)this.GaugeCell.PublishClone(context);
			}
			return gaugeRow;
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(GaugeRow.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.GaugePanel:
					writer.WriteReference(this.m_gaugePanel);
					break;
				case MemberName.Cells:
					writer.Write(this.m_cells);
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
			reader.RegisterDeclaration(GaugeRow.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.GaugeCell:
					this.GaugeCell = (GaugeCell)reader.ReadRIFObject();
					break;
				case MemberName.GaugePanel:
					this.m_gaugePanel = reader.ReadReference<GaugePanel>(this);
					break;
				case MemberName.Cells:
					this.m_cells = reader.ReadListOfRIFObjects<GaugeCellList>();
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
			if (memberReferencesCollection.TryGetValue(GaugeRow.m_Declaration.ObjectType, out list))
			{
				foreach (MemberReference item in list)
				{
					MemberName memberName = item.MemberName;
					if (memberName == MemberName.GaugePanel)
					{
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						this.m_gaugePanel = (GaugePanel)referenceableItems[item.RefID];
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeRow;
		}
	}
}
