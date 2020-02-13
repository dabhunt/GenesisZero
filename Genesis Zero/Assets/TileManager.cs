using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
	//public float Zoffset;
	public GameObject[] tilePrefabs;
	
	private float currentPos = 22.0f;
	private float tileLength = 22.0f;
	private float tileHeight = 6.5f;
	
	
    // Start is called before the first frame update
    private void Start()
    {
        generateBuilding(3, 16);
		generateBuilding(2, 8);
		generateBuilding(3, 12);
    }

    // Update is called once per frame
    private void Update()
    {
        
    }
	
	private void generateBuilding(int width, int TileCount)
	{
		//Variables
		Vector3 spawnVector = new Vector3(1,0,0) * currentPos;
		float currentPosClone = currentPos;
		int end = TileCount; // Equals 0 when all tiles are placed
		int floorWidth = width;
		int shift = width; // Equals 0 when floor is complete
		int height = 1;
		
		//Create Building
		spawnVector.z += 2;
		
		while (end > 0)
		{
			//Create new tile and set parent to TileManager
			GameObject newTile;
			newTile = Instantiate(tilePrefabs[Random.Range(0, tilePrefabs.Length)]) as GameObject;
			newTile.transform.SetParent(transform);
			
			//Transform Position & Update
			if (shift <= 0)
			{
				int binRand;
				if (floorWidth > 2) binRand = Random.Range(0, 4);
				else binRand = 0;
				
				if (binRand == 0) shift = floorWidth;
				if (binRand == 1)
				{
					floorWidth -= 1;
					shift = floorWidth;
				}
				if (binRand == 2)
				{
					floorWidth -= 2;
					shift = floorWidth;
					currentPosClone += Random.Range(0.0f, 44.0f);
				}
				if (binRand == 3)
				{
					floorWidth -= 1;
					shift = floorWidth;
					currentPosClone += 22.0f;
				}
				
				spawnVector = new Vector3(1,0,0) * currentPosClone;
				spawnVector.y += tileHeight * height;
				spawnVector.z += 2;
				height++;
			}
			newTile.transform.position = spawnVector;
			spawnVector.x += tileLength;
			
			shift--;
			end--;
		}
		
		//Shift currentPos
		currentPos += tileLength * width;
		currentPos += Random.Range(10.0f, 30.0f);
	}
}
