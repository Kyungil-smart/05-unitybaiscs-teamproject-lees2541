using System;
using System.Collections;
using System.Linq;
using Boss.Skills;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Boss
{
	public class BossBrain : MonoBehaviour
	{
		[SerializeField] private bool enableDebug;

		private BossController controller;
		private BossStat stat;
		private YieldInstruction skillYield = new WaitForSeconds(2.5f);

		public UnityEvent BossDied;

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

			Die();
		}

		void Die()
		{
			StopAllCoroutines();
			controller.StopAllCoroutines();
			controller.CancelSkillForce();
			BossSceneManager.Instance.SetCamera(BossSceneManager.BossCameraInfo.BossCameraType.BossDie);
			BossDied?.Invoke();

			Invoke(nameof(CallSceneManager), 3f);
		}

		void CallSceneManager()
		{
			BossSceneManager.Instance.GameEnd();
		}

		private void OnGUI()
		{
			if (!enableDebug) return;
			if (GUILayout.Button("Kill Boss"))
			{
				Die();
			}

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