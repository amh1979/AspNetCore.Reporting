using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace AspNetCore.Reporting.Chart.WebForms.Design
{
	internal class AngleValueEditor : UITypeEditor
	{
		private IWindowsFormsEditorService edSvc;

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && context.Instance != null && provider != null)
			{
				this.edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (this.edSvc != null)
				{
					AngleTrackForm angleTrackForm = new AngleTrackForm();
					angleTrackForm.ValueChanged += this.ValueChanged;
					bool flag = true;
					if (value is int)
					{
						angleTrackForm.Angle = (int)value;
					}
					else if (value is byte)
					{
						flag = false;
						angleTrackForm.Angle = (byte)value;
					}
					this.edSvc.DropDownControl(angleTrackForm);
					value = ((!flag) ? ((object)(byte)angleTrackForm.Angle) : ((object)angleTrackForm.Angle));
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

		private void ValueChanged(object sender, EventArgs e)
		{
			if (this.edSvc != null)
			{
				this.edSvc.CloseDropDown();
			}
		}
	}
}
