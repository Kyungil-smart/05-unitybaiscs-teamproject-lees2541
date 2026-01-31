using System;
using System.Collections;
using System.Linq;
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
		private BossStat stat;
		private YieldInstruction skillYield = new WaitForSeconds(2.5f);

		private void Awake()
		{
			controller = GetComponent<BossController>();
			stat = GetComponent<BossStat>();
		}

		private void Start()
		{
			StartCoroutine(BossLogic());
		}

		IEnumerator BossLogic()
		{
			var values = Enum.GetValues(typeof(BossSkillType)).Cast<BossSkillType>().ToArray();
			while (!stat.IsDie)
			{
				BossSkillType randomElement = values[Random.Range(0, values.Length)];
				controller.CastSkill(randomElement);

				yield return skillYield;
				while (controller.IsCasting) yield return null;
			}

			Debug.Log("Died");
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