using System.Collections.Generic;
using UnityEngine;

public class AfterimagePool : MonoBehaviour
{
    public static AfterimagePool Instance { get; private set; }

    [Header("Settings")]
    public GameObject ghostPrefab; // 1단계에서 만든 프리팹 연결
    public int poolSize = 50; // 미리 만들어둘 개수

    private Queue<GameObject> poolQueue = new Queue<GameObject>();

    void Awake()
    {
        // 싱글톤 설정
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        InitializePool();
    }

    void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(ghostPrefab, transform);
            obj.SetActive(false); // 비활성화 상태로 대기
            poolQueue.Enqueue(obj);
        }
    }

    // 외부에서 잔상을 요청할 때 호출하는 함수
    public GameObject GetFromPool()
    {
        // 큐에 사용 가능한 오브젝트가 있으면 꺼내줌
        if (poolQueue.Count > 0)
        {
            GameObject obj = poolQueue.Dequeue();
            // 나중에 다시 큐에 넣기 위해, 비활성화될 때 동작할 로직이 필요하지만
            // GhostSprite가 스스로 비활성화되므로, 여기서는 꺼낼 때 큐 맨 뒤로 다시 넣는 방식을 씁니다.
            poolQueue.Enqueue(obj); 
            return obj;
        }
        else
        {
            // 혹시 부족하면 새로 만들어서 줌 (옵션)
            GameObject obj = Instantiate(ghostPrefab, transform);
            poolQueue.Enqueue(obj);
            return obj;
        }
    }
}