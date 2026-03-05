using UnityEngine;
using UnityEngine.Tilemaps;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;

public class StrawberrySpawner : MonoBehaviourPunCallbacks
{
    [Header("Referencias")]
    public GameObject strawberryPrefab;
    public Tilemap tilemap;

    [Header("Configuración")]
    public int maxStrawberries = 8;
    public float respawnDelay = 3f;

    [Header("Audio")]
    public AudioClip strawberryLoopSound;

    [Header("Límites de Spawn")]
    public float minX = -5f;
    public float maxX = 5f;
    public float minY = -1.5f;
    public float maxY = 4f;

    private List<GameObject> activeStrawberries = new List<GameObject>();
    private HashSet<int> collectedIds = new HashSet<int>();
    private System.Random rng;
    private int nextId = 0;

    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                int seed = Random.Range(1, 99999);
                GetComponent<PhotonView>().RPC("RPC_InitSpawner", RpcTarget.AllBuffered, seed);
            }
        }
        else
        {
            InitSpawner(Random.Range(1, 99999));
        }
    }

    [PunRPC]
    void RPC_InitSpawner(int seed)
    {
        InitSpawner(seed);
    }

    void InitSpawner(int seed)
    {
        Debug.Log("🍓 Spawner de fresas - Semilla: " + seed);
        rng = new System.Random(seed);
        for (int i = 0; i < maxStrawberries; i++)
        {
            Vector2 pos = GetValidPosition();
            SpawnStrawberryAt(pos.x, pos.y, nextId++);
        }
    }

    // Genera una posición que NO esté dentro de un tile sólido (determinista por semilla)
    Vector2 GetValidPosition()
    {
        for (int attempt = 0; attempt < 50; attempt++)
        {
            float x = (float)(rng.NextDouble() * (maxX - minX) + minX);
            float y = (float)(rng.NextDouble() * (maxY - minY) + minY);

            Vector3Int cellPos = tilemap.WorldToCell(new Vector3(x, y, 0));
            if (!tilemap.HasTile(cellPos))
            {
                return new Vector2(x, y);
            }
        }
        return new Vector2(0, 0);
    }

    void SpawnStrawberryAt(float x, float y, int id)
    {
        GameObject obj = Instantiate(strawberryPrefab, new Vector3(x, y, 0), Quaternion.identity);
        obj.name = "Strawberry_" + id;
        Strawberry sb = obj.GetComponent<Strawberry>();
        if (sb != null)
        {
            sb.strawberryId = id;
            sb.spawner = this;
            sb.SetupAudio(strawberryLoopSound);
        }
        activeStrawberries.Add(obj);
    }

    public void CollectStrawberry(int id, int actorNumber)
    {
        if (PhotonNetwork.IsConnected)
        {
            GetComponent<PhotonView>().RPC("RPC_CollectStrawberry", RpcTarget.All, id, actorNumber);
        }
        else
        {
            DoCollect(id, actorNumber);
        }
    }

    [PunRPC]
    void RPC_CollectStrawberry(int id, int actorNumber)
    {
        DoCollect(id, actorNumber);
    }

    void DoCollect(int id, int actorNumber)
    {
        if (collectedIds.Contains(id)) return;
        collectedIds.Add(id);

        GameObject strawberry = activeStrawberries.Find(
            s => s != null && s.GetComponent<Strawberry>() != null && s.GetComponent<Strawberry>().strawberryId == id
        );
        if (strawberry != null)
        {
            activeStrawberries.Remove(strawberry);
            Destroy(strawberry);
        }

        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
        {
            gm.AddScore(actorNumber, 1);
        }

        Debug.Log("🍓 Fresa #" + id + " recogida por jugador " + actorNumber);

        if (PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected)
        {
            StartCoroutine(RespawnAfterDelay());
        }
    }

    IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);

        float x = 0, y = 0;
        for (int attempt = 0; attempt < 50; attempt++)
        {
            x = Random.Range(minX, maxX);
            y = Random.Range(minY, maxY);
            Vector3Int cellPos = tilemap.WorldToCell(new Vector3(x, y, 0));
            if (!tilemap.HasTile(cellPos)) break;
        }

        int id = nextId++;
        if (PhotonNetwork.IsConnected)
        {
            GetComponent<PhotonView>().RPC("RPC_SpawnAt", RpcTarget.All, x, y, id);
        }
        else
        {
            SpawnStrawberryAt(x, y, id);
        }
    }

    [PunRPC]
    void RPC_SpawnAt(float x, float y, int id)
    {
        if (id >= nextId) nextId = id + 1;
        SpawnStrawberryAt(x, y, id);
    }
}
