using UnityEngine;

/// <summary>
/// MonoBehaviourに対応したシングルトンクラス
/// （例）public class GameManager : SingletonMonoBehaviour<GameManager>
/// </summary>
public partial class SingletonMonoBehaviour<T> : DebugSetUp where T : MonoBehaviour
{
    /// --------関数一覧-------- ///
    /// -------public関数------- ///

    /// <summary>インスタンスを取得します</summary>
    public static T Instance
    {
        get
        {
            m_instance ??= (T)FindObjectOfType(typeof(T));
            Debug.Assert(m_instance == null, typeof(T) + "をアタッチしているGameObjectがありません");
            return m_instance;
        }
    }

    /// -------public関数------- ///
    /// -----protected関数------ ///

    protected override void Awake()
    {
        base.Awake();
        // 他のGameObjectにアタッチされているか調べる
        if (this != Instance)
        {
            // アタッチされている場合は破棄する
            Debug.LogWarning("既に" + typeof(T) + "があるのでオブジェクトが破棄されます");
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    /// -----protected関数------ ///
    /// ------private関数------- ///



    /// ------private関数------- ///
    /// --------関数一覧-------- ///
}
public partial class SingletonMonoBehaviour<T>
{
    /// --------変数一覧-------- ///
    /// -------public変数------- ///



    /// -------public変数------- ///
    /// -----protected変数------ ///



    /// -----protected変数------ ///
    /// ------private変数------- ///

    private static T m_instance;

    /// ------private変数------- ///
    /// -------プロパティ------- ///



    /// -------プロパティ------- ///
    /// --------変数一覧-------- ///
}
