using System.Collections;
using System.Collections.Generic;
using UnityChan;
using UnityEditor;
using UnityEngine;

public class Side_AddJump : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController GetJump = other.GetComponent<PlayerController>();

            if (GetJump != null)
            {
                // 예시: jumpConsumed를 false로 설정 (원하는 동작에 맞게 수정)
                //GetJump.jumpConsumed = false;
            }

            Debug.Log("AddJumpItem!");

            gameObject.SetActive(false);
        }
    }
}