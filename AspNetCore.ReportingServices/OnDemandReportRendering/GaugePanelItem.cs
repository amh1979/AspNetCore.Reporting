using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class GaugePanelItem : GaugePanelObjectCollectionItem, IROMStyleDefinitionContainer, IROMActionOwner
	{
		internal GaugePanel m_gaugePanel;

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanelItem m_defObject;

		private Style m_style;

		private ActionInfo m_actionInfo;

		private ReportDoubleProperty m_top;

		private ReportDoubleProperty m_left;

		private ReportDoubleProperty m_height;

		private ReportDoubleProperty m_width;

		private ReportIntProperty m_zIndex;

		private ReportBoolProperty m_hidden;

		private ReportStringProperty m_toolTip;

		string IROMActionOwner.UniqueName
		{
			get
			{
				return this.m_gaugePanel.GaugePanelDef.UniqueName + 'x' + this.m_defObject.ID;
			}
		}

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

		public string ParentItem
		{
			get
			{
				return this.m_defObject.ParentItem;
			}
		}

		internal GaugePanel GaugePanelDef
		{
			get
			{
				return this.m_gaugePanel;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanelItem GaugePanelItemDef
		{
			get
			{
				return this.m_defObject;
			}
		}

		public GaugePanelItemInstance Instance
		{
			get
			{
				return (GaugePanelItemInstance)this.GetInstance();
			}
		}

		internal GaugePanelItem(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanelItem defObject, GaugePanel gaugePanel)
		{
			this.m_defObject = defObject;
			this.m_gaugePanel = gaugePanel;
		}

		internal abstract BaseInstance GetInstance();

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
		}
	}
}
