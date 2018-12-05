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
### 


## Credits 
- Kudos to arvtesh for this awesome Asynchronous operations for Unity library. ( UnityFx.Async on the Asset Store ) 
- Kudos to karsnen for his [StackOverflow response]( https://stackoverflow.com/questions/45875240/unable-to-play-video-clip-downloaded-from-url-using-videoplayer-in-unity ) for asynchronously downloading video files. 
- Kudos to adrenak for his initial work on creating a 360 Video Player in Unity, found [here](https://github.com/adrenak/UniVRMedia).  
- Kudos to Joachim Ante for his work on ReverseNormals.cs found [here](http://wiki.unity3d.com/index.php/ReverseNormals).
- Sample video used: [Visit the Phillippines](https://www.youtube.com/watch?v=vQt2NRT5yP4)