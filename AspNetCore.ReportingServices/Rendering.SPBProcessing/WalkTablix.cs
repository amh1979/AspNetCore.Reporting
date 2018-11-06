using AspNetCore.ReportingServices.OnDemandReportRendering;

namespace AspNetCore.ReportingServices.Rendering.SPBProcessing
{
	internal abstract class WalkTablix
	{
		internal enum State
		{
			RowMembers,
			ColMembers,
			DetailRows
		}

		internal static int AddMembersToCurrentPage(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember memberParent, int rowMemberIndexCell, State state, bool createDetail, bool noRows, PageContext context, bool useForPageHFEval, Interactivity interactivity)
		{
			TablixMemberCollection tablixMemberCollection = null;
			if (memberParent == null)
			{
				switch (state)
				{
				case State.RowMembers:
					tablixMemberCollection = tablix.RowHierarchy.MemberCollection;
					break;
				case State.ColMembers:
					WalkTablix.AddCornerToCurrentPage(tablix.Corner, context, useForPageHFEval, interactivity);
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
				if (state == State.RowMembers)
				{
					WalkTablix.AddMembersToCurrentPage(tablix, null, memberParent.MemberCellIndex, State.DetailRows, createDetail, noRows, context, useForPageHFEval, interactivity);
				}
				else if (createDetail)
				{
					WalkTablix.AddDetailCellToCurrentPage(tablix, memberParent.MemberCellIndex, rowMemberIndexCell, context, useForPageHFEval, interactivity);
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
				if (noRows && tablixMember.HideIfNoRows)
				{
					if (interactivity == null)
					{
						continue;
					}
					useForPageHFEval2 = false;
				}
				flag = true;
				tablixMemberInstance = tablixMember.Instance;
				if (tablixMember.IsStatic)
				{
					flag2 = WalkTablix.WalkTablixMember(tablixMember, ref useForPageHFEval2, interactivity);
				}
				else
				{
					tablixDynamicMemberInstance = (TablixDynamicMemberInstance)tablixMemberInstance;
					tablixDynamicMemberInstance.ResetContext();
					flag = tablixDynamicMemberInstance.MoveNext();
					if (flag)
					{
						flag2 = WalkTablix.WalkTablixMember(tablixMember, ref useForPageHFEval2, interactivity);
					}
				}
				while (flag)
				{
					if (flag2)
					{
						int num2 = WalkTablix.AddMembersToCurrentPage(tablix, tablixMember, rowMemberIndexCell, state, createDetail, noRows, context, useForPageHFEval2, interactivity);
						if (state != State.DetailRows)
						{
							if (interactivity != null)
							{
								interactivity.RegisterGroupLabel(tablixMember.Group, context);
							}
							if (tablixMember.TablixHeader != null)
							{
								if (num2 > 0)
								{
									RegisterItem.RegisterHiddenItem(tablixMember.TablixHeader.CellContents.ReportItem, context, useForPageHFEval2, interactivity);
									num++;
								}
								else if (interactivity != null)
								{
									RegisterItem.RegisterHiddenItem(tablixMember.TablixHeader.CellContents.ReportItem, context, false, interactivity);
								}
							}
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
							flag2 = WalkTablix.WalkTablixMember(tablixMember, ref useForPageHFEval2, interactivity);
						}
					}
				}
				tablixMemberInstance = null;
			}
			return num;
		}

		private static void AddCornerToCurrentPage(TablixCorner corner, PageContext context, bool useForPageHFEval, Interactivity interactivity)
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
							RegisterItem.RegisterHiddenItem(((ReportElementCollectionBase<TablixCornerCell>)tablixCornerRow)[j].CellContents.ReportItem, context, useForPageHFEval, interactivity);
						}
					}
				}
			}
		}

		private static bool WalkTablixMember(TablixMember tablixMember, ref bool useForPageHFEval, Interactivity interactivity)
		{
			if (useForPageHFEval && tablixMember.Visibility != null && tablixMember.Visibility.ToggleItem == null && tablixMember.Visibility.Hidden.IsExpression && tablixMember.Instance.Visibility.CurrentlyHidden)
			{
				useForPageHFEval = false;
			}
			if (!useForPageHFEval && interactivity == null)
			{
				return false;
			}
			return true;
		}

		internal static void AddDetailCellToCurrentPage(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix, int colMemberIndexCell, int rowMemberIndexCell, PageContext context, bool useForPageHFEval, Interactivity interactivity)
		{
			if (rowMemberIndexCell >= 0)
			{
				TablixRowCollection rowCollection = tablix.Body.RowCollection;
				TablixCell tablixCell = ((ReportElementCollectionBase<TablixCell>)((ReportElementCollectionBase<TablixRow>)rowCollection)[rowMemberIndexCell])[colMemberIndexCell];
				if (tablixCell != null && tablixCell.CellContents != null)
				{
					RegisterItem.RegisterHiddenItem(tablixCell.CellContents.ReportItem, context, useForPageHFEval, interactivity);
				}
			}
		}
	}
}
