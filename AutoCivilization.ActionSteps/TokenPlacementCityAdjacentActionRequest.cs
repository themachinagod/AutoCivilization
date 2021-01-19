﻿using AutoCivilization.Abstractions;
using AutoCivilization.Abstractions.ActionSteps;
using System.Collections.Generic;

namespace AutoCivilization.ActionSteps
{
    public class TokenPlacementCityAdjacentActionRequestStep : StepActionBase, ITokenPlacementCityAdjacentActionRequestStep
    {
        public TokenPlacementCityAdjacentActionRequestStep(IBotMoveStateCache botMoveStateService) : base(botMoveStateService)
        {
            OperationType = OperationType.ActionRequest;
        }

        public override MoveStepActionData ExecuteAction()
        {
            var maxPlacements = _botMoveStateService.BaseCityControlTokensToBePlaced + _botMoveStateService.TradeTokensAvailable[FocusType.Culture];
            return new MoveStepActionData($"Please place {maxPlacements} control tokens on spaces adjacent to any of my cities using the following placement priority rules:\nNatural wonder\nResource token\nVacant barbarian spawn point\nAdjacent to the most cities\nAdjacent to city closest to maturity\nHighest terrain difficulty",
                   new List<string>());
        }
    }
}
