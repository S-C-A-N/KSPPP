using System;
using UnityEngine;
using System.Collections.Generic;

namespace KSPPP.UI
{
	public static class Layout {
		public enum VAlign { Top, Bottom, Middle, Fill, None }
		public enum HAlign { Left, Right, Center, Fill, None }
		public static bool IsMissingKey<K,V> (this Dictionary<K,V> t, K name) {
			return !t.ContainsKey(name);
		}

		public static Dictionary<string,bool> Toggles = new Dictionary<string,bool>();
		public static bool GetToggle(string name) {
			if (Toggles.IsMissingKey(name)) Toggles.Add(name, false);
			return Toggles[name];
		}
		public static bool SetToggle(string name, bool b) {
			if (Toggles.IsMissingKey(name)) { Toggles.Add(name,b); return b; }
			Toggles[name] = b;
			return b;
		}
		public static bool Toggle(string name,GUIContent content, params GUILayoutOption[] opts) {
			return SetToggle(name, GUILayout.Toggle(GetToggle(name),content,opts));
		}
		public static bool Toggle(string name,string text, params GUILayoutOption[] opts) {
			return SetToggle(name, GUILayout.Toggle(GetToggle(name),text,opts));
		}
		public static bool Toggle(string name,Texture texture, params GUILayoutOption[] opts) {
			return SetToggle(name, GUILayout.Toggle(GetToggle(name),texture,opts));
		}
		public static bool Toggle(string name,GUIContent content, GUIStyle style, params GUILayoutOption[] opts) {
			return SetToggle(name, GUILayout.Toggle(GetToggle(name),content,style,opts));
		}
		public static bool Toggle(string name,string text, GUIStyle style, params GUILayoutOption[] opts) {
			return SetToggle(name, GUILayout.Toggle(GetToggle(name),text,style,opts));
		}
		public static bool Toggle(string name,Texture texture, GUIStyle style, params GUILayoutOption[] opts) {
			return SetToggle(name, GUILayout.Toggle(GetToggle(name),texture,style,opts));
		}
	}

	public class Group : IDisposable {
		public static Dictionary<string,Rect> positions = new Dictionary<string, Rect>();
		private static Rect @get (string name) {
			if (positions.IsMissingKey(name)) positions.Add(name,new Rect());
			return positions[name];
		}
		private static Rect @set (string name, Rect r) {
			if (positions.IsMissingKey(name)) 
				positions.Add(name, r);
			positions[name] = r;
			return positions[name];
			}

		public Group(string name) 																		{ UnityEngine.GUI.BeginGroup(@get(name)); 			}
		public Group(string name, string t) 													{ UnityEngine.GUI.BeginGroup(@get(name), t); 		}
		public Group(string name, Texture t) 													{ UnityEngine.GUI.BeginGroup(@get(name), t); 		}
		public Group(string name,						GUIContent c)							{ UnityEngine.GUI.BeginGroup(@get(name), 	c); 	}
		public Group(string name, 												GUIStyle s) { UnityEngine.GUI.BeginGroup(@get(name),		s); }
		public Group(string name, string t,								GUIStyle s) { UnityEngine.GUI.BeginGroup(@get(name), t, 	s); }
		public Group(string name, Texture t,							GUIStyle s) { UnityEngine.GUI.BeginGroup(@get(name), t, 	s); }
		public Group(string name, 					GUIContent c,	GUIStyle s) { UnityEngine.GUI.BeginGroup(@get(name), 	c,	s); }

		void IDisposable.Dispose() {
			UnityEngine.GUI.EndGroup();
		}

	}
	public class ScrollView : IDisposable {
		public static Dictionary<string,Vector2> sheets = new Dictionary<string, Vector2>();

		private static Vector2 @get (string name) {
			if (sheets.IsMissingKey(name)) sheets.Add(name,new Vector2());
			return sheets[name];
		}
		private static Vector2 @set (string name, Vector2 v) {
			if (sheets.IsMissingKey(name)) sheets.Add(name,v);
			sheets[name] = v;
			return sheets[name];
		}

		// GUILayout.BeginScrollView has 7 overloaded methods.
		// * The first argument is always a Vector2. (scroll position)
		// * None of these arguments is a name; so that is introduced here.
		public ScrollView(string name,GUIStyle style) {			
			@set(name,GUILayout.BeginScrollView(@get(name),style));			
		}
		public ScrollView(string name,																	params GUILayoutOption[] opts) {			
			@set(name,GUILayout.BeginScrollView(@get(name),opts));			
		}
		public ScrollView(string name,GUIStyle style, 													params GUILayoutOption[] opts) {			
			@set(name,GUILayout.BeginScrollView(@get(name),style,opts));			
		}
		public ScrollView(string name,bool showH, bool showV,											params GUILayoutOption[] opts) {			
			@set(name,GUILayout.BeginScrollView(@get(name),showH,showV,opts));			
		}
		public ScrollView(string name,						GUIStyle hBar, GUIStyle vBar,				params GUILayoutOption[] opts) {			
			@set(name,GUILayout.BeginScrollView(@get(name),hBar,vBar,opts));
		}
		public ScrollView(string name,bool showH, bool showV,GUIStyle hBar, GUIStyle vBar, 				params GUILayoutOption[] opts) {			
			@set(name,GUILayout.BeginScrollView(@get(name),showH,showV,hBar,vBar,opts));			
		}
		public ScrollView(string name,bool showH, bool showV,GUIStyle hBar, GUIStyle vBar, GUIStyle area,params GUILayoutOption[] opts) {			
			@set(name,GUILayout.BeginScrollView(@get(name),showH,showV,hBar,vBar,area,opts));			
		}

		void IDisposable.Dispose() {
			GUILayout.EndScrollView();
		}
	}
	public class Area : IDisposable {
		public Area(Rect screenRect) {
			GUILayout.BeginArea(screenRect);
		}
		public Area(Rect screenRect,GUIStyle style) {
			GUILayout.BeginArea(screenRect,style);
		}
		public Area(Rect screenRect,GUIContent content) {
			GUILayout.BeginArea(screenRect,content);
		}
		public Area(Rect screenRect,GUIContent content,GUIStyle style) {
			GUILayout.BeginArea(screenRect,content,style);
		}
		public Area(Rect screenRect,Texture image,GUIStyle style) {
			GUILayout.BeginArea(screenRect,image,style);
		}
		void IDisposable.Dispose() {
			GUILayout.EndArea();
		}
	}
	public class Row  : IDisposable {

		public Layout.HAlign align;

		public Row(Layout.HAlign alignment,									params GUILayoutOption[] opts) {
			GUILayout.BeginHorizontal(opts);
			align = alignment;
			SpaceIfNeeded();
		}
		public Row(Layout.HAlign alignment,					GUIStyle style, params GUILayoutOption[] opts) {
			GUILayout.BeginHorizontal(style,opts);	
			align = alignment;
			SpaceIfNeeded();
		}
		public Row(Layout.HAlign alignment, GUIContent content,GUIStyle style, params GUILayoutOption[] opts) {
			GUILayout.BeginHorizontal(content,style,opts);	
			align = alignment;
			SpaceIfNeeded();
		}
		public Row(Layout.HAlign alignment, Texture image,		GUIStyle style, params GUILayoutOption[] opts) {
			GUILayout.BeginHorizontal(image,style,opts);
			align = alignment;	
			SpaceIfNeeded();
		}
		public Row(Layout.HAlign alignment = Layout.HAlign.None) {
			GUILayout.BeginHorizontal();
			align = alignment;
			SpaceIfNeeded();
		}
		private void SpaceIfNeeded() {
			switch (align) {
			case Layout.HAlign.Right:
			case Layout.HAlign.Center:	GUILayout.FlexibleSpace(); break;
			}

		}
		void IDisposable.Dispose() {
			switch (align) {
			case Layout.HAlign.Left:
			case Layout.HAlign.Center:	GUILayout.FlexibleSpace(); break;
			}
			GUILayout.EndHorizontal();
		}
	}
	public class Col  : IDisposable {
	
		public Layout.VAlign align;

		public Col(Layout.VAlign alignment,																			params GUILayoutOption[] opts) {
			GUILayout.BeginVertical(opts);
			align = alignment;
			SpaceIfNeeded();
		}
		public Col(Layout.VAlign alignment,											GUIStyle style, params GUILayoutOption[] opts) {
			GUILayout.BeginVertical(style,opts);	
			align = alignment;
			SpaceIfNeeded();
		}
		public Col(Layout.VAlign alignment, GUIContent content,	GUIStyle style, params GUILayoutOption[] opts) {
			GUILayout.BeginVertical(content,style,opts);	
			align = alignment;
			SpaceIfNeeded();
		}
		public Col(Layout.VAlign alignment, Texture image,			GUIStyle style, params GUILayoutOption[] opts) {
			GUILayout.BeginVertical(image,style,opts);
			align = alignment;	
			SpaceIfNeeded();
		}
		public Col(Layout.VAlign alignment = Layout.VAlign.None) {
			GUILayout.BeginVertical();
			align = alignment;
			SpaceIfNeeded();
		}
		private void SpaceIfNeeded() {
			switch (align) {
				case Layout.VAlign.Bottom:
				case Layout.VAlign.Middle:	GUILayout.FlexibleSpace(); break;
			}

		}
		void IDisposable.Dispose() {
			switch (align) {
				case Layout.VAlign.Top:
				case Layout.VAlign.Middle:	GUILayout.FlexibleSpace(); break;
			}
			GUILayout.EndVertical();
		}
	}
	public class UsageExample {
	
		public void DrawSomething() {
			using (new Area(new Rect(0,0,Screen.width,Screen.height)) )
			using (new Row (Layout.HAlign.Center))
			using (new Col (Layout.VAlign.Middle)) {
				GUILayout.Label("This label is in the center",GUILayout.ExpandWidth(false),GUILayout.ExpandHeight(false));
			}
		}
	}
}

