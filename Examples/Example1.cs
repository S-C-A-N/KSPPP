using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using KSP;
using UnityEngine;
using KSPPP;

using Log = KSPPP.Logging.ConsoleLogger;

namespace SkinsAhoy
{
	[KSPAddon (KSPAddon.Startup.MainMenu, false)]
	public class SkinsAhoy : MBE
	{
		SkinsMainWindow Main;
		protected override void Awake ()
		{
			Main = gameObject.AddComponent<SkinsMainWindow> ();
		}

		protected override void OnGUI_FirstRun ()
		{
			GUISkin skinCustom = SkinsLibrary.CopySkin ("Unity");
			skinCustom.button = SkinsLibrary.DefKSPSkin.button;
			SkinsLibrary.AddSkin ("CustomSkin", skinCustom);

			GUIStyle redButton = new GUIStyle (SkinsLibrary.DefKSPSkin.button);
			redButton.name = "RedButton";
			redButton.normal.textColor = Color.red;
			redButton.hover.textColor = Color.red;
            
			SkinsLibrary.AddStyle (redButton, "KSP");
			SkinsLibrary.AddStyle (redButton, "CustomSkin");

			redButton = new GUIStyle (SkinsLibrary.DefUnitySkin.button);
			redButton.name = "RedButton";
			redButton.normal.textColor = Color.red;
			redButton.hover.textColor = Color.red;
			//SkinsLibrary.AddStyle(redButton, SkinsLibrary.DefSkinType.Unity);
			SkinsLibrary.AddStyle (redButton, "Unity");

			GUIStyle CustomTooltip = new GUIStyle ();
			CustomTooltip.name = "Tooltip";
			CustomTooltip.normal.textColor = Color.yellow;
			SkinsLibrary.AddStyle (CustomTooltip, "CustomSkin");
			
			
			// let's try to manually fix VSlider and HScrollbar in KSP's skin
			var vs			= SkinsLibrary.DefKSPSkin.verticalSlider;
			var vst			= SkinsLibrary.DefKSPSkin.verticalSliderThumb;
			var hSlider		= SkinsLibrary.DefKSPSkin.horizontalSlider;
			var hSliderThumb= SkinsLibrary.DefKSPSkin.horizontalSliderThumb;
			
			vs.normal = hSlider.normal;
			vs.border = hSlider.border;
			vs.padding = hSlider.padding;
			vs.margin = hSlider.margin;
			vs.overflow = hSlider.overflow;
			
			vst.active = hSliderThumb.active;
			vst.normal = hSliderThumb.normal;
			vst.hover = hSliderThumb.hover;
			vst.border = hSliderThumb.border;
			vst.padding = hSliderThumb.padding;
			vst.margin = hSliderThumb.margin;
			vst.overflow = hSliderThumb.overflow;
			vst.fixedWidth = 12;
			vst.stretchWidth = false;
			vst.fixedHeight = 28.1975f;
			vst.stretchHeight = true;
			
			var hsb = SkinsLibrary.DefKSPSkin.horizontalScrollbar;
			var hsbt = SkinsLibrary.DefKSPSkin.horizontalScrollbarThumb;
			
			hsb.normal = hSlider.normal;
			hsb.border = hSlider.border;
			hsb.padding = hSlider.padding;
			hsb.margin = hSlider.margin;
			hsb.overflow = hSlider.overflow;
			
			hsbt.active = hSliderThumb.active;
			hsbt.normal = hSliderThumb.normal;
			hsbt.hover = hSliderThumb.hover;
			hsbt.border = hSliderThumb.border;
			hsbt.padding = hSliderThumb.padding;
			hsbt.margin = hSliderThumb.margin;
			hsbt.overflow = hSliderThumb.overflow;
			hsbt.fixedWidth = 28.1975f;
			hsbt.stretchWidth = true;
			hsbt.fixedHeight = 12;
			hsbt.stretchHeight = false;
			
			var vsb = SkinsLibrary.DefKSPSkin.verticalScrollbar;
			var vsbt = SkinsLibrary.DefKSPSkin.verticalScrollbarThumb;
			
			vsb.normal = hSlider.normal;
			vsb.border = hSlider.border;
			vsb.padding = hSlider.padding;
			vsb.margin = hSlider.margin;
			vsb.overflow = hSlider.overflow;
			
			vsbt.active = hSliderThumb.active;
			vsbt.normal = hSliderThumb.normal;
			vsbt.hover = hSliderThumb.hover;
			vsbt.border = hSliderThumb.border;
			vsbt.padding = hSliderThumb.padding;
			vsbt.margin = hSliderThumb.margin;
			vsbt.overflow = hSliderThumb.overflow;
			vsbt.fixedWidth = 12;
			vsbt.stretchWidth = false;
			vsbt.fixedHeight = 28.1975f;
			vsbt.stretchHeight = true;
			
		}
	}

	internal class SkinsMainWindow : MBW
	{
		protected override void Awake ()
		{
			Visible = true;
			WindowCaption = "Main Window";
			WindowRect = new Rect (0, 0, 300, 400);
			TooltipsEnabled = true;
			DragEnabled = true;
			skinsWindows = new List<SkinsWindow> ();
			looperWindows = new List<LooperWindow> ();
			iconWindows = new List<IconWindow> ();
		}
		private List<SkinsWindow> skinsWindows;
		private List<LooperWindow> looperWindows;
		private List<IconWindow> iconWindows;

		protected override void DrawWindow (int id)
		{
			GUILayout.Label ("Choose a Skin");
			if (GUILayout.Button (textWithTT ("KSP Style", "Sets the style to be the default KSPStyle.")))
				SkinsLibrary.SetCurrent ("KSP");
			if (GUILayout.Button (textWithTT ("Unity Style", "Sets the style to be the default Unity Style.")))
				SkinsLibrary.SetCurrent ("Unity");
			if (GUILayout.Button (textWithTT ("Custom Style", "Sets the style to be a custom style I just made up.")))
				SkinsLibrary.SetCurrent ("CustomSkin");


			GUILayout.Space (20);
			if (GUILayout.Button (textWithTT ("Open Skins Window", "Opens a window at a random location."))) {
				SkinsWindow winTemp = gameObject.AddComponent<SkinsWindow> ();
				skinsWindows.Add (winTemp);
			}
			if (GUILayout.Button (textWithTT ("Destroy Skins Window", "Close a window (picks oldest first?)."))) {
				if (skinsWindows.Count > 0) {
					skinsWindows [0].Visible = false;
					skinsWindows [0] = null;
					skinsWindows.RemoveAt (0);
				}
			}
			if (GUILayout.Button (textWithTT ("Open Timer Window", "Opens a window at a random location."))) {
				LooperWindow winTemp = gameObject.AddComponent<LooperWindow> ();
				looperWindows.Add (winTemp);
			}
			if (GUILayout.Button (textWithTT ("Destroy Timer Window", "Destroy one of the Timer Windows"))) {
				if (looperWindows.Count > 0) {
					looperWindows [0].Visible = false;
					looperWindows [0] = null;
					looperWindows.RemoveAt (0);
				}
			}

			if (GUILayout.Button (textWithTT ("Open Icons Window", "Opens a window at a random location."))) {
				IconWindow winTemp = gameObject.AddComponent<IconWindow> ();
				iconWindows.Add (winTemp);
			}
			if (GUILayout.Button (textWithTT ("Destroy Icons Window", "Destroy one of the Icon Windows"))) {
				if (iconWindows.Count > 0) {
					iconWindows [0].Visible = false;
					iconWindows [0] = null;
					iconWindows.RemoveAt (0);
				}
			}

			if (GUILayout.Button (textWithTT ("Toggle Drag for all Windows", "Except this window, evidently."))) {
				foreach (SkinsWindow sw in skinsWindows) sw.DragEnabled = !sw.DragEnabled;
				foreach (LooperWindow lw in looperWindows) lw.DragEnabled = !lw.DragEnabled;
				foreach (IconWindow iw in iconWindows) iw.DragEnabled = !iw.DragEnabled;
			}
			if (GUILayout.Button (textWithTT ("Toggle Resize for all Windows", "This allows you to resize the window down to a minimum size and up to a maximum size."))) {
				foreach (SkinsWindow sw in skinsWindows) sw.ResizeEnabled = !sw.ResizeEnabled;
				foreach (LooperWindow lw in looperWindows) lw.ResizeEnabled = !lw.ResizeEnabled;
				foreach (IconWindow iw in iconWindows) iw.ResizeEnabled = !iw.ResizeEnabled;
			}
			if (GUILayout.Button (textWithTT ("Toggle Clamping for \" \" ", "Clamping prevents windows from going off of the screen."))) {
				foreach (SkinsWindow sw in skinsWindows) sw.ClampEnabled = !sw.ClampEnabled;
				foreach (LooperWindow lw in looperWindows)  lw.ClampEnabled = !lw.ClampEnabled;
				foreach (IconWindow iw in iconWindows) iw.ClampEnabled = !iw.ClampEnabled;
			}
			if (GUILayout.Button (textWithTT ("Toggle Tooltips for \" \" ", "Tooltips are hovering help boxes which appear on mouseover."))) {
				foreach (SkinsWindow sw in skinsWindows) sw.TooltipsEnabled = !sw.TooltipsEnabled;
				foreach (LooperWindow lw in looperWindows) lw.TooltipsEnabled = !lw.TooltipsEnabled;
				foreach (IconWindow iw in iconWindows) iw.TooltipsEnabled = !iw.TooltipsEnabled;

			}
			
			GUILayout.Label ("Tooltipwidth. 0 means nowrap");
			var a = GUILayout.TextField (TooltipMaxWidth.ToString ());
			TooltipMaxWidth = Convert.ToInt32 (a);
		}
	}

	internal class SkinsWindow : MBW
	{
		protected override void Awake ()
		{
			WindowCaption = "Test Window";
			Visible = true;
			WindowRect = new Rect (UnityEngine.Random.Range (100, 800), UnityEngine.Random.Range (100, 800), 300, 300);
			ClampEnabled = true;
			DragEnabled = true;
			TooltipsEnabled = true;
			ResizeEnabled = true;
						
			foreach (CelestialBody body in FlightGlobals.Bodies) {
				bodyNames.Add (body.bodyName);
				bodyNameDesc.Add (textWithTT (body.bodyName, body.bodyDescription));
				Log.Now ("adding body name: {0}", body.bodyName);
			}


		}

		float horizvalue = 50;
		float vertvalue = 0;
		float hslider = 50;
		float vslider = 100;
		string pw = "";
		string text = "This is a text area, presumably with sliders and other nice features.\nThis is another line.\nAnd another.";
		float pairedhslider = 0;
		Vector2 pos = new Vector2 ();
		int toolbarA = 0;
		int toolbarB = 1;
		List<string> bodyNames = new List<string> ();
		List<GUIContent> bodyNameDesc = new List<GUIContent> ();

		
		int selectedBody = 0;
		int selectedBody2 = 0;


		Boolean togglevalue = false;
		protected override void DrawWindow (int id)
		{
			
			/*	These items support being passed GUIContent, and thus will support toolips:
		 *		(a * beside means we have an example of this tooltip working)
		 * 
		 *	BeginArea
		 *	BeginHorizontal *
		 *	BeginVertical *
		 *
		 *	Box
		 *	Button *
		 *	Label *
		 *	RepeatButton *
		 *	SelectionGrid (has GUIContent[], actually) *
		 *	Toggle *
		 *	Toolbar (has GUIContent[]) *
		 *	Window
		 *
		 *	This means that some things will not ever have tooltips:
		 *
		 *	BeginScrollView
		 *	HorizontalScrollbar
		 *	HorizontalSlider
		 *	VerticalScrollbar
		 *	VerticalSlider
		 *	Passwordfield
		 *	TextArea
		 *	TextField
		 *
		 */

			// TODO: 1
			GUILayout.Button (textWithTT ("This is a button", "the first button in this window."));

			// TODO: 2
			togglevalue = GUILayout.Toggle (togglevalue, textWithTT ("This is a Toggle", "this is the second (but first toggle) button in this window"));

			// TODO: 3
			GUILayout.Label (textWithTT ("This is a Label", "With a secondary label!"));
				pos = GUILayout.BeginScrollView (pos, GoS (), GoE ());
					text = GUILayout.TextArea (text, GoS (), GoE ());
				GUILayout.EndScrollView ();

			// TODO: 4
			growE (textWithTT ("", "horizontal section"), "box");
				growS (textWithTT ("", "these two sliders are uncoupled"), "box");

			// TODO: 5
					horizvalue = GUILayout.HorizontalScrollbar (horizvalue, 0, 0, 100);
			// TODO: 6
					hslider = GUILayout.HorizontalSlider (hslider, 0, 100);

				stopE (); 
				growE (textWithTT ("", "these two sliders are coupled"), "box");
			// TODO: 7
					pairedhslider = GUILayout.HorizontalScrollbar (pairedhslider, 0, 0, 100);
					pairedhslider = GUILayout.HorizontalSlider (pairedhslider, 0, 100);

				stopS ();
				growE ();
					growE ();
			// TODO: 8
						vertvalue = GUILayout.VerticalScrollbar (vertvalue, 0, 0, 100);
						vslider	= GUILayout.VerticalSlider (vslider, 0, 100);
					stopE ();
				stopS ();
			stopE ();

			// TODO: 9
			GUILayout.Label ("HScrollbar Value=" + horizvalue.ToString ("0"));
			GUILayout.Label ("HSlider Value=" + hslider.ToString ("0"));
			GUILayout.Label ("VScrollbar Value=" + vertvalue.ToString ("0"));
			GUILayout.Label ("VSlider Value=" + vslider.ToString ("0"));

			// TODO: 10
			var repeat = GUILayout.RepeatButton (textWithTT ("hold me down", "this will be true as long as the button is held"));
			GUILayout.Label ("RepeatButton Value =" + repeat.ToString ());

			// TODO: 11
			var toolbarEntries = new string[] { "first", "second", "third" };
			var toolbarWithTT = new GUIContent[] {
				textWithTT ("first", "the first button"),
				textWithTT ("second", "the second button"),
				textWithTT ("third", "the third button")
			};
			
			// TODO: 12
			toolbarA = GUILayout.Toolbar (toolbarA, toolbarEntries);
			toolbarB = GUILayout.Toolbar (toolbarB, toolbarWithTT);
			GUILayout.Label ("ToolbarA selection:" + toolbarA.ToString ());
			GUILayout.Label ("ToolbarB selection:" + toolbarB.ToString ());


			// TODO: 13
			//selectedBody = GUILayout.SelectionGrid (selectedBody, bodyNames.ToArray (),5);
			//GUILayout.Label ("Selected Body: " + bodyNames[selectedBody].ToString ());

			selectedBody2 = GUILayout.SelectionGrid (selectedBody2, bodyNameDesc.ToArray (), 5);
			GUILayout.Label ("Selected Body: " + bodyNameDesc [selectedBody2].text.ToString ());



			// TODO: 14
			pw = GUILayout.PasswordField (pw, '*');
			GUILayout.Label ("Password Value=" + pw);

			// TODO: 15
			GUILayout.Button (textWithTT ("red text button", "this button is red no matter what skin"), "RedButton");
			GUILayout.Button (textWithTT ("Unity button", "this button is Unity skinned no matter what"), SkinsLibrary.DefUnitySkin.button);

			// TODO: 16
			GUILayout.Label ("DragEnabled:" + DragEnabled.ToString ());
		}
	}
    
	//[KSPAddon(KSPAddon.Startup.MainMenu, false)]
	internal class LooperWindow : MBW
	{
		protected override void Awake ()
		{
			//this just shows the window
			WindowRect = new Rect (300, 0, 300, 200);
			Visible = true;
			ResizeEnabled = false;
			TooltipsEnabled = true;
			DragEnabled = true;
			//this is a wrapper for invokerepeating - once a second
			StartLooper (UnityEngine.Random.Range (1, 10));
		}

		protected override void Looper ()
		{
			//hsa the smoothstep run past its duration
			if ((Time.time - SmoothStart) > SmoothDuration) {
				//record the start of the smoothing
				SmoothStart = Time.time;
				//get a new destination and start stepping
				windMinimum = 0.0f;
				windMax = 6.0f;

				//store the final value as the new start
				windInitial = windFinal;
				//generate a new final value
				windFinal = UnityEngine.Random.Range (windMinimum, windMax);
			}

			//step closer
			//How far through the smooth duration are we
			float StepFraction = (Time.time - SmoothStart) / SmoothDuration;
			//now change the force
			windForce = UnityEngine.Mathf.SmoothStep (windInitial, windFinal, StepFraction);
		}

		internal float SmoothStart = 0f;
		internal float SmoothDuration = 5f;
		internal float windMinimum = 0f;
		internal float windMax = 0f;
		internal float windInitial = 0f;
		internal float windFinal = 0f;
		internal float windForce = 0f;

		protected override void DrawWindow (int id)
		{
			GUILayout.BeginVertical ();
			GUILayout.Label (textWithTT (string.Format ("{0}", SmoothStart), "SmoothStart: the time after game start this timer was launched."));
			GUILayout.Label (textWithTT (string.Format ("{0}", SmoothDuration), "SmoothDuraction: currently hardcoded to 5."));
			GUILayout.Label (textWithTT (string.Format ("{0}", (Time.time - SmoothStart)), "Time since SmoothStart"));
			GUILayout.Label (textWithTT (string.Format ("{0}", windInitial), "windInitial: ???"));
			GUILayout.Label (textWithTT (string.Format ("{0}", windForce), "windForce: ???"));
			GUILayout.Label (textWithTT (string.Format ("{0}", windFinal), "windFinal:"));
			GUILayout.EndVertical ();
		}
	}

	internal class IconWindow : MBW {
		List<GUIContent> icons = new List<GUIContent>();
		List<GUIContent> small = new List<GUIContent>();
		List<GUIContent> planets = new List<GUIContent>();
		List<GUIContent> moons = new List<GUIContent>();

		int selectedIcon = 0;
		int selectedSmallIcon = 0;
		int selectedPlanet = 0;
		int selectedMoon = 0;

		bool useSmall = false;

		protected override void Awake ()
		{
			WindowCaption = "Icon Window";
			Visible = true;
			WindowRect = new Rect (UnityEngine.Random.Range (100, 800), UnityEngine.Random.Range (100, 800), 300, 300);
			ClampEnabled = true;
			DragEnabled = true;
			TooltipsEnabled = true;
			ResizeEnabled = true;

			Icon.loadAssets();

			foreach (Icons i in (Icons[]) Enum.GetValues(typeof(Icons))) {
				var total = Enum.GetNames(typeof(Icons)).Count() - 1;

				Texture2D norm = new Texture2D(32,32,TextureFormat.ARGB32,false);

				var pix = Icon.octIcons.GetPixels(0,32*(int)(total - i),32,32);

				norm.SetPixels(0,0,32,32,pix);
				norm.Apply(false);

				// create smaller versions, too
				var newtex = Instantiate (norm);
				TextureScale.Bilinear ((Texture2D) newtex, (int) norm.width/2, (int) norm.height/2);

				icons.Add(new GUIContent(norm, i.ToString() ));
				small.Add(new GUIContent((Texture2D)newtex, i.ToString() ));

				Log.Now("adding real texture for: {0}", i.ToString());
			}

			foreach (PlanetIcons i in (PlanetIcons[]) Enum.GetValues(typeof(PlanetIcons))) {
				var total = Enum.GetNames(typeof(PlanetIcons)).Count() - 1;
				var extra = Enum.GetNames(typeof(MoonIcons)).Count();
				Texture2D icon = new Texture2D(26,26,TextureFormat.ARGB32,false);

				var pix = Icon.planetIcons.GetPixels(0,26*(int)((total + extra) - i),26,26);

				icon.SetPixels(0,0,26,26,pix);
				icon.Apply(false);

				planets.Add(new GUIContent(icon, i.ToString() ));

				Log.Now("adding real texture for: {0}", i.ToString());
			}

			foreach (MoonIcons i in (MoonIcons[]) Enum.GetValues(typeof(MoonIcons))) {
				var total = Enum.GetNames(typeof(MoonIcons)).Count() - 1;
				var extra = Enum.GetNames(typeof(PlanetIcons)).Count();

				Texture2D icon = new Texture2D(26,26,TextureFormat.ARGB32,false);

				var pix = Icon.planetIcons.GetPixels(0,26*(int)((total) - i),26,26);

				icon.SetPixels(0,0,26,26,pix);
				icon.Apply(false);

				moons.Add(new GUIContent(icon, i.ToString() ));

				Log.Now("adding real texture for: {0}", i.ToString());
			}
		}

		protected override void DrawWindow(int id) {
		

			selectedPlanet = GUILayout.SelectionGrid (selectedPlanet, planets.ToArray(), 7, NoE(), NoS());
			GUILayout.Label ("Selected Planet: " + planets[selectedPlanet].tooltip.ToString());

			selectedMoon = GUILayout.SelectionGrid (selectedMoon, moons.ToArray(),11, NoE(), NoS());
			GUILayout.Label ("Selected Moon: " + moons[selectedMoon].tooltip.ToString());

			useSmall = GUILayout.Toggle(useSmall, "Show Small Icons");

			switch (useSmall) {
				case true: {
					selectedSmallIcon = GUILayout.SelectionGrid (selectedSmallIcon, small.ToArray(), 13, NoE(),NoS());
						GUILayout.Label ("Selected Small Icon: " + small[selectedSmallIcon].tooltip.ToString());
					}; break;
				case false: {
					selectedIcon = GUILayout.SelectionGrid (selectedIcon, icons.ToArray(), 13, NoE(),NoS());
						GUILayout.Label ("Selected Normal Icon: " + icons[selectedIcon].tooltip.ToString());
					}; break;
			}
		}
	}
}
