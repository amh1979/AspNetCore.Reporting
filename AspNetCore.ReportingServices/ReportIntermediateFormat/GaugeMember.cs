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
	internal sealed class GaugeMember : ReportHierarchyNode, IPersistable
	{
		private GaugeMemberList m_innerMembers;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GaugeMember.GetDeclaration();

		internal override string RdlElementName
		{
			get
			{
				return "GaugeMember";
			}
		}

		internal override HierarchyNodeList InnerHierarchy
		{
			get
			{
				return this.m_innerMembers;
			}
		}

		internal GaugeMember ChildGaugeMember
		{
			get
			{
				if (this.m_innerMembers != null && this.m_innerMembers.Count > 0)
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
						this.m_innerMembers = new GaugeMemberList();
					}
					else
					{
						this.m_innerMembers.Clear();
					}
					this.m_innerMembers.Add(value);
				}
			}
		}

		internal GaugeMember()
		{
		}

		internal GaugeMember(int id, GaugePanel crItem)
			: base(id, crItem)
		{
		}

		internal void SetIsCategoryMember(bool value)
		{
			base.m_isColumn = value;
			if (this.ChildGaugeMember != null)
			{
				this.ChildGaugeMember.SetIsCategoryMember(value);
			}
		}

		protected override void DataGroupStart(AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder builder)
		{
			builder.DataGroupStart(AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.GaugePanel, base.m_isColumn);
		}

		protected override int DataGroupEnd(AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder builder)
		{
			return builder.DataGroupEnd(AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.GaugePanel, base.m_isColumn);
		}

		internal override bool Initialize(InitializationContext context)
		{
			if (!base.m_isColumn)
			{
				if (base.m_grouping != null)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidRowGaugeMemberCannotBeDynamic, Severity.Error, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, context.TablixName, "GaugeMember", "Group", base.m_grouping.Name);
				}
				if (this.m_innerMembers != null)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidRowGaugeMemberCannotContainChildMember, Severity.Error, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, context.TablixName, "GaugeMember");
				}
			}
			else if (this.m_innerMembers != null && this.m_innerMembers.OriginalNodeCount > 1)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidColumnGaugeMemberCannotContainMultipleChildMember, Severity.Error, AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, context.TablixName, "GaugeMember");
			}
			return base.Initialize(context);
		}

		internal override object PublishClone(AutomaticSubtotalContext context, DataRegion newContainingRegion)
		{
			GaugeMember gaugeMember = (GaugeMember)base.PublishClone(context, newContainingRegion);
			if (this.ChildGaugeMember != null)
			{
				gaugeMember.ChildGaugeMember = (GaugeMember)this.ChildGaugeMember.PublishClone(context, newContainingRegion);
			}
			return gaugeMember;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new ReadOnlyMemberInfo(MemberName.GaugeMember, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeMember));
			list.Add(new MemberInfo(MemberName.ColumnMembers, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeMember));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeMember, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportHierarchyNode, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(GaugeMember.m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.ColumnMembers)
				{
					writer.Write(this.m_innerMembers);
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
			reader.RegisterDeclaration(GaugeMember.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.GaugeMember:
					this.ChildGaugeMember = (GaugeMember)reader.ReadRIFObject();
					break;
				case MemberName.ColumnMembers:
					this.m_innerMembers = reader.ReadListOfRIFObjects<GaugeMemberList>();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeMember;
		}

		internal override void SetExprHost(IMemberNode memberExprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(memberExprHost != null && reportObjectModel != null);
			base.MemberNodeSetExprHost(memberExprHost, reportObjectModel);
		}

		internal override void MemberContentsSetExprHost(ObjectModelImpl reportObjectModel, bool traverseDataRegions)
		{
		}
	}
}
