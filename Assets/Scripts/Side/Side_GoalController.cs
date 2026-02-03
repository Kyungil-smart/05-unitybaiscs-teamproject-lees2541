using UnityEngine;

public class Side_GoalController : MonoBehaviour
{

    private void Update()
    {
        _setRotation();
    }
    private void _setRotation()
    { 
        // ����: Y������ �� ������ 3���� ȸ��
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
