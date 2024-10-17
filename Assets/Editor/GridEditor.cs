using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor
{

    [Serializable]
    public class TileType
    {
        public Material Material;
        
        private float cost;

        public float Cost
        {
            get => cost;
            set
            {
                cost = value;
                cost = Mathf.Max(.001f, cost);
            }
        }
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
            Debug.Log($"Successfully loaded {materialDictionary.Count} materials");
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
                    Debug.LogError($"Load Failed: No movement cost found for material {pair.Value.name}, symbol {pair.Key}");
                    return null;
                }
                tileTypes.Add(new TileType(pair.Value, value, pair.Key));
            }
            return tileTypes;
        }
        
        public static string ToSaveMaterialPathString(List<TileType> tileTypes)
        {
            var result = "";
            foreach (var tileType in tileTypes)
            {
                result += $"\t\t\t{{'{tileType.Symbol}', \"{tileType.MaterialPath.Replace("Assets/Resources/", "").Replace(".mat","")}\"}},\n";
            }
            return result;
        }
        
        public static string ToSaveCostString(List<TileType> tileTypes)
        {
            var result = "";
            foreach (var tileType in tileTypes)
            {
                result += $"\t\t\t{{'{tileType.Symbol}', {tileType.Cost}f}},\n";
            }
            return result;
        }
    }

    public static class UniqueCharGenerator
    {
        //starting from ASCII 0
        // private static char currentChar = (char) 33;
        //
        // private static char[] invalidChars = {'\n', '\r', '\t', ' ', '\'', '\"', '\\', ',', ' '};
        
        private static string validChars = "abcdeghijklmnopqrstuvwxyzABCDEGHIJKLMNOPQRSTUVWXYZ0123456789";
        private static int currentCharIndex = 0;
        private static char currentChar => validChars[currentCharIndex];
        
        public static char GetUniqueChar()
        {
            // while (invalidChars.Contains(currentChar))
            // {
            //     currentChar++;
            // }
            currentCharIndex++;
            if (currentCharIndex >= validChars.Length)
            {
                currentCharIndex = 0;
            }
            return currentChar;
        }
        
        public static void Reset()
        {
            currentCharIndex = 0;
        }
        
    }
    
    
    public class GridEditor : EditorWindow
    {
        private static Vector2Int gridSize = new Vector2Int(10, 8);

        public static Vector2Int GridSize
        {
            get => gridSize;
            set
            {
                value.x = Mathf.Max(1, value.x);
                value.y = Mathf.Max(1, value.y);
                gridSize = value;
            }
        }

        private static TextAsset gridScript;
        private static string gridScriptPath = "Assets/Scripts/Grids/";
        private static string gridScriptName = "";
        
        private static List<TileType> tileTypes = new List<TileType>();
        private static TileType selectedTileType;
        private static bool isTileSelectionFoldout;
        private static bool isTileCreatorFoldout;
        private static bool isGridFoldout;
        private static Material newTileMaterial;
        private static float newTileCost;
        private static List<String> gridString = new List<string>();
        private static string scriptTemplateString;

        private void OnGUI()
        {
            if (EditorApplication.isCompiling)
            {
                EditorGUILayout.HelpBox("Waiting for compiling....", MessageType.Info);
                return;
            }

            if (EditorApplication.isPlaying)
            {
                EditorGUILayout.HelpBox("Cannot edit grid while playing", MessageType.Info);
                return;
            }
            
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("From Existing Script", EditorStyles.boldLabel);
            gridScript = (TextAsset) EditorGUILayout.ObjectField("Grid Script", gridScript, typeof(TextAsset), false);
            LoadGridScriptButton();
            SaveToCurrentFileButton();
            EditorGUILayout.EndVertical();
            SaveToNewFile();
            TileEditorButtons();
            TileCreator();
            GridSize = EditorGUILayout.Vector2IntField("Grid Size", GridSize);
            Grid();
        }

        private void SaveToNewFile()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Save To New File", EditorStyles.boldLabel);
            gridScriptName = EditorGUILayout.TextField("Grid Script Name", gridScriptName);
            if (!IsValidFileName(gridScriptName))
            {
                EditorGUILayout.HelpBox("Invalid script name", MessageType.Error);
                EditorGUILayout.EndVertical();
                return;
            }
            gridScriptPath = EditorGUILayout.TextField("Grid Script Path", gridScriptPath);
            if (!Directory.Exists(gridScriptPath))
            {
                EditorGUILayout.HelpBox("Invalid script path", MessageType.Error);
                EditorGUILayout.EndVertical();
                return;
            }
            if (GUILayout.Button("Save To New File"))
            {
                var gridScriptText = GenerateScriptText();
                var className = gridScriptName;
                gridScriptText = System.Text.RegularExpressions.Regex.Replace(gridScriptText, "public class (.*?):",
                    $"public class {className}:", System.Text.RegularExpressions.RegexOptions.Singleline);
                //Create the file
                var path = $"{gridScriptPath}/{gridScriptName}.cs";
                if (File.Exists(path))
                {
                    Debug.LogError("File already exists");
                    return;
                }
                File.WriteAllText(path, gridScriptText);
                AssetDatabase.Refresh();
            }
            
            EditorGUILayout.EndVertical();
        }
        
        private bool IsValidFileName(string fileName)
        {
            return fileName.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) == -1 && fileName.Length > 0 &&
                   char.IsLetter(fileName[0]);
        }

        private void SaveToCurrentFileButton()
        {
            if (GUILayout.Button("Save To Selected File"))
            {
                if (!gridScript)
                {
                    Debug.LogError("Grid script is null");
                    return;
                }

                var gridScriptText = GenerateScriptText(false);
                var className = gridScript.name;
                gridScriptText = System.Text.RegularExpressions.Regex.Replace(gridScriptText, "public class (.*?):",
                    $"public class {className}:", System.Text.RegularExpressions.RegexOptions.Singleline);
                File.WriteAllText(AssetDatabase.GetAssetPath(gridScript), gridScriptText);
                AssetDatabase.Refresh();
            }
        }

        private string GenerateScriptText(bool overwrite = true)
        {
            var result = scriptTemplateString;
            if (!overwrite)
            {
                result = gridScript.text;
            }
            var tileMatPaths = TileType.ToSaveMaterialPathString(tileTypes);
            var tileCosts = TileType.ToSaveCostString(tileTypes);
            var gridStrings = "";
            for (int i = 0; i < gridString.Count; i++)
            {
                gridStrings += $"\t\t\t\"{gridString[i]}\",\n";
            }
            //RE replace
            result = System.Text.RegularExpressions.Regex.Replace(result, "//RE_MARK_0_START(.*?)//RE_MARK_0_END",
                $"//RE_MARK_0_START\n{gridStrings}\t\t\t//RE_MARK_0_END\n", System.Text.RegularExpressions.RegexOptions.Singleline);
            result = System.Text.RegularExpressions.Regex.Replace(result, "//RE_MARK_1_START(.*?)//RE_MARK_1_END",
                $"//RE_MARK_1_START\n{tileMatPaths}\t\t\t//RE_MARK_1_END\n", System.Text.RegularExpressions.RegexOptions.Singleline);
            result = System.Text.RegularExpressions.Regex.Replace(result, "//RE_MARK_2_START(.*?)//RE_MARK_2_END", 
                $"//RE_MARK_2_START\n{tileCosts}\t\t\t//RE_MARK_2_END\n", System.Text.RegularExpressions.RegexOptions.Singleline);
            return result;
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
            
            //adjust the grid size
            while (gridString.Count < gridSize.y)
            {
                gridString.Add(new string(tileTypes[0].Symbol, gridSize.x));
            }
            while (gridString.Count > gridSize.y)
            {
                gridString.RemoveAt(gridString.Count - 1);
            }

            while (gridString[0].Length < gridSize.x)
            {
                for (int i = 0; i < gridString.Count; i++)
                {
                    gridString[i] += tileTypes[0].Symbol;
                }
            }
            while (gridString[0].Length > gridSize.x)
            {
                for (int i = 0; i < gridString.Count; i++)
                {
                    gridString[i] = gridString[i].Remove(gridString[i].Length - 1);
                }
            }
            
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
                        gridString[y] = gridString[y].Remove(x, 1).Insert(x, tileType.Symbol.ToString());
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
            bool isMaterialValid = false;
            newTileMaterial = (Material)EditorGUILayout.ObjectField("Material", newTileMaterial, typeof(Material), false);
            // check material path
            if (newTileMaterial)
            {
                var path = AssetDatabase.GetAssetPath(newTileMaterial);
                if (path.StartsWith("Assets/Resources/"))
                {
                    isMaterialValid = true;
                }
                else
                {
                    EditorGUILayout.HelpBox("Material must be in Resources folder", MessageType.Error);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Material is null", MessageType.Error);
            }
            newTileCost = EditorGUILayout.FloatField("Movement Cost", newTileCost);
            if (isMaterialValid)
            {
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
                    Debug.Log($"Tile created successfully, symbol: {newChar}");
                }
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
                var material = (Material) EditorGUILayout.ObjectField("Material", tiles.Material, typeof(Material),
                    false);;
                if (material)
                {
                    tiles.Material = material;
                    var path = AssetDatabase.GetAssetPath(tiles.Material);
                    if (path.StartsWith("Assets/Resources/"))
                    {
                        tiles.Material = tiles.Material;
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("Material must be in Resources folder", MessageType.Error);
                    }
                }

                
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
                LoadFromString(gridScriptText);
            }
        }

        private static void LoadFromString(string gridScriptText)
        {
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
        

        [MenuItem("Window/GridEditor")]
        public static void ShowWindow()
        {
            GetWindow(typeof(GridEditor));
            UniqueCharGenerator.Reset();
            LoadTemplate();
        }

        private static void LoadTemplate()
        {
            var templatePath = "Assets/Editor/GridScriptTemplate.txt";
            if (!File.Exists(templatePath))
            {
                Debug.LogError("Template file not found");
                return;
            }
            scriptTemplateString = File.ReadAllText(templatePath);
            LoadFromString(scriptTemplateString);
            selectedTileType = tileTypes[0];
            Debug.Log("Template loaded successfully");
        }

        private void OnProjectChange()
        {
            UniqueCharGenerator.Reset();
            LoadTemplate();
        }
    }
}