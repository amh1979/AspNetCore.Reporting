using AspNetCore.ReportingServices.ReportProcessing;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class ListContent : Group, IDocumentMapEntry
	{
		private ListContentInstance m_listContentInstance;

		private ListContentInstanceInfo m_listContentInstanceInfo;

		private ReportItemCollection m_reportItemCollection;

		public override string DataElementName
		{
			get
			{
				AspNetCore.ReportingServices.ReportProcessing.List list = (AspNetCore.ReportingServices.ReportProcessing.List)base.OwnerDataRegion.ReportItemDef;
				if (list.Grouping == null)
				{
					return list.DataInstanceName;
				}
				return list.Grouping.DataElementName;
			}
		}

		public override DataElementOutputTypes DataElementOutput
		{
			get
			{
				AspNetCore.ReportingServices.ReportProcessing.List list = (AspNetCore.ReportingServices.ReportProcessing.List)base.OwnerDataRegion.ReportItemDef;
				if (list.Grouping == null)
				{
					return list.DataInstanceElementOutput;
				}
				return list.Grouping.DataElementOutput;
			}
		}

		public override string ID
		{
			get
			{
				if (base.OwnerDataRegion.ReportItemDef.RenderingModelID == null)
				{
					base.OwnerDataRegion.ReportItemDef.RenderingModelID = ((AspNetCore.ReportingServices.ReportProcessing.List)base.OwnerDataRegion.ReportItemDef).ListContentID.ToString(CultureInfo.InvariantCulture);
				}
				return base.OwnerDataRegion.ReportItemDef.RenderingModelID;
			}
		}

		public ReportItemCollection ReportItemCollection
		{
			get
			{
				ReportItemCollection reportItemCollection = this.m_reportItemCollection;
				if (this.m_reportItemCollection == null)
				{
					ReportItemColInstance reportItemColInstance = null;
					if (this.m_listContentInstance != null)
					{
						reportItemColInstance = this.m_listContentInstance.ReportItemColInstance;
					}
					reportItemCollection = new ReportItemCollection(((AspNetCore.ReportingServices.ReportProcessing.List)base.OwnerDataRegion.ReportItemDef).ReportItems, reportItemColInstance, base.OwnerDataRegion.RenderingContext, null);
					if (base.OwnerDataRegion.RenderingContext.CacheState)
					{
						this.m_reportItemCollection = reportItemCollection;
					}
				}
				return reportItemCollection;
			}
		}

		public override string Label
		{
			get
			{
				string result = null;
				if (base.m_groupingDef != null && base.m_groupingDef.GroupLabel != null)
				{
					result = ((base.m_groupingDef.GroupLabel.Type != ExpressionInfo.Types.Constant) ? ((this.m_listContentInstance != null) ? this.InstanceInfo.Label : null) : base.m_groupingDef.GroupLabel.Value);
				}
				return result;
			}
		}

		public bool InDocumentMap
		{
			get
			{
				if (this.m_listContentInstance != null && base.m_groupingDef != null)
				{
					return null != base.m_groupingDef.GroupLabel;
				}
				return false;
			}
		}

		public override bool Hidden
		{
			get
			{
				if (this.m_listContentInstance == null)
				{
					return RenderingContext.GetDefinitionHidden(base.OwnerDataRegion.ReportItemDef.Visibility);
				}
				if (base.OwnerDataRegion.ReportItemDef.Visibility == null)
				{
					return false;
				}
				if (base.OwnerDataRegion.ReportItemDef.Visibility.Toggle != null)
				{
					return base.OwnerDataRegion.RenderingContext.IsItemHidden(this.m_listContentInstance.UniqueName, false);
				}
				return this.InstanceInfo.StartHidden;
			}
		}

		public override CustomPropertyCollection CustomProperties
		{
			get
			{
				CustomPropertyCollection customPropertyCollection = base.m_customProperties;
				if (base.m_customProperties == null)
				{
					if (base.m_groupingDef == null || base.m_groupingDef.CustomProperties == null)
					{
						return null;
					}
					customPropertyCollection = ((this.m_listContentInstance != null) ? new CustomPropertyCollection(base.m_groupingDef.CustomProperties, this.InstanceInfo.CustomPropertyInstances) : new CustomPropertyCollection(base.m_groupingDef.CustomProperties, null));
					if (base.OwnerDataRegion.RenderingContext.CacheState)
					{
						base.m_customProperties = customPropertyCollection;
					}
				}
				return customPropertyCollection;
			}
		}

		internal ListContentInstanceInfo InstanceInfo
		{
			get
			{
				if (this.m_listContentInstance == null)
				{
					return null;
				}
				if (this.m_listContentInstanceInfo == null)
				{
					this.m_listContentInstanceInfo = this.m_listContentInstance.GetInstanceInfo(base.OwnerDataRegion.RenderingContext.ChunkManager);
				}
				return this.m_listContentInstanceInfo;
			}
		}

		internal ListContent(List owner, int instanceIndex)
			: base(owner, ((AspNetCore.ReportingServices.ReportProcessing.List)owner.ReportItemDef).Grouping, owner.ReportItemDef.Visibility)
		{
			if (owner.ReportItemInstance != null)
			{
				ListInstance listInstance = (ListInstance)owner.ReportItemInstance;
				ListContentInstanceList listContents = listInstance.ListContents;
				if (listContents != null)
				{
					if (instanceIndex < listContents.Count)
					{
						this.m_listContentInstance = listContents[instanceIndex];
						if (this.m_listContentInstance != null)
						{
							base.m_uniqueName = this.m_listContentInstance.UniqueName;
						}
					}
					else
					{
						Global.Tracer.Assert(0 == listContents.Count);
					}
				}
			}
		}
	}
}
