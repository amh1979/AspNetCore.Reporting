using System.Drawing;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class FrameTitle8Border : FrameTitle1Border
	{
		public override string Name
		{
			get
			{
				return "FrameTitle8";
			}
		}

		public FrameTitle8Border()
		{
			base.sizeLeftTop = new SizeF(0f, base.sizeLeftTop.Height);
			base.sizeRightBottom = new SizeF(0f, base.sizeRightBottom.Height);
			float[] array = base.innerCorners = new float[8]
			{
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f
			};
		}
	}
}
