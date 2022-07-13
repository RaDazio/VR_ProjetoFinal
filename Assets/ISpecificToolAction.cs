using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpecificToolAction
{
    public void DoNextAction();
    public void OnEnable();
    public void OnDisable();
    public void UpdateAttachTransform();
    public void UpdateCanChangeTool();
    public void ResetPreview();
}