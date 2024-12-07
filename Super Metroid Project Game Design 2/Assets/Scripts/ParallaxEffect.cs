using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ParallaxEffect : MonoBehaviour
{
    public Transform cinemachineCameraTransform; 
    public float parallaxFactorX = 0.5f; 
    public float parallaxFactorY = 0.5f; 

    private Vector3 lastCameraPosition;

    private void Start()
    {
        if (cinemachineCameraTransform == null)
        {
            Debug.LogWarning("Cinemachine Camera Transform is not assigned.");
            return;
        }

        lastCameraPosition = cinemachineCameraTransform.position;
    }

    private void LateUpdate()
    {
        if (cinemachineCameraTransform == null || transform == null)
        {
            return;
        }

        Vector3 cameraDelta = cinemachineCameraTransform.position - lastCameraPosition;
        transform.position += new Vector3(cameraDelta.x * parallaxFactorX, cameraDelta.y * parallaxFactorY, 0);
        lastCameraPosition = cinemachineCameraTransform.position;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (cinemachineCameraTransform == null)
        {
            Debug.LogWarning("Cinemachine Camera Transform is null after scene load. Attempting to reassign...");
            // You can attempt to find the camera in the scene if not manually assigned
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                cinemachineCameraTransform = mainCamera.transform;
                Debug.Log("Cinemachine Camera Transform reassigned.");
                lastCameraPosition = cinemachineCameraTransform.position; // Reset camera position
            }
            else
            {
                Debug.LogError("Main Camera not found in the scene. ParallaxEffect will not function.");
            }
        }
    }
}
