using JYW.Game.EventPlay;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class TV : MonoBehaviour
{

    [SerializeField] private VideoPlayer videoComp;

    private void Start()
    {
        EventPlayManager.Instance.AddAction(gameObject, TVOnOff);
    }

    public void TVOnOff()
    {
        videoComp.Play();
        StartCoroutine(Stop());
    }

    IEnumerator Stop()
    {
        yield return new WaitForSeconds(15f);
        videoComp.Stop();
    }
}
