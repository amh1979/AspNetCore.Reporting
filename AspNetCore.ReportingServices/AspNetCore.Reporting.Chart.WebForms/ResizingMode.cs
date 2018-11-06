namespace AspNetCore.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeResizingMode")]
	internal enum ResizingMode
	{
		TopLeftHandle,
		TopHandle,
		TopRightHandle,
		RightHandle,
		BottomRightHandle,
		BottomHandle,
		BottomLeftHandle,
		LeftHandle,
		AnchorHandle,
		Moving = 0x10,
		MovingPathPoints = 0x20,
		None = 0x40
	}
}
