using System;
using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace 天降福瑞;

[BepInPlugin("Plugin.Furry", "Furry", "1.0.0")]
public class 天降福瑞 : BaseUnityPlugin
{
	public static bool 弹出提示 = true;

	private static List<CardData> fightList = new List<CardData>();

	private void Awake()
	{
		Harmony.CreateAndPatchAll(typeof(天降福瑞), (string)null);
		((BaseUnityPlugin)this).Logger.LogInfo((object)"Plugin 天降福瑞 is loaded!");
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(GameLoad), "LoadMainGameData")]
	public static void SomePatch()
	{
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Expected O, but got Unknown
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Expected O, but got Unknown
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		//IL_0466: Invalid comparison between Unknown and I4
		CardData fromID = UniqueIDScriptable.GetFromID<CardData>("eae1336ab6ae448288a993160c4a9576");
		if ((Object)(object)fromID == (Object)null)
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
				string name = ((Object)GameLoad.Instance.DataBase.AllData[i]).name;
				UniqueIDScriptable obj = GameLoad.Instance.DataBase.AllData[i];
				dictionary[name] = (CardData)(object)((obj is CardData) ? obj : null);
				if (((Object)GameLoad.Instance.DataBase.AllData[i]).name.EndsWith("Fight") || ((Object)GameLoad.Instance.DataBase.AllData[i]).name.EndsWith("Raid") || ((Object)GameLoad.Instance.DataBase.AllData[i]).name.EndsWith("RaidCrop"))
				{
					List<CardData> list = fightList;
					UniqueIDScriptable obj2 = GameLoad.Instance.DataBase.AllData[i];
					list.Add((CardData)(object)((obj2 is CardData) ? obj2 : null));
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
			CardsDropCollection val = new CardsDropCollection();
			val.CollectionName = "Thunder";
			val.CountsAsSuccess = true;
			val.CollectionWeight = 1;
			bool flag = false;
			CardData val2 = fightList[j];
			if (val2.DismantleActions.Count > 0)
			{
				for (int k = 0; k < val2.DismantleActions.Count; k++)
				{
					DismantleCardAction val3 = val2.DismantleActions[k];
					if (((CardAction)val3).ProducedCards.Length != 0)
					{
						for (int l = 0; l < ((CardAction)val3).ProducedCards.Length; l++)
						{
							CardsDropCollection val4 = ((CardAction)val3).ProducedCards[l];
							if (val4.CollectionName == "Success")
							{
								flag = true;
								CardDrop[] value3 = Traverse.Create((object)val4).Field("DroppedCards").GetValue<CardDrop[]>();
								Traverse.Create((object)val).Field("DroppedCards").SetValue((object)value3);
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
			if (flag && (Object)(object)fromID3 != (Object)null)
			{
				DismantleCardAction val5 = new DismantleCardAction();
				((CardAction)val5).ActionName.DefaultText = "以惊雷击碎黑暗！";
				Array.Resize(ref ((CardAction)val5).ProducedCards, 1);
				((CardAction)val5).ProducedCards[0] = val;
				((CardAction)val5).ReceivingCardChanges.ModType = (CardModifications)3;
				((CardAction)val5).RequiredCardsOnBoard = ((CardAction)fromID3.DismantleActions[0]).RequiredCardsOnBoard;
				val2.DismantleActions.Add(val5);
			}
		}
		List<CardData> list2 = new List<CardData>();
		foreach (KeyValuePair<string, CardData> item in dictionary)
		{
			CardData value4 = item.Value;
			if ((int)value4.CardType == 4 && ((Object)value4).name.StartsWith("Env_") && !value4.InstancedEnvironment)
			{
				float num = Traverse.Create((object)value4).Field("MaxWeightCapacity").GetValue<float>();
				if (((UniqueIDScriptable)value4).UniqueID == "00944065989257b40add8b38bdd7b150")
				{
					num = 1f;
				}
				if (num == 0f)
				{
					list2.Add(item.Value);
				}
			}
		}
		Random random = new Random();
		int index = random.Next(0, list2.Count);
		CharacterPerk fromID4 = UniqueIDScriptable.GetFromID<CharacterPerk>("2ba0171750144a858a71caaf741f138a");
		fromID4.OverrideEnvironment = list2[index];
		CardData fromID5 = UniqueIDScriptable.GetFromID<CardData>("5b441d4f587749deb53cd531f03eeb6f");
		CardData fromID6 = UniqueIDScriptable.GetFromID<CardData>("0cec431bdf7d424c8590c10d3605aaf8");
		List<CardData> list3 = new List<CardData>();
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
