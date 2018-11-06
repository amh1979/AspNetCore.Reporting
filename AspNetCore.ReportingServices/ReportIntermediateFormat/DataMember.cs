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
	internal sealed class DataMember : ReportHierarchyNode
	{
		private DataMemberList m_dataMembers;

		[NonSerialized]
		private bool m_subtotal;

		[NonSerialized]
		private DataMember m_parentMember;

		[NonSerialized]
		private static readonly Declaration m_Declaration = DataMember.GetDeclaration();

		[NonSerialized]
		private DataGroupExprHost m_exprHost;

		internal override string RdlElementName
		{
			get
			{
				return "DataMember";
			}
		}

		internal override HierarchyNodeList InnerHierarchy
		{
			get
			{
				return this.m_dataMembers;
			}
		}

		internal DataMemberList SubMembers
		{
			get
			{
				return this.m_dataMembers;
			}
			set
			{
				this.m_dataMembers = value;
			}
		}

		internal DataGroupExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal DataMember ParentMember
		{
			get
			{
				return this.m_parentMember;
			}
			set
			{
				this.m_parentMember = value;
			}
		}

		internal bool Subtotal
		{
			get
			{
				return this.m_subtotal;
			}
			set
			{
				this.m_subtotal = value;
			}
		}

		internal DataMember()
		{
		}

		internal DataMember(int id, CustomReportItem crItem)
			: base(id, crItem)
		{
		}

		protected override void DataGroupStart(AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder builder)
		{
			builder.DataGroupStart(AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.CustomReportItem, base.m_isColumn);
		}

		protected override int DataGroupEnd(AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder builder)
		{
			return builder.DataGroupEnd(AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.CustomReportItem, base.m_isColumn);
		}

		internal override object PublishClone(AutomaticSubtotalContext context, DataRegion newContainingRegion, bool isSubtotal)
		{
			if (isSubtotal && base.m_grouping != null)
			{
				context.RegisterScopeName(base.m_grouping.Name);
			}
			DataMember dataMember = (DataMember)base.PublishClone(context, newContainingRegion, isSubtotal);
			if (this.m_dataMembers != null)
			{
				dataMember.m_dataMembers = new DataMemberList(this.m_dataMembers.Count);
				foreach (DataMember dataMember4 in this.m_dataMembers)
				{
					DataMember dataMember3 = (DataMember)dataMember4.PublishClone(context, newContainingRegion, isSubtotal);
					dataMember3.ParentMember = this;
					dataMember.m_dataMembers.Add(dataMember3);
				}
			}
			if (this.m_dataMembers == null && isSubtotal)
			{
				RowList rows = context.CurrentDataRegion.Rows;
				if (base.m_isColumn)
				{
					for (int i = 0; i < rows.Count; i++)
					{
						Cell value = (Cell)rows[i].Cells[context.CurrentIndex].PublishClone(context);
						context.CellLists[i].Add(value);
					}
				}
				else
				{
					context.Rows.Add((Row)rows[context.CurrentIndex].PublishClone(context));
				}
				context.CurrentIndex++;
			}
			return dataMember;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.DataMembers, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataMember));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataMember, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportHierarchyNode, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(DataMember.m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.DataMembers)
				{
					writer.Write(this.m_dataMembers);
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
			reader.RegisterDeclaration(DataMember.m_Declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.DataMembers)
				{
					this.m_dataMembers = reader.ReadListOfRIFObjects<DataMemberList>();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataMember;
		}

		internal override void SetExprHost(IMemberNode memberExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID >= 0)
			{
				Global.Tracer.Assert(memberExprHost != null && reportObjectModel != null);
				this.m_exprHost = (DataGroupExprHost)memberExprHost;
				this.m_exprHost.SetReportObjectModel(reportObjectModel);
				base.MemberNodeSetExprHost(this.m_exprHost, reportObjectModel);
			}
		}

		internal override void MemberContentsSetExprHost(ObjectModelImpl reportObjectModel, bool traverseDataRegions)
		{
		}
	}
}
