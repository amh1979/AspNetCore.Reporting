using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class XamlRenderer : IDisposable
	{
		private XmlDocument xaml;

		private bool allowPathGradientTransform = true;

		private Color[] layerHues;

		private XamlLayer[] layers;

		private bool disposed;

		public XmlDocument Xaml
		{
			get
			{
				return this.xaml;
			}
		}

		public bool AllowPathGradientTransform
		{
			get
			{
				return this.allowPathGradientTransform;
			}
			set
			{
				this.allowPathGradientTransform = value;
			}
		}

		private Color[] LayerHues
		{
			get
			{
				return this.layerHues;
			}
		}

		public XamlLayer[] Layers
		{
			get
			{
				return this.layers;
			}
			set
			{
				if (this.layers != value && this.layers != null)
				{
					XamlLayer[] array = this.layers;
					foreach (XamlLayer xamlLayer in array)
					{
						if (xamlLayer != null)
						{
							xamlLayer.Dispose();
						}
					}
				}
				this.layers = value;
			}
		}

		public XamlRenderer(string xamlResource)
		{
			this.xaml = new XmlDocument();
			Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(MapControl).Namespace + ".Xaml." + xamlResource);
			this.xaml.Load(manifestResourceStream);
			manifestResourceStream.Close();
		}

		public void ParseXaml(RectangleF viewportRect, Color[] layerHues)
		{
			this.layerHues = layerHues;
			XmlNode xmlNode = this.Xaml.DocumentElement;
			if (xmlNode.Name == "Viewbox")
			{
				xmlNode = this.FindChildNode(xmlNode, "Canvas");
			}
			RectangleF rootCanvasBoundingRectangle = this.GetRootCanvasBoundingRectangle(xmlNode);
			int num = this.CountChildNodes(xmlNode);
			this.Layers = new XamlLayer[num];
			int num2 = 0;
			foreach (XmlNode item in xmlNode)
			{
				if (item.NodeType != XmlNodeType.Comment)
				{
					this.Layers[num2] = this.ParseCanvas(item, num2, rootCanvasBoundingRectangle, viewportRect);
					num2++;
				}
			}
		}

		private RectangleF GetRootCanvasBoundingRectangle(XmlNode rootCanvas)
		{
			XmlAttribute xmlAttribute = rootCanvas.Attributes["Width"];
			XmlAttribute xmlAttribute2 = rootCanvas.Attributes["Height"];
			if (xmlAttribute != null && xmlAttribute2 != null)
			{
				float width = float.Parse(xmlAttribute.Value, CultureInfo.InvariantCulture);
				float height = float.Parse(xmlAttribute2.Value, CultureInfo.InvariantCulture);
				return new RectangleF(0f, 0f, width, height);
			}
			throw new Exception(SR.ExceptionXamlInvalidCanvasSize);
		}

		private RectangleF GetCanvasBoundingRectangle(XmlNode canvasNode)
		{
			RectangleF rectangleF = RectangleF.Empty;
			foreach (XmlNode childNode in canvasNode.ChildNodes)
			{
				if (childNode.NodeType != XmlNodeType.Comment)
				{
					RectangleF rectangleF2 = default(RectangleF);
					if (childNode.Name == "Canvas")
					{
						rectangleF2 = this.GetCanvasBoundingRectangle(childNode);
					}
					else
					{
						XmlAttribute xmlAttribute = childNode.Attributes["Canvas.Left"];
						XmlAttribute xmlAttribute2 = childNode.Attributes["Canvas.Top"];
						XmlAttribute xmlAttribute3 = childNode.Attributes["Width"];
						XmlAttribute xmlAttribute4 = childNode.Attributes["Height"];
						if (childNode.Name == "Path" && xmlAttribute == null && xmlAttribute2 == null)
						{
							string[] streamGeometryParts = this.GetStreamGeometryParts(childNode.Attributes["Data"].Value);
							rectangleF2 = this.GetStreamGeometryBounds(streamGeometryParts, false);
						}
						else
						{
							rectangleF2.X = float.Parse(xmlAttribute.Value, CultureInfo.InvariantCulture);
							rectangleF2.Y = float.Parse(xmlAttribute2.Value, CultureInfo.InvariantCulture);
							rectangleF2.Width = float.Parse(xmlAttribute3.Value, CultureInfo.InvariantCulture);
							rectangleF2.Height = float.Parse(xmlAttribute4.Value, CultureInfo.InvariantCulture);
						}
					}
					rectangleF = ((!(rectangleF == RectangleF.Empty)) ? RectangleF.Union(rectangleF, rectangleF2) : rectangleF2);
				}
			}
			return rectangleF;
		}

		private XamlLayer ParseCanvas(XmlNode canvasNode, int layerIndex, RectangleF fromBounds, RectangleF toBounds)
		{
			XamlLayer xamlLayer = new XamlLayer(toBounds.Location);
			toBounds.Location = PointF.Empty;
			XmlNode xmlNode = this.FindChildNode(canvasNode, "Canvas");
			if (xmlNode != null)
			{
				RectangleF[] slicedBounds = this.GetSlicedBounds(canvasNode);
				RectangleF[] array = this.TransformSlicedBounds(slicedBounds, fromBounds, toBounds);
				int num = this.CountChildNodes(canvasNode);
				xamlLayer.InnerLayers = new XamlLayer[num];
				int num2 = 0;
				{
					foreach (XmlNode item in canvasNode)
					{
						if (item.NodeType != XmlNodeType.Comment)
						{
							xamlLayer.InnerLayers[num2] = this.ParseCanvas(item, layerIndex, slicedBounds[num2], array[num2]);
							num2++;
						}
					}
					return xamlLayer;
				}
			}
			int num3 = 0;
			int num4 = this.CountChildNodes(canvasNode);
			xamlLayer.Paths = new GraphicsPath[num4];
			xamlLayer.Brushes = new Brush[num4];
			xamlLayer.Pens = new Pen[num4];
			foreach (XmlNode childNode in canvasNode.ChildNodes)
			{
				RectangleF rectangleF;
				RectangleF originalShapeRect;
				GraphicsPath graphicsPath;
				if (childNode.NodeType != XmlNodeType.Comment)
				{
					rectangleF = default(RectangleF);
					XmlAttribute xmlAttribute = childNode.Attributes["Canvas.Left"];
					XmlAttribute xmlAttribute2 = childNode.Attributes["Canvas.Top"];
					XmlAttribute xmlAttribute3 = childNode.Attributes["Width"];
					XmlAttribute xmlAttribute4 = childNode.Attributes["Height"];
					XmlAttribute xmlAttribute5 = childNode.Attributes["Stretch"];
					bool flag = false;
					if (xmlAttribute5 != null)
					{
						flag = (xmlAttribute5.Value == "Fill");
					}
					bool includeOrigin = false;
					if (childNode.Name == "Path" && xmlAttribute == null && xmlAttribute2 == null)
					{
						string[] streamGeometryParts = this.GetStreamGeometryParts(childNode.Attributes["Data"].Value);
						includeOrigin = true;
						rectangleF = this.GetStreamGeometryBounds(streamGeometryParts, includeOrigin);
					}
					else
					{
						if (xmlAttribute != null)
						{
							rectangleF.X = float.Parse(xmlAttribute.Value, CultureInfo.InvariantCulture);
						}
						if (xmlAttribute2 != null)
						{
							rectangleF.Y = float.Parse(xmlAttribute2.Value, CultureInfo.InvariantCulture);
						}
						rectangleF.Width = float.Parse(xmlAttribute3.Value, CultureInfo.InvariantCulture);
						rectangleF.Height = float.Parse(xmlAttribute4.Value, CultureInfo.InvariantCulture);
					}
					PointF location = rectangleF.Location;
					originalShapeRect = rectangleF;
					rectangleF = this.TransformRectangle(rectangleF, fromBounds, toBounds);
					graphicsPath = new GraphicsPath();
					if (childNode.Name == "Ellipse")
					{
						graphicsPath.AddEllipse(rectangleF);
						goto IL_0369;
					}
					if (childNode.Name == "Path")
					{
						XmlAttribute xmlAttribute6 = childNode.Attributes["Data"];
						if (xmlAttribute6 != null)
						{
							string[] streamGeometryParts2 = this.GetStreamGeometryParts(xmlAttribute6.Value);
							float stretchFactorX = 1f;
							float stretchFactorY = 1f;
							if (flag)
							{
								RectangleF streamGeometryBounds = this.GetStreamGeometryBounds(streamGeometryParts2, false);
								stretchFactorX = originalShapeRect.Width / streamGeometryBounds.Width;
								stretchFactorY = originalShapeRect.Height / streamGeometryBounds.Height;
							}
							this.IntepretStreamGeometry(streamGeometryParts2, location, stretchFactorX, stretchFactorY, includeOrigin, fromBounds, toBounds, ref graphicsPath);
						}
						goto IL_0369;
					}
					if (childNode.Name == "Rectangle")
					{
						graphicsPath.AddRectangle(rectangleF);
						goto IL_0369;
					}
					throw new Exception(SR.ExceptionXamlShapeNotSupported(childNode.Name));
				}
				continue;
				IL_0369:
				xamlLayer.Paths[num3] = graphicsPath;
				Brush brush = null;
				XmlAttribute xmlAttribute7 = childNode.Attributes["Fill"];
				XmlNode xmlNode4 = this.FindFillNode(childNode);
				if (xmlAttribute7 != null)
				{
					Color color = ColorTranslator.FromHtml(xmlAttribute7.Value);
					brush = new SolidBrush(this.TransformColor(color, layerIndex));
				}
				else if (xmlNode4 != null)
				{
					brush = this.CreateBrush(this.FindChildNode(xmlNode4, "*"), layerIndex, rectangleF, originalShapeRect, fromBounds, toBounds);
				}
				xamlLayer.Brushes[num3] = brush;
				Pen pen = null;
				XmlAttribute xmlAttribute8 = childNode.Attributes["Stroke"];
				XmlNode xmlNode5 = this.FindStrokeNode(childNode);
				if (xmlAttribute8 != null)
				{
					Color color2 = ColorTranslator.FromHtml(xmlAttribute8.Value);
					pen = new Pen(this.TransformColor(color2, layerIndex));
				}
				else if (xmlNode5 != null)
				{
					Brush brush2 = this.CreateBrush(this.FindChildNode(xmlNode5, "*"), layerIndex, rectangleF, originalShapeRect, fromBounds, toBounds);
					pen = new Pen(brush2);
				}
				XmlAttribute xmlAttribute9 = childNode.Attributes["StrokeThickness"];
				if (xmlAttribute9 != null)
				{
					float num5 = float.Parse(xmlAttribute9.Value, CultureInfo.InvariantCulture);
					num5 = (pen.Width = num5 * (toBounds.Width / fromBounds.Width));
				}
				XmlAttribute xmlAttribute10 = childNode.Attributes["StrokeLineJoin"];
				if (xmlAttribute10 != null)
				{
					pen.LineJoin = (LineJoin)Enum.Parse(typeof(LineJoin), xmlAttribute10.Value);
				}
				xamlLayer.Pens[num3] = pen;
				num3++;
			}
			return xamlLayer;
		}

		private RectangleF[] TransformSlicedBounds(RectangleF[] slicedBounds, RectangleF fromBounds, RectangleF toBounds)
		{
			RectangleF[] array = new RectangleF[slicedBounds.Length];
			float num = toBounds.Width / toBounds.Height;
			float width = slicedBounds[slicedBounds.Length - 5].Width;
			float height = slicedBounds[slicedBounds.Length - 8].Height;
			for (int num2 = slicedBounds.Length - 1; num2 >= 0; num2--)
			{
				int num3 = slicedBounds.Length - num2;
				RectangleF rect = slicedBounds[num2];
				switch (num3)
				{
				case 1:
					if (num > 1.0)
					{
						rect.X /= num;
						rect.Width /= num;
					}
					else
					{
						rect.Y *= num;
						rect.Height *= num;
					}
					break;
				case 2:
					if (num > 1.0)
					{
						rect.X = (float)(100.0 - (100.0 - rect.X) / num);
						rect.Width /= num;
					}
					else
					{
						rect.Y *= num;
						rect.Height *= num;
					}
					break;
				case 3:
					if (num > 1.0)
					{
						rect.X = (float)(100.0 - (100.0 - rect.X) / num);
						rect.Width /= num;
					}
					else
					{
						rect.Y = (float)(100.0 - (100.0 - rect.Y) * num);
						rect.Height *= num;
					}
					break;
				case 4:
					if (num > 1.0)
					{
						rect.X /= num;
						rect.Width /= num;
					}
					else
					{
						rect.Y = (float)(100.0 - (100.0 - rect.Y) * num);
						rect.Height *= num;
					}
					break;
				case 5:
					if (num > 1.0)
					{
						float num8 = (float)((fromBounds.Width - width) / 2.0);
						rect.X = num8 / num;
						rect.Width = (float)(fromBounds.Width - 2.0 * num8 / num);
					}
					else
					{
						rect.Y *= num;
						rect.Height *= num;
					}
					break;
				case 6:
					if (num > 1.0)
					{
						rect.X = (float)(100.0 - (100.0 - rect.X) / num);
						rect.Width /= num;
					}
					else
					{
						float num6 = (float)((fromBounds.Height - height) / 2.0);
						rect.Y = num6 * num;
						rect.Height = (float)(fromBounds.Height - 2.0 * num6 * num);
					}
					break;
				case 7:
					if (num > 1.0)
					{
						float num9 = (float)((fromBounds.Width - width) / 2.0);
						rect.X = num9 / num;
						rect.Width = (float)(fromBounds.Width - 2.0 * num9 / num);
					}
					else
					{
						rect.Y = (float)(100.0 - (100.0 - rect.Y) * num);
						rect.Height *= num;
					}
					break;
				case 8:
					if (num > 1.0)
					{
						rect.X /= num;
						rect.Width /= num;
					}
					else
					{
						float num7 = (float)((fromBounds.Height - height) / 2.0);
						rect.Y = num7 * num;
						rect.Height = (float)(fromBounds.Height - 2.0 * num7 * num);
					}
					break;
				case 9:
					if (num > 1.0)
					{
						float num4 = (float)((fromBounds.Width - width) / 2.0);
						rect.X = num4 / num;
						rect.Width = (float)(fromBounds.Width - 2.0 * num4 / num);
					}
					else
					{
						float num5 = (float)((fromBounds.Height - height) / 2.0);
						rect.Y = num5 * num;
						rect.Height = (float)(fromBounds.Height - 2.0 * num5 * num);
					}
					break;
				}
				array[num2] = this.TransformRectangle(rect, fromBounds, toBounds);
			}
			return array;
		}

		private RectangleF[] GetSlicedBounds(XmlNode canvasNode)
		{
			int num = this.CountChildNodes(canvasNode);
			RectangleF[] array = new RectangleF[num];
			int num2 = 0;
			foreach (XmlNode item in canvasNode)
			{
				if (item.NodeType != XmlNodeType.Comment)
				{
					array[num2] = this.GetCanvasBoundingRectangle(item);
					num2++;
				}
			}
			return array;
		}

		private Brush CreateBrush(XmlNode brushNode, int layerIndex, RectangleF shapeRect, RectangleF originalShapeRect, RectangleF fromBounds, RectangleF toBounds)
		{
			Brush brush = null;
			if (brushNode.Name == "LinearGradientBrush")
			{
				PointF pointF = this.ParsePoint(brushNode.Attributes["StartPoint"].Value);
				PointF pointF2 = this.ParsePoint(brushNode.Attributes["EndPoint"].Value);
				bool flag = false;
				XmlAttribute xmlAttribute = brushNode.Attributes["MappingMode"];
				if (xmlAttribute != null && xmlAttribute.Value == "Absolute")
				{
					flag = true;
				}
				if (flag)
				{
					XmlNode xmlNode = this.FindChildNode(brushNode, "LinearGradientBrush.Transform");
					if (xmlNode != null && xmlNode.HasChildNodes)
					{
						this.ApplyTransform(xmlNode, shapeRect, fromBounds, toBounds, ref pointF, ref pointF2);
					}
					pointF.X += originalShapeRect.X;
					pointF.Y += originalShapeRect.Y;
					pointF = this.AbsoluteToRelative(pointF, originalShapeRect);
					pointF2.X += originalShapeRect.X;
					pointF2.Y += originalShapeRect.Y;
					pointF2 = this.AbsoluteToRelative(pointF2, originalShapeRect);
				}
				pointF = this.RelativeToAbsolute(pointF, shapeRect);
				pointF2 = this.RelativeToAbsolute(pointF2, shapeRect);
				float stretchFactor = this.CalculateStretchFactor(pointF, pointF2, shapeRect);
				this.StretchBrushPoints(stretchFactor, ref pointF, ref pointF2);
				brush = new LinearGradientBrush(pointF, pointF2, Color.Black, Color.Black);
				XmlNode xmlNode2 = this.FindChildNode(brushNode, "LinearGradientBrush.GradientStops");
				if (xmlNode2 == null && this.FindChildNode(brushNode, "GradientStop") != null)
				{
					xmlNode2 = brushNode;
				}
				if (xmlNode2 != null)
				{
					((LinearGradientBrush)brush).InterpolationColors = this.CreateColorBlend(xmlNode2, layerIndex, stretchFactor, false);
				}
				XmlNode xmlNode3 = this.FindChildNode(brushNode, "LinearGradientBrush.RelativeTransform");
				if (xmlNode3 != null && xmlNode3.HasChildNodes)
				{
					this.ApplyRelativeTransform(xmlNode3, originalShapeRect, shapeRect, fromBounds, toBounds, ref brush);
				}
				goto IL_0491;
			}
			if (brushNode.Name == "RadialGradientBrush")
			{
				float num = 0.5f;
				XmlAttribute xmlAttribute2 = brushNode.Attributes["RadiusX"];
				if (xmlAttribute2 != null)
				{
					num = float.Parse(xmlAttribute2.Value, CultureInfo.InvariantCulture);
				}
				float num2 = 0.5f;
				XmlAttribute xmlAttribute3 = brushNode.Attributes["RadiusY"];
				if (xmlAttribute3 != null)
				{
					num2 = float.Parse(xmlAttribute3.Value, CultureInfo.InvariantCulture);
				}
				float num3 = (float)Math.Sqrt(num * 2.0);
				float num4 = (float)Math.Sqrt(num2 * 2.0);
				RectangleF rect = shapeRect;
				rect.Inflate((float)(shapeRect.Size.Width * (num3 - 1.0) / 2.0), (float)(shapeRect.Size.Height * (num4 - 1.0) / 2.0));
				PointF relativePoint = new PointF(0.5f, 0.5f);
				XmlAttribute xmlAttribute4 = brushNode.Attributes["Center"];
				if (xmlAttribute4 != null)
				{
					relativePoint = this.ParsePoint(xmlAttribute4.Value);
				}
				PointF pointF3 = new PointF((float)(shapeRect.X + shapeRect.Width / 2.0), (float)(shapeRect.Y + shapeRect.Height / 2.0));
				PointF pointF4 = this.RelativeToAbsolute(relativePoint, shapeRect);
				rect.Offset(pointF4.X - pointF3.X, pointF4.Y - pointF3.Y);
				float num5 = 8f;
				rect.Inflate((float)(rect.Size.Width * (num5 - 1.0) / 2.0), (float)(rect.Size.Height * (num5 - 1.0) / 2.0));
				GraphicsPath graphicsPath = new GraphicsPath();
				graphicsPath.AddEllipse(rect);
				brush = new PathGradientBrush(graphicsPath);
				PointF relativePoint2 = new PointF(0.5f, 0.5f);
				XmlAttribute xmlAttribute5 = brushNode.Attributes["GradientOrigin"];
				if (xmlAttribute5 != null)
				{
					relativePoint2 = this.ParsePoint(xmlAttribute5.Value);
				}
				((PathGradientBrush)brush).CenterPoint = this.RelativeToAbsolute(relativePoint2, shapeRect);
				XmlNode xmlNode4 = this.FindChildNode(brushNode, "RadialGradientBrush.GradientStops");
				if (xmlNode4 == null && this.FindChildNode(brushNode, "GradientStop") != null)
				{
					xmlNode4 = brushNode;
				}
				if (xmlNode4 != null)
				{
					((PathGradientBrush)brush).InterpolationColors = this.CreateColorBlend(xmlNode4, layerIndex, num5, true);
				}
				XmlNode xmlNode5 = this.FindChildNode(brushNode, "RadialGradientBrush.RelativeTransform");
				if (xmlNode5 != null && xmlNode5.HasChildNodes)
				{
					this.ApplyRelativeTransform(xmlNode5, originalShapeRect, shapeRect, fromBounds, toBounds, ref brush);
				}
				XmlNode xmlNode6 = this.FindChildNode(brushNode, "RadialGradientBrush.Transform");
				if (xmlNode6 != null && xmlNode6.HasChildNodes)
				{
					this.ApplyTransform(xmlNode6, shapeRect, fromBounds, toBounds, ref brush);
				}
				goto IL_0491;
			}
			throw new Exception(SR.ExceptionXamlBrushNotSupported(brushNode.Name));
			IL_0491:
			return brush;
		}

		private void ApplyTransform(XmlNode transformNode, RectangleF shapeRect, RectangleF fromBounds, RectangleF toBounds, ref PointF startPoint, ref PointF endPoint)
		{
			XmlNode xmlNode = this.FindChildNode(transformNode, "TransformGroup");
			if (xmlNode == null)
			{
				xmlNode = transformNode;
			}
			XmlNode xmlNode2 = this.FindChildNode(xmlNode, "MatrixTransform");
			if (xmlNode2 != null)
			{
				XmlAttribute xmlAttribute = xmlNode2.Attributes["Matrix"];
				if (xmlAttribute != null)
				{
					string[] array = xmlAttribute.Value.Split(',');
					float[] array2 = new float[6];
					for (int i = 0; i < array2.Length; i++)
					{
						array2[i] = float.Parse(array[i], CultureInfo.InvariantCulture);
					}
					Matrix matrix = new Matrix(array2[0], array2[1], array2[2], array2[3], array2[4], array2[5]);
					PointF[] array3 = new PointF[2]
					{
						startPoint,
						endPoint
					};
					matrix.TransformPoints(array3);
					startPoint = array3[0];
					endPoint = array3[1];
				}
			}
		}

		private void ApplyTransform(XmlNode transformNode, RectangleF shapeRect, RectangleF fromBounds, RectangleF toBounds, ref Brush brush)
		{
			XmlNode xmlNode = this.FindChildNode(transformNode, "TransformGroup");
			if (xmlNode == null)
			{
				xmlNode = transformNode;
			}
			XmlNode xmlNode2 = this.FindChildNode(xmlNode, "MatrixTransform");
			if (xmlNode2 != null)
			{
				Matrix matrix = null;
				XmlAttribute xmlAttribute = xmlNode2.Attributes["Matrix"];
				if (xmlAttribute != null)
				{
					string[] array = xmlAttribute.Value.Split(',');
					float[] array2 = new float[6];
					for (int i = 0; i < array2.Length; i++)
					{
						array2[i] = float.Parse(array[i], CultureInfo.InvariantCulture);
					}
					PointF pointF = new PointF(array2[4], array2[5]);
					pointF.X /= fromBounds.Width;
					pointF.Y /= fromBounds.Height;
					pointF.X *= toBounds.Width;
					pointF.Y *= toBounds.Height;
					Matrix matrix2 = new Matrix(array2[0], array2[1], array2[2], array2[3], 0f, 0f);
					PointF[] array3 = new PointF[1]
					{
						new PointF(shapeRect.X, shapeRect.Y)
					};
					matrix2.TransformPoints(array3);
					pointF.X += shapeRect.X - array3[0].X;
					pointF.Y += shapeRect.Y - array3[0].Y;
					array2[4] = pointF.X;
					array2[5] = pointF.Y;
					matrix = new Matrix(array2[0], array2[1], array2[2], array2[3], array2[4], array2[5]);
				}
				if (matrix != null && brush is LinearGradientBrush)
				{
					Matrix matrix3 = ((LinearGradientBrush)brush).Transform.Clone();
					matrix3.Multiply(matrix, MatrixOrder.Append);
					((LinearGradientBrush)brush).Transform = matrix3;
				}
				else if (matrix != null && brush is PathGradientBrush && this.AllowPathGradientTransform)
				{
					Matrix matrix4 = ((PathGradientBrush)brush).Transform.Clone();
					matrix4.Multiply(matrix, MatrixOrder.Append);
					((PathGradientBrush)brush).Transform = matrix4;
				}
			}
		}

		private void ApplyRelativeTransform(XmlNode transformNode, RectangleF originalShapeRect, RectangleF shapeRect, RectangleF fromBounds, RectangleF toBounds, ref Brush brush)
		{
			XmlNode xmlNode = this.FindChildNode(transformNode, "TransformGroup");
			if (xmlNode == null)
			{
				xmlNode = transformNode;
			}
			XmlNode xmlNode2 = this.FindChildNode(xmlNode, "ScaleTransform");
			if (xmlNode2 != null)
			{
				PointF relativePoint = new PointF(0f, 0f);
				XmlAttribute xmlAttribute = xmlNode2.Attributes["CenterX"];
				if (xmlAttribute != null)
				{
					relativePoint.X = float.Parse(xmlAttribute.Value, CultureInfo.InvariantCulture);
				}
				XmlAttribute xmlAttribute2 = xmlNode2.Attributes["CenterY"];
				if (xmlAttribute2 != null)
				{
					relativePoint.Y = float.Parse(xmlAttribute2.Value, CultureInfo.InvariantCulture);
				}
				relativePoint = this.RelativeToAbsolute(relativePoint, shapeRect);
				float scaleX = 1f;
				XmlAttribute xmlAttribute3 = xmlNode2.Attributes["ScaleX"];
				if (xmlAttribute3 != null)
				{
					scaleX = float.Parse(xmlAttribute3.Value, CultureInfo.InvariantCulture);
				}
				float scaleY = 1f;
				XmlAttribute xmlAttribute4 = xmlNode2.Attributes["ScaleY"];
				if (xmlAttribute4 != null)
				{
					scaleY = float.Parse(xmlAttribute4.Value, CultureInfo.InvariantCulture);
				}
				if (brush is LinearGradientBrush)
				{
					Matrix matrix = ((LinearGradientBrush)brush).Transform.Clone();
					matrix.Translate((float)(0.0 - relativePoint.X), (float)(0.0 - relativePoint.Y), MatrixOrder.Append);
					matrix.Scale(scaleX, scaleY, MatrixOrder.Append);
					matrix.Translate(relativePoint.X, relativePoint.Y, MatrixOrder.Append);
					((LinearGradientBrush)brush).Transform = matrix;
				}
				else if (brush is PathGradientBrush && this.AllowPathGradientTransform)
				{
					Matrix matrix2 = ((PathGradientBrush)brush).Transform.Clone();
					matrix2.Translate((float)(0.0 - relativePoint.X), (float)(0.0 - relativePoint.Y), MatrixOrder.Append);
					matrix2.Scale(scaleX, scaleY, MatrixOrder.Append);
					matrix2.Translate(relativePoint.X, relativePoint.Y, MatrixOrder.Append);
					((PathGradientBrush)brush).Transform = matrix2;
				}
			}
			XmlNode xmlNode3 = this.FindChildNode(xmlNode, "SkewTransform");
			if (xmlNode3 != null)
			{
				float num = 0f;
				XmlAttribute xmlAttribute5 = xmlNode3.Attributes["AngleX"];
				if (xmlAttribute5 != null)
				{
					num = float.Parse(xmlAttribute5.Value, CultureInfo.InvariantCulture);
				}
				num = (float)Math.Tan((double)num * 3.1415926535897931 / 180.0) * shapeRect.Width / shapeRect.Height;
				float num2 = 0f;
				XmlAttribute xmlAttribute6 = xmlNode3.Attributes["AngleY"];
				if (xmlAttribute6 != null)
				{
					num2 = float.Parse(xmlAttribute6.Value, CultureInfo.InvariantCulture);
				}
				num2 = (float)Math.Tan((double)num2 * 3.1415926535897931 / 180.0) * shapeRect.Height / shapeRect.Width;
				if (brush is LinearGradientBrush)
				{
					Matrix matrix3 = ((LinearGradientBrush)brush).Transform.Clone();
					matrix3.Translate((float)(0.0 - shapeRect.X), (float)(0.0 - shapeRect.Y), MatrixOrder.Append);
					matrix3.Shear(num, num2, MatrixOrder.Append);
					matrix3.Translate(shapeRect.X, shapeRect.Y, MatrixOrder.Append);
					((LinearGradientBrush)brush).Transform = matrix3;
				}
				else if (brush is PathGradientBrush && this.AllowPathGradientTransform)
				{
					Matrix matrix4 = ((PathGradientBrush)brush).Transform.Clone();
					matrix4.Translate((float)(0.0 - shapeRect.X), (float)(0.0 - shapeRect.Y), MatrixOrder.Append);
					matrix4.Shear(num, num2, MatrixOrder.Append);
					matrix4.Translate(shapeRect.X, shapeRect.Y, MatrixOrder.Append);
					((PathGradientBrush)brush).Transform = matrix4;
				}
			}
			XmlNode xmlNode4 = this.FindChildNode(xmlNode, "RotateTransform");
			if (xmlNode4 != null)
			{
				PointF pointF = new PointF(0.5f, 0.5f);
				XmlAttribute xmlAttribute7 = xmlNode4.Attributes["CenterX"];
				if (xmlAttribute7 != null)
				{
					pointF.X = float.Parse(xmlAttribute7.Value, CultureInfo.InvariantCulture);
				}
				XmlAttribute xmlAttribute8 = xmlNode4.Attributes["CenterY"];
				if (xmlAttribute8 != null)
				{
					pointF.Y = float.Parse(xmlAttribute8.Value, CultureInfo.InvariantCulture);
				}
				pointF = this.RelativeToAbsolute(pointF, shapeRect);
				float angle = 0f;
				XmlAttribute xmlAttribute9 = xmlNode4.Attributes["Angle"];
				if (xmlAttribute9 != null)
				{
					angle = float.Parse(xmlAttribute9.Value, CultureInfo.InvariantCulture);
				}
				if (brush is LinearGradientBrush)
				{
					Matrix matrix5 = ((LinearGradientBrush)brush).Transform.Clone();
					matrix5.RotateAt(angle, pointF, MatrixOrder.Append);
					((LinearGradientBrush)brush).Transform = matrix5;
				}
				else if (brush is PathGradientBrush && this.AllowPathGradientTransform)
				{
					Matrix matrix6 = ((PathGradientBrush)brush).Transform.Clone();
					matrix6.RotateAt(angle, pointF, MatrixOrder.Append);
					((PathGradientBrush)brush).Transform = matrix6;
				}
			}
			XmlNode xmlNode5 = this.FindChildNode(xmlNode, "TranslateTransform");
			if (xmlNode5 != null)
			{
				PointF relativePoint2 = new PointF(0f, 0f);
				XmlAttribute xmlAttribute10 = xmlNode5.Attributes["X"];
				if (xmlAttribute10 != null)
				{
					relativePoint2.X = float.Parse(xmlAttribute10.Value, CultureInfo.InvariantCulture);
				}
				XmlAttribute xmlAttribute11 = xmlNode5.Attributes["Y"];
				if (xmlAttribute11 != null)
				{
					relativePoint2.Y = float.Parse(xmlAttribute11.Value, CultureInfo.InvariantCulture);
				}
				relativePoint2 = this.RelativeToAbsolute(relativePoint2, shapeRect);
				relativePoint2.X -= shapeRect.X;
				relativePoint2.Y -= shapeRect.Y;
				if (brush is LinearGradientBrush)
				{
					Matrix matrix7 = ((LinearGradientBrush)brush).Transform.Clone();
					matrix7.Translate(relativePoint2.X, relativePoint2.Y, MatrixOrder.Append);
					((LinearGradientBrush)brush).Transform = matrix7;
				}
				else if (brush is PathGradientBrush && this.AllowPathGradientTransform)
				{
					Matrix matrix8 = ((PathGradientBrush)brush).Transform.Clone();
					matrix8.Translate(relativePoint2.X, relativePoint2.Y, MatrixOrder.Append);
					((PathGradientBrush)brush).Transform = matrix8;
				}
			}
		}

		private void StretchBrushPoints(float stretchFactor, ref PointF startPoint, ref PointF endPoint)
		{
			PointF pointF = new PointF((float)(startPoint.X + (endPoint.X - startPoint.X) / 2.0), (float)(startPoint.Y + (endPoint.Y - startPoint.Y) / 2.0));
			startPoint.X -= pointF.X;
			startPoint.Y -= pointF.Y;
			endPoint.X -= pointF.X;
			endPoint.Y -= pointF.Y;
			startPoint.X *= stretchFactor;
			startPoint.Y *= stretchFactor;
			endPoint.X *= stretchFactor;
			endPoint.Y *= stretchFactor;
			startPoint.X += pointF.X;
			startPoint.Y += pointF.Y;
			endPoint.X += pointF.X;
			endPoint.Y += pointF.Y;
		}

		private float CalculateStretchFactor(PointF startPoint, PointF endPoint, RectangleF shapeRect)
		{
			float num = (float)Math.Sqrt((double)((endPoint.X - startPoint.X) * (endPoint.X - startPoint.X) + (endPoint.Y - startPoint.Y) * (endPoint.Y - startPoint.Y)));
			float num2 = (float)Math.Sqrt((double)(shapeRect.Width * shapeRect.Width + shapeRect.Height * shapeRect.Height));
			return (float)(num2 / num * 10.0);
		}

		private ColorBlend CreateColorBlend(XmlNode gradientStopsNode, int layerIndex, float stretchFactor, bool radialBrush)
		{
			SortedList sortedList = new SortedList();
			foreach (XmlNode childNode in gradientStopsNode.ChildNodes)
			{
				if (!(childNode.Name != "GradientStop"))
				{
					Color color = ColorTranslator.FromHtml(childNode.Attributes["Color"].Value);
					color = this.TransformColor(color, layerIndex);
					float num = float.Parse(childNode.Attributes["Offset"].Value, CultureInfo.InvariantCulture);
					if (radialBrush)
					{
						num /= stretchFactor;
						num = (float)(1.0 - num);
					}
					else
					{
						num = (float)(num - 0.5);
						num /= stretchFactor;
						num = (float)(num + 0.5);
					}
					sortedList.Add(num, color);
				}
			}
			ColorBlend colorBlend = new ColorBlend(sortedList.Count + 2);
			int num2 = 1;
			IDictionaryEnumerator enumerator2 = sortedList.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator2.Current;
					colorBlend.Positions[num2] = (float)dictionaryEntry.Key;
					colorBlend.Colors[num2] = (Color)dictionaryEntry.Value;
					num2++;
				}
			}
			finally
			{
				IDisposable disposable = enumerator2 as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			colorBlend.Positions[0] = 0f;
			colorBlend.Colors[0] = (Color)sortedList.GetValueList()[0];
			colorBlend.Positions[num2] = 1f;
			colorBlend.Colors[num2] = (Color)sortedList.GetValueList()[sortedList.Count - 1];
			return colorBlend;
		}

		private RectangleF GetStreamGeometryBounds(string[] parts, bool includeOrigin)
		{
			RectangleF rectangleF = RectangleF.Empty;
			using (GraphicsPath graphicsPath = new GraphicsPath())
			{
				PointF pointF = PointF.Empty;
				for (int i = 0; i < parts.Length; i++)
				{
					if (parts[i] == "F0" || parts[i] == "f0")
					{
						graphicsPath.FillMode = FillMode.Alternate;
					}
					else if (parts[i] == "F1" || parts[i] == "f1")
					{
						graphicsPath.FillMode = FillMode.Winding;
					}
					else if (parts[i] == "M")
					{
						graphicsPath.StartFigure();
						pointF = this.ParsePoint(parts[++i]);
					}
					else if (parts[i] == "C")
					{
						ArrayList arrayList = new ArrayList();
						arrayList.Add(pointF);
						while (++i < parts.Length && parts[i].IndexOf(',') != -1)
						{
							PointF pointF2 = this.ParsePoint(parts[i]);
							arrayList.Add(pointF2);
						}
						i--;
						graphicsPath.AddBeziers((PointF[])arrayList.ToArray(typeof(PointF)));
						pointF = (PointF)arrayList[arrayList.Count - 1];
					}
					else if (parts[i] == "L")
					{
						ArrayList arrayList2 = new ArrayList();
						arrayList2.Add(pointF);
						while (++i < parts.Length && parts[i].IndexOf(',') != -1)
						{
							PointF pointF3 = this.ParsePoint(parts[i]);
							arrayList2.Add(pointF3);
						}
						i--;
						graphicsPath.AddLines((PointF[])arrayList2.ToArray(typeof(PointF)));
						pointF = (PointF)arrayList2[arrayList2.Count - 1];
					}
					else if (parts[i] == "Z" || parts[i] == "z")
					{
						graphicsPath.CloseFigure();
					}
					else if (!(parts[i] == ""))
					{
						throw new Exception(SR.ExceptionXamlGeometryNotSupported(parts[i]));
					}
				}
				graphicsPath.Flatten();
				rectangleF = graphicsPath.GetBounds();
			}
			if (includeOrigin)
			{
				rectangleF = RectangleF.Union(rectangleF, RectangleF.Empty);
			}
			return rectangleF;
		}

		private string[] GetStreamGeometryParts(string streamGeometry)
		{
			streamGeometry = streamGeometry.Replace("M", " M ");
			streamGeometry = streamGeometry.Replace("C", " C ");
			streamGeometry = streamGeometry.Replace("L", " L ");
			streamGeometry = streamGeometry.Replace("  ", " ");
			return streamGeometry.Split(' ');
		}

		private void IntepretStreamGeometry(string[] parts, PointF shapeOffset, float stretchFactorX, float stretchFactorY, bool includeOrigin, RectangleF fromBounds, RectangleF toBounds, ref GraphicsPath graphicsPath)
		{
			shapeOffset.X /= stretchFactorX;
			shapeOffset.Y /= stretchFactorY;
			toBounds.Width *= stretchFactorX;
			toBounds.Height *= stretchFactorY;
			PointF location = this.GetStreamGeometryBounds(parts, includeOrigin).Location;
			location.X -= shapeOffset.X;
			location.Y -= shapeOffset.Y;
			PointF pointF = PointF.Empty;
			int num = 0;
			while (true)
			{
				if (num < parts.Length)
				{
					if (parts[num] == "F0" || parts[num] == "f0")
					{
						graphicsPath.FillMode = FillMode.Alternate;
					}
					else if (parts[num] == "F1" || parts[num] == "f1")
					{
						graphicsPath.FillMode = FillMode.Winding;
					}
					else if (parts[num] == "M")
					{
						graphicsPath.StartFigure();
						pointF = this.OffsetPoint(this.ParsePoint(parts[++num]), location);
						pointF = this.TransformPoint(pointF, fromBounds, toBounds);
					}
					else if (parts[num] == "C")
					{
						ArrayList arrayList = new ArrayList();
						arrayList.Add(pointF);
						while (++num < parts.Length && parts[num].IndexOf(',') != -1)
						{
							PointF point = this.OffsetPoint(this.ParsePoint(parts[num]), location);
							point = this.TransformPoint(point, fromBounds, toBounds);
							arrayList.Add(point);
						}
						num--;
						graphicsPath.AddBeziers((PointF[])arrayList.ToArray(typeof(PointF)));
						pointF = (PointF)arrayList[arrayList.Count - 1];
					}
					else if (parts[num] == "L")
					{
						ArrayList arrayList2 = new ArrayList();
						arrayList2.Add(pointF);
						while (++num < parts.Length && parts[num].IndexOf(',') != -1)
						{
							PointF point2 = this.OffsetPoint(this.ParsePoint(parts[num]), location);
							point2 = this.TransformPoint(point2, fromBounds, toBounds);
							arrayList2.Add(point2);
						}
						num--;
						graphicsPath.AddLines((PointF[])arrayList2.ToArray(typeof(PointF)));
						pointF = (PointF)arrayList2[arrayList2.Count - 1];
					}
					else if (parts[num] == "Z" || parts[num] == "z")
					{
						graphicsPath.CloseFigure();
					}
					else if (!(parts[num] == ""))
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			throw new Exception(SR.ExceptionXamlGeometryNotSupported(parts[num]));
		}

		private PointF RelativeToAbsolute(PointF relativePoint, RectangleF relativeToRect)
		{
			PointF result = default(PointF);
			result.X = relativeToRect.X + relativeToRect.Width * relativePoint.X;
			result.Y = relativeToRect.Y + relativeToRect.Height * relativePoint.Y;
			return result;
		}

		private PointF AbsoluteToRelative(PointF alsolutePoint, RectangleF relativeToRect)
		{
			PointF result = default(PointF);
			result.X = (alsolutePoint.X - relativeToRect.X) / relativeToRect.Width;
			result.Y = (alsolutePoint.Y - relativeToRect.Y) / relativeToRect.Height;
			return result;
		}

		private XmlNode FindFillNode(XmlNode shapeNode)
		{
			foreach (XmlNode childNode in shapeNode.ChildNodes)
			{
				if (childNode.Name == "Shape.Fill" || childNode.Name == shapeNode.Name + ".Fill")
				{
					return childNode;
				}
			}
			return null;
		}

		private XmlNode FindStrokeNode(XmlNode shapeNode)
		{
			foreach (XmlNode childNode in shapeNode.ChildNodes)
			{
				if (childNode.Name == "Shape.Stroke" || childNode.Name == shapeNode.Name + ".Stroke")
				{
					return childNode;
				}
			}
			return null;
		}

		private XmlNode FindChildNode(XmlNode parent, string childName)
		{
			foreach (XmlNode childNode in parent.ChildNodes)
			{
				if (childName == "*" && childNode.NodeType != XmlNodeType.Comment)
				{
					return childNode;
				}
				if (childNode.Name == childName)
				{
					return childNode;
				}
			}
			return null;
		}

		private int CountChildNodes(XmlNode parentNode)
		{
			int num = 0;
			foreach (XmlNode childNode in parentNode.ChildNodes)
			{
				if (childNode.NodeType != XmlNodeType.Comment)
				{
					num++;
				}
			}
			return num;
		}

		private PointF OffsetPoint(PointF point, PointF offset)
		{
			point.X -= offset.X;
			point.Y -= offset.Y;
			return point;
		}

		private PointF TransformPoint(PointF point, RectangleF fromBounds, RectangleF toBounds)
		{
			point.X -= fromBounds.X;
			point.Y -= fromBounds.Y;
			point.X /= fromBounds.Width;
			point.Y /= fromBounds.Height;
			point.X *= toBounds.Width;
			point.Y *= toBounds.Height;
			point.X += toBounds.X;
			point.Y += toBounds.Y;
			return point;
		}

		private RectangleF TransformRectangle(RectangleF rect, RectangleF fromBounds, RectangleF toBounds)
		{
			rect.Location = this.TransformPoint(rect.Location, fromBounds, toBounds);
			rect.Width /= fromBounds.Width;
			rect.Height /= fromBounds.Height;
			rect.Width *= toBounds.Width;
			rect.Height *= toBounds.Height;
			return rect;
		}

		private Color TransformColor(Color color, int layerIndex)
		{
			if (this.LayerHues != null && layerIndex < this.LayerHues.Length)
			{
				ColorMatrix colorMatrix = new ColorMatrix();
				colorMatrix.Matrix00 = (float)((float)(int)this.LayerHues[layerIndex].R / 255.0);
				colorMatrix.Matrix11 = (float)((float)(int)this.LayerHues[layerIndex].G / 255.0);
				colorMatrix.Matrix22 = (float)((float)(int)this.LayerHues[layerIndex].B / 255.0);
				colorMatrix.Matrix33 = (float)((float)(int)this.LayerHues[layerIndex].A / 255.0);
				float[] array = new float[5]
				{
					(float)((float)(int)color.R / 255.0),
					(float)((float)(int)color.G / 255.0),
					(float)((float)(int)color.B / 255.0),
					(float)((float)(int)color.A / 255.0),
					1f
				};
				int val = (int)((array[0] * colorMatrix.Matrix00 + array[1] * colorMatrix.Matrix10 + array[2] * colorMatrix.Matrix20 + array[3] * colorMatrix.Matrix30 + array[4] * colorMatrix.Matrix40) * 255.0);
				int val2 = (int)((array[0] * colorMatrix.Matrix01 + array[1] * colorMatrix.Matrix11 + array[2] * colorMatrix.Matrix21 + array[3] * colorMatrix.Matrix31 + array[4] * colorMatrix.Matrix41) * 255.0);
				int val3 = (int)((array[0] * colorMatrix.Matrix02 + array[1] * colorMatrix.Matrix12 + array[2] * colorMatrix.Matrix22 + array[3] * colorMatrix.Matrix32 + array[4] * colorMatrix.Matrix42) * 255.0);
				int val4 = (int)((array[0] * colorMatrix.Matrix03 + array[1] * colorMatrix.Matrix13 + array[2] * colorMatrix.Matrix23 + array[3] * colorMatrix.Matrix33 + array[4] * colorMatrix.Matrix43) * 255.0);
				val = Math.Max(val, 0);
				val = Math.Min(val, 255);
				val2 = Math.Max(val2, 0);
				val2 = Math.Min(val2, 255);
				val3 = Math.Max(val3, 0);
				val3 = Math.Min(val3, 255);
				val4 = Math.Max(val4, 0);
				val4 = Math.Min(val4, 255);
				HLSColor hLSColor = new HLSColor(val, val2, val3);
				float brightness = this.LayerHues[layerIndex].GetBrightness();
				return Color.FromArgb(val4, hLSColor.Lighten(brightness));
			}
			return color;
		}

		private PointF ParsePoint(string point)
		{
			string[] array = point.Split(',');
			return new PointF(float.Parse(array[0], CultureInfo.InvariantCulture), float.Parse(array[1], CultureInfo.InvariantCulture));
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed && disposing)
			{
				this.Layers = null;
			}
			this.disposed = true;
		}

		~XamlRenderer()
		{
			this.Dispose(false);
		}
	}
}
