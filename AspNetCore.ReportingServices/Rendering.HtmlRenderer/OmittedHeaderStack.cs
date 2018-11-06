using System.Collections.Generic;
using System.Text;

namespace AspNetCore.ReportingServices.Rendering.HtmlRenderer
{
	internal class OmittedHeaderStack : Stack<OmittedHeaderData>
	{
		public string GetHeaders(int column, int currentLevel, string idPrefix)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (OmittedHeaderData item in this)
			{
				string text = item.IDs[column];
				if (text != null && item.Level > currentLevel)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(" ");
					}
					if (idPrefix != null)
					{
						stringBuilder.Append(idPrefix);
					}
					stringBuilder.Append(text);
				}
			}
			return stringBuilder.ToString();
		}

		public void PopLevel(int level)
		{
			if (base.Count != 0)
			{
				OmittedHeaderData omittedHeaderData = base.Peek();
				while (omittedHeaderData.Level < level && base.Count != 0)
				{
					base.Pop();
					if (base.Count <= 0)
					{
						break;
					}
					omittedHeaderData = base.Peek();
				}
			}
		}

		public void Push(int level, int column, int colspan, string id, int columnCount)
		{
			this.PopLevel(level);
			OmittedHeaderData omittedHeaderData = null;
			if (base.Count > 0)
			{
				omittedHeaderData = base.Peek();
				if (omittedHeaderData.Level != level)
				{
					omittedHeaderData = null;
				}
			}
			if (omittedHeaderData == null)
			{
				omittedHeaderData = new OmittedHeaderData();
				omittedHeaderData.IDs = new string[columnCount];
				omittedHeaderData.Level = level;
				base.Push(omittedHeaderData);
			}
			int num = column + colspan;
			for (int i = column; i < num; i++)
			{
				omittedHeaderData.IDs[i] = id;
			}
		}
	}
}
