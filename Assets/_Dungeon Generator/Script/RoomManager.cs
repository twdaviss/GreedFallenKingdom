using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class RoomManager : MonoBehaviour
{
    [SerializeField] private GameObject aStar;

    [SerializeField] private GameObject entryRoom;
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

    public List<RoomController> currentRooms;
    public List<RoomController> currentDeadEndRooms;

    [Space]

    [Header("Room Delay")]
    public float delaySpawnRoomType = 0.0F;
    private bool delaySpawnRoomCheck = false;
    [HideInInspector] public bool bossSpawned = false;
    [HideInInspector] public bool shopSpawned = false;

    [Space]

    [Header("Special Rooms")]
    public GameObject[] treasureItems;
    public List<GameObject> npcList;
    public GameObject key;
    public GameObject boss;
    public GameObject shop;
    public GameObject abandonedShop;

    public bool roomsFinished = false;
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
        currentRooms.Clear();

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
            SetTreasureRoom();
            SetNPCRoom();

            delaySpawnRoomCheck = true;
            delaySpawnRoomType = 0F;
        }
        else if (!delaySpawnRoomCheck)
        {
            delaySpawnRoomType -= Time.deltaTime;
        }
    }

    private void SetBossRoom()
    {
        GameObject newBoss;
        for (int i = currentRooms.Count - 1; i >= 0; i--)
        {
            if (currentRooms[i].currentRoomType != RoomType.empty)
            {
                currentRooms[i].currentRoomType = RoomType.boss;
                newBoss = Instantiate(boss, currentRooms[i].transform.position, Quaternion.identity);
                newBoss.transform.parent = currentRooms[i].transform;
                currentRooms[i].SetSpecialRoomActive();

                break;
            }
        }
    }

    private void SetShopRoom()
    {
        GameObject newShop;
        int randomIndex = Random.Range(0, currentRooms.Count);
        //RoomType roomType = Random.value < abandonedShopChance ? RoomType.abandonShop : RoomType.shop;
        RoomType roomType = RoomType.shop;
        while (currentRooms[randomIndex].currentRoomType != RoomType.normal)
        {
            randomIndex = Random.Range(0, currentRooms.Count);
        }
        currentRooms[randomIndex].currentRoomType = RoomType.shop;
        if (roomType == RoomType.shop)
        {
            newShop = Instantiate(shop, currentRooms[randomIndex].transform.position, Quaternion.identity);
            newShop.transform.parent = currentRooms[randomIndex].transform;
        }
        else if (roomType == RoomType.abandonShop)
        {
            newShop = Instantiate(abandonedShop, currentRooms[randomIndex].transform.position, Quaternion.identity);
            newShop.transform.parent = currentRooms[randomIndex].transform;
        }
        currentRooms[randomIndex].SetSpecialRoomActive();
    }

    public void SetTreasureRoom()
    {
        GameObject newTreasure;
        int randomItemIndex = Random.Range(0, treasureItems.Length);
        int randomRoomIndex = Random.Range(0, currentRooms.Count);

        if (currentRooms[randomRoomIndex].currentRoomType != RoomType.normal)
        {
            return;
        }

        currentRooms[randomRoomIndex].currentRoomType = RoomType.treasure;
        newTreasure = Instantiate(treasureItems[randomItemIndex], currentRooms[randomRoomIndex].transform.position, Quaternion.identity);
        newTreasure.transform.parent = currentRooms[randomRoomIndex].transform;
    }

    public void SetNPCRoom()
    {
        GameObject newNPC;
        int randomItemIndex = Random.Range(0, npcList.Count);
        int randomRoomIndex = Random.Range(0, currentRooms.Count);

        if (currentRooms[randomRoomIndex].currentRoomType != RoomType.normal)
        {
            return;
        }

        currentRooms[randomRoomIndex].currentRoomType = RoomType.npc;
            
        newNPC = Instantiate(npcList[randomItemIndex], currentRooms[randomRoomIndex].transform.position, Quaternion.identity);
        newNPC.transform.parent = currentRooms[randomRoomIndex].transform;

        npcList.RemoveAt(randomItemIndex);
        currentRooms[randomRoomIndex].SetSpecialRoomActive();
    }

    public void CheckRoomsFinished()
    {
        if (currentRooms.Count < minRooms || currentRooms.Count > maxRooms)
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
    }
}
