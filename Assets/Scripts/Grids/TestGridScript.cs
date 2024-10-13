using System.Collections.Generic;
using UnityEngine;

namespace Grids
{
    public class TestGridScript : GridScript
    {
        
        string[] gridString = new string[]{
            //RE_MARK_0_START
            "-ww-|-wr-rrrr-|---rrrr-----",
            "-ww-|----r----|------r--www",
            "-ww-|----r----|------r--ww-",
            "-ww-|----rrrrrrrrrrrr--dww-",
            "-wwwwwwww|----|-------dww--",
            "-wwwwwwwwww---|------d-----",
            "----|---www---|-----dd-----",
            "----|---www---|----dd------",
            "--ddd----w--www---dd-------",
            "--drd----wwww-w--dd--------",
            "--drd----|----wwdd---------",
            "--dddddd-|----|wdd---------",
            "----dddddd----|------------",
            "----|---dd----|------------",
            //RE_MARK_0_END
        };

        private Dictionary<char, Material> materialDictionary = new Dictionary<char, Material>();

        private Dictionary<char, string> materialPathDictionary = new Dictionary<char, string>()
        {
            //RE_MARK_1_START
            {'w', "_Core/Materials/Water"},
            {'r', "_Core/Materials/Rock"},
            {'-', "_Core/Materials/Grass"},
            {'d', "_Core/Materials/Rock"},
            {'|', "_Core/Materials/Forest"},
            //RE_MARK_1_END
        };
        
        private Dictionary<char, float> costDictionary = new Dictionary<char, float>()
        {
            //RE_MARK_2_START
            {'w', 1.34f},
            {'r', 1},
            {'d', 10},
            {'-', 1},
            {'|', 1},
            //RE_MARK_2_END
        };

        // Use this for initialization
        void Start () {
            LoadMaterials();
            gridWidth = gridString[0].Length;
            gridHeight = gridString.Length;
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