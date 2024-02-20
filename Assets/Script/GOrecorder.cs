using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class GOrecorder : MonoBehaviour
{
    public CharacterRotate cr;
    public AnimationClip clips;

    public bool record = false;
    GameObjectRecorder m_Recorder;

    private void Start()
    {
        m_Recorder = new GameObjectRecorder(gameObject);

        // 녹화된 객체의 모든 자식들까지 포함시키기
        m_Recorder.BindComponentsOfType<Transform>(gameObject, true) ;
    }

    private void LateUpdate() // 레코딩은 모든 절차 끝나고 실행되는 레이트업데이트에서 하는게 안전
    {
        if (clips == null) { return; }
        cr.Aim();
        if (record)
        {
            m_Recorder.TakeSnapshot(Time.deltaTime);
        }
        else if (m_Recorder.isRecording)
        {
            m_Recorder.SaveToClip(clips);
            m_Recorder.ResetRecording();
        }

    }
    private void OnDisable()
    {
        if (clips == null) { return; }
        if (m_Recorder.isRecording)
        { m_Recorder.SaveToClip(clips); }
    }
}
