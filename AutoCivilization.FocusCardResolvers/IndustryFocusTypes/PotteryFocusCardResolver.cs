﻿using AutoCivilization.Abstractions;
using AutoCivilization.Abstractions.ActionSteps;
using AutoCivilization.Abstractions.FocusCardResolvers;
using AutoCivilization.Abstractions.TechnologyResolvers;
using AutoCivilization.Console;
using System.Collections.Generic;
using System.Text;

namespace AutoCivilization.FocusCardResolvers
{
    public class PotteryFocusCardMoveResolver : FocusCardMoveResolverBase, IIndustryLevel1FocusCardMoveResolver
    {
        private const int BaseProduction = 5;

        private ICultureResolverUtility _cultureResolverUtility;
        private readonly IFocusBarTechnologyUpgradeResolver _focusBarTechnologyUpgradeResolver;

        public PotteryFocusCardMoveResolver(IWonderPlacementCityActionRequestStep wonderPlacementCityActionRequestStep,
                                            ICityPlacementActionRequestStep cityPlacementActionRequestStep,
                                            ICityPlacementInformationRequestStep cityPlacementInformationRequestStep,
                                            IFocusBarTechnologyUpgradeResolver focusBarTechnologyUpgradeResolver) : base()
        {
            //_cultureResolverUtility = cultureResolverUtility;
            _focusBarTechnologyUpgradeResolver = focusBarTechnologyUpgradeResolver;

            FocusType = FocusType.Industry;
            FocusLevel = FocusLevel.Lvl1;

            _actionSteps.Add(0, wonderPlacementCityActionRequestStep);
            _actionSteps.Add(1, cityPlacementActionRequestStep);
            _actionSteps.Add(2, cityPlacementInformationRequestStep);
        }

        public override void PrimeMoveState(BotGameState botGameStateService)
        {
            //_moveState = _cultureResolverUtility.CreateBasicCultureMoveState(botGameStateService, BaseProduction);
            _moveState = new BotMoveState();
            _moveState.ActiveFocusBarForMove = botGameStateService.ActiveFocusBar;
            _moveState.ActiveWonderCardDecks = botGameStateService.WonderCardDecks;
            _moveState.PurchasedWonders = new List<WonderCardModel>(botGameStateService.PurchasedWonders);
            _moveState.VisitedCityStates = new List<CityStateModel>(botGameStateService.VisitedCityStates);
            _moveState.ControlledNaturalWonders = new List<string>(botGameStateService.ControlledNaturalWonders);
            _moveState.TradeTokensAvailable = new Dictionary<FocusType, int>(botGameStateService.TradeTokens);
            _moveState.NaturalResourcesToSpend = botGameStateService.ControlledNaturalResources;
            _moveState.FriendlyCityCount = botGameStateService.FriendlyCityCount;
            _moveState.BaseProductionPoints = BaseProduction;
        }

        public override string UpdateGameStateForMove(BotGameState botGameStateService)
        {
            //_cultureResolverUtility.UpdateBaseCultureGameStateForMove(_moveState, botGameStateService);

            botGameStateService.WonderCardDecks = _moveState.ActiveWonderCardDecks;
            botGameStateService.PurchasedWonders = new List<WonderCardModel>(_moveState.PurchasedWonders);
            botGameStateService.TradeTokens = new Dictionary<FocusType, int>(_moveState.TradeTokensAvailable);
            botGameStateService.FriendlyCityCount += _moveState.FriendlyCitiesAddedThisTurn;
            botGameStateService.ControlledNaturalResources -= _moveState.NaturalResourcesSpentThisTurn;

            FocusBarUpgradeResponse freeUpgrade = new FocusBarUpgradeResponse(false, _moveState.ActiveFocusBarForMove, _moveState.ActiveFocusBarForMove.ActiveFocusSlot, null);
            if (!_moveState.HasPurchasedCityThisTurn && !_moveState.HasPurchasedWonderThisTurn)
            {
                freeUpgrade = _focusBarTechnologyUpgradeResolver.RegenerateFocusBarSpecificTechnologyLevelUpgrade(_moveState.ActiveFocusBarForMove, FocusType.Industry);
                botGameStateService.ActiveFocusBar = freeUpgrade.UpgradedFocusBar;
            }

            _currentStep = -1;
            return BuildMoveSummary(freeUpgrade);
        }

        private string BuildMoveSummary(FocusBarUpgradeResponse freeTechUpgradeResponse)
        {
            var summary = "To summarise my move I did the following;\n";
            StringBuilder sb = new StringBuilder(summary);
            if (_moveState.WonderPurchasedThisTurn != null)
            {
                sb.Append($"I updated my game state to show that I purchased the world wonder {_moveState.WonderPurchasedThisTurn.Name}, the token of which you placed on my stongest free city\n");
                sb.Append($"I facilitated this move with the following production capacity breakdown;\n");
                sb.Append($"Pottery focus card base capacity: {_moveState.BaseProductionPoints} production points\n");
                sb.Append($"Industry diplomacy cards retained bonus: {_moveState.VisitedCityStates.Count} held diplomacy cards worth {_moveState.VisitedCityStates.Count} production points\n");
                sb.Append($"Natural wonder resources retained bonus: {_moveState.ControlledNaturalWonders.Count} held natural wonders worth {_moveState.ControlledNaturalWonders.Count * 2} production points\n");
                sb.Append($"Natural resources points : {_moveState.NaturalResourcesToSpend} held worth {_moveState.NaturalResourcesToSpend * 2} production points, of which we spent {_moveState.NaturalResourcesSpentThisTurn} resources\n");
                sb.Append($"Industry trade token points : {_moveState.TradeTokensAvailable[FocusType.Industry]} held worth {_moveState.TradeTokensAvailable[FocusType.Industry]} production points of which we spent {_moveState.IndustryTokensUsedThisTurn} trade tokens\n");
                sb.Append($"Total production capacity available before purchase: {_moveState.ComputedProductionCapacityForTurn} production points\n");
            }
            else
            {
                sb.Append($"I was unable to purchase a world wonder on this turn for the following reasons;\n");
                if (_moveState.FriendlyCityCount <= _moveState.PurchasedWonders.Count) sb.Append($"I do not have enough friendly cities without existing wonder tokens\n");
                else sb.Append($"I do not have enough production capacity to purchase any of the unlocked wonders\n");
            }
            
            if (_moveState.HasPurchasedCityThisTurn) sb.Append($"I updated my game state to show that I build 1 new city of which you placed on the board in a legal space\n");

            if (!_moveState.HasPurchasedCityThisTurn && !_moveState.HasPurchasedWonderThisTurn && freeTechUpgradeResponse.HasUpgraded)
            {
                sb.Append($"Becuase I did not manage to purchase a wonder or build a city on this turn, I received a free Industry technology upgrade allowing me to upgrade from { freeTechUpgradeResponse.OldTechnology.Name} to { freeTechUpgradeResponse.NewTechnology.Name}\n");
            }
            return sb.ToString(); // _cultureResolverUtility.BuildGeneralisedCultureMoveSummary(summary, _moveState);
        }
    }
}