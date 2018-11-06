using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System.Drawing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapColorScale : MapDockableSubItem
	{
		private MapColorScaleTitle m_mapColorScaleTitle;

		private ReportSizeProperty m_tickMarkLength;

		private ReportColorProperty m_colorBarBorderColor;

		private ReportIntProperty m_labelInterval;

		private ReportStringProperty m_labelFormat;

		private ReportEnumProperty<MapLabelPlacement> m_labelPlacement;

		private ReportEnumProperty<MapLabelBehavior> m_labelBehavior;

		private ReportBoolProperty m_hideEndLabels;

		private ReportColorProperty m_rangeGapColor;

		private ReportStringProperty m_noDataText;

		public MapColorScaleTitle MapColorScaleTitle
		{
			get
			{
				if (this.m_mapColorScaleTitle == null && this.MapColorScaleDef.MapColorScaleTitle != null)
				{
					this.m_mapColorScaleTitle = new MapColorScaleTitle(this.MapColorScaleDef.MapColorScaleTitle, base.m_map);
				}
				return this.m_mapColorScaleTitle;
			}
		}

		public ReportSizeProperty TickMarkLength
		{
			get
			{
				if (this.m_tickMarkLength == null && this.MapColorScaleDef.TickMarkLength != null)
				{
					this.m_tickMarkLength = new ReportSizeProperty(this.MapColorScaleDef.TickMarkLength);
				}
				return this.m_tickMarkLength;
			}
		}

		public ReportColorProperty ColorBarBorderColor
		{
			get
			{
				if (this.m_colorBarBorderColor == null && this.MapColorScaleDef.ColorBarBorderColor != null)
				{
					ExpressionInfo colorBarBorderColor = this.MapColorScaleDef.ColorBarBorderColor;
					if (colorBarBorderColor != null)
					{
						this.m_colorBarBorderColor = new ReportColorProperty(colorBarBorderColor.IsExpression, this.MapColorScaleDef.ColorBarBorderColor.OriginalText, colorBarBorderColor.IsExpression ? null : new ReportColor(colorBarBorderColor.StringValue.Trim(), true), colorBarBorderColor.IsExpression ? new ReportColor("", Color.Empty, true) : null);
					}
				}
				return this.m_colorBarBorderColor;
			}
		}

		public ReportIntProperty LabelInterval
		{
			get
			{
				if (this.m_labelInterval == null && this.MapColorScaleDef.LabelInterval != null)
				{
					this.m_labelInterval = new ReportIntProperty(this.MapColorScaleDef.LabelInterval.IsExpression, this.MapColorScaleDef.LabelInterval.OriginalText, this.MapColorScaleDef.LabelInterval.IntValue, 0);
				}
				return this.m_labelInterval;
			}
		}

		public ReportStringProperty LabelFormat
		{
			get
			{
				if (this.m_labelFormat == null && this.MapColorScaleDef.LabelFormat != null)
				{
					this.m_labelFormat = new ReportStringProperty(this.MapColorScaleDef.LabelFormat);
				}
				return this.m_labelFormat;
			}
		}

		public ReportEnumProperty<MapLabelPlacement> LabelPlacement
		{
			get
			{
				if (this.m_labelPlacement == null && this.MapColorScaleDef.LabelPlacement != null)
				{
					this.m_labelPlacement = new ReportEnumProperty<MapLabelPlacement>(this.MapColorScaleDef.LabelPlacement.IsExpression, this.MapColorScaleDef.LabelPlacement.OriginalText, EnumTranslator.TranslateLabelPlacement(this.MapColorScaleDef.LabelPlacement.StringValue, null));
				}
				return this.m_labelPlacement;
			}
		}

		public ReportEnumProperty<MapLabelBehavior> LabelBehavior
		{
			get
			{
				if (this.m_labelBehavior == null && this.MapColorScaleDef.LabelBehavior != null)
				{
					this.m_labelBehavior = new ReportEnumProperty<MapLabelBehavior>(this.MapColorScaleDef.LabelBehavior.IsExpression, this.MapColorScaleDef.LabelBehavior.OriginalText, EnumTranslator.TranslateLabelBehavior(this.MapColorScaleDef.LabelBehavior.StringValue, null));
				}
				return this.m_labelBehavior;
			}
		}

		public ReportBoolProperty HideEndLabels
		{
			get
			{
				if (this.m_hideEndLabels == null && this.MapColorScaleDef.HideEndLabels != null)
				{
					this.m_hideEndLabels = new ReportBoolProperty(this.MapColorScaleDef.HideEndLabels);
				}
				return this.m_hideEndLabels;
			}
		}

		public ReportColorProperty RangeGapColor
		{
			get
			{
				if (this.m_rangeGapColor == null && this.MapColorScaleDef.RangeGapColor != null)
				{
					ExpressionInfo rangeGapColor = this.MapColorScaleDef.RangeGapColor;
					if (rangeGapColor != null)
					{
						this.m_rangeGapColor = new ReportColorProperty(rangeGapColor.IsExpression, this.MapColorScaleDef.RangeGapColor.OriginalText, rangeGapColor.IsExpression ? null : new ReportColor(rangeGapColor.StringValue.Trim(), true), rangeGapColor.IsExpression ? new ReportColor("", Color.Empty, true) : null);
					}
				}
				return this.m_rangeGapColor;
			}
		}

		public ReportStringProperty NoDataText
		{
			get
			{
				if (this.m_noDataText == null && this.MapColorScaleDef.NoDataText != null)
				{
					this.m_noDataText = new ReportStringProperty(this.MapColorScaleDef.NoDataText);
				}
				return this.m_noDataText;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorScale MapColorScaleDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorScale)base.m_defObject;
			}
		}

		public new MapColorScaleInstance Instance
		{
			get
			{
				return (MapColorScaleInstance)this.GetInstance();
			}
		}

		internal MapColorScale(AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorScale defObject, Map map)
			: base(defObject, map)
		{
		}

		internal override MapSubItemInstance GetInstance()
		{
			if (base.m_map.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (base.m_instance == null)
			{
				base.m_instance = new MapColorScaleInstance(this);
			}
			return (MapSubItemInstance)base.m_instance;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (base.m_instance != null)
			{
				base.m_instance.SetNewContext();
			}
			if (this.m_mapColorScaleTitle != null)
			{
				this.m_mapColorScaleTitle.SetNewContext();
			}
		}
	}
}
