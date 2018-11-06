using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using System;

namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ReportObjectModelProxy : MarshalByRefObject, IReportObjectModelProxyForCustomCode
	{
		private OnDemandObjectModel m_reportObjectModel;

        protected  Fields Fields
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

        protected Lookups Lookups
		{
			get
			{
				return this.m_reportObjectModel.Lookups;
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

        protected Variables Variables
		{
			get
			{
				return this.m_reportObjectModel.Variables;
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

		Variables IReportObjectModelProxyForCustomCode.Variables
		{
			get
			{
				return this.Variables;
			}
		}

		internal void SetReportObjectModel(OnDemandObjectModel reportObjectModel)
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

		protected object MinValue(params object[] arguments)
		{
			return this.m_reportObjectModel.MinValue(arguments);
		}

		protected object MaxValue(params object[] arguments)
		{
			return this.m_reportObjectModel.MaxValue(arguments);
		}

		protected string CreateDrillthroughContext()
		{
			throw new NotSupportedException();
		}

		public override object InitializeLifetimeService()
		{
			return null;
		}
	}
}
