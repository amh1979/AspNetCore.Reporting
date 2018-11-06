using AspNetCore.ReportingServices.ReportProcessing;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal class TableRow
	{
		internal Table m_owner;

		internal AspNetCore.ReportingServices.ReportProcessing.TableRow m_rowDef;

		internal TableRowInstance m_rowInstance;

		internal TableCellCollection m_rowCells;

		internal TableRowInstanceInfo m_tableRowInstanceInfo;

		public string ID
		{
			get
			{
				if (this.m_rowDef.RenderingModelID == null)
				{
					this.m_rowDef.RenderingModelID = this.m_rowDef.ID.ToString(CultureInfo.InvariantCulture);
				}
				return this.m_rowDef.RenderingModelID;
			}
		}

		public string UniqueName
		{
			get
			{
				if (this.m_rowInstance == null)
				{
					return null;
				}
				return this.m_rowInstance.UniqueName.ToString(CultureInfo.InvariantCulture);
			}
		}

		public object SharedRenderingInfo
		{
			get
			{
				return this.m_owner.RenderingContext.RenderingInfoManager.SharedRenderingInfo[this.m_rowDef.ID];
			}
			set
			{
				this.m_owner.RenderingContext.RenderingInfoManager.SharedRenderingInfo[this.m_rowDef.ID] = value;
			}
		}

		public ReportSize Height
		{
			get
			{
				if (this.m_rowDef.HeightForRendering == null)
				{
					this.m_rowDef.HeightForRendering = new ReportSize(this.m_rowDef.Height, this.m_rowDef.HeightValue);
				}
				return this.m_rowDef.HeightForRendering;
			}
		}

		public TableCellCollection TableCellCollection
		{
			get
			{
				TableCellCollection tableCellCollection = this.m_rowCells;
				if (this.m_rowCells == null)
				{
					tableCellCollection = new TableCellCollection(this.m_owner, this.m_rowDef, this.m_rowInstance);
					if (this.m_owner.RenderingContext.CacheState)
					{
						this.m_rowCells = tableCellCollection;
					}
				}
				return tableCellCollection;
			}
		}

		public virtual bool Hidden
		{
			get
			{
				if (this.m_rowInstance == null)
				{
					return RenderingContext.GetDefinitionHidden(this.m_rowDef.Visibility);
				}
				if (this.m_rowDef.Visibility == null)
				{
					return false;
				}
				if (this.m_rowDef.Visibility.Toggle != null)
				{
					return this.m_owner.RenderingContext.IsItemHidden(this.m_rowInstance.UniqueName, false);
				}
				return this.InstanceInfo.StartHidden;
			}
		}

		public virtual bool HasToggle
		{
			get
			{
				return Visibility.HasToggle(this.m_rowDef.Visibility);
			}
		}

		public virtual string ToggleItem
		{
			get
			{
				if (this.m_rowDef.Visibility == null)
				{
					return null;
				}
				return this.m_rowDef.Visibility.Toggle;
			}
		}

		public virtual TextBox ToggleParent
		{
			get
			{
				if (!this.HasToggle)
				{
					return null;
				}
				if (this.m_rowInstance == null)
				{
					return null;
				}
				return this.m_owner.RenderingContext.GetToggleParent(this.m_rowInstance.UniqueName);
			}
		}

		public virtual SharedHiddenState SharedHidden
		{
			get
			{
				return Visibility.GetSharedHidden(this.m_rowDef.Visibility);
			}
		}

		public virtual bool IsToggleChild
		{
			get
			{
				if (this.m_rowInstance == null)
				{
					return false;
				}
				return this.m_owner.RenderingContext.IsToggleChild(this.m_rowInstance.UniqueName);
			}
		}

		internal TableRowInstanceInfo InstanceInfo
		{
			get
			{
				if (this.m_rowInstance == null)
				{
					return null;
				}
				if (this.m_tableRowInstanceInfo == null)
				{
					this.m_tableRowInstanceInfo = this.m_rowInstance.GetInstanceInfo(this.m_owner.RenderingContext.ChunkManager);
				}
				return this.m_tableRowInstanceInfo;
			}
		}

		internal TableRow(Table owner, AspNetCore.ReportingServices.ReportProcessing.TableRow rowDef, TableRowInstance rowInstance)
		{
			this.m_owner = owner;
			this.m_rowDef = rowDef;
			this.m_rowInstance = rowInstance;
		}
	}
}
