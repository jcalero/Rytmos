using UnityEngine;
using System.Collections;
/// <summary>
/// PulseSender.cs
/// 
/// Handles the pulse animation, color and events.
/// </summary>
public class PulseSender : MonoBehaviour {
    public float Radius;                            // The radius of he pulse
    public float CurrentHealth;                     // Current health of the pulse
    public float MaxHealth = 3;                     // Max health of the pulse

    private int segments = 50;                      // The nr of segments the pulse has. Fewer means less "smooth".
    private LineRenderer line;                      // The line renderer that creates the pulse
    private SphereCollider sphereColl;              // The collider attatched to the pulse
    private bool held;                              // Flag for whether the player is keeping his pulse "active"
    private Color finalColor = Color.clear;         // The final color of the pulse once the player releases his finger
    private float pulseMax;                         // Maximum range of the pulse
    private float timer = 0;                        // Timer for energy cost when retracting the pulse
    private float pulseBackEnergyRate = 0.1f;       // The rate at which the pulse retraction reduces energy. Lower = higher rate.

    void Start() {
        held = true;
        Radius = .4f;
        line = gameObject.GetComponent<LineRenderer>();
        line.SetVertexCount(segments + 1);
        line.useWorldSpace = false;
        line.material.color = finalColor;
        CurrentHealth = MaxHealth;
        float lineWidth = CurrentHealth / 10;
        line.SetWidth(lineWidth, lineWidth);

        sphereColl = gameObject.GetComponent<SphereCollider>();

        //Find distance for the maximum radius
        pulseMax = new Vector2(Game.screenLeft, Game.screenTop).magnitude;

    }

    void Update() {
        //If holding the second finger, increase timer and decrease energy
        if (Input.GetMouseButton(1) && held) {
            timer += Time.deltaTime;
            if (timer > pulseBackEnergyRate) {
                if (Player.energy > 1)
                    Player.energy--;
                else {
                    held = false;
                    finalColor = Level.singleColourSelect(Input.mousePosition);
                }
                timer = 0;
            }
            Radius = Radius - 3 * Time.deltaTime;
        } else
            Radius = Radius + 3 * Time.deltaTime;

        //If you have released the button, and the pulse is the current one, set it to be not held and set the Colour
        if (Input.GetMouseButtonUp(0) && held) {
            held = false;
            finalColor = Level.singleColourSelect(Input.mousePosition);
        }

        //What the colour should be - this is where the transition has to take place. 
        Color chosen;
        if (held)
            chosen = Level.singleColourSelect(Input.mousePosition);
        else
            chosen = finalColor;


        //Create the circle, and set the line material
        RedrawPoints(chosen);
        line.material.color = chosen;
        sphereColl.radius = Radius + 0.1f;

        //If too big, destroy itself
        if (Radius > pulseMax || (Radius < .3f) || CurrentHealth == 0)
            Destroy(gameObject);

    }

    void OnTriggerExit(Collider otherObject) {
        if (otherObject.GetType() == typeof(BoxCollider)) {
            CurrentHealth--;
            if (CurrentHealth == 0)
                Destroy(gameObject);
        }
    }

    // Reduce pulse health if it collides with another object
    void OnTriggerEnter(Collider otherObject) {
        if (otherObject.GetType() == typeof(SphereCollider)) {
            CurrentHealth--;
            if (CurrentHealth == 0) {
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    ///  Recalculates point positions
    /// </summary>
    /// <param name="c">Color of the points/line</param>
    void RedrawPoints(Color c) {
        float x;
        float y;
        float z = 0f;
        float angle = 0f;

        for (int i = 0; i < (segments + 1); i++) {
            x = Mathf.Sin(Mathf.Deg2Rad * angle);
            y = Mathf.Cos(Mathf.Deg2Rad * angle);
            line.SetPosition(i, new Vector3(x, y, z) * Radius);
            line.SetColors(new Color(c.r, c.g, c.b, ((CurrentHealth / MaxHealth) * .5f) + .5f), new Color(c.r, c.g, c.b, ((CurrentHealth / MaxHealth) * .5f) + .5f));
            line.material.SetColor("_Emission", new Color(c.r, c.g, c.b, c.a / 3));
            float lineWidth = CurrentHealth / 10;
            if (lineWidth < .2f)
                lineWidth += .05f;

            line.SetWidth(lineWidth, lineWidth);
            angle += (370f / segments - (370f/segments*0.01f));
        }
    }
}