using UnityEngine;
using System.Collections;

public static class Layers
{
	public const int DefaultLayer = 0;


    public const int InteractableLayer = 8;
    public const int NoCollision = 9;

    public const int DefaultLayerMask = 1 << DefaultLayer;
	public const int InteractableLayerMask = 1 << Layers.InteractableLayer;
    public const int NoCollisionMask = 1 << NoCollision;
    public const int ALL_LAYERS_MASK = Layers.InteractableLayerMask | DefaultLayerMask;

}
