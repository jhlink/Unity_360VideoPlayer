# Description

v0.0.6

This project attempts to create a basic implementation for a dedicated 360 video player in Unity.  

The inspiration behind this project came after discovering that Unity's native video player does not handle asynchronous video downloads nor does it possess preset configurations for 360 video. 

This Unity package attempts to create a simple, variant 360 Video Player that achieves the follow features:
- Non-UI blocking, asynchronous downloads of video assets from URLs via Promises. 
- "Normal Vector" inversion for view inside a primitive GameObject sphere.

This library is primarily developed for mobile platforms. This has been tested on the Android, but untested on iOS devices. As a result, resource utilization / asset downloading are executed based on need. 

## Software Architecture Diagram 

The project encapsulates different functions of the variant 360 video player into different classes explained in the following. 

- MasterPlayerController (MPC): The developer will load, play, or stop the video in this class via publicly exposed methods ( see class for more details ).
- AssetDownloader (AD): When a video does not exist on the local device, the MPC delegates the asynchronous download of the file
  to the AssetDownloader class. Using UnityFx, Promises are utilized instead of Coroutines for performance and error handling.
 - **Note** Downloaded files are stored on the [Application.persistentDataPath](https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html). Application.streamingAssetsPath was not used because the directory is readOnly at runtime.
- VideoCollectionManager (VCM): The VideoCollectionManager largely manages the collection of video URLs and their respective ID, or video name. 

  [IMPORTANT] -- This is the hackiest aspect of the library. The VideoURL and VideoID lists must be equal in length, otherwise an assert with the class
  will raise an AssertionException. This VideoCollectionManager should really be replaced with AssetBundles, but that's a feature for another day. :) 
  One can assume that order will be strictly adhered, such that videoURL[0] will refer to videoID[0]. 

  - **Note** When the Unity receives a lowMemory alert from a given platform, this signals within the VideoCollectionManager a callback that purges all downloaded video assets. 

  The VCM will provide the MPC a reference to the AssetContainer, which possesses the following member variables:
   - AssetHttpEndpoint
   - AssetFileType
   - AssignedAssetFileName
   - AssetLocalFilePath

  - PlayerConfigurator: This class configures a vanilla Unity video player for 360 video playback and audio support. Additionally, inverting the normal vectors and flipping the mirrored video is handled here.  


              PUBLICLY            +         PRIVATE
            ACCESSIBLE API        |
                                  |
                                  |      +---------------+
                                  |      |  Asset        |
                   +-------------------->|   Downloader  |
                   |              |      +---------------+
                   |              |
                   |              |
           +-------v------+       |      +---------------+
           |              |       |      |  Video        |    +--------------+
           | MasterPlayer |<------------>|   Collection  |<-->|  Asset       |
        -->|  Controller  |       |      |    Manager    |    |   Container  |
           |              |       |      +---------------+    +--------------+
           +-------^------+       |
                   |              |
                   |              |      +----------------+
                   +-------------------->|  Player        |
                                  |      |   Configurator |
                                  |      +----------------+
                                  |
                                  |
                                  |
                                  |
                                  +


## Instructions
Within the imported Unity package, there exists a VideoSphere prefab, which is already pre-configured to download and handle playback for 360 video content. 

Click and drag the prefab to your scene, and within the VideoCollectionManager script populate the videoID and videoURL arrays with the video file name and the URL to the asset, respectively. 

- **Note** By default, the library assumes that the video is in .mp4 format and assumes that the video file is equirectangular. 

Any given video may be played by calling the following public function provided by the MasterPlayerController class, where videoFileName is the video ID of the asset you wish to play. 

`MasterPlayerController.playVideo(string videoFileName)`

Videos can be stopped, paused, and resumed using the following commands that are exposed in the MasterPlayerController class.

```
MasterPlayerController.stopVideo()
MasterPlayerController.pauseVideo()
MasterPlayerController.resumeVideo()
```

While other videos are playing or actions are occurring, other video assets can be downloaded in the background by calling the following.

`MasterPlayerController.queueVideoDownload(string videoFileName)`

Note that populating the VideoCollectionManager with more than one asset does not mean that they will be downloaded immediately. It is the developer's responsibility to explicitly enqueue the video using the functions exposed by the MasterPlayerController class.

- **Note** Be sure to disable the 'Play Demo' flag within the Master Player Controller script. 

The above functions only work for videos that are defined in the VideoCollectionManager script.

### Progress Tracking
Glorious progress tracking has been implemented via the UnityFx.Async library. The AssetDownloader will attempt to process any queued assets as quickly as possible, so an assetFileName has been provided to identify the asset with the associated progressValue.

This handler is called from AssetDownloader whenever the progressValue changes for a given download. 
Therefore, it is encouraged to implement any progress updating functionationality within this function.  

`handleDownloadProgress(float progressValue, string assetFileName)`

Various file check functions have also been provided in case the asset has already been downloaded and the reported progressValue is not associated with the desired asset.    

```
MasterPlayerController.isAssetDownloaded(string videoFileName)
MasterPlayerController.isAssetInDownloadQueue(string videoFileName)
```

### 360 Video Player Demo Scene

For developers familiar with Unity, this scene is a minimalist demo of the capabilities of a VideoSphere. 

Given that the 'Play Demo' flag is set on the MasterPlayerController script within the VideoSphere GameObject, playing the scene should result with a video played immediately. 

### CardboardDemoScene

This demo incorporates the Google VR SDK (v1.17.0) to demonstrate playing and stopping a 360 video player using the same function calls specified in the Instruction section above. 

## Troubleshooting
- When building and installing the project on an Android device, the environment is pink. 

This means that the default "Unlit/Texture" is missing in the build file. 
Go to Edit -> ProjectSettings -> Graphics -> Built-In Shader Settings.
Ensure within the 'Always Included Shader' array that an element containing the "Unlit/Texture" exists. 

## Credits 
- Kudos to arvtesh for this awesome Asynchronous operations for Unity library. ( UnityFx.Async on the Asset Store ) 
- Kudos to karsnen for his [StackOverflow response]( https://stackoverflow.com/questions/45875240/unable-to-play-video-clip-downloaded-from-url-using-videoplayer-in-unity ) for asynchronously downloading video files. 
- Kudos to adrenak for his initial work on creating a 360 Video Player in Unity, found [here](https://github.com/adrenak/UniVRMedia).  
- Kudos to Joachim Ante for his work on ReverseNormals.cs found [here](http://wiki.unity3d.com/index.php/ReverseNormals).
- Sample video used: [Visit the Phillippines](https://www.youtube.com/watch?v=vQt2NRT5yP4)
