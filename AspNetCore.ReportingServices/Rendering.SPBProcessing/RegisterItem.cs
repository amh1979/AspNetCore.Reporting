using AspNetCore.ReportingServices.OnDemandReportRendering;

namespace AspNetCore.ReportingServices.Rendering.SPBProcessing
{
	internal abstract class RegisterItem
	{
		internal static void RegisterPageItem(PageItem pageItem, PageContext pageContext, bool useForPageHFEval, Interactivity interactivity)
		{
			if (!useForPageHFEval && interactivity == null)
			{
				return;
			}
			ReportItem source = pageItem.Source;
			bool flag = false;
			if (useForPageHFEval)
			{
				if (pageItem.ItemState == PageItem.State.OnPageHidden)
				{
					if (source.Visibility.ToggleItem != null || !source.Visibility.Hidden.IsExpression)
					{
						flag = true;
					}
				}
				else
				{
					AspNetCore.ReportingServices.OnDemandReportRendering.TextBox textBox = source as AspNetCore.ReportingServices.OnDemandReportRendering.TextBox;
					if (textBox != null)
					{
						((TextBoxInstance)textBox.Instance).AddToCurrentPage();
					}
				}
			}
			if (interactivity != null && pageItem.ItemState != PageItem.State.OnPageHidden)
			{
				interactivity = null;
			}
			if (!flag && interactivity == null)
			{
				return;
			}
			RegisterItem.RegisterHiddenItem(source, pageContext, flag, interactivity);
		}

		internal static void RegisterHiddenItem(ReportItem reportItem, PageContext pageContext, bool useForPageHFEval, Interactivity interactivity)
		{
			if (reportItem != null)
			{
				bool flag = false;
				bool flag2 = false;
				if (useForPageHFEval)
				{
					flag = HeaderFooterEval.AddToCurrentPage(reportItem);
				}
				if (interactivity != null && !interactivity.RegisterHiddenItem(reportItem, pageContext))
				{
					interactivity = null;
				}
				if (!flag && interactivity == null)
				{
					return;
				}
				if (reportItem is AspNetCore.ReportingServices.OnDemandReportRendering.TextBox)
				{
					if (flag)
					{
						AspNetCore.ReportingServices.OnDemandReportRendering.TextBox textBox = reportItem as AspNetCore.ReportingServices.OnDemandReportRendering.TextBox;
						if (textBox != null)
						{
							((TextBoxInstance)textBox.Instance).AddToCurrentPage();
						}
					}
				}
				else if (reportItem is AspNetCore.ReportingServices.OnDemandReportRendering.Rectangle)
				{
					RegisterItem.RegisterHiddenItem(((AspNetCore.ReportingServices.OnDemandReportRendering.Rectangle)reportItem).ReportItemCollection, pageContext, flag, interactivity);
				}
				else if (reportItem is AspNetCore.ReportingServices.OnDemandReportRendering.SubReport)
				{
					if (interactivity != null)
					{
						AspNetCore.ReportingServices.OnDemandReportRendering.SubReport subReport = (AspNetCore.ReportingServices.OnDemandReportRendering.SubReport)reportItem;
						SubReportInstance subReportInstance = (SubReportInstance)subReport.Instance;
						if (!subReportInstance.ProcessedWithError && !subReportInstance.NoRows)
						{
							for (int i = 0; i < subReport.Report.ReportSections.Count; i++)
							{
								RegisterItem.RegisterHiddenItem(((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.ReportSection>)subReport.Report.ReportSections)[i].Body.ReportItemCollection, pageContext, false, interactivity);
							}
						}
					}
				}
				else if (reportItem is AspNetCore.ReportingServices.OnDemandReportRendering.Tablix)
				{
					RegisterItem.RegisterHiddenItem((AspNetCore.ReportingServices.OnDemandReportRendering.Tablix)reportItem, pageContext, flag, interactivity);
				}
			}
		}

		private static void RegisterHiddenItem(ReportItemCollection collection, PageContext pageContext, bool useForPageHFEval, Interactivity interactivity)
		{
			if (collection != null && collection.Count != 0)
			{
				for (int i = 0; i < collection.Count; i++)
				{
					RegisterItem.RegisterHiddenItem(((ReportElementCollectionBase<ReportItem>)collection)[i], pageContext, useForPageHFEval, interactivity);
					if (!useForPageHFEval && interactivity.Done)
					{
						break;
					}
				}
			}
		}

		private static void RegisterHiddenItem(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix, PageContext pageContext, bool useForPageHFEval, Interactivity interactivity)
		{
			if (tablix != null)
			{
				bool flag = false;
				if (useForPageHFEval)
				{
					flag = HeaderFooterEval.AddToCurrentPage(tablix);
				}
				if (!flag && interactivity == null)
				{
					return;
				}
				TablixInstance tablixInstance = (TablixInstance)tablix.Instance;
				WalkTablix.AddMembersToCurrentPage(tablix, null, -1, WalkTablix.State.ColMembers, false, tablixInstance.NoRows, pageContext, flag, interactivity);
				WalkTablix.AddMembersToCurrentPage(tablix, null, 0, WalkTablix.State.RowMembers, true, tablixInstance.NoRows, pageContext, flag, interactivity);
			}
		}
	}
}
