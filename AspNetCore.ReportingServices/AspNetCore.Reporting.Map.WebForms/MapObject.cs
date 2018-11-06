using System;
using System.Drawing;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class MapObject : IDisposable
	{
		internal bool initialized = true;

		private object parent;

		private CommonElements common;

		protected bool disposed;

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
					if (obj is MapObject)
					{
						this.common = ((MapObject)obj).Common;
					}
					else if (obj is NamedElement)
					{
						this.common = ((NamedElement)obj).Common;
					}
					else if (obj is NamedCollection)
					{
						this.common = ((NamedCollection)obj).Common;
					}
					else if (obj is MapCore)
					{
						this.common = ((MapCore)obj).Common;
					}
				}
				return this.common;
			}
			set
			{
				this.common = value;
			}
		}

		internal MapObject(object parent)
		{
			this.parent = parent;
		}

		internal virtual void Invalidate()
		{
			if (this.Common != null)
			{
				this.Common.MapCore.Invalidate();
			}
		}

		internal virtual void Invalidate(RectangleF rect)
		{
			if (this.Common != null)
			{
				this.Common.MapCore.Invalidate(rect);
			}
		}

		internal virtual void InvalidateViewport(bool invalidateGridSections)
		{
			if (this.Common != null)
			{
				this.Common.MapCore.InvalidateViewport(invalidateGridSections);
			}
		}

		internal virtual void InvalidateDistanceScalePanel()
		{
			if (this.Common != null)
			{
				this.Common.MapCore.InvalidateDistanceScalePanel();
			}
		}

		internal virtual void InvalidateViewport()
		{
			if (this.Common != null)
			{
				this.Common.MapCore.InvalidateViewport(true);
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
