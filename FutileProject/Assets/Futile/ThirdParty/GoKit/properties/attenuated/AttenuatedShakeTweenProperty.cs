using UnityEngine;
using System.Collections;



public class AttenuatedShakeTweenProperty : AttenuatedTweenProperty
{

	/// <summary>
	/// you can shake any AbstractTweenProperty. 
	/// frameMod allows you to specify what frame count the shakes should occur on. for example, a frameMod of 3 would mean that only when
	/// frameCount % 3 == 0 will the shake occur
	/// </summary>
	public AttenuatedShakeTweenProperty( AbstractTweenProperty tweenPorperty, int frameMod = 1 ) : base( tweenPorperty, (t,b,c,d) => b+Random.Range(-c,c) ,frameMod )
	{
	}
}

