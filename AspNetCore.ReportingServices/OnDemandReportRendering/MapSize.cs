using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapSize
	{
		private Map m_map;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapSize m_defObject;

		private MapSizeInstance m_instance;

		private ReportDoubleProperty m_width;

		private ReportDoubleProperty m_height;

		private ReportEnumProperty<Unit> m_unit;

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

		public ReportDoubleProperty Height
		{
			get
			{
				if (this.m_height == null && this.m_defObject.Height != null)
				{
					this.m_height = new ReportDoubleProperty(this.m_defObject.Height);
				}
				return this.m_height;
			}
		}

		public ReportEnumProperty<Unit> Unit
		{
			get
			{
				if (this.m_unit == null && this.m_defObject.Unit != null)
				{
					this.m_unit = new ReportEnumProperty<Unit>(this.m_defObject.Unit.IsExpression, this.m_defObject.Unit.OriginalText, EnumTranslator.TranslateUnit(this.m_defObject.Unit.StringValue, null));
				}
				return this.m_unit;
			}
		}

		internal Map MapDef
		{
			get
			{
				return this.m_map;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapSize MapSizeDef
		{
			get
			{
				return this.m_defObject;
			}
		}

		public MapSizeInstance Instance
		{
			get
			{
				if (this.m_map.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					this.m_instance = new MapSizeInstance(this);
				}
				return this.m_instance;
			}
		}

		internal MapSize(AspNetCore.ReportingServices.ReportIntermediateFormat.MapSize defObject, Map map)
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
		}
	}
}
