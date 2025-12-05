using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class TimeControll : MonoBehaviour
{
    [Header("Settings")]
    public float recordTime = 5f;

    private bool isRewinding = false;
    private List<PointInTime> pointsInTime;
    private Rigidbody2D rb;
    private Animator anim; // 애니메이터 참조 추가
    private Transform current;
    private Vector3 currentRecordPos;
    private Vector3 currentRecordSca;
    private PlayerControll playerControll { get; set; } 

    void Start()
    {
        pointsInTime = new List<PointInTime>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>(); // 컴포넌트 가져오기
        playerControll = GetComponent<PlayerControll>();
        current = transform;
        currentRecordPos = new Vector3(current.position.x, current.position.y, current.position.z);
        
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
        
        if (playerControll != null) playerControll.enabled = false;
        // 선택 사항: 되감기 중에는 애니메이터 자체의 속도를 0으로 만들어
        // 불필요한 연산을 줄이고 완전한 수동 제어 상태로 만듭니다.
        if (anim != null) anim.speed = 0;
    }

    void StopRewind()
    {
        isRewinding = false;
        rb.isKinematic = false;
        
        // if(pointsInTime.Count > 0)
        // {
        //      rb.linearVelocity = pointsInTime[0].velocity;
        // }
        if (playerControll != null) playerControll.enabled = true;
        // 애니메이터 속도 복구
        if (anim != null) anim.speed = 1;
    }
    
    void Record()
    {
        if (anim == null) return;

        currentRecordPos = new Vector3(current.position.x, current.position.y, current.position.z);
        currentRecordSca = new Vector3(currentRecordPos.x, currentRecordPos.y, currentRecordPos.z);
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        pointsInTime.Insert(0, new PointInTime(currentRecordPos, transform.rotation, 
            currentRecordSca, stateInfo.shortNameHash, stateInfo.normalizedTime));

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

            transform.position = new Vector3(point.position.x, point.position.y, point.position.z);
            transform.rotation = point.rotation;
            
            if (point.scale.x < 0.5) current.localScale = new Vector3(-1, 1, 1);
            else current.localScale = new Vector3(1, 1, 1);

            // 핵심: 애니메이션 강제 설정
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
    public Vector3 scale;
    public Quaternion rotation;
    public float height;

     public int animStateHash;      // 애니메이션 상태의 고유 ID
     public float animNormalizedTime; // 애니메이션 재생 진행률 (0.0 ~ 1.0)
    public PointInTime(Vector3 _pos, Quaternion _rot, Vector3 _sca, int _animHash, float _animTime)
    {
        position = _pos;
        rotation = _rot;
        scale = _sca;
        animStateHash = _animHash;
        animNormalizedTime = _animTime;
    }
}

