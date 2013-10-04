using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/*
Use :

FLabelAnimate label=new FLabelAnimate(Config.fontFile,"The quick brown fox\njumps over the lazy\ndog",Config.textParams);
GoTweenConfig config=new GoTweenConfig().intProp("endVisibleCharIdx",label.text.Length,false);
AddChild (label);
		
Go.to (label,1f,config);

*/



public class FLabelAnimate : FLabel
{
	protected int _startVisibleCharIdx=0; //idx included
	protected int _endVisibleCharIdx=0; //idx excluded, when is equal to 0, nothing is shown
	
	public FLabelAnimate(string fontName, string text, bool startVisible=false, FTextParams textParams=null):base(fontName, text, textParams==null?new FTextParams():textParams) 
	{
		if (startVisible) {
			_endVisibleCharIdx=text.Length;
		}
	}
	
	//startVisibleCharIdx and endVisibleStartIdx are properties that can be animated with GoKit
	public int startVisibleCharIdx { 
		get { return _startVisibleCharIdx; }
		set {
			if (value!=_startVisibleCharIdx) {
				_startVisibleCharIdx=value;
				_isAlphaDirty=true;
			}	
		}
	}
	public int endVisibleCharIdx { 
		get { return _endVisibleCharIdx; }
		set {
			if (value!=_endVisibleCharIdx) {
				_endVisibleCharIdx=value;
				_isAlphaDirty=true;
			}	
		}
	}
	
	override public void PopulateRenderLayer()
	{
		if(_isOnStage && _firstFacetIndex != -1)
		{
			_isMeshDirty = false;
			
			Vector3[] vertices = _renderLayer.vertices;
			Vector2[] uvs = _renderLayer.uvs;
			Color[] colors = _renderLayer.colors;
			
			int vertexIndex0 = _firstFacetIndex*4;
			int vertexIndex1 = vertexIndex0 + 1;
			int vertexIndex2 = vertexIndex0 + 2;
			int vertexIndex3 = vertexIndex0 + 3;

			int charIdx=0;
			int lineCount = _letterQuadLines.Length;
			for(int i = 0; i<lineCount; i++)
			{
				FLetterQuad[] quads = _letterQuadLines[i].quads;
				
				
				int quadCount = quads.Length;
				
				for(int q = 0; q<quadCount; q++)
				{
					FLetterQuad quad = quads[q];
					FCharInfo charInfo = quad.charInfo;
					
					_concatenatedMatrix.ApplyVector3FromLocalVector2(ref vertices[vertexIndex0], quad.topLeft,0);
					_concatenatedMatrix.ApplyVector3FromLocalVector2(ref vertices[vertexIndex1], quad.topRight,0);
					_concatenatedMatrix.ApplyVector3FromLocalVector2(ref vertices[vertexIndex2], quad.bottomRight,0);
					_concatenatedMatrix.ApplyVector3FromLocalVector2(ref vertices[vertexIndex3], quad.bottomLeft,0);
					
					uvs[vertexIndex0] = charInfo.uvTopLeft;
					uvs[vertexIndex1] = charInfo.uvTopRight;
					uvs[vertexIndex2] = charInfo.uvBottomRight;
					uvs[vertexIndex3] = charInfo.uvBottomLeft;
					
					if ((charIdx>=_startVisibleCharIdx)&&(charIdx<_endVisibleCharIdx)) {
						//shown
						colors[vertexIndex0] = _alphaColor;
						colors[vertexIndex1] = _alphaColor;
						colors[vertexIndex2] = _alphaColor;
						colors[vertexIndex3] = _alphaColor;
					} else {
						//not shown
						colors[vertexIndex0] = Color.clear;
						colors[vertexIndex1] = Color.clear;
						colors[vertexIndex2] = Color.clear;
						colors[vertexIndex3] = Color.clear;
					}
					
					vertexIndex0 += 4;
					vertexIndex1 += 4;
					vertexIndex2 += 4;
					vertexIndex3 += 4;
					
					charIdx++;
				}
			}
			
			_renderLayer.HandleVertsChange();
		}
	}
}