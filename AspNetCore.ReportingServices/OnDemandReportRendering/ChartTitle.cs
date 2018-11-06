using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartTitle : ChartObjectCollectionItem<ChartTitleInstance>, IROMStyleDefinitionContainer, IROMActionOwner
	{
		private Chart m_chart;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartTitle m_chartTitleDef;

		private Style m_style;

		private ReportStringProperty m_caption;

		private AspNetCore.ReportingServices.ReportProcessing.ChartTitle m_renderChartTitleDef;

		private AspNetCore.ReportingServices.ReportProcessing.ChartTitleInstance m_renderChartTitleInstance;

		private ReportEnumProperty<ChartTitlePositions> m_position;

		private ActionInfo m_actionInfo;

		private ReportBoolProperty m_hidden;

		private ReportIntProperty m_dockOffset;

		private ReportBoolProperty m_dockOutsideChartArea;

		private ReportStringProperty m_toolTip;

		private ReportEnumProperty<TextOrientations> m_textOrientation;

		private ChartElementPosition m_chartElementPosition;

		public string Name
		{
			get
			{
				if (this.m_chart.IsOldSnapshot)
				{
					return "Default";
				}
				return this.m_chartTitleDef.TitleName;
			}
		}

		public ReportStringProperty Caption
		{
			get
			{
				if (this.m_caption == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						if (this.m_renderChartTitleDef.Caption != null)
						{
							this.m_caption = new ReportStringProperty(this.m_renderChartTitleDef.Caption);
						}
					}
					else if (this.m_chartTitleDef.Caption != null)
					{
						this.m_caption = new ReportStringProperty(this.m_chartTitleDef.Caption);
					}
				}
				return this.m_caption;
			}
		}

		public Style Style
		{
			get
			{
				if (this.m_style == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						this.m_style = new Style(this.m_renderChartTitleDef.StyleClass, this.m_renderChartTitleInstance.StyleAttributeValues, this.m_chart.RenderingContext);
					}
					else if (this.m_chartTitleDef != null)
					{
						this.m_style = new Style(this.m_chart, this.m_chart, this.m_chartTitleDef, this.m_chart.RenderingContext);
					}
				}
				return this.m_style;
			}
		}

		public ReportEnumProperty<ChartTitlePositions> Position
		{
			get
			{
				if (this.m_position == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						ChartTitlePositions value = ChartTitlePositions.TopCenter;
						switch (this.m_renderChartTitleDef.Position)
						{
						case AspNetCore.ReportingServices.ReportProcessing.ChartTitle.Positions.Center:
						case AspNetCore.ReportingServices.ReportProcessing.ChartTitle.Positions.Near:
						case AspNetCore.ReportingServices.ReportProcessing.ChartTitle.Positions.Far:
							value = ChartTitlePositions.TopCenter;
							break;
						}
						this.m_position = new ReportEnumProperty<ChartTitlePositions>(value);
					}
					else if (this.m_chartTitleDef.Position != null)
					{
						this.m_position = new ReportEnumProperty<ChartTitlePositions>(this.m_chartTitleDef.Position.IsExpression, this.m_chartTitleDef.Position.OriginalText, EnumTranslator.TranslateChartTitlePosition(this.m_chartTitleDef.Position.StringValue, null));
					}
				}
				return this.m_position;
			}
		}

		public string UniqueName
		{
			get
			{
				return this.m_chart.ChartDef.UniqueName + 'x' + "ChartTitle_" + this.Name;
			}
		}

		public ActionInfo ActionInfo
		{
			get
			{
				if (this.m_actionInfo == null && !this.m_chart.IsOldSnapshot && this.m_chartTitleDef.Action != null)
				{
					this.m_actionInfo = new ActionInfo(this.m_chart.RenderingContext, this.m_chart, this.m_chartTitleDef.Action, this.m_chart.ChartDef, this.m_chart, ObjectType.Chart, this.m_chartTitleDef.Name, this);
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

		public ReportBoolProperty Hidden
		{
			get
			{
				if (this.m_hidden == null && !this.m_chart.IsOldSnapshot && this.m_chartTitleDef.Hidden != null)
				{
					this.m_hidden = new ReportBoolProperty(this.m_chartTitleDef.Hidden);
				}
				return this.m_hidden;
			}
		}

		public string DockToChartArea
		{
			get
			{
				if (this.m_chart.IsOldSnapshot)
				{
					return null;
				}
				return this.m_chartTitleDef.DockToChartArea;
			}
		}

		public ReportIntProperty DockOffset
		{
			get
			{
				if (this.m_dockOffset == null && !this.m_chart.IsOldSnapshot && this.m_chartTitleDef.DockOffset != null)
				{
					this.m_dockOffset = new ReportIntProperty(this.m_chartTitleDef.DockOffset.IsExpression, this.m_chartTitleDef.DockOffset.OriginalText, this.m_chartTitleDef.DockOffset.IntValue, 0);
				}
				return this.m_dockOffset;
			}
		}

		public ReportBoolProperty DockOutsideChartArea
		{
			get
			{
				if (this.m_dockOutsideChartArea == null && !this.m_chart.IsOldSnapshot && this.m_chartTitleDef.DockOutsideChartArea != null)
				{
					this.m_dockOutsideChartArea = new ReportBoolProperty(this.m_chartTitleDef.DockOutsideChartArea);
				}
				return this.m_dockOutsideChartArea;
			}
		}

		public ReportStringProperty ToolTip
		{
			get
			{
				if (this.m_toolTip == null && !this.m_chart.IsOldSnapshot && this.m_chartTitleDef.ToolTip != null)
				{
					this.m_toolTip = new ReportStringProperty(this.m_chartTitleDef.ToolTip);
				}
				return this.m_toolTip;
			}
		}

		public ReportEnumProperty<TextOrientations> TextOrientation
		{
			get
			{
				if (this.m_textOrientation == null && !this.m_chart.IsOldSnapshot && this.m_chartTitleDef.TextOrientation != null)
				{
					this.m_textOrientation = new ReportEnumProperty<TextOrientations>(this.m_chartTitleDef.TextOrientation.IsExpression, this.m_chartTitleDef.TextOrientation.OriginalText, EnumTranslator.TranslateTextOrientations(this.m_chartTitleDef.TextOrientation.StringValue, null));
				}
				return this.m_textOrientation;
			}
		}

		public ChartElementPosition ChartElementPosition
		{
			get
			{
				if (this.m_chartElementPosition == null && !this.m_chart.IsOldSnapshot && this.m_chartTitleDef.ChartElementPosition != null)
				{
					this.m_chartElementPosition = new ChartElementPosition(this.m_chartTitleDef.ChartElementPosition, this.m_chart);
				}
				return this.m_chartElementPosition;
			}
		}

		internal Chart ChartDef
		{
			get
			{
				return this.m_chart;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.ChartTitle ChartTitleDef
		{
			get
			{
				return this.m_chartTitleDef;
			}
		}

		internal AspNetCore.ReportingServices.ReportProcessing.ChartTitleInstance RenderChartTitleInstance
		{
			get
			{
				return this.m_renderChartTitleInstance;
			}
		}

		public ChartTitleInstance Instance
		{
			get
			{
				if (this.m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (base.m_instance == null)
				{
					base.m_instance = new ChartTitleInstance(this);
				}
				return base.m_instance;
			}
		}

		internal ChartTitle(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartTitle chartTitleDef, Chart chart)
		{
			this.m_chart = chart;
			this.m_chartTitleDef = chartTitleDef;
		}

		internal ChartTitle(AspNetCore.ReportingServices.ReportProcessing.ChartTitle renderChartTitleDef, AspNetCore.ReportingServices.ReportProcessing.ChartTitleInstance renderChartTitleInstance, Chart chart)
		{
			this.m_chart = chart;
			this.m_renderChartTitleDef = renderChartTitleDef;
			this.m_renderChartTitleInstance = renderChartTitleInstance;
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
			if (this.m_chartElementPosition != null)
			{
				this.m_chartElementPosition.SetNewContext();
			}
		}
	}
}
