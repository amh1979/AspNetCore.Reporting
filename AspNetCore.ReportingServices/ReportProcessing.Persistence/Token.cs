namespace AspNetCore.ReportingServices.ReportProcessing.Persistence
{
	internal enum Token : byte
	{
		Null,
		Object,
		EndObject,
		Reference,
		Enum,
		TypedArray,
		Array,
		Declaration,
		DataFieldInfo,
		Guid = 239,
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
