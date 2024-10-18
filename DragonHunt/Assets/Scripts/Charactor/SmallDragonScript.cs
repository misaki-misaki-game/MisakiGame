using System.Collections;

namespace Misaki
{
    public partial class SmallDragonScript : EnemyScript
    {
        /// --------関数一覧-------- ///

        #region public関数
        /// -------public関数------- ///

        public override void Born()
        {
            // 召喚エフェクトとSEを流す
            GenerateEffectNoneParent(EffectName.summon, gameObject);
            SEPlay(SEList.E_BornSE); // SEを流す
        }

        public override IEnumerator BraveHitReaction()
        {
            // 被ダメージ状態がスーパーアーマーでなければ小怯みアニメーションを再生
            if (damageState != DamageState.E_SuperArmor)
            {
                SmallHitReaction(0);
            }

            StartCoroutine(base.BraveHitReaction());

            yield return null;
        }

        /// -------public関数------- ///
        #endregion

        #region protected関数
        /// -----protected関数------ ///

        protected override void Start()
        {
            base.Start();

            Born();
        }

        /// -----protected関数------ ///
        #endregion

        #region private関数
        /// ------private関数------- ///



        /// ------private関数------- ///
        #endregion

        /// --------関数一覧-------- ///
    }
    public partial class SmallDragonScript
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



        /// ------private変数------- ///
        #endregion

        #region プロパティ
        /// -------プロパティ------- ///



        /// -------プロパティ------- ///
        #endregion

        /// --------変数一覧-------- ///
    }
}