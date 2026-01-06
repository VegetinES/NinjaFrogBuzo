using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Connection: MonoBehaviourPunCallbacks
{
    private void Start()
    {
        Debug.Log("=== CONNECTION START ===");
        Debug.Log("Iniciando conexión a Photon...");
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene = true;
        Debug.Log("AutomaticallySyncScene activado");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("✅ CONECTADO AL MASTER SERVER!");
        Debug.Log("Intentando unirse/crear sala 'room1'...");
        
        RoomOptions options = new RoomOptions() { MaxPlayers = 4};
        PhotonNetwork.JoinOrCreateRoom("room1", options, TypedLobby.Default);
    }

    public void ButtonConnect()
    {
        Debug.Log("Botón 'Conexión' presionado");
        UnityEngine.UI.Button btn = GetComponent<UnityEngine.UI.Button>();
        if (btn != null) 
        {
            btn.interactable = false;
            Debug.Log("Botón desactivado");
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("✅ CONECTADO A LA SALA: " + PhotonNetwork.CurrentRoom.Name);
        Debug.Log("Número de jugadores en sala: " + PhotonNetwork.CurrentRoom.PlayerCount);
        Debug.Log("¿Soy MasterClient? " + PhotonNetwork.IsMasterClient);
        Debug.Log("Mi PhotonView ID: " + PhotonNetwork.LocalPlayer.ActorNumber);
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            Debug.Log("⚠️ HAY MÁS DE 1 JUGADOR - CAMBIANDO A GAMESCENE...");
            Debug.Log("Cargando escena índice 1 (GameScene)");
            PhotonNetwork.LoadLevel(1);
            Destroy(this);
        }
    }

    // CORREGIDO: Especificar el tipo completo
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log("🎮 NUEVO JUGADOR ENTRÓ A LA SALA: " + newPlayer.NickName);
        Debug.Log("Total jugadores ahora: " + PhotonNetwork.CurrentRoom.PlayerCount);
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log("⚠️ JUGADOR SALIÓ DE LA SALA: " + otherPlayer.NickName);
        Debug.Log("Total jugadores ahora: " + PhotonNetwork.CurrentRoom.PlayerCount);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError("❌ DESCONECTADO DE PHOTON - Razón: " + cause);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError("❌ ERROR AL UNIRSE A SALA - Código: " + returnCode + " Mensaje: " + message);
    }
}
