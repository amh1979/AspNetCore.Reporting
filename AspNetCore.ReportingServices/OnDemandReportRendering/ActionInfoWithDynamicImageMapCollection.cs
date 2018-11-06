using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ActionInfoWithDynamicImageMapCollection : ReportElementCollectionBase<ActionInfoWithDynamicImageMap>
	{
		private List<ActionInfoWithDynamicImageMap> m_list;

		public override ActionInfoWithDynamicImageMap this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					return this.m_list[index];
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		public override int Count
		{
			get
			{
				return this.m_list.Count;
			}
		}

		internal List<ActionInfoWithDynamicImageMap> InternalList
		{
			get
			{
				return this.m_list;
			}
		}

		internal ActionInfoWithDynamicImageMapCollection()
		{
			this.m_list = new List<ActionInfoWithDynamicImageMap>();
		}

		internal ActionInfoWithDynamicImageMapCollection(RenderingContext renderingContext, ImageMapAreasCollection imageMaps)
		{
			int count = imageMaps.Count;
			this.m_list = new List<ActionInfoWithDynamicImageMap>(count);
			for (int i = 0; i < count; i++)
			{
				AspNetCore.ReportingServices.ReportRendering.ImageMapArea imageMapArea = imageMaps[i];
				if (imageMapArea != null && imageMapArea.ActionInfo != null)
				{
					ImageMapAreasCollection imageMapAreasCollection = new ImageMapAreasCollection(1);
					imageMapAreasCollection.Add(imageMapArea);
					this.m_list.Add(new ActionInfoWithDynamicImageMap(renderingContext, imageMapArea.ActionInfo, imageMapAreasCollection));
				}
			}
		}

		internal ActionInfoWithDynamicImageMap Add(RenderingContext renderingContext, ReportItem owner, IROMActionOwner romActionOwner)
		{
			ActionInfoWithDynamicImageMap actionInfoWithDynamicImageMap = new ActionInfoWithDynamicImageMap(renderingContext, owner, romActionOwner);
			this.m_list.Add(actionInfoWithDynamicImageMap);
			return actionInfoWithDynamicImageMap;
		}

		internal void ConstructDefinitions()
		{
			foreach (ActionInfoWithDynamicImageMap item in this.m_list)
			{
				item.ConstructActionDefinition();
			}
		}
	}
}
