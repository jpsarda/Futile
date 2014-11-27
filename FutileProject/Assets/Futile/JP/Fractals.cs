using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;


/*
FractalElement e = new FractalElement(60f,true,4);
Color c=RandomUtils.RandomColor();
c.a=RXRandom.Range(0.5f,1f);
e.SetColor(c);
//e.SetRandomParams();
AddChild(e);
*/

public class FractalElement : FContainer
{
	protected List<FractalElement> _elements;
	protected float _size;
	protected float _spriteScaleRatio;
	
	public float angle;
	public float radius,dynamicRadius;
	public FSprite sprite;
	public float rotationSpeed;
	public float radiusRotationSpeed,radiusRotation;
	public float elementsRotationSpeed;
	
	protected int _maxChaincount;
	protected bool _root;

	FAtlasElement _atlasElement;
		
	public FractalElement(float size,bool root,int maxChainCount,FAtlasElement atlasElement=null,float spriteScaleRatio=1f)
	{
		_size=size;
		_root=root;
		_spriteScaleRatio=spriteScaleRatio;
		_maxChaincount=maxChainCount;
		_atlasElement=atlasElement;
		if (_atlasElement==null) {
			_atlasElement=Futile.atlasManager.GetElementWithName("Futile_White");
		}
		_elements=new List<FractalElement>();
		
		Build();
	}
	
	public override void HandleRemovedFromStage ()
	{
		if (_root) {
			Futile.instance.SignalUpdate+=Loop;
		}
		base.HandleRemovedFromStage ();
	}
	public override void HandleAddedToStage ()
	{
		if (_root) {
			Futile.instance.SignalUpdate+=Loop;
		}
		base.HandleAddedToStage ();
	}
	
	public void Build() {
		sprite=new FSprite(_atlasElement);
		//sprite=new FSprite("Banana");
		//sprite=new FSprite("pskystar");
		//sprite=new FSprite("pspiral");
		//sprite=new FSprite("pbubble");
		//sprite=new FSprite("pcircle");
		//sprite=new FSprite("pmoonglow");
		//sprite=new FSprite("pstar");
		//sprite=new FSprite("soleil_aura");
		//sprite=new FSprite("pcircles");
		//sprite=new FSprite("pleinne_lune_aura");
		AddChild(sprite);
		sprite.rotation=RXRandom.Float(360f);

		/*
		rotationSpeed=RXRandom.Range(-100f,100f);
		//radiusRotationSpeed=RXRandom.Range(-50f,50f);
		radiusRotationSpeed=RXRandom.Range(-10f,10f);
		elementsRotationSpeed=RXRandom.Range(-10f,10f);
		
		sprite.scale=_size*0.95f/sprite.textureRect.width;
		*/

		FSpriteTrail<SpriteCursor2D> trail=new FSpriteTrail<SpriteCursor2D>(sprite,new List<int>() {1,2,3,5,7,9,11,14,17},true);
		trail.FadeIn(0.5f);

		if (_maxChaincount>0) {
			int count;
			if (_root) {
				count=RXRandom.Range(2,6);
			} else {
				count=RXRandom.Range(0,8);
			}
			for (int i=0;i<count;i++) {
				FractalElement element=new FractalElement(_size*RXRandom.Range(0.55f,0.85f),false,_maxChaincount-1,_atlasElement,_spriteScaleRatio);
				AddChild(element);
				element.angle=RXRandom.Float(Mathf.PI*2);
				element.dynamicRadius=element.radius=_size;
				element.UpdatePos();
				_elements.Add(element);
			}
		}
		
		SetRandomParams();
		
	}
	
	const float ColorOffset=0.3f;
	public void SetColor(Color color) {
		sprite.color=color;
		foreach (FractalElement element in _elements) {
			element.SetColor(new Color(color.r+RXRandom.Range(-ColorOffset,ColorOffset),color.g+RXRandom.Range(-ColorOffset,ColorOffset),color.b+RXRandom.Range(-ColorOffset,ColorOffset),color.a+RXRandom.Range(-ColorOffset,ColorOffset)));
		}
	}

	public void SetRandomParams() {
		rotationSpeed=RXRandom.Range(-100f,100f);
		if (RXRandom.Float()<0.5f) {
			radiusRotationSpeed=RXRandom.Range(-10f,10f);
		} else {
			radiusRotationSpeed=RXRandom.Range(-40f,40f);
		}

		elementsRotationSpeed=RXRandom.Range(-10f,10f);
		sprite.scale=_spriteScaleRatio*_size*0.95f/sprite.textureRect.width;
		foreach (FractalElement element in _elements) {
			element.SetRandomParams();
		}

		if (_root) {
			globalSpeefactor=RXRandom.Range(0.2f,0.8f);
			if (RXRandom.Float()<0.5f) {
				jumpFactor=RXRandom.Range(0.0f,0.1f);
			} else {
				jumpFactor=RXRandom.Range(0.0f,0.8f);
			}
		}
	}
	
	public void UpdatePos() {
		x=Mathf.Cos(angle)*dynamicRadius;
		y=Mathf.Sin(angle)*dynamicRadius;
	}
	
	static float globalSpeefactor=0.5f;
	static float jumpFactor=0.2f;
	public void Loop() {
		radiusRotation+=radiusRotationSpeed*Time.deltaTime*globalSpeefactor;
		dynamicRadius=radius*(1f+jumpFactor*Mathf.Sin(radiusRotation));
		sprite.rotation+=rotationSpeed*Time.deltaTime*globalSpeefactor;
		foreach (FractalElement element in _elements) {
			element.angle+=Time.deltaTime*elementsRotationSpeed*globalSpeefactor;
			element.UpdatePos();
			element.Loop();
		}
	}
}


