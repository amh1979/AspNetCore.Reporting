using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	public sealed class CalculatedFieldWrapperImpl : CalculatedFieldWrapper, IStorable, IPersistable
	{
		private AspNetCore.ReportingServices.ReportIntermediateFormat.Field m_fieldDef;

		private object m_value;

		private bool m_isValueReady;

		private bool m_isVisited;

		private AspNetCore.ReportingServices.RdlExpressions.ReportRuntime m_reportRT;

		private bool m_errorOccurred;

		private string m_exceptionMessage;

		[NonSerialized]
		private IErrorContext m_iErrorContext;

		[NonSerialized]
		private static Declaration m_declaration = CalculatedFieldWrapperImpl.GetDeclaration();

		public override object Value
		{
			get
			{
				if (!this.m_isValueReady)
				{
					this.CalculateValue();
				}
				return this.m_value;
			}
		}

		internal bool ErrorOccurred
		{
			get
			{
				if (!this.m_isValueReady)
				{
					this.CalculateValue();
				}
				return this.m_errorOccurred;
			}
		}

		internal string ExceptionMessage
		{
			get
			{
				return this.m_exceptionMessage;
			}
		}

		public int Size
		{
			get
			{
				return ItemSizes.ReferenceSize + ItemSizes.SizeOf(this.m_value) + 1 + 1 + ItemSizes.ReferenceSize + ItemSizes.ReferenceSize + 1 + ItemSizes.SizeOf(this.m_exceptionMessage);
			}
		}

		internal CalculatedFieldWrapperImpl()
		{
		}

		internal CalculatedFieldWrapperImpl(AspNetCore.ReportingServices.ReportIntermediateFormat.Field fieldDef, AspNetCore.ReportingServices.RdlExpressions.ReportRuntime reportRT)
		{
			this.m_fieldDef = fieldDef;
			this.m_reportRT = reportRT;
			this.m_iErrorContext = reportRT;
		}

		internal void ResetValue()
		{
			this.m_isValueReady = false;
			this.m_isVisited = false;
			this.m_value = null;
		}

		private void CalculateValue()
		{
			if (this.m_isVisited)
			{
				this.m_iErrorContext.Register(ProcessingErrorCode.rsCyclicExpression, Severity.Warning, ObjectType.Field, this.m_fieldDef.Name, "Value");
				throw new ReportProcessingException_InvalidOperationException();
			}
			this.m_isVisited = true;
			AspNetCore.ReportingServices.RdlExpressions.VariantResult variantResult = this.m_reportRT.EvaluateFieldValueExpression(this.m_fieldDef);
			this.m_value = variantResult.Value;
			this.m_errorOccurred = variantResult.ErrorOccurred;
			if (this.m_errorOccurred)
			{
				this.m_exceptionMessage = variantResult.ExceptionMessage;
			}
			this.m_isVisited = false;
			this.m_isValueReady = true;
		}

		void IPersistable.Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(CalculatedFieldWrapperImpl.m_declaration);
			IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.FieldDef:
				{
					int value2 = scalabilityCache.StoreStaticReference(this.m_fieldDef);
					writer.Write(value2);
					break;
				}
				case MemberName.Value:
					writer.Write(this.m_value);
					break;
				case MemberName.IsValueReady:
					writer.Write(this.m_isValueReady);
					break;
				case MemberName.IsVisited:
					writer.Write(this.m_isVisited);
					break;
				case MemberName.ReportRuntime:
				{
					int value = scalabilityCache.StoreStaticReference(this.m_reportRT);
					writer.Write(value);
					break;
				}
				case MemberName.ErrorOccurred:
					writer.Write(this.m_errorOccurred);
					break;
				case MemberName.ExceptionMessage:
					writer.Write(this.m_exceptionMessage);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		void IPersistable.Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(CalculatedFieldWrapperImpl.m_declaration);
			IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.FieldDef:
				{
					int id2 = reader.ReadInt32();
					this.m_fieldDef = (AspNetCore.ReportingServices.ReportIntermediateFormat.Field)scalabilityCache.FetchStaticReference(id2);
					break;
				}
				case MemberName.Value:
					this.m_value = reader.ReadVariant();
					break;
				case MemberName.IsValueReady:
					this.m_isValueReady = reader.ReadBoolean();
					break;
				case MemberName.IsVisited:
					this.m_isVisited = reader.ReadBoolean();
					break;
				case MemberName.ReportRuntime:
				{
					int id = reader.ReadInt32();
					this.m_reportRT = (AspNetCore.ReportingServices.RdlExpressions.ReportRuntime)scalabilityCache.FetchStaticReference(id);
					this.m_iErrorContext = this.m_reportRT;
					break;
				}
				case MemberName.ErrorOccurred:
					this.m_errorOccurred = reader.ReadBoolean();
					break;
				case MemberName.ExceptionMessage:
					this.m_exceptionMessage = reader.ReadString();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		void IPersistable.ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType IPersistable.GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.CalculatedFieldWrapperImpl;
		}

		public static Declaration GetDeclaration()
		{
			if (CalculatedFieldWrapperImpl.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.FieldDef, Token.Int32));
				list.Add(new MemberInfo(MemberName.Value, Token.Object));
				list.Add(new MemberInfo(MemberName.IsValueReady, Token.Boolean));
				list.Add(new MemberInfo(MemberName.IsVisited, Token.Boolean));
				list.Add(new MemberInfo(MemberName.ReportRuntime, Token.Int32));
				list.Add(new MemberInfo(MemberName.ErrorOccurred, Token.Boolean));
				list.Add(new MemberInfo(MemberName.ExceptionMessage, Token.String));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.CalculatedFieldWrapperImpl, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return CalculatedFieldWrapperImpl.m_declaration;
		}
	}
}
