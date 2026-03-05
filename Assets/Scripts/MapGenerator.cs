using UnityEngine;
using UnityEngine.Tilemaps;
using Photon.Pun;

public class MapGenerator : MonoBehaviour
{
    [Header("Referencias")]
    public Tilemap tilemap;

    [Header("Tiles")]
    public TileBase sueloTile;     // Sprite de suelo (ruido bajo / zonas bajas)
    public TileBase montanaTile;   // Sprite de montaña (ruido alto / zonas altas)

    [Header("Tamaño del Mapa")]
    public int mapWidth = 200;     // Ancho total del mapa en tiles
    public int mapHeight = 50;     // Altura máxima posible del mapa

    [Header("Perlin Noise")]
    public float magnification = 15f;  // Escala del ruido (más alto = terreno más suave)
    public int seed = 0;               // Semilla (0 = aleatoria)

    [Header("Forma del Terreno")]
    public int baseHeight = 8;         // Altura mínima del terreno
    public int maxExtraHeight = 25;    // Altura extra máxima que añade el Perlin Noise
    [Range(0f, 1f)]
    public float mountainThreshold = 0.55f; // Umbral: por encima de este valor de Perlin = montaña

    void Start()
    {
        // Solo el MasterClient genera el mapa y envía la semilla
        if (PhotonNetwork.IsConnected)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                // Generar semilla aleatoria si es 0
                if (seed == 0) seed = Random.Range(1, 99999);

                // Enviar semilla a todos los jugadores
                GetComponent<PhotonView>().RPC("RPC_GenerateMap", RpcTarget.AllBuffered, seed);
            }
        }
        else
        {
            // Modo offline / testing sin Photon
            if (seed == 0) seed = Random.Range(1, 99999);
            GenerateMap(seed);
        }
    }

    [PunRPC]
    void RPC_GenerateMap(int sharedSeed)
    {
        GenerateMap(sharedSeed);
    }

    void GenerateMap(int usedSeed)
    {
        Debug.Log("🗺️ Generando mapa con semilla: " + usedSeed);

        // Asegurar que el Tilemap tenga collider y NO caiga por gravedad
        if (tilemap.GetComponent<TilemapCollider2D>() == null)
            tilemap.gameObject.AddComponent<TilemapCollider2D>();

        // Si tiene Rigidbody2D, ponerlo en Static para que no caiga
        Rigidbody2D rb = tilemap.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.bodyType = RigidbodyType2D.Static;

        tilemap.ClearAllTiles();

        // Offset para centrar el mapa (los jugadores spawnean cerca de x=0)
        int halfWidth = mapWidth / 2;

        for (int x = 0; x < mapWidth; x++)
        {
            // Obtener valor Perlin para esta columna (solo eje X → perfil de altura)
            float perlinValue = Mathf.PerlinNoise(
                (x + usedSeed) / magnification,
                usedSeed * 0.1f
            );
            perlinValue = Mathf.Clamp01(perlinValue);

            // Calcular altura del terreno en esta columna
            int terrainHeight = baseHeight + Mathf.FloorToInt(perlinValue * maxExtraHeight);
            terrainHeight = Mathf.Clamp(terrainHeight, 1, mapHeight);

            // Determinar si esta columna es zona de montaña (ruido alto)
            bool isMountainColumn = perlinValue >= mountainThreshold;

            // Rellenar bloques desde abajo hasta la altura del terreno
            for (int y = 0; y < terrainHeight; y++)
            {
                // Posición en el tilemap (centrado en x=0)
                Vector3Int tilePos = new Vector3Int(x - halfWidth, y, 0);

                if (isMountainColumn)
                {
                    tilemap.SetTile(tilePos, montanaTile);
                }
                else
                {
                    tilemap.SetTile(tilePos, sueloTile);
                }
            }
        }

        Debug.Log("✅ Mapa generado: " + mapWidth + "x columnas");
    }
}
