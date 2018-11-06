using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class TagCollection : ReportElementCollectionBase<Tag>
	{
		private readonly Image m_image;

		private readonly List<Tag> m_collection;

		public override int Count
		{
			get
			{
				return this.m_collection.Count;
			}
		}

		public override Tag this[int i]
		{
			get
			{
				return this.m_collection[i];
			}
		}

		internal TagCollection(Image image)
		{
			this.m_image = image;
			List<ExpressionInfo> tags = this.m_image.ImageDef.Tags;
			this.m_collection = new List<Tag>(tags.Count);
			for (int i = 0; i < tags.Count; i++)
			{
				this.m_collection.Add(new Tag(image, tags[i]));
			}
		}

		internal void SetNewContext()
		{
			for (int i = 0; i < this.m_collection.Count; i++)
			{
				this.m_collection[i].SetNewContext();
			}
		}
	}
}
