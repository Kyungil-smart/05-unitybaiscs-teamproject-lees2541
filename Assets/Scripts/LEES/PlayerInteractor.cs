using UnityEngine;

namespace UnityChan
{
	public class PlayerInteractor : MonoBehaviour
	{
		[Header("First Person (Look Ray)")] [SerializeField]
		private Transform rayOrigin; // 비워두면 Camera.main 사용

		[SerializeField] private float rayDistance = 2f;

		[Header("Third Person / Quarter View (Proximity)")] [SerializeField]
		private float proximityRadius = 3f;

		[SerializeField] private float proximityScanInterval = 0.05f; // 0.05~0.1 권장
		[SerializeField] private bool includeTriggers = true;

		public IInteract CurrentInteractable
		{
			get => currentInteractable;
			set
			{
				currentInteractable?.OnInteractFocusExit();
				currentInteractable = value;
				Debug.Log("CurrentInteractable: " + currentInteractable ?? "None");
				currentInteractable?.OnInteractFocusEnter();
			}
		}

		private IInteract currentInteractable;

		private PlayerController controller;
		private Collider[] selfColliders;
		private readonly Collider[] overlapBuffer = new Collider[32];

		private float scanTimer;

		private void Awake()
		{
			controller = GetComponent<PlayerController>();
			selfColliders = GetComponentsInChildren<Collider>();
		}

		private void Update()
		{
			// UpdateTarget(); REFACTOR: 플레이어 전방 기준으로만 Interact 시도하게 구현했습니다.
			TryUpdateInteractable();

			if (Input.GetKeyDown(KeyCode.F))
			{
				CurrentInteractable.Interact();
			}
		}

		private void TryUpdateInteractable()
		{
			Ray forwardRay = new Ray(transform.position + Vector3.up * 1.3f, transform.forward);
			Ray cameraRay = new Ray(transform.position + Vector3.up * 1.3f,
				controller.MainCameraTransform.forward);

			if (Physics.Raycast(forwardRay, out RaycastHit hit, rayDistance))
			{
				if (hit.collider.gameObject.TryGetComponent(out IInteract interact) && interact != CurrentInteractable)
					CurrentInteractable = interact;
			}
			else if (Physics.Raycast(cameraRay, out RaycastHit hit2, rayDistance))
			{
				if (hit2.collider.gameObject.TryGetComponent(out IInteract interact) && interact != CurrentInteractable)
					CurrentInteractable = interact;
			}
			else
			{
				if (CurrentInteractable != null)
					CurrentInteractable = null;
			}
		}

		private void UpdateTarget()
		{
			// 1인칭: 바라보는 대상 레이캐스트
			if (controller != null && controller.PerspectiveState == PlayerController.MovementSpace.Camera)
			{
				UpdateByRaycast();
				return;
			}

			// 쿼터뷰/사이드뷰: 근접(반경) 검사
			scanTimer -= Time.deltaTime;
			if (scanTimer <= 0f)
			{
				scanTimer = proximityScanInterval;
				UpdateByProximity();
			}
		}

		private Transform GetRayOrigin()
		{
			if (rayOrigin != null) return rayOrigin;
			var cam = Camera.main;
			return cam != null ? cam.transform : transform;
		}

		private void UpdateByRaycast()
		{
			Transform origin = GetRayOrigin();
			Ray ray = new Ray(origin.position, origin.forward);

			if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, ~0,
				    includeTriggers ? QueryTriggerInteraction.Collide : QueryTriggerInteraction.Ignore))
			{
				if (hit.collider.gameObject.TryGetComponent(out IInteract interact))
				{
					CurrentInteractable = interact;
					return;
				}
			}

			CurrentInteractable = null;
		}

		private void UpdateByProximity()
		{
			var qti = includeTriggers ? QueryTriggerInteraction.Collide : QueryTriggerInteraction.Ignore;

			int count = Physics.OverlapSphereNonAlloc(transform.position, proximityRadius, overlapBuffer, ~0, qti);

			IInteract best = null;
			float bestD2 = float.PositiveInfinity;

			for (int i = 0; i < count; i++)
			{
				var c = overlapBuffer[i];
				if (c == null) continue;
				if (IsSelfCollider(c)) continue;

				if (!c.gameObject.TryGetComponent(out IInteract interact)) continue;

				Vector3 closest = c.ClosestPoint(transform.position);
				float d2 = (closest - transform.position).sqrMagnitude;

				if (d2 < bestD2)
				{
					bestD2 = d2;
					best = interact;
				}
			}

			CurrentInteractable = best;
		}

		private bool TryGetInteract(Collider col, out IInteract interact)
		{
			return col.gameObject.TryGetComponent(out interact);
		}

		private bool IsSelfCollider(Collider other)
		{
			for (int i = 0; i < selfColliders.Length; i++)
				if (selfColliders[i] == other)
					return true;
			return false;
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawRay(transform.position + Vector3.up * 1.3f, transform.forward * rayDistance);
			// Gizmos.DrawWireSphere(transform.position, proximityRadius);
		}
	}
}