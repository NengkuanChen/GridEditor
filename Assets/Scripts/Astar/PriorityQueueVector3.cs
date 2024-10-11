using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;

public class PriorityQueueVector3 : FastPriorityQueueNode
{
    public Vector3 Vector { get; private set; }
    public PriorityQueueVector3(Vector3 vector3)
    {
        Vector = vector3;
    }
}
