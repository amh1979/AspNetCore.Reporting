using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using System.Drawing;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer
{
	internal sealed class Palette
	{
		public const int MaxColorIndex = 55;

		public const int MaxRGBValue = 255;

		public const int MinColorIndex = 0;

		public const int MinRGBValue = 0;

		private readonly IPaletteModel mModel;

		internal Palette(IPaletteModel model)
		{
			this.mModel = model;
		}

		public override bool Equals(object obj)
		{
			if (obj != null && obj is Palette)
			{
				if (obj == this)
				{
					return true;
				}
				Palette palette = (Palette)obj;
				return palette.mModel.Equals(this.mModel);
			}
			return false;
		}

		public Color GetColor(System.Drawing.Color color)
		{
			return this.mModel.getColor(color.R, color.G, color.B).Interface;
		}

		public override int GetHashCode()
		{
			return this.mModel.GetHashCode();
		}

		public void SetColorAt(int index, int red, int green, int blue)
		{
			this.mModel.setColorAt(index, red, green, blue);
		}

		public int GetColorIndex(int red, int green, int blue)
		{
			return this.mModel.GetColorIndex(red, green, blue);
		}

		public Color GetColorAt(int index)
		{
			return this.mModel.GetColorAt(index).Interface;
		}
	}
}
