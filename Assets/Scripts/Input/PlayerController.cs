using KBCore.Refs;
using UnityEngine.Events;
using UnityEngine;

using Unity.Cinemachine;
using System.Collections.Generic;

using Utilities;
using System;

namespace Platformer
{
    public class PlayerController : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField, Self] Rigidbody rb;
        [SerializeField, Self] GroundChecker groundChecker;
        [SerializeField, Self] Animator animator;
        [SerializeField, Anywhere] InputReader input;

        [Header("Movement Settings")]
        [SerializeField] float moveSpeed = 6f;
        [SerializeField] float rotationSpeed = 15f;
        [SerializeField] float smoothTime = 2f;

        [Header("Push Settings")]
        [SerializeField] float pushCheckDistance = 5f;
        [SerializeField] float pushCheckHeight = -0.1f;
        bool isPushing;
        bool pushHeld = false;
        Pushable currentPushable = null;
        Vector3 pushAxis; // locked axis
        [SerializeField] float pushSpeedLocal = 3f;
        Vector3 pushOffset;
        bool offsetSet = false;
        float originalPlayerDrag, originalPlayerAngularDrag, originalPushableDrag, originalPushableAngularDrag;
        bool dragAdjusted = false;

        [Header("Jump Settings")]
        [SerializeField] float jumpForce = 10f;
        [SerializeField] float jumpDuration = 0.5f;
        [SerializeField] float jumpCooldown = 0f;
        [SerializeField] float gravity = 3f;

        Transform mainCam;

        float currentSpeed;
        float velocity;
        float jumpVelocity;

        Vector3 movement;

        public bool MovementEnabled { get; set; } = true;

        List<Timer> timers;

        CountdownTimer jumpTimer;
        CountdownTimer jumpCooldownTimer;

        StateMachine stateMachine;

        // Animator Parameters
        static readonly int SpeedHash = Animator.StringToHash("Speed");
        static readonly int IsPushingHash = Animator.StringToHash("IsPushing");
        static readonly int PushPullDirectionHash = Animator.StringToHash("PushPullDirection");

        void Awake()
        {
            mainCam = Camera.main.transform;
            rb.freezeRotation = true;

            jumpTimer = new CountdownTimer(jumpDuration);
            jumpCooldownTimer = new CountdownTimer(jumpCooldown);
            timers = new List<Timer>(2) { jumpTimer, jumpCooldownTimer };

            jumpTimer.OnTimerStart += () => jumpVelocity = jumpForce;
            jumpTimer.OnTimerStop += () => jumpCooldownTimer.Start();

            stateMachine = new StateMachine();

            var locomotionState = new LocomotionState(this, animator);
            var pushState = new PushState(this, animator);
            var jumpState = new JumpState(this, animator);

            At(locomotionState, jumpState, new FuncPredicate(() => jumpTimer.IsRunning));
            At(jumpState, locomotionState, new FuncPredicate(() => !jumpTimer.IsRunning && groundChecker.IsGrounded));
            At(locomotionState, pushState, new FuncPredicate(() => isPushing));
            At(pushState, locomotionState, new FuncPredicate(() => !isPushing));
            At(pushState, jumpState, new FuncPredicate(() => jumpTimer.IsRunning));

            stateMachine.SetState(locomotionState);
        }

        void At(IState from, IState to, IPredicate condition) =>
            stateMachine.AddTransition(from, to, condition);

        void OnEnable()
        {
            input.Jump += OnJump;
            input.Attack += OnPush;

        }

        void OnDisable()
        {
            input.Jump -= OnJump;
            input.Attack -= OnPush;

        }

        void OnJump(bool performed)
        {
            if (performed && !jumpTimer.IsRunning && !jumpCooldownTimer.IsRunning && groundChecker.IsGrounded)
                jumpTimer.Start();
            else if (!performed && jumpTimer.IsRunning)
                jumpTimer.Stop();
        }

        void Update()
        {
            if (!MovementEnabled)
            {

                rb.linearVelocity = Vector3.zero;

                isPushing = false;
                currentSpeed = 0f;
                movement = Vector3.zero;
                animator.SetFloat(SpeedHash, 0f);
                animator.SetBool(IsPushingHash, false);
                animator.SetFloat(PushPullDirectionHash, 0f);
                return;
            }

            // ---  Read player input ---
            movement = new Vector3(input.Direction.x, 0f, input.Direction.y).normalized;

            // ---  Handle pushing ---
            if (pushHeld && currentPushable != null && currentPushable.IsInteracted && offsetSet)
            {
                HandlePushPull();
            }
            else if (pushHeld)
            {
                HandleMovement();
            }
            else
            {
                // Unparent player when not pushing (not left clicking)
                // if (transform.parent != null)
                // {
                //     transform.SetParent(null);
                // }
                if (dragAdjusted && currentPushable != null)
                {
                    Rigidbody rbPush = currentPushable.GetComponent<Rigidbody>();
                    if (rbPush != null)
                    {
                        rbPush.linearDamping = originalPushableDrag;
                        rbPush.angularDamping = originalPushableAngularDrag;
                        rbPush.isKinematic = false;
                        rbPush.linearVelocity = Vector3.zero;
                        rbPush.angularVelocity = Vector3.zero;
                    }
                    dragAdjusted = false;
                }
                if (currentPushable != null)
                {
                    currentPushable.ResetInteraction();
                    currentPushable = null;
                }
                isPushing = false;
                animator.SetBool(IsPushingHash, false);
                animator.SetFloat(PushPullDirectionHash, 0f);
                offsetSet = false;
                pushAxis = Vector3.zero;
                pushOffset = Vector3.zero;
                offsetSet = false;
            }

            // ---  Update state machine and other timers ---
            stateMachine.Update();
            HandleTimers();
            UpdateAnimator();
        }

        void HandlePushPull()
        {
            if (currentPushable == null || !offsetSet) return;
            Rigidbody rbPush = currentPushable.GetComponent<Rigidbody>();
            if (rbPush == null) return;
            if (!groundChecker.IsGrounded)
            {
                isPushing = false;
                animator.SetBool(IsPushingHash, false);
                animator.SetFloat(PushPullDirectionHash, 0f);
                return;
            }


            if (!dragAdjusted)
            {
                originalPlayerDrag = rb.linearDamping;
                originalPlayerAngularDrag = rb.angularDamping;
                originalPushableDrag = rbPush.linearDamping;
                originalPushableAngularDrag = rbPush.angularDamping;

                rb.linearDamping = 0f;
                rb.angularDamping = 0f;
                rbPush.linearDamping = 0f;
                rbPush.angularDamping = 0f;

                dragAdjusted = true;
                //rb.isKinematic = true;
            }

            // Only process push/pull if there is movement input
            if (input.Direction.x == 0 && input.Direction.y == 0)
            {
                // No input: do not move player or box, prevent drift
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
                if (rbPush != null)
                {
                    rbPush.linearVelocity = Vector3.zero;
                }
                isPushing = true;
                animator.SetBool(IsPushingHash, true);
                animator.SetFloat(PushPullDirectionHash, 0f);
                return;
            }


            Vector3 inputDir = Quaternion.AngleAxis(mainCam.eulerAngles.y, Vector3.up) * movement;

            // Calculate dot products for all four local axes
            float forwardAmount = Vector3.Dot(inputDir, currentPushable.transform.forward);
            float backwardAmount = Vector3.Dot(inputDir, -currentPushable.transform.forward);
            float rightAmount = Vector3.Dot(inputDir, currentPushable.transform.right);
            float leftAmount = Vector3.Dot(inputDir, -currentPushable.transform.right);

            // Find the dominant direction by direct comparison
            float absForward = Mathf.Abs(forwardAmount);
            float absBackward = Mathf.Abs(backwardAmount);
            float absRight = Mathf.Abs(rightAmount);
            float absLeft = Mathf.Abs(leftAmount);
            float maxAmount = absForward;
            string dominant = "Forward";
            Vector3 axis = currentPushable.transform.forward;
            float axisAmount = forwardAmount;
            if (absBackward >= maxAmount)
            {
                maxAmount = absBackward;
                dominant = "Backward";
                axis = -currentPushable.transform.forward;
                axisAmount = backwardAmount;
            }
            if (absRight >= maxAmount)
            {
                maxAmount = absRight;
                dominant = "Right";
                axis = currentPushable.transform.right;
                axisAmount = rightAmount;
            }
            if (absLeft >= maxAmount)
            {
                maxAmount = absLeft;
                dominant = "Left";
                axis = -currentPushable.transform.right;
                axisAmount = leftAmount;
            }
            // Universal push/pull detection: +1 for push (toward box), -1 for pull (away from box)
            Vector3 playerToBox = (currentPushable.transform.position - transform.position).normalized;

            float animDirection = Vector3.Dot(inputDir, playerToBox) >= 0 ? 1f : -1f;
            Debug.Log($"forward: {forwardAmount}, backward: {backwardAmount}, right: {rightAmount}, left: {leftAmount}, max: {maxAmount}, dominant: {dominant}, animDirection: {animDirection}");

            // Play push or pull sound effect
            if (AudioManager.Instance != null)
            {
                if (animDirection >= 0.5f)
                    AudioManager.Instance.PlayPush();
                else if (animDirection <= -0.5f)
                    AudioManager.Instance.PlayPull();
            }

            if (Mathf.Abs(axisAmount) < 0.05f)
            {
                // No significant input: do not move player or box, prevent drift
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
                if (rbPush != null)
                {
                    rbPush.linearVelocity = Vector3.zero;
                }
                isPushing = true;
                animator.SetBool(IsPushingHash, true);
                animator.SetFloat(PushPullDirectionHash, 0f);
                return;
            }

            Vector3 moveDelta = axis.normalized * Mathf.Sign(axisAmount) * pushSpeedLocal * Time.deltaTime;
            moveDelta.y = 0f;

            rbPush.MovePosition(rbPush.position + moveDelta);
            rb.MovePosition(rbPush.position + pushOffset);

            isPushing = true;
            animator.SetBool(IsPushingHash, true);
            animator.SetFloat(PushPullDirectionHash, Mathf.Clamp(animDirection, -1f, 1f));
        }

        void OnPush(bool performed)
        {
            pushHeld = performed;

            if (performed)
            {
                Debug.Log($"OnPush called: performed={performed} (left click detected)");
                if (Physics.Raycast(transform.position + Vector3.up * pushCheckHeight,
                                    transform.forward,
                                    out RaycastHit hit, pushCheckDistance))
                {
                    if (hit.collider.CompareTag("Pushable"))
                    {
                        currentPushable = hit.collider.GetComponent<Pushable>();
                        if (currentPushable != null && !currentPushable.IsInteracted)
                        {
                            currentPushable.Interact();
                            pushOffset = transform.position - currentPushable.transform.position;
                            offsetSet = true;
                        }
                    }
                }
            }
            else
            {
                if (currentPushable != null)
                {
                    Rigidbody rbPush = currentPushable.GetComponent<Rigidbody>();
                    if (rbPush != null)
                    {
                        // Kill all momentum
                        rbPush.linearVelocity = Vector3.zero;
                        rbPush.angularVelocity = Vector3.zero;

                        // Reset interaction → turn kinematic back on
                        currentPushable.ResetInteraction();

                        // Restore drag
                        rbPush.linearDamping = originalPushableDrag;
                        rbPush.angularDamping = originalPushableAngularDrag;
                    }
                }

                //rb.isKinematic = false;
                rb.linearDamping = originalPlayerDrag;
                rb.angularDamping = originalPlayerAngularDrag;

                dragAdjusted = false;
                offsetSet = false;
                currentPushable = null;
                isPushing = false;
                pushOffset = Vector3.zero;
                pushAxis = Vector3.zero;

                animator.SetBool(IsPushingHash, false);
                animator.SetFloat(PushPullDirectionHash, 0f);
            }
        }


        void FixedUpdate()
        {
            if (!MovementEnabled)
            {
                rb.linearVelocity = Vector3.zero;
                isPushing = false;
                return;
            }

            stateMachine.FixedUpdate();
        }

        void HandleTimers()
        {
            foreach (var timer in timers)
                timer.Tick(Time.deltaTime);
        }

        public void HandleJump()
        {
            if (!jumpTimer.IsRunning && groundChecker.IsGrounded)
            {
                jumpVelocity = 0f;
                jumpTimer.Stop();
                return;
            }

            if (!jumpTimer.IsRunning)
                jumpVelocity += Physics.gravity.y * gravity * Time.fixedDeltaTime;

            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpVelocity, rb.linearVelocity.z);
        }

        void UpdateAnimator()
        {
            animator.SetFloat(SpeedHash, currentSpeed);
            animator.SetBool(IsPushingHash, isPushing);
        }

        public void HandleMovement()
        {
            // Only block movement if actively pushing a box
            if (pushHeld && currentPushable != null && currentPushable.IsInteracted)
                return;

            var adjustedDirection = Quaternion.AngleAxis(mainCam.eulerAngles.y, Vector3.up) * movement;

            if (adjustedDirection.magnitude > 0f)
            {
                isPushing = CheckPushable();
                // Only rotate if not pushing/pulling
                if (!(pushHeld && currentPushable != null && currentPushable.IsInteracted))
                    HandleRotation(adjustedDirection);
                // Always allow movement unless actively pushing a box
                HandleHorizontalMovement(adjustedDirection);
                SmoothSpeed(adjustedDirection.magnitude);
            }
            else
            {
                SmoothSpeed(0f);
                isPushing = false;
                // Zero horizontal velocity when not moving
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            }
        }
        void HandleHorizontalMovement(Vector3 adjustedDirection)
        {
            // var adjustedMovement = adjustedDirection * (moveSpeed * Time.deltaTime);
            // characterController.Move(adjustedMovement);
            //speed is distance/time. 
            Vector3 velocity = adjustedDirection * moveSpeed * Time.fixedDeltaTime;

            rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);


        }
        public void DisableMovement()
        {
            MovementEnabled = false;
        }
        public void EnableMovement()
        {
            MovementEnabled = true;
        }
        bool CheckPushable()
        {
            var origin = transform.position + Vector3.up * pushCheckHeight;
            var dir = transform.forward;

            Debug.DrawRay(origin, dir * pushCheckDistance, Color.green);

            if (Physics.Raycast(origin, dir, out RaycastHit hit, pushCheckDistance))
            {
                if (hit.collider.CompareTag("Pushable"))
                {
                    var pushable = hit.collider.GetComponent<Pushable>();
                    if (pushable != null && pushable.IsInteracted)
                        return true;
                }
            }
            return false;
        }

        void HandleRotation(Vector3 adjustedDirection)
        {
            var targetRotation = Quaternion.LookRotation(adjustedDirection);
            if (!isPushing)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

        }

        void SmoothSpeed(float value)
        {
            currentSpeed = Mathf.SmoothDamp(currentSpeed, value, ref velocity, smoothTime);
        }

        public void ForceExitPushState()
        {
            isPushing = false;
            animator.SetBool(IsPushingHash, false);
            stateMachine?.Update();
        }
        public void OnFootstep()
        {
            if (groundChecker.IsGrounded)
            {
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlayFootstep();
            }
        }
    }
}
