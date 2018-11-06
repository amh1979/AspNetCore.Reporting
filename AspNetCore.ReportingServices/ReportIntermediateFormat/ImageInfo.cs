using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class ImageInfo : IPersistable
	{
		private string m_streamName;

		private string m_mimeType;

		private bool m_errorOccurred;

		[NonSerialized]
		private WeakReference m_imageDataRef;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ImageInfo.GetDeclaration();

		internal string StreamName
		{
			get
			{
				return this.m_streamName;
			}
			set
			{
				this.m_streamName = value;
			}
		}

		internal string MimeType
		{
			get
			{
				return this.m_mimeType;
			}
			set
			{
				this.m_mimeType = value;
			}
		}

		internal bool ErrorOccurred
		{
			get
			{
				return this.m_errorOccurred;
			}
			set
			{
				this.m_errorOccurred = value;
			}
		}

		internal ImageInfo()
		{
		}

		internal ImageInfo(string streamName, string mimeType)
		{
			this.m_streamName = streamName;
			this.m_mimeType = mimeType;
		}

		internal byte[] GetCachedImageData()
		{
			if (this.m_imageDataRef != null && this.m_imageDataRef.IsAlive)
			{
				return (byte[])this.m_imageDataRef.Target;
			}
			return null;
		}

		internal void SetCachedImageData(byte[] imageData)
		{
			if (this.m_imageDataRef == null)
			{
				this.m_imageDataRef = new WeakReference(imageData);
			}
			else
			{
				this.m_imageDataRef.Target = imageData;
			}
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.StreamName, Token.String));
			list.Add(new MemberInfo(MemberName.MIMEType, Token.String));
			list.Add(new MemberInfo(MemberName.ErrorOccurred, Token.Boolean));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ImageInfo, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(ImageInfo.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.StreamName:
					writer.Write(this.m_streamName);
					break;
				case MemberName.MIMEType:
					writer.Write(this.m_mimeType);
					break;
				case MemberName.ErrorOccurred:
					writer.Write(this.m_errorOccurred);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(ImageInfo.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.StreamName:
					this.m_streamName = reader.ReadString();
					break;
				case MemberName.MIMEType:
					this.m_mimeType = reader.ReadString();
					break;
				case MemberName.ErrorOccurred:
					this.m_errorOccurred = reader.ReadBoolean();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false);
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ImageInfo;
		}
	}
}
