using Components;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class CameraTrackingController : MonoBehaviour {
    public float TrackingZoom = 10;

    private Entity targetEntity = Entity.Null;
    private Transform targetTransform = null;
    private Camera trackingCamera;
    private bool isTracking = false;
    private bool startTracking = false;
    private EntityManager em;

    private void Awake()
    {
        trackingCamera = GetComponent<Camera>();
    }

    private void Start()
    {
        em = World.Active.GetOrCreateManager<EntityManager>();
    }

    void Update() {
        startTracking = false;
        if (Input.GetKeyDown(KeyCode.T))
        {
            isTracking = !isTracking;
            if (isTracking)
            {
                startTracking = true;

                // This could probably be a single linq but need more mojo
                targetEntity = em.GetAllEntities().Where((e) => em.HasComponent<PlayerInput>(e)).FirstOrDefault();
                if (!em.Exists(targetEntity))
                {
                    targetEntity = em.GetAllEntities().Where((e) => em.HasComponent<CurrentFittest>(e)).FirstOrDefault();
                }
            } else
            {
                targetEntity = Entity.Null;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            targetEntity = Entity.Null;
            targetTransform = null;
            isTracking = false;
        }

        if (isTracking)
        {
            if (targetEntity != Entity.Null && em.Exists(targetEntity))
            {
                targetTransform = em.GetComponentObject<Transform>(targetEntity);
            }
            else
            {
                targetEntity = Entity.Null;
                targetTransform = null;
            }
        }
    }

    private void LateUpdate()
    {
        if (isTracking && targetTransform != null)
        {
            Vector3 position = new Vector3(
                targetTransform.position.x,
                trackingCamera.transform.position.y,
                targetTransform.position.z
            );

            transform.position = position;

            if (startTracking)
            {
                trackingCamera.orthographicSize = TrackingZoom;
            }
        }
    }
}
