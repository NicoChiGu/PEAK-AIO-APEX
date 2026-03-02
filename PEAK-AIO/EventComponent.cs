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
                    ConstantFields.GetMovementModifierField()?.SetValue(movement, ConfigManager.SpeedAmount.Value);

                if (ConfigManager.JumpMod.Value)
                {
                    ConstantFields.GetJumpGravityField()?.SetValue(movement, ConfigManager.JumpAmount.Value);

                    if (ConfigManager.NoFallDmg.Value)
                        ConstantFields.GetFallDamageTimeField()?.SetValue(movement, 999f);
                }
            }
        }

        if (ConfigManager.ClimbMod.Value)
        {
            var climb = GameHelpers.GetClimbingComponent();
            if (climb != null)
                ConstantFields.GetClimbSpeedModField()?.SetValue(climb, ConfigManager.ClimbAmount.Value);
        }

        if (ConfigManager.VineClimbMod.Value)
        {
            var vine = GameHelpers.GetVineClimbComponent();
            if (vine != null)
                ConstantFields.GetVineClimbSpeedModField()?.SetValue(vine, ConfigManager.VineClimbAmount.Value);
        }

        if (ConfigManager.RopeClimbMod.Value)
        {
            var rope = GameHelpers.GetRopeClimbComponent();
            if (rope != null)
                ConstantFields.GetRopeClimbSpeedModField()?.SetValue(rope, ConfigManager.RopeClimbAmount.Value);
        }

        stateTimer += Time.deltaTime;
        if (stateTimer < STATE_INTERVAL)
            return;
        stateTimer = 0f;

        if (ConfigManager.InfiniteStamina.Value || ConfigManager.LockStatus.Value)
        {
            var character = GameHelpers.GetCharacterComponent();
            if (character != null)
            {
                if (ConfigManager.InfiniteStamina.Value)
                    ConstantFields.GetInfiniteStaminaProperty()?.SetValue(character, true);

                if (ConfigManager.LockStatus.Value)
                    ConstantFields.GetStatusLockProperty()?.SetValue(character, true);
            }
        }
    }
}
