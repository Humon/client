using UnityEngine;
using System.IO;

//-----------------------------------------------------------------------------
// Copyright 2015-2017 RenderHeads Ltd.  All rights reserverd.
//-----------------------------------------------------------------------------

namespace RenderHeads.Media.AVProVideo.Demos
{
	/// <summary>
	/// Demonstration of how to load from a video from a byte array.
	/// It should be noted that only Windows using DirectShow API currently supports this feature.
	/// </summary> 
	public class LoadFromBuffer : MonoBehaviour
	{
		[SerializeField]
		private MediaPlayer _mp = null;

		[SerializeField]
		private string _filename = string.Empty;

		void Start()
		{
			if (_mp != null)
			{
				byte[] buffer = null;
				using (FileStream fs = new FileStream(_filename, FileMode.Open, FileAccess.Read))
				{
					using (BinaryReader br = new BinaryReader(fs))
					{
						long bufferLength = new FileInfo(_filename).Length;
						buffer = br.ReadBytes((int)bufferLength);
					}
				}

				if (buffer != null)
				{
					_mp.OpenVideoFromBuffer(buffer);
				}
			}

			System.GC.Collect();
		}
	}
}

