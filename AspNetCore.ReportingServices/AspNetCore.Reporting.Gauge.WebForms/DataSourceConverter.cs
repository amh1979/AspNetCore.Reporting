using System.Collections;
using System.ComponentModel;
using System.Data;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class DataSourceConverter : ReferenceConverter
	{
		public DataSourceConverter()
			: base(typeof(IListSource))
		{
		}

		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return true;
		}

		private void FillList(ArrayList list1, StandardValuesCollection collection1)
		{
			foreach (object item in collection1)
			{
				if (item != null && list1.IndexOf(item) == -1)
				{
					list1.Add(item);
				}
			}
		}

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			ArrayList arrayList = new ArrayList();
			StandardValuesCollection standardValues = base.GetStandardValues(context);
			StandardValuesCollection standardValuesCollection = null;
			this.FillList(arrayList, standardValues);
			ReferenceConverter referenceConverter = null;
			referenceConverter = new ReferenceConverter(typeof(IListSource));
			standardValuesCollection = referenceConverter.GetStandardValues(context);
			this.FillList(arrayList, standardValuesCollection);
			referenceConverter = new ReferenceConverter(typeof(DataView));
			standardValuesCollection = referenceConverter.GetStandardValues(context);
			this.FillList(arrayList, standardValuesCollection);
			referenceConverter = new ReferenceConverter(typeof(IDbDataAdapter));
			standardValuesCollection = referenceConverter.GetStandardValues(context);
			this.FillList(arrayList, standardValuesCollection);
			referenceConverter = new ReferenceConverter(typeof(IDataReader));
			standardValuesCollection = referenceConverter.GetStandardValues(context);
			this.FillList(arrayList, standardValuesCollection);
			referenceConverter = new ReferenceConverter(typeof(IDbCommand));
			standardValuesCollection = referenceConverter.GetStandardValues(context);
			this.FillList(arrayList, standardValuesCollection);
			arrayList.Add(null);
			return new StandardValuesCollection(arrayList);
		}
	}
}
