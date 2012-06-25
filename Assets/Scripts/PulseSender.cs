using UnityEngine;
using System.Collections;

public class PulseSender : MonoBehaviour {
    private int segments = 50;
    public float radius;
    private LineRenderer line;
    private SphereCollider sphereColl;
    private bool held;
    private Color finalColor = Color.clear;
    public float amountToHit;
    public float pulseHealth = 3;
    private float pulseMax;
    private float timer = 0;
    
    void Start () 
    {
        held = true;
        radius = .4f;
        line = gameObject.GetComponent<LineRenderer>();
        line.SetVertexCount (segments+1);
        line.useWorldSpace = false;
        line.material.color = finalColor;
        amountToHit = pulseHealth;
        float lineWidth = amountToHit/10;
        line.SetWidth(lineWidth, lineWidth); 

        sphereColl = gameObject.GetComponent<SphereCollider>();
        
        //Find distance for the radius
        pulseMax = new Vector2(Game.screenLeft, Game.screenTop).magnitude;
        
    }
    
    void Update () 
    {
        //If holding the second finger, increase timer and decrease energy
        if(Input.GetMouseButton(1) && held) {
            timer += Time.deltaTime;
            if (timer > 0.2f)
            {
                if (Player.energy > 1)
                    Player.energy -= 2;
                else {
                    held = false;
                    finalColor = Level.singleColourSelect(Input.mousePosition);
                }
                timer = 0;
            }
            radius = radius - 3 * Time.deltaTime;
        } else 
            radius = radius + 3 * Time.deltaTime;

        //If you have released the button, and the pulse is the current one, set it to be not held and set the Colour
        if(Input.GetMouseButtonUp(0) && held) {
            held = false;
            finalColor = Level.singleColourSelect(Input.mousePosition);
        }
        
        //What the colour should be - this is where the transition has to take place. 
        Color chosen;
        if(held) 
            chosen = Level.singleColourSelect(Input.mousePosition);
        else 
            chosen = finalColor;	
        
        
        //Create the circle, and set the line material
        RedrawPoints(chosen);
        line.material.color = chosen;
        sphereColl.radius = radius + 0.1f;
        
        //If too big, destroy itself
        if(radius > pulseMax || (radius < .3f) || amountToHit == 0)
            Destroy(gameObject);
        
    }
    
    void OnTriggerEnter (Collider otherObject) 
    {
        amountToHit--;
        if(amountToHit==0) {
            Destroy(gameObject);
        }
        
    }
    
    void RedrawPoints (Color c) 
    {
        float x;
        float y;
        float z = 0f;
        float angle = 0f;
        
        for(int i=0; i<(segments+1); i++) {
            x = Mathf.Sin (Mathf.Deg2Rad * angle);
            y = Mathf.Cos (Mathf.Deg2Rad * angle);			
            line.SetPosition (i, new Vector3(x,y,z) * radius);
            line.SetColors(new Color(c.r, c.g, c.b, ((amountToHit/pulseHealth)*.5f)+.5f)   ,new Color(c.r, c.g, c.b, ((amountToHit/pulseHealth)*.5f)+.5f));
            line.material.SetColor("_Emission", new Color(c.r, c.g, c.b, c.a/3));
            float lineWidth = amountToHit/10;
            if(lineWidth < .2f) 
                lineWidth += .05f;	
            
            line.SetWidth(lineWidth, lineWidth);
            angle += (360f / segments);
        }
    }   
}
