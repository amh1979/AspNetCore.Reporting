using System.ComponentModel;
using System.Drawing;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[DefaultProperty("Name")]
	[SRDescription("DescriptionAttributeNamedImage_NamedImage")]
	internal class NamedImage
	{
		private string name = string.Empty;

		private Image image;

		[Bindable(false)]
		[SRDescription("DescriptionAttributeNamedImage_Name")]
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		[SRDescription("DescriptionAttributeNamedImage_Image")]
		[Bindable(false)]
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
			this.name = name;
			this.image = image;
		}
	}
}
