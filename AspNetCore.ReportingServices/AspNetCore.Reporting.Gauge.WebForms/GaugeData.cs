using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	[Serializable]
	[ToolboxItem(false)]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	internal sealed class GaugeData : DataSet
	{
		private ValuesDataTable tableValues;

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ValuesDataTable Values
		{
			get
			{
				return this.tableValues;
			}
		}

		public GaugeData()
		{
			this.InitClass();
			CollectionChangeEventHandler value = this.SchemaChanged;
			base.Tables.CollectionChanged += value;
			base.Relations.CollectionChanged += value;
		}

		private GaugeData(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			string text = (string)info.GetValue("XmlSchema", typeof(string));
			if (text != null)
			{
				DataSet dataSet = new DataSet();
				dataSet.ReadXmlSchema(new XmlTextReader(new StringReader(text)));
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
				this.InitClass();
			}
			base.GetSerializationData(info, context);
			CollectionChangeEventHandler value = this.SchemaChanged;
			base.Tables.CollectionChanged += value;
			base.Relations.CollectionChanged += value;
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}

		public override DataSet Clone()
		{
			GaugeData gaugeData = (GaugeData)base.Clone();
			gaugeData.InitVars();
			return gaugeData;
		}

		protected override bool ShouldSerializeTables()
		{
			return false;
		}

		protected override bool ShouldSerializeRelations()
		{
			return false;
		}

		protected override void ReadXmlSerializable(XmlReader reader)
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

		protected override XmlSchema GetSchemaSerializable()
		{
			MemoryStream memoryStream = new MemoryStream();
			base.WriteXmlSchema(new XmlTextWriter(memoryStream, null));
			memoryStream.Position = 0L;
			return XmlSchema.Read(new XmlTextReader(memoryStream), null);
		}

		internal void InitVars()
		{
			this.tableValues = (ValuesDataTable)base.Tables["Values"];
			if (this.tableValues != null)
			{
				this.tableValues.InitVars();
			}
		}

		private void InitClass()
		{
			base.DataSetName = "GaugeData";
			base.Prefix = "";
			base.Namespace = "http://tempuri.org/GaugeData.xsd";
			base.Locale = new CultureInfo("en-US");
			base.CaseSensitive = false;
			base.EnforceConstraints = true;
			this.tableValues = new ValuesDataTable();
			base.Tables.Add(this.tableValues);
		}

		private bool ShouldSerializeValues()
		{
			return false;
		}

		private void SchemaChanged(object sender, CollectionChangeEventArgs e)
		{
			if (e.Action == CollectionChangeAction.Remove)
			{
				this.InitVars();
			}
		}
	}
}
