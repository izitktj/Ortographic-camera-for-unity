using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [Header("Setup")]
    public Camera Cam;
    public GameObject Player;
	public float MaxCameraZoom;
    public float MinCameraZoom;

    [Header("Configs")]
    public bool InvertXAxis = false;
    public float CameraMovSensitivity = 1.0f;
    public float ScrollZoomSensitivity = 1.0f;

    Vector3 DiferencePlayerAndCamera;
    Vector3 CamPosition = new Vector3(0.0f, 0.0f, 0.0f);
    bool bDragCamera = false;
    float t = 1.1f;

    void Start()
    {
        //Get diference between player and camera (used to maintain the original distance from the camera)
        DiferencePlayerAndCamera = new Vector3(Cam.transform.position.x - Player.transform.position.x, Cam.transform.position.y - Player.transform.position.y, Cam.transform.position.z - Player.transform.position.z);
    }

    void Update()
    {
        FollowPlayer();
        DragCamera();
        ScrollZoom();
    }

    void ScrollZoom() {
       //Scroll zoom
        if (Input.GetAxis("Mouse ScrollWheel") < 0f && Cam.orthographicSize < MaxCameraZoom)
        {
           Cam.orthographicSize = Cam.orthographicSize + 1 * ScrollZoomSensitivity;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0f && Cam.orthographicSize > MinCameraZoom)
        {
           Cam.orthographicSize = Cam.orthographicSize - 1 * ScrollZoomSensitivity;
        }
    }

    void DragCamera() {
        //Move camera
        if (Input.GetMouseButton(1))
        {
            bDragCamera = true;
            float MouseY = Input.GetAxis("Mouse Y");
            float MouseX = Input.GetAxis("Mouse X");

            if(!InvertXAxis) MouseX = MouseX - MouseX * 2;

            Vector3 MouseVector = new Vector3(MouseY * CameraMovSensitivity, 0.0f , MouseX * CameraMovSensitivity);
            Cam.transform.position = Cam.transform.position + MouseVector;
        }

        //Return Camera

        if (Input.GetMouseButtonUp(1))
        {
            bDragCamera = false;
            CamPosition = Cam.transform.position; //Get last camera position
            t = 0.0f; //Reset Lerp animation
        }

        //Make camera return to player
        if(bDragCamera == false && t < 1.1f)
        {
            Cam.transform.position = new Vector3(Mathf.Lerp(CamPosition.x, Player.transform.position.x + DiferencePlayerAndCamera.x, t), Mathf.Lerp(CamPosition.y, Player.transform.position.y + DiferencePlayerAndCamera.y, t), Mathf.Lerp(CamPosition.z, Player.transform.position.z + DiferencePlayerAndCamera.z, t));

            t += 0.9f * Time.deltaTime;
        }

    }

    void FollowPlayer() {
        //Check if is player dragging the camera, else, center to the player
        if(bDragCamera == false && t >= 1.1f) {
            Cam.transform.position = new Vector3(Player.transform.position.x + DiferencePlayerAndCamera.x, Player.transform.position.y + DiferencePlayerAndCamera.y, Player.transform.position.z + DiferencePlayerAndCamera.z);
        }
    }
}
