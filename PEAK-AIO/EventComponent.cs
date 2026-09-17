using System;
using System.Collections.Generic;
using UnityEngine;

public class EventComponent : MonoBehaviour
{
    private float stateTimer = 0f;
    private float validationTimer = 0f;
    private float locationSnapshotTimer = 0f;
    private const float STATE_INTERVAL = 0.1f;
    private const float VALIDATION_INTERVAL = 1f;
    private const float LOCATION_SNAPSHOT_INTERVAL = 2f;

    private struct DelayedAction
    {
        public Action action;
        public float time;
    }

    private static readonly List<DelayedAction> delayedActions = new List<DelayedAction>();

    public static void QueueDelayedAction(Action action, float delaySeconds)
    {
        if (action == null) return;
        lock (delayedActions)
        {
            delayedActions.Add(new DelayedAction
            {
                action = action,
                time = Time.time + delaySeconds
            });
        }
    }

    public static EventComponent Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this);
        }
    }

    private void Update()
    {
        lock (delayedActions)
        {
            for (int i = delayedActions.Count - 1; i >= 0; i--)
            {
                if (Time.time >= delayedActions[i].time)
                {
                    var act = delayedActions[i].action;
                    delayedActions.RemoveAt(i);
                    try { act(); }
                    catch (Exception ex)
                    {
                        ConfigManager.Logger.LogError("[EventComponent] DelayedAction failed: " + ex);
                    }
                }
            }
        }

        stateTimer += Time.deltaTime;
        if (stateTimer >= STATE_INTERVAL)
        {
            stateTimer = 0f;
            GameHelpers.InvalidateCache();
        }

        validationTimer += Time.deltaTime;
        if (validationTimer >= VALIDATION_INTERVAL)
        {
            validationTimer = 0f;

            try
            {
                var localChar = Character.localCharacter;
                if (localChar != null && localChar.data != null && !localChar.data.dead)
                {
                    Utilities.CaptureInventorySnapshot(localChar);
                }
            }
            catch { }

            if (Globals.infiniteToolCharge)
            {
                try
                {
                    Utilities.SafeRechargeToolSlots();
                }
                catch { }
            }
        }

        locationSnapshotTimer += Time.deltaTime;
        if (locationSnapshotTimer >= LOCATION_SNAPSHOT_INTERVAL)
        {
            locationSnapshotTimer = 0f;
            try
            {
                var allChars = Character.AllCharacters;
                if (allChars != null)
                {
                    for (int i = 0; i < allChars.Count; i++)
                    {
                        var ch = allChars[i];
                        if (ch != null && ch.data != null && !ch.data.dead && !ch.data.passedOut && ch.data.isGrounded)
                        {
                            int viewId = ch.photonView != null ? ch.photonView.ViewID : ch.GetInstanceID();
                            Globals.playerSafeLocations[viewId] = new Globals.PlayerLocationSnapshot
                            {
                                safePosition = Utilities.GetCharacterPosition(ch),
                                lastRecordedTime = Time.time
                            };
                        }
                    }
                }
            }
            catch { }
        }

        if (ConfigManager.SpeedMod.Value || ConfigManager.JumpMod.Value)
        {
            var movement = GameHelpers.GetMovementComponent();
            if (movement != null)
            {
                if (ConfigManager.SpeedMod.Value)
                {
                    var f = ConstantFields.GetMovementModifierField();
                    if (f != null) f.SetValue(movement, ConfigManager.SpeedAmount.Value);
                }

                if (ConfigManager.JumpMod.Value)
                {
                    var jf = ConstantFields.GetJumpGravityField();
                    if (jf != null) jf.SetValue(movement, ConfigManager.JumpAmount.Value);

                    if (ConfigManager.NoFallDmg.Value)
                    {
                        var ff = ConstantFields.GetFallDamageTimeField();
                        if (ff != null) ff.SetValue(movement, 999f);
                    }
                }
            }
        }

        if (ConfigManager.ClimbMod.Value)
        {
            var climb = GameHelpers.GetClimbingComponent();
            if (climb != null)
            {
                var f = ConstantFields.GetClimbSpeedModField();
                if (f != null) f.SetValue(climb, ConfigManager.ClimbAmount.Value);
            }
        }

        if (ConfigManager.VineClimbMod.Value)
        {
            var vine = GameHelpers.GetVineClimbComponent();
            if (vine != null)
            {
                var f = ConstantFields.GetVineClimbSpeedModField();
                if (f != null) f.SetValue(vine, ConfigManager.VineClimbAmount.Value);
            }
        }

        if (ConfigManager.RopeClimbMod.Value)
        {
            var rope = GameHelpers.GetRopeClimbComponent();
            if (rope != null)
            {
                var f = ConstantFields.GetRopeClimbSpeedModField();
                if (f != null) f.SetValue(rope, ConfigManager.RopeClimbAmount.Value);
            }
        }

        stateTimer += Time.deltaTime;
        if (stateTimer < STATE_INTERVAL)
            return;
        stateTimer = 0f;

        if (ConfigManager.InfiniteStamina.Value || ConfigManager.LockStatus.Value || ConfigManager.NoWeight.Value)
        {
            var character = GameHelpers.GetCharacterComponent();
            if (character != null)
            {
                if (ConfigManager.InfiniteStamina.Value)
                {
                    var p = ConstantFields.GetInfiniteStaminaProperty();
                    if (p != null) p.SetValue(character, true, null);
                }

                if (ConfigManager.LockStatus.Value)
                {
                    var p = ConstantFields.GetStatusLockProperty();
                    if (p != null) p.SetValue(character, true, null);
                }

                if (ConfigManager.NoWeight.Value)
                {
                    if (character.refs != null && character.refs.afflictions != null)
                    {
                        character.refs.afflictions.SetStatus(CharacterAfflictions.STATUSTYPE.Weight, 0f, false);
                    }
                }
            }
        }
    }
}
