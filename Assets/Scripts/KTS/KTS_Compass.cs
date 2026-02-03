using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KTS_Compass : MonoBehaviour
{
	private Animator animator;

	private void Start()
	{
		StartCoroutine(InitAnimation());
	}

	IEnumerator InitAnimation()
	{
		while (transform.position.y < 1)
		{
			transform.Translate(Vector3.up * (0.5f * Time.deltaTime));
			yield return null;
		}

		animator = GetComponent<Animator>();
		animator.SetTrigger("Start");
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			LevelLoader.Instance.LoadLevel("BossStage");
		}
	}
}