using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class ReportItemProcessing : MemberBase
	{
		internal string DefinitionName;

		internal string Label;

		internal string Bookmark;

		internal string Tooltip;

		internal ReportSize Height;

		internal ReportSize Width;

		internal ReportSize Top;

		internal ReportSize Left;

		internal int ZIndex;

		internal bool Hidden;

		internal SharedHiddenState SharedHidden = SharedHiddenState.Never;

		internal DataValueInstanceList SharedStyles;

		internal DataValueInstanceList NonSharedStyles;

		internal ReportItemProcessing()
			: base(true)
		{
		}

		internal ReportItemProcessing DeepClone()
		{
			ReportItemProcessing reportItemProcessing = new ReportItemProcessing();
			if (this.DefinitionName != null)
			{
				reportItemProcessing.DefinitionName = string.Copy(this.DefinitionName);
			}
			if (this.Label != null)
			{
				reportItemProcessing.Label = string.Copy(this.Label);
			}
			if (this.Bookmark != null)
			{
				reportItemProcessing.Bookmark = string.Copy(this.Bookmark);
			}
			if (this.Tooltip != null)
			{
				reportItemProcessing.Tooltip = string.Copy(this.Tooltip);
			}
			if (this.Height != null)
			{
				reportItemProcessing.Height = this.Height.DeepClone();
			}
			if (this.Width != null)
			{
				reportItemProcessing.Width = this.Width.DeepClone();
			}
			if (this.Top != null)
			{
				reportItemProcessing.Top = this.Top.DeepClone();
			}
			if (this.Left != null)
			{
				reportItemProcessing.Left = this.Left.DeepClone();
			}
			reportItemProcessing.ZIndex = this.ZIndex;
			reportItemProcessing.Hidden = this.Hidden;
			reportItemProcessing.SharedHidden = this.SharedHidden;
			Global.Tracer.Assert(this.SharedStyles == null && null == this.NonSharedStyles);
			return reportItemProcessing;
		}
	}
}
