using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.RichText
{
	internal class ParagraphNumberCalculator
	{
		private List<int> m_paragraphLevelList = new List<int>();

		internal void UpdateParagraph(IParagraphProps paragraphProps)
		{
			int listLevel = paragraphProps.ListLevel;
			if (listLevel == 0)
			{
				this.m_paragraphLevelList.Clear();
			}
			else
			{
				int count = this.m_paragraphLevelList.Count;
				if (paragraphProps.ListStyle == RPLFormat.ListStyles.Numbered)
				{
					int num = 1;
					if (count > listLevel)
					{
						this.m_paragraphLevelList.RemoveRange(listLevel, count - listLevel);
						num = this.m_paragraphLevelList[listLevel - 1] + 1;
						this.m_paragraphLevelList[listLevel - 1] = num;
					}
					else if (count == listLevel)
					{
						num = this.m_paragraphLevelList[listLevel - 1] + 1;
						this.m_paragraphLevelList[listLevel - 1] = num;
					}
					else
					{
						for (int i = count; i < listLevel - 1; i++)
						{
							this.m_paragraphLevelList.Add(0);
						}
						this.m_paragraphLevelList.Add(1);
					}
					paragraphProps.ParagraphNumber = num;
				}
				else if (count >= listLevel)
				{
					this.m_paragraphLevelList.RemoveRange(listLevel - 1, count - listLevel + 1);
				}
			}
		}
	}
}
