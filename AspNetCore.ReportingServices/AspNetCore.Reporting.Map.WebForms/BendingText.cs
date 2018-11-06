using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class BendingText
	{
		protected char WHITESPACE_SUBSTITUTE = '-';

		public GraphicsPath CreatePath(Font font, PointF[] points, string text, int startIndex, int labelOffset)
		{
			GraphicsPath result = default(GraphicsPath);
			int num = this.BendText(out result, points, text, font, startIndex, false, true, labelOffset);
			if (num != -1)
			{
				if (points[num].X < points[startIndex].X)
				{
					this.BendText(out result, points, text, font, startIndex, true, false, labelOffset);
				}
				else
				{
					this.BendText(out result, points, text, font, startIndex, false, false, labelOffset);
				}
			}
			return result;
		}

		protected int BendText(out GraphicsPath gBendedText, PointF[] points, string text, Font font, int startIndex, bool backward, bool calculateOnly, int labelOffset)
		{
			gBendedText = null;
			using (GraphicsPath graphicsPath = new GraphicsPath())
			{
				graphicsPath.AddString(text, font.FontFamily, (int)font.Style, font.Size, new Point(0, 0), new StringFormat());
				double num = (double)(graphicsPath.GetBounds().Height + graphicsPath.GetBounds().Y);
				graphicsPath.Reset();
				double num2 = 0.0;
				double num3 = 0.0;
				double num4 = 0.0;
				double num5 = 0.0;
				int num6 = checked(startIndex + 1);
				bool flag = false;
				int num7 = backward ? (text.Length - 1) : 0;
				while (!flag)
				{
					char c = text[num7];
					bool flag2;
					if (char.IsWhiteSpace(c))
					{
						flag2 = true;
						c = this.WHITESPACE_SUBSTITUTE;
					}
					else
					{
						flag2 = false;
					}
					graphicsPath.AddString(c.ToString(CultureInfo.CurrentCulture), font.FontFamily, (int)font.Style, font.Size, new Point(0, 0), new StringFormat());
					double num8 = (double)(graphicsPath.GetBounds().Width + graphicsPath.GetBounds().X);
					double num9 = (double)graphicsPath.GetBounds().X;
					double num10 = num / 2.0;
					if (backward)
					{
						Matrix matrix = new Matrix();
						matrix.RotateAt(180f, new PointF((float)(graphicsPath.GetBounds().X + graphicsPath.GetBounds().Width / 2.0), (float)(num / 2.0)));
						graphicsPath.Transform(matrix);
					}
					if (num6 < points.Length)
					{
						if (num2 != 0.0)
						{
							if (num2 < num8)
							{
								num6 = this.FindNearestPoint(points, (PointF)new Point((int)num4, (int)num5), startIndex, num8, out num2);
								num3 = Math.Asin(((double)points[num6].Y - num5) / num2);
								if ((double)points[num6].X < num4)
								{
									num3 = 3.1415926535897931 - num3;
								}
							}
						}
						else
						{
							num6 = this.FindNearestPoint(points, startIndex, num8, out num2);
							num4 = (double)points[startIndex].X;
							num5 = (double)points[startIndex].Y;
							num3 = Math.Asin((double)(points[num6].Y - points[startIndex].Y) / num2);
							if (points[num6].X < points[startIndex].X)
							{
								num3 = 3.1415926535897931 - num3;
							}
						}
					}
					Matrix matrix2 = new Matrix();
					matrix2.RotateAt((float)(num3 * 180.0 / 3.1415926535897931), new PointF((float)num9, (float)num10));
					Matrix matrix3 = new Matrix();
					matrix3.Translate((float)(num4 - num9), (float)(num5 - num10));
					graphicsPath.Transform(matrix2);
					graphicsPath.Transform(matrix3);
					if (!calculateOnly && !flag2)
					{
						if (gBendedText == null)
						{
							gBendedText = new GraphicsPath();
						}
						gBendedText.AddPath(graphicsPath, false);
					}
					graphicsPath.Reset();
					if (num2 < num8)
					{
						num6++;
					}
					startIndex = num6;
					num5 += num8 * Math.Sin(num3);
					num4 += num8 * Math.Cos(num3);
					num2 -= num8;
					if (backward)
					{
						if (num7 == 0)
						{
							flag = true;
						}
						else
						{
							num7--;
						}
					}
					else if (num7 == text.Length - 1)
					{
						flag = true;
					}
					else
					{
						num7++;
					}
				}
				if (num6 < points.Length && num2 >= 0.0)
				{
					return num6;
				}
				return -1;
			}
		}

		private int FindNearestPoint(PointF[] points, int startIndex, double distanceDesired, out double distanceFound)
		{
			return this.FindNearestPoint(points, points[startIndex], startIndex + 1, distanceDesired, out distanceFound);
		}

		private int FindNearestPoint(PointF[] points, PointF point, int startIndex, double distanceDesired, out double distanceFound)
		{
			int i = startIndex;
			int result = startIndex;
			double num = distanceDesired * distanceDesired;
			double num2 = 0.0;
			for (; i < points.Length; i++)
			{
				double num3 = (double)((point.X - points[i].X) * (point.X - points[i].X) + (point.Y - points[i].Y) * (point.Y - points[i].Y));
				if (num3 > num && (Math.Abs(num3 - num) < Math.Abs(num2 - num) || num2 == 0.0))
				{
					num2 = num3;
					result = i;
				}
				if (num3 > num2 && num3 > num)
				{
					distanceFound = Math.Sqrt(num2);
					return result;
				}
			}
			distanceFound = Math.Sqrt(num2);
			return result;
		}
	}
}
