using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class LabelFormatEditorForm : Form
	{
		internal string resultFormat = "";

		private string formatString = "";

		private string formatNumeric = "";

		private TabControl tabControl;

		private ComboBox comboBoxFormatType;

		private TabPage tabPageNumeric;

		private TabPage tabPageCustom;

		private TextBox textBoxFormatString;

		private TextBox textBoxCustomSample;

		private Label labelCustomDescription;

		private Label labelNumericFormatDescription;

		private TextBox textBoxNumericSample;

		private Button buttonOk;

		private Button buttonCancel;

		private Label label1;

		private Label label2;

		private Label label6;

		private Label label4;

		private Label label3;

		private TextBox textBoxPrecision;

		private Container components;

		private void InitializeComponent()
		{
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(LabelFormatEditorForm));
			this.tabControl = new TabControl();
			this.tabPageNumeric = new TabPage();
			this.textBoxPrecision = new TextBox();
			this.textBoxNumericSample = new TextBox();
			this.label3 = new Label();
			this.labelNumericFormatDescription = new Label();
			this.label2 = new Label();
			this.comboBoxFormatType = new ComboBox();
			this.label1 = new Label();
			this.tabPageCustom = new TabPage();
			this.textBoxFormatString = new TextBox();
			this.label6 = new Label();
			this.textBoxCustomSample = new TextBox();
			this.label4 = new Label();
			this.labelCustomDescription = new Label();
			this.buttonOk = new Button();
			this.buttonCancel = new Button();
			this.tabControl.SuspendLayout();
			this.tabPageNumeric.SuspendLayout();
			this.tabPageCustom.SuspendLayout();
			base.SuspendLayout();
			this.tabControl.Controls.Add(this.tabPageNumeric);
			this.tabControl.Controls.Add(this.tabPageCustom);
			componentResourceManager.ApplyResources(this.tabControl, "tabControl");
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.SelectedIndexChanged += this.tabControl_SelectedIndexChanged;
			this.tabPageNumeric.Controls.Add(this.textBoxPrecision);
			this.tabPageNumeric.Controls.Add(this.textBoxNumericSample);
			this.tabPageNumeric.Controls.Add(this.label3);
			this.tabPageNumeric.Controls.Add(this.labelNumericFormatDescription);
			this.tabPageNumeric.Controls.Add(this.label2);
			this.tabPageNumeric.Controls.Add(this.comboBoxFormatType);
			this.tabPageNumeric.Controls.Add(this.label1);
			componentResourceManager.ApplyResources(this.tabPageNumeric, "tabPageNumeric");
			this.tabPageNumeric.Name = "tabPageNumeric";
			componentResourceManager.ApplyResources(this.textBoxPrecision, "textBoxPrecision");
			this.textBoxPrecision.Name = "textBoxPrecision";
			this.textBoxPrecision.TextChanged += this.textBoxPrecision_TextChanged;
			componentResourceManager.ApplyResources(this.textBoxNumericSample, "textBoxNumericSample");
			this.textBoxNumericSample.Name = "textBoxNumericSample";
			this.textBoxNumericSample.ReadOnly = true;
			componentResourceManager.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
			componentResourceManager.ApplyResources(this.labelNumericFormatDescription, "labelNumericFormatDescription");
			this.labelNumericFormatDescription.Name = "labelNumericFormatDescription";
			componentResourceManager.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			this.comboBoxFormatType.DropDownStyle = ComboBoxStyle.DropDownList;
			this.comboBoxFormatType.Items.AddRange(new object[6]
			{
				componentResourceManager.GetString("comboBoxFormatType.Items"),
				componentResourceManager.GetString("comboBoxFormatType.Items1"),
				componentResourceManager.GetString("comboBoxFormatType.Items2"),
				componentResourceManager.GetString("comboBoxFormatType.Items3"),
				componentResourceManager.GetString("comboBoxFormatType.Items4"),
				componentResourceManager.GetString("comboBoxFormatType.Items5")
			});
			componentResourceManager.ApplyResources(this.comboBoxFormatType, "comboBoxFormatType");
			this.comboBoxFormatType.Name = "comboBoxFormatType";
			this.comboBoxFormatType.SelectedIndexChanged += this.comboBoxFormatType_SelectedIndexChanged;
			componentResourceManager.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			this.tabPageCustom.Controls.Add(this.textBoxFormatString);
			this.tabPageCustom.Controls.Add(this.label6);
			this.tabPageCustom.Controls.Add(this.textBoxCustomSample);
			this.tabPageCustom.Controls.Add(this.label4);
			this.tabPageCustom.Controls.Add(this.labelCustomDescription);
			componentResourceManager.ApplyResources(this.tabPageCustom, "tabPageCustom");
			this.tabPageCustom.Name = "tabPageCustom";
			componentResourceManager.ApplyResources(this.textBoxFormatString, "textBoxFormatString");
			this.textBoxFormatString.Name = "textBoxFormatString";
			this.textBoxFormatString.TextChanged += this.textBoxFormatString_TextChanged;
			componentResourceManager.ApplyResources(this.label6, "label6");
			this.label6.Name = "label6";
			componentResourceManager.ApplyResources(this.textBoxCustomSample, "textBoxCustomSample");
			this.textBoxCustomSample.Name = "textBoxCustomSample";
			this.textBoxCustomSample.ReadOnly = true;
			componentResourceManager.ApplyResources(this.label4, "label4");
			this.label4.Name = "label4";
			componentResourceManager.ApplyResources(this.labelCustomDescription, "labelCustomDescription");
			this.labelCustomDescription.Name = "labelCustomDescription";
			this.buttonOk.DialogResult = DialogResult.OK;
			componentResourceManager.ApplyResources(this.buttonOk, "buttonOk");
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Click += this.buttonOk_Click;
			this.buttonCancel.DialogResult = DialogResult.Cancel;
			componentResourceManager.ApplyResources(this.buttonCancel, "buttonCancel");
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Click += this.buttonCancel_Click;
			base.AcceptButton = this.buttonOk;
			componentResourceManager.ApplyResources(this, "$this");
			base.Controls.Add(this.buttonCancel);
			base.Controls.Add(this.buttonOk);
			base.Controls.Add(this.tabControl);
			base.FormBorderStyle = FormBorderStyle.FixedDialog;
			base.Name = "LabelFormatEditorForm";
			base.Load += this.LabelFormatEditorForm_Load;
			this.tabControl.ResumeLayout(false);
			this.tabPageNumeric.ResumeLayout(false);
			this.tabPageNumeric.PerformLayout();
			this.tabPageCustom.ResumeLayout(false);
			this.tabPageCustom.PerformLayout();
			base.ResumeLayout(false);
		}

		public LabelFormatEditorForm()
		{
			this.InitializeComponent();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void UpdateNumericSample()
		{
			this.formatString = this.formatNumeric + this.textBoxPrecision.Text;
			if (this.formatString.StartsWith("D", StringComparison.Ordinal))
			{
				this.textBoxNumericSample.Text = string.Format(CultureInfo.CurrentCulture, "{0:" + this.formatString + "}", 12345);
			}
			else if (this.formatString.StartsWith("P", StringComparison.Ordinal))
			{
				this.textBoxNumericSample.Text = string.Format(CultureInfo.CurrentCulture, "{0:" + this.formatString + "}", 0.126);
			}
			else
			{
				this.textBoxNumericSample.Text = string.Format(CultureInfo.CurrentCulture, "{0:" + this.formatString + "}", 12345.6789);
			}
		}

		private void comboBoxFormatType_SelectedIndexChanged(object sender, EventArgs e)
		{
			string[] array = new string[6]
			{
				"F",
				"C",
				"E",
				"G",
				"N",
				"P"
			};
			string[] array2 = new string[6]
			{
				SR.LabelFormatDescriptionF,
				SR.LabelFormatDescriptionC,
				SR.LabelFormatDescriptionE,
				SR.LabelFormatDescriptionG,
				SR.LabelFormatDescriptionN,
				SR.LabelFormatDescriptionP
			};
			this.formatNumeric = array[this.comboBoxFormatType.SelectedIndex];
			this.labelNumericFormatDescription.Text = array2[this.comboBoxFormatType.SelectedIndex];
			this.UpdateNumericSample();
		}

		private void LabelFormatEditorForm_Load(object sender, EventArgs e)
		{
			this.comboBoxFormatType.SelectedIndex = 0;
		}

		private void textBoxPrecision_TextChanged(object sender, EventArgs e)
		{
			if (this.textBoxPrecision.Text.Length >= 1 && !char.IsDigit(this.textBoxPrecision.Text[0]))
			{
				MessageBox.Show(this, SR.LabelFormatPrecisionMsg, SR.LabelFormatPrecisionMsgCaption, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1, GlobalizationHelper.GetMessageBoxOptions(this));
				this.textBoxPrecision.Text = string.Empty;
			}
			else if (this.textBoxPrecision.Text.Length >= 2 && (!char.IsDigit(this.textBoxPrecision.Text[0]) || !char.IsDigit(this.textBoxPrecision.Text[1])))
			{
				MessageBox.Show(this, SR.LabelFormatPrecisionMsg, SR.LabelFormatPrecisionMsgCaption, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1, GlobalizationHelper.GetMessageBoxOptions(this));
				this.textBoxPrecision.Text = string.Empty;
			}
			this.UpdateNumericSample();
		}

		private void textBoxFormatString_TextChanged(object sender, EventArgs e)
		{
			this.UpdateCustomExample();
		}

		private void UpdateCustomExample()
		{
			bool flag = false;
			this.formatString = this.textBoxFormatString.Text;
			if (!flag)
			{
				try
				{
					this.textBoxCustomSample.Text = string.Format(CultureInfo.CurrentCulture, "{0:" + this.textBoxFormatString.Text + "}", 12345.6789);
					flag = true;
				}
				catch (Exception)
				{
				}
				if (!flag)
				{
					try
					{
						this.textBoxCustomSample.Text = string.Format(CultureInfo.CurrentCulture, "{0:" + this.textBoxFormatString.Text + "}", 12345);
						flag = true;
					}
					catch (Exception)
					{
					}
				}
			}
			if (!flag)
			{
				this.textBoxCustomSample.Text = SR.LabelFormatInvalidCustomFormat;
			}
		}

		private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.tabControl.SelectedIndex == 0)
			{
				this.comboBoxFormatType.SelectedIndex = 0;
				this.comboBoxFormatType.Focus();
			}
			else if (this.tabControl.SelectedIndex == 1)
			{
				this.formatString = "";
				this.textBoxFormatString.Focus();
			}
		}

		private void buttonOk_Click(object sender, EventArgs e)
		{
			this.resultFormat = this.formatString;
			base.Close();
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			base.Close();
		}
	}
}
