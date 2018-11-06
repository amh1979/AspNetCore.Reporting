using AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel;
using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using AspNetCore.ReportingServices.ReportRendering;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class Matrix : Pivot, IPageBreakItem
	{
		private sealed class OWCFlagsCalculator
		{
			private bool m_useOWC = true;

			private StringList m_owcCellNames = new StringList();

			private BoolList m_owcGroupExpression = new BoolList();

			private int m_staticHeadingCount;

			private OWCFlagsCalculator()
			{
			}

			internal static void Calculate(Matrix matrix)
			{
				Global.Tracer.Assert(null != matrix);
				OWCFlagsCalculator oWCFlagsCalculator = new OWCFlagsCalculator();
				oWCFlagsCalculator.CalculateOWCFlags(matrix);
				if (oWCFlagsCalculator.m_useOWC)
				{
					matrix.UseOWC = oWCFlagsCalculator.m_useOWC;
					matrix.OwcCellNames = oWCFlagsCalculator.m_owcCellNames;
					int num = 0;
					for (MatrixHeading matrixHeading = matrix.Rows; matrixHeading != null; matrixHeading = matrixHeading.SubHeading)
					{
						if (matrixHeading.Grouping != null)
						{
							matrixHeading.OwcGroupExpression = oWCFlagsCalculator.m_owcGroupExpression[num];
							num++;
						}
					}
					for (MatrixHeading matrixHeading2 = matrix.Columns; matrixHeading2 != null; matrixHeading2 = matrixHeading2.SubHeading)
					{
						if (matrixHeading2.Grouping != null)
						{
							matrixHeading2.OwcGroupExpression = oWCFlagsCalculator.m_owcGroupExpression[num];
							num++;
						}
					}
				}
			}

			private void CalculateOWCFlags(Matrix matrix)
			{
				this.CalculateOWCFlags(matrix.Rows);
				if (!this.IsFinish())
				{
					this.CalculateOWCFlags(matrix.Columns);
					if (!this.IsFinish() && matrix.CellReportItems != null)
					{
						int num = 0;
						while (true)
						{
							if (num < matrix.CellReportItems.Count)
							{
								this.DetectIllegalReportItems(matrix.CellReportItems[num]);
								if (!this.IsFinish())
								{
									TextBox textBox = this.FindNotAlwaysHiddenTextBox(matrix.CellReportItems[num]);
									if (!this.IsFinish())
									{
										if (textBox != null)
										{
											Global.Tracer.Assert(null != textBox.Value);
											DataAggregateInfo sumAggregateWithoutScope = textBox.Value.GetSumAggregateWithoutScope();
											if (sumAggregateWithoutScope != null)
											{
												Global.Tracer.Assert(null != sumAggregateWithoutScope.Expressions);
												Global.Tracer.Assert(1 == sumAggregateWithoutScope.Expressions.Length);
												if (ExpressionInfo.Types.Field == sumAggregateWithoutScope.Expressions[0].Type)
												{
													this.m_owcCellNames.Add(sumAggregateWithoutScope.Expressions[0].Value);
												}
												else
												{
													this.m_owcCellNames.Add(textBox.Name);
												}
												num++;
												continue;
											}
											break;
										}
										this.m_useOWC = false;
									}
								}
							}
							return;
						}
						this.m_useOWC = false;
					}
				}
			}

			private void CalculateOWCFlags(MatrixHeading heading)
			{
				if (heading != null)
				{
					if (heading.Grouping == null)
					{
						this.m_staticHeadingCount++;
						if (this.m_staticHeadingCount > 1)
						{
							this.m_useOWC = false;
							return;
						}
						if (heading.SubHeading != null)
						{
							this.m_useOWC = false;
							return;
						}
						if (heading.ReportItems != null)
						{
							int num = 0;
							while (num < heading.ReportItems.Count)
							{
								this.DetectIllegalReportItems(heading.ReportItems[num]);
								if (this.IsFinish())
								{
									return;
								}
								TextBox textBox = this.FindNotAlwaysHiddenTextBox(heading.ReportItems[num]);
								if (this.IsFinish())
								{
									break;
								}
								if (textBox != null)
								{
									num++;
									continue;
								}
								this.m_useOWC = false;
								break;
							}
						}
						goto IL_017d;
					}
					ExpressionInfo expressionInfo = null;
					if (heading.Grouping.GroupExpressions != null)
					{
						if (heading.Grouping.GroupExpressions.Count != 1)
						{
							this.m_useOWC = false;
							return;
						}
						expressionInfo = heading.Grouping.GroupExpressions[0];
					}
					this.DetectIllegalReportItems(heading.ReportItem);
					if (!this.IsFinish())
					{
						TextBox textBox2 = this.FindNotAlwaysHiddenTextBox(heading.ReportItem);
						if (!this.IsFinish())
						{
							if (textBox2 != null)
							{
								Global.Tracer.Assert(null != expressionInfo);
								Global.Tracer.Assert(null != textBox2.Value);
								if (expressionInfo.OriginalText != textBox2.Value.OriginalText)
								{
									this.m_owcGroupExpression.Add(true);
								}
								else
								{
									this.m_owcGroupExpression.Add(false);
								}
								goto IL_017d;
							}
							this.m_useOWC = false;
						}
					}
				}
				return;
				IL_017d:
				this.CalculateOWCFlags(heading.SubHeading);
			}

			private void DetectIllegalReportItems(ReportItem reportItem)
			{
				if (reportItem is DataRegion || reportItem is Image || reportItem is SubReport || reportItem is ActiveXControl || reportItem is CheckBox)
				{
					this.m_useOWC = false;
				}
				else if (reportItem is Rectangle)
				{
					this.DetectIllegalReportItems(((Rectangle)reportItem).ReportItems);
				}
			}

			private void DetectIllegalReportItems(ReportItemCollection reportItems)
			{
				if (reportItems != null)
				{
					for (int i = 0; i < reportItems.Count; i++)
					{
						this.DetectIllegalReportItems(reportItems[i]);
						if (this.IsFinish())
						{
							break;
						}
					}
				}
			}

			private TextBox FindNotAlwaysHiddenTextBox(ReportItem reportItem)
			{
				if (reportItem is TextBox)
				{
					if (Visibility.GetSharedHidden(reportItem.Visibility) != 0)
					{
						return (TextBox)reportItem;
					}
				}
				else if (reportItem is Rectangle)
				{
					return this.FindNotAlwaysHiddenTextBox(((Rectangle)reportItem).ReportItems);
				}
				return null;
			}

			private TextBox FindNotAlwaysHiddenTextBox(ReportItemCollection reportItems)
			{
				if (reportItems == null)
				{
					return null;
				}
				TextBox textBox = null;
				for (int i = 0; i < reportItems.Count; i++)
				{
					ReportItem reportItem = reportItems[i];
					TextBox textBox2 = this.FindNotAlwaysHiddenTextBox(reportItem);
					if (this.IsFinish())
					{
						return null;
					}
					if (textBox2 != null)
					{
						if (textBox != null)
						{
							this.m_useOWC = false;
							return null;
						}
						textBox = textBox2;
					}
				}
				return textBox;
			}

			private bool IsFinish()
			{
				return !this.m_useOWC;
			}
		}

		private sealed class TopLevelItemsSizes
		{
			private MatrixColumnList m_columns;

			private MatrixRowList m_rows;

			private InitializationContext m_context;

			private TopLevelItemsSizes(MatrixColumnList columns, MatrixRowList rows, InitializationContext context)
			{
				this.m_columns = columns;
				this.m_rows = rows;
				this.m_context = context;
			}

			internal static void Calculate(Matrix matrix, double cornerWidth, double cornerHeight, double colsWidth, double rowsHeight, InitializationContext context)
			{
				TopLevelItemsSizes topLevelItemsSizes = new TopLevelItemsSizes(matrix.MatrixColumns, matrix.MatrixRows, context);
				topLevelItemsSizes.CalculateSizes(matrix, cornerWidth, cornerHeight, colsWidth, rowsHeight);
			}

			private void CalculateSizes(Matrix matrix, double cornerWidth, double cornerHeight, double colsWidth, double rowsHeight)
			{
				this.CalculateCorner(matrix, cornerWidth, cornerHeight);
				this.CalculateColumns(matrix.Columns, colsWidth);
				this.CalculateRows(matrix.Rows, rowsHeight);
				this.CalculateCells(matrix);
			}

			private void CalculateCorner(Matrix matrix, double width, double height)
			{
				if (matrix.CornerReportItems != null && 0 < matrix.CornerReportItems.Count)
				{
					this.CalculateSize(matrix.CornerReportItems[0], width, height);
				}
			}

			private void CalculateCells(Matrix matrix)
			{
				int num = 0;
				for (int i = 0; i < this.m_rows.Count; i++)
				{
					for (int j = 0; j < this.m_columns.Count; j++)
					{
						this.CalculateSize(matrix.CellReportItems[num], this.m_columns[j].WidthValue, this.m_rows[i].HeightValue);
						num++;
					}
				}
			}

			private void CalculateColumns(MatrixHeading column, double width)
			{
				if (column != null)
				{
					double num = width;
					if (column.Grouping == null)
					{
						if (column.ReportItems != null)
						{
							for (int i = 0; i < column.ReportItems.Count; i++)
							{
								this.CalculateSize(column.ReportItems[i], this.m_columns[i].WidthValue, column.SizeValue);
								if (this.m_columns[i].WidthValue < num)
								{
									num = this.m_columns[i].WidthValue;
								}
							}
						}
					}
					else
					{
						if (column.Subtotal != null)
						{
							this.CalculateSize(column.Subtotal.ReportItem, width, column.SizeValue);
						}
						this.CalculateSize(column.ReportItem, width, column.SizeValue);
					}
					this.CalculateColumns(column.SubHeading, num);
				}
			}

			private void CalculateRows(MatrixHeading row, double height)
			{
				if (row != null)
				{
					double num = height;
					if (row.Grouping == null)
					{
						if (row.ReportItems != null)
						{
							for (int i = 0; i < row.ReportItems.Count; i++)
							{
								this.CalculateSize(row.ReportItems[i], row.SizeValue, this.m_rows[i].HeightValue);
								if (this.m_rows[i].HeightValue < num)
								{
									num = this.m_rows[i].HeightValue;
								}
							}
						}
					}
					else
					{
						if (row.Subtotal != null)
						{
							this.CalculateSize(row.Subtotal.ReportItem, row.SizeValue, height);
						}
						this.CalculateSize(row.ReportItem, row.SizeValue, height);
					}
					this.CalculateRows(row.SubHeading, num);
				}
			}

			private void CalculateSize(ReportItem item, double width, double height)
			{
				if (item != null)
				{
					item.CalculateSizes(width, height, this.m_context, true);
				}
			}
		}

		private MatrixHeading m_columns;

		private MatrixHeading m_rows;

		private ReportItemCollection m_cornerReportItems;

		private ReportItemCollection m_cellReportItems;

		private IntList m_cellIDs;

		private bool m_propagatedPageBreakAtStart;

		private bool m_propagatedPageBreakAtEnd;

		private int m_innerRowLevelWithPageBreak = -1;

		private MatrixRowList m_matrixRows;

		private MatrixColumnList m_matrixColumns;

		private int m_groupsBeforeRowHeaders;

		private bool m_layoutDirection;

		[Reference]
		private MatrixHeading m_staticColumns;

		[Reference]
		private MatrixHeading m_staticRows;

		private bool m_useOWC;

		private StringList m_owcCellNames;

		private string m_cellDataElementName;

		private bool m_columnGroupingFixedHeader;

		private bool m_rowGroupingFixedHeader;

		[NonSerialized]
		private bool m_firstInstance = true;

		[NonSerialized]
		private BoolList m_firstCellInstances;

		[NonSerialized]
		private MatrixExprHost m_exprHost;

		[NonSerialized]
		private int m_currentPage = -1;

		[NonSerialized]
		private int m_cellPage = -1;

		[NonSerialized]
		private ReportProcessing.PageTextboxes m_cellPageTextboxes;

		[NonSerialized]
		private ReportProcessing.PageTextboxes m_columnHeaderPageTextboxes;

		[NonSerialized]
		private ReportProcessing.PageTextboxes m_rowHeaderPageTextboxes;

		[NonSerialized]
		private NonComputedUniqueNames m_cornerNonComputedUniqueNames;

		[NonSerialized]
		private bool m_inOutermostSubtotalCell;

		[NonSerialized]
		private ReportSizeCollection m_cellHeightsForRendering;

		[NonSerialized]
		private ReportSizeCollection m_cellWidthsForRendering;

		[NonSerialized]
		private string[] m_cellIDsForRendering;

		internal override ObjectType ObjectType
		{
			get
			{
				return ObjectType.Matrix;
			}
		}

		internal ReportItemCollection CornerReportItems
		{
			get
			{
				return this.m_cornerReportItems;
			}
			set
			{
				this.m_cornerReportItems = value;
			}
		}

		internal ReportItem CornerReportItem
		{
			get
			{
				if (this.m_cornerReportItems != null && 0 < this.m_cornerReportItems.Count)
				{
					return this.m_cornerReportItems[0];
				}
				return null;
			}
		}

		internal override PivotHeading PivotColumns
		{
			get
			{
				return this.m_columns;
			}
		}

		internal override PivotHeading PivotRows
		{
			get
			{
				return this.m_rows;
			}
		}

		internal MatrixHeading Columns
		{
			get
			{
				return this.m_columns;
			}
			set
			{
				this.m_columns = value;
			}
		}

		internal MatrixHeading Rows
		{
			get
			{
				return this.m_rows;
			}
			set
			{
				this.m_rows = value;
			}
		}

		internal ReportItemCollection CellReportItems
		{
			get
			{
				return this.m_cellReportItems;
			}
			set
			{
				this.m_cellReportItems = value;
			}
		}

		internal override RunningValueInfoList PivotCellRunningValues
		{
			get
			{
				return this.m_cellReportItems.RunningValues;
			}
		}

		internal IntList CellIDs
		{
			get
			{
				return this.m_cellIDs;
			}
			set
			{
				this.m_cellIDs = value;
			}
		}

		internal bool PropagatedPageBreakAtStart
		{
			get
			{
				return this.m_propagatedPageBreakAtStart;
			}
			set
			{
				this.m_propagatedPageBreakAtStart = value;
			}
		}

		internal bool PropagatedPageBreakAtEnd
		{
			get
			{
				return this.m_propagatedPageBreakAtEnd;
			}
			set
			{
				this.m_propagatedPageBreakAtEnd = value;
			}
		}

		internal int InnerRowLevelWithPageBreak
		{
			get
			{
				return this.m_innerRowLevelWithPageBreak;
			}
			set
			{
				this.m_innerRowLevelWithPageBreak = value;
			}
		}

		internal MatrixRowList MatrixRows
		{
			get
			{
				return this.m_matrixRows;
			}
			set
			{
				this.m_matrixRows = value;
			}
		}

		internal MatrixColumnList MatrixColumns
		{
			get
			{
				return this.m_matrixColumns;
			}
			set
			{
				this.m_matrixColumns = value;
			}
		}

		internal int GroupsBeforeRowHeaders
		{
			get
			{
				return this.m_groupsBeforeRowHeaders;
			}
			set
			{
				this.m_groupsBeforeRowHeaders = value;
			}
		}

		internal bool LayoutDirection
		{
			get
			{
				return this.m_layoutDirection;
			}
			set
			{
				this.m_layoutDirection = value;
			}
		}

		internal override PivotHeading PivotStaticColumns
		{
			get
			{
				return this.m_staticColumns;
			}
		}

		internal override PivotHeading PivotStaticRows
		{
			get
			{
				return this.m_staticRows;
			}
		}

		internal MatrixHeading StaticColumns
		{
			get
			{
				return this.m_staticColumns;
			}
			set
			{
				this.m_staticColumns = value;
			}
		}

		internal MatrixHeading StaticRows
		{
			get
			{
				return this.m_staticRows;
			}
			set
			{
				this.m_staticRows = value;
			}
		}

		internal bool UseOWC
		{
			get
			{
				return this.m_useOWC;
			}
			set
			{
				this.m_useOWC = value;
			}
		}

		internal StringList OwcCellNames
		{
			get
			{
				return this.m_owcCellNames;
			}
			set
			{
				this.m_owcCellNames = value;
			}
		}

		internal string CellDataElementName
		{
			get
			{
				return this.m_cellDataElementName;
			}
			set
			{
				this.m_cellDataElementName = value;
			}
		}

		internal bool FirstInstance
		{
			get
			{
				return this.m_firstInstance;
			}
			set
			{
				this.m_firstInstance = value;
			}
		}

		internal BoolList FirstCellInstances
		{
			get
			{
				return this.m_firstCellInstances;
			}
			set
			{
				this.m_firstCellInstances = value;
			}
		}

		internal MatrixExprHost MatrixExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		protected override DataRegionExprHost DataRegionExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal int CurrentPage
		{
			get
			{
				return this.m_currentPage;
			}
			set
			{
				this.m_currentPage = value;
			}
		}

		internal NonComputedUniqueNames CornerNonComputedUniqueNames
		{
			get
			{
				return this.m_cornerNonComputedUniqueNames;
			}
			set
			{
				this.m_cornerNonComputedUniqueNames = value;
			}
		}

		internal bool InOutermostSubtotalCell
		{
			get
			{
				return this.m_inOutermostSubtotalCell;
			}
			set
			{
				this.m_inOutermostSubtotalCell = value;
			}
		}

		internal ReportSizeCollection CellHeightsForRendering
		{
			get
			{
				if (this.m_cellHeightsForRendering == null)
				{
					this.m_cellHeightsForRendering = new ReportSizeCollection(this.m_matrixRows.Count);
					for (int i = 0; i < this.m_matrixRows.Count; i++)
					{
						MatrixRow matrixRow = this.m_matrixRows[i];
						this.m_cellHeightsForRendering[i] = new ReportSize(matrixRow.Height, matrixRow.HeightValue);
					}
				}
				return this.m_cellHeightsForRendering;
			}
		}

		internal ReportSizeCollection CellWidthsForRendering
		{
			get
			{
				if (this.m_cellWidthsForRendering == null)
				{
					this.m_cellWidthsForRendering = new ReportSizeCollection(this.m_matrixColumns.Count);
					for (int i = 0; i < this.m_matrixColumns.Count; i++)
					{
						MatrixColumn matrixColumn = this.m_matrixColumns[i];
						this.m_cellWidthsForRendering[i] = new ReportSize(matrixColumn.Width, matrixColumn.WidthValue);
					}
				}
				return this.m_cellWidthsForRendering;
			}
		}

		internal string[] CellIDsForRendering
		{
			get
			{
				return this.m_cellIDsForRendering;
			}
			set
			{
				this.m_cellIDsForRendering = value;
			}
		}

		internal bool ColumnGroupingFixedHeader
		{
			get
			{
				return this.m_columnGroupingFixedHeader;
			}
			set
			{
				this.m_columnGroupingFixedHeader = value;
			}
		}

		internal bool RowGroupingFixedHeader
		{
			get
			{
				return this.m_rowGroupingFixedHeader;
			}
			set
			{
				this.m_rowGroupingFixedHeader = value;
			}
		}

		internal ReportProcessing.PageTextboxes CellPageTextboxes
		{
			get
			{
				return this.m_cellPageTextboxes;
			}
		}

		internal ReportProcessing.PageTextboxes ColumnHeaderPageTextboxes
		{
			get
			{
				return this.m_columnHeaderPageTextboxes;
			}
		}

		internal ReportProcessing.PageTextboxes RowHeaderPageTextboxes
		{
			get
			{
				return this.m_rowHeaderPageTextboxes;
			}
		}

		internal int CellPage
		{
			get
			{
				if (0 > this.m_cellPage)
				{
					this.m_cellPage = this.m_currentPage;
				}
				return this.m_cellPage;
			}
			set
			{
				this.m_cellPage = value;
			}
		}

		internal Matrix(ReportItem parent)
			: base(parent)
		{
		}

		internal Matrix(int id, int idForCornerReportItems, int idForCellReportItems, ReportItem parent)
			: base(id, parent)
		{
			this.m_cornerReportItems = new ReportItemCollection(idForCornerReportItems, false);
			this.m_cellReportItems = new ReportItemCollection(idForCellReportItems, false);
		}

		internal override bool Initialize(InitializationContext context)
		{
			context.Location |= LocationFlags.InMatrixOrTable;
			context.ObjectType = this.ObjectType;
			context.ObjectName = base.m_name;
			context.RegisterDataRegion(this);
			this.InternalInitialize(context);
			context.UnRegisterDataRegion(this);
			return false;
		}

		private void InternalInitialize(InitializationContext context)
		{
			context.Location = (context.Location | LocationFlags.InDataSet | LocationFlags.InDataRegion);
			context.ExprHostBuilder.MatrixStart(base.m_name);
			base.Initialize(context);
			context.RegisterRunningValues(base.m_runningValues);
			if (base.m_visibility != null)
			{
				base.m_visibility.Initialize(context, true, false);
			}
			this.CornerInitialize(context);
			context.Location &= ~LocationFlags.InMatrixCellTopLevelItem;
			bool flag = false;
			bool flag2 = false;
			context.Location |= LocationFlags.InMatrixGroupHeader;
			int expectedNumberOfMatrixColumns = default(int);
			double num = default(double);
			this.ColumnsInitialize(context, out expectedNumberOfMatrixColumns, out num, out flag);
			flag2 = flag;
			int expectedNumberOfMatrixRows = default(int);
			double num2 = default(double);
			this.RowsInitialize(context, out expectedNumberOfMatrixRows, out num2, out flag);
			context.Location &= ~LocationFlags.InMatrixGroupHeader;
			if (flag)
			{
				flag2 = true;
			}
			double num3 = default(double);
			double num4 = default(double);
			this.MatrixCellInitialize(context, expectedNumberOfMatrixColumns, expectedNumberOfMatrixRows, flag2, out num3, out num4);
			if (base.m_visibility != null)
			{
				base.m_visibility.UnRegisterReceiver(context);
			}
			context.UnRegisterRunningValues(base.m_runningValues);
			base.CopyHeadingAggregates(this.m_rows);
			this.m_rows.TransferHeadingAggregates();
			base.CopyHeadingAggregates(this.m_columns);
			this.m_columns.TransferHeadingAggregates();
			base.m_heightValue = num + num3;
			base.m_height = Converter.ConvertSize(base.m_heightValue);
			base.m_widthValue = num2 + num4;
			base.m_width = Converter.ConvertSize(base.m_widthValue);
			if (!context.ErrorContext.HasError)
			{
				TopLevelItemsSizes.Calculate(this, num2, num, num4, num3, context);
			}
			base.ExprHostID = context.ExprHostBuilder.MatrixEnd();
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID >= 0)
			{
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null);
				this.m_exprHost = reportExprHost.MatrixHostsRemotable[base.ExprHostID];
				base.DataRegionSetExprHost(this.m_exprHost, reportObjectModel);
			}
		}

		protected override void DataRendererInitialize(InitializationContext context)
		{
			base.DataRendererInitialize(context);
			CLSNameValidator.ValidateDataElementName(ref this.m_cellDataElementName, "Cell", context.ObjectType, context.ObjectName, "CellDataElementName", context.ErrorContext);
		}

		internal override void RegisterReceiver(InitializationContext context)
		{
			context.RegisterReportItems(this.m_cornerReportItems);
			if (base.m_visibility != null)
			{
				base.m_visibility.RegisterReceiver(context, true);
			}
			this.m_cornerReportItems.RegisterReceiver(context);
			this.ColumnsRegisterReceiver(context);
			this.RowsRegisterReceiver(context);
			this.MatrixCellRegisterReceiver(context);
			if (base.m_visibility != null)
			{
				base.m_visibility.UnRegisterReceiver(context);
			}
			context.UnRegisterReportItems(this.m_cornerReportItems);
		}

		internal void CalculatePropagatedFlags()
		{
			MatrixHeading matrixHeading = this.m_rows;
			int num = 0;
			do
			{
				if (matrixHeading.Grouping != null)
				{
					if (matrixHeading.Grouping.PageBreakAtStart)
					{
						this.m_propagatedPageBreakAtStart = true;
						this.m_innerRowLevelWithPageBreak = num;
					}
					if (matrixHeading.Grouping.PageBreakAtEnd)
					{
						this.m_propagatedPageBreakAtEnd = true;
						this.m_innerRowLevelWithPageBreak = num;
					}
				}
				matrixHeading = matrixHeading.SubHeading;
				num++;
			}
			while (matrixHeading != null);
		}

		private void CornerInitialize(InitializationContext context)
		{
			this.m_cornerReportItems.Initialize(context, false);
		}

		private void ColumnsInitialize(InitializationContext context, out int expectedNumberOfMatrixColumns, out double size, out bool computedSubtotal)
		{
			computedSubtotal = false;
			size = 0.0;
			this.m_columns.DynamicInitialize(true, 0, context, ref size);
			this.m_columns.StaticInitialize(context);
			expectedNumberOfMatrixColumns = ((this.m_staticColumns == null) ? 1 : this.m_staticColumns.NumberOfStatics);
			if (this.m_columns.Grouping == null)
			{
				Global.Tracer.Assert(null != this.m_columns.ReportItems);
				context.SpecialTransferRunningValues(this.m_columns.ReportItems.RunningValues);
			}
			else if (this.m_columns.Subtotal != null)
			{
				Global.Tracer.Assert(null != this.m_columns.Subtotal.ReportItems);
				context.SpecialTransferRunningValues(this.m_columns.Subtotal.ReportItems.RunningValues);
				computedSubtotal = this.m_columns.Subtotal.Computed;
			}
		}

		private void ColumnsRegisterReceiver(InitializationContext context)
		{
			this.m_columns.DynamicRegisterReceiver(context);
			this.m_columns.StaticRegisterReceiver(context);
		}

		private void RowsInitialize(InitializationContext context, out int expectedNumberOfMatrixRows, out double size, out bool computedSubtotal)
		{
			computedSubtotal = false;
			size = 0.0;
			this.m_rows.DynamicInitialize(false, 0, context, ref size);
			this.m_rows.StaticInitialize(context);
			expectedNumberOfMatrixRows = ((this.m_staticRows == null) ? 1 : this.m_staticRows.NumberOfStatics);
			if (this.m_rows.Grouping == null)
			{
				Global.Tracer.Assert(null != this.m_rows.ReportItems);
				context.SpecialTransferRunningValues(this.m_rows.ReportItems.RunningValues);
			}
			else if (this.m_rows.Subtotal != null)
			{
				Global.Tracer.Assert(null != this.m_rows.Subtotal.ReportItems);
				context.SpecialTransferRunningValues(this.m_rows.Subtotal.ReportItems.RunningValues);
				computedSubtotal = this.m_rows.Subtotal.Computed;
			}
		}

		private void RowsRegisterReceiver(InitializationContext context)
		{
			this.m_rows.DynamicRegisterReceiver(context);
			this.m_rows.StaticRegisterReceiver(context);
		}

		private void MatrixCellInitialize(InitializationContext context, int expectedNumberOfMatrixColumns, int expectedNumberOfMatrixRows, bool computedSubtotal, out double totalCellHeight, out double totalCellWidth)
		{
			if (expectedNumberOfMatrixColumns != this.m_matrixColumns.Count)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsWrongNumberOfMatrixColumns, Severity.Error, context.ObjectType, context.ObjectName, "MatrixColumns");
			}
			if (expectedNumberOfMatrixRows != this.m_matrixRows.Count)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsWrongNumberOfMatrixRows, Severity.Error, context.ObjectType, context.ObjectName, "MatrixRows");
			}
			for (int i = 0; i < this.m_matrixRows.Count; i++)
			{
				if (expectedNumberOfMatrixColumns != this.m_matrixRows[i].NumberOfMatrixCells)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsWrongNumberOfMatrixCells, Severity.Error, context.ObjectType, context.ObjectName, "MatrixCells");
				}
			}
			totalCellHeight = 0.0;
			totalCellWidth = 0.0;
			for (int j = 0; j < this.m_matrixColumns.Count; j++)
			{
				this.m_matrixColumns[j].Initialize(context);
				totalCellWidth = Math.Round(totalCellWidth + this.m_matrixColumns[j].WidthValue, Validator.DecimalPrecision);
			}
			for (int k = 0; k < this.m_matrixRows.Count; k++)
			{
				this.m_matrixRows[k].Initialize(context);
				totalCellHeight = Math.Round(totalCellHeight + this.m_matrixRows[k].HeightValue, Validator.DecimalPrecision);
			}
			context.Location = (context.Location | LocationFlags.InMatrixCell | LocationFlags.InMatrixCellTopLevelItem);
			context.MatrixName = base.m_name;
			context.RegisterTablixCellScope(this.m_columns.SubHeading == null && null == this.m_columns.Grouping, base.m_cellAggregates, base.m_cellPostSortAggregates);
			for (MatrixHeading matrixHeading = this.m_rows; matrixHeading != null; matrixHeading = matrixHeading.SubHeading)
			{
				context.RegisterReportItems(matrixHeading.ReportItems);
				if (matrixHeading.Grouping != null)
				{
					context.Location |= LocationFlags.InGrouping;
					context.RegisterGroupingScopeForTablixCell(matrixHeading.Grouping.Name, false, matrixHeading.Grouping.SimpleGroupExpressions, matrixHeading.Aggregates, matrixHeading.PostSortAggregates, matrixHeading.RecursiveAggregates, matrixHeading.Grouping);
				}
			}
			if (this.m_rows.Grouping != null && this.m_rows.Subtotal != null && this.m_staticRows != null)
			{
				context.CopyRunningValues(this.StaticRows.ReportItems.RunningValues, base.m_aggregates);
			}
			for (MatrixHeading matrixHeading = this.m_columns; matrixHeading != null; matrixHeading = matrixHeading.SubHeading)
			{
				context.RegisterReportItems(matrixHeading.ReportItems);
				if (matrixHeading.Grouping != null)
				{
					context.Location |= LocationFlags.InGrouping;
					context.RegisterGroupingScopeForTablixCell(matrixHeading.Grouping.Name, true, matrixHeading.Grouping.SimpleGroupExpressions, matrixHeading.Aggregates, matrixHeading.PostSortAggregates, matrixHeading.RecursiveAggregates, matrixHeading.Grouping);
				}
			}
			if (this.m_columns.Grouping != null && this.m_columns.Subtotal != null && this.m_staticColumns != null)
			{
				context.CopyRunningValues(this.StaticColumns.ReportItems.RunningValues, base.m_aggregates);
			}
			if (computedSubtotal)
			{
				this.m_cellReportItems.MarkChildrenComputed();
			}
			context.RegisterReportItems(this.m_cellReportItems);
			OWCFlagsCalculator.Calculate(this);
			bool registerHiddenReceiver = context.RegisterHiddenReceiver;
			context.RegisterHiddenReceiver = false;
			context.RegisterScopeInMatrixCell(base.Name, "0_CellScope" + base.Name, true);
			this.m_cellReportItems.Initialize(context, true);
			if (context.IsRunningValueDirectionColumn())
			{
				base.m_processingInnerGrouping = ProcessingInnerGroupings.Row;
			}
			context.UpdateScopesInMatrixCells(base.Name, this.GenerateUserSortGroupingList(ProcessingInnerGroupings.Row == base.m_processingInnerGrouping));
			context.TextboxesWithDetailSortExpressionInitialize();
			context.RegisterHiddenReceiver = registerHiddenReceiver;
			for (MatrixHeading matrixHeading = this.m_rows; matrixHeading != null; matrixHeading = matrixHeading.SubHeading)
			{
				context.UnRegisterReportItems(matrixHeading.ReportItems);
				if (matrixHeading.Grouping != null)
				{
					context.UnRegisterGroupingScopeForTablixCell(matrixHeading.Grouping.Name, false);
					context.ProcessUserSortInnerScope(matrixHeading.Grouping.Name, true, false);
				}
			}
			for (MatrixHeading matrixHeading = this.m_rows; matrixHeading != null; matrixHeading = matrixHeading.SubHeading)
			{
				if (matrixHeading.Grouping != null)
				{
					context.ValidateUserSortInnerScope(matrixHeading.Grouping.Name);
				}
			}
			for (MatrixHeading matrixHeading = this.m_columns; matrixHeading != null; matrixHeading = matrixHeading.SubHeading)
			{
				context.UnRegisterReportItems(matrixHeading.ReportItems);
				if (matrixHeading.Grouping != null)
				{
					context.UnRegisterGroupingScopeForTablixCell(matrixHeading.Grouping.Name, true);
					context.ProcessUserSortInnerScope(matrixHeading.Grouping.Name, true, true);
				}
			}
			for (MatrixHeading matrixHeading = this.m_columns; matrixHeading != null; matrixHeading = matrixHeading.SubHeading)
			{
				if (matrixHeading.Grouping != null)
				{
					context.ValidateUserSortInnerScope(matrixHeading.Grouping.Name);
				}
			}
			this.m_cellReportItems.RegisterReceiver(context);
			context.UnRegisterReportItems(this.m_cellReportItems);
			context.UnRegisterTablixCellScope();
		}

		private GroupingList GenerateUserSortGroupingList(bool rowIsInnerGrouping)
		{
			GroupingList groupingList = new GroupingList();
			for (MatrixHeading matrixHeading = rowIsInnerGrouping ? this.m_rows : this.m_columns; matrixHeading != null; matrixHeading = matrixHeading.SubHeading)
			{
				if (matrixHeading.Grouping != null)
				{
					groupingList.Add(matrixHeading.Grouping);
				}
			}
			for (MatrixHeading matrixHeading = rowIsInnerGrouping ? this.m_columns : this.m_rows; matrixHeading != null; matrixHeading = matrixHeading.SubHeading)
			{
				if (matrixHeading.Grouping != null)
				{
					groupingList.Add(matrixHeading.Grouping);
				}
			}
			return groupingList;
		}

		private void MatrixCellRegisterReceiver(InitializationContext context)
		{
			context.RegisterReportItems(this.m_cellReportItems);
			this.m_cellReportItems.RegisterReceiver(context);
			context.UnRegisterReportItems(this.m_cellReportItems);
		}

		bool IPageBreakItem.IgnorePageBreaks()
		{
			if (base.m_pagebreakState == PageBreakStates.Unknown)
			{
				if (SharedHiddenState.Never != Visibility.GetSharedHidden(base.m_visibility))
				{
					base.m_pagebreakState = PageBreakStates.CanIgnore;
				}
				else if (SharedHiddenState.Never != Visibility.GetSharedHidden(this.m_rows.Visibility))
				{
					base.m_pagebreakState = PageBreakStates.CanIgnore;
				}
				else
				{
					base.m_pagebreakState = PageBreakStates.CannotIgnore;
				}
			}
			if (PageBreakStates.CanIgnore == base.m_pagebreakState)
			{
				return true;
			}
			return false;
		}

		internal ReportItem GetCellReportItem(int rowIndex, int columnIndex)
		{
			int index = rowIndex * this.m_matrixColumns.Count + columnIndex;
			return this.m_cellReportItems[index];
		}

		internal void InitializePageSectionProcessing()
		{
			this.m_cellPageTextboxes = new ReportProcessing.PageTextboxes();
			this.m_columnHeaderPageTextboxes = new ReportProcessing.PageTextboxes();
			this.m_rowHeaderPageTextboxes = new ReportProcessing.PageTextboxes();
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Columns, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.MatrixHeading));
			memberInfoList.Add(new MemberInfo(MemberName.Rows, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.MatrixHeading));
			memberInfoList.Add(new MemberInfo(MemberName.CornerReportItems, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemCollection));
			memberInfoList.Add(new MemberInfo(MemberName.CellReportItems, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemCollection));
			memberInfoList.Add(new MemberInfo(MemberName.CellIDs, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.IntList));
			memberInfoList.Add(new MemberInfo(MemberName.PropagatedPageBreakAtStart, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.PropagatedPageBreakAtEnd, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.InnerRowLevelWithPageBreak, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.MatrixRows, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.MatrixRowList));
			memberInfoList.Add(new MemberInfo(MemberName.MatrixColumns, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.MatrixColumnList));
			memberInfoList.Add(new MemberInfo(MemberName.GroupsBeforeRowHeaders, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.LayoutDirection, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.StaticColumns, Token.Reference, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.MatrixHeading));
			memberInfoList.Add(new MemberInfo(MemberName.StaticRows, Token.Reference, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.MatrixHeading));
			memberInfoList.Add(new MemberInfo(MemberName.UseOwc, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.OwcCellNames, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.StringList));
			memberInfoList.Add(new MemberInfo(MemberName.CellDataElementName, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.ColumnGroupingFixedHeader, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.RowGroupingFixedHeader, Token.Boolean));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Pivot, memberInfoList);
		}
	}
}
