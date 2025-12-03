using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

// 스탯, 전투 관련 기능을 넣어놓을겁니다. 그리고 어떤 개체에서든 Player를 찾을 수 있도록 만듭니다.
public class Player : MonoBehaviour
{
    public static Player LocalPlayer;
    
    private PlayerControll PlayerController { get; set; }
    // private Dictionary<string, AttackInfo> AttackInfoDic { get; set; } = new();
    //
    // [field:SerializeField] public Transform HeadUpPivot { get; private set; }
    //
    // //아래 필드는 특수(원래 이렇게 안씀)함. 원래는 테이블에 포함된 녀석.
    // //우리는 커스텀 인스펙터 기능을 테스트해보기 위해 일부러
    // //배열로 선언하여 사용함. 좋은 방법은 아님
    // public AttackInfo[] AttackInfos;
    //
    private void Awake()
    {
        if (LocalPlayer == null) LocalPlayer = this;
        else Destroy(gameObject);
        
        
    }

   
}
