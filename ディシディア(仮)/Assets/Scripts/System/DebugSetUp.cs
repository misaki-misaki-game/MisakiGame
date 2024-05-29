using UnityEngine;

// 岬作成のデバッグログを表示するかどうかを決めるスクリプトです
// このスクリプトを継承すれば、継承したスクリプトに記載したデバッグログを表示の有無をProject上で設定できます
// Project上のScriptableObjects/GameSettingsのチェックの有無でデバッグログを表示するかしないかが決まります
public partial class DebugSetUp : MonoBehaviour
{
    /// --------関数一覧-------- ///
    /// -------public関数------- ///



    /// -------public関数------- ///
    /// -----protected関数------ ///
    
    protected virtual void Awake()
    {
        // スクリプタブルオブジェクトのboolによって
        // デバッグログを表示するかどうかを判断する
        // true時...表示 false時...非表示
        Debug.unityLogger.logEnabled = debugSettings.debugLogEnabled;
    }


    /// -----protected関数------ ///
    /// ------private関数------- ///



    /// ------private関数------- ///
    /// --------関数一覧-------- ///
}
public partial class DebugSetUp
{
    /// --------変数一覧-------- ///
    /// -------public変数------- ///



    /// -------public変数------- ///
    /// -----protected変数------ ///



    /// -----protected変数------ ///
    /// ------private変数------- ///
    
    [SerializeField] private DebugSettings debugSettings;    //ゲームの設定データ


    /// ------private変数------- ///
    /// -------プロパティ------- ///



    /// -------プロパティ------- ///
    /// --------変数一覧-------- ///
}
