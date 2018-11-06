using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal abstract class RuntimeMemberObj : IStorable, IPersistable
	{
		protected IReference<IScope> m_owner;

		protected RuntimeDataTablixGroupRootObjReference m_groupRoot;

		private static Declaration m_declaration = RuntimeMemberObj.GetDeclaration();

		internal RuntimeDataTablixGroupRootObjReference GroupRoot
		{
			get
			{
				return this.m_groupRoot;
			}
		}

		public virtual int Size
		{
			get
			{
				return ItemSizes.SizeOf(this.m_owner) + ItemSizes.SizeOf(this.m_groupRoot);
			}
		}

		internal RuntimeMemberObj()
		{
		}

		internal RuntimeMemberObj(IReference<IScope> owner, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode dynamicMember)
		{
			this.m_owner = owner;
		}

		internal virtual void NextRow(bool isOuterGrouping, OnDemandProcessingContext odpContext)
		{
			if ((BaseReference)null != (object)this.m_groupRoot)
			{
				using (this.m_groupRoot.PinValue())
				{
					this.m_groupRoot.Value().NextRow();
				}
			}
		}

		internal virtual bool SortAndFilter(AggregateUpdateContext aggContext)
		{
			if ((BaseReference)null != (object)this.m_groupRoot)
			{
				using (this.m_groupRoot.PinValue())
				{
					return this.m_groupRoot.Value().SortAndFilter(aggContext);
				}
			}
			return true;
		}

		internal virtual void UpdateAggregates(AggregateUpdateContext aggContext)
		{
			if ((BaseReference)null != (object)this.m_groupRoot)
			{
				using (this.m_groupRoot.PinValue())
				{
					this.m_groupRoot.Value().UpdateAggregates(aggContext);
				}
			}
		}

		internal virtual void CalculateRunningValues(Dictionary<string, IReference<RuntimeGroupRootObj>> groupCol, IReference<RuntimeGroupRootObj> lastGroup, AggregateUpdateContext aggContext)
		{
			if ((BaseReference)null != (object)this.m_groupRoot)
			{
				using (this.m_groupRoot.PinValue())
				{
					this.m_groupRoot.Value().CalculateRunningValues(groupCol, lastGroup, aggContext);
				}
			}
		}

		internal virtual void PrepareCalculateRunningValues()
		{
			if ((BaseReference)null != (object)this.m_groupRoot)
			{
				using (this.m_groupRoot.PinValue())
				{
					this.m_groupRoot.Value().PrepareCalculateRunningValues();
				}
			}
		}

		internal abstract void CreateInstances(IReference<RuntimeDataRegionObj> outerGroupRef, OnDemandProcessingContext odpContext, DataRegionInstance dataRegionInstance, bool outerGroupings, IReference<RuntimeDataTablixGroupRootObj> currOuterGroupRoot, ScopeInstance parentInstance, IReference<RuntimeMemberObj>[] innerMembers, IReference<RuntimeDataTablixGroupLeafObj> innerGroupLeafRef);

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(RuntimeMemberObj.m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Owner:
					writer.Write(this.m_owner);
					break;
				case MemberName.GroupRoot:
					writer.Write(this.m_groupRoot);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public virtual void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(RuntimeMemberObj.m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Owner:
					this.m_owner = (IReference<IScope>)reader.ReadRIFObject();
					break;
				case MemberName.GroupRoot:
					this.m_groupRoot = (RuntimeDataTablixGroupRootObjReference)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public virtual void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public virtual AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeMemberObj;
		}

		public static Declaration GetDeclaration()
		{
			if (RuntimeMemberObj.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.Owner, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IScopeReference));
				list.Add(new MemberInfo(MemberName.GroupRoot, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixGroupRootObjReference));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeMemberObj, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return RuntimeMemberObj.m_declaration;
		}
	}
}
