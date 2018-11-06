using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using System.Collections;
using System.Threading;

namespace AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	public sealed class ReportItemsImpl : ReportItems
	{
		private bool m_lockAdd;

		private Hashtable m_collection;

		private bool m_specialMode;

		private string m_specialModeIndex;

		public override AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel.ReportItem this[string key]
		{
			get
			{
				if (key != null && this.m_collection != null)
				{
					if (this.m_specialMode)
					{
						this.m_specialModeIndex = key;
					}
					AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel.ReportItem reportItem = this.m_collection[key] as AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel.ReportItem;
					if (reportItem == null)
					{
						throw new ReportProcessingException_NonExistingReportItemReference(key);
					}
					return reportItem;
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

		internal ReportItemsImpl(bool lockAdd)
		{
			this.m_lockAdd = lockAdd;
			this.m_collection = new Hashtable();
			this.m_specialMode = false;
			this.m_specialModeIndex = null;
		}

		internal void ResetAll()
		{
			foreach (ReportItemImpl value in this.m_collection.Values)
			{
				value.Reset();
			}
		}

		internal void ResetAll(AspNetCore.ReportingServices.RdlExpressions.VariantResult aResult)
		{
			foreach (ReportItemImpl value in this.m_collection.Values)
			{
				value.Reset(aResult);
			}
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

		internal void AddAll(ReportItemsImpl reportItems)
		{
			foreach (ReportItemImpl value in reportItems.m_collection.Values)
			{
				this.Add(value);
			}
		}

		internal AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel.ReportItem GetReportItem(string aName)
		{
			return this.m_collection[aName] as AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel.ReportItem;
		}

		internal string GetSpecialModeIndex()
		{
			string specialModeIndex = this.m_specialModeIndex;
			this.m_specialModeIndex = null;
			return specialModeIndex;
		}
	}
}
