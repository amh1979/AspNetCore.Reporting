using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class RecordSetInfo : IPersistable
	{
		private bool m_readerExtensionsSupported;

		private RecordSetPropertyNamesList m_fieldPropertyNames;

		private string[] m_fieldNames;

		private CompareOptions m_compareOptions;

		private string m_commandText;

		private string m_rewrittenCommandText;

		private string m_cultureName;

		private DateTime m_executionTime = DateTime.MinValue;

		[NonSerialized]
		private bool m_validCompareOptions;

		[NonSerialized]
		private static readonly Declaration m_Declaration = RecordSetInfo.GetDeclaration();

		internal bool ReaderExtensionsSupported
		{
			get
			{
				return this.m_readerExtensionsSupported;
			}
		}

		internal RecordSetPropertyNamesList FieldPropertyNames
		{
			get
			{
				return this.m_fieldPropertyNames;
			}
		}

		internal CompareOptions CompareOptions
		{
			get
			{
				return this.m_compareOptions;
			}
		}

		internal string[] FieldNames
		{
			get
			{
				return this.m_fieldNames;
			}
		}

		internal string CommandText
		{
			get
			{
				return this.m_commandText;
			}
		}

		internal string RewrittenCommandText
		{
			get
			{
				return this.m_rewrittenCommandText;
			}
		}

		internal string CultureName
		{
			get
			{
				return this.m_cultureName;
			}
		}

		internal DateTime ExecutionTime
		{
			get
			{
				return this.m_executionTime;
			}
		}

		internal bool ValidCompareOptions
		{
			get
			{
				return this.m_validCompareOptions;
			}
		}

		internal RecordSetInfo()
		{
		}

		internal RecordSetInfo(bool readerExtensionsSupported, bool persistCalculatedFields, DataSetInstance dataSetInstance, DateTime reportExecutionTime)
		{
			this.m_readerExtensionsSupported = readerExtensionsSupported;
			this.m_compareOptions = dataSetInstance.DataSetDef.GetCLRCompareOptions();
			this.m_commandText = dataSetInstance.CommandText;
			this.m_rewrittenCommandText = dataSetInstance.RewrittenCommandText;
			this.m_cultureName = dataSetInstance.DataSetDef.CreateCultureInfoFromLcid().Name;
			this.m_executionTime = dataSetInstance.GetQueryExecutionTime(reportExecutionTime);
			int count = dataSetInstance.DataSetDef.Fields.Count;
			if (count > 0)
			{
				int num = 0;
				if (persistCalculatedFields)
				{
					this.m_fieldNames = new string[count];
				}
				else
				{
					this.m_fieldNames = new string[dataSetInstance.DataSetDef.NonCalculatedFieldCount];
				}
				for (int i = 0; i < count; i++)
				{
					if (persistCalculatedFields || !dataSetInstance.DataSetDef.Fields[i].IsCalculatedField)
					{
						this.m_fieldNames[num++] = dataSetInstance.DataSetDef.Fields[i].Name;
					}
				}
			}
		}

		internal void PopulateExtendedFieldsProperties(DataSetInstance dataSetInstance)
		{
			if (dataSetInstance.FieldInfos != null)
			{
				int num = dataSetInstance.FieldInfos.Length;
				this.m_fieldPropertyNames = new RecordSetPropertyNamesList(num);
				for (int i = 0; i < num; i++)
				{
					FieldInfo fieldInfo = dataSetInstance.FieldInfos[i];
					RecordSetPropertyNames recordSetPropertyNames = null;
					if (fieldInfo != null && fieldInfo.PropertyCount != 0)
					{
						recordSetPropertyNames = new RecordSetPropertyNames();
						recordSetPropertyNames.PropertyNames = new List<string>(fieldInfo.PropertyCount);
						recordSetPropertyNames.PropertyNames.AddRange(fieldInfo.PropertyNames);
					}
					this.m_fieldPropertyNames.Add(recordSetPropertyNames);
				}
			}
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ReaderExtensionsSupported, Token.Boolean));
			list.Add(new MemberInfo(MemberName.FieldPropertyNames, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RecordSetPropertyNames));
			list.Add(new MemberInfo(MemberName.CompareOptions, Token.Enum));
			list.Add(new MemberInfo(MemberName.FieldNames, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveArray, Token.String));
			list.Add(new MemberInfo(MemberName.CommandText, Token.String));
			list.Add(new MemberInfo(MemberName.RewrittenCommandText, Token.String));
			list.Add(new MemberInfo(MemberName.CultureName, Token.String));
			list.Add(new MemberInfo(MemberName.ExecutionTime, Token.DateTime));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RecordSetInfo, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(RecordSetInfo.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ReaderExtensionsSupported:
					writer.Write(this.m_readerExtensionsSupported);
					break;
				case MemberName.FieldPropertyNames:
					writer.Write(this.m_fieldPropertyNames);
					break;
				case MemberName.CompareOptions:
					writer.WriteEnum((int)this.m_compareOptions);
					break;
				case MemberName.FieldNames:
					writer.Write(this.m_fieldNames);
					break;
				case MemberName.CommandText:
					writer.Write(this.m_commandText);
					break;
				case MemberName.RewrittenCommandText:
					writer.Write(this.m_rewrittenCommandText);
					break;
				case MemberName.CultureName:
					writer.Write(this.m_cultureName);
					break;
				case MemberName.ExecutionTime:
					writer.Write(this.m_executionTime);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(RecordSetInfo.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.ReaderExtensionsSupported:
					this.m_readerExtensionsSupported = reader.ReadBoolean();
					break;
				case MemberName.FieldPropertyNames:
					this.m_fieldPropertyNames = reader.ReadListOfRIFObjects<RecordSetPropertyNamesList>();
					break;
				case MemberName.CompareOptions:
					this.m_compareOptions = (CompareOptions)reader.ReadEnum();
					break;
				case MemberName.FieldNames:
					this.m_fieldNames = reader.ReadStringArray();
					break;
				case MemberName.CommandText:
					this.m_commandText = reader.ReadString();
					break;
				case MemberName.RewrittenCommandText:
					this.m_rewrittenCommandText = reader.ReadString();
					break;
				case MemberName.CultureName:
					this.m_cultureName = reader.ReadString();
					break;
				case MemberName.ExecutionTime:
					this.m_executionTime = reader.ReadDateTime();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false);
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RecordSetInfo;
		}
	}
}
