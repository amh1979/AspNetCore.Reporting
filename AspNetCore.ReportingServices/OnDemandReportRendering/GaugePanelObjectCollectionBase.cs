using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class GaugePanelObjectCollectionBase<T> : IEnumerable<T>, IEnumerable where T : GaugePanelObjectCollectionItem
	{
		private T[] m_collection;

		public T this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					if (this.m_collection == null)
					{
						this.m_collection = new T[this.Count];
					}
					if (this.m_collection[index] == null)
					{
						this.m_collection[index] = this.CreateGaugePanelObject(index);
					}
					return this.m_collection[index];
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		public abstract int Count
		{
			get;
		}

		protected abstract T CreateGaugePanelObject(int index);

		public IEnumerator<T> GetEnumerator()
		{
			for (int i = 0; i < this.Count; i++)
			{
				yield return this[i];
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		internal void SetNewContext()
		{
			if (this.m_collection != null)
			{
				for (int i = 0; i < this.m_collection.Length; i++)
				{
					T val = this.m_collection[i];
					if (val != null)
					{
						val.SetNewContext();
					}
				}
			}
		}
	}
}
