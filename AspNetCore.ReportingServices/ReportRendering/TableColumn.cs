using AspNetCore.ReportingServices.ReportProcessing;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class TableColumn
	{
		private Table m_owner;

		private AspNetCore.ReportingServices.ReportProcessing.TableColumn m_columnDef;

		private TableColumnInstance m_columnInstance;

		private int m_index;

		public string UniqueName
		{
			get
			{
				if (this.ColumnInstance == null)
				{
					return null;
				}
				return this.ColumnInstance.UniqueName.ToString(CultureInfo.InvariantCulture);
			}
		}

		public ReportSize Width
		{
			get
			{
				if (this.m_columnDef.WidthForRendering == null)
				{
					this.m_columnDef.WidthForRendering = new ReportSize(this.m_columnDef.Width, this.m_columnDef.WidthValue);
				}
				return this.m_columnDef.WidthForRendering;
			}
		}

		public bool Hidden
		{
			get
			{
				if (this.ColumnInstance == null)
				{
					return RenderingContext.GetDefinitionHidden(this.m_columnDef.Visibility);
				}
				if (this.m_columnDef.Visibility == null)
				{
					return false;
				}
				if (this.m_columnDef.Visibility.Toggle != null)
				{
					return this.m_owner.RenderingContext.IsItemHidden(this.ColumnInstance.UniqueName, false);
				}
				return this.ColumnInstance.StartHidden;
			}
		}

		public bool HasToggle
		{
			get
			{
				return Visibility.HasToggle(this.m_columnDef.Visibility);
			}
		}

		public string ToggleItem
		{
			get
			{
				if (this.m_columnDef.Visibility == null)
				{
					return null;
				}
				return this.m_columnDef.Visibility.Toggle;
			}
		}

		public TextBox ToggleParent
		{
			get
			{
				if (!this.HasToggle)
				{
					return null;
				}
				if (this.ColumnInstance == null)
				{
					return null;
				}
				return this.m_owner.RenderingContext.GetToggleParent(this.ColumnInstance.UniqueName);
			}
		}

		public SharedHiddenState SharedHidden
		{
			get
			{
				return Visibility.GetSharedHidden(this.m_columnDef.Visibility);
			}
		}

		public bool IsToggleChild
		{
			get
			{
				if (this.ColumnInstance == null)
				{
					return false;
				}
				return this.m_owner.RenderingContext.IsToggleChild(this.ColumnInstance.UniqueName);
			}
		}

		internal TableColumnInstance ColumnInstance
		{
			get
			{
				if (this.m_columnInstance == null)
				{
					TableInstanceInfo tableInstanceInfo = (TableInstanceInfo)this.m_owner.InstanceInfo;
					if (tableInstanceInfo != null)
					{
						TableColumnInstance[] columnInstances = tableInstanceInfo.ColumnInstances;
						if (columnInstances != null)
						{
							this.m_columnInstance = columnInstances[this.m_index];
						}
					}
				}
				return this.m_columnInstance;
			}
		}

		public bool FixedHeader
		{
			get
			{
				return this.m_columnDef.FixedHeader;
			}
		}

		internal AspNetCore.ReportingServices.ReportProcessing.TableColumn ColumnDefinition
		{
			get
			{
				return this.m_columnDef;
			}
		}

		internal TableColumn(Table owner, AspNetCore.ReportingServices.ReportProcessing.TableColumn columnDef, int index)
		{
			this.m_owner = owner;
			this.m_columnDef = columnDef;
			this.m_index = index;
		}
	}
}
