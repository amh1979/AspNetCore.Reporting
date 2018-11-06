using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal class GroupTreePartition : IPersistable
	{
		private List<IReference<ScopeInstance>> m_topLevelScopeInstances;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GroupTreePartition.GetDeclaration();

		internal bool IsEmpty
		{
			get
			{
				return null == this.m_topLevelScopeInstances;
			}
		}

		internal List<IReference<ScopeInstance>> TopLevelScopeInstances
		{
			get
			{
				return this.m_topLevelScopeInstances;
			}
		}

		internal GroupTreePartition()
		{
		}

		internal void AddTopLevelScopeInstance(IReference<ScopeInstance> instance)
		{
			if (this.m_topLevelScopeInstances == null)
			{
				this.m_topLevelScopeInstances = new List<IReference<ScopeInstance>>();
			}
			this.m_topLevelScopeInstances.Add(instance);
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.TopLevelScopeInstances, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScopeInstanceReference));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GroupTreePartition, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(GroupTreePartition.m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.TopLevelScopeInstances)
				{
					writer.Write(this.m_topLevelScopeInstances);
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
		}

		public virtual void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(GroupTreePartition.m_Declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.TopLevelScopeInstances)
				{
					this.m_topLevelScopeInstances = reader.ReadGenericListOfRIFObjects<IReference<ScopeInstance>>();
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
			reader.ResolveReferences();
		}

		public virtual void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false);
		}

		public virtual AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GroupTreePartition;
		}
	}
}
