using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class SortFilterEventInfo : IPersistable
	{
		[Reference]
		private IInScopeEventSource m_eventSource;

		private List<object>[] m_eventSourceScopeInfo;

		[NonSerialized]
		private static readonly Declaration m_Declaration = SortFilterEventInfo.GetDeclaration();

		internal IInScopeEventSource EventSource
		{
			get
			{
				return this.m_eventSource;
			}
			set
			{
				this.m_eventSource = value;
			}
		}

		internal List<object>[] EventSourceScopeInfo
		{
			get
			{
				return this.m_eventSourceScopeInfo;
			}
			set
			{
				this.m_eventSourceScopeInfo = value;
			}
		}

		internal SortFilterEventInfo()
		{
		}

		internal SortFilterEventInfo(IInScopeEventSource eventSource)
		{
			this.m_eventSource = eventSource;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.EventSource, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IInScopeEventSource, Token.GlobalReference));
			list.Add(new MemberInfo(MemberName.EventSourceScopeInfo, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveArray, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortFilterEventInfo, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(SortFilterEventInfo.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.EventSource:
					writer.WriteGlobalReference(this.m_eventSource);
					break;
				case MemberName.EventSourceScopeInfo:
					writer.WriteArrayOfListsOfPrimitives(this.m_eventSourceScopeInfo);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(SortFilterEventInfo.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.EventSource:
					this.m_eventSource = reader.ReadGlobalReference<IInScopeEventSource>();
					break;
				case MemberName.EventSourceScopeInfo:
					this.m_eventSourceScopeInfo = reader.ReadArrayOfListsOfPrimitives<object>();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortFilterEventInfo;
		}
	}
}
