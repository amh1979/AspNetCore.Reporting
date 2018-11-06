using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class GaugeScale : GaugePanelObjectCollectionItem, IROMStyleDefinitionContainer, IROMActionOwner
	{
		internal GaugePanel m_gaugePanel;

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeScale m_defObject;

		private Style m_style;

		private ActionInfo m_actionInfo;

		private ScaleRangeCollection m_scaleRanges;

		private CustomLabelCollection m_customLabels;

		private ReportDoubleProperty m_interval;

		private ReportDoubleProperty m_intervalOffset;

		private ReportBoolProperty m_logarithmic;

		private ReportDoubleProperty m_logarithmicBase;

		private GaugeInputValue m_maximumValue;

		private GaugeInputValue m_minimumValue;

		private ReportDoubleProperty m_multiplier;

		private ReportBoolProperty m_reversed;

		private GaugeTickMarks m_gaugeMajorTickMarks;

		private GaugeTickMarks m_gaugeMinorTickMarks;

		private ScalePin m_maximumPin;

		private ScalePin m_minimumPin;

		private ScaleLabels m_scaleLabels;

		private ReportBoolProperty m_tickMarksOnTop;

		private ReportStringProperty m_toolTip;

		private ReportBoolProperty m_hidden;

		private ReportDoubleProperty m_width;

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

		public string UniqueName
		{
			get
			{
				return this.m_gaugePanel.GaugePanelDef.UniqueName + 'x' + this.m_defObject.ID;
			}
		}

		public ActionInfo ActionInfo
		{
			get
			{
				if (this.m_actionInfo == null && this.m_defObject.Action != null)
				{
					this.m_actionInfo = new ActionInfo(this.m_gaugePanel.RenderingContext, this.m_gaugePanel, this.m_defObject.Action, this.m_gaugePanel.GaugePanelDef, this.m_gaugePanel, ObjectType.GaugePanel, this.m_gaugePanel.Name, this);
				}
				return this.m_actionInfo;
			}
		}

		public List<string> FieldsUsedInValueExpression
		{
			get
			{
				return null;
			}
		}

		public string Name
		{
			get
			{
				return this.m_defObject.Name;
			}
		}

		public ScaleRangeCollection ScaleRanges
		{
			get
			{
				if (this.m_scaleRanges == null && this.m_defObject.ScaleRanges != null)
				{
					this.m_scaleRanges = new ScaleRangeCollection(this, this.m_gaugePanel);
				}
				return this.m_scaleRanges;
			}
		}

		public CustomLabelCollection CustomLabels
		{
			get
			{
				if (this.m_customLabels == null && this.m_defObject.CustomLabels != null)
				{
					this.m_customLabels = new CustomLabelCollection(this, this.m_gaugePanel);
				}
				return this.m_customLabels;
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

		public ReportBoolProperty Logarithmic
		{
			get
			{
				if (this.m_logarithmic == null && this.m_defObject.Logarithmic != null)
				{
					this.m_logarithmic = new ReportBoolProperty(this.m_defObject.Logarithmic);
				}
				return this.m_logarithmic;
			}
		}

		public ReportDoubleProperty LogarithmicBase
		{
			get
			{
				if (this.m_logarithmicBase == null && this.m_defObject.LogarithmicBase != null)
				{
					this.m_logarithmicBase = new ReportDoubleProperty(this.m_defObject.LogarithmicBase);
				}
				return this.m_logarithmicBase;
			}
		}

		public GaugeInputValue MaximumValue
		{
			get
			{
				if (this.m_maximumValue == null && this.m_defObject.MaximumValue != null)
				{
					this.m_maximumValue = new GaugeInputValue(this.m_defObject.MaximumValue, this.m_gaugePanel);
				}
				return this.m_maximumValue;
			}
		}

		public GaugeInputValue MinimumValue
		{
			get
			{
				if (this.m_minimumValue == null && this.m_defObject.MinimumValue != null)
				{
					this.m_minimumValue = new GaugeInputValue(this.m_defObject.MinimumValue, this.m_gaugePanel);
				}
				return this.m_minimumValue;
			}
		}

		public ReportDoubleProperty Multiplier
		{
			get
			{
				if (this.m_multiplier == null && this.m_defObject.Multiplier != null)
				{
					this.m_multiplier = new ReportDoubleProperty(this.m_defObject.Multiplier);
				}
				return this.m_multiplier;
			}
		}

		public ReportBoolProperty Reversed
		{
			get
			{
				if (this.m_reversed == null && this.m_defObject.Reversed != null)
				{
					this.m_reversed = new ReportBoolProperty(this.m_defObject.Reversed);
				}
				return this.m_reversed;
			}
		}

		public GaugeTickMarks GaugeMajorTickMarks
		{
			get
			{
				if (this.m_gaugeMajorTickMarks == null && this.m_defObject.GaugeMajorTickMarks != null)
				{
					this.m_gaugeMajorTickMarks = new GaugeTickMarks(this.m_defObject.GaugeMajorTickMarks, this.m_gaugePanel);
				}
				return this.m_gaugeMajorTickMarks;
			}
		}

		public GaugeTickMarks GaugeMinorTickMarks
		{
			get
			{
				if (this.m_gaugeMinorTickMarks == null && this.m_defObject.GaugeMinorTickMarks != null)
				{
					this.m_gaugeMinorTickMarks = new GaugeTickMarks(this.m_defObject.GaugeMinorTickMarks, this.m_gaugePanel);
				}
				return this.m_gaugeMinorTickMarks;
			}
		}

		public ScalePin MaximumPin
		{
			get
			{
				if (this.m_maximumPin == null && this.m_defObject.MaximumPin != null)
				{
					this.m_maximumPin = new ScalePin(this.m_defObject.MaximumPin, this.m_gaugePanel);
				}
				return this.m_maximumPin;
			}
		}

		public ScalePin MinimumPin
		{
			get
			{
				if (this.m_minimumPin == null && this.m_defObject.MinimumPin != null)
				{
					this.m_minimumPin = new ScalePin(this.m_defObject.MinimumPin, this.m_gaugePanel);
				}
				return this.m_minimumPin;
			}
		}

		public ScaleLabels ScaleLabels
		{
			get
			{
				if (this.m_scaleLabels == null && this.m_defObject.ScaleLabels != null)
				{
					this.m_scaleLabels = new ScaleLabels(this.m_defObject.ScaleLabels, this.m_gaugePanel);
				}
				return this.m_scaleLabels;
			}
		}

		public ReportBoolProperty TickMarksOnTop
		{
			get
			{
				if (this.m_tickMarksOnTop == null && this.m_defObject.TickMarksOnTop != null)
				{
					this.m_tickMarksOnTop = new ReportBoolProperty(this.m_defObject.TickMarksOnTop);
				}
				return this.m_tickMarksOnTop;
			}
		}

		public ReportStringProperty ToolTip
		{
			get
			{
				if (this.m_toolTip == null && this.m_defObject.ToolTip != null)
				{
					this.m_toolTip = new ReportStringProperty(this.m_defObject.ToolTip);
				}
				return this.m_toolTip;
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

		public ReportDoubleProperty Width
		{
			get
			{
				if (this.m_width == null && this.m_defObject.Width != null)
				{
					this.m_width = new ReportDoubleProperty(this.m_defObject.Width);
				}
				return this.m_width;
			}
		}

		internal GaugePanel GaugePanelDef
		{
			get
			{
				return this.m_gaugePanel;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeScale GaugeScaleDef
		{
			get
			{
				return this.m_defObject;
			}
		}

		public GaugeScaleInstance Instance
		{
			get
			{
				return this.GetInstance();
			}
		}

		internal GaugeScale(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeScale defObject, GaugePanel gaugePanel)
		{
			this.m_defObject = defObject;
			this.m_gaugePanel = gaugePanel;
		}

		internal abstract GaugeScaleInstance GetInstance();

		internal override void SetNewContext()
		{
			if (base.m_instance != null)
			{
				base.m_instance.SetNewContext();
			}
			if (this.m_style != null)
			{
				this.m_style.SetNewContext();
			}
			if (this.m_actionInfo != null)
			{
				this.m_actionInfo.SetNewContext();
			}
			if (this.m_scaleRanges != null)
			{
				this.m_scaleRanges.SetNewContext();
			}
			if (this.m_customLabels != null)
			{
				this.m_customLabels.SetNewContext();
			}
			if (this.m_maximumValue != null)
			{
				this.m_maximumValue.SetNewContext();
			}
			if (this.m_minimumValue != null)
			{
				this.m_minimumValue.SetNewContext();
			}
			if (this.m_gaugeMajorTickMarks != null)
			{
				this.m_gaugeMajorTickMarks.SetNewContext();
			}
			if (this.m_gaugeMinorTickMarks != null)
			{
				this.m_gaugeMinorTickMarks.SetNewContext();
			}
			if (this.m_maximumPin != null)
			{
				this.m_maximumPin.SetNewContext();
			}
			if (this.m_minimumPin != null)
			{
				this.m_minimumPin.SetNewContext();
			}
			if (this.m_scaleLabels != null)
			{
				this.m_scaleLabels.SetNewContext();
			}
		}
	}
}
