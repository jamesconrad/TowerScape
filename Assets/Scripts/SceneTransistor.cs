﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransistor : MonoBehaviour {

    [Tooltip("Animation to be played before the transition actually happens")]
    public Animation PlayerTransitionAnim;

    [Tooltip("Scene to transition to.")]
    public string sceneName;
    [Tooltip("Controller axis to actually transition.")]
    public string controllerAxis = "";
    [Tooltip("Key used as backup to actually transition.")]
    public KeyCode transitionKey;
    [Tooltip("Interaction text.")]
    public string interactionText;
    [Tooltip("Canvas interaction pane.")]
    public InteractionPane pane;

    private bool playerInArea = false;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        if (playerInArea && (Input.GetKeyDown(transitionKey) || (controllerAxis != "" && Input.GetAxis(controllerAxis) > 0.1)))
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
	}

    void OnTriggerEnter(Collider collision)
    {
        print("Object entered with tag: " + collision.gameObject.tag);
        if (collision.transform.CompareTag("PlayerRoot"))
        {
            pane.SetActive(true);
            pane.SetText(interactionText);
            playerInArea = true;
        }
    }

    void OnTriggerExit(Collider collision)
    {
        if (collision.transform.CompareTag("PlayerRoot"))
        {
            pane.SetActive(false);
            playerInArea = false;
        }
    }
}
