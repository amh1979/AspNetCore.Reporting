using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.HPBProcessing
{
	internal class RTLTextBoxes
	{
		private List<Dictionary<string, List<object>>> m_delayedTB;

		private Stack<Dictionary<string, List<object>>> m_rtlDelayedTB;

		internal List<Dictionary<string, List<object>>> DelayedTB
		{
			get
			{
				return this.m_delayedTB;
			}
			set
			{
				this.m_delayedTB = value;
			}
		}

		internal Stack<Dictionary<string, List<object>>> RTLDelayedTB
		{
			get
			{
				return this.m_rtlDelayedTB;
			}
			set
			{
				this.m_rtlDelayedTB = value;
			}
		}

		internal RTLTextBoxes(List<Dictionary<string, List<object>>> delayedTB)
		{
			this.m_delayedTB = delayedTB;
		}

		internal void Push(List<Dictionary<string, List<object>>> delayedTB)
		{
			if (delayedTB != null && delayedTB.Count != 0)
			{
				if (this.m_rtlDelayedTB == null)
				{
					this.m_rtlDelayedTB = new Stack<Dictionary<string, List<object>>>();
				}
				for (int i = 0; i < delayedTB.Count; i++)
				{
					this.m_rtlDelayedTB.Push(delayedTB[i]);
				}
			}
		}

		internal List<Dictionary<string, List<object>>> RegisterRTLLevel()
		{
			if (this.m_rtlDelayedTB != null && this.m_rtlDelayedTB.Count > 0)
			{
				if (this.m_delayedTB == null)
				{
					this.m_delayedTB = new List<Dictionary<string, List<object>>>();
				}
				while (this.m_rtlDelayedTB.Count > 0)
				{
					this.m_delayedTB.Add(this.m_rtlDelayedTB.Pop());
				}
			}
			return this.m_delayedTB;
		}

		internal void RegisterTextBoxes(PageContext pageContext)
		{
			if (this.m_delayedTB != null)
			{
				for (int i = 0; i < this.m_delayedTB.Count; i++)
				{
					this.RegisterTextBoxes(this.m_delayedTB[i], pageContext);
				}
				this.m_delayedTB = null;
			}
			if (this.m_rtlDelayedTB != null && this.m_rtlDelayedTB.Count > 0)
			{
				while (this.m_rtlDelayedTB.Count > 0)
				{
					this.RegisterTextBoxes(this.m_rtlDelayedTB.Pop(), pageContext);
				}
				this.m_rtlDelayedTB = null;
			}
		}

		private void RegisterTextBoxes(Dictionary<string, List<object>> textBoxValues, PageContext pageContext)
		{
			if (textBoxValues != null && !pageContext.Common.InSubReport)
			{
				foreach (string key in textBoxValues.Keys)
				{
					List<object> list = textBoxValues[key];
					foreach (object item in list)
					{
						pageContext.AddTextBox(key, item);
					}
				}
			}
		}
	}
}
