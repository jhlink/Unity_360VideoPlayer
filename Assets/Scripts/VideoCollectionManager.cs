using UnityEngine.Assertions;
using System.Collections.Generic;
using UnityEngine;

public class VideoCollectionManager : MonoBehaviour
{
	public string[] videoID;
	public string[] videoUrls;

	private Dictionary<string, string> videoStringMap;

	private void Start() {
		assertVideoArrayConditions ();
	}

	private void assertVideoArrayConditions() {
		Assert.raiseExceptions = true;
		Assert.AreEqual(videoUrls.Length, videoID.Length);
	}

	private void initializeDictionary() {
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
}

