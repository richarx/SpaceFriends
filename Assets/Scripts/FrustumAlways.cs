using UnityEngine;

[ExecuteInEditMode]
public class FrustumAlways : MonoBehaviour 
{
    public Camera cameraShowFrustumAlways;
    private void OnDrawGizmos()
    {
        if (cameraShowFrustumAlways)
        {
            Gizmos.matrix = cameraShowFrustumAlways.transform.localToWorldMatrix;
            Gizmos.DrawFrustum(cameraShowFrustumAlways.transform.position, 
                cameraShowFrustumAlways.fieldOfView, 
                cameraShowFrustumAlways.farClipPlane, 
                cameraShowFrustumAlways.nearClipPlane, 
                cameraShowFrustumAlways.aspect);
        }
    }
}