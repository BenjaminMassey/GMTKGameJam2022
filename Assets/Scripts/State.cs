using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State : MonoBehaviour
{
    [SerializeField]
    private int mPlayerHealth = 100;

    [SerializeField]
    private int mPlayerMaxHealth = 100;

    [SerializeField]
    private GameObject mHealth;

    [SerializeField]
    private GameObject mMainTextObj;

    private Text mMainText;

    private Tweeners mTweeners;

    private float mHealthbarMax;

    private void Start()
    {
        mMainText = mMainTextObj.GetComponent<Text>();
        mTweeners = GetComponent<Tweeners>();
        mHealthbarMax = ((RectTransform)mHealth.transform).rect.width;
    }

    // Update is called once per frame
    private void Update()
    {

    }

    private void FixedUpdate()
    {

    }

    public void Damage(int x)
    {
        mPlayerHealth -= x;
        mPlayerHealth = Mathf.Clamp(mPlayerHealth, 0, mPlayerMaxHealth);
        SyncHealth();
    }

    public void MainMessage(string message, float t)
    {
        SetMainText(message);
        mTweeners.TimedCallback(() => SetMainText(""), t);
    }

    private void SyncHealth()
    {
        RectTransform rt = (RectTransform)mHealth.transform;
        float percent = mPlayerHealth / (float)mPlayerMaxHealth;
        Vector2 start = rt.sizeDelta;
        Vector2 target = new Vector2(percent * mHealthbarMax, rt.rect.height);
        mTweeners.TweenNormal(0.25f, (float t) => rt.sizeDelta = start + (t * (target - start)));
    }

    private void SetMainText(string text)
    {
        mMainText.text = text;
    }
}
