using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.ReportProcessing;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence
{
	internal struct IntermediateFormatWriter
	{
		private int m_currentMemberIndex;

		private Declaration m_currentDeclaration;

		private Dictionary<ObjectType, Declaration> m_writtenDecls;

		private PersistenceBinaryWriter m_writer;

		private PersistenceHelper m_persistenceContext;

		private bool m_isSeekable;

		private int m_lastMemberInfoIndex;

		private MemberInfo m_currentMember;

		private readonly bool m_prohibitSerializableValues;

		private int m_compatVersion;

		private BinaryFormatter m_binaryFormatter;

		private bool UsesCompatVersion
		{
			get
			{
				return this.m_compatVersion != 0;
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
				return this.m_persistenceContext;
			}
		}

		internal bool ProhibitSerializableValues
		{
			get
			{
				return this.m_prohibitSerializableValues;
			}
		}

		internal IntermediateFormatWriter(Stream str, int compatVersion)
		{
			this = new IntermediateFormatWriter(str, 0L, null, null, compatVersion, false);
		}

		internal IntermediateFormatWriter(Stream str, int compatVersion, bool prohibitSerializableValues)
		{
			this = new IntermediateFormatWriter(str, 0L, null, null, compatVersion, prohibitSerializableValues);
		}

		internal IntermediateFormatWriter(Stream str, PersistenceHelper persistenceContext, int compatVersion)
		{
			this = new IntermediateFormatWriter(str, 0L, null, persistenceContext, compatVersion, false);
		}

		internal IntermediateFormatWriter(Stream str, List<Declaration> declarations, int compatVersion)
		{
			this = new IntermediateFormatWriter(str, 0L, declarations, null, compatVersion, false);
		}

		internal IntermediateFormatWriter(Stream str, List<Declaration> declarations, int compatVersion, bool prohibitSerializableValues)
		{
			this = new IntermediateFormatWriter(str, 0L, declarations, null, compatVersion, prohibitSerializableValues);
		}

		internal IntermediateFormatWriter(Stream str, List<Declaration> declarations, PersistenceHelper persistenceContext, int compatVersion)
		{
			this = new IntermediateFormatWriter(str, 0L, declarations, persistenceContext, compatVersion, false);
		}

		internal IntermediateFormatWriter(Stream str, List<Declaration> declarations, PersistenceHelper persistenceContext, int compatVersion, bool prohibitSerializableValues)
		{
			this = new IntermediateFormatWriter(str, 0L, declarations, persistenceContext, compatVersion, prohibitSerializableValues);
		}

		internal IntermediateFormatWriter(Stream str, long startOffset, List<Declaration> declarations, PersistenceHelper persistenceContext, int compatVersion, bool prohibitSerializableValues)
		{
			this.m_writer = new PersistenceBinaryWriter(str);
			this.m_writtenDecls = new Dictionary<ObjectType, Declaration>(EqualityComparers.ObjectTypeComparerInstance);
			this.m_currentDeclaration = null;
			this.m_currentMemberIndex = 0;
			this.m_lastMemberInfoIndex = 0;
			this.m_currentMember = null;
			this.m_persistenceContext = persistenceContext;
			this.m_isSeekable = false;
			this.m_binaryFormatter = null;
			this.m_compatVersion = compatVersion;
			this.m_prohibitSerializableValues = prohibitSerializableValues;
			if (0 == startOffset)
			{
				Global.Tracer.Assert(!this.m_isSeekable, "(!m_isSeekable)");
				this.Write(IntermediateFormatVersion.Current);
			}
			this.m_isSeekable = (null != declarations);
			PersistenceFlags persistenceFlags = PersistenceFlags.None;
			if (this.m_isSeekable)
			{
				persistenceFlags = PersistenceFlags.Seekable;
			}
			if (this.UsesCompatVersion)
			{
				persistenceFlags |= PersistenceFlags.CompatVersioned;
			}
			if (0 == startOffset)
			{
				this.m_writer.WriteEnum((int)persistenceFlags);
				if (this.UsesCompatVersion)
				{
					this.m_writer.Write(this.m_compatVersion);
				}
				if (this.m_isSeekable)
				{
					this.WriteDeclarations(declarations);
				}
			}
			else if (this.m_isSeekable)
			{
				this.FilterAndStoreDeclarations(declarations);
			}
		}

		private void WriteDeclarations(List<Declaration> declarations)
		{
			this.m_writer.WriteListStart(ObjectType.Declaration, declarations.Count);
			for (int i = 0; i < declarations.Count; i++)
			{
				Declaration decl = declarations[i];
				this.WriteDeclaration(decl);
			}
		}

		private Declaration WriteDeclaration(Declaration decl)
		{
			decl = this.FilterAndStoreDeclaration(decl);
			this.m_writer.Write(decl);
			return decl;
		}

		private void FilterAndStoreDeclarations(List<Declaration> declarations)
		{
			for (int i = 0; i < declarations.Count; i++)
			{
				Declaration decl = declarations[i];
				this.FilterAndStoreDeclaration(decl);
			}
		}

		private Declaration FilterAndStoreDeclaration(Declaration decl)
		{
			decl = decl.CreateFilteredDeclarationForWriteVersion(this.m_compatVersion);
			this.m_writtenDecls.Add(decl.ObjectType, decl);
			return decl;
		}

		internal void RegisterDeclaration(Declaration declaration)
		{
			Declaration currentDeclaration = default(Declaration);
			if (!this.m_writtenDecls.TryGetValue(declaration.ObjectType, out currentDeclaration))
			{
				currentDeclaration = this.WriteDeclaration(declaration);
			}
			this.m_currentDeclaration = currentDeclaration;
			this.m_lastMemberInfoIndex = this.m_currentDeclaration.MemberInfoList.Count - 1;
			this.m_currentMemberIndex = -1;
		}

		internal bool NextMember()
		{
			if (this.m_currentMemberIndex < this.m_lastMemberInfoIndex)
			{
				this.m_currentMemberIndex++;
				this.m_currentMember = this.m_currentDeclaration.MemberInfoList[this.m_currentMemberIndex];
				return true;
			}
			return false;
		}

		internal void Write(IPersistable persistableObj)
		{
			this.Write(persistableObj, true);
		}

		private void Write(IPersistable persistableObj, bool verify)
		{
			if (persistableObj != null)
			{
				this.m_writer.Write(persistableObj.GetObjectType());
				persistableObj.Serialize(this);
			}
			else
			{
				this.m_writer.WriteNull();
			}
		}

		internal void WriteNameObjectCollection(INameObjectCollection collection)
		{
			if (collection == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.WriteDictionaryStart(this.m_currentMember.ObjectType, collection.Count);
				for (int i = 0; i < collection.Count; i++)
				{
					this.m_writer.Write(collection.GetKey(i));
					this.Write(collection.GetValue(i));
				}
			}
		}

		internal void WriteStringRIFObjectDictionary<TVal>(Dictionary<string, TVal> dictionary) where TVal : IPersistable
		{
			if (dictionary == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.WriteDictionaryStart(this.m_currentMember.ObjectType, dictionary.Count);
				foreach (KeyValuePair<string, TVal> item in dictionary)
				{
					this.m_writer.Write(item.Key);
					this.Write((IPersistable)(object)item.Value);
				}
			}
		}

		internal void WriteStringListOfStringDictionary(Dictionary<string, List<string>> dictionary)
		{
			if (dictionary == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.WriteDictionaryStart(this.m_currentMember.ObjectType, dictionary.Count);
				foreach (KeyValuePair<string, List<string>> item in dictionary)
				{
					this.m_writer.Write(item.Key);
					this.WriteListOfPrimitives(item.Value, false);
				}
			}
		}

		internal void WriteInt32RIFObjectDictionary<TVal>(Dictionary<int, TVal> dictionary) where TVal : IPersistable
		{
			if (dictionary == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.WriteDictionaryStart(this.m_currentMember.ObjectType, dictionary.Count);
				foreach (KeyValuePair<int, TVal> item in dictionary)
				{
					this.m_writer.Write(item.Key);
					this.Write((IPersistable)(object)item.Value);
				}
			}
		}

		internal void WriteInt32RIFObjectDictionary<TVal>(IDictionary dictionary) where TVal : IPersistable
		{
			if (dictionary == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.WriteDictionaryStart(this.m_currentMember.ObjectType, dictionary.Count);
				IDictionaryEnumerator enumerator = dictionary.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator.Current;
						this.m_writer.Write((int)dictionaryEntry.Key);
						this.Write((IPersistable)(object)(TVal)dictionaryEntry.Value);
					}
				}
				finally
				{
					IDisposable disposable = enumerator as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
		}

		internal void WriteStringRIFObjectHashtable(Hashtable hashtable)
		{
			if (hashtable == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.WriteDictionaryStart(this.m_currentMember.ObjectType, hashtable.Count);
				IDictionaryEnumerator enumerator = hashtable.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator.Current;
						this.m_writer.Write((string)dictionaryEntry.Key);
						this.Write((IPersistable)dictionaryEntry.Value);
					}
				}
				finally
				{
					IDisposable disposable = enumerator as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
		}

		internal void WriteInt32PrimitiveListHashtable<T>(Hashtable hashtable)
		{
			if (hashtable == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.WriteDictionaryStart(this.m_currentMember.ObjectType, hashtable.Count);
				IDictionaryEnumerator enumerator = hashtable.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator.Current;
						this.m_writer.Write((int)dictionaryEntry.Key);
						this.WriteListOfPrimitives((List<T>)dictionaryEntry.Value, false);
					}
				}
				finally
				{
					IDisposable disposable = enumerator as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
		}

		internal void WriteStringObjectHashtable(IDictionary dictionary)
		{
			if (dictionary == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.WriteDictionaryStart(this.m_currentMember.ObjectType, dictionary.Count);
				IDictionaryEnumerator enumerator = dictionary.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator.Current;
						this.m_writer.Write((string)dictionaryEntry.Key);
						this.Write(dictionaryEntry.Value);
					}
				}
				finally
				{
					IDisposable disposable = enumerator as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
		}

		internal void WriteStringRIFObjectHashtable(IDictionary dictionary)
		{
			if (dictionary == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.WriteDictionaryStart(this.m_currentMember.ObjectType, dictionary.Count);
				IDictionaryEnumerator enumerator = dictionary.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator.Current;
						this.m_writer.Write((string)dictionaryEntry.Key);
						this.Write((IPersistable)dictionaryEntry.Value);
					}
				}
				finally
				{
					IDisposable disposable = enumerator as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
		}

		internal void WriteStringInt32Hashtable(IDictionary dictionary)
		{
			if (dictionary == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.WriteDictionaryStart(this.m_currentMember.ObjectType, dictionary.Count);
				IDictionaryEnumerator enumerator = dictionary.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator.Current;
						this.m_writer.Write((string)dictionaryEntry.Key);
						this.m_writer.Write((int)dictionaryEntry.Value);
					}
				}
				finally
				{
					IDisposable disposable = enumerator as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
		}

		internal void WriteStringStringHashtable(IDictionary dictionary)
		{
			if (dictionary == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.WriteDictionaryStart(this.m_currentMember.ObjectType, dictionary.Count);
				IDictionaryEnumerator enumerator = dictionary.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator.Current;
						this.m_writer.Write((string)dictionaryEntry.Key);
						this.m_writer.Write((string)dictionaryEntry.Value);
					}
				}
				finally
				{
					IDisposable disposable = enumerator as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
		}

		internal void WriteObjectHashtableHashtable(Hashtable hashtable)
		{
			if (hashtable == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.WriteDictionaryStart(this.m_currentMember.ObjectType, hashtable.Count);
				IDictionaryEnumerator enumerator = hashtable.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator.Current;
						this.Write(dictionaryEntry.Key);
						this.WriteVariantVariantHashtable((Hashtable)dictionaryEntry.Value);
					}
				}
				finally
				{
					IDisposable disposable = enumerator as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
		}

		internal void WriteNLevelVariantHashtable(Hashtable hashtable)
		{
			if (hashtable == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.WriteDictionaryStart(this.m_currentMember.ObjectType, hashtable.Count);
				IDictionaryEnumerator enumerator = hashtable.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator.Current;
						this.Write(dictionaryEntry.Key);
						Hashtable hashtable2 = dictionaryEntry.Value as Hashtable;
						if (hashtable2 != null)
						{
							this.m_writer.Write(Token.Hashtable);
							this.WriteNLevelVariantHashtable(hashtable2);
						}
						else
						{
							this.m_writer.Write(Token.Object);
							this.Write(dictionaryEntry.Value, false, true);
						}
					}
				}
				finally
				{
					IDisposable disposable = enumerator as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
		}

		internal void WriteRIFObjectStringHashtable(IDictionary hashtable)
		{
			if (hashtable == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.WriteDictionaryStart(this.m_currentMember.ObjectType, hashtable.Count);
				IDictionaryEnumerator enumerator = hashtable.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator.Current;
						this.Write((IPersistable)dictionaryEntry.Key);
						this.m_writer.Write((string)dictionaryEntry.Value);
					}
				}
				finally
				{
					IDisposable disposable = enumerator as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
		}

		internal void WriteVariantVariantHashtable(Hashtable hashtable)
		{
			if (hashtable == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.WriteDictionaryStart(this.m_currentMember.ObjectType, hashtable.Count);
				IDictionaryEnumerator enumerator = hashtable.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator.Current;
						this.Write(dictionaryEntry.Key);
						this.Write(dictionaryEntry.Value);
					}
				}
				finally
				{
					IDisposable disposable = enumerator as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
		}

		internal void WriteVariantListVariantDictionary(Dictionary<List<object>, object> dictionary)
		{
			if (dictionary == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.WriteDictionaryStart(this.m_currentMember.ObjectType, dictionary.Count);
				foreach (KeyValuePair<List<object>, object> item in dictionary)
				{
					this.WriteListOfVariant(item.Key);
					this.Write(item.Value);
				}
			}
		}

		internal void WriteStringVariantListDictionary(Dictionary<string, List<object>> dictionary)
		{
			if (dictionary == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.WriteDictionaryStart(this.m_currentMember.ObjectType, dictionary.Count);
				foreach (KeyValuePair<string, List<object>> item in dictionary)
				{
					this.Write(item.Key);
					this.WriteListOfVariant(item.Value);
				}
			}
		}

		internal void WriteStringBoolArrayDictionary(Dictionary<string, bool[]> dictionary)
		{
			if (dictionary == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.WriteDictionaryStart(this.m_currentMember.ObjectType, dictionary.Count);
				foreach (KeyValuePair<string, bool[]> item in dictionary)
				{
					this.m_writer.Write(item.Key);
					this.m_writer.Write(item.Value);
				}
			}
		}

		internal void WriteInt32StringHashtable(Hashtable hashtable)
		{
			if (hashtable == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.WriteDictionaryStart(this.m_currentMember.ObjectType, hashtable.Count);
				IDictionaryEnumerator enumerator = hashtable.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator.Current;
						this.m_writer.Write((int)dictionaryEntry.Key);
						this.m_writer.Write((string)dictionaryEntry.Value);
					}
				}
				finally
				{
					IDisposable disposable = enumerator as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
		}

		internal void WriteByteVariantHashtable(IDictionary dictionary)
		{
			if (dictionary == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.WriteDictionaryStart(this.m_currentMember.ObjectType, dictionary.Count);
				IDictionaryEnumerator enumerator = dictionary.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator.Current;
						this.m_writer.Write((byte)dictionaryEntry.Key);
						this.Write(dictionaryEntry.Value);
					}
				}
				finally
				{
					IDisposable disposable = enumerator as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
		}

		internal void WriteVariantRifObjectDictionary(IDictionary dictionary)
		{
			if (dictionary == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.WriteDictionaryStart(this.m_currentMember.ObjectType, dictionary.Count);
				IDictionaryEnumerator enumerator = dictionary.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator.Current;
						this.Write(dictionaryEntry.Key);
						this.Write((IPersistable)dictionaryEntry.Value);
					}
				}
				finally
				{
					IDisposable disposable = enumerator as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
		}

		internal void WriteVariantListOfRifObjectDictionary(IDictionary dictionary)
		{
			if (dictionary == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.WriteDictionaryStart(this.m_currentMember.ObjectType, dictionary.Count);
				IDictionaryEnumerator enumerator = dictionary.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator.Current;
						this.Write(dictionaryEntry.Key);
						this.Write((IPersistable)dictionaryEntry.Value);
					}
				}
				finally
				{
					IDisposable disposable = enumerator as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
		}

		internal void Int32SerializableDictionary(Dictionary<int, object> dictionary)
		{
			if (dictionary == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.WriteDictionaryStart(this.m_currentMember.ObjectType, dictionary.Count);
				foreach (KeyValuePair<int, object> item in dictionary)
				{
					this.m_writer.Write(item.Key);
					this.WriteSerializable(item.Value);
				}
			}
		}

		internal void WriteListOfReferences(IList rifList)
		{
			if (rifList == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.WriteListStart(this.m_currentMember.ObjectType, rifList.Count);
				for (int i = 0; i < rifList.Count; i++)
				{
					this.WriteReferenceInList((IReferenceable)rifList[i]);
				}
			}
		}

		internal void WriteListOfGlobalReferences(IList rifList)
		{
			if (rifList == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.WriteListStart(this.m_currentMember.ObjectType, rifList.Count);
				for (int i = 0; i < rifList.Count; i++)
				{
					this.WriteGlobalReferenceInList((IGloballyReferenceable)rifList[i]);
				}
			}
		}

		internal void Write<T>(List<T> rifList) where T : IPersistable
		{
			this.WriteRIFList(rifList);
		}

		internal void WriteRIFList<T>(IList<T> rifList) where T : IPersistable
		{
			if (rifList == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.WriteListStart(this.m_currentMember.ObjectType, ((ICollection<T>)rifList).Count);
				for (int i = 0; i < ((ICollection<T>)rifList).Count; i++)
				{
					this.Write((IPersistable)(object)rifList[i]);
				}
			}
		}

		internal void Write(ArrayList rifObjectList)
		{
			if (rifObjectList == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.WriteListStart(this.m_currentMember.ObjectType, rifObjectList.Count);
				for (int i = 0; i < rifObjectList.Count; i++)
				{
					this.Write((IPersistable)rifObjectList[i]);
				}
			}
		}

		internal void Write<T>(List<List<T>> rifObjectLists) where T : IPersistable
		{
			if (rifObjectLists == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.WriteListStart(this.m_currentMember.ObjectType, rifObjectLists.Count);
				for (int i = 0; i < rifObjectLists.Count; i++)
				{
					this.Write(rifObjectLists[i]);
				}
			}
		}

		internal void Write<T>(List<T[]> rifObjectArrays) where T : IPersistable
		{
			if (rifObjectArrays == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.WriteListStart(this.m_currentMember.ObjectType, rifObjectArrays.Count);
				for (int i = 0; i < rifObjectArrays.Count; i++)
				{
					this.Write(rifObjectArrays[i]);
				}
			}
		}

		internal void WriteListOfVariant(IList list)
		{
			if (list == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.WriteListStart(ObjectType.VariantList, list.Count);
				for (int i = 0; i < list.Count; i++)
				{
					this.Write(list[i], false, true);
				}
			}
		}

		internal void WriteArrayListOfPrimitives(ArrayList list)
		{
			if (list == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.WriteListStart(ObjectType.PrimitiveList, list.Count);
				for (int i = 0; i < list.Count; i++)
				{
					this.Write(list[i], false, true);
				}
			}
		}

		internal void WriteListOfPrimitives<T>(List<T> list)
		{
			this.WriteListOfPrimitives(list, true);
		}

		private void WriteListOfPrimitives<T>(List<T> list, bool verify)
		{
			if (list == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.WriteListStart(ObjectType.PrimitiveList, list.Count);
				for (int i = 0; i < list.Count; i++)
				{
					this.Write(list[i], false, true);
				}
			}
		}

		internal void WriteArrayOfListsOfPrimitives<T>(List<T>[] arrayOfLists)
		{
			this.WriteArrayOfListsOfPrimitives(arrayOfLists, true);
		}

		private void WriteArrayOfListsOfPrimitives<T>(List<T>[] arrayOfLists, bool validate)
		{
			if (arrayOfLists == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.WriteArrayStart(ObjectType.PrimitiveArray, arrayOfLists.Length);
				for (int i = 0; i < arrayOfLists.Length; i++)
				{
					this.WriteListOfPrimitives(arrayOfLists[i]);
				}
			}
		}

		internal void WriteListOfArrayOfListsOfPrimitives<T>(List<List<T>[]> outerList)
		{
			if (outerList == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.WriteListStart(ObjectType.PrimitiveList, outerList.Count);
				for (int i = 0; i < outerList.Count; i++)
				{
					this.WriteArrayOfListsOfPrimitives(outerList[i], false);
				}
			}
		}

		internal void Write<T>(List<T>[] rifObjectListArray) where T : IPersistable
		{
			if (rifObjectListArray == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.WriteArrayStart(ObjectType.RIFObjectArray, rifObjectListArray.Length);
				for (int i = 0; i < rifObjectListArray.Length; i++)
				{
					this.Write(rifObjectListArray[i]);
				}
			}
		}

		internal void Write(string[] strings)
		{
			if (strings == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.WriteArrayStart(ObjectType.PrimitiveArray, strings.Length);
				for (int i = 0; i < strings.Length; i++)
				{
					this.Write(strings[i]);
				}
			}
		}

		internal void Write(object[] array)
		{
			if (array == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.WriteArrayStart(ObjectType.PrimitiveArray, array.Length);
				for (int i = 0; i < array.Length; i++)
				{
					this.Write(array[i]);
				}
			}
		}

		internal void WriteVariantOrPersistableArray(object[] array)
		{
			if (array == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.WriteArrayStart(ObjectType.PrimitiveArray, array.Length);
				for (int i = 0; i < array.Length; i++)
				{
					this.WriteVariantOrPersistable(array[i]);
				}
			}
		}

		internal void WriteSerializableArray(object[] array)
		{
			if (array == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.WriteArrayStart(ObjectType.SerializableArray, array.Length);
				for (int i = 0; i < array.Length; i++)
				{
					this.WriteSerializable(array[i]);
				}
			}
		}

		internal void Write(IPersistable[] array)
		{
			if (array == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				this.m_writer.WriteArrayStart(ObjectType.RIFObjectArray, array.Length);
				for (int i = 0; i < array.Length; i++)
				{
					this.Write(array[i]);
				}
			}
		}

		internal void Write(IPersistable[,][] array)
		{
			if (array == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				int length = array.GetLength(0);
				int length2 = array.GetLength(1);
				this.m_writer.Write2DArrayStart(ObjectType.Array2D, length, length2);
				for (int i = 0; i < length; i++)
				{
					for (int j = 0; j < length2; j++)
					{
						this.Write(array[i, j]);
					}
				}
			}
		}

		internal void Write(IPersistable[,] array)
		{
			if (array == null)
			{
				this.m_writer.WriteNull();
			}
			else
			{
				int length = array.GetLength(0);
				int length2 = array.GetLength(1);
				this.m_writer.Write2DArrayStart(ObjectType.Array2D, length, length2);
				for (int i = 0; i < length; i++)
				{
					for (int j = 0; j < length2; j++)
					{
						this.Write(array[i, j]);
					}
				}
			}
		}

		internal void Write(float[] array)
		{
			this.m_writer.Write(array);
		}

		internal void Write(int[] array)
		{
			this.m_writer.Write(array);
		}

		internal void Write(long[] array)
		{
			this.m_writer.Write(array);
		}

		internal void Write(char[] array)
		{
			this.m_writer.Write(array);
		}

		internal void Write(byte[] array)
		{
			this.m_writer.Write(array);
		}

		internal void Write(bool[] array)
		{
			this.m_writer.Write(array);
		}

		internal void Write(double[] array)
		{
			this.m_writer.Write(array);
		}

		internal void Write(DateTime dateTime)
		{
			this.m_writer.Write(dateTime, this.m_currentMember.Token);
		}

		internal void Write(DateTimeOffset dateTimeOffset)
		{
			this.m_writer.Write(dateTimeOffset);
		}

		internal void Write(TimeSpan timeSpan)
		{
			this.m_writer.Write(timeSpan);
		}

		internal void Write(Guid guid)
		{
			this.m_writer.Write(guid);
		}

		internal void Write(string value)
		{
			this.m_writer.Write(value);
		}

		internal void Write(bool value)
		{
			this.m_writer.Write(value);
		}

		internal void Write(short value)
		{
			this.m_writer.Write(value);
		}

		internal void Write(int value)
		{
			this.m_writer.Write(value);
		}

		internal void Write(long value)
		{
			this.m_writer.Write(value);
		}

		internal void Write(ushort value)
		{
			this.m_writer.Write(value);
		}

		internal void Write(uint value)
		{
			this.m_writer.Write(value);
		}

		internal void Write(ulong value)
		{
			this.m_writer.Write(value);
		}

		internal void Write(char value)
		{
			this.m_writer.Write(value);
		}

		internal void Write(byte value)
		{
			this.m_writer.Write(value);
		}

		internal void Write(sbyte value)
		{
			this.m_writer.Write(value);
		}

		internal void Write(float value)
		{
			this.m_writer.Write(value);
		}

		internal void Write(double value)
		{
			this.m_writer.Write(value);
		}

		internal void Write(decimal value)
		{
			this.m_writer.Write(value);
		}

		internal void Write7BitEncodedInt(int value)
		{
			this.m_writer.WriteEnum(value);
		}

		internal void WriteEnum(int value)
		{
			this.m_writer.WriteEnum(value);
		}

		internal void WriteNull()
		{
			this.m_writer.WriteNull();
		}

		internal void Write(CultureInfo threadCulture)
		{
			if (threadCulture != null)
			{
				this.m_writer.Write(threadCulture.LCID);
			}
			else
			{
				this.m_writer.Write(-1);
			}
		}

		private void WriteReferenceInList(IReferenceable referenceableItem)
		{
			this.WriteReferenceID((referenceableItem != null) ? referenceableItem.ID : (-2));
		}

		internal void WriteReference(IReferenceable referenceableItem)
		{
			this.WriteReferenceID((referenceableItem != null) ? referenceableItem.ID : (-1));
		}

		internal void WriteReferenceID(int referenceID)
		{
			if (referenceID == -1)
			{
				this.WriteNull();
			}
			else
			{
				this.m_writer.Write(this.m_currentMember.ObjectType);
				this.m_writer.Write(referenceID);
			}
		}

		private void WriteGlobalReferenceInList(IGloballyReferenceable globalReference)
		{
			this.WriteGlobalReferenceID((globalReference != null) ? globalReference.GlobalID : (-2));
		}

		internal void WriteGlobalReference(IGloballyReferenceable globalReference)
		{
			this.WriteGlobalReferenceID((globalReference != null) ? globalReference.GlobalID : (-1));
		}

		internal void WriteGlobalReferenceID(int globalReferenceID)
		{
			if (globalReferenceID == -1)
			{
				this.WriteNull();
			}
			else
			{
				this.m_writer.Write(this.m_currentMember.ObjectType);
				this.m_writer.Write(globalReferenceID);
			}
		}

		internal void Write(ObjectType type)
		{
			this.m_writer.Write(type);
		}

        internal bool CanWrite(object obj)
        {
            if (obj == null)
            {
                return true;
            }
            Type type = obj.GetType();
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Empty:
                case TypeCode.DBNull:
                case TypeCode.Boolean:
                case TypeCode.Char:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.DateTime:
                case TypeCode.String:
                    return true;
                default:
                    if (!(obj is IPersistable) && !(obj is DateTimeOffset) && !(obj is TimeSpan) && !(obj is Guid) && !(obj is byte[]))
                    {
                        //if (!(obj is SqlGeography) && !(obj is SqlGeometry))
                        //{
                        //    return false;
                        //}
                        return true;
                    }
                    return true;
            }
        }

		internal void WriteSerializable(object obj)
		{
			if (!this.TryWriteSerializable(obj))
			{
				this.m_writer.Write(Token.Null);
			}
		}

		internal bool TryWriteSerializable(object obj)
		{
			if (!this.TryWrite(obj))
			{
				long position = this.m_writer.BaseStream.Position;
				try
				{
					if (!this.ProhibitSerializableValues && (obj is ISerializable || (obj.GetType().Attributes & TypeAttributes.Serializable) != 0))
					{
						if (this.m_binaryFormatter == null)
						{
							this.m_binaryFormatter = new BinaryFormatter();
						}
						this.m_writer.Write(Token.Serializable);
						this.m_binaryFormatter.Serialize(this.m_writer.BaseStream, obj);
						goto end_IL_001d;
					}
					return false;
					end_IL_001d:;
				}
				catch (RSException)
				{
					throw;
				}
				catch (Exception ex2)
				{
					this.m_writer.BaseStream.Position = position;
					Global.Tracer.Trace(TraceLevel.Warning, "Error occurred during serialization: " + ex2.Message);
					return false;
				}
			}
			return true;
		}

		internal void WriteVariantOrPersistable(object obj)
		{
			IPersistable persistable = obj as IPersistable;
			if (persistable != null)
			{
				this.m_writer.Write(Token.Object);
				this.Write(persistable, false);
			}
			else
			{
				this.Write(obj);
			}
		}

		internal void Write(object obj)
		{
			this.Write(obj, true, true);
		}

		internal bool TryWrite(object obj)
		{
			return this.Write(obj, true, false);
		}

		private bool Write(object obj, bool verify, bool assertOnInvalidType)
		{
			if (obj == null || obj == DBNull.Value)
			{
				this.m_writer.Write(Token.Null);
			}
			else
			{
				Type type = obj.GetType();
				switch (Type.GetTypeCode(type))
				{
				case TypeCode.Empty:
				case TypeCode.DBNull:
					this.m_writer.Write(Token.Null);
					break;
				case TypeCode.Boolean:
					this.m_writer.Write(Token.Boolean);
					this.m_writer.Write((bool)obj);
					break;
				case TypeCode.Byte:
					this.m_writer.Write(Token.Byte);
					this.m_writer.Write((byte)obj);
					break;
				case TypeCode.Char:
					this.m_writer.Write(Token.Char);
					this.m_writer.Write((char)obj);
					break;
				case TypeCode.DateTime:
				{
					DateTime dateTime = (DateTime)obj;
					Token token = this.m_currentMember.Token;
					if (token == Token.Object || token == Token.Serializable)
					{
						token = (Token)((dateTime.Kind == DateTimeKind.Unspecified) ? 241 : 236);
					}
					this.m_writer.Write(token);
					this.m_writer.Write(dateTime, token);
					break;
				}
				case TypeCode.Decimal:
					this.m_writer.Write(Token.Decimal);
					this.m_writer.Write((decimal)obj);
					break;
				case TypeCode.Double:
					this.m_writer.Write(Token.Double);
					this.m_writer.Write((double)obj);
					break;
				case TypeCode.Int16:
					this.m_writer.Write(Token.Int16);
					this.m_writer.Write((short)obj);
					break;
				case TypeCode.Int32:
					this.m_writer.Write(Token.Int32);
					this.m_writer.Write((int)obj);
					break;
				case TypeCode.Int64:
					this.m_writer.Write(Token.Int64);
					this.m_writer.Write((long)obj);
					break;
				case TypeCode.SByte:
					this.m_writer.Write(Token.SByte);
					this.m_writer.Write((sbyte)obj);
					break;
				case TypeCode.Single:
					this.m_writer.Write(Token.Single);
					this.m_writer.Write((float)obj);
					break;
				case TypeCode.String:
					this.m_writer.Write(Token.String);
					this.m_writer.Write((string)obj, false);
					break;
				case TypeCode.UInt16:
					this.m_writer.Write(Token.UInt16);
					this.m_writer.Write((ushort)obj);
					break;
				case TypeCode.UInt32:
					this.m_writer.Write(Token.UInt32);
					this.m_writer.Write((uint)obj);
					break;
				case TypeCode.UInt64:
					this.m_writer.Write(Token.UInt64);
					this.m_writer.Write((ulong)obj);
					break;
				default:
					if (obj is TimeSpan)
					{
						this.m_writer.Write(Token.TimeSpan);
						this.m_writer.Write((TimeSpan)obj);
						break;
					}
					if (obj is DateTimeOffset)
					{
						this.m_writer.Write(Token.DateTimeOffset);
						this.m_writer.Write((DateTimeOffset)obj);
						break;
					}
					if (obj is Guid)
					{
						this.m_writer.Write(Token.Guid);
						this.m_writer.Write((Guid)obj);
						break;
					}
					if (obj is Enum)
					{
						Global.Tracer.Assert(false, "You must call WriteEnum for enums");
						break;
					}
					if (obj is byte[])
					{
						this.m_writer.Write(Token.ByteArray);
						this.m_writer.Write((byte[])obj);
						break;
					}
                        //if (obj is SqlGeography)
                        //{
                        //    this.m_writer.Write(Token.SqlGeography);
                        //    this.m_writer.Write((SqlGeography)obj);
                        //    break;
                        //}
                        //if (obj is SqlGeometry)
                        //{
                        //    this.m_writer.Write(Token.SqlGeometry);
                        //    this.m_writer.Write((SqlGeometry)obj);
                        //    break;
                        //}
                        if (assertOnInvalidType)
					{
						Global.Tracer.Assert(false, "Unsupported object type: " + obj.GetType());
					}
					return false;
				}
			}
			return true;
		}
	}
}
