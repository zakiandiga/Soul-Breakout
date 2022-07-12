using ECM.Controllers;
using UnityEngine;

namespace ECM.Examples.DashingExample
{
    /// <summary>
    /// This example shows how to extend a character controller to add dashing state.
    /// </summary>
    /// <seealso cref="ECM.Controllers.BaseCharacterController" />
    
    public class DashingCharacter : BaseCharacterController
    {
        #region EDITOR EXPOSED FIELDS

        [Tooltip("Dashing duration in seconds.")]
        public float dashDuration = 0.15f;

        [Tooltip("Dashing impulse, e.g. an instant velocity change while dashing.")]
        public float dashImpulse = 10.0f;

        #endregion

        #region FIELDS

        private bool _isDashing;
        private float _dashingTime;

        #endregion

        #region METHODS

        /// <summary>
        /// Determines whether this character is dashing.
        /// </summary>

        public bool IsDashing()
        {
            return _isDashing;
        }

        /// <summary>
        /// Starts a dash.
        /// </summary>
        
        public void Dash()
        {
            if (IsDashing())
                return;
            
            _isDashing = true;
            _dashingTime = 0.0f;
        }

        /// <summary>
        /// Stops the character from dashing.
        /// </summary>
        
        public void StopDashing()
        {
            if (!IsDashing())
                return;

            _isDashing = false;
            _dashingTime = 0.0f;

            // Cancel dash momentum, if not grounded, preserve gravity

            if (isGrounded)
                movement.velocity = Vector3.zero;
            else
                movement.velocity = Vector3.Project(movement.velocity, transform.up);
        }
        
        /// <summary>
        /// Handle Dashing state.
        /// </summary>
        
        protected virtual void Dashing()
        {
            // Bypass acceleration, deceleration and friction while dashing

            movement.Move(moveDirection * dashImpulse, dashImpulse);

            // cancel any vertical velocity while dashing on air (e.g. Cancel gravity)

            if (!movement.isOnGround)
                movement.velocity = Vector3.ProjectOnPlane(movement.velocity, transform.up);

            // Update dash timer, if time completes, stops dashing

            _dashingTime += Time.deltaTime;

            if (_dashingTime > dashDuration)
                StopDashing();
        }

        /// <summary>
        /// Extends Move method to handle dashing state.
        /// </summary>
        
        protected override void Move()
        {
            if (IsDashing())
            {
                // Dashing state

                Dashing();
            }
            else
            {
                // Default state(s)

                base.Move();
            }
        }

        /// <summary>
        /// Handles the dashing input.
        /// </summary>
        
        protected virtual void HandleDashingInput()
        {
            // Starts a dash

            if (Input.GetKeyDown(KeyCode.LeftShift))
                Dash();
            
            if (IsDashing())
            {
                // If dashing, keep character's facing dash direction

                moveDirection = transform.forward;
            }
        }

        /// <summary>
        /// Extends HandleInput method to add dashing state support.
        /// </summary>
        
        protected override void HandleInput()
        {
            base.HandleInput();

            HandleDashingInput();
        }

        #endregion
    }
}
