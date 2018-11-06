using System;
using System.ComponentModel;

namespace AspNetCore.Reporting.Chart.WebForms
{
	internal enum ChartElementType
	{
		Nothing,
		DataPoint,
		Axis,
		PlottingArea,
		LegendArea,
		LegendItem,
		Gridlines,
		StripLines,
		TickMarks,
		Title,
		AxisLabels,
		AxisTitle,
		SBThumbTracker,
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		SBSmallDecrement,
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		SBSmallIncrement,
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		SBLargeDecrement,
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		SBLargeIncrement,
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This item is not supported in web environment")]
		[Browsable(false)]
		SBZoomReset,
		Annotation,
		DataPointLabel,
		AxisLabelImage,
		LegendTitle,
		LegendHeader
	}
}
