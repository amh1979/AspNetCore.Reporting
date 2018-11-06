using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using AspNetCore.ReportingServices.Rendering.Utilities;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace AspNetCore.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class Tablix : PageItem, IStorable, IPersistable
	{
		internal enum TablixRegion : byte
		{
			Unknown,
			Corner,
			ColumnHeader,
			RowHeader,
			Data
		}

		internal class CreateItemsContext
		{
			[Flags]
			private enum CreateItemState : byte
			{
				AdvanceRow = 1,
				PartialDetailRow = 2,
				GroupPageBreaks = 4,
				ContentFullyCreated = 8
			}

			private double m_spaceToFill;

			private double m_detailRowsHeight;

			private CreateItemState m_state = CreateItemState.AdvanceRow;

			internal double DetailRowsHeight
			{
				get
				{
					return this.m_detailRowsHeight;
				}
			}

			internal bool PartialDetailRow
			{
				get
				{
					return (int)(this.m_state & CreateItemState.PartialDetailRow) > 0;
				}
				set
				{
					if (value)
					{
						this.m_state |= CreateItemState.PartialDetailRow;
					}
					else
					{
						this.m_state &= ~CreateItemState.PartialDetailRow;
					}
				}
			}

			internal bool ContentFullyCreated
			{
				get
				{
					return (int)(this.m_state & CreateItemState.ContentFullyCreated) > 0;
				}
				set
				{
					if (value)
					{
						this.m_state |= CreateItemState.ContentFullyCreated;
					}
					else
					{
						this.m_state &= ~CreateItemState.ContentFullyCreated;
					}
				}
			}

			internal bool GroupPageBreaks
			{
				get
				{
					return (int)(this.m_state & CreateItemState.GroupPageBreaks) > 0;
				}
				set
				{
					if (value)
					{
						this.m_state |= CreateItemState.GroupPageBreaks;
					}
					else
					{
						this.m_state &= ~CreateItemState.GroupPageBreaks;
					}
				}
			}

			internal bool AdvanceRow
			{
				get
				{
					if (!this.PartialDetailRow && !this.GroupPageBreaks)
					{
						return (int)(this.m_state & CreateItemState.AdvanceRow) > 0;
					}
					return false;
				}
				set
				{
					if (value)
					{
						this.m_state |= CreateItemState.AdvanceRow;
					}
					else
					{
						this.m_state &= ~CreateItemState.AdvanceRow;
					}
				}
			}

			internal bool InnerSpanPages
			{
				get
				{
					if (!this.PartialDetailRow && !this.GroupPageBreaks)
					{
						return false;
					}
					return true;
				}
			}

			internal double SpaceToFill
			{
				get
				{
					return this.m_spaceToFill - this.m_detailRowsHeight;
				}
			}

			internal CreateItemsContext(double spaceToFill)
			{
				this.m_spaceToFill = spaceToFill;
				this.ContentFullyCreated = true;
			}

			internal void UpdateInfo(CreateItemsContext childCreateItems)
			{
				this.m_detailRowsHeight += childCreateItems.DetailRowsHeight;
				if (!childCreateItems.AdvanceRow)
				{
					this.AdvanceRow = false;
				}
				if (childCreateItems.PartialDetailRow)
				{
					this.PartialDetailRow = true;
				}
				if (!childCreateItems.ContentFullyCreated)
				{
					this.ContentFullyCreated = false;
				}
				if (childCreateItems.GroupPageBreaks)
				{
					this.GroupPageBreaks = true;
				}
			}

			internal void UpdateInfo(RowInfo rowInfo, double delta)
			{
				this.m_detailRowsHeight += Math.Max(0.0, rowInfo.NormalizeRowHeight + delta);
				if (rowInfo.SpanPagesRow)
				{
					this.PartialDetailRow = true;
				}
				if (!rowInfo.ContentFullyCreated)
				{
					this.ContentFullyCreated = false;
					this.PartialDetailRow = true;
				}
				if (this.m_spaceToFill - this.m_detailRowsHeight <= 0.01)
				{
					this.AdvanceRow = false;
				}
			}
		}

		internal class MergeDetailRows
		{
			[Flags]
			internal enum DetailRowState : byte
			{
				Merge = 1,
				Insert = 2,
				Skip = 4
			}

			private int m_destDetailRows;

			private List<DetailRowState> m_detailRowState;

			internal int Span
			{
				get
				{
					return this.m_destDetailRows;
				}
			}

			internal List<DetailRowState> DetailRowsState
			{
				get
				{
					return this.m_detailRowState;
				}
			}

			internal MergeDetailRows(int destDetailRows)
			{
				this.m_destDetailRows = destDetailRows;
				this.m_detailRowState = new List<DetailRowState>(destDetailRows);
			}

			internal void AddRowState(DetailRowState state)
			{
				this.m_detailRowState.Add(state);
			}
		}

		internal class LevelInfo
		{
			[Flags]
			private enum LevelInfoState : byte
			{
				PartialLevel = 1,
				HiddenLevel = 2,
				IgnoreTotals = 4
			}

			private int m_spanForParent;

			private double m_sizeForParent;

			private List<PageStructMemberCell> m_memberCells;

			private int m_partialItemIndex = -1;

			private double m_sourceSize;

			private int m_sourceIndex = -1;

			private LevelInfoState m_state = LevelInfoState.HiddenLevel;

			private CreateItemsContext m_createItemsContext;

			internal int SpanForParent
			{
				get
				{
					return this.m_spanForParent;
				}
				set
				{
					this.m_spanForParent = value;
				}
			}

			internal double SizeForParent
			{
				get
				{
					return this.m_sizeForParent;
				}
				set
				{
					this.m_sizeForParent = value;
				}
			}

			internal double SourceSize
			{
				get
				{
					return this.m_sourceSize;
				}
				set
				{
					this.m_sourceSize = value;
				}
			}

			internal List<PageStructMemberCell> MemberCells
			{
				get
				{
					return this.m_memberCells;
				}
				set
				{
					this.m_memberCells = value;
				}
			}

			internal bool OmittedList
			{
				get
				{
					if (this.m_memberCells == null)
					{
						return true;
					}
					for (int i = 0; i < this.m_memberCells.Count; i++)
					{
						if (!this.m_memberCells[i].OmittedList)
						{
							return false;
						}
					}
					return true;
				}
			}

			internal bool HiddenLevel
			{
				get
				{
					if (this.m_memberCells == null)
					{
						return false;
					}
					return (int)(this.m_state & LevelInfoState.HiddenLevel) > 0;
				}
			}

			internal bool PartialLevel
			{
				get
				{
					return (int)(this.m_state & LevelInfoState.PartialLevel) > 0;
				}
			}

			internal bool IgnoreTotals
			{
				get
				{
					return (int)(this.m_state & LevelInfoState.IgnoreTotals) > 0;
				}
				set
				{
					if (value)
					{
						this.m_state |= LevelInfoState.IgnoreTotals;
					}
					else
					{
						this.m_state &= ~LevelInfoState.IgnoreTotals;
					}
				}
			}

			internal bool PartialStruct
			{
				get
				{
					if (this.PartialLevel)
					{
						return true;
					}
					if (!this.m_createItemsContext.ContentFullyCreated)
					{
						return true;
					}
					return false;
				}
			}

			internal CreateItemsContext CreateItems
			{
				get
				{
					return this.m_createItemsContext;
				}
			}

			internal LevelInfo(double spaceToFill)
			{
				this.m_createItemsContext = new CreateItemsContext(spaceToFill);
			}

			internal LevelInfo(List<PageStructMemberCell> memberCells, int partialItemIndex, int sourceIndex, CreateItemsContext createItems)
			{
				this.m_memberCells = memberCells;
				this.m_sourceIndex = sourceIndex;
				this.m_partialItemIndex = partialItemIndex;
				if (this.m_partialItemIndex >= 0)
				{
					this.m_state |= LevelInfoState.PartialLevel;
				}
				this.m_createItemsContext = createItems;
			}

			internal MergeDetailRows AddMemberCell(TablixMember member, int sourceIndex, PageMemberCell memberCell, int defMemberInfo, TablixRegion region, double[] sourceSizes, LevelInfo childLevelInfo, PageContext pageContext, Tablix tablix)
			{
				PageStructMemberCell pageStructMemberCell = null;
				if (sourceIndex > this.m_sourceIndex)
				{
					this.m_sourceIndex = sourceIndex;
					if (member.IsStatic)
					{
						pageStructMemberCell = new PageStructStaticMemberCell(sourceIndex, childLevelInfo.PartialStruct, false, member, defMemberInfo);
						if (childLevelInfo.PartialLevel)
						{
							this.m_state |= LevelInfoState.PartialLevel;
						}
					}
					else if (childLevelInfo.PartialStruct)
					{
						pageStructMemberCell = new PageStructDynamicMemberCell(sourceIndex, true, false, member, defMemberInfo);
						this.m_state |= LevelInfoState.PartialLevel;
					}
					else
					{
						pageStructMemberCell = new PageStructDynamicMemberCell(sourceIndex, true, true, member, defMemberInfo);
						this.m_state |= LevelInfoState.PartialLevel;
					}
					if (this.m_memberCells == null)
					{
						this.m_memberCells = new List<PageStructMemberCell>();
					}
					this.m_memberCells.Add(pageStructMemberCell);
					if (!memberCell.Hidden)
					{
						this.m_sourceSize += sourceSizes[member.MemberCellIndex];
					}
				}
				else if (this.m_partialItemIndex >= 0)
				{
					pageStructMemberCell = this.m_memberCells[this.m_partialItemIndex];
					if (childLevelInfo.PartialStruct)
					{
						pageStructMemberCell.CreateItem = false;
					}
				}
				else
				{
					pageStructMemberCell = this.m_memberCells[this.m_memberCells.Count - 1];
					if (member.IsStatic)
					{
						pageStructMemberCell.CreateItem = false;
						pageStructMemberCell.PartialItem = childLevelInfo.PartialStruct;
						if (childLevelInfo.PartialLevel)
						{
							this.m_state |= LevelInfoState.PartialLevel;
						}
					}
					else
					{
						pageStructMemberCell.PartialItem = true;
						this.m_state |= LevelInfoState.PartialLevel;
						if (childLevelInfo.PartialStruct)
						{
							pageStructMemberCell.CreateItem = false;
						}
						else
						{
							pageStructMemberCell.CreateItem = true;
						}
					}
				}
				if (!memberCell.Hidden)
				{
					this.m_state &= ~LevelInfoState.HiddenLevel;
				}
				if (!childLevelInfo.CreateItems.AdvanceRow)
				{
					if (memberCell.KeepTogether)
					{
						tablix.TraceInvalidatedKeepTogetherMember(pageContext, pageStructMemberCell);
					}
					memberCell.KeepTogether = false;
					if (childLevelInfo.CreateItems.InnerSpanPages)
					{
						memberCell.SpanPages = true;
						pageStructMemberCell.SpanPages = true;
					}
				}
				else
				{
					memberCell.KeepTogether = member.KeepTogether;
				}
				MergeDetailRows result = pageStructMemberCell.AddPageMemberCell(memberCell, region, pageContext);
				this.m_createItemsContext.UpdateInfo(childLevelInfo.CreateItems);
				return result;
			}

			internal void AddPartialStructMemberCell(TablixMember member, int sourceIndex, int defMemberIndex, bool createItem)
			{
				this.m_sourceIndex = sourceIndex;
				PageStructMemberCell pageStructMemberCell = null;
				pageStructMemberCell = ((!member.IsStatic) ? ((PageStructMemberCell)new PageStructDynamicMemberCell(sourceIndex, true, createItem, member, defMemberIndex)) : ((PageStructMemberCell)new PageStructStaticMemberCell(sourceIndex, true, createItem, member, defMemberIndex)));
				if (this.m_memberCells == null)
				{
					this.m_memberCells = new List<PageStructMemberCell>();
				}
				this.m_memberCells.Add(pageStructMemberCell);
				this.m_state |= LevelInfoState.PartialLevel;
			}

			internal void FinishPartialStructMember()
			{
				if (this.PartialLevel)
				{
					PageStructMemberCell pageStructMemberCell = null;
					this.m_state &= ~LevelInfoState.PartialLevel;
					if (this.m_partialItemIndex >= 0)
					{
						pageStructMemberCell = this.m_memberCells[this.m_partialItemIndex];
						this.m_partialItemIndex = -1;
					}
					else
					{
						pageStructMemberCell = this.m_memberCells[this.m_memberCells.Count - 1];
					}
					pageStructMemberCell.PartialItem = false;
				}
			}

			internal void SetKeepWith(int count, bool keepWith, bool repeatWith)
			{
				PageStructStaticMemberCell pageStructStaticMemberCell = null;
				int num = this.m_memberCells.Count - 1;
				while (count > 0)
				{
					pageStructStaticMemberCell = (this.m_memberCells[num] as PageStructStaticMemberCell);
					pageStructStaticMemberCell.SetKeepWith(keepWith, repeatWith);
					num--;
					count--;
				}
			}
		}

		internal class HeaderFooterRow
		{
			[Flags]
			private enum HeaderFooterRowState : byte
			{
				OnPage = 1,
				OnPageRepeat = 2,
				RepeatWith = 4,
				KeepWith = 8
			}

			private int m_rowIndex;

			private int m_rowSpan;

			private double m_height;

			private HeaderFooterRowState m_state;

			private PageStructMemberCell m_structMemberCell;

			internal double Height
			{
				get
				{
					return this.m_height;
				}
			}

			internal bool RepeatWith
			{
				get
				{
					return (int)(this.m_state & HeaderFooterRowState.RepeatWith) > 0;
				}
				set
				{
					if (value)
					{
						this.m_state |= HeaderFooterRowState.RepeatWith;
					}
					else
					{
						this.m_state &= ~HeaderFooterRowState.RepeatWith;
					}
				}
			}

			internal bool KeepWith
			{
				get
				{
					return (int)(this.m_state & HeaderFooterRowState.KeepWith) > 0;
				}
				set
				{
					if (value)
					{
						this.m_state |= HeaderFooterRowState.KeepWith;
					}
					else
					{
						this.m_state &= ~HeaderFooterRowState.KeepWith;
					}
				}
			}

			internal bool OnPage
			{
				get
				{
					return (int)(this.m_state & HeaderFooterRowState.OnPage) > 0;
				}
				set
				{
					if (value)
					{
						this.m_state |= HeaderFooterRowState.OnPage;
					}
					else
					{
						this.m_state &= ~HeaderFooterRowState.OnPage;
					}
				}
			}

			internal bool OnPageRepeat
			{
				get
				{
					return (int)(this.m_state & HeaderFooterRowState.OnPageRepeat) > 0;
				}
				set
				{
					if (value)
					{
						this.m_state |= HeaderFooterRowState.OnPageRepeat;
					}
					else
					{
						this.m_state &= ~HeaderFooterRowState.OnPageRepeat;
					}
				}
			}

			internal int RowIndex
			{
				get
				{
					return this.m_rowIndex;
				}
			}

			internal int RowSpan
			{
				get
				{
					return this.m_rowSpan;
				}
			}

			internal PageStructMemberCell StructMemberCell
			{
				get
				{
					return this.m_structMemberCell;
				}
			}

			internal HeaderFooterRow(int rowIndex, int rowSpan, double height, bool onPage, bool repeatWith, bool keepWith, PageStructMemberCell structMemberCell)
			{
				this.m_rowIndex = rowIndex;
				this.m_rowSpan = rowSpan;
				this.m_height = height;
				this.OnPage = onPage;
				this.RepeatWith = repeatWith;
				this.KeepWith = keepWith;
				this.m_structMemberCell = structMemberCell;
			}

			internal HeaderFooterRow(HeaderFooterRow copy)
			{
				this.m_rowIndex = copy.RowIndex;
				this.m_rowSpan = copy.RowSpan;
				this.m_height = copy.Height;
				this.OnPage = copy.OnPage;
				this.OnPageRepeat = copy.OnPageRepeat;
				this.RepeatWith = copy.RepeatWith;
				this.KeepWith = copy.KeepWith;
				this.m_structMemberCell = copy.StructMemberCell;
			}

			internal int BringDetailRowOnPage(ScalableList<RowInfo> detailRows)
			{
				if (this.OnPage && this.OnPageRepeat)
				{
					RowInfo rowInfo = null;
					for (int i = 0; i < this.m_rowSpan; i++)
					{
						using (detailRows.GetAndPin(this.m_rowIndex + i, out rowInfo))
						{
							rowInfo.PageVerticalState = RowInfo.VerticalState.Repeat;
						}
					}
					return 1;
				}
				return 0;
			}
		}

		internal class HeaderFooterRows
		{
			private double m_totalHeight;

			private List<HeaderFooterRow> m_headerFooterRows;

			internal double Height
			{
				get
				{
					return this.m_totalHeight;
				}
			}

			internal int Count
			{
				get
				{
					if (this.m_headerFooterRows == null)
					{
						return 0;
					}
					return this.m_headerFooterRows.Count;
				}
			}

			internal List<HeaderFooterRow> HFRows
			{
				get
				{
					return this.m_headerFooterRows;
				}
			}

			internal int CountNotOnPage
			{
				get
				{
					if (this.m_headerFooterRows == null)
					{
						return 0;
					}
					int num = 0;
					HeaderFooterRow headerFooterRow = null;
					for (int num2 = this.m_headerFooterRows.Count - 1; num2 >= 0; num2--)
					{
						headerFooterRow = this.m_headerFooterRows[num2];
						if (headerFooterRow.OnPage)
						{
							break;
						}
						num++;
					}
					return num;
				}
			}

			internal double RepeatHeight
			{
				get
				{
					if (this.m_headerFooterRows == null)
					{
						return 0.0;
					}
					double num = 0.0;
					HeaderFooterRow headerFooterRow = null;
					for (int num2 = this.m_headerFooterRows.Count - 1; num2 >= 0; num2--)
					{
						headerFooterRow = this.m_headerFooterRows[num2];
						if (headerFooterRow.RepeatWith)
						{
							num += headerFooterRow.Height;
						}
					}
					return num;
				}
			}

			internal double RepeatNotOnPageHeight
			{
				get
				{
					if (this.m_headerFooterRows == null)
					{
						return 0.0;
					}
					double num = 0.0;
					HeaderFooterRow headerFooterRow = null;
					for (int num2 = this.m_headerFooterRows.Count - 1; num2 >= 0; num2--)
					{
						headerFooterRow = this.m_headerFooterRows[num2];
						if (!headerFooterRow.OnPage && headerFooterRow.RepeatWith)
						{
							num += headerFooterRow.Height;
						}
					}
					return num;
				}
			}

			internal double KeepWithAndRepeatHeight
			{
				get
				{
					if (this.m_headerFooterRows == null)
					{
						return 0.0;
					}
					double num = 0.0;
					HeaderFooterRow headerFooterRow = null;
					bool flag = true;
					for (int num2 = this.m_headerFooterRows.Count - 1; num2 >= 0; num2--)
					{
						headerFooterRow = this.m_headerFooterRows[num2];
						if (flag)
						{
							if (headerFooterRow.KeepWith)
							{
								num += headerFooterRow.Height;
							}
							else
							{
								flag = false;
							}
						}
						if (!flag && headerFooterRow.RepeatWith)
						{
							num += headerFooterRow.Height;
						}
					}
					return num;
				}
			}

			internal double KeepWithNoRepeatNoPageHeight
			{
				get
				{
					if (this.m_headerFooterRows == null)
					{
						return 0.0;
					}
					double num = 0.0;
					HeaderFooterRow headerFooterRow = null;
					for (int num2 = this.m_headerFooterRows.Count - 1; num2 >= 0; num2--)
					{
						headerFooterRow = this.m_headerFooterRows[num2];
						if (!headerFooterRow.KeepWith)
						{
							break;
						}
						if (!headerFooterRow.RepeatWith && !headerFooterRow.OnPage)
						{
							num += headerFooterRow.Height;
						}
					}
					return num;
				}
			}

			internal HeaderFooterRows()
			{
			}

			internal HeaderFooterRows(HeaderFooterRows copy)
			{
				if (copy != null)
				{
					this.m_totalHeight = copy.Height;
					if (copy.Count > 0)
					{
						this.m_headerFooterRows = new List<HeaderFooterRow>(copy.Count);
						for (int i = 0; i < copy.Count; i++)
						{
							this.m_headerFooterRows.Add(new HeaderFooterRow(copy.HFRows[i]));
						}
					}
				}
			}

			internal HeaderFooterRows(int count, HeaderFooterRows prevRows, HeaderFooterRows levelRows)
			{
				this.m_headerFooterRows = new List<HeaderFooterRow>(count);
				this.Add(prevRows);
				this.Add(levelRows);
			}

			internal void AddClone(HeaderFooterRows rows)
			{
				if (rows != null)
				{
					this.m_totalHeight += rows.Height;
					if (rows.Count > 0)
					{
						if (this.m_headerFooterRows == null)
						{
							this.m_headerFooterRows = new List<HeaderFooterRow>(rows.Count);
						}
						for (int i = 0; i < rows.Count; i++)
						{
							this.m_headerFooterRows.Add(new HeaderFooterRow(rows.HFRows[i]));
						}
					}
				}
			}

			internal void Add(HeaderFooterRows rows)
			{
				if (rows != null)
				{
					this.m_totalHeight += rows.Height;
					if (rows.Count > 0)
					{
						if (this.m_headerFooterRows == null)
						{
							this.m_headerFooterRows = new List<HeaderFooterRow>(rows.Count);
						}
						for (int i = 0; i < rows.Count; i++)
						{
							this.m_headerFooterRows.Add(rows.HFRows[i]);
						}
					}
				}
			}

			internal void SetRowsKeepWith(bool keepWith)
			{
				if (this.m_headerFooterRows != null)
				{
					for (int i = 0; i < this.m_headerFooterRows.Count; i++)
					{
						this.m_headerFooterRows[i].KeepWith = keepWith;
					}
				}
			}

			internal void SetRowsOnPage(bool onPage)
			{
				if (this.m_headerFooterRows != null)
				{
					for (int i = 0; i < this.m_headerFooterRows.Count; i++)
					{
						this.m_headerFooterRows[i].OnPage = onPage;
					}
				}
			}

			internal void AddHeaderRow(int rowIndex, int rowSpan, double height, bool onPage, bool repeatWith, bool keepWith, PageStructMemberCell structMemberCell)
			{
				if (this.m_headerFooterRows == null)
				{
					this.m_headerFooterRows = new List<HeaderFooterRow>();
				}
				this.m_headerFooterRows.Add(new HeaderFooterRow(rowIndex, rowSpan, height, onPage, repeatWith, keepWith, structMemberCell));
				this.m_totalHeight += height;
			}

			internal void AddFooterRow(int rowIndex, int rowSpan, double height, bool onPage, bool repeatWith, bool keepWith, PageStructMemberCell structMemberCell)
			{
				if (this.m_headerFooterRows == null)
				{
					this.m_headerFooterRows = new List<HeaderFooterRow>();
				}
				this.m_headerFooterRows.Insert(0, new HeaderFooterRow(rowIndex, rowSpan, height, onPage, repeatWith, keepWith, structMemberCell));
				this.m_totalHeight += height;
			}

			internal int AddKeepWithAndRepeat(ref double delta, ref double addHeight, PageContext pageContext, Tablix tablix)
			{
				if (this.m_headerFooterRows == null)
				{
					return 0;
				}
				int num = 0;
				HeaderFooterRow headerFooterRow = null;
				bool flag = true;
				for (int num2 = this.m_headerFooterRows.Count - 1; num2 >= 0; num2--)
				{
					headerFooterRow = this.m_headerFooterRows[num2];
					if (!headerFooterRow.OnPage)
					{
						if (flag)
						{
							if (!headerFooterRow.KeepWith)
							{
								flag = false;
								if (headerFooterRow.RepeatWith)
								{
									goto IL_005a;
								}
								continue;
							}
							goto IL_005a;
						}
						if (headerFooterRow.RepeatWith)
						{
							goto IL_005a;
						}
					}
					continue;
					IL_005a:
					if (headerFooterRow.Height <= delta)
					{
						delta -= headerFooterRow.Height;
						addHeight += headerFooterRow.Height;
						num++;
						headerFooterRow.OnPage = true;
						if (!flag)
						{
							headerFooterRow.OnPageRepeat = true;
							tablix.TraceRepeatOnNewPage(pageContext, headerFooterRow);
						}
						continue;
					}
					delta = 0.0;
					if (pageContext.Common.DiagnosticsEnabled && !flag)
					{
						tablix.TraceInvalidatedRepeatOnNewPageSize(pageContext, headerFooterRow);
						continue;
					}
					return num;
				}
				return num;
			}

			internal int AddToPageRepeat(ref double delta, ref double addHeight, PageContext pageContext, Tablix tablix)
			{
				if (this.m_headerFooterRows == null)
				{
					return 0;
				}
				int num = 0;
				HeaderFooterRow headerFooterRow = null;
				for (int num2 = this.m_headerFooterRows.Count - 1; num2 >= 0; num2--)
				{
					headerFooterRow = this.m_headerFooterRows[num2];
					if (headerFooterRow.RepeatWith && !headerFooterRow.OnPage)
					{
						if (!(headerFooterRow.Height <= delta))
						{
							delta = 0.0;
							if (pageContext.Common.DiagnosticsEnabled)
							{
								tablix.TraceInvalidatedRepeatOnNewPageSize(pageContext, headerFooterRow);
								continue;
							}
							return num;
						}
						delta -= headerFooterRow.Height;
						addHeight += headerFooterRow.Height;
						num++;
						headerFooterRow.OnPage = true;
						headerFooterRow.OnPageRepeat = true;
						tablix.TraceRepeatOnNewPage(pageContext, headerFooterRow);
					}
				}
				return num;
			}

			internal void CheckInvalidatedPageRepeat(PageContext pageContext, Tablix tablix)
			{
				if (this.m_headerFooterRows != null)
				{
					HeaderFooterRow headerFooterRow = null;
					for (int num = this.m_headerFooterRows.Count - 1; num >= 0; num--)
					{
						headerFooterRow = this.m_headerFooterRows[num];
						if (headerFooterRow.RepeatWith && !headerFooterRow.OnPage)
						{
							tablix.TraceInvalidatedRepeatOnNewPageSplitMember(pageContext, headerFooterRow);
						}
					}
				}
			}

			internal int MoveToNextPage(ref double delta)
			{
				if (this.m_headerFooterRows == null)
				{
					return 0;
				}
				int num = 0;
				HeaderFooterRow headerFooterRow = null;
				int num2 = this.m_headerFooterRows.Count - 1;
				while (num2 >= 0)
				{
					headerFooterRow = this.m_headerFooterRows[num2];
					if (!headerFooterRow.OnPage)
					{
						break;
					}
					if (headerFooterRow.OnPageRepeat)
					{
						break;
					}
					if (headerFooterRow.Height <= delta)
					{
						delta -= headerFooterRow.Height;
						num++;
						headerFooterRow.OnPage = false;
						headerFooterRow.OnPageRepeat = false;
						num2--;
						continue;
					}
					delta = 0.0;
					return num;
				}
				return num;
			}

			internal int BringDetailRowOnPage(ScalableList<RowInfo> detailRows)
			{
				if (this.m_headerFooterRows == null)
				{
					return 0;
				}
				int num = 0;
				for (int i = 0; i < this.m_headerFooterRows.Count; i++)
				{
					num += this.m_headerFooterRows[i].BringDetailRowOnPage(detailRows);
				}
				return num;
			}
		}

		private class ColumnSpan
		{
			public int Start;

			public int Span;

			public double SpanSize;

			public ColumnSpan(int start, int span, double spanSize)
			{
				this.Start = start;
				this.Span = span;
				this.SpanSize = spanSize;
			}

			public int CalculateEmptyColumnns(ScalableList<ColumnInfo> columnInfoList)
			{
				int num = 0;
				for (int i = this.Start; i < this.Start + this.Span; i++)
				{
					ColumnInfo columnInfo = columnInfoList[i];
					if (columnInfo == null || columnInfo.Empty)
					{
						num++;
					}
				}
				return num;
			}
		}

		[Flags]
		private enum TablixState : short
		{
			NoRows = 1,
			IsLTR = 2,
			RepeatColumnHeaders = 4,
			RepeatedColumnHeaders = 8,
			AddToPageColumnHeaders = 0x10,
			SplitColumnHeaders = 0x20,
			RepeatRowHeaders = 0x40,
			AddToPageRowHeaders = 0x80,
			SplitRowHeaders = 0x100,
			ColumnHeadersCreated = 0x200,
			RowHeadersCreated = 0x400
		}

		internal class PageMemberCell : IStorable, IPersistable
		{
			[Flags]
			private enum MemberState : byte
			{
				Clear = 0,
				Hidden = 1,
				SpanPages = 2,
				ContentOnPage = 4,
				IgnoreTotals = 8,
				KeepTogether = 0x10
			}

			private PageItem m_memberItem;

			private List<PageStructMemberCell> m_children;

			internal double m_sourceWidth;

			internal int m_rowSpan;

			internal int m_colSpan;

			internal int m_currRowSpan;

			internal int m_currColSpan;

			internal double m_startPos;

			internal double m_size;

			internal string m_label;

			internal string m_uniqueName;

			private MemberState m_memberState;

			private string m_pageName;

			private static Declaration m_declaration = PageMemberCell.GetDeclaration();

			internal List<PageStructMemberCell> Children
			{
				get
				{
					return this.m_children;
				}
				set
				{
					this.m_children = value;
				}
			}

			internal bool SpanPages
			{
				get
				{
					return (int)(this.m_memberState & MemberState.SpanPages) > 0;
				}
				set
				{
					if (value)
					{
						this.m_memberState |= MemberState.SpanPages;
					}
					else
					{
						this.m_memberState &= ~MemberState.SpanPages;
					}
				}
			}

			internal bool HasInnerPageName
			{
				get
				{
					if (this.m_children != null && this.m_children.Count != 0)
					{
						for (int i = 0; i < this.m_children.Count; i++)
						{
							if (this.m_children[i].HasInnerPageName)
							{
								return true;
							}
						}
						return false;
					}
					return false;
				}
			}

			internal bool Hidden
			{
				get
				{
					return (int)(this.m_memberState & MemberState.Hidden) > 0;
				}
				set
				{
					if (value)
					{
						this.m_memberState |= MemberState.Hidden;
					}
					else
					{
						this.m_memberState &= ~MemberState.Hidden;
					}
				}
			}

			internal bool ContentOnPage
			{
				get
				{
					return (int)(this.m_memberState & MemberState.ContentOnPage) > 0;
				}
				set
				{
					if (value)
					{
						this.m_memberState |= MemberState.ContentOnPage;
					}
					else
					{
						this.m_memberState &= ~MemberState.ContentOnPage;
					}
				}
			}

			internal bool IgnoreTotals
			{
				get
				{
					return (int)(this.m_memberState & MemberState.IgnoreTotals) > 0;
				}
				set
				{
					if (value)
					{
						this.m_memberState |= MemberState.IgnoreTotals;
					}
					else
					{
						this.m_memberState &= ~MemberState.IgnoreTotals;
					}
				}
			}

			internal bool HasOmittedChildren
			{
				get
				{
					if (this.m_children != null && this.m_children.Count != 0)
					{
						for (int i = 0; i < this.m_children.Count; i++)
						{
							if (this.m_children[i].HasOmittedChildren)
							{
								return true;
							}
						}
						return false;
					}
					return false;
				}
			}

			internal bool KeepTogether
			{
				get
				{
					return (int)(this.m_memberState & MemberState.KeepTogether) > 0;
				}
				set
				{
					if (value)
					{
						this.m_memberState |= MemberState.KeepTogether;
					}
					else
					{
						this.m_memberState &= ~MemberState.KeepTogether;
					}
				}
			}

			internal double StartPos
			{
				get
				{
					return this.m_startPos;
				}
				set
				{
					this.m_startPos = value;
				}
			}

			internal double SizeValue
			{
				get
				{
					return this.m_size;
				}
				set
				{
					this.m_size = value;
				}
			}

			internal double EndPos
			{
				get
				{
					return this.m_startPos + this.m_size;
				}
			}

			internal double ContentBottom
			{
				get
				{
					return this.m_startPos + this.ContentHeight;
				}
			}

			internal double ContentRight
			{
				get
				{
					return this.m_startPos + this.ContentWidth;
				}
			}

			internal int ColSpan
			{
				get
				{
					return this.m_colSpan;
				}
				set
				{
					this.m_colSpan = value;
				}
			}

			internal int RowSpan
			{
				get
				{
					return this.m_rowSpan;
				}
				set
				{
					this.m_rowSpan = value;
				}
			}

			internal int CurrRowSpan
			{
				get
				{
					return this.m_currRowSpan;
				}
				set
				{
					this.m_currRowSpan = value;
				}
			}

			internal int CurrColSpan
			{
				get
				{
					return this.m_currColSpan;
				}
				set
				{
					this.m_currColSpan = value;
				}
			}

			internal double ContentHeight
			{
				get
				{
					if (this.m_memberItem != null && !this.Hidden)
					{
						return this.m_memberItem.ItemPageSizes.Bottom;
					}
					return 0.0;
				}
			}

			internal double ContentWidth
			{
				get
				{
					if (this.m_memberItem != null && !this.Hidden)
					{
						return this.m_memberItem.ItemPageSizes.Right;
					}
					return 0.0;
				}
			}

			internal PageItem MemberItem
			{
				get
				{
					return this.m_memberItem;
				}
				set
				{
					this.m_memberItem = value;
				}
			}

			internal string UniqueName
			{
				get
				{
					return this.m_uniqueName;
				}
			}

			internal string Label
			{
				get
				{
					return this.m_label;
				}
			}

			internal string PageName
			{
				get
				{
					return this.m_pageName;
				}
				set
				{
					this.m_pageName = value;
				}
			}

			public int Size
			{
				get
				{
					return 41 + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_memberItem) + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_children) + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_label) + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_uniqueName) + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_pageName);
				}
			}

			internal PageMemberCell()
			{
			}

			internal PageMemberCell(PageItem item, int rowSpan, int colSpan, double sourceWidth, Group group)
			{
				this.m_memberItem = item;
				this.m_sourceWidth = sourceWidth;
				this.m_rowSpan = rowSpan;
				this.m_currRowSpan = rowSpan;
				this.m_colSpan = colSpan;
				this.m_currColSpan = colSpan;
				if (group != null)
				{
					GroupInstance instance = group.Instance;
					if (group.DocumentMapLabel != null)
					{
						if (group.DocumentMapLabel.IsExpression)
						{
							this.m_label = instance.DocumentMapLabel;
						}
						else
						{
							this.m_label = group.DocumentMapLabel.Value;
						}
						if (this.m_label != null)
						{
							this.m_uniqueName = instance.UniqueName;
						}
					}
				}
			}

			public void Serialize(IntermediateFormatWriter writer)
			{
				writer.RegisterDeclaration(PageMemberCell.m_declaration);
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.MemberItem:
						writer.Write(this.m_memberItem);
						break;
					case MemberName.Children:
						writer.Write(this.m_children);
						break;
					case MemberName.Width:
						writer.Write(this.m_sourceWidth);
						break;
					case MemberName.RowSpan:
						writer.Write(this.m_rowSpan);
						break;
					case MemberName.ColSpan:
						writer.Write(this.m_colSpan);
						break;
					case MemberName.CurrRowSpan:
						writer.Write(this.m_currRowSpan);
						break;
					case MemberName.CurrColSpan:
						writer.Write(this.m_currColSpan);
						break;
					case MemberName.StartPos:
						writer.Write(this.m_startPos);
						break;
					case MemberName.Size:
						writer.Write(this.m_size);
						break;
					case MemberName.Label:
						writer.Write(this.m_label);
						break;
					case MemberName.UniqueName:
						writer.Write(this.m_uniqueName);
						break;
					case MemberName.MemberState:
						writer.Write((byte)this.m_memberState);
						break;
					case MemberName.PageName:
						writer.Write(this.m_pageName);
						break;
					default:
						RSTrace.RenderingTracer.Assert(false, string.Empty);
						break;
					}
				}
			}

			public void Deserialize(IntermediateFormatReader reader)
			{
				reader.RegisterDeclaration(PageMemberCell.m_declaration);
				while (reader.NextMember())
				{
					switch (reader.CurrentMember.MemberName)
					{
					case MemberName.MemberItem:
						this.m_memberItem = (PageItem)reader.ReadRIFObject();
						break;
					case MemberName.Children:
						this.m_children = reader.ReadGenericListOfRIFObjects<PageStructMemberCell>();
						break;
					case MemberName.Width:
						this.m_sourceWidth = reader.ReadDouble();
						break;
					case MemberName.RowSpan:
						this.m_rowSpan = reader.ReadInt32();
						break;
					case MemberName.ColSpan:
						this.m_colSpan = reader.ReadInt32();
						break;
					case MemberName.CurrRowSpan:
						this.m_currRowSpan = reader.ReadInt32();
						break;
					case MemberName.CurrColSpan:
						this.m_currColSpan = reader.ReadInt32();
						break;
					case MemberName.StartPos:
						this.m_startPos = reader.ReadDouble();
						break;
					case MemberName.Size:
						this.m_size = reader.ReadDouble();
						break;
					case MemberName.Label:
						this.m_label = reader.ReadString();
						break;
					case MemberName.UniqueName:
						this.m_uniqueName = reader.ReadString();
						break;
					case MemberName.MemberState:
						this.m_memberState = (MemberState)reader.ReadByte();
						break;
					case MemberName.PageName:
						this.m_pageName = reader.ReadString();
						break;
					default:
						RSTrace.RenderingTracer.Assert(false, string.Empty);
						break;
					}
				}
			}

			public void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
			{
			}

			public ObjectType GetObjectType()
			{
				return ObjectType.PageMemberCell;
			}

			internal static Declaration GetDeclaration()
			{
				if (PageMemberCell.m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.MemberItem, ObjectType.PageItem));
					list.Add(new MemberInfo(MemberName.Children, ObjectType.RIFObjectList, ObjectType.PageStructMemberCell));
					list.Add(new MemberInfo(MemberName.Width, Token.Double));
					list.Add(new MemberInfo(MemberName.RowSpan, Token.Int32));
					list.Add(new MemberInfo(MemberName.ColSpan, Token.Int32));
					list.Add(new MemberInfo(MemberName.CurrRowSpan, Token.Int32));
					list.Add(new MemberInfo(MemberName.CurrColSpan, Token.Int32));
					list.Add(new MemberInfo(MemberName.StartPos, Token.Double));
					list.Add(new MemberInfo(MemberName.Size, Token.Double));
					list.Add(new MemberInfo(MemberName.Label, Token.String));
					list.Add(new MemberInfo(MemberName.UniqueName, Token.String));
					list.Add(new MemberInfo(MemberName.MemberState, Token.Byte));
					list.Add(new MemberInfo(MemberName.PageName, Token.String));
					return new Declaration(ObjectType.PageMemberCell, ObjectType.None, list);
				}
				return PageMemberCell.m_declaration;
			}

			internal bool RHOnVerticalPage(ScalableList<RowInfo> detailRows, int rowIndex)
			{
				RowInfo rowInfo = detailRows[rowIndex];
				if (rowInfo.PageVerticalState == RowInfo.VerticalState.Below)
				{
					return false;
				}
				if (this.m_rowSpan > 1)
				{
					rowInfo = detailRows[rowIndex + this.m_rowSpan - 1];
				}
				if (rowInfo.PageVerticalState == RowInfo.VerticalState.Above)
				{
					return false;
				}
				return true;
			}

			internal bool RHColsOnHorizontalPage(List<SizeInfo> rowHeadersWidths, int colIndex, out int colSpan)
			{
				colSpan = 0;
				SizeInfo sizeInfo = null;
				if (this.m_colSpan > 0)
				{
					for (int i = 0; i < this.m_colSpan; i++)
					{
						sizeInfo = rowHeadersWidths[colIndex + i];
						if (sizeInfo.State == SizeInfo.PageState.Normal)
						{
							colSpan++;
						}
					}
					return colSpan > 0;
				}
				return true;
			}

			internal bool CHRowsOnVerticalPage(List<SizeInfo> colHeadersHeights, int rowIndex, out int rowSpan)
			{
				rowSpan = 0;
				SizeInfo sizeInfo = null;
				if (this.m_rowSpan > 0)
				{
					for (int i = 0; i < this.m_rowSpan; i++)
					{
						sizeInfo = colHeadersHeights[rowIndex + i];
						if (sizeInfo.State == SizeInfo.PageState.Normal)
						{
							rowSpan++;
						}
					}
					return rowSpan > 0;
				}
				return true;
			}

			private bool CHOnVerticalPage(List<SizeInfo> colHeadersHeigths, int rowIndex)
			{
				if (colHeadersHeigths == null)
				{
					return true;
				}
				SizeInfo sizeInfo = null;
				if (this.m_rowSpan == 0)
				{
					rowIndex--;
					if (rowIndex < 0)
					{
						rowIndex = 0;
					}
					sizeInfo = colHeadersHeigths[rowIndex];
					if (sizeInfo.State == SizeInfo.PageState.Normal)
					{
						return true;
					}
				}
				else
				{
					for (int i = 0; i < this.m_rowSpan; i++)
					{
						sizeInfo = colHeadersHeigths[rowIndex + i];
						if (sizeInfo.State == SizeInfo.PageState.Normal)
						{
							return true;
						}
					}
				}
				return false;
			}

			internal bool CHOnHorizontalPage(ScalableList<ColumnInfo> columnInfo, int colIndex)
			{
				ColumnInfo columnInfo2 = columnInfo[colIndex];
				if (columnInfo2.PageHorizontalState == ColumnInfo.HorizontalState.AtRight)
				{
					return false;
				}
				if (this.m_colSpan > 1)
				{
					columnInfo2 = columnInfo[colIndex + this.m_colSpan - 1];
				}
				if (columnInfo2.PageHorizontalState == ColumnInfo.HorizontalState.AtLeft)
				{
					return false;
				}
				return true;
			}

			internal void ResolveRHVertical(ScalableList<RowInfo> detailRows, int rowIndex, double startInTablix, double endInTablix, PageContext pageContext)
			{
				int num = rowIndex + this.m_rowSpan - 1;
				while (rowIndex < num && detailRows[rowIndex].PageVerticalState == RowInfo.VerticalState.Above)
				{
					rowIndex++;
				}
				this.m_size = detailRows[num].Bottom - detailRows[rowIndex].Top;
				if (this.m_memberItem != null)
				{
					if (this.m_memberItem.KTVIsUnresolved || this.m_memberItem.NeedResolve)
					{
						double topInParentSystem = Math.Max(0.0, startInTablix - this.m_startPos);
						double bottomInParentSystem = endInTablix - this.m_startPos;
						if (!pageContext.IgnorePageBreaks)
						{
							PageContext pageContext2 = new PageContext(pageContext);
							pageContext2.IgnorePageBreaks = true;
							pageContext2.IgnorePageBreaksReason = PageContext.IgnorePageBreakReason.InsideTablixCell;
						}
						this.m_memberItem.ResolveVertical(pageContext, topInParentSystem, bottomInParentSystem, null, true, pageContext.Common.CanOverwritePageBreak, pageContext.Common.CanSetPageName);
					}
					double num2 = this.m_memberItem.ItemPageSizes.Bottom - this.m_size;
					if (num2 > 0.0)
					{
						this.m_size += num2;
						RowInfo rowInfo = null;
						using (detailRows.GetAndPin(num, out rowInfo))
						{
							rowInfo.Height += num2;
						}
						num++;
						if (num < detailRows.Count)
						{
							using (detailRows.GetAndPin(num, out rowInfo))
							{
								rowInfo.PageVerticalState = RowInfo.VerticalState.Unknown;
							}
						}
					}
				}
			}

			internal void ResolveCHHorizontal(ScalableList<ColumnInfo> columnInfo, int colIndex, double startInTablix, double endInTablix, PageContext pageContext)
			{
				int num = colIndex + this.m_colSpan - 1;
				while (colIndex < num && columnInfo[colIndex].PageHorizontalState == ColumnInfo.HorizontalState.AtLeft)
				{
					colIndex++;
				}
				this.m_size = columnInfo[num].Right - columnInfo[colIndex].Left;
				if (this.m_memberItem != null)
				{
					if (this.m_memberItem.KTHIsUnresolved || this.m_memberItem.NeedResolve)
					{
						double leftInParentSystem = Math.Max(0.0, startInTablix - this.m_startPos);
						double rightInParentSystem = endInTablix - this.m_startPos;
						if (!pageContext.IgnorePageBreaks)
						{
							PageContext pageContext2 = new PageContext(pageContext);
							pageContext2.IgnorePageBreaks = true;
							pageContext2.IgnorePageBreaksReason = PageContext.IgnorePageBreakReason.InsideTablixCell;
						}
						this.m_memberItem.ResolveHorizontal(pageContext, leftInParentSystem, rightInParentSystem, null, true);
					}
					double num2 = this.m_memberItem.ItemPageSizes.Right - this.m_size;
					if (num2 > 0.0)
					{
						this.m_size += num2;
						ColumnInfo columnInfo2 = null;
						using (columnInfo.GetAndPin(num, out columnInfo2))
						{
							columnInfo2.SizeValue += num2;
						}
						num++;
						if (num < columnInfo.Count)
						{
							using (columnInfo.GetAndPin(num, out columnInfo2))
							{
								columnInfo2.PageHorizontalState = ColumnInfo.HorizontalState.Unknown;
							}
						}
					}
				}
			}

			internal void AlignCHToPageVertical(int rowIndex, int targetRowIndex, double startInTablix, double endInTablix, List<SizeInfo> rowHeights, PageContext pageContext)
			{
				if (!this.Hidden)
				{
					if (rowIndex <= targetRowIndex && rowIndex + this.m_rowSpan > targetRowIndex)
					{
						if (this.m_memberItem != null)
						{
							if (!this.m_memberItem.KTVIsUnresolved && !this.m_memberItem.NeedResolve)
							{
								return;
							}
							double startPos = rowHeights[rowIndex].StartPos;
							double num = 0.0;
							for (int i = rowIndex; i < targetRowIndex; i++)
							{
								num += rowHeights[i].SizeValue;
							}
							double topInParentSystem = Math.Max(0.0, startInTablix - startPos);
							double bottomInParentSystem = endInTablix - startPos;
							PageContext pageContext2 = pageContext;
							if (!pageContext.IgnorePageBreaks)
							{
								pageContext2 = new PageContext(pageContext);
								pageContext2.IgnorePageBreaks = true;
								pageContext2.IgnorePageBreaksReason = PageContext.IgnorePageBreakReason.InsideTablixCell;
							}
							this.m_memberItem.ResolveVertical(pageContext2, topInParentSystem, bottomInParentSystem, null, true, pageContext.Common.CanOverwritePageBreak, pageContext.Common.CanSetPageName);
							num = this.m_memberItem.ItemPageSizes.Bottom - num;
							if (num > 0.0)
							{
								SizeInfo.UpdateSize(rowHeights[targetRowIndex], rowIndex + this.m_rowSpan - targetRowIndex, num);
							}
						}
					}
					else if (this.m_children != null)
					{
						for (int j = 0; j < this.m_children.Count; j++)
						{
							this.m_children[j].AlignCHToPageVertical(rowIndex + this.m_rowSpan, targetRowIndex, startInTablix, endInTablix, rowHeights, pageContext);
						}
					}
				}
			}

			internal void AlignRHToPageHorizontal(ScalableList<RowInfo> detailRows, int rowIndex, int colIndex, int targetColIndex, double startInTablix, double endInTablix, List<SizeInfo> colWidths, bool isLTR, PageContext pageContext)
			{
				if (this.RHOnVerticalPage(detailRows, rowIndex))
				{
					if (colIndex <= targetColIndex && colIndex + this.m_colSpan > targetColIndex)
					{
						if (this.m_memberItem != null)
						{
							if (!this.m_memberItem.KTHIsUnresolved && !this.m_memberItem.NeedResolve)
							{
								return;
							}
							double num = 0.0;
							double num2 = 0.0;
							if (isLTR)
							{
								num = colWidths[colIndex].StartPos;
								for (int i = colIndex; i < targetColIndex; i++)
								{
									num2 += colWidths[i].SizeValue;
								}
							}
							else
							{
								num = colWidths[colIndex + this.m_colSpan - 1].StartPos;
								for (int num3 = colIndex + this.m_colSpan - 1; num3 > targetColIndex; num3--)
								{
									num2 += colWidths[num3].SizeValue;
								}
							}
							double leftInParentSystem = Math.Max(0.0, startInTablix - num);
							double rightInParentSystem = endInTablix - num;
							this.m_memberItem.ResolveHorizontal(pageContext, leftInParentSystem, rightInParentSystem, null, true);
							num2 = this.m_memberItem.ItemPageSizes.Right - num2;
							if (num2 > 0.0)
							{
								if (isLTR)
								{
									SizeInfo.UpdateSize(colWidths[targetColIndex], colIndex + this.m_colSpan - targetColIndex, num2);
								}
								else
								{
									SizeInfo.UpdateSize(colWidths[targetColIndex], targetColIndex - colIndex + 1, num2);
								}
							}
						}
					}
					else if (this.m_children != null)
					{
						for (int j = 0; j < this.m_children.Count; j++)
						{
							this.m_children[j].AlignRHToPageHorizontal(detailRows, rowIndex, colIndex + this.m_colSpan, targetColIndex, startInTablix, endInTablix, colWidths, isLTR, pageContext);
							rowIndex += this.m_children[j].Span;
						}
					}
				}
			}

			internal void CalculateRHHorizontal(ScalableList<RowInfo> detailRows, int rowIndex, int colIndex, ref List<SizeInfo> colWidths, PageContext pageContext)
			{
				if (!this.Hidden && this.RHOnVerticalPage(detailRows, rowIndex))
				{
					if (this.m_memberItem != null)
					{
						bool flag = false;
						PageContext pageContext2 = new PageContext(pageContext);
						this.m_memberItem.CalculateHorizontal(pageContext2, 0.0, 1.7976931348623157E+308, null, new List<PageItem>(), ref flag, true, this.m_sourceWidth);
						double size = Math.Max(this.m_sourceWidth, this.m_memberItem.ItemPageSizes.Width);
						Tablix.UpdateSizes(colIndex, this.m_colSpan, size, ref colWidths);
					}
					else
					{
						Tablix.UpdateSizes(colIndex, this.m_colSpan, this.m_sourceWidth, ref colWidths);
					}
					if (this.m_children != null)
					{
						colIndex += this.m_colSpan;
						for (int i = 0; i < this.m_children.Count; i++)
						{
							this.m_children[i].CalculateRHHorizontal(detailRows, rowIndex, colIndex, ref colWidths, pageContext);
							rowIndex += this.m_children[i].Span;
						}
					}
				}
			}

			internal int AddToPageRHContent(List<SizeInfo> colWidths, ScalableList<RowInfo> detailRows, int rowIndex, int colIndex, bool isLTR, RPLWriter rplWriter, PageContext pageContext, double pageLeft, double pageTop, double pageRight, double pageBottom, RepeatState repeatState, ref bool hasLabels)
			{
				if (!this.RHOnVerticalPage(detailRows, rowIndex))
				{
					return 0;
				}
				if (this.m_label != null)
				{
					hasLabels = true;
				}
				if (this.m_memberItem != null)
				{
					double pageTop2 = Math.Max(0.0, pageTop - this.m_startPos);
					double pageBottom2 = pageBottom - this.m_startPos;
					double num = 0.0;
					double num2 = 0.0;
					double num3 = 0.0;
					if (this.Hidden)
					{
						if (colWidths != null)
						{
							if (isLTR)
							{
								colIndex--;
								num = ((colIndex < 0) ? colWidths[0].StartPos : colWidths[colIndex].EndPos);
							}
							else
							{
								num = ((colIndex != colWidths.Count - 1) ? colWidths[colIndex].EndPos : colWidths[colIndex].StartPos);
							}
						}
					}
					else
					{
						num = ((!isLTR) ? colWidths[colIndex + this.m_colSpan - 1].StartPos : colWidths[colIndex].StartPos);
					}
					num3 = Math.Max(0.0, pageLeft - num);
					num2 = pageRight - num;
					double height = this.m_memberItem.ItemPageSizes.Height;
					this.m_memberItem.ItemPageSizes.AdjustHeightTo(this.m_size + Math.Abs(this.m_memberItem.ItemPageSizes.Top));
					this.ContentOnPage = this.m_memberItem.AddToPage(rplWriter, pageContext, num3, pageTop2, num2, pageBottom2, repeatState);
					if (!PageMemberCell.IsSpanning(this.m_startPos, this.EndPos, pageTop, pageBottom))
					{
						RowInfo rowInfo = detailRows[rowIndex];
						this.m_memberItem.ItemPageSizes.AdjustHeightTo(rowInfo.RepeatOnPage ? height : 0.0);
					}
					if (this.ContentOnPage)
					{
						this.ContentOnPage = this.m_memberItem.ContentOnPage;
					}
				}
				int num4 = 0;
				if (this.m_children == null)
				{
					if (this.Hidden)
					{
						return num4;
					}
					for (int i = 0; i < this.m_rowSpan; i++)
					{
						if (detailRows[rowIndex + i].PageVerticalState == RowInfo.VerticalState.Normal)
						{
							num4++;
						}
					}
					return num4;
				}
				if (!this.Hidden)
				{
					colIndex += this.m_colSpan;
				}
				for (int j = 0; j < this.m_children.Count; j++)
				{
					num4 += this.m_children[j].AddToPageRHContent(colWidths, detailRows, rowIndex, colIndex, isLTR, rplWriter, pageContext, pageLeft, pageTop, pageRight, pageBottom, repeatState, ref hasLabels);
					rowIndex += this.m_children[j].Span;
				}
				return num4;
			}

			internal void CalculateCHHorizontal(List<SizeInfo> rowHeigths, int rowIndex, int colIndex, ScalableList<ColumnInfo> columnInfo, PageContext pageContext, bool onPage)
			{
				if (this.Hidden)
				{
					Tablix.UpdateHidden(colIndex, this.m_colSpan, columnInfo);
				}
				else
				{
					if (this.m_children != null)
					{
						int num = colIndex;
						for (int i = 0; i < this.m_children.Count; i++)
						{
							this.m_children[i].CalculateCHHorizontal(rowHeigths, rowIndex + this.m_rowSpan, num, columnInfo, pageContext, onPage);
							num += this.m_children[i].Span;
						}
					}
					if (onPage && this.CHOnVerticalPage(rowHeigths, rowIndex))
					{
						if (this.m_memberItem != null)
						{
							bool flag = false;
							bool unresolved = this.m_memberItem.KTHIsUnresolved || this.m_memberItem.NeedResolve;
							PageContext pageContext2 = new PageContext(pageContext);
							this.m_memberItem.CalculateHorizontal(pageContext2, 0.0, 1.7976931348623157E+308, null, new List<PageItem>(), ref flag, true, this.m_sourceWidth);
							Tablix.UpdateSizes(colIndex, this.m_colSpan, this.m_memberItem.ItemPageSizes.Width, unresolved, false, true, columnInfo);
						}
						else
						{
							Tablix.UpdateSizes(colIndex, this.m_colSpan, this.m_sourceWidth, false, false, true, columnInfo);
						}
					}
				}
			}

			internal int AddToPageCHContent(List<SizeInfo> rowHeights, ScalableList<ColumnInfo> columnInfo, int rowIndex, int colIndex, bool isLTR, RPLWriter rplWriter, PageContext pageContext, double pageLeft, double pageTop, double pageRight, double pageBottom, RepeatState repeatState, ref bool hasLabels)
			{
				int num = 0;
				int num2 = rowIndex;
				if (!this.Hidden)
				{
					num2 += this.m_rowSpan;
				}
				if (this.m_label != null)
				{
					hasLabels = true;
				}
				if (this.m_children != null)
				{
					int num3 = colIndex;
					for (int i = 0; i < this.m_children.Count; i++)
					{
						num += this.m_children[i].AddToPageCHContent(rowHeights, columnInfo, num2, num3, isLTR, rplWriter, pageContext, pageLeft, pageTop, pageRight, pageBottom, repeatState, ref hasLabels);
						num3 += this.m_children[i].Span;
					}
				}
				else if (!this.Hidden)
				{
					for (int j = 0; j < this.m_colSpan; j++)
					{
						if (columnInfo[colIndex + j].PageHorizontalState == ColumnInfo.HorizontalState.Normal)
						{
							num++;
						}
					}
				}
				if (this.m_memberItem != null)
				{
					RepeatState repeatState2 = repeatState;
					if (num != this.m_colSpan && columnInfo[colIndex + this.m_colSpan - 1].PageHorizontalState != ColumnInfo.HorizontalState.Normal)
					{
						repeatState2 |= RepeatState.Horizontal;
					}
					if (this.Hidden)
					{
						double num4 = 0.0;
						rowIndex--;
						if (rowHeights != null)
						{
							if (rowIndex < 0)
							{
								rowIndex = 0;
								num4 = rowHeights[0].StartPos;
							}
							else
							{
								num4 = rowHeights[rowIndex].EndPos;
							}
						}
						if (rowHeights == null || rowHeights[rowIndex].State == SizeInfo.PageState.Normal)
						{
							double pageTop2 = Math.Max(0.0, pageTop - num4);
							double pageBottom2 = pageBottom - num4;
							double pageLeft2 = Math.Max(0.0, pageLeft - this.m_startPos);
							double pageRight2 = pageRight - this.m_startPos;
							if (rowHeights != null)
							{
								this.m_memberItem.ItemPageSizes.AdjustHeightTo(rowHeights[rowIndex + this.m_rowSpan - 1].EndPos - num4);
							}
							this.ContentOnPage = this.m_memberItem.AddToPage(rplWriter, pageContext, pageLeft2, pageTop2, pageRight2, pageBottom2, repeatState2);
						}
					}
					else
					{
						if (!this.CHOnVerticalPage(rowHeights, rowIndex))
						{
							return num;
						}
						double startPos = rowHeights[rowIndex].StartPos;
						double pageTop3 = Math.Max(0.0, pageTop - startPos);
						double pageBottom3 = pageBottom - startPos;
						double pageLeft3 = Math.Max(0.0, pageLeft - this.m_startPos);
						double pageRight3 = pageRight - this.m_startPos;
						this.m_memberItem.ItemPageSizes.AdjustHeightTo(rowHeights[rowIndex + this.m_rowSpan - 1].EndPos - startPos);
						this.ContentOnPage = this.m_memberItem.AddToPage(rplWriter, pageContext, pageLeft3, pageTop3, pageRight3, pageBottom3, repeatState2);
					}
					if (this.ContentOnPage)
					{
						this.ContentOnPage = this.m_memberItem.ContentOnPage;
					}
				}
				return num;
			}

			internal double NormalizeDetailRowHeight(ScalableList<RowInfo> detailRows, int rowIndex, double startInTablix, double endInTablix, ref int lastRowOnPage, ref int lastRowBelow, bool update, ref double addedHeight)
			{
				double num = 0.0;
				int num2 = rowIndex;
				if (this.m_children != null)
				{
					for (int i = 0; i < this.m_children.Count; i++)
					{
						num += this.m_children[i].NormalizeDetailRowHeight(detailRows, num2, startInTablix, endInTablix, ref lastRowOnPage, ref lastRowBelow, update, ref addedHeight);
						num2 += this.m_children[i].Span;
					}
				}
				else
				{
					RowInfo rowInfo = null;
					RoundedDouble roundedDouble = new RoundedDouble(0.0);
					for (int j = 0; j < this.m_rowSpan; j++)
					{
						using (detailRows.GetAndPin(num2, out rowInfo))
						{
							if (rowInfo.PageVerticalState == RowInfo.VerticalState.Above)
							{
								num2++;
								goto end_IL_0089;
							}
							if (rowInfo.PageVerticalState == RowInfo.VerticalState.Repeat)
							{
								rowInfo.Top = startInTablix;
								if (lastRowOnPage >= 0)
								{
									rowInfo.Top = detailRows[lastRowOnPage].Bottom;
								}
								lastRowOnPage = num2;
								rowInfo.PageVerticalState = RowInfo.VerticalState.Normal;
								rowInfo.RepeatOnPage = true;
								num += detailRows[num2].Height;
							}
							else if (rowInfo.PageVerticalState == RowInfo.VerticalState.TopOfNextPage)
							{
								rowInfo.Top = endInTablix;
								rowInfo.PageVerticalState = RowInfo.VerticalState.Below;
								lastRowBelow = num2;
							}
							else if (rowInfo.PageVerticalState == RowInfo.VerticalState.Below)
							{
								rowInfo.PageVerticalState = RowInfo.VerticalState.Normal;
								if (lastRowOnPage >= 0)
								{
									rowInfo.Top = detailRows[lastRowOnPage].Bottom;
								}
								lastRowOnPage = num2;
								num += detailRows[num2].Height;
							}
							else if (lastRowBelow >= 0)
							{
								rowInfo.Top = detailRows[lastRowBelow].Bottom;
								lastRowBelow = num2;
								rowInfo.PageVerticalState = RowInfo.VerticalState.Below;
							}
							else
							{
								if (rowInfo.PageVerticalState == RowInfo.VerticalState.Unknown)
								{
									rowInfo.Top = startInTablix;
									rowInfo.PageVerticalState = RowInfo.VerticalState.Normal;
								}
								if (lastRowOnPage >= 0)
								{
									rowInfo.Top = detailRows[lastRowOnPage].Bottom;
								}
								if (update)
								{
									roundedDouble.Value = rowInfo.Top;
									if (roundedDouble >= endInTablix)
									{
										lastRowBelow = num2;
										rowInfo.PageVerticalState = RowInfo.VerticalState.Below;
									}
									else
									{
										lastRowOnPage = num2;
									}
								}
								else
								{
									lastRowOnPage = num2;
								}
								num += detailRows[num2].Height;
							}
							goto IL_01f5;
							end_IL_0089:;
						}
						continue;
						IL_01f5:
						num2++;
					}
				}
				while (rowIndex < num2 && detailRows[rowIndex].PageVerticalState == RowInfo.VerticalState.Above)
				{
					rowIndex++;
				}
				if (rowIndex == num2)
				{
					return 0.0;
				}
				if (rowIndex < detailRows.Count)
				{
					this.m_startPos = detailRows[rowIndex].Top;
				}
				if (detailRows[rowIndex].PageVerticalState == RowInfo.VerticalState.Below)
				{
					return 0.0;
				}
				if (this.m_memberItem != null && !this.Hidden)
				{
					double num3 = this.m_memberItem.ItemPageSizes.Bottom - num;
					if (num3 > 0.0)
					{
						addedHeight += num3;
						num += num3;
						rowIndex += this.m_currRowSpan - 1;
						RowInfo rowInfo2 = null;
						using (detailRows.GetAndPin(rowIndex, out rowInfo2))
						{
							rowInfo2.Height += num3;
						}
					}
				}
				this.m_size = num;
				return num;
			}

			private static bool IsSpanning(double top, double bottom, double startTop, double endBottom)
			{
				RoundedDouble x = new RoundedDouble(top);
				RoundedDouble x2 = new RoundedDouble(bottom);
				if (x < startTop && x2 > startTop)
				{
					return true;
				}
				if (x >= startTop)
				{
					return x2 > endBottom;
				}
				return false;
			}

			internal void UpdateRows(ScalableList<RowInfo> detailRows, int rowIndex, bool parentKeepRepeatWith, int keepRow, bool delete, double startInTablix, double endInTablix)
			{
				int num = rowIndex;
				RowInfo rowInfo = detailRows[num];
				if (rowInfo.PageVerticalState != RowInfo.VerticalState.Below)
				{
					rowInfo = detailRows[num + this.m_rowSpan - 1];
					if (rowInfo.PageVerticalState == RowInfo.VerticalState.Above)
					{
						this.m_currRowSpan = this.m_rowSpan;
					}
					else
					{
						this.m_pageName = null;
						bool parentKeepRepeatWith2 = parentKeepRepeatWith;
						bool flag = false;
						if (this.m_memberItem != null && PageMemberCell.IsSpanning(this.m_startPos, this.EndPos, startInTablix, endInTablix))
						{
							parentKeepRepeatWith2 = false;
							flag = true;
						}
						if (this.m_children != null)
						{
							PageStructMemberCell pageStructMemberCell = null;
							for (int i = 0; i < this.m_children.Count; i++)
							{
								pageStructMemberCell = this.m_children[i];
								pageStructMemberCell.UpdateRows(detailRows, num, parentKeepRepeatWith2, keepRow, delete, startInTablix, endInTablix);
								if (delete && pageStructMemberCell.Span == 0 && !pageStructMemberCell.PartialItem)
								{
									this.m_children.RemoveAt(i);
									i--;
									i -= Tablix.RemoveHeadersAbove(this.m_children, i, detailRows, ref num);
								}
								num += pageStructMemberCell.Span;
							}
							if (this.m_children.Count == 0)
							{
								this.m_children = null;
							}
							this.m_rowSpan = (this.m_currRowSpan = num - rowIndex);
						}
						else
						{
							num += this.m_rowSpan;
							RoundedDouble roundedDouble = new RoundedDouble(0.0);
							for (int j = 0; j < this.m_rowSpan; j++)
							{
								rowInfo = detailRows[rowIndex + j];
								if (rowInfo.SpanPagesRow)
								{
									break;
								}
								roundedDouble.Value = rowInfo.Bottom;
								if (!(roundedDouble <= endInTablix))
								{
									break;
								}
								if (parentKeepRepeatWith && keepRow > 0)
								{
									if (keepRow == 1)
									{
										if (rowInfo.PageVerticalState != RowInfo.VerticalState.Above)
										{
											roundedDouble.Value = rowInfo.Top;
											if (roundedDouble >= startInTablix)
											{
												using (detailRows.GetAndPin(rowIndex + j, out rowInfo))
												{
													rowInfo.PageVerticalState = RowInfo.VerticalState.Above;
												}
												continue;
											}
											goto IL_0202;
										}
										continue;
									}
									if (rowInfo.RepeatOnPage)
									{
										using (detailRows.GetAndPin(rowIndex + j, out rowInfo))
										{
											rowInfo.PageVerticalState = RowInfo.VerticalState.Unknown;
											rowInfo.RepeatOnPage = false;
										}
										continue;
									}
								}
								goto IL_0202;
								IL_0202:
								if (delete)
								{
									rowInfo.DisposeDetailCells();
									detailRows.RemoveAt(rowIndex + j);
									this.m_rowSpan--;
									j--;
									num--;
								}
							}
							this.m_currRowSpan = this.m_rowSpan;
						}
						if (this.m_rowSpan > 0)
						{
							while (rowIndex < num && detailRows[rowIndex].PageVerticalState == RowInfo.VerticalState.Above)
							{
								rowIndex++;
								this.m_currRowSpan--;
							}
							if (this.m_currRowSpan > 0)
							{
								if (flag)
								{
									double num2 = detailRows[rowIndex].Top - this.m_startPos;
									if (num2 > 0.0)
									{
										this.m_memberItem.ItemPageSizes.Top -= num2;
										this.m_size -= num2;
									}
									this.m_startPos = detailRows[rowIndex].Top;
								}
								else
								{
									this.m_startPos = detailRows[rowIndex].Top;
									this.m_size = detailRows[rowIndex + this.m_currRowSpan - 1].Bottom - this.m_startPos;
								}
							}
							else
							{
								this.m_currRowSpan = this.m_rowSpan;
							}
						}
					}
				}
			}

			internal void UpdateColumns(ScalableList<ColumnInfo> columnInfo, int colIndex, double startInTablix, double endInTablix)
			{
				int num = colIndex;
				bool flag = false;
				if (this.m_memberItem != null && PageMemberCell.IsSpanning(this.m_startPos, this.EndPos, startInTablix, endInTablix))
				{
					flag = true;
				}
				if (this.m_children != null)
				{
					PageStructMemberCell pageStructMemberCell = null;
					for (int i = 0; i < this.m_children.Count; i++)
					{
						pageStructMemberCell = this.m_children[i];
						pageStructMemberCell.UpdateColumns(columnInfo, num, startInTablix, endInTablix);
						num += pageStructMemberCell.Span;
					}
				}
				else
				{
					num += this.m_colSpan;
					RoundedDouble roundedDouble = new RoundedDouble(0.0);
					ColumnInfo columnInfo2 = null;
					while (colIndex < num)
					{
						columnInfo2 = columnInfo[colIndex];
						if (columnInfo2.PageHorizontalState == ColumnInfo.HorizontalState.AtLeft)
						{
							colIndex++;
						}
						else
						{
							roundedDouble.Value = columnInfo2.Right;
							if (!(roundedDouble <= endInTablix))
							{
								break;
							}
							using (columnInfo.GetAndPin(colIndex, out columnInfo2))
							{
								columnInfo2.PageHorizontalState = ColumnInfo.HorizontalState.AtLeft;
							}
							colIndex++;
						}
					}
				}
				if (this.m_colSpan > 0)
				{
					while (colIndex < num && columnInfo[colIndex].PageHorizontalState == ColumnInfo.HorizontalState.AtLeft)
					{
						colIndex++;
					}
					this.m_currColSpan = num - colIndex;
					if (this.m_currColSpan > 0)
					{
						ColumnInfo columnInfo3 = columnInfo[colIndex];
						if (flag)
						{
							double num2 = columnInfo3.Left - this.m_startPos;
							if (num2 > 0.0)
							{
								this.m_memberItem.ItemPageSizes.Left -= num2;
								this.m_size -= num2;
							}
							this.m_startPos = columnInfo3.Left;
						}
						else
						{
							this.m_startPos = columnInfo3.Left;
							if (this.m_currColSpan == 1)
							{
								this.m_size = columnInfo3.Right - this.m_startPos;
							}
							else
							{
								this.m_size = columnInfo[colIndex + this.m_currColSpan - 1].Right - this.m_startPos;
							}
						}
					}
					else
					{
						this.m_currColSpan = this.m_colSpan;
					}
				}
			}

			internal double NormalizeDetailColWidth(ScalableList<ColumnInfo> detailCols, int colIndex, double startInTablix, double endInTablix, ref int lastColOnPage, ref int lastColAtRight, bool update, ref double addedWidth)
			{
				double num = 0.0;
				int num2 = colIndex;
				if (this.m_children != null)
				{
					for (int i = 0; i < this.m_children.Count; i++)
					{
						num += this.m_children[i].NormalizeDetailColWidth(detailCols, num2, startInTablix, endInTablix, ref lastColOnPage, ref lastColAtRight, update, ref addedWidth);
						num2 += this.m_children[i].Span;
					}
				}
				else
				{
					ColumnInfo columnInfo = null;
					RoundedDouble roundedDouble = new RoundedDouble(0.0);
					for (int j = 0; j < this.m_colSpan; j++)
					{
						using (detailCols.GetAndPin(num2, out columnInfo))
						{
							if (columnInfo.PageHorizontalState == ColumnInfo.HorizontalState.AtLeft)
							{
								num2++;
								goto end_IL_0089;
							}
							if (columnInfo.PageHorizontalState == ColumnInfo.HorizontalState.LeftOfNextPage)
							{
								columnInfo.Left = endInTablix;
								columnInfo.PageHorizontalState = ColumnInfo.HorizontalState.AtRight;
								lastColAtRight = num2;
							}
							else if (columnInfo.PageHorizontalState == ColumnInfo.HorizontalState.AtRight)
							{
								columnInfo.PageHorizontalState = ColumnInfo.HorizontalState.Normal;
								if (lastColOnPage >= 0)
								{
									columnInfo.Left = detailCols[lastColOnPage].Right;
								}
								lastColOnPage = num2;
								num += columnInfo.SizeValue;
							}
							else if (lastColAtRight >= 0)
							{
								columnInfo.Left = detailCols[lastColAtRight].Right;
								lastColAtRight = num2;
								columnInfo.PageHorizontalState = ColumnInfo.HorizontalState.AtRight;
							}
							else
							{
								if (columnInfo.PageHorizontalState == ColumnInfo.HorizontalState.Unknown)
								{
									columnInfo.Left = startInTablix;
									columnInfo.PageHorizontalState = ColumnInfo.HorizontalState.Normal;
								}
								if (lastColOnPage >= 0)
								{
									columnInfo.Left = detailCols[lastColOnPage].Right;
								}
								if (update)
								{
									roundedDouble.Value = columnInfo.Left;
									if (roundedDouble >= endInTablix)
									{
										lastColAtRight = num2;
										columnInfo.PageHorizontalState = ColumnInfo.HorizontalState.AtRight;
									}
									else
									{
										lastColOnPage = num2;
									}
								}
								else
								{
									lastColOnPage = num2;
								}
								num += columnInfo.SizeValue;
							}
							goto IL_0199;
							end_IL_0089:;
						}
						continue;
						IL_0199:
						num2++;
					}
				}
				while (colIndex < num2 && detailCols[colIndex].PageHorizontalState == ColumnInfo.HorizontalState.AtLeft)
				{
					colIndex++;
				}
				if (colIndex == num2)
				{
					return 0.0;
				}
				if (colIndex < detailCols.Count)
				{
					this.m_startPos = detailCols[colIndex].Left;
				}
				if (detailCols[colIndex].PageHorizontalState == ColumnInfo.HorizontalState.AtRight)
				{
					return 0.0;
				}
				if (this.m_memberItem != null && !this.Hidden)
				{
					double num3 = this.m_memberItem.ItemPageSizes.Right - num;
					if (num3 > 0.0)
					{
						addedWidth += num3;
						num += num3;
						colIndex += this.m_currColSpan - 1;
						ColumnInfo columnInfo2 = null;
						using (detailCols.GetAndPin(colIndex, out columnInfo2))
						{
							columnInfo2.SizeValue += num3;
						}
					}
				}
				this.m_size = num;
				return num;
			}

			internal void Reverse(PageContext pageContext)
			{
				if (this.m_children != null)
				{
					this.m_children.Reverse();
					for (int i = 0; i < this.m_children.Count; i++)
					{
						this.m_children[i].Reverse(pageContext);
					}
				}
			}

			internal bool ResolveCHDuplicates(int rowIndex, int targetRowIndex, double startInTablix, List<SizeInfo> rowHeights, PageContext pageContext)
			{
				if (this.Hidden)
				{
					return false;
				}
				bool flag = false;
				if (rowIndex <= targetRowIndex && rowIndex + this.m_rowSpan > targetRowIndex && this.m_memberItem != null)
				{
					double topInParentSystem = Math.Max(0.0, startInTablix - rowHeights[rowIndex].StartPos);
					flag = this.m_memberItem.ResolveDuplicates(pageContext, topInParentSystem, null, false);
					if (flag)
					{
						double startPo = rowHeights[rowIndex].StartPos;
						double num = 0.0;
						for (int i = rowIndex; i < targetRowIndex; i++)
						{
							num += rowHeights[i].SizeValue;
						}
						num = this.m_memberItem.ItemPageSizes.Bottom - num;
						if (num > 0.0)
						{
							SizeInfo.UpdateSize(rowHeights[targetRowIndex], rowIndex + this.m_rowSpan - targetRowIndex, num);
						}
					}
				}
				if (this.m_children != null)
				{
					for (int j = 0; j < this.m_children.Count; j++)
					{
						flag |= this.m_children[j].ResolveCHDuplicates(rowIndex + this.m_rowSpan, targetRowIndex, startInTablix, rowHeights, pageContext);
					}
				}
				return flag;
			}

			internal bool ResolveRHDuplicates(int rowIndex, double startInTablix, ScalableList<RowInfo> detailRows, PageContext pageContext)
			{
				int num = rowIndex;
				int num2 = rowIndex + this.m_rowSpan - 1;
				while (rowIndex < num2 && detailRows[rowIndex].PageVerticalState == RowInfo.VerticalState.Above)
				{
					rowIndex++;
				}
				double startPos = this.m_startPos;
				if (rowIndex < detailRows.Count)
				{
					this.m_startPos = detailRows[rowIndex].Top;
				}
				bool flag = false;
				if (this.m_memberItem != null && !this.Hidden)
				{
					double topInParentSystem = Math.Max(0.0, startInTablix - startPos);
					flag = this.m_memberItem.ResolveDuplicates(pageContext, topInParentSystem, null, false);
				}
				if (this.m_children != null)
				{
					for (int i = 0; i < this.m_children.Count; i++)
					{
						flag |= this.m_children[i].ResolveRHDuplicates(num, startInTablix, detailRows, pageContext);
						num += this.m_children[i].Span;
					}
				}
				return flag;
			}
		}

		internal abstract class PageStructMemberCell : IStorable, IPersistable
		{
			[Flags]
			private enum MemberState : byte
			{
				Clear = 0,
				PartialItem = 1,
				CreateItem = 2,
				HasOmittedChildren = 4,
				NotOmittedList = 8,
				SpanPages = 0x10,
				HasInstances = 0x20,
				HasInnerPageName = 0x40
			}

			private int m_sourceIndex;

			private MemberState m_memberState;

			protected int m_span;

			private int m_memberDefIndex = -1;

			private TablixMember m_tablixMember;

			private static Declaration m_declaration = PageStructMemberCell.GetDeclaration();

			internal int SourceIndex
			{
				get
				{
					return this.m_sourceIndex;
				}
			}

			internal bool PartialItem
			{
				get
				{
					return (int)(this.m_memberState & MemberState.PartialItem) > 0;
				}
				set
				{
					if (value)
					{
						this.m_memberState |= MemberState.PartialItem;
					}
					else
					{
						this.m_memberState &= ~MemberState.PartialItem;
					}
				}
			}

			internal bool CreateItem
			{
				get
				{
					return (int)(this.m_memberState & MemberState.CreateItem) > 0;
				}
				set
				{
					if (value)
					{
						this.m_memberState |= MemberState.CreateItem;
					}
					else
					{
						this.m_memberState &= ~MemberState.CreateItem;
					}
				}
			}

			internal bool OmittedList
			{
				get
				{
					return false;
				}
				set
				{
					if (value)
					{
						this.m_memberState &= ~MemberState.NotOmittedList;
					}
					else
					{
						this.m_memberState |= MemberState.NotOmittedList;
					}
				}
			}

			internal bool HasOmittedChildren
			{
				get
				{
					return (int)(this.m_memberState & MemberState.HasOmittedChildren) > 0;
				}
				set
				{
					if (value)
					{
						this.m_memberState |= MemberState.HasOmittedChildren;
					}
					else
					{
						this.m_memberState &= ~MemberState.HasOmittedChildren;
					}
				}
			}

			internal bool SpanPages
			{
				get
				{
					return (int)(this.m_memberState & MemberState.SpanPages) > 0;
				}
				set
				{
					if (value)
					{
						this.m_memberState |= MemberState.SpanPages;
					}
					else
					{
						this.m_memberState &= ~MemberState.SpanPages;
					}
				}
			}

			internal bool HasInnerPageName
			{
				get
				{
					return (int)(this.m_memberState & MemberState.HasInnerPageName) > 0;
				}
				set
				{
					if (value)
					{
						this.m_memberState |= MemberState.HasInnerPageName;
					}
					else
					{
						this.m_memberState &= ~MemberState.HasInnerPageName;
					}
				}
			}

			internal bool HasInstances
			{
				get
				{
					return (int)(this.m_memberState & MemberState.HasInstances) > 0;
				}
				set
				{
					if (value)
					{
						this.m_memberState |= MemberState.HasInstances;
					}
					else
					{
						this.m_memberState &= ~MemberState.HasInstances;
					}
				}
			}

			internal virtual bool StaticTree
			{
				get
				{
					return false;
				}
				set
				{
				}
			}

			internal int Span
			{
				get
				{
					return this.m_span;
				}
				set
				{
					this.m_span = value;
				}
			}

			internal int MemberDefIndex
			{
				get
				{
					return this.m_memberDefIndex;
				}
			}

			internal byte State
			{
				get
				{
					return (byte)this.m_memberState;
				}
			}

			internal abstract bool Hidden
			{
				get;
			}

			internal abstract double StartPos
			{
				get;
			}

			internal abstract double SizeValue
			{
				get;
			}

			internal abstract double EndPos
			{
				get;
			}

			internal abstract int PartialRowSpan
			{
				get;
			}

			internal TablixMember TablixMember
			{
				get
				{
					return this.m_tablixMember;
				}
			}

			public virtual int Size
			{
				get
				{
					return AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.ReferenceSize + 1 + 12;
				}
			}

			internal PageStructMemberCell()
			{
			}

			internal PageStructMemberCell(int sourceIndex, bool partialItem, bool createItem, TablixMember member, int memberDefIndex)
			{
				this.m_sourceIndex = sourceIndex;
				if (partialItem)
				{
					this.m_memberState |= MemberState.PartialItem;
				}
				if (createItem)
				{
					this.m_memberState |= MemberState.CreateItem;
				}
				this.m_memberDefIndex = memberDefIndex;
				this.m_tablixMember = member;
			}

			internal PageStructMemberCell(PageStructMemberCell copy, int span)
			{
				this.m_sourceIndex = copy.SourceIndex;
				this.m_memberDefIndex = copy.MemberDefIndex;
				this.m_memberState = (MemberState)copy.State;
				this.m_span = span;
				this.m_tablixMember = copy.TablixMember;
			}

			public virtual void Serialize(IntermediateFormatWriter writer)
			{
				writer.RegisterDeclaration(PageStructMemberCell.m_declaration);
				IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.State:
						writer.Write((byte)this.m_memberState);
						break;
					case MemberName.SourceIndex:
						writer.Write(this.m_sourceIndex);
						break;
					case MemberName.DefIndex:
						writer.Write(this.m_memberDefIndex);
						break;
					case MemberName.Span:
						writer.Write(this.m_span);
						break;
					case MemberName.TablixMember:
					{
						int value = scalabilityCache.StoreStaticReference(this.m_tablixMember);
						writer.Write(value);
						break;
					}
					default:
						RSTrace.RenderingTracer.Assert(false, string.Empty);
						break;
					}
				}
			}

			public virtual void Deserialize(IntermediateFormatReader reader)
			{
				reader.RegisterDeclaration(PageStructMemberCell.m_declaration);
				IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
				while (reader.NextMember())
				{
					switch (reader.CurrentMember.MemberName)
					{
					case MemberName.State:
						this.m_memberState = (MemberState)reader.ReadByte();
						break;
					case MemberName.SourceIndex:
						this.m_sourceIndex = reader.ReadInt32();
						break;
					case MemberName.DefIndex:
						this.m_memberDefIndex = reader.ReadInt32();
						break;
					case MemberName.Span:
						this.m_span = reader.ReadInt32();
						break;
					case MemberName.TablixMember:
					{
						int id = reader.ReadInt32();
						this.m_tablixMember = (TablixMember)scalabilityCache.FetchStaticReference(id);
						break;
					}
					default:
						RSTrace.RenderingTracer.Assert(false, string.Empty);
						break;
					}
				}
			}

			public void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
			{
			}

			public virtual ObjectType GetObjectType()
			{
				return ObjectType.PageStructMemberCell;
			}

			internal static Declaration GetDeclaration()
			{
				if (PageStructMemberCell.m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.State, Token.Byte));
					list.Add(new MemberInfo(MemberName.SourceIndex, Token.Int32));
					list.Add(new MemberInfo(MemberName.DefIndex, Token.Int32));
					list.Add(new MemberInfo(MemberName.Span, Token.Int32));
					list.Add(new MemberInfo(MemberName.TablixMember, Token.Int32));
					return new Declaration(ObjectType.PageStructMemberCell, ObjectType.None, list);
				}
				return PageStructMemberCell.m_declaration;
			}

			internal abstract IDisposable PartialMemberInstance(out PageMemberCell memberCell);

			internal abstract void RemoveLastMemberCell();

			internal abstract void DisposeInstances();

			internal abstract void MergePageStructMembers(PageStructMemberCell mergeStructMember, TablixRegion region, MergeDetailRows detailRowsState);

			internal abstract bool ResolveCHDuplicates(int rowIndex, int targetRowIndex, double startInTablix, List<SizeInfo> rowHeights, PageContext pageContext);

			internal abstract bool ResolveRHDuplicates(int rowIndex, double startInTablix, ScalableList<RowInfo> detailRows, PageContext pageContext);

			internal abstract MergeDetailRows AddPageMemberCell(PageMemberCell memberCell, TablixRegion region, PageContext pageContext);

			internal abstract void AlignCHToPageVertical(int rowIndex, int targetRowIndex, double startInTablix, double endInTablix, List<SizeInfo> rowHeights, PageContext pageContext);

			internal abstract void AlignRHToPageHorizontal(ScalableList<RowInfo> detailRows, int rowIndex, int colIndex, int targetColIndex, double startInTablix, double endInTablix, List<SizeInfo> colWidths, bool isLTR, PageContext pageContext);

			internal abstract void CalculateRHHorizontal(ScalableList<RowInfo> detailRows, int rowIndex, int colIndex, ref List<SizeInfo> colWidths, PageContext pageContext);

			internal abstract void CalculateCHHorizontal(List<SizeInfo> rowHeights, int rowIndex, int colIndex, ScalableList<ColumnInfo> columnInfo, PageContext pageContext, bool onPage);

			internal abstract int AddToPageCHContent(List<SizeInfo> rowHeights, ScalableList<ColumnInfo> columnInfo, int rowIndex, int colIndex, bool isLTR, RPLWriter rplWriter, PageContext pageContext, double pageLeft, double pageTop, double pageRight, double pageBottom, RepeatState repeatState, ref bool hasLabels);

			internal abstract int AddToPageRHContent(List<SizeInfo> colWidths, ScalableList<RowInfo> detailRows, int rowIndex, int colIndex, bool isLTR, RPLWriter rplWriter, PageContext pageContext, double pageLeft, double pageTop, double pageRight, double pageBottom, RepeatState repeatState, ref bool hasLabels);

			internal abstract double NormalizeDetailRowHeight(ScalableList<RowInfo> detailRows, int rowIndex, double startInTablix, double endInTablix, ref int lastRowOnPage, ref int lastRowBelow, bool update, ref double addedHeight);

			internal abstract double NormalizeDetailColWidth(ScalableList<ColumnInfo> detailCols, int colIndex, double startInTablix, double endInTablix, ref int lastColOnPage, ref int lastColBelow, bool update, ref double addedWidth);

			internal abstract void UpdateRows(ScalableList<RowInfo> detailRows, int rowIndex, bool parentKeepRepeatWith, int keepRow, bool delete, double startInTablix, double endInTablix);

			internal abstract void UpdateColumns(ScalableList<ColumnInfo> columnInfo, int colIndex, double startInTablix, double endInTablix);

			internal abstract void Reverse(PageContext pageContext);

			internal virtual PageStructDynamicMemberCell Split(int colsBeforeRowHeaders, PageContext pageContext)
			{
				return null;
			}

			internal void MergePageMembers(PageMemberCell destPageMember, PageMemberCell srcPageMember, TablixRegion region, MergeDetailRows detailRowsState)
			{
				if (destPageMember != null && srcPageMember != null)
				{
					if (destPageMember.MemberItem == null)
					{
						destPageMember.MemberItem = srcPageMember.MemberItem;
					}
					else
					{
						((HiddenPageItem)destPageMember.MemberItem).AddToCollection((HiddenPageItem)srcPageMember.MemberItem);
					}
					if (destPageMember.Children == null)
					{
						detailRowsState.AddRowState(MergeDetailRows.DetailRowState.Merge);
					}
					else
					{
						PageStructMemberCell pageStructMemberCell = null;
						PageStructMemberCell pageStructMemberCell2 = null;
						int num = -1;
						int num2 = 0;
						int num3 = 0;
						int num4 = 0;
						while (true)
						{
							if (num4 >= destPageMember.Children.Count && num3 >= srcPageMember.Children.Count)
							{
								break;
							}
							num = -1;
							if (num4 < destPageMember.Children.Count)
							{
								pageStructMemberCell = destPageMember.Children[num4];
								num = pageStructMemberCell.m_sourceIndex;
							}
							num2 = num + 1;
							if (num3 < srcPageMember.Children.Count)
							{
								pageStructMemberCell2 = srcPageMember.Children[num3];
								num2 = pageStructMemberCell2.m_sourceIndex;
							}
							if (num == num2)
							{
								int span = pageStructMemberCell.Span;
								pageStructMemberCell.MergePageStructMembers(pageStructMemberCell2, region, detailRowsState);
								span = pageStructMemberCell.Span - span;
								if (region == TablixRegion.RowHeader)
								{
									destPageMember.RowSpan += span;
								}
								else
								{
									destPageMember.ColSpan += span;
								}
								this.Span += span;
								num4++;
								num3++;
							}
							else if (num < 0 || num > num2)
							{
								destPageMember.Children.Insert(num4, pageStructMemberCell2);
								if (region == TablixRegion.RowHeader)
								{
									destPageMember.RowSpan += pageStructMemberCell2.Span;
								}
								else
								{
									destPageMember.ColSpan += pageStructMemberCell2.Span;
								}
								this.Span += pageStructMemberCell2.Span;
								num4++;
								num3++;
								for (int i = 0; i < pageStructMemberCell2.Span; i++)
								{
									detailRowsState.AddRowState(MergeDetailRows.DetailRowState.Insert);
								}
							}
							else
							{
								num4++;
								for (int j = 0; j < pageStructMemberCell.Span; j++)
								{
									detailRowsState.AddRowState(MergeDetailRows.DetailRowState.Skip);
								}
							}
						}
					}
				}
			}
		}

		internal class PageStructStaticMemberCell : PageStructMemberCell
		{
			[Flags]
			private enum MemberState : byte
			{
				Clear = 0,
				Header = 1,
				Footer = 2,
				KeepWith = 4,
				RepeatWith = 8,
				StaticTree = 0x10
			}

			private PageMemberCell m_memberInstance;

			private MemberState m_memberState;

			private static Declaration m_declaration = PageStructStaticMemberCell.GetDeclaration();

			internal PageMemberCell MemberInstance
			{
				get
				{
					return this.m_memberInstance;
				}
			}

			internal override int PartialRowSpan
			{
				get
				{
					return 0;
				}
			}

			internal bool Header
			{
				get
				{
					return (int)(this.m_memberState & MemberState.Header) > 0;
				}
			}

			internal bool Footer
			{
				get
				{
					return (int)(this.m_memberState & MemberState.Footer) > 0;
				}
			}

			internal bool KeepWith
			{
				get
				{
					return (int)(this.m_memberState & MemberState.KeepWith) > 0;
				}
				set
				{
					if (value)
					{
						this.m_memberState |= MemberState.KeepWith;
					}
					else
					{
						this.m_memberState &= ~MemberState.KeepWith;
					}
				}
			}

			internal bool RepeatWith
			{
				get
				{
					return (int)(this.m_memberState & MemberState.RepeatWith) > 0;
				}
			}

			internal override bool StaticTree
			{
				get
				{
					return (int)(this.m_memberState & MemberState.StaticTree) > 0;
				}
				set
				{
					if (value)
					{
						this.m_memberState |= MemberState.StaticTree;
					}
					else
					{
						this.m_memberState &= ~MemberState.StaticTree;
					}
				}
			}

			internal override bool Hidden
			{
				get
				{
					if (this.m_memberInstance == null)
					{
						return false;
					}
					return this.m_memberInstance.Hidden;
				}
			}

			internal override double StartPos
			{
				get
				{
					return this.m_memberInstance.StartPos;
				}
			}

			internal override double SizeValue
			{
				get
				{
					return this.m_memberInstance.SizeValue;
				}
			}

			internal override double EndPos
			{
				get
				{
					return this.m_memberInstance.EndPos;
				}
			}

			public override int Size
			{
				get
				{
					return base.Size + 1 + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_memberInstance);
				}
			}

			internal PageStructStaticMemberCell()
			{
			}

			internal PageStructStaticMemberCell(int sourceIndex, bool partialItem, bool createItem, TablixMember member, int memberDefIndex)
				: base(sourceIndex, partialItem, createItem, member, memberDefIndex)
			{
				if (member.KeepWithGroup == KeepWithGroup.After)
				{
					this.m_memberState = MemberState.Header;
				}
				else if (member.KeepWithGroup == KeepWithGroup.Before)
				{
					this.m_memberState = MemberState.Footer;
				}
			}

			public override void Serialize(IntermediateFormatWriter writer)
			{
				base.Serialize(writer);
				writer.RegisterDeclaration(PageStructStaticMemberCell.m_declaration);
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.MemberState:
						writer.Write((byte)this.m_memberState);
						break;
					case MemberName.MemberInstance:
						writer.Write(this.m_memberInstance);
						break;
					default:
						RSTrace.RenderingTracer.Assert(false, string.Empty);
						break;
					}
				}
			}

			public override void Deserialize(IntermediateFormatReader reader)
			{
				base.Deserialize(reader);
				reader.RegisterDeclaration(PageStructStaticMemberCell.m_declaration);
				while (reader.NextMember())
				{
					switch (reader.CurrentMember.MemberName)
					{
					case MemberName.MemberState:
						this.m_memberState = (MemberState)reader.ReadByte();
						break;
					case MemberName.MemberInstance:
						this.m_memberInstance = (PageMemberCell)reader.ReadRIFObject();
						break;
					default:
						RSTrace.RenderingTracer.Assert(false, string.Empty);
						break;
					}
				}
			}

			public override ObjectType GetObjectType()
			{
				return ObjectType.PageStructStaticMemberCell;
			}

			internal new static Declaration GetDeclaration()
			{
				if (PageStructStaticMemberCell.m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.MemberState, Token.Byte));
					list.Add(new MemberInfo(MemberName.MemberInstance, ObjectType.PageMemberCell));
					return new Declaration(ObjectType.PageStructStaticMemberCell, ObjectType.PageStructMemberCell, list);
				}
				return PageStructStaticMemberCell.m_declaration;
			}

			internal void SetKeepWith(bool keepWith, bool repeatWith)
			{
				if (keepWith)
				{
					this.m_memberState |= MemberState.KeepWith;
				}
				if (repeatWith)
				{
					this.m_memberState |= MemberState.RepeatWith;
				}
			}

			internal override IDisposable PartialMemberInstance(out PageMemberCell memberCell)
			{
				memberCell = this.m_memberInstance;
				return null;
			}

			internal override void RemoveLastMemberCell()
			{
				this.m_memberInstance = null;
			}

			internal override void DisposeInstances()
			{
				this.m_memberInstance = null;
			}

			internal override void MergePageStructMembers(PageStructMemberCell mergeStructMember, TablixRegion region, MergeDetailRows detailRowsState)
			{
				PageStructStaticMemberCell pageStructStaticMemberCell = mergeStructMember as PageStructStaticMemberCell;
				if (pageStructStaticMemberCell != null)
				{
					base.MergePageMembers(this.m_memberInstance, pageStructStaticMemberCell.MemberInstance, region, detailRowsState);
				}
			}

			internal override MergeDetailRows AddPageMemberCell(PageMemberCell memberCell, TablixRegion region, PageContext pageContext)
			{
				this.m_memberInstance = memberCell;
				base.HasInstances = true;
				int num = 0;
				if (region == TablixRegion.ColumnHeader)
				{
					base.m_span = memberCell.ColSpan;
					num = memberCell.RowSpan;
				}
				else
				{
					base.m_span = memberCell.RowSpan;
					num = memberCell.ColSpan;
				}
				if (memberCell.Hidden)
				{
					base.OmittedList = false;
				}
				else
				{
					if (num == 0)
					{
						base.HasOmittedChildren = true;
					}
					if (num > 0 || this.StaticTree || memberCell.Label != null || memberCell.Children != null)
					{
						base.OmittedList = false;
					}
					if (memberCell.HasInnerPageName)
					{
						base.HasInnerPageName = true;
					}
				}
				return null;
			}

			internal override void AlignCHToPageVertical(int rowIndex, int targetRowIndex, double startInTablix, double endInTablix, List<SizeInfo> rowHeights, PageContext pageContext)
			{
				if (this.m_memberInstance != null)
				{
					this.m_memberInstance.AlignCHToPageVertical(rowIndex, targetRowIndex, startInTablix, endInTablix, rowHeights, pageContext);
				}
			}

			internal override double NormalizeDetailRowHeight(ScalableList<RowInfo> detailRows, int rowIndex, double startInTablix, double endInTablix, ref int lastRowOnPage, ref int lastRowBelow, bool update, ref double addedHeight)
			{
				if (this.m_memberInstance == null)
				{
					return 0.0;
				}
				return this.m_memberInstance.NormalizeDetailRowHeight(detailRows, rowIndex, startInTablix, endInTablix, ref lastRowOnPage, ref lastRowBelow, update, ref addedHeight);
			}

			internal override void UpdateRows(ScalableList<RowInfo> detailRows, int rowIndex, bool parentKeepRepeatWith, int keepRow, bool delete, double startInTablix, double endInTablix)
			{
				if (this.m_memberInstance != null)
				{
					if (parentKeepRepeatWith && keepRow == 0 && this.RepeatWith)
					{
						keepRow = (this.Header ? 1 : 2);
					}
					this.m_memberInstance.UpdateRows(detailRows, rowIndex, parentKeepRepeatWith, keepRow, delete, startInTablix, endInTablix);
					base.m_span = this.m_memberInstance.RowSpan;
					if (!this.m_memberInstance.HasInnerPageName)
					{
						base.HasInnerPageName = false;
					}
				}
			}

			internal override void UpdateColumns(ScalableList<ColumnInfo> columnInfo, int colIndex, double startInTablix, double endInTablix)
			{
				if (this.m_memberInstance != null && columnInfo[colIndex].PageHorizontalState != ColumnInfo.HorizontalState.AtRight && columnInfo[colIndex + this.m_memberInstance.ColSpan - 1].PageHorizontalState != ColumnInfo.HorizontalState.AtLeft)
				{
					this.m_memberInstance.UpdateColumns(columnInfo, colIndex, startInTablix, endInTablix);
				}
			}

			internal override void AlignRHToPageHorizontal(ScalableList<RowInfo> detailRows, int rowIndex, int colIndex, int targetColIndex, double startInTablix, double endInTablix, List<SizeInfo> colWidths, bool isLTR, PageContext pageContext)
			{
				if (this.m_memberInstance != null)
				{
					this.m_memberInstance.AlignRHToPageHorizontal(detailRows, rowIndex, colIndex, targetColIndex, startInTablix, endInTablix, colWidths, isLTR, pageContext);
				}
			}

			internal override void CalculateRHHorizontal(ScalableList<RowInfo> detailRows, int rowIndex, int colIndex, ref List<SizeInfo> colWidths, PageContext pageContext)
			{
				if (this.m_memberInstance != null)
				{
					this.m_memberInstance.CalculateRHHorizontal(detailRows, rowIndex, colIndex, ref colWidths, pageContext);
				}
			}

			internal override int AddToPageRHContent(List<SizeInfo> colWidths, ScalableList<RowInfo> detailRows, int rowIndex, int colIndex, bool isLTR, RPLWriter rplWriter, PageContext pageContext, double pageLeft, double pageTop, double pageRight, double pageBottom, RepeatState repeatState, ref bool hasLabels)
			{
				if (this.m_memberInstance == null)
				{
					return 0;
				}
				return this.m_memberInstance.AddToPageRHContent(colWidths, detailRows, rowIndex, colIndex, isLTR, rplWriter, pageContext, pageLeft, pageTop, pageRight, pageBottom, repeatState, ref hasLabels);
			}

			internal override void CalculateCHHorizontal(List<SizeInfo> rowHeights, int rowIndex, int colIndex, ScalableList<ColumnInfo> columnInfo, PageContext pageContext, bool onPage)
			{
				if (this.m_memberInstance != null)
				{
					this.m_memberInstance.CalculateCHHorizontal(rowHeights, rowIndex, colIndex, columnInfo, pageContext, onPage);
				}
			}

			internal override int AddToPageCHContent(List<SizeInfo> rowHeights, ScalableList<ColumnInfo> columnInfo, int rowIndex, int colIndex, bool isLTR, RPLWriter rplWriter, PageContext pageContext, double pageLeft, double pageTop, double pageRight, double pageBottom, RepeatState repeatState, ref bool hasLabels)
			{
				if (this.m_memberInstance == null)
				{
					return 0;
				}
				if (columnInfo[colIndex].PageHorizontalState == ColumnInfo.HorizontalState.AtRight)
				{
					return 0;
				}
				if (columnInfo[colIndex + this.m_memberInstance.ColSpan - 1].PageHorizontalState == ColumnInfo.HorizontalState.AtLeft)
				{
					return 0;
				}
				return this.m_memberInstance.AddToPageCHContent(rowHeights, columnInfo, rowIndex, colIndex, isLTR, rplWriter, pageContext, pageLeft, pageTop, pageRight, pageBottom, repeatState, ref hasLabels);
			}

			internal override double NormalizeDetailColWidth(ScalableList<ColumnInfo> detailCols, int colIndex, double startInTablix, double endInTablix, ref int lastColOnPage, ref int lastColBelow, bool update, ref double addedWidth)
			{
				if (this.m_memberInstance == null)
				{
					return 0.0;
				}
				if (detailCols[colIndex + this.m_memberInstance.ColSpan - 1].PageHorizontalState == ColumnInfo.HorizontalState.AtLeft)
				{
					return 0.0;
				}
				return this.m_memberInstance.NormalizeDetailColWidth(detailCols, colIndex, startInTablix, endInTablix, ref lastColOnPage, ref lastColBelow, update, ref addedWidth);
			}

			internal override void Reverse(PageContext pageContext)
			{
				if (this.m_memberInstance != null)
				{
					this.m_memberInstance.Reverse(pageContext);
				}
			}

			internal override bool ResolveCHDuplicates(int rowIndex, int targetRowIndex, double startInTablix, List<SizeInfo> rowHeights, PageContext pageContext)
			{
				if (this.m_memberInstance == null)
				{
					return false;
				}
				return this.m_memberInstance.ResolveCHDuplicates(rowIndex, targetRowIndex, startInTablix, rowHeights, pageContext);
			}

			internal override bool ResolveRHDuplicates(int rowIndex, double startInTablix, ScalableList<RowInfo> detailRows, PageContext pageContext)
			{
				if (this.m_memberInstance == null)
				{
					return false;
				}
				return this.m_memberInstance.ResolveRHDuplicates(rowIndex, startInTablix, detailRows, pageContext);
			}
		}

		internal class PageStructDynamicMemberCell : PageStructMemberCell
		{
			private ScalableList<PageMemberCell> m_memberInstances;

			private static Declaration m_declaration = PageStructDynamicMemberCell.GetDeclaration();

			internal ScalableList<PageMemberCell> MemberInstances
			{
				get
				{
					return this.m_memberInstances;
				}
			}

			internal override bool Hidden
			{
				get
				{
					if (this.m_memberInstances != null && this.m_memberInstances.Count != 0)
					{
						for (int i = 0; i < this.m_memberInstances.Count; i++)
						{
							if (!this.m_memberInstances[i].Hidden)
							{
								return false;
							}
						}
						return true;
					}
					return false;
				}
			}

			internal override double StartPos
			{
				get
				{
					return this.m_memberInstances[0].StartPos;
				}
			}

			internal override double SizeValue
			{
				get
				{
					return this.EndPos - this.StartPos;
				}
			}

			internal override double EndPos
			{
				get
				{
					return this.m_memberInstances[this.m_memberInstances.Count - 1].EndPos;
				}
			}

			internal override int PartialRowSpan
			{
				get
				{
					if (this.m_memberInstances != null && this.m_memberInstances.Count != 0)
					{
						int num = 0;
						for (int i = 0; i < this.m_memberInstances.Count - 1; i++)
						{
							num += this.m_memberInstances[i].RowSpan;
						}
						return num;
					}
					return 0;
				}
			}

			public override int Size
			{
				get
				{
					return base.Size + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_memberInstances);
				}
			}

			internal PageStructDynamicMemberCell()
			{
			}

			internal PageStructDynamicMemberCell(int sourceIndex, bool partialItem, bool createItem, TablixMember member, int memberDefIndex)
				: base(sourceIndex, partialItem, createItem, member, memberDefIndex)
			{
			}

			internal PageStructDynamicMemberCell(PageStructDynamicMemberCell copy, ScalableList<PageMemberCell> memberInstances, int span)
				: base(copy, span)
			{
				this.m_memberInstances = memberInstances;
			}

			public override void Serialize(IntermediateFormatWriter writer)
			{
				base.Serialize(writer);
				writer.RegisterDeclaration(PageStructDynamicMemberCell.m_declaration);
				while (writer.NextMember())
				{
					MemberName memberName = writer.CurrentMember.MemberName;
					if (memberName == MemberName.MemberInstances)
					{
						writer.Write(this.m_memberInstances);
					}
					else
					{
						RSTrace.RenderingTracer.Assert(false, string.Empty);
					}
				}
			}

			public override void Deserialize(IntermediateFormatReader reader)
			{
				base.Deserialize(reader);
				reader.RegisterDeclaration(PageStructDynamicMemberCell.m_declaration);
				while (reader.NextMember())
				{
					MemberName memberName = reader.CurrentMember.MemberName;
					if (memberName == MemberName.MemberInstances)
					{
						this.m_memberInstances = reader.ReadRIFObject<ScalableList<PageMemberCell>>();
					}
					else
					{
						RSTrace.RenderingTracer.Assert(false, string.Empty);
					}
				}
			}

			public override ObjectType GetObjectType()
			{
				return ObjectType.PageStructDynamicMemberCell;
			}

			internal new static Declaration GetDeclaration()
			{
				if (PageStructDynamicMemberCell.m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.MemberInstances, ObjectType.ScalableList, ObjectType.PageMemberCell));
					return new Declaration(ObjectType.PageStructDynamicMemberCell, ObjectType.PageStructMemberCell, list);
				}
				return PageStructDynamicMemberCell.m_declaration;
			}

			internal override IDisposable PartialMemberInstance(out PageMemberCell memberCell)
			{
				memberCell = null;
				if (this.m_memberInstances != null && this.m_memberInstances.Count != 0)
				{
					return this.m_memberInstances.GetAndPin(this.m_memberInstances.Count - 1, out memberCell);
				}
				return null;
			}

			internal override void RemoveLastMemberCell()
			{
				if (this.m_memberInstances != null && this.m_memberInstances.Count != 0)
				{
					this.m_memberInstances.RemoveAt(this.m_memberInstances.Count - 1);
				}
			}

			internal override void DisposeInstances()
			{
				if (this.m_memberInstances != null)
				{
					this.m_memberInstances.Dispose();
					this.m_memberInstances = null;
				}
			}

			internal override void MergePageStructMembers(PageStructMemberCell mergeStructMember, TablixRegion region, MergeDetailRows detailRowsState)
			{
				PageStructDynamicMemberCell pageStructDynamicMemberCell = mergeStructMember as PageStructDynamicMemberCell;
				if (pageStructDynamicMemberCell != null)
				{
					PageMemberCell destPageMember = null;
					IDisposable andPin = this.m_memberInstances.GetAndPin(0, out destPageMember);
					base.MergePageMembers(destPageMember, pageStructDynamicMemberCell.MemberInstances[0], region, detailRowsState);
					andPin.Dispose();
				}
			}

			internal override MergeDetailRows AddPageMemberCell(PageMemberCell memberCell, TablixRegion region, PageContext pageContext)
			{
				if (this.m_memberInstances == null)
				{
					this.m_memberInstances = new ScalableList<PageMemberCell>(0, pageContext.ScalabilityCache);
					base.HasInstances = true;
				}
				if (memberCell.Hidden)
				{
					PageMemberCell pageMemberCell = null;
					if (this.m_memberInstances.Count > 0)
					{
						using (this.m_memberInstances.GetAndPin(this.m_memberInstances.Count - 1, out pageMemberCell))
						{
							if (pageMemberCell != null && pageMemberCell.Hidden)
							{
								MergeDetailRows mergeDetailRows = null;
								mergeDetailRows = ((region != TablixRegion.RowHeader) ? new MergeDetailRows(pageMemberCell.ColSpan) : new MergeDetailRows(pageMemberCell.RowSpan));
								base.MergePageMembers(pageMemberCell, memberCell, region, mergeDetailRows);
								return mergeDetailRows;
							}
						}
					}
				}
				this.m_memberInstances.Add(memberCell);
				int num = 0;
				if (region == TablixRegion.ColumnHeader)
				{
					base.m_span += memberCell.ColSpan;
					num = memberCell.RowSpan;
				}
				else
				{
					base.m_span += memberCell.RowSpan;
					num = memberCell.ColSpan;
				}
				if (memberCell.Hidden)
				{
					base.OmittedList = false;
				}
				else
				{
					if (num == 0)
					{
						base.HasOmittedChildren = true;
					}
					if (num > 0 || memberCell.Label != null || memberCell.Children != null)
					{
						base.OmittedList = false;
					}
					if (memberCell.HasInnerPageName || memberCell.PageName != null)
					{
						base.HasInnerPageName = true;
					}
				}
				return null;
			}

			internal override void AlignCHToPageVertical(int rowIndex, int targetRowIndex, double startInTablix, double endInTablix, List<SizeInfo> rowHeights, PageContext pageContext)
			{
				if (this.m_memberInstances != null)
				{
					PageMemberCell pageMemberCell = null;
					for (int i = 0; i < this.m_memberInstances.Count; i++)
					{
						using (this.m_memberInstances.GetAndPin(i, out pageMemberCell))
						{
							pageMemberCell.AlignCHToPageVertical(rowIndex, targetRowIndex, startInTablix, endInTablix, rowHeights, pageContext);
						}
					}
				}
			}

			internal override double NormalizeDetailRowHeight(ScalableList<RowInfo> detailRows, int rowIndex, double startInTablix, double endInTablix, ref int lastRowOnPage, ref int lastRowBelow, bool update, ref double addedHeight)
			{
				if (this.m_memberInstances == null)
				{
					return 0.0;
				}
				double num = 0.0;
				PageMemberCell pageMemberCell = null;
				for (int i = 0; i < this.m_memberInstances.Count; i++)
				{
					using (this.m_memberInstances.GetAndPin(i, out pageMemberCell))
					{
						num += pageMemberCell.NormalizeDetailRowHeight(detailRows, rowIndex, startInTablix, endInTablix, ref lastRowOnPage, ref lastRowBelow, update, ref addedHeight);
						rowIndex += pageMemberCell.RowSpan;
					}
				}
				return num;
			}

			internal override void UpdateRows(ScalableList<RowInfo> detailRows, int rowIndex, bool parentKeepRepeatWith, int keepRow, bool delete, double startInTablix, double endInTablix)
			{
				if (this.m_memberInstances != null && this.m_memberInstances.Count != 0)
				{
					base.m_span = 0;
					int num = -1;
					int num2 = -1;
					PageMemberCell pageMemberCell = null;
					for (int i = 0; i < this.m_memberInstances.Count; i++)
					{
						using (this.m_memberInstances.GetAndPin(i, out pageMemberCell))
						{
							pageMemberCell.UpdateRows(detailRows, rowIndex, parentKeepRepeatWith, 0, delete, startInTablix, endInTablix);
						}
						pageMemberCell = this.m_memberInstances[i];
						if (pageMemberCell.RowSpan == 0)
						{
							if (num < 0)
							{
								num = i;
							}
							num2 = i;
						}
						else if (num >= 0)
						{
							int num3 = num2 - num + 1;
							this.m_memberInstances.RemoveRange(num, num3);
							i -= num3;
							num = (num2 = -1);
						}
						rowIndex += pageMemberCell.RowSpan;
						base.m_span += pageMemberCell.RowSpan;
					}
					if (num >= 0)
					{
						if (base.PartialItem && !base.CreateItem)
						{
							num2--;
						}
						if (num2 >= num)
						{
							int count = num2 - num + 1;
							this.m_memberInstances.RemoveRange(num, count);
						}
					}
					if (base.HasInnerPageName)
					{
						for (int j = 0; j < this.m_memberInstances.Count; j++)
						{
							pageMemberCell = this.m_memberInstances[j];
							if (pageMemberCell.PageName != null || pageMemberCell.HasInnerPageName)
							{
								return;
							}
						}
						base.HasInnerPageName = false;
					}
				}
			}

			internal override void AlignRHToPageHorizontal(ScalableList<RowInfo> detailRows, int rowIndex, int colIndex, int targetColIndex, double startInTablix, double endInTablix, List<SizeInfo> colWidths, bool isLTR, PageContext pageContext)
			{
				if (this.m_memberInstances != null)
				{
					PageMemberCell pageMemberCell = null;
					for (int i = 0; i < this.m_memberInstances.Count; i++)
					{
						if (detailRows[rowIndex].PageVerticalState == RowInfo.VerticalState.Below)
						{
							break;
						}
						using (this.m_memberInstances.GetAndPin(i, out pageMemberCell))
						{
							pageMemberCell.AlignRHToPageHorizontal(detailRows, rowIndex, colIndex, targetColIndex, startInTablix, endInTablix, colWidths, isLTR, pageContext);
							rowIndex += pageMemberCell.RowSpan;
						}
					}
				}
			}

			internal override void CalculateRHHorizontal(ScalableList<RowInfo> detailRows, int rowIndex, int colIndex, ref List<SizeInfo> colWidths, PageContext pageContext)
			{
				if (this.m_memberInstances != null)
				{
					PageMemberCell pageMemberCell = null;
					for (int i = 0; i < this.m_memberInstances.Count; i++)
					{
						if (detailRows[rowIndex].PageVerticalState == RowInfo.VerticalState.Below)
						{
							break;
						}
						using (this.m_memberInstances.GetAndPin(i, out pageMemberCell))
						{
							pageMemberCell.CalculateRHHorizontal(detailRows, rowIndex, colIndex, ref colWidths, pageContext);
							rowIndex += pageMemberCell.RowSpan;
						}
					}
				}
			}

			internal override int AddToPageRHContent(List<SizeInfo> colWidths, ScalableList<RowInfo> detailRows, int rowIndex, int colIndex, bool isLTR, RPLWriter rplWriter, PageContext pageContext, double pageLeft, double pageTop, double pageRight, double pageBottom, RepeatState repeatState, ref bool hasLabels)
			{
				if (this.m_memberInstances == null)
				{
					return 0;
				}
				int num = 0;
				PageMemberCell pageMemberCell = null;
				for (int i = 0; i < this.m_memberInstances.Count; i++)
				{
					if (detailRows[rowIndex].PageVerticalState == RowInfo.VerticalState.Below)
					{
						break;
					}
					using (this.m_memberInstances.GetAndPin(i, out pageMemberCell))
					{
						num += pageMemberCell.AddToPageRHContent(colWidths, detailRows, rowIndex, colIndex, isLTR, rplWriter, pageContext, pageLeft, pageTop, pageRight, pageBottom, repeatState, ref hasLabels);
						rowIndex += pageMemberCell.RowSpan;
					}
				}
				return num;
			}

			internal override void CalculateCHHorizontal(List<SizeInfo> rowHeights, int rowIndex, int colIndex, ScalableList<ColumnInfo> colWidths, PageContext pageContext, bool onPage)
			{
				if (this.m_memberInstances != null)
				{
					PageMemberCell pageMemberCell = null;
					for (int i = 0; i < this.m_memberInstances.Count; i++)
					{
						using (this.m_memberInstances.GetAndPin(i, out pageMemberCell))
						{
							pageMemberCell.CalculateCHHorizontal(rowHeights, rowIndex, colIndex, colWidths, pageContext, onPage);
							colIndex += pageMemberCell.ColSpan;
						}
					}
				}
			}

			internal override int AddToPageCHContent(List<SizeInfo> rowHeights, ScalableList<ColumnInfo> columnInfo, int rowIndex, int colIndex, bool isLTR, RPLWriter rplWriter, PageContext pageContext, double pageLeft, double pageTop, double pageRight, double pageBottom, RepeatState repeatState, ref bool hasLabels)
			{
				if (this.m_memberInstances == null)
				{
					return 0;
				}
				int num = 0;
				int num2 = colIndex;
				int num3 = 0;
				RTLTextBoxes delayedTB = null;
				if (rplWriter != null)
				{
					rplWriter.EnterDelayedTBLevel(isLTR, ref delayedTB);
				}
				PageMemberCell pageMemberCell = null;
				for (int i = 0; i < this.m_memberInstances.Count; i++)
				{
					using (this.m_memberInstances.GetAndPin(i, out pageMemberCell))
					{
						num3 = num2;
						num2 += pageMemberCell.ColSpan;
						if (columnInfo[num3].PageHorizontalState != ColumnInfo.HorizontalState.AtRight)
						{
							if (columnInfo[num2 - 1].PageHorizontalState != ColumnInfo.HorizontalState.AtLeft)
							{
								num += pageMemberCell.AddToPageCHContent(rowHeights, columnInfo, rowIndex, num3, isLTR, rplWriter, pageContext, pageLeft, pageTop, pageRight, pageBottom, repeatState, ref hasLabels);
								goto IL_009c;
							}
							continue;
						}
					}
					break;
					IL_009c:
					if (rplWriter != null)
					{
						rplWriter.RegisterCellTextBoxes(isLTR, delayedTB);
					}
				}
				if (rplWriter != null)
				{
					rplWriter.LeaveDelayedTBLevel(isLTR, delayedTB, pageContext);
				}
				return num;
			}

			internal override void UpdateColumns(ScalableList<ColumnInfo> columnInfo, int colIndex, double startInTablix, double endInTablix)
			{
				if (this.m_memberInstances != null)
				{
					int num = colIndex;
					int num2 = 0;
					PageMemberCell pageMemberCell = null;
					for (int i = 0; i < this.m_memberInstances.Count; i++)
					{
						using (this.m_memberInstances.GetAndPin(i, out pageMemberCell))
						{
							num2 = num;
							num += pageMemberCell.ColSpan;
							if (columnInfo[num2].PageHorizontalState != ColumnInfo.HorizontalState.AtRight)
							{
								if (columnInfo[num - 1].PageHorizontalState != ColumnInfo.HorizontalState.AtLeft)
								{
									pageMemberCell.UpdateColumns(columnInfo, num2, startInTablix, endInTablix);
								}
								goto end_IL_0023;
							}
							return;
							end_IL_0023:;
						}
					}
				}
			}

			internal override double NormalizeDetailColWidth(ScalableList<ColumnInfo> detailCols, int colIndex, double startInTablix, double endInTablix, ref int lastColOnPage, ref int lastColBelow, bool update, ref double addedWidth)
			{
				if (this.m_memberInstances == null)
				{
					return 0.0;
				}
				double num = 0.0;
				int num2 = colIndex;
				int num3 = 0;
				PageMemberCell pageMemberCell = null;
				for (int i = 0; i < this.m_memberInstances.Count; i++)
				{
					num3 = num2;
					num2 += this.m_memberInstances[i].ColSpan;
					if (detailCols[num2 - 1].PageHorizontalState != ColumnInfo.HorizontalState.AtLeft)
					{
						using (this.m_memberInstances.GetAndPin(i, out pageMemberCell))
						{
							num += pageMemberCell.NormalizeDetailColWidth(detailCols, num3, startInTablix, endInTablix, ref lastColOnPage, ref lastColBelow, update, ref addedWidth);
						}
					}
				}
				return num;
			}

			internal override void Reverse(PageContext pageContext)
			{
				if (this.m_memberInstances != null)
				{
					ScalableList<PageMemberCell> scalableList = new ScalableList<PageMemberCell>(0, pageContext.ScalabilityCache);
					for (int num = this.m_memberInstances.Count - 1; num >= 0; num--)
					{
						scalableList.Add(this.m_memberInstances[num]);
					}
					this.m_memberInstances = scalableList;
					PageMemberCell pageMemberCell = null;
					for (int i = 0; i < this.m_memberInstances.Count; i++)
					{
						using (this.m_memberInstances.GetAndPin(i, out pageMemberCell))
						{
							pageMemberCell.Reverse(pageContext);
						}
					}
				}
			}

			internal override PageStructDynamicMemberCell Split(int colsBeforeRowHeaders, PageContext pageContext)
			{
				if (colsBeforeRowHeaders != 0 && this.m_memberInstances != null)
				{
					ScalableList<PageMemberCell> scalableList = new ScalableList<PageMemberCell>(0, pageContext.ScalabilityCache);
					int num = 0;
					int num2 = 0;
					PageMemberCell pageMemberCell = null;
					while (num2 < colsBeforeRowHeaders)
					{
						pageMemberCell = this.m_memberInstances[num];
						num2 += pageMemberCell.ColSpan;
						scalableList.Add(pageMemberCell);
						num++;
					}
					if (num2 == base.m_span)
					{
						return null;
					}
					base.m_span -= num2;
					this.m_memberInstances.RemoveRange(0, num);
					return new PageStructDynamicMemberCell(this, scalableList, num2);
				}
				return null;
			}

			internal override bool ResolveCHDuplicates(int rowIndex, int targetRowIndex, double startInTablix, List<SizeInfo> rowHeights, PageContext pageContext)
			{
				if (this.m_memberInstances == null)
				{
					return false;
				}
				PageMemberCell pageMemberCell = null;
				bool flag = false;
				for (int i = 0; i < this.m_memberInstances.Count; i++)
				{
					using (this.m_memberInstances.GetAndPin(i, out pageMemberCell))
					{
						flag |= pageMemberCell.ResolveCHDuplicates(rowIndex, targetRowIndex, startInTablix, rowHeights, pageContext);
					}
				}
				return flag;
			}

			internal override bool ResolveRHDuplicates(int rowIndex, double startInTablix, ScalableList<RowInfo> detailRows, PageContext pageContext)
			{
				if (this.m_memberInstances == null)
				{
					return false;
				}
				PageMemberCell pageMemberCell = null;
				bool flag = false;
				for (int i = 0; i < this.m_memberInstances.Count; i++)
				{
					using (this.m_memberInstances.GetAndPin(i, out pageMemberCell))
					{
						flag |= pageMemberCell.ResolveRHDuplicates(rowIndex, startInTablix, detailRows, pageContext);
						rowIndex += pageMemberCell.RowSpan;
					}
				}
				return flag;
			}
		}

		internal abstract class PageTalixCell : IStorable, IPersistable
		{
			internal PageItem m_cellItem;

			internal int m_colSpan;

			private static Declaration m_declaration = PageTalixCell.GetDeclaration();

			internal int ColSpan
			{
				get
				{
					return this.m_colSpan;
				}
				set
				{
					this.m_colSpan = value;
				}
			}

			internal PageItem CellItem
			{
				get
				{
					return this.m_cellItem;
				}
				set
				{
					this.m_cellItem = value;
				}
			}

			public virtual int Size
			{
				get
				{
					return 4 + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_cellItem);
				}
			}

			internal PageTalixCell()
			{
			}

			internal PageTalixCell(PageItem item, int colSpan)
			{
				this.m_cellItem = item;
				this.m_colSpan = colSpan;
			}

			public virtual void Serialize(IntermediateFormatWriter writer)
			{
				writer.RegisterDeclaration(PageTalixCell.m_declaration);
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.ColSpan:
						writer.Write(this.m_colSpan);
						break;
					case MemberName.CellItem:
						writer.Write(this.m_cellItem);
						break;
					default:
						RSTrace.RenderingTracer.Assert(false, string.Empty);
						break;
					}
				}
			}

			public virtual void Deserialize(IntermediateFormatReader reader)
			{
				reader.RegisterDeclaration(PageTalixCell.m_declaration);
				while (reader.NextMember())
				{
					switch (reader.CurrentMember.MemberName)
					{
					case MemberName.ColSpan:
						this.m_colSpan = reader.ReadInt32();
						break;
					case MemberName.CellItem:
						this.m_cellItem = (PageItem)reader.ReadRIFObject();
						break;
					default:
						RSTrace.RenderingTracer.Assert(false, string.Empty);
						break;
					}
				}
			}

			public void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
			{
			}

			public virtual ObjectType GetObjectType()
			{
				return ObjectType.PageTablixCell;
			}

			internal static Declaration GetDeclaration()
			{
				if (PageTalixCell.m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.ColSpan, Token.Int32));
					list.Add(new MemberInfo(MemberName.CellItem, ObjectType.PageItem));
					return new Declaration(ObjectType.PageTablixCell, ObjectType.None, list);
				}
				return PageTalixCell.m_declaration;
			}
		}

		internal class PageDetailCell : PageTalixCell
		{
			[Flags]
			private enum PageDetailCellState : byte
			{
				Clear = 0,
				Hidden = 1,
				Release = 2,
				ContentOnPage = 4,
				KeepTogether = 8
			}

			internal double m_sourceWidth;

			private PageDetailCellState m_state;

			private static Declaration m_declaration = PageDetailCell.GetDeclaration();

			internal double SourceWidth
			{
				get
				{
					return this.m_sourceWidth;
				}
				set
				{
					this.m_sourceWidth = value;
				}
			}

			internal bool Hidden
			{
				get
				{
					return (int)(this.m_state & PageDetailCellState.Hidden) > 0;
				}
				set
				{
					if (value)
					{
						this.m_state |= PageDetailCellState.Hidden;
					}
					else
					{
						this.m_state &= ~PageDetailCellState.Hidden;
					}
				}
			}

			internal bool Release
			{
				get
				{
					return (int)(this.m_state & PageDetailCellState.Release) > 0;
				}
				set
				{
					if (value)
					{
						this.m_state |= PageDetailCellState.Release;
					}
					else
					{
						this.m_state &= ~PageDetailCellState.Release;
					}
				}
			}

			internal bool ContentOnPage
			{
				get
				{
					return (int)(this.m_state & PageDetailCellState.ContentOnPage) > 0;
				}
				set
				{
					if (value)
					{
						this.m_state |= PageDetailCellState.ContentOnPage;
					}
					else
					{
						this.m_state &= ~PageDetailCellState.ContentOnPage;
					}
				}
			}

			internal bool KeepTogether
			{
				get
				{
					return (int)(this.m_state & PageDetailCellState.KeepTogether) > 0;
				}
				set
				{
					if (value)
					{
						this.m_state |= PageDetailCellState.KeepTogether;
					}
					else
					{
						this.m_state &= ~PageDetailCellState.KeepTogether;
					}
				}
			}

			public override int Size
			{
				get
				{
					return base.Size + 1 + 8;
				}
			}

			internal PageDetailCell()
			{
			}

			internal PageDetailCell(PageItem item, double sourceWidth)
				: base(item, 1)
			{
				this.m_sourceWidth = sourceWidth;
			}

			public override void Serialize(IntermediateFormatWriter writer)
			{
				base.Serialize(writer);
				writer.RegisterDeclaration(PageDetailCell.m_declaration);
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.State:
						writer.Write((byte)this.m_state);
						break;
					case MemberName.Width:
						writer.Write(this.m_sourceWidth);
						break;
					default:
						RSTrace.RenderingTracer.Assert(false, string.Empty);
						break;
					}
				}
			}

			public override void Deserialize(IntermediateFormatReader reader)
			{
				base.Deserialize(reader);
				reader.RegisterDeclaration(PageDetailCell.m_declaration);
				while (reader.NextMember())
				{
					switch (reader.CurrentMember.MemberName)
					{
					case MemberName.State:
						this.m_state = (PageDetailCellState)reader.ReadByte();
						break;
					case MemberName.Width:
						this.m_sourceWidth = reader.ReadDouble();
						break;
					default:
						RSTrace.RenderingTracer.Assert(false, string.Empty);
						break;
					}
				}
			}

			public override ObjectType GetObjectType()
			{
				return ObjectType.PageDetailCell;
			}

			internal new static Declaration GetDeclaration()
			{
				if (PageDetailCell.m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.State, Token.Byte));
					list.Add(new MemberInfo(MemberName.Width, Token.Double));
					return new Declaration(ObjectType.PageDetailCell, ObjectType.PageTablixCell, list);
				}
				return PageDetailCell.m_declaration;
			}
		}

		internal class PageCornerCell : PageTalixCell
		{
			internal double m_sourceWidth;

			internal int m_rowSpan;

			internal bool m_contentOnPage;

			private static Declaration m_declaration = PageCornerCell.GetDeclaration();

			internal int RowSpan
			{
				get
				{
					return this.m_rowSpan;
				}
			}

			internal bool ContentOnPage
			{
				get
				{
					return this.m_contentOnPage;
				}
			}

			public override int Size
			{
				get
				{
					return base.Size + 1 + 8 + 4;
				}
			}

			internal PageCornerCell()
			{
			}

			internal PageCornerCell(PageItem item, int rowSpan, int colSpan, double sourceWidth, double sourceHeight)
				: base(item, colSpan)
			{
				this.m_sourceWidth = sourceWidth;
				this.m_rowSpan = rowSpan;
			}

			public override void Serialize(IntermediateFormatWriter writer)
			{
				base.Serialize(writer);
				writer.RegisterDeclaration(PageCornerCell.m_declaration);
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.ContentOnPage:
						writer.Write(this.m_contentOnPage);
						break;
					case MemberName.RowSpan:
						writer.Write(this.m_rowSpan);
						break;
					case MemberName.Width:
						writer.Write(this.m_sourceWidth);
						break;
					default:
						RSTrace.RenderingTracer.Assert(false, string.Empty);
						break;
					}
				}
			}

			public override void Deserialize(IntermediateFormatReader reader)
			{
				base.Deserialize(reader);
				reader.RegisterDeclaration(PageCornerCell.m_declaration);
				while (reader.NextMember())
				{
					switch (reader.CurrentMember.MemberName)
					{
					case MemberName.ContentOnPage:
						this.m_contentOnPage = reader.ReadBoolean();
						break;
					case MemberName.RowSpan:
						this.m_rowSpan = reader.ReadInt32();
						break;
					case MemberName.Width:
						this.m_sourceWidth = reader.ReadDouble();
						break;
					default:
						RSTrace.RenderingTracer.Assert(false, string.Empty);
						break;
					}
				}
			}

			public override ObjectType GetObjectType()
			{
				return ObjectType.PageCornerCell;
			}

			internal new static Declaration GetDeclaration()
			{
				if (PageCornerCell.m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.ContentOnPage, Token.Boolean));
					list.Add(new MemberInfo(MemberName.RowSpan, Token.Int32));
					list.Add(new MemberInfo(MemberName.Width, Token.Double));
					return new Declaration(ObjectType.PageCornerCell, ObjectType.PageTablixCell, list);
				}
				return PageCornerCell.m_declaration;
			}

			internal void CalculateHorizontal(int colIndex, ref List<SizeInfo> colWidths, PageContext context)
			{
				if (base.m_cellItem != null)
				{
					bool flag = false;
					PageContext pageContext = new PageContext(context);
					base.m_cellItem.CalculateHorizontal(pageContext, 0.0, 1.7976931348623157E+308, null, new List<PageItem>(), ref flag, true, this.m_sourceWidth);
					double size = Math.Max(this.m_sourceWidth, base.m_cellItem.ItemPageSizes.Width);
					Tablix.UpdateSizes(colIndex, base.m_colSpan, size, ref colWidths);
				}
			}

			internal void AlignToPageVertical(int rowIndex, int targetRowIndex, double startInTablix, double endInTablix, List<SizeInfo> rowHeights, PageContext pageContext)
			{
				if (base.m_cellItem != null && (base.m_cellItem.KTVIsUnresolved || base.m_cellItem.NeedResolve))
				{
					double num = 0.0;
					for (int i = rowIndex; i < targetRowIndex; i++)
					{
						num += rowHeights[i].SizeValue;
					}
					double topInParentSystem = Math.Max(0.0, startInTablix - rowHeights[rowIndex].StartPos);
					double bottomInParentSystem = endInTablix - rowHeights[rowIndex].StartPos;
					PageContext pageContext2 = pageContext;
					if (!pageContext.IgnorePageBreaks)
					{
						pageContext2 = new PageContext(pageContext);
						pageContext2.IgnorePageBreaks = true;
						pageContext2.IgnorePageBreaksReason = PageContext.IgnorePageBreakReason.InsideTablixCell;
					}
					base.m_cellItem.ResolveVertical(pageContext2, topInParentSystem, bottomInParentSystem, null, true, pageContext.Common.CanOverwritePageBreak, pageContext.Common.CanSetPageName);
					num = base.m_cellItem.ItemPageSizes.Bottom - num;
					if (num > 0.0)
					{
						SizeInfo.UpdateSize(rowHeights[targetRowIndex], rowIndex + this.m_rowSpan - targetRowIndex, num);
					}
				}
			}

			internal bool ResolveDuplicates(int rowIndex, int targetRowIndex, double startInTablix, List<SizeInfo> rowHeights, PageContext pageContext)
			{
				if (base.m_cellItem == null)
				{
					return false;
				}
				double topInParentSystem = Math.Max(0.0, startInTablix - rowHeights[rowIndex].StartPos);
				bool flag = base.m_cellItem.ResolveDuplicates(pageContext, topInParentSystem, null, false);
				if (flag)
				{
					double num = 0.0;
					for (int i = rowIndex; i < targetRowIndex; i++)
					{
						num += rowHeights[i].SizeValue;
					}
					num = base.m_cellItem.ItemPageSizes.Bottom - num;
					if (num > 0.0)
					{
						SizeInfo.UpdateSize(rowHeights[targetRowIndex], rowIndex + this.m_rowSpan - targetRowIndex, num);
					}
				}
				return flag;
			}

			internal void AlignToPageHorizontal(int colIndex, int targetColIndex, double startInTablix, double endInTablix, List<SizeInfo> colWidths, bool isLTR, PageContext pageContext)
			{
				if (base.m_cellItem != null && (base.m_cellItem.KTHIsUnresolved || base.m_cellItem.NeedResolve))
				{
					double num = 0.0;
					double num2 = 0.0;
					if (isLTR)
					{
						num2 = colWidths[colIndex].StartPos;
						for (int i = colIndex; i < targetColIndex; i++)
						{
							num += colWidths[i].SizeValue;
						}
					}
					else
					{
						num2 = colWidths[colIndex + base.m_colSpan - 1].StartPos;
						for (int num3 = colIndex + base.m_colSpan - 1; num3 > targetColIndex; num3--)
						{
							num += colWidths[num3].SizeValue;
						}
					}
					double leftInParentSystem = Math.Max(0.0, startInTablix - num2);
					double rightInParentSystem = endInTablix - num2;
					base.m_cellItem.ResolveHorizontal(pageContext, leftInParentSystem, rightInParentSystem, null, true);
					num = base.m_cellItem.ItemPageSizes.Right - num;
					if (num > 0.0)
					{
						if (isLTR)
						{
							SizeInfo.UpdateSize(colWidths[targetColIndex], colIndex + base.m_colSpan - targetColIndex, num);
						}
						else
						{
							SizeInfo.UpdateSize(colWidths[targetColIndex], targetColIndex - colIndex + 1, num);
						}
					}
				}
			}

			internal void AddToPageContent(List<SizeInfo> rowHeights, List<SizeInfo> colWidths, int rowIndex, int colIndex, bool isLTR, RPLWriter rplWriter, PageContext pageContext, double pageLeft, double pageTop, double pageRight, double pageBottom, RepeatState repeatState)
			{
				if (base.m_cellItem != null)
				{
					double startPos = rowHeights[rowIndex].StartPos;
					double pageTop2 = Math.Max(0.0, pageTop - startPos);
					double pageBottom2 = pageBottom - startPos;
					double num = 0.0;
					num = ((!isLTR) ? colWidths[colIndex + base.m_colSpan - 1].StartPos : colWidths[colIndex].StartPos);
					double pageLeft2 = Math.Max(0.0, pageLeft - num);
					double pageRight2 = pageRight - num;
					base.m_cellItem.ItemPageSizes.AdjustHeightTo(rowHeights[rowIndex + this.m_rowSpan - 1].EndPos - startPos);
					this.m_contentOnPage = base.m_cellItem.AddToPage(rplWriter, pageContext, pageLeft2, pageTop2, pageRight2, pageBottom2, repeatState);
					if (this.m_contentOnPage)
					{
						this.m_contentOnPage = base.m_cellItem.ContentOnPage;
					}
				}
			}
		}

		internal class RowInfo : IStorable, IPersistable
		{
			[Flags]
			private enum RowInfoState : ushort
			{
				Clear = 0,
				PageBreakAtStart = 1,
				PageBreakAtEnd = 2,
				SpanPagesRow = 4,
				IgnoreInnerPageBreaks = 8,
				IgnoreKeepWith = 0x10,
				Hidden = 0x20,
				RepeatWith = 0x40,
				RepeatOnPage = 0x80,
				ResolvedRow = 0x100,
				KeepTogether = 0x200,
				ContentFullyCreated = 0x400
			}

			internal enum VerticalState : byte
			{
				Unknown,
				Normal,
				Above,
				Below,
				Repeat,
				TopOfNextPage
			}

			private double m_normalizeRowHeight;

			private double m_sourceHeight;

			private ScalableList<PageDetailCell> m_detailCells;

			private RowInfoState m_rowInfoState;

			private VerticalState m_verticalState;

			private double m_height;

			private double m_top = -1.7976931348623157E+308;

			private long m_offset = -1L;

			[StaticReference]
			private RPLTablixRow m_rplRow;

			private PageBreakProperties m_pageBreakPropertiesAtStart;

			private PageBreakProperties m_pageBreakPropertiesAtEnd;

			private static Declaration m_declaration = RowInfo.GetDeclaration();

			internal double Height
			{
				get
				{
					return this.m_height;
				}
				set
				{
					this.m_height = value;
				}
			}

			internal double Top
			{
				get
				{
					return this.m_top;
				}
				set
				{
					this.m_top = value;
				}
			}

			internal double Bottom
			{
				get
				{
					return this.m_top + this.m_height;
				}
			}

			internal double NormalizeRowHeight
			{
				get
				{
					return this.m_normalizeRowHeight;
				}
			}

			internal bool PageBreaksAtStart
			{
				get
				{
					return (int)(this.m_rowInfoState & RowInfoState.PageBreakAtStart) > 0;
				}
				set
				{
					if (value)
					{
						this.m_rowInfoState |= RowInfoState.PageBreakAtStart;
					}
					else
					{
						this.m_rowInfoState &= ~RowInfoState.PageBreakAtStart;
					}
				}
			}

			internal bool PageBreaksAtEnd
			{
				get
				{
					return (int)(this.m_rowInfoState & RowInfoState.PageBreakAtEnd) > 0;
				}
				set
				{
					if (value)
					{
						this.m_rowInfoState |= RowInfoState.PageBreakAtEnd;
					}
					else
					{
						this.m_rowInfoState &= ~RowInfoState.PageBreakAtEnd;
					}
				}
			}

			internal bool IgnoreKeepWith
			{
				get
				{
					return (int)(this.m_rowInfoState & RowInfoState.IgnoreKeepWith) > 0;
				}
				set
				{
					if (value)
					{
						this.m_rowInfoState |= RowInfoState.IgnoreKeepWith;
					}
					else
					{
						this.m_rowInfoState &= ~RowInfoState.IgnoreKeepWith;
					}
				}
			}

			internal bool RepeatOnPage
			{
				get
				{
					return (int)(this.m_rowInfoState & RowInfoState.RepeatOnPage) > 0;
				}
				set
				{
					if (value)
					{
						this.m_rowInfoState |= RowInfoState.RepeatOnPage;
					}
					else
					{
						this.m_rowInfoState &= ~RowInfoState.RepeatOnPage;
					}
				}
			}

			internal bool RepeatWith
			{
				get
				{
					return (int)(this.m_rowInfoState & RowInfoState.RepeatWith) > 0;
				}
				set
				{
					if (value)
					{
						this.m_rowInfoState |= RowInfoState.RepeatWith;
					}
					else
					{
						this.m_rowInfoState &= ~RowInfoState.RepeatWith;
					}
				}
			}

			internal bool SpanPagesRow
			{
				get
				{
					return (int)(this.m_rowInfoState & RowInfoState.SpanPagesRow) > 0;
				}
				set
				{
					if (value)
					{
						this.m_rowInfoState |= RowInfoState.SpanPagesRow;
					}
					else
					{
						this.m_rowInfoState &= ~RowInfoState.SpanPagesRow;
					}
				}
			}

			internal bool ContentFullyCreated
			{
				get
				{
					return (int)(this.m_rowInfoState & RowInfoState.ContentFullyCreated) > 0;
				}
				set
				{
					if (value)
					{
						this.m_rowInfoState |= RowInfoState.ContentFullyCreated;
					}
					else
					{
						this.m_rowInfoState &= ~RowInfoState.ContentFullyCreated;
					}
				}
			}

			internal bool Hidden
			{
				get
				{
					return (int)(this.m_rowInfoState & RowInfoState.Hidden) > 0;
				}
				set
				{
					if (value)
					{
						this.m_rowInfoState |= RowInfoState.Hidden;
					}
					else
					{
						this.m_rowInfoState &= ~RowInfoState.Hidden;
					}
				}
			}

			internal bool ResolvedRow
			{
				get
				{
					return (int)(this.m_rowInfoState & RowInfoState.ResolvedRow) > 0;
				}
				set
				{
					if (value)
					{
						this.m_rowInfoState |= RowInfoState.ResolvedRow;
					}
					else
					{
						this.m_rowInfoState &= ~RowInfoState.ResolvedRow;
					}
				}
			}

			internal bool KeepTogether
			{
				get
				{
					return (int)(this.m_rowInfoState & RowInfoState.KeepTogether) > 0;
				}
				set
				{
					if (value)
					{
						this.m_rowInfoState |= RowInfoState.KeepTogether;
					}
					else
					{
						this.m_rowInfoState &= ~RowInfoState.KeepTogether;
					}
				}
			}

			internal VerticalState PageVerticalState
			{
				get
				{
					return this.m_verticalState;
				}
				set
				{
					this.m_verticalState = value;
				}
			}

			internal long Offset
			{
				get
				{
					return this.m_offset;
				}
			}

			internal RPLTablixRow RPLTablixRow
			{
				get
				{
					return this.m_rplRow;
				}
				set
				{
					this.m_rplRow = value;
				}
			}

			internal ScalableList<PageDetailCell> Cells
			{
				get
				{
					return this.m_detailCells;
				}
			}

			public PageBreakProperties PageBreakPropertiesAtStart
			{
				get
				{
					return this.m_pageBreakPropertiesAtStart;
				}
				set
				{
					this.m_pageBreakPropertiesAtStart = value;
				}
			}

			public PageBreakProperties PageBreakPropertiesAtEnd
			{
				get
				{
					return this.m_pageBreakPropertiesAtEnd;
				}
				set
				{
					this.m_pageBreakPropertiesAtEnd = value;
				}
			}

			public int Size
			{
				get
				{
					return 43 + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.ReferenceSize + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_detailCells) + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_pageBreakPropertiesAtStart) + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_pageBreakPropertiesAtEnd);
				}
			}

			internal RowInfo()
			{
			}

			internal RowInfo(double sourceHeight)
			{
				this.m_sourceHeight = sourceHeight;
				this.m_normalizeRowHeight = sourceHeight;
				this.m_height = sourceHeight;
				this.ContentFullyCreated = true;
			}

			public void Serialize(IntermediateFormatWriter writer)
			{
				writer.RegisterDeclaration(RowInfo.m_declaration);
				IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.RowHeight:
						writer.Write(this.m_normalizeRowHeight);
						break;
					case MemberName.SourceHeight:
						writer.Write(this.m_sourceHeight);
						break;
					case MemberName.DetailCells:
						writer.Write(this.m_detailCells);
						break;
					case MemberName.State:
						writer.Write((ushort)this.m_rowInfoState);
						break;
					case MemberName.VerticalState:
						writer.Write((byte)this.m_verticalState);
						break;
					case MemberName.Height:
						writer.Write(this.m_height);
						break;
					case MemberName.Top:
						writer.Write(this.m_top);
						break;
					case MemberName.Offset:
						writer.Write(this.m_offset);
						break;
					case MemberName.RPLTablixRow:
					{
						int value = scalabilityCache.StoreStaticReference(this.m_rplRow);
						writer.Write(value);
						break;
					}
					case MemberName.PageBreakPropertiesAtStart:
						writer.Write(this.m_pageBreakPropertiesAtStart);
						break;
					case MemberName.PageBreakPropertiesAtEnd:
						writer.Write(this.m_pageBreakPropertiesAtEnd);
						break;
					default:
						RSTrace.RenderingTracer.Assert(false, string.Empty);
						break;
					}
				}
			}

			public void Deserialize(IntermediateFormatReader reader)
			{
				reader.RegisterDeclaration(RowInfo.m_declaration);
				IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
				while (reader.NextMember())
				{
					switch (reader.CurrentMember.MemberName)
					{
					case MemberName.RowHeight:
						this.m_normalizeRowHeight = reader.ReadDouble();
						break;
					case MemberName.SourceHeight:
						this.m_sourceHeight = reader.ReadDouble();
						break;
					case MemberName.DetailCells:
						this.m_detailCells = reader.ReadRIFObject<ScalableList<PageDetailCell>>();
						break;
					case MemberName.State:
						this.m_rowInfoState = (RowInfoState)reader.ReadUInt16();
						break;
					case MemberName.VerticalState:
						this.m_verticalState = (VerticalState)reader.ReadByte();
						break;
					case MemberName.Height:
						this.m_height = reader.ReadDouble();
						break;
					case MemberName.Top:
						this.m_top = reader.ReadDouble();
						break;
					case MemberName.Offset:
						this.m_offset = reader.ReadInt64();
						break;
					case MemberName.RPLTablixRow:
					{
						int id = reader.ReadInt32();
						this.m_rplRow = (RPLTablixRow)scalabilityCache.FetchStaticReference(id);
						break;
					}
					case MemberName.PageBreakPropertiesAtStart:
						this.m_pageBreakPropertiesAtStart = (PageBreakProperties)reader.ReadRIFObject();
						break;
					case MemberName.PageBreakPropertiesAtEnd:
						this.m_pageBreakPropertiesAtEnd = (PageBreakProperties)reader.ReadRIFObject();
						break;
					default:
						RSTrace.RenderingTracer.Assert(false, string.Empty);
						break;
					}
				}
			}

			public void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
			{
			}

			public ObjectType GetObjectType()
			{
				return ObjectType.RowInfo;
			}

			internal static Declaration GetDeclaration()
			{
				if (RowInfo.m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.RowHeight, Token.Double));
					list.Add(new MemberInfo(MemberName.SourceHeight, Token.Double));
					list.Add(new MemberInfo(MemberName.DetailCells, ObjectType.ScalableList, ObjectType.PageDetailCell));
					list.Add(new MemberInfo(MemberName.State, Token.UInt16));
					list.Add(new MemberInfo(MemberName.VerticalState, Token.Byte));
					list.Add(new MemberInfo(MemberName.Height, Token.Double));
					list.Add(new MemberInfo(MemberName.Top, Token.Double));
					list.Add(new MemberInfo(MemberName.Offset, Token.Int64));
					list.Add(new MemberInfo(MemberName.RPLTablixRow, Token.Int32));
					list.Add(new MemberInfo(MemberName.PageBreakPropertiesAtStart, ObjectType.PageBreakProperties));
					list.Add(new MemberInfo(MemberName.PageBreakPropertiesAtEnd, ObjectType.PageBreakProperties));
					return new Declaration(ObjectType.RowInfo, ObjectType.None, list);
				}
				return RowInfo.m_declaration;
			}

			internal void DisposeDetailCells()
			{
				if (this.m_detailCells != null)
				{
					this.m_detailCells.Dispose();
					this.m_detailCells = null;
				}
			}

			internal void ReverseDetailCells(PageContext pageContext)
			{
				if (this.m_detailCells != null && this.m_detailCells.Count != 0)
				{
					ScalableList<PageDetailCell> scalableList = new ScalableList<PageDetailCell>(0, pageContext.ScalabilityCache);
					for (int num = this.m_detailCells.Count - 1; num >= 0; num--)
					{
						scalableList.Add(this.m_detailCells[num]);
					}
					this.m_detailCells.Dispose();
					this.m_detailCells = scalableList;
				}
			}

			internal IDisposable UpdateLastDetailCell(double cellColDefWidth, out PageDetailCell lastCell)
			{
				lastCell = null;
				if (this.m_detailCells != null && this.m_detailCells.Count != 0)
				{
					IDisposable andPin = this.m_detailCells.GetAndPin(this.m_detailCells.Count - 1, out lastCell);
					lastCell.ColSpan++;
					lastCell.SourceWidth += cellColDefWidth;
					return andPin;
				}
				return null;
			}

			internal void AddDetailCell(PageDetailCell detailCell, PageContext pageContext)
			{
				if (this.m_detailCells == null)
				{
					this.m_detailCells = new ScalableList<PageDetailCell>(0, pageContext.ScalabilityCache);
				}
				this.m_detailCells.Add(detailCell);
			}

			internal void CalculateVerticalLastDetailCell(PageContext context, bool firstTouch, bool delayCalc)
			{
				if (!this.Hidden && this.m_detailCells != null && this.m_detailCells.Count != 0)
				{
					PageDetailCell pageDetailCell = null;
					using (this.m_detailCells.GetAndPin(this.m_detailCells.Count - 1, out pageDetailCell))
					{
						if (pageDetailCell.CellItem != null && !pageDetailCell.Hidden)
						{
							TextBox textBox = pageDetailCell.CellItem as TextBox;
							if (textBox != null)
							{
								if (pageDetailCell.SourceWidth > 0.0)
								{
									textBox.ItemPageSizes.Width = pageDetailCell.SourceWidth;
								}
								if (firstTouch)
								{
									if (delayCalc)
									{
										textBox.CalcSizeState = TextBox.CalcSize.Delay;
									}
									goto IL_00b0;
								}
								if (textBox.CalcSizeState == TextBox.CalcSize.Delay)
								{
									textBox.CalcSizeState = TextBox.CalcSize.LateCalc;
									goto IL_00b0;
								}
							}
							else if (firstTouch)
							{
								goto IL_00b0;
							}
						}
						goto end_IL_003c;
						IL_00b0:
						bool flag = false;
						pageDetailCell.CellItem.CalculateVertical(context, 0.0, context.ColumnHeight, null, new List<PageItem>(), ref flag, true, pageDetailCell.SourceWidth);
						if (pageDetailCell.CellItem.ItemPageSizes.Height > this.m_normalizeRowHeight)
						{
							this.m_normalizeRowHeight = pageDetailCell.CellItem.ItemPageSizes.Height;
							this.m_height = this.m_normalizeRowHeight;
						}
						this.CheckCellSpanPages(pageDetailCell, context.ColumnHeight);
						if (context.IgnorePageBreaks)
						{
							this.m_rowInfoState |= RowInfoState.IgnoreInnerPageBreaks;
						}
						end_IL_003c:;
					}
				}
			}

			internal void UpdatePinnedRow()
			{
				this.SpanPagesRow = false;
				this.ContentFullyCreated = true;
				this.m_normalizeRowHeight = this.m_sourceHeight;
				this.m_height = this.m_sourceHeight;
				this.ResolvedRow = true;
			}

			internal void ResetRowHeight()
			{
				this.m_height = this.m_normalizeRowHeight;
			}

			internal void UpdateVerticalDetailCell(PageContext context, double startInTablix, double endInTablix, ref int detailCellIndex)
			{
				if (!this.Hidden && this.m_detailCells != null && this.m_detailCells.Count != 0)
				{
					PageDetailCell pageDetailCell = null;
					using (this.m_detailCells.GetAndPin(detailCellIndex, out pageDetailCell))
					{
						detailCellIndex++;
						if (pageDetailCell.CellItem != null && !pageDetailCell.Hidden)
						{
							bool flag = false;
							double topInParentSystem = startInTablix - this.m_top;
							double num = endInTablix - this.m_top;
							PageContext pageContext = new PageContext(context, true);
							if ((int)(this.m_rowInfoState & RowInfoState.IgnoreInnerPageBreaks) > 0)
							{
								pageContext.IgnorePageBreaks = true;
								if (!context.IgnorePageBreaks)
								{
									pageContext.IgnorePageBreaksReason = PageContext.IgnorePageBreakReason.Unknown;
								}
							}
							pageDetailCell.CellItem.CalculateVertical(context, topInParentSystem, num, null, new List<PageItem>(), ref flag, false, pageDetailCell.SourceWidth);
							if (pageDetailCell.CellItem.ItemPageSizes.Bottom > this.m_normalizeRowHeight)
							{
								this.m_normalizeRowHeight = pageDetailCell.CellItem.ItemPageSizes.Bottom;
								this.m_height = this.m_normalizeRowHeight;
							}
							this.CheckCellSpanPages(pageDetailCell, num);
						}
					}
				}
			}

			private void CheckCellSpanPages(PageDetailCell lastCell, double endInRow)
			{
				if (lastCell.CellItem.PBAreUnresolved)
				{
					this.m_rowInfoState |= RowInfoState.SpanPagesRow;
				}
				else
				{
					RoundedDouble x = new RoundedDouble(this.m_height);
					if (x > endInRow)
					{
						this.m_rowInfoState |= RowInfoState.SpanPagesRow;
						this.ContentFullyCreated = false;
					}
				}
				if (!lastCell.CellItem.FullyCreated)
				{
					this.ContentFullyCreated = false;
				}
			}

			internal void CalculateHorizontal(ScalableList<ColumnInfo> columnInfo, PageContext context)
			{
				if (this.PageVerticalState == VerticalState.Normal && !this.Hidden && this.m_detailCells != null && this.m_detailCells.Count != 0)
				{
					PageDetailCell pageDetailCell = null;
					PageContext pageContext = new PageContext(context);
					int num = 0;
					if (this.RepeatOnPage)
					{
						pageContext.ResetHorizontal = true;
					}
					else
					{
						pageContext.ResetHorizontal = context.ResetHorizontal;
					}
					for (int i = 0; i < this.m_detailCells.Count; i++)
					{
						using (this.m_detailCells.GetAndPin(i, out pageDetailCell))
						{
							if (!pageDetailCell.Hidden)
							{
								if (pageDetailCell.CellItem == null)
								{
									Tablix.UpdateSizes(num, pageDetailCell.ColSpan, pageDetailCell.SourceWidth, false, false, false, columnInfo);
								}
								else
								{
									bool flag = false;
									pageDetailCell.CellItem.CalculateHorizontal(pageContext, 0.0, 1.7976931348623157E+308, null, new List<PageItem>(), ref flag, true, pageDetailCell.SourceWidth);
									double size = Math.Max(pageDetailCell.SourceWidth, pageDetailCell.CellItem.ItemPageSizes.Width);
									bool unresolved = pageDetailCell.CellItem.KTHIsUnresolved || pageDetailCell.CellItem.NeedResolve;
									Tablix.UpdateSizes(num, pageDetailCell.ColSpan, size, unresolved, pageDetailCell.KeepTogether, false, columnInfo);
								}
							}
							else
							{
								Tablix.UpdateHidden(num, pageDetailCell.ColSpan, columnInfo);
							}
						}
						num += pageDetailCell.ColSpan;
					}
				}
			}

			internal bool ResolveVertical(double startInTablix, double endInTablix, PageContext context)
			{
				if (this.m_detailCells != null && !this.Hidden)
				{
					if (this.ResolvedRow)
					{
						this.ResolvedRow = false;
						return false;
					}
					PageDetailCell pageDetailCell = null;
					PageItem pageItem = null;
					double topInParentSystem = Math.Max(0.0, startInTablix - this.m_top);
					double num = endInTablix - this.m_top;
					PageContext pageContext = context;
					if ((int)(this.m_rowInfoState & RowInfoState.IgnoreInnerPageBreaks) > 0)
					{
						pageContext = new PageContext(context);
						pageContext.IgnorePageBreaks = true;
						if (!context.IgnorePageBreaks)
						{
							pageContext.IgnorePageBreaksReason = PageContext.IgnorePageBreakReason.Unknown;
						}
					}
					this.SpanPagesRow = false;
					this.ContentFullyCreated = true;
					double num2 = this.m_sourceHeight;
					for (int i = 0; i < this.m_detailCells.Count; i++)
					{
						using (this.m_detailCells.GetAndPin(i, out pageDetailCell))
						{
							if (!pageDetailCell.Hidden && !pageDetailCell.Release)
							{
								pageItem = pageDetailCell.CellItem;
								if (pageItem != null)
								{
									if (pageItem.KTVIsUnresolved || pageItem.PBAreUnresolved || pageItem.NeedResolve)
									{
										pageItem.ResolveVertical(context, topInParentSystem, num, null, true, pageContext.Common.CanOverwritePageBreak, pageContext.Common.CanSetPageName);
									}
									if (pageItem.ItemPageSizes.Bottom > num2)
									{
										num2 = pageItem.ItemPageSizes.Bottom;
									}
									if (pageItem.PBAreUnresolved)
									{
										this.m_rowInfoState |= RowInfoState.SpanPagesRow;
									}
									if (!pageItem.FullyCreated)
									{
										this.ContentFullyCreated = false;
									}
								}
							}
						}
					}
					if (!this.SpanPagesRow)
					{
						RoundedDouble x = new RoundedDouble(num2);
						if (x > num)
						{
							this.m_rowInfoState |= RowInfoState.SpanPagesRow;
						}
					}
					RoundedDouble x2 = new RoundedDouble(this.m_normalizeRowHeight - num2);
					if (x2 == 0.0)
					{
						return false;
					}
					this.m_normalizeRowHeight = num2;
					this.m_height = this.m_normalizeRowHeight;
					return true;
				}
				return false;
			}

			internal void ResolveHorizontal(ScalableList<ColumnInfo> columnInfo, int colIndex, double startInTablix, double endInTablix, PageContext pageContext)
			{
				if (this.PageVerticalState == VerticalState.Normal && !this.Hidden && this.m_detailCells != null && this.m_detailCells.Count != 0)
				{
					int num = 0;
					int num2 = 0;
					PageDetailCell pageDetailCell = null;
					PageItem pageItem = null;
					PageContext pageContext2 = new PageContext(pageContext);
					double num3 = 0.0;
					double num4 = 0.0;
					double num5 = 0.0;
					bool flag = false;
					ColumnInfo columnInfo2 = null;
					for (int i = 0; i < this.m_detailCells.Count; i++)
					{
						pageDetailCell = this.m_detailCells[i];
						num2 = num + pageDetailCell.ColSpan;
						if (!pageDetailCell.Hidden && num <= colIndex && num2 > colIndex)
						{
							columnInfo2 = columnInfo[num];
							num5 = columnInfo[num2 - 1].Right - columnInfo2.Left;
							num3 = Math.Max(0.0, startInTablix - columnInfo2.Left);
							num4 = endInTablix - columnInfo2.Left;
							using (this.m_detailCells.GetAndPin(i, out pageDetailCell))
							{
								pageItem = pageDetailCell.CellItem;
								if (pageItem != null && (pageItem.KTHIsUnresolved || pageItem.NeedResolve))
								{
									pageItem.ResolveHorizontal(pageContext2, num3, num4, null, true);
									double num6 = pageItem.ItemPageSizes.Right - num5;
									if (num6 > 0.0)
									{
										int num7 = num2 - 1;
										while (num7 > colIndex && columnInfo[num7].Hidden)
										{
											num7--;
										}
										using (columnInfo.GetAndPin(num7, out columnInfo2))
										{
											columnInfo2.SizeValue += num6;
										}
										num7++;
										if (num7 < columnInfo.Count)
										{
											using (columnInfo.GetAndPin(num7, out columnInfo2))
											{
												columnInfo2.PageHorizontalState = ColumnInfo.HorizontalState.Unknown;
											}
										}
									}
									if (pageItem.KTHIsUnresolved || pageItem.NeedResolve)
									{
										flag = true;
									}
								}
							}
						}
						num = num2;
					}
					if (flag)
					{
						using (columnInfo.GetAndPin(colIndex, out columnInfo2))
						{
							columnInfo2.Unresolved = flag;
						}
					}
				}
			}

			internal int AddToPageContent(ScalableList<ColumnInfo> columnInfo, out int colsOnPage, bool isLTR, bool pinnedToParentCell, RPLWriter rplWriter, PageContext pageContext, double pageLeft, double pageTop, double pageRight, double pageBottom, RepeatState repeatState)
			{
				colsOnPage = 0;
				if (this.PageVerticalState != VerticalState.Normal)
				{
					return 0;
				}
				if (this.m_detailCells != null && this.m_detailCells.Count != 0)
				{
					PageDetailCell pageDetailCell = null;
					int i = 0;
					int num = 0;
					double pageTop2 = Math.Max(0.0, pageTop - this.m_top);
					double num2 = pageBottom - this.m_top;
					double num3 = 0.0;
					double num4 = 0.0;
					double num5 = 0.0;
					RTLTextBoxes delayedTB = null;
					RepeatState repeatState2 = repeatState;
					if (this.RepeatWith && !this.SpanPagesRow)
					{
						repeatState2 |= RepeatState.Vertical;
					}
					if (rplWriter != null)
					{
						rplWriter.EnterDelayedTBLevel(isLTR, ref delayedTB);
					}
					for (int j = 0; j < this.m_detailCells.Count; j++)
					{
						using (this.m_detailCells.GetAndPin(j, out pageDetailCell))
						{
							num = i + pageDetailCell.ColSpan - 1;
							if (columnInfo[i].PageHorizontalState != ColumnInfo.HorizontalState.AtRight)
							{
								if (columnInfo[num].PageHorizontalState == ColumnInfo.HorizontalState.AtLeft)
								{
									i = num + 1;
								}
								else
								{
									PageItem cellItem = pageDetailCell.CellItem;
									if (cellItem != null)
									{
										num5 = columnInfo[i].Left;
										num3 = Math.Max(0.0, pageLeft - num5);
										num4 = pageRight - num5;
										if (pinnedToParentCell)
										{
											cellItem.ItemPageSizes.AdjustHeightTo(this.m_height);
										}
										pageDetailCell.ContentOnPage = cellItem.AddToPage(rplWriter, pageContext, num3, pageTop2, num4, num2, repeatState2);
										if (pageDetailCell.ContentOnPage)
										{
											pageDetailCell.ContentOnPage = cellItem.ContentOnPage;
										}
										if (pageDetailCell.ContentOnPage && rplWriter != null)
										{
											rplWriter.RegisterCellTextBoxes(isLTR, delayedTB);
										}
										if (repeatState2 == RepeatState.None && num2 >= this.m_normalizeRowHeight && cellItem.Release(num2, num4))
										{
											pageDetailCell.Release = true;
										}
									}
									if (!pageDetailCell.Hidden && !this.Hidden)
									{
										ColumnInfo columnInfo2 = null;
										for (; i <= num; i++)
										{
											columnInfo2 = columnInfo[i];
											if (columnInfo2.PageHorizontalState != ColumnInfo.HorizontalState.AtLeft)
											{
												break;
											}
										}
										colsOnPage += num - i + 1;
										for (; i <= num; i++)
										{
											columnInfo2 = columnInfo[i];
											if (columnInfo2.Hidden)
											{
												colsOnPage--;
											}
											else if (columnInfo2.PageHorizontalState == ColumnInfo.HorizontalState.AtRight)
											{
												colsOnPage -= num - i + 1;
												i = num;
											}
										}
									}
									i = num + 1;
								}
								continue;
							}
						}
						break;
					}
					if (rplWriter != null)
					{
						rplWriter.LeaveDelayedTBLevel(isLTR, delayedTB, pageContext);
					}
					if (this.Hidden)
					{
						return 0;
					}
					return 1;
				}
				return 1;
			}

			internal int AddToPage(ScalableList<ColumnInfo> columnInfo, RPLWriter rplWriter, int pageRowIndex, int colsBeforeRH, int headerRowCols)
			{
				if (this.PageVerticalState == VerticalState.Normal && !this.Hidden)
				{
					if (this.m_detailCells != null && this.m_detailCells.Count != 0)
					{
						List<RPLTablixCell> list = null;
						PageDetailCell pageDetailCell = null;
						PageItem pageItem = null;
						int i = 0;
						int num = 0;
						int num2 = 0;
						int num3 = 0;
						BinaryWriter binaryWriter = rplWriter.BinaryWriter;
						if (binaryWriter != null)
						{
							this.m_offset = binaryWriter.BaseStream.Position;
							binaryWriter.Write((byte)18);
							binaryWriter.Write(pageRowIndex);
						}
						else
						{
							list = new List<RPLTablixCell>();
							this.m_rplRow = new RPLTablixRow(list);
						}
						for (int j = 0; j < this.m_detailCells.Count; j++)
						{
							pageDetailCell = this.m_detailCells[j];
							num = i + pageDetailCell.ColSpan - 1;
							if (columnInfo[i].PageHorizontalState == ColumnInfo.HorizontalState.AtRight)
							{
								break;
							}
							if (columnInfo[num].PageHorizontalState == ColumnInfo.HorizontalState.AtLeft)
							{
								i = num + 1;
							}
							else
							{
								double num4 = 0.0;
								for (; i <= num && columnInfo[i].PageHorizontalState == ColumnInfo.HorizontalState.AtLeft; i++)
								{
									num4 += columnInfo[i].SizeValue;
								}
								num3 = num - i + 1;
								bool flag = false;
								for (; i <= num; i++)
								{
									if (columnInfo[i].Hidden)
									{
										num3--;
									}
									else if (columnInfo[i].PageHorizontalState == ColumnInfo.HorizontalState.AtRight)
									{
										num3 -= num - i + 1;
										i = num;
										flag = true;
									}
								}
								if (num2 == colsBeforeRH)
								{
									num2 += headerRowCols;
								}
								if (!pageDetailCell.Hidden)
								{
									pageItem = pageDetailCell.CellItem;
									double num5 = 0.0;
									if (pageItem != null && pageDetailCell.ContentOnPage)
									{
										num5 = pageItem.ItemPageSizes.Left - num4;
										if (flag && !(pageItem is TextBox))
										{
											flag = false;
										}
									}
									if (binaryWriter != null)
									{
										binaryWriter.Write((byte)13);
										if (pageItem != null && pageDetailCell.ContentOnPage)
										{
											binaryWriter.Write((byte)4);
											binaryWriter.Write(pageItem.Offset);
											if (pageItem.RplItemState > 0)
											{
												binaryWriter.Write((byte)13);
												binaryWriter.Write(pageItem.RplItemState);
											}
											if (pageItem.ItemPageSizes.Top != 0.0)
											{
												binaryWriter.Write((byte)0);
												binaryWriter.Write((float)pageItem.ItemPageSizes.Top);
											}
											if (num5 != 0.0)
											{
												binaryWriter.Write((byte)1);
												binaryWriter.Write((float)num5);
											}
											if (flag && pageItem.ItemPageSizes.Width != 0.0)
											{
												binaryWriter.Write((byte)2);
												binaryWriter.Write((float)pageItem.ItemPageSizes.Width);
											}
										}
										if (num3 > 1)
										{
											binaryWriter.Write((byte)5);
											binaryWriter.Write(num3);
										}
										binaryWriter.Write((byte)8);
										binaryWriter.Write(num2);
										binaryWriter.Write((byte)255);
									}
									else
									{
										RPLTablixCell rPLTablixCell = null;
										if (pageItem != null && pageDetailCell.ContentOnPage)
										{
											RPLItem element = pageItem.RPLElement as RPLItem;
											rPLTablixCell = new RPLTablixCell(element, pageItem.RplItemState);
											if (pageItem.ItemPageSizes.Top != 0.0)
											{
												rPLTablixCell.ContentSizes = new RPLSizes();
												rPLTablixCell.ContentSizes.Top = (float)pageItem.ItemPageSizes.Top;
											}
											if (num5 != 0.0)
											{
												if (rPLTablixCell.ContentSizes == null)
												{
													rPLTablixCell.ContentSizes = new RPLSizes();
												}
												rPLTablixCell.ContentSizes.Left = (float)num5;
											}
											if (flag && pageItem.ItemPageSizes.Width != 0.0)
											{
												if (rPLTablixCell.ContentSizes == null)
												{
													rPLTablixCell.ContentSizes = new RPLSizes();
												}
												rPLTablixCell.ContentSizes.Width = (float)pageItem.ItemPageSizes.Width;
											}
										}
										else
										{
											rPLTablixCell = new RPLTablixCell();
										}
										rPLTablixCell.ColSpan = num3;
										rPLTablixCell.ColIndex = num2;
										rPLTablixCell.RowIndex = pageRowIndex;
										list.Add(rPLTablixCell);
									}
									num2 += num3;
								}
								if (pageDetailCell.Release)
								{
									using (this.m_detailCells.GetAndPin(j, out pageDetailCell))
									{
										pageDetailCell.CellItem = null;
									}
								}
							}
						}
						if (binaryWriter != null)
						{
							binaryWriter.Write((byte)255);
						}
						return 1;
					}
					return 1;
				}
				return 0;
			}

			internal int MergeDetailCells(int destCellIndex, List<int> destState, int srcCellIndex, List<int> srcState)
			{
				PageDetailCell pageDetailCell = null;
				int num = -1;
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				int num5 = srcCellIndex;
				int result = 0;
				while (true)
				{
					if (num3 >= destState.Count && num4 >= srcState.Count)
					{
						break;
					}
					num = -1;
					if (num3 < destState.Count)
					{
						num = destState[num3];
					}
					num2 = num + 1;
					if (num4 < srcState.Count)
					{
						num2 = srcState[num4];
					}
					if (num == num2)
					{
						using (this.m_detailCells.GetAndPin(destCellIndex, out pageDetailCell))
						{
							HiddenPageItem hiddenPageItem = pageDetailCell.CellItem as HiddenPageItem;
							if (hiddenPageItem != null)
							{
								hiddenPageItem.AddToCollection((HiddenPageItem)this.m_detailCells[srcCellIndex].CellItem);
							}
						}
						num3++;
						num4++;
						destCellIndex++;
						srcCellIndex++;
					}
					else if (num < 0 || num > num2)
					{
						this.m_detailCells.Insert(destCellIndex, this.m_detailCells[srcCellIndex]);
						result = this.m_detailCells[srcCellIndex].ColSpan;
						num5++;
						destCellIndex++;
						srcCellIndex++;
						destState.Insert(num3, num2);
						num3++;
						num4++;
					}
					else
					{
						destCellIndex++;
						num3++;
					}
				}
				this.m_detailCells.RemoveRange(num5, srcState.Count);
				return result;
			}

			internal void Merge(RowInfo rowSource)
			{
				if (this.m_detailCells != null)
				{
					PageDetailCell pageDetailCell = null;
					for (int i = 0; i < this.m_detailCells.Count; i++)
					{
						using (this.m_detailCells.GetAndPin(i, out pageDetailCell))
						{
							HiddenPageItem hiddenPageItem = pageDetailCell.CellItem as HiddenPageItem;
							if (hiddenPageItem != null)
							{
								HiddenPageItem hiddenItem = (HiddenPageItem)rowSource.Cells[i].CellItem;
								hiddenPageItem.AddToCollection(hiddenItem);
							}
						}
					}
				}
			}

			internal bool ResolveDuplicates(double startInTablix, PageContext pageContext)
			{
				if (!this.Hidden && this.m_detailCells != null)
				{
					if (this.PageVerticalState != VerticalState.Above && this.PageVerticalState != VerticalState.Repeat)
					{
						double topInParentSystem = Math.Max(0.0, startInTablix - this.m_top);
						PageDetailCell pageDetailCell = null;
						bool result = false;
						for (int i = 0; i < this.m_detailCells.Count; i++)
						{
							using (this.m_detailCells.GetAndPin(i, out pageDetailCell))
							{
								if (pageDetailCell.CellItem != null && !pageDetailCell.Hidden && pageDetailCell.CellItem.ResolveDuplicates(pageContext, topInParentSystem, null, false))
								{
									result = true;
									if (pageDetailCell.CellItem.ItemPageSizes.Bottom > this.m_normalizeRowHeight)
									{
										this.m_normalizeRowHeight = pageDetailCell.CellItem.ItemPageSizes.Bottom;
										this.m_height = this.m_normalizeRowHeight;
									}
								}
							}
						}
						return result;
					}
					return false;
				}
				return false;
			}
		}

		internal class SizeInfo : IStorable, IPersistable
		{
			internal enum PageState : byte
			{
				Unknown,
				Normal,
				Skip
			}

			private double m_size;

			private double m_startPos;

			private Hashtable m_spanSize;

			private PageState m_state;

			private static Declaration m_declaration = SizeInfo.GetDeclaration();

			internal Hashtable SpanSize
			{
				get
				{
					return this.m_spanSize;
				}
				set
				{
					this.m_spanSize = value;
				}
			}

			internal double SizeValue
			{
				get
				{
					return this.m_size;
				}
				set
				{
					this.m_size = value;
				}
			}

			internal double StartPos
			{
				get
				{
					return this.m_startPos;
				}
				set
				{
					this.m_startPos = value;
				}
			}

			internal double EndPos
			{
				get
				{
					return this.m_startPos + this.m_size;
				}
			}

			internal PageState State
			{
				get
				{
					return this.m_state;
				}
				set
				{
					this.m_state = value;
				}
			}

			public int Size
			{
				get
				{
					return 16 + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_spanSize) + 1;
				}
			}

			internal SizeInfo()
			{
			}

			internal SizeInfo(double size)
			{
				this.m_size = size;
			}

			public void Serialize(IntermediateFormatWriter writer)
			{
				writer.RegisterDeclaration(SizeInfo.m_declaration);
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.Size:
						writer.Write(this.m_size);
						break;
					case MemberName.StartPos:
						writer.Write(this.m_startPos);
						break;
					case MemberName.SpanSize:
						writer.WriteVariantVariantHashtable(this.m_spanSize);
						break;
					case MemberName.State:
						writer.Write(this.m_state);
						break;
					default:
						RSTrace.RenderingTracer.Assert(false, string.Empty);
						break;
					}
				}
			}

			public void Deserialize(IntermediateFormatReader reader)
			{
				reader.RegisterDeclaration(SizeInfo.m_declaration);
				while (reader.NextMember())
				{
					switch (reader.CurrentMember.MemberName)
					{
					case MemberName.Size:
						this.m_size = reader.ReadDouble();
						break;
					case MemberName.StartPos:
						this.m_startPos = reader.ReadDouble();
						break;
					case MemberName.SpanSize:
						this.m_spanSize = reader.ReadVariantVariantHashtable();
						break;
					case MemberName.State:
						this.m_state = (PageState)reader.ReadByte();
						break;
					default:
						RSTrace.RenderingTracer.Assert(false, string.Empty);
						break;
					}
				}
			}

			public void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
			{
			}

			public ObjectType GetObjectType()
			{
				return ObjectType.SizeInfo;
			}

			internal static Declaration GetDeclaration()
			{
				if (SizeInfo.m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.Size, Token.Double));
					list.Add(new MemberInfo(MemberName.StartPos, Token.Double));
					list.Add(new MemberInfo(MemberName.SpanSize, ObjectType.VariantVariantHashtable));
					list.Add(new MemberInfo(MemberName.State, Token.Byte));
					return new Declaration(ObjectType.SizeInfo, ObjectType.None, list);
				}
				return SizeInfo.m_declaration;
			}

			internal void AddSpanSize(int span, double spanSize)
			{
				RSTrace.RenderingTracer.Assert(span > 1, string.Empty);
				if (this.m_spanSize == null)
				{
					this.m_spanSize = new Hashtable();
					this.m_spanSize.Add(span, spanSize);
				}
				else
				{
					object obj = this.m_spanSize[span];
					if (obj == null)
					{
						this.m_spanSize.Add(span, spanSize);
					}
					else
					{
						this.m_spanSize[span] = Math.Max(spanSize, (double)obj);
					}
				}
			}

			internal static void UpdateSize(SizeInfo sizeInfo, int span, double size)
			{
				RSTrace.RenderingTracer.Assert(span > 0, string.Empty);
				if (span == 1)
				{
					sizeInfo.SizeValue = Math.Max(sizeInfo.SizeValue, size);
				}
				else
				{
					sizeInfo.AddSpanSize(span, size);
				}
			}
		}

		internal class ColumnInfo : IStorable, IPersistable
		{
			internal enum HorizontalState : byte
			{
				Unknown,
				Normal,
				AtLeft,
				AtRight,
				LeftOfNextPage
			}

			[Flags]
			private enum ColumnInfoState : byte
			{
				BlockedBySpan = 1,
				Unresolved = 2,
				Hidden = 4,
				KeepTogether = 8
			}

			private double m_size;

			internal double m_left = -1.7976931348623157E+308;

			private Hashtable m_spanSize;

			private ColumnInfoState m_state;

			internal HorizontalState m_horizontalState;

			private static Declaration m_declaration = ColumnInfo.GetDeclaration();

			internal Hashtable SpanSize
			{
				get
				{
					return this.m_spanSize;
				}
				set
				{
					this.m_spanSize = value;
				}
			}

			internal double SizeValue
			{
				get
				{
					return this.m_size;
				}
				set
				{
					this.m_size = value;
				}
			}

			internal bool Empty
			{
				get
				{
					if (this.m_size == 0.0)
					{
						return !this.Hidden;
					}
					return false;
				}
			}

			internal bool Hidden
			{
				get
				{
					return (int)(this.m_state & ColumnInfoState.Hidden) > 0;
				}
				set
				{
					if (value)
					{
						this.m_state |= ColumnInfoState.Hidden;
					}
					else
					{
						this.m_state &= ~ColumnInfoState.Hidden;
					}
				}
			}

			internal bool BlockedBySpan
			{
				get
				{
					return (int)(this.m_state & ColumnInfoState.BlockedBySpan) > 0;
				}
				set
				{
					if (value)
					{
						this.m_state |= ColumnInfoState.BlockedBySpan;
					}
					else
					{
						this.m_state &= ~ColumnInfoState.BlockedBySpan;
					}
				}
			}

			internal bool Unresolved
			{
				get
				{
					return (int)(this.m_state & ColumnInfoState.Unresolved) > 0;
				}
				set
				{
					if (value)
					{
						this.m_state |= ColumnInfoState.Unresolved;
					}
					else
					{
						this.m_state &= ~ColumnInfoState.Unresolved;
					}
				}
			}

			internal bool KeepTogether
			{
				get
				{
					return (int)(this.m_state & ColumnInfoState.KeepTogether) > 0;
				}
				set
				{
					if (value)
					{
						this.m_state |= ColumnInfoState.KeepTogether;
					}
					else
					{
						this.m_state &= ~ColumnInfoState.KeepTogether;
					}
				}
			}

			internal HorizontalState PageHorizontalState
			{
				get
				{
					return this.m_horizontalState;
				}
				set
				{
					this.m_horizontalState = value;
				}
			}

			internal double Left
			{
				get
				{
					return this.m_left;
				}
				set
				{
					this.m_left = value;
				}
			}

			internal double Right
			{
				get
				{
					return this.m_left + this.m_size;
				}
			}

			public int Size
			{
				get
				{
					return 16 + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_spanSize) + 2;
				}
			}

			internal ColumnInfo()
			{
			}

			internal ColumnInfo(double size)
			{
				this.m_size = size;
			}

			public void Serialize(IntermediateFormatWriter writer)
			{
				writer.RegisterDeclaration(ColumnInfo.m_declaration);
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.Size:
						writer.Write(this.m_size);
						break;
					case MemberName.Left:
						writer.Write(this.m_left);
						break;
					case MemberName.SpanSize:
						writer.WriteVariantVariantHashtable(this.m_spanSize);
						break;
					case MemberName.State:
						writer.Write((byte)this.m_state);
						break;
					case MemberName.HorizontalState:
						writer.Write((byte)this.m_horizontalState);
						break;
					default:
						RSTrace.RenderingTracer.Assert(false, string.Empty);
						break;
					}
				}
			}

			public void Deserialize(IntermediateFormatReader reader)
			{
				reader.RegisterDeclaration(ColumnInfo.m_declaration);
				while (reader.NextMember())
				{
					switch (reader.CurrentMember.MemberName)
					{
					case MemberName.Size:
						this.m_size = reader.ReadDouble();
						break;
					case MemberName.Left:
						this.m_left = reader.ReadDouble();
						break;
					case MemberName.SpanSize:
						this.m_spanSize = reader.ReadVariantVariantHashtable();
						break;
					case MemberName.State:
						this.m_state = (ColumnInfoState)reader.ReadByte();
						break;
					case MemberName.HorizontalState:
						this.m_horizontalState = (HorizontalState)reader.ReadByte();
						break;
					default:
						RSTrace.RenderingTracer.Assert(false, string.Empty);
						break;
					}
				}
			}

			public void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
			{
			}

			public ObjectType GetObjectType()
			{
				return ObjectType.ColumnInfo;
			}

			internal static Declaration GetDeclaration()
			{
				if (ColumnInfo.m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.Size, Token.Double));
					list.Add(new MemberInfo(MemberName.Left, Token.Double));
					list.Add(new MemberInfo(MemberName.SpanSize, ObjectType.VariantVariantHashtable));
					list.Add(new MemberInfo(MemberName.State, Token.Byte));
					list.Add(new MemberInfo(MemberName.HorizontalState, Token.Byte));
					return new Declaration(ObjectType.ColumnInfo, ObjectType.None, list);
				}
				return ColumnInfo.m_declaration;
			}

			internal void AddSpanSize(int span, double spanSize)
			{
				if (this.m_spanSize == null)
				{
					this.m_spanSize = new Hashtable();
					this.m_spanSize.Add(span, spanSize);
				}
				else
				{
					object obj = this.m_spanSize[span];
					if (obj == null)
					{
						this.m_spanSize.Add(span, spanSize);
					}
					else
					{
						this.m_spanSize[span] = Math.Max(spanSize, (double)obj);
					}
				}
			}
		}

		private TablixState m_tablixStateFlags;

		[StaticReference]
		private TablixRowCollection m_bodyRows;

		private double[] m_bodyRowsHeights;

		private double[] m_bodyColWidths;

		private int m_rowMembersDepth;

		private int m_colMembersDepth;

		[StaticReference]
		private List<RPLTablixMemberDef> m_rowMemberDefList;

		private Hashtable m_rowMemberDefIndexes;

		[StaticReference]
		private List<RPLTablixMemberDef> m_colMemberDefList;

		private Hashtable m_colMemberDefIndexes;

		private Hashtable m_rowMemberInstanceIndexes;

		private Hashtable m_memberAtLevelIndexes;

		private int m_ignoreCellPageBreaks;

		private int m_headerRowCols;

		private int m_headerColumnRows;

		private int m_rowMemberIndexCell = -1;

		private int m_colMemberIndexCell = -1;

		private int m_colsBeforeRowHeaders;

		private List<PageStructMemberCell> m_columnHeaders;

		private List<SizeInfo> m_colHeaderHeights;

		private List<PageStructMemberCell> m_rowHeaders;

		private List<SizeInfo> m_rowHeaderWidths;

		private ScalableList<RowInfo> m_detailRows;

		private PageCornerCell[,] m_cornerCells;

		private int m_ignoreGroupPageBreaks;

		private ScalableList<ColumnInfo> m_columnInfo;

		private int m_ignoreCol;

		private int m_ignoreRow;

		private static Declaration m_declaration = Tablix.GetDeclaration();

		internal bool PinnedToParentCell
		{
			get
			{
				if (base.TablixCellTopItem && this.m_ignoreCellPageBreaks > 0)
				{
					return true;
				}
				return false;
			}
		}

		private bool NoRows
		{
			get
			{
				return this.GetFlagValue(TablixState.NoRows);
			}
			set
			{
				this.SetFlagValue(TablixState.NoRows, value);
			}
		}

		private bool IsLTR
		{
			get
			{
				return this.GetFlagValue(TablixState.IsLTR);
			}
			set
			{
				this.SetFlagValue(TablixState.IsLTR, value);
			}
		}

		private bool RepeatColumnHeaders
		{
			get
			{
				return this.GetFlagValue(TablixState.RepeatColumnHeaders);
			}
			set
			{
				this.SetFlagValue(TablixState.RepeatColumnHeaders, value);
			}
		}

		private bool RepeatedColumnHeaders
		{
			get
			{
				return this.GetFlagValue(TablixState.RepeatedColumnHeaders);
			}
			set
			{
				this.SetFlagValue(TablixState.RepeatedColumnHeaders, value);
			}
		}

		private bool AddToPageColumnHeaders
		{
			get
			{
				return this.GetFlagValue(TablixState.AddToPageColumnHeaders);
			}
			set
			{
				this.SetFlagValue(TablixState.AddToPageColumnHeaders, value);
			}
		}

		private bool SplitColumnHeaders
		{
			get
			{
				return this.GetFlagValue(TablixState.SplitColumnHeaders);
			}
			set
			{
				this.SetFlagValue(TablixState.SplitColumnHeaders, value);
			}
		}

		private bool RepeatRowHeaders
		{
			get
			{
				return this.GetFlagValue(TablixState.RepeatRowHeaders);
			}
			set
			{
				this.SetFlagValue(TablixState.RepeatRowHeaders, value);
			}
		}

		private bool AddToPageRowHeaders
		{
			get
			{
				return this.GetFlagValue(TablixState.AddToPageRowHeaders);
			}
			set
			{
				this.SetFlagValue(TablixState.AddToPageRowHeaders, value);
			}
		}

		private bool SplitRowHeaders
		{
			get
			{
				return this.GetFlagValue(TablixState.SplitRowHeaders);
			}
			set
			{
				this.SetFlagValue(TablixState.SplitRowHeaders, value);
			}
		}

		private bool ColumnHeadersCreated
		{
			get
			{
				return this.GetFlagValue(TablixState.ColumnHeadersCreated);
			}
			set
			{
				this.SetFlagValue(TablixState.ColumnHeadersCreated, value);
			}
		}

		private bool RowHeadersCreated
		{
			get
			{
				return this.GetFlagValue(TablixState.RowHeadersCreated);
			}
			set
			{
				this.SetFlagValue(TablixState.RowHeadersCreated, value);
			}
		}

		public override int Size
		{
			get
			{
				return base.Size + 2 + 3 * AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.ReferenceSize + 44 + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_bodyRowsHeights) + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_bodyColWidths) + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_rowMemberDefIndexes) + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_colMemberDefIndexes) + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_memberAtLevelIndexes) + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_rowMemberInstanceIndexes) + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_columnHeaders) + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_colHeaderHeights) + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_rowHeaders) + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_rowHeaderWidths) + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_detailRows) + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_cornerCells) + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_columnInfo);
			}
		}

		internal Tablix()
		{
		}

		internal Tablix(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix source, PageContext pageContext)
			: base(source)
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix = (AspNetCore.ReportingServices.OnDemandReportRendering.Tablix)base.m_source;
			TablixInstance tablixInstance = (TablixInstance)source.Instance;
			base.m_itemPageSizes = new ItemSizes(source);
			base.m_pageBreakProperties = PageBreakProperties.Create(tablix.PageBreak, this, pageContext);
			if (!pageContext.IgnorePageBreaks)
			{
				base.m_pageName = tablixInstance.PageName;
			}
			else if (pageContext.Common.DiagnosticsEnabled && tablix.PageBreak.BreakLocation != 0)
			{
				pageContext.Common.TracePageBreakIgnored(this, pageContext.IgnorePageBreaksReason);
			}
			base.KeepTogetherHorizontal = source.KeepTogether;
			base.KeepTogetherVertical = source.KeepTogether;
			bool unresolvedKTV = base.UnresolvedKTH = source.KeepTogether;
			base.UnresolvedKTV = unresolvedKTV;
			bool unresolvedPBE = base.UnresolvedPBS = true;
			base.UnresolvedPBE = unresolvedPBE;
			base.NeedResolve = true;
			this.m_bodyRows = source.Body.RowCollection;
			this.m_bodyRowsHeights = new double[this.m_bodyRows.Count];
			for (int i = 0; i < this.m_bodyRows.Count; i++)
			{
				this.m_bodyRowsHeights[i] = ((ReportElementCollectionBase<TablixRow>)this.m_bodyRows)[i].Height.ToMillimeters();
			}
			TablixColumnCollection columnCollection = source.Body.ColumnCollection;
			this.m_bodyColWidths = new double[columnCollection.Count];
			for (int j = 0; j < columnCollection.Count; j++)
			{
				this.m_bodyColWidths[j] = ((ReportElementCollectionBase<TablixColumn>)columnCollection)[j].Width.ToMillimeters();
			}
			if (source.Body.IgnoreCellPageBreaks)
			{
				this.m_ignoreCellPageBreaks = 1;
			}
			this.m_rowMembersDepth = this.TablixMembersDepthTree(source.RowHierarchy.MemberCollection);
			this.m_colMembersDepth = this.TablixMembersDepthTree(source.ColumnHierarchy.MemberCollection);
			this.NoRows = tablixInstance.NoRows;
			base.FullyCreated = false;
		}

		internal bool CheckPageBreaks(PageContext pageContext)
		{
			if (pageContext.IgnorePageBreaks)
			{
				return false;
			}
			if (this.m_ignoreGroupPageBreaks > 0)
			{
				return false;
			}
			return true;
		}

		private static int RemoveHeadersAbove(List<PageStructMemberCell> members, int endIndex, ScalableList<RowInfo> detailRows, ref int rowEndIndex)
		{
			if (members == null)
			{
				return 0;
			}
			int num = 0;
			int num2 = 0;
			int num3 = rowEndIndex;
			int num4 = endIndex;
			PageStructStaticMemberCell pageStructStaticMemberCell = null;
			while (num4 >= 0)
			{
				pageStructStaticMemberCell = (members[num4] as PageStructStaticMemberCell);
				if (pageStructStaticMemberCell == null)
				{
					break;
				}
				if (!pageStructStaticMemberCell.Header)
				{
					break;
				}
				if (!pageStructStaticMemberCell.RepeatWith)
				{
					break;
				}
				num4--;
				num3 -= pageStructStaticMemberCell.Span;
			}
			num4++;
			RowInfo rowInfo = null;
			for (int i = num3; i < rowEndIndex; i++)
			{
				rowInfo = detailRows[i];
				if (rowInfo.PageVerticalState != RowInfo.VerticalState.Above)
				{
					break;
				}
				rowInfo.DisposeDetailCells();
				num2++;
			}
			if (num2 > 0)
			{
				detailRows.RemoveRange(num3, num2);
				rowEndIndex -= num2;
				int num5 = num4;
				while (num5 <= endIndex)
				{
					pageStructStaticMemberCell = (members[num5] as PageStructStaticMemberCell);
					if (num2 >= pageStructStaticMemberCell.Span)
					{
						num2 -= pageStructStaticMemberCell.Span;
						num++;
						pageStructStaticMemberCell.DisposeInstances();
						num5++;
						continue;
					}
					pageStructStaticMemberCell.Span -= num2;
					break;
				}
				members.RemoveRange(num4, num);
			}
			return num;
		}

		private static double ResolveStartPos(List<SizeInfo> sizeInfoList, double startPos)
		{
			if (sizeInfoList != null && sizeInfoList.Count != 0)
			{
				sizeInfoList[0].StartPos = startPos;
				for (int i = 1; i < sizeInfoList.Count; i++)
				{
					sizeInfoList[i].StartPos = sizeInfoList[i - 1].EndPos;
				}
				return sizeInfoList[sizeInfoList.Count - 1].EndPos;
			}
			return 0.0;
		}

		private static double ResolveStartPosRTL(List<SizeInfo> sizeInfoList, double startPos)
		{
			if (sizeInfoList != null && sizeInfoList.Count != 0)
			{
				sizeInfoList[sizeInfoList.Count - 1].StartPos = startPos;
				for (int num = sizeInfoList.Count - 2; num >= 0; num--)
				{
					sizeInfoList[num].StartPos = sizeInfoList[num + 1].EndPos;
				}
				return sizeInfoList[0].EndPos;
			}
			return 0.0;
		}

		private static double ResolveStartPosAndState(List<SizeInfo> sizeInfoList, double startPos, SizeInfo.PageState state)
		{
			if (sizeInfoList != null && sizeInfoList.Count != 0)
			{
				sizeInfoList[0].StartPos = startPos;
				sizeInfoList[0].State = state;
				for (int i = 1; i < sizeInfoList.Count; i++)
				{
					sizeInfoList[i].StartPos = sizeInfoList[i - 1].EndPos;
					sizeInfoList[i].State = state;
				}
				return sizeInfoList[sizeInfoList.Count - 1].EndPos;
			}
			return 0.0;
		}

		private static double ResolveStartPosAndStateRTL(List<SizeInfo> sizeInfoList, double startPos, SizeInfo.PageState state)
		{
			if (sizeInfoList != null && sizeInfoList.Count != 0)
			{
				sizeInfoList[sizeInfoList.Count - 1].StartPos = startPos;
				sizeInfoList[sizeInfoList.Count - 1].State = state;
				for (int num = sizeInfoList.Count - 2; num >= 0; num--)
				{
					sizeInfoList[num].StartPos = sizeInfoList[num + 1].EndPos;
					sizeInfoList[num].State = state;
				}
				return sizeInfoList[0].EndPos;
			}
			return 0.0;
		}

		private static void ResolveSizes(List<SizeInfo> sizeInfoList)
		{
			if (sizeInfoList != null)
			{
				SizeInfo sizeInfo = null;
				Hashtable hashtable = null;
				double num = 0.0;
				double num2 = 0.0;
				int num3 = 0;
				for (int i = 0; i < sizeInfoList.Count; i++)
				{
					int num4 = 2;
					sizeInfo = sizeInfoList[i];
					hashtable = sizeInfo.SpanSize;
					if (hashtable != null)
					{
						while (hashtable.Count > 0)
						{
							if (hashtable[num4] != null)
							{
								num3 = 0;
								num2 = 0.0;
								num = (double)hashtable[num4];
								for (int j = i; j < i + num4; j++)
								{
									if (sizeInfoList[j] == null || sizeInfoList[j].SizeValue == 0.0)
									{
										num3++;
									}
									else
									{
										num2 += sizeInfoList[j].SizeValue;
									}
								}
								if (num2 < num)
								{
									if (num3 == 0)
									{
										sizeInfoList[i + num4 - 1].SizeValue += num - num2;
									}
									else
									{
										num2 = (num - num2) / (double)num3;
										for (int k = i; k < i + num4; k++)
										{
											if (sizeInfoList[k] == null)
											{
												sizeInfoList[k] = new SizeInfo(num2);
											}
											else if (sizeInfoList[k].SizeValue == 0.0)
											{
												sizeInfoList[k].SizeValue += num2;
											}
										}
									}
								}
								hashtable.Remove(num4);
							}
							num4++;
						}
						sizeInfo.SpanSize = null;
					}
				}
			}
		}

		private static void UpdateSizes(int start, int span, double size, ref List<SizeInfo> sizeInfoList)
		{
			if (span != 0)
			{
				if (sizeInfoList == null)
				{
					sizeInfoList = new List<SizeInfo>();
				}
				while (sizeInfoList.Count <= start + span - 1)
				{
					sizeInfoList.Add(null);
				}
				if (span == 1)
				{
					if (sizeInfoList[start] == null)
					{
						sizeInfoList[start] = new SizeInfo(size);
					}
					else
					{
						sizeInfoList[start].SizeValue = Math.Max(sizeInfoList[start].SizeValue, size);
					}
				}
				else
				{
					if (sizeInfoList[start] == null)
					{
						sizeInfoList[start] = new SizeInfo();
					}
					sizeInfoList[start].AddSpanSize(span, size);
				}
			}
		}

		private static List<ColumnSpan> GetColumnSpans(ScalableList<ColumnInfo> columnInfoList)
		{
			List<ColumnSpan> list = new List<ColumnSpan>();
			ColumnInfo columnInfo = null;
			Hashtable hashtable = null;
			for (int i = 0; i < columnInfoList.Count; i++)
			{
				int num = 2;
				using (columnInfoList.GetAndPin(i, out columnInfo))
				{
					hashtable = columnInfo.SpanSize;
					columnInfo.SpanSize = null;
				}
				if (hashtable != null)
				{
					while (hashtable.Count > 0)
					{
						if (hashtable[num] != null)
						{
							double spanSize = (double)hashtable[num];
							list.Add(new ColumnSpan(i, num, spanSize));
							hashtable.Remove(num);
						}
						num++;
					}
				}
			}
			return list;
		}

		private static void ResolveSizes(ScalableList<ColumnInfo> columnInfoList)
		{
			if (columnInfoList != null)
			{
				List<ColumnSpan> columnSpans = Tablix.GetColumnSpans(columnInfoList);
				while (columnSpans.Count > 0)
				{
					int index = 0;
					ColumnSpan columnSpan = columnSpans[index];
					int num = columnSpan.CalculateEmptyColumnns(columnInfoList);
					for (int i = 1; i < columnSpans.Count; i++)
					{
						if (num == 0)
						{
							break;
						}
						int num2 = columnSpans[i].CalculateEmptyColumnns(columnInfoList);
						if (num2 < num)
						{
							index = i;
							columnSpan = columnSpans[index];
							num = num2;
						}
					}
					ColumnInfo columnInfo = null;
					double num3 = 0.0;
					for (int j = columnSpan.Start; j < columnSpan.Start + columnSpan.Span; j++)
					{
						columnInfo = columnInfoList[j];
						if (columnInfo != null && !columnInfo.Empty)
						{
							num3 += columnInfo.SizeValue;
						}
					}
					if (num3 < columnSpan.SpanSize)
					{
						if (num == 0)
						{
							int num4 = columnSpan.Start + columnSpan.Span - 1;
							while (num4 > columnSpan.Start && columnInfoList[num4].Hidden)
							{
								num4--;
							}
							using (columnInfoList.GetAndPin(num4, out columnInfo))
							{
								columnInfo.SizeValue += columnSpan.SpanSize - num3;
							}
						}
						else
						{
							num3 = (columnSpan.SpanSize - num3) / (double)num;
							for (int k = columnSpan.Start; k < columnSpan.Start + columnSpan.Span; k++)
							{
								using (columnInfoList.GetAndPin(k, out columnInfo))
								{
									if (columnInfo == null)
									{
										columnInfoList[k] = new ColumnInfo(num3);
									}
									else if (columnInfo.Empty)
									{
										columnInfo.SizeValue += num3;
									}
								}
							}
						}
					}
					columnSpans.RemoveAt(index);
				}
			}
		}

		private static void UpdateSizes(int start, int span, double size, bool unresolved, bool keepTogether, bool split, ScalableList<ColumnInfo> columnInfoList)
		{
			if (columnInfoList != null)
			{
				ColumnInfo columnInfo = null;
				if (split)
				{
					int num = 0;
					double num2 = 0.0;
					for (int i = start; i < start + span; i++)
					{
						if (i >= columnInfoList.Count)
						{
							num++;
						}
						else
						{
							columnInfo = columnInfoList[i];
							if (columnInfo == null || columnInfo.Empty)
							{
								num++;
							}
							else
							{
								num2 += columnInfo.SizeValue;
							}
						}
					}
					if (num2 < size)
					{
						if (num == 0)
						{
							int num3 = start + span - 1;
							while (num3 > start && columnInfoList[num3].Hidden)
							{
								num3--;
							}
							using (columnInfoList.GetAndPin(num3, out columnInfo))
							{
								columnInfo.SizeValue += size - num2;
							}
						}
						else
						{
							num2 = (size - num2) / (double)num;
							for (int j = start; j < start + span; j++)
							{
								if (j >= columnInfoList.Count)
								{
									while (columnInfoList.Count < j)
									{
										columnInfoList.Add(new ColumnInfo());
									}
									columnInfoList.Add(new ColumnInfo(num2));
								}
								else
								{
									using (columnInfoList.GetAndPin(j, out columnInfo))
									{
										if (columnInfo == null)
										{
											columnInfoList[j] = new ColumnInfo(num2);
										}
										else if (columnInfo.Empty)
										{
											columnInfo.SizeValue += num2;
										}
									}
								}
							}
						}
					}
				}
				else
				{
					while (columnInfoList.Count <= start + span - 1)
					{
						columnInfoList.Add(new ColumnInfo());
					}
					using (columnInfoList.GetAndPin(start, out columnInfo))
					{
						if (span == 1)
						{
							columnInfo.SizeValue = Math.Max(columnInfo.SizeValue, size);
						}
						else
						{
							columnInfo.AddSpanSize(span, size);
						}
						columnInfo.Unresolved |= unresolved;
						columnInfo.KeepTogether |= keepTogether;
					}
					for (int k = 1; k < span; k++)
					{
						using (columnInfoList.GetAndPin(start + k, out columnInfo))
						{
							columnInfo.BlockedBySpan = true;
							columnInfo.Unresolved |= unresolved;
						}
					}
				}
			}
		}

		private static void UpdateHidden(int start, int span, ScalableList<ColumnInfo> columnInfoList)
		{
			if (columnInfoList != null)
			{
				while (columnInfoList.Count <= start + span - 1)
				{
					columnInfoList.Add(new ColumnInfo());
				}
				ColumnInfo columnInfo = null;
				for (int i = 0; i < span; i++)
				{
					using (columnInfoList.GetAndPin(start + i, out columnInfo))
					{
						columnInfo.Hidden = true;
					}
				}
			}
		}

		internal override void WriteItemSharedStyleProps(BinaryWriter spbifWriter, Style style, PageContext pageContext)
		{
			base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.BackgroundColor, 34);
			base.WriteBackgroundImage(style, true, spbifWriter, pageContext);
		}

		internal override void WriteItemSharedStyleProps(RPLStyleProps rplStyleProps, Style style, PageContext pageContext)
		{
			PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.BackgroundColor, 34);
			base.WriteBackgroundImage(style, true, rplStyleProps, pageContext);
		}

		internal override void WriteItemNonSharedStyleProps(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, StyleAttributeNames styleAtt, PageContext pageContext)
		{
			switch (styleAtt)
			{
			case StyleAttributeNames.BackgroundColor:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BackgroundColor, 34);
				break;
			case StyleAttributeNames.BackgroundImage:
				base.WriteBackgroundImage(styleDef, false, spbifWriter, pageContext);
				break;
			}
		}

		internal override void WriteItemNonSharedStyleProps(RPLStyleProps rplStyleProps, Style styleDef, StyleInstance style, StyleAttributeNames styleAtt, PageContext pageContext)
		{
			switch (styleAtt)
			{
			case StyleAttributeNames.BackgroundColor:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BackgroundColor, 34);
				break;
			case StyleAttributeNames.BackgroundImage:
				base.WriteBackgroundImage(styleDef, false, rplStyleProps, pageContext);
				break;
			}
		}

		internal override void WriteStartItemToStream(RPLWriter rplWriter, PageContext pageContext)
		{
			if (rplWriter != null)
			{
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				if (binaryWriter != null)
				{
					Stream baseStream = binaryWriter.BaseStream;
					base.m_offset = baseStream.Position;
					binaryWriter.Write((byte)13);
					base.WriteElementProps(binaryWriter, rplWriter, pageContext, base.m_offset + 1);
				}
				else if (base.m_rplElement == null)
				{
					base.m_rplElement = new RPLTablix();
					base.WriteElementProps(base.m_rplElement.ElementProps, pageContext);
				}
				else
				{
					RPLItemProps rplElementProps = base.m_rplElement.ElementProps as RPLItemProps;
					base.m_rplElement = new RPLTablix(rplElementProps);
				}
			}
		}

		internal void WriteEndItemToStream(RPLWriter rplWriter, int columnsOnPage, int rowsOnPage, bool hasLabelsOnCH, bool hasLabelsOnRH)
		{
			if (rplWriter != null)
			{
				this.WriteDetailRows(rplWriter, columnsOnPage, rowsOnPage);
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				if (binaryWriter != null)
				{
					Stream baseStream = binaryWriter.BaseStream;
					long position = baseStream.Position;
					binaryWriter.Write((byte)17);
					binaryWriter.Write(base.m_offset);
					this.WriteTablixMeasurements(rplWriter, columnsOnPage, rowsOnPage, hasLabelsOnCH, hasLabelsOnRH);
					binaryWriter.Write((byte)255);
					binaryWriter.Flush();
					base.m_offset = baseStream.Position;
					binaryWriter.Write((byte)254);
					binaryWriter.Write(position);
					binaryWriter.Write((byte)255);
				}
				else
				{
					this.WriteTablixMeasurements(rplWriter, columnsOnPage, rowsOnPage, hasLabelsOnCH, hasLabelsOnRH);
				}
			}
		}

		private void WriteDetailRows(RPLWriter rplWriter, int columnsOnPage, int rowsOnPage)
		{
			if (rplWriter != null && this.m_detailRows != null && columnsOnPage != 0 && rowsOnPage != 0)
			{
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				int i = 0;
				if (this.m_colsBeforeRowHeaders > 0)
				{
					ColumnInfo columnInfo = null;
					for (; i < this.m_colsBeforeRowHeaders; i++)
					{
						columnInfo = this.m_columnInfo[i];
						if (columnInfo.PageHorizontalState == ColumnInfo.HorizontalState.AtRight)
						{
							i = this.m_columnInfo.Count;
							break;
						}
						if (columnInfo.PageHorizontalState == ColumnInfo.HorizontalState.Normal && !columnInfo.Hidden)
						{
							num++;
						}
					}
				}
				if (this.AddToPageColumnHeaders && this.m_colHeaderHeights != null)
				{
					for (int j = 0; j < this.m_colHeaderHeights.Count; j++)
					{
						if (this.m_colHeaderHeights[j].State == SizeInfo.PageState.Normal)
						{
							num2++;
						}
					}
				}
				if (this.AddToPageRowHeaders && this.m_rowHeaderWidths != null)
				{
					for (int k = 0; k < this.m_rowHeaderWidths.Count; k++)
					{
						if (this.m_rowHeaderWidths[k].State == SizeInfo.PageState.Normal)
						{
							num3++;
						}
					}
				}
				int num4 = num2;
				RowInfo rowInfo = null;
				for (int l = 0; l < this.m_detailRows.Count; l++)
				{
					using (this.m_detailRows.GetAndPin(l, out rowInfo))
					{
						num4 += rowInfo.AddToPage(this.m_columnInfo, rplWriter, num4, num, num3);
					}
				}
			}
		}

		internal void WriteTablixMeasurements(RPLWriter rplWriter, int columnsOnPage, int rowsOnPage, bool hasLabelsOnCH, bool hasLabelsOnRH)
		{
			if (rplWriter != null)
			{
				if (columnsOnPage == 0 && rowsOnPage == 0)
				{
					return;
				}
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				RPLTablix rPLTablix = base.m_rplElement as RPLTablix;
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				double num5 = 0.0;
				double num6 = 0.0;
				if (columnsOnPage > 0)
				{
					if (binaryWriter != null)
					{
						binaryWriter.Write((byte)4);
						binaryWriter.Write(columnsOnPage);
					}
					else
					{
						rPLTablix = (base.m_rplElement as RPLTablix);
						rPLTablix.ColumnWidths = new float[columnsOnPage];
					}
					int i = 0;
					ColumnInfo columnInfo = null;
					if (this.m_colsBeforeRowHeaders > 0)
					{
						for (; i < this.m_colsBeforeRowHeaders; i++)
						{
							columnInfo = this.m_columnInfo[i];
							if (columnInfo.PageHorizontalState == ColumnInfo.HorizontalState.AtRight)
							{
								i = this.m_columnInfo.Count;
								break;
							}
							if (columnInfo.PageHorizontalState == ColumnInfo.HorizontalState.Normal && !columnInfo.Hidden)
							{
								if (num4 == 0)
								{
									num6 = columnInfo.Left;
								}
								if (binaryWriter != null)
								{
									binaryWriter.Write((float)columnInfo.SizeValue);
									binaryWriter.Write(false);
								}
								else
								{
									rPLTablix.ColumnWidths[num4] = (float)columnInfo.SizeValue;
								}
								num4++;
								num++;
							}
						}
					}
					if (this.AddToPageRowHeaders && this.m_rowHeaderWidths != null)
					{
						if (this.IsLTR)
						{
							for (int j = 0; j < this.m_rowHeaderWidths.Count; j++)
							{
								if (this.m_rowHeaderWidths[j].State == SizeInfo.PageState.Normal)
								{
									if (num4 == 0)
									{
										num6 = this.m_rowHeaderWidths[j].StartPos;
									}
									if (binaryWriter != null)
									{
										binaryWriter.Write((float)this.m_rowHeaderWidths[j].SizeValue);
										binaryWriter.Write(false);
									}
									else
									{
										rPLTablix.ColumnWidths[num4] = (float)this.m_rowHeaderWidths[j].SizeValue;
									}
									num4++;
									num3++;
								}
							}
						}
						else
						{
							for (int num7 = this.m_rowHeaderWidths.Count - 1; num7 >= 0; num7--)
							{
								if (this.m_rowHeaderWidths[num7].State == SizeInfo.PageState.Normal)
								{
									if (num4 == 0)
									{
										num6 = this.m_rowHeaderWidths[num7].StartPos;
									}
									if (binaryWriter != null)
									{
										binaryWriter.Write((float)this.m_rowHeaderWidths[num7].SizeValue);
										binaryWriter.Write(false);
									}
									else
									{
										rPLTablix.ColumnWidths[num4] = (float)this.m_rowHeaderWidths[num7].SizeValue;
									}
									num4++;
									num3++;
								}
							}
						}
					}
					for (; i < this.m_columnInfo.Count; i++)
					{
						columnInfo = this.m_columnInfo[i];
						if (columnInfo.PageHorizontalState == ColumnInfo.HorizontalState.AtRight)
						{
							i = this.m_columnInfo.Count;
							break;
						}
						if (columnInfo.PageHorizontalState == ColumnInfo.HorizontalState.Normal && !columnInfo.Hidden)
						{
							if (num4 == 0)
							{
								num6 = columnInfo.Left;
							}
							if (binaryWriter != null)
							{
								binaryWriter.Write((float)columnInfo.SizeValue);
								binaryWriter.Write(false);
							}
							else
							{
								rPLTablix.ColumnWidths[num4] = (float)columnInfo.SizeValue;
							}
							num4++;
						}
					}
				}
				if (rowsOnPage > 0)
				{
					num4 = 0;
					if (binaryWriter != null)
					{
						binaryWriter.Write((byte)5);
						binaryWriter.Write(rowsOnPage);
					}
					else
					{
						rPLTablix.RowHeights = new float[rowsOnPage];
					}
					if (this.AddToPageColumnHeaders && this.m_colHeaderHeights != null)
					{
						for (int k = 0; k < this.m_colHeaderHeights.Count; k++)
						{
							if (this.m_colHeaderHeights[k].State == SizeInfo.PageState.Normal)
							{
								if (num4 == 0)
								{
									num5 = this.m_colHeaderHeights[k].StartPos;
								}
								if (binaryWriter != null)
								{
									binaryWriter.Write((float)this.m_colHeaderHeights[k].SizeValue);
									binaryWriter.Write(false);
								}
								else
								{
									rPLTablix.RowHeights[num4] = (float)this.m_colHeaderHeights[k].SizeValue;
								}
								num4++;
								num2++;
							}
						}
					}
					if (this.m_detailRows != null)
					{
						RowInfo rowInfo = null;
						for (int l = 0; l < this.m_detailRows.Count; l++)
						{
							rowInfo = this.m_detailRows[l];
							if (rowInfo.PageVerticalState == RowInfo.VerticalState.Normal && !rowInfo.Hidden)
							{
								if (num4 == 0)
								{
									num5 = rowInfo.Top;
								}
								if (binaryWriter != null)
								{
									binaryWriter.Write((float)rowInfo.Height);
									binaryWriter.Write(false);
								}
								else
								{
									rPLTablix.RowHeights[num4] = (float)rowInfo.Height;
								}
								num4++;
							}
						}
					}
				}
				if (binaryWriter != null)
				{
					binaryWriter.Write((byte)0);
					binaryWriter.Write(num2);
					binaryWriter.Write((byte)1);
					binaryWriter.Write(num3);
					binaryWriter.Write((byte)2);
					binaryWriter.Write(num);
					if (num5 > 0.0)
					{
						binaryWriter.Write((byte)6);
						binaryWriter.Write((float)num5);
					}
					if (num6 > 0.0)
					{
						binaryWriter.Write((byte)7);
						binaryWriter.Write((float)num6);
					}
					if (!this.IsLTR)
					{
						binaryWriter.Write((byte)3);
						binaryWriter.Write((byte)1);
					}
				}
				else
				{
					rPLTablix.ColumnHeaderRows = num2;
					rPLTablix.RowHeaderColumns = num3;
					rPLTablix.ColsBeforeRowHeaders = num;
					rPLTablix.ContentTop = (float)num5;
					rPLTablix.ContentLeft = (float)num6;
					if (!this.IsLTR)
					{
						rPLTablix.LayoutDirection = RPLFormat.Directions.RTL;
					}
				}
				if (binaryWriter != null)
				{
					this.WriteTablixMemberDefList(binaryWriter, this.m_rowMemberDefList, TablixRegion.RowHeader);
					this.WriteTablixMemberDefList(binaryWriter, this.m_colMemberDefList, TablixRegion.ColumnHeader);
				}
				this.WriteTablixContent(rplWriter, num, num3, num2, hasLabelsOnCH, hasLabelsOnRH);
			}
		}

		private void WriteTablixMemberDefList(BinaryWriter spbifWriter, List<RPLTablixMemberDef> membersDefList, TablixRegion region)
		{
			if (membersDefList != null && membersDefList.Count != 0)
			{
				if (region == TablixRegion.RowHeader)
				{
					spbifWriter.Write((byte)14);
				}
				else
				{
					spbifWriter.Write((byte)15);
				}
				spbifWriter.Write(membersDefList.Count);
				RPLTablixMemberDef rPLTablixMemberDef = null;
				for (int i = 0; i < membersDefList.Count; i++)
				{
					rPLTablixMemberDef = membersDefList[i];
					spbifWriter.Write((byte)16);
					if (rPLTablixMemberDef.DefinitionPath != null)
					{
						spbifWriter.Write((byte)0);
						spbifWriter.Write(rPLTablixMemberDef.DefinitionPath);
					}
					if (rPLTablixMemberDef.Level > 0)
					{
						spbifWriter.Write((byte)1);
						spbifWriter.Write(rPLTablixMemberDef.Level);
					}
					if (rPLTablixMemberDef.MemberCellIndex > 0)
					{
						spbifWriter.Write((byte)2);
						spbifWriter.Write(rPLTablixMemberDef.MemberCellIndex);
					}
					if (rPLTablixMemberDef.State > 0)
					{
						spbifWriter.Write((byte)3);
						spbifWriter.Write(rPLTablixMemberDef.State);
					}
					spbifWriter.Write((byte)255);
				}
			}
		}

		private void OpenDetailRow(RPLWriter rplWriter, int rowIndex, bool newRow)
		{
			BinaryWriter binaryWriter = rplWriter.BinaryWriter;
			RowInfo rowInfo = this.m_detailRows[rowIndex];
			if (binaryWriter != null)
			{
				if (newRow)
				{
					binaryWriter.Write((byte)8);
				}
				if (rowInfo.Offset >= 0)
				{
					binaryWriter.Write((byte)9);
					binaryWriter.Write(rowInfo.Offset);
				}
			}
			else
			{
				if (newRow)
				{
					RPLTablixFullRow rPLTablixFullRow = new RPLTablixFullRow(-1, 0);
					((RPLTablix)base.m_rplElement).AddRow(rPLTablixFullRow);
					rplWriter.TablixRow = rPLTablixFullRow;
				}
				if (rowInfo.RPLTablixRow != null)
				{
					rplWriter.TablixRow.SetBodyStart();
					rplWriter.TablixRow.AddCells(rowInfo.RPLTablixRow.RowCells);
					using (this.m_detailRows.GetAndPin(rowIndex, out rowInfo))
					{
						rowInfo.RPLTablixRow = null;
					}
				}
			}
		}

		private void CloseRow(RPLWriter rplWriter)
		{
			BinaryWriter binaryWriter = rplWriter.BinaryWriter;
			if (binaryWriter != null)
			{
				binaryWriter.Write((byte)255);
			}
		}

		private void OpenHeaderRow(RPLWriter rplWriter, bool omittedRow, int headerStart)
		{
			BinaryWriter binaryWriter = rplWriter.BinaryWriter;
			if (binaryWriter != null)
			{
				binaryWriter.Write((byte)8);
			}
			else
			{
				RPLTablixRow rPLTablixRow = null;
				rPLTablixRow = ((!omittedRow) ? ((RPLTablixRow)new RPLTablixFullRow(headerStart, -1)) : ((RPLTablixRow)new RPLTablixOmittedRow()));
				((RPLTablix)base.m_rplElement).AddRow(rPLTablixRow);
				rplWriter.TablixRow = rPLTablixRow;
			}
		}

		private void WriteMemberToStream(RPLWriter rplWriter, PageStructMemberCell structMember, PageMemberCell memberCell, int rowIndex, int colIndex, int rowSpan, int colSpan, TablixRegion region)
		{
			BinaryWriter binaryWriter = rplWriter.BinaryWriter;
			if (binaryWriter != null)
			{
				if (region == TablixRegion.ColumnHeader)
				{
					binaryWriter.Write((byte)11);
				}
				else
				{
					binaryWriter.Write((byte)12);
				}
				if (memberCell.MemberItem != null && memberCell.ContentOnPage)
				{
					binaryWriter.Write((byte)4);
					binaryWriter.Write(memberCell.MemberItem.Offset);
					if (memberCell.MemberItem.RplItemState > 0)
					{
						binaryWriter.Write((byte)13);
						binaryWriter.Write(memberCell.MemberItem.RplItemState);
					}
					if (memberCell.MemberItem.ItemPageSizes.Top != 0.0)
					{
						binaryWriter.Write((byte)0);
						binaryWriter.Write((float)memberCell.MemberItem.ItemPageSizes.Top);
					}
					if (memberCell.MemberItem.ItemPageSizes.Left != 0.0)
					{
						binaryWriter.Write((byte)1);
						binaryWriter.Write((float)memberCell.MemberItem.ItemPageSizes.Left);
					}
				}
				if (colSpan != 1)
				{
					binaryWriter.Write((byte)5);
					binaryWriter.Write(colSpan);
				}
				if (rowSpan != 1)
				{
					binaryWriter.Write((byte)6);
					binaryWriter.Write(rowSpan);
				}
				if (structMember.MemberDefIndex >= 0)
				{
					binaryWriter.Write((byte)7);
					binaryWriter.Write(structMember.MemberDefIndex);
				}
				binaryWriter.Write((byte)9);
				binaryWriter.Write(rowIndex);
				binaryWriter.Write((byte)8);
				binaryWriter.Write(colIndex);
				if (memberCell.UniqueName != null)
				{
					binaryWriter.Write((byte)11);
					binaryWriter.Write(memberCell.UniqueName);
				}
				if (memberCell.Label != null)
				{
					binaryWriter.Write((byte)10);
					binaryWriter.Write(memberCell.Label);
				}
				binaryWriter.Write((byte)255);
			}
			else
			{
				RPLTablixMemberCell rPLTablixMemberCell = null;
				if (memberCell.MemberItem == null || !memberCell.ContentOnPage)
				{
					rPLTablixMemberCell = new RPLTablixMemberCell(null, 0, rowSpan, colSpan);
				}
				else
				{
					RPLItem element = memberCell.MemberItem.RPLElement as RPLItem;
					rPLTablixMemberCell = new RPLTablixMemberCell(element, memberCell.MemberItem.RplItemState, rowSpan, colSpan);
					if (memberCell.MemberItem.ItemPageSizes.Top != 0.0)
					{
						rPLTablixMemberCell.ContentSizes = new RPLSizes();
						rPLTablixMemberCell.ContentSizes.Top = (float)memberCell.MemberItem.ItemPageSizes.Top;
					}
					if (memberCell.MemberItem.ItemPageSizes.Left != 0.0)
					{
						if (rPLTablixMemberCell.ContentSizes == null)
						{
							rPLTablixMemberCell.ContentSizes = new RPLSizes();
						}
						rPLTablixMemberCell.ContentSizes.Left = (float)memberCell.MemberItem.ItemPageSizes.Left;
					}
				}
				rPLTablixMemberCell.RowIndex = rowIndex;
				rPLTablixMemberCell.ColIndex = colIndex;
				if (region == TablixRegion.RowHeader)
				{
					rPLTablixMemberCell.TablixMemberDef = this.m_rowMemberDefList[structMember.MemberDefIndex];
				}
				else
				{
					rPLTablixMemberCell.TablixMemberDef = this.m_colMemberDefList[structMember.MemberDefIndex];
				}
				rPLTablixMemberCell.UniqueName = memberCell.UniqueName;
				rPLTablixMemberCell.GroupLabel = memberCell.Label;
				if (rPLTablixMemberCell.ColSpan == 0 || rPLTablixMemberCell.RowSpan == 0)
				{
					rplWriter.TablixRow.AddOmittedHeader(rPLTablixMemberCell);
				}
				else
				{
					rplWriter.TablixRow.SetHeaderStart();
					rplWriter.TablixRow.RowCells.Add(rPLTablixMemberCell);
				}
			}
		}

		private void WriteCornerCellToStream(RPLWriter rplWriter, PageCornerCell cornerCell, int rowIndex, int colIndex, int rowSpan, int colSpan)
		{
			BinaryWriter binaryWriter = rplWriter.BinaryWriter;
			if (binaryWriter != null)
			{
				binaryWriter.Write((byte)10);
				if (cornerCell.CellItem != null && cornerCell.ContentOnPage)
				{
					binaryWriter.Write((byte)4);
					binaryWriter.Write(cornerCell.CellItem.Offset);
					if (cornerCell.CellItem.RplItemState > 0)
					{
						binaryWriter.Write((byte)13);
						binaryWriter.Write(cornerCell.CellItem.RplItemState);
					}
					if (cornerCell.CellItem.ItemPageSizes.Top != 0.0)
					{
						binaryWriter.Write((byte)0);
						binaryWriter.Write((float)cornerCell.CellItem.ItemPageSizes.Top);
					}
					if (cornerCell.CellItem.ItemPageSizes.Left != 0.0)
					{
						binaryWriter.Write((byte)1);
						binaryWriter.Write((float)cornerCell.CellItem.ItemPageSizes.Left);
					}
				}
				if (colSpan != 1)
				{
					binaryWriter.Write((byte)5);
					binaryWriter.Write(colSpan);
				}
				if (rowSpan != 1)
				{
					binaryWriter.Write((byte)6);
					binaryWriter.Write(rowSpan);
				}
				binaryWriter.Write((byte)9);
				binaryWriter.Write(rowIndex);
				binaryWriter.Write((byte)8);
				binaryWriter.Write(colIndex);
				binaryWriter.Write((byte)255);
			}
			else
			{
				RPLTablixCornerCell rPLTablixCornerCell = null;
				if (cornerCell.CellItem != null && cornerCell.ContentOnPage)
				{
					RPLItem element = cornerCell.CellItem.RPLElement as RPLItem;
					rPLTablixCornerCell = new RPLTablixCornerCell(element, cornerCell.CellItem.RplItemState, rowSpan, colSpan);
					if (cornerCell.CellItem.ItemPageSizes.Top != 0.0)
					{
						rPLTablixCornerCell.ContentSizes = new RPLSizes();
						rPLTablixCornerCell.ContentSizes.Top = (float)cornerCell.CellItem.ItemPageSizes.Top;
					}
					if (cornerCell.CellItem.ItemPageSizes.Left != 0.0)
					{
						if (rPLTablixCornerCell.ContentSizes == null)
						{
							rPLTablixCornerCell.ContentSizes = new RPLSizes();
						}
						rPLTablixCornerCell.ContentSizes.Left = (float)cornerCell.CellItem.ItemPageSizes.Left;
					}
				}
				else
				{
					rPLTablixCornerCell = new RPLTablixCornerCell(null, 0, rowSpan, colSpan);
				}
				rPLTablixCornerCell.RowIndex = rowIndex;
				rPLTablixCornerCell.ColIndex = colIndex;
				rplWriter.TablixRow.RowCells.Add(rPLTablixCornerCell);
			}
		}

		private void WriteTablixContent(RPLWriter rplWriter, int colsBeforeRH, int headerRowCols, int headerColumnRows, bool hasLabelsOnCH, bool hasLabelsOnRH)
		{
			if (this.m_columnHeaders != null)
			{
				this.WriteCornerAndColumnMembers(rplWriter, colsBeforeRH, headerRowCols, headerColumnRows, hasLabelsOnCH);
			}
			else
			{
				this.WriteCornerOnly(rplWriter, headerRowCols, headerColumnRows);
			}
			if (this.AddToPageRowHeaders && (headerRowCols > 0 || hasLabelsOnRH) && this.m_rowHeaders != null)
			{
				int num = headerColumnRows;
				int num2 = 0;
				bool flag = true;
				if (this.IsLTR)
				{
					for (int i = 0; i < this.m_rowHeaders.Count; i++)
					{
						this.WriteRowMembersLTR(this.m_rowHeaders[i], num2, 0, rplWriter, ref num, colsBeforeRH, ref flag);
						num2 += this.m_rowHeaders[i].Span;
					}
				}
				else
				{
					int[] state = new int[this.m_rowMembersDepth + 1];
					int num3 = 0;
					int num4 = colsBeforeRH;
					for (int j = 0; j < this.m_rowHeaders.Count; j++)
					{
						this.WriteRowMembersRTL(this.m_rowHeaders[j], num2, 0, ref num3, ref flag, rplWriter, ref num, ref num4, state, 0, colsBeforeRH);
						num2 += this.m_rowHeaders[j].Span;
						num3 = num2;
					}
				}
			}
			else
			{
				this.WriteDetailOffsetRows(rplWriter);
			}
		}

		private void WriteDetailOffsetRows(RPLWriter rplWriter)
		{
			if (this.m_detailRows != null)
			{
				RowInfo rowInfo = null;
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				RPLTablix rPLTablix = base.m_rplElement as RPLTablix;
				for (int i = 0; i < this.m_detailRows.Count; i++)
				{
					rowInfo = this.m_detailRows[i];
					if (rowInfo.PageVerticalState == RowInfo.VerticalState.Normal && !rowInfo.Hidden)
					{
						if (binaryWriter != null)
						{
							binaryWriter.Write((byte)8);
							if (rowInfo.Offset >= 0)
							{
								binaryWriter.Write((byte)9);
								binaryWriter.Write(rowInfo.Offset);
							}
							binaryWriter.Write((byte)255);
						}
						else
						{
							rPLTablix.AddRow(rowInfo.RPLTablixRow);
							using (this.m_detailRows.GetAndPin(i, out rowInfo))
							{
								rowInfo.RPLTablixRow = null;
							}
						}
					}
				}
			}
		}

		private bool NeedWrite(PageMemberCell memberCell, int memberDefIndex, TablixRegion region)
		{
			if (memberCell == null)
			{
				return false;
			}
			if (memberCell.Label != null)
			{
				return true;
			}
			if (region == TablixRegion.RowHeader)
			{
				return this.m_rowMemberDefList[memberDefIndex].StaticHeadersTree;
			}
			return this.m_colMemberDefList[memberDefIndex].StaticHeadersTree;
		}

		private void WriteRowMembersLTR(PageStructMemberCell structMember, int rowIndex, int colIndex, RPLWriter rplWriter, ref int pageRowIndex, int pageColIndex, ref bool newRow)
		{
			PageStructStaticMemberCell pageStructStaticMemberCell = structMember as PageStructStaticMemberCell;
			if (pageStructStaticMemberCell != null)
			{
				this.WriteRowMembersLTR(pageStructStaticMemberCell.MemberInstance, (PageStructMemberCell)pageStructStaticMemberCell, rowIndex, colIndex, rplWriter, ref pageRowIndex, pageColIndex, ref newRow);
			}
			else
			{
				PageStructDynamicMemberCell pageStructDynamicMemberCell = structMember as PageStructDynamicMemberCell;
				if (pageStructDynamicMemberCell.MemberInstances != null)
				{
					PageMemberCell pageMemberCell = null;
					for (int i = 0; i < pageStructDynamicMemberCell.MemberInstances.Count; i++)
					{
						if (this.m_detailRows[rowIndex].PageVerticalState == RowInfo.VerticalState.Below)
						{
							break;
						}
						pageMemberCell = pageStructDynamicMemberCell.MemberInstances[i];
						this.WriteRowMembersLTR(pageMemberCell, (PageStructMemberCell)pageStructDynamicMemberCell, rowIndex, colIndex, rplWriter, ref pageRowIndex, pageColIndex, ref newRow);
						rowIndex += pageMemberCell.RowSpan;
					}
				}
			}
		}

		private void WriteRowMembersLTR(PageMemberCell memberCell, PageStructMemberCell structMember, int rowIndex, int colIndex, RPLWriter rplWriter, ref int pageRowIndex, int pageColIndex, ref bool newRow)
		{
			if (memberCell != null && !memberCell.Hidden && memberCell.RHOnVerticalPage(this.m_detailRows, rowIndex))
			{
				int num = 0;
				int num2 = 0;
				if (memberCell.RHColsOnHorizontalPage(this.m_rowHeaderWidths, colIndex, out num) && (num > 0 || this.NeedWrite(memberCell, structMember.MemberDefIndex, TablixRegion.RowHeader)))
				{
					RowInfo rowInfo = null;
					for (int i = 0; i < memberCell.RowSpan; i++)
					{
						rowInfo = this.m_detailRows[rowIndex + i];
						if (rowInfo.PageVerticalState == RowInfo.VerticalState.Normal && !rowInfo.Hidden)
						{
							num2++;
						}
					}
					if (newRow)
					{
						this.OpenHeaderRow(rplWriter, false, -1);
						newRow = false;
					}
					this.WriteMemberToStream(rplWriter, structMember, memberCell, pageRowIndex, pageColIndex, num2, num, TablixRegion.RowHeader);
					pageColIndex += num;
				}
				if (memberCell.Children == null)
				{
					this.OpenDetailRow(rplWriter, rowIndex, newRow);
					this.CloseRow(rplWriter);
					pageRowIndex++;
					newRow = true;
					for (int j = 1; j < memberCell.RowSpan; j++)
					{
						if (this.m_detailRows[rowIndex + j].PageVerticalState == RowInfo.VerticalState.Normal)
						{
							this.OpenDetailRow(rplWriter, rowIndex + j, newRow);
							this.CloseRow(rplWriter);
							pageRowIndex++;
						}
					}
				}
				else
				{
					for (int k = 0; k < memberCell.Children.Count; k++)
					{
						this.WriteRowMembersLTR(memberCell.Children[k], rowIndex, colIndex + memberCell.ColSpan, rplWriter, ref pageRowIndex, pageColIndex, ref newRow);
						rowIndex += memberCell.Children[k].Span;
					}
				}
			}
		}

		private void WriteRowMembersRTL(PageStructMemberCell structMember, int rowIndex, int colIndex, ref int targetRow, ref bool newRow, RPLWriter rplWriter, ref int pageRowIndex, ref int pageColIndex, int[] state, int stateIndex, int startColIndex)
		{
			PageStructStaticMemberCell pageStructStaticMemberCell = structMember as PageStructStaticMemberCell;
			bool flag = false;
			if (pageStructStaticMemberCell != null)
			{
				flag = this.WriteRowMembersRTL(pageStructStaticMemberCell.MemberInstance, (PageStructMemberCell)pageStructStaticMemberCell, rowIndex, colIndex, ref targetRow, ref newRow, rplWriter, ref pageRowIndex, ref pageColIndex, state, stateIndex, startColIndex);
				this.CloseRTLRow(stateIndex, ref newRow, ref pageRowIndex, rplWriter);
				if (flag)
				{
					rowIndex += pageStructStaticMemberCell.Span;
					targetRow = rowIndex;
				}
			}
			else
			{
				PageStructDynamicMemberCell pageStructDynamicMemberCell = structMember as PageStructDynamicMemberCell;
				if (pageStructDynamicMemberCell.MemberInstances != null)
				{
					PageMemberCell pageMemberCell = null;
					for (int i = 0; i < pageStructDynamicMemberCell.MemberInstances.Count; i++)
					{
						pageMemberCell = pageStructDynamicMemberCell.MemberInstances[i];
						if (rowIndex + pageMemberCell.RowSpan <= targetRow)
						{
							rowIndex += pageMemberCell.RowSpan;
							continue;
						}
						if (this.m_detailRows[rowIndex].PageVerticalState == RowInfo.VerticalState.Below)
						{
							break;
						}
						flag = this.WriteRowMembersRTL(pageMemberCell, (PageStructMemberCell)pageStructDynamicMemberCell, rowIndex, colIndex, ref targetRow, ref newRow, rplWriter, ref pageRowIndex, ref pageColIndex, state, stateIndex, startColIndex);
						this.CloseRTLRow(stateIndex, ref newRow, ref pageRowIndex, rplWriter);
						if (!flag)
						{
							break;
						}
						rowIndex += pageMemberCell.RowSpan;
						targetRow = rowIndex;
					}
				}
			}
		}

		private bool WriteRowMembersRTL(PageMemberCell memberCell, PageStructMemberCell structMember, int rowIndex, int colIndex, ref int targetRow, ref bool newRow, RPLWriter rplWriter, ref int pageRowIndex, ref int pageColIndex, int[] state, int stateIndex, int startColIndex)
		{
			if (memberCell != null && !memberCell.Hidden)
			{
				if (!memberCell.RHOnVerticalPage(this.m_detailRows, rowIndex))
				{
					newRow = false;
					return true;
				}
				int num = -1;
				int num2 = 0;
				int num3 = 0;
				bool flag = false;
				if (state[stateIndex] == 0)
				{
					flag = true;
					RowInfo rowInfo = null;
					for (int i = 0; i < memberCell.RowSpan; i++)
					{
						rowInfo = this.m_detailRows[rowIndex + i];
						if (rowInfo.PageVerticalState == RowInfo.VerticalState.Normal && !rowInfo.Hidden)
						{
							if (num < 0)
							{
								num = rowIndex + i;
								state[stateIndex] = rowIndex + i;
							}
							num2++;
						}
					}
				}
				if (flag)
				{
					if (memberCell.RHColsOnHorizontalPage(this.m_rowHeaderWidths, colIndex, out num3))
					{
						if (num3 <= 0 && !this.NeedWrite(memberCell, structMember.MemberDefIndex, TablixRegion.RowHeader))
						{
							flag = false;
						}
					}
					else
					{
						flag = false;
					}
				}
				if (memberCell.Children == null)
				{
					this.OpenDetailRow(rplWriter, rowIndex, true);
					pageColIndex = startColIndex;
					newRow = true;
					if (flag)
					{
						this.WriteMemberToStream(rplWriter, structMember, memberCell, pageRowIndex, pageColIndex, num2, num3, TablixRegion.RowHeader);
						pageColIndex += num3;
					}
				}
				else
				{
					int num4 = rowIndex + memberCell.RowSpan - 1;
					int num5 = 0;
					for (int j = 0; j < memberCell.Children.Count; j++)
					{
						num5 = rowIndex + memberCell.Children[j].Span - 1;
						if (num5 < targetRow)
						{
							rowIndex += memberCell.Children[j].Span;
						}
						else
						{
							if (memberCell.ColSpan == 0)
							{
								this.WriteRowMembersRTL(memberCell.Children[j], rowIndex, colIndex, ref targetRow, ref newRow, rplWriter, ref pageRowIndex, ref pageColIndex, state, stateIndex + 1, startColIndex);
							}
							else
							{
								this.WriteRowMembersRTL(memberCell.Children[j], rowIndex, colIndex + memberCell.ColSpan, ref targetRow, ref newRow, rplWriter, ref pageRowIndex, ref pageColIndex, state, stateIndex + memberCell.ColSpan, startColIndex);
							}
							if (flag)
							{
								this.WriteMemberToStream(rplWriter, structMember, memberCell, pageRowIndex, pageColIndex, num2, num3, TablixRegion.RowHeader);
								pageColIndex += num3;
								flag = false;
							}
							state[stateIndex]++;
							if (stateIndex == 0)
							{
								this.CloseRTLRow(ref newRow, ref pageRowIndex, rplWriter);
								if (targetRow < num5)
								{
									j--;
									targetRow++;
								}
								else
								{
									rowIndex += memberCell.Children[j].Span;
									targetRow = rowIndex;
								}
							}
							else if (targetRow < num4)
							{
								return false;
							}
						}
					}
				}
				state[stateIndex] = 0;
				return stateIndex == 0;
			}
			return true;
		}

		private void CloseRTLRow(int stateIndex, ref bool newRow, ref int pageRowIndex, RPLWriter rplWriter)
		{
			if (stateIndex == 0)
			{
				this.CloseRTLRow(ref newRow, ref pageRowIndex, rplWriter);
			}
		}

		private void CloseRTLRow(ref bool newRow, ref int pageRowIndex, RPLWriter rplWriter)
		{
			if (newRow)
			{
				this.CloseRow(rplWriter);
				pageRowIndex++;
				newRow = false;
			}
		}

		private void WriteCornerAndColumnMembers(RPLWriter rplWriter, int colsBeforeRH, int headerRowCols, int headerColumnRows, bool hasLabelsOnCH)
		{
			if (this.AddToPageColumnHeaders)
			{
				if (headerColumnRows == 0 && !hasLabelsOnCH)
				{
					return;
				}
				int num = 0;
				int i = 0;
				int rowIndex = 0;
				int num2 = 0;
				for (int j = 0; j < this.m_columnHeaders.Count; j++)
				{
					num2 += this.m_columnHeaders[j].Span;
				}
				bool flag = false;
				int num3 = 0;
				int num4 = 0;
				PageStructMemberCell pageStructMemberCell = null;
				int num5 = 0;
				int num6 = -1;
				int num7 = 0;
				while (num7 < this.m_columnHeaders.Count)
				{
					if (!this.m_columnHeaders[num7].HasOmittedChildren)
					{
						num7++;
						continue;
					}
					flag = true;
					break;
				}
				for (; i < this.m_headerColumnRows; i++)
				{
					if (this.m_colHeaderHeights[i].State == SizeInfo.PageState.Normal)
					{
						num = 0;
						num4 = 0;
						num5 = 0;
						num6 = -1;
						if (flag)
						{
							this.WriteOmittedColHeadersRows(rplWriter, rowIndex, num3, headerRowCols);
							flag = false;
						}
						this.OpenHeaderRow(rplWriter, false, 0);
						if (this.m_colsBeforeRowHeaders > 0)
						{
							while (num5 < this.m_columnHeaders.Count && num < this.m_colsBeforeRowHeaders)
							{
								pageStructMemberCell = this.m_columnHeaders[num5];
								if (num + pageStructMemberCell.Span <= this.m_colsBeforeRowHeaders)
								{
									this.WriteColMembers(pageStructMemberCell, i, 0, num, rplWriter, num3, ref num4, ref flag);
									num += pageStructMemberCell.Span;
									num5++;
									continue;
								}
								PageStructDynamicMemberCell pageStructDynamicMemberCell = pageStructMemberCell as PageStructDynamicMemberCell;
								PageMemberCell pageMemberCell = null;
								int num8 = 0;
								while (num8 < pageStructDynamicMemberCell.MemberInstances.Count)
								{
									if (this.m_columnInfo[num].PageHorizontalState != ColumnInfo.HorizontalState.AtRight)
									{
										pageMemberCell = pageStructDynamicMemberCell.MemberInstances[num8];
										this.WriteColMembers(pageMemberCell, pageStructDynamicMemberCell, i, 0, num, rplWriter, num3, ref num4, ref flag);
										num += pageMemberCell.ColSpan;
										if (num >= this.m_colsBeforeRowHeaders)
										{
											num6 = num8 + 1;
											break;
										}
										num8++;
										continue;
									}
									num6 = pageStructDynamicMemberCell.MemberInstances.Count;
									break;
								}
								break;
							}
						}
						if (headerRowCols > 0)
						{
							this.WriteCornerCells(i, rplWriter, num3, num4);
							num4 += headerRowCols;
						}
						for (; num5 < this.m_columnHeaders.Count; num5++)
						{
							pageStructMemberCell = this.m_columnHeaders[num5];
							if (num6 >= 0)
							{
								PageStructDynamicMemberCell pageStructDynamicMemberCell2 = pageStructMemberCell as PageStructDynamicMemberCell;
								PageMemberCell pageMemberCell2 = null;
								int num9 = num6;
								while (num9 < pageStructDynamicMemberCell2.MemberInstances.Count)
								{
									if (this.m_columnInfo[num].PageHorizontalState != ColumnInfo.HorizontalState.AtRight)
									{
										pageMemberCell2 = pageStructDynamicMemberCell2.MemberInstances[num9];
										this.WriteColMembers(pageMemberCell2, pageStructDynamicMemberCell2, i, 0, num, rplWriter, num3, ref num4, ref flag);
										num += pageMemberCell2.ColSpan;
										num9++;
										continue;
									}
									num6 = pageStructDynamicMemberCell2.MemberInstances.Count;
									break;
								}
								num6 = -1;
							}
							else
							{
								this.WriteColMembers(pageStructMemberCell, i, 0, num, rplWriter, num3, ref num4, ref flag);
								num += pageStructMemberCell.Span;
							}
						}
						this.CloseRow(rplWriter);
						num3++;
						rowIndex = i + 1;
					}
				}
				if (flag)
				{
					this.WriteOmittedColHeadersRows(rplWriter, rowIndex, num3, headerRowCols);
				}
			}
		}

		private void WriteCornerOnly(RPLWriter rplWriter, int headerRowCols, int headerColumnRows)
		{
			if (headerColumnRows != 0 && this.m_colHeaderHeights != null && headerRowCols != 0)
			{
				int i = 0;
				int num = 0;
				for (; i < this.m_headerColumnRows; i++)
				{
					if (this.m_colHeaderHeights[i].State == SizeInfo.PageState.Normal)
					{
						this.OpenHeaderRow(rplWriter, false, 0);
						this.WriteCornerCells(i, rplWriter, num, 0);
						this.CloseRow(rplWriter);
						num++;
					}
				}
			}
		}

		private void WriteColMembers(PageStructMemberCell structMember, int targetRow, int rowIndex, int colIndex, RPLWriter rplWriter, int pageRowIndex, ref int pageColIndex, ref bool writeOmittedHeaders)
		{
			PageStructStaticMemberCell pageStructStaticMemberCell = structMember as PageStructStaticMemberCell;
			if (pageStructStaticMemberCell != null)
			{
				this.WriteColMembers(pageStructStaticMemberCell.MemberInstance, (PageStructMemberCell)pageStructStaticMemberCell, targetRow, rowIndex, colIndex, rplWriter, pageRowIndex, ref pageColIndex, ref writeOmittedHeaders);
			}
			else
			{
				PageStructDynamicMemberCell pageStructDynamicMemberCell = structMember as PageStructDynamicMemberCell;
				if (pageStructDynamicMemberCell.MemberInstances != null)
				{
					PageMemberCell pageMemberCell = null;
					for (int i = 0; i < pageStructDynamicMemberCell.MemberInstances.Count; i++)
					{
						if (this.m_columnInfo[colIndex].PageHorizontalState == ColumnInfo.HorizontalState.AtRight)
						{
							break;
						}
						pageMemberCell = pageStructDynamicMemberCell.MemberInstances[i];
						this.WriteColMembers(pageMemberCell, (PageStructMemberCell)pageStructDynamicMemberCell, targetRow, rowIndex, colIndex, rplWriter, pageRowIndex, ref pageColIndex, ref writeOmittedHeaders);
						colIndex += pageMemberCell.ColSpan;
					}
				}
			}
		}

		private void WriteColMembers(PageMemberCell memberCell, PageStructMemberCell structMember, int targetRow, int rowIndex, int colIndex, RPLWriter rplWriter, int pageRowIndex, ref int pageColIndex, ref bool writeOmittedHeaders)
		{
			if (memberCell != null && !memberCell.Hidden && memberCell.CHOnHorizontalPage(this.m_columnInfo, colIndex))
			{
				bool flag = false;
				if (targetRow == rowIndex)
				{
					flag = true;
				}
				else if (targetRow < rowIndex + memberCell.RowSpan)
				{
					int num = rowIndex;
					for (int i = 0; i < memberCell.RowSpan; i++)
					{
						if (this.m_colHeaderHeights[rowIndex + i].State == SizeInfo.PageState.Skip)
						{
							num++;
						}
					}
					if (targetRow == num)
					{
						flag = true;
					}
				}
				int num2 = 0;
				int rowSpan = 0;
				bool flag2 = false;
				if (memberCell.RowSpan > 0 && (flag || targetRow < rowIndex + memberCell.RowSpan))
				{
					flag2 = memberCell.CHRowsOnVerticalPage(this.m_colHeaderHeights, rowIndex, out rowSpan);
					if (flag2)
					{
						ColumnInfo columnInfo = null;
						for (int j = 0; j < memberCell.ColSpan; j++)
						{
							columnInfo = this.m_columnInfo[colIndex + j];
							if (columnInfo.PageHorizontalState == ColumnInfo.HorizontalState.Normal && !columnInfo.Hidden)
							{
								num2++;
							}
						}
					}
				}
				if (flag)
				{
					if (memberCell.RowSpan > 0)
					{
						if (flag2)
						{
							if (memberCell.HasOmittedChildren && memberCell.RowSpan == 1)
							{
								writeOmittedHeaders = true;
							}
							this.WriteMemberToStream(rplWriter, structMember, memberCell, pageRowIndex, pageColIndex, rowSpan, num2, TablixRegion.ColumnHeader);
							pageColIndex += num2;
						}
					}
					else if (memberCell.Children != null)
					{
						for (int k = 0; k < memberCell.Children.Count; k++)
						{
							this.WriteColMembers(memberCell.Children[k], targetRow, rowIndex, colIndex, rplWriter, pageRowIndex, ref pageColIndex, ref writeOmittedHeaders);
							colIndex += memberCell.Children[k].Span;
						}
					}
				}
				else if (targetRow < rowIndex + memberCell.RowSpan)
				{
					if (targetRow == rowIndex + memberCell.RowSpan - 1 && memberCell.RowSpan > 0 && memberCell.HasOmittedChildren)
					{
						writeOmittedHeaders = true;
					}
					pageColIndex += num2;
				}
				else if (memberCell.Children != null)
				{
					for (int l = 0; l < memberCell.Children.Count; l++)
					{
						this.WriteColMembers(memberCell.Children[l], targetRow, rowIndex + memberCell.RowSpan, colIndex, rplWriter, pageRowIndex, ref pageColIndex, ref writeOmittedHeaders);
						colIndex += memberCell.Children[l].Span;
					}
				}
			}
		}

		private void WriteOmittedColHeadersRows(RPLWriter rplWriter, int rowIndex, int pageRowIndex, int headerRowCols)
		{
			int num = 0;
			bool flag = true;
			int num2 = 0;
			bool flag2 = true;
			int num3 = 0;
			PageStructMemberCell pageStructMemberCell = null;
			int num4 = 0;
			int num5 = -1;
			while (flag)
			{
				num = 0;
				flag = false;
				num3 = 0;
				num4 = 0;
				num5 = -1;
				flag2 = true;
				if (this.m_colsBeforeRowHeaders > 0)
				{
					while (num4 < this.m_columnHeaders.Count && num < this.m_colsBeforeRowHeaders)
					{
						pageStructMemberCell = this.m_columnHeaders[num4];
						if (num + pageStructMemberCell.Span <= this.m_colsBeforeRowHeaders)
						{
							this.WriteOmittedColMembers(pageStructMemberCell, rowIndex, 0, num2, num, rplWriter, pageRowIndex, ref num3, ref flag, ref flag2);
							num += pageStructMemberCell.Span;
							num4++;
							continue;
						}
						PageStructDynamicMemberCell pageStructDynamicMemberCell = pageStructMemberCell as PageStructDynamicMemberCell;
						PageMemberCell pageMemberCell = null;
						int num6 = 0;
						while (num6 < pageStructDynamicMemberCell.MemberInstances.Count)
						{
							if (this.m_columnInfo[num].PageHorizontalState != ColumnInfo.HorizontalState.AtRight)
							{
								pageMemberCell = pageStructDynamicMemberCell.MemberInstances[num6];
								this.WriteOmittedColMembers(pageMemberCell, pageStructDynamicMemberCell, rowIndex, 0, num2, num, rplWriter, pageRowIndex, ref num3, ref flag, ref flag2);
								num += pageMemberCell.ColSpan;
								if (num >= this.m_colsBeforeRowHeaders)
								{
									num5 = num6 + 1;
									break;
								}
								num6++;
								continue;
							}
							num5 = pageStructDynamicMemberCell.MemberInstances.Count;
							break;
						}
						break;
					}
				}
				num3 += headerRowCols;
				for (; num4 < this.m_columnHeaders.Count; num4++)
				{
					pageStructMemberCell = this.m_columnHeaders[num4];
					if (num5 >= 0)
					{
						PageStructDynamicMemberCell pageStructDynamicMemberCell2 = pageStructMemberCell as PageStructDynamicMemberCell;
						PageMemberCell pageMemberCell2 = null;
						int num7 = num5;
						while (num7 < pageStructDynamicMemberCell2.MemberInstances.Count)
						{
							if (this.m_columnInfo[num].PageHorizontalState != ColumnInfo.HorizontalState.AtRight)
							{
								pageMemberCell2 = pageStructDynamicMemberCell2.MemberInstances[num7];
								this.WriteOmittedColMembers(pageMemberCell2, pageStructDynamicMemberCell2, rowIndex, 0, num2, num, rplWriter, pageRowIndex, ref num3, ref flag, ref flag2);
								num += pageMemberCell2.ColSpan;
								num7++;
								continue;
							}
							num5 = pageStructDynamicMemberCell2.MemberInstances.Count;
							break;
						}
						num5 = -1;
					}
					else
					{
						this.WriteOmittedColMembers(pageStructMemberCell, rowIndex, 0, num2, num, rplWriter, pageRowIndex, ref num3, ref flag, ref flag2);
						num += pageStructMemberCell.Span;
					}
				}
				if (!flag2)
				{
					this.CloseRow(rplWriter);
				}
				num2++;
			}
		}

		private void WriteOmittedColMembers(PageStructMemberCell structMember, int targetRow, int rowIndex, int targetLevel, int colIndex, RPLWriter rplWriter, int pageRowIndex, ref int pageColIndex, ref bool writeOmittedHeaders, ref bool openRow)
		{
			PageStructStaticMemberCell pageStructStaticMemberCell = structMember as PageStructStaticMemberCell;
			if (pageStructStaticMemberCell != null)
			{
				this.WriteOmittedColMembers(pageStructStaticMemberCell.MemberInstance, (PageStructMemberCell)pageStructStaticMemberCell, targetRow, rowIndex, targetLevel, colIndex, rplWriter, pageRowIndex, ref pageColIndex, ref writeOmittedHeaders, ref openRow);
			}
			else
			{
				PageStructDynamicMemberCell pageStructDynamicMemberCell = structMember as PageStructDynamicMemberCell;
				if (pageStructDynamicMemberCell.MemberInstances != null)
				{
					PageMemberCell pageMemberCell = null;
					for (int i = 0; i < pageStructDynamicMemberCell.MemberInstances.Count; i++)
					{
						if (this.m_columnInfo[colIndex].PageHorizontalState == ColumnInfo.HorizontalState.AtRight)
						{
							break;
						}
						pageMemberCell = pageStructDynamicMemberCell.MemberInstances[i];
						this.WriteOmittedColMembers(pageMemberCell, (PageStructMemberCell)pageStructDynamicMemberCell, targetRow, rowIndex, targetLevel, colIndex, rplWriter, pageRowIndex, ref pageColIndex, ref writeOmittedHeaders, ref openRow);
						colIndex += pageMemberCell.ColSpan;
					}
				}
			}
		}

		private void WriteOmittedColMembers(PageMemberCell memberCell, PageStructMemberCell structMember, int targetRow, int rowIndex, int targetLevel, int colIndex, RPLWriter rplWriter, int pageRowIndex, ref int pageColIndex, ref bool writeOmittedHeaders, ref bool openRow)
		{
			if (memberCell != null && !memberCell.Hidden)
			{
				if (targetRow == rowIndex)
				{
					this.WriteOmittedColMembersLevel(memberCell, structMember, targetLevel, 0, colIndex, rplWriter, pageRowIndex, ref pageColIndex, ref writeOmittedHeaders, ref openRow);
				}
				else if (targetRow >= rowIndex + memberCell.RowSpan && memberCell.Children != null)
				{
					for (int i = 0; i < memberCell.Children.Count; i++)
					{
						this.WriteOmittedColMembers(memberCell.Children[i], targetRow, rowIndex + memberCell.RowSpan, targetLevel, colIndex, rplWriter, pageRowIndex, ref pageColIndex, ref writeOmittedHeaders, ref openRow);
						colIndex += memberCell.Children[i].Span;
					}
				}
			}
		}

		private void WriteOmittedColMembersLevel(PageMemberCell memberCell, PageStructMemberCell structMember, int targetLevel, int level, int colIndex, RPLWriter rplWriter, int pageRowIndex, ref int pageColIndex, ref bool writeOmittedHeaders, ref bool openRow)
		{
			if (memberCell != null && !memberCell.Hidden && memberCell.CHOnHorizontalPage(this.m_columnInfo, colIndex))
			{
				if (targetLevel == level)
				{
					if (memberCell.RowSpan == 0)
					{
						if (this.NeedWrite(memberCell, structMember.MemberDefIndex, TablixRegion.ColumnHeader))
						{
							if (openRow)
							{
								this.OpenHeaderRow(rplWriter, true, -1);
								openRow = false;
							}
							int num = 0;
							ColumnInfo columnInfo = null;
							for (int i = 0; i < memberCell.ColSpan; i++)
							{
								columnInfo = this.m_columnInfo[colIndex + i];
								if (columnInfo.PageHorizontalState == ColumnInfo.HorizontalState.Normal && !columnInfo.Hidden)
								{
									num++;
								}
							}
							this.WriteMemberToStream(rplWriter, structMember, memberCell, pageRowIndex, pageColIndex, 0, num, TablixRegion.ColumnHeader);
							pageColIndex += num;
						}
						if (memberCell.HasOmittedChildren)
						{
							writeOmittedHeaders = true;
						}
					}
				}
				else
				{
					bool flag = false;
					if (memberCell.RowSpan == 0 && memberCell.HasOmittedChildren && memberCell.Children != null)
					{
						for (int j = 0; j < memberCell.Children.Count; j++)
						{
							this.WriteOmittedColMembersLevel(memberCell.Children[j], targetLevel, level + 1, colIndex, rplWriter, pageRowIndex, ref pageColIndex, ref flag, ref openRow);
							colIndex += memberCell.Children[j].Span;
						}
						if (flag)
						{
							writeOmittedHeaders = true;
						}
					}
				}
			}
		}

		private void WriteOmittedColMembersLevel(PageStructMemberCell structMember, int targetLevel, int level, int colIndex, RPLWriter rplWriter, int pageRowIndex, ref int pageColIndex, ref bool writeOmittedHeaders, ref bool openRow)
		{
			PageStructStaticMemberCell pageStructStaticMemberCell = structMember as PageStructStaticMemberCell;
			if (pageStructStaticMemberCell != null)
			{
				this.WriteOmittedColMembersLevel(pageStructStaticMemberCell.MemberInstance, (PageStructMemberCell)pageStructStaticMemberCell, targetLevel, level, colIndex, rplWriter, pageRowIndex, ref pageColIndex, ref writeOmittedHeaders, ref openRow);
			}
			else
			{
				PageStructDynamicMemberCell pageStructDynamicMemberCell = structMember as PageStructDynamicMemberCell;
				if (pageStructDynamicMemberCell.MemberInstances != null)
				{
					PageMemberCell pageMemberCell = null;
					for (int i = 0; i < pageStructDynamicMemberCell.MemberInstances.Count; i++)
					{
						if (this.m_columnInfo[colIndex].PageHorizontalState == ColumnInfo.HorizontalState.AtRight)
						{
							break;
						}
						pageMemberCell = pageStructDynamicMemberCell.MemberInstances[i];
						this.WriteOmittedColMembersLevel(pageMemberCell, (PageStructMemberCell)pageStructDynamicMemberCell, targetLevel, level, colIndex, rplWriter, pageRowIndex, ref pageColIndex, ref writeOmittedHeaders, ref openRow);
						colIndex += pageMemberCell.ColSpan;
					}
				}
			}
		}

		private void WriteCornerCells(int targetRow, RPLWriter rplWriter, int pageRowIndex, int pageColIndex)
		{
			PageCornerCell pageCornerCell = null;
			int num = 0;
			bool flag = false;
			int num2 = 0;
			int num3 = 0;
			for (int i = 0; i <= targetRow; i++)
			{
				while (num < this.m_headerRowCols)
				{
					pageCornerCell = ((!this.IsLTR) ? this.m_cornerCells[i, this.m_headerRowCols - num - 1] : this.m_cornerCells[i, num]);
					if (pageCornerCell != null)
					{
						if (i + pageCornerCell.RowSpan > targetRow)
						{
							flag = false;
							if (targetRow == i)
							{
								flag = true;
							}
							else
							{
								int num4 = i;
								for (int j = 0; j < pageCornerCell.RowSpan; j++)
								{
									if (this.m_colHeaderHeights[i + j].State == SizeInfo.PageState.Skip)
									{
										num4++;
									}
								}
								if (targetRow == num4)
								{
									flag = true;
								}
							}
							if (flag)
							{
								num2 = 0;
								if (this.IsLTR)
								{
									for (int k = 0; k < pageCornerCell.ColSpan; k++)
									{
										if (this.m_rowHeaderWidths[num + k].State == SizeInfo.PageState.Normal)
										{
											num2++;
										}
									}
								}
								else
								{
									for (int l = 0; l < pageCornerCell.ColSpan; l++)
									{
										if (this.m_rowHeaderWidths[this.m_headerRowCols - num - 1 + l].State == SizeInfo.PageState.Normal)
										{
											num2++;
										}
									}
								}
								if (num2 > 0)
								{
									num3 = 0;
									for (int m = 0; m < pageCornerCell.RowSpan; m++)
									{
										if (this.m_colHeaderHeights[i + m].State == SizeInfo.PageState.Normal)
										{
											num3++;
										}
									}
									this.WriteCornerCellToStream(rplWriter, pageCornerCell, pageRowIndex, pageColIndex, num3, num2);
									pageColIndex += num2;
								}
							}
						}
						num += pageCornerCell.ColSpan;
					}
					else
					{
						num++;
					}
				}
				num = 0;
			}
		}

		private int AddToPageCornerCells(out int rowsOnPage, RPLWriter rplWriter, PageContext pageContext, double pageLeft, double pageTop, double pageRight, double pageBottom, RepeatState repeatState)
		{
			rowsOnPage = 0;
			if (this.m_colHeaderHeights == null && this.m_rowHeaderWidths == null)
			{
				return 0;
			}
			PageCornerCell pageCornerCell = null;
			int num = -1;
			int num2 = -1;
			if (this.AddToPageColumnHeaders && this.m_colHeaderHeights != null)
			{
				for (int i = 0; i < this.m_colHeaderHeights.Count; i++)
				{
					if (this.m_colHeaderHeights[i].State == SizeInfo.PageState.Normal)
					{
						if (num < 0)
						{
							num = (num2 = i);
						}
						else
						{
							num2++;
						}
						rowsOnPage++;
					}
				}
			}
			int num3 = 0;
			int num4 = -1;
			int num5 = -1;
			if (this.AddToPageRowHeaders && this.m_rowHeaderWidths != null)
			{
				for (int j = 0; j < this.m_rowHeaderWidths.Count; j++)
				{
					if (this.m_rowHeaderWidths[j].State == SizeInfo.PageState.Normal)
					{
						if (num4 < 0)
						{
							num4 = (num5 = j);
						}
						else
						{
							num5++;
						}
						num3++;
					}
				}
			}
			for (int k = 0; k <= num2; k++)
			{
				for (int l = 0; l <= num5; l++)
				{
					pageCornerCell = this.m_cornerCells[k, l];
					if (pageCornerCell != null && k + pageCornerCell.RowSpan >= num && l + pageCornerCell.ColSpan >= num4)
					{
						pageCornerCell.AddToPageContent(this.m_colHeaderHeights, this.m_rowHeaderWidths, k, l, this.IsLTR, rplWriter, pageContext, pageLeft, pageTop, pageRight, pageBottom, repeatState);
					}
				}
			}
			return num3;
		}

		private bool ResolveDuplicatesCornerCells(PageContext pageContext, double startInTablix, ref int startRowIndex)
		{
			bool flag = false;
			PageCornerCell pageCornerCell = null;
			if (this.SplitColumnHeaders && this.m_colHeaderHeights != null)
			{
				RoundedDouble roundedDouble = new RoundedDouble(0.0);
				for (int i = 0; i < this.m_colHeaderHeights.Count; i++)
				{
					roundedDouble.Value = this.m_colHeaderHeights[i].EndPos;
					if (!(roundedDouble <= startInTablix))
					{
						break;
					}
					startRowIndex++;
				}
			}
			for (int j = 0; j < this.m_headerColumnRows; j++)
			{
				for (int k = 0; k < this.m_headerRowCols; k++)
				{
					pageCornerCell = this.m_cornerCells[j, k];
					if (pageCornerCell != null && j + pageCornerCell.RowSpan >= startRowIndex)
					{
						flag |= pageCornerCell.ResolveDuplicates(j, startRowIndex, startInTablix, this.m_colHeaderHeights, pageContext);
					}
				}
			}
			return flag;
		}

		internal override void OmitBorderOnPageBreak(RPLWriter rplWriter, double pageLeft, double pageTop, double pageRight, double pageBottom)
		{
			if (rplWriter != null && ((AspNetCore.ReportingServices.OnDemandReportRendering.Tablix)base.m_source).OmitBorderOnPageBreak)
			{
				base.OmitBorderOnPageBreak(pageLeft, pageTop, pageRight, pageBottom);
			}
		}

		internal override bool AddToPage(RPLWriter rplWriter, PageContext pageContext, double pageLeft, double pageTop, double pageRight, double pageBottom, RepeatState repeatState)
		{
			if (this.HitsCurrentPage(pageLeft, pageTop, pageRight, pageBottom))
			{
				this.WriteStartItemToStream(rplWriter, pageContext);
				this.OmitBorderOnPageBreak(rplWriter, pageLeft, pageTop, pageRight, pageBottom);
				double num = Math.Max(0.0, pageLeft - base.ItemPageSizes.Left);
				double num2 = Math.Max(0.0, pageTop - base.ItemPageSizes.Top);
				double num3 = pageRight - base.ItemPageSizes.Left;
				double num4 = pageBottom - base.ItemPageSizes.Top;
				bool hasLabelsOnCH = false;
				bool hasLabelsOnRH = false;
				int num5 = 0;
				RepeatState repeatState2 = repeatState;
				if (this.RepeatColumnHeaders)
				{
					repeatState2 |= RepeatState.Vertical;
				}
				if (this.RepeatRowHeaders)
				{
					repeatState2 |= RepeatState.Horizontal;
				}
				int num6 = this.AddToPageCornerCells(out num5, rplWriter, pageContext, num, num2, num3, num4, repeatState2);
				int num7 = 0;
				if (this.AddToPageColumnHeaders && this.m_columnHeaders != null)
				{
					repeatState2 = repeatState;
					int num8 = 0;
					if (this.RepeatColumnHeaders)
					{
						repeatState2 |= RepeatState.Vertical;
					}
					for (int i = 0; i < this.m_columnHeaders.Count; i++)
					{
						num7 += this.m_columnHeaders[i].AddToPageCHContent(this.m_colHeaderHeights, this.m_columnInfo, 0, num8, this.IsLTR, rplWriter, pageContext, num, num2, num3, num4, repeatState2, ref hasLabelsOnCH);
						num8 += this.m_columnHeaders[i].Span;
					}
				}
				if (this.AddToPageRowHeaders && this.m_rowHeaders != null)
				{
					repeatState2 = repeatState;
					int num9 = 0;
					if (this.RepeatRowHeaders && repeatState2 == RepeatState.None)
					{
						RoundedDouble x = new RoundedDouble(base.ItemPageSizes.Right);
						if (!(x <= pageRight))
						{
							repeatState2 |= RepeatState.Horizontal;
						}
					}
					for (int j = 0; j < this.m_rowHeaders.Count; j++)
					{
						this.m_rowHeaders[j].AddToPageRHContent(this.m_rowHeaderWidths, this.m_detailRows, num9, 0, this.IsLTR, rplWriter, pageContext, num, num2, num3, num4, repeatState2, ref hasLabelsOnRH);
						num9 += this.m_rowHeaders[j].Span;
					}
				}
				if (this.m_detailRows != null)
				{
					int num10 = 0;
					bool pinnedToParentCell = this.m_ignoreCellPageBreaks > 0;
					for (int k = 0; k < this.m_detailRows.Count; k++)
					{
						num5 += this.m_detailRows[k].AddToPageContent(this.m_columnInfo, out num10, this.IsLTR, pinnedToParentCell, rplWriter, pageContext, num, num2, num3, num4, repeatState);
						if (num10 > num7)
						{
							num7 = num10;
						}
					}
				}
				num6 += num7;
				this.WriteEndItemToStream(rplWriter, num6, num5, hasLabelsOnCH, hasLabelsOnRH);
				RoundedDouble x2 = new RoundedDouble(base.ItemPageSizes.Right);
				if ((repeatState & RepeatState.Horizontal) == RepeatState.None || x2 > pageRight)
				{
					this.UpdateColumns(num, num3);
				}
				if (x2 <= pageRight)
				{
					bool delete = true;
					if (repeatState != 0)
					{
						delete = false;
					}
					this.UpdateRows(num2, num4, delete);
					if ((repeatState & RepeatState.Horizontal) == RepeatState.None)
					{
						this.m_columnInfo.Dispose();
						this.m_columnInfo = null;
						this.m_rowHeaderWidths = null;
					}
				}
				return true;
			}
			return false;
		}

		internal override bool ResolveDuplicates(PageContext pageContext, double topInParentSystem, PageItem[] siblings, bool recalculate)
		{
			base.ResolveDuplicates(pageContext, topInParentSystem, siblings, recalculate);
			if (!this.ColumnHeadersCreated)
			{
				return false;
			}
			bool flag = false;
			bool flag2 = false;
			double num = Math.Max(0.0, topInParentSystem - base.ItemPageSizes.Top);
			double num2 = 0.0;
			if (this.m_colHeaderHeights != null && (num == 0.0 || this.SplitColumnHeaders))
			{
				int targetRowIndex = 0;
				flag = this.ResolveDuplicatesCornerCells(pageContext, num, ref targetRowIndex);
				if (this.m_columnHeaders != null)
				{
					for (int i = 0; i < this.m_columnHeaders.Count; i++)
					{
						flag |= this.m_columnHeaders[i].ResolveCHDuplicates(0, targetRowIndex, num, this.m_colHeaderHeights, pageContext);
					}
				}
				if (flag)
				{
					if (this.SplitColumnHeaders)
					{
						flag = false;
					}
					else
					{
						Tablix.ResolveSizes(this.m_colHeaderHeights);
						Tablix.ResolveStartPos(this.m_colHeaderHeights, 0.0);
						num2 = this.m_colHeaderHeights[this.m_headerColumnRows - 1].EndPos;
					}
				}
			}
			if (!this.SplitColumnHeaders)
			{
				if (this.m_rowHeaders != null)
				{
					int num3 = 0;
					for (int j = 0; j < this.m_rowHeaders.Count; j++)
					{
						flag2 |= this.m_rowHeaders[j].ResolveRHDuplicates(num3, num, this.m_detailRows, pageContext);
						num3 += this.m_rowHeaders[j].Span;
					}
				}
				if (this.m_detailRows != null)
				{
					for (int k = 0; k < this.m_detailRows.Count; k++)
					{
						flag2 |= this.m_detailRows[k].ResolveDuplicates(num, pageContext);
					}
				}
			}
			if (flag)
			{
				this.NormalizeRowHeadersHeights(num + num2, 1.7976931348623157E+308, false);
			}
			else if (flag2)
			{
				if (this.RowHeadersCreated && num == 0.0)
				{
					this.NormalizeRowHeadersHeights(num + num2, 1.7976931348623157E+308, false);
				}
				else
				{
					flag2 = false;
				}
			}
			if (!flag && !flag2)
			{
				return false;
			}
			return true;
		}

		protected override void DetermineVerticalSize(PageContext pageContext, double topInParentSystem, double bottomInParentSystem, List<PageItem> ancestors, ref bool anyAncestorHasKT, bool hasUnpinnedAncestors)
		{
			double num = Math.Max(0.0, topInParentSystem - base.ItemPageSizes.Top);
			double num2 = bottomInParentSystem - base.ItemPageSizes.Top;
			double num3 = 1.7976931348623157E+308;
			bool flag = true;
			if (!pageContext.FullOnPage)
			{
				num3 = ((!anyAncestorHasKT && !base.KeepTogetherVertical && !hasUnpinnedAncestors) ? (num2 - num) : pageContext.ColumnHeight);
				if (this.ColumnHeadersCreated && this.m_detailRows != null && this.m_detailRows.Count > 0)
				{
					RowInfo rowInfo = null;
					if (base.ItemPageSizes.Top >= topInParentSystem)
					{
						rowInfo = this.m_detailRows[this.m_detailRows.Count - 1];
						num3 = Math.Min(num3, Math.Max(0.0, num2 - rowInfo.Bottom));
					}
					else
					{
						for (int num4 = this.m_detailRows.Count - 1; num4 >= 0; num4--)
						{
							rowInfo = this.m_detailRows[num4];
							if (rowInfo.PageVerticalState == RowInfo.VerticalState.Below)
							{
								num3 = Math.Min(num3, Math.Max(0.0, num2 - rowInfo.Bottom));
								break;
							}
						}
					}
				}
			}
			else if (this.ColumnHeadersCreated && base.ItemPageSizes.Top >= topInParentSystem)
			{
				flag = false;
			}
			if (flag)
			{
				CreateItemsContext createItems = new CreateItemsContext(num3);
				PageContext pageContext2 = pageContext;
				if (!pageContext.IgnorePageBreaks && base.IgnorePageBreaks)
				{
					pageContext2 = new PageContext(pageContext, pageContext.CacheNonSharedProps);
					pageContext2.IgnorePageBreaks = true;
					pageContext2.IgnorePageBreaksReason = PageContext.IgnorePageBreakReason.ToggleableItem;
				}
				this.CreateVertically(pageContext2, createItems, num, num2, topInParentSystem);
			}
			if (this.RowHeadersCreated)
			{
				if (this.m_detailRows != null && this.m_detailRows.Count > 0)
				{
					double bottom = this.m_detailRows[this.m_detailRows.Count - 1].Bottom;
					base.ItemPageSizes.AdjustHeightTo(bottom);
				}
				base.FullyCreated = true;
			}
			else
			{
				RoundedDouble x = new RoundedDouble(base.ItemPageSizes.Height);
				if (x <= num2)
				{
					base.ItemPageSizes.AdjustHeightTo(num2 + 0.02);
				}
				base.FullyCreated = false;
			}
		}

		protected override bool InvalidateVerticalKT(bool anyAncestorHasKT, PageContext pageContext)
		{
			if (!this.RowHeadersCreated)
			{
				if (pageContext.Common.DiagnosticsEnabled && base.KeepTogetherVertical)
				{
					base.TraceInvalidatedKeepTogetherVertical(pageContext);
				}
				return true;
			}
			return base.InvalidateVerticalKT(anyAncestorHasKT, pageContext);
		}

		protected override bool ResolveKeepTogetherVertical(PageContext pageContext, double topInParentSystem, double bottomInParentSystem, bool resolveItem, bool canOverwritePageBreak, bool canSetPageName)
		{
			PageBreakProperties pageBreakProperties = null;
			if (!canOverwritePageBreak)
			{
				pageBreakProperties = pageContext.Common.RegisteredPageBreakProperties;
			}
			string text = null;
			if (!canSetPageName)
			{
				text = pageContext.Common.PageName;
			}
			double num = bottomInParentSystem - base.ItemPageSizes.Top;
			if (base.UnresolvedKTV)
			{
				base.UnresolvedKTV = false;
				if (base.KeepTogetherVertical)
				{
					RoundedDouble x = new RoundedDouble(base.ItemPageSizes.Height);
					if (x > num && x <= pageContext.ColumnHeight)
					{
						base.ItemPageSizes.MoveVertical(num);
						base.OnThisVerticalPage = false;
						base.TraceKeepTogetherVertical(pageContext);
						return true;
					}
				}
			}
			bool canPush = false;
			if (base.ItemPageSizes.Top > topInParentSystem)
			{
				canPush = true;
			}
			double startInTablix = Math.Max(0.0, topInParentSystem - base.ItemPageSizes.Top);
			this.AddToPageColumnHeaders = true;
			this.RepeatedColumnHeaders = false;
			if (resolveItem)
			{
				this.NormalizeRowHeadersHeights(startInTablix, num, false);
			}
			this.MarkTablixRowsForVerticalPage(startInTablix, num, pageContext, canOverwritePageBreak, canPush);
			if (base.StartsOnThisPage(pageContext, topInParentSystem, bottomInParentSystem))
			{
				pageContext.Common.SetPageName(base.PageName, canSetPageName);
			}
			if (this.RowHeadersCreated && !base.PageBreakAtEnd && this.m_detailRows != null && this.m_detailRows.Count > 0 && this.m_detailRows[this.m_detailRows.Count - 1].PageBreaksAtEnd)
			{
				if (base.ItemPageSizes.Bottom < bottomInParentSystem)
				{
					double num2 = bottomInParentSystem - base.ItemPageSizes.Bottom;
					base.ItemPageSizes.DeltaY += num2;
					PageBreakProperties pageBreakPropertiesAtEnd = this.m_detailRows[this.m_detailRows.Count - 1].PageBreakPropertiesAtEnd;
					pageContext.Common.RegisterPageBreakProperties(pageBreakPropertiesAtEnd, canOverwritePageBreak);
				}
				if (pageContext.Common.DiagnosticsEnabled && base.ItemPageSizes.Bottom == bottomInParentSystem)
				{
					RowInfo rowInfo = this.m_detailRows[this.m_detailRows.Count - 1];
					object source = rowInfo.PageBreakPropertiesAtEnd.Source;
					pageContext.Common.TracePageBreakIgnoredAtBottomOfPage(source);
				}
			}
			if (pageBreakProperties != null)
			{
				pageContext.Common.RegisterPageBreakProperties(pageBreakProperties, true);
			}
			if (text != null)
			{
				pageContext.Common.SetPageName(text, true);
			}
			return false;
		}

		protected override void DetermineHorizontalSize(PageContext pageContext, double leftInParentSystem, double rightInParentSystem, List<PageItem> ancestors, bool anyAncestorHasKT, bool hasUnpinnedAncestors)
		{
			double startInTablix = Math.Max(0.0, leftInParentSystem - base.ItemPageSizes.Left);
			double endInTablix = rightInParentSystem - base.ItemPageSizes.Left;
			if (anyAncestorHasKT || base.KeepTogetherHorizontal || hasUnpinnedAncestors)
			{
				double columnWidth = pageContext.ColumnWidth;
			}
			this.CalculateContentHorizontally(startInTablix, endInTablix, pageContext);
		}

		protected override bool ResolveKeepTogetherHorizontal(PageContext pageContext, double leftInParentSystem, double rightInParentSystem, bool resolveItem)
		{
			double num = rightInParentSystem - base.ItemPageSizes.Left;
			if (base.UnresolvedKTH)
			{
				base.UnresolvedKTH = false;
				if (base.KeepTogetherHorizontal)
				{
					RoundedDouble x = new RoundedDouble(base.ItemPageSizes.Width);
					if (x > num && x <= pageContext.ColumnWidth)
					{
						base.ItemPageSizes.MoveHorizontal(num);
						base.TraceKeepTogetherHorizontal(pageContext);
						return true;
					}
				}
			}
			double startInTablix = Math.Max(0.0, leftInParentSystem - base.ItemPageSizes.Left);
			this.AddToPageRowHeaders = true;
			if (this.m_columnInfo != null && this.m_columnInfo.Count > 0)
			{
				if (resolveItem)
				{
					this.NormalizeColHeadersWidths(startInTablix, num, false);
				}
				this.MarkTablixColumnsForHorizontalPage(startInTablix, num, pageContext);
			}
			else
			{
				this.SplitRowHeaders = true;
				this.MarkSplitRowHeaders(startInTablix, num, pageContext);
			}
			return false;
		}

		private void SaveRowMemberInstanceIndex(TablixMember rowMember, TablixDynamicMemberInstance rowMemberInstance)
		{
			if (this.m_rowMemberInstanceIndexes == null)
			{
				this.m_rowMemberInstanceIndexes = new Hashtable(20, 0.1f);
			}
			this.m_rowMemberInstanceIndexes[rowMember.ID] = rowMemberInstance.GetInstanceIndex();
		}

		private void RestoreRowMemberInstanceIndex(TablixMember rowMember, TablixDynamicMemberInstance rowMemberInstance)
		{
			if (this.m_rowMemberInstanceIndexes != null)
			{
				int? nullable = (int?)this.m_rowMemberInstanceIndexes[rowMember.ID];
				if (nullable.HasValue)
				{
					int value = nullable.Value;
					if (value != rowMemberInstance.GetInstanceIndex())
					{
						rowMemberInstance.SetInstanceIndex(value);
					}
				}
			}
		}

		private void UpdateRows(double startInTablix, double endInTablix, bool delete)
		{
			if (this.m_rowHeaders != null)
			{
				PageStructMemberCell pageStructMemberCell = null;
				int num = 0;
				for (int i = 0; i < this.m_rowHeaders.Count; i++)
				{
					pageStructMemberCell = this.m_rowHeaders[i];
					pageStructMemberCell.UpdateRows(this.m_detailRows, num, true, 0, delete, startInTablix, endInTablix);
					if (pageStructMemberCell.Span == 0 && !pageStructMemberCell.PartialItem)
					{
						pageStructMemberCell.DisposeInstances();
						this.m_rowHeaders.RemoveAt(i);
						i--;
						i -= Tablix.RemoveHeadersAbove(this.m_rowHeaders, i, this.m_detailRows, ref num);
					}
					num += pageStructMemberCell.Span;
				}
				if (this.m_rowHeaders.Count == 0)
				{
					this.m_rowHeaders = null;
				}
			}
		}

		private void CreateVertically(PageContext pageContext, CreateItemsContext createItems, double startInTablix, double endInTablix, double topInParentSystem)
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix = (AspNetCore.ReportingServices.OnDemandReportRendering.Tablix)base.m_source;
			if (!this.ColumnHeadersCreated)
			{
				if (tablix.LayoutDirection == TablixLayoutDirection.LTR)
				{
					this.IsLTR = true;
				}
				this.m_headerColumnRows = tablix.Columns;
				this.m_headerRowCols = tablix.Rows;
				this.RepeatColumnHeaders = tablix.RepeatColumnHeaders;
				this.RepeatRowHeaders = tablix.RepeatRowHeaders;
				this.AddToPageColumnHeaders = true;
				this.AddToPageRowHeaders = true;
				base.ItemPageSizes.AdjustHeightTo(0.0);
			}
			double num = 0.0;
			if (createItems.SpaceToFill <= 0.01)
			{
				this.NormalizeRowHeadersHeights(startInTablix + num, endInTablix, false);
			}
			else
			{
				num = this.CreateTablixItems(tablix, pageContext, createItems, startInTablix, endInTablix);
				this.NormalizeRowHeadersHeights(startInTablix + num, endInTablix, false);
			}
		}

		private double CreateTablixItems(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix, PageContext pageContext, CreateItemsContext createItems, double startInTablix, double endInTablix)
		{
			double result = this.CreateColumnsHeaders(tablix, pageContext);
			if (this.RowHeadersCreated)
			{
				return result;
			}
			bool flag = false;
			this.CreateTablixRows(tablix, (TablixMember)null, this.m_rowMembersDepth, false, 0, 0, ref this.m_rowHeaders, false, ref flag, false, createItems, startInTablix, endInTablix, pageContext);
			if (flag)
			{
				this.RowHeadersCreated = true;
				if (this.m_rowHeaders != null && this.m_rowHeaders.Count == 0)
				{
					this.m_rowHeaders = null;
				}
			}
			this.m_ignoreGroupPageBreaks = 0;
			return result;
		}

		private void AlignSplitColumnHeaders(int startRowIndex, int lastRowIndex, double startInTablix, double endInTablix, PageContext pageContext, bool normalize)
		{
			this.m_colHeaderHeights[lastRowIndex].State = SizeInfo.PageState.Normal;
			for (int i = 0; i < this.m_columnHeaders.Count; i++)
			{
				this.m_columnHeaders[i].AlignCHToPageVertical(0, lastRowIndex, startInTablix, endInTablix, this.m_colHeaderHeights, pageContext);
			}
			this.AlignCornerCellsToPageVertical(lastRowIndex, startInTablix, endInTablix, pageContext);
			Tablix.ResolveSizes(this.m_colHeaderHeights);
			Tablix.ResolveStartPos(this.m_colHeaderHeights, 0.0);
			double num = 0.0;
			for (int j = 0; j < startRowIndex; j++)
			{
				this.m_colHeaderHeights[j].State = SizeInfo.PageState.Skip;
			}
			for (int k = startRowIndex; k <= lastRowIndex; k++)
			{
				this.m_colHeaderHeights[k].State = SizeInfo.PageState.Normal;
			}
			int num2 = this.m_colHeaderHeights.Count - 1;
			for (int l = lastRowIndex + 1; l <= num2; l++)
			{
				this.m_colHeaderHeights[l].State = SizeInfo.PageState.Skip;
			}
			num = this.m_colHeaderHeights[num2].EndPos;
			if (normalize)
			{
				if (this.m_detailRows != null && this.m_detailRows.Count > 0)
				{
					RowInfo rowInfo = null;
					using (this.m_detailRows.GetAndPin(0, out rowInfo))
					{
						rowInfo.PageVerticalState = RowInfo.VerticalState.Normal;
						rowInfo.Top = num;
					}
					this.NormalizeRowHeadersHeights(startInTablix, endInTablix, true);
				}
				else
				{
					base.ItemPageSizes.AdjustHeightTo(num);
				}
			}
		}

		private void MarkDetailRowVerticalState(int rowIndex, RowInfo.VerticalState state)
		{
			this.MarkDetailRowVerticalState(rowIndex, state, null, false);
		}

		private void MarkDetailRowVerticalState(int rowIndex, RowInfo.VerticalState state, PageContext pageContext, bool canOverwritePageBreak)
		{
			RowInfo rowInfo = null;
			using (this.m_detailRows.GetAndPin(rowIndex, out rowInfo))
			{
				rowInfo.PageVerticalState = state;
				if (rowInfo.PageVerticalState == RowInfo.VerticalState.TopOfNextPage && pageContext != null)
				{
					pageContext.Common.RegisterPageBreakProperties(rowInfo.PageBreakPropertiesAtStart, canOverwritePageBreak);
				}
			}
		}

		private void MarkTablixRowsForVerticalPage(double startInTablix, double endInTablix, PageContext pageContext, bool canOverwritePageBreak, bool canPush)
		{
			bool flag = false;
			RoundedDouble roundedDouble = new RoundedDouble(0.0);
			RoundedDouble roundedDouble2 = new RoundedDouble(0.0);
			int rowsOnPage = 0;
			if (startInTablix == 0.0)
			{
				if (this.m_headerColumnRows > 0 && this.m_colHeaderHeights != null)
				{
					int num = 0;
					for (int i = 0; i < this.m_colHeaderHeights.Count; i++)
					{
						roundedDouble.Value += this.m_colHeaderHeights[i].SizeValue;
						if (roundedDouble < endInTablix)
						{
							num++;
						}
					}
					if (roundedDouble <= endInTablix)
					{
						if (roundedDouble > 0.0)
						{
							rowsOnPage = 1;
						}
						roundedDouble2.Value = endInTablix;
						if (roundedDouble2 == pageContext.ColumnHeight || this.PinnedToParentCell)
						{
							startInTablix += roundedDouble.Value;
							roundedDouble.Value = 0.0;
						}
						else
						{
							flag = true;
						}
						goto IL_045d;
					}
					if (roundedDouble <= pageContext.ColumnHeight && !this.PinnedToParentCell)
					{
						base.ItemPageSizes.MoveVertical(endInTablix);
						base.OnThisVerticalPage = false;
					}
					else
					{
						this.SplitColumnHeaders = true;
						this.RepeatColumnHeaders = false;
						this.AlignSplitColumnHeaders(0, num, startInTablix, endInTablix, pageContext, true);
					}
				}
				else if (this.m_detailRows != null)
				{
					roundedDouble2.Value = endInTablix;
					if (roundedDouble2 == pageContext.ColumnHeight)
					{
						if (pageContext.Common.DiagnosticsEnabled && this.m_detailRows.Count > 0)
						{
							RowInfo rowInfo = this.m_detailRows[0];
							if (rowInfo.PageBreaksAtStart)
							{
								object source = rowInfo.PageBreakPropertiesAtStart.Source;
								pageContext.Common.TracePageBreakIgnoredAtTopOfPage(source);
							}
						}
					}
					else if (!this.PinnedToParentCell)
					{
						if (this.m_detailRows.Count > 0 && this.m_detailRows[0].PageBreaksAtStart && canPush)
						{
							base.ItemPageSizes.MoveVertical(endInTablix);
							base.OnThisVerticalPage = false;
							pageContext.Common.RegisterPageBreakProperties(this.m_detailRows[0].PageBreakPropertiesAtStart, canOverwritePageBreak);
							return;
						}
						rowsOnPage = 1;
						flag = true;
					}
					goto IL_045d;
				}
				return;
			}
			if (this.SplitColumnHeaders)
			{
				int num2 = 0;
				int num3 = 0;
				for (int j = 0; j < this.m_colHeaderHeights.Count; j++)
				{
					roundedDouble2.Value = this.m_colHeaderHeights[j].EndPos;
					if (roundedDouble2 <= startInTablix)
					{
						this.m_colHeaderHeights[j].State = SizeInfo.PageState.Skip;
						num3++;
						num2++;
					}
					else
					{
						roundedDouble.Value += this.m_colHeaderHeights[j].SizeValue;
						roundedDouble.Value -= Math.Max(0.0, startInTablix - this.m_colHeaderHeights[j].StartPos);
						roundedDouble2.Value = startInTablix + roundedDouble.Value;
						if (roundedDouble2 < endInTablix)
						{
							this.m_colHeaderHeights[j].State = SizeInfo.PageState.Normal;
							num2++;
						}
					}
				}
				roundedDouble.Value += startInTablix;
				if (roundedDouble <= endInTablix)
				{
					this.AlignSplitColumnHeaders(num3, num2 - 1, startInTablix, endInTablix, pageContext, false);
					if (this.m_detailRows != null && this.m_detailRows.Count > 0)
					{
						this.MarkDetailRowVerticalState(0, RowInfo.VerticalState.TopOfNextPage, pageContext, canOverwritePageBreak);
						this.NormalizeRowHeadersHeights(startInTablix, endInTablix, false);
					}
					else
					{
						base.ItemPageSizes.AdjustHeightTo(roundedDouble.Value);
					}
					this.SplitColumnHeaders = false;
				}
				else
				{
					this.AlignSplitColumnHeaders(num3, num2, startInTablix, endInTablix, pageContext, true);
				}
				return;
			}
			if (this.RepeatColumnHeaders && this.m_colHeaderHeights != null)
			{
				if (this.TryRepeatColumnHeaders(startInTablix, endInTablix))
				{
					for (int k = 0; k < this.m_colHeaderHeights.Count; k++)
					{
						roundedDouble += this.m_colHeaderHeights[k].SizeValue;
					}
					RowInfo rowInfo2 = null;
					for (int l = 0; l < this.m_detailRows.Count; l++)
					{
						using (this.m_detailRows.GetAndPin(l, out rowInfo2))
						{
							if (rowInfo2.PageVerticalState != RowInfo.VerticalState.Above)
							{
								rowInfo2.PageVerticalState = RowInfo.VerticalState.Unknown;
								break;
							}
						}
					}
					this.RepeatedColumnHeaders = true;
					if (roundedDouble > 0.0)
					{
						rowsOnPage = 1;
					}
				}
				else
				{
					this.AddToPageColumnHeaders = false;
				}
			}
			else
			{
				this.AddToPageColumnHeaders = false;
			}
			goto IL_045d;
			IL_045d:
			HeaderFooterRows prevHeaders = null;
			HeaderFooterRows prevFooters = null;
			bool flag2 = false;
			bool flag3 = false;
			int rowIndex = 0;
			double num4 = endInTablix;
			if (this.MarkDetailRowsForVerticalPage(this.m_rowHeaders, startInTablix + roundedDouble.Value, ref num4, prevHeaders, prevFooters, ref flag3, ref flag2, rowIndex, rowsOnPage, pageContext) == 0)
			{
				if (flag)
				{
					if (this.m_detailRows.Count > 0)
					{
						this.MarkDetailRowVerticalState(0, RowInfo.VerticalState.Normal);
					}
					base.ItemPageSizes.MoveVertical(endInTablix);
					base.OnThisVerticalPage = false;
					return;
				}
				if (roundedDouble > 0.0)
				{
					RowInfo rowInfo3 = null;
					for (int m = 0; m < this.m_detailRows.Count; m++)
					{
						using (this.m_detailRows.GetAndPin(m, out rowInfo3))
						{
							if (rowInfo3.PageVerticalState != RowInfo.VerticalState.Above)
							{
								rowInfo3.PageVerticalState = RowInfo.VerticalState.Unknown;
								break;
							}
						}
					}
					prevHeaders = null;
					prevFooters = null;
					flag2 = false;
					flag3 = false;
					rowIndex = 0;
					num4 = endInTablix;
					rowsOnPage = this.MarkDetailRowsForVerticalPage(this.m_rowHeaders, startInTablix, ref num4, prevHeaders, prevFooters, ref flag3, ref flag2, rowIndex, 0, pageContext);
					this.AddToPageColumnHeaders = false;
					roundedDouble.Value = 0.0;
				}
			}
			if (this.AddToPageColumnHeaders && roundedDouble > 0.0 && !flag)
			{
				Tablix.ResolveStartPosAndState(this.m_colHeaderHeights, startInTablix, SizeInfo.PageState.Normal);
			}
			this.NormalizeRowHeadersHeights(startInTablix + roundedDouble.Value, endInTablix, true);
		}

		private void AlignCornerCellsToPageVertical(int targetRowIndex, double startInTablix, double endInTablix, PageContext pageContext)
		{
			if (this.m_cornerCells != null)
			{
				PageCornerCell pageCornerCell = null;
				for (int i = 0; i < this.m_headerColumnRows; i++)
				{
					for (int j = 0; j < this.m_headerRowCols; j++)
					{
						pageCornerCell = this.m_cornerCells[i, j];
						if (pageCornerCell != null && i <= targetRowIndex && i + pageCornerCell.RowSpan > targetRowIndex)
						{
							pageCornerCell.AlignToPageVertical(i, targetRowIndex, startInTablix, endInTablix, this.m_colHeaderHeights, pageContext);
						}
					}
				}
			}
		}

		private void NormalizeRowHeadersHeights(double startInTablix, double endInTablix, bool update)
		{
			if (this.m_rowHeaders != null)
			{
				int num = 0;
				int index = -1;
				int num2 = -1;
				double num3 = 0.0;
				for (int i = 0; i < this.m_rowHeaders.Count; i++)
				{
					this.m_rowHeaders[i].NormalizeDetailRowHeight(this.m_detailRows, num, startInTablix, endInTablix, ref index, ref num2, update, ref num3);
					num += this.m_rowHeaders[i].Span;
				}
				double num4 = 0.0;
				num4 = ((num2 < 0) ? this.m_detailRows[index].Bottom : this.m_detailRows[num2].Bottom);
				if (num4 > base.ItemPageSizes.Height)
				{
					base.ItemPageSizes.AdjustHeightTo(num4);
				}
			}
		}

		private bool TryRepeatColumnHeaders(double startInTablix, double endInTablix)
		{
			if (this.m_detailRows != null && this.m_detailRows.Count != 0)
			{
				RowInfo rowInfo = null;
				RoundedDouble roundedDouble = new RoundedDouble(0.0);
				for (int i = 0; i < this.m_detailRows.Count; i++)
				{
					rowInfo = this.m_detailRows[i];
					roundedDouble.Value = rowInfo.Bottom;
					if (roundedDouble > startInTablix)
					{
						roundedDouble.Value = rowInfo.Top;
						if (roundedDouble < startInTablix)
						{
							return false;
						}
						return true;
					}
				}
				return false;
			}
			return false;
		}

		private bool ResolveFooters(HeaderFooterRows prevFooters, ref double endInTablix, double delta, bool lastMember, ref bool prevFootersOnPage, PageContext pageContext)
		{
			if (prevFooters == null)
			{
				return false;
			}
			if (delta <= 0.0)
			{
				return true;
			}
			int num = 0;
			double num2 = 0.0;
			num = ((!lastMember) ? prevFooters.AddToPageRepeat(ref delta, ref num2, pageContext, this) : prevFooters.AddKeepWithAndRepeat(ref delta, ref num2, pageContext, this));
			if (num > 0)
			{
				endInTablix -= num2;
				prevFootersOnPage = true;
			}
			if (delta <= 0.0)
			{
				return true;
			}
			return false;
		}

		private bool ResolveHeaders(HeaderFooterRows prevHeaders, ref double endInTablix, double delta, PageContext pageContext)
		{
			if (prevHeaders == null)
			{
				return false;
			}
			if (delta <= 0.0)
			{
				return true;
			}
			double num = 0.0;
			prevHeaders.AddToPageRepeat(ref delta, ref num, pageContext, this);
			endInTablix -= num;
			if (delta <= 0.0)
			{
				return true;
			}
			return false;
		}

		private void MoveRowToNextPage(int rowIndex, PageContext pageContext)
		{
			if (rowIndex < this.m_detailRows.Count)
			{
				this.MarkDetailRowVerticalState(rowIndex, RowInfo.VerticalState.TopOfNextPage, pageContext, false);
			}
		}

		private int CalculateCurrentRowSpan(PageMemberCell memberCell, int rowIndex)
		{
			int num = memberCell.RowSpan - memberCell.CurrRowSpan;
			if (num == 0)
			{
				return memberCell.CurrRowSpan;
			}
			int num2 = 0;
			for (int i = rowIndex; i < rowIndex + num; i++)
			{
				if (this.m_detailRows[i].PageVerticalState == RowInfo.VerticalState.Repeat)
				{
					num2++;
				}
			}
			return memberCell.CurrRowSpan + num2;
		}

		private bool MarkSpanRHMemberOnPage(PageMemberCell memberCell, double startInTablix, double endInTablix, int rowIndex, int rowsOnPage, int spanContent, PageContext pageContext)
		{
			int num = this.MarkSpanRHMemberDetailRows(memberCell.Children, startInTablix, endInTablix, rowIndex, rowsOnPage, spanContent, pageContext);
			memberCell.ResolveRHVertical(this.m_detailRows, rowIndex, startInTablix, endInTablix, pageContext);
			if (num < memberCell.CurrRowSpan)
			{
				memberCell.CurrRowSpan = num;
				return true;
			}
			RoundedDouble x = new RoundedDouble(memberCell.EndPos);
			if (x > endInTablix)
			{
				return true;
			}
			RowInfo rowInfo = this.m_detailRows[rowIndex + memberCell.RowSpan - 1];
			if (rowInfo.PageBreaksAtEnd && spanContent == 0)
			{
				pageContext.Common.RegisterPageBreakProperties(rowInfo.PageBreakPropertiesAtEnd, true);
				this.MoveRowToNextPage(rowIndex + memberCell.RowSpan, pageContext);
				return true;
			}
			return false;
		}

		private bool MarkLeafOnPage(ref bool leafsOnPage, HeaderFooterRows prevHeaders, HeaderFooterRows prevFooters, HeaderFooterRows headers, HeaderFooterRows footers, ref double endInTablix, int rowIndex, bool lastMember, double spaceOnPage, ref bool prevFootersOnPage, PageContext pageContext)
		{
			bool flag = false;
			if (!leafsOnPage)
			{
				leafsOnPage = true;
				double num = spaceOnPage;
				flag = this.ResolveHeaders(headers, ref num, num, pageContext);
				if (!flag)
				{
					flag = this.ResolveHeaders(prevHeaders, ref num, num, pageContext);
					if (!flag)
					{
						flag = this.ResolveFooters(footers, ref num, num, lastMember, ref prevFootersOnPage, pageContext);
						if (!flag)
						{
							flag = this.ResolveFooters(prevFooters, ref num, num, lastMember, ref prevFootersOnPage, pageContext);
						}
					}
				}
				endInTablix -= spaceOnPage - num;
			}
			if (!flag)
			{
				RowInfo rowInfo = this.m_detailRows[rowIndex - 1];
				if (rowInfo.PageBreaksAtEnd)
				{
					flag = true;
					pageContext.Common.RegisterPageBreakProperties(rowInfo.PageBreakPropertiesAtEnd, true);
				}
			}
			if (flag)
			{
				this.MoveRowToNextPage(rowIndex, pageContext);
			}
			return flag;
		}

		internal int MarkSpanRHMemberDetailRows(List<PageStructMemberCell> members, double startInTablix, double endInTablix, int rowIndex, int rowsOnPage, int spanContent, PageContext pageContext)
		{
			if (members == null)
			{
				RowInfo rowInfo = this.m_detailRows[rowIndex];
				if (rowInfo.ResolveVertical(startInTablix, endInTablix, pageContext) && rowIndex + 1 < this.m_detailRows.Count)
				{
					this.MarkDetailRowVerticalState(rowIndex + 1, RowInfo.VerticalState.Unknown);
				}
				return 1;
			}
			int num = 0;
			int num2 = 0;
			PageStructMemberCell pageStructMemberCell = null;
			RoundedDouble roundedDouble = new RoundedDouble(0.0);
			RoundedDouble roundedDouble2 = new RoundedDouble(0.0);
			RoundedDouble roundedDouble3 = new RoundedDouble(0.0);
			RoundedDouble roundedDouble4 = new RoundedDouble(0.0);
			RoundedDouble roundedDouble5 = new RoundedDouble(0.0);
			while (num < members.Count)
			{
				this.NormalizeHeights(members, num, rowIndex, startInTablix, endInTablix);
				pageStructMemberCell = members[num];
				if (!pageStructMemberCell.HasInstances)
				{
					return num2;
				}
				roundedDouble.Value = pageStructMemberCell.StartPos;
				roundedDouble2.Value = pageStructMemberCell.EndPos;
				if (!(roundedDouble >= endInTablix) && this.m_detailRows[rowIndex].PageVerticalState != RowInfo.VerticalState.TopOfNextPage)
				{
					if (pageStructMemberCell is PageStructStaticMemberCell)
					{
						PageStructStaticMemberCell pageStructStaticMemberCell = (PageStructStaticMemberCell)pageStructMemberCell;
						PageMemberCell memberInstance = pageStructStaticMemberCell.MemberInstance;
						roundedDouble3.Value = memberInstance.StartPos;
						roundedDouble4.Value = memberInstance.EndPos;
						roundedDouble5.Value = memberInstance.ContentBottom;
						if (roundedDouble4 <= startInTablix)
						{
							rowIndex += memberInstance.RowSpan;
							num++;
						}
						else if (roundedDouble3 < startInTablix)
						{
							if (memberInstance.MemberItem != null)
							{
								spanContent++;
							}
							bool flag = this.MarkSpanRHMemberOnPage(memberInstance, startInTablix, endInTablix, rowIndex, rowsOnPage, spanContent, pageContext);
							if (memberInstance.MemberItem != null)
							{
								spanContent--;
							}
							num2 += memberInstance.CurrRowSpan;
							if (flag)
							{
								return num2;
							}
							rowsOnPage += memberInstance.CurrRowSpan;
							rowIndex += memberInstance.RowSpan;
							num++;
						}
						else
						{
							if (rowsOnPage > 0 && spanContent == 0)
							{
								bool flag2 = false;
								RowInfo rowInfo2 = this.m_detailRows[rowIndex];
								if (rowInfo2.PageBreaksAtStart)
								{
									flag2 = true;
								}
								else if (roundedDouble5 > endInTablix)
								{
									flag2 = true;
								}
								else if (roundedDouble4 > endInTablix)
								{
									RoundedDouble x = new RoundedDouble(memberInstance.SizeValue);
									if (x <= pageContext.ColumnHeight)
									{
										if (memberInstance.KeepTogether)
										{
											flag2 = true;
											this.TraceKeepTogetherRowMember(pageContext, pageStructStaticMemberCell);
										}
										else if (memberInstance.Children == null && !memberInstance.SpanPages && rowInfo2.KeepTogether)
										{
											flag2 = true;
											this.TraceKeepTogetherRow(pageContext, rowInfo2);
										}
									}
								}
								if (flag2)
								{
									this.MarkDetailRowVerticalState(rowIndex, RowInfo.VerticalState.TopOfNextPage, pageContext, true);
									return num2;
								}
							}
							if (roundedDouble4 <= endInTablix && !memberInstance.SpanPages && !pageStructStaticMemberCell.PartialItem)
							{
								rowIndex += memberInstance.RowSpan;
								num2 += memberInstance.CurrRowSpan;
								if (!memberInstance.Hidden)
								{
									rowsOnPage += memberInstance.CurrRowSpan;
								}
							}
							else if (this.MarkSpanRHMember(memberInstance, roundedDouble5, startInTablix, endInTablix, ref rowIndex, ref num2, rowsOnPage, spanContent, pageContext))
							{
								return num2;
							}
							num++;
						}
					}
					else
					{
						PageStructDynamicMemberCell pageStructDynamicMemberCell = (PageStructDynamicMemberCell)pageStructMemberCell;
						PageMemberCell pageMemberCell = pageStructDynamicMemberCell.MemberInstances[0];
						int num3 = pageStructDynamicMemberCell.MemberInstances.Count - 1;
						int num4 = num3;
						int num5 = 0;
						int num6 = rowIndex + pageStructDynamicMemberCell.Span;
						if (pageStructDynamicMemberCell.PartialItem && !pageStructDynamicMemberCell.CreateItem)
						{
							num4--;
						}
						roundedDouble3.Value = pageMemberCell.StartPos;
						if (roundedDouble3 < startInTablix)
						{
							if (pageMemberCell.MemberItem != null)
							{
								spanContent++;
							}
							using (pageStructDynamicMemberCell.MemberInstances.GetAndPin(0, out pageMemberCell))
							{
								bool flag3 = this.MarkSpanRHMemberOnPage(pageMemberCell, startInTablix, endInTablix, rowIndex, rowsOnPage, spanContent, pageContext);
								if (pageMemberCell.MemberItem != null)
								{
									spanContent--;
								}
								num2 += pageMemberCell.CurrRowSpan;
								if (flag3)
								{
									return num2;
								}
								rowsOnPage += pageMemberCell.CurrRowSpan;
								rowIndex += pageMemberCell.RowSpan;
							}
							num5++;
						}
						while (num5 <= num3)
						{
							string text = null;
							if (!pageContext.Common.CanSetPageName)
							{
								text = pageContext.Common.PageName;
							}
							this.NormalizeHeights(pageStructDynamicMemberCell.MemberInstances, num5, rowIndex, startInTablix, endInTablix);
							if (num5 > 0)
							{
								pageMemberCell = pageStructDynamicMemberCell.MemberInstances[num5];
							}
							roundedDouble3.Value = pageMemberCell.StartPos;
							roundedDouble4.Value = pageMemberCell.EndPos;
							roundedDouble5.Value = pageMemberCell.ContentBottom;
							if (!(roundedDouble3 >= endInTablix) && this.m_detailRows[rowIndex].PageVerticalState != RowInfo.VerticalState.TopOfNextPage)
							{
								if (rowsOnPage > 0 && spanContent == 0)
								{
									bool flag4 = false;
									RowInfo rowInfo3 = this.m_detailRows[rowIndex];
									if (rowInfo3.PageBreaksAtStart)
									{
										flag4 = true;
									}
									else
									{
										RoundedDouble roundedDouble6 = new RoundedDouble(0.0);
										if (roundedDouble5 > endInTablix)
										{
											flag4 = true;
										}
										if (!pageMemberCell.SpanPages && roundedDouble4 > endInTablix && (pageMemberCell.KeepTogether || (pageMemberCell.Children == null && rowInfo3.KeepTogether)))
										{
											roundedDouble6.Value = pageMemberCell.SizeValue;
											if (roundedDouble6 <= pageContext.ColumnHeight)
											{
												flag4 = true;
												this.TraceKeepTogetherRowMemberRow(pageContext, rowInfo3, pageStructDynamicMemberCell, pageMemberCell.KeepTogether);
											}
										}
									}
									if (flag4)
									{
										this.MarkDetailRowVerticalState(rowIndex, RowInfo.VerticalState.TopOfNextPage, pageContext, true);
										return num2;
									}
								}
								if (!pageStructDynamicMemberCell.SpanPages && !pageStructDynamicMemberCell.Hidden)
								{
									int num7 = num6;
									int num8 = this.AdvanceInDynamicMember(pageStructDynamicMemberCell, rowIndex, ref num7, endInTablix, num5, num3);
									if (num8 > num5)
									{
										rowsOnPage += num7 - rowIndex;
										num2 += num7 - rowIndex;
										rowIndex = num7;
										this.ProcessVisibleDynamicMemberOnPage(pageContext, pageStructDynamicMemberCell, num5, num8);
										num5 = num8;
										continue;
									}
								}
								if (num5 <= num4 && !pageMemberCell.SpanPages && roundedDouble4 <= endInTablix)
								{
									rowIndex += pageMemberCell.RowSpan;
									num2 += pageMemberCell.CurrRowSpan;
									if (!pageMemberCell.Hidden)
									{
										rowsOnPage += pageMemberCell.CurrRowSpan;
										this.ProcessFullVisibleMemberCellOnPage(pageContext, pageMemberCell);
									}
								}
								else
								{
									using (pageStructDynamicMemberCell.MemberInstances.GetAndPin(num5, out pageMemberCell))
									{
										if (this.MarkSpanRHMember(pageMemberCell, roundedDouble5, startInTablix, endInTablix, ref rowIndex, ref num2, rowsOnPage, spanContent, pageContext))
										{
											if (text != null)
											{
												pageContext.Common.SetPageName(text, true);
											}
											return num2;
										}
									}
								}
								num5++;
								if (text != null)
								{
									pageContext.Common.SetPageName(text, true);
								}
								continue;
							}
							if (!(roundedDouble3 == endInTablix))
							{
								return num2;
							}
							this.MarkDetailRowVerticalState(rowIndex, RowInfo.VerticalState.TopOfNextPage, pageContext, true);
							break;
						}
						num++;
					}
					continue;
				}
				if (!(roundedDouble == endInTablix))
				{
					return num2;
				}
				this.MarkDetailRowVerticalState(rowIndex, RowInfo.VerticalState.TopOfNextPage, pageContext, true);
				break;
			}
			return num2;
		}

		private bool MarkSpanRHMember(PageMemberCell memberCell, RoundedDouble contentBottom, double startInTablix, double endInTablix, ref int rowIndex, ref int levelRowsOnPage, int rowsOnPage, int spanContent, PageContext pageContext)
		{
			if (contentBottom <= endInTablix && memberCell.Children != null)
			{
				int num = 0;
				num = this.MarkSpanRHMemberDetailRows(memberCell.Children, startInTablix, endInTablix, rowIndex, rowsOnPage, spanContent, pageContext);
				if (num == 0)
				{
					return true;
				}
				this.ProcessVisibleMemberCellOnPage(pageContext, memberCell);
				rowIndex += memberCell.RowSpan;
				if (num < memberCell.CurrRowSpan)
				{
					memberCell.CurrRowSpan = num;
					levelRowsOnPage += memberCell.CurrRowSpan;
					return true;
				}
				levelRowsOnPage += memberCell.CurrRowSpan;
				RowInfo rowInfo = this.m_detailRows[rowIndex - 1];
				if (rowInfo.PageBreaksAtEnd && spanContent == 0)
				{
					pageContext.Common.RegisterPageBreakProperties(rowInfo.PageBreakPropertiesAtEnd, true);
					this.MoveRowToNextPage(rowIndex, pageContext);
					return true;
				}
				return false;
			}
			if (memberCell.MemberItem != null)
			{
				spanContent++;
			}
			this.MarkSpanRHMemberOnPage(memberCell, startInTablix, endInTablix, rowIndex, rowsOnPage, spanContent, pageContext);
			if (memberCell.MemberItem != null)
			{
				spanContent--;
			}
			levelRowsOnPage += memberCell.CurrRowSpan;
			this.ProcessVisibleMemberCellOnPage(pageContext, memberCell);
			return true;
		}

		private void ProcessVisibleDynamicMemberOnPage(PageContext pageContext, PageStructDynamicMemberCell dynamicMember, int startIndex, int endIndex)
		{
			if (pageContext.Common.CanSetPageName && dynamicMember.HasInnerPageName)
			{
				PageMemberCell pageMemberCell = null;
				if (startIndex < endIndex)
				{
					pageMemberCell = dynamicMember.MemberInstances[startIndex];
					pageContext.Common.SetPageName(pageMemberCell.PageName, false);
					if (pageContext.Common.CanSetPageName)
					{
						if (pageMemberCell.HasInnerPageName)
						{
							PageStructMemberCell pageStructMemberCell = null;
							int num = 0;
							while (true)
							{
								if (num < pageMemberCell.Children.Count)
								{
									pageStructMemberCell = pageMemberCell.Children[num];
									if (!pageStructMemberCell.HasInnerPageName)
									{
										num++;
										continue;
									}
									break;
								}
								return;
							}
							this.ProcessVisibleDynamicMemberOnPage(pageContext, pageStructMemberCell as PageStructDynamicMemberCell);
						}
						else
						{
							startIndex++;
						}
					}
				}
			}
		}

		private void ProcessVisibleDynamicMemberOnPage(PageContext pageContext, PageStructDynamicMemberCell dynamicMember)
		{
			if (dynamicMember != null && dynamicMember.MemberInstances != null)
			{
				this.ProcessVisibleDynamicMemberOnPage(pageContext, dynamicMember, 0, dynamicMember.MemberInstances.Count);
			}
		}

		private void ProcessFullVisibleMemberCellOnPage(PageContext pageContext, PageMemberCell memberCell)
		{
			pageContext.Common.SetPageName(memberCell.PageName, false);
			if (pageContext.Common.CanSetPageName && memberCell.HasInnerPageName)
			{
				PageStructMemberCell pageStructMemberCell = null;
				int num = 0;
				while (true)
				{
					if (num < memberCell.Children.Count)
					{
						pageStructMemberCell = memberCell.Children[num];
						if (!pageStructMemberCell.HasInnerPageName)
						{
							num++;
							continue;
						}
						break;
					}
					return;
				}
				this.ProcessVisibleDynamicMemberOnPage(pageContext, pageStructMemberCell as PageStructDynamicMemberCell);
			}
		}

		private void ProcessVisibleMemberCellOnPage(PageContext pageContext, PageMemberCell memberCell)
		{
			pageContext.Common.SetPageName(memberCell.PageName, true);
		}

		internal int MarkDetailRowsForVerticalPage(List<PageStructMemberCell> members, double startInTablix, ref double endInTablix, HeaderFooterRows prevHeaders, HeaderFooterRows prevFooters, ref bool prevFootersOnPage, ref bool leafsOnPage, int rowIndex, int rowsOnPage, PageContext pageContext)
		{
			if (members == null)
			{
				return 1;
			}
			int num = 0;
			HeaderFooterRows headerFooterRows = null;
			HeaderFooterRows headerFooterRows2 = null;
			int num2 = 0;
			PageStructMemberCell pageStructMemberCell = null;
			RoundedDouble roundedDouble = new RoundedDouble(0.0);
			RoundedDouble roundedDouble2 = new RoundedDouble(0.0);
			RoundedDouble roundedDouble3 = new RoundedDouble(0.0);
			RoundedDouble roundedDouble4 = new RoundedDouble(0.0);
			RoundedDouble roundedDouble5 = new RoundedDouble(0.0);
			while (num < members.Count)
			{
				this.NormalizeHeights(members, num, rowIndex, startInTablix, endInTablix);
				pageStructMemberCell = members[num];
				if (!pageStructMemberCell.HasInstances)
				{
					return num2;
				}
				roundedDouble.Value = pageStructMemberCell.StartPos;
				roundedDouble2.Value = pageStructMemberCell.EndPos;
				if (!(roundedDouble >= endInTablix) && this.m_detailRows[rowIndex].PageVerticalState != RowInfo.VerticalState.TopOfNextPage)
				{
					if (pageStructMemberCell is PageStructStaticMemberCell)
					{
						PageStructStaticMemberCell pageStructStaticMemberCell = (PageStructStaticMemberCell)pageStructMemberCell;
						PageMemberCell memberInstance = pageStructStaticMemberCell.MemberInstance;
						roundedDouble3.Value = memberInstance.StartPos;
						roundedDouble4.Value = memberInstance.EndPos;
						roundedDouble5.Value = memberInstance.ContentBottom;
						if (roundedDouble4 <= startInTablix)
						{
							if (pageStructStaticMemberCell.Header && pageStructStaticMemberCell.RepeatWith)
							{
								if (headerFooterRows == null)
								{
									headerFooterRows = new HeaderFooterRows();
								}
								headerFooterRows.AddHeaderRow(rowIndex, memberInstance.RowSpan, memberInstance.SizeValue, false, true, false, pageStructStaticMemberCell);
							}
							rowIndex += memberInstance.RowSpan;
							num++;
							continue;
						}
						if (roundedDouble3 < startInTablix)
						{
							leafsOnPage = true;
							int num3 = 0;
							if (memberInstance.MemberItem != null)
							{
								num3++;
							}
							bool flag = this.MarkSpanRHMemberOnPage(memberInstance, startInTablix, endInTablix, rowIndex, rowsOnPage, num3, pageContext);
							num2 += memberInstance.CurrRowSpan;
							if (flag)
							{
								return num2;
							}
							rowsOnPage += memberInstance.CurrRowSpan;
							rowIndex += memberInstance.RowSpan;
							num++;
							if (headerFooterRows != null)
							{
								headerFooterRows.CheckInvalidatedPageRepeat(pageContext, this);
							}
							if (headerFooterRows2 != null)
							{
								headerFooterRows2.CheckInvalidatedPageRepeat(pageContext, this);
							}
							continue;
						}
						if (rowsOnPage > 0)
						{
							bool flag2 = false;
							RowInfo rowInfo = this.m_detailRows[rowIndex];
							RoundedDouble roundedDouble6 = new RoundedDouble(rowInfo.Top);
							if (rowInfo.PageBreaksAtStart && roundedDouble6 > startInTablix)
							{
								flag2 = true;
							}
							else if (roundedDouble5 > endInTablix)
							{
								flag2 = true;
							}
							else if (memberInstance.Children == null && prevFootersOnPage && (memberInstance.SpanPages || roundedDouble4 > endInTablix))
							{
								flag2 = true;
							}
							else if (roundedDouble4 > endInTablix)
							{
								roundedDouble6.Value = memberInstance.SizeValue;
								if (roundedDouble6 <= pageContext.ColumnHeight)
								{
									if (memberInstance.KeepTogether)
									{
										flag2 = true;
										this.TraceKeepTogetherRowMember(pageContext, pageStructStaticMemberCell);
									}
									else if (memberInstance.Children == null && !memberInstance.SpanPages && rowInfo.KeepTogether)
									{
										flag2 = true;
										this.TraceKeepTogetherRow(pageContext, rowInfo);
									}
								}
							}
							if (flag2)
							{
								this.MarkDetailRowVerticalState(rowIndex, RowInfo.VerticalState.TopOfNextPage, pageContext, true);
								return num2;
							}
						}
						if (!memberInstance.Hidden)
						{
							int num4 = num;
							if (pageStructStaticMemberCell.Header && pageStructStaticMemberCell.KeepWith && rowsOnPage > 0 && !memberInstance.SpanPages)
							{
								bool flag3 = false;
								double pageHeight = endInTablix - memberInstance.StartPos;
								if (this.MoveKeepWithHeaders(members, ref num4, rowIndex, pageHeight, prevHeaders, prevFooters, headerFooterRows, leafsOnPage, prevFootersOnPage, pageContext))
								{
									if (num < num4)
									{
										rowIndex += pageStructMemberCell.Span;
										num2 += pageStructMemberCell.Span;
										for (num++; num < num4; num++)
										{
											pageStructMemberCell = members[num];
											roundedDouble2.Value = pageStructMemberCell.EndPos;
											memberInstance = ((PageStructStaticMemberCell)pageStructMemberCell).MemberInstance;
											if (roundedDouble2 <= endInTablix)
											{
												rowIndex += memberInstance.CurrRowSpan;
												num2 += memberInstance.CurrRowSpan;
											}
										}
									}
									this.MarkDetailRowVerticalState(rowIndex, RowInfo.VerticalState.TopOfNextPage, pageContext, true);
									return num2 + this.BringHeadersFootersOnPage(headerFooterRows, headerFooterRows2);
								}
							}
							if (num < num4)
							{
								bool flag4 = false;
								while (num < num4)
								{
									pageStructMemberCell = members[num];
									rowIndex += pageStructMemberCell.Span;
									if (!pageStructMemberCell.Hidden)
									{
										rowsOnPage += pageStructMemberCell.Span;
									}
									num2 += pageStructMemberCell.Span;
									num++;
									if (pageStructMemberCell is PageStructDynamicMemberCell)
									{
										flag4 = true;
										this.ProcessVisibleDynamicMemberOnPage(pageContext, pageStructMemberCell as PageStructDynamicMemberCell);
									}
									else if (!flag4)
									{
										memberInstance = ((PageStructStaticMemberCell)pageStructMemberCell).MemberInstance;
										if (headerFooterRows == null)
										{
											headerFooterRows = new HeaderFooterRows();
										}
										headerFooterRows.AddHeaderRow(rowIndex, memberInstance.RowSpan, memberInstance.SizeValue, true, pageStructStaticMemberCell.RepeatWith, true, pageStructMemberCell);
									}
								}
								if (flag4)
								{
									headerFooterRows = null;
									if (this.MarkLeafOnPage(ref leafsOnPage, prevHeaders, prevFooters, (HeaderFooterRows)null, (HeaderFooterRows)null, ref endInTablix, rowIndex, this.IsLastVisibleMember(num4 - 1, members), endInTablix - pageStructMemberCell.EndPos, ref prevFootersOnPage, pageContext))
									{
										return num2;
									}
								}
								continue;
							}
							if (pageStructStaticMemberCell.Header && (pageStructStaticMemberCell.KeepWith || pageStructStaticMemberCell.RepeatWith))
							{
								if (headerFooterRows == null)
								{
									headerFooterRows = new HeaderFooterRows();
								}
								headerFooterRows.AddHeaderRow(rowIndex, memberInstance.RowSpan, memberInstance.SizeValue, true, pageStructStaticMemberCell.RepeatWith, pageStructStaticMemberCell.KeepWith, pageStructStaticMemberCell);
							}
						}
						if (roundedDouble4 <= endInTablix && !memberInstance.SpanPages && !pageStructStaticMemberCell.PartialItem)
						{
							if (memberInstance.Hidden)
							{
								rowIndex += memberInstance.RowSpan;
								num2 += pageStructMemberCell.Span;
								num++;
							}
							else
							{
								num++;
								if (leafsOnPage && this.IsLastVisibleMember(num - 1, members) && prevFooters != null && this.ResolveKeepWithFooters(prevFooters, null, memberInstance, endInTablix, pageContext))
								{
									this.MarkDetailRowVerticalState(rowIndex, RowInfo.VerticalState.TopOfNextPage, pageContext, true);
									return num2;
								}
								bool flag5 = false;
								if (pageStructStaticMemberCell.Footer && pageStructStaticMemberCell.KeepWith && rowsOnPage == 0)
								{
									flag5 = true;
								}
								rowIndex += memberInstance.RowSpan;
								rowsOnPage += memberInstance.CurrRowSpan;
								num2 += memberInstance.CurrRowSpan;
								if (!pageStructStaticMemberCell.KeepWith || flag5)
								{
									bool flag6 = false;
									this.ProcessFullVisibleMemberCellOnPage(pageContext, memberInstance);
									if (this.MarkLeafOnPage(ref leafsOnPage, prevHeaders, prevFooters, (HeaderFooterRows)null, (HeaderFooterRows)null, ref endInTablix, rowIndex, this.IsLastVisibleMember(num - 1, members), endInTablix - memberInstance.EndPos, ref prevFootersOnPage, pageContext))
									{
										return num2 + this.BringHeadersFootersOnPage(headerFooterRows, null);
									}
								}
							}
							continue;
						}
						if (roundedDouble5 <= endInTablix && memberInstance.Children != null)
						{
							int num5 = 0;
							num5 = this.MarkDetailRowsForVerticalPage(memberInstance.Children, startInTablix, ref endInTablix, prevHeaders, prevFooters, ref prevFootersOnPage, ref leafsOnPage, rowIndex, rowsOnPage, pageContext);
							if (num5 == 0)
							{
								return num2 + this.BringHeadersFootersOnPage(headerFooterRows, null);
							}
							int num6 = this.CalculateCurrentRowSpan(memberInstance, rowIndex);
							rowIndex += memberInstance.RowSpan;
							if (num5 < num6)
							{
								memberInstance.CurrRowSpan = num5;
								num2 += memberInstance.CurrRowSpan;
								return num2 + this.BringHeadersFootersOnPage(headerFooterRows, null);
							}
							memberInstance.CurrRowSpan = num6;
							num2 += memberInstance.CurrRowSpan;
							RowInfo rowInfo2 = this.m_detailRows[rowIndex - 1];
							if (rowInfo2.PageBreaksAtEnd)
							{
								pageContext.Common.RegisterPageBreakProperties(rowInfo2.PageBreakPropertiesAtEnd, true);
								this.MoveRowToNextPage(rowIndex, pageContext);
								return num2 + this.BringHeadersFootersOnPage(headerFooterRows, null);
							}
							rowsOnPage += memberInstance.CurrRowSpan;
							num++;
							continue;
						}
						int num7 = 0;
						if (memberInstance.MemberItem != null)
						{
							num7++;
						}
						this.MarkSpanRHMemberOnPage(memberInstance, startInTablix, endInTablix, rowIndex, rowsOnPage, num7, pageContext);
						num2 += memberInstance.CurrRowSpan;
						if (headerFooterRows != null)
						{
							headerFooterRows.CheckInvalidatedPageRepeat(pageContext, this);
						}
						if (headerFooterRows2 != null)
						{
							headerFooterRows2.CheckInvalidatedPageRepeat(pageContext, this);
						}
						return num2;
					}
					PageStructDynamicMemberCell pageStructDynamicMemberCell = (PageStructDynamicMemberCell)pageStructMemberCell;
					PageMemberCell pageMemberCell = pageStructDynamicMemberCell.MemberInstances[0];
					int num8 = pageStructDynamicMemberCell.MemberInstances.Count - 1;
					int num9 = 0;
					bool flag7 = false;
					bool flag8 = false;
					int num10 = rowIndex + pageStructDynamicMemberCell.Span;
					headerFooterRows2 = this.CollectFooters(members, num + 1, rowIndex + pageStructDynamicMemberCell.Span, ref flag7, ref flag8);
					if (headerFooterRows2 != null && flag8)
					{
						if (pageStructDynamicMemberCell.PartialItem)
						{
							flag8 = false;
							headerFooterRows2.SetRowsKeepWith(false);
						}
						else if (this.m_detailRows[rowIndex + pageStructDynamicMemberCell.Span - 1].PageBreaksAtEnd)
						{
							flag8 = false;
							headerFooterRows2.SetRowsKeepWith(false);
						}
						else if (num8 > 0)
						{
							headerFooterRows2.SetRowsKeepWith(false);
						}
					}
					roundedDouble3.Value = pageMemberCell.StartPos;
					if (roundedDouble3 < startInTablix)
					{
						leafsOnPage = true;
						int num11 = 0;
						if (pageMemberCell.MemberItem != null)
						{
							num11++;
						}
						using (pageStructDynamicMemberCell.MemberInstances.GetAndPin(0, out pageMemberCell))
						{
							bool flag9 = this.MarkSpanRHMemberOnPage(pageMemberCell, startInTablix, endInTablix, rowIndex, rowsOnPage, num11, pageContext);
							num2 += pageMemberCell.CurrRowSpan;
							if (flag9)
							{
								return num2;
							}
							rowsOnPage += pageMemberCell.CurrRowSpan;
							rowIndex += pageMemberCell.RowSpan;
						}
						num9++;
						if (headerFooterRows != null)
						{
							headerFooterRows.CheckInvalidatedPageRepeat(pageContext, this);
						}
						if (headerFooterRows2 != null)
						{
							headerFooterRows2.CheckInvalidatedPageRepeat(pageContext, this);
						}
					}
					HeaderFooterRows prevHeaders2 = this.MergeHeadersFooters(prevHeaders, headerFooterRows);
					HeaderFooterRows prevFooters2 = this.MergeHeadersFooters(prevFooters, headerFooterRows2);
					while (num9 <= num8)
					{
						string text = null;
						if (!pageContext.Common.CanSetPageName)
						{
							text = pageContext.Common.PageName;
						}
						this.NormalizeHeights(pageStructDynamicMemberCell.MemberInstances, num9, rowIndex, startInTablix, endInTablix);
						if (num9 > 0)
						{
							pageMemberCell = pageStructDynamicMemberCell.MemberInstances[num9];
						}
						roundedDouble3.Value = pageMemberCell.StartPos;
						roundedDouble4.Value = pageMemberCell.EndPos;
						roundedDouble5.Value = pageMemberCell.ContentBottom;
						if (!(roundedDouble3 >= endInTablix) && this.m_detailRows[rowIndex].PageVerticalState != RowInfo.VerticalState.TopOfNextPage)
						{
							if (headerFooterRows2 != null && flag8 && !pageStructDynamicMemberCell.PartialItem && num9 == num8)
							{
								headerFooterRows2.SetRowsKeepWith(true);
							}
							if (rowsOnPage > 0)
							{
								bool flag10 = false;
								RowInfo rowInfo3 = this.m_detailRows[rowIndex];
								RoundedDouble roundedDouble7 = new RoundedDouble(rowInfo3.Top);
								if (rowInfo3.PageBreaksAtStart && roundedDouble7 > startInTablix)
								{
									flag10 = true;
								}
								else
								{
									if (roundedDouble5 > endInTablix)
									{
										flag10 = true;
									}
									else if (pageMemberCell.KeepTogether && roundedDouble4 > endInTablix && (num9 > 0 || headerFooterRows == null || headerFooterRows.CountNotOnPage == headerFooterRows.Count))
									{
										RoundedDouble roundedDouble8 = new RoundedDouble(pageMemberCell.SizeValue);
										if (headerFooterRows != null)
										{
											if (num9 == 0)
											{
												roundedDouble8.Value += headerFooterRows.Height;
											}
											else
											{
												roundedDouble8.Value += headerFooterRows.RepeatHeight;
											}
										}
										if (headerFooterRows2 != null)
										{
											if (num9 == num8)
											{
												roundedDouble8.Value += headerFooterRows2.KeepWithAndRepeatHeight;
											}
											else
											{
												roundedDouble8.Value += headerFooterRows2.RepeatHeight;
											}
										}
										roundedDouble8.Value += this.SpaceNeededForFullPage(prevHeaders, prevFooters, flag7);
										if (roundedDouble8 <= pageContext.ColumnHeight)
										{
											flag10 = true;
											this.TraceKeepTogetherRowMember(pageContext, pageStructDynamicMemberCell);
										}
									}
									if (!flag10 && pageMemberCell.Children == null)
									{
										if (pageMemberCell.SpanPages)
										{
											if (prevFootersOnPage)
											{
												flag10 = true;
											}
										}
										else if (roundedDouble4 > endInTablix)
										{
											if (prevFootersOnPage)
											{
												flag10 = true;
											}
											else if (rowInfo3.KeepTogether || pageMemberCell.KeepTogether)
											{
												roundedDouble7.Value = pageMemberCell.SizeValue;
												if (roundedDouble7 <= pageContext.ColumnHeight)
												{
													flag10 = true;
													this.TraceKeepTogetherRowMemberRow(pageContext, rowInfo3, pageStructDynamicMemberCell, pageMemberCell.KeepTogether);
												}
											}
										}
									}
								}
								if (flag10)
								{
									this.MarkDetailRowVerticalState(rowIndex, RowInfo.VerticalState.TopOfNextPage, pageContext, true);
									return num2 + this.BringHeadersFootersOnPage(headerFooterRows, headerFooterRows2);
								}
							}
							if (num9 > 0 && num9 < num8)
							{
								if (!pageStructDynamicMemberCell.SpanPages)
								{
									int num12 = num10;
									int num13 = this.AdvanceInDynamicMember(pageStructDynamicMemberCell, rowIndex, ref num12, endInTablix, num9, num8);
									if (num13 > num9)
									{
										rowsOnPage += num12 - rowIndex;
										num2 += num12 - rowIndex;
										rowIndex = num12;
										this.ProcessVisibleDynamicMemberOnPage(pageContext, pageStructDynamicMemberCell, num9, num13);
										num9 = num13;
										continue;
									}
								}
								else if (roundedDouble4 <= endInTablix && !pageMemberCell.SpanPages)
								{
									rowIndex += pageMemberCell.RowSpan;
									rowsOnPage += pageMemberCell.CurrRowSpan;
									num2 += pageMemberCell.CurrRowSpan;
									num9++;
									this.ProcessFullVisibleMemberCellOnPage(pageContext, pageMemberCell);
									continue;
								}
							}
							if (roundedDouble5 <= endInTablix && pageMemberCell.Children != null)
							{
								if (pageMemberCell.Hidden)
								{
									rowIndex += pageMemberCell.RowSpan;
									num2 += pageMemberCell.CurrRowSpan;
								}
								else
								{
									int num14 = 0;
									using (pageStructDynamicMemberCell.MemberInstances.GetAndPin(num9, out pageMemberCell))
									{
										num14 = this.MarkDetailRowsForVerticalPage(pageMemberCell.Children, startInTablix, ref endInTablix, prevHeaders2, prevFooters2, ref prevFootersOnPage, ref leafsOnPage, rowIndex, rowsOnPage, pageContext);
										if (num14 == 0)
										{
											num2 += this.BringHeadersFootersOnPage(headerFooterRows, headerFooterRows2);
											return num2;
										}
										this.ProcessVisibleMemberCellOnPage(pageContext, pageMemberCell);
										int num15 = this.CalculateCurrentRowSpan(pageMemberCell, rowIndex);
										rowIndex += pageMemberCell.RowSpan;
										if (num14 < num15)
										{
											pageMemberCell.CurrRowSpan = num14;
											num2 += pageMemberCell.CurrRowSpan;
											num2 += this.BringHeadersFootersOnPage(headerFooterRows, headerFooterRows2);
											if (text != null)
											{
												pageContext.Common.SetPageName(text, true);
											}
											return num2;
										}
										pageMemberCell.CurrRowSpan = num15;
										num2 += pageMemberCell.CurrRowSpan;
										RowInfo rowInfo4 = this.m_detailRows[rowIndex - 1];
										if (rowInfo4.PageBreaksAtEnd)
										{
											if (!rowInfo4.SpanPagesRow)
											{
												pageContext.Common.RegisterPageBreakProperties(rowInfo4.PageBreakPropertiesAtEnd, true);
											}
											this.MoveRowToNextPage(rowIndex, pageContext);
											num2 += this.BringHeadersFootersOnPage(headerFooterRows, headerFooterRows2);
											if (text != null)
											{
												pageContext.Common.SetPageName(text, true);
											}
											return num2;
										}
										rowsOnPage += pageMemberCell.CurrRowSpan;
									}
								}
								goto IL_0f41;
							}
							if (pageMemberCell.Children == null && roundedDouble4 <= endInTablix && !pageMemberCell.SpanPages)
							{
								if (pageMemberCell.Hidden)
								{
									rowIndex += pageMemberCell.RowSpan;
									num2 += pageMemberCell.CurrRowSpan;
								}
								else
								{
									bool flag11 = false;
									if (leafsOnPage && flag7 && num9 == num8 && prevFooters != null && this.ResolveKeepWithFooters(prevFooters2, headerFooterRows2, pageMemberCell, endInTablix, pageContext))
									{
										this.MarkDetailRowVerticalState(rowIndex, RowInfo.VerticalState.TopOfNextPage, pageContext, true);
										return num2;
									}
									rowIndex += pageMemberCell.RowSpan;
									rowsOnPage += pageMemberCell.CurrRowSpan;
									num2 += pageMemberCell.CurrRowSpan;
									this.ProcessFullVisibleMemberCellOnPage(pageContext, pageMemberCell);
									if (this.MarkLeafOnPage(ref leafsOnPage, prevHeaders, prevFooters, headerFooterRows, headerFooterRows2, ref endInTablix, rowIndex, flag7, endInTablix - pageMemberCell.EndPos, ref prevFootersOnPage, pageContext))
									{
										num2 += this.BringHeadersFootersOnPage(headerFooterRows, headerFooterRows2);
										if (text != null)
										{
											pageContext.Common.SetPageName(text, true);
										}
										return num2;
									}
								}
								goto IL_0f41;
							}
							int num16 = 0;
							if (pageMemberCell.MemberItem != null)
							{
								num16++;
							}
							using (pageStructDynamicMemberCell.MemberInstances.GetAndPin(num9, out pageMemberCell))
							{
								this.MarkSpanRHMemberOnPage(pageMemberCell, startInTablix, endInTablix, rowIndex, rowsOnPage, num16, pageContext);
								this.ProcessVisibleMemberCellOnPage(pageContext, pageMemberCell);
								num2 += pageMemberCell.CurrRowSpan;
							}
							if (text != null)
							{
								pageContext.Common.SetPageName(text, true);
							}
							if (headerFooterRows != null)
							{
								headerFooterRows.CheckInvalidatedPageRepeat(pageContext, this);
							}
							if (headerFooterRows2 != null)
							{
								headerFooterRows2.CheckInvalidatedPageRepeat(pageContext, this);
							}
							return num2;
						}
						num2 += this.BringHeadersFootersOnPage(headerFooterRows, headerFooterRows2);
						if (!(roundedDouble3 == endInTablix))
						{
							return num2;
						}
						this.MarkDetailRowVerticalState(rowIndex, RowInfo.VerticalState.TopOfNextPage, pageContext, true);
						break;
						IL_0f41:
						num9++;
						if (text != null)
						{
							pageContext.Common.SetPageName(text, true);
						}
					}
					int num17 = this.BringHeadersFootersOnPage(headerFooterRows, null);
					num2 += num17;
					rowsOnPage += num17;
					headerFooterRows = null;
					num++;
					if (headerFooterRows2 != null)
					{
						num17 = headerFooterRows2.Count - headerFooterRows2.CountNotOnPage;
						num += num17;
						num2 += num17;
						rowsOnPage += num17;
						headerFooterRows2 = null;
					}
					continue;
				}
				if (!(roundedDouble == endInTablix))
				{
					return num2;
				}
				this.MarkDetailRowVerticalState(rowIndex, RowInfo.VerticalState.TopOfNextPage, pageContext, true);
				break;
			}
			return num2;
		}

		private int AdvanceInDynamicMember(PageStructDynamicMemberCell dynamicMember, int startRowIndex, ref int endRowIndex, double endInTablix, int start, int end)
		{
			int num = (end - start) / 2;
			int num2 = this.AdvanceInDynamicMember(dynamicMember, endInTablix, start, end);
			if (num2 > num)
			{
				while (end >= num2)
				{
					endRowIndex -= dynamicMember.MemberInstances[end].RowSpan;
					end--;
				}
			}
			else
			{
				while (start < num2)
				{
					startRowIndex += dynamicMember.MemberInstances[start].RowSpan;
					start++;
				}
				endRowIndex = startRowIndex;
			}
			return num2;
		}

		private int AdvanceInDynamicMember(PageStructDynamicMemberCell dynamic, double endInTablix, int start, int end)
		{
			ScalableList<PageMemberCell> memberInstances = dynamic.MemberInstances;
			RoundedDouble roundedDouble = new RoundedDouble(0.0);
			int num = -1;
			int num2 = (end - start) / 2;
			while (num2 > 0)
			{
				roundedDouble.Value = memberInstances[start + num2].EndPos;
				if (roundedDouble < endInTablix)
				{
					start += num2;
					num2 = (end - start) / 2;
					num = start;
				}
				else
				{
					end = start + num2;
					num2 /= 2;
				}
			}
			if (num >= 0)
			{
				return num + 1;
			}
			return start;
		}

		internal HeaderFooterRows CollectFooters(List<PageStructMemberCell> members, int index, int rowIndex, ref bool lastMember, ref bool footersKeepWith)
		{
			if (members == null)
			{
				return null;
			}
			HeaderFooterRows headerFooterRows = null;
			PageStructStaticMemberCell pageStructStaticMemberCell = null;
			bool pageBreaksAtEnd = this.m_detailRows[rowIndex - 1].PageBreaksAtEnd;
			for (int i = index; i < members.Count; i++)
			{
				pageStructStaticMemberCell = (members[i] as PageStructStaticMemberCell);
				if (pageStructStaticMemberCell == null)
				{
					return headerFooterRows;
				}
				if (!pageStructStaticMemberCell.Footer || (!pageStructStaticMemberCell.KeepWith && !pageStructStaticMemberCell.RepeatWith))
				{
					return headerFooterRows;
				}
				if (pageBreaksAtEnd)
				{
					pageStructStaticMemberCell.KeepWith = false;
				}
				if (pageStructStaticMemberCell.KeepWith || pageStructStaticMemberCell.RepeatWith)
				{
					if (headerFooterRows == null)
					{
						headerFooterRows = new HeaderFooterRows();
					}
					headerFooterRows.AddFooterRow(rowIndex, pageStructStaticMemberCell.Span, pageStructStaticMemberCell.MemberInstance.SizeValue, false, pageStructStaticMemberCell.RepeatWith, pageStructStaticMemberCell.KeepWith, pageStructStaticMemberCell);
					footersKeepWith = pageStructStaticMemberCell.KeepWith;
				}
				rowIndex += pageStructStaticMemberCell.Span;
			}
			lastMember = true;
			return headerFooterRows;
		}

		private bool ResolveKeepWithFooters(HeaderFooterRows prevFooters, HeaderFooterRows footers, PageMemberCell memberCell, double endInTablix, PageContext pageContext)
		{
			RoundedDouble roundedDouble = new RoundedDouble(prevFooters.KeepWithNoRepeatNoPageHeight);
			roundedDouble.Value += memberCell.EndPos;
			if (roundedDouble > endInTablix)
			{
				RoundedDouble x = new RoundedDouble(memberCell.SizeValue);
				x += prevFooters.KeepWithNoRepeatNoPageHeight;
				if (x <= pageContext.ColumnHeight)
				{
					return true;
				}
				if (footers != null)
				{
					roundedDouble = new RoundedDouble(footers.KeepWithNoRepeatNoPageHeight);
					roundedDouble.Value += memberCell.EndPos;
					if (roundedDouble > endInTablix)
					{
						x = new RoundedDouble(memberCell.SizeValue);
						x += footers.KeepWithNoRepeatNoPageHeight;
						if (x <= pageContext.ColumnHeight)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		private HeaderFooterRows MergeHeadersFooters(HeaderFooterRows prevRows, HeaderFooterRows levelRows)
		{
			int num = 0;
			if (prevRows != null)
			{
				num += prevRows.Count;
			}
			if (levelRows != null)
			{
				num += levelRows.Count;
			}
			if (num > 0)
			{
				return new HeaderFooterRows(num, prevRows, levelRows);
			}
			return null;
		}

		private int BringHeadersFootersOnPage(HeaderFooterRows levelHeaders, HeaderFooterRows levelFooters)
		{
			int num = 0;
			if (levelHeaders != null)
			{
				num = levelHeaders.BringDetailRowOnPage(this.m_detailRows);
			}
			if (levelFooters != null)
			{
				num += levelFooters.BringDetailRowOnPage(this.m_detailRows);
			}
			return num;
		}

		private void NormalizeHeights(List<PageStructMemberCell> members, int index, int rowIndex, double startInTablix, double endInTablix)
		{
			if (members != null)
			{
				int num = rowIndex - 1;
				RowInfo rowInfo = null;
				for (int i = rowIndex; i < this.m_detailRows.Count; i++)
				{
					rowInfo = this.m_detailRows[i];
					if (rowInfo.PageVerticalState != RowInfo.VerticalState.Above)
					{
						if (rowInfo.PageVerticalState == RowInfo.VerticalState.Unknown)
						{
							if (i > rowIndex)
							{
								num = -1;
							}
							break;
						}
						return;
					}
				}
				double num2 = 0.0;
				int num3 = -1;
				for (int j = index; j < members.Count; j++)
				{
					members[j].NormalizeDetailRowHeight(this.m_detailRows, rowIndex, startInTablix, endInTablix, ref num, ref num3, false, ref num2);
					rowIndex += members[j].Span;
				}
				if (rowIndex < this.m_detailRows.Count)
				{
					this.MarkDetailRowVerticalState(rowIndex, RowInfo.VerticalState.Unknown);
				}
			}
		}

		private void NormalizeHeights(ScalableList<PageMemberCell> members, int index, int rowIndex, double startInTablix, double endInTablix)
		{
			if (members != null)
			{
				int num = rowIndex - 1;
				RowInfo rowInfo = null;
				for (int i = rowIndex; i < this.m_detailRows.Count; i++)
				{
					rowInfo = this.m_detailRows[i];
					if (rowInfo.PageVerticalState != RowInfo.VerticalState.Above)
					{
						if (rowInfo.PageVerticalState == RowInfo.VerticalState.Unknown)
						{
							if (i > rowIndex)
							{
								num = -1;
							}
							break;
						}
						return;
					}
				}
				double num2 = 0.0;
				int num3 = -1;
				PageMemberCell pageMemberCell = null;
				for (int j = index; j < members.Count; j++)
				{
					using (members.GetAndPin(j, out pageMemberCell))
					{
						pageMemberCell.NormalizeDetailRowHeight(this.m_detailRows, rowIndex, startInTablix, endInTablix, ref num, ref num3, false, ref num2);
						rowIndex += pageMemberCell.RowSpan;
					}
				}
				if (rowIndex < this.m_detailRows.Count)
				{
					this.MarkDetailRowVerticalState(rowIndex, RowInfo.VerticalState.Unknown);
				}
			}
		}

		private bool MoveKeepWithHeaders(List<PageStructMemberCell> members, ref int index, int rowIndex, double pageHeight, HeaderFooterRows prevHeaders, HeaderFooterRows prevFooters, HeaderFooterRows headers, bool leafsOnPage, bool prevFootersOnPage, PageContext pageContext)
		{
			if (members == null)
			{
				return false;
			}
			PageStructStaticMemberCell pageStructStaticMemberCell = null;
			PageStructDynamicMemberCell pageStructDynamicMemberCell = null;
			PageStructMemberCell pageStructMemberCell = null;
			HeaderFooterRows headerFooterRows = null;
			HeaderFooterRows headerFooterRows2 = null;
			int i = index;
			int num = -1;
			int rowIndex2 = rowIndex;
			bool flag = true;
			bool flag2 = false;
			bool flag3 = false;
			for (; i < members.Count; i++)
			{
				pageStructStaticMemberCell = (members[i] as PageStructStaticMemberCell);
				if (pageStructStaticMemberCell != null)
				{
					if (pageStructStaticMemberCell.Header)
					{
						if (pageStructDynamicMemberCell == null && pageStructStaticMemberCell.HasInstances)
						{
							if (!pageStructStaticMemberCell.KeepWith)
							{
								pageStructMemberCell = pageStructStaticMemberCell;
								rowIndex2 = rowIndex;
								num = i;
								if (i + 1 < members.Count)
								{
									flag = false;
								}
								break;
							}
							if (headerFooterRows == null)
							{
								headerFooterRows = new HeaderFooterRows();
							}
							headerFooterRows.AddHeaderRow(rowIndex, pageStructStaticMemberCell.Span, pageStructStaticMemberCell.SizeValue, false, pageStructStaticMemberCell.RepeatWith, true, pageStructStaticMemberCell);
							if (headerFooterRows.Height > pageContext.ColumnHeight)
							{
								return false;
							}
							rowIndex += pageStructStaticMemberCell.Span;
							continue;
						}
						flag = false;
					}
					else if (pageStructDynamicMemberCell != null)
					{
						if (pageStructStaticMemberCell.Footer && (pageStructStaticMemberCell.KeepWith || pageStructStaticMemberCell.RepeatWith))
						{
							if (pageStructDynamicMemberCell.PartialItem)
							{
								flag3 = false;
							}
							else
							{
								if (flag2)
								{
									pageStructStaticMemberCell.KeepWith = false;
								}
								flag3 = pageStructStaticMemberCell.KeepWith;
							}
							if (flag3 || pageStructStaticMemberCell.RepeatWith)
							{
								if (headerFooterRows2 == null)
								{
									headerFooterRows2 = new HeaderFooterRows();
								}
								headerFooterRows2.AddFooterRow(rowIndex, pageStructStaticMemberCell.Span, pageStructStaticMemberCell.SizeValue, false, pageStructStaticMemberCell.RepeatWith, flag3, pageStructStaticMemberCell);
							}
							rowIndex += pageStructStaticMemberCell.Span;
							continue;
						}
						flag = false;
					}
					else if (!pageStructStaticMemberCell.HasInstances)
					{
						flag = false;
					}
					else
					{
						pageStructMemberCell = pageStructStaticMemberCell;
						rowIndex2 = rowIndex;
						num = i;
						if (i + 1 < members.Count)
						{
							flag = false;
						}
					}
				}
				else
				{
					if (pageStructDynamicMemberCell == null)
					{
						pageStructMemberCell = members[i];
						rowIndex2 = rowIndex;
						num = i;
						pageStructDynamicMemberCell = (pageStructMemberCell as PageStructDynamicMemberCell);
						rowIndex += pageStructMemberCell.Span;
						flag2 = this.m_detailRows[rowIndex - 1].PageBreaksAtEnd;
						continue;
					}
					flag = false;
				}
				break;
			}
			if (pageStructMemberCell != null && !pageStructMemberCell.Hidden)
			{
				RoundedDouble roundedDouble = new RoundedDouble(0.0);
				if (!pageStructMemberCell.SpanPages && !pageStructMemberCell.PartialItem)
				{
					roundedDouble.Value = pageStructMemberCell.SizeValue;
					if (headerFooterRows != null)
					{
						roundedDouble.Value += headerFooterRows.Height;
					}
					if (headerFooterRows2 != null)
					{
						roundedDouble.Value += headerFooterRows2.Height;
					}
					if (roundedDouble <= pageContext.ColumnHeight)
					{
						RoundedDouble roundedDouble2 = new RoundedDouble(roundedDouble.Value);
						roundedDouble2.Value += this.SpaceNeededForThisPage(prevHeaders, prevFooters, headers, flag, leafsOnPage);
						if (roundedDouble2 <= pageHeight)
						{
							index = num;
							if (headerFooterRows2 != null)
							{
								index += headerFooterRows2.Count;
							}
							return false;
						}
					}
				}
				PageMemberCell pageMemberCell = null;
				bool flag4 = false;
				if (pageStructDynamicMemberCell != null)
				{
					pageMemberCell = pageStructDynamicMemberCell.MemberInstances[0];
					if ((pageStructDynamicMemberCell.PartialItem || pageStructDynamicMemberCell.MemberInstances.Count > 1) && headerFooterRows2 != null)
					{
						headerFooterRows2.SetRowsKeepWith(false);
					}
					if (pageStructDynamicMemberCell.PartialItem && !pageStructDynamicMemberCell.CreateItem)
					{
						flag4 = true;
					}
				}
				else
				{
					pageMemberCell = pageStructStaticMemberCell.MemberInstance;
				}
				if (pageMemberCell.Children != null)
				{
					bool flag5 = false;
					int num2 = 0;
					HeaderFooterRows headerFooterRows3 = null;
					HeaderFooterRows headerFooterRows4 = null;
					if (headerFooterRows2 != null && prevFootersOnPage && this.ResolveFooters(headerFooterRows2, ref pageHeight, pageHeight, flag, ref prevFootersOnPage, pageContext))
					{
						return true;
					}
					headerFooterRows4 = new HeaderFooterRows(prevFooters);
					headerFooterRows4.AddClone(headerFooterRows2);
					if (headerFooterRows4.Count == 0)
					{
						headerFooterRows4 = null;
					}
					if (headerFooterRows != null)
					{
						pageHeight -= headerFooterRows.Height;
						headerFooterRows.SetRowsOnPage(true);
					}
					headerFooterRows3 = new HeaderFooterRows(prevHeaders);
					headerFooterRows3.AddClone(headers);
					headerFooterRows3.AddClone(headerFooterRows);
					if (headerFooterRows3.Count == 0)
					{
						headerFooterRows3 = null;
					}
					RoundedDouble roundedDouble3 = new RoundedDouble(pageMemberCell.ContentHeight);
					if (roundedDouble3 > pageHeight)
					{
						int num3 = this.MoveToNextPageWithHeaders(pageContext, pageMemberCell, headerFooterRows, headers, prevHeaders);
						index = num - num3;
						return true;
					}
					roundedDouble3.Value = pageMemberCell.SizeValue;
					if (!flag4 && !pageMemberCell.SpanPages)
					{
						if (roundedDouble3 < pageHeight)
						{
							double num4 = pageHeight - pageMemberCell.SizeValue;
							bool flag6 = false;
							if (!prevFootersOnPage && headerFooterRows2 != null)
							{
								flag6 = this.ResolveFooters(headerFooterRows2, ref num4, num4, flag, ref prevFootersOnPage, pageContext);
							}
							if (!flag6)
							{
								num4 -= this.SpaceNeededForThisPage(prevHeaders, prevFooters, headers, flag, leafsOnPage);
								if (num4 >= 0.0)
								{
									index = num;
									return false;
								}
							}
						}
						else if (pageMemberCell.KeepTogether)
						{
							int num5 = this.MoveToNextPageWithHeaders(pageContext, pageMemberCell, headerFooterRows, headers, prevHeaders);
							this.TraceKeepTogetherRowMember(pageContext, (pageStructDynamicMemberCell != null) ? ((PageStructMemberCell)pageStructDynamicMemberCell) : ((PageStructMemberCell)pageStructStaticMemberCell));
							index = num - num5;
							return true;
						}
					}
					if (pageStructDynamicMemberCell != null)
					{
						using (pageStructDynamicMemberCell.MemberInstances.GetAndPin(0, out pageMemberCell))
						{
							flag5 = this.MoveKeepWithHeaders(pageMemberCell.Children, ref num2, rowIndex2, pageHeight, headerFooterRows3, headerFooterRows4, null, leafsOnPage, prevFootersOnPage, pageContext);
						}
					}
					else
					{
						flag5 = this.MoveKeepWithHeaders(pageMemberCell.Children, ref num2, rowIndex2, pageHeight, headerFooterRows3, headerFooterRows4, null, leafsOnPage, prevFootersOnPage, pageContext);
					}
					if (flag5 && num2 == 0)
					{
						int num6 = this.MoveToNextPageWithHeaders(pageContext, pageMemberCell, headerFooterRows, headers, prevHeaders);
						index = num - num6;
						return true;
					}
					index = num;
					return flag5;
				}
				if (pageMemberCell.SpanPages)
				{
					return false;
				}
				if (headerFooterRows != null)
				{
					pageHeight -= headerFooterRows.Height;
					headerFooterRows.SetRowsOnPage(true);
				}
				if (headerFooterRows2 != null && prevFootersOnPage && this.ResolveFooters(headerFooterRows2, ref pageHeight, pageHeight, flag, ref prevFootersOnPage, pageContext))
				{
					return true;
				}
				RoundedDouble x = new RoundedDouble(pageMemberCell.SizeValue);
				if (x > pageHeight)
				{
					int num7 = this.MoveToNextPageWithHeaders(pageContext, pageMemberCell, headerFooterRows, headers, prevHeaders);
					index = num - num7;
					return true;
				}
				double num8 = pageHeight - pageMemberCell.SizeValue;
				bool flag7 = false;
				bool flag8 = false;
				if (!prevFootersOnPage && headerFooterRows2 != null)
				{
					flag7 = this.ResolveFooters(headerFooterRows2, ref num8, num8, flag, ref prevFootersOnPage, pageContext);
				}
				if (!flag7)
				{
					num8 -= this.SpaceNeededForThisPage(prevHeaders, prevFooters, headers, flag, leafsOnPage);
					if (num8 >= 0.0)
					{
						index = num;
						return false;
					}
				}
				else
				{
					flag8 = true;
				}
				num8 = pageContext.ColumnHeight - pageMemberCell.SizeValue;
				prevFootersOnPage = false;
				if (headerFooterRows != null)
				{
					num8 -= headerFooterRows.Height;
				}
				if (flag)
				{
					flag7 = this.ResolveFooters(headerFooterRows2, ref num8, num8, flag, ref prevFootersOnPage, pageContext);
				}
				if (!flag7)
				{
					if (flag8 && !flag && !this.ResolveFooters(headerFooterRows2, ref num8, num8, flag, ref prevFootersOnPage, pageContext))
					{
						return true;
					}
					if (headers != null)
					{
						num8 -= headers.Height;
					}
					num8 -= this.SpaceNeededForFullPage(prevHeaders, prevFooters, flag);
					if (num8 <= pageContext.ColumnHeight)
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		private int MoveToNextPageWithHeaders(PageContext pageContext, PageMemberCell memberInstance, HeaderFooterRows currHeaders, HeaderFooterRows headers, HeaderFooterRows prevHeaders)
		{
			double num = pageContext.ColumnHeight - memberInstance.SizeValue;
			int result = 0;
			if (currHeaders != null)
			{
				result = currHeaders.MoveToNextPage(ref num);
			}
			if (num > 0.0 && headers == null && prevHeaders != null)
			{
				prevHeaders.MoveToNextPage(ref num);
			}
			return result;
		}

		private double SpaceNeededForThisPage(HeaderFooterRows prevHeaders, HeaderFooterRows prevFooters, HeaderFooterRows headers, bool lastMember, bool leafsOnPage)
		{
			double num = 0.0;
			if (leafsOnPage)
			{
				if (prevFooters != null && lastMember)
				{
					num += prevFooters.KeepWithNoRepeatNoPageHeight;
				}
			}
			else
			{
				if (headers != null)
				{
					num += headers.RepeatNotOnPageHeight;
				}
				if (prevHeaders != null)
				{
					num += prevHeaders.RepeatNotOnPageHeight;
				}
				if (prevFooters != null)
				{
					num = ((!lastMember) ? (num + prevFooters.RepeatHeight) : (num + prevFooters.KeepWithAndRepeatHeight));
				}
			}
			return num;
		}

		private double SpaceNeededForFullPage(HeaderFooterRows prevHeaders, HeaderFooterRows prevFooters, bool lastMember)
		{
			double num = 0.0;
			if (prevHeaders != null)
			{
				num += prevHeaders.RepeatHeight;
			}
			if (prevFooters != null)
			{
				num = ((!lastMember) ? (num + prevFooters.RepeatHeight) : (num + prevFooters.KeepWithAndRepeatHeight));
			}
			return num;
		}

		private void UpdateColumns(double startInTablix, double endInTablix)
		{
			if (this.m_columnHeaders != null && this.m_columnInfo != null && this.m_columnInfo.Count != 0)
			{
				PageStructMemberCell pageStructMemberCell = null;
				int num = 0;
				for (int i = 0; i < this.m_columnHeaders.Count; i++)
				{
					pageStructMemberCell = this.m_columnHeaders[i];
					pageStructMemberCell.UpdateColumns(this.m_columnInfo, num, startInTablix, endInTablix);
					num += pageStructMemberCell.Span;
				}
			}
		}

		private void AlignCornerCellsToPageHorizontally(int targetColIndex, double startInTablix, double endInTablix, PageContext pageContext)
		{
			if (this.m_cornerCells != null && this.m_colHeaderHeights != null && this.AddToPageColumnHeaders)
			{
				PageCornerCell pageCornerCell = null;
				bool flag = false;
				for (int i = 0; i < this.m_colHeaderHeights.Count; i++)
				{
					for (int j = 0; j < this.m_headerRowCols; j++)
					{
						pageCornerCell = this.m_cornerCells[i, j];
						if (pageCornerCell != null && j <= targetColIndex && j + pageCornerCell.ColSpan > targetColIndex)
						{
							flag = false;
							int num = 0;
							while (!flag && num < pageCornerCell.RowSpan)
							{
								if (this.m_colHeaderHeights[i + num].State == SizeInfo.PageState.Normal)
								{
									pageCornerCell.AlignToPageHorizontal(j, targetColIndex, startInTablix, endInTablix, this.m_rowHeaderWidths, this.IsLTR, pageContext);
									flag = true;
								}
								num++;
							}
						}
					}
				}
			}
		}

		private void CornerRowsHorizontalSize(PageContext pageContext)
		{
			if (this.m_cornerCells != null && this.m_colHeaderHeights != null && this.AddToPageColumnHeaders)
			{
				int num = 0;
				PageCornerCell pageCornerCell = null;
				for (int i = 0; i < this.m_colHeaderHeights.Count; i++)
				{
					if (this.m_colHeaderHeights[i].State == SizeInfo.PageState.Normal)
					{
						num = 0;
						while (num < this.m_headerRowCols)
						{
							pageCornerCell = this.m_cornerCells[i, num];
							if (pageCornerCell != null)
							{
								pageCornerCell.CalculateHorizontal(num, ref this.m_rowHeaderWidths, pageContext);
								num += pageCornerCell.ColSpan;
							}
							else
							{
								num++;
							}
						}
					}
				}
			}
		}

		private bool TryRepeatRowHeaders(double startInTablix, double endInTablix, ref int colIndex)
		{
			if (this.m_columnInfo != null && this.m_columnInfo.Count != 0)
			{
				ColumnInfo columnInfo = null;
				RoundedDouble roundedDouble = new RoundedDouble(0.0);
				for (int i = 0; i < this.m_columnInfo.Count; i++)
				{
					columnInfo = this.m_columnInfo[i];
					if (columnInfo.PageHorizontalState != ColumnInfo.HorizontalState.AtLeft)
					{
						roundedDouble.Value = columnInfo.Right;
						if (roundedDouble > startInTablix)
						{
							roundedDouble.Value = columnInfo.Left;
							if (roundedDouble < startInTablix)
							{
								return false;
							}
							colIndex = i;
							return true;
						}
					}
				}
				return false;
			}
			return false;
		}

		private void CalculateContentHorizontally(double startInTablix, double endInTablix, PageContext pageContext)
		{
			if (startInTablix == 0.0 && this.m_columnInfo == null)
			{
				double num = 0.0;
				double startPos = startInTablix;
				this.CornerRowsHorizontalSize(pageContext);
				if (this.m_rowHeaders != null)
				{
					int num2 = 0;
					for (int i = 0; i < this.m_rowHeaders.Count; i++)
					{
						this.m_rowHeaders[i].CalculateRHHorizontal(this.m_detailRows, num2, 0, ref this.m_rowHeaderWidths, pageContext);
						num2 += this.m_rowHeaders[i].Span;
					}
				}
				Tablix.ResolveSizes(this.m_rowHeaderWidths);
				if (this.m_headerRowCols > 0 && this.m_rowHeaderWidths != null)
				{
					for (int j = 0; j < this.m_rowHeaderWidths.Count; j++)
					{
						num += this.m_rowHeaderWidths[j].SizeValue;
					}
				}
				this.m_columnInfo = new ScalableList<ColumnInfo>(0, pageContext.ScalabilityCache);
				if (this.m_columnHeaders != null && this.AddToPageColumnHeaders)
				{
					PageContext pageContext2 = pageContext;
					if (this.RepeatedColumnHeaders && !pageContext.ResetHorizontal)
					{
						pageContext2 = new PageContext(pageContext);
						pageContext2.ResetHorizontal = true;
					}
					int num3 = 0;
					for (int k = 0; k < this.m_columnHeaders.Count; k++)
					{
						this.m_columnHeaders[k].CalculateCHHorizontal(this.m_colHeaderHeights, 0, num3, this.m_columnInfo, pageContext2, this.AddToPageColumnHeaders);
						num3 += this.m_columnHeaders[k].Span;
					}
				}
				if (this.m_detailRows != null)
				{
					for (int l = 0; l < this.m_detailRows.Count; l++)
					{
						this.m_detailRows[l].CalculateHorizontal(this.m_columnInfo, pageContext);
					}
				}
				Tablix.ResolveSizes(this.m_columnInfo);
				if (this.m_columnInfo != null && this.m_columnInfo.Count > 0)
				{
					bool flag = true;
					if (this.m_columnHeaders != null)
					{
						int num4 = 0;
						int num5 = -1;
						int num6 = -1;
						double num7 = 0.0;
						for (int m = 0; m < this.m_columnHeaders.Count; m++)
						{
							if (num4 == this.m_colsBeforeRowHeaders && (this.IsLTR || num4 > 0))
							{
								if (num4 > 0)
								{
									startInTablix += this.m_columnInfo[num4 - 1].Right;
									startPos = startInTablix;
								}
								startInTablix += num;
								num5 = -1;
								num6 = -1;
								flag = false;
							}
							this.m_columnHeaders[m].NormalizeDetailColWidth(this.m_columnInfo, num4, startInTablix, endInTablix, ref num5, ref num6, false, ref num7);
							num4 += this.m_columnHeaders[m].Span;
						}
						if (num4 == this.m_colsBeforeRowHeaders && num4 > 0)
						{
							startInTablix += this.m_columnInfo[num4 - 1].Right;
							startPos = startInTablix;
						}
					}
					if (flag)
					{
						base.ItemPageSizes.AdjustWidthTo(this.m_columnInfo[this.m_columnInfo.Count - 1].Right + num);
					}
					else
					{
						base.ItemPageSizes.AdjustWidthTo(this.m_columnInfo[this.m_columnInfo.Count - 1].Right);
					}
				}
				else
				{
					base.ItemPageSizes.AdjustWidthTo(num);
				}
				if (this.IsLTR)
				{
					Tablix.ResolveStartPosAndState(this.m_rowHeaderWidths, startPos, SizeInfo.PageState.Normal);
				}
				else
				{
					Tablix.ResolveStartPosAndStateRTL(this.m_rowHeaderWidths, startPos, SizeInfo.PageState.Normal);
				}
			}
			else
			{
				this.NormalizeColHeadersWidths(startInTablix, endInTablix, false);
				if (this.m_colsBeforeRowHeaders > 0)
				{
					bool flag2 = false;
					if (this.m_columnInfo != null && this.m_columnInfo.Count > 0)
					{
						if (this.m_columnInfo[this.m_colsBeforeRowHeaders - 1].PageHorizontalState == ColumnInfo.HorizontalState.AtLeft && this.m_colsBeforeRowHeaders == this.m_columnInfo.Count)
						{
							flag2 = true;
						}
					}
					else
					{
						flag2 = true;
					}
					if (flag2)
					{
						double amount = 0.0;
						if (this.m_rowHeaderWidths != null)
						{
							amount = ((!this.SplitRowHeaders) ? ((!this.IsLTR) ? Tablix.ResolveStartPosAndStateRTL(this.m_rowHeaderWidths, startInTablix, SizeInfo.PageState.Normal) : Tablix.ResolveStartPosAndState(this.m_rowHeaderWidths, startInTablix, SizeInfo.PageState.Normal)) : ((!this.IsLTR) ? this.m_rowHeaderWidths[0].EndPos : this.m_rowHeaderWidths[this.m_rowHeaderWidths.Count - 1].EndPos));
						}
						base.ItemPageSizes.AdjustWidthTo(amount);
					}
				}
			}
		}

		private int FindRHColumnSpanHorizontal(double startInTablix, double endInTablix, out int firstColIndex, ref double rowHeadersWidth)
		{
			int num = 0;
			RoundedDouble roundedDouble = new RoundedDouble(0.0);
			if (this.IsLTR)
			{
				firstColIndex = 0;
				for (int i = 0; i < this.m_rowHeaderWidths.Count; i++)
				{
					roundedDouble.Value = this.m_rowHeaderWidths[i].EndPos;
					if (roundedDouble <= startInTablix)
					{
						this.m_rowHeaderWidths[i].State = SizeInfo.PageState.Skip;
						num++;
						firstColIndex++;
					}
					else
					{
						rowHeadersWidth += this.m_rowHeaderWidths[i].SizeValue;
						rowHeadersWidth -= Math.Max(0.0, startInTablix - this.m_rowHeaderWidths[i].StartPos);
						roundedDouble.Value = startInTablix + rowHeadersWidth;
						if (roundedDouble < endInTablix)
						{
							this.m_rowHeaderWidths[i].State = SizeInfo.PageState.Normal;
							num++;
						}
					}
				}
				if (num == this.m_rowHeaderWidths.Count)
				{
					num--;
				}
			}
			else
			{
				num = (firstColIndex = this.m_rowHeaderWidths.Count - 1);
				for (int num2 = num; num2 >= 0; num2--)
				{
					roundedDouble.Value = this.m_rowHeaderWidths[num2].EndPos;
					if (roundedDouble <= startInTablix)
					{
						this.m_rowHeaderWidths[num2].State = SizeInfo.PageState.Skip;
						num--;
						firstColIndex--;
					}
					else
					{
						rowHeadersWidth += this.m_rowHeaderWidths[num2].SizeValue;
						rowHeadersWidth -= Math.Max(0.0, startInTablix - this.m_rowHeaderWidths[num2].StartPos);
						roundedDouble.Value = startInTablix + rowHeadersWidth;
						if (roundedDouble < endInTablix)
						{
							this.m_rowHeaderWidths[num2].State = SizeInfo.PageState.Normal;
							num--;
						}
					}
				}
				if (num < 0)
				{
					num = 0;
				}
			}
			return num;
		}

		private void MarkColumnHorizontalState(int colIndex, ColumnInfo.HorizontalState state)
		{
			ColumnInfo columnInfo = null;
			using (this.m_columnInfo.GetAndPin(colIndex, out columnInfo))
			{
				columnInfo.PageHorizontalState = state;
			}
		}

		private void MarkSplitRowHeaders(double startInTablix, double endInTablix, PageContext pageContext)
		{
			if (this.m_rowHeaderWidths != null && this.m_rowHeaderWidths.Count != 0)
			{
				double num = 0.0;
				int firstColIndex = 0;
				int num2 = this.FindRHColumnSpanHorizontal(startInTablix, endInTablix, out firstColIndex, ref num);
				RoundedDouble x = new RoundedDouble(startInTablix + num);
				if (x <= endInTablix)
				{
					if (this.m_columnInfo != null && this.m_columnInfo.Count > 0)
					{
						this.AlignSplitRowHeaders(firstColIndex, num2, startInTablix, endInTablix, pageContext, false);
						if (this.m_colsBeforeRowHeaders > 0)
						{
							if (this.m_colsBeforeRowHeaders < this.m_columnInfo.Count)
							{
								this.MarkColumnHorizontalState(this.m_colsBeforeRowHeaders, ColumnInfo.HorizontalState.LeftOfNextPage);
								this.NormalizeColHeadersWidths(startInTablix, endInTablix, false);
							}
							else
							{
								base.ItemPageSizes.AdjustWidthTo(this.m_rowHeaderWidths[num2].EndPos);
							}
						}
						else
						{
							this.MarkColumnHorizontalState(0, ColumnInfo.HorizontalState.LeftOfNextPage);
							this.NormalizeColHeadersWidths(startInTablix, endInTablix, false);
						}
					}
					else
					{
						base.ItemPageSizes.AdjustWidthTo(this.m_rowHeaderWidths[num2].EndPos);
					}
					this.SplitRowHeaders = false;
				}
				else
				{
					this.AlignSplitRowHeaders(firstColIndex, num2, startInTablix, endInTablix, pageContext, true);
				}
			}
		}

		private void AlignSplitRowHeaders(int firstColIndex, int lastColIndex, double startInTablix, double endInTablix, PageContext pageContext, bool normalize)
		{
			this.m_rowHeaderWidths[lastColIndex].State = SizeInfo.PageState.Normal;
			if (this.m_rowHeaders != null)
			{
				for (int i = 0; i < this.m_rowHeaders.Count; i++)
				{
					this.m_rowHeaders[i].AlignRHToPageHorizontal(this.m_detailRows, 0, 0, lastColIndex, startInTablix, endInTablix, this.m_rowHeaderWidths, this.IsLTR, pageContext);
				}
			}
			this.AlignCornerCellsToPageHorizontally(lastColIndex, startInTablix, endInTablix, pageContext);
			Tablix.ResolveSizes(this.m_rowHeaderWidths);
			int num = this.m_rowHeaderWidths.Count - 1;
			double num2 = 0.0;
			if (this.IsLTR)
			{
				Tablix.ResolveStartPos(this.m_rowHeaderWidths, this.m_rowHeaderWidths[0].StartPos);
				for (int j = 0; j < firstColIndex; j++)
				{
					this.m_rowHeaderWidths[j].State = SizeInfo.PageState.Skip;
				}
				for (int k = firstColIndex; k <= lastColIndex; k++)
				{
					this.m_rowHeaderWidths[k].State = SizeInfo.PageState.Normal;
				}
				for (int l = lastColIndex + 1; l < this.m_rowHeaderWidths.Count; l++)
				{
					this.m_rowHeaderWidths[l].State = SizeInfo.PageState.Skip;
				}
				num2 = this.m_rowHeaderWidths[num].EndPos;
			}
			else
			{
				Tablix.ResolveStartPosRTL(this.m_rowHeaderWidths, this.m_rowHeaderWidths[num].StartPos);
				for (int m = 0; m < lastColIndex; m++)
				{
					this.m_rowHeaderWidths[m].State = SizeInfo.PageState.Skip;
				}
				for (int n = lastColIndex; n <= firstColIndex; n++)
				{
					this.m_rowHeaderWidths[n].State = SizeInfo.PageState.Normal;
				}
				for (int num3 = firstColIndex + 1; num3 <= num; num3++)
				{
					this.m_rowHeaderWidths[num3].State = SizeInfo.PageState.Skip;
				}
				num2 = this.m_rowHeaderWidths[0].EndPos;
			}
			if (normalize)
			{
				if (this.m_columnInfo != null && this.m_columnInfo.Count > 0)
				{
					if (this.m_colsBeforeRowHeaders > 0)
					{
						if (this.m_colsBeforeRowHeaders < this.m_columnInfo.Count)
						{
							this.MarkColumnHorizontalState(this.m_colsBeforeRowHeaders, ColumnInfo.HorizontalState.Normal);
							this.m_columnInfo[this.m_colsBeforeRowHeaders].Left = num2;
							this.NormalizeColHeadersWidths(startInTablix, endInTablix, true);
						}
						else
						{
							base.ItemPageSizes.AdjustWidthTo(num2);
						}
					}
					else
					{
						this.MarkColumnHorizontalState(0, ColumnInfo.HorizontalState.Normal);
						this.m_columnInfo[this.m_colsBeforeRowHeaders].Left = num2;
						this.NormalizeColHeadersWidths(startInTablix, endInTablix, true);
					}
				}
				else
				{
					base.ItemPageSizes.AdjustWidthTo(num2);
				}
			}
		}

		private void MarkTablixRTLColumnsForHorizontalPage(double startInTablix, double endInTablix, PageContext pageContext)
		{
			double num = 0.0;
			bool flag = false;
			bool flag2 = true;
			RoundedDouble roundedDouble = new RoundedDouble(0.0);
			if (this.m_headerRowCols > 0 && this.m_rowHeaderWidths != null)
			{
				for (int i = 0; i < this.m_rowHeaderWidths.Count; i++)
				{
					num += this.m_rowHeaderWidths[i].SizeValue;
				}
				roundedDouble.Value = startInTablix + num;
				if (roundedDouble < endInTablix)
				{
					flag = true;
					if (startInTablix == 0.0)
					{
						roundedDouble.Value = endInTablix;
						if (roundedDouble == pageContext.ColumnWidth || this.PinnedToParentCell)
						{
							flag2 = false;
						}
					}
					else
					{
						flag2 = false;
					}
				}
				else if (startInTablix == 0.0)
				{
					if (this.PinnedToParentCell)
					{
						flag2 = false;
					}
					else
					{
						roundedDouble.Value = num;
						if (roundedDouble < pageContext.ColumnWidth)
						{
							base.ItemPageSizes.MoveHorizontal(endInTablix);
							return;
						}
					}
				}
				else
				{
					flag2 = false;
				}
			}
			else if (this.PinnedToParentCell)
			{
				flag2 = false;
			}
			int colsOnPage = 0;
			bool allowSpanAtRight = true;
			double num2 = endInTablix;
			if (flag)
			{
				colsOnPage = 1;
				allowSpanAtRight = false;
				num2 -= num;
			}
			if (this.MarkDetailColsForHorizontalPage(this.m_columnHeaders, startInTablix, num2, endInTablix, 0, colsOnPage, 0, this.m_colsBeforeRowHeaders, allowSpanAtRight, pageContext) == 0)
			{
				if (flag2)
				{
					if (this.m_columnInfo != null && this.m_columnInfo.Count > 0)
					{
						this.MarkColumnHorizontalState(0, ColumnInfo.HorizontalState.Normal);
					}
					base.ItemPageSizes.MoveHorizontal(endInTablix);
					return;
				}
				if (flag)
				{
					flag = false;
					if (this.m_columnInfo != null && this.m_columnInfo.Count > 0)
					{
						int num3 = 0;
						while (num3 < this.m_columnInfo.Count)
						{
							if (this.m_columnInfo[num3].PageHorizontalState == ColumnInfo.HorizontalState.AtLeft)
							{
								num3++;
								continue;
							}
							this.MarkColumnHorizontalState(num3, ColumnInfo.HorizontalState.Normal);
							break;
						}
					}
					colsOnPage = this.MarkDetailColsForHorizontalPage(this.m_columnHeaders, startInTablix, endInTablix, endInTablix, 0, 0, 0, this.m_colsBeforeRowHeaders, true, pageContext);
				}
			}
			this.NormalizeColHeadersWidths(startInTablix, endInTablix, true);
			if (flag)
			{
				this.AddToPageRowHeaders = true;
				double num4 = startInTablix;
				int num5 = this.m_colsBeforeRowHeaders - 1;
				ColumnInfo columnInfo = null;
				while (num5 >= 0)
				{
					columnInfo = this.m_columnInfo[num5];
					if (columnInfo.PageHorizontalState == ColumnInfo.HorizontalState.Normal)
					{
						num4 = columnInfo.Right;
						break;
					}
					num5--;
				}
				if (this.IsLTR)
				{
					Tablix.ResolveStartPosAndState(this.m_rowHeaderWidths, num4, SizeInfo.PageState.Normal);
				}
				else
				{
					Tablix.ResolveStartPosAndStateRTL(this.m_rowHeaderWidths, num4, SizeInfo.PageState.Normal);
				}
				columnInfo = this.m_columnInfo[this.m_colsBeforeRowHeaders - 1];
				if (columnInfo.PageHorizontalState == ColumnInfo.HorizontalState.Normal)
				{
					if (this.m_colsBeforeRowHeaders < this.m_columnInfo.Count)
					{
						this.MarkColumnHorizontalState(this.m_colsBeforeRowHeaders, ColumnInfo.HorizontalState.Unknown);
						this.NormalizeColHeadersWidths(num4 + num, endInTablix, false, this.m_colsBeforeRowHeaders);
						this.MarkTablixLTRColumnsForHorizontalPage(num4, endInTablix, true, pageContext);
					}
					else
					{
						base.ItemPageSizes.AdjustWidthTo(columnInfo.Right + num);
					}
				}
			}
			else
			{
				this.AddToPageRowHeaders = false;
				if (this.IsLTR)
				{
					Tablix.ResolveStartPosAndState(this.m_rowHeaderWidths, endInTablix, SizeInfo.PageState.Skip);
				}
				else
				{
					Tablix.ResolveStartPosAndStateRTL(this.m_rowHeaderWidths, endInTablix, SizeInfo.PageState.Skip);
				}
				if (this.m_columnInfo[this.m_colsBeforeRowHeaders - 1].PageHorizontalState == ColumnInfo.HorizontalState.Normal)
				{
					if (this.m_colsBeforeRowHeaders < this.m_columnInfo.Count)
					{
						this.MarkColumnHorizontalState(this.m_colsBeforeRowHeaders, ColumnInfo.HorizontalState.LeftOfNextPage);
						this.NormalizeColHeadersWidths(startInTablix, endInTablix + num, false);
					}
					else if (num > 0.0)
					{
						base.ItemPageSizes.AdjustWidthTo(endInTablix + num);
					}
				}
			}
		}

		private void MarkTablixLTRColumnsForHorizontalPage(double startInTablix, double endInTablix, bool rowHeadersOnPageInFlow, PageContext pageContext)
		{
			double num = 0.0;
			bool flag = false;
			RoundedDouble roundedDouble = new RoundedDouble(0.0);
			int colsOnPage = 0;
			if (rowHeadersOnPageInFlow)
			{
				colsOnPage = 1;
			}
			if (startInTablix == 0.0 || rowHeadersOnPageInFlow)
			{
				if (this.m_headerRowCols > 0 && this.m_rowHeaderWidths != null)
				{
					int firstColIndex = 0;
					int lastColIndex = this.FindRHColumnSpanHorizontal(startInTablix, endInTablix, out firstColIndex, ref num);
					if (num > 0.0)
					{
						colsOnPage = 1;
					}
					roundedDouble.Value = startInTablix + num;
					if (roundedDouble <= endInTablix)
					{
						roundedDouble.Value = endInTablix;
						if (rowHeadersOnPageInFlow || roundedDouble == pageContext.ColumnWidth || this.PinnedToParentCell)
						{
							startInTablix += num;
							num = 0.0;
						}
						else
						{
							flag = true;
						}
						goto IL_0203;
					}
					roundedDouble.Value = num;
					if (!rowHeadersOnPageInFlow && roundedDouble <= pageContext.ColumnWidth && !this.PinnedToParentCell)
					{
						base.ItemPageSizes.MoveHorizontal(endInTablix);
					}
					else
					{
						this.SplitRowHeaders = true;
						this.RepeatRowHeaders = false;
						this.AlignSplitRowHeaders(firstColIndex, lastColIndex, startInTablix, endInTablix, pageContext, true);
					}
					return;
				}
				if (!rowHeadersOnPageInFlow)
				{
					roundedDouble.Value = endInTablix;
					if (!(roundedDouble == pageContext.ColumnWidth) && !this.PinnedToParentCell)
					{
						colsOnPage = 1;
						flag = true;
					}
				}
			}
			else
			{
				if (this.SplitRowHeaders)
				{
					this.MarkSplitRowHeaders(startInTablix, endInTablix, pageContext);
					return;
				}
				if (!rowHeadersOnPageInFlow)
				{
					if (this.RepeatRowHeaders && this.m_rowHeaderWidths != null)
					{
						int num2 = 0;
						if (this.TryRepeatRowHeaders(startInTablix, endInTablix, ref num2))
						{
							for (int i = 0; i < this.m_rowHeaderWidths.Count; i++)
							{
								num += this.m_rowHeaderWidths[i].SizeValue;
							}
							int num3 = num2;
							while (num3 < this.m_columnInfo.Count)
							{
								if (this.m_columnInfo[num3].PageHorizontalState == ColumnInfo.HorizontalState.AtLeft)
								{
									num3++;
									continue;
								}
								this.MarkColumnHorizontalState(num3, ColumnInfo.HorizontalState.Unknown);
								break;
							}
							if (num > 0.0)
							{
								colsOnPage = 1;
							}
						}
						else
						{
							this.AddToPageRowHeaders = false;
						}
					}
					else
					{
						this.AddToPageRowHeaders = false;
					}
				}
			}
			goto IL_0203;
			IL_0203:
			if (this.MarkDetailColsForHorizontalPage(this.m_columnHeaders, startInTablix + num, endInTablix, endInTablix, 0, colsOnPage, 0, 0, true, pageContext) == 0)
			{
				if (flag)
				{
					if (this.m_columnInfo != null && this.m_columnInfo.Count > 0)
					{
						this.MarkColumnHorizontalState(0, ColumnInfo.HorizontalState.Normal);
					}
					base.ItemPageSizes.MoveHorizontal(endInTablix);
					return;
				}
				if (rowHeadersOnPageInFlow)
				{
					this.MarkColumnHorizontalState(this.m_colsBeforeRowHeaders, ColumnInfo.HorizontalState.LeftOfNextPage);
				}
				else if (num > 0.0)
				{
					int num4 = 0;
					while (num4 < this.m_columnInfo.Count)
					{
						if (this.m_columnInfo[num4].PageHorizontalState == ColumnInfo.HorizontalState.AtLeft)
						{
							num4++;
							continue;
						}
						this.MarkColumnHorizontalState(num4, ColumnInfo.HorizontalState.Unknown);
						break;
					}
					colsOnPage = this.MarkDetailColsForHorizontalPage(this.m_columnHeaders, startInTablix, endInTablix, endInTablix, 0, 0, 0, 0, true, pageContext);
					this.AddToPageRowHeaders = false;
					num = 0.0;
				}
			}
			if (this.AddToPageRowHeaders && num > 0.0 && !flag)
			{
				if (this.IsLTR)
				{
					Tablix.ResolveStartPosAndState(this.m_rowHeaderWidths, startInTablix, SizeInfo.PageState.Normal);
				}
				else
				{
					Tablix.ResolveStartPosAndStateRTL(this.m_rowHeaderWidths, startInTablix, SizeInfo.PageState.Normal);
				}
			}
			this.NormalizeColHeadersWidths(startInTablix + num, endInTablix, true);
		}

		private void MarkTablixColumnsForHorizontalPage(double startInTablix, double endInTablix, PageContext pageContext)
		{
			if (this.m_colsBeforeRowHeaders > 0)
			{
				if (this.m_columnInfo[this.m_colsBeforeRowHeaders - 1].PageHorizontalState != ColumnInfo.HorizontalState.AtLeft)
				{
					this.MarkTablixRTLColumnsForHorizontalPage(startInTablix, endInTablix, pageContext);
				}
				else if (this.m_colsBeforeRowHeaders < this.m_columnInfo.Count)
				{
					bool rowHeadersOnPageInFlow = false;
					if (startInTablix > 0.0 && !this.SplitRowHeaders && this.m_rowHeaderWidths != null)
					{
						double num = 0.0;
						num = ((!this.IsLTR) ? this.m_rowHeaderWidths[this.m_rowHeaderWidths.Count - 1].StartPos : this.m_rowHeaderWidths[0].StartPos);
						if (num == startInTablix)
						{
							rowHeadersOnPageInFlow = true;
						}
					}
					this.MarkTablixLTRColumnsForHorizontalPage(startInTablix, endInTablix, rowHeadersOnPageInFlow, pageContext);
				}
				else
				{
					this.SplitRowHeaders = true;
					this.MarkSplitRowHeaders(startInTablix, endInTablix, pageContext);
				}
			}
			else
			{
				this.MarkTablixLTRColumnsForHorizontalPage(startInTablix, endInTablix, false, pageContext);
			}
		}

		private int MarkDetailColsForHorizontalPage(List<PageStructMemberCell> members, double startInTablix, double endInTablix, double realEndInTablix, int colIndex, int colsOnPage, int spanContent, int stopIndex, bool allowSpanAtRight, PageContext pageContext)
		{
			if (members == null)
			{
				if (this.m_detailRows != null)
				{
					bool flag = false;
					ColumnInfo columnInfo = null;
					using (this.m_columnInfo.GetAndPin(colIndex, out columnInfo))
					{
						if (columnInfo.Unresolved)
						{
							flag = true;
							columnInfo.Unresolved = false;
						}
					}
					if (flag)
					{
						for (int i = 0; i < this.m_detailRows.Count; i++)
						{
							this.m_detailRows[i].ResolveHorizontal(this.m_columnInfo, colIndex, startInTablix, realEndInTablix, pageContext);
						}
					}
				}
				return 1;
			}
			int num = 0;
			int num2 = 0;
			PageStructMemberCell pageStructMemberCell = null;
			RoundedDouble roundedDouble = new RoundedDouble(0.0);
			RoundedDouble roundedDouble2 = new RoundedDouble(0.0);
			RoundedDouble roundedDouble3 = new RoundedDouble(0.0);
			RoundedDouble roundedDouble4 = new RoundedDouble(0.0);
			RoundedDouble roundedDouble5 = new RoundedDouble(0.0);
			while (num < members.Count)
			{
				this.NormalizeWidths(members, num, colIndex, startInTablix, endInTablix);
				pageStructMemberCell = members[num];
				roundedDouble.Value = pageStructMemberCell.StartPos;
				roundedDouble2.Value = pageStructMemberCell.EndPos;
				if (stopIndex > 0 && colIndex >= stopIndex)
				{
					return num2;
				}
				if (roundedDouble >= endInTablix || this.m_columnInfo[colIndex].PageHorizontalState == ColumnInfo.HorizontalState.LeftOfNextPage)
				{
					return num2;
				}
				if (roundedDouble2 <= startInTablix)
				{
					colIndex += pageStructMemberCell.Span;
					num++;
				}
				else
				{
					if (pageStructMemberCell is PageStructStaticMemberCell)
					{
						PageStructStaticMemberCell pageStructStaticMemberCell = (PageStructStaticMemberCell)pageStructMemberCell;
						PageMemberCell memberInstance = pageStructStaticMemberCell.MemberInstance;
						roundedDouble3.Value = memberInstance.StartPos;
						roundedDouble4.Value = memberInstance.EndPos;
						roundedDouble5.Value = memberInstance.ContentRight;
						if (roundedDouble3 < startInTablix)
						{
							if (memberInstance.MemberItem != null)
							{
								spanContent++;
							}
							bool flag2 = this.MarkSpanCHMemberOnPage(memberInstance, startInTablix, endInTablix, realEndInTablix, colIndex, colsOnPage, spanContent, stopIndex, allowSpanAtRight, pageContext);
							num2 += memberInstance.CurrColSpan;
							if (flag2)
							{
								return num2;
							}
							colsOnPage += memberInstance.CurrColSpan;
							colIndex += memberInstance.ColSpan;
							num++;
							continue;
						}
						if (colsOnPage > 0 && spanContent == 0)
						{
							bool flag3 = false;
							ColumnInfo columnInfo2 = this.m_columnInfo[colIndex];
							if (!columnInfo2.BlockedBySpan)
							{
								if (roundedDouble5 > endInTablix)
								{
									flag3 = true;
								}
								else if (roundedDouble4 > endInTablix)
								{
									RoundedDouble x = new RoundedDouble(memberInstance.SizeValue);
									if (x <= pageContext.ColumnWidth)
									{
										if (memberInstance.KeepTogether)
										{
											flag3 = true;
											this.TraceKeepTogetherColumnMember(pageContext, pageStructStaticMemberCell);
										}
										else if (memberInstance.Children == null && columnInfo2.KeepTogether)
										{
											flag3 = true;
											this.TraceKeepTogetherColumn(pageContext, columnInfo2);
										}
									}
								}
							}
							if (flag3)
							{
								this.MarkColumnHorizontalState(colIndex, ColumnInfo.HorizontalState.LeftOfNextPage);
								return num2;
							}
						}
						if (roundedDouble4 <= endInTablix)
						{
							colIndex += memberInstance.ColSpan;
							num2 += memberInstance.CurrColSpan;
							if (!memberInstance.Hidden)
							{
								colsOnPage += memberInstance.CurrColSpan;
							}
							num++;
							continue;
						}
						if (roundedDouble5 <= endInTablix && memberInstance.Children != null)
						{
							int num3 = 0;
							int colIndex2 = colIndex;
							num3 = this.MarkDetailColsForHorizontalPage(memberInstance.Children, startInTablix, endInTablix, realEndInTablix, colIndex2, colsOnPage, spanContent, stopIndex, allowSpanAtRight, pageContext);
							if (num3 == 0)
							{
								return num2;
							}
							colIndex += memberInstance.ColSpan;
							if (num3 < memberInstance.CurrColSpan)
							{
								memberInstance.CurrColSpan = num3;
								return num2 + memberInstance.CurrColSpan;
							}
							num2 += memberInstance.CurrColSpan;
							colsOnPage += memberInstance.CurrColSpan;
							num++;
							continue;
						}
						if (allowSpanAtRight)
						{
							if (memberInstance.MemberItem != null)
							{
								spanContent++;
							}
							this.MarkSpanCHMemberOnPage(memberInstance, startInTablix, endInTablix, realEndInTablix, colIndex, colsOnPage, spanContent, stopIndex, true, pageContext);
							num2 += memberInstance.CurrColSpan;
						}
						return num2;
					}
					PageStructDynamicMemberCell pageStructDynamicMemberCell = (PageStructDynamicMemberCell)pageStructMemberCell;
					PageMemberCell pageMemberCell = null;
					int num4 = pageStructDynamicMemberCell.MemberInstances.Count - 1;
					int j;
					for (j = 0; j <= num4; j++)
					{
						pageMemberCell = pageStructDynamicMemberCell.MemberInstances[j];
						roundedDouble4.Value = pageMemberCell.EndPos;
						if (!(roundedDouble4 <= startInTablix))
						{
							break;
						}
						colIndex += pageMemberCell.ColSpan;
					}
					if (stopIndex > 0 && colIndex >= stopIndex)
					{
						return num2;
					}
					roundedDouble3.Value = pageMemberCell.StartPos;
					if (roundedDouble3 < startInTablix)
					{
						if (pageMemberCell.MemberItem != null)
						{
							spanContent++;
						}
						using (pageStructDynamicMemberCell.MemberInstances.GetAndPin(j, out pageMemberCell))
						{
							bool flag4 = this.MarkSpanCHMemberOnPage(pageMemberCell, startInTablix, endInTablix, realEndInTablix, colIndex, colsOnPage, spanContent, stopIndex, allowSpanAtRight, pageContext);
							num2 += pageMemberCell.CurrColSpan;
							if (flag4)
							{
								return num2;
							}
							colsOnPage += pageMemberCell.CurrColSpan;
							colIndex += pageMemberCell.ColSpan;
						}
						j++;
					}
					for (; j <= num4; j++)
					{
						this.NormalizeWidths(pageStructDynamicMemberCell.MemberInstances, j, colIndex, startInTablix, endInTablix);
						if (stopIndex > 0 && colIndex >= stopIndex)
						{
							return num2;
						}
						pageMemberCell = pageStructDynamicMemberCell.MemberInstances[j];
						roundedDouble3.Value = pageMemberCell.StartPos;
						roundedDouble4.Value = pageMemberCell.EndPos;
						roundedDouble5.Value = pageMemberCell.ContentRight;
						if (colsOnPage > 0 && spanContent == 0)
						{
							bool flag5 = false;
							ColumnInfo columnInfo3 = this.m_columnInfo[colIndex];
							if (!columnInfo3.BlockedBySpan)
							{
								RoundedDouble roundedDouble6 = new RoundedDouble(0.0);
								if (roundedDouble5 > endInTablix)
								{
									flag5 = true;
								}
								if (roundedDouble4 > endInTablix && (pageMemberCell.KeepTogether || (pageMemberCell.Children == null && columnInfo3.KeepTogether)))
								{
									roundedDouble6.Value = pageMemberCell.SizeValue;
									if (pageMemberCell.SizeValue <= pageContext.ColumnWidth)
									{
										flag5 = true;
										this.TraceKeepTogetherColumnMemberColumn(pageContext, columnInfo3, pageStructDynamicMemberCell, pageMemberCell.KeepTogether);
									}
								}
							}
							if (flag5)
							{
								this.MarkColumnHorizontalState(colIndex, ColumnInfo.HorizontalState.LeftOfNextPage);
								return num2;
							}
						}
						if (roundedDouble4 <= endInTablix)
						{
							colIndex += pageMemberCell.ColSpan;
							num2 += pageMemberCell.CurrColSpan;
							if (!pageMemberCell.Hidden)
							{
								colsOnPage += pageMemberCell.CurrColSpan;
							}
							continue;
						}
						if (roundedDouble5 <= endInTablix && pageMemberCell.Children != null)
						{
							int num5 = 0;
							using (pageStructDynamicMemberCell.MemberInstances.GetAndPin(j, out pageMemberCell))
							{
								num5 = this.MarkDetailColsForHorizontalPage(pageMemberCell.Children, startInTablix, endInTablix, realEndInTablix, colIndex, colsOnPage, spanContent, stopIndex, allowSpanAtRight, pageContext);
								if (num5 == 0)
								{
									return num2;
								}
								colIndex += pageMemberCell.ColSpan;
								if (num5 < pageMemberCell.CurrColSpan)
								{
									pageMemberCell.CurrColSpan = num5;
									num2 += pageMemberCell.CurrColSpan;
									return num2;
								}
								num2 += pageMemberCell.CurrColSpan;
								colsOnPage += pageMemberCell.CurrColSpan;
							}
							continue;
						}
						if (!allowSpanAtRight)
						{
							return num2;
						}
						if (pageMemberCell.MemberItem != null)
						{
							spanContent++;
						}
						using (pageStructDynamicMemberCell.MemberInstances.GetAndPin(j, out pageMemberCell))
						{
							this.MarkSpanCHMemberOnPage(pageMemberCell, startInTablix, endInTablix, realEndInTablix, colIndex, colsOnPage, spanContent, stopIndex, true, pageContext);
							return num2 + pageMemberCell.CurrColSpan;
						}
					}
					num++;
				}
			}
			return num2;
		}

		private bool MarkSpanCHMemberOnPage(PageMemberCell memberCell, double startInTablix, double endInTablix, double realEndInTablix, int colIndex, int colsOnPage, int spanContent, int stopIndex, bool allowSpanAtRight, PageContext pageContext)
		{
			int num = this.MarkDetailColsForHorizontalPage(memberCell.Children, startInTablix, endInTablix, realEndInTablix, colIndex, colsOnPage, spanContent, stopIndex, allowSpanAtRight, pageContext);
			memberCell.ResolveCHHorizontal(this.m_columnInfo, colIndex, startInTablix, realEndInTablix, pageContext);
			if (num < memberCell.CurrColSpan)
			{
				memberCell.CurrColSpan = num;
				return true;
			}
			RoundedDouble x = new RoundedDouble(memberCell.EndPos);
			if (x > endInTablix)
			{
				return true;
			}
			return false;
		}

		private void NormalizeColHeadersWidths(double startInTablix, double endInTablix, bool update)
		{
			if (this.m_columnHeaders != null)
			{
				int num = 0;
				int num2 = -1;
				int num3 = -1;
				double num4 = 0.0;
				for (int i = 0; i < this.m_columnHeaders.Count; i++)
				{
					this.m_columnHeaders[i].NormalizeDetailColWidth(this.m_columnInfo, num, startInTablix, endInTablix, ref num2, ref num3, update, ref num4);
					num += this.m_columnHeaders[i].Span;
				}
				if (num3 >= 0)
				{
					base.ItemPageSizes.AdjustWidthTo(this.m_columnInfo[num3].Right);
				}
				else if (num2 >= 0)
				{
					base.ItemPageSizes.AdjustWidthTo(this.m_columnInfo[num2].Right);
				}
			}
		}

		private void NormalizeColHeadersWidths(double startInTablix, double endInTablix, bool update, int startColIndex)
		{
			if (this.m_columnHeaders != null)
			{
				int num = 0;
				int num2 = -1;
				int num3 = -1;
				double num4 = 0.0;
				for (int i = 0; i < this.m_columnHeaders.Count; i++)
				{
					if (num >= startColIndex)
					{
						this.m_columnHeaders[i].NormalizeDetailColWidth(this.m_columnInfo, num, startInTablix, endInTablix, ref num2, ref num3, update, ref num4);
					}
					num += this.m_columnHeaders[i].Span;
				}
				if (num3 >= 0)
				{
					base.ItemPageSizes.AdjustWidthTo(this.m_columnInfo[num3].Right);
				}
				else if (num2 >= 0)
				{
					base.ItemPageSizes.AdjustWidthTo(this.m_columnInfo[num2].Right);
				}
			}
		}

		private void NormalizeWidths(List<PageStructMemberCell> members, int index, int colIndex, double startInTablix, double endInTablix)
		{
			if (members != null)
			{
				ColumnInfo columnInfo = null;
				bool flag = false;
				while (index < members.Count)
				{
					columnInfo = this.m_columnInfo[colIndex + members[index].Span - 1];
					if (columnInfo.PageHorizontalState != ColumnInfo.HorizontalState.AtLeft)
					{
						break;
					}
					colIndex += members[index].Span;
					index++;
					flag = true;
				}
				int num = colIndex - 1;
				if (flag)
				{
					num = -1;
				}
				for (int i = colIndex; i < this.m_columnInfo.Count; i++)
				{
					columnInfo = this.m_columnInfo[i];
					if (columnInfo.PageHorizontalState != ColumnInfo.HorizontalState.AtLeft)
					{
						if (columnInfo.PageHorizontalState == ColumnInfo.HorizontalState.Unknown)
						{
							if (i > colIndex)
							{
								num = -1;
							}
							break;
						}
						return;
					}
				}
				double num2 = 0.0;
				int num3 = -1;
				for (int j = index; j < members.Count; j++)
				{
					members[j].NormalizeDetailColWidth(this.m_columnInfo, colIndex, startInTablix, endInTablix, ref num, ref num3, false, ref num2);
					colIndex += members[j].Span;
				}
				if (colIndex < this.m_columnInfo.Count)
				{
					this.MarkColumnHorizontalState(colIndex, ColumnInfo.HorizontalState.Unknown);
				}
			}
		}

		private void NormalizeWidths(ScalableList<PageMemberCell> members, int index, int colIndex, double startInTablix, double endInTablix)
		{
			if (members != null)
			{
				ColumnInfo columnInfo = null;
				bool flag = false;
				while (index < members.Count)
				{
					columnInfo = this.m_columnInfo[colIndex + members[index].ColSpan - 1];
					if (columnInfo.PageHorizontalState != ColumnInfo.HorizontalState.AtLeft)
					{
						break;
					}
					colIndex += members[index].ColSpan;
					index++;
					flag = true;
				}
				int num = colIndex - 1;
				if (flag)
				{
					num = -1;
				}
				for (int i = colIndex; i < this.m_columnInfo.Count; i++)
				{
					columnInfo = this.m_columnInfo[i];
					if (columnInfo.PageHorizontalState != ColumnInfo.HorizontalState.AtLeft)
					{
						if (columnInfo.PageHorizontalState == ColumnInfo.HorizontalState.Unknown)
						{
							if (i > colIndex)
							{
								num = -1;
							}
							break;
						}
						return;
					}
				}
				double num2 = 0.0;
				int num3 = -1;
				PageMemberCell pageMemberCell = null;
				for (int j = index; j < members.Count; j++)
				{
					using (members.GetAndPin(j, out pageMemberCell))
					{
						pageMemberCell.NormalizeDetailColWidth(this.m_columnInfo, colIndex, startInTablix, endInTablix, ref num, ref num3, false, ref num2);
						colIndex += pageMemberCell.ColSpan;
					}
				}
				if (colIndex < this.m_columnInfo.Count)
				{
					this.MarkColumnHorizontalState(colIndex, ColumnInfo.HorizontalState.Unknown);
				}
			}
		}

		private bool StaticDecendents(TablixMemberCollection children)
		{
			if (children != null && children.Count != 0)
			{
				bool flag = true;
				for (int i = 0; i < children.Count; i++)
				{
					if (!flag)
					{
						break;
					}
					flag = (((ReportElementCollectionBase<TablixMember>)children)[i].IsStatic && this.StaticDecendents(((ReportElementCollectionBase<TablixMember>)children)[i].Children));
				}
				return flag;
			}
			return true;
		}

		private int TablixMembersDepthTree(TablixMemberCollection memberCollection)
		{
			if (memberCollection != null && memberCollection.Count != 0)
			{
				int num = 0;
				for (int i = 0; i < memberCollection.Count; i++)
				{
					num = Math.Max(num, this.TablixMembersDepthTree(((ReportElementCollectionBase<TablixMember>)memberCollection)[i].Children));
				}
				return num + 1;
			}
			return 0;
		}

		private int AddTablixMemberDef(ref Hashtable memberDefIndexes, ref List<RPLTablixMemberDef> memberDefList, TablixMember tablixMember, bool borderHeader, int defTreeLevel, int sourceIndex, PageContext pageContext)
		{
			int? nullable = null;
			if (memberDefIndexes == null)
			{
				memberDefIndexes = new Hashtable();
				memberDefList = new List<RPLTablixMemberDef>();
				this.m_memberAtLevelIndexes = new Hashtable();
			}
			else
			{
				nullable = (int?)memberDefIndexes[tablixMember.DefinitionPath];
			}
			if (!nullable.HasValue)
			{
				nullable = memberDefList.Count;
				memberDefIndexes.Add(tablixMember.DefinitionPath, nullable);
				byte state = 0;
				if (borderHeader)
				{
					state = 4;
				}
				RPLTablixMemberDef item = new RPLTablixMemberDef(tablixMember.DefinitionPath, tablixMember.MemberCellIndex, state, defTreeLevel);
				memberDefList.Add(item);
				this.m_memberAtLevelIndexes.Add(tablixMember.DefinitionPath, sourceIndex);
				if (pageContext.Common.DiagnosticsEnabled && pageContext.IgnorePageBreaks && tablixMember.Group != null && tablixMember.Group.PageBreak.BreakLocation != 0)
				{
					pageContext.Common.TracePageBreakIgnored(tablixMember, pageContext.IgnorePageBreaksReason);
				}
			}
			return nullable.Value;
		}

		internal bool AlwayHiddenMember(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember member, Visibility visibility, bool evalPageHeaderFooter)
		{
			if (visibility == null)
			{
				return false;
			}
			if (visibility.HiddenState == SharedHiddenState.Always && !evalPageHeaderFooter)
			{
				return true;
			}
			return false;
		}

		internal bool EnterColMemberInstance(TablixMember colMember, Visibility visibility, bool evalPageHeaderFooter)
		{
			if (colMember.IsTotal)
			{
				return false;
			}
			if (visibility == null)
			{
				return true;
			}
			if (visibility.HiddenState == SharedHiddenState.Never)
			{
				return true;
			}
			if (visibility.HiddenState == SharedHiddenState.Always)
			{
				if (evalPageHeaderFooter)
				{
					this.m_ignoreCol++;
					return true;
				}
				return false;
			}
			if (visibility.ToggleItem != null)
			{
				if (colMember.Instance.Visibility.CurrentlyHidden)
				{
					if (!evalPageHeaderFooter)
					{
						return false;
					}
					this.m_ignoreCol++;
				}
				return true;
			}
			return !colMember.Instance.Visibility.CurrentlyHidden;
		}

		internal void LeaveColMemberInstance(TablixMember colMember, Visibility visibility, bool evalPageHeaderFooter)
		{
			if (!colMember.IsTotal && visibility != null && visibility.HiddenState != SharedHiddenState.Never)
			{
				if (visibility.HiddenState == SharedHiddenState.Always && evalPageHeaderFooter)
				{
					this.m_ignoreCol--;
				}
				if (visibility.ToggleItem != null)
				{
					bool flag = false;
					if (evalPageHeaderFooter)
					{
						flag = colMember.Instance.Visibility.CurrentlyHidden;
					}
					if (flag)
					{
						this.m_ignoreCol--;
					}
				}
			}
		}

		internal bool EnterRowMember(TablixMember rowMember, Visibility visibility, bool evalPageHeaderFooter)
		{
			if (rowMember.IsTotal)
			{
				this.m_ignoreGroupPageBreaks++;
				return false;
			}
			if (visibility == null)
			{
				return true;
			}
			if (visibility.HiddenState == SharedHiddenState.Never)
			{
				return true;
			}
			if (visibility.HiddenState == SharedHiddenState.Always)
			{
				if (evalPageHeaderFooter)
				{
					this.m_ignoreRow++;
					return true;
				}
				return false;
			}
			if (visibility.ToggleItem != null)
			{
				VisibilityInstance visibility2 = rowMember.Instance.Visibility;
				this.m_ignoreGroupPageBreaks++;
				if (visibility2.CurrentlyHidden)
				{
					if (!evalPageHeaderFooter)
					{
						return false;
					}
					this.m_ignoreRow++;
				}
				return true;
			}
			return !rowMember.Instance.Visibility.CurrentlyHidden;
		}

		internal void LeaveRowMember(TablixMember rowMember, Visibility visibility, bool evalPageHeaderFooter)
		{
			bool currentlyHidden = false;
			if (visibility != null && visibility.ToggleItem != null)
			{
				currentlyHidden = rowMember.Instance.Visibility.CurrentlyHidden;
			}
			this.LeaveRowMember(rowMember, visibility, evalPageHeaderFooter, currentlyHidden);
		}

		internal void LeaveRowMember(TablixMember rowMember, Visibility visibility, bool evalPageHeaderFooter, bool currentlyHidden)
		{
			if (rowMember.IsTotal)
			{
				this.m_ignoreGroupPageBreaks--;
			}
			else if (visibility != null && visibility.HiddenState != SharedHiddenState.Never)
			{
				if (visibility.HiddenState == SharedHiddenState.Always && evalPageHeaderFooter)
				{
					this.m_ignoreRow--;
				}
				if (visibility.ToggleItem != null)
				{
					this.m_ignoreGroupPageBreaks--;
					bool flag = false;
					if (evalPageHeaderFooter)
					{
						flag = currentlyHidden;
					}
					if (flag)
					{
						this.m_ignoreRow--;
					}
				}
			}
		}

		internal bool EnterRowMemberInstance(TablixMember rowMember, Visibility visibility, bool evalPageHeaderFooter)
		{
			if (rowMember.IsTotal)
			{
				return false;
			}
			if (visibility == null)
			{
				return true;
			}
			if (visibility.HiddenState == SharedHiddenState.Never)
			{
				return true;
			}
			if (visibility.HiddenState == SharedHiddenState.Always)
			{
				if (evalPageHeaderFooter)
				{
					this.m_ignoreRow++;
					return true;
				}
				return false;
			}
			if (visibility.ToggleItem != null)
			{
				if (rowMember.Instance.Visibility.CurrentlyHidden)
				{
					if (!evalPageHeaderFooter)
					{
						return false;
					}
					this.m_ignoreRow++;
				}
				return true;
			}
			return !rowMember.Instance.Visibility.CurrentlyHidden;
		}

		internal void LeaveRowMemberInstance(TablixMember rowMember, Visibility visibility, bool evalPageHeaderFooter, bool currentlyHidden)
		{
			if (!rowMember.IsTotal && visibility != null && visibility.HiddenState != SharedHiddenState.Never)
			{
				if (visibility.HiddenState == SharedHiddenState.Always && evalPageHeaderFooter)
				{
					this.m_ignoreRow--;
				}
				if (visibility.ToggleItem != null)
				{
					bool flag = false;
					if (evalPageHeaderFooter)
					{
						flag = currentlyHidden;
					}
					if (flag)
					{
						this.m_ignoreRow--;
					}
				}
			}
		}

		internal PageMemberCell AddColMember(TablixMember colMember, int rowIndex, int rowSpan, int colSpan, LevelInfo childInfo, PageContext pageContext, double updateHeight)
		{
			PageMemberCell pageMemberCell = null;
			if (colMember.TablixHeader == null)
			{
				if (this.m_ignoreCol > 0 || childInfo.HiddenLevel)
				{
					pageMemberCell = new PageMemberCell(null, 0, colSpan, 0.0, null);
					pageMemberCell.Hidden = true;
				}
				else
				{
					pageMemberCell = new PageMemberCell(null, 0, colSpan, childInfo.SourceSize, colMember.Group);
				}
				return pageMemberCell;
			}
			ReportItem reportItem = colMember.TablixHeader.CellContents.ReportItem;
			double num = colMember.TablixHeader.Size.ToMillimeters() + childInfo.SizeForParent - updateHeight;
			if (reportItem != null)
			{
				PageContext.IgnorePageBreakReason ignorePageBreakReason = PageContext.IgnorePageBreakReason.InsideTablixCell;
				if (pageContext.IgnorePageBreaks)
				{
					ignorePageBreakReason = pageContext.IgnorePageBreaksReason;
				}
				PageContext pageContext2 = new PageContext(pageContext, true, true, ignorePageBreakReason, true);
				if (this.m_ignoreCol > 0 || childInfo.HiddenLevel)
				{
					PageItem item = new HiddenPageItem(reportItem, pageContext2, true);
					pageMemberCell = new PageMemberCell(item, rowSpan + childInfo.SpanForParent, colSpan, 0.0, null);
					pageMemberCell.Hidden = true;
				}
				else
				{
					PageItem item = PageItem.Create(reportItem, true, true, pageContext2);
					item.ItemPageSizes.Width = childInfo.SourceSize;
					bool flag = false;
					item.CalculateVertical(pageContext2, 0.0, 1.7976931348623157E+308, null, new List<PageItem>(), ref flag, true);
					pageMemberCell = new PageMemberCell(item, rowSpan + childInfo.SpanForParent, colSpan, childInfo.SourceSize, colMember.Group);
					num = Math.Max(num, item.ItemPageSizes.Height);
					Tablix.UpdateSizes(rowIndex, pageMemberCell.RowSpan, num, ref this.m_colHeaderHeights);
				}
			}
			else if (this.m_ignoreCol > 0 || childInfo.HiddenLevel)
			{
				pageMemberCell = new PageMemberCell(null, rowSpan + childInfo.SpanForParent, colSpan, 0.0, null);
				pageMemberCell.Hidden = true;
			}
			else
			{
				pageMemberCell = new PageMemberCell(null, rowSpan + childInfo.SpanForParent, colSpan, childInfo.SourceSize, colMember.Group);
				Tablix.UpdateSizes(rowIndex, pageMemberCell.RowSpan, num, ref this.m_colHeaderHeights);
			}
			return pageMemberCell;
		}

		internal PageMemberCell AddTotalColMember(TablixMember colMember, int rowIndex, int rowSpan, int colSpan, LevelInfo parentInfo, LevelInfo childInfo, PageContext pageContext)
		{
			PageMemberCell pageMemberCell = null;
			double updateHeight = 0.0;
			if (parentInfo.SpanForParent > 0)
			{
				rowIndex += parentInfo.SpanForParent;
				rowSpan -= parentInfo.SpanForParent;
				updateHeight = parentInfo.SizeForParent;
				if (rowSpan == 0)
				{
					if (childInfo.HiddenLevel)
					{
						pageMemberCell = new PageMemberCell(null, 0, colSpan, 0.0, null);
						pageMemberCell.Hidden = true;
					}
					else
					{
						pageMemberCell = new PageMemberCell(null, 0, colSpan, childInfo.SourceSize, null);
					}
					return pageMemberCell;
				}
			}
			return this.AddColMember(colMember, rowIndex, rowSpan, colSpan, childInfo, pageContext, updateHeight);
		}

		internal PageMemberCell AddRowMember(TablixMember rowMember, int colIndex, int rowSpan, int colSpan, LevelInfo childInfo, PageContext pageContext, double updateWidth)
		{
			PageMemberCell pageMemberCell = null;
			if (rowMember.TablixHeader == null)
			{
				if (this.m_ignoreRow > 0 || childInfo.HiddenLevel)
				{
					pageMemberCell = new PageMemberCell(null, rowSpan, 0, 0.0, null);
					pageMemberCell.Hidden = true;
				}
				else
				{
					pageMemberCell = new PageMemberCell(null, rowSpan, 0, 0.0, rowMember.Group);
					if (rowMember.Group != null && this.CheckPageBreaks(pageContext))
					{
						pageMemberCell.PageName = rowMember.Group.Instance.PageName;
					}
				}
				if (childInfo.IgnoreTotals)
				{
					pageMemberCell.IgnoreTotals = true;
				}
				return pageMemberCell;
			}
			ReportItem reportItem = rowMember.TablixHeader.CellContents.ReportItem;
			double num = rowMember.TablixHeader.Size.ToMillimeters() + childInfo.SizeForParent - updateWidth;
			if (reportItem != null)
			{
				PageContext.IgnorePageBreakReason ignorePageBreakReason = PageContext.IgnorePageBreakReason.InsideTablixCell;
				if (pageContext.IgnorePageBreaks)
				{
					ignorePageBreakReason = pageContext.IgnorePageBreaksReason;
				}
				PageContext pageContext2 = new PageContext(pageContext, true, true, ignorePageBreakReason, true);
				if (this.m_ignoreRow > 0 || childInfo.HiddenLevel)
				{
					PageItem item = new HiddenPageItem(reportItem, pageContext2, true);
					pageMemberCell = new PageMemberCell(item, rowSpan, colSpan + childInfo.SpanForParent, num, null);
					pageMemberCell.Hidden = true;
				}
				else
				{
					PageItem item = PageItem.Create(reportItem, true, true, pageContext2);
					item.ItemPageSizes.Height = childInfo.SourceSize;
					if (item is TextBox)
					{
						item.ItemPageSizes.Width = num;
					}
					bool flag = false;
					item.CalculateVertical(pageContext2, 0.0, 1.7976931348623157E+308, null, new List<PageItem>(), ref flag, true);
					pageMemberCell = new PageMemberCell(item, rowSpan, colSpan + childInfo.SpanForParent, num, rowMember.Group);
					if (rowMember.Group != null && this.CheckPageBreaks(pageContext))
					{
						pageMemberCell.PageName = rowMember.Group.Instance.PageName;
					}
				}
			}
			else if (this.m_ignoreRow > 0 || childInfo.HiddenLevel)
			{
				pageMemberCell = new PageMemberCell(null, rowSpan, colSpan + childInfo.SpanForParent, num, null);
				pageMemberCell.Hidden = true;
			}
			else
			{
				pageMemberCell = new PageMemberCell(null, rowSpan, colSpan + childInfo.SpanForParent, num, rowMember.Group);
				if (rowMember.Group != null && this.CheckPageBreaks(pageContext))
				{
					pageMemberCell.PageName = rowMember.Group.Instance.PageName;
				}
			}
			if (childInfo.IgnoreTotals)
			{
				pageMemberCell.IgnoreTotals = true;
			}
			return pageMemberCell;
		}

		internal PageMemberCell AddTotalRowMember(TablixMember rowMember, int colIndex, int rowSpan, int colSpan, LevelInfo parentInfo, LevelInfo childInfo, PageContext pageContext)
		{
			PageMemberCell pageMemberCell = null;
			double updateWidth = 0.0;
			if (parentInfo.SpanForParent > 0)
			{
				colIndex += parentInfo.SpanForParent;
				colSpan -= parentInfo.SpanForParent;
				updateWidth = parentInfo.SizeForParent;
				if (colSpan == 0)
				{
					if (childInfo.HiddenLevel)
					{
						pageMemberCell = new PageMemberCell(null, rowSpan, 0, 0.0, null);
						pageMemberCell.Hidden = true;
					}
					else
					{
						pageMemberCell = new PageMemberCell(null, rowSpan, 0, 0.0, null);
					}
					return pageMemberCell;
				}
			}
			return this.AddRowMember(rowMember, colIndex, rowSpan, colSpan, childInfo, pageContext, updateWidth);
		}

		internal void CreateCornerCell(PageItem topItem, CellContents cellContents, int rowIndex, int colIndex, double sourceWidth, double sourceHeight)
		{
			if (this.m_cornerCells == null)
			{
				this.m_cornerCells = new PageCornerCell[this.m_headerColumnRows, this.m_headerRowCols];
			}
			PageCornerCell[,] cornerCells = this.m_cornerCells;
			PageCornerCell pageCornerCell = new PageCornerCell(topItem, cellContents.RowSpan, cellContents.ColSpan, sourceWidth, sourceHeight);
			cornerCells[rowIndex, colIndex] = pageCornerCell;
		}

		private bool CreateDetailCell(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember colMemberParent, int colGridIndex, RowInfo rowInfo, PageContext pageContext)
		{
			bool result = false;
			int num = colMemberParent.MemberCellIndex;
			TablixCell tablixCell = ((ReportElementCollectionBase<TablixCell>)((ReportElementCollectionBase<TablixRow>)this.m_bodyRows)[this.m_rowMemberIndexCell])[num];
			double num2 = this.m_bodyColWidths[num];
			if (tablixCell == null)
			{
				while (num > 0)
				{
					num--;
					tablixCell = ((ReportElementCollectionBase<TablixCell>)((ReportElementCollectionBase<TablixRow>)this.m_bodyRows)[this.m_rowMemberIndexCell])[num];
					if (tablixCell != null)
					{
						break;
					}
				}
			}
			if (this.m_colMemberIndexCell >= 0 && num == this.m_colMemberIndexCell)
			{
				PageDetailCell pageDetailCell = null;
				IDisposable disposable = null;
				if (this.m_ignoreCol > 0 || this.m_ignoreRow > 0)
				{
					disposable = rowInfo.UpdateLastDetailCell(0.0, out pageDetailCell);
				}
				else
				{
					disposable = rowInfo.UpdateLastDetailCell(num2, out pageDetailCell);
					if (pageDetailCell.Hidden)
					{
						pageDetailCell.Hidden = false;
						if (pageDetailCell.CellItem != null)
						{
							PageContext pageContext2 = new PageContext(pageContext, true);
							if (this.m_ignoreCellPageBreaks > 0 || this.m_ignoreGroupPageBreaks > 0)
							{
								pageContext2.IgnorePageBreaks = true;
								if (!pageContext.IgnorePageBreaks)
								{
									if (this.m_ignoreCellPageBreaks > 0)
									{
										pageContext2.IgnorePageBreaksReason = PageContext.IgnorePageBreakReason.InsideTablixCell;
									}
									else
									{
										pageContext2.IgnorePageBreaksReason = PageContext.IgnorePageBreakReason.ToggleableItem;
									}
								}
							}
							pageDetailCell.CellItem = PageItem.Create(pageDetailCell.CellItem.Source, true, false, pageContext2);
							rowInfo.CalculateVerticalLastDetailCell(pageContext2, true, true);
							this.UpdateTopItemKT(pageDetailCell.CellItem, rowInfo, pageDetailCell);
						}
					}
				}
				if (disposable != null)
				{
					disposable.Dispose();
					disposable = null;
				}
			}
			else
			{
				PageContext pageContext3 = new PageContext(pageContext, true);
				bool delayCalc = false;
				if (this.m_ignoreCellPageBreaks > 0 || this.m_ignoreGroupPageBreaks > 0)
				{
					pageContext3.IgnorePageBreaks = true;
					if (!pageContext.IgnorePageBreaks)
					{
						if (this.m_ignoreCellPageBreaks > 0)
						{
							pageContext3.IgnorePageBreaksReason = PageContext.IgnorePageBreakReason.InsideTablixCell;
						}
						else
						{
							pageContext3.IgnorePageBreaksReason = PageContext.IgnorePageBreakReason.ToggleableItem;
						}
					}
				}
				rowInfo.CalculateVerticalLastDetailCell(pageContext3, false, delayCalc);
				result = true;
				this.m_colMemberIndexCell = num;
				if (tablixCell.CellContents.ColSpan == 1)
				{
					this.m_colMemberIndexCell = -1;
				}
				else
				{
					delayCalc = true;
				}
				PageDetailCell pageDetailCell2 = null;
				ReportItem reportItem = null;
				if (tablixCell.CellContents != null)
				{
					reportItem = tablixCell.CellContents.ReportItem;
				}
				if (this.m_ignoreCol > 0 || this.m_ignoreRow > 0)
				{
					if (reportItem != null)
					{
						PageItem item = new HiddenPageItem(reportItem, pageContext3, true);
						pageDetailCell2 = new PageDetailCell(item, 0.0);
					}
					else
					{
						pageDetailCell2 = new PageDetailCell(null, 0.0);
					}
					pageDetailCell2.Hidden = true;
					rowInfo.AddDetailCell(pageDetailCell2, pageContext);
				}
				else
				{
					if (reportItem != null)
					{
						PageItem pageItem = PageItem.Create(reportItem, true, false, pageContext3);
						pageDetailCell2 = new PageDetailCell(pageItem, num2);
						this.UpdateTopItemKT(pageItem, rowInfo, pageDetailCell2);
					}
					else
					{
						pageDetailCell2 = new PageDetailCell(null, num2);
					}
					rowInfo.AddDetailCell(pageDetailCell2, pageContext);
					rowInfo.CalculateVerticalLastDetailCell(pageContext3, true, delayCalc);
				}
			}
			return result;
		}

		private void UpdateTopItemKT(PageItem topItem, RowInfo rowInfo, PageDetailCell currCell)
		{
			if (topItem.KeepTogetherHorizontal)
			{
				currCell.KeepTogether = true;
				topItem.KeepTogetherHorizontal = false;
				topItem.UnresolvedKTH = false;
			}
			if (topItem.KeepTogetherVertical)
			{
				rowInfo.KeepTogether = true;
				topItem.KeepTogetherVertical = false;
				topItem.UnresolvedKTV = false;
			}
		}

		private bool UpdateDetailCell(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember colMemberParent, int colGridIndex, RowInfo rowInfo, double startInTablix, double endInTablix, ref int detailCellIndex, PageContext pageContext)
		{
			bool result = false;
			int num = colMemberParent.MemberCellIndex;
			TablixCell tablixCell = ((ReportElementCollectionBase<TablixCell>)((ReportElementCollectionBase<TablixRow>)this.m_bodyRows)[this.m_rowMemberIndexCell])[num];
			if (tablixCell == null)
			{
				while (num > 0)
				{
					num--;
					tablixCell = ((ReportElementCollectionBase<TablixCell>)((ReportElementCollectionBase<TablixRow>)this.m_bodyRows)[this.m_rowMemberIndexCell])[num];
					if (tablixCell != null)
					{
						break;
					}
				}
			}
			if (this.m_colMemberIndexCell < 0 || num != this.m_colMemberIndexCell)
			{
				PageContext pageContext2 = new PageContext(pageContext, true);
				if (this.m_ignoreCellPageBreaks > 0 || this.m_ignoreGroupPageBreaks > 0)
				{
					pageContext2.IgnorePageBreaks = true;
					if (!pageContext.IgnorePageBreaks)
					{
						if (this.m_ignoreCellPageBreaks > 0)
						{
							pageContext2.IgnorePageBreaksReason = PageContext.IgnorePageBreakReason.InsideTablixCell;
						}
						else
						{
							pageContext2.IgnorePageBreaksReason = PageContext.IgnorePageBreakReason.ToggleableItem;
						}
					}
				}
				rowInfo.UpdateVerticalDetailCell(pageContext2, startInTablix, endInTablix, ref detailCellIndex);
				result = true;
				this.m_colMemberIndexCell = num;
				if (tablixCell.CellContents.ColSpan == 1)
				{
					this.m_colMemberIndexCell = -1;
				}
			}
			return result;
		}

		internal void CreateCorner(TablixCorner corner, PageContext pageContext)
		{
			if (this.m_cornerCells == null && this.m_headerColumnRows != 0 && this.m_headerRowCols != 0)
			{
				TablixCornerRowCollection rowCollection = corner.RowCollection;
				TablixCornerRow tablixCornerRow = null;
				for (int i = 0; i < rowCollection.Count; i++)
				{
					tablixCornerRow = ((ReportElementCollectionBase<TablixCornerRow>)rowCollection)[i];
					for (int j = 0; j < tablixCornerRow.Count; j++)
					{
						this.AddCornerCell(((ReportElementCollectionBase<TablixCornerCell>)tablixCornerRow)[j], i, j, pageContext);
					}
				}
			}
		}

		private void AddCornerCell(TablixCornerCell tablixCornerCell, int rowIndex, int colIndex, PageContext pageContext)
		{
			if (tablixCornerCell != null && tablixCornerCell.CellContents != null)
			{
				CellContents cellContents = tablixCornerCell.CellContents;
				ReportItem reportItem = cellContents.ReportItem;
				PageItem pageItem = null;
				if (reportItem != null)
				{
					PageContext.IgnorePageBreakReason ignorePageBreakReason = PageContext.IgnorePageBreakReason.InsideTablixCell;
					if (pageContext.IgnorePageBreaks)
					{
						ignorePageBreakReason = pageContext.IgnorePageBreaksReason;
					}
					PageContext pageContext2 = new PageContext(pageContext, true, true, ignorePageBreakReason, true);
					pageItem = PageItem.Create(reportItem, true, true, pageContext2);
					bool flag = false;
					pageItem.CalculateVertical(pageContext2, 0.0, 1.7976931348623157E+308, null, new List<PageItem>(), ref flag, true);
					Tablix.UpdateSizes(rowIndex, cellContents.RowSpan, pageItem.ItemPageSizes.Height, ref this.m_colHeaderHeights);
					this.CreateCornerCell(pageItem, cellContents, rowIndex, colIndex, pageItem.ItemPageSizes.Width, pageItem.ItemPageSizes.Height);
				}
				else
				{
					this.CreateCornerCell(null, cellContents, rowIndex, colIndex, 0.0, 0.0);
				}
			}
		}

		private double CreateColumnsHeaders(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix, PageContext pageContext)
		{
			if (this.ColumnHeadersCreated)
			{
				return 0.0;
			}
			this.CreateCorner(tablix.Corner, pageContext);
			LevelInfo levelInfo = null;
			this.CreateColumnMemberChildren(tablix, (TablixMember)null, this.m_colMembersDepth, false, 0, this.m_headerRowCols, out levelInfo, pageContext);
			this.ColumnHeadersCreated = true;
			this.m_columnHeaders = levelInfo.MemberCells;
			if (this.m_columnHeaders != null && this.m_columnHeaders.Count > 0)
			{
				PageStructDynamicMemberCell pageStructDynamicMemberCell = this.m_columnHeaders[0].Split(this.m_colsBeforeRowHeaders, pageContext);
				if (pageStructDynamicMemberCell != null)
				{
					this.m_columnHeaders.Insert(0, pageStructDynamicMemberCell);
				}
				if (!this.IsLTR)
				{
					int num = 0;
					this.m_columnHeaders.Reverse();
					for (int i = 0; i < this.m_columnHeaders.Count; i++)
					{
						this.m_columnHeaders[i].Reverse(pageContext);
						num += this.m_columnHeaders[i].Span;
					}
					this.m_colsBeforeRowHeaders = num - this.m_colsBeforeRowHeaders;
				}
			}
			Tablix.ResolveSizes(this.m_colHeaderHeights);
			double num2 = Tablix.ResolveStartPosAndState(this.m_colHeaderHeights, 0.0, SizeInfo.PageState.Normal);
			base.ItemPageSizes.AdjustHeightTo(num2);
			return num2;
		}

		private int CreateColumnMemberChildren(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember colMemberParent, int defTreeLevel, bool parentBorderHeader, int parentRowIndex, int parentColIndex, out LevelInfo parentLevelInfo, PageContext pageContext)
		{
			parentLevelInfo = new LevelInfo(0.0);
			TablixMemberCollection tablixMemberCollection = null;
			int num = 0;
			if (colMemberParent == null)
			{
				if (this.m_colsBeforeRowHeaders == 0)
				{
					num = tablix.GroupsBeforeRowHeaders;
				}
				tablixMemberCollection = tablix.ColumnHierarchy.MemberCollection;
			}
			else
			{
				tablixMemberCollection = colMemberParent.Children;
			}
			if (tablixMemberCollection == null)
			{
				if (this.m_ignoreCol == 0)
				{
					parentLevelInfo.SourceSize += this.m_bodyColWidths[colMemberParent.MemberCellIndex];
				}
				return 1;
			}
			int num2 = parentColIndex;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			bool flag = true;
			LevelInfo levelInfo = null;
			bool flag2 = true;
			int num6 = 0;
			bool flag3 = false;
			int num7 = 0;
			int num8 = -1;
			TablixMember tablixMember = null;
			Visibility visibility = null;
			TablixMemberInstance tablixMemberInstance = null;
			TablixDynamicMemberInstance tablixDynamicMemberInstance = null;
			List<int> list = null;
			int num9 = 0;
			for (int i = 0; i < tablixMemberCollection.Count; i++)
			{
				tablixMember = ((ReportElementCollectionBase<TablixMember>)tablixMemberCollection)[i];
				visibility = tablixMember.Visibility;
				flag3 = parentBorderHeader;
				if ((!this.NoRows || !tablixMember.HideIfNoRows) && !this.AlwayHiddenMember(tablix, tablixMember, visibility, pageContext.EvaluatePageHeaderFooter))
				{
					flag = true;
					tablixMemberInstance = tablixMember.Instance;
					if (tablixMember.IsStatic)
					{
						num = 0;
						flag2 = this.EnterColMemberInstance(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter);
						if (tablixMember.IsTotal)
						{
							if (!flag2 && num3 <= num6 && this.m_ignoreCol == 0 && num9 == 0)
							{
								if (list == null)
								{
									list = new List<int>();
								}
								list.Add(i);
							}
						}
						else if (!flag3)
						{
							flag3 = this.StaticDecendents(tablixMember.Children);
						}
					}
					else
					{
						num6 = num2;
						num3 = num2;
						if (i > 0)
						{
							num = 0;
						}
						tablixDynamicMemberInstance = (TablixDynamicMemberInstance)tablixMemberInstance;
						tablixDynamicMemberInstance.ResetContext();
						flag = tablixDynamicMemberInstance.MoveNext();
						if (flag)
						{
							flag2 = this.EnterColMemberInstance(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter);
						}
					}
					num8 = this.AddTablixMemberDef(ref this.m_colMemberDefIndexes, ref this.m_colMemberDefList, tablixMember, flag3, defTreeLevel, i, pageContext);
					while (flag)
					{
						if (flag2)
						{
							num4 = 0;
							if (tablixMember.TablixHeader != null)
							{
								num4 = tablixMember.TablixHeader.CellContents.RowSpan;
								num7 = defTreeLevel - num4;
							}
							else
							{
								num7 = defTreeLevel - 1;
							}
							num5 = this.CreateColumnMemberChildren(tablix, tablixMember, num7, flag3, parentRowIndex + num4, num2, out levelInfo, pageContext);
							if (num5 > 0)
							{
								PageMemberCell pageMemberCell = this.AddColMember(tablixMember, parentRowIndex, num4, num5, levelInfo, pageContext, 0.0);
								if (!levelInfo.OmittedList)
								{
									pageMemberCell.Children = levelInfo.MemberCells;
								}
								MergeDetailRows mergeDetailRows = parentLevelInfo.AddMemberCell(tablixMember, i, pageMemberCell, num8, TablixRegion.ColumnHeader, this.m_bodyColWidths, levelInfo, pageContext, this);
								if (mergeDetailRows == null)
								{
									num2 += num5;
									if (!pageMemberCell.Hidden)
									{
										num3 += num5;
										num9 += num5;
										list = null;
									}
								}
							}
						}
						this.LeaveColMemberInstance(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter);
						if (tablixMember.IsStatic)
						{
							flag = false;
						}
						else
						{
							flag = tablixDynamicMemberInstance.MoveNext();
							if (num > 0)
							{
								this.m_colsBeforeRowHeaders += num5;
								num--;
							}
							if (flag)
							{
								flag2 = this.EnterColMemberInstance(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter);
							}
						}
						num5 = 0;
					}
					tablixDynamicMemberInstance = null;
				}
			}
			if (num9 == 0)
			{
				num2 += this.CreateColumnMemberTotals(list, tablixMemberCollection, defTreeLevel, parentBorderHeader, parentRowIndex, num2, parentLevelInfo, pageContext);
			}
			return num2 - parentColIndex;
		}

		private int CreateColumnMemberTotals(List<int> totals, TablixMemberCollection columnMembers, int defTreeLevel, bool parentBorderHeader, int parentRowIndex, int parentColIndex, LevelInfo parentLevelInfo, PageContext pageContext)
		{
			if (totals != null && totals.Count != 0)
			{
				double sizeForParent = 0.0;
				int num = 2147483647;
				bool flag = false;
				int num2 = 0;
				TablixMember tablixMember = null;
				if (parentRowIndex > 0)
				{
					for (int i = 0; i < totals.Count; i++)
					{
						num2 = totals[i];
						tablixMember = ((ReportElementCollectionBase<TablixMember>)columnMembers)[num2];
						if (tablixMember.TablixHeader != null && tablixMember.TablixHeader.CellContents.RowSpan < num)
						{
							num = tablixMember.TablixHeader.CellContents.RowSpan;
							sizeForParent = tablixMember.TablixHeader.Size.ToMillimeters();
							flag = true;
						}
					}
					if (flag)
					{
						parentLevelInfo.SpanForParent = num;
						parentLevelInfo.SizeForParent = sizeForParent;
					}
				}
				int num3 = parentColIndex;
				int num4 = 0;
				int num5 = 0;
				LevelInfo levelInfo = null;
				int num6 = 0;
				int num7 = -1;
				for (int j = 0; j < totals.Count; j++)
				{
					num2 = totals[j];
					tablixMember = ((ReportElementCollectionBase<TablixMember>)columnMembers)[num2];
					num7 = this.AddTablixMemberDef(ref this.m_colMemberDefIndexes, ref this.m_colMemberDefList, tablixMember, parentBorderHeader, defTreeLevel, j, pageContext);
					num4 = 0;
					if (tablixMember.TablixHeader != null)
					{
						num4 = tablixMember.TablixHeader.CellContents.RowSpan;
						num6 = defTreeLevel - num4;
					}
					else
					{
						num6 = defTreeLevel - 1;
					}
					num5 = this.CreateColumnMemberChildren((AspNetCore.ReportingServices.OnDemandReportRendering.Tablix)null, tablixMember, num6, parentBorderHeader, parentRowIndex + num4, num3, out levelInfo, pageContext);
					if (num5 > 0)
					{
						PageMemberCell pageMemberCell = this.AddTotalColMember(tablixMember, parentRowIndex, num4, num5, parentLevelInfo, levelInfo, pageContext);
						if (!levelInfo.OmittedList)
						{
							pageMemberCell.Children = levelInfo.MemberCells;
						}
						parentLevelInfo.AddMemberCell(tablixMember, num2, pageMemberCell, num7, TablixRegion.ColumnHeader, this.m_bodyColWidths, levelInfo, pageContext, this);
						num3 += num5;
					}
				}
				return num3 - parentColIndex;
			}
			return 0;
		}

		private int TraverseColumnMembers(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember colMemberParent, int parentColIndex, RowInfo currRowInfo, bool create, double startInTablix, double endInTablix, ref int detailCellIndex, out int visibleSpan, List<int> detailCellsState, PageContext pageContext)
		{
			visibleSpan = 0;
			TablixMemberCollection tablixMemberCollection = null;
			tablixMemberCollection = ((colMemberParent != null) ? colMemberParent.Children : tablix.ColumnHierarchy.MemberCollection);
			if (tablixMemberCollection == null)
			{
				bool flag = false;
				flag = ((!create) ? this.UpdateDetailCell(tablix, colMemberParent, parentColIndex, currRowInfo, startInTablix, endInTablix, ref detailCellIndex, pageContext) : this.CreateDetailCell(tablix, colMemberParent, parentColIndex, currRowInfo, pageContext));
				if (this.m_ignoreCol == 0)
				{
					visibleSpan = 1;
				}
				if (flag && detailCellsState != null)
				{
					detailCellsState.Add(this.m_colMemberIndexCell);
				}
				return 1;
			}
			int num = parentColIndex;
			int num2 = 0;
			int num3 = 0;
			bool flag2 = true;
			bool flag3 = true;
			int num4 = 0;
			TablixMember tablixMember = null;
			Visibility visibility = null;
			TablixMemberInstance tablixMemberInstance = null;
			TablixDynamicMemberInstance tablixDynamicMemberInstance = null;
			List<int> list = null;
			int num5 = 0;
			List<int> list2 = null;
			for (int i = 0; i < tablixMemberCollection.Count; i++)
			{
				tablixMember = ((ReportElementCollectionBase<TablixMember>)tablixMemberCollection)[i];
				visibility = tablixMember.Visibility;
				if ((!this.NoRows || !tablixMember.HideIfNoRows) && !this.AlwayHiddenMember(tablix, tablixMember, visibility, pageContext.EvaluatePageHeaderFooter))
				{
					flag2 = true;
					tablixMemberInstance = tablixMember.Instance;
					if (tablixMember.IsStatic)
					{
						flag3 = this.EnterColMemberInstance(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter);
						if (tablixMember.IsTotal && !flag3 && num2 <= num4 && this.m_ignoreCol == 0 && num5 == 0)
						{
							if (list == null)
							{
								list = new List<int>();
							}
							list.Add(i);
						}
					}
					else
					{
						this.m_colMemberIndexCell = -1;
						num4 = num;
						num2 = num;
						tablixDynamicMemberInstance = (TablixDynamicMemberInstance)tablixMemberInstance;
						tablixDynamicMemberInstance.ResetContext();
						flag2 = tablixDynamicMemberInstance.MoveNext();
						if (flag2)
						{
							flag3 = this.EnterColMemberInstance(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter);
						}
					}
					if (detailCellsState != null && list2 != null)
					{
						for (int j = 0; j < list2.Count; j++)
						{
							detailCellsState.Add(list2[j]);
						}
						list2 = null;
					}
					int num6 = 0;
					int destCellIndex = 0;
					int num7 = 0;
					List<int> list3 = null;
					while (flag2)
					{
						if (flag3)
						{
							if (this.m_ignoreCol > 0)
							{
								list3 = new List<int>(1);
								if (currRowInfo.Cells != null)
								{
									num7 = currRowInfo.Cells.Count;
								}
							}
							num3 = this.TraverseColumnMembers(tablix, tablixMember, num, currRowInfo, create, startInTablix, endInTablix, ref detailCellIndex, out num6, list3, pageContext);
							if (this.m_ignoreCol > 0)
							{
								if (list2 == null)
								{
									num += num3;
									list2 = list3;
									destCellIndex = num7;
								}
								else
								{
									num += currRowInfo.MergeDetailCells(destCellIndex, list2, num7, list3);
								}
								list3 = null;
							}
							else
							{
								num += num3;
								num2 += num6;
								num5 += num6;
								visibleSpan += num6;
								list = null;
								list2 = null;
							}
						}
						this.LeaveColMemberInstance(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter);
						if (tablixMember.IsStatic)
						{
							flag2 = false;
						}
						else
						{
							this.m_colMemberIndexCell = -1;
							flag2 = tablixDynamicMemberInstance.MoveNext();
							if (flag2)
							{
								flag3 = this.EnterColMemberInstance(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter);
							}
						}
					}
					tablixDynamicMemberInstance = null;
				}
			}
			if (detailCellsState != null && list2 != null)
			{
				for (int k = 0; k < list2.Count; k++)
				{
					detailCellsState.Add(list2[k]);
				}
				list2 = null;
			}
			if (num5 == 0)
			{
				num += this.TraverseTotalColumnMembers(tablix, list, tablixMemberCollection, num, currRowInfo, create, startInTablix, endInTablix, ref detailCellIndex, ref visibleSpan, pageContext);
			}
			return num - parentColIndex;
		}

		private int TraverseTotalColumnMembers(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix, List<int> totals, TablixMemberCollection columnMembers, int parentColIndex, RowInfo currRowInfo, bool create, double startInTablix, double endInTablix, ref int detailCellIndex, ref int visibleSpan, PageContext pageContext)
		{
			if (totals != null && totals.Count != 0)
			{
				TablixMember tablixMember = null;
				int num = 0;
				int num2 = 0;
				int num3 = parentColIndex;
				int num4 = 0;
				for (int i = 0; i < totals.Count; i++)
				{
					num = totals[i];
					tablixMember = ((ReportElementCollectionBase<TablixMember>)columnMembers)[num];
					num4 = this.TraverseColumnMembers(tablix, tablixMember, num3, currRowInfo, create, startInTablix, endInTablix, ref detailCellIndex, out num2, (List<int>)null, pageContext);
					num3 += num4;
					visibleSpan += num2;
				}
				return num3 - parentColIndex;
			}
			return 0;
		}

		private int CreateTablixRows(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember rowMemberParent, int defTreeLevel, bool parentBorderHeader, int parentRowIndex, int parentColIndex, ref List<PageStructMemberCell> pageStructMemberCell, bool ignoreTotals, ref bool finishLevel, bool parentHasFooters, CreateItemsContext createItems, double startInTablix, double endInTablix, PageContext pageContext)
		{
			TablixMemberCollection tablixMemberCollection = null;
			tablixMemberCollection = ((rowMemberParent != null) ? rowMemberParent.Children : tablix.RowHierarchy.MemberCollection);
			if (tablixMemberCollection == null)
			{
				if (parentRowIndex >= this.m_detailRows.Count)
				{
					finishLevel = true;
					return 0;
				}
				RowInfo rowInfo = null;
				using (this.m_detailRows.GetAndPin(parentRowIndex, out rowInfo))
				{
					if (rowInfo.Top < startInTablix)
					{
						double num = startInTablix - rowInfo.Top;
						this.UpdateDetailRow(tablix, rowMemberParent, rowInfo, parentColIndex, startInTablix, endInTablix, pageContext);
						createItems.UpdateInfo(rowInfo, 0.0 - num);
						if (rowInfo.ContentFullyCreated)
						{
							finishLevel = true;
						}
					}
					else
					{
						createItems.UpdateInfo(rowInfo, 0.0);
						rowInfo.ResetRowHeight();
					}
				}
				return 0;
			}
			int num2 = -1;
			PageStructMemberCell pageStructMemberCell2 = null;
			TablixMember tablixMember = null;
			int num3 = parentRowIndex;
			int num4 = 0;
			bool flag = false;
			int num5 = -1;
			if (pageStructMemberCell != null)
			{
				for (int i = 0; i < pageStructMemberCell.Count; i++)
				{
					flag = false;
					pageStructMemberCell2 = pageStructMemberCell[i];
					num2 = pageStructMemberCell2.SourceIndex;
					tablixMember = ((ReportElementCollectionBase<TablixMember>)tablixMemberCollection)[num2];
					if (pageStructMemberCell2.PartialItem)
					{
						bool flag2 = parentHasFooters;
						if (!flag2 && !tablixMember.IsStatic && i + 1 == pageStructMemberCell.Count)
						{
							bool flag3 = true;
							num5 = this.CheckKeepWithGroupDown(tablixMemberCollection, num2 + 1, KeepWithGroup.Before, ref flag3);
							if (num5 > num2)
							{
								flag2 = true;
							}
						}
						if (pageStructMemberCell2.CreateItem)
						{
							if (pageStructMemberCell2.Span > 0 && num3 + pageStructMemberCell2.Span - 1 < this.m_detailRows.Count)
							{
								RowInfo rowInfo2 = null;
								using (this.m_detailRows.GetAndPin(num3 + pageStructMemberCell2.Span - 1, out rowInfo2))
								{
									rowInfo2.ResetRowHeight();
								}
							}
							if (!pageStructMemberCell2.HasInstances)
							{
								LevelInfo levelInfo = new LevelInfo(pageStructMemberCell, -1, num2, createItems);
								if (ignoreTotals)
								{
									levelInfo.IgnoreTotals = true;
								}
								int num6 = this.CreateRowMemberChildren(tablix, rowMemberParent, defTreeLevel, parentBorderHeader, num3, parentColIndex, num2, false, flag2, levelInfo, pageContext);
								num4 += num6;
								if (!levelInfo.PartialStruct)
								{
									finishLevel = true;
								}
								if (pageStructMemberCell2.Span == 0)
								{
									pageStructMemberCell2.DisposeInstances();
									pageStructMemberCell.RemoveAt(i);
									i--;
								}
								return num4;
							}
							LevelInfo levelInfo2 = new LevelInfo(pageStructMemberCell, i, num2, createItems);
							int num7 = this.CreateDynamicRowMemberChildren(tablix, rowMemberParent, defTreeLevel, num2, num3 + pageStructMemberCell2.Span, parentColIndex, flag2, levelInfo2, pageContext);
							num4 += num7;
							if (levelInfo2.PartialStruct)
							{
								return num4;
							}
							if (pageStructMemberCell2.Span == 0)
							{
								pageStructMemberCell2.DisposeInstances();
								pageStructMemberCell.RemoveAt(i);
								i--;
								i -= Tablix.RemoveHeadersAbove(pageStructMemberCell, i, this.m_detailRows, ref num3);
							}
							num3 += pageStructMemberCell2.Span;
							continue;
						}
						PageMemberCell pageMemberCell = null;
						IDisposable disposable3 = pageStructMemberCell2.PartialMemberInstance(out pageMemberCell);
						if (tablixMember.IsTotal || (tablixMember.Visibility != null && tablixMember.Visibility.ToggleItem != null))
						{
							this.m_ignoreGroupPageBreaks++;
						}
						bool parentBorderHeader2 = parentBorderHeader;
						if (!parentBorderHeader && tablixMember.IsStatic)
						{
							parentBorderHeader2 = this.StaticDecendents(tablixMember.Children);
						}
						int defTreeLevel2 = (pageMemberCell.ColSpan <= 0) ? (defTreeLevel - 1) : (defTreeLevel - pageMemberCell.ColSpan);
						List<PageStructMemberCell> children = pageMemberCell.Children;
						if (!tablixMember.IsStatic)
						{
							this.RestoreRowMemberInstanceIndex(tablixMember, (TablixDynamicMemberInstance)tablixMember.Instance);
						}
						int num8 = this.CreateTablixRows(tablix, tablixMember, defTreeLevel2, parentBorderHeader2, num3 + pageStructMemberCell2.PartialRowSpan, parentColIndex + pageMemberCell.ColSpan, ref children, pageMemberCell.IgnoreTotals, ref flag, flag2, createItems, startInTablix, endInTablix, pageContext);
						if (createItems.InnerSpanPages)
						{
							pageMemberCell.SpanPages = true;
							pageStructMemberCell2.SpanPages = true;
						}
						pageMemberCell.Children = children;
						bool flag4 = this.CheckPageBreaks(pageContext);
						if (tablixMember.IsTotal || (tablixMember.Visibility != null && tablixMember.Visibility.ToggleItem != null))
						{
							this.m_ignoreGroupPageBreaks--;
						}
						pageStructMemberCell2.Span += num8;
						pageMemberCell.RowSpan += num8;
						pageMemberCell.CurrRowSpan += num8;
						num4 += num8;
						num3 += pageStructMemberCell2.Span;
						if (disposable3 != null)
						{
							disposable3.Dispose();
							disposable3 = null;
						}
						if (flag)
						{
							if (tablixMember.IsStatic)
							{
								if (pageStructMemberCell2.Span == 0)
								{
									pageStructMemberCell2.DisposeInstances();
									pageStructMemberCell.RemoveAt(i);
									i--;
								}
								pageStructMemberCell2.PartialItem = false;
							}
							else
							{
								PageBreak pageBreak = tablixMember.Group.PageBreak;
								PageBreakProperties pageBreakProperties = PageBreakProperties.Create(pageBreak, tablixMember, pageContext);
								pageStructMemberCell2.CreateItem = true;
								TablixDynamicMemberInstance rowDynamicInstance = (TablixDynamicMemberInstance)tablixMember.Instance;
								if (this.AdvanceToNextVisibleInstance(tablix, tablixMember, rowDynamicInstance))
								{
									if (pageMemberCell.RowSpan == 0)
									{
										pageStructMemberCell2.RemoveLastMemberCell();
									}
									bool flag5 = false;
									if (flag4 && pageStructMemberCell2.Span > 0)
									{
										bool flag6 = false;
										if (this.ApplyDynamicPageBreaks(tablixMember, num3, flag2, ref flag6, ref flag5, pageBreakProperties, pageContext))
										{
											return num4;
										}
									}
									if (createItems.AdvanceRow)
									{
										i--;
										num3 -= pageStructMemberCell2.Span;
									}
									else
									{
										bool flag7 = false;
										if (!flag5)
										{
											flag7 = this.DynamicWithVisibleChildren(tablixMember);
										}
										if (flag7)
										{
											return num4;
										}
										i--;
										num3 -= pageStructMemberCell2.Span;
									}
								}
								else
								{
									pageStructMemberCell2.PartialItem = false;
									if (pageStructMemberCell2.Span == 0)
									{
										pageStructMemberCell2.DisposeInstances();
										pageStructMemberCell.RemoveAt(i);
										i--;
										int num9 = Tablix.RemoveHeadersAbove(pageStructMemberCell, i, this.m_detailRows, ref num3);
										i -= num9;
										num4 -= num9;
									}
									else if (flag4 && pageBreakProperties != null && pageBreakProperties.PageBreakAtEnd)
									{
										createItems.GroupPageBreaks = true;
										RowInfo rowInfo3 = null;
										using (this.m_detailRows.GetAndPin(num3 - 1, out rowInfo3))
										{
											rowInfo3.PageBreaksAtEnd = true;
											rowInfo3.PageBreakPropertiesAtEnd = pageBreakProperties;
										}
									}
								}
							}
							continue;
						}
						return num4;
					}
					num3 += pageStructMemberCell2.Span;
				}
			}
			if (num2 + 1 <= num5)
			{
				LevelInfo levelInfo3 = new LevelInfo(pageStructMemberCell, -1, num2, createItems);
				num2++;
				int num10 = this.CreateKeepWithRowMember(tablix, rowMemberParent, defTreeLevel, num3, parentColIndex, num2, num5, true, false, levelInfo3, pageContext);
				pageStructMemberCell = levelInfo3.MemberCells;
				num4 += num10;
				if (levelInfo3.PartialStruct)
				{
					return num4;
				}
				num2 = num5;
				num3 += num10;
			}
			if (num2 + 1 < tablixMemberCollection.Count)
			{
				LevelInfo levelInfo4 = new LevelInfo(pageStructMemberCell, -1, num2, createItems);
				if (ignoreTotals)
				{
					levelInfo4.IgnoreTotals = true;
				}
				num2++;
				int num11 = this.CreateRowMemberChildren(tablix, rowMemberParent, defTreeLevel, parentBorderHeader, num3, parentColIndex, num2, true, parentHasFooters, levelInfo4, pageContext);
				pageStructMemberCell = levelInfo4.MemberCells;
				num4 += num11;
				if (!levelInfo4.PartialStruct)
				{
					finishLevel = true;
				}
			}
			else
			{
				finishLevel = true;
			}
			return num4;
		}

		private void SetPageBreakAndKeepWith(int index, PageBreakProperties pageBreakProperties, bool overwrite)
		{
			RowInfo rowInfo = null;
			using (this.m_detailRows.GetAndPin(index, out rowInfo))
			{
				rowInfo.PageBreaksAtEnd = true;
				if (overwrite || rowInfo.PageBreakPropertiesAtEnd == null)
				{
					rowInfo.PageBreakPropertiesAtEnd = pageBreakProperties;
				}
				rowInfo.IgnoreKeepWith = true;
			}
		}

		private int MergeToggledDetailRows(int destIndex, int srcIndex, List<MergeDetailRows.DetailRowState> detailRowsState)
		{
			RowInfo rowInfo = null;
			RowInfo rowInfo2 = null;
			int num = 0;
			for (int i = 0; i < detailRowsState.Count; i++)
			{
				switch (detailRowsState[i])
				{
				case MergeDetailRows.DetailRowState.Merge:
					rowInfo = this.m_detailRows[destIndex];
					rowInfo2 = this.m_detailRows[srcIndex];
					rowInfo.Merge(rowInfo2);
					rowInfo2.DisposeDetailCells();
					destIndex++;
					srcIndex++;
					break;
				case MergeDetailRows.DetailRowState.Insert:
					this.m_detailRows.Insert(destIndex, this.m_detailRows[srcIndex]);
					destIndex++;
					srcIndex += 2;
					num++;
					break;
				default:
					destIndex++;
					break;
				}
			}
			return num;
		}

		private int CreateRowMemberChildren(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember rowMemberParent, int defTreeLevel, bool parentBorderHeader, int parentRowIndex, int parentColIndex, int sourceIndex, bool resetContext, bool parentHasFooters, LevelInfo parentLevelInfo, PageContext pageContext)
		{
			TablixMemberCollection tablixMemberCollection = null;
			tablixMemberCollection = ((rowMemberParent != null) ? rowMemberParent.Children : tablix.RowHierarchy.MemberCollection);
			if (tablixMemberCollection == null)
			{
				RowInfo rowInfo = this.CreateDetailRow(tablix, rowMemberParent, parentRowIndex, parentColIndex, pageContext);
				parentLevelInfo.CreateItems.UpdateInfo(rowInfo, 0.0);
				if (this.m_ignoreRow == 0)
				{
					parentLevelInfo.SourceSize += this.m_bodyRowsHeights[rowMemberParent.MemberCellIndex];
				}
				return 1;
			}
			int num = parentRowIndex;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			bool flag = true;
			LevelInfo levelInfo = null;
			bool flag2 = false;
			bool flag3 = true;
			int num5 = -1;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			bool keepWith = false;
			bool flag7 = false;
			bool flag8 = false;
			int num6 = 0;
			int num7 = 0;
			TablixMember tablixMember = null;
			Visibility visibility = null;
			TablixMemberInstance tablixMemberInstance = null;
			TablixDynamicMemberInstance tablixDynamicMemberInstance = null;
			List<int> list = null;
			int num8 = 0;
			for (int i = sourceIndex; i < tablixMemberCollection.Count; i++)
			{
				tablixMember = ((ReportElementCollectionBase<TablixMember>)tablixMemberCollection)[i];
				visibility = tablixMember.Visibility;
				flag8 = parentBorderHeader;
				flag2 = parentHasFooters;
				if ((!this.NoRows || !tablixMember.HideIfNoRows) && !this.AlwayHiddenMember(tablix, tablixMember, visibility, pageContext.EvaluatePageHeaderFooter))
				{
					flag4 = false;
					flag3 = true;
					flag = true;
					levelInfo = null;
					flag7 = tablixMember.KeepTogether;
					tablixMemberInstance = tablixMember.Instance;
					if (tablixMember.IsStatic)
					{
						if (!parentLevelInfo.CreateItems.AdvanceRow && !tablixMember.IsTotal)
						{
							if (num5 >= 0 && num2 > num5)
							{
								bool flag9 = false;
								bool flag10 = (byte)((!this.m_detailRows[num - 1].IgnoreKeepWith) ? 1 : 0) != 0;
								int num9 = this.CheckKeepWithGroupDown(tablixMemberCollection, i, KeepWithGroup.Before, ref flag9);
								if (num9 >= i && (flag9 || flag10))
								{
									bool keepWith2 = (byte)((!this.m_detailRows[num - 1].PageBreaksAtEnd) ? 1 : 0) != 0;
									bool partialDetailRow = parentLevelInfo.CreateItems.PartialDetailRow;
									parentLevelInfo.CreateItems.PartialDetailRow = false;
									this.m_ignoreCellPageBreaks++;
									num += this.CreateKeepWithRowMember(tablix, rowMemberParent, defTreeLevel, num, parentColIndex, i, num9, keepWith2, false, parentLevelInfo, pageContext);
									this.m_ignoreCellPageBreaks--;
									if (parentLevelInfo.CreateItems.PartialDetailRow)
									{
										return num - parentRowIndex;
									}
									parentLevelInfo.CreateItems.PartialDetailRow = partialDetailRow;
									i = num9;
									num5 = -1;
									continue;
								}
							}
							bool flag11 = true;
							if (flag2)
							{
								flag11 = this.StaticWithVisibleChildren(tablixMember);
							}
							if (flag11)
							{
								RowInfo rowInfo2 = null;
								using (this.m_detailRows.GetAndPin(num - 1, out rowInfo2))
								{
									rowInfo2.IgnoreKeepWith = true;
								}
								if (!flag8)
								{
									flag8 = this.StaticDecendents(tablixMember.Children);
								}
								num7 = this.AddTablixMemberDef(ref this.m_rowMemberDefIndexes, ref this.m_rowMemberDefList, tablixMember, flag8, defTreeLevel, i, pageContext);
								parentLevelInfo.AddPartialStructMemberCell(tablixMember, i, num7, true);
								return num - parentRowIndex;
							}
						}
						flag3 = this.EnterRowMember(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter);
						if (tablixMember.IsTotal)
						{
							if (!flag3 && num2 <= num5 && this.m_ignoreRow == 0 && num8 == 0 && !parentLevelInfo.IgnoreTotals)
							{
								if (list == null)
								{
									list = new List<int>();
								}
								list.Add(i);
							}
						}
						else if (!flag8)
						{
							flag8 = this.StaticDecendents(tablixMember.Children);
						}
						if (tablixMember.KeepWithGroup != 0)
						{
							this.m_ignoreCellPageBreaks++;
							keepWith = false;
							if (this.KeepTogetherStaticHeader(tablixMemberCollection, tablixMember, i, ref keepWith))
							{
								flag6 = true;
							}
							else if (num5 >= 0 && num2 > num5 && tablixMember.KeepWithGroup == KeepWithGroup.Before)
							{
								flag6 = true;
								keepWith = true;
							}
						}
						num5 = -1;
					}
					else
					{
						num5 = num;
						num2 = num;
						tablixDynamicMemberInstance = (TablixDynamicMemberInstance)tablixMemberInstance;
						if (resetContext || i > sourceIndex)
						{
							tablixDynamicMemberInstance.ResetContext();
							flag = tablixDynamicMemberInstance.MoveNext();
							flag4 = true;
							if (flag)
							{
								flag = this.CheckAndAdvanceToNextVisibleInstance(tablix, tablixMember, tablixDynamicMemberInstance);
							}
							this.SaveRowMemberInstanceIndex(tablixMember, tablixDynamicMemberInstance);
							if (flag && !flag2)
							{
								bool flag12 = false;
								int num10 = this.CheckKeepWithGroupDown(tablixMemberCollection, i + 1, KeepWithGroup.Before, ref flag12);
								if (num10 > i)
								{
									flag2 = true;
								}
							}
						}
						if (!parentLevelInfo.CreateItems.AdvanceRow)
						{
							if (!flag)
							{
								continue;
							}
							bool flag13 = true;
							if (flag2)
							{
								flag13 = this.DynamicWithVisibleChildren(tablixMember);
							}
							if (flag13)
							{
								RowInfo rowInfo3 = null;
								using (this.m_detailRows.GetAndPin(num - 1, out rowInfo3))
								{
									rowInfo3.IgnoreKeepWith = true;
								}
								num7 = this.AddTablixMemberDef(ref this.m_rowMemberDefIndexes, ref this.m_rowMemberDefList, tablixMember, flag8, defTreeLevel, i, pageContext);
								parentLevelInfo.AddPartialStructMemberCell(tablixMember, i, num7, true);
								return num - parentRowIndex;
							}
						}
						if (flag)
						{
							flag3 = this.EnterRowMember(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter);
						}
					}
					num7 = this.AddTablixMemberDef(ref this.m_rowMemberDefIndexes, ref this.m_rowMemberDefList, tablixMember, flag8, defTreeLevel, i, pageContext);
					for (; flag; num3 = 0)
					{
						if (flag6 || flag7)
						{
							if (levelInfo == null)
							{
								double spaceToFill = Math.Max(pageContext.ColumnHeight, parentLevelInfo.CreateItems.SpaceToFill);
								levelInfo = new LevelInfo(spaceToFill);
							}
							else
							{
								levelInfo = new LevelInfo(levelInfo.CreateItems.SpaceToFill);
							}
						}
						else
						{
							levelInfo = new LevelInfo(parentLevelInfo.CreateItems.SpaceToFill);
						}
						if (flag3)
						{
							if (!tablixMember.IsStatic && flag4 && this.CheckPageBreaks(pageContext))
							{
								PageBreak pageBreak = tablixMember.Group.PageBreak;
								PageBreakProperties pageBreakProperties = PageBreakProperties.Create(pageBreak, tablixMember, pageContext);
								if (pageBreakProperties != null && pageBreakProperties.PageBreakAtStart)
								{
									if (num > 1 && this.m_detailRows[num - 1].PageVerticalState != RowInfo.VerticalState.Above)
									{
										if (this.DynamicWithVisibleChildren(tablixMember))
										{
											parentLevelInfo.CreateItems.GroupPageBreaks = true;
											this.SetPageBreakAndKeepWith(num - 1, pageBreakProperties, false);
											parentLevelInfo.AddPartialStructMemberCell(tablixMember, i, num7, true);
											this.LeaveRowMember(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter);
											return num - parentRowIndex;
										}
									}
									else
									{
										flag5 = true;
									}
								}
							}
							num4 = 0;
							if (tablixMember.TablixHeader != null)
							{
								num4 = tablixMember.TablixHeader.CellContents.ColSpan;
								num6 = defTreeLevel - num4;
							}
							else
							{
								num6 = defTreeLevel - 1;
							}
							num3 = this.CreateRowMemberChildren(tablix, tablixMember, num6, flag8, num, parentColIndex + num4, 0, true, flag2, levelInfo, pageContext);
							if (num3 > 0)
							{
								if (flag4)
								{
									flag4 = false;
									if (flag5)
									{
										flag5 = false;
										RowInfo rowInfo4 = null;
										using (this.m_detailRows.GetAndPin(num, out rowInfo4))
										{
											rowInfo4.PageBreaksAtStart = true;
											rowInfo4.PageBreakPropertiesAtStart = PageBreakProperties.Create(tablixMember.Group.PageBreak, tablixMember, pageContext);
										}
										flag7 = false;
									}
								}
								PageMemberCell pageMemberCell = this.AddRowMember(tablixMember, parentColIndex, num3, num4, levelInfo, pageContext, 0.0);
								if (!levelInfo.OmittedList)
								{
									pageMemberCell.Children = levelInfo.MemberCells;
								}
								MergeDetailRows mergeDetailRows = parentLevelInfo.AddMemberCell(tablixMember, i, pageMemberCell, num7, TablixRegion.RowHeader, this.m_bodyRowsHeights, levelInfo, pageContext, this);
								if (mergeDetailRows != null)
								{
									num += this.MergeToggledDetailRows(num - mergeDetailRows.Span, num, mergeDetailRows.DetailRowsState);
									this.m_detailRows.RemoveRange(num, num3);
								}
								else
								{
									num += num3;
									if (!pageMemberCell.Hidden)
									{
										num2 += num3;
										num8 += num3;
										list = null;
										parentLevelInfo.IgnoreTotals = true;
									}
								}
							}
							else if (levelInfo.PartialLevel)
							{
								parentLevelInfo.AddPartialStructMemberCell(tablixMember, i, num7, true);
							}
						}
						if (tablixMember.IsStatic)
						{
							flag = false;
							flag7 = false;
							this.LeaveRowMember(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter);
							if (tablixMember.KeepWithGroup != 0)
							{
								this.m_ignoreCellPageBreaks--;
							}
							if (parentLevelInfo.CreateItems.ContentFullyCreated && !levelInfo.PartialLevel)
							{
								if (!parentLevelInfo.CreateItems.PartialDetailRow && flag6)
								{
									int num11 = num - num3;
									bool flag14 = false;
									bool flag15 = false;
									int num12 = this.CheckKeepWithGroupDown(tablixMemberCollection, i + 1, tablixMember.KeepWithGroup, ref flag15);
									if (num3 > 0)
									{
										if (num12 <= i || tablixMember.KeepWithGroup == KeepWithGroup.Before)
										{
											parentLevelInfo.SetKeepWith(1, keepWith, tablixMember.RepeatOnNewPage);
											RowInfo rowInfo5 = null;
											for (int j = 0; j < num3; j++)
											{
												using (this.m_detailRows.GetAndPin(j + num11, out rowInfo5))
												{
													rowInfo5.RepeatWith = tablixMember.RepeatOnNewPage;
												}
											}
										}
										else
										{
											flag14 = true;
										}
									}
									if (num12 > i)
									{
										this.m_ignoreCellPageBreaks++;
										num += this.CreateKeepWithRowMember(tablix, rowMemberParent, defTreeLevel, num, parentColIndex, i + 1, num12, keepWith, flag14, parentLevelInfo, pageContext);
										this.m_ignoreCellPageBreaks--;
										if (parentLevelInfo.CreateItems.PartialDetailRow)
										{
											parentLevelInfo.CreateItems.ContentFullyCreated = false;
											return num - parentRowIndex;
										}
										if (flag14)
										{
											RowInfo rowInfo6 = null;
											for (int k = 0; k < num3; k++)
											{
												using (this.m_detailRows.GetAndPin(k + num11, out rowInfo6))
												{
													rowInfo6.RepeatWith = tablixMember.RepeatOnNewPage;
												}
											}
										}
										i = num12;
									}
									flag6 = false;
									if (tablixMember.KeepWithGroup == KeepWithGroup.After)
									{
										parentLevelInfo.CreateItems.AdvanceRow = true;
										flag7 = true;
									}
									else if (tablixMember.KeepWithGroup == KeepWithGroup.Before)
									{
										flag7 = true;
									}
								}
								if (!flag7 && num > parentRowIndex && parentLevelInfo.CreateItems.SpaceToFill <= 0.01)
								{
									parentLevelInfo.CreateItems.AdvanceRow = false;
								}
								continue;
							}
							return num - parentRowIndex;
						}
						if (!levelInfo.PartialLevel && parentLevelInfo.CreateItems.ContentFullyCreated)
						{
							PageBreak pageBreak2 = tablixMember.Group.PageBreak;
							PageBreakProperties pageBreakProperties2 = PageBreakProperties.Create(pageBreak2, tablixMember, pageContext);
							bool currentlyHidden = false;
							if (visibility != null && visibility.ToggleItem != null)
							{
								currentlyHidden = tablixMember.Instance.Visibility.CurrentlyHidden;
							}
							flag = this.AdvanceToNextVisibleInstance(tablix, tablixMember, tablixDynamicMemberInstance);
							bool flag16 = false;
							bool keepWith3 = true;
							if (this.CheckPageBreaks(pageContext) && num2 > num5)
							{
								if (flag)
								{
									if (this.ApplyDynamicPageBreaks(tablixMember, num, flag2, ref keepWith3, ref flag16, pageBreakProperties2, pageContext))
									{
										parentLevelInfo.CreateItems.GroupPageBreaks = true;
										num5 = -1;
										this.LeaveRowMember(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter, currentlyHidden);
										bool flag17 = false;
										int num13 = this.CheckKeepWithGroupDown(tablixMemberCollection, i + 1, KeepWithGroup.Before, ref flag17);
										if (num13 > i && flag17)
										{
											this.m_ignoreCellPageBreaks++;
											num += this.CreateKeepWithRowMember(tablix, rowMemberParent, defTreeLevel, num, parentColIndex, i + 1, num13, keepWith3, false, parentLevelInfo, pageContext);
											this.m_ignoreCellPageBreaks--;
										}
										return num - parentRowIndex;
									}
								}
								else if (pageBreakProperties2 != null && pageBreakProperties2.PageBreakAtEnd)
								{
									parentLevelInfo.CreateItems.GroupPageBreaks = true;
									this.SetPageBreakAndKeepWith(num - 1, pageBreakProperties2, true);
									num5 = -1;
								}
							}
							if (flag)
							{
								if (num > num5)
								{
									if (flag7)
									{
										if (levelInfo.CreateItems.SpaceToFill <= -0.01)
										{
											parentLevelInfo.CreateItems.AdvanceRow = false;
										}
									}
									else if (parentLevelInfo.CreateItems.SpaceToFill <= -0.01)
									{
										parentLevelInfo.CreateItems.AdvanceRow = false;
									}
								}
								if (!parentLevelInfo.CreateItems.AdvanceRow)
								{
									bool flag18 = false;
									if (!flag16)
									{
										flag18 = this.DynamicWithVisibleChildren(tablixMember);
									}
									if (flag18)
									{
										this.LeaveRowMember(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter, currentlyHidden);
										num5 = -1;
										RowInfo rowInfo7 = null;
										using (this.m_detailRows.GetAndPin(num - 1, out rowInfo7))
										{
											rowInfo7.IgnoreKeepWith = true;
										}
										bool flag19 = false;
										int num14 = this.CheckKeepWithGroupDown(tablixMemberCollection, i + 1, KeepWithGroup.Before, ref flag19);
										if (num14 > i && flag19)
										{
											this.m_ignoreCellPageBreaks++;
											num += this.CreateKeepWithRowMember(tablix, rowMemberParent, defTreeLevel, num, parentColIndex, i + 1, num14, keepWith3, false, parentLevelInfo, pageContext);
											this.m_ignoreCellPageBreaks--;
										}
										return num - parentRowIndex;
									}
								}
								this.LeaveRowMemberInstance(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter, currentlyHidden);
								flag3 = this.EnterRowMemberInstance(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter);
							}
							else
							{
								parentLevelInfo.FinishPartialStructMember();
								this.LeaveRowMember(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter, currentlyHidden);
							}
							continue;
						}
						this.LeaveRowMember(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter);
						bool flag20 = false;
						int num15 = this.CheckKeepWithGroupDown(tablixMemberCollection, i + 1, KeepWithGroup.Before, ref flag20);
						if (num15 <= i || !flag20)
						{
							return num - parentRowIndex;
						}
						this.m_ignoreCellPageBreaks++;
						num += this.CreateKeepWithRowMember(tablix, rowMemberParent, defTreeLevel, num, parentColIndex, i + 1, num15, true, false, parentLevelInfo, pageContext);
						this.m_ignoreCellPageBreaks--;
						break;
					}
					tablixDynamicMemberInstance = null;
				}
			}
			if (num8 == 0)
			{
				num += this.CreateRowMemberTotals(tablix, list, tablixMemberCollection, defTreeLevel, parentBorderHeader, num, parentColIndex, parentHasFooters, parentLevelInfo, pageContext);
				if (num > parentRowIndex && parentLevelInfo.CreateItems.SpaceToFill <= 0.01)
				{
					parentLevelInfo.CreateItems.AdvanceRow = false;
				}
			}
			if (num >= 1)
			{
				RowInfo rowInfo8 = null;
				using (this.m_detailRows.GetAndPin(num - 1, out rowInfo8))
				{
					rowInfo8.IgnoreKeepWith = rowInfo8.PageBreaksAtEnd;
				}
			}
			return num - parentRowIndex;
		}

		private int CreateRowMemberTotals(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix, List<int> totals, TablixMemberCollection rowMembers, int defTreeLevel, bool parentBorderHeader, int parentRowIndex, int parentColIndex, bool parentHasFooters, LevelInfo parentLevelInfo, PageContext pageContext)
		{
			if (totals != null && totals.Count != 0)
			{
				double sizeForParent = 0.0;
				int num = 2147483647;
				bool flag = false;
				int num2 = 0;
				TablixMember tablixMember = null;
				if (parentColIndex > 0)
				{
					for (int i = 0; i < totals.Count; i++)
					{
						num2 = totals[i];
						tablixMember = ((ReportElementCollectionBase<TablixMember>)rowMembers)[num2];
						if (tablixMember.TablixHeader != null && tablixMember.TablixHeader.CellContents.ColSpan < num)
						{
							num = tablixMember.TablixHeader.CellContents.ColSpan;
							sizeForParent = tablixMember.TablixHeader.Size.ToMillimeters();
							flag = true;
						}
					}
					if (flag)
					{
						parentLevelInfo.SpanForParent = num;
						parentLevelInfo.SizeForParent = sizeForParent;
					}
				}
				int num3 = parentRowIndex;
				int num4 = 0;
				int num5 = 0;
				LevelInfo levelInfo = null;
				int num6 = 0;
				int num7 = -1;
				for (int j = 0; j < totals.Count; j++)
				{
					num2 = totals[j];
					tablixMember = ((ReportElementCollectionBase<TablixMember>)rowMembers)[num2];
					this.EnterRowMember(tablixMember, null, false);
					num7 = this.AddTablixMemberDef(ref this.m_rowMemberDefIndexes, ref this.m_rowMemberDefList, tablixMember, parentBorderHeader, defTreeLevel, j, pageContext);
					levelInfo = new LevelInfo(1.7976931348623157E+308);
					num5 = 0;
					if (tablixMember.TablixHeader != null)
					{
						num5 = tablixMember.TablixHeader.CellContents.ColSpan;
						num6 = defTreeLevel - num5;
					}
					else
					{
						num6 = defTreeLevel - 1;
					}
					num4 = this.CreateRowMemberChildren(tablix, tablixMember, num6, parentBorderHeader, num3, parentColIndex + num5, 0, true, parentHasFooters, levelInfo, pageContext);
					if (num4 > 0)
					{
						PageMemberCell pageMemberCell = this.AddTotalRowMember(tablixMember, parentColIndex, num4, num5, parentLevelInfo, levelInfo, pageContext);
						if (!levelInfo.OmittedList)
						{
							pageMemberCell.Children = levelInfo.MemberCells;
						}
						parentLevelInfo.AddMemberCell(tablixMember, num2, pageMemberCell, num7, TablixRegion.RowHeader, this.m_bodyRowsHeights, levelInfo, pageContext, this);
						num3 += num4;
					}
					this.LeaveRowMember(tablixMember, null, false);
				}
				return num3 - parentRowIndex;
			}
			return 0;
		}

		private RowInfo CreateDetailRow(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember rowMemberParent, int parentRowIndex, int parentColIndex, PageContext pageContext)
		{
			this.m_rowMemberIndexCell = rowMemberParent.MemberCellIndex;
			RowInfo rowInfo = null;
			if (this.m_ignoreRow > 0)
			{
				rowInfo = new RowInfo(0.0);
				rowInfo.Hidden = true;
			}
			else
			{
				rowInfo = ((this.m_ignoreCellPageBreaks <= 0) ? new RowInfo(0.0) : new RowInfo(this.m_bodyRowsHeights[this.m_rowMemberIndexCell]));
			}
			int num = 0;
			int num2 = 0;
			this.TraverseColumnMembers(tablix, (TablixMember)null, parentColIndex, rowInfo, true, 0.0, pageContext.ColumnHeight, ref num, out num2, (List<int>)null, pageContext);
			PageContext pageContext2 = new PageContext(pageContext, true);
			if (this.m_ignoreCellPageBreaks > 0 || this.m_ignoreGroupPageBreaks > 0)
			{
				pageContext2.IgnorePageBreaks = true;
				if (!pageContext.IgnorePageBreaks)
				{
					if (this.m_ignoreCellPageBreaks > 0)
					{
						pageContext2.IgnorePageBreaksReason = PageContext.IgnorePageBreakReason.InsideTablixCell;
					}
					else
					{
						pageContext2.IgnorePageBreaksReason = PageContext.IgnorePageBreakReason.ToggleableItem;
					}
				}
			}
			rowInfo.CalculateVerticalLastDetailCell(pageContext2, false, false);
			if (!this.IsLTR)
			{
				rowInfo.ReverseDetailCells(pageContext);
			}
			if (this.m_detailRows == null)
			{
				this.m_detailRows = new ScalableList<RowInfo>(0, pageContext.ScalabilityCache);
			}
			if (parentRowIndex >= this.m_detailRows.Count)
			{
				this.m_detailRows.Add(rowInfo);
			}
			else
			{
				this.m_detailRows.Insert(parentRowIndex, rowInfo);
			}
			this.m_colMemberIndexCell = -1;
			return rowInfo;
		}

		private void UpdateDetailRow(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember rowMemberParent, RowInfo currRowInfo, int parentColIndex, double startInTablix, double endInTablix, PageContext pageContext)
		{
			this.m_rowMemberIndexCell = rowMemberParent.MemberCellIndex;
			currRowInfo.UpdatePinnedRow();
			int num = 0;
			int num2 = 0;
			this.TraverseColumnMembers(tablix, (TablixMember)null, parentColIndex, currRowInfo, false, startInTablix, endInTablix, ref num, out num2, (List<int>)null, pageContext);
			this.m_colMemberIndexCell = -1;
		}

		private int CreateDynamicRowMemberChildren(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember rowMemberParent, int defTreeLevel, int sourceIndex, int parentRowIndex, int parentColIndex, bool parentHasFooters, LevelInfo parentLevelInfo, PageContext pageContext)
		{
			TablixMemberCollection tablixMemberCollection = null;
			tablixMemberCollection = ((rowMemberParent != null) ? rowMemberParent.Children : tablix.RowHierarchy.MemberCollection);
			if (tablixMemberCollection == null)
			{
				RowInfo rowInfo = this.CreateDetailRow(tablix, rowMemberParent, parentRowIndex, parentColIndex, pageContext);
				parentLevelInfo.CreateItems.UpdateInfo(rowInfo, 0.0);
				parentLevelInfo.SourceSize += this.m_bodyRowsHeights[rowMemberParent.MemberCellIndex];
				return 1;
			}
			int num = parentRowIndex;
			int num2 = 0;
			int num3 = 0;
			bool flag = true;
			LevelInfo levelInfo = null;
			int num4 = 0;
			int num5 = num;
			TablixMember tablixMember = ((ReportElementCollectionBase<TablixMember>)tablixMemberCollection)[sourceIndex];
			Visibility visibility = tablixMember.Visibility;
			TablixDynamicMemberInstance tablixDynamicMemberInstance = (TablixDynamicMemberInstance)tablixMember.Instance;
			this.RestoreRowMemberInstanceIndex(tablixMember, tablixDynamicMemberInstance);
			bool flag2 = this.EnterRowMember(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter);
			while (flag)
			{
				levelInfo = new LevelInfo(parentLevelInfo.CreateItems.SpaceToFill);
				if (flag2)
				{
					num3 = 0;
					if (tablixMember.TablixHeader != null)
					{
						num3 = tablixMember.TablixHeader.CellContents.ColSpan;
						num4 = defTreeLevel - num3;
					}
					else
					{
						num4 = defTreeLevel - 1;
					}
					num2 = this.CreateRowMemberChildren(tablix, tablixMember, num4, false, num, parentColIndex + num3, 0, true, parentHasFooters, levelInfo, pageContext);
					if (num2 > 0)
					{
						PageMemberCell pageMemberCell = this.AddRowMember(tablixMember, parentColIndex, num2, num3, levelInfo, pageContext, 0.0);
						if (!levelInfo.OmittedList)
						{
							pageMemberCell.Children = levelInfo.MemberCells;
						}
						MergeDetailRows mergeDetailRows = parentLevelInfo.AddMemberCell(tablixMember, sourceIndex, pageMemberCell, -1, TablixRegion.RowHeader, this.m_bodyRowsHeights, levelInfo, pageContext, this);
						if (mergeDetailRows != null)
						{
							num += this.MergeToggledDetailRows(num - mergeDetailRows.Span, num, mergeDetailRows.DetailRowsState);
							this.m_detailRows.RemoveRange(num, num2);
						}
						else
						{
							num += num2;
						}
					}
				}
				if (levelInfo.PartialLevel)
				{
					this.LeaveRowMember(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter);
					return num - parentRowIndex;
				}
				if (!parentLevelInfo.CreateItems.ContentFullyCreated)
				{
					this.LeaveRowMember(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter);
					return num - parentRowIndex;
				}
				PageBreak pageBreak = tablixMember.Group.PageBreak;
				PageBreakProperties pageBreakProperties = PageBreakProperties.Create(pageBreak, tablixMember, pageContext);
				bool currentlyHidden = false;
				if (visibility != null && visibility.ToggleItem != null)
				{
					currentlyHidden = tablixMember.Instance.Visibility.CurrentlyHidden;
				}
				flag = this.AdvanceToNextVisibleInstance(tablix, tablixMember, tablixDynamicMemberInstance);
				bool flag3 = false;
				if (this.CheckPageBreaks(pageContext) && num > num5)
				{
					if (flag)
					{
						bool flag4 = false;
						if (this.ApplyDynamicPageBreaks(tablixMember, num, parentHasFooters, ref flag4, ref flag3, pageBreakProperties, pageContext))
						{
							this.LeaveRowMember(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter, currentlyHidden);
							parentLevelInfo.CreateItems.GroupPageBreaks = true;
							return num - parentRowIndex;
						}
					}
					else if (pageBreakProperties != null && pageBreakProperties.PageBreakAtEnd)
					{
						parentLevelInfo.CreateItems.GroupPageBreaks = true;
						this.SetPageBreakAndKeepWith(num - 1, pageBreakProperties, true);
					}
				}
				if (flag)
				{
					if (parentLevelInfo.CreateItems.SpaceToFill <= 0.01)
					{
						parentLevelInfo.CreateItems.AdvanceRow = false;
					}
					if (!parentLevelInfo.CreateItems.AdvanceRow)
					{
						bool flag5 = false;
						if (!flag3)
						{
							flag5 = this.DynamicWithVisibleChildren(tablixMember);
						}
						if (flag5)
						{
							this.LeaveRowMember(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter, currentlyHidden);
							return num - parentRowIndex;
						}
					}
					this.LeaveRowMemberInstance(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter, currentlyHidden);
					flag2 = this.EnterRowMemberInstance(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter);
				}
				else
				{
					parentLevelInfo.FinishPartialStructMember();
					this.LeaveRowMember(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter, currentlyHidden);
				}
			}
			return num - parentRowIndex;
		}

		private bool ApplyDynamicPageBreaks(TablixMember rowMember, int rowIndex, bool memberHasFooters, ref bool keepWith, ref bool checkVisibleChildren, PageBreakProperties pageBreakProperties, PageContext pageContext)
		{
			bool flag = false;
			PageBreak pageBreak = rowMember.Group.PageBreak;
			if (pageBreakProperties != null && pageBreakProperties.PageBreakAtEnd)
			{
				keepWith = false;
				bool flag2 = true;
				if (memberHasFooters)
				{
					checkVisibleChildren = true;
					flag2 = this.DynamicWithVisibleChildren(rowMember);
				}
				if (flag2)
				{
					this.SetPageBreakAndKeepWith(rowIndex - 1, pageBreakProperties, true);
					flag = true;
				}
			}
			if (!flag && !checkVisibleChildren)
			{
				if (!pageBreak.Instance.Disabled && (pageBreak.BreakLocation == PageBreakLocation.Between || pageBreak.BreakLocation == PageBreakLocation.Start))
				{
					checkVisibleChildren = true;
					if (this.DynamicWithVisibleChildren(rowMember))
					{
						this.SetPageBreakAndKeepWith(rowIndex - 1, PageBreakProperties.Create(pageBreak, rowMember, pageContext), true);
						flag = true;
					}
				}
				else if (pageContext.Common.DiagnosticsEnabled && pageBreak.Instance.Disabled && (pageBreak.BreakLocation == PageBreakLocation.Between || pageBreak.BreakLocation == PageBreakLocation.Start))
				{
					pageContext.Common.TracePageBreakIgnoredDisabled(rowMember);
				}
			}
			return flag;
		}

		private bool KeepTogetherStaticHeader(TablixMemberCollection rowMembers, TablixMember staticMember, int staticIndex, ref bool keepWithGroup)
		{
			if (staticMember.KeepWithGroup != KeepWithGroup.After)
			{
				return false;
			}
			bool repeatOnNewPage = staticMember.RepeatOnNewPage;
			int num = this.CheckKeepWithGroupDown(rowMembers, staticIndex + 1, KeepWithGroup.After, ref repeatOnNewPage);
			num++;
			if (num < rowMembers.Count)
			{
				TablixMember tablixMember = ((ReportElementCollectionBase<TablixMember>)rowMembers)[num];
				TablixDynamicMemberInstance tablixDynamicMemberInstance = tablixMember.Instance as TablixDynamicMemberInstance;
				if (tablixDynamicMemberInstance == null)
				{
					return false;
				}
				if (tablixMember.Visibility != null)
				{
					if (tablixMember.Visibility.HiddenState == SharedHiddenState.Always)
					{
						return false;
					}
					if (tablixMember.Visibility.ToggleItem != null)
					{
						int num2 = num + 1;
						if (num2 < rowMembers.Count && ((ReportElementCollectionBase<TablixMember>)rowMembers)[num2].IsTotal)
						{
							keepWithGroup = true;
							return true;
						}
					}
				}
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				tablixDynamicMemberInstance.ResetContext();
				bool flag4 = tablixDynamicMemberInstance.MoveNext();
				while (flag4)
				{
					flag3 = false;
					if (this.RenderRowMemberInstance(tablixMember))
					{
						flag3 = this.MemberWithVisibleChildren(tablixMember, ref flag2);
					}
					if (flag3)
					{
						if (flag2)
						{
							keepWithGroup = false;
							return repeatOnNewPage;
						}
						keepWithGroup = true;
						return true;
					}
					flag4 = tablixDynamicMemberInstance.MoveNext();
				}
			}
			return false;
		}

		private bool DynamicWithVisibleChildren(TablixMember rowMemberParent)
		{
			bool flag = false;
			return this.MemberWithVisibleChildren(rowMemberParent, ref flag);
		}

		private bool StaticWithVisibleChildren(TablixMember rowMemberParent)
		{
			if (rowMemberParent == null)
			{
				return false;
			}
			bool flag = this.RenderRowMemberInstance(rowMemberParent);
			if (flag)
			{
				bool flag2 = false;
				flag = this.MemberWithVisibleChildren(rowMemberParent, ref flag2);
			}
			return flag;
		}

		private bool MemberWithVisibleChildren(TablixMember rowMemberParent, ref bool childPageBreakAtStart)
		{
			if (rowMemberParent == null)
			{
				return false;
			}
			TablixMemberCollection children = rowMemberParent.Children;
			if (children != null && children.Count != 0)
			{
				bool flag = true;
				bool flag2 = true;
				TablixMember tablixMember = null;
				TablixMemberInstance tablixMemberInstance = null;
				TablixDynamicMemberInstance tablixDynamicMemberInstance = null;
				for (int i = 0; i < children.Count; i++)
				{
					tablixMember = ((ReportElementCollectionBase<TablixMember>)children)[i];
					if (tablixMember.Visibility == null || tablixMember.Visibility.HiddenState != 0)
					{
						flag2 = true;
						flag = true;
						tablixMemberInstance = tablixMember.Instance;
						if (tablixMember.IsStatic)
						{
							flag2 = this.RenderRowMemberInstance(tablixMember);
						}
						else
						{
							if (tablixMember.Visibility != null && tablixMember.Visibility.ToggleItem != null)
							{
								int num = i + 1;
								if (num < children.Count && ((ReportElementCollectionBase<TablixMember>)children)[num].IsTotal)
								{
									return true;
								}
							}
							tablixDynamicMemberInstance = (TablixDynamicMemberInstance)tablixMemberInstance;
							tablixDynamicMemberInstance.ResetContext();
							flag = tablixDynamicMemberInstance.MoveNext();
							if (flag)
							{
								flag2 = this.RenderRowMemberInstance(tablixMember);
							}
						}
						while (flag)
						{
							if (flag2 && this.MemberWithVisibleChildren(tablixMember, ref childPageBreakAtStart))
							{
								Group group = rowMemberParent.Group;
								if (group != null)
								{
									PageBreakLocation breakLocation = group.PageBreak.BreakLocation;
									if (breakLocation == PageBreakLocation.Start || breakLocation == PageBreakLocation.StartAndEnd)
									{
										childPageBreakAtStart = true;
									}
								}
								return true;
							}
							if (tablixMember.IsStatic)
							{
								flag = false;
							}
							else
							{
								flag = tablixDynamicMemberInstance.MoveNext();
								if (flag)
								{
									flag2 = this.RenderRowMemberInstance(tablixMember);
								}
							}
						}
						tablixDynamicMemberInstance = null;
					}
				}
				return false;
			}
			return true;
		}

		private bool CheckAndAdvanceToNextVisibleInstance(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember rowMember, TablixDynamicMemberInstance rowDynamicInstance)
		{
			bool flag = true;
			bool flag2 = false;
			Visibility visibility = rowMember.Visibility;
			if (visibility != null && visibility.HiddenState == SharedHiddenState.Sometimes && visibility.ToggleItem == null)
			{
				while (flag && rowDynamicInstance.Visibility.CurrentlyHidden)
				{
					flag = rowDynamicInstance.MoveNext();
				}
			}
			return flag;
		}

		private bool AdvanceToNextVisibleInstance(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember rowMember, TablixDynamicMemberInstance rowDynamicInstance)
		{
			bool flag = rowDynamicInstance.MoveNext();
			if (flag)
			{
				flag = this.CheckAndAdvanceToNextVisibleInstance(tablix, rowMember, rowDynamicInstance);
			}
			this.SaveRowMemberInstanceIndex(rowMember, rowDynamicInstance);
			return flag;
		}

		private int CheckKeepWithGroupDown(TablixMemberCollection rowMembers, int start, KeepWithGroup keepWith, ref bool repeatWith)
		{
			TablixMember tablixMember = null;
			while (start < rowMembers.Count)
			{
				tablixMember = ((ReportElementCollectionBase<TablixMember>)rowMembers)[start];
				if (!tablixMember.IsStatic || tablixMember.KeepWithGroup != keepWith)
				{
					return start - 1;
				}
				start++;
				repeatWith = tablixMember.RepeatOnNewPage;
			}
			return start - 1;
		}

		private int CreateKeepWithRowMember(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember rowMemberParent, int defTreeLevel, int parentRowIndex, int parentColIndex, int start, int end, bool keepWith, bool setPrevHeader, LevelInfo parentLevelInfo, PageContext pageContext)
		{
			if (start > end)
			{
				return 0;
			}
			TablixMemberCollection tablixMemberCollection = null;
			tablixMemberCollection = ((rowMemberParent != null) ? rowMemberParent.Children : tablix.RowHierarchy.MemberCollection);
			if (tablixMemberCollection == null)
			{
				RowInfo rowInfo = this.CreateDetailRow(tablix, rowMemberParent, parentRowIndex, parentColIndex, pageContext);
				parentLevelInfo.CreateItems.UpdateInfo(rowInfo, 0.0);
				parentLevelInfo.SourceSize += this.m_bodyRowsHeights[rowMemberParent.MemberCellIndex];
				return 1;
			}
			int num = parentRowIndex;
			int num2 = 0;
			int num3 = 0;
			LevelInfo levelInfo = null;
			bool flag = true;
			TablixMember tablixMember = null;
			Visibility visibility = null;
			int num4 = -1;
			int num5 = 0;
			int num6 = 0;
			double spaceToFill = Math.Max(pageContext.ColumnHeight, parentLevelInfo.CreateItems.SpaceToFill);
			if (start == -1 && end == -1)
			{
				start = 0;
				end = tablixMemberCollection.Count - 1;
			}
			for (int i = start; i <= end; i++)
			{
				tablixMember = ((ReportElementCollectionBase<TablixMember>)tablixMemberCollection)[i];
				visibility = tablixMember.Visibility;
				if (!this.AlwayHiddenMember(tablix, tablixMember, visibility, pageContext.EvaluatePageHeaderFooter))
				{
					flag = this.EnterRowMember(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter);
					if (flag)
					{
						num4 = this.AddTablixMemberDef(ref this.m_rowMemberDefIndexes, ref this.m_rowMemberDefList, tablixMember, true, defTreeLevel, i, pageContext);
						num2 = 0;
						if (tablixMember.TablixHeader != null)
						{
							num2 = tablixMember.TablixHeader.CellContents.ColSpan;
							num5 = defTreeLevel - num2;
						}
						else
						{
							num5 = defTreeLevel - 1;
						}
						levelInfo = new LevelInfo(spaceToFill);
						num3 = this.CreateKeepWithRowMember(tablix, tablixMember, num5, num, parentColIndex + num2, -1, -1, true, false, levelInfo, pageContext);
						if (num3 > 0)
						{
							PageMemberCell pageMemberCell = this.AddRowMember(tablixMember, parentColIndex, num3, num2, levelInfo, pageContext, 0.0);
							if (!levelInfo.OmittedList)
							{
								pageMemberCell.Children = levelInfo.MemberCells;
							}
							parentLevelInfo.AddMemberCell(tablixMember, i, pageMemberCell, num4, TablixRegion.RowHeader, this.m_bodyRowsHeights, levelInfo, pageContext, this);
							num6++;
							num += num3;
						}
					}
					this.LeaveRowMember(tablixMember, visibility, pageContext.EvaluatePageHeaderFooter);
					if (flag && !levelInfo.CreateItems.AdvanceRow)
					{
						num6--;
						break;
					}
				}
			}
			if (num6 > 0)
			{
				if (tablixMember.KeepWithGroup == KeepWithGroup.After && !levelInfo.CreateItems.AdvanceRow)
				{
					return num - parentRowIndex;
				}
				if (setPrevHeader)
				{
					num6++;
				}
				parentLevelInfo.SetKeepWith(num6, keepWith, tablixMember.RepeatOnNewPage);
				for (int j = parentRowIndex; j < num; j++)
				{
					this.m_detailRows[j].RepeatWith = tablixMember.RepeatOnNewPage;
				}
			}
			else if (setPrevHeader)
			{
				parentLevelInfo.SetKeepWith(1, keepWith, tablixMember.RepeatOnNewPage);
			}
			return num - parentRowIndex;
		}

		private bool RenderRowMemberInstance(TablixMember rowMember)
		{
			if (rowMember.IsTotal)
			{
				return false;
			}
			Visibility visibility = rowMember.Visibility;
			if (visibility == null)
			{
				return true;
			}
			if (visibility.HiddenState == SharedHiddenState.Never)
			{
				return true;
			}
			return !rowMember.Instance.Visibility.CurrentlyHidden;
		}

		private bool IsLastVisibleMember(int memberIndex, List<PageStructMemberCell> members)
		{
			if (members.Count != 0 && memberIndex >= 0)
			{
				int num = -1;
				for (int i = 0; i < members.Count; i++)
				{
					if (!members[i].Hidden)
					{
						num = i;
					}
				}
				return memberIndex == num;
			}
			return false;
		}

		private void TraceMember(PageContext pageContext, TablixMember tablixMember, RenderingArea renderingArea, TraceLevel traceLevel, string message)
		{
			if (pageContext.Common.DiagnosticsEnabled)
			{
				string text = DiagnosticsUtilities.BuildTablixMemberPath(base.m_source.Name, tablixMember, this.m_memberAtLevelIndexes);
				RenderingDiagnostics.Trace(renderingArea, traceLevel, message, pageContext.Common.PageNumber, text);
			}
		}

		public void TraceKeepTogetherRowMember(PageContext pageContext, PageStructMemberCell memberCell)
		{
			this.TraceMember(pageContext, memberCell.TablixMember, RenderingArea.KeepTogether, TraceLevel.Info, "PR-DIAG [Page {0}] Member '{1}' kept together - Explicit - Pushed to next page");
		}

		public void TraceKeepTogetherColumnMember(PageContext pageContext, PageStructMemberCell memberCell)
		{
			this.TraceMember(pageContext, memberCell.TablixMember, RenderingArea.KeepTogether, TraceLevel.Info, "PR-DIAG [Page {0}] Member '{1}' kept together - Explicit - Pushed to next page");
		}

		public void TraceInvalidatedKeepTogetherMember(PageContext pageContext, PageStructMemberCell memberCell)
		{
			this.TraceMember(pageContext, memberCell.TablixMember, RenderingArea.KeepTogether, TraceLevel.Info, "PR-DIAG [Page {0}] KeepTogether on Member '{1}' not honored - larger than page");
		}

		public void TraceKeepTogetherRow(PageContext pageContext, RowInfo rowInfo)
		{
			if (pageContext.Common.DiagnosticsEnabled)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (PageDetailCell cell in rowInfo.Cells)
				{
					if (cell.KeepTogether && cell.CellItem != null)
					{
						if (stringBuilder.Length != 0)
						{
							stringBuilder.Append(",");
						}
						stringBuilder.Append(cell.CellItem.Source.Name);
					}
				}
				RenderingDiagnostics.Trace(RenderingArea.KeepTogether, TraceLevel.Info, "PR-DIAG [Page {0}] Item(s) '{1}' kept together - RowContext - Pushed to next page", pageContext.Common.PageNumber, stringBuilder.ToString());
			}
		}

		public void TraceKeepTogetherColumn(PageContext pageContext, ColumnInfo columnInfo)
		{
			if (pageContext.Common.DiagnosticsEnabled)
			{
				RenderingDiagnostics.Trace(RenderingArea.KeepTogether, TraceLevel.Info, "PR-DIAG [Page {0}] Item kept together - ColumnContext - Pushed to next page", pageContext.Common.PageNumber);
			}
		}

		public void TraceKeepTogetherRowMemberRow(PageContext pageContext, RowInfo rowInfo, PageStructMemberCell memberCell, bool memberKeepTogether)
		{
			if (memberKeepTogether)
			{
				this.TraceKeepTogetherRowMember(pageContext, memberCell);
			}
			if (rowInfo.KeepTogether)
			{
				this.TraceKeepTogetherRow(pageContext, rowInfo);
			}
		}

		public void TraceKeepTogetherColumnMemberColumn(PageContext pageContext, ColumnInfo columnInfo, PageStructMemberCell memberCell, bool memberKeepTogether)
		{
			if (memberKeepTogether)
			{
				this.TraceKeepTogetherColumnMember(pageContext, memberCell);
			}
			if (columnInfo.KeepTogether)
			{
				this.TraceKeepTogetherColumn(pageContext, columnInfo);
			}
		}

		public void TraceRepeatOnNewPage(PageContext pageContext, HeaderFooterRow headerFooterRow)
		{
			TablixMember tablixMember = headerFooterRow.StructMemberCell.TablixMember;
			this.TraceMember(pageContext, tablixMember, RenderingArea.RepeatOnNewPage, TraceLevel.Info, "PR-DIAG [Page {0}] '{1}' appears on page due to RepeatOnNewPage");
		}

		public void TraceInvalidatedRepeatOnNewPageSize(PageContext pageContext, HeaderFooterRow headerFooterRow)
		{
			TablixMember tablixMember = headerFooterRow.StructMemberCell.TablixMember;
			this.TraceMember(pageContext, tablixMember, RenderingArea.RepeatOnNewPage, TraceLevel.Info, "PR-DIAG [Page {0}] '{1}' not repeated due to page size contraints");
		}

		public void TraceInvalidatedRepeatOnNewPageSplitMember(PageContext pageContext, HeaderFooterRow headerFooterRow)
		{
			TablixMember tablixMember = headerFooterRow.StructMemberCell.TablixMember;
			this.TraceMember(pageContext, tablixMember, RenderingArea.RepeatOnNewPage, TraceLevel.Info, "PR-DIAG [Page {0}] '{1}' not repeated because child member spans pages");
		}

		private bool GetFlagValue(TablixState flag)
		{
			return (this.m_tablixStateFlags & flag) > (TablixState)0;
		}

		private void SetFlagValue(TablixState flag, bool value)
		{
			if (value)
			{
				this.m_tablixStateFlags |= flag;
			}
			else
			{
				this.m_tablixStateFlags &= (TablixState)(short)(~(int)flag);
			}
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(Tablix.m_declaration);
			IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.TablixState:
					writer.Write((short)this.m_tablixStateFlags);
					break;
				case MemberName.BodyRows:
				{
					int value3 = scalabilityCache.StoreStaticReference(this.m_bodyRows);
					writer.Write(value3);
					break;
				}
				case MemberName.BodyRowHeights:
					writer.Write(this.m_bodyRowsHeights);
					break;
				case MemberName.BodyColWidths:
					writer.Write(this.m_bodyColWidths);
					break;
				case MemberName.RowMembersDepth:
					writer.Write(this.m_rowMembersDepth);
					break;
				case MemberName.ColMembersDepth:
					writer.Write(this.m_colMembersDepth);
					break;
				case MemberName.RowMemberDef:
				{
					int value2 = scalabilityCache.StoreStaticReference(this.m_rowMemberDefList);
					writer.Write(value2);
					break;
				}
				case MemberName.RowMemberDefIndexes:
					writer.WriteStringInt32Hashtable(this.m_rowMemberDefIndexes);
					break;
				case MemberName.ColMemberDef:
				{
					int value = scalabilityCache.StoreStaticReference(this.m_colMemberDefList);
					writer.Write(value);
					break;
				}
				case MemberName.ColMemberDefIndexes:
					writer.WriteStringInt32Hashtable(this.m_colMemberDefIndexes);
					break;
				case MemberName.RowMemberInstanceIndexes:
					writer.WriteStringInt32Hashtable(this.m_rowMemberInstanceIndexes);
					break;
				case MemberName.MemberAtLevelIndexes:
					writer.WriteStringInt32Hashtable(this.m_memberAtLevelIndexes);
					break;
				case MemberName.CellPageBreaks:
					writer.Write(this.m_ignoreCellPageBreaks);
					break;
				case MemberName.HeaderRowCols:
					writer.Write(this.m_headerRowCols);
					break;
				case MemberName.HeaderColumnRows:
					writer.Write(this.m_headerColumnRows);
					break;
				case MemberName.RowMemberIndexCell:
					writer.Write(this.m_rowMemberIndexCell);
					break;
				case MemberName.ColMemberIndexCell:
					writer.Write(this.m_colMemberIndexCell);
					break;
				case MemberName.ColsBeforeRowHeaders:
					writer.Write(this.m_colsBeforeRowHeaders);
					break;
				case MemberName.ColumnHeaders:
					writer.Write(this.m_columnHeaders);
					break;
				case MemberName.ColumnHeadersHeights:
					writer.Write(this.m_colHeaderHeights);
					break;
				case MemberName.RowHeaders:
					writer.Write(this.m_rowHeaders);
					break;
				case MemberName.RowHeadersWidths:
					writer.Write(this.m_rowHeaderWidths);
					break;
				case MemberName.DetailRows:
					writer.Write(this.m_detailRows);
					break;
				case MemberName.CornerCells:
					writer.Write(this.m_cornerCells);
					break;
				case MemberName.GroupPageBreaks:
					writer.Write(this.m_ignoreGroupPageBreaks);
					break;
				case MemberName.ColumnInfo:
					writer.Write(this.m_columnInfo);
					break;
				case MemberName.IgnoreCol:
					writer.Write(this.m_ignoreCol);
					break;
				case MemberName.IgnoreRow:
					writer.Write(this.m_ignoreRow);
					break;
				default:
					RSTrace.RenderingTracer.Assert(false, string.Empty);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(Tablix.m_declaration);
			IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.TablixState:
					this.m_tablixStateFlags = (TablixState)reader.ReadInt16();
					break;
				case MemberName.BodyRows:
				{
					int id3 = reader.ReadInt32();
					this.m_bodyRows = (TablixRowCollection)scalabilityCache.FetchStaticReference(id3);
					break;
				}
				case MemberName.BodyRowHeights:
					this.m_bodyRowsHeights = reader.ReadDoubleArray();
					break;
				case MemberName.BodyColWidths:
					this.m_bodyColWidths = reader.ReadDoubleArray();
					break;
				case MemberName.RowMembersDepth:
					this.m_rowMembersDepth = reader.ReadInt32();
					break;
				case MemberName.ColMembersDepth:
					this.m_colMembersDepth = reader.ReadInt32();
					break;
				case MemberName.RowMemberDef:
				{
					int id2 = reader.ReadInt32();
					this.m_rowMemberDefList = (List<RPLTablixMemberDef>)scalabilityCache.FetchStaticReference(id2);
					break;
				}
				case MemberName.RowMemberDefIndexes:
					this.m_rowMemberDefIndexes = reader.ReadStringInt32Hashtable<Hashtable>();
					break;
				case MemberName.ColMemberDef:
				{
					int id = reader.ReadInt32();
					this.m_colMemberDefList = (List<RPLTablixMemberDef>)scalabilityCache.FetchStaticReference(id);
					break;
				}
				case MemberName.ColMemberDefIndexes:
					this.m_colMemberDefIndexes = reader.ReadStringInt32Hashtable<Hashtable>();
					break;
				case MemberName.RowMemberInstanceIndexes:
					this.m_rowMemberInstanceIndexes = reader.ReadStringInt32Hashtable<Hashtable>();
					break;
				case MemberName.MemberAtLevelIndexes:
					this.m_memberAtLevelIndexes = reader.ReadStringInt32Hashtable<Hashtable>();
					break;
				case MemberName.CellPageBreaks:
					this.m_ignoreCellPageBreaks = reader.ReadInt32();
					break;
				case MemberName.HeaderRowCols:
					this.m_headerRowCols = reader.ReadInt32();
					break;
				case MemberName.HeaderColumnRows:
					this.m_headerColumnRows = reader.ReadInt32();
					break;
				case MemberName.RowMemberIndexCell:
					this.m_rowMemberIndexCell = reader.ReadInt32();
					break;
				case MemberName.ColMemberIndexCell:
					this.m_colMemberIndexCell = reader.ReadInt32();
					break;
				case MemberName.ColsBeforeRowHeaders:
					this.m_colsBeforeRowHeaders = reader.ReadInt32();
					break;
				case MemberName.ColumnHeaders:
					this.m_columnHeaders = reader.ReadGenericListOfRIFObjects<PageStructMemberCell>();
					break;
				case MemberName.ColumnHeadersHeights:
					this.m_colHeaderHeights = reader.ReadGenericListOfRIFObjects<SizeInfo>();
					break;
				case MemberName.RowHeaders:
					this.m_rowHeaders = reader.ReadGenericListOfRIFObjects<PageStructMemberCell>();
					break;
				case MemberName.RowHeadersWidths:
					this.m_rowHeaderWidths = reader.ReadGenericListOfRIFObjects<SizeInfo>();
					break;
				case MemberName.DetailRows:
					this.m_detailRows = reader.ReadRIFObject<ScalableList<RowInfo>>();
					break;
				case MemberName.CornerCells:
					this.m_cornerCells = reader.Read2DArrayOfRIFObjects<PageCornerCell>();
					break;
				case MemberName.GroupPageBreaks:
					this.m_ignoreGroupPageBreaks = reader.ReadInt32();
					break;
				case MemberName.ColumnInfo:
					this.m_columnInfo = reader.ReadRIFObject<ScalableList<ColumnInfo>>();
					break;
				case MemberName.IgnoreCol:
					this.m_ignoreCol = reader.ReadInt32();
					break;
				case MemberName.IgnoreRow:
					this.m_ignoreRow = reader.ReadInt32();
					break;
				default:
					RSTrace.RenderingTracer.Assert(false, string.Empty);
					break;
				}
			}
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.Tablix;
		}

		internal new static Declaration GetDeclaration()
		{
			if (Tablix.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.TablixState, Token.Int16));
				list.Add(new MemberInfo(MemberName.BodyRows, Token.Int32));
				list.Add(new MemberInfo(MemberName.BodyRowHeights, ObjectType.PrimitiveTypedArray, Token.Double));
				list.Add(new MemberInfo(MemberName.BodyColWidths, ObjectType.PrimitiveTypedArray, Token.Double));
				list.Add(new MemberInfo(MemberName.RowMembersDepth, Token.Int32));
				list.Add(new MemberInfo(MemberName.ColMembersDepth, Token.Int32));
				list.Add(new MemberInfo(MemberName.RowMemberDef, Token.Int32));
				list.Add(new MemberInfo(MemberName.RowMemberDefIndexes, ObjectType.StringInt32Hashtable, Token.Int32));
				list.Add(new MemberInfo(MemberName.ColMemberDef, Token.Int32));
				list.Add(new MemberInfo(MemberName.ColMemberDefIndexes, ObjectType.StringInt32Hashtable, Token.Int32));
				list.Add(new MemberInfo(MemberName.RowMemberInstanceIndexes, ObjectType.StringInt32Hashtable, Token.Int32));
				list.Add(new MemberInfo(MemberName.MemberAtLevelIndexes, ObjectType.StringInt32Hashtable, Token.Int32));
				list.Add(new MemberInfo(MemberName.CellPageBreaks, Token.Int32));
				list.Add(new MemberInfo(MemberName.HeaderRowCols, Token.Int32));
				list.Add(new MemberInfo(MemberName.HeaderColumnRows, Token.Int32));
				list.Add(new MemberInfo(MemberName.RowMemberIndexCell, Token.Int32));
				list.Add(new MemberInfo(MemberName.ColMemberIndexCell, Token.Int32));
				list.Add(new MemberInfo(MemberName.ColsBeforeRowHeaders, Token.Int32));
				list.Add(new MemberInfo(MemberName.ColumnHeaders, ObjectType.RIFObjectList, ObjectType.PageStructMemberCell));
				list.Add(new MemberInfo(MemberName.ColumnHeadersHeights, ObjectType.RIFObjectList, ObjectType.SizeInfo));
				list.Add(new MemberInfo(MemberName.RowHeaders, ObjectType.RIFObjectList, ObjectType.PageStructMemberCell));
				list.Add(new MemberInfo(MemberName.RowHeadersWidths, ObjectType.RIFObjectList, ObjectType.SizeInfo));
				list.Add(new MemberInfo(MemberName.DetailRows, ObjectType.ScalableList, ObjectType.RowInfo));
				list.Add(new MemberInfo(MemberName.CornerCells, ObjectType.Array2D, ObjectType.PageCornerCell));
				list.Add(new MemberInfo(MemberName.GroupPageBreaks, Token.Int32));
				list.Add(new MemberInfo(MemberName.ColumnInfo, ObjectType.ScalableList, ObjectType.ColumnInfo));
				list.Add(new MemberInfo(MemberName.IgnoreCol, Token.Int32));
				list.Add(new MemberInfo(MemberName.IgnoreRow, Token.Int32));
				return new Declaration(ObjectType.Tablix, ObjectType.PageItem, list);
			}
			return Tablix.m_declaration;
		}
	}
}
