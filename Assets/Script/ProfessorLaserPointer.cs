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

    private NetworkContext context;
    private LineRenderer laserLine;

    private bool laserActive;
    private float nextSendTime;

    [Serializable]
    private struct LaserMessage
    {
        public bool active;
        public Vector3 origin;
        public Vector3 direction;
    }

    private void Awake()
    {
        laserLine = gameObject.AddComponent<LineRenderer>();

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
    }

    private void OnDisable()
    {
        if (pointerAction != null &&
            pointerAction.action != null)
        {
            pointerAction.action.Disable();
        }

        if (laserLine != null)
        {
            laserLine.enabled = false;
        }
    }

    private void Update()
    {
        if (hostManager == null ||
            !hostManager.IsHost)
        {
            return;
        }

        if (rightController == null ||
            pointerAction == null)
        {
            return;
        }

        bool pressed =
            pointerAction.action.IsPressed();

        if (pressed)
        {
            Vector3 origin =
                rightController.position;

            Vector3 direction =
                rightController.forward;

            UpdateLaser(origin, direction, true);

            if (Time.time >= nextSendTime)
            {
                nextSendTime =
                    Time.time + sendInterval;

                SendLaserMessage(
                    true,
                    origin,
                    direction);
            }
        }
        else if (laserActive)
        {
            UpdateLaser(
                Vector3.zero,
                Vector3.forward,
                false);

            SendLaserMessage(
                false,
                Vector3.zero,
                Vector3.forward);
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
        if (context.Scene == null)
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
        // The host controls the pointer locally.
        if (hostManager != null &&
            hostManager.IsHost)
        {
            return;
        }

        LaserMessage message =
            networkMessage.FromJson<LaserMessage>();

        UpdateLaser(
            message.origin,
            message.direction,
            message.active);
    }
}