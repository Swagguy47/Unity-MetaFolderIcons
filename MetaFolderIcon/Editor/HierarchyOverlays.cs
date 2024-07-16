using MetaFolderIcon.Editor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MetaFolderIcon.Editor
{
    [InitializeOnLoad]
    public class HierarchyOverlays : MonoBehaviour
    {
        public static List<TexCache> cachedTex = new();

        public static char[] hierarchyChars = { '~', '!', '@', '#', '^', '$', '*', '-', '_', '=' };
        public static Color[] hierarchyCols = {new Color(0, 0.5f, 1), Color.red, Color.green, new Color(0, 1, 1),
        new Color(1, 1, 0), new Color(1, 0.6f, 0), new Color(1, 0, 1), Color.white, Color.gray, Color.black};

        public struct TexCache
        {
            public Texture tex;
            public string name;
        }

        static HierarchyOverlays()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyGui;
            EditorApplication.projectWindowItemOnGUI += ProjectGui;
        }

        public static void ProjectGui(string guid, Rect rect)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);

            if (path == "" ||
                    Event.current.type != EventType.Repaint ||
                    !File.GetAttributes(path).HasFlag(FileAttributes.Directory))
            {
                return;
            }

            var newRect = new Rect();
            var texName = "";
            var scaleMode = new ScaleMode();

            //  swap for different icon sizes
            switch (rect.height >= 20)
            {
                //  big icons
                case true:
                    {
                        var overlayHeight = rect.height / 5;
                        var fixedHeight = ((rect.y + rect.height) - overlayHeight) + 5;

                        newRect = new Rect(rect.x, fixedHeight, rect.width, overlayHeight);
                        texName = "HierarchyLineCone.png";
                        scaleMode = ScaleMode.StretchToFill;
                        break;
                    }
                //  small icons
                case false:
                    {
                        newRect = new Rect(rect.x + 15, rect.y, rect.width, rect.height);
                        texName = "HierarchyGradient.png";
                        scaleMode = ScaleMode.ScaleAndCrop;
                        break;
                    }
            }

            //  color tint
            string metaPath = AssetDatabase.GUIDToAssetPath(guid) + ".meta";
            Color col = FolderParser.ParseCol(metaPath);

            if (col == Color.white)
                col = new Color(0, 0, 0, 0);

            //EditorGUI.DrawRect(newRect, new Color(1, 0,0, 0.25f));
            RenderTexture(newRect, texName, col, scaleMode);
        }



        public static void HierarchyGui(int id, Rect rect)
        {
            //  get & check instance
            var instance = EditorUtility.InstanceIDToObject(id);

            if (!instance)
                return;

            var col = new Color(0, 0, 0, 0);

            for(int i = 0; i < hierarchyChars.Length; i++)
            {
                if (instance.name.StartsWith(hierarchyChars[i]))
                {
                    col = hierarchyCols[i];
                    break;
                }
            }

            RenderTexture(rect, "HierarchyCapsule.png", col, ScaleMode.StretchToFill);
        }

        public static void RenderTexture(Rect rect, string fileName, Color col, ScaleMode scaleMode)
        {
            //  get & check texture
            Texture tex;

            tex = GetCachedTex(fileName);

            if (!tex)
            {
                tex = AssetDatabase.LoadAssetAtPath<Texture>("Assets/MetaFolderIcon/Resources/" + fileName);

                //  cache new texture
                TexCache cache = new TexCache();
                cache.tex = tex;
                cache.name = fileName;
                cachedTex.Add(cache);
            }

            if (!tex)
                return;

            //  render
            //EditorGUI.DrawTextureTransparent(rect, tex, ScaleMode.StretchToFill, 0, 0, UnityEngine.Rendering.ColorWriteMask.All, 0);
            GUI.DrawTexture(rect, tex, scaleMode, true, 0, col, 0, 0);
        }

        public static Texture GetCachedTex(string name)
        {
            foreach (TexCache cache in cachedTex)
            {
                if (cache.name == name)
                    return cache.tex;
            }

            return null;
        }
    }
}