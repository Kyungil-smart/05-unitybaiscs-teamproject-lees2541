//
// Mecanim 애니메이션 데이터가 원점에서 이동하지 않을 때 사용하는 Rigidbody 포함 컨트롤러
// 샘플
// 2014/03/13 N.Kobyasahi
//
using UnityEngine;
using System.Collections;

namespace UnityChan
{
    // 필요한 컴포넌트 나열
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Rigidbody))]

    public class PlayerController : MonoBehaviour
    {

        public float animSpeed = 1.5f;              // 애니메이션 재생 속도 설정
        public float lookSmoother = 3.0f;           // a smoothing setting for camera motion
        public bool useCurves = true;               // Mecanim에서 커브 보정을 사용할지 설정
                                                    // 이 스위치가 꺼져 있으면 커브를 사용하지 않음
        public float useCurvesHeight = 0.5f;        // 커브 보정 유효 높이(바닥을 뚫기 쉬우면 값을 크게)

        // 아래는 캐릭터 컨트롤러용 파라미터
        // 전진 속도
        public float forwardSpeed = 7.0f;
        // 후진 속도
        public float backwardSpeed = 2.0f;
        // 측면 속도
        public float sidewaySpeed = 5.0f;
        // 회전 속도
        public float rotateSpeed = 2.0f;
        // 점프 힘
        public float jumpPower = 3.0f;
        // 캐릭터 컨트롤러(캡슐 콜라이더) 참조
        private CapsuleCollider col;
        private Rigidbody rb;
        // 캐릭터 컨트롤러(캡슐 콜라이더) 이동량
        private Vector3 velocity;
        // --- Jump input buffer + grounded (안 씹히는 점프) ---
        private bool jumpRequest = false;
        private bool jumpConsumed = false;
        private const float groundedVelYThreshold = 0.05f; // 바닥에 닿은 것으로 간주하는 Y 속도 임계값
        private float jumpRequestTimer = 0f;
        private const float jumpBufferTime = 0.15f;   // 점프 입력 버퍼(0.1~0.2 추천)

        private float coyoteTimer = 0f;
        private const float coyoteTime = 0.10f;       // 바닥에서 살짝 떨어져도 점프 허용(선택이지만 체감 좋아짐)
        private const float groundEpsilon = 0.06f;    // 바닥 판정 여유

        public enum STATE { FirstP, ThirdP, SideView }
        public STATE PerspectiveState = STATE.FirstP;

        // --- Ground check by LayerMask (바닥 레이어만 감지) ---
        [Header("Ground Check")]
        [SerializeField] private LayerMask groundLayers; // Inspector에서 Ground 레이어(들)만 선택
        [SerializeField] private float groundRayExtraUp = 0.10f;   // 레이 시작점 여유(머리 위)
        [SerializeField] private float groundRayExtraDown = 0.30f; // 레이 길이 여유


        private bool jumpPending = false;       // 점프 애니 시작 ~ 이륙 이벤트까지 대기
        private bool jumpTakeoffEvent = false;  // Animation Event가 켜는 플래그(물리 적용은 FixedUpdate에서)






        // CapsuleCollider에 설정된 콜라이더 Height/Center 초기값을 저장하는 변수
        private float orgColHight;
        private Vector3 orgVectColCenter;

        private Animator anim;                          // 캐릭터에 붙는 Animator 참조
        private AnimatorStateInfo currentBaseState;     // base layer에서 사용하는 Animator의 현재 상태 참조
        private GameObject cameraObject;    // 메인 카메라 참조

        // Animator 각 State 참조
        static int locoState = Animator.StringToHash("Base Layer.Locomotion");
        static int jumpState = Animator.StringToHash("Base Layer.Jump");
        static int idleState = Animator.StringToHash("Base Layer.Idle");
        static int restState = Animator.StringToHash("Base Layer.Rest");

        // PlayerInteractor 참조
        private PlayerInteractor interactor;

        // 초기화
        void Start()
        {
            // Animator 컴포넌트 가져오기
            anim = GetComponent<Animator>();
            // CapsuleCollider 컴포넌트 가져오기(캡슐형 콜리전)
            col = GetComponent<CapsuleCollider>();
            rb = GetComponent<Rigidbody>();
            // 메인 카메라 가져오기
            cameraObject = GameObject.FindWithTag("MainCamera");
            // CapsuleCollider의 Height/Center 초기값 저장
            orgColHight = col.height;
            orgVectColCenter = col.center;
            interactor = GetComponent<PlayerInteractor>();
        }

        void Update()
        {
            // 점프 "눌린 순간"은 Update에서 잡아두기 (FixedUpdate에서 놓치지 않게)
            if (Input.GetButtonDown("Jump"))
            {
                jumpRequest = true;
                jumpRequestTimer = jumpBufferTime;
            }

            // 상호작용 입력
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (interactor != null && interactor.CanInteract)
                    interactor.TryInteract();
            }

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
            if (Input.GetKey(KeyCode.Q)) transform.Rotate(0, -1 * rotateSpeed, 0);
            if (Input.GetKey(KeyCode.E)) transform.Rotate(0, 1 * rotateSpeed, 0);
        }

        // 아래는 메인 처리. Rigidbody와 함께 쓰므로 FixedUpdate에서 처리한다.
        void FixedUpdate()
        {
            float h = Input.GetAxis("Horizontal");              // 입력의 수평 축을 h로 정의
            float v = Input.GetAxis("Vertical");                // 입력의 수직 축을 v로 정의
            //anim.SetFloat("Speed", v);                          // Animator에 설정된 "Speed" 파라미터에 v 전달
            //float speedParam = (Mathf.Abs(v) > 0.1f) ? v : Mathf.Abs(h);
            //anim.SetFloat("Speed", speedParam);
            anim.SetFloat("Direction", h);                      // Animator에 설정된 "Direction" 파라미터에 h 전달
            anim.speed = animSpeed;                             // Animator 모션 재생 속도를 animSpeed로 설정
            currentBaseState = anim.GetCurrentAnimatorStateInfo(0); // 참조용 State 변수에 Base Layer(0)의 현재 State를 저장
            rb.useGravity = true;                               // 점프 중에는 중력을 끄므로, 그 외에는 중력 영향을 받게 한다


            // --- 이동 벡터 계산 (ThirdP는 카메라 기준 월드 직진) ---

            if (PerspectiveState == STATE.ThirdP)
            {
                // (1) ThirdP: 애니 Speed는 항상 양수로 -> S 눌러도 뒷걸음 애니 안 나옴
                float speedParam = Mathf.Max(Mathf.Abs(v), Mathf.Abs(h));
                anim.SetFloat("Speed", speedParam);
                anim.SetFloat("Direction", 0f); // 회전은 우리가 직접 하니까 0 고정

                // (2) 월드 좌표 기준 이동 방향 (카메라 무관)
                Vector3 moveDir = Vector3.right * h + Vector3.forward * v;
                moveDir.y = 0f; // 안전빵


                if (moveDir.sqrMagnitude > 0.0001f)
                {
                    moveDir.Normalize();

                    // (3) 이동은 항상 forwardSpeed로 "직진"
                    velocity = moveDir * forwardSpeed;

                    // (4) 몸을 이동 방향으로 틀기 (S면 180도 돌아서 전진)
                    Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
                    float t = Mathf.Clamp01(rotateSpeed * Time.fixedDeltaTime);
                    rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, t));
                }
                else
                {
                    velocity = Vector3.zero;
                }
            }
            else
            {
                // ===== 기존 FirstP 로직 유지 =====
                float speedParam = (Mathf.Abs(v) > 0.1f) ? v : Mathf.Abs(h);
                anim.SetFloat("Speed", speedParam);
                anim.SetFloat("Direction", h);

                float z = 0f;
                if (v > 0.1f) z = v * forwardSpeed;
                else if (v < -0.1f) z = v * backwardSpeed;

                float x = h * sidewaySpeed;

                velocity = new Vector3(x, 0f, z);
                velocity = transform.TransformDirection(velocity);
            }



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
            if (jumpRequest && !jumpConsumed && !jumpPending && coyoteTimer > 0f && currentBaseState.fullPathHash != restState)
            {
                // 하강 중이면 하강 속도만 없애기(이륙 전이라도 낙하감 완화)
                Vector3 vel = rb.velocity;
                if (vel.y < 0f) vel.y = 0f;
                rb.velocity = vel;

                anim.SetBool("Jump", true);  // 점프 애니 시작만
                jumpPending = true;          // 이륙 이벤트를 기다림
                jumpConsumed = true;         // 연타 방지 락

                // 소비
                jumpRequest = false;
                jumpRequestTimer = 0f;
                coyoteTimer = 0f;
            }

            // Transform 직접 이동 금지! Rigidbody 속도로만 이동 통일
            Vector3 rbVel = rb.velocity;
            rbVel.x = velocity.x;
            rbVel.z = velocity.z;
            rb.velocity = rbVel;





            //transform.Rotate(0, h * rotateSpeed, 0);                // 좌/우 키 입력으로 캐릭터를 Y축 회전

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
            // Locomotion 처리 중
            // 현재 base layer가 locoState일 때
            if (currentBaseState.fullPathHash == locoState)
            {
                // 커브로 콜라이더를 조정 중이면, 안전하게 리셋한다
                if (useCurves)
                {
                    resetCollider();
                }
            }
            // 점프 중 처리
            // 현재 base layer가 jumpState일 때
            else if (currentBaseState.fullPathHash == jumpState)
            {
                //cameraObject.SendMessage("setCameraPositionJumpView");  // 점프용 카메라로 변경
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
                            rb.useGravity = false;                  // 점프 중 중력 영향을 끈다

                        // 캐릭터 중심에서 아래로 레이캐스트
                        Ray ray = new Ray(transform.position + Vector3.up, -Vector3.up);
                        RaycastHit hitInfo = new RaycastHit();
                        // 높이가 useCurvesHeight 이상일 때만, JUMP00 커브로 콜라이더 높이/중심을 조정한다
                        if (Physics.Raycast(ray, out hitInfo))
                        {
                            if (hitInfo.distance > useCurvesHeight)
                            {
                                col.height = orgColHight - jumpHeight;          // 조정된 콜라이더 높이
                                col.center = new Vector3(0, orgVectColCenter.y + jumpHeight, 0);    // 조정된 콜라이더 중심
                            }
                            else
                            {
                                resetCollider();    // 임계값보다 낮으면 초기값으로 되돌림(안전용)
                            }
                        }
                    }
                    anim.SetBool("Jump", false);        // Jump bool 값을 리셋(루프 방지)
                }
            }
            // Idle 중 처리
            // 현재 base layer가 idleState일 때
            else if (currentBaseState.fullPathHash == idleState)
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    anim.SetBool("Rest", true);     // 스페이스 키를 누르면 Rest 상태가 됨
                }
            }
            // Rest 중 처리
            // 현재 base layer가 restState일 때
            else if (currentBaseState.fullPathHash == restState)
            {
                if (!anim.IsInTransition(0))
                {
                    anim.SetBool("Rest", false);    // State가 전환 중이 아니면 Rest bool 값을 리셋(루프 방지)
                }
            }
        }
        public void OnJumpTakeoff()
        {
            jumpTakeoffEvent = true;
        }

        // 캐릭터 콜라이더 크기 리셋 함수
        void resetCollider()
        {
            // 컴포넌트 Height/Center 초기값 복원
            col.height = orgColHight;
            col.center = orgVectColCenter;
        }
        private bool IsGrounded()
        {
            // groundLayers로 "바닥 레이어만" 대상으로 레이를 쏘고,
            // 혹시라도 자신 콜라이더가 잡히면 무시하고 다음 히트를 사용(RaycastAll).
            float extraUp = groundRayExtraUp;
            Vector3 origin = col.bounds.center + Vector3.up * (col.bounds.extents.y + extraUp);

            float expectedToGround = col.bounds.size.y + extraUp;   // 바닥에 붙어있을 때 예상 거리
            float maxDist = expectedToGround + groundRayExtraDown;  // 여유

            var hits = Physics.RaycastAll(
                origin, Vector3.down, maxDist,
                groundLayers, QueryTriggerInteraction.Ignore
            );
            if (hits == null || hits.Length == 0) return false;

            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            foreach (var hit in hits)
            {
                if (hit.collider == col) continue; // 혹시라도 자신 콜라이더면 스킵
                return hit.distance <= expectedToGround + groundEpsilon;
            }

            return false;
        }

    }
}
