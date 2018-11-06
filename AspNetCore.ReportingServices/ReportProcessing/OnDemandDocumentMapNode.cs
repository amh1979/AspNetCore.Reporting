using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal class OnDemandDocumentMapNode
	{
		private string m_label;

		private string m_id;

		private int m_level = -1;

		public string Label
		{
			get
			{
				return this.m_label;
			}
		}

		public string Id
		{
			get
			{
				return this.m_id;
			}
		}

		public int Level
		{
			get
			{
				return this.m_level;
			}
			internal set
			{
				this.m_level = value;
			}
		}

		internal OnDemandDocumentMapNode(string aLabel, string aId, int aLevel)
		{
			this.m_label = aLabel;
			this.m_id = aId;
			this.m_level = aLevel;
		}
	}
}
