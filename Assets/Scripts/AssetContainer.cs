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
		  Debug.Log("Delete: File deleted");	

			// Is this necessary? ... We'll check it eventually.
			Application.Quit ();
		} else {
		  Debug.Log("Delete: Unneeded - File does not exist");	
		}
	}

	public bool doesFileExistLocally() {
		bool result = File.Exists (Application.persistentDataPath + mAssetAssignedFileName);
		String fileCheckString = " - Exists Locally: " + result;
    String verifyFileExists = "Verify: File " + mAssetAssignedFileName + fileCheckString;

		Debug.Log (verifyFileExists);

		return result;
	}
}
