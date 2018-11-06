using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class FieldChooser : Form
	{
		private Label label1;

		private Button okButton;

		private Button cancelButton;

		private string selectedField = "";

		private System.Windows.Forms.Panel gridPlaceHolder;

		private IContainer components;

		private MapDataGridViewer previewGrid = new MapDataGridViewer();

		public string SelectedField
		{
			get
			{
				return this.selectedField;
			}
		}

		public FieldChooser(MapControl mapControl)
		{
			this.InitializeComponent();
			this.previewGrid.Dock = DockStyle.Fill;
			this.previewGrid.AllowMultipleSelection = false;
			this.gridPlaceHolder.Controls.Add(this.previewGrid);
			DataTable dataTable = new DataTable("Fields")
			{
				Locale = CultureInfo.CurrentCulture
			};
			foreach (Field shapeField in mapControl.ShapeFields)
			{
				dataTable.Columns.Add(shapeField.Name, shapeField.Type);
			}
			foreach (Shape shape in mapControl.Shapes)
			{
				DataRow dataRow = dataTable.NewRow();
				foreach (Field shapeField2 in mapControl.ShapeFields)
				{
					if (shape[shapeField2.Name] != null)
					{
						dataRow[shapeField2.Name] = shape[shapeField2.Name];
					}
				}
				dataTable.Rows.Add(dataRow);
			}
			this.previewGrid.Initialize(dataTable);
			this.previewGrid.SelectColumnByIndex(0);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.label1 = new Label();
			this.okButton = new Button();
			this.cancelButton = new Button();
			this.gridPlaceHolder = new System.Windows.Forms.Panel();
			base.SuspendLayout();
			this.label1.AutoSize = true;
			this.label1.Location = new Point(12, 15);
			this.label1.Name = "label1";
			this.label1.Size = new Size(336, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Please choose the shape field based on which groups will be created:";
			this.okButton.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.okButton.DialogResult = DialogResult.OK;
			this.okButton.Location = new Point(340, 368);
			this.okButton.Name = "okButton";
			this.okButton.Size = new Size(92, 26);
			this.okButton.TabIndex = 2;
			this.okButton.Text = "&OK";
			this.okButton.Click += this.okButton_Click;
			this.cancelButton.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.cancelButton.DialogResult = DialogResult.Cancel;
			this.cancelButton.Location = new Point(438, 368);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new Size(92, 26);
			this.cancelButton.TabIndex = 3;
			this.cancelButton.Text = "&Cancel";
			this.gridPlaceHolder.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.gridPlaceHolder.Location = new Point(12, 37);
			this.gridPlaceHolder.Name = "gridPlaceHolder";
			this.gridPlaceHolder.Size = new Size(518, 314);
			this.gridPlaceHolder.TabIndex = 4;
			base.AcceptButton = this.okButton;
			base.CancelButton = this.cancelButton;
			base.ClientSize = new Size(542, 406);
			base.Controls.Add(this.gridPlaceHolder);
			base.Controls.Add(this.cancelButton);
			base.Controls.Add(this.okButton);
			base.Controls.Add(this.label1);
			base.MinimizeBox = false;
			base.Name = "FieldChooser";
			base.SizeGripStyle = SizeGripStyle.Show;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "Map for .NET";
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			this.selectedField = this.previewGrid.SelectedColumnText;
		}
	}
}
