using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Connection: MonoBehaviourPunCallbacks
{
    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); //Nos conectamos con el parámetros definidos.
        PhotonNetwork.AutomaticallySyncScene = true; // Activamos la sincronización de escena.
        // Necesario para el intercambio entre escenas.
    }

    // Método para conectarse al máster
    public override void OnConnectedToMaster()
    {
        print("Conectado al master !!!!");
    }

    // Método de conexión con el máster con el botón
    public void ButtonConnect()
    {
        RoomOptions options = new RoomOptions() { MaxPlayers = 4};
        PhotonNetwork.JoinOrCreateRoom("room1", options, TypedLobby.Default);
    }

    // Método de conexión con una sala
    public override void OnJoinedRoom()
    {
        Debug.Log("Conectada a la sala " + PhotonNetwork.CurrentRoom.Name);
        Debug.Log("Hay..." + PhotonNetwork.CurrentRoom.PlayerCount + " jugadores");
    }

    // Método que controla si pasamos a la siguiente escena si somos más de una persona.
    private void Update()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            // Cargamos siguiente nivel
            PhotonNetwork.LoadLevel(1);
            Destroy(this);
        }
    }
}
