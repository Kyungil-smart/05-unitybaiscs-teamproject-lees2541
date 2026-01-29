using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Boss.Skills
{
	public class BossBasicSkill : MonoBehaviour, IBossSkill
	{
		[SerializeField] private GameObject projectilePrefab;

		private static readonly YieldInstruction startYield = new WaitForSeconds(4f);
		private static readonly YieldInstruction performYield = new WaitForSeconds(1f);

		private List<BossBasicSkillProjectile> projectiles;


		void Init()
		{
			projectiles = new List<BossBasicSkillProjectile>();
		}

		public IEnumerator StartAttack()
		{
			Init();
			for (int i = 0; i < 10; i++)
			{
				var pos = new Vector3(Random.Range(-10, 10), 5, Random.Range(-10, 10));
				projectiles.Add(Instantiate(projectilePrefab, pos, Quaternion.identity)
					.GetComponent<BossBasicSkillProjectile>());
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
			projectiles.ForEach(p => Destroy(p.gameObject));
			projectiles.Clear();
			yield return null;
		}
	}
}