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

    private State mState;
    private Tweeners mTweeners;

    private void Start()
    {
        mState = GameObject.Find("Master").GetComponent<State>();
        mTweeners = GameObject.Find("Master").GetComponent<Tweeners>();
    }

    public void Attack(int i, Vector3 position) 
    {
        switch (i)
        {
            case 1:
                mState.MainMessage("~~~ FIRE ~~~", 1.0f);
                Fire(position);
                break;
            case 2:
                mState.MainMessage("~~~ ICE ~~~", 1.0f);
                Ice(position);
                break;
            case 3:
                mState.MainMessage("~~~ LIGHTNING ~~~", 1.0f);
                Lightning(position);
                break;
            case 4:
                mState.MainMessage("~~~ NATURE ~~~", 1.0f);
                Nature(position);
                break;
            case 5:
                mState.MainMessage("~~~ ROCK ~~~", 1.0f);
                Rock(position);
                break;
            case 6:
                mState.MainMessage("~~~ VOID ~~~", 1.0f);
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
