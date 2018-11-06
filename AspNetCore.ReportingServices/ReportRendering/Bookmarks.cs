using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class Bookmarks
	{
		private BookmarksHashtable m_reportBookmarks;

		public Bookmark this[string bookmarkId]
		{
			get
			{
				if (bookmarkId != null && this.m_reportBookmarks != null)
				{
					BookmarkInformation bookmarkInformation = this.m_reportBookmarks[bookmarkId];
					if (bookmarkInformation != null)
					{
						return new Bookmark(bookmarkId, bookmarkInformation);
					}
					return null;
				}
				return null;
			}
		}

		public IDictionaryEnumerator BookmarksEnumerator
		{
			get
			{
				if (this.m_reportBookmarks == null)
				{
					return null;
				}
				return this.m_reportBookmarks.GetEnumerator();
			}
		}

		internal Bookmarks(BookmarksHashtable reportBookmarks)
		{
			Global.Tracer.Assert(reportBookmarks != null, "The bookmark hashtable being wrapped cannot be null.");
			this.m_reportBookmarks = reportBookmarks;
		}
	}
}
