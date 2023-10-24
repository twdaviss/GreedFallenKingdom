using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class RoomManager : MonoBehaviour
{
    [SerializeField] private GameObject aStar;

    [SerializeField] public GameObject entryRoom;
    [SerializeField] public int minRooms;
    [SerializeField] public int maxRooms;
    [SerializeField] public float centerRoomChance;
    [HideInInspector] public int potentialRooms;

    [Header("Rooms")]
    public GameObject[] bottomRooms;
    public GameObject[] topRooms;
    public GameObject[] leftRooms;
    public GameObject[] rightRooms;
    public GameObject[] centerRoom;
    public GameObject[] closedRooms;

    [Space]

    public List<RoomController> currentRoomTotal;
    public List<RoomController> currentRoomCount;
    public List<RoomController> currentDeadEndRooms;
    //public RoomManager newRoomManager;

    [Space]

    [Header("Room Delay")]
    public float delaySpawnRoomType = 0.0F;
    private bool delaySpawnRoomCheck = false;
    [HideInInspector] public bool bossSpawned = false;
    [HideInInspector] public bool shopSpawned = false;
    [HideInInspector] public bool treasureSpawned = false;
    [HideInInspector] public bool key1Spawned = false;
    [HideInInspector] public bool key2Spawned = false;
    [HideInInspector] public bool key3Spawned = false;
    [HideInInspector] public bool roomsFinished = false;
    [HideInInspector] public bool mapFinished = false;



    [Space]

    [Header("Special Rooms")]
    public GameObject[] treasureItems;
    public List<GameObject> npcList;
    public GameObject[] keys;
    public GameObject boss;
    public GameObject shop;
    public GameObject abandonedShop;

    public int loops = 0;


    public delegate void OnRoomsGenerated();
    public static event OnRoomsGenerated onRoomsGenerated;

    private void Start()
    {
        LoadScene();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            LoadScene();
        }
        if (!roomsFinished)
        {
            SpawnRoomTypes();
        }
        else if (!mapFinished)
        {
            CheckMapFinished();
        }
    }

    private void LoadScene()
    {
        currentRoomCount.Clear();

        shopSpawned = false;
        if (loops == 2)
        {
            bossSpawned = false;
        }
        if (loops > 0)
        {
            loops--;
            StartNewBranch();
            delaySpawnRoomCheck = false;
            delaySpawnRoomType = 0.75F;
            return;
        }

        GameObject newEntryRoom;
        newEntryRoom = Instantiate(entryRoom);
        newEntryRoom.transform.parent = this.transform;

        //delaySpawnRoomCheck = false;
        delaySpawnRoomType = 0.75F;
    }

    private void SpawnRoomTypes()
    {
        if (delaySpawnRoomType <= 0F && !delaySpawnRoomCheck)
        {
            SetBossRoom();
            SetShopRoom();
            SetKeyRoom();
            SetTreasureRoom();
            SetNPCRoom();

            delaySpawnRoomCheck = true;
            delaySpawnRoomType = 0F;
            CheckBranchFinished();
        }
        else if (!delaySpawnRoomCheck)
        {
            delaySpawnRoomType -= Time.deltaTime;
        }
        else
        {
            return;
        }
    }

    private void SetBossRoom()
    {
        if (bossSpawned || loops < 2)
        {
            return;
        }
        GameObject newBoss;
        for (int i = currentRoomTotal.Count - 1; i >= 0; i--)
        {
            if (currentRoomTotal[i].currentRoomType != RoomType.empty)
            {
                currentRoomTotal[i].currentRoomType = RoomType.boss;
                newBoss = Instantiate(boss, currentRoomTotal[i].transform.position, Quaternion.identity);
                newBoss.transform.parent = currentRoomTotal[i].transform;
                currentRoomTotal[i].SetSpecialRoomActive();

                break;
            }
        }
    }

    private void SetShopRoom()
    {
        if (shopSpawned)
        {
            return;
        }
        GameObject newShop;
        int randomIndex = Random.Range(0, currentRoomCount.Count);
        //RoomType roomType = Random.value < abandonedShopChance ? RoomType.abandonShop : RoomType.shop;
        for(int i = currentRoomCount.Count/2; i < currentRoomCount.Count-1; i++)
        {
            if(currentRoomCount[randomIndex].currentRoomType != RoomType.normal)
            {
                continue;
            }
            else
            {
                currentRoomCount[i].currentRoomType = RoomType.shop;
                newShop = Instantiate(shop, currentRoomCount[i].transform.position, Quaternion.identity);
                newShop.transform.parent = currentRoomCount[i].transform;
                break;
            }
        }
        currentRoomCount[randomIndex].SetSpecialRoomActive();
        shopSpawned = true;
    }

    public void SetTreasureRoom()
    {
        if (treasureSpawned)
        {
            return;
        }
        GameObject newTreasure;
        int randomItemIndex = Random.Range(0, treasureItems.Length);
        int randomRoomIndex = Random.Range(0, currentRoomCount.Count);

        if (currentRoomCount[randomRoomIndex].currentRoomType != RoomType.normal)
        {
            return;
        }

        currentRoomCount[randomRoomIndex].currentRoomType = RoomType.treasure;
        newTreasure = Instantiate(treasureItems[randomItemIndex], currentRoomCount[randomRoomIndex].transform.position, Quaternion.identity);
        newTreasure.transform.parent = currentRoomCount[randomRoomIndex].transform;
    }
    public void SetKeyRoom()
    {
        GameObject newKey;

        for (int i = 0; i < currentRoomCount.Count; i++)
        {
            if (currentRoomCount[i].currentRoomType == RoomType.normal)
            {
                currentRoomCount[i].currentRoomType = RoomType.key;
                newKey = Instantiate(keys[loops], currentRoomCount[i].transform.position, Quaternion.identity);
                newKey.transform.parent = currentRoomCount[i].transform;
                switch (loops)
                {
                    case 0:
                        key1Spawned = true;
                        break;
                    case 1:
                        key2Spawned = true;
                        break;
                    case 2:
                        key3Spawned = true;
                        break;
                }
                return;
            }
        }
    }
    public void SetNPCRoom()
    {
        if(npcList.Count == 0)
        {
            return;
        }
        GameObject newNPC;
        int randomItemIndex = Random.Range(0, npcList.Count);
        int randomRoomIndex = Random.Range(0, currentRoomCount.Count);

        if (currentRoomCount[randomRoomIndex].currentRoomType != RoomType.normal || currentRoomCount.Count <=0)
        {
            return;
        }

        currentRoomCount[randomRoomIndex].currentRoomType = RoomType.npc;
            
        newNPC = Instantiate(npcList[randomItemIndex], currentRoomCount[randomRoomIndex].transform.position, Quaternion.identity);
        newNPC.transform.parent = currentRoomCount[randomRoomIndex].transform;

        npcList.RemoveAt(randomItemIndex);
        currentRoomCount[randomRoomIndex].SetSpecialRoomActive();
    }

    public void CheckBranchFinished()
    {
        if (currentRoomCount.Count < minRooms || currentRoomCount.Count > maxRooms)
        {
            LoadScene();
            return;
        }
        else
        {
            foreach(RoomController room in currentRoomCount)
            {
                currentRoomTotal.Add(room);
            }
            //StartNewBranch();

            if (loops < 2)
            {
                StartNewBranch();
            }
            //currentRoomCount.Clear();
            roomsFinished = true;
        }
        
        //this.GetComponent<RoomManager>().enabled = false;
        //StartNewBranch();
    }
    public void CheckMapFinished()
    {
        if(loops != 2 || currentRoomTotal.Count + currentRoomCount.Count < 3 * minRooms)
        {
            roomsFinished = false;
            return;
        }
        else
        {
            foreach (RoomController room in currentRoomCount)
            {
                currentRoomTotal.Add(room);
            }
            currentRoomCount.Clear();
            roomsFinished = true;
            mapFinished = true;

            aStar.SetActive(true);
            if (onRoomsGenerated != null)
            {
                onRoomsGenerated();
            }
        }
        //this.GetComponent<RoomManager>().enabled = false;
    }
    //public void CheckMapFinished()
    //{
    //    if (currentRoomCount.Count < minRooms || currentRoomCount.Count > maxRooms)
    //    {
    //        LoadScene();
    //        return;
    //    }
    //    aStar.SetActive(true);
    //    if (onRoomsGenerated != null)
    //    {
    //        onRoomsGenerated();
    //    }
    //    roomsFinished = true;
    //    //this.GetComponent<RoomManager>().enabled = false;
    //    //StartNewBranch();
    //}

    public void StartNewBranch()
    {
        if(loops > 2)
        {
            return;
        }
        for (int i = currentRoomTotal.Count-1; i > 0; i--)
        {
            if (currentRoomTotal[i].currentRoomType == RoomType.normal)
            {
                Destroy(currentRoomTotal[i].gameObject);
                RoomController newRoom = Instantiate(entryRoom, new Vector2(currentRoomTotal[i].transform.position.x, currentRoomTotal[i].transform.position.y), Quaternion.identity).GetComponent<RoomController>();
                //currentRoomTotal.Remove(currentRoomCount[i]);
                //currentRoomTotal[i] = newRoom.GetComponent<RoomController>();
                currentRoomTotal.RemoveAt(i);
                newRoom.transform.parent = this.transform;
                if (newRoom != null)
                {
                    newRoom.gameObject.GetComponentInChildren<GateManager>().locked = true;
                }
                currentRoomCount.Clear();
                currentRoomCount.Add(newRoom);
                //currentRoomTotal.Add(newRoom);
                //currentRoomTotal.RemoveAt(currentRoomTotal.Count - 1);

                loops++;
                delaySpawnRoomCheck = false;
                delaySpawnRoomType = 0.05F;
                shopSpawned = false;
                return;
            }
        }
    }
}
