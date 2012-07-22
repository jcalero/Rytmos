using UnityEngine;
using System.Collections;

public class FloatingScorePool : MonoBehaviour {

	#region Fields
	public UILabel[] floatScoreLabels;

	public static UILabel[] FloatScoreLabels;

	private static int nextIndex;
	#endregion

	#region Functions

	void Awake() {
		FloatScoreLabels = floatScoreLabels;
	}

	// Use this for initialization
	void Start() {

	}

	// Update is called once per frame
	void Update() {

	}

	//public static UILabel Spawn() {
	//    int index = (lastIndex + 1) % FloatScoreLabels.Length;
	//    return FloatScoreLabels[index];
	//}

	public static UILabel Spawn(Vector3 worldPosition) {
		int index = nextIndex % FloatScoreLabels.Length;
		FloatScoreLabels[index].transform.position = worldPosition;
		nextIndex++;
		if (nextIndex > FloatScoreLabels.Length-1) nextIndex = 0;
		return FloatScoreLabels[index];
	}

	public static void DeSpawn(UILabel label) {

	}

	#endregion

}
