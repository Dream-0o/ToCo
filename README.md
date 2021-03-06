# Toxic Code: Chaos Shards

- This is a card game developed by Unity 3D. 

## Intoduction ##

### 主界面 ###
![image](img/1.png)
- 主界面是打开游戏后，用户见到的第一个界面。主界面上有5个按钮：
- 继续游戏：从上次退出游戏的回合继续进行游戏
- 故事模式：打开地图。在地图中，你可以点击地图上的元素来观看剧情对话或进行卡牌战斗
- 卡牌收藏：显示你所收集到的所有卡牌
- 成就：显示你已经和尚未完成的成就
- 设置：进行游戏设置
- 除主界面外，其它界面都拥有返回按钮，点击返回按钮可以回到主界面。

### 地图 ###
![image](img/2.png)
- 将鼠标移动到地图上的区域（一共有四个区域），选中的区域会高亮显示。点击鼠标，可以进入对应的主线剧情关卡。
- 点击地图上的人物图标，可以进入支线剧情。
- 点击左上角罗盘，可以返回主界面。

### 剧情 ###

![image](img/3.png)
- 点击鼠标来观看剧情对话。

### 对战 ###
![image](img/4.jpg)
- ①-牌堆，下面的数字代表牌堆剩余牌数，你的手牌从这里抽取，每回合抽2张牌。如果牌堆没有牌了，会自动将用过的牌洗成新的牌堆。
- ②-你的人物，上方有生命和护盾的显示部件，人物所站位置与当前距离有关。
- ③-你的手牌，没有上限，但是回合结束时需要弃至2张
- ④-版图，在双方进行距离操作时，版图会增加或是减少。
- ⑤-信息栏，包含时间，回合数和距离的信息。
- ⑥-设置，可以进入设置页面。（为方便测试，此版本该按钮会导向大地图）
- ⑦-强化区，将标有advance的卡牌拖拽至这里，如果能量值够且满足出牌条件的话就会打出加强效果
- ⑧-行动区，可以消耗行动力进行前进或者后退。注意：前进会增加1护盾，护盾满则不能再前进；后退则会消耗1护盾，无护盾时不能后退。可以将手牌拖拽至此区域，消耗手牌来获得一点行动力。行动力每回合也会自动恢复1（上限为3）
- ⑨-回合结束按钮，回合结束时，如果手牌数大于2，则要弃至2张。
- 在对战中，你可以在自己的回合执行以下操作：
- 1. 打出一张手牌，需要当前距离符合手牌上的数值
- 2. 消耗能量，打出卡牌的强化效果
- 3. 消耗行动力，前进或者后退
- 敌人会在你的回合开始时，根据当前距离选择下回合的行动，因此，你可以根据这一规则来预测敌人的行动，借此选择上前攻击或者退避。
- 当敌人生命为0时，你获得胜利。如果你的生命为0，则会输掉战斗。


## Team members ##
- @[FlouriteJ](https://github.com/FlouriteJ)
- @[codenie](https://github.com/codenie)
- @[lingjiameng](https://github.com/lingjiameng)
- @[melting514](https://github.com/melting514)
- @[zhaoxuey](https://github.com/zhaoxuey)