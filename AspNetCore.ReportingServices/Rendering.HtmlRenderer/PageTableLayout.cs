using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.HtmlRenderer
{
	[Serializable]
	internal sealed class PageTableLayout
	{
		private const float LINETHRESHOLD = 0.01f;

		public const double RoundDelta = 0.0001;

		private int m_nrCols;

		private int m_nrRows;

		private PageTableCellList m_tableGrid;

		private bool m_bandTable;

		[NonSerialized]
		private int m_firstVisibleRow;

		[NonSerialized]
		private int m_firstVisibleColumn;

		[NonSerialized]
		private bool m_needExtraRow;

		internal bool BandTable
		{
			get
			{
				return this.m_bandTable;
			}
			set
			{
				this.m_bandTable = value;
			}
		}

		internal int NrCols
		{
			get
			{
				return this.m_nrCols;
			}
			set
			{
				this.m_nrCols = value;
			}
		}

		internal int NrRows
		{
			get
			{
				return this.m_nrRows;
			}
			set
			{
				this.m_nrRows = value;
			}
		}

		internal PageTableLayout(int nrCols, int nrRows)
		{
			this.m_nrCols = nrCols;
			this.m_nrRows = nrRows;
			this.m_tableGrid = new PageTableCellList();
		}

		internal void AddCell(float x, float y, float dx, float dy)
		{
			this.m_tableGrid.Add(new PageTableCell(x, y, dx, dy));
		}

		internal PageTableCell GetCell(int index)
		{
			if (index >= 0 && index <= this.m_nrCols * this.m_nrRows - 1)
			{
				return this.m_tableGrid[index];
			}
			return null;
		}

		internal PageTableCell GetCell(int row, int col)
		{
			if (col >= this.m_nrCols)
			{
				return null;
			}
			return this.GetCell(row * this.m_nrCols + col);
		}

		public static void GenerateTableLayout(RPLItemMeasurement[] repItemCollection, float ownerWidth, float ownerHeight, float delta, out PageTableLayout tableLayout)
		{
			PageTableLayout.GenerateTableLayout(repItemCollection, ownerWidth, ownerHeight, delta, out tableLayout, false, false);
		}

		public static void GenerateTableLayout(RPLItemMeasurement[] repItemCollection, float ownerWidth, float ownerHeight, float delta, out PageTableLayout tableLayout, bool expandLayout, bool consumeContainerWhiteSpace)
		{
			List<float> list = new List<float>();
			List<float> list2 = new List<float>();
			int num = 0;
			int num2 = 0;
			tableLayout = null;
			if (repItemCollection != null)
			{
				PageTableLayout.FillXArray(repItemCollection, ownerWidth, list);
				PageTableLayout.FillYArray(repItemCollection, ownerHeight, list2, delta);
				num = list.Count - 1;
				num2 = list2.Count - 1;
				if (num > 0 && num2 > 0)
				{
					tableLayout = new PageTableLayout(num, num2);
					for (int i = 0; i <= num2 - 1; i++)
					{
						for (int j = 0; j <= num - 1; j++)
						{
							tableLayout.AddCell(list[j], list2[i], list[j + 1] - list[j], list2[i + 1] - list2[i]);
						}
					}
					tableLayout.AttachReportItems(repItemCollection, (double)delta, consumeContainerWhiteSpace);
					num = tableLayout.NrCols;
					num2 = tableLayout.NrRows;
					if (num <= 0 || num2 <= 0)
					{
						tableLayout = null;
					}
					else if (num == 1 && num2 == repItemCollection.Length)
					{
						int k;
						for (k = 0; k < num2 && tableLayout.GetCell(k).InUse; k++)
						{
						}
						tableLayout.BandTable = ((byte)((k >= num2) ? 1 : 0) != 0);
					}
				}
			}
		}

		private void RemoveLastRow()
		{
			this.m_tableGrid.RemoveRange((this.m_nrRows - 1) * this.m_nrCols, this.m_nrCols);
			this.m_nrRows--;
		}

		private void RemoveLastCol()
		{
			for (int num = this.m_nrRows; num > 0; num--)
			{
				int num2 = num * this.m_nrCols - 1;
				if (num < this.m_nrRows)
				{
					for (int i = 1; i < this.m_nrCols; i++)
					{
						PageTableCell cell = this.GetCell(num2 + i);
						if (cell != null && (cell.Eaten || cell.InUse))
						{
							cell.UsedCell -= cell.UsedCell / this.m_nrCols;
						}
					}
				}
				this.m_tableGrid.RemoveAt(num2);
			}
			this.m_nrCols--;
		}

		internal bool NeedExtraRow()
		{
			int num = this.m_firstVisibleRow * this.m_nrCols;
			PageTableCell pageTableCell = null;
			for (int i = this.m_firstVisibleColumn; i < this.m_nrCols; i += pageTableCell.ColSpan)
			{
				pageTableCell = this.m_tableGrid[i + num];
				if (pageTableCell.ColSpan != 1 && (pageTableCell.InUse || pageTableCell.RowSpan > 1 || pageTableCell.HasBorder))
				{
					this.m_needExtraRow = true;
					break;
				}
			}
			if (this.m_needExtraRow)
			{
				for (int j = 0; j < this.m_nrCols; j++)
				{
					if (!this.m_tableGrid[j].Eaten && !this.m_tableGrid[j].InUse && this.m_tableGrid[j].BorderLeft == null)
					{
						pageTableCell = this.m_tableGrid[j];
						for (; j + 1 < this.m_nrCols && !this.m_tableGrid[j + 1].Eaten && !this.m_tableGrid[j + 1].InUse && this.m_tableGrid[j + 1].BorderTop == this.m_tableGrid[j + 1].BorderTop && this.m_tableGrid[j + 1].BorderBottom == this.m_tableGrid[j + 1].BorderBottom && this.m_tableGrid[j + 1].BorderLeft == null; j++)
						{
							pageTableCell.ColSpan++;
							this.m_tableGrid[j + 1].Eaten = true;
						}
					}
				}
			}
			return this.m_needExtraRow;
		}

		internal static bool SkipReportItem(RPLItemMeasurement measurement)
		{
			if (measurement.Height == 0.0)
			{
				if (measurement.Width == 0.0)
				{
					return true;
				}
				if (!(measurement.Element is RPLLine))
				{
					return true;
				}
			}
			else if (measurement.Width == 0.0 && !(measurement.Element is RPLLine))
			{
				return true;
			}
			return false;
		}

		private static void FillXArray(RPLItemMeasurement[] reportItemCol, float parentWidth, List<float> leftPosition)
		{
			int num = 0;
			int num2 = 0;
			RoundedFloat roundedFloat = new RoundedFloat(0f);
			RPLItemMeasurement rPLItemMeasurement = null;
			int num3 = 0;
			if (reportItemCol != null && leftPosition != null)
			{
				leftPosition.Add(roundedFloat.Value);
				while (num3 < reportItemCol.Length)
				{
					rPLItemMeasurement = reportItemCol[num3];
					if (PageTableLayout.SkipReportItem(rPLItemMeasurement))
					{
						num3++;
					}
					else
					{
						RoundedFloat x = new RoundedFloat(rPLItemMeasurement.Left);
						while (num > 0 && x < leftPosition[num])
						{
							num--;
						}
						if (num < 0 || x != leftPosition[num])
						{
							leftPosition.Insert(num + 1, rPLItemMeasurement.Left);
							num2++;
						}
						num = num2;
						roundedFloat.Value = PageTableLayout.ReportItemRightValue(rPLItemMeasurement);
						while (num > 0 && roundedFloat < leftPosition[num])
						{
							num--;
						}
						if (num < 0 || roundedFloat != leftPosition[num])
						{
							leftPosition.Insert(num + 1, roundedFloat.Value);
							num2++;
						}
						num = num2;
						num3++;
					}
				}
				RoundedFloat x2 = new RoundedFloat(parentWidth);
				if (!(x2 > leftPosition[num]) && 1 != leftPosition.Count)
				{
					return;
				}
				leftPosition.Insert(num + 1, parentWidth);
			}
		}

		private static void FillYArray(RPLItemMeasurement[] repItemColl, float parentHeight, List<float> topPosition, float delta)
		{
			int num = 0;
			int num2 = 0;
			RoundedFloat roundedFloat = new RoundedFloat(0f);
			int num3 = 0;
			RPLItemMeasurement rPLItemMeasurement = null;
			if (repItemColl != null && topPosition != null)
			{
				topPosition.Add(roundedFloat.Value);
				while (num3 < repItemColl.Length)
				{
					rPLItemMeasurement = repItemColl[num3];
					if (PageTableLayout.SkipReportItem(rPLItemMeasurement))
					{
						num3++;
					}
					else
					{
						RoundedFloat roundedFloat2 = new RoundedFloat(rPLItemMeasurement.Top - delta);
						while (num > 0 && roundedFloat2 < topPosition[num])
						{
							num--;
						}
						if (num < 0 || roundedFloat2 != topPosition[num])
						{
							topPosition.Insert(num + 1, roundedFloat2.Value - delta);
							num2++;
						}
						num = num2;
						roundedFloat.Value = PageTableLayout.ReportItemBottomValue(rPLItemMeasurement) - delta;
						while (num > 0 && roundedFloat < topPosition[num])
						{
							num--;
						}
						if (num < 0 || roundedFloat != topPosition[num])
						{
							topPosition.Insert(num + 1, roundedFloat.Value);
							num2++;
						}
						num = num2;
						num3++;
					}
				}
				RoundedFloat x = new RoundedFloat(parentHeight - delta);
				if (!(x > topPosition[num]) && 1 != topPosition.Count)
				{
					return;
				}
				topPosition.Insert(num + 1, parentHeight - delta);
			}
		}

		private static float ReportItemRightValue(RPLMeasurement currReportItem)
		{
			return currReportItem.Left + currReportItem.Width;
		}

		private static float ReportItemBottomValue(RPLMeasurement currReportItem)
		{
			return currReportItem.Top + currReportItem.Height;
		}

		internal bool AreSpansInColOne()
		{
			bool flag = false;
			for (int i = 0; i < this.m_nrRows; i++)
			{
				int index = i * this.m_nrCols;
				if (!this.m_tableGrid[index].Eaten && !this.m_tableGrid[index].InUse)
				{
					if (flag)
					{
						return true;
					}
					flag = true;
				}
				else
				{
					if (this.m_tableGrid[index].InUse && this.m_tableGrid[index].RowSpan > 1)
					{
						return true;
					}
					flag = false;
				}
			}
			return false;
		}

		private void AttachVerticalBorder(int xCellFound, int yCellFound, RPLMeasurement measurement, RPLLine currReportItem, bool leftBorder)
		{
			int num = yCellFound;
			PageTableCell pageTableCell = this.m_tableGrid[xCellFound + num * this.m_nrCols];
			double num2 = (double)pageTableCell.DYValue.Value;
			if (leftBorder)
			{
				pageTableCell.BorderLeft = currReportItem;
			}
			else
			{
				pageTableCell.BorderRight = currReportItem;
			}
			while ((double)measurement.Height - num2 > 0.0001)
			{
				num++;
				if (num >= this.m_nrRows)
				{
					break;
				}
				pageTableCell = this.m_tableGrid[xCellFound + num * this.m_nrCols];
				num2 += (double)pageTableCell.DYValue.Value;
				if (leftBorder)
				{
					pageTableCell.BorderLeft = currReportItem;
				}
				else
				{
					pageTableCell.BorderRight = currReportItem;
				}
			}
		}

		private void AttachHorizontalBorder(int xCellFound, int yCellFound, RPLMeasurement measurement, RPLLine currReportItem, bool topBorder)
		{
			double num = 0.0;
			int num2 = 0;
			int num3 = 0;
			PageTableCell pageTableCell = null;
			int num4 = 1;
			num2 = xCellFound;
			num3 = yCellFound * this.m_nrCols;
			pageTableCell = this.m_tableGrid[num2 + num3];
			num = (double)pageTableCell.DXValue.Value;
			if (topBorder)
			{
				pageTableCell.BorderTop = currReportItem;
			}
			else
			{
				pageTableCell.BorderBottom = currReportItem;
			}
			while ((double)measurement.Width - num > 0.0001)
			{
				num2++;
				if (num2 >= this.m_nrCols)
				{
					break;
				}
				pageTableCell = this.m_tableGrid[num2 + num3];
				num += (double)pageTableCell.DXValue.Value;
				num4++;
				if (topBorder)
				{
					pageTableCell.BorderTop = currReportItem;
				}
				else
				{
					pageTableCell.BorderBottom = currReportItem;
				}
			}
		}

		private bool FindRItemCell(RPLMeasurement currReportItem, ref int xCellFound, ref int yCellFound, double delta)
		{
			bool flag = false;
			int num = 0;
			int num2 = 0;
			while (!flag && num < this.m_nrRows)
			{
				if (this.m_tableGrid[this.m_nrCols * num].YValue == (float)((double)currReportItem.Top - delta))
				{
					num2 = 0;
					while (num2 <= this.m_nrCols - 1)
					{
						if (!(this.m_tableGrid[num2 + this.m_nrCols * num].XValue == currReportItem.Left))
						{
							num2++;
							continue;
						}
						flag = true;
						xCellFound = num2;
						yCellFound = num;
						break;
					}
				}
				num++;
			}
			return flag;
		}

		private void AttachRItem(int xCellFound, int yCellFound, int colSpan, int rowSpan, RPLItemMeasurement measurement)
		{
			int num = 0;
			int num2 = 0;
			int index = xCellFound + yCellFound * this.m_nrCols;
			for (num = xCellFound; num <= xCellFound + colSpan - 1; num++)
			{
				for (num2 = yCellFound; num2 <= yCellFound + rowSpan - 1; num2++)
				{
					if (num != xCellFound || num2 != yCellFound)
					{
						this.m_tableGrid[num + this.m_nrCols * num2].MarkCellEaten(index);
					}
				}
			}
			this.m_tableGrid[index].MarkCellUsed(measurement, colSpan, rowSpan, index);
		}

		private void ComputeColRowSpan(RPLMeasurement reportItem, int xCellFound, int yCellFound, ref int colSpans, ref int rowSpans)
		{
			float num = 0f;
			float num2 = 0f;
			int num3 = 0;
			int num4 = 0;
			int num5 = yCellFound * this.m_nrCols;
			int index = xCellFound + num5;
			num = this.m_tableGrid[index].DXValue.Value;
			num3 = xCellFound;
			colSpans = 1;
			for (; (double)(reportItem.Width - num) > 0.0001; num += this.m_tableGrid[num3 + num5].DXValue.Value)
			{
				num3++;
				if (num3 >= this.m_nrCols)
				{
					break;
				}
				colSpans++;
			}
			num2 = this.m_tableGrid[index].DYValue.Value;
			num4 = yCellFound;
			rowSpans = 1;
			for (; (double)(reportItem.Height - num2) > 0.0001; num2 += this.m_tableGrid[xCellFound + this.m_nrCols * num4].DYValue.Value)
			{
				num4++;
				if (num4 >= this.m_nrRows)
				{
					break;
				}
				rowSpans++;
			}
			for (int i = yCellFound; i < yCellFound + rowSpans; i++)
			{
				for (int j = xCellFound; j < xCellFound + colSpans; j++)
				{
					PageTableCell cell = this.GetCell(i, j);
					cell.FirstHorzMerge = (j == xCellFound && colSpans > 1);
					cell.FirstVertMerge = (i == yCellFound && rowSpans > 1);
					cell.HorzMerge = (j > xCellFound);
					cell.VertMerge = (i > yCellFound);
				}
			}
		}

		private void FillAndFindOverlap(RPLItemMeasurement[] repItemCollection, double delta)
		{
			bool flag = false;
			int num = -1;
			int num2 = -1;
			int colSpan = 0;
			int rowSpan = 0;
			int num3 = 0;
			PageTableCell pageTableCell = null;
			int num4 = 0;
			RPLItemMeasurement rPLItemMeasurement = null;
			RPLElement rPLElement = null;
			while (num4 < repItemCollection.Length)
			{
				num = -1;
				num2 = -1;
				flag = false;
				rPLItemMeasurement = repItemCollection[num4];
				if (PageTableLayout.SkipReportItem(rPLItemMeasurement))
				{
					num4++;
				}
				else
				{
					rPLElement = rPLItemMeasurement.Element;
					flag = this.FindRItemCell(rPLItemMeasurement, ref num, ref num2, delta);
					if (!flag && !(rPLElement is RPLLine))
					{
						num4++;
					}
					else
					{
						RPLLine rPLLine = rPLElement as RPLLine;
						if (rPLLine != null)
						{
							RPLElementPropsDef elementPropsDef = rPLLine.ElementPropsDef;
							float width = rPLItemMeasurement.Width;
							if (width >= 0.0 && width < 0.0099999997764825821)
							{
								goto IL_00b8;
							}
							if (width < 0.0 && width > -0.0099999997764825821)
							{
								goto IL_00b8;
							}
							width = rPLItemMeasurement.Height;
							if (width >= 0.0 && width < 0.0099999997764825821)
							{
								goto IL_0176;
							}
							if (width < 0.0 && width > -0.0099999997764825821)
							{
								goto IL_0176;
							}
						}
						num3 = num + this.m_nrCols * num2;
						pageTableCell = this.m_tableGrid[num3];
						if ((pageTableCell.InUse || pageTableCell.Eaten) && rPLElement is RPLLine && rPLItemMeasurement.Width != 0.0 && rPLItemMeasurement.Height != 0.0)
						{
							num4++;
							continue;
						}
						this.ComputeColRowSpan(rPLItemMeasurement, num, num2, ref colSpan, ref rowSpan);
						this.AttachRItem(num, num2, colSpan, rowSpan, rPLItemMeasurement);
						num4++;
					}
				}
				continue;
				IL_00b8:
				if (!flag)
				{
					int num5 = 0;
					bool flag2 = true;
					while (flag2 && num5 < this.m_nrRows)
					{
						if (this.m_tableGrid[num5 * this.m_nrCols].YValue == (float)((double)rPLItemMeasurement.Top - delta))
						{
							num2 = num5;
							flag2 = false;
						}
						num5++;
					}
					num = this.m_nrCols - 1;
					if (!flag2)
					{
						this.AttachVerticalBorder(num, num2, rPLItemMeasurement, (RPLLine)rPLElement, false);
					}
				}
				else
				{
					this.AttachVerticalBorder(num, num2, rPLItemMeasurement, (RPLLine)rPLElement, true);
				}
				num4++;
				continue;
				IL_0176:
				if (!flag)
				{
					int num6 = 0;
					bool flag3 = true;
					while (flag3 && num6 < this.m_nrCols)
					{
						if (this.m_tableGrid[num6].XValue == rPLItemMeasurement.Left)
						{
							num = num6;
							flag3 = false;
						}
						num6++;
					}
					num2 = this.m_nrRows - 1;
					this.AttachHorizontalBorder(num, num2, rPLItemMeasurement, (RPLLine)rPLElement, false);
				}
				else
				{
					this.AttachHorizontalBorder(num, num2, rPLItemMeasurement, (RPLLine)rPLElement, true);
				}
				num4++;
			}
		}

		internal bool GetBool(object b)
		{
			if (b != null)
			{
				return (bool)b;
			}
			return false;
		}

		internal void EmptyRowsCells()
		{
			int num = 0;
			int num2 = 0;
			bool flag = false;
			int num3 = 0;
			PageTableCell pageTableCell = null;
			int num4 = 0;
			int num5 = 0;
			List<int> list = new List<int>();
			int num6 = 0;
			int num7 = 0;
			int num8 = 0;
			for (num = 0; num < this.m_nrRows; num++)
			{
				flag = false;
				int num9 = this.m_nrCols * num4;
				if (this.m_tableGrid[num3].DYValue < 0.2f)
				{
					for (num2 = 0; num2 < this.m_nrCols; num2++)
					{
						if (flag)
						{
							break;
						}
						pageTableCell = this.m_tableGrid[num3 + num2];
						flag = (pageTableCell.InUse || pageTableCell.BorderBottom != null || pageTableCell.BorderTop != null);
					}
					if (!flag)
					{
						num4++;
						num5 = num3;
						num2 = 0;
						while (num2 < this.m_nrCols)
						{
							if (this.m_tableGrid[num3 + num2].Eaten)
							{
								int usedCell = this.m_tableGrid[num3 + num2].UsedCell;
								this.m_tableGrid[usedCell].RowSpan--;
								num2 += this.m_tableGrid[usedCell].ColSpan;
							}
							else
							{
								num2++;
							}
						}
						this.m_tableGrid.RemoveRange(num3, this.m_nrCols);
						this.m_nrRows--;
						num--;
						num3 -= this.m_nrCols;
					}
				}
				else
				{
					flag = true;
				}
				if (flag && num4 > 0)
				{
					for (num2 = 0; num2 < this.m_nrCols; num2++)
					{
						pageTableCell = this.m_tableGrid[num3 + num2];
						if (pageTableCell.InUse && pageTableCell.UsedCell >= num5)
						{
							num6 = pageTableCell.RowSpan;
							num7 = pageTableCell.ColSpan;
							int num10 = 0;
							for (int i = 0; i < num6; i++)
							{
								num10 = num3 + num2 + i * this.m_nrCols;
								for (int j = 0; j < num7; j++)
								{
									pageTableCell = this.m_tableGrid[num10 + j];
									pageTableCell.UsedCell -= num9;
								}
							}
						}
					}
				}
				num3 += this.m_nrCols;
			}
			if (this.m_nrRows > 0)
			{
				for (num2 = 0; num2 < this.m_nrCols; num2++)
				{
					flag = false;
					if (this.m_tableGrid[num2].DXValue < 0.2f)
					{
						num = 0;
						num3 = 0;
						while (num < this.m_nrRows && !flag)
						{
							pageTableCell = this.m_tableGrid[num3 + num2];
							flag = (pageTableCell.InUse || pageTableCell.BorderLeft != null || pageTableCell.BorderRight != null);
							num++;
							num3 += this.m_nrCols;
						}
						if (!flag)
						{
							list.Add(num2);
							num = 0;
							while (num < this.m_nrRows)
							{
								if (this.m_tableGrid[num * this.m_nrCols + num2].Eaten)
								{
									int usedCell2 = this.m_tableGrid[num * this.m_nrCols + num2].UsedCell;
									this.m_tableGrid[usedCell2].ColSpan--;
									num += this.m_tableGrid[usedCell2].RowSpan;
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
			for (int k = 0; k < list.Count; k++)
			{
				for (int l = 0; l < this.m_nrRows; l++)
				{
					num8 = l * this.m_nrCols + list[k] - l - k;
					this.m_tableGrid.RemoveAt(num8);
					for (int m = 0; m < this.m_nrCols - 1; m++)
					{
						if (num8 + m < this.m_tableGrid.Count)
						{
							PageTableCell cell = this.GetCell(num8 + m);
							if (cell != null && (cell.Eaten || cell.InUse))
							{
								int usedCell3 = cell.UsedCell;
								cell.UsedCell -= usedCell3 / this.m_nrCols;
								if (list[k] < usedCell3 % this.m_nrCols)
								{
									cell.UsedCell--;
								}
							}
						}
					}
				}
				this.m_nrCols--;
			}
		}

		internal void AttachReportItems(RPLItemMeasurement[] reportItemCol, double delta, bool consumeContainerWhiteSpace)
		{
			this.FillAndFindOverlap(reportItemCol, delta);
			this.EmptyRowsCells();
			if (consumeContainerWhiteSpace)
			{
				float lastRowHeight = 0f;
				float lastColWidth = 0f;
				if (this.m_nrRows > 1)
				{
					while (!this.HasEmptyBottomRows(ref lastRowHeight))
					{
						this.RemoveConsumeContainerWhiteSpaceRows(lastRowHeight);
					}
				}
				if (this.m_nrCols > 1)
				{
					while (!this.HasEmptyRightColumns(ref lastColWidth))
					{
						this.RemoveConsumeContainerWhiteSpaceColumns(lastColWidth);
					}
				}
			}
		}

		private bool HasEmptyBottomRows(ref float lastRowHeight)
		{
			bool result = false;
			int row = this.m_nrRows - 1;
			lastRowHeight = 0f;
			for (int i = 0; i < this.m_nrCols; i++)
			{
				PageTableCell cell = this.GetCell(row, i);
				if (cell.Eaten && !this.m_tableGrid[cell.UsedCell].InUse)
				{
					continue;
				}
				if (!cell.InUse && !cell.HasBorder && !cell.Eaten)
				{
					lastRowHeight = Math.Max(cell.DYValue.Value, lastRowHeight);
					continue;
				}
				result = true;
				break;
			}
			return result;
		}

		private bool HasEmptyRightColumns(ref float lastColWidth)
		{
			bool result = false;
			int col = this.m_nrCols - 1;
			lastColWidth = 0f;
			for (int i = 0; i < this.m_nrRows; i++)
			{
				PageTableCell cell = this.GetCell(i, col);
				if (cell.Eaten && !this.m_tableGrid[cell.UsedCell].InUse)
				{
					continue;
				}
				if (!cell.InUse && !cell.HasBorder && !cell.Eaten)
				{
					lastColWidth = Math.Max(cell.DXValue.Value, lastColWidth);
					continue;
				}
				result = true;
				break;
			}
			return result;
		}

		private void RemoveConsumeContainerWhiteSpaceRows(float lastRowHeight)
		{
			int num = this.m_nrRows - 2;
			for (int i = 0; i < this.m_nrCols; i++)
			{
				PageTableCell cell = this.GetCell(num, i);
				int num2 = 1;
				while (cell == null)
				{
					cell = this.GetCell(num - num2, i);
					if (cell == null)
					{
						num2++;
						if (num2 > num)
						{
							break;
						}
					}
				}
				if (cell != null)
				{
					cell.ConsumedByEmptyWhiteSpace = true;
					cell.DYValue.Value += lastRowHeight;
					cell.KeepBottomBorder = true;
				}
			}
			this.RemoveLastRow();
		}

		private void RemoveConsumeContainerWhiteSpaceColumns(float lastColWidth)
		{
			int num = this.m_nrCols - 2;
			for (int i = 0; i < this.m_nrRows; i++)
			{
				PageTableCell cell = this.GetCell(i, num);
				int num2 = 1;
				while (cell == null)
				{
					cell = this.GetCell(i, num - num2);
					if (cell == null)
					{
						num2++;
						if (num2 > num)
						{
							break;
						}
					}
				}
				if (cell != null)
				{
					cell.ConsumedByEmptyWhiteSpace = true;
					cell.DXValue.Value += lastColWidth;
					cell.KeepRightBorder = true;
				}
			}
			this.RemoveLastCol();
		}

		internal bool EmptyRow(RPLMeasurement[] repItemColl, bool ignoreLines, int rowIndex, bool renderHeight, ref int skipHeight)
		{
			int i = this.m_firstVisibleColumn;
			bool result = true;
			PageTableCell pageTableCell = null;
			for (; i < this.NrCols; i++)
			{
				pageTableCell = this.GetCell(i + rowIndex);
				if (pageTableCell.InUse)
				{
					RPLMeasurement measurement = pageTableCell.Measurement;
					if (measurement == null && ignoreLines)
					{
						continue;
					}
					result = false;
					if (!renderHeight && skipHeight < pageTableCell.RowSpan)
					{
						skipHeight = pageTableCell.RowSpan;
					}
					break;
				}
			}
			return result;
		}
	}
}
