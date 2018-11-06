using System.Collections;
using System.Threading;

namespace AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel
{
	public sealed class ReportItemsImpl : ReportItems
	{
		internal const string Name = "ReportItems";

		internal const string FullName = "AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel.ReportItems";

		private bool m_lockAdd;

		private Hashtable m_collection;

		private bool m_specialMode;

		private string m_specialModeIndex;

		public override ReportItem this[string key]
		{
			get
			{
				if (key != null && this.m_collection != null)
				{
					try
					{
						if (this.m_specialMode)
						{
							this.m_specialModeIndex = key;
						}
						ReportItem reportItem = this.m_collection[key] as ReportItem;
						if (reportItem == null)
						{
							throw new ReportProcessingException_NonExistingReportItemReference(key);
						}
						return reportItem;
					}
					catch
					{
						throw new ReportProcessingException_NonExistingReportItemReference(key);
					}
				}
				throw new ReportProcessingException_NonExistingReportItemReference(key);
			}
		}

		internal bool SpecialMode
		{
			set
			{
				this.m_specialMode = value;
			}
		}

		internal ReportItemsImpl()
			: this(false)
		{
		}

		internal ReportItemsImpl(bool lockAdd)
		{
			this.m_lockAdd = lockAdd;
			this.m_collection = new Hashtable();
			this.m_specialMode = false;
			this.m_specialModeIndex = null;
		}

		internal void Add(ReportItemImpl reportItem)
		{
			try
			{
				if (this.m_lockAdd)
				{
					Monitor.Enter(this.m_collection);
				}
				this.m_collection.Add(reportItem.Name, reportItem);
			}
			finally
			{
				if (this.m_lockAdd)
				{
					Monitor.Exit(this.m_collection);
				}
			}
		}

		internal string GetSpecialModeIndex()
		{
			string specialModeIndex = this.m_specialModeIndex;
			this.m_specialModeIndex = null;
			return specialModeIndex;
		}
	}
}
