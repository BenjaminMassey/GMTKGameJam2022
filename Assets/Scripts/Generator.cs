using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public bool active = true;

    [SerializeField]
    private GameObject mObject;
    [SerializeField]
    private float mTimeBetween;
    [SerializeField]
    private Vector2 mArea;

    private Tweeners mTweeners;

    // Start is called before the first frame update
    void Start()
    {
        mTweeners = GameObject.Find("Master").GetComponent<Tweeners>();
        StartCoroutine(Generatorer());
    }

    IEnumerator Generatorer()
    {
        while (active)
        {
            Generate();
            yield return new WaitForSeconds(mTimeBetween);
        }
    }

    void Generate()
    {
        GameObject instance = GameObject.Instantiate(mObject);
        float x = transform.position.x + Random.Range(-mArea.x, mArea.x);
        float y = transform.position.y;
        float z = transform.position.z + Random.Range(-mArea.y, mArea.y);
        instance.transform.position = new Vector3(x, y, z);
        Vector3 target = instance.transform.localScale;
        instance.transform.localScale = Vector3.zero;
        mTweeners.TweenScale(instance.transform, target, 0.5f);
    }
}
