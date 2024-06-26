using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using LootLocker;
using LootLocker.Requests;

namespace EndlessRun.Player
{
    [RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour {

        [SerializeField] private float initialSpeed = 8f;
        [SerializeField] private float maxSpeed = 30f;
        [SerializeField] private float speedIncreaseRate = .1f;
        [SerializeField] private float jumpHeight = 1.3f;
        [SerializeField] private float initialGravityValue = -9.81f;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private LayerMask turnLayer; 
        [SerializeField] private LayerMask obstacleLayer;
        [SerializeField] private Animator animator;
        [SerializeField] private AnimationClip slideAnimationClip;


        [SerializeField] private float playerSpeed;
        [SerializeField] private float scoreMultiplier = 10f;
        [SerializeField] private GameObject mesh;

        private float gravity;
        private Vector3 movementDirection = Vector3.forward;
        private Vector3 playerVelocity;


        private PlayerInput playerInput;
        private InputAction turnAction;
        private InputAction jumpAction;
        private InputAction slideAction;
        private CharacterController controller;
        private int slidingAnimationId;

        private bool sliding = false;
        private float score = 0;
        [SerializeField] private UnityEvent<Vector3> turnEvent;
        [SerializeField] private UnityEvent<int> gameOverEvent;
        [SerializeField] private UnityEvent<int> scoreUpdateEvent;

        private void Awake() {
            playerInput = GetComponent<PlayerInput>();
            controller = GetComponent<CharacterController>();
                        
            slidingAnimationId = Animator.StringToHash("Sliding");

            turnAction = playerInput.actions["Turn"];
            jumpAction = playerInput.actions["Jump"];
            slideAction = playerInput.actions["Slide"];
        }

        private void OnEnable() {
            turnAction.performed += PlayerTurn;
            slideAction.performed += PlayerSlide;
            jumpAction.performed += PlayerJump;
        }
        private void OnDisable() {
            turnAction.performed -= PlayerTurn;
            slideAction.performed -= PlayerSlide;
            jumpAction.performed -= PlayerJump;
        }

        private void Start() {
            playerSpeed = initialSpeed;
            gravity = initialGravityValue;
            StartCoroutine(ChangePlayerMaterial());
        }

        private IEnumerator ChangePlayerMaterial(){
            string color = "";
            bool? playerSkinRequest = null;
            LootLockerSDKManager.GetSingleKeyPersistentStorage("skin",(response)=>{
                if(response.success){
                    Debug.Log("Successfully retrieved player Storage with value" + response.payload.value);
                    color = response.payload.value;
                    playerSkinRequest = true;
                }else {
                    Debug.Log("UnSuccessfully retrieved player Storage with value" + response.payload.value);
                    playerSkinRequest = false;
                }
            });
            yield return new WaitUntil(() => playerSkinRequest.HasValue);
            if (playerSkinRequest.Value) {
                if (ColorUtility.TryParseHtmlString(color, out Color newColor)){
                    mesh.GetComponent<MeshRenderer>().material.color = newColor;
                }
            }
        }

        private void PlayerTurn(InputAction.CallbackContext context) {
            Vector3? turnPosition =  CheckTurn(context.ReadValue<float>());
            if (!turnPosition.HasValue)
            {
                GameOver();
                return;
            }
            Vector3 targetDirection = Quaternion.AngleAxis(90 * context.ReadValue<float>(),Vector3.up) *
                movementDirection;

            turnEvent.Invoke(targetDirection);
            Turn(context.ReadValue<float>(),turnPosition.Value);

        }

        private Vector3? CheckTurn(float turnValue) {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, .1f, turnLayer);
            if (hitColliders.Length != 0) {
                Tile tile = hitColliders[0].transform.parent.GetComponent<Tile>();
                TileType type = tile.type;

                if ((type == TileType.LEFT && turnValue == -1 ) || 
                    (type == TileType.RIGHT && turnValue == 1)  ||
                    (type == TileType.SIDEWAYS)) {
                    return tile.pivot.position;
                }
            }
            return null;
        }

        private void Turn(float turnValue,Vector3 turnPosition) {
            Vector3 tempPlayerPosition = new Vector3(turnPosition.x,turnPosition.y,turnPosition.z);
            controller.enabled = false;
            transform.position = tempPlayerPosition;
            controller.enabled = true;

            Quaternion targetRotation = transform.rotation * Quaternion.Euler(0, 90 * turnValue,0);
            transform.rotation = targetRotation;
            movementDirection = transform.forward.normalized;
        }

        private void PlayerSlide(InputAction.CallbackContext context) {
            
            if (!sliding && IsGrounded())
            {
                StartCoroutine(Slide());    
            }

        }
    
        private IEnumerator Slide() {

            sliding = true;

            Vector3 originalControllerCenter = controller.center;
            Vector3 newControllerCenter = originalControllerCenter;
            
            controller.height /=2;
            newControllerCenter.y -= controller.height/2;
            controller.center = newControllerCenter;
            
            animator.Play(slidingAnimationId);
            yield return new WaitForSeconds(slideAnimationClip.length / animator.speed);

            controller.height *=2;
            controller.center = originalControllerCenter;
            sliding = false;

        }
    
        private void PlayerJump(InputAction.CallbackContext context) {

            if (IsGrounded())
            {
                playerVelocity.y += Mathf.Sqrt(jumpHeight * gravity * -3f);
                controller.Move(playerVelocity * Time.deltaTime);
            }
        }

        // Update is called once per frame
        private void Update() {

            // if (!IsGrounded(10f)){
            //     GameOver();
            //     return;
            // }

            score += scoreMultiplier * Time.deltaTime;
            scoreUpdateEvent.Invoke((int)score);

            controller.Move(transform.forward * playerSpeed * Time.deltaTime);
            if (IsGrounded() && playerVelocity.y < 0)
            {
                playerVelocity.y = 0f;
            }
            playerVelocity.y += gravity * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);

            if (playerSpeed < maxSpeed){
                playerSpeed += Time.deltaTime * speedIncreaseRate;
                gravity = initialGravityValue - playerSpeed;

                if (animator.speed < 1.25f)
                {
                    animator.speed += (1/  playerSpeed) * Time.deltaTime;
                }
            }

        }

        private bool IsGrounded(float length = .2f) {
            
            Vector3 raycastOriginFirst = transform.position;
            raycastOriginFirst.y -= controller.height / 2f;
            raycastOriginFirst.y += .1f;

            Vector3 raycastOriginSecond = raycastOriginFirst;
            raycastOriginFirst -= transform.forward * .2f;
            raycastOriginSecond += transform.forward * .2f;

            if (Physics.Raycast(raycastOriginFirst, Vector3.down, out RaycastHit hit, length, groundLayer) ||
                Physics.Raycast(raycastOriginSecond, Vector3.down, out RaycastHit hit2, length, groundLayer))
            {
                return true;
            }
            return false;
        
        }

        private void GameOver() {
            Debug.Log("Game over");
            gameOverEvent.Invoke((int)score);
            gameObject.SetActive(false);
        }

        private void OnControllerColliderHit(ControllerColliderHit hit) {
        
            if(((1 <<hit.collider.gameObject.layer) & obstacleLayer) != 0){
                GameOver();    
            }
        }
    }
}