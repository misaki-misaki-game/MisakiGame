using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using System;

namespace Misaki
{
    public static partial class TagEnumGenerator
    {
        /// --------関数一覧-------- ///

        #region public関数
        /// -------public関数------- ///

        // タグのEnumを生成及び更新する関数(自動または手動可)
        [MenuItem("Tools/Update Tag Enum")]
        public static void GenerateEnum()
        {
            // タグの要素数とEnum.Tagsの要素数を比較して同じなら更新しない
            int f = UnityEditorInternal.InternalEditorUtility.tags.Length;
            int j = Enum.GetValues(typeof(Tags)).Length;
            if (f == j) return;

            Debug.Log("Tagsを更新します");

            // タグの中身を全て取得する
            string[] tags = UnityEditorInternal.InternalEditorUtility.tags;

            // 取得した中身をenumの形に変更して新たなスクリプトを生成または上書きする
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("public enum " + enumName); // enumの名称
                // enumの中身
                writer.WriteLine("{");
                for (int i = 0; i < tags.Length; i++)
                {
                    writer.WriteLine("    " + tags[i].Replace(" ", "_") + " = " + i + ",");
                }
                writer.WriteLine("}");
            }

            // 同期・更新する
            AssetDatabase.Refresh();
        }

        /// -------public関数------- ///
        #endregion

        #region protected関数
        /// -----protected関数------ ///



        /// -----protected関数------ ///
        #endregion

        #region private関数
        /// ------private関数------- ///

        // プロジェクトロード時に動く関数
        [InitializeOnLoadMethod]
        private static void OnProjectLoadedInEditor()
        {
            GenerateEnum();
        }

        // スクリプトリロード時に動く関数
        [DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            GenerateEnum();
        }

        /// ------private関数------- ///
        #endregion

        /// --------関数一覧-------- ///
    }
    public partial class TagEnumGenerator
    {
        /// --------変数一覧-------- ///

        #region public変数
        /// -------public変数------- ///



        /// -------public変数------- ///
        #endregion

        #region protected変数
        /// -----protected変数------ ///



        /// -----protected変数------ ///
        #endregion

        #region private変数
        /// ------private変数------- ///

        private const string enumName = "Tags"; // 生成するenumの名称
        private const string filePath = "Assets/Scripts/System/EnumTags.cs"; // 生成するスクリプトの名称及び指定場所

        /// ------private変数------- ///
        #endregion

        #region プロパティ
        /// -------プロパティ------- ///



        /// -------プロパティ------- ///
        #endregion

        /// --------変数一覧-------- ///
    }
}