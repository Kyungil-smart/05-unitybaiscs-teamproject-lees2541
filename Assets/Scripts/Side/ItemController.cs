using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            U_ChanMove uChanMove = other.GetComponent<U_ChanMove>();

            if (uChanMove != null)
            {
                uChanMove.isJump = false;
            }

            Destroy(gameObject);
        }
    }
}