using System.Collections.Generic;
using UnityEngine;

namespace Grids
{
    public class AlleyTestGrid2: GridScript
    {
        
        string[] gridString = new string[]{
            //RE_MARK_0_START
			"000000000000000000000000000",
			"000000000ccc000000000000000",
			"000000c00000000000000000000",
			"0000000cc000000000000000000",
			"0000c000000c0c00c00c0000000",
			"000000000000000000000000000",
			"000000000000000000000000000",
			"000000000000000000000000000",
			"000000000000000000000000000",
			"000000000000000000000000000",
			"000000000000000000000000000",
			"000000000000000000000000000",
			"000000000000000000000000000",
			"000000000000000000000000000",
			//RE_MARK_0_END

        };

        private Dictionary<char, Material> materialDictionary = new Dictionary<char, Material>();

        private Dictionary<char, string> materialPathDictionary = new Dictionary<char, string>()
        {
            //RE_MARK_1_START
			{'0', "_Core/Materials/Grass"},
			{'b', "Assets/Material/DabuPathMaterial"},
			{'c', "Assets/Students/_AllisonTerry/AllisonMaterial"},
			//RE_MARK_1_END

        };
        
        private Dictionary<char, float> costDictionary = new Dictionary<char, float>()
        {
            //RE_MARK_2_START
			{'0', 0.1f},
			{'b', 1f},
			{'c', 5f},
			//RE_MARK_2_END

        };

        // Use this for initialization
        void Start () {
            LoadMaterials();
            gridWidth = gridString[0].Length;
            gridHeight = gridString.Length;
            Debug.Log($"width: {gridWidth}, height: {gridHeight}");
        }

        public void LoadMaterials()
        {
            foreach (var pair in materialPathDictionary)
            {
                Material mat = Resources.Load<Material>(pair.Value);
                materialDictionary.Add(pair.Key, mat);
            }
        }
	
        protected override Material GetMaterial(int x, int y){

            char c = gridString[y].ToCharArray()[x];
            return materialDictionary[c];
        }

        public override float GetMovementCost(GameObject go)
        {
            Vector2Int girdPosition = WorldToGridPosition(go.transform.position);
            char c = gridString[girdPosition.y][girdPosition.x];
            return costDictionary[c];
        }
        
        public Vector2Int WorldToGridPosition(Vector3 worldPosition)
        {
            float offsetX = (gridWidth * -spacing) / 2f;
            float offsetY = (gridHeight * spacing) / 2f;

            int x = Mathf.RoundToInt((worldPosition.x - offsetX) / spacing);
            int y = Mathf.RoundToInt((offsetY - worldPosition.y) / spacing);

            return new Vector2Int(x, y);
        }
    }
}