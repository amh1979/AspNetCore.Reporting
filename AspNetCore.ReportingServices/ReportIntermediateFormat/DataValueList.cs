using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal class DataValueList : ArrayList, IList<DataValue>, ICollection<DataValue>, IEnumerable<DataValue>, IEnumerable
	{
		internal new DataValue this[int index]
		{
			get
			{
				return (DataValue)base[index];
			}
		}

		DataValue IList<DataValue>.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				base[index] = value;
			}
		}

		int ICollection<DataValue>.Count
		{
			get
			{
				return this.Count;
			}
		}

		bool ICollection<DataValue>.IsReadOnly
		{
			get
			{
				return this.IsReadOnly;
			}
		}

		public DataValueList()
		{
		}

		internal DataValueList(int capacity)
			: base(capacity)
		{
		}

		internal static string CreatePropertyNameString(string prefix, int rowIndex, int cellIndex, int valueIndex)
		{
			if (rowIndex > 0)
			{
				return prefix + "DataValue(Row:" + rowIndex + ")(Cell:" + cellIndex + ")(Index:" + valueIndex + ")";
			}
			return prefix + "CustomProperty(Index:" + valueIndex + ")";
		}

		internal void Initialize(string prefix, InitializationContext context)
		{
			this.Initialize(prefix, -1, -1, true, context);
		}

		internal void Initialize(string prefix, int rowIndex, int cellIndex, bool isCustomProperty, InitializationContext context)
		{
			int count = this.Count;
			AspNetCore.ReportingServices.ReportPublishing.CustomPropertyUniqueNameValidator validator = new AspNetCore.ReportingServices.ReportPublishing.CustomPropertyUniqueNameValidator();
			for (int i = 0; i < count; i++)
			{
				Global.Tracer.Assert(null != this[i]);
				this[i].Initialize(DataValueList.CreatePropertyNameString(prefix, rowIndex + 1, cellIndex + 1, i + 1), isCustomProperty, validator, context);
			}
		}

		internal void SetExprHost(IList<DataValueExprHost> dataValueHosts, ObjectModelImpl reportObjectModel)
		{
			if (dataValueHosts != null)
			{
				int count = this.Count;
				for (int i = 0; i < count; i++)
				{
					Global.Tracer.Assert(null != this[i]);
					this[i].SetExprHost(dataValueHosts, reportObjectModel);
				}
			}
		}

		int IList<DataValue>.IndexOf(DataValue item)
		{
			return this.IndexOf(item);
		}

		void IList<DataValue>.Insert(int index, DataValue item)
		{
			this.Insert(index, item);
		}

		void IList<DataValue>.RemoveAt(int index)
		{
			this.RemoveAt(index);
		}

		void ICollection<DataValue>.Add(DataValue item)
		{
			this.Add(item);
		}

		void ICollection<DataValue>.Clear()
		{
			this.Clear();
		}

		bool ICollection<DataValue>.Contains(DataValue item)
		{
			return this.Contains(item);
		}

		void ICollection<DataValue>.CopyTo(DataValue[] array, int arrayIndex)
		{
			this.CopyTo(array, arrayIndex);
		}

		bool ICollection<DataValue>.Remove(DataValue item)
		{
			if (!this.Contains(item))
			{
				return false;
			}
			this.Remove(item);
			return true;
		}

		IEnumerator<DataValue> IEnumerable<DataValue>.GetEnumerator()
		{
			foreach (DataValue item in this)
			{
				yield return item;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
