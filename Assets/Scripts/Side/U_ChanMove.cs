using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class U_ChanMove : MonoBehaviour
{
    float haxis;
    Vector3 moveVec;
    Rigidbody rigid;
    bool jDown;
    bool isJump;
    [SerializeField][Range(0, 20)] private float _movespeed;
    [SerializeField][Range(0, 20)] private float _jumpforce;

    void Awake()
    {
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
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isJump = false;
        }

        else if (collision.gameObject.CompareTag("AddJumpChance"))
        {
            isJump = false;

            Destroy(collision.gameObject);
        }
    }
}
