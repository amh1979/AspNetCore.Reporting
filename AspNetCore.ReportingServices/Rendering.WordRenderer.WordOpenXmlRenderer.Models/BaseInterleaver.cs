using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal abstract class BaseInterleaver : IInterleave, IStorable, IPersistable
	{
		private int _index;

		private long _location;

		[NonSerialized]
		private static Declaration _declaration;

		public int Index
		{
			get
			{
				return this._index;
			}
		}

		public long Location
		{
			get
			{
				return this._location;
			}
		}

		public virtual int Size
		{
			get
			{
				return 12;
			}
		}

		protected BaseInterleaver(int index, long location)
		{
			this._index = index;
			this._location = location;
		}

		protected BaseInterleaver()
		{
		}

		static BaseInterleaver()
		{
			BaseInterleaver._declaration = new Declaration(ObjectType.WordOpenXmlBaseInterleaver, ObjectType.None, new List<MemberInfo>
			{
				new MemberInfo(MemberName.Index, Token.Int32),
				new MemberInfo(MemberName.Location, Token.Int64)
			});
		}

		public abstract void Write(TextWriter output);

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(BaseInterleaver.GetDeclaration());
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Index:
					writer.Write(this._index);
					break;
				case MemberName.Location:
					writer.Write(this._location);
					break;
				default:
					WordOpenXmlUtils.FailSerializable();
					break;
				}
			}
		}

		public virtual void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(BaseInterleaver.GetDeclaration());
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Index:
					this._index = reader.ReadInt32();
					break;
				case MemberName.Location:
					this._location = reader.ReadInt64();
					break;
				default:
					WordOpenXmlUtils.FailSerializable();
					break;
				}
			}
		}

		public virtual void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public virtual ObjectType GetObjectType()
		{
			return ObjectType.WordOpenXmlBaseInterleaver;
		}

		internal static Declaration GetDeclaration()
		{
			return BaseInterleaver._declaration;
		}
	}
}
