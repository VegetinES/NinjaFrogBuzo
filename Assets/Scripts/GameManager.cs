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
        
        // CORREGIDO: Posiciones arriba del mapa, no debajo
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("🐸 SOY MASTER - Instanciando FROG en posición (-3, 3, 0)");
            GameObject frog = PhotonNetwork.Instantiate("Frog", new Vector3(-3, 3, 0), Quaternion.identity);
            Debug.Log("Frog instanciado: " + frog.name);
        }
        else
        {
            Debug.Log("🤿 SOY CLIENTE - Instanciando VIRTUALGUY en posición (3, 3, 0)");
            GameObject virtualGuy = PhotonNetwork.Instantiate("VirtualGuy", new Vector3(3, 3, 0), Quaternion.identity);
            Debug.Log("VirtualGuy instanciado: " + virtualGuy.name);
        }
        
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
