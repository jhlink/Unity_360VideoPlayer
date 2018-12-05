# Description

This project attempts to create a basic implementation for a dedicated 360 video player in Unity.  

The inspiration behind this project came after discovering that Unity's native video player does not handle asynchronous video downloads nor does it possess preset configurations for 360 video. 

This Unity package attempts to create a simple, variant 360 Video Player that achieves the follow features:
- Non-UI blocking, asynchronous downloads of video assets from URLs via Promises. 
  - Note: Parallelized downloads have not been tested. 
- "Normal Vector" inversion for view inside a primitive GameObject sphere.

## Software Architecture Diagram 

The project encapsulates different functions of the variant 360 video player into different classes explained in the following. 

- MasterPlayerController (MPC): The developer will load, play, or stop the video in this class via publicly exposed methods ( see class for more details ).
- AssetDownloader (AD): When a video does not exist on the local device, the MPC delegates the asynchronous download of the file
  to the AssetDownloader class. Using UnityFx, Promises are utilized instead of Coroutines for performance and error handling.
 - [Note] Downloaded files are stored on the [Application.persistentDataPath](https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html). Application.streamingAssetsPath was not used because the directory is readOnly at runtime.
- VideoCollectionManager (VCM): The VideoCollectionManager largely manages the collection of video URLs and their respective ID, or video name. 

  [IMPORTANT] -- This is the hackiest aspect of the library. The VideoURL and VideoID lists must be equal in length, otherwise an assert with the class
  will raise an AssertionException. This VideoCollectionManager should really be replaced with AssetBundles, but that's a feature for another day. :) 
  One can assume that order will be strictly adhered, such that videoURL[0] will refer to videoID[0]. 

  - [Note] When the Unity receives a lowMemory alert from a given platform, this signals within the VideoCollectionManager a callback that purges all downloaded video assets. 

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
     ----->|  Controller  |       |      |    Manager    |    |   Container  |
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

[Note] By default, the library assumes that the video is in .mp4 format and assumes that the video file is equirectangular. 

Any given video may be played by calling the following public function provided by the MasterPlayerController class, where videoFileName is the video ID of the asset you wish to play. 

`MasterPlayerController.playVideo(string videoFileName)`

While other videos are playing or actions are occurring, other video assets can be downloaded in the background by calling the following.

`MasterPlayerController.loadVideo(string videoFileName)`

- [Note] Be sure to disable the 'Play Demo' flag within the Master Player Controller script. 

The above functions only work for videos that are defined in the VideoCollectionManager script.

### 360 Video Player Demo Scene

### CardboardDemoScene




## Credits 
- Kudos to arvtesh for this awesome Asynchronous operations for Unity library. ( UnityFx.Async on the Asset Store ) 
- Kudos to karsnen for his [StackOverflow response]( https://stackoverflow.com/questions/45875240/unable-to-play-video-clip-downloaded-from-url-using-videoplayer-in-unity ) for asynchronously downloading video files. 
- Kudos to adrenak for his initial work on creating a 360 Video Player in Unity, found [here](https://github.com/adrenak/UniVRMedia).  
- Kudos to Joachim Ante for his work on ReverseNormals.cs found [here](http://wiki.unity3d.com/index.php/ReverseNormals).
- Sample video used: [Visit the Phillippines](https://www.youtube.com/watch?v=vQt2NRT5yP4)