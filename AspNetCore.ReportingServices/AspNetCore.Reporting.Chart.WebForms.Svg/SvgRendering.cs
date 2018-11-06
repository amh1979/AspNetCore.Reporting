using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace AspNetCore.Reporting.Chart.WebForms.Svg
{
	internal class SvgRendering : ChartParameters
	{
		internal XmlTextWriter output;

		private int gradientIDNum;

		private int clipRegionIdNum;

		private bool selectionMode;

		private string gradientIDString = string.Empty;

		private float[] oldMatrix = new float[6];

		private bool antiAlias;

		internal bool antiAliasText;

		private bool transformOpen;

		private string toolTipsText = string.Empty;

		private string title = string.Empty;

		private bool toolTipsActive;

		private bool clipSet;

		private bool resizable;

		private Bitmap stringBitmap;

		private Graphics stringGraph;

		private string toolTipsScript = "\r\n\r\n\t\tfunction ToolTips( document )\r\n\t\t{\r\n\t\t\t// Tool Tip Window\r\n\t\t\twindow.svgToolTip = this;\r\n\r\n\t\t\t// Tool Tip Attributes\r\n\t\t\tToolTips.size = 10;\r\n\t\t\tToolTips.scale = document.getDocumentElement().getCurrentScale();\r\n\t\t\tToolTips.translate = document.getDocumentElement().getCurrentTranslate();\r\n\r\n\t\t\t// Init\r\n\t\t\tthis.Create( document );\r\n\t\t\tAddTitleEvents( document.getDocumentElement() );\r\n\t\t}\r\n\r\n\t\tfunction ToolTips.CreateToolTipRectangle( doc )\r\n\t\t{\r\n\t\t\tvar rectangle;\r\n\r\n\t\t\txCoordinate = -1.0 /4.0 * ToolTips.size;\r\n\t\t\tyCoordinate = -1 * ToolTips.size;\r\n\t\t\twidth = 1.0;\r\n\t\t\theight = 1.25 * ToolTips.size;\r\n\r\n\t\t\trectangle = doc.createElement( ##rect## );\r\n\r\n\t\t\trectangle.setAttribute( ##x##, xCoordinate );\r\n\t\t\trectangle.setAttribute( ##y##, yCoordinate );\r\n\t\t\trectangle.setAttribute( ##width##, width );\r\n\t\t\trectangle.setAttribute( ##height##, height );\r\n\r\n\t\t\trectangle.setAttribute( ##style##, ##stroke:black;fill:#edefc2;## );\r\n\r\n\t\t\treturn rectangle;\r\n\t\t}\r\n\r\n\t\tToolTips.prototype.Create = function( doc )\r\n\t\t{ \r\n\t\t\tthis.rectangle = ToolTips.CreateToolTipRectangle( doc );\r\n\r\n\t\t\tthis.node = doc.createTextNode( #### );\r\n\r\n\t\t\tthis.textElement = doc.createElement( ##text## )\r\n\t\t\tthis.textElement.setAttribute( ##style##, ##font-family:Arial; font-size:## + 10 + ##;fill:black;## );\r\n\t\t\tthis.textElement.appendChild( this.node );\r\n\r\n\t\t\tthis.group = doc.createElement( ##g## ),\r\n\t\t\tthis.group.appendChild( this.rectangle );\r\n\t\t\tthis.group.appendChild( this.textElement );\r\n\r\n\t\t\tdoc.getDocumentElement().appendChild( this.group );\r\n\t\t}\r\n\r\n\t\tToolTips.Start = function Title_Activate(evt)\r\n\t\t{\r\n\t\t\tif (window.svgToolTip.element == null)\r\n\t\t\t{\r\n\t\t\t\tvar  x = (evt.getClientX() - ToolTips.translate.getX())/ToolTips.scale +  0.25*ToolTips.size,\r\n\t\t\t\ty = (evt.getClientY() - ToolTips.translate.getY())/ToolTips.scale - ToolTips.size;\r\n\r\n\t\t\t\tdoc = evt.getTarget().getOwnerDocument();\r\n\t\t\t\tSVGRoot = doc.getDocumentElement();\r\n\t\t\t\tsvgW = parseInt(SVGRoot.getAttribute(##width##));\r\n\t\t\t\tsvgH = parseInt(SVGRoot.getAttribute(##height##));\r\n\t\t\t\t\t\t\t\t\r\n\t\t\t\twindow.svgToolTip.element = evt.getCurrentTarget();\r\n\t\t\t\twindow.svgToolTip.element.removeEventListener(##mouseover##, ToolTips.Start, false);\r\n\t\t\t\twindow.svgToolTip.element.addEventListener(##mouseout##, ToolTips.Stop, false);\r\n\t\t\t\twindow.svgToolTip.node.setNodeValue(TextOf(GetToolTip(window.svgToolTip.element)));\r\n\t\t\t\t\r\n\t\t\t\trectWidth = window.svgToolTip.textElement.getComputedTextLength() + 0.5 * ToolTips.size;\r\n\t\t\t\trectHeight = 1.25 * ToolTips.size;\r\n\r\n\t\t\t\tif( svgW < x + rectWidth )\r\n\t\t\t\t{\r\n\t\t\t\t\tx = svgW - rectWidth;\r\n\t\t\t\t}\r\n\r\n\t\t\t\tif( svgH < y + rectHeight )\r\n\t\t\t\t{\r\n\t\t\t\t\ty = svgH - rectHeight;\r\n\t\t\t\t}\r\n\r\n\t\t\t\tif( y < rectHeight )\r\n\t\t\t\t{\r\n\t\t\t\t\ty = 4 * rectHeight;\r\n\t\t\t\t}\r\n\r\n\t\t\t\tif( x < 0 )\r\n\t\t\t\t{\r\n\t\t\t\t\tx = 0;\r\n\t\t\t\t}\r\n\r\n\t\t\t\twindow.svgToolTip.rectangle.setAttribute(##width##, window.svgToolTip.textElement.getComputedTextLength() + 0.5*ToolTips.size);\r\n\t\t\t\twindow.svgToolTip.group.setAttribute(##transform##, ##translate(## + x + ##,## + y + ##)##);\r\n\t\t\t\twindow.svgToolTip.group.setAttribute(##visibility##, ##visible##);\r\n\t\t\t}\r\n\t\t}\r\n\r\n\t\tToolTips.Stop = function Title_Passivate(evt)\r\n\t\t{\r\n\t\t\tif (window.svgToolTip.element != null)\r\n\t\t\t{\r\n\t\t\t\twindow.svgToolTip.group.setAttribute(##visibility##, ##hidden##);\r\n\t\t\t\twindow.svgToolTip.element.removeEventListener(##mouseout##, ToolTips.Stop, false);\r\n\t\t\t\twindow.svgToolTip.element.addEventListener(##mouseover##, ToolTips.Start, false);\r\n\t\t\t\twindow.svgToolTip.element = null;\r\n\t\t\t}\r\n\t\t}\r\n\r\n\t\tToolTips.Register = function Title_Register(elem)\r\n\t\t{\r\n\t\t\tif (GetToolTip(elem) != null)\r\n\t\t\t\telem.addEventListener(##mouseover##, ToolTips.Start, false);\r\n\t\t}\r\n\t\t\r\n\t\tfunction GetToolTip( svgPrimitives )\r\n\t\t{\r\n\t\t\tvar element = svgPrimitives.getChildNodes();\r\n\r\n\t\t\tfor ( itemIndex = 0; itemIndex < element.getLength(); itemIndex++ )\r\n\t\t\t{\r\n\t\t\t\tif ( element.item( itemIndex ).getNodeType() == 1 && element.item( itemIndex ).getNodeName() == ##title## )\r\n\t\t\t\t{\r\n\t\t\t\t\treturn element.item( itemIndex );\r\n\t\t\t\t}\r\n\t\t\t}\r\n\r\n\t\t\treturn null;\r\n\r\n\t\t}\r\n\r\n\t\tfunction TextOf(elem)\r\n\t\t{\r\n\t\t\tvar childs = elem ? elem.getChildNodes() : null;\r\n\r\n\t\t\tfor (var i=0; childs && i<childs.getLength(); i++)\r\n\t\t\t\tif (childs.item(i).getNodeType() == 3)\r\n\t\t\t\t\treturn childs.item(i).getNodeValue();\r\n   \r\n\t\t\treturn ####;\r\n\t\t}\r\n\r\n\t\tfunction AddTitleEvents(elem)\r\n\t\t{\r\n\t\t\tvar childs = elem.getChildNodes();\r\n\r\n\t\t\tfor (var i=0; i<childs.getLength(); i++)\r\n\t\t\t{\r\n\t\t\t\tif (childs.item(i).getNodeType() == 1)\r\n\t\t\t\t{\r\n\t\t\t\t\tAddTitleEvents(childs.item(i));\r\n\t\t\t\t}\r\n\t\t\t}\r\n\r\n\t\t\tif ( GetToolTip(elem) != null )\r\n\t\t\t{\r\n\t\t\t\telem.addEventListener( ##mouseover##, ToolTips.Start, false );\r\n\t\t\t}\r\n\t\t}\r\n\r\n\t\tfunction LoadHandler( event ) \r\n\t\t{\r\n\t\t\tnew ToolTips( event.getTarget().getOwnerDocument() );\r\n\t\t}\r\n\t\r\n";

		private string emptyLoadHandler = "\r\n\r\n\t\tfunction LoadHandler( event ) \r\n\t\t{\r\n\t\t}\r\n\t\r\n";

		public void Open(XmlTextWriter svgWriter, Size pictureSize)
		{
			this.Open(svgWriter, pictureSize, new SvgOpenParameters(false, false, false));
		}

		public void SetTitle(string title)
		{
			this.title = title;
		}

		public void Open(XmlTextWriter svgWriter, Size pictureSize, SvgOpenParameters extraParameters)
		{
			this.output = svgWriter;
			this.toolTipsActive = extraParameters.ToolTipsEnabled;
			if (svgWriter == null)
			{
				throw new ArgumentException(SR.ExceptionSvgTextWriterInvalid, "svgWriter");
			}
			if (!pictureSize.IsEmpty && pictureSize.Width > 0 && pictureSize.Height > 0)
			{
				base.PictureSize = pictureSize;
				this.output.WriteStartDocument();
				this.output.Formatting = Formatting.Indented;
				this.output.WriteComment(SR.MessageSvgConverterTitle);
				this.output.WriteStartElement("svg", "http://www.w3.org/2000/svg");
				this.output.WriteAttributeString("xmlns:xlink", "http://www.w3.org/1999/xlink");
				if (extraParameters.Resizable)
				{
					if (!extraParameters.PreserveAspectRatio)
					{
						this.output.WriteAttributeString("preserveAspectRatio", "none");
					}
					this.output.WriteAttributeString("viewBox", "0 0 " + base.PictureSize.Width.ToString(CultureInfo.InvariantCulture) + " " + base.PictureSize.Height.ToString(CultureInfo.InvariantCulture));
					this.output.WriteAttributeString("xml:space", "preserve");
					this.resizable = true;
					this.toolTipsActive = false;
				}
				else
				{
					this.output.WriteAttributeString("width", base.PictureSize.Width.ToString(CultureInfo.InvariantCulture));
					this.output.WriteAttributeString("height", base.PictureSize.Height.ToString(CultureInfo.InvariantCulture));
				}
				this.output.WriteAttributeString("onload", "LoadHandler(evt)");
				if (this.title.Length > 0)
				{
					this.output.WriteStartElement("title");
					this.output.WriteString(this.title);
					this.output.WriteEndElement();
				}
				return;
			}
			throw new ArgumentException(SR.ExceptionSvgPictureSizeInvalid, "pictureSize");
		}

		internal void Validate()
		{
			if (this.output == null)
			{
				throw new InvalidOperationException(SR.ExceptionSvgOutputWriterIsNull);
			}
			if (!base.PictureSize.IsEmpty && base.PictureSize.Width > 0 && base.PictureSize.Height > 0)
			{
				return;
			}
			throw new InvalidOperationException(SR.ExceptionSvgPictureSizeInvalid);
		}

		public void Close()
		{
			if (this.toolTipsActive)
			{
				this.toolTipsScript = this.toolTipsScript.Replace("##", "\"");
				this.output.WriteStartElement("script");
				this.output.WriteCData(this.toolTipsScript);
				this.output.WriteEndElement();
			}
			else
			{
				this.emptyLoadHandler = this.emptyLoadHandler.Replace("##", "\"");
				this.output.WriteStartElement("script");
				this.output.WriteCData(this.emptyLoadHandler);
				this.output.WriteEndElement();
			}
			if (this.transformOpen)
			{
				this.output.WriteEndElement();
			}
			this.output.WriteEndDocument();
			this.output.Flush();
			if (this.stringGraph != null)
			{
				this.stringGraph.Dispose();
				this.stringGraph = null;
			}
			if (this.stringBitmap != null)
			{
				this.stringBitmap.Dispose();
				this.stringBitmap = null;
			}
		}

		public void DrawLine(PointF point1, PointF point2)
		{
			this.Validate();
			this.StartGraphicsParameters(false, true);
			this.output.WriteStartElement("line");
			this.output.WriteAttributeString("x1", this.GetX(point1));
			this.output.WriteAttributeString("y1", this.GetY(point1));
			this.output.WriteAttributeString("x2", this.GetX(point2));
			this.output.WriteAttributeString("y2", this.GetY(point2));
			if ((point1.X == point2.X || point1.Y == point2.Y) && this.NoTransformMatrix())
			{
				this.output.WriteAttributeString("shape-rendering", "optimizeSpeed");
			}
			this.SetToolTip();
			this.output.WriteEndElement();
			this.EndGraphicsParameters();
		}

		public void DrawLines(PointF[] points)
		{
			this.Validate();
			this.StartGraphicsParameters(false, true);
			this.output.WriteStartElement("polyline");
			string text = string.Empty;
			foreach (PointF point in points)
			{
				string text2 = text;
				text = text2 + this.GetX(point) + "," + this.GetY(point) + " ";
			}
			this.output.WriteAttributeString("points", text);
			this.SetToolTip();
			this.output.WriteEndElement();
			this.EndGraphicsParameters();
		}

		public void DrawRectangle(RectangleF rect)
		{
			this.StartGraphicsParameters(false, true);
			this.SetRectangle(rect);
			this.EndGraphicsParameters();
		}

		private void SetRectangle(RectangleF rect)
		{
			this.Validate();
			this.output.WriteStartElement("rect");
			this.output.WriteAttributeString("x", this.GetX(rect));
			this.output.WriteAttributeString("y", this.GetY(rect));
			this.output.WriteAttributeString("width", this.GetWidth(rect));
			this.output.WriteAttributeString("height", this.GetHeight(rect));
			this.SetToolTip();
			this.output.WriteEndElement();
		}

		public void DrawPolygon(PointF[] points)
		{
			this.StartGraphicsParameters(false, true);
			this.SetPolygon(points);
			this.EndGraphicsParameters();
		}

		private void SetPolygon(PointF[] points)
		{
			this.Validate();
			this.output.WriteStartElement("polygon");
			string text = string.Empty;
			foreach (PointF point in points)
			{
				string text2 = text;
				text = text2 + this.GetX(point) + "," + this.GetY(point) + " ";
			}
			this.output.WriteAttributeString("points", text);
			this.SetToolTip();
			this.output.WriteEndElement();
		}

		public void DrawArc(RectangleF rect, float startAngle, float sweepAngle)
		{
			this.Validate();
			this.StartGraphicsParameters(false, true);
			this.output.WriteStartElement("path");
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddArc(rect, startAngle, sweepAngle);
			PointF point = graphicsPath.PathPoints[0];
			PointF pointF = graphicsPath.PathPoints[graphicsPath.PathPoints.Length - 1];
			int num = 0;
			if (sweepAngle > 180.0)
			{
				num = 1;
			}
			string empty = string.Empty;
			empty = "M" + this.GetX(point) + "," + this.GetY(point);
			string text = empty;
			empty = text + " a" + this.GetX(rect.Width / 2.0) + "," + this.GetY(rect.Height / 2.0);
			object obj = empty;
			empty = obj + " 0," + num + ",1 ";
			empty = empty + this.GetX((double)(pointF.X - point.X)) + "," + this.GetY((double)(pointF.Y - point.Y));
			this.output.WriteAttributeString("d", empty);
			this.SetToolTip();
			this.output.WriteEndElement();
			this.EndGraphicsParameters();
		}

		public void DrawPie(RectangleF rect, float startAngle, float sweepAngle)
		{
			this.Validate();
			this.StartGraphicsParameters(false, true);
			this.SetPie(rect, startAngle, sweepAngle);
			this.output.WriteEndElement();
		}

		private void SetPie(RectangleF rect, float startAngle, float sweepAngle)
		{
			this.Validate();
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddArc(rect, startAngle, sweepAngle);
			if (!(sweepAngle <= 0.0) && graphicsPath.PointCount != 0)
			{
				this.output.WriteStartElement("path");
				PointF pointF = graphicsPath.PathPoints[0];
				PointF pointF2 = graphicsPath.PathPoints[graphicsPath.PathPoints.Length - 1];
				PointF point = new PointF((float)(rect.X + rect.Width / 2.0), (float)(rect.Y + rect.Height / 2.0));
				PointF point2 = new PointF(pointF.X - point.X, pointF.Y - point.Y);
				PointF point3 = new PointF(point.X - pointF2.X, point.Y - pointF2.Y);
				int num = 0;
				if (sweepAngle > 180.0)
				{
					num = 1;
				}
				string empty = string.Empty;
				empty = "M" + this.GetX(point) + "," + this.GetY(point);
				string text = empty;
				empty = text + " l" + this.GetX(point2) + "," + this.GetY(point2);
				string text2 = empty;
				empty = text2 + " a" + this.GetX(rect.Width / 2.0) + "," + this.GetY(rect.Height / 2.0);
				object obj = empty;
				empty = obj + " 0," + num + ",1 ";
				empty = empty + this.GetX((double)(pointF2.X - pointF.X)) + "," + this.GetY((double)(pointF2.Y - pointF.Y));
				string text3 = empty;
				empty = text3 + " l" + this.GetX(point3) + "," + this.GetY(point3);
				this.output.WriteAttributeString("d", empty);
				this.SetToolTip();
				this.output.WriteEndElement();
			}
		}

		public void DrawEllipse(RectangleF rect)
		{
			this.Validate();
			this.StartGraphicsParameters(false, true);
			this.SetEllipse(rect);
			this.EndGraphicsParameters();
		}

		private void SetEllipse(RectangleF rect)
		{
			this.Validate();
			this.output.WriteStartElement("ellipse");
			this.output.WriteAttributeString("cx", this.GetX(rect.X + rect.Width / 2.0));
			this.output.WriteAttributeString("cy", this.GetY(rect.Y + rect.Height / 2.0));
			this.output.WriteAttributeString("rx", this.GetX(rect.Width / 2.0));
			this.output.WriteAttributeString("ry", this.GetY(rect.Height / 2.0));
			this.SetToolTip();
			this.output.WriteEndElement();
		}

		public void DrawBezier(PointF pt1, PointF pt2, PointF pt3, PointF pt4)
		{
			this.Validate();
			this.StartGraphicsParameters(false, true);
			this.output.WriteStartElement("path");
			string empty = string.Empty;
			empty = "M" + this.GetX(pt1) + "," + this.GetY(pt1);
			string text = empty;
			empty = text + " C" + this.GetX(pt2) + "," + this.GetY(pt2);
			string text2 = empty;
			empty = text2 + " " + this.GetX(pt3) + "," + this.GetY(pt3);
			string text3 = empty;
			empty = text3 + " " + this.GetX(pt4) + "," + this.GetY(pt4);
			this.output.WriteAttributeString("d", empty);
			this.output.WriteEndElement();
			this.SetToolTip();
			this.EndGraphicsParameters();
		}

		public void DrawBeziers(PointF[] points)
		{
			int num = points.Length / 4 + 1;
			PointF[] array = new PointF[num * 4];
			for (int i = 0; i < array.Length; i++)
			{
				if (points.Length >= i)
				{
					array[i] = new PointF(0f, 0f);
				}
				else
				{
					array[i] = points[i];
				}
			}
			for (int j = 0; j < num; j++)
			{
				this.DrawBezier(array[j * 4], array[j * 4 + 1], array[j * 4 + 2], array[j * 4 + 3]);
			}
		}

		public void DrawCurve(PointF[] points, float tension)
		{
			this.Validate();
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddCurve(points, tension);
			graphicsPath.Flatten();
			this.DrawLines(graphicsPath.PathPoints);
			graphicsPath.Dispose();
		}

		public void DrawCurve(PointF[] points, int offset, int numberOfSegments, float tension)
		{
			this.Validate();
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddCurve(points, offset, numberOfSegments, tension);
			graphicsPath.Flatten();
			this.DrawLines(graphicsPath.PathPoints);
			graphicsPath.Dispose();
		}

		public void DrawPath(GraphicsPath path)
		{
			byte[] pathTypes = path.PathTypes;
			int num = 1;
			byte[] array = pathTypes;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == 0)
				{
					num++;
				}
			}
			if (num == 2)
			{
				this.Validate();
				path.Flatten();
				this.DrawPolygon(path.PathPoints);
			}
			else
			{
				this.Validate();
				path.Flatten();
				PointF[] array2 = new PointF[path.PathPoints.Length];
				int num2 = 0;
				int num3 = 0;
				PointF[] pathPoints = path.PathPoints;
				foreach (PointF pointF in pathPoints)
				{
					array2[num2] = pointF;
					if (path.PathTypes[num3] == 129)
					{
						PointF[] array3 = new PointF[num2 + 2];
						for (int k = 0; k <= num2; k++)
						{
							array3[k] = array2[k];
						}
						array3[num2 + 1] = array2[0];
						num2 = 0;
						this.DrawLines(array3);
						array2 = new PointF[path.PathPoints.Length];
					}
					else
					{
						num2++;
					}
					num3++;
				}
			}
		}

		public void DrawClosedCurve(PointF[] points, float tension)
		{
			this.Validate();
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddClosedCurve(points, tension);
			graphicsPath.Flatten();
			this.DrawLines(graphicsPath.PathPoints);
			graphicsPath.Dispose();
		}

		public void DrawImage(Image image, Rectangle destinationRect, int sourceX, int sourceY, int sourceWidth, int sourceHeight, GraphicsUnit sourceUnit, ImageAttributes imageAttr)
		{
			this.Validate();
			Image image2 = new Bitmap(image, destinationRect.Width, destinationRect.Height);
			Graphics graphics = Graphics.FromImage(image2);
			graphics.DrawImage(image, new Rectangle(0, 0, destinationRect.Width, destinationRect.Height), sourceX, sourceY, sourceWidth, sourceHeight, sourceUnit, imageAttr);
			this.output.WriteStartElement("image");
			string str = this.ImageToString(image2);
			this.output.WriteAttributeString("xlink:href", "data:image/jpeg;base64," + str);
			this.output.WriteAttributeString("x", this.GetX(destinationRect));
			this.output.WriteAttributeString("y", this.GetY(destinationRect));
			this.output.WriteAttributeString("width", this.GetWidth(destinationRect));
			this.output.WriteAttributeString("height", this.GetHeight(destinationRect));
			this.output.WriteEndElement();
			image2.Dispose();
		}

		public void DrawImage(Image image, RectangleF dstRect)
		{
			this.Validate();
			Image image2 = new Bitmap(image, (int)dstRect.Width, (int)dstRect.Height);
			Graphics graphics = Graphics.FromImage(image2);
			graphics.DrawImage(image, new RectangleF(0f, 0f, dstRect.Width, dstRect.Height));
			this.output.WriteStartElement("image");
			string str = this.ImageToString(image2);
			this.output.WriteAttributeString("xlink:href", "data:image/jpeg;base64," + str);
			this.output.WriteAttributeString("x", this.GetX(dstRect));
			this.output.WriteAttributeString("y", this.GetY(dstRect));
			this.output.WriteAttributeString("width", this.GetWidth(dstRect));
			this.output.WriteAttributeString("height", this.GetHeight(dstRect));
			this.output.WriteEndElement();
			image2.Dispose();
		}

		public void DrawImage(Image image, Rectangle destinationRect, float sourceX, float sourceY, float sourceWidth, float sourceHeight, GraphicsUnit sourceUnit, ImageAttributes imageAttributes)
		{
			this.Validate();
			Image image2 = new Bitmap(image, destinationRect.Width, destinationRect.Height);
			Graphics graphics = Graphics.FromImage(image2);
			graphics.DrawImage(image, new Rectangle(0, 0, destinationRect.Width, destinationRect.Height), sourceX, sourceY, sourceWidth, sourceHeight, sourceUnit, imageAttributes);
			this.output.WriteStartElement("image");
			string str = this.ImageToString(image2);
			this.output.WriteAttributeString("xlink:href", "data:image/jpeg;base64," + str);
			this.output.WriteAttributeString("x", this.GetX(destinationRect));
			this.output.WriteAttributeString("y", this.GetY(destinationRect));
			this.output.WriteAttributeString("width", this.GetWidth(destinationRect));
			this.output.WriteAttributeString("height", this.GetHeight(destinationRect));
			this.output.WriteEndElement();
			image2.Dispose();
		}

		public void FillTexturedRectangle(TextureBrush textureBrush, RectangleF dstRect)
		{
			this.Validate();
			Image image = new Bitmap(textureBrush.Image, (int)dstRect.Width, (int)dstRect.Height);
			Graphics graphics = Graphics.FromImage(image);
			graphics.FillRectangle(textureBrush, dstRect);
			this.output.WriteStartElement("image");
			string str = this.ImageToString(image);
			this.output.WriteAttributeString("xlink:href", "data:image/jpeg;base64," + str);
			this.output.WriteAttributeString("x", this.GetX(dstRect));
			this.output.WriteAttributeString("y", this.GetY(dstRect));
			this.output.WriteAttributeString("width", this.GetWidth(dstRect));
			this.output.WriteAttributeString("height", this.GetHeight(dstRect));
			this.output.WriteEndElement();
			image.Dispose();
		}

		public void DrawString(string text, RectangleF layoutRect)
		{
			if (text.Length != 0)
			{
				this.Transformations();
				string[] array = default(string[]);
				try
				{
					this.WrapString(text, layoutRect, out array);
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionSvgWrapStringIsNull);
				}
				int num = 0;
				int num2 = array.Length;
				int num3 = array.Length - 1;
				while (num3 >= 0 && (array[num3] == null || array[num3].Length == 0))
				{
					num2--;
					num3--;
				}
				string[] array2 = array;
				foreach (string text2 in array2)
				{
					if (text2 == null || text2.Length == 0)
					{
						num++;
					}
					else
					{
						float number;
						float y;
						if (base.StringFormat.Alignment == StringAlignment.Near)
						{
							number = layoutRect.X;
							y = layoutRect.Y;
						}
						else if (base.StringFormat.Alignment == StringAlignment.Far)
						{
							number = layoutRect.X + layoutRect.Width;
							y = layoutRect.Y;
						}
						else
						{
							number = (float)(layoutRect.X + layoutRect.Width / 2.0);
							y = layoutRect.Y;
						}
						int num4 = (int)Math.Round((double)this.Font.SizeInPoints * (Chart.renderingDpiY / 72.0));
						float num5 = (float)num * (float)num4;
						num5 = (float)(num5 + 1.0);
						float num6 = 0f;
						if (base.StringFormat.LineAlignment == StringAlignment.Center)
						{
							num6 = (float)(num6 + (float)num4 * 0.75);
							num6 = (float)(num6 + (layoutRect.Height - (float)(num2 * num4)) / 2.0);
						}
						else
						{
							num6 = ((base.StringFormat.LineAlignment != 0) ? (num6 + layoutRect.Height) : (num6 + (float)num4));
						}
						this.output.WriteStartElement("text");
						this.output.WriteAttributeString("x", base.ToUSString(number));
						this.output.WriteAttributeString("y", base.ToUSString(y + num6 + num5));
						this.SetStringAlignment(base.StringFormat);
						this.output.WriteAttributeString("font-family", this.Font.FontFamily.Name);
						this.output.WriteAttributeString("font-size", base.ToUSString(this.Font.SizeInPoints) + "pt");
						this.output.WriteAttributeString("fill-opacity", this.GetAlpha(this.TextColor));
						if (this.Font.Italic)
						{
							this.output.WriteAttributeString("font-style", "italic");
						}
						if (this.Font.Bold)
						{
							this.output.WriteAttributeString("font-weight", "bold");
						}
						if (this.Font.Underline)
						{
							this.output.WriteAttributeString("text-decoration", "underline");
						}
						else if (this.Font.Strikeout)
						{
							this.output.WriteAttributeString("text-decoration", "line-through");
						}
						this.output.WriteAttributeString("fill", this.ColorToString(this.TextColor));
						this.output.WriteAttributeString("stroke", "none");
						if (!this.antiAliasText)
						{
							this.output.WriteAttributeString("text-rendering", "optimizeSpeed");
						}
						this.output.WriteString(text2);
						this.SetToolTip();
						this.output.WriteEndElement();
						num++;
					}
				}
			}
		}

		public void DrawString(string text, PointF point)
		{
			if (text.Length != 0)
			{
				this.Transformations();
				this.output.WriteStartElement("text");
				this.output.WriteAttributeString("x", base.ToUSString(point.X));
				this.output.WriteAttributeString("y", base.ToUSString(point.Y));
				this.SetStringAlignment(base.StringFormat);
				this.output.WriteAttributeString("font-family", this.Font.FontFamily.Name);
				this.output.WriteAttributeString("font-size", base.ToUSString(this.Font.SizeInPoints) + "pt");
				if (this.Font.Italic)
				{
					this.output.WriteAttributeString("font-style", "italic");
				}
				if (this.Font.Bold)
				{
					this.output.WriteAttributeString("font-weight", "bold");
				}
				if (this.Font.Underline)
				{
					this.output.WriteAttributeString("text-decoration", "underline");
				}
				else if (this.Font.Strikeout)
				{
					this.output.WriteAttributeString("text-decoration", "line-through");
				}
				this.output.WriteAttributeString("fill-opacity", this.GetAlpha(this.TextColor));
				this.output.WriteAttributeString("fill", this.ColorToString(this.TextColor));
				this.output.WriteAttributeString("stroke", "none");
				if (!this.antiAliasText)
				{
					this.output.WriteAttributeString("text-rendering", "optimizeSpeed");
				}
				this.output.WriteString(text);
				this.SetToolTip();
				this.output.WriteEndElement();
			}
		}

		public void FillRectangle(RectangleF rect)
		{
			this.Validate();
			this.StartGraphicsParameters(true, false);
			this.SetRectangle(rect);
			this.EndGraphicsParameters();
		}

		public void FillPolygon(PointF[] points)
		{
			this.Validate();
			this.StartGraphicsParameters(true, false);
			this.SetPolygon(points);
			this.EndGraphicsParameters();
		}

		public void FillPie(RectangleF rect, float startAngle, float sweepAngle)
		{
			this.Validate();
			this.StartGraphicsParameters(true, false);
			this.SetPie(rect, startAngle, sweepAngle);
			this.EndGraphicsParameters();
		}

		public void FillEllipse(RectangleF rect)
		{
			this.Validate();
			this.StartGraphicsParameters(true, false);
			this.SetEllipse(rect);
			this.EndGraphicsParameters();
		}

		public void FillClosedCurve(PointF[] points, float tension)
		{
			this.Validate();
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddClosedCurve(points, tension);
			graphicsPath.Flatten();
			this.FillPolygon(graphicsPath.PathPoints);
			graphicsPath.Dispose();
		}

		public void FillPath(GraphicsPath path)
		{
			this.Validate();
			path.Flatten();
			this.FillPolygon(path.PathPoints);
		}

		public void FillBezier(PointF pt1, PointF pt2, PointF pt3, PointF pt4)
		{
			this.Validate();
			this.StartGraphicsParameters(true, false);
			this.output.WriteStartElement("path");
			string empty = string.Empty;
			empty = "M" + this.GetX(pt1) + "," + this.GetY(pt1);
			string text = empty;
			empty = text + " C" + this.GetX(pt2) + "," + this.GetY(pt2);
			string text2 = empty;
			empty = text2 + " " + this.GetX(pt3) + "," + this.GetY(pt3);
			string text3 = empty;
			empty = text3 + " " + this.GetX(pt4) + "," + this.GetY(pt4);
			if (!pt1.Equals(pt2))
			{
				string text4 = empty;
				empty = text4 + "  L" + this.GetX(pt4) + "," + this.GetY(pt4);
			}
			this.output.WriteAttributeString("d", empty);
			this.SetToolTip();
			this.output.WriteEndElement();
			this.EndGraphicsParameters();
		}

		public void FillCurve(PointF[] points, float tension)
		{
			this.Validate();
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddCurve(points, tension);
			graphicsPath.Flatten();
			this.FillPolygon(graphicsPath.PathPoints);
			graphicsPath.Dispose();
		}

		public void FillArc(RectangleF rect, float startAngle, float sweepAngle)
		{
			this.Validate();
			this.StartGraphicsParameters(true, false);
			this.output.WriteStartElement("path");
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddArc(rect, startAngle, sweepAngle);
			PointF point = graphicsPath.PathPoints[0];
			PointF pointF = graphicsPath.PathPoints[graphicsPath.PathPoints.Length - 1];
			int num = 0;
			if (sweepAngle > 180.0)
			{
				num = 1;
			}
			string empty = string.Empty;
			empty = "M" + this.GetX(point) + "," + this.GetY(point);
			string text = empty;
			empty = text + " a" + this.GetX(rect.Width / 2.0) + "," + this.GetY(rect.Height / 2.0);
			object obj = empty;
			empty = obj + " 0," + num + ",1 ";
			empty = empty + this.GetX((double)(pointF.X - point.X)) + "," + this.GetY((double)(pointF.Y - point.Y));
			if (!point.Equals(pointF))
			{
				string text2 = empty;
				empty = text2 + "  L" + this.GetX(point) + "," + this.GetY(point);
			}
			this.output.WriteAttributeString("d", empty);
			this.SetToolTip();
			this.output.WriteEndElement();
			this.EndGraphicsParameters();
		}

		private bool MatrixChanged()
		{
			if (this.Transform == null)
			{
				return false;
			}
			for (int i = 0; i < 6; i++)
			{
				if (this.oldMatrix[i] != this.Transform.Elements[i])
				{
					return true;
				}
			}
			return false;
		}

		internal bool NoTransformMatrix()
		{
			if (this.Transform == null)
			{
				return true;
			}
			for (int i = 0; i < 6; i++)
			{
				if (i == 0 || i == 3)
				{
					if (this.Transform.Elements[i] != 1.0)
					{
						return false;
					}
				}
				else if (this.Transform.Elements[i] != 0.0)
				{
					return false;
				}
			}
			return true;
		}

		private void ResetTransformMatrix()
		{
			if (this.oldMatrix != null)
			{
				for (int i = 0; i < 6; i++)
				{
					if (i == 0 || i == 3)
					{
						this.oldMatrix[i] = 1f;
					}
					else
					{
						this.oldMatrix[i] = 0f;
					}
				}
			}
		}

		public float WrapString(string text, RectangleF dstRectangle, out string[] stringArray)
		{
			if (this.stringGraph == null)
			{
				this.stringBitmap = new Bitmap(base.PictureSize.Width, base.PictureSize.Height);
				this.stringGraph = Graphics.FromImage(this.stringBitmap);
			}
			SizeF sizeF = this.stringGraph.MeasureString(text, this.Font, base.PictureSize.Width, this.StringFormat);
			dstRectangle.Height = (float)base.PictureSize.Height;
			int num = (int)(dstRectangle.Height / sizeF.Height);
			if (num == 0)
			{
				num = 1;
			}
			stringArray = new string[num];
			string[] array = this.SplitText(text);
			int num2 = 0;
			string empty = string.Empty;
			string[] array2 = array;
			foreach (string text2 in array2)
			{
				if (text2.Length != 0)
				{
					if (text2[0] == '\n')
					{
						num2++;
						empty = string.Empty;
					}
					if (num2 >= stringArray.Length)
					{
						break;
					}
					empty = stringArray[num2] + text2 + ' ';
					sizeF = this.stringGraph.MeasureString(empty, this.Font, base.PictureSize.Width * 10, this.StringFormat);
					if (sizeF.Width < dstRectangle.Width)
					{
						string[] array3;
						string[] array4 = array3 = stringArray;
						int num3 = num2;
						IntPtr intPtr = (IntPtr)num3;
						array4[num3] = array3[(long)intPtr] + text2 + ' ';
					}
					else
					{
						num2++;
						if (num2 >= stringArray.Length)
						{
							break;
						}
						empty = string.Empty;
						string[] array5;
						string[] array6 = array5 = stringArray;
						int num4 = num2;
						IntPtr intPtr2 = (IntPtr)num4;
						array6[num4] = array5[(long)intPtr2] + text2 + ' ';
					}
				}
			}
			return sizeF.Height;
		}

		private string[] SplitText(string text)
		{
			text = text.Replace("\n", " \n");
			return text.Split(' ');
		}

		protected string ImageToString(Image image)
		{
			MemoryStream memoryStream = new MemoryStream();
			image.Save(memoryStream, ImageFormat.Jpeg);
			memoryStream.Seek(0L, SeekOrigin.Begin);
			StringBuilder stringBuilder = new StringBuilder();
			XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder, CultureInfo.InvariantCulture));
			byte[] array = memoryStream.ToArray();
			xmlTextWriter.WriteBase64(array, 0, array.Length);
			xmlTextWriter.Close();
			memoryStream.Close();
			return stringBuilder.ToString();
		}

		private string GetDashStyle(SvgDashStyle dashStyle)
		{
			switch (dashStyle)
			{
			case SvgDashStyle.Dash:
				return "8,3";
			case SvgDashStyle.DashDashDot:
				return "8,3,8,3,2,3";
			case SvgDashStyle.DashDot:
				return "8,3,2,3";
			case SvgDashStyle.DashDotDot:
				return "8,3,2,3,2,3";
			case SvgDashStyle.Dot:
				return "2,3";
			case SvgDashStyle.DoubleDash:
				return "16,3";
			case SvgDashStyle.DoubleDashDoubleDashHalfDash:
				return "16,3,16,3,4,3";
			case SvgDashStyle.DoubleDashHalfDash:
				return "16,3,4,3";
			case SvgDashStyle.DoubleDashHalfDashHalfDash:
				return "16,3,4,3,4,3";
			case SvgDashStyle.HalfDash:
				return "4,3";
			case SvgDashStyle.HalfDashDot:
				return "4,3,2,3";
			case SvgDashStyle.HalfDashDotDot:
				return "4,3,2,3,2,3";
			case SvgDashStyle.HalfDashHalfDashDot:
				return "4,3,4,3,2,3";
			case SvgDashStyle.Solid:
				return "";
			default:
				return "4,5";
			}
		}

		internal string GetAlpha(Color color)
		{
			double number = (double)(int)color.A / 255.0;
			return base.ToUSString(number);
		}

		internal string ColorToString(Color color)
		{
			string str = color.R.ToString("x2", CultureInfo.InvariantCulture);
			string str2 = color.G.ToString("x2", CultureInfo.InvariantCulture);
			string str3 = color.B.ToString("x2", CultureInfo.InvariantCulture);
			return "#" + str + str2 + str3;
		}

		internal void SetStringAlignment(StringFormat stringFormat)
		{
			bool flag = false;
			if ((stringFormat.FormatFlags & StringFormatFlags.DirectionVertical) == StringFormatFlags.DirectionVertical)
			{
				flag = true;
			}
			if (flag)
			{
				this.output.WriteAttributeString("writing-mode", "tb");
			}
			if (stringFormat.Alignment == StringAlignment.Center)
			{
				this.output.WriteAttributeString("text-anchor", "middle");
			}
			else if (stringFormat.Alignment == StringAlignment.Near)
			{
				this.output.WriteAttributeString("text-anchor", "start");
			}
			else
			{
				this.output.WriteAttributeString("text-anchor", "end");
			}
		}

		internal void Transformations()
		{
			if (this.MatrixChanged())
			{
				if (this.transformOpen)
				{
					this.output.WriteEndElement();
				}
				string matrix = this.GetMatrix(this.Transform, true);
				this.output.WriteStartElement("g");
				this.transformOpen = true;
				this.output.WriteAttributeString("transform", "matrix(" + matrix + ")");
			}
		}

		public void SetSmoothingMode(bool antiAlias, bool shape)
		{
			if (shape)
			{
				this.antiAlias = antiAlias;
			}
			else
			{
				this.antiAliasText = antiAlias;
			}
		}

		private string GetMatrix(Matrix matrix, bool setOldMatrix)
		{
			string text = string.Empty;
			for (int i = 0; i < 6; i++)
			{
				if (setOldMatrix)
				{
					this.oldMatrix[i] = this.Transform.Elements[i];
				}
				text = text + base.ToUSString(matrix.Elements[i]) + " ";
			}
			return text;
		}

		private void SVGDefine(bool fill, bool outline)
		{
			if (fill && this.FillType == SvgFillType.Gradient)
			{
				this.gradientIDString = this.SetGradient(this.BrushColor, this.BrushSecondColor, this.GradientType);
			}
		}

		public void BeginSvgSelection(string href, string title)
		{
			if (href.Length > 0)
			{
				if (this.transformOpen)
				{
					this.output.WriteEndElement();
					this.ResetTransformMatrix();
					this.transformOpen = false;
				}
				if (!href.StartsWith("http", StringComparison.OrdinalIgnoreCase) && !href.StartsWith("mailto", StringComparison.OrdinalIgnoreCase) && !href.StartsWith("javascript", StringComparison.OrdinalIgnoreCase))
				{
					href = "http://" + href;
				}
				this.output.WriteStartElement("a");
				this.output.WriteAttributeString("xlink:href", href);
				this.output.WriteAttributeString("xlink:show", "new");
				this.output.WriteAttributeString("xlink:title", title);
				this.selectionMode = true;
			}
			if (title.Length > 0 && !this.resizable)
			{
				this.toolTipsText = title;
				this.toolTipsActive = true;
			}
		}

		public void EndSvgSelection()
		{
			if (this.selectionMode)
			{
				this.output.WriteEndElement();
				this.selectionMode = false;
			}
			this.toolTipsText = string.Empty;
		}

		internal void SetToolTip()
		{
			if (this.toolTipsText.Length > 0)
			{
				this.output.WriteStartElement("title");
				this.output.WriteString(this.toolTipsText);
				this.output.WriteEndElement();
			}
		}

		internal void StartGraphicsParameters(bool fill, bool outline)
		{
			this.SVGDefine(fill, outline);
			this.Transformations();
			this.output.WriteStartElement("g");
			if (!this.antiAlias)
			{
				this.output.WriteAttributeString("shape-rendering", "optimizeSpeed");
			}
			if (this.clipSet)
			{
				this.output.WriteAttributeString("clip-path", "url(#ChartClipID" + this.clipRegionIdNum + ")");
			}
			if (fill)
			{
				if (this.FillMode == FillMode.Winding)
				{
					this.output.WriteAttributeString("fill-rule", "evenodd");
				}
				if (this.FillType == SvgFillType.Gradient)
				{
					this.output.WriteAttributeString("fill", "url(#" + this.gradientIDString + ")");
				}
				else if (this.FillType == SvgFillType.Solid)
				{
					this.output.WriteAttributeString("fill-opacity", this.GetAlpha(this.BrushColor));
					this.output.WriteAttributeString("fill", this.ColorToString(this.BrushColor));
				}
			}
			else
			{
				this.output.WriteAttributeString("fill", "none");
			}
			if (outline)
			{
				this.output.WriteAttributeString("stroke-opacity", this.GetAlpha(this.PenColor));
				this.output.WriteAttributeString("stroke", this.ColorToString(this.PenColor));
				this.output.WriteAttributeString("stroke-width", base.ToUSString(this.PenWidth));
				if (this.GetDashStyle(this.DashStyle).Length != 0)
				{
					this.output.WriteAttributeString("stroke-dasharray", this.GetDashStyle(this.DashStyle));
				}
				if (base.SvgLineCap == SvgLineCapStyle.Square)
				{
					this.output.WriteAttributeString("stroke-linecap", "square");
				}
				else if (base.SvgLineCap == SvgLineCapStyle.Round)
				{
					this.output.WriteAttributeString("stroke-linecap", "round");
				}
			}
			else
			{
				this.output.WriteAttributeString("stroke", "none");
			}
		}

		internal void EndGraphicsParameters()
		{
			this.output.WriteEndElement();
		}

		private string SetGradient(Color firstColor, Color secondColor, SvgGradientType type)
		{
			if (this.gradientIDNum == 2147483647)
			{
				throw new InvalidOperationException(SR.ExceptionSvgMaximumGradientsNumberExceeded(2147483647.ToString(CultureInfo.CurrentCulture)));
			}
			switch (type)
			{
			case SvgGradientType.None:
				return "";
			case SvgGradientType.Center:
			{
				this.gradientIDNum++;
				string text2 = "GradientIDNumber" + this.gradientIDNum;
				this.SetRadialGradient(text2, firstColor, secondColor);
				return text2;
			}
			default:
			{
				this.gradientIDNum++;
				string text = "GradientIDNumber" + this.gradientIDNum;
				this.SetLinearGradient(text, firstColor, secondColor, type);
				return text;
			}
			}
		}

		private void SetRadialGradient(string gradientID, Color firstColor, Color secondColor)
		{
			this.output.WriteStartElement("defs");
			this.output.WriteStartElement("radialGradient");
			this.output.WriteAttributeString("id", gradientID);
			this.output.WriteAttributeString("gradientUnits", "objectBoundingBox");
			this.output.WriteStartElement("stop");
			this.output.WriteAttributeString("offset", "0%");
			this.output.WriteAttributeString("stop-color", this.ColorToString(firstColor));
			this.output.WriteAttributeString("stop-opacity", this.GetAlpha(firstColor));
			this.output.WriteEndElement();
			this.output.WriteStartElement("stop");
			this.output.WriteAttributeString("offset", "100%");
			this.output.WriteAttributeString("stop-color", this.ColorToString(secondColor));
			this.output.WriteAttributeString("stop-opacity", this.GetAlpha(secondColor));
			this.output.WriteEndElement();
			this.output.WriteEndElement();
			this.output.WriteEndElement();
		}

		private void SetLinearGradient(string gradientID, Color firstColor, Color secondColor, SvgGradientType type)
		{
			this.output.WriteStartElement("defs");
			this.output.WriteStartElement("linearGradient");
			this.output.WriteAttributeString("id", gradientID);
			this.output.WriteAttributeString("gradientUnits", "objectBoundingBox");
			this.output.WriteAttributeString("spreadMethod", "reflect");
			switch (type)
			{
			case SvgGradientType.LeftRight:
				this.output.WriteAttributeString("x1", "0%");
				this.output.WriteAttributeString("y1", "0%");
				this.output.WriteAttributeString("x2", "100%");
				this.output.WriteAttributeString("y2", "0%");
				break;
			case SvgGradientType.DiagonalLeft:
				this.output.WriteAttributeString("x1", "0%");
				this.output.WriteAttributeString("y1", "0%");
				this.output.WriteAttributeString("x2", "100%");
				this.output.WriteAttributeString("y2", "100%");
				break;
			case SvgGradientType.DiagonalRight:
				this.output.WriteAttributeString("x1", "100%");
				this.output.WriteAttributeString("y1", "0%");
				this.output.WriteAttributeString("x2", "0%");
				this.output.WriteAttributeString("y2", "100%");
				break;
			case SvgGradientType.TopBottom:
				this.output.WriteAttributeString("x1", "0%");
				this.output.WriteAttributeString("y1", "0%");
				this.output.WriteAttributeString("x2", "0%");
				this.output.WriteAttributeString("y2", "100%");
				break;
			case SvgGradientType.HorizontalCenter:
				this.output.WriteAttributeString("x1", "0%");
				this.output.WriteAttributeString("y1", "100%");
				this.output.WriteAttributeString("x2", "0%");
				this.output.WriteAttributeString("y2", "50%");
				break;
			case SvgGradientType.VerticalCenter:
				this.output.WriteAttributeString("x1", "100%");
				this.output.WriteAttributeString("y1", "0%");
				this.output.WriteAttributeString("x2", "50%");
				this.output.WriteAttributeString("y2", "0%");
				break;
			default:
				throw new InvalidOperationException(SR.ExceptionSvgGradientUndefined);
			}
			this.output.WriteStartElement("stop");
			this.output.WriteAttributeString("offset", "0%");
			this.output.WriteAttributeString("stop-color", this.ColorToString(firstColor));
			this.output.WriteAttributeString("stop-opacity", this.GetAlpha(firstColor));
			this.output.WriteEndElement();
			this.output.WriteStartElement("stop");
			this.output.WriteAttributeString("offset", "100%");
			this.output.WriteAttributeString("stop-color", this.ColorToString(secondColor));
			this.output.WriteAttributeString("stop-opacity", this.GetAlpha(secondColor));
			this.output.WriteEndElement();
			this.output.WriteEndElement();
			this.output.WriteEndElement();
		}

		public void SetClip(RectangleF rect)
		{
			this.clipRegionIdNum++;
			this.clipSet = true;
			this.output.WriteStartElement("clipPath");
			this.output.WriteAttributeString("id", "ChartClipID" + this.clipRegionIdNum);
			this.output.WriteAttributeString("clipPathUnits", "userSpaceOnUse");
			this.output.WriteStartElement("rect");
			this.output.WriteAttributeString("x", this.GetX(rect));
			this.output.WriteAttributeString("y", this.GetY(rect));
			this.output.WriteAttributeString("width", this.GetWidth(rect));
			this.output.WriteAttributeString("height", this.GetHeight(rect));
			this.output.WriteEndElement();
			this.output.WriteEndElement();
		}

		public void ResetClip()
		{
			this.clipSet = false;
		}
	}
}
