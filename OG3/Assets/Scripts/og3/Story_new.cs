using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class Story_new : MonoBehaviour
{

    //�d�l����
    /*
     * �X�L�b�v�͎��̌��܂��͑I����
     */

    public static Story_new instance;
    

    public bool textnextflag = false;
    private bool automodeflag = false;

    bool textread = false; //�����Đ���
    public bool animationfinishedflag = true;

    private Text _story; //�X�g�[���[�e�L�X�g
    private Text _name;

    private Text _move; //�ړ��A�j���[�V�������̃e�L�X�g


    public TextAsset storyText; //csv�X�g�[���[�f�[�^
    private string _storyArray;
    private List<Qdata_new> _qdataList = new List<Qdata_new>();


    public int qstory = 0; //story�̔ԍ�
    public int qNum = 0; //story��
    int messageCount = 0; //�\������Ă��镶���̐�
    float novelspeed = 0.1f; //�����̕\�����x

    public int nameinput = 0;
    private String heroineName;

    [SerializeField] GameObject textbox;
    [SerializeField] GameObject ScreenButton;
    [SerializeField] GameObject SelectButtonPanel;
    [SerializeField] GameObject logPanel;
    [SerializeField] GameObject MenuPanel;
    [SerializeField] GameObject cannotskipAlertPanel;
    [SerializeField] GameObject LoadingPanel;
    [SerializeField] GameObject settingPanel;
    [SerializeField] GameObject Selectbutton_1;
    [SerializeField] GameObject Selectbutton_2;
    [SerializeField] GameObject SelectButton_3;
    [SerializeField] GameObject menubutton;
    [SerializeField] GameObject monthtext;
    [SerializeField] GameObject monthTextPanel;

    [SerializeField] GameObject moveanimationPrefab;
    [SerializeField] GameObject fadecloseanimationPrefab;
    [SerializeField] GameObject fadeopenanimationPrefab;

    [SerializeField] GameObject[] autosetbutton = new GameObject[5];

    [SerializeField] GameObject[] textsetbutton = new GameObject[5];

    [SerializeField] GameObject[] sesetbutton = new GameObject[5];

    [SerializeField] GameObject[] bgmsetbutton = new GameObject[5];

    // Start is called before the first frame update

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    void Start()
    {
        //�e�L�X�g
        _story = GameObject.Find("MainText").GetComponent<Text>();
        _name = GameObject.Find("NameText").GetComponent<Text>();

        nameinput = PlayerPrefs.GetInt("NAMEINPUT");
        if (nameinput == 0)
        {
            heroineName = PlayerPrefs.GetString("INPUTNAME");
            //Debug.Log("���O" + heroineName);
        }
        else
        {
            heroineName = PlayerPrefs.GetString("INPUTNAME2");
            //Debug.Log("���O����" + heroineName);
        }

        heroineName = PlayerPrefs.GetString("INPUTNAME");

        //csv�t�@�C������e�L�X�g��ǂݍ���
        _storyArray = storyText.text.Replace(" ", "\u00A0");
        _storyArray = storyText.text.Replace("@", heroineName);
        StringReader sr = new StringReader(_storyArray);
        sr.ReadLine();
        while (sr.Peek() > -1)
        {
            string line = sr.ReadLine();
            _qdataList.Add(new Qdata_new(line));
            qNum++;
        }
        //�ŏ��̃X�g�[���[���Z�b�g
        //�m�F�̂��߂�Console�ɏo��
        foreach (Qdata_new q in _qdataList)
        {
            //q.WriteDebugLog();
        }


        //�ŏ��̃X�^�[�g������ύX����
        StartCoroutine(Novel(qstory));
    }

    // Update is called once per frame
    void Update()
    {
        //�R���[�`����i�߂�
        if ((textnextflag && animationfinishedflag))
        {
            //�X�s�[�h��0����߂�Ȃ������������@��Őݒ��ʂőI�񂾕��ɉ������l������悤�ɂ�����
            novelspeed = 0.1f;
            StartCoroutine(Novel(qstory));
        }
    }

    private IEnumerator Novel(int index)
    {
        textnextflag = false;
        //�I�[�g��
        if (automodeflag)
        {
            //�I�[�g���[�h�̑��x�{�^���ɂ���Ă����Œ�~����
            textnextflag = true;
        }

        stilldisplay();
        monthstartdisplay();
        backdisplay();
        characterdisplay();
        charactercolor();
        textcolor();
        moveanimation();
        fadeanimation();


        //���O��month�������Ă���ꏊ��ʉ߂�����e�L�X�g�ύX
        if (_qdataList[index].nameText.Contains("month"))
        {
            //����̌��\��
            GameObject.Find("monthtext").GetComponent<Text>().text = _qdataList[index].nameText.Replace("month", "");
        }

        //���O
        String textcolorsr = _qdataList[index].textcolor;
        if (textcolorsr.Equals("text_own"))
        {
            _name.text = heroineName;
        }
        else if (textcolorsr.Equals("text_monologue"))
        {
            _name.text = "";
        }
        else
        {
            _name.text = _qdataList[index].nameText;
        }


        //�{���Đ�
        _story.text = "";
        messageCount = 0;
        while (_qdataList[index].storyText.Length > messageCount)
        {
            textread = true;
            _story.text += _qdataList[index].storyText[messageCount];
            messageCount++;
            yield return new WaitForSeconds(novelspeed);
        }

        textread = false;
        //�X�g�[���[�ԍ������ɐi�߂�
        qstory++;
    }

    public void onClicked_screenbutton()
    {
        if (textread)
        {
            novelspeed = 0;
        } else
        {
            textnextflag = true;
        }
    }

    public void onClicked_automodebutton()
    {
        //�I�[�g���[�h�X�^�[�g
        automodeflag = true;
    }

    private void stilldisplay()
    {
        //�ꖇ�G
        String stillsr = _qdataList[qstory].stillimage;
        Image stillimage = (Image)GameObject.Find("stillImage").GetComponent<Image>();
        if (stillsr.Equals("0"))
        {
            menubutton.SetActive(true);
            stillimage.sprite = Resources.Load<Sprite>("Sprites/back/back_clear");
        }
        else
        {
            String getstill_index = Regex.Replace(stillsr, @"[^0-9]", "");
            GameManager.instance.getimage[int.Parse(getstill_index)] = 1;
            stillimage.sprite = Resources.Load<Sprite>("Sprites/back/" + stillsr.Replace(getstill_index, ""));
            menubutton.SetActive(false);
        }
    }

    private void monthstartdisplay()
    {
        //���̂͂��߂̉摜
        String monthsr = _qdataList[qstory].monthimage;
        Image monthimage = (Image)GameObject.Find("month").GetComponent<Image>();
        if (monthsr.Equals("0"))
        {
            monthimage.sprite = Resources.Load<Sprite>("Sprites/back/back_clear");
        } else
        {
            monthimage.sprite = Resources.Load<Sprite>("Sprites/month/" + monthsr);
        }
    }

    private void backdisplay()
    {
        //�w�i
        String backsr = _qdataList[qstory].backimage;
        Image backimage = (Image)GameObject.Find("backgroundImage").GetComponent<Image>();
        backimage.sprite = Resources.Load<Sprite>("Sprites/back/" + backsr);
    }

    private void characterdisplay()
    {
        //�Z���^�[�摜
        String centersr = _qdataList[qstory].centerimage;
        Image centerCharacter = (Image)GameObject.Find("CenterCharacterImages").GetComponent<Image>();
        centerCharacter.sprite = Resources.Load<Sprite>("Sprites/human/" + centersr);


        //���C�g�摜
        String rightsr = _qdataList[qstory].rightimage;
        Image rightCharacter = (Image)GameObject.Find("RightCharacterImages").GetComponent<Image>();
        rightCharacter.sprite = Resources.Load<Sprite>("Sprites/human/" + rightsr);


        //���t�g�摜
        String leftsr = _qdataList[qstory].leftimage;
        Image leftCharacter = (Image)GameObject.Find("LeftCharacterImages").GetComponent<Image>();
        leftCharacter.sprite = Resources.Load<Sprite>("Sprites/human/" + leftsr);
    }

    private void charactercolor() {
        //�摜�̐F
        String colorsr = _qdataList[qstory].charactercolor;
        if (int.Parse(colorsr) == 0)
        {
            GameObject.Find("CenterCharacterImages").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            GameObject.Find("RightCharacterImages").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            GameObject.Find("LeftCharacterImages").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
        else if (int.Parse(colorsr) == 1)
        {
            GameObject.Find("CenterCharacterImages").GetComponent<Image>().color = new Color32(100, 100, 100, 255);
            GameObject.Find("RightCharacterImages").GetComponent<Image>().color = new Color32(100, 100, 100, 255);
            GameObject.Find("LeftCharacterImages").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
        else if (int.Parse(colorsr) == 2)
        {
            GameObject.Find("CenterCharacterImages").GetComponent<Image>().color = new Color32(100, 100, 100, 255);
            GameObject.Find("RightCharacterImages").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            GameObject.Find("LeftCharacterImages").GetComponent<Image>().color = new Color32(100, 100, 100, 255);
        }
        else
        {
            GameObject.Find("CenterCharacterImages").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            GameObject.Find("RightCharacterImages").GetComponent<Image>().color = new Color32(100, 100, 100, 255);
            GameObject.Find("LeftCharacterImages").GetComponent<Image>().color = new Color32(100, 100, 100, 255);
        }
    }

    private void textcolor()
    {
        //�e�L�X�g�{�b�N�X�̐F
        String textcolorsr = _qdataList[qstory].textcolor;
        Image textboximage = (Image)GameObject.Find("TextImage").GetComponent<Image>();
        textboximage.sprite = Resources.Load<Sprite>("Sprites/UI/" + textcolorsr);
        monthTextPanel.SetActive(true);
        if (textcolorsr.Equals("0"))
        {
            textboximage.sprite = Resources.Load<Sprite>("Sprites/back/back_clear");
            monthTextPanel.SetActive(false);
        }
    }

    private void moveanimation()
    {
        String moveanimationtext = _qdataList[qstory].moveanimation;
        if (!moveanimationtext.Equals("0"))
        {
            Instantiate(moveanimationPrefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity,GameObject.Find("BackgroundPanel").transform);
            _move = GameObject.Find("movetext").GetComponent<Text>();
            _move.text = moveanimationtext;
            animationfinishedflag = false;
        } else
        {
            animationfinishedflag = true;
        }
    }

    private void fadeanimation()
    {
        String fadeflag = _qdataList[qstory].fadeanimation;
        if (int.Parse(fadeflag) == 1)
        {
            Instantiate(fadecloseanimationPrefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity, GameObject.Find("GameManager").transform);
            animationfinishedflag = false;
        }
    }
}

//������Ǘ�����N���X
public class Qdata_new
{
    int number;
    public string storyText;
    public string nameText;
    public string centerimage;
    public string rightimage;
    public string leftimage;
    public string backimage;
    public string stillimage;
    public string charactercolor;
    public string bgm_state;
    public string bgm_num;
    public string se_num;
    public string selectdisp;
    public string selectbutton_num;
    public string monthimage;
    public string selectbuttontext3;
    public string selectbuttontext1;
    public string selectbuttontext2;
    public string textcolor;
    public string fadeanimation;
    public string moveanimation;


    public Qdata_new(string txt)
    {
        string[] spTxt = txt.Split(',');
        if (spTxt.Length == 21)
        {
            number = int.Parse(spTxt[0]);
            storyText = spTxt[1];
            nameText = spTxt[2];
            centerimage = spTxt[3];
            rightimage = spTxt[4];
            leftimage = spTxt[5];
            backimage = spTxt[6];
            stillimage = spTxt[7];
            charactercolor = spTxt[8];
            bgm_state = spTxt[9];
            bgm_num = spTxt[10];
            se_num = spTxt[11];
            selectdisp = spTxt[12];
            selectbutton_num = spTxt[13];
            monthimage = spTxt[14];
            selectbuttontext3 = spTxt[15];
            selectbuttontext1 = spTxt[16];
            selectbuttontext2 = spTxt[17];
            textcolor = spTxt[18];
            fadeanimation = spTxt[19];
            moveanimation = spTxt[20];
        }
    }

    public void WriteDebugLog()
    {
        Debug.Log(number + "\t" + storyText + "\t" + centerimage + "\t" + nameText + "\t" + selectbuttontext1 + "\t" + selectbuttontext2);
    }

}