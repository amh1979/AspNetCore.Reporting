using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.HtmlRenderer
{
	internal class ListLevelStack
	{
		private List<ListLevel> m_listLevels = new List<ListLevel>();

		internal void PushTo(IHtmlReportWriter renderer, int listLevel, RPLFormat.ListStyles style, bool writeNoVerticalMargin)
		{
			if (listLevel == 0)
			{
				this.PopAll();
			}
			else if (this.m_listLevels.Count == 0)
			{
				this.Push(renderer, listLevel, style, writeNoVerticalMargin);
			}
			else
			{
				ListLevel listLevel2 = this.m_listLevels[this.m_listLevels.Count - 1];
				if (listLevel == listLevel2.Level)
				{
					if (style != listLevel2.Style)
					{
						this.Pop();
						this.Push(renderer, listLevel, style, writeNoVerticalMargin);
					}
				}
				else if (listLevel > listLevel2.Level)
				{
					this.Push(renderer, listLevel, style, writeNoVerticalMargin);
				}
				else
				{
					while (listLevel < listLevel2.Level)
					{
						this.Pop();
						if (this.m_listLevels.Count == 0)
						{
							listLevel2 = null;
							break;
						}
						listLevel2 = this.m_listLevels[this.m_listLevels.Count - 1];
					}
					if (listLevel2 != null && listLevel2.Style != style)
					{
						this.Pop();
					}
					this.Push(renderer, listLevel, style, writeNoVerticalMargin);
				}
			}
		}

		internal void Pop()
		{
			if (this.m_listLevels.Count != 0)
			{
				ListLevel listLevel = this.m_listLevels[this.m_listLevels.Count - 1];
				this.m_listLevels.RemoveAt(this.m_listLevels.Count - 1);
				listLevel.Close();
			}
		}

		internal void PopAll()
		{
			for (int num = this.m_listLevels.Count - 1; num > -1; num--)
			{
				this.Pop();
			}
		}

		internal ListLevel Push(IHtmlReportWriter renderer, int listLevel, RPLFormat.ListStyles style, bool writeNoVerticalMarginClass)
		{
			int num = listLevel - this.m_listLevels.Count;
			ListLevel listLevel2 = null;
			while (num > 0)
			{
				listLevel2 = new ListLevel(renderer, this.m_listLevels.Count + 1, style);
				this.m_listLevels.Add(listLevel2);
				listLevel2.Open(writeNoVerticalMarginClass);
				num--;
			}
			return listLevel2;
		}
	}
}
