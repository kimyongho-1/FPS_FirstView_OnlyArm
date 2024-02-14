using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SpineRotate : MonoBehaviour
{
#if UNITY_EDITOR
    public bool drawBone; public float boneSize = 0.1f;
    public bool drawNormal; public float arrowSize = 0.1f;
    public bool drawTarget; public float targetSize = 0.1f;

    public 
#endif
    InputReceiver myInput; public InputReceiver GetReceiver { get { if (myInput == null) { myInput = GetComponent<InputReceiver>(); } return myInput; } }
    Animator anim; public Animator GetAnim 
    { 
        get { if (anim == null) { anim = GetComponent<Animator>(); }
            return anim;} 
    }
    Transform Spine, Chest, UpperChest;
    public Transform[] GetUpperBody { get { return new Transform[] {
        GetAnim.GetBoneTransform(HumanBodyBones.Spine) ,
        GetAnim.GetBoneTransform(HumanBodyBones.Chest) ,
        GetAnim.GetBoneTransform(HumanBodyBones.UpperChest)
    }; } }
    private void Awake()
    {
        myInput = GetComponent<InputReceiver>();

        Spine = GetAnim.GetBoneTransform(HumanBodyBones.Spine);
        Chest = GetAnim.GetBoneTransform(HumanBodyBones.Chest);
        UpperChest = GetAnim.GetBoneTransform(HumanBodyBones.UpperChest);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        
    }
    private void LateUpdate()
    {
        // 하체회전시 왼쪽으로 회전시엔 회전 에러문제 (짐벌락 의심)
        float rot = Mathf.Lerp(transform.localRotation.eulerAngles.y, myInput.yawAmount, Time.deltaTime);
        transform.localRotation =
               Quaternion.Euler(Vector3.up * rot);
        Quaternion local = UpperChest.localRotation;
        // 로컬이 아닌 월드좌표로 한 이유는 로컬회전축의 경우 기본값떄문에 정직한 y축회전이 아닌 약간 틀어질 경우가 생김
        Chest.rotation = Quaternion.Euler(local.x, myInput.yawAmount * 0.5f, local.z);
        UpperChest.rotation = Quaternion.Euler(local.x, myInput.yawAmount, local.z);


        return;
        // 다리 회전 시작 (회전 진행 코루틴이 쉬고 있고 && 각도차가 40 이상이면)
        if ( StartSpine == null &&
             myInput.yawAmount - transform.localRotation.eulerAngles.y > 40f)
        {
            StartSpine = FollowTheUpper();
            StartCoroutine(StartSpine);
        }
    }
    IEnumerator StartSpine;
    IEnumerator FollowTheUpper()
    {
        float t = 0f;
        float start = transform.localRotation.eulerAngles.y;
        while (t < 1f)
        {
            t += Time.deltaTime;
            float rot = Mathf.Lerp( start , myInput.yawAmount, t);
            transform.localRotation = 
                Quaternion.Euler(Vector3.up * rot) ;
            yield return null;
        }
        // 회전 완료했기에, 코루틴 재활용위하여 비우기
        StartSpine = null;
    }
}
