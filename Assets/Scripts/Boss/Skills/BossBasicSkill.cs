using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boss.Skills
{
	/// <summary>
	/// 투사체를 생성한 뒤 대기시간 이후 추락시킵니다. 투사체에 부딪힌 경우 플레이어는 피해를 입습니다.
	/// </summary>
	public class BossBasicSkill : MonoBehaviour, IBossSkill
	{
		public BossSkillType Type => BossSkillType.BasicCast;

		[SerializeField] private int projectileCount = 10;
		[SerializeField] private GameObject projectilePrefab;

		private static readonly YieldInstruction startYield = new WaitForSeconds(4f);
		private static readonly YieldInstruction performYield = new WaitForSeconds(1f);

		private Stack<BossBasicSkillProjectile> projectiles = new();
		private Vector3 minPos;
		private Vector3 maxPos;

		private void Start()
		{
			minPos = BossSceneManager.Instance.MinCoordinate;
			maxPos = BossSceneManager.Instance.MaxCoordinate;

			projectiles.Clear();
			Transform root = new GameObject($"@{nameof(BossBasicSkill)}").transform;
			root.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

			for (int i = 0; i < projectileCount; i++)
			{
				var obj = Instantiate(projectilePrefab, root.transform);
				obj.SetActive(false);
				projectiles.Push(obj.GetComponent<BossBasicSkillProjectile>());
			}
		}


		public IEnumerator StartAttack()
		{
			BossSceneManager.Instance.SetCamera(BossSceneManager.BossCameraInfo.BossCameraType.BossTop);
			foreach (var projectile in projectiles)
			{
				Vector3 randomPos = minPos.RandomRange(maxPos);
				randomPos.y = 4f;
				projectile.transform.SetPositionAndRotation(randomPos, Quaternion.identity);
				projectile.gameObject.SetActive(true);
			}

			yield return startYield;
		}

		public IEnumerator PerformAttack()
		{
			foreach (var p in projectiles)
			{
				p.Activate();
			}

			yield return performYield;
		}

		public IEnumerator EndAttack()
		{
			BossSceneManager.Instance.SetCamera(BossSceneManager.BossCameraInfo.BossCameraType.PlayerTop);
			foreach (var p in projectiles)
			{
				p.gameObject.SetActive(false);
			}

			yield return null;
		}
	}
}