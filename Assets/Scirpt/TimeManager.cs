using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static bool isRewinding = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isRewinding = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isRewinding = false;
        }
    }
}