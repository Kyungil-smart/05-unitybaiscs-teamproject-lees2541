using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Side_GoalController : MonoBehaviour
{

    private void Update()
    {
        _setRotation();
    }
    private void _setRotation()
    { 
        // 예시: Y축으로 매 프레임 3도씩 회전
        transform.Rotate(0, 3, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Goal!!");

            Destroy(gameObject);
        }
    }
}
