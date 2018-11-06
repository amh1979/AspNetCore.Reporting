using AspNetCore.ReportingServices.DataExtensions;
using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class PublishingResult
	{
		private string m_reportDescription;

		private string m_reportLanguage;

		private ParameterInfoCollection m_parameters;

		private DataSourceInfoCollection m_dataSources;

		private ProcessingMessageList m_warnings;

		private UserLocationFlags m_userReferenceLocation;

		private PageProperties m_pageProperties;

		private string[] m_dataSetsName;

		private bool m_hasExternalImages;

		private bool m_hasHyperlinks;

		private ReportProcessingFlags m_reportProcessingFlags;

		private byte[] m_dataSetsHash;

		private DataSetInfoCollection m_sharedDataSets;

		public string ReportDescription
		{
			get
			{
				return this.m_reportDescription;
			}
		}

		public string ReportLanguage
		{
			get
			{
				return this.m_reportLanguage;
			}
		}

		public bool HasUserProfileQueryDependencies
		{
			get
			{
				if ((this.m_userReferenceLocation & UserLocationFlags.ReportQueries) == (UserLocationFlags)0)
				{
					return false;
				}
				return true;
			}
		}

		public bool HasUserProfileReportDependencies
		{
			get
			{
				if ((this.m_userReferenceLocation & UserLocationFlags.ReportBody) == (UserLocationFlags)0 && (this.m_userReferenceLocation & UserLocationFlags.ReportPageSection) == (UserLocationFlags)0)
				{
					return false;
				}
				return true;
			}
		}

		public ParameterInfoCollection Parameters
		{
			get
			{
				return this.m_parameters;
			}
		}

		public DataSourceInfoCollection DataSources
		{
			get
			{
				return this.m_dataSources;
			}
		}

		public ProcessingMessageList Warnings
		{
			get
			{
				return this.m_warnings;
			}
		}

		public PageProperties PageProperties
		{
			get
			{
				return this.m_pageProperties;
			}
		}

		public string[] DataSetsName
		{
			get
			{
				return this.m_dataSetsName;
			}
		}

		public bool HasExternalImages
		{
			get
			{
				return this.m_hasExternalImages;
			}
		}

		public bool HasHyperlinks
		{
			get
			{
				return this.m_hasHyperlinks;
			}
		}

		public ReportProcessingFlags ReportProcessingFlags
		{
			get
			{
				return this.m_reportProcessingFlags;
			}
		}

		public byte[] DataSetsHash
		{
			get
			{
				return this.m_dataSetsHash;
			}
		}

		public DataSetInfoCollection SharedDataSets
		{
			get
			{
				return this.m_sharedDataSets;
			}
		}

		internal PublishingResult(string reportDescription, string reportLanguage, ParameterInfoCollection parameters, DataSourceInfoCollection dataSources, DataSetInfoCollection sharedDataSetReferences, ProcessingMessageList warnings, UserLocationFlags userReferenceLocation, double pageHeight, double pageWidth, double topMargin, double bottomMargin, double leftMargin, double rightMargin, ArrayList dataSetsName, bool hasExternalImages, bool hasHyperlinks, ReportProcessingFlags reportProcessingFlags, byte[] dataSetsHash)
		{
			this.m_reportDescription = reportDescription;
			this.m_reportLanguage = reportLanguage;
			this.m_parameters = parameters;
			this.m_dataSources = dataSources;
			this.m_sharedDataSets = sharedDataSetReferences;
			this.m_warnings = warnings;
			this.m_userReferenceLocation = userReferenceLocation;
			this.m_hasExternalImages = hasExternalImages;
			this.m_hasHyperlinks = hasHyperlinks;
			this.m_reportProcessingFlags = reportProcessingFlags;
			this.m_dataSetsHash = dataSetsHash;
			this.m_pageProperties = new PageProperties(pageHeight, pageWidth, topMargin, bottomMargin, leftMargin, rightMargin);
			if (dataSetsName != null && dataSetsName.Count > 0)
			{
				this.m_dataSetsName = (string[])dataSetsName.ToArray(typeof(string));
			}
		}
	}
}
