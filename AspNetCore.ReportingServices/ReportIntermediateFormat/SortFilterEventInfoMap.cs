using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class SortFilterEventInfoMap : IPersistable
	{
		private Dictionary<string, SortFilterEventInfo> m_sortFilterEventInfos;

		[NonSerialized]
		private static readonly Declaration m_Declaration = SortFilterEventInfoMap.GetDeclaration();

		internal SortFilterEventInfo this[string eventSourceUniqueName]
		{
			get
			{
				SortFilterEventInfo result = default(SortFilterEventInfo);
				this.m_sortFilterEventInfos.TryGetValue(eventSourceUniqueName, out result);
				return result;
			}
		}

		internal int Count
		{
			get
			{
				if (this.m_sortFilterEventInfos == null)
				{
					return 0;
				}
				return this.m_sortFilterEventInfos.Count;
			}
		}

		internal SortFilterEventInfoMap()
		{
		}

		internal void Add(string eventSourceUniqueName, SortFilterEventInfo eventInfo)
		{
			if (this.m_sortFilterEventInfos == null)
			{
				this.m_sortFilterEventInfos = new Dictionary<string, SortFilterEventInfo>();
			}
			this.m_sortFilterEventInfos.Add(eventSourceUniqueName, eventInfo);
		}

		internal void Merge(SortFilterEventInfoMap partition)
		{
			if (partition.Count != 0)
			{
				foreach (KeyValuePair<string, SortFilterEventInfo> sortFilterEventInfo in partition.m_sortFilterEventInfos)
				{
					this.Add(sortFilterEventInfo.Key, sortFilterEventInfo.Value);
				}
			}
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.SortFilterEventInfos, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StringRIFObjectDictionary, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortFilterEventInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortFilterEventInfoMap, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(SortFilterEventInfoMap.m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.SortFilterEventInfos)
				{
					writer.WriteStringRIFObjectDictionary(this.m_sortFilterEventInfos);
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(SortFilterEventInfoMap.m_Declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.SortFilterEventInfos)
				{
					this.m_sortFilterEventInfos = reader.ReadStringRIFObjectDictionary<SortFilterEventInfo>();
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false);
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortFilterEventInfoMap;
		}
	}
}
