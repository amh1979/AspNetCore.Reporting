using Microsoft.Cloud.Platform.Utils;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Xml;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ParameterInfo : ParameterBase, IPersistable
	{
		internal const string ParametersXmlElement = "Parameters";

		internal const string ParameterXmlElement = "Parameter";

		internal const string ValidValueXmlElement = "ValidValue";

		internal const string LabelXmlElement = "Label";

		internal const string ValuesXmlElement = "Values";

		internal const string ParametersLayoutXmlElement = "ParametersLayout";

		internal const string ParametersGridLayoutDefinitionXmlElement = "ParametersGridLayoutDefinition";

		internal const string ColumnsDefinitionXmlElement = "ColumnsDefinition";

		internal const string NumberOfColumnsXmlElement = "NumberOfColumns";

		internal const string NumberOfRowsXmlElement = "NumberOfRows";

		internal const string CellDefinitionsXmlElement = "CellDefinitions";

		internal const string CellDefinitionXmlElement = "CellDefinition";

		internal const string RowIndexXmlElement = "RowIndex";

		internal const string ColumnsIndexXmlElement = "ColumnIndex";

		internal const string ParameterNameXmlElement = "ParameterName";

		internal const string DynamicValidValuesXmlElement = "DynamicValidValues";

		internal const string DynamicDefaultValueXmlElement = "DynamicDefaultValue";

		internal const string DynamicPromptXmlElement = "DynamicPrompt";

		internal const string DependenciesXmlElement = "Dependencies";

		internal const string DependencyXmlElement = "Dependency";

		internal const string UserProfileStateElement = "UserProfileState";

		internal const string UseExplicitDefaultValueXmlElement = "UseExplicitDefaultValue";

		internal const string ValuesChangedXmlElement = "ValuesChanged";

		internal const string IsUserSuppliedXmlElement = "IsUserSupplied";

		internal const string NilXmlAttribute = "nil";

		private object[] m_values;

		private string[] m_labels;

		private bool m_isUserSupplied;

		private bool m_dynamicValidValues;

		private bool m_dynamicDefaultValue;

		private bool m_dynamicPrompt;

		private string m_prompt;

		private ParameterInfoCollection m_dependencyList;

		private ValidValueList m_validValues;

		private int[] m_dependencyIndexList;

		[NonSerialized]
		private bool m_valuesChanged;

		[NonSerialized]
		private ReportParameterState m_state = ReportParameterState.MissingValidValue;

		[NonSerialized]
		private bool m_othersDependOnMe;

		[NonSerialized]
		private bool m_useExplicitDefaultValue;

		[NonSerialized]
		private int m_indexInCollection = -1;

		[NonSerialized]
		private bool m_missingUpstreamDataSourcePrompt;

		[NonSerialized]
		private static readonly AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.Declaration m_Declaration = ParameterInfo.GetNewDeclaration();

		public object[] Values
		{
			get
			{
				return this.m_values;
			}
			set
			{
				this.m_values = value;
			}
		}

		public string[] Labels
		{
			get
			{
				return this.m_labels;
			}
			set
			{
				this.m_labels = value;
			}
		}

		public ValidValueList ValidValues
		{
			get
			{
				return this.m_validValues;
			}
			set
			{
				this.m_validValues = value;
			}
		}

		public bool DynamicValidValues
		{
			get
			{
				return this.m_dynamicValidValues;
			}
			set
			{
				this.m_dynamicValidValues = value;
			}
		}

		public bool DynamicDefaultValue
		{
			get
			{
				return this.m_dynamicDefaultValue;
			}
			set
			{
				this.m_dynamicDefaultValue = value;
			}
		}

		public bool UseExplicitDefaultValue
		{
			get
			{
				return this.m_useExplicitDefaultValue;
			}
			set
			{
				this.m_useExplicitDefaultValue = value;
			}
		}

		public ParameterInfoCollection DependencyList
		{
			get
			{
				return this.m_dependencyList;
			}
			set
			{
				this.m_dependencyList = value;
			}
		}

		internal bool IsUserSupplied
		{
			get
			{
				return this.m_isUserSupplied;
			}
			set
			{
				this.m_isUserSupplied = value;
			}
		}

		internal bool ValuesChanged
		{
			get
			{
				return this.m_valuesChanged;
			}
			set
			{
				this.m_valuesChanged = value;
			}
		}

		public override string Prompt
		{
			get
			{
				return this.m_prompt;
			}
			set
			{
				this.m_prompt = value;
			}
		}

		public bool DynamicPrompt
		{
			get
			{
				return this.m_dynamicPrompt;
			}
			set
			{
				this.m_dynamicPrompt = value;
			}
		}

		public ReportParameterState State
		{
			get
			{
				return this.m_state;
			}
			set
			{
				this.m_state = value;
			}
		}

		public bool OthersDependOnMe
		{
			get
			{
				return this.m_othersDependOnMe;
			}
			set
			{
				this.m_othersDependOnMe = value;
			}
		}

		public bool IsVisible
		{
			get
			{
				if (base.PromptUser && this.Prompt != null)
				{
					return this.Prompt.Length > 0;
				}
				return false;
			}
		}

		internal int IndexInCollection
		{
			get
			{
				return this.m_indexInCollection;
			}
			set
			{
				this.m_indexInCollection = value;
			}
		}

		internal bool MissingUpstreamDataSourcePrompt
		{
			get
			{
				return this.m_missingUpstreamDataSourcePrompt;
			}
			set
			{
				this.m_missingUpstreamDataSourcePrompt = value;
			}
		}

		public ParameterInfo()
		{
		}

		internal ParameterInfo(ParameterInfo source)
			: base(source)
		{
			this.m_isUserSupplied = source.m_isUserSupplied;
			this.m_valuesChanged = source.m_valuesChanged;
			this.m_dynamicValidValues = source.m_dynamicValidValues;
			this.m_dynamicDefaultValue = source.m_dynamicDefaultValue;
			this.m_state = source.State;
			this.m_othersDependOnMe = source.m_othersDependOnMe;
			this.m_useExplicitDefaultValue = source.m_useExplicitDefaultValue;
			this.m_prompt = source.m_prompt;
			this.m_dynamicPrompt = source.m_dynamicPrompt;
			if (source.m_values != null)
			{
				int num = source.m_values.Length;
				this.m_values = new object[num];
				for (int i = 0; i < num; i++)
				{
					this.m_values[i] = source.m_values[i];
				}
			}
			if (source.m_labels != null)
			{
				int num2 = source.m_labels.Length;
				this.m_labels = new string[num2];
				for (int j = 0; j < num2; j++)
				{
					this.m_labels[j] = source.m_labels[j];
				}
			}
			if (source.m_dependencyList != null)
			{
				int count = source.m_dependencyList.Count;
				this.m_dependencyList = new ParameterInfoCollection(count);
				for (int k = 0; k < count; k++)
				{
					this.m_dependencyList.Add(source.m_dependencyList[k]);
				}
			}
			if (source.m_validValues != null)
			{
				int count2 = source.m_validValues.Count;
				this.m_validValues = new ValidValueList(count2);
				for (int l = 0; l < count2; l++)
				{
					this.m_validValues.Add(source.m_validValues[l]);
				}
			}
		}

		internal ParameterInfo(ParameterBase source)
			: base(source)
		{
			this.m_prompt = source.Prompt;
		}

		internal ParameterInfo(DataSetParameterValue source, bool usedInQuery)
			: base(source, usedInQuery)
		{
		}

		internal ParameterInfo(AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterValue source)
			: base(source)
		{
			this.m_isUserSupplied = true;
		}

		internal new static AspNetCore.ReportingServices.ReportProcessing.Persistence.Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberName.IsUserSupplied, AspNetCore.ReportingServices.ReportProcessing.Persistence.Token.Boolean));
			memberInfoList.Add(new AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberName.Value, AspNetCore.ReportingServices.ReportProcessing.Persistence.Token.Array, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberName.DynamicValidValues, AspNetCore.ReportingServices.ReportProcessing.Persistence.Token.Boolean));
			memberInfoList.Add(new AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberName.DynamicDefaultValue, AspNetCore.ReportingServices.ReportProcessing.Persistence.Token.Boolean));
			memberInfoList.Add(new AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberName.DependencyList, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ParameterInfoCollection));
			memberInfoList.Add(new AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberName.ValidValues, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ValidValueList));
			memberInfoList.Add(new AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberName.Label, AspNetCore.ReportingServices.ReportProcessing.Persistence.Token.Array, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.String));
			return new AspNetCore.ReportingServices.ReportProcessing.Persistence.Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ParameterBase, memberInfoList);
		}

		[SkipMemberStaticValidation(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.DependencyList)]
		internal new static AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.Declaration GetNewDeclaration()
		{
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo>();
			list.Add(new AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.IsUserSupplied, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.Token.Boolean));
			list.Add(new AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Value, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveArray, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.Token.Object));
			list.Add(new AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.DynamicValidValues, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.Token.Boolean));
			list.Add(new AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.DynamicDefaultValue, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.Token.Boolean));
			list.Add(new ReadOnlyMemberInfo(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.DependencyList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterInfo));
			list.Add(new AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.ValidValues, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ValidValue));
			list.Add(new AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Label, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveArray, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.Token.String));
			list.Add(new AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Prompt, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.Token.String));
			list.Add(new AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.DynamicPrompt, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.Token.Boolean));
			list.Add(new AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.DependencyIndexList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.Token.Int32));
			return new AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterInfo, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterBase, list);
		}

		void IPersistable.Serialize(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(ParameterInfo.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Prompt:
					writer.Write(this.m_prompt);
					break;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.DynamicPrompt:
					writer.Write(this.m_dynamicPrompt);
					break;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.IsUserSupplied:
					writer.Write(this.m_isUserSupplied);
					break;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Value:
					writer.Write(this.m_values);
					break;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.DynamicValidValues:
					writer.Write(this.m_dynamicValidValues);
					break;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.DynamicDefaultValue:
					writer.Write(this.m_dynamicDefaultValue);
					break;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.ValidValues:
					writer.Write(this.m_validValues);
					break;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Label:
					writer.Write(this.m_labels);
					break;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.DependencyIndexList:
					this.m_dependencyIndexList = null;
					if (this.m_dependencyList != null)
					{
						this.m_dependencyIndexList = new int[this.m_dependencyList.Count];
						for (int i = 0; i < this.m_dependencyList.Count; i++)
						{
							this.m_dependencyIndexList[i] = this.m_dependencyList[i].IndexInCollection;
						}
					}
					writer.Write(this.m_dependencyIndexList);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		void IPersistable.Deserialize(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(ParameterInfo.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Prompt:
					this.m_prompt = reader.ReadString();
					break;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.DynamicPrompt:
					this.m_dynamicPrompt = reader.ReadBoolean();
					break;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.IsUserSupplied:
					this.m_isUserSupplied = reader.ReadBoolean();
					break;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Value:
					this.m_values = reader.ReadVariantArray();
					break;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.DynamicValidValues:
					this.m_dynamicValidValues = reader.ReadBoolean();
					break;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.DynamicDefaultValue:
					this.m_dynamicDefaultValue = reader.ReadBoolean();
					break;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.DependencyList:
					this.m_dependencyList = reader.ReadListOfRIFObjects<ParameterInfoCollection>();
					break;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.ValidValues:
					this.m_validValues = reader.ReadListOfRIFObjects<ValidValueList>();
					break;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Label:
					this.m_labels = reader.ReadStringArray();
					break;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.DependencyIndexList:
					this.m_dependencyIndexList = reader.ReadInt32Array();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		void IPersistable.ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false);
		}

		AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType IPersistable.GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterInfo;
		}

		internal void ResolveDependencies(ParameterInfoCollection containingCollection)
		{
			if (this.m_dependencyIndexList != null)
			{
				this.m_dependencyList = new ParameterInfoCollection(this.m_dependencyIndexList.Length);
				for (int i = 0; i < this.m_dependencyIndexList.Length; i++)
				{
					this.m_dependencyList.Add(containingCollection[this.m_dependencyIndexList[i]]);
				}
			}
			this.m_dependencyIndexList = null;
		}

		public void SetValuesFromQueryParameter(object value)
		{
			this.m_values = (value as object[]);
			if (this.m_values == null)
			{
				this.m_values = new object[1];
				this.m_values[0] = value;
			}
		}

		public bool AllDependenciesSpecified()
		{
			return this.CalculateDependencyStatus() == ReportParameterDependencyState.AllDependenciesSpecified;
		}

		internal ReportParameterDependencyState CalculateDependencyStatus()
		{
			ReportParameterDependencyState result = ReportParameterDependencyState.AllDependenciesSpecified;
			if (this.DependencyList != null)
			{
				for (int i = 0; i < this.DependencyList.Count; i++)
				{
					ParameterInfo parameterInfo = this.DependencyList[i];
					if (parameterInfo.MissingUpstreamDataSourcePrompt)
					{
						result = ReportParameterDependencyState.MissingUpstreamDataSourcePrompt;
						break;
					}
					if (parameterInfo.State != 0)
					{
						result = ReportParameterDependencyState.HasOutstandingDependencies;
					}
				}
			}
			return result;
		}

		public bool ValueIsValid()
		{
			if (this.Values != null && this.Values.Length != 0)
			{
				for (int i = 0; i < this.Values.Length; i++)
				{
					object obj = this.Values[i];
					if (!base.Nullable && obj == null)
					{
						if (Global.Tracer.TraceVerbose)
						{
							Global.Tracer.Trace(TraceLevel.Verbose, "Value provided for parameter '{0}' is null and parameter is not nullable.", base.Name.MarkAsPrivate());
						}
						return false;
					}
					if (base.DataType == DataType.String && !base.AllowBlank && obj != null && ((string)obj).Length == 0)
					{
						if (Global.Tracer.TraceVerbose)
						{
							Global.Tracer.Trace(TraceLevel.Verbose, "Value provided for string parameter '{0}' is either null or blank and parameter does not allow blanks.", base.Name.MarkAsPrivate());
						}
						return false;
					}
					if (this.ValidValues != null)
					{
						bool flag = false;
						int num = 0;
						while (num < this.ValidValues.Count)
						{
							if (!ParameterBase.ParameterValuesEqual(obj, this.ValidValues[num].Value))
							{
								num++;
								continue;
							}
							flag = true;
							break;
						}
						if (!flag)
						{
							if (Global.Tracer.TraceVerbose)
							{
								Global.Tracer.Trace(TraceLevel.Verbose, "The provided value '{0}' for parameter '{1}' is not a valid value.", obj.ToString().MarkAsPrivate(), base.Name.MarkAsPrivate());
							}
							return false;
						}
					}
				}
				return true;
			}
			return false;
		}

		internal void StoreLabels()
		{
			this.EnsureLabelsAreGenerated();
			if (this.Values != null)
			{
				this.m_labels = new string[this.Values.Length];
				for (int i = 0; i < this.Values.Length; i++)
				{
					string text = null;
					object obj = this.Values[i];
					bool flag = false;
					if (this.ValidValues != null)
					{
						int num = 0;
						while (num < this.ValidValues.Count)
						{
							if (!ParameterBase.ParameterValuesEqual(obj, this.ValidValues[num].Value))
							{
								num++;
								continue;
							}
							flag = true;
							text = this.ValidValues[num].Label;
							break;
						}
					}
					if (!flag && obj != null)
					{
						text = ParameterInfo.CastValueToLabelString(obj, Thread.CurrentThread.CurrentCulture);
					}
					this.m_labels[i] = text;
				}
			}
		}

		internal void EnsureLabelsAreGenerated()
		{
			if (this.ValidValues != null)
			{
				for (int i = 0; i < this.ValidValues.Count; i++)
				{
					this.ValidValues[i].EnsureLabelIsGenerated();
				}
			}
		}

		internal void AddValidValue(object paramValue, string paramLabel)
		{
			if (paramLabel == null)
			{
				paramLabel = ParameterInfo.CastValueToLabelString(paramValue, Thread.CurrentThread.CurrentCulture);
			}
			this.AddValidValueExplicit(paramValue, paramLabel);
		}

		internal void AddValidValue(string paramValue, string paramLabel, ErrorContext errorContext, CultureInfo language)
		{
			object paramValue2 = default(object);
			if (!ParameterBase.CastFromString(paramValue, out paramValue2, base.DataType, language))
			{
				if (errorContext != null)
				{
					errorContext.Register(ProcessingErrorCode.rsParameterPropertyTypeMismatch, Severity.Error, ObjectType.ReportParameter, base.Name, "ValidValue");
					return;
				}
				throw new ReportParameterTypeMismatchException(base.Name);
			}
			this.AddValidValueExplicit(paramValue2, paramLabel);
		}

		internal void AddValidValueExplicit(object paramValue, string paramLabel)
		{
			if (this.ValidValues == null)
			{
				this.ValidValues = new ValidValueList();
			}
			this.ValidValues.Add(new ValidValue(paramValue, paramLabel));
		}

		internal void Parse(string name, List<string> defaultValues, string type, string nullable, string prompt, bool promptIsExpr, string promptUser, string allowBlank, string multiValue, ValidValueList validValues, string usedInQuery, bool hidden, ErrorContext errorContext, CultureInfo language)
		{
			base.Parse(name, defaultValues, type, nullable, prompt, promptUser, allowBlank, multiValue, usedInQuery, hidden, errorContext, language);
			if (hidden)
			{
				this.m_prompt = "";
			}
			else if (prompt == null)
			{
				this.m_prompt = name + ":";
			}
			else
			{
				this.m_prompt = prompt;
			}
			this.DynamicPrompt = promptIsExpr;
			if (validValues != null)
			{
				int count = validValues.Count;
				for (int i = 0; i < count; i++)
				{
					object obj = default(object);
					if (!ParameterBase.CastFromString(validValues[i].StringValue, out obj, base.DataType, language))
					{
						if (errorContext != null)
						{
							errorContext.Register(ProcessingErrorCode.rsParameterPropertyTypeMismatch, Severity.Error, ObjectType.ReportParameter, name, "ValidValue");
							continue;
						}
						throw new ReportParameterTypeMismatchException(name);
					}
					validValues[i].Value = obj;
					base.ValidateValue(obj, errorContext, base.ParameterObjectType, "ValidValue");
				}
				this.m_validValues = validValues;
			}
		}

		internal void Parse(string name, string type, string nullable, string allowBlank, string multiValue, string usedInQuery, string state, string dynamicPrompt, string prompt, string promptUser, ParameterInfoCollection dependencies, string dynamicValidValues, ValidValueList validValues, string dynamicDefaultValue, List<string> defaultValues, List<string> values, string[] labels, CultureInfo language)
		{
			bool hidden = prompt != null && 0 == prompt.Length;
			bool promptIsExpr = false;
			if (dynamicPrompt != null)
			{
				promptIsExpr = bool.Parse(dynamicPrompt);
			}
			this.Parse(name, defaultValues, type, nullable, prompt, promptIsExpr, promptUser, allowBlank, multiValue, validValues, usedInQuery, hidden, null, language);
			if (state != null)
			{
				this.State = (ReportParameterState)Enum.Parse(typeof(ReportParameterState), state);
			}
			this.DependencyList = dependencies;
			if (dynamicValidValues != null)
			{
				this.DynamicValidValues = bool.Parse(dynamicValidValues);
			}
			if (dynamicDefaultValue != null)
			{
				this.DynamicDefaultValue = bool.Parse(dynamicDefaultValue);
			}
			if (values != null)
			{
				this.Values = new object[values.Count];
				for (int i = 0; i < values.Count; i++)
				{
					if (!ParameterBase.CastFromString(values[i], out this.Values[i], base.DataType, language))
					{
						throw new InternalCatalogException("Can not cast report parameter to correct type when reading from XML");
					}
				}
			}
			this.Labels = labels;
		}

		internal void WriteToXml(XmlTextWriter xml, bool writeTransientState)
		{
			xml.WriteStartElement("Parameter");
			xml.WriteElementString("Name", base.Name);
			xml.WriteElementString("Type", base.DataType.ToString());
			xml.WriteElementString("Nullable", base.Nullable.ToString(CultureInfo.InvariantCulture));
			xml.WriteElementString("AllowBlank", base.AllowBlank.ToString(CultureInfo.InvariantCulture));
			xml.WriteElementString("MultiValue", base.MultiValue.ToString(CultureInfo.InvariantCulture));
			xml.WriteElementString("UsedInQuery", base.UsedInQuery.ToString(CultureInfo.InvariantCulture));
			xml.WriteElementString("State", this.State.ToString());
			if (this.Prompt != null)
			{
				xml.WriteElementString("Prompt", this.Prompt);
			}
			if (this.Prompt != null)
			{
				xml.WriteElementString("DynamicPrompt", this.DynamicPrompt.ToString(CultureInfo.InvariantCulture));
			}
			xml.WriteElementString("PromptUser", base.PromptUser.ToString(CultureInfo.InvariantCulture));
			if (this.DependencyList != null)
			{
				xml.WriteStartElement("Dependencies");
				for (int i = 0; i < this.DependencyList.Count; i++)
				{
					if (this.DependencyList[i] != null)
					{
						xml.WriteElementString("Dependency", this.DependencyList[i].Name);
					}
				}
				xml.WriteEndElement();
			}
			if (this.DynamicValidValues)
			{
				xml.WriteElementString("DynamicValidValues", this.DynamicValidValues.ToString(CultureInfo.InvariantCulture));
			}
			if (this.ValidValues != null)
			{
				xml.WriteStartElement("ValidValues");
				for (int j = 0; j < this.ValidValues.Count; j++)
				{
					xml.WriteStartElement("ValidValue");
					if (this.ValidValues[j] != null)
					{
						if (this.ValidValues[j].Value != null)
						{
							this.WriteValueToXml(xml, base.DataType, this.ValidValues[j].Value);
						}
						if (this.ValidValues[j].LabelRaw != null)
						{
							xml.WriteElementString("Label", this.ValidValues[j].LabelRaw);
						}
					}
					xml.WriteEndElement();
				}
				xml.WriteEndElement();
			}
			if (this.DynamicDefaultValue)
			{
				xml.WriteElementString("DynamicDefaultValue", this.DynamicDefaultValue.ToString(CultureInfo.InvariantCulture));
			}
			if (base.DefaultValues != null)
			{
				xml.WriteStartElement("DefaultValues");
				for (int k = 0; k < base.DefaultValues.Length; k++)
				{
					this.WriteValueToXml(xml, base.DataType, base.DefaultValues[k]);
				}
				xml.WriteEndElement();
			}
			if (this.Values != null)
			{
				xml.WriteStartElement("Values");
				for (int l = 0; l < this.Values.Length; l++)
				{
					this.WriteValueToXml(xml, base.DataType, this.Values[l]);
				}
				xml.WriteEndElement();
			}
			if (writeTransientState)
			{
				xml.WriteElementString("IsUserSupplied", this.IsUserSupplied.ToString(CultureInfo.InvariantCulture));
				xml.WriteElementString("ValuesChanged", this.ValuesChanged.ToString(CultureInfo.InvariantCulture));
				xml.WriteElementString("UseExplicitDefaultValue", this.UseExplicitDefaultValue.ToString(CultureInfo.InvariantCulture));
			}
			xml.WriteEndElement();
		}

		internal static string EncodeObjectAsBase64String(object originalValue, bool convertValueToString)
		{
			if (originalValue == null)
			{
				return null;
			}
			try
			{
				if (convertValueToString)
				{
					if (originalValue is bool)
					{
						originalValue = ((bool)originalValue).ToString(CultureInfo.InvariantCulture);
					}
					else if (originalValue is sbyte)
					{
						originalValue = ((sbyte)originalValue).ToString(CultureInfo.InvariantCulture);
					}
					else if (originalValue is short)
					{
						originalValue = ((short)originalValue).ToString(CultureInfo.InvariantCulture);
					}
					else if (originalValue is int)
					{
						originalValue = ((int)originalValue).ToString(CultureInfo.InvariantCulture);
					}
					else if (originalValue is long)
					{
						originalValue = ((long)originalValue).ToString(CultureInfo.InvariantCulture);
					}
					else if (originalValue is byte)
					{
						originalValue = ((byte)originalValue).ToString(CultureInfo.InvariantCulture);
					}
					else if (originalValue is ushort)
					{
						originalValue = ((ushort)originalValue).ToString(CultureInfo.InvariantCulture);
					}
					else if (originalValue is uint)
					{
						originalValue = ((uint)originalValue).ToString(CultureInfo.InvariantCulture);
					}
					else if (originalValue is ulong)
					{
						originalValue = ((ulong)originalValue).ToString(CultureInfo.InvariantCulture);
					}
					else if (originalValue is float)
					{
						originalValue = ((float)originalValue).ToString("r", CultureInfo.InvariantCulture);
					}
					else if (originalValue is double)
					{
						originalValue = ((double)originalValue).ToString("r", CultureInfo.InvariantCulture);
					}
					else if (originalValue is decimal)
					{
						originalValue = ((decimal)originalValue).ToString("f", CultureInfo.InvariantCulture);
					}
					else if (originalValue is DateTime)
					{
						originalValue = ((DateTime)originalValue).ToString("s", CultureInfo.InvariantCulture);
					}
				}
				using (MemoryStream memoryStream = new MemoryStream())
				{
					new AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.IntermediateFormatWriter(memoryStream, 0).Write(new RIFVariantContainer(originalValue));
					memoryStream.Flush();
					byte[] inArray = memoryStream.ToArray();
					memoryStream.Close();
					return Convert.ToBase64String(inArray);
				}
			}
			catch (Exception innerException)
			{
				throw new InternalCatalogException(innerException, "Parameter value encoding failed for type='" + originalValue.GetType().ToString() + "', value='" + originalValue.ToString().MarkAsPrivate() + "'");
			}
		}

		private void WriteValueToXml(XmlTextWriter xml, DataType parameterType, object val)
		{
			this.WriteValueToXml(xml, parameterType, val, false);
		}

		private void WriteValueToXml(XmlTextWriter xml, DataType parameterType, object val, bool convertValueToString)
		{
			if (parameterType == DataType.Object)
			{
				this.WriteValueToXml(xml, ParameterInfo.EncodeObjectAsBase64String(val, convertValueToString));
			}
			else
			{
				this.WriteValueToXml(xml, val);
			}
		}

		private void WriteValueToXml(XmlTextWriter xml, object val)
		{
			xml.WriteStartElement("Value");
			if (val == null)
			{
				xml.WriteAttributeString("nil", bool.TrueString);
			}
			else
			{
				string text = val as string;
				if (text == null)
				{
					xml.WriteString(this.CastToString(val, CultureInfo.InvariantCulture));
				}
				else
				{
					xml.WriteString(text);
				}
			}
			xml.WriteEndElement();
		}

		internal void WriteNameValueToXml(XmlTextWriter xml, bool convertToString)
		{
			xml.WriteStartElement("Parameter");
			xml.WriteElementString("Name", base.Name);
			if (this.Values != null)
			{
				xml.WriteStartElement("Values");
				for (int i = 0; i < this.Values.Length; i++)
				{
					this.WriteValueToXml(xml, base.DataType, this.Values[i], convertToString);
				}
				xml.WriteEndElement();
			}
			xml.WriteEndElement();
		}

		internal static ParameterInfo Cast(ParameterInfo oldValue, ParameterInfo newType, CultureInfo language)
		{
			bool flag = false;
			return ParameterInfo.Cast(oldValue, newType, language, ref flag);
		}

		internal static ParameterInfo Cast(ParameterInfo oldValue, ParameterInfo newType, CultureInfo language, ref bool metaChanges)
		{
			object[] array = null;
			object[] array2 = null;
			if (oldValue.Values != null)
			{
				array = new object[oldValue.Values.Length];
				for (int i = 0; i < oldValue.Values.Length; i++)
				{
					if (!ParameterBase.Cast(oldValue.Values[i], oldValue.DataType, out array[i], newType.DataType, language))
					{
						return null;
					}
				}
			}
			if (oldValue.DefaultValues != null)
			{
				array2 = new object[oldValue.DefaultValues.Length];
				for (int j = 0; j < oldValue.DefaultValues.Length; j++)
				{
					if (!ParameterBase.Cast(oldValue.DefaultValues[j], oldValue.DataType, out array2[j], newType.DataType, language))
					{
						return null;
					}
				}
			}
			if (oldValue.DataType != newType.DataType)
			{
				metaChanges = true;
			}
			ParameterInfo parameterInfo = new ParameterInfo(newType);
			parameterInfo.Values = array;
			parameterInfo.DefaultValues = array2;
			parameterInfo.StoreLabels();
			return parameterInfo;
		}

		public static string CastToString(object val, DataType type, CultureInfo language)
		{
			object obj = default(object);
			if (!ParameterBase.Cast(val, type, out obj, DataType.String, language))
			{
				throw new InternalCatalogException("Can not cast value of report parameter to string.");
			}
			return ParameterInfo.CastValueToLabelString(val, language);
		}

		public string CastToString(object val, CultureInfo language)
		{
			return ParameterInfo.CastToString(val, base.DataType, language);
		}

		internal static string CastValueToLabelString(object val, CultureInfo language)
		{
			if (val == null)
			{
				return null;
			}
			return Convert.ToString(val, language);
		}
	}
}
