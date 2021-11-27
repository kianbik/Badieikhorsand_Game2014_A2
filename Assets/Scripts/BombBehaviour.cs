using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBehaviour : MonoBehaviour
{
    float timeLeft = 3.0f;
    private Animator animatorController;
    void Start()
    {
       
        animatorController = GetComponent<Animator>();
       
    }
    void Update()
    {
        timeLeft -= Time.deltaTime;
        animatorController.SetFloat("TimeLeft", timeLeft);
      
    }
}
