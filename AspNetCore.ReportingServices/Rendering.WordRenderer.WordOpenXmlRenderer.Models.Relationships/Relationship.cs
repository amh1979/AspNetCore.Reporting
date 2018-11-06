using System.Diagnostics;
using System.IO.Packaging;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models.Relationships
{
	[DebuggerDisplay("{RelatedPart.Location} : {RelationshipId}")]
	internal sealed class Relationship
	{
		private string _relationshipId;

		private string _relatedPart;

		private string _relationshipType;

		private TargetMode _targetMode;

		public string RelationshipId
		{
			get
			{
				return this._relationshipId;
			}
			set
			{
				this._relationshipId = value;
			}
		}

		public string RelatedPart
		{
			get
			{
				return this._relatedPart;
			}
			set
			{
				this._relatedPart = value;
			}
		}

		public string RelationshipType
		{
			get
			{
				return this._relationshipType;
			}
			set
			{
				this._relationshipType = value;
			}
		}

		public TargetMode Mode
		{
			get
			{
				return this._targetMode;
			}
			set
			{
				this._targetMode = value;
			}
		}

		public Relationship()
		{
			this._targetMode = TargetMode.Internal;
		}
	}
}
