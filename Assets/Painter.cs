using System;
using UnityEngine;

public class Painter : MonoBehaviour
{
	public Renderer planeRenderer;

	public Material planeMat;

	public Texture2D testTexture;

	public int textureWidth = 300;
	public int textureHeight = 200;

	public bool testScratch = false;

	public Color paintColor;

	[Range(1, 20)]
	public int paintThickness = 10;

	private float scratch = 1;

	private bool holdingMouseButton = false;
	private bool heldMouseButtonLastFrame = false;
	private Vector2 lastHitCoord;

	private Color[] pxls = new Color[65536];

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
		}

		if (Input.GetKey(KeyCode.Mouse0))
		{
			DoRaycast();
		}



		if (Input.GetKeyDown(KeyCode.D))
		{
			TextureDebug();
		}
	}

	private void TextureDebug()
	{
		print($"Bot left pixel is: {testTexture.GetPixel(0, 0)}");
		print($"Bot right pixel is: {testTexture.GetPixel(textureWidth - 1, 0)}");
		print($"Top left pixel is: {testTexture.GetPixel(0, textureHeight - 1)}");
		print($"Top right pixel is: {testTexture.GetPixel(textureWidth - 1, textureHeight - 1)}");
	}

	private void DoRaycast()
	{
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


		if (Physics.Raycast(ray, out hit))
		{
			if (testTexture != null)
			{
				int xCoord = (int)(hit.textureCoord.x * textureWidth);
				int yCoord = (int)(hit.textureCoord.y * textureHeight);

				if (heldMouseButtonLastFrame)
				{
					var magnitude = (int)((lastHitCoord - hit.textureCoord).magnitude);

					for (int i = 0; i < magnitude; i++)
					{
						var lerpedHit = Vector2.Lerp(lastHitCoord, hit.textureCoord, (float)i / (float)magnitude);
						var x = (int)lerpedHit.x * textureWidth;
						var y = (int)lerpedHit.y * textureHeight;
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
		testTexture.Circle(xCoord, yCoord, paintThickness, paintColor);
		testTexture.Apply();
	}

	private void AssignTexture()
	{
		if (testTexture == null)
		{
			print("Make a texture first");
			return;
		}

		planeMat.mainTexture = testTexture;
	}

	private void CreateTexture()
	{
		if (testTexture == null)
		{
			MakeTexture();
		}
	}

	private void MakeTexture()
	{
		testTexture = new Texture2D(textureWidth, textureHeight);
		ClearCanvas();
	}

	public void ClearCanvas()
	{
		for (int w = 0; w < textureWidth; w++)
		{
			for (int h = 0; h < textureHeight; h++)
			{
				testTexture.SetPixel(w, h, Color.white);
			}
		}

		testTexture.Apply();
	}

	public void SetColor(Color color)
	{
		paintColor = color;
	}
}

