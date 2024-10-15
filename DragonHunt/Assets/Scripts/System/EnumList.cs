/// --------�ϐ��ꗗ-------- ///

#region public�ϐ�
/// -------public�ϐ�------- ///

// �A�j���[�V�����̏��
public enum AnimState
{
    E_Idle = 0, // �ҋ@��
    E_Move = 1, // �ړ���
    E_Attack = 2, // �u���C�u�U����
    E_HitReaction = 3, // ��_���[�W��
    E_Dead = 4, // �퓬�s�\��
    E_Guard = 5, // �h�䒆
    E_Dodge = 6, // ���
}

// �U���̎��
public enum AttackState
{
    E_None = 0, // �U�����Ă��Ȃ�
    E_BraveAttack = 1, // �u���C�u�U����
    E_HPAttack = 2 // HP�U����
}

// �u���C�u�̏��
public enum BraveState
{
    E_Default = 0, // �ʏ���
    E_Regenerate = 1, // ���W�F�l���
    E_Break = 2 // �u���C�N���
}

// ��_���[�W�̏��
public enum DamageState
{
    E_Default = 0, // �ʏ���
    E_SuperArmor = 1, // �X�[�p�[�A�[�}�[���
    E_Invincible = 2, // ���G���
    E_Guard = 3, // �h����
    E_Dodge = 4 // ������
}

// �G�t�F�N�g�̖���
public enum EffectName
{
    braveDamageEffect = 0,
    hpDamageEffect = 1,
    hpShockWaveEffect = 2,
    hpFireBreath = 3,
    hpBomb1 = 4,
    hpBomb2 = 5,
    hpMeteor = 6,
    summon = 7,
    death = 8
}

// �p�[�e�B�N���V�X�e���̃g���K�[�Ώ�
public enum ParticleSystemTrigger
{
    E_PlayerWepon = 0,
    E_EnemyWepon = 1
}

// �Q�[���̏��
public enum GameState
{
    E_Title = 0,
    E_InGame = 1
}

// BGM�̃��X�g
public enum BGMList
{
    E_TitleBGM = 0,
    E_OpeningBGM = 1,
    E_InGameBGM = 2,
    E_VictoryBGM = 3,
    E_DefeatBGM = 4
}

// SE�̃��X�g
public enum SEList
{
    E_FootstepsSE = 0,
    E_SlashSE = 1,
    E_GetHitSE = 2,
    E_DragonBiteSE = 3,
    E_DragonstepsSE = 4,
    E_DragonFlameSE = 5,
    E_DragonScreamSE = 6,
    E_DragonWingSE = 7,
    E_DragonGetHitVoiceSE = 8,
    E_BeginChanceTime = 9,
    E_EndChanceTime = 10,
    E_SmallDragonstepsSE = 11,
    E_SmallDragonScreamSE = 12,
    E_BornSE = 13
}

// �v�[������I�u�W�F�N�g�^�C�v
public enum PoolType
{
    E_Effect = 0,
    E_DamageText = 1
}

/// -------public�ϐ�------- ///
#endregion

#region protected�ϐ�
/// -----protected�ϐ�------ ///



/// -----protected�ϐ�------ ///
#endregion

#region private�ϐ�
/// ------private�ϐ�------- ///



/// ------private�ϐ�------- ///
#endregion

#region �v���p�e�B
/// -------�v���p�e�B------- ///



/// -------�v���p�e�B------- ///
#endregion

/// --------�ϐ��ꗗ-------- ///