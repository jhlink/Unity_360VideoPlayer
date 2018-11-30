using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Networking;
using System.IO;

public class PlayerConfigurator
{

  private AudioSource source; 
  private VideoPlayer videoPlayer;

  public void playVideo(GameObject playerContainer, string _url)
  {
    extractPlayerComponents(playerContainer);
    configureGameObjectFor360Content(playerContainer);

    videoPlayer.prepareCompleted += prepareCompleted;
    videoPlayer.source = UnityEngine.Video.VideoSource.Url;
    videoPlayer.audioOutputMode = UnityEngine.Video.VideoAudioOutputMode.AudioSource;
    videoPlayer.controlledAudioTrackCount = 1;

    videoPlayer.waitForFirstFrame = true;
    videoPlayer.playOnAwake = false;
    source.playOnAwake = false;

    videoPlayer.EnableAudioTrack(0, true);
    videoPlayer.SetTargetAudioSource(0, source);
    source.volume = 1.0f;

    videoPlayer.url = _url;
    videoPlayer.Prepare();
  }

//  Summary: Configures the GameObject for displaying 360 video content appropriately
//    via invertNormals and applying appropriate unlit texture via setUnlitTexture.
  private void configureGameObjectFor360Content(GameObject mPlayerContainer) {
    // Note: Mesh is used as opposed to sharedMesh for the following reasons.
    //  - Instantiates a new mesh based on object and provides a reference to this
    //    instantiation and future .mesh property calls. 
    //    -- SharedMesh affects all objects that use mesh reference, which can be
    //    computationally expensive on mobile platforms. 
    //    -- Basic idea: Modify a copy and apply changes only to objects that need it. 
    //  Information concerning Mesh vs. Shared mesh can found here. (Kudos to Gambit-MSplitz) 
    //    https://forum.unity.com/threads/video-player-is-not-playing-audio.486924/#post-3532904
    var mesh = mPlayerContainer.GetComponent<MeshFilter>().mesh;
    invertNormals(mesh);
    setUnlitTexture(mPlayerContainer.GetComponent<Renderer>());
  }

// Summary: Helper method that extract AudioSource and VideoPlayer
//  GameObjects to be configured for 360 video playback.
  private void extractPlayerComponents(GameObject playerContainer) {
    source = playerContainer.GetComponent<AudioSource>();
    videoPlayer = playerContainer.GetComponent<VideoPlayer>();
  }

// Summary: Event handler called when video player has finished
//  preparing resources to play content.
  private void prepareCompleted(VideoPlayer vp ) {
    vp.Play();
    Debug.Log("Video should play");
  }

// Summary: Ensure that project video on Sphere ( GameObject ) 
//  ignores any in-game lighting effects using Unlit/Texture as base.
  void setUnlitTexture(Renderer renderer)
  {
    Texture2D tex = new Texture2D(1, 1);
    tex.SetPixel(0, 0, Color.clear);
    tex.Apply();

    renderer.material.shader = Shader.Find("Unlit/Texture");
    renderer.material.mainTexture = tex;
  }

// Summary: Invert normals of mesh within Sphere ( GameObject ) 
//  to display video on inner geometric surface.
  private void invertNormals(Mesh mesh)
  {
        // Based on ReverseNormals.cs from Joachim Ante here : http://wiki.unity3d.com/index.php/ReverseNormals
    // Calculate normals
    Vector3[] normals = mesh.normals;
    for (int i = 0; i < normals.Length; i++)
      normals[i] = -normals[i];
    mesh.normals = normals;

    // Create triangles
    for (int m = 0; m < mesh.subMeshCount; m++)
    {
      int[] triangles = mesh.GetTriangles(m);
      for (int i = 0; i < triangles.Length; i += 3)
      {
        int temp = triangles[i + 0];
        triangles[i + 0] = triangles[i + 1];
        triangles[i + 1] = temp;
      }
      mesh.SetTriangles(triangles, m);
    }
  }
}

