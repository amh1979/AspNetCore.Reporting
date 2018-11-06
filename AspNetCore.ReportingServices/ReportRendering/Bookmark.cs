using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class Bookmark
	{
		private BookmarkInformation m_underlyingNode;

		private string m_bookmarkId;

		public string BookmarkId
		{
			get
			{
				return this.m_bookmarkId;
			}
		}

		public string UniqueName
		{
			get
			{
				return this.m_underlyingNode.Id;
			}
		}

		public int Page
		{
			get
			{
				return this.m_underlyingNode.Page;
			}
		}

		internal Bookmark(string bookmarkId, BookmarkInformation underlyingNode)
		{
			Global.Tracer.Assert(underlyingNode != null, "The bookmark node being wrapped cannot be null.");
			this.m_bookmarkId = bookmarkId;
			this.m_underlyingNode = underlyingNode;
		}
	}
}
