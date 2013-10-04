using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class PageTestLabelAnimate : PageTest, FMultiTouchableInterface
{
	public PageTestLabelAnimate()
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
	
	public void MyMethodNameWithData(object data) {
		Debug.Log("FButton clicked in speach bubble MyMethodNameWithData : "+data);
	}
	
	protected FPseudoHtmlText _text;
	override public void Start()
	{
		ShowTitle("Animated texts");
		
		_text=new FPseudoHtmlText(Config.fontFile,"<style text-color='#11FF11'><style text-scale='0.5'>Animated texts</style><br/><style text-scale='0.3'>FPseudoHtmlText and FLabelAnimate implement startVisibleCharIdx and endVisibleCharidx properties. They can be animated very simply with GoKit. How cool is that? <style text-color='#FF99FF'><br/><fsprite width='50' src='Monkey_0'/> It works with FSprites and FButtons of course, that's pretty cool! <fbutton src='YellowButton_normal' label='FButtons' scale='0.5' label-scale='0.5' color-down='#FF0000' action='MyMethodNameWithData' data='mybutttonid'/></style></style>",Config.textParams,Futile.screen.width-20f,PseudoHtmlTextAlign.left,1f,this);		
		AddChild(_text);
		_text.endVisibleCharIdx=0;
		
		base.Start();
		
		TempMessage("Click to toggle text",2f);
	}
	
	int step=0;
	protected void Click(Vector2 touch) {
		Go.killAllTweensWithTarget(_text);
		if (step==0) {
			_text.startVisibleCharIdx=0;
	    	GoTweenConfig config=new GoTweenConfig().intProp("endVisibleCharIdx",_text.charCount,false);
			Go.to(_text,2f,config);
		} else if (step==1) {
			_text.endVisibleCharIdx=_text.charCount;
	    	GoTweenConfig config=new GoTweenConfig().intProp("startVisibleCharIdx",_text.charCount,false);
			Go.to(_text,2f,config);
		} else if (step==2) {
			_text.endVisibleCharIdx=_text.charCount;
	    	GoTweenConfig config=new GoTweenConfig().intProp("startVisibleCharIdx",0,false);
			Go.to(_text,2f,config);
		} else { //step==3
			_text.startVisibleCharIdx=0;
	    	GoTweenConfig config=new GoTweenConfig().intProp("endVisibleCharIdx",0,false);
			Go.to(_text,2f,config);
		}
		step++; if (step>=4) step=0;
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

