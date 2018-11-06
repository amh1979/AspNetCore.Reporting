using System;
using System.CodeDom.Compiler;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal abstract class ExpressionParser
	{
		[Flags]
		internal enum DetectionFlags
		{
			ParameterReference = 1,
			UserReference = 2
		}

		internal enum ExpressionType
		{
			General,
			ReportParameter,
			ReportLanguage,
			QueryParameter,
			GroupExpression,
			SortExpression,
			DataSetFilters,
			DataRegionFilters,
			GroupingFilters,
			FieldValue
		}

		internal enum ConstantType
		{
			String,
			Boolean,
			Integer
		}

		internal enum RecursiveFlags
		{
			Simple,
			Recursive
		}

		internal struct ExpressionContext
		{
			private ExpressionType m_expressionType;

			private ConstantType m_constantType;

			private LocationFlags m_location;

			private ObjectType m_objectType;

			private string m_objectName;

			private string m_propertyName;

			private string m_dataSetName;

			private bool m_parseExtended;

			internal ExpressionType ExpressionType
			{
				get
				{
					return this.m_expressionType;
				}
			}

			internal ConstantType ConstantType
			{
				get
				{
					return this.m_constantType;
				}
			}

			internal LocationFlags Location
			{
				get
				{
					return this.m_location;
				}
			}

			internal ObjectType ObjectType
			{
				get
				{
					return this.m_objectType;
				}
			}

			internal string ObjectName
			{
				get
				{
					return this.m_objectName;
				}
			}

			internal string PropertyName
			{
				get
				{
					return this.m_propertyName;
				}
			}

			internal string DataSetName
			{
				get
				{
					return this.m_dataSetName;
				}
			}

			internal bool ParseExtended
			{
				get
				{
					return this.m_parseExtended;
				}
			}

			internal ExpressionContext(ExpressionType expressionType, ConstantType constantType, LocationFlags location, ObjectType objectType, string objectName, string propertyName, string dataSetName, bool parseExtended)
			{
				this.m_expressionType = expressionType;
				this.m_constantType = constantType;
				this.m_location = location;
				this.m_objectType = objectType;
				this.m_objectName = objectName;
				this.m_propertyName = propertyName;
				this.m_dataSetName = dataSetName;
				this.m_parseExtended = parseExtended;
			}
		}

		[Flags]
		protected enum GrammarFlags
		{
			DenyAggregates = 1,
			DenyRunningValue = 2,
			DenyRowNumber = 4,
			DenyFields = 8,
			DenyReportItems = 0x10,
			DenyPageGlobals = 0x20,
			DenyPostSortAggregate = 0x40,
			DenyPrevious = 0x80,
			DenyDataSets = 0x100,
			DenyDataSources = 0x200
		}

		[Flags]
		protected enum Restrictions
		{
			None = 0,
			InPageSection = 0x38E,
			InBody = 0x20,
			AggregateParameterInPageSection = 0x87,
			AggregateParameterInBody = 0x97,
			ReportParameter = 0x39F,
			ReportLanguage = 0x39F,
			QueryParameter = 0x39F,
			GroupExpression = 0x93,
			SortExpression = 0xD6,
			DataSetFilters = 0x97,
			DataRegionFilters = 0x97,
			GroupingFilters = 0xD6,
			FieldValue = 0xB7
		}

		protected ErrorContext m_errorContext;

		private bool m_valueReferenced;

		private bool m_valueReferencedGlobal;

		internal abstract bool BodyRefersToReportItems
		{
			get;
		}

		internal abstract bool PageSectionRefersToReportItems
		{
			get;
		}

		internal abstract int NumberOfAggregates
		{
			get;
		}

		internal abstract int LastID
		{
			get;
		}

		internal bool ValueReferenced
		{
			get
			{
				return this.m_valueReferenced;
			}
		}

		internal bool ValueReferencedGlobal
		{
			get
			{
				return this.m_valueReferencedGlobal;
			}
		}

		internal ExpressionParser(ErrorContext errorContext)
		{
			this.m_errorContext = errorContext;
		}

		internal abstract CodeDomProvider GetCodeCompiler();

		internal abstract string GetCompilerArguments();

		internal abstract ExpressionInfo ParseExpression(string expression, ExpressionContext context);

		internal abstract ExpressionInfo ParseExpression(string expression, ExpressionContext context, DetectionFlags flag, out bool reportParameterReferenced, out string reportParameterName, out bool userCollectionReferenced);

		internal abstract ExpressionInfo ParseExpression(string expression, ExpressionContext context, out bool userCollectionReferenced);

		internal abstract void ConvertField2ComplexExpr(ref ExpressionInfo expression);

		internal void ResetValueReferencedFlag()
		{
			this.m_valueReferenced = false;
		}

		protected static Restrictions ExpressionType2Restrictions(ExpressionType expressionType)
		{
			switch (expressionType)
			{
			case ExpressionType.General:
				return Restrictions.None;
			case ExpressionType.ReportParameter:
				return Restrictions.ReportParameter;
			case ExpressionType.ReportLanguage:
				return Restrictions.ReportParameter;
			case ExpressionType.QueryParameter:
				return Restrictions.ReportParameter;
			case ExpressionType.GroupExpression:
				return Restrictions.GroupExpression;
			case ExpressionType.SortExpression:
				return Restrictions.SortExpression;
			case ExpressionType.DataSetFilters:
				return Restrictions.AggregateParameterInBody;
			case ExpressionType.DataRegionFilters:
				return Restrictions.AggregateParameterInBody;
			case ExpressionType.GroupingFilters:
				return Restrictions.SortExpression;
			case ExpressionType.FieldValue:
				return Restrictions.FieldValue;
			default:
				Global.Tracer.Assert(false);
				return Restrictions.None;
			}
		}

		protected void SetValueReferenced()
		{
			this.m_valueReferenced = true;
			this.m_valueReferencedGlobal = true;
		}
	}
}
