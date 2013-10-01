using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;


public static class VectorUtils
{
	public static bool SegmentsIntersect(Vector2 A, Vector2 B, Vector2 C, Vector2 D)
	{
		float S, T;
	
		if( VectorUtils.LinesIntersect(A, B, C, D, out S, out T )
		   && (S >= 0.0f && S <= 1.0f && T >= 0.0f && T <= 1.0f) )
			return true;
	
		return false;
	}
	
	public static Vector2 LinesIntersectPoint(Vector2 A, Vector2 B, Vector2 C, Vector2 D, out bool valid)
	{
		float S, T;
	
		if( VectorUtils.LinesIntersect(A, B, C, D, out S, out T) ) {
			// Point of intersection
			Vector2 P;
			P.x = A.x + S * (B.x - A.x);
			P.y = A.y + S * (B.y - A.y);
			valid=true;
			return P;
		}
		valid=false;
		return Vector2.zero;
	}
	
	public static bool LinesIntersect(Vector2 A, Vector2 B,
						  Vector2 C, Vector2 D,
						  out float S, out float T)
	{    
		// FAIL: Line undefined
		if ( (A.x==B.x && A.y==B.y) || (C.x==D.x && C.y==D.y) ) {
			S=T=0;
			return false;
		}
	
		float BAx = B.x - A.x;
		float BAy = B.y - A.y;
		float DCx = D.x - C.x;
		float DCy = D.y - C.y;
		float ACx = A.x - C.x;
		float ACy = A.y - C.y;
	
		float denom = DCy*BAx - DCx*BAy;
	
		S = DCx*ACy - DCy*ACx;
		T = BAx*ACy - BAy*ACx;
	
		if (denom == 0) {
			if (S == 0 || T == 0) { 
				// Lines incident
				return true;   
			}
			// Lines parallel and not incident
			return false;
		}
	
		S = S / denom;
		T = T / denom;
	
		// Point of intersection
		// Vector2 P;
		// P.x = A.x + *S * (B.x - A.x);
		// P.y = A.y + *S * (B.y - A.y);
	
		return true;
	}
	
	public static float Angle (Vector2 vector) {
	    Vector2 to = new Vector2(1, 0);
	
	    float result = Vector2.Angle( vector, to );
	    Vector3 cross = Vector3.Cross( vector, to );
	
	    if (cross.z > 0)
	       result = 360f - result;
	
	    return result;
	}
}

public static class RandomUtils
{
	public static Color RandomColor() {
		return new Color(RXRandom.Float (),RXRandom.Float (),RXRandom.Float (),RXRandom.Float ());
	}
	
	public static T RandomEnum<T>() { 
		T[] values = (T[]) Enum.GetValues(typeof(T));
		return values[RXRandom.Range(0,values.Length)];
	}
	
	public static Vector2 RandomDirection(float length) {
		float angle=RXRandom.Float((float)Math.PI*2f);
		return new Vector2((float)Math.Cos(angle)*length,(float)Math.Sin (angle)*length);
	}
}

public static class ColorUtils
{
	public static Color InterpolateRGBColor(Color fromColor, Color toColor, float progress) {
		return new Color(fromColor.r+(toColor.r-fromColor.r)*progress,fromColor.g+(toColor.g-fromColor.g)*progress,fromColor.b+(toColor.b-fromColor.b)*progress);
	}

	public static Color InterpolateRGBAColor(Color fromColor, Color toColor, float progress) {
		return new Color(fromColor.r+(toColor.r-fromColor.r)*progress,fromColor.g+(toColor.g-fromColor.g)*progress,fromColor.b+(toColor.b-fromColor.b)*progress,fromColor.a+(toColor.a-fromColor.a)*progress);
	}

	public static Color InterpolateHSLColor(Color fromColor, Color toColor, float progress) {
		RXColorHSL fromHSL=RXColor.HSLFromColor(fromColor);
		RXColorHSL toHSL=RXColor.HSLFromColor(toColor);
		return RXColor.ColorFromHSL(fromHSL.h+(toHSL.h-fromHSL.h)*progress,fromHSL.s+(toHSL.s-fromHSL.s)*progress,fromHSL.l+(toHSL.l-fromHSL.l)*progress,1f);
	}
}

public static class FutileUtils
{
	/* RenderTexture only available in Unity Pro */
	/*
	public static Texture2D RenderToTexture(FNode n,Rect r) {
		return FutileUtils.RenderToTexture(n,r,16,RenderTextureFormat.ARGB32,RenderTextureReadWrite.Default);
	}
	public static Texture2D RenderToTexture(FNode n,Rect r,int depth,RenderTextureFormat format,RenderTextureReadWrite readWrite) {
		RenderTexture texture = new RenderTexture( (int)r.width,(int)r.height,depth,format,readWrite);
		//Texture2D texture = new Texture2D(r.width,r.height,format,false,Linear);
		return null;
	}
	public static void ScreenShot() {
		RenderTexture rt = new RenderTexture((int)Futile.screen.pixelWidth, (int)Futile.screen.pixelHeight, 24);
        Futile.instance.camera.targetTexture = rt;
        Texture2D screenShot = new Texture2D((int)Futile.screen.pixelWidth, (int)Futile.screen.pixelHeight, TextureFormat.RGB24, false);
        Futile.instance.camera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, (int)Futile.screen.pixelWidth, (int)Futile.screen.pixelHeight), 0, 0);
        Futile.instance.camera.targetTexture = null;
        RenderTexture.active = null;
        //Destroy(rt);
		var bytes = screenShot.EncodeToPNG();
		//System.IO.File.WriteAllBytes(Application.dataPath + "/screenshots/screen" + System.DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + ".png", bytes);
	}
	*/
	
	public static void RenderScreenToTexture(ref Texture2D texture) {
		RenderTexture rt = new RenderTexture((int)Futile.screen.pixelWidth, (int)Futile.screen.pixelHeight, 24);
        Futile.instance.camera.targetTexture = rt;
        Futile.instance.camera.Render();
        RenderTexture.active = rt;
        texture.ReadPixels(new Rect(0, 0, (int)Futile.screen.pixelWidth, (int)Futile.screen.pixelHeight), 0, 0);
        Futile.instance.camera.targetTexture = null;
        RenderTexture.active = null;
	}
	
	public static void SetRecursiveAlpha(FNode node,float alpha) {
		node.alpha=alpha;
		if (node is FContainer) {
			int childCount=((FContainer)node).GetChildCount();
			for (int i=0;i<childCount;i++) {
				FNode child=((FContainer)node).GetChildAt(i);
				FutileUtils.SetRecursiveAlpha(child,alpha);
			}
		}
	}
}


public static class CodeUtils
{
	//Convert string to enum value
	public static T ParseEnum<T>( string value )
	{
	    return (T) Enum.Parse( typeof( T ), value, true );
	}
}


public static class LabelUtils
{
	public static void ScaleMaxWidth(FLabel label,float maxScale,float maxWidth) 
	{
		label.scale=maxScale;
		float currentWidth=label.textRect.width*label.scaleX;
		if (currentWidth>maxWidth) {
			label.scale=maxWidth/label.textRect.width;
		}
	}
}

public enum NodeAlign {
	CENTER,
	LEFT,
	RIGHT,
	TOP,
	BOTTOM,
	TOPLEFT,
	TOPRIGHT,
	BOTTOMLEFT,
	BOTTOMRIGHT,
	NONE,
}

public static class SpriteUtils
{
	public static void Fit(FSprite sprite,Rect rect,NodeAlign align=NodeAlign.CENTER) 
	{
		sprite.scale = Mathf.Min(sprite.textureRect.height/rect.height,sprite.textureRect.width/rect.width);
		Align(sprite,rect,align);
	}
	
	public static void Fill(FSprite sprite,Rect rect,NodeAlign align=NodeAlign.CENTER) 
	{
		sprite.scale = Mathf.Max(sprite.textureRect.height/rect.height,sprite.textureRect.width/rect.width);
		Align(sprite,rect,align);
	}
	
	public static void Align(FSprite sprite,Rect rect,NodeAlign align) {
		switch(align) {
			case NodeAlign.CENTER: {
				sprite.SetPosition(rect.center);
				break;	
			}
			case NodeAlign.LEFT: {
				sprite.y=rect.center.y;
				sprite.x=rect.xMin+sprite.textureRect.width*sprite.scaleX*0.5f;
				break;	
			}
			case NodeAlign.RIGHT: {
				sprite.y=rect.center.y;
				sprite.x=rect.xMax-sprite.textureRect.width*sprite.scaleX*0.5f;
				break;	
			}
			case NodeAlign.TOP: {
				sprite.x=rect.center.x;
				sprite.y=rect.yMax-sprite.textureRect.height*sprite.scaleY*0.5f;
				break;	
			}
			case NodeAlign.BOTTOM: {
				sprite.x=rect.center.x;
				sprite.y=rect.yMin+sprite.textureRect.height*sprite.scaleY*0.5f;
				break;	
			}
			case NodeAlign.TOPLEFT: {
				sprite.x=rect.xMin+sprite.textureRect.width*sprite.scaleX*0.5f;
				sprite.y=rect.yMax-sprite.textureRect.height*sprite.scaleY*0.5f;
				break;	
			}
			case NodeAlign.TOPRIGHT: {
				sprite.x=rect.xMax-sprite.textureRect.width*sprite.scaleX*0.5f;
				sprite.y=rect.yMax-sprite.textureRect.height*sprite.scaleY*0.5f;
				break;	
			}
			case NodeAlign.BOTTOMLEFT: {
				sprite.x=rect.xMin+sprite.textureRect.width*sprite.scaleX*0.5f;
				sprite.y=rect.yMin+sprite.textureRect.height*sprite.scaleY*0.5f;
				break;	
			}
			case NodeAlign.BOTTOMRIGHT: {
				sprite.x=rect.xMax-sprite.textureRect.width*sprite.scaleX*0.5f;
				sprite.y=rect.yMin+sprite.textureRect.height*sprite.scaleY*0.5f;
				break;	
			}
		}
	}
}


public static class FileUtils
{
	public static void Save(string file,byte[] buffer)
	{
#if UNITY_WEBPLAYER
		return;
#else
		using (FileStream stream = new FileStream(file, FileMode.Create))
		{
		    using (BinaryWriter writer = new BinaryWriter(stream))
		    {
				writer.Write(buffer.Length);
		        writer.Write(buffer);
		    }
		}
#endif
	    
	}
	
	public static byte[] Load(string file)
	{
#if UNITY_WEBPLAYER
		return null;
#else
		if (!File.Exists(file)) return null;
	    using (FileStream stream = new FileStream(file, FileMode.Open))
		{
		    using (BinaryReader reader = new BinaryReader(stream))
		    {
				int length=reader.ReadInt32();
		        return reader.ReadBytes(length);
		    }
		}
#endif
		
	}
}
