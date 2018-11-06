using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapLocation
	{
		private Map m_map;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapLocation m_defObject;

		private MapLocationInstance m_instance;

		private ReportDoubleProperty m_left;

		private ReportDoubleProperty m_top;

		private ReportEnumProperty<Unit> m_unit;

		public ReportDoubleProperty Left
		{
			get
			{
				if (this.m_left == null && this.m_defObject.Left != null)
				{
					this.m_left = new ReportDoubleProperty(this.m_defObject.Left);
				}
				return this.m_left;
			}
		}

		public ReportDoubleProperty Top
		{
			get
			{
				if (this.m_top == null && this.m_defObject.Top != null)
				{
					this.m_top = new ReportDoubleProperty(this.m_defObject.Top);
				}
				return this.m_top;
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

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapLocation MapLocationDef
		{
			get
			{
				return this.m_defObject;
			}
		}

		public MapLocationInstance Instance
		{
			get
			{
				if (this.m_map.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					this.m_instance = new MapLocationInstance(this);
				}
				return this.m_instance;
			}
		}

		internal MapLocation(AspNetCore.ReportingServices.ReportIntermediateFormat.MapLocation defObject, Map map)
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
