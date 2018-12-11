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
  private Queue<AssetContainer> downloadQueue;
  private AssetContainer mContainer;

  private void Start() {
    downloadQueue = new Queue<AssetContainer>();
  }

  public void enqueueAssetToDownload(ref AssetContainer container) {
    if ( !downloadQueue.Contains(container) ) {
      downloadQueue.Enqueue(container);
      Debug.Log ("AssetDownloader: Asset enqueued.");
    } else if ( mContainer.doesFileExistLocally () ) {
      Debug.Log ("AssetDownloader: Asset already downloaded.");
    } else { 
      Debug.Log ("AssetDownloader: Asset already queued.");
    }
  }

  public IAsyncOperation<AssetContainer> DownloadVideoAsync (AssetContainer container)
  {
    mContainer = container;

    var result = new AsyncCompletionSource<AssetContainer> ();
      
    if (mContainer.doesFileExistLocally ()) {
      Debug.Log ("AssetDownloader: File already exists.");
      result.SetCanceled ();
    } else {
      Debug.Log ("Coroutine: Start DownloadAsyncVideoData");
      StartCoroutine (DownloadVideoInternal (result, mContainer.AssetHttpEndpoint));
    }

    return result;
  }

  private void handleVideoByteBlob (byte[] data)
  {
    Debug.Log ("Coroutine/Promise/Handler: Begin writing data to file");
    byte[] _videoBytes = data;

    string _pathToFile = Path.Combine (Application.persistentDataPath, mContainer.AssignedAssetFiledName + mContainer.AssetFileType);

    mContainer.AssetLocalFilePath = _pathToFile;

    File.WriteAllBytes (_pathToFile, _videoBytes);
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
