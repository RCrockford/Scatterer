using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Runtime;
using KSP;
using KSP.IO;
using UnityEngine;

namespace scatterer
{
	public static class Utils
	{
		public static GameObject GetMainMenuObject(string name)
		{
			GameObject kopernicusMainMenuObject = GameObject.FindObjectsOfType<GameObject>().FirstOrDefault
				(b => b.name == (name+"(Clone)") && b.transform.parent.name.Contains("Scene"));
			
			if (kopernicusMainMenuObject != null)
				return kopernicusMainMenuObject;
			
			GameObject kspMainMenuObject = GameObject.FindObjectsOfType<GameObject>().FirstOrDefault(b => b.name == name && b.transform.parent.name.Contains("Scene"));
			
			if (kspMainMenuObject == null)
			{
				throw new Exception("No correct main menu object found for "+name);
			}
			
			return kspMainMenuObject;
		}

		public static Transform GetScaledTransform (string body)
		{
			return (ScaledSpace.Instance.transform.FindChild (body));	
		}

		//Add debugging levels?
		public static void Log(string log)
		{
			Debug.Log ("[Scatterer] " + log);
		}
	}
}

