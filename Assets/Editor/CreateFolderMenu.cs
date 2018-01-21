using UnityEngine;
using UnityEditor;

/// <summary>
/// Adds menu item when right-clicking in the project folder -> Create menu
/// 
/// Creates basic folder structure for a new asset
/// </summary>
public class CreateFolderMenu {

    /* List of folders to create for the asset */
    private static string[] _folders = { "Scripts", "Materials", "Imports", "Animations", "Components" };
    
    /// <summary>
    /// Right-click in project view where asset should be created
    /// Create -> Asset Folders
    /// </summary>
    [MenuItem("Assets/Create/Asset Folders", priority = 21)]
	private static void CreateFoldersForAsset()
    {
        Object selectedAsset = Selection.activeObject;
        if (selectedAsset != null)
        {
            string filePath = AssetDatabase.GetAssetPath(selectedAsset);
            if (System.IO.Directory.Exists(filePath))
            {
                string assetGUID = AssetDatabase.CreateFolder(filePath, "New Asset");
                string assetFilePath = AssetDatabase.GUIDToAssetPath(assetGUID);

                foreach (string folder in _folders)
                {
                    AssetDatabase.CreateFolder(assetFilePath, folder);
                }

                /* Create Textures folder under Materials */
                AssetDatabase.CreateFolder(assetFilePath + "/Materials", "Textures");

                /* Create a Readme file for the asset */
                System.IO.File.CreateText(assetFilePath + "/Readme.txt");

                /* Refresh the asset database to show the Readme file */
                AssetDatabase.Refresh();
            }
        }
    }
}
