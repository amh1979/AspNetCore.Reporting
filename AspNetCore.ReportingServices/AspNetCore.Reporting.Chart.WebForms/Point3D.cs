using System.ComponentModel;
using System.Drawing;

namespace AspNetCore.Reporting.Chart.WebForms
{
	internal class Point3D
	{
		private PointF coordXY = new PointF(0f, 0f);

		private float coordZ;

		[Bindable(true)]
		[DefaultValue(0)]
		[SRDescription("DescriptionAttributePoint3D_X")]
		public float X
		{
			get
			{
				return this.coordXY.X;
			}
			set
			{
				this.coordXY.X = value;
			}
		}

		[Bindable(true)]
		[SRDescription("DescriptionAttributePoint3D_Y")]
		[DefaultValue(0)]
		public float Y
		{
			get
			{
				return this.coordXY.Y;
			}
			set
			{
				this.coordXY.Y = value;
			}
		}

		[Bindable(true)]
		[DefaultValue(0)]
		[SRDescription("DescriptionAttributePoint3D_Z")]
		public float Z
		{
			get
			{
				return this.coordZ;
			}
			set
			{
				this.coordZ = value;
			}
		}

		[DefaultValue(0)]
		[SRDescription("DescriptionAttributePoint3D_PointF")]
		[Bindable(true)]
		public PointF PointF
		{
			get
			{
				return this.coordXY;
			}
			set
			{
				this.coordXY = new PointF(value.X, value.Y);
			}
		}

		public Point3D(float x, float y, float z)
		{
			this.coordXY = new PointF(x, y);
			this.coordZ = z;
		}

		public Point3D()
		{
		}
	}
}
