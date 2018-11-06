using AspNetCore.ReportingServices.Diagnostics;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportProcessing.Upgrade
{
	internal sealed class Upgrader
	{
		private sealed class DataSetUpgrader
		{
			internal void Upgrade(Report report)
			{
				List<DataSet> list = new List<DataSet>();
				int num = 0;
				this.DetermineCurrentMaxID(report, ref list, ref num);
				for (int i = 0; i < list.Count; i++)
				{
					num++;
					list[i].ID = num;
				}
				report.LastID = num;
			}

			private void DetermineCurrentMaxID(Report report, ref List<DataSet> datasets, ref int maxID)
			{
				if (report != null)
				{
					maxID += report.LastID;
					if (report.DataSources != null)
					{
						for (int i = 0; i < report.DataSources.Count; i++)
						{
							DataSource dataSource = report.DataSources[i];
							if (dataSource.DataSets != null)
							{
								for (int j = 0; j < dataSource.DataSets.Count; j++)
								{
									DataSet dataSet = dataSource.DataSets[j];
									if (dataSet.ID <= 0)
									{
										datasets.Add(dataSet);
									}
								}
							}
						}
					}
					if (report.SubReports != null)
					{
						for (int k = 0; k < report.SubReports.Count; k++)
						{
							if (report.SubReports[k].Report != null)
							{
								this.DetermineCurrentMaxID(report.SubReports[k].Report, ref datasets, ref maxID);
							}
						}
					}
				}
			}
		}

		private sealed class BookmarkDrillthroughUpgrader
		{
			private ChunkManager.RenderingChunkManager m_chunkManager;

			private ChunkManager.UpgradeManager m_upgradeManager;

			private BookmarksHashtable m_bookmarks;

			private ReportDrillthroughInfo m_drillthroughs;

			internal void Upgrade(ReportSnapshot reportSnapshot, ChunkManager.RenderingChunkManager chunkManager, ChunkManager.UpgradeManager upgradeManager)
			{
				Global.Tracer.Assert(chunkManager != null && upgradeManager != null && null != reportSnapshot, "(null != chunkManager && null != upgradeManager && null != reportSnapshot)");
				this.m_chunkManager = chunkManager;
				this.m_upgradeManager = upgradeManager;
				this.m_bookmarks = new BookmarksHashtable();
				this.m_drillthroughs = new ReportDrillthroughInfo();
				this.ProcessReport(reportSnapshot.Report, reportSnapshot.ReportInstance, 0, reportSnapshot.ReportInstance.NumberOfPages);
				bool flag = 0 != this.m_bookmarks.Count;
				if (reportSnapshot.HasBookmarks)
				{
					Global.Tracer.Assert(flag, "(hasBookmarks)");
					reportSnapshot.BookmarksInfo = this.m_bookmarks;
				}
				else if (flag)
				{
					reportSnapshot.HasBookmarks = true;
					reportSnapshot.BookmarksInfo = this.m_bookmarks;
				}
				else
				{
					reportSnapshot.HasBookmarks = false;
					reportSnapshot.BookmarksInfo = null;
				}
				if (this.m_drillthroughs.Count == 0)
				{
					reportSnapshot.DrillthroughInfo = null;
				}
				else
				{
					reportSnapshot.DrillthroughInfo = this.m_drillthroughs;
				}
			}

			private void ProcessReport(Report report, ReportInstance reportInstance, int startPage, int endPage)
			{
				long chunkOffset = reportInstance.ChunkOffset;
				ReportItemInstanceInfo instanceInfo = reportInstance.GetInstanceInfo(this.m_chunkManager);
				this.m_upgradeManager.AddInstance(instanceInfo, reportInstance, chunkOffset);
				this.ProcessReportItemColInstance(reportInstance.ReportItemColInstance, startPage, endPage);
			}

			private void ProcessReportItemColInstance(ReportItemColInstance reportItemColInstance, int startPage, int endPage)
			{
				if (reportItemColInstance != null)
				{
					long chunkOffset = reportItemColInstance.ChunkOffset;
					ReportItemColInstanceInfo instanceInfo = reportItemColInstance.GetInstanceInfo(this.m_chunkManager, false);
					this.m_upgradeManager.AddInstance(instanceInfo, reportItemColInstance, chunkOffset);
					reportItemColInstance.ChildrenNonComputedUniqueNames = instanceInfo.ChildrenNonComputedUniqueNames;
					Global.Tracer.Assert(null != reportItemColInstance.ReportItemColDef, "(null != reportItemColInstance.ReportItemColDef)");
					ReportItemIndexerList sortedReportItems = reportItemColInstance.ReportItemColDef.SortedReportItems;
					if (sortedReportItems != null)
					{
						int count = sortedReportItems.Count;
						for (int i = 0; i < count; i++)
						{
							int startPage2 = startPage;
							int endPage2 = endPage;
							if (reportItemColInstance.ChildrenStartAndEndPages != null)
							{
								if (0 <= reportItemColInstance.ChildrenStartAndEndPages[i].StartPage)
								{
									startPage2 = reportItemColInstance.ChildrenStartAndEndPages[i].StartPage;
								}
								if (0 <= reportItemColInstance.ChildrenStartAndEndPages[i].EndPage)
								{
									endPage2 = reportItemColInstance.ChildrenStartAndEndPages[i].EndPage;
								}
							}
							if (sortedReportItems[i].IsComputed)
							{
								this.ProcessReportItemInstance(reportItemColInstance[sortedReportItems[i].Index], startPage2, endPage2);
							}
							else
							{
								Global.Tracer.Assert(reportItemColInstance.ChildrenNonComputedUniqueNames != null && sortedReportItems[i].Index < reportItemColInstance.ChildrenNonComputedUniqueNames.Length);
								this.ProcessReportItem(reportItemColInstance.ReportItemColDef[i], reportItemColInstance.ChildrenNonComputedUniqueNames[sortedReportItems[i].Index], startPage2);
							}
						}
					}
				}
			}

			private void ProcessReportItem(ReportItem reportItem, NonComputedUniqueNames uniqueName, int startPage)
			{
				if (reportItem.Bookmark != null && reportItem.Bookmark.Value != null)
				{
					this.m_bookmarks.Add(reportItem.Bookmark.Value, startPage, uniqueName.UniqueName.ToString(CultureInfo.InvariantCulture));
				}
				Rectangle rectangle = reportItem as Rectangle;
				if (rectangle != null && rectangle.ReportItems != null && rectangle.ReportItems.Count != 0)
				{
					Global.Tracer.Assert(uniqueName.ChildrenUniqueNames != null && rectangle.ReportItems.Count == uniqueName.ChildrenUniqueNames.Length);
					for (int i = 0; i < rectangle.ReportItems.Count; i++)
					{
						this.ProcessReportItem(rectangle.ReportItems[i], uniqueName.ChildrenUniqueNames[i], startPage);
					}
				}
				else
				{
					Image image = reportItem as Image;
					if (image != null && image.Action != null)
					{
						this.ProcessNonComputedAction(image.Action, uniqueName.UniqueName);
					}
					else
					{
						TextBox textBox = reportItem as TextBox;
						if (textBox != null && textBox.Action != null)
						{
							this.ProcessNonComputedAction(textBox.Action, uniqueName.UniqueName);
						}
					}
				}
			}

			private void ProcessAction(Action action, ActionInstance actionInstance, int uniqueName)
			{
				if (action != null)
				{
					if (actionInstance != null)
					{
						this.ProcessComputedAction(action, actionInstance, uniqueName);
					}
					else
					{
						this.ProcessNonComputedAction(action, uniqueName);
					}
				}
			}

			private void ProcessComputedAction(Action action, ActionInstance actionInstance, int uniqueName)
			{
				if (action != null && actionInstance != null)
				{
					int count = action.ActionItems.Count;
					for (int i = 0; i < count; i++)
					{
						int computedIndex = action.ActionItems[i].ComputedIndex;
						if (computedIndex >= 0)
						{
							Global.Tracer.Assert(computedIndex < action.ComputedActionItemsCount && computedIndex < actionInstance.ActionItemsValues.Count);
							this.ProcessComputedActionItem(action.ActionItems[i], actionInstance.ActionItemsValues[computedIndex], uniqueName, i);
						}
						else
						{
							this.ProcessNonComputedActionItem(action.ActionItems[i], uniqueName, i);
						}
					}
				}
			}

			private void ProcessNonComputedAction(Action action, int uniqueName)
			{
				if (action != null && action.ActionItems != null)
				{
					int count = action.ActionItems.Count;
					for (int i = 0; i < count; i++)
					{
						this.ProcessNonComputedActionItem(action.ActionItems[i], uniqueName, i);
					}
				}
			}

			private void ProcessNonComputedActionItem(ActionItem actionItem, int uniqueName, int index)
			{
				if (actionItem.DrillthroughReportName != null)
				{
					Global.Tracer.Assert(actionItem.DrillthroughReportName.Type == ExpressionInfo.Types.Constant, "(actionItem.DrillthroughReportName.Type == ExpressionInfo.Types.Constant)");
					DrillthroughParameters drillthroughParameters = null;
					if (actionItem.DrillthroughParameters != null)
					{
						ParameterValue parameterValue = null;
						for (int i = 0; i < actionItem.DrillthroughParameters.Count; i++)
						{
							parameterValue = actionItem.DrillthroughParameters[i];
							if (parameterValue.Omit != null)
							{
								Global.Tracer.Assert(parameterValue.Omit.Type == ExpressionInfo.Types.Constant, "(paramValue.Omit.Type == ExpressionInfo.Types.Constant)");
								if (!parameterValue.Omit.BoolValue)
								{
									goto IL_007d;
								}
								continue;
							}
							goto IL_007d;
							IL_007d:
							Global.Tracer.Assert(parameterValue.Value.Type == ExpressionInfo.Types.Constant, "(paramValue.Value.Type == ExpressionInfo.Types.Constant)");
							if (drillthroughParameters == null)
							{
								drillthroughParameters = new DrillthroughParameters();
							}
							drillthroughParameters.Add(parameterValue.Name, parameterValue.Value.Value);
						}
					}
					DrillthroughInformation drillthroughInfo = new DrillthroughInformation(actionItem.DrillthroughReportName.Value, drillthroughParameters, null);
					string drillthroughId = uniqueName.ToString(CultureInfo.InvariantCulture) + ":" + index.ToString(CultureInfo.InvariantCulture);
					this.m_drillthroughs.AddDrillthrough(drillthroughId, drillthroughInfo);
				}
			}

			private void ProcessComputedActionItem(ActionItem actionItem, ActionItemInstance actionInstance, int uniqueName, int index)
			{
				Global.Tracer.Assert(null != actionItem, "(null != actionItem)");
				if (actionItem.DrillthroughReportName != null)
				{
					string text = null;
					if (ExpressionInfo.Types.Constant == actionItem.DrillthroughReportName.Type)
					{
						text = actionItem.DrillthroughReportName.Value;
					}
					else
					{
						Global.Tracer.Assert(null != actionInstance, "(null != actionInstance)");
						text = actionInstance.DrillthroughReportName;
					}
					if (text != null)
					{
						DrillthroughParameters drillthroughParameters = null;
						if (actionItem.DrillthroughParameters != null)
						{
							int count = actionItem.DrillthroughParameters.Count;
							Global.Tracer.Assert(count == actionInstance.DrillthroughParametersOmits.Count && count == actionInstance.DrillthroughParametersValues.Length && count == actionItem.DrillthroughParameters.Count);
							for (int i = 0; i < count; i++)
							{
								if (!actionInstance.DrillthroughParametersOmits[i])
								{
									if (drillthroughParameters == null)
									{
										drillthroughParameters = new DrillthroughParameters();
									}
									drillthroughParameters.Add(actionItem.DrillthroughParameters[i].Name, actionInstance.DrillthroughParametersValues[i]);
								}
							}
						}
						DrillthroughInformation drillthroughInfo = new DrillthroughInformation(text, drillthroughParameters, null);
						string drillthroughId = uniqueName.ToString(CultureInfo.InvariantCulture) + ":" + index.ToString(CultureInfo.InvariantCulture);
						this.m_drillthroughs.AddDrillthrough(drillthroughId, drillthroughInfo);
					}
				}
			}

			private void ProcessReportItemInstance(ReportItemInstance reportItemInstance, int startPage, int endPage)
			{
				Global.Tracer.Assert(null != reportItemInstance, "(null != reportItemInstance)");
				ReportItem reportItemDef = reportItemInstance.ReportItemDef;
				long chunkOffset = reportItemInstance.ChunkOffset;
				ReportItemInstanceInfo instanceInfo = reportItemInstance.GetInstanceInfo(this.m_chunkManager);
				if (reportItemDef is TextBox)
				{
					bool flag = default(bool);
					SimpleTextBoxInstanceInfo instanceInfo2 = ((TextBoxInstance)reportItemInstance).UpgradeToSimpleTextbox(instanceInfo as TextBoxInstanceInfo, out flag);
					if (flag)
					{
						this.m_upgradeManager.AddInstance(instanceInfo2, reportItemInstance, chunkOffset);
						return;
					}
				}
				this.m_upgradeManager.AddInstance(instanceInfo, reportItemInstance, chunkOffset);
				if (instanceInfo.Bookmark != null)
				{
					this.m_bookmarks.Add(instanceInfo.Bookmark, startPage, reportItemInstance.UniqueName.ToString(CultureInfo.InvariantCulture));
				}
				if (reportItemDef is Rectangle)
				{
					this.ProcessReportItemColInstance(((RectangleInstance)reportItemInstance).ReportItemColInstance, startPage, endPage);
				}
				else if (reportItemDef is Image)
				{
					Image image = reportItemDef as Image;
					if (image.Action != null)
					{
						this.ProcessAction(image.Action, ((ImageInstanceInfo)instanceInfo).Action, reportItemInstance.UniqueName);
					}
				}
				else if (reportItemDef is TextBox)
				{
					TextBox textBox = reportItemDef as TextBox;
					if (textBox.Action != null)
					{
						this.ProcessAction(textBox.Action, ((TextBoxInstanceInfo)instanceInfo).Action, reportItemInstance.UniqueName);
					}
				}
				else if (reportItemDef is SubReport)
				{
					SubReport subReport = (SubReport)reportItemDef;
					SubReportInstance subReportInstance = (SubReportInstance)reportItemInstance;
					if (subReportInstance.ReportInstance != null)
					{
						this.ProcessReport(subReport.Report, subReportInstance.ReportInstance, startPage, endPage);
					}
				}
				else if (reportItemDef is DataRegion)
				{
					if (reportItemDef is List)
					{
						this.ProcessList((List)reportItemDef, (ListInstance)reportItemInstance, startPage);
					}
					else if (reportItemDef is Matrix)
					{
						this.ProcessMatrix((Matrix)reportItemDef, (MatrixInstance)reportItemInstance, startPage, endPage);
					}
					else if (reportItemDef is Table)
					{
						this.ProcessTable((Table)reportItemDef, (TableInstance)reportItemInstance, startPage, endPage);
					}
					else if (reportItemDef is Chart)
					{
						this.ProcessChart((Chart)reportItemDef, (ChartInstance)reportItemInstance, startPage);
					}
					else if (reportItemDef is CustomReportItem)
					{
						this.ProcessCustomReportItem((CustomReportItem)reportItemDef, (CustomReportItemInstance)reportItemInstance, startPage, endPage);
					}
					else if (reportItemDef is OWCChart || !(reportItemDef is ActiveXControl))
					{
						;
					}
				}
			}

			private void ProcessCustomReportItem(CustomReportItem criDef, CustomReportItemInstance criInstance, int startPage, int endPage)
			{
				ReportItem reportItem = null;
				Global.Tracer.Assert(null != criDef.AltReportItem, "(null != criDef.AltReportItem)");
				if (criDef.RenderReportItem != null && 1 == criDef.RenderReportItem.Count)
				{
					reportItem = criDef.RenderReportItem[0];
				}
				else if (criDef.AltReportItem != null && 1 == criDef.AltReportItem.Count)
				{
					Global.Tracer.Assert(null == criDef.RenderReportItem, "(null == criDef.RenderReportItem)");
					reportItem = criDef.AltReportItem[0];
				}
				if (reportItem != null && criInstance.AltReportItemColInstance != null)
				{
					Global.Tracer.Assert(criInstance.AltReportItemColInstance.ReportItemInstances != null && 1 == criInstance.AltReportItemColInstance.ReportItemInstances.Count);
					this.ProcessReportItemInstance(criInstance.AltReportItemColInstance.ReportItemInstances[0], startPage, endPage);
				}
			}

			private void ProcessChart(Chart chartDef, ChartInstance chartInstance, int startPage)
			{
				this.ProcessChartHeadings(chartInstance.RowInstances);
				this.ProcessChartHeadings(chartInstance.ColumnInstances);
				this.ProcessChartDataPoints(chartDef, chartInstance, startPage);
			}

			private void ProcessChartHeadings(ChartHeadingInstanceList headings)
			{
				if (headings != null)
				{
					for (int i = 0; i < headings.Count; i++)
					{
						ChartHeadingInstance chartHeadingInstance = headings[i];
						Global.Tracer.Assert(null != chartHeadingInstance, "(null != headingInstance)");
						long chunkOffset = chartHeadingInstance.ChunkOffset;
						ChartHeadingInstanceInfo instanceInfo = chartHeadingInstance.GetInstanceInfo(this.m_chunkManager);
						this.m_upgradeManager.AddInstance(instanceInfo, chartHeadingInstance, chunkOffset);
						if (chartHeadingInstance.SubHeadingInstances != null)
						{
							this.ProcessChartHeadings(chartHeadingInstance.SubHeadingInstances);
						}
					}
				}
			}

			private void ProcessChartDataPoints(Chart chartDef, ChartInstance chartInstance, int startPage)
			{
				if (chartInstance.DataPoints != null)
				{
					for (int i = 0; i < chartInstance.DataPointSeriesCount; i++)
					{
						for (int j = 0; j < chartInstance.DataPointCategoryCount; j++)
						{
							ChartDataPointInstance chartDataPointInstance = chartInstance.DataPoints[i][j];
							Global.Tracer.Assert(null != chartDataPointInstance, "(null != instance)");
							long chunkOffset = chartDataPointInstance.ChunkOffset;
							ChartDataPointInstanceInfo instanceInfo = chartDataPointInstance.GetInstanceInfo(this.m_chunkManager, chartDef.ChartDataPoints);
							this.m_upgradeManager.AddInstance(instanceInfo, chartDataPointInstance, chunkOffset);
							if (instanceInfo.Action != null)
							{
								Global.Tracer.Assert(instanceInfo.DataPointIndex < chartDef.ChartDataPoints.Count, "(instanceInfo.DataPointIndex < chartDef.ChartDataPoints.Count)");
								Action action = chartDef.ChartDataPoints[instanceInfo.DataPointIndex].Action;
								Global.Tracer.Assert(null != action, "(null != actionDef)");
								this.ProcessAction(action, instanceInfo.Action, chartDataPointInstance.UniqueName);
							}
						}
					}
				}
			}

			private void ProcessList(List listDef, ListInstance listInstance, int startPage)
			{
				if (listInstance.ListContents != null)
				{
					if (listDef.Grouping != null)
					{
						Global.Tracer.Assert(listInstance.ListContents.Count == listInstance.ChildrenStartAndEndPages.Count, "(listInstance.ListContents.Count == listInstance.ChildrenStartAndEndPages.Count)");
						for (int i = 0; i < listInstance.ChildrenStartAndEndPages.Count; i++)
						{
							ListContentInstance listContentInstance = listInstance.ListContents[i];
							long chunkOffset = listContentInstance.ChunkOffset;
							ListContentInstanceInfo instanceInfo = listContentInstance.GetInstanceInfo(this.m_chunkManager);
							this.m_upgradeManager.AddInstance(instanceInfo, listContentInstance, chunkOffset);
							RenderingPagesRanges renderingPagesRanges = listInstance.ChildrenStartAndEndPages[i];
							this.ProcessReportItemColInstance(listContentInstance.ReportItemColInstance, renderingPagesRanges.StartPage, renderingPagesRanges.EndPage);
						}
					}
					else if (listInstance.ChildrenStartAndEndPages == null)
					{
						this.ProcessListContents(listDef, listInstance, startPage, 0, listInstance.ListContents.Count - 1);
					}
					else
					{
						for (int j = 0; j < listInstance.ChildrenStartAndEndPages.Count; j++)
						{
							RenderingPagesRanges renderingPagesRanges2 = listInstance.ChildrenStartAndEndPages[j];
							this.ProcessListContents(listDef, listInstance, startPage + j, renderingPagesRanges2.StartRow, renderingPagesRanges2.StartRow + renderingPagesRanges2.NumberOfDetails - 1);
						}
					}
				}
			}

			private void ProcessListContents(List listDef, ListInstance listInstance, int page, int startRow, int endRow)
			{
				for (int i = startRow; i <= endRow; i++)
				{
					ListContentInstance listContentInstance = listInstance.ListContents[i];
					long chunkOffset = listContentInstance.ChunkOffset;
					ListContentInstanceInfo instanceInfo = listContentInstance.GetInstanceInfo(this.m_chunkManager);
					this.m_upgradeManager.AddInstance(instanceInfo, listContentInstance, chunkOffset);
					this.ProcessReportItemColInstance(listContentInstance.ReportItemColInstance, page, page);
				}
			}

			private void ProcessTable(Table tableDef, TableInstance tableInstance, int startPage, int endPage)
			{
				Global.Tracer.Assert(null != tableInstance, "(null != tableInstance)");
				this.ProcessTableRowInstances(tableInstance.HeaderRowInstances, startPage);
				this.ProcessTableRowInstances(tableInstance.FooterRowInstances, endPage);
				if (tableInstance.TableGroupInstances != null)
				{
					this.ProcessTableGroupInstances(tableInstance.TableGroupInstances, startPage, tableInstance.ChildrenStartAndEndPages);
				}
				else
				{
					this.ProcessTableDetailInstances(tableInstance.TableDetailInstances, startPage, tableInstance.ChildrenStartAndEndPages);
				}
			}

			private void ProcessTableRowInstances(TableRowInstance[] rowInstances, int startPage)
			{
				if (rowInstances != null)
				{
					foreach (TableRowInstance tableRowInstance in rowInstances)
					{
						Global.Tracer.Assert(null != tableRowInstance, "(null != rowInstance)");
						long chunkOffset = tableRowInstance.ChunkOffset;
						TableRowInstanceInfo instanceInfo = tableRowInstance.GetInstanceInfo(this.m_chunkManager);
						this.m_upgradeManager.AddInstance(instanceInfo, tableRowInstance, chunkOffset);
						this.ProcessReportItemColInstance(tableRowInstance.TableRowReportItemColInstance, startPage, startPage);
					}
				}
			}

			private void ProcessTableDetailInstances(TableDetailInstanceList detailInstances, int startPage, RenderingPagesRangesList instancePages)
			{
				if (detailInstances != null)
				{
					int count = instancePages.Count;
					int num = 0;
					for (int i = 0; i < count; i++)
					{
						RenderingPagesRanges renderingPagesRanges = instancePages[i];
						for (int j = 0; j < renderingPagesRanges.NumberOfDetails; j++)
						{
							TableDetailInstance tableDetailInstance = detailInstances[num + j];
							Global.Tracer.Assert(tableDetailInstance != null && null != tableDetailInstance.TableDetailDef);
							long chunkOffset = tableDetailInstance.ChunkOffset;
							TableDetailInstanceInfo instanceInfo = tableDetailInstance.GetInstanceInfo(this.m_chunkManager);
							this.m_upgradeManager.AddInstance(instanceInfo, tableDetailInstance, chunkOffset);
							this.ProcessTableRowInstances(tableDetailInstance.DetailRowInstances, startPage + i);
						}
						num += renderingPagesRanges.NumberOfDetails;
					}
				}
			}

			private void ProcessTableGroupInstances(TableGroupInstanceList groupInstances, int startPage, RenderingPagesRangesList groupInstancePages)
			{
				if (groupInstances != null)
				{
					for (int i = 0; i < groupInstances.Count; i++)
					{
						TableGroupInstance tableGroupInstance = groupInstances[i];
						Global.Tracer.Assert(null != tableGroupInstance, "(null != groupInstance)");
						long chunkOffset = tableGroupInstance.ChunkOffset;
						TableGroupInstanceInfo instanceInfo = tableGroupInstance.GetInstanceInfo(this.m_chunkManager);
						this.m_upgradeManager.AddInstance(instanceInfo, tableGroupInstance, chunkOffset);
						RenderingPagesRanges renderingPagesRanges = groupInstancePages[i];
						this.ProcessTableRowInstances(tableGroupInstance.HeaderRowInstances, renderingPagesRanges.StartPage);
						this.ProcessTableRowInstances(tableGroupInstance.FooterRowInstances, renderingPagesRanges.EndPage);
						if (tableGroupInstance.SubGroupInstances != null)
						{
							this.ProcessTableGroupInstances(tableGroupInstance.SubGroupInstances, renderingPagesRanges.StartPage, tableGroupInstance.ChildrenStartAndEndPages);
						}
						else
						{
							this.ProcessTableDetailInstances(tableGroupInstance.TableDetailInstances, renderingPagesRanges.StartPage, tableGroupInstance.ChildrenStartAndEndPages);
						}
					}
				}
			}

			private void ProcessMatrix(Matrix matrixDef, MatrixInstance matrixInstance, int startPage, int endPage)
			{
				if (matrixInstance.CornerContent != null)
				{
					this.ProcessReportItemInstance(matrixInstance.CornerContent, startPage, startPage);
				}
				int[] rowPages = new int[matrixInstance.CellRowCount];
				this.ProcessMatrixHeadings(matrixInstance.RowInstances, matrixInstance.ChildrenStartAndEndPages, ref rowPages, startPage, endPage);
				this.ProcessMatrixHeadings(matrixInstance.ColumnInstances, null, ref rowPages, startPage, endPage);
				this.ProcessMatrixCells(matrixInstance, rowPages);
			}

			private void ProcessMatrixCells(MatrixInstance matrixInstance, int[] rowPages)
			{
				if (matrixInstance.Cells != null)
				{
					for (int i = 0; i < matrixInstance.CellRowCount; i++)
					{
						for (int j = 0; j < matrixInstance.CellColumnCount; j++)
						{
							MatrixCellInstance matrixCellInstance = matrixInstance.Cells[i][j];
							Global.Tracer.Assert(null != matrixCellInstance, "(null != cellInstance)");
							long chunkOffset = matrixCellInstance.ChunkOffset;
							MatrixCellInstanceInfo instanceInfo = matrixCellInstance.GetInstanceInfo(this.m_chunkManager);
							this.m_upgradeManager.AddInstance(instanceInfo, matrixCellInstance, chunkOffset);
							if (matrixCellInstance.Content != null)
							{
								this.ProcessReportItemInstance(matrixCellInstance.Content, rowPages[i], rowPages[i]);
							}
							else
							{
								ReportItem cellReportItem = matrixInstance.MatrixDef.GetCellReportItem(instanceInfo.RowIndex, instanceInfo.ColumnIndex);
								if (cellReportItem != null)
								{
									this.ProcessReportItem(cellReportItem, instanceInfo.ContentUniqueNames, rowPages[i]);
								}
							}
						}
					}
				}
			}

			private void ProcessMatrixHeadings(MatrixHeadingInstanceList headings, RenderingPagesRangesList pagesList, ref int[] rowPages, int startPage, int endPage)
			{
				if (headings != null)
				{
					for (int i = 0; i < headings.Count; i++)
					{
						MatrixHeadingInstance matrixHeadingInstance = headings[i];
						Global.Tracer.Assert(null != matrixHeadingInstance, "(null != headingInstance)");
						long chunkOffset = matrixHeadingInstance.ChunkOffset;
						MatrixHeading matrixHeadingDef = matrixHeadingInstance.MatrixHeadingDef;
						MatrixHeadingInstanceInfo instanceInfo = matrixHeadingInstance.GetInstanceInfo(this.m_chunkManager);
						this.m_upgradeManager.AddInstance(instanceInfo, matrixHeadingInstance, chunkOffset);
						if (pagesList != null && pagesList.Count != 0)
						{
							Global.Tracer.Assert(headings.Count == pagesList.Count, "(headings.Count == pagesList.Count)");
							startPage = pagesList[i].StartPage;
							endPage = pagesList[i].EndPage;
						}
						if (!matrixHeadingDef.IsColumn)
						{
							Upgrader.SetRowPages(ref rowPages, instanceInfo.HeadingCellIndex, instanceInfo.HeadingSpan, startPage);
						}
						if (matrixHeadingInstance.Content != null)
						{
							this.ProcessReportItemInstance(matrixHeadingInstance.Content, startPage, endPage);
						}
						else
						{
							ReportItem reportItem = (matrixHeadingInstance.IsSubtotal && matrixHeadingDef.Subtotal != null) ? matrixHeadingDef.Subtotal.ReportItem : matrixHeadingDef.ReportItem;
							if (reportItem != null)
							{
								this.ProcessReportItem(reportItem, instanceInfo.ContentUniqueNames, startPage);
							}
						}
						if (matrixHeadingInstance.SubHeadingInstances != null)
						{
							this.ProcessMatrixHeadings(matrixHeadingInstance.SubHeadingInstances, matrixHeadingInstance.ChildrenStartAndEndPages, ref rowPages, startPage, endPage);
						}
					}
				}
			}
		}

		private sealed class PageSectionsGenerator
		{
			private ChunkManager.RenderingChunkManager m_chunkManager;

			private ReportProcessing.CreateReportChunk m_createChunkCallback;

			private ReportProcessing.PageSectionContext m_pageSectionContext;

			private ReportInstanceInfo m_reportInstanceInfo;

			internal void Upgrade(ReportSnapshot reportSnapshot, ICatalogItemContext reportContext, ChunkManager.RenderingChunkManager chunkManager, ReportProcessing.CreateReportChunk createChunkCallback, IGetResource getResourceCallback, RenderingContext renderingContext, IDataProtection dataProtection)
			{
				this.m_chunkManager = chunkManager;
				this.m_createChunkCallback = createChunkCallback;
				Report report = reportSnapshot.Report;
				this.m_pageSectionContext = new ReportProcessing.PageSectionContext(true, report != null && report.MergeOnePass);
				ReportInstance reportInstance = reportSnapshot.ReportInstance;
				this.ProcessReport(report, reportInstance);
				this.ProcessPageHeaderFooter(report, reportInstance, reportSnapshot, reportContext, createChunkCallback, getResourceCallback, renderingContext, dataProtection);
			}

			private void ProcessPageHeaderFooter(Report report, ReportInstance reportInstance, ReportSnapshot reportSnapshot, ICatalogItemContext reportContext, ReportProcessing.CreateReportChunk createChunkCallback, IGetResource getResourceCallback, RenderingContext renderingContext, IDataProtection dataProtection)
			{
				ChunkManager.ProcessingChunkManager processingChunkManager = new ChunkManager.ProcessingChunkManager(createChunkCallback, false);
				ReportProcessing.PageMergeInteractive pageMergeInteractive = new ReportProcessing.PageMergeInteractive();
				ProcessingErrorContext errorContext = new ProcessingErrorContext();
				UserProfileState allowUserProfileState = renderingContext.AllowUserProfileState;
				ReportRuntimeSetup reportRuntimeSetup = renderingContext.ReportRuntimeSetup;
				ReportDrillthroughInfo reportDrillthroughInfo = null;
				pageMergeInteractive.Process(this.m_pageSectionContext.PageTextboxes, reportSnapshot, reportContext, this.m_reportInstanceInfo.ReportName, reportSnapshot.Parameters, processingChunkManager, createChunkCallback, getResourceCallback, errorContext, allowUserProfileState, reportRuntimeSetup, 0, dataProtection, ref reportDrillthroughInfo);
				processingChunkManager.PageSectionFlush(reportSnapshot);
				processingChunkManager.Close();
			}

			private void ProcessReport(Report report, ReportInstance reportInstance)
			{
				this.m_reportInstanceInfo = (reportInstance.GetInstanceInfo(this.m_chunkManager) as ReportInstanceInfo);
				if (Visibility.IsVisible(report, reportInstance, this.m_reportInstanceInfo))
				{
					this.ProcessReportItemColInstance(reportInstance.ReportItemColInstance, report.StartPage, reportInstance.NumberOfPages - 1);
				}
			}

			private void ProcessReportItemColInstance(ReportItemColInstance reportItemColInstance, int startPage, int endPage)
			{
				this.ProcessReportItemColInstance(reportItemColInstance, startPage, endPage, null, null);
			}

			private void ProcessReportItemColInstance(ReportItemColInstance reportItemColInstance, int startPage, int endPage, bool[] tableColumnsVisible, IntList colSpans)
			{
				if (reportItemColInstance != null)
				{
					ReportItemColInstanceInfo instanceInfo = reportItemColInstance.GetInstanceInfo(this.m_chunkManager, false);
					reportItemColInstance.ChildrenNonComputedUniqueNames = instanceInfo.ChildrenNonComputedUniqueNames;
					Global.Tracer.Assert(null != reportItemColInstance.ReportItemColDef, "(null != reportItemColInstance.ReportItemColDef)");
					ReportItemIndexerList sortedReportItems = reportItemColInstance.ReportItemColDef.SortedReportItems;
					if (sortedReportItems != null)
					{
						int count = sortedReportItems.Count;
						List<DataRegion> list = new List<DataRegion>();
						int num = 0;
						for (int i = 0; i < count; i++)
						{
							if (colSpans != null)
							{
								Global.Tracer.Assert(tableColumnsVisible != null && colSpans.Count <= tableColumnsVisible.Length);
								bool flag = Visibility.IsTableCellVisible(tableColumnsVisible, num, colSpans[i]);
								num += colSpans[i];
								if (flag)
								{
									goto IL_00b3;
								}
								continue;
							}
							goto IL_00b3;
							IL_00b3:
							int num2 = startPage;
							int num3 = endPage;
							if (reportItemColInstance.ChildrenStartAndEndPages != null)
							{
								if (0 <= reportItemColInstance.ChildrenStartAndEndPages[i].StartPage)
								{
									num2 = reportItemColInstance.ChildrenStartAndEndPages[i].StartPage;
								}
								if (0 <= reportItemColInstance.ChildrenStartAndEndPages[i].EndPage)
								{
									num3 = reportItemColInstance.ChildrenStartAndEndPages[i].EndPage;
								}
							}
							ReportItem reportItem;
							if (sortedReportItems[i].IsComputed)
							{
								this.ProcessReportItemInstance(reportItemColInstance[sortedReportItems[i].Index], num2, num3, list);
								reportItem = reportItemColInstance[sortedReportItems[i].Index].ReportItemDef;
							}
							else
							{
								Global.Tracer.Assert(reportItemColInstance.ChildrenNonComputedUniqueNames != null && sortedReportItems[i].Index < reportItemColInstance.ChildrenNonComputedUniqueNames.Length);
								this.ProcessReportItem(reportItemColInstance.ReportItemColDef[i], reportItemColInstance.ChildrenNonComputedUniqueNames[sortedReportItems[i].Index], num2);
								reportItem = reportItemColInstance.ReportItemColDef[i];
							}
							Global.Tracer.Assert(null != reportItem, "(null != currentReportItem)");
							if (reportItem.RepeatedSiblingTextboxes != null)
							{
								this.m_pageSectionContext.PageTextboxes.IntegrateRepeatingTextboxValues(reportItem.RepeatedSiblingTextboxes, num2, num3);
							}
						}
						for (int j = 0; j < list.Count; j++)
						{
							DataRegion dataRegion = list[j];
							Global.Tracer.Assert(null != dataRegion.RepeatSiblings, "(null != dataRegion.RepeatSiblings)");
							for (int k = 0; k < dataRegion.RepeatSiblings.Count; k++)
							{
								ReportItem reportItem2 = reportItemColInstance.ReportItemColDef[dataRegion.RepeatSiblings[k]];
								Global.Tracer.Assert(null != reportItem2, "(null != sibling)");
								this.m_pageSectionContext.PageTextboxes.IntegrateRepeatingTextboxValues(reportItem2.RepeatedSiblingTextboxes, dataRegion.StartPage, dataRegion.EndPage);
							}
						}
					}
				}
			}

			private void ProcessReportItem(ReportItem reportItem, NonComputedUniqueNames uniqueName, int startPage)
			{
				if (Visibility.IsVisible(reportItem))
				{
					if (reportItem.RepeatedSibling)
					{
						this.m_pageSectionContext.EnterRepeatingItem();
					}
					Rectangle rectangle = reportItem as Rectangle;
					if (rectangle != null && rectangle.ReportItems != null && rectangle.ReportItems.Count != 0)
					{
						Global.Tracer.Assert(uniqueName.ChildrenUniqueNames != null && rectangle.ReportItems.Count == uniqueName.ChildrenUniqueNames.Length);
						for (int i = 0; i < rectangle.ReportItems.Count; i++)
						{
							this.ProcessReportItem(rectangle.ReportItems[i], uniqueName.ChildrenUniqueNames[i], startPage);
						}
					}
					else if (reportItem is TextBox)
					{
						this.ProcessTextbox(reportItem as TextBox, null, null, null, uniqueName.UniqueName, startPage);
					}
					if (reportItem.RepeatedSibling)
					{
						reportItem.RepeatedSiblingTextboxes = this.m_pageSectionContext.ExitRepeatingItem();
					}
				}
			}

			private void ProcessTextbox(TextBox textbox, TextBoxInstance textboxInstance, TextBoxInstanceInfo textboxInstanceInfo, SimpleTextBoxInstanceInfo simpleTextboxInstanceInfo, int uniqueName, int startPage)
			{
				if (textbox != null && this.m_pageSectionContext.IsParentVisible() && Visibility.IsVisible(textbox, textboxInstance, textboxInstanceInfo))
				{
					if (!this.m_pageSectionContext.InMatrixCell)
					{
						bool inRepeatingItem = this.m_pageSectionContext.InRepeatingItem;
					}
					if (textboxInstance != null)
					{
						if (textboxInstanceInfo != null)
						{
							this.m_pageSectionContext.PageTextboxes.AddTextboxValue(startPage, textbox.Name, textboxInstanceInfo.OriginalValue);
						}
						else if (simpleTextboxInstanceInfo != null)
						{
							this.m_pageSectionContext.PageTextboxes.AddTextboxValue(startPage, textbox.Name, simpleTextboxInstanceInfo.OriginalValue);
						}
					}
					else
					{
						Global.Tracer.Assert(textbox.Value != null && ExpressionInfo.Types.Constant == textbox.Value.Type);
						this.m_pageSectionContext.PageTextboxes.AddTextboxValue(startPage, textbox.Name, textbox.Value.Value);
					}
				}
			}

			private void ProcessReportItemInstance(ReportItemInstance reportItemInstance, int startPage, int endPage)
			{
				List<DataRegion> list = new List<DataRegion>();
				this.ProcessReportItemInstance(reportItemInstance, startPage, endPage, list);
				Global.Tracer.Assert(0 == list.Count, "(0 == dataRegionsWithRepeatSiblings.Count)");
			}

			private void ProcessReportItemInstance(ReportItemInstance reportItemInstance, int startPage, int endPage, List<DataRegion> dataRegionsWithRepeatSiblings)
			{
				Global.Tracer.Assert(null != reportItemInstance, "(null != reportItemInstance)");
				ReportItem reportItemDef = reportItemInstance.ReportItemDef;
				TextBox textBox = reportItemDef as TextBox;
				TextBoxInstance textBoxInstance = reportItemInstance as TextBoxInstance;
				ReportItemInstanceInfo reportItemInstanceInfo = null;
				SimpleTextBoxInstanceInfo simpleTextboxInstanceInfo = null;
				if (textBox != null && textBox.IsSimpleTextBox() && textBoxInstance != null)
				{
					simpleTextboxInstanceInfo = textBoxInstance.GetSimpleInstanceInfo(this.m_chunkManager, false);
				}
				else
				{
					reportItemInstanceInfo = reportItemInstance.GetInstanceInfo(this.m_chunkManager);
				}
				if (Visibility.IsVisible(reportItemDef, reportItemInstance, reportItemInstanceInfo))
				{
					if (reportItemDef.RepeatedSibling)
					{
						this.m_pageSectionContext.EnterRepeatingItem();
					}
					if (textBox != null)
					{
						this.ProcessTextbox(textBox, textBoxInstance, reportItemInstanceInfo as TextBoxInstanceInfo, simpleTextboxInstanceInfo, textBoxInstance.UniqueName, startPage);
					}
					else if (reportItemDef is Rectangle)
					{
						this.ProcessReportItemColInstance(((RectangleInstance)reportItemInstance).ReportItemColInstance, startPage, endPage);
					}
					else if (!(reportItemDef is SubReport) && reportItemDef is DataRegion)
					{
						DataRegion dataRegion = reportItemDef as DataRegion;
						if (dataRegion.RepeatSiblings != null)
						{
							dataRegion.StartPage = startPage;
							dataRegion.EndPage = endPage;
							dataRegionsWithRepeatSiblings.Add(reportItemDef as DataRegion);
						}
						if (reportItemDef is List)
						{
							this.ProcessList((List)reportItemDef, (ListInstance)reportItemInstance, startPage);
						}
						else if (reportItemDef is Matrix)
						{
							this.ProcessMatrix((Matrix)reportItemDef, (MatrixInstance)reportItemInstance, (MatrixInstanceInfo)reportItemInstanceInfo, startPage, endPage);
						}
						else if (reportItemDef is Table)
						{
							this.ProcessTable((Table)reportItemDef, (TableInstance)reportItemInstance, (TableInstanceInfo)reportItemInstanceInfo, startPage, endPage);
						}
					}
					if (reportItemDef.RepeatedSibling)
					{
						reportItemDef.RepeatedSiblingTextboxes = this.m_pageSectionContext.ExitRepeatingItem();
					}
				}
			}

			private void ProcessList(List listDef, ListInstance listInstance, int startPage)
			{
				if (listInstance.ListContents != null)
				{
					if (listDef.Grouping != null)
					{
						Global.Tracer.Assert(listInstance.ListContents.Count == listInstance.ChildrenStartAndEndPages.Count, "(listInstance.ListContents.Count == listInstance.ChildrenStartAndEndPages.Count)");
						for (int i = 0; i < listInstance.ChildrenStartAndEndPages.Count; i++)
						{
							ListContentInstance listContentInstance = listInstance.ListContents[i];
							ListContentInstanceInfo instanceInfo = listContentInstance.GetInstanceInfo(this.m_chunkManager);
							RenderingPagesRanges renderingPagesRanges = listInstance.ChildrenStartAndEndPages[i];
							if (Visibility.IsVisible(listDef.Visibility, instanceInfo.StartHidden))
							{
								this.ProcessReportItemColInstance(listContentInstance.ReportItemColInstance, renderingPagesRanges.StartPage, renderingPagesRanges.EndPage);
							}
						}
					}
					else if (listInstance.ChildrenStartAndEndPages == null)
					{
						this.ProcessListContents(listDef, listInstance, startPage, 0, listInstance.ListContents.Count - 1);
					}
					else
					{
						for (int j = 0; j < listInstance.ChildrenStartAndEndPages.Count; j++)
						{
							RenderingPagesRanges renderingPagesRanges2 = listInstance.ChildrenStartAndEndPages[j];
							this.ProcessListContents(listDef, listInstance, startPage + j, renderingPagesRanges2.StartRow, renderingPagesRanges2.StartRow + renderingPagesRanges2.NumberOfDetails - 1);
						}
					}
				}
			}

			private void ProcessListContents(List listDef, ListInstance listInstance, int page, int startRow, int endRow)
			{
				for (int i = startRow; i <= endRow; i++)
				{
					ListContentInstance listContentInstance = listInstance.ListContents[i];
					ListContentInstanceInfo instanceInfo = listContentInstance.GetInstanceInfo(this.m_chunkManager);
					if (Visibility.IsVisible(listDef.Visibility, instanceInfo.StartHidden))
					{
						this.ProcessReportItemColInstance(listContentInstance.ReportItemColInstance, page, page);
					}
				}
			}

			private void ProcessTable(Table tableDef, TableInstance tableInstance, TableInstanceInfo tableInstanceInfo, int startPage, int endPage)
			{
				Global.Tracer.Assert(tableInstance != null && null != tableDef.TableColumns, "(null != tableInstance && null != tableDef.TableColumns)");
				bool[] array = new bool[tableDef.TableColumns.Count];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = Visibility.IsVisible(tableDef.TableColumns[i].Visibility, tableInstanceInfo.ColumnInstances[i].StartHidden);
				}
				this.ProcessTableRowInstances(tableInstance.HeaderRowInstances, array, startPage, tableDef.HeaderRepeatOnNewPage, endPage);
				if (tableInstance.TableGroupInstances != null)
				{
					this.ProcessTableGroupInstances(tableInstance.TableGroupInstances, array, startPage, tableInstance.ChildrenStartAndEndPages);
				}
				else
				{
					this.ProcessTableDetailInstances(tableInstance.TableDetailInstances, array, startPage, tableInstance.ChildrenStartAndEndPages);
				}
				this.ProcessTableRowInstances(tableInstance.FooterRowInstances, array, endPage, tableDef.FooterRepeatOnNewPage, startPage);
			}

			private void ProcessTableRowInstances(TableRowInstance[] rowInstances, bool[] tableColumnsVisible, int startPage, bool repeat, int endPage)
			{
				if (!repeat)
				{
					this.ProcessTableRowInstances(rowInstances, tableColumnsVisible, startPage);
				}
				else
				{
					int num = Math.Min(startPage, endPage);
					int num2 = Math.Max(startPage, endPage);
					Global.Tracer.Assert(0 <= num && 0 <= num2, "(0 <= minPage && 0 <= maxPage)");
					for (int i = num; i <= num2; i++)
					{
						this.ProcessTableRowInstances(rowInstances, tableColumnsVisible, i);
					}
				}
			}

			private void ProcessTableRowInstances(TableRowInstance[] rowInstances, bool[] tableColumnsVisible, int startPage)
			{
				if (rowInstances != null)
				{
					foreach (TableRowInstance tableRowInstance in rowInstances)
					{
						Global.Tracer.Assert(tableRowInstance != null && null != tableRowInstance.TableRowDef);
						TableRowInstanceInfo instanceInfo = tableRowInstance.GetInstanceInfo(this.m_chunkManager);
						if (Visibility.IsVisible(tableRowInstance.TableRowDef.Visibility, instanceInfo.StartHidden))
						{
							this.ProcessReportItemColInstance(tableRowInstance.TableRowReportItemColInstance, startPage, startPage, tableColumnsVisible, tableRowInstance.TableRowDef.ColSpans);
						}
					}
				}
			}

			private void ProcessTableDetailInstances(TableDetailInstanceList detailInstances, bool[] tableColumnsVisible, int startPage, RenderingPagesRangesList instancePages)
			{
				if (detailInstances != null)
				{
					Global.Tracer.Assert(null != instancePages, "(null != instancePages)");
					int count = instancePages.Count;
					int num = 0;
					for (int i = 0; i < count; i++)
					{
						RenderingPagesRanges renderingPagesRanges = instancePages[i];
						for (int j = 0; j < renderingPagesRanges.NumberOfDetails; j++)
						{
							TableDetailInstance tableDetailInstance = detailInstances[num + j];
							Global.Tracer.Assert(tableDetailInstance != null && null != tableDetailInstance.TableDetailDef);
							TableDetailInstanceInfo instanceInfo = tableDetailInstance.GetInstanceInfo(this.m_chunkManager);
							if (Visibility.IsVisible(tableDetailInstance.TableDetailDef.Visibility, instanceInfo.StartHidden))
							{
								this.ProcessTableRowInstances(tableDetailInstance.DetailRowInstances, tableColumnsVisible, startPage + i);
							}
						}
						num += renderingPagesRanges.NumberOfDetails;
					}
				}
			}

			private void ProcessTableGroupInstances(TableGroupInstanceList groupInstances, bool[] tableColumnsVisible, int startPage, RenderingPagesRangesList groupInstancePages)
			{
				if (groupInstances != null)
				{
					int count = groupInstances.Count;
					Global.Tracer.Assert(groupInstancePages != null && count == groupInstancePages.Count, "(null != groupInstancePages && instanceCount == groupInstancePages.Count)");
					for (int i = 0; i < count; i++)
					{
						TableGroupInstance tableGroupInstance = groupInstances[i];
						Global.Tracer.Assert(tableGroupInstance != null && null != tableGroupInstance.TableGroupDef);
						TableGroupInstanceInfo instanceInfo = tableGroupInstance.GetInstanceInfo(this.m_chunkManager);
						RenderingPagesRanges renderingPagesRanges = groupInstancePages[i];
						if (Visibility.IsVisible(tableGroupInstance.TableGroupDef.Visibility, instanceInfo.StartHidden))
						{
							this.ProcessTableRowInstances(tableGroupInstance.HeaderRowInstances, tableColumnsVisible, renderingPagesRanges.StartPage, tableGroupInstance.TableGroupDef.HeaderRepeatOnNewPage, renderingPagesRanges.EndPage);
							if (tableGroupInstance.SubGroupInstances != null)
							{
								this.ProcessTableGroupInstances(tableGroupInstance.SubGroupInstances, tableColumnsVisible, renderingPagesRanges.StartPage, tableGroupInstance.ChildrenStartAndEndPages);
							}
							else
							{
								this.ProcessTableDetailInstances(tableGroupInstance.TableDetailInstances, tableColumnsVisible, renderingPagesRanges.StartPage, tableGroupInstance.ChildrenStartAndEndPages);
							}
							this.ProcessTableRowInstances(tableGroupInstance.FooterRowInstances, tableColumnsVisible, renderingPagesRanges.EndPage, tableGroupInstance.TableGroupDef.FooterRepeatOnNewPage, renderingPagesRanges.StartPage);
						}
					}
				}
			}

			private void ProcessMatrix(Matrix matrixDef, MatrixInstance matrixInstance, MatrixInstanceInfo matrixInstanceInfo, int startPage, int endPage)
			{
				ReportProcessing.PageSectionContext pageSectionContext = this.m_pageSectionContext;
				this.m_pageSectionContext = new ReportProcessing.PageSectionContext(pageSectionContext);
				ReportProcessing.PageTextboxes source = null;
				if (matrixInstance.CornerContent != null)
				{
					this.m_pageSectionContext.EnterRepeatingItem();
					this.ProcessReportItemInstance(matrixInstance.CornerContent, startPage, endPage);
					source = this.m_pageSectionContext.ExitRepeatingItem();
				}
				matrixDef.InitializePageSectionProcessing();
				bool[] rowCanGetReferenced = new bool[matrixInstance.CellRowCount];
				int[] rowPages = new int[matrixInstance.CellRowCount];
				bool[] colCanGetReferenced = new bool[matrixInstance.CellColumnCount];
				this.ProcessMatrixHeadings(matrixInstance, matrixInstance.ColumnInstances, null, ref colCanGetReferenced, ref rowPages, startPage, endPage);
				this.ProcessMatrixHeadings(matrixInstance, matrixInstance.RowInstances, matrixInstance.ChildrenStartAndEndPages, ref rowCanGetReferenced, ref rowPages, startPage, endPage);
				this.ProcessMatrixCells(matrixInstance, rowCanGetReferenced, rowPages, colCanGetReferenced);
				this.m_pageSectionContext = pageSectionContext;
				this.m_pageSectionContext.PageTextboxes.IntegrateRepeatingTextboxValues(source, startPage, endPage);
				this.m_pageSectionContext.PageTextboxes.IntegrateRepeatingTextboxValues(matrixInstance.MatrixDef.ColumnHeaderPageTextboxes, startPage, endPage);
				this.m_pageSectionContext.PageTextboxes.IntegrateNonRepeatingTextboxValues(matrixInstance.MatrixDef.RowHeaderPageTextboxes);
				this.m_pageSectionContext.PageTextboxes.IntegrateNonRepeatingTextboxValues(matrixInstance.MatrixDef.CellPageTextboxes);
			}

			private void ProcessMatrixCells(MatrixInstance matrixInstance, bool[] rowCanGetReferenced, int[] rowPages, bool[] colCanGetReferenced)
			{
				if (matrixInstance.Cells != null)
				{
					this.m_pageSectionContext.EnterRepeatingItem();
					this.m_pageSectionContext.InMatrixCell = true;
					for (int i = 0; i < matrixInstance.CellRowCount; i++)
					{
						if (rowCanGetReferenced[i])
						{
							for (int j = 0; j < matrixInstance.CellColumnCount; j++)
							{
								if (colCanGetReferenced[j])
								{
									MatrixCellInstance matrixCellInstance = matrixInstance.Cells[i][j];
									Global.Tracer.Assert(null != matrixCellInstance, "(null != cellInstance)");
									MatrixCellInstanceInfo instanceInfo = matrixCellInstance.GetInstanceInfo(this.m_chunkManager);
									if (matrixCellInstance.Content != null)
									{
										this.ProcessReportItemInstance(matrixCellInstance.Content, rowPages[i], rowPages[i]);
									}
									else
									{
										ReportItem cellReportItem = matrixInstance.MatrixDef.GetCellReportItem(instanceInfo.RowIndex, instanceInfo.ColumnIndex);
										if (cellReportItem != null)
										{
											this.ProcessReportItem(cellReportItem, instanceInfo.ContentUniqueNames, rowPages[i]);
										}
									}
								}
							}
						}
					}
					this.m_pageSectionContext.InMatrixCell = false;
					matrixInstance.MatrixDef.CellPageTextboxes.IntegrateNonRepeatingTextboxValues(this.m_pageSectionContext.ExitRepeatingItem());
				}
			}

			private static void SetCellVisiblity(ref bool[] cellVisiblity, int start, int span, bool visible)
			{
				Global.Tracer.Assert(cellVisiblity != null && start >= 0 && span > 0 && start + span <= cellVisiblity.Length, "(null != cellVisiblity && start >= 0 && span > 0 && start + span <= cellVisiblity.Length)");
				for (int i = 0; i < span; i++)
				{
					cellVisiblity[start + i] = visible;
				}
			}

			private void ProcessMatrixHeadings(MatrixInstance matrixInstance, MatrixHeadingInstanceList headings, RenderingPagesRangesList pagesList, ref bool[] cellsCanGetReferenced, ref int[] rowPages, int startPage, int endPage)
			{
				if (headings != null)
				{
					for (int i = 0; i < headings.Count; i++)
					{
						MatrixHeadingInstance matrixHeadingInstance = headings[i];
						MatrixHeading matrixHeadingDef = matrixHeadingInstance.MatrixHeadingDef;
						MatrixHeadingInstanceInfo instanceInfo = matrixHeadingInstance.GetInstanceInfo(this.m_chunkManager);
						this.m_pageSectionContext.EnterMatrixHeadingScope(Visibility.IsVisible(matrixHeadingDef.Visibility, instanceInfo.StartHidden), matrixHeadingDef.IsColumn);
						if (pagesList != null && pagesList.Count != 0)
						{
							Global.Tracer.Assert(headings.Count == pagesList.Count, "(headings.Count == pagesList.Count)");
							startPage = pagesList[i].StartPage;
							endPage = pagesList[i].EndPage;
						}
						if (!matrixHeadingDef.IsColumn)
						{
							Upgrader.SetRowPages(ref rowPages, instanceInfo.HeadingCellIndex, instanceInfo.HeadingSpan, startPage);
						}
						this.m_pageSectionContext.EnterRepeatingItem();
						if (matrixHeadingInstance.Content != null)
						{
							this.ProcessReportItemInstance(matrixHeadingInstance.Content, startPage, endPage);
						}
						else
						{
							ReportItem reportItem = (matrixHeadingInstance.IsSubtotal && matrixHeadingDef.Subtotal != null) ? matrixHeadingDef.Subtotal.ReportItem : matrixHeadingDef.ReportItem;
							if (reportItem != null)
							{
								this.ProcessReportItem(reportItem, instanceInfo.ContentUniqueNames, startPage);
							}
						}
						if (matrixHeadingDef.IsColumn)
						{
							matrixInstance.MatrixDef.ColumnHeaderPageTextboxes.IntegrateNonRepeatingTextboxValues(this.m_pageSectionContext.ExitRepeatingItem());
						}
						else
						{
							matrixInstance.MatrixDef.RowHeaderPageTextboxes.IntegrateRepeatingTextboxValues(this.m_pageSectionContext.ExitRepeatingItem(), startPage, endPage);
						}
						if (matrixHeadingInstance.IsSubtotal)
						{
							this.m_pageSectionContext.EnterMatrixSubtotalScope(matrixHeadingDef.IsColumn);
						}
						PageSectionsGenerator.SetCellVisiblity(ref cellsCanGetReferenced, instanceInfo.HeadingCellIndex, instanceInfo.HeadingSpan, this.m_pageSectionContext.IsParentVisible());
						if (matrixHeadingInstance.SubHeadingInstances != null)
						{
							this.ProcessMatrixHeadings(matrixInstance, matrixHeadingInstance.SubHeadingInstances, matrixHeadingInstance.ChildrenStartAndEndPages, ref cellsCanGetReferenced, ref rowPages, startPage, endPage);
						}
						if (matrixHeadingInstance.IsSubtotal)
						{
							this.m_pageSectionContext.ExitMatrixHeadingScope(matrixHeadingDef.IsColumn);
						}
						this.m_pageSectionContext.ExitMatrixHeadingScope(matrixHeadingDef.IsColumn);
					}
				}
			}
		}

		private sealed class UpgraderForV1Beta2
		{
			private ReportProcessing.Pagination m_pagination;

			private ReportProcessing.NavigationInfo m_navigationInfo;

			private bool m_onePass;

			private ChunkManager.RenderingChunkManager m_chunkManager;

			private bool m_hasDocMap;

			internal void Upgrade(Report report)
			{
				ReportPublishing.CalculateChildrenPostions(report);
				ReportPublishing.CalculateChildrenDependencies(report);
			}

			internal void Upgrade(ReportSnapshot reportSnapshot, ChunkManager.RenderingChunkManager chunkManager, ReportProcessing.CreateReportChunk createChunkCallback)
			{
				Report report = reportSnapshot.Report;
				ReportInstance reportInstance = reportSnapshot.ReportInstance;
				this.Upgrade(report);
				this.m_pagination = new ReportProcessing.Pagination(report.InteractiveHeightValue);
				this.m_navigationInfo = new ReportProcessing.NavigationInfo();
				this.m_onePass = report.MergeOnePass;
				this.m_chunkManager = chunkManager;
				this.m_hasDocMap = reportSnapshot.HasDocumentMap;
				this.ProcessReport(report, reportInstance);
			}

			private void ProcessReport(Report report, ReportInstance reportInstance)
			{
				this.m_pagination.SetReportItemStartPage(report, false);
				this.ProcessReportItemColInstance(reportInstance.ReportItemColInstance);
				this.m_pagination.ProcessEndPage(reportInstance, report, false, false);
				reportInstance.NumberOfPages = report.EndPage + 1;
			}

			private void ProcessReportItemColInstance(ReportItemColInstance collectionInstance)
			{
				if (collectionInstance != null)
				{
					ReportItemCollection reportItemColDef = collectionInstance.ReportItemColDef;
					if (reportItemColDef != null && reportItemColDef.Count >= 1)
					{
						ReportItemColInstanceInfo instanceInfo = collectionInstance.GetInstanceInfo(this.m_chunkManager, false);
						collectionInstance.ChildrenNonComputedUniqueNames = instanceInfo.ChildrenNonComputedUniqueNames;
						ReportItem parent = reportItemColDef[0].Parent;
						int num = parent.StartPage;
						bool flag = false;
						if (parent is Report || parent is List || parent is Rectangle || parent is SubReport || parent is CustomReportItem)
						{
							flag = true;
							collectionInstance.ChildrenStartAndEndPages = new RenderingPagesRangesList();
						}
						int i = 0;
						int index = 0;
						for (; i < reportItemColDef.Count; i++)
						{
							bool flag2 = default(bool);
							int num2 = default(int);
							ReportItem reportItem = default(ReportItem);
							reportItemColDef.GetReportItem(i, out flag2, out num2, out reportItem);
							if (flag2)
							{
								reportItem = reportItemColDef.ComputedReportItems[num2];
								this.ProcessReportItemInstance(collectionInstance.ReportItemInstances[index]);
							}
							else
							{
								collectionInstance.SetPaginationForNonComputedChild(this.m_pagination, reportItem, parent);
								reportItem.ProcessNavigationAction(this.m_navigationInfo, collectionInstance.ChildrenNonComputedUniqueNames[num2], reportItem.StartPage);
							}
							num = Math.Max(num, reportItem.EndPage);
							if (flag)
							{
								RenderingPagesRanges renderingPagesRanges = default(RenderingPagesRanges);
								renderingPagesRanges.StartPage = reportItem.StartPage;
								renderingPagesRanges.EndPage = reportItem.EndPage;
								collectionInstance.ChildrenStartAndEndPages.Add(renderingPagesRanges);
							}
						}
						if (num > parent.EndPage)
						{
							parent.EndPage = num;
							this.m_pagination.SetCurrentPageHeight(parent, 1.0);
						}
					}
				}
			}

			private void ProcessReportItemInstance(ReportItemInstance reportItemInstance)
			{
				ReportItem reportItemDef = reportItemInstance.ReportItemDef;
				bool isContainer = reportItemDef is SubReport || reportItemDef is Rectangle || reportItemDef is DataRegion;
				this.m_pagination.EnterIgnorePageBreak(reportItemDef.Visibility, false);
				ReportItemInstanceInfo instanceInfo = reportItemInstance.GetInstanceInfo(this.m_chunkManager);
				reportItemDef.StartHidden = instanceInfo.StartHidden;
				this.m_pagination.EnterIgnoreHeight(reportItemDef.StartHidden);
				string label = instanceInfo.Label;
				if (reportItemDef is Rectangle)
				{
					Rectangle rectangle = (Rectangle)reportItemDef;
					RectangleInstance rectangleInstance = (RectangleInstance)reportItemInstance;
					bool softPageAtStart = this.m_pagination.CalculateSoftPageBreak(rectangle, 0.0, (double)rectangle.DistanceBeforeTop, false);
					this.m_pagination.SetReportItemStartPage(rectangle, softPageAtStart);
					if (label != null)
					{
						this.m_navigationInfo.EnterDocumentMapChildren();
					}
					this.ProcessReportItemColInstance(rectangleInstance.ReportItemColInstance);
					this.m_pagination.ProcessEndPage(rectangleInstance, reportItemDef, rectangle.PageBreakAtEnd || rectangle.PageBreakAtStart, false);
				}
				else if (reportItemDef is DataRegion)
				{
					DataRegion dataRegion = (DataRegion)reportItemDef;
					bool softPageAtStart2 = this.m_pagination.CalculateSoftPageBreak(reportItemDef, 0.0, (double)dataRegion.DistanceBeforeTop, false);
					this.m_pagination.SetReportItemStartPage(reportItemDef, softPageAtStart2);
					if (reportItemDef is List)
					{
						ListInstance listInstance = (ListInstance)reportItemInstance;
						List list = (List)reportItemDef;
						if (-1 == list.ContentStartPage)
						{
							list.ContentStartPage = list.StartPage;
						}
						if (label != null)
						{
							this.m_navigationInfo.EnterDocumentMapChildren();
						}
						this.ProcessList(list, listInstance);
						this.m_pagination.ProcessListRenderingPages(listInstance, list);
					}
					else if (reportItemDef is Matrix)
					{
						if (label != null)
						{
							this.m_navigationInfo.EnterDocumentMapChildren();
						}
						this.ProcessMatrix((Matrix)reportItemDef, (MatrixInstance)reportItemInstance);
					}
					else if (reportItemDef is Chart)
					{
						if (label != null)
						{
							this.m_navigationInfo.EnterDocumentMapChildren();
						}
						ChartInstance riInstance = (ChartInstance)reportItemInstance;
						this.m_pagination.ProcessEndPage(riInstance, reportItemDef, ((Chart)reportItemDef).PageBreakAtEnd, false);
					}
					else if (reportItemDef is Table)
					{
						if (label != null)
						{
							this.m_navigationInfo.EnterDocumentMapChildren();
						}
						TableInstance tableInstance = (TableInstance)reportItemInstance;
						Table tableDef = (Table)reportItemDef;
						this.ProcessTable(tableDef, tableInstance);
						this.m_pagination.ProcessTableRenderingPages(tableInstance, (Table)reportItemDef);
					}
					else if (reportItemDef is OWCChart)
					{
						if (label != null)
						{
							this.m_navigationInfo.EnterDocumentMapChildren();
						}
						this.m_pagination.ProcessEndPage((OWCChartInstance)reportItemInstance, reportItemDef, ((OWCChart)reportItemDef).PageBreakAtEnd, false);
					}
				}
				else
				{
					if (reportItemDef.Parent != null && (reportItemDef.Parent is Rectangle || reportItemDef.Parent is Report || reportItemDef.Parent is List))
					{
						bool softPageAtStart3 = this.m_pagination.CalculateSoftPageBreak(reportItemDef, 0.0, (double)reportItemDef.DistanceBeforeTop, false, false);
						this.m_pagination.SetReportItemStartPage(reportItemDef, softPageAtStart3);
					}
					if (reportItemDef is SubReport)
					{
						if (label != null)
						{
							this.m_navigationInfo.EnterDocumentMapChildren();
						}
						SubReport subReport = (SubReport)reportItemDef;
						SubReportInstance subReportInstance = (SubReportInstance)reportItemInstance;
						if (subReportInstance.ReportInstance != null)
						{
							this.ProcessReport(subReport.Report, subReportInstance.ReportInstance);
						}
						this.m_pagination.ProcessEndPage(subReportInstance, subReport, false, false);
					}
				}
				if (label != null)
				{
					this.m_navigationInfo.AddToDocumentMap(reportItemInstance.GetDocumentMapUniqueName(), isContainer, reportItemDef.StartPage, label);
				}
				if (reportItemDef.Parent != null)
				{
					if (reportItemDef.EndPage > reportItemDef.Parent.EndPage)
					{
						reportItemDef.Parent.EndPage = reportItemDef.EndPage;
						reportItemDef.Parent.BottomInEndPage = reportItemDef.BottomInEndPage;
						if (reportItemDef.Parent is List)
						{
							((List)reportItemDef.Parent).ContentStartPage = reportItemDef.EndPage;
						}
					}
					else if (reportItemDef.EndPage == reportItemDef.Parent.EndPage)
					{
						reportItemDef.Parent.BottomInEndPage = Math.Max(reportItemDef.Parent.BottomInEndPage, reportItemDef.BottomInEndPage);
					}
				}
				this.m_pagination.LeaveIgnorePageBreak(reportItemDef.Visibility, false);
			}

			private void ProcessList(List listDef, ListInstance listInstance)
			{
				listInstance.ChildrenStartAndEndPages = new RenderingPagesRangesList();
				this.m_pagination.EnterIgnorePageBreak(listDef.Visibility, false);
				if (listDef.Grouping != null)
				{
					this.ProcessListGroupContents(listDef, listInstance);
				}
				else
				{
					this.ProcessListDetailContents(listDef, listInstance);
				}
				this.m_pagination.LeaveIgnorePageBreak(listDef.Visibility, false);
			}

			private void ProcessListGroupContents(List listDef, ListInstance listInstance)
			{
				for (int i = 0; i < listInstance.ListContents.Count; i++)
				{
					ListContentInstance listContentInstance = listInstance.ListContents[i];
					ListContentInstanceInfo instanceInfo = listContentInstance.GetInstanceInfo(this.m_chunkManager);
					listDef.StartHidden = instanceInfo.StartHidden;
					this.m_pagination.EnterIgnoreHeight(listDef.StartHidden);
					string label = instanceInfo.Label;
					bool flag = false;
					if (i > 0)
					{
						flag = this.m_pagination.CalculateSoftPageBreak(null, 0.0, 0.0, false, listDef.Grouping.PageBreakAtStart);
						if (!this.m_pagination.IgnorePageBreak && (this.m_pagination.CanMoveToNextPage(listDef.Grouping.PageBreakAtStart) || flag))
						{
							listDef.ContentStartPage++;
							this.m_pagination.SetCurrentPageHeight(listDef, 0.0);
						}
					}
					RenderingPagesRanges renderingPagesRanges = default(RenderingPagesRanges);
					renderingPagesRanges.StartPage = listDef.ContentStartPage;
					if (listDef.Grouping.GroupLabel != null && label != null)
					{
						this.m_navigationInfo.EnterDocumentMapChildren();
					}
					int startPage = listDef.StartPage;
					listDef.StartPage = listDef.ContentStartPage;
					this.ProcessReportItemColInstance(listContentInstance.ReportItemColInstance);
					this.m_pagination.ProcessEndGroupPage(listDef.IsListMostInner ? listDef.HeightValue : 0.0, listDef.Grouping.PageBreakAtEnd, listDef, true, listDef.StartHidden);
					listDef.ContentStartPage = listDef.EndPage;
					listDef.StartPage = startPage;
					if (this.m_pagination.ShouldItemMoveToChildStartPage(listDef))
					{
						renderingPagesRanges.StartPage = listContentInstance.ReportItemColInstance.ChildrenStartAndEndPages[0].StartPage;
					}
					renderingPagesRanges.EndPage = listDef.EndPage;
					listInstance.ChildrenStartAndEndPages.Add(renderingPagesRanges);
					if (listDef.Grouping.GroupLabel != null && label != null)
					{
						this.m_navigationInfo.AddToDocumentMap(listContentInstance.UniqueName, true, startPage, label);
					}
				}
			}

			private void ProcessListDetailContents(List listDef, ListInstance listInstance)
			{
				double heightValue = listDef.HeightValue;
				for (int i = 0; i < listInstance.ListContents.Count; i++)
				{
					ListContentInstance listContentInstance = listInstance.ListContents[i];
					ListContentInstanceInfo instanceInfo = listContentInstance.GetInstanceInfo(this.m_chunkManager);
					listDef.StartHidden = instanceInfo.StartHidden;
					this.m_pagination.EnterIgnoreHeight(listDef.StartHidden);
					string label = instanceInfo.Label;
					if (!this.m_pagination.IgnoreHeight)
					{
						this.m_pagination.AddToCurrentPageHeight(listDef, heightValue);
					}
					if (!this.m_pagination.IgnorePageBreak && this.m_pagination.CurrentPageHeight >= this.m_pagination.PageHeight && listInstance.NumberOfContentsOnThisPage > 0)
					{
						RenderingPagesRanges renderingPagesRanges = default(RenderingPagesRanges);
						renderingPagesRanges.StartRow = i + 1 - listInstance.NumberOfContentsOnThisPage;
						renderingPagesRanges.NumberOfDetails = listInstance.NumberOfContentsOnThisPage;
						this.m_pagination.SetCurrentPageHeight(listDef, 0.0);
						listDef.ContentStartPage++;
						listDef.BottomInEndPage = 0.0;
						listInstance.ChildrenStartAndEndPages.Add(renderingPagesRanges);
						listInstance.NumberOfContentsOnThisPage = 1;
					}
					else
					{
						listInstance.NumberOfContentsOnThisPage++;
					}
					int startPage = listDef.StartPage;
					listDef.StartPage = listDef.ContentStartPage;
					this.m_pagination.EnterIgnorePageBreak(null, true);
					this.m_pagination.EnterIgnoreHeight(true);
					this.ProcessReportItemColInstance(listContentInstance.ReportItemColInstance);
					this.m_pagination.LeaveIgnoreHeight(true);
					this.m_pagination.LeaveIgnorePageBreak(null, true);
					this.m_pagination.ProcessEndGroupPage(0.0, false, listDef, listInstance.NumberOfContentsOnThisPage > 0, listDef.StartHidden);
					listDef.StartPage = startPage;
				}
				listDef.EndPage = Math.Max(listDef.ContentStartPage, listDef.EndPage);
			}

			private void ProcessTable(Table tableDef, TableInstance tableInstance)
			{
				tableInstance.ChildrenStartAndEndPages = new RenderingPagesRangesList();
				tableInstance.CurrentPage = tableDef.StartPage;
				tableDef.CurrentPage = tableDef.StartPage;
				this.m_pagination.InitProcessTableRenderingPages(tableInstance, tableDef);
				if (tableInstance.HeaderRowInstances != null)
				{
					for (int i = 0; i < tableInstance.HeaderRowInstances.Length; i++)
					{
						this.ProcessReportItemColInstance(tableInstance.HeaderRowInstances[i].TableRowReportItemColInstance);
					}
				}
				if (tableInstance.FooterRowInstances != null)
				{
					for (int j = 0; j < tableInstance.FooterRowInstances.Length; j++)
					{
						this.ProcessReportItemColInstance(tableInstance.FooterRowInstances[j].TableRowReportItemColInstance);
					}
				}
				if (tableInstance.TableGroupInstances != null)
				{
					this.ProcessTableGroups(tableDef, tableInstance, tableInstance.TableGroupInstances, tableInstance.ChildrenStartAndEndPages);
				}
				else if (tableInstance.TableDetailInstances != null)
				{
					int num2 = tableInstance.NumberOfChildrenOnThisPage = this.ProcessTableDetails(tableDef, tableInstance, tableDef.TableDetail, tableInstance.TableDetailInstances, tableInstance.ChildrenStartAndEndPages);
					if (num2 > 0)
					{
						RenderingPagesRanges renderingPagesRanges = default(RenderingPagesRanges);
						renderingPagesRanges.StartRow = tableInstance.TableDetailInstances.Count - num2;
						renderingPagesRanges.NumberOfDetails = num2;
						tableInstance.ChildrenStartAndEndPages.Add(renderingPagesRanges);
					}
				}
			}

			private void ProcessTableGroups(Table tableDef, TableInstance tableInstance, TableGroupInstanceList tableGroupInstances, RenderingPagesRangesList pagesList)
			{
				for (int i = 0; i < tableGroupInstances.Count; i++)
				{
					TableGroupInstance tableGroupInstance = tableGroupInstances[i];
					TableGroup tableGroupDef = tableGroupInstance.TableGroupDef;
					tableGroupInstance.ChildrenStartAndEndPages = new RenderingPagesRangesList();
					TableGroupInstanceInfo instanceInfo = tableGroupInstance.GetInstanceInfo(this.m_chunkManager);
					tableGroupDef.StartHidden = instanceInfo.StartHidden;
					this.m_pagination.EnterIgnoreHeight(tableGroupDef.StartHidden);
					string label = instanceInfo.Label;
					if (label != null)
					{
						this.m_navigationInfo.EnterDocumentMapChildren();
					}
					RenderingPagesRanges renderingPagesRanges = default(RenderingPagesRanges);
					this.m_pagination.InitProcessingTableGroup(tableInstance, tableDef, tableGroupInstance, tableGroupDef, ref renderingPagesRanges, 0 == i);
					if (tableGroupInstance.HeaderRowInstances != null)
					{
						for (int j = 0; j < tableGroupInstance.HeaderRowInstances.Length; j++)
						{
							this.ProcessReportItemColInstance(tableGroupInstance.HeaderRowInstances[j].TableRowReportItemColInstance);
						}
					}
					if (tableGroupInstance.FooterRowInstances != null)
					{
						for (int k = 0; k < tableGroupInstance.FooterRowInstances.Length; k++)
						{
							this.ProcessReportItemColInstance(tableGroupInstance.FooterRowInstances[k].TableRowReportItemColInstance);
						}
					}
					if (tableGroupInstance.SubGroupInstances != null)
					{
						this.ProcessTableGroups(tableDef, tableInstance, tableGroupInstance.SubGroupInstances, tableGroupInstance.ChildrenStartAndEndPages);
					}
					else if (tableGroupInstance.TableDetailInstances != null)
					{
						int num2 = tableGroupInstance.NumberOfChildrenOnThisPage = this.ProcessTableDetails(tableDef, tableInstance, tableDef.TableDetail, tableGroupInstance.TableDetailInstances, tableGroupInstance.ChildrenStartAndEndPages);
						if (num2 > 0)
						{
							RenderingPagesRanges renderingPagesRanges2 = default(RenderingPagesRanges);
							renderingPagesRanges2.StartRow = tableGroupInstance.TableDetailInstances.Count - num2;
							renderingPagesRanges2.NumberOfDetails = num2;
							tableGroupInstance.ChildrenStartAndEndPages.Add(renderingPagesRanges2);
						}
					}
					double footerHeightValue = tableGroupDef.FooterHeightValue;
					tableGroupDef.EndPage = tableInstance.CurrentPage;
					this.m_pagination.ProcessEndGroupPage(footerHeightValue, tableGroupDef.PropagatedPageBreakAtEnd || tableGroupDef.Grouping.PageBreakAtEnd, tableDef, tableGroupInstance.NumberOfChildrenOnThisPage > 0, tableGroupDef.StartHidden);
					renderingPagesRanges.EndPage = tableGroupDef.EndPage;
					pagesList.Add(renderingPagesRanges);
					this.m_pagination.LeaveIgnorePageBreak(tableGroupDef.Visibility, false);
				}
			}

			private int ProcessTableDetails(Table tableDef, TableInstance tableInstance, TableDetail detailDef, TableDetailInstanceList detailInstances, RenderingPagesRangesList pagesList)
			{
				TableRowList detailRows = detailDef.DetailRows;
				double num = -1.0;
				this.m_pagination.EnterIgnorePageBreak(detailRows[0].Visibility, false);
				int num2 = 0;
				for (int i = 0; i < detailInstances.Count; i++)
				{
					TableDetailInstance tableDetailInstance = detailInstances[i];
					TableDetailInstanceInfo instanceInfo = tableDetailInstance.GetInstanceInfo(this.m_chunkManager);
					detailDef.StartHidden = instanceInfo.StartHidden;
					this.m_pagination.EnterIgnoreHeight(detailDef.StartHidden);
					this.m_pagination.ProcessTableDetails(tableDef, tableDetailInstance, detailInstances, ref num, detailRows, pagesList, ref num2);
					tableInstance.CurrentPage = tableDef.CurrentPage;
					tableInstance.NumberOfChildrenOnThisPage = num2;
					this.m_pagination.EnterIgnorePageBreak(null, true);
					this.m_pagination.EnterIgnoreHeight(true);
					if (tableDetailInstance.DetailRowInstances != null)
					{
						for (int j = 0; j < tableDetailInstance.DetailRowInstances.Length; j++)
						{
							this.ProcessReportItemColInstance(tableDetailInstance.DetailRowInstances[i].TableRowReportItemColInstance);
						}
					}
					this.m_pagination.LeaveIgnorePageBreak(null, true);
					this.m_pagination.LeaveIgnoreHeight(true);
					this.m_pagination.LeaveIgnoreHeight(detailDef.StartHidden);
				}
				this.m_pagination.LeaveIgnorePageBreak(detailRows[0].Visibility, false);
				return num2;
			}

			private void ProcessMatrix(Matrix matrixDef, MatrixInstance matrixInstance)
			{
				matrixInstance.ChildrenStartAndEndPages = new RenderingPagesRangesList();
				this.m_pagination.EnterIgnorePageBreak(matrixDef.Visibility, false);
				matrixDef.CurrentPage = matrixDef.StartPage;
				((IPageItem)matrixInstance).StartPage = matrixDef.StartPage;
				MatrixInstanceInfo matrixInstanceInfo = (MatrixInstanceInfo)matrixInstance.GetInstanceInfo(this.m_chunkManager);
				matrixDef.StartHidden = matrixInstanceInfo.StartHidden;
				this.m_pagination.EnterIgnoreHeight(matrixDef.StartHidden);
				if (matrixInstance.CornerContent != null)
				{
					this.ProcessReportItemInstance(matrixInstance.CornerContent);
				}
				else
				{
					ReportItem cornerReportItem = matrixDef.CornerReportItem;
					if (cornerReportItem != null)
					{
						NonComputedUniqueNames cornerNonComputedNames = matrixInstanceInfo.CornerNonComputedNames;
						cornerReportItem.ProcessNavigationAction(this.m_navigationInfo, cornerNonComputedNames, matrixDef.CurrentPage);
					}
				}
				this.ProcessMatrixColumnHeadings(matrixDef, matrixInstance, matrixInstance.ColumnInstances);
				this.ProcessMatrixRowHeadings(matrixDef, matrixInstance, matrixInstance.RowInstances, matrixInstance.ChildrenStartAndEndPages, 0, 0, 0);
				int count = matrixInstance.ChildrenStartAndEndPages.Count;
				if (count > 0)
				{
					matrixDef.EndPage = matrixInstance.ChildrenStartAndEndPages[count - 1].EndPage;
				}
				else
				{
					matrixDef.EndPage = ((IPageItem)matrixInstance).StartPage;
				}
				this.m_pagination.ProcessEndPage(matrixInstance, matrixDef, matrixDef.PageBreakAtEnd || matrixDef.PropagatedPageBreakAtEnd, matrixInstance.NumberOfChildrenOnThisPage > 0);
			}

			private void ProcessMatrixColumnHeadings(Matrix matrixDef, MatrixInstance matrixInstance, MatrixHeadingInstanceList headingInstances)
			{
				for (int i = 0; i < headingInstances.Count; i++)
				{
					MatrixHeadingInstance matrixHeadingInstance = headingInstances[i];
					MatrixHeading matrixHeadingDef = matrixHeadingInstance.MatrixHeadingDef;
					MatrixHeadingInstanceInfo instanceInfo = matrixHeadingInstance.GetInstanceInfo(this.m_chunkManager);
					this.ProcessMatrixHeadingContent(matrixDef, matrixHeadingDef, matrixHeadingInstance, instanceInfo);
					if (matrixHeadingInstance.SubHeadingInstances != null)
					{
						this.ProcessMatrixColumnHeadings(matrixDef, matrixInstance, matrixHeadingInstance.SubHeadingInstances);
					}
					if (instanceInfo.Label != null)
					{
						this.m_navigationInfo.AddToDocumentMap(matrixHeadingInstance.UniqueName, true, matrixDef.CurrentPage, instanceInfo.Label);
					}
				}
			}

			private void ProcessMatrixRowHeadings(Matrix matrixDef, MatrixInstance matrixInstance, MatrixHeadingInstanceList headingInstances, RenderingPagesRangesList pagesList, int rowDefIndex, int headingCellIndex, int rowIndex)
			{
				if (headingInstances == null)
				{
					if (!this.m_pagination.IgnoreHeight)
					{
						this.m_pagination.AddToCurrentPageHeight(matrixDef, matrixDef.MatrixRows[rowDefIndex].HeightValue);
					}
					if (!this.m_pagination.IgnorePageBreak && this.m_pagination.CurrentPageHeight >= this.m_pagination.PageHeight && matrixInstance.RowInstances.Count > 1)
					{
						this.m_pagination.SetCurrentPageHeight(matrixDef, 0.0);
						matrixInstance.ExtraPagesFilled++;
						matrixDef.CurrentPage++;
						matrixInstance.NumberOfChildrenOnThisPage = 0;
					}
					else
					{
						matrixInstance.NumberOfChildrenOnThisPage++;
					}
					for (int i = 0; i < matrixInstance.CellColumnCount; i++)
					{
						MatrixCellInstance matrixCellInstance = matrixInstance.Cells[rowIndex][i];
						ReportItemInstance content = matrixCellInstance.Content;
						if (content != null)
						{
							this.ProcessReportItemInstance(content);
						}
						else
						{
							MatrixCellInstanceInfo instanceInfo = matrixCellInstance.GetInstanceInfo(this.m_chunkManager);
							ReportItem cellReportItem = matrixDef.GetCellReportItem(instanceInfo.RowIndex, instanceInfo.ColumnIndex);
							if (cellReportItem != null)
							{
								NonComputedUniqueNames contentUniqueNames = instanceInfo.ContentUniqueNames;
								cellReportItem.ProcessNavigationAction(this.m_navigationInfo, contentUniqueNames, matrixDef.CurrentPage);
							}
						}
					}
				}
				else
				{
					for (int j = 0; j < headingInstances.Count; j++)
					{
						MatrixHeadingInstance matrixHeadingInstance = headingInstances[j];
						MatrixHeading matrixHeadingDef = matrixHeadingInstance.MatrixHeadingDef;
						matrixHeadingInstance.ChildrenStartAndEndPages = new RenderingPagesRangesList();
						MatrixHeadingInstanceInfo instanceInfo2 = matrixHeadingInstance.GetInstanceInfo(this.m_chunkManager);
						matrixHeadingDef.StartHidden = instanceInfo2.StartHidden;
						this.m_pagination.EnterIgnoreHeight(matrixHeadingDef.StartHidden);
						int startPage = (!matrixHeadingInstance.IsSubtotal && matrixHeadingDef.Grouping != null) ? this.ProcessMatrixDynamicRowHeading(matrixDef, matrixInstance, matrixHeadingDef, matrixHeadingInstance, instanceInfo2, 0 == j, pagesList, rowDefIndex, instanceInfo2.HeadingCellIndex, j) : this.ProcessMatrixRowSubtotalOrStaticHeading(matrixDef, matrixInstance, matrixHeadingDef, matrixHeadingInstance, instanceInfo2, pagesList, matrixHeadingInstance.IsSubtotal ? rowDefIndex : j, instanceInfo2.HeadingCellIndex, j);
						if (instanceInfo2.Label != null)
						{
							this.m_navigationInfo.AddToDocumentMap(matrixHeadingInstance.UniqueName, true, startPage, instanceInfo2.Label);
						}
					}
				}
			}

			private void ProcessMatrixHeadingContent(Matrix matrixDef, MatrixHeading headingDef, MatrixHeadingInstance headingInstance, MatrixHeadingInstanceInfo headingInstanceInfo)
			{
				if (headingInstanceInfo.Label != null)
				{
					this.m_navigationInfo.EnterDocumentMapChildren();
				}
				if (headingInstance.Content != null)
				{
					this.ProcessReportItemInstance(headingInstance.Content);
				}
				else
				{
					ReportItem reportItem = headingDef.ReportItem;
					if (reportItem != null)
					{
						NonComputedUniqueNames contentUniqueNames = headingInstanceInfo.ContentUniqueNames;
						reportItem.ProcessNavigationAction(this.m_navigationInfo, contentUniqueNames, matrixDef.CurrentPage);
					}
				}
			}

			private int ProcessMatrixRowSubtotalOrStaticHeading(Matrix matrixDef, MatrixInstance matrixInstance, MatrixHeading headingDef, MatrixHeadingInstance headingInstance, MatrixHeadingInstanceInfo headingInstanceInfo, RenderingPagesRangesList pagesList, int rowDefIndex, int headingCellIndex, int rowIndex)
			{
				RenderingPagesRanges renderingPagesRanges = default(RenderingPagesRanges);
				bool isSubtotal = headingInstance.IsSubtotal;
				this.m_pagination.EnterIgnorePageBreak(headingDef.Visibility, isSubtotal);
				renderingPagesRanges.StartPage = ((Matrix)matrixInstance.ReportItemDef).CurrentPage;
				this.ProcessMatrixHeadingContent(matrixDef, headingDef, headingInstance, headingInstanceInfo);
				this.ProcessMatrixRowHeadings(matrixDef, matrixInstance, headingInstance.SubHeadingInstances, headingInstance.ChildrenStartAndEndPages, rowDefIndex, headingCellIndex, rowIndex);
				this.m_pagination.LeaveIgnorePageBreak(headingDef.Visibility, isSubtotal);
				renderingPagesRanges.EndPage = ((Matrix)matrixInstance.ReportItemDef).CurrentPage;
				if (headingInstance.ChildrenStartAndEndPages == null || headingInstance.ChildrenStartAndEndPages.Count < 1)
				{
					renderingPagesRanges.EndPage = renderingPagesRanges.StartPage;
				}
				pagesList.Add(renderingPagesRanges);
				return renderingPagesRanges.StartPage;
			}

			private int ProcessMatrixDynamicRowHeading(Matrix matrixDef, MatrixInstance matrixInstance, MatrixHeading headingDef, MatrixHeadingInstance headingInstance, MatrixHeadingInstanceInfo headingInstanceInfo, bool firstHeading, RenderingPagesRangesList pagesList, int rowDefIndex, int headingCellIndex, int rowIndex)
			{
				RenderingPagesRanges renderingPagesRanges = default(RenderingPagesRanges);
				this.m_pagination.EnterIgnorePageBreak(headingDef.Visibility, false);
				if (!this.m_pagination.IgnorePageBreak && !firstHeading && headingDef.Grouping.PageBreakAtStart && matrixInstance.NumberOfChildrenOnThisPage > 0)
				{
					this.m_pagination.SetCurrentPageHeight(matrixInstance.ReportItemDef, 0.0);
					matrixInstance.ExtraPagesFilled++;
					matrixDef.CurrentPage++;
					matrixInstance.NumberOfChildrenOnThisPage = 0;
				}
				renderingPagesRanges.StartPage = matrixDef.CurrentPage;
				this.ProcessMatrixHeadingContent(matrixDef, headingDef, headingInstance, headingInstanceInfo);
				this.ProcessMatrixRowHeadings(matrixDef, matrixInstance, headingInstance.SubHeadingInstances, headingInstance.ChildrenStartAndEndPages, rowDefIndex, headingCellIndex, rowIndex);
				renderingPagesRanges.EndPage = ((Matrix)matrixInstance.ReportItemDef).CurrentPage;
				if (headingInstance.SubHeadingInstances == null || headingInstance.SubHeadingInstances.Count < 1)
				{
					renderingPagesRanges.EndPage = renderingPagesRanges.StartPage;
				}
				else
				{
					renderingPagesRanges.EndPage = headingInstance.ChildrenStartAndEndPages[headingInstance.ChildrenStartAndEndPages.Count - 1].EndPage;
				}
				if (!this.m_pagination.IgnorePageBreak && matrixInstance.NumberOfChildrenOnThisPage > 0 && this.m_pagination.CanMoveToNextPage(headingDef.Grouping.PageBreakAtEnd))
				{
					this.m_pagination.SetCurrentPageHeight(matrixDef, 0.0);
					matrixInstance.ExtraPagesFilled++;
					matrixDef.CurrentPage++;
					matrixInstance.NumberOfChildrenOnThisPage = 0;
				}
				pagesList.Add(renderingPagesRanges);
				this.m_pagination.LeaveIgnoreHeight(headingDef.StartHidden);
				this.m_pagination.LeaveIgnorePageBreak(headingDef.Visibility, false);
				return renderingPagesRanges.StartPage;
			}
		}

		internal static void UpgradeToCurrent(Report report)
		{
			if (report.IntermediateFormatVersion.IsRS2000_Beta2_orOlder)
			{
				UpgraderForV1Beta2 upgraderForV1Beta = new UpgraderForV1Beta2();
				upgraderForV1Beta.Upgrade(report);
			}
		}

		internal static bool UpgradeToCurrent(ReportSnapshot reportSnapshot, ChunkManager.RenderingChunkManager chunkManager, ReportProcessing.CreateReportChunk createChunkCallback)
		{
			if (reportSnapshot.Report.IntermediateFormatVersion.IsRS2000_Beta2_orOlder)
			{
				UpgraderForV1Beta2 upgraderForV1Beta = new UpgraderForV1Beta2();
				upgraderForV1Beta.Upgrade(reportSnapshot, chunkManager, createChunkCallback);
				return true;
			}
			return false;
		}

		internal static void UpgradeDatasetIDs(Report report)
		{
			if (!report.IntermediateFormatVersion.Is_WithUserSort)
			{
				DataSetUpgrader dataSetUpgrader = new DataSetUpgrader();
				dataSetUpgrader.Upgrade(report);
			}
		}

		internal static bool CreateBookmarkDrillthroughChunks(ReportSnapshot reportSnapshot, ChunkManager.RenderingChunkManager chunkManager, ChunkManager.UpgradeManager upgradeManager)
		{
			if (!reportSnapshot.Report.IntermediateFormatVersion.IsRS2005_WithSpecialChunkSplit)
			{
				BookmarkDrillthroughUpgrader bookmarkDrillthroughUpgrader = new BookmarkDrillthroughUpgrader();
				bookmarkDrillthroughUpgrader.Upgrade(reportSnapshot, chunkManager, upgradeManager);
				return true;
			}
			return false;
		}

		internal static void UpgradeToPageSectionsChunk(ReportSnapshot reportSnapshot, ICatalogItemContext reportContext, ChunkManager.RenderingChunkManager chunkManager, ReportProcessing.CreateReportChunk createChunkCallback, IGetResource getResourceCallback, RenderingContext renderingContext, IDataProtection dataProtection)
		{
			if (reportSnapshot.PageSectionOffsets == null)
			{
				if (!reportSnapshot.Report.PageHeaderEvaluation && !reportSnapshot.Report.PageFooterEvaluation)
				{
					return;
				}
				if (!chunkManager.PageSectionChunkExists())
				{
					PageSectionsGenerator pageSectionsGenerator = new PageSectionsGenerator();
					pageSectionsGenerator.Upgrade(reportSnapshot, reportContext, chunkManager, createChunkCallback, getResourceCallback, renderingContext, dataProtection);
				}
			}
		}

		private static void SetRowPages(ref int[] rowPages, int start, int span, int pageNumber)
		{
			Global.Tracer.Assert(rowPages != null && start >= 0 && span > 0 && start + span <= rowPages.Length, "(null != rowPages && start >= 0 && span > 0 && start + span <= rowPages.Length)");
			for (int i = 0; i < span; i++)
			{
				rowPages[start + i] = pageNumber;
			}
		}
	}
}
