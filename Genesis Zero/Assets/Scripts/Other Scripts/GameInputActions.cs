// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/Other Scripts/GameInputActions.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @GameInputActions : IInputActionCollection, IDisposable
{
    private InputActionAsset asset;
    public @GameInputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""GameInputActions"",
    ""maps"": [
        {
            ""name"": ""PlayerControls"",
            ""id"": ""3d6c98bf-f28d-41e8-91c7-fef8e9b8ca48"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""PassThrough"",
                    ""id"": ""ccc0f839-e827-467c-b4ee-be18c02ca35e"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""AimMouse"",
                    ""type"": ""PassThrough"",
                    ""id"": ""95a719cb-580b-4379-94fd-1776eeaec549"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""AimController"",
                    ""type"": ""PassThrough"",
                    ""id"": ""0a784de5-0df8-4345-81b1-6cb16db6728f"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""PassThrough"",
                    ""id"": ""be22d5e4-0e46-40f3-9252-c020cfafbd37"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(pressPoint=0.1)""
                },
                {
                    ""name"": ""Fire"",
                    ""type"": ""PassThrough"",
                    ""id"": ""5a85d603-2e77-42c4-8bdd-536b415de140"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Roll"",
                    ""type"": ""PassThrough"",
                    ""id"": ""d34c41c0-3e49-4d96-8e5e-2de4da6a116c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(pressPoint=0.1)""
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""PassThrough"",
                    ""id"": ""37cdc43c-1b80-4a86-9c7c-e7179db50e60"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(pressPoint=0.1)""
                },
                {
                    ""name"": ""Pause"",
                    ""type"": ""PassThrough"",
                    ""id"": ""c5164900-1a6b-47e7-8d60-2b4b2bc1f4da"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(pressPoint=0.1)""
                },
                {
                    ""name"": ""Ability_1"",
                    ""type"": ""PassThrough"",
                    ""id"": ""505a75dc-e693-4f74-b8ff-ae8e56606379"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(pressPoint=0.1)""
                },
                {
                    ""name"": ""Ability_2"",
                    ""type"": ""PassThrough"",
                    ""id"": ""3e098525-da3b-46aa-a1aa-01166e1e0314"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(pressPoint=0.1)""
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""1d3ca5e7-c417-4978-921a-692e5796624f"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""left"",
                    ""id"": ""95ad9263-5c6e-4974-bc0d-05ea38333fe6"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""951293b6-ffa9-4191-9ef3-f272faf07744"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Arrows"",
                    ""id"": ""57552d33-0232-4686-aa65-a9f0c23f0e62"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""left"",
                    ""id"": ""d92343d3-429f-4439-ae40-6f21f7509062"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""0e63c012-8f82-406b-bdc3-e827e5635d1d"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""fbcee267-9fd5-4240-af30-8d41cff01214"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepads"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b6feffaf-2c8d-4a14-b402-106d8813f640"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""AimMouse"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f9207609-d0e5-499a-934d-2010d2baab7c"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""40316d80-ce88-4164-b943-b89c208d3026"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepads"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0fb0233a-0dc1-4b2d-b593-6ac88d169a90"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c85a4b82-a8a0-4871-8a66-950d82bdba88"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Fire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""271a31f0-3ae3-4f6d-8aa1-d6d83a9bc692"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepads"",
                    ""action"": ""Fire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""54a9844b-62ee-4e59-a494-1c1b31377443"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Roll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""40390624-88f0-4c90-8485-f4cd3cbce95c"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepads"",
                    ""action"": ""Roll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b5839926-24aa-4340-ae7a-8836656242df"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepads"",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""21fc4626-1085-4dc8-81d0-3fdcf25c1670"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7c108ba2-49c9-40b4-9b24-2cf7fd7d360d"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""AimController"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1b4adb14-f008-46dd-9cc1-bddd287b884a"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepads"",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""73846aed-be45-4dc6-9058-949d583585cd"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""448bddf9-d0be-4464-a158-c1afb48bc31d"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Ability_1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f439d375-3560-45fc-b63b-8b9a5af94caf"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepads"",
                    ""action"": ""Ability_1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""78d30b40-61bf-43fc-b0a7-15c9894a9cec"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Ability_2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7799b530-2c93-41d1-be90-67b41d50edca"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepads"",
                    ""action"": ""Ability_2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""MenuControls"",
            ""id"": ""ead61177-0c3c-4604-8373-65c2e4d323e0"",
            ""actions"": [
                {
                    ""name"": ""Select"",
                    ""type"": ""PassThrough"",
                    ""id"": ""63a18198-8d8c-487e-b2ca-33af879bdfd9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(pressPoint=0.1)""
                },
                {
                    ""name"": ""Unpause"",
                    ""type"": ""PassThrough"",
                    ""id"": ""5750c837-e1e9-4e63-afcc-5da20ffc6761"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(pressPoint=0.1)""
                },
                {
                    ""name"": ""Move"",
                    ""type"": ""PassThrough"",
                    ""id"": ""b24e1496-76e7-4b67-a25b-64ac8937ff02"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""f8f6ef78-0b87-48c1-9b10-824c34f0efea"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Select"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""858b2311-0d78-4eb7-845c-1138f7711490"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepads"",
                    ""action"": ""Select"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fcfb9940-cd03-4a23-860c-7cc00c71c822"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""Gamepads"",
                    ""action"": ""Unpause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""95185444-479b-4974-9b4a-8c4f490cdef4"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Unpause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""143407b5-dc57-4443-9ade-3916ce32e956"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepads"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard&Mouse"",
            ""bindingGroup"": ""Keyboard&Mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Gamepads"",
            ""bindingGroup"": ""Gamepads"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // PlayerControls
        m_PlayerControls = asset.FindActionMap("PlayerControls", throwIfNotFound: true);
        m_PlayerControls_Move = m_PlayerControls.FindAction("Move", throwIfNotFound: true);
        m_PlayerControls_AimMouse = m_PlayerControls.FindAction("AimMouse", throwIfNotFound: true);
        m_PlayerControls_AimController = m_PlayerControls.FindAction("AimController", throwIfNotFound: true);
        m_PlayerControls_Jump = m_PlayerControls.FindAction("Jump", throwIfNotFound: true);
        m_PlayerControls_Fire = m_PlayerControls.FindAction("Fire", throwIfNotFound: true);
        m_PlayerControls_Roll = m_PlayerControls.FindAction("Roll", throwIfNotFound: true);
        m_PlayerControls_Interact = m_PlayerControls.FindAction("Interact", throwIfNotFound: true);
        m_PlayerControls_Pause = m_PlayerControls.FindAction("Pause", throwIfNotFound: true);
        m_PlayerControls_Ability_1 = m_PlayerControls.FindAction("Ability_1", throwIfNotFound: true);
        m_PlayerControls_Ability_2 = m_PlayerControls.FindAction("Ability_2", throwIfNotFound: true);
        // MenuControls
        m_MenuControls = asset.FindActionMap("MenuControls", throwIfNotFound: true);
        m_MenuControls_Select = m_MenuControls.FindAction("Select", throwIfNotFound: true);
        m_MenuControls_Unpause = m_MenuControls.FindAction("Unpause", throwIfNotFound: true);
        m_MenuControls_Move = m_MenuControls.FindAction("Move", throwIfNotFound: true);
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

    // PlayerControls
    private readonly InputActionMap m_PlayerControls;
    private IPlayerControlsActions m_PlayerControlsActionsCallbackInterface;
    private readonly InputAction m_PlayerControls_Move;
    private readonly InputAction m_PlayerControls_AimMouse;
    private readonly InputAction m_PlayerControls_AimController;
    private readonly InputAction m_PlayerControls_Jump;
    private readonly InputAction m_PlayerControls_Fire;
    private readonly InputAction m_PlayerControls_Roll;
    private readonly InputAction m_PlayerControls_Interact;
    private readonly InputAction m_PlayerControls_Pause;
    private readonly InputAction m_PlayerControls_Ability_1;
    private readonly InputAction m_PlayerControls_Ability_2;
    public struct PlayerControlsActions
    {
        private @GameInputActions m_Wrapper;
        public PlayerControlsActions(@GameInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_PlayerControls_Move;
        public InputAction @AimMouse => m_Wrapper.m_PlayerControls_AimMouse;
        public InputAction @AimController => m_Wrapper.m_PlayerControls_AimController;
        public InputAction @Jump => m_Wrapper.m_PlayerControls_Jump;
        public InputAction @Fire => m_Wrapper.m_PlayerControls_Fire;
        public InputAction @Roll => m_Wrapper.m_PlayerControls_Roll;
        public InputAction @Interact => m_Wrapper.m_PlayerControls_Interact;
        public InputAction @Pause => m_Wrapper.m_PlayerControls_Pause;
        public InputAction @Ability_1 => m_Wrapper.m_PlayerControls_Ability_1;
        public InputAction @Ability_2 => m_Wrapper.m_PlayerControls_Ability_2;
        public InputActionMap Get() { return m_Wrapper.m_PlayerControls; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerControlsActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerControlsActions instance)
        {
            if (m_Wrapper.m_PlayerControlsActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnMove;
                @AimMouse.started -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnAimMouse;
                @AimMouse.performed -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnAimMouse;
                @AimMouse.canceled -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnAimMouse;
                @AimController.started -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnAimController;
                @AimController.performed -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnAimController;
                @AimController.canceled -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnAimController;
                @Jump.started -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnJump;
                @Fire.started -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnFire;
                @Fire.performed -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnFire;
                @Fire.canceled -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnFire;
                @Roll.started -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnRoll;
                @Roll.performed -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnRoll;
                @Roll.canceled -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnRoll;
                @Interact.started -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnInteract;
                @Interact.performed -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnInteract;
                @Interact.canceled -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnInteract;
                @Pause.started -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnPause;
                @Pause.performed -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnPause;
                @Pause.canceled -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnPause;
                @Ability_1.started -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnAbility_1;
                @Ability_1.performed -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnAbility_1;
                @Ability_1.canceled -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnAbility_1;
                @Ability_2.started -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnAbility_2;
                @Ability_2.performed -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnAbility_2;
                @Ability_2.canceled -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnAbility_2;
            }
            m_Wrapper.m_PlayerControlsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @AimMouse.started += instance.OnAimMouse;
                @AimMouse.performed += instance.OnAimMouse;
                @AimMouse.canceled += instance.OnAimMouse;
                @AimController.started += instance.OnAimController;
                @AimController.performed += instance.OnAimController;
                @AimController.canceled += instance.OnAimController;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Fire.started += instance.OnFire;
                @Fire.performed += instance.OnFire;
                @Fire.canceled += instance.OnFire;
                @Roll.started += instance.OnRoll;
                @Roll.performed += instance.OnRoll;
                @Roll.canceled += instance.OnRoll;
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
                @Pause.started += instance.OnPause;
                @Pause.performed += instance.OnPause;
                @Pause.canceled += instance.OnPause;
                @Ability_1.started += instance.OnAbility_1;
                @Ability_1.performed += instance.OnAbility_1;
                @Ability_1.canceled += instance.OnAbility_1;
                @Ability_2.started += instance.OnAbility_2;
                @Ability_2.performed += instance.OnAbility_2;
                @Ability_2.canceled += instance.OnAbility_2;
            }
        }
    }
    public PlayerControlsActions @PlayerControls => new PlayerControlsActions(this);

    // MenuControls
    private readonly InputActionMap m_MenuControls;
    private IMenuControlsActions m_MenuControlsActionsCallbackInterface;
    private readonly InputAction m_MenuControls_Select;
    private readonly InputAction m_MenuControls_Unpause;
    private readonly InputAction m_MenuControls_Move;
    public struct MenuControlsActions
    {
        private @GameInputActions m_Wrapper;
        public MenuControlsActions(@GameInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Select => m_Wrapper.m_MenuControls_Select;
        public InputAction @Unpause => m_Wrapper.m_MenuControls_Unpause;
        public InputAction @Move => m_Wrapper.m_MenuControls_Move;
        public InputActionMap Get() { return m_Wrapper.m_MenuControls; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MenuControlsActions set) { return set.Get(); }
        public void SetCallbacks(IMenuControlsActions instance)
        {
            if (m_Wrapper.m_MenuControlsActionsCallbackInterface != null)
            {
                @Select.started -= m_Wrapper.m_MenuControlsActionsCallbackInterface.OnSelect;
                @Select.performed -= m_Wrapper.m_MenuControlsActionsCallbackInterface.OnSelect;
                @Select.canceled -= m_Wrapper.m_MenuControlsActionsCallbackInterface.OnSelect;
                @Unpause.started -= m_Wrapper.m_MenuControlsActionsCallbackInterface.OnUnpause;
                @Unpause.performed -= m_Wrapper.m_MenuControlsActionsCallbackInterface.OnUnpause;
                @Unpause.canceled -= m_Wrapper.m_MenuControlsActionsCallbackInterface.OnUnpause;
                @Move.started -= m_Wrapper.m_MenuControlsActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_MenuControlsActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_MenuControlsActionsCallbackInterface.OnMove;
            }
            m_Wrapper.m_MenuControlsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Select.started += instance.OnSelect;
                @Select.performed += instance.OnSelect;
                @Select.canceled += instance.OnSelect;
                @Unpause.started += instance.OnUnpause;
                @Unpause.performed += instance.OnUnpause;
                @Unpause.canceled += instance.OnUnpause;
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
            }
        }
    }
    public MenuControlsActions @MenuControls => new MenuControlsActions(this);
    private int m_KeyboardMouseSchemeIndex = -1;
    public InputControlScheme KeyboardMouseScheme
    {
        get
        {
            if (m_KeyboardMouseSchemeIndex == -1) m_KeyboardMouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard&Mouse");
            return asset.controlSchemes[m_KeyboardMouseSchemeIndex];
        }
    }
    private int m_GamepadsSchemeIndex = -1;
    public InputControlScheme GamepadsScheme
    {
        get
        {
            if (m_GamepadsSchemeIndex == -1) m_GamepadsSchemeIndex = asset.FindControlSchemeIndex("Gamepads");
            return asset.controlSchemes[m_GamepadsSchemeIndex];
        }
    }
    public interface IPlayerControlsActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnAimMouse(InputAction.CallbackContext context);
        void OnAimController(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnFire(InputAction.CallbackContext context);
        void OnRoll(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
        void OnPause(InputAction.CallbackContext context);
        void OnAbility_1(InputAction.CallbackContext context);
        void OnAbility_2(InputAction.CallbackContext context);
    }
    public interface IMenuControlsActions
    {
        void OnSelect(InputAction.CallbackContext context);
        void OnUnpause(InputAction.CallbackContext context);
        void OnMove(InputAction.CallbackContext context);
    }
}
