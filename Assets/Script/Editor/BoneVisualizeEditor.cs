//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;

//[CustomEditor(typeof(SpineRotate))]
//public class BoneVisualizeEditor : Editor
//{
//    static List<SpineRotate> list = new List<SpineRotate>();
//    static bool showVisuals = true; // 전체 리스트 본 출력 끌지말지

    
//    static BoneVisualizeEditor() // 생성자
//    {
//        SceneView.duringSceneGui += Draw;
//        EditorApplication.hierarchyChanged += OnHierarchyChanged;
//        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
//    }
//    static void OnHierarchyChanged() //하이어아키변경시마다 리스트 갱신
//    {
//        list = new List<SpineRotate>(FindObjectsOfType<SpineRotate>());
//    }
//    static void OnPlayModeStateChanged(PlayModeStateChange state) // 플레이모드 진입시
//    {
//        // 플레이 모드로 전환되거나 플레이 모드에서 나올 때 다시 설정
//        if (state == PlayModeStateChange.EnteredPlayMode || state == PlayModeStateChange.ExitingPlayMode)
//        {
//            SceneView.duringSceneGui -= Draw;
//            SceneView.duringSceneGui += Draw;
//        }
//    }

//    public override void OnInspectorGUI() // 씬뷰에 아무리 많은 SR객체가 있어도 인스펙터를 들이다볼 (클릭한)객체는 하나, 그 객체의 인스펙터에 그림
//    {
//        SpineRotate SR = (SpineRotate)target;
//        EditorGUILayout.BeginVertical();
//        // 사용자가 에디터 인스펙터에서 토글할 수 있는 버튼 추가
//        showVisuals = GUILayout.Toggle(showVisuals, "뼈대 시각화하기");

//        if (showVisuals)
//        {
//            EditorGUILayout.BeginHorizontal();
//            SR.drawBone = GUILayout.Toggle(SR.drawBone, "뼈 그리기");
//            if (SR.drawBone)
//            {
//                SR.boneSize = GUILayout.HorizontalSlider(SR.boneSize, 0.1f, 1f);
//            }
//            EditorGUILayout.EndHorizontal(); 

//            EditorGUILayout.BeginHorizontal();
//            SR.drawNormal = GUILayout.Toggle(SR.drawNormal, "노말 표시");
//            if (SR.drawNormal)
//            {
//                SR.arrowSize = GUILayout.HorizontalSlider(SR.arrowSize, 0.1f, 1f);
//            }
//            EditorGUILayout.EndHorizontal();

//        }

//        EditorGUILayout.BeginHorizontal();
//        SR.drawTarget = GUILayout.Toggle(SR.drawTarget, "타겟 표시"); 
//        if (SR.drawTarget)
//        {
//            SR.targetSize = GUILayout.HorizontalSlider(SR.targetSize, 0.1f, 1f);
//        }
//        EditorGUILayout.EndHorizontal();

//        // 기본 인스펙터 GUI 그리기
//        base.OnInspectorGUI(); 
//        EditorGUILayout.EndVertical();
//    }

//    static void Draw(SceneView view) // 씬뷰에 그릴떄는 모든 리스트를 순회(하기 함수는 프레임당 한번만 실행하니까)
//    {
//        if (!showVisuals) { return; }
//        for (int j = 0; j < list.Count; j++)
//        {
//            SpineRotate SR = list[j];
//            Transform[] upperBone = SR.GetUpperBody;

//            for (int i = 0; i < upperBone.Length; i++)
//            {
//                Handles.color = Color.red; // 스피어의 색상 설정
              
//                // 구체 그리기 : 본과 피봇 위치 표시   
//                if (SR.drawBone)
//                { Handles.SphereHandleCap(0, upperBone[i].position, Quaternion.identity, SR.boneSize, EventType.Repaint); }

//                // 화살표 그리기 : 포워드. 방향 표시
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

