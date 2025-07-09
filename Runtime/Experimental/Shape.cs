using Darkness.Runtime.Utils.CustomTypes;
using UnityEngine;

namespace Darkness.Runtime.Experimental
{
    public class Experimental
    {
        public void OptionTest()
        {
            var res = SafeDivide(10, 0);
        }
        
        // Return Option<float> — Some(result) or None if division by zero
        public static Option<float> SafeDivide(float numerator, float denominator)
        {
            if (denominator == 0f)
                return Option<float>.None();

            return Option<float>.Some(numerator / denominator);
        }
    }
}