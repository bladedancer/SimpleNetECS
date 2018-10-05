using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class CameraController : MonoBehaviour {
    public float ScrollMargin = 0.1f;
    public float ScrollSpeed = 5;
    public float ZoomSpeed = 10;
    public float RecenterSpeed = 20f;

    public MeshRenderer Ground;
    private Camera cam;

    enum VIEWPORT_POINT
    {
        BL = 0,
        CL,
        TL,
        TC,
        TR,
        CR,
        BR,
        BC
    };

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }
    
    private void Update()
    {
        moveCamera();
        zoomCamera();
    }

    /// <summary>
    /// Checks the corners of the view port to see if they're over ground
    /// </summary>
    /// <returns>
    /// An array of booleans indicating if the corner is over ground:
    /// bottom left, center left, top left, top center, top right,
    /// center right, bottom right, bottom center.
    /// </returns>
    bool[] getViewportGrounded()
    {
        // Raycast to ensure we don't go over edge
        Ray[] rays = new Ray[]
        {
                Camera.main.ViewportPointToRay(new Vector3(0, 0, 0)), // Bottom Left
                Camera.main.ViewportPointToRay(new Vector3(0, 0.5f, 0)), // Center Left
                Camera.main.ViewportPointToRay(new Vector3(0, 1, 0)), // Top Left
                Camera.main.ViewportPointToRay(new Vector3(0.5f, 1, 0)), // Top Center
                Camera.main.ViewportPointToRay(new Vector3(1, 1, 0)), // Top Right
                Camera.main.ViewportPointToRay(new Vector3(1, 0.5f, 0)), // Center Right
                Camera.main.ViewportPointToRay(new Vector3(1, 0, 0)), // Bottom Right
                Camera.main.ViewportPointToRay(new Vector3(0.5f, 0, 0)) // Bottom Center
        };
        bool[] grounded = new bool[rays.Length];

        for (int i = 0; i < rays.Length; ++i)
        {
            RaycastHit[] hits = Physics.RaycastAll(rays[i]);
            if (hits.Length > 0)
            {
                grounded[i] = hits.Where(h => h.collider.tag == "Ground").Count() > 0;
            }
            else
            {
                grounded[i] = false;
            }
        }
        return grounded;
    }

    void moveCamera() {
        float3 dir = Vector3.zero;
        if (Input.mousePosition.y >= (Screen.height * (1 - ScrollMargin)) && Input.mousePosition.y <= Screen.height)
        {
            dir.z = 1;
        }
        else if (Input.mousePosition.y <= (Screen.height * ScrollMargin) && Input.mousePosition.y >= 0)
        {
            dir.z = -1;
        }

        if (Input.mousePosition.x >= (Screen.width * (1 - ScrollMargin)) && Input.mousePosition.x <= Screen.width)
        {
            dir.x = 1;
        }
        else if (Input.mousePosition.x <= (Screen.width * ScrollMargin) && Input.mousePosition.x >= 0)
        {
            dir.x = -1;
        }

        if (dir.x != 0 || dir.z != 0)
        {
            bool[] grounded = getViewportGrounded();
            if (!grounded[(int) VIEWPORT_POINT.BL] && !grounded[(int)VIEWPORT_POINT.CL] && !grounded[(int)VIEWPORT_POINT.TL] && dir.x == -1)
            {
                // No left
                dir.x = 0;
            }
            if (!grounded[(int)VIEWPORT_POINT.TL] && !grounded[(int)VIEWPORT_POINT.TC] && !grounded[(int)VIEWPORT_POINT.TR] && dir.z == 1)
            {
                // No up
                dir.z = 0;
            }
            if (!grounded[(int)VIEWPORT_POINT.TR] && !grounded[(int)VIEWPORT_POINT.CR] && !grounded[(int)VIEWPORT_POINT.BR] && dir.x == 1)
            {
                // No right
                dir.x = 0;
            }
            if (!grounded[(int)VIEWPORT_POINT.BL] && !grounded[(int)VIEWPORT_POINT.BC] && !grounded[(int)VIEWPORT_POINT.BR] && dir.z == -1)
            {
                // No down
                dir.z = 0;
            }
        }

        cam.transform.Translate(dir * Time.deltaTime * ScrollSpeed, Space.World);
    }

    private void zoomCamera()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll == 0)
        {
            return;
        }

        
        if (scroll > 0)
        {
            // Limit Zoom out and track back to center
            bool[] grounded = getViewportGrounded();
            Vector3 center = new Vector3(
                Ground.transform.position.x,
                10,
                Ground.transform.position.y
            );

            if (!grounded[(int)VIEWPORT_POINT.CL] && !grounded[(int)VIEWPORT_POINT.TC]
                && !grounded[(int)VIEWPORT_POINT.CR] && !grounded[(int)VIEWPORT_POINT.BC])
            {
                // Can't zoom out more
                cam.transform.position = center;
                return;
            }

            if (!grounded[(int)VIEWPORT_POINT.BL] || !grounded[(int)VIEWPORT_POINT.TL]
                || !grounded[(int)VIEWPORT_POINT.TR] || !grounded[(int)VIEWPORT_POINT.BR])
            {
                cam.transform.position = Vector3.Lerp(cam.transform.position, center, Time.deltaTime * RecenterSpeed * scroll);
            }
        }

        float targetSize = Mathf.Max(10, cam.orthographicSize + (ScrollSpeed * scroll));
        cam.orthographicSize = targetSize;
    }
}
