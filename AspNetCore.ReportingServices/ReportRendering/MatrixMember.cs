using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class MatrixMember : Group, IDocumentMapEntry
	{
		internal enum SortOrders
		{
			None,
			Ascending,
			Descending
		}

		private MatrixHeading m_headingDef;

		private MatrixHeadingInstance m_headingInstance;

		private MatrixHeadingInstanceInfo m_headingInstanceInfo;

		private ReportItem m_reportItem;

		private MatrixMemberCollection m_children;

		private MatrixMember m_parent;

		private ReportSize m_width;

		private ReportSize m_height;

		private bool m_isSubtotal;

		private bool m_isParentSubTotal;

		private int m_index;

		private int m_cachedMemberCellIndex = -1;

		public override string ID
		{
			get
			{
				if (this.m_isSubtotal)
				{
					if (this.m_headingDef.Subtotal.RenderingModelID == null)
					{
						this.m_headingDef.Subtotal.RenderingModelID = this.m_headingDef.Subtotal.ID.ToString(CultureInfo.InvariantCulture);
					}
					return this.m_headingDef.Subtotal.RenderingModelID;
				}
				if (this.m_headingDef.Grouping == null)
				{
					if (this.m_headingDef.RenderingModelIDs == null)
					{
						this.m_headingDef.RenderingModelIDs = new string[this.m_headingDef.ReportItems.Count];
					}
					if (this.m_headingDef.RenderingModelIDs[this.m_index] == null)
					{
						this.m_headingDef.RenderingModelIDs[this.m_index] = this.m_headingDef.IDs[this.m_index].ToString(CultureInfo.InvariantCulture);
					}
					return this.m_headingDef.RenderingModelIDs[this.m_index];
				}
				if (this.m_headingDef.RenderingModelID == null)
				{
					this.m_headingDef.RenderingModelID = this.m_headingDef.ID.ToString(CultureInfo.InvariantCulture);
				}
				return this.m_headingDef.RenderingModelID;
			}
		}

		public object SharedRenderingInfo
		{
			get
			{
				int num = (!this.m_isSubtotal) ? ((this.m_headingDef.Grouping != null) ? this.m_headingDef.ID : this.m_headingDef.IDs[this.m_index]) : this.m_headingDef.Subtotal.ID;
				return base.OwnerDataRegion.RenderingContext.RenderingInfoManager.SharedRenderingInfo[num];
			}
			set
			{
				int num = (!this.m_isSubtotal) ? ((this.m_headingDef.Grouping != null) ? this.m_headingDef.ID : this.m_headingDef.IDs[this.m_index]) : this.m_headingDef.Subtotal.ID;
				base.OwnerDataRegion.RenderingContext.RenderingInfoManager.SharedRenderingInfo[num] = value;
			}
		}

		internal ReportSize Size
		{
			get
			{
				if (this.m_headingDef.SizeForRendering == null)
				{
					this.m_headingDef.SizeForRendering = new ReportSize(this.m_headingDef.Size, this.m_headingDef.SizeValue);
				}
				return this.m_headingDef.SizeForRendering;
			}
		}

		internal override TextBox ToggleParent
		{
			get
			{
				if (!this.m_isSubtotal && !this.IsStatic)
				{
					if (Visibility.HasToggle(base.m_visibilityDef))
					{
						return base.OwnerDataRegion.RenderingContext.GetToggleParent(base.m_uniqueName);
					}
					return null;
				}
				return null;
			}
		}

		public override bool HasToggle
		{
			get
			{
				if (!this.m_isSubtotal && !this.IsStatic)
				{
					return Visibility.HasToggle(base.m_visibilityDef);
				}
				return false;
			}
		}

		public override string ToggleItem
		{
			get
			{
				if (!this.m_isSubtotal && !this.IsStatic)
				{
					return base.ToggleItem;
				}
				return null;
			}
		}

		public override SharedHiddenState SharedHidden
		{
			get
			{
				if (this.m_isSubtotal)
				{
					if (!this.m_headingDef.Subtotal.AutoDerived)
					{
						return SharedHiddenState.Never;
					}
					return SharedHiddenState.Always;
				}
				if (this.IsStatic)
				{
					return SharedHiddenState.Never;
				}
				return Visibility.GetSharedHidden(base.m_visibilityDef);
			}
		}

		public override bool IsToggleChild
		{
			get
			{
				if (!this.m_isSubtotal && !this.IsStatic)
				{
					return base.OwnerDataRegion.RenderingContext.IsToggleChild(base.m_uniqueName);
				}
				return false;
			}
		}

		public override bool Hidden
		{
			get
			{
				if (this.m_isSubtotal)
				{
					return this.m_headingDef.Subtotal.AutoDerived;
				}
				if (this.m_headingInstance == null)
				{
					return RenderingContext.GetDefinitionHidden(this.m_headingDef.Visibility);
				}
				if (this.m_headingDef.Visibility == null)
				{
					return false;
				}
				if (this.m_headingDef.Visibility.Toggle != null)
				{
					return base.OwnerDataRegion.RenderingContext.IsItemHidden(this.m_headingInstance.UniqueName, false);
				}
				return this.InstanceInfo.StartHidden;
			}
		}

		public override CustomPropertyCollection CustomProperties
		{
			get
			{
				CustomPropertyCollection customPropertyCollection = base.m_customProperties;
				if (base.m_customProperties == null)
				{
					if (this.m_headingDef.Grouping == null || this.m_headingDef.Grouping.CustomProperties == null)
					{
						return null;
					}
					customPropertyCollection = ((this.m_headingInstance != null) ? new CustomPropertyCollection(this.m_headingDef.Grouping.CustomProperties, this.InstanceInfo.CustomPropertyInstances) : new CustomPropertyCollection(this.m_headingDef.Grouping.CustomProperties, null));
					if (base.OwnerDataRegion.RenderingContext.CacheState)
					{
						base.m_customProperties = customPropertyCollection;
					}
				}
				return customPropertyCollection;
			}
		}

		public ReportItem ReportItem
		{
			get
			{
				ReportItem reportItem = this.m_reportItem;
				if (this.m_reportItem == null)
				{
					AspNetCore.ReportingServices.ReportProcessing.ReportItem reportItem2 = null;
					ReportItemInstance reportItemInstance = null;
					NonComputedUniqueNames nonComputedUniqueNames = null;
					reportItem2 = ((!this.m_isSubtotal) ? ((this.m_headingDef.Grouping != null) ? this.m_headingDef.ReportItem : this.m_headingDef.ReportItems[this.m_index]) : this.m_headingDef.Subtotal.ReportItem);
					if (this.m_headingInstance != null)
					{
						nonComputedUniqueNames = this.InstanceInfo.ContentUniqueNames;
						reportItemInstance = this.m_headingInstance.Content;
					}
					if (reportItem2 != null)
					{
						reportItem = ReportItem.CreateItem(0, reportItem2, reportItemInstance, base.OwnerDataRegion.RenderingContext, nonComputedUniqueNames);
					}
					if (base.OwnerDataRegion.RenderingContext.CacheState)
					{
						this.m_reportItem = reportItem;
					}
				}
				return reportItem;
			}
		}

		public override string Label
		{
			get
			{
				string result = null;
				if (base.m_groupingDef != null && base.m_groupingDef.GroupLabel != null)
				{
					result = ((base.m_groupingDef.GroupLabel.Type != ExpressionInfo.Types.Constant) ? ((this.m_headingInstance != null) ? this.InstanceInfo.Label : null) : base.m_groupingDef.GroupLabel.Value);
				}
				return result;
			}
		}

		public bool InDocumentMap
		{
			get
			{
				if (this.m_headingInstance != null && base.m_groupingDef != null && base.m_groupingDef.GroupLabel != null)
				{
					return !this.m_isSubtotal;
				}
				return false;
			}
		}

		public MatrixMember Parent
		{
			get
			{
				return this.m_parent;
			}
		}

		public MatrixMemberCollection Children
		{
			get
			{
				MatrixHeading matrixHeading = this.m_headingDef.SubHeading;
				if (matrixHeading == null)
				{
					return null;
				}
				MatrixMemberCollection matrixMemberCollection = this.m_children;
				MatrixHeadingInstanceList headingInstances;
				if (this.m_children == null)
				{
					headingInstances = null;
					if (this.m_headingInstance != null)
					{
						if (this.m_headingInstance.SubHeadingInstances != null && this.m_headingInstance.SubHeadingInstances.Count != 0)
						{
							headingInstances = this.m_headingInstance.SubHeadingInstances;
							if (this.m_headingInstance.IsSubtotal)
							{
								matrixHeading = (MatrixHeading)this.m_headingDef.GetInnerStaticHeading();
							}
							goto IL_008e;
						}
						return this.m_children;
					}
					if (this.m_isSubtotal)
					{
						return this.m_children;
					}
					goto IL_008e;
				}
				goto IL_00db;
				IL_008e:
				List<int> memberMapping = Matrix.CalculateMapping(matrixHeading, headingInstances, this.m_isSubtotal || this.m_isParentSubTotal);
				matrixMemberCollection = new MatrixMemberCollection((Matrix)base.OwnerDataRegion, this, matrixHeading, headingInstances, memberMapping, this.m_isSubtotal);
				if (base.OwnerDataRegion.RenderingContext.CacheState)
				{
					this.m_children = matrixMemberCollection;
				}
				goto IL_00db;
				IL_00db:
				return matrixMemberCollection;
			}
		}

		public int MemberCellIndex
		{
			get
			{
				if (this.m_headingInstance == null)
				{
					if (this.m_headingDef.Grouping == null)
					{
						return this.m_index;
					}
					return 0;
				}
				return this.InstanceInfo.HeadingCellIndex;
			}
		}

		internal int CachedMemberCellIndex
		{
			get
			{
				if (this.m_cachedMemberCellIndex < 0)
				{
					this.m_cachedMemberCellIndex = this.MemberCellIndex;
				}
				return this.m_cachedMemberCellIndex;
			}
		}

		public int ColumnSpan
		{
			get
			{
				MatrixHeading matrixHeading = null;
				AspNetCore.ReportingServices.ReportProcessing.Matrix matrix = (AspNetCore.ReportingServices.ReportProcessing.Matrix)base.OwnerDataRegion.ReportItemDef;
				if (this.m_headingDef.IsColumn)
				{
					if (this.m_headingInstance != null)
					{
						return this.InstanceInfo.HeadingSpan;
					}
					matrixHeading = (MatrixHeading)this.m_headingDef.GetInnerStaticHeading();
					if (matrixHeading != null)
					{
						return matrix.MatrixColumns.Count;
					}
					goto IL_0070;
				}
				if (!this.m_isSubtotal && !this.m_isParentSubTotal)
				{
					goto IL_0070;
				}
				return this.m_headingDef.SubtotalSpan;
				IL_0070:
				return 1;
			}
		}

		public int RowSpan
		{
			get
			{
				MatrixHeading matrixHeading = null;
				AspNetCore.ReportingServices.ReportProcessing.Matrix matrix = (AspNetCore.ReportingServices.ReportProcessing.Matrix)base.OwnerDataRegion.ReportItemDef;
				if (this.m_headingDef.IsColumn)
				{
					if (!this.m_isSubtotal && !this.m_isParentSubTotal)
					{
						goto IL_0070;
					}
					return this.m_headingDef.SubtotalSpan;
				}
				if (this.m_headingInstance != null)
				{
					return this.InstanceInfo.HeadingSpan;
				}
				matrixHeading = (MatrixHeading)this.m_headingDef.GetInnerStaticHeading();
				if (matrixHeading != null)
				{
					return matrix.MatrixRows.Count;
				}
				goto IL_0070;
				IL_0070:
				return 1;
			}
		}

		public bool IsTotal
		{
			get
			{
				return this.m_isSubtotal;
			}
		}

		public bool IsStatic
		{
			get
			{
				if (this.m_headingDef.Grouping == null)
				{
					return true;
				}
				return false;
			}
		}

		public ReportSize Width
		{
			get
			{
				ReportSize reportSize = this.m_width;
				if (this.m_width == null)
				{
					if (this.m_headingDef.IsColumn)
					{
						double num = 0.0;
						SizeCollection cellWidths = ((Matrix)base.OwnerDataRegion).CellWidths;
						MatrixHeading matrixHeading = (MatrixHeading)this.m_headingDef.GetInnerStaticHeading();
						if (matrixHeading == null)
						{
							num = (double)this.ColumnSpan * cellWidths[this.MemberCellIndex].ToMillimeters();
							num = Math.Round(num, Validator.DecimalPrecision);
						}
						else
						{
							for (int i = 0; i < this.ColumnSpan; i++)
							{
								num += cellWidths[this.MemberCellIndex + i].ToMillimeters();
								num = Math.Round(num, Validator.DecimalPrecision);
							}
						}
						reportSize = new ReportSize(num + "mm", num);
					}
					else
					{
						if ((this.m_isSubtotal || this.m_isParentSubTotal) && 1 != this.m_headingDef.SubtotalSpan)
						{
							double num2 = 0.0;
							MatrixHeading matrixHeading2 = this.m_headingDef;
							for (int j = 0; j < this.m_headingDef.SubtotalSpan; j++)
							{
								num2 += matrixHeading2.SizeValue;
								num2 = Math.Round(num2, Validator.DecimalPrecision);
								matrixHeading2 = matrixHeading2.SubHeading;
							}
							reportSize = new ReportSize(num2 + "mm", num2);
							goto IL_015d;
						}
						reportSize = this.Size;
					}
					goto IL_015d;
				}
				goto IL_0176;
				IL_015d:
				if (base.OwnerDataRegion.RenderingContext.CacheState)
				{
					this.m_width = reportSize;
				}
				goto IL_0176;
				IL_0176:
				return reportSize;
			}
		}

		public ReportSize Height
		{
			get
			{
				ReportSize reportSize = this.m_height;
				if (this.m_height == null)
				{
					if (this.m_headingDef.IsColumn)
					{
						if ((this.m_isSubtotal || this.m_isParentSubTotal) && 1 != this.m_headingDef.SubtotalSpan)
						{
							double num = 0.0;
							MatrixHeading matrixHeading = this.m_headingDef;
							for (int i = 0; i < this.m_headingDef.SubtotalSpan; i++)
							{
								num += matrixHeading.SizeValue;
								num = Math.Round(num, Validator.DecimalPrecision);
								matrixHeading = matrixHeading.SubHeading;
							}
							reportSize = new ReportSize(num + "mm", num);
							goto IL_0160;
						}
						reportSize = this.Size;
					}
					else
					{
						double num2 = 0.0;
						SizeCollection cellHeights = ((Matrix)base.OwnerDataRegion).CellHeights;
						MatrixHeading matrixHeading2 = (MatrixHeading)this.m_headingDef.GetInnerStaticHeading();
						if (matrixHeading2 == null)
						{
							num2 = (double)this.RowSpan * cellHeights[this.MemberCellIndex].ToMillimeters();
							num2 = Math.Round(num2, Validator.DecimalPrecision);
						}
						else
						{
							for (int j = 0; j < this.RowSpan; j++)
							{
								num2 += cellHeights[this.MemberCellIndex + j].ToMillimeters();
								num2 = Math.Round(num2, Validator.DecimalPrecision);
							}
						}
						reportSize = new ReportSize(num2 + "mm", num2);
					}
					goto IL_0160;
				}
				goto IL_0179;
				IL_0179:
				return reportSize;
				IL_0160:
				if (base.OwnerDataRegion.RenderingContext.CacheState)
				{
					this.m_height = reportSize;
				}
				goto IL_0179;
			}
		}

		public object GroupValue
		{
			get
			{
				if (!this.m_isSubtotal && this.m_headingDef.OwcGroupExpression)
				{
					return this.InstanceInfo.GroupExpressionValue;
				}
				return null;
			}
		}

		public SortOrders SortOrder
		{
			get
			{
				SortOrders result = SortOrders.None;
				if (!this.IsStatic)
				{
					BoolList boolList = (this.m_headingDef.Sorting == null) ? this.m_headingDef.Grouping.SortDirections : this.m_headingDef.Sorting.SortDirections;
					if (boolList != null && 0 < boolList.Count)
					{
						result = (SortOrders)(boolList[0] ? 1 : 2);
					}
				}
				return result;
			}
		}

		public override string DataElementName
		{
			get
			{
				if (this.IsTotal)
				{
					return this.m_headingDef.Subtotal.DataElementName;
				}
				if (this.IsStatic)
				{
					return this.m_headingDef.ReportItems[this.m_index].DataElementName;
				}
				return base.DataElementName;
			}
		}

		public override DataElementOutputTypes DataElementOutput
		{
			get
			{
				Global.Tracer.Assert(!this.IsTotal || !this.IsStatic);
				if (this.IsTotal)
				{
					return this.m_headingDef.Subtotal.DataElementOutput;
				}
				if (this.IsStatic)
				{
					return this.DataElementOutputForStatic(null);
				}
				return base.DataElementOutput;
			}
		}

		internal MatrixHeadingInstanceInfo InstanceInfo
		{
			get
			{
				if (this.m_headingInstance == null)
				{
					return null;
				}
				if (this.m_headingInstanceInfo == null)
				{
					this.m_headingInstanceInfo = this.m_headingInstance.GetInstanceInfo(base.OwnerDataRegion.RenderingContext.ChunkManager);
				}
				return this.m_headingInstanceInfo;
			}
		}

		internal bool IsParentSubtotal
		{
			get
			{
				return this.m_isParentSubTotal;
			}
		}

		internal MatrixMember(Matrix owner, MatrixMember parent, MatrixHeading headingDef, MatrixHeadingInstance headingInstance, bool isSubtotal, bool isParentSubTotal, int index)
			: base(owner, headingDef.Grouping, headingDef.Visibility)
		{
			this.m_parent = parent;
			this.m_headingDef = headingDef;
			this.m_headingInstance = headingInstance;
			this.m_isSubtotal = isSubtotal;
			this.m_isParentSubTotal = isParentSubTotal;
			this.m_index = index;
			if (this.m_headingInstance != null)
			{
				base.m_uniqueName = this.m_headingInstance.UniqueName;
			}
		}

		public DataElementOutputTypes DataElementOutputForStatic(MatrixMember staticHeading)
		{
			if (!this.IsStatic)
			{
				return this.DataElementOutput;
			}
			if (staticHeading != null && (!staticHeading.IsStatic || staticHeading.Parent == this.Parent))
			{
				staticHeading = null;
			}
			if (staticHeading != null)
			{
				int index;
				int index2;
				if (this.m_headingDef.IsColumn)
				{
					index = staticHeading.m_index;
					index2 = this.m_index;
				}
				else
				{
					index = this.m_index;
					index2 = staticHeading.m_index;
				}
				return this.GetDataElementOutputTypeFromCell(index, index2);
			}
			AspNetCore.ReportingServices.ReportProcessing.Matrix matrix = (AspNetCore.ReportingServices.ReportProcessing.Matrix)base.OwnerDataRegion.ReportItemDef;
			if (matrix.PivotStaticColumns != null && matrix.PivotStaticRows != null)
			{
				Global.Tracer.Assert(matrix.PivotStaticColumns != null && matrix.PivotStaticRows != null);
				return this.GetDataElementOutputTypeForRowCol(this.m_index);
			}
			return this.GetDataElementOutputTypeFromCell(0, this.m_index);
		}

		public bool IsRowMemberOnThisPage(int memberIndex, int pageNumber, out int startPage, out int endPage)
		{
			startPage = -1;
			endPage = -1;
			RenderingPagesRangesList childrenStartAndEndPages = this.m_headingInstance.ChildrenStartAndEndPages;
			if (childrenStartAndEndPages == null)
			{
				return true;
			}
			Global.Tracer.Assert(memberIndex >= 0 && memberIndex < childrenStartAndEndPages.Count);
			if (memberIndex >= childrenStartAndEndPages.Count)
			{
				return false;
			}
			RenderingPagesRanges renderingPagesRanges = childrenStartAndEndPages[memberIndex];
			startPage = renderingPagesRanges.StartPage;
			endPage = renderingPagesRanges.EndPage;
			if (pageNumber >= startPage)
			{
				return pageNumber <= endPage;
			}
			return false;
		}

		private DataElementOutputTypes GetDataElementOutputTypeFromCell(int rowIndex, int columnIndex)
		{
			AspNetCore.ReportingServices.ReportProcessing.Matrix matrix = (AspNetCore.ReportingServices.ReportProcessing.Matrix)base.OwnerDataRegion.ReportItemDef;
			AspNetCore.ReportingServices.ReportProcessing.ReportItem cellReportItem = matrix.GetCellReportItem(rowIndex, columnIndex);
			return cellReportItem.DataElementOutput;
		}

		private DataElementOutputTypes GetDataElementOutputTypeForRowCol(int index)
		{
			AspNetCore.ReportingServices.ReportProcessing.Matrix matrix = (AspNetCore.ReportingServices.ReportProcessing.Matrix)base.OwnerDataRegion.ReportItemDef;
			int num;
			int num2;
			int count;
			if (this.m_headingDef.IsColumn)
			{
				num = 0;
				num2 = index;
				count = matrix.MatrixRows.Count;
			}
			else
			{
				num = index;
				num2 = 0;
				count = matrix.MatrixColumns.Count;
			}
			while (true)
			{
				AspNetCore.ReportingServices.ReportProcessing.ReportItem cellReportItem = matrix.GetCellReportItem(num, num2);
				if (cellReportItem.DataElementOutput != DataElementOutputTypes.NoOutput)
				{
					return DataElementOutputTypes.Output;
				}
				if (this.m_headingDef.IsColumn)
				{
					num++;
					if (num >= count)
					{
						break;
					}
				}
				else
				{
					num2++;
					if (num2 >= count)
					{
						break;
					}
				}
			}
			return DataElementOutputTypes.NoOutput;
		}

		public void GetChildRowMembersOnPage(int page, out int startChild, out int endChild)
		{
			startChild = -1;
			endChild = -1;
			if (this.m_headingInstance != null)
			{
				RenderingPagesRangesList childrenStartAndEndPages = this.m_headingInstance.ChildrenStartAndEndPages;
				if (childrenStartAndEndPages != null)
				{
					RenderingContext.FindRange(childrenStartAndEndPages, 0, childrenStartAndEndPages.Count - 1, page, ref startChild, ref endChild);
				}
			}
		}
	}
}
