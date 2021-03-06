﻿using AutoCivilization.Console;
using System.Collections.Generic;

namespace AutoCivilization.Abstractions.ActionSteps
{
    public enum OperationType
    {
        ActionRequest,
        InformationRequest
    }

    public interface IStepAction
    {
        int StepIndex { get; set; }
        OperationType OperationType { get; set; }

        bool ShouldExecuteAction(BotMoveState moveState);
        MoveStepActionData ExecuteAction(BotMoveState moveState);
        void UpdateMoveStateForStep(string input, BotMoveState moveState);
    }

    public interface INoActionStep : IStepAction 
    {
    }
}
