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

        // ��ȭ�� ��ü�� ��� �ڽĵ���� ���Խ�Ű��
        m_Recorder.BindComponentsOfType<Transform>(gameObject, true) ;
    }

    private void LateUpdate() // ���ڵ��� ��� ���� ������ ����Ǵ� ����Ʈ������Ʈ���� �ϴ°� ����
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
