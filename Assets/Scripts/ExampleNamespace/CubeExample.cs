using UnityEngine;

namespace ExampleNamespace
{
	public class CubeExample : MonoBehaviour
	{
		public bool once;

		public GameObject EnableText;
		public GameObject DisableText;
		public GameObject InteractedText;

		public void OnInteracted()
		{
			if (once) return;
			once = true;
			InteractedText.SetActive(true);
			EnableText.SetActive(false);
			DisableText.SetActive(false);
		}

		public void OnFocusIn()
		{
			if (once) return;
			EnableText.SetActive(true);
			DisableText.SetActive(false);
		}

		public void OnFocusOut()
		{
			if (once) return;
			EnableText.SetActive(false);
			DisableText.SetActive(true);
		}
	}
}