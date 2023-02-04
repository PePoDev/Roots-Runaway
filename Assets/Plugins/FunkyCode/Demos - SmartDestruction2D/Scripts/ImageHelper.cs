using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageHelper {

	public static Color32[] ResizeCanvas(Texture2D texture, int width, int height)
	{
		var newPixels = ResizeCanvas(texture.GetPixels32(), texture.width, texture.height, width, height);
		texture.Reinitialize(width, height);
		texture.SetPixels32(newPixels);
		texture.Apply();

		return newPixels;
	}

	public static Texture2D CopyTexture(Texture2D copyTexture) {
		Texture2D texture = new Texture2D(copyTexture.width, copyTexture.height);
		for(int x = 0; x < texture.width; ++x) {
			for(int y = 0; y < texture.height; ++y) {
				texture.SetPixel(x, y, copyTexture.GetPixel(x, y));
			}
		}
		texture.Apply();
		return(texture);
	}

	private static Color32[] ResizeCanvas(IList<Color32> pixels, int oldWidth, int oldHeight, int width, int height)
	{
		var newPixels = new Color32[(width * height)];
		var wBorder = (width - oldWidth) / 2;
		var hBorder = (height - oldHeight) / 2;

		for (int r = 0; r < height; r++) {
			var oldR = r - hBorder;
			if (oldR < 0) { continue; }
			if (oldR >= oldHeight) { break; }

			for (int c = 0; c < width; c++) {
				var oldC = c - wBorder;
				if (oldC < 0) { continue; }
				if (oldC >= oldWidth) { break; }

				var oldI = oldR*oldWidth + oldC;
				var i = r*width + c;
				newPixels[i] = pixels[oldI];
			}
		}

		return newPixels;
	}

	public static Texture2D RotateImage(Texture2D originTexture, int angle){
		Texture2D result;
		result = new Texture2D(originTexture.width, originTexture.height);
		Color32[] pix1 = result.GetPixels32();
		Color32[] pix2 = originTexture.GetPixels32();
		int W = originTexture.width;
		int H = originTexture.height;
		int x = 0, y = 0;
		Color32[] pix3 = rotateSquare(pix2, (Mathf.PI/180*(float)angle), originTexture);
		for (int j = 0; j < H; j++){
			for (var i = 0; i < W; i++) {
				//pix1[result.width/2 - originTexture.width/2 + x + i + result.width*(result.height/2-originTexture.height/2+j+y)] = pix2[i + j*originTexture.width];
				pix1[result.width/2 - W/2 + x + i + result.width*(result.height/2-H/2+j+y)] = pix3[i + j*W];
			}
		}
		result.SetPixels32(pix1);
		result.Apply();
	
		return result;
	}

	static Color32[] rotateSquare(Color32[] arr, float phi, Texture2D originTexture){
		int x, y, i, j;
		double sn = Mathf.Sin(phi);
		double cs = Mathf.Cos(phi);
		Color32[] arr2 = originTexture.GetPixels32();
		int W = originTexture.width;
		int H = originTexture.height;
		int xc = W / 2;
		int yc = H / 2;
		for (j=0; j<H; j++){
			for (i=0; i<W; i++){
				arr2[j*W+i] = new Color32(0,0,0,0);
				x = (int)(cs*(i-xc)+sn*(j-yc)+xc);
				y = (int)(-sn*(i-xc)+cs*(j-yc)+yc);
				if ((x>-1) && (x<W) &&(y>-1) && (y<H)){ 
					arr2[j*W+i]=arr[y*W+x];
				}
			}
		}

		return arr2;
	}
}


