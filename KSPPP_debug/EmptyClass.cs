using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using KSP;
using UnityEngine;

[assembly: KSPAssembly("KSDR", 0, 1)]

namespace KSDR
{
	[KSPAddon(KSPAddon.Startup.Instantly,true)]
	public class KSDRManager : MonoBehaviour
	{
		public Dictionary<string, AppDomain> ADDict = new Dictionary<string, AppDomain>(StringComparer.OrdinalIgnoreCase);
		//List<KSDRData> aList = new List<KSDRData>();
		//KSDRProxy proxy;

		public void Awake()
		{   
			string dir = Directory.GetCurrentDirectory();
			//proxy = new KSDRProxy(this);
			ADDict.Add("Main", AppDomain.CurrentDomain);
			Debug.Log("Proxy loaded, Main AD ref'd. Dir: " + dir);          
			string SpawnFile = Directory.GetFiles(dir, "KSDRS.dll",SearchOption.AllDirectories)[0];
			byte[] sa = File.ReadAllBytes(SpawnFile);
			Assembly SpawnAssy = AppDomain.CurrentDomain.Load(sa);
			Type SpawnType = SpawnAssy.GetExportedTypes()[0];            
			object spawn = Activator.CreateInstance(SpawnType);
			MethodInfo spawnAD = SpawnType.GetMethod("createAD");
			//Delegate spawnAD = Delegate.CreateDelegate(SpawnType,spawn,"createAD");
			Debug.Log("tried a bunch of fancy reflection. ^Results so far...^");
			object[] o = {"KSDR Shared Domain"};
			AppDomain sharedAD = (AppDomain)spawnAD.Invoke(spawn,o);
			Debug.Log("Created the shared AD using reflection");
			//AppDomain sharedAD = proxy.createAD("KSDR Shared Domain");
			//ADDict.Add("Shared", sharedAD);
			// Debug.Log("Created the shared AD");
			//AppDomain reflectAD = proxy.createAD("Reflect");
			Debug.Log("Created the reflection AD");


			Debug.Log("Got the directory: " + dir);
			string[] files = Directory.GetFiles(dir, "*.KSD", SearchOption.AllDirectories);
			Debug.Log("Got the '.ksd's: " + files);
			List<byte[]> bas = new List<byte[]>();
			foreach (string file in files)
			{
				bas.Add(File.ReadAllBytes(file));
			}
//			KSDRReflector reflect = (KSDRReflector)reflectAD.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, "KSDRReflector");
//			Debug.Log("Created the reflector");
//			aList.AddRange(reflect.reflectAssemblies(bas));
//			Debug.Log("Reflected the info out");
//			AppDomain.Unload(reflectAD);
//			Debug.Log("Unloaded the reflection AD (That's all we cover so far)");
//			foreach (KSDRData d in aList)
//			{
//				switch (d.loadInto)
//				{
//				case Domain.Shared:
//					{
//						//load into sharedAD
//						break;
//					}
//				case Domain.Main:
//					{
//						//load into AppDomain.CurrentDomain
//						break;
//					}
//				case Domain.Mod:
//					{
//						//check if ADdict has an AppDomain for that
//						//either create one, or load into that one
//						break;
//					}
//				case Domain.Individual:
//					{
//						//Create a new Appdomain for this one
//						break;
//					}
//				default:
//					{
//						Debug.LogError("Mod attempted to load with KSDR had no loadInto set");
//						break;
//					}
//				}

//				//Any cleanup operations
//			}
		}

		/// <summary>
		/// The unity-proxy for the KSDR system logging.
		/// </summary>
		/// <param name="o">The object (usually a string) to be logged</param>
		internal void Log(object o)
		{
			Debug.Log(o);
		}

		public static void Main() {

		}
	}
}
