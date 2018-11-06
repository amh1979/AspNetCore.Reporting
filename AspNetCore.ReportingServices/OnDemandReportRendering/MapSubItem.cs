using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class MapSubItem : MapObjectCollectionItem, IROMStyleDefinitionContainer
	{
		protected Map m_map;

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapSubItem m_defObject;

		private Style m_style;

		private MapLocation m_mapLocation;

		private MapSize m_mapSize;

		private ReportSizeProperty m_leftMargin;

		private ReportSizeProperty m_rightMargin;

		private ReportSizeProperty m_topMargin;

		private ReportSizeProperty m_bottomMargin;

		private ReportIntProperty m_zIndex;

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

		public MapLocation MapLocation
		{
			get
			{
				if (this.m_mapLocation == null && this.m_defObject.MapLocation != null)
				{
					this.m_mapLocation = new MapLocation(this.m_defObject.MapLocation, this.m_map);
				}
				return this.m_mapLocation;
			}
		}

		public MapSize MapSize
		{
			get
			{
				if (this.m_mapSize == null && this.m_defObject.MapSize != null)
				{
					this.m_mapSize = new MapSize(this.m_defObject.MapSize, this.m_map);
				}
				return this.m_mapSize;
			}
		}

		public ReportSizeProperty LeftMargin
		{
			get
			{
				if (this.m_leftMargin == null && this.m_defObject.LeftMargin != null)
				{
					this.m_leftMargin = new ReportSizeProperty(this.m_defObject.LeftMargin);
				}
				return this.m_leftMargin;
			}
		}

		public ReportSizeProperty RightMargin
		{
			get
			{
				if (this.m_rightMargin == null && this.m_defObject.RightMargin != null)
				{
					this.m_rightMargin = new ReportSizeProperty(this.m_defObject.RightMargin);
				}
				return this.m_rightMargin;
			}
		}

		public ReportSizeProperty TopMargin
		{
			get
			{
				if (this.m_topMargin == null && this.m_defObject.TopMargin != null)
				{
					this.m_topMargin = new ReportSizeProperty(this.m_defObject.TopMargin);
				}
				return this.m_topMargin;
			}
		}

		public ReportSizeProperty BottomMargin
		{
			get
			{
				if (this.m_bottomMargin == null && this.m_defObject.BottomMargin != null)
				{
					this.m_bottomMargin = new ReportSizeProperty(this.m_defObject.BottomMargin);
				}
				return this.m_bottomMargin;
			}
		}

		public ReportIntProperty ZIndex
		{
			get
			{
				if (this.m_zIndex == null && this.m_defObject.ZIndex != null)
				{
					this.m_zIndex = new ReportIntProperty(this.m_defObject.ZIndex.IsExpression, this.m_defObject.ZIndex.OriginalText, this.m_defObject.ZIndex.IntValue, 0);
				}
				return this.m_zIndex;
			}
		}

		internal Map MapDef
		{
			get
			{
				return this.m_map;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapSubItem MapSubItemDef
		{
			get
			{
				return this.m_defObject;
			}
		}

		internal MapSubItemInstance Instance
		{
			get
			{
				return this.GetInstance();
			}
		}

		internal MapSubItem(AspNetCore.ReportingServices.ReportIntermediateFormat.MapSubItem defObject, Map map)
		{
			this.m_defObject = defObject;
			this.m_map = map;
		}

		internal abstract MapSubItemInstance GetInstance();

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
			if (this.m_mapLocation != null)
			{
				this.m_mapLocation.SetNewContext();
			}
			if (this.m_mapSize != null)
			{
				this.m_mapSize.SetNewContext();
			}
		}
	}
}
