using System;
using UnityEngine;
using UnityEngine.UI;

public class ColorButton : MonoBehaviour
{
	public Color color;
	public Outline outline;

	public void SetColor(Color color)
	{
		this.color = color;
		GetComponent<Image>().color = color;
	}

	public void SetOutline(bool enabled)
	{
		outline.enabled = enabled;
	}

	public void ClickedOn()
	{
		HandleOutline();
		ReportColor();
	}

	private void ReportColor()
	{
		GameManager.Instance.UpdateColor(color);
	}

	private void HandleOutline()
	{
		for (int i = 0; i < transform.parent.childCount; i++)
		{
			var child = transform.parent.GetChild(i);
			child.gameObject.GetComponent<ColorButton>().SetOutline(child == this.transform);
		}
	}
}
