using UnityEngine;
using System.Collections;
using UnityFx.Async;
using UnityFx.Async.Promises;

// For promises-specific stuff.
using UnityEngine.Networking;
using System;
using System.IO;

public class AssetDownloader : MonoBehaviour
{
	private AssetContainer mContainer;

	public IAsyncOperation<AssetContainer> DownloadVideoAsync (AssetContainer container)
	{
		mContainer = container;

		var result = new AsyncCompletionSource<AssetContainer> ();

		Debug.Log ("Coroutine: Start DownloadAsyncVideoData");

		StartCoroutine (DownloadVideoInternal (result, mContainer.AssetHttpEndpoint));
		return result;
	}

	private void handleVideoByteBlob (byte[] data)
	{
		Debug.Log ("Coroutine/Promise/Handler: Begin writing data to file");
		byte[] _videoBytes = data;
		string _pathToFile = Path.Combine (Application.persistentDataPath, mContainer.AssignedAssetFiledName);
		File.WriteAllBytes (_pathToFile, _videoBytes);
		mContainer.AssetLocalFilePath = _pathToFile;

		Debug.Log ("Coroutine/Promise/Handler: File stored at " + _pathToFile);
	}

	private IEnumerator DownloadVideoInternal (IAsyncCompletionSource<AssetContainer> op, string url)
	{
		Debug.Log ("Coroutine/Promise: Request for Video Data");

		var www = UnityWebRequest.Get (url);
		yield return www.SendWebRequest ();

		if (www.isNetworkError || www.isHttpError) {
			Debug.Log ("Coroutine/Promise: Request failed");
			op.SetException (new Exception (www.error));
		} else {
			Debug.Log ("Coroutine/Promise: Request succeeded");
			handleVideoByteBlob (www.downloadHandler.data);
			op.SetResult (mContainer);
		}
	}
}
