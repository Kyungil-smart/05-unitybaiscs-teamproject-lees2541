using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneManager : MonoBehaviour
{
	public void OnStartBtnClicked()
	{
		PlayerPrefs.SetFloat("StartTime", Time.time);
		SceneManager.LoadScene(1);
	}

	public void OnExitBtnClicked()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#endif
		Application.Quit();
	}
}