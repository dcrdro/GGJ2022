using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWall : MonoBehaviour
{
    public BladePedestal Pedestal;
    public float MoveTime;
    private float TimeLeft = 0;
    public MoveDirection Direction;
    public float Speed;

    public enum MoveDirection
    {
        up,
        down,
        right,
        left,
        back,
        forward
    }

    private void Start()
    {
        Pedestal.KeyActivate += Activated;
    }
    void Update()
    {
        if (TimeLeft >= 0)
        {
            Vector3 dir;
            switch (Direction)
            {
                case MoveDirection.up:
                    dir = Vector3.up;
                    break;
                case MoveDirection.down:
                    dir = Vector3.down;
                    break;
                case MoveDirection.right:
                    dir = Vector3.right;
                    break;
                case MoveDirection.left:
                    dir = Vector3.left;
                    break;
                case MoveDirection.back:
                    dir = Vector3.back;
                    break;
                case MoveDirection.forward:
                    dir = Vector3.forward;
                    break;
                default:
                    dir = Vector3.down;
                    break;
            }
            transform.position += dir * Speed * Time.deltaTime;
        }
        TimeLeft -= Time.deltaTime;
    }
    private void Activated()
    {
        TimeLeft = MoveTime;
    }
}
