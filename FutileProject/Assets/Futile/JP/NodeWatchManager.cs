using UnityEngine;
using System;
using System.Collections.Generic;


/*
This set of classes for following a FNode's changes.
A typical use is when you have 2 sprites that should have the same position but coming from 2 different texture atlas.
To optimize the number of render layers you'll have to show the 2 sprites in 2 different layers.
And sprite 1 could watch the changes of sprite 0 with this set of classes.
*/
	


public interface IFNodeWatcher
{
    void WatchedFNodesChanged();
}

public class FNodeWatcherManager
{
	static readonly FNodeWatcherManager instance=new FNodeWatcherManager();
	static FNodeWatcherManager () {}
	FNodeWatcherManager() {}
	public static FNodeWatcherManager Instance { get { instance.Initialize(); return instance; } }
	
	protected Dictionary<FNode, List<IFNodeWatcher>> _watchersByFNode = new Dictionary<FNode, List<IFNodeWatcher>>();
	protected List<FNode> _watchedFNodes = new List<FNode>();
	
	protected bool _initialized=false;
	protected void Initialize() {
		if (!_initialized) {
			Futile.instance.SignalAfterUpdate+=HandleAfterUpdate;
		}
	}
	
	
	//TODO, garbage collect forgotten listeners using WeakReference http://stackoverflow.com/questions/1686416/c-sharp-get-number-of-references-to-object
	//While this is not done, watchers must unsubscribe
	
	protected HashSet<IFNodeWatcher> _watchersToNotify = new HashSet<IFNodeWatcher>();
	protected void HandleAfterUpdate() {
		_watchersToNotify.Clear();
		
		//Warning this doesn't deal with a watcher watching a watcher
		foreach (FNode watched in _watchedFNodes) {
			if (watched.isMatrixDirty) {
				_watchersToNotify.UnionWith(_watchersByFNode[watched]);
			}
		}
		
		foreach (IFNodeWatcher watcher in _watchersToNotify) {
			watcher.WatchedFNodesChanged();
		}
	}
	
	public void Subscribe(IFNodeWatcher watcher,FNode watched) {
		if (!_watchedFNodes.Contains(watched)) {
			_watchedFNodes.Add(watched);
			List<IFNodeWatcher> watchers=new List<IFNodeWatcher>();
			watchers.Add(watcher);
			_watchersByFNode.Add(watched,watchers);
		} else {
			_watchersByFNode[watched].Add(watcher);
		}
	}
	public void Unsubscribe(IFNodeWatcher watcher,FNode watched) {
		if (_watchedFNodes.Contains(watched)) {
			List<IFNodeWatcher> watchers=_watchersByFNode[watched];
			watchers.Remove(watcher);
			if (watchers.Count==0) {
				_watchersByFNode.Remove(watched);
				_watchedFNodes.Remove(watched);
			}
		}
	}
	public void Unsubscribe(IFNodeWatcher watcher) {
		int i=_watchedFNodes.Count;
		while (i-->0) {
			FNode watched=_watchedFNodes[i];
			List<IFNodeWatcher> watchers=_watchersByFNode[watched];
			watchers.Remove(watcher);
			if (watchers.Count==0) {
				_watchersByFNode.Remove(watched);
				_watchedFNodes.RemoveAt(i);
			}
		}
	}
}


//Extend some methods in IFNodeWatcher interface, but this is not essential
public static class FNodeWatcherExtensions {
	public static void Watch(this IFNodeWatcher watcher, FNode watched){
		FNodeWatcherManager.Instance.Subscribe(watcher,watched);
	}
	public static void Unwatch(this IFNodeWatcher watcher){
		FNodeWatcherManager.Instance.Unsubscribe(watcher);
	}
	public static void Unwatch(this IFNodeWatcher watcher, FNode watched){
		FNodeWatcherManager.Instance.Unsubscribe(watcher,watched);
	}
}





public class FNodeFollower : IFNodeWatcher
{
	protected FNode _follower,_following;
	protected Vector2 _offset;
	
	public FNodeFollower(FNode follower,FNode following) {
		_follower=follower;
		_following=following;
		
		Vector2 globalPos=_following.LocalToGlobal(Vector2.zero);
		Vector2 localPos=_follower.container.GlobalToLocal(globalPos);
		_offset=_follower.GetPosition()-localPos;
		
		((IFNodeWatcher)this).Watch(_following);
	}
	
	virtual public void Stop() {
		((IFNodeWatcher)this).Unwatch(_following);
	}
	
	virtual public void WatchedFNodesChanged() {
		//if (_follower.container!=null) { //actually, you have to check this yourself and Stop the Follower when needed
		Vector2 globalPos=_following.LocalToGlobal(Vector2.zero);
		Vector2 localPos=_follower.container.GlobalToLocal(globalPos);
		_follower.SetPosition(localPos+_offset);
		//}
	}
}