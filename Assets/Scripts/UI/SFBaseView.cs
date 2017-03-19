using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SF;

public class SFBaseView : MonoBehaviour
{
    public void addEventListener(Component widget, string eventType, SFListenerSelector sel)
    {
        var dispatcher = SFUIEventListener.getDispatcherWithGo(widget.gameObject);
        dispatcher.addEventListener(eventType, sel);
    }
}
