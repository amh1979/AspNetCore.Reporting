using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal sealed class Tablix : DataRegion, ICreateSubtotals, IPersistable
	{
		internal enum MarginPosition
		{
			TopMargin,
			BottomMargin,
			LeftMargin,
			RightMargin
		}

		internal class InitData
		{
			internal bool HasFixedColData;

			internal int FixedColStartIndex;

			internal int FixedColLength;

			internal bool HasFixedRowData;

			internal bool IsTopLevelDataRegion;

			internal IList<Pair<double, int>> RowHeaderLevelSizeList;

			internal IList<Pair<double, int>> ColumnHeaderLevelSizeList;
		}

		private class SizeCalculator
		{
			private Tablix m_tablix;

			internal SizeCalculator(Tablix tablix)
			{
				this.m_tablix = tablix;
			}

			internal void CalculateSizes(InitializationContext context)
			{
				if (this.m_tablix.Corner != null)
				{
					this.CalculateCornerSizes(context);
				}
				if (this.m_tablix.TablixRowMembers != null)
				{
					this.CalculateMemberSizes(context, this.m_tablix.TablixRowMembers, false, 0);
				}
				if (this.m_tablix.TablixColumnMembers != null)
				{
					this.CalculateMemberSizes(context, this.m_tablix.TablixColumnMembers, true, 0);
				}
				if (this.m_tablix.TablixRows != null)
				{
					this.CalculateCellSizes(context);
				}
			}

			private void CalculateCellSizes(InitializationContext context)
			{
				for (int i = 0; i < this.m_tablix.TablixRows.Count; i++)
				{
					TablixCellList tablixCells = this.m_tablix.TablixRows[i].TablixCells;
					for (int j = 0; j < tablixCells.Count; j++)
					{
						TablixCell tablixCell = tablixCells[j];
						if (tablixCell.CellContents != null)
						{
							double num = 0.0;
							double heightValue = this.m_tablix.TablixRows[i].HeightValue;
							for (int k = j; k < tablixCell.ColSpan + j; k++)
							{
								num += this.m_tablix.TablixColumns[k].WidthValue;
							}
							tablixCell.CellContents.CalculateSizes(num, heightValue, context, true);
							if (tablixCell.AltCellContents != null)
							{
								tablixCell.AltCellContents.CalculateSizes(num, heightValue, context, true);
							}
						}
					}
				}
			}

			private void CalculateMemberSizes(InitializationContext context, TablixMemberList members, bool isColumn, int index)
			{
				int num = index;
				for (int i = 0; i < members.Count; i++)
				{
					double num2 = 0.0;
					double num3 = 0.0;
					double num4 = 0.0;
					double num5 = 0.0;
					TablixMember tablixMember = members[i];
					if (tablixMember.TablixHeader != null && tablixMember.TablixHeader.CellContents != null)
					{
						if (isColumn)
						{
							num2 = tablixMember.TablixHeader.SizeValue;
							num5 = num2;
							for (int j = num; j < tablixMember.ColSpan + num; j++)
							{
								TablixColumn tablixColumn = this.m_tablix.TablixColumns[j];
								if (!tablixColumn.ForAutoSubtotal)
								{
									num4 += tablixColumn.WidthValue;
								}
								num3 += tablixColumn.WidthValue;
							}
						}
						else
						{
							num3 = tablixMember.TablixHeader.SizeValue;
							num4 += num3;
							for (int k = num; k < tablixMember.RowSpan + num; k++)
							{
								TablixRow tablixRow = this.m_tablix.TablixRows[k];
								if (!tablixRow.ForAutoSubtotal)
								{
									num5 += tablixRow.HeightValue;
								}
								num2 += tablixRow.HeightValue;
							}
						}
						AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType = tablixMember.TablixHeader.CellContents.ObjectType;
						if (objectType == AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart || objectType == AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel || objectType == AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map)
						{
							tablixMember.TablixHeader.CellContents.CalculateSizes(num4, num5, context, true);
						}
						else
						{
							tablixMember.TablixHeader.CellContents.CalculateSizes(num3, num2, context, true);
						}
						if (tablixMember.TablixHeader.AltCellContents != null)
						{
							objectType = tablixMember.TablixHeader.AltCellContents.ObjectType;
							if (objectType == AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart || objectType == AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel || objectType == AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map)
							{
								tablixMember.TablixHeader.AltCellContents.CalculateSizes(num4, num5, context, true);
							}
							else
							{
								tablixMember.TablixHeader.AltCellContents.CalculateSizes(num3, num2, context, true);
							}
						}
					}
					if (tablixMember.SubMembers != null)
					{
						this.CalculateMemberSizes(context, tablixMember.SubMembers, isColumn, num);
					}
					num = ((!isColumn) ? (num + tablixMember.RowSpan) : (num + tablixMember.ColSpan));
				}
			}

			private void CalculateCornerSizes(InitializationContext context)
			{
				double num = 0.0;
				double num2 = 0.0;
				for (int i = 0; i < this.m_tablix.Corner.Count; i++)
				{
					List<TablixCornerCell> list = this.m_tablix.Corner[i];
					for (int j = 0; j < list.Count; j++)
					{
						TablixCornerCell tablixCornerCell = list[j];
						if (tablixCornerCell.CellContents != null)
						{
							num = context.GetHeaderSize(this.m_tablix.InitializationData.ColumnHeaderLevelSizeList, i, tablixCornerCell.RowSpan);
							num2 = context.GetHeaderSize(this.m_tablix.InitializationData.RowHeaderLevelSizeList, j, tablixCornerCell.ColSpan);
							tablixCornerCell.CellContents.CalculateSizes(num2, num, context, true);
							if (tablixCornerCell.AltCellContents != null)
							{
								tablixCornerCell.AltCellContents.CalculateSizes(num2, num, context, true);
							}
						}
					}
				}
			}
		}

		private sealed class IndexInCollectionUpgrader
		{
			private Dictionary<Hashtable, int> m_indexInCollectionTable = new Dictionary<Hashtable, int>(InitializationContext.HashtableKeyComparer.Instance);

			private Hashtable m_groupingScopes = new Hashtable();

			internal void RegisterGroup(string groupName)
			{
				this.m_groupingScopes.Add(groupName, null);
			}

			internal void UnregisterGroup(string groupName)
			{
				this.m_groupingScopes.Remove(groupName);
			}

			internal void SetIndexInCollection(TablixCell indexedInCollection)
			{
				Hashtable key = (Hashtable)this.m_groupingScopes.Clone();
				int num = default(int);
				if (this.m_indexInCollectionTable.TryGetValue(key, out num))
				{
					num++;
					this.m_indexInCollectionTable[key] = num;
				}
				else
				{
					num = 0;
					this.m_indexInCollectionTable.Add(key, num);
				}
				indexedInCollection.IndexInCollection = num;
			}
		}

		private bool m_canScroll;

		private bool m_keepTogether;

		private TablixMemberList m_tablixColumnMembers;

		private TablixMemberList m_tablixRowMembers;

		private TablixRowList m_tablixRows;

		private List<TablixColumn> m_tablixColumns;

		private List<List<TablixCornerCell>> m_corner;

		private PageBreakLocation m_propagatedPageBreakLocation;

		private int m_innerRowLevelWithPageBreak = -1;

		private int m_groupsBeforeRowHeaders;

		private bool m_layoutDirection;

		private bool m_repeatColumnHeaders;

		private bool m_repeatRowHeaders;

		private bool m_fixedColumnHeaders;

		private bool m_fixedRowHeaders;

		private bool m_omitBorderOnPageBreak;

		private bool m_hideStaticsIfNoRows = true;

		[Reference]
		private List<TextBox> m_inScopeTextBoxes;

		private int m_columnHeaderRowCount;

		private int m_rowHeaderColumnCount;

		private BandLayoutOptions m_bandLayout;

		private ExpressionInfo m_topMargin;

		private ExpressionInfo m_bottomMargin;

		private ExpressionInfo m_leftMargin;

		private ExpressionInfo m_rightMargin;

		private bool m_enableRowDrilldown;

		private bool m_enableColumnDrilldown;

		[NonSerialized]
		private InitData m_initData;

		[NonSerialized]
		private bool m_createdSubtotals;

		[NonSerialized]
		private static readonly Declaration m_Declaration = Tablix.GetDeclaration();

		[NonSerialized]
		private bool m_computeHeight;

		[NonSerialized]
		private bool m_computeWidth;

		[NonSerialized]
		private TablixExprHost m_tablixExprHost;

		internal bool CanScroll
		{
			get
			{
				return this.m_canScroll;
			}
			set
			{
				this.m_canScroll = value;
			}
		}

		internal bool ComputeHeight
		{
			get
			{
				return this.m_computeHeight;
			}
			set
			{
				this.m_computeHeight = value;
			}
		}

		internal bool ComputeWidth
		{
			get
			{
				return this.m_computeWidth;
			}
			set
			{
				this.m_computeWidth = value;
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

		internal override AspNetCore.ReportingServices.ReportProcessing.ObjectType ObjectType
		{
			get
			{
				return AspNetCore.ReportingServices.ReportProcessing.ObjectType.Tablix;
			}
		}

		internal override HierarchyNodeList ColumnMembers
		{
			get
			{
				return this.m_tablixColumnMembers;
			}
		}

		internal override HierarchyNodeList RowMembers
		{
			get
			{
				return this.m_tablixRowMembers;
			}
		}

		internal override RowList Rows
		{
			get
			{
				return this.m_tablixRows;
			}
		}

		internal TablixMemberList TablixColumnMembers
		{
			get
			{
				return this.m_tablixColumnMembers;
			}
			set
			{
				this.m_tablixColumnMembers = value;
			}
		}

		internal TablixMemberList TablixRowMembers
		{
			get
			{
				return this.m_tablixRowMembers;
			}
			set
			{
				this.m_tablixRowMembers = value;
			}
		}

		internal TablixRowList TablixRows
		{
			get
			{
				return this.m_tablixRows;
			}
			set
			{
				this.m_tablixRows = value;
			}
		}

		internal List<TablixColumn> TablixColumns
		{
			get
			{
				return this.m_tablixColumns;
			}
			set
			{
				this.m_tablixColumns = value;
			}
		}

		internal List<List<TablixCornerCell>> Corner
		{
			get
			{
				return this.m_corner;
			}
			set
			{
				this.m_corner = value;
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

		public bool RepeatColumnHeaders
		{
			get
			{
				return this.m_repeatColumnHeaders;
			}
			set
			{
				this.m_repeatColumnHeaders = value;
			}
		}

		public bool RepeatRowHeaders
		{
			get
			{
				return this.m_repeatRowHeaders;
			}
			set
			{
				this.m_repeatRowHeaders = value;
			}
		}

		internal bool FixedColumnHeaders
		{
			get
			{
				return this.m_fixedColumnHeaders;
			}
			set
			{
				this.m_fixedColumnHeaders = value;
			}
		}

		internal bool FixedRowHeaders
		{
			get
			{
				return this.m_fixedRowHeaders;
			}
			set
			{
				this.m_fixedRowHeaders = value;
			}
		}

		internal int ColumnHeaderRowCount
		{
			get
			{
				return this.m_columnHeaderRowCount;
			}
			set
			{
				this.m_columnHeaderRowCount = value;
			}
		}

		internal int RowHeaderColumnCount
		{
			get
			{
				return this.m_rowHeaderColumnCount;
			}
			set
			{
				this.m_rowHeaderColumnCount = value;
			}
		}

		internal bool OmitBorderOnPageBreak
		{
			get
			{
				return this.m_omitBorderOnPageBreak;
			}
			set
			{
				this.m_omitBorderOnPageBreak = value;
			}
		}

		internal bool HideStaticsIfNoRows
		{
			get
			{
				return this.m_hideStaticsIfNoRows;
			}
			set
			{
				this.m_hideStaticsIfNoRows = value;
			}
		}

		internal TablixExprHost TablixExprHost
		{
			get
			{
				return this.m_tablixExprHost;
			}
		}

		internal BandLayoutOptions BandLayout
		{
			get
			{
				return this.m_bandLayout;
			}
			set
			{
				this.m_bandLayout = value;
			}
		}

		internal ExpressionInfo TopMargin
		{
			get
			{
				return this.m_topMargin;
			}
			set
			{
				this.m_topMargin = value;
			}
		}

		internal ExpressionInfo BottomMargin
		{
			get
			{
				return this.m_bottomMargin;
			}
			set
			{
				this.m_bottomMargin = value;
			}
		}

		internal ExpressionInfo LeftMargin
		{
			get
			{
				return this.m_leftMargin;
			}
			set
			{
				this.m_leftMargin = value;
			}
		}

		internal ExpressionInfo RightMargin
		{
			get
			{
				return this.m_rightMargin;
			}
			set
			{
				this.m_rightMargin = value;
			}
		}

		internal bool EnableRowDrilldown
		{
			get
			{
				return this.m_enableRowDrilldown;
			}
			set
			{
				this.m_enableRowDrilldown = value;
			}
		}

		internal bool EnableColumnDrilldown
		{
			get
			{
				return this.m_enableColumnDrilldown;
			}
			set
			{
				this.m_enableColumnDrilldown = value;
			}
		}

		protected override IndexedExprHost UserSortExpressionsHost
		{
			get
			{
				if (this.m_tablixExprHost == null)
				{
					return null;
				}
				return this.m_tablixExprHost.UserSortExpressionsHost;
			}
		}

		internal InitData InitializationData
		{
			get
			{
				if (this.m_initData == null)
				{
					this.m_initData = new InitData();
				}
				return this.m_initData;
			}
		}

		internal List<TextBox> InScopeTextBoxes
		{
			get
			{
				return this.m_inScopeTextBoxes;
			}
		}

		internal Tablix(ReportItem parent)
			: base(parent)
		{
		}

		internal Tablix(int id, ReportItem parent)
			: base(id, parent)
		{
		}

		protected override List<ReportItem> ComputeDataRegionScopedItemsForDataProcessing()
		{
			List<ReportItem> result = base.ComputeDataRegionScopedItemsForDataProcessing();
			if (this.Corner != null)
			{
				for (int i = 0; i < this.Corner.Count; i++)
				{
					List<TablixCornerCell> list = this.Corner[i];
					if (list != null && list.Count != 0)
					{
						for (int j = 0; j < list.Count; j++)
						{
							TablixCornerCell rifCell = list[j];
							DataRegion.MergeDataProcessingItems(rifCell, ref result);
						}
					}
				}
			}
			return result;
		}

		protected override void TraverseDataRegionLevelScopes(IRIFScopeVisitor visitor)
		{
			if (this.m_corner != null)
			{
				for (int i = 0; i < this.m_corner.Count; i++)
				{
					List<TablixCornerCell> list = this.m_corner[i];
					if (list != null)
					{
						for (int j = 0; j < list.Count; j++)
						{
							base.TraverseScopes(visitor, list[j], i, j);
						}
					}
				}
			}
		}

		internal override bool Initialize(InitializationContext context)
		{
			context.Location |= AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InTablix;
			context.ObjectType = this.ObjectType;
			context.ObjectName = base.m_name;
			context.ColumnHeaderLevelSizeList = this.m_initData.ColumnHeaderLevelSizeList;
			context.RowHeaderLevelSizeList = this.m_initData.RowHeaderLevelSizeList;
			if (!context.RegisterDataRegion(this))
			{
				return false;
			}
			context.Location |= (AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDataSet | AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDataRegion);
			bool flag = context.RegisterVisibility(base.m_visibility, this);
			context.ExprHostBuilder.DataRegionStart(AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.Tablix, base.m_name);
			base.Initialize(context);
			if (!context.ErrorContext.HasError)
			{
				SizeCalculator sizeCalculator = new SizeCalculator(this);
				sizeCalculator.CalculateSizes(context);
			}
			this.InitializeBand(context);
			base.ExprHostID = context.ExprHostBuilder.DataRegionEnd(AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.Tablix);
			if (flag)
			{
				context.UnRegisterVisibility(base.m_visibility, this);
			}
			context.UnRegisterDataRegion(this);
			return false;
		}

		private void InitializeBand(InitializationContext context)
		{
			this.ValidateMarginAndCreateExpression(this.m_topMargin, MarginPosition.TopMargin, context);
			this.ValidateMarginAndCreateExpression(this.m_bottomMargin, MarginPosition.BottomMargin, context);
			this.ValidateMarginAndCreateExpression(this.m_leftMargin, MarginPosition.LeftMargin, context);
			this.ValidateMarginAndCreateExpression(this.m_rightMargin, MarginPosition.RightMargin, context);
			if (this.m_bandLayout != null)
			{
				this.m_bandLayout.Initialize(this, context);
			}
		}

		private void ValidateMarginAndCreateExpression(ExpressionInfo marginExpression, MarginPosition position, InitializationContext context)
		{
			if (marginExpression != null)
			{
				string text = position.ToString();
				if (!marginExpression.IsExpression)
				{
					context.ValidateSize(marginExpression.OriginalText, text);
				}
				marginExpression.Initialize(text, context);
				context.ExprHostBuilder.MarginExpression(marginExpression, text);
			}
		}

		protected override void InitializeRVDirectionDependentItemsInCorner(InitializationContext context)
		{
			if (this.m_corner != null)
			{
				foreach (List<TablixCornerCell> item in this.m_corner)
				{
					foreach (TablixCornerCell item2 in item)
					{
						if (item2.ColSpan > 0 && item2.RowSpan > 0)
						{
							if (item2.CellContents != null)
							{
								item2.CellContents.InitializeRVDirectionDependentItems(context);
							}
							if (item2.AltCellContents != null)
							{
								item2.AltCellContents.InitializeRVDirectionDependentItems(context);
							}
						}
					}
				}
			}
		}

		protected override void InitializeRVDirectionDependentItems(int outerIndex, int innerIndex, InitializationContext context)
		{
			int index;
			int index2;
			if (base.m_processingInnerGrouping == ProcessingInnerGroupings.Row)
			{
				index = outerIndex;
				index2 = innerIndex;
			}
			else
			{
				index = innerIndex;
				index2 = outerIndex;
			}
			if (this.m_tablixRows != null)
			{
				TablixCell tablixCell = this.m_tablixRows[index2].TablixCells[index];
				if (tablixCell != null)
				{
					tablixCell.InitializeRVDirectionDependentItems(context);
					if (context.HasUserSorts && !context.IsDataRegionScopedCell)
					{
						base.CopyCellAggregates(tablixCell);
					}
				}
			}
		}

		protected override void DetermineGroupingExprValueCountInCorner(InitializationContext context, int groupingExprCount)
		{
			if (this.m_corner != null)
			{
				foreach (List<TablixCornerCell> item in this.m_corner)
				{
					foreach (TablixCornerCell item2 in item)
					{
						if (item2.ColSpan > 0 && item2.RowSpan > 0)
						{
							if (item2.CellContents != null)
							{
								item2.CellContents.DetermineGroupingExprValueCount(context, groupingExprCount);
							}
							if (item2.AltCellContents != null)
							{
								item2.AltCellContents.DetermineGroupingExprValueCount(context, groupingExprCount);
							}
						}
					}
				}
			}
		}

		protected override void DetermineGroupingExprValueCount(int outerIndex, int innerIndex, InitializationContext context, int groupingExprCount)
		{
			int index;
			int index2;
			if (base.m_processingInnerGrouping == ProcessingInnerGroupings.Row)
			{
				index = outerIndex;
				index2 = innerIndex;
			}
			else
			{
				index = innerIndex;
				index2 = outerIndex;
			}
			if (this.m_tablixRows != null)
			{
				TablixCell tablixCell = this.m_tablixRows[index2].TablixCells[index];
				if (tablixCell != null)
				{
					tablixCell.DetermineGroupingExprValueCount(context, groupingExprCount);
				}
			}
		}

		internal static void ValidateKeepWithGroup(TablixMemberList members, InitializationContext context)
		{
			if (members != null && Tablix.HasDynamic(members))
			{
				int num = -1;
				int num2 = -1;
				bool flag = false;
				bool? nullable = null;
				for (int i = 0; i < members.Count; i++)
				{
					if (members[i].Grouping != null)
					{
						num = i;
						flag = false;
						num2 = -1;
						nullable = null;
					}
					else
					{
						if (nullable.HasValue)
						{
							if (nullable != members[i].RepeatOnNewPage)
							{
								context.ErrorContext.Register(ProcessingErrorCode.rsInvalidRepeatOnNewPage, Severity.Error, context.ObjectType, context.ObjectName, "TablixMember", "RepeatOnNewPage", nullable.Value ? "True" : "False", members[i].RepeatOnNewPage ? "True" : "False");
							}
						}
						else
						{
							nullable = members[i].RepeatOnNewPage;
						}
						if (flag && members[i].KeepWithGroup != KeepWithGroup.After)
						{
							context.ErrorContext.Register(ProcessingErrorCode.rsInvalidKeepWithGroup, Severity.Error, context.ObjectType, context.ObjectName, "TablixMember", "KeepWithGroup", "After", (members[i].KeepWithGroup == KeepWithGroup.None) ? "None" : "Before");
						}
						else if (members[i].KeepWithGroup == KeepWithGroup.Before)
						{
							if (num == -1)
							{
								if (members[i].ParentMember != null)
								{
									members[i].KeepWithGroup = members[i].ParentMember.KeepWithGroup;
								}
								else
								{
									members[i].KeepWithGroup = KeepWithGroup.None;
								}
							}
							else if (num != i - 1 && members[i - 1].KeepWithGroup != KeepWithGroup.Before)
							{
								context.ErrorContext.Register(ProcessingErrorCode.rsInvalidKeepWithGroup, Severity.Error, context.ObjectType, context.ObjectName, "TablixMember", "KeepWithGroup", "Before", (members[i - 1].KeepWithGroup == KeepWithGroup.None) ? "None" : "After");
							}
						}
						else if (members[i].KeepWithGroup == KeepWithGroup.After)
						{
							flag = true;
							num2 = i;
						}
					}
				}
				if (flag)
				{
					for (int j = num2; j < members.Count; j++)
					{
						if (members[j].ParentMember != null)
						{
							members[j].KeepWithGroup = members[j].ParentMember.KeepWithGroup;
						}
						else
						{
							members[j].KeepWithGroup = KeepWithGroup.None;
						}
					}
				}
			}
		}

		private static bool HasDynamic(TablixMemberList members)
		{
			foreach (TablixMember member in members)
			{
				if (!member.IsStatic)
				{
					return true;
				}
			}
			return false;
		}

		protected override void InitializeCorner(InitializationContext context)
		{
			if ((this.m_columnHeaderRowCount == 0 || this.m_rowHeaderColumnCount == 0) && this.m_corner == null)
			{
				return;
			}
			if (this.m_corner == null || this.m_corner.Count != this.m_columnHeaderRowCount)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsWrongNumberOfTablixCornerRows, Severity.Error, context.ObjectType, context.ObjectName, "TablixCornerRows");
			}
			if (this.m_corner != null)
			{
				int[] array = new int[this.m_rowHeaderColumnCount];
				int[] array2 = new int[this.m_columnHeaderRowCount];
				for (int i = 0; i < this.m_corner.Count; i++)
				{
					List<TablixCornerCell> list = this.m_corner[i];
					for (int j = 0; j < list.Count; j++)
					{
						if (list.Count != this.m_rowHeaderColumnCount)
						{
							context.ErrorContext.Register(ProcessingErrorCode.rsWrongNumberOfTablixCornerCells, Severity.Error, context.ObjectType, context.ObjectName, "TablixCornerCell", i.ToString(CultureInfo.InvariantCulture.NumberFormat));
							return;
						}
						TablixCornerCell tablixCornerCell = list[j];
						Global.Tracer.Assert((tablixCornerCell.ColSpan == 0) ? (tablixCornerCell.RowSpan == 0) : (tablixCornerCell.RowSpan > 0), "(((cell.ColSpan == 0) ? (cell.RowSpan == 0) : cell.RowSpan > 0))");
						for (int k = 0; k < tablixCornerCell.ColSpan && j + k < array.Length; k++)
						{
							array[j + k] += tablixCornerCell.RowSpan;
						}
						for (int l = 0; l < tablixCornerCell.RowSpan && i + l < array2.Length; l++)
						{
							array2[i + l] += tablixCornerCell.ColSpan;
						}
						tablixCornerCell.Initialize(base.ID, -1, i, j, context);
					}
				}
				for (int m = 0; m < array.Length; m++)
				{
					if (array[m] != this.m_columnHeaderRowCount)
					{
						context.ErrorContext.Register(ProcessingErrorCode.rsInvalidTablixCornerRowSpans, Severity.Error, context.ObjectType, context.ObjectName, "TablixCornerRows", m.ToString(CultureInfo.InvariantCulture.NumberFormat));
					}
				}
				for (int n = 0; n < array2.Length; n++)
				{
					if (array2[n] != this.m_rowHeaderColumnCount)
					{
						context.ErrorContext.Register(ProcessingErrorCode.rsInvalidTablixCornerColumnSpans, Severity.Error, context.ObjectType, context.ObjectName, "TablixCornerCell", n.ToString(CultureInfo.InvariantCulture.NumberFormat));
					}
				}
			}
		}

		protected override bool InitializeRows(InitializationContext context)
		{
			double num = 0.0;
			double num2 = 0.0;
			bool result = true;
			if (this.m_tablixColumnMembers != null && this.m_tablixColumns == null)
			{
				goto IL_0051;
			}
			if (this.m_tablixColumnMembers == null && this.m_tablixColumns != null)
			{
				goto IL_0051;
			}
			if (this.m_tablixColumns != null && this.m_tablixColumns.Count != base.m_columnCount)
			{
				goto IL_0051;
			}
			if (this.m_tablixColumns != null)
			{
				foreach (TablixColumn tablixColumn in this.m_tablixColumns)
				{
					tablixColumn.Initialize(context);
					if (!tablixColumn.ForAutoSubtotal)
					{
						num += tablixColumn.WidthValue;
					}
				}
			}
			goto IL_00d1;
			IL_0051:
			context.ErrorContext.Register(ProcessingErrorCode.rsWrongNumberOfTablixColumns, Severity.Error, context.ObjectType, context.ObjectName, "TablixColumns");
			result = false;
			goto IL_00d1;
			IL_013a:
			if (this.m_tablixRows != null)
			{
				int num3 = 0;
				for (int i = 0; i < this.m_tablixRows.Count; i++)
				{
					TablixRow tablixRow = this.TablixRows[i];
					if (tablixRow == null || tablixRow.Cells == null || tablixRow.Cells.Count != base.m_columnCount)
					{
						context.ErrorContext.Register(ProcessingErrorCode.rsWrongNumberOfTablixCells, Severity.Error, context.ObjectType, context.ObjectName, "TablixCells");
						result = false;
					}
					tablixRow.Initialize(context);
					if (!tablixRow.ForAutoSubtotal)
					{
						num2 += tablixRow.HeightValue;
					}
					num3 = 0;
					for (int j = 0; j < tablixRow.Cells.Count; j++)
					{
						TablixCell tablixCell = tablixRow.TablixCells[j];
						if (tablixCell.ColSpan > 1 && !this.ValidateColSpan(this, j, tablixCell.ColSpan))
						{
							context.ErrorContext.Register(ProcessingErrorCode.rsInvalidTablixCellColSpan, Severity.Error, context.ObjectType, context.ObjectName, "TablixCell");
							result = false;
						}
						num3 += tablixCell.ColSpan;
					}
					if (num3 != base.m_columnCount)
					{
						context.ErrorContext.Register(ProcessingErrorCode.rsInvalidTablixCellColSpans, Severity.Error, context.ObjectType, context.ObjectName, "TablixCells");
						result = false;
					}
				}
			}
			Math.Round(num2, 10);
			Math.Round(num, 10);
			if (!this.CanScroll)
			{
				base.m_heightValue = num2;
				base.m_widthValue = num;
			}
			else
			{
				if (this.ComputeHeight)
				{
					base.m_heightValue = num2;
				}
				else
				{
					base.m_heightValue = new ReportSize(base.m_height).ToMillimeters();
				}
				if (this.ComputeWidth)
				{
					base.m_widthValue = num;
				}
				else
				{
					base.m_widthValue = new ReportSize(base.m_width).ToMillimeters();
				}
			}
			return result;
			IL_010c:
			context.ErrorContext.Register(ProcessingErrorCode.rsWrongNumberOfTablixRows, Severity.Error, context.ObjectType, context.ObjectName, "TablixRows");
			result = false;
			goto IL_013a;
			IL_00d1:
			if (this.m_tablixRowMembers != null && this.m_tablixRows == null)
			{
				goto IL_010c;
			}
			if (this.m_tablixRowMembers == null && this.m_tablixRows != null)
			{
				goto IL_010c;
			}
			if (this.m_tablixRows != null && this.m_tablixRows.Count != base.m_rowCount)
			{
				goto IL_010c;
			}
			goto IL_013a;
		}

		private bool ValidateColSpan(Tablix tablix, int index, int colSpan)
		{
			int num = -1;
			foreach (TablixMember columnMember in tablix.ColumnMembers)
			{
				if (!this.ValidateColSpan(columnMember, index, colSpan, ref num))
				{
					return false;
				}
				if (num >= index + colSpan - 1)
				{
					return true;
				}
			}
			return false;
		}

		private bool ValidateColSpan(TablixMember aMember, int index, int colSpan, ref int current)
		{
			if (current >= index && !aMember.IsStatic)
			{
				return false;
			}
			if (aMember.SubMembers != null && aMember.SubMembers.Count > 0)
			{
				foreach (TablixMember subMember in aMember.SubMembers)
				{
					if (!this.ValidateColSpan(subMember, index, colSpan, ref current))
					{
						return false;
					}
					if (current >= index + colSpan - 1)
					{
						return true;
					}
				}
			}
			else
			{
				current++;
			}
			if (current < index)
			{
				return true;
			}
			return aMember.IsStatic;
		}

		protected override bool ValidateInnerStructure(InitializationContext context)
		{
			if (base.m_rowCount > 0 && base.m_columnCount > 0)
			{
				if (this.m_initData.IsTopLevelDataRegion)
				{
					if (this.m_fixedRowHeaders && this.m_initData.HasFixedColData)
					{
						goto IL_0052;
					}
					if (this.m_fixedColumnHeaders && this.m_initData.HasFixedRowData)
					{
						goto IL_0052;
					}
					goto IL_009a;
				}
				if (this.m_initData.HasFixedColData || this.m_initData.HasFixedRowData || this.m_fixedRowHeaders || this.m_fixedColumnHeaders)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsFixedHeadersInInnerDataRegion, Severity.Error, context.ObjectType, context.ObjectName, "FixedHeader");
				}
				goto IL_026f;
			}
			goto IL_0292;
			IL_0292:
			return true;
			IL_009a:
			if (this.m_initData.HasFixedRowData && !this.m_tablixRowMembers[0].FixedData)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFixedDataRowPosition, Severity.Error, context.ObjectType, context.ObjectName, "FixedData");
			}
			if (this.m_initData.HasFixedColData && this.m_groupsBeforeRowHeaders > 0 && this.m_tablixColumnMembers[0].FixedData)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFixedDataColumnPosition, Severity.Error, context.ObjectType, context.ObjectName, "FixedData");
			}
			if (this.m_initData.HasFixedColData)
			{
				for (int i = 0; i < this.m_tablixRows.Count; i++)
				{
					TablixRow tablixRow = this.m_tablixRows[i];
					int num = tablixRow.TablixCells[this.m_initData.FixedColStartIndex].ColSpan;
					if (num > 0)
					{
						for (int j = this.m_initData.FixedColStartIndex + 1; j < this.m_initData.FixedColStartIndex + this.m_initData.FixedColLength; j++)
						{
							num += tablixRow.TablixCells[j].ColSpan;
						}
					}
					if (num != this.m_initData.FixedColLength)
					{
						context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFixedDataBodyCellSpans, Severity.Error, context.ObjectType, context.ObjectName, i.ToString(CultureInfo.InvariantCulture));
					}
				}
			}
			goto IL_026f;
			IL_0052:
			context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFixedHeaderOnOppositeHierarchy, Severity.Error, context.ObjectType, context.ObjectName, "FixedHeader", this.m_fixedColumnHeaders ? "FixedColumnHeaders" : "FixedRowHeaders");
			goto IL_009a;
			IL_026f:
			if (this.m_groupsBeforeRowHeaders > 0 && this.m_tablixColumnMembers[0].Grouping == null)
			{
				this.m_groupsBeforeRowHeaders = 0;
			}
			goto IL_0292;
		}

		protected override bool InitializeColumnMembers(InitializationContext context)
		{
			if (this.m_tablixColumnMembers == null)
			{
				base.m_heightValue = 0.0;
				base.m_height = "0mm";
				return false;
			}
			bool flag = base.InitializeColumnMembers(context);
			if (flag && (!this.CanScroll || this.ComputeHeight))
			{
				base.m_heightValue += context.GetTotalHeaderSize(true, this.m_columnHeaderRowCount);
				base.m_heightValue = Math.Round(base.m_heightValue, 10);
				base.m_height = AspNetCore.ReportingServices.ReportPublishing.Converter.ConvertSize(base.m_heightValue);
			}
			return flag;
		}

		protected override bool InitializeRowMembers(InitializationContext context)
		{
			if (this.m_tablixColumnMembers == null)
			{
				base.m_widthValue = 0.0;
				base.m_width = "0mm";
				return false;
			}
			Tablix.ValidateKeepWithGroup(this.m_tablixRowMembers, context);
			bool flag = base.InitializeRowMembers(context);
			if (flag && (!this.CanScroll || this.ComputeWidth))
			{
				base.m_widthValue += context.GetTotalHeaderSize(false, this.m_rowHeaderColumnCount);
				base.m_widthValue = Math.Round(base.m_widthValue, 10);
				base.m_width = AspNetCore.ReportingServices.ReportPublishing.Converter.ConvertSize(base.m_widthValue);
			}
			return flag;
		}

		protected override void InitializeData(InitializationContext context)
		{
			context.RegisterReportItems(this.m_tablixRows);
			base.InitializeData(context);
			context.UnRegisterReportItems(this.m_tablixRows);
		}

		internal bool ValidateBandReportItemReference(string reportItemName)
		{
			if (reportItemName == null)
			{
				return true;
			}
			return this.ContainsReportItemInCurrentScope(reportItemName, false, true);
		}

		private bool IsOrContainsReportItemInCurrentScope(ReportItem currentItem, string reportItemName)
		{
			if (currentItem == null)
			{
				return false;
			}
			if (string.CompareOrdinal(currentItem.Name, reportItemName) == 0)
			{
				return true;
			}
			switch (currentItem.GetObjectType())
			{
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Rectangle:
				return this.ContainsReportItemInCurrentScope(((Rectangle)currentItem).ReportItems, reportItemName);
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Tablix:
			{
				Tablix tablix = (Tablix)currentItem;
				if (tablix.HasFilters)
				{
					break;
				}
				return tablix.ContainsReportItemInCurrentScope(reportItemName, true, false);
			}
			}
			return false;
		}

		private bool ContainsReportItemInCurrentScope(ReportItemCollection items, string reportItemName)
		{
			if (items == null)
			{
				return false;
			}
			foreach (ReportItem item in items)
			{
				if (this.IsOrContainsReportItemInCurrentScope(item, reportItemName))
				{
					return true;
				}
			}
			return false;
		}

		private bool ContainsReportItemInCurrentScope(string reportItemName, bool includeCorner, bool includeDynamics)
		{
			List<int> rowCellIndices = new List<int>();
			List<int> colCellIndices = new List<int>();
			if ((!includeCorner || !this.CornerContainsReportItemInCurrentScope(reportItemName)) && !this.ContainsReportItemInCurrentScope(this.m_tablixRowMembers, reportItemName, includeDynamics, ref rowCellIndices) && !this.ContainsReportItemInCurrentScope(this.m_tablixColumnMembers, reportItemName, includeDynamics, ref colCellIndices))
			{
				return this.BodyContainsReportItemInCurrentScope(rowCellIndices, colCellIndices, reportItemName);
			}
			return true;
		}

		private bool CornerContainsReportItemInCurrentScope(string reportItemName)
		{
			if (this.m_corner == null)
			{
				return false;
			}
			foreach (List<TablixCornerCell> item in this.m_corner)
			{
				if (item != null)
				{
					foreach (TablixCornerCell item2 in item)
					{
						if (this.ContainsReportItemInCurrentScope(item2, reportItemName))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		private bool ContainsReportItemInCurrentScope(TablixCellBase cell, string reportItemName)
		{
			if (cell == null)
			{
				return false;
			}
			if (!this.IsOrContainsReportItemInCurrentScope(cell.CellContents, reportItemName))
			{
				return this.IsOrContainsReportItemInCurrentScope(cell.AltCellContents, reportItemName);
			}
			return true;
		}

		private bool ContainsReportItemInCurrentScope(TablixHeader header, string reportItemName)
		{
			if (header == null)
			{
				return false;
			}
			if (!this.IsOrContainsReportItemInCurrentScope(header.CellContents, reportItemName))
			{
				return this.IsOrContainsReportItemInCurrentScope(header.AltCellContents, reportItemName);
			}
			return true;
		}

		private bool ContainsReportItemInCurrentScope(TablixMemberList members, string reportItemName, bool includeDynamics, ref List<int> memberCellIndices)
		{
			if (members == null)
			{
				return false;
			}
			foreach (TablixMember member in members)
			{
				if (!member.IsStatic && !includeDynamics)
				{
					continue;
				}
				if (this.ContainsReportItemInCurrentScope(member.TablixHeader, reportItemName) || this.ContainsReportItemInCurrentScope(member.SubMembers, reportItemName, includeDynamics, ref memberCellIndices))
				{
					return true;
				}
				if (member.IsLeaf)
				{
					memberCellIndices.Add(member.MemberCellIndex);
				}
			}
			return false;
		}

		private bool BodyContainsReportItemInCurrentScope(List<int> rowCellIndices, List<int> colCellIndices, string reportItemName)
		{
			foreach (int rowCellIndex in rowCellIndices)
			{
				TablixRow tablixRow = this.TablixRows[rowCellIndex];
				if (tablixRow.Cells != null)
				{
					foreach (int colCellIndex in colCellIndices)
					{
						TablixCell cell = tablixRow.TablixCells[colCellIndex];
						if (this.ContainsReportItemInCurrentScope(cell, reportItemName))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			Tablix tablix = (Tablix)(context.CurrentDataRegionClone = (Tablix)base.PublishClone(context));
			if (this.m_tablixColumnMembers != null)
			{
				tablix.m_tablixColumnMembers = new TablixMemberList(this.m_tablixColumnMembers.Count);
				foreach (TablixMember tablixColumnMember in this.m_tablixColumnMembers)
				{
					tablix.m_tablixColumnMembers.Add(tablixColumnMember.PublishClone(context, tablix));
				}
			}
			if (this.m_tablixRowMembers != null)
			{
				tablix.m_tablixRowMembers = new TablixMemberList(this.m_tablixRowMembers.Count);
				foreach (TablixMember tablixRowMember in this.m_tablixRowMembers)
				{
					tablix.m_tablixRowMembers.Add(tablixRowMember.PublishClone(context, tablix));
				}
			}
			if (this.m_corner != null)
			{
				tablix.m_corner = new List<List<TablixCornerCell>>(this.m_corner.Count);
				foreach (List<TablixCornerCell> item in this.m_corner)
				{
					List<TablixCornerCell> list = new List<TablixCornerCell>(item.Count);
					foreach (TablixCornerCell item2 in item)
					{
						list.Add((TablixCornerCell)item2.PublishClone(context));
					}
					tablix.m_corner.Add(list);
				}
			}
			if (this.m_tablixRows != null)
			{
				tablix.m_tablixRows = new TablixRowList(this.m_tablixRows.Count);
				foreach (TablixRow tablixRow in this.m_tablixRows)
				{
					tablix.m_tablixRows.Add((TablixRow)tablixRow.PublishClone(context));
				}
			}
			if (this.m_tablixColumns != null)
			{
				tablix.m_tablixColumns = new List<TablixColumn>(this.m_tablixColumns.Count);
				foreach (TablixColumn tablixColumn in this.m_tablixColumns)
				{
					tablix.m_tablixColumns.Add((TablixColumn)tablixColumn.PublishClone(context));
				}
			}
			context.CreateSubtotalsDefinitions.Add(tablix);
			return tablix;
		}

		protected override ReportHierarchyNode CreateHierarchyNode(int id)
		{
			return new TablixMember(id, this);
		}

		protected override Row CreateRow(int id, int columnCount)
		{
			TablixRow tablixRow = new TablixRow(id);
			tablixRow.Height = "0mm";
			tablixRow.TablixCells = new TablixCellList(columnCount);
			return tablixRow;
		}

		protected override Cell CreateCell(int id, int rowIndex, int colIndex)
		{
			TablixCell tablixCell = new TablixCell(id, this);
			if (rowIndex != -1)
			{
				tablixCell.ColSpan = 1;
				tablixCell.RowSpan = ((TablixCell)this.Rows[rowIndex].Cells[0]).RowSpan;
			}
			else if (colIndex != -1)
			{
				tablixCell.ColSpan = ((TablixCell)this.Rows[0].Cells[colIndex]).ColSpan;
				tablixCell.RowSpan = 1;
			}
			return tablixCell;
		}

		protected override void CreateDomainScopeRowsAndCells(AutomaticSubtotalContext context, ReportHierarchyNode member)
		{
			base.CreateDomainScopeRowsAndCells(context, member);
			if (member.IsColumn)
			{
				TablixColumn tablixColumn = new TablixColumn(context.GenerateID());
				tablixColumn.Width = "0mm";
				this.m_tablixColumns.Insert(this.ColumnMembers.GetMemberIndex(member), tablixColumn);
			}
		}

		public void CreateAutomaticSubtotals(AutomaticSubtotalContext context)
		{
			if (!this.m_createdSubtotals && this.m_tablixRows != null && base.m_rowCount == this.m_tablixRows.Count && this.m_tablixColumns != null && this.m_tablixColumns.Count == base.m_columnCount)
			{
				for (int i = 0; i < this.m_tablixRows.Count; i++)
				{
					if (this.m_tablixRows[i].Cells == null || this.m_tablixRows[i].Cells.Count != base.m_columnCount)
					{
						return;
					}
				}
				context.Location = AspNetCore.ReportingServices.ReportPublishing.LocationFlags.None;
				context.ObjectType = this.ObjectType;
				context.ObjectName = "Tablix";
				context.CurrentDataRegion = this;
				context.OriginalRowCount = base.m_rowCount;
				context.OriginalColumnCount = base.m_columnCount;
				context.CellLists = new List<CellList>(this.m_tablixRows.Count);
				for (int j = 0; j < this.m_tablixRows.Count; j++)
				{
					context.CellLists.Add(new CellList());
				}
				context.TablixColumns = new List<TablixColumn>(this.m_tablixColumns.Count);
				context.Rows = new RowList(this.m_tablixRows.Count);
				context.CurrentScope = base.m_name;
				context.CurrentDataScope = this;
				context.StartIndex = 0;
				this.CreateAutomaticSubtotals(context, this.m_tablixColumnMembers, true);
				context.StartIndex = 0;
				this.CreateAutomaticSubtotals(context, this.m_tablixRowMembers, false);
				context.CurrentScope = null;
				context.CurrentDataScope = null;
				this.m_createdSubtotals = true;
			}
		}

		private int CreateAutomaticSubtotals(AutomaticSubtotalContext context, TablixMemberList members, bool isColumn)
		{
			int num = 0;
			bool flag = this.AllSiblingsHaveConditionalOrToggleableVisibility(members);
			for (int i = 0; i < members.Count; i++)
			{
				TablixMember tablixMember = members[i];
				if (tablixMember.Grouping != null && tablixMember.HasToggleableVisibility && flag)
				{
					context.CurrentIndex = context.StartIndex;
					if (isColumn)
					{
						foreach (CellList cellList in context.CellLists)
						{
							cellList.Clear();
						}
						context.TablixColumns.Clear();
					}
					else
					{
						context.Rows.Clear();
					}
					int num2 = 0;
					bool flag2 = true;
					context.HeaderLevel = tablixMember.HeaderLevel;
					Global.Tracer.Assert(tablixMember.HeaderLevelHasStaticArray != null, "(member.HeaderLevelHasStaticArray != null)");
					context.HeaderLevelHasStaticArray = tablixMember.HeaderLevelHasStaticArray;
					base.BuildAndSetupAxisScopeTreeForAutoSubtotals(ref context, tablixMember);
					TablixMember tablixMember2 = tablixMember.CreateAutomaticSubtotalClone(context, tablixMember.ParentMember, true, out num2, ref flag2);
					context.AdjustReferences();
					tablixMember2.IsAutoSubtotal = true;
					if (i + 1 < members.Count)
					{
						TablixMember tablixMember3 = members[i + 1];
						if (tablixMember3.IsStatic && tablixMember3.KeepWithGroup == KeepWithGroup.Before)
						{
							tablixMember2.KeepWithGroup = KeepWithGroup.Before;
							tablixMember2.RepeatOnNewPage = tablixMember3.RepeatOnNewPage;
						}
					}
					members.Insert(i + 1, tablixMember2);
					num = context.CurrentIndex - context.StartIndex;
					if (isColumn)
					{
						for (int j = 0; j < this.m_tablixRows.Count; j++)
						{
							this.m_tablixRows[j].Cells.InsertRange(context.CurrentIndex, context.CellLists[j]);
						}
						this.m_tablixColumns.InsertRange(context.CurrentIndex, context.TablixColumns);
						base.m_columnCount += num;
					}
					else
					{
						this.m_tablixRows.InsertRange(context.CurrentIndex, context.Rows);
						base.m_rowCount += num;
					}
					if (tablixMember.SubMembers != null)
					{
						context.CurrentScope = tablixMember.Grouping.Name;
						context.CurrentDataScope = tablixMember;
						int num3 = this.CreateAutomaticSubtotals(context, tablixMember.SubMembers, isColumn);
						if (isColumn)
						{
							tablixMember.ColSpan += num3;
						}
						else
						{
							tablixMember.RowSpan += num3;
						}
						num += num3;
					}
					else
					{
						context.StartIndex++;
					}
				}
				else if (tablixMember.SubMembers != null)
				{
					if (tablixMember.Grouping != null)
					{
						context.CurrentScope = tablixMember.Grouping.Name;
						context.CurrentDataScope = tablixMember;
					}
					int num4 = this.CreateAutomaticSubtotals(context, tablixMember.SubMembers, isColumn);
					if (isColumn)
					{
						tablixMember.ColSpan += num4;
					}
					else
					{
						tablixMember.RowSpan += num4;
					}
					num += num4;
				}
				else
				{
					context.StartIndex++;
				}
			}
			return num;
		}

		private bool AllSiblingsHaveConditionalOrToggleableVisibility(TablixMemberList members)
		{
			if (members.Count > 1)
			{
				for (int i = 0; i < members.Count; i++)
				{
					TablixMember tablixMember = members[i];
					if (!tablixMember.HasConditionalOrToggleableVisibility)
					{
						return false;
					}
				}
			}
			return true;
		}

		internal void ValidateBandStructure(PublishingContextStruct context)
		{
			int num = 0;
			int num2 = 0;
			bool flag = false;
			this.SetIgnoredPropertiesForBandingToDefault(context);
			if (this.LayoutDirection)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidBandInvalidLayoutDirection, Severity.Error, context.ObjectType, context.ObjectName, "Tablix");
			}
			foreach (TablixMember tablixColumnMember in this.TablixColumnMembers)
			{
				tablixColumnMember.ValidateTablixMemberForBanding(context, out flag);
				if (flag)
				{
					num++;
				}
			}
			foreach (TablixMember tablixRowMember in this.TablixRowMembers)
			{
				tablixRowMember.ValidateTablixMemberForBanding(context, out flag);
				if (flag)
				{
					num2++;
				}
			}
		}

		private void SetIgnoredPropertiesForBandingToDefault(PublishingContextStruct context)
		{
			if (this.GroupsBeforeRowHeaders != 0)
			{
				this.GroupsBeforeRowHeaders = 0;
				context.ErrorContext.Register(ProcessingErrorCode.rsBandIgnoredProperties, Severity.Warning, context.ObjectType, context.ObjectName, "GroupsBeforeRowHeaders");
			}
			if (this.RepeatColumnHeaders)
			{
				this.RepeatColumnHeaders = false;
				context.ErrorContext.Register(ProcessingErrorCode.rsBandIgnoredProperties, Severity.Warning, context.ObjectType, context.ObjectName, "RepeatColumnHeaders");
			}
			if (this.RepeatRowHeaders)
			{
				this.RepeatRowHeaders = false;
				context.ErrorContext.Register(ProcessingErrorCode.rsBandIgnoredProperties, Severity.Warning, context.ObjectType, context.ObjectName, "FixedColumnHeaders");
			}
			if (this.FixedColumnHeaders)
			{
				this.FixedColumnHeaders = false;
				context.ErrorContext.Register(ProcessingErrorCode.rsBandIgnoredProperties, Severity.Warning, context.ObjectType, context.ObjectName, "FixedColumnHeaders");
			}
			if (this.FixedRowHeaders)
			{
				this.FixedRowHeaders = false;
				context.ErrorContext.Register(ProcessingErrorCode.rsBandIgnoredProperties, Severity.Warning, context.ObjectType, context.ObjectName, "FixedRowHeaders");
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.TablixColumnMembers, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixMember));
			list.Add(new MemberInfo(MemberName.TablixRowMembers, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixMember));
			list.Add(new MemberInfo(MemberName.TablixRows, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixRow));
			list.Add(new MemberInfo(MemberName.TablixColumns, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixColumn));
			list.Add(new MemberInfo(MemberName.TablixCornerCells, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixCornerCell));
			list.Add(new MemberInfo(MemberName.PropagatedPageBreakLocation, Token.Enum));
			list.Add(new MemberInfo(MemberName.InnerRowLevelWithPageBreak, Token.Int32));
			list.Add(new MemberInfo(MemberName.GroupsBeforeRowHeaders, Token.Int32));
			list.Add(new MemberInfo(MemberName.LayoutDirection, Token.Boolean));
			list.Add(new MemberInfo(MemberName.RepeatColumnHeaders, Token.Boolean));
			list.Add(new MemberInfo(MemberName.RepeatRowHeaders, Token.Boolean));
			list.Add(new MemberInfo(MemberName.FixedColumnHeaders, Token.Boolean));
			list.Add(new MemberInfo(MemberName.FixedRowHeaders, Token.Boolean));
			list.Add(new MemberInfo(MemberName.OmitBorderOnPageBreak, Token.Boolean));
			list.Add(new MemberInfo(MemberName.HideStaticsIfNoRows, Token.Boolean));
			list.Add(new MemberInfo(MemberName.InScopeTextBoxes, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Token.Reference, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TextBox));
			list.Add(new MemberInfo(MemberName.ColumnHeaderRowCount, Token.Int32));
			list.Add(new MemberInfo(MemberName.RowHeaderColumnCount, Token.Int32));
			list.Add(new MemberInfo(MemberName.KeepTogether, Token.Boolean));
			list.Add(new MemberInfo(MemberName.CanScroll, Token.Boolean));
			list.Add(new MemberInfo(MemberName.BandLayout, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObject));
			list.Add(new MemberInfo(MemberName.TopMargin, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.BottomMargin, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.LeftMargin, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.RightMargin, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.EnableRowDrilldown, Token.Boolean, Lifetime.AddedIn(200)));
			list.Add(new MemberInfo(MemberName.EnableColumnDrilldown, Token.Boolean, Lifetime.AddedIn(200)));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Tablix, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegion, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(Tablix.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.CanScroll:
					writer.Write(this.m_canScroll);
					break;
				case MemberName.KeepTogether:
					writer.Write(this.m_keepTogether);
					break;
				case MemberName.TablixColumnMembers:
					writer.Write(this.m_tablixColumnMembers);
					break;
				case MemberName.TablixRowMembers:
					writer.Write(this.m_tablixRowMembers);
					break;
				case MemberName.TablixRows:
					writer.Write(this.m_tablixRows);
					break;
				case MemberName.TablixColumns:
					writer.Write(this.m_tablixColumns);
					break;
				case MemberName.TablixCornerCells:
					writer.Write(this.m_corner);
					break;
				case MemberName.PropagatedPageBreakLocation:
					writer.WriteEnum((int)this.m_propagatedPageBreakLocation);
					break;
				case MemberName.InnerRowLevelWithPageBreak:
					writer.Write(this.m_innerRowLevelWithPageBreak);
					break;
				case MemberName.GroupsBeforeRowHeaders:
					writer.Write(this.m_groupsBeforeRowHeaders);
					break;
				case MemberName.LayoutDirection:
					writer.Write(this.m_layoutDirection);
					break;
				case MemberName.RepeatColumnHeaders:
					writer.Write(this.m_repeatColumnHeaders);
					break;
				case MemberName.RepeatRowHeaders:
					writer.Write(this.m_repeatRowHeaders);
					break;
				case MemberName.FixedColumnHeaders:
					writer.Write(this.m_fixedColumnHeaders);
					break;
				case MemberName.FixedRowHeaders:
					writer.Write(this.m_fixedRowHeaders);
					break;
				case MemberName.OmitBorderOnPageBreak:
					writer.Write(this.m_omitBorderOnPageBreak);
					break;
				case MemberName.HideStaticsIfNoRows:
					writer.Write(this.m_hideStaticsIfNoRows);
					break;
				case MemberName.InScopeTextBoxes:
					writer.WriteListOfReferences(this.m_inScopeTextBoxes);
					break;
				case MemberName.ColumnHeaderRowCount:
					writer.Write(this.m_columnHeaderRowCount);
					break;
				case MemberName.RowHeaderColumnCount:
					writer.Write(this.m_rowHeaderColumnCount);
					break;
				case MemberName.BandLayout:
					writer.Write(this.m_bandLayout);
					break;
				case MemberName.TopMargin:
					writer.Write(this.m_topMargin);
					break;
				case MemberName.BottomMargin:
					writer.Write(this.m_bottomMargin);
					break;
				case MemberName.LeftMargin:
					writer.Write(this.m_leftMargin);
					break;
				case MemberName.RightMargin:
					writer.Write(this.m_rightMargin);
					break;
				case MemberName.EnableRowDrilldown:
					writer.Write(this.m_enableRowDrilldown);
					break;
				case MemberName.EnableColumnDrilldown:
					writer.Write(this.m_enableColumnDrilldown);
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
			reader.RegisterDeclaration(Tablix.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.CanScroll:
					this.m_canScroll = reader.ReadBoolean();
					break;
				case MemberName.KeepTogether:
					this.m_keepTogether = reader.ReadBoolean();
					break;
				case MemberName.TablixColumnMembers:
					this.m_tablixColumnMembers = reader.ReadListOfRIFObjects<TablixMemberList>();
					break;
				case MemberName.TablixRowMembers:
					this.m_tablixRowMembers = reader.ReadListOfRIFObjects<TablixMemberList>();
					break;
				case MemberName.TablixRows:
					this.m_tablixRows = reader.ReadListOfRIFObjects<TablixRowList>();
					break;
				case MemberName.TablixColumns:
					this.m_tablixColumns = reader.ReadGenericListOfRIFObjects<TablixColumn>();
					break;
				case MemberName.TablixCornerCells:
					this.m_corner = reader.ReadListOfListsOfRIFObjects<TablixCornerCell>();
					break;
				case MemberName.PropagatedPageBreakLocation:
					this.m_propagatedPageBreakLocation = (PageBreakLocation)reader.ReadEnum();
					break;
				case MemberName.InnerRowLevelWithPageBreak:
					this.m_innerRowLevelWithPageBreak = reader.ReadInt32();
					break;
				case MemberName.GroupsBeforeRowHeaders:
					this.m_groupsBeforeRowHeaders = reader.ReadInt32();
					break;
				case MemberName.LayoutDirection:
					this.m_layoutDirection = reader.ReadBoolean();
					break;
				case MemberName.RepeatColumnHeaders:
					this.m_repeatColumnHeaders = reader.ReadBoolean();
					break;
				case MemberName.RepeatRowHeaders:
					this.m_repeatRowHeaders = reader.ReadBoolean();
					break;
				case MemberName.FixedColumnHeaders:
					this.m_fixedColumnHeaders = reader.ReadBoolean();
					break;
				case MemberName.FixedRowHeaders:
					this.m_fixedRowHeaders = reader.ReadBoolean();
					break;
				case MemberName.OmitBorderOnPageBreak:
					this.m_omitBorderOnPageBreak = reader.ReadBoolean();
					break;
				case MemberName.HideStaticsIfNoRows:
					this.m_hideStaticsIfNoRows = reader.ReadBoolean();
					break;
				case MemberName.InScopeTextBoxes:
					this.m_inScopeTextBoxes = reader.ReadGenericListOfReferences<TextBox>(this);
					break;
				case MemberName.ColumnHeaderRowCount:
					this.m_columnHeaderRowCount = reader.ReadInt32();
					break;
				case MemberName.RowHeaderColumnCount:
					this.m_rowHeaderColumnCount = reader.ReadInt32();
					break;
				case MemberName.BandLayout:
					this.m_bandLayout = reader.ReadRIFObject<BandLayoutOptions>();
					break;
				case MemberName.TopMargin:
					this.m_topMargin = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.BottomMargin:
					this.m_bottomMargin = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.LeftMargin:
					this.m_leftMargin = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.RightMargin:
					this.m_rightMargin = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.EnableRowDrilldown:
					this.m_enableRowDrilldown = reader.ReadBoolean();
					break;
				case MemberName.EnableColumnDrilldown:
					this.m_enableColumnDrilldown = reader.ReadBoolean();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
			if (reader.IntermediateFormatVersion.CompareTo(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.IntermediateFormatVersion.RTM2008) < 0)
			{
				this.FixIndexInCollections();
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
			List<MemberReference> list = default(List<MemberReference>);
			if (memberReferencesCollection.TryGetValue(Tablix.m_Declaration.ObjectType, out list))
			{
				foreach (MemberReference item in list)
				{
					MemberName memberName = item.MemberName;
					if (memberName == MemberName.InScopeTextBoxes)
					{
						if (this.m_inScopeTextBoxes == null)
						{
							this.m_inScopeTextBoxes = new List<TextBox>();
						}
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						Global.Tracer.Assert(referenceableItems[item.RefID] is TextBox);
						this.m_inScopeTextBoxes.Add((TextBox)referenceableItems[item.RefID]);
					}
					else
					{
						Global.Tracer.Assert(false);
					}
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Tablix;
		}

		private void FixIndexInCollections()
		{
			IndexInCollectionUpgrader indexUpgrader = new IndexInCollectionUpgrader();
			if (this.m_tablixRowMembers != null && this.m_tablixColumnMembers != null)
			{
				int num = 0;
				int num2 = 0;
				foreach (TablixMember tablixRowMember in this.m_tablixRowMembers)
				{
					this.FixIndexInCollection(tablixRowMember, indexUpgrader, false, ref num, ref num2);
				}
			}
		}

		private void FixIndexInCollection(TablixMember member, IndexInCollectionUpgrader indexUpgrader, bool isColumn, ref int rowIndex, ref int colIndex)
		{
			if (!member.IsStatic)
			{
				indexUpgrader.RegisterGroup(member.Grouping.Name);
			}
			if (member.SubMembers != null && member.SubMembers.Count > 0)
			{
				foreach (TablixMember subMember in member.SubMembers)
				{
					this.FixIndexInCollection(subMember, indexUpgrader, isColumn, ref rowIndex, ref colIndex);
				}
			}
			else if (!isColumn)
			{
				colIndex = 0;
				foreach (TablixMember tablixColumnMember in this.m_tablixColumnMembers)
				{
					this.FixIndexInCollection(tablixColumnMember, indexUpgrader, true, ref rowIndex, ref colIndex);
				}
				rowIndex++;
			}
			else
			{
				TablixCell tablixCell = this.m_tablixRows[rowIndex].TablixCells[colIndex];
				if (tablixCell != null)
				{
					indexUpgrader.SetIndexInCollection(tablixCell);
				}
				colIndex++;
			}
			if (!member.IsStatic)
			{
				indexUpgrader.UnregisterGroup(member.Grouping.Name);
			}
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID >= 0)
			{
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null, "(reportExprHost != null && reportObjectModel != null)");
				this.m_tablixExprHost = reportExprHost.TablixHostsRemotable[base.ExprHostID];
				base.DataRegionSetExprHost(this.m_tablixExprHost, this.m_tablixExprHost.SortHost, this.m_tablixExprHost.FilterHostsRemotable, this.m_tablixExprHost.UserSortExpressionsHost, this.m_tablixExprHost.PageBreakExprHost, this.m_tablixExprHost.JoinConditionExprHostsRemotable, reportObjectModel);
			}
		}

		internal override void DataRegionContentsSetExprHost(ObjectModelImpl reportObjectModel, bool traverseDataRegions)
		{
			if (this.m_corner != null)
			{
				for (int i = 0; i < this.m_corner.Count; i++)
				{
					List<TablixCornerCell> list = this.m_corner[i];
					for (int j = 0; j < list.Count; j++)
					{
						TablixCornerCell tablixCornerCell = list[j];
						if (tablixCornerCell != null && tablixCornerCell.CellContents != null)
						{
							reportObjectModel.OdpContext.RuntimeInitializeReportItemObjs(tablixCornerCell.CellContents, traverseDataRegions);
							if (tablixCornerCell.AltCellContents != null)
							{
								reportObjectModel.OdpContext.RuntimeInitializeReportItemObjs(tablixCornerCell.AltCellContents, traverseDataRegions);
							}
						}
					}
				}
			}
			if (this.m_tablixRows != null)
			{
				IList<TablixCellExprHost> list2 = (this.m_tablixExprHost != null) ? this.m_tablixExprHost.CellHostsRemotable : null;
				for (int k = 0; k < this.m_tablixRows.Count; k++)
				{
					TablixRow tablixRow = this.m_tablixRows[k];
					Global.Tracer.Assert(tablixRow != null && null != tablixRow.Cells, "(null != row && null != row.Cells)");
					for (int l = 0; l < tablixRow.TablixCells.Count; l++)
					{
						TablixCell tablixCell = tablixRow.TablixCells[l];
						Global.Tracer.Assert(null != tablixCell, "(null != cell)");
						if (list2 != null && tablixCell.ExpressionHostID >= 0)
						{
							tablixCell.SetExprHost(list2[tablixCell.ExpressionHostID], reportObjectModel);
						}
						if (tablixCell.CellContents != null)
						{
							reportObjectModel.OdpContext.RuntimeInitializeReportItemObjs(tablixCell.CellContents, traverseDataRegions);
							if (tablixCell.AltCellContents != null)
							{
								reportObjectModel.OdpContext.RuntimeInitializeReportItemObjs(tablixCell.AltCellContents, traverseDataRegions);
							}
						}
					}
				}
			}
		}

		internal override object EvaluateNoRowsMessageExpression()
		{
			return this.m_tablixExprHost.NoRowsExpr;
		}

		internal string EvaluateTablixMargin(IReportScopeInstance reportScopeInstance, MarginPosition marginPosition, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			return context.ReportRuntime.EvaluateTablixMarginExpression(this, marginPosition);
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
	}
}
