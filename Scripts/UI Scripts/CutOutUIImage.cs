using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;


public class CutOutUIImage : Image
{
    public override Material materialForRendering
    {
        get {
            Material matCopy = new Material(base.materialForRendering);
            matCopy.SetFloat("_StencilComp", (float)CompareFunction.NotEqual);
            return matCopy;
        }
    } 


}
