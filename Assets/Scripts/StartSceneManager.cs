using UnityEngine;

public class StartSceneManager : MonoBehaviour
{
	public void OnStartBtnClicked()
	{
		PlayerPrefs.SetFloat("StartTime", Time.time);
		LevelLoader.Instance.LoadLevel("FirstPersonScene");
	}

	public void OnExitBtnClicked()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#endif
		Application.Quit();
	}
}