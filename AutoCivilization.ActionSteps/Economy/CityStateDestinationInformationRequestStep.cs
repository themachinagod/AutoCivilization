﻿using AutoCivilization.Abstractions;
using AutoCivilization.Abstractions.ActionSteps;
using AutoCivilization.Console;
using System;
using System.Linq;

namespace AutoCivilization.ActionSteps
{
    public class CityStateCaravanDestinationInformationRequestStep : StepActionBase, ICityStateCaravanDestinationInformationRequestStep
    {
        private readonly IGlobalGameCache _globalGameCache;
        private readonly IOrdinalSuffixResolver _ordinalSuffixResolver;

        private const int BaseTradeTokensForCityStateVisit = 2;

        public CityStateCaravanDestinationInformationRequestStep(IGlobalGameCache globalGameCache,
                                                          IOrdinalSuffixResolver ordinalSuffixResolver) : base()
        {
            OperationType = OperationType.InformationRequest;

            _globalGameCache = globalGameCache;
            _ordinalSuffixResolver = ordinalSuffixResolver;
        }

        public override bool ShouldExecuteAction(BotMoveState moveState)
        {
            var movingCaravan = moveState.TradeCaravansAvailable[moveState.CurrentCaravanIdToMove - 1];
            if (movingCaravan.CaravanDestinationType == CaravanDestinationType.CityState) return true;
            return false;
        }

        public override MoveStepActionData ExecuteAction(BotMoveState moveState)
        {
            // TODO: this lists all avilable city states - would be better if we can limit this list:
            //       to remove city states that are not in the current game
            //       currently hardwired for 2plyr prologue board!

            var caravanRef = _ordinalSuffixResolver.GetOrdinalSuffixWithInput(moveState.CurrentCaravanIdToMove);
            var cityStates = _globalGameCache.CityStates.Select(x => $"{x.Id}. {x.Name}").ToList();
            return new MoveStepActionData($"Which city state did my {caravanRef} trade caravan arrive at?",
                   cityStates);
        }

        /// <summary>
        /// Update move state with visited city state for the active caravan
        /// </summary>
        /// <param name="input">The code for the city states visited specified by the user</param>
        /// <param name="moveState">The current move state to work from</param>
        public override void UpdateMoveStateForStep(string input, BotMoveState moveState)
        {
            var selectedid = Convert.ToInt32(input);
            var citystate = _globalGameCache.CityStates.First(x => x.Id == selectedid);
            var movingCaravan = moveState.TradeCaravansAvailable[moveState.CurrentCaravanIdToMove - 1];
            movingCaravan.CaravanCityStateDestination = citystate;
            moveState.TradeTokensAvailable[citystate.Type] += BaseTradeTokensForCityStateVisit;
        }
    }
}
