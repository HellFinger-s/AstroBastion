using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigitPlace : MonoBehaviour
{
    public List<GameObject> digits;
    public List<MeshRenderer> digitMeshRenderers;
    public int digit;


    public void SetMaterial(Material mat)
    {
        digitMeshRenderers[digit].material = mat;
    }
}
