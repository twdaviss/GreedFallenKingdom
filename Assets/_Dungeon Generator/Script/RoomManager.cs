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



    [Space]

    [Header("Special Rooms")]
    public GameObject[] treasureItems;
    public List<GameObject> npcList;
    public GameObject key;
    public GameObject boss;
    public GameObject shop;
    public GameObject abandonedShop;

    public int loops = 0;


    [HideInInspector] public bool roomsFinished = false;
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
        if (bossSpawned && shopSpawned)
        {
            CheckRoomsFinished();
        }
    }

    private void LoadScene()
    {
        bossSpawned = false;
        shopSpawned = false;
        currentRoomTotal.Clear();
        currentRoomCount.Clear();

        GameObject newEntryRoom;
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        newEntryRoom = Instantiate(entryRoom);
        newEntryRoom.transform.parent = this.transform;

        delaySpawnRoomCheck = false;
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
        }
        else if (currentRoomCount.Count == maxRooms && loops < 2 && key1Spawned)
        {
            StartNewBranch();
            return;
        }
        else if (!delaySpawnRoomCheck)
        {
            delaySpawnRoomType -= Time.deltaTime;
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
        int randomIndex = Random.Range(0, currentRoomTotal.Count);
        //RoomType roomType = Random.value < abandonedShopChance ? RoomType.abandonShop : RoomType.shop;
        RoomType roomType = RoomType.shop;
        while (currentRoomTotal[randomIndex].currentRoomType != RoomType.normal)
        {
            randomIndex = Random.Range(0, currentRoomTotal.Count);
        }
        currentRoomTotal[randomIndex].currentRoomType = RoomType.shop;
        if (roomType == RoomType.shop)
        {
            newShop = Instantiate(shop, currentRoomTotal[randomIndex].transform.position, Quaternion.identity);
            newShop.transform.parent = currentRoomTotal[randomIndex].transform;
        }
        else if (roomType == RoomType.abandonShop)
        {
            newShop = Instantiate(abandonedShop, currentRoomTotal[randomIndex].transform.position, Quaternion.identity);
            newShop.transform.parent = currentRoomTotal[randomIndex].transform;
        }
        currentRoomTotal[randomIndex].SetSpecialRoomActive();
    }

    public void SetTreasureRoom()
    {
        if (treasureSpawned)
        {
            return;
        }
        GameObject newTreasure;
        int randomItemIndex = Random.Range(0, treasureItems.Length);
        int randomRoomIndex = Random.Range(0, currentRoomTotal.Count);

        if (currentRoomTotal[randomRoomIndex].currentRoomType != RoomType.normal)
        {
            return;
        }

        currentRoomTotal[randomRoomIndex].currentRoomType = RoomType.treasure;
        newTreasure = Instantiate(treasureItems[randomItemIndex], currentRoomTotal[randomRoomIndex].transform.position, Quaternion.identity);
        newTreasure.transform.parent = currentRoomTotal[randomRoomIndex].transform;
    }
    public void SetKeyRoom()
    {
        GameObject newKey;

        for (int i = 0; i < currentRoomCount.Count; i++)
        {
            if (currentRoomTotal[i].currentRoomType == RoomType.normal)
            {
                currentRoomCount[i].currentRoomType = RoomType.key;
                newKey = Instantiate(key, currentRoomCount[i].transform.position, Quaternion.identity);
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
                break;
            }
        }
    }
    public void SetNPCRoom()
    {
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

    public void CheckRoomsFinished()
    {
        if (currentRoomCount.Count < minRooms || currentRoomCount.Count > maxRooms)
        {
            LoadScene();
            return;
        }
        aStar.SetActive(true);
        if(onRoomsGenerated != null)
        {
            onRoomsGenerated();
        }
        roomsFinished = true;
        //this.GetComponent<RoomManager>().enabled = false;
        //StartNewBranch();
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
                currentRoomTotal[i] = Instantiate(entryRoom, new Vector2(currentRoomCount[i].transform.position.x, currentRoomCount[i].transform.position.y), Quaternion.identity).GetComponent<RoomController>();
                //currentRoomTotal.Remove(currentRoomCount[i]);
                //currentRoomTotal[i] = newRoom.GetComponent<RoomController>();
                currentRoomTotal[i].transform.parent = this.transform;
                if (currentRoomTotal[i] != null)
                {
                    currentRoomTotal[i].gameObject.GetComponentInChildren<GateManager>().locked = true;
                }
                currentRoomCount.Clear();
                currentRoomCount.Add(currentRoomTotal[i]);
                currentRoomTotal.RemoveAt(currentRoomTotal.Count - 1);

                loops++;
                delaySpawnRoomCheck = false;
                delaySpawnRoomType = 0.05F;
                return;
            }
        }
    }
}
