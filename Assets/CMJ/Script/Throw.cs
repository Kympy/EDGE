using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Throw : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Rigidbody rb;

    float startTime, endTime;
    Vector3 startPosition, endPosition;
    float y;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        y = transform.position.y;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        throw new System.NotImplementedException();

        transform.position = GetPosition(eventData);
    }



    public void OnBeginDrag(PointerEventData eventData)
    {
        throw new System.NotImplementedException();

        startTime = Time.time;
        startPosition = GetPosition(eventData);

        rb.velocity = Vector3.zero;
    }

    

    public void OnEndDrag(PointerEventData eventData)
    {
        throw new System.NotImplementedException();

        endTime = Time.time;
        endPosition = GetPosition(eventData);

        Vector3 dir = (endPosition - startPosition).normalized;
        float dis = (endPosition - startPosition).magnitude;
        float speed = dis / (endTime - startTime);

        rb.AddForce(dir * speed * 5f);
    }

    Vector3 GetPosition(PointerEventData eventData)
    {
        RaycastHit[] hits;
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);

        hits = Physics.RaycastAll(ray, 1000f);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.name != transform.name)
            {
                return new Vector3(hit.point.x, y, hit.point.z);
            }
        }

        return transform.position;
    }
}
