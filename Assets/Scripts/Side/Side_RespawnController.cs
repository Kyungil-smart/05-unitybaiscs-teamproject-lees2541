using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Search;
using UnityChan;
public class Side_RespawnController : MonoBehaviour
{
    private GameObject[] items;
    public Vector3 respawnPos = new Vector3(0, 3, 4);

    private void Start()
    {
        items = GameObject.FindGameObjectsWithTag("AddJumpChance");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Dead!");

            PlayerController SetPosition = other.GetComponent<PlayerController>();
            if (SetPosition != null)
            {
                SetPosition.enabled = false;
                other.transform.position = respawnPos;
                SetPosition.enabled = true;
            }
            else
            {
                other.transform.position = respawnPos;
            }
            PlayerRespawn();
        }
    }

    private void Update()
    { 
        if(Input.GetKeyDown(KeyCode.F5))
        {
            ItemRespawn();
        }
    }
    private void ItemRespawn()
    {
        if (items != null)
        {
            foreach (var item in items)
            {
                if (item != null)
                    item.SetActive(true);
            }
        }
    }

    private void PlayerRespawn()
    {
        Debug.Log("Respawn Item");
        foreach (var item in items)
        {
            if (item != null)
                item.SetActive(true);
        }
    }

}