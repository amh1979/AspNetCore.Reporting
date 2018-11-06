using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace AspNetCore.Reporting.Map.WebForms
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
			this.tabControl.Controls.AddRange(new Control[2]
			{
				this.tabPageNumeric,
				this.tabPageCustom
			});
			this.tabControl.Location = new Point(8, 8);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new Size(488, 216);
			this.tabControl.TabIndex = 0;
			this.tabControl.SelectedIndexChanged += this.tabControl_SelectedIndexChanged;
			this.tabPageNumeric.Controls.AddRange(new Control[7]
			{
				this.textBoxPrecision,
				this.textBoxNumericSample,
				this.label3,
				this.labelNumericFormatDescription,
				this.label2,
				this.comboBoxFormatType,
				this.label1
			});
			this.tabPageNumeric.Location = new Point(4, 22);
			this.tabPageNumeric.Name = "tabPageNumeric";
			this.tabPageNumeric.Size = new Size(480, 190);
			this.tabPageNumeric.TabIndex = 0;
			this.tabPageNumeric.Text = "Numeric";
			this.textBoxPrecision.Location = new Point(112, 40);
			this.textBoxPrecision.MaxLength = 2;
			this.textBoxPrecision.Name = "textBoxPrecision";
			this.textBoxPrecision.Size = new Size(200, 20);
			this.textBoxPrecision.TabIndex = 13;
			this.textBoxPrecision.Text = "";
			this.textBoxPrecision.TextChanged += this.textBoxPrecision_TextChanged;
			this.textBoxNumericSample.Location = new Point(112, 64);
			this.textBoxNumericSample.Name = "textBoxNumericSample";
			this.textBoxNumericSample.ReadOnly = true;
			this.textBoxNumericSample.Size = new Size(200, 20);
			this.textBoxNumericSample.TabIndex = 12;
			this.textBoxNumericSample.Text = "";
			this.label3.Location = new Point(8, 64);
			this.label3.Name = "label3";
			this.label3.Size = new Size(104, 23);
			this.label3.TabIndex = 6;
			this.label3.Text = "Sample:";
			//this.label3.TextAlign = ContentAlignment.MiddleRight;
			this.labelNumericFormatDescription.Location = new Point(8, 96);
			this.labelNumericFormatDescription.Name = "labelNumericFormatDescription";
			this.labelNumericFormatDescription.Size = new Size(464, 88);
			this.labelNumericFormatDescription.TabIndex = 5;
			this.label2.Location = new Point(8, 40);
			this.label2.Name = "label2";
			this.label2.Size = new Size(104, 23);
			this.label2.TabIndex = 2;
			this.label2.Text = "Precision specifier:";
			//this.label2.TextAlign = ContentAlignment.MiddleRight;
			this.comboBoxFormatType.DropDownStyle = ComboBoxStyle.DropDownList;
			this.comboBoxFormatType.Items.AddRange(new object[6]
			{
				"Fixed-point",
				"Currency",
				"Scientific",
				"General",
				"Number",
				"Percent"
			});
			this.comboBoxFormatType.Location = new Point(112, 16);
			this.comboBoxFormatType.Name = "comboBoxFormatType";
			this.comboBoxFormatType.Size = new Size(200, 21);
			this.comboBoxFormatType.TabIndex = 10;
			this.comboBoxFormatType.SelectedIndexChanged += this.comboBoxFormatType_SelectedIndexChanged;
			this.label1.Location = new Point(8, 16);
			this.label1.Name = "label1";
			this.label1.Size = new Size(104, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "Format type:";
			//this.label1.TextAlign = ContentAlignment.MiddleRight;
			this.tabPageCustom.Controls.AddRange(new Control[5]
			{
				this.textBoxFormatString,
				this.label6,
				this.textBoxCustomSample,
				this.label4,
				this.labelCustomDescription
			});
			this.tabPageCustom.Location = new Point(4, 22);
			this.tabPageCustom.Name = "tabPageCustom";
			this.tabPageCustom.Size = new Size(480, 190);
			this.tabPageCustom.TabIndex = 2;
			this.tabPageCustom.Text = "Custom";
			this.textBoxFormatString.Location = new Point(112, 12);
			this.textBoxFormatString.Name = "textBoxFormatString";
			this.textBoxFormatString.Size = new Size(200, 20);
			this.textBoxFormatString.TabIndex = 31;
			this.textBoxFormatString.Text = "";
			this.textBoxFormatString.TextChanged += this.textBoxFormatString_TextChanged;
			this.label6.Location = new Point(8, 12);
			this.label6.Name = "label6";
			this.label6.Size = new Size(104, 23);
			this.label6.TabIndex = 8;
			this.label6.Text = "Format string:";
			//this.label6.TextAlign = ContentAlignment.MiddleRight;
			this.textBoxCustomSample.Location = new Point(112, 36);
			this.textBoxCustomSample.Name = "textBoxCustomSample";
			this.textBoxCustomSample.ReadOnly = true;
			this.textBoxCustomSample.Size = new Size(200, 20);
			this.textBoxCustomSample.TabIndex = 32;
			this.textBoxCustomSample.Text = "";
			this.label4.Location = new Point(8, 36);
			this.label4.Name = "label4";
			this.label4.Size = new Size(104, 23);
			this.label4.TabIndex = 11;
			this.label4.Text = "Sample:";
			//this.label4.TextAlign = ContentAlignment.MiddleRight;
			this.labelCustomDescription.Location = new Point(8, 72);
			this.labelCustomDescription.Name = "labelCustomDescription";
			this.labelCustomDescription.Size = new Size(464, 112);
			this.labelCustomDescription.TabIndex = 10;
			this.labelCustomDescription.Text = "Characters that can be used to create custom numeric format strings:  '0' - Zero placeholder, '#' - Digit placeholder, '.' - Decimal point, ',' - Thousand separator and number scaling, '%' - Percentage placeholder, 'E0' - Scientific notation, '\\\\' - Escape character, ';' - Section separator.\\r\\n\\r\\nAll other characters are copied to the output string as literals in the position they appear.";
			this.buttonOk.DialogResult = DialogResult.OK;
			this.buttonOk.Location = new Point(504, 40);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.TabIndex = 40;
			this.buttonOk.Text = "Ok";
			this.buttonOk.Click += this.buttonOk_Click;
			this.buttonCancel.DialogResult = DialogResult.Cancel;
			this.buttonCancel.Location = new Point(504, 80);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.TabIndex = 41;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.Click += this.buttonCancel_Click;
			base.AcceptButton = this.buttonOk;
			this.AutoScaleBaseSize = new Size(5, 13);
			base.ClientSize = new Size(592, 229);
			base.Controls.AddRange(new Control[3]
			{
				this.buttonCancel,
				this.buttonOk,
				this.tabControl
			});
			base.FormBorderStyle = FormBorderStyle.FixedDialog;
			base.Name = "LabelFormatEditorForm";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "Format Editor";
			base.Load += this.LabelFormatEditorForm_Load;
			this.tabControl.ResumeLayout(false);
			this.tabPageNumeric.ResumeLayout(false);
			this.tabPageCustom.ResumeLayout(false);
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
				"The number is converted to the most compact decimal form, using fixed or scientific notation. The precision specifier determines the number of significant digits in the resulting string.",
				"The number is converted to a string that represents a currency amount. The conversion is controlled by the system currency format information. The precision specifier indicates the desired number of decimal places.",
				"The number is converted to a string of the form \"-d.ddd…E+ddd\" or \"-d.ddd…e+ddd\", where each 'd' indicates a digit (0-9). The string starts with a minus sign if the number is negative. One digit always precedes the decimal point. The precision specifier indicates the desired number of digits after the decimal point.",
				"The number is converted to a string of the form \"-ddd.ddd…\" where each 'd' indicates a digit (0-9). The string starts with a minus sign if the number is negative. The precision specifier indicates the desired number of decimal places.",
				"The number is converted to a string of the form \"-d,ddd,ddd.ddd…\", where each 'd' indicates a digit (0-9). The string starts with a minus sign if the number is negative. Thousand separators are inserted between each group of three digits to the left of the decimal point. The precision specifier indicates the desired number of decimal places.",
				"The number is converted to a string that represents a percent. The precision specifier indicates the desired number of decimal places."
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
				this.textBoxCustomSample.Text = "Invalid custom format string";
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
