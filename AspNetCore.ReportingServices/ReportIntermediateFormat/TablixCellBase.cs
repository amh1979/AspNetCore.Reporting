using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal abstract class TablixCellBase : Cell, IPersistable
	{
		protected int m_rowSpan;

		protected int m_colSpan;

		protected ReportItem m_cellContents;

		protected ReportItem m_altCellContents;

		[NonSerialized]
		private List<ReportItem> m_cellContentCollection;

		[NonSerialized]
		private static readonly Declaration m_Declaration = TablixCellBase.GetDeclaration();

		internal int ColSpan
		{
			get
			{
				return this.m_colSpan;
			}
			set
			{
				this.m_colSpan = value;
			}
		}

		internal int RowSpan
		{
			get
			{
				return this.m_rowSpan;
			}
			set
			{
				this.m_rowSpan = value;
			}
		}

		internal ReportItem CellContents
		{
			get
			{
				return this.m_cellContents;
			}
			set
			{
				this.m_cellContents = value;
			}
		}

		internal ReportItem AltCellContents
		{
			get
			{
				return this.m_altCellContents;
			}
			set
			{
				this.m_altCellContents = value;
			}
		}

		internal override List<ReportItem> CellContentCollection
		{
			get
			{
				if (this.m_cellContentCollection == null && base.m_hasInnerGroupTreeHierarchy && this.m_cellContents != null)
				{
					this.m_cellContentCollection = new List<ReportItem>((this.m_altCellContents == null) ? 1 : 2);
					if (this.m_cellContents != null)
					{
						this.m_cellContentCollection.Add(this.m_cellContents);
					}
					if (this.m_altCellContents != null)
					{
						this.m_cellContentCollection.Add(this.m_altCellContents);
					}
				}
				return this.m_cellContentCollection;
			}
		}

		public override AspNetCore.ReportingServices.ReportProcessing.ObjectType DataScopeObjectType
		{
			get
			{
				return AspNetCore.ReportingServices.ReportProcessing.ObjectType.TablixCell;
			}
		}

		protected override AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode ExprHostDataRegionMode
		{
			get
			{
				return AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.Tablix;
			}
		}

		internal TablixCellBase()
		{
		}

		internal TablixCellBase(int id, DataRegion dataRegion)
			: base(id, dataRegion)
		{
		}

		protected override void TraverseNestedScopes(IRIFScopeVisitor visitor)
		{
			if (this.m_cellContents != null)
			{
				this.m_cellContents.TraverseScopes(visitor);
			}
			if (this.m_altCellContents != null)
			{
				this.m_altCellContents.TraverseScopes(visitor);
			}
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			TablixCellBase tablixCellBase = (TablixCellBase)base.PublishClone(context);
			if (this.m_cellContents != null)
			{
				tablixCellBase.m_cellContents = (ReportItem)this.m_cellContents.PublishClone(context);
				if (this.m_altCellContents != null)
				{
					Global.Tracer.Assert(tablixCellBase.m_cellContents is CustomReportItem);
					tablixCellBase.m_altCellContents = (ReportItem)this.m_altCellContents.PublishClone(context);
					((CustomReportItem)tablixCellBase.m_cellContents).AltReportItem = tablixCellBase.m_altCellContents;
				}
			}
			return tablixCellBase;
		}

		internal override void InternalInitialize(int parentRowID, int parentColumnID, int rowindex, int colIndex, InitializationContext context)
		{
			if (this.m_cellContents != null)
			{
				if (this.IsDataRegionBodyCell)
				{
					context.IsTopLevelCellContents = true;
				}
				this.m_cellContents.Initialize(context);
				this.DataRendererInitialize(context);
				if (this.m_altCellContents != null)
				{
					this.m_altCellContents.Initialize(context);
				}
				base.m_hasInnerGroupTreeHierarchy = (Cell.ContainsInnerGroupTreeHierarchy(this.m_cellContents) | Cell.ContainsInnerGroupTreeHierarchy(this.m_altCellContents));
			}
		}

		internal virtual void DataRendererInitialize(InitializationContext context)
		{
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.RowSpan, Token.Int32));
			list.Add(new MemberInfo(MemberName.ColSpan, Token.Int32));
			list.Add(new MemberInfo(MemberName.CellContents, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItem));
			list.Add(new MemberInfo(MemberName.AltCellContents, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItem));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixCellBase, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Cell, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(TablixCellBase.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.RowSpan:
					writer.Write(this.m_rowSpan);
					break;
				case MemberName.ColSpan:
					writer.Write(this.m_colSpan);
					break;
				case MemberName.CellContents:
					writer.Write(this.m_cellContents);
					break;
				case MemberName.AltCellContents:
					writer.Write(this.m_altCellContents);
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
			reader.RegisterDeclaration(TablixCellBase.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.RowSpan:
					this.m_rowSpan = reader.ReadInt32();
					break;
				case MemberName.ColSpan:
					this.m_colSpan = reader.ReadInt32();
					break;
				case MemberName.CellContents:
					this.m_cellContents = (ReportItem)reader.ReadRIFObject();
					break;
				case MemberName.AltCellContents:
					this.m_altCellContents = (ReportItem)reader.ReadRIFObject();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixCellBase;
		}
	}
}
