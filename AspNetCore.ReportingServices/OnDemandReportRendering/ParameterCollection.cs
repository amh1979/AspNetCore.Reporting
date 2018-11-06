using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ParameterCollection : ReportElementCollectionBase<Parameter>
	{
		private bool m_isOldSnapshot;

		private List<Parameter> m_list;

		private NameValueCollection m_drillthroughParameters;

		private DrillthroughParameters m_parametersNameObjectCollection;

		private ActionDrillthrough m_actionDef;

		public NameValueCollection ToNameValueCollection
		{
			get
			{
				if (!this.m_isOldSnapshot && this.m_drillthroughParameters == null && this.m_list != null)
				{
					bool[] sharedParameters = default(bool[]);
					this.m_drillthroughParameters = this.ConvertToNameValueCollection(false, out sharedParameters);
					if (0 < this.m_drillthroughParameters.Count)
					{
						this.m_drillthroughParameters.Add("rs:ParameterLanguage", "");
					}
					bool flag = false;
					AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.StoreServerParameters storeServerParameters = this.m_actionDef.Owner.RenderingContext.OdpContext.StoreServerParameters;
					if (storeServerParameters != null)
					{
						string reportName = this.m_actionDef.Instance.ReportName;
						ICatalogItemContext subreportContext = this.m_actionDef.PathResolutionContext.GetSubreportContext(reportName);
						this.m_drillthroughParameters = storeServerParameters(subreportContext, this.m_drillthroughParameters, sharedParameters, out flag);
					}
				}
				return this.m_drillthroughParameters;
			}
		}

		public override Parameter this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					return this.m_list[index];
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		public override int Count
		{
			get
			{
				return this.m_list.Count;
			}
		}

		internal DrillthroughParameters ParametersNameObjectCollection
		{
			get
			{
				if (!this.m_isOldSnapshot && this.m_parametersNameObjectCollection == null && this.m_list != null)
				{
					int count = this.m_list.Count;
					this.m_parametersNameObjectCollection = new DrillthroughParameters(count);
					for (int i = 0; i < count; i++)
					{
						Parameter parameter = this.m_list[i];
						ParameterInstance instance = parameter.Instance;
						if (!instance.Omit)
						{
							this.m_parametersNameObjectCollection.Add(parameter.Name, instance.Value);
						}
					}
				}
				return this.m_parametersNameObjectCollection;
			}
		}

		internal ParameterCollection(ActionDrillthrough actionDef, List<AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterValue> parameters)
		{
			this.m_isOldSnapshot = false;
			this.m_actionDef = actionDef;
			if (parameters == null)
			{
				this.m_list = new List<Parameter>();
			}
			else
			{
				int count = parameters.Count;
				this.m_list = new List<Parameter>(count);
				for (int i = 0; i < count; i++)
				{
					this.m_list.Add(new Parameter(actionDef, parameters[i]));
				}
			}
		}

		internal ParameterCollection(ActionDrillthrough actionDef, NameValueCollection drillthroughParameters, DrillthroughParameters parametersNameObjectCollection, ParameterValueList parameters, ActionItemInstance actionInstance)
		{
			this.m_isOldSnapshot = true;
			this.m_actionDef = actionDef;
			this.m_drillthroughParameters = drillthroughParameters;
			this.m_parametersNameObjectCollection = parametersNameObjectCollection;
			if (parameters == null)
			{
				this.m_list = new List<Parameter>();
			}
			else
			{
				int count = parameters.Count;
				this.m_list = new List<Parameter>(count);
				for (int i = 0; i < count; i++)
				{
					this.m_list.Add(new Parameter(actionDef, parameters[i], actionInstance, i));
				}
			}
		}

		internal NameValueCollection ToNameValueCollectionForDrillthroughEvent()
		{
			bool[] array = default(bool[]);
			return this.ConvertToNameValueCollection(true, out array);
		}

		private NameValueCollection ConvertToNameValueCollection(bool forDrillthroughEvent, out bool[] sharedParams)
		{
			int count = this.m_list.Count;
			NameValueCollection nameValueCollection = new NameValueCollection(count);
			sharedParams = new bool[count];
			for (int i = 0; i < count; i++)
			{
				Parameter parameter = this.m_list[i];
				ParameterInstance instance = parameter.Instance;
				AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterValue parameterDef = parameter.ParameterDef;
				object obj = null;
				if (parameterDef.Value != null && parameterDef.Value.Type == AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Token)
				{
					sharedParams[i] = true;
				}
				else
				{
					sharedParams[i] = false;
				}
				if (!instance.Omit)
				{
					obj = instance.Value;
					if (obj == null)
					{
						nameValueCollection.Add(parameter.Name, null);
					}
					else
					{
						object[] array = obj as object[];
						if (array != null)
						{
							for (int j = 0; j < array.Length; j++)
							{
								nameValueCollection.Add(parameter.Name, this.ConvertValueToString(array[j], forDrillthroughEvent));
							}
						}
						else
						{
							nameValueCollection.Add(parameter.Name, this.ConvertValueToString(obj, forDrillthroughEvent));
						}
					}
				}
			}
			return nameValueCollection;
		}

		private string ConvertValueToString(object value, bool forDrillthroughEvent)
		{
			if (forDrillthroughEvent)
			{
				return Formatter.FormatWithClientCulture(value);
			}
			return Formatter.FormatWithInvariantCulture(value);
		}

		internal Parameter Add(ActionDrillthrough owner, AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterValue paramDef)
		{
			Parameter parameter = new Parameter(owner, paramDef);
			this.m_list.Add(parameter);
			return parameter;
		}

		internal void Update(NameValueCollection drillthroughParameters, DrillthroughParameters nameObjectCollection, ActionItemInstance actionInstance)
		{
			int count = this.m_list.Count;
			for (int i = 0; i < count; i++)
			{
				this.m_list[i].Update(actionInstance, i);
			}
			this.m_parametersNameObjectCollection = nameObjectCollection;
			this.m_drillthroughParameters = drillthroughParameters;
			this.m_parametersNameObjectCollection = nameObjectCollection;
		}

		internal void SetNewContext()
		{
			if (!this.m_isOldSnapshot)
			{
				this.m_drillthroughParameters = null;
				this.m_parametersNameObjectCollection = null;
			}
			if (this.m_list != null)
			{
				for (int i = 0; i < this.m_list.Count; i++)
				{
					this.m_list[i].SetNewContext();
				}
			}
		}

		internal void ConstructParameterDefinitions()
		{
			foreach (Parameter item in this.m_list)
			{
				item.ConstructParameterDefinition();
			}
		}
	}
}
