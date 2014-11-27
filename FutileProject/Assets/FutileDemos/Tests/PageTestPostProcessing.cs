using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Reflection;

public class PageTestPostProcessing : PageTest, FMultiTouchableInterface
{
	public PageTestPostProcessing()
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
		FSprite bg =new FSprite("colorgrid");
		bg.shader=FShader.AdditiveColor;
		bg.color=new Color(0.1f,0.1f,0.1f);
		bg.scale=2f;
		AddChild(bg);

		ShowTitle("Post processing effects\nClick anywhere");
		

		base.Start();
	}



	public void Action0(object o) {
		Vector2 pos=(Vector2)o;
		WaveExploPostProcessing.Get().StartIt(pos);
	}

	public void Action1(object o) {
		Vector2 pos=(Vector2)o;
		ImpactPostProcessing.Get().StartIt(pos);
	}

	public void Action2(object o) {
		Vector2 pos=(Vector2)o;
		DistortionPostProcessing.Get().StartIt(pos);
	}

	int _actionIndex=0;
	protected void Click(Vector2 touch) {
		Type delegateType = this.GetType();
		MethodInfo theMethod = delegateType.GetMethod("Action"+_actionIndex);
		if (theMethod==null) {
			_actionIndex=0;
			theMethod = delegateType.GetMethod("Action"+_actionIndex);
		}
		if (theMethod!=null) {
			object[] methodParams = new object[]{touch};
			theMethod.Invoke(this, methodParams);
			_actionIndex++;
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

