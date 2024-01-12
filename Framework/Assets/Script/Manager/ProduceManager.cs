using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public enum CurScene
{
    Help,
    ProduceName,
    DecideName, 
    // �ƴϿ� ������ index �������� ����
};
//�ڵ忡�� ����� �Է� ó��, �ִϸ��̼� �� �ؽ�Ʈ ȿ��, ȭ�� ��ȯ ���� ���ԵǾ� �ֽ��ϴ�.
//�ڵ�� C# ���� �ۼ��Ǿ����� Unity�� 
//MonoBehaviour Ŭ������ ��ӹ޾� ����� ��ȣ �ۿ� �� ���� ������ ó���մϴ�.

public class ProduceManager : MonoBehaviour
{
    public CurScene curScene = CurScene.Help;

    public GameObject[] sprites;
    public TextMeshProUGUI[] Alphabet;

    [SerializeField]
    private TextMeshProUGUI CurName;

    [SerializeField]
    private TextMeshProUGUI CurDecudeName;

    [SerializeField]
    private TextMeshProUGUI NopeText;

    [SerializeField]
    private TextMeshProUGUI TitleText;
    [SerializeField]
    private TextMeshProUGUI NoText;
    [SerializeField]
    private TextMeshProUGUI YesText;

    public RectTransform Help;
    public RectTransform ProduceName;
    public RectTransform DecideName;
    public RectTransform Nope;
    public Image fadeOut;


    [SerializeField] [Range(0.01f, 1f)] float shakeRange = 0.05f;
    [SerializeField] [Range(0.1f, 1f)] float duration = 0.5f;
    private SoundManager soundManager; // SoundManager �ν��Ͻ��� �ʵ�� �߰�

    public int index = 0;
    float alpha = 0;
    public RectTransform[] Temp = new RectTransform[TotalCharacters];
    public RectTransform NameTemp;

    //�̽��� ���� �ƹ��͵� �Է� ����ü ������ ������ decide
    //TotalCharacters -> ���� ���� ���ĺ� �ѷ�
    //choiceAlphabets -> ������ �ܾ�
    //refusal �ƴϿ� 
    //decide �� 

    const int TotalCharacters = 55;
    const int choiceAlphabets = 3;

    const int refusal = 59;
    const int decide = 60;


    private float scaleText = 1;
    private bool isfadeOut = false;
    void Start()
    {
        soundManager = SoundManager.Instance;
        soundManager.BGSoundPlay();
        CurName.text = "";
        CurDecudeName.text = "";
        index = 0;
        isfadeOut = false;
        NameTemp.localPosition = CurDecudeName.rectTransform.localPosition;
        CurIndexYellow();
        for (int i = 0; i <= TotalCharacters; i++)
        {
            Temp[i].localPosition = Alphabet[i].rectTransform.localPosition;
        }
        AlphabetShake();
        NameShake();
        ChangeScene(CurScene.Help);
    }

    void Update()
    {
        StartShake();
        NameStartShake();
        MoveIndex();
        //���ڶ���.
        SelectZ();
        if (isfadeOut)
        {
            if (alpha <= 1)
            {
                alpha += Time.deltaTime * 0.20f;

                Color c = fadeOut.color;
                c.a = alpha;
                fadeOut.color = c;
            }
            else
            {
                Debug.Log("��");
                alpha = 1;
                SceneManager.LoadScene("GameScene");
            }
        }
    }

    void CurIndexYellow()
    {
        if (index < 0)
            index = 0;

        else if (index >= Alphabet.Length)
            index = Alphabet.Length - 1;

        Alphabet[index].color = Color.yellow;
    }

    void PrevIndexWhile()
    {
        Alphabet[index].color = Color.white;
    }

    void MoveIndex()
    {
        if (curScene != CurScene.Help)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                PrevIndexWhile();
                if (index != refusal)
                    index--;
                else
                    index++;
                CurIndexYellow();
                NoneIndex(false);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                PrevIndexWhile();
                if (curScene == CurScene.ProduceName && index < TotalCharacters + choiceAlphabets)
                    index++;

                if (index == decide)
                    index--;
                else if (index == refusal)
                    index++;

                CurIndexYellow();
                NoneIndex(true);
            }
            if (curScene != CurScene.DecideName)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    PrevIndexWhile();
                    index -= 7;
                    CurIndexYellow();
                    NoneIndex(false);
                }
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    PrevIndexWhile();
                    index += 7;
                    CurIndexYellow();
                    NoneIndex(false);
                }
            }
        }
    }
    void NoneIndex(bool isSign) // + true, - false
    {
        if (Alphabet[index].name == "None" && curScene != CurScene.DecideName)
        {
            PrevIndexWhile();
            if (isSign)
                index++;
            else
                index--;
            CurIndexYellow();
            NoneIndex(isSign);
        }
    }

    void Exit()
    {
        ChangeScene(CurScene.Help);
    }
    void Erase()
    {
        if (CurName.text.Length > 0)
            CurName.text = CurName.text.Substring(0, CurName.text.Length - 1);
    }
    void Decide()
    {
        GameManager.Instance.player_Name = CurName.text;
        if (!isfadeOut)
        {
            soundManager.StopBGSound();
            soundManager.SFXPlay("Flower_Lalf", 216);
            isfadeOut = true;
        }
    }

    void SelectZ()
    {
        if (curScene == CurScene.ProduceName)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                if (index == TotalCharacters + choiceAlphabets - 2)// Nopt
                {
                    Exit();
                }
                else if (index == TotalCharacters + choiceAlphabets - 1)// Nopt
                {
                    Erase();
                }
                else if (index == TotalCharacters + choiceAlphabets && CurName.text.Length > 0)
                {
                    ChangeScene(CurScene.DecideName);
                }
                else
                {
                    if (CurName.text.Length < 8)
                    {
                        CurName.text += Alphabet[index].name;
                    }
                }
            }
        }
        else if (curScene == CurScene.Help)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                ChangeScene(CurScene.ProduceName);
            }
        }
        else
        {
            if (scaleText < 2.7f)
            {
                Vector3 TextPos = NameTemp.gameObject.transform.localPosition;
                scaleText += 0.5f * Time.deltaTime;
                TextPos.y -= 80 * Time.deltaTime;
                NameTemp.gameObject.transform.localPosition = new Vector3(TextPos.x, TextPos.y, 1);
            }
            CurDecudeName.gameObject.transform.localScale = new Vector3(scaleText, scaleText, 1);
            if (Input.GetKeyDown(KeyCode.Z))
            {
                if (index == refusal)
                {
                    ChangeScene(CurScene.ProduceName);
                    scaleText = 1;
                    CurDecudeName.gameObject.transform.localScale = new Vector3(1, 1, 1);
                    NameTemp.gameObject.transform.localPosition = new Vector3(115, 106, 0);
                    index = TotalCharacters + choiceAlphabets;
                }
                else
                {
                    Decide();
                }
            }
        }
    }

    void ChangeScene(CurScene nextscene)
    {
        curScene = nextscene;

        ProduceName.gameObject.SetActive(false);
        DecideName.gameObject.SetActive(false);
        Nope.gameObject.SetActive(false);
        Help.gameObject.SetActive(false);

        if (curScene == CurScene.Help)//���۹�
        {
            ProduceName.gameObject.SetActive(false);
            DecideName.gameObject.SetActive(false);
            Help.gameObject.SetActive(true);
            Nope.gameObject.SetActive(false);
        }
        else if (curScene == CurScene.ProduceName) //�̸����ϱ�
        {
            ProduceName.gameObject.SetActive(true);
            DecideName.gameObject.SetActive(false);
            Help.gameObject.SetActive(false);
            Nope.gameObject.SetActive(false);
        }
        else //����
        {
            //�ʱ�ȭ
            DecideName.gameObject.SetActive(true);
            Nope.gameObject.SetActive(true);
            TitleText.gameObject.SetActive(false);

            YesText.gameObject.SetActive(false);
            NoText.gameObject.SetActive(true);
            //���� �ʱ�ȭ
            NoText.text = "���ư���";
            TitleText.text = " �� �̸��� Ȯ���մϱ�?";

            if (CurName.text == "Alphys")
            {
                NopeText.text = "��_�׷��� ��.";
                CurDecudeName.text = CurName.text;
            }
            else if (CurName.text == "Asgore")
            {
                NopeText.text = "�׷��Դ� �� �� ���ܴ�.";
                CurDecudeName.text = CurName.text;
                Nope.gameObject.SetActive(true);
            }
            else if (CurName.text == "Asriel")
            {
                NopeText.text = "...";
                CurDecudeName.text = CurName.text;
            }
            else if (CurName.text == "Flowey")
            {
                NopeText.text = "���� �̹� �� \n�̸� ���� �ִٰ�";
                CurDecudeName.text = CurName.text;
            }
            else if (CurName.text == "Gaster")
            {
                SceneManager.LoadScene("IntroScene");
            }
            else if (CurName.text == "sans")
            {
                NopeText.text = "�ƾƴ�.";
                CurDecudeName.text = CurName.text;
            }
            else if (CurName.text == "Toriel")
            {
                NopeText.text = "�������� �̸��� \n������ �����Ϸ�\n�ư���.";
                CurDecudeName.text = CurName.text;
            }
            else if (CurName.text == "Undyne")
            {
                NopeText.text = "�� �̸� ������ �˾Ƽ� ������!";
                CurDecudeName.text = CurName.text;
            }
            else if (CurName.text == "Chara")
            {
                NopeText.text = "��¥ �̸�.";
                CurDecudeName.text = CurName.text;
            }
            
            else if (CurName.text == "Frisk")
            {
                YesText.gameObject.SetActive(true);
                TitleText.gameObject.SetActive(true);
                Nope.gameObject.SetActive(false);
                TitleText.text = "��� : �� �̸��� ����� ���� \n�������� ���� �� �Դϴ�.\n�̴�������Ͻðڽ��ϱ�?";
                NoText.text = "�ƴϿ�";
                CurDecudeName.text = CurName.text;
            }
            else
            {
                YesText.gameObject.SetActive(true);

                TitleText.gameObject.SetActive(true);
                Nope.gameObject.SetActive(false);
                if (CurName.text == "Papyru")
                {
                    TitleText.text = "�� ����ϳ��!!!";
                }
                if (CurName.text == "Decide")
                {
                    TitleText.text = "�Ϻ��� ����.";
                }
                NoText.text = "�ƴϿ�";
            }
            CurDecudeName.text = CurName.text;
            index = refusal;
            CurIndexYellow();
            NoneIndex(true);
        }
    }


    void AlphabetShake()
    {
        InvokeRepeating("StartShake", 0f, 0.05f);
        Invoke("StopShake", duration);
    }

    void StartShake()
    {
        for (int i = 0; i <= TotalCharacters; i++)
        {
            float alphabeShakeX = Random.value * shakeRange * 2 - shakeRange;
            float alphabeShakeY = Random.value * shakeRange * 2 - shakeRange;
            Vector2 alphabetPos = Alphabet[i].transform.localPosition;
            alphabetPos.x += alphabeShakeX;
            alphabetPos.y += alphabeShakeY;
            Alphabet[i].rectTransform.localPosition = alphabetPos;
        }
        Invoke("StopShake", duration);
    }

    void StopShake()
    {
        CancelInvoke("StartShake");
        for (int i = 0; i <= TotalCharacters; i++)
        {
            Alphabet[i].rectTransform.localPosition = Temp[i].localPosition;
        }
    }
    void NameShake()
    {
        InvokeRepeating("NameStartShake", 0f, 0.05f);
        Invoke("NameStopShake", duration);
    }
    void NameStartShake()
    {
        float alphabeShakeX = Random.value * shakeRange * 1.4f * scaleText - shakeRange;
        float alphabeShakeY = Random.value * shakeRange * 1.4f * scaleText - shakeRange;
        Vector2 alphabetPos = CurDecudeName.transform.localPosition;
        alphabetPos.x += alphabeShakeX;
        alphabetPos.y += alphabeShakeY;
        CurDecudeName.rectTransform.localPosition = alphabetPos;
        Invoke("NameStopShake", duration);
    }

    void NameStopShake()
    {
        CancelInvoke("NameStartShake");
        CurDecudeName.rectTransform.localPosition = NameTemp.localPosition;
    }
}
