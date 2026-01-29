using System.Collections;
using System.Collections.Generic;
using Boss.VFX;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Boss.Skills
{
	/// <summary>
	/// 맵의 가로 축 기준으로 레이저를 발사합니다. 발사할 위치와 방향을 강조한 뒤 대기 시간 이후 공격합니다.
	/// </summary>
	public class BossHorizontalLaserSkill : MonoBehaviour, IBossSkill
	{
		public BossSkillType Type => BossSkillType.HorizontalLaser;

		[SerializeField] private int laserCount = 3;
		[SerializeField] private GameObject indicatorPrefab;

		private readonly Stack<(Vector3, Vector3)> laserData = new(); // (Origin, Direction) 튜플 값을 갖는 Stack
		private readonly List<LineRenderer> indicators = new();
		private readonly YieldInstruction startYield = new WaitForSeconds(2);
		private readonly YieldInstruction performYield = new WaitForSeconds(1);

		private float effectPosX = -50f;
		private float effectPosY = 0.6f;
		private float laserLength = 100f;
		private Vector3 minPos;
		private Vector3 maxPos;

		private void Start()
		{
			minPos = BossSceneManager.Instance.MinCoordinate;
			maxPos = BossSceneManager.Instance.MaxCoordinate;

			Transform root = new GameObject($"@{nameof(BossHorizontalLaserSkill)}").transform;
			root.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

			indicators.Clear();
			laserData.Clear();

			for (int i = 0; i < laserCount; i++)
			{
				var obj = Instantiate(indicatorPrefab, root);
				obj.SetActive(false);
				indicators.Add(obj.GetComponent<LineRenderer>());
			}
		}

		public IEnumerator StartAttack()
		{
			for (int i = 0; i < laserCount; i++)
			{
				Vector3 p0 = new Vector3(minPos.x, effectPosY, Random.Range(minPos.z, maxPos.z));
				Vector3 p1 = new Vector3(maxPos.x, effectPosY, Random.Range(minPos.z, maxPos.z));
				Vector3 dir = (p1 - p0).normalized;
				laserData.Push((p0, dir));

				// 중점 기준으로 양쪽으로 뻗기 (화면 중앙을 지나도록)
				Vector3 mid = (p0 + p1) * 0.5f;
				Vector3 start = mid - dir * 100;
				Vector3 end = mid + dir * 100;

				var lr = indicators[i];
				lr.SetPosition(0, start);
				lr.SetPosition(1, end);
				indicators[i].gameObject.SetActive(true);
			}

			yield return startYield;
		}

		public IEnumerator PerformAttack()
		{
			foreach (var line in indicators)
			{
				line.gameObject.SetActive(false);
			}

			foreach (var laserTuple in laserData)
			{
				BossVFXManager.Instance.Spawn(VFXType.HorizontalLineCrack, laserTuple.Item1,
					Quaternion.LookRotation(laserTuple.Item2));

				// BoxSweep[Physics.BoxCast()] and Attack
			}

			yield return performYield;
		}

		public IEnumerator EndAttack()
		{
			laserData.Clear();

			yield return null;
		}
	}
}