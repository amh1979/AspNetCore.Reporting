using System.Drawing;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class FrameTitle7Border : FrameTitle1Border
	{
		public override string Name
		{
			get
			{
				return "FrameTitle7";
			}
		}

		public FrameTitle7Border()
		{
			base.sizeRightBottom = new SizeF(0f, base.sizeRightBottom.Height);
			float[] array = base.innerCorners = new float[8]
			{
				15f,
				1f,
				1f,
				1f,
				1f,
				15f,
				15f,
				15f
			};
		}
	}
}
