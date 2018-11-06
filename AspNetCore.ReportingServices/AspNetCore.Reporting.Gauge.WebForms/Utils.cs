using System;
using System.Drawing;
using System.Globalization;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal static class Utils
	{
		internal const float GoldenRatio = 1.618034f;

		internal static string SRGetStr(string key, params object[] p)
		{
			return string.Format(CultureInfo.CurrentCulture, SR.Keys.GetString(key), p);
		}

		internal static double Round(double value, int precision)
		{
			if (precision >= 0)
			{
				return Math.Round(value, precision);
			}
			precision = -precision;
			double num = Math.Pow(10.0, (double)precision);
			return Math.Round(value / num, 0) * num;
		}

		internal static float Deg2Rad(float angleInDegree)
		{
			return (float)((double)Math.Abs(angleInDegree) * 3.1415926535897931 / 180.0);
		}

		internal static float Rad2Deg(float angleInRadians)
		{
			return (float)((double)Math.Abs(angleInRadians) / 3.1415926535897931 * 180.0);
		}

		internal static float NormalizeAngle(float angle)
		{
			if (angle < 0.0)
			{
				return (float)(360.0 - angle);
			}
			if (angle > 360.0)
			{
				return (float)(angle - 360.0);
			}
			return angle;
		}

		internal static float GetContactPointOffset(SizeF size, float angle)
		{
			angle = Utils.NormalizeAngle(Math.Abs(angle));
			if (angle >= 180.0)
			{
				angle = (float)(angle % 180.0);
			}
			if (angle % 180.0 > 90.0)
			{
				angle = (float)(180.0 - angle % 180.0);
			}
			float num = Utils.Rad2Deg((float)Math.Atan((double)(size.Width / size.Height)));
			float num2 = 0f;
			if (angle >= num)
			{
				return (float)(size.Width / 2.0 / Math.Sin((double)Utils.Deg2Rad(angle)));
			}
			return (float)(size.Height / 2.0 / Math.Cos((double)Utils.Deg2Rad(angle)));
		}

		internal static float ToGDIAngle(float angle)
		{
			angle = (float)(angle + 90.0);
			if (!(angle > 360.0))
			{
				return angle;
			}
			return (float)(angle - 360.0);
		}

		internal static RectangleF NormalizeRectangle(RectangleF boundRect, SizeF insetSize, bool resizeResult)
		{
			RectangleF result = boundRect;
			if (resizeResult)
			{
				float num = insetSize.Width / insetSize.Height;
				float num2 = boundRect.Size.Width / boundRect.Size.Height;
				if (num2 > num)
				{
					result.Height = boundRect.Size.Height;
					result.Width = result.Height * num;
					result.X += (float)((boundRect.Size.Width - result.Width) / 2.0);
				}
				else
				{
					result.Width = boundRect.Size.Width;
					result.Height = result.Width / num;
					result.Y += (float)((boundRect.Size.Height - result.Height) / 2.0);
				}
			}
			else
			{
				result.Width = insetSize.Width;
				result.Height = insetSize.Height;
				result.X += (float)((boundRect.Size.Width - result.Width) / 2.0);
				result.Y += (float)((boundRect.Size.Height - result.Height) / 2.0);
			}
			return result;
		}
	}
}
