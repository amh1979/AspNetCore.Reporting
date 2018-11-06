using System;
using System.Collections;


namespace AspNetCore.Reporting
{
	internal sealed class DataSourceWrapper : IDataSource
	{
		//private DataSourceViewSelectCallbackEvent m_selectCompletionEvent;

		private object m_cachedValue;

		private readonly ReportDataSource m_ds;

	public	string Name
		{
			get
			{
				return this.m_ds.Name;
			}
		}

	public	object Value
		{
			get
			{
				return this.GetIDataSource();
			}
		}

		internal DataSourceWrapper(ReportDataSource ds)
		{
			this.m_ds = ds;
			this.StartAsyncSelect();
		}

		private void StartAsyncSelect()
		{
			object value = this.m_ds.Value;
            /*
			if (!(value is ObjectDataSource) && value is System.Web.UI.IDataSource)
			{
				DataSourceView dataSourceView = null;
				if (!string.IsNullOrEmpty(this.m_ds.DataMember))
				{
					dataSourceView = ((System.Web.UI.IDataSource)value).GetView(this.m_ds.DataMember);
				}
				else
				{
					ICollection viewNames = ((System.Web.UI.IDataSource)value).GetViewNames();
					IEnumerator enumerator = viewNames.GetEnumerator();
					if (enumerator != null && enumerator.MoveNext())
					{
						dataSourceView = ((System.Web.UI.IDataSource)value).GetView(enumerator.Current as string);
					}
				}
				if (dataSourceView == null)
				{
					throw new InvalidOperationException(Errors.DataControl_ViewNotFound("", this.m_ds.Name));
				}
				this.m_selectCompletionEvent = new DataSourceViewSelectCallbackEvent(dataSourceView);
				this.m_selectCompletionEvent.InvokeSelect();
			}
            */
		}

		private object GetIDataSource()
		{
			if (this.m_cachedValue != null)
			{
				return this.m_cachedValue;
			}
			object obj = this.m_ds.Value;
            /*
			if (obj is ObjectDataSource)
			{
				obj = (this.m_cachedValue = ((ObjectDataSource)obj).Select());
			}
			else if (obj is System.Web.UI.IDataSource)
			{
				if (this.m_selectCompletionEvent != null)
				{
					this.m_selectCompletionEvent.WaitForSelectCompleted();
					obj = (this.m_cachedValue = this.m_selectCompletionEvent.Data);
				}
				else
				{
					obj = null;
				}
			}
            */
			return obj;
		}
	}
}
