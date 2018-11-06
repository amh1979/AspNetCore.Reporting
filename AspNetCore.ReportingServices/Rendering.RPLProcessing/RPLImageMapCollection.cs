using System;

namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLImageMapCollection
	{
		private RPLImageMap[] m_list;

		public RPLImageMap this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					return this.m_list[index];
				}
				throw new InvalidOperationException();
			}
			set
			{
				if (index >= 0 && index < this.Count)
				{
					this.m_list[index] = value;
					return;
				}
				throw new InvalidOperationException();
			}
		}

		public int Count
		{
			get
			{
				if (this.m_list == null)
				{
					return 0;
				}
				return this.m_list.Length;
			}
		}

		internal RPLImageMapCollection(int count)
		{
			this.m_list = new RPLImageMap[count];
		}

		internal RPLImageMapCollection(RPLImageMap[] list)
		{
			this.m_list = list;
		}
	}
}
