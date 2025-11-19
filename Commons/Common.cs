using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Diagnostics;

namespace JYW.Game.Commons
{
    public enum Scenes
    {
        Init,
        Title,
        Chapter1
    }

    [System.Serializable]
    public class CameraMovementData
    {
        public Vector3 StartPosition; // 카메라 시작 위치
        public Vector3 EndPosition;   // 카메라 끝 위치
        public float Duration;        // 이동 시간
    }

    [System.Serializable]
    public class CameraAimData
    {
        public string aimedTargetName; 
        public float Duration;        // 지속 시간
    }

    [System.Serializable]
    public class SoundData
    {
        public AudioClip audioClip;
        public float volume = 1.0f;
        public bool isLoop = false;
    }

    [System.Serializable]
    public class FadeInfo
    {
        public bool isFadeIn;
        public float duration;
    }


    [System.Serializable]
    public class AimingIntractionPrompt
    {
        public string requiredItem;
        public string promptText;
        public KeyCode interactionKey = KeyCode.E;
    }
}