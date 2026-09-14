using UnityEngine;

public class EventComponent : MonoBehaviour
{
    private float stateTimer = 0f;
    private float validationTimer = 0f;
    private const float STATE_INTERVAL = 0.1f;
    private const float VALIDATION_INTERVAL = 1f;

    private void Update()
    {
        validationTimer += Time.deltaTime;
        if (validationTimer >= VALIDATION_INTERVAL)
        {
            validationTimer = 0f;
            GameHelpers.InvalidateCache();
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
