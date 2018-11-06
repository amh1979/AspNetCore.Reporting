using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class FlagsEnumUITypeEditor : UITypeEditor
	{
		private Type enumType;

		private IWindowsFormsEditorService edSvc;

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && context.Instance != null && provider != null)
			{
				this.edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (this.edSvc != null)
				{
					if (value != null)
					{
						this.enumType = value.GetType();
					}
					else if (context != null && context.PropertyDescriptor != null)
					{
						this.enumType = context.PropertyDescriptor.PropertyType;
					}
					if (this.enumType != null)
					{
						FlagsEnumCheckedListBox flagsEnumCheckedListBox = new FlagsEnumCheckedListBox(value, this.enumType);
						this.edSvc.DropDownControl(flagsEnumCheckedListBox);
						value = flagsEnumCheckedListBox.GetNewValue();
					}
				}
			}
			return value;
		}

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			if (context != null && context.Instance != null)
			{
				return UITypeEditorEditStyle.DropDown;
			}
			return base.GetEditStyle(context);
		}
	}
}
