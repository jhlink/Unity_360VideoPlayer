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

	public string toString() {
		string fileNameAtLocalPath = "File: " + mAssetAssignedFileName + " at path: " + mAssetLocalFilePath;
		string fileEndPointString = " - URL: " + mAssetHttpEndpoint;
		return fileNameAtLocalPath + fileEndPointString;
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

	public bool doesFileExistLocally ()
	{
		string persistentFilePath = Application.persistentDataPath + "/" + mAssetAssignedFileName;
		bool result = File.Exists (persistentFilePath);

		if ( result ) { 
			mAssetLocalFilePath = Path.Combine(Application.persistentDataPath,  	Path.GetFileName(persistentFilePath));
		} else {
			Debug.Log("Is it really not found? Check the VideoID list and verify that the file ID is terminated with a file extension. Hacky? Absolutely.");
		}

		String fileCheckString = " - Exists Locally: " + result;
		String verifyFileExists = "Verify: File " + mAssetAssignedFileName + fileCheckString;

		Debug.Log (verifyFileExists);

		return result;
	}
}
