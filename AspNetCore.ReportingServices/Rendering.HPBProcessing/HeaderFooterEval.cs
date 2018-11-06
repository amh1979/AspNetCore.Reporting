using AspNetCore.ReportingServices.OnDemandReportRendering;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.HPBProcessing
{
	internal static class HeaderFooterEval
	{
		private enum TablixState
		{
			RowMembers,
			ColMembers,
			DetailRows
		}

		internal static void CollectTextBoxes(ReportItem reportItem, PageContext pageContext, bool useForPageHFEval, Dictionary<string, List<object>> textBoxes)
		{
			if (reportItem != null && useForPageHFEval && HeaderFooterEval.ShouldBeCollected(reportItem))
			{
				if (reportItem is AspNetCore.ReportingServices.OnDemandReportRendering.TextBox)
				{
					AspNetCore.ReportingServices.OnDemandReportRendering.TextBox textBox = (AspNetCore.ReportingServices.OnDemandReportRendering.TextBox)reportItem;
					List<object> list = null;
					if (!textBoxes.ContainsKey(textBox.Name))
					{
						list = new List<object>();
						textBoxes[textBox.Name] = list;
					}
					else
					{
						list = textBoxes[textBox.Name];
					}
					list.Add(((TextBoxInstance)textBox.Instance).OriginalValue);
				}
				else if (reportItem is AspNetCore.ReportingServices.OnDemandReportRendering.Rectangle)
				{
					HeaderFooterEval.CollectTextBoxes(((AspNetCore.ReportingServices.OnDemandReportRendering.Rectangle)reportItem).ReportItemCollection, pageContext, true, textBoxes);
				}
				else if (!(reportItem is AspNetCore.ReportingServices.OnDemandReportRendering.SubReport) && reportItem is AspNetCore.ReportingServices.OnDemandReportRendering.Tablix)
				{
					HeaderFooterEval.CollectTextBoxes((AspNetCore.ReportingServices.OnDemandReportRendering.Tablix)reportItem, pageContext, true, textBoxes);
				}
			}
		}

		private static void CollectTextBoxes(ReportItemCollection collection, PageContext pageContext, bool useForPageHFEval, Dictionary<string, List<object>> textBoxes)
		{
			if (collection != null && collection.Count != 0)
			{
				for (int i = 0; i < collection.Count; i++)
				{
					HeaderFooterEval.CollectTextBoxes(((ReportElementCollectionBase<ReportItem>)collection)[i], pageContext, useForPageHFEval, textBoxes);
				}
			}
		}

		private static void CollectTextBoxes(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix, PageContext pageContext, bool useForPageHFEval, Dictionary<string, List<object>> textBoxes)
		{
			if (tablix != null && useForPageHFEval && HeaderFooterEval.ShouldBeCollected(tablix))
			{
				TablixInstance tablixInstance = (TablixInstance)tablix.Instance;
				HeaderFooterEval.CollectTablixMembersContents(tablix, null, -1, TablixState.ColMembers, tablixInstance.NoRows, pageContext, true, textBoxes);
				HeaderFooterEval.CollectTablixMembersContents(tablix, null, 0, TablixState.RowMembers, tablixInstance.NoRows, pageContext, true, textBoxes);
			}
		}

		private static bool ShouldBeCollected(ReportItem reportItem)
		{
			if (reportItem == null)
			{
				return false;
			}
			if (reportItem.Visibility != null && reportItem.Visibility.ToggleItem == null && reportItem.Visibility.Hidden.IsExpression && reportItem.Instance.Visibility.CurrentlyHidden)
			{
				return false;
			}
			return true;
		}

		private static bool ShouldBeCollected(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix)
		{
			TablixInstance tablixInstance = (TablixInstance)tablix.Instance;
			if (tablixInstance.NoRows)
			{
				if (tablix.NoRowsMessage != null)
				{
					string text = null;
					text = ((!tablix.NoRowsMessage.IsExpression) ? tablix.NoRowsMessage.Value : tablixInstance.NoRowsMessage);
					if (text != null)
					{
						return false;
					}
				}
				if (tablix.HideStaticsIfNoRows)
				{
					return false;
				}
			}
			return true;
		}

		private static bool ShouldBeCollected(TablixMember tablixMember, ref bool useForPageHFEval)
		{
			if (useForPageHFEval && tablixMember.Visibility != null && tablixMember.Visibility.ToggleItem == null && tablixMember.Visibility.Hidden.IsExpression && tablixMember.Instance.Visibility.CurrentlyHidden)
			{
				useForPageHFEval = false;
			}
			if (useForPageHFEval)
			{
				return true;
			}
			return false;
		}

		private static int CollectTablixMembersContents(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember memberParent, int rowMemberIndexCell, TablixState state, bool noRows, PageContext context, bool useForPageHFEval, Dictionary<string, List<object>> textBoxes)
		{
			TablixMemberCollection tablixMemberCollection = null;
			if (memberParent == null)
			{
				switch (state)
				{
				case TablixState.RowMembers:
					tablixMemberCollection = tablix.RowHierarchy.MemberCollection;
					break;
				case TablixState.ColMembers:
					HeaderFooterEval.CollectTablixCornerContents(tablix.Corner, context, useForPageHFEval, textBoxes);
					goto default;
				default:
					tablixMemberCollection = tablix.ColumnHierarchy.MemberCollection;
					break;
				}
			}
			else
			{
				tablixMemberCollection = memberParent.Children;
			}
			if (tablixMemberCollection == null)
			{
				if (state == TablixState.RowMembers)
				{
					HeaderFooterEval.CollectTablixMembersContents(tablix, null, memberParent.MemberCellIndex, TablixState.DetailRows, noRows, context, useForPageHFEval, textBoxes);
				}
				else
				{
					HeaderFooterEval.CollectDetailCellContents(tablix, memberParent.MemberCellIndex, rowMemberIndexCell, context, useForPageHFEval, textBoxes);
				}
				if (!useForPageHFEval)
				{
					return 0;
				}
				return 1;
			}
			bool flag = true;
			bool flag2 = true;
			TablixMember tablixMember = null;
			TablixMemberInstance tablixMemberInstance = null;
			TablixDynamicMemberInstance tablixDynamicMemberInstance = null;
			int num = 0;
			bool useForPageHFEval2 = useForPageHFEval;
			for (int i = 0; i < tablixMemberCollection.Count; i++)
			{
				useForPageHFEval2 = useForPageHFEval;
				tablixMember = ((ReportElementCollectionBase<TablixMember>)tablixMemberCollection)[i];
				if (!noRows || !tablixMember.HideIfNoRows)
				{
					flag = true;
					tablixMemberInstance = tablixMember.Instance;
					if (tablixMember.IsStatic)
					{
						flag2 = HeaderFooterEval.ShouldBeCollected(tablixMember, ref useForPageHFEval2);
					}
					else
					{
						tablixDynamicMemberInstance = (TablixDynamicMemberInstance)tablixMemberInstance;
						tablixDynamicMemberInstance.ResetContext();
						flag = tablixDynamicMemberInstance.MoveNext();
						if (flag)
						{
							flag2 = HeaderFooterEval.ShouldBeCollected(tablixMember, ref useForPageHFEval2);
						}
					}
					while (flag)
					{
						if (flag2)
						{
							int num2 = HeaderFooterEval.CollectTablixMembersContents(tablix, tablixMember, rowMemberIndexCell, state, noRows, context, useForPageHFEval2, textBoxes);
							if (state != TablixState.DetailRows && tablixMember.TablixHeader != null && num2 > 0)
							{
								HeaderFooterEval.CollectTextBoxes(tablixMember.TablixHeader.CellContents.ReportItem, context, useForPageHFEval2, textBoxes);
								num++;
							}
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
								useForPageHFEval2 = useForPageHFEval;
								flag2 = HeaderFooterEval.ShouldBeCollected(tablixMember, ref useForPageHFEval2);
							}
						}
					}
					tablixMemberInstance = null;
				}
			}
			return num;
		}

		private static void CollectTablixCornerContents(TablixCorner corner, PageContext context, bool useForPageHFEval, Dictionary<string, List<object>> textBoxes)
		{
			if (corner != null)
			{
				TablixCornerRowCollection rowCollection = corner.RowCollection;
				TablixCornerRow tablixCornerRow = null;
				for (int i = 0; i < rowCollection.Count; i++)
				{
					tablixCornerRow = ((ReportElementCollectionBase<TablixCornerRow>)rowCollection)[i];
					for (int j = 0; j < tablixCornerRow.Count; j++)
					{
						if (((ReportElementCollectionBase<TablixCornerCell>)tablixCornerRow)[j] != null && ((ReportElementCollectionBase<TablixCornerCell>)tablixCornerRow)[j].CellContents != null)
						{
							HeaderFooterEval.CollectTextBoxes(((ReportElementCollectionBase<TablixCornerCell>)tablixCornerRow)[j].CellContents.ReportItem, context, useForPageHFEval, textBoxes);
						}
					}
				}
			}
		}

		private static void CollectDetailCellContents(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix, int colMemberIndexCell, int rowMemberIndexCell, PageContext context, bool useForPageHFEval, Dictionary<string, List<object>> textBoxes)
		{
			if (rowMemberIndexCell >= 0)
			{
				TablixRowCollection rowCollection = tablix.Body.RowCollection;
				TablixCell tablixCell = ((ReportElementCollectionBase<TablixCell>)((ReportElementCollectionBase<TablixRow>)rowCollection)[rowMemberIndexCell])[colMemberIndexCell];
				if (tablixCell != null && tablixCell.CellContents != null)
				{
					HeaderFooterEval.CollectTextBoxes(tablixCell.CellContents.ReportItem, context, useForPageHFEval, textBoxes);
				}
			}
		}
	}
}
