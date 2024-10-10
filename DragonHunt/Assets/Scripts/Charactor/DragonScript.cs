using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Misaki
{
    public partial class DragonScript : EnemyScript
    {
        /// --------関数一覧-------- ///

        #region public関数
        /// -------public関数------- ///

        public override void Dead()
        {
            onDragonDead.OnNext(Unit.Default);  // イベントを発行
            onDragonDead.OnCompleted();  // イベント終了

            base.Dead();
        }

        public override IEnumerator HPHitReaction()
        {
            // HPが50%を切ったら形態変更
            if (!isPhaseChange && parameter.hp < parameter.maxHp / 2)
            {
                isPhaseChange = true;
                onPhaseChange.OnNext(Unit.Default);  // イベントを発行
                onPhaseChange.OnCompleted();  // イベント終了
            }

            StartCoroutine(base.HPHitReaction());

            yield return null;
        }

        /// <summary>
        /// ボムを設置する関数
        /// </summary>
        public void BombsSetup()
        {
            int attempts = 0; // 試行回数
            int bombCount = 0; // ボム格納回数

            // ボムリストに3つボムが格納されるまでループ
            while (bombCount < bombMax)
            {
                // 無限ループ防止
                attempts++;
                if (attempts > 100) break;

                // ランダムな値を代入
                int randomIndex = UnityEngine.Random.Range(0, bombPos.Length);

                // 重複を避け、ボムリストに格納
                if (!bombHashSet.Contains(randomIndex))
                {
                    bombHashSet.Add(randomIndex);
                    bombs.Add(bombPos[randomIndex]);
                    bombCount++;
                }
            }

            // ボムリストの場所にエフェクトを生成
            foreach (GameObject pos in bombs)
            {
                GameObject bombEffect1 = GenerateEffect(EffectName.hpBomb1, pos);
                StartCoroutine(BombTimer(pos, bombEffect1));
            }

            // ボムリストをリセット
            bombs.Clear();
        }

        /// <summary>
        /// ボム生成処理をリセットする関数
        /// </summary>
        public void ResetBomb()
        {
            bombHashSet.Clear();
        }

        public IObservable<Unit> OnDragonDead => onDragonDead; // ドラゴンが戦闘不能になった際に監視者に通知
        public IObservable<Unit> OnPhaseChange => onPhaseChange; // ドラゴンが形態変更の際に監視者に通知

        /// -------public関数------- ///
        #endregion

        #region protected関数
        /// -----protected関数------ ///

        protected override void Start()
        {
            base.Start();
            InitializeBombPos();
            BeginSuperArmor();
        }

        /// -----protected関数------ ///
        #endregion

        #region private関数
        /// ------private関数------- ///

        /// <summary>
        /// ボム関係の初期化関数
        /// </summary>
        private void InitializeBombPos()
        {
            // ボムの場所を初期化
            bombPos = new GameObject[bombsField.transform.childCount];
            for (int i = 0; i < bombPos.Length; i++) bombPos[i] = bombsField.transform.GetChild(i).gameObject;
        }

        /// <summary>
        /// ボムを起爆させる関数
        /// </summary>
        private IEnumerator BombTimer(GameObject effectPos, GameObject bombEffect1)
        {
            // ボムを起爆するまで待ってからエフェクトを生成する
            yield return new WaitForSeconds(bombTime);
            EffectManager.effectGroups[(int)EffectName.hpBomb1].pool.ReleaseGameObject(bombEffect1);
            BeginHPBullet(EffectName.hpBomb2, effectPos);
        }

        /// ------private関数------- ///
        #endregion

        /// --------関数一覧-------- ///
    }
    public partial class DragonScript
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

        public bool isPhaseChange = false; // 形態変更したか

        [SerializeField] private int bombMax; // ボムをセットできる上限
        private HashSet<int> bombHashSet = new HashSet<int>(); // ボム抽選のハッシュセット

        [SerializeField] private float bombTime; // ボムが爆発するまでの時間

        private GameObject[] bombPos; // ボムの設置場所配列
        private List<GameObject> bombs = new List<GameObject>(); // ボムリスト
        [SerializeField] private GameObject bombsField; // ボムのフィールド

        private Subject<Unit> onDragonDead = new Subject<Unit>(); // 戦闘不能イベントのためのSubject
        private Subject<Unit> onPhaseChange = new Subject<Unit>(); // 形態変更イベントのためのSubject

        /// ------private変数------- ///
        #endregion

        #region プロパティ
        /// -------プロパティ------- ///



        /// -------プロパティ------- ///
        #endregion

        /// --------変数一覧-------- ///
    }
}