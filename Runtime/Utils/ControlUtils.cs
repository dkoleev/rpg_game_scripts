using UnityEngine;

namespace Darkness.Runtime.Utils {
    public static class ControlUtils {
        /// <summary>
        /// Setup dead zones. Don't forget that there are also default settings in PlayerSetting->InputSystem->Settings
        /// </summary>
        /// <param name="input"></param>
        /// <param name="minDeadZone"></param>
        /// <param name="maxDeadZone"></param>
        /// <param name="snapTo8Directions"></param>
        /// <returns></returns>
        public static Vector2 ApplyDeadZones(Vector2 input, float minDeadZone = 0.15f, float maxDeadZone = 0.95f, bool snapTo8Directions = false) {
            input.x = Mathf.Abs(input.x) < minDeadZone ? 0 : Mathf.Clamp(input.x / maxDeadZone, -1, 1);
            input.y = Mathf.Abs(input.y) < minDeadZone ? 0 : Mathf.Clamp(input.y / maxDeadZone, -1, 1);
            
            if (snapTo8Directions && input != Vector2.zero)
            {
                // Normalize to nearest 8-direction vector
                Vector2 normalized = input.normalized;
                var angle = Mathf.Atan2(normalized.y, normalized.x) * Mathf.Rad2Deg;
                var snappedAngle = Mathf.Round(angle / 45f) * 45f;
                var rad = snappedAngle * Mathf.Deg2Rad;
                
                return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;
            }

            return input;
        }
    }
}