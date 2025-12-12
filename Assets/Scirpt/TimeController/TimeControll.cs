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
    private SpriteRenderer sr;
    
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
        sr = GetComponentInChildren<SpriteRenderer>();
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

            transform.position = point.position;
            transform.rotation = point.rotation;
            transform.localScale = point.scale;
            anim.SetBool(IS_ATTACKED, point.isAttacked);
            col.enabled = point.collState;
            
            // 잔상 거리나 투명도 조절
            Material mat = sr.material;
            mat.EnableKeyword("GHOST_ON");
            mat.SetFloat("_GhostTransparency", 0.5f);
            mat.SetFloat("_GhostDist", 0.2f); // 잔상 거리
            
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

