using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController),typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    
    [SerializeField] private float initialSpeed = 4f;
    [SerializeField] private float maxSpeed = 30f;
    [SerializeField] private float speedIncreaseRate = .1f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float initialGravityValue = -9.81f;
    [SerializeField] private LayerMask groundLayer;
    
    private float playerSpeed;
    private float gravity;
    private Vector3 movementDirection = Vector3.forward;
    private PlayerInput playerInput;
    private InputAction turnAction;
    private InputAction jumpAction;
    private InputAction slideAction;
    private CharacterController controller;

    private void Awake() {
        playerInput = GetComponent<PlayerInput>();
        controller = GetComponent<CharacterController>();
        turnAction = playerInput.actions["Turn"];
        jumpAction = playerInput.actions["Jump"];
        slideAction = playerInput.actions["Slide"];
    }

    private void OnEnable() {
        /*turnAction.performed += PlayerTurn();
        slideAction.performed += PlayerSlide();
        jumpAction.performed += PlayerJump();*/
    }
    private void OnDisable() {
        /*turnAction.performed -= PlyerTurn();
        slideAction.performed -= PlyerSlide();
        jumpAction.performed -= PlyerJump();*/
    }
    
    void Start()
    {
        playerSpeed = initialSpeed;
        gravity = initialGravityValue;
    }

    private void PlayerTurn(InputAction.CallbackContext context){

    }
    private void PlayerSlide(InputAction.CallbackContext context){

    }
    private void PlayerJump(InputAction.CallbackContext context){

    }

    // Update is called once per frame
    void Update()
    { 
        controller.Move(transform.forward * playerSpeed * Time.deltaTime);
    }

    private bool IsGrounded(float length){
        Vector3 raycastOriginFirst = transform.position;
        raycastOriginFirst.y -= controller.height / 2f;
        raycastOriginFirst.y += .1f;

        Vector3 raycastOriginSecond = raycastOriginFirst;
        raycastOriginFirst -= transform.forward * .2f;
        raycastOriginSecond += transform.forward * .2f;

        Debug.DrawLine(raycastOriginFirst,Vector3.down,Color.green,2f);
        Debug.DrawLine(raycastOriginSecond,Vector3.down,Color.red,2f);

        if (Physics.Raycast(raycastOriginFirst, Vector3.down, out RaycastHit hit, length, groundLayer) || 
            Physics.Raycast(raycastOriginSecond, Vector3.down, out RaycastHit hit2, length, groundLayer)) {
            return true;
        }
        return false;
    }
}
