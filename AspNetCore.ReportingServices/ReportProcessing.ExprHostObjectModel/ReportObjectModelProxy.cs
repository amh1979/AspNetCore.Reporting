using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	public abstract class ReportObjectModelProxy : MarshalByRefObject, IReportObjectModelProxyForCustomCode
	{
		private ObjectModel m_reportObjectModel;

		protected Fields Fields
		{
			get
			{
				return this.m_reportObjectModel.Fields;
			}
		}

		protected Parameters Parameters
		{
			get
			{
				return this.m_reportObjectModel.Parameters;
			}
		}

		protected Globals Globals
		{
			get
			{
				return this.m_reportObjectModel.Globals;
			}
		}

		protected User User
		{
			get
			{
				return this.m_reportObjectModel.User;
			}
		}

		protected ReportItems ReportItems
		{
			get
			{
				return this.m_reportObjectModel.ReportItems;
			}
		}

		protected Aggregates Aggregates
		{
			get
			{
				return this.m_reportObjectModel.Aggregates;
			}
		}

		protected DataSets DataSets
		{
			get
			{
				return this.m_reportObjectModel.DataSets;
			}
		}

		protected DataSources DataSources
		{
			get
			{
				return this.m_reportObjectModel.DataSources;
			}
		}

		Parameters IReportObjectModelProxyForCustomCode.Parameters
		{
			get
			{
				return this.Parameters;
			}
		}

		Globals IReportObjectModelProxyForCustomCode.Globals
		{
			get
			{
				return this.Globals;
			}
		}

		User IReportObjectModelProxyForCustomCode.User
		{
			get
			{
				return this.User;
			}
		}

		internal void SetReportObjectModel(ObjectModel reportObjectModel)
		{
			this.m_reportObjectModel = reportObjectModel;
		}

		protected bool InScope(string scope)
		{
			return this.m_reportObjectModel.InScope(scope);
		}

		protected int Level()
		{
			return this.m_reportObjectModel.RecursiveLevel(null);
		}

		protected int Level(string scope)
		{
			return this.m_reportObjectModel.RecursiveLevel(scope);
		}
	}
}
