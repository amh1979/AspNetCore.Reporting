using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class LabelFormatEditor : UITypeEditor
	{
		private IWindowsFormsEditorService edSvc;

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && context.Instance != null && provider != null)
			{
				this.edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (this.edSvc != null)
				{
					LabelFormatEditorForm labelFormatEditorForm = new LabelFormatEditorForm();
					labelFormatEditorForm.resultFormat = (string)value;
					this.edSvc.ShowDialog(labelFormatEditorForm);
					value = labelFormatEditorForm.resultFormat;
				}
			}
			return value;
		}

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			if (context != null && context.Instance != null)
			{
				return UITypeEditorEditStyle.Modal;
			}
			return base.GetEditStyle(context);
		}
	}
}
