﻿using UnityEngine.Assertions;
using System.Collections.Generic;
using UnityEngine;

public class VideoCollectionManager : MonoBehaviour
{
  public string[] videoID;
  public string[] videoUrls;
  private bool debugMode = false;
  private Dictionary<string, AssetContainer> videoStringMap = new Dictionary<string, AssetContainer>();

  private void Start() {
    assertVideoArrayConditions();
    initializeDictionary();
    Application.lowMemory += onLowMemoryWarning;
  }

  private void assertVideoArrayConditions() {
    Assert.raiseExceptions = true;
    Assert.AreEqual(videoUrls.Length, videoID.Length);
  }

  private void onLowMemoryWarning() {
    Debug.Log("Manager: Low Memory Warning Received -> Purging all video assets");
    foreach (var item in videoStringMap.Values)
    {
      item.deleteFileFromSystem();
    }
  }

  public void initialize() {
    Debug.Log("Initializing");
    initializeDictionary();
    Debug.Log("Completed initialization");
  }

  public void initializeDictionary() {
    assertVideoArrayConditions ();
    if (videoStringMap.Count > 0) {
      return;
    }

    if (videoID.Length == videoUrls.Length) {
      for (int i = 0; i < videoID.Length; i++) {
        string videoFileName = videoID[ i ];
        string videoUrlItem = videoUrls[ i ];

        AssetContainer assetContainer = new AssetContainer(videoUrlItem, videoFileName);
        videoStringMap.Add (videoFileName, assetContainer);

        if ( debugMode ) {
          Debug.Log("Manager: " + videoFileName + " at URL " + videoStringMap[videoFileName] );
        }
      }
    }
  }

  public AssetContainer getContainerWithKey(string key) {
    AssetContainer videoContainer = new AssetContainer();
    if ( videoStringMap.TryGetValue(key, out videoContainer) ) {
      if ( debugMode ) {
        Debug.Log("Manager: Found " + key + " at Container:\n" + videoContainer.debugString());
      }
    } else {
      if ( debugMode ) {
        Debug.Log("Manager: Value for " + key + " not found");
      }
    }
    return videoContainer;
  }
}