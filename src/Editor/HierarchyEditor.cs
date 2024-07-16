using System.IO;
using UnityEditor;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

namespace MetaFolderIcon.Editor
{
    public class HierarchyEditor : EditorWindow
    {

        public static void RecolorObject(GameObject[] selection, string col)
        {
            foreach (GameObject go in selection)
                go.name = col + go.name;
        }

        [MenuItem("GameObject/[ Recolor Object ]/Red", false, 0)]
        static void RecolorRed()
        {
            RecolorObject(Selection.gameObjects, "!");
        }

        [MenuItem("GameObject/[ Recolor Object ]/Green", false, 1)]
        static void RecolorGreen()
        {
            RecolorObject(Selection.gameObjects, "@");
        }
        [MenuItem("GameObject/[ Recolor Object ]/Blue", false, 2)]
        static void RecolorBlue()
        {
            RecolorObject(Selection.gameObjects, "~");
        }
        [MenuItem("GameObject/[ Recolor Object ]/Cyan", false, 3)]
        static void RecolorCyan()
        {
            RecolorObject(Selection.gameObjects, "#");
        }
        [MenuItem("GameObject/[ Recolor Object ]/Yellow", false, 4)]
        static void RecolorYellow()
        {
            RecolorObject(Selection.gameObjects, "^");
        }
        [MenuItem("GameObject/[ Recolor Object ]/Orange", false, 5)]
        static void RecolorOrange()
        {
            RecolorObject(Selection.gameObjects, "$");
        }
        [MenuItem("GameObject/[ Recolor Object ]/Magenta", false, 6)]
        static void RecolorMagenta()
        {
            RecolorObject(Selection.gameObjects, "*");
        }
        [MenuItem("GameObject/[ Recolor Object ]/White", false, 7)]
        static void RecolorWhite()
        {
            RecolorObject(Selection.gameObjects, "-");
        }
        [MenuItem("GameObject/[ Recolor Object ]/Gray", false, 8)]
        static void RecolorGray()
        {
            RecolorObject(Selection.gameObjects, "_");
        }
        [MenuItem("GameObject/[ Recolor Object ]/Black", false, 9)]
        static void RecolorBlack()
        {
            RecolorObject(Selection.gameObjects, "=");
        }
    }
}
