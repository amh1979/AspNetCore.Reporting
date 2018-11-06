namespace AspNetCore.ReportingServices.Rendering.SPBProcessing
{
	internal enum PaginationInfoProperties : byte
	{
		Unknown,
		ItemSizes,
		PaddItemSizes,
		ItemState,
		PageItemsAbove,
		PageItemsLeft,
		ItemsCreated,
		IndexesLeftToRight,
		RepeatWithItems,
		RightEdgeItem,
		Children,
		PrevPageEnd,
		RelativeTop,
		RelativeBottom,
		RelativeTopToBottom,
		DataRegionIndex,
		LevelForRepeat,
		TablixCreateState,
		MembersInstanceIndex,
		ChildPage,
		IndexesTopToBottom,
		DefLeftValue,
		IgnoreTotalsOnLastLevel,
		SectionIndex,
		Delimiter = 0xFF
	}
}
