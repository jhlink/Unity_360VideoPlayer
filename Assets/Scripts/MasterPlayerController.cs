using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using UnityFx.Async.Promises;
using UnityEngine.Video;

public class MasterPlayerController : MonoBehaviour
{

  public bool playDemo = false;
  public AssetDownloader downloader;
  public VideoCollectionManager manager;

  private AssetContainer resultContainer;
  private PlayerConfigurator playerConfigurator;

  private void Start ()
  {
    initializeComponents();
    manager.initialize();

    if ( playDemo ) {
      playVideo("sample");
    }
  }

  [SerializeField]
  internal VideoPlayer myVideoPlayer;

  public void loadVideo(string videoName) {
    resultContainer = manager.getContainerWithKey(videoName);

    downloader.DownloadVideoAsync (resultContainer)
      .Then ( mContainer => {
      resultContainer = mContainer;
      Debug.Log (resultContainer.AssetLocalFilePath);
    })
      .Catch<System.OperationCanceledException> (e => {
      Debug.Log (e);
      Debug.Log (resultContainer.AssetLocalFilePath);
    })	
      .Catch (e => {
      Debug.LogException (e);
    });	
  }

  public void playVideo(string videoName) {
    resultContainer = manager.getContainerWithKey(videoName);

    downloader.DownloadVideoAsync (resultContainer)
      .Then ( mContainer => {
      resultContainer = mContainer;
      startVideoPlayer();
    })
      .Catch<System.OperationCanceledException> (e => {
      startVideoPlayer();
    })	
      .Catch (e => {
      Debug.LogException (e);
    });	
  }

  public void stopVideo() {
    playerConfigurator.stopVideo(this.gameObject);
  }

  private void startVideoPlayer() {
    Debug.Log("Playing video : " + resultContainer.AssignedAssetFiledName);
    playerConfigurator.playVideo(this.gameObject, resultContainer.AssetLocalFilePath);
  }

  private void initializeComponents() {
    this.gameObject.AddComponent<AudioSource>();
    playerConfigurator = new PlayerConfigurator();
    if ( myVideoPlayer == null) {
      myVideoPlayer = this.gameObject.AddComponent<VideoPlayer>();
    }
    playerConfigurator.configureVideoPlayer(this.gameObject);
    downloader.progressChangedCallback += handleDownloadProgress;
  }

  private void handleDownloadProgress( float progressPercentage, string assetFileName ) {
    Debug.Log(assetFileName + " progress percentage " + progressPercentage.ToString()) ;
  }
}
