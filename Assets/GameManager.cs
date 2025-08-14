using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public FlagContext currentContext;
	public Transform buttonHolster;

	public TextMeshProUGUI countryNameTMP;
	public Painter painter;

	public RawImage targetImage;

	public static GameManager Instance;

	private void Awake()
	{
		if (!Instance)
		{
			Instance = this;
		}

		if (Instance != this)
		{
			Destroy(this);
		}
	}

	void Start()
	{
		painter.InitPainter(300, 200);
	}

	public void Setup(FlagContext context)
	{
		SetupButtons();
		SetTargetImage();
		ClearCanvas();
		SetText(context.countryName);
	}

	private void SetTargetImage()
	{
		targetImage.texture = currentContext.flag;
	}

	private void SetText(string countryName)
	{
		countryNameTMP.text = countryName;
	}

	internal void UpdateColor(Color color)
	{
		painter.SetColor(color);
	}

	private void SetupButtons()
	{
		//Setup all colors
		for (int i = 0; i < buttonHolster.childCount; i++)
		{
			var b = buttonHolster.GetChild(i);

			if (i < currentContext.colors.Length)
			{
				b.gameObject.SetActive(true);
				b.gameObject.GetComponent<ColorButton>().SetColor(currentContext.colors[i]);
			}
			else
			{
				b.gameObject.SetActive(false);
			}
		}

		//Set default color
		buttonHolster.GetChild(0).GetComponent<ColorButton>().ClickedOn();
	}

	private void ClearCanvas()
	{
		painter.ClearCanvas();
	}

	public void ManualSetup()
	{
		if (!currentContext)
		{
			return;
		}

		Setup(currentContext);
	}

	public void LoadRandomFlag()
	{

	}

	public void CompareFlags()
	{
		var a = painter.testTexture;
		var b = currentContext.flag;

		if (a.width != b.width)
		{
			print("The images are not the same width");
			return;
		}

		if (a.height != b.height)
		{
			print("The images are not the same height");
			return;
		}

		int matches = 0;
		Color aColor;
		Color bColor;

		for (int width = 0; width < a.width; width++)
		{
			for (int height = 0; height < a.height; height++)
			{
				aColor = a.GetPixel(width, height);
				bColor = b.GetPixel(width, height);

				if (a.GetPixel(width, height) == b.GetPixel(width, height))
				{
					matches++;
				}
			}
		}

		var totalPixels = a.width * a.height;
		float matchPercent = (float)matches / (float)totalPixels;

		print($"The images were {Mathf.Round(matchPercent * 100)}% similar. Sheesh");
	}
}
