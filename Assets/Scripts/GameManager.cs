using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Text timeText;
    public GameObject card;
    public GameObject endText;
    public GameObject tryMatchCountText;
    float time;
    int tryMatchCount;
    bool isSpeedUp;

    public static GameManager I;

    public GameObject firstCard;
    public GameObject secondCard;

    public AudioClip match;
    public AudioClip fail;
    public AudioSource audioSource;
    public Text matchText;

    [Header("��ź�� ��������Ʈ ������ ���� 500x500")]
    public int rtanSpriteSize = 500;

    [Header("ī�� ����Ʈ �ð�")]
    public float matchTextTime = 1f;
    public string unCorrectMessage = "����";


    [Header("��Ī �÷�")]
    public Color correctColor;
    public Color unCorrectColor;

    [Header("ī�� ���")]
    public const string CARD_PATH = "cardImages";

    [Header("���� �Ǵ� ���� �� ������")]
    public float radius = 5f;

    [Header("ī�� ����")]
    public float cardSettingTime = 1f;
    bool isSettings = true;

    private void Awake()
    {
        I = this;
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        Time.timeScale = 1f;
        time = 30f;
        timeText.text = time.ToString("N2");

        int[] teams = { 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7 };
        teams = teams.OrderBy(item => Random.Range(-1.0f, 1.0f)).ToArray();
        
        // ������ ��������Ʈ ��� �θ���
        Sprite[] sprites = Resources.LoadAll<Sprite>(CARD_PATH);
        tryMatchCount = 0;

        for (int i = 0; i < 16; i++)
        {
            GameObject newCard = Instantiate(card);
            newCard.transform.parent = GameObject.Find("cards").transform;

            // ū ���� ������ ��ŭ�� ���� ��ġ ��
            newCard.transform.position = Random.onUnitSphere * radius;
            float x = (i / 4) * 1.4f - 2.1f;
            float y = (i % 4) * 1.4f - 3.0f;

            StartCoroutine(CoMoveOffsetPosition(newCard.transform, new Vector3(x, y, 0)));

            Transform frontTrans = newCard.transform.Find("front");
            SpriteRenderer cardRenderer = frontTrans.GetComponent<SpriteRenderer>();

            cardRenderer.sprite = sprites[teams[i]];

            // ������ ����
            Vector3 tempScale = frontTrans.transform.localScale;
            tempScale.x *= rtanSpriteSize / cardRenderer.sprite.rect.width;
            tempScale.y *= rtanSpriteSize / cardRenderer.sprite.rect.height;
            frontTrans.localScale = tempScale;
        }

        yield return new WaitForSeconds(cardSettingTime);

        isSettings = false;
    }

    IEnumerator CoMoveOffsetPosition(Transform cardTrans, Vector3 destination)
    {
        Vector3 offsetPos = cardTrans.position;
        Vector3 targetPos = Vector3.zero;
        float ratio = 0f;
        while (ratio < cardSettingTime)
        {
            ratio += Time.deltaTime;
            targetPos = Vector3.Lerp(offsetPos, destination, ratio / cardSettingTime);

            // ���� ������ (x-a)^2 + (y-b)^2 = r^2
            float halfRadius = radius * 0.5f;
            // �������� ����
            float powRadius = Mathf.Pow(halfRadius, 2);
            // ���� x��ġ�� ��ǥ�� �������� ����������
            bool isDestinationXLow = targetPos.x > destination.x;
            // �������̶�� ������ ���ֱ� �����̶�� ������ �����ֱ� (���� ���� x��ǥ�� ��������ŭ ���̳��ϱ�)
            float powXPos = isDestinationXLow ? Mathf.Pow(targetPos.x - destination.x - halfRadius, 2) 
                : Mathf.Pow(targetPos.x - destination.x + halfRadius, 2);
            // y��ǥ
            float yPos = Mathf.Sqrt(Mathf.Abs(powRadius - powXPos));

            // ���� ��ġ���� ��ǥ���������� ����(���� ����) ���� �� + �� �߽����κ��� y��ǥ
            targetPos.y += yPos;
            cardTrans.position = targetPos;

            yield return null;
        }
    }

    void Update()
    {
        if (isSettings)
        {
            return;
        }

        if (time <= 0f)
        {
            Time.timeScale = 0f;
            endText.SetActive(true);
            tryMatchCountText.GetComponent<Text>().text = tryMatchCount + " try";
            tryMatchCountText.SetActive(true);
        } else if (time <= 5f) {
            if (!isSpeedUp)
            {
                timeText.color = Color.red;
                audioManager.A.playSpeedUpMusic();
                isSpeedUp = true;
            }
            time -= Time.deltaTime;
            timeText.text = time.ToString("N2");
        } else
        {
            time -= Time.deltaTime;
            timeText.text = time.ToString("N2");
        }
    }

    public void isMatched()
    {
        string firstCardImage = firstCard.transform.Find("front").GetComponent<SpriteRenderer>().sprite.name;
        string secondCardImage = secondCard.transform.Find("front").GetComponent<SpriteRenderer>().sprite.name;

        if (firstCardImage == secondCardImage)
        {
            StartCoroutine(CoVerifyMatching(firstCardImage, true));

            audioSource.PlayOneShot(match);

            firstCard.GetComponent<card>().destroyCard();
            secondCard.GetComponent<card>().destroyCard();

            int cardsLeft = GameObject.Find("cards").transform.childCount;
            if (cardsLeft == 2)
            {
                Invoke("GameEnd", 1f);
            }
        }
        else
        {
            StartCoroutine(CoVerifyMatching(firstCardImage));
            audioSource.PlayOneShot(fail);
        
            firstCard.GetComponent<card>().closeCard();
            secondCard.GetComponent<card>().closeCard();
        }

        firstCard = null;
        secondCard = null;
        tryMatchCount++;
    }

    private IEnumerator CoVerifyMatching(string cardName, bool isCorrect = false)
    {
        // ������ ��
        if (isCorrect)
        {
            matchText.text = cardName.Split('_')[0];
            matchText.color = correctColor;
        }
        // �ƴ� ��
        else
        {
            matchText.text = unCorrectMessage;
            matchText.color = unCorrectColor;
        }

        matchText.gameObject.SetActive(true);
        yield return new WaitForSeconds(matchTextTime);
        matchText.gameObject.SetActive(false);
    }

    void GameEnd()
    {
        Time.timeScale = 0f;
        endText.SetActive(true);
        tryMatchCountText.GetComponent<Text>().text = tryMatchCount + " try";
        tryMatchCountText.SetActive(true);
    }

    public void retryGame()
    {
        SceneManager.LoadScene("MainScene");
    }
}
