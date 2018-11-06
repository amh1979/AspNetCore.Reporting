using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapGridLines : IROMStyleDefinitionContainer
	{
		private Map m_map;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapGridLines m_defObject;

		private MapGridLinesInstance m_instance;

		private Style m_style;

		private ReportBoolProperty m_hidden;

		private ReportDoubleProperty m_interval;

		private ReportBoolProperty m_showLabels;

		private ReportEnumProperty<MapLabelPosition> m_labelPosition;

		public Style Style
		{
			get
			{
				if (this.m_style == null)
				{
					this.m_style = new Style(this.m_map, this.m_map.ReportScope, this.m_defObject, this.m_map.RenderingContext);
				}
				return this.m_style;
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

		public ReportBoolProperty ShowLabels
		{
			get
			{
				if (this.m_showLabels == null && this.m_defObject.ShowLabels != null)
				{
					this.m_showLabels = new ReportBoolProperty(this.m_defObject.ShowLabels);
				}
				return this.m_showLabels;
			}
		}

		public ReportEnumProperty<MapLabelPosition> LabelPosition
		{
			get
			{
				if (this.m_labelPosition == null && this.m_defObject.LabelPosition != null)
				{
					this.m_labelPosition = new ReportEnumProperty<MapLabelPosition>(this.m_defObject.LabelPosition.IsExpression, this.m_defObject.LabelPosition.OriginalText, EnumTranslator.TranslateLabelPosition(this.m_defObject.LabelPosition.StringValue, null));
				}
				return this.m_labelPosition;
			}
		}

		internal Map MapDef
		{
			get
			{
				return this.m_map;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapGridLines MapGridLinesDef
		{
			get
			{
				return this.m_defObject;
			}
		}

		public MapGridLinesInstance Instance
		{
			get
			{
				if (this.m_map.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					this.m_instance = new MapGridLinesInstance(this);
				}
				return this.m_instance;
			}
		}

		internal MapGridLines(AspNetCore.ReportingServices.ReportIntermediateFormat.MapGridLines defObject, Map map)
		{
			this.m_defObject = defObject;
			this.m_map = map;
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
