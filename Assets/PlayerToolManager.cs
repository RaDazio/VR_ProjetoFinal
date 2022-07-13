using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerToolManager : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] public GameObject[] ToolsPrefabs;
    public GameObject SelectedToolPrefab = null;
    public GameObject SelectedToolPreview = null;
    public GameObject SelectedToolInstance = null;


    [SerializeField] public GameObject LeftHandController;
    [SerializeField] public GameObject RightHandControoler;

    private GameObject controllerUsed = null;

    void Start() { }

    // Update is called once per frame
    void Update()
    {
        if (SelectedToolInstance != null && SelectedToolInstance.GetComponent<ToolController>().EditedFinish)
            SelectedToolInstance = null;
        if (SelectedToolPreview != null)
        {
            Transform attachTransform = SelectedToolPreview.GetComponent<ToolController>().GetAttachedTransform();
            if (attachTransform)
            {
                SelectedToolPreview.transform.position = attachTransform.position;
            }
        }

    }

    public void TrySetTool(GameObject rayCastHitted, GameObject controller)
    {
        //Debug.Log("TrySetTool
        ToolController t_Controller = TryGetToolController(rayCastHitted);

        if (t_Controller != null &&
            IsAMapedTool(t_Controller.ToolType) &&
            CanChangeTool())
        {
            ResetManager();
            SelectedToolPrefab = ToolsPrefabs.First(t => t.GetComponent<ToolController>().ToolType == t_Controller.ToolType);
  
            SetPreview(controller);
            controllerUsed = controller;
        }
    }

    public void TryPlaceNewFeature(Vector3 position)
    {
        //Debug.Log("TryPlaceNewFeature");
        if (CanObjectBePlaced())
        {
            SpawnFeature(position);
        }
        else if (SelectedToolInstance)
        {
            SelectedToolInstance.GetComponent<ToolController>().DoNextAction();
        }

        if (SelectedToolInstance && !SelectedToolInstance.GetComponent<ToolController>().EditedFinish)
        {
            ClearPreview();
            //Debug.Log("CLEAR PREVIEW");
        }
        if (SelectedToolInstance && SelectedToolInstance.GetComponent<ToolController>().EditedFinish)
        {
            SetPreview(controllerUsed);
            //Debug.Log("RESETING PREVIEW"); 
        }


        return;
    }

    public void ResetManager()
    {
        if (SelectedToolInstance)
        {
            if (!SelectedToolInstance.GetComponent<ToolController>().EditedFinish)
                Destroy(SelectedToolInstance);

            ClearPreview();

            SelectedToolPrefab = SelectedToolPreview = SelectedToolInstance = null;
        }
    }

    public void ClearPreview()
    {
        if (SelectedToolPreview)
        {
            Destroy(SelectedToolPreview);
            SelectedToolPreview = null;
        }

    }

    private ToolController TryGetToolController(GameObject target)
    {
        ToolController t_Controller = target.GetComponentInParent(typeof(ToolController), false) as ToolController;
        return t_Controller;
    }

    private void SpawnFeature(Vector3 position)
    {
        SelectedToolInstance = Instantiate(SelectedToolPrefab, SelectedToolPreview.transform.position, SelectedToolPreview.transform.rotation);
        //SelectedToolInstance.transform.position = SelectedToolPreview.transform.position;
        //SelectedToolInstance.transform.rotation = SelectedToolPreview.transform.rotation;
        ToolController t_controller = SelectedToolInstance.GetComponent<ToolController>();

        t_controller.isInstance = true;
        t_controller.GetComponentInChildren<XRGrabInteractable>().enabled = false;
        t_controller.SetController(SelectedToolPreview.GetComponent<ToolController>().ControllerRef);
        t_controller.DoNextAction();

    }

    private void SetPreview(GameObject controller = null)
    {
      
        if (SelectedToolPrefab != null && CanChangePreview())
        {
            if (SelectedToolPreview)
            {
                var previewControler = SelectedToolPreview.GetComponent<ToolController>();
                previewControler.ResetPreview();
                Destroy(SelectedToolPreview);

            }
            SelectedToolPreview = Instantiate(SelectedToolPrefab, Vector3.zero, Quaternion.identity);
            SelectedToolPreview.name = SelectedToolPrefab.name + " - preview";
            ToolController t_Controller = SelectedToolPreview.GetComponent<ToolController>();
            t_Controller.SetPreviewTool();
            //SelectedToolPreview.GetComponent<XRGrabInteractable>().enabled = false;
            if (controller != null) SelectedToolPreview.GetComponent<ToolController>().SetController(controller);
        }

    }

    private bool CanChangePreview()
    {
        string type1 = "";
        string type2 = "";
        if (SelectedToolPreview != null && SelectedToolPreview.TryGetComponent(out ToolController controller1))
        {
            type1 = controller1.ToolType;
        }

        if (SelectedToolInstance != null && SelectedToolInstance.TryGetComponent(out ToolController controller2))
        {
            type2 = controller2.ToolType;
        }
        return SelectedToolPreview == null || (type1!=type2);
    }

    private bool CanObjectBePlaced()
    {
        if(SelectedToolPrefab)
        {
            if (SelectedToolInstance == null) return true;
            return SelectedToolInstance.GetComponent<ToolController>().EditedFinish;
        }
        return false;
    }

    private bool CanChangeTool()
    {
        return SelectedToolPrefab == null || (SelectedToolPreview != null &&
                SelectedToolPreview.GetComponent<ToolController>().CanBeDesellected());
    }

    private bool IsAMapedTool(string toolName)
    {
        return ToolsPrefabs.Select(t => t.GetComponent<ToolController>().ToolType).Contains(toolName);
    }




}



