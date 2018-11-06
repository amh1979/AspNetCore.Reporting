using System;

namespace AspNetCore.ReportingServices.DataExtensions
{
	[Serializable]
	internal sealed class DataSetInfo
	{
		private Guid m_id = Guid.Empty;

		private Guid m_linkSharedDataSetId = Guid.Empty;

		private string m_absolutePath;

		private string m_dataSetName;

		private byte[] m_secDesc;

		private Guid m_compiledDefinitionId = Guid.Empty;

		private Guid m_dataSourceId = Guid.Empty;

		private string m_parameters;

		private bool m_referenceValid;

		private readonly byte[] m_definition;

		public Guid ID
		{
			get
			{
				return this.m_id;
			}
			set
			{
				this.m_id = value;
			}
		}

		public Guid LinkedSharedDataSetID
		{
			get
			{
				return this.m_linkSharedDataSetId;
			}
			set
			{
				this.m_linkSharedDataSetId = value;
			}
		}

		public string AbsolutePath
		{
			get
			{
				return this.m_absolutePath;
			}
			set
			{
				this.m_absolutePath = value;
			}
		}

		public string DataSetName
		{
			get
			{
				return this.m_dataSetName;
			}
			set
			{
				this.m_dataSetName = value;
			}
		}

		public byte[] SecurityDescriptor
		{
			get
			{
				return this.m_secDesc;
			}
		}

		public Guid CompiledDefinitionId
		{
			get
			{
				return this.m_compiledDefinitionId;
			}
		}

		public Guid DataSourceId
		{
			get
			{
				return this.m_dataSourceId;
			}
			set
			{
				this.m_dataSourceId = value;
			}
		}

		public byte[] Definition
		{
			get
			{
				return this.m_definition;
			}
		}

		public string ParametersXml
		{
			get
			{
				return this.m_parameters;
			}
		}

		public DataSetInfo(Guid id, Guid linkId, string name, string absolutePath, byte[] secDesc, Guid compiledDefinitionId, string parameters)
		{
			this.m_id = id;
			this.m_linkSharedDataSetId = linkId;
			this.m_dataSetName = name;
			this.m_absolutePath = absolutePath;
			this.m_secDesc = secDesc;
			this.m_compiledDefinitionId = compiledDefinitionId;
			this.m_referenceValid = (this.m_linkSharedDataSetId != Guid.Empty);
			this.m_parameters = parameters;
		}

		public DataSetInfo(string reportDataSetName, string absolutePath, Guid linkId)
		{
			this.m_id = Guid.NewGuid();
			this.m_dataSetName = reportDataSetName;
			this.m_absolutePath = absolutePath;
			this.m_linkSharedDataSetId = linkId;
			this.m_referenceValid = (this.m_linkSharedDataSetId != Guid.Empty);
		}

		public DataSetInfo(string reportDataSetName, string absolutePath)
		{
			this.m_id = Guid.NewGuid();
			this.m_dataSetName = reportDataSetName;
			this.m_absolutePath = absolutePath;
			this.m_linkSharedDataSetId = Guid.Empty;
			this.m_referenceValid = false;
		}

		public DataSetInfo(string reportDataSetName, string absolutePath, byte[] definition)
		{
			if (definition == null)
			{
				throw new ArgumentNullException("definition");
			}
			this.m_definition = definition;
			this.m_id = Guid.NewGuid();
			this.m_dataSetName = reportDataSetName;
			this.m_absolutePath = absolutePath;
			this.m_linkSharedDataSetId = Guid.Empty;
			this.m_referenceValid = true;
		}

		public bool IsValidReference()
		{
			return this.m_referenceValid;
		}
	}
}
