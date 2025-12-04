using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class Player : MonoBehaviour
{
    public static Player LocalPlayer;
    
    private PlayerControll PlayerController { get; set; }
    
    private void Awake()
    {
        if (LocalPlayer == null) LocalPlayer = this;
        else Destroy(gameObject);
        
        
    }

   
}
