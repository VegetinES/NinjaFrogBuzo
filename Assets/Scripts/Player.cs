using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Player : MonoBehaviour
{
    public float speed, jumpForce;
    private Rigidbody2D rig;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<PhotonView>().IsMine) //Si no es mío este script
        {
            rig = GetComponent<Rigidbody2D>();
        }
        anim = GetComponent<Animator>();
        
        //Ojo las cámaras no se sincronizan
        Camera.main.transform.SetParent(transform);
        Camera.main.transform.position = transform.position + (Vector3.up) + transform.forward*-10;
    }

    // Update is called once per frame
    void Update()
    {
        // Le damos velocidad pero también en el eje y porque sino se quedaría parado.
        if (GetComponent<PhotonView>().IsMine)
        {
            rig.velocity = (transform.right * speed * Input.GetAxis("Horizontal")) + (transform.up * rig.velocity.y);
            
            if (rig.velocity.x > 0.1f) //Cambiamos la imagen de movimiento
                GetComponent<PhotonView>().RPC("RotateSprite", RpcTarget.All, false);
            else if (rig.velocity.x < 0.1f)
                GetComponent<PhotonView>().RPC("RotateSprite", RpcTarget.All, true);
            
            //añadimos el salto
            if (Input.GetButtonDown("Jump"))
            {
                rig.AddForce(transform.up * jumpForce);
            }
            
            //Añadimos la animación.
            anim.SetFloat("velocityX", Mathf.Abs(rig.velocity.x));
            anim.SetFloat("velocityY", rig.velocity.y);
        }
    }

    [PunRPC]
    public void RotateSprite(bool rotate)
    {
        GetComponent<SpriteRenderer>().flipX = rotate;
    }
}
