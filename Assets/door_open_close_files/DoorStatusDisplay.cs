using UnityEngine;

public class DoorStatusDisplay : MonoBehaviour
{
    public Material statusMaterial; // Assign in the inspector
    public Texture openTexture; // Assign in the inspector
    public Texture closedTexture; // Assign in the inspector
    public DoorOpenClose door; // Reference to your door script or component that knows the door's state
    public Color emissionColor = Color.white; // Assign the default emission color in the inspector
    private void Update()
    {
        // Update the emission texture and color based on the door state
        if (door.isDoorOpen)
        {
            // Set the emission texture to the openTexture
            statusMaterial.SetTexture("_EmissionMap", openTexture);
            // Set the emission color
            statusMaterial.SetColor("_EmissionColor", emissionColor);
            // Enable emission keyword (this is necessary for some shaders)
            statusMaterial.EnableKeyword("_EMISSION");
        }
        else
        {
            // Set the emission texture to the closedTexture
            statusMaterial.SetTexture("_EmissionMap", closedTexture);
            // Set the emission color
            statusMaterial.SetColor("_EmissionColor", emissionColor);
            // Enable emission keyword (this is necessary for some shaders)
            statusMaterial.EnableKeyword("_EMISSION");
        }

        // Tell the renderer that the material is updated during runtime
        statusMaterial.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
    }
}
