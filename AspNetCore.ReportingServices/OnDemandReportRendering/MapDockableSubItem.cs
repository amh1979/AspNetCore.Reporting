using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class MapDockableSubItem : MapSubItem, IROMActionOwner
	{
		private ActionInfo m_actionInfo;

		private ReportEnumProperty<MapPosition> m_position;

		private ReportBoolProperty m_dockOutsideViewport;

		private ReportBoolProperty m_hidden;

		private ReportStringProperty m_toolTip;

		public string UniqueName
		{
			get
			{
				return base.m_map.MapDef.UniqueName + 'x' + this.MapDockableSubItemDef.ID;
			}
		}

		public ActionInfo ActionInfo
		{
			get
			{
				if (this.m_actionInfo == null && this.MapDockableSubItemDef.Action != null)
				{
					this.m_actionInfo = new ActionInfo(base.m_map.RenderingContext, base.m_map.ReportScope, this.MapDockableSubItemDef.Action, base.m_map.MapDef, base.m_map, ObjectType.Map, base.m_map.Name, this);
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

		public ReportEnumProperty<MapPosition> Position
		{
			get
			{
				if (this.m_position == null && this.MapDockableSubItemDef.Position != null)
				{
					this.m_position = new ReportEnumProperty<MapPosition>(this.MapDockableSubItemDef.Position.IsExpression, this.MapDockableSubItemDef.Position.OriginalText, EnumTranslator.TranslateMapPosition(this.MapDockableSubItemDef.Position.StringValue, null));
				}
				return this.m_position;
			}
		}

		public ReportBoolProperty DockOutsideViewport
		{
			get
			{
				if (this.m_dockOutsideViewport == null && this.MapDockableSubItemDef.DockOutsideViewport != null)
				{
					this.m_dockOutsideViewport = new ReportBoolProperty(this.MapDockableSubItemDef.DockOutsideViewport);
				}
				return this.m_dockOutsideViewport;
			}
		}

		public ReportBoolProperty Hidden
		{
			get
			{
				if (this.m_hidden == null && this.MapDockableSubItemDef.Hidden != null)
				{
					this.m_hidden = new ReportBoolProperty(this.MapDockableSubItemDef.Hidden);
				}
				return this.m_hidden;
			}
		}

		public ReportStringProperty ToolTip
		{
			get
			{
				if (this.m_toolTip == null && this.MapDockableSubItemDef.ToolTip != null)
				{
					this.m_toolTip = new ReportStringProperty(this.MapDockableSubItemDef.ToolTip);
				}
				return this.m_toolTip;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapDockableSubItem MapDockableSubItemDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.MapDockableSubItem)base.m_defObject;
			}
		}

		internal new MapDockableSubItemInstance Instance
		{
			get
			{
				return (MapDockableSubItemInstance)this.GetInstance();
			}
		}

		internal MapDockableSubItem(AspNetCore.ReportingServices.ReportIntermediateFormat.MapDockableSubItem defObject, Map map)
			: base(defObject, map)
		{
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (base.m_instance != null)
			{
				base.m_instance.SetNewContext();
			}
			if (this.m_actionInfo != null)
			{
				this.m_actionInfo.SetNewContext();
			}
		}
	}
}
