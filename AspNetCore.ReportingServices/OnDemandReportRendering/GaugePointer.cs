using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class GaugePointer : GaugePanelObjectCollectionItem, IROMStyleDefinitionContainer, IROMActionOwner
	{
		internal GaugePanel m_gaugePanel;

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePointer m_defObject;

		private Style m_style;

		private ActionInfo m_actionInfo;

		private GaugeInputValue m_gaugeInputValue;

		private ReportEnumProperty<GaugeBarStarts> m_barStart;

		private ReportDoubleProperty m_distanceFromScale;

		private PointerImage m_pointerImage;

		private ReportDoubleProperty m_markerLength;

		private ReportEnumProperty<GaugeMarkerStyles> m_markerStyle;

		private ReportEnumProperty<GaugePointerPlacements> m_placement;

		private ReportBoolProperty m_snappingEnabled;

		private ReportDoubleProperty m_snappingInterval;

		private ReportStringProperty m_toolTip;

		private ReportBoolProperty m_hidden;

		private ReportDoubleProperty m_width;

		private CompiledGaugePointerInstance[] m_compiledInstances;

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

		public GaugeInputValue GaugeInputValue
		{
			get
			{
				if (this.m_gaugeInputValue == null && this.m_defObject.GaugeInputValue != null)
				{
					this.m_gaugeInputValue = new GaugeInputValue(this.m_defObject.GaugeInputValue, this.m_gaugePanel);
				}
				return this.m_gaugeInputValue;
			}
		}

		public ReportEnumProperty<GaugeBarStarts> BarStart
		{
			get
			{
				if (this.m_barStart == null && this.m_defObject.BarStart != null)
				{
					this.m_barStart = new ReportEnumProperty<GaugeBarStarts>(this.m_defObject.BarStart.IsExpression, this.m_defObject.BarStart.OriginalText, EnumTranslator.TranslateGaugeBarStarts(this.m_defObject.BarStart.StringValue, null));
				}
				return this.m_barStart;
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

		public PointerImage PointerImage
		{
			get
			{
				if (this.m_pointerImage == null && this.m_defObject.PointerImage != null)
				{
					this.m_pointerImage = new PointerImage(this.m_defObject.PointerImage, this.m_gaugePanel);
				}
				return this.m_pointerImage;
			}
		}

		public ReportDoubleProperty MarkerLength
		{
			get
			{
				if (this.m_markerLength == null && this.m_defObject.MarkerLength != null)
				{
					this.m_markerLength = new ReportDoubleProperty(this.m_defObject.MarkerLength);
				}
				return this.m_markerLength;
			}
		}

		public ReportEnumProperty<GaugeMarkerStyles> MarkerStyle
		{
			get
			{
				if (this.m_markerStyle == null && this.m_defObject.MarkerStyle != null)
				{
					this.m_markerStyle = new ReportEnumProperty<GaugeMarkerStyles>(this.m_defObject.MarkerStyle.IsExpression, this.m_defObject.MarkerStyle.OriginalText, EnumTranslator.TranslateGaugeMarkerStyles(this.m_defObject.MarkerStyle.StringValue, null));
				}
				return this.m_markerStyle;
			}
		}

		public ReportEnumProperty<GaugePointerPlacements> Placement
		{
			get
			{
				if (this.m_placement == null && this.m_defObject.Placement != null)
				{
					this.m_placement = new ReportEnumProperty<GaugePointerPlacements>(this.m_defObject.Placement.IsExpression, this.m_defObject.Placement.OriginalText, EnumTranslator.TranslateGaugePointerPlacements(this.m_defObject.Placement.StringValue, null));
				}
				return this.m_placement;
			}
		}

		public ReportBoolProperty SnappingEnabled
		{
			get
			{
				if (this.m_snappingEnabled == null && this.m_defObject.SnappingEnabled != null)
				{
					this.m_snappingEnabled = new ReportBoolProperty(this.m_defObject.SnappingEnabled);
				}
				return this.m_snappingEnabled;
			}
		}

		public ReportDoubleProperty SnappingInterval
		{
			get
			{
				if (this.m_snappingInterval == null && this.m_defObject.SnappingInterval != null)
				{
					this.m_snappingInterval = new ReportDoubleProperty(this.m_defObject.SnappingInterval);
				}
				return this.m_snappingInterval;
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

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePointer GaugePointerDef
		{
			get
			{
				return this.m_defObject;
			}
		}

		public GaugePointerInstance Instance
		{
			get
			{
				return this.GetInstance();
			}
		}

		public CompiledGaugePointerInstance[] CompiledInstances
		{
			get
			{
				this.GaugePanelDef.ProcessCompiledInstances();
				return this.m_compiledInstances;
			}
			internal set
			{
				this.m_compiledInstances = value;
			}
		}

		internal GaugePointer(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePointer defObject, GaugePanel gaugePanel)
		{
			this.m_defObject = defObject;
			this.m_gaugePanel = gaugePanel;
		}

		internal abstract GaugePointerInstance GetInstance();

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
			if (this.m_gaugeInputValue != null)
			{
				this.m_gaugeInputValue.SetNewContext();
			}
			if (this.m_pointerImage != null)
			{
				this.m_pointerImage.SetNewContext();
			}
		}
	}
}
