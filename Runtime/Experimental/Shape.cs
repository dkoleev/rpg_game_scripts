using Darkness.Runtime.Utils.CustomTypes;
using UnityEngine;

namespace Darkness.Runtime.Experimental {
    public class Experimental {
        public void OptionTest() {
            var result = Divide(10, 2);
            result.Match(
                success => Debug.Log($"Success: {success}"),
                error => Debug.Log($"Error: {error}")
            );
        }

        private Result<float> Divide(float a, float b) {
            if (b == 0) return Result<float>.Fail("Cannot divide by zero");

            return Result<float>.Success(a / b);
        }
    }
}