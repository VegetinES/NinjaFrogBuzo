using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Murcielago_Volver_Behaviour : StateMachineBehaviour
{
    [SerializeField] private float velocidadMovimiento;
    private Vector3 puntoInicial;
    private Murcielago murcielago;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        murcielago = animator.gameObject.GetComponent<Murcielago>();
        puntoInicial = murcielago.puntoInicial;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.position = Vector2.MoveTowards(animator.transform.position, puntoInicial, velocidadMovimiento * Time.deltaTime);
        murcielago.Girar(puntoInicial);
        
        if (animator.transform.position == puntoInicial)
        {
            animator.SetTrigger("Llego"); // Para cambiar de estado
        }
    }
}