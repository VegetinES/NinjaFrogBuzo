using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Murcielago : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private float distancia;
    [SerializeField] private float cooldownQuitarFresa = 1f;
    public Vector3 puntoInicial;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // El jugador más cercano (solo válido en MasterClient)
    public Transform jugador { get; private set; }
    private readonly Dictionary<int, float> ultimoGolpePorJugador = new Dictionary<int, float>();

    void Start()
    {
        animator = GetComponent<Animator>();
        puntoInicial = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Inicializar a distancia grande para que el Animator no dispare Seguir antes de encontrar al jugador
        animator.SetFloat("Distancia", 999f);

        ActualizarJugadorMasCercano();
    }

    void Update()
    {
        // Solo el MasterClient (o modo offline) ejecuta la IA
        if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient) return;

        ActualizarJugadorMasCercano();

        if (jugador == null) return;

        distancia = Vector2.Distance(transform.position, jugador.position);
        animator.SetFloat("Distancia", distancia);
    }

    private void ActualizarJugadorMasCercano()
    {
        GameObject[] jugadores = GameObject.FindGameObjectsWithTag("Player");
        float menorDistancia = Mathf.Infinity;
        Transform masCercano = null;

        foreach (GameObject j in jugadores)
        {
            float d = Vector2.Distance(transform.position, j.transform.position);
            if (d < menorDistancia)
            {
                menorDistancia = d;
                masCercano = j.transform;
            }
        }

        jugador = masCercano;
    }

    public void Girar(Vector3 objetivo)
    {
        spriteRenderer.flipX = transform.position.x < objetivo.x;
    }

    // Sincroniza posición, distancia y flip con todos los clientes
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(animator.GetFloat("Distancia"));
            stream.SendNext(spriteRenderer.flipX);
        }
        else
        {
            transform.position = (Vector3)stream.ReceiveNext();
            animator.SetFloat("Distancia", (float)stream.ReceiveNext());
            spriteRenderer.flipX = (bool)stream.ReceiveNext();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IntentarQuitarFresa(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        IntentarQuitarFresa(other);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        IntentarQuitarFresa(collision.collider);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        IntentarQuitarFresa(collision.collider);
    }

    private void IntentarQuitarFresa(Collider2D other)
    {
        if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient) return;
        if (!TryGetActorNumber(other, out int actorNumber)) return;

        float tiempoActual = Time.time;
        if (ultimoGolpePorJugador.TryGetValue(actorNumber, out float ultimoGolpe) && tiempoActual - ultimoGolpe < cooldownQuitarFresa)
            return;

        ultimoGolpePorJugador[actorNumber] = tiempoActual;

        if (PhotonNetwork.IsConnected)
            photonView.RPC(nameof(RPC_QuitarFresa), RpcTarget.All, actorNumber);
        else
            QuitarFresa(actorNumber);
    }

    private bool TryGetActorNumber(Collider2D other, out int actorNumber)
    {
        actorNumber = -1;

        Player player = other.GetComponentInParent<Player>();
        if (player == null) return false;

        PhotonView playerView = player.GetComponent<PhotonView>();
        if (playerView == null || playerView.Owner == null) return false;

        actorNumber = playerView.Owner.ActorNumber;
        return true;
    }

    [PunRPC]
    private void RPC_QuitarFresa(int actorNumber)
    {
        QuitarFresa(actorNumber);
    }

    private void QuitarFresa(int actorNumber)
    {
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
            gameManager.AddScore(actorNumber, -1);
    }
}