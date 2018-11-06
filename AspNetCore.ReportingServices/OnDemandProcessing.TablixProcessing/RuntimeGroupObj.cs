using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal abstract class RuntimeGroupObj : RuntimeHierarchyObj
	{
		protected RuntimeGroupLeafObjReference m_lastChild;

		protected RuntimeGroupLeafObjReference m_firstChild;

		private static Declaration m_declaration = RuntimeGroupObj.GetDeclaration();

		internal RuntimeGroupLeafObjReference LastChild
		{
			get
			{
				return this.m_lastChild;
			}
			set
			{
				this.m_lastChild = value;
			}
		}

		internal RuntimeGroupLeafObjReference FirstChild
		{
			get
			{
				return this.m_firstChild;
			}
			set
			{
				this.m_firstChild = value;
			}
		}

		internal virtual int RecursiveLevel
		{
			get
			{
				return -1;
			}
		}

		public override int Size
		{
			get
			{
				return base.Size + ItemSizes.SizeOf(this.m_lastChild) + ItemSizes.SizeOf(this.m_firstChild);
			}
		}

		protected RuntimeGroupObj()
		{
		}

		protected RuntimeGroupObj(OnDemandProcessingContext odpContext, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, int level)
			: base(odpContext, objectType, level)
		{
		}

		internal void AddChild(RuntimeGroupLeafObjReference child)
		{
			if ((BaseReference)null != (object)this.m_lastChild)
			{
				using (this.m_lastChild.PinValue())
				{
					this.m_lastChild.Value().NextLeaf = child;
				}
			}
			else
			{
				this.m_firstChild = child;
			}
			using (child.PinValue())
			{
				RuntimeGroupLeafObj runtimeGroupLeafObj = child.Value();
				runtimeGroupLeafObj.PrevLeaf = this.m_lastChild;
				runtimeGroupLeafObj.NextLeaf = null;
				runtimeGroupLeafObj.Parent = (RuntimeGroupObjReference)base.m_selfReference;
			}
			this.m_lastChild = child;
		}

		internal void InsertToSortTree(RuntimeGroupLeafObjReference groupLeaf)
		{
			using (base.m_hierarchyRoot.PinValue())
			{
				RuntimeGroupRootObj runtimeGroupRootObj = (RuntimeGroupRootObj)base.m_hierarchyRoot.Value();
				AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping grouping = runtimeGroupRootObj.HierarchyDef.Grouping;
				if (runtimeGroupRootObj.ProcessSecondPassSorting)
				{
					Global.Tracer.Assert(base.m_grouping != null, "(m_grouping != null)");
					runtimeGroupRootObj.LastChild = groupLeaf;
					Global.Tracer.Assert(null != grouping, "(null != groupingDef)");
					object keyValue = base.m_odpContext.ReportRuntime.EvaluateRuntimeExpression(base.m_expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Grouping, grouping.Name, "Sort");
					base.m_grouping.NextRow(keyValue);
				}
				else
				{
					Global.Tracer.Assert(runtimeGroupRootObj.HierarchyDef.HasFilters || runtimeGroupRootObj.HierarchyDef.HasInnerFilters, "(groupRoot.HierarchyDef.HasFilters || groupRoot.HierarchyDef.HasInnerFilters)");
					this.AddChild(groupLeaf);
				}
			}
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(RuntimeGroupObj.m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.FirstChild:
					writer.Write(this.m_firstChild);
					break;
				case MemberName.LastChild:
					writer.Write(this.m_lastChild);
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
			reader.RegisterDeclaration(RuntimeGroupObj.m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.FirstChild:
					this.m_firstChild = (RuntimeGroupLeafObjReference)reader.ReadRIFObject();
					break;
				case MemberName.LastChild:
					this.m_lastChild = (RuntimeGroupLeafObjReference)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupObj;
		}

		public new static Declaration GetDeclaration()
		{
			if (RuntimeGroupObj.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.LastChild, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupLeafObjReference));
				list.Add(new MemberInfo(MemberName.FirstChild, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupLeafObjReference));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupObj, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeHierarchyObj, list);
			}
			return RuntimeGroupObj.m_declaration;
		}
	}
}
