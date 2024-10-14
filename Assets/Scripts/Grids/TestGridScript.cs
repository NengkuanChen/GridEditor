using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Grids
{
    public class TestGridScript: GridScript
    {
        
        string[] gridString = new string[]{
            //RE_MARK_0_START
			"-ww-|-wr-rrrr-|---rrrr-----",
			"-ww-|----r----|------r--www",
			"-ww-|----r----|------r--ww-",
			"-ww-|----rrrrrrrrrrrr--dww-",
			"-wwwwwwwwdddd-ddd-----dww--",
			"-wwwwwwwwdw---|--ddd-d-----",
			"----|---wdw---|-----dd-----",
			"----|---wwddd-|ddd-dd-ddd--",
			"--ddd----w--www---dd--d-d--",
			"--drd----wwww-w--wdd--d-dd-",
			"--drd----|----wwdd-d-d--dd-",
			"--dddddd-|----|wdd-ddd-d-d-",
			"----dddddd----|-----d-dd-d-",
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
			{'r', 1f},
			{'-', 1f},
			{'d', 10f},
			{'|', 1f},
			//RE_MARK_2_END

        };

        // Use this for initialization
        void Start () {
            LoadMaterials();
            gridWidth = gridString[0].Length;
            gridHeight = gridString.Length;
            gridArray = GetGrid();
            Debug.Log($"width: {gridWidth}, height: {gridHeight}");
        }

        void UpdateTile(int x, int y)
        {
	        Destroy(gridArray[x, y]);
	        
	        float offsetX = (gridWidth  * -spacing)/2f;
	        float offsetY = (gridHeight * spacing)/2f;
	        
	        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
	        quad.transform.localScale = new Vector3(spacing, spacing, spacing);
	        quad.transform.position = new Vector3(offsetX + x * spacing, 
		        offsetY - y * spacing, 0);

	        quad.transform.parent = transform;

	        gridArray[x, y] = quad;
					
	        quad.GetComponent<MeshRenderer>().sharedMaterial = GetMaterial(x, y);
        }

        // Change the tile on (x, y) according to the char indicator
        public void ChangeTile(int x, int y, char indicator)
        {
	        StringBuilder sb = new StringBuilder(gridString[y]);
	        sb[x] = indicator;
	        Debug.Log(gridString[y] + "\n changed to :\n" + sb.ToString());
	        gridString[y] = sb.ToString();
	        UpdateTile(x, y);
        }
        
        // Change the tile on (x, y) to a random material
        public void RandomChangeTile(int x, int y)
        {
	        Debug.Log("KK");
	        var keys = materialDictionary.Keys.ToArray();
	        char key = keys[Random.Range(0, keys.Length)];
	        ChangeTile(x, y, key);
        }

        void Update()
        {
	        // if (Input.GetKeyDown(KeyCode.K))
		       //  RandomChangeTile(0,0);
	        // if(Input.GetKeyDown(KeyCode.J))
		       //  RandomChangeTile(2,2);
	        // if(Input.GetKeyDown(KeyCode.L))
		       //  RandomChangeTile(gridWidth - 1,gridHeight-1);
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