using Microsoft.Cloud.Platform.Utils;
using AspNetCore.ReportingServices.DataExtensions;
using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	internal class ObjectModelImpl : OnDemandObjectModel, IConvertible, IStaticReferenceable
	{
		public sealed class SecondaryFieldsCollectionWithAutomaticRestore : IDisposable
		{
			private ObjectModelImpl m_reportOM;

			private FieldsContext m_fieldsContext;

			internal SecondaryFieldsCollectionWithAutomaticRestore(ObjectModelImpl reportOM, FieldsContext fieldsContext)
			{
				this.m_reportOM = reportOM;
				this.m_fieldsContext = fieldsContext;
			}

			public void Dispose()
			{
				this.m_reportOM.RestoreFields(this.m_fieldsContext);
			}
		}

		internal const string NamespacePrefix = "AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel.";

		private FieldsContext m_currentFields;

		private ParametersImpl m_parameters;

		private GlobalsImpl m_globals;

		private UserImpl m_user;

		private ReportItemsImpl m_reportItems;

		private AggregatesImpl m_aggregates;

		private LookupsImpl m_lookups;

		private DataSetsImpl m_dataSets;

		private DataSourcesImpl m_dataSources;

		private VariablesImpl m_variables;

		private OnDemandProcessingContext m_odpContext;

		private int m_id = -2147483648;

		internal virtual bool UseDataSetFieldsCache
		{
			get
			{
				if (!this.m_odpContext.InSubreport)
				{
					return !this.m_odpContext.IsPageHeaderFooter;
				}
				return false;
			}
		}

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

		public override Lookups Lookups
		{
			get
			{
				return this.LookupsImpl;
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

		public override Variables Variables
		{
			get
			{
				return this.VariablesImpl;
			}
		}

		internal FieldsContext CurrentFields
		{
			get
			{
				return this.m_currentFields;
			}
		}

		internal FieldsImpl FieldsImpl
		{
			get
			{
				return this.m_currentFields.Fields;
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

		internal LookupsImpl LookupsImpl
		{
			get
			{
				return this.m_lookups;
			}
			set
			{
				this.m_lookups = value;
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

		internal VariablesImpl VariablesImpl
		{
			get
			{
				return this.m_variables;
			}
			set
			{
				this.m_variables = value;
			}
		}

		internal OnDemandProcessingContext OdpContext
		{
			get
			{
				return this.m_odpContext;
			}
		}

		internal bool AllFieldsCleared
		{
			get
			{
				return this.m_currentFields.AllFieldsCleared;
			}
		}

		int IStaticReferenceable.ID
		{
			get
			{
				return this.m_id;
			}
		}

		internal ObjectModelImpl(OnDemandProcessingContext odpContext)
		{
			this.m_currentFields = null;
			this.m_parameters = null;
			this.m_globals = null;
			this.m_user = null;
			this.m_reportItems = null;
			this.m_aggregates = null;
			this.m_lookups = null;
			this.m_dataSets = null;
			this.m_dataSources = null;
			this.m_odpContext = odpContext;
		}

		internal ObjectModelImpl(ObjectModelImpl copy, OnDemandProcessingContext odpContext)
		{
			this.m_odpContext = odpContext;
			this.m_currentFields = new FieldsContext(this);
			this.m_parameters = copy.m_parameters;
			this.m_globals = new GlobalsImpl(odpContext);
			this.m_user = new UserImpl(copy.m_user, odpContext);
			this.m_dataSets = copy.m_dataSets;
			this.m_dataSources = copy.m_dataSources;
			this.m_reportItems = null;
			this.m_aggregates = null;
			this.m_lookups = null;
		}

		internal void Initialize(DataSetDefinition dataSetDefinition)
		{
			int size = 0;
			if (dataSetDefinition.DataSetCore != null && dataSetDefinition.DataSetCore.Query != null && dataSetDefinition.DataSetCore.Query.Parameters != null)
			{
				size = dataSetDefinition.DataSetCore.Query.Parameters.Count;
			}
			this.m_parameters = new ParametersImpl(size);
			this.InitializeGlobalAndUserCollections();
			this.m_currentFields = new FieldsContext(this, dataSetDefinition.DataSetCore);
			this.m_dataSources = new DataSourcesImpl(0);
			this.m_dataSets = new DataSetsImpl(0);
			this.m_variables = new VariablesImpl(false);
			this.m_aggregates = new AggregatesImpl(false, this.m_odpContext);
			this.m_reportItems = new ReportItemsImpl(false);
			this.m_lookups = new LookupsImpl();
		}

		internal void Initialize(AspNetCore.ReportingServices.ReportIntermediateFormat.Report report, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance)
		{
			int size = 0;
			if (report.Parameters != null)
			{
				size = report.Parameters.Count;
			}
			this.m_parameters = new ParametersImpl(size);
			this.InitializeGlobalAndUserCollections();
			this.m_currentFields = new FieldsContext(this);
			this.m_dataSources = new DataSourcesImpl(report.DataSourceCount);
			this.m_dataSets = new DataSetsImpl(report.DataSetCount);
			this.InitOrUpdateDataSetCollection(report, reportInstance, true);
			this.m_variables = new VariablesImpl(false);
			this.m_aggregates = new AggregatesImpl(false, this.m_odpContext);
			this.m_reportItems = new ReportItemsImpl(false);
			this.m_lookups = new LookupsImpl();
		}

		private void InitializeGlobalAndUserCollections()
		{
			this.m_globals = new GlobalsImpl(this.m_odpContext);
			this.m_user = new UserImpl(this.m_odpContext.RequestUserName, this.m_odpContext.UserLanguage.Name, this.m_odpContext.AllowUserProfileState, this.m_odpContext);
		}

		private int InitOrUpdateDataSetCollection(AspNetCore.ReportingServices.ReportIntermediateFormat.Report report, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance, bool initialize)
		{
			int result = 0;
			for (int i = 0; i < report.DataSourceCount; i++)
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.DataSource dataSource = report.DataSources[i];
				if (initialize && !dataSource.IsArtificialForSharedDataSets)
				{
					this.m_dataSources.Add(dataSource);
				}
				if (dataSource.DataSets != null)
				{
					for (int j = 0; j < dataSource.DataSets.Count; j++)
					{
						this.InitDataSet(reportInstance, dataSource.DataSets[j], ref result);
					}
				}
			}
			return result;
		}

		private void InitDataSet(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance, AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet, ref int dataSetCount)
		{
			DataSetInstance dataSetInstance = null;
			if (reportInstance != null)
			{
				dataSetInstance = reportInstance.GetDataSetInstance(dataSet, this.m_odpContext);
			}
			this.m_dataSets.AddOrUpdate(dataSet, dataSetInstance, this.m_odpContext.ExecutionTime);
			if (!dataSet.UsedOnlyInParameters)
			{
				dataSetCount++;
			}
		}

		internal void Initialize(ParameterInfoCollection parameters)
		{
			this.m_parameters = new ParametersImpl(parameters.Count);
			if (parameters != null && parameters.Count > 0)
			{
				for (int i = 0; i < parameters.Count; i++)
				{
					ParameterInfo parameterInfo = parameters[i];
					this.m_parameters.Add(parameterInfo.Name, new ParameterImpl(parameterInfo));
				}
			}
		}

		internal void SetForNewSubReportContext(ParametersImpl parameters)
		{
			this.m_parameters = parameters;
			if (this.m_variables != null)
			{
				this.m_variables.ResetAll();
			}
			if (this.m_reportItems != null)
			{
				this.m_reportItems.ResetAll();
			}
			if (this.m_aggregates != null)
			{
				this.m_aggregates.ClearAll();
			}
			if (this.m_currentFields != null && this.m_currentFields.Fields != null)
			{
				this.ResetFieldValues();
			}
			this.InitOrUpdateDataSetCollection(this.m_odpContext.ReportDefinition, this.m_odpContext.CurrentReportInstance, false);
		}

		internal void SetupEmptyTopLevelFields()
		{
			this.m_currentFields = new FieldsContext(this);
		}

		internal void SetupPageSectionDataSetFields(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataset)
		{
			this.m_currentFields = new FieldsContext(this, dataset.DataSetCore, false, true);
			this.m_currentFields.Fields.NeedsInlineSetup = true;
		}

		internal void SetupFieldsForNewDataSet(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataset, bool addRowIndex, bool noRows, bool forceNewFieldsContext)
		{
			this.m_currentFields.ResetFieldFlags();
			this.SetupFieldsForNewDataSetWithoutResettingOldFieldFlags(dataset, addRowIndex, noRows, forceNewFieldsContext);
		}

		private void SetupFieldsForNewDataSetWithoutResettingOldFieldFlags(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataset, bool addRowIndex, bool noRows, bool forceNewFieldsContext)
		{
			this.m_currentFields = (this.UseDataSetFieldsCache ? dataset.DataSetCore.FieldsContext : null);
			if (this.m_currentFields != null && this.m_currentFields.Fields.IsCollectionInitialized && !this.m_currentFields.Fields.NeedsInlineSetup && !forceNewFieldsContext)
			{
				return;
			}
			this.m_currentFields = new FieldsContext(this, dataset.DataSetCore, addRowIndex, noRows);
		}

		internal void CreateNoRows()
		{
			this.m_currentFields.CreateNoRows();
		}

		internal void ResetFieldValues()
		{
			if (this.m_odpContext.IsTablixProcessingMode || this.m_odpContext.StreamingMode)
			{
				this.m_currentFields.CreateNoRows();
			}
			else
			{
				this.m_currentFields.CreateNullFieldValues();
			}
		}

		internal void PerformPendingFieldValueUpdate()
		{
			this.m_currentFields.PerformPendingFieldValueUpdate(this, this.UseDataSetFieldsCache);
		}

		internal void RegisterOnDemandFieldValueUpdate(long firstRowOffsetInScope, DataSetInstance dataSetInstance, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.DataChunkReader dataReader)
		{
			this.m_currentFields.RegisterOnDemandFieldValueUpdate(firstRowOffsetInScope, dataSetInstance, dataReader);
		}

		internal void UpdateFieldValues(long firstRowOffsetInScope)
		{
			this.m_currentFields.UpdateFieldValues(this, this.UseDataSetFieldsCache, firstRowOffsetInScope);
		}

		internal void UpdateFieldValues(bool reuseFieldObjects, AspNetCore.ReportingServices.ReportIntermediateFormat.RecordRow row, DataSetInstance dataSetInstance, bool readerExtensionsSupported)
		{
			this.m_currentFields.UpdateFieldValues(this, this.UseDataSetFieldsCache, reuseFieldObjects, row, dataSetInstance, readerExtensionsSupported);
		}

		internal void ResetFieldsUsedInExpression()
		{
			this.FieldsImpl.ResetFieldsUsedInExpression();
			this.AggregatesImpl.ResetFieldsUsedInExpression();
		}

		internal void AddFieldsUsedInExpression(List<string> fieldsUsedInExpression)
		{
			this.FieldsImpl.AddFieldsUsedInExpression(fieldsUsedInExpression);
			this.AggregatesImpl.AddFieldsUsedInExpression(this.m_odpContext, fieldsUsedInExpression);
		}

		internal SecondaryFieldsCollectionWithAutomaticRestore SetupNewFieldsWithBackup(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataset, DataSetInstance dataSetInstance, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.DataChunkReader dataChunkReader)
		{
			SecondaryFieldsCollectionWithAutomaticRestore result = new SecondaryFieldsCollectionWithAutomaticRestore(this, this.m_currentFields);
			bool addRowIndex = this.m_currentFields.Fields.Count != this.m_currentFields.Fields.CountWithRowIndex;
			this.SetupFieldsForNewDataSetWithoutResettingOldFieldFlags(dataset, addRowIndex, dataSetInstance.NoRows, true);
			this.m_currentFields.UpdateDataSetInfo(dataSetInstance, dataChunkReader);
			return result;
		}

		internal void RestoreFields(FieldsContext fieldsContext)
		{
			this.m_currentFields = fieldsContext;
			this.m_currentFields.AttachToDataSetCache(this);
		}

		internal FieldsImpl GetFieldsImplForUpdate(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet currentDataSet)
		{
			if (currentDataSet.DataSetCore != this.m_currentFields.DataSet)
			{
				if (currentDataSet.DataSetCore.FieldsContext != null && this.UseDataSetFieldsCache)
				{
					this.m_currentFields = currentDataSet.DataSetCore.FieldsContext;
				}
				else
				{
					Global.Tracer.Assert(false, "Fields collection is not setup correctly. Actual: " + this.m_currentFields.DataSet.Name.MarkAsPrivate() + " Expected: " + currentDataSet.DataSetCore.Name.MarkAsPrivate());
				}
			}
			return this.m_currentFields.Fields;
		}

		public override bool InScope(string scope)
		{
			return this.m_odpContext.InScope(scope);
		}

		public override int RecursiveLevel(string scope)
		{
			return this.m_odpContext.RecursiveLevel(scope);
		}

		public override object MinValue(params object[] arguments)
		{
			return this.m_odpContext.ReportRuntime.MinValue(arguments);
		}

		public override object MaxValue(params object[] arguments)
		{
			return this.m_odpContext.ReportRuntime.MaxValue(arguments);
		}

		void IStaticReferenceable.SetID(int id)
		{
			this.m_id = id;
		}

		AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType IStaticReferenceable.GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ObjectModelImpl;
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
