using System.Xml.Serialization;

namespace AspNetCore.ReportingServices.Diagnostics.Internal
{
    internal sealed class ScaleTimeCategory
	{
		public long? Pagination
		{
			get;
			set;
		}

		[XmlIgnore]
		public bool PaginationSpecified
		{
			get
			{
				return this.Pagination.HasValue;
			}
		}

		public long? Rendering
		{
			get;
			set;
		}

		[XmlIgnore]
		public bool RenderingSpecified
		{
			get
			{
				return this.Rendering.HasValue;
			}
		}

		public long? Processing
		{
			get;
			set;
		}

		[XmlIgnore]
		public bool ProcessingSpecified
		{
			get
			{
				return this.Processing.HasValue;
			}
		}

		internal ScaleTimeCategory()
		{
		}
	}
}
