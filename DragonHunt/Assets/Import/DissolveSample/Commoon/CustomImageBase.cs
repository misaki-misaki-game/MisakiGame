// Copyright (C) 2022 Hatayama Masamichi
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Applibot
{
    [ExecuteAlways]  // ���s���A�܂��̓G�f�B�^��ł̕ύX���ɂ����̃N���X���L��
    [RequireComponent(typeof(Graphic))]  // ���̃R���|�[�l���g�����݂���ɂ� Graphic ���K�{
    public class CustomImageBase : MonoBehaviour, IMaterialModifier
    {
        // Graphic�R���|�[�l���g�̃L���b�V���p�ϐ�
        [NonSerialized] private Graphic _graphic;

        // Image�R���|�[�l���g�̃L���b�V���p�ϐ�
        [NonSerialized] private Image _image;

        // �}�e���A���̎Q�Ƃ�ێ�����ϐ�
        protected Material material;

        // CanvasScaler�̃L���b�V���p�ϐ�
        private CanvasScaler _canvasScaler;

        // �V�F�[�_�[�Ŏg�p����e�N�X�`���̋�`����\���v���p�e�BID
        private int _textureRectId = Shader.PropertyToID("_textureRect");

        // CanvasScaler �v���p�e�B�BCanvasScaler ���L���b�V������Ă��Ȃ��ꍇ�A�擾���ăL���b�V��
        protected CanvasScaler canvasScaler
        {
            get
            {
                if (_canvasScaler == null)
                {
                    // Graphic �̃L�����o�X���� CanvasScaler ���擾
                    _canvasScaler = graphic.canvas.GetComponent<CanvasScaler>();
                }
                return _canvasScaler;
            }
        }

        // Graphic �v���p�e�B�BGraphic ���L���b�V������Ă��Ȃ��ꍇ�A�擾���ăL���b�V��
        public Graphic graphic
        {
            get
            {
                if (_graphic == null)
                {
                    _graphic = GetComponent<Graphic>();
                }
                return _graphic;
            }
        }

        // �}�e���A����ύX���邽�߂̃C���^�[�t�F�[�X�����B���̃}�e���A����n���ď������s��
        public Material GetModifiedMaterial(Material baseMaterial)
        {
            if (!isActiveAndEnabled || graphic == null)
            {
                // �����܂��� Graphic ���Ȃ��ꍇ�̓x�[�X�}�e���A�������̂܂ܕԂ�
                return baseMaterial;
            }

            // �}�e���A�����X�V
            UpdateMaterial(baseMaterial);
            // �A�g���X����ݒ�
            SetAtlasInfo();
            return material;
        }

        // �A�j���[�V�����v���p�e�B���K�p���ꂽ��̏���
        private void OnDidApplyAnimationProperties()
        {
            if (!isActiveAndEnabled || graphic == null)
            {
                return;
            }

            // �}�e���A�����ύX���ꂽ���Ƃ�ʒm
            graphic.SetMaterialDirty();
        }

        // �h���N���X�ŃI�[�o�[���C�h�����A�}�e���A���̍X�V���s�����\�b�h
        protected virtual void UpdateMaterial(Material baseMaterial)
        {
            // �h���N���X�ł̎�����z��
        }

        // �A�g���X����ݒ肷�郁�\�b�h
        private void SetAtlasInfo()
        {
            // Image�R���|�[�l���g���Ȃ��ꍇ�͏I��
            if (_image == null)
            {
                return;
            }

            // �X�v���C�g���A�g���X�Ƀp�b�N����Ă��Ȃ��ꍇ�A�L�[���[�h�𖳌���
            if (!_image.sprite.packed)
            {
                material.DisableKeyword("USE_ATLAS");
                return;
            }

            // �X�v���C�g�̃e�N�X�`����`�����擾
            Rect textureRect = _image.sprite.textureRect;
            Vector4 r = new Vector4(
                textureRect.x,
                textureRect.y,
                textureRect.width,
                textureRect.height);
            // �V�F�[�_�[�Ƀe�N�X�`����`����ݒ�
            material.SetVector(_textureRectId, r);
            material.EnableKeyword("USE_ATLAS");
        }

        // �R���|�[�l���g���L���ɂȂ����ۂ̏���
        protected void OnEnable()
        {
            if (graphic == null)
            {
                return;
            }

            // Graphic��Image�ɃL���X�g
            _image = graphic as Image;
            // �}�e���A�����X�V����K�v�����邱�Ƃ�ʒm
            graphic.SetMaterialDirty();
        }

        // �R���|�[�l���g�������ɂȂ����ۂ̏���
        protected void OnDisable()
        {
            if (material != null)
            {
                // �}�e���A����j��
                DestroyMaterial();
            }

            if (graphic != null)
            {
                // �}�e���A���̍X�V��ʒm
                graphic.SetMaterialDirty();
            }
        }

        // �R���|�[�l���g���j�����ꂽ�ۂ̏���
        protected void OnDestroy()
        {
            if (material != null)
            {
                // �}�e���A����j��
                DestroyMaterial();
            }
        }

        // �}�e���A����j�����郁�\�b�h
        public void DestroyMaterial()
        {
#if UNITY_EDITOR
            // �G�f�B�^���ōĐ����łȂ��ꍇ�A�����Ƀ}�e���A����j��
            if (EditorApplication.isPlaying == false)
            {
                DestroyImmediate(material);
                material = null;
                return;
            }
#endif
            // �Đ����̏ꍇ�A�ʏ�̃}�e���A���j������
            Destroy(material);
            material = null;
        }

#if UNITY_EDITOR
        // �G�f�B�^�Ō��؎��̏���
        protected void OnValidate()
        {
            if (!isActiveAndEnabled || graphic == null)
            {
                return;
            }

            // �}�e���A���̍X�V��ʒm
            graphic.SetMaterialDirty();
        }
#endif
    }
}