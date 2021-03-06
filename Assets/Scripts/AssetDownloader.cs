﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityFx.Async;
using UnityFx.Async.Promises;

// For promises-specific stuff.
using UnityEngine.Networking;
using System;
using System.IO;

public class AssetDownloader : MonoBehaviour {
  private Queue<AssetContainer> downloadQueue;
  private AssetContainer mContainer;
  private bool isReadyToDownload = true;
  private bool isFlaggedPriority = false;
  private AssetContainer priorityContainer;
  private Action<AssetContainer> priorityCallback;

  public Action<float, string> progressChangedCallback;

  private void Start() {
    downloadQueue = new Queue<AssetContainer>();
  }

  private void Update() {
    if (isReadyToDownload) {
      initiateNextDownload();
    }
  }

  //  Summary: The go-to function when requesting any asset to be downloaded
  //    due to its conditional pre-checks.   
  public void enqueueAssetToDownload(ref AssetContainer container) {
    if (shouldEnqueue(ref container)) {
      downloadQueue.Enqueue(container);
      Debug.Log("AssetDownloader: Enqueued Asset of ID : " + container.AssignedAssetFiledName);
    }
  }

  //  Summary: This function sets the condition for the requested container to be
  //    downloaded after the completed download of the current asset in order to
  //    centralize all downloaded through the initiateNextDownload method. 
  public void priorityEnqueueVideo(ref AssetContainer container, Action<AssetContainer> completionCallback) {
    priorityContainer = container;
    priorityCallback = completionCallback;
    isFlaggedPriority = true;
    Debug.Log("AssetDownloader/PriorityEnqueueVideo: Set flag, container, and callback");
  }

  //  Summary: Helper function that initiates the subsequent file download, which
  //    includes both priority and normal enqueue. 
  private void initiateNextDownload() {
    if (isFlaggedPriority) {
      isReadyToDownload = false;
      isFlaggedPriority = false;
      mContainer = priorityContainer;
      Debug.Log("AssetDownloader/InitiateNextDownload/Priority: Initiating download for " + mContainer.AssignedAssetFiledName);

      IAsyncOperation<AssetContainer> asyncOp = downloadVideoAsync();

      asyncOp.Completed += ( sender, args ) => {
        priorityCallback(mContainer);
        priorityContainer = null;
        isReadyToDownload = true;
      };

      configuredAsyncOperationProgressChangedCallback(asyncOp);
    } else if (downloadQueue.Count > 0) {

      isReadyToDownload = false;
      mContainer = downloadQueue.Dequeue();
      Debug.Log("AssetDownloader/InitiateNextDownload/Normal: Initiating download for " + mContainer.AssignedAssetFiledName);

      IAsyncOperation<AssetContainer> asyncOp = downloadVideoAsync();

      asyncOp.Completed += ( sender, args )  => {
        isReadyToDownload = true;
      };

      configuredAsyncOperationProgressChangedCallback(asyncOp);
    }
  }

  //  Summary: Helper method to avoid code duplication for ProgressChanged callback for an AsyncOperation
  private void configuredAsyncOperationProgressChangedCallback(IAsyncOperation<AssetContainer> container) {
      container.ProgressChanged += ( sender, args ) => {
        if ( progressChangedCallback != null ) {
          progressChangedCallback(args.ProgressPercentage, mContainer.AssignedAssetFiledName);
        } else {
          Debug.Log("AssetDownloader: ProgressChangedCallback is null");
        }
      };
  }

  // Summary: Publicly exposed method that checks whether the asset is pending download.
  public bool isAssetQueuedForDownload( ref AssetContainer container ) {
    return !shouldEnqueue(ref container);
  }

  //  Summary: Helper function that checks for file existence
  //    in local memory and in the download queue. 
  private bool shouldEnqueue(ref AssetContainer container) {
    bool result = false;
    if (mContainer == container) {
      // Note: We only care if the container and mContainer objects both refer to the same object.
      Debug.Log("AssetDownloader: Asset downloading.");
    } else if (downloadQueue.Contains(container)) {
      Debug.Log("AssetDownloader: Asset already queued.");
    } else if (mContainer.doesFileExistLocally()) {
      Debug.Log("AssetDownloader: Asset already downloaded.");
    } else if (!downloadQueue.Contains(container)) {
      // The above condition is a sanity check to avoid any unexpected behaviour
      //  from a broad, catch-all 'else' statement. 
      result = true;
    }

    return result;
  }

  //  Summary: A wrapper function that handles pre-download checks and initiates the 
  //    Download coroutine, the use of which is essential for download progress tracking.
  private IAsyncOperation<AssetContainer> downloadVideoAsync() {
    var result = new AsyncCompletionSource<AssetContainer>(AsyncOperationStatus.Running);

    if (mContainer.doesFileExistLocally()) {
      Debug.Log("AssetDownloader: File already exists.");
      result.SetCanceled();
    } else {
      Debug.Log("Coroutine: Start DownloadAsyncVideoData");
      AsyncUtility.StartCoroutine(downloadVideoInternal(result, mContainer.AssetHttpEndpoint));
    }

    return result.Operation;
  }

  //  Summary: The UnityWebRequest is executed through a Coroutine in order to capture
  //    progress data to report through the progressChangedCallback Action.
  private IEnumerator downloadVideoInternal(IAsyncCompletionSource<AssetContainer> op, string url) {
    Debug.Log("Coroutine/Promise: Request for Video Data");

    var www = UnityWebRequest.Get(url);
    var result = www.SendWebRequest();

    while (!result.isDone) {
      op.TrySetProgress(result.progress);
      yield return null;
    }

    if (www.isNetworkError || www.isHttpError) {
      Debug.Log("Coroutine/Promise: Request failed");
      op.SetException(new Exception(www.error));
    } else {
      Debug.Log("Coroutine/Promise: Request succeeded");
      handleVideoByteBlob(www.downloadHandler.data);
      op.SetResult(mContainer);
    }
  }

  //  Summary: Takes a byte[] of the video downloaded and writes it into memory
  //    using the filename and extension provided by mContainer.
  private void handleVideoByteBlob(byte[] data) {
    Debug.Log("Coroutine/Promise/Handler: Begin writing data to file");
    byte[] _videoBytes = data;

    string _pathToFile = Path.Combine(Application.persistentDataPath, mContainer.AssignedAssetFiledName + mContainer.AssetFileType);

    mContainer.AssetLocalFilePath = _pathToFile;

    File.WriteAllBytes(_pathToFile, _videoBytes);
    Debug.Log("Coroutine/Promise/Handler: File stored at " + _pathToFile);
  }
}