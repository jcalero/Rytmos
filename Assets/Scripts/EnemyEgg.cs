using UnityEngine;

public class EnemyEgg : EnemyScript
{
    #region Fields
    private int colorIndex;
    #endregion

    #region Functions
    protected override void Start()
    {
        colorIndex = Random.Range(0, colors.Length);
        MainColor = colors[colorIndex];
        minSpeed = 3f;
        maxSpeed = 3.5f;
        health = 1;
        base.Start();
    }
    #endregion
}