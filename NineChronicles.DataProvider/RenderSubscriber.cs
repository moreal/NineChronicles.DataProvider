namespace NineChronicles.DataProvider
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Bencodex.Types;
    using Lib9c.Model.Order;
    using Lib9c.Renderer;
    using Libplanet;
    using Libplanet.Action;
    using Libplanet.Assets;
    using Microsoft.Extensions.Hosting;
    using Nekoyume;
    using Nekoyume.Action;
    using Nekoyume.Battle;
    using Nekoyume.Extensions;
    using Nekoyume.Model.Item;
    using Nekoyume.Model.State;
    using Nekoyume.TableData;
    using NineChronicles.DataProvider.Store;
    using NineChronicles.DataProvider.Store.Models;
    using NineChronicles.Headless;
    using Serilog;

    public class RenderSubscriber : BackgroundService
    {
        private const int InsertInterval = 500;
        private readonly BlockRenderer _blockRenderer;
        private readonly ActionRenderer _actionRenderer;
        private readonly ExceptionRenderer _exceptionRenderer;
        private readonly NodeStatusRenderer _nodeStatusRenderer;
        private readonly List<AgentModel> _hasAgentList = new List<AgentModel>();
        private readonly List<AvatarModel> _hasAvatarList = new List<AvatarModel>();
        private readonly List<HackAndSlashModel> _hasList = new List<HackAndSlashModel>();
        private readonly List<AgentModel> _rbAgentList = new List<AgentModel>();
        private readonly List<AvatarModel> _rbAvatarList = new List<AvatarModel>();
        private readonly List<AgentModel> _ccAgentList = new List<AgentModel>();
        private readonly List<AvatarModel> _ccAvatarList = new List<AvatarModel>();
        private readonly List<CombinationConsumableModel> _ccList = new List<CombinationConsumableModel>();
        private readonly List<AgentModel> _ceAgentList = new List<AgentModel>();
        private readonly List<AvatarModel> _ceAvatarList = new List<AvatarModel>();
        private readonly List<CombinationEquipmentModel> _ceList = new List<CombinationEquipmentModel>();
        private readonly List<AgentModel> _eqAgentList = new List<AgentModel>();
        private readonly List<AvatarModel> _eqAvatarList = new List<AvatarModel>();
        private readonly List<EquipmentModel> _eqList = new List<EquipmentModel>();
        private readonly List<AgentModel> _ieAgentList = new List<AgentModel>();
        private readonly List<AvatarModel> _ieAvatarList = new List<AvatarModel>();
        private readonly List<ItemEnhancementModel> _ieList = new List<ItemEnhancementModel>();
        private readonly List<AgentModel> _buyAgentList = new List<AgentModel>();
        private readonly List<AvatarModel> _buyAvatarList = new List<AvatarModel>();
        private readonly List<ShopHistoryEquipmentModel> _buyShopEquipmentsList = new List<ShopHistoryEquipmentModel>();
        private readonly List<ShopHistoryCostumeModel> _buyShopCostumesList = new List<ShopHistoryCostumeModel>();
        private readonly List<ShopHistoryMaterialModel> _buyShopMaterialsList = new List<ShopHistoryMaterialModel>();
        private readonly List<ShopHistoryConsumableModel> _buyShopConsumablesList = new List<ShopHistoryConsumableModel>();
        private readonly List<AgentModel> _stakeAgentList = new List<AgentModel>();
        private readonly List<StakeModel> _stakeList = new List<StakeModel>();
        private readonly List<AgentModel> _claimStakeAgentList = new List<AgentModel>();
        private readonly List<AvatarModel> _claimStakeAvatarList = new List<AvatarModel>();
        private readonly List<ClaimStakeRewardModel> _claimStakeList = new List<ClaimStakeRewardModel>();
        private readonly List<AgentModel> _mmcAgentList = new List<AgentModel>();
        private readonly List<MigrateMonsterCollectionModel> _mmcList = new List<MigrateMonsterCollectionModel>();
        private int _renderCount = 0;

        public RenderSubscriber(
            NineChroniclesNodeService nodeService,
            MySqlStore mySqlStore
        )
        {
            _blockRenderer = nodeService.BlockRenderer;
            _actionRenderer = nodeService.ActionRenderer;
            _exceptionRenderer = nodeService.ExceptionRenderer;
            _nodeStatusRenderer = nodeService.NodeStatusRenderer;
            MySqlStore = mySqlStore;
        }

        internal MySqlStore MySqlStore { get; }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _actionRenderer.EveryRender<ActionBase>()
                .Subscribe(
                    ev =>
                    {
                        try
                        {
                            if (ev.Exception != null)
                            {
                                return;
                            }

                            if (_renderCount == InsertInterval)
                            {
                                var start = DateTimeOffset.Now;
                                Log.Debug("Storing Data");
                                MySqlStore.StoreAgentList(_hasAgentList.GroupBy(i => i.Address).Select(i => i.FirstOrDefault()).ToList());
                                MySqlStore.StoreAvatarList(_hasAvatarList.GroupBy(i => i.Address).Select(i => i.FirstOrDefault()).ToList());
                                MySqlStore.StoreHackAndSlashList(_hasList.GroupBy(i => i.Id).Select(i => i.FirstOrDefault()).ToList());
                                MySqlStore.StoreAgentList(_rbAgentList.GroupBy(i => i.Address).Select(i => i.FirstOrDefault()).ToList());
                                MySqlStore.StoreAvatarList(_rbAvatarList.GroupBy(i => i.Address).Select(i => i.FirstOrDefault()).ToList());
                                MySqlStore.StoreAgentList(_ccAgentList.GroupBy(i => i.Address).Select(i => i.FirstOrDefault()).ToList());
                                MySqlStore.StoreAvatarList(_ccAvatarList.GroupBy(i => i.Address).Select(i => i.FirstOrDefault()).ToList());
                                MySqlStore.StoreCombinationConsumableList(_ccList.GroupBy(i => i.Id).Select(i => i.FirstOrDefault()).ToList());
                                MySqlStore.StoreAgentList(_ceAgentList.GroupBy(i => i.Address).Select(i => i.FirstOrDefault()).ToList());
                                MySqlStore.StoreAvatarList(_ceAvatarList.GroupBy(i => i.Address).Select(i => i.FirstOrDefault()).ToList());
                                MySqlStore.StoreCombinationEquipmentList(_ceList.GroupBy(i => i.Id).Select(i => i.FirstOrDefault()).ToList());
                                MySqlStore.StoreAgentList(_ieAgentList.GroupBy(i => i.Address).Select(i => i.FirstOrDefault()).ToList());
                                MySqlStore.StoreAvatarList(_ieAvatarList.GroupBy(i => i.Address).Select(i => i.FirstOrDefault()).ToList());
                                MySqlStore.StoreItemEnhancementList(_ieList.GroupBy(i => i.Id).Select(i => i.FirstOrDefault()).ToList());
                                MySqlStore.StoreAgentList(_buyAgentList.GroupBy(i => i.Address).Select(i => i.FirstOrDefault()).ToList());
                                MySqlStore.StoreAvatarList(_buyAvatarList.GroupBy(i => i.Address).Select(i => i.FirstOrDefault()).ToList());
                                MySqlStore.StoreShopHistoryEquipmentList(_buyShopEquipmentsList.GroupBy(i => i.OrderId).Select(i => i.FirstOrDefault()).ToList());
                                MySqlStore.StoreShopHistoryCostumeList(_buyShopCostumesList.GroupBy(i => i.OrderId).Select(i => i.FirstOrDefault()).ToList());
                                MySqlStore.StoreShopHistoryMaterialList(_buyShopMaterialsList.GroupBy(i => i.OrderId).Select(i => i.FirstOrDefault()).ToList());
                                MySqlStore.StoreShopHistoryConsumableList(_buyShopConsumablesList.GroupBy(i => i.OrderId).Select(i => i.FirstOrDefault()).ToList());
                                MySqlStore.StoreAgentList(_eqAgentList.GroupBy(i => i.Address).Select(i => i.FirstOrDefault()).ToList());
                                MySqlStore.StoreAvatarList(_eqAvatarList.GroupBy(i => i.Address).Select(i => i.FirstOrDefault()).ToList());
                                MySqlStore.ProcessEquipmentList(_eqList.GroupBy(i => i.ItemId).Select(i => i.FirstOrDefault()).ToList());
                                MySqlStore.StoreAgentList(_stakeAgentList.GroupBy(i => i.Address).Select(i => i.FirstOrDefault()).ToList());
                                MySqlStore.StoreStakingList(_stakeList.ToList());
                                MySqlStore.StoreAgentList(_claimStakeAgentList.GroupBy(i => i.Address).Select(i => i.FirstOrDefault()).ToList());
                                MySqlStore.StoreAvatarList(_claimStakeAvatarList.GroupBy(i => i.Address).Select(i => i.FirstOrDefault()).ToList());
                                MySqlStore.StoreClaimStakeRewardList(_claimStakeList.ToList());
                                MySqlStore.StoreAgentList(_mmcAgentList.GroupBy(i => i.Address).Select(i => i.FirstOrDefault()).ToList());
                                MySqlStore.StoreMigrateMonsterCollectionList(_mmcList.ToList());
                                _renderCount = 0;
                                _hasAgentList.Clear();
                                _hasAvatarList.Clear();
                                _hasList.Clear();
                                _rbAgentList.Clear();
                                _rbAvatarList.Clear();
                                _ccAgentList.Clear();
                                _ccAvatarList.Clear();
                                _ccList.Clear();
                                _ceAgentList.Clear();
                                _ceAvatarList.Clear();
                                _ceList.Clear();
                                _ieAgentList.Clear();
                                _ieAvatarList.Clear();
                                _ieList.Clear();
                                _buyAgentList.Clear();
                                _buyAvatarList.Clear();
                                _buyShopEquipmentsList.Clear();
                                _buyShopCostumesList.Clear();
                                _buyShopMaterialsList.Clear();
                                _buyShopConsumablesList.Clear();
                                _eqAgentList.Clear();
                                _eqAvatarList.Clear();
                                _eqList.Clear();
                                _stakeAgentList.Clear();
                                _stakeList.Clear();
                                _claimStakeAgentList.Clear();
                                _claimStakeAvatarList.Clear();
                                _claimStakeList.Clear();
                                _mmcAgentList.Clear();
                                _mmcList.Clear();
                                var end = DateTimeOffset.Now;
                                Log.Debug($"Storing Data Complete. Time Taken: {(end - start).Milliseconds} ms.");
                            }

                            if (ev.Action is HackAndSlash has)
                            {
                                _renderCount++;
                                Log.Debug($"Render Count: #{_renderCount}");
                                var start = DateTimeOffset.Now;
                                AvatarState avatarState = ev.OutputStates.GetAvatarStateV2(has.avatarAddress);
                                var previousStates = ev.PreviousStates;
                                var characterSheet = previousStates.GetSheet<CharacterSheet>();
                                var avatarLevel = avatarState.level;
                                var avatarArmorId = avatarState.GetArmorId();
                                var avatarTitleCostume = avatarState.inventory.Costumes.FirstOrDefault(costume => costume.ItemSubType == ItemSubType.Title && costume.equipped);
                                int? avatarTitleId = null;
                                if (avatarTitleCostume != null)
                                {
                                    avatarTitleId = avatarTitleCostume.Id;
                                }

                                var avatarCp = CPHelper.GetCP(avatarState, characterSheet);
                                string avatarName = avatarState.name;

                                Log.Debug(
                                    "AvatarName: {0}, AvatarLevel: {1}, ArmorId: {2}, TitleId: {3}, CP: {4}",
                                    avatarName,
                                    avatarLevel,
                                    avatarArmorId,
                                    avatarTitleId,
                                    avatarCp);

                                bool isClear = avatarState.stageMap.ContainsKey(has.stageId);

                                _hasAgentList.Add(new AgentModel()
                                {
                                    Address = ev.Signer.ToString(),
                                });
                                _hasAvatarList.Add(new AvatarModel()
                                {
                                    Address = has.avatarAddress.ToString(),
                                    AgentAddress = ev.Signer.ToString(),
                                    Name = avatarName,
                                    AvatarLevel = avatarLevel,
                                    TitleId = avatarTitleId,
                                    ArmorId = avatarArmorId,
                                    Cp = avatarCp,
                                });
                                _hasList.Add(new HackAndSlashModel()
                                {
                                    Id = has.Id.ToString(),
                                    AgentAddress = ev.Signer.ToString(),
                                    AvatarAddress = has.avatarAddress.ToString(),
                                    StageId = has.stageId,
                                    Cleared = isClear,
                                    Mimisbrunnr = has.stageId > 10000000,
                                    BlockIndex = ev.BlockIndex,
                                });

                                var end = DateTimeOffset.Now;
                                Log.Debug("Stored HackAndSlash action in block #{index}. Time Taken: {time} ms.", ev.BlockIndex, (end - start).Milliseconds);
                            }

                            if (ev.Action is RankingBattle rb)
                            {
                                _renderCount++;
                                Log.Debug($"Render Count: #{_renderCount}");
                                var start = DateTimeOffset.Now;
                                AvatarState avatarState = ev.OutputStates.GetAvatarStateV2(rb.avatarAddress);
                                var previousStates = ev.PreviousStates;
                                var characterSheet = previousStates.GetSheet<CharacterSheet>();
                                var avatarLevel = avatarState.level;
                                var avatarArmorId = avatarState.GetArmorId();
                                var avatarTitleCostume = avatarState.inventory.Costumes.FirstOrDefault(costume => costume.ItemSubType == ItemSubType.Title && costume.equipped);
                                int? avatarTitleId = null;
                                if (avatarTitleCostume != null)
                                {
                                    avatarTitleId = avatarTitleCostume.Id;
                                }

                                var avatarCp = CPHelper.GetCP(avatarState, characterSheet);
                                string avatarName = avatarState.name;

                                Log.Debug(
                                    "AvatarName: {0}, AvatarLevel: {1}, ArmorId: {2}, TitleId: {3}, CP: {4}",
                                    avatarName,
                                    avatarLevel,
                                    avatarArmorId,
                                    avatarTitleId,
                                    avatarCp);

                                _rbAgentList.Add(new AgentModel()
                                {
                                    Address = ev.Signer.ToString(),
                                });
                                _rbAvatarList.Add(new AvatarModel()
                                {
                                    Address = rb.avatarAddress.ToString(),
                                    AgentAddress = ev.Signer.ToString(),
                                    Name = avatarName,
                                    AvatarLevel = avatarLevel,
                                    TitleId = avatarTitleId,
                                    ArmorId = avatarArmorId,
                                    Cp = avatarCp,
                                });

                                var end = DateTimeOffset.Now;
                                Log.Debug("Stored RankingBattle avatar data in block #{index}. Time Taken: {time} ms.", ev.BlockIndex, (end - start).Milliseconds);
                            }

                            if (ev.Action is CombinationConsumable combinationConsumable)
                            {
                                _renderCount++;
                                Log.Debug($"Render Count: #{_renderCount}");
                                var start = DateTimeOffset.Now;
                                AvatarState avatarState = ev.OutputStates.GetAvatarStateV2(combinationConsumable.avatarAddress);
                                var previousStates = ev.PreviousStates;
                                var characterSheet = previousStates.GetSheet<CharacterSheet>();
                                var avatarLevel = avatarState.level;
                                var avatarArmorId = avatarState.GetArmorId();
                                var avatarTitleCostume = avatarState.inventory.Costumes.FirstOrDefault(costume => costume.ItemSubType == ItemSubType.Title && costume.equipped);
                                int? avatarTitleId = null;
                                if (avatarTitleCostume != null)
                                {
                                    avatarTitleId = avatarTitleCostume.Id;
                                }

                                var avatarCp = CPHelper.GetCP(avatarState, characterSheet);
                                string avatarName = avatarState.name;

                                _ccAgentList.Add(new AgentModel()
                                {
                                    Address = ev.Signer.ToString(),
                                });
                                _ccAvatarList.Add(new AvatarModel()
                                {
                                    Address = combinationConsumable.avatarAddress.ToString(),
                                    AgentAddress = ev.Signer.ToString(),
                                    Name = avatarName,
                                    AvatarLevel = avatarLevel,
                                    TitleId = avatarTitleId,
                                    ArmorId = avatarArmorId,
                                    Cp = avatarCp,
                                });
                                _ccList.Add(new CombinationConsumableModel()
                                {
                                    Id = combinationConsumable.Id.ToString(),
                                    AgentAddress = ev.Signer.ToString(),
                                    AvatarAddress = combinationConsumable.avatarAddress.ToString(),
                                    RecipeId = combinationConsumable.recipeId,
                                    SlotIndex = combinationConsumable.slotIndex,
                                    BlockIndex = ev.BlockIndex,
                                });

                                var end = DateTimeOffset.Now;
                                Log.Debug("Stored CombinationConsumable action in block #{index}. Time Taken: {time} ms.", ev.BlockIndex, (end - start).Milliseconds);
                            }

                            if (ev.Action is CombinationEquipment combinationEquipment)
                            {
                                _renderCount++;
                                Log.Debug($"Render Count: #{_renderCount}");
                                var start = DateTimeOffset.Now;
                                AvatarState avatarState = ev.OutputStates.GetAvatarStateV2(combinationEquipment.avatarAddress);
                                var previousStates = ev.PreviousStates;
                                var characterSheet = previousStates.GetSheet<CharacterSheet>();
                                var avatarLevel = avatarState.level;
                                var avatarArmorId = avatarState.GetArmorId();
                                var avatarTitleCostume = avatarState.inventory.Costumes.FirstOrDefault(costume => costume.ItemSubType == ItemSubType.Title && costume.equipped);
                                int? avatarTitleId = null;
                                if (avatarTitleCostume != null)
                                {
                                    avatarTitleId = avatarTitleCostume.Id;
                                }

                                var avatarCp = CPHelper.GetCP(avatarState, characterSheet);
                                string avatarName = avatarState.name;

                                _ceAgentList.Add(new AgentModel()
                                {
                                    Address = ev.Signer.ToString(),
                                });
                                _ceAvatarList.Add(new AvatarModel()
                                {
                                    Address = combinationEquipment.avatarAddress.ToString(),
                                    AgentAddress = ev.Signer.ToString(),
                                    Name = avatarName,
                                    AvatarLevel = avatarLevel,
                                    TitleId = avatarTitleId,
                                    ArmorId = avatarArmorId,
                                    Cp = avatarCp,
                                });
                                _ceList.Add(new CombinationEquipmentModel()
                                {
                                    Id = combinationEquipment.Id.ToString(),
                                    AgentAddress = ev.Signer.ToString(),
                                    AvatarAddress = combinationEquipment.avatarAddress.ToString(),
                                    RecipeId = combinationEquipment.recipeId,
                                    SlotIndex = combinationEquipment.slotIndex,
                                    SubRecipeId = combinationEquipment.subRecipeId ?? 0,
                                    BlockIndex = ev.BlockIndex,
                                });

                                var end = DateTimeOffset.Now;
                                Log.Debug("Stored CombinationEquipment action in block #{index}. Time Taken: {time} ms.", ev.BlockIndex, (end - start).Milliseconds);
                                start = DateTimeOffset.Now;

                                var slotState = ev.OutputStates.GetCombinationSlotState(
                                    combinationEquipment.avatarAddress,
                                    combinationEquipment.slotIndex);

                                if (slotState?.Result.itemUsable.ItemType is ItemType.Equipment)
                                {
                                    ProcessEquipmentData(
                                         ev.Signer,
                                         combinationEquipment.avatarAddress,
                                         avatarName,
                                         avatarLevel,
                                         avatarTitleId,
                                         avatarArmorId,
                                         avatarCp,
                                         (Equipment)slotState.Result.itemUsable);
                                }

                                end = DateTimeOffset.Now;
                                Log.Debug(
                                    "Stored avatar {address}'s equipment in block #{index}. Time Taken: {time} ms.",
                                    combinationEquipment.avatarAddress,
                                    ev.BlockIndex,
                                    (end - start).Milliseconds);
                            }

                            if (ev.Action is ItemEnhancement itemEnhancement)
                            {
                                _renderCount++;
                                Log.Debug($"Render Count: #{_renderCount}");
                                var start = DateTimeOffset.Now;
                                AvatarState avatarState = ev.OutputStates.GetAvatarStateV2(itemEnhancement.avatarAddress);
                                var previousStates = ev.PreviousStates;
                                var characterSheet = previousStates.GetSheet<CharacterSheet>();
                                var avatarLevel = avatarState.level;
                                var avatarArmorId = avatarState.GetArmorId();
                                var avatarTitleCostume = avatarState.inventory.Costumes.FirstOrDefault(costume => costume.ItemSubType == ItemSubType.Title && costume.equipped);
                                int? avatarTitleId = null;
                                if (avatarTitleCostume != null)
                                {
                                    avatarTitleId = avatarTitleCostume.Id;
                                }

                                var avatarCp = CPHelper.GetCP(avatarState, characterSheet);
                                string avatarName = avatarState.name;

                                _ieAgentList.Add(new AgentModel()
                                {
                                    Address = ev.Signer.ToString(),
                                });
                                _ieAvatarList.Add(new AvatarModel()
                                {
                                    Address = itemEnhancement.avatarAddress.ToString(),
                                    AgentAddress = ev.Signer.ToString(),
                                    Name = avatarName,
                                    AvatarLevel = avatarLevel,
                                    TitleId = avatarTitleId,
                                    ArmorId = avatarArmorId,
                                    Cp = avatarCp,
                                });
                                _ieList.Add(new ItemEnhancementModel()
                                {
                                    Id = itemEnhancement.Id.ToString(),
                                    AgentAddress = ev.Signer.ToString(),
                                    AvatarAddress = itemEnhancement.avatarAddress.ToString(),
                                    ItemId = itemEnhancement.itemId.ToString(),
                                    MaterialId = itemEnhancement.materialId.ToString(),
                                    SlotIndex = itemEnhancement.slotIndex,
                                    BlockIndex = ev.BlockIndex,
                                });

                                var end = DateTimeOffset.Now;
                                Log.Debug("Stored ItemEnhancement action in block #{index}. Time Taken: {time} ms.", ev.BlockIndex, (end - start).Milliseconds);
                                start = DateTimeOffset.Now;

                                var slotState = ev.OutputStates.GetCombinationSlotState(
                                    itemEnhancement.avatarAddress,
                                    itemEnhancement.slotIndex);

                                if (slotState?.Result.itemUsable.ItemType is ItemType.Equipment)
                                {
                                    ProcessEquipmentData(
                                        ev.Signer,
                                        itemEnhancement.avatarAddress,
                                        avatarName,
                                        avatarLevel,
                                        avatarTitleId,
                                        avatarArmorId,
                                        avatarCp,
                                        (Equipment)slotState.Result.itemUsable);
                                }

                                end = DateTimeOffset.Now;
                                Log.Debug(
                                    "Stored avatar {address}'s equipment in block #{index}. Time Taken: {time} ms.",
                                    itemEnhancement.avatarAddress,
                                    ev.BlockIndex,
                                    (end - start).Milliseconds);
                            }

                            if (ev.Action is Buy buy)
                            {
                                _renderCount++;
                                Log.Debug($"Render Count: #{_renderCount}");
                                var start = DateTimeOffset.Now;
                                AvatarState avatarState = ev.OutputStates.GetAvatarStateV2(buy.buyerAvatarAddress);
                                var previousStates = ev.PreviousStates;
                                var characterSheet = previousStates.GetSheet<CharacterSheet>();
                                var avatarLevel = avatarState.level;
                                var avatarArmorId = avatarState.GetArmorId();
                                var avatarTitleCostume = avatarState.inventory.Costumes.FirstOrDefault(costume => costume.ItemSubType == ItemSubType.Title && costume.equipped);
                                int? avatarTitleId = null;
                                if (avatarTitleCostume != null)
                                {
                                    avatarTitleId = avatarTitleCostume.Id;
                                }

                                var avatarCp = CPHelper.GetCP(avatarState, characterSheet);
                                string avatarName = avatarState.name;

                                _buyAgentList.Add(new AgentModel()
                                {
                                    Address = ev.Signer.ToString(),
                                });
                                _buyAvatarList.Add(new AvatarModel()
                                {
                                    Address = buy.buyerAvatarAddress.ToString(),
                                    AgentAddress = ev.Signer.ToString(),
                                    Name = avatarName,
                                    AvatarLevel = avatarLevel,
                                    TitleId = avatarTitleId,
                                    ArmorId = avatarArmorId,
                                    Cp = avatarCp,
                                });

                                var buyerInventory = avatarState.inventory;
                                foreach (var purchaseInfo in buy.purchaseInfos)
                                {
                                    var state = ev.OutputStates.GetState(
                                    Addresses.GetItemAddress(purchaseInfo.TradableId));
                                    ITradableItem orderItem =
                                        (ITradableItem)ItemFactory.Deserialize((Dictionary)state!);
                                    Order order =
                                        OrderFactory.Deserialize(
                                            (Dictionary)ev.OutputStates.GetState(
                                                Order.DeriveAddress(purchaseInfo.OrderId))!);
                                    int itemCount = order is FungibleOrder fungibleOrder
                                        ? fungibleOrder.ItemCount
                                        : 1;
                                    if (orderItem.ItemType == ItemType.Equipment)
                                    {
                                        Equipment equipment = (Equipment)orderItem;
                                        _buyShopEquipmentsList.Add(new ShopHistoryEquipmentModel()
                                        {
                                            OrderId = purchaseInfo.OrderId.ToString(),
                                            TxId = string.Empty,
                                            BlockIndex = ev.BlockIndex,
                                            BlockHash = string.Empty,
                                            ItemId = equipment.ItemId.ToString(),
                                            SellerAvatarAddress = purchaseInfo.SellerAvatarAddress.ToString(),
                                            BuyerAvatarAddress = buy.buyerAvatarAddress.ToString(),
                                            Price = decimal.Parse(purchaseInfo.Price.ToString().Split(" ").FirstOrDefault()!),
                                            ItemType = equipment.ItemType.ToString(),
                                            ItemSubType = equipment.ItemSubType.ToString(),
                                            Id = equipment.Id,
                                            BuffSkillCount = equipment.BuffSkills.Count,
                                            ElementalType = equipment.ElementalType.ToString(),
                                            Grade = equipment.Grade,
                                            SetId = equipment.SetId,
                                            SkillsCount = equipment.Skills.Count,
                                            SpineResourcePath = equipment.SpineResourcePath,
                                            RequiredBlockIndex = equipment.RequiredBlockIndex,
                                            NonFungibleId = equipment.NonFungibleId.ToString(),
                                            TradableId = equipment.TradableId.ToString(),
                                            UniqueStatType = equipment.UniqueStatType.ToString(),
                                            ItemCount = itemCount,
                                            TimeStamp = DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                        });
                                    }

                                    if (orderItem.ItemType == ItemType.Costume)
                                    {
                                        Costume costume = (Costume)orderItem;
                                        _buyShopCostumesList.Add(new ShopHistoryCostumeModel()
                                        {
                                            OrderId = purchaseInfo.OrderId.ToString(),
                                            TxId = string.Empty,
                                            BlockIndex = ev.BlockIndex,
                                            BlockHash = string.Empty,
                                            ItemId = costume.ItemId.ToString(),
                                            SellerAvatarAddress = purchaseInfo.SellerAvatarAddress.ToString(),
                                            BuyerAvatarAddress = buy.buyerAvatarAddress.ToString(),
                                            Price = decimal.Parse(purchaseInfo.Price.ToString().Split(" ").FirstOrDefault()!),
                                            ItemType = costume.ItemType.ToString(),
                                            ItemSubType = costume.ItemSubType.ToString(),
                                            Id = costume.Id,
                                            ElementalType = costume.ElementalType.ToString(),
                                            Grade = costume.Grade,
                                            Equipped = costume.Equipped,
                                            SpineResourcePath = costume.SpineResourcePath,
                                            RequiredBlockIndex = costume.RequiredBlockIndex,
                                            NonFungibleId = costume.NonFungibleId.ToString(),
                                            TradableId = costume.TradableId.ToString(),
                                            ItemCount = itemCount,
                                            TimeStamp = DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                        });
                                    }

                                    if (orderItem.ItemType == ItemType.Material)
                                    {
                                        Material material = (Material)orderItem;
                                        _buyShopMaterialsList.Add(new ShopHistoryMaterialModel()
                                        {
                                            OrderId = purchaseInfo.OrderId.ToString(),
                                            TxId = string.Empty,
                                            BlockIndex = ev.BlockIndex,
                                            BlockHash = string.Empty,
                                            ItemId = material.ItemId.ToString(),
                                            SellerAvatarAddress = purchaseInfo.SellerAvatarAddress.ToString(),
                                            BuyerAvatarAddress = buy.buyerAvatarAddress.ToString(),
                                            Price = decimal.Parse(purchaseInfo.Price.ToString().Split(" ").FirstOrDefault()!),
                                            ItemType = material.ItemType.ToString(),
                                            ItemSubType = material.ItemSubType.ToString(),
                                            Id = material.Id,
                                            ElementalType = material.ElementalType.ToString(),
                                            Grade = material.Grade,
                                            ItemCount = itemCount,
                                            TimeStamp = DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                        });
                                    }

                                    if (orderItem.ItemType == ItemType.Consumable)
                                    {
                                        Consumable consumable = (Consumable)orderItem;
                                        _buyShopConsumablesList.Add(new ShopHistoryConsumableModel()
                                        {
                                            OrderId = purchaseInfo.OrderId.ToString(),
                                            TxId = string.Empty,
                                            BlockIndex = ev.BlockIndex,
                                            BlockHash = string.Empty,
                                            ItemId = consumable.ItemId.ToString(),
                                            SellerAvatarAddress = purchaseInfo.SellerAvatarAddress.ToString(),
                                            BuyerAvatarAddress = buy.buyerAvatarAddress.ToString(),
                                            Price = decimal.Parse(purchaseInfo.Price.ToString().Split(" ").FirstOrDefault()!),
                                            ItemType = consumable.ItemType.ToString(),
                                            ItemSubType = consumable.ItemSubType.ToString(),
                                            Id = consumable.Id,
                                            BuffSkillCount = consumable.BuffSkills.Count,
                                            ElementalType = consumable.ElementalType.ToString(),
                                            Grade = consumable.Grade,
                                            SkillsCount = consumable.Skills.Count,
                                            RequiredBlockIndex = consumable.RequiredBlockIndex,
                                            NonFungibleId = consumable.NonFungibleId.ToString(),
                                            TradableId = consumable.TradableId.ToString(),
                                            MainStat = consumable.MainStat.ToString(),
                                            ItemCount = itemCount,
                                            TimeStamp = DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                        });
                                    }

                                    if (purchaseInfo.ItemSubType == ItemSubType.Armor
                                        || purchaseInfo.ItemSubType == ItemSubType.Belt
                                        || purchaseInfo.ItemSubType == ItemSubType.Necklace
                                        || purchaseInfo.ItemSubType == ItemSubType.Ring
                                        || purchaseInfo.ItemSubType == ItemSubType.Weapon)
                                    {
                                        var sellerState = ev.OutputStates.GetAvatarStateV2(purchaseInfo.SellerAvatarAddress);
                                        var sellerInventory = sellerState.inventory;

                                        if (buyerInventory.Equipments == null || sellerInventory.Equipments == null)
                                        {
                                            continue;
                                        }

                                        Equipment? equipment = buyerInventory.Equipments.SingleOrDefault(i =>
                                            i.TradableId == purchaseInfo.TradableId) ?? sellerInventory.Equipments.SingleOrDefault(i =>
                                            i.TradableId == purchaseInfo.TradableId);

                                        if (equipment is { } equipmentNotNull)
                                        {
                                            ProcessEquipmentData(
                                                ev.Signer,
                                                buy.buyerAvatarAddress,
                                                avatarName,
                                                avatarLevel,
                                                avatarTitleId,
                                                avatarArmorId,
                                                avatarCp,
                                                equipmentNotNull);
                                        }
                                    }
                                }

                                var end = DateTimeOffset.Now;
                                Log.Debug(
                                    "Stored avatar {address}'s equipment in block #{index}. Time Taken: {time} ms.",
                                    buy.buyerAvatarAddress,
                                    ev.BlockIndex,
                                    (end - start).Milliseconds);
                            }

                            if (ev.Action is Stake stake)
                            {
                                _renderCount++;
                                Log.Debug($"Render Count: #{_renderCount}");
                                var start = DateTimeOffset.Now;
                                ev.OutputStates.TryGetStakeState(ev.Signer, out StakeState stakeState);
                                var prevStakeStartBlockIndex =
                                    !ev.PreviousStates.TryGetStakeState(ev.Signer, out StakeState prevStakeState)
                                        ? 0 : prevStakeState.StartedBlockIndex;
                                var newStakeStartBlockIndex = stakeState.StartedBlockIndex;
                                var currency = ev.OutputStates.GetGoldCurrency();
                                var balance = ev.OutputStates.GetBalance(ev.Signer, currency);
                                var stakeStateAddress = StakeState.DeriveAddress(ev.Signer);
                                var previousAmount = ev.PreviousStates.GetBalance(stakeStateAddress, currency);
                                var newAmount = ev.OutputStates.GetBalance(stakeStateAddress, currency);

                                _stakeAgentList.Add(new AgentModel()
                                {
                                    Address = ev.Signer.ToString(),
                                });
                                _stakeList.Add(new StakeModel()
                                {
                                    BlockIndex = ev.BlockIndex,
                                    AgentAddress = ev.Signer.ToString(),
                                    PreviousAmount = Convert.ToDecimal(previousAmount.GetQuantityString()),
                                    NewAmount = Convert.ToDecimal(newAmount.GetQuantityString()),
                                    RemainingNCG = Convert.ToDecimal(balance.GetQuantityString()),
                                    PrevStakeStartBlockIndex = prevStakeStartBlockIndex,
                                    NewStakeStartBlockIndex = newStakeStartBlockIndex,
                                    TimeStamp = DateTimeOffset.Now,
                                });
                                var end = DateTimeOffset.Now;
                                Log.Debug("Stored Stake action in block #{index}. Time Taken: {time} ms.", ev.BlockIndex, (end - start).Milliseconds);
                            }

                            if (ev.Action is ClaimStakeReward claimStakeReward)
                            {
                                _renderCount++;
                                Log.Debug($"Render Count: #{_renderCount}");
                                var start = DateTimeOffset.Now;
                                var plainValue = (Bencodex.Types.Dictionary)claimStakeReward.PlainValue;
                                var avatarAddress = plainValue["AvatarAddressKey"].ToAddress();
                                AvatarState avatarState = ev.OutputStates.GetAvatarStateV2(avatarAddress);
                                var previousStates = ev.PreviousStates;
                                var characterSheet = previousStates.GetSheet<CharacterSheet>();
                                var avatarLevel = avatarState.level;
                                var avatarArmorId = avatarState.GetArmorId();
                                var avatarTitleCostume = avatarState.inventory.Costumes.FirstOrDefault(costume => costume.ItemSubType == ItemSubType.Title && costume.equipped);
                                int? avatarTitleId = null;
                                if (avatarTitleCostume != null)
                                {
                                    avatarTitleId = avatarTitleCostume.Id;
                                }

                                var avatarCp = CPHelper.GetCP(avatarState, characterSheet);
                                string avatarName = avatarState.name;

                                var id = claimStakeReward.Id;
                                ev.OutputStates.TryGetStakeState(ev.Signer, out StakeState stakeState);
                                ev.PreviousStates.TryGetStakeState(ev.Signer, out StakeState prevStakeState);

                                var claimStakeStartBlockIndex = prevStakeState.StartedBlockIndex;
                                var claimStakeEndBlockIndex = prevStakeState.ReceivedBlockIndex;
                                var currency = ev.OutputStates.GetGoldCurrency();
                                var stakeStateAddress = StakeState.DeriveAddress(ev.Signer);
                                var stakedAmount = ev.OutputStates.GetBalance(stakeStateAddress, currency);

                                var sheets = ev.PreviousStates.GetSheets(new[]
                                {
                                    typeof(StakeRegularRewardSheet),
                                    typeof(ConsumableItemSheet),
                                    typeof(CostumeItemSheet),
                                    typeof(EquipmentItemSheet),
                                    typeof(MaterialItemSheet),
                                });
                                StakeRegularRewardSheet stakeRegularRewardSheet = sheets.GetSheet<StakeRegularRewardSheet>();
                                int level = stakeRegularRewardSheet.FindLevelByStakedAmount(ev.Signer, stakedAmount);
                                var rewards = stakeRegularRewardSheet[level].Rewards;
                                var accumulatedRewards = stakeState.CalculateAccumulatedRewards(ev.BlockIndex);

                                // Assume previewnet from the NCG's minter address.
                                bool isPreviewNet = ev.PreviousStates.GetGoldCurrency().Minters!
                                    .Contains(new Address("340f110b91d0577a9ae0ea69ce15269436f217da"));

                                // https://github.com/planetarium/lib9c/pull/1073
                                bool addZeroItemForChainConsistency = isPreviewNet && ev.BlockIndex < 1_200_000;
                                int hourGlassCount = 0;
                                int apPotionCount = 0;
                                foreach (var reward in rewards)
                                {
                                    var (quantity, _) = stakedAmount.DivRem(currency * reward.Rate);
                                    if (!addZeroItemForChainConsistency && quantity < 1)
                                    {
                                        // If the quantity is zero, it doesn't add the item into inventory.
                                        continue;
                                    }

                                    if (reward.ItemId == 400000)
                                    {
                                        hourGlassCount += (int)quantity * accumulatedRewards;
                                    }

                                    if (reward.ItemId == 500000)
                                    {
                                        apPotionCount += (int)quantity * accumulatedRewards;
                                    }
                                }

                                if (ev.PreviousStates.TryGetSheet<StakeRegularFixedRewardSheet>(
                                        out var stakeRegularFixedRewardSheet))
                                {
                                    var fixedRewards = stakeRegularFixedRewardSheet[level].Rewards;
                                    foreach (var reward in fixedRewards)
                                    {
                                        if (reward.ItemId == 400000)
                                        {
                                            hourGlassCount += (int)reward.Count * accumulatedRewards;
                                        }

                                        if (reward.ItemId == 500000)
                                        {
                                            apPotionCount += (int)reward.Count * accumulatedRewards;
                                        }
                                    }
                                }

                                _claimStakeAgentList.Add(new AgentModel()
                                {
                                    Address = ev.Signer.ToString(),
                                });
                                _claimStakeAvatarList.Add(new AvatarModel()
                                {
                                    Address = avatarAddress.ToString(),
                                    AgentAddress = ev.Signer.ToString(),
                                    Name = avatarName,
                                    AvatarLevel = avatarLevel,
                                    TitleId = avatarTitleId,
                                    ArmorId = avatarArmorId,
                                    Cp = avatarCp,
                                });
                                _claimStakeList.Add(new ClaimStakeRewardModel()
                                {
                                    Id = id.ToString(),
                                    BlockIndex = ev.BlockIndex,
                                    AgentAddress = ev.Signer.ToString(),
                                    ClaimRewardAvatarAddress = avatarAddress.ToString(),
                                    HourGlassCount = hourGlassCount,
                                    ApPotionCount = apPotionCount,
                                    ClaimStakeStartBlockIndex = claimStakeStartBlockIndex,
                                    ClaimStakeEndBlockIndex = claimStakeEndBlockIndex,
                                    TimeStamp = DateTimeOffset.Now,
                                });
                                var end = DateTimeOffset.Now;
                                Log.Debug("Stored ClaimStakeReward action in block #{index}. Time Taken: {time} ms.", ev.BlockIndex, (end - start).Milliseconds);
                            }

                            if (ev.Action is MigrateMonsterCollection mc)
                            {
                                _renderCount++;
                                Log.Debug($"Render Count: #{_renderCount}");
                                var start = DateTimeOffset.Now;
                                ev.OutputStates.TryGetStakeState(ev.Signer, out StakeState stakeState);
                                var agentState = ev.PreviousStates.GetAgentState(ev.Signer);
                                Address collectionAddress = MonsterCollectionState.DeriveAddress(ev.Signer, agentState.MonsterCollectionRound);
                                ev.PreviousStates.TryGetState(collectionAddress, out Dictionary stateDict);
                                var monsterCollectionState = new MonsterCollectionState(stateDict);
                                var currency = ev.OutputStates.GetGoldCurrency();
                                var migrationAmount = ev.PreviousStates.GetBalance(monsterCollectionState.address, currency);
                                var migrationStartBlockIndex = ev.BlockIndex;
                                var stakeStartBlockIndex = stakeState.StartedBlockIndex;
                                _mmcAgentList.Add(new AgentModel()
                                {
                                    Address = ev.Signer.ToString(),
                                });
                                _mmcList.Add(new MigrateMonsterCollectionModel()
                                {
                                    BlockIndex = ev.BlockIndex,
                                    AgentAddress = ev.Signer.ToString(),
                                    MigrationAmount = Convert.ToDecimal(migrationAmount.GetQuantityString()),
                                    MigrationStartBlockIndex = migrationStartBlockIndex,
                                    StakeStartBlockIndex = stakeStartBlockIndex,
                                    TimeStamp = DateTimeOffset.Now,
                                });
                                var end = DateTimeOffset.Now;
                                Log.Debug("Stored MigrateMonsterCollection action in block #{index}. Time Taken: {time} ms.", ev.BlockIndex, (end - start).Milliseconds);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error("RenderSubscriber: {message}", ex.Message);
                        }
                    });

            _actionRenderer.EveryUnrender<ActionBase>()
                .Subscribe(
                    ev =>
                    {
                        try
                        {
                            if (ev.Exception != null)
                            {
                                return;
                            }

                            if (ev.Action is HackAndSlash has)
                            {
                                MySqlStore.DeleteHackAndSlash(has.Id);
                                Log.Debug("Deleted HackAndSlash action in block #{index}", ev.BlockIndex);
                            }

                            if (ev.Action is CombinationConsumable combinationConsumable)
                            {
                                MySqlStore.DeleteCombinationConsumable(combinationConsumable.Id);
                                Log.Debug("Deleted CombinationConsumable action in block #{index}", ev.BlockIndex);
                            }

                            if (ev.Action is CombinationEquipment combinationEquipment)
                            {
                                MySqlStore.DeleteCombinationEquipment(combinationEquipment.Id);
                                Log.Debug("Deleted CombinationEquipment action in block #{index}", ev.BlockIndex);
                                AvatarState avatarState = ev.OutputStates.GetAvatarStateV2(combinationEquipment.avatarAddress);
                                var previousStates = ev.PreviousStates;
                                var characterSheet = previousStates.GetSheet<CharacterSheet>();
                                var avatarLevel = avatarState.level;
                                var avatarArmorId = avatarState.GetArmorId();
                                var avatarTitleCostume = avatarState.inventory.Costumes.FirstOrDefault(costume => costume.ItemSubType == ItemSubType.Title && costume.equipped);
                                int? avatarTitleId = null;
                                if (avatarTitleCostume != null)
                                {
                                    avatarTitleId = avatarTitleCostume.Id;
                                }

                                var avatarCp = CPHelper.GetCP(avatarState, characterSheet);
                                string avatarName = avatarState.name;
                                var slotState = ev.OutputStates.GetCombinationSlotState(
                                    combinationEquipment.avatarAddress,
                                    combinationEquipment.slotIndex);

                                if (slotState?.Result.itemUsable.ItemType is ItemType.Equipment)
                                {
                                    ProcessEquipmentData(
                                        ev.Signer,
                                        combinationEquipment.avatarAddress,
                                        avatarName,
                                        avatarLevel,
                                        avatarTitleId,
                                        avatarArmorId,
                                        avatarCp,
                                        (Equipment)slotState.Result.itemUsable);
                                }

                                Log.Debug(
                                    "Reverted avatar {address}'s equipments in block #{index}",
                                    combinationEquipment.avatarAddress,
                                    ev.BlockIndex);
                            }

                            if (ev.Action is ItemEnhancement itemEnhancement)
                            {
                                MySqlStore.DeleteItemEnhancement(itemEnhancement.Id);
                                Log.Debug("Deleted ItemEnhancement action in block #{index}", ev.BlockIndex);
                                AvatarState avatarState = ev.OutputStates.GetAvatarStateV2(itemEnhancement.avatarAddress);
                                var previousStates = ev.PreviousStates;
                                var characterSheet = previousStates.GetSheet<CharacterSheet>();
                                var avatarLevel = avatarState.level;
                                var avatarArmorId = avatarState.GetArmorId();
                                var avatarTitleCostume = avatarState.inventory.Costumes.FirstOrDefault(costume => costume.ItemSubType == ItemSubType.Title && costume.equipped);
                                int? avatarTitleId = null;
                                if (avatarTitleCostume != null)
                                {
                                    avatarTitleId = avatarTitleCostume.Id;
                                }

                                var avatarCp = CPHelper.GetCP(avatarState, characterSheet);
                                string avatarName = avatarState.name;
                                var slotState = ev.OutputStates.GetCombinationSlotState(
                                    itemEnhancement.avatarAddress,
                                    itemEnhancement.slotIndex);

                                if (slotState?.Result.itemUsable.ItemType is ItemType.Equipment)
                                {
                                    ProcessEquipmentData(
                                        ev.Signer,
                                        itemEnhancement.avatarAddress,
                                        avatarName,
                                        avatarLevel,
                                        avatarTitleId,
                                        avatarArmorId,
                                        avatarCp,
                                        (Equipment)slotState.Result.itemUsable);
                                }

                                Log.Debug(
                                    "Reverted avatar {address}'s equipments in block #{index}",
                                    itemEnhancement.avatarAddress,
                                    ev.BlockIndex);
                            }

                            if (ev.Action is Buy buy)
                            {
                                var buyerInventory = ev.OutputStates.GetAvatarStateV2(buy.buyerAvatarAddress).inventory;

                                foreach (var purchaseInfo in buy.purchaseInfos)
                                {
                                    if (purchaseInfo.ItemSubType == ItemSubType.Armor
                                        || purchaseInfo.ItemSubType == ItemSubType.Belt
                                        || purchaseInfo.ItemSubType == ItemSubType.Necklace
                                        || purchaseInfo.ItemSubType == ItemSubType.Ring
                                        || purchaseInfo.ItemSubType == ItemSubType.Weapon)
                                    {
                                        AvatarState sellerState = ev.OutputStates.GetAvatarStateV2(purchaseInfo.SellerAvatarAddress);
                                        var sellerInventory = sellerState.inventory;
                                        var previousStates = ev.PreviousStates;
                                        var characterSheet = previousStates.GetSheet<CharacterSheet>();
                                        var avatarLevel = sellerState.level;
                                        var avatarArmorId = sellerState.GetArmorId();
                                        var avatarTitleCostume = sellerState.inventory.Costumes.FirstOrDefault(costume => costume.ItemSubType == ItemSubType.Title && costume.equipped);
                                        int? avatarTitleId = null;
                                        if (avatarTitleCostume != null)
                                        {
                                            avatarTitleId = avatarTitleCostume.Id;
                                        }

                                        var avatarCp = CPHelper.GetCP(sellerState, characterSheet);
                                        string avatarName = sellerState.name;

                                        if (buyerInventory.Equipments == null || sellerInventory.Equipments == null)
                                        {
                                            continue;
                                        }

                                        MySqlStore.StoreAgent(ev.Signer);
                                        MySqlStore.StoreAvatar(
                                            purchaseInfo.SellerAvatarAddress,
                                            purchaseInfo.SellerAgentAddress,
                                            avatarName,
                                            avatarLevel,
                                            avatarTitleId,
                                            avatarArmorId,
                                            avatarCp);
                                        Equipment? equipment = buyerInventory.Equipments.SingleOrDefault(i =>
                                            i.TradableId == purchaseInfo.TradableId) ?? sellerInventory.Equipments.SingleOrDefault(i =>
                                            i.TradableId == purchaseInfo.TradableId);

                                        if (equipment is { } equipmentNotNull)
                                        {
                                            ProcessEquipmentData(
                                                purchaseInfo.SellerAvatarAddress,
                                                purchaseInfo.SellerAgentAddress,
                                                avatarName,
                                                avatarLevel,
                                                avatarTitleId,
                                                avatarArmorId,
                                                avatarCp,
                                                equipmentNotNull);
                                        }
                                    }
                                }

                                Log.Debug(
                                    "Reverted avatar {address}'s equipment in block #{index}",
                                    buy.buyerAvatarAddress,
                                    ev.BlockIndex);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error("RenderSubscriber: {message}", ex.Message);
                        }
                    });
            return Task.CompletedTask;
        }

        private void ProcessEquipmentData(
            Address agentAddress,
            Address avatarAddress,
            string avatarName,
            int avatarLevel,
            int? avatarTitleId,
            int avatarArmorId,
            int avatarCp,
            Equipment equipment)
        {
            var cp = CPHelper.GetCP(equipment);
            _eqAgentList.Add(new AgentModel()
            {
                Address = agentAddress.ToString(),
            });
            _eqAvatarList.Add(new AvatarModel()
            {
                Address = avatarAddress.ToString(),
                AgentAddress = agentAddress.ToString(),
                Name = avatarName,
                AvatarLevel = avatarLevel,
                TitleId = avatarTitleId,
                ArmorId = avatarArmorId,
                Cp = avatarCp,
            });
            _eqList.Add(new EquipmentModel()
            {
                ItemId = equipment.ItemId.ToString(),
                AgentAddress = agentAddress.ToString(),
                AvatarAddress = avatarAddress.ToString(),
                EquipmentId = equipment.Id,
                Cp = cp,
                Level = equipment.level,
                ItemSubType = equipment.ItemSubType.ToString(),
            });
        }
    }
}
