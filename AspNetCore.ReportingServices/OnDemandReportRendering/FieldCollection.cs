using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class FieldCollection : ReportElementCollectionBase<Field>
	{
		private Field[] m_collection;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet m_dataSetdef;

		public override int Count
		{
			get
			{
				return this.m_dataSetdef.NonCalculatedFieldCount;
			}
		}

		public override Field this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					if (this.m_collection == null)
					{
						this.m_collection = new Field[this.Count];
					}
					if (this.m_collection[index] == null)
					{
						this.m_collection[index] = new Field(this.m_dataSetdef.Fields[index]);
					}
					return this.m_collection[index];
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		public Field this[string name]
		{
			get
			{
				return ((ReportElementCollectionBase<Field>)this)[this.GetFieldIndex(name)];
			}
		}

		internal FieldCollection(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSetDef)
		{
			this.m_dataSetdef = dataSetDef;
		}

		public int GetFieldIndex(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return -1;
			}
			for (int i = 0; i < this.Count; i++)
			{
				if (string.Equals(name, this.m_dataSetdef.Fields[i].Name, StringComparison.Ordinal))
				{
					return i;
				}
			}
			return -1;
		}
	}
}
