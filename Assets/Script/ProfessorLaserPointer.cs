
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Ubiq.Messaging;

public class ProfessorLaserPointer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HostManager hostManager;
    [SerializeField] private Transform rightController;

    [Header("Input")]
    [SerializeField] private InputActionReference pointerAction;

    [Header("Laser Appearance")]
    [SerializeField] private Material laserMaterial;
    [SerializeField] private Color laserColor = Color.cyan;
    [SerializeField] private float laserLength = 15f;
    [SerializeField] private float laserWidth = 0.008f;

    [Header("Networking")]
    [SerializeField] private float sendInterval = 0.05f;
    [SerializeField] private float remoteTimeout = 1f;

    private NetworkContext context;
    private LineRenderer laserLine;

    private bool laserActive;
    private float nextSendTime;
    private float lastRemoteUpdateTime;
    private bool networkReady;

    [Serializable]
    private struct LaserMessage
    {
        public bool active;
        public Vector3 origin;
        public Vector3 direction;
    }

    private void Awake()
    {
        laserLine = GetComponent<LineRenderer>();

        if (laserLine == null)
        {
            laserLine = gameObject.AddComponent<LineRenderer>();
        }

        laserLine.positionCount = 2;
        laserLine.startWidth = laserWidth;
        laserLine.endWidth = laserWidth;
        laserLine.useWorldSpace = true;
        laserLine.startColor = laserColor;
        laserLine.endColor = laserColor;

        if (laserMaterial != null)
        {
            laserLine.material = laserMaterial;
        }

        laserLine.enabled = false;
    }

    private void Start()
    {
        context = NetworkScene.Register(this);
        networkReady = true;

        Debug.Log(
            "ProfessorLaserPointer: Registered with Ubiq.");
    }

    private void OnEnable()
    {
        if (pointerAction != null &&
            pointerAction.action != null)
        {
            pointerAction.action.Enable();
        }

        if (hostManager != null)
        {
            hostManager.OnRoleChanged += OnRoleChanged;
        }
    }

    private void OnDisable()
    {
        if (hostManager != null)
        {
            hostManager.OnRoleChanged -= OnRoleChanged;
        }

        if (networkReady &&
            hostManager != null &&
            hostManager.IsHost &&
            laserActive)
        {
            SendLaserMessage(
                false,
                Vector3.zero,
                Vector3.forward);
        }

        if (pointerAction != null &&
            pointerAction.action != null)
        {
            pointerAction.action.Disable();
        }

        HideLaser();
    }

    private void Update()
    {
        if (hostManager == null)
        {
            HideLaser();
            return;
        }

        if (!hostManager.IsHost)
        {
            // Remote pointers are controlled by Ubiq messages.
            // Hide stale pointers if the host disconnects.
            if (laserActive &&
                Time.time - lastRemoteUpdateTime > remoteTimeout)
            {
                HideLaser();
            }

            return;
        }

        if (rightController == null ||
            pointerAction == null ||
            pointerAction.action == null)
        {
            if (laserActive)
            {
                StopPointing();
            }

            return;
        }

        bool pressed = pointerAction.action.IsPressed();

        if (pressed)
        {
            Vector3 origin = rightController.position;
            Vector3 direction = rightController.forward;

            UpdateLaser(origin, direction, true);

            if (Time.time >= nextSendTime)
            {
                nextSendTime =
                    Time.time + Mathf.Max(0.02f, sendInterval);

                SendLaserMessage(true, origin, direction);
            }
        }
        else if (laserActive)
        {
            StopPointing();
        }
    }

    private void OnRoleChanged()
    {
        if (hostManager == null ||
            !hostManager.IsHost)
        {
            HideLaser();
        }
    }

    private void StopPointing()
    {
        HideLaser();

        SendLaserMessage(
            false,
            Vector3.zero,
            Vector3.forward);
    }

    private void HideLaser()
    {
        laserActive = false;

        if (laserLine != null)
        {
            laserLine.enabled = false;
        }
    }

    private void UpdateLaser(
        Vector3 origin,
        Vector3 direction,
        bool active)
    {
        laserActive = active;
        laserLine.enabled = active;

        if (!active)
        {
            return;
        }

        Vector3 endpoint =
            origin + direction.normalized * laserLength;

        laserLine.SetPosition(0, origin);
        laserLine.SetPosition(1, endpoint);
    }

    private void SendLaserMessage(
        bool active,
        Vector3 origin,
        Vector3 direction)
    {
        if (!networkReady || context.Scene == null)
        {
            return;
        }

        LaserMessage message = new LaserMessage
        {
            active = active,
            origin = origin,
            direction = direction
        };

        context.SendJson(message);
    }

    public void ProcessMessage(
        ReferenceCountedSceneGraphMessage networkMessage)
    {
        if (hostManager != null &&
            hostManager.IsHost)
        {
            return;
        }

        LaserMessage message =
            networkMessage.FromJson<LaserMessage>();

        lastRemoteUpdateTime = Time.time;

        UpdateLaser(
            message.origin,
            message.direction,
            message.active);
    }
}
