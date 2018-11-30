using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Networking;
using System.IO;

public class PlayerConfigurator
{

  private AudioSource source; 

  public void playVideo(VideoPlayer videoPlayer, AudioSource vAudio, GameObject playerContainer, string _url)
  {
    var mesh = playerContainer.GetComponent<MeshFilter>().mesh;
    invertNormals(mesh);
    SetColor(playerContainer.GetComponent<Renderer>());
    videoPlayer.prepareCompleted += prepareCompleted;

    videoPlayer.source = UnityEngine.Video.VideoSource.Url;
    videoPlayer.audioOutputMode = UnityEngine.Video.VideoAudioOutputMode.AudioSource;
    videoPlayer.controlledAudioTrackCount = 1;

    videoPlayer.waitForFirstFrame = true;
    videoPlayer.playOnAwake = false;
    vAudio.playOnAwake = false;

    videoPlayer.EnableAudioTrack(0, true);
    videoPlayer.SetTargetAudioSource(0, vAudio);
    vAudio.volume = 1.0f;

    videoPlayer.url = _url;

    videoPlayer.Prepare();
  }

  private void prepareCompleted(VideoPlayer vp ) {
    vp.Play();
    if ( vp.GetTargetAudioSource(0).isActiveAndEnabled ) {
        Debug.Log("Audio is actived and enabled");
    }
    if ( vp.GetTargetAudioSource(0).isPlaying ) {
        Debug.Log("Audio is playing");
    }
    Debug.Log("Video should play");
  }

  void SetColor(Renderer renderer)
  {
    Texture2D tex = new Texture2D(1, 1);
    tex.SetPixel(0, 0, Color.clear);
    tex.Apply();

    renderer.material.shader = Shader.Find("Unlit/Texture");
    renderer.material.mainTexture = tex;
  }

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

