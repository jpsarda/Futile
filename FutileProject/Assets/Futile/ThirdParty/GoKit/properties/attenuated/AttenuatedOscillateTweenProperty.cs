using UnityEngine;
using System.Collections;



public class AttenuatedOscillateTweenProperty : AttenuatedTweenProperty
{

	/// <summary>
	/// you can oscilate any AbstractTweenProperty. 
	/// frameMod allows you to specify what frame count the shakes should occur on. for example, a frameMod of 3 would mean that only when
	/// frameCount % 3 == 0 will the shake occur
	/// </summary>
	public AttenuatedOscillateTweenProperty( AbstractTweenProperty tweenPorperty, float period, int frameMod = 1 ) : base( tweenPorperty, (t,b,c,d) => b+c*Mathf.Sin(t*2*Mathf.PI/period) ,frameMod )
	{
	}
}

