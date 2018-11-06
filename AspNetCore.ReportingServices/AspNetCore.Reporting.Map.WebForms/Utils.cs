using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class Utils
	{
		internal const float GoldenRatio = 1.618034f;

		private static ResourceManager resMng;

		internal static ResourceManager ResourceStr
		{
			get
			{
				return Utils.resMng;
			}
		}

		static Utils()
		{
			Utils.resMng = new ResourceManager(typeof(MapControl).Namespace + ".Resources.Strings", Assembly.GetExecutingAssembly());
		}

		public static string GetStack()
		{
			string text = "";
			StackTrace stackTrace = new StackTrace(true);
			for (int i = 0; i < stackTrace.FrameCount; i++)
			{
				StackFrame frame = stackTrace.GetFrame(i);
				text += string.Format(CultureInfo.CurrentCulture, "{0} at [{1}] {2} \n", frame.GetMethod(), frame.GetFileName(), frame.GetFileLineNumber());
			}
			return text;
		}

		public static void StartTrace()
		{
			if (Trace.Listeners.Count == 1)
			{
				Trace.Listeners.Add(new TextWriterTraceListener(File.AppendText("c:\\TestFile.txt")));
				Trace.AutoFlush = true;
			}
		}

		public static void StopTrace()
		{
			if (Trace.Listeners.Count > 2)
			{
				for (int i = 2; i < Trace.Listeners.Count; i++)
				{
					Trace.Listeners[i].Close();
				}
			}
		}

		internal static IEnumerable<PointF> GetRectangePoints(RectangleF rectangle)
		{
			PointF point = rectangle.Location;
			yield return point;
			point.X += rectangle.Width;
			yield return point;
			point.Y += rectangle.Height;
			yield return point;
			point.X -= rectangle.Width;
			yield return point;
			point.Y -= rectangle.Height;
			yield return point;
		}

		internal static IEnumerable<PointF> DensifyPoints(IEnumerable<PointF> points, double step)
		{
			PointF prevPoint = new PointF(3.40282347E+38f, 3.40282347E+38f);
			foreach (PointF point in points)
			{
				if (prevPoint.X != 3.4028234663852886E+38 && !prevPoint.Equals(point))
				{
					PointF pointF = point;
					float dx = pointF.X - prevPoint.X;
					PointF pointF2 = point;
					float dy = pointF2.Y - prevPoint.Y;
					int stepCountX = (int)Math.Round(Math.Abs((double)dx / step));
					int stepCountY = (int)Math.Round(Math.Abs((double)dy / step));
					int stepCount = Math.Max(stepCountX, stepCountY);
					if (stepCount > 0)
					{
						float stepX = dx / (float)stepCount;
						float stepY = dy / (float)stepCount;
						for (int i = 0; i < stepCount - 1; i++)
						{
							prevPoint.X += stepX;
							prevPoint.Y += stepY;
							yield return prevPoint;
						}
					}
				}
				yield return point;
				prevPoint = point;
			}
		}

		internal static float GetDistanceSqr(PointF pointA, PointF pointB)
		{
			double num = (double)(pointA.X - pointB.X);
			double num2 = (double)(pointA.Y - pointB.Y);
			return (float)(num * num + num2 * num2);
		}

		internal static double GetDistanceSqr(MapPoint pointA, MapPoint pointB)
		{
			double num = pointA.X - pointB.X;
			double num2 = pointA.Y - pointB.Y;
			return num * num + num2 * num2;
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

		internal static string GetImageCustomProperty(Image image, CustomPropertyTag customPropertyTag)
		{
			try
			{
				PropertyItem[] propertyItems = image.PropertyItems;
				foreach (PropertyItem propertyItem in propertyItems)
				{
					if (propertyItem.Id == (int)customPropertyTag && propertyItem.Value != null)
					{
						return Encoding.Unicode.GetString(propertyItem.Value);
					}
				}
			}
			catch
			{
				return string.Empty;
			}
			return string.Empty;
		}

		internal static void SetImageCustomProperty(Image image, CustomPropertyTag customPropertyTag, string text)
		{
			Type typeFromHandle = typeof(PropertyItem);
			ConstructorInfo constructorInfo = typeFromHandle.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0];
			PropertyItem propertyItem = (PropertyItem)constructorInfo.Invoke(null);
			propertyItem.Id = (int)customPropertyTag;
			propertyItem.Type = 1;
			propertyItem.Value = Encoding.Unicode.GetBytes(text);
			propertyItem.Len = propertyItem.Value.Length;
			image.SetPropertyItem(propertyItem);
		}
	}
}
