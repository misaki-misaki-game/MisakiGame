using UnityEngine;

namespace Misaki
{
    // ���쐬�̃f�o�b�O���O��\�����邩�ǂ��������߂�X�N���v�g�ł�
    // ���̃X�N���v�g���p������΁A�p�������X�N���v�g�ɋL�ڂ����f�o�b�O���O��\���̗L����Project��Őݒ�ł��܂�
    // Project���ScriptableObjects/GameSettings�̃`�F�b�N�̗L���Ńf�o�b�O���O��\�����邩���Ȃ��������܂�܂�
    public partial class DebugSetUp : MonoBehaviour
    {
        /// --------�֐��ꗗ-------- ///

        #region public�֐�
        /// -------public�֐�------- ///



        /// -------public�֐�------- ///
        #endregion

        #region protected�֐�
        /// -----protected�֐�------ ///

        protected virtual void Awake()
        {
            // �X�N���v�^�u���I�u�W�F�N�g��bool�ɂ����
            // �f�o�b�O���O��\�����邩�ǂ����𔻒f����
            // true��...�\�� false��...��\��
            Debug.unityLogger.logEnabled = debugSettings.debugLogEnabled;
        }

        /// -----protected�֐�------ ///
        #endregion

        #region private�֐�
        /// ------private�֐�------- ///



        /// ------private�֐�------- ///
        #endregion

        /// --------�֐��ꗗ-------- ///
    }
    public partial class DebugSetUp
    {
        /// --------�ϐ��ꗗ-------- ///

        #region public�ϐ�
        /// -------public�ϐ�------- ///



        /// -------public�ϐ�------- ///
        #endregion

        #region protected�ϐ�
        /// -----protected�ϐ�------ ///



        /// -----protected�ϐ�------ ///
        #endregion

        #region private�ϐ�
        /// ------private�ϐ�------- ///

        [SerializeField] private DebugSettings debugSettings;    //�Q�[���̐ݒ�f�[�^

        /// ------private�ϐ�------- ///
        #endregion

        #region �v���p�e�B
        /// -------�v���p�e�B------- ///



        /// -------�v���p�e�B------- ///
        #endregion

        /// --------�ϐ��ꗗ-------- ///
    }
}