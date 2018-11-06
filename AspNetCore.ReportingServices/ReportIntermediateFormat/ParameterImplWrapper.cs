using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[SkipStaticValidation]
	internal class ParameterImplWrapper : IPersistable
	{
		private ParameterImpl m_odpParameter;

		private static readonly Declaration m_Declaration = ParameterImplWrapper.GetDeclaration();

		internal ParameterImpl WrappedParameterImpl
		{
			get
			{
				return this.m_odpParameter;
			}
			set
			{
				this.m_odpParameter = value;
			}
		}

		internal ParameterImplWrapper()
		{
			this.m_odpParameter = new ParameterImpl();
		}

		internal ParameterImplWrapper(ParameterImpl odpParameter)
		{
			this.m_odpParameter = odpParameter;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Value, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveArray, Token.Object));
			list.Add(new MemberInfo(MemberName.Label, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveArray, Token.String));
			list.Add(new MemberInfo(MemberName.IsMultiValue, Token.Boolean));
			list.Add(new MemberInfo(MemberName.Prompt, Token.String));
			list.Add(new MemberInfo(MemberName.IsUserSupplied, Token.Boolean));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Parameter, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(ParameterImplWrapper.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Value:
					writer.Write(this.m_odpParameter.GetValues());
					break;
				case MemberName.Label:
					writer.Write(this.m_odpParameter.GetLabels());
					break;
				case MemberName.IsMultiValue:
					writer.Write(this.m_odpParameter.IsMultiValue);
					break;
				case MemberName.Prompt:
					writer.Write(this.m_odpParameter.Prompt);
					break;
				case MemberName.IsUserSupplied:
					writer.Write(this.m_odpParameter.IsUserSupplied);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(ParameterImplWrapper.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Value:
					this.m_odpParameter.SetValues(reader.ReadVariantArray());
					break;
				case MemberName.Label:
					this.m_odpParameter.SetLabels(reader.ReadStringArray());
					break;
				case MemberName.IsMultiValue:
					this.m_odpParameter.SetIsMultiValue(reader.ReadBoolean());
					break;
				case MemberName.Prompt:
					this.m_odpParameter.SetPrompt(reader.ReadString());
					break;
				case MemberName.IsUserSupplied:
					this.m_odpParameter.SetIsUserSupplied(reader.ReadBoolean());
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Parameter;
		}
	}
}
