using UnityEngine;
//ゲームの設定用ScriptableObject
[CreateAssetMenu(menuName = "ScriptableObject/DebugSettings", fileName = "DebugSettings")]
public class DebugSettings : ScriptableObject
{
    /// --------変数一覧-------- ///
    /// -------public変数------- /// 

    [Header("デバッグログを表示したい場合はチェックをつける")]
    public bool debugLogEnabled; //デバッグログを表示するか


    /// -------public変数------- ///
    /// -----protected変数------ ///



    /// -----protected変数------ ///
    /// ------private変数------- ///



    /// ------private変数------- ///
    /// -------プロパティ------- ///



    /// -------プロパティ------- ///
    /// --------変数一覧-------- ///

}