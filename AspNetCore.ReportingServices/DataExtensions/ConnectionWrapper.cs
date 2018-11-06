using AspNetCore.ReportingServices.DataProcessing;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.Interfaces;
using System;
using System.Data;

namespace AspNetCore.ReportingServices.DataExtensions
{
	internal class ConnectionWrapper : BaseDataWrapper, AspNetCore.ReportingServices.DataProcessing.IDbConnection, IDisposable, IExtension
	{
		protected bool m_wrappedManagedProvider;

		public virtual string ConnectionString
		{
			get
			{
				return this.UnderlyingConnection.ConnectionString;
			}
			set
			{
				this.UnderlyingConnection.ConnectionString = value;
			}
		}

		public virtual int ConnectionTimeout
		{
			get
			{
				return this.UnderlyingConnection.ConnectionTimeout;
			}
		}

		public System.Data.IDbConnection UnderlyingConnection
		{
			get
			{
				RSTrace.DataExtensionTracer.Assert(base.UnderlyingObject != null, "If the underlying connection is not provided in the constructor it must be set before accessing it.");
				return (System.Data.IDbConnection)base.UnderlyingObject;
			}
		}

		public virtual string LocalizedName
		{
			get
			{
				return null;
			}
		}

		public bool WrappedManagedProvider
		{
			get
			{
				return this.m_wrappedManagedProvider;
			}
			internal set
			{
				this.m_wrappedManagedProvider = value;
			}
		}

		public ConnectionWrapper(System.Data.IDbConnection underlyingConnection)
			: base(underlyingConnection)
		{
		}

		public virtual void Open()
		{
			this.UnderlyingConnection.Open();
		}

		public virtual void Close()
		{
			this.UnderlyingConnection.Close();
		}

		public virtual AspNetCore.ReportingServices.DataProcessing.IDbCommand CreateCommand()
		{
			return new CommandWrapper(this.UnderlyingConnection.CreateCommand());
		}

		public virtual AspNetCore.ReportingServices.DataProcessing.IDbTransaction BeginTransaction()
		{
			return new TransactionWrapper(this.UnderlyingConnection.BeginTransaction());
		}

		public virtual void SetConfiguration(string configInfo)
		{
		}
	}
}
