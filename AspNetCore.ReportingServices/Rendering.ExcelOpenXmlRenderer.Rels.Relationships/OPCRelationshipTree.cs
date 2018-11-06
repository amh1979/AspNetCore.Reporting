using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Rels.Archive;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Packaging;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Rels.Relationships
{
	internal class OPCRelationshipTree
	{
		private Dictionary<string, RelPart> _parts;

		private Dictionary<string, List<Relationship>> _relationships;

		private Dictionary<string, string> _blobPathsByUniqueId;

		private string _documentRootRelationshipType;

		private string _docRootLocation;

		private Package _package;

		private int maxRelationshipId = 1;

		private int maxPartId;

		public OPCRelationshipTree(string documentRootRelationshipType, Package package)
		{
			this._package = package;
			this._documentRootRelationshipType = documentRootRelationshipType;
			this._parts = new Dictionary<string, RelPart>();
			this._relationships = new Dictionary<string, List<Relationship>>();
			this._blobPathsByUniqueId = new Dictionary<string, string>();
		}

		public void WriteTree()
		{
			foreach (string key in this._parts.Keys)
			{
				if (!(this._parts[key] is PhantomPart))
				{
					PackagePart part = this._package.GetPart(new Uri(Utils.CleanName(key), UriKind.Relative));
					StreamWriter streamWriter = new StreamWriter(part.GetStream());
					((XmlPart)this._parts[key]).HydratedPart.Write(streamWriter);
					streamWriter.Flush();
				}
			}
		}

		public RelPart GetPartByContentType(string contenttype)
		{
			foreach (RelPart value in this._parts.Values)
			{
				if (value.ContentType == contenttype)
				{
					return value;
				}
			}
			return null;
		}

		public RelPart GetPartByLocation(string location)
		{
			RelPart result = default(RelPart);
			if (!this._parts.TryGetValue(location, out result))
			{
				return null;
			}
			return result;
		}

		public List<Relationship> GetRelationshipsByPath(string partLocation)
		{
			List<Relationship> result = default(List<Relationship>);
			if (this._relationships.TryGetValue(partLocation, out result))
			{
				return result;
			}
			return new List<Relationship>();
		}

		private string UniqueLocation(string locationHint)
		{
			this.maxPartId++;
			return string.Format(CultureInfo.InvariantCulture, locationHint, this.maxPartId);
		}

		private string NextRelationshipId()
		{
			this.maxRelationshipId++;
			return string.Format(CultureInfo.InvariantCulture, "rId{0}", this.maxRelationshipId);
		}

		public Relationship AddRootPartToTree(OoxmlPart part, string contentType, string relationshipType, string locationHint)
		{
			return this.AddRootPartToTree(part, contentType, relationshipType, locationHint, ContentTypeAction.Override);
		}

		public Relationship AddRootPartToTree(OoxmlPart part, string contentType, string relationshipType, string locationHint, ContentTypeAction ctypeAction)
		{
			Relationship relationship = this.AddPart(part, contentType, relationshipType, locationHint, "/", ctypeAction);
			if (relationshipType == this._documentRootRelationshipType)
			{
				this._docRootLocation = relationship.RelatedPart;
			}
			return relationship;
		}

		public Relationship AddPartToTree(OoxmlPart part, string contentType, string relationshipType, string locationHint, XmlPart parent)
		{
			return this.AddPartToTree(part, contentType, relationshipType, locationHint, parent, ContentTypeAction.Override);
		}

		public Relationship AddPartToTree(OoxmlPart part, string contentType, string relationshipType, string locationHint, XmlPart parent, ContentTypeAction ctypeAction)
		{
			return this.AddPart(part, contentType, relationshipType, locationHint, parent.Location, ctypeAction);
		}

		private Relationship AddPart(OoxmlPart part, string contentType, string relationshipType, string locationHint, string parentLocation, ContentTypeAction ctypeAction)
		{
			XmlPart xmlPart = new XmlPart();
			xmlPart.ContentType = contentType;
			xmlPart.HydratedPart = part;
			if (locationHint.Contains("{0}"))
			{
				xmlPart.Location = this.UniqueLocation(locationHint);
			}
			else
			{
				xmlPart.Location = locationHint;
			}
			this._parts.Add(xmlPart.Location, xmlPart);
			this._package.CreatePart(new Uri(Utils.CleanName(xmlPart.Location), UriKind.Relative), xmlPart.ContentType, CompressionOption.Normal);
			return this.AddRelationship(xmlPart.Location, relationshipType, parentLocation);
		}

		public Relationship AddImageToTree(string uniqueId, string extension, string relationshipType, string locationHint, string parentLocation, ContentTypeAction ctypeAction, out bool newBlob)
		{
			PhantomPart phantomPart = new PhantomPart();
			phantomPart.ContentType = "image/" + extension;
			string text = default(string);
			if (this._blobPathsByUniqueId.TryGetValue(uniqueId, out text))
			{
				phantomPart.Location = text;
				newBlob = false;
			}
			else
			{
				CompressionOption compressionOption = (CompressionOption)((extension == "jpg" || extension == "png" || extension == "gif") ? (-1) : 0);
				text = this.UniqueLocation(locationHint);
				this._blobPathsByUniqueId[uniqueId] = text;
				phantomPart.Location = text;
				this._parts.Add(phantomPart.Location, phantomPart);
				this._package.CreatePart(new Uri(Utils.CleanName(phantomPart.Location), UriKind.Relative), phantomPart.ContentType, compressionOption);
				newBlob = true;
			}
			return this.AddRelationship(phantomPart.Location, relationshipType, parentLocation);
		}

		public Relationship AddStreamingPartToTree(string contentType, string relationshipType, string locationHint, XmlPart parent)
		{
			return this.AddStreamingPartToTree(contentType, relationshipType, locationHint, parent, ContentTypeAction.Override);
		}

		public Relationship AddStreamingPartToTree(string contentType, string relationshipType, string locationHint, XmlPart parent, ContentTypeAction ctypeAction)
		{
			string location = this.UniqueLocation(locationHint);
			PhantomPart phantomPart = new PhantomPart();
			phantomPart.ContentType = contentType;
			phantomPart.Location = location;
			this._parts.Add(phantomPart.Location, phantomPart);
			this._package.CreatePart(new Uri(Utils.CleanName(phantomPart.Location), UriKind.Relative), phantomPart.ContentType, CompressionOption.Normal);
			return this.AddRelationship(location, relationshipType, parent.Location);
		}

		public Relationship AddExternalPartToTree(string relationshipType, string externalLocation, XmlPart parent, TargetMode targetMode)
		{
			return this.AddRelationship(externalLocation, relationshipType, parent.Location, targetMode);
		}

		private Relationship AddRelationship(string location, string relationshipType, string parentLocation)
		{
			return this.AddRelationship(location, relationshipType, parentLocation, TargetMode.Internal);
		}

		private Relationship AddRelationship(string location, string relationshipType, string parentLocation, TargetMode mode)
		{
			if (!this._relationships.ContainsKey(parentLocation) || this._relationships[parentLocation] == null)
			{
				this._relationships[parentLocation] = new List<Relationship>();
			}
			else
			{
				foreach (Relationship item in this._relationships[parentLocation])
				{
					if (item.RelatedPart == location)
					{
						return item;
					}
				}
			}
			Relationship relationship = new Relationship();
			relationship.RelatedPart = location;
			relationship.RelationshipId = this.NextRelationshipId();
			relationship.RelationshipType = relationshipType;
			relationship.Mode = mode;
			this._relationships[parentLocation].Add(relationship);
			Uri targetUri = (relationship.Mode != 0) ? new Uri(relationship.RelatedPart, UriKind.Absolute) : new Uri(Utils.CleanName(relationship.RelatedPart), UriKind.Relative);
			if (parentLocation == "/")
			{
				this._package.CreateRelationship(targetUri, relationship.Mode, relationship.RelationshipType, relationship.RelationshipId);
			}
			else
			{
				PackagePart part = this._package.GetPart(new Uri(Utils.CleanName(parentLocation), UriKind.Relative));
				part.CreateRelationship(targetUri, relationship.Mode, relationship.RelationshipType, relationship.RelationshipId);
			}
			return relationship;
		}
	}
}
