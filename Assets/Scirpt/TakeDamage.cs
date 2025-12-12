using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamage : MonoBehaviour
{
   [Header("Settings")] 
   public float popUpForce = 5.0f;
   public float gravity=6.0f;
   public bool IsDead { get; private set; } = false;

   private Rigidbody2D rigidBody;
   private CapsuleCollider2D col;
   private Animator animator;
   
   private static readonly int IS_ATTACKED = Animator.StringToHash("IsAttacked");

   private void Awake()
   {
      rigidBody = GetComponent<Rigidbody2D>();
      col = GetComponent<CapsuleCollider2D>();
      animator = GetComponentInChildren<Animator>();
   }


   public void Die()
   {
      animator.SetBool(IS_ATTACKED, true);
      col.enabled = false;

      // 2. 시각적 반전 (뒤집기)
      Vector3 scaler = transform.localScale;
      scaler.y = -Mathf.Abs(scaler.y);
      transform.localScale = scaler;

      // 3. 낙하 코루틴 시작
      StartCoroutine(DeadFallSequence());
   }
   
   IEnumerator DeadFallSequence()
   {
      float timer = 0f;
      float duration = 3.0f; // 3초 동안 떨어짐 (이후엔 화면 밖일 테니)
        
      // 초기 속도 설정 (위로 살짝 튐)
      float verticalVelocity = popUpForce;

      while (timer < duration)
      {
         // 되감기 중이라면 코루틴 강제 중단 (시스템에 맡김)
         if (TimeManager.isRewinding) yield break;

         // 물리 공식: v = v0 + at
         verticalVelocity -= gravity * Time.deltaTime;
            
         // 위치 이동: y = y + vt
         transform.position += Vector3.up * verticalVelocity * Time.deltaTime;

         timer += Time.deltaTime;
         yield return null; // 다음 프레임까지 대기
      }
   }
   
}
