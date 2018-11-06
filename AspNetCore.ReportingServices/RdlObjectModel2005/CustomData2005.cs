using AspNetCore.ReportingServices.RdlObjectModel;

namespace AspNetCore.ReportingServices.RdlObjectModel2005
{
	internal class CustomData2005 : CustomData
	{
		public DataHierarchy DataColumnGroupings
		{
			get
			{
				return base.DataColumnHierarchy;
			}
			set
			{
				base.DataColumnHierarchy = value;
			}
		}

		public DataHierarchy DataRowGroupings
		{
			get
			{
				return base.DataRowHierarchy;
			}
			set
			{
				base.DataRowHierarchy = value;
			}
		}

		public CustomData2005()
		{
		}

		public CustomData2005(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
		}
	}
}
