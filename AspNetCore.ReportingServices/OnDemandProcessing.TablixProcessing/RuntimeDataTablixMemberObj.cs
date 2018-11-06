using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal sealed class RuntimeDataTablixMemberObj : RuntimeMemberObj
	{
		private bool m_hasStaticMembers;

		private List<int> m_staticLeafCellIndexes;

		[NonSerialized]
		private static Declaration m_declaration = RuntimeDataTablixMemberObj.GetDeclaration();

		public override int Size
		{
			get
			{
				return base.Size + 1 + ItemSizes.SizeOf(this.m_staticLeafCellIndexes);
			}
		}

		internal RuntimeDataTablixMemberObj()
		{
		}

		private RuntimeDataTablixMemberObj(IReference<IScope> owner, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode dynamicMember, ref DataActions dataAction, OnDemandProcessingContext odpContext, IReference<RuntimeMemberObj>[] innerGroupings, HierarchyNodeList staticMembers, bool outerMostStatics, int headingLevel, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType)
			: base(owner, dynamicMember)
		{
			int excludedCellIndex = -1;
			if (dynamicMember != null)
			{
				excludedCellIndex = dynamicMember.MemberCellIndex;
				RuntimeDataTablixGroupRootObj runtimeDataTablixGroupRootObj = new RuntimeDataTablixGroupRootObj(owner, dynamicMember, ref dataAction, odpContext, innerGroupings, outerMostStatics, headingLevel, objectType);
				base.m_groupRoot = (RuntimeDataTablixGroupRootObjReference)runtimeDataTablixGroupRootObj.SelfReference;
				base.m_groupRoot.UnPinValue();
			}
			if (staticMembers != null && staticMembers.Count != 0)
			{
				int count = staticMembers.Count;
				this.m_hasStaticMembers = true;
				this.m_staticLeafCellIndexes = staticMembers.GetLeafCellIndexes(excludedCellIndex);
			}
		}

		internal static IReference<RuntimeMemberObj> CreateRuntimeMemberObject(IReference<IScope> owner, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode dynamicMemberDef, ref DataActions dataAction, OnDemandProcessingContext odpContext, IReference<RuntimeMemberObj>[] innerGroupings, HierarchyNodeList staticMembers, bool outerMostStatics, int headingLevel, AspNetCore.ReportingServices.ReportProcessing.ObjectType dataRegionType)
		{
			RuntimeDataTablixMemberObj obj = new RuntimeDataTablixMemberObj(owner, dynamicMemberDef, ref dataAction, odpContext, innerGroupings, staticMembers, outerMostStatics, headingLevel, dataRegionType);
			return odpContext.TablixProcessingScalabilityCache.Allocate((RuntimeMemberObj)obj, headingLevel);
		}

		internal override void CreateInstances(IReference<RuntimeDataRegionObj> containingScopeRef, OnDemandProcessingContext odpContext, DataRegionInstance dataRegionInstance, bool isOuterGrouping, IReference<RuntimeDataTablixGroupRootObj> currOuterGroupRoot, ScopeInstance parentInstance, IReference<RuntimeMemberObj>[] innerMembers, IReference<RuntimeDataTablixGroupLeafObj> innerGroupLeaf)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef = dataRegionInstance.DataRegionDef;
			if (isOuterGrouping && this.m_hasStaticMembers)
			{
				dataRegionDef.NewOuterCells();
			}
			if ((BaseReference)null != (object)base.m_groupRoot)
			{
				dataRegionDef.CurrentOuterGroupRoot = currOuterGroupRoot;
				using (base.m_groupRoot.PinValue())
				{
					base.m_groupRoot.Value().CreateInstances(parentInstance, innerMembers, innerGroupLeaf);
				}
				if (this.m_staticLeafCellIndexes != null)
				{
					if (isOuterGrouping && this.m_hasStaticMembers)
					{
						dataRegionDef.NewOuterCells();
					}
					IReference<RuntimeDataTablixGroupRootObj> reference = null;
					if (base.m_owner is IReference<RuntimeDataTablixObj>)
					{
						using (base.m_owner.PinValue())
						{
							((RuntimeDataTablixObj)base.m_owner.Value()).SetupEnvironment();
						}
						if (isOuterGrouping && this.m_hasStaticMembers)
						{
							reference = dataRegionDef.CurrentOuterGroupRoot;
							dataRegionDef.CurrentOuterGroupRoot = null;
							currOuterGroupRoot = null;
						}
					}
					else
					{
						using (containingScopeRef.PinValue())
						{
							containingScopeRef.Value().SetupEnvironment();
						}
					}
					this.CreateCells(containingScopeRef, odpContext, dataRegionInstance, isOuterGrouping, currOuterGroupRoot, parentInstance, innerMembers, innerGroupLeaf);
					if (reference != null)
					{
						dataRegionDef.CurrentOuterGroupRoot = reference;
					}
				}
			}
			else
			{
				this.CreateCells(containingScopeRef, odpContext, dataRegionInstance, isOuterGrouping, currOuterGroupRoot, parentInstance, innerMembers, innerGroupLeaf);
			}
		}

		private void CreateCells(IReference<RuntimeDataRegionObj> containingScopeRef, OnDemandProcessingContext odpContext, DataRegionInstance dataRegionInstance, bool isOuterGroup, IReference<RuntimeDataTablixGroupRootObj> currOuterGroupRoot, ScopeInstance parentInstance, IReference<RuntimeMemberObj>[] innerMembers, IReference<RuntimeDataTablixGroupLeafObj> innerGroupLeafRef)
		{
			using (containingScopeRef.PinValue())
			{
				RuntimeDataRegionObj runtimeDataRegionObj = containingScopeRef.Value();
				if (runtimeDataRegionObj is RuntimeDataTablixGroupLeafObj)
				{
					RuntimeDataTablixGroupLeafObj runtimeDataTablixGroupLeafObj = (RuntimeDataTablixGroupLeafObj)runtimeDataRegionObj;
					runtimeDataTablixGroupLeafObj.CreateStaticCells(dataRegionInstance, parentInstance, currOuterGroupRoot, isOuterGroup, this.m_staticLeafCellIndexes, innerMembers, innerGroupLeafRef);
				}
				else
				{
					((RuntimeDataTablixObj)runtimeDataRegionObj).CreateOutermostStaticCells(dataRegionInstance, isOuterGroup, innerMembers, innerGroupLeafRef);
				}
			}
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(RuntimeDataTablixMemberObj.m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.HasStaticMembers:
					writer.Write(this.m_hasStaticMembers);
					break;
				case MemberName.StaticLeafCellIndexes:
					writer.WriteListOfPrimitives(this.m_staticLeafCellIndexes);
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
			reader.RegisterDeclaration(RuntimeDataTablixMemberObj.m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.HasStaticMembers:
					this.m_hasStaticMembers = reader.ReadBoolean();
					break;
				case MemberName.StaticLeafCellIndexes:
					this.m_staticLeafCellIndexes = reader.ReadListOfPrimitives<int>();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixMemberObj;
		}

		public new static Declaration GetDeclaration()
		{
			if (RuntimeDataTablixMemberObj.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.HasStaticMembers, Token.Boolean));
				list.Add(new MemberInfo(MemberName.StaticLeafCellIndexes, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Int32));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixMemberObj, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeMemberObj, list);
			}
			return RuntimeDataTablixMemberObj.m_declaration;
		}
	}
}
