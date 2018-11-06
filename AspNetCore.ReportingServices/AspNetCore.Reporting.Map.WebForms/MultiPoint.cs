using System.IO;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class MultiPoint
	{
		public double[] Box = new double[4];

		public int NumPoints;

		public ShapePoint[] Points;

		public void Read(BinaryReader reader)
		{
			this.Box[0] = reader.ReadDouble();
			this.Box[1] = reader.ReadDouble();
			this.Box[2] = reader.ReadDouble();
			this.Box[3] = reader.ReadDouble();
			this.NumPoints = reader.ReadInt32();
			this.Points = new ShapePoint[this.NumPoints];
			for (int i = 0; i < this.NumPoints; i++)
			{
				ShapePoint shapePoint = new ShapePoint();
				shapePoint.Read(reader);
				this.Points[i] = shapePoint;
			}
		}
	}
}
