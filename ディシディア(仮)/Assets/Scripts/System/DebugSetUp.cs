using UnityEngine;

// ���쐬�̃f�o�b�O���O��\�����邩�ǂ��������߂�X�N���v�g�ł�
// ���̃X�N���v�g���p������΁A�p�������X�N���v�g�ɋL�ڂ����f�o�b�O���O��\���̗L����Project��Őݒ�ł��܂�
// Project���ScriptableObjects/GameSettings�̃`�F�b�N�̗L���Ńf�o�b�O���O��\�����邩���Ȃ��������܂�܂�
public partial class DebugSetUp : MonoBehaviour
{
    /// --------�֐��ꗗ-------- ///
    /// -------public�֐�------- ///



    /// -------public�֐�------- ///
    /// -----protected�֐�------ ///
    
    protected virtual void Awake()
    {
        // �X�N���v�^�u���I�u�W�F�N�g��bool�ɂ����
        // �f�o�b�O���O��\�����邩�ǂ����𔻒f����
        // true��...�\�� false��...��\��
        Debug.unityLogger.logEnabled = debugSettings.debugLogEnabled;
    }


    /// -----protected�֐�------ ///
    /// ------private�֐�------- ///



    /// ------private�֐�------- ///
    /// --------�֐��ꗗ-------- ///
}
public partial class DebugSetUp
{
    /// --------�ϐ��ꗗ-------- ///
    /// -------public�ϐ�------- ///



    /// -------public�ϐ�------- ///
    /// -----protected�ϐ�------ ///



    /// -----protected�ϐ�------ ///
    /// ------private�ϐ�------- ///
    
    [SerializeField] private DebugSettings debugSettings;    //�Q�[���̐ݒ�f�[�^


    /// ------private�ϐ�------- ///
    /// -------�v���p�e�B------- ///



    /// -------�v���p�e�B------- ///
    /// --------�ϐ��ꗗ-------- ///
}
