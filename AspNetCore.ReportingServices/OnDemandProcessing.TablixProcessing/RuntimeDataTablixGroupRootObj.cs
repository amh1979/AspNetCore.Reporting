using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal class RuntimeDataTablixGroupRootObj : RuntimeGroupRootObj
	{
		private List<int> m_recursiveParentIndexes;

		private IReference<RuntimeMemberObj>[] m_innerGroupings;

		private List<string> m_cellRVs;

		private List<string> m_staticCellRVs;

		private List<string> m_cellPreviousValues;

		private List<string> m_staticCellPreviousValues;

		private int m_headingLevel;

		private bool m_outermostStatics;

		private bool m_hasLeafCells;

		private bool m_processOutermostStaticCells;

		private bool m_processStaticCellsForRVs;

		private int m_currentMemberIndexWithinScopeLevel = -1;

		[NonSerialized]
		private DataRegionMemberInstance m_currentMemberInstance;

		[NonSerialized]
		private static Declaration m_declaration = RuntimeDataTablixGroupRootObj.GetDeclaration();

		internal IReference<RuntimeMemberObj>[] InnerGroupings
		{
			get
			{
				return this.m_innerGroupings;
			}
		}

		internal int HeadingLevel
		{
			get
			{
				return this.m_headingLevel;
			}
		}

		internal bool OutermostStatics
		{
			get
			{
				return this.m_outermostStatics;
			}
		}

		internal bool ProcessOutermostStaticCells
		{
			get
			{
				return this.m_processOutermostStaticCells;
			}
		}

		internal bool HasLeafCells
		{
			get
			{
				return this.m_hasLeafCells;
			}
		}

		internal object CurrentGroupExpressionValue
		{
			get
			{
				return base.m_currentGroupExprValue;
			}
		}

		internal int CurrentMemberIndexWithinScopeLevel
		{
			get
			{
				return this.m_currentMemberIndexWithinScopeLevel;
			}
			set
			{
				this.m_currentMemberIndexWithinScopeLevel = value;
			}
		}

		internal DataRegionMemberInstance CurrentMemberInstance
		{
			get
			{
				return this.m_currentMemberInstance;
			}
			set
			{
				this.m_currentMemberInstance = value;
			}
		}

		public override int Size
		{
			get
			{
				return base.Size + ItemSizes.SizeOf(this.m_innerGroupings) + ItemSizes.SizeOf(this.m_cellRVs) + ItemSizes.SizeOf(this.m_staticCellRVs) + ItemSizes.SizeOf(this.m_cellPreviousValues) + ItemSizes.SizeOf(this.m_staticCellPreviousValues) + ItemSizes.SizeOf(this.m_recursiveParentIndexes) + 4 + 1 + 1 + 1 + 4 + 1 + ItemSizes.ReferenceSize;
			}
		}

		internal RuntimeDataTablixGroupRootObj()
		{
		}

		internal RuntimeDataTablixGroupRootObj(IReference<IScope> outerScope, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode dynamicMember, ref DataActions dataAction, OnDemandProcessingContext odpContext, IReference<RuntimeMemberObj>[] innerGroupings, bool outermostStatics, int headingLevel, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType)
			: base(outerScope, dynamicMember, dataAction, odpContext, objectType)
		{
			this.m_innerGroupings = innerGroupings;
			this.m_headingLevel = headingLevel;
			this.m_outermostStatics = outermostStatics;
			this.m_hasLeafCells = false;
			HierarchyNodeList innerStaticMembersInSameScope = dynamicMember.InnerStaticMembersInSameScope;
			this.m_hasLeafCells = (!dynamicMember.HasInnerDynamic || (innerStaticMembersInSameScope != null && innerStaticMembersInSameScope.LeafCellIndexes != null));
			if (innerGroupings == null && innerStaticMembersInSameScope != null && innerStaticMembersInSameScope.LeafCellIndexes != null)
			{
				goto IL_0075;
			}
			if (innerGroupings != null && outermostStatics)
			{
				goto IL_0075;
			}
			goto IL_0084;
			IL_0084:
			if (this.m_hasLeafCells && outermostStatics)
			{
				this.m_processOutermostStaticCells = true;
			}
			this.NeedProcessDataActions(dynamicMember);
			this.NeedProcessDataActions(dynamicMember.InnerStaticMembersInSameScope);
			if (dynamicMember.Grouping.Filters == null)
			{
				dataAction = DataActions.None;
			}
			if (!this.m_processOutermostStaticCells && !this.m_processStaticCellsForRVs)
			{
				return;
			}
			if (dynamicMember.DataRegionDef.CellPostSortAggregates == null && dynamicMember.DataRegionDef.CellRunningValues == null)
			{
				return;
			}
			base.m_dataAction |= DataActions.PostSortAggregates;
			return;
			IL_0075:
			if (this.m_hasLeafCells)
			{
				this.m_processStaticCellsForRVs = true;
			}
			goto IL_0084;
		}

		private void NeedProcessDataActions(HierarchyNodeList members)
		{
			if (members != null)
			{
				for (int i = 0; i < members.Count; i++)
				{
					this.NeedProcessDataActions(members[i]);
				}
			}
		}

		private void NeedProcessDataActions(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode memberDefinition)
		{
			if (memberDefinition != null)
			{
				this.NeedProcessDataActions(memberDefinition.RunningValues);
			}
		}

		private void NeedProcessDataActions(List<AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo> runningValues)
		{
			if ((base.m_dataAction & DataActions.PostSortAggregates) == DataActions.None && runningValues != null && 0 < runningValues.Count)
			{
				base.m_dataAction |= DataActions.PostSortAggregates;
			}
		}

		protected override void UpdateDataRegionGroupRootInfo()
		{
			if (this.m_innerGroupings != null)
			{
				base.HierarchyDef.DataRegionDef.CurrentOuterGroupRootObjs[base.m_hierarchyDef.HierarchyDynamicIndex] = (RuntimeDataTablixGroupRootObjReference)base.SelfReference;
			}
		}

		internal virtual void PrepareCalculateRunningValues()
		{
			base.TraverseGroupOrSortTree(ProcessingStages.PreparePeerGroupRunningValues, null);
		}

		internal override void CalculateRunningValues(Dictionary<string, IReference<RuntimeGroupRootObj>> groupCol, IReference<RuntimeGroupRootObj> lastGroup, AggregateUpdateContext aggContext)
		{
			base.CalculateRunningValues(groupCol, lastGroup, aggContext);
			if (this.m_processStaticCellsForRVs || this.m_processOutermostStaticCells)
			{
				base.m_hierarchyDef.DataRegionDef.ProcessOutermostStaticCellRunningValues = true;
				if (this.m_innerGroupings != null)
				{
					base.m_hierarchyDef.DataRegionDef.CurrentOuterGroupRoot = (RuntimeDataTablixGroupRootObjReference)base.SelfReference;
				}
				this.AddCellRunningValues(groupCol, ref this.m_staticCellRVs, ref this.m_staticCellPreviousValues, true);
				base.m_hierarchyDef.DataRegionDef.ProcessOutermostStaticCellRunningValues = false;
			}
			if (this.m_innerGroupings == null)
			{
				IReference<RuntimeDataTablixGroupRootObj> currentOuterGroupRoot = base.m_hierarchyDef.DataRegionDef.CurrentOuterGroupRoot;
				if (currentOuterGroupRoot != null)
				{
					base.m_hierarchyDef.DataRegionDef.ProcessCellRunningValues = true;
					this.m_cellRVs = null;
					this.m_cellPreviousValues = null;
					this.AddCellRunningValues(groupCol, ref this.m_cellRVs, ref this.m_cellPreviousValues, false);
					base.m_hierarchyDef.DataRegionDef.ProcessCellRunningValues = false;
				}
			}
			base.AddRunningValues(base.m_hierarchyDef.RunningValues);
			base.AddRunningValuesOfAggregates();
			base.TraverseGroupOrSortTree(ProcessingStages.RunningValues, aggContext);
			if (base.m_hierarchyDef.Grouping.Name != null)
			{
				groupCol.Remove(base.m_hierarchyDef.Grouping.Name);
			}
		}

		private void AddCellRunningValues(Dictionary<string, IReference<RuntimeGroupRootObj>> groupCol, ref List<string> runningValues, ref List<string> previousValues, bool outermostStatics)
		{
			if (base.m_hierarchyDef.DataRegionDef.CellRunningValues != null && 0 < base.m_hierarchyDef.DataRegionDef.CellRunningValues.Count && base.AddRunningValues(base.m_hierarchyDef.DataRegionDef.CellRunningValues, ref runningValues, ref previousValues, groupCol, true, outermostStatics))
			{
				base.m_dataAction |= DataActions.PostSortAggregates;
			}
		}

		internal override void CalculatePreviousAggregates()
		{
			if (FlagUtils.HasFlag(base.m_dataAction, DataActions.PostSortAggregates))
			{
				AggregatesImpl aggregatesImpl = base.m_odpContext.ReportObjectModel.AggregatesImpl;
				if (base.m_hierarchyDef.DataRegionDef.ProcessCellRunningValues)
				{
					if (this.m_cellPreviousValues != null)
					{
						for (int i = 0; i < this.m_cellPreviousValues.Count; i++)
						{
							string text = this.m_cellPreviousValues[i];
							AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregateObj = aggregatesImpl.GetAggregateObj(text);
							Global.Tracer.Assert(aggregateObj != null, "Missing expected previous aggregate: {0}", text);
							aggregateObj.Update();
						}
					}
					if (base.m_outerScope != null && (base.m_outerDataAction & DataActions.PostSortAggregates) != 0)
					{
						using (base.m_outerScope.PinValue())
						{
							base.m_outerScope.Value().CalculatePreviousAggregates();
						}
					}
				}
				else
				{
					if (this.m_staticCellPreviousValues != null)
					{
						for (int j = 0; j < this.m_staticCellPreviousValues.Count; j++)
						{
							string text2 = this.m_staticCellPreviousValues[j];
							AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregateObj2 = aggregatesImpl.GetAggregateObj(text2);
							Global.Tracer.Assert(aggregateObj2 != null, "Missing expected previous aggregate: {0}", text2);
							aggregateObj2.Update();
						}
					}
					base.CalculatePreviousAggregates();
				}
			}
		}

		public override void ReadRow(DataActions dataAction, ITraversalContext context)
		{
			if (FlagUtils.HasFlag(dataAction, DataActions.PostSortAggregates) && FlagUtils.HasFlag(base.m_dataAction, DataActions.PostSortAggregates))
			{
				AggregatesImpl aggregatesImpl = base.m_odpContext.ReportObjectModel.AggregatesImpl;
				if (base.m_hierarchyDef.DataRegionDef.ProcessCellRunningValues)
				{
					if (this.m_cellRVs != null)
					{
						for (int i = 0; i < this.m_cellRVs.Count; i++)
						{
							string text = this.m_cellRVs[i];
							AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregateObj = aggregatesImpl.GetAggregateObj(text);
							Global.Tracer.Assert(aggregateObj != null, "Missing expected running value: {0}", text);
							aggregateObj.Update();
						}
					}
					if (base.m_outerScope != null && base.m_hierarchyDef.DataRegionDef.CellPostSortAggregates != null)
					{
						using (base.m_outerScope.PinValue())
						{
							base.m_outerScope.Value().ReadRow(dataAction, context);
						}
					}
				}
				else
				{
					if (this.m_staticCellRVs != null)
					{
						for (int j = 0; j < this.m_staticCellRVs.Count; j++)
						{
							string text2 = this.m_staticCellRVs[j];
							AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregateObj2 = aggregatesImpl.GetAggregateObj(text2);
							Global.Tracer.Assert(aggregateObj2 != null, "Missing expected running value: {0}", text2);
							aggregateObj2.Update();
						}
					}
					base.ReadRow(dataAction, context);
				}
			}
		}

		internal void DoneReadingRows(ref AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[] runningValueValues, ref AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[] runningValueOfAggregateValues, ref AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[] cellRunningValueValues)
		{
			AggregatesImpl aggregatesImpl = base.m_odpContext.ReportObjectModel.AggregatesImpl;
			RuntimeRICollection.StoreRunningValues(aggregatesImpl, base.m_hierarchyDef.RunningValues, ref runningValueValues);
			if (base.m_hierarchyDef.DataScopeInfo != null)
			{
				RuntimeRICollection.StoreRunningValues(aggregatesImpl, base.m_hierarchyDef.DataScopeInfo.RunningValuesOfAggregates, ref runningValueOfAggregateValues);
			}
			int num = (this.m_staticCellPreviousValues != null) ? this.m_staticCellPreviousValues.Count : 0;
			int num2 = (this.m_staticCellRVs != null) ? this.m_staticCellRVs.Count : 0;
			if (num2 > 0)
			{
				cellRunningValueValues = new AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[num2 + num];
				for (int i = 0; i < num2; i++)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregateObj = aggregatesImpl.GetAggregateObj(this.m_staticCellRVs[i]);
					cellRunningValueValues[i] = aggregateObj.AggregateResult();
				}
			}
			if (num > 0)
			{
				if (cellRunningValueValues == null)
				{
					cellRunningValueValues = new AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[num];
				}
				for (int j = 0; j < num; j++)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregateObj2 = aggregatesImpl.GetAggregateObj(this.m_staticCellPreviousValues[j]);
					cellRunningValueValues[num2 + j] = aggregateObj2.AggregateResult();
				}
			}
		}

		internal void SetupCellRunningValues(AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[] cellRunningValueValues)
		{
			if (cellRunningValueValues != null)
			{
				AggregatesImpl aggregatesImpl = base.m_odpContext.ReportObjectModel.AggregatesImpl;
				int num = (this.m_staticCellPreviousValues != null) ? this.m_staticCellPreviousValues.Count : 0;
				int num2 = (this.m_staticCellRVs != null) ? this.m_staticCellRVs.Count : 0;
				if (num2 > 0)
				{
					for (int i = 0; i < num2; i++)
					{
						string text = this.m_staticCellRVs[i];
						AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregateObj = aggregatesImpl.GetAggregateObj(text);
						Global.Tracer.Assert(aggregateObj != null, "Missing expected running value: {0}", text);
						aggregateObj.Set(cellRunningValueValues[i]);
					}
				}
				if (num > 0)
				{
					for (int j = 0; j < num; j++)
					{
						string text2 = this.m_staticCellPreviousValues[j];
						AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregateObj2 = aggregatesImpl.GetAggregateObj(text2);
						Global.Tracer.Assert(aggregateObj2 != null, "Missing expected running value: {0}", text2);
						aggregateObj2.Set(cellRunningValueValues[num2 + j]);
					}
				}
			}
		}

		internal bool GetCellTargetForNonDetailSort()
		{
			using (base.m_outerScope.PinValue())
			{
				IScope scope = base.m_outerScope.Value();
				if (scope is RuntimeTablixObj)
				{
					return scope.TargetForNonDetailSort;
				}
				Global.Tracer.Assert(scope is RuntimeTablixGroupLeafObj, "(outerScopeObj is RuntimeTablixGroupLeafObj)");
				return ((RuntimeTablixGroupLeafObj)scope).GetCellTargetForNonDetailSort();
			}
		}

		internal bool GetCellTargetForSort(int index, bool detailSort)
		{
			using (base.m_outerScope.PinValue())
			{
				IScope scope = base.m_outerScope.Value();
				if (scope is RuntimeTablixObj)
				{
					return scope.IsTargetForSort(index, detailSort);
				}
				Global.Tracer.Assert(scope is RuntimeTablixGroupLeafObj, "(outerScopeObj is RuntimeTablixGroupLeafObj)");
				return ((RuntimeTablixGroupLeafObj)scope).GetCellTargetForSort(index, detailSort);
			}
		}

		internal int GetRecursiveParentIndex(int recursiveLevel)
		{
			return this.m_recursiveParentIndexes[recursiveLevel];
		}

		internal void SetRecursiveParentIndex(int instanceIndex, int recursiveLevel)
		{
			if (this.m_recursiveParentIndexes == null)
			{
				this.m_recursiveParentIndexes = new List<int>();
			}
			while (recursiveLevel >= this.m_recursiveParentIndexes.Count)
			{
				this.m_recursiveParentIndexes.Add(-1);
			}
			this.m_recursiveParentIndexes[recursiveLevel] = instanceIndex;
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(RuntimeDataTablixGroupRootObj.m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.InnerGroupings:
					writer.Write(this.m_innerGroupings);
					break;
				case MemberName.CellRunningValues:
					writer.WriteListOfPrimitives(this.m_cellRVs);
					break;
				case MemberName.StaticCellRunningValues:
					writer.WriteListOfPrimitives(this.m_staticCellRVs);
					break;
				case MemberName.CellPreviousValues:
					writer.WriteListOfPrimitives(this.m_cellPreviousValues);
					break;
				case MemberName.StaticCellPreviousValues:
					writer.WriteListOfPrimitives(this.m_staticCellPreviousValues);
					break;
				case MemberName.HeadingLevel:
					writer.Write(this.m_headingLevel);
					break;
				case MemberName.OutermostStatics:
					writer.Write(this.m_outermostStatics);
					break;
				case MemberName.HasLeafCells:
					writer.Write(this.m_hasLeafCells);
					break;
				case MemberName.ProcessOutermostStaticCells:
					writer.Write(this.m_processOutermostStaticCells);
					break;
				case MemberName.CurrentMemberIndexWithinScopeLevel:
					writer.Write(this.m_currentMemberIndexWithinScopeLevel);
					break;
				case MemberName.RecursiveParentIndexes:
					writer.WriteListOfPrimitives(this.m_recursiveParentIndexes);
					break;
				case MemberName.ProcessStaticCellsForRVs:
					writer.Write(this.m_processStaticCellsForRVs);
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
			reader.RegisterDeclaration(RuntimeDataTablixGroupRootObj.m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.InnerGroupings:
					this.m_innerGroupings = reader.ReadArrayOfRIFObjects<IReference<RuntimeMemberObj>>();
					break;
				case MemberName.CellRunningValues:
					this.m_cellRVs = reader.ReadListOfPrimitives<string>();
					break;
				case MemberName.StaticCellRunningValues:
					this.m_staticCellRVs = reader.ReadListOfPrimitives<string>();
					break;
				case MemberName.CellPreviousValues:
					this.m_cellPreviousValues = reader.ReadListOfPrimitives<string>();
					break;
				case MemberName.StaticCellPreviousValues:
					this.m_staticCellPreviousValues = reader.ReadListOfPrimitives<string>();
					break;
				case MemberName.HeadingLevel:
					this.m_headingLevel = reader.ReadInt32();
					break;
				case MemberName.OutermostStatics:
					this.m_outermostStatics = reader.ReadBoolean();
					break;
				case MemberName.HasLeafCells:
					this.m_hasLeafCells = reader.ReadBoolean();
					break;
				case MemberName.ProcessOutermostStaticCells:
					this.m_processOutermostStaticCells = reader.ReadBoolean();
					break;
				case MemberName.CurrentMemberIndexWithinScopeLevel:
					this.m_currentMemberIndexWithinScopeLevel = reader.ReadInt32();
					break;
				case MemberName.RecursiveParentIndexes:
					this.m_recursiveParentIndexes = reader.ReadListOfPrimitives<int>();
					break;
				case MemberName.ProcessStaticCellsForRVs:
					this.m_processStaticCellsForRVs = reader.ReadBoolean();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixGroupRootObj;
		}

		public new static Declaration GetDeclaration()
		{
			if (RuntimeDataTablixGroupRootObj.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.InnerGroupings, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectArray, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeMemberObjReference));
				list.Add(new MemberInfo(MemberName.CellRunningValues, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.String));
				list.Add(new MemberInfo(MemberName.StaticCellRunningValues, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.String));
				list.Add(new MemberInfo(MemberName.CellPreviousValues, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.String));
				list.Add(new MemberInfo(MemberName.StaticCellPreviousValues, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.String));
				list.Add(new MemberInfo(MemberName.HeadingLevel, Token.Int32));
				list.Add(new MemberInfo(MemberName.OutermostStatics, Token.Boolean));
				list.Add(new MemberInfo(MemberName.HasLeafCells, Token.Boolean));
				list.Add(new MemberInfo(MemberName.ProcessOutermostStaticCells, Token.Boolean));
				list.Add(new MemberInfo(MemberName.CurrentMemberIndexWithinScopeLevel, Token.Int32));
				list.Add(new MemberInfo(MemberName.RecursiveParentIndexes, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Int32));
				list.Add(new MemberInfo(MemberName.ProcessStaticCellsForRVs, Token.Boolean));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixGroupRootObj, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupRootObj, list);
			}
			return RuntimeDataTablixGroupRootObj.m_declaration;
		}
	}
}
