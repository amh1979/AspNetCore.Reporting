using AspNetCore.Reporting.Chart.WebForms.ChartTypes;
using AspNetCore.Reporting.Chart.WebForms.Data;
using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Drawing;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms.Utilities
{
	internal class SelectionManager : IServiceProvider
	{
		internal class ChartAreaRectangle
		{
			internal ChartArea ChartArea;

			internal RectangleF Rectangle = RectangleF.Empty;
		}

		private IServiceContainer service;

		private ArrayList selectableObjectList = new ArrayList();

		private bool enabled;

		internal Point selectionPoint = Point.Empty;

		internal bool invalidated = true;

		private ContextElementTypes selectableTypes = ContextElementTypes.Any;

		internal ObjectInfo selectedObjectInfo = new ObjectInfo();

		private HotRegion hotRegion = new HotRegion();

		private Chart chartControl;

		private ChartPicture chartPicture;

		private DataManager dataManager;

		internal Chart ChartControl
		{
			get
			{
				if (this.chartControl == null && this.Chart != null)
				{
					this.chartControl = this.Chart.common.Chart;
				}
				return this.chartControl;
			}
		}

		internal ChartPicture Chart
		{
			get
			{
				if (this.chartPicture == null)
				{
					this.chartPicture = (this.service.GetService(typeof(ChartImage)) as ChartPicture);
					if (this.chartPicture == null)
					{
						this.chartPicture = (this.service.GetService(typeof(ChartPicture)) as ChartPicture);
					}
				}
				return this.chartPicture;
			}
		}

		internal DataManager DataManager
		{
			get
			{
				if (this.dataManager == null)
				{
					this.dataManager = (this.service.GetService(typeof(DataManager)) as DataManager);
				}
				return this.dataManager;
			}
		}

		internal ChartGraphics Graph
		{
			get
			{
				if (this.Chart != null)
				{
					return this.Chart.common.graph;
				}
				return null;
			}
		}

		internal ContextElementTypes SelectableTypes
		{
			get
			{
				return this.selectableTypes;
			}
			set
			{
				this.selectableTypes = value;
				this.Invalidate();
			}
		}

		public virtual Point SelectionPoint
		{
			get
			{
				return this.selectionPoint;
			}
			set
			{
				this.selectionPoint = value;
				this.Invalidate();
			}
		}

		internal ObjectInfo Result
		{
			get
			{
				this.CheckInvalidated();
				return this.selectedObjectInfo;
			}
			set
			{
				this.invalidated = false;
				this.selectedObjectInfo = value;
			}
		}

		internal HitTestResult HitTestResult
		{
			get
			{
				return this.selectedObjectInfo.InspectedObject as HitTestResult;
			}
			set
			{
				this.Result = ObjectInfo.Get(value, this.chartControl);
			}
		}

		internal HotRegion HotRegion
		{
			get
			{
				return this.hotRegion;
			}
			set
			{
				this.hotRegion = value;
			}
		}

		internal bool Enabled
		{
			get
			{
				return this.enabled;
			}
			set
			{
				this.enabled = value;
			}
		}

		internal SelectionManager(IServiceContainer service, bool assignService)
		{
			this.service = service;
		}

		internal SelectionManager(IServiceContainer service)
		{
			if (service == null)
			{
				throw new ArgumentNullException("service");
			}
			if (service.GetService(typeof(SelectionManager)) != null)
			{
				throw new ArgumentException(SR.ExceptionObjectSelectorAlreadyRegistred);
			}
			this.service = service;
			this.service.AddService(typeof(SelectionManager), this);
		}

		internal bool SelectChartElement(ChartElementType elementType, object chartObject, object chartSubObject)
		{
			if (this.ChartControl != null)
			{
				string seriesName = string.Empty;
				int num = 0;
				if ((elementType == ChartElementType.DataPoint || elementType == ChartElementType.DataPointLabel) && !(chartObject is Series) && !(chartObject is DataPoint))
				{
					return false;
				}
				DataPoint dataPoint = null;
				if (chartObject is Series)
				{
					seriesName = (chartObject as Series).Name;
					if (chartSubObject is DataPoint && (chartSubObject as DataPoint).series != null)
					{
						dataPoint = (chartSubObject as DataPoint);
						seriesName = dataPoint.series.Name;
						num = dataPoint.series.Points.IndexOf(dataPoint);
					}
				}
				else if (chartObject is DataPoint && (chartObject as DataPoint).series != null)
				{
					dataPoint = (chartObject as DataPoint);
					seriesName = dataPoint.series.Name;
					num = dataPoint.series.Points.IndexOf(dataPoint);
				}
				if (num == -1 && dataPoint != null && dataPoint.IsAttributeSet("OriginalPointIndex"))
				{
					int.TryParse(((DataPointAttributes)dataPoint)["OriginalPointIndex"], NumberStyles.Integer, (IFormatProvider)CultureInfo.InvariantCulture, out num);
				}
				if (num == -1)
				{
					return false;
				}
				this.HitTestResult = this.ChartControl.GetHitTestResult(seriesName, num, elementType, chartObject);
				return true;
			}
			return false;
		}

		internal bool IsChartElementSelected(ChartElementType elementType, object chartObject, object chartSubObject)
		{
			if (this.HitTestResult != null && this.HitTestResult.ChartElementType == elementType && this.HitTestResult.Object == chartObject && this.HitTestResult.SubObject == chartSubObject)
			{
				return true;
			}
			return false;
		}

		internal void Invalidate()
		{
			this.invalidated = true;
		}

		internal virtual void CheckInvalidated()
		{
			if (this.invalidated)
			{
				this.HitTest();
			}
		}

		private PointF GetRelativeHitPoint()
		{
			if (this.Chart != null && this.Chart.common != null && this.Chart.common.graph != null)
			{
				return this.Chart.common.graph.GetRelativePoint(this.selectionPoint);
			}
			return PointF.Empty;
		}

		private IList GetHitOrderList()
		{
			ArrayList arrayList = new ArrayList();
			ContextElementTypes contextElementTypes = this.selectableTypes;
			if ((contextElementTypes & ContextElementTypes.Annotation) == ContextElementTypes.Annotation)
			{
				arrayList.Add(ChartElementType.Annotation);
			}
			if ((contextElementTypes & ContextElementTypes.Title) == ContextElementTypes.Title)
			{
				arrayList.Add(ChartElementType.Title);
			}
			if ((contextElementTypes & ContextElementTypes.Legend) == ContextElementTypes.Legend)
			{
				arrayList.Add(ChartElementType.LegendArea);
				arrayList.Add(ChartElementType.LegendItem);
				arrayList.Add(ChartElementType.LegendTitle);
			}
			if ((contextElementTypes & ContextElementTypes.Series) == ContextElementTypes.Series)
			{
				arrayList.Add(ChartElementType.DataPoint);
				arrayList.Add(ChartElementType.DataPointLabel);
			}
			if ((contextElementTypes & ContextElementTypes.Axis) == ContextElementTypes.Axis || (contextElementTypes & ContextElementTypes.AxisLabel) == ContextElementTypes.AxisLabel)
			{
				arrayList.Add(ChartElementType.Axis);
				arrayList.Add(ChartElementType.AxisLabelImage);
				arrayList.Add(ChartElementType.AxisTitle);
				arrayList.Add(ChartElementType.AxisLabels);
				arrayList.Add(ChartElementType.TickMarks);
			}
			if ((contextElementTypes & ContextElementTypes.ChartArea) == ContextElementTypes.ChartArea)
			{
				arrayList.Add(ChartElementType.PlottingArea);
				arrayList.Add(ChartElementType.StripLines);
				arrayList.Add(ChartElementType.Gridlines);
			}
			return arrayList;
		}

		protected bool IsArea3D(ChartArea area)
		{
			if (area.Area3DStyle.Enable3D && !this.IsChartAreaCircular(area) && area.matrix3D != null)
			{
				return area.matrix3D.IsInitialized();
			}
			return false;
		}

		protected internal virtual IList GetAxisMarkers(ChartGraphics graph, Axis axis)
		{
			ArrayList arrayList = new ArrayList();
			if (axis == null)
			{
				return arrayList;
			}
			PointF empty = PointF.Empty;
			PointF empty2 = PointF.Empty;
			switch (axis.AxisPosition)
			{
			case AxisPosition.Left:
				empty.X = (float)axis.GetAxisPosition();
				empty.Y = axis.PlotAreaPosition.Bottom();
				empty2.X = (float)axis.GetAxisPosition();
				empty2.Y = axis.PlotAreaPosition.Y;
				break;
			case AxisPosition.Right:
				empty.X = (float)axis.GetAxisPosition();
				empty.Y = axis.PlotAreaPosition.Bottom();
				empty2.X = (float)axis.GetAxisPosition();
				empty2.Y = axis.PlotAreaPosition.Y;
				break;
			case AxisPosition.Bottom:
				empty.X = axis.PlotAreaPosition.X;
				empty.Y = (float)axis.GetAxisPosition();
				empty2.X = axis.PlotAreaPosition.Right();
				empty2.Y = (float)axis.GetAxisPosition();
				break;
			case AxisPosition.Top:
				empty.X = axis.PlotAreaPosition.X;
				empty.Y = (float)axis.GetAxisPosition();
				empty2.X = axis.PlotAreaPosition.Right();
				empty2.Y = (float)axis.GetAxisPosition();
				break;
			}
			IList markers = this.GetMarkers(RectangleF.FromLTRB(empty.X, empty.Y, empty2.X, empty2.Y));
			if (this.IsArea3D(axis.chartArea))
			{
				float areaSceneDepth = axis.chartArea.areaSceneDepth;
				Point3D[] array = new Point3D[markers.Count];
				for (int i = 0; i < markers.Count; i++)
				{
					array[i] = new Point3D(((PointF)markers[i]).X, ((PointF)markers[i]).Y, areaSceneDepth);
				}
				axis.chartArea.matrix3D.TransformPoints(array);
				for (int j = 0; j < markers.Count; j++)
				{
					markers[j] = array[j].PointF;
				}
			}
			foreach (PointF item in markers)
			{
				arrayList.Add(graph.GetAbsolutePoint(item));
			}
			return arrayList;
		}

		protected internal bool IsChartAreaCircular(ChartArea area)
		{
			foreach (object chartType2 in area.ChartTypes)
			{
				IChartType chartType = area.Common.ChartTypeRegistry.GetChartType(chartType2.ToString());
				if (chartType != null && (chartType.CircularChartArea || !chartType.RequireAxes))
				{
					return true;
				}
			}
			return false;
		}

		protected internal virtual IList GetAreaMarkers(ChartGraphics graph, ChartArea area)
		{
			ArrayList arrayList = new ArrayList();
			if (area == null)
			{
				return arrayList;
			}
			IList markers = this.GetMarkers(area.PlotAreaPosition.ToRectangleF());
			if (this.IsChartAreaCircular(area))
			{
				markers = this.GetMarkers(area.Position.ToRectangleF());
			}
			if (this.IsArea3D(area))
			{
				float z = 0f;
				Point3D[] array = new Point3D[markers.Count];
				for (int i = 0; i < markers.Count; i++)
				{
					array[i] = new Point3D(((PointF)markers[i]).X, ((PointF)markers[i]).Y, z);
				}
				area.matrix3D.TransformPoints(array);
				for (int j = 0; j < markers.Count; j++)
				{
					markers[j] = array[j].PointF;
				}
			}
			foreach (PointF item in markers)
			{
				arrayList.Add(graph.GetAbsolutePoint(item));
			}
			return arrayList;
		}

		protected internal virtual void SearchForHotRegion()
		{
			object contextObjectNoLabel = this.Result.GetContextObjectNoLabel();
			ChartElementType chartElementType = this.Result.GetChartElementType();
			this.hotRegion = new HotRegion();
			if (contextObjectNoLabel != null)
			{
				HotRegionsList hotRegionsList = this.Chart.common.HotRegionsList;
				if (hotRegionsList.List.Count == 0)
				{
					this.ChartControl.HitTest(2, 2);
				}
				int num = hotRegionsList.List.Count - 1;
				HotRegion hotRegion;
				while (true)
				{
					if (num >= 0)
					{
						hotRegion = (HotRegion)hotRegionsList.List[num];
						if (hotRegion.SelectedObject == contextObjectNoLabel && hotRegion.Type == chartElementType && hotRegion.Type == chartElementType)
						{
							break;
						}
						num--;
						continue;
					}
					return;
				}
				this.hotRegion = hotRegion;
			}
		}

		internal void Reset()
		{
			this.SelectionPoint = Point.Empty;
		}

		internal virtual void HitTest()
		{
			this.invalidated = false;
			this.selectedObjectInfo = new ObjectInfo();
			if (!(this.selectionPoint == Point.Empty) && this.SelectableTypes != 0)
			{
				Chart chart = this.ChartControl;
				if (chart != null)
				{
					try
					{
						HitTestResult hitTestResult = null;
						if (this.SelectableTypes == ContextElementTypes.Any)
						{
							hitTestResult = chart.HitTest(this.SelectionPoint.X, this.SelectionPoint.Y);
						}
						else
						{
							foreach (ChartElementType hitOrder in this.GetHitOrderList())
							{
								hitTestResult = chart.HitTest(this.SelectionPoint.X, this.SelectionPoint.Y, hitOrder);
								if (hitTestResult.Object != null)
								{
									break;
								}
							}
						}
						this.selectedObjectInfo = ObjectInfo.Get(hitTestResult, chart);
					}
					catch
					{
						chart.IsDesignMode();
					}
				}
			}
		}

		internal virtual void DrawSelection()
		{
			if (this.enabled)
			{
				ChartGraphics graph = this.Graph;
				if (graph != null)
				{
					this.DrawSelection(graph.Graphics);
				}
			}
		}

		protected internal virtual IList GetMarkers(RectangleF rect)
		{
			return this.GetMarkers(rect, true);
		}

		protected internal virtual IList GetMarkers(RectangleF rect, bool addAdditionalMarkers)
		{
			ArrayList arrayList = new ArrayList();
			if (!addAdditionalMarkers)
			{
				if (rect.Width > 0.0 && rect.Height > 0.0)
				{
					arrayList.Add(new PointF(rect.Left, rect.Top));
					arrayList.Add(new PointF(rect.Right, rect.Top));
					arrayList.Add(new PointF(rect.Right, rect.Bottom));
					arrayList.Add(new PointF(rect.Left, rect.Bottom));
				}
				else if (rect.Width > 0.0)
				{
					arrayList.Add(new PointF(rect.Left, rect.Top));
					arrayList.Add(new PointF(rect.Right, rect.Top));
				}
				else if (rect.Height > 0.0)
				{
					arrayList.Add(new PointF(rect.Left, rect.Top));
					arrayList.Add(new PointF(rect.Left, rect.Bottom));
				}
			}
			else if (rect.Width > 0.0)
			{
				arrayList.Add(new PointF(rect.Left, rect.Top));
				if (rect.Width > 30.0)
				{
					arrayList.Add(new PointF((float)(rect.Left + rect.Width / 2.0), rect.Top));
				}
				arrayList.Add(new PointF(rect.Right, rect.Top));
				if (rect.Height > 30.0)
				{
					arrayList.Add(new PointF(rect.Right, (float)(rect.Top + rect.Height / 2.0)));
				}
				arrayList.Add(new PointF(rect.Right, rect.Bottom));
				if (rect.Width > 30.0)
				{
					arrayList.Add(new PointF((float)(rect.Left + rect.Width / 2.0), rect.Bottom));
				}
				arrayList.Add(new PointF(rect.Left, rect.Bottom));
				if (rect.Height > 30.0)
				{
					arrayList.Add(new PointF(rect.Left, (float)(rect.Top + rect.Height / 2.0)));
				}
			}
			else if (rect.Width > 0.0)
			{
				arrayList.Add(new PointF(rect.Left, rect.Top));
				if (rect.Width > 30.0)
				{
					arrayList.Add(new PointF((float)(rect.Left + rect.Width / 2.0), rect.Top));
				}
				arrayList.Add(new PointF(rect.Right, rect.Top));
			}
			else if (rect.Height > 0.0)
			{
				arrayList.Add(new PointF(rect.Left, rect.Bottom));
				if (rect.Height > 30.0)
				{
					arrayList.Add(new PointF(rect.Left, (float)(rect.Top + rect.Height / 2.0)));
				}
				arrayList.Add(new PointF(rect.Left, rect.Top));
			}
			return arrayList;
		}

		protected internal PointF Transform3D(ChartArea3D chartArea, DataPoint point, ChartGraphics graph)
		{
			if (chartArea is ChartArea && this.IsArea3D((ChartArea)chartArea))
			{
				float num = chartArea.areaSceneDepth;
				if (point != null && point.series != null)
				{
					float num2 = 0f;
					chartArea.GetSeriesZPositionAndDepth(point.series, out num2, out num);
					num = (float)(num + num2 / 2.0);
				}
				PointF positionRel = point.positionRel;
				Point3D[] array = new Point3D[1]
				{
					new Point3D(positionRel.X, positionRel.Y, num)
				};
				chartArea.matrix3D.TransformPoints(array);
				return array[0].PointF;
			}
			return point.positionRel;
		}

		internal bool IsElementClickable(object element, ChartElementType chartElementType)
		{
			if (element != null)
			{
				HotRegionsList hotRegionsList = this.Chart.common.HotRegionsList;
				if (hotRegionsList.List.Count == 0)
				{
					this.ChartControl.HitTest(2, 2);
				}
				foreach (HotRegion item in hotRegionsList.List)
				{
					ChartElementType type = item.Type;
					if (type == ChartElementType.DataPointLabel)
					{
						if (element is Series && string.Compare(item.SeriesName, ((Series)element).Name, StringComparison.Ordinal) == 0 && item.Type == chartElementType)
						{
							return true;
						}
					}
					else if (item.SelectedObject == element && item.Type == chartElementType)
					{
						return true;
					}
				}
			}
			return false;
		}

		protected internal virtual IList GetMarkers(ChartGraphics graph)
		{
			ArrayList arrayList = new ArrayList();
			if (this.Result.ElementType == ContextElementTypes.None)
			{
				return arrayList;
			}
			if (this.Result.ElementType == ContextElementTypes.ChartArea)
			{
				return this.GetAreaMarkers(graph, this.Result.ChartArea);
			}
			if (this.Result.ElementType == ContextElementTypes.Series)
			{
				Series series = this.Result.GetContextObject() as Series;
				if (series != null)
				{
					string text = series.ChartArea;
					if (string.CompareOrdinal(text, "Default") == 0 && this.Chart.ChartAreas.GetIndex(text) == -1 && this.Chart.ChartAreas.Count > 0)
					{
						text = this.Chart.ChartAreas[0].Name;
					}
					if (this.Chart.ChartAreas.GetIndex(text) != -1 && series.Enabled)
					{
						ChartArea chartArea = this.Chart.ChartAreas[text];
						if (this.ChartControl.Series.GetIndex(series.Name) != -1)
						{
							series = this.ChartControl.Series[series.Name];
						}
						DataPointCollection dataPointCollection = series.Points;
						if (dataPointCollection.Count == 0)
						{
							dataPointCollection = series.fakeDataPoints;
						}
						{
							foreach (DataPoint item in dataPointCollection)
							{
								PointF relative = this.Transform3D(chartArea, item, graph);
								if (!float.IsNaN(relative.X) && !float.IsNaN(relative.Y))
								{
									arrayList.Add(graph.GetAbsolutePoint(relative));
								}
							}
							return arrayList;
						}
					}
				}
			}
			else
			{
				if (this.Result.ElementType.ToString().IndexOf("Axis", StringComparison.Ordinal) != -1)
				{
					return this.GetAxisMarkers(graph, this.Result.GetContextObjectNoLabel() as Axis);
				}
				if (this.hotRegion.Type != 0)
				{
					RectangleF absoluteRectangle = graph.GetAbsoluteRectangle(this.hotRegion.BoundingRectangle);
					if (this.Result.ElementType == ContextElementTypes.Axis || this.Result.ElementType == ContextElementTypes.AxisLabel)
					{
						SizeF size = absoluteRectangle.Size;
						if (absoluteRectangle.Width > absoluteRectangle.Height)
						{
							absoluteRectangle.Height = 0f;
						}
						else
						{
							absoluteRectangle.Width = 0f;
						}
						Axis axis = this.Result.GetContextObjectNoLabel() as Axis;
						if (axis != null)
						{
							switch (axis.Type)
							{
							case AxisName.X2:
								absoluteRectangle.Offset(0f, size.Height);
								break;
							case AxisName.Y:
								absoluteRectangle.Offset(size.Width, 0f);
								break;
							}
						}
					}
					return this.GetMarkers(absoluteRectangle);
				}
			}
			return arrayList;
		}

		internal virtual void DrawSelection(Graphics g)
		{
			if (!this.Chart.isSelectionMode)
			{
				this.CheckInvalidated();
				if (this.Result.ElementType != 0)
				{
					this.SearchForHotRegion();
					ChartGraphics graph = this.Graph;
					if (graph != null)
					{
						graph.Graphics = g;
						IList markers = this.GetMarkers(graph);
						foreach (PointF item in markers)
						{
							int markerSize = 5;
							graph.DrawMarkerAbs(item, MarkerStyle.Square, markerSize, Color.White, Color.Gray, 1, string.Empty, Color.Empty, 0, Color.Empty, RectangleF.Empty, true);
						}
					}
				}
			}
		}

		object IServiceProvider.GetService(Type serviceType)
		{
			if (serviceType == base.GetType())
			{
				return this;
			}
			return this.service.GetService(serviceType);
		}
	}
}
