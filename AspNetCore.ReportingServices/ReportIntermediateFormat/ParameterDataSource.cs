using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal class ParameterDataSource : IPersistable, IParameterDataSource
	{
		private int m_dataSourceIndex = -1;

		private int m_dataSetIndex = -1;

		private int m_valueFieldIndex = -1;

		private int m_labelFieldIndex = -1;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ParameterDataSource.GetDeclaration();

		public int DataSourceIndex
		{
			get
			{
				return this.m_dataSourceIndex;
			}
			set
			{
				this.m_dataSourceIndex = value;
			}
		}

		public int DataSetIndex
		{
			get
			{
				return this.m_dataSetIndex;
			}
			set
			{
				this.m_dataSetIndex = value;
			}
		}

		public int ValueFieldIndex
		{
			get
			{
				return this.m_valueFieldIndex;
			}
			set
			{
				this.m_valueFieldIndex = value;
			}
		}

		public int LabelFieldIndex
		{
			get
			{
				return this.m_labelFieldIndex;
			}
			set
			{
				this.m_labelFieldIndex = value;
			}
		}

		internal ParameterDataSource()
		{
		}

		internal ParameterDataSource(int dataSourceIndex, int dataSetIndex)
		{
			this.m_dataSourceIndex = dataSourceIndex;
			this.m_dataSetIndex = dataSetIndex;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.DataSourceIndex, Token.Int32));
			list.Add(new MemberInfo(MemberName.DataSetIndex, Token.Int32));
			list.Add(new MemberInfo(MemberName.ValueFieldIndex, Token.Int32));
			list.Add(new MemberInfo(MemberName.LabelFieldIndex, Token.Int32));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterDataSource, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(ParameterDataSource.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.DataSourceIndex:
					writer.Write(this.m_dataSourceIndex);
					break;
				case MemberName.DataSetIndex:
					writer.Write(this.m_dataSetIndex);
					break;
				case MemberName.ValueFieldIndex:
					writer.Write(this.m_valueFieldIndex);
					break;
				case MemberName.LabelFieldIndex:
					writer.Write(this.m_labelFieldIndex);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public virtual void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(ParameterDataSource.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.DataSourceIndex:
					this.m_dataSourceIndex = reader.ReadInt32();
					break;
				case MemberName.DataSetIndex:
					this.m_dataSetIndex = reader.ReadInt32();
					break;
				case MemberName.ValueFieldIndex:
					this.m_valueFieldIndex = reader.ReadInt32();
					break;
				case MemberName.LabelFieldIndex:
					this.m_labelFieldIndex = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public virtual void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false);
		}

		public virtual AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterDataSource;
		}
	}
}
