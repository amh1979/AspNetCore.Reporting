using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace AspNetCore.Reporting.Gauge.WinForms.DataEngine
{
	[Serializable]
	[ToolboxItem(false)]
	[XmlRoot("GaugeData")]
	[HelpKeyword("vs.data.DataSet")]
	[XmlSchemaProvider("GetTypedDataSetSchema")]
	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "2.0.0.0")]
	[DesignerCategory("code")]
	internal class GaugeData : DataSet
	{
		public delegate void ValuesRowChangeEventHandler(object sender, ValuesRowChangeEvent e);

		[Serializable]
		[XmlSchemaProvider("GetTypedTableSchema")]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "2.0.0.0")]
		internal class ValuesDataTable : DataTable, IEnumerable
		{
			private DataColumn columnDateStamp;

			private DataColumn columnValue;

			[DebuggerNonUserCode]
			public DataColumn DateStampColumn
			{
				get
				{
					return this.columnDateStamp;
				}
			}

			[DebuggerNonUserCode]
			public DataColumn ValueColumn
			{
				get
				{
					return this.columnValue;
				}
			}

			[Browsable(false)]
			[DebuggerNonUserCode]
			public int Count
			{
				get
				{
					return base.Rows.Count;
				}
			}

			[DebuggerNonUserCode]
			public ValuesRow this[int index]
			{
				get
				{
					return (ValuesRow)base.Rows[index];
				}
			}

			public event ValuesRowChangeEventHandler ValuesRowChanging;

			public event ValuesRowChangeEventHandler ValuesRowChanged;

			public event ValuesRowChangeEventHandler ValuesRowDeleting;

			public event ValuesRowChangeEventHandler ValuesRowDeleted;

			[DebuggerNonUserCode]
			public ValuesDataTable()
			{
				base.TableName = "Values";
				this.BeginInit();
				this.InitClass();
				this.EndInit();
			}

			[DebuggerNonUserCode]
			internal ValuesDataTable(DataTable table)
			{
				base.TableName = table.TableName;
				if (table.CaseSensitive != table.DataSet.CaseSensitive)
				{
					base.CaseSensitive = table.CaseSensitive;
				}
				if (table.Locale.ToString() != table.DataSet.Locale.ToString())
				{
					base.Locale = table.Locale;
				}
				if (table.Namespace != table.DataSet.Namespace)
				{
					base.Namespace = table.Namespace;
				}
				base.Prefix = table.Prefix;
				base.MinimumCapacity = table.MinimumCapacity;
			}

			[DebuggerNonUserCode]
			protected ValuesDataTable(SerializationInfo info, StreamingContext context)
				: base(info, context)
			{
				this.InitVars();
			}

			[DebuggerNonUserCode]
			public void AddValuesRow(ValuesRow row)
			{
				base.Rows.Add(row);
			}

			[DebuggerNonUserCode]
			public ValuesRow AddValuesRow(DateTime DateStamp, double Value)
			{
				ValuesRow valuesRow = (ValuesRow)base.NewRow();
				valuesRow.ItemArray = new object[2]
				{
					DateStamp,
					Value
				};
				base.Rows.Add(valuesRow);
				return valuesRow;
			}

			[DebuggerNonUserCode]
			public ValuesRow FindByDateStamp(DateTime DateStamp)
			{
				return (ValuesRow)base.Rows.Find(new object[1]
				{
					DateStamp
				});
			}

			[DebuggerNonUserCode]
			public virtual IEnumerator GetEnumerator()
			{
				return base.Rows.GetEnumerator();
			}

			[DebuggerNonUserCode]
			public override DataTable Clone()
			{
				ValuesDataTable valuesDataTable = (ValuesDataTable)base.Clone();
				valuesDataTable.InitVars();
				return valuesDataTable;
			}

			[DebuggerNonUserCode]
			protected override DataTable CreateInstance()
			{
				return new ValuesDataTable();
			}

			[DebuggerNonUserCode]
			internal void InitVars()
			{
				this.columnDateStamp = base.Columns["DateStamp"];
				this.columnValue = base.Columns["Value"];
			}

			[DebuggerNonUserCode]
			private void InitClass()
			{
				this.columnDateStamp = new DataColumn("DateStamp", typeof(DateTime), null, MappingType.Element);
				base.Columns.Add(this.columnDateStamp);
				this.columnValue = new DataColumn("Value", typeof(double), null, MappingType.Element);
				base.Columns.Add(this.columnValue);
				base.Constraints.Add(new UniqueConstraint("GaugeDataKey", new DataColumn[1]
				{
					this.columnDateStamp
				}, true));
				this.columnDateStamp.AllowDBNull = false;
				this.columnDateStamp.Unique = true;
			}

			[DebuggerNonUserCode]
			public ValuesRow NewValuesRow()
			{
				return (ValuesRow)base.NewRow();
			}

			[DebuggerNonUserCode]
			protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
			{
				return new ValuesRow(builder);
			}

			[DebuggerNonUserCode]
			protected override Type GetRowType()
			{
				return typeof(ValuesRow);
			}

			[DebuggerNonUserCode]
			protected override void OnRowChanged(DataRowChangeEventArgs e)
			{
				base.OnRowChanged(e);
				if (this.ValuesRowChanged != null)
				{
					this.ValuesRowChanged(this, new ValuesRowChangeEvent((ValuesRow)e.Row, e.Action));
				}
			}

			[DebuggerNonUserCode]
			protected override void OnRowChanging(DataRowChangeEventArgs e)
			{
				base.OnRowChanging(e);
				if (this.ValuesRowChanging != null)
				{
					this.ValuesRowChanging(this, new ValuesRowChangeEvent((ValuesRow)e.Row, e.Action));
				}
			}

			[DebuggerNonUserCode]
			protected override void OnRowDeleted(DataRowChangeEventArgs e)
			{
				base.OnRowDeleted(e);
				if (this.ValuesRowDeleted != null)
				{
					this.ValuesRowDeleted(this, new ValuesRowChangeEvent((ValuesRow)e.Row, e.Action));
				}
			}

			[DebuggerNonUserCode]
			protected override void OnRowDeleting(DataRowChangeEventArgs e)
			{
				base.OnRowDeleting(e);
				if (this.ValuesRowDeleting != null)
				{
					this.ValuesRowDeleting(this, new ValuesRowChangeEvent((ValuesRow)e.Row, e.Action));
				}
			}

			[DebuggerNonUserCode]
			public void RemoveValuesRow(ValuesRow row)
			{
				base.Rows.Remove(row);
			}

			[DebuggerNonUserCode]
			public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
			{
				XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
				XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
				GaugeData gaugeData = new GaugeData();
				xs.Add(gaugeData.GetSchemaSerializable());
				XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
				xmlSchemaAny.Namespace = "http://www.w3.org/2001/XMLSchema";
				xmlSchemaAny.MinOccurs = 0m;
				xmlSchemaAny.MaxOccurs = 79228162514264337593543950335m;
				xmlSchemaAny.ProcessContents = XmlSchemaContentProcessing.Lax;
				xmlSchemaSequence.Items.Add(xmlSchemaAny);
				XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
				xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
				xmlSchemaAny2.MinOccurs = 1m;
				xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
				xmlSchemaSequence.Items.Add(xmlSchemaAny2);
				XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
				xmlSchemaAttribute.Name = "namespace";
				xmlSchemaAttribute.FixedValue = gaugeData.Namespace;
				xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute);
				XmlSchemaAttribute xmlSchemaAttribute2 = new XmlSchemaAttribute();
				xmlSchemaAttribute2.Name = "tableTypeName";
				xmlSchemaAttribute2.FixedValue = "ValuesDataTable";
				xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute2);
				xmlSchemaComplexType.Particle = xmlSchemaSequence;
				return xmlSchemaComplexType;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "2.0.0.0")]
		internal class ValuesRow : DataRow
		{
			private ValuesDataTable tableValues;

			[DebuggerNonUserCode]
			public DateTime DateStamp
			{
				get
				{
					return (DateTime)base[this.tableValues.DateStampColumn];
				}
				set
				{
					base[this.tableValues.DateStampColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			public double Value
			{
				get
				{
					try
					{
						return (double)base[this.tableValues.ValueColumn];
					}
					catch (InvalidCastException innerException)
					{
						throw new StrongTypingException("The value for column 'Value' in table 'Values' is DBNull.", innerException);
					}
				}
				set
				{
					base[this.tableValues.ValueColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			internal ValuesRow(DataRowBuilder rb)
				: base(rb)
			{
				this.tableValues = (ValuesDataTable)base.Table;
			}

			[DebuggerNonUserCode]
			public bool IsValueNull()
			{
				return base.IsNull(this.tableValues.ValueColumn);
			}

			[DebuggerNonUserCode]
			public void SetValueNull()
			{
				base[this.tableValues.ValueColumn] = Convert.DBNull;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "2.0.0.0")]
		internal class ValuesRowChangeEvent : EventArgs
		{
			private ValuesRow eventRow;

			private DataRowAction eventAction;

			[DebuggerNonUserCode]
			public ValuesRow Row
			{
				get
				{
					return this.eventRow;
				}
			}

			[DebuggerNonUserCode]
			public DataRowAction Action
			{
				get
				{
					return this.eventAction;
				}
			}

			[DebuggerNonUserCode]
			public ValuesRowChangeEvent(ValuesRow row, DataRowAction action)
			{
				this.eventRow = row;
				this.eventAction = action;
			}
		}

		private ValuesDataTable tableValues;

		private SchemaSerializationMode _schemaSerializationMode = SchemaSerializationMode.IncludeSchema;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Browsable(false)]
		[DebuggerNonUserCode]
		public ValuesDataTable Values
		{
			get
			{
				return this.tableValues;
			}
		}

		[Browsable(true)]
		[DebuggerNonUserCode]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override SchemaSerializationMode SchemaSerializationMode
		{
			get
			{
				return this._schemaSerializationMode;
			}
			set
			{
				this._schemaSerializationMode = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DebuggerNonUserCode]
		public new DataTableCollection Tables
		{
			get
			{
				return base.Tables;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DebuggerNonUserCode]
		public new DataRelationCollection Relations
		{
			get
			{
				return base.Relations;
			}
		}

		[DebuggerNonUserCode]
		public GaugeData()
		{
			base.BeginInit();
			this.InitClass();
			CollectionChangeEventHandler value = this.SchemaChanged;
			base.Tables.CollectionChanged += value;
			base.Relations.CollectionChanged += value;
			base.EndInit();
		}

		[DebuggerNonUserCode]
		protected GaugeData(SerializationInfo info, StreamingContext context)
			: base(info, context, false)
		{
			if (base.IsBinarySerialized(info, context))
			{
				this.InitVars(false);
				CollectionChangeEventHandler value = this.SchemaChanged;
				this.Tables.CollectionChanged += value;
				this.Relations.CollectionChanged += value;
			}
			else
			{
				string s = (string)info.GetValue("XmlSchema", typeof(string));
				if (base.DetermineSchemaSerializationMode(info, context) == SchemaSerializationMode.IncludeSchema)
				{
					DataSet dataSet = new DataSet();
					dataSet.ReadXmlSchema(new XmlTextReader(new StringReader(s)));
					if (dataSet.Tables["Values"] != null)
					{
						base.Tables.Add(new ValuesDataTable(dataSet.Tables["Values"]));
					}
					base.DataSetName = dataSet.DataSetName;
					base.Prefix = dataSet.Prefix;
					base.Namespace = dataSet.Namespace;
					base.Locale = dataSet.Locale;
					base.CaseSensitive = dataSet.CaseSensitive;
					base.EnforceConstraints = dataSet.EnforceConstraints;
					base.Merge(dataSet, false, MissingSchemaAction.Add);
					this.InitVars();
				}
				else
				{
					base.ReadXmlSchema(new XmlTextReader(new StringReader(s)));
				}
				base.GetSerializationData(info, context);
				CollectionChangeEventHandler value2 = this.SchemaChanged;
				base.Tables.CollectionChanged += value2;
				this.Relations.CollectionChanged += value2;
			}
		}

		[DebuggerNonUserCode]
		protected override void InitializeDerivedDataSet()
		{
			base.BeginInit();
			this.InitClass();
			base.EndInit();
		}

		[DebuggerNonUserCode]
		public override DataSet Clone()
		{
			GaugeData gaugeData = (GaugeData)base.Clone();
			gaugeData.InitVars();
			gaugeData.SchemaSerializationMode = this.SchemaSerializationMode;
			return gaugeData;
		}

		[DebuggerNonUserCode]
		protected override bool ShouldSerializeTables()
		{
			return false;
		}

		[DebuggerNonUserCode]
		protected override bool ShouldSerializeRelations()
		{
			return false;
		}

		[DebuggerNonUserCode]
		protected override void ReadXmlSerializable(XmlReader reader)
		{
			if (base.DetermineSchemaSerializationMode(reader) == SchemaSerializationMode.IncludeSchema)
			{
				this.Reset();
				DataSet dataSet = new DataSet();
				dataSet.ReadXml(reader);
				if (dataSet.Tables["Values"] != null)
				{
					base.Tables.Add(new ValuesDataTable(dataSet.Tables["Values"]));
				}
				base.DataSetName = dataSet.DataSetName;
				base.Prefix = dataSet.Prefix;
				base.Namespace = dataSet.Namespace;
				base.Locale = dataSet.Locale;
				base.CaseSensitive = dataSet.CaseSensitive;
				base.EnforceConstraints = dataSet.EnforceConstraints;
				base.Merge(dataSet, false, MissingSchemaAction.Add);
				this.InitVars();
			}
			else
			{
				base.ReadXml(reader);
				this.InitVars();
			}
		}

		[DebuggerNonUserCode]
		protected override XmlSchema GetSchemaSerializable()
		{
			MemoryStream memoryStream = new MemoryStream();
			base.WriteXmlSchema(new XmlTextWriter(memoryStream, null));
			memoryStream.Position = 0L;
			return XmlSchema.Read(new XmlTextReader(memoryStream), null);
		}

		[DebuggerNonUserCode]
		internal void InitVars()
		{
			this.InitVars(true);
		}

		[DebuggerNonUserCode]
		internal void InitVars(bool initTable)
		{
			this.tableValues = (ValuesDataTable)base.Tables["Values"];
			if (initTable && this.tableValues != null)
			{
				this.tableValues.InitVars();
			}
		}

		[DebuggerNonUserCode]
		private void InitClass()
		{
			base.DataSetName = "GaugeData";
			base.Prefix = "";
			base.Namespace = "http://tempuri.org/GaugeData.xsd";
			base.EnforceConstraints = true;
			this.SchemaSerializationMode = SchemaSerializationMode.IncludeSchema;
			this.tableValues = new ValuesDataTable();
			base.Tables.Add(this.tableValues);
		}

		[DebuggerNonUserCode]
		private bool ShouldSerializeValues()
		{
			return false;
		}

		[DebuggerNonUserCode]
		private void SchemaChanged(object sender, CollectionChangeEventArgs e)
		{
			if (e.Action == CollectionChangeAction.Remove)
			{
				this.InitVars();
			}
		}

		[DebuggerNonUserCode]
		public static XmlSchemaComplexType GetTypedDataSetSchema(XmlSchemaSet xs)
		{
			GaugeData gaugeData = new GaugeData();
			XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
			XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
			xs.Add(gaugeData.GetSchemaSerializable());
			XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
			xmlSchemaAny.Namespace = gaugeData.Namespace;
			xmlSchemaSequence.Items.Add(xmlSchemaAny);
			xmlSchemaComplexType.Particle = xmlSchemaSequence;
			return xmlSchemaComplexType;
		}
	}
}
