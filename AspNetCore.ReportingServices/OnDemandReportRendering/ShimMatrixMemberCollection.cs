using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimMatrixMemberCollection : ShimMemberCollection
	{
		private int m_definitionStartIndex = -1;

		private int m_definitionEndIndex = -1;

		private int m_dynamicSubgroupChildIndex = -1;

		private int m_subtotalChildIndex = -1;

		private double m_sizeDelta;

		public override TablixMember this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					return (TablixMember)base.m_children[index];
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		internal override double SizeDelta
		{
			get
			{
				return this.m_sizeDelta;
			}
		}

		public override int Count
		{
			get
			{
				return base.m_children.Length;
			}
		}

		internal PageBreakLocation PropagatedGroupBreakLocation
		{
			get
			{
				if (this.m_dynamicSubgroupChildIndex < 0)
				{
					return PageBreakLocation.None;
				}
				return ((TablixMember)base.m_children[this.m_dynamicSubgroupChildIndex]).PropagatedGroupBreak;
			}
		}

		internal ShimMatrixMemberCollection(IDefinitionPath parentDefinitionPath, Tablix owner, bool isColumnGroup, ShimMatrixMember parent, MatrixMemberCollection renderMemberCollection, MatrixMemberInfoCache matrixMemberCellIndexes)
			: base(parentDefinitionPath, owner, isColumnGroup)
		{
			this.m_definitionStartIndex = owner.GetCurrentMemberCellDefinitionIndex();
			int count = renderMemberCollection.Count;
			if (renderMemberCollection[0].IsStatic)
			{
				base.m_children = new ShimMatrixMember[count];
				for (int i = 0; i < count; i++)
				{
					base.m_children[i] = new ShimMatrixMember(this, owner, parent, i, isColumnGroup, i, renderMemberCollection[i], false, matrixMemberCellIndexes);
				}
			}
			else
			{
				this.m_dynamicSubgroupChildIndex = 0;
				bool flag = null != renderMemberCollection.MatrixHeadingDef.Subtotal;
				bool flag2 = flag && Subtotal.PositionType.After == renderMemberCollection.MatrixHeadingDef.Subtotal.Position;
				base.m_children = new ShimMatrixMember[(!flag) ? 1 : 2];
				if (flag)
				{
					this.m_subtotalChildIndex = 0;
					if (flag2)
					{
						this.m_subtotalChildIndex++;
					}
					else
					{
						this.m_dynamicSubgroupChildIndex++;
					}
				}
				if (flag)
				{
					MatrixMember matrixMember = renderMemberCollection[this.m_subtotalChildIndex];
					AspNetCore.ReportingServices.ReportRendering.ReportItem reportItem = matrixMember.ReportItem;
					if (reportItem != null)
					{
						if (isColumnGroup)
						{
							this.m_sizeDelta += reportItem.Width.ToMillimeters();
						}
						else
						{
							this.m_sizeDelta += reportItem.Height.ToMillimeters();
						}
					}
				}
				if (flag && !flag2)
				{
					base.m_children[this.m_subtotalChildIndex] = new ShimMatrixMember(this, owner, parent, this.m_subtotalChildIndex, isColumnGroup, 0, renderMemberCollection[0], flag2, matrixMemberCellIndexes);
				}
				ShimRenderGroups renderGroups = new ShimRenderGroups(renderMemberCollection, flag && !flag2, flag && flag2);
				ShimMatrixMember shimMatrixMember = (ShimMatrixMember)(base.m_children[this.m_dynamicSubgroupChildIndex] = new ShimMatrixMember(this, owner, parent, this.m_dynamicSubgroupChildIndex, isColumnGroup, this.m_dynamicSubgroupChildIndex, renderGroups, matrixMemberCellIndexes));
				if (flag && flag2)
				{
					base.m_children[this.m_subtotalChildIndex] = new ShimMatrixMember(this, owner, parent, this.m_subtotalChildIndex, isColumnGroup, count - 1, renderMemberCollection[count - 1], flag2, matrixMemberCellIndexes);
				}
				this.m_sizeDelta += shimMatrixMember.SizeDelta;
			}
			this.m_definitionEndIndex = owner.GetCurrentMemberCellDefinitionIndex();
		}

		internal void UpdateContext(MatrixMemberInfoCache matrixMemberCellIndexes)
		{
			if (base.m_children != null)
			{
				if (base.m_isColumnGroup)
				{
					this.ResetContext(base.OwnerTablix.RenderMatrix.ColumnMemberCollection, matrixMemberCellIndexes);
				}
				else
				{
					this.ResetContext(base.OwnerTablix.RenderMatrix.RowMemberCollection);
				}
			}
		}

		internal void ResetContext(MatrixMemberCollection newRenderMemberCollection)
		{
			this.ResetContext(newRenderMemberCollection, null);
		}

		internal void ResetContext(MatrixMemberCollection newRenderMemberCollection, MatrixMemberInfoCache matrixMemberCellIndexes)
		{
			if (base.m_children != null)
			{
				MatrixMember staticOrSubtotal = null;
				int newAfterSubtotalCollectionIndex = -1;
				ShimRenderGroups renderGroups = null;
				if (newRenderMemberCollection != null)
				{
					renderGroups = new ShimRenderGroups(newRenderMemberCollection, 0 == this.m_subtotalChildIndex, 1 == this.m_subtotalChildIndex);
					int count = newRenderMemberCollection.Count;
					if (this.m_subtotalChildIndex == 0)
					{
						staticOrSubtotal = newRenderMemberCollection[0];
					}
					else if (1 == this.m_subtotalChildIndex)
					{
						staticOrSubtotal = newRenderMemberCollection[count - 1];
						newAfterSubtotalCollectionIndex = count - 1;
					}
				}
				if (this.m_dynamicSubgroupChildIndex >= 0)
				{
					((ShimMatrixMember)base.m_children[this.m_dynamicSubgroupChildIndex]).ResetContext(null, -1, renderGroups, matrixMemberCellIndexes);
					if (this.m_subtotalChildIndex >= 0)
					{
						((ShimMatrixMember)base.m_children[this.m_subtotalChildIndex]).ResetContext(staticOrSubtotal, newAfterSubtotalCollectionIndex, null, matrixMemberCellIndexes);
					}
				}
				else
				{
					for (int i = 0; i < base.m_children.Length; i++)
					{
						staticOrSubtotal = ((newRenderMemberCollection != null) ? newRenderMemberCollection[i] : null);
						((ShimMatrixMember)base.m_children[i]).ResetContext(staticOrSubtotal, -1, null, matrixMemberCellIndexes);
					}
				}
			}
		}
	}
}
