using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor
{

    [Serializable]
    public class TileType
    {
        public Material Material;
        public float Cost;
        public char Symbol;
        public string Name => Material.name;
        public string MaterialPath => AssetDatabase.GetAssetPath(Material);
        
        public TileType(Material material, float cost, char symbol)
        {
            Material = material;
            Cost = cost;
            Symbol = symbol;
        }
        
        public static implicit operator char(TileType tileType)
        {
            return tileType.Symbol;
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        public static List<TileType> FromString(string tileMatPaths, string tileCosts)
        {
            var tileTypes = new List<TileType>();
            var pathsRE = System.Text.RegularExpressions.Regex.Matches(tileMatPaths,
                "{(.*?)}", System.Text.RegularExpressions.RegexOptions.Singleline);
            var costsRE = System.Text.RegularExpressions.Regex.Matches(tileCosts,
                "{(.*?)}", System.Text.RegularExpressions.RegexOptions.Singleline);
            if (pathsRE.Count != costsRE.Count)
            {
                Debug.LogError("Load Failed: Tile material paths and movement costs count mismatch");
                return null;
            }
            Dictionary<char, Material> materialDictionary = new Dictionary<char, Material>();
            Dictionary<char, float> costDictionary = new Dictionary<char, float>();
            for (var i = 0; i < pathsRE.Count; i++)
            {
                var split = pathsRE[i].Groups[1].Value.Replace("\"", "").Replace("\'", "").Split();
                var symbol = split[0][0];
                var path = split[1];
                var material = Resources.Load<Material>(path);
                if (!material)
                {
                    Debug.LogError($"Load Failed: Material not found at path {path}");
                    return null;
                }
                materialDictionary.Add(symbol, material);
            }
            for (var i = 0; i < costsRE.Count; i++)
            {
                var split = costsRE[i].Groups[1].Value.Replace("\'","").Replace("f", "").Split();
                var symbol = split[0][0];
                var cost = float.Parse(split[1]);
                costDictionary.Add(symbol, cost);
            }
            foreach (var pair in materialDictionary)
            {
                if (!costDictionary.TryGetValue(pair.Key, out var value))
                {
                    Debug.LogError("Load Failed: No movement cost found for material " + pair.Value.name);
                    return null;
                }
                tileTypes.Add(new TileType(pair.Value, value, pair.Key));
            }
            return tileTypes;
        }
    }

    public static class UniqueCharGenerator
    {
        //starting from ASCII 0
        private static char currentChar = (char) 0;
        
        private static char[] invalidChars = {'\n', '\r', '\t', ' ', '\'', '\"', '\\', ',', ' '};
        
        public static char GetUniqueChar()
        {
            var newChar = currentChar++;
            if (Array.IndexOf(invalidChars, newChar) != -1)
            {
                newChar = currentChar++;
            }

            return newChar;
        }
        
        public static void Reset()
        {
            currentChar = (char) 0;
        }
        
        public static void SetCurrentChar(char c)
        {
            currentChar = c;
        }
    }
    
    
    public class GridEditor : EditorWindow
    {
        private static Vector2Int gridSize = new Vector2Int(10, 8);

        public static Vector2Int GridSize
        {
            get => gridSize;
            set => gridSize = value;
        }
        
        private static TextAsset gridScript;
        private static string gridScriptPath;
        private static string gridScriptName;
        
        private static List<TileType> tileTypes = new List<TileType>();
        private static TileType selectedTileType;
        private static bool isTileSelectionFoldout;
        private static bool isTileCreatorFoldout;
        private static bool isGridFoldout;
        private static Material newTileMaterial;
        private static float newTileCost;
        private static List<String> gridString = new List<string>();

        private void OnGUI()
        {
            if (EditorApplication.isCompiling)
            {
                EditorGUILayout.HelpBox("Waiting for compiling....", MessageType.Info);
                return;
            }
            
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Edit Existing Script", EditorStyles.boldLabel);
            gridScript = (TextAsset) EditorGUILayout.ObjectField("Grid Script", gridScript, typeof(TextAsset), false);
            LoadGridScriptButton();
            EditorGUILayout.EndVertical();
            TileEditorButtons();
            TileCreator();
            Grid();
        }

        private void SaveToCurrentFile()
        {
            
        }

        private void Grid()
        {
            isGridFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(isGridFoldout, "Grid Editor");
            //foldout
            if (!isGridFoldout)
            {
                EditorGUILayout.EndFoldoutHeaderGroup();
                return;
            }
            if (tileTypes.Count == 0)
            {
                Debug.LogWarning("Add at least one tile type to edit the grid");
                isGridFoldout = false;
                EditorGUILayout.EndFoldoutHeaderGroup();
                return;
            }
            
            EditorGUILayout.LabelField("Grid Editor", EditorStyles.boldLabel);
            
            EditorGUILayout.EndFoldoutHeaderGroup();
            
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.imagePosition = ImagePosition.ImageOnly;
            
            for (int y = 0; y < gridSize.y; y++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int x = 0; x < gridSize.x; x++)
                {
                    var tile = gridString[y][x];
                    var tileType = tileTypes.Find(type => type.Symbol == tile);
                    if (tileType == null)
                    {
                        Debug.LogWarning(
                            $"Tile type not found for symbol \"{tile}\", setting to default {tileTypes[0].Material.name}");
                        tileType = tileTypes[0];
                    }
                    if (GUILayout.Button(tileType.Material.mainTexture, buttonStyle,GUILayout.Width(30), GUILayout.Height(30)))
                    {
                        gridString[y] = gridString[y].Remove(x, 1).Insert(x, selectedTileType.Symbol.ToString());
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        private void TileCreator()
        {
            isTileCreatorFoldout =
                EditorGUILayout.BeginFoldoutHeaderGroup(isTileCreatorFoldout, "Tile Creator");
            //foldout
            if (!isTileCreatorFoldout)
            {
                EditorGUILayout.EndFoldoutHeaderGroup();
                return;
            }
            EditorGUILayout.LabelField("Tile Creator", EditorStyles.boldLabel);
            newTileMaterial =
                (Material)EditorGUILayout.ObjectField("Material", newTileMaterial, typeof(Material), false);
            newTileCost = EditorGUILayout.FloatField("Movement Cost", newTileCost);
            if (GUILayout.Button("Create Tile"))
            {
                if (!newTileMaterial)
                {
                    Debug.LogError("Material is null");
                    return;
                }
                var newChar = UniqueCharGenerator.GetUniqueChar();
                while (tileTypes.Exists(tileType => tileType.Symbol == newChar))
                {
                    newChar = UniqueCharGenerator.GetUniqueChar();
                }
                tileTypes.Add(new TileType(newTileMaterial, newTileCost, newChar));
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }
        

        private void TileEditorButtons()
        {
            isTileSelectionFoldout =
                EditorGUILayout.BeginFoldoutHeaderGroup(isTileSelectionFoldout, "Tile Editor");
            //foldout
            if (!isTileSelectionFoldout)
            {
                EditorGUILayout.EndFoldoutHeaderGroup();
                return;
            }

            if (tileTypes.Count == 0)
            {
                EditorGUILayout.HelpBox("No tiles had been added...", MessageType.Info);
            }
            
            EditorGUILayout.LabelField("Tile Editor", EditorStyles.boldLabel);
            List<TileType> toRemove = new List<TileType>();
            int count = 0;
            foreach (var tiles in tileTypes)
            {
                EditorGUILayout.BeginHorizontal("helpbox");
                //stretch the texture to 50x50
                GUILayout.Label(tiles.Material.mainTexture, GUILayout.Width(50), GUILayout.Height(50));
                EditorGUILayout.BeginVertical();
                if (tiles == selectedTileType)
                {
                    EditorGUILayout.LabelField("Selected", EditorStyles.boldLabel);
                }
                else
                {
                    if (GUILayout.Button("Select", GUILayout.Width(50), GUILayout.Height(15)))
                    {
                        selectedTileType = tiles;
                    }
                    if (GUILayout.Button("X", GUILayout.Width(25), GUILayout.Height(15)))
                    {
                        toRemove.Add(tiles);
                    }
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical();
                tiles.Cost = EditorGUILayout.FloatField("Movement Cost", tiles.Cost);
                tiles.Cost = Mathf.Max(0, tiles.Cost);
                tiles.Material = (Material) EditorGUILayout.ObjectField("Material", tiles.Material, typeof(Material),
                    false);
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }
            foreach (var tile in toRemove)
            {
                tileTypes.Remove(tile);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
        
        
        private void LoadGridScriptButton()
        {
            if (GUILayout.Button("Load Grid Script"))
            {
                gridScriptPath = AssetDatabase.GetAssetPath(gridScript);
                gridScriptName = gridScript.name;
                tileTypes.Clear();
                var gridScriptText = gridScript.text;
                var tileMatStrings = System.Text.RegularExpressions.Regex.Matches(gridScriptText,
                    "//RE_MARK_1_START(.*?)//RE_MARK_1_END", System.Text.RegularExpressions.RegexOptions.Singleline);
                if (tileMatStrings.Count == 0)
                {
                    Debug.LogError("Load Failed: No tile material paths found in the script");
                    return;
                }
                var tileMatPathsStrings = tileMatStrings[0].Groups[1].Value;
                
                
                var tileCostStrings = System.Text.RegularExpressions.Regex.Matches(gridScriptText,
                    "//RE_MARK_2_START(.*?)//RE_MARK_2_END", System.Text.RegularExpressions.RegexOptions.Singleline);
                if (tileCostStrings.Count == 0)
                {
                    Debug.LogError("Load Failed: No tile movement costs found in the script");
                    return;
                }

                var tileCostStringsValue = tileCostStrings[0].Groups[1].Value;
                
                tileTypes = TileType.FromString(tileMatPathsStrings, tileCostStringsValue);
                UniqueCharGenerator.Reset();
                Debug.Log($"Tile types loaded successfully, count: {tileTypes.Count}");

                var mapStringRE = System.Text.RegularExpressions.Regex.Matches(gridScriptText,
                    "//RE_MARK_0_START(.*?)//RE_MARK_0_END", System.Text.RegularExpressions.RegexOptions.Singleline);
                if(mapStringRE.Count == 0)
                {
                    Debug.LogError("Load Failed: No gridString found in the script");
                    return;
                }

                var mapString = mapStringRE[0].Groups[1].Value.Replace(",", "").Replace("\"","").Replace("\t", "").Replace(" ", "").Replace("\r", "");
                var mapStringLines = mapString.Split('\n');
                gridString.Clear();
                for (int i = 1; i < mapStringLines.Length - 1; i++)
                {
                    gridString.Add(mapStringLines[i]);
                }
                gridSize = new Vector2Int(gridString[0].Length, gridString.Count);
                Debug.Log($"Grid loaded successfully, size: {gridSize}");
            }
        }
        

        [MenuItem("Window/GridEditor")]
        public static void ShowWindow()
        {
            GetWindow(typeof(GridEditor));
            UniqueCharGenerator.Reset();
        }
    }
}