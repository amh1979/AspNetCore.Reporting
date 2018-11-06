using System;

namespace AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel
{
	public sealed class ObjectModelImpl : ObjectModel, IConvertible
	{
		internal const string NamespacePrefix = "AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel.";

		private FieldsImpl m_fields;

		private ParametersImpl m_parameters;

		private GlobalsImpl m_globals;

		private UserImpl m_user;

		private ReportItemsImpl m_reportItems;

		private AggregatesImpl m_aggregates;

		private DataSetsImpl m_dataSets;

		private DataSourcesImpl m_dataSources;

		private ReportProcessing.ProcessingContext m_processingContext;

		public override Fields Fields
		{
			get
			{
				return this.FieldsImpl;
			}
		}

		public override Parameters Parameters
		{
			get
			{
				return this.ParametersImpl;
			}
		}

		public override Globals Globals
		{
			get
			{
				return this.GlobalsImpl;
			}
		}

		public override User User
		{
			get
			{
				return this.UserImpl;
			}
		}

		public override ReportItems ReportItems
		{
			get
			{
				return this.ReportItemsImpl;
			}
		}

		public override Aggregates Aggregates
		{
			get
			{
				return this.AggregatesImpl;
			}
		}

		public override DataSets DataSets
		{
			get
			{
				return this.DataSetsImpl;
			}
		}

		public override DataSources DataSources
		{
			get
			{
				return this.DataSourcesImpl;
			}
		}

		internal FieldsImpl FieldsImpl
		{
			get
			{
				return this.m_fields;
			}
			set
			{
				this.m_fields = value;
			}
		}

		internal ParametersImpl ParametersImpl
		{
			get
			{
				return this.m_parameters;
			}
			set
			{
				this.m_parameters = value;
			}
		}

		internal GlobalsImpl GlobalsImpl
		{
			get
			{
				return this.m_globals;
			}
			set
			{
				this.m_globals = value;
			}
		}

		internal UserImpl UserImpl
		{
			get
			{
				return this.m_user;
			}
			set
			{
				this.m_user = value;
			}
		}

		internal ReportItemsImpl ReportItemsImpl
		{
			get
			{
				return this.m_reportItems;
			}
			set
			{
				this.m_reportItems = value;
			}
		}

		internal AggregatesImpl AggregatesImpl
		{
			get
			{
				return this.m_aggregates;
			}
			set
			{
				this.m_aggregates = value;
			}
		}

		internal DataSetsImpl DataSetsImpl
		{
			get
			{
				return this.m_dataSets;
			}
			set
			{
				this.m_dataSets = value;
			}
		}

		internal DataSourcesImpl DataSourcesImpl
		{
			get
			{
				return this.m_dataSources;
			}
			set
			{
				this.m_dataSources = value;
			}
		}

		internal ObjectModelImpl(ReportProcessing.ProcessingContext processingContext)
		{
			this.m_fields = null;
			this.m_parameters = null;
			this.m_globals = null;
			this.m_user = null;
			this.m_reportItems = null;
			this.m_aggregates = null;
			this.m_dataSets = null;
			this.m_dataSources = null;
			this.m_processingContext = processingContext;
		}

		internal ObjectModelImpl(ObjectModelImpl copy, ReportProcessing.ProcessingContext processingContext)
		{
			this.m_fields = null;
			this.m_parameters = copy.m_parameters;
			this.m_globals = copy.m_globals;
			this.m_user = copy.m_user;
			this.m_reportItems = copy.m_reportItems;
			this.m_aggregates = copy.m_aggregates;
			this.m_dataSets = copy.m_dataSets;
			this.m_dataSources = copy.m_dataSources;
			this.m_processingContext = processingContext;
		}

		public override bool InScope(string scope)
		{
			return this.m_processingContext.ReportRuntime.InScope(scope);
		}

		public override int RecursiveLevel(string scope)
		{
			return this.m_processingContext.ReportRuntime.RecursiveLevel(scope);
		}

		TypeCode IConvertible.GetTypeCode()
		{
			return TypeCode.Object;
		}

		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		byte IConvertible.ToByte(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		char IConvertible.ToChar(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		double IConvertible.ToDouble(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		short IConvertible.ToInt16(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		int IConvertible.ToInt32(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		long IConvertible.ToInt64(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		float IConvertible.ToSingle(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		string IConvertible.ToString(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		object IConvertible.ToType(Type conversionType, IFormatProvider provider)
		{
			if (conversionType == typeof(ObjectModel))
			{
				return this;
			}
			throw new NotSupportedException();
		}

		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}
	}
}
