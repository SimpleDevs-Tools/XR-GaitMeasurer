using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GaitOscillateMeasurer : MonoBehaviour
{
    [Header("=== Required References ===")]
    [Tooltip("Reference to the Camera Rig parent Transform. NOT the Camera Offset")]    public Transform cameraRig = null;
    [Tooltip("Reference to the player's XR Head.")]                                     public Transform headRef = null;

    [Header("=== Outputs - READ ONLY ===")]
    [Tooltip("The measured minimum height in world space of the player's gait")]    public float minY = 0f;
    [Tooltip("The measured maximum height in world space of the player's gait")]    public float maxY = 0f;
    [Tooltip("The required height offset that needs to be applied to `cameraRig`")] public float heightOffset = 0f;
    [Space]
    public TextMeshProUGUI statusTextbox;
    public TextMeshProUGUI minTextbox;
    public TextMeshProUGUI maxTextbox;
    public TextMeshProUGUI heightOffsetTextbox;

    private bool m_measureInitialized = false;
    private bool m_correctInitialized = false;
    private bool m_applyInitialized = false;

    // PUBLICLY ACCESSIBLE. Toggles the "Measurer" stage.
    public void ToggleMeasurer() {
        if (!m_measureInitialized) StartMeasurer();
        else EndMeasurer();
    }
    // PUBLICLY ACCESSIBLE. Force intiializes the "Measurer" stage.
    public void StartMeasurer() {
        minY = headRef.localPosition.y;
        maxY = headRef.localPosition.y;
        m_measureInitialized = true;
        if (statusTextbox != null) statusTextbox.text = "Measurer Started";
    }
    // PUBLICLY ACCESSIBLE. Force ends the "Measurer" stage.
    public void EndMeasurer() {
        m_measureInitialized = false;
        if (statusTextbox != null) statusTextbox.text = "Measurer Ended";
    }

    private void MeasureOscillation() {
        float yPos = headRef.localPosition.y;
        if (yPos < minY) minY = yPos;
        if (yPos > maxY) maxY = yPos;

        if (minTextbox != null) minTextbox.text = $"Min: {minY.ToString()}";
        if (maxTextbox != null) maxTextbox.text = $"Max: {maxY.ToString()}";
    }

    // PUBLICLY ACCESSIBLE: Toggles the "Corrector" stage
    public void ToggleCorrector() {
        if (!m_correctInitialized) StartCorrector();
        else EndCorrector();
    }
    // PUBLICLY ACCESSIBLE: Force initializes the "Corrector" stage. Also forces the "Apply" stage to be activated too.
    public void StartCorrector() {
        m_correctInitialized = true;
        m_applyInitialized = true;
        if (statusTextbox != null) statusTextbox.text = "Corrector and Applier Started";
    }
    // PUBLICLY ACCESSIBLE: Force ends the "Corrector" stage. Does NOT end the "Apply" stage though.
    public void EndCorrector() {
        m_correctInitialized = false;
        if (statusTextbox != null) statusTextbox.text = "Corrector Ended";
    }

    public void ToggleApplier() {
        if (!m_applyInitialized) StartApplier();
        else EndApplier();
    }
    public void StartApplier() {
        m_applyInitialized = true;
        if (statusTextbox != null) statusTextbox.text = "Applier Started";
    }
    public void EndApplier() {
        m_applyInitialized = false;
        if (statusTextbox != null) statusTextbox.text = "Applier Ended";
    }

    // PUBLICLY ACCESSIBLE: force-resets any cached data
    public void Reset() {
        heightOffset = 0f;
        minY = 0f;
        maxY = 0f;

        m_measureInitialized = false;
        m_correctInitialized = false;
        m_applyInitialized = false;
    }

    private void Update() {
        if (headRef == null || cameraRig == null) return;
        if (m_measureInitialized) MeasureOscillation();
        if (m_correctInitialized) CorrectHeight();
        ApplyHeight();
    }

    // The running theory behind this is that we control the Y position of the camera rig based on the current world-scale position of the head.
    // If we're sinking into the ground, then we'll notice that the y world positions will be getting smaller than the minimum y oscillation height.
    // Likewise, if we're floating, then we'll notice that the y world position will be bigger than the max oscillation height.
    // We need to maintain a height offset that is persistent. We adjust the height offset based on our world position height
    private void CorrectHeight() {
        // Measure the current distance of the head relative to the virtual floor
        float curDistanceToFloor = headRef.position.y;
        // If our current height is smaller than the minY, then we add to the height offset
        if (curDistanceToFloor < minY) {
            heightOffset += minY-curDistanceToFloor;
        }
        // If our current height is larger than maxY, then we remove from the height offset
        if (curDistanceToFloor > maxY) {
            heightOffset -= curDistanceToFloor-maxY;
        }
        if (heightOffsetTextbox != null) heightOffsetTextbox.text = $"Height offset: {heightOffset}";
    }

    private void ApplyHeight() {
        // Finally, we set the camera rig's y position
        cameraRig.position = (m_applyInitialized) ? new Vector3(0f, heightOffset, 0f) : Vector3.zero;
    }
}
