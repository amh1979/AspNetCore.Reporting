using AspNetCore.ReportingServices.DataProcessing;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using System;
using System.Data;
using System.Diagnostics;

namespace AspNetCore.ReportingServices.DataExtensions
{
	internal class TransactionWrapper : BaseDataWrapper, AspNetCore.ReportingServices.DataProcessing.IDbTransaction, IDisposable
	{
		protected internal System.Data.IDbTransaction UnderlyingTransaction
		{
			get
			{
				return (System.Data.IDbTransaction)base.UnderlyingObject;
			}
		}

		protected internal TransactionWrapper(System.Data.IDbTransaction underlyingTransaction)
			: base(underlyingTransaction)
		{
		}

		public virtual void Commit()
		{
			this.UnderlyingTransaction.Commit();
		}

		public virtual void Rollback()
		{
			if (this.UnderlyingTransaction.Connection != null && this.UnderlyingTransaction.Connection.State != 0 && this.UnderlyingTransaction.Connection.State != ConnectionState.Broken)
			{
				this.UnderlyingTransaction.Rollback();
			}
			else if (RSTrace.DataExtensionTracer.TraceWarning)
			{
				RSTrace.DataExtensionTracer.Trace(TraceLevel.Warning, "TransactionWrapper.Rollback not called, connection is not valid");
			}
		}
	}
}
