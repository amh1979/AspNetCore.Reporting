using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;

namespace AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	public sealed class ParameterImpl : Parameter
	{
		private object[] m_value;

		private string[] m_labels;

		private bool m_isMultiValue;

		private string m_prompt;

		private int m_hash;

		private bool m_isUserSupplied;

		private bool m_isDataSetQueryParameter;

		public override object Value
		{
			get
			{
				if (this.m_value == null)
				{
					return null;
				}
				if (this.m_isMultiValue && (!this.m_isDataSetQueryParameter || this.m_value.Length != 1))
				{
					return this.m_value;
				}
				return this.m_value[0];
			}
		}

		public override object Label
		{
			get
			{
				if (this.m_labels != null && this.m_labels.Length != 0)
				{
					if (this.m_isMultiValue && (!this.m_isDataSetQueryParameter || this.m_labels.Length != 1))
					{
						return this.m_labels;
					}
					return this.m_labels[0];
				}
				return null;
			}
		}

		public override int Count
		{
			get
			{
				if (this.m_value == null)
				{
					return 0;
				}
				return this.m_value.Length;
			}
		}

		public override bool IsMultiValue
		{
			get
			{
				return this.m_isMultiValue;
			}
		}

		internal bool IsUserSupplied
		{
			get
			{
				return this.m_isUserSupplied;
			}
		}

		internal string Prompt
		{
			get
			{
				return this.m_prompt;
			}
		}

		internal ParameterImpl()
		{
		}

		internal ParameterImpl(ParameterInfo parameterInfo)
		{
			this.m_value = parameterInfo.Values;
			this.m_labels = parameterInfo.Labels;
			this.m_isMultiValue = parameterInfo.MultiValue;
			this.m_prompt = parameterInfo.Prompt;
			this.m_isUserSupplied = parameterInfo.IsUserSupplied;
			if (parameterInfo.ParameterObjectType == ObjectType.QueryParameter)
			{
				this.m_isDataSetQueryParameter = true;
			}
		}

		internal void SetIsMultiValue(bool isMultiValue)
		{
			this.m_isMultiValue = isMultiValue;
		}

		internal void SetIsUserSupplied(bool isUserSupplied)
		{
			this.m_isUserSupplied = isUserSupplied;
		}

		internal void SetValues(object[] values)
		{
			this.m_value = values;
		}

		internal object[] GetValues()
		{
			return this.m_value;
		}

		internal void SetLabels(string[] labels)
		{
			this.m_labels = labels;
		}

		internal string[] GetLabels()
		{
			return this.m_labels;
		}

		internal void SetPrompt(string prompt)
		{
			this.m_prompt = prompt;
		}

		internal bool ValuesAreEqual(ParameterImpl obj)
		{
			if (!this.m_isUserSupplied)
			{
				return true;
			}
			int count = this.Count;
			if (obj != null && count == obj.Count)
			{
				object[] values = obj.GetValues();
				for (int i = 0; i < count; i++)
				{
					if (!object.Equals(this.m_value[i], values[i]))
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		internal int GetValuesHashCode()
		{
			if (!this.m_isUserSupplied)
			{
				return 0;
			}
			if (this.m_hash == 0)
			{
				int count = this.Count;
				this.m_hash = (0x1A03 | count + 1 << 16);
				for (int i = 0; i < count; i++)
				{
					if (this.m_value[i] != null)
					{
						this.m_hash ^= this.m_value[i].GetHashCode();
					}
				}
			}
			return this.m_hash;
		}
	}
}
