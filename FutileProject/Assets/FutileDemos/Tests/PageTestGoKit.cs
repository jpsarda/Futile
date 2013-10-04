using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class PageTestGoKit : PageTest, FMultiTouchableInterface
{
	public PageTestGoKit()
	{
	}

	override public void HandleAddedToStage()
	{
		Futile.touchManager.AddMultiTouchTarget(this);
		base.HandleAddedToStage();	
	}
	
	override public void HandleRemovedFromStage()
	{
		Futile.touchManager.RemoveMultiTouchTarget(this);
		base.HandleRemovedFromStage();
	}
	
	protected List<FSprite> _sprites;
	protected List<GoTween> _spriteAnims=null;
	override public void Start()
	{
		ShowTitle("GoKit Shake and Oscillate");
		
		_sprites=new List<FSprite>();
		_spriteAnims=new List<GoTween>();
		
		//Build the grid
		FSprite sprite=new FSprite("Monkey_0");
		float scale=0.25f;
		float width=sprite.textureRect.width*scale;
		float height=sprite.textureRect.height*scale;
		int rows=4;
		int columns=5;
		for(int i=0;i<columns;i++) {
			float x=-((float)(columns-1)*0.5f-(float)i)*width;
			for(int j=0;j<rows;j++) {
				float y=((float)(rows-1)*0.5f-(float)j)*height;
				sprite=new FSprite("Monkey_0");
				sprite.scale=scale;
				sprite.x=x; sprite.y=y;
				_sprites.Add(sprite);
				AddChild(sprite);
			}
		}

		
		base.Start();
		
		TempMessage("Click to start",2f);
	}
	
	protected GoTween StartRandomAnim(FSprite sprite) {
		GoTweenConfig config=new GoTweenConfig();
		
		int r=RXRandom.Int(8);
		if (r==0) {
			config.shakeFloatProp("x",RXRandom.Range(1f,10f)).shakeFloatProp("y",RXRandom.Range(1f,10f));
		} else if (r==1) {
			config.oscillateFloatProp("x",RXRandom.Range(1f,10f),RXRandom.Range(0.1f,3f)).oscillateFloatProp("y",RXRandom.Range(1f,10f),RXRandom.Range(0.1f,3f));
		} else if (r==2) {
			config.shakeFloatProp("rotation",RXRandom.Range(1f,20f));
		} else if (r==3) {
			config.oscillateFloatProp("rotation",RXRandom.Range(1f,20f),RXRandom.Range(0.1f,3f));
		} else if (r==4) {
			config.shakeFloatProp("scale",RXRandom.Range(0.05f,0.1f));
		} else if (r==5) {
			config.oscillateFloatProp("scale",RXRandom.Range(0.05f,0.1f),RXRandom.Range(0.1f,3f));
		} else if (r==6) {
			config.shakeColorProp("color",RandomUtils.RandomColor());
		} else if (r==7) {
			config.oscillateColorProp("color",RandomUtils.RandomColor(),RXRandom.Range(0.1f,3f));
		}
		
		config.easeType=RandomUtils.RandomEnum<GoEaseType>();
		while(config.easeType==GoEaseType.Punch) {
			config.easeType=RandomUtils.RandomEnum<GoEaseType>();
		}
		//config.easeType=GoEaseType.ElasticInOut;
		
		return Go.to (sprite,1f,config);
	}
	protected void Click(Vector2 touch) {
		if (_spriteAnims!=null) {
			foreach (GoTween tween in _spriteAnims) {
				tween.complete();
				tween.goTo(tween.totalElapsedTime);
				tween.destroy();
			}
			_spriteAnims.Clear();
		}
		foreach (FSprite sprite in _sprites) {
			_spriteAnims.Add(StartRandomAnim(sprite));
		}
	}
	
	public void HandleMultiTouch(FTouch[] touches)
	{
		foreach(FTouch touch in touches)
		{
			if (touch.phase == TouchPhase.Ended) {
				Click(touch.position);
			}
		}
	}
}

