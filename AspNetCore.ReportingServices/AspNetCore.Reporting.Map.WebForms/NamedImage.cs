using System.ComponentModel;
using System.Drawing;

namespace AspNetCore.Reporting.Map.WebForms
{
	[Description("Image with a unique name, saved as a resource.")]
	[DefaultProperty("Name")]
	[TypeConverter(typeof(NamedImageConverter))]
	internal class NamedImage : NamedElement
	{
		private Image image;

		[SRDescription("DescriptionAttributeNamedImage_Image")]
		public Image Image
		{
			get
			{
				return this.image;
			}
			set
			{
				this.image = value;
			}
		}

		public NamedImage()
		{
		}

		public NamedImage(string name, Image image)
		{
			this.Name = name;
			this.image = image;
		}

		public override string ToString()
		{
			return this.Name;
		}

		internal override void Invalidate()
		{
			if (this.Common != null)
			{
				this.Common.MapCore.InvalidateAndLayout();
			}
		}
	}
}
