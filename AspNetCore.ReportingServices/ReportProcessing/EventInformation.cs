using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class EventInformation
	{
		[Serializable]
		internal class SortEventInfo
		{
			[Serializable]
			private struct SortInfoStruct
			{
				internal int ReportItemUniqueName;

				internal bool SortDirection;

				internal Hashtable PeerSorts;

				internal SortInfoStruct(int uniqueName, bool direction, Hashtable peerSorts)
				{
					this.ReportItemUniqueName = uniqueName;
					this.SortDirection = direction;
					this.PeerSorts = peerSorts;
				}
			}

			private ArrayList m_collection;

			private Hashtable m_nameMap;

			internal int Count
			{
				get
				{
					return this.m_collection.Count;
				}
			}

			internal SortEventInfo()
			{
				this.m_collection = new ArrayList();
				this.m_nameMap = new Hashtable();
			}

			private SortEventInfo(SortEventInfo copy)
			{
				this.m_collection = (ArrayList)copy.m_collection.Clone();
				this.m_nameMap = (Hashtable)copy.m_nameMap.Clone();
			}

			internal void Add(int uniqueName, bool direction, Hashtable peerSorts)
			{
				this.Remove(uniqueName);
				this.m_nameMap.Add(uniqueName, this.m_collection.Count);
				this.m_collection.Add(new SortInfoStruct(uniqueName, direction, peerSorts));
			}

			internal bool Remove(int uniqueName)
			{
				object obj = this.m_nameMap[uniqueName];
				if (obj != null)
				{
					this.m_nameMap.Remove(uniqueName);
					this.m_collection.RemoveAt((int)obj);
					for (int i = (int)obj; i < this.m_collection.Count; i++)
					{
						SortInfoStruct sortInfoStruct = (SortInfoStruct)this.m_collection[i];
						this.m_nameMap[sortInfoStruct.ReportItemUniqueName] = i;
					}
					return true;
				}
				return false;
			}

			internal bool ClearPeerSorts(int uniqueName)
			{
				bool result = false;
				IntList intList = null;
				for (int i = 0; i < this.m_collection.Count; i++)
				{
					SortInfoStruct sortInfoStruct = (SortInfoStruct)this.m_collection[i];
					Hashtable peerSorts = sortInfoStruct.PeerSorts;
					if (peerSorts != null)
					{
						if (intList == null)
						{
							intList = new IntList();
						}
						if (peerSorts.Contains(uniqueName))
						{
							intList.Add(sortInfoStruct.ReportItemUniqueName);
						}
					}
				}
				if (intList != null)
				{
					if (0 < intList.Count)
					{
						for (int j = 0; j < intList.Count; j++)
						{
							this.Remove(intList[j]);
						}
						result = true;
					}
				}
				else if (this.m_collection.Count > 0)
				{
					this.m_nameMap.Clear();
					this.m_collection.Clear();
					result = true;
				}
				return result;
			}

			internal int GetUniqueNameAt(int index)
			{
				Global.Tracer.Assert(0 <= index && index < this.m_collection.Count, "(0 <= index && index < m_collection.Count)");
				return ((SortInfoStruct)this.m_collection[index]).ReportItemUniqueName;
			}

			internal bool GetSortDirectionAt(int index)
			{
				Global.Tracer.Assert(0 <= index && index < this.m_collection.Count, "(0 <= index && index < m_collection.Count)");
				return ((SortInfoStruct)this.m_collection[index]).SortDirection;
			}

			internal SortOptions GetSortState(int uniqueName)
			{
				if (this.m_nameMap != null)
				{
					Global.Tracer.Assert(null != this.m_collection, "(null != m_collection)");
					object obj = this.m_nameMap[uniqueName];
					if (obj != null)
					{
						SortInfoStruct sortInfoStruct = (SortInfoStruct)this.m_collection[(int)obj];
						if (!sortInfoStruct.SortDirection)
						{
							return SortOptions.Descending;
						}
						return SortOptions.Ascending;
					}
				}
				return SortOptions.None;
			}

			internal SortEventInfo Clone()
			{
				return new SortEventInfo(this);
			}
		}

		[Serializable]
		internal class OdpSortEventInfo
		{
			[Serializable]
			private struct SortInfoStruct
			{
				internal string EventSourceUniqueName;

				internal bool SortDirection;

				internal Hashtable PeerSorts;

				internal SortInfoStruct(string uniqueName, bool direction, Hashtable peerSorts)
				{
					this.EventSourceUniqueName = uniqueName;
					this.SortDirection = direction;
					this.PeerSorts = peerSorts;
				}
			}

			private ArrayList m_collection;

			private Dictionary<string, int> m_uniqueNameMap;

			internal int Count
			{
				get
				{
					return this.m_collection.Count;
				}
			}

			internal OdpSortEventInfo()
			{
				this.m_collection = new ArrayList();
				this.m_uniqueNameMap = new Dictionary<string, int>();
			}

			private OdpSortEventInfo(OdpSortEventInfo copy)
			{
				this.m_collection = (ArrayList)copy.m_collection.Clone();
				if (copy.m_uniqueNameMap != null)
				{
					this.m_uniqueNameMap = new Dictionary<string, int>(copy.m_uniqueNameMap.Count);
					IDictionaryEnumerator dictionaryEnumerator = (IDictionaryEnumerator)(object)copy.m_uniqueNameMap.GetEnumerator();
					while (dictionaryEnumerator.MoveNext())
					{
						if (dictionaryEnumerator.Key != null)
						{
							this.m_uniqueNameMap.Add(dictionaryEnumerator.Key as string, (int)dictionaryEnumerator.Value);
						}
					}
				}
			}

			internal void Add(string uniqueNameString, bool direction, Hashtable peerSorts)
			{
				this.Remove(uniqueNameString);
				this.m_uniqueNameMap.Add(uniqueNameString, this.m_collection.Count);
				this.m_collection.Add(new SortInfoStruct(uniqueNameString, direction, peerSorts));
			}

			internal bool Remove(int id, List<InstancePathItem> instancePath)
			{
				return this.Remove(InstancePathItem.GenerateUniqueNameString(id, instancePath));
			}

			internal bool Remove(string uniqueNameString)
			{
				int num = default(int);
				if (!this.m_uniqueNameMap.TryGetValue(uniqueNameString, out num))
				{
					return false;
				}
				this.m_uniqueNameMap.Remove(uniqueNameString);
				this.m_collection.RemoveAt(num);
				for (int i = num; i < this.m_collection.Count; i++)
				{
					SortInfoStruct sortInfoStruct = (SortInfoStruct)this.m_collection[i];
					this.m_uniqueNameMap[sortInfoStruct.EventSourceUniqueName] = i;
				}
				return true;
			}

			internal bool ClearPeerSorts(string uniqueNameString)
			{
				bool result = false;
				List<string> list = null;
				int num = 0;
				for (int i = 0; i < this.m_collection.Count; i++)
				{
					SortInfoStruct sortInfoStruct = (SortInfoStruct)this.m_collection[i];
					Hashtable peerSorts = sortInfoStruct.PeerSorts;
					if (peerSorts != null && peerSorts.Count != 0)
					{
						if (list == null)
						{
							list = new List<string>();
						}
						if (peerSorts.Contains(uniqueNameString))
						{
							list.Add(sortInfoStruct.EventSourceUniqueName);
							num++;
						}
					}
				}
				if (num != 0)
				{
					for (int j = 0; j < num; j++)
					{
						this.Remove(list[j]);
					}
					result = true;
				}
				return result;
			}

			internal string GetUniqueNameAt(int index)
			{
				Global.Tracer.Assert(0 <= index && index < this.m_collection.Count, "(0 <= index && index < m_collection.Count)");
				return ((SortInfoStruct)this.m_collection[index]).EventSourceUniqueName;
			}

			internal bool GetSortDirectionAt(int index)
			{
				Global.Tracer.Assert(0 <= index && index < this.m_collection.Count, "(0 <= index && index < m_collection.Count)");
				return ((SortInfoStruct)this.m_collection[index]).SortDirection;
			}

			internal SortOptions GetSortState(string eventSourceUniqueName)
			{
				if (this.m_uniqueNameMap != null)
				{
					Global.Tracer.Assert(null != this.m_collection, "(null != m_collection)");
					int index = default(int);
					if (this.m_uniqueNameMap.TryGetValue(eventSourceUniqueName, out index))
					{
						SortInfoStruct sortInfoStruct = (SortInfoStruct)this.m_collection[index];
						if (!sortInfoStruct.SortDirection)
						{
							return SortOptions.Descending;
						}
						return SortOptions.Ascending;
					}
				}
				return SortOptions.None;
			}

			internal OdpSortEventInfo Clone()
			{
				return new OdpSortEventInfo(this);
			}
		}

		[Serializable]
		internal class RendererEventInformation
		{
			private Hashtable m_validToggleSenders;

			private Hashtable m_drillthroughInfo;

			internal Hashtable ValidToggleSenders
			{
				get
				{
					return this.m_validToggleSenders;
				}
				set
				{
					this.m_validToggleSenders = value;
				}
			}

			internal Hashtable DrillthroughInfo
			{
				get
				{
					return this.m_drillthroughInfo;
				}
				set
				{
					this.m_drillthroughInfo = value;
				}
			}

			internal RendererEventInformation()
			{
			}

			internal RendererEventInformation(RendererEventInformation copy)
			{
				Global.Tracer.Assert(null != copy, "(null != copy)");
				if (copy.m_validToggleSenders != null)
				{
					this.m_validToggleSenders = (Hashtable)copy.m_validToggleSenders.Clone();
				}
				if (copy.m_drillthroughInfo != null)
				{
					this.m_drillthroughInfo = (Hashtable)copy.m_drillthroughInfo.Clone();
				}
			}

			internal void Reset()
			{
				this.m_validToggleSenders = null;
				this.m_drillthroughInfo = null;
			}

			internal bool ValidToggleSender(string senderId)
			{
				if (this.m_validToggleSenders != null)
				{
					return this.m_validToggleSenders.ContainsKey(senderId);
				}
				return false;
			}

			internal DrillthroughInfo GetDrillthroughInfo(string drillthroughId)
			{
				if (this.m_drillthroughInfo != null)
				{
					return (DrillthroughInfo)this.m_drillthroughInfo[drillthroughId];
				}
				return null;
			}
		}

		private bool m_hasShowHideInfo;

		private Hashtable m_toggleStateInfo;

		private Hashtable m_hiddenInfo;

		private bool m_hasSortInfo;

		private SortEventInfo m_sortInfo;

		private OdpSortEventInfo m_odpSortInfo;

		private Dictionary<string, RendererEventInformation> m_rendererEventInformation;

		[NonSerialized]
		private bool m_changed;

		internal Hashtable ToggleStateInfo
		{
			get
			{
				return this.m_toggleStateInfo;
			}
			set
			{
				this.m_hasShowHideInfo = (value != null);
				this.m_toggleStateInfo = value;
			}
		}

		internal Hashtable HiddenInfo
		{
			get
			{
				return this.m_hiddenInfo;
			}
			set
			{
				this.m_hiddenInfo = value;
			}
		}

		internal SortEventInfo SortInfo
		{
			get
			{
				return this.m_sortInfo;
			}
			set
			{
				this.m_hasSortInfo = (null != value);
				this.m_sortInfo = value;
			}
		}

		internal OdpSortEventInfo OdpSortInfo
		{
			get
			{
				return this.m_odpSortInfo;
			}
			set
			{
				this.m_hasSortInfo = (null != value);
				this.m_odpSortInfo = value;
			}
		}

		internal bool Changed
		{
			get
			{
				return this.m_changed;
			}
			set
			{
				this.m_changed = value;
			}
		}

		internal EventInformation()
		{
		}

		public EventInformation(EventInformation copy)
		{
			Global.Tracer.Assert(null != copy, "(null != copy)");
			this.m_hasShowHideInfo = copy.m_hasShowHideInfo;
			if (copy.m_toggleStateInfo != null)
			{
				this.m_toggleStateInfo = (Hashtable)copy.m_toggleStateInfo.Clone();
			}
			if (copy.m_hiddenInfo != null)
			{
				this.m_hiddenInfo = (Hashtable)copy.m_hiddenInfo.Clone();
			}
			this.m_hasSortInfo = copy.m_hasSortInfo;
			if (copy.m_sortInfo != null)
			{
				this.m_sortInfo = copy.m_sortInfo.Clone();
			}
			else if (copy.m_odpSortInfo != null)
			{
				this.m_odpSortInfo = copy.m_odpSortInfo.Clone();
			}
			if (copy.m_rendererEventInformation != null)
			{
				this.m_rendererEventInformation = new Dictionary<string, RendererEventInformation>(copy.m_rendererEventInformation.Count);
				foreach (string key in copy.m_rendererEventInformation.Keys)
				{
					RendererEventInformation copy2 = copy.m_rendererEventInformation[key];
					RendererEventInformation value = new RendererEventInformation(copy2);
					this.m_rendererEventInformation[key] = value;
				}
			}
		}

		public byte[] Serialize()
		{
			Global.Tracer.Assert(this.m_hasShowHideInfo || this.m_hasSortInfo || this.m_rendererEventInformation != null, "(m_hasShowHideInfo || m_hasSortInfo || m_rendererEventInformation != null)");
			MemoryStream memoryStream = null;
			try
			{
				if (this.m_hasShowHideInfo)
				{
					Global.Tracer.Assert(null != this.m_toggleStateInfo, "(null != m_toggleStateInfo)");
				}
				if (this.m_hasSortInfo)
				{
					Global.Tracer.Assert(this.m_sortInfo != null || null != this.m_odpSortInfo, "(null != m_sortInfo || null != m_odpSortInfo)");
				}
				memoryStream = new MemoryStream();
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				binaryFormatter.Serialize(memoryStream, this);
				return memoryStream.ToArray();
			}
			finally
			{
				if (memoryStream != null)
				{
					memoryStream.Close();
				}
			}
		}

		public static EventInformation Deserialize(byte[] data)
		{
			if (data == null)
			{
				return null;
			}
			MemoryStream stream = null;
			EventInformation result = null;
			try
			{
				stream = new MemoryStream(data, false);
				BinaryFormatter bFormatter = new BinaryFormatter();
                //result = (EventInformation)bFormatter.Deserialize(stream);
                //result.m_changed = false;
               
                //todo: can delete?
                RevertImpersonationContext.Run(delegate
				{
					EventInformation eventInformation = (EventInformation)bFormatter.Deserialize(stream);
					eventInformation.m_changed = false;
					result = eventInformation;
				});
			}
			finally
			{
				if (stream != null)
				{
					stream.Close();
				}
			}
			return result;
		}

		internal RendererEventInformation GetRendererEventInformation(string aRenderFormat)
		{
			if (this.m_rendererEventInformation == null)
			{
				this.m_rendererEventInformation = new Dictionary<string, RendererEventInformation>();
			}
			RendererEventInformation rendererEventInformation = null;
			if (!this.m_rendererEventInformation.TryGetValue(aRenderFormat, out rendererEventInformation))
			{
				rendererEventInformation = new RendererEventInformation();
				this.m_rendererEventInformation[aRenderFormat] = rendererEventInformation;
			}
			return rendererEventInformation;
		}

		internal bool ValidToggleSender(string senderId)
		{
			if (this.m_rendererEventInformation == null)
			{
				return false;
			}
			foreach (string key in this.m_rendererEventInformation.Keys)
			{
				if (this.m_rendererEventInformation[key].ValidToggleSender(senderId))
				{
					return true;
				}
			}
			return false;
		}

		internal DrillthroughInfo GetDrillthroughInfo(string drillthroughId)
		{
			if (this.m_rendererEventInformation != null)
			{
				foreach (string key in this.m_rendererEventInformation.Keys)
				{
					RendererEventInformation rendererEventInformation = this.m_rendererEventInformation[key];
					DrillthroughInfo drillthroughInfo = rendererEventInformation.GetDrillthroughInfo(drillthroughId);
					if (drillthroughInfo != null)
					{
						return drillthroughInfo;
					}
				}
			}
			return null;
		}
	}
}
