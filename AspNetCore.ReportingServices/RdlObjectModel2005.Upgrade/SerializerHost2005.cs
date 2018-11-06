using AspNetCore.ReportingServices.RdlObjectModel;
using AspNetCore.ReportingServices.RdlObjectModel.Serialization;
using AspNetCore.ReportingServices.RdlObjectModel2008.Upgrade;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlObjectModel2005.Upgrade
{
	internal class SerializerHost2005 : SerializerHost2008
	{
		private List<IUpgradeable> m_upgradeable;

		private List<DataSource2005> m_dataSources;

		private Hashtable m_nameTable;

		private static Type[,] m_substituteTypes;

		public List<IUpgradeable> Upgradeable
		{
			get
			{
				return this.m_upgradeable;
			}
			set
			{
				this.m_upgradeable = value;
			}
		}

		public List<DataSource2005> DataSources
		{
			get
			{
				return this.m_dataSources;
			}
			set
			{
				this.m_dataSources = value;
			}
		}

		public Hashtable NameTable
		{
			get
			{
				return this.m_nameTable;
			}
			set
			{
				this.m_nameTable = value;
			}
		}

		public override Type GetSubstituteType(Type type)
		{
			return SerializerHost2005.GetSubstituteType(type, base.m_serializing);
		}

		public static Type GetSubstituteType(Type type, bool serializing)
		{
			if (!serializing)
			{
				if (type == typeof(Field))
				{
					return typeof(FieldEx);
				}
				for (int i = 0; i < SerializerHost2005.m_substituteTypes.GetLength(0); i++)
				{
					if (type == SerializerHost2005.m_substituteTypes[i, 0])
					{
						return SerializerHost2005.m_substituteTypes[i, 1];
					}
				}
			}
			else
			{
				if (type.BaseType == typeof(Tablix))
				{
					return typeof(Tablix);
				}
				for (int j = 0; j < SerializerHost2005.m_substituteTypes.GetLength(0); j++)
				{
					if (type == SerializerHost2005.m_substituteTypes[j, 1])
					{
						return SerializerHost2005.m_substituteTypes[j, 0];
					}
				}
			}
			return type;
		}

		public override void OnDeserialization(object value)
		{
			if (base.m_extraStringData != null)
			{
				if (value is IExpression)
				{
					IExpression expression = (IExpression)value;
					expression.Expression += base.m_extraStringData;
				}
				base.m_extraStringData = null;
			}
			if (this.m_nameTable != null && value is IGlobalNamedObject)
			{
				this.m_nameTable[((IGlobalNamedObject)value).Name] = value;
			}
			if (this.m_upgradeable != null)
			{
				if (this.m_dataSources != null && value is DataSource2005)
				{
					this.m_dataSources.Add((DataSource2005)value);
				}
				else if (value is IUpgradeable)
				{
					this.m_upgradeable.Add((IUpgradeable)value);
				}
			}
			base.OnDeserialization(value);
		}

		public override IEnumerable<ExtensionNamespace> GetExtensionNamespaces()
		{
			return new ExtensionNamespace[1]
			{
				new ExtensionNamespace("rd", "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner", false)
			};
		}

		public SerializerHost2005(bool serializing)
			: base(serializing)
		{
		}

		static SerializerHost2005()
		{
			Type[,] array = new Type[20, 2];
			Type[,] array2 = array;
			Type typeFromHandle = typeof(Report);
			array2[0, 0] = typeFromHandle;
			Type[,] array3 = array;
			Type typeFromHandle2 = typeof(Report2005);
			array3[0, 1] = typeFromHandle2;
			Type[,] array4 = array;
			Type typeFromHandle3 = typeof(Body);
			array4[1, 0] = typeFromHandle3;
			Type[,] array5 = array;
			Type typeFromHandle4 = typeof(Body2005);
			array5[1, 1] = typeFromHandle4;
			Type[,] array6 = array;
			Type typeFromHandle5 = typeof(Rectangle);
			array6[2, 0] = typeFromHandle5;
			Type[,] array7 = array;
			Type typeFromHandle6 = typeof(Rectangle2005);
			array7[2, 1] = typeFromHandle6;
			Type[,] array8 = array;
			Type typeFromHandle7 = typeof(Textbox);
			array8[3, 0] = typeFromHandle7;
			Type[,] array9 = array;
			Type typeFromHandle8 = typeof(Textbox2005);
			array9[3, 1] = typeFromHandle8;
			Type[,] array10 = array;
			Type typeFromHandle9 = typeof(Image);
			array10[4, 0] = typeFromHandle9;
			Type[,] array11 = array;
			Type typeFromHandle10 = typeof(Image2005);
			array11[4, 1] = typeFromHandle10;
			Type[,] array12 = array;
			Type typeFromHandle11 = typeof(Line);
			array12[5, 0] = typeFromHandle11;
			Type[,] array13 = array;
			Type typeFromHandle12 = typeof(Line2005);
			array13[5, 1] = typeFromHandle12;
			Type[,] array14 = array;
			Type typeFromHandle13 = typeof(Chart);
			array14[6, 0] = typeFromHandle13;
			Type[,] array15 = array;
			Type typeFromHandle14 = typeof(Chart2005);
			array15[6, 1] = typeFromHandle14;
			Type[,] array16 = array;
			Type typeFromHandle15 = typeof(CustomReportItem);
			array16[7, 0] = typeFromHandle15;
			Type[,] array17 = array;
			Type typeFromHandle16 = typeof(CustomReportItem2005);
			array17[7, 1] = typeFromHandle16;
			Type[,] array18 = array;
			Type typeFromHandle17 = typeof(Style);
			array18[8, 0] = typeFromHandle17;
			Type[,] array19 = array;
			Type typeFromHandle18 = typeof(Style2005);
			array19[8, 1] = typeFromHandle18;
			Type[,] array20 = array;
			Type typeFromHandle19 = typeof(BackgroundImage);
			array20[9, 0] = typeFromHandle19;
			Type[,] array21 = array;
			Type typeFromHandle20 = typeof(BackgroundImage2005);
			array21[9, 1] = typeFromHandle20;
			Type[,] array22 = array;
			Type typeFromHandle21 = typeof(Group);
			array22[10, 0] = typeFromHandle21;
			Type[,] array23 = array;
			Type typeFromHandle22 = typeof(Grouping2005);
			array23[10, 1] = typeFromHandle22;
			Type[,] array24 = array;
			Type typeFromHandle23 = typeof(SortExpression);
			array24[11, 0] = typeFromHandle23;
			Type[,] array25 = array;
			Type typeFromHandle24 = typeof(SortBy2005);
			array25[11, 1] = typeFromHandle24;
			Type[,] array26 = array;
			Type typeFromHandle25 = typeof(ChartDataPoint);
			array26[12, 0] = typeFromHandle25;
			Type[,] array27 = array;
			Type typeFromHandle26 = typeof(DataPoint2005);
			array27[12, 1] = typeFromHandle26;
			Type[,] array28 = array;
			Type typeFromHandle27 = typeof(CustomData);
			array28[13, 0] = typeFromHandle27;
			Type[,] array29 = array;
			Type typeFromHandle28 = typeof(CustomData2005);
			array29[13, 1] = typeFromHandle28;
			Type[,] array30 = array;
			Type typeFromHandle29 = typeof(DataHierarchy);
			array30[14, 0] = typeFromHandle29;
			Type[,] array31 = array;
			Type typeFromHandle30 = typeof(DataGroupings2005);
			array31[14, 1] = typeFromHandle30;
			Type[,] array32 = array;
			Type typeFromHandle31 = typeof(DataMember);
			array32[15, 0] = typeFromHandle31;
			Type[,] array33 = array;
			Type typeFromHandle32 = typeof(DataGrouping2005);
			array33[15, 1] = typeFromHandle32;
			Type[,] array34 = array;
			Type typeFromHandle33 = typeof(Subreport);
			array34[16, 0] = typeFromHandle33;
			Type[,] array35 = array;
			Type typeFromHandle34 = typeof(Subreport2005);
			array35[16, 1] = typeFromHandle34;
			Type[,] array36 = array;
			Type typeFromHandle35 = typeof(DataSource);
			array36[17, 0] = typeFromHandle35;
			Type[,] array37 = array;
			Type typeFromHandle36 = typeof(DataSource2005);
			array37[17, 1] = typeFromHandle36;
			Type[,] array38 = array;
			Type typeFromHandle37 = typeof(Query);
			array38[18, 0] = typeFromHandle37;
			Type[,] array39 = array;
			Type typeFromHandle38 = typeof(Query2005);
			array39[18, 1] = typeFromHandle38;
			Type[,] array40 = array;
			Type typeFromHandle39 = typeof(ReportParameter);
			array40[19, 0] = typeFromHandle39;
			Type[,] array41 = array;
			Type typeFromHandle40 = typeof(ReportParameter2005);
			array41[19, 1] = typeFromHandle40;
			SerializerHost2005.m_substituteTypes = array;
		}
	}
}
