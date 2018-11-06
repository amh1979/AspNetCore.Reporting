using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal sealed class TablixRow : Row, IPersistable
	{
		private string m_height;

		private double m_heightValue;

		private TablixCellList m_cells;

		[NonSerialized]
		private bool m_forAutoSubtotal;

		[NonSerialized]
		private static readonly Declaration m_Declaration = TablixRow.GetDeclaration();

		internal string Height
		{
			get
			{
				return this.m_height;
			}
			set
			{
				this.m_height = value;
			}
		}

		internal double HeightValue
		{
			get
			{
				return this.m_heightValue;
			}
			set
			{
				this.m_heightValue = value;
			}
		}

		internal override CellList Cells
		{
			get
			{
				return this.m_cells;
			}
		}

		internal TablixCellList TablixCells
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

		internal bool ForAutoSubtotal
		{
			get
			{
				return this.m_forAutoSubtotal;
			}
			set
			{
				this.m_forAutoSubtotal = value;
			}
		}

		internal TablixRow()
		{
		}

		internal TablixRow(int id)
			: base(id)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			this.m_heightValue = context.ValidateSize(this.m_height, "Height");
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			TablixRow tablixRow = (TablixRow)base.PublishClone(context);
			if (this.m_height != null)
			{
				tablixRow.m_height = (string)this.m_height.Clone();
			}
			if (this.m_cells != null)
			{
				tablixRow.m_cells = new TablixCellList(this.m_cells.Count);
				{
					foreach (TablixCell cell in this.m_cells)
					{
						tablixRow.m_cells.Add((TablixCell)cell.PublishClone(context));
					}
					return tablixRow;
				}
			}
			return tablixRow;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Height, Token.String));
			list.Add(new MemberInfo(MemberName.HeightValue, Token.Double));
			list.Add(new MemberInfo(MemberName.TablixCells, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixCell));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixRow, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Row, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(TablixRow.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Height:
					writer.Write(this.m_height);
					break;
				case MemberName.HeightValue:
					writer.Write(this.m_heightValue);
					break;
				case MemberName.TablixCells:
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
			reader.RegisterDeclaration(TablixRow.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Height:
					this.m_height = reader.ReadString();
					break;
				case MemberName.HeightValue:
					this.m_heightValue = reader.ReadDouble();
					break;
				case MemberName.TablixCells:
					this.m_cells = reader.ReadListOfRIFObjects<TablixCellList>();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false);
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixRow;
		}
	}
}
