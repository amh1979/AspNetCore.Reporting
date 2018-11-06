using AspNetCore.ReportingServices.Interfaces;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ImageRenderer
{
	internal sealed class SharedRenderer
	{
		internal const float INCH_TO_MILLIMETER = 25.4f;

		public const float MIN_DOUBLE_BORDER_SIZE = 0.5292f;

		private SharedRenderer()
		{
		}

		internal static void CalculateImageRectangle(RectangleF position, GDIImageProps gdiProperties, RPLFormat.Sizings sizing, out RectangleF imagePositionAndSize, out RectangleF imagePortion)
		{
			SharedRenderer.CalculateImageRectangle(position, gdiProperties.Width, gdiProperties.Height, gdiProperties.HorizontalResolution, gdiProperties.VerticalResolution, sizing, out imagePositionAndSize, out imagePortion);
		}

		internal static void CalculateImageRectangle(RectangleF position, int width, int height, float horizontalResolution, float verticalResolution, RPLFormat.Sizings sizing, out RectangleF imagePositionAndSize, out RectangleF imagePortion)
		{
			imagePositionAndSize = position;
			if (sizing == RPLFormat.Sizings.Clip)
			{
				imagePortion = new RectangleF(0f, 0f, (float)width, (float)height);
				float num = (float)SharedRenderer.ConvertToPixels(imagePositionAndSize.Width, horizontalResolution);
				if ((float)width > num)
				{
					imagePortion.Width = num;
				}
				else
				{
					imagePositionAndSize.Width = SharedRenderer.ConvertToMillimeters(width, horizontalResolution);
				}
				float num2 = (float)SharedRenderer.ConvertToPixels(imagePositionAndSize.Height, verticalResolution);
				if ((float)height > num2)
				{
					imagePortion.Height = num2;
				}
				else
				{
					imagePositionAndSize.Height = SharedRenderer.ConvertToMillimeters(height, verticalResolution);
				}
			}
			else
			{
				imagePortion = new RectangleF(0f, 0f, (float)width, (float)height);
				switch (sizing)
				{
				case RPLFormat.Sizings.AutoSize:
					imagePositionAndSize.Width = SharedRenderer.ConvertToMillimeters(width, horizontalResolution);
					imagePositionAndSize.Height = SharedRenderer.ConvertToMillimeters(height, verticalResolution);
					break;
				case RPLFormat.Sizings.FitProportional:
				{
					float num3 = SharedRenderer.ConvertToMillimeters(width, horizontalResolution);
					float num4 = SharedRenderer.ConvertToMillimeters(height, verticalResolution);
					float num5 = position.Width / num3;
					float num6 = position.Height / num4;
					if (num5 > num6)
					{
						imagePositionAndSize.Width = num3 * num6;
					}
					else
					{
						imagePositionAndSize.Height = num4 * num5;
					}
					break;
				}
				}
			}
		}

		internal static float ConvertToMillimeters(int pixels, float dpi)
		{
			if (dpi == 0.0)
			{
				return 3.40282347E+38f;
			}
			return (float)(1.0 / dpi * (float)pixels * 25.399999618530273);
		}

		internal static int ConvertToPixels(float mm, float dpi)
		{
			return Convert.ToInt32((double)dpi * 0.03937007874 * (double)mm);
		}

		internal static float ConvertToMillimeters(int pixels, float? dpi, WriterBase writer)
		{
			if (dpi.HasValue)
			{
				return SharedRenderer.ConvertToMillimeters(pixels, dpi.Value);
			}
			return writer.ConvertToMillimeters(pixels);
		}

		internal static int ConvertToPixels(float mm, float? dpi, WriterBase writer)
		{
			if (dpi.HasValue)
			{
				return SharedRenderer.ConvertToPixels(mm, dpi.Value);
			}
			return writer.ConvertToPixels(mm);
		}

		internal static void DrawImage(System.Drawing.Graphics graphics, Image image, RectangleF rectDestMM, RectangleF rectSourcePX)
		{
			SharedRenderer.DrawImage(graphics, image, rectDestMM, rectSourcePX, null);
		}

		internal static void DrawImage(System.Drawing.Graphics graphics, Image image, RectangleF rectDestMM, RectangleF rectSourcePX, ImageAttributes imageAttributes)
		{
			SharedRenderer.DrawImage(graphics, image, new PointF[3]
			{
				rectDestMM.Location,
				new PointF(rectDestMM.Location.X + rectDestMM.Width, rectDestMM.Location.Y),
				new PointF(rectDestMM.Location.X, rectDestMM.Location.Y + rectDestMM.Height)
			}, rectSourcePX, imageAttributes);
		}

		internal static void DrawImage(System.Drawing.Graphics graphics, Image image, PointF[] pointsDestMM, RectangleF rectSourcePX, ImageAttributes imageAttributes)
		{
			PointF[] destPoints = new PointF[3]
			{
				new PointF((float)SharedRenderer.ConvertToPixels(pointsDestMM[0].X, graphics.DpiX), (float)SharedRenderer.ConvertToPixels(pointsDestMM[0].Y, graphics.DpiY)),
				new PointF((float)SharedRenderer.ConvertToPixels(pointsDestMM[1].X, graphics.DpiX), (float)SharedRenderer.ConvertToPixels(pointsDestMM[1].Y, graphics.DpiY)),
				new PointF((float)SharedRenderer.ConvertToPixels(pointsDestMM[2].X, graphics.DpiX), (float)SharedRenderer.ConvertToPixels(pointsDestMM[2].Y, graphics.DpiY))
			};
			using (Matrix matrix = graphics.Transform)
			{
				using (Matrix transform = new Matrix(matrix.Elements[0], matrix.Elements[1], matrix.Elements[2], matrix.Elements[3], (float)SharedRenderer.ConvertToPixels(matrix.Elements[4], graphics.DpiX), (float)SharedRenderer.ConvertToPixels(matrix.Elements[5], graphics.DpiY)))
				{
					graphics.Transform = transform;
					graphics.PageUnit = GraphicsUnit.Pixel;
					graphics.DrawImage(image, destPoints, rectSourcePX, GraphicsUnit.Pixel, imageAttributes);
					graphics.PageUnit = GraphicsUnit.Millimeter;
					graphics.Transform = matrix;
				}
			}
		}

		internal static void GetFontFormatInformation(RPLElementProps elementProperties, out RPLFormat.WritingModes writingMode, out RPLFormat.Directions direction, out RPLFormat.VerticalAlignments verticalAlign, out RPLFormat.TextAlignments textAlign, ref bool stringFormatFromInstance)
		{
			writingMode = (RPLFormat.WritingModes)SharedRenderer.GetStylePropertyValueObject(elementProperties, (byte)30, ref stringFormatFromInstance);
			direction = (RPLFormat.Directions)SharedRenderer.GetStylePropertyValueObject(elementProperties, (byte)29, ref stringFormatFromInstance);
			verticalAlign = (RPLFormat.VerticalAlignments)SharedRenderer.GetStylePropertyValueObject(elementProperties, (byte)26, ref stringFormatFromInstance);
			textAlign = (RPLFormat.TextAlignments)SharedRenderer.GetStylePropertyValueObject(elementProperties, (byte)25, ref stringFormatFromInstance);
		}

		internal static void GetFontStyleInformation(RPLElementProps elementProperties, out RPLFormat.FontStyles fontStyle, out RPLFormat.FontWeights fontWeight, out RPLFormat.TextDecorations textDecoration, out float fontSize, out string fontFamily, ref bool fontStyleFromInstance)
		{
			fontSize = (float)new RPLReportSize(SharedRenderer.GetStylePropertyValueString(elementProperties, (byte)21, ref fontStyleFromInstance)).ToPoints();
			fontStyle = (RPLFormat.FontStyles)SharedRenderer.GetStylePropertyValueObject(elementProperties, (byte)19, ref fontStyleFromInstance);
			fontWeight = (RPLFormat.FontWeights)SharedRenderer.GetStylePropertyValueObject(elementProperties, (byte)22, ref fontStyleFromInstance);
			textDecoration = (RPLFormat.TextDecorations)SharedRenderer.GetStylePropertyValueObject(elementProperties, (byte)24, ref fontStyleFromInstance);
			fontFamily = SharedRenderer.GetStylePropertyValueString(elementProperties, (byte)20, ref fontStyleFromInstance);
		}

		internal static RectangleF GetMeasurementRectangle(RPLMeasurement measurement, RectangleF bounds)
		{
			return new RectangleF(measurement.Left + bounds.Left, measurement.Top + bounds.Top, measurement.Width, measurement.Height);
		}

		internal static bool GetImage(RPLReport rplReport, ref byte[] imageData, long imageDataOffset)
		{
			if (imageData != null)
			{
				return true;
			}
			if (imageDataOffset <= 0)
			{
				return false;
			}
			imageData = rplReport.GetImage(imageDataOffset);
			if (imageData == null)
			{
				return false;
			}
			return true;
		}

		internal static Stream GetEmbeddedImageStream(RPLReport rplReport, long imageDataOffset, CreateAndRegisterStream createAndRegisterStream, string imageName)
		{
			Stream stream = null;
			if (imageDataOffset > 0)
			{
				stream = createAndRegisterStream(imageName, string.Empty, null, null, true, StreamOper.CreateOnly);
				rplReport.GetImage(imageDataOffset, stream);
			}
			return stream;
		}

		internal static bool GetImage(RPLReport rplReport, ref byte[] imageData, long imageDataOffset, ref GDIImageProps gdiImageProps)
		{
			if (SharedRenderer.GetImage(rplReport, ref imageData, imageDataOffset))
			{
				if (gdiImageProps == null)
				{
					try
					{
						using (Image image = Image.FromStream(new MemoryStream(imageData)))
						{
							gdiImageProps = new GDIImageProps(image);
						}
					}
					catch
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		internal static Color GetReportColorStyle(RPLElementStyle properties, byte style)
		{
			object obj = properties[style];
			if (obj != null)
			{
				return new RPLReportColor((string)obj).ToColor();
			}
			return Color.Empty;
		}

		internal static float GetReportSizeStyleMM(RPLElementStyle properties, byte style)
		{
			object obj = properties[style];
			if (obj != null)
			{
				return (float)new RPLReportSize((string)obj).ToMillimeters();
			}
			return float.NaN;
		}

		internal static RPLFormat.BorderStyles GetStylePropertyValueBorderStyle(RPLElementStyle properties, byte style, RPLFormat.BorderStyles defaultStyle)
		{
			object obj = properties[style];
			if (obj != null)
			{
				return (RPLFormat.BorderStyles)obj;
			}
			return defaultStyle;
		}

		internal static RPLFormat.BorderStyles GetStylePropertyValueBorderStyle(RPLElementProps properties, byte style, RPLFormat.BorderStyles defaultStyle)
		{
			object stylePropertyValueObject = SharedRenderer.GetStylePropertyValueObject(properties, style);
			if (stylePropertyValueObject != null)
			{
				return (RPLFormat.BorderStyles)stylePropertyValueObject;
			}
			return defaultStyle;
		}

		internal static object GetStylePropertyValueObject(RPLElementProps properties, byte style)
		{
			bool flag = false;
			return SharedRenderer.GetStylePropertyValueObject(properties, style, ref flag);
		}

		internal static object GetStylePropertyValueObject(RPLElementProps properties, byte style, ref bool fromInstance)
		{
			object obj = null;
			if (properties.NonSharedStyle != null)
			{
				obj = properties.NonSharedStyle[style];
				if (obj != null)
				{
					fromInstance = true;
					return obj;
				}
			}
			if (properties.Definition.SharedStyle != null)
			{
				obj = properties.Definition.SharedStyle[style];
			}
			return obj;
		}

		internal static string GetStylePropertyValueString(RPLElementProps properties, byte style)
		{
			bool flag = false;
			return SharedRenderer.GetStylePropertyValueString(properties, style, ref flag);
		}

		internal static string GetStylePropertyValueString(RPLElementProps properties, byte style, ref bool fromInstance)
		{
			object stylePropertyValueObject = SharedRenderer.GetStylePropertyValueObject(properties, style, ref fromInstance);
			if (stylePropertyValueObject == null)
			{
				return null;
			}
			return (string)stylePropertyValueObject;
		}

		internal static RPLFormat.TextAlignments GetTextAlignForGeneral(TypeCode typeCode, RPLFormat.Directions direction)
		{
			switch (typeCode)
			{
			case TypeCode.SByte:
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.UInt16:
			case TypeCode.Int32:
			case TypeCode.UInt32:
			case TypeCode.Int64:
			case TypeCode.UInt64:
			case TypeCode.Single:
			case TypeCode.Double:
			case TypeCode.Decimal:
			case TypeCode.DateTime:
				if (direction == RPLFormat.Directions.LTR)
				{
					return RPLFormat.TextAlignments.Right;
				}
				return RPLFormat.TextAlignments.Left;
			default:
				if (direction == RPLFormat.Directions.LTR)
				{
					return RPLFormat.TextAlignments.Left;
				}
				return RPLFormat.TextAlignments.Right;
			}
		}

		internal static bool IsWeightBold(RPLFormat.FontWeights weight)
		{
			if (weight != RPLFormat.FontWeights.SemiBold && weight != RPLFormat.FontWeights.Bold && weight != RPLFormat.FontWeights.ExtraBold && weight != RPLFormat.FontWeights.Heavy)
			{
				return false;
			}
			return true;
		}

		internal static void ProcessBottomBorder(WriterBase writer, List<Operation> operations, float borderWidthBottom, RPLFormat.BorderStyles borderStyleBottom, Color borderColorBottom, Color borderColorLeft, Color borderColorRight, float borderBottom, float borderBottomEdge, float borderLeftEdge, float borderRightEdge, float borderBottomEdgeUnclipped, float borderLeftEdgeUnclipped, float borderRightEdgeUnclipped, float borderWidthLeft, float borderWidthRight, float borderWidthBottomUnclipped, float borderWidthLeftUnclipped, float borderWidthRightUnclipped)
		{
			switch (borderStyleBottom)
			{
			case RPLFormat.BorderStyles.None:
				return;
			case RPLFormat.BorderStyles.Solid:
			{
				if (!(borderWidthBottom > writer.HalfPixelWidthY * 2.0))
				{
					break;
				}
				if (!(borderWidthLeft > 0.0) || !(borderColorBottom != borderColorLeft))
				{
					if (!(borderWidthRight > 0.0))
					{
						break;
					}
					if (!(borderColorBottom != borderColorRight))
					{
						break;
					}
				}
				PointF[] polygon = new PointF[4]
				{
					new PointF(borderLeftEdge, borderBottomEdge),
					new PointF(borderRightEdge, borderBottomEdge),
					new PointF(borderRightEdge - borderWidthRight, borderBottomEdge - borderWidthBottom),
					new PointF(borderLeftEdge + borderWidthLeft, borderBottomEdge - borderWidthBottom)
				};
				if (operations == null)
				{
					writer.FillPolygon(borderColorBottom, polygon);
				}
				else
				{
					operations.Add(new FillPolygonOp(borderColorBottom, polygon));
				}
				return;
			}
			}
			if (borderStyleBottom == RPLFormat.BorderStyles.Double)
			{
				PointF[] array = new PointF[4];
				float num = (float)(borderWidthBottomUnclipped / 3.0);
				if (borderBottomEdge >= borderBottomEdgeUnclipped - num)
				{
					float num2 = Math.Max((float)(borderWidthLeft - borderWidthLeftUnclipped / 3.0 * 2.0), 0f);
					float num3 = Math.Max((float)(borderWidthRight - borderWidthRightUnclipped / 3.0 * 2.0), 0f);
					array[0] = new PointF(borderLeftEdge, borderBottomEdge);
					array[1] = new PointF(borderRightEdge, borderBottomEdge);
					array[2] = new PointF(borderRightEdge - num3, borderBottomEdgeUnclipped - num);
					array[3] = new PointF(borderLeftEdge + num2, borderBottomEdgeUnclipped - num);
					if (operations == null)
					{
						writer.FillPolygon(borderColorBottom, array);
					}
					else
					{
						operations.Add(new FillPolygonOp(borderColorBottom, array));
					}
				}
				array = new PointF[4];
				float num4 = (float)(borderBottomEdgeUnclipped - borderWidthBottomUnclipped / 3.0 * 2.0);
				float x;
				float x2;
				if (borderWidthLeft > 0.0)
				{
					x = (float)(borderLeftEdgeUnclipped + borderWidthLeftUnclipped / 3.0 * 2.0);
					x2 = borderLeftEdgeUnclipped + borderWidthLeftUnclipped;
				}
				else
				{
					x = (x2 = borderLeftEdge);
				}
				float x3;
				float x4;
				if (borderWidthRight > 0.0)
				{
					x3 = (float)(borderRightEdgeUnclipped - borderWidthRightUnclipped / 3.0 * 2.0);
					x4 = borderRightEdgeUnclipped - borderWidthRightUnclipped;
				}
				else
				{
					x3 = (x4 = borderRightEdge);
				}
				array[0] = new PointF(x, num4);
				array[1] = new PointF(x3, num4);
				array[2] = new PointF(x4, num4 - num);
				array[3] = new PointF(x2, num4 - num);
				if (operations == null)
				{
					writer.FillPolygon(borderColorBottom, array);
				}
				else
				{
					operations.Add(new FillPolygonOp(borderColorBottom, array));
				}
			}
			else if (operations == null)
			{
				writer.DrawLine(borderColorBottom, borderWidthBottom, borderStyleBottom, borderLeftEdge, borderBottom, borderRightEdge, borderBottom);
			}
			else
			{
				operations.Add(new DrawLineOp(borderColorBottom, borderWidthBottom, borderStyleBottom, borderLeftEdge, borderBottom, borderRightEdge, borderBottom));
			}
		}

		internal static void ProcessLeftBorder(WriterBase writer, List<Operation> operations, float borderWidthLeft, RPLFormat.BorderStyles borderStyleLeft, Color borderColorLeft, Color borderColorTop, Color borderColorBottom, float borderLeft, float borderLeftEdge, float borderTopEdge, float borderBottomEdge, float borderLeftEdgeUnclipped, float borderTopEdgeUnclipped, float borderBottomEdgeUnclipped, float borderWidthTop, float borderWidthBottom, float borderWidthLeftUnclipped, float borderWidthTopUnclipped, float borderWidthBottomUnclipped)
		{
			switch (borderStyleLeft)
			{
			case RPLFormat.BorderStyles.None:
				return;
			case RPLFormat.BorderStyles.Solid:
			{
				if (!(borderWidthLeft > writer.HalfPixelWidthX * 2.0))
				{
					break;
				}
				if (!(borderWidthTop > 0.0) || !(borderColorLeft != borderColorTop))
				{
					if (!(borderWidthBottom > 0.0))
					{
						break;
					}
					if (!(borderColorLeft != borderColorBottom))
					{
						break;
					}
				}
				PointF[] polygon = new PointF[4]
				{
					new PointF(borderLeftEdge, borderTopEdge),
					new PointF(borderLeftEdge, borderBottomEdge),
					new PointF(borderLeftEdge + borderWidthLeft, borderBottomEdge - borderWidthBottom),
					new PointF(borderLeftEdge + borderWidthLeft, borderTopEdge + borderWidthTop)
				};
				if (operations == null)
				{
					writer.FillPolygon(borderColorLeft, polygon);
				}
				else
				{
					operations.Add(new FillPolygonOp(borderColorLeft, polygon));
				}
				return;
			}
			}
			if (borderStyleLeft == RPLFormat.BorderStyles.Double)
			{
				PointF[] array = new PointF[4];
				float num = (float)(borderWidthLeftUnclipped / 3.0);
				if (borderLeftEdge <= borderLeftEdgeUnclipped + num)
				{
					float num2 = Math.Max((float)(borderWidthTop - borderWidthTopUnclipped / 3.0 * 2.0), 0f);
					float num3 = Math.Max((float)(borderWidthBottom - borderWidthBottomUnclipped / 3.0 * 2.0), 0f);
					array[0] = new PointF(borderLeftEdge, borderTopEdge);
					array[1] = new PointF(borderLeftEdge, borderBottomEdge);
					array[2] = new PointF(borderLeftEdgeUnclipped + num, borderBottomEdge - num3);
					array[3] = new PointF(borderLeftEdgeUnclipped + num, borderTopEdge + num2);
					if (operations == null)
					{
						writer.FillPolygon(borderColorLeft, array);
					}
					else
					{
						operations.Add(new FillPolygonOp(borderColorLeft, array));
					}
				}
				array = new PointF[4];
				float num4 = (float)(borderLeftEdgeUnclipped + borderWidthLeftUnclipped / 3.0 * 2.0);
				float y;
				float y2;
				if (borderWidthTop > 0.0)
				{
					y = (float)(borderTopEdgeUnclipped + borderWidthTopUnclipped / 3.0 * 2.0);
					y2 = borderTopEdgeUnclipped + borderWidthTopUnclipped;
				}
				else
				{
					y = (y2 = borderTopEdge);
				}
				float y3;
				float y4;
				if (borderWidthBottom > 0.0)
				{
					y3 = (float)(borderBottomEdgeUnclipped - borderWidthBottomUnclipped / 3.0 * 2.0);
					y4 = borderBottomEdgeUnclipped - borderWidthBottomUnclipped;
				}
				else
				{
					y3 = (y4 = borderBottomEdge);
				}
				array[0] = new PointF(num4, y);
				array[1] = new PointF(num4, y3);
				array[2] = new PointF(num4 + num, y4);
				array[3] = new PointF(num4 + num, y2);
				if (operations == null)
				{
					writer.FillPolygon(borderColorLeft, array);
				}
				else
				{
					operations.Add(new FillPolygonOp(borderColorLeft, array));
				}
			}
			else if (operations == null)
			{
				writer.DrawLine(borderColorLeft, borderWidthLeft, borderStyleLeft, borderLeft, borderTopEdge, borderLeft, borderBottomEdge);
			}
			else
			{
				operations.Add(new DrawLineOp(borderColorLeft, borderWidthLeft, borderStyleLeft, borderLeft, borderTopEdge, borderLeft, borderBottomEdge));
			}
		}

		internal static void ProcessRightBorder(WriterBase writer, List<Operation> operations, float borderWidthRight, RPLFormat.BorderStyles borderStyleRight, Color borderColorRight, Color borderColorTop, Color borderColorBottom, float borderRight, float borderRightEdge, float borderTopEdge, float borderBottomEdge, float borderRightEdgeUnclipped, float borderTopEdgeUnclipped, float borderBottomEdgeUnclipped, float borderWidthTop, float borderWidthBottom, float borderWidthRightUnclipped, float borderWidthTopUnclipped, float borderWidthBottomUnclipped)
		{
			switch (borderStyleRight)
			{
			case RPLFormat.BorderStyles.None:
				return;
			case RPLFormat.BorderStyles.Solid:
			{
				if (!(borderWidthRight > writer.HalfPixelWidthX * 2.0))
				{
					break;
				}
				if (!(borderWidthTop > 0.0) || !(borderColorRight != borderColorTop))
				{
					if (!(borderWidthBottom > 0.0))
					{
						break;
					}
					if (!(borderColorRight != borderColorBottom))
					{
						break;
					}
				}
				PointF[] polygon = new PointF[4]
				{
					new PointF(borderRightEdge, borderTopEdge),
					new PointF(borderRightEdge, borderBottomEdge),
					new PointF(borderRightEdge - borderWidthRight, borderBottomEdge - borderWidthBottom),
					new PointF(borderRightEdge - borderWidthRight, borderTopEdge + borderWidthTop)
				};
				if (operations == null)
				{
					writer.FillPolygon(borderColorRight, polygon);
				}
				else
				{
					operations.Add(new FillPolygonOp(borderColorRight, polygon));
				}
				return;
			}
			}
			if (borderStyleRight == RPLFormat.BorderStyles.Double)
			{
				PointF[] array = new PointF[4];
				float num = (float)(borderWidthRightUnclipped / 3.0);
				if (borderRightEdge >= borderRightEdgeUnclipped - num)
				{
					float num2 = Math.Max((float)(borderWidthTop - borderWidthTopUnclipped / 3.0 * 2.0), 0f);
					float num3 = Math.Max((float)(borderWidthBottom - borderWidthBottomUnclipped / 3.0 * 2.0), 0f);
					array[0] = new PointF(borderRightEdge, borderTopEdge);
					array[1] = new PointF(borderRightEdge, borderBottomEdge);
					array[2] = new PointF(borderRightEdgeUnclipped - num, borderBottomEdge - num3);
					array[3] = new PointF(borderRightEdgeUnclipped - num, borderTopEdge + num2);
					if (operations == null)
					{
						writer.FillPolygon(borderColorRight, array);
					}
					else
					{
						operations.Add(new FillPolygonOp(borderColorRight, array));
					}
				}
				array = new PointF[4];
				float num4 = (float)(borderRightEdgeUnclipped - borderWidthRightUnclipped / 3.0 * 2.0);
				float y;
				float y2;
				if (borderWidthTop > 0.0)
				{
					y = (float)(borderTopEdgeUnclipped + borderWidthTopUnclipped / 3.0 * 2.0);
					y2 = borderTopEdgeUnclipped + borderWidthTopUnclipped;
				}
				else
				{
					y = (y2 = borderTopEdge);
				}
				float y3;
				float y4;
				if (borderWidthBottom > 0.0)
				{
					y3 = (float)(borderBottomEdgeUnclipped - borderWidthBottomUnclipped / 3.0 * 2.0);
					y4 = borderBottomEdgeUnclipped - borderWidthBottomUnclipped;
				}
				else
				{
					y3 = (y4 = borderBottomEdge);
				}
				array[0] = new PointF(num4, y);
				array[1] = new PointF(num4, y3);
				array[2] = new PointF(num4 - num, y4);
				array[3] = new PointF(num4 - num, y2);
				if (operations == null)
				{
					writer.FillPolygon(borderColorRight, array);
				}
				else
				{
					operations.Add(new FillPolygonOp(borderColorRight, array));
				}
			}
			else if (operations == null)
			{
				writer.DrawLine(borderColorRight, borderWidthRight, borderStyleRight, borderRight, borderTopEdge, borderRight, borderBottomEdge);
			}
			else
			{
				operations.Add(new DrawLineOp(borderColorRight, borderWidthRight, borderStyleRight, borderRight, borderTopEdge, borderRight, borderBottomEdge));
			}
		}

		internal static void ProcessTopBorder(WriterBase writer, List<Operation> operations, float borderWidthTop, RPLFormat.BorderStyles borderStyleTop, Color borderColorTop, Color borderColorLeft, Color borderColorRight, float borderTop, float borderTopEdge, float borderLeftEdge, float borderRightEdge, float borderTopEdgeUnclipped, float borderLeftEdgeUnclipped, float borderRightEdgeUnclipped, float borderWidthLeft, float borderWidthRight, float borderWidthTopUnclipped, float borderWidthLeftUnclipped, float borderWidthRightUnclipped)
		{
			switch (borderStyleTop)
			{
			case RPLFormat.BorderStyles.None:
				return;
			case RPLFormat.BorderStyles.Solid:
			{
				if (!(borderWidthTop > writer.HalfPixelWidthY * 2.0))
				{
					break;
				}
				if (!(borderWidthLeft > 0.0) || !(borderColorTop != borderColorLeft))
				{
					if (!(borderWidthRight > 0.0))
					{
						break;
					}
					if (!(borderColorTop != borderColorRight))
					{
						break;
					}
				}
				PointF[] polygon = new PointF[4]
				{
					new PointF(borderLeftEdge, borderTopEdge),
					new PointF(borderRightEdge, borderTopEdge),
					new PointF(borderRightEdge - borderWidthRight, borderTopEdge + borderWidthTop),
					new PointF(borderLeftEdge + borderWidthLeft, borderTopEdge + borderWidthTop)
				};
				if (operations == null)
				{
					writer.FillPolygon(borderColorTop, polygon);
				}
				else
				{
					operations.Add(new FillPolygonOp(borderColorTop, polygon));
				}
				return;
			}
			}
			if (borderStyleTop == RPLFormat.BorderStyles.Double)
			{
				PointF[] array = new PointF[4];
				float num = (float)(borderWidthTopUnclipped / 3.0);
				if (borderTopEdge <= borderTopEdgeUnclipped + num)
				{
					float num2 = Math.Max((float)(borderWidthLeft - borderWidthLeftUnclipped / 3.0 * 2.0), 0f);
					float num3 = Math.Max((float)(borderWidthRight - borderWidthRightUnclipped / 3.0 * 2.0), 0f);
					array[0] = new PointF(borderLeftEdge, borderTopEdge);
					array[1] = new PointF(borderRightEdge, borderTopEdge);
					array[2] = new PointF(borderRightEdge - num3, borderTopEdgeUnclipped + num);
					array[3] = new PointF(borderLeftEdge + num2, borderTopEdgeUnclipped + num);
					if (operations == null)
					{
						writer.FillPolygon(borderColorTop, array);
					}
					else
					{
						operations.Add(new FillPolygonOp(borderColorTop, array));
					}
				}
				array = new PointF[4];
				float num4 = (float)(borderTopEdgeUnclipped + borderWidthTopUnclipped / 3.0 * 2.0);
				float x;
				float x2;
				if (borderWidthLeft > 0.0)
				{
					x = (float)(borderLeftEdgeUnclipped + borderWidthLeftUnclipped / 3.0 * 2.0);
					x2 = borderLeftEdgeUnclipped + borderWidthLeftUnclipped;
				}
				else
				{
					x = (x2 = borderLeftEdge);
				}
				float x3;
				float x4;
				if (borderWidthRight > 0.0)
				{
					x3 = (float)(borderRightEdgeUnclipped - borderWidthRightUnclipped / 3.0 * 2.0);
					x4 = borderRightEdgeUnclipped - borderWidthRightUnclipped;
				}
				else
				{
					x3 = (x4 = borderRightEdge);
				}
				array[0] = new PointF(x, num4);
				array[1] = new PointF(x3, num4);
				array[2] = new PointF(x4, num4 + num);
				array[3] = new PointF(x2, num4 + num);
				if (operations == null)
				{
					writer.FillPolygon(borderColorTop, array);
				}
				else
				{
					operations.Add(new FillPolygonOp(borderColorTop, array));
				}
			}
			else if (operations == null)
			{
				writer.DrawLine(borderColorTop, borderWidthTop, borderStyleTop, borderLeftEdge, borderTop, borderRightEdge, borderTop);
			}
			else
			{
				operations.Add(new DrawLineOp(borderColorTop, borderWidthTop, borderStyleTop, borderLeftEdge, borderTop, borderRightEdge, borderTop));
			}
		}

		internal static bool CalculateImageClippedUnscaledBounds(WriterBase writer, RectangleF bounds, int width, int height, float xOffsetMM, float yOffsetMM, out RectangleF destination, out RectangleF source)
		{
			return SharedRenderer.CalculateImageClippedUnscaledBounds(writer, bounds, width, height, xOffsetMM, yOffsetMM, (int?)null, (int?)null, out destination, out source);
		}

		internal static bool CalculateImageClippedUnscaledBounds(WriterBase writer, RectangleF bounds, int width, int height, float xOffsetMM, float yOffsetMM, int? measureImageDpiX, int? measureImageDpiY, out RectangleF destination, out RectangleF source)
		{
			destination = Rectangle.Empty;
			source = Rectangle.Empty;
			if (!(bounds.Left + xOffsetMM > bounds.Right) && !(bounds.Top + yOffsetMM > bounds.Bottom))
			{
				RectangleF rectangleF = default(RectangleF);
				float num = SharedRenderer.ConvertToMillimeters(width, (float?)measureImageDpiX, writer);
				if (xOffsetMM >= 0.0)
				{
					rectangleF.X = bounds.Left + xOffsetMM;
					rectangleF.Width = Math.Min(num, bounds.Width - xOffsetMM);
				}
				else
				{
					rectangleF.X = bounds.Left;
					rectangleF.Width = Math.Min(num, num + xOffsetMM);
				}
				float num2 = SharedRenderer.ConvertToMillimeters(height, (float?)measureImageDpiY, writer);
				if (yOffsetMM >= 0.0)
				{
					rectangleF.Y = bounds.Top + yOffsetMM;
					rectangleF.Height = Math.Min(num2, bounds.Height - yOffsetMM);
				}
				else
				{
					rectangleF.Y = bounds.Top;
					rectangleF.Height = Math.Min(num2, num2 + yOffsetMM);
				}
				if (!(rectangleF.Right < 0.0) && !(rectangleF.Bottom < 0.0))
				{
					destination = rectangleF;
					float x = 0f;
					if (xOffsetMM < 0.0)
					{
						x = (float)(-SharedRenderer.ConvertToPixels(xOffsetMM, (float?)measureImageDpiX, writer));
					}
					float y = 0f;
					if (yOffsetMM < 0.0)
					{
						y = (float)(-SharedRenderer.ConvertToPixels(yOffsetMM, (float?)measureImageDpiY, writer));
					}
					float width2 = (float)Math.Min(width, SharedRenderer.ConvertToPixels(rectangleF.Width, (float?)measureImageDpiX, writer));
					float height2 = (float)Math.Min(height, SharedRenderer.ConvertToPixels(rectangleF.Height, (float?)measureImageDpiY, writer));
					source = new RectangleF(x, y, width2, height2);
					return true;
				}
				return false;
			}
			return false;
		}

		internal static void CalculateColumnZIndexes(RPLTablix tablix, RPLTablixRow row, int currentRow, int[] columnZIndexes)
		{
			if (currentRow < tablix.ColumnHeaderRows)
			{
				for (int i = 0; i < row.NumCells; i++)
				{
					RPLTablixMemberCell rPLTablixMemberCell = row[i] as RPLTablixMemberCell;
					if (rPLTablixMemberCell != null)
					{
						columnZIndexes[rPLTablixMemberCell.ColIndex] = Math.Min(columnZIndexes[rPLTablixMemberCell.ColIndex], SharedRenderer.CalculateZIndex(rPLTablixMemberCell));
					}
				}
			}
			else
			{
				for (int j = 0; j < row.NumCells; j++)
				{
					RPLTablixMemberCell rPLTablixMemberCell2 = row[j] as RPLTablixMemberCell;
					if (rPLTablixMemberCell2 != null)
					{
						columnZIndexes[rPLTablixMemberCell2.ColIndex] = SharedRenderer.CalculateZIndex(rPLTablixMemberCell2);
					}
				}
			}
		}

		internal static int CalculateRowZIndex(RPLTablixRow row)
		{
			int num = 2147483647;
			for (int i = 0; i < row.NumCells; i++)
			{
				RPLTablixMemberCell rPLTablixMemberCell = row[i] as RPLTablixMemberCell;
				if (rPLTablixMemberCell != null)
				{
					num = Math.Min(num, SharedRenderer.CalculateZIndex(rPLTablixMemberCell));
				}
			}
			if (row.OmittedHeaders != null && row.OmittedHeaders.Count > 0)
			{
				for (int j = 0; j < row.OmittedHeaders.Count; j++)
				{
					num = Math.Min(num, SharedRenderer.CalculateZIndex(row.OmittedHeaders[j]));
				}
			}
			return num;
		}

		internal static int CalculateZIndex(RPLTablixMemberCell header)
		{
			int num = header.TablixMemberDef.Level * 3;
			if (!header.TablixMemberDef.StaticHeadersTree)
			{
				num--;
			}
			return num;
		}
	}
}
