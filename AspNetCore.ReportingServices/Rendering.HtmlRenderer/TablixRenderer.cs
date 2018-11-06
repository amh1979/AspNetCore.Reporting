using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Web;

namespace AspNetCore.ReportingServices.Rendering.HtmlRenderer
{
	internal class TablixRenderer : IReportItemRenderer
	{
		private const string FixedRowMarker = "r";

		private const string FixedColMarker = "c";

		private const string EmptyColMarker = "e";

		private const string EmptyHeightColMarker = "h";

		private HTML5Renderer html5Renderer;

		public TablixRenderer(HTML5Renderer renderer)
		{
			this.html5Renderer = renderer;
		}

		public void RenderReportItem(RPLElement reportItem, RPLItemMeasurement measurement, StyleContext styleContext, ref int borderContext, bool renderId, bool treatAsTopLevel = false)
		{
			RPLTablix rPLTablix = reportItem as RPLTablix;
			RPLElementProps elementProps = rPLTablix.ElementProps;
			RPLElementPropsDef definition = elementProps.Definition;
			string uniqueName = elementProps.UniqueName;
			TablixFixedHeaderStorage tablixFixedHeaderStorage = new TablixFixedHeaderStorage();
			if (rPLTablix.ColumnWidths == null)
			{
				rPLTablix.ColumnWidths = new float[0];
			}
			if (rPLTablix.RowHeights == null)
			{
				rPLTablix.RowHeights = new float[0];
			}
			bool flag = this.InitFixedColumnHeaders(rPLTablix, uniqueName, tablixFixedHeaderStorage);
			bool flag2 = this.InitFixedRowHeaders(rPLTablix, uniqueName, tablixFixedHeaderStorage);
			bool flag3 = rPLTablix.ColumnHeaderRows == 0 && rPLTablix.RowHeaderColumns == 0 && !this.html5Renderer.m_deviceInfo.AccessibleTablix;
			if (treatAsTopLevel)
			{
				this.html5Renderer.WriteStream(HTMLElements.m_openSpan);
				this.html5Renderer.WriteAccesibilityTags(RenderRes.AccessibleTableBoxLabel, elementProps, treatAsTopLevel);
				this.html5Renderer.WriteStream(HTMLElements.m_closeBracket);
				this.html5Renderer.WriteStream(HTMLElements.m_closeSpan);
			}
			if (flag && flag2)
			{
				tablixFixedHeaderStorage.CornerHeaders = new List<string>();
			}
			this.html5Renderer.WriteStream(HTMLElements.m_openTable);
			int columns = (rPLTablix.ColumnHeaderRows > 0 || rPLTablix.RowHeaderColumns > 0 || !flag3) ? (rPLTablix.ColumnWidths.Length + 1) : rPLTablix.ColumnWidths.Length;
			this.html5Renderer.WriteStream(HTMLElements.m_cols);
			this.html5Renderer.WriteStream(columns.ToString(CultureInfo.InvariantCulture));
			this.html5Renderer.WriteStream(HTMLElements.m_quote);
			if (renderId || flag || flag2)
			{
				this.html5Renderer.RenderReportItemId(uniqueName);
			}
			this.html5Renderer.WriteToolTip(rPLTablix.ElementProps);
			this.html5Renderer.WriteStream(HTMLElements.m_zeroBorder);
			this.html5Renderer.OpenStyle();
			this.html5Renderer.WriteStream(HTMLElements.m_borderCollapse);
			this.html5Renderer.WriteStream(HTMLElements.m_semiColon);
			if (this.html5Renderer.m_deviceInfo.OutlookCompat && measurement != null)
			{
				this.html5Renderer.RenderMeasurementWidth(measurement.Width, true);
			}
			this.html5Renderer.RenderReportItemStyle((RPLElement)rPLTablix, elementProps, definition, measurement, styleContext, ref borderContext, definition.ID);
			this.html5Renderer.CloseStyle(true);
			this.html5Renderer.WriteStream(HTMLElements.m_closeBracket);
			int colsBeforeRowHeaders = rPLTablix.ColsBeforeRowHeaders;
			RPLTablixRow nextRow = rPLTablix.GetNextRow();
			List<RPLTablixOmittedRow> list = new List<RPLTablixOmittedRow>();
			while (nextRow != null && nextRow is RPLTablixOmittedRow)
			{
				list.Add((RPLTablixOmittedRow)nextRow);
				nextRow = rPLTablix.GetNextRow();
			}
			if (flag3)
			{
				this.RenderEmptyTopTablixRow(rPLTablix, list, uniqueName, false, tablixFixedHeaderStorage);
				this.RenderSimpleTablixRows(rPLTablix, uniqueName, nextRow, borderContext, tablixFixedHeaderStorage);
			}
			else
			{
				styleContext = new StyleContext();
				float[] columnWidths = rPLTablix.ColumnWidths;
				float[] rowHeights = rPLTablix.RowHeights;
				int num = columnWidths.Length;
				int numRows = rowHeights.Length;
				this.RenderEmptyTopTablixRow(rPLTablix, list, uniqueName, true, tablixFixedHeaderStorage);
				bool flag4 = flag;
				int num2 = 0;
				list = new List<RPLTablixOmittedRow>();
				HTMLHeader[] array = null;
				string[] array2 = null;
				OmittedHeaderStack omittedHeaders = null;
				if (this.html5Renderer.m_deviceInfo.AccessibleTablix)
				{
					array = new HTMLHeader[rPLTablix.RowHeaderColumns];
					array2 = new string[num];
					omittedHeaders = new OmittedHeaderStack();
				}
				while (nextRow != null)
				{
					if (nextRow is RPLTablixOmittedRow)
					{
						list.Add((RPLTablixOmittedRow)nextRow);
						nextRow = rPLTablix.GetNextRow();
						continue;
					}
					if (rowHeights[num2] == 0.0 && num2 > 1 && nextRow.NumCells == 1 && nextRow[0].Element is RPLRectangle)
					{
						RPLRectangle rPLRectangle = (RPLRectangle)nextRow[0].Element;
						if (rPLRectangle.Children != null && rPLRectangle.Children.Length != 0)
						{
							goto IL_0385;
						}
						nextRow = rPLTablix.GetNextRow();
						num2++;
						continue;
					}
					goto IL_0385;
					IL_0385:
					this.html5Renderer.WriteStream(HTMLElements.m_openTR);
					if (rPLTablix.FixedRow(num2) || flag2 || flag4)
					{
						string text = uniqueName + "r" + num2;
						this.html5Renderer.RenderReportItemId(text);
						if (rPLTablix.FixedRow(num2))
						{
							tablixFixedHeaderStorage.ColumnHeaders.Add(text);
							if (tablixFixedHeaderStorage.CornerHeaders != null)
							{
								tablixFixedHeaderStorage.CornerHeaders.Add(text);
							}
						}
						else if (flag4)
						{
							tablixFixedHeaderStorage.BodyID = text;
							flag4 = false;
						}
						if (flag2)
						{
							tablixFixedHeaderStorage.RowHeaders.Add(text);
						}
					}
					this.html5Renderer.WriteStream(HTMLElements.m_valign);
					this.html5Renderer.WriteStream(HTMLElements.m_topValue);
					this.html5Renderer.WriteStream(HTMLElements.m_quote);
					this.html5Renderer.WriteStream(HTMLElements.m_closeBracket);
					this.RenderEmptyHeightCell(rowHeights[num2], uniqueName, rPLTablix.FixedRow(num2), num2, tablixFixedHeaderStorage);
					int num3 = 0;
					int numCells = nextRow.NumCells;
					int num4 = numCells;
					if (nextRow.BodyStart == -1)
					{
						int[] omittedIndices = new int[list.Count];
						for (int i = num3; i < num4; i++)
						{
							RPLTablixCell rPLTablixCell = nextRow[i];
							this.RenderColumnHeaderTablixCell(rPLTablix, uniqueName, num, rPLTablixCell.ColIndex, rPLTablixCell.ColSpan, num2, borderContext, rPLTablixCell, styleContext, tablixFixedHeaderStorage, list, omittedIndices);
							if (array2 != null && num2 < rPLTablix.ColumnHeaderRows)
							{
								string text2 = null;
								if (rPLTablixCell is RPLTablixMemberCell)
								{
									text2 = ((RPLTablixMemberCell)rPLTablixCell).UniqueName;
									if (text2 == null && rPLTablixCell.Element != null)
									{
										text2 = rPLTablixCell.Element.ElementProps.UniqueName;
										((RPLTablixMemberCell)rPLTablixCell).UniqueName = text2;
									}
									if (text2 != null)
									{
										for (int j = 0; j < rPLTablixCell.ColSpan; j++)
										{
											string text3 = array2[rPLTablixCell.ColIndex + j];
											text3 = ((text3 != null) ? (text3 + " " + HttpUtility.HtmlAttributeEncode(this.html5Renderer.m_deviceInfo.HtmlPrefixId) + text2) : (HttpUtility.HtmlAttributeEncode(this.html5Renderer.m_deviceInfo.HtmlPrefixId) + text2));
											array2[rPLTablixCell.ColIndex + j] = text3;
										}
										goto IL_05bb;
									}
									continue;
								}
							}
							goto IL_05bb;
							IL_05bb:
							nextRow[i] = null;
						}
						list = new List<RPLTablixOmittedRow>();
					}
					else
					{
						if (array != null)
						{
							int headerStart = nextRow.HeaderStart;
							int num5 = 0;
							for (int k = 0; k < array.Length; k++)
							{
								HTMLHeader hTMLHeader = array[k];
								if (array[k] != null)
								{
									if (array[k].ID != null)
									{
										RPLTablixCell rPLTablixCell2 = nextRow[num5 + headerStart];
										hTMLHeader.ID = this.CalculateRowHeaderId(rPLTablixCell2, rPLTablix.FixedColumns[rPLTablixCell2.ColIndex], uniqueName, num2, k + rPLTablix.ColsBeforeRowHeaders, null, this.html5Renderer.m_deviceInfo.AccessibleTablix, false);
										hTMLHeader.Span = rPLTablixCell2.RowSpan;
										num5++;
									}
									if (array[k].Span > 1)
									{
										array[k].Span--;
									}
								}
								else
								{
									hTMLHeader = (array[k] = new HTMLHeader());
								}
							}
						}
						if (list != null && list.Count > 0)
						{
							for (int l = 0; l < list.Count; l++)
							{
								this.RenderTablixOmittedRow(columns, list[l]);
							}
							list = null;
						}
						List<RPLTablixMemberCell> omittedHeaders2 = nextRow.OmittedHeaders;
						if (colsBeforeRowHeaders > 0)
						{
							int num6 = 0;
							int headerStart2 = nextRow.HeaderStart;
							int bodyStart = nextRow.BodyStart;
							int m = headerStart2;
							int n = bodyStart;
							int num7 = 0;
							for (; n < num4; n++)
							{
								if (num7 >= colsBeforeRowHeaders)
								{
									break;
								}
								RPLTablixCell rPLTablixCell3 = nextRow[n];
								int colSpan = rPLTablixCell3.ColSpan;
								this.RenderTablixCell(rPLTablix, false, uniqueName, num, numRows, num7, colSpan, num2, borderContext, rPLTablixCell3, omittedHeaders2, ref num6, styleContext, tablixFixedHeaderStorage, array, array2, omittedHeaders);
								num7 += colSpan;
								nextRow[n] = null;
							}
							num4 = ((bodyStart > headerStart2) ? bodyStart : num4);
							if (m >= 0)
							{
								for (; m < num4; m++)
								{
									RPLTablixCell rPLTablixCell4 = nextRow[m];
									int colSpan2 = rPLTablixCell4.ColSpan;
									this.RenderTablixCell(rPLTablix, flag2, uniqueName, num, numRows, num7, colSpan2, num2, borderContext, rPLTablixCell4, omittedHeaders2, ref num6, styleContext, tablixFixedHeaderStorage, array, array2, omittedHeaders);
									num7 += colSpan2;
									nextRow[m] = null;
								}
							}
							num3 = n;
							num4 = ((bodyStart < headerStart2) ? headerStart2 : numCells);
							for (int num8 = num3; num8 < num4; num8++)
							{
								RPLTablixCell rPLTablixCell5 = nextRow[num8];
								this.RenderTablixCell(rPLTablix, false, uniqueName, num, numRows, rPLTablixCell5.ColIndex, rPLTablixCell5.ColSpan, num2, borderContext, rPLTablixCell5, omittedHeaders2, ref num6, styleContext, tablixFixedHeaderStorage, array, array2, omittedHeaders);
								nextRow[num8] = null;
							}
						}
						else
						{
							int num9 = 0;
							for (int num10 = num3; num10 < num4; num10++)
							{
								RPLTablixCell rPLTablixCell6 = nextRow[num10];
								int colIndex = rPLTablixCell6.ColIndex;
								this.RenderTablixCell(rPLTablix, rPLTablix.FixedColumns[colIndex], uniqueName, num, numRows, colIndex, rPLTablixCell6.ColSpan, num2, borderContext, rPLTablixCell6, omittedHeaders2, ref num9, styleContext, tablixFixedHeaderStorage, array, array2, omittedHeaders);
								nextRow[num10] = null;
							}
						}
					}
					this.html5Renderer.WriteStream(HTMLElements.m_closeTR);
					nextRow = rPLTablix.GetNextRow();
					num2++;
				}
			}
			this.html5Renderer.WriteStream(HTMLElements.m_closeTable);
			if (!flag && !flag2)
			{
				return;
			}
			if (this.html5Renderer.m_fixedHeaders == null)
			{
				this.html5Renderer.m_fixedHeaders = new ArrayList();
			}
			this.html5Renderer.m_fixedHeaders.Add(tablixFixedHeaderStorage);
		}

		private void RenderTablixOmittedRow(int columns, RPLTablixRow currentRow)
		{
			int i = 0;
			List<RPLTablixMemberCell> omittedHeaders;
			for (omittedHeaders = currentRow.OmittedHeaders; i < omittedHeaders.Count && omittedHeaders[i].GroupLabel == null; i++)
			{
			}
			if (i < omittedHeaders.Count)
			{
				int num = omittedHeaders[i].ColIndex;
				this.html5Renderer.WriteStream(HTMLElements.m_openTR);
				this.html5Renderer.WriteStream(HTMLElements.m_zeroHeight);
				this.html5Renderer.WriteStream(HTMLElements.m_closeBracket);
				this.html5Renderer.WriteStream(HTMLElements.m_openTD);
				this.html5Renderer.WriteStream(HTMLElements.m_colSpan);
				this.html5Renderer.WriteStream(num.ToString(CultureInfo.InvariantCulture));
				this.html5Renderer.WriteStream(HTMLElements.m_quote);
				this.html5Renderer.WriteStream(HTMLElements.m_closeBracket);
				this.html5Renderer.WriteStream(HTMLElements.m_closeTD);
				for (; i < omittedHeaders.Count; i++)
				{
					if (omittedHeaders[i].GroupLabel != null)
					{
						this.html5Renderer.WriteStream(HTMLElements.m_openTD);
						int colIndex = omittedHeaders[i].ColIndex;
						int num2 = colIndex - num;
						if (num2 > 1)
						{
							this.html5Renderer.WriteStream(HTMLElements.m_colSpan);
							this.html5Renderer.WriteStream(num2.ToString(CultureInfo.InvariantCulture));
							this.html5Renderer.WriteStream(HTMLElements.m_quote);
							this.html5Renderer.WriteStream(HTMLElements.m_closeBracket);
							this.html5Renderer.WriteStream(HTMLElements.m_closeTD);
							this.html5Renderer.WriteStream(HTMLElements.m_openTD);
						}
						int colSpan = omittedHeaders[i].ColSpan;
						if (colSpan > 1)
						{
							this.html5Renderer.WriteStream(HTMLElements.m_colSpan);
							this.html5Renderer.WriteStream(colSpan.ToString(CultureInfo.InvariantCulture));
							this.html5Renderer.WriteStream(HTMLElements.m_quote);
						}
						this.html5Renderer.RenderReportItemId(omittedHeaders[i].UniqueName);
						this.html5Renderer.WriteStream(HTMLElements.m_closeBracket);
						this.html5Renderer.WriteStream(HTMLElements.m_closeTD);
						num = colIndex + colSpan;
					}
				}
				if (num < columns)
				{
					this.html5Renderer.WriteStream(HTMLElements.m_openTD);
					this.html5Renderer.WriteStream(HTMLElements.m_colSpan);
					this.html5Renderer.WriteStream((columns - num).ToString(CultureInfo.InvariantCulture));
					this.html5Renderer.WriteStream(HTMLElements.m_quote);
					this.html5Renderer.WriteStream(HTMLElements.m_closeBracket);
					this.html5Renderer.WriteStream(HTMLElements.m_closeTD);
				}
				this.html5Renderer.WriteStream(HTMLElements.m_closeTR);
			}
		}

		protected void RenderSimpleTablixRows(RPLTablix tablix, string tablixID, RPLTablixRow currentRow, int borderContext, TablixFixedHeaderStorage headerStorage)
		{
			int num = 0;
			StyleContext styleContext = new StyleContext();
			float[] rowHeights = tablix.RowHeights;
			int num2 = tablix.ColumnWidths.Length;
			int num3 = rowHeights.Length;
			bool flag = headerStorage.ColumnHeaders != null;
			SharedListLayoutState sharedListLayoutState = SharedListLayoutState.None;
			while (currentRow != null)
			{
				List<RPLTablixMemberCell> omittedHeaders = currentRow.OmittedHeaders;
				int num4 = 0;
				if (num2 == 1)
				{
					sharedListLayoutState = SharedListLayoutState.None;
					bool flag2 = tablix.SharedLayoutRow(num);
					bool flag3 = tablix.UseSharedLayoutRow(num);
					bool flag4 = tablix.RowsState.Length > num + 1 && tablix.UseSharedLayoutRow(num + 1);
					if (flag2 && flag4)
					{
						sharedListLayoutState = SharedListLayoutState.Start;
					}
					else if (flag3)
					{
						sharedListLayoutState = (SharedListLayoutState)((!flag4) ? 3 : 2);
					}
				}
				if (sharedListLayoutState != 0 && sharedListLayoutState != SharedListLayoutState.Start)
				{
					goto IL_01db;
				}
				if (rowHeights[num] == 0.0 && num > 1 && currentRow.NumCells == 1 && currentRow[0].Element is RPLRectangle)
				{
					RPLRectangle rPLRectangle = (RPLRectangle)currentRow[0].Element;
					if (rPLRectangle.Children != null && rPLRectangle.Children.Length != 0)
					{
						goto IL_00fe;
					}
					currentRow = tablix.GetNextRow();
					num++;
					continue;
				}
				goto IL_00fe;
				IL_00fe:
				this.html5Renderer.WriteStream(HTMLElements.m_openTR);
				if (tablix.FixedRow(num) || headerStorage.RowHeaders != null || flag)
				{
					string text = tablixID + "tr" + num;
					this.html5Renderer.RenderReportItemId(text);
					if (tablix.FixedRow(num))
					{
						headerStorage.ColumnHeaders.Add(text);
						if (headerStorage.CornerHeaders != null)
						{
							headerStorage.CornerHeaders.Add(text);
						}
					}
					else if (flag)
					{
						headerStorage.BodyID = text;
						flag = false;
					}
					if (headerStorage.RowHeaders != null)
					{
						headerStorage.RowHeaders.Add(text);
					}
				}
				this.html5Renderer.WriteStream(HTMLElements.m_valign);
				this.html5Renderer.WriteStream(HTMLElements.m_topValue);
				this.html5Renderer.WriteStream(HTMLElements.m_quote);
				this.html5Renderer.WriteStream(HTMLElements.m_closeBracket);
				goto IL_01db;
				IL_01db:
				int numCells = currentRow.NumCells;
				bool firstRow = num == 0;
				bool lastRow = num == num3 - 1;
				RPLTablixCell rPLTablixCell = currentRow[0];
				currentRow[0] = null;
				if (sharedListLayoutState != 0)
				{
					this.RenderListReportItem(tablix, rPLTablixCell, omittedHeaders, borderContext, styleContext, firstRow, lastRow, sharedListLayoutState, rPLTablixCell.Element);
				}
				else
				{
					this.RenderSimpleTablixCellWithHeight(rowHeights[num], tablix, tablixID, num2, num, borderContext, rPLTablixCell, omittedHeaders, ref num4, styleContext, firstRow, lastRow, headerStorage);
				}
				int i;
				for (i = 1; i < numCells - 1; i++)
				{
					rPLTablixCell = currentRow[i];
					this.RenderSimpleTablixCell(tablix, tablixID, rPLTablixCell.ColSpan, num, borderContext, rPLTablixCell, omittedHeaders, ref num4, false, firstRow, lastRow, headerStorage);
					currentRow[i] = null;
				}
				if (numCells > 1)
				{
					rPLTablixCell = currentRow[i];
					this.RenderSimpleTablixCell(tablix, tablixID, rPLTablixCell.ColSpan, num, borderContext, rPLTablixCell, omittedHeaders, ref num4, true, firstRow, lastRow, headerStorage);
					currentRow[i] = null;
				}
				if (sharedListLayoutState == SharedListLayoutState.None || sharedListLayoutState == SharedListLayoutState.End)
				{
					this.html5Renderer.WriteStream(HTMLElements.m_closeTR);
				}
				currentRow = tablix.GetNextRow();
				num++;
			}
		}

		private void RenderSimpleTablixCellWithHeight(float height, RPLTablix tablix, string tablixID, int numCols, int row, int tablixContext, RPLTablixCell cell, List<RPLTablixMemberCell> omittedCells, ref int omittedIndex, StyleContext styleContext, bool firstRow, bool lastRow, TablixFixedHeaderStorage headerStorage)
		{
			int colIndex = cell.ColIndex;
			int num = cell.ColSpan;
			bool lastCol = colIndex + num == numCols;
			bool zeroWidth = styleContext.ZeroWidth;
			float columnWidth = tablix.GetColumnWidth(colIndex, num);
			styleContext.ZeroWidth = (columnWidth == 0.0);
			int startIndex = this.RenderZeroWidthTDsForTablix(colIndex, num, tablix);
			num = this.GetColSpanMinusZeroWidthColumns(colIndex, num, tablix);
			this.html5Renderer.WriteStream(HTMLElements.m_openTD);
			this.RenderSimpleTablixCellID(tablix, tablixID, row, headerStorage, colIndex);
			if (num > 1)
			{
				this.html5Renderer.WriteStream(HTMLElements.m_colSpan);
				this.html5Renderer.WriteStream(num.ToString(CultureInfo.InvariantCulture));
				this.html5Renderer.WriteStream(HTMLElements.m_quote);
			}
			this.html5Renderer.OpenStyle();
			this.html5Renderer.WriteStream(HTMLElements.m_styleHeight);
			this.html5Renderer.WriteDStream(height);
			this.html5Renderer.WriteStream(HTMLElements.m_mm);
			RPLElement element = cell.Element;
			if (element != null)
			{
				this.html5Renderer.WriteStream(HTMLElements.m_semiColon);
				int num2 = 0;
				this.RenderTablixReportItemStyle(tablix, tablixContext, cell, styleContext, true, lastCol, firstRow, lastRow, element, ref num2);
				this.RenderTablixOmittedHeaderCells(omittedCells, colIndex, lastCol, ref omittedIndex);
				this.RenderReportItem(tablix, tablixContext, cell, styleContext, true, lastCol, firstRow, lastRow, element, ref num2);
			}
			else
			{
				if (styleContext.ZeroWidth)
				{
					this.html5Renderer.WriteStream(HTMLElements.m_displayNone);
				}
				this.html5Renderer.CloseStyle(true);
				this.html5Renderer.WriteStream(HTMLElements.m_closeBracket);
				this.RenderTablixOmittedHeaderCells(omittedCells, colIndex, lastCol, ref omittedIndex);
				this.html5Renderer.WriteStream(HTMLElements.m_nbsp);
			}
			this.html5Renderer.WriteStream(HTMLElements.m_closeTD);
			this.RenderZeroWidthTDsForTablix(startIndex, num, tablix);
			styleContext.ZeroWidth = zeroWidth;
		}

		private void RenderTablixReportItemStyle(RPLTablix tablix, int tablixContext, RPLTablixCell cell, StyleContext styleContext, bool firstCol, bool lastCol, bool firstRow, bool lastRow, RPLElement cellItem, ref int borderContext)
		{
			RPLElementProps elementProps = cellItem.ElementProps;
			RPLElementPropsDef definition = elementProps.Definition;
			RPLTextBox rPLTextBox = cellItem as RPLTextBox;
			RPLTextBoxProps rPLTextBoxProps = (rPLTextBox != null) ? (rPLTextBox.ElementProps as RPLTextBoxProps) : null;
			RPLTextBoxPropsDef rPLTextBoxPropsDef = (rPLTextBox != null) ? (elementProps.Definition as RPLTextBoxPropsDef) : null;
			styleContext.OmitBordersState = cell.ElementState;
			if (!(cellItem is RPLLine))
			{
				styleContext.StyleOnCell = true;
				borderContext = HTML5Renderer.GetNewContext(tablixContext, firstCol, lastCol, firstRow, lastRow);
				if (rPLTextBox != null)
				{
					bool ignorePadding = styleContext.IgnorePadding;
					styleContext.IgnorePadding = true;
					RPLItemMeasurement rPLItemMeasurement = null;
					if (this.html5Renderer.m_deviceInfo.OutlookCompat || !this.html5Renderer.m_deviceInfo.IsBrowserIE)
					{
						rPLItemMeasurement = new RPLItemMeasurement();
						rPLItemMeasurement.Width = tablix.GetColumnWidth(cell.ColIndex, cell.ColSpan);
					}
					styleContext.EmptyTextBox = (rPLTextBoxPropsDef.IsSimple && string.IsNullOrEmpty(rPLTextBoxProps.Value) && string.IsNullOrEmpty(rPLTextBoxPropsDef.Value) && !this.html5Renderer.NeedSharedToggleParent(rPLTextBoxProps) && !this.html5Renderer.CanSort(rPLTextBoxPropsDef));
					string textBoxClass = this.html5Renderer.GetTextBoxClass(rPLTextBoxPropsDef, rPLTextBoxProps, rPLTextBoxProps.NonSharedStyle, definition.ID + "c");
					bool onlyRenderMeasurementsBackgroundBorders = styleContext.OnlyRenderMeasurementsBackgroundBorders;
					if (HTML5Renderer.IsWritingModeVertical(rPLTextBoxProps.Style) && this.html5Renderer.m_deviceInfo.IsBrowserIE && (rPLTextBoxPropsDef.CanGrow || (this.html5Renderer.m_deviceInfo.BrowserMode == BrowserMode.Standards && !this.html5Renderer.m_deviceInfo.IsBrowserIE6Or7StandardsMode)))
					{
						styleContext.OnlyRenderMeasurementsBackgroundBorders = true;
					}
					this.html5Renderer.RenderReportItemStyle(cellItem, elementProps, definition, rPLTextBoxProps.NonSharedStyle, rPLTextBoxPropsDef.SharedStyle, rPLItemMeasurement, styleContext, ref borderContext, textBoxClass, false);
					styleContext.OnlyRenderMeasurementsBackgroundBorders = onlyRenderMeasurementsBackgroundBorders;
					styleContext.IgnorePadding = ignorePadding;
				}
				else
				{
					this.html5Renderer.RenderReportItemStyle(cellItem, elementProps, definition, (RPLItemMeasurement)null, styleContext, ref borderContext, definition.ID + "c");
				}
				styleContext.StyleOnCell = false;
			}
			else if (styleContext.ZeroWidth)
			{
				this.html5Renderer.WriteStream(HTMLElements.m_displayNone);
			}
			this.html5Renderer.CloseStyle(true);
			if (styleContext.EmptyTextBox && rPLTextBox != null && elementProps != null)
			{
				this.html5Renderer.WriteToolTip(elementProps);
			}
			this.html5Renderer.WriteStream(HTMLElements.m_closeBracket);
		}

		private void RenderReportItem(RPLTablix tablix, int tablixContext, RPLTablixCell cell, StyleContext styleContext, bool firstCol, bool lastCol, bool firstRow, bool lastRow, RPLElement cellItem, ref int borderContext)
		{
			RPLElementProps elementProps = cellItem.ElementProps;
			RPLElementPropsDef definition = elementProps.Definition;
			RPLTextBox rPLTextBox = cellItem as RPLTextBox;
			RPLTextBoxProps rPLTextBoxProps = (rPLTextBox != null) ? (rPLTextBox.ElementProps as RPLTextBoxProps) : null;
			RPLTextBoxPropsDef rPLTextBoxPropsDef = (rPLTextBox != null) ? (elementProps.Definition as RPLTextBoxPropsDef) : null;
			RPLItemMeasurement rPLItemMeasurement = new RPLItemMeasurement();
			styleContext.OmitBordersState = cell.ElementState;
			if (styleContext.EmptyTextBox)
			{
				bool flag = false;
				RPLActionInfo actionInfo = rPLTextBoxProps.ActionInfo;
				if (this.html5Renderer.HasAction(actionInfo))
				{
					this.html5Renderer.RenderElementHyperlinkAllTextStyles(rPLTextBoxProps.Style, actionInfo.Actions[0], rPLTextBoxPropsDef.ID + "a");
					this.html5Renderer.WriteStream(HTMLElements.m_openDiv);
					this.html5Renderer.OpenStyle();
					rPLItemMeasurement.Height = tablix.GetRowHeight(cell.RowIndex, cell.RowSpan);
					rPLItemMeasurement.Height = this.html5Renderer.GetInnerContainerHeightSubtractBorders(rPLItemMeasurement, rPLTextBoxProps.Style);
					this.html5Renderer.RenderMeasurementMinHeight(rPLItemMeasurement.Height);
					this.html5Renderer.WriteStream(HTMLElements.m_semiColon);
					this.html5Renderer.WriteStream(HTMLElements.m_cursorHand);
					this.html5Renderer.WriteStream(HTMLElements.m_semiColon);
					this.html5Renderer.CloseStyle(true);
					this.html5Renderer.WriteStream(HTMLElements.m_closeBracket);
					flag = true;
				}
				this.html5Renderer.WriteStream(HTMLElements.m_nbsp);
				if (flag)
				{
					this.html5Renderer.WriteStream(HTMLElements.m_closeDiv);
					this.html5Renderer.WriteStream(HTMLElements.m_closeA);
				}
			}
			else
			{
				styleContext.InTablix = true;
				bool renderId = this.html5Renderer.NeedReportItemId(cellItem, elementProps);
				if (rPLTextBox != null)
				{
					styleContext.RenderMeasurements = false;
					rPLItemMeasurement.Width = tablix.GetColumnWidth(cell.ColIndex, cell.ColSpan);
					rPLItemMeasurement.Height = tablix.GetRowHeight(cell.RowIndex, cell.RowSpan);
					int num = 0;
					new TextBoxRenderer(this.html5Renderer).RenderReportItem(rPLTextBox, rPLItemMeasurement, styleContext, ref num, renderId, false);
				}
				else
				{
					rPLItemMeasurement.Width = tablix.GetColumnWidth(cell.ColIndex, cell.ColSpan);
					rPLItemMeasurement.Height = tablix.GetRowHeight(cell.RowIndex, cell.RowSpan);
					if (cellItem is RPLRectangle || cellItem is RPLSubReport || cellItem is RPLLine)
					{
						styleContext.RenderMeasurements = false;
					}
					this.html5Renderer.RenderReportItem(cellItem, elementProps, definition, rPLItemMeasurement, styleContext, borderContext, renderId, false);
				}
			}
			styleContext.Reset();
		}

		internal void RenderEmptyTopTablixRow(RPLTablix tablix, List<RPLTablixOmittedRow> omittedRows, string tablixID, bool emptyCol, TablixFixedHeaderStorage headerStorage)
		{
			bool flag = headerStorage.RowHeaders != null || headerStorage.ColumnHeaders != null;
			this.html5Renderer.WriteStream(HTMLElements.m_openTR);
			if (flag)
			{
				string text = tablixID + "r";
				this.html5Renderer.RenderReportItemId(text);
				if (headerStorage.RowHeaders != null)
				{
					headerStorage.RowHeaders.Add(text);
				}
				if (headerStorage.ColumnHeaders != null)
				{
					headerStorage.ColumnHeaders.Add(text);
				}
				if (headerStorage.CornerHeaders != null)
				{
					headerStorage.CornerHeaders.Add(text);
				}
			}
			this.html5Renderer.WriteStream(HTMLElements.m_zeroHeight);
			this.html5Renderer.WriteStream(HTMLElements.m_closeBracket);
			if (emptyCol)
			{
				headerStorage.HasEmptyCol = true;
				this.html5Renderer.WriteStream(HTMLElements.m_openTD);
				if (headerStorage.RowHeaders != null)
				{
					string text2 = tablixID + "e";
					this.html5Renderer.RenderReportItemId(text2);
					headerStorage.RowHeaders.Add(text2);
					if (headerStorage.CornerHeaders != null)
					{
						headerStorage.CornerHeaders.Add(text2);
					}
				}
				this.html5Renderer.WriteStream(HTMLElements.m_openStyle);
				this.html5Renderer.WriteStream(HTMLElements.m_styleWidth);
				this.html5Renderer.WriteStream("0");
				this.html5Renderer.WriteStream(HTMLElements.m_px);
				this.html5Renderer.WriteStream(HTMLElements.m_closeQuote);
				this.html5Renderer.WriteStream(HTMLElements.m_closeTD);
			}
			int[] array = new int[omittedRows.Count];
			for (int i = 0; i < tablix.ColumnWidths.Length; i++)
			{
				this.html5Renderer.WriteStream(HTMLElements.m_openTD);
				if (tablix.FixedColumns[i] && headerStorage.RowHeaders != null)
				{
					string text3 = tablixID + "e" + i;
					this.html5Renderer.RenderReportItemId(text3);
					headerStorage.RowHeaders.Add(text3);
					if (i == tablix.ColumnWidths.Length - 1 || !tablix.FixedColumns[i + 1])
					{
						headerStorage.LastRowGroupCol = text3;
					}
					if (headerStorage.CornerHeaders != null)
					{
						headerStorage.CornerHeaders.Add(text3);
					}
				}
				this.html5Renderer.WriteStream(HTMLElements.m_openStyle);
				if (tablix.ColumnWidths[i] == 0.0)
				{
					this.html5Renderer.WriteStream(HTMLElements.m_displayNone);
				}
				this.html5Renderer.WriteStream(HTMLElements.m_styleWidth);
				this.html5Renderer.WriteDStream(tablix.ColumnWidths[i]);
				this.html5Renderer.WriteStream(HTMLElements.m_mm);
				this.html5Renderer.WriteStream(HTMLElements.m_semiColon);
				this.html5Renderer.WriteStream(HTMLElements.m_styleMinWidth);
				this.html5Renderer.WriteDStream(tablix.ColumnWidths[i]);
				this.html5Renderer.WriteStream(HTMLElements.m_mm);
				this.html5Renderer.WriteStream(HTMLElements.m_closeQuote);
				for (int j = 0; j < omittedRows.Count; j++)
				{
					List<RPLTablixMemberCell> omittedHeaders = omittedRows[j].OmittedHeaders;
					this.RenderTablixOmittedHeaderCells(omittedHeaders, i, false, ref array[j]);
				}
				this.html5Renderer.WriteStream(HTMLElements.m_closeTD);
			}
			this.html5Renderer.WriteStream(HTMLElements.m_closeTR);
		}

		private void RenderTablixCell(RPLTablix tablix, bool fixedHeader, string tablixID, int numCols, int numRows, int col, int colSpan, int row, int tablixContext, RPLTablixCell cell, List<RPLTablixMemberCell> omittedCells, ref int omittedIndex, StyleContext styleContext, TablixFixedHeaderStorage headerStorage, HTMLHeader[] rowHeaderIds, string[] colHeaderIds, OmittedHeaderStack omittedHeaders)
		{
			bool lastCol = col + colSpan == numCols;
			bool zeroWidth = styleContext.ZeroWidth;
			float columnWidth = tablix.GetColumnWidth(cell.ColIndex, cell.ColSpan);
			styleContext.ZeroWidth = (columnWidth == 0.0);
			int startIndex = this.RenderZeroWidthTDsForTablix(col, colSpan, tablix);
			colSpan = this.GetColSpanMinusZeroWidthColumns(col, colSpan, tablix);
			bool useElementName = this.html5Renderer.m_deviceInfo.AccessibleTablix && tablix.RowHeaderColumns > 0 && col >= tablix.ColsBeforeRowHeaders && col < tablix.RowHeaderColumns + tablix.ColsBeforeRowHeaders;
			bool fixedCornerHeader = fixedHeader && tablix.FixedColumns[col] && tablix.FixedRow(row);
			string text = this.CalculateRowHeaderId(cell, fixedHeader, tablixID, cell.RowIndex, cell.ColIndex, headerStorage, useElementName, fixedCornerHeader);
			this.html5Renderer.WriteStream(HTMLElements.m_openTD);
			if (this.html5Renderer.m_deviceInfo.AccessibleTablix)
			{
				this.RenderAccessibleHeaders(tablix, fixedHeader, numCols, cell.ColIndex, colSpan, cell.RowIndex, cell, omittedCells, rowHeaderIds, colHeaderIds, omittedHeaders, ref text);
			}
			if (text != null)
			{
				this.html5Renderer.RenderReportItemId(text);
			}
			int rowSpan = cell.RowSpan;
			if (cell.RowSpan > 1)
			{
				this.html5Renderer.WriteStream(HTMLElements.m_rowSpan);
				this.html5Renderer.WriteStream(cell.RowSpan.ToString(CultureInfo.InvariantCulture));
				this.html5Renderer.WriteStream(HTMLElements.m_quote);
				this.html5Renderer.WriteStream(HTMLElements.m_inlineHeight);
				this.html5Renderer.WriteStream(Utility.MmToPxAsString((double)tablix.GetRowHeight(cell.RowIndex, cell.RowSpan)));
				this.html5Renderer.WriteStream(HTMLElements.m_quote);
			}
			if (colSpan > 1)
			{
				this.html5Renderer.WriteStream(HTMLElements.m_colSpan);
				this.html5Renderer.WriteStream(cell.ColSpan.ToString(CultureInfo.InvariantCulture));
				this.html5Renderer.WriteStream(HTMLElements.m_quote);
			}
			RPLElement element = cell.Element;
			if (element != null)
			{
				int num = 0;
				this.RenderTablixReportItemStyle(tablix, tablixContext, cell, styleContext, col == 0, lastCol, row == 0, row + rowSpan == numRows, element, ref num);
				this.RenderTablixOmittedHeaderCells(omittedCells, col, lastCol, ref omittedIndex);
				this.RenderReportItem(tablix, tablixContext, cell, styleContext, col == 0, lastCol, row == 0, row + rowSpan == numRows, element, ref num);
			}
			else
			{
				if (styleContext.ZeroWidth)
				{
					this.html5Renderer.OpenStyle();
					this.html5Renderer.WriteStream(HTMLElements.m_displayNone);
					this.html5Renderer.CloseStyle(true);
				}
				this.html5Renderer.WriteStream(HTMLElements.m_closeBracket);
				this.RenderTablixOmittedHeaderCells(omittedCells, col, lastCol, ref omittedIndex);
				this.html5Renderer.WriteStream(HTMLElements.m_nbsp);
			}
			this.html5Renderer.WriteStream(HTMLElements.m_closeTD);
			this.RenderZeroWidthTDsForTablix(startIndex, colSpan, tablix);
			styleContext.ZeroWidth = zeroWidth;
		}

		private void RenderColumnHeaderTablixCell(RPLTablix tablix, string tablixID, int numCols, int col, int colSpan, int row, int tablixContext, RPLTablixCell cell, StyleContext styleContext, TablixFixedHeaderStorage headerStorage, List<RPLTablixOmittedRow> omittedRows, int[] omittedIndices)
		{
			bool lastCol = col + colSpan == numCols;
			bool zeroWidth = styleContext.ZeroWidth;
			float columnWidth = tablix.GetColumnWidth(col, colSpan);
			styleContext.ZeroWidth = (columnWidth == 0.0);
			int startIndex = this.RenderZeroWidthTDsForTablix(col, colSpan, tablix);
			colSpan = this.GetColSpanMinusZeroWidthColumns(col, colSpan, tablix);
			this.html5Renderer.WriteStream(HTMLElements.m_openTD);
			int rowSpan = cell.RowSpan;
			string text = null;
			if (cell is RPLTablixMemberCell && (((RPLTablixMemberCell)cell).GroupLabel != null || this.html5Renderer.m_deviceInfo.AccessibleTablix))
			{
				text = ((RPLTablixMemberCell)cell).UniqueName;
				if (text == null && cell.Element != null && cell.Element.ElementProps != null)
				{
					text = cell.Element.ElementProps.UniqueName;
					((RPLTablixMemberCell)cell).UniqueName = text;
				}
				if (text != null)
				{
					this.html5Renderer.RenderReportItemId(text);
				}
			}
			if (tablix.FixedColumns[col])
			{
				if (text == null)
				{
					text = tablixID + "r" + row + "c" + col;
					this.html5Renderer.RenderReportItemId(text);
				}
				headerStorage.RowHeaders.Add(text);
				if (headerStorage.CornerHeaders != null)
				{
					headerStorage.CornerHeaders.Add(text);
				}
			}
			if (rowSpan > 1)
			{
				this.html5Renderer.WriteStream(HTMLElements.m_rowSpan);
				this.html5Renderer.WriteStream(cell.RowSpan.ToString(CultureInfo.InvariantCulture));
				this.html5Renderer.WriteStream(HTMLElements.m_quote);
				this.html5Renderer.WriteStream(HTMLElements.m_inlineHeight);
				this.html5Renderer.WriteStream(Utility.MmToPxAsString((double)tablix.GetRowHeight(cell.RowIndex, cell.RowSpan)));
				this.html5Renderer.WriteStream(HTMLElements.m_quote);
			}
			if (colSpan > 1)
			{
				this.html5Renderer.WriteStream(HTMLElements.m_colSpan);
				this.html5Renderer.WriteStream(cell.ColSpan.ToString(CultureInfo.InvariantCulture));
				this.html5Renderer.WriteStream(HTMLElements.m_quote);
			}
			RPLElement element = cell.Element;
			if (element != null)
			{
				int num = 0;
				this.RenderTablixReportItemStyle(tablix, tablixContext, cell, styleContext, col == 0, lastCol, row == 0, false, element, ref num);
				for (int i = 0; i < omittedRows.Count; i++)
				{
					this.RenderTablixOmittedHeaderCells(omittedRows[i].OmittedHeaders, col, lastCol, ref omittedIndices[i]);
				}
				this.RenderReportItem(tablix, tablixContext, cell, styleContext, col == 0, lastCol, row == 0, false, element, ref num);
			}
			else
			{
				if (styleContext.ZeroWidth)
				{
					this.html5Renderer.OpenStyle();
					this.html5Renderer.WriteStream(HTMLElements.m_displayNone);
					this.html5Renderer.CloseStyle(true);
				}
				this.html5Renderer.WriteStream(HTMLElements.m_closeBracket);
				for (int j = 0; j < omittedRows.Count; j++)
				{
					this.RenderTablixOmittedHeaderCells(omittedRows[j].OmittedHeaders, col, lastCol, ref omittedIndices[j]);
				}
				this.html5Renderer.WriteStream(HTMLElements.m_nbsp);
			}
			this.html5Renderer.WriteStream(HTMLElements.m_closeTD);
			this.RenderZeroWidthTDsForTablix(startIndex, colSpan, tablix);
			styleContext.ZeroWidth = zeroWidth;
		}

		private int RenderZeroWidthTDsForTablix(int startIndex, int colSpan, RPLTablix tablix)
		{
			int i;
			for (i = startIndex; i < startIndex + colSpan && tablix.ColumnWidths[i] == 0.0; i++)
			{
				this.html5Renderer.WriteStream(HTMLElements.m_openTD);
				this.html5Renderer.OpenStyle();
				this.html5Renderer.WriteStream(HTMLElements.m_displayNone);
				this.html5Renderer.CloseStyle(true);
				this.html5Renderer.WriteStream(HTMLElements.m_closeBracket);
				this.html5Renderer.WriteStream(HTMLElements.m_closeTD);
			}
			return i;
		}

		private void RenderSimpleTablixCellID(RPLTablix tablix, string tablixID, int row, TablixFixedHeaderStorage headerStorage, int col)
		{
			if (tablix.FixedColumns[col])
			{
				string text = tablixID + "r" + row + "c" + col;
				this.html5Renderer.RenderReportItemId(text);
				headerStorage.RowHeaders.Add(text);
				if (headerStorage.CornerHeaders != null && tablix.FixedRow(row))
				{
					headerStorage.CornerHeaders.Add(text);
				}
			}
		}

		private void RenderSimpleTablixCell(RPLTablix tablix, string tablixID, int colSpan, int row, int tablixContext, RPLTablixCell cell, List<RPLTablixMemberCell> omittedCells, ref int omittedIndex, bool lastCol, bool firstRow, bool lastRow, TablixFixedHeaderStorage headerStorage)
		{
			StyleContext styleContext = new StyleContext();
			int colIndex = cell.ColIndex;
			bool zeroWidth = styleContext.ZeroWidth;
			float columnWidth = tablix.GetColumnWidth(colIndex, cell.ColSpan);
			styleContext.ZeroWidth = (columnWidth == 0.0);
			int startIndex = this.RenderZeroWidthTDsForTablix(colIndex, colSpan, tablix);
			colSpan = this.GetColSpanMinusZeroWidthColumns(colIndex, colSpan, tablix);
			this.html5Renderer.WriteStream(HTMLElements.m_openTD);
			this.RenderSimpleTablixCellID(tablix, tablixID, row, headerStorage, colIndex);
			if (colSpan > 1)
			{
				this.html5Renderer.WriteStream(HTMLElements.m_colSpan);
				this.html5Renderer.WriteStream(colSpan.ToString(CultureInfo.InvariantCulture));
				this.html5Renderer.WriteStream(HTMLElements.m_quote);
			}
			RPLElement element = cell.Element;
			if (element != null)
			{
				int num = 0;
				this.RenderTablixReportItemStyle(tablix, tablixContext, cell, styleContext, false, lastCol, firstRow, lastRow, element, ref num);
				this.RenderTablixOmittedHeaderCells(omittedCells, colIndex, lastCol, ref omittedIndex);
				this.RenderReportItem(tablix, tablixContext, cell, styleContext, false, lastCol, firstRow, lastRow, element, ref num);
			}
			else
			{
				if (styleContext.ZeroWidth)
				{
					this.html5Renderer.OpenStyle();
					this.html5Renderer.WriteStream(HTMLElements.m_displayNone);
					this.html5Renderer.CloseStyle(true);
				}
				this.html5Renderer.WriteStream(HTMLElements.m_closeBracket);
				this.html5Renderer.WriteStream(HTMLElements.m_nbsp);
				this.RenderTablixOmittedHeaderCells(omittedCells, colIndex, lastCol, ref omittedIndex);
			}
			this.html5Renderer.WriteStream(HTMLElements.m_closeTD);
			this.RenderZeroWidthTDsForTablix(startIndex, colSpan, tablix);
			styleContext.ZeroWidth = zeroWidth;
		}

		private bool InitFixedColumnHeaders(RPLTablix tablix, string tablixID, TablixFixedHeaderStorage storage)
		{
			for (int i = 0; i < tablix.RowHeights.Length; i++)
			{
				if (tablix.FixedRow(i))
				{
					storage.HtmlId = tablixID;
					storage.ColumnHeaders = new List<string>();
					return true;
				}
			}
			return false;
		}

		private bool InitFixedRowHeaders(RPLTablix tablix, string tablixID, TablixFixedHeaderStorage storage)
		{
			for (int i = 0; i < tablix.ColumnWidths.Length; i++)
			{
				if (tablix.FixedColumns[i])
				{
					storage.HtmlId = tablixID;
					storage.RowHeaders = new List<string>();
					return true;
				}
			}
			return false;
		}

		private string CalculateRowHeaderId(RPLTablixCell cell, bool fixedHeader, string tablixID, int row, int col, TablixFixedHeaderStorage headerStorage, bool useElementName, bool fixedCornerHeader)
		{
			string text = null;
			if (cell is RPLTablixMemberCell)
			{
				if (((RPLTablixMemberCell)cell).GroupLabel != null)
				{
					text = ((RPLTablixMemberCell)cell).UniqueName;
				}
				else if (!fixedHeader && useElementName && cell.Element != null && cell.Element.ElementProps != null)
				{
					text = cell.Element.ElementProps.UniqueName;
				}
			}
			if (fixedHeader)
			{
				if (text == null)
				{
					text = tablixID + "r" + row + "c" + col;
				}
				if (headerStorage != null)
				{
					headerStorage.RowHeaders.Add(text);
					if (headerStorage.CornerHeaders != null && fixedCornerHeader)
					{
						headerStorage.CornerHeaders.Add(text);
					}
				}
			}
			return text;
		}

		private void RenderEmptyHeightCell(float height, string tablixID, bool fixedRow, int row, TablixFixedHeaderStorage headerStorage)
		{
			this.html5Renderer.WriteStream(HTMLElements.m_openTD);
			if (headerStorage.RowHeaders != null)
			{
				string text = tablixID + "h" + row;
				this.html5Renderer.RenderReportItemId(text);
				headerStorage.RowHeaders.Add(text);
				if (fixedRow && headerStorage.CornerHeaders != null)
				{
					headerStorage.CornerHeaders.Add(text);
				}
			}
			this.html5Renderer.WriteStream(HTMLElements.m_openStyle);
			this.html5Renderer.WriteStream(HTMLElements.m_styleHeight);
			this.html5Renderer.WriteDStream(height);
			this.html5Renderer.WriteStream(HTMLElements.m_mm);
			this.html5Renderer.WriteStream(HTMLElements.m_closeQuote);
			this.html5Renderer.WriteStream(HTMLElements.m_closeTD);
		}

		protected int GetColSpanMinusZeroWidthColumns(int startColIndex, int colSpan, RPLTablix tablix)
		{
			int num = colSpan;
			for (int i = startColIndex; i < startColIndex + colSpan; i++)
			{
				if (tablix.ColumnWidths[i] == 0.0)
				{
					num--;
				}
			}
			return num;
		}

		private void RenderListReportItem(RPLTablix tablix, RPLTablixCell cell, List<RPLTablixMemberCell> omittedHeaders, int tablixContext, StyleContext styleContext, bool firstRow, bool lastRow, SharedListLayoutState layoutState, RPLElement cellItem)
		{
			RPLElementProps elementProps = cellItem.ElementProps;
			RPLElementPropsDef definition = elementProps.Definition;
			RPLItemMeasurement rPLItemMeasurement = null;
			rPLItemMeasurement = new RPLItemMeasurement();
			rPLItemMeasurement.Width = tablix.ColumnWidths[0];
			rPLItemMeasurement.Height = tablix.GetRowHeight(cell.RowIndex, cell.RowSpan);
			rPLItemMeasurement.State = cell.ElementState;
			bool zeroWidth = styleContext.ZeroWidth;
			styleContext.ZeroWidth = (rPLItemMeasurement.Width == 0.0);
			if (layoutState == SharedListLayoutState.Start)
			{
				this.html5Renderer.WriteStream(HTMLElements.m_openTD);
				if (styleContext.ZeroWidth)
				{
					this.html5Renderer.OpenStyle();
					this.html5Renderer.WriteStream(HTMLElements.m_displayNone);
					this.html5Renderer.CloseStyle(true);
				}
				this.html5Renderer.WriteStream(HTMLElements.m_closeBracket);
			}
			if (cellItem is RPLRectangle)
			{
				int num = tablix.ColumnWidths.Length;
				int colIndex = cell.ColIndex;
				int colSpan = cell.ColSpan;
				bool right = colIndex + colSpan == num;
				int newContext = HTML5Renderer.GetNewContext(tablixContext, true, right, firstRow, lastRow);
				this.RenderListRectangle((RPLRectangle)cellItem, omittedHeaders, rPLItemMeasurement, elementProps, definition, layoutState, newContext);
				if (layoutState == SharedListLayoutState.End)
				{
					this.html5Renderer.WriteStream(HTMLElements.m_closeTD);
				}
			}
			else
			{
				int num2 = 0;
				this.RenderTablixOmittedHeaderCells(omittedHeaders, 0, true, ref num2);
				styleContext.Reset();
				if (layoutState == SharedListLayoutState.End)
				{
					this.html5Renderer.WriteStream(HTMLElements.m_closeTD);
				}
			}
			styleContext.ZeroWidth = zeroWidth;
		}

		private void RenderTablixOmittedHeaderCells(List<RPLTablixMemberCell> omittedHeaders, int colIndex, bool lastCol, ref int omittedIndex)
		{
			if (omittedHeaders != null)
			{
				while (true)
				{
					if (omittedIndex >= omittedHeaders.Count)
					{
						break;
					}
					if (omittedHeaders[omittedIndex].ColIndex != colIndex)
					{
						if (!lastCol)
						{
							break;
						}
						if (omittedHeaders[omittedIndex].ColIndex <= colIndex)
						{
							break;
						}
					}
					RPLTablixMemberCell rPLTablixMemberCell = omittedHeaders[omittedIndex];
					if (rPLTablixMemberCell.GroupLabel != null)
					{
						this.html5Renderer.RenderNavigationId(rPLTablixMemberCell.UniqueName);
					}
					omittedIndex++;
				}
			}
		}

		private void RenderListRectangle(RPLContainer rectangle, List<RPLTablixMemberCell> omittedHeaders, RPLItemMeasurement measurement, RPLElementProps props, RPLElementPropsDef def, SharedListLayoutState layoutState, int borderContext)
		{
			RPLItemMeasurement[] children = rectangle.Children;
			this.html5Renderer.GenerateHTMLTable(children, measurement.Top, measurement.Left, measurement.Width, measurement.Height, borderContext, false, layoutState, omittedHeaders, props.Style, false);
		}

		private void RenderAccessibleHeaders(RPLTablix tablix, bool fixedHeader, int numCols, int col, int colSpan, int row, RPLTablixCell cell, List<RPLTablixMemberCell> omittedCells, HTMLHeader[] rowHeaderIds, string[] colHeaderIds, OmittedHeaderStack omittedHeaders, ref string id)
		{
			int currentLevel = -1;
			if (tablix.RowHeaderColumns == 0 && omittedCells != null && omittedCells.Count > 0)
			{
				foreach (RPLTablixMemberCell omittedCell in omittedCells)
				{
					RPLTablixMemberDef tablixMemberDef = omittedCell.TablixMemberDef;
					if (tablixMemberDef != null && tablixMemberDef.IsStatic && tablixMemberDef.StaticHeadersTree)
					{
						if (id == null && cell.Element != null && cell.Element.ElementProps.UniqueName != null)
						{
							id = cell.Element.ElementProps.UniqueName;
						}
						currentLevel = tablixMemberDef.Level;
						omittedHeaders.Push(tablixMemberDef.Level, col, colSpan, id, numCols);
					}
				}
			}
			if (row >= tablix.ColumnHeaderRows && !fixedHeader && (col < tablix.ColsBeforeRowHeaders || tablix.RowHeaderColumns <= 0 || col >= tablix.RowHeaderColumns + tablix.ColsBeforeRowHeaders))
			{
				bool flag = false;
				string text = colHeaderIds[cell.ColIndex];
				if (!string.IsNullOrEmpty(text))
				{
					this.html5Renderer.WriteStream(HTMLElements.m_headers);
					this.html5Renderer.WriteStream(text);
					flag = true;
				}
				foreach (HTMLHeader hTMLHeader in rowHeaderIds)
				{
					string iD = hTMLHeader.ID;
					if (!string.IsNullOrEmpty(iD))
					{
						if (flag)
						{
							this.html5Renderer.WriteStream(HTMLElements.m_space);
						}
						else
						{
							this.html5Renderer.WriteStream(HTMLElements.m_headers);
						}
						this.html5Renderer.WriteAttrEncoded(this.html5Renderer.m_deviceInfo.HtmlPrefixId);
						this.html5Renderer.WriteStream(iD);
						flag = true;
					}
				}
				string headers = omittedHeaders.GetHeaders(col, currentLevel, HttpUtility.HtmlAttributeEncode(this.html5Renderer.m_deviceInfo.HtmlPrefixId));
				if (!string.IsNullOrEmpty(headers))
				{
					if (flag)
					{
						this.html5Renderer.WriteStream(HTMLElements.m_space);
					}
					else
					{
						this.html5Renderer.WriteStream(HTMLElements.m_headers);
					}
					this.html5Renderer.WriteStream(headers);
					flag = true;
				}
				if (flag)
				{
					this.html5Renderer.WriteStream(HTMLElements.m_quote);
				}
			}
		}
	}
}
