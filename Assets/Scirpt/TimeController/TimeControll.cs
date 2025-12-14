using System;
using System.Collections;
using System.Collections.Generic;
using AllIn1SpriteShader;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class TimeControll : MonoBehaviour
{
    [Header("Settings")]
    public float recordTime = 5f;

    [Header("RewindingDisableScripts")] 
    public MonoBehaviour[] ScriptsDisabled;
    
    
    [Header("Components")]
    private List<PointInTime> pointsInTime;
    private CapsuleCollider2D col;
    private Rigidbody2D rb;
    private Animator anim; // 애니메이터 참조 추가
    private Transform current;
    private AllIn1Shader allIn1;
    
    [Header("Afterimage Effect")]
    public bool enableAfterimage = true; // 잔상 효과 켤지 여부
    public Color afterimageColor = new Color(0f, 1f, 1f, 0.7f); // 산데비스탄 느낌의 형광 시안색
    public float afterimageFadeSpeed = 5f; // 사라지는 속도
    public float afterimageSpawnRate = 0.05f; // 잔상 생성 간격 (초)

    private float afterimageTimer;
    private SpriteRenderer mySr; // 내 스프라이트 렌더
    
    private Vector3 currentRecordPos;
    private Vector3 currentRecordSca;
    private PlayerControll playerControll { get; set; } 
    private static readonly int IS_ATTACKED = Animator.StringToHash("IsAttacked");
    
    void Start()
    {
        pointsInTime = new List<PointInTime>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>(); // 컴포넌트 가져오기
        current = transform;
        currentRecordPos = new Vector3(current.position.x, current.position.y, current.position.z);
        col = GetComponent<CapsuleCollider2D>();
        allIn1 = GetComponentInChildren<AllIn1Shader>();
        mySr = GetComponentInChildren<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        if (TimeManager.isRewinding) Rewind();
        else Record();
    }
    
    void Record()
    {
        if (anim == null) return;
        if (ScriptsDisabled != null)
        {
            foreach (var script in ScriptsDisabled)
                if (script != null) script.enabled = true;
        }
        currentRecordPos = new Vector3(current.position.x, current.position.y, current.position.z);
        currentRecordSca = current.localScale;
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        bool IsAttackedState = anim.GetBool(IS_ATTACKED);
        bool CollState = col ? col.enabled : true;

        pointsInTime.Insert(0, new PointInTime(currentRecordPos, transform.rotation, 
            currentRecordSca, stateInfo.shortNameHash, stateInfo.normalizedTime, IsAttackedState, CollState));

        if (pointsInTime.Count > Mathf.Round(recordTime * 50))
        {
            pointsInTime.RemoveAt(pointsInTime.Count - 1);
        }
    }

    void Rewind()
    {
        if (ScriptsDisabled != null)
        {
            foreach (var script in ScriptsDisabled)
                if (script != null) script.enabled = false;
        }
        if (pointsInTime.Count > 0 && anim != null)
        {
            PointInTime point = pointsInTime[0];

           
            // 2. 현재 나의 모습으로 초기화 (위치 이동 전에 찍어야 함)
            afterimageTimer -= Time.unscaledDeltaTime; // 되감기 중 TimeScale 영향 받을 수 있으므로 unscaled 추천
            if (afterimageTimer <= 0f)
            {
                // 1. 풀에서 가져옴
                GameObject ghostObj = AfterimagePool.Instance.GetFromPool();
                GhostSprite ghost = ghostObj.GetComponent<GhostSprite>();
                    
                // 2. 현재 내 모습으로 세팅 (Material은 프리팹에 이미 적용되어 있음)
                // 현재 위치(transform.position)에 생성되므로, 나는 떠나도 얘는 여기 남습니다.
                ghost.Init(mySr.sprite, transform.position, transform.rotation, transform.localScale, afterimageFadeSpeed);
                    
                // 3. 타이머 리셋
                afterimageTimer = afterimageSpawnRate;
            }
            
            transform.position = point.position;
            transform.rotation = point.rotation;
            transform.localScale = point.scale;
            anim.SetBool(IS_ATTACKED, point.isAttacked);
            col.enabled = point.collState;

            // 핵심: 애니메이션 강제 설정
            // Play(상태해시, 레이어인덱스, 정규화된시간)
            // 이 함수는 애니메이터에게 특정 상태의 특정 시점으로 즉시 점프하라고 명령합니다.
            anim.Play(point.animStateHash, 0, point.animNormalizedTime);
            pointsInTime.RemoveAt(0);
        }
        else
        {
        }
    }
    public List<PointInTime> GetHistory()
    {
        return pointsInTime;
    }
}

//-------------------------------------------------------------------
public class PointInTime
{
    public Vector3 position;
    public Vector3 scale;
    public Quaternion rotation;
    public float height;
    public Rigidbody2D rb;

    public bool collState;
    public bool isAttacked;
    public int animStateHash;      // 애니메이션 상태의 고유 ID
    public float animNormalizedTime; // 애니메이션 재생 진행률 (0.0 ~ 1.0)
    public PointInTime(Vector3 _pos, Quaternion _rot, Vector3 _sca, int _animHash, float _animTime, bool _isAttacked, bool _collState)
    {
        position = _pos;
        rotation = _rot;
        scale = _sca;
        animStateHash = _animHash;
        animNormalizedTime = _animTime;
        isAttacked = _isAttacked;
        collState = _collState;
    }
}

