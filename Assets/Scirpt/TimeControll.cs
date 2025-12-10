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

    [Header("RewindingDisableScripts")] 
    public MonoBehaviour[] ScriptsDisabled;
    
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
        current = transform;
        currentRecordPos = new Vector3(current.position.x, current.position.y, current.position.z);
        
    }

    // void Update()
    // {
    //     // if (Input.GetKeyDown(KeyCode.LeftShift)) StartRewind();
    //     // if (Input.GetKeyUp(KeyCode.LeftShift)) StopRewind();
    // }

    void FixedUpdate()
    {
        if (TimeManager.isRewinding) Rewind();
        else Record();
    }
    
    // void StartRewind()
    // {
    //     TimeManager.isRewinding = true;
    //     
    //     if (playerControll != null) playerControll.enabled = false;
    //     if (anim != null) anim.speed = 0;
    // }
    //
    // void StopRewind()
    // {
    //     TimeManager.isRewinding = false;
    //     
    //     if (playerControll != null) playerControll.enabled = true;
    //     if (anim != null) anim.speed = 1;
    // }
    
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

        pointsInTime.Insert(0, new PointInTime(currentRecordPos, transform.rotation, 
            currentRecordSca, stateInfo.shortNameHash, stateInfo.normalizedTime));

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

