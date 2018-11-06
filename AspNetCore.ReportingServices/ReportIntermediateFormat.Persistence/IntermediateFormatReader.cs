using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence
{
	internal struct IntermediateFormatReader
	{
		private int m_currentMemberIndex;

		private Declaration m_currentPersistedDeclaration;

		private Dictionary<ObjectType, Declaration> m_readDecls;

		private PersistenceBinaryReader m_reader;

		private Dictionary<IPersistable, Dictionary<ObjectType, List<MemberReference>>> m_memberReferencesCollection;

		private Dictionary<int, IReferenceable> m_referenceableItems;

		private GlobalIDOwnerCollection m_globalIDOwners;

		private IRIFObjectCreator m_rifObjectCreator;

		private PersistenceHelper m_persistenceHelper;

		private IntermediateFormatVersion m_version;

		private long m_objectStartPosition;

		private PersistenceFlags m_persistenceFlags;

		private int m_currentMemberInfoCount;

		private MemberInfo m_currentMember;

		private int m_compatVersion;

		private BinaryFormatter m_binaryFormatter;

		internal bool CanSeek
		{
			get
			{
				return IntermediateFormatReader.HasPersistenceFlag(this.m_persistenceFlags, PersistenceFlags.Seekable);
			}
		}

		internal bool EOS
		{
			get
			{
				return this.m_reader.EOS;
			}
		}

		internal IntermediateFormatVersion IntermediateFormatVersion
		{
			get
			{
				return this.m_version;
			}
		}

		internal MemberInfo CurrentMember
		{
			get
			{
				return this.m_currentMember;
			}
		}

		internal PersistenceHelper PersistenceHelper
		{
			get
			{
				return this.m_persistenceHelper;
			}
		}

		internal long ObjectStartPosition
		{
			get
			{
				return this.m_objectStartPosition;
			}
		}

		internal bool HasReferences
		{
			get
			{
				if (this.m_memberReferencesCollection != null)
				{
					return this.m_memberReferencesCollection.Count > 0;
				}
				return false;
			}
		}

		internal GlobalIDOwnerCollection GlobalIDOwners
		{
			get
			{
				return this.m_globalIDOwners;
			}
		}

		internal IntermediateFormatReader(Stream str, IRIFObjectCreator rifObjectCreator)
		{
			this = new IntermediateFormatReader(str, rifObjectCreator, null, null);
		}

		internal IntermediateFormatReader(Stream str, IRIFObjectCreator rifObjectCreator, PersistenceHelper persistenceHelper)
		{
			this = new IntermediateFormatReader(str, rifObjectCreator, null, persistenceHelper);
		}

		internal IntermediateFormatReader(Stream str, IRIFObjectCreator rifObjectCreator, GlobalIDOwnerCollection globalIDOwnersFromOtherStream)
		{
			this = new IntermediateFormatReader(str, rifObjectCreator, globalIDOwnersFromOtherStream, null);
		}

		internal IntermediateFormatReader(Stream str, IRIFObjectCreator rifObjectCreator, GlobalIDOwnerCollection globalIDOwnersFromOtherStream, PersistenceHelper persistenceHelper)
		{
			this = new IntermediateFormatReader(str, rifObjectCreator, globalIDOwnersFromOtherStream, persistenceHelper, null, null, PersistenceFlags.None, true, PersistenceConstants.CurrentCompatVersion);
		}

		internal IntermediateFormatReader(Stream str, IRIFObjectCreator rifObjectCreator, GlobalIDOwnerCollection globalIDOwnersFromOtherStream, PersistenceHelper persistenceHelper, int compatVersion)
		{
			this = new IntermediateFormatReader(str, rifObjectCreator, globalIDOwnersFromOtherStream, persistenceHelper, null, null, PersistenceFlags.None, true, compatVersion);
		}

		internal IntermediateFormatReader(Stream str, IRIFObjectCreator rifObjectCreator, GlobalIDOwnerCollection globalIDOwnersFromOtherStream, PersistenceHelper persistenceHelper, List<Declaration> declarations, IntermediateFormatVersion version, PersistenceFlags flags)
		{
			this = new IntermediateFormatReader(str, rifObjectCreator, globalIDOwnersFromOtherStream, persistenceHelper, declarations, version, flags, false, PersistenceConstants.CurrentCompatVersion);
		}

		private IntermediateFormatReader(Stream str, IRIFObjectCreator rifObjectCreator, GlobalIDOwnerCollection globalIDOwnersFromOtherStream, PersistenceHelper persistenceHelper, List<Declaration> declarations, IntermediateFormatVersion version, PersistenceFlags flags, bool initFromStream, int compatVersion)
		{
			this.m_currentMemberIndex = -1;
			this.m_readDecls = new Dictionary<ObjectType, Declaration>(EqualityComparers.ObjectTypeComparerInstance);
			this.m_currentPersistedDeclaration = null;
			this.m_reader = new PersistenceBinaryReader(str);
			this.m_referenceableItems = new Dictionary<int, IReferenceable>(EqualityComparers.Int32ComparerInstance);
			this.m_memberReferencesCollection = new Dictionary<IPersistable, Dictionary<ObjectType, List<MemberReference>>>();
			this.m_rifObjectCreator = rifObjectCreator;
			this.m_persistenceHelper = persistenceHelper;
			this.m_version = null;
			this.m_persistenceFlags = PersistenceFlags.None;
			this.m_objectStartPosition = 0L;
			this.m_globalIDOwners = null;
			this.m_currentMemberInfoCount = 0;
			this.m_currentMember = null;
			this.m_binaryFormatter = null;
			this.m_compatVersion = compatVersion;
			if (globalIDOwnersFromOtherStream == null)
			{
				this.m_globalIDOwners = new GlobalIDOwnerCollection();
			}
			else
			{
				this.m_globalIDOwners = globalIDOwnersFromOtherStream;
			}
			if (initFromStream)
			{
				this.m_version = this.ReadIntermediateFormatVersion();
				this.m_persistenceFlags = (PersistenceFlags)this.m_reader.ReadEnum();
				if (IntermediateFormatReader.HasPersistenceFlag(this.m_persistenceFlags, PersistenceFlags.CompatVersioned))
				{
					int documentCompatVersion = this.m_reader.ReadInt32();
					IncompatibleRIFVersionException.ThrowIfIncompatible(documentCompatVersion, this.m_compatVersion);
				}
				if (IntermediateFormatReader.HasPersistenceFlag(this.m_persistenceFlags, PersistenceFlags.Seekable))
				{
					this.ReadDeclarations();
				}
			}
			else
			{
				this.m_version = version;
				this.m_persistenceFlags = flags;
				this.PrepareDeclarationsFromInitialization(declarations);
			}
			this.m_objectStartPosition = this.m_reader.StreamPosition;
		}

		private static bool HasPersistenceFlag(PersistenceFlags flags, PersistenceFlags flagToTest)
		{
			return (flags & flagToTest) != PersistenceFlags.None;
		}

		private void PrepareDeclarationsFromInitialization(List<Declaration> declarations)
		{
			for (int i = 0; i < declarations.Count; i++)
			{
				Declaration declaration = declarations[i].CreateFilteredDeclarationForWriteVersion(this.m_compatVersion);
				this.m_readDecls.Add(declaration.ObjectType, declaration);
			}
		}

		private void ReadDeclarations()
		{
			int num = default(int);
			if (this.m_reader.ReadListStart(ObjectType.Declaration, out num))
			{
				for (int i = 0; i < num; i++)
				{
					Declaration declaration = this.m_reader.ReadDeclaration();
					this.m_readDecls.Add(declaration.ObjectType, declaration);
				}
			}
		}

		internal void RegisterDeclaration(Declaration declaration)
		{
			this.m_currentMemberIndex = -1;
			if (!this.m_readDecls.TryGetValue(declaration.ObjectType, out this.m_currentPersistedDeclaration))
			{
				this.m_currentPersistedDeclaration = this.m_reader.ReadDeclaration();
				this.m_currentPersistedDeclaration.RegisterCurrentDeclaration(declaration);
				this.m_readDecls.Add(declaration.ObjectType, this.m_currentPersistedDeclaration);
			}
			else if (!this.m_currentPersistedDeclaration.RegisteredCurrentDeclaration)
			{
				this.m_currentPersistedDeclaration.RegisterCurrentDeclaration(declaration);
			}
			this.m_currentMemberInfoCount = this.m_currentPersistedDeclaration.MemberInfoList.Count;
		}

		internal bool NextMember()
		{
			this.m_currentMemberIndex++;
			if (this.m_currentMemberIndex < this.m_currentMemberInfoCount)
			{
				if (this.m_currentPersistedDeclaration.HasSkippedMembers && this.m_currentPersistedDeclaration.IsMemberSkipped(this.m_currentMemberIndex))
				{
					this.SkipMembers(this.m_currentPersistedDeclaration.MembersToSkip(this.m_currentMemberIndex));
					return this.NextMember();
				}
				this.m_currentMember = this.m_currentPersistedDeclaration.MemberInfoList[this.m_currentMemberIndex];
				return true;
			}
			return false;
		}

		internal void ResolveReferences()
		{
			foreach (KeyValuePair<IPersistable, Dictionary<ObjectType, List<MemberReference>>> item in this.m_memberReferencesCollection)
			{
				item.Key.ResolveReferences(item.Value, this.m_referenceableItems);
				item.Value.Clear();
			}
		}

		internal void ClearReferences()
		{
			this.m_referenceableItems = new Dictionary<int, IReferenceable>(EqualityComparers.Int32ComparerInstance);
			this.m_memberReferencesCollection = new Dictionary<IPersistable, Dictionary<ObjectType, List<MemberReference>>>();
		}

		private void SkipMembers(int toSkip)
		{
			for (int i = 0; i < toSkip; i++)
			{
				this.m_currentMember = this.m_currentPersistedDeclaration.MemberInfoList[this.m_currentMemberIndex];
				switch (this.m_currentMember.ObjectType)
				{
				case ObjectType.PrimitiveTypedArray:
					switch (this.m_currentMember.Token)
					{
					case Token.Byte:
					case Token.SByte:
						this.m_reader.SkipTypedArray(1);
						break;
					case Token.Char:
					case Token.Int16:
					case Token.UInt16:
						this.m_reader.SkipTypedArray(2);
						break;
					case Token.Int32:
					case Token.UInt32:
					case Token.Single:
						this.m_reader.SkipTypedArray(4);
						break;
					case Token.DateTime:
					case Token.TimeSpan:
					case Token.Int64:
					case Token.UInt64:
					case Token.Double:
						this.m_reader.SkipTypedArray(8);
						break;
					case Token.DateTimeOffset:
					case Token.Guid:
						this.m_reader.SkipBytes(16);
						break;
					case Token.Decimal:
						this.m_reader.ReadDecimal();
						break;
					default:
						Global.Tracer.Assert(false);
						break;
					}
					break;
				case ObjectType.RIFObjectArray:
					if (this.m_currentMember.Token == Token.Reference)
					{
						this.SkipListOrArrayOfReferences();
					}
					else
					{
						this.SkipListOrArrayOfRIFObjects();
					}
					break;
				case ObjectType.RIFObjectList:
					if (this.m_currentMember.Token == Token.Reference)
					{
						this.SkipListOrArrayOfReferences();
					}
					else
					{
						this.SkipListOrArrayOfRIFObjects();
					}
					break;
				case ObjectType.PrimitiveArray:
					this.SkipArrayOfPrimitives();
					break;
				case ObjectType.PrimitiveList:
					this.SkipListOfPrimitives();
					break;
				case ObjectType.StringRIFObjectDictionary:
					this.SkipStringRIFObjectDictionary();
					break;
				case ObjectType.Int32PrimitiveListHashtable:
					this.SkipInt32PrimitiveListHashtable();
					break;
				case ObjectType.ObjectHashtableHashtable:
					this.SkipObjectHashtableHashtable();
					break;
				case ObjectType.StringObjectHashtable:
					this.SkipStringObjectHashtable();
					break;
				case ObjectType.Int32RIFObjectDictionary:
					this.SkipInt32RIFObjectDictionary();
					break;
				case ObjectType.None:
					this.SkipPrimitive(this.m_currentMember.Token);
					break;
				default:
					if (this.m_currentMember.Token == Token.Reference)
					{
						this.m_reader.SkipMultiByteInt();
					}
					else
					{
						this.SkipRIFObject();
					}
					break;
				case ObjectType.Null:
					break;
				}
				this.m_currentMemberIndex++;
			}
			this.m_currentMemberIndex--;
		}

		private void SkipPrimitive(Token token)
		{
			switch (token)
			{
			case Token.Null:
				break;
			case Token.String:
				this.m_reader.SkipString();
				break;
			case Token.Boolean:
			case Token.Byte:
			case Token.SByte:
				this.m_reader.SkipBytes(1);
				break;
			case Token.Char:
			case Token.Int16:
			case Token.UInt16:
				this.m_reader.SkipBytes(2);
				break;
			case Token.Int32:
			case Token.UInt32:
			case Token.Single:
				this.m_reader.SkipBytes(4);
				break;
			case Token.Reference:
			case Token.Enum:
				this.m_reader.SkipMultiByteInt();
				break;
			case Token.DateTime:
			case Token.TimeSpan:
			case Token.Int64:
			case Token.UInt64:
			case Token.Double:
				this.m_reader.SkipBytes(8);
				break;
			case Token.DateTimeOffset:
			case Token.Guid:
			case Token.Decimal:
				this.m_reader.SkipBytes(16);
				break;
			case Token.Object:
				this.SkipPrimitive(this.m_reader.ReadToken());
				break;
			default:
				Global.Tracer.Assert(false);
				break;
			}
		}

		private void SkipArrayOfPrimitives()
		{
			int num = default(int);
			if (this.m_reader.ReadArrayStart(this.m_currentMember.ObjectType, out num))
			{
				for (int i = 0; i < num; i++)
				{
					this.SkipPrimitive(this.m_reader.ReadToken());
				}
			}
		}

		private void SkipListOfPrimitives()
		{
			int num = default(int);
			if (this.m_reader.ReadListStart(this.m_currentMember.ObjectType, out num))
			{
				for (int i = 0; i < num; i++)
				{
					this.SkipPrimitive(this.m_reader.ReadToken());
				}
			}
		}

		private void SkipListOrArrayOfReferences()
		{
			int num = default(int);
			if (this.m_reader.ReadListStart(this.m_currentMember.ObjectType, out num))
			{
				for (int i = 0; i < num; i++)
				{
					if (this.m_reader.ReadObjectType() != 0)
					{
						this.m_reader.SkipMultiByteInt();
					}
				}
			}
		}

		private void SkipListOrArrayOfRIFObjects()
		{
			int num = default(int);
			if (this.m_reader.ReadListStart(this.m_currentMember.ObjectType, out num))
			{
				for (int i = 0; i < num; i++)
				{
					this.SkipRIFObject();
				}
			}
		}

        private void SkipRIFObject()
        {
            ObjectType objectType = this.m_reader.ReadObjectType();
            if (objectType != ObjectType.Null)
            {
                ((IntermediateFormatReader)base.MemberwiseClone()).__SkipRIFObjectPrivate(objectType);
            }
        }

        private void __SkipRIFObjectPrivate(ObjectType objectType)
		{
			this.m_currentMemberIndex = 0;
			if (!this.m_readDecls.TryGetValue(objectType, out this.m_currentPersistedDeclaration))
			{
				this.m_currentPersistedDeclaration = this.m_reader.ReadDeclaration();
				this.m_readDecls.Add(objectType, this.m_currentPersistedDeclaration);
			}
			this.m_currentMemberInfoCount = this.m_currentPersistedDeclaration.MemberInfoList.Count;
			this.SkipMembers(this.m_currentMemberInfoCount);
		}

		private void SkipStringRIFObjectDictionary()
		{
			int num = default(int);
			if (this.m_reader.ReadDictionaryStart(this.m_currentMember.ObjectType, out num))
			{
				for (int i = 0; i < num; i++)
				{
					this.m_reader.SkipString();
					this.SkipRIFObject();
				}
			}
		}

		private void SkipInt32RIFObjectDictionary()
		{
			int num = default(int);
			if (this.m_reader.ReadDictionaryStart(this.m_currentMember.ObjectType, out num))
			{
				for (int i = 0; i < num; i++)
				{
					this.m_reader.SkipBytes(4);
					this.SkipRIFObject();
				}
			}
		}

		internal void SkipInt32PrimitiveListHashtable()
		{
			int num = default(int);
			if (this.m_reader.ReadDictionaryStart(this.m_currentMember.ObjectType, out num))
			{
				for (int i = 0; i < num; i++)
				{
					this.m_reader.SkipBytes(4);
					this.SkipListOfPrimitives();
				}
			}
		}

		internal void SkipStringObjectHashtable()
		{
			int num = default(int);
			if (this.m_reader.ReadDictionaryStart(this.m_currentMember.ObjectType, out num))
			{
				for (int i = 0; i < num; i++)
				{
					this.m_reader.SkipString();
					this.SkipPrimitive(this.m_reader.ReadToken());
				}
			}
		}

		internal void SkipObjectHashtableHashtable()
		{
			int num = default(int);
			if (this.m_reader.ReadDictionaryStart(this.m_currentMember.ObjectType, out num))
			{
				for (int i = 0; i < num; i++)
				{
					this.SkipPrimitive(this.m_reader.ReadToken());
					this.SkipObjectHashtableHashtable();
				}
			}
		}

		internal void Seek(long newPosition)
		{
			this.Seek(newPosition, SeekOrigin.Begin);
		}

		internal void Seek(long newPosition, SeekOrigin seekOrigin)
		{
			this.m_reader.Seek(newPosition, seekOrigin);
		}

		internal IPersistable ReadRIFObject()
		{
			return this.ReadRIFObject(true);
		}

		internal IPersistable ReadRIFObject(IPersistable persitObj)
		{
			ObjectType objectType = this.ReadRIFObjectStart();
			if (objectType != 0)
			{
				persitObj.Deserialize(this);
				this.ReadRIFObjectFinish(objectType, persitObj, false);
			}
			else
			{
				persitObj = null;
			}
			return persitObj;
		}

		internal T ReadRIFObject<T>() where T : IPersistable, new()
		{
			ObjectType objectType = this.ReadRIFObjectStart();
			T val = default(T);
			if (objectType != 0)
			{
				val = new T();
				val.Deserialize(this);
				this.ReadRIFObjectFinish(objectType, (IPersistable)(object)val, false);
			}
			return val;
		}

		private IPersistable ReadRIFObject(bool verify)
		{
			ObjectType objectType = this.ReadRIFObjectStart();
			IPersistable persistable = null;
			if (objectType != 0)
			{
				persistable = this.m_rifObjectCreator.CreateRIFObject(objectType, ref this);
				this.AddReferenceableItem(persistable);
			}
			return persistable;
		}

		private void AddReferenceableItem(IPersistable persistObj)
		{
			IReferenceable referenceable = persistObj as IReferenceable;
			if (referenceable != null && referenceable.ID != -2)
			{
				this.m_referenceableItems.Add(referenceable.ID, referenceable);
			}
			IGlobalIDOwner globalIDOwner = persistObj as IGlobalIDOwner;
			if (globalIDOwner != null)
			{
				int num = globalIDOwner.GlobalID = this.m_globalIDOwners.GetGlobalID();
				IGloballyReferenceable globallyReferenceable = persistObj as IGloballyReferenceable;
				if (globallyReferenceable != null)
				{
					this.m_globalIDOwners.Add(globallyReferenceable);
				}
			}
		}

		private ObjectType ReadRIFObjectStart()
		{
			this.m_objectStartPosition = this.m_reader.StreamPosition;
			return this.m_reader.ReadObjectType();
		}

		private void ReadRIFObjectFinish(ObjectType persistedType, IPersistable persitObj, bool verify)
		{
			if (this.m_currentPersistedDeclaration == null)
			{
				;
			}
			if (persitObj is IReferenceable)
			{
				IReferenceable referenceable = (IReferenceable)persitObj;
				this.m_referenceableItems.Add(referenceable.ID, referenceable);
			}
		}

		internal Dictionary<string, TValue> ReadStringRIFObjectDictionary<TValue>() where TValue : IPersistable
		{
			int num = default(int);
			if (this.m_reader.ReadDictionaryStart(this.m_currentMember.ObjectType, out num))
			{
				Dictionary<string, TValue> dictionary = new Dictionary<string, TValue>(num, EqualityComparers.StringComparerInstance);
				for (int i = 0; i < num; i++)
				{
					dictionary.Add(this.ReadString(false), (TValue)this.ReadRIFObject());
				}
				return dictionary;
			}
			return null;
		}

		internal Dictionary<int, TValue> ReadInt32RIFObjectDictionary<TValue>() where TValue : IPersistable
		{
			int num = default(int);
			if (this.m_reader.ReadDictionaryStart(this.m_currentMember.ObjectType, out num))
			{
				Dictionary<int, TValue> dictionary = new Dictionary<int, TValue>(num, (IEqualityComparer<int>)EqualityComparers.Int32ComparerInstance);
				for (int i = 0; i < num; i++)
				{
					dictionary.Add(this.ReadInt32(false), (TValue)this.ReadRIFObject());
				}
				return dictionary;
			}
			return null;
		}

		internal IDictionary ReadInt32RIFObjectDictionary<T>(CreateDictionary<T> dictionaryCreator) where T : IDictionary
		{
			int num = default(int);
			if (this.m_reader.ReadDictionaryStart(this.m_currentMember.ObjectType, out num))
			{
				IDictionary dictionary = (IDictionary)(object)dictionaryCreator(num);
				for (int i = 0; i < num; i++)
				{
					dictionary.Add(this.ReadInt32(false), this.ReadRIFObject());
				}
				return dictionary;
			}
			return null;
		}

		internal T ReadInt32PrimitiveListHashtable<T, U>() where T : Hashtable, new()where U : struct
		{
			int num = default(int);
			if (this.m_reader.ReadDictionaryStart(this.m_currentMember.ObjectType, out num))
			{
				T result = new T();
				for (int i = 0; i < num; i++)
				{
					result.Add(this.ReadInt32(false), this.ReadListOfPrimitives<U>());
				}
				return result;
			}
			return null;
		}

		internal T ReadStringInt32Hashtable<T>() where T : IDictionary, new()
		{
			int num = default(int);
			if (this.m_reader.ReadDictionaryStart(this.m_currentMember.ObjectType, out num))
			{
				T result = new T();
				for (int i = 0; i < num; i++)
				{
					result.Add(this.ReadString(false), this.ReadInt32());
				}
				return result;
			}
			return default(T);
		}

		internal T ReadByteVariantHashtable<T>() where T : IDictionary, new()
		{
			int num = default(int);
			if (this.m_reader.ReadDictionaryStart(this.m_currentMember.ObjectType, out num))
			{
				T result = new T();
				for (int i = 0; i < num; i++)
				{
					result.Add(this.ReadByte(false), this.ReadVariant());
				}
				return result;
			}
			return default(T);
		}

		internal T ReadStringStringHashtable<T>() where T : IDictionary, new()
		{
			int num = default(int);
			if (this.m_reader.ReadDictionaryStart(this.m_currentMember.ObjectType, out num))
			{
				T result = new T();
				for (int i = 0; i < num; i++)
				{
					result.Add(this.ReadString(false), this.ReadString(false));
				}
				return result;
			}
			return default(T);
		}

		internal T ReadStringObjectHashtable<T>() where T : IDictionary, new()
		{
			int num = default(int);
			if (this.m_reader.ReadDictionaryStart(this.m_currentMember.ObjectType, out num))
			{
				T result = new T();
				for (int i = 0; i < num; i++)
				{
					result.Add(this.ReadString(false), this.ReadVariant());
				}
				return result;
			}
			return default(T);
		}

		internal T ReadStringRIFObjectHashtable<T>() where T : IDictionary, new()
		{
			int num = default(int);
			if (this.m_reader.ReadDictionaryStart(this.m_currentMember.ObjectType, out num))
			{
				T result = new T();
				for (int i = 0; i < num; i++)
				{
					result.Add(this.ReadString(false), this.ReadRIFObject());
				}
				return result;
			}
			return default(T);
		}

		internal Dictionary<string, List<string>> ReadStringListOfStringDictionary()
		{
			int num = default(int);
			if (this.m_reader.ReadDictionaryStart(this.m_currentMember.ObjectType, out num))
			{
				Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
				for (int i = 0; i < num; i++)
				{
					dictionary.Add(this.ReadString(false), this.ReadListOfPrimitives<string>());
				}
				return dictionary;
			}
			return null;
		}

		internal T ReadStringObjectHashtable<T>(CreateDictionary<T> createDictionary, Predicate<string> allowKey, Converter<string, string> processName, Converter<object, object> processValue) where T : IDictionary
		{
			int num = default(int);
			if (this.m_reader.ReadDictionaryStart(this.m_currentMember.ObjectType, out num))
			{
				T result = createDictionary(num);
				for (int i = 0; i < num; i++)
				{
					string text = this.ReadString(false);
					object input = this.ReadVariant();
					if (allowKey(text))
					{
						result.Add(processName(text), processValue(input));
					}
				}
				return result;
			}
			return default(T);
		}

		internal Hashtable ReadObjectHashtableHashtable()
		{
			int num = default(int);
			if (this.m_reader.ReadDictionaryStart(this.m_currentMember.ObjectType, out num))
			{
				Hashtable hashtable = new Hashtable();
				for (int i = 0; i < num; i++)
				{
					hashtable.Add(this.ReadVariant(), this.ReadVariantVariantHashtable());
				}
				return hashtable;
			}
			return null;
		}

		internal Hashtable ReadNLevelVariantHashtable()
		{
			int num = default(int);
			if (this.m_reader.ReadDictionaryStart(this.m_currentMember.ObjectType, out num))
			{
				Hashtable hashtable = new Hashtable();
				for (int i = 0; i < num; i++)
				{
					object key = this.ReadVariant();
					Token token = this.m_reader.ReadToken();
					object value;
					switch (token)
					{
					case Token.Hashtable:
						value = this.ReadNLevelVariantHashtable();
						break;
					case Token.Object:
						value = this.ReadVariant();
						break;
					default:
						Global.Tracer.Assert(false, "Invalid token: {0} while reading NLevelVariantHashtable", token);
						value = null;
						break;
					}
					hashtable.Add(key, value);
				}
				return hashtable;
			}
			return null;
		}

		internal T ReadNameObjectCollection<T>() where T : class, INameObjectCollection, new()
		{
			int num = default(int);
			if (this.m_reader.ReadDictionaryStart(this.m_currentMember.ObjectType, out num))
			{
				T result = new T();
				for (int i = 0; i < num; i++)
				{
					result.Add(this.ReadString(false), this.ReadVariant());
				}
				return result;
			}
			return null;
		}

		internal T? ReadNullable<T>() where T : struct
		{
			return (T?)this.ReadVariant();
		}

		internal Dictionary<T, string> ReadRIFObjectStringHashtable<T>() where T : IPersistable
		{
			return this.ReadRIFObjectStringHashtable((Dictionary<T, string>)null);
		}

		internal Dictionary<T, string> ReadRIFObjectStringHashtable<T>(Dictionary<T, string> dictionary) where T : IPersistable
		{
			int num = default(int);
			if (this.m_reader.ReadDictionaryStart(this.m_currentMember.ObjectType, out num))
			{
				if (dictionary == null)
				{
					dictionary = new Dictionary<T, string>(num);
				}
				for (int i = 0; i < num; i++)
				{
					dictionary.Add((T)this.ReadRIFObject(), this.ReadString(false));
				}
			}
			return dictionary;
		}

		internal Hashtable ReadVariantVariantHashtable()
		{
			int num = default(int);
			if (this.m_reader.ReadDictionaryStart(this.m_currentMember.ObjectType, out num))
			{
				Hashtable hashtable = new Hashtable();
				for (int i = 0; i < num; i++)
				{
					hashtable.Add(this.ReadVariant(), this.ReadVariant());
				}
				return hashtable;
			}
			return null;
		}

		internal Dictionary<List<object>, object> ReadVariantListVariantDictionary()
		{
			int num = default(int);
			if (this.m_reader.ReadDictionaryStart(this.m_currentMember.ObjectType, out num))
			{
				Dictionary<List<object>, object> dictionary = new Dictionary<List<object>, object>();
				for (int i = 0; i < num; i++)
				{
					dictionary.Add(this.ReadListOfVariant<List<object>>(), this.ReadVariant());
				}
				return dictionary;
			}
			return null;
		}

		internal Dictionary<string, List<object>> ReadStringVariantListDictionary()
		{
			int num = default(int);
			if (this.m_reader.ReadDictionaryStart(this.m_currentMember.ObjectType, out num))
			{
				Dictionary<string, List<object>> dictionary = new Dictionary<string, List<object>>();
				for (int i = 0; i < num; i++)
				{
					dictionary.Add(this.ReadString(), this.ReadListOfVariant<List<object>>());
				}
				return dictionary;
			}
			return null;
		}

		internal Dictionary<string, bool[]> ReadStringBoolArrayDictionary()
		{
			int num = default(int);
			if (this.m_reader.ReadDictionaryStart(this.m_currentMember.ObjectType, out num))
			{
				Dictionary<string, bool[]> dictionary = new Dictionary<string, bool[]>();
				for (int i = 0; i < num; i++)
				{
					dictionary.Add(this.ReadString(false), this.m_reader.ReadBooleanArray());
				}
				return dictionary;
			}
			return null;
		}

		internal Hashtable ReadInt32StringHashtable()
		{
			int num = default(int);
			if (this.m_reader.ReadDictionaryStart(this.m_currentMember.ObjectType, out num))
			{
				Hashtable hashtable = new Hashtable();
				for (int i = 0; i < num; i++)
				{
					hashtable.Add(this.ReadInt32(false), this.ReadString(false));
				}
				return hashtable;
			}
			return null;
		}

		internal T ReadVariantRIFObjectDictionary<T>(CreateDictionary<T> creator) where T : IDictionary, new()
		{
			int num = default(int);
			if (this.m_reader.ReadDictionaryStart(this.m_currentMember.ObjectType, out num))
			{
				T result = creator(num);
				for (int i = 0; i < num; i++)
				{
					result.Add(this.ReadVariant(), this.ReadRIFObject());
				}
				return result;
			}
			return default(T);
		}

		internal T ReadVariantListOfRIFObjectDictionary<T, V>(CreateDictionary<T> creator) where T : IDictionary, new()where V : class, IList, new()
		{
			int num = default(int);
			if (this.m_reader.ReadDictionaryStart(this.m_currentMember.ObjectType, out num))
			{
				T result = creator(num);
				for (int i = 0; i < num; i++)
				{
					result.Add(this.ReadVariant(), this.ReadListOfRIFObjects<V>());
				}
				return result;
			}
			return default(T);
		}

		internal Dictionary<int, object> Int32SerializableDictionary()
		{
			int num = default(int);
			if (this.m_reader.ReadDictionaryStart(this.m_currentMember.ObjectType, out num))
			{
				Dictionary<int, object> dictionary = new Dictionary<int, object>();
				for (int i = 0; i < num; i++)
				{
					dictionary.Add(this.ReadInt32(false), this.ReadSerializable());
				}
				return dictionary;
			}
			return null;
		}

		internal T ReadListOfRIFObjects<T>() where T : class, IList, new()
		{
			int num = default(int);
			if (this.m_reader.ReadListStart(this.m_currentMember.ObjectType, out num))
			{
				T result = new T();
				for (int i = 0; i < num; i++)
				{
					result.Add(this.ReadRIFObject());
				}
				return result;
			}
			return null;
		}

		internal void ReadListOfRIFObjects(IList list)
		{
			int num = default(int);
			if (this.m_reader.ReadListStart(this.m_currentMember.ObjectType, out num))
			{
				for (int i = 0; i < num; i++)
				{
					list.Add(this.ReadRIFObject());
				}
			}
		}

		internal void ReadListOfRIFObjects<T>(Action<T> addRIFObject) where T : IPersistable
		{
			int num = default(int);
			if (this.m_reader.ReadListStart(this.m_currentMember.ObjectType, out num))
			{
				for (int i = 0; i < num; i++)
				{
					addRIFObject((T)this.ReadRIFObject());
				}
			}
		}

		internal List<T> ReadGenericListOfRIFObjects<T>() where T : IPersistable
		{
			int num = default(int);
			if (this.m_reader.ReadListStart(this.m_currentMember.ObjectType, out num))
			{
				List<T> list = new List<T>(num);
				for (int i = 0; i < num; i++)
				{
					list.Add((T)this.ReadRIFObject());
				}
				return list;
			}
			return null;
		}

		internal List<T> ReadGenericListOfRIFObjectsUsingNew<T>() where T : IPersistable, new()
		{
			int num = default(int);
			if (this.m_reader.ReadListStart(this.m_currentMember.ObjectType, out num))
			{
				List<T> list = new List<T>(num);
				for (int i = 0; i < num; i++)
				{
					list.Add(this.ReadRIFObject<T>());
				}
				return list;
			}
			return null;
		}

		internal List<T> ReadGenericListOfRIFObjects<T>(Action<T> action) where T : IPersistable
		{
			int num = default(int);
			if (this.m_reader.ReadListStart(this.m_currentMember.ObjectType, out num))
			{
				List<T> list = new List<T>(num);
				for (int i = 0; i < num; i++)
				{
					T val = (T)this.ReadRIFObject();
					action(val);
					list.Add(val);
				}
				return list;
			}
			return null;
		}

		internal List<List<T>> ReadListOfListsOfRIFObjects<T>() where T : IPersistable
		{
			int num = default(int);
			if (this.m_reader.ReadListStart(this.m_currentMember.ObjectType, out num))
			{
				List<List<T>> list = new List<List<T>>(num);
				for (int i = 0; i < num; i++)
				{
					List<T> item = this.ReadGenericListOfRIFObjects<T>();
					list.Add(item);
				}
				return list;
			}
			return null;
		}

		internal List<T[]> ReadListOfRIFObjectArrays<T>() where T : IPersistable
		{
			int num = default(int);
			if (this.m_reader.ReadListStart(this.m_currentMember.ObjectType, out num))
			{
				List<T[]> list = new List<T[]>(num);
				for (int i = 0; i < num; i++)
				{
					T[] item = this.ReadArrayOfRIFObjects<T>();
					list.Add(item);
				}
				return list;
			}
			return null;
		}

		internal List<T> ReadListOfPrimitives<T>()
		{
			int num = default(int);
			if (this.m_reader.ReadListStart(ObjectType.PrimitiveList, out num))
			{
				List<T> list = new List<T>(num);
				for (int i = 0; i < num; i++)
				{
					list.Add((T)this.ReadVariant());
				}
				return list;
			}
			return null;
		}

		internal List<List<T>[]> ReadListOfArrayOfListsOfPrimitives<T>()
		{
			int num = default(int);
			if (this.m_reader.ReadListStart(ObjectType.PrimitiveList, out num))
			{
				List<List<T>[]> list = new List<List<T>[]>(num);
				for (int i = 0; i < num; i++)
				{
					list.Add(this.ReadArrayOfListsOfPrimitives<T>());
				}
				return list;
			}
			return null;
		}

		internal T ReadListOfVariant<T>() where T : class, IList, new()
		{
			int num = default(int);
			if (this.m_reader.ReadListStart(this.m_currentMember.ObjectType, out num))
			{
				T result = new T();
				for (int i = 0; i < num; i++)
				{
					result.Add(this.ReadVariant());
				}
				return result;
			}
			return null;
		}

		internal List<T>[] ReadArrayOfListsOfPrimitives<T>()
		{
			int num = default(int);
			if (this.m_reader.ReadArrayStart(ObjectType.PrimitiveArray, out num))
			{
				List<T>[] array = new List<T>[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = this.ReadListOfPrimitives<T>();
				}
				return array;
			}
			return null;
		}

		internal List<T>[] ReadArrayOfRIFObjectLists<T>() where T : IPersistable
		{
			int num = default(int);
			if (this.m_reader.ReadArrayStart(ObjectType.RIFObjectArray, out num))
			{
				List<T>[] array = new List<T>[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = this.ReadGenericListOfRIFObjects<T>();
				}
				return array;
			}
			return null;
		}

		internal T[] ReadArrayOfRIFObjects<T>() where T : IPersistable
		{
			return this.ReadArrayOfRIFObjects<T>(true);
		}

		private T[] ReadArrayOfRIFObjects<T>(bool verify) where T : IPersistable
		{
			int num = default(int);
			if (this.m_reader.ReadArrayStart(ObjectType.RIFObjectArray, out num))
			{
				T[] array = new T[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = (T)this.ReadRIFObject(verify);
				}
				return array;
			}
			return null;
		}

        internal T[,][] Read2DArrayOfArrayOfRIFObjects<T>() where T : IPersistable
        {
            int num = -1;
            int num2 = -1;
            if (this.m_reader.Read2DArrayStart(ObjectType.Array2D, out num, out num2))
            {
                T[,][] array = new T[num, num2][];
                for (int i = 0; i < num; i++)
                {
                    for (int j = 0; j < num2; j++)
                    {
                        array[i, j] = this.ReadArrayOfRIFObjects<T>(false);
                    }
                }
                return array;
            }
            return null;
        }

        internal T[,] Read2DArrayOfRIFObjects<T>() where T : IPersistable
        {
            int num = -1;
            int num2 = -1;
            if (this.m_reader.Read2DArrayStart(ObjectType.Array2D, out num, out num2))
            {
                T[,] array = new T[num, num2];
                for (int i = 0; i < num; i++)
                {
                    for (int j = 0; j < num2; j++)
                    {
                        array[i, j] = (T)((object)this.ReadRIFObject());
                    }
                }
                return array;
            }
            return null;
        }

        internal T[] ReadArrayOfRIFObjects<RIFT, T>(Converter<RIFT, T> convertRIFObject) where RIFT : IPersistable
		{
			int num = default(int);
			if (this.m_reader.ReadArrayStart(ObjectType.RIFObjectArray, out num))
			{
				T[] array = new T[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = convertRIFObject((RIFT)this.ReadRIFObject());
				}
				return array;
			}
			return null;
		}

		internal string[] ReadStringArray()
		{
			int num = default(int);
			if (this.m_reader.ReadArrayStart(this.m_currentMember.ObjectType, out num))
			{
				string[] array = new string[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = this.ReadString();
				}
				return array;
			}
			return null;
		}

		internal object[] ReadVariantArray()
		{
			int num = default(int);
			if (this.m_reader.ReadArrayStart(this.m_currentMember.ObjectType, out num))
			{
				object[] array = new object[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = this.ReadVariant();
				}
				return array;
			}
			return null;
		}

		internal object[] ReadSerializableArray()
		{
			int num = default(int);
			if (this.m_reader.ReadArrayStart(this.m_currentMember.ObjectType, out num))
			{
				object[] array = new object[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = this.ReadSerializable();
				}
				return array;
			}
			return null;
		}

		internal object ReadSerializable()
		{
			Token token = this.m_reader.ReadToken();
			if (token == Token.Serializable)
			{
				return this.ReadISerializable();
			}
			return this.ReadVariant(token);
		}

		private object ReadISerializable()
		{
			try
			{
				if (this.m_binaryFormatter == null)
				{
					this.m_binaryFormatter = new BinaryFormatter();
				}
				return this.m_binaryFormatter.Deserialize(this.m_reader.BaseStream);
			}
			catch (Exception innerException)
			{
				throw new RSException(ErrorCode.rsProcessingError, ErrorStrings.Keys.GetString(ErrorCode.rsProcessingError.ToString()), innerException, Global.Tracer, null);
			}
		}

		internal object ReadVariant()
		{
			Token token = this.m_reader.ReadToken();
			return this.ReadVariant(token);
		}

        private object ReadVariant(Token token)
        {
            switch (token)
            {
                case Token.Null:
                    return null;
                case Token.String:
                    return this.m_reader.ReadString(false);
                case Token.Char:
                    return this.m_reader.ReadChar();
                case Token.Boolean:
                    return this.m_reader.ReadBoolean();
                case Token.Int16:
                    return this.m_reader.ReadInt16();
                case Token.Int32:
                    return this.m_reader.ReadInt32();
                case Token.Int64:
                    return this.m_reader.ReadInt64();
                case Token.UInt16:
                    return this.m_reader.ReadUInt16();
                case Token.UInt32:
                    return this.m_reader.ReadUInt32();
                case Token.UInt64:
                    return this.m_reader.ReadUInt64();
                case Token.Byte:
                    return this.m_reader.ReadByte();
                case Token.SByte:
                    return this.m_reader.ReadSByte();
                case Token.Single:
                    return this.m_reader.ReadSingle();
                case Token.Double:
                    return this.m_reader.ReadDouble();
                case Token.Decimal:
                    return this.m_reader.ReadDecimal();
                case Token.DateTime:
                    return this.m_reader.ReadDateTime();
                case Token.DateTimeWithKind:
                    return this.m_reader.ReadDateTimeWithKind();
                case Token.DateTimeOffset:
                    return this.m_reader.ReadDateTimeOffset();
                case Token.TimeSpan:
                    return this.m_reader.ReadTimeSpan();
                case Token.Guid:
                    return this.m_reader.ReadGuid();
                case Token.ByteArray:
                    return this.m_reader.ReadByteArray();
                //case Token.SqlGeography:
                //    return this.m_reader.ReadSqlGeography();
                //case Token.SqlGeometry:
                //    return this.m_reader.ReadSqlGeometry();
                case Token.Object:
                    return this.ReadRIFObject(false);
                default:
                    Global.Tracer.Assert(false);
                    return null;
            }
        }

		internal int[] ReadInt32Array()
		{
			return this.m_reader.ReadInt32Array();
		}

		internal long[] ReadInt64Array()
		{
			return this.m_reader.ReadInt64Array();
		}

		internal float[] ReadSingleArray()
		{
			return this.m_reader.ReadFloatArray();
		}

		internal char[] ReadCharArray()
		{
			return this.m_reader.ReadCharArray();
		}

		internal byte[] ReadByteArray()
		{
			return this.m_reader.ReadByteArray();
		}

		internal bool[] ReadBooleanArray()
		{
			return this.m_reader.ReadBooleanArray();
		}

		internal double[] ReadDoubleArray()
		{
			return this.m_reader.ReadDoubleArray();
		}

		internal byte ReadByte()
		{
			return this.ReadByte(true);
		}

		internal byte ReadByte(bool verify)
		{
			return this.m_reader.ReadByte();
		}

		internal sbyte ReadSByte()
		{
			return this.m_reader.ReadSByte();
		}

		internal char ReadChar()
		{
			return this.m_reader.ReadChar();
		}

		internal short ReadInt16()
		{
			return this.m_reader.ReadInt16();
		}

		internal ushort ReadUInt16()
		{
			return this.m_reader.ReadUInt16();
		}

		internal int ReadInt32()
		{
			return this.ReadInt32(true);
		}

		private int ReadInt32(bool verify)
		{
			return this.m_reader.ReadInt32();
		}

		internal uint ReadUInt32()
		{
			return this.m_reader.ReadUInt32();
		}

		internal long ReadInt64()
		{
			return this.m_reader.ReadInt64();
		}

		internal ulong ReadUInt64()
		{
			return this.m_reader.ReadUInt64();
		}

		internal float ReadSingle()
		{
			return this.m_reader.ReadSingle();
		}

		internal double ReadDouble()
		{
			return this.m_reader.ReadDouble();
		}

		internal decimal ReadDecimal()
		{
			return this.m_reader.ReadDecimal();
		}

		internal string ReadString()
		{
			return this.ReadString(true);
		}

		private string ReadString(bool verify)
		{
			return this.m_reader.ReadString();
		}

		internal bool ReadBoolean()
		{
			return this.m_reader.ReadBoolean();
		}

		internal DateTime ReadDateTime()
		{
			return this.m_reader.ReadDateTime();
		}

		internal DateTime ReadDateTimeWithKind()
		{
			return this.m_reader.ReadDateTimeWithKind();
		}

		internal DateTimeOffset ReadDateTimeOffset()
		{
			return this.m_reader.ReadDateTimeOffset();
		}

		internal TimeSpan ReadTimeSpan()
		{
			return this.m_reader.ReadTimeSpan();
		}

		internal int Read7BitEncodedInt()
		{
			return this.m_reader.ReadEnum();
		}

		internal int ReadEnum()
		{
			return this.m_reader.ReadEnum();
		}

		internal Guid ReadGuid()
		{
			return this.m_reader.ReadGuid();
		}

		internal CultureInfo ReadCultureInfo()
		{
			int num = this.m_reader.ReadInt32();
			if (num == -1)
			{
				return null;
			}
			return new CultureInfo(num, false);
		}

		internal List<T> ReadGenericListOfReferences<T>(IPersistable obj) where T : IReferenceable
		{
			int num = default(int);
			if (this.m_reader.ReadListStart(this.m_currentMember.ObjectType, out num))
			{
				List<T> list = new List<T>();
				for (int i = 0; i < num; i++)
				{
					T val = this.ReadReference<T>(obj, false);
					if (val != null)
					{
						list.Add(val);
					}
				}
				return list;
			}
			return null;
		}

		internal int ReadListOfReferencesNoResolution(IPersistable obj)
		{
			int num = default(int);
			if (this.m_reader.ReadListStart(this.m_currentMember.ObjectType, out num))
			{
				for (int i = 0; i < num; i++)
				{
					this.ReadReference<IReferenceable>(obj, true);
				}
			}
			return num;
		}

		internal T ReadListOfReferences<T, U>(IPersistable obj) where T : class, IList, new()where U : IReferenceable
		{
			int num = default(int);
			if (this.m_reader.ReadListStart(this.m_currentMember.ObjectType, out num))
			{
				T result = new T();
				for (int i = 0; i < num; i++)
				{
					U val = this.ReadReference<U>(obj, false);
					if (val != null)
					{
						result.Add(val);
					}
				}
				return result;
			}
			return null;
		}

		internal T ReadReference<T>(IPersistable obj) where T : IReferenceable
		{
			return this.ReadReference<T>(obj, false);
		}

		private T ReadReference<T>(IPersistable obj, bool delayReferenceResolution) where T : IReferenceable
		{
			int num = default(int);
			ObjectType objectType = default(ObjectType);
			if (this.m_reader.ReadReference(out num, out objectType))
			{
				IReferenceable referenceable = null;
				if (delayReferenceResolution || !this.m_referenceableItems.TryGetValue(num, out referenceable))
				{
					Dictionary<ObjectType, List<MemberReference>> dictionary = default(Dictionary<ObjectType, List<MemberReference>>);
					if (!this.m_memberReferencesCollection.TryGetValue(obj, out dictionary))
					{
						dictionary = new Dictionary<ObjectType, List<MemberReference>>(EqualityComparers.ObjectTypeComparerInstance);
						this.m_memberReferencesCollection.Add(obj, dictionary);
					}
					List<MemberReference> list = default(List<MemberReference>);
					if (!dictionary.TryGetValue(this.m_currentPersistedDeclaration.ObjectType, out list))
					{
						list = new List<MemberReference>();
						dictionary.Add(this.m_currentPersistedDeclaration.ObjectType, list);
					}
					list.Add(new MemberReference(this.m_currentMember.MemberName, num));
				}
				return (T)referenceable;
			}
			return default(T);
		}

		internal List<T> ReadGenericListOfGloablReferences<T>() where T : IGloballyReferenceable
		{
			int num = default(int);
			if (this.m_reader.ReadListStart(this.m_currentMember.ObjectType, out num))
			{
				List<T> list = new List<T>();
				for (int i = 0; i < num; i++)
				{
					T val = this.ReadGlobalReference<T>();
					if (val != null)
					{
						list.Add(val);
					}
				}
				return list;
			}
			return null;
		}

		internal T ReadListOfGloablReferences<T, U>() where T : class, IList, new()where U : IGloballyReferenceable
		{
			int num = default(int);
			if (this.m_reader.ReadListStart(this.m_currentMember.ObjectType, out num))
			{
				T result = new T();
				for (int i = 0; i < num; i++)
				{
					U val = this.ReadGlobalReference<U>();
					if (val != null)
					{
						result.Add(val);
					}
				}
				return result;
			}
			return null;
		}

		internal T ReadGlobalReference<T>() where T : IGloballyReferenceable
		{
			IGloballyReferenceable globallyReferenceable = null;
			int refID = default(int);
			ObjectType objectType = default(ObjectType);
			if (this.m_reader.ReadReference(out refID, out objectType))
			{
				this.m_globalIDOwners.TryGetValue(refID, out globallyReferenceable);
			}
			return (T)globallyReferenceable;
		}

		internal IntermediateFormatVersion ReadIntermediateFormatVersion()
		{
			long streamPosition = this.m_reader.StreamPosition;
			ObjectType objectType = this.m_reader.ReadObjectType();
			if (objectType != ObjectType.IntermediateFormatVersion)
			{
				throw new IncompatibleFormatVersionException(objectType, streamPosition);
			}
			IntermediateFormatVersion intermediateFormatVersion = new IntermediateFormatVersion();
			intermediateFormatVersion.Deserialize(this);
			return intermediateFormatVersion;
		}
	}
}
