using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stageSelectManager : MonoBehaviour
{
    private int stage = 1;

    public static stageSelectManager SSM;

    const string LOCKED_STAGE = "lockedStage";

    private void Awake()
    {
        SSM = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // �������� ���� �׻� ���� Ŭ������ ���������� ���� ���������� ���� (1�������� ���������� 2��������)
        if (PlayerPrefs.HasKey(LOCKED_STAGE)) 
        {
            SSM.stage = PlayerPrefs.GetInt(LOCKED_STAGE) + 1;
        } else
        {
            SSM.stage = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public int getStage()
    {
        return stage;
    }

    public void setStage(int value)
    {
        SSM.stage = value;
    }
}