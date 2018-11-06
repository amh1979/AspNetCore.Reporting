using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AspNetCore.Reporting
{
	internal sealed class ReportParameterInfo
	{
		private string m_name;

		private ParameterDataType m_dataType;

		private bool m_isNullable;

		private bool m_allowBlank;

		private bool m_isMultiValue;

		private bool m_isQueryParameter;

		private string m_prompt;

		private bool m_promptUser;

		private bool m_areDefaultValuesQueryBased;

		private bool m_areValidValuesQueryBased;

		private string m_errorMessage;

		private IList<ValidValue> m_validValues;

		private IList<string> m_currentValues;

		private ParameterState m_state;

		private ReportParameterInfoCollection m_dependencyCollection;

		private ReportParameterInfoCollection m_dependentsCollection;

		private string[] m_dependencies;

		private List<ReportParameterInfo> m_dependentsCollectionConstruction = new List<ReportParameterInfo>();

		private bool m_visible;

		internal bool HasUnsatisfiedDownstreamParametersWithDefaults
		{
			get
			{
				foreach (ReportParameterInfo dependent in this.Dependents)
				{
					if (dependent.AreDefaultValuesQueryBased && dependent.State != 0)
					{
						return true;
					}
				}
				return false;
			}
		}

		public string Name
		{
			get
			{
				return this.m_name;
			}
		}

		public ParameterDataType DataType
		{
			get
			{
				return this.m_dataType;
			}
		}

		public bool Nullable
		{
			get
			{
				return this.m_isNullable;
			}
		}

		public bool AllowBlank
		{
			get
			{
				return this.m_allowBlank;
			}
		}

		public bool MultiValue
		{
			get
			{
				return this.m_isMultiValue;
			}
		}

		public bool IsQueryParameter
		{
			get
			{
				return this.m_isQueryParameter;
			}
		}

		public string Prompt
		{
			get
			{
				return this.m_prompt;
			}
		}

		public bool PromptUser
		{
			get
			{
				return this.m_promptUser;
			}
		}

		public ReportParameterInfoCollection Dependencies
		{
			get
			{
				return this.m_dependencyCollection;
			}
		}

		public ReportParameterInfoCollection Dependents
		{
			get
			{
				if (this.m_dependentsCollection == null)
				{
					this.m_dependentsCollection = new ReportParameterInfoCollection(this.m_dependentsCollectionConstruction);
				}
				return this.m_dependentsCollection;
			}
		}

		public bool AreValidValuesQueryBased
		{
			get
			{
				return this.m_areValidValuesQueryBased;
			}
		}

		public IList<ValidValue> ValidValues
		{
			get
			{
				return this.m_validValues;
			}
		}

		public bool AreDefaultValuesQueryBased
		{
			get
			{
				return this.m_areDefaultValuesQueryBased;
			}
		}

		public IList<string> Values
		{
			get
			{
				return this.m_currentValues;
			}
		}

		public ParameterState State
		{
			get
			{
				return this.m_state;
			}
		}

		public string ErrorMessage
		{
			get
			{
				return this.m_errorMessage;
			}
		}

		public bool Visible
		{
			get
			{
				return this.m_visible;
			}
			internal set
			{
				this.m_visible = value;
			}
		}

		internal ReportParameterInfo(string name, ParameterDataType dataType, bool isNullable, bool allowBlank, bool isMultiValue, bool isQueryParameter, string prompt, bool promptUser, bool areDefaultValuesQueryBased, bool areValidValuesQueryBased, string errorMessage, string[] currentValues, IList<ValidValue> validValues, string[] dependencies, ParameterState state)
		{
			this.m_name = name;
			this.m_dataType = dataType;
			this.m_isNullable = isNullable;
			this.m_allowBlank = allowBlank;
			this.m_isMultiValue = isMultiValue;
			this.m_isQueryParameter = isQueryParameter;
			this.m_prompt = prompt;
			this.m_promptUser = promptUser;
			this.m_areDefaultValuesQueryBased = areDefaultValuesQueryBased;
			this.m_areValidValuesQueryBased = areValidValuesQueryBased;
			this.m_errorMessage = errorMessage;
			this.m_currentValues = new ReadOnlyCollection<string>(currentValues ?? new string[0]);
			this.m_validValues = validValues;
			this.m_dependencies = dependencies;
			this.m_state = state;
			this.m_visible = true;
		}

		internal void SetDependencies(ReportParameterInfoCollection coll)
		{
			if (this.m_dependencyCollection == null)
			{
				if (this.m_dependencies == null)
				{
					this.m_dependencyCollection = new ReportParameterInfoCollection();
				}
				else
				{
					List<ReportParameterInfo> list = new List<ReportParameterInfo>();
					string[] dependencies = this.m_dependencies;
					foreach (string name in dependencies)
					{
						ReportParameterInfo reportParameterInfo = coll[name];
						if (reportParameterInfo != null)
						{
							list.Add(reportParameterInfo);
							reportParameterInfo.m_dependentsCollectionConstruction.Add(this);
						}
					}
					this.m_dependencyCollection = new ReportParameterInfoCollection(list);
				}
			}
		}
	}
}
