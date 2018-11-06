using System;

namespace AspNetCore.ReportingServices
{
	internal sealed class RefCountedAppDomain : IDisposable
	{
		private class AppDomainRefCount
		{
			private int m_refCount;

			public void IncrementRefCount()
			{
				lock (this)
				{
					this.m_refCount++;
				}
			}

			public int DecrementRefCount()
			{
				lock (this)
				{
					return --this.m_refCount;
				}
			}
		}

		private AppDomainRefCount m_refCount;

		private AppDomain m_appDomain;

		public AppDomain AppDomain
		{
			get
			{
				return this.m_appDomain;
			}
		}

		public RefCountedAppDomain(AppDomain appDomain)
			: this(appDomain, new AppDomainRefCount())
		{
		}

		private RefCountedAppDomain(AppDomain appDomain, AppDomainRefCount refCount)
		{
			this.m_appDomain = appDomain;
			this.m_refCount = refCount;
			this.m_refCount.IncrementRefCount();
		}

		public RefCountedAppDomain CreateNewReference()
		{
			return new RefCountedAppDomain(this.m_appDomain, this.m_refCount);
		}

		public void Dispose()
		{
			if (this.m_appDomain != null)
			{
				try
				{
					if (this.m_refCount.DecrementRefCount() == 0)
					{
						AppDomain.Unload(this.m_appDomain);
					}
				}
				catch (CannotUnloadAppDomainException)
				{
				}
				finally
				{
					this.m_appDomain = null;
					this.m_refCount = null;
				}
			}
		}
	}
}
