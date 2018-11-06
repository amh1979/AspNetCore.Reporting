using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer
{
	internal sealed class ColumnProperties
	{
		private readonly IColumnModel mModel;

		public bool Hidden
		{
			set
			{
				this.mModel.Hidden = value;
			}
		}

		public bool OutlineCollapsed
		{
			set
			{
				this.mModel.OutlineCollapsed = value;
			}
		}

		public int OutlineLevel
		{
			set
			{
				this.mModel.OutlineLevel = value;
			}
		}

		public double Width
		{
			set
			{
				this.mModel.Width = value;
			}
		}

		internal ColumnProperties(IColumnModel model)
		{
			this.mModel = model;
		}

		public override bool Equals(object obj)
		{
			if (obj != null && obj is ColumnProperties)
			{
				if (obj == this)
				{
					return true;
				}
				ColumnProperties columnProperties = (ColumnProperties)obj;
				return columnProperties.mModel.Equals(this.mModel);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return this.mModel.GetHashCode();
		}
	}
}
