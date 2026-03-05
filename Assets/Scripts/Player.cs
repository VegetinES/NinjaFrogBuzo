using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Player : MonoBehaviour
{
    public float speed, jumpForce;
    private Rigidbody2D rig;
    private Animator anim;
    private bool lastFlipState = false;
    private bool isDead = false;

    [Header("Audio")]
    public AudioClip jumpSound; // Sonido de salto
    public AudioClip deathSound; // Sonido de muerte
    private AudioSource audioSource;

    void Start()
    {
        Debug.Log("=== PLAYER START ===");
        Debug.Log("GameObject: " + gameObject.name);
        Debug.Log("Posición inicial: " + transform.position);
        
        PhotonView pv = GetComponent<PhotonView>();
        if (pv != null)
        {
            Debug.Log("PhotonView encontrado - ViewID: " + pv.ViewID);
            Debug.Log("¿Es mío (IsMine)? " + pv.IsMine);
            Debug.Log("Owner: " + pv.Owner.NickName);
        }
        else
        {
            Debug.LogError("❌ PhotonView NO encontrado!");
        }
        
        if (GetComponent<PhotonView>().IsMine)
        {
            Debug.Log("✅ ESTE PERSONAJE ES MÍO - Inicializando controles");
            
            rig = GetComponent<Rigidbody2D>();
            if (rig != null)
            {
                Debug.Log("✅ Rigidbody2D encontrado");
            }
            else
            {
                Debug.LogError("❌ Rigidbody2D NO encontrado!");
            }

            // Audio
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.volume = 0.5f;

            if (Camera.main != null)
            {
                Camera.main.transform.SetParent(transform);
                Camera.main.transform.localPosition = new Vector3(0, 0, -10);
            }
        }
        else
        {
            Debug.Log("⚪ Este personaje NO es mío - Solo se visualiza");
        }
        
        anim = GetComponent<Animator>();
        if (anim != null)
        {
            Debug.Log("✅ Animator encontrado");
        }
        else
        {
            Debug.LogError("❌ Animator NO encontrado!");
        }
    }

    void Update()
    {
        if (!GetComponent<PhotonView>().IsMine || isDead) return;

        // Movimiento
        float horizontal = Input.GetAxis("Horizontal");
        Vector2 newVelocity = (transform.right * speed * horizontal) + (transform.up * rig.velocity.y);
        rig.velocity = newVelocity;
        
        // Debug de movimiento (solo ocasionalmente para no saturar)
        if (horizontal != 0 && Time.frameCount % 60 == 0)
        {
            Debug.Log("Movimiento - Input: " + horizontal + " | Velocidad: " + rig.velocity);
        }
        
        // Flip con RPC optimizado
        if (rig.velocity.x > 0.1f && lastFlipState == true)
        {
            Debug.Log("⬅️ Flip a DERECHA (false)");
            GetComponent<PhotonView>().RPC("RotateSprite", RpcTarget.All, false);
            lastFlipState = false;
        }
        else if (rig.velocity.x < -0.1f && lastFlipState == false)
        {
            Debug.Log("➡️ Flip a IZQUIERDA (true)");
            GetComponent<PhotonView>().RPC("RotateSprite", RpcTarget.All, true);
            lastFlipState = true;
        }
        
        // Salto
        if (Input.GetButtonDown("Jump"))
        {
            Debug.Log("🦘 SALTO! - Fuerza: " + jumpForce);
            rig.AddForce(transform.up * jumpForce);

            if (jumpSound != null && audioSource != null)
                audioSource.PlayOneShot(jumpSound);
        }
        
        // Animaciones
        float velX = Mathf.Abs(rig.velocity.x);
        float velY = rig.velocity.y;
        anim.SetFloat("velocityX", velX);
        anim.SetFloat("velocityY", velY);
        
        // Debug de animación (ocasional)
        if (Time.frameCount % 120 == 0)
        {
            Debug.Log("Animator - velocityX: " + velX + " | velocityY: " + velY);
        }
    }

    [PunRPC]
    public void RotateSprite(bool rotate)
    {
        GetComponent<SpriteRenderer>().flipX = rotate;
        Debug.Log("RPC RotateSprite ejecutado - flipX: " + rotate + " en " + gameObject.name);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!GetComponent<PhotonView>().IsMine) return;

        Strawberry strawberry = col.GetComponent<Strawberry>();
        if (strawberry != null && strawberry.spawner != null)
        {
            strawberry.spawner.CollectStrawberry(strawberry.strawberryId, PhotonNetwork.LocalPlayer.ActorNumber);
        }
    }
}
