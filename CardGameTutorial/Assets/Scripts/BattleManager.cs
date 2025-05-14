using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// 定义游戏阶段的枚举类型
public enum GamePhase
{
    playerDraw,  // 玩家抽卡阶段
    playerAction, // 玩家行动阶段
    enemyDraw, // 敌人抽卡阶段
    enemyAction, // 敌人行动阶段
    gameStart // 游戏开始阶段
}

/* 原本定义游戏事件的枚举类型，此处注释掉
public enum GameEvent
{
    phaseChange, monsterDestroy
}
*/

// BattleManager 类继承自 MonoSingleton，用于管理战斗流程
public class BattleManager : MonoSingleton<BattleManager>
{
    public GameObject playerData; // 玩家数据对象
    public GameObject enemyData; // 敌人数据对象
    public GameObject playerHands; // 玩家手牌对象
    public GameObject enemyHands; // 敌人手牌对象
    public GameObject[] playerBlocks; // 玩家怪兽区的格子数组
    public GameObject[] enemyBlocks; // 敌人怪兽区的格子数组
    public List<Card> playerDeckList = new List<Card>(); // 玩家卡组列表
    public List<Card> enemyDeckList = new List<Card>(); // 敌人卡组列表
    public int currentTurn = 1;//计数器
    public GameObject cardPrefab; // 卡牌预制体

    public GameObject arrowPrefab; // 召唤指示箭头预制体
    public GameObject attackPrefab; // 攻击指示箭头预制体
    private GameObject arrow; // 当前显示的箭头对象

    // 玩家和敌人的生命值
    public int playerHealthPoint;
    public int enemyHealthPoint;

    public GameObject playerIcon; // 玩家图标对象
    public GameObject enemyIcon; // 敌人图标对象

    // 玩家和敌人的最大召唤次数
    public int maxPlayerSummonCount;
    public int playerSummonCount;
    public int maxEnemySummonCount;
    public int enemySummonCount;

    // 当前游戏阶段
    public GamePhase currentPhase = GamePhase.playerDraw;

    protected CardData CardDate; // 卡牌数据对象

    public Transform canvas; // 画布对象

    private GameObject waitingMonster; // 等待召唤的怪兽对象
    private int waitingID; // 等待召唤的玩家编号
    public GameObject attackingMonster; // 当前正在攻击的怪兽对象
    private int attackingID; // 当前正在攻击的玩家编号

    // 阶段变更事件，用于通知其他对象游戏阶段发生了变化
    public UnityEvent phaseChangeEvent;

    // 脚本实例化时调用的方法，通常用于初始化操作
    void Start()
    {
        // 开始游戏
        GameStart();
    }

    // 每帧调用一次的方法，可用于处理需要实时更新的逻辑
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            CancelSummonOrAttack();
        }
    }

    //取消攻击/召唤的方法
    private void CancelSummonOrAttack()
    {
        if (arrow != null)
        {
            Destroy(arrow);
        }
        waitingMonster = null;
        attackingMonster = null;
        foreach (var block in playerBlocks)
        {
            block.GetComponent<CardBlock>().CloseAll();
        }
        foreach (var block in enemyBlocks)
        {
            block.GetComponent<CardBlock>().CloseAll();
        }
    }

    // 玩家抽卡方法，只有在玩家抽卡阶段才能调用
    public void OnPlayerDrawCard()
    {
        if (currentPhase == GamePhase.playerDraw)
        {
            // 玩家抽一张卡
            DrawCard(0, 1);
        }
    }

    // 敌人抽卡方法，只有在敌人抽卡阶段才能调用
    public void OnEnemyDrawCard()
    {
        if (currentPhase == GamePhase.enemyDraw)
        {
            // 敌人抽一张卡
            DrawCard(1, 1);
        }
    }

    // 抽卡方法，_player 表示玩家编号（0 为玩家，1 为敌人），_number 表示抽卡数量，_back 表示是否显示卡背，_state 表示是否改变游戏阶段
    public void DrawCard(int _player, int _number, bool _back = false, bool _state = true)
    {
        if (_player == 0)
        {
            // 玩家抽卡
            for (int i = 0; i < _number; i++)
            {
                // 实例化一张新卡牌
                GameObject newCard = GameObject.Instantiate(cardPrefab, playerHands.transform);
                // 设置新卡牌的信息
                newCard.GetComponent<CardDisplay>().card = playerDeckList[0];
                // 从玩家卡组中移除已抽取的卡牌
                playerDeckList.RemoveAt(0);
                // 设置新卡牌的状态为玩家手牌
                newCard.GetComponent<BattleCard>().cardState = CardState.inPlayerHand;
                // 如果需要显示卡背
                if (_back)
                {
                    newCard.GetComponent<CardDisplay>().back = true;
                }
                // 如果需要改变游戏阶段
                if (_state)
                {
                    currentPhase = GamePhase.playerAction;
                    // 触发阶段变更事件
                    phaseChangeEvent.Invoke();
                }
            }
        }
        else if (_player == 1)
        {
            // 敌人抽卡
            for (int i = 0; i < _number; i++)
            {
                // 实例化一张新卡牌
                GameObject newCard = GameObject.Instantiate(cardPrefab, enemyHands.transform);
                // 设置新卡牌的信息
                newCard.GetComponent<CardDisplay>().card = enemyDeckList[0];
                // 从敌人卡组中移除已抽取的卡牌
                enemyDeckList.RemoveAt(0);
                // 设置新卡牌的状态为敌人手牌
                newCard.GetComponent<BattleCard>().cardState = CardState.inEnemyHand;
                // 如果需要显示卡背
                if (_back)
                {
                    newCard.GetComponent<CardDisplay>().back = true;
                }
                // 如果需要改变游戏阶段
                if (_state)
                {
                    currentPhase = GamePhase.enemyAction;
                    // 触发阶段变更事件
                    phaseChangeEvent.Invoke();
                }
            }
        }
    }

    // 点击结束回合的方法，通常为虚方法，可被子类重写
    public virtual void OnClickTurnEnd()
    {
        // 结束当前回合
        TurnEnd();
    }

    // 结束回合的方法，处理回合结束时的逻辑
    public void TurnEnd()
    {


        // 如果有箭头对象，销毁它
        if (arrow != null)
        {
            CancelSummonOrAttack();
        }

        // 检查是玩家还未抽卡
        if (currentPhase == GamePhase.playerDraw||currentPhase==GamePhase.enemyDraw)
        {
            Debug.Log("玩家未抽卡，不能结束回合！");
            return; // 直接返回，不执行后续回合结束逻辑
        }

        if (currentPhase == GamePhase.playerAction)
        {
            // 玩家行动阶段结束，进入敌人抽卡阶段
            currentPhase = GamePhase.enemyDraw;
            // 重置敌人的召唤次数
            enemySummonCount = maxEnemySummonCount;

            // 可以在这里添加玩家图标和敌人图标可攻击状态的设置逻辑，此处注释掉
            //playerIcon.GetComponent<AttackTarget>().attackable = true;
            //enemyIcon.GetComponent<AttackTarget>().attackable = false;

            // 设置玩家怪兽区的怪兽可攻击状态
            foreach (var block in playerBlocks)
            {
                if (block.GetComponent<CardBlock>().monsterCard != null)
                {
                    block.GetComponent<CardBlock>().monsterCard.GetComponent<AttackTarget>().attackable = true;
                }
            }
            // 设置敌人怪兽区的怪兽不可攻击状态，并重置攻击状态
            foreach (var block in enemyBlocks)
            {
                if (block.GetComponent<CardBlock>().monsterCard != null)
                {
                    block.GetComponent<CardBlock>().monsterCard.GetComponent<AttackTarget>().attackable = false;
                    block.GetComponent<CardBlock>().monsterCard.GetComponent<BattleCard>().hasAttacked = false;
                }
            }
        }
        else if (currentPhase == GamePhase.enemyAction)
        {
            // 敌人行动阶段结束，进入玩家抽卡阶段
            currentPhase = GamePhase.playerDraw;
            // 重置玩家的召唤次数
            playerSummonCount = maxPlayerSummonCount;

            // 可以在这里添加玩家图标和敌人图标可攻击状态的设置逻辑，此处注释掉
            //playerIcon.GetComponent<AttackTarget>().attackable = false;
            //enemyIcon.GetComponent<AttackTarget>().attackable = true;

            // 设置玩家怪兽区的怪兽不可攻击状态，并重置攻击状态
            foreach (var block in playerBlocks)
            {
                if (block.GetComponent<CardBlock>().monsterCard != null)
                {
                    block.GetComponent<CardBlock>().monsterCard.GetComponent<AttackTarget>().attackable = false;
                    block.GetComponent<CardBlock>().monsterCard.GetComponent<BattleCard>().hasAttacked = false;
                }
                
            }
            // 设置敌人怪兽区的怪兽可攻击状态
            foreach (var block in enemyBlocks)
            {
                if (block.GetComponent<CardBlock>().monsterCard != null)
                {
                    block.GetComponent<CardBlock>().monsterCard.GetComponent<AttackTarget>().attackable = true;
                }
            }
        }
        // 触发阶段变更事件
        currentTurn++;
        phaseChangeEvent.Invoke();
        Debug.Log("当前回合数" +currentTurn);

    }

    // 召唤请求方法，点击手牌时触发
    public void SummonRequst(Vector2 _startPoint, int _player, GameObject _monster)
    {
        // 如果有箭头对象，销毁它
        if (arrow != null)
        {
            Destroy(arrow);
        }
        if (_player == 0 && playerSummonCount >= 1)
        {
            // 玩家有召唤次数时，实例化召唤指示箭头
            arrow = GameObject.Instantiate(arrowPrefab, canvas);
            // 设置箭头的起始点
            arrow.GetComponent<ArrowFollow>().SetStartPoint(_startPoint);
            // 遍历玩家怪兽区的格子，将有空位的格子设置为可召唤状态
            foreach (var block in playerBlocks)
            {
                if (block.GetComponent<CardBlock>().monsterCard == null)
                {
                    block.GetComponent<CardBlock>().SetSummon();
                }
            }
            // 记录等待召唤的怪兽和玩家编号
            waitingMonster = _monster;
            waitingID = _player;
        }
        else if (_player == 1 && enemySummonCount >= 1)
        {
            // 敌人有召唤次数时，实例化召唤指示箭头
            arrow = GameObject.Instantiate(arrowPrefab, canvas);
            // 设置箭头的起始点
            arrow.GetComponent<ArrowFollow>().SetStartPoint(_startPoint);
            // 遍历敌人怪兽区的格子，将有空位的格子设置为可召唤状态
            foreach (var block in enemyBlocks)
            {
                if (block.GetComponent<CardBlock>().monsterCard == null)
                {
                    block.GetComponent<CardBlock>().SetSummon();
                }
            }
            // 记录等待召唤的怪兽和玩家编号
            waitingMonster = _monster;
            waitingID = _player;
        }
    }

    // 召唤确认方法，点击格子时触发
    public void SummonCofirm(Transform _block)
    {
        // 执行召唤操作
        Summon(waitingMonster, waitingID, _block);
        // 清空等待召唤的怪兽对象
        waitingMonster = null;
    }

    /// <summary>
    /// 召唤怪兽的方法
    /// </summary>
    /// <param name="_monster">要召唤的怪兽卡物体</param>
    /// <param name="_id">召唤者编号</param>
    /// <param name="_block">要召唤到的格子节点</param>
    public void Summon(GameObject _monster, int _id, Transform _block)
    {
        // 将怪兽卡的父对象设置为指定格子
        _monster.transform.SetParent(_block);
        // 显示怪兽卡信息
        _monster.GetComponent<CardDisplay>().ShowCard();
        // 将怪兽卡赋值给格子的 monsterCard 属性
        _block.GetComponent<CardBlock>().monsterCard = _monster;
        // 设置怪兽卡的位置为格子的本地原点
        _monster.transform.localPosition = Vector3.zero;
        if (_id == 0)
        {
            // 玩家召唤，设置怪兽卡状态为玩家怪兽区
            _monster.GetComponent<BattleCard>().cardState = CardState.inPlayerBlock;
            // 玩家召唤次数减一
            playerSummonCount--;
            // 关闭玩家怪兽区所有格子的召唤提示
            foreach (var block in playerBlocks)
            {
                block.GetComponent<CardBlock>().CloseAll();
            }
        }
        else if (_id == 1)
        {
            // 敌人召唤，设置怪兽卡状态为敌人怪兽区
            _monster.GetComponent<BattleCard>().cardState = CardState.inEnemyBlock;
            // 敌人召唤次数减一
            enemySummonCount--;
            // 关闭敌人怪兽区所有格子的召唤提示
            foreach (var block in enemyBlocks)
            {
                block.GetComponent<CardBlock>().CloseAll();
            }
        }

        // 如果有箭头对象，销毁它
        if (arrow != null)
        {
            Destroy(arrow);
        }
    }

    // 攻击请求方法，点击怪兽卡时触发
    public void AttackRequst(Vector2 _startPoint, int _player, GameObject _monster)
    {
        if (currentTurn == 1)
        {
            Debug.Log("第一回合不能攻击！");
            return; // 直接返回，不执行后续攻击逻辑
        }

        if (arrow == null)
        {
            arrow = GameObject.Instantiate(attackPrefab, canvas);
        }
        
        arrow.GetComponent<ArrowFollow>().SetStartPoint(_startPoint);

        // 直接攻击条件
        bool strightAttack = true;
        GameObject[] enemyBlocksToCheck = _player == 0 ? enemyBlocks : playerBlocks;
        GameObject enemyIconToTarget = _player == 0 ? enemyIcon : playerIcon;

        foreach (var block in enemyBlocksToCheck)
        {
            if (block.GetComponent<CardBlock>().monsterCard != null)
            {
                block.GetComponent<CardBlock>().SetAttack();
                strightAttack = false;
            }
        }

        if (strightAttack)
        {
            // 可以直接攻击对手玩家，显示指向对方图标的箭头
            // 这里可以根据实际情况调整箭头的显示逻辑
            arrow.GetComponent<ArrowFollow>().endPoint = enemyIconToTarget.transform.position;
            enemyIconToTarget.GetComponent<AttackTarget>().attackable = true;
        }

        attackingMonster = _monster;
        attackingID = _player;
    }

    // 攻击确认方法，点击攻击目标时触发
    public void AttackCofirm(GameObject _target)
    {
        // 执行攻击操作
        Attack(attackingMonster, attackingID, _target);
        // 清空当前正在攻击的怪兽对象
        attackingMonster = null;
    }

    // 攻击方法，处理攻击时的逻辑
    public void Attack(GameObject _monster, int _id, GameObject _target)
    {
        if (arrow != null)
        {
            Destroy(arrow);
        }
        _monster.GetComponent<BattleCard>().hasAttacked = true;
        Debug.Log("攻击成立");

        var attackMonster = _monster.GetComponent<CardDisplay>().card as MonsterCard;

        if (_target == (_id == 0 ? enemyIcon : playerIcon))
        {
            // 直接攻击对方玩家
            if (_id == 0)
            {
                enemyHealthPoint -= attackMonster.attack;
                Debug.Log($"玩家攻击对方玩家，对方剩余生命值: {enemyHealthPoint}");
            }
            else
            {
                playerHealthPoint -= attackMonster.attack;
                Debug.Log($"敌方攻击玩家，玩家剩余生命值: {playerHealthPoint}");
            }
        }
        else
        {
            var targetMonster = _target.GetComponent<CardDisplay>().card as MonsterCard;
            targetMonster.GetDamage(attackMonster.attack);
            if (targetMonster.healthPoint > 0)
            {
                _target.GetComponent<CardDisplay>().ShowCard();
            }
            else
            {
                Destroy(_target);
            }
        }

        foreach (var block in playerBlocks)
        {
            block.GetComponent<CardBlock>().CloseAll();
        }
        foreach (var block in enemyBlocks)
        {
            block.GetComponent<CardBlock>().CloseAll();
        }
    }

    // 游戏开始方法，通常为虚方法，可被子类重写
    public virtual void GameStart()
    {
        playerHealthPoint = 200;
        enemyHealthPoint = 200;
        // 重置玩家和敌人的召唤次数
        playerSummonCount = maxPlayerSummonCount;
        enemySummonCount = maxEnemySummonCount;
        // 获取卡牌数据对象
        CardDate = playerData.GetComponent<CardData>();

        // 设置当前游戏阶段为游戏开始阶段
        currentPhase = GamePhase.gameStart;
        // 读取玩家和敌人的卡组
        ReadDeck();
        // 玩家和敌人各抽五张牌
        DrawCard(0, 5);
        DrawCard(1, 5);
        // 设置当前游戏阶段为玩家抽卡阶段
        Random.Range(0, 1);
        currentPhase = GamePhase.playerDraw;
        currentPhase = GamePhase.enemyDraw;
        
    }

    // 从数据中读取卡组的方法
    public void ReadDeck()
    {
        // 获取玩家数据管理器
        PlayerDataManager pdm = playerData.GetComponent<PlayerDataManager>();
        for (int i = 0; i < pdm.playerDeck.Length; i++)
        {
            if (pdm.playerDeck[i] != 0)
            {
                // 如果玩家卡组中有该卡牌，将其添加到玩家卡组列表中
                int counter = pdm.playerDeck[i];
                for (int j = 0; j < counter; j++)
                {
                    playerDeckList.Add(CardDate.CopyCard(i));
                }
            }
        }
        // 获取敌人数据管理器
        PlayerDataManager edm = enemyData.GetComponent<PlayerDataManager>();
        for (int i = 0; i < edm.playerDeck.Length; i++)
        {
            if (edm.playerDeck[i] != 0)
            {
                // 如果敌人卡组中有该卡牌，将其添加到敌人卡组列表中
                int counter = edm.playerDeck[i];
                for (int j = 0; j < counter; j++)
                {
                    enemyDeckList.Add(CardDate.CopyCard(i));
                }
            }
        }
        // 对玩家和敌人的卡组进行洗牌
        ShuffletDeck(0);
        ShuffletDeck(1);
        // 可以在这里添加输出卡组信息的逻辑，此处注释掉
        //foreach (var item in playerDeckList)
        //{
        //    Debug.Log(item.cardName);
        //}
    }

    // 将卡组洗牌的方法，_player 表示玩家编号（0 为玩家，1 为敌人）
    public void ShuffletDeck(int _player)
    {
        // 洗牌算法的基本思路是遍历整个卡组，对于每一张牌，都和随机的一张牌调换位置。
        switch (_player)
        {
            case 0:
                for (int i = 0; i < playerDeckList.Count; i++)
                {
                    // 随机生成一个索引
                    int rad = Random.Range(0, playerDeckList.Count);
                    // 交换当前卡牌和随机卡牌的位置
                    Card temp = playerDeckList[i];
                    playerDeckList[i] = playerDeckList[rad];
                    playerDeckList[rad] = temp;
                }
                break;
            case 1:
                for (int i = 0; i < enemyDeckList.Count; i++)
                {
                    // 随机生成一个索引
                    int rad = Random.Range(0, enemyDeckList.Count);
                    // 交换当前卡牌和随机卡牌的位置
                    Card temp = enemyDeckList[i];
                    enemyDeckList[i] = enemyDeckList[rad];
                    enemyDeckList[rad] = temp;
                }
                break;
        }
    }
}