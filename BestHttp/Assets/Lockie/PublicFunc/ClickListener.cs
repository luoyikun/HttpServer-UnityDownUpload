using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ClickListener : MonoBehaviour, IPointerClickHandler
{
    public delegate void VoidDelegate(GameObject go);
    public VoidDelegate onClick;
    public object parameter;
    public object parameter1;

    static public ClickListener Get(GameObject go)
    {
        ClickListener listener = go.GetComponent<ClickListener>();
        if (listener == null) listener = go.AddComponent<ClickListener>();
        return listener;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (onClick != null)
        {
            onClick(gameObject);
        }
    }

}