using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using UnityFx.Async.Promises;
using UnityEngine.Video;

public class MasterPlayerController : MonoBehaviour
{
	public AssetDownloader downloader;
	public VideoCollectionManager manager;

	private AssetContainer resultContainer;
	private PlayerConfigurator playerConfigurator;

	private void Start ()
	{
		manager.initialize();
		initializeComponents();
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

	public void playVideo() {
		Debug.Log("Playing video : " + resultContainer.AssignedAssetFiledName);
		playerConfigurator.playVideo(this.gameObject, resultContainer.AssetLocalFilePath);
	}

	private void initializeComponents() {
		this.gameObject.AddComponent<AudioSource>();
		playerConfigurator = new PlayerConfigurator();
		if ( myVideoPlayer == null) {
			myVideoPlayer = this.gameObject.AddComponent<VideoPlayer>();
		}
	}


}
