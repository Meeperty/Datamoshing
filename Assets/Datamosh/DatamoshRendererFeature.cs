using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DatamoshRendererFeature : ScriptableRendererFeature
{
	private DatamoshRenderPass dmPass;
	[SerializeField]
	private Material dmMaterial;
	[SerializeField]
	private Shader dmShader;

	private RTHandle previousFrameTexHandle;

	private RTHandle previousColors;

	public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
	{
		if (renderingData.cameraData.cameraType == CameraType.Game)
			renderer.EnqueuePass(dmPass);
	}

	public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
	{
		if (renderingData.cameraData.cameraType == CameraType.Game)
		{
			dmPass.ConfigureInput(ScriptableRenderPassInput.Color | ScriptableRenderPassInput.Motion);
			dmPass.SetTarget(renderer.cameraColorTargetHandle);
		}
	}

	public override void Create()
	{
		RTHandles.Initialize(Screen.width, Screen.height);
		RTHandles.SetReferenceSize(Screen.width, Screen.height);
		previousFrameTexHandle = RTHandles.Alloc(Screen.width, Screen.height);
		dmPass = new DatamoshRenderPass(dmMaterial, previousFrameTexHandle);

		Application.targetFrameRate = 60;
	}
}
