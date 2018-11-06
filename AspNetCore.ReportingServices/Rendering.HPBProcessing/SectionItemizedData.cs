using AspNetCore.ReportingServices.Rendering.RichText;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.HPBProcessing
{
	internal class SectionItemizedData
	{
		internal Dictionary<string, List<TextRunItemizedData>> HeaderFooter;

		private List<Dictionary<string, List<TextRunItemizedData>>> m_columns;

		internal List<Dictionary<string, List<TextRunItemizedData>>> Columns
		{
			get
			{
				if (this.m_columns == null)
				{
					this.m_columns = new List<Dictionary<string, List<TextRunItemizedData>>>();
				}
				return this.m_columns;
			}
		}
	}
}
