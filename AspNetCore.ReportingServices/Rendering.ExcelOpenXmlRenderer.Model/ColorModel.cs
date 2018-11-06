namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model
{
	internal abstract class ColorModel
	{
		private Color mInterface;

		public Color Interface
		{
			get
			{
				if (this.mInterface == null)
				{
					this.mInterface = new Color(this);
				}
				return this.mInterface;
			}
		}

		public abstract int getRed();

		public abstract int getBlue();

		public abstract int getGreen();

		public override bool Equals(object aObject)
		{
			if (aObject is ColorModel)
			{
				ColorModel colorModel = (ColorModel)aObject;
				if (this.getRed() == colorModel.getRed() && this.getGreen() == colorModel.getGreen())
				{
					return this.getBlue() == colorModel.getBlue();
				}
				return false;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
