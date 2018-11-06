using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal class SubReportInfo : IPersistable
	{
		internal class ParametersImplValuesComparer : IEqualityComparer<ParametersImplWrapper>
		{
			internal static readonly ParametersImplValuesComparer Instance = new ParametersImplValuesComparer();

			private ParametersImplValuesComparer()
			{
			}

			public bool Equals(ParametersImplWrapper obj1, ParametersImplWrapper obj2)
			{
				if (obj1 == obj2)
				{
					return true;
				}
				if (obj1 != null && obj2 != null)
				{
					return obj1.ValuesAreEqual(obj2);
				}
				return false;
			}

			public int GetHashCode(ParametersImplWrapper obj)
			{
				return obj.GetValuesHashCode();
			}
		}

		private int m_lastID;

		private string m_uniqueName;

		[NonSerialized]
		private Dictionary<ParametersImplWrapper, int> m_parameterValuesToInfoIndexMap;

		private List<ParametersImplWrapper> m_instanceParameterValues;

		private int m_userSortDataSetGlobalId = -1;

		[NonSerialized]
		private CommonSubReportInfo m_commonSubReportInfo;

		[NonSerialized]
		private static readonly Declaration m_Declaration = SubReportInfo.GetDeclaration();

		internal CommonSubReportInfo CommonSubReportInfo
		{
			get
			{
				return this.m_commonSubReportInfo;
			}
			set
			{
				this.m_commonSubReportInfo = value;
			}
		}

		internal string UniqueName
		{
			get
			{
				return this.m_uniqueName;
			}
		}

		internal int LastID
		{
			get
			{
				return this.m_lastID;
			}
			set
			{
				this.m_lastID = value;
			}
		}

		internal int UserSortDataSetGlobalId
		{
			get
			{
				return this.m_userSortDataSetGlobalId;
			}
			set
			{
				this.m_userSortDataSetGlobalId = value;
			}
		}

		internal SubReportInfo()
		{
		}

		internal SubReportInfo(Guid uniqueName)
		{
			this.m_uniqueName = uniqueName.ToString("N");
		}

		internal int GetChunkNameModifierForParamValues(ParametersImpl parameterValues, bool addEntry, ref bool? isShared, out ParametersImpl fullParameterValues)
		{
			if (parameterValues == null)
			{
				parameterValues = new ParametersImpl();
			}
			if (this.m_parameterValuesToInfoIndexMap == null)
			{
				this.m_instanceParameterValues = new List<ParametersImplWrapper>();
				this.m_parameterValuesToInfoIndexMap = new Dictionary<ParametersImplWrapper, int>(ParametersImplValuesComparer.Instance);
			}
			ParametersImplWrapper parametersImplWrapper = new ParametersImplWrapper(parameterValues);
			int count = default(int);
			if (this.m_parameterValuesToInfoIndexMap.TryGetValue(parametersImplWrapper, out count))
			{
				fullParameterValues = this.m_instanceParameterValues[count].WrappedParametersImpl;
				if (!isShared.HasValue)
				{
					isShared = true;
				}
			}
			else
			{
				isShared = false;
				fullParameterValues = parameterValues;
				if (addEntry)
				{
					count = this.m_instanceParameterValues.Count;
					this.m_instanceParameterValues.Add(parametersImplWrapper);
					this.m_parameterValuesToInfoIndexMap.Add(parametersImplWrapper, count);
				}
			}
			return count;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.LastID, Token.Int32));
			list.Add(new MemberInfo(MemberName.UniqueName, Token.String));
			list.Add(new MemberInfo(MemberName.InstanceParameterValues, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Parameters));
			list.Add(new MemberInfo(MemberName.DataSetID, Token.Int32));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SubReportInfo, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(SubReportInfo.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.LastID:
					writer.Write(this.m_lastID);
					break;
				case MemberName.UniqueName:
					writer.Write(this.m_uniqueName);
					break;
				case MemberName.InstanceParameterValues:
					writer.Write(this.m_instanceParameterValues);
					break;
				case MemberName.DataSetID:
					writer.Write(this.m_userSortDataSetGlobalId);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(SubReportInfo.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.LastID:
					this.m_lastID = reader.ReadInt32();
					break;
				case MemberName.UniqueName:
					this.m_uniqueName = reader.ReadString();
					break;
				case MemberName.InstanceParameterValues:
					this.m_instanceParameterValues = reader.ReadListOfRIFObjects<List<ParametersImplWrapper>>();
					break;
				case MemberName.DataSetID:
					this.m_userSortDataSetGlobalId = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
			if (this.m_instanceParameterValues != null)
			{
				this.m_parameterValuesToInfoIndexMap = new Dictionary<ParametersImplWrapper, int>(ParametersImplValuesComparer.Instance);
				for (int i = 0; i < this.m_instanceParameterValues.Count; i++)
				{
					this.m_parameterValuesToInfoIndexMap[this.m_instanceParameterValues[i]] = i;
				}
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false);
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SubReportInfo;
		}
	}
}
