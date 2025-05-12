using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public GameObject cardLabel;
    public GameObject panel;

    public GameObject playerData;

    private PlayerDataManager pdm;
    private CardData cardData;
    private LibraryManager libraryManager;
    private List<GameObject> cardPool = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        pdm = playerData.GetComponent<PlayerDataManager>();
        libraryManager = GameObject.Find("LibraryManager").GetComponent<LibraryManager>();
        cardData = playerData.GetComponent<CardData>();

        UpdateDeck();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateDeck()
    {
        ClearPool();
        for (int i = 0; i < pdm.playerDeck.Length; i++)
        {
            if (pdm.playerDeck[i] != 0)
            {
                GameObject newCard = GameObject.Instantiate(cardLabel, panel.transform);
                cardPool.Add(newCard);
                newCard.GetComponent<CardDisplay>().card = cardData.CardList[i];
                newCard.GetComponent<UICardCounter>().counter.text = pdm.playerDeck[i].ToString();
            }
        }
    }
    public void ReturnAllCardsToLibrary()
    {
        for (int i = 0; i < pdm.playerDeck.Length; i++)
        {
            if (pdm.playerDeck[i] > 0)
            {
                // 将卡组中的卡片数量添加到仓库中
                pdm.playerCards[i] += pdm.playerDeck[i];
                // 将卡组中的卡片数量置为 0
                pdm.playerDeck[i] = 0;
            }
        }
    }
    public void ClearPool()
    {
        foreach (var card in cardPool)
        {
            Destroy(card);
        }
        cardPool.Clear();
    }

    public void OnClickSave()
    {
        pdm.SavePlayerData();
    }
    public void OnClickClear()
    {
        ReturnAllCardsToLibrary();
        UpdateDeck();
        libraryManager.UpdateLibrary();
        pdm.updateText();
        pdm.SavePlayerData(); // 保存数据到文件
    }
    public void OnClickdel() //卡牌分解
    {
        int cash = pdm.totalCoins;
        int startnum=pdm.Sum(pdm.playerCards);
        {
            // 检查并限制卡组
            for (int i = 0; i < pdm.playerDeck.Length; i++)
            {

                if (pdm.playerDeck[i] > 3)
                {
                    pdm.playerDeck[i] = 3;
                }
            }

            // 检查并限制仓库
            for (int i = 0; i < pdm.playerCards.Length; i++)
            {
                if (pdm.playerCards[i]+pdm.playerDeck[i] > 3)
                {
                    if(pdm.playerDeck[i]==3) 
                    { pdm.playerCards[i] = 0; }
                    if(pdm.playerDeck[i]==2)
                    { pdm.playerCards[i] = 1; }
                    if(pdm.playerDeck[i]==1)
                    { pdm.playerCards[i] = 2; }
                    if(pdm.playerDeck[i]==0)
                    { pdm.playerCards[i] = 3; }
                   

                }
            }
            int finishnum = pdm.Sum(pdm.playerCards);
            pdm.totalCoins = cash + (startnum - finishnum) *30;
        }
        UpdateDeck();
        libraryManager.UpdateLibrary();
        pdm.updateText();
        // 保存玩家数据
        pdm.SavePlayerData();
    }
}
