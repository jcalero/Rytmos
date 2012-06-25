using UnityEngine;

public class EnemyCircle : EnemyScript
{
    #region Fields
    private int colorIndex;
    #endregion

    #region Functions
    protected override void Start()
    {
        colorIndex = Random.Range(0, colors.Length);
        MainColor = colors[colorIndex];
        minSpeed = 0.4f;
        maxSpeed = 0.8f;
        health = 2;
        base.Start();
    }
    #endregion
}