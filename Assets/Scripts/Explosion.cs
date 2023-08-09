using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour, IPoolable<Explosion>
{
    [Header("ī�� ���� �ݹ�")]
    Action<Explosion> returnCallback;
    
    [Header("ī�� ���� �ִϸ�����")]
    Animator animator;

    [Header("���ҽ� ���")]
    const string EXPLOSION_PATH = "Explosion";

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        SetSize();
        animator.Play(EXPLOSION_PATH);
    }

    void OnDisable()
    {
        returnCallback?.Invoke(this);
    }

    public void SetReturnObject(Action<Explosion> action)
    {
        returnCallback = action;
    }

    void SetSize()
    {
        if (stageManager.S.stageCardSizeDict.Count == 0)
        {
            return;
        }

        transform.localScale *= stageManager.S.stageCardSizeDict[stageSelectManager.SSM.getStage()];
    }

    // �ִϸ��̼� ���� �� SetActive(false);
    void OnExploded()
    {
        gameObject.SetActive(false);
    }
}
