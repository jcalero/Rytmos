using UnityEngine;
using System.Collections;

public class SpriteTools{

	#region Fields
	#endregion

	#region Functions

	public static float[] CalculateSprite(UIAtlas atlas, string name) {
		float[] output = new float[6];
		float left;
		float bottom;
		float width;
		float height;
		float UVHeight = 1f;
		float UVWidth = 1f;

		UIAtlas.Sprite sprite = atlas.GetSprite(name);
		if (sprite == null) {
			Debug.LogError("No sprite with that name: " + name);
			return null;
		}
		left = (int)sprite.inner.xMin;
		bottom = (int)sprite.inner.yMax;
		width = (int)sprite.inner.width;
		height = (int)sprite.inner.height;

		float widthHeightRatio = sprite.inner.width / sprite.inner.height;
		if (widthHeightRatio > 1)
			UVHeight = 1f / widthHeightRatio;       // It's a "wide" sprite
		else if (widthHeightRatio < 1)
			UVWidth = 1f * widthHeightRatio;        // It's a "tall" sprite

		output[0] = UVWidth;
		output[1] = UVHeight;
		output[2] = left;
		output[3] = bottom;
		output[4] = width;
		output[5] = height;

		return output;
	}

	#endregion
}
