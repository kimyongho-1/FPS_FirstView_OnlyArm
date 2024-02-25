//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;

//[CustomEditor(typeof(SpineRotate))]
//public class BoneVisualizeEditor : Editor
//{
//    static List<SpineRotate> list = new List<SpineRotate>();
//    static bool showVisuals = true; // ��ü ����Ʈ �� ��� ��������

    
//    static BoneVisualizeEditor() // ������
//    {
//        SceneView.duringSceneGui += Draw;
//        EditorApplication.hierarchyChanged += OnHierarchyChanged;
//        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
//    }
//    static void OnHierarchyChanged() //���̾��Ű����ø��� ����Ʈ ����
//    {
//        list = new List<SpineRotate>(FindObjectsOfType<SpineRotate>());
//    }
//    static void OnPlayModeStateChanged(PlayModeStateChange state) // �÷��̸�� ���Խ�
//    {
//        // �÷��� ���� ��ȯ�ǰų� �÷��� ��忡�� ���� �� �ٽ� ����
//        if (state == PlayModeStateChange.EnteredPlayMode || state == PlayModeStateChange.ExitingPlayMode)
//        {
//            SceneView.duringSceneGui -= Draw;
//            SceneView.duringSceneGui += Draw;
//        }
//    }

//    public override void OnInspectorGUI() // ���信 �ƹ��� ���� SR��ü�� �־ �ν����͸� ���̴ٺ� (Ŭ����)��ü�� �ϳ�, �� ��ü�� �ν����Ϳ� �׸�
//    {
//        SpineRotate SR = (SpineRotate)target;
//        EditorGUILayout.BeginVertical();
//        // ����ڰ� ������ �ν����Ϳ��� ����� �� �ִ� ��ư �߰�
//        showVisuals = GUILayout.Toggle(showVisuals, "���� �ð�ȭ�ϱ�");

//        if (showVisuals)
//        {
//            EditorGUILayout.BeginHorizontal();
//            SR.drawBone = GUILayout.Toggle(SR.drawBone, "�� �׸���");
//            if (SR.drawBone)
//            {
//                SR.boneSize = GUILayout.HorizontalSlider(SR.boneSize, 0.1f, 1f);
//            }
//            EditorGUILayout.EndHorizontal(); 

//            EditorGUILayout.BeginHorizontal();
//            SR.drawNormal = GUILayout.Toggle(SR.drawNormal, "�븻 ǥ��");
//            if (SR.drawNormal)
//            {
//                SR.arrowSize = GUILayout.HorizontalSlider(SR.arrowSize, 0.1f, 1f);
//            }
//            EditorGUILayout.EndHorizontal();

//        }

//        EditorGUILayout.BeginHorizontal();
//        SR.drawTarget = GUILayout.Toggle(SR.drawTarget, "Ÿ�� ǥ��"); 
//        if (SR.drawTarget)
//        {
//            SR.targetSize = GUILayout.HorizontalSlider(SR.targetSize, 0.1f, 1f);
//        }
//        EditorGUILayout.EndHorizontal();

//        // �⺻ �ν����� GUI �׸���
//        base.OnInspectorGUI(); 
//        EditorGUILayout.EndVertical();
//    }

//    static void Draw(SceneView view) // ���信 �׸����� ��� ����Ʈ�� ��ȸ(�ϱ� �Լ��� �����Ӵ� �ѹ��� �����ϴϱ�)
//    {
//        if (!showVisuals) { return; }
//        for (int j = 0; j < list.Count; j++)
//        {
//            SpineRotate SR = list[j];
//            Transform[] upperBone = SR.GetUpperBody;

//            for (int i = 0; i < upperBone.Length; i++)
//            {
//                Handles.color = Color.red; // ���Ǿ��� ���� ����
              
//                // ��ü �׸��� : ���� �Ǻ� ��ġ ǥ��   
//                if (SR.drawBone)
//                { Handles.SphereHandleCap(0, upperBone[i].position, Quaternion.identity, SR.boneSize, EventType.Repaint); }

//                // ȭ��ǥ �׸��� : ������. ���� ǥ��
//                if (SR.drawNormal)
//                {
//                    Handles.ArrowHandleCap(0, upperBone[i].position, Quaternion.LookRotation(upperBone[i].forward), SR.arrowSize, EventType.Repaint);
//                }

//                Handles.color = Color.blue;
//                if (SR.drawTarget) { Handles.CubeHandleCap(0, SR.GetReceiver.LookTarget.position, Quaternion.identity, SR.targetSize, EventType.Repaint); }
//                Handles.color = Color.red;
//            }
//        }
//    }
//    private void OnSceneGUI()
//    {
        
//    }
//}

