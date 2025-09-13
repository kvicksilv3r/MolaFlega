using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public FlagContext currentContext;
    public Transform buttonHolster;
    public GameObject resultsHolster;

    public TextMeshProUGUI countryNameTMP;
    public TextMeshProUGUI resultsTextTMP;

    public GameObject resultsImage;

    public Painter painter;

    public RawImage targetImage;

    public Slider thicknessSlider;

    public static GameManager Instance;

    private FlagContext[] contextDB;

    public Transform thicknessPreview;

    private float thicknessMaxSize = 0.7f;

    public GameObject donePaintingButton;
    public GameObject nextRoundButton;

    public GameObject interactionsHolster;

    public float canvasRefWidth;
    public float canvasRefHeight;

    public float resultRefWidth;
    public float resultRefHeight;

    public float intendedFlagWidth = 300;
    public float intendedFlagHeight = 200;

    public Button undoButton;

    public GameObject paintCanvas;

    private int lastFlagID = 999999;

    public int similarityPercent;

    public bool forceLoadCountryButton = false;
    public FlagContext manualCountrySelection;

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
        painter.InitPainter(context.visualFlag.width, context.visualFlag.height);
        HideResults();
        SetText(context.countryName);
    }

    private void HideResults()
    {
        resultsHolster.SetActive(false);
        resultsImage.SetActive(false);
    }

    public void UndoPaint()
    {
        painter.PopPaintStack();
    }

    public void NewCanvas()
    {
        painter.NewPainting();
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
        donePaintingButton.SetActive(false);
        nextRoundButton.SetActive(true);
    }

    private void DisplayResults()
    {
        resultsHolster.SetActive(true);
        resultsTextTMP.text = $"{similarityPercent}%";
        resultsImage.SetActive(true);
        interactionsHolster.SetActive(false);
    }

    internal void UpdateColor(Color color)
    {
        painter.SetColor(color);
    }

    private void Update()
    {
        undoButton.interactable = (painter.GetPaintStackSize() > 1);
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

    public void ClearCanvas()
    {
        painter.ClearCanvas();
    }

    public void NewRound()
    {
        LoadRandomFlag();

        SetCanvasPhysicalSize();
        SetResultsPhysicalSize();

        Setup(currentContext);
        interactionsHolster.SetActive(true);
        donePaintingButton.SetActive(true);
        nextRoundButton.SetActive(false);
    }


    private void SetCanvasPhysicalSize()
    {
        var width = currentContext.visualFlag.width * (canvasRefWidth / intendedFlagWidth);
        var height = currentContext.visualFlag.height * (canvasRefHeight / intendedFlagHeight);

        paintCanvas.transform.localScale = new Vector3(width, 1, height);
    }

    private void SetResultsPhysicalSize()
    {
        var width = currentContext.visualFlag.width * (resultRefWidth / intendedFlagWidth);
        var height = currentContext.visualFlag.height * (resultRefHeight / intendedFlagHeight);

        resultsImage.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
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
        currentContext = contextDB[GetRandomFlagIndex()];
    }

    public int GetRandomFlagIndex()
    {
        var rngFlagIndex = Random.Range(0, contextDB.Length);
        if (rngFlagIndex == lastFlagID)
        {
            rngFlagIndex = GetRandomFlagIndex();
        }

        lastFlagID = rngFlagIndex;

        return rngFlagIndex;
    }

    public void CompareFlags()
    {
        var a = painter.mainPaintTexture;
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
        int badPixels = 0;
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
                else
                {
                    var targetPixel = b.GetPixel(width, height);
                    var colorMatch = 0;
                    for (int x = 0; x < currentContext.colors.Length; x++)
                    {
                        if (currentContext.colors[x] == targetPixel)
                        {
                            colorMatch++;
                        }
                    }

                    if (colorMatch == 0)
                    {
                        badPixels++;
                    }
                }
            }
        }

        var totalPixels = (a.width * a.height) - badPixels;
        float matchPercent = (float)matches / (float)totalPixels;
        similarityPercent = (int)Mathf.Round(matchPercent * 100);
        print($"The images were {similarityPercent}% similar. Sheesh");
    }

    public void ThicknessSliderChanged()
    {
        painter.SetBrushThickness((int)thicknessSlider.value);
        SetThicknessPreview();
    }

    private void SetThicknessPreview()
    {
        var newSize = thicknessMaxSize * (thicknessSlider.value / thicknessSlider.maxValue);
        thicknessPreview.localScale = new Vector3(newSize, newSize, 1);
    }

    private void OnValidate()
    {
        if (forceLoadCountryButton && manualCountrySelection != null)
        {
            forceLoadCountryButton = false;

            currentContext = manualCountrySelection;

            SetCanvasPhysicalSize();
            SetResultsPhysicalSize();

            Setup(currentContext);
            interactionsHolster.SetActive(true);
            donePaintingButton.SetActive(true);
            nextRoundButton.SetActive(false);
        }
    }
}
