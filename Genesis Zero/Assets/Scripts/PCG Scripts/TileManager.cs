using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
	//Public Variables (Visible in Inspector)
	public int maxBuildingWidth = 5;
	public int minBuildingWidth = 2;
	public int maxBuildingTileCount = 24;
	public int minBuildingTileCount = 8;
	public int numberOfBuildings = 3;
	public int levelSpacing = 1000;
	public float minOffset;
	public float maxOffset;
	public string teleporterID = "Teleporter_Mock2";
	public GameObject[] tilePrefabs;
	public GameObject[] enemyPrefabs;
	
	//Private Variables
	private float currentPos = 22.0f;
	private float tileLength = 22.0f;
	private float tileHeight = 6.5f;
	
	
    // Start is called before the first frame update
    private void Start()
    {
		//Level 1
		int level = 0;
        for (int i = 0; i < numberOfBuildings; ++i)
		{
			generateBuilding(Random.Range(minBuildingWidth, maxBuildingWidth), Random.Range(minBuildingTileCount, maxBuildingTileCount), level);
		}
		
		//Level 2
		teleporterID = "Teleporter_Mock3";
		++level;
		currentPos = levelSpacing*level + 22;
		for (int i = 0; i < numberOfBuildings; ++i)
		{
			generateBuilding(Random.Range(minBuildingWidth, maxBuildingWidth), Random.Range(minBuildingTileCount, maxBuildingTileCount), level);
		}
		
		//Level 3
		teleporterID = "Teleporter_Mock4";
		++level;
		currentPos = levelSpacing*level + 22;
		for (int i = 0; i < numberOfBuildings; ++i)
		{
			generateBuilding(Random.Range(minBuildingWidth, maxBuildingWidth), Random.Range(minBuildingTileCount, maxBuildingTileCount), level);
		}
    }

    // Update is called once per frame
    private void Update()
    {
        
    }
	
	private void generateBuilding(int width, int TileCount, int levelNumber)
	{
		//Variables
		Vector3 spawnVector = new Vector3(1,0,0) * currentPos; //spawnVector for tiles
		float currentPosClone = currentPos; //Function scope copy of currentPos
		int end = TileCount; // Equals 0 when all tiles are placed
		int floorWidth = width; //Total width of current floor in tiles (x axis)
		int shift = width; // Equals 0 when floor is complete
		int height = 1; //Current building height (# of floors)
		
		//Create Building
		spawnVector.z += 2; //Initialize spawnVector Z position
		
		while (end > 0)
		{
			//Create new tile and set parent to TileManager
			GameObject newTile;
			newTile = Instantiate(tilePrefabs[Random.Range(0, tilePrefabs.Length)]) as GameObject;
			newTile.transform.SetParent(transform);
			
			//Transform Position & Update
			if (shift <= 0)
			{
				//Randomly Select Case (0-4)
				int tileRand;
				float offset = Random.Range(minOffset,maxOffset);
				if (floorWidth > 3) tileRand = Random.Range(0, 4);
				else tileRand = 0;
				
				if (tileRand == 0) shift = floorWidth; // Case 0
				else if (tileRand == 1) //Case 1
				{
					floorWidth -= 1;
					shift = floorWidth;
				}
				else if (tileRand == 2) //Case 2
				{
					//Disabled due to unintended behavior
					//floorWidth -= 2;
					//shift = floorWidth;
					//currentPosClone += Random.Range(0.0f, 44.0f)
					
					//Replacement Behavior
					floorWidth -= 1;
					shift = floorWidth;
				}
				else if (tileRand == 3) //Case 3
				{
					floorWidth -= 1;
					shift = floorWidth;
					currentPosClone += tileLength;
				}
				
				//Set spawnVector for next floor
				spawnVector = new Vector3(1,0,0) * currentPosClone;
				spawnVector.x += offset;
				spawnVector.y += tileHeight * height;
				spawnVector.z += 2;
				height++;
			}
			//Spawn tile and move spawnVector
			newTile.transform.position = spawnVector;
			spawnVector.x += tileLength;
			
			//Spawn Enemy
			if (Random.Range(0, 5) == 0)
			{
				int i = Random.Range(5, 9);
				spawnVector.y += 3;
				spawnVector.x -= 11;
				spawnVector.z -= 2;
				while (i > 0)
				{
					GameObject newEnemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)]) as GameObject;
					newEnemy.transform.position = spawnVector;
					--i;
				}
				
				spawnVector.y -= 3;
				spawnVector.x += 11;
				spawnVector.z += 2;
			}
			
			//Spawn Teleporter
			if (Random.Range(0, 10) == 0)
			{
				GameObject exitPortal = GameObject.Find(teleporterID);
				spawnVector.y += 7;
				spawnVector.x -= 22;
				spawnVector.z -= 2;
				exitPortal.transform.position = spawnVector;
				spawnVector.y -= 7;
				spawnVector.x += 22;
				spawnVector.z += 2;
			}
			
			//Iterate counting variables
			shift--;
			end--;
		}
		
		//Shift currentPos for next building
		currentPos += tileLength * width;
		currentPos += Random.Range(10.0f, 30.0f);
	}
}
