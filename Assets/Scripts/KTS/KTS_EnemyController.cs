using System;
using System.Collections.Generic;
using UnityChan.Combat;
using UnityEngine;
using UnityEngine.AI;

public class KTS_EnemyController : MonoBehaviour
{
	[SerializeField] private List<Transform> path;
	[SerializeField] private GameObject keyPrefab;
	[SerializeField, Range(0, 2f)] private float attackRange = 1f;
	[SerializeField] private float damage = 10f;
	[SerializeField] private float attackCooldown = 3f;

	private HealthSystem healthSystem;
	private NavMeshAgent navMeshAgent;
	private Animator animator;
	private Transform trackTarget;

	private int currentPatrolPointIndex;
	private float lastAttackTime;

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
		animator = GetComponent<Animator>();
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
				TryAttack();
				break;
			case State.Dead:
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private void TryAttack()
	{
		navMeshAgent.SetDestination(trackTarget.position);
		animator.SetBool("IsMoving", navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance);

		if (Vector3.Distance(trackTarget.position, transform.position) < attackRange)
		{
			Attack();
		}
	}

	private void Attack()
	{
		if (Time.time < lastAttackTime + attackCooldown) return;

		var damageable = trackTarget.GetComponent<IDamageable>();
		damageable.TakeDamage(damage, gameObject);
		lastAttackTime = Time.time;
		animator.SetTrigger("Attack");
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
		state = State.Dead;
		var go = Instantiate(keyPrefab, transform.position, Quaternion.identity);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			state = State.Attack;
			trackTarget = other.transform;
			animator.SetBool("FoundEnemy", true);
		}
	}
}