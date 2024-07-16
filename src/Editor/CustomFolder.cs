using System.IO;
using UnityEditor;
using UnityEngine;

namespace MetaFolderIcon.Editor
{
    [InitializeOnLoad]
    public class CustomFolder
    {
        static CustomFolder()
        {
            IconDictionaryCreator.BuildDictionary();
            EditorApplication.projectWindowItemOnGUI += DrawFolderIcon;
        }

        static void DrawFolderIcon(string guid, Rect rect)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var iconDictionary = IconDictionaryCreator.IconDictionary;

            if (path == "" ||
                Event.current.type != EventType.Repaint ||
                !File.GetAttributes(path).HasFlag(FileAttributes.Directory))
            {
                return;
            }

            string metaPath = AssetDatabase.GUIDToAssetPath(guid) + ".meta";

            //  exclude packages to allow compatibility with oddly formatted assets
            if (metaPath.Split("/".ToCharArray())[0] == "Packages") return;
            
            bool iconExists;
            string dictionaryPath = guid;
            iconExists = iconDictionary.ContainsKey(Path.GetFileName(guid));
            if (iconDictionary.ContainsKey(Path.GetFileName(guid)) && FolderParser.CheckFresh(metaPath))
                iconExists = false;
            if (!iconExists)  {
                iconExists = iconDictionary.ContainsKey(Path.GetFileName(path.ToLower()));
                dictionaryPath = Path.GetFileName(path.ToLower());
            } 

            if (iconDictionary.ContainsKey(Path.GetFileName(guid)) && !FolderParser.CheckData(metaPath))
            {
                AssetDatabase.DeleteAsset("Assets/MetaFolderIcon/Icons/" + guid + ".png");
                iconExists = false;
            }
            
            if (!iconExists)
            {
                Rect imageSquare;

                if (rect.height > 20)
                {
                    imageSquare = new Rect(rect.x - 1, rect.y - 1, rect.width + 2, rect.width + 2);
                }
                else if (rect.x > 20)
                {
                    imageSquare = new Rect(rect.x - 1, rect.y - 1, rect.height + 2, rect.height + 2);
                }
                else
                {
                    imageSquare = new Rect(rect.x + 2, rect.y - 1, rect.height + 2, rect.height + 2);
                }

                var basic = IconDictionaryCreator.IconDictionary[Path.GetFileName("Basic".ToLower())];
                if (basic == null)
                {
                    return;
                }
                
                //redraw editor icon
                if(FolderParser.CheckData(metaPath))
                    basic = GenerateIcon(guid);

                GUI.DrawTexture(imageSquare, basic);
                return;
            }

            Rect imageRect;

            if (rect.height > 20)
            {
                imageRect = new Rect(rect.x - 1, rect.y - 1, rect.width + 2, rect.width + 2);
            }
            else if (rect.x > 20)
            {
                imageRect = new Rect(rect.x - 1, rect.y - 1, rect.height + 2, rect.height + 2);
            }
            else
            {
                imageRect = new Rect(rect.x + 2, rect.y - 1, rect.height + 2, rect.height + 2);
            }
            
            var texture = IconDictionaryCreator.IconDictionary[dictionaryPath]; //  change to GUID?
            if (texture == null)
            {
                return;
            }

            //texture = GenerateIcon(guid);

            GUI.DrawTexture(imageRect, texture);
        }


        //  TODO:   Refactor?
        static Texture GenerateIcon(string guid)
        {
            string metaPath = AssetDatabase.GUIDToAssetPath(guid) + ".meta";

            Color col = FolderParser.ParseCol(metaPath);
            Texture ico = FolderParser.ParseTex(metaPath);

            Texture bg = IconDictionaryCreator.IconDictionary[Path.GetFileName("FolderBackground".ToLower())];
            
            if (ico == null)
                ico = IconDictionaryCreator.IconDictionary[Path.GetFileName("Basic".ToLower())];
            
            //get base texture and extract pixel colors
            var basic = (Texture2D)ico;
            FixInput((Texture2D)ico);
            Color[] cols = basic.GetPixels();

            //weird / 2 nonsense purely to reduce reocurring instances of resolution-getting
            Texture2D tex = new Texture2D(ico.width, ico.height);

            bool onlyUnderline = FolderParser.ParseUnderline(metaPath);

            //loop each color in pixel and tint based on user color
            for (int i = 0; i < cols.Length; i++) {
                float a = cols[i].a;
                cols[i] *= onlyUnderline ? Color.white : col;
                cols[i].a = a;
            }
            tex.SetPixels(cols); 
            
            byte[] tex2 = tex.EncodeToPNG();

            FileStream stream = new FileStream("Assets/MetaFolderIcon/Icons/" + guid + ".png", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            BinaryWriter writer = new BinaryWriter(stream);
            for (int j = 0; j < tex2.Length; j++)
                writer.Write(tex2[j]);

            writer.Close();
            stream.Close();
            
            AssetDatabase.ImportAsset("Assets/MetaFolderIcon/Icons/" + guid + ".png", ImportAssetOptions.ForceUpdate);
            
            return tex;
        }
        
        static void FixInput(Texture2D input) //Does as title suggests
        {
            TextureImporter A = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath((Object)input));
            A.isReadable = true;
            A.textureType = TextureImporterType.Default;
            A.textureCompression = TextureImporterCompression.Uncompressed;
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath((Object)input), ImportAssetOptions.ForceUpdate);
            input = (Texture2D)AssetDatabase.LoadAssetAtPath(AssetDatabase.GetAssetPath((Object)input), typeof(Texture2D));
        }
    }
}
