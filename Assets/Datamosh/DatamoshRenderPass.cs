using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEditor;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine.Experimental.Rendering.RenderGraphModule;
using UnityEngine.Experimental.Rendering;

public class DatamoshRenderPass : ScriptableRenderPass
{
	private ProfilingSampler pSampler = new("Datamosh");
	
    private Material mat;

    private RTHandle dmTexHandle;

	private RTHandle cameraColorHandle;

	private RTHandle prevFrameTexHandle;
	private RenderTextureDescriptor cameraDescriptor;

	private bool isFirstFrame = true;

    public DatamoshRenderPass(Material mat, RTHandle frameCarryoverTexture)
    {
        this.mat = mat;
		renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
		prevFrameTexHandle = frameCarryoverTexture;
    }

	public void SetTarget(RTHandle colorHandle)
	{
		cameraColorHandle = colorHandle;
	}

	public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
	{
		cameraColorHandle = renderingData.cameraData.renderer.cameraColorTargetHandle;
		ConfigureTarget(new RTHandle[] { cameraColorHandle, dmTexHandle });
	}

	public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
	{
		RenderTextureDescriptor desc = new(cameraTextureDescriptor.width, cameraTextureDescriptor.height);

		dmTexHandle = RTHandles.Alloc(desc);
		//RenderingUtils.ReAllocateIfNeeded(ref dmTexHandle, desc, name:"dmTex");

		//RenderingUtils.ReAllocateIfNeeded(ref prevFrameTexHandle, cameraTextureDescriptor, name:"prevFrameTexHandle");
		cameraDescriptor = cameraTextureDescriptor;

		Debug.Log("Configure");
	}

	public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
	{
		Debug.Log($"Executing (first frame {isFirstFrame})");
		CommandBuffer cmd = CommandBufferPool.Get();

		RTHandle camTexHandle = renderingData.cameraData.renderer.cameraColorTargetHandle;

		using (new UnityEngine.Rendering.ProfilingScope(cmd, pSampler)) 
		{
		if (isFirstFrame)
		{
			SetTarget(prevFrameTexHandle);
			Blitter.BlitCameraTexture(cmd, camTexHandle, prevFrameTexHandle, mat, 1);
			
			isFirstFrame = false;
		}
		else if (!prevFrameTexHandle.IsUnityNull())
		{
			Blitter.BlitCameraTexture(cmd, prevFrameTexHandle, dmTexHandle, mat, 1); // simple blit from camera to temp
			Blitter.BlitCameraTexture(cmd, dmTexHandle, camTexHandle, mat, 0); // datamosh from temp to camera
			//CoreUtils.DrawFullScreen(cmd, mat, shaderPassId:0);
			
			//RenderingUtils.ReAllocateIfNeeded(ref prevFrameTexHandle, cameraDescriptor, name:"previousFrame");
			
			Blitter.BlitCameraTexture(cmd, camTexHandle, prevFrameTexHandle, mat, 1); // simple blit from camera to prev, to be used next frame
		}
		}

		context.ExecuteCommandBuffer(cmd);
		cmd.Clear();
		CommandBufferPool.Release(cmd);
		dmTexHandle.Release();
	}
}
