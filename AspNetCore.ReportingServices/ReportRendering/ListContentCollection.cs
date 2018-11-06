using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class ListContentCollection
	{
		private List m_owner;

		private ListContent[] m_listContents;

		private ListContent m_firstListContent;

		public ListContent this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					ListContent listContent = null;
					if (index == 0)
					{
						listContent = this.m_firstListContent;
					}
					else if (this.m_listContents != null)
					{
						listContent = this.m_listContents[index - 1];
					}
					if (listContent == null)
					{
						listContent = new ListContent(this.m_owner, index);
						if (this.m_owner.RenderingContext.CacheState)
						{
							if (index == 0)
							{
								this.m_firstListContent = listContent;
							}
							else
							{
								if (this.m_listContents == null)
								{
									this.m_listContents = new ListContent[this.Count - 1];
								}
								this.m_listContents[index - 1] = listContent;
							}
						}
					}
					return listContent;
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		public int Count
		{
			get
			{
				int num = 0;
				ListInstance listInstance = (ListInstance)this.m_owner.ReportItemInstance;
				if (listInstance != null)
				{
					num = listInstance.ListContents.Count;
				}
				if (num == 0)
				{
					return 1;
				}
				return num;
			}
		}

		internal ListContentCollection(List owner)
		{
			this.m_owner = owner;
		}
	}
}
