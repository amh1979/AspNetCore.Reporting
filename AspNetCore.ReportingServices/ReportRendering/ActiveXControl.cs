using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Specialized;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class ActiveXControl : ReportItem
	{
		internal sealed class Parameter
		{
			private string m_name;

			private object m_value;

			public string Name
			{
				get
				{
					return this.m_name;
				}
			}

			public object Value
			{
				get
				{
					return this.m_value;
				}
			}

			internal Parameter(string name, object value)
			{
				this.m_name = name;
				this.m_value = value;
			}
		}

		internal sealed class ParameterCollection : NameObjectCollectionBase
		{
			public Parameter this[int index]
			{
				get
				{
					return (Parameter)base.BaseGet(index);
				}
			}

			public Parameter this[string name]
			{
				get
				{
					return (Parameter)base.BaseGet(name);
				}
			}

			internal ParameterCollection()
			{
			}

			internal void Add(Parameter parameter)
			{
				base.BaseAdd(parameter.Name, parameter);
			}
		}

		private ParameterCollection m_parameters;

		private ReportUrl m_codeBase;

		public string ClassID
		{
			get
			{
				return ((AspNetCore.ReportingServices.ReportProcessing.ActiveXControl)base.ReportItemDef).ClassID;
			}
		}

		public ReportUrl CodeBase
		{
			get
			{
				string codeBase = ((AspNetCore.ReportingServices.ReportProcessing.ActiveXControl)base.ReportItemDef).CodeBase;
				if (codeBase == null)
				{
					return null;
				}
				ReportUrl reportUrl = this.m_codeBase;
				if (this.m_codeBase == null)
				{
					reportUrl = new ReportUrl(base.RenderingContext, codeBase);
					if (base.RenderingContext.CacheState)
					{
						this.m_codeBase = reportUrl;
					}
				}
				return reportUrl;
			}
		}

		public ParameterCollection Parameters
		{
			get
			{
				ParameterCollection parameterCollection = this.m_parameters;
				if (this.m_parameters == null)
				{
					AspNetCore.ReportingServices.ReportProcessing.ActiveXControl activeXControl = (AspNetCore.ReportingServices.ReportProcessing.ActiveXControl)base.ReportItemDef;
					if (activeXControl.Parameters != null && activeXControl.Parameters.Count > 0)
					{
						parameterCollection = new ParameterCollection();
						for (int i = 0; i < activeXControl.Parameters.Count; i++)
						{
							ParameterValue parameterValue = activeXControl.Parameters[i];
							object value = (parameterValue.Value.Type != ExpressionInfo.Types.Constant) ? ((base.ReportItemInstance != null) ? ((ActiveXControlInstanceInfo)base.InstanceInfo).ParameterValues[i] : null) : parameterValue.Value.Value;
							parameterCollection.Add(new Parameter(parameterValue.Name, value));
						}
						if (base.RenderingContext.CacheState)
						{
							this.m_parameters = parameterCollection;
						}
					}
				}
				return parameterCollection;
			}
		}

		internal ActiveXControl(string uniqueName, int intUniqueName, AspNetCore.ReportingServices.ReportProcessing.ActiveXControl reportItemDef, ActiveXControlInstance reportItemInstance, RenderingContext renderingContext)
			: base(uniqueName, intUniqueName, reportItemDef, reportItemInstance, renderingContext)
		{
		}
	}
}
