using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.SPBProcessing
{
	internal sealed class SubReport : PageItem
	{
		private PageItem[] m_childrenBody;

		private double m_prevPageEnd;

		private int m_bodyIndex = -1;

		private int m_bodiesOnPage;

		private long m_bodyOffset;

		internal SubReport(AspNetCore.ReportingServices.OnDemandReportRendering.SubReport source, PageContext pageContext, bool createForRepeat)
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
		}

		internal void UpdateItemPageState(bool omitBordersOnPageBreaks)
		{
			if (base.m_itemState == State.SpanPages)
			{
				if (omitBordersOnPageBreaks)
				{
					base.m_rplItemState |= 1;
				}
				base.m_itemState = State.OnPage;
			}
			base.m_itemPageSizes.AdjustHeightTo(base.ItemPageSizes.Height - this.m_prevPageEnd);
			this.m_prevPageEnd = 0.0;
		}

		internal override void UpdateItem(PageItemHelper itemHelper)
		{
			if (itemHelper != null)
			{
				base.UpdateItem(itemHelper);
				this.m_prevPageEnd = itemHelper.PrevPageEnd;
			}
		}

		internal override bool CalculatePage(RPLWriter rplWriter, PageItemHelper lastPageInfo, PageContext pageContext, PageItem[] siblings, RepeatWithItem[] repeatWithItems, double parentTopInPage, ref double parentPageHeight, Interactivity interactivity)
		{
			base.AdjustOriginFromItemsAbove(siblings, repeatWithItems);
			if (!this.HitsCurrentPage(pageContext, parentTopInPage))
			{
				return false;
			}
			bool flag = false;
			bool flag2 = false;
			this.CalculateIndexOfFirstBodyOnPage(lastPageInfo, out flag, out flag2);
			ItemSizes itemSizes = null;
			if (flag && base.ResolveItemHiddenState(rplWriter, interactivity, pageContext, false, ref itemSizes))
			{
				parentPageHeight = Math.Max(parentPageHeight, base.m_itemPageSizes.Bottom);
				if (rplWriter != null && base.m_itemRenderSizes == null)
				{
					this.CreateItemRenderSizes(null, pageContext, false);
				}
				return true;
			}
			this.WriteStartItemToStream(rplWriter, pageContext);
			this.CreateChildren(pageContext, lastPageInfo, flag2);
			AspNetCore.ReportingServices.OnDemandReportRendering.SubReport subReport = (AspNetCore.ReportingServices.OnDemandReportRendering.SubReport)base.m_source;
			PageContext pageContext2 = pageContext;
			if (!pageContext2.FullOnPage)
			{
				if (base.IgnorePageBreaks)
				{
					pageContext2 = new PageContext(pageContext, PageContext.PageContextFlags.FullOnPage, PageContext.IgnorePBReasonFlag.Toggled);
				}
				else if (flag && subReport.KeepTogether && !pageContext2.KeepTogether)
				{
					pageContext2 = new PageContext(pageContext);
					pageContext2.KeepTogether = true;
				}
			}
			this.m_bodiesOnPage = 0;
			double num = parentTopInPage + base.m_itemPageSizes.Top;
			double num2 = 0.0;
			double num3 = 0.0;
			double num4 = 0.0;
			bool flag3 = false;
			for (int i = this.m_bodyIndex; i < this.m_childrenBody.Length; i++)
			{
				if (flag3)
				{
					break;
				}
				PageItem pageItem = this.m_childrenBody[i];
				double num5 = 0.0;
				if (flag2)
				{
					pageItem.CalculatePage(rplWriter, lastPageInfo.ChildPage, pageContext2, null, null, num, ref num5, interactivity);
					flag2 = false;
				}
				else
				{
					pageItem.CalculatePage(rplWriter, null, pageContext2, null, null, num, ref num5, interactivity);
				}
				if (pageContext2.CancelPage)
				{
					base.m_itemState = State.Below;
					base.m_rplElement = null;
					this.m_childrenBody = null;
					return false;
				}
				if (pageItem.ItemState == State.TopNextPage && i == 0)
				{
					base.m_itemState = State.TopNextPage;
					this.m_bodyIndex = -1;
					return false;
				}
				this.m_bodiesOnPage++;
				num += num5;
				num2 += num5;
				base.m_itemState = State.OnPage;
				if (!pageContext2.FullOnPage)
				{
					this.m_prevPageEnd = num2;
					if (pageItem.ItemState != State.OnPage)
					{
						if (pageItem.ItemState == State.OnPagePBEnd && i == this.m_childrenBody.Length - 1)
						{
							base.m_itemState = State.OnPagePBEnd;
						}
						else
						{
							if (pageItem.ItemState == State.Below)
							{
								this.m_bodiesOnPage--;
							}
							base.m_itemState = State.SpanPages;
							this.m_prevPageEnd = 0.0;
						}
					}
					if (base.m_itemState == State.SpanPages || base.m_itemState == State.OnPagePBEnd)
					{
						flag3 = true;
					}
					else if ((RoundedDouble)num >= pageContext2.PageHeight)
					{
						flag3 = true;
						if (base.m_itemState == State.OnPage && i < this.m_childrenBody.Length - 1)
						{
							base.m_itemState = State.SpanPages;
						}
					}
				}
				if (rplWriter != null)
				{
					num3 = Math.Max(num3, pageItem.ItemRenderSizes.Width);
					num4 += pageItem.ItemRenderSizes.Height;
				}
			}
			if (itemSizes == null)
			{
				base.m_itemPageSizes.AdjustHeightTo(this.m_childrenBody[this.m_bodyIndex + this.m_bodiesOnPage - 1].ItemPageSizes.Bottom);
			}
			else
			{
				itemSizes.AdjustHeightTo(this.m_childrenBody[this.m_bodyIndex + this.m_bodiesOnPage - 1].ItemPageSizes.Bottom);
			}
			if (rplWriter != null)
			{
				this.CreateItemRenderSizes(itemSizes, pageContext2, false);
				base.m_itemRenderSizes.AdjustWidthTo(num3);
				base.m_itemRenderSizes.AdjustHeightTo(num4);
				this.WriteEndItemToStream(rplWriter, pageContext2);
			}
			if (base.m_itemState != State.SpanPages)
			{
				parentPageHeight = Math.Max(parentPageHeight, base.m_itemPageSizes.Bottom);
			}
			else
			{
				if (subReport.OmitBorderOnPageBreak)
				{
					base.m_rplItemState |= 2;
				}
				parentPageHeight = Math.Max(parentPageHeight, base.m_itemPageSizes.Top + num2);
			}
			this.ReleaseBodyChildrenOnPage();
			this.m_bodyIndex += this.m_bodiesOnPage - 1;
			return true;
		}

		internal void WriteStartItemToStream(RPLWriter rplWriter, PageContext pageContext)
		{
			if (rplWriter != null)
			{
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				if (binaryWriter != null)
				{
					if (pageContext.VersionPicker == RPLVersionEnum.RPL2008 || pageContext.VersionPicker == RPLVersionEnum.RPL2008WithImageConsolidation)
					{
						this.WriteStartItemToRPLStream2008(rplWriter, binaryWriter, pageContext);
					}
					else
					{
						this.WriteStartItemToRPLStream(rplWriter, binaryWriter, pageContext);
					}
				}
				else
				{
					this.WriteStartItemToRPLOM(rplWriter, pageContext);
				}
			}
		}

		internal void WriteEndItemToStream(RPLWriter rplWriter, PageContext pageContext)
		{
			if (rplWriter != null)
			{
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				if (binaryWriter != null)
				{
					if (pageContext.VersionPicker == RPLVersionEnum.RPL2008 || pageContext.VersionPicker == RPLVersionEnum.RPL2008WithImageConsolidation)
					{
						this.WriteEndItemToRPLStream2008(binaryWriter);
					}
					else
					{
						this.WriteEndItemToRPLStream(binaryWriter);
					}
				}
				else
				{
					this.WriteEndItemToRPLOM(rplWriter);
				}
			}
		}

		internal override void WriteCustomSharedItemProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext)
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.SubReport subReport = (AspNetCore.ReportingServices.OnDemandReportRendering.SubReport)base.m_source;
			if (subReport.ReportName != null)
			{
				spbifWriter.Write((byte)15);
				spbifWriter.Write(subReport.ReportName);
			}
		}

		internal override void WriteCustomSharedItemProps(RPLElementPropsDef sharedProps, RPLWriter rplWriter, PageContext pageContext)
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.SubReport subReport = (AspNetCore.ReportingServices.OnDemandReportRendering.SubReport)base.m_source;
			if (subReport.ReportName != null)
			{
				((RPLSubReportPropsDef)sharedProps).ReportName = subReport.ReportName;
			}
		}

		internal override void WriteCustomNonSharedItemProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext)
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.Report report = ((AspNetCore.ReportingServices.OnDemandReportRendering.SubReport)base.m_source).Report;
			if (report != null && report.Language != null)
			{
				string text = null;
				if (report.Language.IsExpression)
				{
					ReportInstance instance = report.Instance;
					text = instance.Language;
				}
				else
				{
					text = report.Language.Value;
				}
				if (text != null)
				{
					spbifWriter.Write((byte)11);
					spbifWriter.Write(text);
				}
			}
		}

		internal override void WriteCustomNonSharedItemProps(RPLElementProps nonSharedProps, RPLWriter rplWriter, PageContext pageContext)
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.Report report = ((AspNetCore.ReportingServices.OnDemandReportRendering.SubReport)base.m_source).Report;
			if (report != null && report.Language != null)
			{
				string text = null;
				if (report.Language.IsExpression)
				{
					ReportInstance instance = report.Instance;
					text = instance.Language;
				}
				else
				{
					text = report.Language.Value;
				}
				((RPLSubReportProps)nonSharedProps).Language = text;
			}
		}

		internal override void WritePaginationInfo(BinaryWriter reportPageInfo)
		{
			if (reportPageInfo != null)
			{
				reportPageInfo.Write((byte)4);
				base.WritePaginationInfoProperties(reportPageInfo);
				reportPageInfo.Write((byte)23);
				reportPageInfo.Write(this.m_bodyIndex);
				reportPageInfo.Write((byte)11);
				reportPageInfo.Write(this.m_prevPageEnd);
				if (this.m_childrenBody != null && this.m_childrenBody[this.m_bodyIndex] != null)
				{
					reportPageInfo.Write((byte)19);
					this.m_childrenBody[this.m_bodyIndex].WritePaginationInfo(reportPageInfo);
				}
				reportPageInfo.Write((byte)255);
			}
		}

		internal override PageItemHelper WritePaginationInfo()
		{
			PageItemHelper pageItemHelper = new PageItemHelper(4);
			base.WritePaginationInfoProperties(pageItemHelper);
			pageItemHelper.BodyIndex = this.m_bodyIndex;
			pageItemHelper.PrevPageEnd = this.m_prevPageEnd;
			if (this.m_childrenBody != null && this.m_childrenBody[this.m_bodyIndex] != null)
			{
				pageItemHelper.ChildPage = this.m_childrenBody[this.m_bodyIndex].WritePaginationInfo();
			}
			return pageItemHelper;
		}

		private void CalculateIndexOfFirstBodyOnPage(PageItemHelper lastPageInfo, out bool firstPage, out bool needsFirstBodyUpdate)
		{
			firstPage = false;
			needsFirstBodyUpdate = false;
			if (lastPageInfo != null)
			{
				this.m_bodyIndex = lastPageInfo.BodyIndex;
				if (lastPageInfo.ChildPage != null)
				{
					needsFirstBodyUpdate = true;
				}
				else
				{
					if (this.m_bodyIndex < 0)
					{
						firstPage = true;
					}
					this.m_bodyIndex++;
				}
			}
			else
			{
				this.m_bodyIndex = 0;
				firstPage = true;
			}
		}

		private void CreateChildren(PageContext pageContext, PageItemHelper lastPageInfo, bool needsFirstBodyUpdate)
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.SubReport subReport = (AspNetCore.ReportingServices.OnDemandReportRendering.SubReport)base.m_source;
			ReportSectionCollection reportSections = subReport.Report.ReportSections;
			if (this.m_childrenBody == null)
			{
				this.m_childrenBody = new PageItem[reportSections.Count];
				for (int i = this.m_bodyIndex; i < reportSections.Count; i++)
				{
					this.m_childrenBody[i] = new ReportBody(((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.ReportSection>)subReport.Report.ReportSections)[i].Body, ((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.ReportSection>)subReport.Report.ReportSections)[i].Width, pageContext);
				}
			}
			if (needsFirstBodyUpdate)
			{
				this.m_childrenBody[this.m_bodyIndex].UpdateItem(lastPageInfo.ChildPage);
				this.UpdateItemPageState(subReport.OmitBorderOnPageBreak);
			}
		}

		private void ReleaseBodyChildrenOnPage()
		{
			for (int i = 0; i < this.m_bodiesOnPage; i++)
			{
				PageItem pageItem = this.m_childrenBody[i + this.m_bodyIndex];
				if (pageItem != null)
				{
					if (i < this.m_bodiesOnPage - 1)
					{
						pageItem = null;
					}
					else if (pageItem.ItemState == State.OnPage || pageItem.ItemState == State.OnPagePBEnd)
					{
						pageItem = null;
					}
				}
			}
		}

		private void WriteStartItemToRPLStream(RPLWriter rplWriter, BinaryWriter spbifWriter, PageContext pageContext)
		{
			Stream baseStream = spbifWriter.BaseStream;
			base.m_offset = baseStream.Position;
			spbifWriter.Write((byte)12);
			this.WriteElementProps(spbifWriter, rplWriter, pageContext, base.m_offset + 1);
		}

		private void WriteStartItemToRPLOM(RPLWriter rplWriter, PageContext pageContext)
		{
			base.m_rplElement = new RPLSubReport();
			this.WriteElementProps(base.m_rplElement.ElementProps, rplWriter, pageContext);
		}

		private void WriteStartItemToRPLStream2008(RPLWriter rplWriter, BinaryWriter spbifWriter, PageContext pageContext)
		{
			Stream baseStream = spbifWriter.BaseStream;
			base.m_offset = baseStream.Position;
			spbifWriter.Write((byte)12);
			this.WriteElementProps(spbifWriter, rplWriter, pageContext, base.m_offset + 1);
			this.m_bodyOffset = baseStream.Position;
			spbifWriter.Write((byte)6);
			spbifWriter.Write((byte)15);
			spbifWriter.Write((byte)0);
			spbifWriter.Write((byte)1);
			spbifWriter.Write(base.m_source.ID + "_SBID");
			spbifWriter.Write((byte)255);
			spbifWriter.Write((byte)1);
			spbifWriter.Write((byte)255);
			spbifWriter.Write((byte)255);
		}

		private void WriteEndItemToRPLStream(BinaryWriter spbifWriter)
		{
			Stream baseStream = spbifWriter.BaseStream;
			long position = baseStream.Position;
			spbifWriter.Write((byte)16);
			spbifWriter.Write(base.m_offset);
			spbifWriter.Write(this.m_bodiesOnPage);
			for (int i = 0; i < this.m_bodiesOnPage; i++)
			{
				this.m_childrenBody[i + this.m_bodyIndex].WritePageItemRenderSizes(spbifWriter);
			}
			base.m_offset = baseStream.Position;
			spbifWriter.Write((byte)254);
			spbifWriter.Write(position);
			spbifWriter.Write((byte)255);
		}

		private void WriteEndItemToRPLOM(RPLWriter rplWriter)
		{
			RPLItemMeasurement[] array = new RPLItemMeasurement[this.m_bodiesOnPage];
			((RPLContainer)base.m_rplElement).Children = array;
			for (int i = 0; i < this.m_bodiesOnPage; i++)
			{
				array[i] = this.m_childrenBody[i + this.m_bodyIndex].WritePageItemRenderSizes();
			}
		}

		private void WriteEndItemToRPLStream2008(BinaryWriter spbifWriter)
		{
			double num = 0.0;
			double num2 = 0.0;
			Stream baseStream = spbifWriter.BaseStream;
			long position = baseStream.Position;
			spbifWriter.Write((byte)16);
			spbifWriter.Write(this.m_bodyOffset);
			spbifWriter.Write(this.m_bodiesOnPage);
			for (int i = 0; i < this.m_bodiesOnPage; i++)
			{
				spbifWriter.Write((float)this.m_childrenBody[i + this.m_bodyIndex].ItemRenderSizes.Left);
				spbifWriter.Write((float)(this.m_childrenBody[i + this.m_bodyIndex].ItemRenderSizes.Top + num2));
				spbifWriter.Write((float)this.m_childrenBody[i + this.m_bodyIndex].ItemRenderSizes.Width);
				spbifWriter.Write((float)this.m_childrenBody[i + this.m_bodyIndex].ItemRenderSizes.Height);
				spbifWriter.Write(0);
				spbifWriter.Write((byte)0);
				spbifWriter.Write(this.m_childrenBody[i + this.m_bodyIndex].Offset);
				num = Math.Max(num, this.m_childrenBody[i + this.m_bodyIndex].ItemRenderSizes.Width);
				num2 += this.m_childrenBody[i + this.m_bodyIndex].ItemRenderSizes.Height;
			}
			this.m_bodyOffset = baseStream.Position;
			spbifWriter.Write((byte)254);
			spbifWriter.Write(position);
			spbifWriter.Write((byte)255);
			long position2 = baseStream.Position;
			spbifWriter.Write((byte)16);
			spbifWriter.Write(base.m_offset);
			spbifWriter.Write(1);
			spbifWriter.Write(0f);
			spbifWriter.Write(0f);
			spbifWriter.Write((float)num);
			spbifWriter.Write((float)num2);
			spbifWriter.Write(0);
			spbifWriter.Write((byte)0);
			spbifWriter.Write(this.m_bodyOffset);
			base.m_offset = baseStream.Position;
			spbifWriter.Write((byte)254);
			spbifWriter.Write(position2);
			spbifWriter.Write((byte)255);
		}
	}
}
