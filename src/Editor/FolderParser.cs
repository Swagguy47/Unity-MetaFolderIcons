using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

//          handles writing / parsing folder visuals from associated meta files
namespace MetaFolderIcon.Editor
{
    public class FolderParser : UnityEditor.Editor
    {
        //      get color from meta
        public static Color ParseCol(string meta)
        {
            List<string> lines = ReadData(meta);

            //  no data exists
            if (lines.Count <= 8) return Color.white;

            //  split data into parts
            string[] data = lines[8].Split("?");

            //  create color from data
            return new Color(float.Parse(data[0]), float.Parse(data[1]), float.Parse(data[2]));
        }

        //      get texture from meta
        public static Texture ParseTex(string meta)
        {
            List<string> lines = ReadData(meta);

            //  no data exists
            if (lines.Count <= 8) return null;

            //  split data into parts
            string[] data = lines[8].Split("?");

            //  get texture from path
            return AssetDatabase.LoadAssetAtPath<Texture>(data[3]); ;
        }

        //      get texture from meta
        public static bool ParseUnderline(string meta)
        {
            List<string> lines = ReadData(meta);

            //  no data exists
            if (lines.Count <= 8) return false;

            //  split data into parts
            string[] data = lines[8].Split("?");

            var trueFalse = data[data.Length - 1];

            if (trueFalse == "")
                return false;

            bool output = false;
            if (bool.TryParse(trueFalse, out output))
                return output;
            else 
                return false;
        }

        //      Write new folder data to meta
        public static void WriteData(string meta, string data)
        {
            //  get all lines from meta file
            List<string> lines = ReadData(meta);

            if (lines.Count <= 8) { //hopefully not less than
                //add data for first time
                lines.Add(data);
            } else {
                //replace existing data
                lines[8] = data;
            }

            //  write to file
            File.WriteAllLines(meta, lines);
        }

        //      read data from meta
        public static List<string> ReadData(string meta)
        {
            if (meta == "Assets.meta") return new List<string>() { "" }; //failsafe to prevent attempted reading of root Assets folder metadata

            return File.ReadAllLines(meta).ToList<string>();
        }

        public static bool CheckData(string meta)
        {
            if (meta == "Assets.meta") return false;
            string[] data = File.ReadAllLines(meta);
            data = data[data.Length - 1].Split("?");
            return data.Length > 2;
        }
    
        public static bool ClearData(string meta)
        {
            List<string> lines = ReadData(meta);

            //  no data exists
            if (lines.Count <= 8) return false;

            List<string> newLines = new();

            for (int i = 0; i < lines.Count - 1; i++)
            {
                newLines.Add(lines[i]);
            }
        
            //  write to file
            File.WriteAllLines(meta, newLines);

            return true;
        }

        public static bool CheckFresh(string meta)
        {
            List<string> lines = ReadData(meta);

            //  no data exists
            if (lines.Count <= 8) return false;

            //  split data into parts
            string[] data = lines[8].Split("?");

            if (data.Length > 4)
            {
                if (data[4] == "FRESH")
                {
                    data[4] = "UNFRESH";
                    lines[8] = "";
                    for (int i = 0; i < data.Length; i++)
                    {
                        if (i != 0)
                            lines[8] += "?";
                        lines[8] += data[i];
                    }
                
                    //  write to file
                    File.WriteAllLines(meta, lines);

                    return true;
                }
            }

            return false;
        }
    }
}
