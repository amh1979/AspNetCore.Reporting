using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class MapObjectCollectionBase<T> : ReportElementCollectionBase<T> where T : IMapObjectCollectionItem
	{
		private T[] m_collection;

		public override T this[int index]
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
						this.m_collection[index] = this.CreateMapObject(index);
					}
					return this.m_collection[index];
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		protected abstract T CreateMapObject(int index);

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
