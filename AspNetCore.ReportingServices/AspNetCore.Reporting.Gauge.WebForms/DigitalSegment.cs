using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal static class DigitalSegment
	{
		internal const float widthRatio = 0.618034f;

		internal const float shearFactor = 0.0618034f;

		internal const float sgmntWidth7 = 0.142857149f;

		private static PointF[] GetSegment7(PointF p, SizeF s)
		{
			return new PointF[6]
			{
				new PointF((float)(p.X - s.Width / 2.0), p.Y),
				new PointF((float)(p.X - s.Width / 2.0 + s.Height / 2.0), (float)(p.Y - s.Height / 2.0)),
				new PointF((float)(p.X + s.Width / 2.0 - s.Height / 2.0), (float)(p.Y - s.Height / 2.0)),
				new PointF((float)(p.X + s.Width / 2.0), p.Y),
				new PointF((float)(p.X + s.Width / 2.0 - s.Height / 2.0), (float)(p.Y + s.Height / 2.0)),
				new PointF((float)(p.X - s.Width / 2.0 + s.Height / 2.0), (float)(p.Y + s.Height / 2.0))
			};
		}

		private static PointF[] GetSegmentHKLN(PointF p, SizeF s, float smallWidth, bool left)
		{
			using (GraphicsPath graphicsPath = new GraphicsPath())
			{
				RectangleF rect = new RectangleF(0f, 0f, (float)(s.Width / 2.0 - smallWidth - smallWidth / 6.0), (float)(smallWidth / 0.6180340051651001));
				rect.X = (float)((0.0 - rect.Width) / 2.0);
				rect.Y = (float)((0.0 - rect.Height) / 2.0);
				graphicsPath.AddRectangle(rect);
				using (Matrix matrix = new Matrix())
				{
					matrix.Shear(0f, (float)(left ? 1.2999999523162842 : -1.2999999523162842));
					graphicsPath.Transform(matrix);
					matrix.Reset();
					matrix.Translate(p.X, p.Y);
					graphicsPath.Transform(matrix);
				}
				return (PointF[])graphicsPath.PathPoints.Clone();
			}
		}

		private static GraphicsPath GetSegment7(LEDSegment7 segment, PointF p, float size)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			SizeF sizeF = new SizeF((float)(size * 0.6180340051651001), size);
			float num = (float)(sizeF.Width * 0.1428571492433548);
			SizeF s = new SizeF(sizeF.Width - num, num);
			SizeF s2 = new SizeF((float)(sizeF.Height / 2.0 - num / 2.0), num);
			s.Width -= (float)(num / 3.0);
			s2.Width -= (float)(num / 3.0);
			if ((long)segment <= 16L)
			{
				if ((long)segment <= 4L)
				{
					if ((long)segment < 1L)
					{
						goto IL_04d0;
					}
					switch (segment - 1)
					{
					case LEDSegment7.Empty:
						graphicsPath.AddPolygon(DigitalSegment.GetSegment7(p, s));
						graphicsPath.CloseAllFigures();
						using (Matrix matrix3 = new Matrix())
						{
							matrix3.Translate(0f, (float)((0.0 - sizeF.Height) / 2.0 + num / 2.0));
							graphicsPath.Transform(matrix3);
							return graphicsPath;
						}
					case LEDSegment7.SA | LEDSegment7.SB:
						graphicsPath.AddPolygon(DigitalSegment.GetSegment7(p, s2));
						graphicsPath.CloseAllFigures();
						using (Matrix matrix2 = new Matrix())
						{
							matrix2.RotateAt(90f, p);
							graphicsPath.Transform(matrix2);
							matrix2.Reset();
							matrix2.Translate((float)(sizeF.Width / 2.0 - num / 2.0), (float)(sizeF.Height / 4.0 - num / 4.0));
							graphicsPath.Transform(matrix2);
							return graphicsPath;
						}
					case LEDSegment7.SA:
						graphicsPath.AddPolygon(DigitalSegment.GetSegment7(p, s2));
						graphicsPath.CloseAllFigures();
						using (Matrix matrix = new Matrix())
						{
							matrix.RotateAt(90f, p);
							graphicsPath.Transform(matrix);
							matrix.Reset();
							matrix.Translate((float)(sizeF.Width / 2.0 - num / 2.0), (float)((0.0 - sizeF.Height) / 4.0 + num / 4.0));
							graphicsPath.Transform(matrix);
							return graphicsPath;
						}
					case LEDSegment7.SB:
						goto IL_04d0;
					}
				}
				switch (segment)
				{
				case LEDSegment7.SD:
					graphicsPath.AddPolygon(DigitalSegment.GetSegment7(p, s));
					graphicsPath.CloseAllFigures();
					using (Matrix matrix5 = new Matrix())
					{
						matrix5.RotateAt(180f, p);
						graphicsPath.Transform(matrix5);
						matrix5.Reset();
						matrix5.Translate(0f, (float)(sizeF.Height / 2.0 - num / 2.0));
						graphicsPath.Transform(matrix5);
						return graphicsPath;
					}
				case LEDSegment7.SE:
					graphicsPath.AddPolygon(DigitalSegment.GetSegment7(p, s2));
					graphicsPath.CloseAllFigures();
					using (Matrix matrix4 = new Matrix())
					{
						matrix4.RotateAt(90f, p);
						graphicsPath.Transform(matrix4);
						matrix4.Reset();
						matrix4.Translate((float)((0.0 - sizeF.Width) / 2.0 + num / 2.0), (float)(sizeF.Height / 4.0 - num / 4.0));
						graphicsPath.Transform(matrix4);
						return graphicsPath;
					}
				}
			}
			else
			{
				switch (segment)
				{
				case LEDSegment7.SG:
					graphicsPath.AddPolygon(DigitalSegment.GetSegment7(p, s));
					graphicsPath.CloseAllFigures();
					break;
				case LEDSegment7.SF:
					graphicsPath.AddPolygon(DigitalSegment.GetSegment7(p, s2));
					graphicsPath.CloseAllFigures();
					using (Matrix matrix8 = new Matrix())
					{
						matrix8.RotateAt(90f, p);
						graphicsPath.Transform(matrix8);
						matrix8.Reset();
						matrix8.Translate((float)((0.0 - sizeF.Width) / 2.0 + num / 2.0), (float)((0.0 - sizeF.Height) / 4.0 + num / 4.0));
						graphicsPath.Transform(matrix8);
						return graphicsPath;
					}
				case LEDSegment7.SDP:
					graphicsPath.AddEllipse(p.X, p.Y, (float)(num * 2.0), (float)(num * 2.0));
					using (Matrix matrix7 = new Matrix())
					{
						matrix7.Translate((float)(sizeF.Width / 2.0 + num / 2.0), (float)(sizeF.Height / 2.0 - num * 2.0));
						graphicsPath.Transform(matrix7);
						return graphicsPath;
					}
				case LEDSegment7.SComma:
					graphicsPath.AddRectangle(new RectangleF(p.X, p.Y, (float)(num * 2.0), (float)(num * 4.0)));
					using (Matrix matrix6 = new Matrix())
					{
						matrix6.Translate((float)(sizeF.Width / 2.0 + num / 2.0), (float)(sizeF.Height / 2.0 - num * 2.0));
						graphicsPath.Transform(matrix6);
						return graphicsPath;
					}
				}
			}
			goto IL_04d0;
			IL_04d0:
			return graphicsPath;
		}

		private static GraphicsPath GetSegment14(LEDSegment14 segment, PointF p, float size)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			SizeF sizeF = new SizeF((float)(size * 0.6180340051651001), size);
			float num = (float)(sizeF.Width * 0.1428571492433548);
			SizeF s = new SizeF(sizeF.Width - num, num);
			SizeF s2 = new SizeF((float)(sizeF.Height / 2.0 - num / 2.0), num);
			s.Width -= (float)(num / 3.0);
			s2.Width -= (float)(num / 3.0);
			switch (segment)
			{
			case LEDSegment14.SG1:
				s.Width = (float)(s.Width / 2.0 - num / 6.0);
				graphicsPath.AddPolygon(DigitalSegment.GetSegment7(p, s));
				graphicsPath.CloseAllFigures();
				using (Matrix matrix8 = new Matrix())
				{
					matrix8.Translate((float)((0.0 - s.Width) / 2.0 - num / 6.0), 0f);
					graphicsPath.Transform(matrix8);
					return graphicsPath;
				}
			case LEDSegment14.SG2:
				s.Width = (float)(s.Width / 2.0 - num / 6.0);
				graphicsPath.AddPolygon(DigitalSegment.GetSegment7(p, s));
				graphicsPath.CloseAllFigures();
				using (Matrix matrix7 = new Matrix())
				{
					matrix7.Translate((float)(s.Width / 2.0 + num / 6.0), 0f);
					graphicsPath.Transform(matrix7);
					return graphicsPath;
				}
			case LEDSegment14.SJ:
				s2.Width -= (float)(num / 3.0);
				graphicsPath.AddPolygon(DigitalSegment.GetSegment7(p, s2));
				graphicsPath.CloseAllFigures();
				using (Matrix matrix6 = new Matrix())
				{
					matrix6.RotateAt(90f, p);
					graphicsPath.Transform(matrix6);
					matrix6.Reset();
					matrix6.Translate(0f, (float)((0.0 - s2.Width) / 2.0 - num / 6.0));
					graphicsPath.Transform(matrix6);
					return graphicsPath;
				}
			case LEDSegment14.SM:
				s2.Width -= (float)(num / 3.0);
				graphicsPath.AddPolygon(DigitalSegment.GetSegment7(p, s2));
				graphicsPath.CloseAllFigures();
				using (Matrix matrix5 = new Matrix())
				{
					matrix5.RotateAt(90f, p);
					graphicsPath.Transform(matrix5);
					matrix5.Reset();
					matrix5.Translate(0f, (float)(s2.Width / 2.0 + num / 6.0));
					graphicsPath.Transform(matrix5);
					return graphicsPath;
				}
			case LEDSegment14.SH:
				graphicsPath.AddPolygon(DigitalSegment.GetSegmentHKLN(p, s, num, true));
				using (Matrix matrix4 = new Matrix())
				{
					matrix4.Translate((float)((0.0 - (s.Width / 2.0 + num / 6.0)) / 2.0), (float)((0.0 - s2.Width) / 2.0 - num / 6.0));
					graphicsPath.Transform(matrix4);
					return graphicsPath;
				}
			case LEDSegment14.SL:
				graphicsPath.AddPolygon(DigitalSegment.GetSegmentHKLN(p, s, num, true));
				using (Matrix matrix3 = new Matrix())
				{
					matrix3.Translate((float)((s.Width / 2.0 + num / 6.0) / 2.0), (float)(s2.Width / 2.0 - num / 6.0));
					graphicsPath.Transform(matrix3);
					return graphicsPath;
				}
			case LEDSegment14.SK:
				graphicsPath.AddPolygon(DigitalSegment.GetSegmentHKLN(p, s, num, false));
				using (Matrix matrix2 = new Matrix())
				{
					matrix2.Translate((float)((s.Width / 2.0 + num / 6.0) / 2.0), (float)((0.0 - s2.Width) / 2.0 - num / 6.0));
					graphicsPath.Transform(matrix2);
					return graphicsPath;
				}
			case LEDSegment14.SN:
				graphicsPath.AddPolygon(DigitalSegment.GetSegmentHKLN(p, s, num, false));
				using (Matrix matrix = new Matrix())
				{
					matrix.Translate((float)((0.0 - (s.Width / 2.0 + num / 6.0)) / 2.0), (float)(s2.Width / 2.0 - num / 6.0));
					graphicsPath.Transform(matrix);
					return graphicsPath;
				}
			default:
				return graphicsPath;
			}
		}

		internal static GraphicsPath GetSegments(LEDSegment7 segments, PointF point, float size)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			PointF p = new PointF(0f, 0f);
			if ((segments & LEDSegment7.SA) == LEDSegment7.SA)
			{
				using (GraphicsPath graphicsPath2 = DigitalSegment.GetSegment7(LEDSegment7.SA, p, size))
				{
					if (graphicsPath2.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath2, false);
					}
				}
			}
			if ((segments & LEDSegment7.SB) == LEDSegment7.SB)
			{
				using (GraphicsPath graphicsPath3 = DigitalSegment.GetSegment7(LEDSegment7.SB, p, size))
				{
					if (graphicsPath3.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath3, false);
					}
				}
			}
			if ((segments & LEDSegment7.SC) == LEDSegment7.SC)
			{
				using (GraphicsPath graphicsPath4 = DigitalSegment.GetSegment7(LEDSegment7.SC, p, size))
				{
					if (graphicsPath4.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath4, false);
					}
				}
			}
			if ((segments & LEDSegment7.SD) == LEDSegment7.SD)
			{
				using (GraphicsPath graphicsPath5 = DigitalSegment.GetSegment7(LEDSegment7.SD, p, size))
				{
					if (graphicsPath5.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath5, false);
					}
				}
			}
			if ((segments & LEDSegment7.SE) == LEDSegment7.SE)
			{
				using (GraphicsPath graphicsPath6 = DigitalSegment.GetSegment7(LEDSegment7.SE, p, size))
				{
					if (graphicsPath6.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath6, false);
					}
				}
			}
			if ((segments & LEDSegment7.SF) == LEDSegment7.SF)
			{
				using (GraphicsPath graphicsPath7 = DigitalSegment.GetSegment7(LEDSegment7.SF, p, size))
				{
					if (graphicsPath7.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath7, false);
					}
				}
			}
			if ((segments & LEDSegment7.SG) == LEDSegment7.SG)
			{
				using (GraphicsPath graphicsPath8 = DigitalSegment.GetSegment7(LEDSegment7.SG, p, size))
				{
					if (graphicsPath8.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath8, false);
					}
				}
			}
			if ((segments & LEDSegment7.SDP) == LEDSegment7.SDP)
			{
				using (GraphicsPath graphicsPath9 = DigitalSegment.GetSegment7(LEDSegment7.SDP, p, size))
				{
					if (graphicsPath9.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath9, false);
					}
				}
			}
			if ((segments & LEDSegment7.SComma) == LEDSegment7.SComma)
			{
				using (GraphicsPath graphicsPath10 = DigitalSegment.GetSegment7(LEDSegment7.SComma, p, size))
				{
					if (graphicsPath10.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath10, false);
						return graphicsPath;
					}
					return graphicsPath;
				}
			}
			return graphicsPath;
		}

		internal static GraphicsPath GetOrientedSegments(LEDSegment7 segments, PointF point, float size, SegmentsCache cache)
		{
			GraphicsPath graphicsPath = cache.GetSegment((Enum)(object)segments, point, size);
			if (graphicsPath == null)
			{
				graphicsPath = DigitalSegment.GetSegments(segments, point, size);
				using (Matrix matrix = new Matrix())
				{
					matrix.Shear(-0.0618034f, 0f);
					graphicsPath.Transform(matrix);
					matrix.Reset();
					matrix.Translate(point.X, point.Y);
					graphicsPath.Transform(matrix);
					matrix.Reset();
				}
				cache.SetSegment((Enum)(object)segments, graphicsPath, point, size);
			}
			return graphicsPath;
		}

		internal static GraphicsPath GetSegments(LEDSegment14 segments, PointF point, float size)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			PointF p = new PointF(0f, 0f);
			if ((segments & LEDSegment14.SA) == LEDSegment14.SA)
			{
				using (GraphicsPath graphicsPath2 = DigitalSegment.GetSegment7(LEDSegment7.SA, p, size))
				{
					if (graphicsPath2.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath2, false);
					}
				}
			}
			if ((segments & LEDSegment14.SB) == LEDSegment14.SB)
			{
				using (GraphicsPath graphicsPath3 = DigitalSegment.GetSegment7(LEDSegment7.SB, p, size))
				{
					if (graphicsPath3.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath3, false);
					}
				}
			}
			if ((segments & LEDSegment14.SC) == LEDSegment14.SC)
			{
				using (GraphicsPath graphicsPath4 = DigitalSegment.GetSegment7(LEDSegment7.SC, p, size))
				{
					if (graphicsPath4.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath4, false);
					}
				}
			}
			if ((segments & LEDSegment14.SD) == LEDSegment14.SD)
			{
				using (GraphicsPath graphicsPath5 = DigitalSegment.GetSegment7(LEDSegment7.SD, p, size))
				{
					if (graphicsPath5.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath5, false);
					}
				}
			}
			if ((segments & LEDSegment14.SE) == LEDSegment14.SE)
			{
				using (GraphicsPath graphicsPath6 = DigitalSegment.GetSegment7(LEDSegment7.SE, p, size))
				{
					if (graphicsPath6.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath6, false);
					}
				}
			}
			if ((segments & LEDSegment14.SF) == LEDSegment14.SF)
			{
				using (GraphicsPath graphicsPath7 = DigitalSegment.GetSegment7(LEDSegment7.SF, p, size))
				{
					if (graphicsPath7.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath7, false);
					}
				}
			}
			if ((segments & LEDSegment14.SDP) == LEDSegment14.SDP)
			{
				using (GraphicsPath graphicsPath8 = DigitalSegment.GetSegment7(LEDSegment7.SDP, p, size))
				{
					if (graphicsPath8.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath8, false);
					}
				}
			}
			if ((segments & LEDSegment14.SComma) == LEDSegment14.SComma)
			{
				using (GraphicsPath graphicsPath9 = DigitalSegment.GetSegment7(LEDSegment7.SComma, p, size))
				{
					if (graphicsPath9.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath9, false);
					}
				}
			}
			if ((segments & LEDSegment14.SG1) == LEDSegment14.SG1)
			{
				using (GraphicsPath graphicsPath10 = DigitalSegment.GetSegment14(LEDSegment14.SG1, p, size))
				{
					if (graphicsPath10.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath10, false);
					}
				}
			}
			if ((segments & LEDSegment14.SG2) == LEDSegment14.SG2)
			{
				using (GraphicsPath graphicsPath11 = DigitalSegment.GetSegment14(LEDSegment14.SG2, p, size))
				{
					if (graphicsPath11.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath11, false);
					}
				}
			}
			if ((segments & LEDSegment14.SJ) == LEDSegment14.SJ)
			{
				using (GraphicsPath graphicsPath12 = DigitalSegment.GetSegment14(LEDSegment14.SJ, p, size))
				{
					if (graphicsPath12.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath12, false);
					}
				}
			}
			if ((segments & LEDSegment14.SM) == LEDSegment14.SM)
			{
				using (GraphicsPath graphicsPath13 = DigitalSegment.GetSegment14(LEDSegment14.SM, p, size))
				{
					if (graphicsPath13.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath13, false);
					}
				}
			}
			if ((segments & LEDSegment14.SH) == LEDSegment14.SH)
			{
				using (GraphicsPath graphicsPath14 = DigitalSegment.GetSegment14(LEDSegment14.SH, p, size))
				{
					if (graphicsPath14.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath14, false);
					}
				}
			}
			if ((segments & LEDSegment14.SK) == LEDSegment14.SK)
			{
				using (GraphicsPath graphicsPath15 = DigitalSegment.GetSegment14(LEDSegment14.SK, p, size))
				{
					if (graphicsPath15.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath15, false);
					}
				}
			}
			if ((segments & LEDSegment14.SL) == LEDSegment14.SL)
			{
				using (GraphicsPath graphicsPath16 = DigitalSegment.GetSegment14(LEDSegment14.SL, p, size))
				{
					if (graphicsPath16.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath16, false);
					}
				}
			}
			if ((segments & LEDSegment14.SN) == LEDSegment14.SN)
			{
				using (GraphicsPath graphicsPath17 = DigitalSegment.GetSegment14(LEDSegment14.SN, p, size))
				{
					if (graphicsPath17.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath17, false);
						return graphicsPath;
					}
					return graphicsPath;
				}
			}
			return graphicsPath;
		}

		internal static GraphicsPath GetOrientedSegments(LEDSegment14 segments, PointF point, float size, SegmentsCache cache)
		{
			GraphicsPath graphicsPath = cache.GetSegment((Enum)(object)segments, point, size);
			if (graphicsPath == null)
			{
				graphicsPath = DigitalSegment.GetSegments(segments, point, size);
				using (Matrix matrix = new Matrix())
				{
					matrix.Shear(-0.0618034f, 0f);
					graphicsPath.Transform(matrix);
					matrix.Reset();
					matrix.Translate(point.X, point.Y);
					graphicsPath.Transform(matrix);
					matrix.Reset();
				}
				cache.SetSegment((Enum)(object)segments, graphicsPath, point, size);
			}
			return graphicsPath;
		}

		internal static GraphicsPath GetSymbol7(char symbol, PointF point, float size, bool decDot, bool comma, bool sepDots, SegmentsCache cache)
		{
			LEDSegment7 lEDSegment = LEDSegment7.Empty;
			if (char.IsDigit(symbol))
			{
				lEDSegment = (LEDSegment7)Enum.Parse(typeof(LEDSegment7), "N" + symbol);
			}
			else
			{
				switch (symbol)
				{
				case '-':
					lEDSegment = LEDSegment7.SG;
					break;
				case ' ':
					lEDSegment = LEDSegment7.Empty;
					break;
				case '+':
					lEDSegment = LEDSegment7.Empty;
					break;
				default:
					try
					{
						lEDSegment = (LEDSegment7)Enum.Parse(typeof(LEDSegment7), "C" + symbol);
					}
					catch
					{
						lEDSegment = LEDSegment7.Unknown;
					}
					break;
				}
			}
			if (decDot)
			{
				lEDSegment |= LEDSegment7.SDP;
			}
			if (comma)
			{
				lEDSegment |= LEDSegment7.SComma;
			}
			return DigitalSegment.GetOrientedSegments(lEDSegment, point, size, cache);
		}

		internal static GraphicsPath GetSymbol14(char symbol, PointF point, float size, bool decDot, bool comma, bool sepDots, SegmentsCache cache)
		{
			LEDSegment14 lEDSegment = LEDSegment14.Empty;
			if (char.IsDigit(symbol))
			{
				lEDSegment = (LEDSegment14)Enum.Parse(typeof(LEDSegment14), "N" + symbol);
			}
			else
			{
				switch (symbol)
				{
				case ' ':
					lEDSegment = LEDSegment14.Empty;
					break;
				case '-':
					lEDSegment = LEDSegment14.SG;
					break;
				case '+':
					lEDSegment = LEDSegment14.Plus;
					break;
				case '$':
					lEDSegment = LEDSegment14.CDollar;
					break;
				default:
					try
					{
						lEDSegment = (LEDSegment14)Enum.Parse(typeof(LEDSegment14), "C" + symbol);
					}
					catch
					{
						lEDSegment = LEDSegment14.Unknown;
					}
					break;
				}
			}
			if (decDot)
			{
				lEDSegment |= LEDSegment14.SDP;
			}
			if (comma)
			{
				lEDSegment |= LEDSegment14.SComma;
			}
			return DigitalSegment.GetOrientedSegments(lEDSegment, point, size, cache);
		}
	}
}
