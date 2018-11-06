using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.Interfaces;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.HtmlRenderer;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using AspNetCore.ReportingServices.Rendering.SPBProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer
{
	internal abstract class WordRenderer
	{
		private delegate void RenderSize(float points);

		private class SplitTablix
		{
			private SplitTablixRow[] m_rows;

			internal SplitTablixRow this[int index]
			{
				get
				{
					return this.m_rows[index];
				}
				set
				{
					this.m_rows[index] = value;
				}
			}

			internal SplitTablix(int numRows)
			{
				this.m_rows = new SplitTablixRow[numRows];
			}
		}

		private class SplitTablixRow
		{
			internal TablixGhostCell FirstCell;

			internal List<RPLTablixCell> Cells = new List<RPLTablixCell>();
		}

		protected class TablixGhostCell
		{
			internal int RowSpan;

			internal BorderContext Context;

			internal RPLTablixCell Cell;

			internal int ColSpan;
		}

		internal const int MaximumListLevel = 9;

		protected AspNetCore.ReportingServices.Rendering.SPBProcessing.SPBProcessing m_spbProcessing;

		protected IWordWriter m_writer;

		private int m_labelLevel;

		protected bool m_inHeaderFooter;

		protected RPLReport m_rplReport;

		protected DeviceInfo m_deviceInfo;

		private bool m_omitHyperlinks;

		private bool m_omitDrillthroughs;

		protected float m_pageHeight;

		protected bool m_needsToResetTextboxes;

		internal WordRenderer(CreateAndRegisterStream createAndRegisterStream, AspNetCore.ReportingServices.Rendering.SPBProcessing.SPBProcessing spbProcessing, IWordWriter writer, DeviceInfo deviceInfo, string reportName)
		{
			this.m_spbProcessing = spbProcessing;
			writer.Init(createAndRegisterStream, deviceInfo.AutoFit, reportName);
			this.m_writer = writer;
			this.m_omitHyperlinks = deviceInfo.OmitHyperlinks;
			this.m_omitDrillthroughs = deviceInfo.OmitDrillthroughs;
			this.m_deviceInfo = deviceInfo;
		}

		internal abstract bool Render();

		protected abstract void RenderTablixCell(RPLTablix tablix, float left, float[] widths, TablixGhostCell[] ghostCells, BorderContext borderContext, int nextCell, RPLTablixCell cell, List<RPLTablixMemberCell>.Enumerator omittedCells, bool lastCell);

		protected abstract void RenderTextBox(RPLTextBox textBox, RPLItemMeasurement measurement, int cellIndex, float left, BorderContext borderContext, bool inTablix, bool hasBorder);

		protected abstract bool RenderRectangleItemAndLines(RPLContainer rectangle, BorderContext borderContext, int y, PageTableCell cell, string linkToChildId, float runningLeft, bool rowUsed);

		private void RenderReportItem(RPLElement element, RPLItemMeasurement measurement, int cellIndex, float left, BorderContext borderContext, bool inTablix)
		{
			if (element != null)
			{
				this.RenderBookmarksLabels(element);
				bool flag = !inTablix && this.HasAnyBorder(element.ElementProps.Style);
				if (element is RPLTextBox)
				{
					this.RenderTextBox(element as RPLTextBox, measurement, cellIndex, left, borderContext, inTablix, flag);
				}
				else if (element is RPLLine)
				{
					RPLElementStyle style = element.ElementProps.Style;
					string color = (string)style[0];
					string width = (string)style[10];
					RPLFormat.BorderStyles style2 = (RPLFormat.BorderStyles)style[5];
					bool slant = ((RPLLinePropsDef)element.ElementProps.Definition).Slant;
					this.m_writer.WriteCellDiagonal(cellIndex, style2, width, color, slant);
					this.m_writer.WriteEmptyStyle();
				}
				else if (element is RPLTablix)
				{
					if (cellIndex != -1 && inTablix)
					{
						this.RenderCellProperties(element.ElementProps.Style, cellIndex, inTablix);
						this.SetZeroPadding(cellIndex);
					}
					RPLTablix rPLTablix = (RPLTablix)element;
					if (rPLTablix.ColumnWidths != null)
					{
						int num = -1;
						float num2 = left + measurement.Width;
						if (num2 > 558.79998779296875)
						{
							num2 = left;
							for (int i = 0; i < rPLTablix.ColumnWidths.Length; i++)
							{
								num2 += rPLTablix.ColumnWidths[i];
								if (num2 > 558.79998779296875)
								{
									num = i - 1;
									break;
								}
							}
						}
						else if (rPLTablix.ColumnWidths.Length > 63)
						{
							num = 63;
						}
						if (num > -1)
						{
							this.WriteBeginTableRowCell(measurement, false);
							this.RenderCellProperties(element.ElementProps.Style, 0, true);
							this.SetZeroPadding(0);
							this.m_writer.ApplyCellBorderContext(borderContext);
							this.RenderSplitTablix(rPLTablix, measurement, num, borderContext);
							this.WriteEndCellRowTable(borderContext);
						}
						else
						{
							this.RenderTablix(rPLTablix, 0f, borderContext, inTablix);
						}
					}
				}
				else if (element is RPLSubReport)
				{
					this.RenderSubreport(element, measurement, cellIndex, borderContext, inTablix, flag);
				}
				else if (element is RPLImage)
				{
					RPLImage rPLImage = (RPLImage)element;
					RPLImageProps rPLImageProps = (RPLImageProps)rPLImage.ElementProps;
					RPLImagePropsDef rPLImagePropsDef = (RPLImagePropsDef)rPLImageProps.Definition;
					RPLFormat.Sizings sizing = rPLImagePropsDef.Sizing;
					bool flag2 = sizing == RPLFormat.Sizings.AutoSize && this.m_writer.AutoFit == AutoFit.True && !inTablix;
					flag = (flag || flag2);
					if (flag)
					{
						this.m_writer.WriteTableBegin(0f, true);
						float num3 = measurement.Width;
						if (flag2)
						{
							num3 = 1f;
						}
						this.m_writer.WriteTableRowBegin(0f, measurement.Height, new float[1]
						{
							num3
						});
						this.m_writer.WriteTableCellBegin(0, 1, false, false, false, false);
						this.m_writer.ApplyCellBorderContext(borderContext);
						cellIndex = 0;
					}
					if (cellIndex != -1)
					{
						this.RenderCellStyle(element.ElementProps.Style, cellIndex);
					}
					byte[] array = this.CreateImageBuf(rPLImageProps.Image);
					this.RemovePaddingFromMeasurement(measurement, element.ElementProps.Style);
					bool flag3 = this.HasAction(rPLImageProps.ActionInfo);
					if (flag3)
					{
						if (sizing == RPLFormat.Sizings.AutoSize && array != null)
						{
							this.m_writer.IgnoreRowHeight(true);
						}
						this.RenderAction(rPLImageProps.ActionInfo.Actions[0]);
					}
					this.m_writer.AddImage(array, measurement.Height, measurement.Width, sizing);
					if (flag3)
					{
						this.m_writer.WriteHyperlinkEnd();
					}
					if (flag)
					{
						this.WriteEndCellRowTable(borderContext);
					}
				}
				else if (element is RPLChart || element is RPLGaugePanel || element is RPLMap)
				{
					if (cellIndex != -1)
					{
						this.RenderCellStyle(element.ElementProps.Style, cellIndex);
					}
					RPLDynamicImageProps rPLDynamicImageProps = (RPLDynamicImageProps)element.ElementProps;
					byte[] array2 = null;
					if (rPLDynamicImageProps.DynamicImageContent != null)
					{
						array2 = new byte[(int)rPLDynamicImageProps.DynamicImageContent.Length];
						rPLDynamicImageProps.DynamicImageContent.Position = 0L;
						rPLDynamicImageProps.DynamicImageContent.Read(array2, 0, (int)rPLDynamicImageProps.DynamicImageContent.Length);
					}
					else if (rPLDynamicImageProps.DynamicImageContentOffset >= 0)
					{
						array2 = this.m_rplReport.GetImage(rPLDynamicImageProps.DynamicImageContentOffset);
					}
					this.m_writer.AddImage(array2, measurement.Height, measurement.Width, RPLFormat.Sizings.Fit);
				}
				else if (element is RPLContainer)
				{
					this.RenderRPLContainer(element, inTablix, measurement, cellIndex, borderContext, flag);
				}
			}
		}

		protected abstract void RenderRPLContainer(RPLElement element, bool inTablix, RPLItemMeasurement measurement, int cellIndex, BorderContext borderContext, bool hasBorder);

		protected void RenderRPLContainerProperties(RPLElement element, bool inTablix, int cellIndex)
		{
			if (inTablix && cellIndex > -1)
			{
				this.RenderCellProperties(element.ElementProps.Style, cellIndex, true);
			}
		}

		protected void RenderRPLContainerContents(RPLElement element, RPLItemMeasurement measurement, BorderContext borderContext, bool inTablix, bool hasBorder)
		{
			if (!this.RenderRectangle(element as RPLContainer, 0f, measurement, borderContext, inTablix) && !inTablix)
			{
				this.WriteBeginTableRowCell(measurement, false);
				this.RenderCellProperties(element.ElementProps.Style, 0, hasBorder);
				this.m_writer.ApplyCellBorderContext(borderContext);
				this.m_writer.WriteEmptyStyle();
				this.WriteEndCellRowTable(borderContext);
			}
		}

		private void RenderSubreport(RPLElement element, RPLItemMeasurement measurement, int cellIndex, BorderContext borderContext, bool inTablix, bool hasBorder)
		{
			if (hasBorder)
			{
				this.WriteBeginTableRowCell(measurement, false);
				this.m_writer.ApplyCellBorderContext(borderContext);
				cellIndex = 0;
			}
			if (cellIndex != -1)
			{
				this.RenderCellProperties(element.ElementProps.Style, cellIndex, hasBorder || inTablix, false, false);
			}
			BorderContext parentBorderContext = this.HasBorders(element.ElementProps.Style, borderContext);
			RPLContainer rPLContainer = element as RPLContainer;
			int num = rPLContainer.Children.Length;
			bool flag = false;
			if (num == 1)
			{
				RPLContainer rectangle = (RPLContainer)rPLContainer.Children[0].Element;
				flag = this.RenderRectangle(rectangle, 0f, measurement, parentBorderContext, inTablix);
			}
			else
			{
				float width = measurement.Width;
				float num2 = 0f;
				for (int i = 0; i < num; i++)
				{
					RPLItemMeasurement rPLItemMeasurement = rPLContainer.Children[i];
					rPLItemMeasurement.Width = width;
					rPLItemMeasurement.Top = num2;
					num2 += rPLItemMeasurement.Height;
				}
				measurement.Height = num2;
				flag = this.RenderRectangle(rPLContainer, 0f, measurement, parentBorderContext, false);
			}
			if (hasBorder)
			{
				this.WriteEndCellRowTable(borderContext);
			}
			else if (!flag && !inTablix)
			{
				this.WriteBeginTableRowCell(measurement, false);
				this.RenderCellProperties(element.ElementProps.Style, 0, hasBorder);
				this.m_writer.ApplyCellBorderContext(borderContext);
				this.m_writer.WriteEmptyStyle();
				this.WriteEndCellRowTable(borderContext);
			}
		}

		private static void RenderSizeProp(RPLReportSize nonShared, RPLReportSize shared, RenderSize sizeFunction)
		{
			double num = 0.0;
			if (nonShared == null)
			{
				if (shared != null)
				{
					num = shared.ToPoints();
					goto IL_0021;
				}
				return;
			}
			num = nonShared.ToPoints();
			goto IL_0021;
			IL_0021:
			if (num > 0.0)
			{
				sizeFunction((float)num);
			}
		}

		protected void RenderTextBox(RPLTextBox textBox, bool inTablix, int cellIndex, bool needsTable, RPLElementStyle style, RPLItemMeasurement measurement, bool notCanGrow, RPLTextBoxPropsDef textBoxPropsDef, RPLTextBoxProps textBoxProps, bool isSimple, string textBoxValue, BorderContext borderContext, int oldCellIndex)
		{
			if (needsTable)
			{
				if (inTablix)
				{
					this.RemovePaddingFromMeasurement(measurement, style);
				}
				this.WriteBeginTableRowCell(measurement, notCanGrow);
				this.m_writer.ApplyCellBorderContext(borderContext);
				cellIndex = 0;
			}
			if (textBoxPropsDef.CanGrow && textBoxPropsDef.CanShrink)
			{
				this.m_writer.IgnoreRowHeight(true);
			}
			TypeCode typeCode = textBoxProps.TypeCode;
			ArrayList arrayList = null;
			if (this.m_inHeaderFooter && isSimple)
			{
				string text = textBoxPropsDef.Formula;
				if (!string.IsNullOrEmpty(text))
				{
					if (text.StartsWith("=", StringComparison.Ordinal))
					{
						text = text.Remove(0, 1);
					}
					arrayList = FormulaHandler.ProcessHeaderFooterFormula(text);
				}
			}
			bool flag = this.HasAction(textBoxProps.ActionInfo);
			RPLAction rPLAction = null;
			if (flag)
			{
				rPLAction = textBoxProps.ActionInfo.Actions[0];
				this.RenderAction(rPLAction);
			}
			if (arrayList != null)
			{
				for (int i = 0; i < arrayList.Count; i++)
				{
					this.RenderTextProperties(typeCode, style);
					this.RenderFormulaString(arrayList[i]);
				}
			}
			else if (isSimple)
			{
				if (!string.IsNullOrEmpty(textBoxValue))
				{
					RPLFormat.Directions directions = RPLFormat.Directions.LTR;
					object obj = style[29];
					if (obj != null)
					{
						directions = (RPLFormat.Directions)obj;
					}
					this.m_writer.AddTextStyleProp(29, directions);
					object obj2 = style[25];
					if (obj2 != null)
					{
						this.m_writer.RenderTextAlign(typeCode, (RPLFormat.TextAlignments)obj2, directions);
					}
					this.RenderTextRunStyle(style, directions);
					this.m_writer.WriteText(textBoxValue);
				}
			}
			else
			{
				this.RenderTextBoxRich(textBox, rPLAction);
			}
			if (flag)
			{
				this.m_writer.WriteHyperlinkEnd();
			}
			if (needsTable)
			{
				this.RenderCellProperties(style, cellIndex, !inTablix, !inTablix, true);
				this.WriteEndCellRowTable(borderContext);
				cellIndex = oldCellIndex;
			}
		}

		protected void RenderTextBoxProperties(bool inTablix, int cellIndex, bool needsTable, RPLElementStyle style)
		{
			if (!inTablix && needsTable)
			{
				return;
			}
			this.RenderCellProperties(style, cellIndex, true, true, !needsTable);
		}

		protected RPLTextBoxProps GetTextBoxProperties(RPLTextBox textBox, out RPLTextBoxPropsDef textBoxPropsDef, out bool isSimple, out string textBoxValue, bool inTablix, out bool notCanGrow, bool hasBorder, int cellIndex, out bool needsTable, out RPLElementStyle style, out int oldCellIndex)
		{
			RPLTextBoxProps rPLTextBoxProps = textBox.ElementProps as RPLTextBoxProps;
			textBoxPropsDef = (rPLTextBoxProps.Definition as RPLTextBoxPropsDef);
			isSimple = textBoxPropsDef.IsSimple;
			textBoxValue = null;
			if (isSimple)
			{
				textBoxValue = rPLTextBoxProps.Value;
				if (string.IsNullOrEmpty(textBoxValue))
				{
					textBoxValue = textBoxPropsDef.Value;
				}
			}
			notCanGrow = (!textBoxPropsDef.CanGrow && isSimple && !string.IsNullOrEmpty(textBoxValue));
			needsTable = (!inTablix || notCanGrow || hasBorder);
			oldCellIndex = cellIndex;
			style = textBox.ElementProps.Style;
			return rPLTextBoxProps;
		}

		private void RenderTextBoxRich(RPLTextBox textBox, RPLAction textBoxAction)
		{
			RPLTextBoxProps rPLTextBoxProps = textBox.ElementProps as RPLTextBoxProps;
			RPLElementStyle style = rPLTextBoxProps.Style;
			RPLFormat.Directions directions = (RPLFormat.Directions)style[29];
			RPLParagraph nextParagraph = textBox.GetNextParagraph();
			RPLTextRunProps rPLTextRunProps = null;
			RPLTextRunPropsDef rPLTextRunPropsDef = null;
			this.m_writer.ResetListlevels();
			bool flag = directions == RPLFormat.Directions.RTL;
			Queue<RPLParagraph> queue = null;
			Queue<RPLTextRun> queue2 = null;
			if (this.m_needsToResetTextboxes)
			{
				queue = new Queue<RPLParagraph>();
				queue2 = new Queue<RPLTextRun>();
			}
			while (nextParagraph != null)
			{
				RPLParagraphProps rPLParagraphProps = nextParagraph.ElementProps as RPLParagraphProps;
				RPLParagraphPropsDef rPLParagraphPropsDef = rPLParagraphProps.Definition as RPLParagraphPropsDef;
				RPLElementStyle style2 = rPLParagraphProps.Style;
				object obj = style2[25];
				if (obj != null)
				{
					this.m_writer.RenderTextAlign(TypeCode.String, (RPLFormat.TextAlignments)obj, directions);
				}
				this.m_writer.AddTextStyleProp(29, directions);
				RPLReportSize hangingIndent = rPLParagraphProps.HangingIndent;
				if (hangingIndent == null)
				{
					hangingIndent = rPLParagraphPropsDef.HangingIndent;
				}
				RPLReportSize rPLReportSize = rPLParagraphProps.LeftIndent;
				if (rPLReportSize == null)
				{
					rPLReportSize = rPLParagraphPropsDef.LeftIndent;
				}
				RPLReportSize rPLReportSize2 = rPLParagraphProps.RightIndent;
				if (rPLReportSize2 == null)
				{
					rPLReportSize2 = rPLParagraphPropsDef.RightIndent;
				}
				if (flag)
				{
					RPLReportSize rPLReportSize3 = rPLReportSize2;
					rPLReportSize2 = rPLReportSize;
					rPLReportSize = rPLReportSize3;
				}
				double num = 0.0;
				if (rPLReportSize != null)
				{
					num = rPLReportSize.ToPoints();
				}
				double num2 = 0.0;
				if (hangingIndent != null)
				{
					num2 = hangingIndent.ToPoints();
				}
				int num3 = rPLParagraphProps.ListLevel ?? rPLParagraphPropsDef.ListLevel;
				RPLFormat.ListStyles listStyle = rPLParagraphProps.ListStyle ?? rPLParagraphPropsDef.ListStyle;
				if (num3 > 9)
				{
					num3 = 9;
				}
				if (num3 > 0)
				{
					num += (double)(36 * (num3 - 1) + 18);
					num2 -= 18.0;
				}
				if (num2 < 0.0)
				{
					num -= num2;
				}
				if (num2 != 0.0)
				{
					this.m_writer.AddFirstLineIndent((float)num2);
				}
				if (num > 0.0)
				{
					this.m_writer.AddLeftIndent((float)num);
				}
				if (rPLReportSize2 != null)
				{
					this.m_writer.AddRightIndent((float)rPLReportSize2.ToMillimeters());
				}
				RPLReportSize spaceAfter = rPLParagraphProps.SpaceAfter;
				RPLReportSize spaceAfter2 = rPLParagraphPropsDef.SpaceAfter;
				IWordWriter writer = this.m_writer;
				WordRenderer.RenderSizeProp(spaceAfter, spaceAfter2, writer.AddSpaceAfter);
				RPLReportSize spaceBefore = rPLParagraphProps.SpaceBefore;
				RPLReportSize spaceBefore2 = rPLParagraphPropsDef.SpaceBefore;
				IWordWriter writer2 = this.m_writer;
				WordRenderer.RenderSizeProp(spaceBefore, spaceBefore2, writer2.AddSpaceBefore);
				for (RPLTextRun nextTextRun = nextParagraph.GetNextTextRun(); nextTextRun != null; nextTextRun = nextParagraph.GetNextTextRun())
				{
					rPLTextRunProps = (nextTextRun.ElementProps as RPLTextRunProps);
					bool flag2 = this.HasAction(rPLTextRunProps.ActionInfo);
					if (flag2)
					{
						if (textBoxAction != null)
						{
							this.m_writer.WriteHyperlinkEnd();
						}
						RPLAction action = rPLTextRunProps.ActionInfo.Actions[0];
						this.RenderAction(action);
					}
					ArrayList arrayList = null;
					if (this.m_inHeaderFooter)
					{
						rPLTextRunPropsDef = (rPLTextRunProps.Definition as RPLTextRunPropsDef);
						string text = rPLTextRunPropsDef.Formula;
						if (!string.IsNullOrEmpty(text) && rPLTextRunPropsDef.Markup != RPLFormat.MarkupStyles.HTML && rPLTextRunProps.Markup != RPLFormat.MarkupStyles.HTML)
						{
							if (text.StartsWith("=", StringComparison.Ordinal))
							{
								text = text.Remove(0, 1);
							}
							arrayList = FormulaHandler.ProcessHeaderFooterFormula(text);
						}
					}
					if (arrayList != null)
					{
						for (int i = 0; i < arrayList.Count; i++)
						{
							this.RenderTextRunStyle(rPLTextRunProps.Style, directions);
							this.RenderFormulaString(arrayList[i]);
						}
					}
					else
					{
						string value = rPLTextRunProps.Value;
						if (value == null)
						{
							RPLTextRunPropsDef rPLTextRunPropsDef2 = rPLTextRunProps.Definition as RPLTextRunPropsDef;
							value = rPLTextRunPropsDef2.Value;
						}
						if (!string.IsNullOrEmpty(value))
						{
							this.RenderTextRunStyle(rPLTextRunProps.Style, directions);
							this.m_writer.WriteText(value);
						}
					}
					if (flag2)
					{
						this.m_writer.WriteHyperlinkEnd();
						if (textBoxAction != null)
						{
							this.RenderAction(textBoxAction);
						}
					}
					if (this.m_needsToResetTextboxes)
					{
						queue2.Enqueue(nextTextRun);
					}
				}
				if (this.m_needsToResetTextboxes)
				{
					nextParagraph.TextRuns = queue2;
					queue2 = new Queue<RPLTextRun>();
					queue.Enqueue(nextParagraph);
				}
				nextParagraph = textBox.GetNextParagraph();
				if (num3 > 0)
				{
					this.m_writer.WriteListEnd(num3, listStyle, nextParagraph != null);
				}
				else if (nextParagraph != null)
				{
					this.m_writer.WriteParagraphEnd();
					this.m_writer.ResetListlevels();
				}
			}
			if (this.m_needsToResetTextboxes)
			{
				textBox.Paragraphs = queue;
			}
		}

		public void RenderTextRunStyle(RPLElementStyle runStyle, RPLFormat.Directions dir)
		{
			object obj = runStyle[22];
			if (obj != null)
			{
				this.m_writer.RenderFontWeight((RPLFormat.FontWeights)obj, dir);
			}
			obj = runStyle[19];
			if (obj != null)
			{
				this.m_writer.RenderFontStyle((RPLFormat.FontStyles)obj, dir);
			}
			obj = runStyle[32];
			if (obj != null)
			{
				this.m_writer.AddTextStyleProp(32, obj);
			}
			if (dir != 0)
			{
				this.m_writer.RenderTextRunDirection(dir);
			}
			this.m_writer.AddTextStyleProp(24, runStyle[24]);
			this.m_writer.AddTextStyleProp(27, runStyle[27]);
			string text = runStyle[20] as string;
			if (text != null)
			{
				this.m_writer.RenderFontFamily(text, dir);
			}
			text = (runStyle[21] as string);
			if (text != null)
			{
				this.m_writer.RenderFontSize(text, dir);
			}
		}

		public void RemovePaddingFromMeasurement(RPLItemMeasurement measurement, RPLElementStyle style)
		{
			double num = this.ToMM(style[15], null);
			double num2 = this.ToMM(style[16], null);
			measurement.Width = (float)((double)measurement.Width - (num + num2));
			double num3 = this.ToMM(style[17], null);
			double num4 = this.ToMM(style[18], null);
			measurement.Height = (float)((double)measurement.Height - (num3 + num4));
		}

		public bool HasAnyBorder(RPLElementStyle style)
		{
			if (!this.HasBorder(style, Positions.Top) && !this.HasBorder(style, Positions.Bottom) && !this.HasBorder(style, Positions.Left))
			{
				return this.HasBorder(style, Positions.Right);
			}
			return true;
		}

		public BorderContext HasBorders(RPLElementStyle style, BorderContext parentBorderContext)
		{
			BorderContext borderContext = new BorderContext();
			borderContext.Top = (parentBorderContext.Top || this.HasBorder(style, Positions.Top));
			borderContext.Left = (parentBorderContext.Left || this.HasBorder(style, Positions.Left));
			borderContext.Bottom = (parentBorderContext.Bottom || this.HasBorder(style, Positions.Bottom));
			borderContext.Right = (parentBorderContext.Right || this.HasBorder(style, Positions.Right));
			return borderContext;
		}

		public bool HasBorder(RPLElementStyle style, Positions pos)
		{
			object defaultSize = style[10];
			object obj = style[5];
			object size = null;
			object obj2 = null;
			switch (pos)
			{
			case Positions.Top:
				size = style[13];
				obj2 = style[8];
				break;
			case Positions.Bottom:
				size = style[14];
				obj2 = style[9];
				break;
			case Positions.Right:
				size = style[12];
				obj2 = style[7];
				break;
			case Positions.Left:
				size = style[11];
				obj2 = style[6];
				break;
			}
			if (obj2 != null)
			{
				if ((RPLFormat.BorderStyles)obj2 == RPLFormat.BorderStyles.None)
				{
					return false;
				}
			}
			else if (obj != null && (RPLFormat.BorderStyles)obj == RPLFormat.BorderStyles.None)
			{
				return false;
			}
			double num = this.ToMM(size, defaultSize);
			return num > 0.0;
		}

		public static bool IsWritingModeVertical(IRPLStyle style)
		{
			object obj = style[30];
			if (obj != null)
			{
				RPLFormat.WritingModes writingModes = (RPLFormat.WritingModes)obj;
				if (writingModes != RPLFormat.WritingModes.Vertical && writingModes != RPLFormat.WritingModes.Rotate270)
				{
					goto IL_001d;
				}
				return true;
			}
			goto IL_001d;
			IL_001d:
			return false;
		}

		public double ToMM(object size, object defaultSize)
		{
			if (size == null)
			{
				if (defaultSize == null)
				{
					return 0.0;
				}
				size = defaultSize;
			}
			RPLReportSize rPLReportSize = new RPLReportSize(size as string);
			return rPLReportSize.ToMillimeters();
		}

		private void RenderAction(RPLAction action)
		{
			string text = action.Hyperlink;
			bool bookmarkLink = false;
			if (text == null || this.m_omitHyperlinks)
			{
				text = action.DrillthroughUrl;
				if (text == null || this.m_omitDrillthroughs)
				{
					text = action.BookmarkLink;
					bookmarkLink = (text != null);
				}
			}
			if (text != null)
			{
				this.m_writer.WriteHyperlinkBegin(text, bookmarkLink);
			}
		}

		private void RenderFormulaString(object obj)
		{
			if (obj is string)
			{
				this.m_writer.WriteText((string)obj);
			}
			else if (obj is FormulaHandler.GlobalExpressionType)
			{
				switch ((FormulaHandler.GlobalExpressionType)obj)
				{
				case FormulaHandler.GlobalExpressionType.PageNumber:
					this.m_writer.WritePageNumberField();
					break;
				case FormulaHandler.GlobalExpressionType.ReportName:
					this.m_writer.WriteText(this.m_rplReport.ReportName);
					break;
				case FormulaHandler.GlobalExpressionType.TotalPages:
					this.m_writer.WriteTotalPagesField();
					break;
				}
			}
		}

		private void RenderBookmarksLabels(RPLElement element)
		{
			RPLItem rPLItem = element as RPLItem;
			RPLItemProps rPLItemProps = rPLItem.ElementProps as RPLItemProps;
			RPLItemPropsDef rPLItemPropsDef = rPLItemProps.Definition as RPLItemPropsDef;
			if (rPLItem is RPLRectangle)
			{
				RPLRectanglePropsDef rPLRectanglePropsDef = rPLItemPropsDef as RPLRectanglePropsDef;
				if (rPLRectanglePropsDef.LinkToChildId != null)
				{
					return;
				}
			}
			if (!string.IsNullOrEmpty(rPLItemProps.Bookmark))
			{
				this.m_writer.RenderBookmark(rPLItemProps.Bookmark);
			}
			else if (!string.IsNullOrEmpty(rPLItemPropsDef.Bookmark))
			{
				this.m_writer.RenderBookmark(rPLItemPropsDef.Bookmark);
			}
			if (!string.IsNullOrEmpty(rPLItemProps.Label))
			{
				this.m_writer.RenderLabel(rPLItemProps.Label, this.m_labelLevel);
			}
			else if (!string.IsNullOrEmpty(rPLItemPropsDef.Label))
			{
				this.m_writer.RenderLabel(rPLItemPropsDef.Label, this.m_labelLevel);
			}
		}

		private void RenderTextProperties(TypeCode typeCode, IRPLStyle style)
		{
			this.m_writer.AddTextStyleProp(23, style[23]);
			this.m_writer.AddTextStyleProp(24, style[24]);
			this.m_writer.AddTextStyleProp(27, style[27]);
			this.m_writer.AddTextStyleProp(28, style[28]);
			this.m_writer.AddTextStyleProp(31, style[31]);
			this.m_writer.AddTextStyleProp(32, style[32]);
			object obj = style[29];
			RPLFormat.Directions directions = (obj != null) ? ((RPLFormat.Directions)obj) : RPLFormat.Directions.LTR;
			this.m_writer.AddTextStyleProp(29, obj);
			object obj2 = style[25];
			if (obj2 != null)
			{
				this.m_writer.RenderTextAlign(typeCode, (RPLFormat.TextAlignments)obj2, directions);
			}
			obj2 = style[21];
			if (obj2 != null)
			{
				this.m_writer.RenderFontSize(obj2 as string, directions);
			}
			obj2 = style[19];
			if (obj2 != null)
			{
				this.m_writer.RenderFontStyle((RPLFormat.FontStyles)obj2, directions);
			}
			obj2 = style[22];
			if (obj2 != null)
			{
				this.m_writer.RenderFontWeight((RPLFormat.FontWeights)obj2, directions);
			}
			obj2 = style[20];
			if (obj2 != null)
			{
				this.m_writer.RenderFontFamily(obj2 as string, directions);
			}
		}

		private void RenderCellProperties(IRPLStyle style, int cellIndex, bool needsBorderOrPadding)
		{
			this.RenderCellProperties(style, cellIndex, needsBorderOrPadding, needsBorderOrPadding, false);
		}

		private void RenderCellProperties(IRPLStyle style, int cellIndex, bool needsBorder, bool needsPadding, bool needsWritingMode)
		{
			if (needsBorder)
			{
				this.m_writer.AddCellStyleProp(cellIndex, 0, style[0]);
				this.m_writer.AddCellStyleProp(cellIndex, 1, style[1]);
				this.m_writer.AddCellStyleProp(cellIndex, 2, style[2]);
				this.m_writer.AddCellStyleProp(cellIndex, 3, style[3]);
				this.m_writer.AddCellStyleProp(cellIndex, 4, style[4]);
				this.m_writer.AddCellStyleProp(cellIndex, 5, style[5]);
				this.m_writer.AddCellStyleProp(cellIndex, 6, style[6]);
				this.m_writer.AddCellStyleProp(cellIndex, 7, style[7]);
				this.m_writer.AddCellStyleProp(cellIndex, 8, style[8]);
				this.m_writer.AddCellStyleProp(cellIndex, 9, style[9]);
				this.m_writer.AddCellStyleProp(cellIndex, 10, style[10]);
				this.m_writer.AddCellStyleProp(cellIndex, 11, style[11]);
				this.m_writer.AddCellStyleProp(cellIndex, 12, style[12]);
				this.m_writer.AddCellStyleProp(cellIndex, 13, style[13]);
				this.m_writer.AddCellStyleProp(cellIndex, 14, style[14]);
			}
			if (needsPadding)
			{
				this.m_writer.AddPadding(cellIndex, 18, style[18], 0);
				this.m_writer.AddPadding(cellIndex, 15, style[15], 0);
				this.m_writer.AddPadding(cellIndex, 16, style[16], 0);
				this.m_writer.AddPadding(cellIndex, 17, style[17], 0);
			}
			else
			{
				this.SetZeroPadding(cellIndex);
			}
			this.m_writer.AddCellStyleProp(cellIndex, 26, style[26]);
			if (needsWritingMode)
			{
				this.m_writer.AddCellStyleProp(cellIndex, 30, style[30]);
			}
			this.m_writer.AddCellStyleProp(cellIndex, 33, style[33]);
			this.m_writer.AddCellStyleProp(cellIndex, 34, style[34]);
		}

		private void RenderTableProperties(IRPLStyle style, bool isTablix, BorderContext parentBorderContext)
		{
			if (isTablix)
			{
				this.m_writer.AddTableStyleProp(0, style[0]);
				this.m_writer.AddTableStyleProp(1, style[1]);
				this.m_writer.AddTableStyleProp(2, style[2]);
				this.m_writer.AddTableStyleProp(3, style[3]);
				this.m_writer.AddTableStyleProp(4, style[4]);
				this.m_writer.AddTableStyleProp(5, style[5]);
				this.m_writer.AddTableStyleProp(6, style[6]);
				this.m_writer.AddTableStyleProp(7, style[7]);
				this.m_writer.AddTableStyleProp(8, style[8]);
				this.m_writer.AddTableStyleProp(9, style[9]);
				this.m_writer.AddTableStyleProp(10, style[10]);
				this.m_writer.AddTableStyleProp(11, style[11]);
				this.m_writer.AddTableStyleProp(12, style[12]);
				this.m_writer.AddTableStyleProp(13, style[13]);
				this.m_writer.AddTableStyleProp(14, style[14]);
				this.m_writer.SetTableContext(parentBorderContext);
			}
			this.m_writer.AddTableStyleProp(26, style[26]);
			this.m_writer.AddTableStyleProp(30, style[30]);
			this.m_writer.AddTableStyleProp(33, style[33]);
			this.m_writer.AddTableStyleProp(34, style[34]);
		}

		private void SetZeroPadding(int cellIndex)
		{
			this.m_writer.AddPadding(cellIndex, 18, null, 0);
			this.m_writer.AddPadding(cellIndex, 15, null, 0);
			this.m_writer.AddPadding(cellIndex, 16, null, 0);
			this.m_writer.AddPadding(cellIndex, 17, null, 0);
		}

		private void RenderCellStyle(IRPLStyle style, int cellIndex)
		{
			this.RenderCellProperties(style, cellIndex, true, true, false);
		}

		private void RenderSplitTablix(RPLTablix tablix, RPLItemMeasurement measurement, int splitColumn, BorderContext parentBorderContext)
		{
			float[] columnWidths = tablix.ColumnWidths;
			int totalColumns = columnWidths.Length;
			int totalRows = tablix.RowHeights.Length;
			int num = (int)Math.Ceiling((double)((float)columnWidths.Length / (float)splitColumn));
			List<RPLTablixMemberCell> list = new List<RPLTablixMemberCell>();
			SplitTablix[] array = new SplitTablix[num];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new SplitTablix(tablix.RowHeights.Length);
			}
			List<RPLTablixMemberCell>[] array2 = new List<RPLTablixMemberCell>[tablix.RowHeights.Length];
			RPLTablixRow nextRow = tablix.GetNextRow();
			int num2 = 0;
			while (nextRow != null)
			{
				if (nextRow is RPLTablixOmittedRow)
				{
					List<RPLTablixMemberCell> omittedHeaders = nextRow.OmittedHeaders;
					for (int j = 0; j < omittedHeaders.Count; j++)
					{
						if (!string.IsNullOrEmpty(omittedHeaders[j].GroupLabel))
						{
							this.m_writer.RenderLabel(omittedHeaders[j].GroupLabel, this.m_labelLevel);
						}
					}
					nextRow = tablix.GetNextRow();
				}
				else
				{
					if (nextRow.NumCells != 0)
					{
						for (int k = 0; k < array.Length; k++)
						{
							array[k][num2] = new SplitTablixRow();
						}
						array2[num2] = nextRow.OmittedHeaders;
						if (tablix.ColsBeforeRowHeaders > 0 && nextRow.BodyStart != -1)
						{
							int num3 = 0;
							int colsBeforeRowHeaders = tablix.ColsBeforeRowHeaders;
							int l;
							for (l = nextRow.BodyStart; l < nextRow.NumCells; l++)
							{
								if (num3 >= colsBeforeRowHeaders)
								{
									break;
								}
								RPLTablixCell cell = nextRow[l];
								WordRenderer.PlaceCellIntoSplitTablix(array, num2, cell, totalColumns, totalRows, splitColumn, parentBorderContext);
							}
							int num4 = (nextRow.BodyStart > nextRow.HeaderStart) ? nextRow.BodyStart : nextRow.NumCells;
							for (int m = nextRow.HeaderStart; m < num4; m++)
							{
								RPLTablixCell cell2 = nextRow[m];
								WordRenderer.PlaceCellIntoSplitTablix(array, num2, cell2, totalColumns, totalRows, splitColumn, parentBorderContext);
							}
							num4 = ((nextRow.BodyStart < nextRow.HeaderStart) ? nextRow.HeaderStart : nextRow.NumCells);
							for (int n = l; n < num4; n++)
							{
								RPLTablixCell cell3 = nextRow[n];
								WordRenderer.PlaceCellIntoSplitTablix(array, num2, cell3, totalColumns, totalRows, splitColumn, parentBorderContext);
							}
						}
						else
						{
							for (int num5 = 0; num5 < nextRow.NumCells; num5++)
							{
								RPLTablixCell cell4 = nextRow[num5];
								WordRenderer.PlaceCellIntoSplitTablix(array, num2, cell4, totalColumns, totalRows, splitColumn, parentBorderContext);
							}
						}
					}
					nextRow = tablix.GetNextRow();
					num2++;
				}
			}
			float[] array3 = new float[num];
			for (int num6 = 0; num6 < columnWidths.Length; num6++)
			{
				array3[num6 / splitColumn] += columnWidths[num6];
			}
			BorderContext borderContext = new BorderContext();
			this.m_writer.WriteTableBegin(0f, false);
			this.m_writer.WriteTableRowBegin(0f, measurement.Height, array3);
			BorderContext borderContext2 = this.HasBorders(tablix.ElementProps.Style, parentBorderContext);
			int num7 = columnWidths.Length % splitColumn;
			for (int num8 = 0; num8 < num; num8++)
			{
				this.m_writer.WriteTableCellBegin(num8, num, false, false, false, false);
				this.m_writer.WriteTableBegin(0f, false);
				SplitTablix splitTablix = array[num8];
				int num9 = (num7 > 0 && num8 == num - 1) ? num7 : splitColumn;
				float[] array4 = new float[num9];
				Array.Copy(columnWidths, num8 * splitColumn, array4, 0, num9);
				TablixGhostCell[] array5 = new TablixGhostCell[array4.Length];
				for (int num10 = 0; num10 < tablix.RowHeights.Length; num10++)
				{
					this.m_writer.WriteTableRowBegin(0f, tablix.RowHeights[num10], array4);
					SplitTablixRow splitTablixRow = splitTablix[num10];
					int num11 = 0;
					if (splitTablixRow.FirstCell != null)
					{
						TablixGhostCell firstCell = splitTablixRow.FirstCell;
						for (int num12 = 0; num12 < firstCell.ColSpan; num12++)
						{
							this.m_writer.WriteTableCellBegin(num11 + num12, num9, firstCell.RowSpan > 1, firstCell.ColSpan > 1 && num12 == 0, false, num12 > 0);
							this.m_writer.ApplyCellBorderContext(firstCell.Context);
							if (firstCell.Cell.Element != null)
							{
								IRPLStyle style = firstCell.Cell.Element.ElementProps.Style;
								this.RenderCellStyle(style, num11 + num12);
							}
							this.m_writer.WriteTableCellEnd(num11 + num12, firstCell.Context, false);
						}
						num11 += firstCell.ColSpan;
						firstCell.RowSpan--;
						if (firstCell.RowSpan > 0)
						{
							array5[0] = firstCell;
						}
					}
					List<RPLTablixMemberCell>.Enumerator enumerator = list.GetEnumerator();
					if (array2[num10] != null)
					{
						enumerator = array2[num10].GetEnumerator();
					}
					enumerator.MoveNext();
					if (splitTablixRow.Cells.Count == 0)
					{
						this.RenderGhostCells(array4, array5, num11, num9);
					}
					else
					{
						for (int num13 = 0; num13 < splitTablixRow.Cells.Count; num13++)
						{
							RPLTablixCell rPLTablixCell = splitTablixRow.Cells[num13];
							borderContext.Top = (rPLTablixCell.RowIndex == 0 && borderContext2.Top);
							borderContext.Left = (rPLTablixCell.ColIndex == 0 && borderContext2.Left);
							borderContext.Bottom = (rPLTablixCell.RowIndex + rPLTablixCell.RowSpan == tablix.RowHeights.Length && borderContext2.Bottom);
							borderContext.Right = (rPLTablixCell.ColIndex + rPLTablixCell.ColSpan == tablix.ColumnWidths.Length && borderContext2.Right);
							this.RenderTablixCell(tablix, 0f, array4, array5, borderContext, num11, rPLTablixCell, enumerator, rPLTablixCell.ColIndex + rPLTablixCell.ColSpan == tablix.ColumnWidths.Length);
							num11 = rPLTablixCell.ColIndex + rPLTablixCell.ColSpan;
						}
						if (num11 != num9)
						{
							this.RenderGhostCells(array4, array5, num11, num9);
						}
						while (enumerator.Current != null)
						{
							if (!string.IsNullOrEmpty(enumerator.Current.GroupLabel))
							{
								this.m_writer.RenderLabel(enumerator.Current.GroupLabel, this.m_labelLevel);
							}
							enumerator.MoveNext();
						}
					}
					this.m_writer.WriteTableRowEnd();
				}
				this.m_writer.WriteTableEnd();
				this.m_writer.WriteTableCellEnd(num8, new BorderContext(), false);
			}
			this.m_writer.WriteTableRowEnd();
			this.m_writer.WriteTableEnd();
		}

		private static void PlaceCellIntoSplitTablix(SplitTablix[] tablices, int x, RPLTablixCell cell, int totalColumns, int totalRows, int splitColumn, BorderContext parentBorderContext)
		{
			int num = cell.ColIndex / splitColumn;
			tablices[num][x].Cells.Add(cell);
			int num2 = cell.ColIndex + cell.ColSpan;
			int i = num + 1;
			if (num2 > i * splitColumn)
			{
				cell.ColSpan = i * splitColumn - cell.ColIndex;
				for (; num2 > i * splitColumn; i++)
				{
					int colSpan = Math.Min(num2 - i * splitColumn, splitColumn);
					TablixGhostCell tablixGhostCell = new TablixGhostCell();
					tablixGhostCell.Cell = cell;
					tablixGhostCell.ColSpan = colSpan;
					tablixGhostCell.RowSpan = cell.RowSpan;
					tablixGhostCell.Context = new BorderContext();
					tablixGhostCell.Context.Top = (x == 0);
					tablixGhostCell.Context.Left = (num == 0 && cell.ColIndex == 0);
					tablixGhostCell.Context.Bottom = (x == totalRows - 1);
					tablixGhostCell.Context.Right = (num == tablices.Length - 1 && num2 == totalColumns - 1);
					tablices[i][x].FirstCell = tablixGhostCell;
				}
			}
			cell.ColIndex -= num * splitColumn;
		}

		private void RenderTablix(RPLTablix element, float left, BorderContext parentBorderContext, bool inTablix)
		{
			float[] columnWidths = element.ColumnWidths;
			TablixGhostCell[] ghostCells = new TablixGhostCell[columnWidths.Length];
			this.m_writer.WriteTableBegin(left, false);
			RPLElementStyle style = element.ElementProps.Style;
			BorderContext borderContext = this.HasBorders(style, parentBorderContext);
			if (inTablix)
			{
				parentBorderContext = borderContext;
			}
			this.RenderTablixStyle(style, parentBorderContext);
			RPLTablixRow nextRow = element.GetNextRow();
			BorderContext borderContext2 = new BorderContext();
			List<RPLTablixMemberCell> list = new List<RPLTablixMemberCell>();
			int num = 0;
			while (nextRow != null)
			{
				if (nextRow is RPLTablixOmittedRow)
				{
					List<RPLTablixMemberCell> omittedHeaders = nextRow.OmittedHeaders;
					for (int i = 0; i < omittedHeaders.Count; i++)
					{
						if (!string.IsNullOrEmpty(omittedHeaders[i].GroupLabel))
						{
							this.m_writer.RenderLabel(omittedHeaders[i].GroupLabel, this.m_labelLevel);
						}
					}
					nextRow = element.GetNextRow();
				}
				else
				{
					this.m_writer.WriteTableRowBegin(left, element.RowHeights[num], columnWidths);
					if (nextRow.NumCells == 0)
					{
						this.RenderGhostCells(columnWidths, ghostCells, 0, columnWidths.Length);
					}
					else
					{
						int num2 = 0;
						List<RPLTablixMemberCell> omittedHeaders2 = nextRow.OmittedHeaders;
						List<RPLTablixMemberCell>.Enumerator enumerator = list.GetEnumerator();
						if (omittedHeaders2 != null)
						{
							enumerator = omittedHeaders2.GetEnumerator();
						}
						enumerator.MoveNext();
						if (element.ColsBeforeRowHeaders > 0 && nextRow.BodyStart != -1)
						{
							int num3 = 0;
							int colsBeforeRowHeaders = element.ColsBeforeRowHeaders;
							int j;
							for (j = nextRow.BodyStart; j < nextRow.NumCells; j++)
							{
								if (num3 >= colsBeforeRowHeaders)
								{
									break;
								}
								RPLTablixCell rPLTablixCell = nextRow[j];
								borderContext2.Top = (rPLTablixCell.RowIndex == 0 && borderContext.Top);
								borderContext2.Left = (rPLTablixCell.ColIndex == 0 && borderContext.Left);
								borderContext2.Bottom = (borderContext.Bottom && rPLTablixCell.RowIndex + rPLTablixCell.RowSpan == element.RowHeights.Length);
								borderContext2.Right = (borderContext.Left && rPLTablixCell.ColIndex + rPLTablixCell.ColSpan == element.ColumnWidths.Length);
								this.RenderTablixCell(element, left, columnWidths, ghostCells, borderContext2, num2, rPLTablixCell, enumerator, rPLTablixCell.ColIndex + rPLTablixCell.ColSpan == element.ColumnWidths.Length);
								num2 = rPLTablixCell.ColIndex + rPLTablixCell.ColSpan;
								num3 += rPLTablixCell.ColSpan;
							}
							int num4 = (nextRow.BodyStart > nextRow.HeaderStart) ? nextRow.BodyStart : nextRow.NumCells;
							int num5 = (nextRow.HeaderStart >= 0) ? nextRow.HeaderStart : 0;
							for (int k = num5; k < num4; k++)
							{
								RPLTablixCell rPLTablixCell2 = nextRow[k];
								borderContext2.Top = (rPLTablixCell2.RowIndex == 0 && borderContext.Top);
								borderContext2.Left = (rPLTablixCell2.ColIndex == 0 && borderContext.Left);
								borderContext2.Bottom = (borderContext.Bottom && rPLTablixCell2.RowIndex + rPLTablixCell2.RowSpan == element.RowHeights.Length);
								borderContext2.Right = (borderContext.Right && rPLTablixCell2.ColIndex + rPLTablixCell2.ColSpan == element.ColumnWidths.Length);
								this.RenderTablixCell(element, left, columnWidths, ghostCells, borderContext2, num2, rPLTablixCell2, enumerator, rPLTablixCell2.ColIndex + rPLTablixCell2.ColSpan == element.ColumnWidths.Length);
								num2 = rPLTablixCell2.ColIndex + rPLTablixCell2.ColSpan;
							}
							num4 = ((nextRow.BodyStart < nextRow.HeaderStart) ? nextRow.HeaderStart : nextRow.NumCells);
							for (int l = j; l < num4; l++)
							{
								RPLTablixCell rPLTablixCell3 = nextRow[l];
								borderContext2.Top = (rPLTablixCell3.RowIndex == 0 && borderContext.Top);
								borderContext2.Left = (rPLTablixCell3.ColIndex == 0 && borderContext.Left);
								borderContext2.Bottom = (borderContext.Bottom && rPLTablixCell3.RowIndex + rPLTablixCell3.RowSpan == element.RowHeights.Length);
								borderContext2.Right = (borderContext.Right && rPLTablixCell3.ColIndex + rPLTablixCell3.ColSpan == element.ColumnWidths.Length);
								this.RenderTablixCell(element, left, columnWidths, ghostCells, borderContext2, num2, rPLTablixCell3, enumerator, rPLTablixCell3.ColIndex + rPLTablixCell3.ColSpan == element.ColumnWidths.Length);
								num2 = rPLTablixCell3.ColIndex + rPLTablixCell3.ColSpan;
							}
						}
						else
						{
							for (int m = 0; m < nextRow.NumCells; m++)
							{
								RPLTablixCell rPLTablixCell4 = nextRow[m];
								borderContext2.Top = (rPLTablixCell4.RowIndex == 0 && borderContext.Top);
								borderContext2.Left = (rPLTablixCell4.ColIndex == 0 && borderContext.Left);
								borderContext2.Bottom = (borderContext.Bottom && rPLTablixCell4.RowIndex + rPLTablixCell4.RowSpan == element.RowHeights.Length);
								borderContext2.Right = (borderContext.Right && rPLTablixCell4.ColIndex + rPLTablixCell4.ColSpan == element.ColumnWidths.Length);
								this.RenderTablixCell(element, left, columnWidths, ghostCells, borderContext2, num2, rPLTablixCell4, enumerator, rPLTablixCell4.ColIndex + rPLTablixCell4.ColSpan == element.ColumnWidths.Length);
								num2 = rPLTablixCell4.ColIndex + rPLTablixCell4.ColSpan;
							}
						}
						if (num2 != columnWidths.Length)
						{
							this.RenderGhostCells(columnWidths, ghostCells, num2, columnWidths.Length);
						}
					}
					this.m_writer.WriteTableRowEnd();
					nextRow = element.GetNextRow();
					num++;
				}
			}
			this.m_writer.WriteTableEnd();
		}

		protected void FinishRenderingTablixCell(RPLTablixCell cell, float[] widths, TablixGhostCell[] ghostCells, BorderContext borderContext)
		{
			RPLTablixMemberCell rPLTablixMemberCell = cell as RPLTablixMemberCell;
			if (rPLTablixMemberCell != null)
			{
				string groupLabel = rPLTablixMemberCell.GroupLabel;
				if (groupLabel != null)
				{
					this.m_writer.RenderLabel(groupLabel, this.m_labelLevel);
				}
			}
			this.m_writer.WriteTableCellEnd(cell.ColIndex, borderContext, false);
			if (cell.ColSpan > 1)
			{
				for (int i = 1; i < cell.ColSpan; i++)
				{
					this.m_writer.WriteTableCellBegin(cell.ColIndex + i, widths.Length, cell.RowSpan > 1, false, false, true);
					this.m_writer.ApplyCellBorderContext(borderContext);
					if (cell.Element != null)
					{
						this.RenderCellProperties(cell.Element.ElementProps.Style, cell.ColIndex + i, true);
						this.m_writer.ClearCellBorder(TableData.Positions.Left);
						if (i != cell.ColSpan - 1)
						{
							this.m_writer.ClearCellBorder(TableData.Positions.Right);
						}
						if (cell.RowSpan > 1)
						{
							this.m_writer.ClearCellBorder(TableData.Positions.Bottom);
						}
					}
					this.m_writer.WriteTableCellEnd(cell.ColIndex + i, borderContext, false);
				}
			}
			if (cell.RowSpan > 1)
			{
				ghostCells[cell.ColIndex] = new TablixGhostCell();
				ghostCells[cell.ColIndex].Cell = cell;
				ghostCells[cell.ColIndex].RowSpan = cell.RowSpan - 1;
				ghostCells[cell.ColIndex].ColSpan = cell.ColSpan;
				ghostCells[cell.ColIndex].Context = new BorderContext(borderContext);
			}
		}

		protected void RenderTablixCellItem(RPLTablixCell cell, float[] widths, RPLItemMeasurement measurement, float left, BorderContext borderContext)
		{
			this.RenderReportItem(cell.Element, measurement, cell.ColIndex, this.GetLeft(widths, cell.ColIndex, left), borderContext, true);
		}

		protected void ClearTablixCellBorders(RPLTablixCell cell)
		{
			if (cell.ColSpan > 1)
			{
				this.m_writer.ClearCellBorder(TableData.Positions.Right);
			}
			if (cell.RowSpan > 1)
			{
				this.m_writer.ClearCellBorder(TableData.Positions.Bottom);
			}
		}

		protected RPLItemMeasurement GetTablixCellMeasurement(RPLTablixCell cell, int nextCell, float[] widths, TablixGhostCell[] ghostCells, List<RPLTablixMemberCell>.Enumerator omittedCells, bool lastCell, RPLTablix tablix)
		{
			if (cell.ColIndex != nextCell)
			{
				this.RenderGhostCells(widths, ghostCells, nextCell, cell.ColIndex);
			}
			this.m_writer.WriteTableCellBegin(cell.ColIndex, widths.Length, cell.RowSpan > 1, cell.ColSpan > 1, false, false);
			this.RenderOmittedCells(omittedCells, cell.ColIndex, lastCell);
			RPLItemMeasurement rPLItemMeasurement = new RPLItemMeasurement();
			rPLItemMeasurement.Width = 0f;
			for (int i = 0; i < cell.ColSpan; i++)
			{
				rPLItemMeasurement.Width += tablix.ColumnWidths[i + cell.ColIndex];
			}
			rPLItemMeasurement.Height = 0f;
			for (int j = 0; j < cell.RowSpan; j++)
			{
				rPLItemMeasurement.Height += tablix.RowHeights[j + cell.RowIndex];
			}
			return rPLItemMeasurement;
		}

		private void RenderOmittedCells(List<RPLTablixMemberCell>.Enumerator omittedCells, int colIndex, bool lastCell)
		{
			while (true)
			{
				if (omittedCells.Current == null)
				{
					break;
				}
				if (omittedCells.Current.ColIndex != colIndex && !lastCell)
				{
					break;
				}
				if (!string.IsNullOrEmpty(omittedCells.Current.GroupLabel))
				{
					this.m_writer.RenderLabel(omittedCells.Current.GroupLabel, this.m_labelLevel);
				}
				omittedCells.MoveNext();
			}
		}

		private void RenderGhostCells(float[] widths, TablixGhostCell[] ghostCells, int nextCell, int endIndex)
		{
			while (nextCell < ghostCells.Length && ghostCells[nextCell] != null && nextCell < endIndex)
			{
				TablixGhostCell tablixGhostCell = ghostCells[nextCell];
				for (int i = 0; i < tablixGhostCell.ColSpan; i++)
				{
					this.m_writer.WriteTableCellBegin(nextCell + i, widths.Length, false, tablixGhostCell.ColSpan > 1 && i == 0, true, i > 0);
					this.m_writer.ApplyCellBorderContext(tablixGhostCell.Context);
					if (tablixGhostCell.Cell.Element != null)
					{
						IRPLStyle style = tablixGhostCell.Cell.Element.ElementProps.Style;
						this.RenderCellStyle(style, nextCell + i);
						this.m_writer.ClearCellBorder(TableData.Positions.Top);
						if (tablixGhostCell.RowSpan > 1)
						{
							this.m_writer.ClearCellBorder(TableData.Positions.Bottom);
						}
						if (i != 0)
						{
							this.m_writer.ClearCellBorder(TableData.Positions.Left);
						}
						if (i != tablixGhostCell.ColSpan - 1)
						{
							this.m_writer.ClearCellBorder(TableData.Positions.Right);
						}
					}
					this.m_writer.WriteTableCellEnd(nextCell + i, tablixGhostCell.Context, false);
				}
				tablixGhostCell.RowSpan--;
				if (tablixGhostCell.RowSpan == 0)
				{
					ghostCells[nextCell] = null;
				}
				nextCell += tablixGhostCell.ColSpan;
			}
		}

		private void RenderTablixStyle(IRPLStyle style, BorderContext borderContext)
		{
			this.RenderTableProperties(style, true, borderContext);
		}

		private float GetLeft(float[] widths, int index, float left)
		{
			float num = left;
			for (int i = 0; i < index; i++)
			{
				num += widths[i];
			}
			return num;
		}

		private static void AssertRectangleColumns(RPLContainer rectangle, PageTableLayout layout)
		{
			if (layout == null)
			{
				return;
			}
			if (layout.NrCols <= 63)
			{
				return;
			}
			string text = null;
			if (rectangle is RPLRectangle)
			{
				RPLRectanglePropsDef rPLRectanglePropsDef = (RPLRectanglePropsDef)rectangle.ElementProps.Definition;
				text = string.Format(CultureInfo.CurrentCulture, WordRenderRes.ColumnsErrorRectangle, rPLRectanglePropsDef.Name);
			}
			else
			{
				text = ((!(rectangle is RPLBody)) ? WordRenderRes.ColumnsErrorHeaderFooter : WordRenderRes.ColumnsErrorBody);
			}
			throw new ReportRenderingException(text);
		}

		private bool RenderRectangle(RPLContainer rectangle, float left, RPLMeasurement rectangleMeasurement, BorderContext parentBorderContext, bool inTablix)
		{
			return this.RenderRectangle(rectangle, left, false, rectangleMeasurement, parentBorderContext, inTablix, false);
		}

		protected bool RenderRectangle(RPLContainer rectangle, float left, bool canGrow, RPLMeasurement rectangleMeasurement, BorderContext parentBorderContext, bool inTablix, bool ignoreStyles)
		{
			RPLItemMeasurement[] children = rectangle.Children;
			if (children != null && children.Length != 0)
			{
				this.m_labelLevel++;
				PageTableLayout pageTableLayout = null;
				string text = null;
				if (rectangle is RPLRectangle)
				{
					RPLRectanglePropsDef rPLRectanglePropsDef = (RPLRectanglePropsDef)rectangle.ElementProps.Definition;
					text = rPLRectanglePropsDef.LinkToChildId;
				}
				float ownerWidth = 0f;
				float ownerHeight = 0f;
				if (rectangleMeasurement != null)
				{
					ownerWidth = rectangleMeasurement.Width;
					ownerHeight = rectangleMeasurement.Height;
				}
				PageTableLayout.GenerateTableLayout(children, ownerWidth, ownerHeight, 0f, out pageTableLayout);
				WordRenderer.AssertRectangleColumns(rectangle, pageTableLayout);
				this.m_writer.WriteTableBegin(left, true);
				RPLElementStyle style = rectangle.ElementProps.Style;
				BorderContext borderContext = new BorderContext();
				BorderContext borderContext2 = null;
				if (ignoreStyles)
				{
					borderContext2 = new BorderContext();
				}
				else
				{
					borderContext2 = this.HasBorders(style, parentBorderContext);
					this.RenderTableProperties(style, !inTablix, parentBorderContext);
				}
				if (pageTableLayout != null && (!pageTableLayout.BandTable || !this.m_writer.CanBand))
				{
					float[] array = new float[pageTableLayout.NrCols];
					for (int i = 0; i < pageTableLayout.NrCols; i++)
					{
						PageTableCell cell = pageTableLayout.GetCell(0, i);
						array[i] = cell.DXValue.Value;
					}
					bool flag = false;
					for (int j = 0; j < pageTableLayout.NrRows; j++)
					{
						float num = left;
						float value = pageTableLayout.GetCell(j, 0).DYValue.Value;
						flag = false;
						this.m_writer.WriteTableRowBegin(left, value, array);
						this.m_writer.IgnoreRowHeight(canGrow);
						borderContext.Top = (j == 0 && borderContext2.Top);
						for (int k = 0; k < pageTableLayout.NrCols; k++)
						{
							PageTableCell cell2 = pageTableLayout.GetCell(j, k);
							borderContext.Left = (k == 0 && borderContext2.Left);
							borderContext.Bottom = (borderContext2.Bottom && j + cell2.RowSpan >= pageTableLayout.NrRows);
							borderContext.Right = (borderContext2.Right && k + cell2.ColSpan >= pageTableLayout.NrCols);
							this.m_writer.WriteTableCellBegin(k, pageTableLayout.NrCols, cell2.FirstVertMerge, cell2.FirstHorzMerge, cell2.VertMerge, cell2.HorzMerge);
							this.m_writer.ApplyCellBorderContext(borderContext);
							flag = this.RenderRectangleItemAndLines(rectangle, borderContext, k, cell2, text, num, flag);
							this.m_writer.WriteTableCellEnd(k, borderContext, !cell2.InUse);
							num += array[k];
						}
						if (value > this.m_pageHeight && flag)
						{
							this.m_writer.IgnoreRowHeight(true);
						}
						this.m_writer.WriteTableRowEnd();
					}
				}
				else if (children.Length == 1)
				{
					RPLItemMeasurement rPLItemMeasurement = children[0];
					this.m_writer.WriteTableRowBegin(left, rPLItemMeasurement.Height, new float[1]
					{
						rPLItemMeasurement.Width
					});
					this.m_writer.WriteTableCellBegin(0, 1, false, false, false, false);
					RPLElement element = rPLItemMeasurement.Element;
					if (element.ElementProps.Definition.ID == text)
					{
						this.RenderBookmarksLabels(rectangle);
					}
					this.m_writer.ApplyCellBorderContext(borderContext2);
					this.RenderReportItem(element, rPLItemMeasurement, 0, left, borderContext2, false);
					this.m_writer.WriteTableCellEnd(0, borderContext2, false);
					this.m_writer.WriteTableRowEnd();
				}
				else
				{
					borderContext.Left = borderContext2.Left;
					borderContext.Right = borderContext2.Right;
					for (int l = 0; l < children.Length; l++)
					{
						borderContext.Top = (l == 0 && borderContext2.Top);
						borderContext.Bottom = (l == children.Length - 1 && borderContext2.Bottom);
						RPLItemMeasurement rPLItemMeasurement2 = children[l];
						RPLElement element2 = rPLItemMeasurement2.Element;
						if (element2.ElementProps.Definition.ID == text)
						{
							this.RenderBookmarksLabels(rectangle);
						}
						this.m_writer.WriteTableRowBegin(0f, rPLItemMeasurement2.Height, new float[1]
						{
							rPLItemMeasurement2.Width
						});
						this.m_writer.WriteTableCellBegin(0, 1, false, false, false, false);
						this.m_writer.ApplyCellBorderContext(borderContext);
						this.RenderReportItem(element2, rPLItemMeasurement2, 0, left, borderContext, false);
						this.m_writer.WriteTableCellEnd(0, borderContext, false);
						this.m_writer.WriteTableRowEnd();
					}
				}
				this.m_writer.WriteTableEnd();
				this.m_labelLevel--;
				return true;
			}
			return false;
		}

		protected bool RenderRectangleItem(int y, PageTableCell cell, BorderContext borderContext, string linkToChildId, RPLContainer rectangle, float runningLeft, bool rowUsed)
		{
			if (cell.InUse)
			{
				RPLElement element = cell.Measurement.Element;
				if (element.ElementProps.Definition.ID == linkToChildId)
				{
					this.RenderBookmarksLabels(rectangle);
				}
				this.RenderReportItem(element, cell.Measurement, y, runningLeft, borderContext, false);
				if (cell.RowSpan == 1 && (element is RPLTablix || element is RPLSubReport || element is RPLRectangle))
				{
					this.m_writer.IgnoreRowHeight(true);
				}
				rowUsed = true;
			}
			else if (cell.Eaten)
			{
				rowUsed = true;
			}
			return rowUsed;
		}

		protected void RenderLines(int cellIndex, PageTableCell cell, BorderContext borderContext)
		{
			if (cell.BorderLeft != null)
			{
				byte lineStyleCode = 6;
				byte widthCode = 11;
				byte colorCode = 1;
				this.RenderLine(cellIndex, cell.BorderLeft, lineStyleCode, widthCode, colorCode, borderContext.Left);
			}
			if (cell.BorderRight != null)
			{
				byte lineStyleCode2 = 7;
				byte widthCode2 = 12;
				byte colorCode2 = 2;
				this.RenderLine(cellIndex, cell.BorderRight, lineStyleCode2, widthCode2, colorCode2, borderContext.Right);
			}
			if (cell.BorderTop != null)
			{
				byte lineStyleCode3 = 8;
				byte widthCode3 = 13;
				byte colorCode3 = 3;
				this.RenderLine(cellIndex, cell.BorderTop, lineStyleCode3, widthCode3, colorCode3, borderContext.Top);
			}
			if (cell.BorderBottom != null)
			{
				byte lineStyleCode4 = 9;
				byte widthCode4 = 14;
				byte colorCode4 = 4;
				this.RenderLine(cellIndex, cell.BorderBottom, lineStyleCode4, widthCode4, colorCode4, borderContext.Bottom);
			}
		}

		private void RenderLine(int cellIndex, RPLLine line, byte lineStyleCode, byte widthCode, byte colorCode, bool onlyLabel)
		{
			if (!onlyLabel)
			{
				RPLElementStyle style = line.ElementProps.Style;
				object value = style[5];
				object value2 = style[0];
				object value3 = style[10];
				this.m_writer.AddCellStyleProp(cellIndex, lineStyleCode, value);
				this.m_writer.AddCellStyleProp(cellIndex, colorCode, value2);
				this.m_writer.AddCellStyleProp(cellIndex, widthCode, value3);
			}
			this.RenderBookmarksLabels(line);
		}

		private byte[] CreateImageBuf(RPLImageData imgData)
		{
			if (imgData.ImageDataOffset >= 0)
			{
				return this.m_rplReport.GetImage(imgData.ImageDataOffset);
			}
			if (imgData.ImageData != null)
			{
				return imgData.ImageData;
			}
			return null;
		}

		private bool HasAction(RPLAction action)
		{
			if (action.BookmarkLink == null && (action.DrillthroughUrl == null || this.m_omitDrillthroughs))
			{
				if (action.Hyperlink != null)
				{
					return !this.m_omitHyperlinks;
				}
				return false;
			}
			return true;
		}

		private bool HasAction(RPLActionInfo actionInfo)
		{
			if (actionInfo != null)
			{
				return this.HasAction(actionInfo.Actions[0]);
			}
			return false;
		}

		private void WriteBeginTableRowCell(RPLItemMeasurement measurement, bool notCanGrow)
		{
			this.m_writer.WriteTableBegin(0f, true);
			this.m_writer.WriteTableRowBegin(0f, measurement.Height, new float[1]
			{
				measurement.Width
			});
			if (notCanGrow)
			{
				this.m_writer.SetWriteExactRowHeight(true);
			}
			this.m_writer.WriteTableCellBegin(0, 1, false, false, false, false);
		}

		private void WriteEndCellRowTable(BorderContext borderContext)
		{
			this.m_writer.WriteTableCellEnd(0, borderContext, false);
			this.m_writer.WriteTableRowEnd();
			this.m_writer.WriteTableEnd();
		}

		protected void CachePage(ref bool pageCached, List<RPLReport> rplReportCache)
		{
			if (!pageCached)
			{
				rplReportCache.Add(this.m_rplReport);
				pageCached = true;
			}
		}

		protected bool SetFirstPageDimensions(bool firstPage, RPLPageContent pageContent, ref RPLPageLayout rplPageLayout, ref float leftMargin, ref float rightMargin, ref float width, ref string title, ref string author, ref string description)
		{
			if (firstPage)
			{
				rplPageLayout = pageContent.PageLayout;
				leftMargin = rplPageLayout.MarginLeft;
				rightMargin = rplPageLayout.MarginRight;
				if (Word97Writer.FixMargins(rplPageLayout.PageWidth, ref leftMargin, ref rightMargin) && RSTrace.RenderingTracer.TraceVerbose)
				{
					RSTrace.RenderingTracer.Trace("The left or right margin is either <0 or the sum exceeds the page width.");
				}
				this.m_pageHeight = rplPageLayout.PageHeight;
				width = rplPageLayout.PageWidth;
				float pageWidth = rplPageLayout.PageWidth;
				author = this.m_rplReport.Author;
				title = this.m_rplReport.ReportName;
				description = this.m_rplReport.Description;
				firstPage = false;
			}
			return firstPage;
		}

		protected float RevisePageDimensions(float leftMargin, float rightMargin, float width, float bodyWidth, AutoFit initialAutoFit)
		{
			if (!this.m_deviceInfo.FixedPageWidth)
			{
				float num = bodyWidth + leftMargin + rightMargin;
				if (width < num)
				{
					width = num;
				}
			}
			if (width > 558.79998779296875)
			{
				width = 558.8f;
			}
			if (initialAutoFit == AutoFit.Default)
			{
				if (bodyWidth > 558.79998779296875 - (leftMargin + rightMargin))
				{
					this.m_writer.AutoFit = AutoFit.Never;
				}
				else
				{
					this.m_writer.AutoFit = AutoFit.True;
				}
			}
			return width;
		}

		protected void RenderHeaderBetweenSections(RPLReportSection section, bool firstSection)
		{
			if (!firstSection && section.Header != null)
			{
				RPLHeaderFooter rPLHeaderFooter = section.Header.Element as RPLHeaderFooter;
				if (rPLHeaderFooter != null && (rPLHeaderFooter.ElementPropsDef as RPLHeaderFooterPropsDef).PrintBetweenSections)
				{
					this.m_needsToResetTextboxes = true;
					this.m_inHeaderFooter = true;
					this.RenderRectangle(rPLHeaderFooter, 0f, false, section.Header, new BorderContext(), false, true);
					this.m_inHeaderFooter = false;
					this.m_needsToResetTextboxes = false;
				}
			}
		}

		protected void RenderBodyContent(float bodyWidth, RPLItemMeasurement bodyMeasurement)
		{
			RPLMeasurement rPLMeasurement = new RPLMeasurement(bodyMeasurement);
			rPLMeasurement.Width = bodyWidth;
			this.RenderRectangle((RPLContainer)bodyMeasurement.Element, 0f, false, rPLMeasurement, new BorderContext(), false, true);
		}

		protected RPLReportSection AdvanceToNextSection(RPLPageContent pageContent, RPLReportSection section, ref bool firstSection, SectionEntry lastSection, RPLHeaderFooter footer, SectionEntry se)
		{
			if (pageContent.HasNextReportSection())
			{
				if (footer == null && se == null)
				{
					se = lastSection;
					if (se.FooterMeasurement != null)
					{
						footer = (section.Footer.Element as RPLHeaderFooter);
					}
				}
				if (footer != null && (footer.ElementPropsDef as RPLHeaderFooterPropsDef).PrintBetweenSections)
				{
					this.m_needsToResetTextboxes = true;
					this.m_inHeaderFooter = true;
					this.RenderRectangle(footer, 0f, false, section.Footer, new BorderContext(), false, true);
					this.m_inHeaderFooter = false;
					this.m_needsToResetTextboxes = false;
				}
				this.m_writer.WriteEndSection();
				firstSection = false;
			}
			section = pageContent.GetNextReportSection();
			return section;
		}

		protected void FinishRendering(List<RPLReport> rplReportCache, string title, string author, string description)
		{
			for (int i = 0; i < rplReportCache.Count; i++)
			{
				rplReportCache[i].Release();
				rplReportCache[i] = null;
			}
			if (this.m_rplReport != null)
			{
				this.m_rplReport.Release();
				this.m_rplReport = null;
			}
			this.m_writer.Finish(title, author, description);
		}
	}
}
