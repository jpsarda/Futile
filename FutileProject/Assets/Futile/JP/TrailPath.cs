using UnityEngine;
using System;
using System.Collections.Generic;

/* 
 * 
 * Trail FX for Futile 2D engine (Unity3D). Works on any custom node as long as you provide a CreateClone method.
 * FSpriteTrail provided as an example.
 * 
 * Usage
 * 
 * 
	FSprite sprite=new FSprite("heros/magicianjp0");
	AddChild(sprite);
	
	GoTweenConfig config=new GoTweenConfig();
	config.oscillateFloatProp("x",100f,2f,true,1);
	config.oscillateFloatProp("y",100f,1.7f,true,1);
	Go.to (sprite,1000000,config);
	sprite.scale=2f;
	
	FSpriteTrail<SpriteCursor2D> trail=new FSpriteTrail<SpriteCursor2D>(sprite,new List<int>() {5,10,15,20, 25, 30,35,40,45,50},true);
	trail.FadeIn(0.5f);
*/


public class TrailPath2D<T> where T : BaseCursor2D, new () 
{
	protected T headCursor;
	protected bool initialized=false;
	protected List<T> _cursors;
	protected int _maxCursors;
	public int maxCursors { get { return _maxCursors; } }
	protected int _headIdx;
	protected T _defaultCursor;
	public TrailPath2D(int maxCursors,T defaultCursor)
	{
		_maxCursors=maxCursors;
		_cursors=new List<T>(_maxCursors);
		_headIdx=-1;
		_defaultCursor=defaultCursor;
	}
	
	public void Push(T cursor) {
		headCursor=cursor;
		_headIdx++; if (_headIdx>=_maxCursors) _headIdx=0;
		if (_headIdx>=_cursors.Count) {
			_cursors.Add(new T()); //add (only at the begining until the List is full)
		}
		_cursors[_headIdx].SetValuesFromCursor(cursor); //replace (most of the time)
	}
	
	//headOffset >=0 and <_maxCursors
	public T Get(int headOffset) {
		return Get (headOffset,_defaultCursor);
	}
	public T Get(int headOffset,T defaultCursor) {
		int offsetIdx=_headIdx-headOffset;
		if (offsetIdx<0) {
			if (_cursors.Count<_maxCursors) {
				return defaultCursor;
			}
			offsetIdx+=_maxCursors;
		}
		return _cursors[offsetIdx];
	}
	
	
	//Debug
	public void LogCursors() {
		String log="|";
		for (int i=0;i<_maxCursors;i++) {
			log+="ofst "+i+":"+Get(i).position+"|";
		}
		Debug.Log(log);
	}
}



public class BaseCursor2D
{
	public Vector2 position;
	public BaseCursor2D()
	{
	}
	
	virtual public void SetValuesFromCursor(BaseCursor2D cursor) {
		position=cursor.position;
		
	}
	
	virtual public void SetValuesFromNode(FNode node) {
		position=node.LocalToGlobal(Vector2.zero);
	}
	virtual public void SetOnNode(FNode node) {
		node.SetPosition(node.container.GlobalToLocal(position));
	}
}


public class AdvancedCursor2D : BaseCursor2D
{
	public float rotation;
	public float scaleX;
	public float scaleY;
	public float alpha;
	public AdvancedCursor2D() : base()
	{
	}
	
	override public void SetValuesFromCursor(BaseCursor2D cursor) {
		base.SetValuesFromCursor(cursor);
		AdvancedCursor2D extendedCursor=cursor as AdvancedCursor2D;
		if (extendedCursor!=null) {
			rotation=extendedCursor.rotation;
			scaleX=extendedCursor.scaleX;
			scaleY=extendedCursor.scaleY;
			alpha=extendedCursor.alpha;
		}
	}
	
	override public void SetValuesFromNode(FNode node) {
		base.SetValuesFromNode(node);
		rotation=node.rotation;
		scaleX=node.scaleX;
		scaleY=node.scaleY;
		alpha=node.alpha;
	}
	override public void SetOnNode(FNode node) {
		base.SetOnNode(node);
		node.rotation=rotation;
		node.scaleX=scaleX;
		node.scaleY=scaleY;
		node.alpha=alpha;
	}
}



public class SpriteCursor2D : AdvancedCursor2D
{
	public Color color;
	public SpriteCursor2D() : base()
	{
	}
	
	override public void SetValuesFromCursor(BaseCursor2D cursor) {
		base.SetValuesFromCursor(cursor);
		SpriteCursor2D extendedCursor=cursor as SpriteCursor2D;
		if (extendedCursor!=null) {
			color=extendedCursor.color;
		}
	}
	
	override public void SetValuesFromNode(FNode node) {
		base.SetValuesFromNode(node);
		FSprite sprite=node as FSprite;
		if (sprite!=null) {
			color=sprite.color;
		}
	}
	override public void SetOnNode(FNode node) {
		base.SetOnNode(node);
		FSprite sprite=node as FSprite;
		if (sprite!=null) {
			sprite.color=color;
		}
	}
}





abstract public class FNodeTrail<T> : FContainer where T : BaseCursor2D, new () {
	protected TrailPath2D<T> _trailPath;
	protected FNode _head;
	protected List<int> _offsets;
	protected T _workingCursor;
	protected bool _front; // clones in over the head node?
	public FNodeTrail(FNode head,List<int> offsets,bool front) {
		_workingCursor=new T();
		_head=head;
		_offsets=offsets;
		_front=front;
	}
	
	public void FadeIn(float duration) {
		if (_container==null) {
			if (_head.container!=null) {
				
				_trailPath= new TrailPath2D<T>(_offsets.GetLastObject()+1,null);
				
				_workingCursor.SetValuesFromNode(_head);
				
				T defaultCursor=new T();
				defaultCursor.SetValuesFromCursor(_workingCursor);
					
				_trailPath= new TrailPath2D<T>(_offsets.GetLastObject()+1,defaultCursor);
				
				_trailPath.Push(_workingCursor);
				
				_head.container.AddChild(this);
				if (!_front) _head.MoveToFront();
				
				for (int i=0;i<_offsets.Count;i++) {
					FNode node=CreateClone();
					AddChild(node);
				}
				
				this.alpha=0;
				FxUtils.FadeIn(this,duration);
				
				Futile.instance.SignalUpdate+=HandleUpdate;
			}
		}
	}
	
	protected void HandleUpdate() {
		_workingCursor.SetValuesFromNode(_head);
		_trailPath.Push(_workingCursor);
		int i=_offsets.Count;
		while (i-->0) {
			UpdateNode(i);
		}
		//_trailPath.LogCursors();
	}
	
	protected void UpdateNode(int i) {
		int offset=_offsets[i];
		T nodeCursor=_trailPath.Get(offset); //not null has we have a defaultCursor
		FNode node=GetChildAt(_front?i:(_offsets.Count-i-1));
		nodeCursor.SetOnNode(node);
		
		NodeFx(node,i,offset);
	}
	
	//Custom FX
	//As a default FX, we set an alpha gradient on the trail
	virtual protected void NodeFx(FNode node,int i,int offset) {
		node.alpha*=1f-(float)offset/(float)_trailPath.maxCursors;
		/* Debug only
		if (i==0) (node as FSprite).color=Color.red;
		if (i==1) (node as FSprite).color=Color.green;
		if (i==2) (node as FSprite).color=Color.blue;
		*/
	}
	
	public override void HandleRemovedFromStage ()
	{
		base.HandleRemovedFromStage ();
		Futile.instance.SignalUpdate-=HandleUpdate;
	}
	
	
	public void FadeOut(float duration) {
		FxUtils.FadeOut(this,duration,true);
	}
	
	
	abstract protected FNode CreateClone();
	
}




//Example of extending the class for sprites
//You can easily make your custom Trail class for your custom node class as long as you provide the CreateClone method

public class FSpriteTrail<T> : FNodeTrail<T> where T : BaseCursor2D, new () {
	public FSpriteTrail(FSprite head,List<int> offsets,bool front) : base(head,offsets,front) {
	}
	override protected void NodeFx(FNode node,int i,int offset) {
		base.NodeFx(node,i,offset);
	}
	override protected FNode CreateClone() {
		FSprite sprite=new FSprite((_head as FSprite).element.name);
		return sprite;
	}
}

