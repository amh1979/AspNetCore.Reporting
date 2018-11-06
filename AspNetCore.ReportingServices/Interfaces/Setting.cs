using System.Collections;

namespace AspNetCore.ReportingServices.Interfaces
{
	internal class Setting
	{
		private string m_name;

		private string m_displayName;

		private string m_value;

		private bool m_required;

		private bool m_readOnly;

		private string m_field;

		private string m_error;

		private bool m_encrypted;

		private bool m_isPassword;

		private ArrayList m_validValues = new ArrayList();

		public string Name
		{
			get
			{
				return this.m_name;
			}
			set
			{
				this.m_name = value;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.m_displayName;
			}
			set
			{
				this.m_displayName = value;
			}
		}

		public string Value
		{
			get
			{
				return this.m_value;
			}
			set
			{
				this.m_value = value;
			}
		}

		public bool Required
		{
			get
			{
				return this.m_required;
			}
			set
			{
				this.m_required = value;
			}
		}

		public bool ReadOnly
		{
			get
			{
				return this.m_readOnly;
			}
			set
			{
				this.m_readOnly = value;
			}
		}

		public string Field
		{
			get
			{
				return this.m_field;
			}
			set
			{
				this.m_field = value;
			}
		}

		public string Error
		{
			get
			{
				return this.m_error;
			}
			set
			{
				this.m_error = value;
			}
		}

		public ValidValue[] ValidValues
		{
			get
			{
				return this.m_validValues.ToArray(typeof(ValidValue)) as ValidValue[];
			}
			set
			{
				if (value == null)
				{
					this.m_validValues = new ArrayList();
				}
				else
				{
					this.m_validValues = new ArrayList(value);
				}
			}
		}

		public bool Encrypted
		{
			get
			{
				return this.m_encrypted;
			}
			set
			{
				this.m_encrypted = value;
			}
		}

		public bool IsPassword
		{
			get
			{
				return this.m_isPassword;
			}
			set
			{
				this.m_isPassword = value;
			}
		}

		public void AddValidValue(ValidValue val)
		{
			this.m_validValues.Add(val);
		}

		public void AddValidValue(string label, string val)
		{
			ValidValue validValue = new ValidValue();
			validValue.Value = val;
			validValue.Label = label;
			this.AddValidValue(validValue);
		}
	}
}
