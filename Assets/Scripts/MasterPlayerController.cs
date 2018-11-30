using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using UnityFx.Async.Promises;

public class MasterPlayerController : MonoBehaviour
{
	public AssetDownloader downloader;
	public VideoCollectionManager manager;

	private AssetContainer resultContainer;

	private void Start ()
	{
		manager.initialize();
		initialize();
	}

	[SerializeField]
	internal UnityEngine.Video.VideoPlayer myVideoPlayer;

	public void initializePlayerForVideo(string videoName) {
		resultContainer = manager.getContainerWithKey(videoName);

		downloader.DownloadVideoAsync (resultContainer)
			.Then (resultContainer => {
			Debug.Log (resultContainer.AssetLocalFilePath);
			PlayerConfigurator playerConfigurator = new PlayerConfigurator();
			playerConfigurator.playVideo(this.gameObject, resultContainer.AssetLocalFilePath);
		})
			.Catch<System.OperationCanceledException> (e => {
			Debug.Log (e);
			PlayerConfigurator playerConfigurator = new PlayerConfigurator();
			playerConfigurator.playVideo(this.gameObject, resultContainer.AssetLocalFilePath );
		})	
			.Catch (e => {
			Debug.LogException (e);
		});	
	}

	private void initialize() {
		this.gameObject.AddComponent<AudioSource>();
	}

	private void playVideo() {

	}

}
