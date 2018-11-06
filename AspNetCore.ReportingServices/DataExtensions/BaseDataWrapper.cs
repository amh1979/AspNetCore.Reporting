using AspNetCore.ReportingServices.Diagnostics.Utilities;
using System;

namespace AspNetCore.ReportingServices.DataExtensions
{
	internal abstract class BaseDataWrapper : IDisposable
	{
		private object m_underlyingObject;

		protected object UnderlyingObject
		{
			get
			{
				return this.m_underlyingObject;
			}
			set
			{
				RSTrace.DataExtensionTracer.Assert(this.m_underlyingObject == null, "Should never replace the underlying connection");
				this.m_underlyingObject = value;
			}
		}

		public override bool Equals(object obj)
		{
			if (obj != null && base.GetType() == obj.GetType())
			{
				if (this.m_underlyingObject == null)
				{
					return ((BaseDataWrapper)obj).m_underlyingObject == null;
				}
				return this.m_underlyingObject.Equals(((BaseDataWrapper)obj).m_underlyingObject);
			}
			return false;
		}

		public override int GetHashCode()
		{
			if (this.m_underlyingObject != null)
			{
				return this.m_underlyingObject.GetHashCode();
			}
			return base.GetHashCode();
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		protected BaseDataWrapper(object underlyingObject)
		{
			this.m_underlyingObject = underlyingObject;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				IDisposable disposable = this.m_underlyingObject as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			this.m_underlyingObject = null;
		}
	}
}
