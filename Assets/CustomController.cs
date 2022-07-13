using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class CustomController : MonoBehaviour
{
    // Start is called before the first frame update
    public InputActionReference selectToolReference = null;
    public InputActionReference placeNewFeatureReference = null;

    private GameObject player = null;
    private XRRayInteractor RayInteractor = null;
    private PlayerToolManager toolManager = null;

    void Start()
    {
        RayInteractor = GetComponent<XRRayInteractor>();
        player = GameObject.FindGameObjectWithTag("Player");
        toolManager = player.GetComponent<PlayerToolManager>();
        if (selectToolReference)
            selectToolReference.action.started += OnSelectToolAction;
        if (placeNewFeatureReference)
            placeNewFeatureReference.action.started += OnPlaceNewFeature;
    }
    private void OnDestroy()
    {
        if (selectToolReference)
            selectToolReference.action.started -= OnSelectToolAction;
        if (placeNewFeatureReference)
            placeNewFeatureReference.action.started -= OnPlaceNewFeature;
    }

    // Update is called once per frame
    void Update()
    {
     
    }


    private void OnSelectToolAction(InputAction.CallbackContext context)
    {
        if (RayInteractor.TryGetCurrent3DRaycastHit(out var raycastHit))
        {
            toolManager.TrySetTool(raycastHit.transform.gameObject, gameObject);
        }
    }

    private void OnPlaceNewFeature(InputAction.CallbackContext context)
    {
        toolManager.TryPlaceNewFeature(RayInteractor.attachTransform.position);
    }
}
