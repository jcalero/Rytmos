using UnityEngine;
using System.Collections;

public class LineFeedback : MonoBehaviour {
	
	public GameObject linePrefab;
	
	// Use this for initialization
	void Start () {
		Color purple = new Color(.5f, 0, .5f, 1f);
		
		
		//Universal line properties
		linePrefab.GetComponent<LineRenderer>().SetVertexCount(2);
		linePrefab.GetComponent<LineRenderer>().SetWidth(.2f, .2f);
		
		//First line - Green to cyan
		linePrefab.GetComponent<LineRenderer>().SetPosition(0, new Vector3(Game.screenLeft, Game.screenBottom));
		linePrefab.GetComponent<LineRenderer>().SetPosition(1, new Vector3(Game.screenMiddle, Game.screenBottom));
		linePrefab.GetComponent<LineRenderer>().SetColors(Color.green, Color.cyan);
		Instantiate(linePrefab, new Vector3(Game.screenLeft, Game.screenBottom, 0), linePrefab.transform.localRotation);
		
		//Second line - Cyan to blue
		linePrefab.GetComponent<LineRenderer>().SetPosition(0, new Vector3(Game.screenMiddle, Game.screenBottom));
		linePrefab.GetComponent<LineRenderer>().SetPosition(1, new Vector3(Game.screenRight, Game.screenBottom));
		linePrefab.GetComponent<LineRenderer>().SetColors(Color.cyan, Color.blue);
		Instantiate(linePrefab, new Vector3(Game.screenMiddle, Game.screenBottom, 0), linePrefab.transform.localRotation);
		
		//Third line - blue to purple
		linePrefab.GetComponent<LineRenderer>().SetPosition(0, new Vector3(Game.screenRight, Game.screenBottom));
		linePrefab.GetComponent<LineRenderer>().SetPosition(1, new Vector3(Game.screenRight, Game.screenTop));
		linePrefab.GetComponent<LineRenderer>().SetColors(Color.blue, purple);
		Instantiate(linePrefab, new Vector3(Game.screenRight, Game.screenBottom, 0), linePrefab.transform.localRotation);
		
		//Fourth line - purple to red
		linePrefab.GetComponent<LineRenderer>().SetPosition(0, new Vector3(Game.screenRight, Game.screenTop));
		linePrefab.GetComponent<LineRenderer>().SetPosition(1, new Vector3(Game.screenMiddle, Game.screenTop));
		linePrefab.GetComponent<LineRenderer>().SetColors(purple, Color.red);
		Instantiate(linePrefab, new Vector3(Game.screenRight, Game.screenTop, 0), linePrefab.transform.localRotation);
		
		//Fifth line - red to yellow
		linePrefab.GetComponent<LineRenderer>().SetPosition(0, new Vector3(Game.screenMiddle, Game.screenTop));
		linePrefab.GetComponent<LineRenderer>().SetPosition(1, new Vector3(Game.screenLeft, Game.screenTop));
		linePrefab.GetComponent<LineRenderer>().SetColors(Color.red, Color.yellow);
		Instantiate(linePrefab, new Vector3(Game.screenMiddle, Game.screenTop, 0), linePrefab.transform.localRotation);
		
		//Sixth line - yellow to green
		linePrefab.GetComponent<LineRenderer>().SetPosition(0, new Vector3(Game.screenLeft, Game.screenTop));
		linePrefab.GetComponent<LineRenderer>().SetPosition(1, new Vector3(Game.screenLeft, Game.screenBottom));
		linePrefab.GetComponent<LineRenderer>().SetColors(Color.yellow, Color.green);
		Instantiate(linePrefab, new Vector3(Game.screenLeft, Game.screenTop, 0), linePrefab.transform.localRotation);
		
	}
}
