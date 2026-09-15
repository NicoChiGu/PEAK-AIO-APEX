using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000345 RID: 837
public class StormVisual : MonoBehaviour
{
	// Token: 0x0600157B RID: 5499 RVA: 0x0006E480 File Offset: 0x0006C680
	private void Start()
	{
		this.zone = base.GetComponentInParent<WindChillZone>();
		this.fogConfig = base.GetComponentInParent<FogConfig>();
		if (this.quadRend)
		{
			this.quadMat = this.quadRend.material;
		}
	}

	// Token: 0x0600157C RID: 5500 RVA: 0x0006E4B8 File Offset: 0x0006C6B8
	private void LateUpdate()
	{
		this.observedPlayerInWindZone = this.zone.windActive && this.zone.observedCharacterInsideBounds;
		if (this.useWindChillZoneIntensity)
		{
			Mathf.Clamp01(this.zone.windIntensity * this.windchillZoneMult);
		}
		if (this.observedPlayerInWindZone)
		{
			if (this.useWindChillZoneIntensity)
			{
				this.windIntensity = Mathf.Clamp01(this.zone.windIntensity * this.windchillZoneMult);
				this.windFactor = Mathf.Lerp(this.windFactor, this.windIntensity, Time.deltaTime / 2f);
				for (int i = 0; i < this.part.Length; i++)
				{
					if (!this.part[i].isPlaying && this.zone.windIntensity > 0.1f)
					{
						this.part[i].Play();
					}
				}
				Shader.SetGlobalFloat("GlobalWind", this.windIntensity);
			}
			else
			{
				for (int j = 0; j < this.part.Length; j++)
				{
					if (!this.part[j].isPlaying)
					{
						this.part[j].Play();
					}
				}
				this.windFactor = Mathf.Lerp(this.windFactor, Mathf.Clamp01(this.zone.hasBeenActiveFor * 0.2f), Time.deltaTime);
			}
		}
		else
		{
			if (this.useWindChillZoneIntensity)
			{
				for (int k = 0; k < this.part.Length; k++)
				{
					if (this.part[k].isPlaying && this.zone.windIntensity < 0.1f)
					{
						this.part[k].Stop();
					}
				}
			}
			else
			{
				for (int l = 0; l < this.part.Length; l++)
				{
					if (this.part[l].isPlaying)
					{
						this.part[l].Stop();
					}
				}
			}
			this.windIntensity = Mathf.Lerp(this.windIntensity, 0f, Time.deltaTime);
			Shader.SetGlobalFloat("GlobalWind", this.windIntensity);
			this.windFactor = Mathf.Lerp(this.windFactor, 0f, Time.deltaTime);
		}
		if (this.stormType == StormVisual.StormType.Rain)
		{
			DayNightManager.instance.rainstormWindFactor = this.windFactor;
		}
		else if (this.stormType == StormVisual.StormType.Snow)
		{
			DayNightManager.instance.snowstormWindFactor = this.windFactor;
		}
		else if (this.stormType == StormVisual.StormType.Wind)
		{
			DayNightManager.instance.rainstormWindFactor = this.windFactor;
			this.particleForceField.directionX = this.zone.currentWindDirection.x * this.windIntensity * this.windParticleMult;
			this.particleForceField.directionZ = this.zone.currentWindDirection.z * this.windIntensity * this.windParticleMult;
		}
		if (this.zone.observedCharacterInsideBounds)
		{
			base.transform.position = Character.observedCharacter.Center;
			if (this.zone.currentWindDirection != Vector3.zero)
			{
				base.transform.rotation = Quaternion.LookRotation(this.zone.currentWindDirection);
			}
			if (base.gameObject.CompareTag("Storm"))
			{
				Character.observedCharacter.refs.animations.stormAudio.stormVisual = this;
			}
			if (base.gameObject.CompareTag("Rain"))
			{
				Character.observedCharacter.refs.animations.stormAudio.rainVisual = this;
			}
			if (this.fogConfig && this.zone.windActive)
			{
				this.fogConfig.SetFog();
			}
			if (this.quadMat)
			{
				this.quadRend.enabled = true;
				this.quadMat.SetFloat("_Alpha", this.windFactor);
				return;
			}
		}
		else if (this.quadRend)
		{
			this.quadRend.enabled = false;
		}
	}

	// Token: 0x0600157D RID: 5501 RVA: 0x0006E89B File Offset: 0x0006CA9B
	private void OnDisable()
	{
		Shader.SetGlobalFloat("GlobalWind", 0f);
	}

	// Token: 0x04001446 RID: 5190
	public ParticleSystem[] part;

	// Token: 0x04001447 RID: 5191
	public MeshRenderer quadRend;

	// Token: 0x04001448 RID: 5192
	private Material quadMat;

	// Token: 0x04001449 RID: 5193
	private FogConfig fogConfig;

	// Token: 0x0400144A RID: 5194
	public AudioLoop stormSFX;

	// Token: 0x0400144B RID: 5195
	[FormerlySerializedAs("playerInWindZone")]
	public bool observedPlayerInWindZone;

	// Token: 0x0400144C RID: 5196
	private WindChillZone zone;

	// Token: 0x0400144D RID: 5197
	public bool useWindChillZoneIntensity;

	// Token: 0x0400144E RID: 5198
	public float windchillZoneMult;

	// Token: 0x0400144F RID: 5199
	private float windIntensity;

	// Token: 0x04001450 RID: 5200
	public ParticleSystemForceField particleForceField;

	// Token: 0x04001451 RID: 5201
	public float windParticleMult = 50f;

	// Token: 0x04001452 RID: 5202
	public StormVisual.StormType stormType;

	// Token: 0x04001453 RID: 5203
	public float windFactor;

	// Token: 0x02000512 RID: 1298
	public enum StormType
	{
		// Token: 0x04001B76 RID: 7030
		Rain,
		// Token: 0x04001B77 RID: 7031
		Snow,
		// Token: 0x04001B78 RID: 7032
		Wind
	}
}
