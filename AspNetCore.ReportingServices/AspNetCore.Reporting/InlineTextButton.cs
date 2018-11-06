using System;
using System.Globalization;


namespace AspNetCore.Reporting
{
	internal sealed class InlineTextButton 
	{
		

		private string m_onClickScript;


		public  string Text
		{
            get;set;
		}

 

		public string Url
		{
            get;set;
		}

		public string OnClickScript
		{
			get
			{
				return this.m_onClickScript;
			}
			set
			{
				this.m_onClickScript = value;
			}
		}

		public event EventHandler Click;

		public InlineTextButton(string id, string text)
		{

		}




		public void RaisePostBackEvent(string eventArgument)
		{
			if (this.Click != null)
			{
				this.Click(this, EventArgs.Empty);
			}
		}
	}
}
