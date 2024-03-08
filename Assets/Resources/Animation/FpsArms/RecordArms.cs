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
        // 여기서 recordedClip을 사용하거나 저장할 수 있습니다.
    }
    public AnimationClip cl;
 
    // 런타임에 생성된 애니메이션 클립을 에셋으로 저장하는 예제 함수
    void SaveAnimationClip(AnimationClip clip, string path)
    {
    #if UNITY_EDITOR
        // 에셋으로 저장
        AssetDatabase.CreateAsset(clip, $"Assets/@ConverterOutPut/{path}.anim");
        AssetDatabase.SaveAssets();
    
        // 필요한 경우 여기에서 AssetBundle을 생성하고 애니메이션 클립을 추가하는 로직을 구현
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
        // 애니메이션 클립 사용 예:
        // GetComponent<Animation>().AddClip(recordedClip, "recordedAnimation");
    }
}
