using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class ShapefileInfo : IPersistable
	{
		private string m_streamName;

		private bool m_errorOccurred;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ShapefileInfo.GetDeclaration();

		internal string StreamName
		{
			get
			{
				return this.m_streamName;
			}
			set
			{
				this.m_streamName = value;
			}
		}

		internal bool ErrorOccurred
		{
			get
			{
				return this.m_errorOccurred;
			}
			set
			{
				this.m_errorOccurred = value;
			}
		}

		internal ShapefileInfo()
		{
		}

		internal ShapefileInfo(string streamName)
		{
			this.m_streamName = streamName;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.StreamName, Token.String));
			list.Add(new MemberInfo(MemberName.ErrorOccurred, Token.Boolean));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ShapefileInfo, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(ShapefileInfo.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.StreamName:
					writer.Write(this.m_streamName);
					break;
				case MemberName.ErrorOccurred:
					writer.Write(this.m_errorOccurred);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(ShapefileInfo.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.StreamName:
					this.m_streamName = reader.ReadString();
					break;
				case MemberName.ErrorOccurred:
					this.m_errorOccurred = reader.ReadBoolean();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ShapefileInfo;
		}
	}
}
