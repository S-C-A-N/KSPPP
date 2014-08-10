using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using Mono.CSharp;
using KSPPP;
using Event = UnityEngine.Event;

namespace KSPPP
{
	public class DelegateReportPrinter : ReportPrinter
	{
		public delegate void OnPrintDelegate(AbstractMessage msg, bool showFullPath);

		public event OnPrintDelegate OnPrint;

		public DelegateReportPrinter(OnPrintDelegate func)
		{
			OnPrint += func;
		}

		public override void Print(AbstractMessage msg, bool showFullPath)
		{
			OnPrint(msg, showFullPath);
		}
	}

	[KSPAddon(KSPAddon.Startup.Instantly, true)]
	public class EmbeddedCSharp : MonoBehaviour 
	{
		public string code = "public class Program {\n\tpublic static string Test() {\n\t\treturn \"Hello world!\";\n\t}\n}";
		public string codeLine = "Program.Test();";
		public string incCode = "";
		public string console = "";
		public string fileName = "autorun";

		private static KeyCode hotkey = KeyCode.BackQuote;
		public List<string> history = new List<string>();

		public int historyPos = 0;

		public Rect winPos = new Rect();
		public Vector2 codePos = new Vector2();
		public Vector2 consolePos = new Vector2();
		public bool hidden = false;
		public bool coloring = true;

		Evaluator evaluator;
		static EmbeddedCSharp instance;

		FileBrowser fb;
		Texture2D whiteBG;

		void Start()
		{
			GameObject.DontDestroyOnLoad(gameObject);
			whiteBG = new Texture2D(1, 1, TextureFormat.ARGB32, false);
			whiteBG.SetPixel(0, 0, Color.white);
			whiteBG.Apply();
		}

		void FileSelectedCallback(string path)
		{
			fb = null;
			code = File.ReadAllText(path);
		}

		public void Log(AbstractMessage msg, bool showFullPath)
		{
			Log(msg.Text);
		}

		public static void Log(string msg)
		{
			instance.console += msg + "\n";
			instance.consolePos = new Vector2(0, float.PositiveInfinity);
		}

		void Update()
		{
			if (evaluator == null)
			{
				instance = this;
				CompilerSettings cs = new CompilerSettings();
				cs.AssemblyReferences = (new string[] { "System.dll", "System.Core.dll", "Assembly-CSharp.dll", "Assembly-CSharp-firstpass.dll", "UnityEngine.dll", "KSPPP.dll" }).ToList();
				//cs.Version = LanguageVersion.V_3;
				evaluator = new Evaluator(new CompilerContext(cs, new DelegateReportPrinter(Log)));
				evaluator.ReferenceAssembly(Assembly.GetCallingAssembly());
				/*
                foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                {
                    console += "Loading assembly: " + a.FullName + "\n";
                    evaluator.ReferenceAssembly(a);
                }
                */

				if (File.Exists(KSP.IO.IOUtils.GetFilePathFor(typeof(EmbeddedCSharp), "autorun.cs")))
				{
					code = File.ReadAllText(KSP.IO.IOUtils.GetFilePathFor(typeof(EmbeddedCSharp), "autorun.cs"));
				}
			}
			if (Input.GetKeyDown(hotkey))
				hidden = !hidden;
			//if (Input.GetKeyDown(KeyCode.Alpha8) && (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))) { hidden = !hidden; }
		}

		void OnGUI()
		{
			if (!hidden)
			{
				GUI.skin = AssetBase.GetGUISkin("KSP window 7");
				winPos = GUILayout.Window(8935, winPos, WindowGUI, "MuMech Embedded C# Console");
				if (fb != null)
				{
					fb.OnGUI();
				}
			}
		}

		void WindowGUI(int wID)
		{
			GUILayout.BeginHorizontal();

			fileName = GUILayout.TextField(fileName, GUILayout.ExpandWidth(true));

			#region save button
			if (GUILayout.Button("Save", GUILayout.Width(50)))
			{
				File.WriteAllText(KSP.IO.IOUtils.GetFilePathFor(typeof(EmbeddedCSharp), fileName + ".cs"), code);
			}
			#endregion
			#region load button
			if (GUILayout.Button("Load", GUILayout.Width(50)))
			{
				fb = new FileBrowser(new Rect(Screen.width / 4, Screen.height / 4, Screen.width / 2, Screen.height / 2), "Load .cs file", FileSelectedCallback);
				fb.BrowserType = FileBrowserType.File;
				fb.CurrentDirectory = KSP.IO.IOUtils.GetFilePathFor(typeof(EmbeddedCSharp), "");
				fb.SelectionPattern = "*.cs";
			}
			#endregion

			GUILayout.EndHorizontal();

			#region code entry
			codePos = GUILayout.BeginScrollView(codePos, GoE(), GoS());

			if (Event.current.type == EventType.KeyDown)
			{
				switch (Event.current.keyCode)
				{
				case KeyCode.F1:
					coloring = !coloring;
					Event.current.Use();
					break;
				}
			}
			GUIStyle textStyle = new GUIStyle();
			textStyle.normal.textColor = Color.white;
			textStyle.wordWrap = false;
			textStyle.richText = true;

			if (coloring)
			{
				/*
                print("Pre - code: " + code);
                print("Pre - formatted: " + formattedCode);
                */
				string formattedCode = "<color=#000000>" + (new ColorCode.CodeColorizer().Colorize(code, ColorCode.Languages.CSharp)) + "</color>";

				textStyle.normal.background = whiteBG;

				formattedCode = GUILayout.TextArea(formattedCode, textStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

				//code = System.Web.Util.HttpEncoder.HtmlDecode(Regex.Replace(formattedCode, @"<[^>]*(>|$)", string.Empty));
				/*
                print("Pos - formatted: " + formattedCode);
                print("Pos - code: " + code);
                */
			}
			else
			{
				code = GUILayout.TextArea(code, textStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
			}
			GUILayout.EndScrollView();
			#endregion
			#region output
			consolePos = GUILayout.BeginScrollView(consolePos, GUILayout.Width(500), GUILayout.Height(100));
			console = GUILayout.TextArea(console, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
			GUILayout.EndScrollView();
			#endregion


			GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
			#region input
			if (Event.current.type == EventType.KeyDown)
			{
				switch (Event.current.keyCode)
				{
				case KeyCode.Return:
					RunCode();
					Event.current.Use();
					break;
				case KeyCode.UpArrow:
					if (historyPos < history.Count)
					{
						historyPos++;
						codeLine = history[history.Count - historyPos];
					}
					Event.current.Use();
					break;
				case KeyCode.DownArrow:
					if (historyPos > 0)
					{
						historyPos--;
						if (historyPos <= 0)
						{
							codeLine = "";
						}
						else
						{
							codeLine = history[history.Count - historyPos];
						}
					}
					Event.current.Use();
					break;
				}
			}
			codeLine = GUILayout.TextField(codeLine, GUILayout.ExpandWidth(true));
			#endregion
			#region execute button
			if (GUILayout.Button("Execute", GUILayout.Width(75)))
			{
				RunCode();
			}
			#endregion
			GUILayout.EndHorizontal();
			GUI.DragWindow();
		}

		void RunCode()
		{
			try
			{
				object result;
				bool result_set;
				evaluator.Evaluate(code, out result, out result_set);

				if (result_set) {
					switch (result == null) {
					case true:	Log("null"); break;
					default:	Log(result.ToString()); break;
					}
				}
				if (codeLine.Trim().Length > 0) {
					if (history.LastOrDefault() != codeLine) { history.Add(codeLine); }
					historyPos = 0;
					Log(codeLine);

					incCode = evaluator.Evaluate(incCode + codeLine, out result, out result_set);

					if (incCode == null) { incCode = ""; }

					if (result_set) {
						switch (result == null) {
						case true:	Log("null"); break;
						default:	Log(result.ToString()); break;
						}
					}
					codeLine = "";
				}
				string[] lines = console.Split('\n');
				if (lines.Length > 50)
				{
					console = String.Join("\n", lines.Skip(lines.Length - 50).ToArray());
					consolePos = new Vector2(0, float.PositiveInfinity);
				}
			}
			catch (Exception e)
			{
				Log("Exception: " + e.Message);
			}
		}
	}
}