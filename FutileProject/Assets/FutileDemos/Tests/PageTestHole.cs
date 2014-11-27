using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class PageTestHole : PageTest, FMultiTouchableInterface
{
	public PageTestHole()
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

		base.Start();



		for (int i=0;i<1;i++) {
			FSprite _holeBg=new FSprite("colorgrid");
			_holeBg.scale=0.66f;
			_holeBg.color=Color.red;
			_holeBg.y=0;
			_holeBg.x=0;
			AddChild(_holeBg);
			
			GoTweenConfig config=new GoTweenConfig().oscillateFloatProp("rotation",90,1.5f,false,1);
			Go.to(_holeBg,1000000,config);

			FSprite _hole=new FSprite("sun"); _hole.scaleX=2f; _hole.scaleY=2f;
			_hole.alpha=0.0f;
			_hole.y=0;
			_hole.x=_holeBg.x;
			AddChild(_hole);
			FSpriteHolesShader shader=new FSpriteHolesShader();
			shader.SetHole0(_hole);
			
			shader.UpdateEveryFrameHole0(true);
			config=new GoTweenConfig().oscillateFloatProp("y",120,5.5f,false,1);
			Go.to(_hole,1000000,config);
			config=new GoTweenConfig().oscillateFloatProp("rotation",180,10.5f,false,1);
			Go.to(_hole,1000000,config);
			_holeBg.shader=shader;
			
		}

		ShowTitle("Holes & masking with shaders");
	}
	public void HandleMultiTouch(FTouch[] touches)
	{
		foreach(FTouch touch in touches)
		{
			if (touch.phase == TouchPhase.Ended) {
				//Click(touch.position);
			}
		}
	}

}

