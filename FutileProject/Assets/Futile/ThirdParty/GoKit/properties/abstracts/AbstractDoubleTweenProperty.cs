using UnityEngine;
using System.Collections;


public abstract class AbstractDoubleTweenProperty : AbstractTweenProperty
{
	protected AbstractTweenProperty _tweenProperty;

	/// <summary>
	/// reference another tween property
	/// </summary>
	public AbstractDoubleTweenProperty( AbstractTweenProperty tweenPorperty ) : base( true )
	{
		_tweenProperty = tweenPorperty;
	}
	
	
	#region Object overrides
	
	public override int GetHashCode()
	{
		return base.GetHashCode();
	}
	
	
	public override bool Equals( object obj )
	{
		if (base.Equals(obj)) {
			return _tweenProperty.Equals(((AbstractDoubleTweenProperty)obj)._tweenProperty);
		}
		return false;
	}
	
	#endregion
	
	public override void init( GoTween owner )
	{
		base.init(owner);
		//Init owner of _tweenProperty and force ease type to linear
		_tweenProperty.setEaseType(GoEaseType.Linear);
		_tweenProperty.init(owner);
	}

	public override bool validateTarget( object target )
	{
		return _tweenProperty.validateTarget(target);
	}
	
	
	public override void prepareForUse()
	{
		_tweenProperty.prepareForUse();
	}
}
