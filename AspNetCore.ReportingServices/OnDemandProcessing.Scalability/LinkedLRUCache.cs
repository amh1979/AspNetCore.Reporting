using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class LinkedLRUCache<T> where T : ItemHolder
	{
		private int m_count;

		private ItemHolder m_sentinal;

		public int Count
		{
			get
			{
				return this.m_count;
			}
		}

		public LinkedLRUCache()
		{
			this.m_sentinal = new ItemHolder();
			this.Clear();
		}

		public void Add(ItemHolder item)
		{
			this.m_count++;
			item.Next = this.m_sentinal;
			item.Previous = this.m_sentinal.Previous;
			this.m_sentinal.Previous.Next = item;
			this.m_sentinal.Previous = item;
		}

		public void Bump(ItemHolder item)
		{
			item.Previous.Next = item.Next;
			item.Next.Previous = item.Previous;
			item.Next = this.m_sentinal;
			item.Previous = this.m_sentinal.Previous;
			this.m_sentinal.Previous.Next = item;
			this.m_sentinal.Previous = item;
		}

		public T ExtractLRU()
		{
			if (this.m_count == 0)
			{
				Global.Tracer.Assert(false, "Cannot ExtractLRU from empty cache");
			}
			ItemHolder next = this.m_sentinal.Next;
			this.Remove(next);
			return (T)next;
		}

		public void Remove(ItemHolder item)
		{
			if (this.m_count == 0)
			{
				Global.Tracer.Assert(false, "Cannot ExtractLRU from empty cache");
			}
			this.m_count--;
			item.Previous.Next = item.Next;
			item.Next.Previous = item.Previous;
			item.Next = null;
			item.Previous = null;
		}

		public T Peek()
		{
			if (this.m_count == 0)
			{
				return null;
			}
			return (T)this.m_sentinal.Next;
		}

		public void Clear()
		{
			this.m_count = 0;
			this.m_sentinal.Previous = this.m_sentinal;
			this.m_sentinal.Next = this.m_sentinal;
		}
	}
}
