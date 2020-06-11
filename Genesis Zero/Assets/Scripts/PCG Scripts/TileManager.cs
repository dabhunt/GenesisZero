using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
	//Public Variables (Visible in Inspector)
	[Header("Tile Spawning")]
	public static TileManager instance;
	public bool MayhemMode = false;
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
	public float guideArrowSpawnChance = .12f;
	private float[] interactableSpawnChances;
	[Header("Teleporter Spawning")]
	public float teleSpawnChance = .03f;
	public int dontSpawnUntilAfter = 30;//the minimum amount of cubbies that must be passed before a teleporter can spawn
	//public float ForceTeleSpawnDist = 15; // a teleporter mat less than this distance away from the end building will forcibly spawn a teleporter
	[Header("Prefab Containers")]
	private GameObject[] tilePrefabs;
	public GameObject[] industrialTilePrefabs;
	public GameObject[] cityTilePrefabs;
	public GameObject[] rooftopPrefabs;
	public GameObject[] enemyPrefabs;
	public GameObject[] interactablePrefabs;
	public GameObject levelEndCityBuilding;
	public GameObject levelEndIndustrialBuilding;
	
	[Header("Enemy Spawning")]
	public Vector2 MinMaxEnemies = new Vector2(1, 2);
	public Vector2 MinMaxClusterDistance = new Vector2(4, 7); //min guarantees enemy clusters can't be closer than this, Max guarantees they can't be further than this
	[Range(0, 1)]
	public float SpawnChance = .1f;
	public int playerOnlevel = 0;
	public TemporaryTextDisplay tempTextDisplay;
	//Private Variables
	private float currentPos = 22.0f;
	private float tileLength = 22.0f;
	private float tileHeight = 6.5f;
	private int seedValue;
	private float levelTracking, enemyspawnchanceincease;
	private List<GameObject> endBuildings;
	private List<GameObject> teleporterInstances;
	private GameObject[] betweenAreas;
	private Sprite[] ads;
	private GameObject LevelParent;
	List<List<GameObject>> guideArrows = new List<List<GameObject>>();
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
		betweenAreas = Resources.LoadAll<GameObject>("InbetweenAreas");
		ads = Resources.LoadAll<Sprite>("Billboards");
		SkillObject[] sskills = Resources.LoadAll<SkillObject>("Skills/Starter Mods");
		guideArrows.Add(new List<GameObject>());//level 1 arrows
		guideArrows.Add(new List<GameObject>());//level 2
		interactableSpawnChances = new[]{ godHeadSpawnChance, chestSpawnChance, merchantSpawnChance, scrapConverterSpawnChance, teleSpawnChance, guideArrowSpawnChance};
		endBuildings = new List<GameObject>();
		LevelParent = new GameObject("LevelParentInstance");
		LevelParent.transform.SetParent(transform);
		
		tilePrefabs = industrialTilePrefabs;
		
		if (SaveLoadManager.instance.newGame == true)
		{
			seedValue = Random.Range(0, 999999);
			Random.InitState(seedValue);
		}
		else
		{
			seedValue = SaveLoadManager.instance.LoadMapData().seed;
		}
		
		if (!MayhemMode)
		{
			//Level 1
			int level = 1;
			currentPos = levelSpacing * level + 22;
			for (int i = 0; i < numberOfBuildings; ++i)
			{
				generateBuilding(Random.Range(minBuildingWidth, maxBuildingWidth), Random.Range(minBuildingTileCount, maxBuildingTileCount), level);
			}
			Vector3 spawnVector = new Vector3(1, 0, 0) * currentPos + new Vector3(0, -2, 0); //spawnVector for tiles
			GameObject endBuilding = (GameObject)GameObject.Instantiate(levelEndCityBuilding, spawnVector, Quaternion.Euler(0, 141.6f, 0));

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
			PlaceInteractables();
		}
		else
		{
			//Mayhem Mode	

			Invoke("DelayedStart", .5f);
			//mayhemLevelUp();
			mayhemLevelUp();
		}

		//Placemat PCG Pass

    }
	private void DelayedStart()
	{
		//StateManager.instance.Teleport( new Vector2(levelSpacing + 5,40) , false, true);
		StateManager.instance.SkipTutorial();
		tempTextDisplay. ShowText("Clear as many levels as possible without dying", 5, .75f);
	}
	public int GetSeed()
	{
		return seedValue;
	}
	public void PlaceInteractables()
	{
		levelTracking = levelSpacing;
		//bool AllTeleportersSpawned = false;
		teleporterInstances = new List<GameObject>();
		int curMatLevel = 0;
		print("placing Interactables");
		int LastMatLevel = 0;
		GameObject newestTele = null;
		int iter = 0;
		foreach (GameObject mat in GameObject.FindGameObjectsWithTag("Placemat"))
		{
			curMatLevel = Mathf.FloorToInt(mat.transform.position.x / levelSpacing); //convert X position to what level we are on
																					 //print("mat.transform: " + mat.transform.position.x + " = curmatlevel: " + curMatLevel);
			if (mat.transform.position.x > levelSpacing)
			{
				if (mat.name == "GodHeadMat" && Random.value <= godHeadSpawnChance) //Case 1: God Heads
				{
					GameObject newGodHead = Instantiate(interactablePrefabs[0]) as GameObject;
					newGodHead.transform.position = mat.transform.position;
					newGodHead.transform.SetParent(mat.transform.parent);
				}
				else if (mat.name == "ChestMat" || mat.name == "MerchantMat" || mat.name == "ScrapMat")
				{
					int rng = Random.Range(1, 5); //get num from 1 to 4 to choose what may spawn here, since all are roughly the same space requirement
					if (Random.value <= interactableSpawnChances[rng])
					{
						//if it didn't pass the random check, try again
						GameObject newInteractable;
						if (rng == 4)
						{
							newInteractable = Instantiate(interactablePrefabs[5]) as GameObject; // replace with Guidance Arrow
							newInteractable.name = "Guidance Arrow";
							newInteractable.transform.position = mat.transform.position + new Vector3(0, 2, -2);
							guideArrows[curMatLevel - 1].Add(newInteractable); //keep track of guide arrows so that they can point at the teleporter properly
						}
						else
						{
							newInteractable = Instantiate(interactablePrefabs[rng]) as GameObject;
							newInteractable.transform.position = mat.transform.position;
						}

						newInteractable.transform.SetParent(mat.transform.parent);
					}
				}
				else if (mat.name == "TeleportMat")
				{
					//print("curmatlevel: " + curMatLevel);
					if (curMatLevel > LastMatLevel)//guarantees the creation of a teleporter on each level
					{
						if (curMatLevel > 1)
						{
							foreach (GameObject arrow in guideArrows[curMatLevel - 2])
							{
								//print("curmatlevel: " + curMatLevel);
								if (arrow != null)
									arrow.transform.LookAt(newestTele.transform); //make all arrows point at the tele for the level just finished
							}
						}
						newestTele = NewTeleporter(mat);
						iter = 0; //reset iter upon reaching the next level
					}
					//if less than this number of cubbies have been checked, force the teleporter to have it's position updated regardless of rng.
					//this prevents the teleporter from ever spawning in the first building.
					if (iter < dontSpawnUntilAfter || Random.value <= teleSpawnChance)
					{
						newestTele.transform.position = new Vector3(mat.transform.position.x, mat.transform.position.y, 0);
					}
					LastMatLevel = curMatLevel;
				}
				iter++; //keep track of how many cubbies are checked
			}
		}
		//the final level of PCG needs this for the guidance arrows
		foreach (GameObject tele in GameObject.FindGameObjectsWithTag("Teleporter"))
		{
			if (tele.name.Contains("to Boss"))
			{
				int z = 0;
				//print("level 2 tele found");
				foreach (GameObject arrow in guideArrows[1])
				{
					//print("Arrow " + z); 
					if (arrow != null)
						arrow.transform.LookAt(newestTele.transform); //make all arrows point at the tele for the level just finished
					z++;
				}
			}
		}
	}
	private GameObject NewTeleporter(GameObject mat)
	{
		if(!MayhemMode)
		{
			GameObject newTele = Instantiate(interactablePrefabs[4]) as GameObject;
			teleporterInstances.Add(newTele);
			if (teleporterInstances.Count < 2)
			{
				newTele.name = "Teleporter in Level " + (teleporterInstances.Count);
				levelTracking += levelSpacing;
				newTele.GetComponent<Teleporter>().SetDestination(new Vector2(levelTracking + 2, 4));
				teleporterInstances[teleporterInstances.Count - 1].transform.position = mat.transform.position;
			}
			else
			{
				newTele.name = "Teleporter to Boss Room";
				teleporterInstances[teleporterInstances.Count - 1].transform.position = mat.transform.position;
				newTele.GetComponent<Teleporter>().BossRoomOverride = true; //makes this TP go to boss room instead
			}
			newTele.transform.SetParent(LevelParent.transform);
			return newTele;
		}
		else
		{
			print("teleporter instances: " + teleporterInstances);
			if (teleporterInstances.Count < 1)
			{
				GameObject newTele = Instantiate(interactablePrefabs[4]) as GameObject;
				teleporterInstances.Add(newTele);
				newTele.name = "Mayhem Teleporter";
				//newTele.transform.SetParent(LevelParent.transform);
				teleporterInstances.Add(newTele);
			}
			teleporterInstances[teleporterInstances.Count - 1].GetComponent<Teleporter>().SetDestination(new Vector2(levelSpacing + 2, 5));
			teleporterInstances[teleporterInstances.Count - 1].transform.position = mat.transform.position;
			teleporterInstances[teleporterInstances.Count - 1].transform.SetParent(LevelParent.transform);
			return teleporterInstances[teleporterInstances.Count - 1];
		}
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
		int tilesSinceLastCluster = 0;
		List<GameObject> roofList = new List<GameObject>();
		List<Vector3> vectorList = new List<Vector3>();
		
		//Create Building

		spawnVector.z += 2; //Initialize spawnVector Z position
		while (end > 0)
		{
			//Create new tile and set parent to TileManager
			GameObject newTile;
			newTile = Instantiate(tilePrefabs[Random.Range(0, tilePrefabs.Length)]) as GameObject;
			//newTile = Instantiate(tilePrefabs[0]) as GameObject;
			if (newTile.GetComponent<MeshRenderer>() == null)
			{
				MeshCombiner combiner = newTile.AddComponent<MeshCombiner>();
				combiner.CreateMultiMaterialMesh = true;
				combiner.DeactivateCombinedChildrenMeshRenderers = true;
				combiner.DeactivateCombinedChildren = true;
				combiner.CombineMeshes(false);
				newTile.GetComponent<MeshRenderer>().receiveShadows = false;
				newTile.transform.localRotation = Quaternion.Euler(0, 180, 0);
			}
			newTile.transform.SetParent(LevelParent.transform);
			//Transform Position & Update
			if (shift <= 0)
			{
				//Randomly Select Case (0-4)
				int tileRand;
				float offset = Random.Range(minOffset, maxOffset);
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
				spawnVector = new Vector3(1, 0, 0) * currentPosClone;
				spawnVector.x += offset;
				spawnVector.y += tileHeight * height;
				spawnVector.z += 2;
				height++;
			}

			if (tilePrefabs == cityTilePrefabs)
			{
				//Instantiate & Spawn Rooftop
				GameObject newRooftop = Instantiate(rooftopPrefabs[Random.Range(0, rooftopPrefabs.Length)]) as GameObject;
				newRooftop.transform.SetParent(LevelParent.transform);
				spawnVector.z += 5.2f;
				newRooftop.transform.position = spawnVector;
				Vector3 newVector = newRooftop.transform.position;
				spawnVector.z -= 5.2f;
				// Lists of rooftop objects and building spawn vectors
				//(Used for rooftop cleanup later)
				roofList.Add(newRooftop);
				vectorList.Add(newVector);
			}
			//Spawn tile and move spawnVector
			newTile.transform.position = spawnVector;

			spawnVector.x += tileLength;
			bool clusterSpawned = false;
			//Spawn Enemy
			int amount = 0;
			if (tilesSinceLastCluster > MinMaxClusterDistance.x && tilesSinceLastCluster < MinMaxClusterDistance.y) //if it's within our constraints, use randomness
			{ 
				if (Random.value <= (SpawnChance + enemyspawnchanceincease))
				{
					clusterSpawned = true;
				}
			} 
			else if (tilesSinceLastCluster >= MinMaxClusterDistance.y)
			{
				clusterSpawned = true;
			}
			if (clusterSpawned) //if an enemy cluster spawned in this tile
			{
				if (MayhemMode)
					amount = Random.Range((int)MinMaxEnemies.x, (int)MinMaxEnemies.y + MayhemTimer.instance.GetLevelsCleared() - 1);
				else
					amount = Random.Range((int)MinMaxEnemies.x, (int)MinMaxEnemies.y + levelNumber - 1);
				spawnVector = SpawnEnemy(amount, spawnVector);
				enemyspawnchanceincease = 0;
				tilesSinceLastCluster = 0;
			}
			else // if an enemy cluster did not spawn in this tile
			{
				enemyspawnchanceincease += .05f;
				tilesSinceLastCluster++;
			}
			//Iterate counting variables
			shift--;
			end--;
		}
		
		//Remove extra rooftops
		if (tilePrefabs == cityTilePrefabs)
		{
			for (int i = 0; i < vectorList.Count; ++i)
			{
				GameObject roofObject = roofList[i];
				Vector3 roofVector = roofObject.transform.position;
			
				int j = i + 1;
				while (j < vectorList.Count)
				{
					if (Vector3.Distance(roofVector, vectorList[j]) < 7.0)
					{
						Destroy(roofObject);
						j = vectorList.Count;
					}
					++j;
				}
			}
		}
		//Shift currentPos for next building
		currentPos += tileLength * width;
		float oldBuildingX = currentPos;
		float spacing = Random.Range(8.0f, 15.0f);//where to start the next building
		float nextBuildingStart = currentPos + spacing;
		float randMulti = Random.Range(.45f, .55f);
		float sOffset = (spacing  * randMulti);
		int rng = Random.Range(0, betweenAreas.Length - 1);
		GameObject areaObj = Instantiate(betweenAreas[rng]) as GameObject;
		areaObj.transform.SetParent(LevelParent.transform);
		InbetweenArea area = areaObj.GetComponent<InbetweenArea>();
		areaObj.transform.position = new Vector3(oldBuildingX - sOffset , Random.Range(0, area.offset.y), Random.Range(10, area.offset.z)); // randomize location somewhat
		foreach (GameObject bill in area.billboards)
		{
			bill.GetComponent<SpriteRenderer>().color = area.AcceptableColors[Random.Range(0, area.AcceptableColors.Length - 1)]; //get random color from acceptable color list of colors
			bill.GetComponent<SpriteRenderer>().sprite = ads[Random.Range(0, ads.Length - 1)]; //set sprite to a random image in the billboards folder
		}
		//areaObj = VFXManager.instance.ChangeColor(areaObj, area.AcceptableColors[Random.Range(0, area.AcceptableColors.Length - 1)]);
		currentPos += spacing;
	}
	public Vector3 SpawnEnemy(int amount , Vector3 spawnVector)
	{
		spawnVector.y += 3;
		spawnVector.x -= 11;
		spawnVector.z -= 2;

		while (amount > 0)
		{
			GameObject newEnemy = null;
			if (MayhemMode) 
			{
				newEnemy = EnemyManager.instance.SpawnEnemy(spawnVector); //spawns a new enemy with the correct amount of health for mayhem
			}
			else 
			{
				newEnemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)]) as GameObject;
			}
			newEnemy.transform.position = spawnVector + new Vector3(amount,0,0);
			newEnemy.transform.SetParent(LevelParent.transform);
			--amount;
		}
		spawnVector.y -= 3;
		spawnVector.x += 11;
		spawnVector.z += 2;
		return spawnVector;
	}
	public void SwapTileSet()
	{
		if (tilePrefabs == cityTilePrefabs)
			tilePrefabs = industrialTilePrefabs;
		else
		{
			tilePrefabs = cityTilePrefabs; //randomly choose the tileset
		}
	}
	public void mayhemLevelUp()
	{
		if (MayhemMode)
		{
			//Clear Previous Level
			AudioManager.instance.PlaySongsForLevel(Random.Range(0, 4));
			EnemyManager.instance.DestroyAll();
			Destroy(LevelParent);
			LevelParent = new GameObject("LevelParentInstance");
			LevelParent.transform.SetParent(transform);
			SwapTileSet(); //alternate tiles
			//Construct New Level
			int level = 1;
			currentPos = levelSpacing * level + 22;
			for (int i = 0; i < numberOfBuildings; ++i)
			{
				generateBuilding(Random.Range(minBuildingWidth, maxBuildingWidth), Random.Range(minBuildingTileCount, maxBuildingTileCount), level);
			} //the final level of PCG needs this for the guidance arrows
			Vector3 spawnVector = new Vector3(1, 0, 0) * currentPos + new Vector3(0, -2, 0); //spawnVector for tiles																		 //GameObject endBuilding = (GameObject)GameObject.Instantiate(levelEndCityBuilding, spawnVector, Quaternion.Euler(0, 141.6f, 0));
			PlaceInteractables();
			GameObject endBuilding = (GameObject)GameObject.Instantiate(levelEndCityBuilding, spawnVector, Quaternion.Euler(0, 141.6f, 0));
			endBuilding.transform.SetParent(LevelParent.transform);
			foreach (GameObject tele in GameObject.FindGameObjectsWithTag("Teleporter"))
			{
				if (tele.name.Contains("Mayhem"))
				{
					int z = 0;
					//print("level 2 tele found");
					foreach (GameObject arrow in guideArrows[0])
					{
						//print("Arrow " + z); 
						if (arrow != null)
							arrow.transform.LookAt(teleporterInstances[teleporterInstances.Count-1].transform); //make all arrows point at the tele for the level just finished
						z++;
					}
				}
			}
			GetComponent<DeactivateDistant>().SetFirstCheck(true);
		}
	}
}
