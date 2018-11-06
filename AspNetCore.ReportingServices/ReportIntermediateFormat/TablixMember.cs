using Microsoft.Cloud.Platform.Utils;
using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
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
	internal class TablixMember : ReportHierarchyNode, IPersistable, IVisibilityOwner, IReferenceable
	{
		private class VisibilityState
		{
			internal DataRegionMemberInstance MemberInstance;

			internal bool CachedHiddenValue;

			internal bool HasCachedHidden;

			internal bool CachedDeepHiddenValue;

			internal bool HasCachedDeepHidden;

			internal bool CachedStartHiddenValue;

			internal bool HasCachedStartHidden;

			internal void Reset()
			{
				this.CachedHiddenValue = false;
				this.HasCachedHidden = false;
				this.CachedDeepHiddenValue = false;
				this.HasCachedDeepHidden = false;
				this.CachedStartHiddenValue = false;
				this.HasCachedStartHidden = false;
				this.MemberInstance = null;
			}
		}

		private TablixHeader m_tablixHeader;

		private TablixMemberList m_tablixMembers;

		private Visibility m_visibility;

		private PageBreakLocation m_propagatedPageBreakLocation;

		private bool m_keepTogether;

		private bool m_fixedData;

		private KeepWithGroup m_keepWithGroup;

		private bool m_repeatOnNewPage;

		private string m_dataElementName;

		private DataElementOutputTypes m_dataElementOutput = DataElementOutputTypes.Auto;

		private bool m_hideIfNoRows;

		[Reference]
		private List<TextBox> m_inScopeTextBoxes;

		[Reference]
		private IVisibilityOwner m_containingDynamicVisibility;

		[Reference]
		private IVisibilityOwner m_containingDynamicRowVisibility;

		[Reference]
		private IVisibilityOwner m_containingDynamicColumnVisibility;

		[NonSerialized]
		private bool m_keepTogetherSpecified;

		[NonSerialized]
		private TablixMember m_parentMember;

		[NonSerialized]
		private bool[] m_headerLevelHasStaticArray;

		[NonSerialized]
		private int m_headerLevel = -1;

		[NonSerialized]
		private bool m_isInnerMostMemberWithHeader;

		[NonSerialized]
		private bool m_hasStaticPeerWithHeader;

		[NonSerialized]
		private int m_resizedForLevel;

		[NonSerialized]
		private bool m_canHaveSpanDecreased;

		[NonSerialized]
		private int m_consecutiveZeroHeightDescendentCount;

		[NonSerialized]
		private int m_consecutiveZeroHeightAncestorCount;

		[NonSerialized]
		private static readonly Declaration m_Declaration = TablixMember.GetDeclaration();

		[NonSerialized]
		private string m_senderUniqueName;

		[NonSerialized]
		private int? m_parentInstanceIndex = null;

		[NonSerialized]
		private bool? m_instanceHasRecursiveChildren = null;

		[NonSerialized]
		private IList<DataRegionMemberInstance> m_memberInstances;

		[NonSerialized]
		private TablixMemberExprHost m_exprHost;

		[NonSerialized]
		private IReportScopeInstance m_romScopeInstance;

		[NonSerialized]
		private List<VisibilityState> m_recursiveVisibilityCache;

		[NonSerialized]
		private VisibilityState m_nonRecursiveVisibilityCache;

		internal override string RdlElementName
		{
			get
			{
				return "TablixMember";
			}
		}

		internal override HierarchyNodeList InnerHierarchy
		{
			get
			{
				return this.m_tablixMembers;
			}
		}

		internal TablixMemberList SubMembers
		{
			get
			{
				return this.m_tablixMembers;
			}
			set
			{
				this.m_tablixMembers = value;
			}
		}

		public Visibility Visibility
		{
			get
			{
				return this.m_visibility;
			}
			set
			{
				this.m_visibility = value;
			}
		}

		internal TablixMember ParentMember
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

		internal bool HasStaticPeerWithHeader
		{
			get
			{
				return this.m_hasStaticPeerWithHeader;
			}
			set
			{
				this.m_hasStaticPeerWithHeader = value;
			}
		}

		internal PageBreakLocation PropagatedPageBreakLocation
		{
			get
			{
				return this.m_propagatedPageBreakLocation;
			}
			set
			{
				this.m_propagatedPageBreakLocation = value;
			}
		}

		internal TablixHeader TablixHeader
		{
			get
			{
				return this.m_tablixHeader;
			}
			set
			{
				this.m_tablixHeader = value;
			}
		}

		internal bool FixedData
		{
			get
			{
				return this.m_fixedData;
			}
			set
			{
				this.m_fixedData = value;
			}
		}

		internal bool RepeatOnNewPage
		{
			get
			{
				return this.m_repeatOnNewPage;
			}
			set
			{
				this.m_repeatOnNewPage = value;
			}
		}

		internal KeepWithGroup KeepWithGroup
		{
			get
			{
				return this.m_keepWithGroup;
			}
			set
			{
				this.m_keepWithGroup = value;
			}
		}

		internal bool KeepTogether
		{
			get
			{
				return this.m_keepTogether;
			}
			set
			{
				this.m_keepTogether = value;
			}
		}

		internal bool KeepTogetherSpecified
		{
			get
			{
				return this.m_keepTogetherSpecified;
			}
			set
			{
				this.m_keepTogetherSpecified = value;
			}
		}

		internal bool HideIfNoRows
		{
			get
			{
				return this.m_hideIfNoRows;
			}
			set
			{
				this.m_hideIfNoRows = value;
			}
		}

		internal string DataElementName
		{
			get
			{
				return this.m_dataElementName;
			}
			set
			{
				this.m_dataElementName = value;
			}
		}

		internal DataElementOutputTypes DataElementOutput
		{
			get
			{
				return this.m_dataElementOutput;
			}
			set
			{
				this.m_dataElementOutput = value;
			}
		}

		internal override bool IsNonToggleableHiddenMember
		{
			get
			{
				if (this.m_visibility != null && this.m_visibility.Toggle == null && this.m_visibility.Hidden != null && this.m_visibility.Hidden.Type == ExpressionInfo.Types.Constant)
				{
					return this.m_visibility.Hidden.BoolValue;
				}
				return false;
			}
		}

		private bool WasResized
		{
			get
			{
				return this.m_resizedForLevel > 0;
			}
		}

		internal bool CanHaveSpanDecreased
		{
			get
			{
				return this.m_canHaveSpanDecreased;
			}
			set
			{
				this.m_canHaveSpanDecreased = value;
			}
		}

		internal bool HasToggleableVisibility
		{
			get
			{
				if (this.m_visibility != null)
				{
					return this.m_visibility.Toggle != null;
				}
				return false;
			}
		}

		internal bool HasConditionalOrToggleableVisibility
		{
			get
			{
				if (this.m_visibility != null)
				{
					if (this.m_visibility.Toggle == null)
					{
						if (this.m_visibility.Hidden != null)
						{
							return this.m_visibility.Hidden.Type != ExpressionInfo.Types.Constant;
						}
						return false;
					}
					return true;
				}
				return false;
			}
		}

		internal bool[] HeaderLevelHasStaticArray
		{
			get
			{
				return this.m_headerLevelHasStaticArray;
			}
			set
			{
				this.m_headerLevelHasStaticArray = value;
			}
		}

		internal int HeaderLevel
		{
			get
			{
				return this.m_headerLevel;
			}
			set
			{
				this.m_headerLevel = value;
			}
		}

		internal bool IsInnerMostMemberWithHeader
		{
			get
			{
				return this.m_isInnerMostMemberWithHeader;
			}
			set
			{
				this.m_isInnerMostMemberWithHeader = value;
			}
		}

		internal override bool IsTablixMember
		{
			get
			{
				return true;
			}
		}

		internal List<TextBox> InScopeTextBoxes
		{
			get
			{
				return this.m_inScopeTextBoxes;
			}
		}

		public IReportScopeInstance ROMScopeInstance
		{
			get
			{
				return this.m_romScopeInstance;
			}
			set
			{
				this.m_romScopeInstance = value;
			}
		}

		public IVisibilityOwner ContainingDynamicVisibility
		{
			get
			{
				return this.m_containingDynamicVisibility;
			}
			set
			{
				this.m_containingDynamicVisibility = value;
			}
		}

		public IVisibilityOwner ContainingDynamicColumnVisibility
		{
			get
			{
				return this.m_containingDynamicColumnVisibility;
			}
			set
			{
				this.m_containingDynamicColumnVisibility = value;
			}
		}

		public IVisibilityOwner ContainingDynamicRowVisibility
		{
			get
			{
				return this.m_containingDynamicRowVisibility;
			}
			set
			{
				this.m_containingDynamicRowVisibility = value;
			}
		}

		public string SenderUniqueName
		{
			get
			{
				if (this.m_senderUniqueName == null && this.m_visibility != null)
				{
					TextBox toggleSender = this.m_visibility.ToggleSender;
					if (toggleSender != null)
					{
						if (toggleSender.RecursiveSender && this.m_visibility.RecursiveReceiver)
						{
							int value = this.GetRecursiveParentIndex().Value;
							if (value >= 0)
							{
								this.m_senderUniqueName = toggleSender.GetRecursiveUniqueName(value);
							}
						}
						else
						{
							this.m_senderUniqueName = toggleSender.UniqueName;
						}
					}
				}
				return this.m_senderUniqueName;
			}
		}

		internal int ConsecutiveZeroHeightDescendentCount
		{
			get
			{
				return this.m_consecutiveZeroHeightDescendentCount;
			}
			set
			{
				this.m_consecutiveZeroHeightDescendentCount = value;
			}
		}

		internal int ConsecutiveZeroHeightAncestorCount
		{
			get
			{
				return this.m_consecutiveZeroHeightAncestorCount;
			}
			set
			{
				this.m_consecutiveZeroHeightAncestorCount = value;
			}
		}

		internal bool InstanceHasRecursiveChildren
		{
			get
			{
				if (this.m_instanceHasRecursiveChildren.HasValue)
				{
					return this.m_instanceHasRecursiveChildren.Value;
				}
				return true;
			}
		}

		internal override List<ReportItem> MemberContentCollection
		{
			get
			{
				List<ReportItem> list = null;
				if (this.m_tablixHeader != null && this.m_tablixHeader.CellContents != null)
				{
					list = new List<ReportItem>((this.m_tablixHeader.AltCellContents == null) ? 1 : 2);
					list.Add(this.m_tablixHeader.CellContents);
					if (this.m_tablixHeader.AltCellContents != null)
					{
						list.Add(this.m_tablixHeader.AltCellContents);
					}
				}
				return list;
			}
		}

		private ToggleCascadeDirection ToggleCascadeDirection
		{
			get
			{
				if (base.IsColumn)
				{
					return ToggleCascadeDirection.Column;
				}
				return ToggleCascadeDirection.Row;
			}
		}

		internal TablixMember()
		{
		}

		internal TablixMember(int id, Tablix tablixDef)
			: base(id, tablixDef)
		{
		}

		internal override void TraverseMemberScopes(IRIFScopeVisitor visitor)
		{
			if (this.m_tablixHeader != null)
			{
				if (this.m_tablixHeader.CellContents != null)
				{
					this.m_tablixHeader.CellContents.TraverseScopes(visitor);
				}
				if (this.m_tablixHeader.AltCellContents != null)
				{
					this.m_tablixHeader.AltCellContents.TraverseScopes(visitor);
				}
			}
		}

		public bool ComputeHidden(AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext renderingContext, ToggleCascadeDirection direction)
		{
			VisibilityState cachedVisibilityState = this.GetCachedVisibilityState(renderingContext.OdpContext);
			if (!cachedVisibilityState.HasCachedHidden)
			{
				cachedVisibilityState.HasCachedHidden = true;
				bool flag = false;
				int? nullable = this.SetupParentRecursiveIndex(renderingContext.OdpContext);
				if (nullable.HasValue && nullable < 0 && this.IsRecursiveToggleReceiver())
				{
					flag = true;
					cachedVisibilityState.CachedHiddenValue = false;
				}
				else
				{
					cachedVisibilityState.CachedHiddenValue = Visibility.ComputeHidden((IVisibilityOwner)this, renderingContext, direction, out flag);
				}
				if (flag)
				{
					cachedVisibilityState.HasCachedDeepHidden = true;
					cachedVisibilityState.CachedDeepHiddenValue = cachedVisibilityState.CachedHiddenValue;
				}
			}
			return cachedVisibilityState.CachedHiddenValue;
		}

		public bool ComputeDeepHidden(AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext renderingContext, ToggleCascadeDirection direction)
		{
			VisibilityState cachedVisibilityState = this.GetCachedVisibilityState(renderingContext.OdpContext);
			if (!cachedVisibilityState.HasCachedDeepHidden)
			{
				direction = this.ToggleCascadeDirection;
				bool flag = false;
				flag = ((!cachedVisibilityState.HasCachedHidden) ? this.ComputeHidden(renderingContext, direction) : cachedVisibilityState.CachedHiddenValue);
				if (!cachedVisibilityState.HasCachedDeepHidden)
				{
					cachedVisibilityState.HasCachedDeepHidden = true;
					cachedVisibilityState.CachedDeepHiddenValue = Visibility.ComputeDeepHidden(flag, this, direction, renderingContext);
				}
			}
			return cachedVisibilityState.CachedDeepHiddenValue;
		}

		public bool ComputeToggleSenderDeepHidden(AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext renderingContext)
		{
			bool flag = false;
			ToggleCascadeDirection toggleCascadeDirection = this.ToggleCascadeDirection;
			TablixMember recursiveMember = this.GetRecursiveMember();
			if (recursiveMember != null)
			{
				int? nullable = this.SetupParentRecursiveIndex(renderingContext.OdpContext);
				if (nullable.Value >= 0)
				{
					IList<DataRegionMemberInstance> memberInstances = recursiveMember.m_memberInstances;
					DataRegionMemberInstance dataRegionMemberInstance = memberInstances[nullable.Value];
					int? parentInstanceIndex = recursiveMember.m_parentInstanceIndex;
					IList<DataRegionMemberInstance> memberInstances2 = recursiveMember.m_memberInstances;
					bool? instanceHasRecursiveChildren = recursiveMember.m_instanceHasRecursiveChildren;
					int instanceIndex = recursiveMember.InstancePathItem.InstanceIndex;
					recursiveMember.InstancePathItem.SetContext(nullable.Value);
					this.m_romScopeInstance.IsNewContext = true;
					recursiveMember.ResetVisibilityComputationCache();
					if (this != recursiveMember)
					{
						this.ResetVisibilityComputationCache();
					}
					flag |= this.m_visibility.ToggleSender.ComputeDeepHidden(renderingContext, toggleCascadeDirection);
					recursiveMember.InstancePathItem.SetContext(instanceIndex);
					this.m_romScopeInstance.IsNewContext = true;
					recursiveMember.m_parentInstanceIndex = parentInstanceIndex;
					recursiveMember.m_memberInstances = memberInstances2;
					recursiveMember.m_instanceHasRecursiveChildren = instanceHasRecursiveChildren;
				}
			}
			return flag;
		}

		public bool ComputeStartHidden(AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext renderingContext)
		{
			VisibilityState cachedVisibilityState = this.GetCachedVisibilityState(renderingContext.OdpContext);
			if (!cachedVisibilityState.HasCachedStartHidden)
			{
				cachedVisibilityState.HasCachedStartHidden = true;
				if (this.m_visibility == null || this.m_visibility.Hidden == null)
				{
					cachedVisibilityState.CachedStartHiddenValue = false;
				}
				else if (!this.m_visibility.Hidden.IsExpression)
				{
					cachedVisibilityState.CachedStartHiddenValue = this.m_visibility.Hidden.BoolValue;
				}
				else
				{
					cachedVisibilityState.CachedStartHiddenValue = this.EvaluateStartHidden(this.m_romScopeInstance, renderingContext.OdpContext);
				}
			}
			return cachedVisibilityState.CachedStartHiddenValue;
		}

		internal void ResetVisibilityComputationCache()
		{
			if (this.m_nonRecursiveVisibilityCache != null)
			{
				this.m_nonRecursiveVisibilityCache.Reset();
			}
			this.m_parentInstanceIndex = null;
			this.m_senderUniqueName = null;
		}

		protected override void DataGroupStart(AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder builder)
		{
			builder.DataGroupStart(AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.Tablix, base.m_isColumn);
		}

		protected override int DataGroupEnd(AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder builder)
		{
			return builder.DataGroupEnd(AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.Tablix, base.m_isColumn);
		}

		internal override object PublishClone(AutomaticSubtotalContext context, DataRegion newContainingRegion)
		{
			TablixMember tablixMember = (TablixMember)base.PublishClone(context, newContainingRegion);
			if (this.m_tablixHeader != null)
			{
				tablixMember.m_tablixHeader = (TablixHeader)this.m_tablixHeader.PublishClone(context, false);
			}
			if (this.m_tablixMembers != null)
			{
				tablixMember.m_tablixMembers = new TablixMemberList(this.m_tablixMembers.Count);
				foreach (TablixMember tablixMember4 in this.m_tablixMembers)
				{
					TablixMember tablixMember3 = (TablixMember)tablixMember4.PublishClone(context, newContainingRegion);
					tablixMember3.ParentMember = tablixMember;
					tablixMember.m_tablixMembers.Add(tablixMember3);
				}
			}
			if (this.m_visibility != null)
			{
				tablixMember.m_visibility = (Visibility)this.m_visibility.PublishClone(context, false);
			}
			return tablixMember;
		}

		internal TablixMember CreateAutomaticSubtotalClone(AutomaticSubtotalContext context, TablixMember parent, bool isDynamicTarget, out int aDynamicsRemoved, ref bool aAllWereDynamic)
		{
			TablixMember tablixMember = null;
			TablixMember tablixMember2 = null;
			aDynamicsRemoved = 0;
			int num = -1;
			if (base.m_grouping != null)
			{
				context.RegisterScopeName(base.m_grouping.Name);
			}
			if (!isDynamicTarget && base.m_grouping != null && !context.DynamicWithStaticPeerEncountered)
			{
				if (base.m_isColumn && context.OriginalColumnCount > 1)
				{
					goto IL_0068;
				}
				if (!base.m_isColumn && context.OriginalRowCount > 1)
				{
					goto IL_0068;
				}
				goto IL_01ea;
			}
			goto IL_00ba;
			IL_02f3:
			if (this.m_tablixMembers != null)
			{
				int num2 = 2147483647;
				bool flag = true;
				int count = tablixMember2.m_tablixMembers.Count;
				foreach (TablixMember tablixMember7 in this.m_tablixMembers)
				{
					int val = 0;
					TablixMember tablixMember4 = tablixMember7.CreateAutomaticSubtotalClone(context, tablixMember2, false, out val, ref flag);
					if (tablixMember4 != null)
					{
						if (tablixMember7.m_grouping == null)
						{
							flag = false;
							num2 = 0;
						}
						tablixMember2.m_tablixMembers.Add(tablixMember4);
					}
					num2 = Math.Min(num2, val);
				}
				aDynamicsRemoved += num2;
				aAllWereDynamic &= flag;
				if (tablixMember != null && (base.m_grouping == null || isDynamicTarget))
				{
					for (int i = count; i < tablixMember2.m_tablixMembers.Count; i++)
					{
						TablixMember tablixMember5 = tablixMember2.m_tablixMembers[i];
						if (tablixMember5.m_canHaveSpanDecreased)
						{
							if (flag)
							{
								if (base.m_isColumn)
								{
									tablixMember5.RowSpan = 1;
								}
								else
								{
									tablixMember5.ColSpan = 1;
								}
							}
							else if (base.m_isColumn)
							{
								if (tablixMember5.RowSpan > 1)
								{
									tablixMember5.RowSpan -= num2;
								}
							}
							else if (tablixMember5.ColSpan > 1)
							{
								tablixMember5.ColSpan -= num2;
							}
							tablixMember5.m_canHaveSpanDecreased = false;
							tablixMember5.m_resizedForLevel = 1;
						}
					}
				}
			}
			else
			{
				RowList rows = context.CurrentDataRegion.Rows;
				if (base.m_isColumn)
				{
					for (int j = 0; j < rows.Count; j++)
					{
						Cell value = (Cell)rows[j].Cells[context.CurrentIndex].PublishClone(context);
						context.CellLists[j].Add(value);
					}
					TablixColumn tablixColumn = (TablixColumn)((Tablix)context.CurrentDataRegion).TablixColumns[context.CurrentIndex].PublishClone(context);
					tablixColumn.ForAutoSubtotal = true;
					context.TablixColumns.Add(tablixColumn);
				}
				else
				{
					TablixRow tablixRow = (TablixRow)rows[context.CurrentIndex].PublishClone(context);
					tablixRow.ForAutoSubtotal = true;
					context.Rows.Add(tablixRow);
				}
				context.CurrentIndex++;
			}
			if (tablixMember != null && tablixMember.m_tablixMembers != null && tablixMember.m_tablixMembers.Count == 0)
			{
				tablixMember.m_tablixMembers = null;
			}
			return tablixMember;
			IL_0068:
			if (!context.HasStaticPeerWithHeader(this, out num) && (this.m_parentMember.m_tablixMembers.Count <= 1 || !this.m_isInnerMostMemberWithHeader || this.HasStaticAncestorWithOneMemberGenerations(this)) && (this.m_isInnerMostMemberWithHeader || this.m_tablixMembers != null || this.HasInnermostHeaderAncestorWithOneMemberGenerations(this)))
			{
				goto IL_01ea;
			}
			goto IL_00ba;
			IL_00ba:
			tablixMember = (TablixMember)base.PublishClone(context, null, true);
			tablixMember2 = tablixMember;
			tablixMember2.DataElementOutput = DataElementOutputTypes.NoOutput;
			tablixMember2.ParentMember = parent;
			if (parent != null)
			{
				tablixMember2.m_level = parent.m_level + 1;
			}
			else
			{
				Global.Tracer.Assert(tablixMember2.m_level == 0, "(currentClone.m_level == 0)");
			}
			if (this.m_tablixHeader != null)
			{
				tablixMember2.m_headerLevel = context.HeaderLevel;
				context.HeaderLevel++;
				tablixMember2.m_tablixHeader = (TablixHeader)this.m_tablixHeader.PublishClone(context, base.m_grouping != null);
			}
			if (base.m_grouping != null)
			{
				if (num > 0 || (context.HasStaticPeerWithHeader(this, out num) && num > 0))
				{
					if (base.m_isColumn)
					{
						tablixMember2.RowSpan -= num;
					}
					else
					{
						tablixMember2.ColSpan -= num;
					}
					tablixMember2.m_resizedForLevel = 1;
				}
				if (!context.DynamicWithStaticPeerEncountered)
				{
					tablixMember2.m_canHaveSpanDecreased = true;
				}
			}
			if (this.m_visibility != null)
			{
				tablixMember2.m_visibility = (Visibility)this.m_visibility.PublishClone(context, isDynamicTarget);
			}
			if (this.m_tablixMembers != null)
			{
				tablixMember2.m_tablixMembers = new TablixMemberList(this.m_tablixMembers.Count);
			}
			goto IL_02f3;
			IL_01ea:
			if (this.m_tablixMembers != null)
			{
				tablixMember2 = parent;
				if (base.m_isColumn)
				{
					aDynamicsRemoved = base.RowSpan;
				}
				else
				{
					aDynamicsRemoved = base.ColSpan;
				}
			}
			if (this.m_tablixHeader != null)
			{
				context.HeaderLevel++;
			}
			if (this.m_tablixHeader != null)
			{
				TablixMember tablixMember6 = parent;
				while (tablixMember6.m_tablixHeader == null)
				{
					if (tablixMember6.m_parentMember != null)
					{
						tablixMember6 = tablixMember6.m_parentMember;
					}
				}
				if (tablixMember6.m_tablixHeader != null && tablixMember6.m_resizedForLevel < this.m_headerLevel)
				{
					if (base.m_isColumn)
					{
						tablixMember6.RowSpan += base.RowSpan;
						Global.Tracer.Assert(this.m_headerLevel > 0, "(this.m_headerLevel > 0)");
						tablixMember6.m_resizedForLevel = this.m_headerLevel + base.RowSpan - 1;
					}
					else
					{
						tablixMember6.ColSpan += base.ColSpan;
						Global.Tracer.Assert(this.m_headerLevel > 0, "(this.m_headerLevel > 0)");
						tablixMember6.m_resizedForLevel = this.m_headerLevel + base.ColSpan - 1;
					}
				}
			}
			goto IL_02f3;
		}

		private bool HasStaticAncestorWithOneMemberGenerations(TablixMember member)
		{
			if (member.ParentMember != null)
			{
				if (member.ParentMember.Grouping == null)
				{
					if (member.ParentMember.SubMembers.Count == 1)
					{
						return true;
					}
				}
				else if (member.ParentMember.SubMembers.Count == 1)
				{
					return this.HasStaticAncestorWithOneMemberGenerations(member.ParentMember);
				}
			}
			return false;
		}

		private bool HasInnermostHeaderAncestorWithOneMemberGenerations(TablixMember member)
		{
			if (member.ParentMember != null)
			{
				if (member.ParentMember.m_isInnerMostMemberWithHeader)
				{
					if (member.ParentMember.SubMembers.Count == 1)
					{
						return true;
					}
				}
				else if (member.ParentMember.SubMembers.Count == 1)
				{
					return this.HasInnermostHeaderAncestorWithOneMemberGenerations(member.ParentMember);
				}
			}
			return false;
		}

		internal override bool InnerInitialize(InitializationContext context, bool restrictive)
		{
			context.RegisterMemberReportItems(this, true, restrictive);
			if (this.m_visibility != null)
			{
				this.m_visibility.Initialize(context, false);
			}
			bool result = base.InnerInitialize(context, restrictive);
			if (this.m_tablixHeader != null)
			{
				this.m_tablixHeader.Initialize(context, base.m_isColumn, this.WasResized);
			}
			context.UnRegisterMemberReportItems(this, true, restrictive);
			return result;
		}

		internal override bool Initialize(InitializationContext context, bool restrictive)
		{
			if (this.m_visibility != null)
			{
				string objectName = null;
				AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType = AspNetCore.ReportingServices.ReportProcessing.ObjectType.Tablix;
				if (base.m_grouping != null)
				{
					objectName = context.ObjectName;
					objectType = context.ObjectType;
					context.ObjectType = AspNetCore.ReportingServices.ReportProcessing.ObjectType.Grouping;
					context.ObjectName = base.m_grouping.Name;
				}
				VisibilityToggleInfo visibilityToggleInfo = this.m_visibility.RegisterVisibilityToggle(context);
				if (visibilityToggleInfo != null)
				{
					visibilityToggleInfo.IsTablixMember = true;
				}
				if (base.m_grouping != null)
				{
					if (visibilityToggleInfo != null)
					{
						visibilityToggleInfo.GroupName = base.m_grouping.Name;
					}
					context.ObjectName = objectName;
					context.ObjectType = objectType;
				}
			}
			bool flag = context.RegisterVisibility(this.m_visibility, this);
			if (!this.m_hideIfNoRows && base.m_grouping == null)
			{
				((Tablix)context.GetCurrentDataRegion()).HideStaticsIfNoRows = false;
			}
			if (!base.m_isColumn)
			{
				Tablix.ValidateKeepWithGroup(this.m_tablixMembers, context);
			}
			bool flag2 = base.Initialize(context, restrictive);
			if (this.m_keepWithGroup != 0 && flag2)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidKeepWithGroupOnDynamicTablixMember, Severity.Error, context.ObjectType, context.ObjectName, "TablixMember", "KeepWithGroup", KeepWithGroup.None.ToString());
				this.m_keepWithGroup = KeepWithGroup.None;
			}
			this.DataRendererInitialize(context);
			if (flag)
			{
				context.UnRegisterVisibility(this.m_visibility, this);
			}
			return flag2;
		}

		internal void DataRendererInitialize(InitializationContext context)
		{
			if (this.m_dataElementOutput == DataElementOutputTypes.Auto)
			{
				if (base.m_grouping != null || (this.m_tablixHeader != null && this.m_tablixHeader.CellContents != null))
				{
					this.m_dataElementOutput = DataElementOutputTypes.Output;
				}
				else
				{
					this.m_dataElementOutput = DataElementOutputTypes.ContentsOnly;
				}
			}
			string defaultName = string.Empty;
			if (base.m_grouping != null)
			{
				defaultName = base.m_grouping.Name + "_Collection";
			}
			else if (this.m_tablixHeader != null && this.m_tablixHeader.CellContents != null)
			{
				defaultName = this.m_tablixHeader.CellContents.DataElementName;
			}
			AspNetCore.ReportingServices.ReportPublishing.CLSNameValidator.ValidateDataElementName(ref this.m_dataElementName, defaultName, context.ObjectType, context.ObjectName, "DataElementName", context.ErrorContext);
		}

		internal override bool PreInitializeDataMember(InitializationContext context)
		{
			if (this.m_tablixHeader != null)
			{
				if (this.WasResized)
				{
					double headerSize = context.GetHeaderSize(base.m_isColumn, this.m_headerLevel, base.m_isColumn ? base.m_rowSpan : base.m_colSpan);
					this.m_tablixHeader.SizeValue = Math.Round(headerSize, 10);
					this.m_tablixHeader.Size = AspNetCore.ReportingServices.ReportPublishing.Converter.ConvertSize(headerSize);
				}
				else if (this.m_headerLevel > -1)
				{
					context.ValidateHeaderSize(this.m_tablixHeader.SizeValue, this.m_headerLevel, base.m_isColumn ? base.m_rowSpan : base.m_colSpan, base.m_isColumn, base.m_memberCellIndex);
				}
			}
			bool result = context.RegisterVisibility(this.m_visibility, this);
			context.RegisterMemberReportItems(this, false);
			return result;
		}

		internal override void PostInitializeDataMember(InitializationContext context, bool registeredVisibility)
		{
			context.UnRegisterMemberReportItems(this, false);
			if (registeredVisibility)
			{
				context.UnRegisterVisibility(this.m_visibility, this);
			}
			base.PostInitializeDataMember(context, registeredVisibility);
		}

		internal override void InitializeRVDirectionDependentItems(InitializationContext context)
		{
			if (this.m_tablixHeader != null)
			{
				if (this.m_tablixHeader.CellContents != null)
				{
					this.m_tablixHeader.CellContents.InitializeRVDirectionDependentItems(context);
				}
				if (this.m_tablixHeader.AltCellContents != null)
				{
					this.m_tablixHeader.AltCellContents.InitializeRVDirectionDependentItems(context);
				}
			}
		}

		internal override void DetermineGroupingExprValueCount(InitializationContext context, int groupingExprCount)
		{
			if (this.m_tablixHeader != null)
			{
				if (this.m_tablixHeader.CellContents != null)
				{
					this.m_tablixHeader.CellContents.DetermineGroupingExprValueCount(context, groupingExprCount);
				}
				if (this.m_tablixHeader.AltCellContents != null)
				{
					this.m_tablixHeader.AltCellContents.DetermineGroupingExprValueCount(context, groupingExprCount);
				}
			}
		}

		internal void ValidateTablixMemberForBanding(PublishingContextStruct context, out bool isdynamic)
		{
			isdynamic = false;
			this.SetIgnoredPropertiesForBandingToDefault(context);
			if (!base.IsStatic)
			{
				isdynamic = true;
				if (this.Visibility != null && this.Visibility.IsToggleReceiver)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidBandShouldNotBeTogglable, Severity.Error, context.ObjectType, context.ObjectName, base.Grouping.Name.MarkAsModelInfo());
				}
				if (base.Grouping.PageBreak != null)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidBandPageBreakIsSet, Severity.Error, context.ObjectType, context.ObjectName, base.Grouping.Name.MarkAsModelInfo());
				}
			}
		}

		internal void SetIgnoredPropertiesForBandingToDefault(PublishingContextStruct context)
		{
			if (base.CustomProperties != null)
			{
				base.CustomProperties = null;
				context.ErrorContext.Register(ProcessingErrorCode.rsBandIgnoredProperties, Severity.Warning, context.ObjectType, context.ObjectName, "CustomProperties");
			}
			if (this.FixedData)
			{
				this.FixedData = false;
				context.ErrorContext.Register(ProcessingErrorCode.rsBandIgnoredProperties, Severity.Warning, context.ObjectType, context.ObjectName, "FixedData");
			}
			if (this.HideIfNoRows)
			{
				this.HideIfNoRows = false;
				context.ErrorContext.Register(ProcessingErrorCode.rsBandIgnoredProperties, Severity.Warning, context.ObjectType, context.ObjectName, "HideIfNoRows");
			}
			if (this.KeepWithGroup != 0)
			{
				this.KeepWithGroup = KeepWithGroup.None;
				context.ErrorContext.Register(ProcessingErrorCode.rsBandIgnoredProperties, Severity.Warning, context.ObjectType, context.ObjectName, "KeepWithGroup");
			}
			if (this.RepeatOnNewPage)
			{
				this.RepeatOnNewPage = false;
				context.ErrorContext.Register(ProcessingErrorCode.rsBandIgnoredProperties, Severity.Warning, context.ObjectType, context.ObjectName, "RepeatOnNewPage");
			}
			if (!this.KeepTogether)
			{
				this.KeepTogether = true;
				if (this.KeepTogetherSpecified)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsBandKeepTogetherIgnored, Severity.Warning, context.ObjectType, context.ObjectName, "KeepTogether");
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.TablixHeader, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixHeader));
			list.Add(new MemberInfo(MemberName.TablixMembers, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixMember));
			list.Add(new MemberInfo(MemberName.Visibility, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Visibility));
			list.Add(new MemberInfo(MemberName.PropagatedPageBreakLocation, Token.Enum));
			list.Add(new MemberInfo(MemberName.FixedData, Token.Boolean));
			list.Add(new MemberInfo(MemberName.KeepWithGroup, Token.Enum));
			list.Add(new MemberInfo(MemberName.RepeatOnNewPage, Token.Boolean));
			list.Add(new MemberInfo(MemberName.DataElementName, Token.String));
			list.Add(new MemberInfo(MemberName.DataElementOutput, Token.Enum));
			list.Add(new MemberInfo(MemberName.HideIfNoRows, Token.Boolean));
			list.Add(new MemberInfo(MemberName.KeepTogether, Token.Boolean));
			list.Add(new MemberInfo(MemberName.InScopeTextBoxes, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Token.Reference, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TextBox));
			list.Add(new MemberInfo(MemberName.ContainingDynamicVisibility, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IVisibilityOwner, Token.Reference));
			list.Add(new MemberInfo(MemberName.ContainingDynamicRowVisibility, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IVisibilityOwner, Token.Reference));
			list.Add(new MemberInfo(MemberName.ContainingDynamicColumnVisibility, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IVisibilityOwner, Token.Reference));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixMember, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportHierarchyNode, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(TablixMember.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.TablixHeader:
					writer.Write(this.m_tablixHeader);
					break;
				case MemberName.TablixMembers:
					writer.Write(this.m_tablixMembers);
					break;
				case MemberName.Visibility:
					writer.Write(this.m_visibility);
					break;
				case MemberName.PropagatedPageBreakLocation:
					writer.WriteEnum((int)this.m_propagatedPageBreakLocation);
					break;
				case MemberName.FixedData:
					writer.Write(this.m_fixedData);
					break;
				case MemberName.KeepWithGroup:
					writer.WriteEnum((int)this.m_keepWithGroup);
					break;
				case MemberName.RepeatOnNewPage:
					writer.Write(this.m_repeatOnNewPage);
					break;
				case MemberName.DataElementName:
					writer.Write(this.m_dataElementName);
					break;
				case MemberName.DataElementOutput:
					writer.WriteEnum((int)this.m_dataElementOutput);
					break;
				case MemberName.HideIfNoRows:
					writer.Write(this.m_hideIfNoRows);
					break;
				case MemberName.KeepTogether:
					writer.Write(this.m_keepTogether);
					break;
				case MemberName.InScopeTextBoxes:
					writer.WriteListOfReferences(this.m_inScopeTextBoxes);
					break;
				case MemberName.ContainingDynamicVisibility:
					writer.WriteReference(this.m_containingDynamicVisibility);
					break;
				case MemberName.ContainingDynamicRowVisibility:
					writer.WriteReference(this.m_containingDynamicRowVisibility);
					break;
				case MemberName.ContainingDynamicColumnVisibility:
					writer.WriteReference(this.m_containingDynamicColumnVisibility);
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
			reader.RegisterDeclaration(TablixMember.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.TablixHeader:
					this.m_tablixHeader = (TablixHeader)reader.ReadRIFObject();
					break;
				case MemberName.TablixMembers:
					this.m_tablixMembers = reader.ReadListOfRIFObjects<TablixMemberList>();
					break;
				case MemberName.Visibility:
					this.m_visibility = (Visibility)reader.ReadRIFObject();
					break;
				case MemberName.PropagatedPageBreakLocation:
					this.m_propagatedPageBreakLocation = (PageBreakLocation)reader.ReadEnum();
					break;
				case MemberName.FixedData:
					this.m_fixedData = reader.ReadBoolean();
					break;
				case MemberName.KeepWithGroup:
					this.m_keepWithGroup = (KeepWithGroup)reader.ReadEnum();
					break;
				case MemberName.RepeatOnNewPage:
					this.m_repeatOnNewPage = reader.ReadBoolean();
					break;
				case MemberName.DataElementName:
					this.m_dataElementName = reader.ReadString();
					break;
				case MemberName.DataElementOutput:
					this.m_dataElementOutput = (DataElementOutputTypes)reader.ReadEnum();
					break;
				case MemberName.HideIfNoRows:
					this.m_hideIfNoRows = reader.ReadBoolean();
					break;
				case MemberName.KeepTogether:
					this.m_keepTogether = reader.ReadBoolean();
					break;
				case MemberName.InScopeTextBoxes:
					this.m_inScopeTextBoxes = reader.ReadGenericListOfReferences<TextBox>(this);
					break;
				case MemberName.ContainingDynamicVisibility:
					this.m_containingDynamicVisibility = reader.ReadReference<IVisibilityOwner>(this);
					break;
				case MemberName.ContainingDynamicRowVisibility:
					this.m_containingDynamicRowVisibility = reader.ReadReference<IVisibilityOwner>(this);
					break;
				case MemberName.ContainingDynamicColumnVisibility:
					this.m_containingDynamicColumnVisibility = reader.ReadReference<IVisibilityOwner>(this);
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
			List<MemberReference> list = default(List<MemberReference>);
			if (memberReferencesCollection.TryGetValue(TablixMember.m_Declaration.ObjectType, out list))
			{
				foreach (MemberReference item in list)
				{
					switch (item.MemberName)
					{
					case MemberName.InScopeTextBoxes:
						if (this.m_inScopeTextBoxes == null)
						{
							this.m_inScopeTextBoxes = new List<TextBox>();
						}
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						Global.Tracer.Assert(referenceableItems[item.RefID] is TextBox);
						this.m_inScopeTextBoxes.Add((TextBox)referenceableItems[item.RefID]);
						break;
					case MemberName.ContainingDynamicVisibility:
					{
						IReferenceable referenceable2 = default(IReferenceable);
						if (referenceableItems.TryGetValue(item.RefID, out referenceable2))
						{
							this.m_containingDynamicVisibility = (referenceable2 as IVisibilityOwner);
						}
						break;
					}
					case MemberName.ContainingDynamicRowVisibility:
					{
						IReferenceable referenceable3 = default(IReferenceable);
						if (referenceableItems.TryGetValue(item.RefID, out referenceable3))
						{
							this.m_containingDynamicRowVisibility = (referenceable3 as IVisibilityOwner);
						}
						break;
					}
					case MemberName.ContainingDynamicColumnVisibility:
					{
						IReferenceable referenceable = default(IReferenceable);
						if (referenceableItems.TryGetValue(item.RefID, out referenceable))
						{
							this.m_containingDynamicColumnVisibility = (referenceable as IVisibilityOwner);
						}
						break;
					}
					default:
						Global.Tracer.Assert(false);
						break;
					}
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixMember;
		}

		internal override void SetExprHost(IMemberNode memberExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID >= 0)
			{
				Global.Tracer.Assert(memberExprHost != null && reportObjectModel != null);
				this.m_exprHost = (TablixMemberExprHost)memberExprHost;
				this.m_exprHost.SetReportObjectModel(reportObjectModel);
				base.MemberNodeSetExprHost(this.m_exprHost, reportObjectModel);
			}
		}

		internal override void MemberContentsSetExprHost(ObjectModelImpl reportObjectModel, bool traverseDataRegions)
		{
			if (this.m_tablixHeader != null && this.m_tablixHeader.CellContents != null)
			{
				reportObjectModel.OdpContext.RuntimeInitializeReportItemObjs(this.m_tablixHeader.CellContents, traverseDataRegions);
				if (this.m_tablixHeader.AltCellContents != null)
				{
					reportObjectModel.OdpContext.RuntimeInitializeReportItemObjs(this.m_tablixHeader.AltCellContents, traverseDataRegions);
				}
			}
		}

		internal override void MoveNextForUserSort(OnDemandProcessingContext odpContext)
		{
			base.MoveNextForUserSort(odpContext);
			this.ResetTextBoxImpls(odpContext);
		}

		protected override void AddInScopeTextBox(TextBox textbox)
		{
			if (this.m_inScopeTextBoxes == null)
			{
				this.m_inScopeTextBoxes = new List<TextBox>();
			}
			this.m_inScopeTextBoxes.Add(textbox);
		}

		internal override void ResetTextBoxImpls(OnDemandProcessingContext context)
		{
			if (this.m_inScopeTextBoxes != null)
			{
				for (int i = 0; i < this.m_inScopeTextBoxes.Count; i++)
				{
					this.m_inScopeTextBoxes[i].ResetTextBoxImpl(context);
				}
			}
		}

		internal bool EvaluateStartHidden(IReportScopeInstance romInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, romInstance);
			string objectName = base.IsStatic ? base.m_dataRegionDef.Name : base.m_grouping.Name;
			return context.ReportRuntime.EvaluateStartHiddenExpression(this.m_visibility, this.m_exprHost, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Tablix, objectName);
		}

		internal override void SetMemberInstances(IList<DataRegionMemberInstance> memberInstances)
		{
			this.m_memberInstances = memberInstances;
		}

		internal override void SetRecursiveParentIndex(int parentInstanceIndex)
		{
			if (this.m_parentInstanceIndex.HasValue && parentInstanceIndex == this.m_parentInstanceIndex)
			{
				return;
			}
			this.m_senderUniqueName = null;
			this.m_parentInstanceIndex = parentInstanceIndex;
		}

		internal override void SetInstanceHasRecursiveChildren(bool? hasRecursiveChildren)
		{
			this.m_instanceHasRecursiveChildren = hasRecursiveChildren;
		}

		private int? SetupParentRecursiveIndex(OnDemandProcessingContext odpContext)
		{
			if (this.IsRecursive())
			{
				if (!this.m_parentInstanceIndex.HasValue)
				{
					odpContext.SetupContext(this, this.m_romScopeInstance);
				}
			}
			else if (this.IsToggleableChildOfRecursive())
			{
				this.m_parentInstanceIndex = this.m_visibility.RecursiveMember.SetupParentRecursiveIndex(odpContext);
			}
			return this.m_parentInstanceIndex;
		}

		private VisibilityState GetCachedVisibilityState(OnDemandProcessingContext odpContext)
		{
			return this.GetCachedVisibilityState(odpContext, -2147483648);
		}

		private VisibilityState GetCachedVisibilityState(OnDemandProcessingContext odpContext, int memberIndex)
		{
			TablixMember recursiveMember = this.GetRecursiveMember();
			if (recursiveMember != null && this.m_visibility != null && this.m_visibility.RecursiveReceiver)
			{
				if (memberIndex == -2147483648)
				{
					if (!recursiveMember.m_parentInstanceIndex.HasValue)
					{
						recursiveMember.SetupParentRecursiveIndex(odpContext);
					}
					memberIndex = recursiveMember.CurrentMemberIndex;
				}
				DataRegionMemberInstance dataRegionMemberInstance = recursiveMember.m_memberInstances[memberIndex];
				int recursiveLevel = dataRegionMemberInstance.RecursiveLevel;
				if (this.m_recursiveVisibilityCache == null)
				{
					this.m_recursiveVisibilityCache = new List<VisibilityState>();
				}
				VisibilityState visibilityState = null;
				if (recursiveLevel >= this.m_recursiveVisibilityCache.Count)
				{
					for (int i = this.m_recursiveVisibilityCache.Count; i <= recursiveLevel; i++)
					{
						visibilityState = new VisibilityState();
						this.m_recursiveVisibilityCache.Add(visibilityState);
					}
				}
				else
				{
					visibilityState = this.m_recursiveVisibilityCache[recursiveLevel];
				}
				if (visibilityState.MemberInstance != dataRegionMemberInstance)
				{
					visibilityState.Reset();
					visibilityState.MemberInstance = dataRegionMemberInstance;
				}
				return visibilityState;
			}
			if (this.m_nonRecursiveVisibilityCache == null)
			{
				this.m_nonRecursiveVisibilityCache = new VisibilityState();
			}
			else if (this.m_romScopeInstance != null && this.m_romScopeInstance.IsNewContext)
			{
				this.m_nonRecursiveVisibilityCache.Reset();
			}
			return this.m_nonRecursiveVisibilityCache;
		}

		private TablixMember GetRecursiveMember()
		{
			TablixMember result = null;
			if (this.IsRecursive())
			{
				result = this;
			}
			else if (this.IsToggleableChildOfRecursive())
			{
				result = this.m_visibility.RecursiveMember;
			}
			return result;
		}

		private int? GetRecursiveParentIndex()
		{
			return this.m_parentInstanceIndex;
		}

		private bool IsRecursive()
		{
			if (base.m_grouping != null)
			{
				return base.m_grouping.Parent != null;
			}
			return false;
		}

		private bool IsToggleableChildOfRecursive()
		{
			if (this.m_visibility != null)
			{
				return this.m_visibility.RecursiveMember != null;
			}
			return false;
		}

		private bool IsRecursiveToggleReceiver()
		{
			if (this.m_visibility != null && this.m_visibility.Toggle != null)
			{
				return this.m_visibility.RecursiveReceiver;
			}
			return false;
		}
	}
}
