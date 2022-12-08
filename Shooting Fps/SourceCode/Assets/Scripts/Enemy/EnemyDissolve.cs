using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDissolve : MonoBehaviour
{
    public MeshRenderer mesh;

    public float dissolveRate = 0.0125f;
    public float refreshRate = 0.025f;

    private Material[] meshMats;

    void Start()
    {
        if(mesh != null)
        {
            StartCoroutine(DissolveCo());
        }
    }

    IEnumerator DissolveCo()
    {
        if(meshMats.Length > 0)
        {
            float counter = 0;
            while (meshMats[0].GetFloat("_DissolveAmount") < 1)
            {
                counter += dissolveRate;
                for(int i = 0; i < meshMats.Length; i++)
                {
                    meshMats[i].SetFloat("_DissolveAmount", counter);
                }

                yield return new WaitForSeconds(refreshRate);
            }
        }
    }
}
