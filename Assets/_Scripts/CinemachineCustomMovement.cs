using ECM.Common;
using UnityEngine;
using Cinemachine;

namespace ECM.Controllers
{
    /// <summary>
    /// Custom character controller. This shows how make the character move relative to MainCamera view direction.
    /// </summary>

    public class CinemachineCustomMovement : BaseCharacterController
    {
        public Components.MouseLook mouseLook { get; private set; }

        [SerializeField] private CinemachineVirtualCamera virtualCam;
        private CinemachinePOV cinemachinePOV;

        private Camera mainCam;
        private Transform cameraPivot;

        protected override void Animate()
        {
            // Add animator related code here...
        }

        protected void RotateView()
        {
            if (moveDirection == Vector3.zero)
                mouseLook.LookRotation(movement, mainCam.transform);
            else
                mouseLook.MoveRotation(movement, mainCam.transform);
        }

        private void ForceRecentering()
        {
            if(moveDirection == Vector3.zero)
            {
                cinemachinePOV.m_HorizontalRecentering.m_enabled = false;
            }
            else
            {
                cinemachinePOV.m_HorizontalRecentering.m_enabled = true;
            }
        }

        protected override void UpdateRotation()
        {
            RotateView();
        }

        protected override Vector3 CalcDesiredVelocity()
        {

            return mainCam.transform.TransformDirection(base.CalcDesiredVelocity());
        }

        protected override void HandleInput()
        {
            // Toggle pause / resume.
            // By default, will restore character's velocity on resume (eg: restoreVelocityOnResume = true)

            if (Input.GetKeyDown(KeyCode.P))
                pause = !pause;

            // Handle user input

            jump = Input.GetButton("Jump");

            crouch = Input.GetKey(KeyCode.C);

            moveDirection = new Vector3
            {
                x = Input.GetAxisRaw("Horizontal"),
                y = 0.0f,
                z = Input.GetAxisRaw("Vertical")
            };

            // Transform the given moveDirection to be relative to the main camera's view direction.
            // Here we use the included extension .relativeTo...
            
        }


        public override void Awake()
        {
            base.Awake();

            mouseLook = GetComponent<Components.MouseLook>();
            cameraPivot = transform.Find("FollorTarget");
            mainCam = Camera.main;

            mouseLook.Init(transform, mainCam.transform);

            cinemachinePOV = virtualCam.GetCinemachineComponent<CinemachinePOV>();

        }

        public override void Update()
        {
            base.Update();

            ForceRecentering();
        }
    }
}
