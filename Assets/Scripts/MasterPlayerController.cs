using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using UnityFx.Async.Promises;
using UnityEngine.Video;

public class MasterPlayerController : MonoBehaviour {

  public bool playDemo = false;
  public AssetDownloader downloader;
  public VideoCollectionManager manager;

  private PlayerConfigurator playerConfigurator;

  private void Start() {
    initializeComponents();
    manager.initialize();

    if (playDemo) {
      playVideo("sample");
    }
  }

  [SerializeField]
  internal VideoPlayer myVideoPlayer;

  public void queueVideoDownload(string videoName) {
    AssetContainer resultContainer = manager.getContainerWithKey(videoName);

    downloader.enqueueAssetToDownload(ref resultContainer);
  }

  public bool isAssetDownloaded(string videoName) {
    AssetContainer resultContainer = manager.getContainerWithKey(videoName);
    return resultContainer.doesFileExistLocally(); 
  }

  public bool isAssetInDownloadQueue(string videoName) {
    AssetContainer resultContainer = manager.getContainerWithKey(videoName);
    return downloader.isAssetQueuedForDownload(ref resultContainer);
  }

  public void playVideo(string videoName) {
    AssetContainer resultContainer = manager.getContainerWithKey(videoName);

    if (isAssetDownloaded(videoName)) {
      startVideoPlayer(resultContainer);
    } else {
      Debug.Log("MasterPlayerController: Asset does not exist locally -> Initiating OnTheFly Download");
      downloader.priorityEnqueueVideo(ref resultContainer, startVideoPlayer);
    }
  }

  public void stopVideo() {
    playerConfigurator.stopVideo(this.gameObject);
  }

  private void startVideoPlayer(AssetContainer resultContainer) {
    Debug.Log("Playing video : " + resultContainer.AssignedAssetFiledName);
    playerConfigurator.playVideo(this.gameObject, resultContainer.AssetLocalFilePath);
  }

  private void initializeComponents() {
    this.gameObject.AddComponent<AudioSource>();
    playerConfigurator = new PlayerConfigurator();
    if (myVideoPlayer == null) {
      myVideoPlayer = this.gameObject.AddComponent<VideoPlayer>();
    }
    playerConfigurator.configureVideoPlayer(this.gameObject);
    downloader.progressChangedCallback += handleDownloadProgress;
  }

  // Summary: A progress handler that reports the download progress of current asset
  //  in queue. ProgressPercentage is an int value starting and ending at 0 and 100,
  //  respectively. AssetDownloader will try to process all assets in the queue,
  //  so the assetFileName is provided to identify the asset being downloaded.   
  //  The developer is encouraged to modify the logic of this function to handle
  //  functionality as needed. Note: The function header must remain unchanged. 
  private void handleDownloadProgress(float progressValue, string assetFileName) {
    Debug.Log(assetFileName + " progress percentage " + progressValue.ToString());
  }
}
