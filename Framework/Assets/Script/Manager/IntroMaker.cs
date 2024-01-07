using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class IntroMaker : MonoBehaviour
{
    //IntroSprites
    public Sprite[] sprites;

    [SerializeField]
    private Image CurIntro;

    //IntroTexts
    public TextAsset[] texts;
    
    [SerializeField]
    private TextMeshProUGUI CurText;

    public int readSpeed;
    
    int count;
    int index;
    public float skip = 3.5f;

    //Color A
    float alpha;
    bool isUp = false, isDown = false;
    bool isIntroStart = false;


    [SerializeField]
    private Image IntroCast;

    bool IntroUp = false;
    bool SoundCheck = false;
    private void Start()
    {
        index = 0;
        count = 0;
        alpha = 1.0f;
        isUp = false;
        isDown = false;
        IntroUp = false;
        SoundCheck = false;
        isIntroStart = false;
        CurText.text = "";
        IntroCast.gameObject.SetActive(false);
    }

    //���� �������
    void NextSprite()
    {
        CurIntro.sprite = sprites[count];
        
        if (texts.Length > count)
            NextText();
        else
        {
            if (count != 11)
            {
                index = 0;
                CurText.text = "";
                Invoke("EndText", 3.3f);
            }
        }
    }
    // ���� �ؽ�Ʈ�� ���۵ǵ���
    void NextText()
    {
        index = 0;
        CurText.text = "";
        Invoke("UpdateText", 1f / readSpeed);
    }
    void UpdateText()
    {
        if (index == 0)
            Invoke("EndText", skip);

        if (count != 13)
        {
            if (CurText.text != texts[count].text)
            {
                if (texts[count].text[index] == '.' && count == 4)
                    Invoke("LastText", 0.4f);
                else
                {
                    CurText.text += texts[count].text[index];
                    if (texts[count].text[index] != ' ' && texts[count].text[index] != ','
                        && texts[count].text[index] != '.' && texts[count].text[index] != '\n')
                            SoundManager.Instance.SFXTextPlay("SND_TXT2", 1);

                    index++;

                    Invoke("UpdateText", 1f / readSpeed);
                }
            }
        }
    }

    // Ư�� .�϶� ���۵Ǵ� �̺�Ʈ
    void LastText()
    {
        CurText.text += texts[count].text[index];
        SoundManager.Instance.SFXTextPlay("SND_TXT2", 1);

        index++;

        Invoke("UpdateText", 1f / readSpeed);
    }

    // �ϴ� ������ 3.5�� �ڿ� ����ȭ������ �Ѿ���� �ϴ°�..
    // UpdateText���� �����ϸ� �ؽ�Ʈ�� �� ������ �Ѿ�Ե� �Ҽ�����..
    void EndText()
    {
        if (count != 10)
            isDown = true;
        else
        {
            isUp = true;
            alpha = 0;
            count++;
            CurIntro.sprite = sprites[count];
        }
    }

    //���� ȭ������
    void NextScene()
    {
        SceneManager.LoadScene("ProduceScene");
    }

    // ���� �ö󰡴� �̺�Ʈ
    void EventSprite()
    {
        if (count == 10)
        {
            if (!IntroUp)
            {
                transform.localPosition = new Vector2(0, 459);
                IntroUp = true;
            }
            IntroCast.gameObject.SetActive(true);
        }
        else if (count == 11)
        {
            if (transform.localPosition.y > -120)
            {
                Vector2 vector2 = transform.position;
                vector2.y -= 50 * Time.deltaTime;
                transform.position = vector2;
                alpha = 1;
            }
            else
            {
                count++;
            }
            IntroCast.gameObject.SetActive(true);
        }
        else if (count == 12)
        {
            Invoke("EndText", 3.8f);
            alpha = 1.0f;
            IntroCast.gameObject.SetActive(true);
        }
    }

    // Sprite A����ȯ
    void EventA()
    {
        if (isIntroStart)
        {
            if (isDown)
            {
                if (alpha > 0)
                {
                    alpha -= Time.deltaTime * 0.8f;
                    Color c = CurIntro.color;
                    c.a = alpha;
                    CurIntro.color = c;
                    if (count == 12) //������ ��������Ʈ�϶�.
                    {
                        isIntroStart = false;
                        count++;
                        SoundCheck = true;
                        SoundManager.Instance.StopBGSound();
                        SoundManager.Instance.SFXPlay("IntroSFX", 215);
                        alpha = 1;
                        CurText.text = "";
                        index = 0;
                        CurIntro.sprite = sprites[count];
                        IntroCast.gameObject.SetActive(false);
                        transform.localPosition = Vector3.zero;
                    }
                }
                else
                {
                    isDown = false;
                    isUp = true;
                    alpha = 0;
                    count++;
                    CurIntro.sprite = sprites[count];
                    IntroCast.gameObject.SetActive(false);
                    transform.localPosition = Vector3.zero;
                }
            }

            if (isUp)
            {
                if (alpha <= 1)
                {
                    alpha += Time.deltaTime * 0.8f;

                    Color c = CurIntro.color;
                    c.a = alpha;
                    CurIntro.color = c;
                }
                else
                {
                    isUp = false;
                    alpha = 1;
                    if (count != 13)
                        NextSprite();
                    else
                        isIntroStart = false;
                }
            }

        }
    } 

    // Spacebar ��
    void EventSpace()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isIntroStart)
            {
                if (count != 13)
                {
                    isIntroStart = true;
                    SoundManager.Instance.BGSoundPlay();
                    NextSprite();
                }
                else
                {
                    count = 13;
                    if (!SoundCheck)
                    {
                        SoundManager.Instance.StopBGSound();
                        SoundManager.Instance.SFXPlay("IntroSFX", 215);
                    }
                    SoundCheck = true;
                    alpha = 1;
                    CurText.text = "";
                    index = 0;
                    Invoke("NextScene", 3f);
                    CurIntro.sprite = sprites[count];
                    IntroCast.gameObject.SetActive(false);
                    transform.localPosition = Vector3.zero;
                }
            }
            else
            {
                isIntroStart = false;
                count = 13;
                if (!SoundCheck)
                {
                    SoundManager.Instance.StopBGSound();
                    SoundManager.Instance.SFXPlay("IntroSFX", 215);
                }
                SoundCheck = true;
                alpha = 1;
                CurText.text = "";
                index = 0;
                Invoke("NextScene", 3f);
                CurIntro.sprite = sprites[count];
                IntroCast.gameObject.SetActive(false);
                transform.localPosition = Vector3.zero;
            }
        }
    }
    private void Update()
    {
        EventSprite();
        EventSpace();
        EventA();
    }
}
