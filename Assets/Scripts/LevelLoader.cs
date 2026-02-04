using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
	private static LevelLoader instance;

	public static LevelLoader Instance
	{
		get
		{
			if (instance == null)
			{
				CreateStaticObject();
			}

			return instance;
		}
	}

	public GameObject canvas;
	public Animator animator;

	private static void CreateStaticObject()
	{
		var prefab = Resources.Load<GameObject>("SceneLoader");
		var go = Instantiate(prefab);
		instance = go.GetComponent<LevelLoader>();
		DontDestroyOnLoad(go);
	}

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public void LoadLevel(string sceneName)
	{
		StartCoroutine(Load(sceneName));
	}

	private IEnumerator Load(string sceneName)
	{
		canvas.SetActive(true);
		animator.SetTrigger("LoadStart");
		yield return new WaitForSeconds(0.6f);

		SceneManager.LoadScene(sceneName);
		yield return new WaitForSeconds(0.1f);

		animator.SetBool("LoadEnd", true);
		yield return new WaitForSeconds(0.6f);
		animator.SetBool("LoadEnd", false);
		canvas.SetActive(false);
	}
}