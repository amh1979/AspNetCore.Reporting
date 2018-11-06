using System.Collections;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence
{
	internal delegate T CreateDictionary<T>(int dictionaryLength) where T : IDictionary;
}
