using UnityEngine;
using System.Collections;
using UnityFx.Async;
using UnityFx.Async.Promises;

// For promises-specific stuff.
using UnityEngine.Networking;
using System;

public class AssetDownloader : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		DownloadTextAsync ("http://www.google.com")
		    .Then (text => Debug.Log (text))
		    .Catch (e => Debug.LogException (e));	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public IAsyncOperation<string> DownloadTextAsync (string url)
	{
		var result = new AsyncCompletionSource<string> ();
		StartCoroutine (DownloadTextInternal (result, url));
		return result;
	}

	private IEnumerator DownloadTextInternal (IAsyncCompletionSource<string> op, string url)
	{
		var www = UnityWebRequest.Get (url);
		yield return www.Send ();

		if (www.isNetworkError || www.isHttpError) {
			op.SetException (new Exception (www.error));
		} else {
			op.SetResult (www.downloadHandler.text);
		}
	}
}

