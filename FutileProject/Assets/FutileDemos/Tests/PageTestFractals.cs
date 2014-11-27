using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class PageTestFractals : PageTest, FMultiTouchableInterface
{
	public PageTestFractals()
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
	
	
	override public void Start()
	{
		ShowTitle("Fractals fun\nClick to set random params");
		
		newOne();
		
		base.Start();


		FButton button=new FButton("Futile_White","Futile_White",null,null);
		button.SetColors(new Color(0f,0f,1f),new Color(0f,0f,0.5f));
		button.AddLabel(Config.fontFile,"New tree",Color.white);
		button.scaleX=8f;
		button.label.scaleX=0.5f/button.scaleX;
		button.scaleY=2f;
		button.label.scaleY=0.5f/button.scaleY;

		
		button.x=Futile.screen.halfWidth-button.hitRect.width*button.scaleX*0.5f;
		button.y=Futile.screen.halfHeight-button.hitRect.height*button.scaleY*0.5f;
		this.AddChild(button);
		
		button.SignalRelease+=HandleNewButtonRelease;
	}

	protected void HandleNewButtonRelease(FButton b) {
		newOne();
	}

	protected FractalElement _f=null;
	protected void newOne() {
		if (_f!=null) _f.RemoveFromContainer();
		_f = new FractalElement(40f,true,4,Futile.atlasManager.GetElementWithName("psmoke0"),3f);
		Color c=RandomUtils.RandomColor();
		c.a=RXRandom.Range(0.25f,0.75f);
		_f.SetColor(c);
		AddChild(_f);
	}
	
	protected void Click(Vector2 touch) {
		if (_f==null) {
			newOne();
		} else {
			if (_f!=null) {
				Color c=RandomUtils.RandomColor();
				c.a=RXRandom.Range(0.25f,0.75f);
				_f.SetColor(c);
				_f.SetRandomParams();
			}
		}
		/*
	    GoTweenConfig config;
		//config=new TweenConfig().colorProp("bottomColor",RandomUtils.RandomColor()).colorProp("topColor",RandomUtils.RandomColor());
	    //Go.to (_bicolorSprite0,0.25f,config);
		config=new GoTweenConfig().colorProp("bottomColor",RandomUtils.RandomColor()).colorProp("topColor",RandomUtils.RandomColor());
	    Go.to (_bicolorSprite1,0.25f,config);
	    */
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

