using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal sealed class RuntimeSortDataHolder : IStorable, IPersistable
	{
		private DataFieldRow m_firstRow;

		private ScalableList<DataFieldRow> m_dataRows;

		private static Declaration m_declaration = RuntimeSortDataHolder.GetDeclaration();

		public int Size
		{
			get
			{
				return ItemSizes.SizeOf(this.m_firstRow) + ItemSizes.SizeOf(this.m_dataRows);
			}
		}

		internal RuntimeSortDataHolder()
		{
		}

		internal void NextRow(OnDemandProcessingContext odpContext, int depth)
		{
			DataFieldRow dataFieldRow = new DataFieldRow(odpContext.ReportObjectModel.FieldsImpl, true);
			if (this.m_firstRow == null)
			{
				this.m_firstRow = dataFieldRow;
			}
			else
			{
				if (this.m_dataRows == null)
				{
					this.m_dataRows = new ScalableList<DataFieldRow>(depth, odpContext.TablixProcessingScalabilityCache);
				}
				this.m_dataRows.Add(dataFieldRow);
			}
		}

		internal void Traverse(ProcessingStages operation, ITraversalContext traversalContext, IHierarchyObj owner)
		{
			Global.Tracer.Assert(ProcessingStages.UserSortFilter == operation || owner.InDataRowSortPhase, "Invalid call to RuntimeSortDataHolder.Traverse.  Must be in UserSortFilter stage or InDataRowSortPhase");
			if (this.m_firstRow != null)
			{
				DataRowSortOwnerTraversalContext context = traversalContext as DataRowSortOwnerTraversalContext;
				this.Traverse(this.m_firstRow, operation, context, owner);
				if (this.m_dataRows != null)
				{
					for (int i = 0; i < this.m_dataRows.Count; i++)
					{
						this.Traverse(this.m_dataRows[i], operation, context, owner);
					}
				}
			}
		}

		private void Traverse(DataFieldRow dataRow, ProcessingStages operation, DataRowSortOwnerTraversalContext context, IHierarchyObj owner)
		{
			dataRow.SetFields(owner.OdpContext.ReportObjectModel.FieldsImpl);
			if (operation == ProcessingStages.UserSortFilter)
			{
				owner.ReadRow();
			}
			else
			{
				context.SortOwner.PostDataRowSortNextRow();
			}
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(RuntimeSortDataHolder.m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.FirstRow:
					writer.Write(this.m_firstRow);
					break;
				case MemberName.DataRows:
					writer.Write(this.m_dataRows);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(RuntimeSortDataHolder.m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.FirstRow:
					this.m_firstRow = (DataFieldRow)reader.ReadRIFObject();
					break;
				case MemberName.DataRows:
					this.m_dataRows = reader.ReadRIFObject<ScalableList<DataFieldRow>>();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeSortDataHolder;
		}

		public static Declaration GetDeclaration()
		{
			if (RuntimeSortDataHolder.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.FirstRow, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataFieldRow));
				list.Add(new MemberInfo(MemberName.DataRows, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableList));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeSortDataHolder, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return RuntimeSortDataHolder.m_declaration;
		}
	}
}
