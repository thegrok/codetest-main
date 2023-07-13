using System;
using System.Collections.Generic;
using UnityEngine;

public class GhostSystem : MonoBehaviour
{
    [SerializeField] PlayerControl ghostControl;

    private static PlayerInputRecording recording;

    private bool isRecording = false;
    private bool isPlayingBack = false;

    private int playbackIndex;
    private float playbackStartTime;

    public bool IsRecording { get => isRecording; }

    private Transform targetPosition;
    private Transform targetDirection;
    private Rigidbody targetRigidbody;
    private KeyCode[] observationKeys;

    private bool[] keyStates = new bool[Enum.GetNames(typeof(KeyCode)).Length];


    public void SetObservationTargets(Transform targetPosition, Transform targetDirection, Rigidbody targetRigidBody, KeyCode[] observationKeys)
    {
        this.targetPosition = targetPosition;
        this.targetDirection = targetDirection;
        this.targetRigidbody = targetRigidBody;
        this.observationKeys = observationKeys;
    }

    public void ToggleRecording()
    {
        if (!isRecording)
        {
            recording = new PlayerInputRecording();
            recording.recordingStartTime = Time.time;
            recording.startPosition = targetPosition.position;
            recording.startDirection = targetDirection.forward;
            recording.startVelocity = targetRigidbody.velocity;

            ghostControl.gameObject.SetActive(false);

            AddInitialSnapshot();
        }
        else
        {
            AddFinalSnapshot();
        }

        isRecording = !isRecording;
    }

    private void AddInitialSnapshot()
    {
        foreach (var key in observationKeys)
        {
            int stateIndex = (int)key;

            recording.snapshots.Add(new PlayerInputSnapshot
            {
                keyCodeIndex = stateIndex,
                keyState = Input.GetKey(key),
                timeOffset = 0
            });
        }
    }

    private void AddFinalSnapshot()
    {
        foreach (var key in observationKeys)
        {
            int stateIndex = (int)key;

            recording.snapshots.Add(new PlayerInputSnapshot
            {
                keyCodeIndex = stateIndex,
                keyState = false,
                timeOffset = Time.time - recording.recordingStartTime
            });
        }
    }

    void Update()
    {
        if (IsRecording)
        {
            foreach (var key in observationKeys)
            {
                CheckKeyStateChange(key);
            }
        }
        else if (isPlayingBack)
        {
            ApplyKeyStateChange();
        }
    }

    private void CheckKeyStateChange(KeyCode keyCode)
    {
        int stateIndex = (int)keyCode;
        bool keyState;
        
        if (Input.GetKeyDown(keyCode))
        {
            keyState = true;
        }
        else if (Input.GetKeyUp(keyCode))
        {
            keyState = false;
        }
        else
        {
            return;
        }

        recording.snapshots.Add(new PlayerInputSnapshot
        {
            keyCodeIndex = stateIndex,
            keyState = keyState,
            timeOffset = Time.time - recording.recordingStartTime
        });
    }

    public void StartPlayback()
    {
        if (recording == null)
        {
            Debug.Log("Nothing's been recorded, so there's nothing to playback!");
            return;
        }

        if (isRecording)
        {
            ToggleRecording();
        }

        isPlayingBack = true;
        playbackIndex = 0;
        playbackStartTime = Time.time;

        ghostControl.gameObject.SetActive(true);

        ApplyStartStateToTarget(ghostControl);
    }

    public void ApplyStartStateToTarget(PlayerControl control)
    {
        control.UpdateVelocity(recording.startPosition, recording.startDirection, recording.startVelocity);
    }

    private void ApplyKeyStateChange()
    {
        var snapshot = recording.snapshots[playbackIndex];
        if (snapshot.timeOffset <= Time.time - playbackStartTime)
        {
            keyStates[snapshot.keyCodeIndex] = snapshot.keyState;
            
            playbackIndex++;

            if (playbackIndex == recording.snapshots.Count)
            {
                isPlayingBack = false;
            }
        }

        ghostControl.ApplyKeyStates(keyStates);
    }

    private class PlayerInputRecording
    {
        public Vector3 startPosition;
        public Vector3 startDirection;
        public Vector3 startVelocity;

        public float recordingStartTime;
        public List<PlayerInputSnapshot> snapshots = new List<PlayerInputSnapshot>();
    }

    private class PlayerInputSnapshot
    {
        public int keyCodeIndex;
        public bool keyState;
        public float timeOffset;
    }
}
