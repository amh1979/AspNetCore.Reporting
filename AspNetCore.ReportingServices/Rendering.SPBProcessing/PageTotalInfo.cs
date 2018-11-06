using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.SPBProcessing
{
	internal class PageTotalInfo
	{
		private List<KeyValuePair<int, int>> m_regionPageTotals;

		private List<KeyValuePair<int, string>> m_pageNames;

		private bool m_isCounting;

		private bool m_isDone;

		internal bool CalculationDone
		{
			get
			{
				return this.m_isDone;
			}
			set
			{
				this.m_isDone = value;
			}
		}

		internal bool IsCounting
		{
			get
			{
				return this.m_isCounting;
			}
		}

		internal PageTotalInfo(string initialPageName)
		{
			this.m_regionPageTotals = new List<KeyValuePair<int, int>>();
			this.m_pageNames = new List<KeyValuePair<int, string>>();
			if (initialPageName != null)
			{
				this.m_pageNames.Add(new KeyValuePair<int, string>(1, initialPageName));
			}
		}

		internal void RegisterPageNumberForStart(int currentOverallPageNumber)
		{
			if (!this.m_isDone)
			{
				if (!this.m_isCounting)
				{
					this.m_regionPageTotals.Add(new KeyValuePair<int, int>(currentOverallPageNumber, 1));
					this.m_isCounting = true;
				}
				else
				{
					int key = this.m_regionPageTotals[this.m_regionPageTotals.Count - 1].Key;
					int value = this.m_regionPageTotals[this.m_regionPageTotals.Count - 1].Value;
					this.m_regionPageTotals[this.m_regionPageTotals.Count - 1] = new KeyValuePair<int, int>(key, value + 1);
				}
			}
		}

		internal void SetPageName(int currentOverallPageNumber, string pageName)
		{
			if (pageName != null)
			{
				this.m_pageNames.Add(new KeyValuePair<int, string>(currentOverallPageNumber, pageName));
			}
		}

		internal void FinalizePageNumberForTotal()
		{
			if (!this.m_isDone)
			{
				this.m_isCounting = false;
			}
		}

		internal List<KeyValuePair<int, int>> GetPageNumberList()
		{
			return this.m_regionPageTotals;
		}

		internal List<KeyValuePair<int, string>> GetPageNameList()
		{
			return this.m_pageNames;
		}

		internal void RetrievePageBreakData(int currentOverallPageNumber, out int pageNumber, out int totalPages)
		{
			pageNumber = 1;
			totalPages = 1;
			if (this.m_regionPageTotals != null && this.m_regionPageTotals.Count != 0)
			{
				KeyValuePair<int, int> keyValuePair = this.Search(this.m_regionPageTotals, currentOverallPageNumber);
				int key = keyValuePair.Key;
				totalPages = keyValuePair.Value;
				pageNumber = currentOverallPageNumber - key + 1;
			}
		}

		internal string GetPageName(int currentOverallPageNumber)
		{
			if (this.m_pageNames != null && this.m_pageNames.Count != 0)
			{
				if (currentOverallPageNumber < this.m_pageNames[0].Key)
				{
					return null;
				}
				return this.Search(this.m_pageNames, currentOverallPageNumber).Value;
			}
			return null;
		}

		private KeyValuePair<int, TValue> Search<TValue>(List<KeyValuePair<int, TValue>> collection, int currentOverallPageNumber)
		{
			int num = collection.Count;
			KeyValuePair<int, TValue> result = collection[num - 1];
			if (result.Key <= currentOverallPageNumber)
			{
				return result;
			}
			int num2 = 0;
			int num3 = (num - num2) / 2;
			int num4 = 1;
			while (num3 > 0)
			{
				num3 += num2;
				result = collection[num3 - 1];
				if (result.Key == currentOverallPageNumber)
				{
					return result;
				}
				if (result.Key < currentOverallPageNumber)
				{
					num2 = num3;
					num4 = num3;
				}
				else
				{
					num = num3;
				}
				num3 = (num - num2) / 2;
			}
			return collection[num4 - 1];
		}

		internal void SetupPageTotalInfo(bool isCalculationDone, bool isCounting, List<KeyValuePair<int, int>> pageNumberList, List<KeyValuePair<int, string>> pageNameList)
		{
			this.m_regionPageTotals.Clear();
			this.m_regionPageTotals = pageNumberList;
			this.m_pageNames.Clear();
			this.m_pageNames = pageNameList;
			this.m_isCounting = isCounting;
			this.m_isDone = isCalculationDone;
		}
	}
}
