using UnityChan;
using UnityEngine;

public class Side_AddJump : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Side_PlayerController GetJump = other.GetComponent<Side_PlayerController>();

            if (GetJump != null)
            {
                GetJump.isJump = false;
            }

            Debug.Log("AddJumpItem!");

            gameObject.SetActive(false);
        }
    }
}