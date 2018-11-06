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

namespace AspNetCore.ReportingServices.Rendering.SPBProcessing
{
	internal sealed class Tablix : PageItem
	{
		internal enum TablixRegion : byte
		{
			Unknown,
			Corner,
			ColumnHeader,
			RowHeader,
			Data
		}

		internal class RowMemberInfo : IStorable, IPersistable
		{
			private byte m_rowState;

			private double m_height;

			private static Declaration m_declaration = RowMemberInfo.GetDeclaration();

			internal byte RowState
			{
				get
				{
					return this.m_rowState;
				}
				set
				{
					this.m_rowState = value;
				}
			}

			internal bool Fixed
			{
				set
				{
					if (value)
					{
						this.m_rowState = 1;
					}
				}
			}

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

			public int Size
			{
				get
				{
					return 9;
				}
			}

			internal RowMemberInfo()
			{
			}

			internal RowMemberInfo(double height)
			{
				this.m_height = height;
			}

			public void Serialize(IntermediateFormatWriter writer)
			{
				writer.RegisterDeclaration(RowMemberInfo.m_declaration);
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.State:
						writer.Write(this.m_rowState);
						break;
					case MemberName.Height:
						writer.Write(this.m_height);
						break;
					default:
						RSTrace.RenderingTracer.Assert(false);
						break;
					}
				}
			}

			public void Deserialize(IntermediateFormatReader reader)
			{
				reader.RegisterDeclaration(RowMemberInfo.m_declaration);
				while (reader.NextMember())
				{
					switch (reader.CurrentMember.MemberName)
					{
					case MemberName.State:
						this.m_rowState = reader.ReadByte();
						break;
					case MemberName.Height:
						this.m_height = reader.ReadDouble();
						break;
					default:
						RSTrace.RenderingTracer.Assert(false);
						break;
					}
				}
			}

			public void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
			{
			}

			public ObjectType GetObjectType()
			{
				return ObjectType.RowMemberInfo;
			}

			internal static Declaration GetDeclaration()
			{
				if (RowMemberInfo.m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.State, Token.Byte));
					list.Add(new MemberInfo(MemberName.Height, Token.Double));
					return new Declaration(ObjectType.RowMemberInfo, ObjectType.None, list);
				}
				return RowMemberInfo.m_declaration;
			}
		}

		internal class SizeInfo : IStorable, IPersistable
		{
			private double m_size = double.NaN;

			private Hashtable m_spanSize;

			private bool m_fixed;

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

			internal double Value
			{
				get
				{
					if (double.IsNaN(this.m_size))
					{
						return 0.0;
					}
					return this.m_size;
				}
				set
				{
					this.m_size = value;
				}
			}

			internal bool Fixed
			{
				get
				{
					return this.m_fixed;
				}
				set
				{
					this.m_fixed = value;
				}
			}

			internal bool ZeroSized
			{
				get
				{
					return this.m_size == 0.0;
				}
			}

			internal bool Empty
			{
				get
				{
					return double.IsNaN(this.m_size);
				}
			}

			public int Size
			{
				get
				{
					return 9 + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_spanSize);
				}
			}

			internal SizeInfo()
			{
			}

			internal SizeInfo(bool fixedData)
			{
				this.m_fixed = fixedData;
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
					case MemberName.Fixed:
						writer.Write(this.m_fixed);
						break;
					case MemberName.Size:
						writer.Write(this.m_size);
						break;
					case MemberName.SpanSize:
						writer.WriteVariantVariantHashtable(this.m_spanSize);
						break;
					default:
						RSTrace.RenderingTracer.Assert(false);
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
					case MemberName.Fixed:
						this.m_fixed = reader.ReadBoolean();
						break;
					case MemberName.Size:
						this.m_size = reader.ReadDouble();
						break;
					case MemberName.SpanSize:
						this.m_spanSize = reader.ReadVariantVariantHashtable();
						break;
					default:
						RSTrace.RenderingTracer.Assert(false);
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
					list.Add(new MemberInfo(MemberName.Fixed, Token.Boolean));
					list.Add(new MemberInfo(MemberName.Size, Token.Double));
					list.Add(new MemberInfo(MemberName.SpanSize, ObjectType.VariantVariantHashtable));
					return new Declaration(ObjectType.SizeInfo, ObjectType.None, list);
				}
				return SizeInfo.m_declaration;
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

		internal class ColumnSpanInfo
		{
			private int m_start;

			private int m_span;

			private double m_spanSize;

			internal int Start
			{
				get
				{
					return this.m_start;
				}
			}

			internal int Span
			{
				get
				{
					return this.m_span;
				}
			}

			internal double SpanSize
			{
				get
				{
					return this.m_spanSize;
				}
			}

			internal ColumnSpanInfo(int start, int span, double spanSize)
			{
				this.m_start = start;
				this.m_span = span;
				this.m_spanSize = spanSize;
			}

			internal int CalculateEmptyColumnns(ScalableList<SizeInfo> sizeInfoList)
			{
				int num = 0;
				for (int i = this.Start; i < this.Start + this.Span; i++)
				{
					SizeInfo sizeInfo = sizeInfoList[i];
					if (sizeInfo == null || sizeInfo.Empty)
					{
						num++;
					}
				}
				return num;
			}
		}

		internal class InnerToggleState
		{
			private bool m_toggle;

			private List<InnerToggleState> m_children;

			internal bool Toggle
			{
				get
				{
					return this.m_toggle;
				}
			}

			internal List<InnerToggleState> Children
			{
				get
				{
					return this.m_children;
				}
			}

			internal bool HasChildren
			{
				get
				{
					if (this.m_children == null)
					{
						return false;
					}
					return this.m_children.Count > 0;
				}
			}

			internal InnerToggleState(bool toggle, List<InnerToggleState> children)
			{
				this.m_toggle = toggle;
				this.m_children = children;
			}
		}

		internal abstract class TablixContext
		{
			private bool m_noRows;

			protected bool m_isLTR;

			protected int m_headerRowCols;

			protected int m_headerColumnRows;

			protected int m_rowMemberIndexCell = -1;

			protected int m_colMemberIndexCell = -1;

			protected int m_colsBeforeRowHeaders;

			protected ScalableList<RowMemberInfo> m_rowHeights;

			protected ScalableList<SizeInfo> m_colWidths;

			protected List<SizeInfo> m_colHeaderHeights;

			protected int[] m_detailRowCellsCapacity;

			protected ScalableList<PageMemberCell> m_columnHeaders;

			private bool m_columnHeadersCreated;

			private bool m_omittedOuterColumnHeaders;

			protected ScalableList<PageMemberCell> m_rowHeaders;

			private bool m_partialRow;

			protected double m_maxDetailRowHeight;

			protected double m_maxDetailRowHeightRender;

			protected double m_lastDetailCellWidth;

			protected double m_lastDetailDefCellWidth;

			protected int m_lastDetailCellColIndex;

			protected RPLWriter m_rplWriter;

			protected PageItemHelper m_partialItemHelper;

			private Interactivity m_interactivity;

			private PageContext m_pageContext;

			private double m_cellsTopInPage;

			private int m_ignoreHeight;

			private int m_ignoreGroupPageBreaks;

			private int m_isTotal;

			private int m_ignoreRow;

			private int m_ignoreCol;

			private bool m_keepWith;

			private bool m_repeatWith;

			protected byte m_sharedLayoutRow;

			protected bool m_staticDetailRow = true;

			private bool m_pageBreakNeedsOverride;

			internal bool m_detailsOnPage;

			internal bool m_pageBreakAtEnd;

			protected bool m_propagatedPageBreak;

			private bool m_firstRecursiveToggleRow;

			private TextBox m_textBoxDelayCalc;

			private string m_currentToggleMemberPath;

			internal RPLWriter Writer
			{
				get
				{
					return this.m_rplWriter;
				}
			}

			internal IScalabilityCache Cache
			{
				get
				{
					return this.m_pageContext.ScalabilityCache;
				}
			}

			internal PageItemHelper PartialItemHelper
			{
				get
				{
					return this.m_partialItemHelper;
				}
				set
				{
					this.m_partialItemHelper = value;
				}
			}

			internal PageContext PageContext
			{
				get
				{
					return this.m_pageContext;
				}
			}

			internal Interactivity Interactivity
			{
				get
				{
					return this.m_interactivity;
				}
			}

			internal bool NoRows
			{
				get
				{
					return this.m_noRows;
				}
			}

			internal bool PageBreakAtEnd
			{
				get
				{
					return this.m_pageBreakAtEnd;
				}
				set
				{
					this.m_pageBreakAtEnd = value;
				}
			}

			internal bool PageBreakNeedsOverride
			{
				get
				{
					return this.m_pageBreakNeedsOverride;
				}
				set
				{
					this.m_pageBreakNeedsOverride = value;
				}
			}

			internal bool PropagatedPageBreak
			{
				get
				{
					return this.m_propagatedPageBreak;
				}
				set
				{
					this.m_propagatedPageBreak = value;
				}
			}

			internal bool ColumnHeadersCreated
			{
				get
				{
					return this.m_columnHeadersCreated;
				}
				set
				{
					this.m_columnHeadersCreated = value;
				}
			}

			internal bool OmittedOuterColumnHeaders
			{
				get
				{
					return this.m_omittedOuterColumnHeaders;
				}
				set
				{
					this.m_omittedOuterColumnHeaders = value;
				}
			}

			internal ScalableList<PageMemberCell> OuterColumnHeaders
			{
				get
				{
					return this.m_columnHeaders;
				}
				set
				{
					this.m_columnHeaders = value;
				}
			}

			internal ScalableList<PageMemberCell> OuterRowHeaders
			{
				get
				{
					return this.m_rowHeaders;
				}
				set
				{
					this.m_rowHeaders = value;
				}
			}

			internal int RowMemberIndexCell
			{
				get
				{
					return this.m_rowMemberIndexCell;
				}
				set
				{
					this.m_rowMemberIndexCell = value;
				}
			}

			internal int ColMemberIndexCell
			{
				get
				{
					return this.m_colMemberIndexCell;
				}
				set
				{
					this.m_colMemberIndexCell = value;
				}
			}

			internal int HeaderColumnRows
			{
				get
				{
					return this.m_headerColumnRows;
				}
			}

			internal int HeaderRowColumns
			{
				get
				{
					return this.m_headerRowCols;
				}
			}

			internal int ColsBeforeRowHeaders
			{
				get
				{
					return this.m_colsBeforeRowHeaders;
				}
				set
				{
					this.m_colsBeforeRowHeaders = value;
				}
			}

			internal double TablixBottom
			{
				get
				{
					return this.m_cellsTopInPage;
				}
			}

			internal bool CheckPageBreaks
			{
				get
				{
					if (this.m_pageContext.IgnorePageBreaks)
					{
						return false;
					}
					if (this.m_ignoreGroupPageBreaks > 0)
					{
						return false;
					}
					return true;
				}
			}

			internal bool IgnoreRow
			{
				get
				{
					return this.m_ignoreRow > 0;
				}
			}

			internal bool IgnoreColumn
			{
				get
				{
					return this.m_ignoreCol > 0;
				}
			}

			internal bool IsTotal
			{
				get
				{
					return this.m_isTotal > 0;
				}
			}

			internal bool AddToggledItems
			{
				get
				{
					return this.m_pageContext.AddToggledItems;
				}
			}

			internal bool KeepWith
			{
				get
				{
					return this.m_keepWith;
				}
			}

			internal bool RepeatWith
			{
				set
				{
					this.m_repeatWith = value;
				}
			}

			internal bool IgnoreHeight
			{
				set
				{
					if (value)
					{
						this.m_ignoreHeight++;
					}
					else
					{
						this.m_ignoreHeight--;
					}
				}
			}

			internal byte SharedLayoutRow
			{
				get
				{
					return this.m_sharedLayoutRow;
				}
				set
				{
					this.m_sharedLayoutRow = value;
				}
			}

			internal bool StaticDetailRow
			{
				set
				{
					this.m_staticDetailRow = value;
				}
			}

			internal TablixContext(RPLWriter rplWriter, PageItemHelper partialItemHelper, bool noRows, bool isLTR, PageContext pageContext, double cellsTopInPage, int headerRowCols, int headerColumnRows, int[] defDetailRowsCapacity, Interactivity interactivity)
			{
				this.m_rplWriter = rplWriter;
				this.m_partialItemHelper = partialItemHelper;
				this.m_noRows = noRows;
				this.m_pageContext = pageContext;
				this.m_cellsTopInPage = cellsTopInPage;
				this.m_headerRowCols = headerRowCols;
				this.m_headerColumnRows = headerColumnRows;
				this.m_detailRowCellsCapacity = defDetailRowsCapacity;
				this.m_interactivity = interactivity;
				this.m_isLTR = isLTR;
				this.m_pageContext.InitCache();
			}

			internal bool AlwaysHiddenMember(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember member, Visibility visibility, TablixRegion region, bool createDetail, ref LevelInfo levelInfo)
			{
				int num = 0;
				bool result = this.AlwaysHiddenMember(tablix, member, visibility, region, createDetail, ref num);
				levelInfo.IgnoredRowsCols += num;
				return result;
			}

			internal bool AlwaysHiddenMember(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember member, Visibility visibility, TablixRegion region, bool createDetail, ref int ignored)
			{
				if (visibility == null)
				{
					return false;
				}
				if (visibility.HiddenState == SharedHiddenState.Always)
				{
					this.AddMemberToCurrentPage(tablix, member, region, createDetail, true, ref ignored);
					return true;
				}
				return false;
			}

			internal void AddTotalsToCurrentPage(ref List<int> totalsIndex, TablixMemberCollection members, TablixRegion region, bool createDetail)
			{
				if (totalsIndex != null && totalsIndex.Count != 0)
				{
					int num = 0;
					for (int i = 0; i < totalsIndex.Count; i++)
					{
						num = totalsIndex[i];
						this.AddMemberToCurrentPage(null, ((ReportElementCollectionBase<TablixMember>)members)[num], region, createDetail);
					}
					totalsIndex = null;
				}
			}

			internal void AddMemberToCurrentPage(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember member, TablixRegion region, bool createDetail)
			{
				int num = 0;
				this.AddMemberToCurrentPage(tablix, member, region, createDetail, false, ref num);
			}

			private void AddMemberToCurrentPage(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember member, TablixRegion region, bool createDetail, bool headerFooterEval, ref int ignored)
			{
				if (!this.m_pageContext.CancelPage)
				{
					bool flag = false;
					Interactivity interactivity = null;
					if (headerFooterEval && this.m_pageContext.EvaluatePageHeaderFooter && this.m_rplWriter != null)
					{
						flag = true;
					}
					if (this.m_interactivity != null && this.m_interactivity.RegisterHiddenItems)
					{
						interactivity = this.m_interactivity;
					}
					if (!flag && interactivity == null)
					{
						return;
					}
					int num = 0;
					if (region == TablixRegion.RowHeader)
					{
						num = WalkTablix.AddMembersToCurrentPage(tablix, member, member.MemberCellIndex, WalkTablix.State.RowMembers, true, this.m_noRows, this.m_pageContext, flag, interactivity);
					}
					else
					{
						if (this.m_columnHeadersCreated)
						{
							WalkTablix.AddMembersToCurrentPage(tablix, member, this.m_rowMemberIndexCell, WalkTablix.State.DetailRows, true, this.m_noRows, this.m_pageContext, flag, interactivity);
							return;
						}
						num = WalkTablix.AddMembersToCurrentPage(tablix, member, this.m_rowMemberIndexCell, WalkTablix.State.ColMembers, createDetail, this.m_noRows, this.m_pageContext, flag, interactivity);
					}
					if (interactivity != null)
					{
						interactivity.RegisterGroupLabel(member.Group, this.m_pageContext);
					}
					if (member.TablixHeader != null)
					{
						if (num > 0)
						{
							RegisterItem.RegisterHiddenItem(member.TablixHeader.CellContents.ReportItem, this.m_pageContext, flag, interactivity);
							ignored += num;
						}
						else if (interactivity != null)
						{
							RegisterItem.RegisterHiddenItem(member.TablixHeader.CellContents.ReportItem, this.m_pageContext, false, interactivity);
						}
					}
				}
			}

			internal void AddMemberHeaderToCurrentPage(TablixMember member, TablixRegion region, bool headerFooterEval)
			{
				if (member != null && !this.m_pageContext.CancelPage && (!this.m_columnHeadersCreated || region != TablixRegion.ColumnHeader))
				{
					if (this.m_interactivity != null)
					{
						this.m_interactivity.RegisterGroupLabel(member.Group, this.m_pageContext);
					}
					if (member.TablixHeader != null)
					{
						bool flag = false;
						if (headerFooterEval && this.m_pageContext.EvaluatePageHeaderFooter && this.m_rplWriter != null)
						{
							flag = true;
						}
						if (!flag)
						{
							if (this.m_interactivity == null)
							{
								return;
							}
							if (!this.m_interactivity.RegisterHiddenItems)
							{
								return;
							}
						}
						if (region != TablixRegion.RowHeader && this.m_columnHeadersCreated)
						{
							return;
						}
						RegisterItem.RegisterHiddenItem(member.TablixHeader.CellContents.ReportItem, this.m_pageContext, flag, this.m_interactivity);
					}
				}
			}

			internal void AddDetailRowToCurrentPage(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix)
			{
				if (!this.m_pageContext.CancelPage)
				{
					bool flag = false;
					if (this.m_pageContext.EvaluatePageHeaderFooter && this.m_rplWriter != null)
					{
						flag = true;
					}
					if (!flag)
					{
						if (this.m_interactivity == null)
						{
							return;
						}
						if (!this.m_interactivity.RegisterHiddenItems)
						{
							return;
						}
					}
					WalkTablix.AddMembersToCurrentPage(tablix, null, this.m_rowMemberIndexCell, WalkTablix.State.DetailRows, true, this.m_noRows, this.m_pageContext, flag, this.m_interactivity);
				}
			}

			internal void AddDetailCellToCurrentPage(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix, int colMemberIndex)
			{
				if (!this.m_pageContext.CancelPage)
				{
					bool flag = false;
					if (this.m_pageContext.EvaluatePageHeaderFooter && this.m_rplWriter != null)
					{
						flag = true;
					}
					if (!flag)
					{
						if (this.m_interactivity == null)
						{
							return;
						}
						if (!this.m_interactivity.RegisterHiddenItems)
						{
							return;
						}
					}
					WalkTablix.AddDetailCellToCurrentPage(tablix, colMemberIndex, this.m_rowMemberIndexCell, this.m_pageContext, flag, this.m_interactivity);
				}
			}

			internal bool EnterColMemberInstance(TablixMember colMember, Visibility visibility, bool hasRecursivePeer, bool hasVisibleStaticPeer, out byte memberState)
			{
				memberState = 0;
				if (colMember.IsTotal)
				{
					if (this.m_pageContext.AddToggledItems && !hasRecursivePeer)
					{
						return !hasVisibleStaticPeer;
					}
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
				if (visibility.ToggleItem != null)
				{
					if (this.m_pageContext.AddToggledItems)
					{
						memberState = 2;
					}
					if (colMember.Instance.Visibility.CurrentlyHidden)
					{
						if (this.m_pageContext.AddToggledItems)
						{
							memberState |= 4;
						}
						else
						{
							this.m_ignoreCol++;
						}
					}
					return true;
				}
				return !colMember.Instance.Visibility.CurrentlyHidden;
			}

			internal void LeaveColMemberInstance(TablixMember colMember, Visibility visibility)
			{
				if (!colMember.IsTotal && visibility != null && visibility.HiddenState != SharedHiddenState.Never && visibility.ToggleItem != null)
				{
					bool flag = false;
					if (!this.m_pageContext.AddToggledItems)
					{
						flag = colMember.Instance.Visibility.CurrentlyHidden;
					}
					if (flag)
					{
						this.m_ignoreCol--;
					}
				}
			}

			internal bool EnterRowMemberInstance(TablixMember rowMember, Visibility visibility, bool hasRecursivePeer)
			{
				byte b = 0;
				bool flag = true;
				return this.EnterRowMemberInstance((Tablix)null, rowMember, (double[])null, visibility, hasRecursivePeer, (InnerToggleState)null, out b, ref flag);
			}

			internal bool EnterRowMemberInstance(Tablix tablix, TablixMember rowMember, double[] tablixCellHeights, Visibility visibility, bool hasRecursivePeer, InnerToggleState toggleState, out byte memberState, ref bool advanceRow)
			{
				memberState = 0;
				if (rowMember.IsTotal)
				{
					return this.m_pageContext.AddToggledItems;
				}
				if (visibility != null && visibility.HiddenState != SharedHiddenState.Never)
				{
					if (visibility.ToggleItem != null)
					{
						if (this.m_pageContext.AddToggledItems)
						{
							memberState = 2;
						}
						if (rowMember.Instance.Visibility.CurrentlyHidden)
						{
							if (this.m_pageContext.AddToggledItems)
							{
								memberState |= 4;
							}
							else
							{
								this.m_ignoreRow++;
							}
						}
						return true;
					}
					if (hasRecursivePeer)
					{
						this.EnterRecursiveRowMemberInstance(rowMember, tablixCellHeights, toggleState, ref advanceRow);
					}
					return !rowMember.Instance.Visibility.CurrentlyHidden;
				}
				if (hasRecursivePeer)
				{
					this.EnterRecursiveRowMemberInstance(rowMember, tablixCellHeights, toggleState, ref advanceRow);
				}
				return true;
			}

			internal bool EnterRowMember(Tablix tablix, TablixMember rowMember, Visibility visibility, bool hasRecursivePeer, bool hasVisibleStaticPeer)
			{
				byte b = 0;
				return this.EnterRowMember(tablix, rowMember, visibility, (InnerToggleState)null, hasRecursivePeer, hasVisibleStaticPeer, out b);
			}

			internal bool EnterRowMember(Tablix tablix, TablixMember rowMember, Visibility visibility, InnerToggleState toggleState, bool hasRecursivePeer, bool hasVisibleStaticPeer, out byte memberState)
			{
				memberState = 0;
				if (rowMember.IsTotal)
				{
					this.m_ignoreGroupPageBreaks++;
					this.m_ignoreHeight++;
					this.m_isTotal++;
					if (this.m_pageContext.AddToggledItems && !hasRecursivePeer)
					{
						return !hasVisibleStaticPeer;
					}
					return false;
				}
				if (visibility != null && visibility.HiddenState != SharedHiddenState.Never)
				{
					if (visibility.ToggleItem != null)
					{
						if (this.m_pageContext.AddToggledItems)
						{
							memberState = 2;
						}
						VisibilityInstance visibility2 = rowMember.Instance.Visibility;
						this.m_ignoreGroupPageBreaks++;
						if (this.PageContext.TracingEnabled)
						{
							this.RegisterToggleMemberPath(tablix, rowMember);
						}
						bool flag = false;
						flag = ((!visibility.Hidden.IsExpression) ? visibility.Hidden.Value : visibility2.StartHidden);
						if (flag && !this.m_firstRecursiveToggleRow)
						{
							this.m_ignoreHeight++;
						}
						if (visibility2.CurrentlyHidden)
						{
							if (this.m_pageContext.AddToggledItems)
							{
								memberState |= 4;
							}
							else
							{
								this.m_ignoreRow++;
							}
						}
						return true;
					}
					if (toggleState != null)
					{
						if (hasRecursivePeer && toggleState.Toggle)
						{
							goto IL_0188;
						}
						if (rowMember.IsStatic && toggleState.HasChildren)
						{
							goto IL_0188;
						}
					}
					goto IL_01a1;
				}
				if (toggleState != null)
				{
					if (hasRecursivePeer && toggleState.Toggle)
					{
						goto IL_007d;
					}
					if (rowMember.IsStatic && toggleState.HasChildren)
					{
						goto IL_007d;
					}
				}
				goto IL_00ab;
				IL_01a1:
				return !rowMember.Instance.Visibility.CurrentlyHidden;
				IL_0188:
				this.m_ignoreGroupPageBreaks++;
				if (hasRecursivePeer)
				{
					this.m_firstRecursiveToggleRow = true;
				}
				goto IL_01a1;
				IL_00ab:
				return true;
				IL_007d:
				this.m_ignoreGroupPageBreaks++;
				if (hasRecursivePeer)
				{
					this.m_firstRecursiveToggleRow = true;
				}
				if (this.PageContext.TracingEnabled)
				{
					this.RegisterToggleMemberPath(tablix, rowMember);
				}
				goto IL_00ab;
			}

			internal void EnterTotalRowMember()
			{
				this.m_ignoreGroupPageBreaks++;
				this.m_ignoreHeight++;
			}

			internal void LeaveTotalRowMember(double[] tablixCellHeights, ref bool advanceRow)
			{
				this.m_ignoreGroupPageBreaks--;
				this.m_ignoreHeight--;
				this.LeaveToggleMember(tablixCellHeights, ref advanceRow);
			}

			internal void LeaveRowMemberInstance(TablixMember rowMember, InnerToggleState toggleState, Visibility visibility, bool hasRecursivePeer)
			{
				if (!rowMember.IsTotal)
				{
					if (visibility == null || visibility.HiddenState == SharedHiddenState.Never)
					{
						if (hasRecursivePeer)
						{
							this.LeaveRecursiveRowMemberInstance(toggleState);
						}
					}
					else if (visibility.ToggleItem != null)
					{
						bool flag = false;
						if (!this.m_pageContext.AddToggledItems)
						{
							flag = rowMember.Instance.Visibility.CurrentlyHidden;
						}
						if (flag)
						{
							this.m_ignoreRow--;
						}
					}
					else if (hasRecursivePeer)
					{
						this.LeaveRecursiveRowMemberInstance(toggleState);
					}
				}
			}

			internal void LeaveRowMember(TablixMember rowMember, double[] tablixCellHeights, Visibility visibility, bool hasRecursivePeer, InnerToggleState toggleState, ref bool advanceRow)
			{
				if (this.m_pageContext.CancelPage)
				{
					advanceRow = false;
				}
				if (rowMember.IsTotal)
				{
					this.m_ignoreGroupPageBreaks--;
					this.m_ignoreHeight--;
					this.m_isTotal--;
					this.LeaveToggleMember(tablixCellHeights, ref advanceRow);
				}
				else if (visibility == null || visibility.HiddenState == SharedHiddenState.Never)
				{
					if (toggleState != null)
					{
						if (hasRecursivePeer && toggleState.Toggle)
						{
							this.m_firstRecursiveToggleRow = false;
						}
						else if (rowMember.IsStatic && toggleState.HasChildren)
						{
							this.m_ignoreGroupPageBreaks--;
							this.LeaveToggleMember(tablixCellHeights, ref advanceRow);
						}
					}
				}
				else if (visibility.ToggleItem != null)
				{
					this.m_ignoreGroupPageBreaks--;
					bool flag = false;
					flag = ((!visibility.Hidden.IsExpression) ? visibility.Hidden.Value : rowMember.Instance.Visibility.StartHidden);
					if (flag && !this.m_firstRecursiveToggleRow)
					{
						this.m_ignoreHeight--;
					}
					this.LeaveToggleMember(tablixCellHeights, ref advanceRow);
				}
				else if (toggleState != null)
				{
					if (hasRecursivePeer && toggleState.Toggle)
					{
						this.m_firstRecursiveToggleRow = false;
					}
					else if (rowMember.IsStatic && toggleState.HasChildren)
					{
						this.m_ignoreGroupPageBreaks--;
						this.LeaveToggleMember(tablixCellHeights, ref advanceRow);
					}
				}
			}

			internal void LeaveRowMember(TablixMember rowMember, double[] tablixCellHeights, double sizeForRepeatWithBefore, Visibility visibility, bool hasRecursivePeer, InnerToggleState toggleState, ref bool advanceRow)
			{
				this.m_cellsTopInPage -= sizeForRepeatWithBefore;
				this.LeaveRowMember(rowMember, tablixCellHeights, visibility, hasRecursivePeer, toggleState, ref advanceRow);
			}

			private void LeaveToggleMember(double[] tablixCellHeights, ref bool advanceRow)
			{
				if (this.m_ignoreGroupPageBreaks == 0 && !this.m_pageContext.FullOnPage)
				{
					int num = this.m_rowMemberIndexCell + 1;
					if (num >= tablixCellHeights.Length)
					{
						num = 0;
					}
					this.CheckPageHeight(num, tablixCellHeights, ref advanceRow);
				}
				if (this.PageContext.TracingEnabled && this.m_ignoreGroupPageBreaks == 0)
				{
					this.ResetToggleMemberPath();
				}
			}

			private void CheckPageHeightOnToggleMember(double[] tablixCellHeights, ref bool advanceRow)
			{
				if (this.m_ignoreGroupPageBreaks == 0 && !this.m_pageContext.FullOnPage)
				{
					this.CheckPageHeight(this.m_rowMemberIndexCell, tablixCellHeights, ref advanceRow);
				}
			}

			private void CheckPageHeight(int cellIndex, double[] tablixCellHeights, ref bool advanceRow)
			{
				if (this.m_detailsOnPage)
				{
					RoundedDouble roundedDouble = new RoundedDouble(this.m_cellsTopInPage + tablixCellHeights[cellIndex]);
					if (roundedDouble > this.m_pageContext.PageHeight)
					{
						advanceRow = false;
						if (this.m_pageContext.TracingEnabled)
						{
							RoundedDouble x = new RoundedDouble(this.m_cellsTopInPage);
							if (x > this.m_pageContext.PageHeight)
							{
								this.TracePageGrownOnImplicitKeepTogetherMember();
							}
						}
					}
					this.m_pageContext.CheckPageSize(roundedDouble);
				}
			}

			private void EnterRecursiveRowMemberInstance(TablixMember rowMember, double[] tablixCellHeights, InnerToggleState toggleState, ref bool advanceRow)
			{
				if (toggleState != null && toggleState.Toggle)
				{
					if (rowMember.Group.Instance.RecursiveLevel == 0)
					{
						this.m_firstRecursiveToggleRow = true;
						this.CheckPageHeightOnToggleMember(tablixCellHeights, ref advanceRow);
					}
					else
					{
						this.m_firstRecursiveToggleRow = false;
					}
					this.m_ignoreGroupPageBreaks++;
				}
			}

			private void LeaveRecursiveRowMemberInstance(InnerToggleState toggleState)
			{
				if (toggleState != null && toggleState.Toggle)
				{
					this.m_ignoreGroupPageBreaks--;
				}
			}

			internal void RegisterKeepWith()
			{
				this.m_keepWith = true;
			}

			internal void UnRegisterKeepWith(KeepWithGroup keepWith, double[] tablixCellHeights, ref bool advanceRow)
			{
				this.m_keepWith = false;
				if (keepWith == KeepWithGroup.After)
				{
					if (this.m_pageContext.CancelPage)
					{
						advanceRow = false;
					}
					else
					{
						advanceRow = true;
					}
				}
				else
				{
					this.LeaveToggleMember(tablixCellHeights, ref advanceRow);
				}
			}

			internal void CreateCorner(TablixCorner corner)
			{
				if (this.m_rplWriter == null && this.m_interactivity == null)
				{
					return;
				}
				if (!this.HasCornerCells() && this.m_headerColumnRows != 0 && this.m_headerRowCols != 0)
				{
					TablixCornerRowCollection rowCollection = corner.RowCollection;
					TablixCornerRow tablixCornerRow = null;
					for (int i = 0; i < rowCollection.Count; i++)
					{
						tablixCornerRow = ((ReportElementCollectionBase<TablixCornerRow>)rowCollection)[i];
						for (int j = 0; j < tablixCornerRow.Count; j++)
						{
							this.AddCornerCell(((ReportElementCollectionBase<TablixCornerCell>)tablixCornerRow)[j], i, j);
						}
					}
				}
			}

			private void AddCornerCell(TablixCornerCell tablixCornerCell, int rowIndex, int colIndex)
			{
				if (tablixCornerCell != null && tablixCornerCell.CellContents != null && !this.m_pageContext.CancelPage)
				{
					CellContents cellContents = tablixCornerCell.CellContents;
					ReportItem reportItem = cellContents.ReportItem;
					PageItem pageItem = null;
					if (reportItem != null)
					{
						PageContext pageContext = new PageContext(this.m_pageContext, PageContext.PageContextFlags.FullOnPage, PageContext.IgnorePBReasonFlag.TablixParent);
						pageItem = PageItem.Create(reportItem, pageContext, true, false);
						double num = 0.0;
						bool useForPageHFEval = false;
						pageItem.CalculatePage(this.m_rplWriter, null, pageContext, null, null, 0.0, ref num, this.m_interactivity);
						if (!pageContext.CancelPage)
						{
							if (this.m_rplWriter != null)
							{
								this.UpdateSizes(colIndex, cellContents.ColSpan, pageItem.ItemRenderSizes.Width, false, ref this.m_colWidths);
								this.UpdateSizes(rowIndex, cellContents.RowSpan, pageItem.ItemRenderSizes.Height, false, ref this.m_colHeaderHeights);
								this.CreateCornerCell(pageItem, cellContents.RowSpan, tablixCornerCell.CellContents.ColSpan, rowIndex, colIndex);
								useForPageHFEval = pageContext.EvaluatePageHeaderFooter;
							}
							RegisterItem.RegisterPageItem(pageItem, pageContext, useForPageHFEval, this.m_interactivity);
						}
					}
					else if (this.m_rplWriter != null)
					{
						this.CreateCornerCell(null, cellContents.RowSpan, cellContents.ColSpan, rowIndex, colIndex);
					}
				}
			}

			internal abstract bool HasCornerCells();

			internal abstract void CreateCornerCell(PageItem topItem, int rowSpan, int colSpan, int rowIndex, int colIndex);

			internal PageMemberCell AddRowMember(TablixMember rowMember, int rowIndex, int colIndex, int rowSpan, int colSpan, byte memberState, int defTreeLevel, LevelInfo childLevelInfo)
			{
				if (this.m_rplWriter == null && this.m_interactivity == null)
				{
					return null;
				}
				if (this.m_pageContext.CancelPage)
				{
					return null;
				}
				return this.AddRowMemberContent(rowMember, rowIndex, colIndex, rowSpan, colSpan, memberState, defTreeLevel, childLevelInfo, 0.0);
			}

			private PageMemberCell AddRowMemberContent(TablixMember rowMember, int rowIndex, int colIndex, int rowSpan, int colSpan, byte memberState, int defTreeLevel, LevelInfo childLevelInfo, double updateWidth)
			{
				if (this.m_interactivity != null)
				{
					this.m_interactivity.RegisterGroupLabel(rowMember.Group, this.m_pageContext);
				}
				if (rowMember.TablixHeader == null)
				{
					if (this.m_rplWriter != null)
					{
						this.UpdateRowHeight(rowIndex, rowSpan, 0.0, rowMember.FixedData, false);
						return this.CreateMemberCell(null, rowSpan, 0, rowMember, memberState, defTreeLevel, TablixRegion.RowHeader);
					}
					return null;
				}
				this.m_sharedLayoutRow = 0;
				ReportItem reportItem = rowMember.TablixHeader.CellContents.ReportItem;
				if (reportItem != null)
				{
					double num = 0.0;
					bool useForPageHFEval = false;
					PageContext pageContext = new PageContext(this.m_pageContext, PageContext.PageContextFlags.FullOnPage, PageContext.IgnorePBReasonFlag.TablixParent);
					PageItem pageItem = PageItem.Create(reportItem, pageContext, true, false);
					double num2 = rowMember.TablixHeader.Size.ToMillimeters() - updateWidth;
					if (childLevelInfo != null)
					{
						num2 += childLevelInfo.SizeForParent;
						if (childLevelInfo.SourceSize > 0.0)
						{
							pageItem.ItemPageSizes.Height = childLevelInfo.SourceSize;
						}
					}
					if (pageItem is TextBox)
					{
						pageItem.ItemPageSizes.Width = num2;
					}
					pageItem.CalculatePage(this.m_rplWriter, null, pageContext, null, null, 0.0, ref num, this.m_interactivity);
					if (pageContext.CancelPage)
					{
						return null;
					}
					if (this.m_rplWriter != null)
					{
						this.UpdateRowHeight(rowIndex, rowSpan, pageItem.ItemRenderSizes.Height, rowMember.FixedData, true);
						num2 = Math.Max(num2, pageItem.ItemRenderSizes.Width);
						if (childLevelInfo == null)
						{
							this.UpdateSizes(colIndex, colSpan, num2, false, ref this.m_colWidths);
						}
						else
						{
							this.UpdateSizes(colIndex, colSpan + childLevelInfo.SpanForParent, num2, false, ref this.m_colWidths);
						}
						useForPageHFEval = pageContext.EvaluatePageHeaderFooter;
					}
					RegisterItem.RegisterPageItem(pageItem, pageContext, useForPageHFEval, this.m_interactivity);
					if (this.m_rplWriter != null)
					{
						if (childLevelInfo == null)
						{
							return this.CreateMemberCell(pageItem, rowSpan, colSpan, rowMember, memberState, defTreeLevel, TablixRegion.RowHeader);
						}
						return this.CreateMemberCell(pageItem, rowSpan, colSpan + childLevelInfo.SpanForParent, rowMember, memberState, defTreeLevel, TablixRegion.RowHeader);
					}
				}
				else if (this.m_rplWriter != null)
				{
					this.UpdateRowHeight(rowIndex, rowSpan, 0.0, rowMember.FixedData, true);
					double num3 = rowMember.TablixHeader.Size.ToMillimeters() - updateWidth;
					if (childLevelInfo == null)
					{
						this.UpdateSizes(colIndex, colSpan, num3, false, ref this.m_colWidths);
						return this.CreateMemberCell(null, rowSpan, colSpan, rowMember, memberState, defTreeLevel, TablixRegion.RowHeader);
					}
					num3 += childLevelInfo.SizeForParent;
					this.UpdateSizes(colIndex, colSpan + childLevelInfo.SpanForParent, num3, false, ref this.m_colWidths);
					return this.CreateMemberCell(null, rowSpan, colSpan + childLevelInfo.SpanForParent, rowMember, memberState, defTreeLevel, TablixRegion.RowHeader);
				}
				return null;
			}

			internal PageMemberCell AddTotalRowMember(TablixMember rowMember, int rowIndex, int colIndex, int rowSpan, int colSpan, byte memberState, int defTreeLevel, LevelInfo parentLevelInfo, LevelInfo childLevelInfo)
			{
				RSTrace.RenderingTracer.Assert(parentLevelInfo != null, "The parent LevelInfo is null.");
				if (this.m_rplWriter == null && this.m_interactivity == null)
				{
					return null;
				}
				if (this.m_pageContext.CancelPage)
				{
					return null;
				}
				double updateWidth = 0.0;
				if (parentLevelInfo.SpanForParent > 0)
				{
					colIndex += parentLevelInfo.SpanForParent;
					colSpan -= parentLevelInfo.SpanForParent;
					updateWidth = parentLevelInfo.SizeForParent;
					if (colSpan == 0)
					{
						if (this.m_rplWriter != null)
						{
							return this.CreateMemberCell(null, rowSpan, 0, rowMember, memberState, defTreeLevel, TablixRegion.RowHeader);
						}
						return null;
					}
				}
				return this.AddRowMemberContent(rowMember, rowIndex, colIndex, rowSpan, colSpan, memberState, defTreeLevel, childLevelInfo, updateWidth);
			}

			internal abstract PageMemberCell CreateMemberCell(PageItem topItem, int rowSpan, int colSpan, TablixMember tablixMember, byte state, int defTreeLevel, TablixRegion region);

			internal PageMemberCell AddColMember(TablixMember colMember, int rowIndex, int colIndex, int rowSpan, int colSpan, byte state, int defTreeLevel, LevelInfo childLevelInfo)
			{
				RSTrace.RenderingTracer.Assert(childLevelInfo != null, "The child LevelInfo is null.");
				if (this.m_rplWriter == null && this.m_interactivity == null)
				{
					goto IL_002f;
				}
				if (this.m_columnHeadersCreated)
				{
					goto IL_002f;
				}
				if (this.m_pageContext.CancelPage)
				{
					return null;
				}
				if (this.m_interactivity != null)
				{
					this.m_interactivity.RegisterGroupLabel(colMember.Group, this.m_pageContext);
				}
				return this.AddColumnMemberContent(colMember, rowIndex, colIndex, rowSpan, colSpan, state, defTreeLevel, childLevelInfo, 0.0);
				IL_002f:
				return null;
			}

			private PageMemberCell AddColumnMemberContent(TablixMember colMember, int rowIndex, int colIndex, int rowSpan, int colSpan, byte state, int defTreeLevel, LevelInfo childLevelInfo, double updateHeight)
			{
				if (colMember.TablixHeader == null)
				{
					if (this.m_rplWriter != null)
					{
						this.UpdateColumnMemberFixedData(colMember, colIndex, colSpan, ref this.m_colWidths);
						return this.CreateMemberCell(null, 0, colSpan, colMember, state, defTreeLevel, TablixRegion.ColumnHeader);
					}
					return null;
				}
				if (this.m_headerColumnRows > 0)
				{
					ReportItem reportItem = colMember.TablixHeader.CellContents.ReportItem;
					if (reportItem != null)
					{
						double num = 0.0;
						bool useForPageHFEval = false;
						PageContext pageContext = new PageContext(this.m_pageContext, PageContext.PageContextFlags.FullOnPage, PageContext.IgnorePBReasonFlag.TablixParent);
						PageItem pageItem = PageItem.Create(reportItem, pageContext, true, false);
						if (childLevelInfo.SourceSize > 0.0)
						{
							pageItem.ItemPageSizes.Width = childLevelInfo.SourceSize;
						}
						pageItem.CalculatePage(this.m_rplWriter, null, pageContext, null, null, 0.0, ref num, this.m_interactivity);
						if (pageContext.CancelPage)
						{
							return null;
						}
						if (this.m_rplWriter != null)
						{
							this.UpdateSizes(colIndex, colSpan, pageItem.ItemRenderSizes.Width, true, ref this.m_colWidths);
							double val = colMember.TablixHeader.Size.ToMillimeters() + childLevelInfo.SizeForParent - updateHeight;
							val = Math.Max(val, pageItem.ItemRenderSizes.Height);
							this.UpdateSizes(rowIndex, rowSpan + childLevelInfo.SpanForParent, val, false, ref this.m_colHeaderHeights);
							this.UpdateColumnMemberFixedData(colMember, colIndex, colSpan, ref this.m_colWidths);
							useForPageHFEval = pageContext.EvaluatePageHeaderFooter;
						}
						RegisterItem.RegisterPageItem(pageItem, pageContext, useForPageHFEval, this.m_interactivity);
						if (this.m_rplWriter != null)
						{
							return this.CreateMemberCell(pageItem, rowSpan + childLevelInfo.SpanForParent, colSpan, colMember, state, defTreeLevel, TablixRegion.ColumnHeader);
						}
					}
					else if (this.m_rplWriter != null)
					{
						this.UpdateSizes(colIndex, colSpan, 0.0, true, ref this.m_colWidths);
						double size = colMember.TablixHeader.Size.ToMillimeters() + childLevelInfo.SizeForParent - updateHeight;
						this.UpdateSizes(rowIndex, rowSpan + childLevelInfo.SpanForParent, size, false, ref this.m_colHeaderHeights);
						this.UpdateColumnMemberFixedData(colMember, colIndex, colSpan, ref this.m_colWidths);
						return this.CreateMemberCell(null, rowSpan + childLevelInfo.SpanForParent, colSpan, colMember, state, defTreeLevel, TablixRegion.ColumnHeader);
					}
				}
				else if (this.m_rplWriter != null)
				{
					this.UpdateSizes(colIndex, colSpan, 0.0, true, ref this.m_colWidths);
					this.UpdateColumnMemberFixedData(colMember, colIndex, colSpan, ref this.m_colWidths);
					return this.CreateMemberCell(null, 0, colSpan, colMember, state, defTreeLevel, TablixRegion.ColumnHeader);
				}
				return null;
			}

			internal PageMemberCell AddTotalColMember(TablixMember colMember, int rowIndex, int colIndex, int rowSpan, int colSpan, byte state, int defTreeLevel, LevelInfo parentLevelInfo, LevelInfo childLevelInfo)
			{
				RSTrace.RenderingTracer.Assert(parentLevelInfo != null, "The parent LevelInfo is null.");
				RSTrace.RenderingTracer.Assert(childLevelInfo != null, "The child LevelInfo is null.");
				if (this.m_rplWriter == null && this.m_interactivity == null)
				{
					goto IL_0046;
				}
				if (this.m_columnHeadersCreated)
				{
					goto IL_0046;
				}
				if (this.m_pageContext.CancelPage)
				{
					return null;
				}
				double updateHeight = 0.0;
				if (parentLevelInfo.SpanForParent > 0)
				{
					rowIndex += parentLevelInfo.SpanForParent;
					rowSpan -= parentLevelInfo.SpanForParent;
					updateHeight = parentLevelInfo.SizeForParent;
					if (rowSpan == 0)
					{
						if (this.m_rplWriter != null)
						{
							return this.CreateMemberCell(null, 0, colSpan, colMember, state, defTreeLevel, TablixRegion.ColumnHeader);
						}
						return null;
					}
				}
				return this.AddColumnMemberContent(colMember, rowIndex, colIndex, rowSpan, colSpan, state, defTreeLevel, childLevelInfo, updateHeight);
				IL_0046:
				return null;
			}

			internal void AddDetailEmptyCell(int colIndex, double cellColDefWidth, double cellCellDefHeight)
			{
				if (cellCellDefHeight > this.m_maxDetailRowHeight)
				{
					this.m_maxDetailRowHeight = cellCellDefHeight;
				}
				if (this.m_rplWriter != null)
				{
					this.UpdateLastDetailCellWidth();
					this.CreateDetailCell(null);
					this.m_lastDetailCellWidth = cellColDefWidth;
					this.m_lastDetailCellColIndex = colIndex;
					if (cellCellDefHeight > this.m_maxDetailRowHeightRender)
					{
						this.m_maxDetailRowHeightRender = cellCellDefHeight;
					}
				}
				this.m_lastDetailDefCellWidth = cellColDefWidth;
			}

			internal PageItem AddDetailCellFromState(TablixCell cellDef, bool ignoreCellPageBreaks)
			{
				ReportItem source = null;
				if (cellDef.CellContents != null)
				{
					source = cellDef.CellContents.ReportItem;
				}
				PageContext pageContext = this.m_pageContext;
				pageContext = ((!ignoreCellPageBreaks) ? ((this.m_ignoreGroupPageBreaks <= 0) ? new PageContext(this.m_pageContext, PageContext.PageContextFlags.StretchPage, PageContext.IgnorePBReasonFlag.TablixParent) : new PageContext(this.m_pageContext, PageContext.PageContextFlags.IgnorePageBreak | PageContext.PageContextFlags.StretchPage, PageContext.IgnorePBReasonFlag.Toggled)) : new PageContext(this.m_pageContext, PageContext.PageContextFlags.FullOnPage, PageContext.IgnorePBReasonFlag.TablixParent));
				PageItem pageItem = PageItem.Create(source, pageContext, true, false);
				pageItem.UpdateItem(this.m_partialItemHelper);
				return pageItem;
			}

			internal ItemOffset AddDetailCell(TablixCell cellDef, int colIndex, double cellColDefWidth, double cellCellDefHeight, bool ignoreCellPageBreaks, bool collect, out bool partialItem)
			{
				ReportItem reportItem = null;
				if (cellDef.CellContents != null)
				{
					reportItem = cellDef.CellContents.ReportItem;
				}
				if (reportItem != null)
				{
					PageContext pageContext = this.m_pageContext;
					pageContext = ((!ignoreCellPageBreaks) ? ((this.m_ignoreGroupPageBreaks <= 0) ? new PageContext(this.m_pageContext, PageContext.PageContextFlags.StretchPage, PageContext.IgnorePBReasonFlag.TablixParent) : new PageContext(this.m_pageContext, PageContext.PageContextFlags.IgnorePageBreak | PageContext.PageContextFlags.StretchPage, PageContext.IgnorePBReasonFlag.Toggled)) : new PageContext(this.m_pageContext, PageContext.PageContextFlags.FullOnPage, PageContext.IgnorePBReasonFlag.TablixParent));
					PageItem pageItem = PageItem.Create(reportItem, pageContext, true, false);
					bool flag = false;
					if (this.m_colMemberIndexCell >= 0)
					{
						AspNetCore.ReportingServices.OnDemandReportRendering.TextBox textBox = reportItem as AspNetCore.ReportingServices.OnDemandReportRendering.TextBox;
						if (textBox != null)
						{
							pageItem.ItemPageSizes.Width = cellColDefWidth;
							if (pageContext.MeasureItems && (textBox.CanGrow || textBox.CanShrink))
							{
								flag = true;
							}
						}
					}
					if (flag)
					{
						this.m_textBoxDelayCalc = (pageItem as TextBox);
						this.m_textBoxDelayCalc.CalcSizeState = TextBox.CalcSize.Delay;
						double num = 0.0;
						pageItem.CalculatePage(this.m_rplWriter, this.m_partialItemHelper, pageContext, null, null, this.m_cellsTopInPage, ref num, this.m_interactivity);
						if (num > this.m_maxDetailRowHeight)
						{
							this.m_maxDetailRowHeight = num;
						}
						if (!pageItem.StaticItem)
						{
							this.m_staticDetailRow = false;
						}
						if (collect)
						{
							RegisterItem.RegisterPageItem(pageItem, pageContext, pageContext.EvaluatePageHeaderFooter, this.m_interactivity);
						}
						if (this.m_rplWriter != null)
						{
							this.UpdateLastDetailCellWidth();
							this.CreateDetailCell(pageItem);
							this.m_lastDetailCellWidth = cellColDefWidth;
							this.m_lastDetailCellColIndex = colIndex;
						}
						if (this.m_textBoxDelayCalc.CalcSizeState == TextBox.CalcSize.Done)
						{
							this.m_textBoxDelayCalc = null;
						}
						partialItem = false;
					}
					else
					{
						partialItem = this.CalculateDetailCell(pageItem, colIndex, collect, pageContext);
					}
					this.m_lastDetailDefCellWidth = cellColDefWidth;
					return pageItem;
				}
				partialItem = false;
				this.AddDetailEmptyCell(colIndex, cellColDefWidth, cellCellDefHeight);
				return new EmptyCell();
			}

			internal bool CalculateDetailCell(PageItem topItem, int colIndex, bool collect)
			{
				return this.CalculateDetailCell(topItem, colIndex, collect, this.m_pageContext);
			}

			private bool CalculateDetailCell(PageItem topItem, int colIndex, bool collect, PageContext pageContext)
			{
				double num = 0.0;
				topItem.CalculatePage(this.m_rplWriter, this.m_partialItemHelper, pageContext, null, null, this.m_cellsTopInPage, ref num, this.m_interactivity);
				if (pageContext.CancelPage)
				{
					return true;
				}
				if (topItem.ItemState == State.TopNextPage)
				{
					this.m_staticDetailRow = false;
					this.AddDetailEmptyCell(colIndex, topItem.ItemPageSizes.Width, 0.0);
				}
				else
				{
					if (num > this.m_maxDetailRowHeight)
					{
						this.m_maxDetailRowHeight = num;
					}
					if (!topItem.StaticItem)
					{
						this.m_staticDetailRow = false;
					}
					if (collect)
					{
						RegisterItem.RegisterPageItem(topItem, pageContext, pageContext.EvaluatePageHeaderFooter, this.m_interactivity);
					}
					if (this.m_rplWriter != null)
					{
						this.UpdateLastDetailCellWidth();
						this.CreateDetailCell(topItem);
						this.m_lastDetailCellWidth = topItem.ItemRenderSizes.Width;
						this.m_lastDetailCellColIndex = colIndex;
						if (topItem.ItemRenderSizes.Height > this.m_maxDetailRowHeightRender)
						{
							this.m_maxDetailRowHeightRender = topItem.ItemRenderSizes.Height;
						}
					}
				}
				if (topItem.ItemState != State.OnPage && topItem.ItemState != State.OnPageHidden)
				{
					if (topItem.ItemState == State.OnPagePBEnd)
					{
						this.m_propagatedPageBreak = true;
						return false;
					}
					this.m_partialRow = true;
					return true;
				}
				return false;
			}

			internal abstract void CreateDetailCell(PageItem topItem);

			internal abstract void UpdateDetailCell(double cellColDefWidth);

			internal abstract void UpdateLastDetailCellWidth();

			internal bool AdvanceRow(double[] tablixCellHeights, List<int> tablixCreateState, int level)
			{
				this.NextRow(tablixCellHeights);
				if (this.m_partialRow)
				{
					if (level >= tablixCreateState.Count)
					{
						tablixCreateState.Add(0);
					}
					else
					{
						tablixCreateState[level] = 0;
					}
					return false;
				}
				if (level < tablixCreateState.Count)
				{
					tablixCreateState.RemoveAt(level);
				}
				if (this.m_pageContext.CancelPage)
				{
					return false;
				}
				if (this.m_pageContext.FullOnPage)
				{
					return true;
				}
				if (!this.m_keepWith && this.m_ignoreGroupPageBreaks <= 0)
				{
					int num = this.m_rowMemberIndexCell + 1;
					if (num >= tablixCellHeights.Length)
					{
						num = 0;
					}
					RoundedDouble roundedDouble = new RoundedDouble(this.m_cellsTopInPage + tablixCellHeights[num]);
					this.m_pageContext.CheckPageSize(roundedDouble);
					if (roundedDouble <= this.m_pageContext.PageHeight)
					{
						return true;
					}
					if (!this.m_detailsOnPage)
					{
						return true;
					}
					return false;
				}
				return true;
			}

			internal double NextRow(double[] tablixCellHeights)
			{
				double num = 0.0;
				if (this.m_ignoreHeight == 0)
				{
					num = ((!this.m_repeatWith && this.m_ignoreGroupPageBreaks <= 0) ? this.m_maxDetailRowHeight : tablixCellHeights[this.m_rowMemberIndexCell]);
					this.m_cellsTopInPage += num;
				}
				this.m_maxDetailRowHeight = 0.0;
				this.m_maxDetailRowHeightRender = 0.0;
				this.m_colMemberIndexCell = -1;
				return num;
			}

			private void UpdateRowHeight(int start, int span, double height, bool fixedData, bool resetState)
			{
				double num = 0.0;
				RowMemberInfo rowMemberInfo = null;
				for (int i = start; i < start + span; i++)
				{
					using (this.m_rowHeights.GetAndPin(i, out rowMemberInfo))
					{
						num += rowMemberInfo.Height;
						if (resetState)
						{
							rowMemberInfo.RowState = 0;
						}
						rowMemberInfo.Fixed = fixedData;
					}
				}
				using (this.m_rowHeights.GetAndPin(start + span - 1, out rowMemberInfo))
				{
					rowMemberInfo.Height += Math.Max(0.0, height - num);
				}
			}

			private void UpdateColumnMemberFixedData(TablixMember colMember, int start, int span, ref ScalableList<SizeInfo> sizeInfoList)
			{
				if (colMember.FixedData)
				{
					if (sizeInfoList == null)
					{
						sizeInfoList = new ScalableList<SizeInfo>(0, this.Cache);
					}
					for (int i = start; i < start + span; i++)
					{
						if (i >= sizeInfoList.Count)
						{
							while (sizeInfoList.Count < i)
							{
								sizeInfoList.Add(null);
							}
							sizeInfoList.Add(new SizeInfo(true));
						}
						else
						{
							SizeInfo sizeInfo = null;
							using (sizeInfoList.GetAndPin(i, out sizeInfo))
							{
								if (sizeInfo == null)
								{
									sizeInfoList[i] = new SizeInfo(true);
								}
								else
								{
									sizeInfo.Fixed = true;
								}
							}
						}
					}
				}
			}

			protected void UpdateSizes(int start, int span, double size, bool split, ref List<SizeInfo> sizeInfoList)
			{
				if (sizeInfoList == null)
				{
					sizeInfoList = new List<SizeInfo>();
				}
				if (split)
				{
					int num = 0;
					double num2 = 0.0;
					for (int i = start; i < start + span; i++)
					{
						if (i >= sizeInfoList.Count || sizeInfoList[i] == null || sizeInfoList[i].Empty)
						{
							num++;
						}
						else
						{
							num2 += sizeInfoList[i].Value;
						}
					}
					if (num2 < size)
					{
						if (num == 0)
						{
							sizeInfoList[start + span - 1].Value += size - num2;
						}
						else
						{
							num2 = (size - num2) / (double)num;
							for (int j = start; j < start + span; j++)
							{
								if (j >= sizeInfoList.Count)
								{
									while (sizeInfoList.Count < j)
									{
										sizeInfoList.Add(null);
									}
									sizeInfoList.Add(new SizeInfo(num2));
								}
								else if (sizeInfoList[j] == null)
								{
									sizeInfoList[j] = new SizeInfo(num2);
								}
								else if (sizeInfoList[j].Empty)
								{
									sizeInfoList[j].Value = num2;
								}
							}
						}
					}
				}
				else
				{
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
							sizeInfoList[start].Value = Math.Max(sizeInfoList[start].Value, size);
						}
					}
					else
					{
						if (sizeInfoList[start] == null)
						{
							sizeInfoList[start] = new SizeInfo(false);
						}
						sizeInfoList[start].AddSpanSize(span, size);
					}
				}
			}

			protected void UpdateSizes(int start, int span, double size, bool split, ref ScalableList<SizeInfo> sizeInfoList)
			{
				if (sizeInfoList == null)
				{
					sizeInfoList = new ScalableList<SizeInfo>(0, this.Cache);
				}
				SizeInfo sizeInfo = null;
				if (split)
				{
					int num = 0;
					double num2 = 0.0;
					for (int i = start; i < start + span; i++)
					{
						if (i >= sizeInfoList.Count)
						{
							num++;
						}
						else
						{
							sizeInfo = sizeInfoList[i];
							if (sizeInfo == null || sizeInfo.Empty)
							{
								num++;
							}
							else
							{
								num2 += sizeInfo.Value;
							}
						}
					}
					if (num2 < size)
					{
						if (num == 0)
						{
							using (sizeInfoList.GetAndPin(start + span - 1, out sizeInfo))
							{
								sizeInfo.Value += size - num2;
							}
						}
						else
						{
							num2 = (size - num2) / (double)num;
							for (int j = start; j < start + span; j++)
							{
								if (j >= sizeInfoList.Count)
								{
									while (sizeInfoList.Count < j)
									{
										sizeInfoList.Add(null);
									}
									sizeInfoList.Add(new SizeInfo(num2));
								}
								else
								{
									using (sizeInfoList.GetAndPin(j, out sizeInfo))
									{
										if (sizeInfo == null)
										{
											sizeInfoList[j] = new SizeInfo(num2);
										}
										else if (sizeInfo.Empty)
										{
											sizeInfo.Value = num2;
										}
									}
								}
							}
						}
					}
				}
				else
				{
					while (sizeInfoList.Count <= start + span - 1)
					{
						sizeInfoList.Add(null);
					}
					using (sizeInfoList.GetAndPin(start, out sizeInfo))
					{
						if (span == 1)
						{
							if (sizeInfo == null)
							{
								sizeInfoList[start] = new SizeInfo(size);
							}
							else
							{
								sizeInfo.Value = Math.Max(sizeInfo.Value, size);
							}
						}
						else
						{
							if (sizeInfo == null)
							{
								sizeInfo = new SizeInfo(false);
								sizeInfoList[start] = sizeInfo;
							}
							sizeInfo.AddSpanSize(span, size);
						}
					}
				}
			}

			protected void ResolveSizes(List<SizeInfo> sizeInfoList)
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
										if (sizeInfoList[j] == null || sizeInfoList[j].Empty)
										{
											num3++;
										}
										else
										{
											num2 += sizeInfoList[j].Value;
										}
									}
									if (num2 < num)
									{
										if (num3 == 0)
										{
											sizeInfoList[i + num4 - 1].Value += num - num2;
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
												else if (sizeInfoList[k].Empty)
												{
													sizeInfoList[k].Value = num2;
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

			protected void ResolveSizes(ScalableList<SizeInfo> sizeInfoList)
			{
				if (sizeInfoList != null)
				{
					List<ColumnSpanInfo> list = new List<ColumnSpanInfo>();
					SizeInfo sizeInfo = null;
					Hashtable hashtable = null;
					for (int i = 0; i < sizeInfoList.Count; i++)
					{
						int num = 2;
						using (sizeInfoList.GetAndPin(i, out sizeInfo))
						{
							if (sizeInfo != null)
							{
								hashtable = sizeInfo.SpanSize;
								if (hashtable != null)
								{
									while (hashtable.Count > 0)
									{
										if (hashtable[num] != null)
										{
											double spanSize = (double)hashtable[num];
											list.Add(new ColumnSpanInfo(i, num, spanSize));
											hashtable.Remove(num);
										}
										num++;
									}
									sizeInfo.SpanSize = null;
								}
							}
						}
					}
					while (list.Count > 0)
					{
						int index = 0;
						ColumnSpanInfo columnSpanInfo = list[index];
						int num2 = columnSpanInfo.CalculateEmptyColumnns(sizeInfoList);
						for (int j = 1; j < list.Count; j++)
						{
							if (num2 == 0)
							{
								break;
							}
							int num3 = list[j].CalculateEmptyColumnns(sizeInfoList);
							if (num3 < num2)
							{
								index = j;
								columnSpanInfo = list[index];
								num2 = num3;
							}
						}
						double num4 = 0.0;
						for (int k = columnSpanInfo.Start; k < columnSpanInfo.Start + columnSpanInfo.Span; k++)
						{
							sizeInfo = sizeInfoList[k];
							if (sizeInfo != null && !sizeInfo.Empty)
							{
								num4 += sizeInfo.Value;
							}
						}
						if (num4 < columnSpanInfo.SpanSize)
						{
							if (num2 == 0)
							{
								for (int num5 = columnSpanInfo.Start + columnSpanInfo.Span - 1; num5 > columnSpanInfo.Start; num5--)
								{
									sizeInfo = sizeInfoList[num5];
									if (sizeInfo == null)
									{
										break;
									}
									if (!sizeInfo.ZeroSized)
									{
										break;
									}
								}
								using (sizeInfoList.GetAndPin(columnSpanInfo.Start + columnSpanInfo.Span - 1, out sizeInfo))
								{
									sizeInfo.Value += columnSpanInfo.SpanSize - num4;
								}
							}
							else
							{
								num4 = (columnSpanInfo.SpanSize - num4) / (double)num2;
								for (int l = columnSpanInfo.Start; l < columnSpanInfo.Start + columnSpanInfo.Span; l++)
								{
									using (sizeInfoList.GetAndPin(l, out sizeInfo))
									{
										if (sizeInfo == null)
										{
											sizeInfoList[l] = new SizeInfo(num4);
										}
										else if (sizeInfo.Empty)
										{
											sizeInfo.Value = num4;
										}
									}
								}
							}
						}
						list.RemoveAt(index);
					}
				}
			}

			internal void DelayedCalculation()
			{
				if (this.m_textBoxDelayCalc != null)
				{
					if (!this.m_pageContext.CancelPage)
					{
						PageContext pageContext = new PageContext(this.m_pageContext, PageContext.PageContextFlags.FullOnPage, PageContext.IgnorePBReasonFlag.TablixParent);
						this.m_textBoxDelayCalc.CalcSizeState = TextBox.CalcSize.None;
						this.m_textBoxDelayCalc.ItemPageSizes.Width = this.m_lastDetailDefCellWidth;
						this.m_textBoxDelayCalc.ItemRenderSizes.Width = this.m_lastDetailDefCellWidth;
						this.m_textBoxDelayCalc.MeasureTextBox(pageContext, null, false);
						this.m_textBoxDelayCalc.DelayWriteContent(this.m_rplWriter, this.m_pageContext);
						if (this.m_textBoxDelayCalc.ItemRenderSizes.Height > this.m_maxDetailRowHeightRender)
						{
							this.m_maxDetailRowHeightRender = this.m_textBoxDelayCalc.ItemRenderSizes.Height;
						}
					}
					this.m_textBoxDelayCalc = null;
				}
			}

			internal abstract void WriteDetailRow(int rowIndex, double[] bodyRowHeights, bool ignoreCellPageBreaks);

			internal abstract void WriteTablixMeasurements(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix, int rowMembersDepth, int colMembersDepth, ref double tablixWidth, ref double tablixHeight);

			protected abstract void OpenHeaderRow(bool omittedRow, int headerStart);

			protected abstract void OpenDetailRow(bool newRow);

			protected abstract void WriteCornerCells(int targetRow, ref int targetCol, int colIndex);

			protected abstract void WriteDetailOffsetRows();

			protected virtual void CloseRow()
			{
			}

			protected void WriteTablixContent(int startColForRowMembers, int rowMembersDepth)
			{
				if (!this.m_pageContext.CancelPage)
				{
					this.WriteCornerAndColumnMembers();
					if (!this.m_pageContext.CancelPage)
					{
						int headerColumnRows = this.m_headerColumnRows;
						if (this.m_isLTR)
						{
							bool flag = true;
							if (this.m_rowHeaders != null)
							{
								this.WriteRowMembersLTR(null, this.m_rowHeaders, ref headerColumnRows, this.m_colsBeforeRowHeaders, ref flag);
							}
							else
							{
								this.WriteDetailOffsetRows();
							}
						}
						else if (this.m_rowHeaders != null)
						{
							int[] state = new int[rowMembersDepth];
							int num = startColForRowMembers;
							this.WriteRowMembersRTL(this.m_rowHeaders, ref headerColumnRows, ref num, state, 0, startColForRowMembers);
						}
						else
						{
							this.WriteDetailOffsetRows();
						}
					}
				}
			}

			protected void WriteRowMembersLTR(PageMemberCell parentMember, ScalableList<PageMemberCell> rowHeaders, ref int rowIndex, int colIndex, ref bool newRow)
			{
				if (rowHeaders == null)
				{
					this.OpenDetailRow(newRow);
					this.CloseRow();
					rowIndex++;
					newRow = true;
					if (parentMember != null)
					{
						for (int i = 1; i < parentMember.RowSpan; i++)
						{
							this.OpenDetailRow(newRow);
							this.CloseRow();
							rowIndex++;
						}
					}
				}
				else
				{
					PageMemberCell pageMemberCell = null;
					for (int j = 0; j < rowHeaders.Count; j++)
					{
						pageMemberCell = rowHeaders[j];
						if (pageMemberCell.ColSpan > 0 || pageMemberCell.NeedWrite)
						{
							if (newRow)
							{
								this.OpenHeaderRow(false, -1);
								newRow = false;
							}
							pageMemberCell.WriteItemToStream(TablixRegion.RowHeader, this.m_rplWriter, rowIndex, colIndex);
						}
						this.WriteRowMembersLTR(pageMemberCell, pageMemberCell.Children, ref rowIndex, colIndex + pageMemberCell.ColSpan, ref newRow);
						rowHeaders[j] = null;
					}
					rowHeaders.Clear();
					rowHeaders = null;
				}
			}

			protected void WriteRowMembersRTL(ScalableList<PageMemberCell> rowHeaders, ref int rowIndex, ref int colIndex, int[] state, int stateIndex, int startColIndex)
			{
				if (rowHeaders == null)
				{
					this.OpenDetailRow(true);
					colIndex = startColIndex;
				}
				else
				{
					PageMemberCell pageMemberCell = null;
					bool flag = true;
					int num = 0;
					for (int i = 0; i < rowHeaders.Count; i++)
					{
						pageMemberCell = rowHeaders[i];
						if (pageMemberCell == null)
						{
							if (num == 10)
							{
								rowHeaders.RemoveRange(0, 10);
								i -= 10;
								num = 0;
							}
							num++;
						}
						else
						{
							flag = true;
							for (int j = state[stateIndex]; j < pageMemberCell.RowSpan; j++)
							{
								if (pageMemberCell.ColSpan == 0)
								{
									this.WriteRowMembersRTL(pageMemberCell.Children, ref rowIndex, ref colIndex, state, stateIndex + 1, startColIndex);
									flag = pageMemberCell.NeedWrite;
								}
								else
								{
									this.WriteRowMembersRTL(pageMemberCell.Children, ref rowIndex, ref colIndex, state, stateIndex + pageMemberCell.ColSpan, startColIndex);
								}
								if (j == 0 && flag)
								{
									pageMemberCell.WriteItemToStream(TablixRegion.RowHeader, this.m_rplWriter, rowIndex, colIndex);
									colIndex += pageMemberCell.ColSpan;
								}
								state[stateIndex]++;
								if (stateIndex == 0)
								{
									this.CloseRow();
									rowIndex++;
								}
								else if (state[stateIndex] < pageMemberCell.RowSpan)
								{
									return;
								}
							}
							state[stateIndex] = 0;
							rowHeaders[i] = null;
							num++;
							if (rowHeaders.Count > i + 1 && stateIndex > 0)
							{
								return;
							}
						}
					}
					rowHeaders.Clear();
					rowHeaders = null;
				}
			}

			protected void WriteCornerAndColumnMembers()
			{
				int i = 0;
				int num = 0;
				if (this.m_columnHeaders == null)
				{
					if (this.m_headerRowCols != 0 && this.m_headerColumnRows != 0)
					{
						for (; i < this.m_headerColumnRows; i++)
						{
							num = 0;
							this.OpenHeaderRow(false, 0);
							this.WriteCornerCells(i, ref num, 0);
							this.CloseRow();
						}
					}
				}
				else
				{
					int num2 = 0;
					int num3 = 0;
					int num4 = 0;
					if (this.m_colsBeforeRowHeaders > 0)
					{
						while (num4 < this.m_columnHeaders.Count)
						{
							num3 += this.m_columnHeaders[num4].ColSpan;
							num4++;
							if (num3 >= this.m_colsBeforeRowHeaders)
							{
								break;
							}
						}
					}
					bool flag = this.m_omittedOuterColumnHeaders;
					for (i = 0; i < this.m_headerColumnRows; i++)
					{
						num2 = 0;
						num = 0;
						if (flag)
						{
							this.WriteOmittedColHeadersRows(num4, i);
							flag = false;
						}
						this.OpenHeaderRow(false, 0);
						if (this.m_isLTR)
						{
							this.WriteColMembers(this.m_columnHeaders, i, 0, ref num2, 0, num4 - 1, ref flag);
						}
						else
						{
							this.WriteColMembers(this.m_columnHeaders, i, 0, ref num2, num4, this.m_columnHeaders.Count - 1, ref flag);
						}
						this.WriteCornerCells(i, ref num, num2);
						num2 += this.m_headerRowCols;
						if (this.m_isLTR)
						{
							this.WriteColMembers(this.m_columnHeaders, i, 0, ref num2, num4, this.m_columnHeaders.Count - 1, ref flag);
						}
						else
						{
							this.WriteColMembers(this.m_columnHeaders, i, 0, ref num2, 0, num4 - 1, ref flag);
						}
						this.CloseRow();
					}
					if (flag)
					{
						this.WriteOmittedColHeadersRows(num4, i);
					}
					this.m_columnHeaders.Clear();
					this.m_columnHeaders = null;
				}
			}

			private void WriteColMembers(ScalableList<PageMemberCell> colHeaders, int targetRow, int rowIndex, ref int colIndex, int start, int end, ref bool writeOmittedHeaders)
			{
				if (colHeaders != null)
				{
					PageMemberCell pageMemberCell = null;
					if (targetRow == rowIndex)
					{
						for (int i = start; i <= end; i++)
						{
							pageMemberCell = ((!this.m_isLTR) ? colHeaders[end - i + start] : colHeaders[i]);
							if (pageMemberCell.RowSpan > 0)
							{
								if (pageMemberCell.HasOmittedChildren && pageMemberCell.RowSpan == 1)
								{
									writeOmittedHeaders = true;
								}
								pageMemberCell.WriteItemToStream(TablixRegion.ColumnHeader, this.m_rplWriter, rowIndex, colIndex);
								colIndex += pageMemberCell.ColSpan;
							}
							else if (pageMemberCell.Children != null)
							{
								this.WriteColMembers(pageMemberCell.Children, targetRow, rowIndex, ref colIndex, 0, pageMemberCell.Children.Count - 1, ref writeOmittedHeaders);
							}
							else
							{
								colIndex += pageMemberCell.ColSpan;
							}
						}
					}
					else
					{
						for (int j = start; j <= end; j++)
						{
							pageMemberCell = ((!this.m_isLTR) ? colHeaders[end - j + start] : colHeaders[j]);
							if (targetRow < rowIndex + pageMemberCell.RowSpan)
							{
								colIndex += pageMemberCell.ColSpan;
								if (targetRow == rowIndex + pageMemberCell.RowSpan - 1 && pageMemberCell.RowSpan > 0 && pageMemberCell.HasOmittedChildren)
								{
									writeOmittedHeaders = true;
								}
							}
							else if (pageMemberCell.Children != null)
							{
								this.WriteColMembers(pageMemberCell.Children, targetRow, rowIndex + pageMemberCell.RowSpan, ref colIndex, 0, pageMemberCell.Children.Count - 1, ref writeOmittedHeaders);
							}
							else
							{
								colIndex += pageMemberCell.ColSpan;
							}
						}
					}
				}
			}

			private void WriteOmittedColHeadersRows(int outerGroupsBRHs, int rowIndex)
			{
				int num = 0;
				bool flag = true;
				int num2 = 0;
				bool flag2 = true;
				while (flag)
				{
					num = 0;
					flag = false;
					flag2 = true;
					if (this.m_isLTR)
					{
						this.WriteOmittedColMembers(this.m_columnHeaders, rowIndex, 0, num2, ref num, 0, outerGroupsBRHs - 1, ref flag, ref flag2);
					}
					else
					{
						this.WriteOmittedColMembers(this.m_columnHeaders, rowIndex, 0, num2, ref num, outerGroupsBRHs, this.m_columnHeaders.Count - 1, ref flag, ref flag2);
					}
					num += this.m_headerRowCols;
					if (this.m_isLTR)
					{
						this.WriteOmittedColMembers(this.m_columnHeaders, rowIndex, 0, num2, ref num, outerGroupsBRHs, this.m_columnHeaders.Count - 1, ref flag, ref flag2);
					}
					else
					{
						this.WriteOmittedColMembers(this.m_columnHeaders, rowIndex, 0, num2, ref num, 0, outerGroupsBRHs - 1, ref flag, ref flag2);
					}
					if (!flag2)
					{
						this.CloseRow();
					}
					num2++;
				}
			}

			private void WriteOmittedColMembers(ScalableList<PageMemberCell> colHeaders, int targetRow, int rowIndex, int targetLevel, ref int colIndex, int start, int end, ref bool writeOmittedHeaders, ref bool openRow)
			{
				if (colHeaders != null)
				{
					PageMemberCell pageMemberCell = null;
					if (targetRow == rowIndex)
					{
						this.WriteOmittedColMembersLevel(colHeaders, targetRow, targetLevel, 0, ref colIndex, start, end, ref writeOmittedHeaders, ref openRow);
					}
					else
					{
						for (int i = start; i <= end; i++)
						{
							pageMemberCell = ((!this.m_isLTR) ? colHeaders[end - i + start] : colHeaders[i]);
							if (targetRow < rowIndex + pageMemberCell.RowSpan)
							{
								colIndex += pageMemberCell.ColSpan;
							}
							else if (pageMemberCell.Children != null)
							{
								this.WriteOmittedColMembers(pageMemberCell.Children, targetRow, rowIndex + pageMemberCell.RowSpan, targetLevel, ref colIndex, 0, pageMemberCell.Children.Count - 1, ref writeOmittedHeaders, ref openRow);
							}
							else
							{
								colIndex += pageMemberCell.ColSpan;
							}
						}
					}
				}
			}

			private void WriteOmittedColMembersLevel(ScalableList<PageMemberCell> colHeaders, int targetRow, int targetLevel, int level, ref int colIndex, int start, int end, ref bool writeOmittedHeaders, ref bool openRow)
			{
				if (colHeaders != null)
				{
					PageMemberCell pageMemberCell = null;
					if (targetLevel == level)
					{
						for (int i = start; i <= end; i++)
						{
							pageMemberCell = ((!this.m_isLTR) ? colHeaders[end - i + start] : colHeaders[i]);
							if (pageMemberCell.RowSpan == 0)
							{
								if (pageMemberCell.NeedWrite)
								{
									if (openRow)
									{
										this.OpenHeaderRow(true, -1);
										openRow = false;
									}
									pageMemberCell.WriteItemToStream(TablixRegion.ColumnHeader, this.m_rplWriter, targetRow, colIndex);
								}
								if (pageMemberCell.HasOmittedChildren)
								{
									writeOmittedHeaders = true;
								}
							}
							colIndex += pageMemberCell.ColSpan;
						}
					}
					else
					{
						bool flag = false;
						for (int j = start; j <= end; j++)
						{
							pageMemberCell = ((!this.m_isLTR) ? colHeaders[end - j + start] : colHeaders[j]);
							flag = false;
							if (pageMemberCell.RowSpan == 0 && pageMemberCell.HasOmittedChildren && pageMemberCell.Children != null)
							{
								this.WriteOmittedColMembersLevel(pageMemberCell.Children, targetRow, targetLevel, level + 1, ref colIndex, 0, pageMemberCell.Children.Count - 1, ref flag, ref openRow);
								if (flag)
								{
									writeOmittedHeaders = true;
								}
								else
								{
									pageMemberCell.HasOmittedChildren = false;
								}
							}
							else
							{
								colIndex += pageMemberCell.ColSpan;
							}
						}
					}
				}
			}

			private void RegisterToggleMemberPath(Tablix tablix, TablixMember rowMember)
			{
				if (tablix != null && rowMember != null && this.m_currentToggleMemberPath == null)
				{
					this.m_currentToggleMemberPath = tablix.BuildTablixMemberPath(rowMember);
				}
			}

			private void ResetToggleMemberPath()
			{
				this.m_currentToggleMemberPath = null;
			}

			private void TracePageGrownOnImplicitKeepTogetherMember()
			{
				if (this.m_currentToggleMemberPath != null)
				{
					RenderingDiagnostics.Trace(RenderingArea.KeepTogether, TraceLevel.Verbose, "PR-DIAG [Page {0}] Member '{1}' kept together - Implicit - Page grown", this.PageContext.PageNumber, this.m_currentToggleMemberPath);
				}
			}
		}

		internal sealed class StreamContext : TablixContext
		{
			private ScalableList<DetailCell> m_cellDetailRow;

			private Queue<long> m_detailRowOffsets;

			private CornerCell[,] m_cornerCells;

			private List<RPLTablixMemberDef> m_rowMemberDefList;

			private Hashtable m_rowMemberDefIndexes;

			private List<RPLTablixMemberDef> m_colMemberDefList;

			private Hashtable m_colMemberDefIndexes;

			internal StreamContext(RPLWriter rplWriter, PageItemHelper partialItemHelper, bool noRows, bool isLTR, PageContext pageContext, double cellsTopInPage, int headerRowCols, int headerColumnRows, int[] defDetailRowsCapacity, Interactivity interactivity)
				: base(rplWriter, partialItemHelper, noRows, isLTR, pageContext, cellsTopInPage, headerRowCols, headerColumnRows, defDetailRowsCapacity, interactivity)
			{
			}

			internal override void CreateDetailCell(PageItem topItem)
			{
				DetailCell detailCell = null;
				detailCell = ((topItem != null) ? new DetailCell(topItem.Offset, topItem.RplItemState) : new DetailCell(0L, 0));
				if (this.m_cellDetailRow == null)
				{
					this.m_cellDetailRow = new ScalableList<DetailCell>(0, base.Cache);
				}
				this.m_cellDetailRow.Add(detailCell);
			}

			internal override void UpdateDetailCell(double cellColDefWidth)
			{
				if (this.m_cellDetailRow != null)
				{
					DetailCell detailCell = null;
					using (this.m_cellDetailRow.GetAndPin(this.m_cellDetailRow.Count - 1, out detailCell))
					{
						detailCell.ColSpan++;
					}
					base.m_lastDetailDefCellWidth += cellColDefWidth;
				}
			}

			internal override void UpdateLastDetailCellWidth()
			{
				if (this.m_cellDetailRow != null)
				{
					DetailCell detailCell = this.m_cellDetailRow[this.m_cellDetailRow.Count - 1];
					double size = Math.Max(base.m_lastDetailCellWidth, base.m_lastDetailDefCellWidth);
					base.UpdateSizes(base.m_lastDetailCellColIndex, detailCell.ColSpan, size, false, ref base.m_colWidths);
					base.m_lastDetailCellWidth = 0.0;
					base.m_lastDetailDefCellWidth = 0.0;
					base.m_lastDetailCellColIndex = 0;
				}
			}

			internal override bool HasCornerCells()
			{
				return null != this.m_cornerCells;
			}

			internal override void CreateCornerCell(PageItem topItem, int rowSpan, int colSpan, int rowIndex, int colIndex)
			{
				CornerCell cornerCell = new CornerCell(topItem.Offset, topItem.RplItemState, rowSpan, colSpan);
				if (this.m_cornerCells == null)
				{
					this.m_cornerCells = new CornerCell[base.m_headerColumnRows, base.m_headerRowCols];
				}
				this.m_cornerCells[rowIndex, colIndex] = cornerCell;
			}

			internal override PageMemberCell CreateMemberCell(PageItem topItem, int rowSpan, int colSpan, TablixMember tablixMember, byte state, int defTreeLevel, TablixRegion region)
			{
				MemberCell memberCell = null;
				memberCell = ((topItem != null) ? new MemberCell(topItem.Offset, topItem.RplItemState, rowSpan, colSpan, tablixMember) : new MemberCell(0L, 0, rowSpan, colSpan, tablixMember));
				if (region == TablixRegion.RowHeader)
				{
					memberCell.TablixMemberDefIndex = this.AddTablixMemberDef(ref this.m_rowMemberDefIndexes, ref this.m_rowMemberDefList, tablixMember, state, defTreeLevel, region);
				}
				else
				{
					memberCell.TablixMemberDefIndex = this.AddTablixMemberDef(ref this.m_colMemberDefIndexes, ref this.m_colMemberDefList, tablixMember, state, defTreeLevel, region);
				}
				return new StreamMemberCell(memberCell, state);
			}

			private int AddTablixMemberDef(ref Hashtable memberDefIndexes, ref List<RPLTablixMemberDef> memberDefList, TablixMember tablixMember, byte state, int defTreeLevel, TablixRegion region)
			{
				object obj = null;
				if (memberDefIndexes == null)
				{
					memberDefIndexes = new Hashtable();
					memberDefList = new List<RPLTablixMemberDef>();
				}
				else
				{
					obj = memberDefIndexes[tablixMember.DefinitionPath];
				}
				if (obj == null)
				{
					obj = memberDefList.Count;
					memberDefIndexes.Add(tablixMember.DefinitionPath, obj);
					byte b = 0;
					if (region == TablixRegion.ColumnHeader)
					{
						b = 1;
					}
					if ((state & 1) > 0)
					{
						b = 4;
					}
					if (tablixMember.IsStatic)
					{
						b = (byte)(b | 2);
					}
					RPLTablixMemberDef item = new RPLTablixMemberDef(tablixMember.DefinitionPath, tablixMember.MemberCellIndex, b, defTreeLevel);
					memberDefList.Add(item);
				}
				return (int)obj;
			}

			internal override void WriteDetailRow(int rowIndex, double[] tablixCellHeights, bool ignoreCellPageBreaks)
			{
				base.m_detailsOnPage = true;
				base.m_pageBreakAtEnd = false;
				if (base.m_rplWriter != null)
				{
					base.DelayedCalculation();
					if (ignoreCellPageBreaks)
					{
						base.m_maxDetailRowHeightRender = Math.Max(base.m_maxDetailRowHeightRender, tablixCellHeights[base.m_rowMemberIndexCell]);
					}
					this.UpdateLastDetailCellWidth();
					RowMemberInfo rowMemberInfo = new RowMemberInfo(base.m_maxDetailRowHeightRender);
					if (this.m_cellDetailRow != null)
					{
						int num = 0;
						DetailCell detailCell = null;
						int num2 = this.m_cellDetailRow.Count - 1;
						if (base.m_sharedLayoutRow > 0)
						{
							if (base.m_staticDetailRow && num2 == 0)
							{
								rowMemberInfo.RowState = base.m_sharedLayoutRow;
								if (base.m_sharedLayoutRow == 2)
								{
									base.m_sharedLayoutRow = 4;
								}
							}
							else
							{
								base.m_sharedLayoutRow = 2;
							}
						}
						base.m_detailRowCellsCapacity[base.m_rowMemberIndexCell] = num2 + 1;
						BinaryWriter binaryWriter = base.m_rplWriter.BinaryWriter;
						if (this.m_detailRowOffsets == null)
						{
							this.m_detailRowOffsets = new Queue<long>();
						}
						this.m_detailRowOffsets.Enqueue(binaryWriter.BaseStream.Position);
						binaryWriter.Write((byte)18);
						binaryWriter.Write(rowIndex + base.m_headerColumnRows);
						for (int i = 0; i <= num2; i++)
						{
							if (base.m_isLTR)
							{
								detailCell = this.m_cellDetailRow[i];
								if (num == base.m_colsBeforeRowHeaders)
								{
									num += base.m_headerRowCols;
								}
							}
							else
							{
								detailCell = this.m_cellDetailRow[num2 - i];
								if (num == base.m_colWidths.Count - base.m_colsBeforeRowHeaders - base.m_headerRowCols)
								{
									num += base.m_headerRowCols;
								}
							}
							detailCell.WriteItemToStream(base.m_rplWriter, num);
							num += detailCell.ColSpan;
						}
						binaryWriter.Write((byte)255);
						this.m_cellDetailRow.Clear();
						this.m_cellDetailRow = null;
					}
					base.m_staticDetailRow = true;
					if (base.m_rowHeights == null)
					{
						base.m_rowHeights = new ScalableList<RowMemberInfo>(0, base.Cache);
					}
					base.m_rowHeights.Add(rowMemberInfo);
				}
			}

			internal override void WriteTablixMeasurements(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix, int rowMembersDepth, int colMembersDepth, ref double tablixWidth, ref double tablixHeight)
			{
				if (base.m_rplWriter != null)
				{
					if (base.m_colWidths == null && base.m_colHeaderHeights == null && base.m_rowHeights == null)
					{
						return;
					}
					int num = 0;
					base.ResolveSizes(base.m_colWidths);
					BinaryWriter binaryWriter = base.m_rplWriter.BinaryWriter;
					binaryWriter.Write((byte)0);
					binaryWriter.Write(base.m_headerColumnRows);
					binaryWriter.Write((byte)1);
					binaryWriter.Write(base.m_headerRowCols);
					if (base.m_colWidths != null)
					{
						binaryWriter.Write((byte)2);
						if (base.m_isLTR)
						{
							num = base.m_colsBeforeRowHeaders;
							binaryWriter.Write(num);
						}
						else
						{
							num = base.m_colWidths.Count - base.m_colsBeforeRowHeaders - base.m_headerRowCols;
							binaryWriter.Write(num);
							binaryWriter.Write((byte)3);
							binaryWriter.Write((byte)1);
						}
						binaryWriter.Write((byte)4);
						binaryWriter.Write(base.m_colWidths.Count);
						if (base.m_isLTR)
						{
							SizeInfo sizeInfo = null;
							if (base.m_colsBeforeRowHeaders > 0)
							{
								for (int i = base.m_headerRowCols; i < base.m_headerRowCols + base.m_colsBeforeRowHeaders; i++)
								{
									sizeInfo = base.m_colWidths[i];
									binaryWriter.Write((float)sizeInfo.Value);
									binaryWriter.Write(sizeInfo.Fixed);
									tablixWidth += sizeInfo.Value;
									base.m_colWidths[i] = null;
								}
								base.m_colWidths.RemoveRange(base.m_headerRowCols, base.m_colsBeforeRowHeaders);
							}
							for (int j = 0; j < base.m_headerRowCols; j++)
							{
								sizeInfo = base.m_colWidths[j];
								binaryWriter.Write((float)sizeInfo.Value);
								binaryWriter.Write(tablix.FixedRowHeaders);
								tablixWidth += sizeInfo.Value;
								base.m_colWidths[j] = null;
							}
							base.m_colWidths.RemoveRange(0, base.m_headerRowCols);
							for (int k = 0; k < base.m_colWidths.Count; k++)
							{
								sizeInfo = base.m_colWidths[k];
								binaryWriter.Write((float)sizeInfo.Value);
								binaryWriter.Write(sizeInfo.Fixed);
								tablixWidth += sizeInfo.Value;
								base.m_colWidths[k] = null;
							}
						}
						else
						{
							SizeInfo sizeInfo2 = null;
							int num2 = base.m_headerRowCols + base.m_colsBeforeRowHeaders;
							for (int num3 = base.m_colWidths.Count - 1; num3 >= num2; num3--)
							{
								sizeInfo2 = base.m_colWidths[num3];
								binaryWriter.Write((float)sizeInfo2.Value);
								binaryWriter.Write(sizeInfo2.Fixed);
								tablixWidth += sizeInfo2.Value;
								base.m_colWidths[num3] = null;
							}
							base.m_colWidths.RemoveRange(num2, base.m_colWidths.Count - num2);
							for (int num4 = base.m_headerRowCols - 1; num4 >= 0; num4--)
							{
								sizeInfo2 = base.m_colWidths[num4];
								binaryWriter.Write((float)sizeInfo2.Value);
								binaryWriter.Write(tablix.FixedRowHeaders);
								tablixWidth += sizeInfo2.Value;
								base.m_colWidths[num4] = null;
							}
							base.m_colWidths.RemoveRange(0, base.m_headerRowCols);
							for (int num5 = base.m_colWidths.Count - 1; num5 >= 0; num5--)
							{
								sizeInfo2 = base.m_colWidths[num5];
								binaryWriter.Write((float)sizeInfo2.Value);
								binaryWriter.Write(sizeInfo2.Fixed);
								tablixWidth += sizeInfo2.Value;
								base.m_colWidths[num5] = null;
							}
						}
						base.m_colWidths.Clear();
						base.m_colWidths = null;
					}
					if (base.m_colHeaderHeights != null || base.m_rowHeights != null)
					{
						binaryWriter.Write((byte)5);
						if (base.m_colHeaderHeights != null)
						{
							if (base.m_rowHeights != null)
							{
								binaryWriter.Write(base.m_rowHeights.Count + base.m_colHeaderHeights.Count);
							}
							else
							{
								binaryWriter.Write(base.m_colHeaderHeights.Count);
							}
							base.ResolveSizes(base.m_colHeaderHeights);
							for (int l = 0; l < base.m_colHeaderHeights.Count; l++)
							{
								binaryWriter.Write((float)base.m_colHeaderHeights[l].Value);
								binaryWriter.Write(tablix.FixedColumnHeaders);
								tablixHeight += base.m_colHeaderHeights[l].Value;
								base.m_colHeaderHeights[l] = null;
							}
							base.m_colHeaderHeights = null;
						}
						else
						{
							binaryWriter.Write(base.m_rowHeights.Count);
						}
						if (base.m_rowHeights != null)
						{
							RowMemberInfo rowMemberInfo = null;
							for (int m = 0; m < base.m_rowHeights.Count; m++)
							{
								rowMemberInfo = base.m_rowHeights[m];
								binaryWriter.Write((float)rowMemberInfo.Height);
								binaryWriter.Write(rowMemberInfo.RowState);
								tablixHeight += rowMemberInfo.Height;
								base.m_rowHeights[m] = null;
							}
							base.m_rowHeights.Clear();
							base.m_rowHeights = null;
						}
					}
					this.m_rowMemberDefIndexes = null;
					this.m_colMemberDefIndexes = null;
					this.WriteTablixMemberDefList(binaryWriter, ref this.m_rowMemberDefList, TablixRegion.RowHeader);
					this.WriteTablixMemberDefList(binaryWriter, ref this.m_colMemberDefList, TablixRegion.ColumnHeader);
					base.WriteTablixContent(num, rowMembersDepth);
				}
			}

			private void WriteTablixMemberDefList(BinaryWriter spbifWriter, ref List<RPLTablixMemberDef> membersDefList, TablixRegion region)
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
						membersDefList[i] = null;
					}
					membersDefList = null;
				}
			}

			protected override void WriteDetailOffsetRows()
			{
				if (this.m_detailRowOffsets != null)
				{
					BinaryWriter binaryWriter = base.m_rplWriter.BinaryWriter;
					while (this.m_detailRowOffsets.Count > 0)
					{
						binaryWriter.Write((byte)8);
						binaryWriter.Write((byte)9);
						binaryWriter.Write(this.m_detailRowOffsets.Dequeue());
						binaryWriter.Write((byte)255);
					}
					this.m_detailRowOffsets = null;
				}
			}

			protected override void OpenDetailRow(bool newRow)
			{
				if (newRow)
				{
					base.m_rplWriter.BinaryWriter.Write((byte)8);
				}
				if (this.m_detailRowOffsets != null)
				{
					base.m_rplWriter.BinaryWriter.Write((byte)9);
					base.m_rplWriter.BinaryWriter.Write(this.m_detailRowOffsets.Dequeue());
				}
			}

			protected override void CloseRow()
			{
				base.m_rplWriter.BinaryWriter.Write((byte)255);
			}

			protected override void OpenHeaderRow(bool omittedRow, int headerStart)
			{
				base.m_rplWriter.BinaryWriter.Write((byte)8);
			}

			protected override void WriteCornerCells(int targetRow, ref int targetCol, int colIndex)
			{
				while (targetCol < base.m_headerRowCols)
				{
					CornerCell cornerCell = null;
					cornerCell = ((!base.m_isLTR) ? this.m_cornerCells[targetRow, base.m_headerRowCols - targetCol - 1] : this.m_cornerCells[targetRow, targetCol]);
					if (cornerCell != null)
					{
						if (base.m_isLTR)
						{
							cornerCell.WriteItemToStream(base.m_rplWriter, targetRow, colIndex + targetCol);
						}
						else
						{
							cornerCell.WriteItemToStream(base.m_rplWriter, targetRow, colIndex + targetCol - cornerCell.ColSpan + 1);
						}
						targetCol += cornerCell.ColSpan;
					}
					else
					{
						targetCol++;
					}
				}
			}
		}

		internal class RPLContext : TablixContext
		{
			private RPLTablix m_rplTablix;

			protected List<RPLTablixCell> m_cellDetailRow;

			private Queue<RPLTablixRow> m_detailRowRPLTablixCells;

			protected RPLTablixCornerCell[,] m_cornerCells;

			private Hashtable m_rplTablixMembersDef;

			internal RPLContext(RPLWriter rplWriter, PageItemHelper partialItemHelper, bool noRows, bool isLTR, PageContext pageContext, double cellsTopInPage, int headerRowCols, int headerColumnRows, int[] defDetailRowsCapacity, Interactivity interactivity, RPLTablix rplTablix)
				: base(rplWriter, partialItemHelper, noRows, isLTR, pageContext, cellsTopInPage, headerRowCols, headerColumnRows, defDetailRowsCapacity, interactivity)
			{
				this.m_rplTablix = rplTablix;
			}

			internal override void CreateDetailCell(PageItem topItem)
			{
				if (!base.PageContext.CancelPage)
				{
					RPLTablixCell rPLTablixCell = null;
					rPLTablixCell = ((topItem != null) ? new RPLTablixCell(topItem.RPLElement, topItem.RplItemState) : new RPLTablixCell());
					if (this.m_cellDetailRow == null)
					{
						this.m_cellDetailRow = new List<RPLTablixCell>(base.m_detailRowCellsCapacity[base.m_rowMemberIndexCell]);
					}
					this.m_cellDetailRow.Add(rPLTablixCell);
				}
			}

			internal override void UpdateDetailCell(double cellColDefWidth)
			{
				if (this.m_cellDetailRow != null)
				{
					RPLTablixCell rPLTablixCell = this.m_cellDetailRow[this.m_cellDetailRow.Count - 1];
					rPLTablixCell.ColSpan++;
					base.m_lastDetailDefCellWidth += cellColDefWidth;
				}
			}

			internal override void UpdateLastDetailCellWidth()
			{
				if (this.m_cellDetailRow != null)
				{
					RPLTablixCell rPLTablixCell = this.m_cellDetailRow[this.m_cellDetailRow.Count - 1];
					double size = Math.Max(base.m_lastDetailCellWidth, base.m_lastDetailDefCellWidth);
					base.UpdateSizes(base.m_lastDetailCellColIndex, rPLTablixCell.ColSpan, size, false, ref base.m_colWidths);
					base.m_lastDetailCellWidth = 0.0;
					base.m_lastDetailDefCellWidth = 0.0;
					base.m_lastDetailCellColIndex = 0;
				}
			}

			internal override bool HasCornerCells()
			{
				return null != this.m_cornerCells;
			}

			internal override void CreateCornerCell(PageItem topItem, int rowSpan, int colSpan, int rowIndex, int colIndex)
			{
				if (!base.PageContext.CancelPage)
				{
					RPLTablixCornerCell rPLTablixCornerCell = new RPLTablixCornerCell(topItem.RPLElement, topItem.RplItemState, rowSpan, colSpan);
					if (this.m_cornerCells == null)
					{
						this.m_cornerCells = new RPLTablixCornerCell[base.m_headerColumnRows, base.m_headerRowCols];
					}
					this.m_cornerCells[rowIndex, colIndex] = rPLTablixCornerCell;
				}
			}

			internal override PageMemberCell CreateMemberCell(PageItem topItem, int rowSpan, int colSpan, TablixMember tablixMember, byte state, int defTreeLevel, TablixRegion region)
			{
				if (base.PageContext.CancelPage)
				{
					return null;
				}
				RPLTablixMemberCell rPLTablixMemberCell = null;
				rPLTablixMemberCell = ((topItem != null) ? new RPLTablixMemberCell(topItem.RPLElement, topItem.RplItemState, rowSpan, colSpan) : new RPLTablixMemberCell(null, 0, rowSpan, colSpan));
				Group group = tablixMember.Group;
				if (group != null)
				{
					GroupInstance instance = group.Instance;
					rPLTablixMemberCell.UniqueName = instance.UniqueName;
					if (group.DocumentMapLabel != null)
					{
						if (group.DocumentMapLabel.IsExpression)
						{
							rPLTablixMemberCell.GroupLabel = instance.DocumentMapLabel;
						}
						else
						{
							rPLTablixMemberCell.GroupLabel = group.DocumentMapLabel.Value;
						}
					}
					rPLTablixMemberCell.RecursiveToggleLevel = -1;
					if (tablixMember.Visibility != null && tablixMember.Visibility.RecursiveToggleReceiver)
					{
						rPLTablixMemberCell.RecursiveToggleLevel = instance.RecursiveLevel;
					}
				}
				RPLTablixMemberDef rPLTablixMemberDef = null;
				if (this.m_rplTablixMembersDef == null)
				{
					this.m_rplTablixMembersDef = new Hashtable();
				}
				else
				{
					rPLTablixMemberDef = (this.m_rplTablixMembersDef[tablixMember.DefinitionPath] as RPLTablixMemberDef);
				}
				if (rPLTablixMemberDef == null)
				{
					byte b = 0;
					if (region == TablixRegion.ColumnHeader)
					{
						b = 1;
					}
					if ((state & 1) > 0)
					{
						b = (byte)(b | 4);
					}
					if (tablixMember.IsStatic)
					{
						b = (byte)(b | 2);
					}
					rPLTablixMemberDef = new RPLTablixMemberDef(tablixMember.DefinitionPath, tablixMember.MemberCellIndex, b, defTreeLevel);
					this.m_rplTablixMembersDef.Add(tablixMember.DefinitionPath, rPLTablixMemberDef);
				}
				rPLTablixMemberCell.TablixMemberDef = rPLTablixMemberDef;
				return new RPLMemberCell(rPLTablixMemberCell, state);
			}

			internal override void WriteDetailRow(int rowIndex, double[] tablixCellHeights, bool ignoreCellPageBreaks)
			{
				base.m_detailsOnPage = true;
				base.m_pageBreakAtEnd = false;
				if (base.m_rplWriter != null)
				{
					if (base.PageContext.CancelPage)
					{
						this.m_cellDetailRow = null;
						this.m_detailRowRPLTablixCells = null;
					}
					else
					{
						base.DelayedCalculation();
						if (ignoreCellPageBreaks)
						{
							base.m_maxDetailRowHeightRender = Math.Max(base.m_maxDetailRowHeightRender, tablixCellHeights[base.m_rowMemberIndexCell]);
						}
						this.UpdateLastDetailCellWidth();
						RowMemberInfo rowMemberInfo = new RowMemberInfo(base.m_maxDetailRowHeightRender);
						if (this.m_cellDetailRow != null)
						{
							int num = 0;
							RPLTablixCell rPLTablixCell = null;
							int num2 = this.m_cellDetailRow.Count - 1;
							if (base.m_sharedLayoutRow > 0)
							{
								if (base.m_staticDetailRow && num2 == 0)
								{
									rowMemberInfo.RowState = base.m_sharedLayoutRow;
									if (base.m_sharedLayoutRow == 2)
									{
										base.m_sharedLayoutRow = 4;
									}
								}
								else
								{
									base.m_sharedLayoutRow = 2;
								}
							}
							base.m_detailRowCellsCapacity[base.m_rowMemberIndexCell] = num2 + 1;
							List<RPLTablixCell> list = null;
							list = ((!base.m_isLTR) ? new List<RPLTablixCell>(num2) : this.m_cellDetailRow);
							if (this.m_detailRowRPLTablixCells == null)
							{
								this.m_detailRowRPLTablixCells = new Queue<RPLTablixRow>();
							}
							this.m_detailRowRPLTablixCells.Enqueue(new RPLTablixRow(list));
							for (int i = 0; i <= num2; i++)
							{
								if (base.m_isLTR)
								{
									rPLTablixCell = this.m_cellDetailRow[i];
									if (num == base.m_colsBeforeRowHeaders)
									{
										num += base.m_headerRowCols;
									}
								}
								else
								{
									rPLTablixCell = this.m_cellDetailRow[num2 - i];
									if (num == base.m_colWidths.Count - base.m_colsBeforeRowHeaders - base.m_headerRowCols)
									{
										num += base.m_headerRowCols;
									}
									list.Add(rPLTablixCell);
								}
								rPLTablixCell.ColIndex = num;
								rPLTablixCell.RowIndex = rowIndex + base.m_headerColumnRows;
								num += rPLTablixCell.ColSpan;
							}
							this.m_cellDetailRow = null;
						}
						base.m_staticDetailRow = true;
						if (base.m_rowHeights == null)
						{
							base.m_rowHeights = new ScalableList<RowMemberInfo>(0, base.Cache);
						}
						base.m_rowHeights.Add(rowMemberInfo);
					}
				}
			}

			internal override void WriteTablixMeasurements(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix, int rowMembersDepth, int colMembersDepth, ref double tablixWidth, ref double tablixHeight)
			{
				if (base.m_rplWriter != null)
				{
					if (base.m_colWidths == null && base.m_colHeaderHeights == null && base.m_rowHeights == null)
					{
						return;
					}
					RPLTablix rplTablix = this.m_rplTablix;
					int num = 0;
					base.ResolveSizes(base.m_colWidths);
					rplTablix.ColumnHeaderRows = base.m_headerColumnRows;
					rplTablix.RowHeaderColumns = base.m_headerRowCols;
					int num2 = 0;
					if (base.m_colWidths != null)
					{
						if (base.m_isLTR)
						{
							num = base.m_colsBeforeRowHeaders;
						}
						else
						{
							num = base.m_colWidths.Count - base.m_colsBeforeRowHeaders - base.m_headerRowCols;
							rplTablix.LayoutDirection = RPLFormat.Directions.RTL;
						}
						rplTablix.ColsBeforeRowHeaders = num;
						rplTablix.ColumnWidths = new float[base.m_colWidths.Count];
						rplTablix.FixedColumns = new bool[base.m_colWidths.Count];
						if (base.m_isLTR)
						{
							SizeInfo sizeInfo = null;
							if (base.m_colsBeforeRowHeaders > 0)
							{
								for (int i = base.m_headerRowCols; i < base.m_headerRowCols + base.m_colsBeforeRowHeaders; i++)
								{
									sizeInfo = base.m_colWidths[i];
									rplTablix.ColumnWidths[num2] = (float)sizeInfo.Value;
									rplTablix.FixedColumns[num2] = sizeInfo.Fixed;
									num2++;
									tablixWidth += sizeInfo.Value;
									base.m_colWidths[i] = null;
								}
								base.m_colWidths.RemoveRange(base.m_headerRowCols, base.m_colsBeforeRowHeaders);
							}
							for (int j = 0; j < base.m_headerRowCols; j++)
							{
								sizeInfo = base.m_colWidths[j];
								rplTablix.ColumnWidths[num2] = (float)sizeInfo.Value;
								rplTablix.FixedColumns[num2] = tablix.FixedRowHeaders;
								num2++;
								tablixWidth += sizeInfo.Value;
								base.m_colWidths[j] = null;
							}
							base.m_colWidths.RemoveRange(0, base.m_headerRowCols);
							for (int k = 0; k < base.m_colWidths.Count; k++)
							{
								sizeInfo = base.m_colWidths[k];
								rplTablix.ColumnWidths[num2] = (float)sizeInfo.Value;
								rplTablix.FixedColumns[num2] = sizeInfo.Fixed;
								num2++;
								tablixWidth += sizeInfo.Value;
								base.m_colWidths[k] = null;
							}
						}
						else
						{
							SizeInfo sizeInfo2 = null;
							int num3 = base.m_headerRowCols + base.m_colsBeforeRowHeaders;
							for (int num4 = base.m_colWidths.Count - 1; num4 >= num3; num4--)
							{
								sizeInfo2 = base.m_colWidths[num4];
								rplTablix.ColumnWidths[num2] = (float)sizeInfo2.Value;
								rplTablix.FixedColumns[num2] = sizeInfo2.Fixed;
								num2++;
								tablixWidth += sizeInfo2.Value;
								base.m_colWidths[num4] = null;
							}
							base.m_colWidths.RemoveRange(num3, base.m_colWidths.Count - num3);
							for (int num5 = base.m_headerRowCols - 1; num5 >= 0; num5--)
							{
								sizeInfo2 = base.m_colWidths[num5];
								rplTablix.ColumnWidths[num2] = (float)sizeInfo2.Value;
								rplTablix.FixedColumns[num2] = tablix.FixedRowHeaders;
								num2++;
								tablixWidth += sizeInfo2.Value;
								base.m_colWidths[num5] = null;
							}
							base.m_colWidths.RemoveRange(0, base.m_headerRowCols);
							for (int num6 = base.m_colWidths.Count - 1; num6 >= 0; num6--)
							{
								sizeInfo2 = base.m_colWidths[num6];
								rplTablix.ColumnWidths[num2] = (float)sizeInfo2.Value;
								rplTablix.FixedColumns[num2] = sizeInfo2.Fixed;
								num2++;
								tablixWidth += sizeInfo2.Value;
								base.m_colWidths[num6] = null;
							}
						}
						base.m_colWidths = null;
					}
					num2 = 0;
					if (base.m_colHeaderHeights != null || base.m_rowHeights != null)
					{
						if (base.m_colHeaderHeights != null)
						{
							if (base.m_rowHeights != null)
							{
								rplTablix.RowHeights = new float[base.m_rowHeights.Count + base.m_colHeaderHeights.Count];
								rplTablix.RowsState = new byte[base.m_rowHeights.Count + base.m_colHeaderHeights.Count];
							}
							else
							{
								rplTablix.RowHeights = new float[base.m_colHeaderHeights.Count];
								rplTablix.RowsState = new byte[base.m_colHeaderHeights.Count];
							}
							base.ResolveSizes(base.m_colHeaderHeights);
							for (int l = 0; l < base.m_colHeaderHeights.Count; l++)
							{
								rplTablix.RowHeights[num2] = (float)base.m_colHeaderHeights[l].Value;
								if (tablix.FixedColumnHeaders)
								{
									rplTablix.RowsState[num2] = 1;
								}
								num2++;
								tablixHeight += base.m_colHeaderHeights[l].Value;
								base.m_colHeaderHeights[l] = null;
							}
							base.m_colHeaderHeights = null;
						}
						else
						{
							rplTablix.RowHeights = new float[base.m_rowHeights.Count];
							rplTablix.RowsState = new byte[base.m_rowHeights.Count];
						}
						if (base.m_rowHeights != null)
						{
							RowMemberInfo rowMemberInfo = null;
							for (int m = 0; m < base.m_rowHeights.Count; m++)
							{
								rowMemberInfo = base.m_rowHeights[m];
								rplTablix.RowHeights[num2] = (float)rowMemberInfo.Height;
								rplTablix.RowsState[num2] = rowMemberInfo.RowState;
								num2++;
								tablixHeight += rowMemberInfo.Height;
								base.m_rowHeights[m] = null;
							}
							base.m_rowHeights.Clear();
							base.m_rowHeights = null;
						}
					}
					this.m_rplTablixMembersDef = null;
					base.WriteTablixContent(num, rowMembersDepth);
				}
			}

			protected override void WriteDetailOffsetRows()
			{
				if (!base.PageContext.CancelPage && this.m_detailRowRPLTablixCells != null)
				{
					while (this.m_detailRowRPLTablixCells.Count > 0)
					{
						this.m_rplTablix.AddRow(this.m_detailRowRPLTablixCells.Dequeue());
					}
					this.m_detailRowRPLTablixCells = null;
				}
				this.m_detailRowRPLTablixCells = null;
			}

			protected override void OpenHeaderRow(bool omittedRow, int headerStart)
			{
				RPLTablixRow rPLTablixRow = null;
				rPLTablixRow = ((!omittedRow) ? ((RPLTablixRow)new RPLTablixFullRow(headerStart, -1)) : ((RPLTablixRow)new RPLTablixOmittedRow()));
				if (!base.PageContext.CancelPage)
				{
					this.m_rplTablix.AddRow(rPLTablixRow);
				}
				base.m_rplWriter.TablixRow = rPLTablixRow;
			}

			protected override void OpenDetailRow(bool newRow)
			{
				if (newRow)
				{
					RPLTablixFullRow rPLTablixFullRow = new RPLTablixFullRow(-1, 0);
					if (!base.PageContext.CancelPage)
					{
						this.m_rplTablix.AddRow(rPLTablixFullRow);
					}
					base.m_rplWriter.TablixRow = rPLTablixFullRow;
				}
				if (this.m_detailRowRPLTablixCells != null)
				{
					RPLTablixRow rPLTablixRow = this.m_detailRowRPLTablixCells.Dequeue();
					base.m_rplWriter.TablixRow.SetBodyStart();
					base.m_rplWriter.TablixRow.AddCells(rPLTablixRow.RowCells);
				}
			}

			protected override void WriteCornerCells(int targetRow, ref int targetCol, int colIndex)
			{
				while (targetCol < base.m_headerRowCols)
				{
					RPLTablixCornerCell rPLTablixCornerCell = null;
					rPLTablixCornerCell = ((!base.m_isLTR) ? this.m_cornerCells[targetRow, base.m_headerRowCols - targetCol - 1] : this.m_cornerCells[targetRow, targetCol]);
					if (rPLTablixCornerCell != null)
					{
						rPLTablixCornerCell.RowIndex = targetRow;
						if (base.m_isLTR)
						{
							rPLTablixCornerCell.ColIndex = colIndex + targetCol;
						}
						else
						{
							rPLTablixCornerCell.ColIndex = colIndex + targetCol - rPLTablixCornerCell.ColSpan + 1;
						}
						base.m_rplWriter.TablixRow.RowCells.Add(rPLTablixCornerCell);
						targetCol += rPLTablixCornerCell.ColSpan;
					}
					else
					{
						targetCol++;
					}
				}
			}
		}

		internal class LevelInfo
		{
			private int m_spanForParent;

			private double m_sizeForParent;

			private ScalableList<PageMemberCell> m_memberCells;

			private bool m_omittedList = true;

			private bool m_omittedMembersCells;

			private int m_ignoredRowsCols;

			private double m_sourceSize;

			private int m_sourceIndex = -1;

			private bool m_hasVisibleStaticPeer;

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

			internal ScalableList<PageMemberCell> MemberCells
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

			internal bool OmittedMembersCells
			{
				get
				{
					return this.m_omittedMembersCells;
				}
				set
				{
					this.m_omittedMembersCells = value;
				}
			}

			internal int IgnoredRowsCols
			{
				get
				{
					return this.m_ignoredRowsCols;
				}
				set
				{
					this.m_ignoredRowsCols = value;
				}
			}

			internal bool OmittedList
			{
				get
				{
					return this.m_omittedList;
				}
			}

			internal double SourceSize
			{
				get
				{
					return this.m_sourceSize;
				}
			}

			internal bool HasVisibleStaticPeer
			{
				get
				{
					return this.m_hasVisibleStaticPeer;
				}
				set
				{
					this.m_hasVisibleStaticPeer = value;
				}
			}

			internal void AddMemberCell(PageMemberCell memberCell, int span, int priority, int sourceIndex, double sourceSize, IScalabilityCache cache)
			{
				if (this.m_memberCells == null)
				{
					this.m_memberCells = new ScalableList<PageMemberCell>(priority, cache);
				}
				this.m_memberCells.Add(memberCell);
				if (span > 0 || memberCell.NeedWrite || memberCell.Children != null)
				{
					this.m_omittedList = false;
				}
				if (sourceIndex > this.m_sourceIndex)
				{
					this.m_sourceSize += sourceSize;
					this.m_sourceIndex = sourceIndex;
				}
			}

			internal void SetDefaults()
			{
				this.m_spanForParent = 0;
				this.m_sizeForParent = 0.0;
				this.m_memberCells = null;
				this.m_omittedList = true;
				this.m_omittedMembersCells = false;
				this.m_ignoredRowsCols = 0;
				this.m_sourceSize = 0.0;
				this.m_sourceIndex = -1;
				this.m_hasVisibleStaticPeer = false;
			}
		}

		internal class EmptyCell : ItemOffset
		{
			public long Offset
			{
				get
				{
					return 0L;
				}
				set
				{
				}
			}
		}

		internal class DetailCell : IStorable, IPersistable
		{
			protected int m_colSpan = 1;

			protected long m_offset;

			protected byte m_cellItemState;

			private static Declaration m_declaration = DetailCell.GetDeclaration();

			internal virtual int RowSpan
			{
				get
				{
					return 1;
				}
				set
				{
				}
			}

			internal virtual int ColSpan
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

			public virtual int Size
			{
				get
				{
					return 13;
				}
			}

			internal DetailCell()
			{
			}

			internal DetailCell(long offset, byte itemState)
			{
				this.m_offset = offset;
				this.m_cellItemState = itemState;
			}

			internal DetailCell(long offset, byte itemState, int colSpan)
			{
				this.m_offset = offset;
				this.m_colSpan = colSpan;
				this.m_cellItemState = itemState;
			}

			public virtual void Serialize(IntermediateFormatWriter writer)
			{
				writer.RegisterDeclaration(DetailCell.m_declaration);
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.ColSpan:
						writer.Write(this.m_colSpan);
						break;
					case MemberName.Offset:
						writer.Write(this.m_offset);
						break;
					case MemberName.State:
						writer.Write(this.m_cellItemState);
						break;
					default:
						RSTrace.RenderingTracer.Assert(false);
						break;
					}
				}
			}

			public virtual void Deserialize(IntermediateFormatReader reader)
			{
				reader.RegisterDeclaration(DetailCell.m_declaration);
				while (reader.NextMember())
				{
					switch (reader.CurrentMember.MemberName)
					{
					case MemberName.ColSpan:
						this.m_colSpan = reader.ReadInt32();
						break;
					case MemberName.Offset:
						this.m_offset = reader.ReadInt64();
						break;
					case MemberName.State:
						this.m_cellItemState = reader.ReadByte();
						break;
					default:
						RSTrace.RenderingTracer.Assert(false);
						break;
					}
				}
			}

			public void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
			{
			}

			public virtual ObjectType GetObjectType()
			{
				return ObjectType.DetailCell;
			}

			internal static Declaration GetDeclaration()
			{
				if (DetailCell.m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.ColSpan, Token.Int32));
					list.Add(new MemberInfo(MemberName.Offset, Token.Int64));
					list.Add(new MemberInfo(MemberName.State, Token.Byte));
					return new Declaration(ObjectType.DetailCell, ObjectType.None, list);
				}
				return DetailCell.m_declaration;
			}

			internal void WriteItemToStream(RPLWriter rplWriter, int colIndex)
			{
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				binaryWriter.Write((byte)13);
				if (this.m_offset > 0)
				{
					binaryWriter.Write((byte)4);
					binaryWriter.Write(this.m_offset);
				}
				if (this.m_cellItemState > 0)
				{
					binaryWriter.Write((byte)13);
					binaryWriter.Write(this.m_cellItemState);
				}
				if (this.m_colSpan != 1)
				{
					binaryWriter.Write((byte)5);
					binaryWriter.Write(this.m_colSpan);
				}
				binaryWriter.Write((byte)8);
				binaryWriter.Write(colIndex);
				binaryWriter.Write((byte)255);
			}
		}

		internal class CornerCell : DetailCell
		{
			protected int m_rowSpan = 1;

			private static Declaration m_declaration = CornerCell.GetDeclaration();

			internal override int RowSpan
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

			public override int Size
			{
				get
				{
					return base.Size + 4;
				}
			}

			internal CornerCell()
			{
			}

			internal CornerCell(long offset, byte itemState, int rowSpan, int colSpan)
				: base(offset, itemState, colSpan)
			{
				this.m_rowSpan = rowSpan;
			}

			internal void WriteItemToStream(RPLWriter rplWriter, int rowIndex, int colIndex)
			{
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				binaryWriter.Write((byte)10);
				if (base.m_offset > 0)
				{
					binaryWriter.Write((byte)4);
					binaryWriter.Write(base.m_offset);
				}
				if (base.m_cellItemState > 0)
				{
					binaryWriter.Write((byte)13);
					binaryWriter.Write(base.m_cellItemState);
				}
				if (base.m_colSpan != 1)
				{
					binaryWriter.Write((byte)5);
					binaryWriter.Write(base.m_colSpan);
				}
				if (this.m_rowSpan != 1)
				{
					binaryWriter.Write((byte)6);
					binaryWriter.Write(this.m_rowSpan);
				}
				binaryWriter.Write((byte)9);
				binaryWriter.Write(rowIndex);
				binaryWriter.Write((byte)8);
				binaryWriter.Write(colIndex);
				binaryWriter.Write((byte)255);
			}

			public override void Serialize(IntermediateFormatWriter writer)
			{
				base.Serialize(writer);
				writer.RegisterDeclaration(CornerCell.m_declaration);
				while (writer.NextMember())
				{
					MemberName memberName = writer.CurrentMember.MemberName;
					if (memberName == MemberName.RowSpan)
					{
						writer.Write(this.m_rowSpan);
					}
					else
					{
						RSTrace.RenderingTracer.Assert(false);
					}
				}
			}

			public override void Deserialize(IntermediateFormatReader reader)
			{
				base.Deserialize(reader);
				reader.RegisterDeclaration(CornerCell.m_declaration);
				while (reader.NextMember())
				{
					MemberName memberName = reader.CurrentMember.MemberName;
					if (memberName == MemberName.RowSpan)
					{
						this.m_rowSpan = reader.ReadInt32();
					}
					else
					{
						RSTrace.RenderingTracer.Assert(false);
					}
				}
			}

			public override ObjectType GetObjectType()
			{
				return ObjectType.CornerCell;
			}

			internal new static Declaration GetDeclaration()
			{
				if (CornerCell.m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.RowSpan, Token.Int32));
					return new Declaration(ObjectType.CornerCell, ObjectType.DetailCell, list);
				}
				return CornerCell.m_declaration;
			}
		}

		internal class MemberCell : CornerCell
		{
			private string m_uniqueName;

			private string m_label;

			private int m_recursiveToggleLevel = -1;

			private int m_tablixMemberDefIndex = -1;

			private static Declaration m_declaration = MemberCell.GetDeclaration();

			internal string GroupLabel
			{
				get
				{
					return this.m_label;
				}
			}

			internal int RecursiveToggleLevel
			{
				get
				{
					return this.m_recursiveToggleLevel;
				}
			}

			internal int TablixMemberDefIndex
			{
				set
				{
					this.m_tablixMemberDefIndex = value;
				}
			}

			public override int Size
			{
				get
				{
					return base.Size + 8 + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_uniqueName) + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_label);
				}
			}

			internal MemberCell()
			{
			}

			internal MemberCell(long offset, byte itemState, int rowSpan, int colSpan, TablixMember tablixMember)
				: base(offset, itemState, rowSpan, colSpan)
			{
				Group group = tablixMember.Group;
				if (group != null)
				{
					GroupInstance instance = group.Instance;
					this.m_uniqueName = instance.UniqueName;
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
					}
					this.m_recursiveToggleLevel = -1;
					if (tablixMember.Visibility != null && tablixMember.Visibility.RecursiveToggleReceiver)
					{
						this.m_recursiveToggleLevel = instance.RecursiveLevel;
					}
				}
			}

			internal void WriteItemToStream(TablixRegion region, RPLWriter rplWriter, int rowIndex, int colIndex, byte state)
			{
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				if (region == TablixRegion.ColumnHeader)
				{
					binaryWriter.Write((byte)11);
				}
				else
				{
					binaryWriter.Write((byte)12);
				}
				if (base.m_offset > 0)
				{
					binaryWriter.Write((byte)4);
					binaryWriter.Write(base.m_offset);
				}
				if (base.m_cellItemState > 0)
				{
					binaryWriter.Write((byte)13);
					binaryWriter.Write(base.m_cellItemState);
				}
				if (base.m_colSpan != 1)
				{
					binaryWriter.Write((byte)5);
					binaryWriter.Write(base.m_colSpan);
				}
				if (base.m_rowSpan != 1)
				{
					binaryWriter.Write((byte)6);
					binaryWriter.Write(base.m_rowSpan);
				}
				if (this.m_tablixMemberDefIndex >= 0)
				{
					binaryWriter.Write((byte)7);
					binaryWriter.Write(this.m_tablixMemberDefIndex);
				}
				binaryWriter.Write((byte)9);
				binaryWriter.Write(rowIndex);
				binaryWriter.Write((byte)8);
				binaryWriter.Write(colIndex);
				if (this.m_uniqueName != null)
				{
					binaryWriter.Write((byte)11);
					binaryWriter.Write(this.m_uniqueName);
				}
				if (this.m_label != null)
				{
					binaryWriter.Write((byte)10);
					binaryWriter.Write(this.m_label);
				}
				binaryWriter.Write((byte)14);
				binaryWriter.Write(this.m_recursiveToggleLevel);
				if (state > 0)
				{
					binaryWriter.Write((byte)12);
					binaryWriter.Write(state);
				}
				binaryWriter.Write((byte)255);
			}

			public override void Serialize(IntermediateFormatWriter writer)
			{
				base.Serialize(writer);
				writer.RegisterDeclaration(MemberCell.m_declaration);
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.UniqueName:
						writer.Write(this.m_uniqueName);
						break;
					case MemberName.Label:
						writer.Write(this.m_label);
						break;
					case MemberName.RecursiveLevel:
						writer.Write(this.m_recursiveToggleLevel);
						break;
					case MemberName.DefIndex:
						writer.Write(this.m_tablixMemberDefIndex);
						break;
					default:
						RSTrace.RenderingTracer.Assert(false);
						break;
					}
				}
			}

			public override void Deserialize(IntermediateFormatReader reader)
			{
				base.Deserialize(reader);
				reader.RegisterDeclaration(MemberCell.m_declaration);
				while (reader.NextMember())
				{
					switch (reader.CurrentMember.MemberName)
					{
					case MemberName.UniqueName:
						this.m_uniqueName = reader.ReadString();
						break;
					case MemberName.Label:
						this.m_label = reader.ReadString();
						break;
					case MemberName.RecursiveLevel:
						this.m_recursiveToggleLevel = reader.ReadInt32();
						break;
					case MemberName.DefIndex:
						this.m_tablixMemberDefIndex = reader.ReadInt32();
						break;
					default:
						RSTrace.RenderingTracer.Assert(false);
						break;
					}
				}
			}

			public override ObjectType GetObjectType()
			{
				return ObjectType.MemberCell;
			}

			internal new static Declaration GetDeclaration()
			{
				if (MemberCell.m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.UniqueName, Token.String));
					list.Add(new MemberInfo(MemberName.Label, Token.String));
					list.Add(new MemberInfo(MemberName.DefIndex, Token.Int32));
					list.Add(new MemberInfo(MemberName.RecursiveLevel, Token.Int32));
					return new Declaration(ObjectType.MemberCell, ObjectType.CornerCell, list);
				}
				return MemberCell.m_declaration;
			}
		}

		internal abstract class PageMemberCell : IStorable, IPersistable
		{
			protected byte m_state;

			private ScalableList<PageMemberCell> m_children;

			private static Declaration m_declaration = PageMemberCell.GetDeclaration();

			internal bool HasOmittedChildren
			{
				get
				{
					return (this.m_state & 8) > 0;
				}
				set
				{
					if (value)
					{
						this.m_state |= 8;
					}
					else
					{
						this.m_state &= 247;
					}
				}
			}

			internal ScalableList<PageMemberCell> Children
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

			internal abstract bool NeedWrite
			{
				get;
			}

			internal abstract int ColSpan
			{
				get;
			}

			internal abstract int RowSpan
			{
				get;
			}

			public virtual int Size
			{
				get
				{
					return 1 + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_children);
				}
			}

			internal PageMemberCell()
			{
			}

			internal PageMemberCell(byte state)
			{
				this.m_state = state;
			}

			internal byte ResolveState()
			{
				byte b = 0;
				if (this.m_state > 1)
				{
					if ((this.m_state & 2) > 0)
					{
						b = 1;
						if ((this.m_state & 4) > 0)
						{
							b = (byte)(b | 2);
						}
					}
					if (this.m_children == null)
					{
						b = (byte)(b | 4);
					}
				}
				return b;
			}

			internal abstract void WriteItemToStream(TablixRegion region, RPLWriter rplWriter, int rowIndex, int colIndex);

			public virtual void Serialize(IntermediateFormatWriter writer)
			{
				writer.RegisterDeclaration(PageMemberCell.m_declaration);
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.State:
						writer.Write(this.m_state);
						break;
					case MemberName.Children:
						writer.Write(this.m_children);
						break;
					default:
						RSTrace.RenderingTracer.Assert(false);
						break;
					}
				}
			}

			public virtual void Deserialize(IntermediateFormatReader reader)
			{
				reader.RegisterDeclaration(PageMemberCell.m_declaration);
				while (reader.NextMember())
				{
					switch (reader.CurrentMember.MemberName)
					{
					case MemberName.State:
						this.m_state = reader.ReadByte();
						break;
					case MemberName.Children:
						this.m_children = reader.ReadRIFObject<ScalableList<PageMemberCell>>();
						break;
					default:
						RSTrace.RenderingTracer.Assert(false);
						break;
					}
				}
			}

			public void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
			{
			}

			public virtual ObjectType GetObjectType()
			{
				return ObjectType.PageMemberCell;
			}

			internal static Declaration GetDeclaration()
			{
				if (PageMemberCell.m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.State, Token.Byte));
					list.Add(new MemberInfo(MemberName.Children, ObjectType.ScalableList, ObjectType.PageMemberCell));
					return new Declaration(ObjectType.PageMemberCell, ObjectType.None, list);
				}
				return PageMemberCell.m_declaration;
			}
		}

		internal class StreamMemberCell : PageMemberCell
		{
			private MemberCell m_memberCell;

			private static Declaration m_declaration = StreamMemberCell.GetDeclaration();

			internal override int ColSpan
			{
				get
				{
					return this.m_memberCell.ColSpan;
				}
			}

			internal override int RowSpan
			{
				get
				{
					return this.m_memberCell.RowSpan;
				}
			}

			internal override bool NeedWrite
			{
				get
				{
					if (base.m_state > 0)
					{
						return true;
					}
					if (this.m_memberCell.GroupLabel != null)
					{
						return true;
					}
					return false;
				}
			}

			public override int Size
			{
				get
				{
					return base.Size + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_memberCell);
				}
			}

			internal StreamMemberCell()
			{
			}

			internal StreamMemberCell(MemberCell cell, byte state)
				: base(state)
			{
				this.m_memberCell = cell;
			}

			internal override void WriteItemToStream(TablixRegion region, RPLWriter rplWriter, int rowIndex, int colIndex)
			{
				byte state = base.ResolveState();
				this.m_memberCell.WriteItemToStream(region, rplWriter, rowIndex, colIndex, state);
			}

			public override void Serialize(IntermediateFormatWriter writer)
			{
				base.Serialize(writer);
				writer.RegisterDeclaration(StreamMemberCell.m_declaration);
				while (writer.NextMember())
				{
					MemberName memberName = writer.CurrentMember.MemberName;
					if (memberName == MemberName.MemberCell)
					{
						writer.Write(this.m_memberCell);
					}
					else
					{
						RSTrace.RenderingTracer.Assert(false);
					}
				}
			}

			public override void Deserialize(IntermediateFormatReader reader)
			{
				base.Deserialize(reader);
				reader.RegisterDeclaration(StreamMemberCell.m_declaration);
				while (reader.NextMember())
				{
					MemberName memberName = reader.CurrentMember.MemberName;
					if (memberName == MemberName.MemberCell)
					{
						this.m_memberCell = (MemberCell)reader.ReadRIFObject();
					}
					else
					{
						RSTrace.RenderingTracer.Assert(false);
					}
				}
			}

			public override ObjectType GetObjectType()
			{
				return ObjectType.StreamMemberCell;
			}

			internal new static Declaration GetDeclaration()
			{
				if (StreamMemberCell.m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.MemberCell, ObjectType.MemberCell));
					return new Declaration(ObjectType.StreamMemberCell, ObjectType.PageMemberCell, list);
				}
				return StreamMemberCell.m_declaration;
			}
		}

		internal class RPLMemberCell : PageMemberCell
		{
			[StaticReference]
			private RPLTablixMemberCell m_memberCell;

			private static Declaration m_declaration = RPLMemberCell.GetDeclaration();

			internal override int ColSpan
			{
				get
				{
					return this.m_memberCell.ColSpan;
				}
			}

			internal override int RowSpan
			{
				get
				{
					return this.m_memberCell.RowSpan;
				}
			}

			internal override bool NeedWrite
			{
				get
				{
					if (base.m_state > 0)
					{
						return true;
					}
					if (this.m_memberCell.GroupLabel != null)
					{
						return true;
					}
					return false;
				}
			}

			public override int Size
			{
				get
				{
					return base.Size + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.ReferenceSize;
				}
			}

			internal RPLMemberCell()
			{
			}

			internal RPLMemberCell(RPLTablixMemberCell cell, byte state)
				: base(state)
			{
				this.m_memberCell = cell;
			}

			internal override void WriteItemToStream(TablixRegion region, RPLWriter rplWriter, int rowIndex, int colIndex)
			{
				this.m_memberCell.RowIndex = rowIndex;
				this.m_memberCell.ColIndex = colIndex;
				this.m_memberCell.State = base.ResolveState();
				if (this.m_memberCell.ColSpan == 0 || this.m_memberCell.RowSpan == 0)
				{
					rplWriter.TablixRow.AddOmittedHeader(this.m_memberCell);
				}
				else
				{
					rplWriter.TablixRow.SetHeaderStart();
					rplWriter.TablixRow.RowCells.Add(this.m_memberCell);
				}
			}

			public override void Serialize(IntermediateFormatWriter writer)
			{
				base.Serialize(writer);
				IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
				writer.RegisterDeclaration(RPLMemberCell.m_declaration);
				while (writer.NextMember())
				{
					MemberName memberName = writer.CurrentMember.MemberName;
					if (memberName == MemberName.MemberCell)
					{
						int value = scalabilityCache.StoreStaticReference(this.m_memberCell);
						writer.Write(value);
					}
					else
					{
						RSTrace.RenderingTracer.Assert(false);
					}
				}
			}

			public override void Deserialize(IntermediateFormatReader reader)
			{
				base.Deserialize(reader);
				IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
				reader.RegisterDeclaration(RPLMemberCell.m_declaration);
				while (reader.NextMember())
				{
					MemberName memberName = reader.CurrentMember.MemberName;
					if (memberName == MemberName.MemberCell)
					{
						int id = reader.ReadInt32();
						this.m_memberCell = (RPLTablixMemberCell)scalabilityCache.FetchStaticReference(id);
					}
					else
					{
						RSTrace.RenderingTracer.Assert(false);
					}
				}
			}

			public override ObjectType GetObjectType()
			{
				return ObjectType.RPLMemberCell;
			}

			internal new static Declaration GetDeclaration()
			{
				if (RPLMemberCell.m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.MemberCell, Token.Int32));
					return new Declaration(ObjectType.RPLMemberCell, ObjectType.PageMemberCell, list);
				}
				return RPLMemberCell.m_declaration;
			}
		}

		internal const byte BorderHeader = 1;

		internal const byte HasToggle = 2;

		internal const byte CollapsedHeader = 4;

		internal const byte HasOmittedChildren = 8;

		private PageItem m_partialPageItem;

		private List<int> m_tablixCreateState;

		private int m_levelForRepeat;

		private bool m_ignoreTotalsOnLastLevel;

		private TablixRowCollection m_bodyRows;

		private double[] m_bodyRowsHeigths;

		private double[] m_bodyColWidths;

		private bool m_ignoreCellPageBreaks;

		private bool m_hasDetailCellsWithColSpan;

		private int m_rowMembersDepth;

		private int m_colMembersDepth;

		private int m_partialMemberLevel;

		private InnerToggleState m_toggleMemberState;

		private Hashtable m_memberIndexByLevel = new Hashtable();

		private string m_outermostKeepWithMemberPath;

		private bool m_repeatOnNewPageRegistered;

		protected override PageBreak PageBreak
		{
			get
			{
				return ((AspNetCore.ReportingServices.OnDemandReportRendering.Tablix)base.m_source).PageBreak;
			}
		}

		protected override string PageName
		{
			get
			{
				return ((TablixInstance)base.m_source.Instance).PageName;
			}
		}

		internal Tablix(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix source, PageContext pageContext, bool createForRepeat)
			: base(source)
		{
			if (pageContext != null)
			{
				if (createForRepeat)
				{
					base.m_itemPageSizes = pageContext.GetSharedFromRepeatItemSizesElement(source, false);
				}
				else
				{
					base.m_itemPageSizes = pageContext.GetSharedItemSizesElement(source, false);
				}
			}
			else
			{
				base.m_itemPageSizes = new ItemSizes(source);
			}
			TablixColumnCollection columnCollection = source.Body.ColumnCollection;
			this.m_bodyColWidths = new double[columnCollection.Count];
			for (int i = 0; i < columnCollection.Count; i++)
			{
				this.m_bodyColWidths[i] = ((ReportElementCollectionBase<TablixColumn>)columnCollection)[i].Width.ToMillimeters();
			}
			this.m_bodyRows = source.Body.RowCollection;
			this.m_bodyRowsHeigths = new double[this.m_bodyRows.Count];
			for (int j = 0; j < this.m_bodyRows.Count; j++)
			{
				this.m_bodyRowsHeigths[j] = ((ReportElementCollectionBase<TablixRow>)this.m_bodyRows)[j].Height.ToMillimeters();
				if (!this.m_hasDetailCellsWithColSpan)
				{
					for (int k = 0; k < columnCollection.Count; k++)
					{
						TablixCell tablixCell = ((ReportElementCollectionBase<TablixCell>)((ReportElementCollectionBase<TablixRow>)this.m_bodyRows)[j])[k];
						if (tablixCell != null && tablixCell.CellContents.ColSpan > 1)
						{
							this.m_hasDetailCellsWithColSpan = true;
							break;
						}
					}
				}
			}
			this.m_ignoreCellPageBreaks = source.Body.IgnoreCellPageBreaks;
			this.m_rowMembersDepth = this.TablixMembersDepthTree(source.RowHierarchy.MemberCollection);
			this.m_colMembersDepth = this.TablixMembersDepthTree(source.ColumnHierarchy.MemberCollection);
			if (!pageContext.AddToggledItems)
			{
				this.m_toggleMemberState = this.GetInnerToggleStateForParent(source.RowHierarchy.MemberCollection);
			}
		}

		internal override void UpdateItem(PageItemHelper itemHelper)
		{
			if (itemHelper != null)
			{
				base.UpdateItem(itemHelper);
				PageTablixHelper pageTablixHelper = itemHelper as PageTablixHelper;
				RSTrace.RenderingTracer.Assert(pageTablixHelper != null, "This should be a tablix");
				this.m_levelForRepeat = pageTablixHelper.LevelForRepeat;
				this.m_tablixCreateState = PageItem.GetNewList(pageTablixHelper.TablixCreateState);
				this.m_ignoreTotalsOnLastLevel = pageTablixHelper.IgnoreTotalsOnLastLevel;
				this.SetTablixMembersInstanceIndex(pageTablixHelper.MembersInstanceIndex);
			}
		}

		private int TablixMembersDepthTree(TablixMemberCollection memberCollection)
		{
			if (memberCollection != null && memberCollection.Count != 0)
			{
				int num = 0;
				for (int i = 0; i < memberCollection.Count; i++)
				{
					TablixMember tablixMember = ((ReportElementCollectionBase<TablixMember>)memberCollection)[i];
					int num2 = 1;
					if (tablixMember.TablixHeader != null)
					{
						num2 = (tablixMember.IsColumn ? tablixMember.TablixHeader.CellContents.RowSpan : tablixMember.TablixHeader.CellContents.ColSpan);
					}
					num = Math.Max(num, this.TablixMembersDepthTree(tablixMember.Children) + num2 - 1);
				}
				return num + 1;
			}
			return 0;
		}

		private InnerToggleState GetInnerToggleStateForParent(TablixMemberCollection members)
		{
			if (members != null && members.Count != 0)
			{
				bool flag = false;
				List<InnerToggleState> list = new List<InnerToggleState>(members.Count);
				if (this.GetInnerToggleState(members, ref flag, list))
				{
					return new InnerToggleState(false, list);
				}
				if (flag)
				{
					return new InnerToggleState(true, null);
				}
				return null;
			}
			return null;
		}

		private InnerToggleState GetInnerToggleState(TablixMember rowMember)
		{
			return this.GetInnerToggleStateForParent(rowMember.Children);
		}

		private bool GetInnerToggleState(TablixMemberCollection members, ref bool toggleState, List<InnerToggleState> membersToggleState)
		{
			bool result = false;
			InnerToggleState innerToggleState = null;
			Visibility visibility = null;
			for (int i = 0; i < members.Count; i++)
			{
				visibility = ((ReportElementCollectionBase<TablixMember>)members)[i].Visibility;
				if (visibility != null && visibility.ToggleItem != null)
				{
					if (((ReportElementCollectionBase<TablixMember>)members)[i].IsStatic)
					{
						toggleState = true;
					}
					membersToggleState.Add(null);
				}
				else if (((ReportElementCollectionBase<TablixMember>)members)[i].IsTotal)
				{
					membersToggleState.Add(null);
				}
				else
				{
					innerToggleState = this.GetInnerToggleState(((ReportElementCollectionBase<TablixMember>)members)[i]);
					if (innerToggleState != null)
					{
						result = true;
					}
					membersToggleState.Add(innerToggleState);
				}
			}
			return result;
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

		private double CornerSize(TablixMemberCollection memberCollection)
		{
			double num = 0.0;
			if (memberCollection != null && memberCollection.Count != 0)
			{
				TablixHeader tablixHeader = ((ReportElementCollectionBase<TablixMember>)memberCollection)[0].TablixHeader;
				if (tablixHeader != null)
				{
					num = tablixHeader.Size.ToMillimeters();
				}
				return num + this.CornerSize(((ReportElementCollectionBase<TablixMember>)memberCollection)[0].Children);
			}
			return num;
		}

		private void CreateDetailCell(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember colMemberParent, int colGridIndex, TablixContext context)
		{
			TablixCell tablixCell = null;
			if (context.PartialItemHelper != null)
			{
				int memberCellIndex = colMemberParent.MemberCellIndex;
				tablixCell = ((ReportElementCollectionBase<TablixCell>)((ReportElementCollectionBase<TablixRow>)this.m_bodyRows)[context.RowMemberIndexCell])[memberCellIndex];
				context.ColMemberIndexCell = memberCellIndex;
				if (tablixCell.CellContents.ColSpan == 1)
				{
					context.ColMemberIndexCell = -1;
				}
				this.m_partialPageItem = context.AddDetailCellFromState(tablixCell, this.m_ignoreCellPageBreaks);
			}
			if (this.m_partialPageItem != null)
			{
				context.StaticDetailRow = false;
				if (this.m_partialPageItem.ItemState == State.OnPagePBEnd)
				{
					context.AddDetailEmptyCell(colGridIndex, this.m_bodyColWidths[colMemberParent.MemberCellIndex], 0.0);
					this.m_partialPageItem = null;
				}
				else
				{
					this.m_partialPageItem.UpdateSizes(0.0, null, null);
					if (this.m_partialPageItem.ItemState == State.TopNextPage)
					{
						this.m_partialPageItem.ItemState = State.OnPage;
					}
					if (!context.CalculateDetailCell(this.m_partialPageItem, colGridIndex, true))
					{
						this.m_partialPageItem = null;
					}
				}
				context.PartialItemHelper = null;
			}
			else
			{
				int memberCellIndex = colMemberParent.MemberCellIndex;
				tablixCell = ((ReportElementCollectionBase<TablixCell>)((ReportElementCollectionBase<TablixRow>)this.m_bodyRows)[context.RowMemberIndexCell])[memberCellIndex];
				double cellColDefWidth = 0.0;
				bool collect = true;
				if (this.m_ignoreCellPageBreaks)
				{
					cellColDefWidth = this.m_bodyColWidths[memberCellIndex];
				}
				if (tablixCell == null)
				{
					while (memberCellIndex > 0)
					{
						memberCellIndex--;
						tablixCell = ((ReportElementCollectionBase<TablixCell>)((ReportElementCollectionBase<TablixRow>)this.m_bodyRows)[context.RowMemberIndexCell])[memberCellIndex];
						if (tablixCell != null)
						{
							break;
						}
					}
					collect = false;
				}
				if (context.ColMemberIndexCell >= 0 && memberCellIndex == context.ColMemberIndexCell)
				{
					context.UpdateDetailCell(cellColDefWidth);
				}
				else
				{
					context.DelayedCalculation();
					context.ColMemberIndexCell = memberCellIndex;
					if (tablixCell.CellContents.ColSpan == 1)
					{
						context.ColMemberIndexCell = -1;
					}
					bool flag = false;
					ItemOffset itemOffset = context.AddDetailCell(tablixCell, colGridIndex, cellColDefWidth, this.m_bodyRowsHeigths[context.RowMemberIndexCell], this.m_ignoreCellPageBreaks, collect, out flag);
					if (flag)
					{
						this.m_partialPageItem = (PageItem)itemOffset;
						context.StaticDetailRow = false;
					}
				}
			}
		}

		private int CreateColumnMemberChildren(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember colMemberParent, int defTreeLevel, bool parentBorderHeader, int parentRowIndex, int parentColIndex, TablixContext context, bool createDetail, ref LevelInfo parentLevelInfo)
		{
			if (parentLevelInfo == null)
			{
				parentLevelInfo = new LevelInfo();
			}
			else
			{
				parentLevelInfo.SetDefaults();
			}
			TablixMemberCollection tablixMemberCollection = null;
			int num = 0;
			if (colMemberParent == null)
			{
				if (context.ColsBeforeRowHeaders == 0)
				{
					num = tablix.GroupsBeforeRowHeaders;
				}
				context.CreateCorner(tablix.Corner);
				tablixMemberCollection = tablix.ColumnHierarchy.MemberCollection;
				parentColIndex = context.HeaderRowColumns;
			}
			else
			{
				tablixMemberCollection = colMemberParent.Children;
			}
			if (tablixMemberCollection == null)
			{
				if (context.PageContext.CancelPage)
				{
					return 0;
				}
				if (context.IgnoreColumn)
				{
					parentLevelInfo.IgnoredRowsCols++;
					if (createDetail)
					{
						context.AddDetailCellToCurrentPage(tablix, colMemberParent.MemberCellIndex);
					}
					return 0;
				}
				if (createDetail)
				{
					this.CreateDetailCell(tablix, colMemberParent, parentColIndex, context);
				}
				return 1;
			}
			int num2 = parentColIndex;
			int num3 = 0;
			int num4 = 0;
			bool flag = true;
			LevelInfo levelInfo = null;
			bool flag2 = true;
			int num5 = 0;
			bool flag3 = false;
			byte b = 0;
			int num6 = 0;
			bool hasRecursivePeer = false;
			TablixMember tablixMember = null;
			Visibility visibility = null;
			TablixMemberInstance tablixMemberInstance = null;
			TablixDynamicMemberInstance tablixDynamicMemberInstance = null;
			List<int> list = null;
			bool flag4 = false;
			parentLevelInfo.HasVisibleStaticPeer = Tablix.CheckForVisibleStaticPeer(tablixMemberCollection);
			for (int i = 0; i < tablixMemberCollection.Count; i++)
			{
				tablixMember = ((ReportElementCollectionBase<TablixMember>)tablixMemberCollection)[i];
				visibility = tablixMember.Visibility;
				flag3 = parentBorderHeader;
				if (context.PageContext.CancelPage)
				{
					break;
				}
				if (context.NoRows && tablixMember.HideIfNoRows)
				{
					context.AddMemberToCurrentPage(tablix, tablixMember, TablixRegion.ColumnHeader, createDetail);
				}
				else if (!context.AlwaysHiddenMember(tablix, tablixMember, visibility, TablixRegion.ColumnHeader, createDetail, ref parentLevelInfo))
				{
					flag = true;
					flag4 = false;
					tablixMemberInstance = tablixMember.Instance;
					if (tablixMember.Group != null)
					{
						hasRecursivePeer = tablixMember.Group.IsRecursive;
					}
					if (tablixMember.IsStatic)
					{
						num = 0;
						flag2 = context.EnterColMemberInstance(tablixMember, visibility, hasRecursivePeer, parentLevelInfo.HasVisibleStaticPeer, out b);
						if (tablixMember.IsTotal)
						{
							if (!flag2 && num2 <= num5 && num2 <= parentColIndex)
							{
								if (list == null)
								{
									list = new List<int>();
								}
								list.Add(i);
								flag4 = true;
							}
						}
						else
						{
							if (!flag3)
							{
								flag3 = this.StaticDecendents(tablixMember.Children);
							}
							if (flag3)
							{
								b = (byte)(b | 1);
							}
						}
					}
					else
					{
						context.DelayedCalculation();
						context.ColMemberIndexCell = -1;
						num5 = num2;
						if (i > 0)
						{
							num = 0;
						}
						tablixDynamicMemberInstance = (TablixDynamicMemberInstance)tablixMemberInstance;
						tablixDynamicMemberInstance.ResetContext();
						flag = tablixDynamicMemberInstance.MoveNext();
						if (flag)
						{
							flag2 = context.EnterColMemberInstance(tablixMember, visibility, false, false, out b);
						}
					}
					while (flag)
					{
						if (flag2)
						{
							num3 = 0;
							if (tablixMember.TablixHeader != null)
							{
								num3 = tablixMember.TablixHeader.CellContents.RowSpan;
								num6 = defTreeLevel - num3;
							}
							else
							{
								num6 = defTreeLevel - 1;
							}
							num4 = this.CreateColumnMemberChildren(tablix, tablixMember, num6, flag3, parentRowIndex + num3, num2, context, createDetail, ref levelInfo);
							if (num4 > 0)
							{
								PageMemberCell pageMemberCell = context.AddColMember(tablixMember, parentRowIndex, num2, num3, num4, b, defTreeLevel, levelInfo);
								if (pageMemberCell != null)
								{
									if (!levelInfo.OmittedList)
									{
										pageMemberCell.Children = levelInfo.MemberCells;
										if (levelInfo.OmittedMembersCells)
										{
											pageMemberCell.HasOmittedChildren = true;
										}
									}
									if (pageMemberCell.RowSpan == 0)
									{
										parentLevelInfo.OmittedMembersCells = true;
									}
									parentLevelInfo.AddMemberCell(pageMemberCell, pageMemberCell.RowSpan, defTreeLevel, i, this.m_bodyColWidths[tablixMember.MemberCellIndex], context.Cache);
								}
								num2 += num4;
								context.AddTotalsToCurrentPage(ref list, tablixMemberCollection, TablixRegion.ColumnHeader, createDetail);
							}
							else if (levelInfo.IgnoredRowsCols > 0)
							{
								context.AddMemberHeaderToCurrentPage(tablixMember, TablixRegion.ColumnHeader, true);
								parentLevelInfo.IgnoredRowsCols += levelInfo.IgnoredRowsCols;
							}
							else
							{
								context.AddMemberHeaderToCurrentPage(tablixMember, TablixRegion.ColumnHeader, false);
							}
						}
						else if (!flag4)
						{
							context.AddMemberToCurrentPage(tablix, tablixMember, TablixRegion.ColumnHeader, createDetail);
						}
						context.LeaveColMemberInstance(tablixMember, visibility);
						if (tablixMember.IsStatic || context.PageContext.CancelPage)
						{
							flag = false;
						}
						else
						{
							context.DelayedCalculation();
							flag = tablixDynamicMemberInstance.MoveNext();
							context.ColMemberIndexCell = -1;
							if (num > 0)
							{
								context.ColsBeforeRowHeaders += num4;
								num--;
							}
							if (flag)
							{
								flag2 = context.EnterColMemberInstance(tablixMember, visibility, false, false, out b);
							}
						}
						num4 = 0;
					}
					tablixDynamicMemberInstance = null;
				}
			}
			if (num2 <= parentColIndex)
			{
				num2 += this.CreateColumnMemberTotals(tablix, list, tablixMemberCollection, defTreeLevel, parentBorderHeader, parentRowIndex, parentColIndex, context, createDetail, parentLevelInfo);
			}
			return num2 - parentColIndex;
		}

		private int CreateColumnMemberTotals(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix, List<int> totals, TablixMemberCollection columnMembers, int defTreeLevel, bool parentBorderHeader, int parentRowIndex, int parentColIndex, TablixContext context, bool createDetail, LevelInfo parentLevelInfo)
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
				for (int j = 0; j < totals.Count; j++)
				{
					num2 = totals[j];
					tablixMember = ((ReportElementCollectionBase<TablixMember>)columnMembers)[num2];
					if (context.PageContext.CancelPage)
					{
						break;
					}
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
					num5 = this.CreateColumnMemberChildren(tablix, tablixMember, num6, parentBorderHeader, parentRowIndex + num4, num3, context, createDetail, ref levelInfo);
					if (num5 > 0)
					{
						PageMemberCell pageMemberCell = context.AddTotalColMember(tablixMember, parentRowIndex, num3, num4, num5, 0, defTreeLevel, parentLevelInfo, levelInfo);
						if (pageMemberCell != null)
						{
							if (!levelInfo.OmittedList)
							{
								pageMemberCell.Children = levelInfo.MemberCells;
								if (levelInfo.OmittedMembersCells)
								{
									pageMemberCell.HasOmittedChildren = true;
								}
							}
							if (pageMemberCell.RowSpan == 0)
							{
								parentLevelInfo.OmittedMembersCells = true;
							}
							parentLevelInfo.AddMemberCell(pageMemberCell, pageMemberCell.RowSpan, defTreeLevel, num2, this.m_bodyColWidths[tablixMember.MemberCellIndex], context.Cache);
						}
					}
					else if (levelInfo.IgnoredRowsCols > 0)
					{
						context.AddMemberHeaderToCurrentPage(tablixMember, TablixRegion.ColumnHeader, true);
						parentLevelInfo.IgnoredRowsCols += levelInfo.IgnoredRowsCols;
					}
					else
					{
						context.AddMemberHeaderToCurrentPage(tablixMember, TablixRegion.ColumnHeader, false);
					}
					num3 += num5;
				}
				return num3 - parentColIndex;
			}
			return 0;
		}

		private int CheckKeepWithGroupUp(TablixMemberCollection rowMembers, int start, KeepWithGroup keepWith)
		{
			TablixMember tablixMember = null;
			while (start >= 0)
			{
				tablixMember = ((ReportElementCollectionBase<TablixMember>)rowMembers)[start];
				if (!tablixMember.IsStatic || tablixMember.KeepWithGroup != keepWith)
				{
					return start + 1;
				}
				start--;
			}
			return 0;
		}

		private int CheckKeepWithGroupDown(TablixMemberCollection rowMembers, int start, KeepWithGroup keepWith)
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
			}
			return start - 1;
		}

		private int CreateKeepWithRowMemberChildren(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember rowMemberParent, InnerToggleState parentToggleState, int defTreeLevel, int parentRowIndex, int parentColIndex, int level, int start, int end, ref LevelInfo parentLevelInfo, TablixContext context)
		{
			if (start <= end && !context.PageContext.CancelPage)
			{
				TablixMemberCollection tablixMemberCollection = null;
				tablixMemberCollection = ((rowMemberParent != null) ? rowMemberParent.Children : tablix.RowHierarchy.MemberCollection);
				if (tablixMemberCollection == null)
				{
					if (context.PageContext.CancelPage)
					{
						return 0;
					}
					context.RowMemberIndexCell = rowMemberParent.MemberCellIndex;
					if (!context.IgnoreRow)
					{
						LevelInfo levelInfo = null;
						bool keepTogether = context.PageContext.KeepTogether;
						context.PageContext.KeepTogether = true;
						this.CreateColumnMemberChildren(tablix, null, this.m_colMembersDepth, false, 0, 0, context, true, ref levelInfo);
						context.PageContext.KeepTogether = keepTogether;
						if (!context.ColumnHeadersCreated)
						{
							context.OuterColumnHeaders = levelInfo.MemberCells;
							context.OmittedOuterColumnHeaders = levelInfo.OmittedMembersCells;
							context.ColumnHeadersCreated = true;
						}
						context.WriteDetailRow(parentRowIndex, this.m_bodyRowsHeigths, this.m_ignoreCellPageBreaks);
					}
					else
					{
						context.AddDetailRowToCurrentPage(tablix);
						parentLevelInfo.IgnoredRowsCols++;
					}
					context.NextRow(this.m_bodyRowsHeigths);
					if (context.PageContext.TracingEnabled && context.TablixBottom > context.PageContext.PageHeight)
					{
						this.TracePageGrownOnKeepWithMember(context.PageContext.PageNumber);
					}
					if (!context.IgnoreRow)
					{
						return 1;
					}
					return 0;
				}
				int num = parentRowIndex;
				int num2 = 0;
				int num3 = 0;
				LevelInfo levelInfo2 = null;
				bool flag = true;
				TablixMember tablixMember = null;
				Visibility visibility = null;
				bool flag2 = false;
				byte b = 0;
				int num4 = 0;
				bool hasRecursivePeer = false;
				InnerToggleState innerToggleState = null;
				if (start == -1 && end == -1)
				{
					start = 0;
					end = tablixMemberCollection.Count - 1;
				}
				for (int i = start; i <= end; i++)
				{
					tablixMember = ((ReportElementCollectionBase<TablixMember>)tablixMemberCollection)[i];
					visibility = tablixMember.Visibility;
					if (context.PageContext.CancelPage)
					{
						break;
					}
					if (!context.AlwaysHiddenMember(tablix, tablixMember, visibility, TablixRegion.RowHeader, true, ref parentLevelInfo))
					{
						if (context.PageContext.TracingEnabled)
						{
							this.RegisterTablixMemberIndex(tablixMember.DefinitionPath, i);
						}
						if (parentToggleState != null && parentToggleState.Children != null)
						{
							innerToggleState = parentToggleState.Children[i];
						}
						if (tablixMember.Group != null)
						{
							hasRecursivePeer = tablixMember.Group.IsRecursive;
						}
						flag = context.EnterRowMember(this, tablixMember, visibility, innerToggleState, hasRecursivePeer, parentLevelInfo.HasVisibleStaticPeer, out b);
						b = (byte)(b | 1);
						if (flag)
						{
							num2 = 0;
							if (tablixMember.TablixHeader != null)
							{
								num2 = tablixMember.TablixHeader.CellContents.ColSpan;
								num4 = defTreeLevel - num2;
							}
							else
							{
								num4 = defTreeLevel - 1;
							}
							bool flag3 = context.PageContext.TracingEnabled && this.RegisterOutermostKeepWithMember(tablixMember);
							bool flag4 = context.PageContext.TracingEnabled && tablixMember.RepeatOnNewPage && this.RegisterOutermostRepeatOnNewPage();
							levelInfo2 = new LevelInfo();
							num3 = this.CreateKeepWithRowMemberChildren(tablix, tablixMember, innerToggleState, num4, num, parentColIndex + num2, level + 1, -1, -1, ref levelInfo2, context);
							if (num3 > 0)
							{
								if (context.PageContext.TracingEnabled && flag4)
								{
									this.TraceRepeatOnNewPage(context.PageContext.PageNumber, tablixMember);
								}
								PageMemberCell pageMemberCell = context.AddRowMember(tablixMember, num, parentColIndex, num3, num2, b, defTreeLevel, null);
								if (pageMemberCell != null)
								{
									if (!levelInfo2.OmittedList)
									{
										pageMemberCell.Children = levelInfo2.MemberCells;
									}
									parentLevelInfo.AddMemberCell(pageMemberCell, pageMemberCell.ColSpan, defTreeLevel, i, this.m_bodyRowsHeigths[tablixMember.MemberCellIndex], context.Cache);
								}
								num += num3;
							}
							else if (levelInfo2.IgnoredRowsCols > 0)
							{
								context.AddMemberHeaderToCurrentPage(tablixMember, TablixRegion.RowHeader, true);
								parentLevelInfo.IgnoredRowsCols += levelInfo2.IgnoredRowsCols;
							}
							else
							{
								context.AddMemberHeaderToCurrentPage(tablixMember, TablixRegion.RowHeader, false);
							}
							if (flag3)
							{
								this.UnregisterOutermostKeepWithMember();
							}
							if (flag4)
							{
								this.UnregisterRepeatOnNewPagePath();
							}
						}
						else
						{
							context.AddMemberToCurrentPage(tablix, tablixMember, TablixRegion.RowHeader, true);
						}
						context.LeaveRowMemberInstance(tablixMember, innerToggleState, visibility, hasRecursivePeer);
						context.LeaveRowMember(tablixMember, this.m_bodyRowsHeigths, visibility, hasRecursivePeer, innerToggleState, ref flag2);
					}
				}
				return num - parentRowIndex;
			}
			return 0;
		}

		private int CreateRowMemberChildren(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember rowMemberParent, InnerToggleState parentToggleState, int defTreeLevel, bool parentBorderHeader, int parentRowIndex, int parentColIndex, int level, TablixContext context, ref bool advanceRow, ref LevelInfo parentLevelInfo, ref List<bool> ignoreTotals, bool keepTogether)
		{
			bool flag = false;
			if (parentLevelInfo == null)
			{
				parentLevelInfo = new LevelInfo();
			}
			else
			{
				parentLevelInfo.SetDefaults();
			}
			TablixMemberCollection tablixMemberCollection = null;
			tablixMemberCollection = ((rowMemberParent != null) ? rowMemberParent.Children : tablix.RowHierarchy.MemberCollection);
			if (tablixMemberCollection == null)
			{
				if (context.PageContext.CancelPage)
				{
					advanceRow = false;
					return 0;
				}
				context.RowMemberIndexCell = rowMemberParent.MemberCellIndex;
				if (!context.IgnoreRow)
				{
					LevelInfo levelInfo = null;
					bool keepTogether2 = context.PageContext.KeepTogether;
					context.PageContext.KeepTogether = (keepTogether || keepTogether2);
					this.CreateColumnMemberChildren(tablix, null, this.m_colMembersDepth, false, 0, 0, context, true, ref levelInfo);
					context.PageContext.KeepTogether = keepTogether2;
					if (!context.ColumnHeadersCreated)
					{
						context.OuterColumnHeaders = levelInfo.MemberCells;
						context.OmittedOuterColumnHeaders = levelInfo.OmittedMembersCells;
						context.ColumnHeadersCreated = true;
					}
					context.WriteDetailRow(parentRowIndex, this.m_bodyRowsHeigths, this.m_ignoreCellPageBreaks);
				}
				else
				{
					context.AddDetailRowToCurrentPage(tablix);
					parentLevelInfo.IgnoredRowsCols++;
				}
				advanceRow = context.AdvanceRow(this.m_bodyRowsHeigths, this.m_tablixCreateState, level);
				if (!context.IgnoreRow)
				{
					return 1;
				}
				return 0;
			}
			int num = parentRowIndex;
			int num2 = 0;
			int num3 = 0;
			bool flag2 = true;
			LevelInfo levelInfo2 = null;
			bool flag3 = true;
			int num4 = -1;
			int num5 = 0;
			bool flag4 = true;
			bool flag5 = false;
			double num6 = 0.0;
			bool flag6 = false;
			byte b = 0;
			int num7 = 0;
			bool flag7 = false;
			bool hasRecursivePeer = false;
			TablixMember tablixMember = null;
			Visibility visibility = null;
			TablixMemberInstance tablixMemberInstance = null;
			TablixDynamicMemberInstance tablixDynamicMemberInstance = null;
			List<int> list = null;
			bool flag8 = false;
			bool flag9 = true;
			InnerToggleState innerToggleState = null;
			parentLevelInfo.HasVisibleStaticPeer = Tablix.CheckForVisibleStaticPeer(tablixMemberCollection);
			if (this.m_tablixCreateState == null || this.m_tablixCreateState.Count <= level)
			{
				if (this.m_tablixCreateState == null)
				{
					this.m_tablixCreateState = new List<int>();
				}
				this.m_tablixCreateState.Add(num5);
				if (ignoreTotals == null)
				{
					ignoreTotals = new List<bool>();
				}
				ignoreTotals.Add(false);
			}
			else
			{
				num5 = this.m_tablixCreateState[level];
				if (num5 < 0)
				{
					num5 = -num5;
					this.m_tablixCreateState[level] = num5;
				}
				else
				{
					flag4 = false;
					if (level <= this.m_partialMemberLevel)
					{
						flag9 = false;
					}
				}
			}
			for (int i = num5; i < tablixMemberCollection.Count; i++)
			{
				tablixMember = ((ReportElementCollectionBase<TablixMember>)tablixMemberCollection)[i];
				visibility = tablixMember.Visibility;
				flag6 = parentBorderHeader;
				if (context.PageContext.TracingEnabled)
				{
					this.RegisterTablixMemberIndex(tablixMember.DefinitionPath, i);
				}
				if (context.PageContext.CancelPage)
				{
					advanceRow = false;
					break;
				}
				if (context.NoRows && tablixMember.HideIfNoRows)
				{
					context.AddMemberToCurrentPage(tablix, tablixMember, TablixRegion.RowHeader, true);
				}
				else if (!context.AlwaysHiddenMember(tablix, tablixMember, visibility, TablixRegion.RowHeader, true, ref parentLevelInfo))
				{
					flag5 = false;
					flag3 = true;
					flag2 = true;
					flag8 = false;
					num6 = 0.0;
					tablixMemberInstance = tablixMember.Instance;
					if (parentToggleState != null && parentToggleState.Children != null)
					{
						innerToggleState = parentToggleState.Children[i];
					}
					if (tablixMember.Group != null)
					{
						hasRecursivePeer = tablixMember.Group.IsRecursive;
					}
					if (tablixMember.IsStatic)
					{
						context.SharedLayoutRow = 0;
						if (!advanceRow && !tablixMember.IsTotal && !context.IsTotal)
						{
							if (num4 >= 0 && (num > num4 || flag7))
							{
								int num8 = this.CheckKeepWithGroupDown(tablixMemberCollection, i, KeepWithGroup.Before);
								if (num8 >= i)
								{
									context.RepeatWith = true;
									num += this.CreateKeepWithRowMemberChildren(tablix, rowMemberParent, parentToggleState, defTreeLevel, num, parentColIndex, level, i, num8, ref parentLevelInfo, context);
									context.RepeatWith = false;
									i = num8;
									num4 = -1;
									continue;
								}
							}
							this.m_tablixCreateState[level] = i;
							return num - parentRowIndex;
						}
						flag3 = context.EnterRowMember(this, tablixMember, visibility, innerToggleState, hasRecursivePeer, parentLevelInfo.HasVisibleStaticPeer, out b);
						if (tablixMember.IsTotal)
						{
							if (!flag3 && num <= num4 && num <= parentRowIndex && !ignoreTotals[level])
							{
								if (list == null)
								{
									list = new List<int>();
								}
								list.Add(i);
								flag8 = true;
							}
						}
						else
						{
							if (!flag6)
							{
								flag6 = this.StaticDecendents(tablixMember.Children);
							}
							if (flag6)
							{
								b = (byte)(b | 1);
							}
						}
						if (flag3)
						{
							if (this.KeepTogetherStaticHeader(tablixMemberCollection, tablixMember, i, context))
							{
								context.RegisterKeepWith();
							}
							else if (num4 >= 0 && (num > num4 || flag7) && tablixMember.KeepWithGroup == KeepWithGroup.Before)
							{
								context.RegisterKeepWith();
							}
						}
						num4 = -1;
					}
					else
					{
						context.SharedLayoutRow = 2;
						num4 = num;
						tablixDynamicMemberInstance = (TablixDynamicMemberInstance)tablixMemberInstance;
						if (flag4 || i > num5)
						{
							tablixDynamicMemberInstance.ResetContext();
							flag2 = tablixDynamicMemberInstance.MoveNext();
							flag5 = true;
							if (flag2)
							{
								flag2 = this.CheckAndAdvanceToNextVisibleInstance(tablix, tablixMember, tablixDynamicMemberInstance, context);
							}
							flag9 = true;
						}
						else if (level <= this.m_levelForRepeat)
						{
							int num9 = this.CheckKeepWithGroupUp(tablixMemberCollection, num5 - 1, KeepWithGroup.After);
							if (num9 < num5 && ((ReportElementCollectionBase<TablixMember>)tablixMemberCollection)[num9].RepeatOnNewPage)
							{
								context.RepeatWith = true;
								num += this.CreateKeepWithRowMemberChildren(tablix, rowMemberParent, parentToggleState, defTreeLevel, num, parentColIndex, level, num9, num5 - 1, ref parentLevelInfo, context);
								context.RepeatWith = false;
							}
						}
						if (!advanceRow)
						{
							if (flag2)
							{
								this.m_tablixCreateState[level] = i;
								return num - parentRowIndex;
							}
							continue;
						}
						if (flag2)
						{
							flag3 = context.EnterRowMember(this, tablixMember, visibility, innerToggleState, hasRecursivePeer, false, out b);
						}
						if (flag3)
						{
							int num10 = this.CheckKeepWithGroupDown(tablixMemberCollection, i + 1, KeepWithGroup.Before);
							if (num10 > i && ((ReportElementCollectionBase<TablixMember>)tablixMemberCollection)[num10].RepeatOnNewPage)
							{
								context.RepeatWith = true;
								num6 = this.RegisterSizeForRepeatWithBefore(tablix, rowMemberParent, i + 1, num10, context);
								context.RepeatWith = false;
							}
						}
					}
					flag7 = false;
					while (flag2)
					{
						if (flag3)
						{
							if (!tablixMember.IsStatic && flag5 && context.CheckPageBreaks && (num > parentRowIndex || parentRowIndex > 0))
							{
								PageBreakInfo pageBreakInfo = Tablix.CreatePageBreakInfo(tablixMember.Group);
								if (pageBreakInfo != null && !pageBreakInfo.Disabled)
								{
									PageBreakLocation breakLocation = pageBreakInfo.BreakLocation;
									if ((breakLocation == PageBreakLocation.Start || breakLocation == PageBreakLocation.StartAndEnd) && this.DynamicWithVisibleChildren(tablixMember, context))
									{
										advanceRow = false;
										this.m_tablixCreateState[level] = i;
										context.PageContext.RegisterPageBreak(pageBreakInfo, context.PageBreakNeedsOverride);
										return num - parentRowIndex;
									}
								}
							}
							num3 = 0;
							if (tablixMember.TablixHeader != null)
							{
								num3 = tablixMember.TablixHeader.CellContents.ColSpan;
								num7 = defTreeLevel - num3;
							}
							else
							{
								num7 = defTreeLevel - 1;
							}
							bool overrideChild = !context.PageContext.IsPageNameRegistered;
							num2 = this.CreateRowMemberChildren(tablix, tablixMember, innerToggleState, num7, flag6, num, parentColIndex + num3, level + 1, context, ref advanceRow, ref levelInfo2, ref ignoreTotals, keepTogether || tablixMember.KeepTogether);
							if (!advanceRow && keepTogether && !context.PageContext.IsPageBreakRegistered)
							{
								if (context.PageContext.TracingEnabled && !flag && rowMemberParent != null && rowMemberParent.KeepTogether)
								{
									this.TracePageGrownOnKeepTogetherMember(context.PageContext.PageNumber, rowMemberParent);
								}
								flag = true;
								advanceRow = true;
							}
							if (num2 > 0)
							{
								flag5 = false;
								PageMemberCell pageMemberCell = context.AddRowMember(tablixMember, num, parentColIndex, num2, num3, b, defTreeLevel, levelInfo2);
								if (flag9 && tablixMember.Group != null && context.CheckPageBreaks)
								{
									context.PageContext.RegisterPageName(tablixMember.Group.Instance.PageName, overrideChild);
								}
								if (pageMemberCell != null)
								{
									if (!levelInfo2.OmittedList)
									{
										pageMemberCell.Children = levelInfo2.MemberCells;
									}
									parentLevelInfo.AddMemberCell(pageMemberCell, pageMemberCell.ColSpan, defTreeLevel, i, this.m_bodyRowsHeigths[tablixMember.MemberCellIndex], context.Cache);
								}
								num += num2;
								context.AddTotalsToCurrentPage(ref list, tablixMemberCollection, TablixRegion.RowHeader, true);
								ignoreTotals[level] = true;
							}
							else if (levelInfo2.IgnoredRowsCols > 0)
							{
								context.AddMemberHeaderToCurrentPage(tablixMember, TablixRegion.RowHeader, true);
								parentLevelInfo.IgnoredRowsCols += levelInfo2.IgnoredRowsCols;
								flag7 = true;
							}
							else
							{
								context.AddMemberHeaderToCurrentPage(tablixMember, TablixRegion.RowHeader, false);
							}
						}
						else if (!flag8)
						{
							context.AddMemberToCurrentPage(tablix, tablixMember, TablixRegion.RowHeader, true);
						}
						context.LeaveRowMemberInstance(tablixMember, innerToggleState, visibility, hasRecursivePeer);
						flag9 = true;
						if (tablixMember.IsStatic)
						{
							flag2 = false;
							context.LeaveRowMember(tablixMember, this.m_bodyRowsHeigths, visibility, hasRecursivePeer, innerToggleState, ref advanceRow);
							if (!advanceRow && level + 1 < this.m_tablixCreateState.Count)
							{
								this.m_tablixCreateState[level] = i;
								return num - parentRowIndex;
							}
							if (context.KeepWith)
							{
								int num11 = this.CheckKeepWithGroupDown(tablixMemberCollection, i + 1, tablixMember.KeepWithGroup);
								if (num11 > i)
								{
									num += this.CreateKeepWithRowMemberChildren(tablix, rowMemberParent, parentToggleState, defTreeLevel, num, parentColIndex, level, i + 1, num11, ref parentLevelInfo, context);
									i = num11;
								}
								context.UnRegisterKeepWith(tablixMember.KeepWithGroup, this.m_bodyRowsHeigths, ref advanceRow);
							}
						}
						else
						{
							if (advanceRow)
							{
								PageBreakInfo pageBreakInfo2 = Tablix.CreatePageBreakInfo(tablixMember.Group);
								PageBreakLocation pageBreakLocation = (pageBreakInfo2 != null && !pageBreakInfo2.Disabled) ? pageBreakInfo2.BreakLocation : PageBreakLocation.None;
								flag2 = this.AdvanceToNextVisibleInstance(tablix, tablixMember, tablixDynamicMemberInstance, context);
								if (context.CheckPageBreaks && num > num4)
								{
									PageBreakInfo pageBreakInfo3 = Tablix.CreatePageBreakInfo(tablixMember.Group);
									PageBreakLocation pageBreakLocation2 = (pageBreakInfo3 != null && !pageBreakInfo3.Disabled) ? pageBreakInfo3.BreakLocation : PageBreakLocation.None;
									if (flag2)
									{
										if (pageBreakLocation != PageBreakLocation.StartAndEnd && pageBreakLocation != PageBreakLocation.End)
										{
											if ((pageBreakLocation2 == PageBreakLocation.Between || pageBreakLocation2 == PageBreakLocation.Start) && this.DynamicWithVisibleChildren(tablixMember, context))
											{
												advanceRow = false;
												num4 = -1;
												int num12 = this.CheckKeepWithGroupDown(tablixMemberCollection, i + 1, KeepWithGroup.Before);
												if (num12 > i && ((ReportElementCollectionBase<TablixMember>)tablixMemberCollection)[num12].RepeatOnNewPage)
												{
													context.IgnoreHeight = true;
													num += this.CreateKeepWithRowMemberChildren(tablix, rowMemberParent, parentToggleState, defTreeLevel, num, parentColIndex, level, i + 1, num12, ref parentLevelInfo, context);
													context.IgnoreHeight = false;
												}
												context.PageContext.RegisterPageBreak(pageBreakInfo3, context.PageBreakNeedsOverride);
												this.m_tablixCreateState[level] = i;
												return num - parentRowIndex;
											}
											goto IL_0ae6;
										}
										advanceRow = false;
										num4 = -1;
										int num13 = this.CheckKeepWithGroupDown(tablixMemberCollection, i + 1, KeepWithGroup.Before);
										if (num13 > i && ((ReportElementCollectionBase<TablixMember>)tablixMemberCollection)[num13].RepeatOnNewPage)
										{
											context.IgnoreHeight = true;
											num += this.CreateKeepWithRowMemberChildren(tablix, rowMemberParent, parentToggleState, defTreeLevel, num, parentColIndex, level, i + 1, num13, ref parentLevelInfo, context);
											context.IgnoreHeight = false;
										}
										context.PageContext.RegisterPageBreak(pageBreakInfo2, context.PageBreakNeedsOverride);
										this.m_tablixCreateState[level] = i;
										return num - parentRowIndex;
									}
									if (pageBreakLocation == PageBreakLocation.End || pageBreakLocation == PageBreakLocation.StartAndEnd)
									{
										advanceRow = false;
										context.PageBreakAtEnd = true;
										num4 = -1;
										int num14 = this.CheckKeepWithGroupDown(tablixMemberCollection, i + 1, KeepWithGroup.Before);
										if (num14 > i && ((ReportElementCollectionBase<TablixMember>)tablixMemberCollection)[num14].RepeatOnNewPage)
										{
											context.IgnoreHeight = true;
											num += this.CreateKeepWithRowMemberChildren(tablix, rowMemberParent, parentToggleState, defTreeLevel, num, parentColIndex, level, i + 1, num14, ref parentLevelInfo, context);
											context.IgnoreHeight = false;
										}
										context.PageContext.RegisterPageBreak(pageBreakInfo2, context.PageBreakNeedsOverride);
									}
									goto IL_0ae6;
								}
								goto IL_0b6a;
							}
							if (level + 1 < this.m_tablixCreateState.Count)
							{
								this.m_tablixCreateState[level] = i;
								int num15 = this.CheckKeepWithGroupDown(tablixMemberCollection, i + 1, KeepWithGroup.Before);
								if (num15 > i && ((ReportElementCollectionBase<TablixMember>)tablixMemberCollection)[num15].RepeatOnNewPage)
								{
									context.IgnoreHeight = true;
									num += this.CreateKeepWithRowMemberChildren(tablix, rowMemberParent, parentToggleState, defTreeLevel, num, parentColIndex, level, i + 1, num15, ref parentLevelInfo, context);
									context.IgnoreHeight = false;
								}
								return num - parentRowIndex;
							}
							bool flag10 = false;
							PageBreakInfo pageBreakInfo4 = Tablix.CreatePageBreakInfo(tablixMember.Group);
							if (pageBreakInfo4 != null && !pageBreakInfo4.Disabled)
							{
								PageBreakLocation breakLocation2 = pageBreakInfo4.BreakLocation;
								if (breakLocation2 == PageBreakLocation.StartAndEnd || breakLocation2 == PageBreakLocation.End)
								{
									flag10 = true;
									context.PageContext.RegisterPageBreak(pageBreakInfo4, context.PageBreakNeedsOverride);
								}
							}
							flag2 = this.AdvanceToNextVisibleInstance(tablix, tablixMember, tablixDynamicMemberInstance, context);
							if (flag2)
							{
								this.m_tablixCreateState[level] = i;
								int num16 = this.CheckKeepWithGroupDown(tablixMemberCollection, i + 1, KeepWithGroup.Before);
								if (num16 > i && ((ReportElementCollectionBase<TablixMember>)tablixMemberCollection)[num16].RepeatOnNewPage)
								{
									context.IgnoreHeight = true;
									num += this.CreateKeepWithRowMemberChildren(tablix, rowMemberParent, parentToggleState, defTreeLevel, num, parentColIndex, level, i + 1, num16, ref parentLevelInfo, context);
									context.IgnoreHeight = false;
								}
								pageBreakInfo4 = Tablix.CreatePageBreakInfo(tablixMember.Group);
								if (pageBreakInfo4 != null && !pageBreakInfo4.Disabled)
								{
									PageBreakLocation breakLocation3 = pageBreakInfo4.BreakLocation;
									if (breakLocation3 == PageBreakLocation.Start || breakLocation3 == PageBreakLocation.Between)
									{
										context.PageContext.RegisterPageBreak(pageBreakInfo4, context.PageBreakNeedsOverride);
									}
								}
								return num - parentRowIndex;
							}
							if (flag10)
							{
								num4 = -1;
								context.PageBreakAtEnd = true;
								int num17 = this.CheckKeepWithGroupDown(tablixMemberCollection, i + 1, KeepWithGroup.Before);
								if (num17 > i && ((ReportElementCollectionBase<TablixMember>)tablixMemberCollection)[num17].RepeatOnNewPage)
								{
									context.IgnoreHeight = true;
									num += this.CreateKeepWithRowMemberChildren(tablix, rowMemberParent, parentToggleState, defTreeLevel, num, parentColIndex, level, i + 1, num17, ref parentLevelInfo, context);
									context.IgnoreHeight = false;
								}
							}
							context.LeaveRowMember(tablixMember, this.m_bodyRowsHeigths, num6, visibility, hasRecursivePeer, innerToggleState, ref advanceRow);
						}
						continue;
						IL_0b6a:
						if (flag2)
						{
							flag3 = context.EnterRowMemberInstance(this, tablixMember, this.m_bodyRowsHeigths, visibility, hasRecursivePeer, innerToggleState, out b, ref advanceRow);
						}
						else
						{
							context.LeaveRowMember(tablixMember, this.m_bodyRowsHeigths, num6, visibility, hasRecursivePeer, innerToggleState, ref advanceRow);
						}
						continue;
						IL_0ae6:
						if (context.PropagatedPageBreak)
						{
							advanceRow = false;
							context.PageBreakAtEnd = true;
							num4 = -1;
							int num18 = this.CheckKeepWithGroupDown(tablixMemberCollection, i + 1, KeepWithGroup.Before);
							if (num18 > i && ((ReportElementCollectionBase<TablixMember>)tablixMemberCollection)[num18].RepeatOnNewPage)
							{
								context.IgnoreHeight = true;
								num += this.CreateKeepWithRowMemberChildren(tablix, rowMemberParent, parentToggleState, defTreeLevel, num, parentColIndex, level, i + 1, num18, ref parentLevelInfo, context);
								context.IgnoreHeight = false;
							}
							if (flag2)
							{
								this.m_tablixCreateState[level] = i;
								return num - parentRowIndex;
							}
						}
						goto IL_0b6a;
					}
					tablixDynamicMemberInstance = null;
				}
			}
			if (num <= parentRowIndex)
			{
				num += this.CreateRowMemberTotals(tablix, list, tablixMemberCollection, defTreeLevel, parentBorderHeader, parentRowIndex, parentColIndex, level, context, ref advanceRow, parentLevelInfo, ignoreTotals, keepTogether);
			}
			this.m_tablixCreateState.RemoveAt(level);
			ignoreTotals.RemoveAt(level);
			if (keepTogether && flag)
			{
				advanceRow = false;
			}
			return num - parentRowIndex;
		}

		private int CreateRowMemberTotals(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix, List<int> totals, TablixMemberCollection rowMembers, int defTreeLevel, bool parentBorderHeader, int parentRowIndex, int parentColIndex, int level, TablixContext context, ref bool advanceRow, LevelInfo parentLevelInfo, List<bool> ignoreTotals, bool parentKeepTogether)
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
				context.EnterTotalRowMember();
				for (int j = 0; j < totals.Count; j++)
				{
					num2 = totals[j];
					tablixMember = ((ReportElementCollectionBase<TablixMember>)rowMembers)[num2];
					if (context.PageContext.CancelPage)
					{
						advanceRow = false;
						break;
					}
					context.SharedLayoutRow = 0;
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
					bool flag2 = true;
					num4 = this.CreateRowMemberChildren(tablix, tablixMember, null, num6, parentBorderHeader, num3, parentColIndex + num5, level + 1, context, ref flag2, ref levelInfo, ref ignoreTotals, parentKeepTogether || tablixMember.KeepTogether);
					if (num4 > 0)
					{
						PageMemberCell pageMemberCell = context.AddTotalRowMember(tablixMember, num3, parentColIndex, num4, num5, 0, defTreeLevel, parentLevelInfo, levelInfo);
						if (pageMemberCell != null)
						{
							if (!levelInfo.OmittedList)
							{
								pageMemberCell.Children = levelInfo.MemberCells;
							}
							parentLevelInfo.AddMemberCell(pageMemberCell, pageMemberCell.ColSpan, defTreeLevel, num2, this.m_bodyRowsHeigths[tablixMember.MemberCellIndex], context.Cache);
						}
						num3 += num4;
					}
					else if (levelInfo.IgnoredRowsCols > 0)
					{
						context.AddMemberHeaderToCurrentPage(tablixMember, TablixRegion.RowHeader, true);
						parentLevelInfo.IgnoredRowsCols += levelInfo.IgnoredRowsCols;
					}
					else
					{
						context.AddMemberHeaderToCurrentPage(tablixMember, TablixRegion.ColumnHeader, false);
					}
				}
				context.LeaveTotalRowMember(this.m_bodyRowsHeigths, ref advanceRow);
				return num3 - parentRowIndex;
			}
			return 0;
		}

		private bool RenderRowMemberInstance(TablixMember rowMember, bool addToggledItems)
		{
			if (rowMember.IsTotal)
			{
				return addToggledItems;
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
			if (visibility.ToggleItem != null && addToggledItems)
			{
				return true;
			}
			return !rowMember.Instance.Visibility.CurrentlyHidden;
		}

		private bool KeepTogetherStaticHeader(TablixMemberCollection rowMembers, TablixMember staticMember, int staticIndex, TablixContext context)
		{
			if (staticMember.KeepWithGroup != KeepWithGroup.After)
			{
				return false;
			}
			int num = this.CheckKeepWithGroupDown(rowMembers, staticIndex + 1, KeepWithGroup.After);
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
						if (num2 < rowMembers.Count)
						{
							return true;
						}
					}
				}
				bool flag = false;
				PageBreakInfo pageBreakInfo = null;
				bool flag2 = false;
				tablixDynamicMemberInstance.ResetContext();
				bool flag3 = tablixDynamicMemberInstance.MoveNext();
				while (flag3)
				{
					flag2 = false;
					if (this.RenderRowMemberInstance(tablixMember, context.AddToggledItems))
					{
						flag2 = this.DynamicWithVisibleChildren(tablixMember, context.AddToggledItems, ref pageBreakInfo);
					}
					if (flag2)
					{
						return null == pageBreakInfo;
					}
					flag3 = tablixDynamicMemberInstance.MoveNext();
				}
			}
			return false;
		}

		private double RegisterSizeForRepeatWithBefore(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember rowMemberParent, int start, int end, TablixContext context)
		{
			if (start > end)
			{
				return 0.0;
			}
			TablixMemberCollection tablixMemberCollection = null;
			tablixMemberCollection = ((rowMemberParent != null) ? rowMemberParent.Children : tablix.RowHierarchy.MemberCollection);
			if (tablixMemberCollection == null)
			{
				context.RowMemberIndexCell = rowMemberParent.MemberCellIndex;
				return context.NextRow(this.m_bodyRowsHeigths);
			}
			double num = 0.0;
			bool flag = true;
			TablixMember tablixMember = null;
			Visibility visibility = null;
			bool flag2 = false;
			bool hasRecursivePeer = false;
			bool hasVisibleStaticPeer = Tablix.CheckForVisibleStaticPeer(tablixMemberCollection);
			if (start == -1 && end == -1)
			{
				start = 0;
				end = tablixMemberCollection.Count - 1;
			}
			for (int i = start; i <= end; i++)
			{
				tablixMember = ((ReportElementCollectionBase<TablixMember>)tablixMemberCollection)[i];
				visibility = tablixMember.Visibility;
				if (visibility == null || visibility.HiddenState != 0)
				{
					if (tablixMember.Group != null)
					{
						hasRecursivePeer = tablixMember.Group.IsRecursive;
					}
					if (context.EnterRowMember(this, tablixMember, visibility, hasRecursivePeer, hasVisibleStaticPeer))
					{
						num += this.RegisterSizeForRepeatWithBefore(tablix, tablixMember, -1, -1, context);
					}
					context.LeaveRowMemberInstance(tablixMember, null, visibility, hasRecursivePeer);
					context.LeaveRowMember(tablixMember, this.m_bodyRowsHeigths, visibility, hasRecursivePeer, null, ref flag2);
				}
			}
			return num;
		}

		private bool DynamicWithVisibleChildren(TablixMember rowMemberParent, TablixContext context)
		{
			if (rowMemberParent == null)
			{
				return false;
			}
			PageBreakInfo pageBreakInfo = null;
			return this.MemberWithVisibleChildren(rowMemberParent.Children, context.AddToggledItems, ref pageBreakInfo);
		}

		private bool DynamicWithNonKeepWithVisibleChildren(TablixMember rowMemberParent, int childrenLevel, TablixContext context, ref bool pageBreak)
		{
			if (rowMemberParent == null)
			{
				return false;
			}
			if (rowMemberParent.Children == null)
			{
				if (rowMemberParent.IsStatic && rowMemberParent.KeepWithGroup != 0)
				{
					return false;
				}
				return true;
			}
			bool result = false;
			int startChild = 0;
			if (childrenLevel >= 0 && childrenLevel < this.m_tablixCreateState.Count)
			{
				startChild = this.m_tablixCreateState[childrenLevel];
			}
			this.MemberWithNonKeepWithVisibleChildren(rowMemberParent.Children, context.AddToggledItems, context.CheckPageBreaks, startChild, childrenLevel, ref result, ref pageBreak);
			return result;
		}

		private bool DynamicWithVisibleChildren(TablixMember rowMemberParent, bool addToggledItems, ref PageBreakInfo pageBreakAtStart)
		{
			if (rowMemberParent == null)
			{
				return false;
			}
			bool flag = this.MemberWithVisibleChildren(rowMemberParent.Children, addToggledItems, ref pageBreakAtStart);
			if (flag)
			{
				PageBreakInfo pageBreakInfo = Tablix.CreatePageBreakInfo(rowMemberParent.Group);
				if (pageBreakInfo != null && !pageBreakInfo.Disabled)
				{
					PageBreakLocation breakLocation = pageBreakInfo.BreakLocation;
					if (breakLocation == PageBreakLocation.Start || breakLocation == PageBreakLocation.StartAndEnd)
					{
						pageBreakAtStart = pageBreakInfo;
					}
				}
			}
			return flag;
		}

		private bool MemberWithVisibleChildren(TablixMemberCollection rowMembers, bool addToggledItems, ref PageBreakInfo pageBreakAtStart)
		{
			if (rowMembers == null)
			{
				return true;
			}
			bool flag = true;
			bool flag2 = true;
			TablixMember tablixMember = null;
			TablixMemberInstance tablixMemberInstance = null;
			TablixDynamicMemberInstance tablixDynamicMemberInstance = null;
			for (int i = 0; i < rowMembers.Count; i++)
			{
				tablixMember = ((ReportElementCollectionBase<TablixMember>)rowMembers)[i];
				if (tablixMember.Visibility == null || tablixMember.Visibility.HiddenState != 0)
				{
					flag2 = true;
					flag = true;
					tablixMemberInstance = tablixMember.Instance;
					if (tablixMember.IsStatic)
					{
						flag2 = this.RenderRowMemberInstance(tablixMember, addToggledItems);
					}
					else
					{
						if (tablixMember.Visibility != null && tablixMember.Visibility.ToggleItem != null)
						{
							int num = i + 1;
							if (num < rowMembers.Count && ((ReportElementCollectionBase<TablixMember>)rowMembers)[num].IsTotal)
							{
								return true;
							}
						}
						tablixDynamicMemberInstance = (TablixDynamicMemberInstance)tablixMemberInstance;
						tablixDynamicMemberInstance.ResetContext();
						flag = tablixDynamicMemberInstance.MoveNext();
						if (flag)
						{
							flag2 = this.RenderRowMemberInstance(tablixMember, addToggledItems);
						}
					}
					while (flag)
					{
						if (flag2 && this.DynamicWithVisibleChildren(tablixMember, addToggledItems, ref pageBreakAtStart))
						{
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
								flag2 = this.RenderRowMemberInstance(tablixMember, addToggledItems);
							}
						}
					}
					tablixDynamicMemberInstance = null;
				}
			}
			return false;
		}

		private bool MemberWithNonKeepWithVisibleChildren(TablixMemberCollection rowMembers, bool addToggledItems, bool checkPageBreaks, int startChild, int level, ref bool found, ref bool pageBreak)
		{
			if (rowMembers == null)
			{
				found = true;
				return true;
			}
			bool flag = true;
			bool flag2 = true;
			TablixMember tablixMember = null;
			TablixMemberInstance tablixMemberInstance = null;
			TablixDynamicMemberInstance tablixDynamicMemberInstance = null;
			bool result = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = checkPageBreaks;
			for (int i = startChild; i < rowMembers.Count; i++)
			{
				tablixMember = ((ReportElementCollectionBase<TablixMember>)rowMembers)[i];
				if (level >= 0)
				{
					level = -1;
					if (tablixMember.KeepWithGroup == KeepWithGroup.After)
					{
						flag3 = true;
					}
				}
				else if (tablixMember.Visibility == null || tablixMember.Visibility.HiddenState != 0)
				{
					flag2 = true;
					flag = true;
					tablixMemberInstance = tablixMember.Instance;
					if (tablixMember.IsStatic)
					{
						flag2 = this.RenderRowMemberInstance(tablixMember, addToggledItems);
					}
					else
					{
						tablixDynamicMemberInstance = (TablixDynamicMemberInstance)tablixMemberInstance;
						tablixDynamicMemberInstance.ResetContext();
						flag = tablixDynamicMemberInstance.MoveNext();
						if (flag)
						{
							flag2 = this.RenderRowMemberInstance(tablixMember, addToggledItems);
						}
					}
					flag5 = checkPageBreaks;
					if (flag5 && (tablixMember.IsTotal || (tablixMember.Visibility != null && tablixMember.Visibility.ToggleItem != null)))
					{
						flag5 = false;
					}
					while (flag)
					{
						if (flag2 && this.MemberWithNonKeepWithVisibleChildren(tablixMember.Children, addToggledItems, flag5, 0, -1, ref flag4, ref pageBreak))
						{
							if (tablixMember.IsStatic)
							{
								if (tablixMember.KeepWithGroup == KeepWithGroup.After)
								{
									flag3 = true;
									result = true;
									goto IL_016f;
								}
								if (tablixMember.KeepWithGroup == KeepWithGroup.Before)
								{
									found = true;
								}
								else
								{
									found = flag4;
								}
								return true;
							}
							if (flag5)
							{
								PageBreakInfo pageBreakInfo = Tablix.CreatePageBreakInfo(tablixMember.Group);
								if (pageBreakInfo != null && !pageBreakInfo.Disabled)
								{
									PageBreakLocation breakLocation = pageBreakInfo.BreakLocation;
									if (breakLocation != 0)
									{
										pageBreak = true;
										if (breakLocation != PageBreakLocation.Start && breakLocation != PageBreakLocation.StartAndEnd)
										{
											goto IL_0165;
										}
										return result;
									}
								}
							}
							goto IL_0165;
						}
						goto IL_016f;
						IL_016f:
						if (tablixMember.IsStatic)
						{
							flag = false;
						}
						else
						{
							flag = tablixDynamicMemberInstance.MoveNext();
							if (flag)
							{
								flag2 = this.RenderRowMemberInstance(tablixMember, addToggledItems);
							}
							else if (flag3)
							{
								found = true;
								return true;
							}
						}
						continue;
						IL_0165:
						found = flag4;
						return true;
					}
					tablixDynamicMemberInstance = null;
				}
			}
			return result;
		}

		private bool CheckAndAdvanceToNextVisibleInstance(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember rowMember, TablixDynamicMemberInstance rowDynamicInstance, TablixContext context)
		{
			bool flag = true;
			bool flag2 = false;
			Visibility visibility = rowMember.Visibility;
			if (visibility != null && visibility.HiddenState == SharedHiddenState.Sometimes && visibility.ToggleItem == null)
			{
				while (flag && rowDynamicInstance.Visibility.CurrentlyHidden)
				{
					context.AddMemberToCurrentPage(tablix, rowMember, TablixRegion.RowHeader, true);
					flag = rowDynamicInstance.MoveNext();
				}
			}
			return flag;
		}

		private bool AdvanceToNextVisibleInstance(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember rowMember, TablixDynamicMemberInstance rowDynamicInstance, TablixContext context)
		{
			bool flag = rowDynamicInstance.MoveNext();
			if (flag)
			{
				flag = this.CheckAndAdvanceToNextVisibleInstance(tablix, rowMember, rowDynamicInstance, context);
			}
			return flag;
		}

		private int AdvanceInState(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember rowMemberParent, int level, TablixContext context, out int ignoredRows, ref int staticHeaderLevel, List<bool> ignoreTotals)
		{
			ignoredRows = 0;
			TablixMemberCollection tablixMemberCollection = null;
			tablixMemberCollection = ((rowMemberParent != null) ? rowMemberParent.Children : tablix.RowHierarchy.MemberCollection);
			if (tablixMemberCollection == null)
			{
				context.RowMemberIndexCell = rowMemberParent.MemberCellIndex;
				if (context.IgnoreRow)
				{
					context.AddDetailRowToCurrentPage(tablix);
					ignoredRows++;
					context.NextRow(this.m_bodyRowsHeigths);
				}
				if (!context.IgnoreRow)
				{
					return 1;
				}
				return 0;
			}
			int num = 0;
			bool flag = true;
			bool flag2 = true;
			int num2 = 0;
			bool flag3 = true;
			int num3 = 0;
			bool flag4 = false;
			bool hasRecursivePeer = false;
			TablixMember tablixMember = null;
			Visibility visibility = null;
			TablixMemberInstance tablixMemberInstance = null;
			TablixDynamicMemberInstance tablixDynamicMemberInstance = null;
			bool hasVisibleStaticPeer = Tablix.CheckForVisibleStaticPeer(tablixMemberCollection);
			if (this.m_tablixCreateState.Count <= level)
			{
				this.m_tablixCreateState.Add(num2);
				ignoreTotals.Add(false);
			}
			else
			{
				flag3 = false;
				num2 = this.m_tablixCreateState[level];
			}
			for (int i = num2; i < tablixMemberCollection.Count; i++)
			{
				tablixMember = ((ReportElementCollectionBase<TablixMember>)tablixMemberCollection)[i];
				visibility = tablixMember.Visibility;
				if (context.AlwaysHiddenMember(tablix, tablixMember, visibility, TablixRegion.RowHeader, true, ref ignoredRows))
				{
					if (this.m_levelForRepeat >= level)
					{
						this.m_levelForRepeat = level - 1;
					}
					if (this.m_partialMemberLevel >= level)
					{
						this.m_partialMemberLevel = level - 1;
					}
				}
				else
				{
					if (i > num2)
					{
						if (this.m_levelForRepeat >= level)
						{
							this.m_levelForRepeat = level - 1;
						}
						if (this.m_partialMemberLevel >= level)
						{
							this.m_partialMemberLevel = level - 1;
						}
					}
					flag2 = true;
					flag = true;
					tablixMemberInstance = tablixMember.Instance;
					if (tablixMember.Group != null)
					{
						hasRecursivePeer = tablixMember.Group.IsRecursive;
					}
					if (tablixMember.IsStatic)
					{
						flag2 = context.EnterRowMember(this, tablixMember, visibility, hasRecursivePeer, hasVisibleStaticPeer);
						if (staticHeaderLevel < 0 && flag2 && tablixMember.KeepWithGroup != 0 && (tablixMember.KeepWithGroup != KeepWithGroup.Before || i == num2))
						{
							staticHeaderLevel = level;
						}
					}
					else
					{
						if (!flag3 && num2 == i)
						{
							int num4 = this.CheckKeepWithGroupUp(tablixMemberCollection, num2 - 1, KeepWithGroup.After);
							if (num4 < num2 && ((ReportElementCollectionBase<TablixMember>)tablixMemberCollection)[num4].RepeatOnNewPage)
							{
								this.m_levelForRepeat = level;
							}
						}
						if (!ignoreTotals[level] && visibility != null && visibility.ToggleItem != null)
						{
							int num5 = i + 1;
							if (num5 < tablixMemberCollection.Count && ((ReportElementCollectionBase<TablixMember>)tablixMemberCollection)[num5].IsTotal)
							{
								if (i == 0)
								{
									this.m_tablixCreateState.RemoveAt(level);
									ignoreTotals.RemoveAt(level);
								}
								else
								{
									this.m_tablixCreateState[level] = -i;
								}
								return 1;
							}
						}
						tablixDynamicMemberInstance = (TablixDynamicMemberInstance)tablixMemberInstance;
						if (flag3 || i > num2)
						{
							tablixDynamicMemberInstance.ResetContext();
							flag = tablixDynamicMemberInstance.MoveNext();
						}
						if (flag)
						{
							flag2 = context.EnterRowMember(null, tablixMember, visibility, hasRecursivePeer, false);
						}
					}
					while (flag)
					{
						if (flag2)
						{
							num = this.AdvanceInState(tablix, tablixMember, level + 1, context, out num3, ref staticHeaderLevel, ignoreTotals);
							if (num > 0)
							{
								context.LeaveRowMemberInstance(tablixMember, null, visibility, hasRecursivePeer);
								context.LeaveRowMember(tablixMember, this.m_bodyRowsHeigths, visibility, hasRecursivePeer, null, ref flag4);
								this.m_tablixCreateState[level] = i;
								ignoreTotals[level] = true;
								return 1;
							}
							if (num3 > 0)
							{
								context.AddMemberHeaderToCurrentPage(tablixMember, TablixRegion.RowHeader, true);
								ignoredRows += num3;
							}
							else
							{
								context.AddMemberHeaderToCurrentPage(tablixMember, TablixRegion.RowHeader, false);
							}
						}
						else
						{
							context.AddMemberToCurrentPage(tablix, tablixMember, TablixRegion.RowHeader, true);
						}
						context.LeaveRowMemberInstance(tablixMember, null, visibility, hasRecursivePeer);
						if (tablixMember.IsStatic)
						{
							flag = false;
							context.LeaveRowMember(tablixMember, this.m_bodyRowsHeigths, visibility, hasRecursivePeer, null, ref flag4);
							if (staticHeaderLevel == level)
							{
								staticHeaderLevel = -1;
							}
						}
						else
						{
							flag = tablixDynamicMemberInstance.MoveNext();
							if (flag)
							{
								flag2 = context.EnterRowMemberInstance(tablixMember, visibility, hasRecursivePeer);
							}
							else
							{
								context.LeaveRowMember(tablixMember, this.m_bodyRowsHeigths, visibility, hasRecursivePeer, null, ref flag4);
							}
						}
						if (this.m_partialMemberLevel >= level)
						{
							this.m_partialMemberLevel = level - 1;
						}
					}
					tablixDynamicMemberInstance = null;
				}
			}
			this.m_tablixCreateState.RemoveAt(level);
			ignoreTotals.RemoveAt(level);
			if (this.m_levelForRepeat >= level)
			{
				this.m_levelForRepeat = level - 1;
			}
			if (this.m_partialMemberLevel >= level)
			{
				this.m_partialMemberLevel = level - 1;
			}
			return 0;
		}

		private List<int> GetTablixMembersInstanceIndex()
		{
			if (this.m_tablixCreateState != null && this.m_tablixCreateState.Count != 0)
			{
				List<int> list = new List<int>(this.m_tablixCreateState.Count);
				AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix = (AspNetCore.ReportingServices.OnDemandReportRendering.Tablix)base.m_source;
				this.GetTablixMembersInstanceIndex(tablix.RowHierarchy.MemberCollection, 0, list);
				return list;
			}
			return null;
		}

		private void GetTablixMembersInstanceIndex(TablixMemberCollection rowMembers, int level, List<int> instanceState)
		{
			if (rowMembers != null && this.m_tablixCreateState.Count > level)
			{
				int i = this.m_tablixCreateState[level];
				TablixMember tablixMember = ((ReportElementCollectionBase<TablixMember>)rowMembers)[i];
				if (tablixMember.IsStatic)
				{
					instanceState.Add(0);
				}
				else
				{
					TablixDynamicMemberInstance tablixDynamicMemberInstance = (TablixDynamicMemberInstance)tablixMember.Instance;
					instanceState.Add(tablixDynamicMemberInstance.GetInstanceIndex());
				}
				this.GetTablixMembersInstanceIndex(tablixMember.Children, level + 1, instanceState);
			}
		}

		private void SetTablixMembersInstanceIndex(List<int> instanceState)
		{
			if (this.m_tablixCreateState != null && this.m_tablixCreateState.Count != 0)
			{
				AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix = (AspNetCore.ReportingServices.OnDemandReportRendering.Tablix)base.m_source;
				this.SetTablixMembersInstanceIndex(tablix.RowHierarchy.MemberCollection, 0, instanceState);
			}
		}

		private void SetTablixMembersInstanceIndex(TablixMemberCollection rowMembers, int level, List<int> instanceState)
		{
			if (rowMembers != null && this.m_tablixCreateState.Count > level)
			{
				int i = this.m_tablixCreateState[level];
				TablixMember tablixMember = ((ReportElementCollectionBase<TablixMember>)rowMembers)[i];
				if (!tablixMember.IsStatic)
				{
					TablixDynamicMemberInstance tablixDynamicMemberInstance = (TablixDynamicMemberInstance)tablixMember.Instance;
					tablixDynamicMemberInstance.SetInstanceIndex(instanceState[level]);
				}
				this.SetTablixMembersInstanceIndex(tablixMember.Children, level + 1, instanceState);
			}
		}

		private void CreateTablixItems(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixContext context)
		{
			bool flag = true;
			LevelInfo levelInfo = null;
			bool flag2 = true;
			List<bool> list = null;
			int staticHeaderLevel = -1;
			if (this.m_tablixCreateState != null)
			{
				this.m_levelForRepeat = -1;
				list = new List<bool>(this.m_tablixCreateState.Count);
				for (int i = 0; i < this.m_tablixCreateState.Count - 1; i++)
				{
					list.Add(true);
				}
				list.Add(this.m_ignoreTotalsOnLastLevel);
				this.m_partialMemberLevel = this.m_tablixCreateState.Count - 2;
				int num = 0;
				this.AdvanceInState(tablix, (TablixMember)null, 0, context, out num, ref staticHeaderLevel, list);
				if (this.m_tablixCreateState.Count == 0)
				{
					flag2 = false;
				}
				else
				{
					this.CheckForOnlyStaticHeaders(tablix, staticHeaderLevel, context);
				}
			}
			if (flag2)
			{
				if (this.m_hasDetailCellsWithColSpan && context.HeaderColumnRows > 0 && !context.ColumnHeadersCreated && context.Writer != null)
				{
					this.CreateColumnMemberChildren(tablix, null, this.m_colMembersDepth, false, 0, 0, context, false, ref levelInfo);
					context.OuterColumnHeaders = levelInfo.MemberCells;
					context.OmittedOuterColumnHeaders = levelInfo.OmittedMembersCells;
					context.ColumnHeadersCreated = true;
				}
				this.CreateRowMemberChildren(tablix, null, this.m_toggleMemberState, this.m_rowMembersDepth, false, 0, 0, 0, context, ref flag, ref levelInfo, ref list, tablix.KeepTogether);
				context.OuterRowHeaders = levelInfo.MemberCells;
				this.m_ignoreTotalsOnLastLevel = false;
				if (this.m_tablixCreateState.Count > 0)
				{
					this.m_ignoreTotalsOnLastLevel = list[list.Count - 1];
				}
			}
			if (!context.PageContext.CancelPage && context.OuterRowHeaders == null && context.HeaderColumnRows > 0)
			{
				if (context.Writer == null && context.Interactivity == null)
				{
					return;
				}
				this.CreateColumnMemberChildren(tablix, null, this.m_colMembersDepth, false, 0, 0, context, false, ref levelInfo);
				if (!context.ColumnHeadersCreated)
				{
					context.OuterColumnHeaders = levelInfo.MemberCells;
					context.OmittedOuterColumnHeaders = levelInfo.OmittedMembersCells;
					context.ColumnHeadersCreated = true;
				}
			}
		}

		private void CheckForOnlyStaticHeaders(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix, int staticHeaderLevel, TablixContext context)
		{
			if (this.m_tablixCreateState != null && this.m_tablixCreateState.Count != 0 && this.m_levelForRepeat >= 0 && staticHeaderLevel > this.m_levelForRepeat)
			{
				TablixMemberCollection tablixMemberCollection = tablix.RowHierarchy.MemberCollection;
				TablixMember tablixMember = null;
				this.m_levelForRepeat = staticHeaderLevel - 1;
				int i;
				for (i = 0; i < staticHeaderLevel; i++)
				{
					if (this.m_tablixCreateState[i] < 0)
					{
						return;
					}
					if (tablixMemberCollection == null)
					{
						break;
					}
					tablixMember = ((ReportElementCollectionBase<TablixMember>)tablixMemberCollection)[this.m_tablixCreateState[i]];
					tablixMemberCollection = tablixMember.Children;
				}
				bool flag = false;
				bool flag2 = this.DynamicWithNonKeepWithVisibleChildren(tablixMember, i, context, ref flag);
				bool flag3 = false;
				List<int> tablixMembersInstanceIndex = null;
				while (!flag2 && !flag)
				{
					TablixMember parent = tablixMember.Parent;
					if (parent == null)
					{
						break;
					}
					if (tablixMember.IsStatic)
					{
						this.m_levelForRepeat--;
						flag2 = this.MemberWithNonKeepWithVisibleChildren(parent.Children, context.AddToggledItems, context.CheckPageBreaks, this.m_tablixCreateState[i - 1] + 1, -1, ref flag2, ref flag);
						if (flag2)
						{
							break;
						}
						if (flag)
						{
							break;
						}
					}
					else
					{
						if (context.CheckPageBreaks)
						{
							PageBreakInfo pageBreakInfo = Tablix.CreatePageBreakInfo(tablixMember.Group);
							if (pageBreakInfo != null && !pageBreakInfo.Disabled && pageBreakInfo.BreakLocation != 0)
							{
								break;
							}
						}
						TablixMemberInstance instance = tablixMember.Instance;
						TablixDynamicMemberInstance tablixDynamicMemberInstance = (TablixDynamicMemberInstance)instance;
						if (!flag3)
						{
							tablixMembersInstanceIndex = this.GetTablixMembersInstanceIndex();
							flag3 = true;
						}
						bool flag4 = tablixDynamicMemberInstance.MoveNext();
						while (flag4 && !flag2)
						{
							if (context.EnterRowMemberInstance(tablixMember, tablixMember.Visibility, false))
							{
								flag2 = this.DynamicWithNonKeepWithVisibleChildren(tablixMember, -1, context, ref flag);
							}
							context.LeaveRowMemberInstance(tablixMember, null, tablixMember.Visibility, false);
							if (!flag2 && !flag)
							{
								flag4 = tablixDynamicMemberInstance.MoveNext();
							}
						}
						if (flag2)
						{
							break;
						}
						this.m_levelForRepeat--;
						flag2 = this.MemberWithNonKeepWithVisibleChildren(parent.Children, context.AddToggledItems, context.CheckPageBreaks, this.m_tablixCreateState[i - 1] + 1, -1, ref flag2, ref flag);
						if (flag2)
						{
							break;
						}
						if (flag)
						{
							break;
						}
					}
					i--;
					tablixMember = tablixMember.Parent;
				}
				if (!flag2)
				{
					this.m_levelForRepeat = -1;
				}
				if (flag3)
				{
					this.SetTablixMembersInstanceIndex(tablixMembersInstanceIndex);
				}
			}
		}

		private static bool CheckForVisibleStaticPeer(TablixMemberCollection members)
		{
			foreach (TablixMember member in members)
			{
				if (member.IsStatic && !member.IsTotal)
				{
					Visibility visibility = member.Visibility;
					if (visibility != null && visibility.HiddenState != SharedHiddenState.Never)
					{
						if (visibility.ToggleItem != null)
						{
							return true;
						}
						if (!member.Instance.Visibility.CurrentlyHidden)
						{
							return true;
						}
						continue;
					}
					return true;
				}
			}
			return false;
		}

		private int[] DefDetailRowsCapacity(TablixRowCollection defDetailRows)
		{
			int[] array = new int[defDetailRows.Count];
			for (int i = 0; i < defDetailRows.Count; i++)
			{
				for (int j = 0; j < ((ReportElementCollectionBase<TablixRow>)defDetailRows)[i].Count; j++)
				{
					if (((ReportElementCollectionBase<TablixCell>)((ReportElementCollectionBase<TablixRow>)defDetailRows)[i])[j] != null)
					{
						array[i]++;
					}
				}
				if (array[i] < 4)
				{
					array[i] = 4;
				}
			}
			return array;
		}

		internal override bool CalculatePage(RPLWriter rplWriter, PageItemHelper lastPageInfo, PageContext pageContext, PageItem[] siblings, RepeatWithItem[] repeatWithItems, double parentTopInPage, ref double parentPageHeight, Interactivity interactivity)
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix = (AspNetCore.ReportingServices.OnDemandReportRendering.Tablix)base.m_source;
			double num = parentTopInPage + base.m_itemPageSizes.Top;
			int num2 = 0;
			ItemSizes itemSizes = null;
			int[] array = null;
			PageItemHelper pageItemHelper = null;
			bool flag = false;
			if (lastPageInfo != null)
			{
				pageItemHelper = lastPageInfo.ChildPage;
			}
			if (this.m_partialPageItem != null || pageItemHelper != null)
			{
				num2 = 0;
				array = this.DefDetailRowsCapacity(this.m_bodyRows);
				if (tablix.OmitBorderOnPageBreak)
				{
					base.m_rplItemState |= 1;
				}
			}
			else if (this.m_tablixCreateState == null)
			{
				num2 = tablix.Columns;
				flag = true;
				base.AdjustOriginFromItemsAbove(siblings, repeatWithItems);
				if (!this.HitsCurrentPage(pageContext, parentTopInPage))
				{
					return false;
				}
				if (base.ResolveItemHiddenState(rplWriter, interactivity, pageContext, false, ref itemSizes))
				{
					parentPageHeight = Math.Max(parentPageHeight, base.m_itemPageSizes.Bottom);
					if (rplWriter != null && base.m_itemRenderSizes == null)
					{
						this.CreateItemRenderSizes(null, pageContext, false);
					}
					return true;
				}
				num = parentTopInPage + base.m_itemPageSizes.Top;
				if (num2 > 0)
				{
					num += this.CornerSize(tablix.ColumnHierarchy.MemberCollection);
				}
				else
				{
					if (!pageContext.IgnorePageBreaks && !base.PageBreakAtStart && (RoundedDouble)num > 0.0)
					{
						PageBreakInfo pageBreakInfo = null;
						this.MemberWithVisibleChildren(tablix.RowHierarchy.MemberCollection, pageContext.AddToggledItems, ref pageBreakInfo);
						if (pageBreakInfo != null)
						{
							base.ItemState = State.TopNextPage;
							pageContext.RegisterPageBreak(pageBreakInfo);
							return false;
						}
					}
					if (pageContext.TracingEnabled && pageContext.IgnorePageBreaks)
					{
						base.TracePageBreakAtStartIgnored(pageContext);
					}
				}
				array = this.DefDetailRowsCapacity(this.m_bodyRows);
				if (!pageContext.IgnorePageBreaks)
				{
					pageContext.RegisterPageName(this.PageName);
				}
			}
			else
			{
				if (tablix.OmitBorderOnPageBreak)
				{
					base.m_rplItemState |= 1;
				}
				if (tablix.RepeatColumnHeaders)
				{
					num2 = tablix.Columns;
				}
				if (num2 > 0)
				{
					double num3 = this.CornerSize(tablix.ColumnHierarchy.MemberCollection);
					if (num3 >= pageContext.PageHeight)
					{
						num3 = 0.0;
					}
					num += num3;
				}
				array = this.DefDetailRowsCapacity(this.m_bodyRows);
			}
			this.WriteStartItemToStream(rplWriter, pageContext);
			TablixInstance tablixInstance = (TablixInstance)tablix.Instance;
			PageContext pageContext2 = pageContext;
			if (!pageContext2.FullOnPage)
			{
				if (base.IgnorePageBreaks || tablixInstance.NoRows)
				{
					pageContext2 = new PageContext(pageContext, PageContext.PageContextFlags.FullOnPage, PageContext.IgnorePBReasonFlag.Toggled);
				}
				else if (flag && tablix.KeepTogether && !pageContext2.KeepTogether)
				{
					pageContext2 = new PageContext(pageContext);
					pageContext2.KeepTogether = true;
					if (pageContext.TracingEnabled && parentTopInPage + base.m_itemPageSizes.Height >= pageContext2.OriginalPageHeight)
					{
						base.TracePageGrownOnKeepTogetherItem(pageContext.PageNumber);
					}
				}
			}
			TablixContext tablixContext = null;
			tablixContext = ((base.m_rplElement != null) ? ((TablixContext)new RPLContext(rplWriter, pageItemHelper, tablixInstance.NoRows, tablix.LayoutDirection == TablixLayoutDirection.LTR, pageContext2, num, tablix.Rows, num2, array, interactivity, (RPLTablix)base.m_rplElement)) : ((TablixContext)new StreamContext(rplWriter, pageItemHelper, tablixInstance.NoRows, tablix.LayoutDirection == TablixLayoutDirection.LTR, pageContext2, num, tablix.Rows, num2, array, interactivity)));
			tablixContext.PageBreakNeedsOverride = !pageContext.IsPageBreakRegistered;
			this.CreateTablixItems(tablix, tablixContext);
			if (tablixContext.PageContext.CancelPage)
			{
				base.m_itemState = State.Below;
				base.m_rplElement = null;
			}
			if (this.m_partialPageItem == null && this.m_tablixCreateState.Count == 0)
			{
				this.m_tablixCreateState = null;
				this.m_partialPageItem = null;
				base.m_itemState = State.OnPage;
				if (!pageContext2.IgnorePageBreaks && (base.PageBreakAtEnd || tablixContext.PageBreakAtEnd || tablixContext.PropagatedPageBreak))
				{
					base.m_itemState = State.OnPagePBEnd;
					if (base.PageBreakAtEnd)
					{
						pageContext.RegisterPageBreak(new PageBreakInfo(this.PageBreak, base.ItemName), tablixContext.PageBreakNeedsOverride);
					}
				}
				if (pageContext.TracingEnabled && pageContext2.IgnorePageBreaks && base.PageBreakAtEnd)
				{
					base.TracePageBreakAtEndIgnored(pageContext2);
				}
			}
			else
			{
				base.m_itemState = State.SpanPages;
				if (tablix.OmitBorderOnPageBreak)
				{
					base.m_rplItemState |= 2;
				}
			}
			if (itemSizes == null)
			{
				base.m_itemPageSizes.AdjustHeightTo(tablixContext.TablixBottom - parentTopInPage - base.m_itemPageSizes.Top);
			}
			parentPageHeight = Math.Max(parentPageHeight, base.m_itemPageSizes.Bottom);
			this.WriteEndItemToStream(rplWriter, tablixContext, tablix, itemSizes);
			return true;
		}

		internal void WriteStartItemToStream(RPLWriter rplWriter, PageContext pageContext)
		{
			if (rplWriter != null)
			{
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				if (binaryWriter != null)
				{
					Stream baseStream = binaryWriter.BaseStream;
					base.m_offset = baseStream.Position;
					binaryWriter.Write((byte)13);
					this.WriteElementProps(binaryWriter, rplWriter, pageContext, base.m_offset + 1);
				}
				else
				{
					base.m_rplElement = new RPLTablix();
					this.WriteElementProps(base.m_rplElement.ElementProps, rplWriter, pageContext);
				}
			}
		}

		internal void WriteEndItemToStream(RPLWriter rplWriter, TablixContext context, AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix, ItemSizes contentSize)
		{
			if (rplWriter != null)
			{
				double amount = 0.0;
				double amount2 = 0.0;
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				if (binaryWriter != null)
				{
					Stream baseStream = binaryWriter.BaseStream;
					long position = baseStream.Position;
					binaryWriter.Write((byte)17);
					binaryWriter.Write(base.m_offset);
					context.WriteTablixMeasurements(tablix, this.m_rowMembersDepth, this.m_colMembersDepth, ref amount, ref amount2);
					binaryWriter.Write((byte)255);
					binaryWriter.Flush();
					base.m_offset = baseStream.Position;
					binaryWriter.Write((byte)254);
					binaryWriter.Write(position);
					binaryWriter.Write((byte)255);
				}
				else
				{
					context.WriteTablixMeasurements(tablix, this.m_rowMembersDepth, this.m_colMembersDepth, ref amount, ref amount2);
				}
				this.CreateItemRenderSizes(contentSize, context.PageContext, false);
				base.m_itemRenderSizes.AdjustHeightTo(amount2);
				base.m_itemRenderSizes.AdjustWidthTo(amount);
			}
		}

		internal override void WriteItemSharedStyleProps(BinaryWriter spbifWriter, Style style, PageContext pageContext)
		{
			base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.BackgroundColor, 34);
			this.WriteBackgroundImage(spbifWriter, style, true, pageContext);
		}

		internal override void WriteItemSharedStyleProps(RPLStyleProps rplStyleProps, Style style, PageContext pageContext)
		{
			base.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.BackgroundColor, 34);
			this.WriteBackgroundImage(rplStyleProps, style, true, pageContext);
		}

		internal override void WriteItemNonSharedStyleProp(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, StyleAttributeNames styleAtt, PageContext pageContext)
		{
			switch (styleAtt)
			{
			case StyleAttributeNames.BackgroundColor:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BackgroundColor, 34);
				break;
			case StyleAttributeNames.BackgroundImage:
				this.WriteBackgroundImage(spbifWriter, styleDef, false, pageContext);
				break;
			}
		}

		internal override void WriteItemNonSharedStyleProp(RPLStyleProps rplStyleProps, Style styleDef, StyleInstance style, StyleAttributeNames styleAtt, PageContext pageContext)
		{
			switch (styleAtt)
			{
			case StyleAttributeNames.BackgroundColor:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BackgroundColor, 34);
				break;
			case StyleAttributeNames.BackgroundImage:
				this.WriteBackgroundImage(rplStyleProps, styleDef, false, pageContext);
				break;
			}
		}

		internal override void WritePaginationInfo(BinaryWriter reportPageInfo)
		{
			if (reportPageInfo != null)
			{
				reportPageInfo.Write((byte)11);
				base.WritePaginationInfoProperties(reportPageInfo);
				reportPageInfo.Write((byte)16);
				reportPageInfo.Write(this.m_levelForRepeat);
				reportPageInfo.Write((byte)22);
				reportPageInfo.Write(this.m_ignoreTotalsOnLastLevel);
				if (this.m_tablixCreateState != null && this.m_tablixCreateState.Count > 0)
				{
					reportPageInfo.Write((byte)17);
					reportPageInfo.Write(this.m_tablixCreateState.Count);
					for (int i = 0; i < this.m_tablixCreateState.Count; i++)
					{
						reportPageInfo.Write(this.m_tablixCreateState[i]);
					}
				}
				List<int> tablixMembersInstanceIndex = this.GetTablixMembersInstanceIndex();
				if (tablixMembersInstanceIndex != null && tablixMembersInstanceIndex.Count > 0)
				{
					reportPageInfo.Write((byte)18);
					reportPageInfo.Write(tablixMembersInstanceIndex.Count);
					for (int j = 0; j < tablixMembersInstanceIndex.Count; j++)
					{
						reportPageInfo.Write(tablixMembersInstanceIndex[j]);
					}
				}
				if (this.m_partialPageItem != null)
				{
					reportPageInfo.Write((byte)19);
					this.m_partialPageItem.WritePaginationInfo(reportPageInfo);
				}
				reportPageInfo.Write((byte)255);
			}
		}

		internal override PageItemHelper WritePaginationInfo()
		{
			PageTablixHelper pageTablixHelper = new PageTablixHelper(11);
			base.WritePaginationInfoProperties(pageTablixHelper);
			pageTablixHelper.LevelForRepeat = this.m_levelForRepeat;
			pageTablixHelper.IgnoreTotalsOnLastLevel = this.m_ignoreTotalsOnLastLevel;
			pageTablixHelper.TablixCreateState = PageItem.GetNewList(this.m_tablixCreateState);
			List<int> tablixMembersInstanceIndex = this.GetTablixMembersInstanceIndex();
			pageTablixHelper.MembersInstanceIndex = PageItem.GetNewList(tablixMembersInstanceIndex);
			if (this.m_partialPageItem != null)
			{
				pageTablixHelper.ChildPage = this.m_partialPageItem.WritePaginationInfo();
			}
			return pageTablixHelper;
		}

		private static PageBreakInfo CreatePageBreakInfo(Group group)
		{
			PageBreakInfo result = null;
			if (group != null)
			{
				result = new PageBreakInfo(group.PageBreak, group.Name);
			}
			return result;
		}

		internal string BuildTablixMemberPath(TablixMember rowMember)
		{
			return DiagnosticsUtilities.BuildTablixMemberPath(base.ItemName, rowMember, this.m_memberIndexByLevel);
		}

		private void TracePageGrownOnKeepTogetherMember(int pageNumber, TablixMember rowMember)
		{
			RenderingDiagnostics.Trace(RenderingArea.KeepTogether, TraceLevel.Verbose, "PR-DIAG [Page {0}] Member '{1}' kept together - RowContext - Page grown", pageNumber, DiagnosticsUtilities.BuildTablixMemberPath(base.ItemName, rowMember, this.m_memberIndexByLevel));
		}

		private void TracePageGrownOnKeepWithMember(int pageNumber)
		{
			if (this.m_outermostKeepWithMemberPath != null)
			{
				RenderingDiagnostics.Trace(RenderingArea.KeepTogether, TraceLevel.Verbose, "PR-DIAG [Page {0}] Member '{1}' kept together - KeepWith - Page grown", pageNumber, this.m_outermostKeepWithMemberPath);
			}
		}

		private void TraceRepeatOnNewPage(int pageNumber, TablixMember rowMember)
		{
			RenderingDiagnostics.Trace(RenderingArea.RepeatOnNewPage, TraceLevel.Verbose, "PR-DIAG [Page {0}] '{1}' appears on page due to RepeatOnNewPage", pageNumber, DiagnosticsUtilities.BuildTablixMemberPath(base.ItemName, rowMember, this.m_memberIndexByLevel));
		}

		private void RegisterTablixMemberIndex(string definitionPath, int index)
		{
			if (definitionPath != null && !this.m_memberIndexByLevel.Contains(definitionPath))
			{
				this.m_memberIndexByLevel.Add(definitionPath, index);
			}
		}

		private bool RegisterOutermostKeepWithMember(TablixMember tablixMember)
		{
			if (this.m_outermostKeepWithMemberPath == null && tablixMember != null)
			{
				this.m_outermostKeepWithMemberPath = this.BuildTablixMemberPath(tablixMember);
				return true;
			}
			return false;
		}

		private void UnregisterOutermostKeepWithMember()
		{
			this.m_outermostKeepWithMemberPath = null;
		}

		private bool RegisterOutermostRepeatOnNewPage()
		{
			if (!this.m_repeatOnNewPageRegistered)
			{
				this.m_repeatOnNewPageRegistered = true;
				return true;
			}
			return false;
		}

		private void UnregisterRepeatOnNewPagePath()
		{
			this.m_repeatOnNewPageRegistered = false;
		}
	}
}
