using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

public class RecordArms : MonoBehaviour
{
    public Transform targetToRecord;
    public float recordTime = 5.0f;

    private AnimationClip recordedClip;
    private List<Keyframe> positionKeyframesX, positionKeyframesY, positionKeyframesZ;
    private float startTime;
    private bool isRecording = false;

    void Start()
    {
        positionKeyframesX = new List<Keyframe>();
        positionKeyframesY = new List<Keyframe>();
        positionKeyframesZ = new List<Keyframe>();
        StartRecording();
    }

    void StartRecording()
    {
        startTime = Time.time;
        isRecording = true;
    }

    void Update()
    {
        if (!isRecording) return;

        float timeSinceStart = Time.time - startTime;

        if (timeSinceStart > recordTime)
        {
            FinishRecording();
            return;
        }

        RecordFrame(timeSinceStart);
    }

    void RecordFrame(float frameTime)
    {
        positionKeyframesX.Add(new Keyframe(frameTime, targetToRecord.localPosition.x));
        positionKeyframesY.Add(new Keyframe(frameTime, targetToRecord.localPosition.y));
        positionKeyframesZ.Add(new Keyframe(frameTime, targetToRecord.localPosition.z));
    }

    void FinishRecording()
    {
        isRecording = false;
        CreateAnimationClipFromKeyframes();
        // ���⼭ recordedClip�� ����ϰų� ������ �� �ֽ��ϴ�.
    }
    public AnimationClip cl;
 
    // ��Ÿ�ӿ� ������ �ִϸ��̼� Ŭ���� �������� �����ϴ� ���� �Լ�
    void SaveAnimationClip(AnimationClip clip, string path)
    {
    #if UNITY_EDITOR
        // �������� ����
        AssetDatabase.CreateAsset(clip, $"Assets/@ConverterOutPut/{path}.anim");
        AssetDatabase.SaveAssets();
    
        // �ʿ��� ��� ���⿡�� AssetBundle�� �����ϰ� �ִϸ��̼� Ŭ���� �߰��ϴ� ������ ����
    #endif
    }
    void CreateAnimationClipFromKeyframes()
    {
        recordedClip = new AnimationClip();
        recordedClip.legacy = true;

        AnimationCurve curveX = new AnimationCurve(positionKeyframesX.ToArray());
        AnimationCurve curveY = new AnimationCurve(positionKeyframesY.ToArray());
        AnimationCurve curveZ = new AnimationCurve(positionKeyframesZ.ToArray());

        recordedClip.SetCurve("", typeof(Transform), "localPosition.x", curveX);
        recordedClip.SetCurve("", typeof(Transform), "localPosition.y", curveY);
        recordedClip.SetCurve("", typeof(Transform), "localPosition.z", curveZ);
        cl = recordedClip;
        SaveAnimationClip(cl, "neAPt");
        // �ִϸ��̼� Ŭ�� ��� ��:
        // GetComponent<Animation>().AddClip(recordedClip, "recordedAnimation");
    }
}
