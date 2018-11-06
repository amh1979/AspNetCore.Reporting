using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class TextBoxList : ArrayList
	{
		internal new TextBox this[int index]
		{
			get
			{
				return (TextBox)base[index];
			}
		}

		internal TextBoxList()
		{
		}

		internal TextBoxList(int capacity)
			: base(capacity)
		{
		}
	}
}
