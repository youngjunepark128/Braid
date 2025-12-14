using UnityEngine;
using System.Collections.Generic;

public class GhostRunner : MonoBehaviour
{
    [Header("Settings")]
    public TimeControll targetPlayer; // 따라할 원본 플레이어
    public float delaySeconds = 3.0f;   // 몇 초 전 과거를 따라할지
    
    [Header("Visuals")]
    public Color ghostColor = new Color(1, 1, 1, 0.5f); // 반투명하게

    private SpriteRenderer sr;
    private Animator anim;
    private int delayFrameCount; // 지연 시간을 프레임 수로 환산
    private List<PointInTime> history = new List<PointInTime>();

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        // 고스트 느낌을 위해 반투명 설정
        if (sr != null) sr.color = ghostColor;

        // 충돌 방지: 고스트는 물리적인 힘을 가하면 안 되므로 Collider를 끄거나 Trigger로 설정
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true; // 혹은 col.enabled = false;

        // 물리 연산 끄기 (Kinematic)
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.bodyType = RigidbodyType2D.Kinematic;
    }

    void FixedUpdate()
    {
        if (targetPlayer == null) return;
        
        // 1. 되감기 중일 때는 고스트도 멈추거나 사라져야 함
        if (TimeManager.isRewinding)
        {
            // 되감기 중엔 잠깐 숨기거나 멈춤
            // gameObject.SetActive(false); // 선택 사항
            return;
        }

        history = targetPlayer.GetHistory();

        if (Input.GetKeyDown(KeyCode.F)) GhostRun();
        // 3. 지연 시간(초)을 프레임(리스트 인덱스)으로 변환
        // FixedUpdate는 보통 0.02초(50Hz)마다 실행되므로, 1초 전은 인덱스 50번임.
        delayFrameCount = Mathf.RoundToInt(delaySeconds / Time.fixedDeltaTime);

        // 4. 리스트에 충분한 데이터가 쌓였는지 확인
        // (게임 시작 직후에는 5초 전 데이터가 없으므로)
        
    }

    public void GhostRun()
    {
        if (history.Count > delayFrameCount)
        {
            if (!sr.enabled) sr.enabled = true; // 데이터 있으면 모습 보이기

            // 과거 시점의 데이터 가져오기
            PointInTime point = history[delayFrameCount];

            // 5. 데이터 적용 (따라하기)
            transform.position = point.position;
            transform.rotation = point.rotation;
            transform.localScale = point.scale;

            if (anim != null)
            {
                anim.Play(point.animStateHash, 0, point.animNormalizedTime);
                
                // PointInTime에 저장해뒀던 추가 정보들 적용
                // (이전에 추가한 isAttacked 등을 여기서도 똑같이 적용 가능)
                // anim.SetBool("IsAttacked", point.isAttacked); 
            }
            
            // 스프라이트 렌더러가 있다면 이미지도 맞춰주기 (필수는 아님)
            // if (sr != null && targetPlayer.GetComponent<SpriteRenderer>() != null)
            //     sr.sprite = targetPlayer.GetComponent<SpriteRenderer>().sprite;
        }
        else
        {
            // 아직 데이터가 부족하면 숨겨두기
            sr.enabled = false;
        }
    }
}