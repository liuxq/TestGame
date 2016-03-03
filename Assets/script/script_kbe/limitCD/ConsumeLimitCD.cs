using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ConsumeLimitCD
{
    private ConsumeLimitCD() { }
    public static readonly ConsumeLimitCD instance = new ConsumeLimitCD();
    public float restTime;
    public float totalTime;
    public void Update(float deltaTime)
    {
        if(restTime > 0)
            restTime -= deltaTime;
    }
    public bool isWaiting()
    {
        return restTime > 0;
    }
    public void Start(float time)
    {
        totalTime = time;
        restTime = totalTime;
    }
}