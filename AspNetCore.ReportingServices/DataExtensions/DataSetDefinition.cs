using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.IO;

namespace AspNetCore.ReportingServices.DataExtensions
{
	[Serializable]
	internal sealed class DataSetDefinition
	{
		private string m_description;

		private Guid m_sharedDataSourceReferenceId = Guid.Empty;

		private ParameterInfoCollection m_dataSetParameters;

		private DataSetCore m_dataSetCore;

		public string Description
		{
			get
			{
				return this.m_description;
			}
			set
			{
				this.m_description = value;
			}
		}

		public Guid SharedDataSourceReferenceId
		{
			get
			{
				return this.m_sharedDataSourceReferenceId;
			}
			set
			{
				this.m_sharedDataSourceReferenceId = value;
			}
		}

		public ParameterInfoCollection DataSetParameters
		{
			get
			{
				return this.m_dataSetParameters;
			}
		}

		public DataSetCore DataSetCore
		{
			get
			{
				return this.m_dataSetCore;
			}
		}

		public DataSetDefinition(DataSetCore dataSetCore, string description, DataSourceInfo dataSourceInfo, ParameterInfoCollection dataSetParameters)
		{
			this.m_dataSetCore = dataSetCore;
			this.m_description = description;
			this.m_dataSetParameters = dataSetParameters;
			if (dataSourceInfo != null && dataSourceInfo.IsReference)
			{
				this.m_sharedDataSourceReferenceId = dataSourceInfo.ID;
			}
		}

		public DataSetDefinition(IChunkFactory getCompiledDefinition)
		{
			Global.Tracer.Assert(null != getCompiledDefinition, "Shared dataset definition chunk factory does not exist");
			string text = default(string);
			Stream chunk = getCompiledDefinition.GetChunk("CompiledDefinition", AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.CompiledDefinition, ChunkMode.Open, out text);
			Global.Tracer.Assert(null != chunk, "Shared dataset definition stream does not exist");
			try
			{
				this.m_dataSetCore = (DataSetCore)new IntermediateFormatReader(chunk, (IRIFObjectCreator)new ProcessingRIFObjectCreator(null, null), (GlobalIDOwnerCollection)null).ReadRIFObject();
			}
			finally
			{
				if (chunk != null)
				{
					chunk.Close();
				}
			}
		}
	}
}
