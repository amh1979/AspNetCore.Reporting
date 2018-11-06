using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ScaleLabels : IROMStyleDefinitionContainer
	{
		private GaugePanel m_gaugePanel;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ScaleLabels m_defObject;

		private ScaleLabelsInstance m_instance;

		private Style m_style;

		private ReportDoubleProperty m_interval;

		private ReportDoubleProperty m_intervalOffset;

		private ReportBoolProperty m_allowUpsideDown;

		private ReportDoubleProperty m_distanceFromScale;

		private ReportDoubleProperty m_fontAngle;

		private ReportEnumProperty<GaugeLabelPlacements> m_placement;

		private ReportBoolProperty m_rotateLabels;

		private ReportBoolProperty m_showEndLabels;

		private ReportBoolProperty m_hidden;

		private ReportBoolProperty m_useFontPercent;

		public Style Style
		{
			get
			{
				if (this.m_style == null)
				{
					this.m_style = new Style(this.m_gaugePanel, this.m_gaugePanel, this.m_defObject, this.m_gaugePanel.RenderingContext);
				}
				return this.m_style;
			}
		}

		public ReportDoubleProperty Interval
		{
			get
			{
				if (this.m_interval == null && this.m_defObject.Interval != null)
				{
					this.m_interval = new ReportDoubleProperty(this.m_defObject.Interval);
				}
				return this.m_interval;
			}
		}

		public ReportDoubleProperty IntervalOffset
		{
			get
			{
				if (this.m_intervalOffset == null && this.m_defObject.IntervalOffset != null)
				{
					this.m_intervalOffset = new ReportDoubleProperty(this.m_defObject.IntervalOffset);
				}
				return this.m_intervalOffset;
			}
		}

		public ReportBoolProperty AllowUpsideDown
		{
			get
			{
				if (this.m_allowUpsideDown == null && this.m_defObject.AllowUpsideDown != null)
				{
					this.m_allowUpsideDown = new ReportBoolProperty(this.m_defObject.AllowUpsideDown);
				}
				return this.m_allowUpsideDown;
			}
		}

		public ReportDoubleProperty DistanceFromScale
		{
			get
			{
				if (this.m_distanceFromScale == null && this.m_defObject.DistanceFromScale != null)
				{
					this.m_distanceFromScale = new ReportDoubleProperty(this.m_defObject.DistanceFromScale);
				}
				return this.m_distanceFromScale;
			}
		}

		public ReportDoubleProperty FontAngle
		{
			get
			{
				if (this.m_fontAngle == null && this.m_defObject.FontAngle != null)
				{
					this.m_fontAngle = new ReportDoubleProperty(this.m_defObject.FontAngle);
				}
				return this.m_fontAngle;
			}
		}

		public ReportEnumProperty<GaugeLabelPlacements> Placement
		{
			get
			{
				if (this.m_placement == null && this.m_defObject.Placement != null)
				{
					this.m_placement = new ReportEnumProperty<GaugeLabelPlacements>(this.m_defObject.Placement.IsExpression, this.m_defObject.Placement.OriginalText, EnumTranslator.TranslateGaugeLabelPlacements(this.m_defObject.Placement.StringValue, null));
				}
				return this.m_placement;
			}
		}

		public ReportBoolProperty RotateLabels
		{
			get
			{
				if (this.m_rotateLabels == null && this.m_defObject.RotateLabels != null)
				{
					this.m_rotateLabels = new ReportBoolProperty(this.m_defObject.RotateLabels);
				}
				return this.m_rotateLabels;
			}
		}

		public ReportBoolProperty ShowEndLabels
		{
			get
			{
				if (this.m_showEndLabels == null && this.m_defObject.ShowEndLabels != null)
				{
					this.m_showEndLabels = new ReportBoolProperty(this.m_defObject.ShowEndLabels);
				}
				return this.m_showEndLabels;
			}
		}

		public ReportBoolProperty Hidden
		{
			get
			{
				if (this.m_hidden == null && this.m_defObject.Hidden != null)
				{
					this.m_hidden = new ReportBoolProperty(this.m_defObject.Hidden);
				}
				return this.m_hidden;
			}
		}

		public ReportBoolProperty UseFontPercent
		{
			get
			{
				if (this.m_useFontPercent == null && this.m_defObject.UseFontPercent != null)
				{
					this.m_useFontPercent = new ReportBoolProperty(this.m_defObject.UseFontPercent);
				}
				return this.m_useFontPercent;
			}
		}

		internal GaugePanel GaugePanelDef
		{
			get
			{
				return this.m_gaugePanel;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.ScaleLabels ScaleLabelsDef
		{
			get
			{
				return this.m_defObject;
			}
		}

		public ScaleLabelsInstance Instance
		{
			get
			{
				if (this.m_gaugePanel.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					this.m_instance = new ScaleLabelsInstance(this);
				}
				return this.m_instance;
			}
		}

		internal ScaleLabels(AspNetCore.ReportingServices.ReportIntermediateFormat.ScaleLabels defObject, GaugePanel gaugePanel)
		{
			this.m_defObject = defObject;
			this.m_gaugePanel = gaugePanel;
		}

		internal void SetNewContext()
		{
			if (this.m_instance != null)
			{
				this.m_instance.SetNewContext();
			}
			if (this.m_style != null)
			{
				this.m_style.SetNewContext();
			}
		}
	}
}
