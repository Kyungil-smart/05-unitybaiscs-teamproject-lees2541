using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Boss.Skills
{
	public class BossBasicSkill : MonoBehaviour, IBossSkill
	{
		[SerializeField] private int projectileCount = 10;
		[SerializeField] private GameObject projectilePrefab;

		private static readonly YieldInstruction startYield = new WaitForSeconds(4f);
		private static readonly YieldInstruction performYield = new WaitForSeconds(1f);

		private Stack<BossBasicSkillProjectile> projectiles = new();
		private bool isCasting;

		void Init()
		{
			while (projectiles.Count > 0)
			{
				Destroy(projectiles.Pop().gameObject);
			}
		}

		public IEnumerator StartAttack()
		{
			for (int i = 0; i < projectileCount; i++)
			{
				var pos = new Vector3(Random.Range(-10, 10), 5, Random.Range(-10, 10));
				var go = Instantiate(projectilePrefab, pos, Quaternion.identity);
				if (go.TryGetComponent(out BossBasicSkillProjectile proj))
					projectiles.Push(proj);
				else
				{
					Debug.LogWarning(
						$"{nameof(BossBasicSkillProjectile)} component is not attached on {projectilePrefab}");
					break;
				}
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
			while (projectiles.Count > 0)
				Destroy(projectiles.Pop().gameObject);
			projectiles.Clear();
			yield return null;
		}
	}
}