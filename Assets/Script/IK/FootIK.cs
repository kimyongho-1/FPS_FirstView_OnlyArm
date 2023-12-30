using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class FootIK : MonoBehaviour
{
    //public IKhandling handle;
    #region Variable

    /// <summary>
    /// �� ���� : Foot�� ��� ��|����|���(��ü���� �ְ�θ� LeafBone)
    /// </summary>
    public Transform[] bones; 
    /// <summary>
    /// ����������, foot�� ��ġ�ؾ��� ����� ��ġ�迭
    /// </summary>
    public Vector3[] positions;
    public Vector3[] startDir;
    Quaternion[] startRotationBone;
    Quaternion startRotationTarget;
    Quaternion startRotationRoot;
    /// <summary>
    /// �� �������� ���� ���� (������ �߻����� ���̴� �������ʾƾ��Ѵ�)
    /// </summary>
    public float[] bonesLength;

    /// <summary>
    /// Fabrik �˰��򿡼�, ����+�θ������ ��ġ�� ����� �ݺ��� Ƚ�� (������ ��Ȯ�� ����)
    /// </summary>
    public int iterations;
    /// <summary>
    /// ���� ��ġ�� ��ǥ����
    /// </summary>
    public Transform Target;
    public Transform Pole;
    float totalBonesLength = 0;// �� �������� �� ������ (��ٿ��� �߱����� �� ��ü����)
    public float leftWeight=1f;
    #endregion

    private void Awake()
    {
        bones = new Transform[3];  // ��->����->��� �� 3��
        positions = new Vector3[3]; // �� ���� ������ġ�� ���� ��ġ���̱⿡, ���� ���� �����ϰ�
        bonesLength = new float[2]; // ��~���� + ����~���  �� 2���� ������
        startDir = new Vector3[3];
        startRotationBone = new Quaternion[3];
        startRotationTarget = Target.rotation;
        // ��ũ��Ʈ�� ����� ��TRANSFORM�������� ��� ã��
       Transform bone = transform;
       startDir[2] = Target.position - bone.position;
       for (int i = bones.Length - 1; i >= 0; i--)
       {
            bones[i] = bone;
            startRotationBone[i] = bone.rotation;
            if (i != bones.Length - 1)
            {
                startDir[i] = bones[i + 1].position - bones[i].position;
                bonesLength[i] = (bones[i + 1].position - bones[i].position).magnitude;
                totalBonesLength += bonesLength[i];
            }
            
            bone = bone.parent;
        }

    }

    /// <summary>
    /// Update���� ���� ȸ��+��ġ���� �����
    /// �ִϸ��̼� ���°� ������Ʈ �ɋ�����, 
    /// ���� ������ ��ġȸ������ ���� �ִϸ��̼ǿ� ���� ����� �������̵� �Ǵ� ���� �߻�
    /// ���� LateUpdate�� �ۼ� �� ����� �����۵� Ȯ��
    /// Update-OnAnimatorIK-LateUpdate ����
    /// </summary>
    public void DoHandIK()
    {
        // to do : weight�� 0�̾ ����
        if (Target == null) { return; }
        if (leftWeight <= 0) { return; }
        // if (Physics.Raycast(rayorigin.position, Vector3.up * -0.5f, out RaycastHit hit, 1f)) {  Target.position = hit.point; }



        // ���� ��ٷ� �̵��� �ƴ�, �����ϴ� ������� ���
        for (int i = 0; i < positions.Length; i++)
        { positions[i] = bones[i].position; }

        //var RootRot = (bones[0].parent != null) ? bones[0].parent.rotation : Quaternion.identity;
        //var RootRotDiff = RootRot * Quaternion.Inverse(startRotationRoot);

        // ��ݿ��� ���� ��ġ������ �Ÿ��� ��ü�� �ѱ��̺��� ũ�ٸ�
        // Ÿ�� �������� ������Ը� ��ġ ������ ������
        if ((Target.position - bones[0].position).sqrMagnitude >= totalBonesLength * totalBonesLength)
        {
            Vector3 dir = (Target.position - positions[0]).normalized;
            // �ְ�θ� ��ü����� ������ �ʿ� X
            for (int i = 1; i < bones.Length; i++)
            {
                #region ����
                // �ڽ��� �θ���ġ����, Ÿ�ٹ������� �̵��� ��ġ������ ����
                // EX : ������ ���, ��ݿ��� Ÿ�ٹ��������� ���⺤�͸� ���ϱ�
                // ���⺤�� * (��ݿ��� ���������� �� ����) �� �� ��ġ���� ���ϱ�
                // ��ݿ��� ���� ���⺤�͸� ���� ���Ͱ� ������ ��ġ�ؾ��� ��ġ����
                #endregion
                positions[i] = dir * bonesLength[i - 1] + positions[i - 1];
            }

        }

        // Ÿ���� ��ü�Ǳ��� ������ ������ ( ik Ư���� ���η��� ���� ��� + ��Ȯ�� ��ġ��� �ʿ�)
        else
        {
            #region �ѹ��� �ƴ� iterations��ŭ ��ġ����� �ݺ��ϴ� ����
            // FARBIK �˰����� ���,
            // Backward -> Forward ������ ��ġ����� �ϴµ�
            // ����(�߿��� �������)����� ���� �� ���븦 ��ǥ(Target)�� ������ �̵�
            // ������(��ݿ��� ��)����� ���� ������ �����Ͽ�, ���Ȼ� ��Ȯ�ϰ� ���

            // ���⼭ iterations�� �����Ҽ��� ��ǥ��ġ�� ��Ȯ������
            // �㳪 �ݺ����� ���ϰ� ������ �ʿ䰡 ���� ��� (�̹� ��Ȯ�� ��ġ�� �ִ���)
            // �ݺ��� ���� ����ó���� �Ͽ� ���� �ݺ����� ������������ ����
            #endregion
            // iterations��ŭ ���� ��ġ�� Vec3�� �ݺ� ���
            for (int it = 0; it < iterations; it++)
            {
                // Back�������  �� -> ���� (�������� ��ġ ��� ���ϱ�)
                for (int i = bones.Length-1; i > 0 ; i--)
                {
                    // ���� ���, Ÿ���� ��ü�������̱⿡, Ÿ�� ��ġ�� �����ϰ� ������
                    if (i == bones.Length - 1)
                    {
                        positions[i] = Target.position;
                        continue;
                    }

                    // �߿��� �������� ���ϴ� ���������� �켱 ��ġ��Ű��
                    positions[i] = positions[i+1] +  // ������ġ�� �ڽĻ� ��ġ����
                        (positions[i] - positions[i+1]).normalized * bonesLength[i]; // �ڽĻ����� �ڽ����� ���ϴ� ���⺤�� * ������
                }

                // ������ ���, �������⿡�� ���� ��ü�������� ��ġ��� (0���� �ε��� ������� �������� �ȵǱ⿡ ����)
                for (int i = 1; i < bones.Length; i++)
                {
                    // �ڽ��� �θ���ġ����, �θ𿡼� ���� ���ϴ� ���⺤�� * �� ����
                    // �� ���� ����� ���� ���� ��ġ�ؾ��� ��ġ����
                    positions[i] = positions[i-1] + // �θ���ġ����
                        (positions[i]- positions[i - 1]).normalized * bonesLength[i-1]; // �θ𿡼��ڽ����� ���ϴ� ����
                }

                // iterations��ŭ ���������, �̹� ��ġ�� ��Ȯ�ҽ�
                // ���� ��� ���ϰ� �ݺ��� ����������
                if ((bones[bones.Length - 1].position - Target.position).sqrMagnitude
                    <= Mathf.Epsilon)
                {
                    break;
                }
            }

        }

        if (Pole != null)
        {
            for (int i = 1; i < bones.Length-1; i++)
            {
                // �߿��� ������� ���ϴ� ������ ��ֺ��ͷ��ϴ� ������� ���� + ����� ��ġ�� ��������� ����ϵ���
                var plane = new Plane((positions[i + 1] - positions[i - 1]), positions[i-1]);

                // ���� Pole��ġ���� ����������� ������ �̷�鼭 ���� ����� ��ġ���ϱ�
                var projectedPole = plane.ClosestPointOnPlane(Pole.position);
                // ������ġ���� ����������� ������ �̷�鼭 ���� ����� ��ġ���ϱ�
                var projectedBone = plane.ClosestPointOnPlane(positions[i]);

                // ��ݿ��� �����Ͽ� ������鿡�� ��ġ�� �� ��������� ���� ���ϱ�
                var angle = Vector3.SignedAngle(projectedBone - positions[i-1]
                    , projectedPole - positions[i-1], plane.normal);

                // ��ġ���� ȸ���� ���Ͽ� ��ȯ�� ��ġ������ �ʱ�ȭ
                positions[i] = Quaternion.AngleAxis(angle, plane.normal)
                    * (positions[i] - positions[i - 1]) + positions[i - 1];
            }
       
        }


        for (int i = 0; i < positions.Length; i++)
        {
            //��
            if (i == positions.Length - 1)
            {
                // ���� ȸ�� ���� ���, ikŸ���� ȸ���� �����ϰ� �����ϴµ�
                // ���� Play����� �ʱ�ȭ�� Ÿ���� �⺻ȸ���� startRotationTarget������ŭ
                // ���� Ÿ�� ȸ���� ���ش� = ���� ȸ������ �����ԵǴµ�
                // �ٽ� �ѹ��� ���� �ڽ��� �⺻ ȸ������ �����ش�.
                bones[i].rotation = Target.rotation;
                bones[i].position = Target.position;
                /*/
                    Target.rotation // ���� Ÿ���� ȸ��������
                    * Quaternion.Inverse(startRotationTarget) // Ÿ���� ����ȸ���� ���� (���� ȸ������ ���ϱ�����)
                    * startRotationBone[i]; // �׸��� ĳ���� ������ �ʱ� ȸ���� �ٽ� ���ϱ� (�ʱ� �ڼ��� �������� ȸ����Ű�� ����)
                /*/
            }
            else
            {
                // startDir�� ���۽� �ʱ�ȭ�� �⺻ ���⺤���̸�
                // �⺻���⿡�� , �θ𿡼� �ڽ����� ���ϴ� ������ ���� ���⺤�ͷ� ���ϴ� ȸ���� ����
                // ���� �ڽ��� �ʱ� ����ȸ������ŭ �����ϵ��� �ٽ� ���Ѱ��� ȸ������ �Է�
                bones[i].rotation =
                    Quaternion.FromToRotation(startDir[i],  // ���۹��⺤�Ϳ��� (��ݿ��� ���� �Ǵ� �������� �߷� ���ϴ� ���⺤��)
                    positions[i + 1] - positions[i])  // ���� ���⺤�͸�ŭ ȸ������ ȸ���� ���ϱ� (POSITION[i+1]���� i�� �����Ҽ��� �ڽĻ��� )
                    * startRotationBone[i]; // �׸��� ĳ���� ������ �ʱ� ȸ���� �ٽ� ���ϱ� (�ʱ� �ڼ��� �������� ȸ����Ű�� ����)
                bones[i].position = positions[i];
            }

            
            //bones[i].position= Vector3.Lerp(bones[i].position, positions[i], handle.GetLeftFootWeight);
        }

    }

    //public Transform rayorigin;
    private void OnDrawGizmos()
    {
        var current = this.transform;
        for (int i = 0; i < bones.Length-1 && current != null && current.parent != null; i++)
        {
            var scale = Vector3.Distance(current.position, current.parent.position) * 0.1f;
            UnityEditor.Handles.matrix = Matrix4x4.TRS(current.position,
                Quaternion.FromToRotation(Vector3.up, current.parent.position - current.position)
                , new Vector3(scale, Vector3.Distance(current.parent.position, current.position), scale));
            UnityEditor.Handles.color = Color.green;
            UnityEditor.Handles.DrawWireCube(Vector3.up * 0.5f, Vector3.one);
            current = current.parent;
        }
      
    }
}
