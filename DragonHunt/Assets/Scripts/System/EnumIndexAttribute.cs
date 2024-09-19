using System;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Misaki
{
    public partial class EnumIndexAttribute : PropertyAttribute
    {
        /// --------関数一覧-------- ///

        #region public関数
        /// -------public関数------- ///

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="enumType">エレメントに付けたい名前のenum</param>
        public EnumIndexAttribute(Type enumType) => _names = Enum.GetNames(enumType);

        /// -------public関数------- ///
        #endregion

        #region protected関数
        /// -----protected関数------ ///



        /// -----protected関数------ ///
        #endregion

        #region private関数
        /// ------private関数------- ///

#if UNITY_EDITOR
        /// <summary>
        /// インスペクターに表示
        /// </summary>
        [CustomPropertyDrawer(typeof(EnumIndexAttribute))]
        private class Drawer : PropertyDrawer
        {
            public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
            {
                var names = ((EnumIndexAttribute)attribute)._names;
                // propertyPath returns something like hogehoge.Array.data[0]
                // so get the index from there.
                // propertyPath は "hogehoge.Array.data[0]" のようなものを返す
                // そのため、そこからインデックスを取得する。
                var index = int.Parse(property.propertyPath.Split('[', ']').Where(c => !string.IsNullOrEmpty(c)).Last());
                if (index < names.Length) label.text = names[index];
                EditorGUI.PropertyField(rect, property, label, includeChildren: true);
            }

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                return EditorGUI.GetPropertyHeight(property, label, includeChildren: true);
            }
        }
#endif

        /// ------private関数------- ///
        #endregion

        /// --------関数一覧-------- ///
    }
    public partial class EnumIndexAttribute
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

        private string[] _names;

        /// ------private変数------- ///
        #endregion

        #region プロパティ
        /// -------プロパティ------- ///



        /// -------プロパティ------- ///
        #endregion

        /// --------変数一覧-------- ///
    }
}