using JYW.Game.EventPlay;
using JYW.Game.Players;
using UnityEngine;

public class Shoes : MonoBehaviour
{
    [SerializeField] private PlayerMove playerMove;

    private void Start()
    {
        EventPlayManager.Instance.AddAction(gameObject, GetShoes);
    }

    public void GetShoes()
    {
        playerMove.Playerspeed += 1f;
    }
}
