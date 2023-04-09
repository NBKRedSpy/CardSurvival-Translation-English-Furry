using System;
using System.Collections.Generic;
using BepInEx;
using HarmonyLib;

namespace 天降福瑞;

[BepInPlugin("Plugin.Furry", "Furry", "1.0.0")]
public class 天降福瑞 : BaseUnityPlugin
{
	public static bool 弹出提示 = true;

	private static List<CardData> fightList = new List<CardData>();

	private void Awake()
	{
		Harmony.CreateAndPatchAll(typeof(天降福瑞));
		base.Logger.LogInfo("Plugin 天降福瑞 is loaded!");
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(GameLoad), "LoadMainGameData")]
	public static void SomePatch()
	{
		CardData fromID = UniqueIDScriptable.GetFromID<CardData>("eae1336ab6ae448288a993160c4a9576");
		if (fromID == null)
		{
			CardData fromID2 = UniqueIDScriptable.GetFromID<CardData>("3ecfa4c4954711edbc22047c16184f06");
			fromID2.StatValues.Clear();
			fromID2.UnlockConditionsDesc.DefaultText = "";
		}
		Dictionary<string, CardData> dictionary = new Dictionary<string, CardData>();
		for (int i = 0; i < GameLoad.Instance.DataBase.AllData.Count; i++)
		{
			if (GameLoad.Instance.DataBase.AllData[i] is CardData)
			{
				dictionary[GameLoad.Instance.DataBase.AllData[i].name] = GameLoad.Instance.DataBase.AllData[i] as CardData;
				if (GameLoad.Instance.DataBase.AllData[i].name.EndsWith("Fight") || GameLoad.Instance.DataBase.AllData[i].name.EndsWith("Raid") || GameLoad.Instance.DataBase.AllData[i].name.EndsWith("RaidCrop"))
				{
					fightList.Add(GameLoad.Instance.DataBase.AllData[i] as CardData);
				}
			}
		}
		if (dictionary.TryGetValue("Bp_Cistern", out var value) && dictionary.TryGetValue("Guil-天降福瑞_新水窖-蓝图", out var value2))
		{
			value.BlueprintCardConditions = value2.BlueprintCardConditions;
			value.BlueprintStatConditions = value2.BlueprintStatConditions;
			value.BlueprintTagConditions = value2.BlueprintTagConditions;
		}
		if (dictionary.TryGetValue("Bp_Cellar", out value) && dictionary.TryGetValue("Guil-天降福瑞_新地窖-蓝图", out value2))
		{
			value.BlueprintCardConditions = value2.BlueprintCardConditions;
			value.BlueprintStatConditions = value2.BlueprintStatConditions;
			value.BlueprintTagConditions = value2.BlueprintTagConditions;
		}
		if (dictionary.TryGetValue("Bp_Well", out value) && dictionary.TryGetValue("Guil-天降福瑞_新水井-蓝图", out value2))
		{
			value.BlueprintCardConditions = value2.BlueprintCardConditions;
			value.BlueprintStatConditions = value2.BlueprintStatConditions;
			value.BlueprintTagConditions = value2.BlueprintTagConditions;
		}
		CardData fromID3 = UniqueIDScriptable.GetFromID<CardData>("3a348d5d10c54ead8523e253e1050565");
		for (int j = 0; j < fightList.Count; j++)
		{
			CardsDropCollection cardsDropCollection = new CardsDropCollection();
			cardsDropCollection.CollectionName = "Thunder";
			cardsDropCollection.CountsAsSuccess = true;
			cardsDropCollection.CollectionWeight = 1;
			bool flag = false;
			CardData cardData = fightList[j];
			if (cardData.DismantleActions.Count > 0)
			{
				for (int k = 0; k < cardData.DismantleActions.Count; k++)
				{
					DismantleCardAction dismantleCardAction = cardData.DismantleActions[k];
					if (dismantleCardAction.ProducedCards.Length != 0)
					{
						for (int l = 0; l < dismantleCardAction.ProducedCards.Length; l++)
						{
							CardsDropCollection cardsDropCollection2 = dismantleCardAction.ProducedCards[l];
							if (cardsDropCollection2.CollectionName == "Success")
							{
								flag = true;
								CardDrop[] value3 = Traverse.Create(cardsDropCollection2).Field("DroppedCards").GetValue<CardDrop[]>();
								Traverse.Create(cardsDropCollection).Field("DroppedCards").SetValue(value3);
								break;
							}
						}
					}
					if (flag)
					{
						break;
					}
				}
			}
			if (flag && fromID3 != null)
			{
				DismantleCardAction dismantleCardAction2 = new DismantleCardAction();
				dismantleCardAction2.ActionName.DefaultText = "以惊雷击碎黑暗！";
				Array.Resize(ref dismantleCardAction2.ProducedCards, 1);
				dismantleCardAction2.ProducedCards[0] = cardsDropCollection;
				dismantleCardAction2.ReceivingCardChanges.ModType = CardModifications.Destroy;
				dismantleCardAction2.RequiredCardsOnBoard = fromID3.DismantleActions[0].RequiredCardsOnBoard;
				cardData.DismantleActions.Add(dismantleCardAction2);
			}
		}
		List<CardData> list = new List<CardData>();
		foreach (KeyValuePair<string, CardData> item in dictionary)
		{
			CardData value4 = item.Value;
			if (value4.CardType == CardTypes.Environment && value4.name.StartsWith("Env_") && !value4.InstancedEnvironment)
			{
				float num = Traverse.Create(value4).Field("MaxWeightCapacity").GetValue<float>();
				if (value4.UniqueID == "00944065989257b40add8b38bdd7b150")
				{
					num = 1f;
				}
				if (num == 0f)
				{
					list.Add(item.Value);
				}
			}
		}
		Random random = new Random();
		int index = random.Next(0, list.Count);
		CharacterPerk fromID4 = UniqueIDScriptable.GetFromID<CharacterPerk>("2ba0171750144a858a71caaf741f138a");
		fromID4.OverrideEnvironment = list[index];
		CardData fromID5 = UniqueIDScriptable.GetFromID<CardData>("5b441d4f587749deb53cd531f03eeb6f");
		CardData fromID6 = UniqueIDScriptable.GetFromID<CardData>("0cec431bdf7d424c8590c10d3605aaf8");
		List<CardData> list2 = new List<CardData>();
		foreach (KeyValuePair<string, CardData> item2 in dictionary)
		{
			if (item2.Key.IndexOf("Seat") > -1)
			{
				CardData value5 = item2.Value;
				Array.Resize(ref value5.CardInteractions, value5.CardInteractions.Length + 1);
				value5.CardInteractions[value5.CardInteractions.Length - 1] = fromID5.CardInteractions[0];
			}
			if ((item2.Key.IndexOf("Luggage") > -1) | (item2.Key.IndexOf("Trunk") > -1) | (item2.Key.IndexOf("Aid") > -1) | (item2.Key.IndexOf("ContainerBag") > -1))
			{
				CardData value6 = item2.Value;
				Array.Resize(ref value6.CardInteractions, value6.CardInteractions.Length + 1);
				value6.CardInteractions[value6.CardInteractions.Length - 1] = fromID6.CardInteractions[0];
			}
		}
	}
}
