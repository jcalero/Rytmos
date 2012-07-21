using UnityEngine;
using System.Collections;

public class UITools : MonoBehaviour {

	#region Fields

	#endregion

	#region Functions

	public static void SetActiveState(Transform t, bool state) {
		for (int i = 0; i < t.childCount; ++i) {
			Transform child = t.GetChild(i);
			//if (child.GetComponent<UIPanel>() != null) continue;

			if (state) {
				child.gameObject.active = true;
				SetActiveState(child, true);
			} else {
				SetActiveState(child, false);
				child.gameObject.active = false;
			}
		}
	}

	/// <summary>
	/// Activate or deactivate the specified panel and all of its children.
	/// </summary>

	public static void SetActiveState(UIPanel panel, bool state) {
		if (state) {
			panel.gameObject.active = true;
			SetActiveState(panel.transform, true);
		} else {
			SetActiveState(panel.transform, false);
			panel.gameObject.active = false;
		}
	}

	public static void SetActiveState<T>(T widget, bool state) where T : UIWidget {
		if (state) {
			widget.gameObject.active = true;
			SetActiveState(widget.transform, true);
		} else {
			SetActiveState(widget.transform, false);
			widget.gameObject.active = false;
		}
	}

	public static void SetActiveState(UIButton button, bool state) {
		if (state) {
			button.gameObject.active = true;
			SetActiveState(button.transform, true);
		} else {
			SetActiveState(button.transform, false);
			button.gameObject.active = false;
		}
	}

	#endregion
}
