using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderIgnoreHandler : MonoBehaviour
{
    [SerializeField]
    public List<Collider> colliders;
    void Start()
    {
        foreach (Collider c in colliders)
        {
            Physics.IgnoreCollision(c, gameObject.GetComponent<SphereCollider>());
            Physics.IgnoreCollision(c, gameObject.GetComponent<MeshCollider>());
        }
    }
}
