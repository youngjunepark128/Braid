using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeControll : MonoBehaviour
{
    [Header("Settings")]
    public float recordTime = 5f;

    private bool isRewinding = false;
    private List<PointInTime> pointsInTime;
    private Rigidbody2D rb;
    private Animator anim; // 애니메이터 참조 추가

    void Start()
    {
        pointsInTime = new List<PointInTime>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>(); // 컴포넌트 가져오기
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift)) StartRewind();
        if (Input.GetKeyUp(KeyCode.LeftShift)) StopRewind();
    }

    void FixedUpdate()
    {
        if (isRewinding) Rewind();
        else Record();
    }

    void StartRewind()
    {
        isRewinding = true;
        rb.isKinematic = true;
        
        // 선택 사항: 되감기 중에는 애니메이터 자체의 속도를 0으로 만들어
        // 불필요한 연산을 줄이고 완전한 수동 제어 상태로 만듭니다.
        if (anim != null) anim.speed = 0;
    }

    void StopRewind()
    {
        isRewinding = false;
        rb.isKinematic = false;
        
        if(pointsInTime.Count > 0)
        {
             rb.linearVelocity = pointsInTime[0].velocity;
        }

        // 애니메이터 속도 복구
        if (anim != null) anim.speed = 1;
    }

    void Record()
    {
        if (anim == null) return;

        // 현재 애니메이터의 상태 정보(0번 레이어 기준)를 가져옵니다.
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        pointsInTime.Insert(0, new PointInTime(
            transform.position, 
            transform.rotation, 
            rb.linearVelocity,
            stateInfo.shortNameHash, // 현재 상태의 해시값 저장
            stateInfo.normalizedTime // 현재 진행률 저장
        ));

        if (pointsInTime.Count > Mathf.Round(recordTime * 50))
        {
            pointsInTime.RemoveAt(pointsInTime.Count - 1);
        }
    }

    void Rewind()
    {
        if (pointsInTime.Count > 0 && anim != null)
        {
            PointInTime point = pointsInTime[0];

            // 1. 위치/회전 복구
            transform.position = point.position;
            transform.rotation = point.rotation;

            // 2. 핵심: 애니메이션 강제 설정
            // Play(상태해시, 레이어인덱스, 정규화된시간)
            // 이 함수는 애니메이터에게 특정 상태의 특정 시점으로 즉시 점프하라고 명령합니다.
            anim.Play(point.animStateHash, 0, point.animNormalizedTime);

            pointsInTime.RemoveAt(0);
        }
        else
        {
            StopRewind();
        }
    }
}

//-------------------------------------------------------------------
public class PointInTime
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector2 velocity;

     public int animStateHash;      // 애니메이션 상태의 고유 ID
     public float animNormalizedTime; // 애니메이션 재생 진행률 (0.0 ~ 1.0)
    public PointInTime(Vector3 _pos, Quaternion _rot, Vector2 _vel, int _animHash, float _animTime)
    {
        position = _pos;
        rotation = _rot;
        velocity = _vel;
        animStateHash = _animHash;
        animNormalizedTime = _animTime;
    }
}

