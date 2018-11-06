using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class MapSpatialElementTemplate : IROMStyleDefinitionContainer, IROMActionOwner
	{
		protected Map m_map;

		protected MapVectorLayer m_mapVectorLayer;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapSpatialElementTemplate m_defObject;

		protected MapSpatialElementTemplateInstance m_instance;

		private Style m_style;

		private ActionInfo m_actionInfo;

		private ReportBoolProperty m_hidden;

		private ReportDoubleProperty m_offsetX;

		private ReportDoubleProperty m_offsetY;

		private ReportStringProperty m_label;

		private ReportStringProperty m_toolTip;

		private ReportStringProperty m_dataElementLabel;

		public Style Style
		{
			get
			{
				if (this.m_style == null)
				{
					this.m_style = new Style(this.m_map, this.ReportScope, this.m_defObject, this.m_map.RenderingContext);
				}
				return this.m_style;
			}
		}

		public string UniqueName
		{
			get
			{
				return this.m_mapVectorLayer.ReportScope.ReportScopeInstance.UniqueName + 'x' + this.m_defObject.ID;
			}
		}

		public ActionInfo ActionInfo
		{
			get
			{
				if (this.m_actionInfo == null && this.m_defObject.Action != null)
				{
					this.m_actionInfo = new ActionInfo(this.m_map.RenderingContext, this.ReportScope, this.m_defObject.Action, this.m_map.MapDef, this.m_map, ObjectType.Map, this.m_map.Name, this);
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
				if (this.m_hidden == null && this.m_defObject.Hidden != null)
				{
					this.m_hidden = new ReportBoolProperty(this.m_defObject.Hidden);
				}
				return this.m_hidden;
			}
		}

		public ReportDoubleProperty OffsetX
		{
			get
			{
				if (this.m_offsetX == null && this.m_defObject.OffsetX != null)
				{
					this.m_offsetX = new ReportDoubleProperty(this.m_defObject.OffsetX);
				}
				return this.m_offsetX;
			}
		}

		public ReportDoubleProperty OffsetY
		{
			get
			{
				if (this.m_offsetY == null && this.m_defObject.OffsetY != null)
				{
					this.m_offsetY = new ReportDoubleProperty(this.m_defObject.OffsetY);
				}
				return this.m_offsetY;
			}
		}

		public ReportStringProperty Label
		{
			get
			{
				if (this.m_label == null && this.m_defObject.Label != null)
				{
					this.m_label = new ReportStringProperty(this.m_defObject.Label);
				}
				return this.m_label;
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

		public ReportStringProperty DataElementLabel
		{
			get
			{
				if (this.m_dataElementLabel == null)
				{
					if (this.m_defObject.DataElementLabel == null)
					{
						return this.Label;
					}
					this.m_dataElementLabel = new ReportStringProperty(this.m_defObject.DataElementLabel);
				}
				return this.m_dataElementLabel;
			}
		}

		public string DataElementName
		{
			get
			{
				return this.m_defObject.DataElementName;
			}
		}

		public DataElementOutputTypes DataElementOutput
		{
			get
			{
				return this.m_defObject.DataElementOutput;
			}
		}

		internal IReportScope ReportScope
		{
			get
			{
				return this.m_mapVectorLayer.ReportScope;
			}
		}

		internal Map MapDef
		{
			get
			{
				return this.m_map;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapSpatialElementTemplate MapSpatialElementTemplateDef
		{
			get
			{
				return this.m_defObject;
			}
		}

		public MapSpatialElementTemplateInstance Instance
		{
			get
			{
				return this.GetInstance();
			}
		}

		internal MapSpatialElementTemplate(AspNetCore.ReportingServices.ReportIntermediateFormat.MapSpatialElementTemplate defObject, MapVectorLayer mapVectorLayer, Map map)
		{
			this.m_defObject = defObject;
			this.m_mapVectorLayer = mapVectorLayer;
			this.m_map = map;
		}

		internal abstract MapSpatialElementTemplateInstance GetInstance();

		internal virtual void SetNewContext()
		{
			if (this.m_instance != null)
			{
				this.m_instance.SetNewContext();
			}
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
