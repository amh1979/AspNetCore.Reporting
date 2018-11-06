using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class RenderingInfoManager
	{
		private const string RenderingInfoChunkPrefix = "RenderingInfo_";

		private RenderingInfoRoot m_renderingInfoRoot;

		private string m_chunkName;

		internal Hashtable RenderingInfo
		{
			get
			{
				return this.RenderingInfoRoot.RenderingInfo;
			}
		}

		internal Hashtable SharedRenderingInfo
		{
			get
			{
				return this.RenderingInfoRoot.SharedRenderingInfo;
			}
		}

		internal Hashtable PageSectionRenderingInfo
		{
			get
			{
				return this.RenderingInfoRoot.PageSectionRenderingInfo;
			}
		}

		internal PaginationInfo PaginationInfo
		{
			get
			{
				return this.RenderingInfoRoot.PaginationInfo;
			}
			set
			{
				this.RenderingInfoRoot.PaginationInfo = value;
			}
		}

		private RenderingInfoRoot RenderingInfoRoot
		{
			get
			{
				if (this.m_renderingInfoRoot == null)
				{
					this.m_renderingInfoRoot = new RenderingInfoRoot();
				}
				return this.m_renderingInfoRoot;
			}
		}

		internal RenderingInfoManager(string rendererID, AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.GetReportChunk getChunkCallback, bool retrieveRenderingInfo)
		{
			this.m_chunkName = "RenderingInfo_" + rendererID;
			if (retrieveRenderingInfo)
			{
				this.m_renderingInfoRoot = this.Deserialize(getChunkCallback);
			}
			else
			{
				this.m_renderingInfoRoot = null;
			}
		}

		internal void Save(AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.CreateReportChunk createChunkCallback)
		{
			if (this.m_renderingInfoRoot != null)
			{
				this.Serialize(this.m_renderingInfoRoot, createChunkCallback);
			}
		}

		private RenderingInfoRoot Deserialize(AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.GetReportChunk getChunkCallback)
		{
			Stream stream = null;
			try
			{
				string text = default(string);
				stream = getChunkCallback(this.m_chunkName, AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Other, out text);
				RenderingInfoRoot result = null;
				if (stream != null)
				{
					BinaryFormatter bFormatter = new BinaryFormatter();
                    //result = (RenderingInfoRoot)bFormatter.Deserialize(stream);
                    //todo: can delete?
                    RevertImpersonationContext.Run(delegate
					{
						result = (RenderingInfoRoot)bFormatter.Deserialize(stream);
					});
				}
				return result;
			}
			catch (SerializationException)
			{
				return null;
			}
			finally
			{
				if (stream != null)
				{
					stream.Close();
				}
			}
		}

		private void Serialize(RenderingInfoRoot renderingInfoRoot, AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.CreateReportChunk createChunkCallback)
		{
			Stream stream = null;
			try
			{
				stream = createChunkCallback(this.m_chunkName, AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Other, null);
				if (stream != null)
				{
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					binaryFormatter.Serialize(stream, renderingInfoRoot);
				}
			}
			finally
			{
				if (stream != null)
				{
					stream.Close();
				}
			}
		}
	}
}
