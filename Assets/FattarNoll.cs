using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FattarNoll : MonoBehaviour
{
	public Renderer planeRenderer;

	public Material planeMat;

	public Texture2D testTexture;

	public int textureWidth = 256;
	public int textureHeight = 256;

	public bool testScratch = false;

	public Color paintColor;

	[Range(1, 20)]
	public int paintThickness = 10;

	private float scratch = 1;

	private Color[] pxls = new Color[65536];


	// Start is called before the first frame update
	void Start()
	{
		Application.targetFrameRate = 60;

		planeMat = planeRenderer.material;

		CreateTexture();
		AssignTexture();
	}

	// Update is called once per frame
	void Update()
	{

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
			print("Eyo boys we hit" + hit.textureCoord);

			if (testTexture != null)
			{
				//testTexture.SetPixel((int)(hit.textureCoord.x * textureWidth), (int)(hit.textureCoord.y * textureHeight), new Color(0, 1, 1, 1));
				int xCoord = (int)(hit.textureCoord.x * textureWidth);
				int yCoord = (int)(hit.textureCoord.y * textureHeight);

				testTexture.Circle(xCoord, yCoord, paintThickness, paintColor);
				testTexture.Apply();
			}
		}
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
	}
}
