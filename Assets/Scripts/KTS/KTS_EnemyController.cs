using System;
using System.Collections;
using System.Collections.Generic;
using UnityChan.Combat;
using UnityEngine;
using UnityEngine.AI;

public class KTS_EnemyController : MonoBehaviour
{
	[SerializeField] private List<Transform> path;
	[SerializeField] private GameObject keyPrefab;

	private HealthSystem healthSystem;
	private NavMeshAgent navMeshAgent;

	private int currentPatrolPointIndex = 0;

	enum State
	{
		Patrol,
		Attack,
		Dead
	}

	private State state = State.Patrol;

	private void Awake()
	{
		healthSystem = GetComponent<HealthSystem>();
		navMeshAgent = GetComponent<NavMeshAgent>();
	}

	private void Start()
	{
		healthSystem.OnDeath += Die;
		currentPatrolPointIndex = 0;
		Vector3 targetPos = path[currentPatrolPointIndex].position;
		navMeshAgent.SetDestination(targetPos);
	}

	private void Update()
	{
		switch (state)
		{
			case State.Patrol:
				Move();
				break;
			case State.Attack:
				break;
			case State.Dead:
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private void Move()
	{
		if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance) return;

		currentPatrolPointIndex = (currentPatrolPointIndex + 1) % path.Count;
		Vector3 targetPos = path[currentPatrolPointIndex].position;
		navMeshAgent.SetDestination(targetPos);
	}


	private void Die()
	{
		var go = Instantiate(keyPrefab, transform.position, Quaternion.identity);
	}
}