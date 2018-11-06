namespace AspNetCore.Reporting.Map.WebForms
{
	internal class Matrix3x3
	{
		public double[,] Elements = new double[3, 3];

		public Matrix3x3()
		{
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					if (i == j)
					{
						this.Elements[i, j] = 1.0;
					}
					else
					{
						this.Elements[i, j] = 0.0;
					}
				}
			}
		}

		public static Matrix3x3 operator *(Matrix3x3 matrixA, Matrix3x3 matrixB)
		{
			Matrix3x3 matrix3x = new Matrix3x3();
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					double[,] elements = matrix3x.Elements;
					int num = i;
					int num2 = j;
					double num3 = matrixA.Elements[i, 0] * matrixB.Elements[0, j] + matrixA.Elements[i, 1] * matrixB.Elements[1, j] + matrixA.Elements[i, 2] * matrixB.Elements[2, j];
					elements[num, num2] = num3;
				}
			}
			return matrix3x;
		}

		public Point3D TransformPoint(Point3D point)
		{
			Point3D result = default(Point3D);
			result.X = this.Elements[0, 0] * point.X + this.Elements[0, 1] * point.Y + this.Elements[0, 2] * point.Z;
			result.Y = this.Elements[1, 0] * point.X + this.Elements[1, 1] * point.Y + this.Elements[1, 2] * point.Z;
			result.Z = this.Elements[2, 0] * point.X + this.Elements[2, 1] * point.Y + this.Elements[2, 2] * point.Z;
			return result;
		}
	}
}
