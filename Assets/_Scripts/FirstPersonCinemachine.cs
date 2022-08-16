using System;
using System.Collections;
using UnityEngine;
using Cinemachine;


namespace ECM.Controllers
{
    public class FirstPersonCinemachine : BaseCharacterController
    {
        #region EDITOR EXPOSED FIELDS

        [Header("First Person")]
        [Tooltip("Speed when moving forward.")]
        [SerializeField]
        private float _forwardSpeed = 5.0f;

        [Tooltip("Speed when moving backwards.")]
        [SerializeField]
        private float _backwardSpeed = 3.0f;

        [Tooltip("Speed when moving sideways.")]
        [SerializeField]
        private float _strafeSpeed = 4.0f;

        [Tooltip("Speed multiplier while running.")]
        [SerializeField]
        private float _runSpeedMultiplier = 2.0f;
        #endregion

        #region PROPERTIES

        /// <summary>
        /// Cached camera pivot transform.
        /// </summary>
        public bool PlayerNoticable => playerNoticable;

        public Transform cameraPivotTransform { get; private set; }

        /// <summary>
        /// Cached camera transform.
        /// </summary>

        public Transform cameraTransform { get; private set; }

        /// <summary>
        /// Cached MouseLook component.
        /// </summary>

        public Components.MouseLook mouseLook { get; private set; }

        /// <summary>
        /// Speed when moving forward.
        /// </summary>

        public float forwardSpeed
        {
            get { return _forwardSpeed; }
            set { _forwardSpeed = Mathf.Max(0.0f, value); }
        }

        /// <summary>
        /// Speed when moving backwards.
        /// </summary>

        public float backwardSpeed
        {
            get { return _backwardSpeed; }
            set { _backwardSpeed = Mathf.Max(0.0f, value); }
        }

        /// <summary>
        /// Speed when moving sideways.
        /// </summary>

        public float strafeSpeed
        {
            get { return _strafeSpeed; }
            set { _strafeSpeed = Mathf.Max(0.0f, value); }
        }

        /// <summary>
        /// Speed multiplier while running.
        /// </summary>

        public float runSpeedMultiplier
        {
            get { return _runSpeedMultiplier; }
            set { _runSpeedMultiplier = Mathf.Max(value, 1.0f); }
        }

        /// <summary>
        /// Run input command.
        /// </summary>

        public bool run { get; set; }

        public bool IsPossessing { get; private set; }

        public bool IsInteracting { get; private set; }

        private bool possess = false;
        private bool interact = false;

        public int CharacterCode => characterCode;

        private float moveSpeed => movement.cachedRigidbody.velocity.magnitude;

        [SerializeField] private int characterCode = 1;
        [SerializeField] private GameObject ghostBody;

        [SerializeField] private float noticableCooldown = 6f;
        private bool playerNoticable = false;
        #endregion

        #region EVENTS
        public event Action<bool> OnPossessPressed;
        public static event Action<Vector3> OnInstantiateGhost;
        public static event Action<int> OnPlayerNewBody;
        #endregion

        #region MISCFIELD
        private CinemachineVirtualCamera virtualCam;
        #endregion

        #region METHODS

        protected override void Animate()
        {
            if (animator == null)
                return;

            if(moveDirection != Vector3.zero)
            {
                animator.SetFloat("MoveSpeed", moveSpeed);


            }
            else
            {
                animator.SetFloat("MoveSpeed", 0);

            }
        }

        /// <summary>
        /// Use this method to animate camera.
        /// The default implementation use this to animate camera's when crouching.
        /// Called on LateUpdate.
        /// </summary>

        protected virtual void AnimateView()
        {
            // Scale camera pivot to simulate crouching

            var yScale = isCrouching ? Mathf.Clamp01(crouchingHeight / standingHeight) : 1.0f;

            cameraPivotTransform.localScale = Vector3.MoveTowards(cameraPivotTransform.localScale,
                new Vector3(1.0f, yScale, 1.0f), 5.0f * Time.deltaTime);
        }

        /// <summary>
        /// Perform 'Look' rotation.
        /// This rotate the character along its y-axis (yaw) and a child camera along its local x-axis (pitch).
        /// </summary>

        protected virtual void RotateView()
        {
            mouseLook.LookRotation(movement, cameraTransform);
        }

        /// <summary>
        /// Override the default ECM UpdateRotation to perform typical fps rotation.
        /// </summary>

        protected override void UpdateRotation()
        {
            RotateView();
        }

        /// <summary>
        /// Get target speed, relative to input moveDirection,
        /// eg: forward, backward or strafe.
        /// </summary>

        protected virtual float GetTargetSpeed()
        {
            // Defaults to forward speed

            var targetSpeed = forwardSpeed;

            // Strafe

            if (moveDirection.x > 0.0f || moveDirection.x < 0.0f)
                targetSpeed = strafeSpeed;

            // Backwards

            if (moveDirection.z < 0.0f)
                targetSpeed = backwardSpeed;

            // Forward handled last as if strafing and moving forward at the same time,
            // forward speed should take precedence

            if (moveDirection.z > 0.0f)
                targetSpeed = forwardSpeed;

            // Handle run speed modifier

            return run ? targetSpeed * runSpeedMultiplier : targetSpeed;
        }

        /// <summary>
        /// Overrides CalcDesiredVelocity to generate a velocity vector relative to view direction
        /// eg: forward, backward or strafe.
        /// </summary>

        protected override Vector3 CalcDesiredVelocity()
        {
            // Set character's target speed (eg: moving forward, backward or strafe)

            speed = GetTargetSpeed();

            // Return desired velocity relative to view direction and target speed

            return transform.TransformDirection(base.CalcDesiredVelocity());
        }

        /// <summary>
        /// Overrides 'BaseCharacterController' HandleInput method,
        /// to perform custom input code. 
        /// </summary>

        protected override void HandleInput()
        {
            // Toggle pause / resume.
            // By default, will restore character's velocity on resume (eg: restoreVelocityOnResume = true)

            if (Input.GetKeyDown(KeyCode.P))
                pause = !pause;

            // Player input
            if(!interact)
            {
                moveDirection = new Vector3
                {
                    x = Input.GetAxisRaw("Horizontal"),
                    y = 0.0f,
                    z = Input.GetAxisRaw("Vertical")
                };

                run = Input.GetButton("Fire3");

                jump = Input.GetButton("Jump");

                crouch = Input.GetKey(KeyCode.C);

                possess = Input.GetKeyDown(KeyCode.E);

            }

            interact = Input.GetKey(KeyCode.T); 
            
        }

        private void Possess()
        {
            OnPossessPressed?.Invoke(possess);
        }

        public void OutOfBody()
        {
            //instantiate ghost body
            Debug.Log(gameObject.name + " POSSESSED, out og body now");
            //OnInstantiateGhost?.Invoke(this.transform.position);
            Instantiate(ghostBody, transform.position - (transform.forward *2), this.transform.rotation);

            this.enabled = false;

            GhostBody.OnGhostBodyReady += DisablingControl;
        }

        private void DisablingControl(GhostBody ghostBody)
        {
            GhostBody.OnGhostBodyReady -= DisablingControl;
            this.virtualCam.Priority = 1;
        }
        #endregion

        #region MONOBEHAVIOUR

        /// <summary>
        /// Validate this editor exposed fields.
        /// </summary>

        public override void OnValidate()
        {
            // Call the parent class' version of method

            base.OnValidate();

            // Validate this editor exposed fields

            forwardSpeed = _forwardSpeed;
            backwardSpeed = _backwardSpeed;
            strafeSpeed = _strafeSpeed;

            runSpeedMultiplier = _runSpeedMultiplier;
        }

        /// <summary>
        /// Initialize this.
        /// </summary>

        public override void Awake()
        {
            // Call the parent class' version of method

            base.Awake();

            // Cache and initialize this components

            mouseLook = GetComponent<Components.MouseLook>();
            if (mouseLook == null)
            {
                Debug.LogError(
                    string.Format(
                        "BaseFPSController: No 'MouseLook' found. Please add a 'MouseLook' component to '{0}' game object",
                        name));
            }

            cameraPivotTransform = transform.Find("Camera_Pivot");
            if (cameraPivotTransform == null)
            {
                Debug.LogError(string.Format(
                    "BaseFPSController: No 'Camera_Pivot' found. Please parent a transform gameobject to '{0}' game object.",
                    name));
            }

            virtualCam = GetComponentInChildren<CinemachineVirtualCamera>();
            if (virtualCam == null)
            {
                Debug.LogError(
                    string.Format(
                        "BaseFPSController: No 'Camera' found. Please parent a camera to '{0}' game object.", name));
            }
            else
            {
                cameraTransform = virtualCam.transform;
                mouseLook.Init(transform, cameraTransform);
            }
        }

        private IEnumerator CountNoticableTime()
        {
            yield return new WaitForSeconds(noticableCooldown);
            playerNoticable = true;

        }

        private void OnEnable()
        {
            OnPlayerNewBody?.Invoke(CharacterCode);

            if (!playerNoticable)
                StartCoroutine(CountNoticableTime());

            if(movement.cachedRigidbody != null)
                movement.cachedRigidbody.useGravity = false;
        }

        private void OnDisable()
        {
            if(IsPossessing)
                IsPossessing = false;

            if (playerNoticable)
                playerNoticable = false;

            movement.cachedRigidbody.useGravity = true;
        }

        public override void Update()
        {
            base.Update();

            if (!IsPossessing)
            {
                if (possess)
                {
                    IsPossessing = true;
                    Possess();
                }
            }

            if (IsPossessing && !possess)
            {
                IsPossessing = false;
            }

            if(!IsInteracting)     //interact
            {
                if(interact)
                {
                    IsInteracting = true;
                }
            }
            if(IsInteracting && !interact)
            {
                IsInteracting = false;
            }
        }

        public virtual void LateUpdate()
        {
            // Perform camera's (view) animation

            AnimateView();
        }

        #endregion
    }
}

