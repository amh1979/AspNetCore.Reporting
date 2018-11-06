using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence
{
	internal class PersistenceConstants
	{
		internal const int NullReferenceID = -2;

		internal const int UndefinedCompatVersion = 0;

		internal static readonly int MajorVersion = 12;

		internal static readonly int MinorVersion = 3;

		internal static readonly int CurrentCompatVersion = ReportProcessingCompatibilityVersion.CurrentVersion;
	}
}
