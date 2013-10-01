using UnityEngine;
using System;
using System.Collections;
using System.Reflection;


public class AttenuatedTweenProperty : AbstractDoubleTweenProperty
{
	protected int _frameCount;
	protected int _frameMod;
	protected Func<float,float,float,float,float> _attenuatedEaseFunction;
	
	
	/// <summary>
	/// you can atenuate any AbstractTweenProperty.
	/// an attenuated tween property always ends at the same position it started.
	/// an be for shaking or oscilating a property for example
	/// frameMod allows you to specify what frame count the shakes should occur on. for example, a frameMod of 3 would mean that only when
	/// frameCount % 3 == 0 will the shake occur
	/// </summary>
	public AttenuatedTweenProperty( AbstractTweenProperty tweenPorperty, Func<float,float,float,float,float> attenuatedEaseFunction, int frameMod = 1 ) : base( tweenPorperty )
	{
		_frameMod = frameMod;
		_attenuatedEaseFunction = attenuatedEaseFunction;
	}
	
	
	#region Object overrides
	
	public override int GetHashCode()
	{
		return base.GetHashCode();
	}
	
	#endregion
	
	public override void prepareForUse()
	{
		base.prepareForUse();
		_frameCount = 0;
	}


	public override void tick( float totalElapsedTime )
	{
		// should we skip any frames?
		if( _frameMod > 1 && ++_frameCount % _frameMod != 0 )
			return;
		
		// _ownerTween is supposed to have a GoEaseLinear easeType
		// in this case _easeFunction is just used to attenuated the _attenuatedEaseFunction
		// _attenuatedEaseFunction contains the main effect (oscillation, shake, etc...)
		
		var attenuatedElapsedTime=_ownerTween.duration-_easeFunction(totalElapsedTime, 0, _ownerTween.duration, _ownerTween.duration);
		//if (attenuatedElapsedTime<0) attenuatedElapsedTime=0;
		var easedTime=_attenuatedEaseFunction(totalElapsedTime,0f,attenuatedElapsedTime,_ownerTween.duration);
		
		//Attenuation doesn't really work if easeType is Punch
		
		/*
		if (attenuatedElapsedTime<=0) {
			if (easedTime!=0) {
				Debug.LogError ("attenuatedElapsedTime="+attenuatedElapsedTime+" easedTime="+easedTime);
			}
		}
		*/
		
		_tweenProperty.tick(easedTime);
	}
}
