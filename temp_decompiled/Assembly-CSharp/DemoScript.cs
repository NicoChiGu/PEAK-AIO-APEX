using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200029D RID: 669
public class DemoScript : MonoBehaviour
{
	// Token: 0x06001255 RID: 4693 RVA: 0x0005CF50 File Offset: 0x0005B150
	private void Start()
	{
		this.originalRotation = base.transform.localRotation;
		Renderer component = this.LightBulb.GetComponent<Renderer>();
		if (Application.isPlaying)
		{
			component.sharedMaterial = component.material;
		}
		this.lightBulbMaterial = component.sharedMaterial;
	}

	// Token: 0x06001256 RID: 4694 RVA: 0x0005CF99 File Offset: 0x0005B199
	private void Update()
	{
		this.RotateMirror();
		this.MoveLightBulb();
		this.UpdateMouseLook();
		this.UpdateMovement();
	}

	// Token: 0x06001257 RID: 4695 RVA: 0x0005CFB3 File Offset: 0x0005B1B3
	public void MirrorRecursionToggled()
	{
		this.ChangeMirrorRecursion();
	}

	// Token: 0x06001258 RID: 4696 RVA: 0x0005CFBC File Offset: 0x0005B1BC
	public void ChangeMirrorRecursion()
	{
		foreach (GameObject gameObject in this.Mirrors)
		{
			gameObject.GetComponent<MirrorScript>().MirrorRecursion = this.RecursionToggle.isOn;
		}
	}

	// Token: 0x06001259 RID: 4697 RVA: 0x0005D01C File Offset: 0x0005B21C
	private void UpdateMovement()
	{
		float num = 4f * Time.deltaTime;
		if (Input.GetKey(119))
		{
			base.transform.Translate(0f, 0f, num);
		}
		else if (Input.GetKey(115))
		{
			base.transform.Translate(0f, 0f, -num);
		}
		if (Input.GetKey(97))
		{
			base.transform.Translate(-num, 0f, 0f);
		}
		else if (Input.GetKey(100))
		{
			base.transform.Translate(num, 0f, 0f);
		}
		if (Input.GetKeyDown(109))
		{
			this.RecursionToggle.isOn = !this.RecursionToggle.isOn;
		}
	}

	// Token: 0x0600125A RID: 4698 RVA: 0x0005D0DC File Offset: 0x0005B2DC
	private void RotateMirror()
	{
		GameObject gameObject = this.Mirrors[0];
		float num = gameObject.transform.rotation.eulerAngles.y;
		if (num > 65f && num < 100f)
		{
			this.rotationModifier = -this.rotationModifier;
			num -= 65f;
			gameObject.transform.Rotate(0f, -num, 0f);
			return;
		}
		if (num > 100f && num < 295f)
		{
			this.rotationModifier = -this.rotationModifier;
			num = 295f - num;
			gameObject.transform.Rotate(0f, num, 0f);
			return;
		}
		gameObject.transform.Rotate(0f, this.rotationModifier * Time.deltaTime * 20f, 0f);
	}

	// Token: 0x0600125B RID: 4699 RVA: 0x0005D1B0 File Offset: 0x0005B3B0
	private void MoveLightBulb()
	{
		float num = this.LightBulb.transform.position.x;
		if (num > 5f)
		{
			this.moveModifier = -this.moveModifier;
			num = 5f;
		}
		else if (num < -5f)
		{
			this.moveModifier = -this.moveModifier;
			num = -5f;
		}
		else
		{
			num += Time.deltaTime * this.moveModifier;
		}
		Light component = this.LightBulb.GetComponent<Light>();
		this.LightBulb.transform.position = new Vector3(num, this.LightBulb.transform.position.y, this.LightBulb.transform.position.z);
		float num2 = Mathf.Min(1f, component.intensity);
		this.lightBulbMaterial.SetColor("_EmissionColor", new Color(num2, num2, num2));
	}

	// Token: 0x0600125C RID: 4700 RVA: 0x0005D294 File Offset: 0x0005B494
	private void UpdateMouseLook()
	{
		if (this.axes == DemoScript.RotationAxes.MouseXAndY)
		{
			this.rotationX += Input.GetAxis("Mouse X") * this.sensitivityX;
			this.rotationY += Input.GetAxis("Mouse Y") * this.sensitivityY;
			this.rotationX = DemoScript.ClampAngle(this.rotationX, this.minimumX, this.maximumX);
			this.rotationY = DemoScript.ClampAngle(this.rotationY, this.minimumY, this.maximumY);
			Quaternion quaternion = Quaternion.AngleAxis(this.rotationX, Vector3.up);
			Quaternion quaternion2 = Quaternion.AngleAxis(this.rotationY, -Vector3.right);
			base.transform.localRotation = this.originalRotation * quaternion * quaternion2;
			return;
		}
		if (this.axes == DemoScript.RotationAxes.MouseX)
		{
			this.rotationX += Input.GetAxis("Mouse X") * this.sensitivityX;
			this.rotationX = DemoScript.ClampAngle(this.rotationX, this.minimumX, this.maximumX);
			Quaternion quaternion3 = Quaternion.AngleAxis(this.rotationX, Vector3.up);
			base.transform.localRotation = this.originalRotation * quaternion3;
			return;
		}
		this.rotationY += Input.GetAxis("Mouse Y") * this.sensitivityY;
		this.rotationY = DemoScript.ClampAngle(this.rotationY, this.minimumY, this.maximumY);
		Quaternion quaternion4 = Quaternion.AngleAxis(-this.rotationY, Vector3.right);
		base.transform.localRotation = this.originalRotation * quaternion4;
	}

	// Token: 0x0600125D RID: 4701 RVA: 0x0005D438 File Offset: 0x0005B638
	public static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360f)
		{
			angle += 360f;
		}
		if (angle > 360f)
		{
			angle -= 360f;
		}
		return Mathf.Clamp(angle, min, max);
	}

	// Token: 0x040010F9 RID: 4345
	public List<GameObject> Mirrors;

	// Token: 0x040010FA RID: 4346
	public GameObject LightBulb;

	// Token: 0x040010FB RID: 4347
	public Toggle RecursionToggle;

	// Token: 0x040010FC RID: 4348
	private float rotationModifier = -1f;

	// Token: 0x040010FD RID: 4349
	private float moveModifier = 1f;

	// Token: 0x040010FE RID: 4350
	private Material lightBulbMaterial;

	// Token: 0x040010FF RID: 4351
	private DemoScript.RotationAxes axes;

	// Token: 0x04001100 RID: 4352
	private float sensitivityX = 15f;

	// Token: 0x04001101 RID: 4353
	private float sensitivityY = 15f;

	// Token: 0x04001102 RID: 4354
	private float minimumX = -360f;

	// Token: 0x04001103 RID: 4355
	private float maximumX = 360f;

	// Token: 0x04001104 RID: 4356
	private float minimumY = -60f;

	// Token: 0x04001105 RID: 4357
	private float maximumY = 60f;

	// Token: 0x04001106 RID: 4358
	private float rotationX;

	// Token: 0x04001107 RID: 4359
	private float rotationY;

	// Token: 0x04001108 RID: 4360
	private Quaternion originalRotation;

	// Token: 0x020004E4 RID: 1252
	private enum RotationAxes
	{
		// Token: 0x04001ADF RID: 6879
		MouseXAndY,
		// Token: 0x04001AE0 RID: 6880
		MouseX,
		// Token: 0x04001AE1 RID: 6881
		MouseY
	}
}
