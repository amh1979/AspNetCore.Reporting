using System.IO;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class PolyLine
	{
		public double[] Box = new double[4];

		public int NumParts;

		public int NumPoints;

		public int[] Parts;

		public ShapePoint[] Points;

		public void Read(BinaryReader reader)
		{
			this.Box[0] = reader.ReadDouble();
			this.Box[1] = reader.ReadDouble();
			this.Box[2] = reader.ReadDouble();
			this.Box[3] = reader.ReadDouble();
			this.NumParts = reader.ReadInt32();
			this.NumPoints = reader.ReadInt32();
			this.Parts = new int[this.NumParts];
			for (int i = 0; i < this.NumParts; i++)
			{
				this.Parts[i] = reader.ReadInt32();
			}
			this.Points = new ShapePoint[this.NumPoints];
			for (int j = 0; j < this.NumPoints; j++)
			{
				ShapePoint shapePoint = new ShapePoint();
				shapePoint.Read(reader);
				this.Points[j] = shapePoint;
			}
		}
	}
}
