using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("มกวม!");
    }
}
