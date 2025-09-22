using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float _speed = 1f;
    [SerializeField] float _lookXSensitivity = 1f;
    [SerializeField] float _lookYSensitivity = 1f;

    CharacterController _controller;
    Camera _camera;

    Vector3 _velocity = Vector3.zero;
    float _gravity = -9.8f;
    float _verticalRotation = 0f;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        if (_controller == null)
            _controller = gameObject.AddComponent<CharacterController>();

        _camera = Camera.main;
    }

    private void Start()
    {
        InputManager.Instance.OnMove += Move;
        InputManager.Instance.OnLook += Look;
    }

    private void Update()
    {
        _velocity.y += _gravity * Time.deltaTime;
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
        _verticalRotation = Mathf.Clamp(_verticalRotation, -90f, 90f);
        _camera.transform.localRotation = Quaternion.Euler(_verticalRotation, 0, 0);

        transform.Rotate(Vector3.up * input.x * Time.deltaTime * _lookXSensitivity);
    }
}
