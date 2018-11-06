namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal struct VariantResult
	{
		internal bool ErrorOccurred;

		internal DataFieldStatus FieldStatus;

		internal string ExceptionMessage;

		internal object Value;

		internal VariantResult(bool errorOccurred, object v)
		{
			this.ErrorOccurred = errorOccurred;
			this.Value = v;
			this.FieldStatus = DataFieldStatus.None;
			this.ExceptionMessage = null;
		}
	}
}
