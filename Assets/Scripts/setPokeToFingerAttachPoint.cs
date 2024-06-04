using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class setPokeToFingerAttachPoint : MonoBehaviour
{
    public Transform pokeAttachTransform;
    XRPokeInteractor pokeInteractor;

    private void Start()
    {
        pokeInteractor = transform.parent.parent.GetComponentInChildren<XRPokeInteractor>();
        SetPokeInteractorTransform();
    }

    void SetPokeInteractorTransform()
    {
        if (pokeAttachTransform != null && pokeInteractor != null)
        {
            pokeInteractor.attachTransform = pokeAttachTransform;
        }
    }
}
