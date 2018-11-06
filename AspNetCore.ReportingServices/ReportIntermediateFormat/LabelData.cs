using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class LabelData : IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = LabelData.GetDeclaration();

		private string m_dataSetName;

		private List<string> m_keyFields;

		private string m_label;

		internal string DataSetName
		{
			get
			{
				return this.m_dataSetName;
			}
			set
			{
				this.m_dataSetName = value;
			}
		}

		internal List<string> KeyFields
		{
			get
			{
				return this.m_keyFields;
			}
			set
			{
				this.m_keyFields = value;
			}
		}

		internal string Label
		{
			get
			{
				return this.m_label;
			}
			set
			{
				this.m_label = value;
			}
		}

		internal void Initialize(Tablix tablix, InitializationContext context)
		{
			context.ValidateSliderLabelData(tablix, this);
		}

		[SkipMemberStaticValidation(MemberName.Key)]
		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.DataSetName, Token.String));
			list.Add(new MemberInfo(MemberName.Key, Token.String, Lifetime.RemovedIn(200)));
			list.Add(new MemberInfo(MemberName.Label, Token.String));
			list.Add(new MemberInfo(MemberName.KeyFields, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.String, Lifetime.AddedIn(200)));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LabelData, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(LabelData.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.DataSetName:
					writer.Write(this.m_dataSetName);
					break;
				case MemberName.Key:
					writer.Write(this.m_keyFields[0]);
					break;
				case MemberName.Label:
					writer.Write(this.m_label);
					break;
				case MemberName.KeyFields:
					writer.WriteListOfPrimitives(this.m_keyFields);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(LabelData.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.DataSetName:
					this.m_dataSetName = reader.ReadString();
					break;
				case MemberName.Key:
				{
					string item = reader.ReadString();
					this.m_keyFields = new List<string>(1);
					this.m_keyFields.Add(item);
					break;
				}
				case MemberName.Label:
					this.m_label = reader.ReadString();
					break;
				case MemberName.KeyFields:
					this.m_keyFields = reader.ReadListOfPrimitives<string>();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LabelData;
		}
	}
}
