using AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel;
using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using AspNetCore.ReportingServices.ReportRendering;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class Table : DataRegion, IPageBreakItem, IRunningValueHolder
	{
		private sealed class OWCFlagsCalculator
		{
			private const uint MaxNumberOfTextBoxAndCheckBox = 1u;

			private uint m_numberOfTextBoxAndCheckBox;

			private bool m_useOWC = true;

			private bool m_owcNonSharedStyles;

			private OWCFlagsCalculator()
			{
			}

			internal static void Calculate(Table table, out bool useOWC, out bool owcNonSharedStyles)
			{
				OWCFlagsCalculator oWCFlagsCalculator = new OWCFlagsCalculator();
				oWCFlagsCalculator.CalculateOWCFlags(table);
				useOWC = oWCFlagsCalculator.m_useOWC;
				owcNonSharedStyles = oWCFlagsCalculator.m_owcNonSharedStyles;
			}

			private void CalculateOWCFlags(Table table)
			{
				this.CalculateOWCFlags(table.HeaderRows);
				if (!this.IsFinish())
				{
					this.CalculateOWCFlags(table.TableGroups);
					if (!this.IsFinish())
					{
						this.CalculateOWCFlags(table.TableDetail);
						if (!this.IsFinish())
						{
							this.CalculateOWCFlags(table.FooterRows);
						}
					}
				}
			}

			private void CalculateOWCFlags(TableGroup tableGroup)
			{
				if (tableGroup != null)
				{
					this.CalculateOWCFlags(tableGroup.HeaderRows);
					if (!this.IsFinish())
					{
						this.CalculateOWCFlags(tableGroup.SubGroup);
						if (!this.IsFinish())
						{
							this.CalculateOWCFlags(tableGroup.FooterRows);
						}
					}
				}
			}

			private void CalculateOWCFlags(TableDetail tableDetail)
			{
				if (tableDetail != null)
				{
					this.CalculateOWCFlags(tableDetail.DetailRows);
					this.IsFinish();
				}
			}

			private void CalculateOWCFlags(TableRowList tableRows)
			{
				if (tableRows != null)
				{
					for (int i = 0; i < tableRows.Count; i++)
					{
						this.CalculateOWCFlags(tableRows[i]);
						if (this.IsFinish())
						{
							break;
						}
					}
				}
			}

			private void CalculateOWCFlags(TableRow tableRow)
			{
				if (tableRow != null && tableRow.ReportItems != null)
				{
					for (int i = 0; i < tableRow.ReportItems.Count; i++)
					{
						this.m_numberOfTextBoxAndCheckBox = 0u;
						this.CalculateOWCFlags(tableRow.ReportItems[i]);
						if (this.IsFinish())
						{
							break;
						}
					}
				}
			}

			private void CalculateOWCFlags(ReportItem item)
			{
				if (item != null)
				{
					if ((!(item is TextBox) && !(item is CheckBox) && !(item is Rectangle)) || item.Visibility == null)
					{
						if (item is TextBox || item is CheckBox)
						{
							this.m_numberOfTextBoxAndCheckBox += 1u;
							if (this.m_numberOfTextBoxAndCheckBox > 1)
							{
								this.m_useOWC = false;
								return;
							}
							if (item.StyleClass != null && item.StyleClass.ExpressionList != null && 0 < item.StyleClass.ExpressionList.Count)
							{
								this.m_owcNonSharedStyles = true;
							}
						}
						if (item is TextBox)
						{
							TextBox textBox = (TextBox)item;
							if (textBox.IsToggle)
							{
								this.m_useOWC = false;
								return;
							}
						}
						if (item is Rectangle)
						{
							Rectangle rectangle = (Rectangle)item;
							if (rectangle.ReportItems != null)
							{
								for (int i = 0; i < rectangle.ReportItems.Count; i++)
								{
									this.CalculateOWCFlags(rectangle.ReportItems[i]);
									if (this.IsFinish())
									{
										return;
									}
								}
							}
						}
						if (!(item is Image) && !(item is SubReport) && !(item is ActiveXControl) && !(item is DataRegion))
						{
							return;
						}
						this.m_useOWC = false;
						return;
					}
					this.m_useOWC = false;
				}
			}

			private bool IsFinish()
			{
				return !this.m_useOWC;
			}
		}

		private sealed class TopLevelItemsSizes
		{
			private TableColumnList m_tableColumns;

			private InitializationContext m_context;

			private TopLevelItemsSizes(TableColumnList tableColumns, InitializationContext context)
			{
				this.m_tableColumns = tableColumns;
				this.m_context = context;
			}

			internal static void Calculate(Table table, InitializationContext context)
			{
				TopLevelItemsSizes topLevelItemsSizes = new TopLevelItemsSizes(table.TableColumns, context);
				topLevelItemsSizes.CalculateSizes(table);
			}

			private void CalculateSizes(Table table)
			{
				this.CalculateSizes(table.HeaderRows);
				this.CalculateSizes(table.TableGroups);
				this.CalculateSizes(table.TableDetail);
				this.CalculateSizes(table.FooterRows);
			}

			private void CalculateSizes(TableGroup tableGroup)
			{
				if (tableGroup != null)
				{
					this.CalculateSizes(tableGroup.HeaderRows);
					this.CalculateSizes(tableGroup.SubGroup);
					this.CalculateSizes(tableGroup.FooterRows);
				}
			}

			private void CalculateSizes(TableDetail tableDetail)
			{
				if (tableDetail != null)
				{
					this.CalculateSizes(tableDetail.DetailRows);
				}
			}

			private void CalculateSizes(TableRowList tableRows)
			{
				if (tableRows != null)
				{
					for (int i = 0; i < tableRows.Count; i++)
					{
						this.CalculateSizes(tableRows[i]);
					}
				}
			}

			private void CalculateSizes(TableRow tableRow)
			{
				if (tableRow != null && tableRow.ReportItems != null)
				{
					int num = 0;
					double num2 = 0.0;
					int num3 = 0;
					for (int i = 0; i < tableRow.ReportItems.Count; i++)
					{
						num2 = 0.0;
						for (num3 = tableRow.ColSpans[i]; num3 > 0; num3--)
						{
							num2 += this.m_tableColumns[num].WidthValue;
							num++;
						}
						this.CalculateSizes(tableRow.ReportItems[i], num2, tableRow.HeightValue);
					}
				}
			}

			private void CalculateSizes(ReportItem item, double width, double height)
			{
				if (item != null)
				{
					item.CalculateSizes(width, height, this.m_context, true);
				}
			}
		}

		private TableColumnList m_tableColumns;

		private TableRowList m_headerRows;

		private bool m_headerRepeatOnNewPage;

		private TableGroup m_tableGroups;

		private TableDetail m_tableDetail;

		private TableGroup m_detailGroup;

		private TableRowList m_footerRows;

		private bool m_footerRepeatOnNewPage;

		private bool m_propagatedPageBreakAtStart;

		private bool m_groupPageBreakAtStart;

		private bool m_propagatedPageBreakAtEnd;

		private bool m_groupPageBreakAtEnd;

		private bool m_fillPage;

		private bool m_useOWC;

		private bool m_owcNonSharedStyles;

		private RunningValueInfoList m_runningValues;

		private string m_detailDataElementName;

		private string m_detailDataCollectionName;

		private DataElementOutputTypes m_detailDataElementOutput;

		private bool m_fixedHeader;

		[NonSerialized]
		private TableExprHost m_exprHost;

		[NonSerialized]
		private int m_currentPage = -1;

		[NonSerialized]
		private bool m_hasFixedColumnHeaders;

		[NonSerialized]
		private bool[] m_columnsStartHidden;

		internal override ObjectType ObjectType
		{
			get
			{
				return ObjectType.Table;
			}
		}

		internal TableColumnList TableColumns
		{
			get
			{
				return this.m_tableColumns;
			}
			set
			{
				this.m_tableColumns = value;
			}
		}

		internal TableRowList HeaderRows
		{
			get
			{
				return this.m_headerRows;
			}
			set
			{
				this.m_headerRows = value;
			}
		}

		internal bool HeaderRepeatOnNewPage
		{
			get
			{
				return this.m_headerRepeatOnNewPage;
			}
			set
			{
				this.m_headerRepeatOnNewPage = value;
			}
		}

		internal TableGroup TableGroups
		{
			get
			{
				return this.m_tableGroups;
			}
			set
			{
				this.m_tableGroups = value;
			}
		}

		internal TableDetail TableDetail
		{
			get
			{
				return this.m_tableDetail;
			}
			set
			{
				this.m_tableDetail = value;
			}
		}

		internal TableGroup DetailGroup
		{
			get
			{
				return this.m_detailGroup;
			}
			set
			{
				this.m_detailGroup = value;
			}
		}

		internal TableRowList FooterRows
		{
			get
			{
				return this.m_footerRows;
			}
			set
			{
				this.m_footerRows = value;
			}
		}

		internal bool FooterRepeatOnNewPage
		{
			get
			{
				return this.m_footerRepeatOnNewPage;
			}
			set
			{
				this.m_footerRepeatOnNewPage = value;
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

		internal bool GroupBreakAtStart
		{
			get
			{
				return this.m_groupPageBreakAtStart;
			}
			set
			{
				this.m_groupPageBreakAtStart = value;
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

		internal bool GroupBreakAtEnd
		{
			get
			{
				return this.m_groupPageBreakAtEnd;
			}
			set
			{
				this.m_groupPageBreakAtEnd = value;
			}
		}

		internal bool FillPage
		{
			get
			{
				return this.m_fillPage;
			}
			set
			{
				this.m_fillPage = value;
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

		internal bool OWCNonSharedStyles
		{
			get
			{
				return this.m_owcNonSharedStyles;
			}
			set
			{
				this.m_owcNonSharedStyles = value;
			}
		}

		internal RunningValueInfoList RunningValues
		{
			get
			{
				return this.m_runningValues;
			}
			set
			{
				this.m_runningValues = value;
			}
		}

		internal string DetailDataElementName
		{
			get
			{
				return this.m_detailDataElementName;
			}
			set
			{
				this.m_detailDataElementName = value;
			}
		}

		internal string DetailDataCollectionName
		{
			get
			{
				return this.m_detailDataCollectionName;
			}
			set
			{
				this.m_detailDataCollectionName = value;
			}
		}

		internal DataElementOutputTypes DetailDataElementOutput
		{
			get
			{
				return this.m_detailDataElementOutput;
			}
			set
			{
				this.m_detailDataElementOutput = value;
			}
		}

		internal TableExprHost TableExprHost
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

		protected override DataRegionExprHost DataRegionExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal bool FixedHeader
		{
			get
			{
				return this.m_fixedHeader;
			}
			set
			{
				this.m_fixedHeader = value;
			}
		}

		internal bool HasFixedColumnHeaders
		{
			get
			{
				return this.m_hasFixedColumnHeaders;
			}
			set
			{
				this.m_hasFixedColumnHeaders = value;
			}
		}

		internal double HeaderHeightValue
		{
			get
			{
				if (this.m_headerRows != null)
				{
					return this.m_headerRows.GetHeightValue();
				}
				return 0.0;
			}
		}

		internal double DetailHeightValue
		{
			get
			{
				if (this.m_tableDetail != null && this.m_tableDetail.DetailRows != null)
				{
					return this.m_tableDetail.DetailRows.GetHeightValue();
				}
				return 0.0;
			}
		}

		internal bool[] ColumnsStartHidden
		{
			get
			{
				return this.m_columnsStartHidden;
			}
			set
			{
				this.m_columnsStartHidden = value;
			}
		}

		internal Table(ReportItem parent)
			: base(parent)
		{
		}

		internal Table(int id, ReportItem parent)
			: base(id, parent)
		{
			this.m_runningValues = new RunningValueInfoList();
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
			context.ExprHostBuilder.TableStart(base.m_name);
			base.Initialize(context);
			context.RegisterRunningValues(this.m_runningValues);
			if (base.m_visibility != null)
			{
				base.m_visibility.Initialize(context, true, false);
			}
			bool[] tableColumnVisibility = default(bool[]);
			this.InitializeTableColumns(context, ref base.m_widthValue, out tableColumnVisibility);
			base.m_width = Converter.ConvertSize(base.m_widthValue);
			this.InitializeHeaderAndFooter(this.m_tableColumns.Count, context, ref base.m_heightValue, tableColumnVisibility);
			context.Location &= ~LocationFlags.InMatrixCellTopLevelItem;
			this.InitializeTableGroupsOrDetail(this.m_tableColumns.Count, context, ref base.m_heightValue, tableColumnVisibility);
			base.m_height = Converter.ConvertSize(base.m_heightValue);
			if (base.m_visibility != null)
			{
				base.m_visibility.UnRegisterReceiver(context);
			}
			context.UnRegisterRunningValues(this.m_runningValues);
			OWCFlagsCalculator.Calculate(this, out this.m_useOWC, out this.m_owcNonSharedStyles);
			if (!context.ErrorContext.HasError)
			{
				TopLevelItemsSizes.Calculate(this, context);
			}
			base.ExprHostID = context.ExprHostBuilder.TableEnd();
		}

		protected override void DataRendererInitialize(InitializationContext context)
		{
			base.DataRendererInitialize(context);
			CLSNameValidator.ValidateDataElementName(ref this.m_detailDataElementName, "Detail", context.ObjectType, context.ObjectName, "DetailDataElementName", context.ErrorContext);
			CLSNameValidator.ValidateDataElementName(ref this.m_detailDataCollectionName, this.m_detailDataElementName + "_Collection", context.ObjectType, context.ObjectName, "DetailDataCollectionName", context.ErrorContext);
		}

		internal override void RegisterReceiver(InitializationContext context)
		{
			this.RegisterHeaderAndFooter(context);
			if (base.m_visibility != null)
			{
				base.m_visibility.RegisterReceiver(context, true);
			}
			this.RegisterTableColumnsReceiver(context);
			this.RegisterHeaderAndFooterReceiver(context);
			this.RegisterTableGroupsOrDetailReceiver(context);
			if (base.m_visibility != null)
			{
				base.m_visibility.UnRegisterReceiver(context);
			}
			this.UnRegisterHeaderAndFooter(context);
		}

		internal void RegisterHeaderAndFooter(InitializationContext context)
		{
			if (this.m_headerRows != null)
			{
				this.m_headerRows.Register(context);
			}
			if (this.m_footerRows != null)
			{
				this.m_footerRows.Register(context);
			}
		}

		internal void UnRegisterHeaderAndFooter(InitializationContext context)
		{
			if (this.m_footerRows != null)
			{
				this.m_footerRows.UnRegister(context);
			}
			if (this.m_headerRows != null)
			{
				this.m_headerRows.UnRegister(context);
			}
		}

		private void InitializeTableColumns(InitializationContext context, ref double tableWidth, out bool[] tableColumnVisibility)
		{
			context.ExprHostBuilder.TableColumnVisibilityHiddenExpressionsStart();
			tableColumnVisibility = new bool[this.m_tableColumns.Count];
			for (int i = 0; i < this.m_tableColumns.Count; i++)
			{
				this.m_tableColumns[i].Initialize(context);
				tableWidth += this.m_tableColumns[i].WidthValue;
				tableColumnVisibility[i] = (this.m_tableColumns[i].Visibility == null || this.m_tableColumns[i].Visibility.Hidden == null || this.m_tableColumns[i].Visibility.Toggle != null || (ExpressionInfo.Types.Constant == this.m_tableColumns[i].Visibility.Hidden.Type && !this.m_tableColumns[i].Visibility.Hidden.BoolValue));
			}
			context.ExprHostBuilder.TableColumnVisibilityHiddenExpressionsEnd();
		}

		private void RegisterTableColumnsReceiver(InitializationContext context)
		{
			for (int i = 0; i < this.m_tableColumns.Count; i++)
			{
				this.m_tableColumns[i].RegisterReceiver(context);
			}
		}

		private void InitializeHeaderAndFooter(int numberOfColumns, InitializationContext context, ref double tableHeight, bool[] tableColumnVisibility)
		{
			context.ExprHostBuilder.TableRowVisibilityHiddenExpressionsStart();
			if (this.m_headerRows != null)
			{
				for (int i = 0; i < this.m_headerRows.Count; i++)
				{
					Global.Tracer.Assert(null != this.m_headerRows[i]);
					this.m_headerRows[i].Initialize(false, numberOfColumns, context, ref tableHeight, tableColumnVisibility);
				}
			}
			if (this.m_footerRows != null)
			{
				for (int j = 0; j < this.m_footerRows.Count; j++)
				{
					Global.Tracer.Assert(null != this.m_footerRows[j]);
					this.m_footerRows[j].Initialize(false, numberOfColumns, context, ref tableHeight, tableColumnVisibility);
				}
			}
			context.ExprHostBuilder.TableRowVisibilityHiddenExpressionsEnd();
		}

		private void RegisterHeaderAndFooterReceiver(InitializationContext context)
		{
			if (this.m_headerRows != null)
			{
				for (int i = 0; i < this.m_headerRows.Count; i++)
				{
					Global.Tracer.Assert(null != this.m_headerRows[i]);
					this.m_headerRows[i].RegisterReceiver(context);
				}
			}
			if (this.m_footerRows != null)
			{
				for (int j = 0; j < this.m_footerRows.Count; j++)
				{
					Global.Tracer.Assert(null != this.m_footerRows[j]);
					this.m_footerRows[j].RegisterReceiver(context);
				}
			}
		}

		private void InitializeTableGroupsOrDetail(int numberOfColumns, InitializationContext context, ref double tableHeight, bool[] tableColumnVisibility)
		{
			TableGroup tableGroup = this.m_detailGroup;
			if (tableGroup != null && this.m_tableGroups == null)
			{
				this.m_tableGroups = this.m_detailGroup;
				tableGroup = null;
			}
			if (this.m_tableGroups != null)
			{
				this.m_tableGroups.Initialize(numberOfColumns, this.m_tableDetail, tableGroup, context, ref tableHeight, tableColumnVisibility);
			}
			else if (this.m_tableDetail != null)
			{
				this.m_tableDetail.Initialize(numberOfColumns, context, ref tableHeight, tableColumnVisibility);
			}
		}

		private void RegisterTableGroupsOrDetailReceiver(InitializationContext context)
		{
			if (this.m_tableGroups != null)
			{
				this.m_tableGroups.RegisterReceiver(context, this.m_tableDetail);
			}
			else if (this.m_tableDetail != null)
			{
				this.m_tableDetail.RegisterReceiver(context);
			}
		}

		RunningValueInfoList IRunningValueHolder.GetRunningValueList()
		{
			return this.m_runningValues;
		}

		void IRunningValueHolder.ClearIfEmpty()
		{
			Global.Tracer.Assert(null != this.m_runningValues);
			if (this.m_runningValues.Count == 0)
			{
				this.m_runningValues = null;
			}
		}

		internal void CalculatePropagatedFlags()
		{
			bool flag = true;
			if (this.m_tableGroups != null)
			{
				this.m_tableGroups.CalculatePropagatedFlags(out this.m_groupPageBreakAtStart, out this.m_groupPageBreakAtEnd);
				if (this.m_tableGroups.HeaderRows != null)
				{
					flag = this.m_tableGroups.HeaderRepeatOnNewPage;
				}
				this.m_propagatedPageBreakAtStart = (this.m_tableGroups.Grouping.PageBreakAtStart || (this.m_tableGroups.PropagatedPageBreakAtStart && flag));
				flag = true;
				if (this.m_tableGroups.FooterRows != null)
				{
					flag = this.m_tableGroups.FooterRepeatOnNewPage;
				}
				this.m_propagatedPageBreakAtEnd = (this.m_tableGroups.Grouping.PageBreakAtEnd || (this.m_tableGroups.PropagatedPageBreakAtEnd && flag));
			}
		}

		bool IPageBreakItem.IgnorePageBreaks()
		{
			if (base.m_pagebreakState == PageBreakStates.Unknown)
			{
				base.m_pagebreakState = PageBreakStates.CanIgnore;
				if (SharedHiddenState.Never == Visibility.GetSharedHidden(base.m_visibility))
				{
					if (this.m_tableColumns != null)
					{
						int i;
						for (i = 0; i < this.m_tableColumns.Count && SharedHiddenState.Never != Visibility.GetSharedHidden(this.m_tableColumns[i].Visibility); i++)
						{
						}
						if (i < this.m_tableColumns.Count)
						{
							base.m_pagebreakState = PageBreakStates.CannotIgnore;
						}
					}
					if (PageBreakStates.CannotIgnore == base.m_pagebreakState)
					{
						if (this.m_tableGroups == null)
						{
							if (this.m_tableDetail != null)
							{
								if (SharedHiddenState.Never != Visibility.GetSharedHidden(this.m_tableDetail.Visibility))
								{
									base.m_pagebreakState = PageBreakStates.CanIgnore;
								}
								for (int j = 0; j < this.m_tableDetail.DetailRows.Count; j++)
								{
									if (SharedHiddenState.Never != Visibility.GetSharedHidden(this.m_tableDetail.DetailRows[j].Visibility))
									{
										base.m_pagebreakState = PageBreakStates.CanIgnore;
									}
								}
							}
						}
						else if (SharedHiddenState.Never != Visibility.GetSharedHidden(this.m_tableGroups.Visibility))
						{
							base.m_pagebreakState = PageBreakStates.CanIgnore;
						}
					}
				}
			}
			if (PageBreakStates.CanIgnore == base.m_pagebreakState)
			{
				return true;
			}
			return false;
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID >= 0)
			{
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null);
				this.m_exprHost = reportExprHost.TableHostsRemotable[base.ExprHostID];
				base.DataRegionSetExprHost(this.m_exprHost, reportObjectModel);
				if (this.m_exprHost.TableColumnVisibilityHiddenExpressions != null)
				{
					this.m_exprHost.TableColumnVisibilityHiddenExpressions.SetReportObjectModel(reportObjectModel);
				}
				if (this.m_exprHost.TableRowVisibilityHiddenExpressions != null)
				{
					this.m_exprHost.TableRowVisibilityHiddenExpressions.SetReportObjectModel(reportObjectModel);
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.TableColumns, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.TableColumnList));
			memberInfoList.Add(new MemberInfo(MemberName.HeaderRows, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.TableRowList));
			memberInfoList.Add(new MemberInfo(MemberName.HeaderRepeatOnNewPage, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.TableGroups, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.TableGroup));
			memberInfoList.Add(new MemberInfo(MemberName.TableDetail, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.TableDetail));
			memberInfoList.Add(new MemberInfo(MemberName.DetailGroup, Token.Reference, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.TableGroup));
			memberInfoList.Add(new MemberInfo(MemberName.FooterRows, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.TableRowList));
			memberInfoList.Add(new MemberInfo(MemberName.FooterRepeatOnNewPage, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.PropagatedPageBreakAtStart, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.GroupPageBreakAtStart, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.PropagatedPageBreakAtEnd, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.GroupPageBreakAtEnd, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.FillPage, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.UseOwc, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.OwcNonSharedStyles, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.RunningValues, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.RunningValueInfoList));
			memberInfoList.Add(new MemberInfo(MemberName.DetailDataElementName, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.DetailDataCollectionName, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.DetailDataElementOutput, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.FixedHeader, Token.Boolean));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.DataRegion, memberInfoList);
		}
	}
}
