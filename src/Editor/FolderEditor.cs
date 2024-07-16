using System.IO;
using UnityEditor;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

namespace MetaFolderIcon.Editor
{
    public class FolderEditor : EditorWindow
    {

        static string path = string.Empty;
        static Color folderCol = Color.white;
        static Texture folderIco = null;
        static bool underlineOnly = false;

        //open customization window

        [MenuItem("Assets/[ Customize Folder ]")]
        static void CustomizeFolder()
        {
            path = isFolder();

            if (!AssetDatabase.IsValidFolder(path))
                return;

            //  failsafe if not a folder
            if (path == null) { Debug.Log("<color=yellow>Please select a folder first!</color>"); return; }

            EditorWindow.GetWindow(typeof(FolderEditor));
        }

        private void OnEnable()
        {
            folderIco = null;
            folderCol = Color.white;
            underlineOnly = false;
        
            string meta = isFolder();

            var iconDictionary = IconDictionaryCreator.IconDictionary;

            if (iconDictionary.ContainsKey(Path.GetFileName(path.ToLower())))
                folderIco = iconDictionary[Path.GetFileName(path.ToLower())];

            if (FolderParser.CheckData(meta + ".meta"))
            {
                folderCol = FolderParser.ParseCol(meta + ".meta");
                folderIco = FolderParser.ParseTex(meta + ".meta");
                underlineOnly = FolderParser.ParseUnderline(meta + ".meta");
            }
            else if(folderIco == null)
            {
                folderIco = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MetaFolderIcon/Icons/FolderBackground.png", typeof(Texture2D));
            }
        }

        private void OnGUI()
        {
            //  TODO: Insert preview graphic

            GUILayout.Space(10);

            GUILayout.Label("Color:");

            //  color
            folderCol = EditorGUILayout.ColorField(folderCol);

            underlineOnly = GUILayout.Toggle(underlineOnly, "Only Recolor Underline");

            GUILayout.Space(10);

            //  icon
            folderIco = (Texture)EditorGUILayout.ObjectField("Icon:", folderIco, typeof(Texture), false);

            GUILayout.Space(10);

            //  apply button
            if (GUILayout.Button("Apply")) {
                ApplyFolderVisuals(false);
                this.Close();
            }

            var folderName = path.Split('/');
            var iconDictionary = IconDictionaryCreator.IconDictionary;

            if (!iconDictionary.ContainsKey(folderName[folderName.Length - 1])) {
                //  apply all button
                if (GUILayout.Button("Apply to all folders named: '" + folderName[folderName.Length - 1] + "'"))
                {
                    ApplyFolderVisuals(true);
                    this.Close();
                }
            }

            //  clear button
            if (GUILayout.Button("Clear metadata")) {
                FolderParser.ClearData(path + ".meta");
                this.Close();
            }

            GUILayout.Space(10);

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            //  label
            GUILayout.Label("Customizing: " + path);

            //  preview render
            DrawPreview();
        }

        void DrawPreview()
        {
            float imageWidth = folderIco.width / 2;
            float imageHeight = folderIco.height / 2;

            var rect = new Rect(0, 275, imageWidth, imageHeight);
            var textRect = new Rect(0, 375, imageWidth, 50);

            var folderName = path.Split("/");

            Texture textTex = null;
            foreach (HierarchyOverlays.TexCache cache in HierarchyOverlays.cachedTex)
                if (cache.name == "HierarchyLineCone.png")
                    textTex = cache.tex;

            var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };

            GUI.DrawTexture(rect, folderIco, UnityEngine.ScaleMode.StretchToFill, true, 0, underlineOnly ? Color.white : folderCol, 0, 0);
            EditorGUI.LabelField(textRect, folderName[folderName.Length - 1], style);
            GUI.DrawTexture(textRect, textTex, UnityEngine.ScaleMode.StretchToFill, true, 0, folderCol == Color.white ? new Color(0,0,0,0) : folderCol, 0, 0);
        }


        //      checks selection is a folder
        //      TODO: Return null if selection is asset
        static string isFolder()
        {
            var Getpath = "";
            var obj = Selection.activeObject;

            if (obj == null) Getpath = "Assets";
            else Getpath = AssetDatabase.GetAssetPath(obj.GetInstanceID());

            if (Getpath.Length > 0) {
                //Debug.Log(Getpath);
                return Getpath;
            }

            return null;
        }

        //      modify meta file to contain new data about folder
        public void ApplyFolderVisuals(bool all)
        {
            string metaPath = path + ".meta";
            string folderVisuals = folderCol.r.ToString() + "?" + folderCol.g.ToString() + "?" + folderCol.b.ToString() + "?" + AssetDatabase.GetAssetPath(folderIco) + "?FRESH?" + underlineOnly;
        
            //  write to file
            FolderParser.WriteData(metaPath, folderVisuals);
        }
    }
}
