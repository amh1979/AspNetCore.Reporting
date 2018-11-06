using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapDataRegion : DataRegion, IMapObjectCollectionItem
	{
		private MapMember m_innerMostMampMember;

		private MapMember m_mapMember;

		public MapMember MapMember
		{
			get
			{
				if (this.m_mapMember == null)
				{
					this.m_mapMember = new MapMember(this.ReportScope, this, this, null, this.MapDataRegionDef.MapMember);
				}
				return this.m_mapMember;
			}
		}

		internal MapMember InnerMostMapMember
		{
			get
			{
				if (this.m_innerMostMampMember == null)
				{
					this.m_innerMostMampMember = this.MapMember;
					while (this.m_innerMostMampMember.ChildMapMember != null)
					{
						this.m_innerMostMampMember = this.m_innerMostMampMember.ChildMapMember;
					}
				}
				return this.m_innerMostMampMember;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapDataRegion MapDataRegionDef
		{
			get
			{
				return base.m_reportItemDef as AspNetCore.ReportingServices.ReportIntermediateFormat.MapDataRegion;
			}
		}

		internal override bool HasDataCells
		{
			get
			{
				return false;
			}
		}

		internal override IDataRegionRowCollection RowCollection
		{
			get
			{
				return null;
			}
		}

		public new MapDataRegionInstance Instance
		{
			get
			{
				return (MapDataRegionInstance)this.GetOrCreateInstance();
			}
		}

		internal MapDataRegion(IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, AspNetCore.ReportingServices.ReportIntermediateFormat.MapDataRegion reportItemDef, RenderingContext renderingContext)
			: base(parentDefinitionPath, indexIntoParentCollectionDef, reportItemDef, renderingContext)
		{
		}

		void IMapObjectCollectionItem.SetNewContext()
		{
			this.SetNewContext();
		}

		internal List<MapVectorLayer> GetChildLayers()
		{
			MapLayerCollection mapLayers = ((Map)base.m_parentDefinitionPath).MapLayers;
			List<MapVectorLayer> list = new List<MapVectorLayer>();
			foreach (MapLayer item in mapLayers)
			{
				if (!(item is MapTileLayer) && object.ReferenceEquals(((MapVectorLayer)item).MapDataRegion, this))
				{
					list.Add((MapVectorLayer)item);
				}
			}
			return list;
		}

		internal override ReportItemInstance GetOrCreateInstance()
		{
			if (base.m_instance == null)
			{
				base.m_instance = new MapDataRegionInstance(this);
			}
			return base.m_instance;
		}

		internal override void SetNewContextChildren()
		{
			if (this.m_mapMember != null)
			{
				this.m_mapMember.ResetContext();
			}
			if (base.m_instance != null)
			{
				base.m_instance.SetNewContext();
			}
		}
	}
}
