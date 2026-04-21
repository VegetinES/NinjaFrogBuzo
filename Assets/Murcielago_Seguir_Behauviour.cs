using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Murcielago_Seguir_Behaviour : StateMachineBehaviour
{
    [SerializeField] private float velocidadMovimiento;
    [SerializeField] private float tiempoBase;

    private float tiempoSeguir;
    private Murcielago murcielago;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        tiempoSeguir = tiempoBase;
        murcielago = animator.gameObject.GetComponent<Murcielago>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Solo el MasterClient (o modo offline) mueve el murciélago
        if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient) return;

        if (murcielago.jugador == null) return;

        animator.transform.position = Vector2.MoveTowards(animator.transform.position, murcielago.jugador.position, velocidadMovimiento * Time.deltaTime);
        murcielago.Girar(murcielago.jugador.position);

        tiempoSeguir -= Time.deltaTime;

        if (tiempoSeguir <= 0)
        {
            animator.SetTrigger("Volver");
        }
    }
}