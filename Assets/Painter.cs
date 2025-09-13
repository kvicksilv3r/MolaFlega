using System;
using System.Collections.Generic;
using UnityEngine;

public class Painter : MonoBehaviour
{
    public Renderer planeRenderer;

    public Material planeMat;

    public Texture2D mainPaintTexture;

    public Stack<Texture2D> paintStack = new Stack<Texture2D>();

    //debug
    public List<Texture2D> debugList = new List<Texture2D>();

    public int textureWidth = 300;
    public int textureHeight = 200;

    public bool testScratch = false;

    public Color paintColor;

    [Range(1, 20)]
    public int paintThickness = 10;

    private bool holdingMouseButton = false;
    private bool heldMouseButtonLastFrame = false;
    private Vector2 lastHitCoord;

    private Color[] pxls = new Color[65536];

    public int minDrawsOnDrag = 4;
    public int magnitudeAmp = 100;

    private bool pixelsChanged = false;

    void Start()
    {
        Application.targetFrameRate = 60;
    }

    public void InitPainter(int textureWidth = 300, int textureHeight = 200)
    {
        this.textureWidth = textureWidth;
        this.textureHeight = textureHeight;

        planeMat = planeRenderer.material;
        CreateTexture();
        AssignTexture();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            holdingMouseButton = true;
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            holdingMouseButton = false;
            heldMouseButtonLastFrame = false;

            if (pixelsChanged)
            {
                PushPaintStack();
                pixelsChanged = false;
            }
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            DoRaycast();
        }
    }

    private void DoRaycast()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


        if (Physics.Raycast(ray, out hit))
        {
            if (mainPaintTexture != null)
            {
                int xCoord = (int)(hit.textureCoord.x * textureWidth);
                int yCoord = (int)(hit.textureCoord.y * textureHeight);

                if (heldMouseButtonLastFrame)
                {
                    var magnitude = (int)(((lastHitCoord - hit.textureCoord) * magnitudeAmp).magnitude);
                    if (magnitude < minDrawsOnDrag) { magnitude = minDrawsOnDrag; }

                    //print(magnitude);
                    for (int i = 0; i < magnitude; i++)
                    {
                        var lerpedHit = Vector2.Lerp(lastHitCoord, hit.textureCoord, (float)i / (float)magnitude);
                        var x = (int)(lerpedHit.x * textureWidth);
                        var y = (int)(lerpedHit.y * textureHeight);
                        PaintPixels(x, y);
                    }
                }

                else
                {
                    PaintPixels(xCoord, yCoord);
                }
                lastHitCoord = hit.textureCoord;
                heldMouseButtonLastFrame = true;
            }
        }
    }

    private void PaintPixels(int xCoord, int yCoord)
    {
        mainPaintTexture.Circle(xCoord, yCoord, paintThickness, paintColor);
        mainPaintTexture.Apply();
        pixelsChanged = true;
    }

    public void SetBrushThickness(int thickness)
    {
        paintThickness = thickness;
    }

    private void AssignTexture()
    {
        if (mainPaintTexture == null)
        {
            print("Make a texture first");
            return;
        }

        planeMat.mainTexture = mainPaintTexture;
    }

    private void CreateTexture()
    {
        MakeTexture();
    }

    private void MakeTexture()
    {
        mainPaintTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
        ClearCanvas();
    }

    public void NewPainting()
    {
        ClearPaintStack();

        ClearCanvas();
    }

    public void ClearCanvas()
    {
        for (int w = 0; w < textureWidth; w++)
        {
            for (int h = 0; h < textureHeight; h++)
            {
                mainPaintTexture.SetPixel(w, h, Color.white);
            }
        }

        mainPaintTexture.Apply();

        PushPaintStack();
    }

    public void SetColor(Color color)
    {
        paintColor = color;
    }

    private void PushPaintStack()
    {
        var newTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
        newTexture.CopyPixels(mainPaintTexture);
        newTexture.Apply();
        paintStack.Push(newTexture);

        debugList.Add(newTexture);
    }

    public void PopPaintStack()
    {
        if (paintStack.Count > 1)
        {
            paintStack.Pop();
            mainPaintTexture.CopyPixels(paintStack.Peek());
            mainPaintTexture.Apply();

            debugList.RemoveAt(debugList.Count - 1);
        }
    }

    private void ClearPaintStack()
    {
        paintStack.Clear();
        debugList.Clear();
    }

    public int GetPaintStackSize()
    {
        return paintStack.Count;
    }
}