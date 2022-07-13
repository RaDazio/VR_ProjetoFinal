using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FurnitureTool_1 : MonoBehaviour, ISpecificToolAction
{
    public string actualAction = "PREVIEW";
    private ToolController t_Controller;
    //float lastUpdate = 0;
    private bool markedForDeleted = false;

    public void DoNextAction()
    {
        switch (actualAction)
        {
            case "PREVIEW":
                actualAction = "ROTATION";
                break;
            case "ROTATION":
                actualAction = "FINALIZE";
                break;
            default:
                break;
        }

        if (actualAction == "FINALIZE")
        {
            GetComponentInParent<ToolController>().EditedFinish = true;
        }
    }


    public void UpdateAttachTransform()
    {
        if (actualAction == "FINALIZE" || markedForDeleted) return;
        if (t_Controller.ControllerRef.TryGetComponent(out XRRayInteractor RayInteractor))
        {
            Transform controllerAttachTransform = RayInteractor.attachTransform;
            float attachHeight = 0;
            if (!float.IsInfinity(controllerAttachTransform.position.y))
                controllerAttachTransform.position += new Vector3(0, attachHeight, 0);
            //Transform newAttachTransform 
            t_Controller.attachTransform = controllerAttachTransform;
        }
    }

    public void OnEnable()
    {
        t_Controller = GetComponentInParent<ToolController>();

        t_Controller.EventCallToolAction += DoNextAction;
        t_Controller.EventUpdateAttachTransform += UpdateAttachTransform;
        t_Controller.EventUpdateDesellectedPossibility += UpdateCanChangeTool;

    }

    public void OnDisable()
    {
        t_Controller.EventCallToolAction -= DoNextAction;
        t_Controller.EventUpdateAttachTransform -= UpdateAttachTransform;
        t_Controller.EventUpdateDesellectedPossibility -= UpdateCanChangeTool;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnDestroy()
    {

    }


    private Transform lastTransform;
    // Update is called once per frame
    void Update()
    {
        //Debug.Log(t_Controller);
        if (t_Controller.isInstance)
        {
            if (actualAction == "FINALIZE" || markedForDeleted)
            {
                return;
            }
            Transform controllerAttachTransform = t_Controller.ControllerRef.GetComponent<XRRayInteractor>().attachTransform;
            if (gameObject.transform.position == controllerAttachTransform.position) return;

            Vector3 diffPosition = controllerAttachTransform.position - gameObject.transform.position;
            if (actualAction == "ROTATION")
            {
                Quaternion newRotation = Quaternion.FromToRotation(Vector3.forward, diffPosition);
                Vector3 Euler = newRotation.eulerAngles;
                Euler.x = 0;
                Euler.z = 0;
                gameObject.transform.parent.transform.rotation = Quaternion.Euler(Euler);
            }
        }
        else
        {
            gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
        }
    }

    public void UpdateCanChangeTool()
    {
        t_Controller.t_CanBeDesselected = actualAction == "PREVIEW" || actualAction == "FINALIZE";
    }

    public void GetPreviewData()
    {
        //throw new NotImplementedException();
    }

    public void ResetPreview()
    {
        //throw new System.NotImplementedException();
    }
}
