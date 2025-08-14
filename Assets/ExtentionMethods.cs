using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtentionMethods
{
	public static Texture2D Circle(this Texture2D tex, int x, int y, int r, Color color)
	{
		float rSquared = r * r;

		for (int u = 0; u < tex.width; u++)
		{
			for (int v = 0; v < tex.height; v++)
			{
				if ((x - u) * (x - u) + (y - v) * (y - v) < rSquared) tex.SetPixel(u, v, color);
			}
		}

		return tex;
	}
}
