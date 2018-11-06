using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class ImageMapAreasCollection
	{
		private RenderingContext m_renderingContext;

		private ArrayList m_list;

		public ImageMapArea this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					if (this.IsChartOrCustomControlImageMap)
					{
						return this.m_list[index] as ImageMapArea;
					}
					return new ImageMapArea(((ImageMapAreaInstanceList)this.m_list)[index], this.m_renderingContext);
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		private bool IsChartOrCustomControlImageMap
		{
			get
			{
				return null == this.m_renderingContext;
			}
		}

		public int Count
		{
			get
			{
				if (this.m_list != null)
				{
					return this.m_list.Count;
				}
				return 0;
			}
		}

		public ImageMapAreasCollection()
		{
			this.m_list = new ArrayList();
		}

		public ImageMapAreasCollection(int capacity)
		{
			this.m_list = new ArrayList(capacity);
		}

		internal ImageMapAreasCollection(ImageMapAreaInstanceList mapAreasInstances, RenderingContext renderingContext)
		{
			Global.Tracer.Assert(null != renderingContext);
			this.m_renderingContext = renderingContext;
			this.m_list = mapAreasInstances;
		}

		public void Add(ImageMapArea mapArea)
		{
			this.m_list.Add(mapArea);
		}

		internal ImageMapAreasCollection DeepClone()
		{
			Global.Tracer.Assert(this.IsChartOrCustomControlImageMap);
			if (this.m_list != null && this.m_list.Count != 0)
			{
				int count = this.m_list.Count;
				ImageMapAreasCollection imageMapAreasCollection = new ImageMapAreasCollection(count);
				for (int i = 0; i < count; i++)
				{
					imageMapAreasCollection.m_list.Add(((ImageMapArea)this.m_list[i]).DeepClone());
				}
				return imageMapAreasCollection;
			}
			return null;
		}

		internal ImageMapAreaInstanceList Deconstruct(AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ProcessingContext processingContext, AspNetCore.ReportingServices.ReportProcessing.CustomReportItem context)
		{
			Global.Tracer.Assert(context != null && null != processingContext);
			if (this.m_list != null && this.m_list.Count != 0)
			{
				int count = this.m_list.Count;
				ImageMapAreaInstanceList imageMapAreaInstanceList = new ImageMapAreaInstanceList(count);
				for (int i = 0; i < count; i++)
				{
					ImageMapAreaInstance value = ((ImageMapArea)this.m_list[i]).Deconstruct(context);
					imageMapAreaInstanceList.Add(value);
				}
				return imageMapAreaInstanceList;
			}
			return null;
		}
	}
}
