namespace UnitySpring
{
    // A two-parameter struct defining spring behaviour that a physical spring model (SpringProperties) can be derived from.
    // Concept taken from Apple, as described in this WWDC video https://developer.apple.com/videos/play/wwdc2018/803/, and implementation taken from https://medium.com/ios-os-x-development/demystifying-uikit-spring-animations-2bb868446773
    [System.Serializable]
    public struct SimpleSpringProperties {
        // Between 0 and 1, where 0 will oscellate forever and 1 will be "fully damped".
        public float dampingRatio;
        // The time taken to oscellate once, or to (approximately) come to a stop for fully damped springs.
        public float frequencyResponse;

        public SimpleSpringProperties (float dampingRatio, float frequencyResponse) {
            this.dampingRatio = dampingRatio;
            this.frequencyResponse = frequencyResponse;
            UnityEngine.Debug.Assert(dampingRatio >= 0);
            UnityEngine.Debug.Assert(frequencyResponse > 0);
        }

        public SpringProperties ToPhysicalSpringProperties() {
            float mass = 1f;
            float stiffness = UnityEngine.Mathf.Pow(2 * UnityEngine.Mathf.PI / frequencyResponse, 2) * mass;
            float damping = 4 * UnityEngine.Mathf.PI * dampingRatio * mass / frequencyResponse;

            return new SpringProperties(damping, mass, stiffness);
        }
    }
    
    [System.Serializable]
    public struct SpringProperties {
        public float damping;// = 26f;
        public float mass;// = 1f;
        public float stiffness;// = 169f;
        
        public SpringProperties (float damping, float mass, float stiffness) {
            this.damping = damping;
            this.mass = mass;
            this.stiffness = stiffness;
        }
    }

    public abstract class SpringBase
    {
        // Default to critically damped
        public float damping = 26f;
        public float mass = 1f;
        public float stiffness = 169f;
        public float startValue;
        public float endValue;
        public float initialVelocity;

        public float currentValue { get; protected set; }
        public float currentVelocity { get; protected set; }

        public void SetProperties (SpringProperties springProperties) {
            damping = springProperties.damping;
            mass = springProperties.mass;
            stiffness = springProperties.stiffness;
        }
        
        public void SetProperties (SimpleSpringProperties springProperties) {
            SetProperties(springProperties.ToPhysicalSpringProperties());
        }

        /// <summary>
        /// Reset all values to initial states.
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// Update the end value in the middle of motion.
        /// This reuse the current velocity and interpolate the value smoothly afterwards.
        /// </summary>
        /// <param name="value">End value</param>
        public virtual void UpdateEndValue(float value) => UpdateEndValue(value, currentVelocity);

        /// <summary>
        /// Update the end value in the middle of motion but using a new velocity.
        /// </summary>
        /// <param name="value">End value</param>
        /// <param name="velocity">New velocity</param>
        public abstract void UpdateEndValue(float value, float velocity);

        /// <summary>
        /// Advance a step by deltaTime(seconds).
        /// </summary>
        /// <param name="deltaTime">Delta time since previous frame</param>
        /// <returns>Evaluated value</returns>
        public abstract float Evaluate(float deltaTime);
    }
}