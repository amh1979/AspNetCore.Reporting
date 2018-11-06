using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal abstract class ParameterBase : IPersistable
	{
		internal enum UsedInQueryType
		{
			False,
			True,
			Auto
		}

		internal const string NameXmlElement = "Name";

		internal const string TypeXmlElement = "Type";

		internal const string NullableXmlElement = "Nullable";

		internal const string AllowBlankXmlElement = "AllowBlank";

		internal const string MultiValueXmlElement = "MultiValue";

		internal const string PromptXmlElement = "Prompt";

		internal const string PromptUserXmlElement = "PromptUser";

		internal const string ValueXmlElement = "Value";

		internal const string UsedInQueryXmlElement = "UsedInQuery";

		internal const string DefaultValuesXmlElement = "DefaultValues";

		internal const string ValidValuesXmlElement = "ValidValues";

		internal const string StateXmlElement = "State";

		private string m_name;

		private DataType m_dataType = DataType.String;

		private bool m_nullable;

		private bool m_promptUser;

		private bool m_usedInQuery;

		private bool m_allowBlank;

		private bool m_multiValue;

		private object[] m_defaultValues;

		[NonSerialized]
		private UsedInQueryType m_usedInQueryAsDefined = UsedInQueryType.Auto;

		[NonSerialized]
		private Hashtable m_dependencies;

		[NonSerialized]
		private static readonly AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.Declaration m_Declaration = ParameterBase.GetNewDeclaration();

		internal ObjectType ParameterObjectType
		{
			get
			{
				if (this.m_dataType == DataType.Object)
				{
					return ObjectType.QueryParameter;
				}
				return ObjectType.ReportParameter;
			}
		}

		public string Name
		{
			get
			{
				return this.m_name;
			}
			set
			{
				this.m_name = value;
			}
		}

		public DataType DataType
		{
			get
			{
				return this.m_dataType;
			}
			set
			{
				this.m_dataType = value;
			}
		}

		public bool Nullable
		{
			get
			{
				return this.m_nullable;
			}
			set
			{
				this.m_nullable = value;
			}
		}

		public abstract string Prompt
		{
			get;
			set;
		}

		public bool PromptUser
		{
			get
			{
				return this.m_promptUser;
			}
			set
			{
				this.m_promptUser = value;
			}
		}

		public bool AllowBlank
		{
			get
			{
				return this.m_allowBlank;
			}
			set
			{
				this.m_allowBlank = value;
			}
		}

		public bool MultiValue
		{
			get
			{
				return this.m_multiValue;
			}
			set
			{
				this.m_multiValue = value;
			}
		}

		public object[] DefaultValues
		{
			get
			{
				return this.m_defaultValues;
			}
			set
			{
				this.m_defaultValues = value;
			}
		}

		internal Hashtable Dependencies
		{
			get
			{
				return this.m_dependencies;
			}
			set
			{
				this.m_dependencies = value;
			}
		}

		public bool UsedInQuery
		{
			get
			{
				return this.m_usedInQuery;
			}
			set
			{
				this.m_usedInQuery = value;
			}
		}

		internal UsedInQueryType UsedInQueryAsDefined
		{
			get
			{
				return this.m_usedInQueryAsDefined;
			}
		}

		public ParameterBase()
		{
		}

		internal ParameterBase(AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterValue source)
		{
			this.m_dataType = DataType.Object;
			this.m_name = source.Name;
			this.m_usedInQuery = false;
		}

		internal ParameterBase(DataSetParameterValue source, bool usedInQuery)
		{
			this.m_dataType = DataType.Object;
			this.m_name = source.UniqueName;
			this.m_nullable = source.Nullable;
			this.m_multiValue = source.MultiValue;
			this.m_allowBlank = false;
			this.m_promptUser = !source.ReadOnly;
			this.m_usedInQuery = usedInQuery;
			if (source.Value != null && !source.Value.IsExpression)
			{
				this.m_defaultValues = new object[1];
				this.m_defaultValues[0] = source.Value.Value;
			}
		}

		internal ParameterBase(ParameterBase source)
		{
			this.m_name = source.m_name;
			this.m_dataType = source.m_dataType;
			this.m_nullable = source.m_nullable;
			this.m_promptUser = source.m_promptUser;
			this.m_allowBlank = source.m_allowBlank;
			this.m_multiValue = source.m_multiValue;
			if (source.m_defaultValues != null)
			{
				int num = source.m_defaultValues.Length;
				this.m_defaultValues = new object[num];
				for (int i = 0; i < num; i++)
				{
					this.m_defaultValues[i] = source.m_defaultValues[i];
				}
			}
			this.m_usedInQuery = source.m_usedInQuery;
		}

		internal static AspNetCore.ReportingServices.ReportProcessing.Persistence.Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberName.Name, AspNetCore.ReportingServices.ReportProcessing.Persistence.Token.String));
			memberInfoList.Add(new AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberName.DataType, AspNetCore.ReportingServices.ReportProcessing.Persistence.Token.Enum));
			memberInfoList.Add(new AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberName.Nullable, AspNetCore.ReportingServices.ReportProcessing.Persistence.Token.Boolean));
			memberInfoList.Add(new AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberName.Prompt, AspNetCore.ReportingServices.ReportProcessing.Persistence.Token.String));
			memberInfoList.Add(new AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberName.UsedInQuery, AspNetCore.ReportingServices.ReportProcessing.Persistence.Token.Boolean));
			memberInfoList.Add(new AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberName.AllowBlank, AspNetCore.ReportingServices.ReportProcessing.Persistence.Token.Boolean));
			memberInfoList.Add(new AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberName.MultiValue, AspNetCore.ReportingServices.ReportProcessing.Persistence.Token.Boolean));
			memberInfoList.Add(new AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberName.DefaultValue, AspNetCore.ReportingServices.ReportProcessing.Persistence.Token.Array, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberName.PromptUser, AspNetCore.ReportingServices.ReportProcessing.Persistence.Token.Boolean));
			return new AspNetCore.ReportingServices.ReportProcessing.Persistence.Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}

		internal static AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.Declaration GetNewDeclaration()
		{
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo>();
			list.Add(new AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Name, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.Token.String));
			list.Add(new AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.DataType, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.Token.Enum));
			list.Add(new AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Nullable, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.Token.Boolean));
			list.Add(new AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.UsedInQuery, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.Token.Boolean));
			list.Add(new AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.AllowBlank, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.Token.Boolean));
			list.Add(new AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.MultiValue, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.Token.Boolean));
			list.Add(new AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.DefaultValue, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveArray, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.Token.Object));
			list.Add(new AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.PromptUser, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.Token.Boolean));
			return new AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterBase, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		internal void Serialize(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(ParameterBase.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Name:
					writer.Write(this.m_name);
					break;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.DataType:
					writer.WriteEnum((int)this.m_dataType);
					break;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Nullable:
					writer.Write(this.m_nullable);
					break;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.UsedInQuery:
					writer.Write(this.m_usedInQuery);
					break;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.AllowBlank:
					writer.Write(this.m_allowBlank);
					break;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.MultiValue:
					writer.Write(this.m_multiValue);
					break;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.DefaultValue:
					writer.Write(this.m_defaultValues);
					break;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.PromptUser:
					writer.Write(this.m_promptUser);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		internal void Deserialize(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(ParameterBase.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Name:
					this.m_name = reader.ReadString();
					break;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.DataType:
					this.m_dataType = (DataType)reader.ReadEnum();
					break;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Nullable:
					this.m_nullable = reader.ReadBoolean();
					break;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.UsedInQuery:
					this.m_usedInQuery = reader.ReadBoolean();
					break;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.AllowBlank:
					this.m_allowBlank = reader.ReadBoolean();
					break;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.MultiValue:
					this.m_multiValue = reader.ReadBoolean();
					break;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.DefaultValue:
					this.m_defaultValues = reader.ReadVariantArray();
					break;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.PromptUser:
					this.m_promptUser = reader.ReadBoolean();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		void IPersistable.Serialize(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.IntermediateFormatWriter writer)
		{
			this.Serialize(writer);
		}

		void IPersistable.Deserialize(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.IntermediateFormatReader reader)
		{
			this.Deserialize(reader);
		}

		void IPersistable.ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false);
		}

		AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType IPersistable.GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterBase;
		}

		internal static bool ValidateValueForNull(object newValue, bool nullable, ErrorContext errorContext, ObjectType parameterType, string parameterName, string parameterValueProperty)
		{
			bool result = true;
			bool flag = errorContext is PublishingErrorContext;
			if (newValue == null && !nullable)
			{
				result = false;
				if (errorContext != null)
				{
					errorContext.Register((ProcessingErrorCode)(flag ? 188 : 187), (Severity)((!flag) ? 1 : 0), parameterType, parameterName, "Nullable", parameterValueProperty);
				}
			}
			return result;
		}

		internal bool ValidateValueForBlank(object newValue, ErrorContext errorContext, string parameterValueProperty)
		{
			bool result = true;
			bool flag = errorContext is PublishingErrorContext;
			if (this.DataType == DataType.String && !this.AllowBlank)
			{
				string a = (string)newValue;
				if (a == string.Empty)
				{
					result = false;
					if (errorContext != null)
					{
						errorContext.Register((ProcessingErrorCode)(flag ? 188 : 187), (Severity)((!flag) ? 1 : 0), ObjectType.ReportParameter, this.m_name, "AllowBlank", parameterValueProperty);
					}
				}
			}
			return result;
		}

		internal void ValidateValue(object newValue, ErrorContext errorContext, ObjectType parameterType, string parameterValueProperty)
		{
			ParameterBase.ValidateValueForNull(newValue, this.Nullable, errorContext, parameterType, this.Name, parameterValueProperty);
			this.ValidateValueForBlank(newValue, errorContext, parameterValueProperty);
		}

		internal virtual void Parse(string name, List<string> defaultValues, string type, string nullable, object prompt, string promptUser, string allowBlank, string multiValue, string usedInQuery, bool hidden, ErrorContext errorContext, CultureInfo language)
		{
			if (name != null && name.Length != 0)
			{
				this.m_name = name;
				if (type != null && type.Length != 0)
				{
					try
					{
						this.m_dataType = (DataType)Enum.Parse(typeof(DataType), type, true);
					}
					catch (ArgumentException)
					{
						if (errorContext != null)
						{
							errorContext.Register(ProcessingErrorCode.rsParameterPropertyTypeMismatch, Severity.Error, ObjectType.Parameter, name, "DataType");
							goto end_IL_0050;
						}
						throw new ElementTypeMismatchException("Type");
						end_IL_0050:;
					}
				}
				else
				{
					this.m_dataType = DataType.String;
				}
				if (nullable != null && nullable.Length != 0)
				{
					try
					{
						this.m_nullable = bool.Parse(nullable);
					}
					catch (FormatException)
					{
						if (errorContext != null)
						{
							errorContext.Register(ProcessingErrorCode.rsParameterPropertyTypeMismatch, Severity.Error, this.ParameterObjectType, name, "Nullable");
							goto end_IL_00a5;
						}
						throw new ElementTypeMismatchException("Nullable");
						end_IL_00a5:;
					}
				}
				else
				{
					this.m_nullable = false;
				}
				if (allowBlank != null && allowBlank.Length != 0)
				{
					try
					{
						this.m_allowBlank = bool.Parse(allowBlank);
					}
					catch (FormatException)
					{
						if (errorContext != null)
						{
							errorContext.Register(ProcessingErrorCode.rsParameterPropertyTypeMismatch, Severity.Error, this.ParameterObjectType, name, "AllowBlank");
							goto end_IL_00fe;
						}
						throw new ElementTypeMismatchException("AllowBlank");
						end_IL_00fe:;
					}
				}
				else
				{
					this.m_allowBlank = false;
				}
				if (multiValue != null && multiValue.Length != 0 && this.m_dataType != DataType.Boolean)
				{
					try
					{
						this.m_multiValue = bool.Parse(multiValue);
					}
					catch (FormatException)
					{
						if (errorContext != null)
						{
							errorContext.Register(ProcessingErrorCode.rsParameterPropertyTypeMismatch, Severity.Error, this.ParameterObjectType, name, "MultiValue");
							goto end_IL_0160;
						}
						throw new ElementTypeMismatchException("MultiValue");
						end_IL_0160:;
					}
				}
				else
				{
					this.m_multiValue = false;
				}
				if (promptUser != null && !(promptUser == string.Empty))
				{
					try
					{
						this.m_promptUser = bool.Parse(promptUser);
					}
					catch (FormatException)
					{
						throw new ElementTypeMismatchException("PromptUser");
					}
				}
				else if (prompt == null)
				{
					this.m_promptUser = false;
				}
				else
				{
					this.m_promptUser = true;
				}
				if (defaultValues == null)
				{
					this.m_defaultValues = null;
				}
				else
				{
					int count = defaultValues.Count;
					this.m_defaultValues = new object[count];
					object obj = default(object);
					for (int i = 0; i < count; this.m_defaultValues[i] = obj, i++)
					{
						if (!ParameterBase.CastFromString(defaultValues[i], out obj, this.m_dataType, language))
						{
							if (errorContext != null)
							{
								errorContext.Register(ProcessingErrorCode.rsParameterPropertyTypeMismatch, Severity.Error, this.ParameterObjectType, name, "DefaultValue");
								continue;
							}
							throw new ReportParameterTypeMismatchException(name);
						}
						this.ValidateValue(obj, errorContext, this.ParameterObjectType, "DefaultValue");
					}
				}
				this.m_usedInQuery = true;
				if (usedInQuery != null && usedInQuery.Length != 0)
				{
					try
					{
						this.m_usedInQueryAsDefined = (UsedInQueryType)Enum.Parse(typeof(UsedInQueryType), usedInQuery, true);
					}
					catch (ArgumentException)
					{
						if (errorContext != null)
						{
							errorContext.Register(ProcessingErrorCode.rsParameterPropertyTypeMismatch, Severity.Error, this.ParameterObjectType, name, "MultiValue");
							goto end_IL_02a3;
						}
						throw new ElementTypeMismatchException("UsedInQuery");
						end_IL_02a3:;
					}
					if (this.m_usedInQueryAsDefined == UsedInQueryType.False)
					{
						this.m_usedInQuery = false;
					}
					else if (this.m_usedInQueryAsDefined == UsedInQueryType.True)
					{
						this.m_usedInQuery = true;
					}
				}
				else
				{
					this.m_usedInQueryAsDefined = UsedInQueryType.Auto;
				}
				if (usedInQuery != null && usedInQuery.Length != 0)
				{
					try
					{
						this.m_usedInQueryAsDefined = (UsedInQueryType)Enum.Parse(typeof(UsedInQueryType), usedInQuery, true);
					}
					catch (ArgumentException)
					{
						throw new ElementTypeMismatchException("UsedInQuery");
					}
					if (this.m_usedInQueryAsDefined == UsedInQueryType.False)
					{
						this.m_usedInQuery = false;
					}
					else if (this.m_usedInQueryAsDefined == UsedInQueryType.True)
					{
						this.m_usedInQuery = true;
					}
				}
				else
				{
					this.m_usedInQueryAsDefined = UsedInQueryType.Auto;
				}
				return;
			}
			throw new MissingElementException("Name");
		}

		internal static bool Cast(object oldValue, DataType oldType, out object newValue, DataType newType, CultureInfo language)
		{
			if (oldValue == null)
			{
				newValue = null;
				return true;
			}
			switch (oldType)
			{
			case DataType.Object:
				newValue = oldValue;
				return true;
			case DataType.String:
				return ParameterBase.CastFromString((string)oldValue, out newValue, newType, language);
			case DataType.Boolean:
				return ParameterBase.CastFromBoolean((bool)oldValue, out newValue, newType, language);
			case DataType.Float:
				return ParameterBase.CastFromDouble((double)oldValue, out newValue, newType, language);
			case DataType.DateTime:
				if (oldValue is DateTimeOffset)
				{
					return ParameterBase.CastFromDateTimeOffset((DateTimeOffset)oldValue, out newValue, newType, language);
				}
				return ParameterBase.CastFromDateTime((DateTime)oldValue, out newValue, newType, language);
			case DataType.Integer:
				return ParameterBase.CastFromInteger((int)oldValue, out newValue, newType, language);
			default:
				throw new InternalCatalogException("Parameter type is not one of the supported types in Cast");
			}
		}

		internal static bool DecodeObjectFromBase64String(string originalValue, out object newValue)
		{
			newValue = null;
			if (string.IsNullOrEmpty(originalValue))
			{
				return true;
			}
			try
			{
				byte[] buffer = Convert.FromBase64String(originalValue);
				using (MemoryStream str = new MemoryStream(buffer))
				{
					ProcessingRIFObjectCreator rifObjectCreator = new ProcessingRIFObjectCreator(null, null);
					RIFVariantContainer rIFVariantContainer = (RIFVariantContainer)new AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.IntermediateFormatReader(str, rifObjectCreator).ReadRIFObject();
					newValue = rIFVariantContainer.Value;
				}
			}
			catch (Exception innerException)
			{
				throw new InternalCatalogException(innerException, "Parameter value decoding failed for base64 encoded string='" + originalValue + "'");
			}
			return true;
		}

		public static bool CastFromString(string oldString, out object newValue, DataType newType, CultureInfo language)
		{
			newValue = null;
			if (oldString == null)
			{
				return true;
			}
			switch (newType)
			{
			case DataType.Object:
				return ParameterBase.DecodeObjectFromBase64String(oldString, out newValue);
			case DataType.String:
				newValue = oldString;
				return true;
			case DataType.Boolean:
				if (string.Compare(oldString, "true", true, language) != 0 && string.Compare(oldString, "enable", true, language) != 0 && string.Compare(oldString, "enabled", true, language) != 0 && string.Compare(oldString, "yes", true, language) != 0 && string.Compare(oldString, "on", true, language) != 0 && string.Compare(oldString, "+", true, language) != 0)
				{
					if (string.Compare(oldString, "false", true, language) != 0 && string.Compare(oldString, "disable", true, language) != 0 && string.Compare(oldString, "disabled", true, language) != 0 && string.Compare(oldString, "no", true, language) != 0 && string.Compare(oldString, "off", true, language) != 0 && string.Compare(oldString, "-", true, language) != 0)
					{
						return false;
					}
					newValue = false;
					return true;
				}
				newValue = true;
				return true;
			case DataType.Float:
				try
				{
					newValue = double.Parse(oldString, language);
					return true;
				}
				catch (Exception ex)
				{
					if (!(ex is FormatException) && !(ex is OverflowException))
					{
						throw;
					}
					return false;
				}
			case DataType.Integer:
				try
				{
					newValue = int.Parse(oldString, language);
					return true;
				}
				catch (Exception ex2)
				{
					if (!(ex2 is FormatException) && !(ex2 is OverflowException))
					{
						throw;
					}
					return false;
				}
			case DataType.DateTime:
			{
				DateTimeOffset dateTimeOffset = default(DateTimeOffset);
				bool flag = default(bool);
				if (DateTimeUtil.TryParseDateTime(oldString, language, out dateTimeOffset, out flag))
				{
					if (flag)
					{
						newValue = dateTimeOffset;
					}
					else
					{
						newValue = dateTimeOffset.DateTime;
					}
					return true;
				}
				return false;
			}
			default:
				throw new InternalCatalogException("Parameter type is not one of the supported types in Cast");
			}
		}

		internal static bool CastFromBoolean(bool oldBoolean, out object newValue, DataType newType, CultureInfo language)
		{
			newValue = null;
			switch (newType)
			{
			case DataType.Object:
				newValue = oldBoolean;
				return true;
			case DataType.String:
				newValue = oldBoolean.ToString(language);
				return true;
			case DataType.Boolean:
				newValue = oldBoolean;
				return true;
			case DataType.Float:
				newValue = (double)(oldBoolean ? 1 : 0);
				return true;
			case DataType.Integer:
				newValue = (oldBoolean ? 1 : 0);
				return true;
			case DataType.DateTime:
				return false;
			default:
				throw new InternalCatalogException("Parameter type is not one of the supported types in Cast");
			}
		}

		internal static bool CastFromDouble(double oldDouble, out object newValue, DataType newType, CultureInfo language)
		{
			newValue = null;
			checked
			{
				switch (newType)
				{
				case DataType.Object:
					newValue = oldDouble;
					return true;
				case DataType.String:
					newValue = oldDouble.ToString(language);
					return true;
				case DataType.Boolean:
					newValue = (oldDouble != 0.0);
					return true;
				case DataType.Float:
					newValue = oldDouble;
					return true;
				case DataType.Integer:
					try
					{
						newValue = (int)oldDouble;
					}
					catch (OverflowException)
					{
						return false;
					}
					return true;
				case DataType.DateTime:
					try
					{
						newValue = new DateTime((long)oldDouble);
					}
					catch (Exception ex)
					{
						if (!(ex is ArgumentOutOfRangeException) && !(ex is OverflowException))
						{
							throw;
						}
						return false;
					}
					return true;
				default:
					throw new InternalCatalogException("Parameter type is not one of the supported types in Cast");
				}
			}
		}

		internal static bool CastFromInteger(int oldInteger, out object newValue, DataType newType, CultureInfo language)
		{
			newValue = null;
			switch (newType)
			{
			case DataType.Object:
				newValue = oldInteger;
				return true;
			case DataType.String:
				newValue = oldInteger.ToString(language);
				return true;
			case DataType.Boolean:
				newValue = (oldInteger != 0);
				return true;
			case DataType.Float:
				newValue = (double)oldInteger;
				return true;
			case DataType.Integer:
				newValue = oldInteger;
				return true;
			case DataType.DateTime:
				try
				{
					newValue = new DateTime(oldInteger);
				}
				catch (Exception ex)
				{
					if (!(ex is ArgumentOutOfRangeException) && !(ex is OverflowException))
					{
						throw;
					}
					return false;
				}
				return true;
			default:
				throw new InternalCatalogException("Parameter type is not one of the supported types in Cast");
			}
		}

		internal static bool CastFromDateTime(DateTime oldDateTime, out object newValue, DataType newType, CultureInfo language)
		{
			newValue = null;
			switch (newType)
			{
			case DataType.Object:
				newValue = oldDateTime;
				return true;
			case DataType.String:
				newValue = oldDateTime.ToString(language);
				return true;
			case DataType.Boolean:
				return false;
			case DataType.Float:
				newValue = oldDateTime.Ticks;
				return true;
			case DataType.Integer:
				try
				{
					newValue = Convert.ToInt32(oldDateTime.Ticks);
					return true;
				}
				catch (OverflowException)
				{
					return false;
				}
			case DataType.DateTime:
				newValue = oldDateTime;
				return true;
			default:
				throw new InternalCatalogException("Parameter type is not one of the supported types in Cast");
			}
		}

		internal static bool CastFromDateTimeOffset(DateTimeOffset oldDateTime, out object newValue, DataType newType, CultureInfo language)
		{
			newValue = null;
			switch (newType)
			{
			case DataType.Object:
				newValue = oldDateTime;
				return true;
			case DataType.String:
				newValue = oldDateTime.ToString(language);
				return true;
			case DataType.Boolean:
				return false;
			case DataType.Float:
				return false;
			case DataType.Integer:
				return false;
			case DataType.DateTime:
				newValue = oldDateTime;
				return true;
			default:
				throw new InternalCatalogException("Parameter type is not one of the supported types in Cast");
			}
		}

		public static bool ParameterValuesEqual(object o1, object o2)
		{
			return object.Equals(o1, o2);
		}

		internal static bool IsSharedDataSetParameterObjectType(ObjectType ot)
		{
			switch (ot)
			{
			case ObjectType.QueryParameter:
				return true;
			case ObjectType.ReportParameter:
			case ObjectType.Parameter:
				return false;
			default:
				Global.Tracer.Assert(false, "Unknown ObjectType: {0}", ot);
				return ObjectType.QueryParameter == ot;
			}
		}
	}
}
