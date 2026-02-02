using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Side_GoalController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Goal!!");

            Destroy(gameObject);
        }
    }
}
