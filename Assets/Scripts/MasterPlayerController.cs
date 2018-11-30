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


	private void Start ()
	{
		downloadData ();
	}

	[SerializeField]
	internal UnityEngine.Video.VideoPlayer myVideoPlayer;

	private void downloadData ()
	{
		manager.initialize(false);
		string videoKey = "intro_p1.mp4";
		string videoUrl = manager.getUrlWithKey(videoKey);

		AudioSource vAudio = this.gameObject.AddComponent<AudioSource>();

		AssetContainer container = new AssetContainer (videoUrl, videoKey);
		downloader.DownloadVideoAsync (container)
			.Then (resultContainer => {
			Debug.Log (resultContainer.AssetLocalFilePath);
			PlayerConfigurator playerConfigurator = new PlayerConfigurator();
			playerConfigurator.playVideo(this.gameObject, resultContainer.AssetLocalFilePath);
		})
			.Catch<System.OperationCanceledException> (e => {
			Debug.Log (e);
			PlayerConfigurator playerConfigurator = new PlayerConfigurator();
			playerConfigurator.playVideo(this.gameObject, container.AssetLocalFilePath );
		})	
			.Catch (e => {
			Debug.LogException (e);
		});	
	}

	private void playVideo() {

	}

}
