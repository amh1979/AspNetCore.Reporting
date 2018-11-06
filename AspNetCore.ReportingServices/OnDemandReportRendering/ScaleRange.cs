using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;
using System.Drawing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ScaleRange : GaugePanelObjectCollectionItem, IROMStyleDefinitionContainer, IROMActionOwner
	{
		private GaugePanel m_gaugePanel;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ScaleRange m_defObject;

		private Style m_style;

		private ActionInfo m_actionInfo;

		private ReportDoubleProperty m_distanceFromScale;

		private GaugeInputValue m_startValue;

		private GaugeInputValue m_endValue;

		private ReportDoubleProperty m_startWidth;

		private ReportDoubleProperty m_endWidth;

		private ReportColorProperty m_inRangeBarPointerColor;

		private ReportColorProperty m_inRangeLabelColor;

		private ReportColorProperty m_inRangeTickMarksColor;

		private ReportEnumProperty<ScaleRangePlacements> m_placement;

		private ReportStringProperty m_toolTip;

		private ReportBoolProperty m_hidden;

		private ReportEnumProperty<BackgroundGradientTypes> m_backgroundGradientType;

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

		public GaugeInputValue StartValue
		{
			get
			{
				if (this.m_startValue == null && this.m_defObject.StartValue != null)
				{
					this.m_startValue = new GaugeInputValue(this.m_defObject.StartValue, this.m_gaugePanel);
				}
				return this.m_startValue;
			}
		}

		public GaugeInputValue EndValue
		{
			get
			{
				if (this.m_endValue == null && this.m_defObject.EndValue != null)
				{
					this.m_endValue = new GaugeInputValue(this.m_defObject.EndValue, this.m_gaugePanel);
				}
				return this.m_endValue;
			}
		}

		public ReportDoubleProperty StartWidth
		{
			get
			{
				if (this.m_startWidth == null && this.m_defObject.StartWidth != null)
				{
					this.m_startWidth = new ReportDoubleProperty(this.m_defObject.StartWidth);
				}
				return this.m_startWidth;
			}
		}

		public ReportDoubleProperty EndWidth
		{
			get
			{
				if (this.m_endWidth == null && this.m_defObject.EndWidth != null)
				{
					this.m_endWidth = new ReportDoubleProperty(this.m_defObject.EndWidth);
				}
				return this.m_endWidth;
			}
		}

		public ReportColorProperty InRangeBarPointerColor
		{
			get
			{
				if (this.m_inRangeBarPointerColor == null && this.m_defObject.InRangeBarPointerColor != null)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo inRangeBarPointerColor = this.m_defObject.InRangeBarPointerColor;
					if (inRangeBarPointerColor != null)
					{
						this.m_inRangeBarPointerColor = new ReportColorProperty(inRangeBarPointerColor.IsExpression, inRangeBarPointerColor.OriginalText, inRangeBarPointerColor.IsExpression ? null : new ReportColor(inRangeBarPointerColor.StringValue.Trim(), true), inRangeBarPointerColor.IsExpression ? new ReportColor("", Color.Empty, true) : null);
					}
				}
				return this.m_inRangeBarPointerColor;
			}
		}

		public ReportColorProperty InRangeLabelColor
		{
			get
			{
				if (this.m_inRangeLabelColor == null && this.m_defObject.InRangeLabelColor != null)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo inRangeLabelColor = this.m_defObject.InRangeLabelColor;
					if (inRangeLabelColor != null)
					{
						this.m_inRangeLabelColor = new ReportColorProperty(inRangeLabelColor.IsExpression, inRangeLabelColor.OriginalText, inRangeLabelColor.IsExpression ? null : new ReportColor(inRangeLabelColor.StringValue.Trim(), true), inRangeLabelColor.IsExpression ? new ReportColor("", Color.Empty, true) : null);
					}
				}
				return this.m_inRangeLabelColor;
			}
		}

		public ReportColorProperty InRangeTickMarksColor
		{
			get
			{
				if (this.m_inRangeTickMarksColor == null && this.m_defObject.InRangeTickMarksColor != null)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo inRangeTickMarksColor = this.m_defObject.InRangeTickMarksColor;
					if (inRangeTickMarksColor != null)
					{
						this.m_inRangeTickMarksColor = new ReportColorProperty(inRangeTickMarksColor.IsExpression, inRangeTickMarksColor.OriginalText, inRangeTickMarksColor.IsExpression ? null : new ReportColor(inRangeTickMarksColor.StringValue.Trim(), true), inRangeTickMarksColor.IsExpression ? new ReportColor("", Color.Empty, true) : null);
					}
				}
				return this.m_inRangeTickMarksColor;
			}
		}

		public ReportEnumProperty<BackgroundGradientTypes> BackgroundGradientType
		{
			get
			{
				if (this.m_backgroundGradientType == null && this.m_defObject.BackgroundGradientType != null)
				{
					this.m_backgroundGradientType = new ReportEnumProperty<BackgroundGradientTypes>(this.m_defObject.BackgroundGradientType.IsExpression, this.m_defObject.BackgroundGradientType.OriginalText, EnumTranslator.TranslateBackgroundGradientTypes(this.m_defObject.BackgroundGradientType.StringValue, null));
				}
				return this.m_backgroundGradientType;
			}
		}

		public ReportEnumProperty<ScaleRangePlacements> Placement
		{
			get
			{
				if (this.m_placement == null && this.m_defObject.Placement != null)
				{
					this.m_placement = new ReportEnumProperty<ScaleRangePlacements>(this.m_defObject.Placement.IsExpression, this.m_defObject.Placement.OriginalText, EnumTranslator.TranslateScaleRangePlacements(this.m_defObject.Placement.StringValue, null));
				}
				return this.m_placement;
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

		internal GaugePanel GaugePanelDef
		{
			get
			{
				return this.m_gaugePanel;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.ScaleRange ScaleRangeDef
		{
			get
			{
				return this.m_defObject;
			}
		}

		public ScaleRangeInstance Instance
		{
			get
			{
				if (this.m_gaugePanel.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (base.m_instance == null)
				{
					base.m_instance = new ScaleRangeInstance(this);
				}
				return (ScaleRangeInstance)base.m_instance;
			}
		}

		internal ScaleRange(AspNetCore.ReportingServices.ReportIntermediateFormat.ScaleRange defObject, GaugePanel gaugePanel)
		{
			this.m_defObject = defObject;
			this.m_gaugePanel = gaugePanel;
		}

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
			if (this.m_startValue != null)
			{
				this.m_startValue.SetNewContext();
			}
			if (this.m_endValue != null)
			{
				this.m_endValue.SetNewContext();
			}
		}
	}
}
