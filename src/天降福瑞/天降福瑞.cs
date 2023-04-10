using System;
using System.Collections.Generic;
using BepInEx;
using HarmonyLib;

namespace 天降福瑞
{
    [BepInPlugin("Plugin.Furry", "Furry", "1.0.0")]
public class 天降福瑞 : BaseUnityPlugin
{
	public static bool 弹出提示 = true;


	private void Awake()
	{
			//Plugin startup logic
			//天降福瑞.弹出提示 = base.Config.Bind<bool>("Furry Setting", "Reminder", true, "是否弹出提示").Value;
		Harmony.CreateAndPatchAll(typeof(天降福瑞));
            Logger.LogInfo("Plugin 天降福瑞 is loaded!");
	}


        static List<CardData> fightList = new List<CardData>{};


        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameLoad), "LoadMainGameData")]
        public static void SomePatch()
        {
            /*“天启”不合用警告
            CardData yoyo = UniqueIDScriptable.GetFromID<CardData>("e7c4238794c011edb96204ea56599bd2");
            if (yoyo != null && 弹出提示)
            {
                DialogResult result = MessageBox.Show("天降福瑞mod将大幅度降低游戏难度，为您的游戏体验着想，请尽量不要与天启等高难度mod合用，如执意合用，请点击是，进入游戏，合用后禁止跳脸输出；如不合用，请点击否，游戏将退出",
                    "温馨提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (result == DialogResult.No)
                {
                    Process.GetCurrentProcess().Kill();
                }
            }
            */


            //“御灵”未合用，删除“圣灵药”蓝图
            CardData ayu = UniqueIDScriptable.GetFromID<CardData>("eae1336ab6ae448288a993160c4a9576");
            if (ayu == null)
            {
                CardData bpDrug = UniqueIDScriptable.GetFromID<CardData>("3ecfa4c4954711edbc22047c16184f06");
                bpDrug.StatValues.Clear();
                bpDrug.UnlockConditionsDesc.DefaultText = "";

            }

            //读取所有卡
            Dictionary<string, CardData> card_dict = new Dictionary<string, CardData>();
            for (int i = 0; i < GameLoad.Instance.DataBase.AllData.Count; i++)
            {
                if (GameLoad.Instance.DataBase.AllData[i] is CardData)
                {
                    //card_dict.Add(GameLoad.Instance.DataBase.AllData[i].name, GameLoad.Instance.DataBase.AllData[i] as CardData);
                    card_dict[GameLoad.Instance.DataBase.AllData[i].name] = GameLoad.Instance.DataBase.AllData[i] as CardData;
                    if ((GameLoad.Instance.DataBase.AllData[i].name.EndsWith("Fight")) || (GameLoad.Instance.DataBase.AllData[i].name.EndsWith("Raid")) || (GameLoad.Instance.DataBase.AllData[i].name.EndsWith("RaidCrop"))){
                        fightList.Add(GameLoad.Instance.DataBase.AllData[i] as CardData);
				}
			}
            }

            //================================修改水窖、地窖、水井=============================
            if (card_dict.TryGetValue("Bp_Cistern", out CardData a) && card_dict.TryGetValue("Guil-天降福瑞_新水窖-蓝图", out CardData b)) //水窖
            {
                a.BlueprintCardConditions = b.BlueprintCardConditions;
                a.BlueprintStatConditions = b.BlueprintStatConditions;
                a.BlueprintTagConditions = b.BlueprintTagConditions;
            }
            if (card_dict.TryGetValue("Bp_Cellar", out  a) && card_dict.TryGetValue("Guil-天降福瑞_新地窖-蓝图", out  b)) //地窖
            {
                a.BlueprintCardConditions = b.BlueprintCardConditions;
                a.BlueprintStatConditions = b.BlueprintStatConditions;
                a.BlueprintTagConditions = b.BlueprintTagConditions;
            }
            if (card_dict.TryGetValue("Bp_Well", out  a) && card_dict.TryGetValue("Guil-天降福瑞_新水井-蓝图", out  b)) //水井
            {
                a.BlueprintCardConditions = b.BlueprintCardConditions;
                a.BlueprintStatConditions = b.BlueprintStatConditions;
                a.BlueprintTagConditions = b.BlueprintTagConditions;
            }

			//================================惊雷印记========================================
			CardData Muban2 = UniqueIDScriptable.GetFromID<CardData>("3a348d5d10c54ead8523e253e1050565");            
            for (int i = 0; i < fightList.Count; i++)
            {
                CardsDropCollection cdc = new CardsDropCollection();
                cdc.CollectionName = "Thunder";
                cdc.CountsAsSuccess = true;
                cdc.CollectionWeight = 1;
                
                
                bool findSuccess = false;

                //遍历dis找一个名叫“success”的cdc
                CardData a1 = fightList[i];
                if(a1.DismantleActions.Count > 0)
                {
                    for(int j = 0;j < a1.DismantleActions.Count;j++)
                    {
                        DismantleCardAction d1 = a1.DismantleActions[j];
                        if(d1.ProducedCards.Length > 0)
                        {
                            for (int k = 0; k < d1.ProducedCards.Length; k++)
                            {
                                CardsDropCollection c1 = d1.ProducedCards[k];
                                if (c1.CollectionName == "Success")
                                {
                                    findSuccess = true;
                                    CardDrop[] DroppedCards1 = Traverse.Create(c1).Field("DroppedCards").GetValue<CardDrop[]>();
                                    Traverse.Create(cdc).Field("DroppedCards").SetValue(DroppedCards1);
                                    break;
                                }
                            }
                        }
                        if(findSuccess)
                        {
                            break;
                        }
				}
			}
                if (findSuccess && Muban2 != null)
			{
                    DismantleCardAction dis = new DismantleCardAction();
                    dis.ActionName.DefaultText = "以惊雷击碎黑暗！";
				    dis.ActionName.LocalizationKey = "LightWithThunderbolt";
                    Array.Resize(ref dis.ProducedCards, 1);
                    dis.ProducedCards[0] = (cdc);
                    dis.ReceivingCardChanges.ModType = CardModifications.Destroy;
                    dis.RequiredCardsOnBoard = Muban2.DismantleActions[0].RequiredCardsOnBoard;
                    a1.DismantleActions.Add(dis);

			}
		}
            
            //================================随机开局========================================
            List<CardData> PlaceList = new List<CardData> {};

            foreach (KeyValuePair<string, CardData> kvp in card_dict)
            {
                CardData carda = kvp.Value;
                if (carda.CardType == CardTypes.Environment && carda.name.StartsWith("Env_") && carda.InstancedEnvironment == false)                 
                {
                    //CardDrop[] DroppedCards1 = Traverse.Create(c1).Field("DroppedCards").GetValue<CardDrop[]>();
                    Single mwc = Traverse.Create(carda).Field("MaxWeightCapacity").GetValue<Single>();
                    if(carda.UniqueID == "00944065989257b40add8b38bdd7b150") { mwc = 1; } //把“酸湖”从起始地点中拿出去                  
                    if (mwc == 0)
                    {
                        PlaceList.Add(kvp.Value);
                    }                  
                }
            }

            //假如 list里有5个图 序号是0到4 count是5
            System.Random rnum = new System.Random();
            int rnum2 = rnum.Next(0, PlaceList.Count);

            //修改特性
            CharacterPerk randomStart = UniqueIDScriptable.GetFromID<CharacterPerk>("2ba0171750144a858a71caaf741f138a");
            randomStart.OverrideEnvironment = PlaceList[rnum2];
            
            //================================坠机拆除========================================
            CardData 座椅模板 = UniqueIDScriptable.GetFromID<CardData>("5b441d4f587749deb53cd531f03eeb6f");
            CardData 行李箱模板 = UniqueIDScriptable.GetFromID<CardData>("0cec431bdf7d424c8590c10d3605aaf8");
			List<CardData> SeatList = new List<CardData> {  };
			foreach (KeyValuePair<string, CardData> kvp in card_dict) {
				if(kvp.Key.IndexOf("Seat") > -1) {
                    CardData card = kvp.Value;
                    Array.Resize(ref card.CardInteractions, card.CardInteractions.Length + 1);
                    card.CardInteractions[card.CardInteractions.Length -1] = 座椅模板.CardInteractions[0];
				}
				if ((kvp.Key.IndexOf("Luggage") > -1) | (kvp.Key.IndexOf("Trunk") > -1) | (kvp.Key.IndexOf("Aid") > -1) | (kvp.Key.IndexOf("ContainerBag") > -1) | (kvp.Key.IndexOf("箱") > -1)) {
					CardData card = kvp.Value;
					Array.Resize(ref card.CardInteractions, card.CardInteractions.Length + 1);
					card.CardInteractions[card.CardInteractions.Length - 1] = 行李箱模板.CardInteractions[0];
				}
			}
		}
	}
}
