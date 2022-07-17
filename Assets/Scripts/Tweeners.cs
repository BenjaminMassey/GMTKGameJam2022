using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tweeners : MonoBehaviour
{
    public void TweenPosition(Transform t, Vector3 target_pos, float total_time, Action update = null, Action finish = null)
    {
        StartCoroutine(TweenPositionCoroutine(t, target_pos, total_time, update, finish));
    }

    public void TweenLocalPosition(Transform t, Vector3 target_pos, float total_time, Action update = null, Action finish = null)
    {
        StartCoroutine(TweenLocalPositionCoroutine(t, target_pos, total_time, update, finish));
    }

    public void TweenRotation(Transform t, Vector3 target_euler, float total_time, Action update = null, Action finish = null)
    {
        StartCoroutine(TweenRotationCoroutine(t, target_euler, total_time, update, finish));
    }

    public void TweenNormal(float total_time, Action<float> update, Action finish = null)
    {
        StartCoroutine(TweenNormalCoroutine(total_time, update, finish));
    }

    private IEnumerator TweenPositionCoroutine(Transform t, Vector3 target_pos, float total_time, Action update, Action finish)
    {
        Vector3 start_pos = t.position;
        float time_step = 1.0f / 50.0f;
        float steps = total_time / time_step;
        Vector3 amount = (target_pos - start_pos) / steps;
        for (int i = 0; i < steps; i++)
        {
            t.position = t.position + amount;
            if (update != null) update();
            yield return new WaitForSeconds(time_step);
        }
        t.position = target_pos;
        if (finish != null) finish();
    }

    private IEnumerator TweenLocalPositionCoroutine(Transform t, Vector3 target_pos, float total_time, Action update, Action finish)
    {
        Vector3 start_pos = t.localPosition;
        float time_step = 1.0f / 50.0f;
        float steps = total_time / time_step;
        Vector3 amount = (target_pos - start_pos) / steps;
        for (int i = 0; i < steps; i++)
        {
            t.localPosition = t.localPosition + amount;
            if (update != null) update();
            yield return new WaitForSeconds(time_step);
        }
        t.localPosition = target_pos;
        if (finish != null) finish();
    }

    private IEnumerator TweenRotationCoroutine(Transform t, Vector3 target_euler, float total_time, Action update, Action finish)
    {
        Vector3 start_euler = t.eulerAngles;
        float time_step = 1.0f / 50.0f;
        float steps = total_time / time_step;
        Vector3 amount = (target_euler - start_euler) / steps;
        for (int i = 0; i < steps; i++)
        {
            t.Rotate(amount);
            if (update != null) update();
            yield return new WaitForSeconds(time_step);
        }
        t.eulerAngles = target_euler;
        if (finish != null) finish();
    }

    private IEnumerator TweenNormalCoroutine(float total_time, Action<float> update, Action finish)
    {
        float time_step = 1.0f / 50.0f;
        float steps = total_time / time_step;
        for (int i = 0; i < steps; i++)
        {
            if (update != null) update(i / (float) steps);
            yield return new WaitForSeconds(time_step);
        }
        if (update != null) update(1.0f);
        if (finish != null) finish();
    }
}
