using AspNetCore.ReportingServices.ReportProcessing;
using System;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class CustomReportItem : ReportItem
	{
		private ReportItem m_altReportItem;

		private CustomData m_customData;

		private bool m_isProcessing;

		private NonComputedUniqueNames[] m_childrenNonComputedUniqueNames;

		public string Type
		{
			get
			{
				return ((AspNetCore.ReportingServices.ReportProcessing.CustomReportItem)base.ReportItemDef).Type;
			}
		}

		public ReportItem AltReportItem
		{
			get
			{
				if (this.m_isProcessing)
				{
					return null;
				}
				ReportItem reportItem = this.m_altReportItem;
				if (this.m_altReportItem == null)
				{
					AspNetCore.ReportingServices.ReportProcessing.CustomReportItem customReportItem = (AspNetCore.ReportingServices.ReportProcessing.CustomReportItem)base.ReportItemDef;
					AspNetCore.ReportingServices.ReportProcessing.ReportItem reportItem2 = null;
					Global.Tracer.Assert(customReportItem.RenderReportItem != null || null != customReportItem.AltReportItem);
					if (customReportItem.RenderReportItem != null && 1 == customReportItem.RenderReportItem.Count)
					{
						reportItem2 = customReportItem.RenderReportItem[0];
					}
					else if (customReportItem.AltReportItem != null && 1 == customReportItem.AltReportItem.Count)
					{
						Global.Tracer.Assert(null == customReportItem.RenderReportItem);
						reportItem2 = customReportItem.AltReportItem[0];
					}
					if (reportItem2 != null)
					{
						ReportItemInstance reportItemInstance = null;
						NonComputedUniqueNames[] childrenNonComputedUniqueNames = this.m_childrenNonComputedUniqueNames;
						if (base.ReportItemInstance != null)
						{
							CustomReportItemInstance customReportItemInstance = (CustomReportItemInstance)base.ReportItemInstance;
							Global.Tracer.Assert(null != customReportItemInstance);
							if (customReportItemInstance.AltReportItemColInstance != null)
							{
								if (customReportItemInstance.AltReportItemColInstance.ReportItemInstances != null && 0 < customReportItemInstance.AltReportItemColInstance.ReportItemInstances.Count)
								{
									reportItemInstance = customReportItemInstance.AltReportItemColInstance[0];
								}
								else
								{
									if (customReportItemInstance.AltReportItemColInstance.ChildrenNonComputedUniqueNames != null)
									{
										childrenNonComputedUniqueNames = customReportItemInstance.AltReportItemColInstance.ChildrenNonComputedUniqueNames;
									}
									if (childrenNonComputedUniqueNames == null)
									{
										ReportItemColInstanceInfo instanceInfo = customReportItemInstance.AltReportItemColInstance.GetInstanceInfo(this.RenderingContext.ChunkManager, this.RenderingContext.InPageSection);
										childrenNonComputedUniqueNames = instanceInfo.ChildrenNonComputedUniqueNames;
									}
								}
							}
						}
						reportItem = ReportItem.CreateItem(0, reportItem2, reportItemInstance, this.RenderingContext, (childrenNonComputedUniqueNames == null) ? null : childrenNonComputedUniqueNames[0]);
						if (base.UseCache)
						{
							this.m_altReportItem = reportItem;
						}
					}
				}
				return reportItem;
			}
		}

		public CustomData CustomData
		{
			get
			{
				CustomData customData = this.m_customData;
				if (this.m_customData == null)
				{
					customData = new CustomData(this);
					if (base.UseCache)
					{
						this.m_customData = customData;
					}
				}
				return customData;
			}
		}

		public override bool Hidden
		{
			get
			{
				if (!this.m_isProcessing)
				{
					return base.Hidden;
				}
				if (base.ReportItemDef.Visibility == null)
				{
					return false;
				}
				return base.InstanceInfo.StartHidden;
			}
		}

		internal new TextBox ToggleParent
		{
			get
			{
				if (!this.m_isProcessing)
				{
					return base.ToggleParent;
				}
				return null;
			}
		}

		public new bool IsToggleChild
		{
			get
			{
				if (!this.m_isProcessing)
				{
					return base.IsToggleChild;
				}
				return false;
			}
		}

		public override object SharedRenderingInfo
		{
			get
			{
				if (!this.m_isProcessing)
				{
					return base.SharedRenderingInfo;
				}
				return null;
			}
			set
			{
				if (!this.m_isProcessing)
				{
					base.SharedRenderingInfo = value;
					return;
				}
				throw new NotSupportedException();
			}
		}

		public new object RenderingInfo
		{
			get
			{
				if (!this.m_isProcessing)
				{
					return base.RenderingInfo;
				}
				return null;
			}
			set
			{
				if (!this.m_isProcessing)
				{
					base.RenderingInfo = value;
					return;
				}
				throw new NotSupportedException();
			}
		}

		internal AspNetCore.ReportingServices.ReportProcessing.CustomReportItem CriDefinition
		{
			get
			{
				return base.ReportItemDef as AspNetCore.ReportingServices.ReportProcessing.CustomReportItem;
			}
		}

		internal CustomReportItemInstance CriInstance
		{
			get
			{
				return base.ReportItemInstance as CustomReportItemInstance;
			}
		}

		internal new RenderingContext RenderingContext
		{
			get
			{
				if (this.m_isProcessing)
				{
					return null;
				}
				return base.RenderingContext;
			}
		}

		internal CustomReportItem(AspNetCore.ReportingServices.ReportProcessing.CustomReportItem criDef, CustomReportItemInstance criInstance, CustomReportItemInstanceInfo instanceInfo)
			: base(criDef, criInstance, instanceInfo)
		{
			this.m_isProcessing = true;
		}

		internal CustomReportItem(string uniqueName, int intUniqueName, AspNetCore.ReportingServices.ReportProcessing.ReportItem reportItemDef, ReportItemInstance reportItemInstance, RenderingContext renderingContext, NonComputedUniqueNames[] childrenNonComputedUniqueNames)
			: base(uniqueName, intUniqueName, reportItemDef, reportItemInstance, renderingContext)
		{
			this.m_isProcessing = false;
			this.m_childrenNonComputedUniqueNames = childrenNonComputedUniqueNames;
		}
	}
}
