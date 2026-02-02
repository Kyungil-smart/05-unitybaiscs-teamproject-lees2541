using System.Collections;
using System.Collections.Generic;
using UnityChan;
using UnityEditor;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMove GetJump = other.GetComponent<PlayerMove>();

            if (GetJump != null)
            {
                GetJump.isJump = false;
            }

            Debug.Log("AddJumpItem!");

            gameObject.SetActive(false);
        }
    }
}