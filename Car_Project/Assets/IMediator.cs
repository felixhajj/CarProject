using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IMediator
{
    void Notify(object sender, string eventType, object data = null);
}
