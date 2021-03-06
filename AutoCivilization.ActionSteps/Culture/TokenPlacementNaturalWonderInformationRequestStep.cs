﻿using AutoCivilization.Abstractions;
using AutoCivilization.Abstractions.ActionSteps;
using AutoCivilization.Console;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoCivilization.ActionSteps
{
    public class TokenPlacementNaturalWonderInformationRequestStep : StepActionBase, ITokenPlacementNaturalWonderControlledInformationRequestStep
    {
        public TokenPlacementNaturalWonderInformationRequestStep() : base()
        {
            OperationType = OperationType.InformationRequest;
        }

        public override bool ShouldExecuteAction(BotMoveState moveState)
        {
            var totalTokensPlaced = moveState.CityControlTokensPlacedThisTurn + moveState.TerritroyControlTokensPlacedThisTurn;
            return ((totalTokensPlaced > 0) &&
                   (moveState.NaturalResourceTokensControlledThisTurn < totalTokensPlaced));
        }

        public override MoveStepActionData ExecuteAction(BotMoveState moveState)
        {
            // TODO: we need natural wonders for game
            //       currently hard wired!
            //       bit of a hack for multiple just now to avoid a request loop - fine for 2 just now

            // TODO: review how we do this...
            //       - allow multiple repsonses from the list (either in loop or delimited entry)
            
            var naturalWonders = new List<string> { "0. None", "1. Mt Everest", "2. Gran Mesa", "3. Both" };
            return new MoveStepActionData($"Please remove the natural wonder token from the board and place it next to my leadersheet. What is the name(s) of the natural wonder(s) I managed to take control of on this turn?",
                   naturalWonders);
        }

        /// <summary>
        /// Update move state with which natural natural wonders were controlled this turn
        /// </summary>
        /// <param name="input">The code for the natural wonders visited specified by the user</param>
        /// <param name="moveState">The current move state to work from</param>
        public override void UpdateMoveStateForStep(string input, BotMoveState moveState)
        {
            // TODO: we need natural wonders for game
            //       currently hard wired!
            //       bit of a hack for multiple just now to avoid a request loop - fine for 2 just now

            switch (Convert.ToInt32(input))
            {
                case 1:
                    moveState.ControlledNaturalWonders.Add("Mt. Everest");
                    moveState.NaturalWonderTokensControlledThisTurn = 1;
                    break;
                case 2:
                    moveState.ControlledNaturalWonders.Add("Gran Mesa");
                    moveState.NaturalWonderTokensControlledThisTurn = 1;
                    break;
                case 3:
                    moveState.ControlledNaturalWonders.Add("Gran Mesa");
                    moveState.ControlledNaturalWonders.Add("Mt. Everest");
                    moveState.NaturalWonderTokensControlledThisTurn = 2;
                    break;
                default:
                    moveState.NaturalWonderTokensControlledThisTurn = 0;
                    break;
            }
        }
    }
}
