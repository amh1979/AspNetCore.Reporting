using AspNetCore.ReportingServices.ReportRendering;
using System;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimMatrixMember : ShimTablixMember
	{
		private int m_definitionStartIndex = -1;

		private int m_definitionEndIndex = -1;

		private int m_renderCollectionIndex = -1;

		private bool m_isAfterSubtotal;

		internal MatrixMember m_staticOrSubtotal;

		private MatrixMemberInfoCache m_currentMatrixMemberCellIndexes;

		internal double SizeDelta
		{
			get
			{
				if (base.m_children != null)
				{
					return base.m_children.SizeDelta;
				}
				return 0.0;
			}
		}

		public override bool HideIfNoRows
		{
			get
			{
				if ((base.m_parent == null || base.m_parent.IsStatic) && this.m_staticOrSubtotal != null && this.m_staticOrSubtotal.IsTotal)
				{
					return true;
				}
				return base.HideIfNoRows;
			}
		}

		public override string DataElementName
		{
			get
			{
				if (this.m_staticOrSubtotal != null)
				{
					return this.m_staticOrSubtotal.DataElementName;
				}
				return base.DataElementName;
			}
		}

		public override DataElementOutputTypes DataElementOutput
		{
			get
			{
				if (this.m_staticOrSubtotal != null)
				{
					return (DataElementOutputTypes)this.m_staticOrSubtotal.DataElementOutput;
				}
				return base.DataElementOutput;
			}
		}

		public override TablixHeader TablixHeader
		{
			get
			{
				if (base.m_header == null)
				{
					base.m_header = new TablixHeader(base.OwnerTablix, this);
				}
				return base.m_header;
			}
		}

		public override TablixMemberCollection Children
		{
			get
			{
				return base.m_children;
			}
		}

		public override bool FixedData
		{
			get
			{
				return false;
			}
		}

		public override bool IsStatic
		{
			get
			{
				return null != this.m_staticOrSubtotal;
			}
		}

		internal override int RowSpan
		{
			get
			{
				if (base.m_isColumn)
				{
					return this.CurrentRenderMatrixMember.RowSpan;
				}
				return this.m_definitionEndIndex - this.m_definitionStartIndex;
			}
		}

		internal override int ColSpan
		{
			get
			{
				if (base.m_isColumn)
				{
					return this.m_definitionEndIndex - this.m_definitionStartIndex;
				}
				return this.CurrentRenderMatrixMember.ColumnSpan;
			}
		}

		public override int MemberCellIndex
		{
			get
			{
				return this.m_definitionStartIndex;
			}
		}

		public override bool IsTotal
		{
			get
			{
				if (base.m_group == null && this.m_staticOrSubtotal != null && this.m_staticOrSubtotal.IsTotal && this.m_staticOrSubtotal.Hidden && this.m_staticOrSubtotal.SharedHidden == AspNetCore.ReportingServices.ReportRendering.SharedHiddenState.Always)
				{
					return true;
				}
				return false;
			}
		}

		public override Visibility Visibility
		{
			get
			{
				if (base.m_visibility == null && base.m_group != null && base.m_group.CurrentShimRenderGroup.m_visibilityDef != null)
				{
					base.m_visibility = new ShimMatrixMemberVisibility(this);
				}
				return base.m_visibility;
			}
		}

		internal override PageBreakLocation PropagatedGroupBreak
		{
			get
			{
				if (this.IsStatic)
				{
					return PageBreakLocation.None;
				}
				return base.m_propagatedPageBreak;
			}
		}

		public override bool KeepTogether
		{
			get
			{
				if (base.m_isColumn)
				{
					return false;
				}
				return true;
			}
		}

		public override TablixMemberInstance Instance
		{
			get
			{
				if (base.OwnerTablix.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (base.m_instance == null)
				{
					if (this.IsStatic)
					{
						base.m_instance = new TablixMemberInstance(base.OwnerTablix, this);
					}
					else
					{
						TablixDynamicMemberInstance instance = new TablixDynamicMemberInstance(base.OwnerTablix, this, new InternalShimDynamicMemberLogic(this));
						base.m_owner.RenderingContext.AddDynamicInstance(instance);
						base.m_instance = instance;
					}
				}
				return base.m_instance;
			}
		}

		internal int DefinitionStartIndex
		{
			get
			{
				return this.m_definitionStartIndex;
			}
		}

		internal int DefinitionEndIndex
		{
			get
			{
				return this.m_definitionEndIndex;
			}
		}

		internal int AdjustedRenderCollectionIndex
		{
			get
			{
				if (this.IsStatic)
				{
					return this.m_renderCollectionIndex;
				}
				return this.m_renderCollectionIndex + Math.Max(0, base.m_group.CurrentRenderGroupIndex);
			}
		}

		internal MatrixMemberInfoCache CurrentMatrixMemberCellIndexes
		{
			get
			{
				return this.m_currentMatrixMemberCellIndexes;
			}
		}

		internal MatrixMember CurrentRenderMatrixMember
		{
			get
			{
				if (this.m_staticOrSubtotal != null)
				{
					return this.m_staticOrSubtotal;
				}
				return base.m_group.CurrentShimRenderGroup as MatrixMember;
			}
		}

		internal ShimMatrixMember(IDefinitionPath parentDefinitionPath, Tablix owner, ShimMatrixMember parent, int parentCollectionIndex, bool isColumn, int renderCollectionIndex, MatrixMember staticOrSubtotal, bool isAfterSubtotal, MatrixMemberInfoCache matrixMemberCellIndexes)
			: base(parentDefinitionPath, owner, parent, parentCollectionIndex, isColumn)
		{
			this.m_renderCollectionIndex = renderCollectionIndex;
			this.m_isAfterSubtotal = isAfterSubtotal;
			this.m_currentMatrixMemberCellIndexes = matrixMemberCellIndexes;
			this.m_definitionStartIndex = owner.GetCurrentMemberCellDefinitionIndex();
			this.m_staticOrSubtotal = staticOrSubtotal;
			this.GenerateInnerHierarchy(owner, parent, isColumn, staticOrSubtotal.Children);
			this.m_definitionEndIndex = owner.GetCurrentMemberCellDefinitionIndex();
		}

		internal ShimMatrixMember(IDefinitionPath parentDefinitionPath, Tablix owner, ShimMatrixMember parent, int parentCollectionIndex, bool isColumn, int renderCollectionIndex, ShimRenderGroups renderGroups, MatrixMemberInfoCache matrixMemberCellIndexes)
			: base(parentDefinitionPath, owner, parent, parentCollectionIndex, isColumn)
		{
			this.m_renderCollectionIndex = renderCollectionIndex;
			this.m_currentMatrixMemberCellIndexes = matrixMemberCellIndexes;
			this.m_definitionStartIndex = owner.GetCurrentMemberCellDefinitionIndex();
			base.m_group = new Group(owner, renderGroups, this);
			this.GenerateInnerHierarchy(owner, parent, isColumn, ((MatrixMember)base.m_group.CurrentShimRenderGroup).Children);
			this.m_definitionEndIndex = owner.GetCurrentMemberCellDefinitionIndex();
		}

		private void GenerateInnerHierarchy(Tablix owner, ShimMatrixMember parent, bool isColumn, MatrixMemberCollection children)
		{
			if (children != null)
			{
				MatrixMemberInfoCache matrixMemberInfoCache = null;
				if (base.m_isColumn)
				{
					if (children.MatrixHeadingDef.SubHeading != null)
					{
						matrixMemberInfoCache = new MatrixMemberInfoCache(-1, children.Count);
					}
					else
					{
						int startIndex = (this.m_staticOrSubtotal != null) ? this.m_staticOrSubtotal.MemberCellIndex : this.AdjustedRenderCollectionIndex;
						matrixMemberInfoCache = new MatrixMemberInfoCache(startIndex, -1);
					}
					this.m_currentMatrixMemberCellIndexes.Children[this.AdjustedRenderCollectionIndex] = matrixMemberInfoCache;
				}
				base.m_children = new ShimMatrixMemberCollection(this, owner, isColumn, this, children, matrixMemberInfoCache);
			}
			else
			{
				owner.GetAndIncrementMemberCellDefinitionIndex();
			}
		}

		internal override bool SetNewContext(int index)
		{
			base.ResetContext();
			if (base.m_instance != null)
			{
				base.m_instance.SetNewContext();
			}
			if (base.m_group != null)
			{
				if (base.OwnerTablix.RenderMatrix.NoRows)
				{
					return false;
				}
				if (index >= 0 && index < base.m_group.RenderGroups.Count)
				{
					base.m_group.CurrentRenderGroupIndex = index;
					MatrixMember currentRenderGroup = base.m_group.RenderGroups[index] as MatrixMember;
					this.UpdateMatrixMemberInfoCache(base.m_group.RenderGroups.MatrixMemberCollectionCount, null);
					this.UpdateContext(currentRenderGroup);
					return true;
				}
				return false;
			}
			return index <= 1;
		}

		internal override void ResetContext()
		{
			base.ResetContext();
			if (base.m_group.CurrentRenderGroupIndex >= 0)
			{
				this.ResetContext(null, -1, null, null);
			}
		}

		internal void ResetContext(MatrixMember staticOrSubtotal, int newAfterSubtotalCollectionIndex, ShimRenderGroups renderGroups, MatrixMemberInfoCache newMatrixMemberCellIndexes)
		{
			int currentAllocationSize = 1;
			if (base.m_group != null)
			{
				base.m_group.CurrentRenderGroupIndex = -1;
				if (renderGroups != null)
				{
					base.m_group.RenderGroups = renderGroups;
				}
				currentAllocationSize = base.m_group.RenderGroups.MatrixMemberCollectionCount;
			}
			else if (staticOrSubtotal != null)
			{
				this.m_staticOrSubtotal = staticOrSubtotal;
				if (this.m_isAfterSubtotal && newAfterSubtotalCollectionIndex >= 0)
				{
					this.m_renderCollectionIndex = newAfterSubtotalCollectionIndex;
				}
			}
			this.UpdateMatrixMemberInfoCache(currentAllocationSize, newMatrixMemberCellIndexes);
			if (this.IsStatic)
			{
				this.UpdateContext(this.m_staticOrSubtotal);
			}
			else if (!base.OwnerTablix.RenderMatrix.NoRows && base.m_group.RenderGroups != null && base.m_group.RenderGroups.Count > 0)
			{
				this.UpdateContext(base.m_group.RenderGroups[0] as MatrixMember);
			}
		}

		private void UpdateContext(MatrixMember currentRenderGroup)
		{
			if (base.m_header != null)
			{
				base.m_header.ResetCellContents();
			}
			if (base.m_children != null)
			{
				((ShimMatrixMemberCollection)base.m_children).ResetContext(currentRenderGroup.Children);
			}
			else
			{
				((ShimMatrixRowCollection)base.OwnerTablix.Body.RowCollection).UpdateCells(this);
			}
		}

		private void UpdateMatrixMemberInfoCache(int currentAllocationSize, MatrixMemberInfoCache newMatrixMemberCellIndexes)
		{
			if (base.m_isColumn)
			{
				MatrixMemberInfoCache matrixMemberInfoCache = (base.m_parent == null) ? null : ((ShimMatrixMember)base.m_parent).CurrentMatrixMemberCellIndexes;
				if (matrixMemberInfoCache == null)
				{
					if (newMatrixMemberCellIndexes != null)
					{
						this.m_currentMatrixMemberCellIndexes = newMatrixMemberCellIndexes;
					}
				}
				else
				{
					int adjustedRenderCollectionIndex = ((ShimMatrixMember)base.m_parent).AdjustedRenderCollectionIndex;
					MatrixMemberInfoCache matrixMemberInfoCache2 = matrixMemberInfoCache.Children[adjustedRenderCollectionIndex];
					if (matrixMemberInfoCache2 == null)
					{
						matrixMemberInfoCache2 = ((base.m_children == null) ? new MatrixMemberInfoCache(matrixMemberInfoCache.GetCellIndex((ShimMatrixMember)base.m_parent), -1) : new MatrixMemberInfoCache(-1, currentAllocationSize));
						matrixMemberInfoCache.Children[adjustedRenderCollectionIndex] = matrixMemberInfoCache2;
					}
					this.m_currentMatrixMemberCellIndexes = matrixMemberInfoCache2;
				}
			}
		}
	}
}
