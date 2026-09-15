using System;
using UnityEngine;

// Token: 0x0200029E RID: 670
public class MirrorCameraScript : MonoBehaviour
{
	// Token: 0x0600125F RID: 4703 RVA: 0x0005D4D0 File Offset: 0x0005B6D0
	private void Start()
	{
		this.mirrorScript = base.GetComponentInParent<MirrorScript>();
		this.cameraObject = base.GetComponent<Camera>();
		if (this.mirrorScript.AddFlareLayer)
		{
			this.cameraObject.gameObject.AddComponent<FlareLayer>();
		}
		this.mirrorRenderer = this.MirrorObject.GetComponent<Renderer>();
		if (Application.isPlaying)
		{
			foreach (Material material in this.mirrorRenderer.sharedMaterials)
			{
				if (material.name == "MirrorMaterial")
				{
					this.mirrorRenderer.sharedMaterial = material;
					break;
				}
			}
		}
		this.mirrorMaterial = this.mirrorRenderer.sharedMaterial;
		this.CreateRenderTexture();
	}

	// Token: 0x06001260 RID: 4704 RVA: 0x0005D580 File Offset: 0x0005B780
	private void CreateRenderTexture()
	{
		if (this.reflectionTexture == null || this.oldReflectionTextureSize != this.mirrorScript.TextureSize)
		{
			if (this.reflectionTexture)
			{
				Object.DestroyImmediate(this.reflectionTexture);
			}
			this.reflectionTexture = new RenderTexture(this.mirrorScript.TextureSize, this.mirrorScript.TextureSize, 16);
			this.reflectionTexture.filterMode = 1;
			this.reflectionTexture.antiAliasing = 1;
			this.reflectionTexture.name = "MirrorRenderTexture_" + base.GetInstanceID().ToString();
			this.reflectionTexture.hideFlags = 61;
			this.reflectionTexture.autoGenerateMips = false;
			this.reflectionTexture.wrapMode = 1;
			this.mirrorMaterial.SetTexture("_MainTex", this.reflectionTexture);
			this.oldReflectionTextureSize = this.mirrorScript.TextureSize;
		}
		if (this.cameraObject.targetTexture != this.reflectionTexture)
		{
			this.cameraObject.targetTexture = this.reflectionTexture;
		}
	}

	// Token: 0x06001261 RID: 4705 RVA: 0x0005D69C File Offset: 0x0005B89C
	private void Update()
	{
		if (this.VRMode && Camera.current == Camera.main)
		{
			return;
		}
		this.CreateRenderTexture();
	}

	// Token: 0x06001262 RID: 4706 RVA: 0x0005D6C0 File Offset: 0x0005B8C0
	private void UpdateCameraProperties(Camera src, Camera dest)
	{
		dest.clearFlags = src.clearFlags;
		dest.backgroundColor = src.backgroundColor;
		if (src.clearFlags == 1)
		{
			Skybox component = src.GetComponent<Skybox>();
			Skybox component2 = dest.GetComponent<Skybox>();
			if (!component || !component.material)
			{
				component2.enabled = false;
			}
			else
			{
				component2.enabled = true;
				component2.material = component.material;
			}
		}
		dest.orthographic = src.orthographic;
		dest.orthographicSize = src.orthographicSize;
		if (this.mirrorScript.AspectRatio > 0f)
		{
			dest.aspect = this.mirrorScript.AspectRatio;
		}
		else
		{
			dest.aspect = src.aspect;
		}
		dest.renderingPath = src.renderingPath;
	}

	// Token: 0x06001263 RID: 4707 RVA: 0x0005D784 File Offset: 0x0005B984
	internal void RenderMirror()
	{
		Camera current;
		if (MirrorCameraScript.renderingMirror || !base.enabled || (current = Camera.current) == null || this.mirrorRenderer == null || this.mirrorMaterial == null || !this.mirrorRenderer.enabled)
		{
			return;
		}
		MirrorCameraScript.renderingMirror = true;
		int pixelLightCount = QualitySettings.pixelLightCount;
		if (QualitySettings.pixelLightCount != this.mirrorScript.MaximumPerPixelLights)
		{
			QualitySettings.pixelLightCount = this.mirrorScript.MaximumPerPixelLights;
		}
		try
		{
			this.UpdateCameraProperties(current, this.cameraObject);
			if (this.mirrorScript.MirrorRecursion)
			{
				this.mirrorMaterial.EnableKeyword("MIRROR_RECURSION");
				this.cameraObject.ResetWorldToCameraMatrix();
				this.cameraObject.ResetProjectionMatrix();
				this.cameraObject.projectionMatrix = this.cameraObject.projectionMatrix * Matrix4x4.Scale(new Vector3(-1f, 1f, 1f));
				this.cameraObject.cullingMask = -17 & this.mirrorScript.ReflectLayers.value;
				GL.invertCulling = true;
				this.cameraObject.Render();
				GL.invertCulling = false;
			}
			else
			{
				this.mirrorMaterial.DisableKeyword("MIRROR_RECURSION");
				Vector3 position = base.transform.position;
				Vector3 vector = (this.mirrorScript.NormalIsForward ? base.transform.forward : base.transform.up);
				float num = -Vector3.Dot(vector, position) - this.mirrorScript.ClipPlaneOffset;
				Vector4 vector2;
				vector2..ctor(vector.x, vector.y, vector.z, num);
				this.CalculateReflectionMatrix(ref vector2);
				Vector3 position2 = this.cameraObject.transform.position;
				float farClipPlane = this.cameraObject.farClipPlane;
				Vector3 vector3 = this.reflectionMatrix.MultiplyPoint(position2);
				Matrix4x4 matrix4x = current.worldToCameraMatrix;
				if (this.VRMode)
				{
					if (current.stereoActiveEye == null)
					{
						ref Matrix4x4 ptr = ref matrix4x;
						ptr[12] = ptr[12] + 0.011f;
					}
					else if (current.stereoActiveEye == 1)
					{
						ref Matrix4x4 ptr = ref matrix4x;
						ptr[12] = ptr[12] - 0.011f;
					}
				}
				matrix4x *= this.reflectionMatrix;
				this.cameraObject.worldToCameraMatrix = matrix4x;
				Vector4 vector4 = this.CameraSpacePlane(ref matrix4x, ref position, ref vector, 1f);
				this.cameraObject.projectionMatrix = current.CalculateObliqueMatrix(vector4);
				GL.invertCulling = true;
				this.cameraObject.transform.position = vector3;
				this.cameraObject.farClipPlane = this.mirrorScript.FarClipPlane;
				this.cameraObject.cullingMask = -17 & this.mirrorScript.ReflectLayers.value;
				this.cameraObject.Render();
				this.cameraObject.transform.position = position2;
				this.cameraObject.farClipPlane = farClipPlane;
				GL.invertCulling = false;
			}
		}
		finally
		{
			MirrorCameraScript.renderingMirror = false;
			if (QualitySettings.pixelLightCount != pixelLightCount)
			{
				QualitySettings.pixelLightCount = pixelLightCount;
			}
		}
	}

	// Token: 0x06001264 RID: 4708 RVA: 0x0005DAB0 File Offset: 0x0005BCB0
	private void OnDisable()
	{
		if (this.reflectionTexture)
		{
			Object.DestroyImmediate(this.reflectionTexture);
			this.reflectionTexture = null;
		}
	}

	// Token: 0x06001265 RID: 4709 RVA: 0x0005DAD4 File Offset: 0x0005BCD4
	private Vector4 CameraSpacePlane(ref Matrix4x4 worldToCameraMatrix, ref Vector3 pos, ref Vector3 normal, float sideSign)
	{
		Vector3 vector = pos + normal * this.mirrorScript.ClipPlaneOffset;
		Vector3 vector2 = worldToCameraMatrix.MultiplyPoint(vector);
		Vector3 vector3 = worldToCameraMatrix.MultiplyVector(normal).normalized * sideSign;
		return new Vector4(vector3.x, vector3.y, vector3.z, -Vector3.Dot(vector2, vector3));
	}

	// Token: 0x06001266 RID: 4710 RVA: 0x0005DB48 File Offset: 0x0005BD48
	private void CalculateReflectionMatrix(ref Vector4 plane)
	{
		this.reflectionMatrix.m00 = 1f - 2f * plane[0] * plane[0];
		this.reflectionMatrix.m01 = -2f * plane[0] * plane[1];
		this.reflectionMatrix.m02 = -2f * plane[0] * plane[2];
		this.reflectionMatrix.m03 = -2f * plane[3] * plane[0];
		this.reflectionMatrix.m10 = -2f * plane[1] * plane[0];
		this.reflectionMatrix.m11 = 1f - 2f * plane[1] * plane[1];
		this.reflectionMatrix.m12 = -2f * plane[1] * plane[2];
		this.reflectionMatrix.m13 = -2f * plane[3] * plane[1];
		this.reflectionMatrix.m20 = -2f * plane[2] * plane[0];
		this.reflectionMatrix.m21 = -2f * plane[2] * plane[1];
		this.reflectionMatrix.m22 = 1f - 2f * plane[2] * plane[2];
		this.reflectionMatrix.m23 = -2f * plane[3] * plane[2];
		this.reflectionMatrix.m30 = 0f;
		this.reflectionMatrix.m31 = 0f;
		this.reflectionMatrix.m32 = 0f;
		this.reflectionMatrix.m33 = 1f;
	}

	// Token: 0x06001267 RID: 4711 RVA: 0x0005DD28 File Offset: 0x0005BF28
	private static void CalculateObliqueMatrix(ref Matrix4x4 projection, ref Vector4 clipPlane)
	{
		Vector4 vector = projection.inverse * new Vector4(MirrorCameraScript.Sign(clipPlane.x), MirrorCameraScript.Sign(clipPlane.y), 1f, 1f);
		Vector4 vector2 = clipPlane * (2f / Vector4.Dot(clipPlane, vector));
		projection[2] = vector2.x - projection[3];
		projection[6] = vector2.y - projection[7];
		projection[10] = vector2.z - projection[11];
		projection[14] = vector2.w - projection[15];
	}

	// Token: 0x06001268 RID: 4712 RVA: 0x0005DDDC File Offset: 0x0005BFDC
	private static float Sign(float a)
	{
		if (a > 0f)
		{
			return 1f;
		}
		if (a < 0f)
		{
			return -1f;
		}
		return 0f;
	}

	// Token: 0x04001109 RID: 4361
	public GameObject MirrorObject;

	// Token: 0x0400110A RID: 4362
	public bool VRMode;

	// Token: 0x0400110B RID: 4363
	private Renderer mirrorRenderer;

	// Token: 0x0400110C RID: 4364
	private Material mirrorMaterial;

	// Token: 0x0400110D RID: 4365
	private MirrorScript mirrorScript;

	// Token: 0x0400110E RID: 4366
	private Camera cameraObject;

	// Token: 0x0400110F RID: 4367
	private RenderTexture reflectionTexture;

	// Token: 0x04001110 RID: 4368
	private Matrix4x4 reflectionMatrix;

	// Token: 0x04001111 RID: 4369
	private int oldReflectionTextureSize;

	// Token: 0x04001112 RID: 4370
	private static bool renderingMirror;
}
