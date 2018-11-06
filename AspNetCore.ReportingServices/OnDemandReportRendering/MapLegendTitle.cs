using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System.Drawing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapLegendTitle : IROMStyleDefinitionContainer
	{
		private Map m_map;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapLegendTitle m_defObject;

		private MapLegendTitleInstance m_instance;

		private Style m_style;

		private ReportStringProperty m_caption;

		private ReportEnumProperty<MapLegendTitleSeparator> m_titleSeparator;

		private ReportColorProperty m_titleSeparatorColor;

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

		public ReportStringProperty Caption
		{
			get
			{
				if (this.m_caption == null && this.m_defObject.Caption != null)
				{
					this.m_caption = new ReportStringProperty(this.m_defObject.Caption);
				}
				return this.m_caption;
			}
		}

		public ReportEnumProperty<MapLegendTitleSeparator> TitleSeparator
		{
			get
			{
				if (this.m_titleSeparator == null && this.m_defObject.TitleSeparator != null)
				{
					this.m_titleSeparator = new ReportEnumProperty<MapLegendTitleSeparator>(this.m_defObject.TitleSeparator.IsExpression, this.m_defObject.TitleSeparator.OriginalText, EnumTranslator.TranslateMapLegendTitleSeparator(this.m_defObject.TitleSeparator.StringValue, null));
				}
				return this.m_titleSeparator;
			}
		}

		public ReportColorProperty TitleSeparatorColor
		{
			get
			{
				if (this.m_titleSeparatorColor == null && this.m_defObject.TitleSeparatorColor != null)
				{
					ExpressionInfo titleSeparatorColor = this.m_defObject.TitleSeparatorColor;
					if (titleSeparatorColor != null)
					{
						this.m_titleSeparatorColor = new ReportColorProperty(titleSeparatorColor.IsExpression, this.m_defObject.TitleSeparatorColor.OriginalText, titleSeparatorColor.IsExpression ? null : new ReportColor(titleSeparatorColor.StringValue.Trim(), true), titleSeparatorColor.IsExpression ? new ReportColor("", Color.Empty, true) : null);
					}
				}
				return this.m_titleSeparatorColor;
			}
		}

		internal Map MapDef
		{
			get
			{
				return this.m_map;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapLegendTitle MapLegendTitleDef
		{
			get
			{
				return this.m_defObject;
			}
		}

		public MapLegendTitleInstance Instance
		{
			get
			{
				if (this.m_map.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					this.m_instance = new MapLegendTitleInstance(this);
				}
				return this.m_instance;
			}
		}

		internal MapLegendTitle(AspNetCore.ReportingServices.ReportIntermediateFormat.MapLegendTitle defObject, Map map)
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
