﻿using UnityEngine;
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
  private bool isReadyToDownload = true;

  public Action<float, string> progressChangedCallback;

  private void Start() {
    downloadQueue = new Queue<AssetContainer>();
  }

  private void Update() {
    initiateNextDownload();
  }

  private void initiateNextDownload()  {
    if ( isReadyToDownload ) {
      if ( downloadQueue.Count > 0 ) {
        isReadyToDownload = false;
        mContainer = downloadQueue.Dequeue();

        IAsyncOperation<AssetContainer> asyncOp = DownloadVideoAsync(mContainer);
      }
    }
  }

  public void enqueueAssetToDownload(ref AssetContainer container) {
    if ( shouldEnqueue(container) ) {
      downloadQueue.Enqueue(container);
      Debug.Log ("AssetDownloader: Asset enqueued.");
    }
  }

  private bool shouldEnqueue( ref AssetContainer container ) {
    bool result = false;
    if ( !downloadQueue.Contains(container) ) {
      result = true;
    } else if ( mContainer.doesFileExistLocally () ) {
      Debug.Log ("AssetDownloader: Asset already downloaded.");
    } else { 
      Debug.Log ("AssetDownloader: Asset already queued.");
    }

    return result;
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
