using UnityEngine.Assertions;
using System.Collections.Generic;
using UnityEngine;

public class VideoCollectionManager : MonoBehaviour
{
	public string[] videoID;
	public string[] videoUrls;

	private Dictionary<string, string> videoStringMap = new Dictionary<string, string>();

	private void Start() {
		assertVideoArrayConditions();
		initializeDictionary();
	}

	private void assertVideoArrayConditions() {
		Assert.raiseExceptions = true;
		Assert.AreEqual(videoUrls.Length, videoID.Length);
	}

	public void initializeDictionary() {
		assertVideoArrayConditions ();
		if (videoStringMap.Count > 0) {
			return;
		}

		if (videoID.Length == videoUrls.Length) {
			for (int i = 0; i < videoID.Length; i++) {
				string videoIdItem = videoID[ i ];
				string videoUrlItem = videoUrls[ i ];

				videoStringMap.Add (videoIdItem, videoUrlItem);

				Debug.Log("Manager: " + videoIdItem + " at URL " + videoStringMap[videoIdItem] );
			}
		}
	}

	public string getUrlWithKey(string key) {
		string urlStringHolder = "";
		if ( videoStringMap.TryGetValue(key, out urlStringHolder) ) {
			Debug.Log("Manager: Found " + key + " at URL " + urlStringHolder);
		} else {
			Debug.Log("Manager: Value for " + key + " not found");
		}
		return urlStringHolder;
	}
}