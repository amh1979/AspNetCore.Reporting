using System;
using System.Collections;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ProgressivePackaging
{
	internal abstract class ImageMessageReader<T> : IImageMessageReader, IDisposable, IEnumerable<T>, IEnumerable where T : ImageMessageElement
	{
		private bool m_disposed;

		public abstract IEnumerator<T> GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<T>)this).GetEnumerator();
		}

		public abstract void InternalDispose();

		public void Dispose()
		{
			if (!this.m_disposed)
			{
				this.InternalDispose();
				this.m_disposed = true;
			}
		}
	}
}
