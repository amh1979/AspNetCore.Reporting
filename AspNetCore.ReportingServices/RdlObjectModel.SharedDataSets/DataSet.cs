using System.Collections.Generic;
using System.Xml.Serialization;

namespace AspNetCore.ReportingServices.RdlObjectModel.SharedDataSets
{
	internal class DataSet : DataSetBase
	{
		internal new class Definition : DefinitionStore<DataSet, Definition.Properties>
		{
			internal enum Properties
			{
				Name,
				CaseSensitivity,
				Collation,
				AccentSensitivity,
				KanatypeSensitivity,
				WidthSensitivity,
				InterpretSubtotalsAsDetails,
				Query,
				Fields,
				Filters
			}
		}

		public Query Query
		{
			get
			{
				return (Query)base.PropertyStore.GetObject(7);
			}
			set
			{
				base.PropertyStore.SetObject(7, value);
			}
		}

		[XmlArrayItem("Field", typeof(Field), Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2010/01/shareddatasetdefinition")]
		[XmlElement(typeof(RdlCollection<Field>))]
		public IList<Field> Fields
		{
			get
			{
				return (IList<Field>)base.PropertyStore.GetObject(8);
			}
			set
			{
				base.PropertyStore.SetObject(8, value);
			}
		}

		[XmlArrayItem("Filter", typeof(Filter), Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2010/01/shareddatasetdefinition")]
		[XmlElement(typeof(RdlCollection<Filter>))]
		public IList<Filter> Filters
		{
			get
			{
				return (IList<Filter>)base.PropertyStore.GetObject(9);
			}
			set
			{
				base.PropertyStore.SetObject(9, value);
			}
		}

		public DataSet()
		{
		}

		internal DataSet(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public bool Equals(DataSet dataSet)
		{
			if (dataSet == null)
			{
				return false;
			}
			if (this.Query == null && dataSet.Query != null)
			{
				goto IL_0038;
			}
			if (this.Query != null && dataSet.Query == null)
			{
				goto IL_0038;
			}
			if (!this.Query.Equals(dataSet.Query))
			{
				goto IL_0038;
			}
			if (!this.FieldsAreEqual(this.Fields, dataSet.Fields))
			{
				return false;
			}
			if (!this.FiltersAreEqual(this.Filters, dataSet.Filters))
			{
				return false;
			}
			if (!base.Equals(dataSet))
			{
				return false;
			}
			return true;
			IL_0038:
			return false;
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as DataSet);
		}

		public override int GetHashCode()
		{
			if (this.Query != null)
			{
				return this.Query.GetHashCode();
			}
			return base.GetHashCode();
		}

		private bool FieldsAreEqual(IList<Field> FirstList, IList<Field> SecondList)
		{
			if (FirstList.Count != SecondList.Count)
			{
				return false;
			}
			for (int i = 0; i < FirstList.Count; i++)
			{
				if (!FirstList[i].Equals(SecondList[i]))
				{
					return false;
				}
			}
			return true;
		}

		private bool FiltersAreEqual(IList<Filter> FirstList, IList<Filter> SecondList)
		{
			if (FirstList.Count != SecondList.Count)
			{
				return false;
			}
			for (int i = 0; i < FirstList.Count; i++)
			{
				if (!FirstList[i].Equals(SecondList[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override QueryBase GetQuery()
		{
			return this.Query;
		}

		public override void Initialize()
		{
			base.Initialize();
			this.Query = new Query();
			this.Fields = new RdlCollection<Field>();
			this.Filters = new RdlCollection<Filter>();
		}
	}
}
