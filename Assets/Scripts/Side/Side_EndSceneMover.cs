using UnityEngine;
using UnityEngine.SceneManagement;
public class Side_EndSceneMover : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("QuarterViewScene");
        }
    }
}
