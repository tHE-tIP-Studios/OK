// GENERATED AUTOMATICALLY FROM 'Assets/InputSystem/FishingControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @FishingControls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @FishingControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""FishingControls"",
    ""maps"": [
        {
            ""name"": ""Rod"",
            ""id"": ""5bb206aa-3ac3-45ac-96ab-b3921594f5d8"",
            ""actions"": [
                {
                    ""name"": ""Throw"",
                    ""type"": ""Button"",
                    ""id"": ""d0531901-48da-4cff-aed9-5afdff6b2fb9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""e971c487-419f-4ef8-94bd-287e028260f4"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Throw"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Rod
        m_Rod = asset.FindActionMap("Rod", throwIfNotFound: true);
        m_Rod_Throw = m_Rod.FindAction("Throw", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Rod
    private readonly InputActionMap m_Rod;
    private IRodActions m_RodActionsCallbackInterface;
    private readonly InputAction m_Rod_Throw;
    public struct RodActions
    {
        private @FishingControls m_Wrapper;
        public RodActions(@FishingControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Throw => m_Wrapper.m_Rod_Throw;
        public InputActionMap Get() { return m_Wrapper.m_Rod; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(RodActions set) { return set.Get(); }
        public void SetCallbacks(IRodActions instance)
        {
            if (m_Wrapper.m_RodActionsCallbackInterface != null)
            {
                @Throw.started -= m_Wrapper.m_RodActionsCallbackInterface.OnThrow;
                @Throw.performed -= m_Wrapper.m_RodActionsCallbackInterface.OnThrow;
                @Throw.canceled -= m_Wrapper.m_RodActionsCallbackInterface.OnThrow;
            }
            m_Wrapper.m_RodActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Throw.started += instance.OnThrow;
                @Throw.performed += instance.OnThrow;
                @Throw.canceled += instance.OnThrow;
            }
        }
    }
    public RodActions @Rod => new RodActions(this);
    public interface IRodActions
    {
        void OnThrow(InputAction.CallbackContext context);
    }
}
