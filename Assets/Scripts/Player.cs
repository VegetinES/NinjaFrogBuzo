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
            
            // Configurar cámara
            Debug.Log("Buscando Camera.main...");
            if (Camera.main != null)
            {
                Debug.Log("✅ Camera.main encontrada en: " + Camera.main.transform.position);
                Camera.main.transform.SetParent(transform);
                Camera.main.transform.localPosition = new Vector3(0, 0, -10);
                Debug.Log("Cámara reposicionada a localPosition: " + Camera.main.transform.localPosition);
                Debug.Log("Posición mundial de cámara: " + Camera.main.transform.position);
            }
            else
            {
                Debug.LogError("❌ Camera.main NO encontrada!");
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
        if (GetComponent<PhotonView>().IsMine)
        {
            // Movimiento
            float horizontal = Input.GetAxis("Horizontal");
            Vector2 newVelocity = (transform.right * speed * horizontal) + (transform.up * rig.velocity.y);
            rig.velocity = newVelocity;
            
            // Debug de movimiento (solo ocasionalmente para no saturar)
            if (horizontal != 0 && Time.frameCount % 60 == 0) // Cada 60 frames
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
            }
            
            // Animaciones
            float velX = Mathf.Abs(rig.velocity.x);
            float velY = rig.velocity.y;
            anim.SetFloat("velocityX", velX);
            anim.SetFloat("velocityY", velY);
            
            // Debug de animación (ocasional)
            if (Time.frameCount % 120 == 0) // Cada 2 segundos
            {
                Debug.Log("Animator - velocityX: " + velX + " | velocityY: " + velY);
            }
        }
    }

    [PunRPC]
    public void RotateSprite(bool rotate)
    {
        GetComponent<SpriteRenderer>().flipX = rotate;
        Debug.Log("RPC RotateSprite ejecutado - flipX: " + rotate + " en " + gameObject.name);
    }
}
