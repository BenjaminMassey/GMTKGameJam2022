using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour
{
    [SerializeField]
    private GameObject mPlayerObj;

    [SerializeField]
    private GameObject mCameraObj;

    [SerializeField]
    private float mSpeedFrontBack = 2.0f;

    [SerializeField]
    private float mSpeedLeftRight = 2.0f;

    [SerializeField]
    private float mJumpPower = 1000.0f;

    [SerializeField]
    private float mMouseSpeed = 20.0f;

    [SerializeField]
    private float mTurnSpeed = 2.5f;

    [SerializeField]
    private float mMaxVelocity = 20.0f;

    private Rigidbody mPlayerRB;

    private GameObject mDice;
    private GameObject mBottom;

    private bool mJumping = false;
    private bool mDescent = false;

    private bool mWiggleFeetFront = true;

    // Start is called before the first frame update
    void Start() 
    {
        mPlayerRB = mPlayerObj.GetComponent<Rigidbody>();

        mDice = mPlayerObj.transform.Find("Dice").gameObject;
        mBottom = mPlayerObj.transform.Find("Bottom").gameObject;

        Cursor.lockState = CursorLockMode.Locked;

        StartCoroutine(HandleTurning());
    }

    // Update is called once per frame
    void Update() 
    {
        if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        Vector3 force = Vector3.zero;

        force += Move();

        if (force != Vector3.zero && !mJumping) WiggleLegs();

        force += Jump();

        mPlayerRB.AddForce(force, ForceMode.Force);

        HandleCamera();

        MaxSpeed();
    }

    Vector3 Move() 
    {
        int x = 0;
        int y = 0;
        x += Input.GetKey(KeyCode.D) ? 1 : 0;
        x -= Input.GetKey(KeyCode.A) ? 1 : 0;
        y += Input.GetKey(KeyCode.W) ? 1 : 0;
        y -= Input.GetKey(KeyCode.S) ? 1 : 0;
        if (x != 0 || y != 0)
        {
            Vector3 moveX = mCameraObj.transform.right * x * mSpeedLeftRight;
            Vector3 moveY = mCameraObj.transform.forward * y * mSpeedFrontBack;
            return moveX + moveY;
        }
        else return Vector3.zero;
    }

    Vector3 Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !mJumping)
        {
            StartCoroutine(HandleJumping());
            return new Vector3(0.0f, mJumpPower, 0.0f);
        }
        else return Vector3.zero;
    }

    void HandleCamera()
    {
        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");
        Vector3 euler = mCameraObj.transform.eulerAngles;
        mCameraObj.transform.RotateAround(
            mPlayerObj.transform.position,
            Vector3.up,
            x * mMouseSpeed
        );
        mCameraObj.transform.eulerAngles = new Vector3(
            euler.x,
            mCameraObj.transform.eulerAngles.y,
            euler.z
        );
    }

    IEnumerator HandleTurning() 
    {
        while (true) { 
            foreach (GameObject part in new GameObject[] { mDice, mBottom })
            {
                float diff = mCameraObj.transform.eulerAngles.y - part.transform.eulerAngles.y;
                if (diff > 180.0f) diff -= 360.0f;
                else if (diff < -180.0f) diff += 360.0f;
                part.transform.Rotate(
                    new Vector3(
                        0.0f,
                        diff,
                        0.0f),
                    diff > mTurnSpeed || diff < -mTurnSpeed ? mTurnSpeed : 0.0f
                );
            }
            yield return new WaitForSeconds(1.0f / 100.0f);
        }
    }

    IEnumerator HandleJumping() 
    {
        if (mJumping) yield break;
        
        mJumping = true;
        while (mJumping) {
            Vector3 vel = mPlayerRB.velocity;
            if (vel.y < 0.0f && !mDescent) mDescent = true;
            else if (vel.y == 0.0f && mDescent) mJumping = false;
            yield return new WaitForFixedUpdate();
        }
        mDescent = false;
    }

    void MaxSpeed() 
    {
        Vector3 vel = mPlayerRB.velocity;
        mPlayerRB.velocity = new Vector3(
            Mathf.Min(vel.x, mMaxVelocity),
            vel.y,
            Mathf.Min(vel.z, mMaxVelocity)
        );
    }

    void WiggleLegs() {
        mBottom.transform.Rotate(mBottom.transform.up, mWiggleFeetFront ? 0.35f : -0.35f);
        if ((Time.frameCount % 250.0f) == 0) mWiggleFeetFront = !mWiggleFeetFront;
    }

}
