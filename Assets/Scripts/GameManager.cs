using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class GameManager: MonoBehaviourPunCallbacks
{
    [Header("Fin de Partida")]
    public float gameDuration = 120f; // Duración de la partida en segundos
    public TMP_Text timerText;

    [Header("Puntuación")]
    public TMP_Text scoreText;

    private float timeRemaining;
    private bool gameEnded = false;
    private Dictionary<int, int> scores = new Dictionary<int, int>();

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
        Vector3[] positions = { new Vector3(-3, -2f, 0), new Vector3(3, -2f, 0), new Vector3(0, -2f, 0) };

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

    public void AddScore(int actorNumber, int amount)
    {
        if (!scores.ContainsKey(actorNumber))
            scores[actorNumber] = 0;
        scores[actorNumber] = Mathf.Max(0, scores[actorNumber] + amount);

        if (actorNumber == PhotonNetwork.LocalPlayer.ActorNumber && scoreText != null)
        {
            scoreText.text = "Fresas: " + scores[actorNumber];
        }
    }

    public void EndGame()
    {
        if (gameEnded) return;
        gameEnded = true;

        Debug.Log("🏁 FIN DE PARTIDA");

        int bestActor = -1;
        int bestScore = -1;
        foreach (var kvp in scores)
        {
            if (kvp.Value > bestScore)
            {
                bestScore = kvp.Value;
                bestActor = kvp.Key;
            }
        }

        if (timerText != null)
        {
            if (bestActor >= 0)
            {
                string winnerName = "Jugador " + bestActor;
                foreach (var p in PhotonNetwork.PlayerList)
                {
                    if (p.ActorNumber == bestActor)
                        winnerName = p.NickName;
                }
                timerText.text = "¡" + winnerName + " gana con " + bestScore + " fresas!";
            }
            else
            {
                timerText.text = "¡Nadie recogió fresas!";
            }
        }

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(ReturnToLobby());
        }
    }

    IEnumerator ReturnToLobby()
    {
        yield return new WaitForSeconds(5f);
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
