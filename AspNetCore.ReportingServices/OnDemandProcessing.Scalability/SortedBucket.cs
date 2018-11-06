namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class SortedBucket
	{
		internal int Limit;

		internal int Minimum;

		internal Heap<long, Space> m_spaces;

		internal int Count
		{
			get
			{
				return this.m_spaces.Count;
			}
		}

		internal int Maximum
		{
			get
			{
				return (int)this.Peek().Size;
			}
		}

		internal SortedBucket(int maxSpacesPerBucket)
		{
			int num = 500;
			if (num > maxSpacesPerBucket)
			{
				num = maxSpacesPerBucket;
			}
			this.m_spaces = new Heap<long, Space>(num, maxSpacesPerBucket);
			this.Minimum = 2147483647;
		}

		internal SortedBucket Split(int maxSpacesPerBucket)
		{
			SortedBucket sortedBucket = new SortedBucket(maxSpacesPerBucket);
			int num = this.Count / 2;
			for (int i = 0; i < num; i++)
			{
				sortedBucket.Insert(this.ExtractMax());
			}
			sortedBucket.Limit = sortedBucket.Minimum;
			return sortedBucket;
		}

		internal void Insert(Space space)
		{
			if (space.Size < this.Minimum)
			{
				this.Minimum = (int)space.Size;
			}
			this.m_spaces.Insert(space.Size, space);
		}

		internal Space Peek()
		{
			return this.m_spaces.Peek();
		}

		internal Space ExtractMax()
		{
			Space result = this.m_spaces.ExtractMax();
			if (this.m_spaces.Count == 0)
			{
				this.Minimum = 2147483647;
			}
			return result;
		}
	}
}
