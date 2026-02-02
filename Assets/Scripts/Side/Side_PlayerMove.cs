using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Side_layerMove : MonoBehaviour
{
    float haxis;
    Vector3 moveVec;
    Rigidbody rigid;
    bool jDown;
    public bool isJump;
    private Animator anim;
    [SerializeField][Range(0, 20)] private float _movespeed;
    [SerializeField][Range(0, 20)] private float _jumpforce;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
    }

    void GetInput()
    {
        haxis = Input.GetAxisRaw("Horizontal");
        jDown = Input.GetButtonDown("Jump");
    }

    void Move()
    {
        moveVec = new Vector3(0, 0, haxis).normalized;
        transform.position += moveVec * _movespeed * Time.deltaTime;
    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVec);
    }

    void Jump()
    {
        if (jDown && !isJump)
        {
            rigid.AddForce(Vector3.up * _jumpforce, ForceMode.Impulse);
            isJump = true;
            anim.SetBool("Jump", true);
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isJump = false;
        }
    }
}
