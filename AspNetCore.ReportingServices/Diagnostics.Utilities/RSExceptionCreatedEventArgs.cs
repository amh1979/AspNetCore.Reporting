using System;
using System.Diagnostics;

namespace AspNetCore.ReportingServices.Diagnostics.Utilities
{
	internal sealed class RSExceptionCreatedEventArgs : EventArgs
	{
		private readonly RSException m_e;

		public RSException Exception
		{
			[DebuggerStepThrough]
			get
			{
				return this.m_e;
			}
		}

		public RSExceptionCreatedEventArgs(RSException exception)
		{
			if (exception == null)
			{
				throw new ArgumentNullException("exception");
			}
			this.m_e = exception;
		}
	}
}
