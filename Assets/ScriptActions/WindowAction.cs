using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class WindowAction : MonoBehaviour, ISpecificToolAction
{
    private ToolController t_Controller;
    private string actualAction = "SPAWNED";

    private GameObject lastWall = null;

    public void DoNextAction()
    {
        switch (actualAction)
        {
            case "SPAWNED":
                actualAction = "FINALIZE";
                break;
            default:
                break;
        }

        if (actualAction == "FINALIZE")
        {
            GetComponentInParent<ToolController>().EditedFinish = true;
            DestroyPossibleWall();
        }
    }

    private void DestroyPossibleWall()
    {
        if (t_Controller.ControllerRef.TryGetComponent(out XRRayInteractor RayInteractor))
        {
            if (RayInteractor.TryGetCurrent3DRaycastHit(out var raycastHit))
            {
                ToolController otherObjectController = raycastHit.transform.gameObject.GetComponentInParent<ToolController>();

                if (otherObjectController && otherObjectController.ToolType == "Wall")
                {
                    Destroy(raycastHit.transform.gameObject);
                }
            }
        }
    }

    public void OnEnable()
    {
        t_Controller = GetComponentInParent<ToolController>();

        t_Controller.EventCallToolAction += DoNextAction;
        t_Controller.EventUpdateAttachTransform += UpdateAttachTransform;
        t_Controller.EventUpdateDesellectedPossibility += UpdateCanChangeTool;
        t_Controller.EventResetPreview += ResetPreview;
    }

    public void OnDisable()
    {
        t_Controller.EventCallToolAction -= DoNextAction;
        t_Controller.EventUpdateAttachTransform -= UpdateAttachTransform;
        t_Controller.EventUpdateDesellectedPossibility -= UpdateCanChangeTool;
    }
    public void UpdateAttachTransform()
    {
        //Debug.Log(lastWall);
        if (actualAction == "FINALIZE") return;
        if (t_Controller.ControllerRef.TryGetComponent(out XRRayInteractor RayInteractor))
        {
            //Debug.Log(t_Controller);
            Transform controllerAttachTransform = RayInteractor.attachTransform;
            float attachHeight = controllerAttachTransform.position.y + transform.lossyScale.y / 2;
            if (!float.IsInfinity(controllerAttachTransform.position.y))
                controllerAttachTransform.position += new Vector3(0, attachHeight, 0);
            //Transform newAttachTransform 
            t_Controller.attachTransform = controllerAttachTransform;


            if (RayInteractor.TryGetCurrent3DRaycastHit(out var raycastHit))
            {
                ToolController otherObjectController = raycastHit.transform.gameObject.GetComponentInParent<ToolController>();
                if (otherObjectController != null)
                {
                    if (otherObjectController.ToolType == "Wall")
                    {
                        if (lastWall == null || lastWall != otherObjectController)
                        {
                            if(lastWall) lastWall.GetComponent<Renderer>().enabled = true;
                            lastWall = raycastHit.transform.gameObject;
                        }
                        lastWall.GetComponent<Renderer>().enabled = false;
                        Vector3 newRotation = new Vector3(raycastHit.transform.eulerAngles.x, raycastHit.transform.eulerAngles.y - 90, raycastHit.transform.eulerAngles.z);
                        gameObject.transform.parent.transform.rotation = Quaternion.Euler(newRotation);
                        t_Controller.attachTransform.position = lastWall.transform.parent.position;
                    }
                    else
                    {
                        if (lastWall) lastWall.GetComponent<Renderer>().enabled = true;
                    }
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    void OnDestroy()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateCanChangeTool()
    {
        t_Controller.t_CanBeDesselected = true;
    }

    public void ResetPreview()
    {
        //throw new System.NotImplementedException();
        if (lastWall) lastWall.GetComponent<Renderer>().enabled = true;
    }
}
