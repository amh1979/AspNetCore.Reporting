using System;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[Flags]
	internal enum LabelsAutoFitStyles
	{
		None = 0,
		IncreaseFont = 1,
		DecreaseFont = 2,
		OffsetLabels = 4,
		LabelsAngleStep30 = 8,
		LabelsAngleStep45 = 0x10,
		LabelsAngleStep90 = 0x20,
		WordWrap = 0x40
	}
}
