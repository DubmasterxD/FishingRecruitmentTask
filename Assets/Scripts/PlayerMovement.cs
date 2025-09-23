using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float _speed = 1f;
    [SerializeField] float _lookXSensitivity = 1f;
    [SerializeField] float _lookYSensitivity = 1f;

    CharacterController _controller;
    Camera _camera;

    bool _isLookingLocked;
    Vector3 _lockDirection;
    float _verticalRotation;

    Vector3 _velocity;
    const float GRAVITY = -9.8f;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        if (_controller == null)
            _controller = gameObject.AddComponent<CharacterController>();

        _camera = Camera.main;
        _isLookingLocked = false;
        _velocity = Vector3.zero;
        _verticalRotation = 0f;
    }

    private void Start()
    {
        InputManager.Instance.OnMove += Move;
        InputManager.Instance.OnLook += Look;
    }

    private void Update()
    {
        _velocity.y += GRAVITY * Time.deltaTime;
        if (_controller.isGrounded)
            _velocity.y = 0;
        _controller.Move(_velocity * Time.deltaTime);
    }

    void Move(Vector2 input)
    {
        Vector3 moveDirection = new Vector3(input.x, 0, input.y);
        _controller.Move(transform.TransformDirection(moveDirection) * Time.deltaTime * _speed);
    }

    void Look(Vector2 input)
    {
        _verticalRotation -= input.y * Time.deltaTime * _lookYSensitivity;
        _verticalRotation = Mathf.Clamp(_verticalRotation, -20f, 30f);
        _camera.transform.localRotation = Quaternion.Euler(_verticalRotation, 0, 0);

        Vector3 horizontalRotation = Vector3.up * input.x * Time.deltaTime * _lookXSensitivity;
        transform.Rotate(horizontalRotation);

        if(_isLookingLocked)
        {
            float lockedAngle = Vector3.Angle(transform.forward, _lockDirection);
            if(lockedAngle > 25f)
            {
                transform.Rotate(-horizontalRotation);
            }
        }
    }

    public void LockLookDirection()
    {
        _isLookingLocked = true;
        _lockDirection = transform.forward;
    }

    public void UnlockLookDirection()
    {
        _isLookingLocked = false;
    }
}
