using UnityEngine;

public class EnemyCircle : EnemyScript
{
    #region Fields
    private Color[] colors = new Color[] { Color.red, Color.green, Color.cyan, Color.blue, Color.yellow, new Color(.5f, 0f, .5f, 1f) };
    private int colorIndex;
    #endregion

    #region Functions
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        colorIndex = Random.Range(0, colors.Length);
        MainColor = colors[colorIndex];
        base.Start();
    }
    #endregion
}