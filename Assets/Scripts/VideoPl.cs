using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using UnityFx.Async.Promises;

public class VideoPl : MonoBehaviour
{
	public AssetDownloader downloader;

	private void Start ()
	{
		downloadData ();
		//StartCoroutine (this.loadVideoFromThisURL (videoUrl));
	}

	[SerializeField]
	internal UnityEngine.Video.VideoPlayer myVideoPlayer;
	string videoUrl = "https://bitbucket.org/jhlink/story_static_host/raw/9353308e7c994be2ed6187b78125a906d60c824b/Videos/ch0_intro/intro_p1.mp4";

	private void downloadData ()
	{
		string fileName = "intro.mp4";
		AssetContainer container = new AssetContainer (videoUrl, fileName);
		downloader.DownloadVideoAsync (container)
			.Then (resultContainer => {
			Debug.Log (resultContainer.AssetLocalFilePath);
		})
			.Catch<System.OperationCanceledException> (e => {
			Debug.Log (e);
		})	
			.Catch (e => {
			Debug.LogException (e);
		});	
	}

	private IEnumerator loadVideoFromThisURL (string _url)
	{
		UnityWebRequest _videoRequest = UnityWebRequest.Get (_url);

		yield return _videoRequest.SendWebRequest ();

		if (_videoRequest.isDone == false || _videoRequest.error != null) {
			Debug.Log ("Request = " + _videoRequest.error);
		}

		Debug.Log ("Video Done - " + _videoRequest.isDone);

		byte[] _videoBytes = _videoRequest.downloadHandler.data;

		string _pathToFile = Path.Combine (Application.persistentDataPath, "movie.mp4");
		File.WriteAllBytes (_pathToFile, _videoBytes);
		Debug.Log (_pathToFile);
		StartCoroutine (this.playThisURLInVideo (_pathToFile));
		yield return null;
	}

	private void invertNormals (Mesh mesh)
	{
		// Calculate normals
		Vector3[] normals = mesh.normals;
		for (int i = 0; i < normals.Length; i++)
			normals [i] = -normals [i];
		mesh.normals = normals;

		// Create triangles
		for (int m = 0; m < mesh.subMeshCount; m++) {
			int[] triangles = mesh.GetTriangles (m);
			for (int i = 0; i < triangles.Length; i += 3) {
				int temp = triangles [i + 0];
				triangles [i + 0] = triangles [i + 1];
				triangles [i + 1] = temp;
			}
			mesh.SetTriangles (triangles, m);
		}
	}

	private IEnumerator playThisURLInVideo (string _url)
	{
		myVideoPlayer.source = UnityEngine.Video.VideoSource.Url;
		myVideoPlayer.url = _url;
		myVideoPlayer.Prepare ();

		var mesh = this.gameObject.GetComponent<MeshFilter> ().sharedMesh;
		invertNormals (mesh);
		SetColor (this.gameObject.GetComponent<Renderer> ());

		while (myVideoPlayer.isPrepared == false) {
			yield return null;
		}

		Debug.Log ("Video should play");
		myVideoPlayer.Play ();
	}

	//	void CreateHostObject() {
	//		Host = GameObject.CreatePrimitive(PrimitiveType.Sphere);
	//		Host.layer = Config.playerLayer;
	//		Host.name = "UniVRMediaObject";
	//		Host.transform.localScale = new Vector3(-100F, 100F, 100F);
	//		Host.transform.position = Config.position;
	//	}
	//
	//	void ManageComponents() {
	//		var collider = Host.GetComponent<Collider>();
	//		if (collider != null) UnityEngine.Object.Destroy(collider);
	//
	//		Video = Host.AddComponent<VideoPlayer>();
	//		Audio = Host.AddComponent<AudioSource>();
	//
	//		Video.audioOutputMode = VideoAudioOutputMode.AudioSource;
	//		Video.SetTargetAudioSource(0, Audio);
	//	}
	//
	//	void InvertNormals() {
	//		var filter = Host.GetComponent<MeshFilter>();
	//		filter.mesh.InvertNormals();
	//	}
	//
	void SetColor (Renderer renderer)
	{
		Texture2D tex = new Texture2D (1, 1);
		tex.SetPixel (0, 0, Color.clear);
		tex.Apply ();

		renderer.material.shader = Shader.Find ("Unlit/Texture");
		renderer.material.mainTexture = tex;
	}
}