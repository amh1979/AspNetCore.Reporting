using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
//
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal abstract class DataAggregate : IStorable, IPersistable
	{
		internal abstract DataAggregateInfo.AggregateTypes AggregateType
		{
			get;
		}

		public abstract int Size
		{
			get;
		}

		internal abstract void Init();

		internal abstract void Update(object[] expressions, IErrorContext iErrorContext);

		internal abstract object Result();

		internal abstract DataAggregate ConstructAggregator(OnDemandProcessingContext odpContext, DataAggregateInfo aggregateDef);

		internal static AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode GetTypeCode(object o)
		{
			bool flag = default(bool);
			return DataAggregate.GetTypeCode(o, true, out flag);
		}

		internal static AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode GetTypeCode(object o, bool throwException, out bool valid)
		{
            valid = true;
            if (o is string)
            {
                return ReportProcessing.DataAggregate.DataTypeCode.String;
            }
            if (o is int)
            {
                return ReportProcessing.DataAggregate.DataTypeCode.Int32;
            }
            if (o is double)
            {
                return ReportProcessing.DataAggregate.DataTypeCode.Double;
            }
            if (o == null || DBNull.Value == o)
            {
                return ReportProcessing.DataAggregate.DataTypeCode.Null;
            }
            if (o is ushort)
            {
                return ReportProcessing.DataAggregate.DataTypeCode.UInt16;
            }
            if (o is short)
            {
                return ReportProcessing.DataAggregate.DataTypeCode.Int16;
            }
            if (o is long)
            {
                return ReportProcessing.DataAggregate.DataTypeCode.Int64;
            }
            if (o is decimal)
            {
                return ReportProcessing.DataAggregate.DataTypeCode.Decimal;
            }
            if (o is uint)
            {
                return ReportProcessing.DataAggregate.DataTypeCode.UInt32;
            }
            if (o is ulong)
            {
                return ReportProcessing.DataAggregate.DataTypeCode.UInt64;
            }
            if (o is byte)
            {
                return ReportProcessing.DataAggregate.DataTypeCode.Byte;
            }
            if (o is sbyte)
            {
                return ReportProcessing.DataAggregate.DataTypeCode.SByte;
            }
            if (o is DateTime)
            {
                return ReportProcessing.DataAggregate.DataTypeCode.DateTime;
            }
            if (o is char)
            {
                return ReportProcessing.DataAggregate.DataTypeCode.Char;
            }
            if (o is bool)
            {
                return ReportProcessing.DataAggregate.DataTypeCode.Boolean;
            }
            if (o is TimeSpan)
            {
                return ReportProcessing.DataAggregate.DataTypeCode.TimeSpan;
            }
            if (o is DateTimeOffset)
            {
                return ReportProcessing.DataAggregate.DataTypeCode.DateTimeOffset;
            }
            if (o is float)
            {
                return ReportProcessing.DataAggregate.DataTypeCode.Single;
            }
            if (o is byte[])
            {
                return ReportProcessing.DataAggregate.DataTypeCode.ByteArray;
            }
            //if (o is SqlGeography)
            //{
            //    return DataAggregate.DataTypeCode.SqlGeography;
            //}
            //if (o is SqlGeometry)
            //{
            //    return DataAggregate.DataTypeCode.SqlGeometry;
            //}
            valid = false;
            if (throwException)
            {
                throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
            }
            return ReportProcessing.DataAggregate.DataTypeCode.Null;
        }

		protected static bool IsNull(AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode typeCode)
		{
			return AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Null == typeCode;
		}

		protected static bool IsVariant(AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode typeCode)
		{
			return AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.ByteArray != typeCode;
		}

		protected static void ConvertToDoubleOrDecimal(AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode numericType, object numericData, out AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode doubleOrDecimalType, out object doubleOrDecimalData)
		{
			if (AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Decimal == numericType)
			{
				doubleOrDecimalType = AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Decimal;
				doubleOrDecimalData = numericData;
			}
			else
			{
				doubleOrDecimalType = AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Double;
				doubleOrDecimalData = DataTypeUtility.ConvertToDouble(numericType, numericData);
			}
		}

		protected static object Add(AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode xType, object x, AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode yType, object y)
		{
			if (AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Double == xType && AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Double == yType)
			{
				return (double)x + (double)y;
			}
			if (AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Decimal == xType && AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Decimal == yType)
			{
				return (decimal)x + (decimal)y;
			}
			Global.Tracer.Assert(false);
			throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
		}

		protected static object Square(AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode xType, object x)
		{
			if (AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Double == xType)
			{
				return (double)x * (double)x;
			}
			if (AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Decimal == xType)
			{
				return (decimal)x * (decimal)x;
			}
			Global.Tracer.Assert(false);
			throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
		}

		public abstract void Serialize(IntermediateFormatWriter writer);

		public abstract void Deserialize(IntermediateFormatReader reader);

		public abstract void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems);

		public abstract AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType();
	}
}
