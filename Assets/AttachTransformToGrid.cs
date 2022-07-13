
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class AttachTransformToGrid : MonoBehaviour
{
    // Start is called before the first frame     [SerializeField] GameObject parentObject;
    [SerializeField] GameObject parentObject;
    [SerializeField] GameObject gridFloor;
    XRRayInteractor interactor = null;

    Plane planeRef;
    void Start()
    {
        interactor = parentObject.GetComponent<XRRayInteractor>();
    }


    // Update is called once per frame
    void Update()
    {
        if (gameObject == null) return;
        
        Ray ray = new Ray();
        ray.direction = interactor.rayOriginTransform.forward;
        ray.origin = interactor.rayOriginTransform.position;
        Vector3 newPosition = gridFloor.GetComponent<GridBuilder>().GetClosestPositionToGrid(ray);
        if(newPosition!= Vector3.zero)
        {
            gameObject.transform.position = newPosition;
        }

    }
}
