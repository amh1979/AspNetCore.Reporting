//
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class MapDataGridViewer : Control
	{
		private class DoubleBufferedDataGrid : DataGrid
		{
			internal ArrayList selectedColumns = new ArrayList();

			public bool AllowMultipleSelection;

			public bool InteractiveSelection = true;

			public CurrencyManager CurrencyManager
			{
				get
				{
					return base.ListManager;
				}
			}

			public int SelectedColumnIndex
			{
				get
				{
					return (int)this.selectedColumns[0];
				}
			}

			public string SelectedColumnText
			{
				get
				{
					if (this.selectedColumns.Count == 0)
					{
						return "";
					}
					return base.TableStyles[0].GridColumnStyles[(int)this.selectedColumns[0]].HeaderText;
				}
			}

			public int[] SelectedColumnIndices
			{
				get
				{
					return (int[])this.selectedColumns.ToArray(typeof(int));
				}
			}

			public string[] SelectedColumnsText
			{
				get
				{
					string[] array = new string[this.selectedColumns.Count];
					for (int i = 0; i < this.selectedColumns.Count; i++)
					{
						array[i] = base.TableStyles[0].GridColumnStyles[(int)this.selectedColumns[i]].HeaderText;
					}
					return array;
				}
			}

			public DoubleBufferedDataGrid()
			{
				base.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
				base.TabStop = false;
			}

			protected override void OnMouseMove(MouseEventArgs e)
			{
				Point position = new Point(e.X, e.Y);
				HitTestInfo hitTestInfo = base.HitTest(position);
				if (hitTestInfo.Type != HitTestType.RowResize)
				{
					base.OnMouseMove(e);
				}
			}

			protected override void OnMouseDown(MouseEventArgs e)
			{
				Point position = new Point(e.X, e.Y);
				HitTestInfo hitTestInfo = base.HitTest(position);
				if (hitTestInfo.Type == HitTestType.ColumnHeader || hitTestInfo.Type == HitTestType.Cell)
				{
					if (!this.InteractiveSelection)
					{
						if (hitTestInfo.Type == HitTestType.ColumnHeader)
						{
							base.OnMouseDown(e);
						}
						return;
					}
					bool flag = (Control.ModifierKeys & Keys.Shift) == Keys.Shift;
					bool flag2 = (Control.ModifierKeys & Keys.Control) == Keys.Control;
					if (this.AllowMultipleSelection)
					{
						if (!flag && !flag2)
						{
							this.selectedColumns.Clear();
						}
						if (flag && !flag2)
						{
							int num = (this.selectedColumns.Count > 0) ? ((int)this.selectedColumns[this.selectedColumns.Count - 1]) : (-1);
							if (hitTestInfo.Column > num)
							{
								for (int i = num + 1; i <= hitTestInfo.Column; i++)
								{
									if (!this.selectedColumns.Contains(i))
									{
										this.selectedColumns.Add(i);
									}
								}
							}
							else if (hitTestInfo.Column < num)
							{
								for (int num2 = num - 1; num2 >= hitTestInfo.Column; num2--)
								{
									if (!this.selectedColumns.Contains(num2))
									{
										this.selectedColumns.Add(num2);
									}
								}
							}
						}
					}
					if (flag2)
					{
						if (this.selectedColumns.Contains(hitTestInfo.Column))
						{
							this.selectedColumns.Remove(hitTestInfo.Column);
						}
						else
						{
							this.selectedColumns.Add(hitTestInfo.Column);
						}
					}
					if (!flag && !flag2)
					{
						this.selectedColumns.Clear();
						this.selectedColumns.Add(hitTestInfo.Column);
					}
					base.Invalidate();
				}
				else if (hitTestInfo.Type == HitTestType.RowResize)
				{
					return;
				}
				base.OnMouseDown(e);
			}

			public void SetColumnsSize()
			{
                /*
				Graphics graphics = null;
				StringFormat stringFormat = null;
				DataGridTableStyle dataGridTableStyle = base.TableStyles[0];
				try
				{
					graphics = Graphics.FromHwnd(base.Handle);
					stringFormat = new StringFormat(StringFormat.GenericTypographic);                    
					int num = (int)Math.Ceiling((double)graphics.MeasureString("WW", this.Font, base.Width, stringFormat).Width);
					float[] array = new float[dataGridTableStyle.GridColumnStyles.Count];
					for (int i = 0; i < dataGridTableStyle.GridColumnStyles.Count; i++)
					{
						array[i] = graphics.MeasureString(dataGridTableStyle.GridColumnStyles[i].HeaderText, this.Font, base.Width, stringFormat).Width + (float)num;
					}
					bool flag = true;
					foreach (DataRowView item in (DataView)base.DataSource)
					{
						for (int j = 0; j < dataGridTableStyle.GridColumnStyles.Count; j++)
						{
							string text = item[dataGridTableStyle.GridColumnStyles[j].HeaderText].ToString();
							if (text.Length > 50)
							{
								text = item[dataGridTableStyle.GridColumnStyles[j].HeaderText].ToString().Substring(0, 50);
							}
							SizeF sizeF = graphics.MeasureString(text, this.Font, base.Width, stringFormat);
							if (flag && dataGridTableStyle.AllowSorting)
							{
								sizeF.Width += (float)(3.5 * (float)num);
							}
							else
							{
								sizeF.Width += (float)num;
							}
							if (sizeF.Width > array[j])
							{
								array[j] = sizeF.Width;
							}
						}
						flag = false;
					}
					for (int k = 0; k < array.Length; k++)
					{
						dataGridTableStyle.GridColumnStyles[k].Width = (int)array[k];
					}
				}
				finally
				{
					if (graphics != null)
					{
						graphics.Dispose();
					}
					if (stringFormat != null)
					{
						stringFormat.Dispose();
					}
				}
                */
			}

			public void SelectColumnByIndex(int index)
			{
				this.SelectColumnByIndex(new int[1]
				{
					index
				});
			}

			public void SelectColumnByIndex(ICollection indices)
			{
				this.selectedColumns.Clear();
				foreach (int index in indices)
				{
					if (!this.selectedColumns.Contains(index))
					{
						this.selectedColumns.Add(index);
					}
				}
				base.Invalidate();
			}

			public void SelectColumnByName(string name)
			{
				this.SelectColumnByName(new string[1]
				{
					name
				});
			}

			public void SelectColumnByName(ICollection names)
			{
				ArrayList arrayList = new ArrayList();
				this.selectedColumns.Clear();
				foreach (DataGridColumnStyle gridColumnStyle in base.TableStyles[0].GridColumnStyles)
				{
					arrayList.Add(gridColumnStyle.HeaderText);
				}
				foreach (string name in names)
				{
					int num = arrayList.IndexOf(name);
					if (num >= 0 && !this.selectedColumns.Contains(num))
					{
						this.selectedColumns.Add(num);
					}
				}
				base.Invalidate();
			}

			public void ScrollToColumn(string name)
			{
				if (base.HorizScrollBar.Visible)
				{
					int num = -1;
					for (int i = 0; i < base.TableStyles[0].GridColumnStyles.Count; i++)
					{
						if (base.TableStyles[0].GridColumnStyles[i].HeaderText == name)
						{
							num = i;
						}
					}
					if (num != -1)
					{
						base.CurrentCell = new DataGridCell(0, 0);
						base.CurrentCell = new DataGridCell(0, num);
					}
				}
			}
		}

		internal class NonEditableCellColumn : DataGridTextBoxColumn
		{
			private bool rightAligned;

			private int columnIndex = -1;

			public NonEditableCellColumn(int colIndex, bool rightAligned)
			{
				this.rightAligned = rightAligned;
				this.columnIndex = colIndex;
			}

			protected override void Edit(CurrencyManager source, int rowNum, Rectangle bounds, bool readOnly, string instantText, bool cellIsVisible)
			{
			}
           
			protected  void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum, Brush backBrush, Brush foreBrush, bool alignToRight)
			{
				if (this.DataGridTableStyle != null && this.DataGridTableStyle.DataGrid != null)
				{
					DoubleBufferedDataGrid doubleBufferedDataGrid = this.DataGridTableStyle.DataGrid as DoubleBufferedDataGrid;
					if (doubleBufferedDataGrid != null && doubleBufferedDataGrid.selectedColumns.Contains(this.columnIndex))
					{
						backBrush = new SolidBrush(doubleBufferedDataGrid.SelectionBackColor);
						foreBrush = new SolidBrush(doubleBufferedDataGrid.SelectionForeColor);
					}
				}
				string text = this.GetText(this.GetColumnValueAtRow(source, rowNum));
				if (text.Length > 50)
				{
					text = text.Substring(0, 50) + "...";
				}
				//this.PaintText(g, bounds, text, backBrush, foreBrush, this.rightAligned);
			}

			protected  void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum, bool alignToRight)
			{
				string text = this.GetText(this.GetColumnValueAtRow(source, rowNum));
				if (text.Length > 50)
				{
					text = text.Substring(0, 50) + "...";
				} 
				//this.PaintText(g, bounds, text, alignToRight);
			}

 
            private string GetText(object value)
			{
				if (value is DBNull)
				{
					return this.NullText;
				}
				if (!string.IsNullOrEmpty(base.Format) && value is IFormattable)
				{
					try
					{
						return ((IFormattable)value).ToString(base.Format, base.FormatInfo);
					}
					catch
					{
					}
				}
				else
				{
					TypeConverter converter = TypeDescriptor.GetConverter(this.PropertyDescriptor.PropertyType);
					if (converter != null && converter.CanConvertTo(typeof(string)))
					{
						return (string)converter.ConvertTo(value, typeof(string));
					}
				}
				if (value == null)
				{
					return "";
				}
				return value.ToString();
			}
		}

		private DoubleBufferedDataGrid dataGrid;

		private bool interactiveSelection = true;

		private bool allowMultipleSelection;

		public bool InteractiveSelection
		{
			get
			{
				return this.interactiveSelection;
			}
			set
			{
				this.interactiveSelection = value;
				if (this.dataGrid != null)
				{
					this.dataGrid.InteractiveSelection = value;
				}
			}
		}

		public bool AllowMultipleSelection
		{
			get
			{
				return this.allowMultipleSelection;
			}
			set
			{
				this.allowMultipleSelection = value;
				if (this.dataGrid != null)
				{
					this.dataGrid.AllowMultipleSelection = value;
				}
			}
		}

		public int SelectedColumnIndex
		{
			get
			{
				return this.dataGrid.SelectedColumnIndex;
			}
		}

		public string SelectedColumnText
		{
			get
			{
				return this.dataGrid.SelectedColumnText;
			}
		}

		public int[] SelectedColumnIndices
		{
			get
			{
				return this.dataGrid.SelectedColumnIndices;
			}
		}

		public string[] SelectedColumnsText
		{
			get
			{
				return this.dataGrid.SelectedColumnsText;
			}
		}

		//public DataGrid InternalGrid
		//{
		//	get
		//	{
		//		return this.dataGrid;
		//	}
		//}

		public void Initialize(DataTable data)
		{
			base.Controls.Clear();
			if (data != null && data.Columns.Count != 0)
			{
				this.dataGrid = new DoubleBufferedDataGrid();
				//this.dataGrid.Dock = DockStyle.Fill;
				//this.dataGrid.CaptionVisible = false;
				this.dataGrid.InteractiveSelection = this.InteractiveSelection;
				this.dataGrid.AllowMultipleSelection = this.AllowMultipleSelection;
				base.Controls.Add(this.dataGrid);
				DataView defaultView = data.DefaultView;
				if (data.Rows.Count > 1000)
				{
					DataTable dataTable = new DataTable();
					dataTable.Locale = CultureInfo.CurrentCulture;
					foreach (DataColumn column in data.Columns)
					{
						dataTable.Columns.Add(new DataColumn(column.ColumnName, column.DataType, column.Expression));
					}
					for (int i = 0; i < 1000; i++)
					{
						dataTable.ImportRow(data.Rows[i]);
					}
					defaultView = dataTable.DefaultView;
				}
				defaultView.AllowNew = false;
				defaultView.AllowDelete = false;
				defaultView.AllowEdit = false;
				this.dataGrid.DataSource = defaultView;
				BindingContext bindingContext = new BindingContext();
				CurrencyManager listManager = (CurrencyManager)bindingContext[this.dataGrid.DataSource];
				DataGridTableStyle dataGridTableStyle = new DataGridTableStyle(listManager);
				dataGridTableStyle.GridColumnStyles.Clear();
				if (this.dataGrid.CurrencyManager != null)
				{
					int num = 0;
					{
						IEnumerator enumerator2 = this.dataGrid.CurrencyManager.GetItemProperties().GetEnumerator();
						try
						{
							PropertyDescriptor propertyDescriptor;
							bool rightAligned;
							NonEditableCellColumn nonEditableCellColumn;
							for (; enumerator2.MoveNext(); nonEditableCellColumn = new NonEditableCellColumn(num++, rightAligned), nonEditableCellColumn.HeaderText = propertyDescriptor.Name, nonEditableCellColumn.MappingName = propertyDescriptor.Name, nonEditableCellColumn.NullText = "(null)", dataGridTableStyle.GridColumnStyles.Add(nonEditableCellColumn))
							{
								propertyDescriptor = (PropertyDescriptor)enumerator2.Current;
								rightAligned = false;
								if (propertyDescriptor.PropertyType.IsPrimitive && propertyDescriptor.PropertyType != typeof(char))
								{
									goto IL_01ee;
								}
								if (propertyDescriptor.PropertyType == typeof(decimal))
								{
									goto IL_01ee;
								}
								continue;
								IL_01ee:
								rightAligned = true;
							}
						}
						finally
						{
							IDisposable disposable = enumerator2 as IDisposable;
							if (disposable != null)
							{
								disposable.Dispose();
							}
						}
					}
					dataGridTableStyle.AllowSorting = true;
					/*
                    foreach (DataColumn column2 in data.Columns)
					{
						if (column2.DataType == typeof(SqlGeometry) || column2.DataType == typeof(SqlGeography))
						{
							dataGridTableStyle.AllowSorting = false;
						}
					}
                    */
					dataGridTableStyle.RowHeadersVisible = false;
					this.dataGrid.TableStyles.Add(dataGridTableStyle);
					this.dataGrid.SetColumnsSize();
				}
			}
		}

		public void SelectColumnByIndex(int index)
		{
			if (this.dataGrid != null)
			{
				this.dataGrid.SelectColumnByIndex(index);
			}
		}

		public void SelectColumnByIndex(int[] indices)
		{
			if (this.dataGrid != null)
			{
				this.dataGrid.SelectColumnByIndex(indices);
			}
		}

		public void SelectColumnByName(string name)
		{
			if (this.dataGrid != null)
			{
				this.dataGrid.SelectColumnByName(name);
			}
		}

		public void SelectColumnByName(string[] names)
		{
			if (this.dataGrid != null)
			{
				this.dataGrid.SelectColumnByName(names);
			}
		}

		public void ScrollToColumn(string name)
		{
			if (this.dataGrid != null)
			{
				this.dataGrid.ScrollToColumn(name);
			}
		}
	}
}
