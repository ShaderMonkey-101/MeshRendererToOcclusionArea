using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(OcclusionArea))]
public class HelenaMeshRendererToOcclusionArea : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private OcclusionArea occlusionArea;

    /// <summary>
    /// Initializes components and copies values from MeshRenderer to OcclusionArea if both components are present
    /// and the GameObject is active in the hierarchy.
    /// </summary>
    private void Awake()
    {
        // Only proceed if the GameObject is active in the hierarchy
        if (!gameObject.activeInHierarchy)
        {
            Debug.Log($"[{gameObject.name}] - GameObject is inactive. Skipping processing.");
            return;
        }

        // Initialize components
        InitializeComponents();

        // If both components exist, proceed to copy values
        if (meshRenderer != null && occlusionArea != null)
        {
            CopyValues();
        }
        else
        {
            // Warn if components are missing
            Debug.LogWarning($"[{gameObject.name}] - Missing required components (MeshRenderer or OcclusionArea). Values were not copied.", this);
        }
    }

    /// <summary>
    /// Initializes the MeshRenderer and OcclusionArea components, logging errors if they are missing.
    /// </summary>
    private void InitializeComponents()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            Debug.LogError($"[Error] - MeshRenderer is missing on '{gameObject.name}'. Please add it to ensure proper functionality.", this);
        }

        occlusionArea = GetComponent<OcclusionArea>();
        if (occlusionArea == null)
        {
            Debug.LogError($"[Error] - OcclusionArea is missing on '{gameObject.name}'. Please add it to proceed.", this);
        }
    }

    /// <summary>
    /// Copies the bounds from the MeshRenderer to the OcclusionArea, adjusting both size and center.
    /// </summary>
    private void CopyValues()
    {
        // Ensure both components are available before proceeding
        if (meshRenderer == null || occlusionArea == null)
        {
            Debug.LogError($"[{gameObject.name}] - Cannot copy values: One or both required components are missing.", this);
            return;
        }

        // Fetch the bounds of the MeshRenderer in world space
        Bounds meshBounds = meshRenderer.bounds;

        // Set the OcclusionArea size to match the MeshRenderer's bounds
        occlusionArea.size = meshBounds.size;

        // Convert MeshRenderer's center from world space to local space for OcclusionArea
        occlusionArea.center = transform.InverseTransformPoint(meshBounds.center);

        // Log success message
        Debug.Log($"[{gameObject.name}] - Successfully copied MeshRenderer bounds to OcclusionArea.", this);
    }

    /// <summary>
    /// Allows manual copying of MeshRenderer bounds to OcclusionArea via the Unity Inspector context menu.
    /// </summary>
    [ContextMenu("Copy MeshRenderer to OcclusionArea")]
    public void CopyValuesManually()
    {
        // Reinitialize components to ensure they are up to date
        InitializeComponents();

        // Proceed with copying values if components are valid
        CopyValues();
    }

    /// <summary>
    /// Batch operation to copy MeshRenderer bounds to OcclusionArea for all objects in the scene that have both components
    /// and are active in the hierarchy.
    /// </summary>
    [MenuItem("HelenaTools/Copy MeshRenderer to OcclusionArea")]
    public static void CopyAllMeshRenderersToOcclusionAreas()
    {
        // Get all GameObjects in the scene
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        int updatedCount = 0;

        // Iterate over each GameObject
        foreach (GameObject obj in allObjects)
        {
            // Skip inactive GameObjects
            if (!obj.activeSelf)
            {
                continue;
            }

            // Ensure the GameObject contains both MeshRenderer and OcclusionArea components
            MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
            OcclusionArea occlusionArea = obj.GetComponent<OcclusionArea>();

            // Proceed only if both components exist
            if (meshRenderer != null && occlusionArea != null)
            {
                // Copy the size and center values from MeshRenderer to OcclusionArea
                Bounds meshBounds = meshRenderer.bounds;
                occlusionArea.size = meshBounds.size;
                occlusionArea.center = obj.transform.InverseTransformPoint(meshBounds.center);

                updatedCount++;
                Debug.Log($"[{obj.name}] - OcclusionArea successfully updated based on MeshRenderer bounds.");
            }
        }

        // Log the total number of updated GameObjects
        Debug.Log($"Completed copying OcclusionArea for {updatedCount} GameObjects in the scene.");
    }
}
