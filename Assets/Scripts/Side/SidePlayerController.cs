using UnityEngine;

namespace UnityChan
{

	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(CapsuleCollider))]
	[RequireComponent(typeof(Rigidbody))]
	public class SidePlayerController : MonoBehaviour
	{
		public enum MovementSpace
		{
			World,
			Camera
		}

		[Header("Movement")] public MovementSpace PerspectiveState = MovementSpace.World;
		public Transform MainCameraTransform;

		[Header("Control")] public float moveSpeed = 7.0f;
		public float rotateSpeed = 7.0f;
		public float jumpPower = 1.0f;
		public LayerMask groundLayers;

		[Header("Animation")] public float animSpeed = 1.5f; // 애니메이션 재생 속도 설정
		public float lookSmoother = 3.0f; // a smoothing setting for camera motion
		public bool useCurves = true; // 메카님에서 커브 보정을 사용 유무
		public float useCurvesHeight = 0.5f; // 커브 보정 유효 높이(바닥을 뚫기 쉬우면 값을 크게)

		private CapsuleCollider col;
		private Rigidbody rb;
		private Animator anim;

		// 캐릭터 컨트롤러(캡슐 콜라이더) 이동량
		private Vector3 velocity;

		// --- Jump input buffer + grounded (안 씹히는 점프) ---
		private bool jumpRequest;
		private bool jumpConsumed;
		private float jumpRequestTimer;
		private const float groundedVelYThreshold = 0.05f; // 바닥에 닿은 것으로 간주하는 Y 속도 임계값
		private const float jumpBufferTime = 0.15f; // 점프 입력 버퍼(0.1~0.2 추천)

		private float coyoteTimer;
		private const float coyoteTime = 0.10f; // 바닥에서 살짝 떨어져도 점프 허용(선택이지만 체감 좋아짐)
		private const float groundEpsilon = 0.06f; // 바닥 판정 여유

		public bool jumpPending; // 점프 애니 시작 ~ 이륙 이벤트까지 대기
		private bool jumpTakeoffEvent; // Animation Event가 켜는 플래그(물리 적용은 FixedUpdate에서)

		// CapsuleCollider에 설정된 콜라이더 Height/Center 초기값을 저장하는 변수
		private float colliderOriginHeight;
		private Vector3 colliderOriginCenter;

		private AnimatorStateInfo currentBaseState; // base layer에서 사용하는 Animator의 현재 상태 참조

		static int locoState = Animator.StringToHash("Base Layer.Locomotion");
		static int jumpState = Animator.StringToHash("Base Layer.Jump");
		static int idleState = Animator.StringToHash("Base Layer.Idle");
		static int restState = Animator.StringToHash("Base Layer.Rest");


		private void Awake()
		{
			anim = GetComponent<Animator>();
			col = GetComponent<CapsuleCollider>();
			rb = GetComponent<Rigidbody>();
		}

		void Start()
		{
			if (MainCameraTransform == null)
				MainCameraTransform = Camera.main.transform;

			colliderOriginHeight = col.height;
			colliderOriginCenter = col.center;
		}

		void Update()
		{
			ProcessInput();

			// 버퍼 시간 감소
			if (jumpRequest)
			{
				jumpRequestTimer -= Time.deltaTime;
				if (jumpRequestTimer <= 0f)
				{
					jumpRequest = false;
					jumpRequestTimer = 0f;
				}
			}
		}

		void FixedUpdate()
		{
			Move();
			Jump();
		}

		public void SetCameraTransform(Transform tr)
		{
			MainCameraTransform = tr;
		}

		private void ProcessInput()
		{
			if (Input.GetButtonDown("Jump"))
			{
				jumpRequest = true;
				jumpRequestTimer = jumpBufferTime;
			}

			if (Input.GetKey(KeyCode.Q)) transform.Rotate(0, -1 * rotateSpeed, 0);
			if (Input.GetKey(KeyCode.E)) transform.Rotate(0, 1 * rotateSpeed, 0);
		}

		private void Jump()
		{
			// 1) 접지 판정: "올라가는 중"이면 접지로 보지 않음(가짜 접지로 공중 점프되는 것 방지)
			bool grounded = IsGrounded() && rb.velocity.y <= groundedVelYThreshold;

			// 2) 착지했으면 점프 락 해제
			//    (이륙 대기중(jumpPending)일 땐 아직 점프가 끝난 것이 아니므로 락을 풀지 않음)
			if (grounded && !jumpPending)
			{
				jumpConsumed = false;
			}

			if (grounded)
			{
				coyoteTimer = coyoteTime;
			}
			else coyoteTimer = Mathf.Max(0f, coyoteTimer - Time.fixedDeltaTime);


			// 점프: 착지 전까지 1번만
			if (jumpRequest && !jumpConsumed && !jumpPending && coyoteTimer > 0f &&
			    currentBaseState.fullPathHash != restState)
			{
				// 하강 중이면 하강 속도만 없애기(이륙 전이라도 낙하감 완화)
				Vector3 vel = rb.velocity;
				if (vel.y < 0f) vel.y = 0f;
				rb.velocity = vel;

				anim.SetBool("Jump", true); // 점프 애니 시작만
				jumpPending = true; // 이륙 이벤트를 기다림
				jumpConsumed = true; // 연타 방지 락

				// 소비
				jumpRequest = false;
				jumpRequestTimer = 0f;
				coyoteTimer = 0f;
			}

			// Animation Event가 들어오면, 다음 FixedUpdate에서 점프 힘 적용(물리 타이밍 안정)
			if (jumpTakeoffEvent)
			{
				jumpTakeoffEvent = false;

				if (jumpPending)
				{
					Vector3 vel = rb.velocity;
					if (vel.y < 0f) vel.y = 0f;
					rb.velocity = vel;

					rb.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
					jumpPending = false;
				}
			}

			// 아래는 Animator 각 State에서의 처리
			if (currentBaseState.fullPathHash == locoState)
			{
				// 커브로 콜라이더를 조정 중이면, 안전하게 리셋한다
				if (useCurves)
				{
					ResetCollider();
				}
			}
			else if (currentBaseState.fullPathHash == jumpState)
			{
				// State가 트랜지션 중이 아닐 때
				if (!anim.IsInTransition(0))
				{
					// 아래는 커브 보정을 하는 경우의 처리
					if (useCurves)
					{
						// 아래는 JUMP00 애니메이션에 포함된 커브 JumpHeight/GravityControl
						// JumpHeight: JUMP00에서의 점프 높이(0~1)
						// GravityControl: 1이면 점프 중(중력 무효), 0이면 중력 유효
						float jumpHeight = anim.GetFloat("JumpHeight");
						float gravityControl = anim.GetFloat("GravityControl");
						if (gravityControl > 0)
							rb.useGravity = false; // 점프 중 중력 영향을 끈다

						// 캐릭터 중심에서 아래로 레이캐스트
						Ray ray = new Ray(transform.position + Vector3.up, -Vector3.up);
						RaycastHit hitInfo = new RaycastHit();
						// 높이가 useCurvesHeight 이상일 때만, JUMP00 커브로 콜라이더 높이/중심을 조정한다
						if (Physics.Raycast(ray, out hitInfo))
						{
							if (hitInfo.distance > useCurvesHeight)
							{
								col.height = colliderOriginHeight - jumpHeight; // 조정된 콜라이더 높이
								col.center = new Vector3(0, colliderOriginCenter.y + jumpHeight, 0); // 조정된 콜라이더 중심
							}
							else
							{
								ResetCollider(); // 임계값보다 낮으면 초기값으로 되돌림(안전용)
							}
						}
					}

					anim.SetBool("Jump", false); // Jump bool 값을 리셋(루프 방지)
				}
			}
			else if (currentBaseState.fullPathHash == idleState)
			{
				if (Input.GetKeyDown(KeyCode.R))
				{
					anim.SetBool("Rest", true); // 스페이스 키를 누르면 Rest 상태가 됨
				}
			}
			else if (currentBaseState.fullPathHash == restState)
			{
				if (!anim.IsInTransition(0))
				{
					anim.SetBool("Rest", false); // State가 전환 중이 아니면 Rest bool 값을 리셋(루프 방지)
				}
			}
		}

        void Move()
		{
			float h = Input.GetAxisRaw("Horizontal");
			float v = Input.GetAxisRaw("Vertical");

			anim.SetFloat("Direction", h); // Animator에 설정된 "Direction" 파라미터에 h 전달
			anim.speed = animSpeed; // Animator 모션 재생 속도를 animSpeed로 설정
			currentBaseState = anim.GetCurrentAnimatorStateInfo(0); // 참조용 State 변수에 Base Layer(0)의 현재 State를 저장
			rb.useGravity = true; // 점프 중에는 중력을 끄므로, 그 외에는 중력 영향을 받게 한다

			Vector3 moveDir = Vector3.zero;
			if (PerspectiveState == MovementSpace.World)
			{
				moveDir = new Vector3(h, 0, v).normalized;
			}
			else if (PerspectiveState == MovementSpace.Camera)
			{
				moveDir = MainCameraTransform.transform.forward * v + MainCameraTransform.transform.right * h;
				moveDir.y = 0;
				moveDir.Normalize();
			}

			// 애니메이션 세팅, Speed는 0 이상으로 설정하여 뒤로 걷는 클립이 재생되지 않도록 설정
			float speedParam = Mathf.Clamp01(moveDir.magnitude);
			anim.SetFloat("Speed", speedParam);
			anim.SetFloat("Direction", 0f); // 회전은 0 고정

			if (moveDir.magnitude > 0.01f)
			{
				// 항상 가야하는 방향으로 직진
				rb.MovePosition(transform.position + moveDir * (moveSpeed * Time.fixedDeltaTime));

				// 몸을 이동 방향으로 틀기
				Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
				float t = Mathf.Clamp01(rotateSpeed * Time.fixedDeltaTime);
				rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, t));
			}
		}
        public void ExternalJump(float power)
        {
            // 바닥 판정/점프 상태 강제 리셋
            jumpPending = false;
            jumpConsumed = true;
            jumpRequest = false;
            coyoteTimer = 0f;

            anim.SetBool("Jump", true);
            jumpPending = true;

            Vector3 vel = rb.velocity;
            if (vel.y < 0f) vel.y = 0f;
            rb.velocity = vel;

            rb.AddForce(Vector3.up * power, ForceMode.VelocityChange);
        }

        public void OnJumpTakeoff()
		{
			jumpTakeoffEvent = true;
		}

		// 캐릭터 콜라이더 크기 리셋 함수
		void ResetCollider()
		{
			// 컴포넌트 Height/Center 초기값 복원
			col.height = colliderOriginHeight;
			col.center = colliderOriginCenter;
		}

		private bool IsGrounded()
		{
			var ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);
			return Physics.Raycast(ray, 0.2f, groundLayers);
		}
	}
}