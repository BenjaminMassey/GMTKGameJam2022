using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacks : MonoBehaviour
{
    [SerializeField]
    private GameObject mLightning;

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
                break;
            case 2:
                break;
            case 3:
                Lightning(position);
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                break;
            default:
                Debug.Log("Unknown attack " + i);
                break;
        }
    }

    private void Lightning(Vector3 position)
    {
        GameObject instance = GameObject.Instantiate(mLightning);
        instance.transform.position = position;
        mTweeners.TimedCallback(() => Destroy(instance), 1.0f);
    }
}
