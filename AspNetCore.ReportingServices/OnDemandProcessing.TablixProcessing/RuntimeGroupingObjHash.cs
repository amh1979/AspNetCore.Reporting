using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal sealed class RuntimeGroupingObjHash : RuntimeGroupingObj
	{
		private ScalableDictionary<object, IReference<RuntimeHierarchyObj>> m_hashtable;

		private ScalableDictionary<object, ChildLeafInfo> m_parentInfo;

		[NonSerialized]
		private static Declaration m_declaration = RuntimeGroupingObjHash.GetDeclaration();

		public override int Size
		{
			get
			{
				return base.Size + ItemSizes.SizeOf(this.m_hashtable) + ItemSizes.SizeOf(this.m_parentInfo);
			}
		}

		internal RuntimeGroupingObjHash()
		{
		}

		internal RuntimeGroupingObjHash(RuntimeHierarchyObj owner, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType)
			: base(owner, objectType)
		{
			OnDemandProcessingContext odpContext = owner.OdpContext;
			this.m_hashtable = new ScalableDictionary<object, IReference<RuntimeHierarchyObj>>(owner.Depth + 1, odpContext.TablixProcessingScalabilityCache, 101, 27, odpContext.ProcessingComparer);
		}

		internal override void Cleanup()
		{
			if (this.m_hashtable != null)
			{
				this.m_hashtable.Dispose();
				this.m_hashtable = null;
			}
			if (this.m_parentInfo != null)
			{
				this.m_parentInfo.Dispose();
				this.m_parentInfo = null;
			}
		}

		internal override void NextRow(object keyValue, bool hasParent, object parentKey)
		{
			IReference<RuntimeHierarchyObj> reference = null;
			try
			{
				this.m_hashtable.TryGetValue(keyValue, out reference);
			}
			catch (ReportProcessingException_SpatialTypeComparisonError reportProcessingException_SpatialTypeComparisonError)
			{
				throw new ReportProcessingException(base.m_owner.RegisterSpatialTypeComparisonError(reportProcessingException_SpatialTypeComparisonError.Type));
			}
			catch (ReportProcessingException_ComparisonError e)
			{
				throw new ReportProcessingException(base.m_owner.RegisterComparisonError("GroupExpression", e));
			}
			if (reference != null)
			{
				using (reference.PinValue())
				{
					reference.Value().NextRow();
				}
			}
			else
			{
				RuntimeHierarchyObj runtimeHierarchyObj = new RuntimeHierarchyObj(base.m_owner, base.m_objectType, ((IScope)base.m_owner).Depth + 1);
				reference = (IReference<RuntimeHierarchyObj>)runtimeHierarchyObj.SelfReference;
				try
				{
					this.m_hashtable.Add(keyValue, reference);
					runtimeHierarchyObj = reference.Value();
					runtimeHierarchyObj.NextRow();
					if (hasParent)
					{
						IReference<RuntimeHierarchyObj> reference2 = null;
						IReference<RuntimeGroupLeafObj> reference3 = null;
						try
						{
							this.m_hashtable.TryGetValue(parentKey, out reference2);
						}
						catch (ReportProcessingException_SpatialTypeComparisonError reportProcessingException_SpatialTypeComparisonError2)
						{
							throw new ReportProcessingException(base.m_owner.RegisterSpatialTypeComparisonError(reportProcessingException_SpatialTypeComparisonError2.Type));
						}
						catch (ReportProcessingException_ComparisonError e2)
						{
							throw new ReportProcessingException(base.m_owner.RegisterComparisonError("Parent", e2));
						}
						if (reference2 != null)
						{
							RuntimeHierarchyObj runtimeHierarchyObj2 = reference2.Value();
							Global.Tracer.Assert(null != runtimeHierarchyObj2.HierarchyObjs, "(null != parentHierarchyObj.HierarchyObjs)");
							reference3 = (RuntimeGroupLeafObjReference)runtimeHierarchyObj2.HierarchyObjs[0];
						}
						Global.Tracer.Assert(null != runtimeHierarchyObj.HierarchyObjs, "(null != hierarchyObj.HierarchyObjs)");
						RuntimeGroupLeafObjReference runtimeGroupLeafObjReference = (RuntimeGroupLeafObjReference)runtimeHierarchyObj.HierarchyObjs[0];
						bool addToWaitList = true;
						if (reference3 == runtimeGroupLeafObjReference)
						{
							reference3 = null;
							addToWaitList = false;
						}
						this.ProcessChildren(keyValue, reference3, runtimeGroupLeafObjReference);
						this.ProcessParent(parentKey, reference3, runtimeGroupLeafObjReference, addToWaitList);
					}
				}
				finally
				{
					reference.UnPinValue();
				}
			}
		}

		internal override void Traverse(ProcessingStages operation, bool ascending, ITraversalContext traversalContext)
		{
			RuntimeGroupRootObj runtimeGroupRootObj = base.m_owner as RuntimeGroupRootObj;
			Global.Tracer.Assert(null != runtimeGroupRootObj, "(null != groupRootOwner)");
			runtimeGroupRootObj.TraverseLinkedGroupLeaves(operation, ascending, traversalContext);
		}

		internal override void CopyDomainScopeGroupInstances(RuntimeGroupRootObj destination)
		{
			OnDemandProcessingContext odpContext = base.m_owner.OdpContext;
			DomainScopeContext domainScopeContext = odpContext.DomainScopeContext;
			domainScopeContext.CurrentDomainScope = new DomainScopeContext.DomainScopeInfo();
			domainScopeContext.CurrentDomainScope.InitializeKeys((base.m_owner as RuntimeGroupRootObj).GroupExpressions.Count);
			this.CopyDomainScopeGroupInstance(destination, this.m_hashtable);
			domainScopeContext.CurrentDomainScope = null;
		}

		private void CopyDomainScopeGroupInstance(RuntimeGroupRootObj destination, ScalableDictionary<object, IReference<RuntimeHierarchyObj>> runtimeHierarchyObjRefs)
		{
			IReference<RuntimeHierarchyObj> reference = null;
			DomainScopeContext.DomainScopeInfo currentDomainScope = base.m_owner.OdpContext.DomainScopeContext.CurrentDomainScope;
			foreach (object key in runtimeHierarchyObjRefs.Keys)
			{
				currentDomainScope.AddKey(key);
				reference = runtimeHierarchyObjRefs[key];
				using (reference.PinValue())
				{
					RuntimeHierarchyObj runtimeHierarchyObj = reference.Value();
					if (runtimeHierarchyObj.HierarchyObjs == null)
					{
						RuntimeGroupingObjHash runtimeGroupingObjHash = (RuntimeGroupingObjHash)runtimeHierarchyObj.Grouping;
						this.CopyDomainScopeGroupInstance(destination, runtimeGroupingObjHash.m_hashtable);
					}
					else
					{
						Global.Tracer.Assert(runtimeHierarchyObj.HierarchyObjs.Count == 1, "hierarchyObject.HierarchyObjs.Count == 1");
						IReference<RuntimeHierarchyObj> reference2 = runtimeHierarchyObj.HierarchyObjs[0];
						using (reference2.PinValue())
						{
							RuntimeDataTablixGroupLeafObj runtimeDataTablixGroupLeafObj = (RuntimeDataTablixGroupLeafObj)reference2.Value();
							currentDomainScope.CurrentRow = runtimeDataTablixGroupLeafObj.FirstRow;
							destination.NextRow();
						}
					}
				}
				currentDomainScope.RemoveKey();
			}
		}

		private void ProcessParent(object parentKey, IReference<RuntimeGroupLeafObj> parentObj, RuntimeGroupLeafObjReference childObj, bool addToWaitList)
		{
			if (parentObj != null)
			{
				using (parentObj.PinValue())
				{
					parentObj.Value().AddChild(childObj);
				}
			}
			else
			{
				RuntimeGroupRootObj runtimeGroupRootObj = base.m_owner as RuntimeGroupRootObj;
				runtimeGroupRootObj.AddChild(childObj);
				if (addToWaitList)
				{
					ChildLeafInfo childLeafInfo = null;
					IDisposable disposable2 = null;
					try
					{
						if (this.m_parentInfo == null)
						{
							this.m_parentInfo = this.CreateParentInfo();
						}
						else
						{
							this.m_parentInfo.TryGetAndPin(parentKey, out childLeafInfo, out disposable2);
						}
						if (childLeafInfo == null)
						{
							childLeafInfo = new ChildLeafInfo();
							disposable2 = this.m_parentInfo.AddAndPin(parentKey, childLeafInfo);
						}
						childLeafInfo.Add(childObj);
					}
					finally
					{
						if (disposable2 != null)
						{
							disposable2.Dispose();
						}
					}
				}
			}
		}

		private ScalableDictionary<object, ChildLeafInfo> CreateParentInfo()
		{
			OnDemandProcessingContext odpContext = base.m_owner.OdpContext;
			return new ScalableDictionary<object, ChildLeafInfo>(base.m_owner.Depth, odpContext.TablixProcessingScalabilityCache, 101, 27);
		}

		private void ProcessChildren(object thisKey, IReference<RuntimeGroupLeafObj> parentObj, IReference<RuntimeGroupLeafObj> thisObj)
		{
			ChildLeafInfo childLeafInfo = null;
			if (this.m_parentInfo != null)
			{
				this.m_parentInfo.TryGetValue(thisKey, out childLeafInfo);
			}
			if (childLeafInfo != null)
			{
				for (int i = 0; i < childLeafInfo.Count; i++)
				{
					RuntimeGroupLeafObjReference runtimeGroupLeafObjReference = ((List<RuntimeGroupLeafObjReference>)childLeafInfo)[i];
					using (runtimeGroupLeafObjReference.PinValue())
					{
						RuntimeGroupLeafObj runtimeGroupLeafObj = runtimeGroupLeafObjReference.Value();
						bool flag = false;
						IReference<RuntimeGroupObj> reference = parentObj as IReference<RuntimeGroupObj>;
						while (reference != null && !flag)
						{
							RuntimeGroupLeafObj runtimeGroupLeafObj2 = reference.Value() as RuntimeGroupLeafObj;
							if (runtimeGroupLeafObj2 == runtimeGroupLeafObj)
							{
								flag = true;
							}
							reference = ((runtimeGroupLeafObj2 != null) ? runtimeGroupLeafObj2.Parent : null);
						}
						if (!flag)
						{
							runtimeGroupLeafObj.RemoveFromParent((RuntimeGroupRootObjReference)base.m_owner.SelfReference);
							using (thisObj.PinValue())
							{
								thisObj.Value().AddChild(runtimeGroupLeafObjReference);
							}
						}
					}
				}
				this.m_parentInfo.Remove(thisKey);
			}
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(RuntimeGroupingObjHash.m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Hashtable:
					writer.Write(this.m_hashtable);
					break;
				case MemberName.ParentInfo:
					writer.Write(this.m_parentInfo);
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
			reader.RegisterDeclaration(RuntimeGroupingObjHash.m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Hashtable:
					this.m_hashtable = reader.ReadRIFObject<ScalableDictionary<object, IReference<RuntimeHierarchyObj>>>();
					break;
				case MemberName.ParentInfo:
					this.m_parentInfo = reader.ReadRIFObject<ScalableDictionary<object, ChildLeafInfo>>();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupingObjHash;
		}

		public new static Declaration GetDeclaration()
		{
			if (RuntimeGroupingObjHash.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.Hashtable, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionary));
				list.Add(new MemberInfo(MemberName.ParentInfo, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionary));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupingObjHash, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupingObj, list);
			}
			return RuntimeGroupingObjHash.m_declaration;
		}
	}
}
