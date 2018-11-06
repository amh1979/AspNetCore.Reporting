using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AspNetCore.Reporting.Chart.WebForms
{
	internal class Matrix3D
	{
		private enum RotationAxis
		{
			X,
			Y,
			Z
		}

		private float[][] mainMatrix;

		private float translateX;

		private float translateY;

		private float translateZ;

		private float scale;

		private float shiftX;

		private float shiftY;

		internal float perspective;

		private bool rightAngleAxis;

		private float perspectiveFactor = float.NaN;

		private float perspectiveZ;

		internal float angleX;

		internal float angleY;

		private Point3D[] lightVectors = new Point3D[7];

		private LightStyle lightStyle;

		public bool IsInitialized()
		{
			return this.mainMatrix != null;
		}

		internal void Initialize(RectangleF innerPlotRectangle, float depth, float angleX, float angleY, float perspective, bool rightAngleAxis)
		{
			this.Reset();
			this.translateX = (float)(innerPlotRectangle.X + innerPlotRectangle.Width / 2.0);
			this.translateY = (float)(innerPlotRectangle.Y + innerPlotRectangle.Height / 2.0);
			this.translateZ = (float)(depth / 2.0);
			float width = innerPlotRectangle.Width;
			float height = innerPlotRectangle.Height;
			this.perspective = perspective;
			this.rightAngleAxis = rightAngleAxis;
			this.angleX = angleX;
			this.angleY = angleY;
			angleX = (float)(angleX / 180.0 * 3.1415927410125732);
			angleY = (float)(angleY / 180.0 * 3.1415927410125732);
			Point3D[] array = this.Set3DBarPoints(width, height, depth);
			this.Translate(this.translateX, this.translateY, 0f);
			if (!rightAngleAxis)
			{
				this.Rotate((double)angleX, RotationAxis.X);
				this.Rotate((double)angleY, RotationAxis.Y);
			}
			else if (this.angleY >= 45.0)
			{
				this.Rotate(1.5707963267948966, RotationAxis.Y);
			}
			else if (this.angleY <= -45.0)
			{
				this.Rotate(-1.5707963267948966, RotationAxis.Y);
			}
			this.GetValues(array);
			float num = -3.40282347E+38f;
			if (perspective != 0.0 || rightAngleAxis)
			{
				Point3D[] array2 = array;
				foreach (Point3D point3D in array2)
				{
					if (point3D.Z > num)
					{
						num = point3D.Z;
					}
				}
				this.perspectiveZ = num;
			}
			if (perspective != 0.0)
			{
				this.perspectiveFactor = (float)(perspective / 2000.0);
				this.Perspective(array);
			}
			if (rightAngleAxis)
			{
				this.RightAngleProjection(array);
				float num2 = 0f;
				float num3 = 0f;
				float num4 = 0f;
				float num5 = 0f;
				Point3D[] array3 = array;
				foreach (Point3D point3D2 in array3)
				{
					if (point3D2.X - this.translateX < 0.0 && Math.Abs(point3D2.X - this.translateX) > num2)
					{
						num2 = Math.Abs(point3D2.X - this.translateX);
					}
					if (point3D2.X - this.translateX >= 0.0 && Math.Abs(point3D2.X - this.translateX) > num4)
					{
						num4 = Math.Abs(point3D2.X - this.translateX);
					}
					if (point3D2.Y - this.translateY < 0.0 && Math.Abs(point3D2.Y - this.translateY) > num3)
					{
						num3 = Math.Abs(point3D2.Y - this.translateY);
					}
					if (point3D2.Y - this.translateY >= 0.0 && Math.Abs(point3D2.Y - this.translateY) > num5)
					{
						num5 = Math.Abs(point3D2.Y - this.translateY);
					}
				}
				this.shiftX = (float)((num4 - num2) / 2.0);
				this.shiftY = (float)((num5 - num3) / 2.0);
				this.RightAngleShift(array);
			}
			float num6 = -3.40282347E+38f;
			float num7 = -3.40282347E+38f;
			Point3D[] array4 = array;
			foreach (Point3D point3D3 in array4)
			{
				if (num6 < Math.Abs(point3D3.X - this.translateX) / width * 2.0)
				{
					num6 = (float)(Math.Abs(point3D3.X - this.translateX) / width * 2.0);
				}
				if (num7 < Math.Abs(point3D3.Y - this.translateY) / height * 2.0)
				{
					num7 = (float)(Math.Abs(point3D3.Y - this.translateY) / height * 2.0);
				}
			}
			this.scale = ((num7 > num6) ? num7 : num6);
			this.Scale(array);
		}

		public void TransformPoints(Point3D[] points)
		{
			this.TransformPoints(points, true);
		}

		private void TransformPoints(Point3D[] points, bool withPerspective)
		{
			if (this.mainMatrix == null)
			{
				throw new InvalidOperationException(SR.ExceptionMatrix3DNotinitialized);
			}
			foreach (Point3D point3D in points)
			{
				point3D.X -= this.translateX;
				point3D.Y -= this.translateY;
				point3D.Z -= this.translateZ;
			}
			this.GetValues(points);
			if (this.perspective != 0.0 && withPerspective)
			{
				this.Perspective(points);
			}
			if (this.rightAngleAxis)
			{
				this.RightAngleProjection(points);
				this.RightAngleShift(points);
			}
			this.Scale(points);
		}

		private void RightAngleShift(Point3D[] points)
		{
			foreach (Point3D point3D in points)
			{
				point3D.X -= this.shiftX;
				point3D.Y -= this.shiftY;
			}
		}

		private void RightAngleProjection(Point3D[] points)
		{
			float num = 45f;
			float num2 = (float)(this.angleX / 45.0);
			float num3 = (float)((!(this.angleY >= 45.0)) ? ((!(this.angleY <= -45.0)) ? (this.angleY / num) : ((this.angleY + 90.0) / num)) : ((this.angleY - 90.0) / num));
			foreach (Point3D point3D in points)
			{
				point3D.X += (this.perspectiveZ - point3D.Z) * num3;
				point3D.Y -= (this.perspectiveZ - point3D.Z) * num2;
			}
		}

		private void Perspective(Point3D[] points)
		{
			foreach (Point3D point3D in points)
			{
				point3D.X = (float)(this.translateX + (point3D.X - this.translateX) / (1.0 + (this.perspectiveZ - point3D.Z) * this.perspectiveFactor));
				point3D.Y = (float)(this.translateY + (point3D.Y - this.translateY) / (1.0 + (this.perspectiveZ - point3D.Z) * this.perspectiveFactor));
			}
		}

		private void Scale(Point3D[] points)
		{
			foreach (Point3D point3D in points)
			{
				point3D.X = this.translateX + (point3D.X - this.translateX) / this.scale;
				point3D.Y = this.translateY + (point3D.Y - this.translateY) / this.scale;
			}
		}

		private void Translate(float dx, float dy, float dz)
		{
			float[][] array = new float[4][]
			{
				new float[4],
				new float[4],
				new float[4],
				new float[4]
			};
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					if (i == j)
					{
						array[i][j] = 1f;
					}
					else
					{
						array[i][j] = 0f;
					}
				}
			}
			array[0][3] = dx;
			array[1][3] = dy;
			array[2][3] = dz;
			this.Multiply(array, MatrixOrder.Prepend, true);
		}

		private void Reset()
		{
			this.mainMatrix = new float[4][];
			this.mainMatrix[0] = new float[4];
			this.mainMatrix[1] = new float[4];
			this.mainMatrix[2] = new float[4];
			this.mainMatrix[3] = new float[4];
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					if (i == j)
					{
						this.mainMatrix[i][j] = 1f;
					}
					else
					{
						this.mainMatrix[i][j] = 0f;
					}
				}
			}
		}

		private float[][] Multiply(float[][] mulMatrix, MatrixOrder order, bool setMainMatrix)
		{
			float[][] array = new float[4][]
			{
				new float[4],
				new float[4],
				new float[4],
				new float[4]
			};
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					array[i][j] = 0f;
					for (int k = 0; k < 4; k++)
					{
						if (order == MatrixOrder.Prepend)
						{
							array[i][j] += this.mainMatrix[i][k] * mulMatrix[k][j];
						}
						else
						{
							array[i][j] += mulMatrix[i][k] * this.mainMatrix[k][j];
						}
					}
				}
			}
			if (setMainMatrix)
			{
				this.mainMatrix = array;
			}
			return array;
		}

		private void MultiplyVector(float[] mulVector, ref float[] resultVector)
		{
			for (int i = 0; i < 3; i++)
			{
				resultVector[i] = 0f;
				for (int j = 0; j < 4; j++)
				{
					resultVector[i] += this.mainMatrix[i][j] * mulVector[j];
				}
			}
		}

		private void Rotate(double angle, RotationAxis axis)
		{
			float[][] array = new float[4][]
			{
				new float[4],
				new float[4],
				new float[4],
				new float[4]
			};
			angle = -1.0 * angle;
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					if (i == j)
					{
						array[i][j] = 1f;
					}
					else
					{
						array[i][j] = 0f;
					}
				}
			}
			switch (axis)
			{
			case RotationAxis.X:
				array[1][1] = (float)Math.Cos(angle);
				array[1][2] = (float)(0.0 - Math.Sin(angle));
				array[2][1] = (float)Math.Sin(angle);
				array[2][2] = (float)Math.Cos(angle);
				break;
			case RotationAxis.Y:
				array[0][0] = (float)Math.Cos(angle);
				array[0][2] = (float)Math.Sin(angle);
				array[2][0] = (float)(0.0 - Math.Sin(angle));
				array[2][2] = (float)Math.Cos(angle);
				break;
			case RotationAxis.Z:
				array[0][0] = (float)Math.Cos(angle);
				array[0][1] = (float)(0.0 - Math.Sin(angle));
				array[1][0] = (float)Math.Sin(angle);
				array[1][1] = (float)Math.Cos(angle);
				break;
			}
			this.Multiply(array, MatrixOrder.Prepend, true);
		}

		private void GetValues(Point3D[] points)
		{
			float[] array = new float[4];
			float[] array2 = new float[4];
			foreach (Point3D point3D in points)
			{
				array[0] = point3D.X;
				array[1] = point3D.Y;
				array[2] = point3D.Z;
				array[3] = 1f;
				this.MultiplyVector(array, ref array2);
				point3D.X = array2[0];
				point3D.Y = array2[1];
				point3D.Z = array2[2];
			}
		}

		private Point3D[] Set3DBarPoints(float dx, float dy, float dz)
		{
			return new Point3D[8]
			{
				new Point3D((float)((0.0 - dx) / 2.0), (float)((0.0 - dy) / 2.0), (float)(dz / 2.0)),
				new Point3D((float)(dx / 2.0), (float)((0.0 - dy) / 2.0), (float)(dz / 2.0)),
				new Point3D((float)(dx / 2.0), (float)(dy / 2.0), (float)(dz / 2.0)),
				new Point3D((float)((0.0 - dx) / 2.0), (float)(dy / 2.0), (float)(dz / 2.0)),
				new Point3D((float)((0.0 - dx) / 2.0), (float)((0.0 - dy) / 2.0), (float)((0.0 - dz) / 2.0)),
				new Point3D((float)(dx / 2.0), (float)((0.0 - dy) / 2.0), (float)((0.0 - dz) / 2.0)),
				new Point3D((float)(dx / 2.0), (float)(dy / 2.0), (float)((0.0 - dz) / 2.0)),
				new Point3D((float)((0.0 - dx) / 2.0), (float)(dy / 2.0), (float)((0.0 - dz) / 2.0))
			};
		}

		internal void InitLight(LightStyle lightStyle)
		{
			this.lightStyle = lightStyle;
			this.lightVectors[0] = new Point3D(0f, 0f, 0f);
			this.lightVectors[1] = new Point3D(0f, 0f, 1f);
			this.lightVectors[2] = new Point3D(0f, 0f, -1f);
			this.lightVectors[3] = new Point3D(-1f, 0f, 0f);
			this.lightVectors[4] = new Point3D(1f, 0f, 0f);
			this.lightVectors[5] = new Point3D(0f, -1f, 0f);
			this.lightVectors[6] = new Point3D(0f, 1f, 0f);
			this.TransformPoints(this.lightVectors, false);
			this.lightVectors[1].X -= this.lightVectors[0].X;
			this.lightVectors[1].Y -= this.lightVectors[0].Y;
			this.lightVectors[1].Z -= this.lightVectors[0].Z;
			this.lightVectors[2].X -= this.lightVectors[0].X;
			this.lightVectors[2].Y -= this.lightVectors[0].Y;
			this.lightVectors[2].Z -= this.lightVectors[0].Z;
			this.lightVectors[3].X -= this.lightVectors[0].X;
			this.lightVectors[3].Y -= this.lightVectors[0].Y;
			this.lightVectors[3].Z -= this.lightVectors[0].Z;
			this.lightVectors[4].X -= this.lightVectors[0].X;
			this.lightVectors[4].Y -= this.lightVectors[0].Y;
			this.lightVectors[4].Z -= this.lightVectors[0].Z;
			this.lightVectors[5].X -= this.lightVectors[0].X;
			this.lightVectors[5].Y -= this.lightVectors[0].Y;
			this.lightVectors[5].Z -= this.lightVectors[0].Z;
			this.lightVectors[6].X -= this.lightVectors[0].X;
			this.lightVectors[6].Y -= this.lightVectors[0].Y;
			this.lightVectors[6].Z -= this.lightVectors[0].Z;
		}

		internal void GetLight(Color surfaceColor, out Color front, out Color back, out Color left, out Color right, out Color top, out Color bottom)
		{
			switch (this.lightStyle)
			{
			case LightStyle.None:
				front = surfaceColor;
				left = surfaceColor;
				top = surfaceColor;
				back = surfaceColor;
				right = surfaceColor;
				bottom = surfaceColor;
				break;
			case LightStyle.Simplistic:
				front = surfaceColor;
				left = ChartGraphics.GetGradientColor(surfaceColor, Color.Black, 0.25);
				top = ChartGraphics.GetGradientColor(surfaceColor, Color.Black, 0.15);
				back = surfaceColor;
				right = ChartGraphics.GetGradientColor(surfaceColor, Color.Black, 0.25);
				bottom = ChartGraphics.GetGradientColor(surfaceColor, Color.Black, 0.15);
				break;
			default:
				if (this.rightAngleAxis)
				{
					Point3D point3D = new Point3D(0f, 0f, -1f);
					this.RightAngleProjection(new Point3D[1]
					{
						point3D
					});
					if (this.angleY >= 45.0 || this.angleY <= -45.0)
					{
						front = ChartGraphics.GetGradientColor(surfaceColor, Color.Black, (double)this.GetAngle(point3D, this.lightVectors[1]) / 3.1415926535897931);
						back = ChartGraphics.GetGradientColor(surfaceColor, Color.Black, (double)this.GetAngle(point3D, this.lightVectors[2]) / 3.1415926535897931);
						left = ChartGraphics.GetGradientColor(surfaceColor, Color.Black, 0.0);
						right = ChartGraphics.GetGradientColor(surfaceColor, Color.Black, 0.0);
					}
					else
					{
						front = ChartGraphics.GetGradientColor(surfaceColor, Color.Black, 0.0);
						back = ChartGraphics.GetGradientColor(surfaceColor, Color.Black, 1.0);
						left = ChartGraphics.GetGradientColor(surfaceColor, Color.Black, (double)this.GetAngle(point3D, this.lightVectors[3]) / 3.1415926535897931);
						right = ChartGraphics.GetGradientColor(surfaceColor, Color.Black, (double)this.GetAngle(point3D, this.lightVectors[4]) / 3.1415926535897931);
					}
					top = ChartGraphics.GetGradientColor(surfaceColor, Color.Black, (double)this.GetAngle(point3D, this.lightVectors[5]) / 3.1415926535897931);
					bottom = ChartGraphics.GetGradientColor(surfaceColor, Color.Black, (double)this.GetAngle(point3D, this.lightVectors[6]) / 3.1415926535897931);
				}
				else
				{
					Point3D a = new Point3D(0f, 0f, 1f);
					front = this.GetBrightGradientColor(surfaceColor, (double)this.GetAngle(a, this.lightVectors[1]) / 3.1415926535897931);
					back = this.GetBrightGradientColor(surfaceColor, (double)this.GetAngle(a, this.lightVectors[2]) / 3.1415926535897931);
					left = this.GetBrightGradientColor(surfaceColor, (double)this.GetAngle(a, this.lightVectors[3]) / 3.1415926535897931);
					right = this.GetBrightGradientColor(surfaceColor, (double)this.GetAngle(a, this.lightVectors[4]) / 3.1415926535897931);
					top = this.GetBrightGradientColor(surfaceColor, (double)this.GetAngle(a, this.lightVectors[5]) / 3.1415926535897931);
					bottom = this.GetBrightGradientColor(surfaceColor, (double)this.GetAngle(a, this.lightVectors[6]) / 3.1415926535897931);
				}
				break;
			}
		}

		internal Color GetPolygonLight(Point3D[] points, Color surfaceColor, bool visiblePolygon, float yAngle, SurfaceNames surfaceName, bool switchSeriesOrder)
		{
			Color result = surfaceColor;
			Point3D point3D = new Point3D(0f, 0f, 1f);
			switch (this.lightStyle)
			{
			case LightStyle.Simplistic:
			{
				Point3D point3D5 = new Point3D();
				point3D5.X = points[0].X - points[1].X;
				point3D5.Y = points[0].Y - points[1].Y;
				point3D5.Z = points[0].Z - points[1].Z;
				Point3D point3D6 = new Point3D();
				point3D6.X = points[2].X - points[1].X;
				point3D6.Y = points[2].Y - points[1].Y;
				point3D6.Z = points[2].Z - points[1].Z;
				Point3D point3D7 = new Point3D();
				point3D7.X = point3D5.Y * point3D6.Z - point3D5.Z * point3D6.Y;
				point3D7.Y = point3D5.Z * point3D6.X - point3D5.X * point3D6.Z;
				point3D7.Z = point3D5.X * point3D6.Y - point3D5.Y * point3D6.X;
				switch (surfaceName)
				{
				case SurfaceNames.Left:
					result = ChartGraphics.GetGradientColor(surfaceColor, Color.Black, 0.15);
					break;
				case SurfaceNames.Right:
					result = ChartGraphics.GetGradientColor(surfaceColor, Color.Black, 0.15);
					break;
				case SurfaceNames.Front:
					result = surfaceColor;
					break;
				case SurfaceNames.Back:
					result = surfaceColor;
					break;
				default:
				{
					float angle;
					float angle2;
					if (switchSeriesOrder)
					{
						if (yAngle > 0.0 && yAngle <= 90.0)
						{
							angle = this.GetAngle(point3D7, this.lightVectors[3]);
							angle2 = this.GetAngle(point3D7, this.lightVectors[4]);
						}
						else
						{
							angle = this.GetAngle(point3D7, this.lightVectors[4]);
							angle2 = this.GetAngle(point3D7, this.lightVectors[3]);
						}
					}
					else if (yAngle > 0.0 && yAngle <= 90.0)
					{
						angle = this.GetAngle(point3D7, this.lightVectors[4]);
						angle2 = this.GetAngle(point3D7, this.lightVectors[3]);
					}
					else
					{
						angle = this.GetAngle(point3D7, this.lightVectors[3]);
						angle2 = this.GetAngle(point3D7, this.lightVectors[4]);
					}
					result = ((!((double)Math.Abs(angle - angle2) < 0.01)) ? ((!(angle < angle2)) ? ChartGraphics.GetGradientColor(surfaceColor, Color.Black, 0.15) : ChartGraphics.GetGradientColor(surfaceColor, Color.Black, 0.25)) : ChartGraphics.GetGradientColor(surfaceColor, Color.Black, 0.25));
					break;
				}
				}
				break;
			}
			default:
			{
				Point3D point3D2 = new Point3D();
				point3D2.X = points[0].X - points[1].X;
				point3D2.Y = points[0].Y - points[1].Y;
				point3D2.Z = points[0].Z - points[1].Z;
				Point3D point3D3 = new Point3D();
				point3D3.X = points[2].X - points[1].X;
				point3D3.Y = points[2].Y - points[1].Y;
				point3D3.Z = points[2].Z - points[1].Z;
				Point3D point3D4 = new Point3D();
				point3D4.X = point3D2.Y * point3D3.Z - point3D2.Z * point3D3.Y;
				point3D4.Y = point3D2.Z * point3D3.X - point3D2.X * point3D3.Z;
				point3D4.Z = point3D2.X * point3D3.Y - point3D2.Y * point3D3.X;
				switch (surfaceName)
				{
				case SurfaceNames.Front:
					point3D.Z *= -1f;
					result = this.GetBrightGradientColor(surfaceColor, (double)this.GetAngle(point3D, this.lightVectors[2]) / 3.1415926535897931);
					break;
				case SurfaceNames.Back:
					point3D.Z *= -1f;
					result = this.GetBrightGradientColor(surfaceColor, (double)this.GetAngle(point3D, this.lightVectors[1]) / 3.1415926535897931);
					break;
				default:
					if (visiblePolygon)
					{
						point3D.Z *= -1f;
					}
					result = this.GetBrightGradientColor(surfaceColor, (double)this.GetAngle(point3D, point3D4) / 3.1415926535897931);
					break;
				}
				break;
			}
			case LightStyle.None:
				break;
			}
			return result;
		}

		private Color GetBrightGradientColor(Color beginColor, double position)
		{
			position *= 2.0;
			double num = 0.5;
			if (position < num)
			{
				return ChartGraphics.GetGradientColor(Color.FromArgb(beginColor.A, 255, 255, 255), beginColor, 1.0 - num + position);
			}
			if (0.0 - num + position < 1.0)
			{
				return ChartGraphics.GetGradientColor(beginColor, Color.Black, 0.0 - num + position);
			}
			return Color.FromArgb(beginColor.A, 0, 0, 0);
		}

		private float GetAngle(Point3D a, Point3D b)
		{
			double num = Math.Acos((double)(a.X * b.X + a.Y * b.Y + a.Z * b.Z) / (Math.Sqrt((double)(a.X * a.X + a.Y * a.Y + a.Z * a.Z)) * Math.Sqrt((double)(b.X * b.X + b.Y * b.Y + b.Z * b.Z))));
			return (float)num;
		}
	}
}
