using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using System;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal sealed class CommonRowCache : IDisposable
	{
		internal const int UnInitializedRowIndex = -1;

		private ScalableList<DataFieldRow> m_rows;

		internal int Count
		{
			get
			{
				return this.m_rows.Count;
			}
		}

		internal int LastRowIndex
		{
			get
			{
				return this.Count - 1;
			}
		}

		internal CommonRowCache(IScalabilityCache scaleCache)
		{
			this.m_rows = new ScalableList<DataFieldRow>(0, scaleCache, 1000, 100);
		}

		internal int AddRow(DataFieldRow row)
		{
			int count = this.m_rows.Count;
			this.m_rows.Add(row);
			return count;
		}

		internal DataFieldRow GetRow(int index)
		{
			return this.m_rows[index];
		}

		internal void SetupRow(int index, OnDemandProcessingContext odpContext)
		{
			DataFieldRow row = this.GetRow(index);
			row.SetFields(odpContext.ReportObjectModel.FieldsImpl);
		}

		public void Dispose()
		{
			this.m_rows.Dispose();
			this.m_rows = null;
		}
	}
}
