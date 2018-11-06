using AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models.Relationships;
using AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser;
using System;
using System.IO;
using System.IO.Packaging;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal sealed class PartManager
	{
		private OPCRelationshipTree _relationshipTree;

		private Package _zipPackage;

		public PartManager(Package zipPackage)
		{
			this._zipPackage = zipPackage;
			this._relationshipTree = new OPCRelationshipTree("http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument", this._zipPackage);
		}

		public void Write()
		{
			try
			{
				this._relationshipTree.WriteTree();
			}
			finally
			{
				Package zipPackage = this._zipPackage;
				zipPackage.Flush();
				zipPackage.Close();
			}
		}

		public Relationship AddPartToTree(OoxmlPart part, string contentType, string relationshipType, string locationHint, XmlPart parent)
		{
			return this._relationshipTree.AddPartToTree(part, contentType, relationshipType, locationHint, parent);
		}

		public Relationship AddStreamingPartToTree(string contentType, string relationshipType, string locationHint, XmlPart parent)
		{
			return this._relationshipTree.AddStreamingPartToTree(contentType, relationshipType, locationHint, parent);
		}

		public Relationship AddStreamingRootPartToTree(string contentType, string relationshipType, string locationHint)
		{
			return this._relationshipTree.AddStreamingRootPartToTree(contentType, relationshipType, locationHint);
		}

		public Relationship AddExternalPartToTree(string relationshipType, string externalTarget, XmlPart parent, TargetMode targetMode)
		{
			return this._relationshipTree.AddExternalPartToTree(relationshipType, externalTarget, parent, targetMode);
		}

		public Relationship AddImageToTree(Stream data, ImageHash hash, string extension, string relationshipType, string locationHint, string parentLocation)
		{
			bool flag = default(bool);
			Relationship relationship = this._relationshipTree.AddImageToTree(hash, extension, relationshipType, locationHint, parentLocation, ContentTypeAction.Default, out flag);
			if (flag)
			{
				Package zipPackage = this._zipPackage;
				PackagePart part = zipPackage.GetPart(new Uri(WordOpenXmlUtils.CleanName(relationship.RelatedPart), UriKind.Relative));
				Stream stream = part.GetStream();
				WordOpenXmlUtils.CopyStream(data, stream);
			}
			return relationship;
		}

		public Relationship WriteStaticRootPart(OoxmlPart part, string contentType, string relationshipType, string locationHint)
		{
			Relationship relationship = this._relationshipTree.AddRootPartToTree(part, contentType, relationshipType, locationHint);
			this._relationshipTree.WritePart(relationship.RelatedPart);
			return relationship;
		}

		public Relationship WriteStaticPart(OoxmlPart part, string contentType, string relationshipType, string locationHint, XmlPart parent)
		{
			Relationship relationship = this._relationshipTree.AddPartToTree(part, contentType, relationshipType, locationHint, parent);
			this._relationshipTree.WritePart(relationship.RelatedPart);
			return relationship;
		}

		public RelPart GetPartByContentType(string contenttype)
		{
			return this._relationshipTree.GetPartByContentType(contenttype);
		}

		public XmlPart GetPartByLocation(string location)
		{
			return (XmlPart)this._relationshipTree.GetPartByLocation(location);
		}

		public XmlPart GetRootPart()
		{
			return this._relationshipTree.GetRootPart();
		}

		public static string CleanName(string name)
		{
			name = name.Replace('\\', '/');
			if (!name.StartsWith("/", StringComparison.Ordinal) && !name.StartsWith("#", StringComparison.Ordinal))
			{
				return "/" + name;
			}
			return name;
		}
	}
}
