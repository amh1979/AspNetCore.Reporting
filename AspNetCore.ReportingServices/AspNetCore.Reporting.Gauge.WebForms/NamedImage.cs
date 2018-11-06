using System.ComponentModel;
using System.Drawing;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	[TypeConverter(typeof(NamedImageConverter))]
	[SRDescription("DescriptionAttributeNamedImage_NamedImage")]
	[DefaultProperty("Name")]
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
			: base(name)
		{
			this.image = image;
		}

		public override string ToString()
		{
			return this.Name;
		}
	}
}
