using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorQuantization : MonoBehaviour {

	public ComputeShader computeShader;
	public uint[] histogramData;
	public int[] colorData;

	ComputeBuffer histogramBuffer;
	ComputeBuffer colorBuffer;
	int handleMain;
	int handleInitialize;
	int handleColorQInitialize;
	int handleColorQ;
	Color[] newColors;


	public int Threshold = 50000;


	public int Size = 2;
	public GameObject Trainer;
	public Texture2D colorPalette;
	public Material colorqMat;


	// Use this for initialization
	void Start () {
		//
		handleColorQInitialize = computeShader.FindKernel("ColorQInitialize");
		handleColorQ = computeShader.FindKernel("ColorQ");
		colorBuffer = new ComputeBuffer(4, sizeof(int) * 4);
		colorData = new int[4 * 4];
		newColors = new Color[4];

		
		computeShader.SetBuffer(handleColorQ, "ColorBuffer", colorBuffer);
		computeShader.SetBuffer(handleColorQInitialize, "ColorBuffer", colorBuffer);

		colorPalette = new Texture2D(Size, Size, TextureFormat.ARGB32, false);
		// colorPalette.SetPixel(0, 0, Color.red);
     	// colorPalette.SetPixel(1, 0, Color.blue);
		// colorPalette.SetPixel(2, 0, new Color(0.1f, 0.1f, 0.1f, 1f));
     	// colorPalette.SetPixel(0, 1, Color.green);
		// colorPalette.SetPixel(1, 1, new Color(0.2f, 0.2f, 0.2f, 1f));
     	// colorPalette.SetPixel(2, 1, Color.magenta);
		// colorPalette.SetPixel(0, 2, new Color(0.3f, 0.3f, 0.3f, 1f));
		// colorPalette.SetPixel(1, 2, new Color(0.4f, 0.4f, 0.4f, 1f));
		// colorPalette.SetPixel(2, 2, new Color(0.5f, 0.5f, 0.5f, 1f));
		colorPalette.SetPixel(0, 0, Color.red);
		colorPalette.SetPixel(1, 0, Color.green);
		colorPalette.SetPixel(0, 1, Color.blue);
		colorPalette.SetPixel(1, 1, Color.magenta);
		
		colorPalette.filterMode = FilterMode.Point;
 
     	// Apply all SetPixel calls
     	colorPalette.Apply();
	}
	
	// Update is called once per frame
	void Update () {
		 for(int i = 0; i < 4; i ++)
		 {
		 	print(colorData[i * 4 + 0] + " " + i);
		// 	print(colorData[i * 4 + 1]);
		// 	print(colorData[i * 4 + 2]);
		 	print(colorData[i * 4 + 3] + " " + i);
		 }
		

		colorqMat.SetTexture("_ColorPalette", colorPalette);

		//print(histogramData[200]);
	}

	void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		Graphics.Blit(src, dest, colorqMat);

		computeShader.SetTexture(handleColorQ, "InputTexture", src);
		computeShader.SetTexture(handleColorQ, "ColorPalette", colorPalette);
		computeShader.Dispatch(handleColorQInitialize, 4, 1, 1);
		computeShader.Dispatch(handleColorQ, (src.width + 7)/8, (src.height + 7)/8, 1);
		colorBuffer.GetData(colorData);

		int maxIndex = 0;
		int maxData = 0;

		for(int i = 0; i < 4; i ++)
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
			//else
				//newColors[i] = Color.white;

			//print(newColors[i]);
			if(colorData[i * 4 + 3] < Threshold)
			{
				// newColors[i].r = thresholdVec[i] * 0.0001f;
				// newColors[i].g = thresholdVec[i] * 0.0001f;
				// newColors[i].b = thresholdVec[i] * 0.0001f;
				// thresholdVec[i] ++;
				// print(newColors[i]);

			}

			//if(newColors[i] == new Color(0, 0, 0, 1f))
				//newColors[i] = Color.magenta;
		}

		for(int i = 0; i < 4; i ++)
		{
			if(colorData[i * 4 + 3] < Threshold)
			{
				//newColors[i] = newColors[maxIndex] - new Color(0.05f, 0.05f, 0.05f, 0f);
				print(newColors[i]);
			}
		}

		// for(int i = 0; i < 4; i ++)
		// {
		// 	print(newColors[i].r);
		// 	print(newColors[i].g);
		// 	print(newColors[i].b);
		// }
		colorPalette.SetPixels(newColors);
		colorPalette.Apply();
	}
}
