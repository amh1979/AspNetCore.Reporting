using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using AspNetCore.ReportingServices.ReportRendering;
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Xml;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class OWCChartInstanceInfo : ReportItemInstanceInfo
	{
		private VariantList[] m_chartData;

		private string m_noRows;

		internal VariantList this[int index]
		{
			get
			{
				if (0 <= index && index < this.m_chartData.Length)
				{
					return this.m_chartData[index];
				}
				throw new InvalidOperationException();
			}
		}

		internal VariantList[] ChartData
		{
			get
			{
				return this.m_chartData;
			}
			set
			{
				this.m_chartData = value;
			}
		}

		internal int Size
		{
			get
			{
				Global.Tracer.Assert(0 < this.m_chartData.Length);
				return this.m_chartData[0].Count;
			}
		}

		internal string NoRows
		{
			get
			{
				return this.m_noRows;
			}
			set
			{
				this.m_noRows = value;
			}
		}

		internal OWCChartInstanceInfo(ReportProcessing.ProcessingContext pc, OWCChart reportItemDef, OWCChartInstance owner)
			: base(pc, reportItemDef, owner, false)
		{
			this.m_chartData = new VariantList[reportItemDef.ChartData.Count];
			for (int i = 0; i < reportItemDef.ChartData.Count; i++)
			{
				this.m_chartData[i] = new VariantList();
			}
			this.m_noRows = pc.ReportRuntime.EvaluateDataRegionNoRowsExpression(reportItemDef, reportItemDef.ObjectType, reportItemDef.Name, "NoRows");
		}

		internal OWCChartInstanceInfo(ReportProcessing.ProcessingContext pc, OWCChart reportItemDef, OWCChartInstance owner, VariantList[] chartData)
			: base(pc, reportItemDef, owner, false)
		{
			this.m_chartData = chartData;
			this.m_noRows = pc.ReportRuntime.EvaluateDataRegionNoRowsExpression(reportItemDef, reportItemDef.ObjectType, reportItemDef.Name, "NoRows");
		}

		internal OWCChartInstanceInfo(OWCChart reportItemDef)
			: base(reportItemDef)
		{
		}

		internal void ChartDataXML(IChartStream chartStream)
		{
			OWCChart oWCChart = (OWCChart)base.m_reportItemDef;
			int count = this.m_chartData[0].Count;
			int i = 0;
			int num = 0;
			string value = string.Empty;
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
			xmlTextWriter.WriteStartElement("xml");
			xmlTextWriter.WriteAttributeString("xmlns", "s", null, "uuid:BDC6E3F0-6DA3-11d1-A2A3-00AA00C14882");
			xmlTextWriter.WriteAttributeString("xmlns", "dt", null, "uuid:C2F41010-65B3-11d1-A29F-00AA00C14882");
			xmlTextWriter.WriteAttributeString("xmlns", "rs", null, "urn:schemas-microsoft-com:rowset");
			xmlTextWriter.WriteAttributeString("xmlns", "z", null, "#RowsetSchema");
			xmlTextWriter.WriteStartElement("s", "Schema", null);
			xmlTextWriter.WriteAttributeString("id", "RowsetSchema");
			xmlTextWriter.WriteStartElement("s", "ElementType", null);
			xmlTextWriter.WriteAttributeString("name", "row");
			xmlTextWriter.WriteAttributeString("content", "eltOnly");
			for (i = 0; i < oWCChart.ChartData.Count; i++)
			{
				xmlTextWriter.WriteStartElement("s", "AttributeType", null);
				xmlTextWriter.WriteAttributeString("name", "c" + i.ToString(CultureInfo.InvariantCulture));
				xmlTextWriter.WriteAttributeString("rs", "name", null, oWCChart.ChartData[i].Name);
				xmlTextWriter.WriteAttributeString("rs", "nullable", null, "true");
				xmlTextWriter.WriteAttributeString("rs", "writeunknown", null, "true");
				xmlTextWriter.WriteStartElement("s", "datatype", null);
				for (num = 0; num < this.m_chartData[i].Count && ((ArrayList)this.m_chartData[i])[num] == null; num++)
				{
				}
				if (num < this.m_chartData[i].Count)
				{
					Type type = ((ArrayList)this.m_chartData[i])[num].GetType();
					switch (Type.GetTypeCode(type))
					{
					case TypeCode.Boolean:
						value = "boolean";
						break;
					case TypeCode.Byte:
						value = "ui1";
						break;
					case TypeCode.Char:
						value = "char";
						break;
					case TypeCode.DateTime:
						value = "dateTime";
						break;
					case TypeCode.Single:
						value = "r4";
						break;
					case TypeCode.Double:
						value = "float";
						break;
					case TypeCode.Decimal:
						value = "r8";
						break;
					case TypeCode.Int16:
						value = "i2";
						break;
					case TypeCode.Int32:
						value = "i4";
						break;
					case TypeCode.Int64:
						value = "i8";
						break;
					case TypeCode.Object:
						if (((ArrayList)this.m_chartData[i])[num] is TimeSpan)
						{
							value = "time";
						}
						else if (((ArrayList)this.m_chartData[i])[num] is byte[])
						{
							value = "bin.hex";
						}
						break;
					case TypeCode.SByte:
						value = "i1";
						break;
					case TypeCode.UInt16:
						value = "ui2";
						break;
					case TypeCode.UInt32:
						value = "ui4";
						break;
					case TypeCode.UInt64:
						value = "ui8";
						break;
					default:
						value = "string";
						break;
					}
				}
				else
				{
					value = "string";
				}
				xmlTextWriter.WriteAttributeString("dt", "type", null, value);
				xmlTextWriter.WriteAttributeString("rs", "fixedlength", null, "true");
				xmlTextWriter.WriteEndElement();
				xmlTextWriter.WriteEndElement();
			}
			xmlTextWriter.WriteStartElement("s", "extends", null);
			xmlTextWriter.WriteAttributeString("type", "rs:rowbase");
			xmlTextWriter.WriteEndElement();
			xmlTextWriter.WriteEndElement();
			xmlTextWriter.WriteEndElement();
			xmlTextWriter.WriteStartElement("rs", "data", null);
			bool flag = true;
			object obj = null;
			for (int j = 0; j < count; j++)
			{
				i = 0;
				while (i < oWCChart.ChartData.Count)
				{
					if (((ArrayList)this.m_chartData[i])[j] == null)
					{
						i++;
						continue;
					}
					flag = false;
					break;
				}
				if (!flag)
				{
					xmlTextWriter.WriteStartElement("z", "row", null);
					for (i = 0; i < oWCChart.ChartData.Count; i++)
					{
						obj = ((ArrayList)this.m_chartData[i])[j];
						if (obj != null)
						{
							string value2 = (obj is IFormattable) ? ((IFormattable)obj).ToString(null, CultureInfo.InvariantCulture) : obj.ToString();
							xmlTextWriter.WriteAttributeString("c" + i.ToString(CultureInfo.InvariantCulture), value2);
						}
					}
					xmlTextWriter.WriteEndElement();
					xmlTextWriter.WriteWhitespace("\r\n");
					flag = true;
				}
			}
			xmlTextWriter.WriteEndElement();
			xmlTextWriter.WriteEndElement();
			chartStream.Write(stringWriter.ToString());
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.ChartData, Token.Array, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.VariantList));
			memberInfoList.Add(new MemberInfo(MemberName.NoRows, Token.String));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemInstanceInfo, memberInfoList);
		}
	}
}
