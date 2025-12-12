using UnityEngine;

public class GhostSprite : MonoBehaviour
{
    private SpriteRenderer sr;
    private Material mat; // 쉐이더 재질 제어용
    private float fadeSpeed;
    private float currentAlpha;

    // AllIn1Shader의 투명도 프로퍼티 이름 (보통 _Alpha 혹은 _FadeAmount 등이나, 
    // AllIn1Shader는 기본적으로 SpriteRenderer의 Color Alpha를 따라가도록 설계되어 있기도 함.
    // 하지만 확실한 효과를 위해 재질의 Alpha를 제어하겠습니다.)
    
    public void Init(Sprite sprite, Vector3 pos, Quaternion rot, Vector3 scale, Color color, float speed)
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        
        // 중요: 공유 재질(SharedMaterial)을 쓰면 모든 잔상이 동시에 투명해지므로
        // 개별 인스턴스 재질(material)을 가져옵니다.
        if (mat == null) mat = sr.material; 

        transform.position = pos;
        transform.rotation = rot;
        transform.localScale = scale;

        sr.sprite = sprite;
        
        // 쉐이더 효과를 위해 색상은 쉐이더 세팅을 따라가되, Alpha값만 초기화
        currentAlpha = color.a; 
        
        // 쉐이더의 _Color나 _MainColor 프로퍼티를 변경 (쉐이더 종류에 따라 변수명 확인 필요)
        // AllIn1Shader는 보통 SpriteRenderer의 Color를 틴트(Tint)로 사용하므로 아래 코드가 유효합니다.
        sr.color = color; 

        fadeSpeed = speed;
        gameObject.SetActive(true);
    }

    void Update()
    {
        // 투명도 감소
        currentAlpha -= fadeSpeed * Time.deltaTime;

        // 1. SpriteRenderer 컬러 알파값 조절 (가장 기본)
        Color spriteColor = sr.color;
        spriteColor.a = currentAlpha;
        sr.color = spriteColor;

        // 2. 만약 쉐이더에 별도의 Fade 프로퍼티가 있다면 여기서 조절
        // mat.SetFloat("_Alpha", currentAlpha); 

        if (currentAlpha <= 0f)
        {
            gameObject.SetActive(false);
        }
    }
}