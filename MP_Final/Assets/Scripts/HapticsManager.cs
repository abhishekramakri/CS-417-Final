using UnityEngine;
using UnityEngine.XR;

public class HapticsManager : MonoBehaviour
{
    public static HapticsManager Instance;

    void Awake()
    {
        Instance = this;
    }

    // Simple vibration pulse
    public void Pulse(float amplitude, float duration)
    {
        // Left controller
        TriggerHaptics(XRNode.LeftHand, amplitude, duration);

        // Right controller
        TriggerHaptics(XRNode.RightHand, amplitude, duration);
    }

    void TriggerHaptics(XRNode node, float amplitude, float duration)
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(node);

        if (device.isValid)
        {
            device.SendHapticImpulse(0u, amplitude, duration);
        }
    }
}