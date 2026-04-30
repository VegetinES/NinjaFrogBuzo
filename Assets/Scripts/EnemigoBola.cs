using UnityEngine;
using Photon.Pun;

public class EnemigoBola : MonoBehaviourPun, IPunObservable
{
    private enum Estado { Patrulla, Persigue }
    private Estado estadoActual = Estado.Patrulla;

    [Header("Patrulla (límites del mapa)")]
    [SerializeField] private Vector2 mapaMin = new Vector2(-8f, -4f);
    [SerializeField] private Vector2 mapaMax = new Vector2(8f, 4f);
    [SerializeField] private float velocidadPatrulla = 2f;
    [SerializeField] private float toleranciaDestino = 0.3f;
    [SerializeField] private float tiempoMaxAtascado = 1.5f;

    [Header("Detección")]
    [SerializeField] private float radioDeteccion = 6f;

    [Header("Persecución")]
    [SerializeField] private float velocidadPersecucion = 4f;

    private Vector3 destinoPatrulla;
    private Transform jugador;
    private Rigidbody2D rb;
    private float timerAtascado;
    private Vector3 posicionPrevia;

    // para sincronizar con los otros clientes
    private Vector3 posicionRed;
    private Estado estadoRed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        posicionPrevia = transform.position;
        ElegirNuevoDestino();
        ActualizarJugadorMasCercano();
    }

    void Update()
    {
        if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
        {
            estadoActual = estadoRed;
            return;
        }

        ActualizarJugadorMasCercano();

        float distancia = jugador != null
            ? Vector2.Distance(transform.position, jugador.position)
            : float.MaxValue;

        switch (estadoActual)
        {
            case Estado.Patrulla:
                if (distancia <= radioDeteccion)
                    estadoActual = Estado.Persigue;
                break;

            case Estado.Persigue:
                if (distancia > radioDeteccion)
                {
                    estadoActual = Estado.Patrulla;
                    ElegirNuevoDestino();
                }
                break;
        }
    }

    void FixedUpdate()
    {
        // clientes remotos interpolan la posición que manda el master
        if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
        {
            rb.MovePosition(Vector3.MoveTowards(
                transform.position, posicionRed, Time.fixedDeltaTime * velocidadPersecucion * 2f));
            return;
        }

        switch (estadoActual)
        {
            case Estado.Patrulla: EjecutarPatrulla(); break;
            case Estado.Persigue: EjecutarPersecucion(); break;
        }
    }

    private void EjecutarPatrulla()
    {
        Vector2 dir = ((Vector2)destinoPatrulla - (Vector2)transform.position).normalized;
        rb.velocity = dir * velocidadPatrulla;

        if (Vector2.Distance(transform.position, destinoPatrulla) <= toleranciaDestino)
        {
            ElegirNuevoDestino();
            timerAtascado = 0f;
            posicionPrevia = transform.position;
            return;
        }

        // si lleva un rato sin moverse es que está atascado, busca otro destino
        timerAtascado += Time.fixedDeltaTime;
        if (timerAtascado >= tiempoMaxAtascado)
        {
            if (Vector2.Distance(transform.position, posicionPrevia) < 0.15f)
                ElegirNuevoDestino();
            timerAtascado = 0f;
            posicionPrevia = transform.position;
        }
    }

    private void ElegirNuevoDestino()
    {
        float x = Random.Range(mapaMin.x, mapaMax.x);
        float y = Random.Range(mapaMin.y, mapaMax.y);
        destinoPatrulla = new Vector3(x, y, 0f);
    }

    private void EjecutarPersecucion()
    {
        if (jugador == null) { rb.velocity = Vector2.zero; return; }
        Vector2 dir = ((Vector2)jugador.position - (Vector2)transform.position).normalized;
        rb.velocity = dir * velocidadPersecucion;
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext((int)estadoActual);
        }
        else
        {
            posicionRed = (Vector3)stream.ReceiveNext();
            estadoRed = (Estado)(int)stream.ReceiveNext();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 centro = new Vector3((mapaMin.x + mapaMax.x) / 2f, (mapaMin.y + mapaMax.y) / 2f, 0f);
        Vector3 tamanio = new Vector3(mapaMax.x - mapaMin.x, mapaMax.y - mapaMin.y, 0f);
        Gizmos.DrawWireCube(centro, tamanio);

        if (Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(destinoPatrulla, 0.2f);
            Gizmos.DrawLine(transform.position, destinoPatrulla);
        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radioDeteccion);
    }
}
