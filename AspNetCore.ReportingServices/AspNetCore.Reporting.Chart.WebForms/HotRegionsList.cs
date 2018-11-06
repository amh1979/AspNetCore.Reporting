using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms
{
	internal class HotRegionsList
	{
		private ProcessMode processChartMode = ProcessMode.Paint;

		private ArrayList regionList;

		private CommonElements common;

		internal bool hitTestCalled;

		internal ProcessMode ProcessChartMode
		{
			get
			{
				return this.processChartMode;
			}
			set
			{
				this.processChartMode = value;
				if (this.common != null)
				{
					this.common.processModePaint = ((this.processChartMode & ProcessMode.Paint) == ProcessMode.Paint);
					this.common.processModeRegions = ((this.processChartMode & ProcessMode.HotRegions) == ProcessMode.HotRegions || (this.processChartMode & ProcessMode.ImageMaps) == ProcessMode.ImageMaps);
				}
			}
		}

		internal ArrayList List
		{
			get
			{
				return this.regionList;
			}
			set
			{
				this.regionList = value;
			}
		}

		public HotRegionsList(CommonElements common)
		{
			this.common = common;
		}

		internal void CheckHotRegions(int x, int y, ChartElementType requestedElement, bool ignoreTransparent, out string series, out int point, out ChartElementType type, out object obj, out object subObj)
		{
			obj = null;
			subObj = null;
			point = 0;
			series = "";
			type = ChartElementType.Nothing;
			RectangleF rectangleF = new RectangleF((float)(x - 1), (float)(y - 1), 2f, 2f);
			float x2 = this.common.graph.GetRelativePoint(new PointF((float)x, (float)y)).X;
			float y2 = this.common.graph.GetRelativePoint(new PointF((float)x, (float)y)).Y;
			RectangleF relativeRectangle = this.common.graph.GetRelativeRectangle(rectangleF);
			bool flag = false;
			int num = this.regionList.Count - 1;
			HotRegion hotRegion;
			while (true)
			{
				if (num >= 0)
				{
					hotRegion = (HotRegion)this.regionList[num];
					float x3;
					float y3;
					RectangleF rect;
					if (hotRegion.RelativeCoordinates)
					{
						x3 = x2;
						y3 = y2;
						rect = relativeRectangle;
					}
					else
					{
						x3 = (float)x;
						y3 = (float)y;
						rect = rectangleF;
					}
					if (requestedElement == ChartElementType.Nothing || requestedElement == hotRegion.Type)
					{
						flag = false;
						if (hotRegion.SeriesName.Length > 0 && (this.common == null || this.common.Chart.Series.GetIndex(hotRegion.SeriesName) < 0 || hotRegion.PointIndex >= this.common.Chart.Series[hotRegion.SeriesName].Points.Count))
						{
							if (!this.common.Chart.IsDesignMode())
							{
								goto IL_0290;
							}
							if (this.common.Chart.Series.GetIndex(hotRegion.SeriesName) > -1 && hotRegion.PointIndex > -1)
							{
								flag = true;
							}
						}
						if ((!ignoreTransparent || !this.IsElementTransparent(hotRegion)) && hotRegion.BoundingRectangle.IntersectsWith(rect))
						{
							bool flag2 = false;
							if (hotRegion.Path != null)
							{
								GraphicsPathIterator graphicsPathIterator = new GraphicsPathIterator(hotRegion.Path);
								if (graphicsPathIterator.SubpathCount > 1)
								{
									GraphicsPath graphicsPath = new GraphicsPath();
									while (graphicsPathIterator.NextMarker(graphicsPath) > 0 && !flag2)
									{
										if (graphicsPath.IsVisible(x3, y3))
										{
											flag2 = true;
										}
										graphicsPath.Reset();
									}
								}
								else if (hotRegion.Path.IsVisible(x3, y3))
								{
									flag2 = true;
								}
							}
							else
							{
								flag2 = true;
							}
							if (flag2)
							{
								break;
							}
						}
					}
					goto IL_0290;
				}
				return;
				IL_0290:
				num--;
			}
			if (flag)
			{
				series = hotRegion.SeriesName;
				point = -1;
				obj = this.common.Chart.Series[hotRegion.SeriesName];
				type = ChartElementType.Nothing;
			}
			else
			{
				series = hotRegion.SeriesName;
				point = hotRegion.PointIndex;
				obj = hotRegion.SelectedObject;
				subObj = hotRegion.SelectedSubObject;
				type = hotRegion.Type;
			}
		}

		private bool IsElementTransparent(HotRegion region)
		{
			bool result = false;
			if (region.Type == ChartElementType.DataPoint)
			{
				if (this.common != null && this.common.Chart != null)
				{
					DataPoint dataPoint = region.SelectedObject as DataPoint;
					if (region.SeriesName.Length > 0)
					{
						dataPoint = this.common.Chart.Series[region.SeriesName].Points[region.PointIndex];
					}
					if (dataPoint != null && dataPoint.Color == Color.Transparent)
					{
						result = true;
					}
				}
			}
			else if (region.SelectedObject is Axis)
			{
				if (((Axis)region.SelectedObject).LineColor == Color.Transparent)
				{
					result = true;
				}
			}
			else if (region.SelectedObject is ChartArea)
			{
				if (((ChartArea)region.SelectedObject).BackColor == Color.Transparent)
				{
					result = true;
				}
			}
			else if (region.SelectedObject is Legend)
			{
				if (((Legend)region.SelectedObject).BackColor == Color.Transparent)
				{
					result = true;
				}
			}
			else if (region.SelectedObject is Grid)
			{
				if (((Grid)region.SelectedObject).LineColor == Color.Transparent)
				{
					result = true;
				}
			}
			else if (region.SelectedObject is StripLine)
			{
				if (((StripLine)region.SelectedObject).BackColor == Color.Transparent)
				{
					result = true;
				}
			}
			else if (region.SelectedObject is TickMark)
			{
				if (((TickMark)region.SelectedObject).LineColor == Color.Transparent)
				{
					result = true;
				}
			}
			else if (region.SelectedObject is Title)
			{
				Title title = (Title)region.SelectedObject;
				if ((title.Text.Length == 0 || title.Color == Color.Transparent) && (title.BackColor == Color.Transparent || title.BackColor.IsEmpty))
				{
					result = true;
				}
			}
			return result;
		}

		public void AddHotRegion(ChartGraphics graph, RectangleF rectSize, DataPoint point, string seriesName, int pointIndex)
		{
			if ((this.ProcessChartMode & ProcessMode.ImageMaps) == ProcessMode.ImageMaps && this.common.ChartPicture.MapEnabled && (point.ToolTip.Length > 0 || point.Href.Length > 0 || point.MapAreaAttributes.Length > 0))
			{
				this.common.ChartPicture.MapAreas.Insert(0, point.ReplaceKeywords(point.ToolTip), point.ReplaceKeywords(point.Href), point.ReplaceKeywords(point.MapAreaAttributes), rectSize, ((IMapAreaAttributes)point).Tag);
			}
			if ((this.ProcessChartMode & ProcessMode.HotRegions) == ProcessMode.HotRegions)
			{
				HotRegion hotRegion = new HotRegion();
				hotRegion.BoundingRectangle = rectSize;
				hotRegion.SeriesName = seriesName;
				hotRegion.PointIndex = pointIndex;
				hotRegion.Type = ChartElementType.DataPoint;
				hotRegion.RelativeCoordinates = true;
				if (point != null && point.IsAttributeSet("OriginalPointIndex"))
				{
					hotRegion.PointIndex = int.Parse(((DataPointAttributes)point)["OriginalPointIndex"], CultureInfo.InvariantCulture);
				}
				this.regionList.Add(hotRegion);
			}
		}

		internal void AddHotRegion(int insertIndex, ChartGraphics graph, RectangleF rectSize, DataPoint point, string seriesName, int pointIndex)
		{
			if ((this.ProcessChartMode & ProcessMode.ImageMaps) == ProcessMode.ImageMaps && this.common.ChartPicture.MapEnabled && (point.ToolTip.Length > 0 || point.Href.Length > 0 || point.MapAreaAttributes.Length > 0))
			{
				this.common.ChartPicture.MapAreas.Insert(0, point.ReplaceKeywords(point.ToolTip), point.ReplaceKeywords(point.Href), point.ReplaceKeywords(point.MapAreaAttributes), rectSize, ((IMapAreaAttributes)point).Tag);
			}
			if ((this.ProcessChartMode & ProcessMode.HotRegions) == ProcessMode.HotRegions)
			{
				HotRegion hotRegion = new HotRegion();
				hotRegion.BoundingRectangle = rectSize;
				hotRegion.SeriesName = seriesName;
				hotRegion.PointIndex = pointIndex;
				hotRegion.Type = ChartElementType.DataPoint;
				hotRegion.RelativeCoordinates = true;
				if (point != null && point.IsAttributeSet("OriginalPointIndex"))
				{
					hotRegion.PointIndex = int.Parse(((DataPointAttributes)point)["OriginalPointIndex"], CultureInfo.InvariantCulture);
				}
				this.regionList.Add(hotRegion);
			}
		}

		internal void AddHotRegion(GraphicsPath path, bool relativePath, ChartGraphics graph, DataPoint point, string seriesName, int pointIndex)
		{
			if (path != null)
			{
				if ((this.ProcessChartMode & ProcessMode.ImageMaps) == ProcessMode.ImageMaps && this.common.ChartPicture.MapEnabled && (point.ToolTip.Length > 0 || point.Href.Length > 0 || point.MapAreaAttributes.Length > 0))
				{
					int count = this.common.ChartPicture.MapAreas.Count;
					this.common.ChartPicture.MapAreas.Insert(0, point.ReplaceKeywords(point.ToolTip), point.ReplaceKeywords(point.Href), point.ReplaceKeywords(point.MapAreaAttributes), path, !relativePath, graph);
					for (int i = 0; i < this.common.ChartPicture.MapAreas.Count - count; i++)
					{
						((IMapAreaAttributes)this.common.ChartPicture.MapAreas[i]).Tag = ((IMapAreaAttributes)point).Tag;
					}
				}
				if ((this.ProcessChartMode & ProcessMode.HotRegions) == ProcessMode.HotRegions)
				{
					HotRegion hotRegion = new HotRegion();
					hotRegion.SeriesName = seriesName;
					hotRegion.PointIndex = pointIndex;
					hotRegion.Type = ChartElementType.DataPoint;
					hotRegion.Path = path;
					hotRegion.BoundingRectangle = path.GetBounds();
					hotRegion.RelativeCoordinates = relativePath;
					if (point != null && point.IsAttributeSet("OriginalPointIndex"))
					{
						hotRegion.PointIndex = int.Parse(((DataPointAttributes)point)["OriginalPointIndex"], CultureInfo.InvariantCulture);
					}
					this.regionList.Add(hotRegion);
				}
			}
		}

		internal void AddHotRegion(int insertIndex, GraphicsPath path, bool relativePath, ChartGraphics graph, DataPoint point, string seriesName, int pointIndex)
		{
			if ((this.ProcessChartMode & ProcessMode.ImageMaps) == ProcessMode.ImageMaps && this.common.ChartPicture.MapEnabled && (point.ToolTip.Length > 0 || point.Href.Length > 0 || point.MapAreaAttributes.Length > 0))
			{
				int count = this.common.ChartPicture.MapAreas.Count;
				this.common.ChartPicture.MapAreas.Insert(insertIndex, point.ReplaceKeywords(point.ToolTip), point.ReplaceKeywords(point.Href), point.ReplaceKeywords(point.MapAreaAttributes), path, !relativePath, graph);
				for (int i = insertIndex; i < this.common.ChartPicture.MapAreas.Count - count; i++)
				{
					((IMapAreaAttributes)this.common.ChartPicture.MapAreas[i]).Tag = ((IMapAreaAttributes)point).Tag;
				}
			}
			if ((this.ProcessChartMode & ProcessMode.HotRegions) == ProcessMode.HotRegions)
			{
				HotRegion hotRegion = new HotRegion();
				hotRegion.SeriesName = seriesName;
				hotRegion.PointIndex = pointIndex;
				hotRegion.Type = ChartElementType.DataPoint;
				hotRegion.Path = path;
				hotRegion.BoundingRectangle = path.GetBounds();
				hotRegion.RelativeCoordinates = relativePath;
				if (point != null && point.IsAttributeSet("OriginalPointIndex"))
				{
					hotRegion.PointIndex = int.Parse(((DataPointAttributes)point)["OriginalPointIndex"], CultureInfo.InvariantCulture);
				}
				this.regionList.Add(hotRegion);
			}
		}

		internal void AddHotRegion(ChartGraphics graph, GraphicsPath path, bool relativePath, float[] coord, DataPoint point, string seriesName, int pointIndex)
		{
			if ((this.ProcessChartMode & ProcessMode.ImageMaps) == ProcessMode.ImageMaps && this.common.ChartPicture.MapEnabled && (point.ToolTip.Length > 0 || point.Href.Length > 0 || point.MapAreaAttributes.Length > 0))
			{
				this.common.ChartPicture.MapAreas.Insert(0, MapAreaShape.Polygon, point.ReplaceKeywords(point.ToolTip), point.ReplaceKeywords(point.Href), point.ReplaceKeywords(point.MapAreaAttributes), coord, ((IMapAreaAttributes)point).Tag);
			}
			if ((this.ProcessChartMode & ProcessMode.HotRegions) == ProcessMode.HotRegions)
			{
				HotRegion hotRegion = new HotRegion();
				hotRegion.SeriesName = seriesName;
				hotRegion.PointIndex = pointIndex;
				hotRegion.Type = ChartElementType.DataPoint;
				hotRegion.Path = path;
				hotRegion.BoundingRectangle = path.GetBounds();
				hotRegion.RelativeCoordinates = relativePath;
				if (point != null && point.IsAttributeSet("OriginalPointIndex"))
				{
					hotRegion.PointIndex = int.Parse(((DataPointAttributes)point)["OriginalPointIndex"], CultureInfo.InvariantCulture);
				}
				this.regionList.Add(hotRegion);
			}
		}

		internal void AddHotRegion(int insertIndex, ChartGraphics graph, float x, float y, float radius, DataPoint point, string seriesName, int pointIndex)
		{
			if ((this.ProcessChartMode & ProcessMode.ImageMaps) == ProcessMode.ImageMaps && this.common.ChartPicture.MapEnabled && (point.ToolTip.Length > 0 || point.Href.Length > 0 || point.MapAreaAttributes.Length > 0))
			{
				float[] coordinates = new float[3]
				{
					x,
					y,
					radius
				};
				this.common.ChartPicture.MapAreas.Insert(insertIndex, MapAreaShape.Circle, point.ReplaceKeywords(point.ToolTip), point.ReplaceKeywords(point.Href), point.ReplaceKeywords(point.MapAreaAttributes), coordinates, ((IMapAreaAttributes)point).Tag);
			}
			if ((this.ProcessChartMode & ProcessMode.HotRegions) == ProcessMode.HotRegions)
			{
				HotRegion hotRegion = new HotRegion();
				PointF absolutePoint = graph.GetAbsolutePoint(new PointF(x, y));
				SizeF absoluteSize = graph.GetAbsoluteSize(new SizeF(radius, radius));
				GraphicsPath graphicsPath = new GraphicsPath();
				graphicsPath.AddEllipse(absolutePoint.X - absoluteSize.Width, absolutePoint.Y - absoluteSize.Width, (float)(2.0 * absoluteSize.Width), (float)(2.0 * absoluteSize.Width));
				hotRegion.BoundingRectangle = graphicsPath.GetBounds();
				hotRegion.SeriesName = seriesName;
				hotRegion.Type = ChartElementType.DataPoint;
				hotRegion.PointIndex = pointIndex;
				hotRegion.Path = graphicsPath;
				hotRegion.RelativeCoordinates = false;
				if (point != null && point.IsAttributeSet("OriginalPointIndex"))
				{
					hotRegion.PointIndex = int.Parse(((DataPointAttributes)point)["OriginalPointIndex"], CultureInfo.InvariantCulture);
				}
				this.regionList.Add(hotRegion);
			}
		}

		internal void AddHotRegion(ChartGraphics graph, RectangleF rectArea, string toolTip, string hRef, string mapAreaAttributes, object selectedObject, ChartElementType type, string series)
		{
			if ((this.ProcessChartMode & ProcessMode.ImageMaps) == ProcessMode.ImageMaps && this.common.ChartPicture.MapEnabled && (toolTip.Length > 0 || hRef.Length > 0 || mapAreaAttributes.Length > 0))
			{
				this.common.ChartPicture.MapAreas.Add(toolTip, hRef, mapAreaAttributes, rectArea, ((IMapAreaAttributes)selectedObject).Tag);
			}
			if ((this.ProcessChartMode & ProcessMode.HotRegions) == ProcessMode.HotRegions)
			{
				HotRegion hotRegion = new HotRegion();
				hotRegion.BoundingRectangle = rectArea;
				hotRegion.RelativeCoordinates = true;
				hotRegion.Type = type;
				hotRegion.SelectedObject = selectedObject;
				if (series != null && series != string.Empty)
				{
					hotRegion.SeriesName = series;
				}
				this.regionList.Add(hotRegion);
			}
		}

		internal void AddHotRegion(ChartGraphics graph, RectangleF rectArea, string toolTip, string hRef, string mapAreaAttributes, object selectedObject, object selectedSubObject, ChartElementType type, string series)
		{
			if ((this.ProcessChartMode & ProcessMode.ImageMaps) == ProcessMode.ImageMaps && this.common.ChartPicture.MapEnabled && (toolTip.Length > 0 || hRef.Length > 0 || mapAreaAttributes.Length > 0))
			{
				this.common.ChartPicture.MapAreas.Add(toolTip, hRef, mapAreaAttributes, rectArea, ((IMapAreaAttributes)selectedObject).Tag);
			}
			if ((this.ProcessChartMode & ProcessMode.HotRegions) == ProcessMode.HotRegions)
			{
				HotRegion hotRegion = new HotRegion();
				hotRegion.BoundingRectangle = rectArea;
				hotRegion.RelativeCoordinates = true;
				hotRegion.Type = type;
				hotRegion.SelectedObject = selectedObject;
				hotRegion.SelectedSubObject = selectedSubObject;
				if (series != null && series != string.Empty)
				{
					hotRegion.SeriesName = series;
				}
				this.regionList.Add(hotRegion);
			}
		}

		internal void AddHotRegion(ChartGraphics graph, GraphicsPath path, bool relativePath, string toolTip, string hRef, string mapAreaAttributes, object selectedObject, ChartElementType type)
		{
			if ((this.ProcessChartMode & ProcessMode.ImageMaps) == ProcessMode.ImageMaps && this.common.ChartPicture.MapEnabled && (toolTip.Length > 0 || hRef.Length > 0 || mapAreaAttributes.Length > 0))
			{
				this.common.ChartPicture.MapAreas.Insert(0, toolTip, hRef, mapAreaAttributes, path, !relativePath, graph);
			}
			if ((this.ProcessChartMode & ProcessMode.HotRegions) == ProcessMode.HotRegions)
			{
				HotRegion hotRegion = new HotRegion();
				hotRegion.Type = type;
				hotRegion.Path = path;
				hotRegion.SelectedObject = selectedObject;
				hotRegion.BoundingRectangle = path.GetBounds();
				hotRegion.RelativeCoordinates = relativePath;
				this.regionList.Add(hotRegion);
			}
		}

		internal void AddHotRegion(RectangleF rectArea, object selectedObject, ChartElementType type, bool relativeCoordinates)
		{
			this.AddHotRegion(rectArea, selectedObject, type, relativeCoordinates, false);
		}

		internal void AddHotRegion(RectangleF rectArea, object selectedObject, ChartElementType type, bool relativeCoordinates, bool insertAtBeginning)
		{
			if ((this.ProcessChartMode & ProcessMode.HotRegions) == ProcessMode.HotRegions)
			{
				HotRegion hotRegion = new HotRegion();
				hotRegion.BoundingRectangle = rectArea;
				hotRegion.RelativeCoordinates = relativeCoordinates;
				hotRegion.Type = type;
				hotRegion.SelectedObject = selectedObject;
				if (insertAtBeginning)
				{
					this.regionList.Insert(this.regionList.Count - 1, hotRegion);
				}
				else
				{
					this.regionList.Add(hotRegion);
				}
			}
		}

		internal void AddHotRegion(GraphicsPath path, bool relativePath, ChartGraphics graph, ChartElementType type, object selectedObject)
		{
			if ((this.ProcessChartMode & ProcessMode.HotRegions) == ProcessMode.HotRegions)
			{
				HotRegion hotRegion = new HotRegion();
				hotRegion.SelectedObject = selectedObject;
				hotRegion.Type = type;
				hotRegion.Path = path;
				hotRegion.BoundingRectangle = path.GetBounds();
				hotRegion.RelativeCoordinates = relativePath;
				this.regionList.Add(hotRegion);
			}
		}

		internal int FindInsertIndex()
		{
			int num = 0;
			foreach (MapArea mapArea in this.common.ChartPicture.MapAreas)
			{
				if (!mapArea.Custom)
				{
					return num;
				}
				num++;
			}
			return num;
		}
	}
}
