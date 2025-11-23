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
        public float StartTime;       // 이벤트 시작 후 몇초에 시작할지
        public float EndTime;         // 이벤트 시작 후 몇초에 끝낼지
    }

    [System.Serializable]
    public class CameraAimData
    {
        public string aimedTargetName;
        public float StartTime;       // 이벤트 시작 후 몇초에 시작할지
        public float EndTime;         // 이벤트 시작 후 몇초에 끝낼지
    }

    [System.Serializable]
    public class SoundData
    {
        public float time;
        public AudioClip audioClip;
        public float volume = 1.0f;
        public bool isLoop = false;
    }

    [System.Serializable]
    public class FadeInfo
    {
        public bool isFadeIn;

        // 새 필드: 이벤트 시작 후 몇초에 시작/끝낼지
        public float StartTime;
        public float EndTime;
    }

    [System.Serializable]
    public class AimingIntractionPrompt
    {
        public string requiredItem;
        public string promptText;
        public KeyCode interactionKey = KeyCode.E;
    }

    [System.Serializable]
    public class SpeechData
    {
        public float StartTime; // 이벤트 시작 후 몇초에 표시 시작
        public float EndTime;   // 이벤트 시작 후 몇초에 표시 끝
        [TextArea] public string Text;
    }
}