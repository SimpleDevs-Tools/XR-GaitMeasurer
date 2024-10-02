using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GaitOscillateMeasurer : MonoBehaviour
{
    public Transform cameraRig = null;
    public Transform headRef = null;
    public float minY = 0f;
    public float maxY = 0f;
    public float heightOffset = 0f;

    public TextMeshProUGUI minTextbox;
    public TextMeshProUGUI maxTextbox;

    private bool m_measureInitialized = false;
    private bool m_correctInitialized = false;

    public void ToggleMeasurer() {
        if (!m_measureInitialized) StartMeasurer();
        else EndMeasurer();
    }
    public void StartMeasurer() {
        minY = headRef.localPosition.y;
        maxY = headRef.localPosition.y;
        m_measureInitialized = true;
    }
    public void EndMeasurer() {
        if (minTextbox != null) minTextbox.text = $"Min:";
        if (maxTextbox != null) maxTextbox.text = $"Max:";
        m_measureInitialized = false;
    }

    private void Update() {
        if (headRef == null || cameraRig == null) return;
        if (m_measureInitialized) MeasureOscillation();
        if (m_correctInitialized) CorrectHeight();
    }

    private void MeasureOscillation() {
        float yPos = headRef.localPosition.y;
        if (yPos < minY) minY = yPos;
        if (yPos > maxY) maxY = yPos;

        if (minTextbox != null) minTextbox.text = $"Min: {minY.ToString()}";
        if (maxTextbox != null) maxTextbox.text = $"Max: {maxY.ToString()}";
    }

    public void ToggleCorrector() {
        if (!m_correctInitialized) StartCorrector();
        else EndCorrector();
    }

    public void StartCorrector() {
        heightOffset = 0f;
        m_correctInitialized = true;
    }

    public void EndCorrector() {
        cameraRig.position = Vector3.zero;
        m_correctInitialized = false;
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
        // Finally, we set the camera rig's y position
        cameraRig.position = new Vector3(0f, heightOffset, 0f);
        
    }
}
