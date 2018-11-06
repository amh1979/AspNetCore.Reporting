namespace AspNetCore.ReportingServices.Rendering.SPBProcessing
{
	internal sealed class SPBContext
	{
		public int StartPage = -1;

		public int EndPage = -1;

		public bool MeasureItems;

		public SecondaryStreams SecondaryStreams;

		public bool AddSecondaryStreamNames;

		public bool AddToggledItems;

		public bool AddOriginalValue;

		public bool AddFirstPageHeaderFooter;

		public bool UseImageConsolidation;

		public bool EmfDynamicImage;

		public bool ConvertImages;

		public SPBContext()
		{
		}

		public SPBContext(int startPage, int endPage)
		{
			this.StartPage = startPage;
			this.EndPage = endPage;
		}

		public SPBContext(int startPage, int endPage, bool addToggledItems)
		{
			this.StartPage = startPage;
			this.EndPage = endPage;
			this.AddToggledItems = addToggledItems;
		}
	}
}
