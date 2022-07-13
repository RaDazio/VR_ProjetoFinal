using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class ToolController : MonoBehaviour
{
    [SerializeField] public string ToolType;

    public delegate void SpecificToolCaller();
    public event SpecificToolCaller EventCallToolAction;

    public delegate void UpdateAttachTransform();
    public event UpdateAttachTransform EventUpdateAttachTransform;
    public Transform attachTransform = null;

    public delegate void DCanBeDesellected();
    public event DCanBeDesellected EventUpdateDesellectedPossibility;
    public bool t_CanBeDesselected = false;

    public delegate void EResetPreview();
    public event EResetPreview EventResetPreview;

    //public bool IsActive = false
    public bool EditedFinish = false;

    public GameObject ControllerRef { get; private set; }

    public bool isInstance = false;

 
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //if (isInstance)
        //    Debug.Log(ControllerRef);
    }

    public bool CanBeDesellected()
    {
        EventUpdateDesellectedPossibility?.Invoke();
        return t_CanBeDesselected;
    }

    public void DoNextAction()
    {
        EventCallToolAction?.Invoke();
    }

    public Transform GetAttachedTransform()
    {
        EventUpdateAttachTransform?.Invoke();

        return attachTransform;
    }

    public void SetController(GameObject controller)
    {
        ControllerRef = controller;
    }

    public void SetPreviewTool()
    {
        var xr = GetComponentInChildren<XRGrabInteractable>();
        var bc = GetComponentInChildren<BoxCollider>();
        if (xr) GetComponentInChildren<XRGrabInteractable>().enabled = false;
        if (bc) GetComponentInChildren<BoxCollider>().enabled = false;
    }

    public void ResetPreview()
    {
        EventResetPreview?.Invoke();
    }
}
