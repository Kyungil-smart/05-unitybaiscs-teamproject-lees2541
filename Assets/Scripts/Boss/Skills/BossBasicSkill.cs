using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BossBasicSkill : MonoBehaviour, IBossSkill
{
	[SerializeField] private GameObject projectilePrefab;
	[SerializeField] [Range(0.1f, 5f)] private float performDelay = 3f;

	private YieldInstruction yieldDelay;
	private List<BossBasicSkillProjectile> projectiles;

	private void Awake()
	{
		yieldDelay = new WaitForSeconds(performDelay);
	}

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

		yield return yieldDelay;
	}

	public IEnumerator PerformAttack()
	{
		
		yield return null;
	}

	public IEnumerator EndAttack()
	{
		projectiles.ForEach(Destroy);
		projectiles.Clear();
		yield return null;
	}
}