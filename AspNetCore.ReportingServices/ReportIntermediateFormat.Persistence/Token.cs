namespace AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence
{
	internal enum Token : byte
	{
		Null,
		Object,
		Reference,
		Enum,
		GlobalReference,
		Hashtable = 232,
		Serializable,
		SqlGeometry,
		SqlGeography,
		DateTimeWithKind,
		DateTimeOffset,
		ByteArray,
		Guid,
		String,
		DateTime,
		TimeSpan,
		Char,
		Boolean,
		Int16,
		Int32,
		Int64,
		UInt16,
		UInt32,
		UInt64,
		Byte,
		SByte,
		Single,
		Double,
		Decimal
	}
}
