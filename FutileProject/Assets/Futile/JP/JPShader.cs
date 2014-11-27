using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JPShader
{
	public static FShader Blur;
	public static FShader Test;
	public static FGlowShader Glow; 
	
	private JPShader()
	{
		throw new NotSupportedException("Use JPShader.Init();");
	}
	
	public static void Init() //called by Futile
	{
		Blur = new FBlurShader(0.003f);
		Test = new FTestShader();
		Glow = new FGlowShader(0.001f,new Color(1,1,1,0));
	}
}


public class FBlurShader : FShader
{
	private float _blurAmount;
	
	public FBlurShader(float blurAmount) : base("BlurShader", Shader.Find("Futile/Blur"))
	{
		_blurAmount = blurAmount;
		needsApply = true;
	}
	
	override public void Apply(Material mat)
	{
		mat.SetFloat("_BlurAmount",_blurAmount);
	}
	
	public float blurAmount
	{
		get {return _blurAmount;}
		set {if(_blurAmount != value) {_blurAmount = value; needsApply = true;}}
	}
}



public class FGlowShader : FShader
{
	private float _glowAmount;
	private Color _glowColor;
	
	public FGlowShader(float glowAmount,Color glowColor) : base("GlowShader", Shader.Find("Futile/Glow"))
	{
		_glowAmount = glowAmount;
		_glowColor = glowColor;
		needsApply = true;
	}
	
	override public void Apply(Material mat)
	{
		mat.SetFloat("_GlowAmount",_glowAmount);
		mat.SetColor("_GlowColor",_glowColor);
	}
	
	public float glowAmount
	{
		get {return _glowAmount;}
		set {if(_glowAmount != value) {_glowAmount = value; needsApply = true;}}
	}
	
	public Color glowColor
	{
		get {return _glowColor;}
		set {if(_glowColor != value) {_glowColor = value; needsApply = true;}}
	}
}




public class FTestShader : FShader
{
	public FTestShader() : base("TestShader", Shader.Find("Futile/Test"))
	{
		//needsApply = true;
	}
	
	override public void Apply(Material mat)
	{
		//mat.SetFloat("_BlurForce",_fParam0);
	}
	/*
	public float fParam0
	{
		get {return fParam0;}
		set {if(_fParam0 != value) {_fParam0 = value; needsApply = true;}}
	}
	*/
}


public class FHole0Shader : FShader
{
	private Texture _hole0Texture=null;
	private Vector2 _hole0Offset=Vector2.zero;
	public Texture hole0Texture
	{
		get {return _hole0Texture;}
	}
	
	public void SetHole0(FAtlasElement hole0Element,FAtlasElement toDigElement) {
		_hole0Texture=hole0Element.atlas.texture;
		_hole0Offset=hole0Element.uvTopLeft-toDigElement.uvTopLeft;
		needsApply = true;
	}
	
	public FHole0Shader() : base("Hole0Shader", Shader.Find("Futile/Hole0"))
	{
		needsApply = true;
	}
	
	override public void Apply(Material mat)
	{
		if (_hole0Texture!=null) {
			mat.SetTexture("_Hole0Tex",_hole0Texture);
			mat.SetTextureOffset("_Hole0Tex",_hole0Offset);
		}
	}
}




public class FAtlasHolesShader : FShader
{
	private Vector2 _hole0Offset=Vector2.zero;
	private Vector2 _hole1Offset=Vector2.zero;
	
	private FAtlasElement _elementHole0,_elementToDig0;
	public void SetHole0(FAtlasElement holeElement,FAtlasElement toDigElement) {
		_elementHole0=holeElement;
		_elementToDig0=toDigElement;
		_hole0Offset=_elementHole0.uvTopLeft-_elementToDig0.uvTopLeft;
		needsApply = true;
	}
	public void UpdateToDig0(FAtlasElement toDigElement) {
		if (_elementHole0!=null) {
			_elementToDig0=toDigElement;
			_hole0Offset=_elementHole0.uvTopLeft-_elementToDig0.uvTopLeft;
			needsApply = true;
		}
	}
	public void ResetHole0() {
		if (_elementHole0!=null) {
			_hole0Offset=Vector2.zero;
			needsApply = true;
		}
	}
	
	private FAtlasElement _elementHole1,_elementToDig1;
	public void SetHole1(FAtlasElement holeElement,FAtlasElement toDigElement) {
		_elementHole1=holeElement;
		_elementToDig1=toDigElement;
		_hole1Offset=_elementHole1.uvTopLeft-_elementToDig1.uvTopLeft;
		needsApply = true;
	}
	public void UpdateToDig1(FAtlasElement toDigElement) {
		if (_elementHole1!=null) {
			_elementToDig1=toDigElement;
			_hole1Offset=_elementHole1.uvTopLeft-_elementToDig1.uvTopLeft;
			needsApply = true;
		}
	}
	public void ResetHole1() {
		if (_elementHole1!=null) {
			_hole1Offset=Vector2.zero;
			needsApply = true;
		}
	}
	
	public FAtlasHolesShader() : base("AtlasHolesShader", Shader.Find("Futile/AtlasHoles"))
	{
		needsApply = true;
	}
	
	override public void Apply(Material mat)
	{
		/*
		if (_hole0Texture!=null) {
			mat.SetTexture("_Hole0Tex",_hole0Texture);
			//mat.SetTextureScale("_Hole0Tex",new Vector2(0.99f,0.99f));
			mat.SetTextureOffset("_Hole0Tex",_hole0Offset);
			//Texture textureTest=mat.GetTexture("_HoleTex");
		}
		*/
		mat.SetFloat("_Hole0OffU",_hole0Offset.x);
		mat.SetFloat("_Hole0OffV",_hole0Offset.y);
		mat.SetFloat("_Hole1OffU",_hole1Offset.x);
		mat.SetFloat("_Hole1OffV",_hole1Offset.y);
		
		//mat.SetTexture("_MainTex",_hole0Texture);
	}
	/*
	public float glowAmount
	{
		get {return _glowAmount;}
		set {if(_glowAmount != value) {_glowAmount = value; needsApply = true;}}
	}
	
	public Color glowColor
	{
		get {return _glowColor;}
		set {if(_glowColor != value) {_glowColor = value; needsApply = true;}}
	}
	*/
}





public class FSpriteHolesShader : FShader
{
	private Texture _hole0Texture=null;
	private Vector4 _hole0LocalToUVParams;
	private Vector4 _hole0MatrixABCD;
	private Vector4 _hole0MatrixTxTy;
	private Vector4 _hole0UVRect;
	private FSprite _hole0Sprite=null;
	
	static private Vector4 _screenParams; //,_screenSize;
	

	public Texture hole0Texture
	{
		get {return _hole0Texture;}
	}
	
	public void SetHole0(FSprite spriteHole) {
		_hole0Sprite=spriteHole;
		
		_hole0Texture=spriteHole.element.atlas.texture;
		
		spriteHole.UpdateMatrix();

		float factorX=Futile.resourceScale/spriteHole.element.atlas.textureSize.x;
		float factorY=Futile.resourceScale/spriteHole.element.atlas.textureSize.y;
		//do this if you have modified version of FElement that keeps the frame info
		_hole0LocalToUVParams=new Vector4( factorX , (spriteHole.element.frame.x+(-spriteHole.textureRect.x-spriteHole.element.sourceRect.x)*Futile.resourceScale)/spriteHole.element.atlas.textureSize.x , factorY , 1+(-(spriteHole.element.frame.y+spriteHole.element.frame.height)+(-spriteHole.textureRect.y-spriteHole.element.sourceRect.y)*Futile.resourceScale)/spriteHole.element.atlas.textureSize.y );
		//float elementFrameX=spriteHole.element.uvRect.x*spriteHole.element.atlas.textureSize.x;
		//float elementFrameY=-spriteHole.element.uvRect.y*spriteHole.element.atlas.textureSize.y+spriteHole.element.atlas.textureSize.y-spriteHole.element.uvRect.height*spriteHole.element.atlas.textureSize.y;
		//_hole0LocalToUVParams=new Vector4( factorX , (elementFrameX+(-spriteHole.textureRect.x-spriteHole.element.sourceRect.x)*Futile.resourceScale)/spriteHole.element.atlas.textureSize.x , factorY , 1+(-(elementFrameY+spriteHole.element.frame.height)+(-spriteHole.textureRect.y-spriteHole.element.sourceRect.y)*Futile.resourceScale)/spriteHole.element.atlas.textureSize.y );
		
		
		_hole0UVRect.x=spriteHole.element.uvRect.xMin;
		_hole0UVRect.y=spriteHole.element.uvRect.yMin;
		_hole0UVRect.z=spriteHole.element.uvRect.xMax;
		_hole0UVRect.w=spriteHole.element.uvRect.yMax;
		
		//test
		/* the old way of computing the uv in shader 
		_hole0MatrixABCD=new Vector4(spriteHole.screenInverseConcatenatedMatrix.a,spriteHole.screenInverseConcatenatedMatrix.b,spriteHole.screenInverseConcatenatedMatrix.c,spriteHole.screenInverseConcatenatedMatrix.d);
		_hole0MatrixTxTy=new Vector4(spriteHole.screenInverseConcatenatedMatrix.tx,spriteHole.screenInverseConcatenatedMatrix.ty,0,0);
		Vector2 vertex=new Vector2(spriteHole.x,spriteHole.y);
		//vertex.x+=100;
		Vector2 tmp=new Vector2 ( (vertex.x+_screenParams.x)*_screenParams.z , (vertex.y+_screenParams.y)*_screenParams.z ); 
	    //futile local sprite coords
    	Vector2 local=new Vector2  ( tmp.x*_hole0MatrixABCD.x + tmp.y*_hole0MatrixABCD.z + _hole0MatrixTxTy.x , tmp.x*_hole0MatrixABCD.y + tmp.y*_hole0MatrixABCD.w + _hole0MatrixTxTy.y );
		//uv
		Vector2 uv=new Vector2 (local.x*_hole0LocalToUVParams.x + _hole0LocalToUVParams.y , local.y*_hole0LocalToUVParams.z + _hole0LocalToUVParams.w );

		
		
		
		
		
		Vector2 uv2=new Vector2 (  
			
			//((vertex.x+_screenParams.x)*_screenParams.z*_hole0MatrixABCD.x + (vertex.y+_screenParams.y)*_screenParams.z*_hole0MatrixABCD.z + _hole0MatrixTxTy.x)*_hole0LocalToUVParams.x + _hole0LocalToUVParams.y , 
			
			vertex.x*_screenParams.z*_hole0MatrixABCD.x*_hole0LocalToUVParams.x    +    vertex.y*_screenParams.z*_hole0MatrixABCD.z*_hole0LocalToUVParams.x    +    (_screenParams.x*_screenParams.z*_hole0MatrixABCD.x + _screenParams.y*_screenParams.z*_hole0MatrixABCD.z + _hole0MatrixTxTy.x)*_hole0LocalToUVParams.x + _hole0LocalToUVParams.y,
			
			
			//((vertex.x+_screenParams.x)*_screenParams.z*_hole0MatrixABCD.y + (vertex.y+_screenParams.y)*_screenParams.z*_hole0MatrixABCD.w + _hole0MatrixTxTy.y)*_hole0LocalToUVParams.z + _hole0LocalToUVParams.w ,
			
			vertex.x*_screenParams.z*_hole0MatrixABCD.y*_hole0LocalToUVParams.z    +    vertex.y*_screenParams.z*_hole0MatrixABCD.w*_hole0LocalToUVParams.z    +    (_screenParams.x*_screenParams.z*_hole0MatrixABCD.y + _screenParams.y*_screenParams.z*_hole0MatrixABCD.w + _hole0MatrixTxTy.y)*_hole0LocalToUVParams.z + _hole0LocalToUVParams.w
			
			
			);
		/**/
		
		Hole0NeedUpdate();
		/*
		_hole0MatrixABCD.x=_screenParams.z*spriteHole.screenInverseConcatenatedMatrix.a*_hole0LocalToUVParams.x;
		_hole0MatrixABCD.z=_screenParams.z*spriteHole.screenInverseConcatenatedMatrix.c*_hole0LocalToUVParams.x;
		_hole0MatrixTxTy.x=(_screenParams.x*_screenParams.z*spriteHole.screenInverseConcatenatedMatrix.a + _screenParams.y*_screenParams.z*spriteHole.screenInverseConcatenatedMatrix.c + _hole0MatrixTxTy.x)*_hole0LocalToUVParams.x + _hole0LocalToUVParams.y;
			
		_hole0MatrixABCD.y=_screenParams.z*spriteHole.screenInverseConcatenatedMatrix.b*_hole0LocalToUVParams.z;
		_hole0MatrixABCD.w=_screenParams.z*spriteHole.screenInverseConcatenatedMatrix.d*_hole0LocalToUVParams.z;
		_hole0MatrixTxTy.y=(_screenParams.x*_screenParams.z*spriteHole.screenInverseConcatenatedMatrix.b + _screenParams.y*_screenParams.z*spriteHole.screenInverseConcatenatedMatrix.d + _hole0MatrixTxTy.y)*_hole0LocalToUVParams.z + _hole0LocalToUVParams.w;
		
		Vector2 uv3=new Vector2  ( _hole0Sprite.x*_hole0MatrixABCD.x + _hole0Sprite.y*_hole0MatrixABCD.z + _hole0MatrixTxTy.x , _hole0Sprite.x*_hole0MatrixABCD.y + _hole0Sprite.y*_hole0MatrixABCD.w + _hole0MatrixTxTy.y );
		*/
			
		needsApply = true;
	}
	
	private bool updateEveryFrameHole0=false;
	public void UpdateEveryFrameHole0(bool b) {
		if (b!=updateEveryFrameHole0) {
			updateEveryFrameHole0=b;
			if (updateEveryFrameHole0) {
				Futile.instance.SignalUpdate+=Hole0NeedUpdate;
			} else {
				Futile.instance.SignalUpdate-=Hole0NeedUpdate;
			}
		}
	}
	
	/* no need since we're using maric from vertex and not from screen coordinates
	public void ScreenNeedUpdate() {
		_screenParams=new Vector4(-Futile.screen.originX * Futile.screen.pixelWidth , -Futile.screen.originY * Futile.screen.pixelHeight , Futile.displayScaleInverse , 0);
		_screenSize=new Vector4(Futile.screen.pixelWidth , Futile.screen.pixelHeight , 0 , 0);
		
		if (_hole0Sprite!=null) Hole0NeedUpdate();
		
		needsApply = true;
	}
	*/
	
	public void Hole0NeedUpdate() {
		FSprite spriteHole=_hole0Sprite;
		
		
		/* 
		//matrix from screen coordinates
		_hole0MatrixABCD.x=_screenParams.z*spriteHole.screenInverseConcatenatedMatrix.a*_hole0LocalToUVParams.x;
		_hole0MatrixABCD.z=_screenParams.z*spriteHole.screenInverseConcatenatedMatrix.c*_hole0LocalToUVParams.x;
		_hole0MatrixTxTy.x=(_screenParams.x*_screenParams.z*spriteHole.screenInverseConcatenatedMatrix.a + _screenParams.y*_screenParams.z*spriteHole.screenInverseConcatenatedMatrix.c + spriteHole.screenInverseConcatenatedMatrix.tx)*_hole0LocalToUVParams.x + _hole0LocalToUVParams.y;
			
		_hole0MatrixABCD.y=_screenParams.z*spriteHole.screenInverseConcatenatedMatrix.b*_hole0LocalToUVParams.z;
		_hole0MatrixABCD.w=_screenParams.z*spriteHole.screenInverseConcatenatedMatrix.d*_hole0LocalToUVParams.z;
		_hole0MatrixTxTy.y=(_screenParams.x*_screenParams.z*spriteHole.screenInverseConcatenatedMatrix.b + _screenParams.y*_screenParams.z*spriteHole.screenInverseConcatenatedMatrix.d + spriteHole.screenInverseConcatenatedMatrix.ty)*_hole0LocalToUVParams.z + _hole0LocalToUVParams.w;
		*/
		
		//mattrix from vertices
		_hole0MatrixABCD.x=_screenParams.z*spriteHole.inverseConcatenatedMatrix.a*_hole0LocalToUVParams.x;
		_hole0MatrixABCD.z=_screenParams.z*spriteHole.inverseConcatenatedMatrix.c*_hole0LocalToUVParams.x;
		_hole0MatrixTxTy.x=(_screenParams.x*_screenParams.z*spriteHole.inverseConcatenatedMatrix.a + _screenParams.y*_screenParams.z*spriteHole.inverseConcatenatedMatrix.c + spriteHole.inverseConcatenatedMatrix.tx)*_hole0LocalToUVParams.x + _hole0LocalToUVParams.y;
			
		_hole0MatrixABCD.y=_screenParams.z*spriteHole.inverseConcatenatedMatrix.b*_hole0LocalToUVParams.z;
		_hole0MatrixABCD.w=_screenParams.z*spriteHole.inverseConcatenatedMatrix.d*_hole0LocalToUVParams.z;
		_hole0MatrixTxTy.y=(_screenParams.x*_screenParams.z*spriteHole.inverseConcatenatedMatrix.b + _screenParams.y*_screenParams.z*spriteHole.inverseConcatenatedMatrix.d + spriteHole.inverseConcatenatedMatrix.ty)*_hole0LocalToUVParams.z + _hole0LocalToUVParams.w;
		
		
		/* tests
		float sourceWidth = _hole0Sprite.element.sourceRect.width;
		float sourceHeight = _hole0Sprite.element.sourceRect.height;
		float left = _hole0Sprite.textureRect.x + _hole0Sprite.element.sourceRect.x;
		float bottom = _hole0Sprite.textureRect.y + (_hole0Sprite.textureRect.height - _hole0Sprite.element.sourceRect.y - _hole0Sprite.element.sourceRect.height);
		
		
		Vector2 localVertice = new Vector2(left,bottom + sourceHeight);
		
		Vector3 globalVertice=Vector3.zero;
		_hole0Sprite.concatenatedMatrix.ApplyVector3FromLocalVector2(ref globalVertice, localVertice,0);
		
		Vector2 elemUv=_hole0Sprite.element.uvTopLeft;
		
		Vector2 shaderUv=new Vector2  ( globalVertice.x*_hole0MatrixABCD.x + globalVertice.y*_hole0MatrixABCD.z + _hole0MatrixTxTy.x , globalVertice.x*_hole0MatrixABCD.y + globalVertice.y*_hole0MatrixABCD.w + _hole0MatrixTxTy.y );
		
		
		
		Vector4 hole0MatrixABCD=new Vector4(spriteHole.screenInverseConcatenatedMatrix.a,spriteHole.screenInverseConcatenatedMatrix.b,spriteHole.screenInverseConcatenatedMatrix.c,spriteHole.screenInverseConcatenatedMatrix.d);
		Vector4 hole0MatrixTxTy=new Vector4(spriteHole.screenInverseConcatenatedMatrix.tx,spriteHole.screenInverseConcatenatedMatrix.ty,0,0);
		Vector2 vertex=new Vector2(globalVertice.x,globalVertice.y);
		//vertex.x+=100;
		Vector2 tmp=new Vector2 ( (vertex.x+_screenParams.x)*_screenParams.z , (vertex.y+_screenParams.y)*_screenParams.z ); 
	    //futile local sprite coords
    	Vector2 local=new Vector2  ( tmp.x*hole0MatrixABCD.x + tmp.y*hole0MatrixABCD.z + hole0MatrixTxTy.x , tmp.x*hole0MatrixABCD.y + tmp.y*hole0MatrixABCD.w + hole0MatrixTxTy.y );
		//uv
		Vector2 uv=new Vector2 (local.x*_hole0LocalToUVParams.x + _hole0LocalToUVParams.y , local.y*_hole0LocalToUVParams.z + _hole0LocalToUVParams.w );
		 */
		
		
		
		
		//test
		//Vector2 uv=new Vector2  ( _hole0Sprite.x*_hole0MatrixABCD.x + _hole0Sprite.y*_hole0MatrixABCD.z + _hole0MatrixTxTy.x , _hole0Sprite.x*_hole0MatrixABCD.y + _hole0Sprite.y*_hole0MatrixABCD.w + _hole0MatrixTxTy.y );
		
		needsApply = true;
	}
	
	public FSpriteHolesShader() : base("SpriteHolesShader", Shader.Find("Futile/SpriteHoles"))
	{
		//ScreenNeedUpdate();
		//no fx screenParams
		_screenParams.x=_screenParams.y=0;
		_screenParams.z=1;
		_screenParams.w=0;
		needsApply=true;
	}
	
	override public void Apply(Material mat)
	{
		if (_hole0Texture!=null) {
			mat.SetTexture("_Hole0Tex",_hole0Texture);
			
			mat.SetVector("_Hole0MatrixABCD",_hole0MatrixABCD);
			mat.SetVector("_Hole0MatrixTxTy",_hole0MatrixTxTy);
			mat.SetVector("_Hole0UVRect",_hole0UVRect);
			
			//mat.SetVector("_Hole0LocalToUV",_hole0LocalToUVParams);
			
			//x=offsetX
			//y=offsetY
			//z=displayScaleInverse
			//w=unused
			//mat.SetVector("_screenParams",_screenParams );
			//mat.SetVector("_screenSize",_screenSize );
		}
	}
}