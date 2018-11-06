using AspNetCore.Reporting.Chart.WebForms.ChartTypes;
using AspNetCore.Reporting.Chart.WebForms.Design;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms
{
	internal class ChartArea3D : ChartAreaAxes
	{
		internal class PointsDrawingOrderComparer : IComparer
		{
			private ChartArea area;

			private Point3D areaProjectionCenter = new Point3D(float.NaN, float.NaN, float.NaN);

			private bool selection;

			public PointsDrawingOrderComparer(ChartArea area, bool selection, COPCoordinates coord)
			{
				this.area = area;
				this.selection = selection;
				if (area.DrawPointsToCenter(ref coord))
				{
					this.areaProjectionCenter = area.GetCenterOfProjection(coord);
				}
			}

			public int Compare(object o1, object o2)
			{
				DataPoint3D dataPoint3D = (DataPoint3D)o1;
				DataPoint3D dataPoint3D2 = (DataPoint3D)o2;
				int num = 0;
				if (dataPoint3D.xPosition < dataPoint3D2.xPosition)
				{
					num = -1;
				}
				else if (dataPoint3D.xPosition > dataPoint3D2.xPosition)
				{
					num = 1;
				}
				else
				{
					if (dataPoint3D.yPosition < dataPoint3D2.yPosition)
					{
						num = 1;
					}
					else if (dataPoint3D.yPosition > dataPoint3D2.yPosition)
					{
						num = -1;
					}
					if (!float.IsNaN(this.areaProjectionCenter.Y))
					{
						double num2 = Math.Min(dataPoint3D.yPosition, dataPoint3D.height);
						double num3 = Math.Max(dataPoint3D.yPosition, dataPoint3D.height);
						double num4 = Math.Min(dataPoint3D2.yPosition, dataPoint3D2.height);
						double num5 = Math.Max(dataPoint3D2.yPosition, dataPoint3D2.height);
						if (this.area.IsBottomSceneWallVisible())
						{
							if (num2 <= (double)this.areaProjectionCenter.Y && num4 <= (double)this.areaProjectionCenter.Y)
							{
								num *= -1;
							}
							else if (num2 <= (double)this.areaProjectionCenter.Y)
							{
								num = 1;
							}
						}
						else
						{
							num = ((!(num3 >= (double)this.areaProjectionCenter.Y) || !(num5 >= (double)this.areaProjectionCenter.Y)) ? ((num3 >= (double)this.areaProjectionCenter.Y) ? 1 : (num * -1)) : num);
						}
					}
					else if (!this.area.IsBottomSceneWallVisible())
					{
						num *= -1;
					}
				}
				if (dataPoint3D.xPosition != dataPoint3D2.xPosition)
				{
					if (!float.IsNaN(this.areaProjectionCenter.X))
					{
						if (dataPoint3D.xPosition + dataPoint3D.width / 2.0 >= (double)this.areaProjectionCenter.X && dataPoint3D2.xPosition + dataPoint3D2.width / 2.0 >= (double)this.areaProjectionCenter.X)
						{
							num *= -1;
						}
					}
					else if (this.area.DrawPointsInReverseOrder())
					{
						num *= -1;
					}
				}
				if (!this.selection)
				{
					return num;
				}
				return -num;
			}
		}

		private ChartArea3DStyle area3DStyle = new ChartArea3DStyle();

		internal Matrix3D matrix3D = new Matrix3D();

		internal SizeF areaSceneWallWidth = SizeF.Empty;

		internal float areaSceneDepth;

		private SurfaceNames visibleSurfaces;

		private double pointsDepth;

		private double pointsGapDepth;

		internal bool reverseSeriesOrder;

		internal bool oldReverseX;

		internal bool oldReverseY;

		internal int oldYAngle = 30;

		internal ArrayList seriesDrawingOrder;

		internal ArrayList stackGroupNames;

		internal ArrayList seriesClusters;

		[SRCategory("CategoryAttribute3D")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[Bindable(true)]
		[DefaultValue(null)]
		[SRDescription("DescriptionAttributeArea3DStyle")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ChartArea3DStyle Area3DStyle
		{
			get
			{
				return this.area3DStyle;
			}
			set
			{
				this.area3DStyle = value;
				this.area3DStyle.Initialize((ChartArea)this);
			}
		}

		public ChartArea3D()
		{
			this.area3DStyle.Initialize((ChartArea)this);
		}

		public void TransformPoints(Point3D[] points)
		{
			foreach (Point3D point3D in points)
			{
				point3D.Z = (float)(point3D.Z / 100.0 * this.areaSceneDepth);
			}
			this.matrix3D.TransformPoints(points);
		}

		protected void DrawArea3DScene(ChartGraphics graph, RectangleF position)
		{
			ChartArea chartArea = (ChartArea)this;
			this.areaSceneWallWidth = graph.GetRelativeSize(new SizeF((float)this.Area3DStyle.WallWidth, (float)this.Area3DStyle.WallWidth));
			this.areaSceneDepth = this.GetArea3DSceneDepth();
			this.matrix3D.Initialize(position, this.areaSceneDepth, (float)this.Area3DStyle.XAngle, (float)this.Area3DStyle.YAngle, (float)this.Area3DStyle.Perspective, this.Area3DStyle.RightAngleAxes);
			this.matrix3D.InitLight(this.Area3DStyle.Light);
			this.visibleSurfaces = graph.GetVisibleSurfaces(position, 0f, this.areaSceneDepth, this.matrix3D);
			Color color = chartArea.BackColor;
			if (color == Color.Transparent)
			{
				this.areaSceneWallWidth = SizeF.Empty;
			}
			else
			{
				if (color == Color.Empty)
				{
					color = Color.LightGray;
				}
				if (this.IsBottomSceneWallVisible())
				{
					position.Height += this.areaSceneWallWidth.Height;
				}
				position.Width += this.areaSceneWallWidth.Width;
				if (this.Area3DStyle.YAngle > 0)
				{
					position.X -= this.areaSceneWallWidth.Width;
				}
				RectangleF position2 = new RectangleF(position.Location, position.Size);
				float width = this.areaSceneWallWidth.Width;
				float positionZ = (float)(0.0 - width);
				if (this.IsMainSceneWallOnFront())
				{
					positionZ = this.areaSceneDepth;
				}
				graph.Fill3DRectangle(position2, positionZ, width, this.matrix3D, chartArea.Area3DStyle.Light, color, chartArea.BackHatchStyle, chartArea.BackImage, chartArea.BackImageMode, chartArea.BackImageTransparentColor, chartArea.BackImageAlign, chartArea.BackGradientType, chartArea.BackGradientEndColor, chartArea.BorderColor, chartArea.BorderWidth, chartArea.BorderStyle, PenAlignment.Outset, DrawingOperationTypes.DrawElement);
				position2 = new RectangleF(position.Location, position.Size);
				position2.Width = this.areaSceneWallWidth.Width;
				if (!this.IsSideSceneWallOnLeft())
				{
					position2.X = position.Right - this.areaSceneWallWidth.Width;
				}
				graph.Fill3DRectangle(position2, 0f, this.areaSceneDepth, this.matrix3D, chartArea.Area3DStyle.Light, color, chartArea.BackHatchStyle, chartArea.BackImage, chartArea.BackImageMode, chartArea.BackImageTransparentColor, chartArea.BackImageAlign, chartArea.BackGradientType, chartArea.BackGradientEndColor, chartArea.BorderColor, chartArea.BorderWidth, chartArea.BorderStyle, PenAlignment.Outset, DrawingOperationTypes.DrawElement);
				if (this.IsBottomSceneWallVisible())
				{
					position2 = new RectangleF(position.Location, position.Size);
					position2.Height = this.areaSceneWallWidth.Height;
					position2.Y = position.Bottom - this.areaSceneWallWidth.Height;
					position2.Width -= this.areaSceneWallWidth.Width;
					if (this.IsSideSceneWallOnLeft())
					{
						position2.X += this.areaSceneWallWidth.Width;
					}
					positionZ = 0f;
					graph.Fill3DRectangle(position2, 0f, this.areaSceneDepth, this.matrix3D, chartArea.Area3DStyle.Light, color, chartArea.BackHatchStyle, chartArea.BackImage, chartArea.BackImageMode, chartArea.BackImageTransparentColor, chartArea.BackImageAlign, chartArea.BackGradientType, chartArea.BackGradientEndColor, chartArea.BorderColor, chartArea.BorderWidth, chartArea.BorderStyle, PenAlignment.Outset, DrawingOperationTypes.DrawElement);
				}
			}
		}

		internal bool IsBottomSceneWallVisible()
		{
			return this.Area3DStyle.XAngle >= 0;
		}

		internal bool IsMainSceneWallOnFront()
		{
			return false;
		}

		internal bool IsSideSceneWallOnLeft()
		{
			return this.Area3DStyle.YAngle > 0;
		}

		public float GetSeriesZPosition(Series series)
		{
			float num = default(float);
			float num2 = default(float);
			this.GetSeriesZPositionAndDepth(series, out num, out num2);
			return (float)((num2 + num / 2.0) / this.areaSceneDepth * 100.0);
		}

		public float GetSeriesDepth(Series series)
		{
			float num = default(float);
			float num2 = default(float);
			this.GetSeriesZPositionAndDepth(series, out num, out num2);
			return (float)(num / this.areaSceneDepth * 100.0);
		}

		private float GetArea3DSceneDepth()
		{
			bool flag = base.IndexedSeries((string[])base.series.ToArray(typeof(string)));
			Series series = null;
			if (base.series.Count > 0)
			{
				series = base.Common.DataManager.Series[(string)base.series[0]];
			}
			Axis axis = ((ChartArea)this).AxisX;
			if (base.series.Count > 0)
			{
				Series series2 = base.Common.DataManager.Series[base.series[0]];
				if (series2 != null && series2.XAxisType == AxisType.Secondary)
				{
					axis = ((ChartArea)this).AxisX2;
				}
			}
			double num = 1.0;
			if (!flag)
			{
				bool flag2 = default(bool);
				num = base.GetPointsInterval(base.series, axis.Logarithmic, axis.logarithmBase, false, out flag2, out series);
			}
			bool flag3 = false;
			if (series != null)
			{
				flag3 = base.Common.ChartTypeRegistry.GetChartType(series.ChartTypeName).SideBySideSeries;
				foreach (string item in base.series)
				{
					if (base.Common.DataManager.Series[item].IsAttributeSet("DrawSideBySide"))
					{
						string strA = ((DataPointAttributes)base.Common.DataManager.Series[item])["DrawSideBySide"];
						if (string.Compare(strA, "False", StringComparison.OrdinalIgnoreCase) == 0)
						{
							flag3 = false;
						}
						else if (string.Compare(strA, "True", StringComparison.OrdinalIgnoreCase) == 0)
						{
							flag3 = true;
						}
						else if (string.Compare(strA, "Auto", StringComparison.OrdinalIgnoreCase) != 0)
						{
							throw new InvalidOperationException(SR.ExceptionAttributeDrawSideBySideInvalid);
						}
					}
				}
			}
			Axis axis2 = ((ChartArea)this).AxisX;
			if (series != null && series.XAxisType == AxisType.Secondary)
			{
				axis2 = ((ChartArea)this).AxisX2;
			}
			double num2 = 0.8;
			int num3 = 1;
			if (series != null && this.Area3DStyle.Clustered && flag3)
			{
				num3 = 0;
				foreach (string item2 in base.series)
				{
					Series series3 = base.Common.DataManager.Series[item2];
					if (string.Compare(series3.ChartTypeName, series.ChartTypeName, StringComparison.OrdinalIgnoreCase) == 0)
					{
						num3++;
					}
				}
			}
			if (series != null && this.Area3DStyle.Clustered && base.Common.ChartTypeRegistry.GetChartType(series.ChartTypeName).SupportStackedGroups)
			{
				string empty = string.Empty;
				if (series.IsAttributeSet("StackedGroupName"))
				{
					string text2 = ((DataPointAttributes)series)["StackedGroupName"];
				}
				num3 = 0;
				ArrayList arrayList = new ArrayList();
				foreach (string item3 in base.series)
				{
					Series series4 = base.Common.DataManager.Series[item3];
					if (string.Compare(series4.ChartTypeName, series.ChartTypeName, StringComparison.OrdinalIgnoreCase) == 0)
					{
						string text = string.Empty;
						if (series4.IsAttributeSet("StackedGroupName"))
						{
							text = ((DataPointAttributes)series4)["StackedGroupName"];
						}
						if (!arrayList.Contains(text))
						{
							arrayList.Add(text);
						}
					}
				}
				num3 = arrayList.Count;
			}
			this.pointsDepth = num * num2 * (double)this.Area3DStyle.PointDepth / 100.0;
			this.pointsDepth = (double)axis2.GetPixelInterval(this.pointsDepth);
			if (series != null)
			{
				this.pointsDepth = series.GetPointWidth(base.Common.graph, axis2, num, 0.8) / (double)num3;
				this.pointsDepth *= (double)this.Area3DStyle.PointDepth / 100.0;
			}
			this.pointsGapDepth = this.pointsDepth * 0.8 * (double)this.Area3DStyle.PointGapDepth / 100.0;
			if (series != null)
			{
				series.GetPointDepthAndGap(base.Common.graph, axis2, ref this.pointsDepth, ref this.pointsGapDepth);
			}
			return (float)((this.pointsGapDepth + this.pointsDepth) * (double)this.GetNumberOfClusters());
		}

		internal void GetSeriesZPositionAndDepth(Series series, out float depth, out float positionZ)
		{
			int seriesClusterIndex = this.GetSeriesClusterIndex(series);
			depth = (float)this.pointsDepth;
			positionZ = (float)(this.pointsGapDepth / 2.0 + (this.pointsDepth + this.pointsGapDepth) * (double)seriesClusterIndex);
		}

		internal int GetNumberOfClusters()
		{
			if (this.seriesClusters == null)
			{
				ArrayList arrayList = new ArrayList();
				ArrayList arrayList2 = new ArrayList();
				this.seriesClusters = new ArrayList();
				int num = -1;
				foreach (string item in base.series)
				{
					Series series = base.Common.DataManager.Series[item];
					if (!this.Area3DStyle.Clustered && base.Common.ChartTypeRegistry.GetChartType(series.ChartTypeName).SupportStackedGroups)
					{
						string seriesStackGroupName = StackedColumnChart.GetSeriesStackGroupName(series);
						if (arrayList2.Contains(seriesStackGroupName))
						{
							bool flag = false;
							int num2 = 0;
							while (!flag && num2 < this.seriesClusters.Count)
							{
								foreach (string item2 in (ArrayList)this.seriesClusters[num2])
								{
									Series series2 = base.Common.DataManager.Series[item2];
									if (seriesStackGroupName == StackedColumnChart.GetSeriesStackGroupName(series2))
									{
										num = num2;
										flag = true;
									}
								}
								num2++;
							}
						}
						else
						{
							num = this.seriesClusters.Count;
							arrayList2.Add(seriesStackGroupName);
						}
					}
					else if (base.Common.ChartTypeRegistry.GetChartType(series.ChartTypeName).Stacked || (this.Area3DStyle.Clustered && base.Common.ChartTypeRegistry.GetChartType(series.ChartTypeName).SideBySideSeries))
					{
						if (arrayList.Contains(series.ChartTypeName.ToUpper(CultureInfo.InvariantCulture)))
						{
							bool flag2 = false;
							int num3 = 0;
							while (!flag2 && num3 < this.seriesClusters.Count)
							{
								foreach (string item3 in (ArrayList)this.seriesClusters[num3])
								{
									Series series3 = base.Common.DataManager.Series[item3];
									if (series3.ChartTypeName.ToUpper(CultureInfo.InvariantCulture) == series.ChartTypeName.ToUpper(CultureInfo.InvariantCulture))
									{
										num = num3;
										flag2 = true;
									}
								}
								num3++;
							}
						}
						else
						{
							num = this.seriesClusters.Count;
							arrayList.Add(series.ChartTypeName.ToUpper(CultureInfo.InvariantCulture));
						}
					}
					else
					{
						num = this.seriesClusters.Count;
					}
					if (this.seriesClusters.Count <= num)
					{
						this.seriesClusters.Add(new ArrayList());
					}
					((ArrayList)this.seriesClusters[num]).Add(item);
				}
			}
			return this.seriesClusters.Count;
		}

		internal int GetSeriesClusterIndex(Series series)
		{
			if (this.seriesClusters == null)
			{
				this.GetNumberOfClusters();
			}
			for (int i = 0; i < this.seriesClusters.Count; i++)
			{
				ArrayList arrayList = (ArrayList)this.seriesClusters[i];
				foreach (string item in arrayList)
				{
					if (item == series.Name)
					{
						if (this.reverseSeriesOrder)
						{
							i = this.seriesClusters.Count - 1 - i;
						}
						return i;
					}
				}
			}
			return 0;
		}

		private float GetEstimatedSceneDepth()
		{
			ChartArea chartArea = (ChartArea)this;
			this.seriesClusters = null;
			ElementPosition innerPlotPosition = chartArea.InnerPlotPosition;
			chartArea.AxisX.PlotAreaPosition = chartArea.Position;
			chartArea.AxisY.PlotAreaPosition = chartArea.Position;
			chartArea.AxisX2.PlotAreaPosition = chartArea.Position;
			chartArea.AxisY2.PlotAreaPosition = chartArea.Position;
			float area3DSceneDepth = this.GetArea3DSceneDepth();
			chartArea.AxisX.PlotAreaPosition = innerPlotPosition;
			chartArea.AxisY.PlotAreaPosition = innerPlotPosition;
			chartArea.AxisX2.PlotAreaPosition = innerPlotPosition;
			chartArea.AxisY2.PlotAreaPosition = innerPlotPosition;
			return area3DSceneDepth;
		}

		internal void Estimate3DInterval(ChartGraphics graph)
		{
			ChartArea chartArea2 = (ChartArea)this;
			this.areaSceneWallWidth = graph.GetRelativeSize(new SizeF((float)this.Area3DStyle.WallWidth, (float)this.Area3DStyle.WallWidth));
			ChartArea chartArea = (ChartArea)this;
			this.areaSceneDepth = this.GetEstimatedSceneDepth();
			RectangleF innerPlotRectangle = chartArea.Position.ToRectangleF();
			if (base.PlotAreaPosition.Width == 0.0 && base.PlotAreaPosition.Height == 0.0 && !chartArea.InnerPlotPosition.Auto && !chartArea.Position.Auto && !chartArea.InnerPlotPosition.Auto)
			{
				innerPlotRectangle.X += (float)(chartArea.Position.Width / 100.0 * chartArea.InnerPlotPosition.X);
				innerPlotRectangle.Y += (float)(chartArea.Position.Height / 100.0 * chartArea.InnerPlotPosition.Y);
				innerPlotRectangle.Width = (float)(chartArea.Position.Width / 100.0 * chartArea.InnerPlotPosition.Width);
				innerPlotRectangle.Height = (float)(chartArea.Position.Height / 100.0 * chartArea.InnerPlotPosition.Height);
			}
			int realYAngle = this.GetRealYAngle();
			Matrix3D matrix3D = new Matrix3D();
			matrix3D.Initialize(innerPlotRectangle, this.areaSceneDepth, (float)this.Area3DStyle.XAngle, (float)realYAngle, (float)this.Area3DStyle.Perspective, this.Area3DStyle.RightAngleAxes);
			Point3D[] array = new Point3D[8];
			bool flag = default(bool);
			if (chartArea.switchValueAxes)
			{
				float marksZPosition = base.axisX.GetMarksZPosition(out flag);
				array[0] = new Point3D(innerPlotRectangle.X, innerPlotRectangle.Y, marksZPosition);
				array[1] = new Point3D(innerPlotRectangle.X, innerPlotRectangle.Bottom, marksZPosition);
				marksZPosition = base.axisY.GetMarksZPosition(out flag);
				array[2] = new Point3D(innerPlotRectangle.X, innerPlotRectangle.Bottom, marksZPosition);
				array[3] = new Point3D(innerPlotRectangle.Right, innerPlotRectangle.Bottom, marksZPosition);
				marksZPosition = base.axisX2.GetMarksZPosition(out flag);
				array[4] = new Point3D(innerPlotRectangle.X, innerPlotRectangle.Y, marksZPosition);
				array[5] = new Point3D(innerPlotRectangle.X, innerPlotRectangle.Bottom, marksZPosition);
				marksZPosition = base.axisY2.GetMarksZPosition(out flag);
				array[6] = new Point3D(innerPlotRectangle.X, innerPlotRectangle.Y, marksZPosition);
				array[7] = new Point3D(innerPlotRectangle.Right, innerPlotRectangle.Y, marksZPosition);
			}
			else
			{
				float marksZPosition = base.axisX.GetMarksZPosition(out flag);
				array[0] = new Point3D(innerPlotRectangle.X, innerPlotRectangle.Bottom, marksZPosition);
				array[1] = new Point3D(innerPlotRectangle.Right, innerPlotRectangle.Bottom, marksZPosition);
				marksZPosition = base.axisY.GetMarksZPosition(out flag);
				array[2] = new Point3D(innerPlotRectangle.X, innerPlotRectangle.Y, marksZPosition);
				array[3] = new Point3D(innerPlotRectangle.X, innerPlotRectangle.Bottom, marksZPosition);
				marksZPosition = base.axisX2.GetMarksZPosition(out flag);
				array[4] = new Point3D(innerPlotRectangle.X, innerPlotRectangle.Y, marksZPosition);
				array[5] = new Point3D(innerPlotRectangle.Right, innerPlotRectangle.Y, marksZPosition);
				marksZPosition = base.axisY2.GetMarksZPosition(out flag);
				array[6] = new Point3D(innerPlotRectangle.X, innerPlotRectangle.Y, marksZPosition);
				array[7] = new Point3D(innerPlotRectangle.X, innerPlotRectangle.Bottom, marksZPosition);
			}
			Axis[] axes = chartArea.Axes;
			foreach (Axis axis in axes)
			{
				axis.crossing = axis.tempCrossing;
			}
			matrix3D.TransformPoints(array);
			int num = 0;
			Axis[] axes2 = chartArea.Axes;
			foreach (Axis axis2 in axes2)
			{
				double num2 = Math.Sqrt((double)((array[num].X - array[num + 1].X) * (array[num].X - array[num + 1].X) + (array[num].Y - array[num + 1].Y) * (array[num].Y - array[num + 1].Y)));
				float num3 = 1f;
				if (!chartArea.switchValueAxes)
				{
					num3 = 0.5f;
				}
				if (axis2.Type == AxisName.X || axis2.Type == AxisName.X2)
				{
					if (chartArea.switchValueAxes)
					{
						axis2.interval3DCorrection = num2 / (double)innerPlotRectangle.Height;
					}
					else
					{
						axis2.interval3DCorrection = num2 / (double)innerPlotRectangle.Width;
					}
				}
				else if (chartArea.switchValueAxes)
				{
					axis2.interval3DCorrection = num2 / (double)innerPlotRectangle.Width;
				}
				else
				{
					axis2.interval3DCorrection = num2 / (double)innerPlotRectangle.Height * (double)num3;
				}
				if (axis2.interval3DCorrection < 0.15)
				{
					axis2.interval3DCorrection = 0.15;
				}
				if (axis2.interval3DCorrection > 0.8)
				{
					axis2.interval3DCorrection = 1.0;
				}
				num += 2;
			}
		}

		internal int GetRealYAngle()
		{
			int result = this.Area3DStyle.YAngle;
			if (this.reverseSeriesOrder && this.Area3DStyle.YAngle >= 0)
			{
				result = this.Area3DStyle.YAngle - 180;
			}
			if (this.reverseSeriesOrder && this.Area3DStyle.YAngle <= 0)
			{
				result = this.Area3DStyle.YAngle + 180;
			}
			return result;
		}

		internal bool ShouldDrawOnSurface(SurfaceNames surfaceName, bool backLayer, bool onEdge)
		{
			bool flag = (this.visibleSurfaces & surfaceName) == surfaceName;
			if (onEdge)
			{
				return backLayer;
			}
			return backLayer == !flag;
		}

		internal bool DrawPointsInReverseOrder()
		{
			return this.Area3DStyle.YAngle <= 0;
		}

		internal bool DrawPointsToCenter(ref COPCoordinates coord)
		{
			bool result = false;
			COPCoordinates cOPCoordinates = (COPCoordinates)0;
			if (this.Area3DStyle.Perspective != 0)
			{
				if ((coord & COPCoordinates.X) == COPCoordinates.X)
				{
					if ((this.visibleSurfaces & SurfaceNames.Left) == (SurfaceNames)0 && (this.visibleSurfaces & SurfaceNames.Right) == (SurfaceNames)0)
					{
						result = true;
					}
					cOPCoordinates |= COPCoordinates.X;
				}
				if ((coord & COPCoordinates.Y) == COPCoordinates.Y)
				{
					if ((this.visibleSurfaces & SurfaceNames.Top) == (SurfaceNames)0 && (this.visibleSurfaces & SurfaceNames.Bottom) == (SurfaceNames)0)
					{
						result = true;
					}
					cOPCoordinates |= COPCoordinates.Y;
				}
				if ((coord & COPCoordinates.Z) == COPCoordinates.Z)
				{
					if ((this.visibleSurfaces & SurfaceNames.Front) == (SurfaceNames)0 && (this.visibleSurfaces & SurfaceNames.Back) == (SurfaceNames)0)
					{
						result = true;
					}
					cOPCoordinates |= COPCoordinates.Z;
				}
			}
			return result;
		}

		internal bool DrawSeriesToCenter()
		{
			if (this.Area3DStyle.Perspective != 0 && (this.visibleSurfaces & SurfaceNames.Front) == (SurfaceNames)0 && (this.visibleSurfaces & SurfaceNames.Back) == (SurfaceNames)0)
			{
				return true;
			}
			return false;
		}

		protected void PaintChartSeries3D(ChartGraphics graph)
		{
			ChartArea area = (ChartArea)this;
			ArrayList arrayList = this.GetSeriesDrawingOrder(this.reverseSeriesOrder);
			foreach (object item in arrayList)
			{
				Series series = (Series)item;
				IChartType chartType = base.Common.ChartTypeRegistry.GetChartType(series.ChartTypeName);
				chartType.Paint(graph, base.Common, area, series);
			}
		}

		internal ArrayList GetClusterSeriesNames(string seriesName)
		{
			foreach (ArrayList seriesCluster in this.seriesClusters)
			{
				if (seriesCluster.Contains(seriesName))
				{
					return seriesCluster;
				}
			}
			return new ArrayList();
		}

		private ArrayList GetSeriesDrawingOrder(bool reverseSeriesOrder)
		{
			ArrayList arrayList = new ArrayList();
			foreach (ArrayList seriesCluster in this.seriesClusters)
			{
				if (seriesCluster.Count > 0)
				{
					Series value = base.Common.DataManager.Series[(string)seriesCluster[0]];
					arrayList.Add(value);
				}
			}
			if (reverseSeriesOrder)
			{
				arrayList.Reverse();
			}
			if (this.DrawSeriesToCenter() && this.matrix3D.IsInitialized())
			{
				Point3D point3D = new Point3D(float.NaN, float.NaN, float.NaN);
				point3D = this.GetCenterOfProjection(COPCoordinates.Z);
				if (!float.IsNaN(point3D.Z))
				{
					for (int i = 0; i < arrayList.Count; i++)
					{
						if (((Series)arrayList[i]).Points.Count != 0)
						{
							float num = default(float);
							float num2 = default(float);
							this.GetSeriesZPositionAndDepth((Series)arrayList[i], out num, out num2);
							if (num2 >= point3D.Z)
							{
								i--;
								if (i < 0)
								{
									i = 0;
								}
								arrayList.Reverse(i, arrayList.Count - i);
								break;
							}
						}
					}
				}
			}
			return arrayList;
		}

		private int GetNumberOfStackGroups(ArrayList seriesNamesList)
		{
			this.stackGroupNames = new ArrayList();
			foreach (object seriesNames in seriesNamesList)
			{
				int count = this.stackGroupNames.Count;
				Series series = base.Common.DataManager.Series[(string)seriesNames];
				string text = string.Empty;
				if (series.IsAttributeSet("StackedGroupName"))
				{
					text = ((DataPointAttributes)series)["StackedGroupName"];
				}
				if (!this.stackGroupNames.Contains(text))
				{
					this.stackGroupNames.Add(text);
				}
			}
			return this.stackGroupNames.Count;
		}

		internal int GetSeriesStackGroupIndex(Series series, ref string stackGroupName)
		{
			stackGroupName = string.Empty;
			if (this.stackGroupNames != null)
			{
				if (series.IsAttributeSet("StackedGroupName"))
				{
					stackGroupName = ((DataPointAttributes)series)["StackedGroupName"];
				}
				return this.stackGroupNames.IndexOf(stackGroupName);
			}
			return 0;
		}

		internal ArrayList GetDataPointDrawingOrder(ArrayList seriesNamesList, IChartType chartType, bool selection, COPCoordinates coord, IComparer comparer, int mainYValueIndex, bool sideBySide)
		{
			ChartArea chartArea = (ChartArea)this;
			ArrayList arrayList = new ArrayList();
			double num = 1.0;
			if (chartArea.Area3DStyle.Clustered && !chartType.Stacked && sideBySide)
			{
				num = (double)seriesNamesList.Count;
			}
			if (chartType.SupportStackedGroups)
			{
				int numberOfStackGroups = this.GetNumberOfStackGroups(seriesNamesList);
				if (this.Area3DStyle.Clustered && seriesNamesList.Count > 0)
				{
					num = (double)numberOfStackGroups;
				}
			}
			bool flag = chartArea.IndexedSeries((string[])seriesNamesList.ToArray(typeof(string)));
			int num2 = 0;
			foreach (object seriesNames in seriesNamesList)
			{
				Series series = base.Common.DataManager.Series[(string)seriesNames];
				if (chartType.SupportStackedGroups && this.stackGroupNames != null)
				{
					string empty = string.Empty;
					num2 = this.GetSeriesStackGroupIndex(series, ref empty);
					if (chartType is StackedColumnChart)
					{
						((StackedColumnChart)chartType).currentStackGroup = empty;
					}
					else if (chartType is StackedBarChart)
					{
						((StackedBarChart)chartType).currentStackGroup = empty;
					}
				}
				Axis axis = (series.YAxisType == AxisType.Primary) ? chartArea.AxisY : chartArea.AxisY2;
				Axis axis2 = (series.XAxisType == AxisType.Primary) ? chartArea.AxisX : chartArea.AxisX2;
				axis2.GetViewMinimum();
				axis2.GetViewMaximum();
				axis.GetViewMinimum();
				axis.GetViewMaximum();
				bool flag2 = true;
				double interval = 1.0;
				if (!flag)
				{
					interval = ((ChartAreaAxes)chartArea).GetPointsInterval(seriesNamesList, axis2.Logarithmic, axis2.logarithmBase, true, out flag2);
				}
				double num3 = series.GetPointWidth(chartArea.Common.graph, axis2, interval, 0.8) / num;
				float depth = default(float);
				float zPosition = default(float);
				this.GetSeriesZPositionAndDepth(series, out depth, out zPosition);
				int num4 = 0;
				foreach (DataPoint point in series.Points)
				{
					num4++;
					double xPosition;
					double position;
					if (flag)
					{
						xPosition = axis2.GetPosition((double)num4) - num3 * num / 2.0 + num3 / 2.0 + (double)num2 * num3;
						position = axis2.GetPosition((double)num4);
					}
					else if (flag2)
					{
						xPosition = axis2.GetPosition(point.XValue) - num3 * num / 2.0 + num3 / 2.0 + (double)num2 * num3;
						position = axis2.GetPosition(point.XValue);
					}
					else
					{
						xPosition = axis2.GetPosition(point.XValue);
						position = axis2.GetPosition(point.XValue);
					}
					DataPoint3D dataPoint3D = new DataPoint3D();
					dataPoint3D.indexedSeries = flag;
					dataPoint3D.dataPoint = point;
					dataPoint3D.index = num4;
					dataPoint3D.xPosition = xPosition;
					dataPoint3D.xCenterVal = position;
					dataPoint3D.width = series.GetPointWidth(chartArea.Common.graph, axis2, interval, 0.8) / num;
					dataPoint3D.depth = depth;
					dataPoint3D.zPosition = zPosition;
					double yValue = chartType.GetYValue(base.Common, chartArea, series, point, num4 - 1, mainYValueIndex);
					dataPoint3D.yPosition = axis.GetPosition(yValue);
					dataPoint3D.height = axis.GetPosition(yValue - chartType.GetYValue(base.Common, chartArea, series, point, num4 - 1, -1));
					arrayList.Add(dataPoint3D);
				}
				if (num > 1.0 && sideBySide)
				{
					num2++;
				}
			}
			if (comparer == null)
			{
				comparer = new PointsDrawingOrderComparer((ChartArea)this, selection, coord);
			}
			arrayList.Sort(comparer);
			return arrayList;
		}

		internal Point3D GetCenterOfProjection(COPCoordinates coord)
		{
			Point3D[] array = new Point3D[2]
			{
				new Point3D(base.PlotAreaPosition.X, base.PlotAreaPosition.Bottom(), 0f),
				new Point3D(base.PlotAreaPosition.Right(), base.PlotAreaPosition.Y, this.areaSceneDepth)
			};
			bool flag = default(bool);
			bool flag2 = default(bool);
			bool flag3 = default(bool);
			this.CheckSurfaceOrientation(coord, array[0], array[1], out flag, out flag2, out flag3);
			Point3D point3D = new Point3D((float)(flag ? double.NaN : 0.0), (float)(flag2 ? double.NaN : 0.0), (float)(flag3 ? double.NaN : 0.0));
			if ((coord & COPCoordinates.X) == COPCoordinates.X && !flag)
			{
				goto IL_00b3;
			}
			if ((coord & COPCoordinates.Y) == COPCoordinates.Y && !flag2)
			{
				goto IL_00b3;
			}
			if ((coord & COPCoordinates.Z) == COPCoordinates.Z && !flag3)
			{
				goto IL_00b3;
			}
			return point3D;
			IL_00b3:
			SizeF sizeF = new SizeF(0.5f, 0.5f);
			sizeF.Width = (float)(sizeF.Width * 100.0 / (float)(base.Common.Chart.Width - 1));
			sizeF.Height = (float)(sizeF.Height * 100.0 / (float)(base.Common.Chart.Height - 1));
			bool flag4 = false;
			while (!flag4)
			{
				Point3D point3D2 = new Point3D((float)((array[0].X + array[1].X) / 2.0), (float)((array[0].Y + array[1].Y) / 2.0), (float)((array[0].Z + array[1].Z) / 2.0));
				this.CheckSurfaceOrientation(coord, array[0], point3D2, out flag, out flag2, out flag3);
				array[(!flag) ? 1 : 0].X = point3D2.X;
				array[(!flag2) ? 1 : 0].Y = point3D2.Y;
				array[(!flag3) ? 1 : 0].Z = point3D2.Z;
				flag4 = true;
				if ((coord & COPCoordinates.X) == COPCoordinates.X && Math.Abs(array[1].X - array[0].X) >= sizeF.Width)
				{
					flag4 = false;
				}
				if ((coord & COPCoordinates.Y) == COPCoordinates.Y && Math.Abs(array[1].Y - array[0].Y) >= sizeF.Height)
				{
					flag4 = false;
				}
				if ((coord & COPCoordinates.Z) == COPCoordinates.Z && Math.Abs(array[1].Z - array[0].Z) >= sizeF.Width)
				{
					flag4 = false;
				}
			}
			if (!float.IsNaN(point3D.X))
			{
				point3D.X = (float)((array[0].X + array[1].X) / 2.0);
			}
			if (!float.IsNaN(point3D.Y))
			{
				point3D.Y = (float)((array[0].Y + array[1].Y) / 2.0);
			}
			if (!float.IsNaN(point3D.Z))
			{
				point3D.Z = (float)((array[0].Z + array[1].Z) / 2.0);
			}
			return point3D;
		}

		private void CheckSurfaceOrientation(COPCoordinates coord, Point3D point1, Point3D point2, out bool xSameOrientation, out bool ySameOrientation, out bool zSameOrientation)
		{
			Point3D[] array = new Point3D[3];
			xSameOrientation = true;
			ySameOrientation = true;
			zSameOrientation = true;
			if ((coord & COPCoordinates.X) == COPCoordinates.X)
			{
				array[0] = new Point3D(point1.X, base.PlotAreaPosition.Y, 0f);
				array[1] = new Point3D(point1.X, base.PlotAreaPosition.Bottom(), 0f);
				array[2] = new Point3D(point1.X, base.PlotAreaPosition.Bottom(), this.areaSceneDepth);
				this.matrix3D.TransformPoints(array);
				bool flag = ChartGraphics3D.IsSurfaceVisible(array[0], array[1], array[2]);
				array[0] = new Point3D(point2.X, base.PlotAreaPosition.Y, 0f);
				array[1] = new Point3D(point2.X, base.PlotAreaPosition.Bottom(), 0f);
				array[2] = new Point3D(point2.X, base.PlotAreaPosition.Bottom(), this.areaSceneDepth);
				this.matrix3D.TransformPoints(array);
				bool flag2 = ChartGraphics3D.IsSurfaceVisible(array[0], array[1], array[2]);
				xSameOrientation = (flag == flag2);
			}
			if ((coord & COPCoordinates.Y) == COPCoordinates.Y)
			{
				array[0] = new Point3D(base.PlotAreaPosition.X, point1.Y, this.areaSceneDepth);
				array[1] = new Point3D(base.PlotAreaPosition.X, point1.Y, 0f);
				array[2] = new Point3D(base.PlotAreaPosition.Right(), point1.Y, 0f);
				this.matrix3D.TransformPoints(array);
				bool flag = ChartGraphics3D.IsSurfaceVisible(array[0], array[1], array[2]);
				array[0] = new Point3D(base.PlotAreaPosition.X, point2.Y, this.areaSceneDepth);
				array[1] = new Point3D(base.PlotAreaPosition.X, point2.Y, 0f);
				array[2] = new Point3D(base.PlotAreaPosition.Right(), point2.Y, 0f);
				this.matrix3D.TransformPoints(array);
				bool flag2 = ChartGraphics3D.IsSurfaceVisible(array[0], array[1], array[2]);
				ySameOrientation = (flag == flag2);
			}
			if ((coord & COPCoordinates.Z) == COPCoordinates.Z)
			{
				array[0] = new Point3D(base.PlotAreaPosition.X, base.PlotAreaPosition.Y, point1.Z);
				array[1] = new Point3D(base.PlotAreaPosition.X, base.PlotAreaPosition.Bottom(), point1.Z);
				array[2] = new Point3D(base.PlotAreaPosition.Right(), base.PlotAreaPosition.Bottom(), point1.Z);
				this.matrix3D.TransformPoints(array);
				bool flag = ChartGraphics3D.IsSurfaceVisible(array[0], array[1], array[2]);
				array[0] = new Point3D(base.PlotAreaPosition.X, base.PlotAreaPosition.Y, point2.Z);
				array[1] = new Point3D(base.PlotAreaPosition.X, base.PlotAreaPosition.Bottom(), point2.Z);
				array[2] = new Point3D(base.PlotAreaPosition.Right(), base.PlotAreaPosition.Bottom(), point2.Z);
				this.matrix3D.TransformPoints(array);
				bool flag2 = ChartGraphics3D.IsSurfaceVisible(array[0], array[1], array[2]);
				zSameOrientation = (flag == flag2);
			}
		}
	}
}
