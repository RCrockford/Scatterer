using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace scatterer
{
	
	public class BufferManager : MonoBehaviour
	{
		public RenderTexture depthTexture;			//full-scene depth texture, from merged built-in depth textures of the two local cameras
		public RenderTexture refractionTexture;		//textures for the refractions, created once and accessed from here, written to from oceanNode
		//public RenderTexture occlusionTexture;	//for SSAO and eclipses, for now will just contain a copy of the screenspace shadowmask, probably not necessary
		Coroutine checkTexturesCoroutine;

		public bool depthTextureCleared = false; 	//clear depth texture when away from PQS, for the sunflare shader

		public void Awake()
		{
			CreateTextures();
			checkTexturesCoroutine = StartCoroutine (CheckTexturesAreCreated());
		}

		public void CreateTextures() //create simpler method createTexture with params, call it multiple times, make it static and move it to utils, reuse in skynode as well
		{
			if (HighLogic.LoadedScene != GameScenes.TRACKSTATION)
			{
				if (!Scatterer.Instance.unifiedCameraMode)
				{
					depthTexture = Utils.CreateTexture ("ScattererDepthTexture", Screen.width, Screen.height,0, RenderTextureFormat.RFloat, false, FilterMode.Point, 1);
				}
				
				if (Scatterer.Instance.mainSettings.useOceanShaders && Scatterer.Instance.mainSettings.oceanRefraction)
				{
					refractionTexture = Utils.CreateTexture ("ScattererRefractionTexture", Screen.width, Screen.height,0, RenderTextureFormat.ARGB32,false,FilterMode.Point,1);
				}
			}
		}

		//every 10 seconds we check the textures are still created
		//not sure we still need this, the alt-enter bug that destroys rendertextures seems to be gone
		IEnumerator CheckTexturesAreCreated()
		{
			while (true)
			{
				yield return new WaitForSeconds (10f);

				if ((!Scatterer.Instance.unifiedCameraMode && (!depthTexture || !depthTexture.IsCreated ())) ||
				    (Scatterer.Instance.mainSettings.useOceanShaders && Scatterer.Instance.mainSettings.oceanRefraction && (!refractionTexture || !refractionTexture.IsCreated ())))
				{

					Utils.LogDebug ("BufferRenderingManager: Recreating textures");
					CreateTextures ();
					Scatterer.Instance.onRenderTexturesLost();
				}
			}
		}

		public void ClearDepthTexture()
		{
			if (!Scatterer.Instance.unifiedCameraMode)
			{
				RenderTexture rt = RenderTexture.active;
				RenderTexture.active = depthTexture;			
				GL.Clear (false, true, Color.white);
				RenderTexture.active = rt;			
			}
			depthTextureCleared = true;
		}

		public void OnDestroy ()
		{
			if (depthTexture)
			{
				depthTexture.Release ();
				UnityEngine.Object.Destroy (depthTexture);
			}
			if (refractionTexture)
			{
				refractionTexture.Release ();
				UnityEngine.Object.Destroy (refractionTexture);
			}
			if (!ReferenceEquals(checkTexturesCoroutine,null))
			{
				StopCoroutine(checkTexturesCoroutine);
			}
		}

	}
}