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

	public string debugString() {
		string fileNameAtLocalPath = "File: " + mAssetAssignedFileName + " at path: " + mAssetLocalFilePath;
		string fileEndPointString = "\n - URL: " + mAssetHttpEndpoint;
		return fileNameAtLocalPath + fileEndPointString;
	}

	private bool checkIfFileExistInPersistentAssetsPath(string fileName) {
		bool result = false;

		// PersistentDataPath is used instead of streamingDataPath because the latter is
		//	readonly.  
		DirectoryInfo directoryInfo = new DirectoryInfo(Application.persistentDataPath);

		Debug.Log("Verify: PersistentAssets Path - " + Application.persistentDataPath);

		FileInfo[] allFiles = directoryInfo.GetFiles("*.*");
		if ( allFiles.Length != 0) {
			foreach (FileInfo f in allFiles) {
				if ( f.Name.Contains(fileName) ) {
					result = true;
					mAssetLocalFilePath = Path.Combine(Application.persistentDataPath, mAssetAssignedFileName + mAssetFileType);
					Debug.Log("Verify: AssetLocalFile Path - " + mAssetLocalFilePath);
				}
			}
		}

		return result;
	}

	public bool doesFileExistLocally ()
	{
		bool result = !String.IsNullOrEmpty(mAssetLocalFilePath);
		if ( result ) { 
			try {
				result = File.Exists (mAssetLocalFilePath);
			} catch {
				result = false;
			}
		}

		String fileCheckString = " - Exists Locally: " + result;
		String verifyFileExists = "Verify: File " + mAssetAssignedFileName + fileCheckString;

		Debug.Log (verifyFileExists);

		return result;
	}
}
