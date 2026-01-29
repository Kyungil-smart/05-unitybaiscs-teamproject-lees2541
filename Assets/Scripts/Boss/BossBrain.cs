using Boss.Skills;
using Boss.VFX;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Boss
{
	public class BossBrain : MonoBehaviour
	{
		[SerializeField] private bool enableDebug;

		private BossController controller;

		private void Awake()
		{
			controller = GetComponent<BossController>();
		}

		private void Update()
		{
			// Test
			if (Input.GetMouseButtonDown(0))
			{
				BossVFXManager.Instance.Spawn(VFXType.GroundDustExplosion,
					new Vector3(Random.Range(-5, 5), 0.01f, Random.Range(-5, 5)), Quaternion.identity);
			}
		}

		private void OnGUI()
		{
			if (!enableDebug) return;
			if (GUILayout.Button("Cast Basic"))
			{
				controller.CastSkill(BossSkillType.BasicCast);
			}

			if (GUILayout.Button("Cast HorizontalLaser"))
			{
				controller.CastSkill(BossSkillType.HorizontalLaser);
			}


			if (GUILayout.Button("Cast VerticalLaser"))
			{
				controller.CastSkill(BossSkillType.VerticalLaser);
			}
		}
	}
}