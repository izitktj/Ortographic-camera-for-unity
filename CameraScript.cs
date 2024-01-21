using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] Camera Cam;
    [SerializeField] GameObject Player;
	[SerializeField] float MaxCameraZoom;
    [SerializeField] float MinCameraZoom;

    [Header("Configs")]
    public bool InvertXAxis = false;
    public float CameraMovSensitivity = 1.0f;
    public float ScrollZoomSensitivity = 1.0f;

    Vector3 DiferencePlayerAndCamera;
    Vector3 CamPosition = Vector3.zero;
    int CameraState = 0; //States: 0 Stopped/Not Moving, 1 Player Dragging, 2 Returning to Player
    float t = 1.1f;

    void Start()
    {
        //Get diference between player and camera (used to maintain the original distance from the camera)
        DiferencePlayerAndCamera = Cam.transform.position - Player.transform.position;
    }

    void Update()
    {
        if(CameraState == 0) FollowPlayer(); //Follow Player

        if(CameraState ==  2) ReturnToPlayer(); //Make camera return to player

        if(Input.GetMouseButtonUp(1)) HandleMouseUp();
        if(Input.GetMouseButton(1)) HandleMouseDown();

        ScrollZoom();

        StateHandler();
    }

    ///Check if will be zoom in or zoom out and apply to Cam game object
    void ScrollZoom() {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        int scrollInOut = (scrollInput < 0f && Cam.orthographicSize < MaxCameraZoom) ? 1 
                        : (scrollInput > 0f && Cam.orthographicSize > MinCameraZoom) ? -1 : 0; //Check if is scrolling up or down

        Cam.orthographicSize += scrollInOut * ScrollZoomSensitivity;
    }

    void HandleMouseDown() {
        StateHandler(1);
        float MouseY = Input.GetAxis("Mouse Y");
        float MouseX = Input.GetAxis("Mouse X");

        if(!InvertXAxis) MouseX = -MouseX; //Invert Mouse input

        Vector3 MouseVector = new Vector3(MouseY * CameraMovSensitivity, 0.0f , MouseX * CameraMovSensitivity);
        Cam.transform.position += MouseVector;
    }

    void HandleMouseUp() {
        StateHandler(2);
        CamPosition = Cam.transform.position; //Get last camera position
        t = 0.0f; //Reset Lerp animation
    }

    ///Smooth the camera return
    void ReturnToPlayer() {
        StateHandler(2);
        Cam.transform.position = Vector3.Lerp(CamPosition, Player.transform.position + DiferencePlayerAndCamera, t);

        t += 0.9f * Time.deltaTime;
    }

    ///Cam object follows the GameObject "Player"
    void FollowPlayer() {
        Cam.transform.position = Player.transform.position + DiferencePlayerAndCamera;
    }

    ///State handler shifts the CameraState parameter, you can input (or not), and will be changed
    void StateHandler(int _state = 3) {
        CameraState = (DiferencePlayerAndCamera == Cam.transform.position - Player.transform.position && !Input.GetMouseButton(1)) ? 0 
                    : (_state == 3) ? CameraState : _state;
    }
}
