using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
	//Public Variables (Visible in Inspector)
	
	[Header("Tile Spawning")]
	public static TileManager instance;
	public int maxBuildingWidth = 5;
	public int minBuildingWidth = 2;
	public int maxBuildingTileCount = 24;
	public int minBuildingTileCount = 8;
	public int numberOfBuildings = 3;
	public float levelSpacing = 1000;
	public float minOffset;
	public float maxOffset;
	
	[Header("Interactable Spawning")]
	public string teleporterID = "Teleporter_Mock2";
	public float godHeadSpawnChance = .2f;
	public float chestSpawnChance = .2f;
	public float merchantSpawnChance = .2f;
	public float scrapConverterSpawnChance = .2f;
	[Header("Teleporter Spawning")]
	public float teleSpawnChance = .03f;
	public int DontSpawnUntilAfter = 30;//the minimum amount of cubbies that must be passed before a teleporter can spawn
	public float ForceTeleSpawnDist = 15; // a teleporter mat less than this distance away from the end building will forcibly spawn a teleporter
	[Header("Prefab Containers")]
	private GameObject[] tilePrefabs;
	public GameObject[] industrialTilePrefabs;
	public GameObject[] cityTilePrefabs;
	public GameObject[] enemyPrefabs;
	public GameObject[] interactablePrefabs;
	public GameObject levelEndCityBuilding;
	public GameObject levelEndIndustrialBuilding;
	
	[Header("Enemy Spawning")]
	public Vector2 MinMaxEnemies = new Vector2(3, 5);
	[Range(0, 1)]
	public float SpawnChance = .1f;
	public int playerOnlevel = 0;
	//Private Variables
	private float currentPos = 22.0f;
	private float tileLength = 22.0f;
	private float tileHeight = 6.5f;
	private int seedValue;
	private float levelTracking;
	private List<GameObject> endBuildings;
	private List<GameObject> teleporterInstances;
	private void Awake()
	{
		if (instance == null)
			instance = this;
		else
		{
			Destroy(gameObject);
			return;
		}
	}
	private void Start()
    {
		endBuildings = new List<GameObject>();
		tilePrefabs = industrialTilePrefabs;
		//tilePrefabs = cityTilePrefabs;
		if (SaveLoadManager.instance.newGame == true)
		{

			seedValue = Random.Range(0, 9999999);
			Random.InitState(seedValue);
		}
		else
		{
			seedValue = SaveLoadManager.instance.LoadMapData().seed;
		}
		//Level 1
		int level = 1;
		currentPos = levelSpacing * level + 22;
		for (int i = 0; i < numberOfBuildings; ++i)
		{
			generateBuilding(Random.Range(minBuildingWidth, maxBuildingWidth), Random.Range(minBuildingTileCount, maxBuildingTileCount), level);
		}
		Vector3 spawnVector = new Vector3(1, 0, 0) * currentPos + new Vector3(0, -2, 0); //spawnVector for tiles
		GameObject endBuilding = (GameObject)GameObject.Instantiate(levelEndCityBuilding, spawnVector, Quaternion.Euler(0, 141.6f, 0));
		
		//tilePrefabs = industrialTilePrefabs;
		tilePrefabs = cityTilePrefabs;
		//Level 2
		++level;
		currentPos = levelSpacing*level + 22;
		for (int i = 0; i < numberOfBuildings; ++i)
		{
			generateBuilding(Random.Range(minBuildingWidth, maxBuildingWidth), Random.Range(minBuildingTileCount, maxBuildingTileCount), level);
		}
		spawnVector = new Vector3(1, 0, 0) * currentPos + new Vector3(0, -2, 0); //spawnVector for tiles
		GameObject endBuilding2 = (GameObject)GameObject.Instantiate(levelEndCityBuilding, spawnVector, Quaternion.Euler(0, 141.6f, 0));
		//Level 3
		//++level;
		//currentPos = levelSpacing*level + 22;
		//for (int i = 0; i < numberOfBuildings; ++i)
		//{
		//generateBuilding(Random.Range(minBuildingWidth, maxBuildingWidth), Random.Range(minBuildingTileCount, maxBuildingTileCount), level);
		//}

		//Placemat PCG Pass
		levelTracking = levelSpacing;
		//bool AllTeleportersSpawned = false;
		teleporterInstances = new List<GameObject>();
		int curMatLevel = Mathf.RoundToInt(levelSpacing/1000);
		int LastMatLevel = 0;
		GameObject newestTele = null;

		foreach (GameObject mat in GameObject.FindGameObjectsWithTag("Placemat"))
		{
			if (mat.name == "GodHeadMat" && Random.value <= godHeadSpawnChance) //Case 1: God Heads
			{
				GameObject newGodHead = Instantiate(interactablePrefabs[0]) as GameObject;
				newGodHead.transform.position = mat.transform.position;
			}
			else if (mat.name == "ChestMat" && Random.value <= chestSpawnChance) //Case 2: Chests/Safes
			{
				GameObject newChest = Instantiate(interactablePrefabs[1]) as GameObject;
				newChest.transform.position = mat.transform.position;
			}
			else if (mat.name == "MerchantMat" && Random.value <= merchantSpawnChance) //Case 3: Merchants
			{
				GameObject newMerchant = Instantiate(interactablePrefabs[2]) as GameObject;
				newMerchant.transform.position = mat.transform.position;
			}
			else if (mat.name == "ScrapMat" && Random.value <= scrapConverterSpawnChance) //Case 4: Scrap Convertors
			{
				GameObject newScrap = Instantiate(interactablePrefabs[3]) as GameObject;
				newScrap.transform.position = mat.transform.position;
			}
			else if (mat.name == "TeleportMat")
			{
				curMatLevel = Mathf.FloorToInt(mat.transform.position.x / levelSpacing);
				if (curMatLevel > LastMatLevel)
				{ //guarantee the creation of a teleporter on each level
					newestTele = NewTeleporter(mat);
					print("new tele: "+ newestTele.name);
					print("curmatlevel" + curMatLevel);
				}
				if (Random.value <= teleSpawnChance)
				{ //if it passes the spawnchance, update the position
					newestTele.transform.position = mat.transform.position;
				}
				LastMatLevel = curMatLevel;
			}
			//if (newestTele != null && newestTele.GetComponent<Teleporter>().BossRoomOverride == true)
					//AllTeleportersSpawned = true; // if a teleporter with bossroom override has spawned, there are no more portals to make
			
		}
		//this should be turned on later to disable the prefab gameobject
		//portalPrefab.SetActive(false);
    }
	public int GetSeed()
	{
		return seedValue;
	}
	private GameObject NewTeleporter(GameObject mat)
	{
		GameObject newTele = Instantiate(interactablePrefabs[4]) as GameObject;
		teleporterInstances.Add(newTele);
		if (teleporterInstances.Count < 2)
		{
			newTele.name = "Teleporter in Level " + (teleporterInstances.Count);
			levelTracking += levelSpacing;
			newTele.GetComponent<Teleporter>().SetDestination(new Vector2(levelTracking + 5, 40));
			teleporterInstances[teleporterInstances.Count - 1].transform.position = mat.transform.position;
		}
		else
		{
			newTele.name = "Teleporter to Boss Room";
			teleporterInstances[teleporterInstances.Count - 1].transform.position = mat.transform.position;
			newTele.GetComponent<Teleporter>().BossRoomOverride = true; //makes this TP go to boss room instead
		}
		return newTele;
	}
	public void SetSeed(int num)
	{
		seedValue = num;
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
			if (newTile.GetComponent<MeshRenderer>() == null)
			{
				MeshCombiner combiner = newTile.AddComponent<MeshCombiner>();
				combiner.CreateMultiMaterialMesh = true;
				combiner.DeactivateCombinedChildrenMeshRenderers = true;
				combiner.DeactivateCombinedChildren = true;
				combiner.CombineMeshes(false);
				newTile.GetComponent<MeshRenderer>().receiveShadows = false;
				newTile.transform.SetParent(transform);
				newTile.transform.localRotation = Quaternion.Euler(0, 180, 0);
			}

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
			if (Random.value <= SpawnChance)
			{
				int i = Random.Range((int)MinMaxEnemies.x, (int)MinMaxEnemies.y+levelNumber-1);
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
			
			//Iterate counting variables
			shift--;
			end--;
		}
		
		//Shift currentPos for next building
		currentPos += tileLength * width;
		currentPos += Random.Range(8.0f, 15.0f);
	}
}
