//
//Made by HikkusT
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorQuantization : MonoBehaviour {

	public ComputeShader Trainer;
	public Material ColorQMat;
	public int PaletteSize = 2;
	public bool AutoCorrect = false;
	public int Threshold = 10000;

	//Data Handlers
	private int[] colorData;
	private ComputeBuffer colorBuffer;
	int handleColorQInitialize;
	int handleColorQ;
	private Color[] newColors;
	float linePos = 0.5f;
	float lineVel = 1.5f;
	Texture2D colorPalette;
	public RawImage palette;

	void Start () {
		//Getting References
		handleColorQInitialize = Trainer.FindKernel("ColorQInitialize");
		handleColorQ = Trainer.FindKernel("ColorQ");

		InitializeColorPalette(PaletteSize);
	}
	
	void Update () 
	{
		if(Input.GetKey(KeyCode.Mouse0))
			linePos = Mathf.Lerp(linePos, 1, lineVel * Time.deltaTime);

		if(Input.GetKey(KeyCode.Mouse1))
			linePos = Mathf.Lerp(linePos, 0, lineVel * Time.deltaTime);	

		if(Input.GetKey(KeyCode.Space))
			linePos = Mathf.Lerp(linePos, 0.5f, 2f * lineVel * Time.deltaTime);

		if(Input.GetKeyDown(KeyCode.E))
		{
			PaletteSize ++;
			InitializeColorPalette(PaletteSize);
		}

		if(Input.GetKeyDown(KeyCode.Q))
		{
			PaletteSize --;
			InitializeColorPalette(PaletteSize);
		}

		if(Input.GetKeyDown(KeyCode.R))
		{
			ColorQMat.EnableKeyword("EUCLIDIAN_DISTANCE");
			Trainer.SetBool("EUCLIDIAN_DISTANCE", true);
		}

		if(Input.GetKeyDown(KeyCode.T))
		{
			ColorQMat.DisableKeyword("EUCLIDIAN_DISTANCE");
			Trainer.SetBool("EUCLIDIAN_DISTANCE", false);
		}

		ColorQMat.SetTexture("_ColorPalette", colorPalette);
		ColorQMat.SetFloat("_LineP", linePos);
		palette.texture = colorPalette;
	}

	void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		Graphics.Blit(src, dest, ColorQMat);

		//Train our color palette
		Trainer.SetTexture(handleColorQ, "Screen", src);
		Trainer.SetTexture(handleColorQ, "ColorPalette", colorPalette);
		Trainer.Dispatch(handleColorQInitialize, PaletteSize, 1, 1);
		Trainer.Dispatch(handleColorQ, (src.width + 7)/8, (src.height + 7)/8, 1);

		//Retrieve Data
		colorBuffer.GetData(colorData);

		UpdateColorPalette();
	}

	void InitializeColorPalette	(int size)
	{
		//Allocating some variables
		colorBuffer = new ComputeBuffer(size, sizeof(int) * 4);
		colorData = new int[size * 4];
		newColors = new Color[size];

		Trainer.SetBuffer(handleColorQ, "ColorBuffer", colorBuffer);
		Trainer.SetBuffer(handleColorQInitialize, "ColorBuffer", colorBuffer);

		//Create texture
		colorPalette = new Texture2D(1, size, TextureFormat.ARGB32, false);

		//Set values
		for (int i = 0; i < size; i ++)
			colorPalette.SetPixel(0, i, new Color(i / (float)size, i / (float)size, i / (float)size, 1f));
		
		//Apply
		colorPalette.filterMode = FilterMode.Point;
     	colorPalette.Apply();
		ColorQMat.SetFloat("_Size", size);
		Trainer.SetInt("Size", size);
	}

	void UpdateColorPalette()
	{
		int maxIndex = 0;
		int maxData = 0;

		//Loop through accumulated texture(actually buffer) and compute the new colors
		for(int i = 0; i < PaletteSize; i ++)
		{
			if(colorData[i * 4 + 3] != 0)
			{
				newColors[i].r = colorData[i * 4 + 0] / (colorData[i * 4 + 3] * 256f);
				newColors[i].g = colorData[i * 4 + 1] / (colorData[i * 4 + 3] * 256f);
				newColors[i].b = colorData[i * 4 + 2] / (colorData[i * 4 + 3] * 256f);
				newColors[i].a = 1f;

				if (colorData[i * 4 + 3] > maxData)
				{
					maxData = colorData[i * 4 + 3];
					maxIndex = i;
				}
			}
			else
				newColors[i] = Color.white;
		}

		//Check if a color is being under-used
		if (AutoCorrect)
		{
			for(int i = 0; i < PaletteSize; i ++)
			{
				if(colorData[i * 4 + 3] < Threshold)
					newColors[i] = newColors[maxIndex] - new Color(0.05f, 0.05f, 0.05f, 0f);
			}
		}

		//Update the texture
		colorPalette.SetPixels(newColors);
		colorPalette.Apply();
	}

	void Debug()
	{
		//Colors
		for(int i = 0; i < PaletteSize; i ++)
		{
			print(newColors[i].r);
			print(newColors[i].g);
			print(newColors[i].b);
		}

		//Density
		for(int i = 0; i < PaletteSize; i ++)
		 {
		 	print(colorData[i * 4 + 0] + " " + i);
		 	print(colorData[i * 4 + 1] + " " + i);
		 	print(colorData[i * 4 + 2] + " " + i);
		 	print(colorData[i * 4 + 3] + " " + i);
		 }
	}
}
