using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class GameManager: MonoBehaviourPunCallbacks
{
    [Header("Fin de Partida")]
    public float gameDuration = 120f; // Duración de la partida en segundos
    public Text timerText; // UI Text para mostrar el tiempo restante

    private float timeRemaining;
    private bool gameEnded = false;

    void Start()
    {
        Debug.Log("=== GAMEMANAGER START ===");
        Debug.Log("Escena GameScene cargada");
        
        if (!PhotonNetwork.IsConnected)
        {
            Debug.LogError("❌ NO ESTAMOS CONECTADOS A PHOTON!");
            return;
        }
        
        Debug.Log("✅ Conectado a Photon");
        Debug.Log("¿Soy MasterClient? " + PhotonNetwork.IsMasterClient);
        Debug.Log("Jugadores en sala: " + PhotonNetwork.CurrentRoom.PlayerCount);

        string[] prefabs = { "Frog", "VirtualGuy", "Ninja" };
        // Spawn encima del terreno generado (el suelo mínimo es baseHeight=8, así que spawn alto para caer)
        Vector3[] positions = { new Vector3(-3, 40, 0), new Vector3(3, 40, 0), new Vector3(0, 42, 0) };

        int index = Mathf.Clamp(PhotonNetwork.LocalPlayer.ActorNumber - 1, 0, prefabs.Length - 1);
        Debug.Log("🎮 Jugador ActorNumber: " + PhotonNetwork.LocalPlayer.ActorNumber + " → Instanciando: " + prefabs[index]);
        GameObject player = PhotonNetwork.Instantiate(prefabs[index], positions[index], Quaternion.identity);
        Debug.Log("Personaje instanciado: " + player.name);

        timeRemaining = gameDuration;
        
        Debug.Log("=== Verificando elementos en escena ===");
        GameObject grid = GameObject.Find("Grid");
        if (grid != null)
        {
            Debug.Log("✅ Grid encontrado en posición: " + grid.transform.position);
        }
        else
        {
            Debug.LogWarning("⚠️ Grid NO encontrado!");
        }
        
        Camera cam = Camera.main;
        if (cam != null)
        {
            Debug.Log("✅ Camera.main encontrada en posición: " + cam.transform.position);
        }
        else
        {
            Debug.LogWarning("⚠️ Camera.main NO encontrada!");
        }
    }

    void Update()
    {
        if (gameEnded || !PhotonNetwork.IsConnected) return;

        // Temporizador
        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            EndGame();
        }

        // Actualizar UI del timer
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60f);
            int seconds = Mathf.FloorToInt(timeRemaining % 60f);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    // Llamado cuando un jugador cae al vacío o se acaba el tiempo.
    public void EndGame()
    {
        if (gameEnded) return;
        gameEnded = true;

        Debug.Log("🏁 FIN DE PARTIDA");

        // Solo el MasterClient carga la escena para todos
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(ReturnToLobby());
        }
    }

    IEnumerator ReturnToLobby()
    {
        yield return new WaitForSeconds(3f); // Esperar 3 segundos antes de volver
        PhotonNetwork.LoadLevel(0); // Volver a Cortinilla (índice 0)
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log("⚠️ Jugador salió: " + otherPlayer.NickName);
        // Si solo queda 1 jugador, terminar partida
        if (PhotonNetwork.CurrentRoom.PlayerCount <= 1)
        {
            EndGame();
        }
    }
}
