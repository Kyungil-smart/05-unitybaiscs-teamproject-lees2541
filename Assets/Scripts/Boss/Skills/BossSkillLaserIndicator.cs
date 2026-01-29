using System;
using UnityEngine;

namespace Boss.Skills
{
	public class BossSkillLaserIndicator : MonoBehaviour
	{
		private static readonly int property = Shader.PropertyToID("_AlphaOverrideChannel");
		private Material material;

		private void Start()
		{
			material = GetComponent<Renderer>().material;
		}

		private void Update()
		{
			material.SetColor(property,
				new Vector4(0, 0, 0, (Mathf.Sin(Time.time * 8) + 1) / 2));
		}
	}
}