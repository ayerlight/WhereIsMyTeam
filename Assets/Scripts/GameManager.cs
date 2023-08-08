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
    public GameObject scoreText;
    public GameObject resultPanel;
    float time;
    int tryMatchCount;
    int score, minTryCount;
    bool isSpeedUp;
    bool isSuccess;

    const int MAX_TRYCOUNT_SCORE = 1000;

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

    private void Awake()
    {
        I = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;

        int[] teams = { 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7 };
        teams = teams.OrderBy(item => Random.Range(-1.0f, 1.0f)).ToArray();

        // ������ ��������Ʈ ��� �θ���
        Sprite[] sprites = Resources.LoadAll<Sprite>(CARD_PATH);
        time = 30f;
        tryMatchCount = 0;
        isSuccess = false;
        score = 0;
        minTryCount = teams.Length / 2;


        for (int i = 0; i < 16; i++)
        {
            GameObject newCard = Instantiate(card);
            newCard.transform.parent = GameObject.Find("cards").transform;
            float x = (i / 4) * 1.4f - 2.1f;
            float y = (i % 4) * 1.4f - 3.0f;
            newCard.transform.position = new Vector3(x, y, 0);

            Transform frontTrans = newCard.transform.Find("front");
            SpriteRenderer cardRenderer = frontTrans.GetComponent<SpriteRenderer>();

            cardRenderer.sprite = sprites[teams[i]];

            // ������ ����
            Vector3 tempScale = frontTrans.transform.localScale;
            tempScale.x *= rtanSpriteSize / cardRenderer.sprite.rect.width;
            tempScale.y *= rtanSpriteSize / cardRenderer.sprite.rect.height;
            frontTrans.localScale = tempScale;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (time <= 0f)
        {
            Time.timeScale = 0f;
            setResultPanel();
            //endText.SetActive(true);
            //tryMatchCountText.SetActive(true);
        }
        else if (time <= 5f) {
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
                isSuccess = true;
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
        if (isSuccess)
        {
            score += (int)time * 100;
            int tryCntScore = MAX_TRYCOUNT_SCORE - ((tryMatchCount - 8) * 50);
            if (tryCntScore > 0) score += tryCntScore;
        }
        setResultPanel();
        //endText.SetActive(true);
        //tryMatchCountText.SetActive(true);
    }

    public void retryGame()
    {
        SceneManager.LoadScene("MainScene");
    }

    private void setResultPanel()
    {
        // ���� ����� ������ ��� �г��� set
        resultPanel.SetActive(true);    // �г� Ȱ��ȭ
        endText.GetComponent<Text>().text = isSuccess ? "����!" : "����!";  // ���� or ���� �ؽ�Ʈ 
        tryMatchCountText.GetComponent<Text>().text = tryMatchCount + " ȸ �õ�";  // ��Ī �õ� Ƚ�� �ؽ�Ʈ
        scoreText.GetComponent<Text>().text = "score " + score; // ���� �ؽ�Ʈ 
    }
}
