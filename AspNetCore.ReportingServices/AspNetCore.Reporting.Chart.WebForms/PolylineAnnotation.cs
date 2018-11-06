using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributePolylineAnnotation_PolylineAnnotation")]
	internal class PolylineAnnotation : Annotation
	{
		internal GraphicsPath path = new GraphicsPath();

		internal bool pathChanged;

		private AnnotationPathPointCollection pathPoints = new AnnotationPathPointCollection();

		internal bool isPolygon;

		internal bool freeDrawPlacement;

		private LineAnchorCap startCap;

		private LineAnchorCap endCap;

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(LineAnchorCap.None)]
		[SRDescription("DescriptionAttributeStartCap3")]
		public virtual LineAnchorCap StartCap
		{
			get
			{
				return this.startCap;
			}
			set
			{
				this.startCap = value;
				base.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(LineAnchorCap.None)]
		[SRDescription("DescriptionAttributeStartCap3")]
		public virtual LineAnchorCap EndCap
		{
			get
			{
				return this.endCap;
			}
			set
			{
				this.endCap = value;
				base.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeTextAnnotation_LineWidth")]
		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[DefaultValue(typeof(ContentAlignment), "MiddleCenter")]
		public override ContentAlignment Alignment
		{
			get
			{
				return base.Alignment;
			}
			set
			{
				base.Alignment = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeTextAnnotation_LineWidth")]
		public override Color TextColor
		{
			get
			{
				return base.TextColor;
			}
			set
			{
				base.TextColor = value;
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8pt")]
		[SRDescription("DescriptionAttributeTextAnnotation_LineWidth")]
		public override Font TextFont
		{
			get
			{
				return base.TextFont;
			}
			set
			{
				base.TextFont = value;
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeTextStyle5")]
		[DefaultValue(typeof(TextStyle), "Default")]
		public override TextStyle TextStyle
		{
			get
			{
				return base.TextStyle;
			}
			set
			{
				base.TextStyle = value;
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeTextAnnotation_LineWidth")]
		public override Color BackColor
		{
			get
			{
				return base.BackColor;
			}
			set
			{
				base.BackColor = value;
			}
		}

		[DefaultValue(ChartHatchStyle.None)]
		[SRCategory("CategoryAttributeAppearance")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeTextAnnotation_LineWidth")]
		[Browsable(false)]
		public override ChartHatchStyle BackHatchStyle
		{
			get
			{
				return base.BackHatchStyle;
			}
			set
			{
				base.BackHatchStyle = value;
			}
		}

		[Browsable(false)]
		[DefaultValue(GradientType.None)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeTextAnnotation_LineWidth")]
		[SRCategory("CategoryAttributeAppearance")]
		public override GradientType BackGradientType
		{
			get
			{
				return base.BackGradientType;
			}
			set
			{
				base.BackGradientType = value;
			}
		}

		[DefaultValue(typeof(Color), "")]
		[Browsable(false)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeTextAnnotation_LineWidth")]
		[SRCategory("CategoryAttributeAppearance")]
		public override Color BackGradientEndColor
		{
			get
			{
				return base.BackGradientEndColor;
			}
			set
			{
				base.BackGradientEndColor = value;
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeMisc")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeAnnotationType4")]
		[Bindable(true)]
		public override string AnnotationType
		{
			get
			{
				return "Polyline";
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(SelectionPointsStyle.Rectangle)]
		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[ParenthesizePropertyName(true)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeSelectionPointsStyle3")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal override SelectionPointsStyle SelectionPointsStyle
		{
			get
			{
				return SelectionPointsStyle.Rectangle;
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[SRDescription("DescriptionAttributeFreeDrawPlacement")]
		[DefaultValue(false)]
		public virtual bool FreeDrawPlacement
		{
			get
			{
				return this.freeDrawPlacement;
			}
			set
			{
				this.freeDrawPlacement = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributePath")]
		[Browsable(false)]
		[SRCategory("CategoryAttributePosition")]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[DefaultValue(null)]
		public virtual GraphicsPath Path
		{
			get
			{
				return this.path;
			}
			set
			{
				this.path = value;
				this.pathChanged = true;
			}
		}

		[SRCategory("CategoryAttributePosition")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRDescription("DescriptionAttributePathPoints")]
		public AnnotationPathPointCollection PathPoints
		{
			get
			{
				if (this.pathChanged || this.path.PointCount != this.pathPoints.Count)
				{
					this.pathPoints.annotation = null;
					this.pathPoints.Clear();
					PointF[] array = this.path.PathPoints;
					byte[] pathTypes = this.path.PathTypes;
					for (int i = 0; i < array.Length; i++)
					{
						this.pathPoints.Add(new AnnotationPathPoint(array[i].X, array[i].Y, pathTypes[i]));
					}
					this.pathPoints.annotation = this;
				}
				return this.pathPoints;
			}
		}

		public PolylineAnnotation()
		{
			this.pathPoints.annotation = this;
		}

		internal override void Paint(Chart chart, ChartGraphics graphics)
		{
			this.Chart = chart;
			if (this.path.PointCount != 0)
			{
				PointF empty = PointF.Empty;
				PointF empty2 = PointF.Empty;
				SizeF empty3 = SizeF.Empty;
				((Annotation)this).GetRelativePosition(out empty, out empty3, out empty2);
				PointF pointF = new PointF(empty.X + empty3.Width, empty.Y + empty3.Height);
				RectangleF rectangleF = new RectangleF(empty, new SizeF(pointF.X - empty.X, pointF.Y - empty.Y));
				RectangleF rectangleF2 = new RectangleF(rectangleF.Location, rectangleF.Size);
				if (rectangleF2.Width < 0.0)
				{
					rectangleF2.X = rectangleF2.Right;
					rectangleF2.Width = (float)(0.0 - rectangleF2.Width);
				}
				if (rectangleF2.Height < 0.0)
				{
					rectangleF2.Y = rectangleF2.Bottom;
					rectangleF2.Height = (float)(0.0 - rectangleF2.Height);
				}
				if (!float.IsNaN(rectangleF2.X) && !float.IsNaN(rectangleF2.Y) && !float.IsNaN(rectangleF2.Right) && !float.IsNaN(rectangleF2.Bottom))
				{
					RectangleF absoluteRectangle = graphics.GetAbsoluteRectangle(rectangleF2);
					float num = (float)(absoluteRectangle.Width / 100.0);
					float num2 = (float)(absoluteRectangle.Height / 100.0);
					PointF[] array = this.path.PathPoints;
					byte[] pathTypes = this.path.PathTypes;
					for (int i = 0; i < array.Length; i++)
					{
						array[i].X = absoluteRectangle.X + array[i].X * num;
						array[i].Y = absoluteRectangle.Y + array[i].Y * num2;
					}
					GraphicsPath graphicsPath = new GraphicsPath(array, pathTypes);
					bool flag = false;
					LineCap lineCap = LineCap.Flat;
					LineCap lineCap2 = LineCap.Flat;
					if (!this.isPolygon && (this.startCap != 0 || this.endCap != 0))
					{
						flag = true;
						lineCap = graphics.pen.StartCap;
						lineCap2 = graphics.pen.EndCap;
						if (this.startCap == LineAnchorCap.Arrow)
						{
							if (this.LineWidth < 4)
							{
								int num3 = 3 - this.LineWidth;
								graphics.pen.StartCap = LineCap.Custom;
								graphics.pen.CustomStartCap = new AdjustableArrowCap((float)(this.LineWidth + num3), (float)(this.LineWidth + num3), true);
							}
							else
							{
								graphics.pen.StartCap = LineCap.ArrowAnchor;
							}
						}
						else if (this.startCap == LineAnchorCap.Diamond)
						{
							graphics.pen.StartCap = LineCap.DiamondAnchor;
						}
						else if (this.startCap == LineAnchorCap.Round)
						{
							graphics.pen.StartCap = LineCap.RoundAnchor;
						}
						else if (this.startCap == LineAnchorCap.Square)
						{
							graphics.pen.StartCap = LineCap.SquareAnchor;
						}
						if (this.endCap == LineAnchorCap.Arrow)
						{
							if (this.LineWidth < 4)
							{
								int num4 = 3 - this.LineWidth;
								graphics.pen.EndCap = LineCap.Custom;
								graphics.pen.CustomEndCap = new AdjustableArrowCap((float)(this.LineWidth + num4), (float)(this.LineWidth + num4), true);
							}
							else
							{
								graphics.pen.EndCap = LineCap.ArrowAnchor;
							}
						}
						else if (this.endCap == LineAnchorCap.Diamond)
						{
							graphics.pen.EndCap = LineCap.DiamondAnchor;
						}
						else if (this.endCap == LineAnchorCap.Round)
						{
							graphics.pen.EndCap = LineCap.RoundAnchor;
						}
						else if (this.endCap == LineAnchorCap.Square)
						{
							graphics.pen.EndCap = LineCap.SquareAnchor;
						}
					}
					if (this.Chart.chartPicture.common.ProcessModePaint)
					{
						if (this.isPolygon)
						{
							graphicsPath.CloseAllFigures();
							graphics.DrawPathAbs(graphicsPath, this.BackColor, this.BackHatchStyle, string.Empty, ChartImageWrapMode.Scaled, Color.Empty, ChartImageAlign.Center, this.BackGradientType, this.BackGradientEndColor, this.LineColor, this.LineWidth, this.LineStyle, PenAlignment.Center, this.ShadowOffset, this.ShadowColor);
						}
						else
						{
							graphics.DrawPathAbs(graphicsPath, Color.Transparent, ChartHatchStyle.None, string.Empty, ChartImageWrapMode.Scaled, Color.Empty, ChartImageAlign.Center, GradientType.None, Color.Empty, this.LineColor, this.LineWidth, this.LineStyle, PenAlignment.Center, this.ShadowOffset, this.ShadowColor);
						}
					}
					if (this.Chart.chartPicture.common.ProcessModeRegions)
					{
						GraphicsPath graphicsPath2 = null;
						if (this.isPolygon)
						{
							graphicsPath2 = graphicsPath;
						}
						else
						{
							graphicsPath2 = new GraphicsPath();
							graphicsPath2.AddPath(graphicsPath, false);
							using (Pen pen = (Pen)graphics.pen.Clone())
							{
								pen.DashStyle = DashStyle.Solid;
								pen.Width += 2f;
								ChartGraphics.Widen(graphicsPath2, pen);
							}
						}
						this.Chart.chartPicture.common.HotRegionsList.AddHotRegion(graphics, graphicsPath2, false, base.ReplaceKeywords(this.ToolTip), base.ReplaceKeywords(this.Href), base.ReplaceKeywords(this.MapAreaAttributes), this, ChartElementType.Annotation);
					}
					if (flag)
					{
						graphics.pen.StartCap = lineCap;
						graphics.pen.EndCap = lineCap2;
					}
					this.PaintSelectionHandles(graphics, rectangleF2, graphicsPath);
				}
			}
		}

		internal override void AdjustLocationSize(SizeF movingDistance, ResizingMode resizeMode, bool pixelCoord, bool userInput)
		{
			if (resizeMode != ResizingMode.MovingPathPoints)
			{
				base.AdjustLocationSize(movingDistance, resizeMode, pixelCoord, userInput);
			}
			else
			{
				PointF empty = PointF.Empty;
				PointF empty2 = PointF.Empty;
				SizeF empty3 = SizeF.Empty;
				((Annotation)this).GetRelativePosition(out empty, out empty3, out empty2);
				if (userInput)
				{
					GraphicsPath startMovePathRel = base.startMovePathRel;
				}
				if (pixelCoord)
				{
					movingDistance = base.GetGraphics().GetRelativeSize(movingDistance);
				}
				movingDistance.Width /= (float)(base.startMovePositionRel.Width / 100.0);
				movingDistance.Height /= (float)(base.startMovePositionRel.Height / 100.0);
				if (this.path.PointCount > 0)
				{
					GraphicsPath graphicsPath = userInput ? base.startMovePathRel : this.path;
					PointF[] array = graphicsPath.PathPoints;
					byte[] pathTypes = graphicsPath.PathTypes;
					RectangleF empty4 = RectangleF.Empty;
					for (int i = 0; i < array.Length; i++)
					{
						if (base.currentPathPointIndex == i || base.currentPathPointIndex < 0 || base.currentPathPointIndex >= array.Length)
						{
							array[i].X -= movingDistance.Width;
							array[i].Y -= movingDistance.Height;
						}
					}
					if (userInput && this.AllowResizing)
					{
						this.path.Dispose();
						this.path = new GraphicsPath(array, pathTypes);
						RectangleF bounds = this.path.GetBounds();
						bounds.X *= (float)(base.startMovePositionRel.Width / 100.0);
						bounds.Y *= (float)(base.startMovePositionRel.Height / 100.0);
						bounds.X += base.startMovePositionRel.X;
						bounds.Y += base.startMovePositionRel.Y;
						bounds.Width *= (float)(base.startMovePositionRel.Width / 100.0);
						bounds.Height *= (float)(base.startMovePositionRel.Height / 100.0);
						base.SetPositionRelative(bounds, empty2);
						for (int j = 0; j < array.Length; j++)
						{
							array[j].X = (float)(base.startMovePositionRel.X + array[j].X * (base.startMovePositionRel.Width / 100.0));
							array[j].Y = (float)(base.startMovePositionRel.Y + array[j].Y * (base.startMovePositionRel.Height / 100.0));
							array[j].X = (float)((array[j].X - bounds.X) / (bounds.Width / 100.0));
							array[j].Y = (float)((array[j].Y - bounds.Y) / (bounds.Height / 100.0));
						}
					}
					base.positionChanged = true;
					this.path.Dispose();
					this.path = new GraphicsPath(array, pathTypes);
					this.pathChanged = true;
					base.Invalidate();
				}
			}
		}
	}
}
