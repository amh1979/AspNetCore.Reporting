using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class BandLayoutOptions : IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = BandLayoutOptions.GetDeclaration();

		private int m_rowCount = 1;

		private int m_columnCount = 1;

		private Navigation m_navigation;

		internal int RowCount
		{
			get
			{
				return this.m_rowCount;
			}
			set
			{
				this.m_rowCount = value;
			}
		}

		internal int ColumnCount
		{
			get
			{
				return this.m_columnCount;
			}
			set
			{
				this.m_columnCount = value;
			}
		}

		internal Navigation Navigation
		{
			get
			{
				return this.m_navigation;
			}
			set
			{
				this.m_navigation = value;
			}
		}

		internal void Initialize(Tablix tablix, InitializationContext context)
		{
			if (this.m_navigation != null)
			{
				this.m_navigation.Initialize(tablix, context);
			}
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.RowCount, Token.Int32));
			list.Add(new MemberInfo(MemberName.ColumnCount, Token.Int32));
			list.Add(new MemberInfo(MemberName.Navigation, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Navigation));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BandLayoutOptions, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(BandLayoutOptions.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.RowCount:
					writer.Write(this.m_rowCount);
					break;
				case MemberName.ColumnCount:
					writer.Write(this.m_columnCount);
					break;
				case MemberName.Navigation:
					writer.Write(this.m_navigation);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(BandLayoutOptions.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.RowCount:
					this.m_rowCount = reader.ReadInt32();
					break;
				case MemberName.ColumnCount:
					this.m_columnCount = reader.ReadInt32();
					break;
				case MemberName.Navigation:
					this.m_navigation = (Navigation)reader.ReadRIFObject();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BandLayoutOptions;
		}
	}
}
