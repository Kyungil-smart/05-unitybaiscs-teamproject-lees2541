using UnityEngine;

public class SideViewSceneManager : MonoBehaviour
{
	[SerializeField] private TextUIPanel mainTitle;

	private void Start()
	{
		mainTitle.ActivateOnce();
	}
}