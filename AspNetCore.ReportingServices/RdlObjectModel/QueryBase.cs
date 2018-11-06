using System.ComponentModel;
using System.Text.RegularExpressions;

namespace AspNetCore.ReportingServices.RdlObjectModel
{
	internal class QueryBase : ReportObject
	{
		internal class Definition : DefinitionStore<Query, Definition.Properties>
		{
			internal enum Properties
			{
				CommandType,
				CommandText,
				Timeout
			}
		}

		[DefaultValue(CommandTypes.Text)]
		public CommandTypes CommandType
		{
			get
			{
				return (CommandTypes)base.PropertyStore.GetInteger(0);
			}
			set
			{
				base.PropertyStore.SetInteger(0, (int)value);
			}
		}

		public ReportExpression CommandText
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		[ValidValues(0, 2147483647)]
		[DefaultValue(0)]
		public int Timeout
		{
			get
			{
				return base.PropertyStore.GetInteger(2);
			}
			set
			{
				((IntProperty)DefinitionStore<Query, Definition.Properties>.GetProperty(2)).Validate(this, value);
				base.PropertyStore.SetInteger(2, value);
			}
		}

		public QueryBase()
		{
		}

		internal QueryBase(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
		}

		public bool Equals(QueryBase queryBase)
		{
			if (queryBase == null)
			{
				return false;
			}
			if (this.CommandTextEquivalent(this.CommandText, queryBase.CommandText) && this.CommandType == queryBase.CommandType)
			{
				return this.Timeout == queryBase.Timeout;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as QueryBase);
		}

		private bool CommandTextEquivalent(ReportExpression first, ReportExpression second)
		{
			string a = this.FixCommandText(first.ToString());
			string b = this.FixCommandText(second.ToString());
			return a == b;
		}

		private string FixCommandText(string text)
		{
			string input = Regex.Replace(text, "(\\r|\\n)", "");
			return Regex.Replace(input, "^\\s*(.*?)\\s*$", "$1");
		}

		public override int GetHashCode()
		{
			return this.CommandText.GetHashCode();
		}
	}
}
