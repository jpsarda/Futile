using UnityEngine;
using System;
using System.Collections.Generic;



public class BasePostProcessing : MonoBehaviour {
	public Material mat;
	public BasePostProcessing() {
	}
	protected float _radius;
	public float radius {
		get { return _radius; }
		set { 
			_radius=value;
			mat.SetFloat("_Radius",_radius);
		}
	}
	protected float _amplitude;
	public float amplitude {
		get { return _amplitude; }
		set { 
			_amplitude=value;
			mat.SetFloat("_Amplitude",_amplitude);
		}
	}
	protected GoTween _tween=null;
	protected void CancelTweens() {
		if (_tween!=null) _tween.destroy();
		Futile.instance.StopDelayedCall(DestroyIt);
	}
	
	protected bool _staticInstance=false;
	protected void HandleComplete(AbstractGoTween tween) {
		_tween=null;
		if (_staticInstance) {
			//destroy with delay
			this.enabled=false;
			//Don't destroy it as it makes screen flickering on andorid
			Futile.instance.StartDelayedCallback(DestroyIt,1f);
		} else {
			DestroyIt();
		}
	}
	protected void DestroyIt() {
		this.enabled=false;
		if (_staticInstance) {
			DestroyStaticInstance();
		}
		//Futile.instance.StartDelayedCallback(DestroyComponent,0.25f);
		DestroyComponent();
	}
	protected void DestroyComponent() {
		Destroy(this);
	}
	virtual protected void DestroyStaticInstance() {
		//to override 
	}
	protected void SetStaticInstance() {
		_staticInstance=true;
	}
	
	
	//static BasePostProcessing _postProcessing;
	static public BasePostProcessing Get<T>(bool newInstance,ref BasePostProcessing _postProcessing) where T : BasePostProcessing {
		//newInstance=false; //force to false, returns always the smae component, as it seems that destroying a component makes a flicker on Android, so we keep the static instance bu disabled
		if (newInstance) {
			T postProcessing=Futile.instance.camera.gameObject.AddComponent<T>(); 
			return postProcessing;
		} else {
			if (_postProcessing==null) {
				_postProcessing=Futile.instance.camera.gameObject.AddComponent<T>();
				_postProcessing.SetStaticInstance();
			} else {
				_postProcessing.enabled=true;
				Debug.Log("REUSE POST PROCESSING");
			}
			return _postProcessing;
		}
	}
	void OnRenderImage(RenderTexture src, RenderTexture dest) {
		Graphics.Blit(src, dest, mat);
	}
}



public class WaveExploPostProcessing : BasePostProcessing {
	public WaveExploPostProcessing() {
		mat=new Material(Shader.Find("Custom/WaveExplo"));
	}
	
	public void StartIt(Vector2 center,float amplitudeP=0.045f,float radiusP=0.6f,float durationP=0.25f) {
		CancelTweens();
		mat.SetFloat("_CenterX",(center.x+Futile.screen.halfWidth)/Futile.screen.width);
		mat.SetFloat("_CenterY",(center.y+Futile.screen.halfHeight)/Futile.screen.height);
		radius=0f;
		amplitude=amplitudeP;
		
		GoTweenConfig config=new GoTweenConfig().floatProp("radius",radiusP).floatProp ("amplitude",0f);
		//config.easeType=GoEaseType.ExpoOut;
		config.onComplete(HandleComplete);
		Go.to(this,durationP,config);
	}
	
	static BasePostProcessing _postProcessing;
	static public WaveExploPostProcessing Get(bool newInstance=false) {
		return Get<WaveExploPostProcessing>(newInstance,ref _postProcessing) as WaveExploPostProcessing;
	}
	override protected void DestroyStaticInstance() {
		_postProcessing=null;
	}
}




public class XWaveExploPostProcessing : BasePostProcessing {
	public XWaveExploPostProcessing() {
		mat=new Material(Shader.Find("Custom/XWaveExplo"));
	}
	
	public void StartIt(float x,float amplitudeP=0.045f,float radiusP=0.6f,float durationP=0.25f) {
		CancelTweens();
		mat.SetFloat("_CenterX",(x+Futile.screen.halfWidth)/Futile.screen.width);
		radius=0f;
		amplitude=amplitudeP;
		
		GoTweenConfig config=new GoTweenConfig().floatProp("radius",radiusP).floatProp ("amplitude",0f);
		config.onComplete(HandleComplete);
		Go.to(this,durationP,config);
	}
	
	static BasePostProcessing _postProcessing;
	static public XWaveExploPostProcessing Get(bool newInstance=false) {
		return Get<XWaveExploPostProcessing>(newInstance,ref _postProcessing) as XWaveExploPostProcessing;
	}
	override protected void DestroyStaticInstance() {
		_postProcessing=null;
	}
}



public class YWaveExploPostProcessing : BasePostProcessing {
	public YWaveExploPostProcessing() {
		mat=new Material(Shader.Find("Custom/YWaveExplo"));
	}
	
	public void StartIt(float y,float amplitudeP=0.045f,float radiusP=0.6f,float durationP=0.25f) {
		CancelTweens();
		mat.SetFloat("_CenterY",(y+Futile.screen.halfHeight)/Futile.screen.height);
		radius=0f;
		amplitude=amplitudeP;
		
		GoTweenConfig config=new GoTweenConfig().floatProp("radius",radiusP).floatProp ("amplitude",0f);
		config.onComplete(HandleComplete);
		Go.to(this,durationP,config);
	}
	
	static BasePostProcessing _postProcessing;
	static public YWaveExploPostProcessing Get(bool newInstance=false) {
		return Get<YWaveExploPostProcessing>(newInstance,ref _postProcessing) as YWaveExploPostProcessing;
	}
	override protected void DestroyStaticInstance() {
		_postProcessing=null;
	}
}




public class ImpactPostProcessing : BasePostProcessing {
	public ImpactPostProcessing() {
		mat=new Material(Shader.Find("Custom/Impact"));
	}
	
	public void StartIt(Vector2 center,float amplitudeP=0.04f,float durationP=0.5f) {
		CancelTweens();
		mat.SetFloat("_CenterX",(center.x+Futile.screen.halfWidth)/Futile.screen.width);
		mat.SetFloat("_CenterY",(center.y+Futile.screen.halfHeight)/Futile.screen.height);
		amplitude=0f;
		
		GoTweenConfig config=new GoTweenConfig().oscillateFloatProp("amplitude",amplitudeP,0.2f);
		//config.easeType=GoEaseType.ExpoOut;
		config.onComplete(HandleComplete);
		_tween=Go.to(this,durationP,config);
	}
	
	static BasePostProcessing _postProcessing;
	static public ImpactPostProcessing Get(bool newInstance=false) {
		return Get<ImpactPostProcessing>(newInstance,ref _postProcessing) as ImpactPostProcessing;
	}
	override protected void DestroyStaticInstance() {
		_postProcessing=null;
	}
}


public class XImpactPostProcessing : BasePostProcessing {
	public XImpactPostProcessing() {
		mat=new Material(Shader.Find("Custom/XImpact"));
	}
	
	public void StartIt(float x,float amplitudeP=0.04f,float durationP=0.5f) {
		CancelTweens();
		mat.SetFloat("_CenterX",(x+Futile.screen.halfWidth)/Futile.screen.width);
		amplitude=0f;
		
		GoTweenConfig config=new GoTweenConfig().oscillateFloatProp("amplitude",amplitudeP,0.2f);
		//config.easeType=GoEaseType.ExpoOut;
		config.onComplete(HandleComplete);
		_tween=Go.to(this,durationP,config);
	}
	
	static BasePostProcessing _postProcessing;
	static public XImpactPostProcessing Get(bool newInstance=false) {
		return Get<XImpactPostProcessing>(newInstance,ref _postProcessing) as XImpactPostProcessing;
	}
	override protected void DestroyStaticInstance() {
		_postProcessing=null;
	}
}


public class YImpactPostProcessing : BasePostProcessing {
	public YImpactPostProcessing() {
		mat=new Material(Shader.Find("Custom/YImpact"));
	}
	
	public void StartIt(float y,float amplitudeP=0.04f,float durationP=0.5f) {
		CancelTweens();
		mat.SetFloat("_CenterY",(y+Futile.screen.halfHeight)/Futile.screen.height);
		amplitude=0f;
		
		GoTweenConfig config=new GoTweenConfig().oscillateFloatProp("amplitude",amplitudeP,0.2f);
		//config.easeType=GoEaseType.ExpoOut;
		config.onComplete(HandleComplete);
		_tween=Go.to(this,durationP,config);
	}
	
	static BasePostProcessing _postProcessing;
	static public YImpactPostProcessing Get(bool newInstance=false) {
		return Get<YImpactPostProcessing>(newInstance,ref _postProcessing) as YImpactPostProcessing;
	}
	override protected void DestroyStaticInstance() {
		_postProcessing=null;
	}
}






public class DistortionPostProcessing : BasePostProcessing {
	public DistortionPostProcessing() {
		mat=new Material(Shader.Find("Custom/Distortion"));
	}
	
	public void StartIt(Vector2 center,float amplitudeP=0.04f,float durationP=0.5f,float periodP=0.2f) {
		CancelTweens();
		mat.SetFloat("_CenterX",(center.x+Futile.screen.halfWidth)/Futile.screen.width);
		mat.SetFloat("_CenterY",(center.y+Futile.screen.halfHeight)/Futile.screen.height);
		amplitude=0f;
		
		GoTweenConfig config=new GoTweenConfig().oscillateFloatProp("amplitude",amplitudeP,periodP);
		//config.easeType=GoEaseType.ExpoOut;
		config.onComplete(HandleComplete);
		_tween=Go.to(this,durationP,config);
	}
	
	
	public void GoTo(bool terminate,Vector2 center,float amplitudeP=0.1f,float durationP=0.5f,GoEaseType easeType=GoEaseType.Linear) {
		CancelTweens();
		mat.SetFloat("_CenterX",(center.x+Futile.screen.halfWidth)/Futile.screen.width);
		mat.SetFloat("_CenterY",(center.y+Futile.screen.halfHeight)/Futile.screen.height);
		//amplitude=0f;
		
		GoTweenConfig config=new GoTweenConfig().floatProp("amplitude",amplitudeP);
		config.easeType=easeType;
		if (terminate) config.onComplete(HandleComplete);
		_tween=Go.to(this,durationP,config);
	}
	
	static BasePostProcessing _postProcessing;
	static public DistortionPostProcessing Get(bool newInstance=false) {
		return Get<DistortionPostProcessing>(newInstance,ref _postProcessing) as DistortionPostProcessing;
	}
	override protected void DestroyStaticInstance() {
		_postProcessing=null;
	}
}