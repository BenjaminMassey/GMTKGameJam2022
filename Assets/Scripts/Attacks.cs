using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacks : MonoBehaviour
{
    [SerializeField]
    private GameObject mFire;
    [SerializeField]
    private GameObject mIce;
    [SerializeField]
    private GameObject mLightning;
    [SerializeField]
    private GameObject mNature;
    [SerializeField]
    private GameObject mRock;
    [SerializeField]
    private GameObject mVoid;

    private Tweeners mTweeners;

    private void Start()
    {
        mTweeners = GameObject.Find("Master").GetComponent<Tweeners>();
    }

    public void Attack(int i, Vector3 position) 
    {
        switch (i)
        {
            case 1:
                Fire(position);
                break;
            case 2:
                Ice(position);
                break;
            case 3:
                Lightning(position);
                break;
            case 4:
                Nature(position);
                break;
            case 5:
                Rock(position);
                break;
            case 6:
                Void(position);
                break;
            default:
                Debug.Log("Unknown attack " + i);
                break;
        }
    }

    private void Fire(Vector3 position)
    {
        Generic(mFire, position);
    }

    private void Ice(Vector3 position)
    {
        Generic(mIce, position);
    }

    private void Lightning(Vector3 position)
    {
        Generic(mLightning, position);
    }

    private void Nature(Vector3 position)
    {
        Generic(mNature, position);
    }

    private void Rock(Vector3 position)
    {
        Generic(mRock, position);
    }

    private void Void(Vector3 position)
    {
        Generic(mVoid, position);
    }

    private void Generic(GameObject go, Vector3 position)
    {
        GameObject instance = GameObject.Instantiate(go);
        instance.transform.position = position;
        mTweeners.TimedCallback(() => Destroy(instance), 1.0f);
    }
}
