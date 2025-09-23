using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    PlayerInput _playerInput;
    PlayerInput.MovingActions _moving;
    PlayerInput.FishingActions _fishing;

    public event Action<Vector2> OnMove;
    public event Action<Vector2> OnLook;
    public event Action OnBeginCharge;
    public event Action OnReleaseCharge;

    public event Action OnHookFish;
    public event Action OnReelIn;

    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        _playerInput = new PlayerInput();
        _moving = _playerInput.Moving;
        _fishing = _playerInput.Fishing;

        _moving.ChargeBobber.performed += ctx => OnBeginCharge?.Invoke();
        _moving.CastBobber.performed += ctx => OnReleaseCharge?.Invoke();
        _fishing.Hook.performed += ctx => OnHookFish?.Invoke();
        _fishing.Reel.performed += ctx => OnReelIn?.Invoke();

        ToggleMovement(true);
        ToggleFishing(false);
    }

    void Update()
    {
        Vector2 moveInput = _moving.Move.ReadValue<Vector2>();
        if(moveInput != Vector2.zero)
            OnMove?.Invoke(moveInput);

        Vector2 lookInput = _moving.Look.ReadValue<Vector2>();
        if (lookInput != Vector2.zero)
            OnLook?.Invoke(lookInput);
    }

    public void ToggleMovement(bool isEnabled)
    {
        if (isEnabled)
            _moving.Enable();
        else 
            _moving.Disable();
    }

    public void ToggleFishing(bool isEnabled)
    {
        if (isEnabled)
            _fishing.Enable();
        else
            _fishing.Disable();
    }
}
