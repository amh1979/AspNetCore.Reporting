using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartStripLine : ChartObjectCollectionItem<ChartStripLineInstance>, IROMStyleDefinitionContainer, IROMActionOwner
	{
		private Chart m_chart;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartStripLine m_chartStripLineDef;

		private Style m_style;

		private ActionInfo m_actionInfo;

		private ReportStringProperty m_title;

		private ReportEnumProperty<TextOrientations> m_textOrientation;

		private ReportIntProperty m_titleAngle;

		private ReportStringProperty m_toolTip;

		private ReportDoubleProperty m_interval;

		private ReportEnumProperty<ChartIntervalType> m_intervalType;

		private ReportDoubleProperty m_intervalOffset;

		private ReportEnumProperty<ChartIntervalType> m_intervalOffsetType;

		private ReportDoubleProperty m_stripWidth;

		private ReportEnumProperty<ChartIntervalType> m_stripWidthType;

		public Style Style
		{
			get
			{
				if (this.m_style == null && !this.m_chart.IsOldSnapshot && this.m_chartStripLineDef.StyleClass != null)
				{
					this.m_style = new Style(this.m_chart, this.m_chart, this.m_chartStripLineDef, this.m_chart.RenderingContext);
				}
				return this.m_style;
			}
		}

		public string UniqueName
		{
			get
			{
				return this.m_chart.ChartDef.UniqueName + 'x' + this.m_chartStripLineDef.ID;
			}
		}

		public ActionInfo ActionInfo
		{
			get
			{
				if (this.m_actionInfo == null && !this.m_chart.IsOldSnapshot && this.m_chartStripLineDef.Action != null)
				{
					this.m_actionInfo = new ActionInfo(this.m_chart.RenderingContext, this.m_chart, this.m_chartStripLineDef.Action, this.m_chart.ChartDef, this.m_chart, ObjectType.Chart, this.m_chartStripLineDef.Name, this);
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

		public ReportStringProperty Title
		{
			get
			{
				if (this.m_title == null && !this.m_chart.IsOldSnapshot && this.m_chartStripLineDef.Title != null)
				{
					this.m_title = new ReportStringProperty(this.m_chartStripLineDef.Title);
				}
				return this.m_title;
			}
		}

		[Obsolete("Use TextOrientation instead.")]
		public ReportIntProperty TitleAngle
		{
			get
			{
				if (this.m_titleAngle == null && !this.m_chart.IsOldSnapshot && this.m_chartStripLineDef.TitleAngle != null)
				{
					this.m_titleAngle = new ReportIntProperty(this.m_chartStripLineDef.TitleAngle.IsExpression, this.m_chartStripLineDef.TitleAngle.OriginalText, this.m_chartStripLineDef.TitleAngle.IntValue, 0);
				}
				return this.m_titleAngle;
			}
		}

		public ReportEnumProperty<TextOrientations> TextOrientation
		{
			get
			{
				if (this.m_textOrientation == null && !this.m_chart.IsOldSnapshot && this.m_chartStripLineDef.TextOrientation != null)
				{
					this.m_textOrientation = new ReportEnumProperty<TextOrientations>(this.m_chartStripLineDef.TextOrientation.IsExpression, this.m_chartStripLineDef.TextOrientation.OriginalText, EnumTranslator.TranslateTextOrientations(this.m_chartStripLineDef.TextOrientation.StringValue, null));
				}
				return this.m_textOrientation;
			}
		}

		public ReportStringProperty ToolTip
		{
			get
			{
				if (this.m_toolTip == null && !this.m_chart.IsOldSnapshot && this.m_chartStripLineDef.ToolTip != null)
				{
					this.m_toolTip = new ReportStringProperty(this.m_chartStripLineDef.ToolTip);
				}
				return this.m_toolTip;
			}
		}

		public ReportDoubleProperty Interval
		{
			get
			{
				if (this.m_interval == null && !this.m_chart.IsOldSnapshot && this.m_chartStripLineDef.Interval != null)
				{
					this.m_interval = new ReportDoubleProperty(this.m_chartStripLineDef.Interval);
				}
				return this.m_interval;
			}
		}

		public ReportEnumProperty<ChartIntervalType> IntervalType
		{
			get
			{
				if (this.m_intervalType == null && !this.m_chart.IsOldSnapshot && this.m_chartStripLineDef.IntervalType != null)
				{
					this.m_intervalType = new ReportEnumProperty<ChartIntervalType>(this.m_chartStripLineDef.IntervalType.IsExpression, this.m_chartStripLineDef.IntervalType.OriginalText, EnumTranslator.TranslateChartIntervalType(this.m_chartStripLineDef.IntervalType.StringValue, null));
				}
				return this.m_intervalType;
			}
		}

		public ReportDoubleProperty IntervalOffset
		{
			get
			{
				if (this.m_intervalOffset == null && !this.m_chart.IsOldSnapshot && this.m_chartStripLineDef.IntervalOffset != null)
				{
					this.m_intervalOffset = new ReportDoubleProperty(this.m_chartStripLineDef.IntervalOffset);
				}
				return this.m_intervalOffset;
			}
		}

		public ReportEnumProperty<ChartIntervalType> IntervalOffsetType
		{
			get
			{
				if (this.m_intervalOffsetType == null && !this.m_chart.IsOldSnapshot && this.m_chartStripLineDef.IntervalOffsetType != null)
				{
					this.m_intervalOffsetType = new ReportEnumProperty<ChartIntervalType>(this.m_chartStripLineDef.IntervalOffsetType.IsExpression, this.m_chartStripLineDef.IntervalOffsetType.OriginalText, EnumTranslator.TranslateChartIntervalType(this.m_chartStripLineDef.IntervalOffsetType.StringValue, null));
				}
				return this.m_intervalOffsetType;
			}
		}

		public ReportDoubleProperty StripWidth
		{
			get
			{
				if (this.m_stripWidth == null && !this.m_chart.IsOldSnapshot && this.m_chartStripLineDef.StripWidth != null)
				{
					this.m_stripWidth = new ReportDoubleProperty(this.m_chartStripLineDef.StripWidth);
				}
				return this.m_stripWidth;
			}
		}

		public ReportEnumProperty<ChartIntervalType> StripWidthType
		{
			get
			{
				if (this.m_stripWidthType == null && !this.m_chart.IsOldSnapshot && this.m_chartStripLineDef.StripWidthType != null)
				{
					this.m_stripWidthType = new ReportEnumProperty<ChartIntervalType>(this.m_chartStripLineDef.StripWidthType.IsExpression, this.m_chartStripLineDef.StripWidthType.OriginalText, EnumTranslator.TranslateChartIntervalType(this.m_chartStripLineDef.StripWidthType.StringValue, null));
				}
				return this.m_stripWidthType;
			}
		}

		internal Chart ChartDef
		{
			get
			{
				return this.m_chart;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.ChartStripLine ChartStripLineDef
		{
			get
			{
				return this.m_chartStripLineDef;
			}
		}

		public ChartStripLineInstance Instance
		{
			get
			{
				if (this.m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (base.m_instance == null)
				{
					base.m_instance = new ChartStripLineInstance(this);
				}
				return base.m_instance;
			}
		}

		internal ChartStripLine(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartStripLine chartStripLineDef, Chart chart)
		{
			this.m_chartStripLineDef = chartStripLineDef;
			this.m_chart = chart;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (this.m_style != null)
			{
				this.m_style.SetNewContext();
			}
			if (this.m_actionInfo != null)
			{
				this.m_actionInfo.SetNewContext();
			}
		}
	}
}
