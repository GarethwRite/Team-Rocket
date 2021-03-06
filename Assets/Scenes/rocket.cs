﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] float levelLoadDelay = 2f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip deathExpolsion;
    [SerializeField] AudioClip nextLevel;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem nextLevelParticles;
    [SerializeField] ParticleSystem DeathExpolsionParticles;
   

    Rigidbody rigidBody;
    AudioSource audioSource;

    enum State { Alive, Dying, Transending };
    State state = State.Alive;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            RespondToRotateInput();
            RespondToThrustInput();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(state != State.Alive) { return; }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                // do nothing to rocket             
                break;
            case "Finish":              
                StartNextLevelSequence();              
                break;
            default:               
                StartDeathSequence();
                //Destroy(gameObject, 1f);
                break;

        }
    }

    private void StartNextLevelSequence()
    {
        state = State.Transending;
        audioSource.Stop();
        audioSource.PlayOneShot(nextLevel);
        nextLevelParticles.Play();
        Invoke("LoadNextLevel", levelLoadDelay);
    }
    private void StartDeathSequence()
    {
        state = State.Dying;
        audioSource.Stop();       
        audioSource.PlayOneShot(deathExpolsion);
        DeathExpolsionParticles.Play();      
        Invoke("LoadFirstLevel", levelLoadDelay);
    }



    private void LoadNextLevel()
    {
        SceneManager.LoadScene(1);
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void RespondToRotateInput()
    {
        rigidBody.freezeRotation = true; //take manual control of rotation

        float rotationThisFrame = rcsThrust * Time.deltaTime;


        if (Input.GetKey(KeyCode.LeftArrow))
        {       
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
        rigidBody.freezeRotation = false; // resumes physics control of rotation

    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();          
        }
        
        else
        {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
            mainEngineParticles.Play();
        }
        
    }
}
