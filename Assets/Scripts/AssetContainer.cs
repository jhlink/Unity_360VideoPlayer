using System;
using UnityEngine;
using System.IO;

public class AssetContainer
{
	private string mAssetHttpEndpoint = "";
	private string mAssetLocalFilePath = "";

	//  This is a default string name if no mAssetLocalFilePath is provided.
	//    Default behavior will be overwritting the existing fileBlob
	private string mAssetAssignedFileName = "dataBlob";
	private string mAssetFileType = ".mp4";

	public AssetContainer ()
	{
	}

	public AssetContainer (String assetHttpEndpoint, String assetFileName)
	{
		mAssetHttpEndpoint = assetHttpEndpoint;
		mAssetAssignedFileName = assetFileName;
	}

	public string AssetHttpEndpoint { 
		get {
			return mAssetHttpEndpoint;
		}

		set {
			mAssetHttpEndpoint = value;
		}
	}

	public string AssetFileType { 
		get {
			return mAssetFileType;
		}

		set { 
			mAssetFileType = value;	
		}

	}

	public string AssignedAssetFiledName { 
		get {
			return mAssetAssignedFileName;
		} 
		set {
			mAssetAssignedFileName = value; 
		}
	}

	public string AssetLocalFilePath {
		get {
			return mAssetLocalFilePath;
		}
		set {
			if (!String.IsNullOrEmpty (value)) {
				mAssetLocalFilePath = value;
			}
		}
	}

	public void deleteFileFromSystem ()
	{
		if (doesFileExistLocally ()) {
			File.Delete (Application.persistentDataPath + mAssetAssignedFileName);
			Debug.Log ("Delete: File deleted");	

			// Is this necessary? ... We'll check it eventually.
			Application.Quit ();
		} else {
			Debug.Log ("Delete: Unneeded - File does not exist");	
		}
	}

	public string toString() {
		string fileNameAtLocalPath = "File: " + mAssetAssignedFileName + " at path: " + mAssetLocalFilePath;
		string fileEndPointString = "\n - URL: " + mAssetHttpEndpoint;
		return fileNameAtLocalPath + fileEndPointString;
	}

	private bool checkIfFileExistInStreamingAssetsPath(string fileName) {
		bool result = false;

		DirectoryInfo directoryInfo = new DirectoryInfo(Application.streamingAssetsPath);

		Debug.Log("Verify: StreamingAssets Path - " + Application.streamingAssetsPath);

		FileInfo[] allFiles = directoryInfo.GetFiles("*.*");
		foreach (FileInfo f in allFiles) {
			if ( f.Name.Contains(fileName) ) {
				result = true;
				mAssetLocalFilePath = Path.Combine(Application.streamingAssetsPath, mAssetAssignedFileName + mAssetFileType);
				Debug.Log("Verify: AssetLocalFile Path - " + mAssetLocalFilePath);
			}
		}

		return result;
	}

	public bool doesFileExistLocally ()
	{
		//string persistentFilePath = Application.streamingAssetsPath + "/" + mAssetAssignedFileName;
		//bool result = File.Exists (persistentFilePath);
		bool result = checkIfFileExistInStreamingAssetsPath(mAssetAssignedFileName);

		String fileCheckString = " - Exists Locally: " + result;
		String verifyFileExists = "Verify: File " + mAssetAssignedFileName + fileCheckString;

		Debug.Log (verifyFileExists);

		return result;
	}
}
