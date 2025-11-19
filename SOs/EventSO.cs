using JYW.Game.Commons;
using System.Collections.Generic;
using UnityEngine;

namespace JYW.Game.SOs
{
    [CreateAssetMenu(menuName = "Game/Event Data")]
    public class EventSO : ScriptableObject
    {

        [SerializeField] private bool isSpawnObject = false;

        [SerializeField] private GameObject spawnPrefab;

        [SerializeField] private string spawnNaming = "";

        [SerializeField] private Vector3 spawnObjectPosition;

        [SerializeField] private bool isUIOpen;

        [SerializeField] private GameObject uiObject;

        [SerializeField] private bool isSoftSpeech;

        [SerializeField] private List<string> softSpeechTexts = new List<string>();

        [SerializeField] private bool isHardSpeech;

        [SerializeField] private List<string> hardSpeechTexts = new List<string>();
        
        [SerializeField] private KeyCode hardSpeechKey;

        [SerializeField] private bool isJustText;

        [TextArea][SerializeField] private string justText = "";

        [SerializeField] private bool isRotation; //물체를 회전시킬 것인가

        [SerializeField] private GameObject rotationGameObject; //회전시킬 물체

        [SerializeField] private Vector3 rotationEuler; //현재로부터 몇도 회전시킬 것인가

        [SerializeField] private float rotationDuration; //회전하는데 걸리는 시간

        [SerializeField] private bool isMoving; //물체를 이동시킬 것인가

        [SerializeField] private GameObject movingGameObject; //회전시킬 물체

        [SerializeField] private Vector3 movingDistance; //현재로부터 얼마나 이동시킬 것인가

        [SerializeField] private float movingDuration; //이동하는데 걸리는 시간

        [SerializeField] private bool isUnLockMove;

        [SerializeField] private bool isLockMove;

        [SerializeField] private float moveLockDuration;

        [SerializeField] private bool isUnLockCamera;

        [SerializeField] private bool isLockCamera;

        [SerializeField] private float cameraLockDuration;

        [SerializeField] private bool isSceneChange;

        [SerializeField] private Scenes eventscenechange;

        [SerializeField] private bool isSound;

        [SerializeField] private SoundData sound;

        [SerializeField] private bool isSave;

        [SerializeField] private int save = -1;

        [SerializeField] private bool isFade = false;

        [SerializeField] private FadeInfo[] fadeInfos;

        [SerializeField] private bool isCameraMove; //카메라 이동 연출

        [SerializeField] private CameraMovementData[] cameraMovements;

        [SerializeField] private bool isCameraAiming; //카메라 조준 연출

        [SerializeField] private CameraAimData[] cameraAims; //조준하는 오브젝트 이름과 시간



        public bool IsSpawnObject => isSpawnObject;
        public GameObject SpawnPrefab => spawnPrefab;
        public string SpawnNaming => spawnNaming;
        public Vector3 SpawnObjectPosition => spawnObjectPosition;
        public bool IsUIOpen => isUIOpen;
        public GameObject UIObject => uiObject;
        public bool IsSoftSpeech => isSoftSpeech;
        public IReadOnlyList<string> SoftSpeechTexts => softSpeechTexts;
        public bool IsHardSpeech => isHardSpeech;
        public IReadOnlyList<string> HardSpeechTexts => hardSpeechTexts;
        public KeyCode HardSpeechKey => hardSpeechKey;
        public bool IsJustText => isJustText;
        public string JustText => justText;
        public bool IsRotation => isRotation;
        public GameObject RotationGameObject => rotationGameObject;
        public Vector3 RotationEuler => rotationEuler;
        public float RotationDuration => rotationDuration;
        public bool IsMoving => isMoving;
        public GameObject MovingGameObject => movingGameObject;
        public Vector3 MovingDistance => movingDistance;
        public float MovingDuration => movingDuration;
        public bool IsUnLockMove => isUnLockMove;
        public bool IsLockMove => isLockMove;
        public float MoveLockDuration => moveLockDuration;
        public bool IsUnLockCamera => isUnLockCamera;
        public bool IsLockCamera => isLockCamera;
        public float CameraLockDuration => cameraLockDuration;
        public bool IsSceneChange => isSceneChange;
        public Scenes EventSceneChange => eventscenechange;
        public bool IsSound => isSound;
        public SoundData Sound => sound;
        public bool IsSave => isSave;
        public int Save => save;
        public bool IsFade => isFade;
        public FadeInfo[] FadeInfos => fadeInfos;
        public bool IsCameraMove => isCameraMove;
        public CameraMovementData[] CameraMovements => cameraMovements;
        public bool IsCameraAiming => isCameraAiming; //카메라 무언가를 조준합니까?
        public CameraAimData[] CameraAims => cameraAims; //조준하는 오브젝트 위치



    }


}