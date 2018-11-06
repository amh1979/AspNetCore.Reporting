using System;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class GaugeObject : IDisposable
	{
		internal bool initialized = true;

		private object parent;

		private CommonElements common;

		private bool disposed;

		internal virtual object Parent
		{
			get
			{
				return this.parent;
			}
			set
			{
				this.parent = value;
			}
		}

		internal virtual CommonElements Common
		{
			get
			{
				if (this.common == null)
				{
					object obj = this.Parent;
					if (obj is GaugeObject)
					{
						this.common = ((GaugeObject)obj).Common;
					}
					else if (obj is NamedElement)
					{
						this.common = ((NamedElement)obj).Common;
					}
					else if (obj is NamedCollection)
					{
						this.common = ((NamedCollection)obj).Common;
					}
					else if (obj is GaugeCore)
					{
						this.common = ((GaugeCore)obj).Common;
					}
				}
				return this.common;
			}
			set
			{
				this.common = value;
			}
		}

		internal GaugeObject(object parent)
		{
			this.parent = parent;
		}

		internal virtual void Invalidate()
		{
			if (this.Common != null)
			{
				this.Common.GaugeCore.Invalidate();
			}
		}

		internal virtual void Refresh()
		{
			if (this.Common != null)
			{
				this.Common.GaugeCore.Refresh();
			}
		}

		internal virtual void BeginInit()
		{
			this.initialized = false;
		}

		internal virtual void EndInit()
		{
			this.initialized = true;
		}

		internal virtual void ReconnectData(bool exact)
		{
		}

		internal virtual void Notify(MessageType msg, NamedElement element, object param)
		{
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed && disposing)
			{
				this.OnDispose();
			}
			this.disposed = true;
		}

		protected virtual void OnDispose()
		{
		}
	}
}
