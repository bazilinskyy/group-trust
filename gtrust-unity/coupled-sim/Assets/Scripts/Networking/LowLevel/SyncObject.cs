using System;
using UnityEngine;


public class SyncObject : MonoBehaviour
{
    public int NetInstanceId;
    public Action<SyncObject, SyncObject> OnCollision;
}