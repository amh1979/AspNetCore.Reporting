using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Globalization;
using System.Reflection;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class DataFieldMemberConverter : StringConverter
	{
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return false;
		}

		private void AddToList(ArrayList names, DataTable dataTable, bool includeTableName)
		{
			foreach (DataColumn column in dataTable.Columns)
			{
				if (includeTableName)
				{
					names.Add(dataTable.TableName + "." + column.ColumnName);
				}
				else
				{
					names.Add(column.ColumnName);
				}
			}
		}

		internal static ArrayList GetDataSourceMemberNames(object dataSource)
		{
			ArrayList arrayList = new ArrayList();
			if (dataSource != null)
			{
				try
				{
					if (dataSource.GetType().GetInterface("IDataSource") != null)
					{
						try
						{
							MethodInfo method = dataSource.GetType().GetMethod("Select");
							if (method != null)
							{
								if (method.GetParameters().Length == 1)
								{
									Type type = dataSource.GetType().Assembly.GetType("System.Web.UI.DataSourceSelectArguments", true);
									ConstructorInfo constructor = type.GetConstructor(new Type[0]);
									dataSource = method.Invoke(dataSource, new object[1]
									{
										constructor.Invoke(new object[0])
									});
								}
								else
								{
									dataSource = method.Invoke(dataSource, new object[0]);
								}
							}
						}
						catch
						{
						}
					}
					DataTable dataTable = null;
					if (dataSource is DataTable)
					{
						dataTable = (DataTable)dataSource;
					}
					else if (dataSource is DataView)
					{
						dataTable = ((DataView)dataSource).Table;
					}
					else if (dataSource is DataSet && ((DataSet)dataSource).Tables.Count > 0)
					{
						dataTable = ((DataSet)dataSource).Tables[0];
					}
					else if (dataSource is SqlDataAdapter)
					{
						dataTable = new DataTable();
						dataTable.Locale = CultureInfo.InvariantCulture;
						dataTable = ((SqlDataAdapter)dataSource).FillSchema(dataTable, SchemaType.Mapped);
					}
					else if (dataSource is SqlDataReader)
					{
						for (int j = 0; j < ((SqlDataReader)dataSource).FieldCount; j++)
						{
							arrayList.Add(((SqlDataReader)dataSource).GetName(j));
						}
					}
					else if (dataSource is SqlCommand)
					{
						SqlCommand sqlCommand = (SqlCommand)dataSource;
						if (sqlCommand.Connection != null)
						{
							sqlCommand.Connection.Open();
							SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
							if (sqlDataReader.Read())
							{
								for (int l = 0; l < sqlDataReader.FieldCount; l++)
								{
									arrayList.Add(sqlDataReader.GetName(l));
								}
							}
							sqlDataReader.Close();
							sqlCommand.Connection.Close();
						}
					}
					if (dataTable != null)
					{
						foreach (DataColumn column in dataTable.Columns)
						{
							arrayList.Add(column.ColumnName);
						}
					}
					else if (arrayList.Count == 0 && dataSource is ITypedList)
					{
						foreach (PropertyDescriptor itemProperty in ((ITypedList)dataSource).GetItemProperties(null))
						{
							if (itemProperty.PropertyType != typeof(string))
							{
								arrayList.Add(itemProperty.Name);
							}
						}
					}
					else if (arrayList.Count == 0 && dataSource is IEnumerable)
					{
						IEnumerator enumerator3 = ((IEnumerable)dataSource).GetEnumerator();
						enumerator3.Reset();
						enumerator3.MoveNext();
						foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(enumerator3.Current))
						{
							if (property.PropertyType != typeof(string))
							{
								arrayList.Add(property.Name);
							}
						}
					}
				}
				catch
				{
				}
				if (arrayList.Count == 0)
				{
					arrayList.Add("0");
				}
			}
			return arrayList;
		}
	}
}
