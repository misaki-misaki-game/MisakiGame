using UnityEngine;

/// <summary>
/// MonoBehaviour�ɑΉ������V���O���g���N���X
/// �i��jpublic class GameManager : SingletonMonoBehaviour<GameManager>
/// </summary>
public partial class SingletonMonoBehaviour<T> : DebugSetUp where T : MonoBehaviour
{
    /// --------�֐��ꗗ-------- ///
    /// -------public�֐�------- ///

    /// <summary>�C���X�^���X���擾���܂�</summary>
    public static T Instance
    {
        get
        {
            // instance��null�Ȃ�T�N���X���q�G�����L�[�ォ��T���@������΃G���[
            instance ??= (T)FindObjectOfType(typeof(T));
            Debug.Assert(instance == null, typeof(T) + "���A�^�b�`���Ă���GameObject������܂���");
            
            return instance; // �T����instance��Ԃ�
        }
    }

    /// -------public�֐�------- ///
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
        // ���̃I�u�W�F�N�g���V�[�����Ă������Ȃ��悤�ɂ���
        DontDestroyOnLoad(this.gameObject);
    }

    /// -----protected�֐�------ ///
    /// ------private�֐�------- ///



    /// ------private�֐�------- ///
    /// --------�֐��ꗗ-------- ///
}
public partial class SingletonMonoBehaviour<T>
{
    /// --------�ϐ��ꗗ-------- ///
    /// -------public�ϐ�------- ///



    /// -------public�ϐ�------- ///
    /// -----protected�ϐ�------ ///



    /// -----protected�ϐ�------ ///
    /// ------private�ϐ�------- ///

    private static T instance; // �C���X�^���X�ϐ�

    /// ------private�ϐ�------- ///
    /// -------�v���p�e�B------- ///



    /// -------�v���p�e�B------- ///
    /// --------�ϐ��ꗗ-------- ///
}
