using UnityEngine;
using System;
using System.Collections.Generic;

public class FxUtils {
	public FxUtils () {
	}
	static public void Flash (FSprite sprite) {
		Flash (sprite,Color.white);
	}
	static public void Flash (FSprite sprite,Color color) {
		FlashUtil util=new FlashUtil();
		util.go(sprite,color);
	}
	static public void Oscil (FNode node, float period, float ampX, float ampY) {
		OscilUtil util=new OscilUtil();
		util.go(node,period,ampX,ampY);
	}
	static public void OscilScale (FNode node, float period, float scaleX, float scaleY) {
		OscilScaleUtil util=new OscilScaleUtil();
		util.go(node,period,scaleX,scaleY);
	}
	static public void OscilColor (FSprite sprite, float period, Color toColor) {
		OscilColorUtil util=new OscilColorUtil();
		util.go(sprite,period,toColor);
	}
	static public void OscilAlpha (FSprite sprite, float period, float toAlpha) {
		OscilAlphaUtil util=new OscilAlphaUtil();
		util.go(sprite,period,toAlpha);
	}
	static public void Bump (FNode node, float period, float scaleRatio) {
		BumpUtil util=new BumpUtil();
		util.go(node,period,scaleRatio);
	}
	static public void Bump (FNode node) {
		Bump (node,0.1f,1.1f);
	}
	static public void FadeIn(FNode node,float duration) {
		FadeTo(node,1f,duration,false);
	}
	static public void FadeOut(FNode node,float duration,bool autoRemove) {
		FadeTo(node,0f,duration,autoRemove);
	}
	static public void FadeTo(FNode node, float alpha,float duration,bool autoRemove) {
		GoTweenConfig config=new GoTweenConfig().floatProp("alpha",alpha);
		if (autoRemove) {
			config.onComplete(FxHelper.Instance.RemoveFromContainer);
		}
		Go.to(node,duration,config);
	}
	static public void FadeColorTo(FNode node, Color color,float duration,bool autoRemove) {
		GoTweenConfig config=new GoTweenConfig().colorProp("color",color);
		if (autoRemove) {
			config.onComplete(FxHelper.Instance.RemoveFromContainer);
		}
		Go.to(node,duration,config);
	}
	
	static public void CancelActions(FSprite node) {
		FlashUtil.Cancel(node);
		OscilScaleUtil.Cancel(node);
		BumpUtil.Cancel(node);
		
		OscilColorUtil.Cancel(node);
		OscilAlphaUtil.Cancel(node);
		OscilUtil.Cancel(node);
	}
	static public void CancelActions(FNode node) {
		OscilUtil.Cancel(node);
		OscilScaleUtil.Cancel(node);
		BumpUtil.Cancel(node);
	}
}


public class FlashUtil {
	static Dictionary<FNode, GoTween> tweens = new Dictionary<FNode, GoTween>();
	
	public FlashUtil () {
	}

	public void go(FSprite sprite,Color color) {
		Cancel (sprite);
		sprite.shader=FShader.AdditiveColor;
		sprite.color=color;
		GoTweenConfig config0=new GoTweenConfig().colorProp("color",new Color(0f,0f,0f)).onComplete(HandleFlashDone);
		config0.setEaseType(GoEaseType.ExpoOut);
		tweens.Add(sprite,Go.to (sprite, 0.5f, config0));
	}
	protected void HandleFlashDone(AbstractGoTween tween) {
		FSprite sprite=(FSprite)(((GoTween)tween).target);
		tweens.Remove(sprite);
		sprite.shader=FShader.Basic;
		sprite.color=Futile.white;
		sprite.alpha=0.9f; sprite.alpha=1.0f; //alpha dirty
	}
	static public void Cancel (FSprite sprite) {
		GoTween tween=null;
		tweens.TryGetValue(sprite, out tween);
		if (tween!=null) {
			tweens.Remove(sprite);
			sprite.shader=FShader.Basic;
			sprite.color=Futile.white;
			sprite.alpha=0.9f; sprite.alpha=1.0f;  //alpha dirty
			tween.destroy();
		}
	}
}



public class OscilUtil {
	static Dictionary<FNode, OscilUtil> pendings = new Dictionary<FNode, OscilUtil>();
	
	public Vector2 memoPos;
	public GoTweenChain chain=null;

	public OscilUtil () {
	}

	public void go(FNode node, float period, float ampX, float ampY) {
		Cancel (node);
		
		chain = new GoTweenChain(new GoTweenCollectionConfig().setIterations(-1,GoLoopType.PingPong));
		chain.autoRemoveOnComplete=true;
		//chain.setIterations( -1, LoopType.PingPong );
		GoTweenConfig config0=new GoTweenConfig();
		
		if (ampX!=0) config0.floatProp( "x", node.x+ampX );
		if (ampY!=0) config0.floatProp( "y", node.y+ampY );
		
		config0.setEaseType(GoEaseType.SineInOut);
		
		chain.append( new GoTween( node, period, config0 ) );
		chain.play();
		
		pendings.Add(node,this);
	}
	static public void Cancel (FNode node) {
		OscilUtil instance=null;
		pendings.TryGetValue(node, out instance);
		if (instance!=null) {
			instance.chain.destroy();
			node.SetPosition(instance.memoPos);
			pendings.Remove(node);
		}
	}
}




public class OscilScaleUtil {
	static Dictionary<FNode, OscilScaleUtil> pendings = new Dictionary<FNode, OscilScaleUtil>();
	
	public float memoScaleX,memoScaleY;
	public GoTweenChain chain=null;

	public OscilScaleUtil () {
	}

	public void go(FNode node, float period, float scaleX, float scaleY) {
		Cancel (node);
		
		chain = new GoTweenChain(new GoTweenCollectionConfig().setIterations(-1,GoLoopType.PingPong));
		chain.autoRemoveOnComplete=true;
		//chain.setIterations( -1, LoopType.PingPong );
		GoTweenConfig config0=new GoTweenConfig();
		
		if (scaleX!=node.scaleX) config0.floatProp( "scaleX", scaleX );
		if (scaleY!=node.scaleY) config0.floatProp( "scaleY", scaleY );
		
		config0.setEaseType(GoEaseType.SineInOut);
		
		chain.append( new GoTween( node, period, config0 ) );
		chain.play();
		
		memoScaleX=node.scaleX;
		memoScaleY=node.scaleY;
		pendings.Add(node,this);
	}
	static public void Cancel (FNode node) {
		OscilScaleUtil instance=null;
		pendings.TryGetValue(node, out instance);
		if (instance!=null) {
			instance.chain.destroy();
			node.scaleX=instance.memoScaleX;
			node.scaleY=instance.memoScaleX;
			pendings.Remove(node);
		}
	}
}



public class OscilColorUtil {
	static Dictionary<FNode, OscilColorUtil> pendings = new Dictionary<FNode, OscilColorUtil>();
	
	public Color memoColor;
	public GoTweenChain chain=null;

	public OscilColorUtil () {
	}

	public void go(FSprite node, float period, Color toColor  ) {
		Cancel (node);
		
		//Debug.Log ("OscilColorUtil node="+node);
		chain = new GoTweenChain(new GoTweenCollectionConfig().setIterations(-1,GoLoopType.PingPong));
		chain.autoRemoveOnComplete=true;
		//chain.setIterations( -1, LoopType.PingPong );
		GoTweenConfig config0=new GoTweenConfig();
		config0.colorProp("color",toColor);
		config0.setEaseType(GoEaseType.SineInOut);
		
		chain.append( new GoTween( node, period, config0 ) );
		chain.play();
		
		memoColor=node.color;
		pendings.Add(node,this);
	}
	static public void Cancel (FSprite node) {
		OscilColorUtil instance=null;
		pendings.TryGetValue(node, out instance);
		if (instance!=null) {
			instance.chain.destroy();
			node.color=instance.memoColor;
			pendings.Remove(node);
		}
	}
}



public class OscilAlphaUtil {
	static Dictionary<FNode, OscilAlphaUtil> pendings = new Dictionary<FNode, OscilAlphaUtil>();
	
	public float memoAlpha;
	public GoTweenChain chain=null;
	
	public OscilAlphaUtil () {
	}

	public void go(FSprite node, float period, float toAlpha  ) {
		Cancel (node);
		
		chain = new GoTweenChain(new GoTweenCollectionConfig().setIterations(-1,GoLoopType.PingPong));
		chain.autoRemoveOnComplete=true;
		//chain.setIterations( -1, LoopType.PingPong );
		GoTweenConfig config0=new GoTweenConfig();
		config0.floatProp("alpha",toAlpha);
		config0.setEaseType(GoEaseType.SineInOut);
		
		chain.append( new GoTween( node, period, config0 ) );
		chain.play();
		
		memoAlpha=node.alpha;
		pendings.Add(node,this);
	}
	static public void Cancel (FSprite node) {
		OscilAlphaUtil instance=null;
		pendings.TryGetValue(node, out instance);
		if (instance!=null) {
			instance.chain.destroy();
			node.alpha=instance.memoAlpha;
			pendings.Remove(node);
		}
	}
}



public class BumpUtil {
	static Dictionary<FNode, BumpUtil> pendings = new Dictionary<FNode, BumpUtil>();
	
	public float memoScaleX,memoScaleY;
	public GoTween tween=null;
	
	public BumpUtil () {
	}

	public void go(FNode node,float duration,float scaleRatio) {
		Cancel (node);
		
		memoScaleX=node.scaleX;
		memoScaleY=node.scaleY;
		pendings.Add(node,this);
		
		GoTweenConfig config0;
		if (node.scaleX==node.scaleY) {
			config0=new GoTweenConfig().floatProp("scale",node.scale).onComplete(HandleDone);
			node.scale*=scaleRatio;
		} else {
			config0=new GoTweenConfig().floatProp("scaleX",node.scaleX).floatProp("scaleY",node.scaleY).onComplete(HandleDone);
			node.scaleX*=scaleRatio;
			node.scaleY*=scaleRatio;
		}
		config0.setEaseType(GoEaseType.Linear);
		tween=Go.to (node, duration, config0);
	}
	protected void HandleDone(AbstractGoTween tween) {
		FNode node=(FNode)(((GoTween)tween).target);
		//BumpUtil instance=null;
		//pendings.TryGetValue(node, out instance);
		pendings.Remove(node);
	}
	static public void Cancel (FNode node) {
		BumpUtil instance=null;
		pendings.TryGetValue(node, out instance);
		if (instance!=null) {
			instance.tween.destroy();
			node.scaleX=instance.memoScaleX;
			node.scaleY=instance.memoScaleY;
			pendings.Remove(node);
		}
	}
}




public class ShakeUtil {
	static Dictionary<FNode, ShakeUtil> pendings = new Dictionary<FNode, ShakeUtil>();
	
	public Vector2 oPosition;
	public float duration,amplitude;
	public FNode node=null;
	
	public float curDuration,curAmplitude;
	
	public ShakeUtil () {
	}
	public void go(FNode node_,float duration_,float amplitude_) {
		Cancel(node_);

		oPosition=node_.GetPosition();
		//Debug.Log("oPosition="+oPosition);
		curDuration=duration=duration_;
		curAmplitude=amplitude=amplitude_;
		pendings.Add(node_,this);
		if (node==null) {
			Futile.instance.SignalUpdate+=HandleUpdate;
		}
		node=node_;
	}
	protected void HandleUpdate() {
		curDuration-=Time.deltaTime;
		if (curDuration<0) {
			Stop();
		} else {
			curAmplitude=amplitude*curDuration/duration;
			node.x=oPosition.x+RXRandom.Range(-curAmplitude,curAmplitude);
			node.y=oPosition.y+RXRandom.Range(-curAmplitude,curAmplitude);
		}
	}
	protected void Stop() {
		if (node!=null) {
			Futile.instance.SignalUpdate-=HandleUpdate;
			node.SetPosition(oPosition);
			pendings.Remove(node);
			node=null;
			curDuration=-1f;
		}
	}
	static public void Go(FNode node_,float duration_,float amplitude_) {
		(new ShakeUtil()).go (node_,duration_,amplitude_);
	}
	static public void Cancel(FNode node) {
		ShakeUtil obj=null;
		pendings.TryGetValue(node, out obj);
		if (obj!=null) {
			obj.Stop();
		}
	}
}



public class CenteredOscilUtil {
	static Dictionary<FNode, CenteredOscilUtil> pendings = new Dictionary<FNode, CenteredOscilUtil>();
	
	public Vector2 oPosition;
	public float duration,amplitudeX,amplitudeY,periodX,periodY;
	public FNode node=null;
	
	public float curDuration,curAmplitudeX,curAmplitudeY,curDelay;
	
	public CenteredOscilUtil () {
	}
	public void go(FNode node_,float duration_,float amplitudeX_,float periodX_,float amplitudeY_,float periodY_) {
		go (node_,duration_,amplitudeX_,periodX_,amplitudeY_,periodY_,-1f);
	}
	public void go(FNode node_,float duration_,float amplitudeX_,float periodX_,float amplitudeY_,float periodY_,float delay_) {
		Cancel(node_);

		oPosition=node_.GetPosition();
		//Debug.Log("oPosition="+oPosition);
		curDuration=duration=duration_;
		curAmplitudeX=amplitudeX=amplitudeX_;
		curAmplitudeY=amplitudeY=amplitudeY_;
		periodX=periodX_;
		periodY=periodY_;
		curDelay=delay_;
		pendings.Add(node_,this);
		if (node==null) {
			Futile.instance.SignalUpdate+=HandleUpdate;
		}
		node=node_;
	}
	protected void HandleUpdate() {
		if (curDelay>0f) {
			curDelay-=Time.deltaTime;
			return;
		}
		curDuration-=Time.deltaTime;
		if (curDuration<0) {
			Stop();
		} else {
			curAmplitudeX=amplitudeX*curDuration/duration;
			curAmplitudeY=amplitudeY*curDuration/duration;
			node.x=oPosition.x+curAmplitudeX*(float)Math.Sin((duration-curDuration)*2*Math.PI/periodX);
			node.y=oPosition.y+curAmplitudeY*(float)Math.Sin((duration-curDuration)*2*Math.PI/periodY);
		}
	}
	protected void Stop() {
		if (node!=null) {
			Futile.instance.SignalUpdate-=HandleUpdate;
			node.SetPosition(oPosition);
			pendings.Remove(node);
			node=null;
			curDuration=-1f;
		}
	}
	static public void Go(FNode node_,float duration_,float amplitudeX_,float periodX_,float amplitudeY_,float periodY_) {
		(new CenteredOscilUtil()).go (node_,duration_,amplitudeX_,periodX_,amplitudeY_,periodY_);
	}
	static public void Go(FNode node_,float duration_,float amplitudeX_,float periodX_,float amplitudeY_,float periodY_,float delay_) {
		(new CenteredOscilUtil()).go (node_,duration_,amplitudeX_,periodX_,amplitudeY_,periodY_,delay_);
	}
	static public void Cancel(FNode node) {
		CenteredOscilUtil obj=null;
		pendings.TryGetValue(node, out obj);
		if (obj!=null) {
			obj.Stop();
		}
	}
}







public class OscilRotationUtil {
	static Dictionary<FNode, OscilRotationUtil> pendings = new Dictionary<FNode, OscilRotationUtil>();
	
	public float memoRotation;
	//public GoTweenChain chain=null;
	public GoTween tween=null;

	public OscilRotationUtil () {
	}

	public void go(FNode node, float period, float toRotation, float duration) {
		Cancel (node);

		/*
		chain = new GoTweenChain(new GoTweenCollectionConfig().setIterations(-1,GoLoopType.PingPong));
		chain.autoRemoveOnComplete=true;
		//chain.setIterations( -1, LoopType.PingPong );
		GoTweenConfig config0=new GoTweenConfig();
		
		if (scaleX!=node.scaleX) config0.floatProp( "scaleX", scaleX );
		if (scaleY!=node.scaleY) config0.floatProp( "scaleY", scaleY );
		
		config0.setEaseType(GoEaseType.SineInOut);
		
		chain.append( new GoTween( node, period, config0 ) );
		chain.play();
		*/
		
		GoTweenConfig config=new GoTweenConfig().oscillateFloatProp("rotation",toRotation,period,false,1);
		tween=Go.to(node,duration,config);
		
		memoRotation=node.rotation;
		pendings.Add(node,this);
	}
	static public void Cancel (FNode node) {
		OscilRotationUtil instance=null;
		pendings.TryGetValue(node, out instance);
		if (instance!=null) {
			//instance.chain.destroy();
			instance.tween.destroy();
			node.rotation=instance.memoRotation;
			pendings.Remove(node);
		}
	}
}







public class SwitchElementUtil {
	static Dictionary<FNode, SwitchElementUtil> pendings = new Dictionary<FNode, SwitchElementUtil>();
	
	protected FAtlasElement _switchToElment=null;
	
	protected GoTweenChain _chain=null;
	
	protected FSprite _sprite=null;
	protected float _scaleY;
	
	public SwitchElementUtil () {
	}
	public void go(FSprite sprite_,FAtlasElement element,float duration_) {
		Cancel(sprite_);

		_switchToElment=element;
		_sprite=sprite_;
		_scaleY=_sprite.scaleY;
		
		pendings.Add(_sprite,this);
		
		_chain=new GoTweenChain(new GoTweenCollectionConfig().setIterations(1,GoLoopType.PingPong));
		_chain.autoRemoveOnComplete=true;
		
		GoTweenConfig config;
		
		config=new GoTweenConfig().floatProp("scaleY",0f).onComplete(HandleSwitchElement);
		config.easeType=GoEaseType.ExpoOut;

		_chain.append(Go.to(sprite_,duration_*0.5f,config));
		
		config=new GoTweenConfig().floatProp("scaleY",_scaleY).onComplete(HandleEndAnim);
		config.easeType=GoEaseType.ExpoOut;
		_chain.append(Go.to(sprite_,duration_*0.5f,config));
		
		_chain.play();
	}
	protected void HandleSwitchElement(AbstractGoTween tween) {
		_sprite.element=_switchToElment;
	}
	protected void HandleEndAnim(AbstractGoTween tween) {
		Stop ();
	}
	protected void Stop() {
		if (_sprite!=null) {
			_sprite.element=_switchToElment;
			_sprite.scaleY=_scaleY;
			pendings.Remove(_sprite);
			_chain.destroy();
			_chain=null;
			_sprite=null;
		}
	}
	static public void Animate(FSprite sprite_,FAtlasElement element,float duration_) {
		(new SwitchElementUtil()).go ( sprite_, element, duration_);
	}
	static public void Cancel(FSprite sprite) {
		SwitchElementUtil obj=null;
		pendings.TryGetValue(sprite, out obj);
		if (obj!=null) {
			obj.Stop();
		}
	}
}








public class ScaleTransitionUtil {
	static Dictionary<FNode, ScaleTransitionUtil> pendings = new Dictionary<FNode, ScaleTransitionUtil>();
	
	protected FAtlasElement _switchToElment=null;
	
	protected GoTweenChain _chain=null;
	
	System.Action<FNode> _action;
	
	protected FNode _node=null;
	protected float _scaleY;
	protected bool _actionCalled=false;
	
	public ScaleTransitionUtil () {
	}
	public void go(FNode node,System.Action<FNode> action,float duration_) {
		Cancel(node);

		_action=action;
		_node=node;
		_scaleY=node.scaleY;
		
		pendings.Add(_node,this);
		
		_chain=new GoTweenChain(new GoTweenCollectionConfig().setIterations(1,GoLoopType.PingPong));
		_chain.autoRemoveOnComplete=true;
		
		GoTweenConfig config;
		
		config=new GoTweenConfig().floatProp("scaleY",0f).onComplete(HandleMidAnim);
		config.easeType=GoEaseType.ExpoOut;

		_chain.append(Go.to(_node,duration_*0.5f,config));
		
		config=new GoTweenConfig().floatProp("scaleY",_scaleY).onComplete(HandleEndAnim);
		config.easeType=GoEaseType.ExpoOut;
		_chain.append(Go.to(_node,duration_*0.5f,config));
		
		_chain.play();
	}
	protected void HandleMidAnim(AbstractGoTween tween) {
		if (!_actionCalled) {
			_actionCalled=true;
			_action(_node);
		}
	}
	protected void HandleEndAnim(AbstractGoTween tween) {
		Stop ();
	}
	protected void Stop() {
		if (_node!=null) {
			HandleMidAnim(null);
			_node.scaleY=_scaleY;
			pendings.Remove(_node);
			_chain.destroy();
			_chain=null;
			_node=null;
		}
	}
	static public void Animate(FNode node,System.Action<FNode> action,float duration_) {
		(new ScaleTransitionUtil()).go ( node, action, duration_);
	}
	static public void Cancel(FNode node) {
		ScaleTransitionUtil obj=null;
		pendings.TryGetValue(node, out obj);
		if (obj!=null) {
			obj.Stop();
		}
	}
}







public class BlinkUtil {
	static Dictionary<FNode, BlinkUtil> pendings = new Dictionary<FNode, BlinkUtil>();
	
	public bool oVisible;
	public float duration,period,visibleRatio;
	public FNode node=null;
	
	public float curDuration,nextToggle;
	
	public BlinkUtil () {
	}
	public void go(FNode node_,float duration_,float period_,float visibleRatio_) {
		Cancel(node_);

		oVisible=node_.isVisible;
		//Debug.Log("oPosition="+oPosition);
		curDuration=duration=duration_;
		period=period_;
		visibleRatio=visibleRatio_;
		if (oVisible) {
			nextToggle=period*visibleRatio;
		} else {
			nextToggle=period*(1f-visibleRatio);
		}
		pendings.Add(node_,this);
		if (node==null) {
			Futile.instance.SignalUpdate+=HandleUpdate;
		}
		node=node_;
	}
	protected void HandleUpdate() {
		curDuration-=Time.deltaTime;
		nextToggle-=Time.deltaTime;
		if (curDuration<=0) {
			Stop();
		} else {
			if (nextToggle<=0) {
				node.isVisible=!node.isVisible;
				
				if (node.isVisible) {
					nextToggle=period*visibleRatio;
				} else {
					nextToggle=period*(1f-visibleRatio);
				}
			}
		}
	}
	protected void Stop() {
		if (node!=null) {
			Futile.instance.SignalUpdate-=HandleUpdate;
			node.isVisible=oVisible;
			pendings.Remove(node);
			node=null;
			curDuration=-1f;
		}
	}
	static public void Go(FNode node_,float duration_,float period_,float visibleRatio_) {
		(new BlinkUtil()).go(node_,duration_,period_,visibleRatio_);
	}
	static public void Cancel(FNode node) {
		BlinkUtil obj=null;
		pendings.TryGetValue(node, out obj);
		if (obj!=null) {
			obj.Stop();
		}
	}
}








public class FxHelper {
	static readonly FxHelper instance=new FxHelper();
	
	static FxHelper () {
	}

	FxHelper() {
	}

	public static FxHelper Instance { get { return instance; } }
	
	public void RemoveFromContainer(AbstractGoTween tween) {
		//Debug.Log ("RemoveFromContainer tween="+tween);
		FNode node =(FNode)(((GoTween)tween).target);
		node.RemoveFromContainer();
	}
}


