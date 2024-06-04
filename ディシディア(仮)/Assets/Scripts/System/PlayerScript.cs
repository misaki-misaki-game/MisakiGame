using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public partial class PlayerScript : MonoBehaviour
{
    /// --------関数一覧-------- ///
    /// -------public関数------- ///



    /// -------public関数------- ///
    /// -----protected関数------ ///



    /// -----protected関数------ ///
    /// ------private関数------- ///

    void Start()
    {
        // コンポーネントを取得
        con = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

        // マウスを固定する気はない
        // マウスカーソルを非表示にし、位置を固定
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        key = Keyboard.current; // 現在のキーボード情報を取得

        startPos = transform.position; // 初期位置を取得
    }

    void Update()
    {
        // キーボード接続チェック
        if (key == null)
        {
            // キーボードが接続されていないと
            // Keyboard.currentがnullになる
            return;
        }

        // Aキーの入力状態取得
        var aKey = key.aKey;

        // Aキーが押された瞬間かどうか
        if (aKey.wasPressedThisFrame)
        {
            Debug.Log("Aキーが押された！");
        }

        // Aキーが離された瞬間かどうか
        if (aKey.wasReleasedThisFrame)
        {
            Debug.Log("Aキーが離された！");
        }

        // Aキーが押されているかどうか
        if (aKey.isPressed)
        {
            Debug.Log("Aキーが押されている！");
        }
        // 移動速度を取得
        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : normalSpeed;

        // カメラの向きを基準にした正面方向のベクトル
        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;

        // 前後左右の入力（WASDキー）から、移動のためのベクトルを計算
        // Input.GetAxis("Vertical") は前後（WSキー）の入力値
        // Input.GetAxis("Horizontal") は左右（ADキー）の入力値
        Vector3 moveZ = cameraForward * Input.GetAxis("Vertical") * speed;  //　前後（カメラ基準）　 
        Vector3 moveX = Camera.main.transform.right * Input.GetAxis("Horizontal") * speed; // 左右（カメラ基準）

        // isGrounded は地面にいるかどうかを判定します
        // 地面にいるときはジャンプを可能に
        if (con.isGrounded)
        {
            moveDirection = moveZ + moveX;
            if (Input.GetButtonDown("Jump"))
            {
                moveDirection.y = jump;
            }
        }
        else
        {
            // 重力を効かせる
            moveDirection = moveZ + moveX + new Vector3(0, moveDirection.y, 0);
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // 移動のアニメーション
        anim.SetFloat("MoveSpeed", (moveZ + moveX).magnitude);

        // プレイヤーの向きを入力の向きに変更　
        transform.LookAt(transform.position + moveZ + moveX);

        // Move は指定したベクトルだけ移動させる命令
        con.Move(moveDirection * Time.deltaTime);
    }

    /// ------private関数------- ///
    /// --------関数一覧-------- ///
}
public partial class PlayerScript
{
    /// --------変数一覧-------- ///
    /// -------public変数------- ///



    /// -------public変数------- ///
    /// -----protected変数------ ///



    /// -----protected変数------ ///
    /// ------private変数------- ///

    CharacterController con; // CharacterController変数

    Animator anim; // Animator変数

    Keyboard key; // Keyboard変数

    float normalSpeed = 3f; // 通常時の移動速度
    float sprintSpeed = 5f; // ダッシュ時の移動速度
    float jump = 8f;        // ジャンプ力
    float gravity = 10f;    // 重力の大きさ

    Vector3 moveDirection = Vector3.zero; // 移動した位置
    Vector3 startPos; // 初期位置

    /// ------private変数------- ///
    /// -------プロパティ------- ///



    /// -------プロパティ------- ///
    /// --------変数一覧-------- ///
}
