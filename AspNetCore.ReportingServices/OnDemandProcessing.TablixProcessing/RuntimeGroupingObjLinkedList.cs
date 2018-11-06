using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal class RuntimeGroupingObjLinkedList : RuntimeGroupingObj
	{
		[NonSerialized]
		private static Declaration m_declaration = RuntimeGroupingObjLinkedList.GetDeclaration();

		public override int Size
		{
			get
			{
				return base.Size;
			}
		}

		internal RuntimeGroupingObjLinkedList()
		{
		}

		internal RuntimeGroupingObjLinkedList(RuntimeHierarchyObj owner, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType)
			: base(owner, objectType)
		{
		}

		internal override void Cleanup()
		{
		}

		internal override void NextRow(object keyValue, bool hasParent, object parentKey)
		{
			Global.Tracer.Assert(false, "This implementation of RuntimeGroupingObj does not support NextRow");
		}

		internal override void Traverse(ProcessingStages operation, bool ascending, ITraversalContext traversalContext)
		{
			RuntimeGroupRootObj runtimeGroupRootObj = base.m_owner as RuntimeGroupRootObj;
			Global.Tracer.Assert(null != runtimeGroupRootObj, "(null != groupRootOwner)");
			runtimeGroupRootObj.TraverseLinkedGroupLeaves(operation, ascending, traversalContext);
		}

		internal override void CopyDomainScopeGroupInstances(RuntimeGroupRootObj destination)
		{
			Global.Tracer.Assert(false, "Domain Scope should only be applied to Hash groups");
		}

		protected RuntimeHierarchyObjReference CreateHierarchyObjAndAddToParent()
		{
			RuntimeHierarchyObjReference runtimeHierarchyObjReference = null;
			try
			{
				RuntimeHierarchyObj runtimeHierarchyObj = new RuntimeHierarchyObj(base.m_owner, base.m_objectType, ((IScope)base.m_owner).Depth + 1);
				runtimeHierarchyObjReference = (RuntimeHierarchyObjReference)runtimeHierarchyObj.SelfReference;
				runtimeHierarchyObj.NextRow();
				return runtimeHierarchyObjReference;
			}
			finally
			{
				if ((BaseReference)null != (object)runtimeHierarchyObjReference)
				{
					runtimeHierarchyObjReference.UnPinValue();
				}
			}
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(RuntimeGroupingObjLinkedList.m_declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				Global.Tracer.Assert(false);
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(RuntimeGroupingObjLinkedList.m_declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				Global.Tracer.Assert(false);
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupingObjLinkedList;
		}

		public new static Declaration GetDeclaration()
		{
			if (RuntimeGroupingObjLinkedList.m_declaration == null)
			{
				List<MemberInfo> memberInfoList = new List<MemberInfo>();
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupingObjLinkedList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupingObj, memberInfoList);
			}
			return RuntimeGroupingObjLinkedList.m_declaration;
		}
	}
}
