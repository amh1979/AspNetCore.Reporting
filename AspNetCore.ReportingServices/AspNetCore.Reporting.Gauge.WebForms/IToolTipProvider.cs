namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal interface IToolTipProvider
	{
		string GetToolTip(HitTestResult ht);
	}
}
