using System;

namespace AspNetCore.ReportingServices.Interfaces
{
	[Serializable]
	internal class AceStruct
	{
		public string PrincipalName;

		public CatalogOperationsCollection CatalogOperations;

		public ReportOperationsCollection ReportOperations;

		public FolderOperationsCollection FolderOperations;

		public ResourceOperationsCollection ResourceOperations;

		public DatasourceOperationsCollection DatasourceOperations;

		public ModelOperationsCollection ModelOperations;

		public ModelItemOperationsCollection ModelItemOperations;

		public AceStruct(string name)
		{
			this.PrincipalName = name;
			this.CatalogOperations = new CatalogOperationsCollection();
			this.ReportOperations = new ReportOperationsCollection();
			this.FolderOperations = new FolderOperationsCollection();
			this.ResourceOperations = new ResourceOperationsCollection();
			this.DatasourceOperations = new DatasourceOperationsCollection();
			this.ModelOperations = new ModelOperationsCollection();
			this.ModelItemOperations = new ModelItemOperationsCollection();
		}

		public AceStruct(AceStruct other)
		{
			this.PrincipalName = other.PrincipalName;
			this.CatalogOperations = other.CatalogOperations;
			this.ReportOperations = other.ReportOperations;
			this.FolderOperations = other.FolderOperations;
			this.ResourceOperations = other.ResourceOperations;
			this.DatasourceOperations = other.DatasourceOperations;
			this.ModelOperations = other.ModelOperations;
			this.ModelItemOperations = other.ModelItemOperations;
		}
	}
}
