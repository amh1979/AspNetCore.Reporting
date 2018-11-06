using System.Drawing;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeAnnotationSmartLabels_AnnotationSmartLabels")]
	internal class AnnotationSmartLabels : SmartLabels
	{
		internal override bool IsSmartLabelCollide(CommonElements common, ChartGraphics graph, ChartArea area, SmartLabelsStyle smartLabelsStyle, PointF position, SizeF size, PointF markerPosition, StringFormat format, LabelAlignmentTypes labelAlignment, bool checkCalloutLineOverlapping)
		{
			bool result = false;
			if (common.Chart != null)
			{
				foreach (ChartArea chartArea in common.Chart.ChartAreas)
				{
					if (area.Visible)
					{
						chartArea.smartLabels.checkAllCollisions = true;
						if (chartArea.smartLabels.IsSmartLabelCollide(common, graph, area, smartLabelsStyle, position, size, markerPosition, format, labelAlignment, checkCalloutLineOverlapping))
						{
							chartArea.smartLabels.checkAllCollisions = false;
							return true;
						}
						chartArea.smartLabels.checkAllCollisions = false;
					}
				}
			}
			RectangleF labelPosition = base.GetLabelPosition(graph, position, size, format, false);
			bool flag = (byte)((labelAlignment == LabelAlignmentTypes.Center && !smartLabelsStyle.MarkerOverlapping) ? 1 : 0) != 0;
			if (base.checkAllCollisions)
			{
				flag = false;
			}
			foreach (RectangleF smartLabelsPosition in base.smartLabelsPositions)
			{
				if (smartLabelsPosition.IntersectsWith(labelPosition))
				{
					if (!flag)
					{
						return true;
					}
					flag = false;
				}
			}
			return result;
		}

		internal override void AddMarkersPosition(CommonElements common, ChartArea area)
		{
			if (base.smartLabelsPositions.Count == 0 && common != null && common.Chart != null)
			{
				foreach (Annotation annotation in common.Chart.Annotations)
				{
					annotation.AddSmartLabelMarkerPositions(common, base.smartLabelsPositions);
				}
			}
		}

		internal override void DrawCallout(CommonElements common, ChartGraphics graph, ChartArea area, SmartLabelsStyle smartLabelsStyle, PointF labelPosition, SizeF labelSize, StringFormat format, PointF markerPosition, SizeF markerSize, LabelAlignmentTypes labelAlignment)
		{
		}
	}
}
