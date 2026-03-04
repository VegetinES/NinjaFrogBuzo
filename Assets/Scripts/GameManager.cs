using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager: MonoBehaviour
{
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
        Vector3[] positions = { new Vector3(-3, 3, 0), new Vector3(3, 3, 0), new Vector3(0, 5, 0) };

        int index = Mathf.Clamp(PhotonNetwork.LocalPlayer.ActorNumber - 1, 0, prefabs.Length - 1);
        Debug.Log("🎮 Jugador ActorNumber: " + PhotonNetwork.LocalPlayer.ActorNumber + " → Instanciando: " + prefabs[index]);
        GameObject player = PhotonNetwork.Instantiate(prefabs[index], positions[index], Quaternion.identity);
        Debug.Log("Personaje instanciado: " + player.name);
        
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
}
