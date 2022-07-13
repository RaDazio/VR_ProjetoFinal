using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class WallAction : MonoBehaviour, ISpecificToolAction
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
                actualAction = "STRECH";
                break;
            case "STRECH":
                actualAction = "FINALIZE";
                break;

            default:
                break;
        }

        if (actualAction == "FINALIZE")
        {
            GetComponentInParent<ToolController>().EditedFinish = true;

            DivideItSelf();
        }
    }

    private void DivideItSelf()
    {
        Vector3 direction = transform.forward;
        int howMany = (int)(transform.parent.transform.localScale.z / 10);

        for (int i = 0; i < howMany; i++)
        {
            var newObject = Instantiate(transform.parent);
            newObject.transform.position = transform.parent.position + direction * i;
            newObject.transform.localScale = new Vector3(1, 1, 0.1f * 100);
        }
        Destroy(gameObject);
        Destroy(transform.parent.gameObject);
        markedForDeleted = true;
    }

    public void UpdateAttachTransform()
    {
        if (actualAction == "FINALIZE" || markedForDeleted) return;
        if (t_Controller.ControllerRef.TryGetComponent(out XRRayInteractor RayInteractor))
        {
            Transform controllerAttachTransform = RayInteractor.attachTransform;
            float attachHeight = controllerAttachTransform.position.y + transform.lossyScale.y / 2;
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
            else if (actualAction == "STRECH")
            {
                float distance = Vector3.Distance(gameObject.transform.parent.transform.position, controllerAttachTransform.position);

                //gameObject.transform.
                float adjust = 1 / gameObject.transform.localScale.z;
                Vector3 actualScale = gameObject.transform.parent.transform.localScale;
                Vector3 newScale = new Vector3(actualScale.x, actualScale.y, distance * adjust);
                gameObject.transform.parent.transform.localScale = newScale;

            }
        }
        else
        {
            gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
        }
    }

    public void UpdateCanChangeTool()
    {
        t_Controller.t_CanBeDesselected = actualAction == "PREVIEW" || actualAction =="FINALIZE";
    }

    public void ResetPreview()
    {
        //throw new NotImplementedException();
    }
}
