using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/***************/
//cardControl 及 player 类
//待完善
/****************/

public class playerControl : MonoBehaviour
{
    /*****player info********/
    public int playerHealth;//玩家血量
    public int distance;//公共数据距离
    /*************/

    /*******game info**********/
    public enum GameStageSate
    {
        begin,
        playerGetCard,
        playerPrepare,
        playerAction,
        playerEnd,
        EnemyPrepare,
        EnemyAction,
        EnemyEnd,
        Wait,
        DisCardWait,
        AllWait,
        GameEnd
    }

    /******************/

    /************/
    //牌的图片路径  
    private string fullPath = "Assets/Resources/image/cards/deck/";
    private string card_pic_path = "image/cards/deck/";

    /**stage state**/
    public int cardManagerState = 1;//游戏进行阶段标记

    public GameStageSate myState;


    /********card list**********/
    public List<string> CardNames;  //所有牌集合
    public List<string> CardHeapList;//牌堆名字队列;

    public List<string> CardList = new List<string>();//卡牌名字队列
    public string ToUseCard = "";//要使用的卡牌名字
    public List<string> UsedCardList = new List<string>();//使用过卡牌名字队列

    /*******card number and text*********/
    private Text cardHeapCountText;//数量维护标记
    private Text gameStageStateText; //游戏阶段标记
    private Text distanceText; //距离标记维护
    private Text roundCountText; //回合数文本标记

    public int CardHeapNum = 12;//牌堆默认总牌数

    /******prefab or resources*****/
    public GameObject prefab;   //预制件
    public GameObject coverPrefab;      //背面排预制件

    /***postion control***/
    //private Transform originPos1; //牌的初始位置1
    //private Transform originPos2; //牌的初始位置2 not nessery
    public RectTransform heapRectTrans;           //牌堆位置
    public RectTransform CardPanelRectTrans;    //玩家牌堆位置

    public float offsetX = 60;


    /*******object and its list*******/
    GameObject playerSelf;//玩家对象
    GameObject enmey; //敌人对象
    GameObject btnPanel;// 按键组合对象
    //GameObject ensureBtn;// 确认键
    GameObject gameFunc;//逻辑处理对象

    GameObject dataManager;//数据处理对象
    public GameObject RecordManager; //存档管理对象

    private List<GameObject> cards = new List<GameObject>();//卡牌对象列表
    private List<GameObject> covers = new List<GameObject>();   //背面卡牌对象，发牌结束后销毁


    /*****var and parameter*******/
    public int round = 0;//回合数标记
    public float dealCardSpeed = 20;  //发牌速度
    //int count = 0;
    //public float cardSize = 1f;//卡片缩放
    public float posOffset = 2.2f; //卡片偏移
    public float cardMoveSpeed = 1f;//卡片发牌移动速度
    public float posAdjSpeed = 0.3f;//卡片位置自调整速度
    public float prefabWidth = 125f;//预制体宽度



    void Awake()
    {

    }
    /*
    GameObject card01;
    //public Transform card01;//意图表示第一张牌的位置  

    //public Transform card02;

    public GameObject cardsprefab;

    public float thedistance=45;//两张牌的距离  
    */

    // Use this for initialization
    void Start()
    {
        /******预制件加载***********/
        prefab = (GameObject)Resources.Load("prefab/cardPrefab");
        coverPrefab = (GameObject)Resources.Load("prefab/coverPrefab");

        /********对象加载******/
        playerSelf = GameObject.Find("self");
        enmey = GameObject.Find("Enemy");
        btnPanel = GameObject.Find("BtnPanel");
        //ensureBtn = GameObject.Find("UseCardBtn");
        gameFunc = GameObject.Find("GameManager");
        dataManager = GameObject.Find("DataManager");
        RecordManager = GameObject.Find("RecordManager");

        /************位置加载**************/
        //originPos1 = GameObject.Find("CardPanel").GetComponent<RectTransform>();
        heapRectTrans = GameObject.Find("cardHeap").GetComponent<RectTransform>();
        CardPanelRectTrans = GameObject.Find("CardPanel").GetComponent<RectTransform>();

        /*********文本组件加载*********/
        cardHeapCountText = GameObject.Find("cardHeapText").GetComponent<Text>();
        gameStageStateText = GameObject.Find("GameStageText").GetComponent<Text>();
        distanceText = GameObject.Find("DistanceText").GetComponent<Text>();
        roundCountText = GameObject.Find("RoundCountText").GetComponent<Text>();
        /*
        GameObject card = (GameObject)Resources.Load("prefab/cardPre");
        card01 = Instantiate(card);

        Debug.Log("create sucess!");
        GameObject mUICanvas = GameObject.Find("CardPanel");
        card01.transform.parent = mUICanvas.transform;
        CardList.Add(card01);
        */

        //OnTestClick();

        //初始化
        CardNames = GetCardNames();
        CardHeapList = CardNames;
        //Debug.Log(cardNames.Join(","));

        myState = GameStageSate.begin;//置为开始阶段；

        //隐藏不必要按键
        ButtonDisable();
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.S))
        {
            StartCoroutine(Getcard(2));
        }
        */

        /****文本标记维护****/
        //cardHeapCountText = GameObject.Find("cardHeap").GetComponent<Text>();
        if (!!cardHeapCountText)
        {
            cardHeapCountText.text = CardHeapList.Count.ToString();
        }

        if (!!cardHeapCountText)
        {
            distanceText.text = "距离:" + playerSelf.GetComponent<effect_self>().Distance().ToString();
        }

        /***弃牌阶段侦听***/
        if (myState == GameStageSate.DisCardWait && CardList.Count <= 2)
        {
            ButtonDisable();
            myState = GameStageSate.EnemyPrepare;
            //break;
        }

        App();

    }
    public void App()
    {
        /****游戏阶段逻辑顺序****/
        /************* 游戏准备阶段*****************/
        if (myState == GameStageSate.begin)
        {
            if (main.LoadData)
            {
                Debug.Log("开始读档");
                if (RecordManager.GetComponent<OperateRecord>().ReadDataRecord())
                {
                    return;
                }
                else
                {
                    myState = GameStageSate.AllWait;
                }
                Debug.Log("读档完毕");
                
            }
            myState = GameStageSate.AllWait;
            //UI刷新待加入
            if (!!gameStageStateText)//游戏阶段文本刷新
            {
                gameStageStateText.text = "游戏开始";
            }


            OnBegin();//牌的处理函数
            //myState = GameStageSate.playerGetCard; 在DealCard 函数结尾
        }

        /****************己方回合****************/

        else if (myState == GameStageSate.playerGetCard)
        {
            myState = GameStageSate.AllWait;

            Debug.Log("state" + playerSelf.GetComponent<effect_self>().state);
            gameFunc.GetComponent<gameManager>().state_judge();
            //UI刷新待加入
            round += 1;
            roundCountText.text = "第" + round.ToString() + "回合";//回合数文本刷新

            if (!!gameStageStateText)//游戏阶段文本刷新
            {
                gameStageStateText.text = "摸牌阶段";
            }
            //行动力刷新
            playerSelf.GetComponent<effect_self>().IncreaseMoveEnergy(1);

            //洗牌
            if (CardHeapList.Count < 2)
            {
                ShuffleCards();
            }
            StartCoroutine(Getcard(2, 1));//摸牌
            //myState = GameStageSate.playerPrepare; 在GetCard 函数结尾
        }
        else if (myState == GameStageSate.playerPrepare)
        {
            myState = GameStageSate.AllWait;
            //UI刷新待加入
            if (!!gameStageStateText)//游戏阶段文本刷新
            {
                gameStageStateText.text = "准备阶段";
            }
            //TODO:状态调整
            myState = GameStageSate.playerAction; //待加入到函数结尾
        }
        else if (myState == GameStageSate.playerAction)
        {
            myState = GameStageSate.Wait;
            //UI刷新待加入
            if (!!gameStageStateText)//游戏阶段文本刷新
            {
                gameStageStateText.text = "行动阶段";
            }


            ButtonEnable();//显示按钮
                           //进行众多卡牌操作


            //TODO:增加结束按钮来进入下一阶段
            //myState = GameStageSate.playerEnd  加入到结束按钮中  finish  *****not work*****

        }
        else if (myState == GameStageSate.playerEnd)
        {
            myState = GameStageSate.DisCardWait;
            //UI刷新待加入
            if (!!gameStageStateText)//游戏阶段文本刷新
            {
                gameStageStateText.text = "结束阶段";
            }

            ButtonForDiscard();
            //弃牌 待加入  即使用牌直到牌等于两张
            //TODO:写一个弃牌函数  弃牌函数结尾进入下一阶段敌人准备阶段  

            //ButtonDisable();
            //myState = GameStageSate.EnemyPrepare; //待加入到函数结尾
        }

        /*****************敌人回合*********************/
        else if (myState == GameStageSate.EnemyPrepare)
        {
            myState = GameStageSate.AllWait;
            //UI刷新待加入
            if (!!gameStageStateText)//游戏阶段文本刷新
            {
                gameStageStateText.text = "敌方回合";
            }

            myState = GameStageSate.EnemyAction; //待加入到函数结尾
        }
        else if (myState == GameStageSate.EnemyAction)
        {
            myState = GameStageSate.AllWait;

            /**************TODO:增加敌人动作演示*****************/
            if (playerHealth == 1)
                gameFunc.GetComponent<gameManager>().boss1_func();
            else
            {
                gameFunc.GetComponent<gameManager>().boss2_func();

            }
            playerSelf.GetComponent<effect_self>().TakeDamage(3);
            //playerSelf.GetComponent<effect_self>().AddselfBlood(-1);

            //StartCoroutine(JumpEnemy());
            /*****************************/
            myState = GameStageSate.EnemyEnd; //加入到jump函数结尾
        }
        else if (myState == GameStageSate.EnemyEnd)
        {
            myState = GameStageSate.AllWait;

            myState = GameStageSate.playerGetCard;//待加入到函数结尾
        }

    }

    //开始阶段 牌的处理
    public void OnBegin()
    {
        //弃牌堆 和牌堆转化 待调整

        ClearCards(); //清空牌局
        ShuffleCards(); //牌堆洗牌
        StartCoroutine(Getcard(3, 0)); //初始发牌
    }

    //使button生效时
    public void ButtonEnable()
    {
        btnPanel.SetActive(true); //激活btnPanel
        //ensureBtn.SetActive(true); //激活确认键
    }

    //调整Button用于弃牌
    public void ButtonForDiscard()
    {
        btnPanel.SetActive(false); //隐藏btnPanel
        //ensureBtn.SetActive(true); //激活确认键
    }

    //使Button 全部失效
    public void ButtonDisable()
    {
        btnPanel.SetActive(false); //隐藏btnPanel
        //ensureBtn.SetActive(false); //隐藏确认键
    }

    //增加一张卡牌
    public void AddCard(string cardName)
    {
        //将牌加入手牌列表
        CardList.Add(cardName);

        //维护 牌堆卡牌 和 手牌 数量标记
        //cardCountText.text = (CardHeapNum - CardList.Count).ToString();
    }

    //清空所有卡牌 信息
    public void DropCards()
    {
        CardList.Clear();
    }

    //清空所有卡牌 对象
    public void DestroyAllCards()
    {
        cards.ForEach(Destroy);
        cards.Clear();
    }



    //使用卡牌
    public void UseCard(string name)
    {
        //第一步检查
        //再生效并触发UI逻辑
        //结束
        /*
        for (int i = cards.Count - 1; i >= 0; i--)
        {
            if (cards[i].GetComponent<card>().SelectState())
            {
                UsedCardList.Add(CardList[i]);
                ToUseCard = CardList[i];
                Debug.Log(CardList[i]);
                CardList.RemoveAt(i);
            }
        }
        */
        //
        int i = CardList.IndexOf(name);
        //
        UsedCardList.Add(CardList[i]);
        ToUseCard = CardList[i];
        Debug.Log(CardList[i]);
        //删除卡牌名字和对象
        CardList.RemoveAt(i);

        //销毁对象并从列表中删除
        Destroy(cards[i]);
        cards.RemoveAt(i);

        Debug.Log(cards.Count);


        //Debug.Log(ToUseCardList[0]);
        //effect(ToUseCardList[0]);


        //使用卡牌名字
        gameFunc.GetComponent<gameManager>().one_card(ToUseCard);
        //清空要使用卡牌名字
        ToUseCard = "";
        //ToUseCardList.Clear();


        //使用卡牌的出发的ui特效///////////
        ////////////////////////////////

        //更新手牌
        //DestroyAllCards();
        //GenerateAllCards(false);
        AdjPosition();

        //人物动作
        //StartCoroutine(JumpSelf());//自身跳动

        //卡牌引发动画？

        //////////////////////////////



    }


    /// <summary>
    /// 调整卡牌ui层级
    /// </summary>
    public void AdjLayer()
    {
        //GameObject var;
        for (int i = 0; i < cards.Count(); i++)
        {
            cards[i].transform.SetSiblingIndex(i);
        }
    }


    /// <summary>
    /// 调整卡牌位置到指定位置
    /// </summary>
    public void AdjPosition(string selected_card_name = "")
    {
        //StartCoroutine(AdjustPosition(selected_card_name));
        AdjustPosition(selected_card_name);
    }

    //public  IEnumerator AdjustPosition(string selected_card_name = "")
    public void AdjustPosition(string selected_card_name = "")
    {
        int card_index = 0;

        //不新建和销毁卡牌对象
        if (selected_card_name != "")
        {
            int i = CardList.IndexOf(selected_card_name);
            if (i < 0) Debug.Log("no this card");
            //TODO:位置定位

            //重新计算每张牌的偏移
            offsetX = CardPanelRectTrans.rect.width / (CardList.Count() + 1);
            offsetX = Mathf.Min(offsetX, prefabWidth);


            float distance_index = 0;
            //按新的偏移调整之前的牌
            for (; card_index < CardList.Count; card_index++)
            {
                if (card_index == i || card_index == i + 1) distance_index += 0.5f;

                var targetPos = CardPanelRectTrans.position + Vector3.right * offsetX * distance_index;
                cards[card_index].transform.SetAsLastSibling();
                iTween.MoveTo(cards[card_index], targetPos, posAdjSpeed);
                //yield return new WaitForSeconds(1 / dealCardSpeed);
                distance_index += 1;
            }

        }
        else
        {
            //TODO:位置定位
            //重新计算每张牌的偏移
            offsetX = CardPanelRectTrans.rect.width / CardList.Count();
            offsetX = Mathf.Min(offsetX, prefabWidth);

            //按新的偏移调整之前的牌
            for (; card_index < CardList.Count; card_index++)
            {
                var targetPos = CardPanelRectTrans.position + Vector3.right * offsetX * card_index;
                cards[card_index].transform.SetAsLastSibling();
                iTween.MoveTo(cards[card_index], targetPos, posAdjSpeed);
                //yield return new WaitForSeconds(1 / dealCardSpeed);
            }
        }
    }

    //弃牌函数
    public void DisCard(string name)
    {
        //第一步检查


        //再生效并触发UI逻辑
        //结束
        /*
        for (int i = cards.Count - 1; i >= 0; i--)
        {
            if (cards[i].GetComponent<card>().SelectState())
            {
                UsedCardList.Add(CardList[i]);
                ToUseCard = CardList[i];
                Debug.Log(CardList[i]);
                CardList.RemoveAt(i);
            }
        }
        */
        int i = CardList.IndexOf(name);
        UsedCardList.Add(CardList[i]);
        ToUseCard = CardList[i];
        Debug.Log(CardList[i]);

        //删除卡牌名字和对象
        CardList.RemoveAt(i);

        //销毁对象并从列表中删除
        Destroy(cards[i]);
        cards.RemoveAt(i);

        //Debug.Log(cards.Count);


        //Debug.Log(ToUseCardList[0]);
        //effect(ToUseCardList[0]);


        //使用卡牌名字
        //gameFunc.GetComponent<gameManager>().one_card(ToUseCard);
        //清空要使用卡牌名字
        ToUseCard = "";
        //ToUseCardList.Clear();


        //使用卡牌的出发的ui特效///////////
        ////////////////////////////////

        //更新手牌
        //DestroyAllCards();
        //GenerateAllCards(false);
        AdjPosition();

        return;
    }

    //获得卡牌即摸牌  
    public IEnumerator Getcard(int Num, int flag = 2)
    {
        if (flag == 1) yield return new WaitForSeconds(1f);

        float speed = 1f;


        //从牌堆中取出制定数量的牌，
        //维护牌堆列表
        //播放相应动画
        // DestroyAllCards();
        if (Num > CardHeapList.Count)
        {
            ShuffleCards();
        }


        //记录摸牌前牌数
        int cardnum_before = CardList.Count;
        Debug.Log(cardnum_before);

        //重新计算每张牌的偏移
        offsetX = CardPanelRectTrans.rect.width / (CardList.Count() + Num);

        offsetX = Mathf.Min(offsetX, prefabWidth);


        int card_index = 0;

        //按新的偏移调整之前的牌
        for (; card_index < cardnum_before; card_index++)
        {
            var targetPos = CardPanelRectTrans.position + Vector3.right * offsetX * card_index;
            cards[card_index].transform.SetAsLastSibling();
            iTween.MoveTo(cards[card_index], targetPos, speed);
            yield return new WaitForSeconds(1 / dealCardSpeed);
        }

        //Debug.Log(offsetX); Debug.Log("offsetX00000");

        //从牌堆中取牌
        CardHeapNum = CardHeapList.Count();
        for (int i = CardHeapNum - 1; i >= CardHeapNum - Num; i--)
        {
            string cardName = CardHeapList[i];

            //给当前玩家发一张牌
            //playerSelf.GetComponent<player>().AddCard(cardName);
            AddCard(cardName);
            CardHeapList.RemoveAt(i);
        }

        //动画摸牌起点
        var startPos = heapRectTrans.position;

        //新加入牌的位置调整
        for (; card_index < CardList.Count(); card_index++)
        {

            speed = 1f;
            //生成卡牌
            var card = Instantiate(prefab, startPos, Quaternion.identity, CardPanelRectTrans.transform);// TODO: need to fix
            card.GetComponent<RectTransform>().position = startPos;//暂时的处理方法
            card.GetComponent<RectTransform>().localScale = Vector3.one * 0.5f;
            card.GetComponent<card>().InitImage(CardList[card_index], dataManager.GetComponent<DataManager>().Search(CardList[card_index]));

            //移动和放大
            //offsetX = card.GetComponent<RectTransform>().rect.width;// * posOffset;
            var targetPos = CardPanelRectTrans.position + Vector3.right * offsetX * card_index;
            card.transform.SetAsLastSibling();
            //Debug.Log(offsetX);
            //Debug.Log("offsetX11111");
            //动画移动
            iTween.MoveTo(card, targetPos, speed);
            card.GetComponent<RectTransform>().localScale = Vector3.one * 1f;
            card.transform.parent.SetParent(CardPanelRectTrans.transform);
            cards.Add(card);

            yield return new WaitForSeconds(1 / dealCardSpeed);
        }



        //隐藏牌堆
        //heapRectTrans.gameObject.SetActive(false);
        //CardPanelRectTrans.gameObject.SetActive(false);

        //动画结束，进入叫牌阶段
        yield return new WaitForSeconds(0.5f);
        //covers.ForEach(Destroy);
        //covers.Clear();


        //显示玩家手牌
        /*
        if (playerSelf != null)
        {
            //playerSelf.GetComponent<player>().GenerateAllCards();
            GenerateAllCards(true);
        }
        */
        //cardManagerState = CardManagerStates.Bid;

        //摸牌后检查牌堆是否该洗牌
        if (CardHeapList.Count == 0)
        {
            ShuffleCards();
        }

        //摸牌后进入准备阶段
        //摸牌后进入准备阶段
        if (flag == 0)
            myState = GameStageSate.playerGetCard;
        else if (flag == 1)
            myState = GameStageSate.playerPrepare;

    }


    private List<string> GetCardNames()
    {
        //TODO: 改为通过json 数据文件对象读取图片名列表

        /*
        if (Directory.Exists(fullPath))
        {
            DirectoryInfo direction = new DirectoryInfo(fullPath);
            FileInfo[] files = direction.GetFiles("*.png", SearchOption.AllDirectories);

            return files.Select(s => Path.GetFileNameWithoutExtension(s.Name)).ToList();
        }
        return null;
        */
        if (dataManager)
        {
            return dataManager.GetComponent<DataManager>().NameSet();
        }

        return null;
    }

    public void ShuffleCards()
    {
        //进入洗牌阶段
        //cardManagerState = CardManagerStates.ShuffleCards;

        CardHeapList.InsertRange(0, UsedCardList);
        UsedCardList.Clear();

        CardHeapList = CardHeapList.OrderBy(c => Guid.NewGuid()).ToList();
    }


    /// <summary>
    /// 发牌
    /// </summary>
    //获得卡牌即摸牌  
    public IEnumerator DealCard(int Num, int flag = 2)
    {
        //yield return OtherCoroutine();

        float speed = 1f;


        //从牌堆中取出制定数量的牌，
        //维护牌堆列表
        //播放相应动画
        // DestroyAllCards();
        if (Num > CardHeapList.Count)
        {
            ShuffleCards();
        }


        //记录摸牌前牌数
        int cardnum_before = CardList.Count;
        Debug.Log(cardnum_before);

        //重新计算每张牌的偏移
        offsetX = CardPanelRectTrans.rect.width / (CardList.Count() + Num);

        offsetX = Mathf.Min(offsetX, prefabWidth);


        int card_index = 0;

        //按新的偏移调整之前的牌
        for (; card_index < cardnum_before; card_index++)
        {
            var targetPos = CardPanelRectTrans.position + Vector3.right * offsetX * card_index;
            cards[card_index].transform.SetAsLastSibling();
            iTween.MoveTo(cards[card_index], targetPos, speed);
            yield return new WaitForSeconds(1 / dealCardSpeed);
        }

        //Debug.Log(offsetX); Debug.Log("offsetX00000");

        //从牌堆中取牌
        CardHeapNum = CardHeapList.Count();
        for (int i = CardHeapNum - 1; i >= CardHeapNum - Num; i--)
        {
            string cardName = CardHeapList[i];

            //给当前玩家发一张牌
            //playerSelf.GetComponent<player>().AddCard(cardName);
            AddCard(cardName);
            CardHeapList.RemoveAt(i);
        }

        //动画摸牌起点
        var startPos = heapRectTrans.position;

        //新加入牌的位置调整
        for (; card_index < CardList.Count(); card_index++)
        {

            speed = 1f;
            //生成卡牌
            var card = Instantiate(prefab, startPos, Quaternion.identity, CardPanelRectTrans.transform);// TODO: need to fix
            card.GetComponent<RectTransform>().position = startPos;//暂时的处理方法
            card.GetComponent<RectTransform>().localScale = Vector3.one * 0.5f;
            card.GetComponent<card>().InitImage(CardList[card_index], dataManager.GetComponent<DataManager>().Search(CardList[card_index]));

            //移动和放大
            //offsetX = card.GetComponent<RectTransform>().rect.width;// * posOffset;
            var targetPos = CardPanelRectTrans.position + Vector3.right * offsetX * card_index;
            card.transform.SetAsLastSibling();
            //Debug.Log(offsetX);
            //Debug.Log("offsetX11111");
            //动画移动
            iTween.MoveTo(card, targetPos, speed);
            card.GetComponent<RectTransform>().localScale = Vector3.one * 1f;
            card.transform.parent.SetParent(CardPanelRectTrans.transform);
            cards.Add(card);

            yield return new WaitForSeconds(1 / dealCardSpeed);
        }



        //隐藏牌堆
        //heapRectTrans.gameObject.SetActive(false);
        //CardPanelRectTrans.gameObject.SetActive(false);

        //动画结束，进入叫牌阶段
        yield return new WaitForSeconds(0.5f);
        //covers.ForEach(Destroy);
        //covers.Clear();


        //显示玩家手牌
        /*
        if (playerSelf != null)
        {
            //playerSelf.GetComponent<player>().GenerateAllCards();
            GenerateAllCards(true);
        }
        */
        //cardManagerState = CardManagerStates.Bid;

        //摸牌后检查牌堆是否该洗牌
        if (CardHeapList.Count == 0)
        {
            ShuffleCards();
        }

        //摸牌后进入准备阶段
        if (flag == 0)
            myState = GameStageSate.playerPrepare;
        else if (flag == 1)
            myState = GameStageSate.playerGetCard;

    }

    /// <summary>
    /// 清空牌局
    /// </summary>
    public void ClearCards()
    {
        //清空所有玩家卡牌
        if (playerSelf != null)
        {
            //playerSelf.GetComponent<player>().DestroyAllCards();
            DestroyAllCards();
        };
        CardList.Clear();
    }

    public IEnumerator JumpSelf()
    {
        for (int i = 0; i < 2; i++)
        {
            iTween.MoveTo(playerSelf, playerSelf.transform.position + Vector3.up * 32f, 0.4f);
            //iTween.MoveTo(playerSelf, playerSelf.transform.position + Vector3.up * 10f, 0.2f);
            //iTween.MoveTo(playerSelf, playerSelf.transform.position - Vector3.up * 10f, 0.2f);
            yield return new WaitForSeconds(0.1f);
            iTween.MoveTo(playerSelf, playerSelf.transform.position - Vector3.up * 32f, 0.4f);
            yield return new WaitForSeconds(0.1f);

        }

    }
    public IEnumerator JumpEnemy()
    {
        for (int i = 0; i < 2; i++)
        {
            iTween.MoveTo(enmey, enmey.transform.position + Vector3.up * 32f, 0.4f);
            //iTween.MoveTo(enmey, enmey.transform.position + Vector3.up * 10f, 0.2f);
            yield return new WaitForSeconds(0.1f);
            //iTween.MoveTo(enmey, enmey.transform.position - Vector3.up * 10f, 0.2f);

            iTween.MoveTo(enmey, enmey.transform.position - Vector3.up * 32f, 0.4f);
            yield return new WaitForSeconds(0.1f);
        }
        myState = GameStageSate.EnemyEnd;
    }


    /// <summary>
    /// 供其他函数调用的接口
    /// </summary>
    public void AddNewCard(int num)
    {
        //增加指定数量的牌
        StartCoroutine(Getcard(num));
    }
    public void AddNewCard(String cardname)
    {
        //增加指定名字的牌
        CardNames.Add(cardname);
        CardHeapList.Insert(CardHeapList.Count, cardname);
        StartCoroutine(Getcard(1));
    }

    private void DelCard(string cardname)
    {
        //牌使用后删除函数
        CardHeapList.Remove(cardname);
        UsedCardList.Remove(cardname);
        CardNames.Remove(cardname);
    }

    public void CardPromotion()
    {

    }

    public void SaveCard()
    {
        //CardHeapList;//牌堆列表
        //CardList; //手牌列表
        //CardNames; //所有牌的列表
        //UseCardList; //弃牌堆列表
    }

    public void LoadCard(List<String> SavedCardHeapList, List<String> SavedCardList, List<String> SavedUsedCardList)
    {
        CardHeapList.Clear();
        CardHeapList = SavedCardHeapList;
        UsedCardList.Clear();
        UsedCardList = SavedUsedCardList;
        CardList.Clear();
        CardList = SavedCardList;
        Debug.Log("paidui" + SavedCardHeapList.Count + "qipai" + SavedUsedCardList.Count + "shoupai" + SavedCardList.Count);
        Debug.Log("paidui" + CardHeapList.Count + "qipai" + UsedCardList.Count + "shoupai" + CardList.Count);
        GenerateAllCards();

        //CardList;
        //CardNames;

    }

    public void LoadState()
    {
        Debug.Log("执行LoadState");
        myState= myState = GameStageSate.playerAction;
        Debug.Log("执行完毕");
    }

    public void GenerateAllCards()
    {
        if(CardPanelRectTrans==null) CardPanelRectTrans = GameObject.Find("CardPanel").GetComponent<RectTransform>();
        offsetX = CardPanelRectTrans.rect.width / (CardList.Count());
        offsetX = Mathf.Min(offsetX, prefabWidth);

        //动画摸牌起点
        var startPos = heapRectTrans.position;

        //新加入牌的位置调整
        for (int card_index = 0; card_index < CardList.Count(); card_index++)
        {

            float speed = 1f;
            //生成卡牌
            var card = Instantiate(prefab, startPos, Quaternion.identity, CardPanelRectTrans.transform);// TODO: need to fix
            //card.GetComponent<RectTransform>().position = startPos;//暂时的处理方法
            //card.GetComponent<RectTransform>().localScale = Vector3.one* 0.5f;
            card.GetComponent<card>().InitImage(CardList[card_index], dataManager.GetComponent<DataManager>().Search(CardList[card_index]));

            //移动和放大
            //offsetX = card.GetComponent<RectTransform>().rect.width;// * posOffset;
            var targetPos = CardPanelRectTrans.position + Vector3.right * offsetX * card_index;
            card.transform.SetAsLastSibling();

            card.transform.position = targetPos;
            card.GetComponent<RectTransform>().localScale = Vector3.one * 1f;
            card.transform.parent.SetParent(CardPanelRectTrans.transform);
            cards.Add(card);
        }
    }

    /*
    //整理手牌  //动画开关 可选
    public void GenerateAllCards(bool AnimationSwitch)
    {
        //计算每张牌的偏移
        //var offsetX = originPos2.position.x - originPos1.position.x;
        //获取最左边的起点
        //int leftCount = (cardInfos.Count / 2);
        var startPos = heapRectTrans.position;
        offsetX = CardPanelRectTrans.rect.width / CardList.Count();
        //Debug.Log(offsetX); Debug.Log("offsetX00000");
        float speed = 0f;
        for (int i = 0; i < CardList.Count; i++)
        {

            speed = 0.5f;
            //生成卡牌
            var card = Instantiate(prefab, startPos, Quaternion.identity, CardPanelRectTrans.transform);// TODO: need to fix
            card.GetComponent<RectTransform>().position = startPos;//暂时的处理方法
            card.GetComponent<RectTransform>().localScale = Vector3.one * 0.5f;
            card.GetComponent<card>().InitImage(CardList[i], card_pic_path);

            //移动和放大
            //offsetX = card.GetComponent<RectTransform>().rect.width;// * posOffset;
            var targetPos = CardPanelRectTrans.position + Vector3.right * offsetX * i;
            card.transform.SetAsLastSibling();
            //Debug.Log(offsetX);
            //Debug.Log("offsetX11111");
            //动画移动
            iTween.MoveTo(card, targetPos, speed);
            card.GetComponent<RectTransform>().localScale = Vector3.one * 1f;
            card.transform.parent.SetParent(CardPanelRectTrans.transform);

            cards.Add(card);

            
            if (AnimationSwitch)
            {
                speed = 2f;
                //生成卡牌
                var card = Instantiate(prefab, startPos, Quaternion.identity, heapRectTrans.transform);// TODO: need to fix
                card.GetComponent<RectTransform>().position = startPos;//暂时的处理方法
                card.GetComponent<RectTransform>().localScale = Vector3.one * 0.5f;
                card.GetComponent<card>().InitImage(CardList[i], card_pic_path);

                //移动和放大
                //offsetX = card.GetComponent<RectTransform>().rect.width;// * posOffset;
                var targetPos = CardPanelRectTrans.position + Vector3.right * offsetX * i;
                card.transform.SetAsLastSibling();
                Debug.Log(offsetX);
                Debug.Log("offsetX11111");
                //动画移动
                iTween.MoveTo(card, targetPos, speed);
                card.GetComponent<RectTransform>().localScale = Vector3.one * 1f;
                card.transform.parent.SetParent(CardPanelRectTrans.transform);

                cards.Add(card);
            }
            else
            {
                //生成卡牌
                var card = Instantiate(prefab, startPos, Quaternion.identity, CardPanelRectTrans);// TODO: need to fix

                card.GetComponent<RectTransform>().localScale = Vector3.one * 1f;
                card.GetComponent<card>().InitImage(CardList[i], card_pic_path);

                //offsetX = card.GetComponent<RectTransform>().rect.width;// * posOffset;
                var targetPos = startPos + Vector3.right * offsetX * i;
                card.GetComponent<RectTransform>().position = targetPos;//暂时的处理方法
                card.transform.SetAsLastSibling();
                //动画移动
                //iTween.MoveTo(card, targetPos, speed);

                cards.Add(card);

            }
        }
    }
    */
}

