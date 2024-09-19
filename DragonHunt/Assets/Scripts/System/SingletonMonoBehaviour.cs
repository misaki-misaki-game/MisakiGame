using UnityEngine;

namespace Misaki
{
    /// <summary>
    /// MonoBehaviour�ɑΉ������V���O���g���N���X
    /// �i��jpublic class GameManager : SingletonMonoBehaviour<GameManager>
    /// </summary>
    public partial class SingletonMonoBehaviour<T> : DebugSetUp where T : MonoBehaviour
    {
        /// --------�֐��ꗗ-------- ///

        #region public�֐�
        /// -------public�֐�------- ///

        /// <summary>�C���X�^���X���擾���܂�</summary>
        public static T Instance
        {
            get
            {
                // instance��null�Ȃ�T�N���X���q�G�����L�[�ォ��T���@������΃G���[
                if (instance == null)
                {
                    instance = (T)FindObjectOfType(typeof(T));
                    Debug.Assert(instance != null, typeof(T) + "���A�^�b�`���Ă���GameObject������܂���");
                }
                return instance;
            }
        }

        /// -------public�֐�------- ///
        #endregion

        #region protected�֐�
        /// -----protected�֐�------ ///

        protected override void Awake()
        {
            base.Awake();
            // ����GameObject�ɃA�^�b�`����Ă��邩���ׂ�
            if (this != Instance)
            {
                // �A�^�b�`����Ă���ꍇ�͔j������
                Debug.LogWarning("����" + typeof(T) + "������̂ŃI�u�W�F�N�g���j������܂�");
                Destroy(this.gameObject);
                return;
            }
            // true�Ȃ炱�̃I�u�W�F�N�g���V�[�����Ă������Ȃ��悤�ɂ���
            if(isDontDestroy) DontDestroyOnLoad(this.gameObject);
        }

        /// -----protected�֐�------ ///
        #endregion

        #region private�֐�
        /// ------private�֐�------- ///



        /// ------private�֐�------- ///
        #endregion

        /// --------�֐��ꗗ-------- ///
    }
    public partial class SingletonMonoBehaviour<T>
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

        private static T instance; // �C���X�^���X�ϐ�

        [SerializeField] bool isDontDestroy; // DontDestroy�ɐݒ肷�邩�ǂ���

        /// ------private�ϐ�------- ///
        #endregion

        #region �v���p�e�B
        /// -------�v���p�e�B------- ///



        /// -------�v���p�e�B------- ///
        #endregion

        /// --------�ϐ��ꗗ-------- ///
    }
}