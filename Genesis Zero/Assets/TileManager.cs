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
	public GameObject[] tilePrefabs;
	public GameObject[] enemyPrefabs;
	
	//Private Variables
	private float currentPos = 22.0f;
	private float tileLength = 22.0f;
	private float tileHeight = 6.5f;
	
	
    // Start is called before the first frame update
    private void Start()
    {
        for (int i = 0; i < numberOfBuildings; ++i)
		{
			generateBuilding(Random.Range(minBuildingWidth, maxBuildingWidth), Random.Range(minBuildingTileCount, maxBuildingTileCount));
		}
    }

    // Update is called once per frame
    private void Update()
    {
        
    }
	
	private void generateBuilding(int width, int TileCount)
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
				if (floorWidth > 2) tileRand = Random.Range(0, 4);
				else tileRand = 0;
				
				if (tileRand == 0) shift = floorWidth; // Case 0
				else if (tileRand == 1) //Case 1
				{
					floorWidth -= 1;
					shift = floorWidth;
				}
				else if (tileRand == 2) //Case 2
				{
					//Temporarily disabled due to unintended behavior
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
					currentPosClone += 22.0f;
				}
				
				//Set spawnVector for next floor
				spawnVector = new Vector3(1,0,0) * currentPosClone;
				spawnVector.y += tileHeight * height;
				spawnVector.z += 2;
				height++;
			}
			//Spawn tile and move spawnVector
			newTile.transform.position = spawnVector;
			spawnVector.x += tileLength;
			
			//Spawn Enemy
			if (Random.Range(0, 3) == 0)
			{
				GameObject newEnemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)]) as GameObject;
				spawnVector.y += 3;
				spawnVector.x -= 11;
				spawnVector.z -= 2;
				newEnemy.transform.position = spawnVector;
				spawnVector.y -= 3;
				spawnVector.x += 11;
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
