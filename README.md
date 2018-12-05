# Description

This project attempts to create a basic implementation for a dedicated 360 video player in Unity.  

The inspiration behind this project came after discovering that Unity's native video player does not handle asynchronous video downloads nor does it possess preset configurations for 360 video. 

This Unity package attempts to create a simple, variant 360 Video Player that achieves the follow features:
- Asynchronously downloads video assets from URLs via Promises. 
  - Note: Parallelized downloads have not been tested. 
- Applies "Normal Vector" inversion for view inside a primitive GameObject sphere.

## Software Architecture Diagram 

## Instructions
### 


## Credits 
- Kudos to arvtesh for this awesome Asynchronous operations for Unity library. ( UnityFx.Async on the Asset Store ) 
- Kudos to karsnen for his [StackOverflow response]( https://stackoverflow.com/questions/45875240/unable-to-play-video-clip-downloaded-from-url-using-videoplayer-in-unity ) for asynchronously downloading video files. 
- Kudos to adrenak for his initial work on creating a 360 Video Player in Unity, found [here](https://github.com/adrenak/UniVRMedia).  
- Kudos to Joachim Ante for his work on ReverseNormals.cs found [here](http://wiki.unity3d.com/index.php/ReverseNormals).
- Sample video used: [Visit the Phillippines](https://www.youtube.com/watch?v=vQt2NRT5yP4)