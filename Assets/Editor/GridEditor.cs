using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class GridEditor : EditorWindow
    {
        public string GridName { get; set; }
        
        public TextAsset GridFile { get; set; }
        
        public Vector2Int GridSize { get; set; }
        
        private string gridScriptStoredPath = "Assets/Scripts/Grids/";
        
        private Dictionary<int, Material> costToTile = new Dictionary<int, Material>();
        
        public Material DefaultTile { get; set; }
        public int DefaultCost { get; set; }

        private string[] gridStrings;
        
        private void OnGUI()
        {
            GridName = EditorGUILayout.TextField("Grid Name", GridName);
            GridFile = (TextAsset) EditorGUILayout.ObjectField("Grid Script", GridFile, typeof(TextAsset), false);
            if (GUILayout.Button("Create New Grid"))
            {
                if (GridSize.x == 0 || GridSize.y == 0)
                {
                    throw new Exception("Grid size cannot be 0");
                    return;
                }

                if (!DefaultTile)
                {
                    throw new Exception("Default tile cannot be null");
                    return;
                }
                
            }
            GUI.BeginGroup(new Rect(0, 140, position.width, 140), "Tile Selection Area", EditorStyles.helpBox);
            DefaultTile = (Material) EditorGUILayout.ObjectField("Default Tile", DefaultTile, typeof(Material), false);
            DefaultCost = EditorGUILayout.IntField("Default Cost", DefaultCost);
            GUI.EndGroup();
        }
        
        [MenuItem("Window/GridEditor")]
        public static void ShowWindow()
        {
            GetWindow(typeof(GridEditor));
        }
    }
}