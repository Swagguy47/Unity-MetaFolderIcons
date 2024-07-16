using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace MetaFolderIcon.Editor
{
    public class IconDictionaryCreator : AssetPostprocessor
    {
        private const string AssetsPath = "MetaFolderIcon/Icons";
        internal static Dictionary<string, Texture> IconDictionary;

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (!ContainsIconAsset(importedAssets) &&
                !ContainsIconAsset(deletedAssets) &&
                !ContainsIconAsset(movedAssets) &&
                !ContainsIconAsset(movedFromAssetPaths))
            {
                return;
            }

            BuildDictionary();
        }

        private static bool ContainsIconAsset(string[] assets)
        {
            foreach (string str in assets)
            {

                if (ReplaceSeparatorChar(Path.GetDirectoryName(str)) == "Assets/" + AssetsPath)
                {
                    return true;
                }
            }
            return false;
        }

        private static string ReplaceSeparatorChar(string path)
        {
            return path.Replace("\\", "/");
        }

        internal static void BuildDictionary()
        {
            var dictionary = new Dictionary<string, Texture>();

            var dir = new DirectoryInfo(Application.dataPath + "/" + AssetsPath);
            FileInfo[] info = dir.GetFiles("*.png");
            foreach(FileInfo f in info)
            {
                var texture = (Texture)AssetDatabase.LoadAssetAtPath($"Assets/MetaFolderIcon/Icons/{f.Name}", typeof(Texture2D));
                var splitKeys = f.Name.Split(',');
                foreach (string key in splitKeys)
                    dictionary.Add(Path.GetFileNameWithoutExtension(key.ToLower()), texture);
            }

            FileInfo[] infoSO = dir.GetFiles("*.asset");
            foreach (FileInfo f in infoSO) 
            {
                var folderIconSO = (FolderIconSO)AssetDatabase.LoadAssetAtPath($"Assets/MetaFolderIcon/Icons/{f.Name}", typeof(FolderIconSO));

                if (folderIconSO != null) 
                {
                    var texture = (Texture)folderIconSO.icon;

                    foreach (string folderName in folderIconSO.folderNames) 
                    {
                        if (folderName != null) 
                        {
                            dictionary.TryAdd(folderName, texture);
                        }
                    }
                }
            }
            
            IconDictionary = dictionary;
        }
    }
}
