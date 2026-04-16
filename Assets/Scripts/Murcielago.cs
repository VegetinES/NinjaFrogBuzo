using UnityEngine;

public class Murcielago : MonoBehaviour
{
    private Transform jugador;
    [SerializeField] private float distancia;
    public Vector3 puntoInicial;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        puntoInicial = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Inicializar a distancia grande para que el Animator no dispare Seguir antes de encontrar al jugador
        animator.SetFloat("Distancia", 999f);

        GameObject jugadorObj = GameObject.FindGameObjectWithTag("Player");
        if (jugadorObj != null)
        {
            jugador = jugadorObj.transform;
            distancia = Vector2.Distance(transform.position, jugador.position);
            animator.SetFloat("Distancia", distancia);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (jugador == null)
        {
            GameObject jugadorObj = GameObject.FindGameObjectWithTag("Player");
            if (jugadorObj != null)
                jugador = jugadorObj.transform;
            return;
        }

        distancia = Vector2.Distance(transform.position, jugador.position);
        animator.SetFloat("Distancia", distancia);
    }

    public void Girar(Vector3 objetivo)
    {
        if (transform.position.x < objetivo.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }
}