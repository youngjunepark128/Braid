using UnityEngine;

public class GhostSprite : MonoBehaviour
{
    private SpriteRenderer mySr;
    private float fadeSpeed;

    // 초기화 함수
    public void Init(Sprite sprite, Vector3 pos, Quaternion rot, Vector3 scale, float speed)
    {
        if (mySr == null) mySr = GetComponent<SpriteRenderer>();

        transform.position = pos;
        transform.rotation = rot;
        transform.localScale = scale;

        mySr.sprite = sprite;
        
        // [핵심] 시작할 때는 불투명(1)하게 시작
        // 색상은 쉐이더(Material)에서 설정한 Solid Color를 따라가므로 여기선 흰색(기본)에 알파만 1로 줍니다.
        mySr.color = new Color(1, 1, 1, 1); 

        fadeSpeed = speed;
        gameObject.SetActive(true);
    }

    void Update()
    {
        // 서서히 투명해짐
        Color color = mySr.color;
        color.a -= fadeSpeed * Time.deltaTime;
        mySr.color = color;

        // 완전히 투명해지면 풀로 반납
        if (color.a <= 0f)
        {
            gameObject.SetActive(false);
        }
    }
}