using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public FlagContext currentContext;
	public Transform buttonHolster;
	public GameObject resultsHolster;

	public TextMeshProUGUI countryNameTMP;
	public TextMeshProUGUI resultsTextTMP;

	public Painter painter;

	public RawImage targetImage;

	public Slider thicknessSlider;

	public static GameManager Instance;

	private FlagContext[] contextDB;

	public int similarityPercent;

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
		LoadResources();
	}

	public void Setup(FlagContext context)
	{
		SetupButtons();
		ClearCanvas();
		HideResults();
		SetText(context.countryName);
	}

	private void HideResults()
	{
		resultsHolster.SetActive(false);
	}

	private void SetTargetImage()
	{
		targetImage.texture = currentContext.visualFlag;
	}

	private void SetText(string countryName)
	{
		countryNameTMP.text = countryName;
	}

	public void DonePainting()
	{
		CompareFlags();
		SetTargetImage();
		DisplayResults();
	}

	private void DisplayResults()
	{
		resultsHolster.SetActive(true);
		resultsTextTMP.text = $"{similarityPercent}%";
	}

	internal void UpdateColor(Color color)
	{
		painter.SetColor(color);
	}

	private void LoadResources()
	{
		contextDB = Resources.LoadAll<FlagContext>("FlagObjects");
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

	public void NewRound()
	{
		LoadRandomFlag();
		Setup(currentContext);
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
		currentContext = contextDB[Random.Range(0, contextDB.Length)];
	}

	public void CompareFlags()
	{
		var a = painter.testTexture;
		var b = currentContext.visualFlag;

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
		similarityPercent = (int)Mathf.Round(matchPercent * 100);
		print($"The images were {similarityPercent}% similar. Sheesh");
	}

	public void ThicknessSliderChanged()
	{
		painter.SetBrushThickness((int)thicknessSlider.value);
	}
}
