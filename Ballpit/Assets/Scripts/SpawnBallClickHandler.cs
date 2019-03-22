// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;

using UnityEngine;
using UnityEngine.Events;

using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.SDK.UX.Interactable.States;
using Microsoft.MixedReality.Toolkit.SDK.UX.Interactable.Events;
using Microsoft.MixedReality.Toolkit.SDK.UX.Interactable;

using InfoVis.MixedReality.Actions;
using Microsoft.MixedReality.Toolkit.SDK.Utilities.Solvers;
using TMPro;

namespace InfoVis.MixedReality.Interaction.Handlers
{
    /// <summary>
    /// Example of building a custom receiver that can be loaded as part of the events on the Interactable or
    /// in InteractableReceiverList or InteractableReceiver
    /// 
    /// Extend ReceiverBaseMonoBehavior to build external event components
    /// </summary>
    public class SpawnBallInteractablesReceiver : ReceiverBase
    {
        protected State lastState;
        protected string statusString = "State: %state%";
        protected string clickString = "Clicked!";
        protected string voiceString = "VoiceCommand: %voiceCommand%";
        protected string outputString;

        protected string lastVoiceCommand = "";

        protected float clickTime = 2;
        protected Coroutine showClicked;
        protected Coroutine showVoice;
        protected int clickCount = 0;
        public string ResourceToSpawn = "DataBall-Apex";

        protected ObjectSpawner Spawner { get; } = new ObjectSpawner();

        public SpawnBallInteractablesReceiver(UnityEvent ev) : base(ev)
        {
            Name = "SpawnBallEvent";
            HideUnityEvents = true; // hides Unity events in the receiver - meant to be code only
        }

        /// <summary>
        /// allow the info to remove click info if a click event has expired
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        protected IEnumerator ClickTimer(float time)
        {
            yield return new WaitForSeconds(time);
            showClicked = null;
        }

        /// <summary>
        /// allow the info to remove voice command info and it expires
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        protected IEnumerator VoiceTimer(float time)
        {
            yield return new WaitForSeconds(time);
            showVoice = null;
        }

        /// <summary>
        /// Spawns the handler's object
        /// </summary>
        protected void OnShouldSpawnObject()
        {
            GameObject gameObject = Spawner?.Spawn(Host, ResourceToSpawn);
            if (gameObject != null)
            {
                Manipulation.SizeAndLabelDataball(0.5f, "test", gameObject);
            }
        }

        /// <summary>
        /// Called on update, check to see if the state has changed sense the last call
        /// </summary>
        /// <param name="state"></param>
        /// <param name="source"></param>
        public override void OnUpdate(InteractableStates state, Interactable source)
        {
            if (state.CurrentState() != lastState)
            {
                lastState = state.CurrentState();
            }
        }

        /// <summary>
        /// click happened
        /// </summary>
        /// <param name="state"></param>
        /// <param name="source"></param>
        /// <param name="pointer"></param>
        public override void OnClick(InteractableStates state, Interactable source, IMixedRealityPointer pointer = null)
        {
            base.OnClick(state, source);
            if (Host != null)
            {
                if (showClicked != null)
                {
                    Host.StopCoroutine(showClicked);
                    showClicked = null;
                }

                showClicked = Host.StartCoroutine(ClickTimer(clickTime));

                OnShouldSpawnObject();
            }

            clickCount++;
        }

        /// <summary>
        /// voice command called
        /// </summary>
        /// <param name="state"></param>
        /// <param name="source"></param>
        /// <param name="command"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        public override void OnVoiceCommand(InteractableStates state, Interactable source, string command, int index = 0, int length = 1)
        {
            base.OnVoiceCommand(state, source, command, index, length);
            lastVoiceCommand = command;

            if (Host != null)
            {
                if (showVoice != null)
                {
                    Host.StopCoroutine(showVoice);
                    showVoice = null;
                }

                showVoice = Host.StartCoroutine(VoiceTimer(clickTime));

                OnShouldSpawnObject();
            }
        }
    }
}
