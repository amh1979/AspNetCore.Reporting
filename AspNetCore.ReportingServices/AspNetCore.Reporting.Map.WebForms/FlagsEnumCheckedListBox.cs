using System;
using System.Windows.Forms;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class FlagsEnumCheckedListBox : CheckedListBox
	{
		private object editValue;

		private Type editType;

		public FlagsEnumCheckedListBox(object editValue, Type editType)
		{
			this.editValue = editValue;
			this.editType = editType;
			base.BorderStyle = BorderStyle.None;
			this.FillList();
		}

		private void FillList()
		{
			if (!this.editType.IsEnum)
			{
				throw new ArgumentException("UI type editor may be set for the enumerations only.");
			}
			if (Enum.GetUnderlyingType(this.editType) != typeof(int))
			{
				throw new ArgumentException("UI type editor may be set for the enumerations with Int32 underlying type only.");
			}
			int num = 0;
			if (this.editValue != null)
			{
				num = (int)this.editValue;
			}
			foreach (object value in Enum.GetValues(this.editType))
			{
				int num2 = (int)value;
				if (num2 != 0)
				{
					bool isChecked = (num & num2) == num2;
					base.Items.Add(Enum.GetName(this.editType, value), isChecked);
				}
			}
		}

		public object GetNewValue()
		{
			int num = 0;
			foreach (object checkedItem in base.CheckedItems)
			{
				int num2 = (int)Enum.Parse(this.editType, (string)checkedItem);
				num |= num2;
			}
			return Enum.ToObject(this.editType, num);
		}
	}
}
