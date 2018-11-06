using System.Drawing;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal struct Point3D
	{
		public double X;

		public double Y;

		public double Z;

		public Point3D(double x, double y, double z)
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
		}

		public PointF ToPointF()
		{
			return new PointF((float)this.X, (float)this.Y);
		}
	}
}
