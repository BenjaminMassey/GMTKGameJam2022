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

    private Quaternion mPlayerStartRot;
    private float mPlayerStartY;

    private Quaternion mCameraStartRot;
    private Vector3 mCameraStartLocalPos;

    private float mCameraPlayerDistance;

    private State mState;
    private Tweeners mTweeners;
    private Attacks mAttacks;

    private Rigidbody mPlayerRB;

    private GameObject mDice;
    private GameObject mBottom;

    private bool mExploding = false;

    private bool mJumping = false;
    private bool mDescent = false;

    private bool mWiggleFeetFront = true;

    private int mSide = 1;

    // Start is called before the first frame update
    void Start() 
    {
        mPlayerStartRot = mPlayerObj.transform.rotation;
        mPlayerStartY = mPlayerObj.transform.position.y;

        mCameraStartRot = mCameraObj.transform.rotation;
        mCameraStartLocalPos = mCameraObj.transform.localPosition;

        mCameraPlayerDistance = Vector3.Distance(mPlayerObj.transform.position, mCameraObj.transform.position);

        mState = GameObject.Find("Master").GetComponent<State>();

        mTweeners = GameObject.Find("Master").GetComponent<Tweeners>();

        mAttacks = mPlayerObj.GetComponent<Attacks>();

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

        if (Input.GetKeyDown(KeyCode.E)) Explode();

        AttackDebug();

        DamageDebug();

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
        if (mExploding) return Vector3.zero;

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
        if (mExploding) return Vector3.zero;

        if (Input.GetKeyDown(KeyCode.Space) && !mJumping)
        {
            StartCoroutine(HandleJumping());
            return new Vector3(0.0f, mJumpPower, 0.0f);
        }
        else return Vector3.zero;
    }

    void HandleCamera()
    {
        if (mExploding) return;

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
            if (!mExploding)
            {
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
            }
            yield return new WaitForSeconds(1.0f / 100.0f);
        }
    }

    IEnumerator HandleJumping()
    {
        if (mJumping || mExploding) yield break;
        
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
        if (mExploding) return;

        Vector3 vel = mPlayerRB.velocity;
        mPlayerRB.velocity = new Vector3(
            Mathf.Min(vel.x, mMaxVelocity),
            vel.y,
            Mathf.Min(vel.z, mMaxVelocity)
        );
    }

    void WiggleLegs() {
        if (mExploding) return;

        mBottom.transform.Rotate(mBottom.transform.up, mWiggleFeetFront ? 0.45f : -0.45f);
        if ((Time.frameCount % 200.0f) == 0) mWiggleFeetFront = !mWiggleFeetFront;
    }

    void Explode() 
    {
        if (mExploding) return;

        mExploding = true;
        mCameraObj.transform.SetParent(null);
        mPlayerRB.constraints = RigidbodyConstraints.None;
        mPlayerRB.AddExplosionForce(500.0f, mPlayerObj.transform.forward, 100.0f);
        mPlayerRB.AddTorque(mPlayerObj.transform.right);
        mDice.transform.Find("Base").gameObject.GetComponent<BoxCollider>().isTrigger = false;
        GetComponent<BoxCollider>().isTrigger = true;
        mBottom.SetActive(false);
        StartCoroutine(CheckReset());
    }

    IEnumerator CheckReset()
    {
        bool started = false;
        Transform camT = mCameraObj.transform;
        Transform diceT = mDice.transform;
        int count = 0;
        while (true)
        {
            if (started &&
                Mathf.Abs(mPlayerRB.velocity.x) < 0.05f &&
                Mathf.Abs(mPlayerRB.velocity.y) < 0.05f &&
                Mathf.Abs(mPlayerRB.velocity.z) < 0.05f &&
                Mathf.Abs(mPlayerRB.angularVelocity.x) < 0.05f &&
                Mathf.Abs(mPlayerRB.angularVelocity.y) < 0.05f &&
                Mathf.Abs(mPlayerRB.angularVelocity.z) < 0.05f)
            {
                count++;
            }
            else if (!started &&
                     Mathf.Abs(mPlayerRB.velocity.x) > 0.05f &&
                     Mathf.Abs(mPlayerRB.velocity.y) > 0.05f &&
                     Mathf.Abs(mPlayerRB.velocity.z) > 0.05f &&
                     Mathf.Abs(mPlayerRB.angularVelocity.x) > 0.05f &&
                     Mathf.Abs(mPlayerRB.angularVelocity.y) > 0.05f &&
                     Mathf.Abs(mPlayerRB.angularVelocity.z) > 0.05f)
            {
                started = true;
            }
            else
            {
                count = 0;
            }

            if (count >= 50) break;

            camT.LookAt(mDice.transform);
            float dist = Vector3.Distance(camT.position, diceT.position);
            camT.Translate(camT.forward * (dist - mCameraPlayerDistance));

            yield return new WaitForEndOfFrame();
        }
        
        camT.eulerAngles = Vector3.zero;
        mTweeners.TweenRotation(camT, new Vector3(90.0f, 0.0f, 0.0f), 0.25f);
        mTweeners.TweenPosition(camT, diceT.position + (Vector3.up * (mCameraPlayerDistance * 0.5f)),  0.25f);
        yield return new WaitForSeconds(0.5f); // includes 0.5 tween

        mSide = GetSide();
        Debug.Log("Got side of " + mSide);
        mAttacks.Attack(mSide, diceT.position);
        PerformReset();
    }

    void PerformReset()
    {
        mPlayerRB.velocity = Vector3.zero;
        mPlayerRB.angularVelocity = Vector3.zero;
        mPlayerObj.transform.rotation = mPlayerStartRot;
        Vector3 pos = mPlayerObj.transform.position;
        mTweeners.TweenPosition(mPlayerObj.transform, new Vector3(pos.x, mPlayerStartY, pos.z), 0.25f);
        mCameraObj.transform.SetParent(mPlayerObj.transform);
        mTweeners.TweenRotation(mCameraObj.transform, mCameraStartRot.eulerAngles, 0.25f);
        mTweeners.TweenLocalPosition(mCameraObj.transform, mCameraStartLocalPos, 0.25f);
        mPlayerRB.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        mDice.transform.Find("Base").gameObject.GetComponent<BoxCollider>().isTrigger = true;
        GetComponent<BoxCollider>().isTrigger = false;
        mBottom.SetActive(true);
        RotateFaces();
        mExploding = false;
    }

    void RotateFaces()
    {
        Vector3 euler = Vector3.zero;
        switch (mSide)
        {
            case 1:
                euler = new Vector3(0.0f, 0.0f, 0.0f);
                break;
            case 2:
                euler = new Vector3(0.0f, 0.0f, -90.0f);
                break;
            case 3:
                euler = new Vector3(-90.0f, 0.0f, 0.0f);
                break;
            case 4:
                euler = new Vector3(90.0f, 0.0f, 0.0f);
                break;
            case 5:
                euler = new Vector3(0.0f, 0.0f, 90.0f);
                break;
            case 6:
                euler = new Vector3(180.0f, 0.0f, 0.0f);
                break;
            default:
                euler = Vector3.zero;
                break;
        }
        mDice.transform.Find("Faces").localEulerAngles = euler;
    }

    int GetSide()
    {
        float max = -999.9f;
        int result = -1;
        int i = 1;
        foreach (string name in new string[] { "One", "Two", "Three", "Four", "Five", "Six" })
        {
            float val = mDice.transform.Find("Faces").transform.Find(name).transform.GetChild(0).transform.position.y;
            if (val > max) {
                max = val;
                result = i;
            }
            i++;
        }
        return result;
    }

    void AttackDebug()
    {
        Vector3 pos = mDice.transform.position + (Vector3.down * 3.0f);
        if (Input.GetKeyDown(KeyCode.Alpha1)) mAttacks.Attack(1, pos);
        if (Input.GetKeyDown(KeyCode.Alpha2)) mAttacks.Attack(2, pos);
        if (Input.GetKeyDown(KeyCode.Alpha3)) mAttacks.Attack(3, pos);
        if (Input.GetKeyDown(KeyCode.Alpha4)) mAttacks.Attack(4, pos);
        if (Input.GetKeyDown(KeyCode.Alpha5)) mAttacks.Attack(5, pos);
        if (Input.GetKeyDown(KeyCode.Alpha6)) mAttacks.Attack(6, pos);
    }

    void DamageDebug()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) mState.Damage(-10);
        if (Input.GetKeyDown(KeyCode.DownArrow)) mState.Damage(10);
    }
}
