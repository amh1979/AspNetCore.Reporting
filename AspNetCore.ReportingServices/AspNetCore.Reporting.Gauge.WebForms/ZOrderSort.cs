using System.Collections;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class ZOrderSort : IComparer
	{
		private ArrayList collection;

		public ZOrderSort(ArrayList collection)
		{
			this.collection = collection;
		}

		int IComparer.Compare(object x, object y)
		{
			int result = 0;
			if (x is IRenderable && y is IRenderable)
			{
				int num = ((IRenderable)x).GetZOrder();
				int num2 = ((IRenderable)y).GetZOrder();
				if (num == 0)
				{
					num = this.collection.IndexOf(x);
				}
				else if (num > 0)
				{
					num += this.collection.Count;
				}
				if (num2 == 0)
				{
					num2 = this.collection.IndexOf(y);
				}
				else if (num2 > 0)
				{
					num2 += this.collection.Count;
				}
				result = num - num2;
			}
			return result;
		}
	}
}
