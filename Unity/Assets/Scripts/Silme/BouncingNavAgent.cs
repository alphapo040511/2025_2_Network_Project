using UnityEngine;
using UnityEngine.AI;
public enum SlimeState
{
    Free,       // 자유롭게 돌아다니는
    Grabbed,    // 잡힌 상태
    Captured,   // 포획존 안에 있음
    InBasket    // 포획 바구니 안에 있음
}

public class BouncingNavAgent : MonoBehaviour
{
    [Header("슬라임 설정")]
    public float bounceForce = 8f;
    public float jumpInterval = 2f;
    public float wanderRadius = 5f;
    public float moveSpeed = 2f;
    public PhysicMaterial bouncyMaterial;

    [Header("상태")]
    public SlimeState currentState = SlimeState.Free;

    private Rigidbody rb;
    private Vector3 spawnPoint;
    private Vector3 targetPosition;
    private Transform captureZone;
    private SlimeTweenEffect tweenEffect;
    private bool isGroundedLastFrame = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // SlimeTweenEffect 컴포넌트 가져오기 (없으면 추가)
        tweenEffect = GetComponent<SlimeTweenEffect>();
        if (tweenEffect == null)
        {
            tweenEffect = gameObject.AddComponent<SlimeTweenEffect>();
        }

        // PhysicMaterial 적용
        if (bouncyMaterial != null)
            GetComponent<Collider>().material = bouncyMaterial;
    }

    void Start()
    {
        jumpInterval = Random.Range(1.5f, 3f);

        spawnPoint = transform.position;
        targetPosition = GetRandomPosition();

        // 랜덤하게 점프 시작
        InvokeRepeating("Jump", 1f, jumpInterval);
        InvokeRepeating("ChooseNewTarget", 3f, 3f);

        // Idle 상태에서 맥박 효과
        if (currentState == SlimeState.Free)
        {
            tweenEffect.PlayIdlePulse();
        }
    }

    void Update()
    {

        // 움직일 수 있는 상태에서만 이동
        if (currentState == SlimeState.Free || currentState == SlimeState.Captured)
        {
            Vector3 direction = (targetPosition - transform.position);
            direction.y = 0;

            if (direction.magnitude > 0.5f)
            {
                float speed = currentState == SlimeState.Captured ? moveSpeed * 0.5f : moveSpeed;
                rb.AddForce(direction.normalized * speed, ForceMode.Force);
            }
        }
    }

    void Jump()
    {
        // 상태 체크를 맨 처음에!
        if (currentState == SlimeState.Grabbed || currentState == SlimeState.InBasket)
        {
            rb.velocity = Vector3.zero; // 혹시 모를 움직임도 정지
            return;
        }

        if (!IsGrounded())
            return;

        // 점프 전 슬라임 효과
        tweenEffect.PlayJumpEffect();

        // VR 간단 수정: Y 속도 초기화 + 점프력 제한
        Vector3 velocity = rb.velocity;
        velocity.y = 0; // 기존 Y 속도 제거 (누적 방지)
        rb.velocity = velocity;

        Vector3 jumpDirection = Vector3.up * 2f;
        Vector3 moveDirection = Vector3.zero;

        if (currentState == SlimeState.Free || currentState == SlimeState.Captured)
        {
            moveDirection = (targetPosition - transform.position).normalized * 0.3f;
            moveDirection.y = 0;
        }

        // VR에서 점프력 제한 (최대 속도 제한)
        float finalForce = Mathf.Min(bounceForce, 6f); // 6 이하로 제한
        rb.AddForce((jumpDirection + moveDirection) * finalForce, ForceMode.Impulse);
    }

    void OnLanded()
    {
        // 착지 시 슬라임 효과
        tweenEffect.PlayLandingEffect();
    }

    void ChooseNewTarget()
    {
        if (currentState == SlimeState.Free)
        {
            targetPosition = GetRandomPosition();
        }
        else if (currentState == SlimeState.Captured && captureZone != null)
        {
            float captureRadius = 2f;
            Vector2 randomCircle = Random.insideUnitCircle * captureRadius;
            targetPosition = captureZone.position + new Vector3(randomCircle.x, 0, randomCircle.y);
        }
    }

    Vector3 GetRandomPosition()
    {
        Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
        Vector3 newTarget = spawnPoint + new Vector3(randomCircle.x, 0, randomCircle.y);
        return newTarget;
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }
    private void StartIdlePulse()
    {
        if (currentState == SlimeState.Free)
        {
            tweenEffect.PlayIdlePulse();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(spawnPoint, wanderRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(targetPosition, 0.3f);

        if (captureZone != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(captureZone.position, 2f);
        }
    }

    void OnDestroy()
    {
        // 트윈 정리
        if (tweenEffect != null)
        {
            tweenEffect.StopAllTweens();
        }
    }
}
