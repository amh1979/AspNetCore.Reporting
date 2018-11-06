using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal class DataAggregateInfoBucket : AggregateBucket<DataAggregateInfo>
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = DataAggregateInfoBucket.GetDeclaration();

		internal DataAggregateInfoBucket()
		{
		}

		internal DataAggregateInfoBucket(int level)
			: base(level)
		{
		}

		protected override Declaration GetSpecificDeclaration()
		{
			return DataAggregateInfoBucket.m_Declaration;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Aggregates, ObjectType.RIFObjectList, ObjectType.DataAggregateInfo));
			list.Add(new MemberInfo(MemberName.Level, Token.Int32));
			return new Declaration(ObjectType.DataAggregateInfoBucket, ObjectType.None, list);
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.DataAggregateInfoBucket;
		}
	}
}
