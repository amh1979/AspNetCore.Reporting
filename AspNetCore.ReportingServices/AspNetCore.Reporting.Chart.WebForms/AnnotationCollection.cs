using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeAnnotations3")]
	internal class AnnotationCollection : CollectionBase
	{
		internal Chart chart;

		private IServiceContainer serviceContainer;

		private PointF movingResizingStartPoint = PointF.Empty;

		internal Annotation lastClickedAnnotation;

		internal AnnotationGroup annotationGroup;

		private Chart Chart
		{
			get
			{
				if (this.chart == null && this.serviceContainer != null)
				{
					this.chart = (Chart)this.serviceContainer.GetService(typeof(Chart));
				}
				return this.chart;
			}
		}

		[SRDescription("DescriptionAttributeAnnotationCollection_Item")]
		public Annotation this[object parameter]
		{
			get
			{
				if (parameter is int)
				{
					return (Annotation)base.List[(int)parameter];
				}
				if (parameter is string)
				{
					foreach (Annotation item in base.List)
					{
						if (item.Name == (string)parameter)
						{
							return item;
						}
					}
					throw new ArgumentException(SR.ExceptionAnnotationNameNotFound((string)parameter));
				}
				throw new ArgumentException(SR.ExceptionInvalidIndexerArgumentType);
			}
			set
			{
				int num = -1;
				if (value.Name.Length != 0)
				{
					num = base.List.IndexOf(value);
				}
				else
				{
					this.AssignUniqueName(value);
				}
				if (parameter is int)
				{
					if (num != -1 && num != (int)parameter)
					{
						throw new ArgumentException(SR.ExceptionAnnotationNameAlreadyExistsInCollection(value.Name));
					}
					base.List[(int)parameter] = value;
					goto IL_00ee;
				}
				if (parameter is string)
				{
					int num2 = 0;
					foreach (Annotation item in base.List)
					{
						if (item.Name == (string)parameter)
						{
							if (num != -1 && num != num2)
							{
								throw new ArgumentException(SR.ExceptionAnnotationNameAlreadyExistsInCollection(value.Name));
							}
							base.List[num2] = value;
							break;
						}
						num2++;
					}
					goto IL_00ee;
				}
				throw new ArgumentException(SR.ExceptionInvalidIndexerArgumentType);
				IL_00ee:
				this.Invalidate();
			}
		}

		public AnnotationCollection()
		{
			this.serviceContainer = null;
		}

		public AnnotationCollection(IServiceContainer serviceContainer)
		{
			this.serviceContainer = serviceContainer;
		}

		public bool Contains(Annotation annotation)
		{
			return base.List.Contains(annotation);
		}

		public int IndexOf(Annotation value)
		{
			return base.List.IndexOf(value);
		}

		public void Remove(string name)
		{
			Annotation annotation = this.FindByName(name);
			if (annotation != null)
			{
				AnnotationGroup annotationGroup = annotation.GetAnnotationGroup();
				if (annotationGroup != null)
				{
					annotationGroup.Annotations.List.Remove(annotation);
				}
				else
				{
					base.List.Remove(annotation);
				}
			}
		}

		public void Remove(Annotation annotation)
		{
			if (annotation != null)
			{
				AnnotationGroup annotationGroup = annotation.GetAnnotationGroup();
				if (annotationGroup != null)
				{
					annotationGroup.Annotations.List.Remove(annotation);
				}
				else
				{
					base.List.Remove(annotation);
				}
			}
		}

		public int Add(Annotation annotation)
		{
			return base.List.Add(annotation);
		}

		public void Insert(int index, Annotation annotation)
		{
			base.List.Insert(index, annotation);
		}

		public int AddLine(string name, double x1, double y1, double x2, double y2)
		{
			LineAnnotation lineAnnotation = new LineAnnotation();
			if (name.Length > 0)
			{
				lineAnnotation.Name = name;
			}
			lineAnnotation.X = x1;
			lineAnnotation.Y = y1;
			lineAnnotation.Width = x2 - x1;
			lineAnnotation.Height = y2 - y1;
			return base.List.Add(lineAnnotation);
		}

		public int AddArrow(string name, ArrowStyle style, int size, double x1, double y1, double x2, double y2)
		{
			ArrowAnnotation arrowAnnotation = new ArrowAnnotation();
			if (name.Length > 0)
			{
				arrowAnnotation.Name = name;
			}
			arrowAnnotation.X = x1;
			arrowAnnotation.Y = y1;
			arrowAnnotation.Width = x2 - x1;
			arrowAnnotation.Height = y2 - y1;
			arrowAnnotation.ArrowStyle = style;
			arrowAnnotation.ArrowSize = size;
			return base.List.Add(arrowAnnotation);
		}

		public int AddVerticalLine(string name, double x, double y1, double y2)
		{
			VerticalLineAnnotation verticalLineAnnotation = new VerticalLineAnnotation();
			if (name.Length > 0)
			{
				verticalLineAnnotation.Name = name;
			}
			verticalLineAnnotation.X = x;
			verticalLineAnnotation.Y = y1;
			verticalLineAnnotation.Width = 0.0;
			verticalLineAnnotation.Height = y2 - y1;
			return base.List.Add(verticalLineAnnotation);
		}

		public int AddHorizontalLine(string name, double y, double x1, double x2)
		{
			HorizontalLineAnnotation horizontalLineAnnotation = new HorizontalLineAnnotation();
			if (name.Length > 0)
			{
				horizontalLineAnnotation.Name = name;
			}
			horizontalLineAnnotation.X = x1;
			horizontalLineAnnotation.Y = y;
			horizontalLineAnnotation.Width = x2 - x1;
			horizontalLineAnnotation.Height = 0.0;
			return base.List.Add(horizontalLineAnnotation);
		}

		public int AddGroup(string name)
		{
			AnnotationGroup annotationGroup = new AnnotationGroup();
			if (name.Length > 0)
			{
				annotationGroup.Name = name;
			}
			return base.List.Add(annotationGroup);
		}

		public int AddText(string name, string text, double x, double y, double width, double height)
		{
			TextAnnotation textAnnotation = new TextAnnotation();
			if (name.Length > 0)
			{
				textAnnotation.Name = name;
			}
			textAnnotation.Text = text;
			textAnnotation.X = x;
			textAnnotation.Y = y;
			textAnnotation.Width = width;
			textAnnotation.Height = height;
			return base.List.Add(textAnnotation);
		}

		public int AddEllipse(string name, double x, double y, double width, double height)
		{
			EllipseAnnotation ellipseAnnotation = new EllipseAnnotation();
			if (name.Length > 0)
			{
				ellipseAnnotation.Name = name;
			}
			ellipseAnnotation.X = x;
			ellipseAnnotation.Y = y;
			ellipseAnnotation.Width = width;
			ellipseAnnotation.Height = height;
			return base.List.Add(ellipseAnnotation);
		}

		public int AddRectangle(string name, double x, double y, double width, double height)
		{
			RectangleAnnotation rectangleAnnotation = new RectangleAnnotation();
			if (name.Length > 0)
			{
				rectangleAnnotation.Name = name;
			}
			rectangleAnnotation.X = x;
			rectangleAnnotation.Y = y;
			rectangleAnnotation.Width = width;
			rectangleAnnotation.Height = height;
			return base.List.Add(rectangleAnnotation);
		}

		public int AddCallout(string name, string text, CalloutStyle style, double x, double y, double width, double height)
		{
			CalloutAnnotation calloutAnnotation = new CalloutAnnotation();
			if (name.Length > 0)
			{
				calloutAnnotation.Name = name;
			}
			calloutAnnotation.X = x;
			calloutAnnotation.Y = y;
			calloutAnnotation.Width = width;
			calloutAnnotation.Height = height;
			calloutAnnotation.Text = text;
			calloutAnnotation.CalloutStyle = style;
			return base.List.Add(calloutAnnotation);
		}

		public int AddBorder3D(string name, double x, double y, double width, double height)
		{
			Border3DAnnotation border3DAnnotation = new Border3DAnnotation();
			if (name.Length > 0)
			{
				border3DAnnotation.Name = name;
			}
			border3DAnnotation.X = x;
			border3DAnnotation.Y = y;
			border3DAnnotation.Width = width;
			border3DAnnotation.Height = height;
			return base.List.Add(border3DAnnotation);
		}

		public int AddImage(string name, string image, double x, double y, double width, double height)
		{
			ImageAnnotation imageAnnotation = new ImageAnnotation();
			if (name.Length > 0)
			{
				imageAnnotation.Name = name;
			}
			imageAnnotation.X = x;
			imageAnnotation.Y = y;
			imageAnnotation.Width = width;
			imageAnnotation.Height = height;
			imageAnnotation.Image = image;
			return base.List.Add(imageAnnotation);
		}

		public int AddPolygon(string name, GraphicsPath path)
		{
			PolygonAnnotation polygonAnnotation = new PolygonAnnotation();
			if (name.Length > 0)
			{
				polygonAnnotation.Name = name;
			}
			if (path != null)
			{
				polygonAnnotation.Path = path;
			}
			return base.List.Add(polygonAnnotation);
		}

		public int AddPolyline(string name, GraphicsPath path)
		{
			PolylineAnnotation polylineAnnotation = new PolylineAnnotation();
			if (name.Length > 0)
			{
				polylineAnnotation.Name = name;
			}
			if (path != null)
			{
				polylineAnnotation.Path = path;
			}
			return base.List.Add(polylineAnnotation);
		}

		protected override void OnInsert(int index, object value)
		{
			if (((Annotation)value).Name.Length == 0)
			{
				this.AssignUniqueName((Annotation)value);
				if (this.Chart.IsDesignMode() && value is TextAnnotation)
				{
					((TextAnnotation)value).Text = ((TextAnnotation)value).Name;
				}
				return;
			}
			if (this.FindByName(((Annotation)value).Name) == null)
			{
				return;
			}
			throw new InvalidOperationException(SR.ExceptionAnnotationNameAlreadyExistsInCollection(((Annotation)value).Name));
		}

		protected override void OnInsertComplete(int index, object value)
		{
			((Annotation)value).Chart = this.Chart;
			if (value is AnnotationGroup)
			{
				((AnnotationGroup)value).annotations.chart = this.Chart;
				if (this.Chart != null)
				{
					((AnnotationGroup)value).annotations.serviceContainer = this.Chart.serviceContainer;
				}
				foreach (Annotation annotation in ((AnnotationGroup)value).annotations)
				{
					annotation.Chart = this.Chart;
				}
			}
			((Annotation)value).annotationGroup = this.annotationGroup;
			((Annotation)value).ResetCurrentRelativePosition();
			this.Invalidate();
		}

		protected override void OnRemoveComplete(int index, object value)
		{
			if (((Annotation)value).annotationGroup == this.annotationGroup)
			{
				((Annotation)value).annotationGroup = null;
			}
			((Annotation)value).ResetCurrentRelativePosition();
			this.Invalidate();
		}

		protected override void OnClearComplete()
		{
			this.Invalidate();
		}

		internal void Paint(ChartGraphics chartGraph, bool drawAnnotationOnly)
		{
			foreach (Annotation item in base.List)
			{
				item.ResetCurrentRelativePosition();
				if (item.IsVisible())
				{
					bool flag = false;
					if (item.IsAnchorVisible())
					{
						if (item.ClipToChartArea.Length > 0 && item.ClipToChartArea != "NotSet" && this.Chart != null)
						{
							int index = this.Chart.ChartAreas.GetIndex(item.ClipToChartArea);
							if (index >= 0)
							{
								ChartArea chartArea = this.Chart.ChartAreas[index];
								chartGraph.SetClip(chartArea.PlotAreaPosition.ToRectangleF());
								flag = true;
							}
						}
						string empty = string.Empty;
						empty = item.Href;
						chartGraph.StartHotRegion(item.ReplaceKeywords(empty), item.ReplaceKeywords(item.ToolTip));
						item.Paint(this.Chart, chartGraph);
						chartGraph.StopAnimation();
						chartGraph.EndHotRegion();
						if (flag)
						{
							chartGraph.ResetClip();
						}
					}
				}
			}
		}

		private void Invalidate()
		{
		}

		internal void AssignUniqueName(Annotation annotation)
		{
			AnnotationCollection annotationCollection = this;
			if (this.Chart != null)
			{
				annotationCollection = this.Chart.Annotations;
			}
			string empty = string.Empty;
			int num = 1;
			do
			{
				empty = annotation.AnnotationType + num.ToString(CultureInfo.InvariantCulture);
				num++;
			}
			while (annotationCollection.FindByName(empty) != null && num < 10000);
			annotation.Name = empty;
		}

		public Annotation FindByName(string name)
		{
			AnnotationGroup annotationGroup = null;
			return this.FindByName(name, ref annotationGroup);
		}

		internal Annotation FindByName(string name, ref AnnotationGroup group)
		{
			Annotation annotation = null;
			group = null;
			int num = 0;
			while (num < base.List.Count)
			{
				if (string.Compare(this[num].Name, name, StringComparison.Ordinal) != 0)
				{
					AnnotationGroup annotationGroup = this[num] as AnnotationGroup;
					if (annotationGroup != null)
					{
						annotation = annotationGroup.Annotations.FindByName(name, ref group);
						if (annotation != null)
						{
							group = annotationGroup;
							break;
						}
					}
					num++;
					continue;
				}
				annotation = this[num];
				break;
			}
			return annotation;
		}
	}
}
